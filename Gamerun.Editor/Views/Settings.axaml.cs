using Gamerun.Editor.Models;

namespace Gamerun.Editor.Views;

public partial class Settings : GamerunUserControl
{
    public Settings()
    {
        InitializeComponent();
    }

    public override GamerunUserControlType Type => GamerunUserControlType.Settings;

    public override void ReceivedCommand(object? command)
    {
        // TODO: refresh editor settings here
    }
}