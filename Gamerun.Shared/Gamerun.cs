using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Gamerun.Shared.Exceptions;

namespace Gamerun.Shared;

public static class Gamerun
{
    public static AppConfig DefaultAppConfig = new() { FileName = "default", Name = Translations.Translations.ConfigDefaultName };
    public static MangoHudConfig DefaultMangoHUDConfig = new() { FileName = "default", Name = Translations.Translations.ConfigDefaultName };
    public static StrangleConfig DefaultStrangleConfig = new() { FileName = "default", Name = Translations.Translations.ConfigDefaultName };

    public static GamescopeConfig DefaultGamescopeConfig =
        new() { FileName = "default", Name = Translations.Translations.ConfigDefaultName };

    public static List<App> Apps = [];
    public static List<AppConfig> Configs = [DefaultAppConfig];
    public static List<MangoHudConfig> MangoHudConfigs = [DefaultMangoHUDConfig];
    public static List<StrangleConfig> StrangleConfigs = [DefaultStrangleConfig];
    public static List<GamescopeConfig> GamescopeConfigs = [DefaultGamescopeConfig];
    public static GpuInfo[] GPUs { get; set; } = Tools.GetAllGpus();

    private static string AppPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "gamerun");

    public static string ListingPath => Path.Combine(AppPath, "listing");

    public static string ConfigsPath => Path.Combine(AppPath, "configs");
    public static string MangoHudConfigsPath => Path.Combine(AppPath, "mangohud");
    public static string StrangleConfigsPath => Path.Combine(AppPath, "strangle");
    public static string GamescopeConfigsPath => Path.Combine(AppPath, "gamescope");

    public static App GetApp(string commandLine, bool NoFullInit, bool createIfNotExists = false)
    {
        // TODO: Fix this code
        Init(!NoFullInit);
        if (!Directory.Exists(ListingPath) && new DirectoryInfo(ListingPath).GetFiles().Length <= 0)
        {
            if (createIfNotExists) return CreateApp(commandLine);
            throw new GamerunAppNotFoundException(commandLine);
        }

        if (NoFullInit)
        {
            var files = Directory.GetFiles(ListingPath);
            foreach (var file in files)
            {
                using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(fileStream);
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var lineSplit = line.Split('|');
                    if (lineSplit.Length != 2) continue;
                    if (!new Regex(lineSplit[1]).IsMatch(commandLine)) continue;
                    if (!int.TryParse(lineSplit[0], out var configId)) continue;
                    var configPath = Path.Combine(ConfigsPath, $"{configId}");
                    if (!File.Exists(configPath)) continue;
                    AppConfig? settings = null;
                    if (configId == 0)
                    {
                        using var configStream =
                            new FileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        DefaultAppConfig.ReadSettings(configStream);
                        settings = DefaultAppConfig;
                    }
                    else
                    {
                        settings = new AppConfig() { FileName = Path.GetFileNameWithoutExtension(configPath),  };
                        using var configStream =
                            new FileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        settings.ReadSettings(configStream);
                    }

                    return new App(commandLine) { Settings = settings };
                }
            }

            if (createIfNotExists) return CreateApp(commandLine);
            throw new GamerunAppNotFoundException(commandLine);
        }

        var found = Apps.FindAll(x => new Regex(x.CommandLine).IsMatch(commandLine));
        if (found.Count > 0) return found[0];
        if (createIfNotExists) return CreateApp(commandLine);
        throw new GamerunAppNotFoundException(commandLine);
    }

    public static void SaveListing()
    {
        Refresh();
        using var fileStream = new FileStream(ListingPath,
            File.Exists(ListingPath) ? FileMode.Truncate : FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        using var writer = new StreamWriter(fileStream);
        foreach (var app in Apps)
        {
            // TODO: lastRun|config|strangle|mangohud|gamescope|commandline
            var configId = app.Settings is null ? 0 : Configs.IndexOf(app.Settings);
            writer.WriteLine($"{configId}|{app.CommandLine}");
        }
    }

    private static App CreateApp(string commandLine, AppConfig? settings = null)
    {
        var app = new App(commandLine) { Settings = settings };
        SaveListing();
        return app;
    }

    public static void Init(bool full = false)
    {
        // "monke work smart no dumb" future-proof solution to read default configs and create folders.
        foreach (var folder in new Dictionary<string, GamerunSettingsAbstract?>
                 {
                     { AppPath, null },
                     { ConfigsPath, DefaultAppConfig },
                     { MangoHudConfigsPath, DefaultMangoHUDConfig },
                     { StrangleConfigsPath, DefaultStrangleConfig },
                     { GamescopeConfigsPath, DefaultGamescopeConfig }
                     // Template: { Configs Path, Default Config }
                 })
        {
            if (!Directory.Exists(folder.Key)) Directory.CreateDirectory(folder.Key);
            if (folder.Value == null) continue;
            folder.Value.SetAsDefault();
            using var configStream =
                new FileStream(Path.Combine(folder.Key, "0"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            folder.Value.ReadSettings(configStream);
        }

        if (full) Refresh();
    }

    public static void Refresh()
    {
        // Refresh configs first then apps.
        RefreshConfigs();
        RefreshMangoHudConfigs();
        RefreshStrangleConfigs();
        RefreshGamescopeConfigs();
        RefreshApps();
    }

    #region REFRESH PRIVATES

    private static void RefreshApps()
    {
        var files = Directory.GetFiles(ListingPath);
        foreach (var file in files)
        {
            using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            // TODO: read file
            // lastRun|config|strangle|mangohud|gamescope|commandline
            
        }
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
                found.ForEach(x => x.ReadSettings(fileStream));
                continue;
            }
            var newConfig = new AppConfig() { FileName = fileName };
            newConfig.ReadSettings(fileStream);
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
                found.ForEach(x => x.ReadSettings(fileStream));
                continue;
            }
            var newConfig = new MangoHudConfig() {FileName = fileName};
            newConfig.ReadSettings(fileStream);
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
                found.ForEach(x => x.ReadSettings(fileStream));
                continue;
            }
            var newConfig = new StrangleConfig() {FileName = fileName};
            newConfig.ReadSettings(fileStream);
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
                found.ForEach(x => x.ReadSettings(fileStream));
                continue;
            }
            var newConfig = new GamescopeConfig() {FileName = fileName};
            newConfig.ReadSettings(fileStream);
            GamescopeConfigs.Add(newConfig);
        }
    }

    #endregion REFRESH PRIVATES
}