using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sdl.Community.SDLXLIFFSliceOrChange.ExpressionEvaluator
{
    class TypeConversion
    {
        readonly Dictionary<Type, int> _typePrecedence = null;
        static readonly TypeConversion Instance = new TypeConversion();
        /// <summary>
        /// Performs implicit conversion between two expressions depending on their type precedence
        /// </summary>
        /// <param name="le"></param>
        /// <param name="re"></param>
        internal static void Convert(ref Expression le, ref Expression re)
        {
            if (Instance._typePrecedence.ContainsKey(le.Type) && Instance._typePrecedence.ContainsKey(re.Type))
            {
                if (Instance._typePrecedence[le.Type] > Instance._typePrecedence[re.Type]) re = Expression.Convert(re, le.Type);
                if (Instance._typePrecedence[le.Type] < Instance._typePrecedence[re.Type]) le = Expression.Convert(le, re.Type);
            }
        }

        /// <summary>
        /// Performs implicit conversion on an expression against a specified type
        /// </summary>
        /// <param name="le"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static Expression Convert(Expression le, Type type)
        {
            if (Instance._typePrecedence.ContainsKey(le.Type) && Instance._typePrecedence.ContainsKey(type))
            {
                if (Instance._typePrecedence[le.Type] < Instance._typePrecedence[type]) return Expression.Convert(le, type);
            }
            return le;
        }

        TypeConversion()
        {
            _typePrecedence = new Dictionary<Type, int>
                {
                    {typeof (byte), 0},
                    {typeof (int), 1},
                    {typeof (short), 2},
                    {typeof (long), 3},
                    {typeof (float), 4},
                    {typeof (double), 5}
                };
        }
    }
}