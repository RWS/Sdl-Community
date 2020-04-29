using System;
using System.Reflection;

namespace Sdl.Community.InSource.Helpers
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
            this.m_ns = ns;
            this.m_asmb = null;
            AssemblyName[] referencedAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            for (int i = 0; i < (int)referencedAssemblies.Length; i++)
            {
                AssemblyName assemblyName = referencedAssemblies[i];
                if (assemblyName.FullName.StartsWith(an))
                {
                    this.m_asmb = Assembly.Load(assemblyName);
                    return;
                }
            }
        }

        public object Call(object obj, string func, params object[] parameters)
        {
            return this.Call2(obj, func, parameters);
        }

        public object Call2(object obj, string func, object[] parameters)
        {
            return this.CallAs2(obj.GetType(), obj, func, parameters);
        }

        public object CallAs(Type type, object obj, string func, params object[] parameters)
        {
            return this.CallAs2(type, obj, func, parameters);
        }

        public object CallAs2(Type type, object obj, string func, object[] parameters)
        {
            MethodInfo method = type.GetMethod(func, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return method.Invoke(obj, parameters);
        }

        public object Get(object obj, string prop)
        {
            return this.GetAs(obj.GetType(), obj, prop);
        }

        public object GetAs(Type type, object obj, string prop)
        {
            PropertyInfo property = type.GetProperty(prop, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return property.GetValue(obj, null);
        }

        public object GetEnum(string typeName, string name)
        {
            Type type = this.GetType(typeName);
            FieldInfo field = type.GetField(name);
            return field.GetValue(null);
        }

        public Type GetType(string typeName)
        {
            Type type = null;
            char[] chrArray = new char[] { '.' };
            string[] strArrays = typeName.Split(chrArray);
            if ((int)strArrays.Length > 0)
            {
                type = this.m_asmb.GetType(string.Concat(this.m_ns, ".", strArrays[0]));
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
            Type type = this.GetType(name);
            ConstructorInfo[] constructors = type.GetConstructors();
            ConstructorInfo[] constructorInfoArray = constructors;
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
