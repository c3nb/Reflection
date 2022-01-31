using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Reflection.Utils
{
	/// <summary>Specifies the type of method</summary>
	///
	public enum MethodType
	{
		/// <summary>This is a normal method</summary>
		Normal,
		/// <summary>This is a getter</summary>
		Getter,
		/// <summary>This is a setter</summary>
		Setter,
		/// <summary>This is a constructor</summary>
		Constructor,
		/// <summary>This is a static constructor</summary>
		StaticConstructor,
		/// <summary>This targets the MoveNext method of the enumerator result</summary>
		Enumerator
	}

	/// <summary>Specifies the type of argument</summary>
	///
	public enum ArgumentType
	{
		/// <summary>This is a normal argument</summary>
		Normal,
		/// <summary>This is a reference argument (ref)</summary>
		Ref,
		/// <summary>This is an out argument (out)</summary>
		Out,
		/// <summary>This is a pointer argument (&amp;)</summary>
		Pointer
	}

	/// <summary>Specifies the type of patch</summary>
	///
	public enum RuntimePatchType
	{
		/// <summary>Any patch</summary>
		All,
		/// <summary>A prefix patch</summary>
		Prefix,
		/// <summary>A postfix patch</summary>
		Postfix,
		/// <summary>A transpiler</summary>
		Transpiler,
		/// <summary>A finalizer</summary>
		Finalizer,
		/// <summary>A reverse patch</summary>
		ReversePatch
	}

	/// <summary>Specifies the type of reverse patch</summary>
	///
	public enum RuntimeReversePatchType
	{
		/// <summary>Use the unmodified original method (directly from IL)</summary>
		Original,
		/// <summary>Use the original as it is right now including previous patches but excluding future ones</summary>
		Snapshot
	}

	/// <summary>Specifies the type of method call dispatching mechanics</summary>
	///
	public enum MethodDispatchType
	{
		/// <summary>Call the method using dynamic dispatching if method is virtual (including overriden)</summary>
		/// <remarks>
		/// <para>
		/// This is the built-in form of late binding (a.k.a. dynamic binding) and is the default dispatching mechanic in C#.
		/// This directly corresponds with the <see cref="System.Reflection.Emit.OpCodes.Callvirt"/> instruction.
		/// </para>
		/// <para>
		/// For virtual (including overriden) methods, the instance type's most-derived/overriden implementation of the method is called.
		/// For non-virtual (including static) methods, same behavior as <see cref="Call"/>: the exact specified method implementation is called.
		/// </para>
		/// <para>
		/// Note: This is not a fully dynamic dispatch, since non-virtual (including static) methods are still called non-virtually.
		/// A fully dynamic dispatch in C# involves using
		/// the <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types#the-dynamic-type"><c>dynamic</c> type</see>
		/// (actually a fully dynamic binding, since even the name and overload resolution happens at runtime), which <see cref="MethodDispatchType"/> does not support.
		/// </para>
		/// </remarks>
		VirtualCall,
		/// <summary>Call the method using static dispatching, regardless of whether method is virtual (including overriden) or non-virtual (including static)</summary>
		/// <remarks>
		/// <para>
		/// a.k.a. non-virtual dispatching, early binding, or static binding.
		/// This directly corresponds with the <see cref="System.Reflection.Emit.OpCodes.Call"/> instruction.
		/// </para>
		/// <para>
		/// For both virtual (including overriden) and non-virtual (including static) methods, the exact specified method implementation is called, without virtual/override mechanics.
		/// </para>
		/// </remarks>
		Call
	}

	/// <summary>The base class for all Runtime annotations (not meant to be used directly)</summary>
	///
	public class RuntimeAttribute : Attribute
	{
		/// <summary>The common information for all attributes</summary>
		public RuntimeMethod info = new RuntimeMethod();
	}

	/// <summary>Annotation to define your Runtime patch methods</summary>
	///
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Delegate | AttributeTargets.Method, AllowMultiple = true)]
	public class RuntimePatch : RuntimeAttribute
	{
		/// <summary>An empty annotation can be used together with TargetMethod(s)</summary>
		///
		public RuntimePatch()
		{
		}

		/// <summary>An annotation that specifies a class to patch</summary>
		/// <param name="declaringType">The declaring class/type</param>
		///
		public RuntimePatch(Type declaringType)
		{
			info.declaringType = declaringType;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="declaringType">The declaring class/type</param>
		/// <param name="argumentTypes">The argument types of the method or constructor to patch</param>
		///
		public RuntimePatch(Type declaringType, Type[] argumentTypes)
		{
			info.declaringType = declaringType;
			info.argumentTypes = argumentTypes;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="declaringType">The declaring class/type</param>
		/// <param name="methodName">The name of the method, property or constructor to patch</param>
		///
		public RuntimePatch(Type declaringType, string methodName)
		{
			info.declaringType = declaringType;
			info.methodName = methodName;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="declaringType">The declaring class/type</param>
		/// <param name="methodName">The name of the method, property or constructor to patch</param>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		///
		public RuntimePatch(Type declaringType, string methodName, params Type[] argumentTypes)
		{
			info.declaringType = declaringType;
			info.methodName = methodName;
			info.argumentTypes = argumentTypes;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="declaringType">The declaring class/type</param>
		/// <param name="methodName">The name of the method, property or constructor to patch</param>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		/// <param name="argumentVariations">Array of <see cref="ArgumentType"/></param>
		///
		public RuntimePatch(Type declaringType, string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations)
		{
			info.declaringType = declaringType;
			info.methodName = methodName;
			ParseSpecialArguments(argumentTypes, argumentVariations);
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="declaringType">The declaring class/type</param>
		/// <param name="methodType">The <see cref="MethodType"/></param>
		///
		public RuntimePatch(Type declaringType, MethodType methodType)
		{
			info.declaringType = declaringType;
			info.methodType = methodType;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="declaringType">The declaring class/type</param>
		/// <param name="methodType">The <see cref="MethodType"/></param>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		///
		public RuntimePatch(Type declaringType, MethodType methodType, params Type[] argumentTypes)
		{
			info.declaringType = declaringType;
			info.methodType = methodType;
			info.argumentTypes = argumentTypes;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="declaringType">The declaring class/type</param>
		/// <param name="methodType">The <see cref="MethodType"/></param>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		/// <param name="argumentVariations">Array of <see cref="ArgumentType"/></param>
		///
		public RuntimePatch(Type declaringType, MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations)
		{
			info.declaringType = declaringType;
			info.methodType = methodType;
			ParseSpecialArguments(argumentTypes, argumentVariations);
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="declaringType">The declaring class/type</param>
		/// <param name="methodName">The name of the method, property or constructor to patch</param>
		/// <param name="methodType">The <see cref="MethodType"/></param>
		///
		public RuntimePatch(Type declaringType, string methodName, MethodType methodType)
		{
			info.declaringType = declaringType;
			info.methodName = methodName;
			info.methodType = methodType;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="methodName">The name of the method, property or constructor to patch</param>
		///
		public RuntimePatch(string methodName)
		{
			info.methodName = methodName;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="methodName">The name of the method, property or constructor to patch</param>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		///
		public RuntimePatch(string methodName, params Type[] argumentTypes)
		{
			info.methodName = methodName;
			info.argumentTypes = argumentTypes;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="methodName">The name of the method, property or constructor to patch</param>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		/// <param name="argumentVariations">An array of <see cref="ArgumentType"/></param>
		///
		public RuntimePatch(string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations)
		{
			info.methodName = methodName;
			ParseSpecialArguments(argumentTypes, argumentVariations);
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="methodName">The name of the method, property or constructor to patch</param>
		/// <param name="methodType">The <see cref="MethodType"/></param>
		///
		public RuntimePatch(string methodName, MethodType methodType)
		{
			info.methodName = methodName;
			info.methodType = methodType;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="methodType">The <see cref="MethodType"/></param>
		///
		public RuntimePatch(MethodType methodType)
		{
			info.methodType = methodType;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="methodType">The <see cref="MethodType"/></param>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		///
		public RuntimePatch(MethodType methodType, params Type[] argumentTypes)
		{
			info.methodType = methodType;
			info.argumentTypes = argumentTypes;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="methodType">The <see cref="MethodType"/></param>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		/// <param name="argumentVariations">An array of <see cref="ArgumentType"/></param>
		///
		public RuntimePatch(MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations)
		{
			info.methodType = methodType;
			ParseSpecialArguments(argumentTypes, argumentVariations);
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		///
		public RuntimePatch(Type[] argumentTypes)
		{
			info.argumentTypes = argumentTypes;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		/// <param name="argumentVariations">An array of <see cref="ArgumentType"/></param>
		///
		public RuntimePatch(Type[] argumentTypes, ArgumentType[] argumentVariations)
		{
			ParseSpecialArguments(argumentTypes, argumentVariations);
		}

		void ParseSpecialArguments(Type[] argumentTypes, ArgumentType[] argumentVariations)
		{
			if (argumentVariations is null || argumentVariations.Length == 0)
			{
				info.argumentTypes = argumentTypes;
				return;
			}

			if (argumentTypes.Length < argumentVariations.Length)
				throw new ArgumentException("argumentVariations contains more elements than argumentTypes", nameof(argumentVariations));

			var types = new List<Type>();
			for (var i = 0; i < argumentTypes.Length; i++)
			{
				var type = argumentTypes[i];
				switch (argumentVariations[i])
				{
					case ArgumentType.Normal:
						break;
					case ArgumentType.Ref:
					case ArgumentType.Out:
						type = type.MakeByRefType();
						break;
					case ArgumentType.Pointer:
						type = type.MakePointerType();
						break;
				}
				types.Add(type);
			}
			info.argumentTypes = types.ToArray();
		}
	}

	/// <summary>Annotation to define the original method for delegate injection</summary>
	///
	[AttributeUsage(AttributeTargets.Delegate, AllowMultiple = true)]
	public class RuntimeDelegate : RuntimePatch
	{
		/// <summary>An annotation that specifies a class to patch</summary>
		/// <param name="declaringType">The declaring class/type</param>
		///
		public RuntimeDelegate(Type declaringType)
			: base(declaringType) { }

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="declaringType">The declaring class/type</param>
		/// <param name="argumentTypes">The argument types of the method or constructor to patch</param>
		///
		public RuntimeDelegate(Type declaringType, Type[] argumentTypes)
			: base(declaringType, argumentTypes) { }

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="declaringType">The declaring class/type</param>
		/// <param name="methodName">The name of the method, property or constructor to patch</param>
		///
		public RuntimeDelegate(Type declaringType, string methodName)
			: base(declaringType, methodName) { }

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="declaringType">The declaring class/type</param>
		/// <param name="methodName">The name of the method, property or constructor to patch</param>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		///
		public RuntimeDelegate(Type declaringType, string methodName, params Type[] argumentTypes)
			: base(declaringType, methodName, argumentTypes) { }

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="declaringType">The declaring class/type</param>
		/// <param name="methodName">The name of the method, property or constructor to patch</param>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		/// <param name="argumentVariations">Array of <see cref="ArgumentType"/></param>
		///
		public RuntimeDelegate(Type declaringType, string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations)
			: base(declaringType, methodName, argumentTypes, argumentVariations) { }

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="declaringType">The declaring class/type</param>
		/// <param name="methodDispatchType">The <see cref="MethodDispatchType"/></param>
		///
		public RuntimeDelegate(Type declaringType, MethodDispatchType methodDispatchType)
			: base(declaringType, MethodType.Normal)
		{
			info.nonVirtualDelegate = methodDispatchType == MethodDispatchType.Call;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="declaringType">The declaring class/type</param>
		/// <param name="methodDispatchType">The <see cref="MethodDispatchType"/></param>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		///
		public RuntimeDelegate(Type declaringType, MethodDispatchType methodDispatchType, params Type[] argumentTypes)
			: base(declaringType, MethodType.Normal, argumentTypes)
		{
			info.nonVirtualDelegate = methodDispatchType == MethodDispatchType.Call;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="declaringType">The declaring class/type</param>
		/// <param name="methodDispatchType">The <see cref="MethodDispatchType"/></param>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		/// <param name="argumentVariations">Array of <see cref="ArgumentType"/></param>
		///
		public RuntimeDelegate(Type declaringType, MethodDispatchType methodDispatchType, Type[] argumentTypes, ArgumentType[] argumentVariations)
			: base(declaringType, MethodType.Normal, argumentTypes, argumentVariations)
		{
			info.nonVirtualDelegate = methodDispatchType == MethodDispatchType.Call;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="declaringType">The declaring class/type</param>
		/// <param name="methodName">The name of the method, property or constructor to patch</param>
		/// <param name="methodDispatchType">The <see cref="MethodDispatchType"/></param>
		///
		public RuntimeDelegate(Type declaringType, string methodName, MethodDispatchType methodDispatchType)
			: base(declaringType, methodName, MethodType.Normal)
		{
			info.nonVirtualDelegate = methodDispatchType == MethodDispatchType.Call;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="methodName">The name of the method, property or constructor to patch</param>
		///
		public RuntimeDelegate(string methodName)
			: base(methodName) { }

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="methodName">The name of the method, property or constructor to patch</param>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		///
		public RuntimeDelegate(string methodName, params Type[] argumentTypes)
			: base(methodName, argumentTypes) { }

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="methodName">The name of the method, property or constructor to patch</param>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		/// <param name="argumentVariations">An array of <see cref="ArgumentType"/></param>
		///
		public RuntimeDelegate(string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations)
			: base(methodName, argumentTypes, argumentVariations) { }

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="methodName">The name of the method, property or constructor to patch</param>
		/// <param name="methodDispatchType">The <see cref="MethodDispatchType"/></param>
		///
		public RuntimeDelegate(string methodName, MethodDispatchType methodDispatchType)
			: base(methodName, MethodType.Normal)
		{
			info.nonVirtualDelegate = methodDispatchType == MethodDispatchType.Call;
		}

		/// <summary>An annotation that specifies call dispatching mechanics for the delegate</summary>
		/// <param name="methodDispatchType">The <see cref="MethodDispatchType"/></param>
		///
		public RuntimeDelegate(MethodDispatchType methodDispatchType)
		{
			info.nonVirtualDelegate = methodDispatchType == MethodDispatchType.Call;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="methodDispatchType">The <see cref="MethodDispatchType"/></param>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		///
		public RuntimeDelegate(MethodDispatchType methodDispatchType, params Type[] argumentTypes)
			: base(MethodType.Normal, argumentTypes)
		{
			info.nonVirtualDelegate = methodDispatchType == MethodDispatchType.Call;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="methodDispatchType">The <see cref="MethodDispatchType"/></param>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		/// <param name="argumentVariations">An array of <see cref="ArgumentType"/></param>
		///
		public RuntimeDelegate(MethodDispatchType methodDispatchType, Type[] argumentTypes, ArgumentType[] argumentVariations)
			: base(MethodType.Normal, argumentTypes, argumentVariations)
		{
			info.nonVirtualDelegate = methodDispatchType == MethodDispatchType.Call;
		}

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		///
		public RuntimeDelegate(Type[] argumentTypes)
			: base(argumentTypes) { }

		/// <summary>An annotation that specifies a method, property or constructor to patch</summary>
		/// <param name="argumentTypes">An array of argument types to target overloads</param>
		/// <param name="argumentVariations">An array of <see cref="ArgumentType"/></param>
		///
		public RuntimeDelegate(Type[] argumentTypes, ArgumentType[] argumentVariations)
			: base(argumentTypes, argumentVariations) { }
	}

	/// <summary>Annotation to define your standin methods for reverse patching</summary>
	///
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class RuntimeReversePatch : RuntimeAttribute
	{
		/// <summary>An annotation that specifies the type of reverse patching</summary>
		/// <param name="type">The <see cref="RuntimeReversePatchType"/> of the reverse patch</param>
		///
		public RuntimeReversePatch(RuntimeReversePatchType type = RuntimeReversePatchType.Original)
		{
			info.reversePatchType = type;
		}
	}

	/// <summary>A Runtime annotation to define that all methods in a class are to be patched</summary>
	///
	[AttributeUsage(AttributeTargets.Class)]
	public class RuntimePatchAll : RuntimeAttribute
	{
	}

	/// <summary>A Runtime annotation</summary>
	///
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class RuntimePriority : RuntimeAttribute
	{
		/// <summary>A Runtime annotation to define patch priority</summary>
		/// <param name="priority">The priority</param>
		///
		public RuntimePriority(int priority)
		{
			info.priority = priority;
		}
	}

	/// <summary>A Runtime annotation</summary>
	///
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class RuntimeBefore : RuntimeAttribute
	{
		/// <summary>A Runtime annotation to define that a patch comes before another patch</summary>
		/// <param name="before">The array of runtime IDs of the other patches</param>
		///
		public RuntimeBefore(params string[] before)
		{
			info.before = before;
		}
	}

	/// <summary>A Runtime annotation</summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class RuntimeAfter : RuntimeAttribute
	{
		/// <summary>A Runtime annotation to define that a patch comes after another patch</summary>
		/// <param name="after">The array of runtime IDs of the other patches</param>
		///
		public RuntimeAfter(params string[] after)
		{
			info.after = after;
		}
	}

	/// <summary>A Runtime annotation</summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class RuntimeDebug : RuntimeAttribute
	{
		/// <summary>A Runtime annotation to debug a patch (output uses <see cref="FileLog"/> to log to your Desktop)</summary>
		///
		public RuntimeDebug()
		{
			info.debug = true;
		}
	}

	/// <summary>Specifies the Prepare function in a patch class</summary>
	///
	[AttributeUsage(AttributeTargets.Method)]
	public class RuntimePrepare : Attribute
	{
	}

	/// <summary>Specifies the Cleanup function in a patch class</summary>
	///
	[AttributeUsage(AttributeTargets.Method)]
	public class RuntimeCleanup : Attribute
	{
	}

	/// <summary>Specifies the TargetMethod function in a patch class</summary>
	///
	[AttributeUsage(AttributeTargets.Method)]
	public class RuntimeTargetMethod : Attribute
	{
	}

	/// <summary>Specifies the TargetMethods function in a patch class</summary>
	///
	[AttributeUsage(AttributeTargets.Method)]
	public class RuntimeTargetMethods : Attribute
	{
	}

	/// <summary>Specifies the Prefix function in a patch class</summary>
	///
	[AttributeUsage(AttributeTargets.Method)]
	public class RuntimePrefix : Attribute
	{
	}

	/// <summary>Specifies the Postfix function in a patch class</summary>
	///
	[AttributeUsage(AttributeTargets.Method)]
	public class RuntimePostfix : Attribute
	{
	}

	/// <summary>Specifies the Transpiler function in a patch class</summary>
	///
	[AttributeUsage(AttributeTargets.Method)]
	public class RuntimeTranspiler : Attribute
	{
	}

	/// <summary>Specifies the Finalizer function in a patch class</summary>
	///
	[AttributeUsage(AttributeTargets.Method)]
	public class RuntimeFinalizer : Attribute
	{
	}

	/// <summary>A Runtime annotation</summary>
	///
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
	public class RuntimeArgument : Attribute
	{
		/// <summary>The name of the original argument</summary>
		///
		public string OriginalName { get; private set; }

		/// <summary>The index of the original argument</summary>
		///
		public int Index { get; private set; }

		/// <summary>The new name of the original argument</summary>
		///
		public string NewName { get; private set; }

		/// <summary>An annotation to declare injected arguments by name</summary>
		///
		public RuntimeArgument(string originalName) : this(originalName, null)
		{
		}

		/// <summary>An annotation to declare injected arguments by index</summary>
		/// <param name="index">Zero-based index</param>
		///
		public RuntimeArgument(int index) : this(index, null)
		{
		}

		/// <summary>An annotation to declare injected arguments by renaming them</summary>
		/// <param name="originalName">Name of the original argument</param>
		/// <param name="newName">New name</param>
		///
		public RuntimeArgument(string originalName, string newName)
		{
			OriginalName = originalName;
			Index = -1;
			NewName = newName;
		}

		/// <summary>An annotation to declare injected arguments by index and renaming them</summary>
		/// <param name="index">Zero-based index</param>
		/// <param name="name">New name</param>
		///
		public RuntimeArgument(int index, string name)
		{
			OriginalName = null;
			Index = index;
			NewName = name;
		}
	}
}
