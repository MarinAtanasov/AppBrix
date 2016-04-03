using System;
using System.Linq;

namespace AppBrix.ConsoleApp
{
    internal class MessageGenerator
    {
        #region Construction
        public MessageGenerator(string level, int messagesSent = 0 )
        {
            this.Level = level;
            this.MessagesSent = messagesSent;
        }
        #endregion

        #region Properties
        public string Level { get; }

        public int MessagesSent { get; private set; }
        #endregion

        #region Public methods
        public string Generate()
        {
            return string.Format("{0} message: {1}", this.Level, ++this.MessagesSent);
        }
        #endregion
    }
}
