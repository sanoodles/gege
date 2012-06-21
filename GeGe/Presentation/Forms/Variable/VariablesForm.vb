Public Class VariablesForm
    Inherits System.Windows.Forms.Form

    Private VarCtrl As VariablesGridControl

    Public Sub New(ByRef refProject As Project, ByVal Scope As String, _
            Optional ByRef RC As RunControl = Nothing)

        MyBase.New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        VarCtrl = New VariablesGridControl(Grid1, refProject, Scope, RC)

        If RC Is Nothing Then
            SetModeEdit()
        Else
            SetModeRun(RC)
        End If
    End Sub

    Public Sub SetModeEdit()
        Button5.Visible = False
        VarCtrl.SetModeEdit()
    End Sub

    Public Sub SetModeRun(ByRef RC As RunControl)
        Button5.Visible = True
        VarCtrl.SetModeRun(RC)
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        VarCtrl.CreateNewVariable(Me)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        VarCtrl.ModifyVariable()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        VarCtrl.DeleteVariable()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        'Me.Dispose()'VarCtrl queda escuchando eventos
        Me.Dispose()
        Me.Finalize()
    End Sub

    Private Sub Button5_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button5.Click
        VarCtrl.SetVarValue()
    End Sub

    Private Sub ListView1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs)
        VarCtrl.ModifyVariable()
    End Sub



    Private Sub DataGridView1_CellDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles Grid1.CellDoubleClick
        Dim CurCol As Integer
        CurCol = Grid1.CurrentCell.ColumnIndex

        If CurCol = VariablesGridControl.COL_CURRENT_VALUE Then
            VarCtrl.SetVarValue()
        Else
            If VarCtrl.GetMode = ProjectForm.ProjectMode.Edit Then VarCtrl.ModifyVariable()
        End If
    End Sub


    Private Sub Grid1_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles Grid1.CellContentClick

    End Sub
End Class
