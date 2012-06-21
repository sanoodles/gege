'About this class:
'- Coordinates the classes involved in code generation.
'- It could be viewed as a use case controller for the "GeCé" (code generation) use case.
'- But being similar to the class RunControl, it is placed here; as a domain controller.
Public Class GeCeControl

    Private m_gc As GCwithRC

    Public Sub New(ByRef Pr As Project, ByVal path As String)
        Dim gcm As New GCMapping(Pr)
        m_gc = New GCwithRC(Pr, gcm, Nothing, False, path)
    End Sub

    Public Function Go()
        Return m_gc.Go()
    End Function

End Class
