using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Gamerun.Shared.Exceptions;

namespace Gamerun.Shared;

public static class Gamerun
{
    public static AppSettings DefaultAppConfig = new();
    public static MangoHUDSettings DefaultMangoHUDConfig = new();
    public static StrangleSettings DefaultStrangleConfig = new();
    public static GamescopeSettings DefaultGamescopeConfig = new();
    public static List<App> Apps = [];
    public static List<AppSettings> Configs = [DefaultAppConfig];
    public static List<MangoHUDSettings> MangoHUDConfigs = [DefaultMangoHUDConfig];
    public static List<StrangleSettings> StrangleConfigs = [DefaultStrangleConfig];
    public static List<GamescopeSettings> GamescopeConfigs = [DefaultGamescopeConfig];
    public static GpuInfo[] GPUs { get; set; } = Tools.GetAllGpus();

    private static string AppPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "gamerun");

    public static string ListingPath => Path.Combine(AppPath, "listing");

    public static string ConfigsPath => Path.Combine(AppPath, "configs");

    public static App GetApp(string commandLine, bool NoFullInit, bool createIfNotExists = false)
    {
        if (!File.Exists(ListingPath))
        {
            if (createIfNotExists) return CreateApp(commandLine);
            throw new GamerunAppNotFoundException(commandLine);
        }

        Init(!NoFullInit);
        if (NoFullInit)
        {
            using var fileStream = new FileStream(ListingPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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
                AppSettings? settings = null;
                if (configId == 0)
                {
                    using var configStream =
                        new FileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    DefaultAppConfig.ReadSettings(configStream);
                    settings = DefaultAppConfig;
                }
                else
                {
                    settings = new AppSettings();
                    using var configStream =
                        new FileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    settings.ReadSettings(configStream);
                }

                return new App(commandLine) { Settings = settings };
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
            var configId = app.Settings is null ? 0 : Configs.IndexOf(app.Settings);
            writer.WriteLine($"{configId}|{app.CommandLine}");
        }
    }

    private static App CreateApp(string commandLine, AppSettings? settings = null)
    {
        var app = new App(commandLine) { Settings = settings };
        SaveListing();
        return app;
    }

    public static void Init(bool full = false)
    {
        if (!Directory.Exists(AppPath)) Directory.CreateDirectory(AppPath);
        if (!Directory.Exists(ConfigsPath)) Directory.CreateDirectory(ConfigsPath);
        using var configStream =
            new FileStream(Path.Combine(ConfigsPath, "0"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        DefaultAppConfig.ReadSettings(configStream);
        if (full) Refresh();
    }

    public static void Refresh()
    {
        using var fileStream = new FileStream(ListingPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(fileStream);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;
            var lineSplit = line.Split('|');
            if (lineSplit.Length != 2) continue;
            var found = Apps.FindAll(x => new Regex(x.CommandLine).IsMatch(lineSplit[1]));
            if (found.Count > 0)
            {
                if (!int.TryParse(lineSplit[0], out var configId)) continue;
                if (configId == 0)
                {
                    found[0].Settings = DefaultAppConfig;
                }
                else
                {
                    if (Configs.ElementAtOrDefault(configId) != null) continue;
                    var configPath = Path.Combine(ConfigsPath, $"{configId}");
                    if (!File.Exists(configPath)) continue;
                    var settings = new AppSettings();
                    using var configStream =
                        new FileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    settings.ReadSettings(configStream);
                    Configs.Add(settings);
                    found[0].Settings = settings;
                }
            }
            else
            {
                if (!int.TryParse(lineSplit[0], out var configId)) continue;
                AppSettings? settings;
                if (configId == 0)
                {
                    settings = DefaultAppConfig;
                }
                else
                {
                    if (Configs.ElementAtOrDefault(configId) != null) continue;
                    var configPath = Path.Combine(ConfigsPath, $"{configId}");
                    if (!File.Exists(configPath)) continue;
                    settings = new AppSettings();
                    using var configStream =
                        new FileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    settings.ReadSettings(configStream);
                    Configs.Add(settings);
                }

                Apps.Add(new App(lineSplit[1]) { Settings = settings });
            }
        }
    }
}