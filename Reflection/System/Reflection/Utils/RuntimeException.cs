﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace System.Reflection.Utils
{
	/// <summary>Under Mono, RuntimeException wraps IL compile errors with detailed information about the failure</summary>
	/// 
	[Serializable]
	public class RuntimeException : Exception
	{
		Dictionary<int, CodeInstruction> instructions;
		int errorOffset;

		internal RuntimeException() { }
		internal RuntimeException(string message) : base(message) { }
		internal RuntimeException(string message, Exception innerException) : base(message, innerException) { }

		/// <summary>Default serialization constructor (not implemented)</summary>
		/// <param name="serializationInfo">The info</param>
		/// <param name="streamingContext">The context</param>
		/// 
		protected RuntimeException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
		{
			throw new NotImplementedException();
		}

		internal RuntimeException(Exception innerException, Dictionary<int, CodeInstruction> instructions, int errorOffset) : base("IL Compile Error", innerException)
		{
			this.instructions = instructions;
			this.errorOffset = errorOffset;
		}

		internal static Exception Create(Exception ex, Dictionary<int, CodeInstruction> finalInstructions)
		{
			var match = Regex.Match(ex.Message.TrimEnd(), "Reason: Invalid IL code in.+: IL_(\\d{4}): (.+)$");
			if (match.Success is false) return ex;

			var offset = int.Parse(match.Groups[1].Value, System.Globalization.NumberStyles.HexNumber);
			_ = Regex.Replace(match.Groups[2].Value, " {2,}", " ");
			if (ex is RuntimeException hEx)
			{
				hEx.instructions = finalInstructions;
				hEx.errorOffset = offset;
				return hEx;
			}
			return new RuntimeException(ex, finalInstructions, offset);
		}

		/// <summary>Get a list of IL instructions in pairs of offset+code</summary>
		/// <returns>A list of key/value pairs which represent an offset and the code at that offset</returns>
		/// 
		public List<KeyValuePair<int, CodeInstruction>> GetInstructionsWithOffsets()
		{
			return instructions.OrderBy(ins => ins.Key).ToList();
		}

		/// <summary>Get a list of IL instructions without offsets</summary>
		/// <returns>A list of <see cref="CodeInstruction"/></returns>
		/// 
		public List<CodeInstruction> GetInstructions()
		{
			return instructions.OrderBy(ins => ins.Key).Select(ins => ins.Value).ToList();
		}

		/// <summary>Get the error offset of the errornous IL instruction</summary>
		/// <returns>The offset</returns>
		/// 
		public int GetErrorOffset()
		{
			return errorOffset;
		}

		/// <summary>Get the index of the errornous IL instruction</summary>
		/// <returns>The index into the list of instructions or -1 if not found</returns>
		/// 
		public int GetErrorIndex()
		{
			if (instructions.TryGetValue(errorOffset, out var instruction))
				return GetInstructions().IndexOf(instruction);
			return -1;
		}
	}
}
