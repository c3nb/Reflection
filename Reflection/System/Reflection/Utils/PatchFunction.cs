using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Extensions;

namespace System.Reflection.Utils
{
	internal static class PatchFunctions
	{
		/// <summary>Sorts patch methods by their priority rules</summary>
		/// <param name="original">The original method</param>
		/// <param name="patches">Patches to sort</param>
		/// <param name="debug">Use debug mode</param>
		/// <returns>The sorted patch methods</returns>
		///
		internal static List<MethodInfo> GetSortedPatchMethods(MethodBase original, Patch[] patches)
		{
			return new PatchSorter(patches).Sort(original);
		}

		/// <summary>Creates new replacement method with the latest patches and detours the original method</summary>
		/// <param name="original">The original method</param>
		/// <param name="patchInfo">Information describing the patches</param>
		/// <returns>The newly created replacement method</returns>
		///
		internal static MethodInfo UpdateWrapper(MethodBase original, PatchInfo patchInfo)
		{
			var sortedPrefixes = GetSortedPatchMethods(original, patchInfo.prefixes);
			var sortedPostfixes = GetSortedPatchMethods(original, patchInfo.postfixes);
			var sortedTranspilers = GetSortedPatchMethods(original, patchInfo.transpilers);
			var sortedFinalizers = GetSortedPatchMethods(original, patchInfo.finalizers);

			var patcher = new MethodPatcher(original, null, sortedPrefixes, sortedPostfixes, sortedTranspilers, sortedFinalizers);
			var replacement = patcher.CreateReplacement(out var finalInstructions);
			if (replacement is null) throw new MissingMethodException($"Cannot create replacement for {original.FullDescription()}");

			try
			{
				Memory.DetourMethodAndPersist(original, replacement);
			}
			catch (Exception ex)
			{
				throw RuntimeException.Create(ex, finalInstructions);
			}
			return replacement;
		}

		internal static void UpdateRecompiledMethod(MethodBase original, IntPtr codeStart, PatchInfo patchInfo)
		{
			try
			{
				var sortedPrefixes = GetSortedPatchMethods(original, patchInfo.prefixes);
				var sortedPostfixes = GetSortedPatchMethods(original, patchInfo.postfixes);
				var sortedTranspilers = GetSortedPatchMethods(original, patchInfo.transpilers);
				var sortedFinalizers = GetSortedPatchMethods(original, patchInfo.finalizers);

				var patcher = new MethodPatcher(original, null, sortedPrefixes, sortedPostfixes, sortedTranspilers, sortedFinalizers);
				var replacement = patcher.CreateReplacement(out var finalInstructions);
				if (replacement is null) throw new MissingMethodException($"Cannot create replacement for {original.FullDescription()}");

				Memory.DetourCompiledMethod(codeStart, replacement);
			}
			catch
			{
			}
		}

		internal static MethodInfo ReversePatch(RuntimeMethod standin, MethodBase original, MethodInfo postTranspiler)
		{
			if (standin is null)
				throw new ArgumentNullException(nameof(standin));
			if (standin.method is null)
				throw new ArgumentNullException(nameof(standin), $"{nameof(standin)}.{nameof(standin.method)} is NULL");

			var transpilers = new List<MethodInfo>();
			if (standin.reversePatchType == RuntimeReversePatchType.Snapshot)
			{
				var info = Runtime.GetPatchInfo(original);
				transpilers.AddRange(GetSortedPatchMethods(original, info.Transpilers.ToArray()));
			}
			if (postTranspiler is object) transpilers.Add(postTranspiler);

			var empty = new List<MethodInfo>();
			var patcher = new MethodPatcher(standin.method, original, empty, empty, transpilers, empty);
			var replacement = patcher.CreateReplacement(out var finalInstructions);
			if (replacement is null) throw new MissingMethodException($"Cannot create replacement for {standin.method.FullDescription()}");

			try
			{
				var errorString = Memory.DetourMethod(standin.method, replacement);
				if (errorString is object)
					throw new FormatException($"Method {standin.method.FullDescription()} cannot be patched. Reason: {errorString}");
			}
			catch (Exception ex)
			{
				throw RuntimeException.Create(ex, finalInstructions);
			}

			PatchTools.RememberObject(standin.method, replacement);
			return replacement;
		}
	}
}
