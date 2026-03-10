using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using SkeletonLabNote.Models;
using SkeletonLabNote.ViewModels;

namespace SkeletonLabNote;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }

    private void TextBox_OnGotFocus(object? sender, GotFocusEventArgs e)
    {
        if (sender is TextBox textBox && textBox.FindAncestorOfType<ListBox>() is {} listBox)
        {
            listBox.SelectedItem = textBox.DataContext;
        }
    }
}