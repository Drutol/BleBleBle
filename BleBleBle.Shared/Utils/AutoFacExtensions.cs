using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;

namespace BleBleBle.Shared.Utils
{
    public static class AutoFacExtensions
    {
        public static TReturn TypedResolve<TReturn, TParameter>(this ILifetimeScope scope, TParameter parameter)
        {
            return scope.Resolve<TReturn>(new TypedParameter(typeof(TParameter), parameter));
        }

        public static TReturn TypedResolve<TReturn>(this ILifetimeScope scope, params object[] parameter)
        {
            return scope.Resolve<TReturn>(parameter.Select(o => new TypedParameter(o.GetType(), o)));
        }
    }

}
