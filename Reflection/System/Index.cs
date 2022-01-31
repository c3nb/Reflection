﻿namespace System
{
    public readonly struct Index : IEquatable<Index>
    {
        private readonly int _value;

        public Index(int value, bool fromEnd)
        {
            if (value < 0)
            {
                throw new ArgumentException("Index must not be negative.", nameof(value));
            }

            _value = fromEnd ? ~value : value;
        }

        public int Value => _value < 0 ? ~_value : _value;
        public bool FromEnd => _value < 0;
        public override bool Equals(object value) => value is Index && _value == ((Index)value)._value;
        public bool Equals(Index other) => _value == other._value;

        public override int GetHashCode()
        {
            return _value;
        }

        public override string ToString()
        {
            string str = Value.ToString();
            return FromEnd ? "^" + str : str;
        }

        public static implicit operator Index(int value)
            => new Index(value, fromEnd: false);
    }
}