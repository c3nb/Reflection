using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class Enumerable_
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> forEach)
        {
            foreach (var item in enumerable)
                forEach(item);
        }
    }
}
