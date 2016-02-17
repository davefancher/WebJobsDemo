using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace System
{
    [DebuggerNonUserCode]
    public static class FunctionalExtensions
    {
        public static TResult Map<TSource, TResult>(
            this TSource @this,
            Func<TSource, TResult> fn) => fn(@this);

        public static T Tee<T>(
            this T @this,
            Action<T> act)
        {
            act(@this);
            return @this;
        }
    }
}

namespace System.Collections.Generic
{
    [DebuggerNonUserCode]
    public static class IEnumerableExtensions
    {
        public static void Iter<T>(this IEnumerable<T> @this, Action<T> act)
        {
            var enumerator = @this.GetEnumerator();

            while(enumerator.MoveNext())
            {
                act(enumerator.Current);
            }
        }
    }
}
