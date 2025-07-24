using Avalonia;
using Avalonia.Controls;

namespace Gamerun.Editor.Views;

public partial class AppSettingsView : UserControl
{
    public static readonly StyledProperty<Shared.App> GamerunAppProperty =
        AvaloniaProperty.Register<AppSettingsView, Shared.App>(nameof(GamerunApp));

    public AppSettingsView()
    {
        InitializeComponent();
    }

    public Shared.App GamerunApp
    {
        get => GetValue(GamerunAppProperty);
        set => SetValue(GamerunAppProperty, value);
    }
}