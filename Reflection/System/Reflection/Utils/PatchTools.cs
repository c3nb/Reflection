﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace System.Reflection.Utils
{
	internal static class PatchTools
	{
		// Note: Even though this Dictionary is only stored to and never read from, it still needs to be thread-safe:
		// https://stackoverflow.com/a/33153868
		// ThreadStatic has pitfalls (see RememberObject below), but since we must support net35, it's the best available option.
		[ThreadStatic]
		static Dictionary<object, object> objectReferences;

		internal static void RememberObject(object key, object value)
		{
			// ThreadStatic fields are only initialized for one thread, so ensure it's initialized for current thread.
			objectReferences ??= new Dictionary<object, object>();
			objectReferences[key] = value;
		}

		internal static MethodInfo GetPatchMethod(Type patchType, string attributeName)
		{
			var method = patchType.GetMethods(AccessUtils.all)
				.FirstOrDefault(m => m.GetCustomAttributes(true).Any(a => a.GetType().FullName == attributeName));
			if (method is null)
			{
				// not-found is common and normal case, don't use AccessTools which will generate not-found warnings
				var methodName = attributeName.Replace("System.Reflection.Utils.Runtime", "");
				method = patchType.GetMethod(methodName, AccessUtils.all);
			}
			return method;
		}

		internal static AssemblyBuilder DefineDynamicAssembly(string name)
		{
			var assemblyName = new AssemblyName(name);
#if NETCOREAPP2_0 || NETCOREAPP3_0 || NETCOREAPP3_1 || NETSTANDARD2_0 || NET5_0
			return AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#else
			return AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#endif
		}

		internal static List<AttributePatch> GetPatchMethods(Type type)
		{
			return AccessUtils.GetDeclaredMethods(type)
				.Select(method => AttributePatch.Create(method))
				.Where(attributePatch => attributePatch is object)
				.ToList();
		}

		internal static MethodBase GetOriginalMethod(this RuntimeMethod attr)
		{
			try
			{
				switch (attr.methodType)
				{
					case MethodType.Normal:
						if (attr.methodName is null)
							return null;
						return AccessUtils.DeclaredMethod(attr.declaringType, attr.methodName, attr.argumentTypes);

					case MethodType.Getter:
						if (attr.methodName is null)
							return null;
						return AccessUtils.DeclaredProperty(attr.declaringType, attr.methodName).GetGetMethod(true);

					case MethodType.Setter:
						if (attr.methodName is null)
							return null;
						return AccessUtils.DeclaredProperty(attr.declaringType, attr.methodName).GetSetMethod(true);

					case MethodType.Constructor:
						return AccessUtils.DeclaredConstructor(attr.declaringType, attr.argumentTypes);

					case MethodType.StaticConstructor:
						return AccessUtils.GetDeclaredConstructors(attr.declaringType)
							.Where(c => c.IsStatic)
							.FirstOrDefault();

					case MethodType.Enumerator:
						if (attr.methodName is null)
							return null;
						var method = AccessUtils.DeclaredMethod(attr.declaringType, attr.methodName, attr.argumentTypes);
						return AccessUtils.EnumeratorMoveNext(method);
				}
			}
			catch (AmbiguousMatchException ex)
			{
				throw new Exception($"Ambiguous match for RuntimeMethod[{attr.Description()}]", ex.InnerException ?? ex);
			}

			return null;
		}
	}
}