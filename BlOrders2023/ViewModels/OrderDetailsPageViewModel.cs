using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BlOrders2023.ViewModels
{
    public class OrderDetailsPageViewModel : ObservableRecipient, INavigationAware
    {
        #region Properties
        ///// <summary>
        ///// Gets or sets the current Order
        ///// </summary>
        //public Order CurrentOrder
        //{
        //    get => _order;
        //    set
        //    {
        //        SetProperty(ref _order, value);
        //        OnPropertyChanged(nameof(CurrentOrder));
        //    }
        //}

        public ObservableCollection<OrderItem> Items
        {
            get => _items;
            set
            {
                if(_items != value) 
                {
                    value.CollectionChanged += LineItems_Changed;
                }

                if (_items != null)
                {
                    _items.CollectionChanged -= LineItems_Changed;
                }

                _items = value;
                OnPropertyChanged();
            }

        }

        /// <summary>
        /// Gets a list of products for the auto suggest box
        /// </summary>
        public ObservableCollection<Product> SuggestedProducts
        {
            get => _suggestedProducts;
        }

        public int OrderID
        {
            get => _order.OrderID;
            set
            {
                _order.OrderID = value;
                OnPropertyChanged();
            }
        }

        public WholesaleCustomer Customer
        {
            get => _order.Customer;
            private set
            {
                _order.Customer = value;
            }
        }

        public OrderStatus OrderStatus
        {
            get => _order.OrderStatus;
            set
            {
                _order.OrderStatus = value;
                OnPropertyChanged();
            }
        }

        public string? PO_Number
        {
            get => _order.PO_Number;
            set
            {
                _order.PO_Number = value;
                OnPropertyChanged();
            }
        }

        public ShippingType Shipping
        {
            get => _order.Shipping; 
            set
            {
                _order.Shipping = value;
                OnPropertyChanged();
            }
        }

        public string TakenBy
        {
            get => _order.TakenBy;
            set
            {
                _order.TakenBy = value;
                OnPropertyChanged();
            }
        }

        public DateTime PickupDate
        {
            get => _order.PickupDate;
            set
            {
                _order.PickupDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime PickupTime
        {
            get => _order.PickupTime;
            set
            {
                _order.PickupTime = value;
                OnPropertyChanged();
            }
        }

        public bool? Frozen
        {
            get => _order.Frozen;
            set
            {
                _order.Frozen = value;
                OnPropertyChanged();
            }
        }

        public string? Memo
        {
            get => _order.Memo;
            set
            {
                _order.Memo = value;
                OnPropertyChanged();
            }
        }

        public decimal? Memo_Totl
        {
            get => _order.Memo_Totl;
            set
            {
                _order.Memo_Totl = value;
                OnPropertyChanged();
            }
        }

        public float? Memo_Weight
        {
            get => _order.Memo_Weight;
            set
            {
                _order.Memo_Weight = value;
                OnPropertyChanged();
            }
        }


        #endregion Properties

        #region Fields
        private Order _order;
        private ObservableCollection<Product> _suggestedProducts;
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private ObservableCollection<OrderItem> _items;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Creates a new instance of OrderDetailsPageViewModel
        /// </summary>
        public OrderDetailsPageViewModel()
        {
            _order = new();
            _suggestedProducts = new();
            _items= new();
            LoadProducts();
        }
        #endregion Constructors

        #region Methods

        /// <summary>
        /// Notifies anyone listening to this object that a line item changed. 
        /// </summary>
        private void LineItems_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Items));
        }

        public void OnNavigatedTo(object parameter)
        {
            var order = parameter as Order;
            if (order != null)
            {
                _order = order;
                _items = new ObservableCollection<OrderItem>(_order.Items);
                OnAllPropertiesChanged();
            }
        }

        public void OnNavigatedFrom()
        {
            
        }

        public void addItem(Product p)
        {
            OrderItem item = new OrderItem(p, _order);
            Items.Add(item);
        }

        private void OnAllPropertiesChanged()
        {
            OnPropertyChanged(nameof(Items));
            OnPropertyChanged(nameof(OrderID));
            OnPropertyChanged(nameof(Customer));
            OnPropertyChanged(nameof(OrderStatus));
            OnPropertyChanged(nameof(PO_Number));
            OnPropertyChanged(nameof(Shipping));
            OnPropertyChanged(nameof(TakenBy));
            OnPropertyChanged(nameof(PickupDate));
            OnPropertyChanged(nameof(PickupTime));
            OnPropertyChanged(nameof(Frozen));
            OnPropertyChanged(nameof(Memo));
            OnPropertyChanged(nameof(Memo_Totl));
            OnPropertyChanged(nameof(Items));
        }

        #region Queries
        /// <summary>
        /// Queries the database for a list of all products and adds them to SuggestedProducts
        /// </summary>
        public async void LoadProducts()
        {
            await dispatcherQueue.EnqueueAsync(() =>
            {
                SuggestedProducts.Clear();
            });

            IProductsTable table = App.BLDatabase.Products;
            var products = await Task.Run(table.GetAsync);

            await dispatcherQueue.EnqueueAsync(() =>
            {
                foreach (var product in products)
                {
                    SuggestedProducts.Add(product);
                }
            });
        }

        /// <summary>
        /// Queries the database for a list of products that match the given string
        /// </summary>
        /// <param name="query">A string for products to match</param>
        public async void QueryProducts(string query)
        {
            await dispatcherQueue.EnqueueAsync(() =>
            {
                SuggestedProducts.Clear();
            });

            IProductsTable table = App.BLDatabase.Products;
            var products = await Task.Run(() => table.GetAsync(query));

            await dispatcherQueue.EnqueueAsync(() =>
            {
                foreach (var product in products)
                {
                    SuggestedProducts.Add(product);
                }
            });
        }
        /// <summary>
        /// Saves changes to the current Order
        /// </summary>
        public async void SaveCurrentOrder()
        {
            _order.Items = Items.ToList();
            await App.BLDatabase.Orders.UpsertAsync(_order);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void BeforeNavigatedFrom(object? parameter)
        {
            throw new NotImplementedException();
        }

        internal async void DeleteCurrentOrder()
        {
            await App.BLDatabase.Orders.DeleteAsync(_order);
        }
        #endregion Queries
        #endregion Methods
    }
}
