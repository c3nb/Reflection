using System.Reflection.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Extensions;

namespace System.Reflection
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    public sealed class From : MethodAttribute
    {
        public From(string methodFullName, int typeIndex = 0) : base(methodFullName, typeIndex) { }
        public From(Type type, string name, params Type[] parameterTypes) : base(type, name, parameterTypes) { }
        public From(Type type, ConstructorType constructorType, params Type[] parameterTypes) : base(type, constructorType, parameterTypes) { }
        public static void Init(Assembly assembly = null)
        {
            if (Initialized) return;
            foreach (var type in (assembly ?? Assembly.GetCallingAssembly()).GetTypes())
            {
                foreach (var method in type.GetMethods(AccessUtils.all))
                    if (method.GetCustomAttribute(out From from, true))
                        MethodUtils.Replace(method, from.Method);
                foreach (var constructor in type.GetConstructors(AccessUtils.all))
                    if (constructor.GetCustomAttribute(out From from, true))
                        MethodUtils.Replace(constructor, from.Method);
            }
            Initialized = true;
        }
        public static bool Initialized { get; private set; }
    }
}
