namespace Gamerun.Shared.Exceptions;

/// <summary>
///     Exception to throw when socket is not found or a possible socket path is not found.
/// </summary>
public class GamerunSocketNoFoundException : GamerunException
{
    /// <summary>
    ///     Creates a new Gamerun exception which is thrown when socket is not found or a possible socket path is not found.
    /// </summary>
    public GamerunSocketNoFoundException(bool notFound) : base(notFound
        ? Translations.Translations.ExceptionSocketNotFound
        : Translations.Translations.ExceptionSocketNotUsable)
    {
    }
}