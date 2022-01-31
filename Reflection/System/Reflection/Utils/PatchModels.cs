using System.Collections.Generic;
using System.Linq;
using System.Reflection.Extensions;

namespace System.Reflection.Utils
{
	// PatchJobs holds the information during correlation
	// of methods and patches while processing attribute patches
	//
	internal class PatchJobs<T>
	{
		internal class Job
		{
			internal MethodBase original;
			internal T replacement;
			internal List<RuntimeMethod> prefixes = new List<RuntimeMethod>();
			internal List<RuntimeMethod> postfixes = new List<RuntimeMethod>();
			internal List<RuntimeMethod> transpilers = new List<RuntimeMethod>();
			internal List<RuntimeMethod> finalizers = new List<RuntimeMethod>();

			internal void AddPatch(AttributePatch patch)
			{
				switch (patch.type)
				{
					case RuntimePatchType.Prefix:
						prefixes.Add(patch.info);
						break;
					case RuntimePatchType.Postfix:
						postfixes.Add(patch.info);
						break;
					case RuntimePatchType.Transpiler:
						transpilers.Add(patch.info);
						break;
					case RuntimePatchType.Finalizer:
						finalizers.Add(patch.info);
						break;
				}
			}
		}

		internal Dictionary<MethodBase, Job> state = new Dictionary<MethodBase, Job>();

		internal Job GetJob(MethodBase method)
		{
			if (method is null) return null;
			if (state.TryGetValue(method, out var job) is false)
			{
				job = new Job() { original = method };
				state[method] = job;
			}
			return job;
		}

		internal List<Job> GetJobs()
		{
			return state.Values.Where(job =>
				job.prefixes.Count +
				job.postfixes.Count +
				job.transpilers.Count +
				job.finalizers.Count > 0
			).ToList();
		}

		internal List<T> GetReplacements()
		{
			return state.Values.Select(job => job.replacement).ToList();
		}
	}

	// AttributePatch contains all information for a patch defined by attributes
	//
	internal class AttributePatch
	{
		static readonly RuntimePatchType[] allPatchTypes = new[] {
			RuntimePatchType.Prefix,
			RuntimePatchType.Postfix,
			RuntimePatchType.Transpiler,
			RuntimePatchType.Finalizer,
			RuntimePatchType.ReversePatch,
		};

		internal RuntimeMethod info;
		internal RuntimePatchType? type;

		static readonly string runtimeAttributeName = typeof(RuntimeAttribute).FullName;
		internal static AttributePatch Create(MethodInfo patch)
		{
			if (patch is null)
				throw new NullReferenceException("Patch method cannot be null");

			var allAttributes = patch.GetCustomAttributes(true);
			var methodName = patch.Name;
			var type = GetPatchType(methodName, allAttributes);
			if (type is null)
				return null;

			if (type != RuntimePatchType.ReversePatch && patch.IsStatic is false)
				throw new ArgumentException("Patch method " + patch.FullDescription() + " must be static");

			var list = allAttributes
				.Where(attr => attr.GetType().BaseType.FullName == runtimeAttributeName)
				.Select(attr =>
				{
					var f_info = AccessUtils.Field(attr.GetType(), nameof(RuntimeAttribute.info));
					return f_info.GetValue(attr);
				})
				.Select(runtimeInfo => AccessUtils.MakeDeepCopy<RuntimeMethod>(runtimeInfo))
				.ToList();
			var info = RuntimeMethod.Merge(list);
			info.method = patch;

			return new AttributePatch() { info = info, type = type };
		}

		static RuntimePatchType? GetPatchType(string methodName, object[] allAttributes)
		{
			var runtimeAttributes = new HashSet<string>(allAttributes
				.Select(attr => attr.GetType().FullName)
				.Where(name => name.StartsWith("System.Reflection.Utils.Runtime")));

			RuntimePatchType? type = null;
			foreach (var patchType in allPatchTypes)
			{
				var name = patchType.ToString();
				if (name == methodName || runtimeAttributes.Contains($"System.Reflection.Utils.Runtime{name}"))
				{
					type = patchType;
					break;
				}
			}
			return type;
		}
	}
}
