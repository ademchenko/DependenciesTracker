using System.ComponentModel;
using System.Runtime.CompilerServices;
using DependenciesTracking;
using DependenciesTracking.Interfaces;

namespace DependenciesTracking.QuickStartSample
{
    public class Order : INotifyPropertyChanged
    {
        private decimal _price;
        private int _quantity;
        private decimal _cost;
        private decimal _discountPercent;
        private decimal _costWithDiscount;

        public decimal Price
        {
            get { return _price; }
            set
            {
                if (value == _price) return;
                _price = value;
                OnPropertyChanged();
            }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if (value == _quantity) return;
                _quantity = value;
                OnPropertyChanged();
            }
        }

        public decimal Cost
        {
            get { return _cost; }
            set
            {
                if (value == _cost) return;
                _cost = value;
                OnPropertyChanged();
            }
        }

        public decimal DiscountPercent
        {
            get { return _discountPercent; }
            set
            {
                if (value == _discountPercent) return;
                _discountPercent = value;
                OnPropertyChanged();
            }
        }

        public decimal CostWithDiscount
        {
            get { return _costWithDiscount; }
            set
            {
                if (value == _costWithDiscount) return;
                _costWithDiscount = value;
                OnPropertyChanged();
            }
        }

        private static readonly IDependenciesMap<Order> _dependenciesMap = new DependenciesMap<Order>();

        static Order()
        {
            _dependenciesMap.AddDependency(o => o.Cost, o => o.Price * o.Quantity, o => o.Price, o => o.Quantity)
                            .AddDependency(o => o.CostWithDiscount, o => o.Cost * (1 - o.DiscountPercent / 100), o => o.Cost, o => o.DiscountPercent);
        }

        public Order()
        {
            _dependenciesMap.StartTracking(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
