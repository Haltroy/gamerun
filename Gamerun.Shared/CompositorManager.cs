using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gamerun.Shared;

/// <summary>
///     Helper class for optimizing compositors for performance.
/// </summary>
public static class CompositorManager
{
    /// <summary>
    ///     Adds commands to optimize (and revert back) compositors for performance.
    /// </summary>
    /// <param name="args">Arguments to add commands and DBus calls to.</param>
    /// <returns><see cref="GamerunStartArguments" /> which is <paramref name="args" /> with added commands and DBus calls.</returns>
    public static GamerunStartArguments GenerateArgs(GamerunStartArguments args)
    {
        var current = GetCurrentCompositor();

        foreach (var compositor in current)
            switch (compositor)
            {
                case Compositor.KWin:
                    args.StartCommands.Add($"{Tools.GetCommand("qdbus")} org.kde.KWin /Compositor suspend");
                    args.EndCommands.Add($"{Tools.GetCommand("qdbus")} org.kde.KWin /Compositor resume");
                    break;

                case Compositor.Mutter:
                    args.StartCommands.Add(
                        $"{Tools.GetCommand("gsettings")} set org.gnome.desktop.interface enable-animations false");
                    args.EndCommands.Add(
                        $"{Tools.GetCommand("gsettings")} set org.gnome.desktop.interface enable-animations true");
                    break;

                case Compositor.Hyprland:
                    args.StartCommands.Add($"{Tools.GetCommand("hyprctl")} --batch " +
                                           $"keyword animations:enabled 0; " +
                                           $"keyword decoration:drop_shadow 0; " +
                                           $"keyword decoration:blur:enabled 0; " +
                                           $"keyword general:gaps_in 0; " +
                                           $"keyword general:gaps_out 0; " +
                                           $"keyword general:border_size 1; " +
                                           $"keyword decoration:rounding 0");
                    args.EndCommands.Add($"{Tools.GetCommand("hyprctl")} reload");
                    break;

                case Compositor.Sway:
                    args.StartCommands.Add($"{Tools.GetCommand("swaymsg")} 'gaps inner 0; gaps outer 0'");
                    args.EndCommands.Add($"{Tools.GetCommand("swaymsg")} 'reload'");
                    break;

                case Compositor.Picom:
                    args.StartCommands.Add($"{Tools.GetCommand("pkill")} -STOP picom");
                    args.EndCommands.Add($"{Tools.GetCommand("pkill")} -CONT picom");
                    break;

                case Compositor.XFWM4:
                    args.StartCommands.Add(
                        $"{Tools.GetCommand("xfconf-query")} -c xfwm4 -p /general/use_compositing -s false");
                    args.EndCommands.Add(
                        $"{Tools.GetCommand("xfconf-query")} -c xfwm4 -p /general/use_compositing -s true");
                    break;

                case Compositor.Marco:
                    args.StartCommands.Add(
                        $"{Tools.GetCommand("gsettings")} set org.mate.Marco.general compositing-manager false");
                    args.EndCommands.Add(
                        $"{Tools.GetCommand("gsettings")} set org.mate.Marco.general compositing-manager true");
                    break;

                case Compositor.Compiz:
                    // Usually you can just kill Compiz temporarily
                    args.StartCommands.Add($"{Tools.GetCommand("pkill")} -STOP compiz");
                    args.EndCommands.Add($"{Tools.GetCommand("pkill")} -CONT compiz");
                    break;

                case Compositor.None:
                default:
                    // Nothing to do
                    break;
            }

        return args;
    }

    #region PRIVATE

    private static Compositor[] GetCurrentCompositor()
    {
        var detected = new List<Compositor>();

        var desktop = Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP")?.ToLowerInvariant() ?? "";
        var session = Environment.GetEnvironmentVariable("DESKTOP_SESSION")?.ToLowerInvariant() ?? "";

        var processes = Process.GetProcesses();
        foreach (var proc in processes)
            try
            {
                var name = proc.ProcessName.ToLowerInvariant();

                if ((name.Contains("kwin") && !detected.Contains(Compositor.KWin)) || desktop.Contains("kde") ||
                    session.Contains("kde") || desktop.Contains("plasma") || session.Contains("plasma"))
                    detected.Add(Compositor.KWin);

                if ((name.Contains("gnome-shell") && !detected.Contains(Compositor.Mutter)) ||
                    desktop.Contains("gnome") || session.Contains("gnome"))
                    detected.Add(Compositor.Mutter);

                if (name.Contains("hyprland") && !detected.Contains(Compositor.Hyprland))
                    detected.Add(Compositor.Hyprland);

                if (name == "sway" && !detected.Contains(Compositor.Sway))
                    detected.Add(Compositor.Sway);

                if (name.Contains("picom") && !detected.Contains(Compositor.Picom))
                    detected.Add(Compositor.Picom);

                if ((name.Contains("xfwm4") && !detected.Contains(Compositor.XFWM4)) || desktop.Contains("xfce") ||
                    session.Contains("xfce"))
                    detected.Add(Compositor.XFWM4);

                if ((name.Contains("marco") && !detected.Contains(Compositor.Marco)) || desktop.Contains("mate") ||
                    session.Contains("mate"))
                    detected.Add(Compositor.Marco);

                if (name.Contains("compiz") && !detected.Contains(Compositor.Compiz))
                    detected.Add(Compositor.Compiz);
            }
            catch
            {
                // Ignore processes we can't access
            }

        return detected.ToArray();
    }

    private enum Compositor
    {
        None,
        KWin,
        Mutter,
        Hyprland,
        Sway,
        Picom,
        XFWM4,
        Marco,
        Compiz
    }

    #endregion PRIVATE
}