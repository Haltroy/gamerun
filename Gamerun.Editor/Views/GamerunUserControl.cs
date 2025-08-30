using Avalonia;
using Avalonia.Controls;
using Gamerun.Editor.Models;

namespace Gamerun.Editor.Views;

public abstract class GamerunUserControl : UserControl
{
    public static readonly AvaloniaProperty<MainWindow?> MainWindowProperty =
        AvaloniaProperty.Register<GamerunUserControl, MainWindow?>(nameof(MainWindow));

    public MainWindow? MainWindow
    {
        get => GetValue(MainWindowProperty) as MainWindow;
        set => SetValue(MainWindowProperty, value);
    }

    public abstract GamerunUserControlType Type { get; }
    public abstract void ReceivedCommand(object? command);

    public void SendCommandTo(GamerunUserControlType type, object command, bool focus = false)
    {
        if (MainWindow is null) return;
        foreach (var control in MainWindow.ContentCarousel.Items)
            if (control is GamerunUserControl uc && uc.Type == type)
            {
                if (focus) MainWindow.ContentCarousel.SelectedItem = control;
                uc.ReceivedCommand(command);
            }
    }
}