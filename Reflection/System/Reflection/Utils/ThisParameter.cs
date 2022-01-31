namespace System.Reflection.Utils
{
	class ThisParameter : ParameterInfo
	{
		internal ThisParameter(MethodBase method)
		{
			MemberImpl = method;
			ClassImpl = method.DeclaringType;
			NameImpl = "this";
			PositionImpl = -1;
		}
	}
}
