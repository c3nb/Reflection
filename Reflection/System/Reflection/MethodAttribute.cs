#define CODE_ANALYSIS
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Utils;

namespace System.Reflection
{
    public enum ConstructorType
    {
        Static,
        Instance
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = true)]
    public class MethodAttribute : Attribute
    {
        static MethodAttribute()
        {
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()))
            {
                if (!Types.TryGetValue(type.FullName, out var list))
                    list.Add(type);
                else
                    Types.Add(type.FullName, new List<Type>() { type });
            }
        }
        public static Dictionary<string, List<Type>> Types = new Dictionary<string, List<Type>>();
        public MethodAttribute(string methodFullName, int typeIndex = 0)
        {
            var split = methodFullName.Split('.');
            var paramBraces = (string)null;
            if (methodFullName.Contains("("))
                split = methodFullName.Replace(paramBraces = methodFullName.Substring(methodFullName.IndexOf('(')), "").Split('.');
            var method = split.Last();
            var type = methodFullName.Replace($".{(method.Contains("ctor") ? $".{method}" : method)}{paramBraces}", "");
            var isParam = false;
            var parameterTypes = new List<Type>();
            if (paramBraces != null)
            {
                isParam = true;
                var parametersString = paramBraces.Replace("(", "").Replace(")", "");
                if (string.IsNullOrWhiteSpace(parametersString))
                    goto Skip;
                var parameterSplit = parametersString.Split(',');
                parameterTypes = parameterSplit.Select(s => Type.GetType(s)).ToList();
            }
        Skip:
            var decType = Types[type][typeIndex];
            var parameterArr = parameterTypes.ToArray();
            var result = (MethodBase)null;
            if (method == "ctor")
                result = decType.GetConstructor(AccessUtils.all, null, parameterArr, null);
            else if (method == "cctor")
                result = decType.TypeInitializer;
            else
                result = isParam ? decType.GetMethod(method, AccessUtils.all, null, parameterTypes.ToArray(), null) : decType.GetMethod(method, AccessUtils.all);
            if (result == null)
                throw new NullReferenceException("Cannot find method!");
            Method = result;
        }
        public MethodAttribute(Type type, string methodName, params Type[] parameterTypes)
        {
            var isParam = parameterTypes.Length > 0;
            var result = isParam ? type.GetMethod(methodName, AccessUtils.all, null, parameterTypes, null) : type.GetMethod(methodName, AccessUtils.all);
            if (result == null)
                throw new NullReferenceException("Cannot find method!");
            Method = result;
        }
        public MethodAttribute(Type type, ConstructorType constructorType, params Type[] parameterTypes)
        {
            if (constructorType == ConstructorType.Instance)
                Method = type.GetConstructor(AccessUtils.all, null, parameterTypes, null);
            else
                Method = type.TypeInitializer;
        }
        public MethodBase Method { get; protected set; }
    }
}
