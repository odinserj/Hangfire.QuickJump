// Copyright (c) 2016 Alexey Skalozub
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Hangfire.Dashboard.Extensions
{
    /// <summary>
    /// Dispatcher that combines output from several other dispatchers.
    /// Used internally by <see cref="RouteCollectionExtensions.Append"/>.
    /// </summary>
    internal class CompositeDispatcher : IDashboardDispatcher
    {
        private readonly List<IDashboardDispatcher> _dispatchers;

        public CompositeDispatcher(params IDashboardDispatcher[] dispatchers)
        {
            _dispatchers = new List<IDashboardDispatcher>(dispatchers);
        }

        public void AddDispatcher(IDashboardDispatcher dispatcher)
        {
            if (dispatcher == null)
                throw new ArgumentNullException(nameof(dispatcher));

            _dispatchers.Add(dispatcher);
        }

        public async Task Dispatch(DashboardContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (_dispatchers.Count == 0)
                throw new InvalidOperationException("CompositeDispatcher should contain at least one dispatcher");

            foreach (var dispatcher in _dispatchers)
            {
                await dispatcher.Dispatch(context);
            }
        }
    }
}