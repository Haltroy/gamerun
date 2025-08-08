using Gamerun.Editor.Models;

namespace Gamerun.Editor.Views;

public partial class GamescopeMenu : GamerunUserControl
{
    public GamescopeMenu()
    {
        InitializeComponent();
    }

    public override GamerunUserControlType Type => GamerunUserControlType.Gamescope;

    public override void ReceivedCommand(object? command)
    {
        // TODO: string "refresh" refreshes list, GamescopeSettings shoudl open edit menu for that config
    }
}