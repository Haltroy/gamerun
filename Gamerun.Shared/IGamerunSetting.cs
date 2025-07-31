using System.IO;

namespace Gamerun.Shared
{
    /// <summary>
    ///     Abstract class for different settings classes for Gamerun.
    /// </summary>
    public abstract class GamerunSettingsAbstract
    {
        /// <summary>
        ///     Delegate for the save event.
        /// </summary>
        public delegate void GamerunSettingSaveDelegate();

        /// <summary>
        ///     Determines if this uses the defaults. Used to detect if a class needs saving to a file.
        /// </summary>
        public abstract bool IsDefaults { get; }

        /// <summary>
        ///     Reads data from <paramref name="stream" /> and sets the settings accordingly.
        /// </summary>
        /// <param name="stream">Stream to read the data from.</param>
        public abstract void ReadSettings(Stream stream);

        /// <summary>
        ///     Writes data to <paramref name="stream" />.
        /// </summary>
        /// <param name="stream"></param>
        public abstract void WriteSettings(Stream stream);

        /// <summary>
        ///     Generates the command, environment variables and start/end script(s).
        /// </summary>
        /// <param name="args">Arguments from previous module(s).</param>
        /// <returns>Command to run.</returns>
        public abstract GamerunStartArguments GenerateArgs(GamerunStartArguments args);

        /// <summary>
        ///     This event will be fired when a setting changes to dynamically save it.
        /// </summary>
        public abstract event GamerunSettingSaveDelegate? OnSave;
    }
}