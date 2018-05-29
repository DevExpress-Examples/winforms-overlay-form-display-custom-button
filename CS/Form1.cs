using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;

namespace DxSample {
    public partial class Form1 : RibbonForm {
        TaskHelper taskHelper;
        Image buttonImage;
        Image hotButtonImage;

        OverlayTextPainter overlayLabel;
        OverlayImagePainter overlayButton;

        public Form1() {
            this.buttonImage = CreateButtonImage();
            this.hotButtonImage = CreateHotButtonImage();
            this.overlayLabel = new OverlayTextPainter();
            this.overlayButton = new OverlayImagePainter(buttonImage, hotButtonImage, OnCancelButtonClick);
            InitializeComponent();
        }

        Image CreateButtonImage() {
            return ImageHelper.CreateImage(Properties.Resources.cancel_normal);
        }
        Image CreateHotButtonImage() {
            return ImageHelper.CreateImage(Properties.Resources.cancel_active);
        }

        async void OnRunTaskItemClick(object sender, ItemClickEventArgs e) {
            biRunTask.Enabled = false;
            string taskResult;
            IOverlaySplashScreenHandle overlayHandle = SplashScreenManager.ShowOverlayForm(contentPanel, customPainter: new OverlayWindowCompositePainter(overlayLabel, overlayButton));
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
            overlayLabel.Text = value.ToString() + "%";
        }
    }
}
