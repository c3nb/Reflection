using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Security;
using System.Linq;

namespace System.Reflection.Extensions
{
	[SecurityCritical]
	[SecuritySafeCritical]
	public static unsafe class Reflection
	{
		static Reflection()
		{
			string asmName = "Ref";
			object obj = new object();
			asmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName($"{asmName}{IntPtrUtils.GetPointerTr(ref obj)}"), AssemblyBuilderAccess.Run);
			moduleBuilder = asmBuilder.DefineDynamicModule("Module");
		}
		public static Dictionary<string, OpCode> StringOpCodes = new Dictionary<string, OpCode>()
		{
			{"nop", OpCodes.Nop},
			{"break", OpCodes.Break},
			{"ldarg.0", OpCodes.Ldarg_0},
			{"ldarg.1", OpCodes.Ldarg_1},
			{"ldarg.2", OpCodes.Ldarg_2},
			{"ldarg.3", OpCodes.Ldarg_3},
			{"ldloc.0", OpCodes.Ldloc_0},
			{"ldloc.1", OpCodes.Ldloc_1},
			{"ldloc.2", OpCodes.Ldloc_2},
			{"ldloc.3", OpCodes.Ldloc_3},
			{"stloc.0", OpCodes.Stloc_0},
			{"stloc.1", OpCodes.Stloc_1},
			{"stloc.2", OpCodes.Stloc_2},
			{"stloc.3", OpCodes.Stloc_3},
			{"ldarg.s", OpCodes.Ldarg_S},
			{"ldarga.s", OpCodes.Ldarga_S},
			{"starg.s", OpCodes.Starg_S},
			{"ldloc.s", OpCodes.Ldloc_S},
			{"ldloca.s", OpCodes.Ldloca_S},
			{"stloc.s", OpCodes.Stloc_S},
			{"ldnull", OpCodes.Ldnull},
			{"ldc.i4.m1", OpCodes.Ldc_I4_M1},
			{"ldc.i4.0", OpCodes.Ldc_I4_0},
			{"ldc.i4.1", OpCodes.Ldc_I4_1},
			{"ldc.i4.2", OpCodes.Ldc_I4_2},
			{"ldc.i4.3", OpCodes.Ldc_I4_3},
			{"ldc.i4.4", OpCodes.Ldc_I4_4},
			{"ldc.i4.5", OpCodes.Ldc_I4_5},
			{"ldc.i4.6", OpCodes.Ldc_I4_6},
			{"ldc.i4.7", OpCodes.Ldc_I4_7},
			{"ldc.i4.8", OpCodes.Ldc_I4_8},
			{"ldc.i4.s", OpCodes.Ldc_I4_S},
			{"ldc.i4", OpCodes.Ldc_I4},
			{"ldc.i8", OpCodes.Ldc_I8},
			{"ldc.r4", OpCodes.Ldc_R4},
			{"ldc.r8", OpCodes.Ldc_R8},
			{"dup", OpCodes.Dup},
			{"pop", OpCodes.Pop},
			{"jmp", OpCodes.Jmp},
			{"call", OpCodes.Call},
			{"calli", OpCodes.Calli},
			{"ret", OpCodes.Ret},
			{"br.s", OpCodes.Br_S},
			{"brfalse.s", OpCodes.Brfalse_S},
			{"brtrue.s", OpCodes.Brtrue_S},
			{"beq.s", OpCodes.Beq_S},
			{"bge.s", OpCodes.Bge_S},
			{"bgt.s", OpCodes.Bgt_S},
			{"ble.s", OpCodes.Ble_S},
			{"blt.s", OpCodes.Blt_S},
			{"bne.un.s", OpCodes.Bne_Un_S},
			{"bge.un.s", OpCodes.Bge_Un_S},
			{"bgt.un.s", OpCodes.Bgt_Un_S},
			{"ble.un.s", OpCodes.Ble_Un_S},
			{"blt.un.s", OpCodes.Blt_Un_S},
			{"br", OpCodes.Br},
			{"brfalse", OpCodes.Brfalse},
			{"brtrue", OpCodes.Brtrue},
			{"beq", OpCodes.Beq},
			{"bge", OpCodes.Bge},
			{"bgt", OpCodes.Bgt},
			{"ble", OpCodes.Ble},
			{"blt", OpCodes.Blt},
			{"bne.un", OpCodes.Bne_Un},
			{"bge.un", OpCodes.Bge_Un},
			{"bgt.un", OpCodes.Bgt_Un},
			{"ble.un", OpCodes.Ble_Un},
			{"blt.un", OpCodes.Blt_Un},
			{"switch", OpCodes.Switch},
			{"ldind.i1", OpCodes.Ldind_I1},
			{"ldind.u1", OpCodes.Ldind_U1},
			{"ldind.i2", OpCodes.Ldind_I2},
			{"ldind.u2", OpCodes.Ldind_U2},
			{"ldind.i4", OpCodes.Ldind_I4},
			{"ldind.u4", OpCodes.Ldind_U4},
			{"ldind.i8", OpCodes.Ldind_I8},
			{"ldind.i", OpCodes.Ldind_I},
			{"ldind.r4", OpCodes.Ldind_R4},
			{"ldind.r8", OpCodes.Ldind_R8},
			{"ldind.ref", OpCodes.Ldind_Ref},
			{"stind.ref", OpCodes.Stind_Ref},
			{"stind.i1", OpCodes.Stind_I1},
			{"stind.i2", OpCodes.Stind_I2},
			{"stind.i4", OpCodes.Stind_I4},
			{"stind.i8", OpCodes.Stind_I8},
			{"stind.r4", OpCodes.Stind_R4},
			{"stind.r8", OpCodes.Stind_R8},
			{"add", OpCodes.Add},
			{"sub", OpCodes.Sub},
			{"mul", OpCodes.Mul},
			{"div", OpCodes.Div},
			{"div.un", OpCodes.Div_Un},
			{"rem", OpCodes.Rem},
			{"rem.un", OpCodes.Rem_Un},
			{"and", OpCodes.And},
			{"or", OpCodes.Or},
			{"xor", OpCodes.Xor},
			{"shl", OpCodes.Shl},
			{"shr", OpCodes.Shr},
			{"shr.un", OpCodes.Shr_Un},
			{"neg", OpCodes.Neg},
			{"not", OpCodes.Not},
			{"conv.i1", OpCodes.Conv_I1},
			{"conv.i2", OpCodes.Conv_I2},
			{"conv.i4", OpCodes.Conv_I4},
			{"conv.i8", OpCodes.Conv_I8},
			{"conv.r4", OpCodes.Conv_R4},
			{"conv.r8", OpCodes.Conv_R8},
			{"conv.u4", OpCodes.Conv_U4},
			{"conv.u8", OpCodes.Conv_U8},
			{"callvirt", OpCodes.Callvirt},
			{"cpobj", OpCodes.Cpobj},
			{"ldobj", OpCodes.Ldobj},
			{"ldstr", OpCodes.Ldstr},
			{"newobj", OpCodes.Newobj},
			{"castclass", OpCodes.Castclass},
			{"isinst", OpCodes.Isinst},
			{"conv.r.un", OpCodes.Conv_R_Un},
			{"unbox", OpCodes.Unbox},
			{"throw", OpCodes.Throw},
			{"ldfld", OpCodes.Ldfld},
			{"ldflda", OpCodes.Ldflda},
			{"stfld", OpCodes.Stfld},
			{"ldsfld", OpCodes.Ldsfld},
			{"ldsflda", OpCodes.Ldsflda},
			{"stsfld", OpCodes.Stsfld},
			{"stobj", OpCodes.Stobj},
			{"conv.ovf.i1.un", OpCodes.Conv_Ovf_I1_Un},
			{"conv.ovf.i2.un", OpCodes.Conv_Ovf_I2_Un},
			{"conv.ovf.i4.un", OpCodes.Conv_Ovf_I4_Un},
			{"conv.ovf.i8.un", OpCodes.Conv_Ovf_I8_Un},
			{"conv.ovf.u1.un", OpCodes.Conv_Ovf_U1_Un},
			{"conv.ovf.u2.un", OpCodes.Conv_Ovf_U2_Un},
			{"conv.ovf.u4.un", OpCodes.Conv_Ovf_U4_Un},
			{"conv.ovf.u8.un", OpCodes.Conv_Ovf_U8_Un},
			{"conv.ovf.i.un", OpCodes.Conv_Ovf_I_Un},
			{"conv.ovf.u.un", OpCodes.Conv_Ovf_U_Un},
			{"box", OpCodes.Box},
			{"newarr", OpCodes.Newarr},
			{"ldlen", OpCodes.Ldlen},
			{"ldelema", OpCodes.Ldelema},
			{"ldelem.i1", OpCodes.Ldelem_I1},
			{"ldelem.u1", OpCodes.Ldelem_U1},
			{"ldelem.i2", OpCodes.Ldelem_I2},
			{"ldelem.u2", OpCodes.Ldelem_U2},
			{"ldelem.i4", OpCodes.Ldelem_I4},
			{"ldelem.u4", OpCodes.Ldelem_U4},
			{"ldelem.i8", OpCodes.Ldelem_I8},
			{"ldelem.i", OpCodes.Ldelem_I},
			{"ldelem.r4", OpCodes.Ldelem_R4},
			{"ldelem.r8", OpCodes.Ldelem_R8},
			{"ldelem.ref", OpCodes.Ldelem_Ref},
			{"stelem.i", OpCodes.Stelem_I},
			{"stelem.i1", OpCodes.Stelem_I1},
			{"stelem.i2", OpCodes.Stelem_I2},
			{"stelem.i4", OpCodes.Stelem_I4},
			{"stelem.i8", OpCodes.Stelem_I8},
			{"stelem.r4", OpCodes.Stelem_R4},
			{"stelem.r8", OpCodes.Stelem_R8},
			{"stelem.ref", OpCodes.Stelem_Ref},
			{"ldelem", OpCodes.Ldelem},
			{"stelem", OpCodes.Stelem},
			{"unbox.any", OpCodes.Unbox_Any},
			{"conv.ovf.i1", OpCodes.Conv_Ovf_I1},
			{"conv.ovf.u1", OpCodes.Conv_Ovf_U1},
			{"conv.ovf.i2", OpCodes.Conv_Ovf_I2},
			{"conv.ovf.u2", OpCodes.Conv_Ovf_U2},
			{"conv.ovf.i4", OpCodes.Conv_Ovf_I4},
			{"conv.ovf.u4", OpCodes.Conv_Ovf_U4},
			{"conv.ovf.i8", OpCodes.Conv_Ovf_I8},
			{"conv.ovf.u8", OpCodes.Conv_Ovf_U8},
			{"refanyval", OpCodes.Refanyval},
			{"ckfinite", OpCodes.Ckfinite},
			{"mkrefany", OpCodes.Mkrefany},
			{"ldtoken", OpCodes.Ldtoken},
			{"conv.u2", OpCodes.Conv_U2},
			{"conv.u1", OpCodes.Conv_U1},
			{"conv.i", OpCodes.Conv_I},
			{"conv.ovf.i", OpCodes.Conv_Ovf_I},
			{"conv.ovf.u", OpCodes.Conv_Ovf_U},
			{"add.ovf", OpCodes.Add_Ovf},
			{"add.ovf.un", OpCodes.Add_Ovf_Un},
			{"mul.ovf", OpCodes.Mul_Ovf},
			{"mul.ovf.un", OpCodes.Mul_Ovf_Un},
			{"sub.ovf", OpCodes.Sub_Ovf},
			{"sub.ovf.un", OpCodes.Sub_Ovf_Un},
			{"endfinally", OpCodes.Endfinally},
			{"leave", OpCodes.Leave},
			{"leave.s", OpCodes.Leave_S},
			{"stind.i", OpCodes.Stind_I},
			{"conv.u", OpCodes.Conv_U},
			{"prefix7", OpCodes.Prefix7},
			{"prefix6", OpCodes.Prefix6},
			{"prefix5", OpCodes.Prefix5},
			{"prefix4", OpCodes.Prefix4},
			{"prefix3", OpCodes.Prefix3},
			{"prefix2", OpCodes.Prefix2},
			{"prefix1", OpCodes.Prefix1},
			{"prefixref", OpCodes.Prefixref},
			{"arglist", OpCodes.Arglist},
			{"ceq", OpCodes.Ceq},
			{"cgt", OpCodes.Cgt},
			{"cgt.un", OpCodes.Cgt_Un},
			{"clt", OpCodes.Clt},
			{"clt.un", OpCodes.Clt_Un},
			{"ldftn", OpCodes.Ldftn},
			{"ldvirtftn", OpCodes.Ldvirtftn},
			{"ldarg", OpCodes.Ldarg},
			{"ldarga", OpCodes.Ldarga},
			{"starg", OpCodes.Starg},
			{"ldloc", OpCodes.Ldloc},
			{"ldloca", OpCodes.Ldloca},
			{"stloc", OpCodes.Stloc},
			{"localloc", OpCodes.Localloc},
			{"endfilter", OpCodes.Endfilter},
			{"unaligned.", OpCodes.Unaligned},
			{"volatile.", OpCodes.Volatile},
			{"tail.", OpCodes.Tailcall},
			{"initobj", OpCodes.Initobj},
			{"constrained.", OpCodes.Constrained},
			{"cpblk", OpCodes.Cpblk},
			{"initblk", OpCodes.Initblk},
			{"rethrow", OpCodes.Rethrow},
			{"sizeof", OpCodes.Sizeof},
			{"refanytype", OpCodes.Refanytype},
			{"readonly.", OpCodes.Readonly},
		};
		public static Dictionary<int, OpCode> OneByteOpCodes = new Dictionary<int, OpCode>()
		{
				{0, OpCodes.Nop},
				{1, OpCodes.Break},
				{2, OpCodes.Ldarg_0},
				{3, OpCodes.Ldarg_1},
				{4, OpCodes.Ldarg_2},
				{5, OpCodes.Ldarg_3},
				{6, OpCodes.Ldloc_0},
				{7, OpCodes.Ldloc_1},
				{8, OpCodes.Ldloc_2},
				{9, OpCodes.Ldloc_3},
				{10, OpCodes.Stloc_0},
				{11, OpCodes.Stloc_1},
				{12, OpCodes.Stloc_2},
				{13, OpCodes.Stloc_3},
				{14, OpCodes.Ldarg_S},
				{15, OpCodes.Ldarga_S},
				{16, OpCodes.Starg_S},
				{17, OpCodes.Ldloc_S},
				{18, OpCodes.Ldloca_S},
				{19, OpCodes.Stloc_S},
				{20, OpCodes.Ldnull},
				{21, OpCodes.Ldc_I4_M1},
				{22, OpCodes.Ldc_I4_0},
				{23, OpCodes.Ldc_I4_1},
				{24, OpCodes.Ldc_I4_2},
				{25, OpCodes.Ldc_I4_3},
				{26, OpCodes.Ldc_I4_4},
				{27, OpCodes.Ldc_I4_5},
				{28, OpCodes.Ldc_I4_6},
				{29, OpCodes.Ldc_I4_7},
				{30, OpCodes.Ldc_I4_8},
				{31, OpCodes.Ldc_I4_S},
				{32, OpCodes.Ldc_I4},
				{33, OpCodes.Ldc_I8},
				{34, OpCodes.Ldc_R4},
				{35, OpCodes.Ldc_R8},
				{37, OpCodes.Dup},
				{38, OpCodes.Pop},
				{39, OpCodes.Jmp},
				{40, OpCodes.Call},
				{41, OpCodes.Calli},
				{42, OpCodes.Ret},
				{43, OpCodes.Br_S},
				{44, OpCodes.Brfalse_S},
				{45, OpCodes.Brtrue_S},
				{46, OpCodes.Beq_S},
				{47, OpCodes.Bge_S},
				{48, OpCodes.Bgt_S},
				{49, OpCodes.Ble_S},
				{50, OpCodes.Blt_S},
				{51, OpCodes.Bne_Un_S},
				{52, OpCodes.Bge_Un_S},
				{53, OpCodes.Bgt_Un_S},
				{54, OpCodes.Ble_Un_S},
				{55, OpCodes.Blt_Un_S},
				{56, OpCodes.Br},
				{57, OpCodes.Brfalse},
				{58, OpCodes.Brtrue},
				{59, OpCodes.Beq},
				{60, OpCodes.Bge},
				{61, OpCodes.Bgt},
				{62, OpCodes.Ble},
				{63, OpCodes.Blt},
				{64, OpCodes.Bne_Un},
				{65, OpCodes.Bge_Un},
				{66, OpCodes.Bgt_Un},
				{67, OpCodes.Ble_Un},
				{68, OpCodes.Blt_Un},
				{69, OpCodes.Switch},
				{70, OpCodes.Ldind_I1},
				{71, OpCodes.Ldind_U1},
				{72, OpCodes.Ldind_I2},
				{73, OpCodes.Ldind_U2},
				{74, OpCodes.Ldind_I4},
				{75, OpCodes.Ldind_U4},
				{76, OpCodes.Ldind_I8},
				{77, OpCodes.Ldind_I},
				{78, OpCodes.Ldind_R4},
				{79, OpCodes.Ldind_R8},
				{80, OpCodes.Ldind_Ref},
				{81, OpCodes.Stind_Ref},
				{82, OpCodes.Stind_I1},
				{83, OpCodes.Stind_I2},
				{84, OpCodes.Stind_I4},
				{85, OpCodes.Stind_I8},
				{86, OpCodes.Stind_R4},
				{87, OpCodes.Stind_R8},
				{88, OpCodes.Add},
				{89, OpCodes.Sub},
				{90, OpCodes.Mul},
				{91, OpCodes.Div},
				{92, OpCodes.Div_Un},
				{93, OpCodes.Rem},
				{94, OpCodes.Rem_Un},
				{95, OpCodes.And},
				{96, OpCodes.Or},
				{97, OpCodes.Xor},
				{98, OpCodes.Shl},
				{99, OpCodes.Shr},
				{100, OpCodes.Shr_Un},
				{101, OpCodes.Neg},
				{102, OpCodes.Not},
				{103, OpCodes.Conv_I1},
				{104, OpCodes.Conv_I2},
				{105, OpCodes.Conv_I4},
				{106, OpCodes.Conv_I8},
				{107, OpCodes.Conv_R4},
				{108, OpCodes.Conv_R8},
				{109, OpCodes.Conv_U4},
				{110, OpCodes.Conv_U8},
				{111, OpCodes.Callvirt},
				{112, OpCodes.Cpobj},
				{113, OpCodes.Ldobj},
				{114, OpCodes.Ldstr},
				{115, OpCodes.Newobj},
				{116, OpCodes.Castclass},
				{117, OpCodes.Isinst},
				{118, OpCodes.Conv_R_Un},
				{121, OpCodes.Unbox},
				{122, OpCodes.Throw},
				{123, OpCodes.Ldfld},
				{124, OpCodes.Ldflda},
				{125, OpCodes.Stfld},
				{126, OpCodes.Ldsfld},
				{127, OpCodes.Ldsflda},
				{128, OpCodes.Stsfld},
				{129, OpCodes.Stobj},
				{130, OpCodes.Conv_Ovf_I1_Un},
				{131, OpCodes.Conv_Ovf_I2_Un},
				{132, OpCodes.Conv_Ovf_I4_Un},
				{133, OpCodes.Conv_Ovf_I8_Un},
				{134, OpCodes.Conv_Ovf_U1_Un},
				{135, OpCodes.Conv_Ovf_U2_Un},
				{136, OpCodes.Conv_Ovf_U4_Un},
				{137, OpCodes.Conv_Ovf_U8_Un},
				{138, OpCodes.Conv_Ovf_I_Un},
				{139, OpCodes.Conv_Ovf_U_Un},
				{140, OpCodes.Box},
				{141, OpCodes.Newarr},
				{142, OpCodes.Ldlen},
				{143, OpCodes.Ldelema},
				{144, OpCodes.Ldelem_I1},
				{145, OpCodes.Ldelem_U1},
				{146, OpCodes.Ldelem_I2},
				{147, OpCodes.Ldelem_U2},
				{148, OpCodes.Ldelem_I4},
				{149, OpCodes.Ldelem_U4},
				{150, OpCodes.Ldelem_I8},
				{151, OpCodes.Ldelem_I},
				{152, OpCodes.Ldelem_R4},
				{153, OpCodes.Ldelem_R8},
				{154, OpCodes.Ldelem_Ref},
				{155, OpCodes.Stelem_I},
				{156, OpCodes.Stelem_I1},
				{157, OpCodes.Stelem_I2},
				{158, OpCodes.Stelem_I4},
				{159, OpCodes.Stelem_I8},
				{160, OpCodes.Stelem_R4},
				{161, OpCodes.Stelem_R8},
				{162, OpCodes.Stelem_Ref},
				{163, OpCodes.Ldelem},
				{164, OpCodes.Stelem},
				{165, OpCodes.Unbox_Any},
				{179, OpCodes.Conv_Ovf_I1},
				{180, OpCodes.Conv_Ovf_U1},
				{181, OpCodes.Conv_Ovf_I2},
				{182, OpCodes.Conv_Ovf_U2},
				{183, OpCodes.Conv_Ovf_I4},
				{184, OpCodes.Conv_Ovf_U4},
				{185, OpCodes.Conv_Ovf_I8},
				{186, OpCodes.Conv_Ovf_U8},
				{194, OpCodes.Refanyval},
				{195, OpCodes.Ckfinite},
				{198, OpCodes.Mkrefany},
				{208, OpCodes.Ldtoken},
				{209, OpCodes.Conv_U2},
				{210, OpCodes.Conv_U1},
				{211, OpCodes.Conv_I},
				{212, OpCodes.Conv_Ovf_I},
				{213, OpCodes.Conv_Ovf_U},
				{214, OpCodes.Add_Ovf},
				{215, OpCodes.Add_Ovf_Un},
				{216, OpCodes.Mul_Ovf},
				{217, OpCodes.Mul_Ovf_Un},
				{218, OpCodes.Sub_Ovf},
				{219, OpCodes.Sub_Ovf_Un},
				{220, OpCodes.Endfinally},
				{221, OpCodes.Leave},
				{222, OpCodes.Leave_S},
				{223, OpCodes.Stind_I},
				{224, OpCodes.Conv_U},
				{248, OpCodes.Prefix7},
				{249, OpCodes.Prefix6},
				{250, OpCodes.Prefix5},
				{251, OpCodes.Prefix4},
				{252, OpCodes.Prefix3},
				{253, OpCodes.Prefix2},
				{254, OpCodes.Prefix1},
				{255, OpCodes.Prefixref},
		};
		public static Dictionary<int, OpCode> TwoByteOpCodes = new Dictionary<int, OpCode>()
		{
				{0, OpCodes.Arglist},
				{1, OpCodes.Ceq},
				{2, OpCodes.Cgt},
				{3, OpCodes.Cgt_Un},
				{4, OpCodes.Clt},
				{5, OpCodes.Clt_Un},
				{6, OpCodes.Ldftn},
				{7, OpCodes.Ldvirtftn},
				{9, OpCodes.Ldarg},
				{10, OpCodes.Ldarga},
				{11, OpCodes.Starg},
				{12, OpCodes.Ldloc},
				{13, OpCodes.Ldloca},
				{14, OpCodes.Stloc},
				{15, OpCodes.Localloc},
				{17, OpCodes.Endfilter},
				{18, OpCodes.Unaligned},
				{19, OpCodes.Volatile},
				{20, OpCodes.Tailcall},
				{21, OpCodes.Initobj},
				{22, OpCodes.Constrained},
				{23, OpCodes.Cpblk},
				{24, OpCodes.Initblk},
				{26, OpCodes.Rethrow},
				{28, OpCodes.Sizeof},
				{29, OpCodes.Refanytype},
				{30, OpCodes.Readonly},
		};
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> forEach)
		{
			foreach (T t in enumerable)
				forEach(t);
		}
		public static T As<T>(this object obj) where T : class => obj as T ?? (T)obj;
		public static T Cast<T>(this object obj) => (T)obj;
		public static bool GetCustomAttribute<T>(this MemberInfo member, out T attr, bool inherit = false) where T : Attribute
		{
			attr = member.GetCustomAttribute<T>(inherit);
			return attr != null;
		}
		public static bool GetCustomAttributes<T>(this MemberInfo member, out IEnumerable<T> attrs, bool inherit = false) where T : Attribute
		{
			attrs = member.GetCustomAttributes<T>(inherit);
			return attrs.Any();
		}
		/// <summary>
		/// Invokes method safe with parameter
		/// </summary>
		/// <param name="method">To invoke method</param>
		/// <param name="instance">Method's instance (if method is static, it can be null)</param>
		/// <param name="parameters">Not support null value yet</param>
		/// <returns>Method's return value</returns>
		public static object InvokeSafe(this MethodInfo method, object instance, params object[] parameters)
		{
			ParameterInfo[] methodParams = method.GetParameters();
			if (methodParams.Length > parameters.Length) return null;
			var tmp = new Dictionary<Type, List<object>>();
			Dictionary<Type, int> idx = new Dictionary<Type, int>();
			var tmpArr = parameters.Select(o => o?.GetType()).ToArray();
			for (int i = 0; i < parameters.Length; i++)
			{
				Type type = tmpArr[i];
				object param = parameters[i];
				if (tmp.TryGetValue(type, out var list))
					list.Add(param);
				else
					tmp.Add(type, new List<object>() { param });
				if (!idx.ContainsKey(type))
					idx.Add(type, 0);
			}
			List<object> paramList = new List<object>();
			List<Type> types = methodParams.Select(o => o.ParameterType).ToList();
			foreach (Type type in types)
				paramList.Add(tmp[type][idx[type]++]);
			return method.Invoke(instance, paramList.ToArray());
		}
		public static T AddCache<T>(this ICollection<T> list, T value)
		{
			list.Add(value);
			return value;
		}
		public static bool IsStruct(this Type type) =>
			type.IsValueType && type.IsPrimitive == false && type.IsEnum == false && type != typeof(void);
		public static IntPtr Compile(this DynamicMethod meth)
		{
			MethodInfo m_compileMeth = typeof(RuntimeHelpers).GetMethod("_CompileMethod", BindingFlags.NonPublic | BindingFlags.Static);
			Type t = meth.GetType();
			RuntimeMethodHandle handle =
					t.GetMethod("GetMethodDescriptor", BindingFlags.NonPublic | BindingFlags.Instance) is MethodInfo getmd
					? (RuntimeMethodHandle)getmd.Invoke(meth, null)
					: t.GetField("m_method", BindingFlags.NonPublic | BindingFlags.Instance) is FieldInfo mm
					? (RuntimeMethodHandle)mm.GetValue(meth)
					: (RuntimeMethodHandle)t.GetField("mhandle", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(meth);
			Type ht = handle.GetType();
			object result =
				ht.GetField("m_value", BindingFlags.NonPublic | BindingFlags.Instance) is FieldInfo fd
				? fd.GetValue(handle)
				: ht.GetMethod("GetMethodInfo", BindingFlags.NonPublic | BindingFlags.Instance) is MethodInfo mi
				? mi.Invoke(handle, null)
				: null;
			if (result != null)
            {
				try
				{
					m_compileMeth.Invoke(null, new object[1] { result });
					return handle.GetFunctionPointer();
				}
				catch { }
			}
			ParameterInfo p = m_compileMeth.GetParameters()[0];
			if (p.ParameterType.IsAssignableFrom(typeof(IntPtr)))
			{
				m_compileMeth.Invoke(null, new object[1] { handle.Value });
				return handle.GetFunctionPointer();
			}
			if (p.ParameterType.IsAssignableFrom(ht))
				m_compileMeth.Invoke(null, new object[1] { handle });
			return handle.GetFunctionPointer();
		}
		public static int SizeOf(this Type type)
		{
			return (int)sizeOf.MakeGenericMethod(type).Invoke(null, null);
		}
		public static int SizeOf<T>()
        {
			//sizeof !!T
			//ret
			//Emit this instructions via dnspy.
			throw null;
		}
		public const BindingFlags all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty;
		static readonly AssemblyBuilder asmBuilder;
		static readonly ModuleBuilder moduleBuilder;
		static readonly MethodInfo sizeOf = typeof(Reflection).GetMethod("SizeOf", Type.EmptyTypes).GetGenericMethodDefinition();
		public static int TypeCount = 0;
		public static MethodInfo ToDynamicMethod<T>(this T del) where T : Delegate
		{
			Type delType = del.GetType();
			MethodInfo method = delType.GetMethod("Invoke");
			MethodInfo emitMethod = del.Method;
			TypeBuilder typeBuilder = moduleBuilder.DefineType($"Type{TypeCount++}", System.Reflection.TypeAttributes.Public | System.Reflection.TypeAttributes.Class);
			string methodName = "Method";
			ParameterInfo[] parameters = emitMethod.GetParameters();
			Type[] parameterTypes = parameters.Select(x => x.ParameterType).ToArray();
			MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodName, System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.Static, CallingConventions.Standard, emitMethod.ReturnType, parameterTypes);
			for (int i = 0; i < parameters.Length; i++)
				methodBuilder.DefineParameter(i + 1, parameters[i].Attributes, parameters[i].Name);
			FieldBuilder fieldBuilder = typeBuilder.DefineField("patch", delType, System.Reflection.FieldAttributes.Public | System.Reflection.FieldAttributes.Static);
			ILGenerator il = methodBuilder.GetILGenerator();
			il.Emit(OpCodes.Ldsfld, fieldBuilder);
			for (int i = 0; i < parameters.Length; i++)
			{
				if (i == 0)
					il.Emit(OpCodes.Ldarg_0);
				else if (i == 1)
					il.Emit(OpCodes.Ldarg_1);
				else if (i == 2)
					il.Emit(OpCodes.Ldarg_2);
				else if (i == 3)
					il.Emit(OpCodes.Ldarg_3);
				else
					il.Emit(OpCodes.Ldarg, i);
			}
			il.Emit(OpCodes.Callvirt, method);
			il.Emit(OpCodes.Ret);
			Type type = typeBuilder.CreateType();
			MethodInfo defined = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
			type.GetField("patch").SetValue(null, del);
			return defined;
		}
		public static FieldInfo GetFieldInfo(this object obj, string name) => obj.GetType().GetField(name, all);
		public static PropertyInfo GetPropertyInfo(this object obj, string name) => obj.GetType().GetProperty(name, all);
		public static MethodInfo GetMethodInfo(this object obj, string name, Type[] types) => obj.GetType().GetMethod(name, all, null, types, null);
		public static MethodInfo GetMethodInfo(this object obj, string name) => obj.GetType().GetMethod(name, all);
		public static object GetField(this object obj, string name) => obj.GetFieldInfo(name)?.GetValue(obj);
		public static object GetProperty(this object obj, string name)
		{
			PropertyInfo prop = obj.GetPropertyInfo(name);
			if (prop.GetAccessors(true)[0].IsStatic)
				return prop?.GetValue(null);
			else
				return prop?.GetValue(obj);
		}
		public static Indexer GetIndexer(this object obj) => new Indexer(GetPropertyInfo(obj, "Item"), obj);
		public static object Invoke(this object obj, string name, params object[] parameters)
		{
			MethodInfo method = obj.GetMethodInfo(name, parameters.Select(t => t.GetType()).ToArray());
			if (method.IsStatic)
				return method?.Invoke(null, parameters);
			else
				return method?.Invoke(obj, parameters);
		}
		public static T GetField<T>(this object obj, string name) => (T)GetField(obj, name);
		public static T GetProperty<T>(this object obj, string name) => (T)GetProperty(obj, name);
		public static T Invoke<T>(this object obj, string name, params object[] parameters) => (T)Invoke(obj, name, parameters);
		public static Indexer<T> GetIndexer<T>(this object obj) => new Indexer<T>(GetPropertyInfo(obj, "Item"), obj);
		public static void SetProperty<T>(this object obj, string name, object value) => GetPropertyInfo(obj, name)?.SetValue(obj, value);
		public static void SetField(this object obj, string name, object value) => GetFieldInfo(obj, name)?.SetValue(obj, value);
		public static T Construct<T>(params object[] parameters) => (T)Construct(typeof(T), parameters);
		public static object Construct(this Type type, params object[] parameters) => type.GetConstructor(parameters.Select(x => x.GetType()).ToArray()).Invoke(null, parameters);
		public static IntPtr GetFunctionPointer(this MethodInfo method) => method.MethodHandle.GetFunctionPointer();
		public static Type GetType(string @namespace, string typeName, string assemblyName = null) => Type.GetType($"{@namespace}.{typeName}{(assemblyName == null ? "" : $", {assemblyName}")}");
		public static MemberInfo GetInfo<T>(this object obj, string name)
		{
			FieldInfo field;
			PropertyInfo prop;
			MethodInfo method;
			if ((field = obj.GetFieldInfo(name)) != null)
				return field;
			else if ((prop = obj.GetPropertyInfo(name)) != null)
				return prop;
			else if ((method = obj.GetMethodInfo(name)) != null)
				return method;
			else
				return null;
		}
		public static object Get(this object obj, string name)
		{
			FieldInfo field;
			PropertyInfo prop;
			if ((field = obj.GetFieldInfo(name)) != null)
				return field.GetValue(obj);
			else if ((prop = obj.GetPropertyInfo(name)) != null)
				if (prop.GetAccessors()[0].IsStatic)
					return prop.GetValue(null);
				else
					return prop.GetValue(obj);
			else
				return null;
		}
		public static void Set(this object obj, string name, object value)
		{
			FieldInfo field;
			PropertyInfo prop = null;
			if ((field = obj.GetFieldInfo(name)) != null)
				field.SetValue(obj, value);
			else if ((prop = obj.GetPropertyInfo(name)) != null)
				if (prop.GetAccessors()[0].IsStatic)
					prop.SetValue(null, value);
				else
					prop.SetValue(obj, value);
		}
		public static CustomAttributeBuilder[] GetDefaultAttributes(string assemblyName)
		{
			return new[]
			{
				new CustomAttributeBuilder(typeof(AssemblyTitleAttribute).GetConstructor(new[] { typeof(string) }), new object[] { assemblyName }),
				new CustomAttributeBuilder(typeof(AssemblyDescriptionAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "" }),
				new CustomAttributeBuilder(typeof(AssemblyConfigurationAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "" }),
				new CustomAttributeBuilder(typeof(AssemblyCompanyAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "" }),
				new CustomAttributeBuilder(typeof(AssemblyProductAttribute).GetConstructor(new[] { typeof(string) }), new object[] { assemblyName }),
				new CustomAttributeBuilder(typeof(AssemblyCopyrightAttribute).GetConstructor(new[] { typeof(string) }), new object[] { $"Copyright ©  {DateTime.Now.Year}" }),
				new CustomAttributeBuilder(typeof(AssemblyTrademarkAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "" }),
				new CustomAttributeBuilder(typeof(AssemblyCultureAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "" }),
				new CustomAttributeBuilder(typeof(System.Runtime.InteropServices.ComVisibleAttribute).GetConstructor(new[] { typeof(bool) }), new object[] { false }),
				new CustomAttributeBuilder(typeof(AssemblyVersionAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "1.0.0.0" }),
				new CustomAttributeBuilder(typeof(AssemblyFileVersionAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "1.0.0.0" })
			};
		}
	}
}