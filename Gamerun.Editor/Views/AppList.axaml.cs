using Gamerun.Editor.Models;

namespace Gamerun.Editor.Views;

public partial class AppList : GamerunUserControl
{
    public AppList()
    {
        InitializeComponent();
    }

    public override GamerunUserControlType Type => GamerunUserControlType.Applications;

    public override void ReceivedCommand(object? command)
    {
        // TODO: refresh app list
    }
}