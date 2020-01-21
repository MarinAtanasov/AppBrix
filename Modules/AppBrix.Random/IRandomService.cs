// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

using System.Collections.Generic;

namespace AppBrix.Random
{
    /// <summary>
    /// Service that enables generation of randomised objects and values.
    /// </summary>
    public interface IRandomService
    {
        /// <summary>
        /// Creates a generator for creating random items from the provided collection.
        /// If unique=true, generator will end after going through the entire collection once.
        /// If unique=false, the generator will be infinite.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="items">The collection which contains the items.</param>
        /// <param name="unique">If true, each element will be yielded only once.</param>
        /// <returns>The random items generator.</returns>
        public IEnumerable<T> GenerateRandomItems<T>(IList<T> items, bool unique = true);

        /// <summary>
        /// Gets an instance of <see cref="System.Random"/>.
        /// </summary>
        /// <returns>An instance of <see cref="System.Random"/>.</returns>
        public System.Random GetRandom();

        /// <summary>
        /// Shuffles the items in the provided collection.
        /// </summary>
        /// <typeparam name="T">The type of the items within the collection.</typeparam>
        /// <param name="items">The collection with items to be shuffled.</param>
        public void Shuffle<T>(IList<T> items);
    }
}
