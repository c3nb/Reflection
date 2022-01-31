using System;
using System.Linq;
using System.Dynamic;
using System.Reflection;

namespace System.Reflection.Extensions
{
	public class ReflectObject : DynamicObject
	{
		object instance;
		Type type;
		public ReflectObject(object instance)
		{
			this.instance = instance;
			type = instance.GetType();
		}
		public ReflectObject(Type type)
		{
			this.type = type;
			instance = null;
		}
		public ReflectObject(Type type, object instance)
		{
			this.type = type;
			this.instance = instance;
		}
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			string name = binder.Name;
			FieldInfo fi;
			PropertyInfo pi;
			if ((fi = type.GetField(name, Reflection.all)) != null)
			{
				if (fi.IsStatic)
					result = fi.GetValue(null);
				else
					result = fi.GetValue(instance);
				return true;
			}
			else if ((pi = type.GetProperty(name, Reflection.all)) != null)
			{
				MethodInfo getMethod;
				if ((getMethod = pi.GetGetMethod()) != null)
				{
					if (getMethod.IsStatic)
						result = pi.GetValue(null);
					else
						result = pi.GetValue(instance);
					return true;
				}
			}
			result = null;
			return false;
		}
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			string name = binder.Name;
			FieldInfo fi;
			PropertyInfo pi;
			if ((fi = type.GetField(name, Reflection.all)) != null)
			{
				if (fi.IsStatic)
					fi.SetValue(null, value);
				else
					fi.SetValue(instance, value);
				return true;
			}
			else if ((pi = type.GetProperty(name, Reflection.all)) != null)
			{
				MethodInfo setMethod;
				if ((setMethod = pi.GetSetMethod()) != null)
				{
					if (setMethod.IsStatic)
						pi.SetValue(value, null);
					else
						pi.SetValue(value, instance);
					return true;
				}
			}
			return false;
		}
		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			string name = binder.Name;
			MethodInfo method;
			if ((method = type.GetMethod(name, Reflection.all, null, args.Select(obj => obj.GetType()).ToArray(), null)) != null)
			{
				if (method.IsStatic)
					result = method.Invoke(null, args);
				else
					result = method.Invoke(instance, args);
				return true;
			}
			result = null;
			return false;
		}
	}
}
