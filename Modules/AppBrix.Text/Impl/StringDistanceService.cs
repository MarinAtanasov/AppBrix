// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

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

        if (left.Length == 0)
            return right.Length;
        if (right.Length == 0)
            return left.Length;
        if (left == right)
            return 0;

        this.GetMinMaxCharacters(left, right, out var minChar, out var maxChar);
        var charArray = new int[maxChar - minChar + 1];
        var matrix = this.CreateDamerauLevenshteinMatrix(left.Length, right.Length);

        for (var i = 0; i < left.Length; i++)
        {
            var db = 0;
            var leftI = left[i];
            var matrixIp1 = matrix[i + 1];
            var matrixIp2 = matrix[i + 2];
            for (var j = 0; j < right.Length; j++)
            {
                var rightJ = right[j];
                var k = charArray[rightJ - minChar];
                var l = db;

                var matrixIp2Jp1 = matrixIp2[j + 1];                        // insert
                var matrixIp1Jp2 = matrixIp1[j + 2];                        // delete
                var matrixIp1Jp1 = matrixIp1[j + 1];                        // match
                var matrixKLpIJ1mKL = matrix[k][l] + (i - k) + 1 + (j - l); // transpose

                if (leftI == rightJ)
                    db = j + 1;
                else
                    matrixIp1Jp1++;                                         // substitute

                var min = matrixIp2Jp1 < matrixIp1Jp2 ? matrixIp2Jp1 + 1 : matrixIp1Jp2 + 1;
                if (matrixIp1Jp1 < min)
                    min = matrixIp1Jp1;
                if (matrixKLpIJ1mKL < min)
                    min = matrixKLpIJ1mKL;

                matrixIp2[j + 2] = min;
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

        if (left.Length == 0)
            return right.Length;
        if (right.Length == 0)
            return left.Length;
        if (left == right)
            return 0;

        var next = new int[right.Length];
        for (var i = 0; i < next.Length;)
        {
            next[i] = ++i;
        }

        for (var i = 0; i < left.Length; i++)
        {
            var leftI = left[i];
            var previousJm1 = i;
            var nextJm1 = i + 1;
            for (var j = 0; j < right.Length; j++)
            {
                var previousJ = next[j];
                var min = nextJm1 < previousJ ? nextJm1 + 1 : previousJ + 1;
                if (previousJm1 < min)
                    min = leftI == right[j] ? previousJm1 : previousJm1 + 1;
                next[j] = nextJm1 = min;
                previousJm1 = previousJ;
            }
        }

        return next[^1];
    }

    public int GetOptimalStringAlignmentDistance(string left, string right)
    {
        if (left is null)
            throw new ArgumentNullException(nameof(left));
        if (right is null)
            throw new ArgumentNullException(nameof(right));

        if (left.Length == 0)
            return right.Length;
        if (right.Length == 0)
            return left.Length;
        if (left == right)
            return 0;

        var previous = new int[right.Length + 1];
        var current = new int[previous.Length];
        var next = new int[previous.Length];
        for (var i = 0; i < next.Length; i++)
        {
            next[i] = i;
        }

        for (var i = 0; i < left.Length; i++)
        {
            (previous, current, next) = (current, next, previous);

            // First element of next distance is A[i+1][0] edit distance is delete (i+1) chars from left to match right
            next[0] = i + 1;

            var leftI = left[i];
            for (var j = 0; j < right.Length; j++)
            {
                var currentJ = current[j];
                var currentJp1 = current[j + 1];
                var nextJ = next[j];
                var min = nextJ < currentJp1 ? nextJ + 1 : currentJp1 + 1;

                if (leftI != right[j])
                {
                    if (currentJ < min)
                        min = currentJ + 1;

                    // Check if 2 characters have been swapped
                    if (i > 0 && j > 0 && leftI == right[j - 1] && left[i - 1] == right[j] && previous[j - 1] < min)
                        min = previous[j - 1] + 1;
                }
                else if (currentJ < min)
                {
                    min = currentJ;
                }

                next[j + 1] = min;
            }
        }

        return next[^1];
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
        var maxDistance = leftLength > rightLength ? leftLength : rightLength;
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

    private void GetMinMaxCharacters(string left, string right, out int minChar, out int maxChar)
    {
        minChar = left[0];
        maxChar = left[0];

        for (var i = 1; i < left.Length; i++)
        {
            var character = (int)left[i];
            if (character < minChar)
                minChar = character;
            else if (character > maxChar)
                maxChar = character;
        }

        for (var i = 0; i < right.Length; i++)
        {
            var character = (int)right[i];
            if (character < minChar)
                minChar = character;
            else if (character > maxChar)
                maxChar = character;
        }
    }
    #endregion
}
