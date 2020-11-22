using System.Collections.Generic;
using System;

namespace ShoddyChess
{
    public static class ListExtension 
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var element in source)
            {
                action(element);
            }
        }
    }
}