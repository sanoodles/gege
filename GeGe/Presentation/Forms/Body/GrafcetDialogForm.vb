Public Class GrafcetDialogForm
    Inherits System.Windows.Forms.Form
    Public GNumber As String
    Public GName As String
    Public GDocumentation As String

    Private Sub Label2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Dispose()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        GNumber = TextBox2.Text
        GName = TextBox1.Text
    End Sub

    Private Sub SfcDialogForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        AcceptButton = Button1
        TextBox2.Text = GNumber
        TextBox1.Text = GName
        TextBox2.Focus()
        TryEnableOkButton()
    End Sub
    Private Sub TryEnableOkButton()
        If TextBox1.Text = "" Or TextBox1.Text = "" Then
            Button1.Enabled = False
        Else
            Button1.Enabled = True
        End If
    End Sub
    Private Sub TextBox1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox1.KeyPress
        Dim NewChar As Char = e.KeyChar
        If Not InvalidVariableChars.IndexOf(e.KeyChar) = -1 Then
            e.Handled = True
        End If
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        TryEnableOkButton()
    End Sub

    Private Sub TextBox2_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox2.KeyPress
        'Allow only numbers, backspace and supr.
        'http://social.msdn.microsoft.com/Forums/en-US/Vsexpressvb/thread/aab1d64c-a9dc-4dd2-8d2f-83a414e9c909

        If Not Char.IsDigit(e.KeyChar) Then e.Handled = True
        If e.KeyChar = Chr(8) Then e.Handled = False 'allow Backspace
    End Sub


End Class
