// Copyright (c) 2016 Alexey Skalozub
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Hangfire.Dashboard.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="RouteCollection"/>.
    /// </summary>
    internal static class RouteCollectionExtensions
    {
        // ReSharper disable once InconsistentNaming
        private static readonly FieldInfo _dispatchers = typeof(RouteCollection).GetTypeInfo().GetDeclaredField(nameof(_dispatchers));

        /// <summary>
        /// Returns a private list of registered routes.
        /// </summary>
        /// <param name="routes">Route collection</param>
        private static List<Tuple<string, IDashboardDispatcher>> GetDispatchers(this RouteCollection routes)
        {
            if (routes == null)
                throw new ArgumentNullException(nameof(routes));

            return (List<Tuple<string, IDashboardDispatcher>>)_dispatchers.GetValue(routes);
        }

        /// <summary>
        /// Checks if there's a dispatcher registered for given <paramref name="pathTemplate"/>.
        /// </summary>
        /// <param name="routes">Route collection</param>
        /// <param name="pathTemplate">Path template</param>
        public static bool Contains(this RouteCollection routes, string pathTemplate)
        {
            if (routes == null)
                throw new ArgumentNullException(nameof(routes));
            if (pathTemplate == null)
                throw new ArgumentNullException(nameof(pathTemplate));

            return routes.GetDispatchers().Any(x => x.Item1 == pathTemplate);
        }

        /// <summary>
        /// Combines exising dispatcher for <paramref name="pathTemplate"/> with <paramref name="dispatcher"/>.
        /// If there's no dispatcher for the specified path, adds a new one.
        /// </summary>
        /// <param name="routes">Route collection</param>
        /// <param name="pathTemplate">Path template</param>
        /// <param name="dispatcher">Dispatcher to add or append for specified path</param>
        public static void Append(this RouteCollection routes, string pathTemplate, IDashboardDispatcher dispatcher)
        {
            if (routes == null)
                throw new ArgumentNullException(nameof(routes));
            if (pathTemplate == null)
                throw new ArgumentNullException(nameof(pathTemplate));
            if (dispatcher == null)
                throw new ArgumentNullException(nameof(dispatcher));

            var list = routes.GetDispatchers();

            for (var i = 0; i < list.Count; i++)
            {
                var pair = list[i];
                if (pair.Item1 == pathTemplate)
                {
                    if (!(pair.Item2 is CompositeDispatcher composite))
                    {
                        // replace original dispatcher with a composite one
                        composite = new CompositeDispatcher(pair.Item2);
                        list[i] = new Tuple<string, IDashboardDispatcher>(pair.Item1, composite);
                    }

                    composite.AddDispatcher(dispatcher);
                    return;
                }
            }

            routes.Add(pathTemplate, dispatcher);
        }

        /// <summary>
        /// Replaces exising dispatcher for <paramref name="pathTemplate"/> with <paramref name="dispatcher"/>.
        /// If there's no dispatcher for the specified path, adds a new one.
        /// </summary>
        /// <param name="routes">Route collection</param>
        /// <param name="pathTemplate">Path template</param>
        /// <param name="dispatcher">Dispatcher to set for specified path</param>
        public static void Replace(this RouteCollection routes, string pathTemplate, IDashboardDispatcher dispatcher)
        {
            if (routes == null)
                throw new ArgumentNullException(nameof(routes));
            if (pathTemplate == null)
                throw new ArgumentNullException(nameof(pathTemplate));
            if (dispatcher == null)
                throw new ArgumentNullException(nameof(dispatcher));

            var list = routes.GetDispatchers();

            for (var i = 0; i < list.Count; i++)
            {
                var pair = list[i];
                if (pair.Item1 == pathTemplate)
                {
                    list[i] = new Tuple<string, IDashboardDispatcher>(pair.Item1, dispatcher);
                    return;
                }
            }

            routes.Add(pathTemplate, dispatcher);
        }

        /// <summary>
        /// Removes dispatcher for <paramref name="pathTemplate"/>.
        /// </summary>
        /// <param name="routes">Route collection</param>
        /// <param name="pathTemplate">Path template</param>
        public static void Remove(this RouteCollection routes, string pathTemplate)
        {
            if (routes == null)
                throw new ArgumentNullException(nameof(routes));
            if (pathTemplate == null)
                throw new ArgumentNullException(nameof(pathTemplate));

            var list = routes.GetDispatchers();

            for (var i = 0; i < list.Count; i++)
            {
                var pair = list[i];
                if (pair.Item1 == pathTemplate)
                {
                    list.RemoveAt(i);
                    return;
                }
            }
        }
    }
}