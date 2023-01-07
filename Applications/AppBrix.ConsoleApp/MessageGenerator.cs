namespace AppBrix.ConsoleApp;

internal sealed class MessageGenerator
{
    #region Construction
    public MessageGenerator(string level, int messagesSent = 0)
    {
        this.Level = level;
        this.MessagesSent = messagesSent;
    }
    #endregion

    #region Properties
    private string Level { get; }

    private int MessagesSent { get; set; }
    #endregion

    #region Public and overriden methods
    public string Generate() => $"{this.Level} message: {++this.MessagesSent}";
    #endregion
}
