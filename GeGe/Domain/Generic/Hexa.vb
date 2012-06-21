Public Class Hexa

    Public Shared Function EsHex(ByVal v As String) As Boolean
        Return Mid(v, 1, 2) = "0x"
    End Function

    Public Shared Function IntToHex(ByVal v As String) As String
        Return "0x" + Hex(CInt(v))
    End Function

    Public Shared Function HexToInt(ByVal v As String) As String
        If v.Length < 3 Then Return ""
        Return CInt("&H" + Mid(v, 3)).ToString
    End Function

    Public Shared Function HexToBin(ByVal v As String) As String
        Dim i As Integer
        Dim res As String

        res = ""

        For i = 1 To v.Length
            Select Case Mid(v, i, 1)
                Case "0"
                    res += "0000"
                Case "1"
                    res += "0001"
                Case "2"
                    res += "0010"
                Case "3"
                    res += "0011"
                Case "4"
                    res += "0100"
                Case "5"
                    res += "0101"
                Case "6"
                    res += "0110"
                Case "7"
                    res += "0111"
                Case "8"
                    res += "1000"
                Case "9"
                    res += "1001"
                Case "a"
                    res += "1010"
                Case "b"
                    res += "1011"
                Case "c"
                    res += "1100"
                Case "d"
                    res += "1101"
                Case "e"
                    res += "1110"
                Case "f"
                    res += "1111"
            End Select
        Next i

        Return res
    End Function

End Class
