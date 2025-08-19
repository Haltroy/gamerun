using Gamerun.Editor.Models;

namespace Gamerun.Editor.Views;

// Future TODO: Make configs in this menu, but still let users use the default $HOME/.config/MangoHud/MangoHud.conf or another config.
public partial class MangoHUDMenu : GamerunUserControl
{
    public MangoHUDMenu()
    {
        InitializeComponent();
    }

    public override GamerunUserControlType Type => GamerunUserControlType.MangoHUD;

    public override void ReceivedCommand(object? command)
    {
        // TODO: string "refresh" refreshes list, MangoHudConfig opens the edit menu for that page
    }
}