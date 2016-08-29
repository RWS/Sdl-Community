using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sdl.Community.SDLXLIFFSliceOrChange.ExpressionEvaluator.Operators
{
    internal class OpFuncServiceLocator
    {
        static readonly OpFuncServiceLocator Instance = new OpFuncServiceLocator();

        OpFuncServiceLocator()
        {
            _typeActions.Add(typeof(MethodOperator), OpFuncServiceProviders.MethodOperatorFunc);
            _typeActions.Add(typeof(TypeOperator), OpFuncServiceProviders.TypeOperatorFunc);
            _typeActions.Add(typeof(UnaryOperator), OpFuncServiceProviders.UnaryOperatorFunc);
            _typeActions.Add(typeof(BinaryOperator), OpFuncServiceProviders.BinaryOperatorFunc);
        }

        public static Func<OpFuncArgs, Expression> Resolve(Type type)
        {
            return Instance.ResolveType(type);
        }

        Func<OpFuncArgs, Expression> ResolveType(Type type)
        {
            return _typeActions[type];
        }

        readonly Dictionary<Type, Func<OpFuncArgs, Expression>>
            _typeActions = new Dictionary<Type, Func<OpFuncArgs, Expression>>();
    }
}