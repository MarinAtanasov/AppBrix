// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AppBrix.Random.Tests;

public sealed class RandomServiceTests : TestsBase<RandomModule>
{
    #region Setup and cleanup
    public RandomServiceTests() => this.App.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGenerateRandomItemsNullItems()
    {
        var service = this.App.GetRandomService();
        Action action = () => service.GetRandomItems<object>(null!);
        action.Should().Throw<ArgumentNullException>("items should not be null.");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGenerateRandomItemsEmptyItems()
    {
        var service = this.App.GetRandomService();
        var generated = service.GetRandomItems(Array.Empty<object>());
        generated.Should().NotBeNull($"{nameof(service.GetRandomItems)} should never return null.");
        generated.Should().BeSameAs([], $"{nameof(service.GetRandomItems)} should return an empty array.");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGenerateRandomItemsRepeated()
    {
        var service = this.App.GetRandomService();
        var original = Enumerable.Range(0, 10).ToList();
        var items = original.ToList();
        var generated = service.GetRandomItems(items, 42);

        items.Count.Should().Be(original.Count, "The collection size should not be modified.");
        Enumerable.Range(0, original.Count).All(x => items[x] == original[x]).Should().BeTrue("All items should be in their original position.");

        var visited = new int[original.Count];
        var moved = false;
        var repeated = false;
        var total = 0;
        foreach (var item in generated)
        {
            if (total < original.Count)
                moved |= item != original[total];
            repeated |= visited[item] > 1;
            visited[item]++;
            total++;
            if (moved && repeated && total > original.Count || total > original.Count * 2)
                break;
        }
        moved.Should().BeTrue("Some items should have been shuffled.");
        repeated.Should().BeTrue("Some items should be seen more than once.");
        total.Should().BeGreaterThan(original.Count, "Item generation should continue after exhausting the collection.");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGenerateRandomItemsDifferentSeeds()
    {
        var service = this.App.GetRandomService();
        var original = Enumerable.Range(0, 10).ToList();
        var items1 = service.GetRandomItems(original, 23).Take(original.Count).ToList();
        var items2 = service.GetRandomItems(original, 42).Take(original.Count).ToList();
        Enumerable.Range(0, items1.Count).Any(x => items1[x] != items2[x]).Should().BeTrue("Generated items should be different.");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGenerateRandomItemsEqualSeeds()
    {
        var service = this.App.GetRandomService();
        var original = Enumerable.Range(0, 10).ToList();
        var items1 = service.GetRandomItems(original, 42).Take(original.Count).ToList();
        var items2 = service.GetRandomItems(original, 42).Take(original.Count).ToList();
        Enumerable.Range(0, items1.Count).All(x => items1[x] == items2[x]).Should().BeTrue("Generated items should be the same.");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGenerateUniqueItemsNullItems()
    {
        var service = this.App.GetRandomService();
        Action action = () => service.GetUniqueItems<object>(null!);
        action.Should().Throw<ArgumentNullException>("items should not be null.");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGenerateUniqueItemsEmptyItems()
    {
        var service = this.App.GetRandomService();
        var generated = service.GetUniqueItems(Array.Empty<object>());
        generated.Should().NotBeNull($"{nameof(service.GetUniqueItems)} should never return null.");
        generated.Should().BeSameAs([], $"{nameof(service.GetUniqueItems)} should return an empty array.");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGenerateUniqueItems()
    {
        var service = this.App.GetRandomService();
        var original = Enumerable.Range(0, 10).ToList();
        var items = original.ToList();
        var generated = service.GetUniqueItems(items, 42).ToList();

        items.Count.Should().Be(original.Count, "The collection size should not be modified.");
        Enumerable.Range(0, original.Count).All(x => items[x] == original[x]).Should().BeTrue("All items should be in their original position.");

        generated.Count.Should().Be(original.Count, "Maximum unique generated items should be the same as original count.");
        Enumerable.Range(0, original.Count).Any(x => generated[x] != original[x]).Should().BeTrue("Some items should have been shuffled.");

        generated.Sort();
        items.Sort();
        Enumerable.Range(0, original.Count).All(x => generated[x] == items[x]).Should().BeTrue("Generated collection should contain the original items.");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGenerateUniqueItemsDifferentSeeds()
    {
        var service = this.App.GetRandomService();
        var original = Enumerable.Range(0, 10).ToList();
        var items1 = service.GetUniqueItems(original, 23).ToList();
        var items2 = service.GetUniqueItems(original, 42).ToList();
        Enumerable.Range(0, items1.Count).Any(x => items1[x] != items2[x]).Should().BeTrue("Generated items should be different.");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGenerateUniqueItemsEqualSeeds()
    {
        var service = this.App.GetRandomService();
        var original = Enumerable.Range(0, 10).ToList();
        var items1 = service.GetUniqueItems(original, 42).ToList();
        var items2 = service.GetUniqueItems(original, 42).ToList();
        Enumerable.Range(0, items1.Count).All(x => items1[x] == items2[x]).Should().BeTrue("Generated items should be the same.");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestMultiThreadRandom()
    {
        var service = this.App.GetRandomService();
        var lists = new List<List<int>>(Enumerable.Range(0, 32).Select(_ => new List<int>()));
        Enumerable.Range(0, lists.Count)
            .Select(x => lists[x])
            .AsParallel()
            .ForAll(x =>
            {
                var random = service.GetRandom();
                for (var i = 0; i < 1000; i++)
                {
                    x.Add(random.Next(0, int.MaxValue / 2));
                }
            });

        foreach (var list in lists)
        {
            var last = list[^1];
            var ok = false;
            for (var j = 1; j <= 10; j++)
            {
                if (list[list.Count - 1 - j] != last)
                {
                    ok = true;
                    break;
                }
            }
            ok.Should().BeTrue($"{nameof(System.Random)} generation should be thread safe.");
        }
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestShuffleNullItems()
    {
        var service = this.App.GetRandomService();
        var action = () => service.Shuffle<object>(null!);
        action.Should().Throw<ArgumentNullException>("items should not be null.");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestShuffleEmptyItems() => this.App.GetRandomService().Shuffle(Array.Empty<object>());

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestShuffle()
    {
        var service = this.App.GetRandomService();
        var items = Enumerable.Range(0, 10).ToList();
        service.Shuffle(items, 42);
        Enumerable.Range(0, items.Count).Any(x => items[x] != x).Should().BeTrue("Some items should have been shuffled.");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestShuffleDifferentSeeds()
    {
        var service = this.App.GetRandomService();
        var items1 = Enumerable.Range(0, 10).ToList();
        var items2 = Enumerable.Range(0, 10).ToList();
        service.Shuffle(items1, 23);
        service.Shuffle(items2, 42);
        Enumerable.Range(0, items1.Count).Any(x => items1[x] != items2[x]).Should().BeTrue("Shuffles should be different.");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestShuffleEqualSeeds()
    {
        var service = this.App.GetRandomService();
        var items1 = Enumerable.Range(0, 10).ToList();
        var items2 = Enumerable.Range(0, 10).ToList();
        service.Shuffle(items1, 42);
        service.Shuffle(items2, 42);
        Enumerable.Range(0, items1.Count).All(x => items1[x] == items2[x]).Should().BeTrue("Both shuffles should be the same.");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetRandom() => this.AssertPerformance(this.TestPerformanceGetRandomInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetRandomItems() => this.AssertPerformance(this.TestPerformanceGetRandomItemsInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetUniqueItems() => this.AssertPerformance(this.TestPerformanceGetUniqueItemsInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceShuffle() => this.AssertPerformance(this.TestPerformanceShuffleInternal);
    #endregion

    #region Private methods
    private void TestPerformanceGetRandomInternal()
    {
        var service = this.App.GetRandomService();
        for (var i = 0; i < 1000000; i++)
        {
            service.GetRandom();
        }
    }

    private void TestPerformanceGetRandomItemsInternal()
    {
        var service = this.App.GetRandomService();
        var items = Enumerable.Range(0, 100).ToList();
        var sum = 0;
        for (var i = 0; i < 30000; i++)
        {
            sum += service.GetRandomItems(items).Take(10).Sum();
        }
        sum.Should().BePositive();
    }

    private void TestPerformanceGetUniqueItemsInternal()
    {
        var service = this.App.GetRandomService();
        var items = Enumerable.Range(0, 100).ToList();
        var sum = 0;
        for (var i = 0; i < 30000; i++)
        {
            sum += service.GetUniqueItems(items).Take(10).Sum();
        }
        sum.Should().BePositive();
    }

    private void TestPerformanceShuffleInternal()
    {
        var service = this.App.GetRandomService();
        var items = Enumerable.Range(0, 100).ToList();
        for (var i = 0; i < 10000; i++)
        {
            service.Shuffle(items);
        }
    }
    #endregion
}
