Public Class ExcBox

    Public Shared Sub Show(ByRef Ex As Exception)

        Dim msg As String = ""
        Dim cex As Exception = Ex 'current exception
        While Not cex Is Nothing
            msg += vbCrLf + vbCrLf + cex.GetType.Name + ": " + cex.Message
            msg += vbCrLf + cex.StackTrace
            cex = cex.InnerException
        End While

        MsgBox(msg)

    End Sub

End Class
