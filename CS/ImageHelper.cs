using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Svg;

namespace DxSample {
    public static class ImageHelper {
        public static Image CreateImage(byte[] data, ISkinProvider skinProvider = null) {
            SvgBitmap svgBitmap = new SvgBitmap(data);
            return svgBitmap.Render(SvgPaletteHelper.GetSvgPalette(skinProvider ?? UserLookAndFeel.Default, ObjectState.Normal), ScaleUtils.GetScaleFactor().Height);
        }
    }
}
