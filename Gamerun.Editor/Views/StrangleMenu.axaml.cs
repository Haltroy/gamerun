using Gamerun.Editor.Models;

namespace Gamerun.Editor.Views;

public partial class StrangleMenu : GamerunUserControl
{
    public StrangleMenu()
    {
        InitializeComponent();
    }

    public override GamerunUserControlType Type => GamerunUserControlType.Strangle;

    public override void ReceivedCommand(object? command)
    {
        // TODO: string "refresh" refreshes it, StrangleConfig should open settings for that item
    }
}