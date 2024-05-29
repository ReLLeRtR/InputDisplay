using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Cryptography.Xml;
using System.IO;
using System.Reflection;
using Microsoft.VisualBasic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static InputDisplay.DeviceManagement.DeviceInfo;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using InputDisplay.Display;

namespace InputDisplay.DeviceManagement
{
    public enum DeviceType
    {
        RimTypeMouse = 0,
        RimTypeKeyboard = 1,
        RimTypeHid = 2
    }


    public class DeviceInfo : IComparable<DeviceInfo>
    {
        public string name;
        public string path;
        public DeviceType type;
        public int numberOfButtons;
        public IntPtr preparsedData;
        public HIDP_CAPS capabilities;
        public HIDP_BUTTON_CAPS buttonCaps;
        public HIDP_VALUE_CAPS[] valueCaps;
        public DisplayPreset preset;

        public DeviceInfo(IntPtr input,DeviceType type, string name, string path)
        {
            this.name = name;
            this.path = path;
            this.type = type;
            capabilities = default(HIDP_CAPS);
            switch (type)
            {
                case DeviceType.RimTypeHid:
                    CreateHid(input);
                    preset = PresetsManager.DefaultDualsense;
                    break;
                case DeviceType.RimTypeKeyboard:
                    preset = PresetsManager.DefaultKeyboard;
                    break;
                case DeviceType.RimTypeMouse:
                    preset = PresetsManager.DefaultMouse;
                    break;
            }

            //File.WriteAllText(@".\DeviceConfigs\DefaultKeyboard.json", "so fucking true");
        }

        private void CreateHid(IntPtr input)
        {
            uint pcbSize = 0;
            NativeMethods.GetRawInputDeviceInfo(input, RawInputDeviceInfo.PREPARSEDDATA, IntPtr.Zero, ref pcbSize);
            if (pcbSize <= 0)
                return;

            preparsedData = Marshal.AllocHGlobal((int)pcbSize);
            NativeMethods.GetRawInputDeviceInfo(input, RawInputDeviceInfo.PREPARSEDDATA, preparsedData, ref pcbSize);


            NativeMethods.HidP_GetCaps(preparsedData, ref capabilities);

            short capsLength;
            byte[] buttonCapsByt;

            if (capabilities.NumberInputButtonCaps > 0)
            {
                //get button caps
                capsLength = capabilities.NumberInputButtonCaps;
                buttonCapsByt = new byte[capsLength * 72];
                NativeMethods.HidP_GetButtonCaps(0, buttonCapsByt, ref capsLength, preparsedData);
                buttonCaps = Unsafe.As<byte, HIDP_BUTTON_CAPS>(ref buttonCapsByt[0]);
                numberOfButtons = buttonCaps.UsageMax - buttonCaps.UsageMin + 1;
            }

            if (capabilities.NumberInputValueCaps > 0)
            {
                //get value caps
                capsLength = capabilities.NumberInputValueCaps;
                buttonCapsByt = new byte[capsLength * 72];
                NativeMethods.HidP_GetValueCaps(0, buttonCapsByt, ref capsLength, preparsedData);
                Span<HIDP_VALUE_CAPS> pValueCapsSpan = MemoryMarshal.Cast<byte, HIDP_VALUE_CAPS>(buttonCapsByt);
                valueCaps = pValueCapsSpan.ToArray();
            }
        }

        public bool IsUsageValid(int buttonID, int usageValue, out List<UsageConfig> targetValues)
        {
            targetValues = new();
            //Debug.WriteLine(buttonID);
            foreach (UsageConfig conf in preset.usageConfigs.Where(x => x.buttonID == buttonID))
            {
                if (conf.ConditionMet(usageValue) & !conf.displayType.HasFlag(UsageDisplayType.Ignore))
                    targetValues.Add(conf);
            }
            return targetValues.Count > 0;
        }

        public bool PushRequired(int buttonID, bool released)
        {
            if (preset.usageConfigs.Length <= buttonID)
                return false;
            return released | preset.usageConfigs[buttonID].displayType.HasFlag(UsageDisplayType.OnPress);
        }

        public int CompareTo(DeviceInfo? other)
        {
            if (preset.displayPriority < other.preset.displayPriority)
                return -1;
            if (preset.displayPriority > other.preset.displayPriority)
                return 1;

            return 0;
        }
    }
}
