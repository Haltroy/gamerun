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
        ///     Generates the command.
        /// </summary>
        /// <returns>Command to run.</returns>
        public abstract string
            GenerateCommand(); // TODO: This should return a class that has EnvironmentVariables and Command

        /// <summary>
        ///     This event will be fired when a setting changes to dynamically save it.
        /// </summary>
        public abstract event GamerunSettingSaveDelegate? OnSave;
    }
}