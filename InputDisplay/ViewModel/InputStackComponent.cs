using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using InputDisplay;
using InputDisplay.DeviceManagement;
using InputDisplay.ViewModel;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;

namespace InputDisplay.ViewModel
{
    internal class InputStackComponent : ViewModelBase
    {
        private UsageConfig usage;
        public UsageConfig Usage
        {
            set
            {
                usage = value;
                OnPropertyChanged("DisplayText");
                OnPropertyChanged("DisplayIcon");
                OnPropertyChanged("TextColor");
                OnPropertyChanged("ShadowColor");
                OnPropertyChanged("TintWhiteColor");
                OnPropertyChanged("TintBlackColor");
                OnPropertyChanged("Rotation");
                OnPropertyChanged("BlockHeight");
                OnPropertyChanged("TextHeight");
            }
        }
        private bool isMain;
        public bool IsMain
        {
            set
            {
                isMain = value;
                OnPropertyChanged("TextColor");
                OnPropertyChanged("ShadowColor");
                OnPropertyChanged("TintWhiteColor");
                OnPropertyChanged("TintBlackColor");
            }
        }
        private bool isInput;
        public bool IsInput
        {
            set
            {
                isInput = value;
                OnPropertyChanged("BlockHeight");
                OnPropertyChanged("BlockWidth");
            }
        }

        public string DisplayText
        {
            get
            {
                return usage.type.HasFlag(InterpretationType.Text) ? usage.name : null;
            }
        }
        public ImageSource DisplayIcon
        {
            get
            {
                return usage.type.HasFlag(InterpretationType.Icon) ? usage.iconImage : null;
            }
        }

        public Color TintWhiteColor
        {
            get
            {
                return isMain ? usage.iconTintWhiteMainColor : usage.iconTintWhiteSubColor;
            }
        }
        public Color TintBlackColor
        {
            get
            {
                return isMain ? usage.iconTintBlackMainColor : usage.iconTintBlackSubColor;
            }

        }

        public SolidColorBrush TextColor
        {
            get
            {
                return isMain ? usage.textColor : usage.subTextColor;
            }
        }
        public Color ShadowColor
        {
            get
            {
                return isMain ? usage.shadowColor : usage.subShadowColor;
            }
        }
        public int Rotation
        {
            get
            {
                return usage.rotation;
            }
        }
        private int baseSize = 75;

        public int BlockWidth
        {
            get
            {
                return (int)(baseSize * (isInput ? 1 : 0.75f));
            }
        }
        public int BlockHeight
        {
            get
            {
                return (int)(baseSize * (usage.type.HasFlag(InterpretationType.Icon) ? 0.7f : 1));
            }
        }
        public int TextHeight
        {
            get
            {
                return (int)(baseSize * ((float)usage.textSize/100));
            }
        }
    }
}
