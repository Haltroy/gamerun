using System.IO;

namespace Gamerun.Shared;

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
    public virtual bool IsDefaults => false;

    /// <summary>
    ///     Determines if this is the default configuration.
    /// </summary>
    public bool IsDefaultConfig { get; private set; }

    /// <summary>
    ///     Gets the visible name of this configuration.
    /// </summary>
    public string? Name
    {
        get => _name;
        set
        {
            _name = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Gets and sets the file name of this configuration.
    /// </summary>
    public required string FileName
    {
        get => _fileName.SanitizeFilename();
        set
        {
            var save = !string.IsNullOrWhiteSpace(_fileName);
            _fileName = value.SanitizeFilename();
            if (save) OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Reads data from <paramref name="stream" /> and sets the settings accordingly.
    /// </summary>
    /// <param name="stream">Stream to read the data from.</param>
    public virtual void ReadSettings(Stream stream)
    {
    }

    /// <summary>
    ///     Writes data to <paramref name="stream" />.
    /// </summary>
    /// <param name="stream"></param>
    public virtual void WriteSettings(Stream stream)
    {
    }

    /// <summary>
    ///     Generates the command, environment variables and start/end script(s).
    /// </summary>
    /// <param name="args">Arguments from previous module(s).</param>
    /// <returns>Command to run.</returns>
    public virtual GamerunStartArguments GenerateArgs(GamerunStartArguments args)
    {
        return args;
    }

    /// <summary>
    ///     Sets this setting as the default setting to be used in <see cref="Gamerun.Default" />.
    /// </summary>
    internal virtual void SetAsDefault()
    {
        IsDefaultConfig = true;
    }

    /// <summary>
    ///     This event will be fired when a setting changes to dynamically save it.
    /// </summary>
    public virtual event GamerunSettingSaveDelegate? OnSave;


    #region PRIVATES

    private string? _name;
    private string _fileName = string.Empty;

    #endregion PRIVATES
}