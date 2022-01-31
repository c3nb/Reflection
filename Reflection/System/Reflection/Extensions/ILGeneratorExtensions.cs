using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace System.Reflection.Extensions
{
    public static class ILGeneratorExtensions
	{
		public static TValue ComputeIfAbsent<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> resolver)
		{
			if (dict.TryGetValue(key, out TValue value))
				return value;
			else
			{
				value = resolver(key);
				dict.Add(key, value);
			}
			return value;
		}
		public static Dictionary<ILGenerator, List<KeyValuePair<OpCode, object>>> Instructions = new Dictionary<ILGenerator, List<KeyValuePair<OpCode, object>>>();
		public static byte[] GetIL(this ILGenerator ILGenerator)
		{
			int newSize;
			int updateAddr;
			byte[] newBytes;
			int m_currExcStackCount = ILGenerator.GetField<int>("m_currExcStackCount");
			int m_length = ILGenerator.GetField<int>("m_length");
			int m_fixupCount = ILGenerator.GetField<int>("m_fixupCount");
			byte[] m_ILStream = ILGenerator.GetField<byte[]>("m_ILStream");
			if (m_currExcStackCount != 0)
				throw new ArgumentException("UnclosedExceptionBlock");
			if (m_length == 0)
				return null;
			newSize = m_length;
			newBytes = new byte[newSize];
			Array.Copy(m_ILStream, newBytes, newSize);
			Indexer m_fixupData = ILGenerator.GetField("m_fixupData").GetIndexer();
			for (int i = 0; i < m_fixupCount; i++)
			{
				Label m_fixupLabel = m_fixupData[i].GetField<Label>("m_fixupLabel");
				int m_fixupPos = m_fixupData[i].GetField<int>("m_fixupPos");
				int m_fixupInstSize = m_fixupData[i].GetField<int>("m_fixupInstSize");
				updateAddr = ILGenerator.Invoke<int>("GetLabelPos", m_fixupLabel) - (m_fixupPos + m_fixupInstSize);
				if (m_fixupInstSize == 1)
				{
					if (updateAddr < sbyte.MinValue || updateAddr > sbyte.MaxValue)
					{
						throw new NotSupportedException("IllegalOneByteBranch");
					}
					if (updateAddr < 0)
					{
						newBytes[m_fixupPos] = (byte)(256 + updateAddr);
					}
					else
					{
						newBytes[m_fixupPos] = (byte)updateAddr;
					}
				}
				else
				{
					newBytes[m_fixupPos++] = (byte)updateAddr;
					newBytes[m_fixupPos++] = (byte)(updateAddr >> 8);
					newBytes[m_fixupPos++] = (byte)(updateAddr >> 16);
					newBytes[m_fixupPos++] = (byte)(updateAddr >> 24);
				}
			}
			return newBytes;
		}
		public static List<KeyValuePair<OpCode, object>> GetInstructions(this ILGenerator ILGenerator)
        {
			return Instructions[ILGenerator];
        }
		public static void EmitAuto(this ILGenerator ILGenerator, OpCode opcode, params object[] operands)
		{
			if (operands.Length == 1)
				switch (operands[0])
				{
					case string i:
						ILGenerator.Emit(opcode, i);
						return;
					case FieldInfo i:
						ILGenerator.Emit(opcode, i);
						return;
					case Label[] i:
						ILGenerator.Emit(opcode, i);
						return;
					case Label i:
						ILGenerator.Emit(opcode, i);
						return;
					case LocalBuilder i:
						ILGenerator.Emit(opcode, i);
						return;
					case float i:
						ILGenerator.Emit(opcode, i);
						return;
					case byte i:
						ILGenerator.Emit(opcode, i);
						return;
					case sbyte i:
						ILGenerator.Emit(opcode, i);
						return;
					case short i:
						ILGenerator.Emit(opcode, i);
						return;
					case double i:
						ILGenerator.Emit(opcode, i);
						return;
					case MethodInfo i:
						ILGenerator.Emit(opcode, i);
						return;
					case int i:
						ILGenerator.Emit(opcode, i);
						return;
					case long i:
						ILGenerator.Emit(opcode, i);
						return;
					case Type i:
						ILGenerator.Emit(opcode, i);
						return;
					case SignatureHelper i:
						ILGenerator.Emit(opcode, i);
						return;
					case ConstructorInfo i:
						ILGenerator.Emit(opcode, i);
						return;
					default:
						ILGenerator.Emit(opcode);
						return;
				}
			else if (operands.Length == 2)
				switch (operands[0])
				{
					case MethodInfo i:
						ILGenerator.EmitCall(opcode, i, (Type[])operands[1]);
						return;
					default:
						throw new InvalidOperationException();
				}
			else if (operands.Length == 3)
				switch (operands[0])
				{
					case CallingConvention i:
						ILGenerator.EmitCalli(opcode, i, (Type)operands[1], (Type[])operands[2]);
						return;
					default:
						throw new InvalidOperationException();
				}
			else if (operands.Length == 4)
				switch (operands[0])
				{
					case CallingConventions i:
						ILGenerator.EmitCalli(opcode, i, (Type)operands[1], (Type[])operands[2], (Type[])operands[3]);
						return;
					default:
						throw new InvalidOperationException();
				}
			else
				ILGenerator.Emit(opcode);
		}
		#region NoOptions
		/// <summary>
		/// Fills space if opcodes are patched. No meaningful operation is performed although a processing cycle can be consumed.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Nop(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Nop); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Nop, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Nop, null)); return ILGenerator; }
		/// <summary>
		/// Signals the Common Language Infrastructure (CLI) to inform the debugger that a break point has been tripped.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Break(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Break); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Break, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Break, null)); return ILGenerator; }
		/// <summary>
		/// Loads the argument at index 0 onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldarg_0(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldarg_0); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldarg_0, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldarg_0, null)); return ILGenerator; }
		/// <summary>
		/// Loads the argument at index 1 onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldarg_1(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldarg_1); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldarg_1, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldarg_1, null)); return ILGenerator; }
		/// <summary>
		/// Loads the argument at index 2 onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldarg_2(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldarg_2); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldarg_2, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldarg_2, null)); return ILGenerator; }
		/// <summary>
		/// Loads the argument at index 3 onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldarg_3(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldarg_3); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldarg_3, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldarg_3, null)); return ILGenerator; }
		/// <summary>
		/// Loads the local variable at index 0 onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldloc_0(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldloc_0); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldloc_0, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldloc_0, null)); return ILGenerator; }
		/// <summary>
		/// Loads the local variable at index 1 onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldloc_1(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldloc_1); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldloc_1, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldloc_1, null)); return ILGenerator; }
		/// <summary>
		/// Loads the local variable at index 2 onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldloc_2(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldloc_2); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldloc_2, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldloc_2, null)); return ILGenerator; }
		/// <summary>
		/// Loads the local variable at index 3 onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldloc_3(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldloc_3); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldloc_3, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldloc_3, null)); return ILGenerator; }
		/// <summary>
		/// Pops the current value from the top of the evaluation stack and stores it in the local variable list at index 0.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stloc_0(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stloc_0); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stloc_0, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stloc_0, null)); return ILGenerator; }
		/// <summary>
		/// Pops the current value from the top of the evaluation stack and stores it in the local variable list at index 1.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stloc_1(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stloc_1); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stloc_1, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stloc_1, null)); return ILGenerator; }
		/// <summary>
		/// Pops the current value from the top of the evaluation stack and stores it in the local variable list at index 2.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stloc_2(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stloc_2); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stloc_2, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stloc_2, null)); return ILGenerator; }
		/// <summary>
		/// Pops the current value from the top of the evaluation stack and stores it in the local variable list at index 3.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stloc_3(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stloc_3); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stloc_3, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stloc_3, null)); return ILGenerator; }
		/// <summary>
		/// Pushes a null reference (type O) onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldnull(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldnull); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldnull, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldnull, null)); return ILGenerator; }
		/// <summary>
		/// Pushes the integer value of -1 onto the evaluation stack as an int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldc_I4_M1(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldc_I4_M1); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_M1, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_M1, null)); return ILGenerator; }
		/// <summary>
		/// Pushes the integer value of 0 onto the evaluation stack as an int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldc_I4_0(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldc_I4_0); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_0, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_0, null)); return ILGenerator; }
		/// <summary>
		/// Pushes the integer value of 1 onto the evaluation stack as an int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldc_I4_1(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldc_I4_1); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_1, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_1, null)); return ILGenerator; }
		/// <summary>
		/// Pushes the integer value of 2 onto the evaluation stack as an int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldc_I4_2(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldc_I4_2); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_2, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_2, null)); return ILGenerator; }
		/// <summary>
		/// Pushes the integer value of 3 onto the evaluation stack as an int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldc_I4_3(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldc_I4_3); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_3, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_3, null)); return ILGenerator; }
		/// <summary>
		/// Pushes the integer value of 4 onto the evaluation stack as an int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldc_I4_4(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldc_I4_4); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_4, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_4, null)); return ILGenerator; }
		/// <summary>
		/// Pushes the integer value of 5 onto the evaluation stack as an int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldc_I4_5(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldc_I4_5); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_5, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_5, null)); return ILGenerator; }
		/// <summary>
		/// Pushes the integer value of 6 onto the evaluation stack as an int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldc_I4_6(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldc_I4_6); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_6, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_6, null)); return ILGenerator; }
		/// <summary>
		/// Pushes the integer value of 7 onto the evaluation stack as an int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldc_I4_7(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldc_I4_7); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_7, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_7, null)); return ILGenerator; }
		/// <summary>
		/// Pushes the integer value of 8 onto the evaluation stack as an int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldc_I4_8(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldc_I4_8); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_8, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_8, null)); return ILGenerator; }
		/// <summary>
		/// Copies the current topmost value on the evaluation stack, and then pushes the copy onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Dup(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Dup); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Dup, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Dup, null)); return ILGenerator; }
		/// <summary>
		/// Removes the value currently on top of the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Pop(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Pop); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Pop, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Pop, null)); return ILGenerator; }
		/// <summary>
		/// Returns from the current method, pushing a return value (if present) from the callee's evaluation stack onto the caller's evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ret(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ret); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ret, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ret, null)); return ILGenerator; }
		/// <summary>
		/// Loads a value of type int8 as an int32 onto the evaluation stack indirectly.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldind_I1(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldind_I1); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldind_I1, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldind_I1, null)); return ILGenerator; }
		/// <summary>
		/// Loads a value of type unsigned int8 as an int32 onto the evaluation stack indirectly.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldind_U1(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldind_U1); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldind_U1, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldind_U1, null)); return ILGenerator; }
		/// <summary>
		/// Loads a value of type int16 as an int32 onto the evaluation stack indirectly.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldind_I2(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldind_I2); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldind_I2, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldind_I2, null)); return ILGenerator; }
		/// <summary>
		/// Loads a value of type unsigned int16 as an int32 onto the evaluation stack indirectly.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldind_U2(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldind_U2); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldind_U2, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldind_U2, null)); return ILGenerator; }
		/// <summary>
		/// Loads a value of type int32 as an int32 onto the evaluation stack indirectly.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldind_I4(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldind_I4); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldind_I4, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldind_I4, null)); return ILGenerator; }
		/// <summary>
		/// Loads a value of type unsigned int32 as an int32 onto the evaluation stack indirectly.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldind_U4(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldind_U4); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldind_U4, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldind_U4, null)); return ILGenerator; }
		/// <summary>
		/// Loads a value of type int64 as an int64 onto the evaluation stack indirectly.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldind_I8(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldind_I8); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldind_I8, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldind_I8, null)); return ILGenerator; }
		/// <summary>
		/// Loads a value of type native int as a native int onto the evaluation stack indirectly.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldind_I(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldind_I); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldind_I, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldind_I, null)); return ILGenerator; }
		/// <summary>
		/// Loads a value of type float32 as a type F (float) onto the evaluation stack indirectly.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldind_R4(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldind_R4); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldind_R4, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldind_R4, null)); return ILGenerator; }
		/// <summary>
		/// Loads a value of type float64 as a type F (float) onto the evaluation stack indirectly.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldind_R8(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldind_R8); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldind_R8, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldind_R8, null)); return ILGenerator; }
		/// <summary>
		/// Loads an object reference as a type O (object reference) onto the evaluation stack indirectly.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldind_Ref(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldind_Ref); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldind_Ref, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldind_Ref, null)); return ILGenerator; }
		/// <summary>
		/// Stores a object reference value at a supplied address.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stind_Ref(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stind_Ref); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stind_Ref, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stind_Ref, null)); return ILGenerator; }
		/// <summary>
		/// Stores a value of type int8 at a supplied address.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stind_I1(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stind_I1); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stind_I1, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stind_I1, null)); return ILGenerator; }
		/// <summary>
		/// Stores a value of type int16 at a supplied address.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stind_I2(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stind_I2); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stind_I2, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stind_I2, null)); return ILGenerator; }
		/// <summary>
		/// Stores a value of type int32 at a supplied address.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stind_I4(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stind_I4); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stind_I4, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stind_I4, null)); return ILGenerator; }
		/// <summary>
		/// Stores a value of type int64 at a supplied address.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stind_I8(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stind_I8); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stind_I8, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stind_I8, null)); return ILGenerator; }
		/// <summary>
		/// Stores a value of type float32 at a supplied address.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stind_R4(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stind_R4); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stind_R4, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stind_R4, null)); return ILGenerator; }
		/// <summary>
		/// Stores a value of type float64 at a supplied address.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stind_R8(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stind_R8); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stind_R8, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stind_R8, null)); return ILGenerator; }
		/// <summary>
		/// Adds two values and pushes the result onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Add(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Add); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Add, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Add, null)); return ILGenerator; }
		/// <summary>
		/// Subtracts one value from another and pushes the result onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Sub(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Sub); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Sub, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Sub, null)); return ILGenerator; }
		/// <summary>
		/// Multiplies two values and pushes the result on the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Mul(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Mul); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Mul, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Mul, null)); return ILGenerator; }
		/// <summary>
		/// Divides two values and pushes the result as a floating-point (type F) or quotient (type int32) onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Div(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Div); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Div, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Div, null)); return ILGenerator; }
		/// <summary>
		/// Divides two unsigned integer values and pushes the result (int32) onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Div_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Div_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Div_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Div_Un, null)); return ILGenerator; }
		/// <summary>
		/// Divides two values and pushes the remainder onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Rem(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Rem); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Rem, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Rem, null)); return ILGenerator; }
		/// <summary>
		/// Divides two unsigned values and pushes the remainder onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Rem_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Rem_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Rem_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Rem_Un, null)); return ILGenerator; }
		/// <summary>
		/// Computes the bitwise AND of two values and pushes the result onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator And(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.And); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.And, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.And, null)); return ILGenerator; }
		/// <summary>
		/// Compute the bitwise complement of the two integer values on top of the stack and pushes the result onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Or(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Or); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Or, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Or, null)); return ILGenerator; }
		/// <summary>
		/// Computes the bitwise XOR of the top two values on the evaluation stack, pushing the result onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Xor(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Xor); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Xor, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Xor, null)); return ILGenerator; }
		/// <summary>
		/// Shifts an integer value to the left (in zeroes) by a specified number of bits, pushing the result onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Shl(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Shl); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Shl, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Shl, null)); return ILGenerator; }
		/// <summary>
		/// Shifts an integer value (in sign) to the right by a specified number of bits, pushing the result onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Shr(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Shr); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Shr, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Shr, null)); return ILGenerator; }
		/// <summary>
		/// Shifts an unsigned integer value (in zeroes) to the right by a specified number of bits, pushing the result onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Shr_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Shr_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Shr_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Shr_Un, null)); return ILGenerator; }
		/// <summary>
		/// Negates a value and pushes the result onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Neg(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Neg); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Neg, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Neg, null)); return ILGenerator; }
		/// <summary>
		/// Computes the bitwise complement of the integer value on top of the stack and pushes the result onto the evaluation stack as the same type.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Not(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Not); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Not, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Not, null)); return ILGenerator; }
		/// <summary>
		/// Converts the value on top of the evaluation stack to int8, then extends (pads) it to int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_I1(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_I1); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_I1, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_I1, null)); return ILGenerator; }
		/// <summary>
		/// Converts the value on top of the evaluation stack to int16, then extends (pads) it to int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_I2(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_I2); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_I2, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_I2, null)); return ILGenerator; }
		/// <summary>
		/// Converts the value on top of the evaluation stack to int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_I4(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_I4); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_I4, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_I4, null)); return ILGenerator; }
		/// <summary>
		/// Converts the value on top of the evaluation stack to int64.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_I8(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_I8); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_I8, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_I8, null)); return ILGenerator; }
		/// <summary>
		/// Converts the value on top of the evaluation stack to float32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_R4(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_R4); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_R4, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_R4, null)); return ILGenerator; }
		/// <summary>
		/// Converts the value on top of the evaluation stack to float64.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_R8(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_R8); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_R8, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_R8, null)); return ILGenerator; }
		/// <summary>
		/// Converts the value on top of the evaluation stack to unsigned int32, and extends it to int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_U4(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_U4); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_U4, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_U4, null)); return ILGenerator; }
		/// <summary>
		/// Converts the value on top of the evaluation stack to unsigned int64, and extends it to int64.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_U8(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_U8); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_U8, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_U8, null)); return ILGenerator; }
		/// <summary>
		/// Converts the unsigned integer value on top of the evaluation stack to float32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_R_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_R_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_R_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_R_Un, null)); return ILGenerator; }
		/// <summary>
		/// Throws the exception object currently on the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Throw(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Throw); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Throw, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Throw, null)); return ILGenerator; }
		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to signed int8 and extends it to int32, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_I1_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_I1_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I1_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I1_Un, null)); return ILGenerator; }
		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to signed int16 and extends it to int32, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_I2_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_I2_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I2_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I2_Un, null)); return ILGenerator; }
		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to signed int32, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_I4_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_I4_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I4_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I4_Un, null)); return ILGenerator; }
		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to signed int64, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_I8_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_I8_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I8_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I8_Un, null)); return ILGenerator; }
		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to unsigned int8 and extends it to int32, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_U1_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_U1_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U1_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U1_Un, null)); return ILGenerator; }
		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to unsigned int16 and extends it to int32, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_U2_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_U2_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U2_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U2_Un, null)); return ILGenerator; }
		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to unsigned int32, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_U4_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_U4_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U4_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U4_Un, null)); return ILGenerator; }
		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to unsigned int64, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_U8_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_U8_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U8_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U8_Un, null)); return ILGenerator; }
		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to signed native int, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_I_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_I_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I_Un, null)); return ILGenerator; }
		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to unsigned native int, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_U_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_U_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U_Un, null)); return ILGenerator; }
		/// <summary>
		/// Pushes the number of elements of a zero-based, one-dimensional array onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldlen(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldlen); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldlen, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldlen, null)); return ILGenerator; }
		/// <summary>
		/// Loads the element with type int8 at a specified array index onto the top of the evaluation stack as an int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldelem_I1(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldelem_I1); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldelem_I1, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldelem_I1, null)); return ILGenerator; }
		/// <summary>
		/// Loads the element with type unsigned int8 at a specified array index onto the top of the evaluation stack as an int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldelem_U1(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldelem_U1); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldelem_U1, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldelem_U1, null)); return ILGenerator; }
		/// <summary>
		/// Loads the element with type int16 at a specified array index onto the top of the evaluation stack as an int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldelem_I2(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldelem_I2); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldelem_I2, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldelem_I2, null)); return ILGenerator; }
		/// <summary>
		/// Loads the element with type unsigned int16 at a specified array index onto the top of the evaluation stack as an int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldelem_U2(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldelem_U2); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldelem_U2, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldelem_U2, null)); return ILGenerator; }
		/// <summary>
		/// Loads the element with type int32 at a specified array index onto the top of the evaluation stack as an int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldelem_I4(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldelem_I4); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldelem_I4, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldelem_I4, null)); return ILGenerator; }
		/// <summary>
		/// Loads the element with type unsigned int32 at a specified array index onto the top of the evaluation stack as an int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldelem_U4(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldelem_U4); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldelem_U4, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldelem_U4, null)); return ILGenerator; }
		/// <summary>
		/// Loads the element with type int64 at a specified array index onto the top of the evaluation stack as an int64.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldelem_I8(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldelem_I8); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldelem_I8, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldelem_I8, null)); return ILGenerator; }
		/// <summary>
		/// Loads the element with type native int at a specified array index onto the top of the evaluation stack as a native int.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldelem_I(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldelem_I); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldelem_I, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldelem_I, null)); return ILGenerator; }
		/// <summary>
		/// Loads the element with type float32 at a specified array index onto the top of the evaluation stack as type F (float).
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldelem_R4(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldelem_R4); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldelem_R4, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldelem_R4, null)); return ILGenerator; }
		/// <summary>
		/// Loads the element with type float64 at a specified array index onto the top of the evaluation stack as type F (float).
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldelem_R8(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldelem_R8); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldelem_R8, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldelem_R8, null)); return ILGenerator; }
		/// <summary>
		/// Loads the element containing an object reference at a specified array index onto the top of the evaluation stack as type O (object reference).
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ldelem_Ref(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ldelem_Ref); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldelem_Ref, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldelem_Ref, null)); return ILGenerator; }
		/// <summary>
		/// Replaces the array element at a given index with the native int value on the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stelem_I(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stelem_I); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stelem_I, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stelem_I, null)); return ILGenerator; }
		/// <summary>
		/// Replaces the array element at a given index with the int8 value on the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stelem_I1(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stelem_I1); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stelem_I1, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stelem_I1, null)); return ILGenerator; }
		/// <summary>
		/// Replaces the array element at a given index with the int16 value on the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stelem_I2(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stelem_I2); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stelem_I2, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stelem_I2, null)); return ILGenerator; }
		/// <summary>
		/// Replaces the array element at a given index with the int32 value on the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stelem_I4(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stelem_I4); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stelem_I4, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stelem_I4, null)); return ILGenerator; }
		/// <summary>
		/// Replaces the array element at a given index with the int64 value on the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stelem_I8(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stelem_I8); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stelem_I8, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stelem_I8, null)); return ILGenerator; }
		/// <summary>
		/// Replaces the array element at a given index with the float32 value on the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stelem_R4(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stelem_R4); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stelem_R4, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stelem_R4, null)); return ILGenerator; }
		/// <summary>
		/// Replaces the array element at a given index with the float64 value on the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stelem_R8(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stelem_R8); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stelem_R8, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stelem_R8, null)); return ILGenerator; }
		/// <summary>
		/// Replaces the array element at a given index with the object ref value (type O) on the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stelem_Ref(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stelem_Ref); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stelem_Ref, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stelem_Ref, null)); return ILGenerator; }
		/// <summary>
		/// Converts the signed value on top of the evaluation stack to signed int8 and extends it to int32, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_I1(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_I1); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I1, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I1, null)); return ILGenerator; }
		/// <summary>
		/// Converts the signed value on top of the evaluation stack to unsigned int8 and extends it to int32, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_U1(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_U1); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U1, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U1, null)); return ILGenerator; }
		/// <summary>
		/// Converts the signed value on top of the evaluation stack to signed int16 and extending it to int32, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_I2(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_I2); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I2, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I2, null)); return ILGenerator; }
		/// <summary>
		/// Converts the signed value on top of the evaluation stack to unsigned int16 and extends it to int32, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_U2(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_U2); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U2, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U2, null)); return ILGenerator; }
		/// <summary>
		/// Converts the signed value on top of the evaluation stack to signed int32, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_I4(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_I4); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I4, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I4, null)); return ILGenerator; }
		/// <summary>
		/// Converts the signed value on top of the evaluation stack to unsigned int32, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_U4(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_U4); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U4, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U4, null)); return ILGenerator; }
		/// <summary>
		/// Converts the signed value on top of the evaluation stack to signed int64, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_I8(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_I8); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I8, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I8, null)); return ILGenerator; }
		/// <summary>
		/// Converts the signed value on top of the evaluation stack to unsigned int64, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_U8(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_U8); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U8, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U8, null)); return ILGenerator; }
		/// <summary>
		/// Throws ArithmeticException if value is not a finite number.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ckfinite(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ckfinite); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ckfinite, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ckfinite, null)); return ILGenerator; }
		/// <summary>
		/// Converts the value on top of the evaluation stack to unsigned int16, and extends it to int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_U2(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_U2); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_U2, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_U2, null)); return ILGenerator; }
		/// <summary>
		/// Converts the value on top of the evaluation stack to unsigned int8, and extends it to int32.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_U1(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_U1); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_U1, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_U1, null)); return ILGenerator; }
		/// <summary>
		/// Converts the value on top of the evaluation stack to native int.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_I(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_I); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_I, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_I, null)); return ILGenerator; }
		/// <summary>
		/// Converts the signed value on top of the evaluation stack to signed native int, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_I(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_I); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_I, null)); return ILGenerator; }
		/// <summary>
		/// Converts the signed value on top of the evaluation stack to unsigned native int, throwing OverflowException on overflow.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_Ovf_U(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_Ovf_U); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_Ovf_U, null)); return ILGenerator; }
		/// <summary>
		/// Adds two integers, performs an overflow check, and pushes the result onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Add_Ovf(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Add_Ovf); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Add_Ovf, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Add_Ovf, null)); return ILGenerator; }
		/// <summary>
		/// Adds two integers, performs an overflow check, and pushes the result onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Add_Ovf_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Add_Ovf_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Add_Ovf_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Add_Ovf_Un, null)); return ILGenerator; }
		/// <summary>
		/// Multiplies two integer values, performs an overflow check, and pushes the result onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Mul_Ovf(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Mul_Ovf); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Mul_Ovf, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Mul_Ovf, null)); return ILGenerator; }
		/// <summary>
		/// Multiplies two unsigned integer values, performs an overflow check, and pushes the result onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Mul_Ovf_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Mul_Ovf_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Mul_Ovf_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Mul_Ovf_Un, null)); return ILGenerator; }
		/// <summary>
		/// Subtracts one integer value from another, performs an overflow check, and pushes the result onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Sub_Ovf(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Sub_Ovf); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Sub_Ovf, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Sub_Ovf, null)); return ILGenerator; }
		/// <summary>
		/// Subtracts one unsigned integer value from another, performs an overflow check, and pushes the result onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Sub_Ovf_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Sub_Ovf_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Sub_Ovf_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Sub_Ovf_Un, null)); return ILGenerator; }
		/// <summary>
		/// Transfers control from the fault or finally clause of an exception block back to the Common Language Infrastructure (CLI) exception handler.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Endfinally(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Endfinally); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Endfinally, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Endfinally, null)); return ILGenerator; }
		/// <summary>
		/// Stores a value of type native int at a supplied address.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Stind_I(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Stind_I); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stind_I, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stind_I, null)); return ILGenerator; }
		/// <summary>
		/// Converts the value on top of the evaluation stack to unsigned native int, and extends it to native int.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Conv_U(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Conv_U); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Conv_U, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Conv_U, null)); return ILGenerator; }
		/// <summary>
		/// This is a reserved instruction.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Prefix7(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Prefix7); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Prefix7, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Prefix7, null)); return ILGenerator; }
		/// <summary>
		/// This is a reserved instruction.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Prefix6(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Prefix6); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Prefix6, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Prefix6, null)); return ILGenerator; }
		/// <summary>
		/// This is a reserved instruction.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Prefix5(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Prefix5); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Prefix5, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Prefix5, null)); return ILGenerator; }
		/// <summary>
		/// This is a reserved instruction.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Prefix4(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Prefix4); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Prefix4, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Prefix4, null)); return ILGenerator; }
		/// <summary>
		/// This is a reserved instruction.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Prefix3(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Prefix3); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Prefix3, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Prefix3, null)); return ILGenerator; }
		/// <summary>
		/// This is a reserved instruction.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Prefix2(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Prefix2); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Prefix2, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Prefix2, null)); return ILGenerator; }
		/// <summary>
		/// This is a reserved instruction.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Prefix1(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Prefix1); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Prefix1, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Prefix1, null)); return ILGenerator; }
		/// <summary>
		/// This is a reserved instruction.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Prefixref(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Prefixref); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Prefixref, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Prefixref, null)); return ILGenerator; }
		/// <summary>
		/// Returns an unmanaged pointer to the argument list of the current method.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Arglist(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Arglist); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Arglist, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Arglist, null)); return ILGenerator; }
		/// <summary>
		/// Compares two values. If they are equal, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Ceq(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Ceq); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ceq, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ceq, null)); return ILGenerator; }
		/// <summary>
		/// Compares two values. If the first value is greater than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Cgt(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Cgt); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Cgt, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Cgt, null)); return ILGenerator; }
		/// <summary>
		/// Compares two unsigned or unordered values. If the first value is greater than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Cgt_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Cgt_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Cgt_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Cgt_Un, null)); return ILGenerator; }
		/// <summary>
		/// Compares two values. If the first value is less than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Clt(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Clt); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Clt, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Clt, null)); return ILGenerator; }
		/// <summary>
		/// Compares the unsigned or unordered values value1 and value2. If value1 is less than value2, then the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Clt_Un(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Clt_Un); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Clt_Un, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Clt_Un, null)); return ILGenerator; }
		/// <summary>
		/// Allocates a certain number of bytes from the local dynamic memory pool and pushes the address (a transient pointer, type *) of the first allocated byte onto the evaluation stack.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Localloc(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Localloc); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Localloc, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Localloc, null)); return ILGenerator; }
		/// <summary>
		/// Transfers control from the filter clause of an exception back to the Common Language Infrastructure (CLI) exception handler.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Endfilter(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Endfilter); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Endfilter, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Endfilter, null)); return ILGenerator; }
		/// <summary>
		/// Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that location cannot be cached or that multiple stores to that location cannot be suppressed.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Volatile(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Volatile); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Volatile, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Volatile, null)); return ILGenerator; }
		/// <summary>
		/// Performs a postfixed method call instruction such that the current method's stack frame is removed before the actual call instruction is executed.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Tailcall(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Tailcall); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Tailcall, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Tailcall, null)); return ILGenerator; }
		/// <summary>
		/// Copies a specified number bytes from a source address to a destination address.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Cpblk(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Cpblk); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Cpblk, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Cpblk, null)); return ILGenerator; }
		/// <summary>
		/// Initializes a specified block of memory at a specific address to a given size and initial value.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Initblk(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Initblk); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Initblk, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Initblk, null)); return ILGenerator; }
		/// <summary>
		/// Rethrows the current exception.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Rethrow(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Rethrow); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Rethrow, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Rethrow, null)); return ILGenerator; }
		/// <summary>
		/// Retrieves the type token embedded in a typed reference.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Refanytype(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Refanytype); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Refanytype, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Refanytype, null)); return ILGenerator; }
		/// <summary>
		/// Specifies that the subsequent array address operation performs no type check at run time, and that it returns a managed pointer whose mutability is restricted.
		/// </summary>
		/// <returns></returns>
		public static ILGenerator Readonly(this ILGenerator ILGenerator) { ILGenerator.Emit(OpCodes.Readonly); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Readonly, null) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Readonly, null)); return ILGenerator; }
		#endregion
		#region Options
		/// <summary>
		/// Loads the argument (referenced by a specified short form index) onto the evaluation stack.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static ILGenerator Ldarg_S(this ILGenerator ILGenerator, byte index) { ILGenerator.Emit(OpCodes.Ldarg_S, index); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldarg_S, index) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldarg_S, index)); return ILGenerator; }
		/// <summary>
		/// Load an argument address, in short form, onto the evaluation stack.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static ILGenerator Ldarga_S(this ILGenerator ILGenerator, byte index) { ILGenerator.Emit(OpCodes.Ldarga_S, index); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldarga_S, index) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldarga_S, index)); return ILGenerator; }
		/// <summary>
		/// Stores the value on top of the evaluation stack in the argument slot at a specified index, short form.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static ILGenerator Starg_S(this ILGenerator ILGenerator, byte index) { ILGenerator.Emit(OpCodes.Starg_S, index); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Starg_S, index) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Starg_S, index)); return ILGenerator; }
		/// <summary>
		/// Loads the local variable at a specific index onto the evaluation stack, short form.
		/// </summary>
		/// <param name="local"></param>
		/// <returns></returns>
		public static ILGenerator Ldloc_S(this ILGenerator ILGenerator, LocalBuilder local) { ILGenerator.Emit(OpCodes.Ldloc_S, local); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldloc_S, local) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldloc_S, local)); return ILGenerator; }
		/// <summary>
		/// Loads the local variable at a specific index onto the evaluation stack, short form.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static ILGenerator Ldloc_S(this ILGenerator ILGenerator, byte index) { ILGenerator.Emit(OpCodes.Ldloc_S, index); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldloc_S, index) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldloc_S, index)); return ILGenerator; }
		/// <summary>
		/// Loads the address of the local variable at a specific index onto the evaluation stack, short form.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static ILGenerator Ldloca_S(this ILGenerator ILGenerator, byte index) { ILGenerator.Emit(OpCodes.Ldloca_S, index); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldloca_S, index) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldloca_S, index)); return ILGenerator; }
		/// <summary>
		/// Pops the current value from the top of the evaluation stack and stores it in the local variable list at index (short form).
		/// </summary>
		/// <param name="local"></param>
		/// <returns></returns>
		public static ILGenerator Stloc_S(this ILGenerator ILGenerator, LocalBuilder local) { ILGenerator.Emit(OpCodes.Stloc_S, local); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stloc_S, local) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stloc_S, local)); return ILGenerator; }
		/// <summary>
		/// Pops the current value from the top of the evaluation stack and stores it in the local variable list at index (short form).
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static ILGenerator Stloc_S(this ILGenerator ILGenerator, byte index) { ILGenerator.Emit(OpCodes.Stloc_S, index); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stloc_S, index) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stloc_S, index)); return ILGenerator; }
		/// <summary>
		/// Pushes the supplied int8 value onto the evaluation stack as an int32, short form.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static ILGenerator Ldc_I4_S(this ILGenerator ILGenerator, sbyte value) { ILGenerator.Emit(OpCodes.Ldc_I4_S, value); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_S, value) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4_S, value)); return ILGenerator; }
		/// <summary>
		/// Pushes a supplied value of type int32 onto the evaluation stack as an int32.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static ILGenerator Ldc_I4(this ILGenerator ILGenerator, int value) { ILGenerator.Emit(OpCodes.Ldc_I4, value); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4, value) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldc_I4, value)); return ILGenerator; }
		/// <summary>
		/// Pushes a supplied value of type int64 onto the evaluation stack as an int64.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static ILGenerator Ldc_I8(this ILGenerator ILGenerator, long value) { ILGenerator.Emit(OpCodes.Ldc_I8, value); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldc_I8, value) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldc_I8, value)); return ILGenerator; }
		/// <summary>
		/// Loads the element with type float32 at a specified array index onto the top of the evaluation stack as type F (float).
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static ILGenerator Ldc_R4(this ILGenerator ILGenerator, float value) { ILGenerator.Emit(OpCodes.Ldc_R4, value); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldc_R4, value) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldc_R4, value)); return ILGenerator; }
		/// <summary>
		/// Loads the element with type float64 at a specified array index onto the top of the evaluation stack as type F (float).
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static ILGenerator Ldc_R8(this ILGenerator ILGenerator, double value) { ILGenerator.Emit(OpCodes.Ldc_R8, value); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldc_R8, value) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldc_R8, value)); return ILGenerator; }
		/// <summary>
		/// Exits current method and jumps to specified method.
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public static ILGenerator Jmp(this ILGenerator ILGenerator, MethodInfo method) { ILGenerator.Emit(OpCodes.Jmp, method); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Jmp, method) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Jmp, method)); return ILGenerator; }
		/// <summary>
		/// Calls the method indicated by the passed method descriptor.
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public static ILGenerator Call(this ILGenerator ILGenerator, MethodInfo method) { ILGenerator.Emit(OpCodes.Call, method); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Call, method) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Call, method)); return ILGenerator; }
		/// <summary>
		/// Calls the method indicated by the passed method descriptor.
		/// </summary>
		/// <param name="constructor"></param>
		/// <returns></returns>
		public static ILGenerator Call(this ILGenerator ILGenerator, ConstructorInfo constructor) { ILGenerator.Emit(OpCodes.Call, constructor); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Call, constructor) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Call, constructor)); return ILGenerator; }
		/// <summary>
		/// Calls the method indicated by the passed method descriptor.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public static ILGenerator Call(this ILGenerator ILGenerator, MethodInfo method, Type[] types) { ILGenerator.EmitCall(OpCodes.Call, method, types); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Call, types) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Call, types)); return ILGenerator; }
		/// <summary>
		/// Calls the method indicated on the evaluation stack (as a pointer to an entry point) with arguments described by a calling convention.
		/// </summary>
		/// <param name="callingConventions"></param>
		/// <param name="returnType"></param>
		/// <param name="parameterTypes"></param>
		/// <param name="optionalParameterTypes"></param>
		/// <returns></returns>
		public static ILGenerator Calli(this ILGenerator ILGenerator, CallingConventions callingConventions, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes) { ILGenerator.EmitCalli(OpCodes.Calli, callingConventions, returnType, parameterTypes, optionalParameterTypes); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Calli, new object[] { callingConventions, returnType, parameterTypes, optionalParameterTypes }) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Calli, new object[] { callingConventions, returnType, parameterTypes, optionalParameterTypes })); return ILGenerator; }
		/// <summary>
		/// Calls the method indicated on the evaluation stack (as a pointer to an entry point) with arguments described by a calling convention.
		/// </summary>
		/// <param name="callingConvention"></param>
		/// <param name="returnType"></param>
		/// <param name="parameterTypes"></param>
		/// <returns></returns>
		public static ILGenerator Calli(this ILGenerator ILGenerator, CallingConvention callingConvention, Type returnType, Type[] parameterTypes) { ILGenerator.EmitCalli(OpCodes.Calli, callingConvention, returnType, parameterTypes); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Calli, new object[] { callingConvention, returnType, parameterTypes }) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Calli, new object[] { callingConvention, returnType, parameterTypes })); return ILGenerator; }
		/// <summary>
		/// Unconditionally transfers control to a target instruction (short form).
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Br_S(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Br_S, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Br_S, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Br_S, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction if value is false, a null reference, or zero.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Brfalse_S(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Brfalse_S, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Brfalse_S, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Brfalse_S, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction (short form) if value is true, not null, or non-zero.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Brtrue_S(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Brtrue_S, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Brtrue_S, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Brtrue_S, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction (short form) if two values are equal.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Beq_S(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Beq_S, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Beq_S, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Beq_S, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction (short form) if the first value is greater than or equal to the second value.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Bge_S(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Bge_S, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Bge_S, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Bge_S, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction (short form) if the first value is greater than the second value.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Bgt_S(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Bgt_S, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Bgt_S, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Bgt_S, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction (short form) if the first value is less than or equal to the second value.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Ble_S(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Ble_S, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ble_S, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ble_S, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction (short form) if the first value is less than the second value.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Blt_S(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Blt_S, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Blt_S, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Blt_S, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction (short form) when two unsigned integer values or unordered float values are not equal.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Bne_Un_S(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Bne_Un_S, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Bne_Un_S, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Bne_Un_S, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction (short form) if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Bge_Un_S(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Bge_Un_S, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Bge_Un_S, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Bge_Un_S, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction (short form) if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Bgt_Un_S(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Bgt_Un_S, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Bgt_Un_S, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Bgt_Un_S, label)); return ILGenerator; }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Ble_Un_S(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Ble_Un_S, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ble_Un_S, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ble_Un_S, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction (short form) if the first value is less than or equal to the second value, when comparing unsigned integer values or unordered float values.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Blt_Un_S(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Blt_Un_S, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Blt_Un_S, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Blt_Un_S, label)); return ILGenerator; }
		/// <summary>
		/// Unconditionally transfers control to a target instruction.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Br(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Br, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Br, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Br, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction if value is false, a null reference (Nothing in Visual Basic), or zero.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Brfalse(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Brfalse, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Brfalse, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Brfalse, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction if value is true, not null, or non-zero.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Brtrue(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Brtrue, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Brtrue, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Brtrue, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction if two values are equal.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Beq(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Beq, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Beq, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Beq, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction if the first value is greater than or equal to the second value.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Bge(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Bge, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Bge, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Bge, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction if the first value is greater than the second value.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Bgt(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Bgt, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Bgt, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Bgt, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction if the first value is less than or equal to the second value.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Ble(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Ble, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ble, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ble, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction if the first value is less than the second value.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Blt(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Blt, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Blt, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Blt, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction when two unsigned integer values or unordered float values are not equal.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Bne_Un(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Bne_Un, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Bne_Un, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Bne_Un, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Bge_Un(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Bge_Un, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Bge_Un, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Bge_Un, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Bgt_Un(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Bgt_Un, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Bgt_Un, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Bgt_Un, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction if the first value is less than or equal to the second value, when comparing unsigned integer values or unordered float values.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Ble_Un(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Ble_Un, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ble_Un, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ble_Un, label)); return ILGenerator; }
		/// <summary>
		/// Transfers control to a target instruction if the first value is less than the second value, when comparing unsigned integer values or unordered float values.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Blt_Un(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Blt_Un, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Blt_Un, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Blt_Un, label)); return ILGenerator; }
		/// <summary>
		/// Implements a jump table.
		/// </summary>
		/// <param name="labels"></param>
		/// <returns></returns>
		public static ILGenerator Switch(this ILGenerator ILGenerator, Label[] labels) { ILGenerator.Emit(OpCodes.Switch, labels); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Switch, labels) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Switch, labels)); return ILGenerator; }
		/// <summary>
		/// Calls a late-bound method on an object, pushing the return value onto the evaluation stack.
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public static ILGenerator Callvirt(this ILGenerator ILGenerator, MethodInfo method) { ILGenerator.Emit(OpCodes.Callvirt, method); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Callvirt, method) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Callvirt, method)); return ILGenerator; }
		/// <summary>
		/// Calls a late-bound method on an object, pushing the return value onto the evaluation stack.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public static ILGenerator Callvirt(this ILGenerator ILGenerator, MethodInfo method, Type[] types) { ILGenerator.EmitCall(OpCodes.Callvirt, method, types); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Callvirt, new object[] { method, types }) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Callvirt, new object[] { method, types })); return ILGenerator; }
		/// <summary>
		/// Copies the value type located at the address of an object (type &, or native int) to the address of the destination object (type &, or native int).
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILGenerator Cpobj(this ILGenerator ILGenerator, Type type) { ILGenerator.Emit(OpCodes.Cpobj, type); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Cpobj, type) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Cpobj, type)); return ILGenerator; }
		/// <summary>
		/// Copies the value type object pointed to by an address to the top of the evaluation stack.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILGenerator Ldobj(this ILGenerator ILGenerator, Type type) { ILGenerator.Emit(OpCodes.Ldobj, type); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldobj, type) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldobj, type)); return ILGenerator; }
		/// <summary>
		/// Pushes a new object reference to a string literal stored in the metadata.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static ILGenerator Ldstr(this ILGenerator ILGenerator, string value) { ILGenerator.Emit(OpCodes.Ldstr, value); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldstr, value) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldstr, value)); return ILGenerator; }
		/// <summary>
		/// Creates a new object or a new instance of a value type, pushing an object reference (type O) onto the evaluation stack.
		/// </summary>
		/// <param name="constructor"></param>
		/// <returns></returns>
		public static ILGenerator Newobj(this ILGenerator ILGenerator, ConstructorInfo constructor) { ILGenerator.Emit(OpCodes.Newobj, constructor); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Newobj, constructor) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Newobj, constructor)); return ILGenerator; }
		/// <summary>
		/// Attempts to cast an object passed by reference to the specified class.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILGenerator Castclass(this ILGenerator ILGenerator, Type type) { ILGenerator.Emit(OpCodes.Castclass, type); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Castclass, type) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Castclass, type)); return ILGenerator; }
		/// <summary>
		/// Tests whether an object reference (type O) is an instance of a particular class.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILGenerator Isinst(this ILGenerator ILGenerator, Type type) { ILGenerator.Emit(OpCodes.Isinst, type); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Isinst, type) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Isinst, type)); return ILGenerator; }
		/// <summary>
		/// Converts the boxed representation of a value type to its unboxed form.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILGenerator Unbox(this ILGenerator ILGenerator, Type type) { ILGenerator.Emit(OpCodes.Unbox, type); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Unbox, type) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Unbox, type)); return ILGenerator; }
		/// <summary>
		/// Finds the value of a field in the object whose reference is currently on the evaluation stack.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public static ILGenerator Ldfld(this ILGenerator ILGenerator, FieldInfo field) { ILGenerator.Emit(OpCodes.Ldfld, field); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldfld, field) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldfld, field)); return ILGenerator; }
		/// <summary>
		/// Finds the address of a field in the object whose reference is currently on the evaluation stack.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public static ILGenerator Ldflda(this ILGenerator ILGenerator, FieldInfo field) { ILGenerator.Emit(OpCodes.Ldflda, field); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldflda, field) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldflda, field)); return ILGenerator; }
		/// <summary>
		/// Replaces the value stored in the field of an object reference or pointer with a new value.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public static ILGenerator Stfld(this ILGenerator ILGenerator, FieldInfo field) { ILGenerator.Emit(OpCodes.Stfld, field); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stfld, field) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stfld, field)); return ILGenerator; }
		/// <summary>
		/// Pushes the value of a static field onto the evaluation stack.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public static ILGenerator Ldsfld(this ILGenerator ILGenerator, FieldInfo field) { ILGenerator.Emit(OpCodes.Ldsfld, field); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldsfld, field) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldsfld, field)); return ILGenerator; }
		/// <summary>
		/// Pushes the address of a static field onto the evaluation stack.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public static ILGenerator Ldsflda(this ILGenerator ILGenerator, FieldInfo field) { ILGenerator.Emit(OpCodes.Ldsflda, field); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldsflda, field) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldsflda, field)); return ILGenerator; }
		/// <summary>
		/// Replaces the value of a static field with a value from the evaluation stack.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public static ILGenerator Stsfld(this ILGenerator ILGenerator, FieldInfo field) { ILGenerator.Emit(OpCodes.Stsfld, field); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stsfld, field) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stsfld, field)); return ILGenerator; }
		/// <summary>
		/// Copies a value of a specified type from the evaluation stack into a supplied memory address.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILGenerator Stobj(this ILGenerator ILGenerator, Type type) { ILGenerator.Emit(OpCodes.Stobj, type); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stobj, type) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stobj, type)); return ILGenerator; }
		/// <summary>
		/// Converts a value type to an object reference (type O).
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILGenerator Box(this ILGenerator ILGenerator, Type type) { ILGenerator.Emit(OpCodes.Box, type); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Box, type) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Box, type)); return ILGenerator; }
		/// <summary>
		/// Pushes an object reference to a new zero-based, one-dimensional array whose elements are of a specific type onto the evaluation stack.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILGenerator Newarr(this ILGenerator ILGenerator, Type type) { ILGenerator.Emit(OpCodes.Newarr, type); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Newarr, type) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Newarr, type)); return ILGenerator; }
		/// <summary>
		/// Loads the address of the array element at a specified array index onto the top of the evaluation stack as type & (managed pointer).
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILGenerator Ldelema(this ILGenerator ILGenerator, Type type) { ILGenerator.Emit(OpCodes.Ldelema, type); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldelema, type) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldelema, type)); return ILGenerator; }
		/// <summary>
		/// Loads the element at a specified array index onto the top of the evaluation stack as the type specified in the instruction.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILGenerator Ldelem(this ILGenerator ILGenerator, Type type) { ILGenerator.Emit(OpCodes.Ldelem, type); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldelem, type) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldelem, type)); return ILGenerator; }
		/// <summary>
		/// Replaces the array element at a given index with the value on the evaluation stack, whose type is specified in the instruction.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILGenerator Stelem(this ILGenerator ILGenerator, Type type) { ILGenerator.Emit(OpCodes.Stelem, type); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stelem, type) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stelem, type)); return ILGenerator; }
		/// <summary>
		/// Converts the boxed representation of a type specified in the instruction to its unboxed form.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILGenerator Unbox_Any(this ILGenerator ILGenerator, Type type) { ILGenerator.Emit(OpCodes.Unbox_Any, type); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Unbox_Any, type) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Unbox_Any, type)); return ILGenerator; }
		/// <summary>
		/// Retrieves the address (type &) embedded in a typed reference.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILGenerator Refanyval(this ILGenerator ILGenerator, Type type) { ILGenerator.Emit(OpCodes.Refanyval, type); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Refanyval, type) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Refanyval, type)); return ILGenerator; }
		/// <summary>
		/// Pushes a typed reference to an instance of a specific type onto the evaluation stack.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILGenerator Mkrefany(this ILGenerator ILGenerator, Type type) { ILGenerator.Emit(OpCodes.Mkrefany, type); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Mkrefany, type) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Mkrefany, type)); return ILGenerator; }
		/// <summary>
		/// Converts a metadata token to its runtime representation, pushing it onto the evaluation stack.
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public static ILGenerator Ldtoken(this ILGenerator ILGenerator, MethodInfo method) { ILGenerator.Emit(OpCodes.Ldtoken, method); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldtoken, method) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldtoken, method)); return ILGenerator; }
		/// <summary>
		/// Converts a metadata token to its runtime representation, pushing it onto the evaluation stack.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public static ILGenerator Ldtoken(this ILGenerator ILGenerator, FieldInfo field) { ILGenerator.Emit(OpCodes.Ldtoken, field); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldtoken, field) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldtoken, field)); return ILGenerator; }
		/// <summary>
		/// Converts a metadata token to its runtime representation, pushing it onto the evaluation stack.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILGenerator Ldtoken(this ILGenerator ILGenerator, Type type) { ILGenerator.Emit(OpCodes.Ldtoken, type); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldtoken, type) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldtoken, type)); return ILGenerator; }
		/// <summary>
		/// Exits a protected region of code, unconditionally transferring control to a specific target instruction.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Leave(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Leave, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Leave, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Leave, label)); return ILGenerator; }
		/// <summary>
		/// Exits a protected region of code, unconditionally transferring control to a target instruction (short form).
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Leave_S(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Leave_S, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Leave_S, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Leave_S, label)); return ILGenerator; }
		/// <summary>
		/// Pushes an unmanaged pointer (type native int) to the native code implementing a specific method onto the evaluation stack.
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public static ILGenerator Ldftn(this ILGenerator ILGenerator, MethodInfo method) { ILGenerator.Emit(OpCodes.Ldftn, method); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldftn, method) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldftn, method)); return ILGenerator; }
		/// <summary>
		/// Pushes an unmanaged pointer (type native int) to the native code implementing a particular virtual method associated with a specified object onto the evaluation stack.
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public static ILGenerator Ldvirtftn(this ILGenerator ILGenerator, MethodInfo method) { ILGenerator.Emit(OpCodes.Ldvirtftn, method); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldvirtftn, method) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldvirtftn, method)); return ILGenerator; }
		/// <summary>
		/// Loads an argument (referenced by a specified index value) onto the stack.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static ILGenerator Ldarg(this ILGenerator ILGenerator, short index) { ILGenerator.Emit(OpCodes.Ldarg, index); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldarg, index) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldarg, index)); return ILGenerator; }
		/// <summary>
		/// Load an argument address onto the evaluation stack.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static ILGenerator Ldarga(this ILGenerator ILGenerator, short index) { ILGenerator.Emit(OpCodes.Ldarga, index); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldarga, index) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldarga, index)); return ILGenerator; }
		/// <summary>
		/// Stores the value on top of the evaluation stack in the argument slot at a specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static ILGenerator Starg(this ILGenerator ILGenerator, short index) { ILGenerator.Emit(OpCodes.Starg, index); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Starg, index) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Starg, index)); return ILGenerator; }
		/// <summary>
		/// Loads the local variable at a specific index onto the evaluation stack.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static ILGenerator Ldloc(this ILGenerator ILGenerator, short index) { ILGenerator.Emit(OpCodes.Ldloc, index); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldloc, index) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldloc, index)); return ILGenerator; }
		/// <summary>
		/// Loads the local variable at a specific index onto the evaluation stack.
		/// </summary>
		/// <param name="local"></param>
		/// <returns></returns>
		public static ILGenerator Ldloc(this ILGenerator ILGenerator, LocalBuilder local) { ILGenerator.Emit(OpCodes.Ldloc, local); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldloc, local) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldloc, local)); return ILGenerator; }
		/// <summary>
		/// Loads the address of the local variable at a specific index onto the evaluation stack.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static ILGenerator Ldloca(this ILGenerator ILGenerator, short index) { ILGenerator.Emit(OpCodes.Ldloca, index); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Ldloca, index) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Ldloca, index)); return ILGenerator; }
		/// <summary>
		/// Pops the current value from the top of the evaluation stack and stores it in the local variable list at a specified index.
		/// </summary>
		/// <param name="local"></param>
		/// <returns></returns>
		public static ILGenerator Stloc(this ILGenerator ILGenerator, LocalBuilder local) { ILGenerator.Emit(OpCodes.Stloc, local); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stloc, local) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stloc, local)); return ILGenerator; }
		/// <summary>
		/// Pops the current value from the top of the evaluation stack and stores it in the local variable list at a specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static ILGenerator Stloc(this ILGenerator ILGenerator, short index) { ILGenerator.Emit(OpCodes.Stloc, index); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Stloc, index) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Stloc, index)); return ILGenerator; }
		/// <summary>
		/// Indicates that an address currently atop the evaluation stack might not be aligned to the natural size of the immediately following ldind, stind, ldfld, stfld, ldobj, stobj, initblk, or cpblk instruction.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static ILGenerator Unaligned(this ILGenerator ILGenerator, Label label) { ILGenerator.Emit(OpCodes.Unaligned, label); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Unaligned, label) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Unaligned, label)); return ILGenerator; }
		/// <summary>
		/// Indicates that an address currently atop the evaluation stack might not be aligned to the natural size of the immediately following ldind, stind, ldfld, stfld, ldobj, stobj, initblk, or cpblk instruction.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static ILGenerator Unaligned(this ILGenerator ILGenerator, byte index) { ILGenerator.Emit(OpCodes.Unaligned, index); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Unaligned, index) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Unaligned, index)); return ILGenerator; }
		/// <summary>
		/// Initializes each field of the value type at a specified address to a null reference or a 0 of the appropriate primitive type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILGenerator Initobj(this ILGenerator ILGenerator, Type type) { ILGenerator.Emit(OpCodes.Initobj, type); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Initobj, type) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Initobj, type)); return ILGenerator; }
		/// <summary>
		/// Constrains the type on which a virtual method call is made.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILGenerator Constrained(this ILGenerator ILGenerator, Type type) { ILGenerator.Emit(OpCodes.Constrained, type); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Constrained, type) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Constrained, type)); return ILGenerator; }
		/// <summary>
		/// Pushes the size, in bytes, of a supplied value type onto the evaluation stack.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILGenerator Sizeof(this ILGenerator ILGenerator, Type type) { ILGenerator.Emit(OpCodes.Sizeof, type); Instructions.ComputeIfAbsent(ILGenerator, k => new List<KeyValuePair<OpCode, object>>() { new KeyValuePair<OpCode, object>(OpCodes.Sizeof, type) }).Add(new KeyValuePair<OpCode, object>(OpCodes.Sizeof, type)); return ILGenerator; }
		#endregion
	}
}