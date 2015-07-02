using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using DependenciesTracking.Interfaces;

namespace DependenciesTracking.QuickStartSample
{
    public class Invoice : INotifyPropertyChanged
    {
        private readonly ObservableCollection<Order> _orders = new ObservableCollection<Order>();

        private decimal _totalCost;

        private int _inputPrice;

        private int _inputQuantity;

        private readonly DelegateCommand _addNewOrderCommand = new DelegateCommand(iObj =>
        {
            var invoice = ((Invoice)iObj);
            invoice.Orders.Add(new Order { Quantity = invoice.InputQuantity, Price = invoice.InputPrice });
        });

        private readonly DelegateCommand _deleteSelectedOrderCommand = new DelegateCommand(iObj =>
        {
            var invoice = ((Invoice)iObj);
            invoice.Orders.Remove(invoice.SelectedOrder);

        });

        private Order _selectedOrder;

        public ObservableCollection<Order> Orders
        {
            get { return _orders; }
        }

        public decimal TotalCost
        {
            get { return _totalCost; }
            set
            {
                if (_totalCost == value) return;
                _totalCost = value;
                OnPropertyChanged();
            }
        }

        public int InputPrice
        {
            get { return _inputPrice; }
            set
            {
                if (_inputPrice == value) return;
                _inputPrice = value;
                OnPropertyChanged();
            }
        }

        public int InputQuantity
        {
            get { return _inputQuantity; }
            set
            {
                if (_inputQuantity == value) return;
                _inputQuantity = value;
                OnPropertyChanged();
            }
        }

        public Order SelectedOrder
        {
            get { return _selectedOrder; }
            set
            {
                if (_selectedOrder == value) return;
                _selectedOrder = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand AddNewOrderCommand
        {
            get { return _addNewOrderCommand; }
        }

        public DelegateCommand DeleteSelectedOrderCommand
        {
            get { return _deleteSelectedOrderCommand; }
        }

        private static readonly IDependenciesMap<Invoice> _dependenciesMap = new DependenciesMap<Invoice>();

        static Invoice()
        {
            _dependenciesMap.AddDependency(i => i.TotalCost, i => i.Orders.Sum(o => o.Price * o.Quantity),
                                           i => i.Orders.EachElement().Price, i => i.Orders.EachElement().Quantity);
        }

        public Invoice()
        {
            _dependenciesMap.StartTracking(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}