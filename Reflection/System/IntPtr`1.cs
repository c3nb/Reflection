using System.Runtime.Serialization;
using System.Reflection;

namespace System
{
    [Serializable]
    public struct IntPtr<T> : ISerializable, IEquatable<IntPtr<T>>, IComparable<IntPtr<T>>, ICloneable
    {
        public IntPtr Inner { get; set; }
        public T Value
        {
            get => Read();
            set => Write(value);
        }
        public T Read(int offset = 0) => Inner.Read<T>(offset);
        public IntPtr<T> Read(out T value, int offset = 0) => Inner.Read(out value, offset);
        public IntPtr<T> Write(T value, int offset = 0) => Inner.Write(value, offset);
        public IntPtr<To> Cast<To>() => Inner;
        public IntPtr(IntPtr ptr)
        {
            Inner = ptr;
        }
        public unsafe IntPtr(void* value)
        {
            Inner = new IntPtr(value);
        }
        public unsafe override bool Equals(object obj) => Inner.Equals(obj);
        public unsafe override int GetHashCode() => Inner.GetHashCode();
        public unsafe int ToInt32() => Inner.ToInt32();
        public unsafe long ToInt64() => Inner.ToInt64();
        public unsafe override string ToString() => Inner.ToString();
        public unsafe string ToString(string format) => Inner.ToString(format);
        public static IntPtr<T> Create(ref T obj) => IntPtrUtils.GetPointer(ref obj);
        public static IntPtr<T> Create(FieldInfo field, object instance = null) => IntPtrUtils.GetPointer<T>(field, instance);
        public static implicit operator IntPtr<T>(IntPtr ptr) => new IntPtr<T>(ptr);
        public static implicit operator IntPtr(IntPtr<T> ptr) => ptr.Inner;
        public static implicit operator T(IntPtr<T> ptr) => ptr.Value;
        public static explicit operator IntPtr<T>(int value) => (IntPtr)value;
        public static explicit operator IntPtr<T>(long value) => (IntPtr)value;
        public static unsafe explicit operator IntPtr<T>(void* value) => (IntPtr)value;
        public static unsafe explicit operator void*(IntPtr<T> value) => (void*)value.Inner;
        public unsafe static explicit operator int(IntPtr<T> value) => (int)value.Inner;
        public unsafe static explicit operator long(IntPtr<T> value) => (long)value.Inner;
        public unsafe static bool operator ==(IntPtr<T> value1, IntPtr<T> value2) => value1.Inner == value2.Inner;
        public unsafe static bool operator !=(IntPtr<T> value1, IntPtr<T> value2) => value1.Inner != value2.Inner;
        public static IntPtr<T> Add(IntPtr<T> pointer, int offset) => pointer.Inner + offset;
        public static IntPtr<T> operator +(IntPtr<T> pointer, int offset) => pointer.Inner + offset;
        public static IntPtr<T> Subtract(IntPtr<T> pointer, int offset) => pointer.Inner - offset;
        public static IntPtr<T> operator -(IntPtr<T> pointer, int offset) => pointer.Inner - offset;
        public static int Size => IntPtr.Size;
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) => ((ISerializable)Inner).GetObjectData(info, context);
        public unsafe void* ToPointer() => Inner.ToPointer();
        public bool Equals(IntPtr<T> ptr)
        {
            return Inner.Equals(ptr);
        }
        public object Clone()
        {
            return new IntPtr<T>(Inner);
        }
        public int CompareTo(IntPtr<T> ptr)
        {
            if ((int)Inner < (int)ptr.Inner)
                return -1;
            if ((int)Inner > (int)ptr.Inner)
                return 1;
            return 0;
        }
    }
}