using System;
using System.ComponentModel;
using JetBrains.Annotations;
using Xunit;

namespace DependenciesTracker.Tests.PathBuilding
{
    public sealed class ChildOrderItem : INotifyPropertyChanged
    {
        private int _price;
        private int _quantity;

        public int Price
        {
            get { return _price; }
            set
            {
                if (value == _price) return;
                _price = value;
                OnPropertyChanged("Price");
            }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if (value == _quantity) return;
                _quantity = value;
                OnPropertyChanged("Quantity");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public sealed class TestOrder : INotifyPropertyChanged
    {
        private int _price;
        private int _quantity;
        private int _cost;
        private string _clientFirstName;
        private string _clientLastName;
        private string _clientShortDescription;
        private string _clientFullDescription;
        private ChildOrderItem _childItem;
        private int _childItemDoubledPrice;

        public int Price
        {
            get { return _price; }
            set
            {
                if (value == _price) return;
                _price = value;
                OnPropertyChanged("Price");
            }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if (value == _quantity) return;
                _quantity = value;
                OnPropertyChanged("Quantity");
            }
        }

        public int Cost
        {
            get { return _cost; }
            private set
            {
                if (value == _cost) return;
                _cost = value;
                OnPropertyChanged("Cost");
            }
        }

        public string ClientFirstName
        {
            get { return _clientFirstName; }
            set
            {
                if (value == _clientFirstName) return;
                _clientFirstName = value;
                OnPropertyChanged("ClientFirstName");
            }
        }

        public string ClientLastName
        {
            get { return _clientLastName; }
            set
            {
                if (value == _clientLastName) return;
                _clientLastName = value;
                OnPropertyChanged("ClientLastName");
            }
        }

        public string ClientShortDescription
        {
            get { return _clientShortDescription; }
            private set
            {
                if (value == _clientShortDescription) return;
                _clientShortDescription = value;
                OnPropertyChanged("ClientShortDescription");
            }
        }

        public string ClientFullDescription
        {
            get { return _clientFullDescription; }
            private set
            {
                if (value == _clientFullDescription) return;
                _clientFullDescription = value;
                OnPropertyChanged("ClientFullDescription");
            }
        }

        public ChildOrderItem ChildItem
        {
            get { return _childItem; }
            set
            {
                if (Equals(value, _childItem)) return;
                _childItem = value;
                OnPropertyChanged("ChildItem");
            }
        }

        public int ChildItemDoubledPrice
        {
            get { return _childItemDoubledPrice; }
            private set
            {
                if (value == _childItemDoubledPrice) return;
                _childItemDoubledPrice = value;
                OnPropertyChanged("ChildItemDoubledPrice");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class DependentPropertyCalculationsTest
    {
        [Fact]
        public void Init_SimpleValueTypePropertyDependency()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                .AddMap(o => o.Cost, o => 2 * o.Quantity, o => o.Quantity);

            var random = new Random();
            var price = random.Next(1, 100);
            var quantity = random.Next(100, 200);

            var order = new TestOrder { Price = price, Quantity = quantity };
            var costPropertyChangeRaiseCount = 0;

            order.PropertyChanged += (_, args) =>
            {
                Assert.Equal("Cost", args.PropertyName);
                costPropertyChangeRaiseCount++;
            };

            Assert.Equal(0, order.Cost);
            Assert.Equal(0, costPropertyChangeRaiseCount);

            dependencyMap.StartTracking(order);

            Assert.Equal(1, costPropertyChangeRaiseCount);
            Assert.Equal(2 * quantity, order.Cost);
        }

        [Fact]
        public void Init_SimpleRefTypeNotNullPropertyDependency()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                .AddMap(o => o.ClientShortDescription, o => string.Format("Client: {0}", o.ClientFirstName), o => o.ClientFirstName);

            var firstName = Guid.NewGuid().ToString();
            var lastName = Guid.NewGuid().ToString();

            var order = new TestOrder { ClientFirstName = firstName, ClientLastName = lastName };

            var clientShortDescriptionChangeRaiseCount = 0;

            order.PropertyChanged += (_, args) =>
            {
                Assert.Equal("ClientShortDescription", args.PropertyName);
                clientShortDescriptionChangeRaiseCount++;
            };

            Assert.Equal(null, order.ClientShortDescription);
            Assert.Equal(0, clientShortDescriptionChangeRaiseCount);

            dependencyMap.StartTracking(order);

            Assert.Equal(1, clientShortDescriptionChangeRaiseCount);
            Assert.Equal("Client: " + firstName, order.ClientShortDescription);
        }

        [Fact]
        public void Init_SimpleRefTypeNullPropertyDependency()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                .AddMap(o => o.ClientShortDescription,
                    o => o.ClientFirstName == null ? "Client: undefined" : "Client: defined", o => o.ClientFirstName);

            var lastName = Guid.NewGuid().ToString();

            var order = new TestOrder { ClientFirstName = null, ClientLastName = lastName };

            var clientShortDescriptionChangeRaiseCount = 0;

            order.PropertyChanged += (_, args) =>
            {
                Assert.Equal("ClientShortDescription", args.PropertyName);
                clientShortDescriptionChangeRaiseCount++;
            };

            Assert.Equal(null, order.ClientShortDescription);
            Assert.Equal(0, clientShortDescriptionChangeRaiseCount);

            dependencyMap.StartTracking(order);

            Assert.Equal(1, clientShortDescriptionChangeRaiseCount);
            Assert.Equal("Client: undefined", order.ClientShortDescription);
        }

        [Fact]
        public void Init_SimpleValueTypePropertiesDependency()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                .AddMap(o => o.Cost, o => o.Price * o.Quantity, o => o.Quantity, o => o.Price);

            var random = new Random();
            var price = random.Next(1, 100);
            var quantity = random.Next(100, 200);

            var order = new TestOrder { Price = price, Quantity = quantity };

            var costPropertyChangeRaiseCount = 0;

            order.PropertyChanged += (_, args) =>
            {
                Assert.Equal("Cost", args.PropertyName);
                costPropertyChangeRaiseCount++;
            };

            Assert.Equal(0, order.Cost);
            Assert.Equal(0, costPropertyChangeRaiseCount);

            dependencyMap.StartTracking(order);

            Assert.Equal(1, costPropertyChangeRaiseCount);
            Assert.Equal(price * quantity, order.Cost);
        }

        [Fact]
        public void Init_SimpleRefTypePropertiesDependency_NullAndNotNullProperty()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                .AddMap(o => o.ClientFullDescription, o => string.Format("Client: {0} {1}", o.ClientFirstName, "undefined"),
                                o => o.ClientFirstName, o => o.ClientLastName);

            var firstName = Guid.NewGuid().ToString();

            var order = new TestOrder { ClientFirstName = firstName, ClientLastName = null };

            var clientFullDescriptionPropertyChangeRaiseCount = 0;
            order.PropertyChanged += (_, args) =>
            {
                Assert.Equal("ClientFullDescription", args.PropertyName);
                clientFullDescriptionPropertyChangeRaiseCount++;
            };

            Assert.Equal(null, order.ClientShortDescription);
            Assert.Equal(0, clientFullDescriptionPropertyChangeRaiseCount);

            dependencyMap.StartTracking(order);

            Assert.Equal(1, clientFullDescriptionPropertyChangeRaiseCount);
            Assert.Equal("Client: " + firstName + " undefined", order.ClientFullDescription);
        }

        [Fact]
        public void Init_SimplePropertyChainDependency_NullPropertyValueInTheMiddleOfChain()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                                    .AddMap(o => o.ChildItemDoubledPrice,
                                        o => o.ChildItem == null ? -1 : 2 * o.ChildItem.Price, o => o.ChildItem.Price);

            var order = new TestOrder();

            var childItemDoubledPricePropertyChangeRaiseCount = 0;

            order.PropertyChanged += (_, args) =>
            {
                Assert.Equal("ChildItemDoubledPrice", args.PropertyName);
                childItemDoubledPricePropertyChangeRaiseCount++;
            };

            Assert.Equal(0, order.ChildItemDoubledPrice);
            Assert.Equal(0, childItemDoubledPricePropertyChangeRaiseCount);

            dependencyMap.StartTracking(order);

            Assert.Equal(1, childItemDoubledPricePropertyChangeRaiseCount);
            Assert.Equal(-1, order.ChildItemDoubledPrice);
        }

        [Fact]
        public void Init_SimplePropertyChainDependency_NotNullPropertyChain()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                                    .AddMap(o => o.ChildItemDoubledPrice,
                                        o => o.ChildItem == null ? -1 : 2 * o.ChildItem.Price, o => o.ChildItem.Price);

            var random = new Random();
            var childItemPrice = random.Next(1, 100);

            var order = new TestOrder { ChildItem = new ChildOrderItem { Price = childItemPrice } };

            var childItemDoubledPricePropertyChangeRaiseCount = 0;

            order.PropertyChanged += (_, args) =>
            {
                Assert.Equal("ChildItemDoubledPrice", args.PropertyName);
                childItemDoubledPricePropertyChangeRaiseCount++;
            };

            Assert.Equal(0, order.ChildItemDoubledPrice);
            Assert.Equal(0, childItemDoubledPricePropertyChangeRaiseCount);

            dependencyMap.StartTracking(order);

            Assert.Equal(1, childItemDoubledPricePropertyChangeRaiseCount);
            Assert.Equal(2 * childItemPrice, order.ChildItemDoubledPrice);
        }
    }
}