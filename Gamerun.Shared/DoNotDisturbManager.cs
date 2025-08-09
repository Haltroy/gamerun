using System;

namespace Gamerun.Shared;

/// <summary>
///     Class used for detecting the notification manager and making commands to tell it to enable Do Not Disturb mode.
/// </summary>
public static class DoNotDisturbManager
{
    #region FUNCTIONS

    /// <summary>
    ///     Generates arguments for the current running notification system.
    /// </summary>
    /// <param name="args">Arguments class to add commands & DBus calls to.</param>
    /// <returns><see cref="GamerunStartArguments" /> which is <paramref name="args" /> but added commands.</returns>
    public static GamerunStartArguments GenerateArgs(GamerunStartArguments args)
    {
        switch (DetectNotifier())
        {
            case NotifierType.GNOME:
                args.StartCommands.Add(
                    $"{Tools.GetCommand("gdbus")} call --session --dest org.gnome.SessionManager --object-path /org/gnome/SessionManager/Presence --method org.gnome.SessionManager.Presence.SetStatus 3");
                args.EndCommands.Add(
                    $"{Tools.GetCommand("gdbus")} call --session --dest org.gnome.SessionManager --object-path /org/gnome/SessionManager/Presence --method org.gnome.SessionManager.Presence.SetStatus 0");
                break;
            case NotifierType.KDE:
                args.StartCommands.Add(
                    $"{Tools.GetCommand("qdbus")} org.kde.NotificationManager /NotificationManager org.kde.NotificationManager.setInhibited true");
                args.EndCommands.Add(
                    $"{Tools.GetCommand("qdbus")} org.kde.NotificationManager /NotificationManager org.kde.NotificationManager.setInhibited false");
                break;
            case NotifierType.XFCE:
                args.StartCommands.Add(
                    $"{Tools.GetCommand("xfconf-query")} -c xfce4-notifyd -p /do-not-disturb -s true");
                args.EndCommands.Add(
                    $"{Tools.GetCommand("xfconf-query")} -c xfce4-notifyd -p /do-not-disturb -s false");
                break;
            case NotifierType.MATE:
                args.StartCommands.Add(
                    $"{Tools.GetCommand("dconf")} write /org/mate/notifications/notify-osd/do-not-disturb true");
                args.EndCommands.Add(
                    $"{Tools.GetCommand("dconf")} write /org/mate/notifications/notify-osd/do-not-disturb false");
                break;
            case NotifierType.Cinnamon:
                args.StartCommands.Add(
                    $"{Tools.GetCommand("gsettings")} set org.cinnamon.desktop.notifications show-banners false");
                args.EndCommands.Add(
                    $"{Tools.GetCommand("gsettings")} set org.cinnamon.desktop.notifications show-banners true");
                break;
            case NotifierType.Dunst:
                args.StartCommands.Add($"{Tools.GetCommand("dunstctl")} set-paused true");
                args.EndCommands.Add($"{Tools.GetCommand("dunstctl")} set-paused false");
                break;
            case NotifierType.Swaync:
                args.StartCommands.Add($"{Tools.GetCommand("swaync-client")} --toggle-dnd on");
                args.EndCommands.Add($"{Tools.GetCommand("swaync-client")} --toggle-dnd off");
                break;
            case NotifierType.Budgie:
                args.StartDBusCalls.Add(new GamerunDBusCalls("org.buddiesofbudgie.Budgie",
                    "/org/buddiesofbudgie/Budgie", "org.buddiesofbudgie.Budgie.SetDoNotDisturb",
                    ["boolean:true"]));
                args.EndDBusCalls.Add(new GamerunDBusCalls("org.buddiesofbudgie.Budgie", "/org/buddiesofbudgie/Budgie",
                    "org.buddiesofbudgie.Budgie.SetDoNotDisturb",
                    ["boolean:false"]));
                break;
            case NotifierType.Mako:
                args.StartCommands.Add($"{Tools.GetCommand("makoctl")} set-mode do-not-disturb");
                args.EndCommands.Add($"{Tools.GetCommand("makoctl")} set-mode default");
                break;
            case NotifierType.Pantheon:
                args.StartDBusCalls.Add(new GamerunDBusCalls("io.elementary.notifications",
                    "/io/elementary/notifications", "io.elementary.notifications.SetDoNotDisturb",
                    ["boolean:true"]));
                args.EndDBusCalls.Add(new GamerunDBusCalls("io.elementary.notifications",
                    "/io/elementary/notifications", "io.elementary.notifications.SetDoNotDisturb",
                    ["boolean:false"]));
                break;
            case NotifierType.LXQt:
                args.StartDBusCalls.Add(new GamerunDBusCalls("org.lxqt.notificationd", "/org/lxqt/notificationd",
                    "org.lxqt.notificationd.SetDoNotDisturb",
                    ["boolean:true"]));
                args.EndDBusCalls.Add(new GamerunDBusCalls("org.lxqt.notificationd", "/org/lxqt/notificationd",
                    "org.lxqt.notificationd.SetDoNotDisturb",
                    ["boolean:false"]));
                break;
        }

        return args;
    }

    #endregion FUNCTIONS

    #region PRIVATES

    private static NotifierType DetectNotifier()
    {
        var desktop = (Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP") ?? "").ToLower();
        var session = (Environment.GetEnvironmentVariable("DESKTOP_SESSION") ?? "").ToLower();

        if (desktop.Contains("gnome") || session.Contains("gnome")) return NotifierType.GNOME;
        if (desktop.Contains("kde") || desktop.Contains("plasma") || session.Contains("kde") ||
            session.Contains("plasma")) return NotifierType.KDE;
        if (desktop.Contains("xfce") || session.Contains("xfce")) return NotifierType.XFCE;
        if (desktop.Contains("mate") || session.Contains("mate")) return NotifierType.MATE;
        if (desktop.Contains("cinnamon") || session.Contains("cinnamon")) return NotifierType.Cinnamon;
        if (desktop.Contains("pantheon") || session.Contains("pantheon")) return NotifierType.Pantheon;
        if (desktop.Contains("budgie") || session.Contains("budgie")) return NotifierType.Budgie;
        if (desktop.Contains("lxqt") || session.Contains("lxqt")) return NotifierType.LXQt;

        if (Tools.ProcessExists("dunst")) return NotifierType.Dunst;
        if (Tools.ProcessExists("swaync")) return NotifierType.Swaync;
        return Tools.ProcessExists("mako") ? NotifierType.Mako : NotifierType.None;
    }

    private enum NotifierType
    {
        None,
        GNOME,
        KDE,
        XFCE,
        MATE,
        Cinnamon,
        Dunst,
        Swaync,
        Mako,
        Pantheon,
        Budgie,
        LXQt
    }

    #endregion PRIVATES
}