using Avalonia.Controls;
using Gamerun.Editor.Models;

namespace Gamerun.Editor.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";

    public Dock ActionMenuPos => CrossDEButtonLayoutDetector.AreButtonsOnLeft() ? Dock.Left : Dock.Right;
}