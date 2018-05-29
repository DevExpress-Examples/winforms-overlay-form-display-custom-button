using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Utils.Drawing.Helpers;

namespace DxSample {
    public static class NativeAPI {
        public static bool ScreenToClient(IntPtr hWnd, ref NativeMethods.POINT lpPoint) {
            return Core.ScreenToClient(hWnd, ref lpPoint);
        }
        static class Core {
            [DllImport("user32.dll")]
            public static extern bool ScreenToClient(IntPtr hWnd, ref NativeMethods.POINT lpPoint);
        }
    }
}
