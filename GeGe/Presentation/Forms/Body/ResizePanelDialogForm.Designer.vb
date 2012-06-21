<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ResizePanelDialogForm
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
        Me.rdbPixels = New System.Windows.Forms.RadioButton
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.rdbPercentage = New System.Windows.Forms.RadioButton
        Me.Button2 = New System.Windows.Forms.Button
        Me.Button1 = New System.Windows.Forms.Button
        Me.lblReadAmount = New System.Windows.Forms.Label
        Me.tkbAmount = New System.Windows.Forms.TrackBar
        Me.chkY = New System.Windows.Forms.CheckBox
        Me.chkX = New System.Windows.Forms.CheckBox
        Me.rdbDecreaseSize = New System.Windows.Forms.RadioButton
        Me.rdbIncreaseSize = New System.Windows.Forms.RadioButton
        Me.Panel1.SuspendLayout()
        CType(Me.tkbAmount, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rdbPixels
        '
        Me.rdbPixels.AutoSize = True
        Me.rdbPixels.Location = New System.Drawing.Point(-6, 29)
        Me.rdbPixels.Name = "rdbPixels"
        Me.rdbPixels.Size = New System.Drawing.Size(51, 17)
        Me.rdbPixels.TabIndex = 3
        Me.rdbPixels.Text = "pixels"
        Me.rdbPixels.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.rdbPixels)
        Me.Panel1.Controls.Add(Me.rdbPercentage)
        Me.Panel1.Location = New System.Drawing.Point(95, 175)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(200, 51)
        Me.Panel1.TabIndex = 16
        '
        'rdbPercentage
        '
        Me.rdbPercentage.AutoSize = True
        Me.rdbPercentage.Checked = True
        Me.rdbPercentage.Location = New System.Drawing.Point(-6, 6)
        Me.rdbPercentage.Name = "rdbPercentage"
        Me.rdbPercentage.Size = New System.Drawing.Size(61, 17)
        Me.rdbPercentage.TabIndex = 2
        Me.rdbPercentage.TabStop = True
        Me.rdbPercentage.Text = "percent"
        Me.rdbPercentage.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.BackColor = System.Drawing.SystemColors.Control
        Me.Button2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Button2.Location = New System.Drawing.Point(409, 335)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 15
        Me.Button2.Text = "Cancel"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'Button1
        '
        Me.Button1.BackColor = System.Drawing.SystemColors.Control
        Me.Button1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Button1.Location = New System.Drawing.Point(12, 335)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 14
        Me.Button1.Text = "OK"
        Me.Button1.UseVisualStyleBackColor = False
        '
        'lblReadAmount
        '
        Me.lblReadAmount.AutoSize = True
        Me.lblReadAmount.Location = New System.Drawing.Point(92, 146)
        Me.lblReadAmount.Name = "lblReadAmount"
        Me.lblReadAmount.Size = New System.Drawing.Size(19, 13)
        Me.lblReadAmount.TabIndex = 13
        Me.lblReadAmount.Text = "10"
        '
        'tkbAmount
        '
        Me.tkbAmount.Location = New System.Drawing.Point(95, 83)
        Me.tkbAmount.Maximum = 100
        Me.tkbAmount.Name = "tkbAmount"
        Me.tkbAmount.Size = New System.Drawing.Size(354, 42)
        Me.tkbAmount.TabIndex = 12
        Me.tkbAmount.TickFrequency = 10
        Me.tkbAmount.Value = 10
        '
        'chkY
        '
        Me.chkY.AutoSize = True
        Me.chkY.Checked = True
        Me.chkY.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkY.Location = New System.Drawing.Point(95, 279)
        Me.chkY.Name = "chkY"
        Me.chkY.Size = New System.Drawing.Size(83, 17)
        Me.chkY.TabIndex = 10
        Me.chkY.Text = "along Y axis"
        Me.chkY.UseVisualStyleBackColor = True
        '
        'chkX
        '
        Me.chkX.AutoSize = True
        Me.chkX.Checked = True
        Me.chkX.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkX.Location = New System.Drawing.Point(95, 256)
        Me.chkX.Name = "chkX"
        Me.chkX.Size = New System.Drawing.Size(83, 17)
        Me.chkX.TabIndex = 11
        Me.chkX.Text = "along X axis"
        Me.chkX.UseVisualStyleBackColor = True
        '
        'rdbDecreaseSize
        '
        Me.rdbDecreaseSize.AutoSize = True
        Me.rdbDecreaseSize.Location = New System.Drawing.Point(12, 46)
        Me.rdbDecreaseSize.Name = "rdbDecreaseSize"
        Me.rdbDecreaseSize.Size = New System.Drawing.Size(152, 17)
        Me.rdbDecreaseSize.TabIndex = 8
        Me.rdbDecreaseSize.Text = "Decrease workseet size by"
        Me.rdbDecreaseSize.UseVisualStyleBackColor = True
        '
        'rdbIncreaseSize
        '
        Me.rdbIncreaseSize.AutoSize = True
        Me.rdbIncreaseSize.Checked = True
        Me.rdbIncreaseSize.Location = New System.Drawing.Point(12, 23)
        Me.rdbIncreaseSize.Name = "rdbIncreaseSize"
        Me.rdbIncreaseSize.Size = New System.Drawing.Size(147, 17)
        Me.rdbIncreaseSize.TabIndex = 9
        Me.rdbIncreaseSize.TabStop = True
        Me.rdbIncreaseSize.Text = "Increase workseet size by"
        Me.rdbIncreaseSize.UseVisualStyleBackColor = True
        '
        'ResizePanelDialogForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(499, 374)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.lblReadAmount)
        Me.Controls.Add(Me.tkbAmount)
        Me.Controls.Add(Me.chkY)
        Me.Controls.Add(Me.chkX)
        Me.Controls.Add(Me.rdbDecreaseSize)
        Me.Controls.Add(Me.rdbIncreaseSize)
        Me.Name = "ResizePanelDialogForm"
        Me.Text = "ResizePanelDialogForm"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.tkbAmount, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents rdbPixels As System.Windows.Forms.RadioButton
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents rdbPercentage As System.Windows.Forms.RadioButton
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents lblReadAmount As System.Windows.Forms.Label
    Friend WithEvents tkbAmount As System.Windows.Forms.TrackBar
    Friend WithEvents chkY As System.Windows.Forms.CheckBox
    Friend WithEvents chkX As System.Windows.Forms.CheckBox
    Friend WithEvents rdbDecreaseSize As System.Windows.Forms.RadioButton
    Friend WithEvents rdbIncreaseSize As System.Windows.Forms.RadioButton
End Class
