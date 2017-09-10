// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Cloning.Impl
{
    /// <summary>
    /// Class used for traversing a multidimentional array during deep cloning.
    /// </summary>
    internal sealed class ArrayTraverse
    {
        #region Construction
        public ArrayTraverse(Array array)
        {
            this.Position = new int[array.Rank];
            this.maxLengths = new int[array.Rank];
            for (int i = 0; i < array.Rank; i++)
            {
                this.maxLengths[i] = array.GetLength(i) - 1;
            }
        }
        #endregion

        #region Properties
        public int[] Position { get; private set; }
        #endregion

        #region Public and overriden methods
        public bool Step()
        {
            for (int i = 0; i < this.Position.Length; i++)
            {
                if (this.Position[i] < this.maxLengths[i])
                {
                    this.Position[i]++;
                    for (int j = 0; j < i; j++)
                    {
                        this.Position[j] = 0;
                    }
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Private fields and constants
        private int[] maxLengths;
        #endregion
    }
}
