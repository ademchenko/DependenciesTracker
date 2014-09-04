using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using DependenciesTracking;
using DependenciesTracking.Interfaces;
using QuickStartSample.Annotations;

namespace QuickStartSample
{
    public class Invoice : INotifyPropertyChanged
    {
        private readonly ObservableCollection<Order> _orders = new ObservableCollection<Order>();
        private decimal _totalQuantity;
        private decimal _totalCost;
        private Order _candidateOrder = new Order();

        public decimal TotalCost
        {
            get { return _totalCost; }
            set
            {
                if (value == _totalCost) return;
                _totalCost = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalQuantity
        {
            get { return _totalQuantity; }
            set
            {
                if (value == _totalQuantity) return;
                _totalQuantity = value;
                OnPropertyChanged();
            }
        }

        public Order CandidateOrder
        {
            get { return _candidateOrder; }
            set
            {
                if (Equals(value, _candidateOrder)) return;
                _candidateOrder = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Order> Orders
        {
            get { return _orders; }
        }

        private DelegateCommand _addNewOrderCommand;

        public DelegateCommand AddNewOrderCommand
        {
            get { return _addNewOrderCommand ?? (_addNewOrderCommand = new DelegateCommand(_ => AddNewOrderImpl())); }
        }

        private void AddNewOrderImpl()
        {
            Orders.Add(CandidateOrder);
            CandidateOrder = new Order();
        }

        private DelegateCommand _deleteOrderCommand;

        public DelegateCommand DeleteOrderCommand
        {
            get { return _deleteOrderCommand ?? (_addNewOrderCommand = new DelegateCommand(DeleteOrderImpl)); }
        }

        private void DeleteOrderImpl(object parameter)
        {
            var orderToDelete = (Order) parameter;
            Orders.Remove(orderToDelete);
        }
        
        private static readonly IDependenciesMap<Invoice> _dependenciesMap = new DependenciesMap<Invoice>();

        static Invoice()
        {
            _dependenciesMap.AddDependency(i => i.TotalQuantity, i => i.Orders.Sum(o => o.Quantity), i => i.Orders.EachElement().Quantity)
                .AddDependency(i => i.TotalCost, i => i.Orders.Sum(o => o.Price * o.Quantity), i => i.Orders.EachElement().Price, i => i.Orders.EachElement().Quantity);
        }

        public Invoice()
        {
            _dependenciesMap.StartTracking(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}