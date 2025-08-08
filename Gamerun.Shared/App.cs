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
    /// <param name="settings">Configuration of this app, use <c>null</c> for using the default configuration instead.</param>
    public App(string commandLine)
    {
        CommandLine = commandLine;
    }

    #endregion CONSTRUCTORS

    #region FUNCTIONS

    public GamerunStartArguments GenerateStartArgs()
    {
        // TODO
        throw new NotImplementedException();
    }

    #endregion FUNCTIONS

    #region PRIVATES

    private string _CommandLine;
    private AppSettings? _Settings;
    private MangoHUDSettings? _MangoHUDSettings;
    private StrangleSettings? _StrangleSettings;
    private GamescopeSettings? _GamescopeSettings;

    #endregion PRIVATES

    #region PROPERTIES

    /// <summary>
    ///     Command line of this app. Can be Regex.
    /// </summary>
    public string CommandLine
    {
        get => _CommandLine;
        set
        {
            _CommandLine = value;
            Gamerun.SaveListing();
        }
    }

    /// <summary>
    ///     Configuration of this app. If it is null, use <see cref="Gamerun.Default" /> instead.
    /// </summary>
    public AppSettings? Settings
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
    public MangoHUDSettings? MangoHUD
    {
        get => _MangoHUDSettings;
        set
        {
            _MangoHUDSettings = value;
            Gamerun.SaveListing();
        }
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public StrangleSettings? Strangle
    {
        get => _StrangleSettings;
        set
        {
            _StrangleSettings = value;
            Gamerun.SaveListing();
        }
    }

    public GamescopeSettings? Gamescope
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