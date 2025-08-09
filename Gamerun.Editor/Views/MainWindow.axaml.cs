using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Gamerun.Editor.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }


    private void Minimize(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeNormal(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        ;
    }

    private void Close(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void MoveTitleBar(object? sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }

    private void SidebarButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: Control c } || !ContentCarousel.Items.Contains(c)) return;
        ContentCarousel.SelectedItem = c;
    }
}