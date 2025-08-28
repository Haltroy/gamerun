namespace Gamerun.Shared.Exceptions;

/// <summary>
///     Exception to throw when file ends prematurely.
/// </summary>
public class GamerunEndOfStreamException : GamerunException
{
    /// <summary>
    ///     Creates a new Gamerun exception which is thrown when file ends prematurely.
    /// </summary>
    /// <param name="position">Position of stream that ended.</param>
    public GamerunEndOfStreamException(long position) : base(
        Translations.Translations.ExceptionEndOfStream.Replace("%pos%", $"{position}"))
    {
    }
}