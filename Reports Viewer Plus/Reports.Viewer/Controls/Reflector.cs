using System;
using System.Reflection;

namespace Reports.Viewer.Plus.Controls
{
	public class Reflector
	{

		private readonly string _mNs;
		private readonly Assembly _mAsmb;

		public Reflector(string ns) : this(ns, ns)
		{
		}

		public Reflector(string an, string ns)
		{
			_mNs = ns;
			_mAsmb = null;
			var referencedAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
			foreach (var assemblyName in referencedAssemblies)
			{
				if (!assemblyName.FullName.StartsWith(an)) continue;
				_mAsmb = Assembly.Load(assemblyName);
				return;
			}
		}

		public object Call(object obj, string func, params object[] parameters)
		{
			return Call2(obj, func, parameters);
		}

		public object Call2(object obj, string func, object[] parameters)
		{
			return CallAs2(obj.GetType(), obj, func, parameters);
		}

		public object CallAs(Type type, object obj, string func, params object[] parameters)
		{
			return CallAs2(type, obj, func, parameters);
		}

		public object CallAs2(Type type, object obj, string func, object[] parameters)
		{
			var method = type.GetMethod(func, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return method?.Invoke(obj, parameters);
		}

		public object Get(object obj, string prop)
		{
			return GetAs(obj.GetType(), obj, prop);
		}

		public object GetAs(Type type, object obj, string prop)
		{
			var property = type.GetProperty(prop, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return property?.GetValue(obj, null);
		}

		public object GetEnum(string typeName, string name)
		{
			var type = GetType(typeName);
			var field = type.GetField(name);
			return field.GetValue(null);
		}

		public Type GetType(string typeName)
		{
			Type type = null;
			char[] chrArray = { '.' };
			var strArrays = typeName.Split(chrArray);
			if (strArrays.Length > 0)
			{
				type = _mAsmb.GetType(string.Concat(_mNs, ".", strArrays[0]));
			}
			for (var i = 1; i < strArrays.Length; i++)
			{
				type = type.GetNestedType(strArrays[i], BindingFlags.NonPublic);
			}
			return type;
		}

		public object New(string name, params object[] parameters)
		{
			object obj = null;
			var type = GetType(name);
			var constructors = type.GetConstructors();
			var constructorInfoArray = constructors;
			var num = 0;
		Label1:
			while (num < constructorInfoArray.Length)
			{
				var constructorInfo = constructorInfoArray[num];
				try
				{
					obj = constructorInfo.Invoke(parameters);
				}
				catch
				{
					var obj1 = obj;
					goto Label0;
				}
				return obj;
			}
			return null;
		Label0:
			num++;
			goto Label1;
		}
	}
}
