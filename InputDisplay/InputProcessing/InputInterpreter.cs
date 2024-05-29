using InputDisplay;
using InputDisplay.DeviceManagement;
using InputDisplay.InputProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

namespace InputDisplay.InputProcessing
{
    internal static class InputInterpreter
    {
        private static Dictionary<DeviceInfo, List<UsageConfig>> displayedUsages = new();
        private static bool pushBuffer = false;

        public static void Initialize()
        {
            Common.InputProcessed += InterpretInputs;
        }

        private static void InterpretInputs(DeviceInfo device, List<UsageConfig> keyValue, bool pushRequired, bool keyReleased)
        {
            displayedUsages.TryAdd(device, keyValue);
            if (keyValue.Count == 0)
                displayedUsages.Remove(device);
            if (keyReleased)
            {
                pushBuffer = true;
                return;
            }
            List<KeyValuePair<UsageConfig, bool>> inputs = new();
            int i = 0;
            foreach (var currentDevice in displayedUsages.Keys.OrderBy(x => x.preset.displayPriority))
            {
                foreach (var interp in displayedUsages[currentDevice])
                {
                    if (i > 0 & Settings.showAdditions)
                        inputs.Add(new(currentDevice.preset.additionConfig, false));
                    inputs.Add(new(interp, true));
                    i++;
                }
            }
            Common.DisplayInputs?.Invoke(inputs, pushRequired | pushBuffer);
            pushBuffer = false;
        }
    }
}
