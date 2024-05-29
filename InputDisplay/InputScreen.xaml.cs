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

namespace InputDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class InputScreen : Window
    {

        public InputScreen()
        {
            InitializeComponent();
            //InputInterpreter.Initialize();
            //InputProcessor.FinishSetup(this);
            //InputInterpreter.DisplayInputs += UpdateText;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }
    }
}