using InputDisplay.DeviceManagement;
using InputDisplay;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InputDisplay.DeviceManagement
{
    public static class DeviceGetter
    {
        private static object _lock = new object();


        public static Dictionary<string, string> EnumerateDevices2()
        {
            var paths = new Dictionary<string, string>();
            for (int i = 0; i < 3; i++)
            {
                DeviceType dwType = (DeviceType)i;
                Guid guid = dwType switch
                {
                    DeviceType.RimTypeMouse => Guid.Parse("{378DE44C-56EF-11D1-BC8C-00A0C91405DD}"),
                    DeviceType.RimTypeKeyboard => Guid.Parse("{884b96c3-56ef-11d1-bc8c-00a0c91405dd}"),
                    DeviceType.RimTypeHid => Guid.Parse("{4D1E55B2-F16F-11CF-88CB-001111000030}")
                };
                var deviceInfoSet = NativeMethods.SetupDiGetClassDevs(ref guid, null, 0, NativeMethods.DIGCF_PRESENT | NativeMethods.DIGCF_DEVICEINTERFACE);

                if (deviceInfoSet.ToInt64() != NativeMethods.INVALID_HANDLE_VALUE)
                {
                    var deviceInfoData = CreateDeviceInfoData();
                    var deviceIndex = 0;

                    while (NativeMethods.SetupDiEnumDeviceInfo(deviceInfoSet, deviceIndex, ref deviceInfoData))
                    {
                        deviceIndex += 1;

                        var deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
                        deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);
                        var deviceInterfaceIndex = 0;

                        while (NativeMethods.SetupDiEnumDeviceInterfaces(deviceInfoSet, ref deviceInfoData, ref guid, deviceInterfaceIndex, ref deviceInterfaceData))
                        {
                            deviceInterfaceIndex++;
                            var devicePath = GetDevicePath(deviceInfoSet, deviceInterfaceData);
                            var description = GetBusReportedDeviceDescription(deviceInfoSet, ref deviceInfoData) ??
                                              GetDeviceDescription(deviceInfoSet, ref deviceInfoData);
                            var parent = GetDeviceParent(deviceInfoSet, ref deviceInfoData);
                            paths.Add(devicePath, description);
                        }
                    }
                    NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoSet);
                }
            }
            return paths;


            SP_DEVINFO_DATA CreateDeviceInfoData()
            {
                var deviceInfoData = new SP_DEVINFO_DATA();

                deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);
                deviceInfoData.DevInst = 0;
                deviceInfoData.ClassGuid = Guid.Empty;
                deviceInfoData.Reserved = IntPtr.Zero;

                return deviceInfoData;
            }


            string GetDevicePath(IntPtr deviceInfoSet, SP_DEVICE_INTERFACE_DATA deviceInterfaceData)
            {
                var bufferSize = 0;
                var interfaceDetail = new SP_DEVICE_INTERFACE_DETAIL_DATA { Size = IntPtr.Size == 4 ? 4 + Marshal.SystemDefaultCharSize : 8 };

                NativeMethods.SetupDiGetDeviceInterfaceDetailBuffer(deviceInfoSet, ref deviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, IntPtr.Zero);

                return NativeMethods.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, ref interfaceDetail, bufferSize, ref bufferSize, IntPtr.Zero) ?
                    interfaceDetail.DevicePath : null;
            }


            string GetDeviceDescription(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA devinfoData)
            {
                var descriptionBuffer = new byte[1024];

                var requiredSize = 0;
                var type = 0;

                NativeMethods.SetupDiGetDeviceRegistryProperty(deviceInfoSet,
                                                                ref devinfoData,
                                                                NativeMethods.SPDRP_DEVICEDESC,
                                                                ref type,
                                                                descriptionBuffer,
                                                                descriptionBuffer.Length,
                                                                ref requiredSize);

                return descriptionBuffer.ToUTF8String();
            }

            string GetBusReportedDeviceDescription(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA devinfoData)
            {
                var descriptionBuffer = new byte[1024];

                if (Environment.OSVersion.Version.Major > 5)
                {
                    ulong propertyType = 0;
                    var requiredSize = 0;

                    var _continue = NativeMethods.SetupDiGetDeviceProperty(deviceInfoSet,
                                                                            ref devinfoData,
                                                                            ref NativeMethods.DEVPKEY_Device_BusReportedDeviceDesc,
                                                                            ref propertyType,
                                                                            descriptionBuffer,
                                                                            descriptionBuffer.Length,
                                                                            ref requiredSize,
                                                                            0);

                    if (_continue) return descriptionBuffer.ToUTF16String();
                }
                return null;
            }

            string GetDeviceParent(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA devinfoData)
            {
                string result = string.Empty;

                var requiredSize = 0;
                ulong propertyType = 0;

                NativeMethods.SetupDiGetDeviceProperty(deviceInfoSet, ref devinfoData,
                                                            ref NativeMethods.DEVPKEY_Device_Parent, ref propertyType,
                                                            null, 0,
                                                            ref requiredSize, 0);

                if (requiredSize > 0)
                {
                    var descriptionBuffer = new byte[requiredSize];
                    NativeMethods.SetupDiGetDeviceProperty(deviceInfoSet, ref devinfoData,
                                                            ref NativeMethods.DEVPKEY_Device_Parent, ref propertyType,
                                                            descriptionBuffer, descriptionBuffer.Length,
                                                            ref requiredSize, 0);

                    string tmp = System.Text.Encoding.Unicode.GetString(descriptionBuffer);
                    if (tmp.EndsWith("\0"))
                    {
                        tmp = tmp.Remove(tmp.Length - 1);
                    }
                    result = tmp;
                }

                return result;
            }
        }
        public static void EnumerateDevices()
        {
            lock (_lock)
            {
                var paths = EnumerateDevices2();
                Dictionary<IntPtr, DeviceInfo> deviceList = new();
                uint deviceCount = 0;
                var dwSize = (Marshal.SizeOf(typeof(RawInputDeviceList)));


                if (NativeMethods.GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, (uint)dwSize) != 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                    return;
                }


                var pRawInputDeviceList = Marshal.AllocHGlobal((int)(dwSize * deviceCount));
                NativeMethods.GetRawInputDeviceList(pRawInputDeviceList, ref deviceCount, (uint)dwSize);

                for (var i = 0; i < deviceCount; i++)
                {
                    var rid = (RawInputDeviceList)Marshal.PtrToStructure(new IntPtr((pRawInputDeviceList.ToInt64() + (dwSize * i))), typeof(RawInputDeviceList));


                    if (!GetDevInf(rid.hDevice, out object result, out DeviceType dwType) |
                        !GetName(rid.hDevice, out string path))
                        continue;

                    if ((dwType == DeviceType.RimTypeHid |
                        dwType == DeviceType.RimTypeKeyboard |
                        dwType == DeviceType.RimTypeMouse) &
                        !deviceList.ContainsKey(rid.hDevice) & paths.TryGetValue(path.ToLower(), out string name))
                    {

                        DeviceInfo newDevice = new DeviceInfo(rid.hDevice, dwType, name, path);
                        deviceList.Add(rid.hDevice, newDevice);
                    }

                }

                Marshal.FreeHGlobal(pRawInputDeviceList);
                Common.DevicesRegistered?.Invoke(deviceList);
            }

            bool GetDevInf(IntPtr hDevice, out object result, out DeviceType deviceType)
            {
                result = null;
                deviceType = (DeviceType)4;
                uint pcbSize = 0;
                NativeMethods.GetRawInputDeviceInfo(hDevice, RawInputDeviceInfo.RIDI_DEVICEINFO, IntPtr.Zero, ref pcbSize);
                if (pcbSize <= 0)
                    return false;

                var pData = Marshal.AllocHGlobal((int)pcbSize);
                NativeMethods.GetRawInputDeviceInfo(hDevice, RawInputDeviceInfo.RIDI_DEVICEINFO, pData, ref pcbSize);
                var header = Marshal.PtrToStructure<RidDeviceInfoHeader>(pData);
                deviceType = (DeviceType)header.dwType;
                switch (deviceType)
                {
                    case DeviceType.RimTypeMouse:
                        result = Marshal.PtrToStructure<RidDeviceInfoMouse>(pData);
                        break;
                    case DeviceType.RimTypeHid:
                        result = Marshal.PtrToStructure<RidDeviceInfoHid>(pData);
                        break;
                    case DeviceType.RimTypeKeyboard:
                        result = Marshal.PtrToStructure<RidDeviceInfoKeyboard>(pData);
                        break;
                }
                Marshal.FreeHGlobal(pData);
                return true;
            }

            bool GetName(IntPtr hDevice, out string name)
            {
                name = string.Empty;
                uint pcbSize = 0;
                NativeMethods.GetRawInputDeviceInfo(hDevice, RawInputDeviceInfo.RIDI_DEVICENAME, IntPtr.Zero, ref pcbSize);

                if (pcbSize <= 0)
                    return false;

                var pData = Marshal.AllocHGlobal((int)pcbSize);
                NativeMethods.GetRawInputDeviceInfo(hDevice, RawInputDeviceInfo.RIDI_DEVICENAME, pData, ref pcbSize);
                name = Marshal.PtrToStringAnsi(pData);
                Marshal.FreeHGlobal(pData);
                return true;
            }
        }
    }
}



