using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Schema;
using JetBrains.Annotations;
using Xunit;

namespace DependenciesTracker.Tests.PathBuilding
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

        [NotifyPropertyChangedInvocator]
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
    }


    public class DependentPropertyCalculationsTest
    {
        [NotNull]
        private static readonly Random _random = new Random();

        [Fact]
        public void Init_SimpleValueTypePropertyDependency()
        {
            var dependencyMap = new DependenciesMap<TestOrder>()
                .AddMap(o => o.Cost, o => 2 * o.Quantity, o => o.Quantity);

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
                .AddMap(o => o.ClientShortDescription, o => string.Format("Client: {0}", o.ClientFirstName),
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
                .AddMap(o => o.Cost, o => o.Price * o.Quantity, o => o.Quantity, o => o.Price);

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
                .AddMap(o => o.ClientFullDescription, o => string.Format("Client: {0} {1}", o.ClientFirstName,
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
                .AddMap(o => o.ClientFullDescription, o => string.Format("Client: {0} {1}", o.ClientFirstName,
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
                .AddMap(o => o.ChildItemsCollectionDoubledLength, o => o.ChildItems == null ? -1 : 2 * o.ChildItems.Count,
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
                .AddMap(o => o.ChildItemsCollectionDoubledLength, o => o.ChildItems == null ? -1 : 2 * o.ChildItems.Count,
                    o => DependenciesTracker.CollectionExtensions.EachElement(o.ChildItems));

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
                .AddMap(o => o.DoubledItemsCount, o => 2 * o.Count,
                    o => DependenciesTracker.CollectionExtensions.EachElement(o));

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
                .AddMap(o => o.TotalCost, o => o.Sum(i => i.Price * i.Quantity),
                    o => DependenciesTracker.CollectionExtensions.EachElement(o).Quantity,
                    o => DependenciesTracker.CollectionExtensions.EachElement(o).Quantity);

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
                .AddMap(o => o.TotalCost, o => o.Sum(i => i.Price * i.Quantity),
                    o => DependenciesTracker.CollectionExtensions.EachElement(o).Quantity,
                    o => DependenciesTracker.CollectionExtensions.EachElement(o).Quantity);

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
                .AddMap(o => o.ChildItemsTotalCost, o => o.ChildItems.Sum(i => i.Price * i.Quantity)
                                                         + (o.ChildItem == null ? 0 : o.ChildItem.Price * o.ChildItem.Quantity),
                    o => DependenciesTracker.CollectionExtensions.EachElement(o.ChildItems).Price,
                    o => DependenciesTracker.CollectionExtensions.EachElement(o.ChildItems).Quantity,
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
                .AddMap(o => o.ChildItemsTotalCost, o => o.ChildItems.Sum(i => i.Price * i.Quantity)
                                                         +
                                                         (o.ChildItem == null
                                                             ? 0
                                                             : o.ChildItem.Price * o.ChildItem.Quantity),
                    o => DependenciesTracker.CollectionExtensions.EachElement(o.ChildItems).Price,
                    o => DependenciesTracker.CollectionExtensions.EachElement(o.ChildItems).Quantity,
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
                .AddMap(o => o.UnalignedIntMatrixElementsTotalSum, o => o.UnalignedIntMatrix.Sum(i => i.Sum()),
                    o => DependenciesTracker.CollectionExtensions.EachElement(
                        DependenciesTracker.CollectionExtensions.EachElement(o.UnalignedIntMatrix)));

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
                .AddMap(o => o.MaxLengthStringInUnalignedStringMatrix,
                        o => o.UnalignedStringMatrix.SelectMany(c => c).Single(i => i.Length == o.UnalignedStringMatrix.SelectMany(item => item).Max(item => item.Length)),
                        o => DependenciesTracker.CollectionExtensions.EachElement(
                                    DependenciesTracker.CollectionExtensions.EachElement(o.UnalignedIntMatrix)));

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
                                    .AddMap(o => o.Cost, o => o.Price * o.Quantity, o => o.Price, o => o.Quantity)
                                    .AddMap(o => o.CostWithDiscount, o => 0.9m * o.Cost, o => o.Cost);


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
                                    .AddMap(o => o.CostWithDiscount, o => 0.9m * o.Cost, o => o.Cost)
                                    .AddMap(o => o.Cost, o => o.Price * o.Quantity, o => o.Price, o => o.Quantity);


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
                                    .AddMap(o => o.CostWithDiscount, o => 0.9m * o.Cost, o => o.Cost)
                                    .AddMap(o => o.Cost, o => o.Price * o.Quantity, o => o.Price, o => o.Quantity);

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
                                      .AddMap(o => o.Cost, o => o.Price * o.Quantity, o => o.Price, o => o.Quantity);

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
                                        .AddMap(o => o.ChildItemDoubledPrice, o => o.ChildItem == null ? -1 : 2 * o.ChildItem.Price, o => o.ChildItem.Price);

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
                                        .AddMap(o => o.ChildItemDoubledPrice, o => o.ChildItem == null ? -1 : 2 * o.ChildItem.Price, o => o.ChildItem.Price);

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
                                        .AddMap(o => o.ChildItemDoubledPrice, o => o.ChildItem == null ? -1 : 2 * o.ChildItem.Price, o => o.ChildItem.Price);

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
                .AddMap(o => o.ChildItemDoubledPrice, o => 2 * o.ChildItem.Price, o => o.ChildItem.Price);

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
        public void CollectionProperty_LeafPropertyChange()
        {
            var dependenciesMap = new DependenciesMap<TestOrder>()
                                        .AddMap(o => o.ChildItemsTotalQuantity, o => o.ChildItems.Sum(i => i.Quantity),
                                            o => DependenciesTracker.CollectionExtensions.EachElement(o.ChildItems).Quantity);

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

            foreach (var itemToChangeIndex in itemsToChangeIndices)
            {
                var newQuantity = _random.Next(0, 100);
                itemQuantities[itemToChangeIndex] = newQuantity;
                testOrder.ChildItems[itemToChangeIndex].Quantity = newQuantity;
            }

            var expectedChangedTotalQuantity = itemQuantities.Sum();

            Assert.Equal(expectedChangedTotalQuantity, testOrder.ChildItemsTotalQuantity);
        }
    }
}