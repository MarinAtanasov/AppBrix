// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AppBrix.Random.Impl
{
    internal sealed class RandomService : IRandomService, IApplicationLifecycle
    {
        #region IApplicationLifecycle implementation
        public void Initialize(IInitializeContext context)
        {
            this.app = context.App;
            this.randomGenerator = new ThreadLocal<System.Random>(() => new System.Random());
        }

        public void Uninitialize()
        {
            this.app = null;
            this.randomGenerator.Dispose();
            this.randomGenerator = null;
        }
        #endregion

        #region IRandomService implementation
        public IEnumerable<T> GenerateRandomItems<T>(IReadOnlyCollection<T> items, bool unique = true)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (items.Count == 0)
                return Array.Empty<T>();

            return unique ? this.ShuffleInternal(items.ToList()) : this.InfiniteGenerator(items.ToList());
        }

        public System.Random GetRandom() => this.randomGenerator.Value;

        public void Shuffle<T>(IList<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (items.Count == 0)
                return;

            this.ShuffleInternal(items);
        }
        #endregion

        #region Private methods
        private IEnumerable<T> InfiniteGenerator<T>(List<T> items)
        {
            var random = this.GetRandom();
            while (true)
            {
                yield return items[random.Next(items.Count)];
            }
        }

        private IList<T> ShuffleInternal<T>(IList<T> items)
        {
            var random = this.GetRandom();
            for (var i = items.Count - 1; i > 0; i--)
            {
                var n = random.Next(i + 1);
                var temp = items[i];
                items[i] = items[n];
                items[n] = temp;
            }
            return items;
        }
        #endregion

        #region Private fields and constants
        #nullable disable
        private IApp app;
        private ThreadLocal<System.Random> randomGenerator;
        #nullable restore
        #endregion
    }
}
