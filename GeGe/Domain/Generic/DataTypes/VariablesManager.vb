Imports System.Xml.Serialization

Public Class VariablesManager

    'Factory method pattern
    Public Shared Function CreateVariable(ByVal Scope As String, _
            ByVal Name As String, ByVal Documentation As String, _
            ByVal Address As String, ByVal InitialValue As String, _
            ByVal Type As String, ByVal Application As String, ByVal ViewAsHex As Boolean) As Variable

        Dim r As Variable = Nothing
        'Crea e aggiunge la variabile
        'Restituisce un riferimento nullo se c'è un errore
        If CheckValue(InitialValue, Type) Then
            Dim NewVariable As Variable = Nothing
            Select Case Type
                Case "BOOL"
                    NewVariable = New BooleanVariable(Scope, Name, Documentation, Address, CBool(InitialValue).ToString, Application)
                Case "INT"
                    NewVariable = New IntegerVariable(Scope, Name, Documentation, Address, CInt(InitialValue).ToString, ViewAsHex, Application)
                Case "REAL"
                    NewVariable = New RealVariable(Scope, Name, Documentation, Address, _
                        Double.Parse(InitialValue, _
                                System.Globalization.CultureInfo.InvariantCulture.NumberFormat).ToString, Application)
            End Select
            r = NewVariable
        End If

        Return r
    End Function

    Public Shared Function CheckValue(ByVal Value As String, ByVal Type As String) As Boolean
        Select Case Type
            Case "BOOL"
                Dim NewVariable As New BooleanVariable
                CheckValue = NewVariable.CheckValue(Value)
            Case "INT"
                Dim NewVariable As New IntegerVariable
                CheckValue = NewVariable.CheckValue(Value)
            Case "REAL"
                Dim NewVariable As New RealVariable
                CheckValue = NewVariable.CheckValue(Value)
        End Select
    End Function

    Public Shared Function InferDataType(ByVal Expr As String) As String
        If CheckValue(Expr, "BOOL") Then Return "BOOL"
        If CheckValue(Expr, "REAL") Then Return "REAL"
        If CheckValue(Expr, "INT") Then Return "INT"
        Return ""
    End Function

    Public Shared Function DefaultValue(ByVal Type As String) As String
        Select Case Type
            Case "BOOL"
                Return "False"
            Case "REAL"
                Return "0.0"
            Case "INT"
                Return "0"
        End Select
        Return Nothing
    End Function

End Class
