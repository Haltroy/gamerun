using System;
using System.Diagnostics;
using System.IO;
using Gamerun.Shared;

namespace Gamerun.Editor.Models;

public static class CrossDEButtonLayoutDetector
{
    public static bool AreButtonsOnLeft()
    {
        var de = GetDesktopEnvironment()?.ToLower() ?? "";

        if (de.Contains("gnome") || de.Contains("unity") || de.Contains("cinnamon") || de.Contains("mate") ||
            de.Contains("budgie"))
        {
            var gsettings = Tools.GetCommand("gsettings");
            if (string.IsNullOrWhiteSpace(gsettings)) return false; // Assume right side
            var layout = RunCommand(gsettings, "get org.gnome.desktop.wm.preferences button-layout");
            if (string.IsNullOrWhiteSpace(layout)) return false;
            return IsLeftFromColon(layout);
        }

        if (de.Contains("xfce"))
        {
            var xfconf = Tools.GetCommand("xfconf-query");
            if (string.IsNullOrWhiteSpace(xfconf)) return false; // Assume right side
            var layout = RunCommand(xfconf, "-c xfwm4 -p /general/button_layout");
            if (string.IsNullOrWhiteSpace(layout)) return false;
            return layout.StartsWith("close") || layout.Contains("close,"); // XFCE puts left first
        }

        if (de.Contains("kde") || de.Contains("plasma"))
        {
            var kwinrc = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".config",
                "kwinrc");
            if (!File.Exists(kwinrc)) return false; // Assume right side
            foreach (var line in File.ReadAllLines(kwinrc))
            {
                if (!line.StartsWith("ButtonsOnRight=", StringComparison.OrdinalIgnoreCase)) continue;
                var val = line.Split('=')[1].Trim();
                // KDE lists buttons on right; if close is missing, they're on left
                return !val.Contains("Close");
            }
        }

        // Default fallback
        return false; // Assume right side
    }

    private static string? GetDesktopEnvironment()
    {
        var de = Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP");
        if (!string.IsNullOrEmpty(de))
            return de;

        de = Environment.GetEnvironmentVariable("DESKTOP_SESSION");
        return !string.IsNullOrEmpty(de) ? de : null;
    }

    private static bool IsLeftFromColon(string layout)
    {
        if (string.IsNullOrEmpty(layout))
            return false;

        layout = layout.Trim('\'', '"', ' ', '\n', '\r');
        var parts = layout.Split(':');
        return parts.Length > 0 && !string.IsNullOrEmpty(parts[0]);
    }

    private static string? RunCommand(string exePath, string args)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = args,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process != null)
            {
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output.Trim();
            }

            return string.Empty;
        }
        catch
        {
            return null;
        }
    }
}