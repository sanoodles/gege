<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BodyForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BodyForm))
        Me.NewVariableToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SetValueToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuStrip2 = New System.Windows.Forms.MenuStrip
        Me.GrafcetMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.AddstepToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.AddactionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.AdddToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.DeleteSelectedElementsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.WorksheetToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ResizeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SaveAsJPEGToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.sdgJPEGFile = New System.Windows.Forms.SaveFileDialog
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.StatusBarPanel1 = New System.Windows.Forms.StatusBarPanel
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.Grid1 = New System.Windows.Forms.DataGridView
        Me.colScope = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.colName = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.colType = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.colApplication = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.colInitialValue = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip
        Me.NewIniStepButton = New System.Windows.Forms.ToolStripButton
        Me.NewStepButton = New System.Windows.Forms.ToolStripButton
        Me.NewMacroStepButton = New System.Windows.Forms.ToolStripButton
        Me.EnclosingStepButton = New System.Windows.Forms.ToolStripButton
        Me.NewForcingOrderButton = New System.Windows.Forms.ToolStripButton
        Me.NewActionButton = New System.Windows.Forms.ToolStripButton
        Me.NewTransButton = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.SetFinButton = New System.Windows.Forms.ToolStripButton
        Me.DelButton = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
        Me.ShowCursor = New System.Windows.Forms.ToolStripButton
        Me.MenuStrip2.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.StatusBarPanel1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.Grid1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'NewVariableToolStripMenuItem
        '
        Me.NewVariableToolStripMenuItem.Name = "NewVariableToolStripMenuItem"
        Me.NewVariableToolStripMenuItem.Size = New System.Drawing.Size(81, 20)
        Me.NewVariableToolStripMenuItem.Text = "&New variable"
        '
        'SetValueToolStripMenuItem
        '
        Me.SetValueToolStripMenuItem.Name = "SetValueToolStripMenuItem"
        Me.SetValueToolStripMenuItem.Size = New System.Drawing.Size(64, 20)
        Me.SetValueToolStripMenuItem.Text = "&Set value"
        '
        'MenuStrip2
        '
        Me.MenuStrip2.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewVariableToolStripMenuItem, Me.SetValueToolStripMenuItem})
        Me.MenuStrip2.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip2.Name = "MenuStrip2"
        Me.MenuStrip2.Size = New System.Drawing.Size(767, 24)
        Me.MenuStrip2.TabIndex = 19
        Me.MenuStrip2.Text = "MenuStrip2"
        '
        'GrafcetMenuItem
        '
        Me.GrafcetMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddstepToolStripMenuItem, Me.AddactionToolStripMenuItem, Me.AdddToolStripMenuItem, Me.DeleteSelectedElementsToolStripMenuItem})
        Me.GrafcetMenuItem.Name = "GrafcetMenuItem"
        Me.GrafcetMenuItem.Size = New System.Drawing.Size(55, 20)
        Me.GrafcetMenuItem.Text = "&Grafcet"
        '
        'AddstepToolStripMenuItem
        '
        Me.AddstepToolStripMenuItem.Name = "AddstepToolStripMenuItem"
        Me.AddstepToolStripMenuItem.Size = New System.Drawing.Size(205, 22)
        Me.AddstepToolStripMenuItem.Text = "Add &step"
        '
        'AddactionToolStripMenuItem
        '
        Me.AddactionToolStripMenuItem.Name = "AddactionToolStripMenuItem"
        Me.AddactionToolStripMenuItem.Size = New System.Drawing.Size(205, 22)
        Me.AddactionToolStripMenuItem.Text = "Add &action"
        '
        'AdddToolStripMenuItem
        '
        Me.AdddToolStripMenuItem.Name = "AdddToolStripMenuItem"
        Me.AdddToolStripMenuItem.Size = New System.Drawing.Size(205, 22)
        Me.AdddToolStripMenuItem.Text = "Add &transition"
        '
        'DeleteSelectedElementsToolStripMenuItem
        '
        Me.DeleteSelectedElementsToolStripMenuItem.Name = "DeleteSelectedElementsToolStripMenuItem"
        Me.DeleteSelectedElementsToolStripMenuItem.Size = New System.Drawing.Size(205, 22)
        Me.DeleteSelectedElementsToolStripMenuItem.Text = "&Delete selected elements"
        '
        'WorksheetToolStripMenuItem
        '
        Me.WorksheetToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ResizeToolStripMenuItem, Me.SaveAsJPEGToolStripMenuItem})
        Me.WorksheetToolStripMenuItem.Name = "WorksheetToolStripMenuItem"
        Me.WorksheetToolStripMenuItem.Size = New System.Drawing.Size(71, 20)
        Me.WorksheetToolStripMenuItem.Text = "&Worksheet"
        '
        'ResizeToolStripMenuItem
        '
        Me.ResizeToolStripMenuItem.Name = "ResizeToolStripMenuItem"
        Me.ResizeToolStripMenuItem.Size = New System.Drawing.Size(162, 22)
        Me.ResizeToolStripMenuItem.Text = "&Resize..."
        '
        'SaveAsJPEGToolStripMenuItem
        '
        Me.SaveAsJPEGToolStripMenuItem.Name = "SaveAsJPEGToolStripMenuItem"
        Me.SaveAsJPEGToolStripMenuItem.Size = New System.Drawing.Size(162, 22)
        Me.SaveAsJPEGToolStripMenuItem.Text = "&Save as JPEG..."
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GrafcetMenuItem, Me.WorksheetToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(767, 24)
        Me.MenuStrip1.TabIndex = 15
        Me.MenuStrip1.Text = "MenuStrip1"
        Me.MenuStrip1.Visible = False
        '
        'sdgJPEGFile
        '
        Me.sdgJPEGFile.Filter = "JPEG(*.jpg)|*.jpg"
        Me.sdgJPEGFile.Title = "Save as JPEG"
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "")
        Me.ImageList1.Images.SetKeyName(1, "")
        Me.ImageList1.Images.SetKeyName(2, "")
        Me.ImageList1.Images.SetKeyName(3, "")
        Me.ImageList1.Images.SetKeyName(4, "")
        Me.ImageList1.Images.SetKeyName(5, "transition.PNG")
        Me.ImageList1.Images.SetKeyName(6, "step.PNG")
        Me.ImageList1.Images.SetKeyName(7, "")
        Me.ImageList1.Images.SetKeyName(8, "")
        Me.ImageList1.Images.SetKeyName(9, "macro-step.PNG")
        Me.ImageList1.Images.SetKeyName(10, "initial-step.PNG")
        Me.ImageList1.Images.SetKeyName(11, "delete.PNG")
        Me.ImageList1.Images.SetKeyName(12, "action.PNG")
        Me.ImageList1.Images.SetKeyName(13, "")
        Me.ImageList1.Images.SetKeyName(14, "")
        Me.ImageList1.Images.SetKeyName(15, "final-step.PNG")
        Me.ImageList1.Images.SetKeyName(16, "")
        Me.ImageList1.Images.SetKeyName(17, "")
        Me.ImageList1.Images.SetKeyName(18, "Data_Dataset.ico")
        Me.ImageList1.Images.SetKeyName(19, "a.PNG")
        Me.ImageList1.Images.SetKeyName(20, "forcing-order.PNG")
        Me.ImageList1.Images.SetKeyName(21, "enclosing-step.png")
        '
        'StatusBarPanel1
        '
        Me.StatusBarPanel1.Name = "StatusBarPanel1"
        Me.StatusBarPanel1.Text = "Mode:"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 24)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.Grid1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.Panel1)
        Me.SplitContainer1.Size = New System.Drawing.Size(767, 509)
        Me.SplitContainer1.SplitterDistance = 194
        Me.SplitContainer1.TabIndex = 23
        '
        'Grid1
        '
        Me.Grid1.AllowUserToAddRows = False
        Me.Grid1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.Grid1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colScope, Me.colName, Me.colType, Me.colApplication, Me.colInitialValue})
        Me.Grid1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Grid1.Location = New System.Drawing.Point(0, 0)
        Me.Grid1.MultiSelect = False
        Me.Grid1.Name = "Grid1"
        Me.Grid1.Size = New System.Drawing.Size(767, 194)
        Me.Grid1.TabIndex = 23
        '
        'colScope
        '
        Me.colScope.HeaderText = "Scope"
        Me.colScope.Name = "colScope"
        '
        'colName
        '
        Me.colName.HeaderText = "Name"
        Me.colName.Name = "colName"
        Me.colName.ReadOnly = True
        '
        'colType
        '
        Me.colType.HeaderText = "Type"
        Me.colType.Name = "colType"
        Me.colType.ReadOnly = True
        Me.colType.Width = 80
        '
        'colApplication
        '
        Me.colApplication.HeaderText = "Application"
        Me.colApplication.Name = "colApplication"
        Me.colApplication.Width = 80
        '
        'colInitialValue
        '
        Me.colInitialValue.HeaderText = "Initial Value"
        Me.colInitialValue.Name = "colInitialValue"
        Me.colInitialValue.ReadOnly = True
        Me.colInitialValue.Width = 90
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.White
        Me.Panel1.Controls.Add(Me.ToolStrip1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(767, 311)
        Me.Panel1.TabIndex = 23
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewIniStepButton, Me.NewStepButton, Me.NewMacroStepButton, Me.EnclosingStepButton, Me.NewForcingOrderButton, Me.NewActionButton, Me.NewTransButton, Me.ToolStripSeparator1, Me.SetFinButton, Me.DelButton, Me.ToolStripSeparator2, Me.ShowCursor})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(767, 25)
        Me.ToolStrip1.TabIndex = 19
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'NewIniStepButton
        '
        Me.NewIniStepButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.NewIniStepButton.Image = CType(resources.GetObject("NewIniStepButton.Image"), System.Drawing.Image)
        Me.NewIniStepButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.NewIniStepButton.Name = "NewIniStepButton"
        Me.NewIniStepButton.Size = New System.Drawing.Size(23, 22)
        Me.NewIniStepButton.Text = "S ini"
        Me.NewIniStepButton.ToolTipText = "Add initial step"
        '
        'NewStepButton
        '
        Me.NewStepButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.NewStepButton.Image = CType(resources.GetObject("NewStepButton.Image"), System.Drawing.Image)
        Me.NewStepButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.NewStepButton.Name = "NewStepButton"
        Me.NewStepButton.Size = New System.Drawing.Size(23, 22)
        Me.NewStepButton.Text = "S"
        Me.NewStepButton.ToolTipText = "Add step"
        '
        'NewMacroStepButton
        '
        Me.NewMacroStepButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.NewMacroStepButton.Image = CType(resources.GetObject("NewMacroStepButton.Image"), System.Drawing.Image)
        Me.NewMacroStepButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.NewMacroStepButton.Name = "NewMacroStepButton"
        Me.NewMacroStepButton.Size = New System.Drawing.Size(23, 22)
        Me.NewMacroStepButton.Text = "MS"
        Me.NewMacroStepButton.ToolTipText = "Add macro step"
        '
        'EnclosingStepButton
        '
        Me.EnclosingStepButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.EnclosingStepButton.Image = CType(resources.GetObject("EnclosingStepButton.Image"), System.Drawing.Image)
        Me.EnclosingStepButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.EnclosingStepButton.Name = "EnclosingStepButton"
        Me.EnclosingStepButton.Size = New System.Drawing.Size(23, 22)
        Me.EnclosingStepButton.Text = "ES"
        Me.EnclosingStepButton.ToolTipText = "Add enclosing step"
        '
        'NewForcingOrderButton
        '
        Me.NewForcingOrderButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.NewForcingOrderButton.Image = CType(resources.GetObject("NewForcingOrderButton.Image"), System.Drawing.Image)
        Me.NewForcingOrderButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.NewForcingOrderButton.Name = "NewForcingOrderButton"
        Me.NewForcingOrderButton.Size = New System.Drawing.Size(23, 22)
        Me.NewForcingOrderButton.Text = "F"
        Me.NewForcingOrderButton.ToolTipText = "Add forcing order"
        '
        'NewActionButton
        '
        Me.NewActionButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.NewActionButton.Image = CType(resources.GetObject("NewActionButton.Image"), System.Drawing.Image)
        Me.NewActionButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.NewActionButton.Name = "NewActionButton"
        Me.NewActionButton.Size = New System.Drawing.Size(23, 22)
        Me.NewActionButton.Text = "A"
        Me.NewActionButton.ToolTipText = " Add action"
        '
        'NewTransButton
        '
        Me.NewTransButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.NewTransButton.Image = CType(resources.GetObject("NewTransButton.Image"), System.Drawing.Image)
        Me.NewTransButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.NewTransButton.Name = "NewTransButton"
        Me.NewTransButton.Size = New System.Drawing.Size(23, 22)
        Me.NewTransButton.Text = "T"
        Me.NewTransButton.ToolTipText = "Add transition"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'SetFinButton
        '
        Me.SetFinButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.SetFinButton.Image = CType(resources.GetObject("SetFinButton.Image"), System.Drawing.Image)
        Me.SetFinButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.SetFinButton.Name = "SetFinButton"
        Me.SetFinButton.Size = New System.Drawing.Size(23, 22)
        Me.SetFinButton.Text = "Fin"
        Me.SetFinButton.ToolTipText = "Set step as final"
        '
        'DelButton
        '
        Me.DelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.DelButton.Image = CType(resources.GetObject("DelButton.Image"), System.Drawing.Image)
        Me.DelButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.DelButton.Name = "DelButton"
        Me.DelButton.Size = New System.Drawing.Size(23, 22)
        Me.DelButton.Text = "Del"
        Me.DelButton.ToolTipText = "Delete selected item"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'ShowCursor
        '
        Me.ShowCursor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ShowCursor.Image = CType(resources.GetObject("ShowCursor.Image"), System.Drawing.Image)
        Me.ShowCursor.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ShowCursor.Name = "ShowCursor"
        Me.ShowCursor.Size = New System.Drawing.Size(23, 22)
        Me.ShowCursor.Text = "Cur"
        Me.ShowCursor.ToolTipText = "Show cursor"
        '
        'BodyForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(767, 533)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.MenuStrip2)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "BodyForm"
        Me.Text = "BodyForm"
        Me.MenuStrip2.ResumeLayout(False)
        Me.MenuStrip2.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        CType(Me.StatusBarPanel1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.Grid1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents NewVariableToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SetValueToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MenuStrip2 As System.Windows.Forms.MenuStrip
    Friend WithEvents GrafcetMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AddstepToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AddactionToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AdddToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DeleteSelectedElementsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents WorksheetToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ResizeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveAsJPEGToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents sdgJPEGFile As System.Windows.Forms.SaveFileDialog
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents StatusBarPanel1 As System.Windows.Forms.StatusBarPanel
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents Grid1 As System.Windows.Forms.DataGridView
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents NewIniStepButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents NewStepButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents NewMacroStepButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents NewActionButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents NewTransButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SetFinButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents DelButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ShowCursor As System.Windows.Forms.ToolStripButton
    Friend WithEvents colScope As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colName As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colType As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colApplication As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colInitialValue As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents NewForcingOrderButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents EnclosingStepButton As System.Windows.Forms.ToolStripButton
End Class
