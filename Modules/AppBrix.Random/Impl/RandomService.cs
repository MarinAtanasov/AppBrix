// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Random;
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
        public IEnumerable<T> GenerateRandomItems<T>(IList<T> items, bool unique = true)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (items.Count == 0)
                return Array.Empty<T>();

            items = items.ToList();
            if (unique)
            {
                this.Shuffle(items);
                return items;
            }
            else
            {
                return this.InfiniteGenerator(items);
            }
        }

        public System.Random GetRandom() => this.randomGenerator.Value;

        public void Shuffle<T>(IList<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (items.Count == 0)
                return;

            var random = this.GetRandom();
            for (var i = items.Count - 1; i > 0; i--)
            {
                var n = random.Next(i + 1);
                var temp = items[i];
                items[i] = items[n];
                items[n] = temp;
            }
        }
        #endregion

        #region Private methods
        private IEnumerable<T> InfiniteGenerator<T>(IList<T> items)
        {
            var random = this.GetRandom();
            while (true)
            {
                yield return items[random.Next(items.Count)];
            }
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
