using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using ServiceStack.DataAnnotations;
using Syncfusion.UI.Xaml.Data;
using Syncfusion.UI.Xaml.DataGrid;
using Windows.Security.Authentication.Web.Core;

namespace BlOrders2023.UserControls.ViewModels;

public class BoxGridEditorViewModel : ObservableValidator, IGridEditorViewModel
{
    #region Fields
    private ObservableCollection<Box> items = new();
    private bool _canSave = false;
    private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    #endregion Fields
    #region Properties
    public ObservableCollection<object> Items
    {
        get => items.ToObservableCollection<object>();
        set => items = value.Cast<Box>().ToObservableCollection<Box>();
    }
    public Type ItemSourceType { get; set; }
    public bool CanSave
    {
        get => _canSave;
        set => SetProperty(ref _canSave, value);
    }
    #endregion Properties
    #region Constructors
    public BoxGridEditorViewModel()
    {
        _ = QueryBoxesAsync();
    }
    #endregion Constructors
    #region Methods
    public void ValidateItems(object sender, CurrentCellValidatingEventArgs e)
    {
        CanSave = !CanSave;
    }

    private async Task QueryBoxesAsync()
    {
        await dispatcherQueue.EnqueueAsync(() =>
        {
            items.Clear();
        });

        IBoxTable table = App.GetNewDatabase().Boxes;

        var boxes = await Task.Run(() => table.GetAsync());
        await dispatcherQueue.EnqueueAsync(() =>
        {
            foreach(var item in boxes)
            {
                items.Add(item);
            }
            OnPropertyChanged(nameof(Items));
        });
    }

    public void Save(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if(CanSave)
        {
            Debug.WriteLine("Saved Boxes");
        }
    }
    #endregion Methods
}
