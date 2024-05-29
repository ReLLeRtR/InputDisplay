using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
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

namespace InputDisplay.Display
{
    public static class PresetsManager
    {
        private static DisplayPreset defaultKeyboard;
        private static DisplayPreset defaultMouse;
        private static DisplayPreset defaultDualsense;
        public static DisplayPreset DefaultKeyboard
        {
            get
            {
                if(defaultKeyboard == null)
                {
                    string jsonPath = Common.KeyboardConfigPath;
                    string jsonString = File.ReadAllText(jsonPath + Common.ConfigFileName);
                    defaultKeyboard = JsonToPreset(jsonString, jsonPath);
                }
               return defaultKeyboard; 
            }
        }
        public static DisplayPreset DefaultMouse
        {
            get
            {
                if (defaultMouse == null)
                {
                    string jsonPath = Common.MouseConfigPath;
                    string jsonString = File.ReadAllText(jsonPath + Common.ConfigFileName);
                    defaultMouse = JsonToPreset(jsonString, jsonPath);
                }
                return defaultMouse;
            }
        }
        public static DisplayPreset DefaultDualsense
        {
            get
            {
                if (defaultDualsense == null)
                {
                    string jsonPath = Common.DualsenseConfigPath;
                    string jsonString = File.ReadAllText(jsonPath + Common.ConfigFileName);
                    defaultDualsense = JsonToPreset(jsonString, jsonPath);
                }
                return defaultDualsense;
            }
        }

        private static DisplayPreset JsonToPreset(string jsonText, string jsonPath)
        {
            DisplayPreset preset = new DisplayPreset();
            string[] entries = jsonText.Split(new string[] { "," + Environment.NewLine }, StringSplitOptions.None);
            List<UsageConfig> configs = new List<UsageConfig>();
            UsageConfig defaultConfig = new();
            for (int i = 0; i < entries.Length; i++)
            {
                string[] usages = entries[i].Split(new string[] { "][" }, StringSplitOptions.None);
                ParseUsages(usages, jsonPath, ref preset, ref defaultConfig, ref configs);
                //while (configs.Count <= id)
                //    configs.Add(new UsageConfig());
            }
            preset.usageConfigs = configs.ToArray();
            return preset;
        }

        private static bool ParseUsages(string[] strings, string jsonPath, ref DisplayPreset preset, ref UsageConfig defaultUsage, ref List<UsageConfig> configs)
        {
            int id = 0;
            if (strings.Length == 0)
                return false;

            if (strings[0].Contains("ID:"))
            {
                if (strings[0].LastIndexOf(']') == strings[0].Length)
                    strings[0] = strings[0].Remove(strings[0].Length - 1);

                string idText = strings[0][(strings[0].IndexOf(':') + 1)..];
                if (!Int32.TryParse(idText, out id))
                    return false;

                UsageConfig config = (UsageConfig)defaultUsage.Clone();
                config.buttonID = id;

                if (!ParseUsage(strings, jsonPath, ref preset, ref config))
                    return false;

                configs.Add(config);
            }

            else if (strings[0].Contains("Default"))
            {
                if (!ParseUsage(strings, jsonPath, ref preset, ref defaultUsage))
                    return false;
            }

            return true;
        }

        private static bool ParseUsage(string[] strings, string jsonPath, ref DisplayPreset preset, ref UsageConfig config)
        {
            for (int i = 1; i < strings.Length; i++)
            {
                string current = strings[i];
                if (current.LastIndexOf(']') == current.Length - 1)
                    current = current.Remove(current.Length - 1);

                if (current.Contains("DISPLAYTYPE:"))
                {
                    current = current[(current.IndexOf(':') + 1)..];
                    if (!Enum.TryParse(typeof(UsageDisplayType), current, true, out object value))
                        return false;
                    config.displayType = (UsageDisplayType)value;
                    continue;
                }

                if (current.Contains("VALUE:"))
                {
                    current = current[(current.IndexOf(':') + 1)..];
                    if (!Int32.TryParse(current, out int value))
                        return false;

                    config.value = value;
                    continue;
                }

                if (current.Contains("THRESHHOLD:"))
                {
                    current = current[(current.IndexOf(':') + 1)..];
                    if (!Int32.TryParse(current, out int value))
                        return false;

                    config.threshhold = value;
                    continue;
                }

                if (current.Contains("TEXTSIZE:"))
                {
                    current = current[(current.IndexOf(':') + 1)..];
                    if (!Int32.TryParse(current, out int value))
                        return false;

                    config.textSize = value;
                    continue;
                }

                if (current.Contains("ROTATION:"))
                {
                    current = current[(current.IndexOf(':') + 1)..];
                    if (!Int32.TryParse(current, out int value))
                        return false;

                    config.rotation = value;
                    continue;
                }

                if (current.Contains("TEXTMAINCOLOR:"))
                {
                    current = current[(current.IndexOf('#'))..];
                    SolidColorBrush color = (SolidColorBrush)new BrushConverter().ConvertFrom(current);

                    config.textColor = color;
                    continue;
                }
                if (current.Contains("TEXTSUBCOLOR:"))
                {
                    current = current[(current.IndexOf('#'))..];
                    SolidColorBrush color = (SolidColorBrush)new BrushConverter().ConvertFrom(current);

                    config.subTextColor = color;
                    continue;
                }

                if (current.Contains("TINWHITEMAINCOLOR:"))
                {
                    current = current[(current.IndexOf('#'))..];
                    Color color = (Color)ColorConverter.ConvertFromString(current);

                    config.iconTintWhiteMainColor = color;
                    continue;
                }

                if (current.Contains("TINTWHITESUBCOLOR:"))
                {
                    current = current[(current.IndexOf('#'))..];
                    Color color = (Color)ColorConverter.ConvertFromString(current);

                    config.iconTintWhiteSubColor = color;
                    continue;
                }

                if (current.Contains("SHADOWMAINCOLOR:"))
                {
                    current = current[(current.IndexOf('#'))..];
                    Color color = (Color)ColorConverter.ConvertFromString(current);

                    config.shadowColor = color;
                    continue;
                }
                if (current.Contains("SHADOWSUBCOLOR:"))
                {
                    current = current[(current.IndexOf('#'))..];
                    Color color = (Color)ColorConverter.ConvertFromString(current);

                    config.subShadowColor = color;
                    continue;
                }

                if (current.Contains("INTERPRETATIONTYPE:"))
                {
                    current = current[(current.IndexOf(':') + 1)..];
                    string[] enums = current.Split('&');
                    InterpretationType value = 0;
                    foreach(var cur in enums)
                    {
                        if (!Enum.TryParse(typeof(InterpretationType), cur, true, out object curValue))
                            return false;
                        value |= (InterpretationType)curValue;
                    }
                    config.type = value;
                    continue;
                }

                if (current.Contains("ICON:"))
                {
                    int startOfString = current.IndexOf('"') + 1;
                    int length = current.LastIndexOf('"') - startOfString;
                    current = current.Substring(startOfString, length);
                    if(!preset.usageIcons.TryGetValue(current, out BitmapSource image))
                    {
                        image = BitmapFrame.Create(new Uri(jsonPath + Common.ImageFolderName + current, UriKind.Relative));
                        config.iconImage = image;
                        preset.usageIcons.Add(current, image);
                        continue;
                    }
                    config.iconImage = image;
                    continue;
                }

                if (current.Contains('"'))
                {
                    int startOfString = current.IndexOf('"') + 1;
                    int length = current.LastIndexOf('"') - startOfString;
                    current = current.Substring(startOfString, length);
                    config.name = current;
                    continue;
                }

                if (current.Length == 1)
                {
                    switch (current)
                    {
                        case "+":
                            config.condition = ActivateCondition.Greater;
                            break;
                        case "-":
                            config.condition = ActivateCondition.Less;
                            break;
                        case "=":
                            config.condition = ActivateCondition.Equal;
                            break;
                        case "/":
                            config.condition = ActivateCondition.NotEqual;
                            break;
                    }
                    continue;
                }
            }
            return true;
        }


        private static void PresetToJson()
        {

        }
    }
}
