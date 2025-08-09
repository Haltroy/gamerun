using Gamerun.Editor.Models;

namespace Gamerun.Editor.Views;

public partial class AboutMenu : GamerunUserControl
{
    public AboutMenu()
    {
        InitializeComponent();
    }

    public override GamerunUserControlType Type => GamerunUserControlType.About;

    public override void ReceivedCommand(object? command)
    {
        // does nothing 
    }
}