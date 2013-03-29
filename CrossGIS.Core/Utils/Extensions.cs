using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace CrossGIS.Core.Utils
{
    public static class Extensions
    {
        #region ICommand

        public static void Raise(this ICommand command)
        {
            var relayCommand = command as RelayCommand;
            if (relayCommand != null)
                relayCommand.RaiseCanExecuteChanged();
        }

        public static bool TryExecute(this ICommand command, object parameter)
        {
            if (command != null && command.CanExecute(parameter))
            {
                command.Execute(parameter);
                return true;
            }
            return false;
        }

        #endregion

    }
}
