namespace MessageLogic
{
    /// <summary>
    /// Base data
    /// </summary>
    public record SimpleBody();

    /// <summary>
    /// With text
    /// </summary>
    public record Body(string Text) : SimpleBody();

    /// <summary>
    /// With text and Id
    /// </summary>
    public record FullBody(int Key, string Text) : Body(Text);
}
