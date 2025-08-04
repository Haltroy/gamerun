using System;

namespace Gamerun.Shared.Exceptions
{
    /// <summary>
    /// Exception class that is used by Gamerun.
    /// </summary>
    public class GamerunException : Exception
    {
        /// <summary>
        /// Creates a new Gamerun Exception with a message.
        /// </summary>
        /// <param name="message">Message of this exception.</param>
        public GamerunException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception to throw when file ends prematurely.
    /// </summary>
    public class GamerunEndOfStreamException : GamerunException
    {
        /// <summary>
        /// Creates a new Gamerun exception which is thrown when file ends prematurely.
        /// </summary>
        /// <param name="position">Position of stream that ended.</param>
        public GamerunEndOfStreamException(long position): base($"End of stream reached at position {position} prematurely.") {}
    }

    /// <summary>
    /// Exception to throw when App is not found.
    /// </summary>
    public class GamerunAppNotFoundException : GamerunException
    {
        /// <summary>
        /// Creates a new Gamerun exception which is thrown when app is not found.
        /// </summary>
        /// <param name="commandLine">Command line of app that couldn't be found.</param>
        public GamerunAppNotFoundException(string commandLine) : base($"App not found with command-line \"{commandLine}\".") { }
    }
}