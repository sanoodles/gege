Public Class Drawer

    Private Const SNAP_COEFF As Integer = 16

    Public Shared Function Snap(ByVal val As Integer) As Integer
        Return CInt(val / SNAP_COEFF) * SNAP_COEFF
    End Function

End Class
