using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace ExceptionBreaker.Breakpoints.Glyphs {
    public class BreakpointGlyphFactory : IGlyphFactory {
        private const int OverlayGlyphSize = 9;
        private readonly double _breakpointGlyphSize;

        private readonly IDictionary<ExceptionBreakChange, Lazy<BitmapImage>> _images = new Dictionary<ExceptionBreakChange, Lazy<BitmapImage>>();

        public BreakpointGlyphFactory(IWpfTextViewMargin margin) {
            _breakpointGlyphSize = margin.MarginSize;
            InitImages();
        }

        private void InitImages() {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            foreach (var change in new[] {ExceptionBreakChange.SetBreakOnNone, ExceptionBreakChange.SetBreakOnAll}) {
                var changeCapture = change;
                _images[change] = new Lazy<BitmapImage>(() => {
                    var image = new BitmapImage(new Uri("pack://application:,,,/" + assemblyName + ";component/Resources/BreakpointOverlay." + changeCapture + ".png"));
                    image.Freeze();
                    return image;
                }, LazyThreadSafetyMode.ExecutionAndPublication);
            }
        }

        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag) {
            var breakpointTag = tag as BreakpointTag;
            if (breakpointTag == null)
                return null;

            return new Image {
                Source = _images[breakpointTag.BreakpointExtraData.ExceptionBreakChange].Value,
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
