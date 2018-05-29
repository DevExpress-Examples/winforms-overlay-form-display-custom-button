using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using DxSample.Properties;

namespace DxSample {
    public partial class Form1 : RibbonForm {
        TaskHelper taskHelper;
        Image buttonImage;
        Image hotButtonImage;

        OverlayLabelDrawHelper overlayLabelDrawHelper;
        OverlaButtonDrawHelper overlayButtonDrawHelper;

        public Form1() {
            this.buttonImage = CreateButtonImage();
            this.hotButtonImage = CreateHotButtonImage();
            this.overlayLabelDrawHelper = new OverlayLabelDrawHelper();
            this.overlayButtonDrawHelper = new OverlaButtonDrawHelper(hotButtonImage, buttonImage, OnCancelButtonClick);
            InitializeComponent();
        }

        Image CreateButtonImage() {
            return ImageHelper.CreateImage(Resources.cancel_normal);
        }
        Image CreateHotButtonImage() {
            return ImageHelper.CreateImage(Resources.cancel_active);
        }

        async void OnRunTaskItemClick(object sender, ItemClickEventArgs e) {
            biRunTask.Enabled = false;
            string taskResult;
            IOverlaySplashScreenHandle overlayHandle = SplashScreenManager.ShowOverlayForm(contentPanel, customPainter: new OverlayWindowCompositePainter(overlayLabelDrawHelper, overlayButtonDrawHelper));
            try {
                taskResult = await RunTask();
            }
            finally {
                SplashScreenManager.CloseOverlayForm(overlayHandle);
                biRunTask.Enabled = true;
            }
            XtraMessageBox.Show(this, taskResult, "Task Result");
        }

        async Task<string> RunTask() {
            string taskResult;
            this.taskHelper = new TaskHelper();
            try {
                taskResult = await taskHelper.StartTask(new Progress<int>(OnProgressChanged));
            }
            catch(OperationCanceledException) {
                taskResult = "Operation is Cancelled";
            }
            finally {
                taskHelper.Dispose();
                taskHelper = null;
            }
            return taskResult;
        }

        void OnCancelButtonClick() {
            if(taskHelper != null) taskHelper.Cancel();
        }
        void OnProgressChanged(int value) {
            overlayLabelDrawHelper.Text = value.ToString() + "%";
        }
        protected override void Dispose(bool disposing) {
            if(disposing) {
                buttonImage.Dispose();
                hotButtonImage.Dispose();
                if(components != null) components.Dispose();
            }
            base.Dispose(disposing);
        }
    }



    abstract class OverlayElementDrawHelperBase : OverlayWindowPainterBase {
        public OverlayElementDrawHelperBase() {
        }
        protected sealed override void Draw(OverlayWindowCustomDrawContext context) {
            CalculateLayout(context.DrawArgs);
            context.DefaultDraw();
            if(CanDraw) {
                DrawCore(context);
            }
            context.Handled = true;
        }
        protected virtual void CalculateLayout(OverlayLayeredWindowObjectInfoArgs drawArgs) {
        }
        protected virtual bool CanDraw { get { return true; } }
        protected abstract void DrawCore(OverlayWindowCustomDrawContext context);
    }


    class OverlayLabelDrawHelper : OverlayElementDrawHelperBase {
        string text;
        Color color;
        Point textPos;

        static readonly Font font = new Font("Tahoma", 18);

        public OverlayLabelDrawHelper() {
            this.textPos = Point.Empty;
            this.color = Color.Black;
            this.text = string.Empty;
        }
        public string Text {
            get { return text; }
            set { text = value; }
        }
        public Color Color {
            get { return color; }
            set { color = value; }
        }

        protected override bool CanDraw {
            get {
                if(string.IsNullOrEmpty(Text)) return false;
                return true;
            }
        }
        protected override void DrawCore(OverlayWindowCustomDrawContext context) {
            GraphicsCache cache = context.DrawArgs.Cache;
            TextRenderingHint prev = cache.TextRenderingHint;
            cache.TextRenderingHint = TextRenderingHint.AntiAlias;
            try {
                cache.DrawString(Text, font, cache.GetSolidBrush(Color), textPos);
            }
            finally {
                cache.TextRenderingHint = prev;
            }
        }

        protected override void CalculateLayout(OverlayLayeredWindowObjectInfoArgs drawArgs) {
            this.textPos = CalcTextPosition(drawArgs);
        }
        Point CalcTextPosition(OverlayLayeredWindowObjectInfoArgs drawArgs) {
            if(!CanDraw) return Point.Empty;
            Size textSize = drawArgs.Cache.CalcTextSize(Text, font).ToSize();
            return new Point((drawArgs.Bounds.Width - textSize.Width) / 2, drawArgs.ViewInfo.ImageBounds.Bottom + textSize.Height);
        }
    }


    class OverlaButtonDrawHelper : OverlayElementDrawHelperBase {
        readonly Image hotImage;
        readonly Image image;
        readonly Action clickAction;
        Rectangle imageRect;
        Point mousePos;
        Image currentImage;

        public OverlaButtonDrawHelper(Image hotImage, Image image, Action clickAction) {
            this.imageRect = Rectangle.Empty;
            this.hotImage = hotImage;
            this.image = image;
            this.clickAction = clickAction;
        }
        protected override bool ProcessMessage(ref Message msg) {
            if(msg.Msg == MSG.WM_NCHITTEST) {
                msg.Result = new IntPtr(NativeMethods.HT.HTCLIENT);
                return true;
            }
            if(msg.Msg == MSG.WM_LBUTTONDOWN) {
                if(imageRect.Contains(mousePos)) {
                    clickAction();
                    return true;
                }
            }
            return base.ProcessMessage(ref msg);
        }
        protected override void DrawCore(OverlayWindowCustomDrawContext context) {
            context.DrawArgs.Cache.DrawImage(currentImage, imageRect);
        }

        protected override void CalculateLayout(OverlayLayeredWindowObjectInfoArgs drawArgs) {
            this.imageRect = CalcButtonRect(drawArgs);
            this.mousePos = CalcMousePosition(drawArgs.ViewInfo.Owner);
            this.currentImage = imageRect.Contains(mousePos) ? hotImage : image;
        }
        Rectangle CalcButtonRect(OverlayLayeredWindowObjectInfoArgs drawArgs) {
            Point loc = new Point((drawArgs.Bounds.Width - image.Width) / 2, image.Height / 2);
            return new Rectangle(loc, image.Size);
        }
        Point CalcMousePosition(OverlayLayeredWindow window) {
            NativeMethods.POINT point = new NativeMethods.POINT(Control.MousePosition);
            NativeAPI.ScreenToClient(window.Handle, ref point);
            return point.ToPoint();
        }
    }
}
