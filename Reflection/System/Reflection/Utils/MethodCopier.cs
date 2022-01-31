using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Extensions;

namespace System.Reflection.Utils
{
	internal class MethodCopier
	{
		readonly MethodBodyReader reader;
		readonly List<MethodInfo> transpilers = new List<MethodInfo>();
		internal MethodCopier(MethodBase fromMethod, ILGenerator toILGenerator, LocalBuilder[] existingVariables = null)
		{
			if (fromMethod is null) throw new ArgumentNullException(nameof(fromMethod));
			reader = new MethodBodyReader(fromMethod, toILGenerator);
			reader.DeclareVariables(existingVariables);
			reader.ReadInstructions();
		}
		internal void SetDebugging(bool debug)
		{
			reader.SetDebugging(debug);
		}

		internal void SetArgumentShift(bool useShift)
		{
			reader.SetArgumentShift(useShift);
		}

		internal void AddTranspiler(MethodInfo transpiler)
		{
			transpilers.Add(transpiler);
		}

		internal List<CodeInstruction> Finalize(Emitter emitter, List<Label> endLabels, out bool hasReturnCode)
		{
			return reader.FinalizeILCodes(emitter, transpilers, endLabels, out hasReturnCode);
		}

		internal static List<CodeInstruction> GetInstructions(ILGenerator generator, MethodBase method, int maxTranspilers)
		{
			if (generator is null)
				throw new ArgumentNullException(nameof(generator));
			if (method is null)
				throw new ArgumentNullException(nameof(method));

			var originalVariables = MethodPatcher.DeclareLocalVariables(generator, method);
			var useStructReturnBuffer = StructReturnBuffer.NeedsFix(method);
			var copier = new MethodCopier(method, generator, originalVariables);
			copier.SetArgumentShift(useStructReturnBuffer);

			var info = Runtime.GetPatchInfo(method);
			if (info is object)
			{
				var sortedTranspilers = PatchFunctions.GetSortedPatchMethods(method, info.Transpilers.ToArray());
				for (var i = 0; i < maxTranspilers && i < sortedTranspilers.Count; i++)
					copier.AddTranspiler(sortedTranspilers[i]);
			}

			return copier.Finalize(null, null, out var _);
		}
		internal static void Emit(ILGenerator generator, MethodBase method)
        {
			if (generator is null)
				throw new ArgumentNullException(nameof(generator));
			if (method is null)
				throw new ArgumentNullException(nameof(method));
			var originalVariables = MethodPatcher.DeclareLocalVariables(generator, method);
			var useStructReturnBuffer = StructReturnBuffer.NeedsFix(method);
			var copier = new MethodCopier(method, generator, originalVariables);
			copier.SetArgumentShift(useStructReturnBuffer);
			var ends = new List<Label>();
			copier.Finalize(new Emitter(generator), ends, out var returncode);
			foreach (var lab in ends)
				generator.MarkLabel(lab);
			if (returncode)
				generator.Emit(OpCodes.Ret);
		}
	}

}
