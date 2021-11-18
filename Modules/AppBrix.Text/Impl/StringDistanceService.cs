// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Text.Impl;

internal sealed class StringDistanceService : IStringDistanceService
{
    #region Public and overriden methods
    public int GetDamerauLevenshteinDistance(string left, string right)
    {
        if (left is null)
            throw new ArgumentNullException(nameof(left));
        if (right is null)
            throw new ArgumentNullException(nameof(right));

        if (left == right)
            return 0;

        var leftLength = left.Length;
        var rightLength = right.Length;

        if (leftLength == 0)
            return rightLength;
        if (rightLength == 0)
            return leftLength;

        var minChar = Math.Min((int)left.Min(), right.Min());
        var maxChar = Math.Max((int)left.Max(), right.Max());
        var charArray = new int[maxChar - minChar + 1];
        var matrix = this.CreateDamerauLevenshteinMatrix(leftLength, rightLength);

        for (var i = 0; i < leftLength; i++)
        {
            var db = 0;
            var leftI = left[i];
            var matrixIp1 = matrix[i + 1];
            var matrixIp2 = matrix[i + 2];
            for (var j = 0; j < rightLength; j++)
            {
                var rightJ = right[j];
                var k = charArray[rightJ - minChar];
                var l = db;

                var cost = 0;
                if (leftI == rightJ)
                    db = j + 1;
                else
                    cost = 1;

                matrixIp2[j + 2] = Math.Min(
                    Math.Min(
                        matrixIp2[j + 1],                     // insert
                        matrixIp1[j + 2]                      // delete
                    ) + 1,
                    Math.Min(
                        matrixIp1[j + 1] + cost,              // substitute
                        matrix[k][l] + (i - k) + 1 + (j - l)  // transpose
                    )
                );
            }

            charArray[leftI - minChar] = i + 1;
        }

        return matrix[^1][^1];
    }

    public int GetLevenshteinDistance(string left, string right)
    {
        if (left is null)
            throw new ArgumentNullException(nameof(left));
        if (right is null)
            throw new ArgumentNullException(nameof(right));

        if (left == right)
            return 0;

        var leftLength = left.Length;
        var rightLength = right.Length;

        if (leftLength == 0)
            return rightLength;
        if (rightLength == 0)
            return leftLength;

        var previous = new int[rightLength + 1];
        var next = new int[previous.Length];
        for (var i = 0; i < next.Length; i++)
        {
            next[i] = i;
        }

        for (var i = 0; i < leftLength; i++)
        {
            (previous, next) = (next, previous);

            // First element of next distance is A[i+1][0] edit distance is delete (i+1) chars from left to match right
            next[0] = i + 1;

            var leftI = left[i];
            for (var j = 0; j < rightLength; j++)
            {
                next[j + 1] = Math.Min(
                    Math.Min(
                        next[j],         // insert
                        previous[j + 1]  // delete
                    ) + 1,
                    leftI == right[j] ?
                    previous[j] :        // match
                    previous[j] + 1      // substitute
                );
            }
        }

        return next[rightLength];
    }

    public int GetOptimalStringAlignmentDistance(string left, string right)
    {
        if (left is null)
            throw new ArgumentNullException(nameof(left));
        if (right is null)
            throw new ArgumentNullException(nameof(right));

        if (left == right)
            return 0;

        var leftLength = left.Length;
        var rightLength = right.Length;

        if (leftLength == 0)
            return rightLength;
        if (rightLength == 0)
            return leftLength;

        var previous = new int[rightLength + 1];
        var current = new int[previous.Length];
        var next = new int[previous.Length];
        for (var i = 0; i < next.Length; i++)
        {
            next[i] = i;
        }

        for (var i = 0; i < leftLength; i++)
        {
            (previous, current, next) = (current, next, previous);

            // First element of next distance is A[i+1][0] edit distance is delete (i+1) chars from left to match right
            next[0] = i + 1;

            var leftI = left[i];
            for (var j = 0; j < rightLength; j++)
            {
                // next[j] is insert, current[j+1] is delete, current[j] is match, current[j] + 1 is substitute
                var min = Math.Min(next[j], current[j + 1]) + 1;

                if (leftI == right[j])
                {
                    min = Math.Min(min, current[j]);
                }
                else
                {
                    min = Math.Min(min, current[j] + 1);

                    // Check if 2 characters have been swapped
                    if (i > 0 && j > 0 && leftI == right[j - 1] && left[i - 1] == right[j])
                        min = Math.Min(min, previous[j - 1] + 1);
                }

                next[j + 1] = min;
            }
        }

        return next[rightLength];
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Creates and initializes a matrix used in the Damerau-Levenshtein distance algorithm.
    /// </summary>
    /// <remarks>
    /// The matrix is initialized as follows, where N is the maximum possible distance between the two strings:
    /// [N N N N N]
    /// [N 0 1 2 3]
    /// [N 1 0 0 0]
    /// [N 2 0 0 0]
    /// </remarks>
    /// <param name="leftLength">The length of the left string.</param>
    /// <param name="rightLength">The legnth of the right string.</param>
    /// <returns>The initialized matrix.</returns>
    private int[][] CreateDamerauLevenshteinMatrix(int leftLength, int rightLength)
    {
        var maxDistance = Math.Max(leftLength, rightLength);
        var rowLength = rightLength + 2;
        var matrix = new int[leftLength + 2][];

        // First row
        var row = new int[rowLength];
        for (var i = 0; i < rowLength; i++)
        {
            row[i] = maxDistance;
        }
        matrix[0] = row;

        // Second row
        row = new int[rowLength];
        row[0] = maxDistance;
        for (var i = 1; i < rowLength; i++)
        {
            row[i] = i - 1;
        }
        matrix[1] = row;

        // Remaining rows
        for (var i = 2; i < matrix.Length; i++)
        {
            row = new int[rowLength];
            row[0] = maxDistance;
            row[1] = i - 1;
            matrix[i] = row;
        }

        return matrix;
    }
    #endregion
}
