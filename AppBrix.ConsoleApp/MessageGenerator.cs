using System;
using System.Linq;

namespace AppBrix.ConsoleApp
{
    internal class MessageGenerator
    {
        #region Construction
        public MessageGenerator(string level)
        {
            this.level = level;
        }
        #endregion

        #region Public methods
        public string Generate()
        {
            return string.Format("{0} message: {1}", this.level, this.number++);
        }
        #endregion

        #region Private fields and constants
        private int number = 1;
        private readonly string level;
        #endregion
    }
}
