using System;

namespace Gamerun.Shared.Exceptions;

/// <summary>
///     Exception class that is used by Gamerun.
/// </summary>
public class GamerunException : Exception
{
    /// <summary>
    ///     Creates a new Gamerun Exception with a message.
    /// </summary>
    /// <param name="message">Message of this exception.</param>
    public GamerunException(string message) : base(message)
    {
    }
}

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
    public GamerunVersionNotSupportedException(int version, int currentVersion, string config) : base(
        version > currentVersion
            ? $"Configuration \"{config}\" is on version {currentVersion} while the file is on version {version}. Please update your Gamerun installations."
            : $"Configuration \"{config}\" does not support version {version} and support version {currentVersion} and onwards.")
    {
    }
}