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
    public App(string commandLine, AppSettings? settings = null)
    {
        CommandLine = commandLine;
        _Settings = settings;
    }

    #endregion CONSTRUCTORS

    #region PRIVATES

    private string _CommandLine;
    private AppSettings? _Settings;

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

    #endregion PROPERTIES
}