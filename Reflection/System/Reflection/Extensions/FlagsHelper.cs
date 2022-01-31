using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Reflection.Extensions
{
    public static class FlagsHelper
    {
        public static bool IsSet<T>(this T flags, T flag)
        {
            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;
            return (flagsValue & flagValue) != 0;
        }
        public static T Set<T>(this T flags, T flag)
        {
            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;
            return (T)(object)(flagsValue | flagValue);
        }
        public static T Unset<T>(this T flags, T flag)
        {
            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;
            return (T)(object)(flagsValue & (~flagValue));
        }
    }
}
