using System;
using System.ComponentModel;
using DependenciesTracker.Interfaces;

namespace DependenciesTracker.Tests.Stubs
{
    public class FlatOrder : INotifyPropertyChanged
    {
        private int _price;
        private int _quantity;
        private int _cost;

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

        public int Cost
        {
            get { return _cost; }
            private set
            {
                if (_cost != value)
                {
                    _cost = value;
                    OnPropertyChanged("Cost");
                }
            }
        }

        private static readonly IDependenciesMap<FlatOrder> _dependenciesMap = new DependenciesMap<FlatOrder>();
        private IDisposable _tracker;

        static FlatOrder()
        {
            _dependenciesMap.AddMap(o => o.Cost, o => o.Price * o.Quantity, o => o.Price, o => o.Quantity);
        }

        public FlatOrder()
        {
            _tracker = _dependenciesMap.StartTracking(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
