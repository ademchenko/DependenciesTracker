namespace DependenciesTracking.QuickStartSample
{
    public class ViewModel
    {
        private readonly Order _order = new Order();

        private readonly Invoice _invoice = new Invoice();

        public Order Order
        {
            get { return _order; }
        }

        public Invoice Invoice
        {
            get { return _invoice; }
        }
    }
}