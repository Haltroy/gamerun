namespace Gamerun.Shared.Exceptions;

/// <summary>
///     Exception to throw when App is not found.
/// </summary>
public class GamerunAppNotFoundException : GamerunException
{
    /// <summary>
    ///     Creates a new Gamerun exception which is thrown when app is not found.
    /// </summary>
    /// <param name="commandLine">Command line of app that couldn't be found.</param>
    public GamerunAppNotFoundException(string commandLine) : base(
        Translations.Translations.AppNotFound.Replace("%cmd%", commandLine))
    {
    }
}