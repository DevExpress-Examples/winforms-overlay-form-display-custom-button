# Overlay Form Custom Painter
This example shows how to customize an [Overlay Form](https://documentation.devexpress.com/WindowsForms/120029/Controls-and-Libraries/Forms-and-User-Controls/Splash-Screen-Manager/Overlay-Form).
## Overview
An Overlay Form is a semi-transparent splash screen that runs in a background thread and overlays a control or form to prevent access to it. An Overlay Form contains only a wait indicator by default. 

![Default Overlay Form](HelpResources/Default.png)

This example shows how to display:
* a percentage label below the wait indicator;
* a custom button that terminates the processed task and closes the Overlay Form.

![Custom Overlay Form](HelpResources/Custom.png)

The example provides you with the capability to change the label text, button glyph and the action performed on a click.

## Implementation details

You can customize Overlay Form drawing as follows:
* Inherit from the `OverlayWindowPainterBase` class; 
* Override the `Draw` method;
* Pass the created object as a parameter to the `SplashScreenManager.ShowOverlayForm` method.

In this example, you are provided with the `OverlayLabelDrawHelper` and `OverlaButtonDrawHelper` objects. Both are `OverlayWindowPainterBase` descendants which implement the drawing logic for the image and text, respectively. You can use their properties to customize the image and text. The `OverlayWindowCompositePainter` object composes the drawing logic in the `OverlayLabelDrawHelper` and `OverlaButtonDrawHelper` objects and is passed to the `SplashScreenManager.ShowOverlayForm` method as a parameter.

The actual operation performed while the Overlay Form is shown is implemented using the cancellable task available the .Net Framework Class Labrary (Task Parallel Library (TPL)).