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

    private void TopLeftPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginResizeDrag(WindowEdge.NorthWest,e);
    }

    private void TopRightPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginResizeDrag(WindowEdge.NorthEast,e);
    }

    private void TopPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginResizeDrag(WindowEdge.North,e);
    }

    private void BottomLeftPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginResizeDrag(WindowEdge.SouthWest,e);
    }

    private void BottomRightPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginResizeDrag(WindowEdge.SouthEast,e);
    }

    private void BottomPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginResizeDrag(WindowEdge.South,e);
    }

    private void LeftPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginResizeDrag(WindowEdge.West,e);
    }

    private void RightPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginResizeDrag(WindowEdge.East,e);
    }
}