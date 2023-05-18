using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.IdentityModel.Tokens;
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
        /// Gets a list of _products for the auto suggest box
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
                if (value.IsNullOrEmpty())
                {
                    _order.PO_Number = null;
                }
                else
                {
                    _order.PO_Number = value;
                }
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
                if (value.IsNullOrEmpty())
                {
                    _order.Memo = null;
                }
                else
                {
                    _order.Memo = value;
                }
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

        public bool HasNextOrder { get; set; } = false;
        public bool HasPreviousOrder { get; set; } = false;
        #endregion Properties

        #region Fields
        private Order _order;
        private ObservableCollection<Product> _suggestedProducts;
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private ObservableCollection<OrderItem> _items;
        private int _currentOrderIndex;
        private IBLDatabase _db = App.GetNewDatabase();
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Creates a new instance of OrderDetailsPageViewModel
        /// </summary>
        public OrderDetailsPageViewModel()
        {
            _currentOrderIndex = 0;
            _order = new();
            _suggestedProducts = new();
            _items= new();
            LoadProducts();
        }
        #endregion Constructors

        #region Methods

        public bool OrderItemsContains(int id)
        {
            return (_items.FirstOrDefault(i => i.ProductID == id, null) != null);
        }
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
                var entityOrder = _db.Orders.Get(order.OrderID).FirstOrDefault();
                if (entityOrder != null)
                {
                    //We want to track changes so get it from the db context
                    _order = entityOrder;
                }
                else
                {
                    //Must be a new Order
                    _order = order;
                }
                _items = new ObservableCollection<OrderItem>(_order.Items);
                _currentOrderIndex = _order.Customer.orders.OrderBy(o => o.OrderID).ToList().IndexOf(_order);
                HasNextOrder = _currentOrderIndex < _order.Customer.orders.Count - 1;
                HasPreviousOrder = _currentOrderIndex > 0;
                OnAllPropertiesChanged();
            }
        }

        public void OnNavigatedFrom()
        {
            
        }

        public void addItem(Product p)
        {
            var tracked = _db.Products.Get(p.ProductID, false).First();
            OrderItem item = new(tracked,_order);
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
            OnPropertyChanged(nameof(HasNextOrder));
            OnPropertyChanged(nameof(HasPreviousOrder));
        }

        public int? GetNextOrderID()
        {
            if (HasNextOrder)
            {
                return _order.Customer.orders.OrderBy(o => o.OrderID).ToList()[_currentOrderIndex + 1].OrderID;
            }
            return null;
        }
        public Order? GetNextOrder()
        {
            if (HasNextOrder)
            {
                return _order.Customer.orders.OrderBy(o => o.OrderID).ToList()[_currentOrderIndex + 1];
            }
            return null;
        }

        public int? GetPreviousOrderID()
        {
            if (HasPreviousOrder)
            {
                return _order.Customer.orders.OrderBy(o => o.OrderID).ToList()[_currentOrderIndex - 1].OrderID;
            }
            return null;
        }

        public Order? GetPreviousOrder()
        {
            if (HasPreviousOrder)
            {
                return _order.Customer.orders.OrderBy(o => o.OrderID).ToList()[_currentOrderIndex - 1];
            }
            return null;
        }

        #region Queries
        /// <summary>
        /// Queries the database for a list of all _products and adds them to SuggestedProducts
        /// </summary>
        public async void LoadProducts()
        {
            await dispatcherQueue.EnqueueAsync(() =>
            {
                SuggestedProducts.Clear();
            });

            IProductsTable table = _db.Products;
            var products = await Task.Run(() => table.GetAsync((int?)null,false));

            await dispatcherQueue.EnqueueAsync(() =>
            {
                foreach (var product in products)
                {
                    SuggestedProducts.Add(product);
                }
            });
        }

        /// <summary>
        /// Queries the database for a list of _products that match the given string
        /// </summary>
        /// <param name="query">A string for _products to match</param>
        public async void QueryProducts(string query)
        {
            await dispatcherQueue.EnqueueAsync(() =>
            {
                SuggestedProducts.Clear();
            });

            IProductsTable table = _db.Products;
            var products = await Task.Run(() => table.GetAsync(query, false));

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
        public async Task SaveCurrentOrderAsync()
        {
            _order.Items = Items.ToList();
            await _db.Orders.UpsertAsync(_order);
        }

        /// <summary>
        /// Saves changes to the current Order
        /// </summary>
        public void SaveCurrentOrder(bool overwrite=false)
        {
            _order.Items = Items.ToList();
            _db.Orders.Upsert(_order, overwrite);
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

        internal async Task DeleteCurrentOrderAsync()
        {
            await _db.Orders.DeleteAsync(_order);
        }

        internal void DeleteCurrentOrder()
        {
             _db.Orders.Delete(_order);
        }
        #endregion Queries
        #endregion Methods
    }
}
