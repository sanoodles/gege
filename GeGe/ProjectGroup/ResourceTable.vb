Imports System.Threading
Public Class ResourceTable
    Inherits ArrayList
    Private Structure ResourceRecord
        Dim m_Var As Variable
        Dim m_Thread As Thread
    End Structure
    Public Event VarLocked(ByRef T As Thread)
    Public Sub New()
        MyBase.New()
    End Sub
    Public Sub AddResource(ByRef Var As Variable, ByVal RefThread As Thread)
        Dim NewRecord As New ResourceRecord
        NewRecord.m_Var = Var
        NewRecord.m_Thread = RefThread
        Me.Add(NewRecord)
    End Sub
    Public Sub RemoveThread(ByRef Var As Variable, ByVal RefThread As Thread)
        Dim NewRecord As New ResourceRecord
        NewRecord.m_Var = Var
        NewRecord.m_Thread = RefThread
        Me.Remove(NewRecord)
    End Sub
    Public Function FindThread(ByVal Var As Variable) As Thread
        For Each R As ResourceRecord In Me
            If R.m_Var.Name = Var.Name Then
                Return R.m_Thread
            End If
        Next R
        Return Nothing
    End Function
    Public Sub RaiseVarLock(ByRef V As Variable)
        RaiseEvent VarLocked(FindThread(V))
    End Sub

End Class
