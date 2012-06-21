Imports System.Windows.Forms
Imports System.Drawing
Imports System

Public Class ResizePanelDialogForm

    Private m_OldSize As Size

    Public Sub New(ByVal sz As Size)
        MyBase.New()
        Me.InitializeComponent()
        m_OldSize = sz
    End Sub

    ' Calcola le modifiche alle dimensioni lungo entrambi gli assi
    ' e le combina
    Private Function CalculateXResize() As Integer
        Dim ret As Integer = 0
        If rdbPercentage.Checked Then ret = (m_OldSize.Width * tkbAmount.Value) / 100
        If rdbPixels.Checked Then ret = tkbAmount.Value
        If rdbDecreaseSize.Checked Then ret *= -1
        Return ret
    End Function
    Private Function CalculateYResize() As Integer
        Dim ret As Integer = 0
        If rdbPercentage.Checked Then ret = (m_OldSize.Height * tkbAmount.Value) / 100
        If rdbPixels.Checked Then ret = tkbAmount.Value
        If rdbDecreaseSize.Checked Then ret *= -1
        Return ret
    End Function
    Private Function CalculateFinalSize() As Size
        Dim xResize As Integer = 0
        Dim yResize As Integer = 0
        If chkX.Checked Then xResize = CalculateXResize()
        If chkY.Checked Then yResize = CalculateYResize()
        Return New Size(m_OldSize.Width + xResize, m_OldSize.Height + yResize)
    End Function

    Private Sub tkbAmount_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tkbAmount.ValueChanged
        lblReadAmount.Text = tkbAmount.Value.ToString()
    End Sub

    Private Sub rdbPercentage_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdbPercentage.CheckedChanged
        If rdbPercentage.Checked Then
            tkbAmount.SetRange(1, 99)
            tkbAmount.Value = 1
        End If
    End Sub

    Private Sub rdbPixels_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdbPixels.CheckedChanged
        If rdbPixels.Checked Then
            tkbAmount.SetRange(1, Math.Min(m_OldSize.Width, m_OldSize.Height))
            tkbAmount.Value = 1
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        DialogResult = Windows.Forms.DialogResult.OK
        Hide()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        DialogResult = Windows.Forms.DialogResult.Cancel
        Hide()
    End Sub

    Public ReadOnly Property NewSize() As Size
        Get
            Return Me.CalculateFinalSize()
        End Get
    End Property

    Private Sub ResizePanelDialogForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class