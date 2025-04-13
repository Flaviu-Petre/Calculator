using Calculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Calculator.Commands
{
    internal class KeyboardInputHandler
    {
        private readonly MainViewModel _viewModel;

        public KeyboardInputHandler(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void Window_KeyDown(object sender, KeyEventArgs e)
        {
            string keyPressed = "";

            
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                keyPressed = (e.Key - Key.D0).ToString();
            }
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                keyPressed = (e.Key - Key.NumPad0).ToString();
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Add:
                    case Key.OemPlus:
                        keyPressed = "+";
                        break;

                    case Key.Subtract:
                    case Key.OemMinus:
                        keyPressed = "-";
                        break;

                    case Key.Multiply:
                        keyPressed = "*";
                        break;

                    case Key.Divide:
                    case Key.Oem2:
                        keyPressed = "/";
                        break;

                    case Key.Enter:
                        keyPressed = "=";
                        break;

                    case Key.Back:
                        keyPressed = "Del";
                        break;

                    case Key.Delete:
                        keyPressed = "Clr";
                        break;

                    case Key.Decimal:
                    case Key.OemPeriod:
                        keyPressed = ".";
                        break;    
                }
            }

            if (!string.IsNullOrEmpty(keyPressed))
            {
                _viewModel.GetPressedButton(keyPressed);
            }
        }
    }
}
