using Calculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Calculator
{
    internal class ButtonPressCommand : ICommand
    {
        private MainViewModel _viewModel;

        public MainViewModel viewModel
        {
            get { return _viewModel; }
            set { _viewModel = value; }
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            viewModel.GetPressedButton(parameter.ToString());
        }

        public ButtonPressCommand(MainViewModel mainviewModel)
        {
            viewModel = mainviewModel;
        }
    }
}
