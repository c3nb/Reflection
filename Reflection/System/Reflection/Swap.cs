using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Extensions;
using System.Reflection.Utils;

namespace System.Reflection
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    public sealed class Swap : MethodAttribute
    {
        public Swap(string methodFullName, int typeIndex = 0) : base(methodFullName, typeIndex) { }
        public Swap(Type type, string name, params Type[] parameterTypes) : base(type, name, parameterTypes) { }
        public Swap(Type type, ConstructorType constructorType, params Type[] parameterTypes) : base(type, constructorType, parameterTypes) { }
        public static void Init(Assembly assembly = null)
        {
            if (Initialized) return;
            foreach (var type in (assembly ?? Assembly.GetCallingAssembly()).GetTypes())
            {
                foreach (var method in type.GetMethods(AccessUtils.all))
                    if (method.GetCustomAttribute(out Swap swap, true))
                    {
                        var origCopy = MethodUtils.Copy(method);
                        var targetCopy = MethodUtils.Copy(swap.Method);
                        MethodUtils.Replace(swap.Method, origCopy);
                        MethodUtils.Replace(method, targetCopy);
                    }
                foreach (var constructor in type.GetConstructors(AccessUtils.all))
                    if (constructor.GetCustomAttribute(out Swap swap, true))
                    {
                        var origCopy = MethodUtils.Copy(constructor);
                        var targetCopy = MethodUtils.Copy(swap.Method);
                        MethodUtils.Replace(swap.Method, origCopy);
                        MethodUtils.Replace(constructor, targetCopy);
                    }
            }
            Initialized = true;
        }
        public static bool Initialized { get; private set; }
    }
}
