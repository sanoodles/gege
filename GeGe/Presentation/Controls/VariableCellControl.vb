Public Class VariableCellControl

    Private m_Grid As DataGridView
    Private m_Row As Integer
    Private m_ValueColumn As Integer
    Private WithEvents m_Var As Variable

    Public Sub New(ByRef grid As DataGridView, ByVal row As Integer, _
            ByVal valueColumn As Integer, ByRef var As Variable)
        m_Grid = grid
        m_Row = row
        m_ValueColumn = valueColumn
        m_Var = var
    End Sub

    Private Sub ValueChanged() Handles m_Var.ValueChanged, m_Var.ActValueChanged
        If Not m_Grid Is Nothing Then
            m_Grid.Rows(m_Row).Cells(m_ValueColumn).Value = m_Var.ValueToUniversalString
        End If
    End Sub

End Class
