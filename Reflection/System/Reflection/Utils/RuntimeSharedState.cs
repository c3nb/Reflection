﻿using Mono.Cecil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Extensions;

namespace System.Reflection.Utils
{
	internal static class RuntimeSharedState
	{
		const string name = "RuntimeSharedState";
		internal const int internalVersion = 101; // bumb this if the layout of the RuntimeSharedState type changes

		// state/originals are set to instances stored in the global dynamic types static fields with the same name
		static readonly Dictionary<MethodBase, byte[]> state;
		static readonly Dictionary<MethodInfo, MethodBase> originals;
		internal static readonly int actualVersion;

		static RuntimeSharedState()
		{
			// create singleton type
			var type = GetOrCreateSharedStateType();

			// copy 'actualVersion' over to our fields
			var versionField = type.GetField("version");
			if ((int)versionField.GetValue(null) == 0)
				versionField.SetValue(null, internalVersion);
			actualVersion = (int)versionField.GetValue(null);

			// get or initialize global 'state' field
			var stateField = type.GetField("state");
			if (stateField.GetValue(null) is null)
				stateField.SetValue(null, new Dictionary<MethodBase, byte[]>());

			// get or initialize global 'originals' field
			var originalsField = type.GetField("originals");
			if (originalsField != null && originalsField.GetValue(null) is null)
				originalsField.SetValue(null, new Dictionary<MethodInfo, MethodBase>());

			// copy 'state' over to our fields
			state = (Dictionary<MethodBase, byte[]>)stateField.GetValue(null);

			// copy 'originals' over to our fields
			originals = new Dictionary<MethodInfo, MethodBase>();
			if (originalsField != null) // may not exist in older versions
				originals = (Dictionary<MethodInfo, MethodBase>)originalsField.GetValue(null);

			// newer .NET versions can re-jit methods so we need to patch them after that happens
			DetourHelper.Runtime.OnMethodCompiled += (MethodBase method, IntPtr codeStart, ulong codeLen) =>
			{
				if (method == null) return;
				var info = GetPatchInfo(method);
				if (info == null) return;
				PatchFunctions.UpdateRecompiledMethod(method, codeStart, info);
			};
		}

		// creates a dynamic 'global' type if it does not exist
		static Type GetOrCreateSharedStateType()
		{
			var type = Type.GetType(name, false);
			if (type != null) return type;

			using var module = ModuleDefinition.CreateModule(name, new ModuleParameters() { Kind = ModuleKind.Dll, ReflectionImporterProvider = MMReflectionImporter.Provider });
			var attr = Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Abstract | Mono.Cecil.TypeAttributes.Sealed | Mono.Cecil.TypeAttributes.Class;
			var typedef = new TypeDefinition("", name, attr) { BaseType = module.TypeSystem.Object };
			module.Types.Add(typedef);

			typedef.Fields.Add(new FieldDefinition(
				"state",
				Mono.Cecil.FieldAttributes.Public | Mono.Cecil.FieldAttributes.Static,
				module.ImportReference(typeof(Dictionary<MethodBase, byte[]>))
			));

			typedef.Fields.Add(new FieldDefinition(
				"originals",
				Mono.Cecil.FieldAttributes.Public | Mono.Cecil.FieldAttributes.Static,
				module.ImportReference(typeof(Dictionary<MethodInfo, MethodBase>))
			));

			typedef.Fields.Add(new FieldDefinition(
				"version",
				Mono.Cecil.FieldAttributes.Public | Mono.Cecil.FieldAttributes.Static,
				module.ImportReference(typeof(int))
			));

			return ReflectionHelper.Load(module).GetType(name);
		}

		internal static PatchInfo GetPatchInfo(MethodBase method)
		{
			byte[] bytes;
			lock (state) bytes = state.GetValueSafe(method);
			if (bytes is null) return null;
			return PatchInfoSerialization.Deserialize(bytes);
		}

		internal static IEnumerable<MethodBase> GetPatchedMethods()
		{
			lock (state) return state.Keys.ToArray();
		}

		internal static void UpdatePatchInfo(MethodBase original, MethodInfo replacement, PatchInfo patchInfo)
		{
			var bytes = patchInfo.Serialize();
			lock (state) state[original] = bytes;
			lock (originals) originals[replacement] = original;
		}

		internal static MethodBase GetOriginal(MethodInfo replacement)
		{
			lock (originals) return originals.GetValueSafe(replacement);
		}

		static readonly FieldInfo methodAddress = typeof(StackFrame).GetField("methodAddress", BindingFlags.Instance | BindingFlags.NonPublic);
		internal static MethodBase FindReplacement(StackFrame frame)
		{
			var frameMethod = frame.GetMethod();
			var methodStart = 0L;

			if (frameMethod is null)
			{
				if (methodAddress == null) return null;
				methodStart = (long)methodAddress.GetValue(frame);
			}
			else
			{
				var baseMethod = DetourHelper.Runtime.GetIdentifiable(frameMethod);
				methodStart = baseMethod.GetNativeStart().ToInt64();
			}

			// Failed to find any usable method, returning a null frameMethod means we could not find any method from the stacktrace
			//
			if (methodStart == 0)
				return frameMethod;

			lock (originals) return originals.Keys.FirstOrDefault(replacement => replacement.GetNativeStart().ToInt64() == methodStart);
		}
	}
}
