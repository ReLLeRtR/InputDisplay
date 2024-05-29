using InputDisplay.DeviceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InputDisplay
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RidDeviceInfoHeader
    {
        public uint cbSize;
        public uint dwType;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct RidDeviceInfoMouse
    {
        public RidDeviceInfoHeader header;
        public uint dwId;
        public uint dwNumberOfButtons;
        public uint dwSampleRate;
        public bool fHasHorizontalWheel;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RidDeviceInfoKeyboard
    {
        public RidDeviceInfoHeader header;
        public uint dwType;
        public uint dwSubType;
        public uint dwKeyboardMode;
        public uint dwNumberOfFunctionKeys;
        public uint dwNumberOfIndicators;
        public uint dwNumberOfKeysTotal;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RidDeviceInfoHid
    {
        public RidDeviceInfoHeader header;
        public uint dwVendorId;
        public uint dwProductId;
        public uint dwVersionNumber;
        public ushort usUsagePage;
        public ushort usUsage;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct RawInputHeader
    {
        public uint dwType;                     // Type of raw input (RIM_TYPEHID 2, RIM_TYPEKEYBOARD 1, RIM_TYPEMOUSE 0)
        public uint dwSize;                     // Size in bytes of the entire input packet of data. This includes RAWINPUT plus possible extra input reports in the RAWHID variable length array. 
        public IntPtr hDevice;                  // A handle to the device generating the raw input data. 
        public IntPtr wParam;                   // RIM_INPUT 0 if input occurred while application was in the foreground else RIM_INPUTSINK 1 if it was not.

        public override string ToString()
        {
            return string.Format("RawInputHeader\n dwType : {0}\n dwSize : {1}\n hDevice : {2}\n wParam : {3}", dwType, dwSize, hDevice, wParam);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RawHid
    {
        public uint dwSizeHid;
        public uint dwCount;
        public byte bRawData;

        public override string ToString()
        {
            return string.Format("Rawhib\n dwSizeHid : {0}\n dwCount : {1}\n bRawData : {2}\n", dwSizeHid, dwCount, bRawData);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct RawMouse
    {
        [FieldOffset(0)]
        public ushort usFlags;
        [FieldOffset(4)]
        public uint ulButtons;
        [FieldOffset(4)]
        public US_BUTTON_FLAGS usButtonFlags;
        [FieldOffset(6)]
        public ushort usButtonData;
        [FieldOffset(8)]
        public uint ulRawButtons;
        [FieldOffset(12)]
        public int lLastX;
        [FieldOffset(16)]
        public int lLastY;
        [FieldOffset(20)]
        public uint ulExtraInformation;
    }
    [Flags]
    public enum US_BUTTON_FLAGS : ushort
    {
        RI_MOUSE_LEFT_BUTTON_DOWN = 0x0001,
        RI_MOUSE_LEFT_BUTTON_UP =   0x0002,
        RI_MOUSE_RIGHT_BUTTON_DOWN = 0x0004,
        RI_MOUSE_RIGHT_BUTTON_UP = 0x0008,
        RI_MOUSE_MIDDLE_BUTTON_DOWN = 0x0010,
        RI_MOUSE_MIDDLE_BUTTON_UP = 0x0020,
        RI_MOUSE_BUTTON_4_DOWN = 0x0040,
        RI_MOUSE_BUTTON_4_UP = 0x0080,
        RI_MOUSE_BUTTON_5_DOWN = 0x0100,
        RI_MOUSE_BUTTON_5_UP = 0x0200,
        RI_MOUSE_WHEEL = 0x0400,
        RI_MOUSE_HWHEEL = 0x0800,
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct RawKeyboard
    {
        public ushort MakeCode;                 // Scan code from the key depression
        public ushort Flags;                    // One or more of RI_KEY_MAKE, RI_KEY_BREAK, RI_KEY_E0, RI_KEY_E1
        private readonly ushort Reserved;       // Always 0    
        public ushort VKey;                     // Virtual Key Code
        public uint Message;                    // Corresponding Windows message for exmaple (WM_KEYDOWN, WM_SYASKEYDOWN etc)
        public uint ExtraInformation;           // The device-specific addition information for the event (seems to always be zero for keyboards)

        public override string ToString()
        {
            return string.Format("Rawkeyboard\n MakeCode: {0}\n Makecode(hex) : {0:X}\n Flags: {1}\n Reserved: {2}\n VKeyName: {3}\n Message: {4}\n ExtraInformation {5}\n",
                                                MakeCode, Flags, Reserved, VKey, Message, ExtraInformation);
        }
    }

    internal enum RawInputDeviceInfo : uint
    {
        RIDI_DEVICENAME = 0x20000007,
        RIDI_DEVICEINFO = 0x2000000b,
        PREPARSEDDATA = 0x20000005
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputDeviceList
    {
        public IntPtr hDevice;
        public uint dwType;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InputDataHid
    {
        public RawInputHeader header;           // 64 bit header size: 24  32 bit the header size: 16
        public RawHid data;                    // Creating the rest in a struct allows the header size to align correctly for 32/64 bit
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct InputDataKeyboard
    {
        public RawInputHeader header;           // 64 bit header size: 24  32 bit the header size: 16
        public RawKeyboard data;                    // Creating the rest in a struct allows the header size to align correctly for 32/64 bit
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct InputDataMouse
    {
        public RawInputHeader header;           // 64 bit header size: 24  32 bit the header size: 16
        public RawMouse data;                    // Creating the rest in a struct allows the header size to align correctly for 32/64 bit
    }

}
