Imports System
Imports System.Runtime.InteropServices
Imports DevExpress.Utils.Drawing.Helpers

Namespace DxSample

    Public Module NativeAPI

        Public Function ScreenToClient(ByVal hWnd As IntPtr, ByRef lpPoint As NativeMethods.POINT) As Boolean
            Return Core.ScreenToClient(hWnd, lpPoint)
        End Function

        Private NotInheritable Class Core

            <DllImport("user32.dll")>
            Public Shared Function ScreenToClient(ByVal hWnd As IntPtr, ByRef lpPoint As NativeMethods.POINT) As Boolean
            End Function
        End Class
    End Module
End Namespace
