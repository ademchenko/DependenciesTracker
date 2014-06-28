using System;
using System.ComponentModel;
using System.Linq;
using DependenciesTracker;
using DependenciesTracker.Interfaces;

namespace Benchmarks
{
    public class SimpleBenchmarkManualPropertyChangeTestObject : INotifyPropertyChanged
    {
        private int _price;
        private int _quantity;
        private int _cost;

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

        private void UpdateCost()
        {
            Cost = Price * Quantity;
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

        public SimpleBenchmarkManualPropertyChangeTestObject()
        {
            PropertyChanged += SimpleBenchmarkManualPropertyChangeTestObject_PropertyChanged_Price;
            PropertyChanged += SimpleBenchmarkManualPropertyChangeTestObject_PropertyChanged_Quantity;
            UpdateCost();
        }

        void SimpleBenchmarkManualPropertyChangeTestObject_PropertyChanged_Price(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Price")
                UpdateCost();
        }

        void SimpleBenchmarkManualPropertyChangeTestObject_PropertyChanged_Quantity(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Quantity")
                UpdateCost();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SimpleBenchmarkManualTestObject : INotifyPropertyChanged
    {
        private int _price;
        private int _quantity;
        private int _cost;

        public int Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged("Price");
                UpdateCost();
            }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged("Quantity");
                UpdateCost();
            }
        }

        private void UpdateCost()
        {
            Cost = Price * Quantity;
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

        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SimpleBenchmarkTrackerTestObject : INotifyPropertyChanged
    {
        private int _price;
        private int _quantity;
        private int _cost;
        private IDisposable _tracker;

        private static readonly IDependenciesMap<SimpleBenchmarkTrackerTestObject> _dependenciesMap;

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

        static SimpleBenchmarkTrackerTestObject()
        {
            _dependenciesMap = new DependenciesMap<SimpleBenchmarkTrackerTestObject>()
                                .AddMap(o => o.Cost, o => o.Price * o.Quantity, o => o.Price, o => o.Quantity);
        }

        public SimpleBenchmarkTrackerTestObject()
        {
            _tracker = _dependenciesMap.StartTracking(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BenchmarkTests
    {
        public void Test()
        {
            CheckCorrectness();

            var manualObject = new SimpleBenchmarkManualPropertyChangeTestObject();
            var trackerObject = new SimpleBenchmarkTrackerTestObject();

            //Warm up
            Update(manualObject, 1, 2);
            Update(trackerObject, 1, 2);
            Update(manualObject, 2, 10);
            Update(trackerObject, 2, 10);

            var random = new Random();

            //Prepare test data
            var prices = Enumerable.Range(0, 10000000).Select(_ => random.Next(1, 100)).ToArray();
            var quantities = Enumerable.Range(0, 10000000).Select(_ => random.Next(100, 200)).ToArray();

            //Test

            //var swManual = new Stopwatch();

            //swManual.Start();

            //for (int i = 0; i < 10000000; ++i)
            //    Update(manualObject, prices[i], quantities[i]);

            //swManual.Stop();

            //var swTracker = new Stopwatch();

            //swTracker.Start();
            //if (!JetBrains.Profiler.Core.Api.PerformanceProfiler.IsActive)
            //    throw new ApplicationException("Application isn't running under the profiler");

            JetBrains.Profiler.Core.Api.PerformanceProfiler.Begin();
            JetBrains.Profiler.Core.Api.PerformanceProfiler.Start();

            for (int i = 0; i < 10000000; ++i)
            {
                Update(trackerObject, prices[i], quantities[i]);
                Update(manualObject, prices[i], quantities[i]);
            }

            JetBrains.Profiler.Core.Api.PerformanceProfiler.Stop();
            JetBrains.Profiler.Core.Api.PerformanceProfiler.EndSave();

            //swTracker.Stop();

            //Console.WriteLine("Time for manual update: {0} ms", swManual.ElapsedMilliseconds);
            //Console.WriteLine("Time for tracker update: {0} ms", swTracker.ElapsedMilliseconds);

            //Console.WriteLine("Difference: {0} times", swTracker.ElapsedMilliseconds / swManual.ElapsedMilliseconds);
        }

        private void CheckCorrectness()
        {
            var random = new Random();

            var manualObject = new SimpleBenchmarkManualTestObject();

            var expectedPrice = random.Next(100, 200);
            var expectedQuantity = random.Next(200, 300);
            var expectedCost = expectedPrice * expectedQuantity;

            Update(manualObject, expectedPrice, expectedQuantity);

            if (expectedQuantity != manualObject.Quantity)
                throw new InvalidOperationException();


            var trackerObject = new SimpleBenchmarkTrackerTestObject();

            Update(trackerObject, expectedPrice, expectedQuantity);

            if (expectedQuantity != manualObject.Quantity)
                throw new InvalidOperationException();
        }

        private static void Update(SimpleBenchmarkManualTestObject manualObject, int expectedPrice, int expectedQuantity)
        {
            manualObject.Price = expectedPrice;
            manualObject.Quantity = expectedQuantity;
        }

        private static void Update(SimpleBenchmarkTrackerTestObject trackerObject, int expectedPrice, int expectedQuantity)
        {
            trackerObject.Price = expectedPrice;
            trackerObject.Quantity = expectedQuantity;
        }

        private static void Update(SimpleBenchmarkManualPropertyChangeTestObject trackerObject, int expectedPrice, int expectedQuantity)
        {
            trackerObject.Price = expectedPrice;
            trackerObject.Quantity = expectedQuantity;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var tests = new BenchmarkTests();

            tests.Test();
        }
    }
}
