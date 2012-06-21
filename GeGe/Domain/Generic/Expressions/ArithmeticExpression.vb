Public Class ArithmeticExpression

    Public m_Error As String
    Private m_ResGlobalVariables As VariablesList
    Private m_Expression As String

    Public Sub New(ByRef ResGlobalVariables As VariablesList)

        m_ResGlobalVariables = ResGlobalVariables

    End Sub


    Public Function Parse(ByVal Exp As String) As Boolean

        Dim Expected, NewChar As String
        Dim i, NumOpenPar As Integer

        Parse = True
        Expected = "co("
        For i = 1 To Exp.Length
            NewChar = Mid(Exp, i, 1)
            If NewCharOk(Expected, CChar(NewChar)) Then
                Select Case NewChar
                    Case "+", "*", "-", "/"
                        Expected = "c(n"
                    Case "("
                        Expected = "c(n"
                        NumOpenPar = NumOpenPar + 1
                    Case ")"
                        Expected = "o)"
                        NumOpenPar = NumOpenPar - 1
                    Case Else
                        Expected = "co)n"

                End Select
            Else
                m_Error = "Unexpected char at " & i & ": '" & NewChar & "'!"
                Parse = False
                Exit Function
            End If
        Next i

        If NumOpenPar > 0 Then
            m_Error = "Expected ')'"
            Parse = False
            Exit Function
        Else
            If NumOpenPar < 0 Then
                m_Error = "Expected '('"
                Parse = False
                Exit Function
            End If
        End If

    End Function


    Private Function NewCharOk(ByVal Expected As String, ByVal NewChar As Char) As Boolean
        Dim i As Integer
        'Controlla che il test sia vero almeno per uno caratteri attesi
        For i = 0 To Expected.Length - 1
            If CharTest(Expected.Chars(i), NewChar) Then
                NewCharOk = True
                Exit For
            End If
        Next i
    End Function

    Private Function CharTest(ByVal ExpectedChar As Char, ByVal NewChar As Char) As Boolean

        Select Case ExpectedChar
            Case CChar("c")
                If Not (NewChar Like "[+|""'<>().,:; {}~/\]^%@" Or NewChar = "-" Or NewChar = "*" Or NewChar = "#" Or NewChar = "?" Or NewChar = "!" Or NewChar = "[" Or NewChar = "]" Or NewChar = "*" Or NewChar = "/" Or NewChar = "+") Then
                    CharTest = True
                End If

            Case CChar("o")
                If NewChar = "+" Or NewChar = "*" Or NewChar = "-" Or NewChar = "/" Then
                    CharTest = True
                End If
            Case CChar("n")
                If IsNumeric(NewChar) Then
                    CharTest = True
                End If
            Case CChar("(")
                If NewChar = "(" Then
                    CharTest = True
                End If
            Case CChar(")")
                If NewChar = ")" Then
                    CharTest = True
                End If
        End Select
    End Function




    Public Function calculateExp(ByVal exp As String) As Double
        'MsgBox("1 " & exp)
        Dim numPar As Integer = 0
        Dim parPos As Integer
        Dim c As String
        Dim i As Integer = 1
        Dim ParFound As Boolean = False

        While (True)
            c = Mid(exp, i, 1)
            While (c <> "")
                Select Case c
                    Case "("
                        ParFound = True
                        If numPar = 0 Then
                            parPos = i
                        End If
                        numPar = numPar + 1
                    Case ")"
                        numPar = numPar - 1
                        If numPar = 0 Then
                            Dim tmp As String = Mid(exp, parPos + 1, i - parPos - 1)
                            ' MsgBox("mid: " & tmp)

                            exp = exp.Remove(parPos - 1, i - parPos + 1)
                            exp = exp.Insert(parPos - 1, calculateExp(tmp).ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat))
                            ' MsgBox(exp)

                        End If

                End Select
                i = i + 1
                c = Mid(exp, i, 1)
            End While
            If (Not ParFound) Then
                Exit While
            End If
            ParFound = False
            i = 1
            numPar = 0
        End While
        calculateExp = Globals.ParseInvariantDouble(calculate(exp))
    End Function


    ' Perchè As String? Boh!
    Private Function calculate(ByVal exp As String) As String
        ' MsgBox(exp)

        Dim c As String = "", tmp As String = "", tmp2 As String = ""
        Dim i As Integer = 1
        Dim opCount As Integer = 0

        While i < exp.Length

            c = Mid(exp, i, 1)
            If (c = "*" Or c = "/" Or c = "+" Or c = "-") Then
                opCount = opCount + 1
                tmp = c
            End If
            i = i + 1
        End While

        If (opCount = 0) Then
            If (IsNumeric(exp)) Then
                calculate = exp
                Exit Function
            Else
                Dim var As Variable = Nothing
                'var = m_pouInterface.FindVariableByName(exp)
                If IsNothing(var) Then
                    var = m_ResGlobalVariables.FindVariableByName(exp)
                End If
                If IsNothing(var) Then
                    Throw New Exception("Variable " & exp & " not found")
                    Exit Function
                End If
                exp = var.ValueToUniversalString()
                calculate = exp
                Exit Function
            End If

        End If



        If opCount = 1 Then
            Dim firstVar As Variable = Nothing
            Dim secondVar As Variable = Nothing
            Dim first As String = exp.Substring(0, exp.IndexOf(tmp))
            If Not IsNumeric(first) Then
                'firstVar = m_pouInterface.FindVariableByName(first)
                If IsNothing(firstVar) Then
                    firstVar = m_ResGlobalVariables.FindVariableByName(first)
                End If
                If IsNothing(firstVar) Then
                    Throw New Exception("Variable " & first & " not found")
                    Exit Function
                End If
                first = firstVar.ValueToUniversalString()
            End If
            Dim second As String = exp.Substring(exp.IndexOf(tmp) + 1)
            If Not IsNumeric(second) Then
                'secondVar = m_pouInterface.FindVariableByName(second)
                If IsNothing(secondVar) Then
                    secondVar = m_ResGlobalVariables.FindVariableByName(second)
                End If
                If IsNothing(secondVar) Then
                    Throw New Exception("Variable " & second & " not found")
                    Exit Function
                End If
                second = secondVar.ValueToUniversalString()
            End If
            ' MsgBox(first & " " & second)
            Dim firstOp As Double = Globals.ParseInvariantDouble(first)
            Dim secondOp As Double = Globals.ParseInvariantDouble(second)
            Dim res As Double = firstOp
            Select Case tmp
                Case "*"
                    res *= secondOp
                Case "/"
                    res /= secondOp
                Case "+"
                    res += secondOp
                Case "-"
                    res -= secondOp
            End Select
            calculate = res.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)
            Exit Function
        End If

        i = 1
        c = Mid(exp, i, 1)

        If (exp.IndexOf("+") > 0 Or exp.IndexOf("-") > 0) Then
            While (c <> "")
                Select Case c
                    Case "+", "-"
                        tmp = exp.Substring(0, i - 1)
                        tmp2 = exp.Substring(i)

                        exp = exp.Remove(i, exp.Length - i)
                        exp = exp.Insert(i, calculate(tmp2))
                        exp = exp.Remove(0, i - 1)
                        exp = exp.Insert(0, calculate(tmp))
                End Select

                i = i + 1
                c = Mid(exp, i, 1)
            End While

        Else
            While (c <> "")
                Select Case c
                    Case "*", "/"
                        tmp = exp.Substring(0, i - 1)
                        ' tmp2 = exp.Substring(i)

                        'exp = exp.Remove(i, exp.Length - i)
                        'exp = exp.Insert(i, calculate(tmp2))
                        exp = exp.Remove(0, i - 1)
                        exp = exp.Insert(0, calculate(tmp))
                End Select

                i = i + 1
                c = Mid(exp, i, 1)
            End While

        End If



        calculate = calculate(exp)

    End Function

End Class
