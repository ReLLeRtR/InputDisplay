using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using InputDisplay;
using Microsoft.Win32.SafeHandles;

namespace InputDisplay
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DEV_BROADCAST_DEVICE_INTERFACE
    {
        public int Size;
        public int DeviceType;
        public int Reserved;
        public Guid ClassGuid;
        public short Name;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct SP_DEVICE_INTERFACE_DATA
    {
        internal int cbSize;
        internal System.Guid InterfaceClassGuid;
        internal int Flags;
        internal IntPtr Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SP_DEVINFO_DATA
    {
        internal int cbSize;
        internal Guid ClassGuid;
        internal int DevInst;
        internal IntPtr Reserved;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct SP_DEVICE_INTERFACE_DETAIL_DATA
    {
        internal int Size;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        internal string DevicePath;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DEVPROPKEY
    {
        public Guid fmtid;
        public ulong pid;
    }
    [Flags]
    internal enum DEVPROPTYPE : ulong
    {
        DEVPROP_TYPEMOD_ARRAY = 0x00001000,
        DEVPROP_TYPEMOD_LIST = 0x00002000,

        DEVPROP_TYPE_EMPTY = 0x00000000,  // nothing, no property data
        DEVPROP_TYPE_NULL = 0x00000001,  // null property data
        DEVPROP_TYPE_SBYTE = 0x00000002,  // 8-bit signed int (SBYTE)
        DEVPROP_TYPE_BYTE = 0x00000003,  // 8-bit unsigned int (BYTE)
        DEVPROP_TYPE_INT16 = 0x00000004,  // 16-bit signed int (SHORT)
        DEVPROP_TYPE_UINT16 = 0x00000005,  // 16-bit unsigned int (USHORT)
        DEVPROP_TYPE_INT32 = 0x00000006,  // 32-bit signed int (LONG)
        DEVPROP_TYPE_UINT32 = 0x00000007,  // 32-bit unsigned int (ULONG)
        DEVPROP_TYPE_INT64 = 0x00000008,  // 64-bit signed int (LONG64)
        DEVPROP_TYPE_UINT64 = 0x00000009,  // 64-bit unsigned int (ULONG64)
        DEVPROP_TYPE_FLOAT = 0x0000000A,  // 32-bit floating-point (FLOAT)
        DEVPROP_TYPE_DOUBLE = 0x0000000B,  // 64-bit floating-point (DOUBLE)
        DEVPROP_TYPE_DECIMAL = 0x0000000C,  // 128-bit data (DECIMAL)
        DEVPROP_TYPE_GUID = 0x0000000D,  // 128-bit unique identifier (GUID)
        DEVPROP_TYPE_CURRENCY = 0x0000000E,  // 64 bit signed int currency value (CURRENCY)
        DEVPROP_TYPE_DATE = 0x0000000F,  // date (DATE)
        DEVPROP_TYPE_FILETIME = 0x00000010,  // filetime (FILETIME)
        DEVPROP_TYPE_BOOLEAN = 0x00000011,  // 8-bit boolean (DEVPROP_BOOLEAN)
        DEVPROP_TYPE_STRING = 0x00000012,  // null-terminated string
        DEVPROP_TYPE_STRING_LIST = (DEVPROP_TYPE_STRING | DEVPROP_TYPEMOD_LIST), // multi-sz string list
        DEVPROP_TYPE_SECURITY_DESCRIPTOR = 0x00000013,  // self-relative binary SECURITY_DESCRIPTOR
        DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING = 0x00000014,  // security descriptor string (SDDL format)
        DEVPROP_TYPE_DEVPROPKEY = 0x00000015,  // device property key (DEVPROPKEY)
        DEVPROP_TYPE_DEVPROPTYPE = 0x00000016,  // device property type (DEVPROPTYPE)
        DEVPROP_TYPE_BINARY = (DEVPROP_TYPE_BYTE | DEVPROP_TYPEMOD_ARRAY),  // custom binary data
        DEVPROP_TYPE_ERROR = 0x00000017,  // 32-bit Win32 system error code
        DEVPROP_TYPE_NTSTATUS = 0x00000018, // 32-bit NTSTATUS code
        DEVPROP_TYPE_STRING_INDIRECT = 0x00000019, // string resource (@[path\]<dllname>,-<strId>)

        MAX_DEVPROP_TYPE = 0x00000019,
        MAX_DEVPROP_TYPEMOD = 0x00002000,

        DEVPROP_MASK_TYPE = 0x00000FFF,
        DEVPROP_MASK_TYPEMOD = 0x0000F000
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public IntPtr lpSecurityDescriptor;
        public bool bInheritHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SP_CLASSINSTALL_HEADER
    {
        internal int cbSize;
        internal int installFunction;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SP_PROPCHANGE_PARAMS
    {
        internal SP_CLASSINSTALL_HEADER classInstallHeader;
        internal int stateChange;
        internal int scope;
        internal int hwProfile;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HIDP_BUTTON_CAPS
    {
        public short UsagePage; //16
        public byte ReportID; //24
        public bool IsAlias; //32
        public short BitField; //48
        public short LinkCollection; //64
        public short LinkUsage; //80
        public short LinkUsagePage; //96
        public bool IsRange; //104
        public bool IsStringRange; //112
        public bool IsDesignatorRange; //120
        public bool IsAbsolute; //128
        public short ReportCount; //144
        public short Reserved1; //160
        public int Reserved2; //192
        public int Reserved3; //224
        public int Reserved4; //256
        public int Reserved5; //288
        public int Reserved6; //320
        public int Reserved7; //352
        public int Reserved8; //384
        public int Reserved9; //416
        public int Reserved10; //448
        public short UsageMin; //464
        public short UsageMax; //480
        public short StringMin; //16
        public short StringMax; //16
        public short DesignatorMin;
        public short DesignatorMax; //16
        public short DataIndexMin; //16
        public short DataIndexMax; //16
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HIDP_VALUE_CAPS
    {
        public short UsagePage; //16
        public byte ReportID; //24
        public bool IsAlias; //32
        public short BitField; //48
        public short LinkCollection; //64
        public short LinkUsage; //80
        public short LinkUsagePage; //96
        public bool IsRange; //104
        public bool IsStringRange; //112
        public bool IsDesignatorRange; //120
        public bool IsAbsolute; //128

        public bool HasNull; //136
        public byte Reserved; //144
        public short BitSize; //160

        public short ReportCount; //176
        public short Reserved1; //192
        public short Reserved2; //208
        public short Reserved3; //224
        public short Reserved4; //240
        public short Reserved5; //256

        public int UnitsExp; //288
        public int Units; //320

        public int LogicalMin; //352
        public int LogicalMax; //384
        public int PhysicalMin; //416
        public int PhysicalMax; //448

        public short UsageMin; //464
        public short UsageMax; //480
        public short StringMin; //16
        public short StringMax; //16
        public short DesignatorMin;
        public short DesignatorMax;
        public short DataIndexMin;
        public short DataIndexMax;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HIDP_CAPS
    {
        public ushort Usage;
        public ushort UsagePage;
        public short InputReportByteLength;
        public short OutputReportByteLength;
        public short FeatureReportByteLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public short[] Reserved;
        public short NumberLinkCollectionNodes;
        public short NumberInputButtonCaps;
        public short NumberInputValueCaps;
        public short NumberInputDataIndices;
        public short NumberOutputButtonCaps;
        public short NumberOutputValueCaps;
        public short NumberOutputDataIndices;
        public short NumberFeatureButtonCaps;
        public short NumberFeatureValueCaps;
        public short NumberFeatureDataIndices;
    }

    [SuppressUnmanagedCodeSecurity]
    internal static class NativeMethods
    {

        internal const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        internal const int FILE_FLAG_OVERLAPPED = 0x40000000;
        internal const uint FILE_FLAG_NO_BUFFERING = 0x20000000;
        internal const uint FILE_FLAG_WRITE_THROUGH = 0x80000000;
        internal const uint FILE_ATTRIBUTE_TEMPORARY = 0x100;

        internal const short FILE_SHARE_READ = 0x1;
        internal const short FILE_SHARE_WRITE = 0x2;
        internal const uint GENERIC_READ = 0x80000000;
        internal const uint GENERIC_WRITE = 0x40000000;
        internal const Int32 FileShareRead = 1;
        internal const Int32 FileShareWrite = 2;
        internal const Int32 OpenExisting = 3;
        internal const int ACCESS_NONE = 0;
        internal const int INVALID_HANDLE_VALUE = -1;
        internal const short OPEN_EXISTING = 3;
        internal const int WAIT_TIMEOUT = 0x102;
        internal const uint WAIT_OBJECT_0 = 0;
        internal const uint WAIT_FAILED = 0xffffffff;

        internal const int WM_KEYDOWN = 0x0100;
        internal const int WM_KEYUP = 0x0101;
        internal const int WM_SYSKEYDOWN = 0x0104;
        internal const int WM_INPUT = 0x00FF;
        internal const int WM_DEVICECHANGE = 0x0219;

        internal const int DBT_DEVNODES_CHANGED = 0x0007;
        internal const int DBT_DEVICEARRIVAL = 0x8000;
        internal const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        internal const int DBT_DEVTYP_DEVICEINTERFACE = 5;

        internal const short DIGCF_DEFAULT = 0x1;
        internal const short DIGCF_PRESENT = 0x2;
        internal const short DIGCF_PROFILE = 0x8;
        internal const short DIGCF_DEVICEINTERFACE = 0x10;
        internal const int DIGCF_ALLCLASSES = 0x4;
        internal const int DICS_ENABLE = 1;
        internal const int DICS_DISABLE = 2;
        internal const int DICS_FLAG_GLOBAL = 1;
        internal const int DIF_PROPERTYCHANGE = 0x12;

        internal const int MAX_DEV_LEN = 1000;
        internal const int SPDRP_ADDRESS = 0x1c;
        internal const int SPDRP_BUSNUMBER = 0x15;
        internal const int SPDRP_BUSTYPEGUID = 0x13;
        internal const int SPDRP_CAPABILITIES = 0xf;
        internal const int SPDRP_CHARACTERISTICS = 0x1b;
        internal const int SPDRP_CLASS = 7;
        internal const int SPDRP_CLASSGUID = 8;
        internal const int SPDRP_COMPATIBLEIDS = 2;
        internal const int SPDRP_CONFIGFLAGS = 0xa;
        internal const int SPDRP_DEVICE_POWER_DATA = 0x1e;
        internal const int SPDRP_DEVICEDESC = 0;
        internal const int SPDRP_DEVTYPE = 0x19;
        internal const int SPDRP_DRIVER = 9;
        internal const int SPDRP_ENUMERATOR_NAME = 0x16;
        internal const int SPDRP_EXCLUSIVE = 0x1a;
        internal const int SPDRP_FRIENDLYNAME = 0xc;
        internal const int SPDRP_HARDWAREID = 1;
        internal const int SPDRP_LEGACYBUSTYPE = 0x14;
        internal const int SPDRP_LOCATION_INFORMATION = 0xd;
        internal const int SPDRP_LOWERFILTERS = 0x12;
        internal const int SPDRP_MFG = 0xb;
        internal const int SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 0xe;
        internal const int SPDRP_REMOVAL_POLICY = 0x1f;
        internal const int SPDRP_REMOVAL_POLICY_HW_DEFAULT = 0x20;
        internal const int SPDRP_REMOVAL_POLICY_OVERRIDE = 0x21;
        internal const int SPDRP_SECURITY = 0x17;
        internal const int SPDRP_SECURITY_SDS = 0x18;
        internal const int SPDRP_SERVICE = 4;
        internal const int SPDRP_UI_NUMBER = 0x10;
        internal const int SPDRP_UI_NUMBER_DESC_FORMAT = 0x1d;

        internal const int WAIT_INFINITE = 0xffff;

        [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
        static internal extern uint CM_Get_Device_Interface_Property(string pszDeviceInterface, ref DEVPROPKEY PropertyKey, out DEVPROPTYPE PropertyType, byte[] PropertyBuffer, ref uint PropertyBufferSize, uint ulFlags);

        [DllImport("hid.dll")]
        static internal extern bool HidD_GetPreparsedData(IntPtr hidDeviceObject, ref IntPtr preparsedData);

        [DllImport("hid.dll")]
        static internal extern bool HidD_FreePreparsedData(IntPtr preparsedData);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static internal extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode, ref SECURITY_ATTRIBUTES lpSecurityAttributes, int dwCreationDisposition, uint dwFlagsAndAttributes, int hTemplateFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern SafeFileHandle CreateFile(String lpFileName, UInt32 dwDesiredAccess, Int32 dwShareMode, IntPtr lpSecurityAttributes, Int32 dwCreationDisposition, UInt32 dwFlagsAndAttributes, Int32 hTemplateFile);

        [DllImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceRegistryProperty")]
        public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, int propertyVal, ref int propertyRegDataType, byte[] propertyBuffer, int propertyBufferSize, ref int requiredSize);

        [DllImport("setupapi.dll", EntryPoint = "SetupDiGetDevicePropertyW", SetLastError = true)]
        public static extern bool SetupDiGetDeviceProperty(IntPtr deviceInfo, ref SP_DEVINFO_DATA deviceInfoData, ref DEVPROPKEY propkey, ref ulong propertyDataType, byte[] propertyBuffer, int propertyBufferSize, ref int requiredSize, uint flags);

        [DllImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceInterfacePropertyW", SetLastError = true)]
        public static extern bool SetupDiGetDeviceInterfaceProperty(IntPtr deviceInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            ref DEVPROPKEY propkey, ref ulong propertyDataType, byte[] propertyBuffer, int propertyBufferSize, ref int requiredSize, uint flags);

        [DllImport("setupapi.dll")]
        static internal extern bool SetupDiEnumDeviceInfo(IntPtr deviceInfoSet, int memberIndex, ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static internal extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr notificationFilter, Int32 flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint UnregisterDeviceNotification(IntPtr hHandle);

        [DllImport("setupapi.dll")]
        internal static extern int SetupDiCreateDeviceInfoList(ref Guid classGuid, int hwndParent);

        [DllImport("setupapi.dll")]
        internal static extern IntPtr SetupDiCreateDeviceInfoList(IntPtr guid, int hwndParent);

        [DllImport("setupapi.dll")]
        static internal extern int SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        [DllImport("setupapi.dll")]
        internal static extern bool SetupDiOpenDeviceInfo(IntPtr deviceInfoSet, string deviceInstanceId, IntPtr parent, uint flags, ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll")]
        static internal extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, ref Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll")]
        static internal extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr deviceInfoData, ref Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        static internal extern IntPtr SetupDiGetClassDevs(ref System.Guid classGuid, string enumerator, int hwndParent, int flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        static internal extern IntPtr SetupDiGetClassDevs(IntPtr classGuid, string enumerator, int hwndParent, int flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, EntryPoint = "SetupDiGetDeviceInterfaceDetail")]
        static internal extern bool SetupDiGetDeviceInterfaceDetailBuffer(IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, IntPtr deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        static internal extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, IntPtr deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        static internal extern bool SetupDiSetClassInstallParams(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, ref SP_PROPCHANGE_PARAMS classInstallParams, int classInstallParamsSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        static internal extern bool SetupDiCallClassInstaller(int installFunction, IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        static internal extern bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, char[] deviceInstanceId, Int32 deviceInstanceIdSize, ref int requiredSize);

        [DllImport("setupapi.dll", SetLastError = true)]
        static internal extern bool SetupDiClassGuidsFromName(string ClassName, ref Guid ClassGuidArray1stItem, UInt32 ClassGuidArraySize, out UInt32 RequiredSize);


        [DllImport("hid.dll")]
        static internal extern int HidP_GetUsages(short reportType, short usagePage, short link, [Out] short[] usages, [In, Out] ref int usageLength, IntPtr preparsedData, [Out] byte[] report, uint reportLength);

        [DllImport("hid.dll")]
        static internal extern int HidP_GetUsageValue(short reportType, short usagePage, short link, short usage, [Out] out byte usageValue, IntPtr preparsedData, [Out] byte[] report, uint reportLength);

        [DllImport("hid.dll")]
        static internal extern int HidP_GetCaps(IntPtr preparsedData, ref HIDP_CAPS capabilities);

        [DllImport("hid.dll")]
        static internal extern int HidP_GetButtonCaps(short reportType, [Out] byte[] buttonCaps, [In, Out] ref short buttonCapsLength, IntPtr preparsedData);

        [DllImport("hid.dll")]
        static internal extern int HidP_GetValueCaps(short reportType, [Out] byte[] valueCaps, [In, Out] ref short valueCapsLength, IntPtr preparsedData);

        [DllImport("User32.dll", SetLastError = true)]
        internal static extern uint GetRawInputDeviceInfo(IntPtr hDevice, RawInputDeviceInfo command, IntPtr pData, ref uint size);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref uint numberDevices, uint size);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool RegisterRawInputDevices(RawInputDevice[] pRawInputDevice, uint numberDevices, uint size);

        [DllImport("User32.dll")]   // no SetLastError documented for this function
        internal static extern int GetRawInputData(IntPtr hRawInput, DataCommand command, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] pData, [In, Out] ref int size, int sizeHeader);

        public enum DataCommand : uint
        {
            RID_HEADER = 0x10000005, // Get the header information from the RAWINPUT structure.
            RID_INPUT = 0x10000003   // Get the raw data from the RAWINPUT structure.
        }

        [Flags]
        internal enum RawInputDeviceFlags
        {
            NONE = 0,                   // No flags
            REMOVE = 0x00000001,        // Removes the top level collection from the inclusion list. This tells the operating system to stop reading from a device which matches the top level collection. 
            EXCLUDE = 0x00000010,       // Specifies the top level collections to exclude when reading a complete usage page. This flag only affects a TLC whose usage page is already specified with PageOnly.
            PAGEONLY = 0x00000020,      // Specifies all devices whose top level collection is from the specified UsagePage. Note that Usage must be zero. To exclude a particular top level collection, use Exclude.
            NOLEGACY = 0x00000030,      // Prevents any devices specified by UsagePage or Usage from generating legacy messages. This is only for the mouse and keyboard.
            INPUTSINK = 0x00000100,     // Enables the caller to receive the input even when the caller is not in the foreground. Note that WindowHandle must be specified.
            CAPTUREMOUSE = 0x00000200,  // Mouse button click does not activate the other window.
            NOHOTKEYS = 0x00000200,     // Application-defined keyboard device hotkeys are not handled. However, the system hotkeys; for example, ALT+TAB and CTRL+ALT+DEL, are still handled. By default, all keyboard hotkeys are handled. NoHotKeys can be specified even if NoLegacy is not specified and WindowHandle is NULL.
            APPKEYS = 0x00000400,       // Application keys are handled.  NoLegacy must be specified.  Keyboard only.

            // Enables the caller to receive input in the background only if the foreground application does not process it. 
            // In other words, if the foreground application is not registered for raw input, then the background application that is registered will receive the input.
            EXINPUTSINK = 0x00001000,
            DEVNOTIFY = 0x00002000
        }

        public enum HidUsagePage : ushort
        {
            UNDEFINED = 0x00,   // Unknown usage page
            GENERIC = 0x01,     // Generic desktop controls
            SIMULATION = 0x02,  // Simulation controls
            VR = 0x03,          // Virtual reality controls
            SPORT = 0x04,       // Sports controls
            GAME = 0x05,        // Games controls
            KEYBOARD = 0x07,    // Keyboard controls
        }

        public enum HidUsage : ushort
        {
            Undefined = 0x00,       // Unknown usage
            Pointer = 0x01,         // Pointer
            Mouse = 0x02,           // Mouse
            Joystick = 0x04,        // Joystick
            Gamepad = 0x05,         // Game Pad
            Keyboard = 0x06,        // Keyboard
            Keypad = 0x07,          // Keypad
            MultiAxis = 0x80,   // Multi-axis Controller
            Tablet = 0x80,          // Tablet PC controls
            Consumer = 0x0C,        // Consumer
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RawInputDevice
        {
            internal HidUsagePage UsagePage;
            internal HidUsage Usage;
            internal RawInputDeviceFlags Flags;
            internal IntPtr Target;

            public override string ToString()
            {
                return string.Format("{0}/{1}, flags: {2}, target: {3}", UsagePage, Usage, Flags, Target);
            }
        }


        internal static DEVPROPKEY DEVPKEY_Device_BusReportedDeviceDesc =
            new DEVPROPKEY { fmtid = new Guid(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2), pid = 4 };

        internal static DEVPROPKEY DEVPKEY_Device_DeviceDesc =
            new DEVPROPKEY { fmtid = new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), pid = 2 };

        internal static DEVPROPKEY DEVPKEY_Device_HardwareIds =
            new DEVPROPKEY { fmtid = new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), pid = 3 };

        internal static DEVPROPKEY DEVPKEY_Device_UINumber =
            new DEVPROPKEY { fmtid = new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), pid = 18 };

        internal static DEVPROPKEY DEVPKEY_Device_DriverVersion =
            new DEVPROPKEY { fmtid = new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), pid = 3 };

        internal static DEVPROPKEY DEVPKEY_Device_Manufacturer =
            new DEVPROPKEY { fmtid = new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), pid = 13 };

        internal static DEVPROPKEY DEVPKEY_Device_Provider =
            new DEVPROPKEY { fmtid = new Guid(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6), pid = 9 };

        internal static DEVPROPKEY DEVPKEY_Device_Parent =
            new DEVPROPKEY { fmtid = new Guid(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7), pid = 8 };

        internal static DEVPROPKEY DEVPKEY_Device_Siblings =
            new DEVPROPKEY { fmtid = new Guid(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7), pid = 10 };

        internal static DEVPROPKEY DEVPKEY_Device_InstanceId =
            new DEVPROPKEY { fmtid = new Guid(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57), pid = 256 };

    }
}
