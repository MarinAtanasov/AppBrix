// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Cloning;
using System;
using System.Linq;

namespace AppBrix
{
    public static class CloningExtensions
    {
        /// <summary>
        /// Gets the registered <see cref="ICloner"/>.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <returns>The registered <see cref="ICloner"/>.</returns>
        public static ICloner GetCloner(this IApp app)
        {
            return app.Get<ICloner>();
        }

        /// <summary>
        /// Traverses a multidimentional array and executes an action for every item.
        /// </summary>
        /// <param name="array">The array to be traversed.</param>
        /// <param name="action">The action to be executed on every element.</param>
        internal static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.Length == 0)
                return;

            var walker = new ArrayTraverse(array);
            do
            {
                action(array, walker.Position);
            }
            while (walker.Step());
        }

        #region Private classes
        private class ArrayTraverse
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

            #region Public methods
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
        #endregion
    }
}
