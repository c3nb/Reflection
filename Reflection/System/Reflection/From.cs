using System.Reflection.Utils;
using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = true)]
    public sealed class From : Attribute
    {
        public From(string methodFullName)
        {
            var split = methodFullName.Split('.');
            var paramBraces = (string)null;
            if (methodFullName.Contains("("))
                split = methodFullName.Replace(paramBraces = methodFullName.Substring(methodFullName.IndexOf('(')), "").Split('.');
            var method = split.Last();
            var type = methodFullName.Replace($".{method}{paramBraces}", "");
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
            var result = isParam ? Type.GetType(type).GetMethod(method, AccessUtils.all, null, parameterTypes.ToArray(), null) : Type.GetType(type).GetMethod(method, AccessUtils.all);
            if (result == null)
                throw new NullReferenceException("Cannot find method!");
            if (!result.IsStatic)
                throw new InvalidOperationException("Method must be static!");
            Method = result;
        }
        public From(Type type, string methodName, params Type[] parameterTypes)
        {
            var isParam = parameterTypes.Length > 0;
            var result = isParam ? type.GetMethod(methodName, AccessUtils.all, null, parameterTypes, null) : type.GetMethod(methodName, AccessUtils.all);
            if (result == null)
                throw new NullReferenceException("Cannot find method!");
            if (!result.IsStatic)
                throw new InvalidOperationException("Method must be static!");
            Method = result;
        }
        public MethodInfo Method { get; private set; }
        public static void Init(Assembly assembly = null)
        {
            if (Initialized) return;
            foreach (var type in (assembly ?? Assembly.GetCallingAssembly()).GetTypes())
            {
                foreach (var method in type.GetMethods(AccessUtils.all))
                    method.GetCustomAttributes<From>(true).ForEach(from => MethodUtils.Replace(method, from.Method));
                foreach (var constructor in type.GetConstructors(AccessUtils.all))
                    constructor.GetCustomAttributes<From>(true).ForEach(from => MethodUtils.Replace(constructor, from.Method));
            }
            Initialized = true;
        }
        public static bool Initialized { get; private set; }
    }
}
