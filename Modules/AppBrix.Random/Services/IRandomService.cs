// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System.Collections.Generic;

namespace AppBrix.Random.Services;

/// <summary>
/// Service that enables generation of randomised objects and values.
/// </summary>
public interface IRandomService
{
	/// <summary>
	/// Gets an instance of <see cref="System.Random"/>.
	/// </summary>
	/// <param name="seed">A number used to calculate a starting value for the pseudo-random number sequence.</param>
	/// <returns>An instance of <see cref="System.Random"/>.</returns>
	System.Random GetRandom(int? seed = null);

	/// <summary>
	/// Creates a generator for getting random items from the provided enumerable.
	/// The generator is infinite and can return the same item multiple times.
	/// </summary>
	/// <typeparam name="T">The type of the items.</typeparam>
	/// <param name="items">The enumerable which contains the items.</param>
	/// <param name="seed">A number used to calculate a starting value for the pseudo-random number sequence.</param>
	/// <returns>The random items generator.</returns>
	IEnumerable<T> GetRandomItems<T>(IEnumerable<T> items, int? seed = null);

	/// <summary>
	/// Creates a generator for getting random unique items from the provided enumerable.
	/// The generator will end after going through the entire collection.
	/// </summary>
	/// <typeparam name="T">The type of the items.</typeparam>
	/// <param name="items">The enumerable which contains the items.</param>
	/// <param name="seed">A number used to calculate a starting value for the pseudo-random number sequence.</param>
	/// <returns>The unique items generator.</returns>
	IEnumerable<T> GetUniqueItems<T>(IEnumerable<T> items, int? seed = null);

	/// <summary>
	/// Shuffles the items in the provided collection.
	/// </summary>
	/// <typeparam name="T">The type of the items within the collection.</typeparam>
	/// <param name="items">The collection with items to be shuffled.</param>
	/// <param name="seed">A number used to calculate a starting value for the pseudo-random number sequence.</param>
	void Shuffle<T>(IList<T> items, int? seed = null);
}
