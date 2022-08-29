using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using DependenciesTracking.Tests.Stubs;
using Xunit;

namespace DependenciesTracking.Tests
{
    public class DependenciesTrackerLightTests
    {
        [Fact]
        public void OneLevelDepencyGenericTest()
        {
            var order = new FlatOrder();
            Assert.Equal(0, order.Cost);
            order.Price = 12;
            Assert.Equal(0, order.Cost);
            order.Quantity = 3;

            Assert.Equal(36, order.Cost);
        }

        [Fact]
        public void TwoLevelsDependency_UpdateDedendantsInitially()
        {
            var order = new Order();

            Assert.Equal(-1, order.TotalCost);
            Assert.Equal("Order is empty", order.OrderLine);
        }

        [Fact]
        public void TwoLevelsDependency_UpdateMiddleLevel_DefaultLastLevelValues()
        {
            var order = new Order();

            Assert.Equal(-1, order.TotalCost);
            Assert.Equal("Order is empty", order.OrderLine);

            order.Properties = new OrderProperties();

            Assert.Equal(0, order.TotalCost);
            Assert.Equal("Order category: , price = 0, quantity = 0", order.OrderLine);
        }

        [Fact]
        public void TwoLevelsDependency_UpdateMiddleLevel_NonDefaultLastLevelValues()
        {
            var order = new Order();

            Assert.Equal(-1, order.TotalCost);
            Assert.Equal("Order is empty", order.OrderLine);

            order.Properties = new OrderProperties { Category = "Laptops", Price = 11, Quantity = 4 };

            Assert.Equal(44, order.TotalCost);
            Assert.Equal("Order category: Laptops, price = 11, quantity = 4", order.OrderLine);
        }

        [Fact]
        public void TwoLevelsDependency_UpdateLastLevelValues()
        {
            var order = new Order();

            Assert.Equal(-1, order.TotalCost);
            Assert.Equal("Order is empty", order.OrderLine);

            order.Properties = new OrderProperties { Category = "Laptops", Price = 11, Quantity = 4 };

            Assert.Equal(44, order.TotalCost);
            Assert.Equal("Order category: Laptops, price = 11, quantity = 4", order.OrderLine);

            order.Properties.Price = 10;
            Assert.Equal(40, order.TotalCost);
            Assert.Equal("Order category: Laptops, price = 10, quantity = 4", order.OrderLine);

            order.Properties.Quantity = 7;
            Assert.Equal(70, order.TotalCost);
            Assert.Equal("Order category: Laptops, price = 10, quantity = 7", order.OrderLine);

            order.Properties.Category = "Desktops";
            Assert.Equal(70, order.TotalCost);
            Assert.Equal("Order category: Desktops, price = 10, quantity = 7", order.OrderLine);
        }

        [Fact]
        public void TwoLevelsDependency_UpdateMiddleThenLastLevelValues()
        {
            var order = new Order();

            Assert.Equal(-1, order.TotalCost);
            Assert.Equal("Order is empty", order.OrderLine);

            order.Properties = new OrderProperties { Category = "Laptops", Price = 11, Quantity = 4 };

            Assert.Equal(44, order.TotalCost);
            Assert.Equal("Order category: Laptops, price = 11, quantity = 4", order.OrderLine);

            order.Properties.Price = 10;
            Assert.Equal(40, order.TotalCost);
            Assert.Equal("Order category: Laptops, price = 10, quantity = 4", order.OrderLine);

            //Update middle level
            order.Properties = new OrderProperties { Category = "Phones", Price = 5, Quantity = 9 };

            Assert.Equal(45, order.TotalCost);
            Assert.Equal("Order category: Phones, price = 5, quantity = 9", order.OrderLine);

            order.Properties.Category = "Desktops";
            Assert.Equal(45, order.TotalCost);
            Assert.Equal("Order category: Desktops, price = 5, quantity = 9", order.OrderLine);
        }

        [Fact]
        public void TwoLevelsDependency_FirstLevel_DependantOfDependant()
        {
            var order = new Order();
            Assert.Equal(-1, order.TotalCost);
            Assert.Equal("ShortOrderLine is empty", order.ShortOrderLine);

            order.Properties = new OrderProperties();
            Assert.Equal(0, order.TotalCost);
            Assert.Equal("Order has total cost = 0", order.ShortOrderLine);

            order.Properties.Price = 12;
            Assert.Equal(0, order.TotalCost);
            Assert.Equal("Order has total cost = 0", order.ShortOrderLine);

            order.Properties.Quantity = 10;
            Assert.Equal(120, order.TotalCost);
            Assert.Equal("Order has total cost = 120", order.ShortOrderLine);
        }

        [Fact]
        public void NotifyPropertyCollectionTest()
        {
            var map = new DependenciesMap<Invoice>();
            map.AddDependency(i => i.TotalCost, i => 0, i => DependenciesTracking.CollectionExtensions.EachElement(i.Orders).Price, 
                                                        i => DependenciesTracking.CollectionExtensions.EachElement(i.Orders).Quantity);

            foreach (var mapItem in map.MapItems)
                Debug.WriteLine(mapItem.ToString());
        }

        [Fact]
        public void NotifyPropertyCollectionTest_Generic()
        {
            var invoice = new Invoice();
            Assert.Equal(-1, invoice.TotalCost);

            invoice.Orders = new ObservableCollection<FlatOrder> { new FlatOrder { Price = 1, Quantity = 5 }, new FlatOrder { Price = 2, Quantity = 6 } };
            Assert.Equal(17, invoice.TotalCost);

            invoice.Orders[0].Price = 2;
            Assert.Equal(22, invoice.TotalCost);

            invoice.Orders[1].Quantity = 7;
            Assert.Equal(24, invoice.TotalCost);

            invoice.Orders = new ObservableCollection<FlatOrder> { new FlatOrder { Price = 10, Quantity = 7 }, new FlatOrder { Price = 6, Quantity = 11 } };
            Assert.Equal(136, invoice.TotalCost);

            //Add/Insert item
            invoice.Orders.Add(new FlatOrder { Price = 2, Quantity = 3 });
            Assert.Equal(142, invoice.TotalCost);

            invoice.Orders.Insert(1, new FlatOrder { Price = 7, Quantity = 5 });
            Assert.Equal(177, invoice.TotalCost);
            
            //Remove item
            invoice.Orders.RemoveAt(3);
            Assert.Equal(171, invoice.TotalCost);

            //Replace item
            invoice.Orders[2] = new FlatOrder {Price = 3, Quantity = 10};
            Assert.Equal(135, invoice.TotalCost);

            //Reset item
            invoice.Orders.Clear();
            Assert.Equal(0, invoice.TotalCost);
        }

        [Fact]
        public void ObservableCollectionTest()
        {
            ObservableCollection<FlatOrder> orders = new ObservableCollection<FlatOrder>();

            var flatOrder1 = new FlatOrder();
            var flatOrder2 = new FlatOrder();
            orders.Add(flatOrder2);
            Debug.WriteLine("Added {0}", flatOrder2.GetHashCode());
            orders.Add(flatOrder1);
            Debug.WriteLine("Added {0}", flatOrder1.GetHashCode());

            orders.Insert(1, new FlatOrder());

            Debug.WriteLine("Going to remove {0}", flatOrder1.GetHashCode());
            orders.Remove(flatOrder1);

            //Debug.WriteLine("Left {0}", orders.Single().GetHashCode());
        }
    }
}
