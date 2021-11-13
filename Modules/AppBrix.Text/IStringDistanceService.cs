// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

namespace AppBrix.Text
{
    /// <summary>
    /// Service for getting the distance between strings.
    /// </summary>
    public interface IStringDistanceService
    {
        /// <summary>
        /// Gets the minimal number of insertions, deletions, symbol substitutions and transpositions
        /// required to transform one string into another.
        /// </summary>
        /// <param name="left">The first string.</param>
        /// <param name="right">The second string.</param>
        /// <returns>The Damerau-Levenshtein distance between the strings.</returns>
        int GetDamerauLevenshteinDistance(string left, string right);

        /// <summary>
        /// Gets the minimal number of insertions, deletions and symbol substitutions
        /// required to transform one string into another.
        /// </summary>
        /// <param name="left">The first string.</param>
        /// <param name="right">The second string.</param>
        /// <returns>The Levenshtein distance between the strings.</returns>
        int GetLevenshteinDistance(string left, string right);

        /// <summary>
        /// Gets the minimal number of insertions, deletions, symbol substitutions and transpositions
        /// required to transform one string into another where no substring is edited more than once.
        /// </summary>
        /// <param name="left">The first string.</param>
        /// <param name="right">The second string.</param>
        /// <returns>The Damerau-Levenshtein distance between the strings.</returns>
        int GetOptimalStringAlignmentDistance(string left, string right);
    }
}
