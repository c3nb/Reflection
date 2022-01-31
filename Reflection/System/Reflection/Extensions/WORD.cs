﻿using System.Runtime.InteropServices;

namespace System.Reflection.Extensions
{
    [StructLayout(LayoutKind.Explicit, Pack = sizeof(short))]
    public readonly struct WORD : IEquatable<WORD>, IEquatable<short>, IEquatable<ushort>, IEquatable<char>, IComparable<WORD>, IComparable<short>, IComparable<ushort>, IComparable<char>
    {
        [FieldOffset(0)]
        public readonly short signed;
        [FieldOffset(0)]
        public readonly ushort unsigned;
        [FieldOffset(0)]
        public readonly char character;
        public WORD(short _v)
        {
            character = char.MinValue;
            unsigned = 0;
            signed = _v;
        }
        public WORD(ushort _v)
        {
            character = char.MinValue;
            signed = 0;
            unsigned = _v;
        }
        public WORD(char _v)
        {
            signed = 0;
            unsigned = 0;
            character = _v;
        }
        public int CompareTo(char other) => character.CompareTo(other);
        public int CompareTo(ushort other) => unsigned.CompareTo(other);
        public int CompareTo(short other) => signed.CompareTo(other);
        public int CompareTo(WORD other) => unsigned.CompareTo(other.unsigned);
        public bool Equals(char other) => character == other;
        public bool Equals(ushort other) => unsigned == other;
        public bool Equals(short other) => signed == other;
        public bool Equals(WORD other) => unsigned == other.unsigned;

        public override bool Equals(object obj) =>
            obj is WORD r1
            ? Equals(r1)
            : obj is short r2
            ? Equals(r2)
            : obj is ushort r3
            ? Equals(r3)
            : obj is char r4
            ? Equals(r4)
            : false;

        public override string ToString() => unsigned.ToString("X");
        public override int GetHashCode() => base.GetHashCode();
        public int Count()
        {
            // int c = signed - ((signed >> 1) & 0x5555);
            // c = ((c >> 2) & 0x3333) + (c & 0x3333);
            // c = ((c >> 4) + c) & 0x0F0F;
            // return ((c >> 8) + c) & 0x00FF;
            int c = signed - ((signed >> 1) & 0x5555);
            c = (c & 0x3333) + ((c >> 2) & 0x3333);
            return (((c + (c >> 4)) & 0x0F0F) * 0x0101) >> 8;
        }

        public WORD OR(WORD other) => this | other;
        public WORD AND(WORD other) => this & other;
        public WORD XOR(WORD other) => this ^ other;
        public WORD NOT() => ~this;
        public WORD LSHIFT(int shift) => this << shift;
        public WORD RSHIFT(int shift) => this >> shift;
        public WORD ADD(WORD other) => this + other;
        public WORD SUB(WORD other) => this - other;
        public WORD MUL(WORD other) => this * other;
        public WORD DIV(WORD other) => this / other;
        public WORD MOD(WORD other) => this % other;

        public static implicit operator short(WORD value) => value.signed;
        public static implicit operator ushort(WORD value) => value.unsigned;
        public static implicit operator char(WORD value) => value.character;
        public static implicit operator WORD(int value) => new WORD((ushort)value);
        public static implicit operator WORD(short value) => new WORD(value);
        public static implicit operator WORD(ushort value) => new WORD(value);
        public static implicit operator WORD(char value) => new WORD(value);

        #region 닫기
        public static bool operator ==(WORD left, WORD right) => left.unsigned == right.unsigned;
        public static bool operator !=(WORD left, WORD right) => left.unsigned != right.unsigned;
        public static bool operator <(WORD left, WORD right) => left.unsigned < right.unsigned;
        public static bool operator <=(WORD left, WORD right) => left.unsigned <= right.unsigned;
        public static bool operator >(WORD left, WORD right) => left.unsigned > right.unsigned;
        public static bool operator >=(WORD left, WORD right) => left.unsigned >= right.unsigned;

        public static bool operator ==(WORD left, char right) => left.character == right;
        public static bool operator !=(WORD left, char right) => left.character != right;
        public static bool operator <(WORD left, char right) => left.character < right;
        public static bool operator <=(WORD left, char right) => left.character <= right;
        public static bool operator >(WORD left, char right) => left.character > right;
        public static bool operator >=(WORD left, char right) => left.character >= right;

        public static bool operator ==(char left, WORD right) => left == right.character;
        public static bool operator !=(char left, WORD right) => left != right.character;
        public static bool operator <(char left, WORD right) => left < right.character;
        public static bool operator <=(char left, WORD right) => left <= right.character;
        public static bool operator >(char left, WORD right) => left > right.character;
        public static bool operator >=(char left, WORD right) => left >= right.character;

        public static bool operator ==(WORD left, short right) => left.signed == right;
        public static bool operator !=(WORD left, short right) => left.signed != right;
        public static bool operator <(WORD left, short right) => left.signed < right;
        public static bool operator <=(WORD left, short right) => left.signed <= right;
        public static bool operator >(WORD left, short right) => left.signed > right;
        public static bool operator >=(WORD left, short right) => left.signed >= right;

        public static bool operator ==(short left, WORD right) => left == right.signed;
        public static bool operator !=(short left, WORD right) => left != right.signed;
        public static bool operator <(short left, WORD right) => left < right.signed;
        public static bool operator <=(short left, WORD right) => left <= right.signed;
        public static bool operator >(short left, WORD right) => left > right.signed;
        public static bool operator >=(short left, WORD right) => left >= right.signed;

        public static bool operator ==(WORD left, ushort right) => left.unsigned == right;
        public static bool operator !=(WORD left, ushort right) => left.unsigned != right;
        public static bool operator <(WORD left, ushort right) => left.unsigned < right;
        public static bool operator <=(WORD left, ushort right) => left.unsigned <= right;
        public static bool operator >(WORD left, ushort right) => left.unsigned > right;
        public static bool operator >=(WORD left, ushort right) => left.unsigned >= right;

        public static bool operator ==(ushort left, WORD right) => left == right.unsigned;
        public static bool operator !=(ushort left, WORD right) => left != right.unsigned;
        public static bool operator <(ushort left, WORD right) => left < right.unsigned;
        public static bool operator <=(ushort left, WORD right) => left <= right.unsigned;
        public static bool operator >(ushort left, WORD right) => left > right.unsigned;
        public static bool operator >=(ushort left, WORD right) => left >= right.unsigned;
        #endregion

        #region 산술
        public static WORD operator +(WORD left, WORD right) => new WORD((ushort)(left.unsigned + right.unsigned));
        public static WORD operator -(WORD left, WORD right) => new WORD((ushort)(left.unsigned - right.unsigned));
        public static WORD operator *(WORD left, WORD right) => new WORD((ushort)(left.unsigned * right.unsigned));
        public static WORD operator /(WORD left, WORD right) => new WORD((ushort)(left.unsigned / right.unsigned));
        public static WORD operator %(WORD left, WORD right) => new WORD((ushort)(left.unsigned % right.unsigned));

        public static WORD operator +(WORD left, ushort right) => new WORD((ushort)(left.unsigned + right));
        public static WORD operator -(WORD left, ushort right) => new WORD((ushort)(left.unsigned - right));
        public static WORD operator *(WORD left, ushort right) => new WORD((ushort)(left.unsigned * right));
        public static WORD operator /(WORD left, ushort right) => new WORD((ushort)(left.unsigned / right));
        public static WORD operator %(WORD left, ushort right) => new WORD((ushort)(left.unsigned % right));

        public static WORD operator +(ushort left, WORD right) => new WORD((ushort)(left + right.unsigned));
        public static WORD operator -(ushort left, WORD right) => new WORD((ushort)(left - right.unsigned));
        public static WORD operator *(ushort left, WORD right) => new WORD((ushort)(left * right.unsigned));
        public static WORD operator /(ushort left, WORD right) => new WORD((ushort)(left / right.unsigned));
        public static WORD operator %(ushort left, WORD right) => new WORD((ushort)(left % right.unsigned));

        public static WORD operator +(WORD left, short right) => new WORD((short)(left.signed + right));
        public static WORD operator -(WORD left, short right) => new WORD((short)(left.signed - right));
        public static WORD operator *(WORD left, short right) => new WORD((short)(left.signed * right));
        public static WORD operator /(WORD left, short right) => new WORD((short)(left.signed / right));
        public static WORD operator %(WORD left, short right) => new WORD((short)(left.signed % right));

        public static WORD operator +(short left, WORD right) => new WORD((short)(left + right.signed));
        public static WORD operator -(short left, WORD right) => new WORD((short)(left - right.signed));
        public static WORD operator *(short left, WORD right) => new WORD((short)(left * right.signed));
        public static WORD operator /(short left, WORD right) => new WORD((short)(left / right.signed));
        public static WORD operator %(short left, WORD right) => new WORD((short)(left % right.signed));

        public static WORD operator +(WORD left, char right) => new WORD((char)(left.character + right));
        public static WORD operator -(WORD left, char right) => new WORD((char)(left.character - right));
        public static WORD operator *(WORD left, char right) => new WORD((char)(left.character * right));
        public static WORD operator /(WORD left, char right) => new WORD((char)(left.character / right));
        public static WORD operator %(WORD left, char right) => new WORD((char)(left.character % right));

        public static WORD operator +(char left, WORD right) => new WORD((char)(left + right.character));
        public static WORD operator -(char left, WORD right) => new WORD((char)(left - right.character));
        public static WORD operator *(char left, WORD right) => new WORD((char)(left * right.character));
        public static WORD operator /(char left, WORD right) => new WORD((char)(left / right.character));
        public static WORD operator %(char left, WORD right) => new WORD((char)(left % right.character));
        #endregion

        #region 비트
        public static WORD operator <<(WORD left, int right) => new WORD((ushort)(left.unsigned << right));
        public static WORD operator >>(WORD left, int right) => new WORD((ushort)(left.unsigned >> right));
        public static WORD operator &(WORD left, WORD right) => new WORD((ushort)(left.unsigned & right.unsigned));
        public static WORD operator |(WORD left, WORD right) => new WORD((ushort)(left.unsigned | right.unsigned));
        public static WORD operator ^(WORD left, WORD right) => new WORD((ushort)(left.unsigned ^ right.unsigned));
        public static WORD operator ~(WORD left) => new WORD((ushort)~left.unsigned);
        #endregion

        #region 기본 자료형
        public static bool operator ==(WORD left, int right) => left.unsigned == right;
        public static bool operator !=(WORD left, int right) => left.unsigned != right;
        public static bool operator <(WORD left, int right) => left.unsigned < right;
        public static bool operator <=(WORD left, int right) => left.unsigned <= right;
        public static bool operator >(WORD left, int right) => left.unsigned > right;
        public static bool operator >=(WORD left, int right) => left.unsigned >= right;

        public static bool operator ==(int left, WORD right) => left == right.unsigned;
        public static bool operator !=(int left, WORD right) => left != right.unsigned;
        public static bool operator <(int left, WORD right) => left < right.unsigned;
        public static bool operator <=(int left, WORD right) => left <= right.unsigned;
        public static bool operator >(int left, WORD right) => left > right.unsigned;
        public static bool operator >=(int left, WORD right) => left >= right.unsigned;

        public static WORD operator +(WORD left, int right) => new WORD((ushort)(left.unsigned + right));
        public static WORD operator -(WORD left, int right) => new WORD((ushort)(left.unsigned - right));
        public static WORD operator *(WORD left, int right) => new WORD((ushort)(left.unsigned * right));
        public static WORD operator /(WORD left, int right) => new WORD((ushort)(left.unsigned / right));
        public static WORD operator %(WORD left, int right) => new WORD((ushort)(left.unsigned % right));

        public static WORD operator +(int left, WORD right) => new WORD((ushort)(left + right.unsigned));
        public static WORD operator -(int left, WORD right) => new WORD((ushort)(left - right.unsigned));
        public static WORD operator *(int left, WORD right) => new WORD((ushort)(left * right.unsigned));
        public static WORD operator /(int left, WORD right) => new WORD((ushort)(left / right.unsigned));
        public static WORD operator %(int left, WORD right) => new WORD((ushort)(left % right.unsigned));
        #endregion

        #region 모호함 제거
        public static WORD operator ++(WORD left) => new WORD((ushort)(left.unsigned + 1));
        public static WORD operator --(WORD left) => new WORD((ushort)(left.unsigned - 1));
        #endregion
    }
}
