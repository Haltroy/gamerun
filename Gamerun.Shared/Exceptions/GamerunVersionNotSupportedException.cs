namespace Gamerun.Shared.Exceptions;

/// <summary>
///     Exception to throw when a config reads an unsupported version of file.
/// </summary>
public class GamerunVersionNotSupportedException : GamerunException
{
    /// <summary>
    ///     Creates a new Gamerun exception which is thrown when a config reads an unsupported version of file.
    /// </summary>
    /// <param name="version">Version of the file.</param>
    /// <param name="currentVersion">Version of the config.</param>
    /// <param name="config">Name of the config.</param>
    public GamerunVersionNotSupportedException(int version, int currentVersion, string config) : base(string.Format(
        version > currentVersion
            ? Translations.Translations.ExceptionConfigVersion1
            : Translations.Translations.ExceptionConfigVersion2,
        config, currentVersion, version))
    {
    }
}