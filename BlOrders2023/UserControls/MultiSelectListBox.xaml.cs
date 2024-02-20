// MultiSelectListBox.xaml.cs
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Syncfusion.UI.Xaml.Data;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace BlOrders2023.UserControls;

public sealed partial class MultiSelectListBox : UserControl
{
    public ObservableCollection<object> Items { get; set; }
    public ObservableCollection<string> SelectedItems { get; set; } = [];
    public MultiSelectListBox(ObservableCollection<object> items)
    {
        this.InitializeComponent();
        Items = items;
        SelectionTree.Loaded += SelectionTree_Loaded;
        SelectionTree.SelectionChanged += SelectionTree_SelectionChanged;
        foreach (var item in Items)
        {
            RootNode.Children.Add(new(){ Content = item });
        }
    }

    private void SelectionTree_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        foreach (TreeViewNode item in args.AddedItems) 
        {
            if(!item.HasChildren)
            {
                SelectedItems.Add(item.Content.ToString());
            }
        }
        foreach (TreeViewNode item in args.RemovedItems)
        {
            if (!item.HasChildren)
            {
                SelectedItems.Remove(item.Content.ToString());
            }
        }
    }

    private void SelectionTree_Loaded(object sender, RoutedEventArgs e)
    {
        SelectionTree.SelectAll();
    }
}
