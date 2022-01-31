using System.Reflection.Utils;
using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = true)]
    public sealed class To : MethodAttribute
    {
        public To(string methodFullName, int typeIndex = 0) : base(methodFullName, typeIndex) { }
        public To(Type type, string name, params Type[] parameterTypes) : base(type, name, parameterTypes) { }
        public To(Type type, ConstructorType constructorType, params Type[] parameterTypes) : base(type, constructorType, parameterTypes) { }
        public static void Init(Assembly assembly = null)
        {
            if (Initialized) return;
            foreach (var type in (assembly ?? Assembly.GetCallingAssembly()).GetTypes())
            {
                foreach (var method in type.GetMethods(AccessUtils.all))
                    method.GetCustomAttributes<To>(true).ForEach(to => MethodUtils.Replace(to.Method, method));
                foreach (var constructor in type.GetConstructors(AccessUtils.all))
                    constructor.GetCustomAttributes<To>(true).ForEach(to => MethodUtils.Replace(to.Method, constructor));
            }
            Initialized = true;
        }
        public static bool Initialized { get; private set; }
    }
}
