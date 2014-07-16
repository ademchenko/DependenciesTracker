using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
            set
            {
                _childItemsTotalCost = value;
                OnPropertyChanged("ChildItemsTotalCost");
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

            var random = new Random();

            var orderItem1Price = random.Next(0, 10);
            var orderItem1Quantity = random.Next(0, 10);
            var orderItem2Price = random.Next(0, 10);
            var orderItem2Quantity = random.Next(0, 10);
            var orderItem3Price = random.Next(0, 10);
            var orderItem3Quantity = random.Next(0, 10);

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

        [Fact(
            Skip =
                "Issue #6 is created but not yet fixed. It's a low priority issue which doesn't affect the first release"
            )]
        public void Init_CollectionAsARoot_EachElement_ValTypeProperty_Issue6()
        {
            var dependencyMap = new DependenciesMap<TestOrderItemsCollection>()
                .AddMap(o => o.TotalCost, o => o.Sum(i => i.Price * i.Quantity),
                    o => DependenciesTracker.CollectionExtensions.EachElement(o).Quantity,
                    o => DependenciesTracker.CollectionExtensions.EachElement(o).Quantity);

            var random = new Random();

            var orderItem1Price = random.Next(0, 10);
            var orderItem1Quantity = random.Next(0, 10);
            var orderItem2Price = random.Next(0, 10);
            var orderItem2Quantity = random.Next(0, 10);
            var orderItem3Price = random.Next(0, 10);
            var orderItem3Quantity = random.Next(0, 10);

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

            var random = new Random();

            var childItemPrice = random.Next(0, 10);
            var childItemQuantity = random.Next(0, 10);
            var childItem1Price = random.Next(0, 10);
            var childItem1Quantity = random.Next(0, 10);
            var childItem2Price = random.Next(0, 10);
            var childItem2Quantity = random.Next(0, 10);
            var childItem3Price = random.Next(0, 10);
            var child3Quantity = random.Next(0, 10);

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

        [Fact(
            Skip =
                "Issue #6 is created but not yet fixed. It's a low priority issue which doesn't affect the first release"
            )]
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

            var random = new Random();

            var childItemPrice = random.Next(0, 10);
            var childItemQuantity = random.Next(0, 10);
            var childItem1Price = random.Next(0, 10);
            var childItem1Quantity = random.Next(0, 10);
            var childItem2Price = random.Next(0, 10);
            var childItem2Quantity = random.Next(0, 10);
            var childItem3Price = random.Next(0, 10);
            var child3Quantity = random.Next(0, 10);

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

            var random = new Random();

            var item00 = random.Next(0, 10);
            var item01 = random.Next(0, 10);
            var item10 = random.Next(0, 10);
            var item11 = random.Next(0, 10);
            var item12 = random.Next(0, 10);
            var item20 = random.Next(0, 10);

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

            var random = new Random();

            var item00 = Guid.NewGuid().ToString().Substring(0, random.Next(0, 10));
            var item01 = Guid.NewGuid().ToString().Substring(0, random.Next(0, 10));
            var item10 = Guid.NewGuid().ToString().Substring(0, random.Next(0, 10));
            var item11 = Guid.NewGuid().ToString().Substring(0, random.Next(10, 20));
            var item12 = Guid.NewGuid().ToString().Substring(0, random.Next(0, 10));
            var item20 = Guid.NewGuid().ToString().Substring(0, random.Next(0, 10));

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
    }
}