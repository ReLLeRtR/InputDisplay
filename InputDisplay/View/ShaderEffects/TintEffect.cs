using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Effects;
using System.Windows.Media;

namespace InputDisplay.View.ShaderEffects
{
    public class TintEffect : ShaderEffect
    {
        private static PixelShader tintShader = new () { UriSource = Extensions.GetPackUri("Resources/Shaders/TintShader.cso", typeof(TintEffect)) };

        private static readonly DependencyProperty MipImageProperty =
            RegisterPixelShaderSamplerProperty(nameof(MipImage), typeof(TintEffect), 0);

        public Brush MipImage
        {
            get => (Brush)GetValue(MipImageProperty);
            set => SetValue(MipImageProperty, value);
        }

        public static readonly DependencyProperty TintWhiteProperty =
            DependencyProperty.Register(nameof(TintWhite), typeof(Color), typeof(TintEffect),
                new PropertyMetadata(Colors.White, PixelShaderConstantCallback(0)));

        public Color TintWhite
        {
            get => (Color)GetValue(TintWhiteProperty);
            set => SetValue(TintWhiteProperty, value);
        }

        public static readonly DependencyProperty TintBlackProperty =
            DependencyProperty.Register(nameof(TintBlack), typeof(Color), typeof(TintEffect),
                new PropertyMetadata(Colors.Black, PixelShaderConstantCallback(1)));

        public Color TintBlack
        {
            get => (Color)GetValue(TintBlackProperty);
            set => SetValue(TintBlackProperty, value);
        }
        
        public TintEffect()
        {
            PixelShader = tintShader;
            UpdateShaderValue(MipImageProperty);
            UpdateShaderValue(TintWhiteProperty);
            UpdateShaderValue(TintBlackProperty);
        }
    }
}
