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
        
        private decimal _totalCost;

        public ObservableCollection<Order> Orders
        {
            get { return _orders; }
        }

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

        private DelegateCommand _addNewOrderCommand;
                
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