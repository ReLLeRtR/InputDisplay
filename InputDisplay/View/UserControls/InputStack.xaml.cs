using InputDisplay.DeviceManagement;
using InputDisplay.ViewModel;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
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
    /// Interaction logic for InputStack.xaml
    /// </summary>
    public partial class InputStack : UserControl
    {
        private List<InputStackComponent> stackChildren = new();
        private bool isMain;
        public bool IsMain
        {
            set
            {
                if (isMain == value)
                    return;
                isMain = value;
                Height = value ? 50 : 35;
                Opacity = value ? 1 : 0.75f;
                foreach(var child in stackChildren)
                    child.IsMain = value;
            }
        }
        public void AddInput(UsageConfig usage, bool isInput)
        {
            InputBlock block = Pools.pooledBlocks.Unpool();
            InputStackComponent context = (InputStackComponent)block.DataContext;
            context.Usage = usage;
            context.IsMain = true;
            context.IsInput = isInput;
            inputPanel.Children.Add(block);
            stackChildren.Add(context);
        }

        public void Clear()
        {
            foreach (var child in inputPanel.Children)
                Pools.pooledBlocks.Enqueue((InputBlock)child);
            inputPanel.Children.Clear();
            stackChildren.Clear();
        }

        public InputStack()
        {
            InitializeComponent();
        }

        private void InputBlock_TextInput(object sender, TextCompositionEventArgs e)
        {

        }
    }
}
