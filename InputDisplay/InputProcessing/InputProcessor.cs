using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static InputDisplay.NativeMethods;
using System.Windows.Interop;
using System.Windows;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Threading;
using InputDisplay.DeviceManagement;
using System.Runtime.CompilerServices;
using System.Reflection.Metadata;
using Microsoft.Win32.SafeHandles;
using System.Collections;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Windows.Controls;

namespace InputDisplay.InputProcessing
{
    public static class InputProcessor
    {
        private static Dictionary<IntPtr, KeyValuePair<DeviceInfo, List<UsageConfig>>> processedDeviceInputs = new();

        private static CancellationTokenSource mouseRemover;


        public static void Initialize()
        {
            Common.InputReceived += ReceiveRawInput;
            Common.DevicesRegistered += RegisterDevices;
        }

        private static void RegisterDevices(Dictionary<IntPtr, DeviceInfo> deviceList)
        {
            //add new unregistered devices
            for (int i = 0; i < deviceList.Count; i++)
                processedDeviceInputs.TryAdd(deviceList.Keys.ElementAt(i),
                    new KeyValuePair<DeviceInfo, List<UsageConfig>>(deviceList.Values.ElementAt(i), new List<UsageConfig>()));

            //remove disconnected registered devices
            IEnumerable<IntPtr> pointers = processedDeviceInputs.Keys.Where(x => !deviceList.ContainsKey(x));
            for (int i = 0; i < pointers.Count(); i++)
                processedDeviceInputs.Remove(pointers.ElementAt(i));
        }

        private static void ReceiveRawInput(IntPtr hDevice)
        {
            int actualSize = 0;
            int dwSize = NativeMethods.GetRawInputData(hDevice, DataCommand.RID_INPUT, null, ref actualSize, Marshal.SizeOf(typeof(RawInputHeader)));
            if (actualSize < 0)
                return;
            var buffer = new byte[actualSize];
            dwSize = NativeMethods.GetRawInputData(hDevice, DataCommand.RID_INPUT, buffer, ref actualSize, Marshal.SizeOf(typeof(RawInputHeader)));
            var header = Unsafe.As<byte, RawInputHeader> (ref buffer[0]);

            if (!processedDeviceInputs.TryGetValue(header.hDevice, out KeyValuePair<DeviceInfo, List<UsageConfig>> deviceInputs))
                return;

            ProcessRawInput(buffer, deviceInputs, actualSize);
        }

        private static void ProcessRawInput(byte[] buffer, KeyValuePair<DeviceInfo, List<UsageConfig>> deviceInputs, int actualSize)
        {
            DeviceInfo device = deviceInputs.Key;
            List<UsageConfig> values = deviceInputs.Value;

            bool inputsChanged = false;
            bool pushInputs = false;
            bool keyReleased = false;

            if (mouseRemover != null)
            {
                mouseRemover.Cancel();
                mouseRemover = null;
            }
            switch (device.type)
            {
                case DeviceType.RimTypeMouse:
                    ref var inputMs = ref Unsafe.As<byte, InputDataMouse>(ref buffer[0]);
                    ProcessMouseInput(inputMs, device, values, ref inputsChanged, ref pushInputs, ref keyReleased);
                    break;
                case DeviceType.RimTypeKeyboard:

                    ref var inputKb = ref Unsafe.As<byte, InputDataKeyboard>(ref buffer[0]);
                    ProcessKeyboardInput(inputKb, device, values, ref inputsChanged, ref pushInputs, ref keyReleased);

                    break;
                case DeviceType.RimTypeHid:

                    ref var inputHid = ref Unsafe.As<byte, InputDataHid>(ref buffer[0]);
                    var length = inputHid.data.dwSizeHid * inputHid.data.dwCount;
                    if (Unsafe.ByteOffset(ref buffer[0], ref inputHid.data.bRawData) + length > actualSize) // sanity check
                        throw new Exception("Length incorrect");

                    var span = MemoryMarshal.CreateSpan(ref inputHid.data.bRawData, (int)length);
                    byte[] data = span.ToArray();
                    ProcessHidInput(inputHid, device, values, data, ref inputsChanged, ref pushInputs, ref keyReleased);

                    break;
            }
            if (inputsChanged)
                Common.InputProcessed?.Invoke(device, values, pushInputs, keyReleased);
        }

        private static void ProcessHidInput(InputDataHid input, DeviceInfo device, List<UsageConfig> values, byte[] span, ref bool inputsChanged, ref bool pushInput, ref bool keyReleased)
        {
            inputsChanged = false;
            keyReleased = false;
            pushInput = false;

            List<UsageConfig> valuesPrev = new(values);
            values.Clear();

            //get pressed buttons
            int usageLength = device.numberOfButtons;
            short[] usages = new short[device.numberOfButtons];
            NativeMethods.HidP_GetUsages(0, device.buttonCaps.UsagePage, 0, usages, ref usageLength, device.preparsedData, span, input.data.dwSizeHid);

            for (int i = 0; i < usageLength; i++)
            {
                int buttonId = usages[i] - device.buttonCaps.UsageMin;
                if (device.IsUsageValid(buttonId, 1, out List<UsageConfig> target))
                    foreach (var usage in target)
                        values.Add(usage);
            }


            // get values
            for (int i = 0; i < device.capabilities.NumberInputValueCaps; i++)
            {
                byte value = 0;
                short buttonId = device.valueCaps[i].UsageMin;
                NativeMethods.HidP_GetUsageValue(0, device.valueCaps[i].UsagePage, 0, buttonId, out value, device.preparsedData, span, input.data.dwSizeHid);
                if (device.IsUsageValid(buttonId, value, out List<UsageConfig> target))
                    foreach (var usage in target)
                        values.Add(usage);
            }

            foreach (var usage in values.Where(x => !valuesPrev.Contains(x)))
            {
                inputsChanged = true;
                pushInput |= device.PushRequired(usage.buttonID, false);
            }
            foreach (var usage in valuesPrev.Where(x => !values.Contains(x)))
            {
                inputsChanged = true;
                pushInput |= device.PushRequired(usage.buttonID, true);
                keyReleased = true;
            }
        }

        private static void ProcessKeyboardInput(InputDataKeyboard input, DeviceInfo device, List<UsageConfig> values, ref bool inputsChanged, ref bool pushInput, ref bool keyReleased)
        {
            inputsChanged = false;
            keyReleased = false;
            pushInput = false;

            List<UsageConfig> target;
            int keyCode = input.data.MakeCode;
            switch (input.data.Flags)
            {
                case 0:
                case 2:
                    if (!device.IsUsageValid(keyCode, input.data.Flags, out target))
                        return;
                    foreach (UsageConfig i in target)
                    {
                        if (!values.Contains(i))
                        {
                            inputsChanged = true;
                            values.Add(i);
                            pushInput = device.PushRequired(keyCode, false);
                        }
                    }
                    break;
                case 1:
                case 3:
                    if (!device.IsUsageValid(keyCode, input.data.Flags - 1, out target))
                        return;
                    foreach (UsageConfig i in target)
                    {
                        if (values.Contains(i))
                        {
                            inputsChanged = true;
                            values.Remove(i);
                            pushInput = device.PushRequired(keyCode, true);
                            keyReleased = true;
                        }
                    }
                    break;
            }
        }

        private static void ProcessMouseInput(InputDataMouse input, DeviceInfo device, List<UsageConfig> values, ref bool inputsChanged, ref bool pushInput, ref bool keyReleased)
        {
            inputsChanged = false;
            keyReleased = false;
            pushInput = false;
            US_BUTTON_FLAGS flags = input.data.usButtonFlags;
            List<UsageConfig> target;
            switch (flags)
            {
                case US_BUTTON_FLAGS.RI_MOUSE_LEFT_BUTTON_DOWN:
                case US_BUTTON_FLAGS.RI_MOUSE_RIGHT_BUTTON_DOWN:
                case US_BUTTON_FLAGS.RI_MOUSE_MIDDLE_BUTTON_DOWN:
                case US_BUTTON_FLAGS.RI_MOUSE_BUTTON_4_DOWN:
                case US_BUTTON_FLAGS.RI_MOUSE_BUTTON_5_DOWN:
                    int buttonID = ((int)flags).GetExponentOfPowerOfTwo();
                    buttonID /= 2;
                    if (!device.IsUsageValid(buttonID, 1, out target))
                        return;
                    foreach (UsageConfig i in target)
                        if (!values.Contains(i))
                        {
                            inputsChanged = true;
                            values.Add(i);
                            pushInput = device.PushRequired(buttonID, false);
                        }
                    break;
                case US_BUTTON_FLAGS.RI_MOUSE_WHEEL:
                case US_BUTTON_FLAGS.RI_MOUSE_HWHEEL:
                    buttonID = ((int)flags).GetExponentOfPowerOfTwo();
                    buttonID /= 2;
                    int value = input.data.usButtonData == 120 ? 1 : -1;
                    if (!device.IsUsageValid(buttonID, value, out target))
                        return;
                    foreach (UsageConfig i in target)
                    {
                        if (!values.Contains(i))
                        {
                            inputsChanged = true;
                            values.Add(i);
                            pushInput = device.PushRequired(buttonID, false);
                            mouseRemover = new();
                            RemoveMouseInput(i, device, values, mouseRemover.Token);
                        }
                    }
                    break;
                case US_BUTTON_FLAGS.RI_MOUSE_LEFT_BUTTON_UP:
                case US_BUTTON_FLAGS.RI_MOUSE_RIGHT_BUTTON_UP:
                case US_BUTTON_FLAGS.RI_MOUSE_MIDDLE_BUTTON_UP:
                case US_BUTTON_FLAGS.RI_MOUSE_BUTTON_4_UP:
                case US_BUTTON_FLAGS.RI_MOUSE_BUTTON_5_UP:
                    buttonID = ((int)flags / 2).GetExponentOfPowerOfTwo();
                    buttonID /= 2;
                    if (!device.IsUsageValid(buttonID, 1, out target))
                        return;
                    foreach (UsageConfig i in target)
                    {
                        if (values.Contains(i))
                        {
                            inputsChanged = true;
                            values.Remove(i);
                            pushInput = device.PushRequired(buttonID, true);
                            keyReleased = true;
                        }
                    }
                    break;
            }

            static async void RemoveMouseInput(UsageConfig config, DeviceInfo device, List<UsageConfig> values, CancellationToken token)
            {
                await Task.Delay(300);
                if (token.IsCancellationRequested)
                {
                    values.Remove(config);
                    Common.InputProcessed?.Invoke(device, values, true, true);
                    return;
                }
                values.Remove(config);
                Common.InputProcessed?.Invoke(device, values, true, true);
            }
        }

    }
}
