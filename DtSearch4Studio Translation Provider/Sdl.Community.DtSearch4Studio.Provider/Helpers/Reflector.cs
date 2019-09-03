using System;
using System.Reflection;

namespace Sdl.Community.DtSearch4Studio.Provider.Helpers
{
	internal class Reflector
	{
		private string m_ns;

		private Assembly m_asmb;

		public Reflector(string ns) : this(ns, ns)
		{
		}

		public Reflector(string an, string ns)
		{
			m_ns = ns;
			m_asmb = null;
			var referencedAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
			for (int i = 0; i < (int)referencedAssemblies.Length; i++)
			{
				var assemblyName = referencedAssemblies[i];
				if (assemblyName.FullName.StartsWith(an))
				{
					m_asmb = Assembly.Load(assemblyName);
					return;
				}
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
			return method.Invoke(obj, parameters);
		}

		public object Get(object obj, string prop)
		{
			return GetAs(obj.GetType(), obj, prop);
		}

		public object GetAs(Type type, object obj, string prop)
		{
			var property = type.GetProperty(prop, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return property.GetValue(obj, null);
		}

		public object GetEnum(string typeName, string name)
		{
			var type = this.GetType(typeName);
			var field = type.GetField(name);
			return field.GetValue(null);
		}

		public Type GetType(string typeName)
		{
			Type type = null;
			var chrArray = new char[] { '.' };
			var strArrays = typeName.Split(chrArray);
			if ((int)strArrays.Length > 0)
			{
				type = m_asmb.GetType(string.Concat(this.m_ns, ".", strArrays[0]));
			}
			for (int i = 1; i < (int)strArrays.Length; i++)
			{
				type = type.GetNestedType(strArrays[i], BindingFlags.NonPublic);
			}
			return type;
		}

		public object New(string name, params object[] parameters)
		{
			object obj = null;
			var type = this.GetType(name);
			var constructors = type.GetConstructors();
			var constructorInfoArray = constructors;
			int num = 0;
		Label1:
			while (num < (int)constructorInfoArray.Length)
			{
				ConstructorInfo constructorInfo = constructorInfoArray[num];
				try
				{
					obj = constructorInfo.Invoke(parameters);
				}
				catch
				{
					object obj1 = obj;
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