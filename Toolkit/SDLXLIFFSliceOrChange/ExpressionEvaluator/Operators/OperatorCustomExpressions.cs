using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionEvaluator.Operators
{
    internal class OperatorCustomExpressions
    {
        /// <summary>
        /// Returns an Expression that accesses a member on an Expression
        /// </summary>
        /// <param name="le">The expression that contains the member to be accessed</param>
        /// <param name="membername">The name of the member to access</param>
        /// <param name="args">Optional list of arguments to be passed if the member is a method</param>
        /// <returns></returns>
        public static Expression MemberAccess(Expression le, string membername, List<Expression> args)
        {
            List<Type> argTypes = new List<Type>();
            args.ForEach(x => argTypes.Add(x.Type));

            Expression instance = null;
            Type type = null;
            if (le.Type.Name == "RuntimeType")
            {
                type = ((Type)((ConstantExpression)le).Value);
            }
            else
            {
                type = le.Type;
                instance = le;
            }

            MethodInfo mi = type.GetMethod(membername, argTypes.ToArray());
            if (mi != null)
            {
                ParameterInfo[] pi = mi.GetParameters();
                for (int i = 0; i < pi.Length; i++)
                {
                    args[i] = TypeConversion.Convert(args[i], pi[i].ParameterType);
                }
                return Expression.Call(instance, mi, args);
            }
            else
            {
                Expression exp = null;

                PropertyInfo pi = type.GetProperty(membername);
                if (pi != null)
                {
                    exp = Expression.Property(instance, pi);
                }
                else
                {
                    FieldInfo fi = type.GetField(membername);
                    if (fi != null)
                    {
                        exp = Expression.Field(instance, fi);
                    }
                }

                if (exp != null)
                {
                    if (args.Count > 0)
                    {
                        return Expression.ArrayAccess(exp, args);
                    }
                    else
                    {
                        return exp;
                    }
                }
                else
                {
                    throw new Exception(string.Format("Member not found: {0}.{1}", le.Type.Name, membername));
                }
            }
        }

        /// <summary>
        /// Extends the Add Expression handler to handle string concatenation
        /// </summary>
        /// <param name="le"></param>
        /// <param name="re"></param>
        /// <returns></returns>
        public static Expression Add(Expression le, Expression re)
        {
            if (le.Type == typeof(string) && re.Type == typeof(string))
            {
                return Expression.Add(le, re, typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) }));
            }
            else
            {
                return Expression.Add(le, re);
            }
        }

        /// <summary>
        /// Returns an Expression that access a 1-dimensional index on an Array expression 
        /// </summary>
        /// <param name="le"></param>
        /// <param name="re"></param>
        /// <returns></returns>
        public static Expression ArrayAccess(Expression le, Expression re)
        {
            if (le.Type == typeof(string))
            {
                MethodInfo mi = typeof(string).GetMethod("ToCharArray", new Type[] { });
                le = Expression.Call(le, mi);
            }

            return Expression.ArrayAccess(le, re);
        }

    }
}