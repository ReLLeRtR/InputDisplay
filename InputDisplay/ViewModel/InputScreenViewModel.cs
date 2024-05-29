using System;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections.Generic;
using InputDisplay.View.UserControls;
using InputDisplay.DeviceManagement;
using InputDisplay.InputProcessing;
using InputDisplay.ViewModel;
using InputDisplay;

namespace InputDisplay.ViewModel
{
    internal class InputScreenViewModel : ViewModelBase
    {
        private List<InputStack> inputStacks = new();
        public InputScreen window;
        private StackPanel inputStacksPanel;
        private int maxInputSteps = 3;
        public int MaxInputSteps
        {
            get
            {
                return maxInputSteps;
            }
            set
            {
                maxInputSteps = value;
            }
        }


        public InputScreenViewModel()
        {
            window = new InputScreen();
            window.Show();
            inputStacksPanel = window.inputStacksPanel;
            Common.DisplayInputs += UpdateText;
        }

        private void UpdateText(List<KeyValuePair<UsageConfig, bool>> interpretations, bool push)
        {
            if (3 != inputStacks.Count)
                MatchLength(3);
            InputStack stack = inputStacks[0];
            if (push)
            {
                inputStacks = inputStacks.Shift(1);
                stack = inputStacks[0];
                inputStacksPanel.Children.Remove(stack);
                inputStacksPanel.Children.Insert(inputStacks.Count - 1, stack);
                stack.IsMain = true;
                for (int i = 1; i < inputStacks.Count; i++)
                    inputStacks[i].IsMain = false;
            }
            stack.Clear();
            foreach (var interp in interpretations)
                stack.AddInput(interp.Key, interp.Value);
        }

        private void MatchLength(int count)
        {
            while (count > inputStacks.Count)
            {
                InputStack stack = new InputStack();
                inputStacksPanel.Children.Insert(0, stack);
                inputStacks.Add(stack);
                stack.IsMain = false;

            }
            while (count < inputStacks.Count)
            {
                InputStack stack = inputStacks[count];
                inputStacksPanel.Children.Remove(stack);
                inputStacks.RemoveAt(count);
            }
            inputStacks[0].IsMain = true;
        }
    }
}
