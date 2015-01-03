using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace ExceptionBreaker.Breakpoints.Glyphs {
    public class BreakpointGlyphFactory : IGlyphFactory {
        public const string Name = "ExceptionBreaker.BreakpointGlyphFactory";
        private const int OverlayGlyphSize = 9;
        private readonly double _breakpointGlyphSize;

        private readonly BitmapImage _image;

        public BreakpointGlyphFactory(IWpfTextViewMargin margin) {
            _breakpointGlyphSize = margin.MarginSize;

            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            _image = new BitmapImage(new Uri("pack://application:,,,/" + assemblyName + ";component/Resources/BreakpointOverlay.png"));
            _image.Freeze();
        }

        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag) {
            if (!(tag is BreakpointTag))
                return null;

            return new Image {
                Source = _image,
                Margin = new Thickness {
                    Left = _breakpointGlyphSize - OverlayGlyphSize,
                    Top = _breakpointGlyphSize - OverlayGlyphSize
                },
                Width  = 9,
                Height = 9
            };
        }
    }
}
