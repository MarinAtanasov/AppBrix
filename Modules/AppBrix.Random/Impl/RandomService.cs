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
            this.randomGenerator = new ThreadLocal<System.Random>(() => new System.Random());
        }

        public void Uninitialize()
        {
            this.randomGenerator.Dispose();
            this.randomGenerator = null;
        }
        #endregion

        #region IRandomService implementation
        public System.Random GetRandom(int? seed = null) => seed.HasValue ? new System.Random(seed.Value) : this.randomGenerator.Value;

        public IEnumerable<T> GetRandomItems<T>(IReadOnlyCollection<T> items, int? seed = null)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));
            if (items.Count == 0)
                return Array.Empty<T>();

            return this.InfiniteGenerator(items.ToArray(), this.GetRandom(seed));
        }

        public IEnumerable<T> GetUniqueItems<T>(IReadOnlyCollection<T> items, int? seed = null)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));
            if (items.Count == 0)
                return Array.Empty<T>();

            return this.ShuffleInternal(items.ToArray(), this.GetRandom(seed));
        }

        public void Shuffle<T>(IList<T> items, int? seed = null)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));
            if (items.Count == 0)
                return;

            this.ShuffleInternal(items, this.GetRandom(seed));
        }
        #endregion

        #region Private methods
        private IEnumerable<T> InfiniteGenerator<T>(T[] items, System.Random random)
        {
            while (true)
                yield return items[random.Next(items.Length)];
        }

        private IList<T> ShuffleInternal<T>(IList<T> items, System.Random random)
        {
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
        private ThreadLocal<System.Random> randomGenerator;
        #nullable restore
        #endregion
    }
}
