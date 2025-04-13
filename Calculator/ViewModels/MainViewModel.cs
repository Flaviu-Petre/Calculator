using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tema_1_Calculator;
using Calculator.Commands;

namespace Calculator.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        #region Properties and Fields

        // Commands for clipboard operations
        public ICommand CopyCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand CutCommand { get; }
        public ButtonPressCommand buttonPressCommand { get; set; }

        // Main display properties
        private string _KeyPressedString = "";
        public string KeyPressedString
        {
            get { return _KeyPressedString; }
            set { _KeyPressedString = value; OnPropertyChanged(); }
        }

        private string _Entered_Number = "0";
        public string Entered_Number
        {
            get { return _Entered_Number; }
            set
            {
                _Entered_Number = FormatWithGrouping(value);
                OnPropertyChanged();
            }
        }

        // Memory related properties
        private ObservableCollection<string> _memoryValues = new ObservableCollection<string>();
        public ObservableCollection<string> MemoryValues
        {
            get { return _memoryValues; }
            set
            {
                _memoryValues = value;
                OnPropertyChanged();
            }
        }

        private int _selectedMemoryIndex = -1;
        public int SelectedMemoryIndex
        {
            get { return _selectedMemoryIndex; }
            set { _selectedMemoryIndex = value; OnPropertyChanged(); }
        }

        // Digit grouping property
        private bool _isDigitGroupingEnabled = false;
        public bool IsDigitGroupingEnabled
        {
            get { return _isDigitGroupingEnabled; }
            set
            {
                if (_isDigitGroupingEnabled != value)
                {
                    _isDigitGroupingEnabled = value;
                    // Refresh the display when toggling
                    Entered_Number = Entered_Number;
                    OnPropertyChanged();
                }
            }
        }

        // State tracking
        private List<string> EnteredKeys = new List<string>();
        private double Number = 0;
        private bool FirstNumberEntered = true;
        private bool EqualToFlag = false;
        private bool FunctionPressed = false;
        private string SelectedFunction = "";
        public string PreviousEnteredKey = "";

        // PropertyChanged event
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Constructor

        public MainViewModel()
        {
            Entered_Number = "0";
            KeyPressedString = "";
            EnteredKeys = new List<string>();

            buttonPressCommand = new ButtonPressCommand(this);
            CopyCommand = new RelayCommand(CopyText);
            PasteCommand = new RelayCommand(PasteText);
            CutCommand = new RelayCommand(CutText);
        }

        #endregion

        #region Property Changed

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Clipboard Operations

        private void CopyText()
        {
            if (!string.IsNullOrEmpty(Entered_Number))
            {
                Clipboard.SetText(Entered_Number);
            }
        }

        private void PasteText()
        {
            if (Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText();
                if (double.TryParse(clipboardText, out _))
                {
                    // If we had just calculated a result, start a new calculation
                    if (EqualToFlag)
                    {
                        Clear();
                    }
                    Entered_Number = clipboardText;

                    // Update the history display
                    EnteredKeys.Clear();
                    EnteredKeys.Add(Entered_Number);
                    UpdateEnteredKeysOnGui();
                }
            }
        }

        private void CutText()
        {
            if (!string.IsNullOrEmpty(Entered_Number) && Entered_Number != "0")
            {
                Clipboard.SetText(Entered_Number);
                Entered_Number = "0";

                // Also clear the history display
                EnteredKeys.Clear();
                UpdateEnteredKeysOnGui();
            }
        }

        #endregion

        #region Display Formatting

        private string FormatWithGrouping(string number)
        {
            if (!IsDigitGroupingEnabled || string.IsNullOrEmpty(number) || number == "Error")
                return number;

            string cleanNumber = number.Replace(",", "");

            if (double.TryParse(cleanNumber, out double result))
            {
                bool isNegative = result < 0;
                result = Math.Abs(result);

                if (cleanNumber.Contains('.'))
                {
                    string[] parts = cleanNumber.Split('.');
                    string formattedInteger = string.Format("{0:N0}", Math.Floor(result));

                    formattedInteger = formattedInteger.Replace(".", "");
                    return (isNegative ? "-" : "") + formattedInteger + "." + parts[1];
                }

                string formatted = string.Format("{0:N0}", result);

                formatted = formatted.Replace(".", "");
                return (isNegative ? "-" : "") + formatted;
            }

            return number;
        }

        private void UpdateEnteredKeysOnGui()
        {
            KeyPressedString = string.Join("", EnteredKeys);
        }

        #endregion

        #region Math Operations

        private void Addition()
        {
            try
            {
                Number += ParseNumberInput(Entered_Number);
                Entered_Number = FormatNumberOutput(Number);
            }
            catch
            {
                Entered_Number = "Error";
            }
        }

        private void Subtraction()
        {
            try
            {
                Number -= ParseNumberInput(Entered_Number);
                Entered_Number = FormatNumberOutput(Number);
            }
            catch
            {
                Entered_Number = "Error";
            }
        }

        private void Multiplication()
        {
            try
            {
                Number *= ParseNumberInput(Entered_Number);
                Entered_Number = FormatNumberOutput(Number);
            }
            catch
            {
                Entered_Number = "Error";
            }
        }

        private void Division()
        {
            try
            {
                double divisor = ParseNumberInput(Entered_Number);
                if (divisor == 0)
                {
                    Entered_Number = "Error";
                    return;
                }
                Number /= divisor;
                Entered_Number = FormatNumberOutput(Number);
            }
            catch
            {
                Entered_Number = "Error";
            }
        }

        private void Modulo()
        {
            try
            {
                Number = ParseNumberInput(Entered_Number) % 10;
                Entered_Number = FormatNumberOutput(Number);
            }
            catch
            {
                Entered_Number = "Error";
            }
        }

        private void Power2()
        {
            try
            {
                Number = Math.Pow(ParseNumberInput(Entered_Number), 2);
                Entered_Number = FormatNumberOutput(Number);
            }
            catch
            {
                Entered_Number = "Error";
            }
        }

        private void Square_Root()
        {
            try
            {
                double value = ParseNumberInput(Entered_Number);
                if (value < 0)
                {
                    Entered_Number = "Error";
                    return;
                }
                Number = Math.Sqrt(value);
                Entered_Number = FormatNumberOutput(Number);
            }
            catch
            {
                Entered_Number = "Error";
            }
        }

        private void Reciprocal()
        {
            try
            {
                double value = ParseNumberInput(Entered_Number);
                if (value == 0)
                {
                    Entered_Number = "Error";
                    return;
                }
                Number = 1 / value;
                Entered_Number = FormatNumberOutput(Number);
            }
            catch
            {
                Entered_Number = "Error";
            }
        }

        private void Opposite()
        {
            try
            {
                Number = -1 * ParseNumberInput(Entered_Number);
                Entered_Number = FormatNumberOutput(Number);
            }
            catch
            {
                Entered_Number = "Error";
            }
        }

        // Helper method to parse input numbers, handling group separators
        private double ParseNumberInput(string input)
        {
            if (string.IsNullOrEmpty(input) || input == "Error")
                throw new ArgumentException("Invalid input");

            string cleanInput = input.Replace(",", "");
            return Convert.ToDouble(cleanInput);
        }

        // Helper method to format output numbers consistently
        private string FormatNumberOutput(double value)
        {
            // Convert to string with proper formatting
            return value.ToString("G");
        }

        private void EqualTo()
        {
            if (EnteredKeys.Count > 0)
            {
                EnteredKeys.Clear();
                EnteredKeys.Add(Entered_Number);
                UpdateEnteredKeysOnGui();
            }
            EqualToFlag = true;
        }

        #endregion

        #region Memory Operations

        private void StoreInMemory()
        {
            if (!string.IsNullOrEmpty(Entered_Number) &&
                Entered_Number != "Error" &&
                double.TryParse(Entered_Number.Replace(",", ""), out _))
            {
                MemoryValues.Add(Entered_Number.Replace(",", ""));
                SelectedMemoryIndex = MemoryValues.Count - 1;
            }
        }

        private void AddToMemory()
        {
            if (SelectedMemoryIndex >= 0 && SelectedMemoryIndex < MemoryValues.Count &&
                !string.IsNullOrEmpty(Entered_Number) &&
                Entered_Number != "Error" &&
                double.TryParse(Entered_Number.Replace(",", ""), out double number))
            {
                double selectedMemoryValue = Convert.ToDouble(MemoryValues[SelectedMemoryIndex]);
                MemoryValues[SelectedMemoryIndex] = (selectedMemoryValue + number).ToString();
            }
        }

        private void SubtractFromMemory()
        {
            if (SelectedMemoryIndex >= 0 && SelectedMemoryIndex < MemoryValues.Count &&
                !string.IsNullOrEmpty(Entered_Number) &&
                Entered_Number != "Error" &&
                double.TryParse(Entered_Number.Replace(",", ""), out double number))
            {
                double selectedMemoryValue = Convert.ToDouble(MemoryValues[SelectedMemoryIndex]);
                MemoryValues[SelectedMemoryIndex] = (selectedMemoryValue - number).ToString();
            }
        }

        private void RecallFromMemory()
        {
            if (SelectedMemoryIndex >= 0 && SelectedMemoryIndex < MemoryValues.Count)
            {
                Entered_Number = MemoryValues[SelectedMemoryIndex];

                // If we're recalling after a calculation, start fresh
                if (EqualToFlag)
                {
                    EnteredKeys.Clear();
                    FirstNumberEntered = true;
                    EqualToFlag = false;
                }
            }
        }

        #endregion

        #region Clear Operations

        private void Clear()
        {
            EnteredKeys.Clear();
            KeyPressedString = "";
            Entered_Number = "0";
            Number = 0;
            FirstNumberEntered = true;
            EqualToFlag = false;
            FunctionPressed = false;
        }

        private void ClearEntry()
        {
            if (Entered_Number != "0" && !FunctionPressed)
            {
                Entered_Number = "0";

                if (EnteredKeys.Count > 0)
                {
                    string lastKey = EnteredKeys[EnteredKeys.Count - 1];
                    if (!IsOperator(lastKey))
                    {
                        EnteredKeys.RemoveAt(EnteredKeys.Count - 1);
                        UpdateEnteredKeysOnGui();
                    }
                }
            }
        }

        private void Delete()
        {
            if (EqualToFlag)
            {
                return;
            }

            if (FunctionPressed)
            {
                return;
            }

            if (Entered_Number.Length > 1 && Entered_Number != "Error")
            {
                Entered_Number = Entered_Number.Replace(",", "");
                Entered_Number = Entered_Number.Substring(0, Entered_Number.Length - 1);
                if (Entered_Number == "" || Entered_Number == "-")
                {
                    Entered_Number = "0";
                }

                try
                {
                    Number = Convert.ToDouble(Entered_Number);
                }
                catch
                {
                    Entered_Number = "Error";
                }

                if (EnteredKeys.Count > 0 && !IsOperator(EnteredKeys[EnteredKeys.Count - 1]))
                {
                    EnteredKeys[EnteredKeys.Count - 1] = Entered_Number;
                    UpdateEnteredKeysOnGui();
                }
            }
            else
            {
                Entered_Number = "0";

                if (EnteredKeys.Count > 0 && !IsOperator(EnteredKeys[EnteredKeys.Count - 1]))
                {
                    EnteredKeys[EnteredKeys.Count - 1] = "0";
                    UpdateEnteredKeysOnGui();
                }
            }
        }

        #endregion

        #region Button Processing

        private bool IsDigit(string text)
        {
            return text == "0" || text == "1" || text == "2" || text == "3" || text == "4" ||
                   text == "5" || text == "6" || text == "7" || text == "8" || text == "9" || text == ".";
        }

        private bool IsOperator(string text)
        {
            return text == "+" || text == "-" || text == "*" || text == "/" || text == "%" || text == "=";
        }

        private bool IsFunction(string text)
        {
            return text == "1/x" || text == "x^2" || text == "sqrt" || text == "+/-";
        }

        private bool IsMemoryOperation(string text)
        {
            return text == "MS" || text == "MR" || text == "M+" || text == "M-";
        }

        private bool IsClearOperation(string text)
        {
            return text == "Clr" || text == "Ce" || text == "Del";
        }

        private bool PressedButtonIsOperator(string pressedButton)
        {
            if (IsDigit(pressedButton))
            {
                if (EqualToFlag || Entered_Number == "Error")
                {
                    Clear();
                }

                if (FunctionPressed)
                {
                    if (!IsOperator(pressedButton))
                    {
                        EnteredKeys.Add(pressedButton);
                    }
                    Entered_Number = pressedButton == "." ? "0." : pressedButton;
                    FunctionPressed = false;
                }
                else
                {
                    if (pressedButton == "." && Entered_Number.Contains("."))
                    {
                        return false;
                    }

                    if (EnteredKeys.Count == 0 || IsOperator(EnteredKeys[EnteredKeys.Count - 1]))
                    {
                        EnteredKeys.Add(pressedButton == "." ? "0." : pressedButton);
                    }
                    else
                    {
                        EnteredKeys[EnteredKeys.Count - 1] += pressedButton;
                    }

                    if (Entered_Number == "0" && pressedButton != ".")
                    {
                        Entered_Number = pressedButton;
                    }
                    else
                    {
                        Entered_Number += pressedButton;
                    }
                }

                UpdateEnteredKeysOnGui();
                PreviousEnteredKey = pressedButton;
                return false;
            }
            return true;
        }

        public void GetPressedButton(string pressedButton)
        {
            if (Entered_Number == "Error" && !IsClearOperation(pressedButton))
            {
                Clear();
            }

            pressedButton = pressedButton.Replace("ms", "MS").Replace("mr", "MR");

            // Handle clear operations
            if (pressedButton == "Clr")
            {
                Clear();
                PreviousEnteredKey = pressedButton;
                return;
            }

            if (pressedButton == "Ce")
            {
                ClearEntry();
                PreviousEnteredKey = pressedButton;
                return;
            }

            if (pressedButton == "Del")
            {
                Delete();
                PreviousEnteredKey = pressedButton;
                return;
            }

            // Handle function operations
            if (IsFunction(pressedButton))
            {
                if (pressedButton == "1/x")
                {
                    Reciprocal();
                }
                else if (pressedButton == "x^2")
                {
                    Power2();
                }
                else if (pressedButton == "sqrt")
                {
                    Square_Root();
                }
                else if (pressedButton == "+/-")
                {
                    Opposite();
                }

                // Update display with new calculated value
                if (EnteredKeys.Count > 0 && !IsOperator(EnteredKeys[EnteredKeys.Count - 1]))
                {
                    EnteredKeys[EnteredKeys.Count - 1] = Entered_Number;
                }
                else
                {
                    EnteredKeys.Add(Entered_Number);
                }

                UpdateEnteredKeysOnGui();
                PreviousEnteredKey = pressedButton;
                return;
            }

            // Handle memory operations
            if (IsMemoryOperation(pressedButton))
            {
                switch (pressedButton)
                {
                    case "MS":
                        StoreInMemory();
                        break;
                    case "MR":
                        RecallFromMemory();
                        break;
                    case "M+":
                        AddToMemory();
                        break;
                    case "M-":
                        SubtractFromMemory();
                        break;
                }

                PreviousEnteredKey = pressedButton;
                return;
            }

            // Handle number and operator buttons
            if (PressedButtonIsOperator(pressedButton))
            {
                if (IsOperator(PreviousEnteredKey) && IsOperator(pressedButton))
                {
                    if (EnteredKeys.Count > 0)
                    {
                        EnteredKeys[EnteredKeys.Count - 1] = pressedButton;
                        UpdateEnteredKeysOnGui();
                    }

                    switch (pressedButton)
                    {
                        case "+": SelectedFunction = "Addition"; break;
                        case "-": SelectedFunction = "Subtraction"; break;
                        case "*": SelectedFunction = "Multiplication"; break;
                        case "/": SelectedFunction = "Division"; break;
                        case "%": SelectedFunction = "Modulo"; break;
                        case "=": SelectedFunction = "EqualTo"; break;
                    }

                    PreviousEnteredKey = pressedButton;
                    return;
                }

                // Handle the first number in the calculation
                if (FirstNumberEntered)
                {
                    try
                    {
                        Number = Convert.ToDouble(Entered_Number.Replace(",", ""));
                        FirstNumberEntered = false;
                    }
                    catch
                    {
                        Entered_Number = "Error";
                        return;
                    }
                }
                else
                {
                    switch (SelectedFunction)
                    {
                        case "Addition": Addition(); break;
                        case "Subtraction": Subtraction(); break;
                        case "Multiplication": Multiplication(); break;
                        case "Division": Division(); break;
                        case "Modulo": Modulo(); break;
                    }
                }

                // Update displayed calculation
                if (pressedButton == "=")
                {
                    EqualTo();
                    SelectedFunction = "";
                    FirstNumberEntered = true;
                }
                else
                {
                    // Store the operator and prepare for the next number
                    switch (pressedButton)
                    {
                        case "+":
                            SelectedFunction = "Addition";
                            EnteredKeys.Add(pressedButton);
                            break;
                        case "-":
                            SelectedFunction = "Subtraction";
                            EnteredKeys.Add(pressedButton);
                            break;
                        case "*":
                            SelectedFunction = "Multiplication";
                            EnteredKeys.Add(pressedButton);
                            break;
                        case "/":
                            SelectedFunction = "Division";
                            EnteredKeys.Add(pressedButton);
                            break;
                        case "%":
                            SelectedFunction = "Modulo";
                            EnteredKeys.Add(pressedButton);
                            break;
                    }
                    UpdateEnteredKeysOnGui();
                }

                FunctionPressed = true;
                PreviousEnteredKey = pressedButton;
            }
        }

        #endregion
    }
}