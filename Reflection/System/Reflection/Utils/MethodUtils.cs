using System.Linq;
using System.Collections.Generic;
using System.Reflection.Extensions;
using System.Reflection.Emit;
using MonoMod.Utils;
using Mono.Cecil;

namespace System.Reflection.Utils
{
    public static class MethodUtils
    {
		public static Runtime Runtime = new Runtime("MethodUtilsStaticRuntime");
        public static void Replace(MethodBase target, MethodBase dest)
            => Memory.DetourMethod(target, dest);
        public static MethodInfo Copy(MethodBase method)
		    => Runtime.Patch(method);
    }
}
