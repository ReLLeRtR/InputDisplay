using InputDisplay.DeviceManagement;
using InputDisplay.View.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using InputDisplay.Display;

namespace InputDisplay
{
    public delegate void InputReceivedEvent(IntPtr hDevice);
    public delegate void InputProcessedEvent(DeviceInfo device, List<UsageConfig> keyValue, bool pushRequired, bool keyReleased);
    public delegate void RegisterDevicesEvent(Dictionary<IntPtr, DeviceInfo> deviceList);
    public delegate void DisplayInputEvent(List<KeyValuePair<UsageConfig, bool>> interpretations, bool push);

    public static class Common
    {
        public const string KeyboardConfigPath = @".\DeviceConfigs\DefaultKeyboard\";
        public const string MouseConfigPath = @".\DeviceConfigs\DefaultMouse\";
        public const string DualsenseConfigPath = @".\DeviceConfigs\DefaultDualsense\";
        public const string ConfigFileName = @"config.json";
        public const string ImageFolderName = @"Icons\";
        public const string DeviceIconIndicator = "DEVICE ICON PURPOSES ONLY";

        public static InputReceivedEvent InputReceived;
        public static InputProcessedEvent InputProcessed;
        public static RegisterDevicesEvent DevicesRegistered;
        public static DisplayInputEvent DisplayInputs;
    }

    public static class Pools
    {
        public static Queue<InputBlock> pooledBlocks = new();
    }
}
