using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Reflection.Utils
{
	/// <summary>A reverse patcher</summary>
	/// 
	public class ReversePatcher
	{
		readonly Runtime instance;
		readonly MethodBase original;
		readonly RuntimeMethod standin;

		/// <summary>Creates a reverse patcher</summary>
		/// <param name="instance">The Runtime instance</param>
		/// <param name="original">The original method/constructor</param>
		/// <param name="standin">Your stand-in stub method as <see cref="RuntimeMethod"/></param>
		///
		public ReversePatcher(Runtime instance, MethodBase original, RuntimeMethod standin)
		{
			this.instance = instance;
			this.original = original;
			this.standin = standin;
		}

		/// <summary>Applies the patch</summary>
		/// <param name="type">The type of patch, see <see cref="RuntimeReversePatchType"/></param>
		/// <returns>The generated replacement method</returns>
		///
		public MethodInfo Patch(RuntimeReversePatchType type = RuntimeReversePatchType.Original)
		{
			if (original is null)
				throw new NullReferenceException($"Null method for {instance.Id}");

			standin.reversePatchType = type;
			var transpiler = GetTranspiler(standin.method);
			return PatchFunctions.ReversePatch(standin, original, transpiler);
		}

		internal static MethodInfo GetTranspiler(MethodInfo method)
		{
			var methodName = method.Name;
			var type = method.DeclaringType;
			var methods = AccessUtils.GetDeclaredMethods(type);
			var ici = typeof(IEnumerable<CodeInstruction>);
			return methods.FirstOrDefault(m =>
			{
				if (m.ReturnType != ici) return false;
				return m.Name.StartsWith($"<{methodName }>");
			});
		}
	}
}
