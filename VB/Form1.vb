Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports DevExpress.Utils.Drawing
Imports DevExpress.Utils.Drawing.Helpers
Imports DevExpress.XtraBars
Imports DevExpress.XtraBars.Ribbon
Imports DevExpress.XtraEditors
Imports DevExpress.XtraSplashScreen
Imports DxSample.Properties

Namespace DxSample

    Public Partial Class Form1
        Inherits RibbonForm

        Private taskHelper As TaskHelper

        Private buttonImage As Image

        Private hotButtonImage As Image

        Private overlayLabelDrawHelper As OverlayLabelDrawHelper

        Private overlayButtonDrawHelper As OverlaButtonDrawHelper

        Public Sub New()
            buttonImage = CreateButtonImage()
            hotButtonImage = CreateHotButtonImage()
            overlayLabelDrawHelper = New OverlayLabelDrawHelper()
            overlayButtonDrawHelper = New OverlaButtonDrawHelper(hotButtonImage, buttonImage, AddressOf OnCancelButtonClick)
            InitializeComponent()
        End Sub

        Private Function CreateButtonImage() As Image
            Return CreateImage(Resources.cancel_normal)
        End Function

        Private Function CreateHotButtonImage() As Image
            Return CreateImage(Resources.cancel_active)
        End Function

        Private Async Sub OnRunTaskItemClick(ByVal sender As Object, ByVal e As ItemClickEventArgs)
            biRunTask.Enabled = False
            Dim taskResult As String
            Dim overlayHandle As IOverlaySplashScreenHandle = SplashScreenManager.ShowOverlayForm(contentPanel, customPainter:=New OverlayWindowCompositePainter(overlayLabelDrawHelper, overlayButtonDrawHelper))
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
            taskHelper = New TaskHelper()
            Try
                taskResult = Await taskHelper.StartTask(New Progress(Of Integer)(AddressOf OnProgressChanged))
            Catch __unusedOperationCanceledException1__ As OperationCanceledException
                taskResult = "Operation is Cancelled"
            Finally
                taskHelper.Dispose()
                taskHelper = Nothing
            End Try

            Return taskResult
        End Function

        Private Sub OnCancelButtonClick()
            If taskHelper IsNot Nothing Then taskHelper.Cancel()
        End Sub

        Private Sub OnProgressChanged(ByVal value As Integer)
            overlayLabelDrawHelper.Text = value.ToString() & "%"
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                buttonImage.Dispose()
                hotButtonImage.Dispose()
                If components IsNot Nothing Then components.Dispose()
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

        Protected Overridable ReadOnly Property CanDraw As Boolean
            Get
                Return True
            End Get
        End Property

        Protected MustOverride Sub DrawCore(ByVal context As OverlayWindowCustomDrawContext)
    End Class

    Friend Class OverlayLabelDrawHelper
        Inherits OverlayElementDrawHelperBase

        Private textField As String

        Private colorField As Color

        Private textPos As Point

        Private Shared ReadOnly font As Font = New Font("Tahoma", 18)

        Public Sub New()
            textPos = Point.Empty
            colorField = Color.Black
            textField = String.Empty
        End Sub

        Public Property Text As String
            Get
                Return textField
            End Get

            Set(ByVal value As String)
                textField = value
            End Set
        End Property

        Public Property Color As Color
            Get
                Return colorField
            End Get

            Set(ByVal value As Color)
                colorField = value
            End Set
        End Property

        Protected Overrides ReadOnly Property CanDraw As Boolean
            Get
                If String.IsNullOrEmpty(Text) Then Return False
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
            textPos = CalcTextPosition(drawArgs)
        End Sub

        Private Function CalcTextPosition(ByVal drawArgs As OverlayLayeredWindowObjectInfoArgs) As Point
            If Not CanDraw Then Return Point.Empty
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
            imageRect = Rectangle.Empty
            Me.hotImage = hotImage
            Me.image = image
            Me.clickAction = clickAction
        End Sub

        Protected Overrides Function ProcessMessage(ByRef msg As Message) As Boolean
            If msg.Msg = Helpers.MSG.WM_NCHITTEST Then
                msg.Result = New IntPtr(NativeMethods.HT.HTCLIENT)
                Return True
            End If

            If msg.Msg = Helpers.MSG.WM_LBUTTONDOWN Then
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
            imageRect = CalcButtonRect(drawArgs)
            mousePos = CalcMousePosition(drawArgs.ViewInfo.Owner)
            currentImage = If(imageRect.Contains(mousePos), hotImage, image)
        End Sub

        Private Function CalcButtonRect(ByVal drawArgs As OverlayLayeredWindowObjectInfoArgs) As Rectangle
            Dim loc As Point = New Point((drawArgs.Bounds.Width - image.Width) \ 2, image.Height \ 2)
            Return New Rectangle(loc, image.Size)
        End Function

        Private Function CalcMousePosition(ByVal window As OverlayLayeredWindow) As Point
            Dim point As NativeMethods.POINT = New NativeMethods.POINT(Control.MousePosition)
            ScreenToClient(window.Handle, point)
            Return point.ToPoint()
        End Function
    End Class
End Namespace
