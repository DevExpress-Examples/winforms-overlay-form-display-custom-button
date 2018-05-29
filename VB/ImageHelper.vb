Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports DevExpress.LookAndFeel
Imports DevExpress.Skins
Imports DevExpress.Utils
Imports DevExpress.Utils.Drawing
Imports DevExpress.Utils.Svg

Namespace DxSample
    Public NotInheritable Class ImageHelper

        Private Sub New()
        End Sub

        Public Shared Function CreateImage(ByVal data() As Byte, Optional ByVal skinProvider As ISkinProvider = Nothing) As Image
            Dim svgBitmap As New SvgBitmap(data)
            Return svgBitmap.Render(SvgPaletteHelper.GetSvgPalette(If(skinProvider, UserLookAndFeel.Default), ObjectState.Normal), ScaleUtils.GetScaleFactor().Height)
        End Function
    End Class
End Namespace
