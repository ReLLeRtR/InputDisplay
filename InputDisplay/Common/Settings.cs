using InputDisplay.DeviceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace InputDisplay
{
    public static class Settings
    {
        public static bool showAdditions = true, showDeviceIcons = false;
        public static UsageConfig addition = new() { name = "+" };
    }
}
