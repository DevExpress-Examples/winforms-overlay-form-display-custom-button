Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Drawing.Text
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports DevExpress.Utils.Drawing
Imports DevExpress.Utils.Drawing.Helpers
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

        Private overlayLabelDrawHelper As OverlayLabelDrawHelper
        Private overlayButtonDrawHelper As OverlaButtonDrawHelper

        Public Sub New()
            Me.buttonImage = CreateButtonImage()
            Me.hotButtonImage = CreateHotButtonImage()
            Me.overlayLabelDrawHelper = New OverlayLabelDrawHelper()
            Me.overlayButtonDrawHelper = New OverlaButtonDrawHelper(hotButtonImage, buttonImage, AddressOf OnCancelButtonClick)
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
            Dim overlayHandle As IOverlaySplashScreenHandle = SplashScreenManager.ShowOverlayForm(contentPanel, customPainter:= New OverlayWindowCompositePainter(overlayLabelDrawHelper, overlayButtonDrawHelper))
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
            overlayLabelDrawHelper.Text = value.ToString() & "%"
        End Sub
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                buttonImage.Dispose()
                hotButtonImage.Dispose()
                If components IsNot Nothing Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub
    End Class



    Friend MustInherit Class OverlayElementDrawHelperBase
        Inherits OverlayWindowPainterBase

        Public Sub New()
        End Sub
        Protected NotOverridable Overrides Sub Draw(ByVal context As OverlayWindowCustomDrawContext)
            CalculateLayout(context.DrawArgs)
            context.DefaultDraw()
            If CanDraw Then
                DrawCore(context)
            End If
            context.Handled = True
        End Sub
        Protected Overridable Sub CalculateLayout(ByVal drawArgs As OverlayLayeredWindowObjectInfoArgs)
        End Sub
        Protected Overridable ReadOnly Property CanDraw() As Boolean
            Get
                Return True
            End Get
        End Property
        Protected MustOverride Sub DrawCore(ByVal context As OverlayWindowCustomDrawContext)
    End Class


    Friend Class OverlayLabelDrawHelper
        Inherits OverlayElementDrawHelperBase


        Private text_Renamed As String

        Private color_Renamed As Color
        Private textPos As Point

        Private Shared ReadOnly font As New Font("Tahoma", 18)

        Public Sub New()
            Me.textPos = Point.Empty
            Me.color_Renamed = System.Drawing.Color.Black
            Me.text_Renamed = String.Empty
        End Sub
        Public Property Text() As String
            Get
                Return text_Renamed
            End Get
            Set(ByVal value As String)
                text_Renamed = value
            End Set
        End Property
        Public Property Color() As Color
            Get
                Return color_Renamed
            End Get
            Set(ByVal value As Color)
                color_Renamed = value
            End Set
        End Property

        Protected Overrides ReadOnly Property CanDraw() As Boolean
            Get
                If String.IsNullOrEmpty(Text) Then
                    Return False
                End If
                Return True
            End Get
        End Property
        Protected Overrides Sub DrawCore(ByVal context As OverlayWindowCustomDrawContext)
            Dim cache As GraphicsCache = context.DrawArgs.Cache
            Dim prev As TextRenderingHint = cache.TextRenderingHint
            cache.TextRenderingHint = TextRenderingHint.AntiAlias
            Try
                cache.DrawString(Text, font, cache.GetSolidBrush(Color), textPos)
            Finally
                cache.TextRenderingHint = prev
            End Try
        End Sub

        Protected Overrides Sub CalculateLayout(ByVal drawArgs As OverlayLayeredWindowObjectInfoArgs)
            Me.textPos = CalcTextPosition(drawArgs)
        End Sub
        Private Function CalcTextPosition(ByVal drawArgs As OverlayLayeredWindowObjectInfoArgs) As Point
            If Not CanDraw Then
                Return Point.Empty
            End If
            Dim textSize As Size = drawArgs.Cache.CalcTextSize(Text, font).ToSize()
            Return New Point((drawArgs.Bounds.Width - textSize.Width) \ 2, drawArgs.ViewInfo.ImageBounds.Bottom + textSize.Height)
        End Function
    End Class


    Friend Class OverlaButtonDrawHelper
        Inherits OverlayElementDrawHelperBase

        Private ReadOnly hotImage As Image
        Private ReadOnly image As Image
        Private ReadOnly clickAction As Action
        Private imageRect As Rectangle
        Private mousePos As Point
        Private currentImage As Image

        Public Sub New(ByVal hotImage As Image, ByVal image As Image, ByVal clickAction As Action)
            Me.imageRect = Rectangle.Empty
            Me.hotImage = hotImage
            Me.image = image
            Me.clickAction = clickAction
        End Sub
        Protected Overrides Function ProcessMessage(ByRef msg As Message) As Boolean
            If msg.Msg = MSG.WM_NCHITTEST Then
                msg.Result = New IntPtr(NativeMethods.HT.HTCLIENT)
                Return True
            End If
            If msg.Msg = MSG.WM_LBUTTONDOWN Then
                If imageRect.Contains(mousePos) Then
                    clickAction()
                    Return True
                End If
            End If
            Return MyBase.ProcessMessage(msg)
        End Function
        Protected Overrides Sub DrawCore(ByVal context As OverlayWindowCustomDrawContext)
            context.DrawArgs.Cache.DrawImage(currentImage, imageRect)
        End Sub

        Protected Overrides Sub CalculateLayout(ByVal drawArgs As OverlayLayeredWindowObjectInfoArgs)
            Me.imageRect = CalcButtonRect(drawArgs)
            Me.mousePos = CalcMousePosition(drawArgs.ViewInfo.Owner)
            Me.currentImage = If(imageRect.Contains(mousePos), hotImage, image)
        End Sub
        Private Function CalcButtonRect(ByVal drawArgs As OverlayLayeredWindowObjectInfoArgs) As Rectangle
            Dim loc As New Point((drawArgs.Bounds.Width - image.Width) \ 2, image.Height \ 2)
            Return New Rectangle(loc, image.Size)
        End Function
        Private Function CalcMousePosition(ByVal window As OverlayLayeredWindow) As Point
            Dim point As New NativeMethods.POINT(Control.MousePosition)
            NativeAPI.ScreenToClient(window.Handle, point)
            Return point.ToPoint()
        End Function
    End Class
End Namespace
