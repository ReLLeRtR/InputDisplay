using InputDisplay.InputProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InputDisplay.InputProcessing;
using InputDisplay.DeviceManagement;
using InputDisplay;
using static InputDisplay.NativeMethods;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows;
using System.Diagnostics;
using InputDisplay.ViewModel;

namespace InputDisplay
{
    internal class Main
    {
        private InputScreenViewModel inputScreen;
        private IntPtr _notificationHandle = IntPtr.Zero;

        public Main()
        {
            inputScreen = new InputScreenViewModel();
            InputProcessor.Initialize();
            InputInterpreter.Initialize();
            DeviceGetter.EnumerateDevices();
            HookProc();
        }

        public async void HookProc()
        {
            await Task.Delay(10);
            HwndSource source = PresentationSource.FromVisual(inputScreen.window) as HwndSource;
            source.AddHook(WndProc);


            var rid = new RawInputDevice[4];
            RawInputDeviceFlags flag = RawInputDeviceFlags.INPUTSINK | RawInputDeviceFlags.DEVNOTIFY;

            rid[0].UsagePage = HidUsagePage.GENERIC;
            rid[0].Usage = HidUsage.Keyboard;
            rid[0].Flags = flag;
            rid[0].Target = source.Handle;

            rid[1].UsagePage = HidUsagePage.GENERIC;
            rid[1].Usage = HidUsage.Mouse;
            rid[1].Flags = flag;
            rid[1].Target = source.Handle;

            rid[2].UsagePage = HidUsagePage.GENERIC;
            rid[2].Usage = HidUsage.Joystick;
            rid[2].Flags = flag;
            rid[2].Target = source.Handle;

            rid[3].UsagePage = HidUsagePage.GENERIC;
            //rid[3].Usage = HidUsage.Tablet;
            rid[3].Usage = HidUsage.Gamepad;
            rid[3].Flags = flag;
            rid[3].Target = source.Handle;

            if (!NativeMethods.RegisterRawInputDevices(rid, 4, (uint)Marshal.SizeOf(rid[0])))
            {
                throw new ApplicationException("Failed to register raw input device(s).");
            }

            //Guid guid = 0 switch
            //{
            //    0 => Guid.Parse("{378DE44C-56EF-11D1-BC8C-00A0C91405DD}"),
            //    1 => Guid.Parse("{884b96c3-56ef-11d1-bc8c-00a0c91405dd}"),
            //    2 => Guid.Parse("{4D1E55B2-F16F-11CF-88CB-001111000030}")
            //};

            //var dbdi = new DEV_BROADCAST_DEVICE_INTERFACE
            //{
            //    DeviceType = DBT_DEVTYP_DEVICEINTERFACE,
            //    Reserved = 0,
            //    ClassGuid = guid,
            //    Name = 0,
            //};
            //dbdi.Size = Marshal.SizeOf(dbdi);

            //IntPtr buffer = Marshal.AllocHGlobal(dbdi.Size);
            //Marshal.StructureToPtr(dbdi, buffer, true);
            //GCHandle handle = GCHandle.Alloc(source);

            //_notificationHandle = NativeMethods.RegisterDeviceNotification(GCHandle.ToIntPtr(handle), buffer, 0);

            //handle.Free();
        }


        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_INPUT:
                    Common.InputReceived?.Invoke(lParam);
                    break;
                case WM_DEVICECHANGE:
                    Int32 Type = wParam.ToInt32();
                    if (Type == DBT_DEVICEARRIVAL || Type == DBT_DEVICEREMOVECOMPLETE)
                    {

                    }
                    break;
            }

            return IntPtr.Zero;
        }
    }
}