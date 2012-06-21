Imports System.Threading

Public Class Enclosure
    Inherits Grafcet

    Public Sub New(ByRef refProject As Project, ByVal Name As String)
        MyBase.New(refProject, Name)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Function IsAEnclosure() As Boolean
        Return True
    End Function

End Class
