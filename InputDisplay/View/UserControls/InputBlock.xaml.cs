using InputDisplay.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserControl = System.Windows.Controls.UserControl;

namespace InputDisplay.View.UserControls
{
    /// <summary>
    /// Interaction logic for InputBlock.xaml
    /// </summary>
    public partial class InputBlock : UserControl
    {

        public InputBlock()
        {
            InitializeComponent();
            DataContext = new InputStackComponent();
        }
    }
}
