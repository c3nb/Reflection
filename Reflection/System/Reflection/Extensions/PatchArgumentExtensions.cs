using System;
using System.Reflection.Emit;
using System.Linq;
using System.Reflection.Utils;

namespace System.Reflection.Extensions
{
	internal static class PatchArgumentExtensions
	{
		static RuntimeArgument[] AllRuntimeArguments(object[] attributes)
		{
			return attributes.Select(attr =>
			{
				if (attr.GetType().Name != nameof(RuntimeArgument)) return null;
				return AccessUtils.MakeDeepCopy<RuntimeArgument>(attr);
			})
			.Where(harg => harg is object)
			.ToArray();
		}

		static RuntimeArgument GetArgumentAttribute(this ParameterInfo parameter)
		{
			var attributes = parameter.GetCustomAttributes(false);
			return AllRuntimeArguments(attributes).FirstOrDefault();
		}

		static RuntimeArgument[] GetArgumentAttributes(this MethodInfo method)
		{
			if (method is null || method is DynamicMethod)
				return default;

			var attributes = method.GetCustomAttributes(false);
			return AllRuntimeArguments(attributes);
		}

		static RuntimeArgument[] GetArgumentAttributes(this Type type)
		{
			var attributes = type.GetCustomAttributes(false);
			return AllRuntimeArguments(attributes);
		}

		static string GetOriginalArgumentName(this ParameterInfo parameter, string[] originalParameterNames)
		{
			var attribute = parameter.GetArgumentAttribute();

			if (attribute is null)
				return null;

			if (string.IsNullOrEmpty(attribute.OriginalName) is false)
				return attribute.OriginalName;

			if (attribute.Index >= 0 && attribute.Index < originalParameterNames.Length)
				return originalParameterNames[attribute.Index];

			return null;
		}

		static string GetOriginalArgumentName(RuntimeArgument[] attributes, string name, string[] originalParameterNames)
		{
			if ((attributes?.Length ?? 0) <= 0)
				return null;

			var attribute = attributes.SingleOrDefault(p => p.NewName == name);
			if (attribute is null)
				return null;

			if (string.IsNullOrEmpty(attribute.OriginalName) is false)
				return attribute.OriginalName;

			if (originalParameterNames is object && attribute.Index >= 0 && attribute.Index < originalParameterNames.Length)
				return originalParameterNames[attribute.Index];

			return null;
		}

		static string GetOriginalArgumentName(this MethodInfo method, string[] originalParameterNames, string name)
		{
			string argumentName;

			argumentName = GetOriginalArgumentName(method?.GetArgumentAttributes(), name, originalParameterNames);
			if (argumentName is object)
				return argumentName;

			argumentName = GetOriginalArgumentName(method?.DeclaringType?.GetArgumentAttributes(), name, originalParameterNames);
			if (argumentName is object)
				return argumentName;

			return name;
		}

		internal static int GetArgumentIndex(this MethodInfo patch, string[] originalParameterNames, ParameterInfo patchParam)
		{
			if (patch is DynamicMethod)
				return Array.IndexOf(originalParameterNames, patchParam.Name);

			var originalName = patchParam.GetOriginalArgumentName(originalParameterNames);
			if (originalName is object)
				return Array.IndexOf(originalParameterNames, originalName);

			originalName = patch.GetOriginalArgumentName(originalParameterNames, patchParam.Name);
			if (originalName is object)
				return Array.IndexOf(originalParameterNames, originalName);

			return -1;
		}
	}
}
