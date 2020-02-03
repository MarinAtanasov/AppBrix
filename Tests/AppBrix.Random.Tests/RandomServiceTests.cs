// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AppBrix.Random.Tests
{
    public sealed class RandomServiceTests : TestsBase
    {
        #region Setup and cleanup
        public RandomServiceTests() : base(TestUtils.CreateTestApp<RandomModule>()) => this.app.Start();
        #endregion

        #region Tests
        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestGenerateRandomItemsNullItems()
        {
            var service = this.app.GetRandomService();
            Action action = () => service.GenerateRandomItems<object>(null);
            action.Should().Throw<ArgumentNullException>("items should not be null.");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestGenerateRandomItemsEmptyItems()
        {
            var service = this.app.GetRandomService();
            var generated = service.GenerateRandomItems(Array.Empty<object>());
            generated.Should().NotBeNull($"{nameof(service.GenerateRandomItems)} should never return null.");
            generated.Should().BeSameAs(Array.Empty<object>(), $"{nameof(service.GenerateRandomItems)} should return an empty array.");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestGenerateRandomItemsUnique()
        {
            var service = this.app.GetRandomService();
            var original = Enumerable.Range(0, 1000).ToList();
            var items = original.ToList();
            var generated = service.GenerateRandomItems(items).ToList();

            items.Count.Should().Be(original.Count, "The collection size should not be modified.");
            Enumerable.Range(0, original.Count).All(x => items[x] == original[x]).Should().BeTrue("All items should be in their original position.");

            generated.Count.Should().Be(original.Count, "Maximum unique generated items should be the same as original count.");
            Enumerable.Range(0, original.Count).Any(x => generated[x] != original[x]).Should().BeTrue("Some items should have been shuffled.");

            generated.Sort();
            items.Sort();
            Enumerable.Range(0, original.Count).All(x => generated[x] == items[x]).Should().BeTrue("Generated collection should contain the original items.");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestGenerateRandomItemsRepeated()
        {
            var service = this.app.GetRandomService();
            var original = Enumerable.Range(0, 1000).ToList();
            var items = original.ToList();
            var generated = service.GenerateRandomItems(items, unique: false);

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
        public void TestMultiThreadRandom()
        {
            var service = this.app.GetRandomService();
            var lists = new List<List<int>>(Enumerable.Range(0, 32).Select(x => new List<int>()));
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
            for (var i = 0; i < lists.Count; i++)
            {
                var list = lists[i];
                var last = list[list.Count - 1];
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
            var service = this.app.GetRandomService();
            Action action = () => service.Shuffle<object>(null);
            action.Should().Throw<ArgumentNullException>("items should not be null.");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestShuffleEmptyItems() => this.app.GetRandomService().Shuffle(Array.Empty<object>());

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestShuffle()
        {
            var service = this.app.GetRandomService();
            var items = Enumerable.Range(0, 1000).ToList();
            service.Shuffle(items);
            Enumerable.Range(0, items.Count).Any(x => items[x] != x).Should().BeTrue("Some items should have been shuffled.");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceGetRandom() => TestUtils.TestPerformance(this.TestPerformanceGetRandomInternal);

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceShuffle() => TestUtils.TestPerformance(this.TestPerformanceShuffleInternal);
        #endregion

        #region Private methods
        private void TestPerformanceGetRandomInternal()
        {
            var service = this.app.GetRandomService();
            for (var i = 0; i < 800000; i++)
            {
                service.GetRandom();
            }
        }

        private void TestPerformanceShuffleInternal()
        {
            var service = this.app.GetRandomService();
            var items = Enumerable.Range(0, 100).ToList();
            for (var i = 0; i < 8000; i++)
            {
                service.Shuffle(items);
            }
        }
        #endregion
    }
}
