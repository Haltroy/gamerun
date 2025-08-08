using System;

namespace Gamerun.Shared;

/// <summary>
///     Represents an app to run with Gamerun.
/// </summary>
public class App
{
    #region CONSTRUCTORS

    /// <summary>
    ///     Creats a new App.
    /// </summary>
    /// <param name="commandLine">Command line of this app.</param>
    public App(string commandLine)
    {
        CommandLine = commandLine;
    }

    #endregion CONSTRUCTORS

    #region FUNCTIONS

    /// <summary>
    ///     Generates a <see cref="GamerunStartArguments" /> to run the app with Gamerun settings.
    /// </summary>
    /// <returns>
    ///     <see cref="GamerunStartArguments" />
    /// </returns>
    public GamerunStartArguments GenerateStartArgs()
    {
        var args = new GamerunStartArguments();
        args = (Strangle ?? Gamerun.DefaultStrangleConfig).GenerateArgs(args);
        args = (MangoHUD ?? Gamerun.DefaultMangoHUDConfig).GenerateArgs(args);
        args = (Gamescope ?? Gamerun.DefaultGamescopeConfig).GenerateArgs(args);
        args = (Settings ?? Gamerun.DefaultAppConfig).GenerateArgs(args);
        return args;
    }

    #endregion FUNCTIONS

    #region PRIVATES

    private string? _CommandLine;
    private AppConfig? _Settings;
    private MangoHudConfig? _MangoHUDSettings;
    private StrangleConfig? _StrangleSettings;
    private GamescopeConfig? _GamescopeSettings;
    private DateTime _LastAccess = DateTime.Now;

    #endregion PRIVATES

    #region PROPERTIES

    /// <summary>
    ///     Command line of this app. Can be Regex.
    /// </summary>
    public string CommandLine
    {
        get => _CommandLine ?? string.Empty;
        set
        {
            _CommandLine = value;
            Gamerun.SaveListing();
        }
    }

    /// <summary>
    /// Determines the last access of this app.
    /// </summary>
    public DateTime LastAccess
    {
        get => _LastAccess;
        set
        {
            _LastAccess = value;
            Gamerun.SaveListing();
        }
    }

    /// <summary>
    ///     Configuration of this app. If it is null, use <see cref="Gamerun.DefaultAppConfig" /> instead.
    /// </summary>
    public AppConfig? Settings
    {
        get => _Settings;
        set
        {
            _Settings = value;
            Gamerun.SaveListing();
        }
    }

    /// <summary>
    ///     Configuration for MangoHUD for this app. If it is null, use <see cref="Gamerun.DefaultMangoHUDConfig" /> instead.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once MemberCanBePrivate.Global
    public MangoHudConfig? MangoHUD
    {
        get => _MangoHUDSettings;
        set
        {
            _MangoHUDSettings = value;
            Gamerun.SaveListing();
        }
    }

    /// <summary>
    ///     LibStrangle settings. If it is null, use <see cref="Gamerun.DefaultStrangleConfig" /> instead.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public StrangleConfig? Strangle
    {
        get => _StrangleSettings;
        set
        {
            _StrangleSettings = value;
            Gamerun.SaveListing();
        }
    }

    /// <summary>
    ///     Gamescope settings. If it is null, use <see cref="Gamerun.DefaultGamescopeConfig" /> instead.
    /// </summary>
    public GamescopeConfig? Gamescope
    {
        get => _GamescopeSettings;
        set
        {
            _GamescopeSettings = value;
            Gamerun.SaveListing();
        }
    }

    #endregion PROPERTIES
}