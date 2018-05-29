Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading.Tasks
Imports DevExpress.Utils.Drawing.Helpers

Namespace DxSample
    Public NotInheritable Class NativeAPI

        Private Sub New()
        End Sub

        Public Shared Function ScreenToClient(ByVal hWnd As IntPtr, ByRef lpPoint As NativeMethods.POINT) As Boolean
            Return Core.ScreenToClient(hWnd, lpPoint)
        End Function
        Private NotInheritable Class Core

            Private Sub New()
            End Sub

            <DllImport("user32.dll")> _
            Public Shared Function ScreenToClient(ByVal hWnd As IntPtr, ByRef lpPoint As NativeMethods.POINT) As Boolean
            End Function
        End Class
    End Class
End Namespace
