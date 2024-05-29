using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using static InputDisplay.DeviceManagement.DeviceInfo;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace InputDisplay
{
    [Flags]
    internal enum UsageDisplayType : byte
    {
        OnReleaseOnly = 0,
        OnPress = 1,
        Ignore = 2,
        ActiveOnly = 4,
        Always = 8,
        Value = 16
    }

    internal enum ActivateCondition : byte
    {
        Equal = 0,
        NotEqual = 1,
        Less = 2,
        Greater = 3,
    }
    [Flags]
    public enum InterpretationType : byte
    {
        Text = 1,
        Icon = 2
    }


    public class DisplayPreset
    {
        public int displayPriority;
        public BitmapSource icon;
        public Dictionary<string, BitmapSource> usageIcons = new();
        public UsageConfig additionConfig = new() { name = "+", subTextColor = Brushes.Cyan, iconTintWhiteSubColor = Colors.Cyan };
        public UsageConfig[] usageConfigs;
    }

    public class UsageConfig : ICloneable
    {
        internal int buttonID;
        public string name;
        internal BitmapSource iconImage;
        public Color iconTintWhiteMainColor = Colors.White;
        public Color iconTintWhiteSubColor = Colors.White;
        public Color iconTintBlackMainColor = Colors.Black;
        public Color iconTintBlackSubColor = Colors.Black;
        public InterpretationType type = InterpretationType.Text;
        internal int value;
        internal int threshhold;
        internal int rotation;
        internal int textSize = 100;
        public SolidColorBrush textColor = Brushes.White;
        public SolidColorBrush subTextColor = Brushes.White;
        public Color shadowColor = Colors.Black;
        public Color subShadowColor = Colors.Black;
        internal ActivateCondition condition;
        internal UsageDisplayType displayType;


        internal bool ConditionMet(int usageValue)
        {
            switch (condition)
            {
                case ActivateCondition.Equal:
                    if (usageValue >= value - threshhold & usageValue <= value + threshhold)
                        return true;

                    break;
                case ActivateCondition.Less:
                    if (usageValue < value - threshhold)
                        return true;

                    break;
                case ActivateCondition.Greater:
                    if (usageValue > value + threshhold)
                        return true;

                    break;
                case ActivateCondition.NotEqual:
                    if (usageValue < value - threshhold | usageValue > value + threshhold)
                        return true;

                    break;
            }
            return false;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
