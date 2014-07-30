using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageCompiler.Utils
{
    public class TraverseUtil
    {
        /// <summary>
        /// Traverse a tree using DFS
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="childSelector"></param>
        /// <returns></returns>
        public static IEnumerable<T> TraverseDfs<T>(T item, Func<T, IEnumerable> childSelector)
        {
            var stack = new Stack<T>();
            stack.Push(item);
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                foreach (var child in childSelector(next))
                    stack.Push((T)child);
            }
        }

        /// <summary>
        /// Traverse a tree using BFS
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="childSelector"></param>
        /// <returns></returns>
        public static IEnumerable<T> TraverseBfs<T>(T item, Func<T, IEnumerable> childSelector)
        {
            var stack = new Queue<T>();
            stack.Enqueue(item);
            while (stack.Any())
            {
                var next = stack.Dequeue();
                yield return next;

                foreach (var child in childSelector(next))
                    stack.Enqueue((T)child);
            }
        }
    }
}
