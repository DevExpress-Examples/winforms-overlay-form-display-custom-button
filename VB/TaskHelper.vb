Imports System
Imports System.Threading
Imports System.Threading.Tasks

Namespace DxSample

    Public Class TaskHelper
        Implements IDisposable

        Private tokenSource As CancellationTokenSource

        Private cancellationToken As CancellationToken

        Public Sub New()
            tokenSource = New CancellationTokenSource()
            cancellationToken = tokenSource.Token
        End Sub

        Public Function StartTask(ByVal progress As IProgress(Of Integer)) As Task(Of String)
            Return StartTaskCore(progress)
        End Function

        Public Sub Cancel()
            tokenSource.Cancel()
        End Sub

        Private Function StartTaskCore(ByVal progress As IProgress(Of Integer)) As Task(Of String)
            Return Task.Run(Function() CalculateResult(progress), cancellationToken)
        End Function

        Private Function CalculateResult(ByVal progress As IProgress(Of Integer)) As String
            For n As Integer = 0 To 100
                ' do something helpful
                Thread.Sleep(60)
                cancellationToken.ThrowIfCancellationRequested()
                If progress IsNot Nothing Then progress.Report(n)
            Next

            Return "CalculatedValue"
        End Function

#Region "IDisposable"
        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
        End Sub

        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If tokenSource IsNot Nothing Then tokenSource.Dispose()
            End If

            tokenSource = Nothing
        End Sub
#End Region
    End Class
End Namespace
