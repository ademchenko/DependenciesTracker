using System;
using System.Windows.Input;
using DependenciesTracking.QuickStartSample.Annotations;

namespace DependenciesTracking.QuickStartSample
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
