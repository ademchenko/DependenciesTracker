using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace DependenciesTracking.Tests
{
    //NOTE: All test class setters do not have a checking for an equality before an actual value setting to precisely record count of setting attempts

    public sealed class ChildOrderItem : INotifyPropertyChanged
    {
        private int _price;
        private int _quantity;

        public int Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged("Price");
            }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged("Quantity");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
        private ObservableCollection<ChildOrderItem> _childItems;
        private int _childItemsCollectionDoubledLength;
        private decimal _costWithDiscount;
        private int _childItemsTotalCost;
        private int _unalignedIntMatrixElementsTotalSum;
        private int _unalignedIntMatrixMaxRowLength;
        private ObservableCollection<ObservableCollection<string>> _unalignedStringMatrix;
        private string _maxLengthStringInUnalignedStringMatrix;

        public int Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged("Price");
            }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged("Quantity");
            }
        }

        public int Cost
        {
            get { return _cost; }
            private set
            {
                _cost = value;
                OnPropertyChanged("Cost");
            }
        }

        public decimal CostWithDiscount
        {
            get { return _costWithDiscount; }
            private set
            {
                _costWithDiscount = value;
                OnPropertyChanged("CostWithDiscount");
            }
        }


        public string ClientFirstName
        {
            get { return _clientFirstName; }
            set
            {
                _clientFirstName = value;
                OnPropertyChanged("ClientFirstName");
            }
        }

        public string ClientLastName
        {
            get { return _clientLastName; }
            set
            {
                _clientLastName = value;
                OnPropertyChanged("ClientLastName");
            }
        }

        public string ClientShortDescription
        {
            get { return _clientShortDescription; }
            private set
            {
                _clientShortDescription = value;
                OnPropertyChanged("ClientShortDescription");
            }
        }

        public string ClientFullDescription
        {
            get { return _clientFullDescription; }
            private set
            {
                _clientFullDescription = value;
                OnPropertyChanged("ClientFullDescription");
            }
        }

        public ChildOrderItem ChildItem
        {
            get { return _childItem; }
            set
            {
                _childItem = value;
                OnPropertyChanged("ChildItem");
            }
        }

        public int ChildItemDoubledPrice
        {
            get { return _childItemDoubledPrice; }
            private set
            {
                _childItemDoubledPrice = value;
                OnPropertyChanged("ChildItemDoubledPrice");
            }
        }

        public ObservableCollection<ChildOrderItem> ChildItems
        {
            get { return _childItems; }
            set
            {
                _childItems = value;
                OnPropertyChanged("ChildItems");
            }
        }

        public int ChildItemsTotalCost
        {
            get { return _childItemsTotalCost; }
            private set
            {
                _childItemsTotalCost = value;
                OnPropertyChanged("ChildItemsTotalCost");
            }
        }

        public int ChildItemsTotalQuantity
        {
            get { return _childItemsTotalCost; }
            private set
            {
                _childItemsTotalCost = value;
                OnPropertyChanged("ChildItemsTotalQuantity");
            }
        }

        public int ChildItemsCollectionDoubledLength
        {
            get { return _childItemsCollectionDoubledLength; }
            private set
            {
                _childItemsCollectionDoubledLength = value;
                OnPropertyChanged("ChildItemsCollectionDoubledLength");
            }
        }

        public ObservableCollection<ObservableCollection<int>> UnalignedIntMatrix { get; set; }

        public int UnalignedIntMatrixElementsTotalSum
        {
            get { return _unalignedIntMatrixElementsTotalSum; }
            private set
            {
                _unalignedIntMatrixElementsTotalSum = value;
                OnPropertyChanged("UnalignedIntMatrixElementsTotalSum");
            }
        }

        public int UnalignedIntMatrixMaxRowLength
        {
            get { return _unalignedIntMatrixMaxRowLength; }
            private set
            {
                _unalignedIntMatrixMaxRowLength = value;
                OnPropertyChanged("UnalignedIntMatrixMaxRowLength");
            }
        }

        public ObservableCollection<ObservableCollection<string>> UnalignedStringMatrix
        {
            get { return _unalignedStringMatrix; }
            set
            {
                _unalignedStringMatrix = value;
                OnPropertyChanged("UnalignedStringMatrix");
            }
        }

        public string MaxLengthStringInUnalignedStringMatrix
        {
            get { return _maxLengthStringInUnalignedStringMatrix; }
            set
            {
                _maxLengthStringInUnalignedStringMatrix = value;
                OnPropertyChanged("MaxLengthStringInUnalignedStringMatrix");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public sealed class TestOrderItemsCollection : ObservableCollection<ChildOrderItem>
    {
        private int _doubledItemsCount;
        private int _totalCost;
        private int _totalQuantity;

        public int DoubledItemsCount
        {
            get { return _doubledItemsCount; }
            private set
            {
                _doubledItemsCount = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DoubledItemsCount"));
            }
        }

        public int TotalCost
        {
            get { return _totalCost; }
            private set
            {
                _totalCost = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TotalCost"));
            }
        }

        public int TotalQuantity
        {
            get { return _totalQuantity; }
            private set
            {
                _totalQuantity = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TotalQuantity"));
            }
        }

        public TestOrderItemsCollection() { }

        public TestOrderItemsCollection(IEnumerable<ChildOrderItem> collection)
            : base(collection) { }
    }


    public class DependentPropertyCalculationTests
    {
        private static readonly Random _random = new Random();

        [Fact]
        public void Init_SimpleValueTypePropertyDependency()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                .AddDependency(o => o.Cost, o => 2 * o.Quantity, o => o.Quantity);

            var price = _random.Next(1, 100);
            var quantity = _random.Next(100, 200);

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
                .AddDependency(o => o.ClientShortDescription, o => string.Format("Client: {0}", o.ClientFirstName),
                    o => o.ClientFirstName);

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
                .AddDependency(o => o.ClientShortDescription,
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
                .AddDependency(o => o.Cost, o => o.Price * o.Quantity, o => o.Quantity, o => o.Price);

            var price = _random.Next(1, 100);
            var quantity = _random.Next(100, 200);

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

            Assert.Equal(2, costPropertyChangeRaiseCount);
            Assert.Equal(price * quantity, order.Cost);
        }


        [Fact(
            Skip =
                "Issue #6 is created but not yet fixed. It's a low priority issue which doesn't affect the first release"
            )]
        public void Init_SimpleValueTypePropertiesDependency_Issue6()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                .AddDependency(o => o.Cost, o => o.Price * o.Quantity, o => o.Quantity, o => o.Price);

            var price = _random.Next(1, 100);
            var quantity = _random.Next(100, 200);

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
                .AddDependency(o => o.ClientFullDescription, o => string.Format("Client: {0} {1}", o.ClientFirstName,
                    o.ClientLastName ?? "undefined"), o => o.ClientFirstName, o => o.ClientLastName);

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

            Assert.Equal(2, clientFullDescriptionPropertyChangeRaiseCount);
            Assert.Equal("Client: " + firstName + " undefined", order.ClientFullDescription);
        }

        [Fact(
            Skip =
                "Issue #6 is created but not yet fixed. It's a low priority issue which doesn't affect the first release"
            )]
        public void Init_SimpleRefTypePropertiesDependency_NullAndNotNullProperty_Issue6()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                .AddDependency(o => o.ClientFullDescription, o => string.Format("Client: {0} {1}", o.ClientFirstName,
                    o.ClientLastName ?? "undefined"), o => o.ClientFirstName, o => o.ClientLastName);

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
                .AddDependency(o => o.ChildItemDoubledPrice,
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
                .AddDependency(o => o.ChildItemDoubledPrice,
                    o => o.ChildItem == null ? -1 : 2 * o.ChildItem.Price, o => o.ChildItem.Price);

            var childItemPrice = _random.Next(1, 100);

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

            Assert.Equal(2 * childItemPrice, order.ChildItemDoubledPrice);
            Assert.Equal(1, childItemDoubledPricePropertyChangeRaiseCount);
        }

        [Fact]
        public void Init_SimplePropertyChainDependency_CollectionLengthProperty()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                .AddDependency(o => o.ChildItemsCollectionDoubledLength, o => o.ChildItems == null ? -1 : 2 * o.ChildItems.Count,
                    o => o.ChildItems.Count);

            var order = new TestOrder
            {
                ChildItems = new ObservableCollection<ChildOrderItem>
                {
                    //3 items
                    new ChildOrderItem(),
                    new ChildOrderItem(),
                    new ChildOrderItem()
                }
            };

            var childItemsDoubledLengthPropertyChangeRaiseCount = 0;

            order.PropertyChanged += (_, args) =>
            {
                Assert.Equal("ChildItemsCollectionDoubledLength", args.PropertyName);
                childItemsDoubledLengthPropertyChangeRaiseCount++;
            };

            Assert.Equal(0, order.ChildItemsCollectionDoubledLength);
            Assert.Equal(0, childItemsDoubledLengthPropertyChangeRaiseCount);

            dependencyMap.StartTracking(order);

            Assert.Equal(2 * 3, order.ChildItemsCollectionDoubledLength);
            Assert.Equal(1, childItemsDoubledLengthPropertyChangeRaiseCount);
        }

        [Fact]
        public void Init_CollectionAsAProperty_EachElementAtTheEnd()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                .AddDependency(o => o.ChildItemsCollectionDoubledLength, o => o.ChildItems == null ? -1 : 2 * o.ChildItems.Count,
                    o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems));

            var order = new TestOrder
            {
                ChildItems = new ObservableCollection<ChildOrderItem>
                {
                    //3 items
                    new ChildOrderItem(),
                    new ChildOrderItem(),
                    new ChildOrderItem()
                }
            };

            var childItemsDoubledLengthPropertyChangeRaiseCount = 0;

            order.PropertyChanged += (_, args) =>
            {
                Assert.Equal("ChildItemsCollectionDoubledLength", args.PropertyName);
                childItemsDoubledLengthPropertyChangeRaiseCount++;
            };

            Assert.Equal(0, order.ChildItemsCollectionDoubledLength);
            Assert.Equal(0, childItemsDoubledLengthPropertyChangeRaiseCount);

            dependencyMap.StartTracking(order);

            Assert.Equal(2 * 3, order.ChildItemsCollectionDoubledLength);
            Assert.Equal(1, childItemsDoubledLengthPropertyChangeRaiseCount);
        }


        [Fact]
        public void Init_CollectionAsARoot_EachElementAtTheEnd()
        {
            var dependencyMap = new DependenciesMap<TestOrderItemsCollection>()
                .AddDependency(o => o.DoubledItemsCount, o => 2 * o.Count,
                    o => DependenciesTracking.CollectionExtensions.EachElement(o));

            var orderItems = new TestOrderItemsCollection
            {
                //3 items
                new ChildOrderItem(),
                new ChildOrderItem(),
                new ChildOrderItem()
            };

            var childItemsDoubledItemsCountPropertyChangeRaiseCount = 0;

            ((INotifyPropertyChanged)orderItems).PropertyChanged += (_, args) =>
            {
                Assert.Equal("DoubledItemsCount", args.PropertyName);
                childItemsDoubledItemsCountPropertyChangeRaiseCount++;
            };

            Assert.Equal(0, orderItems.DoubledItemsCount);
            Assert.Equal(0, childItemsDoubledItemsCountPropertyChangeRaiseCount);

            dependencyMap.StartTracking(orderItems);

            Assert.Equal(2 * 3, orderItems.DoubledItemsCount);
            Assert.Equal(1, childItemsDoubledItemsCountPropertyChangeRaiseCount);
        }

        [Fact]
        public void Init_CollectionAsARoot_EachElement_ValTypeProperty()
        {
            var dependencyMap = new DependenciesMap<TestOrderItemsCollection>()
                .AddDependency(o => o.TotalCost, o => o.Sum(i => i.Price * i.Quantity),
                    o => DependenciesTracking.CollectionExtensions.EachElement(o).Quantity,
                    o => DependenciesTracking.CollectionExtensions.EachElement(o).Quantity);

            var orderItem1Price = _random.Next(0, 10);
            var orderItem1Quantity = _random.Next(0, 10);
            var orderItem2Price = _random.Next(0, 10);
            var orderItem2Quantity = _random.Next(0, 10);
            var orderItem3Price = _random.Next(0, 10);
            var orderItem3Quantity = _random.Next(0, 10);

            var expectedTotalCost = orderItem1Price * orderItem1Quantity + orderItem2Price * orderItem2Quantity +
                                    orderItem3Price * orderItem3Quantity;


            var orderItems = new TestOrderItemsCollection
            {
                //3 items
                new ChildOrderItem {Price = orderItem1Price, Quantity = orderItem1Quantity},
                new ChildOrderItem {Price = orderItem2Price, Quantity = orderItem2Quantity},
                new ChildOrderItem {Price = orderItem3Price, Quantity = orderItem3Quantity}
            };

            var totalCostPropertyChangeRaiseCount = 0;

            ((INotifyPropertyChanged)orderItems).PropertyChanged += (_, args) =>
            {
                Assert.Equal("TotalCost", args.PropertyName);
                totalCostPropertyChangeRaiseCount++;
            };

            Assert.Equal(0, orderItems.TotalCost);
            Assert.Equal(0, totalCostPropertyChangeRaiseCount);

            dependencyMap.StartTracking(orderItems);

            Assert.Equal(expectedTotalCost, orderItems.TotalCost);
            Assert.Equal(2, totalCostPropertyChangeRaiseCount);
        }

        [Fact(Skip = "Issue #6 is created but not yet fixed. It's a low priority issue which doesn't affect the first release")]
        public void Init_CollectionAsARoot_EachElement_ValTypeProperty_Issue6()
        {
            var dependencyMap = new DependenciesMap<TestOrderItemsCollection>()
                .AddDependency(o => o.TotalCost, o => o.Sum(i => i.Price * i.Quantity),
                    o => DependenciesTracking.CollectionExtensions.EachElement(o).Quantity,
                    o => DependenciesTracking.CollectionExtensions.EachElement(o).Quantity);

            var orderItem1Price = _random.Next(0, 10);
            var orderItem1Quantity = _random.Next(0, 10);
            var orderItem2Price = _random.Next(0, 10);
            var orderItem2Quantity = _random.Next(0, 10);
            var orderItem3Price = _random.Next(0, 10);
            var orderItem3Quantity = _random.Next(0, 10);

            var expectedTotalCost = orderItem1Price * orderItem1Quantity + orderItem2Price * orderItem2Quantity +
                                    orderItem3Price * orderItem3Quantity;


            var orderItems = new TestOrderItemsCollection
            {
                //3 items
                new ChildOrderItem {Price = orderItem1Price, Quantity = orderItem1Quantity},
                new ChildOrderItem {Price = orderItem2Price, Quantity = orderItem2Quantity},
                new ChildOrderItem {Price = orderItem3Price, Quantity = orderItem3Quantity}
            };

            var totalCostPropertyChangeRaiseCount = 0;

            ((INotifyPropertyChanged)orderItems).PropertyChanged += (_, args) =>
            {
                Assert.Equal("TotalCost", args.PropertyName);
                totalCostPropertyChangeRaiseCount++;
            };

            Assert.Equal(0, orderItems.TotalCost);
            Assert.Equal(0, totalCostPropertyChangeRaiseCount);

            dependencyMap.StartTracking(orderItems);

            Assert.Equal(expectedTotalCost, orderItems.TotalCost);
            Assert.Equal(1, totalCostPropertyChangeRaiseCount);
        }

        [Fact]
        public void Init_CollectionAsAProperty_EachElement_ValTypeProperty()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                .AddDependency(o => o.ChildItemsTotalCost, o => o.ChildItems.Sum(i => i.Price * i.Quantity)
                                                         + (o.ChildItem == null ? 0 : o.ChildItem.Price * o.ChildItem.Quantity),
                    o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Price,
                    o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Quantity,
                    o => o.ChildItem);

            var childItemPrice = _random.Next(0, 10);
            var childItemQuantity = _random.Next(0, 10);
            var childItem1Price = _random.Next(0, 10);
            var childItem1Quantity = _random.Next(0, 10);
            var childItem2Price = _random.Next(0, 10);
            var childItem2Quantity = _random.Next(0, 10);
            var childItem3Price = _random.Next(0, 10);
            var child3Quantity = _random.Next(0, 10);

            var expectedChildItemsTotalCost = childItemPrice * childItemQuantity + childItem1Price * childItem1Quantity +
                                              childItem2Price * childItem2Quantity + childItem3Price * child3Quantity;

            var order = new TestOrder
            {
                ChildItem = new ChildOrderItem { Price = childItemPrice, Quantity = childItemQuantity },
                ChildItems = new ObservableCollection<ChildOrderItem>
                {
                    //3 items
                    new ChildOrderItem {Price = childItem1Price, Quantity = childItem1Quantity},
                    new ChildOrderItem {Price = childItem2Price, Quantity = childItem2Quantity},
                    new ChildOrderItem {Price = childItem3Price, Quantity = child3Quantity}
                }
            };

            var childItemsTotalCostPropertyChangeRaiseCount = 0;

            order.PropertyChanged += (_, args) =>
            {
                Assert.Equal("ChildItemsTotalCost", args.PropertyName);
                childItemsTotalCostPropertyChangeRaiseCount++;
            };

            Assert.Equal(0, order.ChildItemsTotalCost);
            Assert.Equal(0, childItemsTotalCostPropertyChangeRaiseCount);

            dependencyMap.StartTracking(order);

            Assert.Equal(expectedChildItemsTotalCost, order.ChildItemsTotalCost);
            Assert.Equal(3, childItemsTotalCostPropertyChangeRaiseCount);
        }

        [Fact(Skip = "Issue #6 is created but not yet fixed. It's a low priority issue which doesn't affect the first release")]
        public void Init_CollectionAsAProperty_EachElement_ValTypeProperty_Issue6()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                .AddDependency(o => o.ChildItemsTotalCost, o => o.ChildItems.Sum(i => i.Price * i.Quantity)
                                                         +
                                                         (o.ChildItem == null
                                                             ? 0
                                                             : o.ChildItem.Price * o.ChildItem.Quantity),
                    o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Price,
                    o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Quantity,
                    o => o.ChildItem);

            var childItemPrice = _random.Next(0, 10);
            var childItemQuantity = _random.Next(0, 10);
            var childItem1Price = _random.Next(0, 10);
            var childItem1Quantity = _random.Next(0, 10);
            var childItem2Price = _random.Next(0, 10);
            var childItem2Quantity = _random.Next(0, 10);
            var childItem3Price = _random.Next(0, 10);
            var child3Quantity = _random.Next(0, 10);

            var expectedChildItemsTotalCost = childItemPrice * childItemQuantity + childItem1Price * childItem1Quantity +
                                              childItem2Price * childItem2Quantity + childItem3Price * child3Quantity;

            var order = new TestOrder
            {
                ChildItem = new ChildOrderItem { Price = childItemPrice, Quantity = childItemQuantity },
                ChildItems = new ObservableCollection<ChildOrderItem>
                {
                    //3 items
                    new ChildOrderItem {Price = childItem1Price, Quantity = childItem1Quantity},
                    new ChildOrderItem {Price = childItem2Price, Quantity = childItem2Quantity},
                    new ChildOrderItem {Price = childItem3Price, Quantity = child3Quantity}
                }
            };

            var childItemsTotalCostPropertyChangeRaiseCount = 0;

            order.PropertyChanged += (_, args) =>
            {
                Assert.Equal("ChildItemsTotalCost", args.PropertyName);
                childItemsTotalCostPropertyChangeRaiseCount++;
            };

            Assert.Equal(0, order.ChildItemsTotalCost);
            Assert.Equal(0, childItemsTotalCostPropertyChangeRaiseCount);

            dependencyMap.StartTracking(order);

            Assert.Equal(expectedChildItemsTotalCost, order.ChildItemsTotalCost);
            Assert.Equal(3, childItemsTotalCostPropertyChangeRaiseCount);
        }

        [Fact]
        public void CollectionAsAProperty_EachElement_EachElement()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                .AddDependency(o => o.UnalignedIntMatrixElementsTotalSum, o => o.UnalignedIntMatrix.Sum(i => i.Sum()),
                    o => DependenciesTracking.CollectionExtensions.EachElement(
                        DependenciesTracking.CollectionExtensions.EachElement(o.UnalignedIntMatrix)));

            var unalignedIntMatrixElementsTotalSumPropertyChangeRaiseCount = 0;

            var item00 = _random.Next(0, 10);
            var item01 = _random.Next(0, 10);
            var item10 = _random.Next(0, 10);
            var item11 = _random.Next(0, 10);
            var item12 = _random.Next(0, 10);
            var item20 = _random.Next(0, 10);

            var expectedSum = item00 + item01 + item10 + item11 + item12 + item20;

            var order = new TestOrder
            {
                UnalignedIntMatrix = new ObservableCollection<ObservableCollection<int>>
                {
                    new ObservableCollection<int> { item00, item01 },
                    new ObservableCollection<int> {item10, item11, item12},
                    new ObservableCollection<int> {item20}
                }
            };

            order.PropertyChanged += (_, args) =>
            {
                Assert.Equal("UnalignedIntMatrixElementsTotalSum", args.PropertyName);
                unalignedIntMatrixElementsTotalSumPropertyChangeRaiseCount++;
            };

            Assert.Equal(0, order.UnalignedIntMatrixElementsTotalSum);
            Assert.Equal(0, unalignedIntMatrixElementsTotalSumPropertyChangeRaiseCount);

            dependencyMap.StartTracking(order);

            Assert.Equal(expectedSum, order.UnalignedIntMatrixElementsTotalSum);
            Assert.Equal(1, unalignedIntMatrixElementsTotalSumPropertyChangeRaiseCount);
        }

        [Fact]
        public void CollectionAsAProperty_EachElement_EachElement_ValTypeProperty()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                .AddDependency(o => o.MaxLengthStringInUnalignedStringMatrix,
                        o => o.UnalignedStringMatrix.SelectMany(c => c).Single(i => i.Length == o.UnalignedStringMatrix.SelectMany(item => item).Max(item => item.Length)),
                        o => DependenciesTracking.CollectionExtensions.EachElement(
                                    DependenciesTracking.CollectionExtensions.EachElement(o.UnalignedIntMatrix)));

            var maxLengthStringInUnalignedStringMatrixPropertyChangeRaiseCount = 0;


            var item00 = Guid.NewGuid().ToString().Substring(0, _random.Next(0, 10));
            var item01 = Guid.NewGuid().ToString().Substring(0, _random.Next(0, 10));
            var item10 = Guid.NewGuid().ToString().Substring(0, _random.Next(0, 10));
            var item11 = Guid.NewGuid().ToString().Substring(0, _random.Next(10, 20));
            var item12 = Guid.NewGuid().ToString().Substring(0, _random.Next(0, 10));
            var item20 = Guid.NewGuid().ToString().Substring(0, _random.Next(0, 10));

            var order = new TestOrder
            {
                UnalignedStringMatrix = new ObservableCollection<ObservableCollection<string>>
                {
                    new ObservableCollection<string> { item00, item01 },
                    new ObservableCollection<string> {item10, item11, item12},
                    new ObservableCollection<string> {item20}
                }
            };

            order.PropertyChanged += (_, args) =>
            {
                Assert.Equal("MaxLengthStringInUnalignedStringMatrix", args.PropertyName);
                maxLengthStringInUnalignedStringMatrixPropertyChangeRaiseCount++;
            };

            Assert.Equal(null, order.MaxLengthStringInUnalignedStringMatrix);
            Assert.Equal(0, maxLengthStringInUnalignedStringMatrixPropertyChangeRaiseCount);

            dependencyMap.StartTracking(order);

            Assert.Equal(item11, order.MaxLengthStringInUnalignedStringMatrix);
            Assert.Equal(1, maxLengthStringInUnalignedStringMatrixPropertyChangeRaiseCount);
        }

        [Fact]
        public void Init_DependentOfDependentPropertyIsBeingCalculatedOnSourcePropertyChange()
        {
            //if we add dependencies in direct order, i.e. source dependent property then dependent of that 
            //dependent property, we observe only a single assignment of the second property which is correct.
            //But reverse order of adding maps shows several initialization assignments of the second property which is wrong
            //and is kind of issue#6 observation. See the Init_DependentOfDependentPropertyIsBeingCalculatedOnSourcePropertyChange_ReverseOrderOfAddingMaps test.
            var dependencyMap = new DependenciesMap<TestOrder>()
                                    .AddDependency(o => o.Cost, o => o.Price * o.Quantity, o => o.Price, o => o.Quantity)
                                    .AddDependency(o => o.CostWithDiscount, o => 0.9m * o.Cost, o => o.Cost);


            var price = _random.Next(1, 10);
            var quantity = _random.Next(1, 10);

            var expectedCostWithDiscount = 0.9m * price * quantity;

            var order = new TestOrder();

            var costWithDiscountPropertyChangeRaiseCount = 0;

            order.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == "CostWithDiscount")
                    costWithDiscountPropertyChangeRaiseCount++;
            };

            Assert.Equal(0, order.CostWithDiscount);
            Assert.Equal(0, costWithDiscountPropertyChangeRaiseCount);

            order.Price = price;
            order.Quantity = quantity;

            dependencyMap.StartTracking(order);

            Assert.Equal(expectedCostWithDiscount, order.CostWithDiscount);
            Assert.Equal(1, costWithDiscountPropertyChangeRaiseCount);
        }

        [Fact]
        public void Init_DependentOfDependentPropertyIsBeingCalculatedOnSourcePropertyChange_ReverseOrderOfAddingMaps()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                                    .AddDependency(o => o.CostWithDiscount, o => 0.9m * o.Cost, o => o.Cost)
                                    .AddDependency(o => o.Cost, o => o.Price * o.Quantity, o => o.Price, o => o.Quantity);


            var price = _random.Next(1, 10);
            var quantity = _random.Next(1, 10);

            var expectedCostWithDiscount = 0.9m * price * quantity;

            var order = new TestOrder();

            var costWithDiscountPropertyChangeRaiseCount = 0;

            order.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == "CostWithDiscount")
                    costWithDiscountPropertyChangeRaiseCount++;
            };

            Assert.Equal(0, order.CostWithDiscount);
            Assert.Equal(0, costWithDiscountPropertyChangeRaiseCount);

            order.Price = price;
            order.Quantity = quantity;

            dependencyMap.StartTracking(order);

            Assert.Equal(expectedCostWithDiscount, order.CostWithDiscount);
            Assert.Equal(3, costWithDiscountPropertyChangeRaiseCount);
        }

        //[Fact]
        [Fact(Skip = "Issue #6 is created but not yet fixed. It's a low priority issue which doesn't affect the first release")]
        public void Init_DependentOfDependentPropertyIsBeingCalculatedOnSourcePropertyChange_ReverseOrderOfAddingMaps_Issue6()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                                    .AddDependency(o => o.CostWithDiscount, o => 0.9m * o.Cost, o => o.Cost)
                                    .AddDependency(o => o.Cost, o => o.Price * o.Quantity, o => o.Price, o => o.Quantity);

            var price = _random.Next(1, 10);
            var quantity = _random.Next(1, 10);

            var expectedCostWithDiscount = 0.9m * price * quantity;

            var order = new TestOrder();

            var costWithDiscountPropertyChangeRaiseCount = 0;

            order.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == "CostWithDiscount")
                    costWithDiscountPropertyChangeRaiseCount++;
            };

            Assert.Equal(0, order.CostWithDiscount);
            Assert.Equal(0, costWithDiscountPropertyChangeRaiseCount);

            order.Price = price;
            order.Quantity = quantity;

            dependencyMap.StartTracking(order);

            Assert.Equal(expectedCostWithDiscount, order.CostWithDiscount);
            Assert.Equal(1, costWithDiscountPropertyChangeRaiseCount);
        }

        [Fact]
        public void SimpleProperty_Change()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                                      .AddDependency(o => o.Cost, o => o.Price * o.Quantity, o => o.Price, o => o.Quantity);

            var price = _random.Next(1, 10);
            var quantity = _random.Next(10, 20);

            var expectedCost = price * quantity;

            var testOrder = new TestOrder { Price = price };

            dependencyMap.StartTracking(testOrder);

            Assert.Equal(0, testOrder.Cost);

            int costPropertyChangeRaiseCount = 0;

            testOrder.PropertyChanged += (_1, e) =>
            {
                if (e.PropertyName == "Cost")
                    costPropertyChangeRaiseCount++;
            };

            testOrder.Quantity = quantity;

            Assert.Equal(expectedCost, testOrder.Cost);
            Assert.Equal(1, costPropertyChangeRaiseCount);
        }

        [Fact]
        public void SimplePropertyChain_ChangeOfLeafProperty()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                                        .AddDependency(o => o.ChildItemDoubledPrice, o => o.ChildItem == null ? -1 : 2 * o.ChildItem.Price, o => o.ChildItem.Price);

            var priceOnLeafChange = _random.Next(1, 10);

            var expectedDoubledPriceOnLeafChange = 2 * priceOnLeafChange;

            var testOrder = new TestOrder { ChildItem = new ChildOrderItem() };

            dependencyMap.StartTracking(testOrder);

            Assert.Equal(0, testOrder.ChildItemDoubledPrice);

            var childItemDoubledPricePropertyChangeRaiseCount = 0;

            PropertyChangedEventHandler propertyChangeOnSettingChildItemPrice = (_1, e) =>
            {
                if (e.PropertyName == "ChildItemDoubledPrice")
                    childItemDoubledPricePropertyChangeRaiseCount++;
            };

            testOrder.PropertyChanged += propertyChangeOnSettingChildItemPrice;

            testOrder.ChildItem.Price = priceOnLeafChange;

            Assert.Equal(1, childItemDoubledPricePropertyChangeRaiseCount);
            Assert.Equal(expectedDoubledPriceOnLeafChange, testOrder.ChildItemDoubledPrice);
        }

        [Fact]
        public void SimplePropertyChain_ChangeOfInnerProperty()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                                        .AddDependency(o => o.ChildItemDoubledPrice, o => o.ChildItem == null ? -1 : 2 * o.ChildItem.Price, o => o.ChildItem.Price);

            var initialChildItemPrice = _random.Next(1, 10);
            var priceOnChildItemChange = _random.Next(1, 10);

            var expectedDoubledPrice = 2 * priceOnChildItemChange;

            var testOrder = new TestOrder { ChildItem = new ChildOrderItem { Price = initialChildItemPrice } };

            dependencyMap.StartTracking(testOrder);

            Assert.Equal(2 * initialChildItemPrice, testOrder.ChildItemDoubledPrice);

            var childItemDoubledPricePropertyChangeRaiseCount = 0;

            PropertyChangedEventHandler propertyChangeOnSettingChildItemPrice = (_1, e) =>
            {
                if (e.PropertyName == "ChildItemDoubledPrice")
                    childItemDoubledPricePropertyChangeRaiseCount++;
            };

            testOrder.PropertyChanged += propertyChangeOnSettingChildItemPrice;

            testOrder.ChildItem = new ChildOrderItem { Price = priceOnChildItemChange };

            Assert.Equal(1, childItemDoubledPricePropertyChangeRaiseCount);
            Assert.Equal(expectedDoubledPrice, testOrder.ChildItemDoubledPrice);
        }

        [Fact]
        public void SimplePropertyChain_ChangeOfLeafAndInnerProperty()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                                        .AddDependency(o => o.ChildItemDoubledPrice, o => o.ChildItem == null ? -1 : 2 * o.ChildItem.Price, o => o.ChildItem.Price);

            var priceOnMiddleChange = _random.Next(1, 10);
            var priceOnLeafChange = _random.Next(1, 10);

            var expectedDoubledPriceOnMiddleChange = 2 * priceOnMiddleChange;
            var expectedDoubledPriceOnLeafChange = 2 * priceOnLeafChange;

            var testOrder = new TestOrder();

            dependencyMap.StartTracking(testOrder);

            Assert.Equal(-1, testOrder.ChildItemDoubledPrice);

            var childItemDoubledPricePropertyChangeOnSettingChildItemRaiseCount = 0;

            PropertyChangedEventHandler propertyChangeOnSettingChildItem = (_1, e) =>
            {
                if (e.PropertyName == "ChildItemDoubledPrice")
                    childItemDoubledPricePropertyChangeOnSettingChildItemRaiseCount++;
            };

            testOrder.PropertyChanged += propertyChangeOnSettingChildItem;

            testOrder.ChildItem = new ChildOrderItem { Price = priceOnMiddleChange };

            Assert.Equal(expectedDoubledPriceOnMiddleChange, testOrder.ChildItemDoubledPrice);
            Assert.Equal(1, childItemDoubledPricePropertyChangeOnSettingChildItemRaiseCount);

            var childItemDoubledPricePropertyChangeOnSettingChildItemPriceRaiseCount = 0;

            PropertyChangedEventHandler propertyChangeOnSettingChildItemPrice = (_1, e) =>
            {
                if (e.PropertyName == "ChildItemDoubledPrice")
                    childItemDoubledPricePropertyChangeOnSettingChildItemPriceRaiseCount++;
            };

            testOrder.PropertyChanged += propertyChangeOnSettingChildItemPrice;

            testOrder.ChildItem.Price = priceOnLeafChange;

            Assert.Equal(expectedDoubledPriceOnLeafChange, testOrder.ChildItemDoubledPrice);
            Assert.Equal(1, childItemDoubledPricePropertyChangeOnSettingChildItemPriceRaiseCount);
        }

        [Fact]
        public void SimplePropertyChain_OrphansDoesNotHaveAnImpactOnDependentProperty()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                .AddDependency(o => o.ChildItemDoubledPrice, o => 2 * o.ChildItem.Price, o => o.ChildItem.Price);

            var testOrder = new TestOrder { ChildItem = new ChildOrderItem { Price = _random.Next(0, 10) } };

            dependencyMap.StartTracking(testOrder);

            //Changing the Price property of the child item has the impact on dependent property 
            //while it's an item of a root object...
            Assert.PropertyChanged(testOrder, "ChildItemDoubledPrice", () => { testOrder.ChildItem.Price = _random.Next(0, 10); });

            var orphanChildItem = testOrder.ChildItem;

            testOrder.ChildItem = new ChildOrderItem { Price = _random.Next(0, 10) };

            //Later when we replaced those child item with the new one
            //there shouldn't be any property change subscription leaks, i.e.
            //changing the Price property of the old child item shouldn't have any impact on dependent property
            testOrder.PropertyChanged += (_1, _2) => Assert.False(true, "Orphan child item impacts dependent property");

            orphanChildItem.Price = _random.Next(0, 10);
        }

        [Fact]
        public void CollectionProperty_CollectionItemLeafPropertyChange()
        {
            var dependenciesMap = new DependenciesMap<TestOrder>()
                                        .AddDependency(o => o.ChildItemsTotalQuantity, o => o.ChildItems.Sum(i => i.Quantity),
                                            o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Quantity);

            var itemsCount = _random.Next(1, 20);
            var itemQuantities = Enumerable.Range(0, itemsCount).Select(_ => _random.Next(0, 100)).ToList();

            var initiallyExpectedTotalQuantity = itemQuantities.Sum();

            var itemsToChangeIndices = new HashSet<int>();

            foreach (var index in Enumerable.Range(0, Math.Min(5, itemsCount)).Select(_ => _random.Next(0, itemsCount)))
                itemsToChangeIndices.Add(index);

            var testOrder = new TestOrder
            {
                ChildItems = new ObservableCollection<ChildOrderItem>(itemQuantities.Select(q => new ChildOrderItem { Quantity = q }))
            };

            Assert.Equal(0, testOrder.ChildItemsTotalQuantity);

            dependenciesMap.StartTracking(testOrder);

            Assert.Equal(initiallyExpectedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            var totalQuantityChange = 0;

            foreach (var itemToChangeIndex in itemsToChangeIndices)
            {
                var quantityChange = _random.Next(1, 100);
                if (_random.Next(0, 2) == 0)
                    quantityChange = -quantityChange;

                totalQuantityChange += quantityChange;

                var newQuantity = itemQuantities[itemToChangeIndex] + quantityChange;

                testOrder.ChildItems[itemToChangeIndex].Quantity = newQuantity;
            }

            var expectedChangedTotalQuantity = initiallyExpectedTotalQuantity + totalQuantityChange;

            CheckExpectedQuantityChanged(initiallyExpectedTotalQuantity, expectedChangedTotalQuantity);

            Assert.Equal(expectedChangedTotalQuantity, testOrder.ChildItemsTotalQuantity);
        }

        [Fact]
        public void RootAsACollection_CollectionItemLeafPropertyChange()
        {
            var dependenciesMap = new DependenciesMap<TestOrderItemsCollection>()
                                        .AddDependency(o => o.TotalQuantity, o => o.Sum(i => i.Quantity),
                                            o => DependenciesTracking.CollectionExtensions.EachElement(o).Quantity);

            var itemsCount = _random.Next(1, 20);
            var itemQuantities = Enumerable.Range(0, itemsCount).Select(_ => _random.Next(0, 100)).ToList();

            var initiallyExpectedTotalQuantity = itemQuantities.Sum();

            var itemsToChangeIndices = new HashSet<int>();

            foreach (var index in Enumerable.Range(0, Math.Min(5, itemsCount)).Select(_ => _random.Next(0, itemsCount)))
                itemsToChangeIndices.Add(index);

            var orderItems = new TestOrderItemsCollection(itemQuantities.Select(q => new ChildOrderItem { Quantity = q }));

            Assert.Equal(0, orderItems.TotalQuantity);

            dependenciesMap.StartTracking(orderItems);

            Assert.Equal(initiallyExpectedTotalQuantity, orderItems.TotalQuantity);

            var totalQuantityChange = 0;

            foreach (var itemToChangeIndex in itemsToChangeIndices)
            {
                var quantityChange = _random.Next(1, 100);
                if (_random.Next(0, 2) == 0)
                    quantityChange = -quantityChange;

                totalQuantityChange += quantityChange;

                var newQuantity = itemQuantities[itemToChangeIndex] + quantityChange;

                orderItems[itemToChangeIndex].Quantity = newQuantity;
            }

            if (totalQuantityChange == 0)
                throw new InvalidOperationException("Wrong test condition. totalQuantityChange cannot be 0 since we need to get different value to ensure that dependent property recalculation works.");

            var expectedChangedTotalQuantity = initiallyExpectedTotalQuantity + totalQuantityChange;

            CheckExpectedQuantityChanged(initiallyExpectedTotalQuantity, expectedChangedTotalQuantity);

            Assert.Equal(expectedChangedTotalQuantity, orderItems.TotalQuantity);
        }

        [Fact]
        public void CollectionProperty_Add()
        {
            var dependenciesMap = new DependenciesMap<TestOrder>()
                                        .AddDependency(o => o.ChildItemsTotalQuantity, o => o.ChildItems.Sum(i => i.Quantity),
                                            o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Quantity);

            var itemsCount = _random.Next(2, 6);
            var itemQuantities = Enumerable.Range(0, itemsCount).Select(_ => _random.Next(0, 100)).ToList();

            var initiallyExpectedTotalQuantity = itemQuantities.Sum();

            var testOrder = new TestOrder
            {
                ChildItems = new ObservableCollection<ChildOrderItem>(itemQuantities.Select(q => new ChildOrderItem { Quantity = q }))
            };

            Assert.Equal(0, testOrder.ChildItemsTotalQuantity);

            dependenciesMap.StartTracking(testOrder);

            Assert.Equal(initiallyExpectedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            var itemsToAddCount = _random.Next(1, 5);
            var newItemQuantities = Enumerable.Range(0, itemsToAddCount).Select(_ => _random.Next(1, 100)).ToList();
            var itemsToAdd = newItemQuantities.Select(q => new ChildOrderItem { Quantity = q }).ToList();

            var collectionChangeRaiseCount = 0;
            testOrder.ChildItems.CollectionChanged += (sender, e) =>
            {
                if (e.Action != NotifyCollectionChangedAction.Add)
                    throw new InvalidOperationException("Wrong test condition. This test should check \"Add\" action.");

                collectionChangeRaiseCount++;
            };

            foreach (var item in itemsToAdd)
            {
                var indexToAdd = _random.Next(0, testOrder.ChildItems.Count);
                testOrder.ChildItems.Insert(indexToAdd, item);
            }

            //Yes, the exception message is not fully correct (in the case of collectionChangeRaiseCount > itemsCount)
            if (collectionChangeRaiseCount != itemsToAddCount)
                throw new InvalidOperationException("Wrong test condition. Probably some of \"Add\" events hasn't been raised.");

            var expectedChangedTotalQuantity = initiallyExpectedTotalQuantity + newItemQuantities.Sum();

            CheckExpectedQuantityChanged(initiallyExpectedTotalQuantity, expectedChangedTotalQuantity);

            Assert.Equal(expectedChangedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            //Check that new items properties are tracked
            var indexToChangeProperty = _random.Next(0, itemsToAdd.Count);
            var changeDelta = _random.Next(1, 100) * (_random.Next(0, 2) == 0 ? 1 : -1);

            Assert.PropertyChanged(testOrder, "ChildItemsTotalQuantity", () =>
            {
                itemsToAdd[indexToChangeProperty].Quantity += changeDelta;
            });

            expectedChangedTotalQuantity += changeDelta;

            Assert.Equal(expectedChangedTotalQuantity, testOrder.ChildItemsTotalQuantity);
        }

        [Fact]
        public void CollectionProperty_Remove()
        {
            var dependenciesMap = new DependenciesMap<TestOrder>()
                                        .AddDependency(o => o.ChildItemsTotalQuantity, o => o.ChildItems.Sum(i => i.Quantity),
                                            o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Quantity);

            var itemsCount = _random.Next(2, 6);
            var itemQuantities = Enumerable.Range(0, itemsCount).Select(_ => _random.Next(1, 100)).ToList();

            var itemsToRemoveCount = _random.Next(1, itemsCount);

            var initiallyExpectedTotalQuantity = itemQuantities.Sum();

            var testOrder = new TestOrder
            {
                ChildItems = new ObservableCollection<ChildOrderItem>(itemQuantities.Select(q => new ChildOrderItem { Quantity = q }))
            };

            Assert.Equal(0, testOrder.ChildItemsTotalQuantity);

            dependenciesMap.StartTracking(testOrder);

            Assert.Equal(initiallyExpectedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            var collectionChangedRaiseCount = 0;

            testOrder.ChildItems.CollectionChanged += (sender, e) =>
            {
                if (e.Action != NotifyCollectionChangedAction.Remove)
                    throw new InvalidOperationException("Wrong test condition. This test should check \"Remove\" action.");

                collectionChangedRaiseCount++;
            };

            for (int i = 0; i < itemsToRemoveCount; i++)
            {
                var indexToRemove = _random.Next(0, testOrder.ChildItems.Count);
                itemQuantities.RemoveAt(indexToRemove);
                testOrder.ChildItems.RemoveAt(indexToRemove);

            }

            //Yes, the exception message is not fully correct (in the case of collectionChangeRaiseCount > itemsToRemoveCount)
            if (collectionChangedRaiseCount != itemsToRemoveCount)
                throw new InvalidOperationException("Wrong test condition. Probably some of \"Remove\" events hasn't been raised.");

            var expectedChangedTotalQuantity = itemQuantities.Sum();

            CheckExpectedQuantityChanged(initiallyExpectedTotalQuantity, expectedChangedTotalQuantity);

            Assert.Equal(expectedChangedTotalQuantity, testOrder.ChildItemsTotalQuantity);
        }

        [Fact]
        public void CollectionProperty_Remove_OrphansAreNotTracked()
        {
            var dependenciesMap = new DependenciesMap<TestOrder>()
                                        .AddDependency(o => o.ChildItemsTotalQuantity, o => o.ChildItems.Sum(i => i.Quantity),
                                            o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Quantity);

            var itemsCount = _random.Next(2, 6);
            var items = Enumerable.Range(0, itemsCount).Select(_ => new ChildOrderItem { Quantity = _random.Next(1, 100) }).ToList();
            var indexToRemove = _random.Next(0, itemsCount);
            var itemToRemove = items[indexToRemove];

            var testOrder = new TestOrder
            {
                ChildItems = new ObservableCollection<ChildOrderItem>(items)
            };

            dependenciesMap.StartTracking(testOrder);

            //Precondition - dependent property is recalculated on item removing
            Assert.PropertyChanged(testOrder, "ChildItemsTotalQuantity", () =>
            {
                testOrder.ChildItems.RemoveAt(indexToRemove);
            });

            testOrder.PropertyChanged += (_, args) =>
            {
                Debug.Assert(args.PropertyName == "ChildItemsTotalQuantity");
                Assert.False(true, "Changing dependent property ChildItemsTotalQuantity is not expected.");
            };

            //Post condition - changing orphan (removed) child item property doesn't affect dependent property
            itemToRemove.Quantity += 2;
        }

        [Fact]
        public void CollectionProperty_Replace()
        {
            var dependenciesMap = new DependenciesMap<TestOrder>()
                .AddDependency(o => o.ChildItemsTotalQuantity, o => o.ChildItems.Sum(i => i.Quantity),
                    o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Quantity);

            var itemsCount = _random.Next(2, 6);
            var itemQuantities = Enumerable.Range(0, itemsCount).Select(_ => _random.Next(1, 100)).ToList();

            var initiallyExpectedTotalQuantity = itemQuantities.Sum();

            var testOrder = new TestOrder
            {
                ChildItems =
                    new ObservableCollection<ChildOrderItem>(
                        itemQuantities.Select(q => new ChildOrderItem { Quantity = q }))
            };

            Assert.Equal(0, testOrder.ChildItemsTotalQuantity);

            dependenciesMap.StartTracking(testOrder);

            Assert.Equal(initiallyExpectedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            var replacedItemIndices =
                Enumerable.Range(0, _random.Next(1, itemsCount + 1))
                    .Select(_ => _random.Next(0, itemsCount))
                    .Distinct()
                    .ToList();

            var newItemDeltaQuantities = new Queue<int>(Enumerable.Range(0, replacedItemIndices.Count).Select(_ => _random.Next(1, 100)));

            var collectionChangeRaiseCount = 0;
            testOrder.ChildItems.CollectionChanged += (sender, e) =>
            {
                if (e.Action != NotifyCollectionChangedAction.Replace)
                    throw new InvalidOperationException("Wrong test condition. This test should check \"Replace\" action.");

                collectionChangeRaiseCount++;
            };

            var newOrderItems = new List<ChildOrderItem>();

            foreach (var itemIndex in replacedItemIndices)
            {
                var newQuantityDelta = newItemDeltaQuantities.Dequeue();
                itemQuantities[itemIndex] += newQuantityDelta;

                var newOrderItem = new ChildOrderItem { Quantity = itemQuantities[itemIndex] };
                newOrderItems.Add(newOrderItem);

                testOrder.ChildItems[itemIndex] = newOrderItem;
            }

            //Yes, the exception message is not fully correct (in the case of collectionChangeRaiseCount > itemsToReplaceCount)
            if (collectionChangeRaiseCount != replacedItemIndices.Count)
                throw new InvalidOperationException("Wrong test condition. Probably some of \"Replace\" events hasn't been raised.");

            var expectedChangedTotalQuantity = itemQuantities.Sum();

            CheckExpectedQuantityChanged(initiallyExpectedTotalQuantity, expectedChangedTotalQuantity);

            Assert.Equal(expectedChangedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            //Check that new items properties are tracked
            var indexToChangeProperty = _random.Next(0, newOrderItems.Count);
            var changeDelta = _random.Next(1, 100) * (_random.Next(0, 2) == 0 ? 1 : -1);

            Assert.PropertyChanged(testOrder, "ChildItemsTotalQuantity", () =>
            {
                newOrderItems[indexToChangeProperty].Quantity += changeDelta;
            });

            expectedChangedTotalQuantity += changeDelta;

            Assert.Equal(expectedChangedTotalQuantity, testOrder.ChildItemsTotalQuantity);
        }

        [Fact]
        public void CollectionProperty_Replace_OrphansAreNotTracked()
        {
            var dependenciesMap = new DependenciesMap<TestOrder>()
                                        .AddDependency(o => o.ChildItemsTotalQuantity, o => o.ChildItems.Sum(i => i.Quantity),
                                            o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Quantity);

            var itemsCount = _random.Next(2, 6);
            var items = Enumerable.Range(0, itemsCount).Select(_ => new ChildOrderItem { Quantity = _random.Next(1, 100) }).ToList();
            var indexToReplace = _random.Next(0, itemsCount);
            var itemToRemove = items[indexToReplace];

            var testOrder = new TestOrder
            {
                ChildItems = new ObservableCollection<ChildOrderItem>(items)
            };

            dependenciesMap.StartTracking(testOrder);

            //Precondition - dependent property is recalculated on item removing
            Assert.PropertyChanged(testOrder, "ChildItemsTotalQuantity", () =>
            {
                testOrder.ChildItems.RemoveAt(indexToReplace);
            });

            testOrder.PropertyChanged += (_, args) =>
            {
                Debug.Assert(args.PropertyName == "ChildItemsTotalQuantity");
                Assert.False(true, "Changing dependent property ChildItemsTotalQuantity is not expected.");
            };

            //Post condition - changing orphan (removed) child item property doesn't affect dependent property
            itemToRemove.Quantity += 2;
        }

        [Fact]
        public void CollectionProperty_Move()
        {
            var dependenciesMap = new DependenciesMap<TestOrder>()
                                        .AddDependency(o => o.ChildItemsTotalQuantity, o => o.ChildItems.Sum(i => i.Quantity),
                                            o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Quantity);

            var itemsCount = _random.Next(2, 6);
            var itemQuantities = Enumerable.Range(0, itemsCount).Select(_ => _random.Next(0, 100)).ToList();

            var initiallyExpectedTotalQuantity = itemQuantities.Sum();

            var testOrder = new TestOrder
            {
                ChildItems = new ObservableCollection<ChildOrderItem>(itemQuantities.Select(q => new ChildOrderItem { Quantity = q }))
            };

            Assert.Equal(0, testOrder.ChildItemsTotalQuantity);

            dependenciesMap.StartTracking(testOrder);

            Assert.Equal(initiallyExpectedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            var itemsToMoveCount = _random.Next(0, itemsCount);

            var movedItemIndices = Enumerable.Range(0, itemsToMoveCount).Select(_ => _random.Next(0, itemsCount)).Distinct();

            var collectionChangeRaised = false;
            testOrder.ChildItems.CollectionChanged += (sender, e) =>
            {
                if (e.Action != NotifyCollectionChangedAction.Move)
                    throw new InvalidOperationException("Wrong test condition. This test should check \"Move\" action.");

                collectionChangeRaised = true;
            };

            //Make one guaranteed movement
            testOrder.ChildItems.Move(0, itemsCount - 1);

            foreach (var movedItemIndex in movedItemIndices)
            {
                //random might return the same as movedItemIndex value. That's the reason why we do one guaranteed (no-random) movement
                var newIndex = _random.Next(0, itemsCount);
                testOrder.ChildItems.Move(movedItemIndex, newIndex);
            }

            if (!collectionChangeRaised)
                throw new InvalidOperationException("Wrong test condition. No one \"Move\" event has been raised.");

            Assert.Equal(initiallyExpectedTotalQuantity, testOrder.ChildItemsTotalQuantity);
        }

        //Two following 2 tests are kind of whitebox tests but they are quite important.
        //According to the implementation details internal tracking structure mimics the source collection items order.
        //So, if a collection consists of three items { 'A', 'B', 'C' } then the tracking structure consists of three 
        //tracking items over the collection elements: { trackingOf('A'), trackingOf('B'), trackingOf('C') }.
        //When me do any of actions on the source collection (Add, Remove, Move and so on) the tracking structure
        //repeats the same action. Like removeAt(0) on the source collection causes trackingOf('A') to be removed as well.
        //Move(0, 2) causes tracking structure to be reordered to { trackingOf('B'), trackingOf('C'), trackingOf('A') } state.
        //So, it's not possible to catch wrong work of a source collection Move handling just by checking the value of dependent
        //property after the movement since it remains the same. Instead we need to find the way to check internal state of tracking
        //items. The following 2 tests do two checks for that:
        // 1. The tracking item on the place 'moveFromIndex' is changed after the source collection item movement. For that check we 
        // remove element at 'moveFromIndex' and expect that this action won't affect the tracking of previously moved item.
        // 2. The tracking item on the place 'moveToIndex' is changed after the source collection item movement and is equal to 
        //trackingOf('A'). For that check we remove element at 'moveToIndex' and expect that the previously moved item won't be tracked
        //after that.
        [Fact]
        public void CollectionProperty_Move_TrackerTracksCorrectItemsAfterMovement_RemoveItemAtTheMoveFromIndex()
        {
            var dependenciesMap = new DependenciesMap<TestOrder>()
                                        .AddDependency(o => o.ChildItemsTotalQuantity, o => o.ChildItems.Sum(i => i.Quantity),
                                            o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Quantity);

            var itemsCount = _random.Next(10, 20);
            var itemQuantities = Enumerable.Range(0, itemsCount).Select(_ => _random.Next(0, 100)).ToList();

            var initiallyExpectedTotalQuantity = itemQuantities.Sum();

            var childOrderItems = itemQuantities.Select(q => new ChildOrderItem { Quantity = q }).ToList();

            var testOrder = new TestOrder
            {
                ChildItems = new ObservableCollection<ChildOrderItem>(childOrderItems)
            };

            Assert.Equal(0, testOrder.ChildItemsTotalQuantity);

            dependenciesMap.StartTracking(testOrder);

            Assert.Equal(initiallyExpectedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            var moveFromIndex = _random.Next(0, itemsCount / 2);
            var moveToIndex = _random.Next(itemsCount / 2, itemsCount);

            var movingItem = childOrderItems[moveFromIndex];

            Debug.Assert(moveFromIndex != moveToIndex);

            var collectionChangeRaised = false;
            NotifyCollectionChangedEventHandler childItemsCollectionChangedOnMoveAction = (sender, e) =>
            {
                if (e.Action != NotifyCollectionChangedAction.Move)
                    throw new InvalidOperationException(
                        string.Format("Wrong test condition. This test expects \"Move\" action, but actual was {0} action.", e.Action));

                collectionChangeRaised = true;
            };

            testOrder.ChildItems.CollectionChanged += childItemsCollectionChangedOnMoveAction;

            //Do movement
            testOrder.ChildItems.Move(moveFromIndex, moveToIndex);

            if (!collectionChangeRaised)
                throw new InvalidOperationException("Wrong test condition. No one \"Move\" event has been raised.");

            testOrder.ChildItems.CollectionChanged -= childItemsCollectionChangedOnMoveAction;

            Assert.Equal(initiallyExpectedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            //We're going to ensure that internal items tracking list updated according to the source items moving.

            //Precondition - moving item is being tracked before the remove element at first index
            Assert.PropertyChanged(testOrder, "ChildItemsTotalQuantity", () =>
            {
                movingItem.Quantity += 2;
            });

            testOrder.ChildItems.RemoveAt(moveFromIndex);

            //Post condition - movingItem continues to be tracked since it was moved from the first index 
            //before the removing by first index
            Assert.PropertyChanged(testOrder, "ChildItemsTotalQuantity", () =>
            {
                movingItem.Quantity += 2;
            });
        }

        [Fact]
        public void CollectionProperty_Move_TrackerTracksCorrectItemsAfterMovement_RemoveItemAtTheMoveToIndex()
        {
            var dependenciesMap = new DependenciesMap<TestOrder>()
                                        .AddDependency(o => o.ChildItemsTotalQuantity, o => o.ChildItems.Sum(i => i.Quantity),
                                            o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Quantity);

            var itemsCount = _random.Next(10, 20);
            var itemQuantities = Enumerable.Range(0, itemsCount).Select(_ => _random.Next(0, 100)).ToList();

            var initiallyExpectedTotalQuantity = itemQuantities.Sum();

            var childOrderItems = itemQuantities.Select(q => new ChildOrderItem { Quantity = q }).ToList();

            var testOrder = new TestOrder
            {
                ChildItems = new ObservableCollection<ChildOrderItem>(childOrderItems)
            };

            Assert.Equal(0, testOrder.ChildItemsTotalQuantity);

            dependenciesMap.StartTracking(testOrder);

            Assert.Equal(initiallyExpectedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            var moveFromIndex = _random.Next(0, itemsCount / 2);
            var moveToIndex = _random.Next(itemsCount / 2, itemsCount);

            var movingItem = childOrderItems[moveFromIndex];

            Debug.Assert(moveFromIndex != moveToIndex);

            var collectionChangeRaised = false;
            NotifyCollectionChangedEventHandler childItemsCollectionChangedOnMoveAction = (sender, e) =>
            {
                if (e.Action != NotifyCollectionChangedAction.Move)
                    throw new InvalidOperationException(
                        string.Format("Wrong test condition. This test expects \"Move\" action, but actual was {0} action.", e.Action));

                collectionChangeRaised = true;
            };

            testOrder.ChildItems.CollectionChanged += childItemsCollectionChangedOnMoveAction;

            //Do movement
            testOrder.ChildItems.Move(moveFromIndex, moveToIndex);

            if (!collectionChangeRaised)
                throw new InvalidOperationException("Wrong test condition. No one \"Move\" event has been raised.");

            testOrder.ChildItems.CollectionChanged -= childItemsCollectionChangedOnMoveAction;

            Assert.Equal(initiallyExpectedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            //We're going to ensure that internal items tracking list updated according to the source items moving.

            //Precondition - moving item is being tracked before the remove element at first index
            Assert.PropertyChanged(testOrder, "ChildItemsTotalQuantity", () =>
            {
                movingItem.Quantity += 2;
            });

            testOrder.ChildItems.RemoveAt(moveToIndex);

            //Post condition - movingItem is removed from the collection and therefore shouldn't be tracked
            testOrder.PropertyChanged += (_, eventArgs) =>
            {
                Debug.Assert(eventArgs.PropertyName == "ChildItemsTotalQuantity");
                Assert.False(true, "No property change was expected because orphan child item property had been changed.");
            };

            movingItem.Quantity += 2;
        }

        [Fact]
        public void CollectionProperty_Reset_CorrectCalculation_OrphansAreNotTracked()
        {
            var dependenciesMap = new DependenciesMap<TestOrder>()
                                        .AddDependency(o => o.ChildItemsTotalQuantity, o => o.ChildItems.Sum(i => i.Quantity),
                                            o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Quantity);

            var itemsCount = _random.Next(2, 6);
            var itemQuantities = Enumerable.Range(0, itemsCount).Select(_ => _random.Next(0, 100)).ToList();

            var initiallyExpectedTotalQuantity = itemQuantities.Sum();

            var childOrderItems = itemQuantities.Select(q => new ChildOrderItem { Quantity = q }).ToList();

            var testOrder = new TestOrder
            {
                ChildItems = new ObservableCollection<ChildOrderItem>(childOrderItems)
            };

            Assert.Equal(0, testOrder.ChildItemsTotalQuantity);

            dependenciesMap.StartTracking(testOrder);

            Assert.Equal(initiallyExpectedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            var collectionChangedRaised = false;
            testOrder.ChildItems.CollectionChanged += (sender, e) =>
            {
                if (e.Action != NotifyCollectionChangedAction.Reset)
                    throw new InvalidOperationException("Wrong test condition. This test should check \"Reset\" action.");

                collectionChangedRaised = true;
            };

            testOrder.ChildItems.Clear();

            if (!collectionChangedRaised)
                throw new InvalidOperationException("Wrong test condition. \"Add\" event is not raised.");

            CheckExpectedQuantityChanged(initiallyExpectedTotalQuantity, 0);

            Assert.Equal(0, testOrder.ChildItemsTotalQuantity);

            //Check that orphans are not tracked
            testOrder.PropertyChanged += (_, args) =>
            {
                Debug.Assert(args.PropertyName == "ChildItemsTotalQuantity");
                Assert.False(true, "Changing dependent property ChildItemsTotalQuantity is not expected.");
            };

            foreach (var childOrderItem in childOrderItems)
                childOrderItem.Quantity += _random.Next(1, 100) * (_random.Next(0, 2) == 0 ? 1 : -1);
        }

        [Fact]
        public void CollectionProperty_Reset_CorrectCalculation_NewlyAddedItemsAreProperlyTracked()
        {
            var dependenciesMap = new DependenciesMap<TestOrder>()
                                        .AddDependency(o => o.ChildItemsTotalQuantity, o => o.ChildItems.Sum(i => i.Quantity),
                                            o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Quantity);

            var itemsCount = _random.Next(2, 6);
            var itemQuantities = Enumerable.Range(0, itemsCount).Select(_ => _random.Next(0, 100)).ToList();

            var initiallyExpectedTotalQuantity = itemQuantities.Sum();

            var childOrderItems = itemQuantities.Select(q => new ChildOrderItem { Quantity = q }).ToList();

            var testOrder = new TestOrder
            {
                ChildItems = new ObservableCollection<ChildOrderItem>(childOrderItems)
            };

            Assert.Equal(0, testOrder.ChildItemsTotalQuantity);

            dependenciesMap.StartTracking(testOrder);

            Assert.Equal(initiallyExpectedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            var collectionChangedRaised = false;
            testOrder.ChildItems.CollectionChanged += (sender, e) => { collectionChangedRaised = true; };

            testOrder.ChildItems.Clear();

            if (!collectionChangedRaised)
                throw new InvalidOperationException("Wrong test condition. \"Reset\" event is not raised.");

            CheckExpectedQuantityChanged(initiallyExpectedTotalQuantity, 0);

            Assert.Equal(0, testOrder.ChildItemsTotalQuantity);

            var newlyAddedChildOrderItem = new ChildOrderItem { Price = 1, Quantity = 2 };

            testOrder.ChildItems.Add(newlyAddedChildOrderItem);

            Assert.Equal(2, testOrder.ChildItemsTotalQuantity);

            testOrder.ChildItems.RemoveAt(0);

            Assert.Equal(0, testOrder.ChildItemsTotalQuantity);

            //Check that orphans are not tracked
            testOrder.PropertyChanged += (_, args) =>
            {
                Debug.Assert(args.PropertyName == "ChildItemsTotalQuantity");
                Assert.False(true, "Changing dependent property ChildItemsTotalQuantity is not expected.");
            };

            newlyAddedChildOrderItem.Price = 4;
            childOrderItems.ForEach(i => { i.Price++; i.Quantity++; });
        }

        [Fact]
        public void CollectionProperty_DisposedTrackerShouldntTrackPathsAndUpdateDependentProperties()
        {
            var dependenciesMap = new DependenciesMap<TestOrder>()
                                          .AddDependency(o => o.ChildItemsTotalCost, o => o.ChildItems.Sum(i => i.Quantity * i.Price),
                                              o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Quantity,
                                              o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Price);

            var initialChildOrderItem = new ChildOrderItem { Price = 10, Quantity = 3 };
            var initialCost = initialChildOrderItem.Price * initialChildOrderItem.Quantity;

            var childOrderItems = new[] { initialChildOrderItem };

            var testOrder = new TestOrder { ChildItems = new ObservableCollection<ChildOrderItem>(childOrderItems) };

            Assert.Equal(0, testOrder.ChildItemsTotalQuantity);

            var tracker = dependenciesMap.StartTracking(testOrder);

            Assert.Equal(initialCost, testOrder.ChildItemsTotalCost);

            tracker.Dispose();
            var allowedToChangeDependentProperty = false;

            //Check that tracker doesn't change dependent properties after dispose
            testOrder.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == "ChildItemsTotalCost")
                    Assert.False(!allowedToChangeDependentProperty, "Changing dependent property ChildItemsTotalQuantity is not expected.");
            };
            //Trying to set the new collection
            testOrder.ChildItems = new ObservableCollection<ChildOrderItem>(childOrderItems);
            //Above changes doesn't have an influence on dependent property since the tracker is disposed
            Assert.Equal(initialCost, testOrder.ChildItemsTotalCost);

            var newlyAddedChildOrderItem = new ChildOrderItem { Price = 1, Quantity = 2 };
            //Trying to add new item into the collection...
            testOrder.ChildItems.Add(newlyAddedChildOrderItem);
            //Above changes doesn't have an influence on dependent property since the tracker is disposed
            Assert.Equal(initialCost, testOrder.ChildItemsTotalCost);

            //then change its properties
            newlyAddedChildOrderItem.Price++;
            newlyAddedChildOrderItem.Quantity++;

            //Above changes doesn't have an influence on dependent property since the tracker is disposed
            Assert.Equal(initialCost, testOrder.ChildItemsTotalCost);

            //Now starting tracking again
            allowedToChangeDependentProperty = true;
            dependenciesMap.StartTracking(testOrder);

            Assert.Equal(initialCost + 2 * 3, testOrder.ChildItemsTotalCost);

            //Trying to change change last item properties
            newlyAddedChildOrderItem.Price++;
            //Now tracker should track again
            Assert.Equal(initialCost + 3 * 3, testOrder.ChildItemsTotalCost);
            newlyAddedChildOrderItem.Quantity++;
            //Now tracker should track again
            Assert.Equal(initialCost + 3 * 4, testOrder.ChildItemsTotalCost);
        }

        [Fact]
        public void SimplePropertyChain_DisposedTrackerShouldntTrackPathsAndUpdateDependentProperties()
        {
            var dependenciesMap = new DependenciesMap<TestOrder>()
                                          .AddDependency(o => o.ChildItemsTotalCost, o => o.ChildItem.Price * o.ChildItem.Quantity,
                                              o => o.ChildItem.Quantity,
                                              o => o.ChildItem.Price);

            var initialChildOrderItem = new ChildOrderItem { Price = 10, Quantity = 3 };
            var initialCost = initialChildOrderItem.Price * initialChildOrderItem.Quantity;

            var testOrder = new TestOrder { ChildItem = initialChildOrderItem };

            Assert.Equal(0, testOrder.ChildItemsTotalQuantity);

            var tracker = dependenciesMap.StartTracking(testOrder);

            Assert.Equal(initialCost, testOrder.ChildItemsTotalCost);

            tracker.Dispose();

            var allowedToChangeDependentProperty = false;

            //Check that tracker doesn't change dependent properties after dispose
            testOrder.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == "ChildItemsTotalQuantity" && !allowedToChangeDependentProperty)
                    Assert.False(true, "Changing dependent property ChildItemsTotalQuantity is not expected.");
            };

            var newlyAddedChildOrderItem = new ChildOrderItem { Price = 1, Quantity = 2 };

            //Trying to change order item...
            testOrder.ChildItem = newlyAddedChildOrderItem;

            //then its properties
            testOrder.ChildItem.Price++;
            testOrder.ChildItem.Quantity++;

            //Above changes doesn't have an influence on dependent property since the tracker is disposed
            Assert.Equal(initialCost, testOrder.ChildItemsTotalCost);

            //Now starting tracking again
            allowedToChangeDependentProperty = true;
            dependenciesMap.StartTracking(testOrder);

            Assert.Equal(2 * 3, testOrder.ChildItemsTotalCost);

            //Now, trying to change order item...
            newlyAddedChildOrderItem = new ChildOrderItem { Price = 5, Quantity = 6 };
            testOrder.ChildItem = newlyAddedChildOrderItem;

            //And dependent property has been recalculated
            Assert.Equal(5 * 6, testOrder.ChildItemsTotalCost);

            //Then trying to change order item properties
            newlyAddedChildOrderItem.Price = 7;
            //And dependent property has been recalculated
            Assert.Equal(7 * 6, testOrder.ChildItemsTotalCost);

            newlyAddedChildOrderItem.Quantity = 12;
            //And dependent property has been recalculated
            Assert.Equal(7 * 12, testOrder.ChildItemsTotalCost);
        }

        [Fact]
        public void CollectionProperty_CollectionChanged_NewCollectionItemsAreTracked()
        {
            var dependenciesMap = new DependenciesMap<TestOrder>()
                                        .AddDependency(o => o.ChildItemsTotalQuantity, o => o.ChildItems.Sum(i => i.Quantity),
                                            o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Quantity);

            var initialItemsCount = _random.Next(2, 6);
            var initialItemQuantities = Enumerable.Range(0, initialItemsCount).Select(_ => _random.Next(0, 100)).ToList();
            var initiallyExpectedTotalQuantity = initialItemQuantities.Sum();
            var initialChildOrderItems = initialItemQuantities.Select(q => new ChildOrderItem { Quantity = q }).ToList();

            var newItemsCount = _random.Next(2, 6);
            var newItemQuantities = Enumerable.Range(0, newItemsCount).Select(_ => _random.Next(0, 100)).ToList();
            var newExpectedTotalQuantity = newItemQuantities.Sum();
            var newChildOrderItems = newItemQuantities.Select(q => new ChildOrderItem { Quantity = q }).ToList();

            var testOrder = new TestOrder
            {
                ChildItems = new ObservableCollection<ChildOrderItem>(initialChildOrderItems)
            };

            Assert.Equal(0, testOrder.ChildItemsTotalQuantity);

            dependenciesMap.StartTracking(testOrder);

            Assert.Equal(initiallyExpectedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            Assert.PropertyChanged(testOrder, "ChildItemsTotalQuantity", () =>
            {
                testOrder.ChildItems = new ObservableCollection<ChildOrderItem>(newChildOrderItems);
            });

            Assert.Equal(newExpectedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            //Check that new items properties are tracked
            var indexToChangeProperty = _random.Next(0, newChildOrderItems.Count);

            var changeDelta = _random.Next(1, 100) * (_random.Next(0, 2) == 0 ? 1 : -1);

            Assert.PropertyChanged(testOrder, "ChildItemsTotalQuantity", () =>
            {
                newChildOrderItems[indexToChangeProperty].Quantity += changeDelta;
            });

            newExpectedTotalQuantity += changeDelta;

            Assert.Equal(newExpectedTotalQuantity, testOrder.ChildItemsTotalQuantity);
        }

        [Fact]
        public void CollectionProperty_CollectionChanged_OrphanCollectionItemsAreNotTracked()
        {
            var dependenciesMap = new DependenciesMap<TestOrder>()
                                        .AddDependency(o => o.ChildItemsTotalQuantity, o => o.ChildItems.Sum(i => i.Quantity),
                                            o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Quantity);

            var initialItemsCount = _random.Next(2, 6);
            var initialItemQuantities = Enumerable.Range(0, initialItemsCount).Select(_ => _random.Next(0, 100)).ToList();
            var initiallyExpectedTotalQuantity = initialItemQuantities.Sum();
            var initialChildOrderItems = initialItemQuantities.Select(q => new ChildOrderItem { Quantity = q }).ToList();

            var newItemsCount = _random.Next(2, 6);
            var newItemQuantities = Enumerable.Range(0, newItemsCount).Select(_ => _random.Next(0, 100)).ToList();
            var newExpectedTotalQuantity = newItemQuantities.Sum();
            var newChildOrderItems = newItemQuantities.Select(q => new ChildOrderItem { Quantity = q }).ToList();

            var testOrder = new TestOrder
            {
                ChildItems = new ObservableCollection<ChildOrderItem>(initialChildOrderItems)
            };

            Assert.Equal(0, testOrder.ChildItemsTotalQuantity);

            dependenciesMap.StartTracking(testOrder);

            Assert.Equal(initiallyExpectedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            Assert.PropertyChanged(testOrder, "ChildItemsTotalQuantity", () =>
            {
                testOrder.ChildItems = new ObservableCollection<ChildOrderItem>(newChildOrderItems);
            });

            Assert.Equal(newExpectedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            testOrder.PropertyChanged += (_, args) =>
            {
                Debug.Assert(args.PropertyName == "ChildItemsTotalQuantity");
                Assert.False(true, "Changing dependent property ChildItemsTotalQuantity is not expected.");
            };

            //Check that orphan items properties are not tracked
            foreach (var initialChildOrderItem in initialChildOrderItems)
            {
                var changeDelta = _random.Next(1, 100) * (_random.Next(0, 2) == 0 ? 1 : -1);
                initialChildOrderItem.Quantity += changeDelta;
            }
        }

        [Fact]
        public void CollectionProperty_NullItemsSuccessfullyTracked()
        {
            var dependenciesMap = new DependenciesMap<TestOrder>()
                                        .AddDependency(o => o.ChildItemsTotalQuantity,
                                            o => o.ChildItems.Any(i => i == null) ? -1 : o.ChildItems.Sum(i => i.Quantity),
                                                o => DependenciesTracking.CollectionExtensions.EachElement(o.ChildItems).Quantity);

            var itemsCount = _random.Next(2, 6);
            var itemQuantities = Enumerable.Range(0, itemsCount).Select(_ => _random.Next(0, 100)).ToList();
            var expectedTotalQuantity = itemQuantities.Sum();
            var initialChildOrderItems = itemQuantities.Select(q => new ChildOrderItem { Quantity = q }).ToList();

            var indexToInsertNullItem = _random.Next(0, itemsCount);

            var testOrder = new TestOrder
            {
                ChildItems = new ObservableCollection<ChildOrderItem>(initialChildOrderItems)
            };

            Assert.Equal(0, testOrder.ChildItemsTotalQuantity);

            dependenciesMap.StartTracking(testOrder);

            Assert.Equal(expectedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            Assert.PropertyChanged(testOrder, "ChildItemsTotalQuantity", () =>
            {
                // ReSharper disable once ConvertToLambdaExpression
                testOrder.ChildItems.Insert(indexToInsertNullItem, null);
            });

            Assert.Equal(-1, testOrder.ChildItemsTotalQuantity);

            //Then we replace null item with non-null item and trying to change its properties 
            //to ensure the new tracking has been started successfully
            var newItemQuantity = _random.Next(1, 100);
            var newChildOrderItem = new ChildOrderItem { Quantity = newItemQuantity };

            Assert.PropertyChanged(testOrder, "ChildItemsTotalQuantity", () =>
            {
                testOrder.ChildItems[indexToInsertNullItem] = newChildOrderItem;
            });

            expectedTotalQuantity += newItemQuantity;

            Assert.Equal(expectedTotalQuantity, testOrder.ChildItemsTotalQuantity);

            var newItemQuantityDelta = _random.Next(1, 100) * (_random.Next(0, 2) == 0 ? 1 : -1);

            Assert.PropertyChanged(testOrder, "ChildItemsTotalQuantity", () =>
            {
                newChildOrderItem.Quantity += newItemQuantityDelta;
            });

            expectedTotalQuantity += newItemQuantityDelta;

            Assert.Equal(expectedTotalQuantity, testOrder.ChildItemsTotalQuantity);
        }

        private static void CheckExpectedQuantityChanged(int initiallyExpectedTotalQuantity, int expectedChangedTotalQuantity)
        {
            if (initiallyExpectedTotalQuantity == expectedChangedTotalQuantity)
                throw new InvalidOperationException("Expected quantity hasn't been really changed");
        }
    }
}