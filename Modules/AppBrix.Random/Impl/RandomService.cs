// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Lifecycle;
using AppBrix.Random.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AppBrix.Random.Impl;

internal sealed class RandomService : IRandomService, IApplicationLifecycle
{
    #region IApplicationLifecycle implementation
    public void Initialize(IInitializeContext context)
    {
        this.randomGenerator = new ThreadLocal<System.Random>(() => new System.Random());
    }

    public void Uninitialize()
    {
        this.randomGenerator?.Dispose();
        this.randomGenerator = null!;
    }
    #endregion

    #region IRandomService implementation
    public System.Random GetRandom(int? seed = null) => seed.HasValue ? new System.Random(seed.Value) : this.randomGenerator.Value!;

    public IEnumerable<T> GetRandomItems<T>(IEnumerable<T> items, int? seed = null)
    {
        if (items is null)
            throw new ArgumentNullException(nameof(items));

        var array = items as T[] ?? items.ToArray();
        return array.Length == 0 ? array : this.GetInfiniteGenerator(array, this.GetRandom(seed));
    }

    public IEnumerable<T> GetUniqueItems<T>(IEnumerable<T> items, int? seed = null)
    {
        if (items is null)
            throw new ArgumentNullException(nameof(items));

        var array = items.ToArray();
        return array.Length == 0 ? array : this.GetUniqueGenerator(array, this.GetRandom(seed));
    }

    public void Shuffle<T>(IList<T> items, int? seed = null)
    {
        if (items is null)
            throw new ArgumentNullException(nameof(items));
        if (items.Count == 0)
            return;

        this.Shuffle(items, this.GetRandom(seed));
    }
    #endregion

    #region Private methods
    private IEnumerable<T> GetInfiniteGenerator<T>(T[] items, System.Random random)
    {
        while (true)
            yield return items[random.Next(items.Length)];
    }

    private IEnumerable<T> GetUniqueGenerator<T>(T[] items, System.Random random)
    {
        for (var i = items.Length - 1; i > 0; i--)
        {
            var n = random.Next(i + 1);
            yield return items[n];
            items[n] = items[i];
        }

        yield return items[0];
    }

    private void Shuffle<T>(IList<T> items, System.Random random)
    {
        for (var i = items.Count - 1; i > 0; i--)
        {
            var n = random.Next(i + 1);
            var temp = items[i];
            items[i] = items[n];
            items[n] = temp;
        }
    }
    #endregion

    #region Private fields and constants
    private ThreadLocal<System.Random> randomGenerator = null!;
    #endregion
}
