Public Class BodyEventRaiser
    Public Event Disposing()

    Public Sub RaiseDisposing()
        RaiseEvent Disposing()
    End Sub
End Class
