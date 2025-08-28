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