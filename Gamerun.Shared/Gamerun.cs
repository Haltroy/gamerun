using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Gamerun.Shared
{
// TODO: Gamescope settings

// TODO: Notification system settings

// TODO: Compositor Settings
    public static class Gamerun
    {
        public static AppSettings Default = new AppSettings();
        public static Dictionary<uint, App> Apps = new Dictionary<uint, App>();

        private static string GlobalConfigPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "gamerun");

        private static string UserConfigPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "gamerun");

        public static string GlobalListingPath => Path.Combine(GlobalConfigPath, "listing");
        public static string UserListingPath => Path.Combine(UserConfigPath, "listing");

        public static string GlobalSettingsPath => Path.Combine(GlobalConfigPath, "apps");
        public static string UserSettingsPath => Path.Combine(UserConfigPath, "apps");

        public static uint? GetAppID(string commandLine)
        {
            var listings = new[] { GlobalListingPath, UserListingPath };
            foreach (var listing in listings)
            {
                if (!File.Exists(listing)) continue;
                using var stream = new FileStream(listing, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(stream);
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var split = line.Split('|');
                    if (split.Length != 2) continue;
                    var appID = split[0];
                    var appName = split[1];
                    if (new Regex(appName).IsMatch(appName) && uint.TryParse(appID, out var id)) return id;
                }
            }

            return null;
        }

        public static App GetApp(string commandLine, bool createIfNotExists = false)
        {
            var id = GetAppID(commandLine);
            if (id == null)
            {
                if (createIfNotExists) return CreateApp(commandLine);

                throw new Exception(); // TODO
            }

            var userFile = Path.Combine(UserSettingsPath, id.ToString());
            var globalFile = Path.Combine(GlobalSettingsPath, id.ToString());
            var userExist = File.Exists(userFile);
            var globalExist = File.Exists(globalFile);
            if (userExist) return new App((uint)id, true, globalExist);

            if (globalExist) return new App((uint)id, false, true);

            if (createIfNotExists) return CreateApp(commandLine, id);
            throw new Exception(); // TODO
        }

        private static App CreateApp(string commandLine, uint? id = null)
        {
            if (id == null)
            {
                var random = new Random();
                var breakRandomGen = false;
                while (!breakRandomGen)
                {
                    var newID = random.Next(0, 100000000); // max 8 digit
                    if (File.Exists(Path.Combine(GlobalSettingsPath, newID.ToString())) ||
                        File.Exists(Path.Combine(UserSettingsPath, newID.ToString()))) continue;
                    id = (uint)newID;
                    breakRandomGen = true;
                }
            }

            var app = new App(commandLine);
            app.ID = (uint)id!;
            // TODO: Save method here
            return app;
        }

        public static void Init()
        {
            // TODO
            Refresh();
        }

        public static void Refresh()
        {
            // TODO
        }
    }
}