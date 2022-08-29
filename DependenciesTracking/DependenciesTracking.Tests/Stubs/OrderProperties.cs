using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DependenciesTracking.Interfaces;

namespace DependenciesTracking.Tests.Stubs
{
    public class Order : INotifyPropertyChanged
    {
        private OrderProperties _properties;
        private string _orderLine;
        private int _totalCost;

        public OrderProperties Properties
        {
            get { return _properties; }
            set
            {
                if (_properties != value)
                {
                    _properties = value;
                    OnPropertyChanged("Properties");
                }
            }
        }

        public int TotalCost
        {
            get { return _totalCost; }
            private set
            {
                if (_totalCost != value)
                {
                    _totalCost = value;
                    OnPropertyChanged("TotalCost");
                }
            }
        }

        public string OrderLine
        {
            get { return _orderLine; }
            private set
            {
                if (_orderLine != value)
                {
                    _orderLine = value;
                    OnPropertyChanged("OrderLine");
                }

            }
        }

        public string ShortOrderLine
        {
            get { return _shortOrderLine; }
            private set
            {
                if (_shortOrderLine != value)
                {
                    _shortOrderLine = value;
                    OnPropertyChanged("ShortOrderLine");
                }
            }
        }

        private static readonly IDependenciesMap<Order> _map = new DependenciesMap<Order>();
        private IDisposable _tracker;
        private string _shortOrderLine;

        static Order()
        {
            _map.AddDependency(o => o.OrderLine, BuildOrderLine, o => o.Properties.Category, o => o.Properties.Price, o => o.Properties.Quantity)
                .AddDependency(o => o.TotalCost, o => o.Properties != null ? o.Properties.Price * o.Properties.Quantity : -1, o => o.Properties.Price, o => o.Properties.Quantity)
                .AddDependency(o => o.ShortOrderLine, o => o.TotalCost == -1 ? "ShortOrderLine is empty" : string.Format("Order has total cost = {0}", o.TotalCost), o => o.TotalCost);

        }

        public Order()
        {
            _tracker = _map.StartTracking(this);
        }

        private static string BuildOrderLine(Order order)
        {
            return order.Properties == null ? "Order is empty"
                : string.Format("Order category: {0}, price = {1}, quantity = {2}", order.Properties.Category, order.Properties.Price, order.Properties.Quantity);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class OrderProperties : INotifyPropertyChanged
    {
        private int _price;
        private int _quantity;
        private string _category;

        public string Category
        {
            get { return _category; }
            set
            {
                if (_category != value)
                {
                    _category = value;
                    OnPropertyChanged("Category");
                }
            }
        }

        public int Price
        {
            get { return _price; }
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged("Price");
                }
            }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged("Quantity");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public sealed class Invoice : INotifyPropertyChanged
    {
        private ObservableCollection<FlatOrder> _orders;
        private int _totalCost;

        public ObservableCollection<FlatOrder> Orders
        {
            get { return _orders; }
            set
            {
                if (_orders != value)
                {
                    _orders = value;
                    OnPropertyChanged("Orders");
                }

            }
        }

        public int TotalCost
        {
            get { return _totalCost; }
            set
            {
                if (_totalCost != value)
                {
                    _totalCost = value;
                    OnPropertyChanged("TotalCost");
                }
            }
        }

        private static readonly DependenciesMap<Invoice> _dependenciesMap = new DependenciesMap<Invoice>();
        private IDisposable _tracker;

        static Invoice()
        {
            _dependenciesMap.AddDependency(i => i.TotalCost, i => i.Orders == null ? -1 : i.Orders.Sum(o => o.Price * o.Quantity),
                                i => DependenciesTracking.CollectionExtensions.EachElement(i.Orders).Price,
                                i => DependenciesTracking.CollectionExtensions.EachElement(i.Orders).Quantity);
        }

        public Invoice()
        {
            _tracker = _dependenciesMap.StartTracking(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}