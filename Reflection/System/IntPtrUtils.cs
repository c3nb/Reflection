using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection.Extensions;
using System.Reflection;

namespace System
{
    public static class IntPtrUtils
    {
        public static unsafe IntPtr Write<T>(this IntPtr ptr, T value, int offset = 0)
        {
            // Declare Local T* (Generic Pointer)
            //ldarg.0
            //ldarg.2
            //call native int[mscorlib] System.IntPtr::op_Addition(native int, int32)
            //call void*[mscorlib] System.IntPtr::op_Explicit(native int)
            //stloc.0
            //ldloc.0
            //ldarg.1
            //stobj !!T
            //ldloc.0
            //sizeof !!T
            //call native int[mscorlib] System.IntPtr::op_Addition(native int, int32)
            //ldarg.2
            //call native int[mscorlib] System.IntPtr::op_Addition(native int, int32)
            //ret
            //Emit this instructions via dnspy.
            throw null;
        }
        public static unsafe IntPtr Read<T>(this IntPtr ptr, out T value, int offset = 0)
        {
            // Declare Local T* (Generic Pointer)
            //ldarg.0
            //ldarg.2
            //call native int[mscorlib] System.IntPtr::op_Addition(native int, int32)
            //call void*[mscorlib] System.IntPtr::op_Explicit(native int)
            //stloc.0
            //ldarg.1
            //ldloc.0
            //ldobj !!T
            //stobj !!T
            //ldloc.0
            //sizeof !!T
            //call native int[mscorlib] System.IntPtr::op_Addition(native int, int32)
            //ldarg.2
            //call native int[mscorlib] System.IntPtr::op_Addition(native int, int32)
            //ret
            //Emit this instructions via dnspy.
            throw null;
        }
        public static T Read<T>(this IntPtr ptr, int offset = 0)
        {
            Read(ptr, out T value, offset);
            return value;
        }
        public static unsafe IntPtr WriteTr<T>(this IntPtr ptr, T value, int offset = 0)
        {
            int size = Reflection.Extensions.Reflection.SizeOf<T>();
            byte* bytePtr = (byte*)(ptr + offset);
            TypedReference valueref = __makeref(value);
            byte* valuePtr = (byte*)*(IntPtr*)&valueref;
            for (int i = 0; i < size; ++i)
                bytePtr[i] = valuePtr[i];
            return (IntPtr)(bytePtr + size);
        }
        public static unsafe IntPtr ReadTr<T>(this IntPtr ptr, out T value, int offset = 0)
        {
            int size = Reflection.Extensions.Reflection.SizeOf<T>();
            byte* bytePtr = (byte*)(ptr + offset);
            T result = default(T);
            TypedReference resultRef = __makeref(result);
            byte* resultPtr = (byte*)*(IntPtr*)&resultRef;
            for (int i = 0; i < size; ++i)
                resultPtr[i] = bytePtr[i];
            value = result;
            return (IntPtr)(bytePtr + size);
        }
        public static T ReadTr<T>(this IntPtr ptr, int offset = 0)
        {
            ReadTr(ptr, out T value, offset);
            return value;
        }
        public static IntPtr<T> Cast<T>(this IntPtr ptr) => ptr;
        public static unsafe IntPtr GetPointer<T>(ref T obj)
        {
            //ldarg.0
            //conv.u
            //ret
            //Emit this instructions via dnspy.
            throw null;
        }
        public static unsafe IntPtr GetPointerTr<T>(ref T obj)
        {
            TypedReference tr = __makeref(obj);
            return *(IntPtr*)&tr;
        }
        public static unsafe IntPtr GetPointer(FieldInfo field, object instance = null)
            => (IntPtr)getPtr.GetGenericMethodDefinition().MakeGenericMethod(field.FieldType).Invoke(null, new object[] { field, instance });
        internal static unsafe IntPtr GetPointer<T>(FieldInfo field, object instance = null)
        {
            if (cache.TryGetValue(field, out Delegate getter))
                return GetPointer(ref ((__ValueFromFieldInfo<T>)getter)(instance));
            if (!field.IsStatic && instance == null)
                throw new Exception("field is not static. instance is required.");
            DynamicMethod dynamicMethod = new DynamicMethod($"__AddressGetter{field.DeclaringType}_{field.Name}", field.FieldType, new[] { typeof(object) }, true);
            dynamicMethod.GetType().GetField("m_returnType", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).SetValue(dynamicMethod, field.FieldType.MakeByRefType());
            ILGenerator il = dynamicMethod.GetILGenerator();
            if (field.IsStatic)
                il.Ldsflda(field).Ret();
            else il.Ldarg_0().Unbox_Any(field.DeclaringType).Ldflda(field).Ret();
            dynamicMethod.Compile();
            __ValueFromFieldInfo<T> result = (__ValueFromFieldInfo<T>)dynamicMethod.CreateDelegate(typeof(__ValueFromFieldInfo<T>));
            cache.Add(field, result);
            return GetPointer(ref result(instance));
        }
        private delegate ref TResult __ValueFromFieldInfo<TResult>(object instance);
        private static readonly Dictionary<FieldInfo, Delegate> cache = new Dictionary<FieldInfo, Delegate>();
        private static MethodInfo getPtr = typeof(IntPtrUtils).GetMethod("GetPointer", BindingFlags.NonPublic | BindingFlags.Static);
    }
}
