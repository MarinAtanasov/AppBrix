// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Random.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AppBrix.Random.Impl;

internal sealed class RandomService : IRandomService
{
    #region IRandomService implementation
    public System.Random GetRandom(int? seed = null) => seed.HasValue ? new System.Random(seed.Value) : System.Random.Shared;

    public IEnumerable<T> GetRandomItems<T>(IEnumerable<T> items, int? seed = null)
    {
        if (items is null)
            throw new ArgumentNullException(nameof(items));

        var array = items as T[] ?? items.ToArray();
        return array.Length == 0 ? Array.Empty<T>() : this.GetInfiniteGenerator(array, this.GetRandom(seed));
    }

    public IEnumerable<T> GetUniqueItems<T>(IEnumerable<T> items, int? seed = null)
    {
        if (items is null)
            throw new ArgumentNullException(nameof(items));

        var array = items.ToArray();
        return array.Length == 0 ? Array.Empty<T>() : this.GetUniqueGenerator(array, this.GetRandom(seed));
    }

    public void Shuffle<T>(IList<T> items, int? seed = null)
    {
        if (items is null)
            throw new ArgumentNullException(nameof(items));
        if (items.Count == 0)
            return;

        var random = this.GetRandom(seed);
        if (items is T[] array)
            random.Shuffle(array);
        else if (items is List<T> list)
            random.Shuffle(CollectionsMarshal.AsSpan(list));
        else
            this.Shuffle(items, random);
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
        var count = items.Count;
        var last = count - 1;
        for (var i = 0; i < last; i++)
        {
            var n = random.Next(i, count);
            if (i != n)
            {
                var temp = items[i];
                items[i] = items[n];
                items[n] = temp;
            }
        }
    }
    #endregion
}
