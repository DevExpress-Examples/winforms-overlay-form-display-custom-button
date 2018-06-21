Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Drawing.Text
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Threading.Tasks
Imports DevExpress.XtraBars
Imports DevExpress.XtraBars.Ribbon
Imports DevExpress.XtraEditors
Imports DevExpress.XtraSplashScreen

Namespace DxSample
    Partial Public Class Form1
        Inherits RibbonForm

        Private taskHelper As TaskHelper
        Private buttonImage As Image
        Private hotButtonImage As Image

        Private overlayLabel As OverlayTextPainter
        Private overlayButton As OverlayImagePainter

        Public Sub New()
            Me.buttonImage = CreateButtonImage()
            Me.hotButtonImage = CreateHotButtonImage()
            Me.overlayLabel = New OverlayTextPainter()
            Me.overlayButton = New OverlayImagePainter(buttonImage, hotButtonImage, AddressOf OnCancelButtonClick)
            InitializeComponent()
        End Sub

        Private Function CreateButtonImage() As Image
            Return ImageHelper.CreateImage(My.Resources.cancel_normal)
        End Function
        Private Function CreateHotButtonImage() As Image
            Return ImageHelper.CreateImage(My.Resources.cancel_active)
        End Function

        Private Async Sub OnRunTaskItemClick(ByVal sender As Object, ByVal e As ItemClickEventArgs) Handles biRunTask.ItemClick
            biRunTask.Enabled = False
            Dim taskResult As String
            Dim overlayHandle As IOverlaySplashScreenHandle = SplashScreenManager.ShowOverlayForm(contentPanel, customPainter:= New OverlayWindowCompositePainter(overlayLabel, overlayButton))
            Try
                taskResult = Await RunTask()
            Finally
                SplashScreenManager.CloseOverlayForm(overlayHandle)
                biRunTask.Enabled = True
            End Try
            XtraMessageBox.Show(Me, taskResult, "Task Result")
        End Sub

        Private Async Function RunTask() As Task(Of String)
            Dim taskResult As String
            Me.taskHelper = New TaskHelper()
            Try
                taskResult = Await taskHelper.StartTask(New Progress(Of Integer)(AddressOf OnProgressChanged))
            Catch e1 As OperationCanceledException
                taskResult = "Operation is Cancelled"
            Finally
                taskHelper.Dispose()
                taskHelper = Nothing
            End Try
            Return taskResult
        End Function

        Private Sub OnCancelButtonClick()
            If taskHelper IsNot Nothing Then
                taskHelper.Cancel()
            End If
        End Sub
        Private Sub OnProgressChanged(ByVal value As Integer)
            overlayLabel.Text = value.ToString() & "%"
        End Sub
    End Class
End Namespace
