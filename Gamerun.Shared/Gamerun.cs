using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Gamerun.Shared.Exceptions;

namespace Gamerun.Shared;

public static class Gamerun
{
    #region PROPERTIES

    /// <summary>
    ///     Determines if things can save into a file. This should be disabled when creating a brand new
    ///     <see cref="GamerunConfigAbstract" /> or <see cref="App" />.
    /// </summary>
    public static bool DoSave { get; set; } = true;

    #endregion PROPERTIES

    #region DEFAULT CONFIGS

    /// <summary>
    ///     The default configuration.
    /// </summary>
    public static AppConfig DefaultAppConfig = new()
        { FileName = "default", Name = Translations.Translations.ConfigDefaultName };

    /// <summary>
    ///     The default MangoHud configuration.
    /// </summary>
    public static MangoHudConfig DefaultMangoHUDConfig =
        new() { FileName = "default", Name = Translations.Translations.ConfigDefaultName };

    /// <summary>
    ///     The default Strangle configuration.
    /// </summary>
    public static StrangleConfig DefaultStrangleConfig =
        new() { FileName = "default", Name = Translations.Translations.ConfigDefaultName };

    /// <summary>
    ///     The default Gamescope configuration.
    /// </summary>
    public static GamescopeConfig DefaultGamescopeConfig =
        new() { FileName = "default", Name = Translations.Translations.ConfigDefaultName };

    #endregion DEFAULT CONFIGS

    #region LISTS

    /// <summary>
    ///     List of <see cref="App" />.
    /// </summary>
    public static List<App> Apps = [];

    /// <summary>
    ///     List of <see cref="AppConfig" />. It should include the <see cref="DefaultAppConfig" /> at all times.
    /// </summary>
    public static List<AppConfig> Configs = [DefaultAppConfig];

    /// <summary>
    ///     List of <see cref="MangoHudConfig" />. It should include the <see cref="DefaultMangoHUDConfig" /> at all times.
    /// </summary>
    public static List<MangoHudConfig> MangoHudConfigs = [DefaultMangoHUDConfig];

    /// <summary>
    ///     List of <see cref="StrangleConfig" />. It should include the <see cref="DefaultStrangleConfig" /> at all times.
    /// </summary>
    public static List<StrangleConfig> StrangleConfigs = [DefaultStrangleConfig];

    /// <summary>
    ///     List of <see cref="GamescopeConfig" />. It should include the <see cref="DefaultGamescopeConfig" /> at all times.
    /// </summary>
    public static List<GamescopeConfig> GamescopeConfigs = [DefaultGamescopeConfig];

    /// <summary>
    ///     List of GPUs in this system (listed from highest to lowest in pwerformance).
    /// </summary>
    public static GpuInfo[] GPUs { get; } = Tools.GetAllGpus();

    #endregion LISTS

    #region PATHS

    /// <summary>
    ///     The application data folder of Gamerun. All other configurations and app information are stored here.
    ///     <para />
    ///     <c>$HOME/.config/gamerun</c>
    /// </summary>
    private static string AppPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "gamerun");

    /// <summary>
    ///     Folder that holds app information for each <see cref="App" />.
    ///     <para />
    ///     <c>$HOME/.config/gamerun/apps</c>
    /// </summary>
    public static string ListingPath => Path.Combine(AppPath, "apps");

    /// <summary>
    ///     Folder that hosts each <see cref="AppConfig" />.
    ///     <para />
    ///     <c>$HOME/.config/gamerun/configs</c>
    /// </summary>
    public static string ConfigsPath => Path.Combine(AppPath, "configs");

    /// <summary>
    ///     Folder that hosts each <see cref="MangoHudConfig" />.
    ///     <para />
    ///     Not to be confused with MangoHud's own configuration folder which is located in <c>$HOME/.config/MangoHud</c>.
    ///     <para />
    ///     <c>$HOME/.config/gamerun/mangohud</c>
    /// </summary>
    public static string MangoHudConfigsPath => Path.Combine(AppPath, "mangohud");

    /// <summary>
    ///     Folder that hosts each <see cref="StrangleConfig" />.
    ///     <para />
    ///     <c>$HOME/.config/gamerun/strangle</c>
    /// </summary>
    public static string StrangleConfigsPath => Path.Combine(AppPath, "strangle");

    /// <summary>
    ///     Folder that hosts each <see cref="GamescopeConfig" />.
    ///     <para />
    ///     <c>$HOME/.config/gamerun/gamescope</c>
    /// </summary>
    public static string GamescopeConfigsPath => Path.Combine(AppPath, "gamescope");

    #endregion PATHS

    #region FUNCTIONS

    /// <summary>
    ///     Gets an <see cref="App" /> from <paramref name="commandLine" />. If doesn't exists then creates it if
    ///     <paramref name="createIfNotExists" /> is <c>true</c>.
    ///     <para />
    ///     NOTE: This function also calls <see cref="SaveListing" />.
    /// </summary>
    /// <param name="commandLine">Command-line of the app.</param>
    /// <param name="createIfNotExists"><c>true</c> to create it if not exists, otherwise <c>false</c>.</param>
    /// <returns><see cref="App" />.</returns>
    /// <exception cref="GamerunAppNotFoundException">
    ///     Thrown if <paramref name="createIfNotExists" /> is <c>false</c> and app
    ///     was not found.
    /// </exception>
    public static App GetApp(string commandLine, bool createIfNotExists = false)
    {
        Init();
        if (!Directory.Exists(ListingPath) && new DirectoryInfo(ListingPath).GetFiles().Length <= 0)
        {
            if (!createIfNotExists) throw new GamerunAppNotFoundException(commandLine);
            DoSave = true;
            return CreateApp(commandLine);
        }

        var files = Directory.GetFiles(ListingPath);
        foreach (var file in files)
        {
            using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(fileStream);
            using var writer = new StreamWriter(fileStream);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                var lineSplit = line.Split('|');
                if (lineSplit.Length != 6) continue;
                if (!new Regex(lineSplit[5]).IsMatch(commandLine)) continue;
                DoSave = false;
                App app = new()
                {
                    FileName = file,
                    LastAccess = DateTime.Now
                };
                if (lineSplit[1] != "default")
                {
                    using var stream = new FileStream(Path.Combine(ConfigsPath, lineSplit[1]), FileMode.Create,
                        FileAccess.Write);
                    app.Config = new AppConfig { FileName = lineSplit[1] };
                    app.Config.Read(stream);
                }

                if (lineSplit[2] != "default")
                {
                    using var stream = new FileStream(Path.Combine(StrangleConfigsPath, lineSplit[2]), FileMode.Create,
                        FileAccess.Write);
                    app.Strangle = new StrangleConfig { FileName = lineSplit[1] };
                    app.Strangle.Read(stream);
                }

                if (lineSplit[3] != "default")
                {
                    using var stream = new FileStream(Path.Combine(MangoHudConfigsPath, lineSplit[3]), FileMode.Create,
                        FileAccess.Write);
                    app.MangoHUD = new MangoHudConfig { FileName = lineSplit[1] };
                    app.MangoHUD.Read(stream);
                }

                if (lineSplit[4] != "default")
                {
                    using var stream = new FileStream(Path.Combine(GamescopeConfigsPath, lineSplit[4]), FileMode.Create,
                        FileAccess.Write);
                    app.Gamescope = new GamescopeConfig { FileName = lineSplit[1] };
                    app.Gamescope.Read(stream);
                }

                DoSave = true;
                SaveListing();
                Apps.Add(app);
                return app;
            }
        }

        if (!createIfNotExists) throw new GamerunAppNotFoundException(commandLine);
        DoSave = true;
        return CreateApp(commandLine);
    }

    /// <summary>
    ///     Saves all <see cref="Apps" /> into their own files.
    /// </summary>
    public static void SaveListing()
    {
        if (!DoSave) return;
        using var fileStream = new FileStream(ListingPath,
            File.Exists(ListingPath) ? FileMode.Truncate : FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        using var writer = new StreamWriter(fileStream);
        foreach (var app in Apps)
            writer.WriteLine(
                $"{app.LastAccess.ToBinary()}|" +
                $"{(app.Config != null ? app.Config.FileName : DefaultAppConfig.FileName)}|" +
                $"{(app.Strangle != null ? app.Strangle.FileName : DefaultStrangleConfig.FileName)}|" +
                $"{(app.MangoHUD != null ? app.MangoHUD.FileName : DefaultMangoHUDConfig.FileName)}|" +
                $"{(app.Gamescope != null ? app.Gamescope.FileName : DefaultGamescopeConfig.FileName)}|" +
                $"{app.CommandLine}");
    }

    /// <summary>
    ///     Creates an app. Used by the client.
    ///     <para />
    ///     NOTE: Created app will call <see cref="SaveListing" />.
    /// </summary>
    /// <param name="commandLine">Command-line of the app.</param>
    /// <returns><see cref="App" />.</returns>
    private static App CreateApp(string commandLine)
    {
        var app = new App { CommandLine = commandLine, FileName = Tools.GenerateUniqueFileName(ListingPath) };
        return app;
    }

    /// <summary>
    ///     Initializes this class.
    /// </summary>
    /// <param name="full"><c>true</c> to also call <see cref="Refresh" />, otherwise <c>false</c>.</param>
    public static void Init(bool full = false)
    {
        DoSave = false;
        foreach (var folder in new Dictionary<string, GamerunConfigAbstract?>
                 {
                     { AppPath, null },
                     { ListingPath, null },
                     { ConfigsPath, DefaultAppConfig },
                     { MangoHudConfigsPath, DefaultMangoHUDConfig },
                     { StrangleConfigsPath, DefaultStrangleConfig },
                     { GamescopeConfigsPath, DefaultGamescopeConfig }
                     // Template: { Configs Path, Default Config }
                 })
        {
            if (!Directory.Exists(folder.Key)) Directory.CreateDirectory(folder.Key);
            if (folder.Value == null) continue;
            var file = Path.Combine(folder.Key, folder.Value.FileName);
            folder.Value.SetAsDefault();
            folder.Value.OnSave += () =>
            {
                if (!DoSave) return;
                using var stream = new FileStream(file, File.Exists(file) ? FileMode.Truncate : FileMode.Open,
                    FileAccess.Write, FileShare.ReadWrite);
                folder.Value.WriteSettings(stream);
            };
            using var configStream =
                new FileStream(file, FileMode.Open, FileAccess.Read,
                    FileShare.ReadWrite);
            folder.Value.Read(configStream);
        }

        if (full) Refresh();
        DoSave = true;
    }

    /// <summary>
    ///     Refreshes everything.
    /// </summary>
    public static void Refresh()
    {
        DoSave = false;
        // Refresh configs first then apps.
        RefreshConfigs();
        RefreshMangoHudConfigs();
        RefreshStrangleConfigs();
        RefreshGamescopeConfigs();
        RefreshApps();
        DoSave = true;
    }

    #region REFRESH PRIVATES

    private static void RefreshApps()
    {
        var files = Directory.GetFiles(ListingPath);
        foreach (var file in files)
        {
            using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var streamReader = new StreamReader(fileStream);
            var found = Apps.FindAll(x => x.FileName == Path.GetFileNameWithoutExtension(file));
            if (found.Count > 0)
            {
                found.ForEach(x => LoadApp(streamReader, x));
            }
            else
            {
                var newApp = new App { FileName = Path.GetFileNameWithoutExtension(file) };
                LoadApp(streamReader, newApp);
                Apps.Add(newApp);
            }
        }
    }

    private static void LoadApp(StreamReader reader, App app)
    {
        var line = reader.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;
        var split = line.Split('|');
        if (split.Length != 6) return;
        if (!long.TryParse(split[0], out var lastAccessLong)) return;
        app.LastAccess = DateTime.FromBinary(lastAccessLong);
        var configFound = Configs.FindAll(x => x.FileName == split[1]);
        var strangleFound = StrangleConfigs.FindAll(x => x.FileName == split[2]);
        var mangohudFound = MangoHudConfigs.FindAll(x => x.FileName == split[3]);
        var gamescopeFound = GamescopeConfigs.FindAll(x => x.FileName == split[4]);
        var commandLine = split[5];
        if (configFound.Count > 0) app.Config = configFound[0];
        if (strangleFound.Count > 0) app.Strangle = strangleFound[0];
        if (mangohudFound.Count > 0) app.MangoHUD = mangohudFound[0];
        if (gamescopeFound.Count > 0) app.Gamescope = gamescopeFound[0];
        if (!string.IsNullOrEmpty(commandLine)) app.CommandLine = commandLine;
    }

    private static void RefreshConfigs()
    {
        var files = Directory.GetFiles(ConfigsPath);
        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var found = Configs.FindAll(x => x.FileName == fileName);
            if (found.Count > 0)
            {
                found.ForEach(x => x.Read(fileStream));
                continue;
            }

            var newConfig = new AppConfig { FileName = fileName };
            newConfig.OnSave += () =>
            {
                if (!DoSave) return;
                using var stream = new FileStream(file, File.Exists(file) ? FileMode.Truncate : FileMode.Open,
                    FileAccess.Write, FileShare.ReadWrite);
                newConfig.WriteSettings(stream);
            };
            newConfig.Read(fileStream);
            Configs.Add(newConfig);
        }
    }

    private static void RefreshMangoHudConfigs()
    {
        var files = Directory.GetFiles(MangoHudConfigsPath);
        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var found = MangoHudConfigs.FindAll(x => x.FileName == fileName);
            if (found.Count > 0)
            {
                found.ForEach(x => x.Read(fileStream));
                continue;
            }

            var newConfig = new MangoHudConfig { FileName = fileName };
            newConfig.OnSave += () =>
            {
                if (!DoSave) return;
                using var stream = new FileStream(file, File.Exists(file) ? FileMode.Truncate : FileMode.Open,
                    FileAccess.Write, FileShare.ReadWrite);
                newConfig.WriteSettings(stream);
            };
            newConfig.Read(fileStream);
            MangoHudConfigs.Add(newConfig);
        }
    }

    private static void RefreshStrangleConfigs()
    {
        var files = Directory.GetFiles(StrangleConfigsPath);
        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var found = StrangleConfigs.FindAll(x => x.FileName == fileName);
            if (found.Count > 0)
            {
                found.ForEach(x => x.Read(fileStream));
                continue;
            }

            var newConfig = new StrangleConfig { FileName = fileName };
            newConfig.OnSave += () =>
            {
                if (!DoSave) return;
                using var stream = new FileStream(file, File.Exists(file) ? FileMode.Truncate : FileMode.Open,
                    FileAccess.Write, FileShare.ReadWrite);
                newConfig.WriteSettings(stream);
            };
            newConfig.Read(fileStream);
            StrangleConfigs.Add(newConfig);
        }
    }

    private static void RefreshGamescopeConfigs()
    {
        var files = Directory.GetFiles(GamescopeConfigsPath);
        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var found = GamescopeConfigs.FindAll(x => x.FileName == fileName);
            if (found.Count > 0)
            {
                found.ForEach(x => x.Read(fileStream));
                continue;
            }

            var newConfig = new GamescopeConfig { FileName = fileName };
            newConfig.OnSave += () =>
            {
                if (!DoSave) return;
                using var stream = new FileStream(file, File.Exists(file) ? FileMode.Truncate : FileMode.Open,
                    FileAccess.Write, FileShare.ReadWrite);
                newConfig.WriteSettings(stream);
            };
            newConfig.Read(fileStream);
            GamescopeConfigs.Add(newConfig);
        }
    }

    #endregion REFRESH PRIVATES

    #endregion FUNCTIONS
}