Namespace DxSample

    Partial Class Form1

        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                Me.buttonImage.Dispose()
                Me.hotButtonImage.Dispose()
                If Me.components IsNot Nothing Then Me.components.Dispose()
            End If

            MyBase.Dispose(disposing)
        End Sub

#Region "Windows Form Designer generated code"
        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Me.ribbonControl1 = New DevExpress.XtraBars.Ribbon.RibbonControl()
            Me.biRunTask = New DevExpress.XtraBars.BarButtonItem()
            Me.ribbonPage1 = New DevExpress.XtraBars.Ribbon.RibbonPage()
            Me.ribbonPageGroup1 = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
            Me.contentPanel = New DevExpress.XtraEditors.PanelControl()
            CType((Me.ribbonControl1), System.ComponentModel.ISupportInitialize).BeginInit()
            CType((Me.contentPanel), System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            ' 
            ' ribbonControl1
            ' 
            Me.ribbonControl1.ExpandCollapseItem.Id = 0
            Me.ribbonControl1.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.ribbonControl1.ExpandCollapseItem, Me.biRunTask})
            Me.ribbonControl1.Location = New System.Drawing.Point(0, 0)
            Me.ribbonControl1.MaxItemId = 2
            Me.ribbonControl1.Name = "ribbonControl1"
            Me.ribbonControl1.Pages.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPage() {Me.ribbonPage1})
            Me.ribbonControl1.Size = New System.Drawing.Size(914, 162)
            ' 
            ' biRunTask
            ' 
            Me.biRunTask.Caption = "Run Task"
            Me.biRunTask.Id = 1
            Me.biRunTask.Name = "biRunTask"
            AddHandler Me.biRunTask.ItemClick, New DevExpress.XtraBars.ItemClickEventHandler(AddressOf Me.OnRunTaskItemClick)
            ' 
            ' ribbonPage1
            ' 
            Me.ribbonPage1.Groups.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPageGroup() {Me.ribbonPageGroup1})
            Me.ribbonPage1.Name = "ribbonPage1"
            Me.ribbonPage1.Text = "ribbonPage1"
            ' 
            ' ribbonPageGroup1
            ' 
            Me.ribbonPageGroup1.ItemLinks.Add(Me.biRunTask)
            Me.ribbonPageGroup1.Name = "ribbonPageGroup1"
            Me.ribbonPageGroup1.Text = "ribbonPageGroup1"
            ' 
            ' contentPanel
            ' 
            Me.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill
            Me.contentPanel.Location = New System.Drawing.Point(0, 162)
            Me.contentPanel.Name = "contentPanel"
            Me.contentPanel.Size = New System.Drawing.Size(914, 401)
            Me.contentPanel.TabIndex = 1
            ' 
            ' Form1
            ' 
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(914, 563)
            Me.Controls.Add(Me.contentPanel)
            Me.Controls.Add(Me.ribbonControl1)
            Me.MinimumSize = New System.Drawing.Size(750, 500)
            Me.Name = "Form1"
            Me.Ribbon = Me.ribbonControl1
            Me.Text = "Form1"
            CType((Me.ribbonControl1), System.ComponentModel.ISupportInitialize).EndInit()
            CType((Me.contentPanel), System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub

#End Region
        Private ribbonControl1 As DevExpress.XtraBars.Ribbon.RibbonControl

        Private ribbonPage1 As DevExpress.XtraBars.Ribbon.RibbonPage

        Private ribbonPageGroup1 As DevExpress.XtraBars.Ribbon.RibbonPageGroup

        Private biRunTask As DevExpress.XtraBars.BarButtonItem

        Private contentPanel As DevExpress.XtraEditors.PanelControl
    End Class
End Namespace
