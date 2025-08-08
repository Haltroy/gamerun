using System.Collections.Generic;

namespace Gamerun.Shared;

/// <summary>
///     Arguments to run & set on game start and end.
/// </summary>
public class GamerunStartArguments
{
    /// <summary>
    ///     Adds commands before the app path.
    ///     <para>PREFIX /usr/bin/game</para>
    /// </summary>
    public string Prefix { get; set; } = string.Empty;

    /// <summary>
    ///     Adds commands after the app path. Mostly will be interpreted as arguments for the app.
    ///     <para>/usr/bin/game POSTFIX</para>
    /// </summary>
    public string Postfix { get; set; } = string.Empty;

    /// <summary>
    ///     Environment variables to set before launching the app.
    /// </summary>
    public Dictionary<string, string> Environment { get; set; } = new();

    /// <summary>
    ///     User's start script path. This will be executed before app starts.
    /// </summary>
    public string StartScript { get; set; } = string.Empty;

    /// <summary>
    ///     Determines if Daemon should be called or not.
    /// </summary>
    public bool RequireDaemonUse { get; set; } = false;

    /// <summary>
    ///     User's end script path. This will be executed after the app closes.
    /// </summary>
    public string EndScript { get; set; } = string.Empty;

    /// <summary>
    ///     Determines wait time for <see cref="StartScript" />.
    ///     <list type="table">
    ///         <listheader>
    ///             <term>Value</term>
    ///             <description>Action</description>
    ///         </listheader>
    ///         <item>
    ///             <term>-1</term>
    ///             <description>Waits the script to complete until starting.</description>
    ///         </item>
    ///         <item>
    ///             <term>0</term>
    ///             <description>Starts the script and the app simultaneously</description>
    ///         </item>
    ///         <item>
    ///             <term><c>n</c>&gt;0</term>
    ///             <description>Starts the script and waits <c>n</c> times, kills it then executes the app.</description>
    ///         </item>
    ///     </list>
    /// </summary>
    public int StartScriptTimeout { get; set; } = 0;

    /// <summary>
    ///     Determines wait time for <see cref="EndScript" />.
    ///     <list type="table">
    ///         <listheader>
    ///             <term>Value</term>
    ///             <description>Action</description>
    ///         </listheader>
    ///         <item>
    ///             <term>-1</term>
    ///             <description>Waits the script to complete until starting.</description>
    ///         </item>
    ///         <item>
    ///             <term>0</term>
    ///             <description>Starts the script asynchronously.</description>
    ///         </item>
    ///         <item>
    ///             <term><c>n</c>&gt;0</term>
    ///             <description>Starts the script and waits <c>n</c> times,then kills it.</description>
    ///         </item>
    ///     </list>
    /// </summary>
    public int EndScriptTimeout { get; set; } = 0;

    /// <summary>
    ///     Commands to execute before starting the app, used by modules. Recommended to only use non-root commands.
    /// </summary>
    public List<string> StartCommands { get; set; } = [];

    /// <summary>
    ///     Commands to execute after closing the app, used by modules. Recommended to only use non-root commands.
    /// </summary>
    public List<string> EndCommands { get; set; } = [];

    /// <summary>
    ///     DBus calls to do when starting the app, used by modules.
    /// </summary>
    public List<GamerunDBusCalls> StartDBusCalls { get; set; } = [];

    /// <summary>
    ///     DBus calls to do when closing the app, used by modules.
    /// </summary>
    public List<GamerunDBusCalls> EndDBusCalls { get; set; } = [];

    /// <summary>
    ///     Arguments to give to Gamerun daemon. It only accepts a fixed set of things for security reasons since it runs on
    ///     root.
    /// </summary>
    public GamerunDaemonArgs DaemonArgs { get; internal set; } = new();
}