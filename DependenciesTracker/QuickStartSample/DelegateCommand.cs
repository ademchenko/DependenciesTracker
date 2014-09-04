using System;
using System.Windows.Input;
using QuickStartSample.Annotations;

namespace QuickStartSample
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _executeAction;

        public DelegateCommand([NotNull] Action<object> executeAction)
        {
            if (executeAction == null) 
                throw new ArgumentNullException("executeAction");
            
            _executeAction = executeAction;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
