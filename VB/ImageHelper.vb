Imports System.Drawing
Imports DevExpress.LookAndFeel
Imports DevExpress.Skins
Imports DevExpress.Utils
Imports DevExpress.Utils.Drawing
Imports DevExpress.Utils.Svg

Namespace DxSample

    Public Module ImageHelper

        Public Function CreateImage(ByVal data As Byte(), ByVal Optional skinProvider As ISkinProvider = Nothing) As Image
            Dim svgBitmap As SvgBitmap = New SvgBitmap(data)
            Return svgBitmap.Render(SvgPaletteHelper.GetSvgPalette(If(skinProvider, UserLookAndFeel.Default), ObjectState.Normal), ScaleUtils.GetScaleFactor().Height)
        End Function
    End Module
End Namespace
