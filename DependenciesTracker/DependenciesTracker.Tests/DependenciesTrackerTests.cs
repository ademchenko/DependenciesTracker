using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using DependenciesTracker.Interfaces;
using DependenciesTracker.Tests.Stubs;
using Xunit;

namespace DependenciesTracker.Tests
{
    public class DependenciesTrackerTests
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
            Expression<Func<Invoice, FlatOrder>> expression = o => o.Orders.AnyElement();

            DependenciesMap<Invoice> map = new DependenciesMap<Invoice>();
            map.AddMap(i => i.TotalCost, i => 0, i => i.Orders.AnyElement().Price, i => i.Orders.AnyElement().Quantity);

            foreach (var mapItem in map.MapItems)
            {
                Debug.WriteLine(mapItem.ToString());
            }



        }
    }
}
