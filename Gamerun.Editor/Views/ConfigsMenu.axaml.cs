using Gamerun.Editor.Models;

namespace Gamerun.Editor.Views;

public partial class ConfigsMenu : GamerunUserControl
{
    public ConfigsMenu()
    {
        InitializeComponent();
    }

    public override GamerunUserControlType Type => GamerunUserControlType.Configs;

    public override void ReceivedCommand(object? command)
    {
        // TODO: string "refresh" refreshes it, AppConfig opens edit menu for that item
    }
}