Imports System.Xml
Imports System.Xml.Serialization

' Non è codice di cui un purista OOP andrebbe fiero, ma funziona abbastanza bene...
'Nodi per costruire l'albero dell'espressione logica
Public Class BoolExpressionNode
    Public Enum TypeOfEdge
        None = 0
        Rising = 1
        Falling = 2
    End Enum

    Public Neg As Boolean
    Public Edge As TypeOfEdge
End Class

Public Class BoolSuperExpressionNode
    Inherits BoolExpressionNode
    Public NextNodes As ArrayList
End Class

Public Class PlusNode
    Inherits BoolSuperExpressionNode
End Class

Public Class MultNode
    Inherits BoolSuperExpressionNode
End Class

Public Class CompareNode
    Inherits BoolSuperExpressionNode
    Public Op As String
End Class

Public Class StepMakerConditionNode
    Inherits BoolExpressionNode
    'Esempio sintassi: [101.T>10s]
    Public StepMaker As BaseGraphicalStep  'Puntatore ala fase
    Public Type As Boolean                 'false (è di tipo S0.X) - true (è di tipo S0.T>2s)
    Public Op As Char                      'Memorizza l'operatore < o >
    Public Time As TimeSpan                'Tempo di attesa
    Public TimeUnit As String              'Memoriza l'unità di tempo scelta
End Class

Public Class VariableNode
    Inherits BoolExpressionNode
    Public WithEvents Var As Variable
    Public Event VarNameChanged()
End Class

Public Class ArithmeticNode
    Inherits BoolExpressionNode
    Public Var As VariableNode
    Public Op As String
    Public Expression As String
End Class

'a boolean expression
' needs the context of a grafcet to determine whether it is valid or not
Public Class BooleanExpression

    '"m_RootNode = Nothing" and Not m_IsFalse means "true".
    '"m_RootNode = Nothing" and m_IsFalse means "false".
    Protected m_IsFalse As Boolean

    Protected m_RootNode As PlusNode
    'Protected m_VariablesUniverse As VariablesList
    Protected m_Grafcet As Grafcet
    Protected m_Error As String
    Protected m_LastValue As Boolean  'Memorizza l'ultimo valore valutato
    Protected m_UsedVariablesList As VariablesList    'Memorizza i le variabili utilizzate
    Protected m_TimerNodes As List(Of StepMakerConditionNode)

    ReadOnly Property TimerNodes() As List(Of StepMakerConditionNode)
        Get
            Return m_TimerNodes
        End Get
    End Property

    Public ReadOnly Property RootNode() As BoolSuperExpressionNode
        Get
            Return m_RootNode
        End Get
    End Property

    Public ReadOnly Property IsFalse() As Boolean
        Get
            Return m_IsFalse
        End Get
    End Property

    Sub New(ByRef refGrafcet As Grafcet)
        m_Grafcet = refGrafcet
        m_UsedVariablesList = New VariablesList(Nothing)
        m_TimerNodes = New List(Of StepMakerConditionNode)
    End Sub

    Sub New(ByRef refGrafcet As Grafcet, ByVal expr As String)
        Me.New(refGrafcet)
        If Not Me.SetExpression(expr) Then _
            Throw New ArgumentException("Invalid expression")
    End Sub

    'Public Function CreateInstance(ByRef RefBackEnd As IIEC61131LanguageImplementation) As BooleanExpression
    '    CreateInstance = New BooleanExpression(RefBackEnd)
    '    CreateInstance.SetExpression(GetExpressionString)
    'End Function

    Public Sub xmlExport(ByRef RefXMLProjectWriter As XmlTextWriter) 'Implements IXMLExportable.xmlExport
        'ST
        RefXMLProjectWriter.WriteElementString("ST", GetExpressionString) 'ST
    End Sub

    Public Function TryParse(ByVal expr As String) As Boolean 'Implements IExpression(Of Boolean).TryParse
        Return Me.SetExpression(expr)
    End Function

    Public Function SetExpression(ByVal Exp As String) As Boolean
        'Restituisce true se riesce il parsing e la creazione dei nodi con il collegamento con le variabili....
        '....altrimenti memorizza l'errore in m_Error
        m_UsedVariablesList.Clear()
        m_Error = ""
        Select Case Exp
            Case "true", "TRUE"
            Case ""
            Case "false", "FALSE"
                m_IsFalse = True
            Case Else
                If Exp <> "" Then
                    Parse(Exp)
                    If m_Error = "" Then 'Se non ci sono stati errori di parsing costruisce l'albero dell'espressione
                        Try
                            m_RootNode = CreatePlusNode(Exp, False)
                        Catch ex As Exception
                            m_RootNode = Nothing
                        End Try
                    End If
                End If
        End Select

        Return m_Error = ""
    End Function

    Public Function ReadError() As String
        ReadError = m_Error
    End Function

    Public Function GetExpressionString() As String 'Implements IExpression(Of Boolean).GetExpressionString
        GetExpressionString = Nothing
        Try 'try poichè le variabili potrebbero essere state rimosse
            If IsNothing(m_RootNode) Then   'Se il nodo non esiste restituisce "true"
                If m_IsFalse Then
                    GetExpressionString = "false"
                Else
                    GetExpressionString = "true"
                End If
            Else
                If IsNothing(m_RootNode.NextNodes) Then   'Se il nodo no ha figli restituisce una stringa vuota
                    GetExpressionString = ""
                Else
                    'Ricostruisce la stringa leggendo gli indirizzi in memoria delle....
                    '....variabili dall'interno dell'albero mediante la funzione chiamata 
                    Dim Result As String = MakeString(m_RootNode)
                    GetExpressionString = Mid(Result, 2, Result.Length - 2) 'Esclude le parentesi esterne
                End If
            End If
        Catch
        End Try
    End Function

    Private Function MakeString(ByRef EvNode As Object) As String
        MakeString = ""
        'Ricostruisce ricorsivamente la strnga dai nodi
        Select Case EvNode.GetType.Name
            Case "PlusNode" 'E' un nodo operatore
                Dim i As Integer
                For i = 0 To EvNode.NextNodes.Count - 1
                    MakeString = MakeString & MakeString(EvNode.NextNodes(i))
                    If Not i = EvNode.NextNodes.Count - 1 Then
                        'Se non è l'ultimo nodo aggiunge un +
                        MakeString = MakeString & "+"
                    End If
                Next i
                'Inserisce le parentesi
                MakeString = "(" & MakeString & ")"
                'Nega il risultato se richiesto
                If EvNode.Neg Then
                    MakeString = "!" & MakeString
                End If
            Case "MultNode"
                Dim i As Integer
                For i = 0 To EvNode.NextNodes.Count - 1
                    MakeString = MakeString & MakeString(EvNode.NextNodes(i))
                    If Not i = EvNode.NextNodes.Count - 1 Then
                        'Se non è l'ultimo nodo aggiunge un +
                        MakeString = MakeString & "*"
                    End If
                    'Nega il risultato se richiesto
                    If EvNode.Neg Then
                        MakeString = "!" & MakeString
                    End If
                Next i
            Case "CompareNode"
                Dim i As Integer
                For i = 0 To EvNode.NextNodes.Count - 1
                    MakeString = MakeString & MakeString(EvNode.NextNodes(i))
                    If Not i = EvNode.NextNodes.Count - 1 Then
                        'Se non è l'ultimo nodo aggiunge l'operatore di confronto
                        MakeString = MakeString & EvNode.op
                    End If
                Next i
            Case "ArithmeticNode"
                MakeString = "{" + EvNode.Var.Var.Name
                MakeString &= EvNode.Op
                MakeString &= EvNode.Expression
                MakeString &= "}"
                If EvNode.Neg Then MakeString = "!" & MakeString
            Case "VariableNode" 'E' un nodo variabile
                'Legge il valore della variabile
                MakeString = EvNode.Var.Name
                'Nega il risultato se richiesto
                If EvNode.Neg Then
                    MakeString = "!" & MakeString
                End If
            Case "StepMakerConditionNode"   'E' un nodo stepmaker
                'Legge il numero della fase
                MakeString = EvNode.StepMaker.Name
                'Controlla Type
                Select Case CBool(EvNode.Type)
                    Case False
                        MakeString = MakeString & ".X"
                    Case True
                        MakeString = MakeString & ".T"
                        'Legge l'operatore
                        MakeString = MakeString & EvNode.Op
                        'Converte il tempo
                        Select Case CStr(EvNode.TimeUnit)
                            Case "ms"
                                MakeString = MakeString & CInt(EvNode.Time.TotalMilliseconds)
                            Case "s"
                                MakeString = MakeString & CInt(EvNode.Time.TotalSeconds)
                            Case "m"
                                MakeString = MakeString & CInt(EvNode.Time.TotalMinutes)
                            Case "h"
                                MakeString = MakeString & CInt(EvNode.Time.TotalHours)
                            Case "d"
                                MakeString = MakeString & CInt(EvNode.Time.TotalDays)
                        End Select
                        'Scrive l'unità di tempo
                        MakeString = MakeString & EvNode.TimeUnit
                End Select
                'Aggiunge le barrette
                MakeString = "[" & MakeString & "]"
                If EvNode.Neg Then
                    MakeString = "!" & MakeString
                End If
        End Select

    End Function

    Public Function GetUsedVariablesList() As System.Collections.Generic.List(Of Variable)
        GetUsedVariablesList = m_UsedVariablesList
    End Function

    Public Function Calculate() As Boolean 'Implements IExpression(Of Boolean).Calculate
        Return Me.Evaluate()
    End Function

    Public Function Evaluate() As Boolean
        If IsNothing(m_RootNode) Then   'Se il nodo non esiste restituisce true
            Evaluate = True
        Else
            If IsNothing(m_RootNode.NextNodes) Then   'Se il nodo no ha figli restituisce true
                Evaluate = True
            Else    'Altrimenti valuta l'espressione
                Try 'L'utente può aver eliminato variabili
                    ''''If m_Error = "" And Not IsNothing(m_RootNode) Then
                    'Se non ci sono errori
                    Evaluate = EvaluateNode(m_RootNode)
                    ''''End If
                Catch ex As System.Exception
                    Evaluate = False
                End Try
            End If
        End If
        m_LastValue = Evaluate
    End Function

    Private Function Parse(ByVal Exp As String) As Boolean
        'Effettua il parsing dell'espressione
        Dim Expected, NewChar As String
        Dim i, NumOpenPar As Integer
        Dim DefiningStepMakerCondition As Boolean
        Dim DefiningArithmeticCondition As Boolean
        Dim PointFind As Boolean   '(punto trovato)

        'Corrispondenze dei caratteri
        'c=carattere
        'o=operatore +,*
        'n=numero
        '!
        '[
        ']
        'u=m,s,h,g (unità di tempo)
        '>=<,> (Operatore di confronto)
        '.=punto
        'T=T,t riferimento al tempo di attivazione di uno step
        'X=X,x riferimento all'attivazione di uno step
        '(
        ')

        Parse = True
        Expected = "c![("
        For i = 1 To Exp.Length
            NewChar = Mid(Exp, i, 1)
            If NewCharOk(Expected, NewChar) Then
                If (Not DefiningStepMakerCondition) AndAlso (Not DefiningArithmeticCondition) Then
                    'Non si sta definendo un StepMakerCondition
                    Select Case NewChar
                        Case "+", "*"
                            Expected = "c![("
                        Case "!"
                            Expected = "c[("
                        Case "("
                            Expected = "c!(["
                            NumOpenPar = NumOpenPar + 1
                        Case ")"
                            Expected = "o)"
                            NumOpenPar = NumOpenPar - 1
                        Case "["
                            DefiningStepMakerCondition = True
                            Expected = "c"
                        Case "{"
                            DefiningArithmeticCondition = True
                            Expected = "c"
                        Case ">", "<", "="
                            Expected = "c>"
                        Case Else
                            Expected = "co)>"
                    End Select
                ElseIf DefiningStepMakerCondition Then
                    'Si sta definendo un StepMakerCondition
                    If NewChar <> "." And Not PointFind Then
                        Expected = "c"
                    Else
                        Select Case NewChar
                            Case "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
                                Expected = "nu"
                            Case "."
                                PointFind = True
                                Expected = "TX"
                            Case "T", "t"
                                Expected = ">"
                            Case "X", "x"
                                Expected = "]"
                            Case "s", "h", "g", "d"
                                Expected = "]"
                            Case "m"
                                Expected = "s]"
                            Case "<", ">"
                                Expected = "n"
                            Case "]"
                                Expected = "o)"
                                DefiningStepMakerCondition = False
                                PointFind = False
                        End Select
                    End If
                Else 'If DefiningArithmeticCondition
                    Select Case NewChar
                        Case "}"
                            Expected = "o)"
                            DefiningArithmeticCondition = False
                        Case Else
                            Expected = "cow>()n"
                    End Select
                End If
            Else
                'Ha trovato un carattere non valido
                'Memorizza l'errore
                m_Error = "Unexpected char at " & i & ": '" & NewChar & "'!"
                Exit Function
            End If
        Next i

        If DefiningStepMakerCondition Then
            m_Error = "Incomplete reference to marker of step"
            Exit Function
            Parse = False
        ElseIf DefiningArithmeticCondition Then
            m_Error = "Incomplete arithmetic condition"
            Exit Function
            Parse = False
        End If
        'Controlla se termina con un carattere, una parentesi chiusa o ]
        Expected = "c)]}"
        NewChar = Mid(Exp, Exp.Length, 1)
        If Not NewCharOk(Expected, NewChar) Then
            'Ha trovato un carattere non valido
            'Memorizza l'errore
            m_Error = "Unexpected char at " & i & ": '" & NewChar & "'!"
            Parse = False
            Exit Function
        End If
        'Controlla il numero di parentesi
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
        'Corrispondenze dei caratteri
        'c=carattere
        'o=operatore +,*
        'n=numero
        '!
        '[
        ']
        'u=m,s,h,g (unità di tempo)
        '>=<,> (Operatore di confronto)
        '.=punto
        'T=T,t riferimento al tempo di attivazione di uno step
        'X=X,x riferimento all'attivazione di uno step
        '(
        ')
        'w=operatore -,/
        Select Case ExpectedChar
            Case "c"
                If Not (NewChar Like "[+|""'<>().,:; {}~/\]^%@" Or NewChar = "-" Or NewChar = "*" _
                Or NewChar = "#" Or NewChar = "?" Or NewChar = "!" Or NewChar = "[" Or NewChar = "]" _
                Or NewChar = "<" Or NewChar = ">" Or NewChar = "=") Then
                    CharTest = True
                End If
            Case "w"
                If NewChar = "-" Or NewChar = "/" Then
                    CharTest = True
                End If
            Case "o"
                If NewChar = "+" Or NewChar = "*" Then
                    CharTest = True
                End If
            Case "["
                If NewChar = "[" Then
                    CharTest = True
                End If
            Case "]"
                If NewChar = "]" Then
                    CharTest = True
                End If
            Case "n"
                If IsNumeric(NewChar) Then
                    CharTest = True
                End If
            Case "!"
                If NewChar = "!" Then
                    CharTest = True
                End If
            Case "u"
                If NewChar = "m" Or NewChar = "s" Or NewChar = "h" Or NewChar = "d" Then
                    CharTest = True
                End If
            Case "s"
                If NewChar = "s" Then
                    CharTest = True
                End If
            Case ">"
                If NewChar = ">" Or NewChar = "<" Or NewChar = "=" Then
                    CharTest = True
                End If
            Case "."
                If NewChar = "." Then
                    CharTest = True
                End If
            Case "T"
                If NewChar = "T" Or NewChar = "t" Then
                    CharTest = True
                End If
            Case "X"
                If NewChar = "X" Or NewChar = "x" Then
                    CharTest = True
                End If
            Case "("
                If NewChar = "(" Then
                    CharTest = True
                End If
            Case ")"
                If NewChar = ")" Then
                    CharTest = True
                End If
        End Select
    End Function

    'aggiunta confronto tra variabili intere
    Private Function CreateCompareNode(ByVal Exp As String) As CompareNode
        Dim RootNode As New CompareNode
        RootNode.NextNodes = New ArrayList
        Dim i, NumOpenPar, FirstPosPar As Integer
        Dim VarName As String = ""
        Dim NewChar As Char
        Dim go As Boolean = True
        Dim ParFind As Boolean
        While (go)
            For i = 1 To Exp.Length
                Select Case Mid(Exp, i, 1)
                    Case "("
                        NumOpenPar = NumOpenPar + 1
                        If Not ParFind Then
                            FirstPosPar = i
                            ParFind = True

                        End If
                    Case ")"
                        NumOpenPar = NumOpenPar - 1
                        If ParFind And NumOpenPar = 0 And Exp.IndexOf("+") >= 0 Then
                            RootNode.NextNodes.Add(CreatePlusNode(Mid(Exp, FirstPosPar + 1, i - FirstPosPar - 1), False))
                            'Rimuove l'espressione dalla stringa
                            Dim j As Integer
                            'Elimina gli eventuali operatori prima della parentesi aperta
                            If Not FirstPosPar = 1 Then
                                j = 1
                            End If

                            Exp = Exp.Remove(FirstPosPar - 1 - j, i - FirstPosPar + 1 + j)
                            i = 0
                            ParFind = False
                            Exit For
                        End If
                End Select
            Next i
            If i > Exp.Length Then   'Non ha trovato più ( )
                go = False
            End If
        End While
        For i = 1 To Exp.Length
            NewChar = Mid(Exp, i, 1)
            Select Case NewChar
                Case ">", "<", "="
                    If VarName <> "" Then
                        'Deve aggiungere un nodo  variabile 
                        Dim NewNode As New VariableNode 'Aggiunge un nodo variabile
                        NewNode.Var = FindVarByName(VarName)
                        If (Not IsNothing(NewNode.Var)) Then
                            If (NewNode.Var.dataType = "BOOL") Then
                                m_Error = "Variable " & VarName & " is of type BOOL"
                            End If
                        End If
                        RootNode.NextNodes.Add(NewNode)
                        RootNode.Op = NewChar
                        If Not IsNothing(NewNode.Var) Then
                            AddToUsedVariablesList(NewNode.Var) 'Aggiunge la variabile alla lista delle variabili utilizzate
                        Else : Return Nothing 'originally this was "Exit Function"
                        End If
                    Else
                        Dim tmpChar As String = Mid(Exp, i - 1, 1)
                        If tmpChar = "<" And NewChar = "=" Then
                            RootNode.Op = "<="
                        End If
                        If tmpChar = ">" And NewChar = "=" Then
                            RootNode.Op = ">="
                        End If
                    End If
                    VarName = ""
                Case "(", ")"

                Case Else
                    VarName = VarName & NewChar
            End Select
        Next i
        'Aggiunge l'ultima variabile rimasta
        If Exp.Length > 0 Then
            'E un nodo variabile
            Dim isFakeVar As Boolean = False
            Dim NewNode As New VariableNode 'Aggiunge un nodo variabile
            NewNode.Var = FindVarByName(VarName)
            If (Not IsNothing(NewNode.Var)) Then
                If (NewNode.Var.dataType = "BOOL") Then
                    m_Error = "Variable " & VarName & " is of type BOOL"
                End If
            Else
                If (IsNumeric(VarName)) Then
                    Dim intVar As New RealVariable()
                    intVar.SetActValue(ParseInvariantDouble(VarName))
                    intVar.Name = VarName
                    NewNode.Var = intVar
                    m_Error = ""
                    isFakeVar = True
                End If
            End If
            RootNode.NextNodes.Add(NewNode)
            If Not IsNothing(NewNode.Var) And Not isFakeVar Then
                AddToUsedVariablesList(NewNode.Var) 'Aggiunge la variabile alla lista delle variabili utilizzate
            End If

        End If
        CreateCompareNode = RootNode
    End Function
    'fine aggiunta

    Private Function FindVarByName(ByVal Name As String) As Variable
        FindVarByName = m_Grafcet.AllVariables.FindVariableByName(Name)
        If IsNothing(FindVarByName) Then
            m_Error = "Invalid name of variable: " & Name
        End If
    End Function

    Private Function CreatePlusNode(ByVal Exp As String, ByVal NodeNeg As Boolean) As PlusNode
        'Crea il nodo + principale
        Dim i, FirstPosPlus, NumOpenPar As Integer
        Dim MultFind, NextNodeNeg, CompareOpFind, squareParFind As Boolean
        Dim blockParFind As Boolean
        Dim RootNode As New PlusNode
        RootNode.Neg = NodeNeg
        RootNode.NextNodes = New ArrayList
        'Cerca i prodotti
        Dim go As Boolean = True
        While (go)
            For i = 1 To Exp.Length
                Select Case Mid(Exp, i, 1)
                    Case "+"
                        If blockParFind Then Exit Select
                        If MultFind And NumOpenPar = 0 Then
                            RootNode.NextNodes.Add(CreateMultNode(Mid(Exp, FirstPosPlus + 1, i - FirstPosPlus - 1)))
                            MultFind = False
                            CompareOpFind = False
                            'Rimuove l'espressione dalla stringa
                            Dim j As Integer
                            'Elimina gli eventuali operatori dopo la parentesi chiusa
                            If Not i = Exp.Length Then
                                j = 1
                            End If
                            Exp = Exp.Remove(FirstPosPlus, i - FirstPosPlus - 1 + j)
                            i = 0
                            Exit For
                        ElseIf CompareOpFind And NumOpenPar = 0 Then
                            RootNode.NextNodes.Add(CreateCompareNode(Mid(Exp, FirstPosPlus + 1, i - FirstPosPlus - 1)))
                            CompareOpFind = False
                            'Rimuove l'espressione dalla stringa
                            Dim j As Integer
                            'Elimina gli eventuali operatori dopo la parentesi chiusa
                            If Not i = Exp.Length Then
                                j = 1
                            End If
                            Exp = Exp.Remove(FirstPosPlus, i - FirstPosPlus - 1 + j)
                            i = 0
                            Exit For

                        Else
                            If NumOpenPar = 0 Then
                                FirstPosPlus = i
                            End If
                        End If
                    Case "*"
                        If blockParFind Then Exit Select
                        MultFind = True
                    Case ">", "<", "="
                        If (Not squareParFind) AndAlso (Not blockParFind) Then
                            CompareOpFind = True
                        End If
                    Case "("
                        If blockParFind Then Exit Select
                        NumOpenPar = NumOpenPar + 1
                    Case ")"
                        If blockParFind Then Exit Select
                        NumOpenPar = NumOpenPar - 1
                    Case "["
                        squareParFind = True
                    Case "]"
                        squareParFind = False
                    Case "{"
                        blockParFind = True
                    Case "}"
                        blockParFind = False

                End Select
            Next i
            If i > Exp.Length Then   'Non ha trovato più *
                go = False
            End If
        End While
        'Controlla l'ultima espressione se è un *
        If MultFind And NumOpenPar = 0 Then
            RootNode.NextNodes.Add(CreateMultNode(Mid(Exp, FirstPosPlus + 1, i)))
            'Rimuove l'espressione dalla stringa
            If FirstPosPlus = 0 Then
                Exp = ""
            Else
                Exp = Exp.Remove(FirstPosPlus - 1, i - FirstPosPlus)
            End If
            ' End If
            'Controlla l'ultima espressione se è un >
        ElseIf CompareOpFind And NumOpenPar = 0 Then
            RootNode.NextNodes.Add(CreateCompareNode(Mid(Exp, FirstPosPlus + 1, i - FirstPosPlus - 1)))
            'Rimuove l'espressione dalla stringa
            If FirstPosPlus = 0 Then
                Exp = ""
            Else
                Exp = Exp.Remove(FirstPosPlus - 1, i - FirstPosPlus)
            End If
        End If
        'Sono terminati i * e i >
        'Exp ora contiene solo + e ( )
        'Cerca le stringhe tra parentesi
        Dim FirstPosPar As Integer
        Dim ParFind As Boolean
        'Cerca i prodotti
        go = True
        While (go)
            For i = 1 To Exp.Length
                Select Case Mid(Exp, i, 1)
                    Case "("
                        NumOpenPar = NumOpenPar + 1
                        If Not ParFind Then
                            FirstPosPar = i
                            ParFind = True
                            'Controlla se prima c'è una negazione
                            If i > 1 Then
                                If Mid(Exp, i - 1, 1) = "!" Then
                                    NextNodeNeg = True
                                End If
                            End If
                        End If
                    Case ")"
                        NumOpenPar = NumOpenPar - 1
                        If ParFind And NumOpenPar = 0 Then
                            RootNode.NextNodes.Add(CreatePlusNode(Mid(Exp, FirstPosPar + 1, i - FirstPosPar - 1), NextNodeNeg))
                            'Rimuove l'espressione dalla stringa
                            Dim j As Integer
                            'Elimina gli eventuali operatori prima della parentesi aperta
                            If Not FirstPosPar = 1 Then
                                j = 1
                            End If
                            'Elimina la negazione se presente
                            If Not FirstPosPar <= 3 And NextNodeNeg Then
                                j = 2
                            End If
                            Exp = Exp.Remove(FirstPosPar - 1 - j, i - FirstPosPar + 1 + j)
                            i = 0
                            ParFind = False
                            NextNodeNeg = False
                            Exit For
                        End If
                End Select
            Next i
            If i > Exp.Length Then   'Non ha trovato più ( )
                go = False
            End If
        End While
        'Sono terminati le stringhe tra parentesi
        'Exp ora contiene solo + 
        Dim VarName As String = ""
        Dim NewChar As Char
        'Cerca le singole variabili tra i +
        For i = 1 To Exp.Length
            NewChar = Mid(Exp, i, 1)
            Select Case NewChar
                Case "+"
                    If VarName <> "" Then
                        'Deve aggiungere un nodo o variabile o stepmaker
                        If Not VarName = "" Then
                            If VarName.StartsWith("{") AndAlso (Not VarName.EndsWith("}")) Then
                                ' deve essere uguale a Case Else
                                VarName &= NewChar
                                Exit Select
                            End If

                            ' é un nodo aritmetico
                            If VarName.EndsWith("}") Then

                                Dim NewNode As ArithmeticNode = CreateArithmeticNode(VarName)
                                If NextNodeNeg Then
                                    NewNode.Neg = True
                                    NextNodeNeg = False
                                End If
                                RootNode.NextNodes.Add(NewNode)
                                AddToUsedVariablesList(NewNode.Var.Var)

                                'E un nodo variabile
                            ElseIf Mid(VarName, 1, 1) <> "[" Then

                                Dim NewNode As New VariableNode 'Aggiunge un nodo variabile
                                NewNode.Var = FindVarByName(VarName)
                                If NextNodeNeg Then 'Nega la variabile se richiesto
                                    NewNode.Neg = True
                                    NextNodeNeg = False
                                End If
                                RootNode.NextNodes.Add(NewNode)
                                If Not IsNothing(NewNode.Var) Then
                                    AddToUsedVariablesList(NewNode.Var) 'Aggiunge la variabile alla lista delle variabili utilizzate
                                End If

                                'E' un nodo stepmaker
                            Else

                                'Aggiunge un nodo stepmaker                        
                                Dim NewNode As StepMakerConditionNode = CreateStepMakerConditionNode(VarName)
                                If NextNodeNeg Then 'Nega il nodo se richiesto
                                    NewNode.Neg = True
                                    NextNodeNeg = False
                                End If
                                RootNode.NextNodes.Add(NewNode)
                                If IsTimerNode(NewNode) Then m_TimerNodes.Add(NewNode)

                            End If

                        End If
                        VarName = ""
                    End If
                Case "!"
                    NextNodeNeg = True
                Case Else
                    VarName = VarName & NewChar
            End Select
        Next i
        'Aggiunge l'ultima variabile rimasto 
        If Exp.Length > 0 Then
            If VarName.EndsWith("}") Then
                ' é un nodo aritmetico
                Dim NewNode As ArithmeticNode = CreateArithmeticNode(VarName)
                If NextNodeNeg Then
                    NewNode.Neg = True
                    NextNodeNeg = False
                End If
                RootNode.NextNodes.Add(NewNode)
                AddToUsedVariablesList(NewNode.Var.Var)
            ElseIf Mid(VarName, 1, 1) <> "[" Then
                'E un nodo variabile
                Dim NewNode As New VariableNode 'Aggiunge un nodo variabile
                NewNode.Var = FindVarByName(VarName)
                If NextNodeNeg Then 'Nega la variabile se richiesto
                    NewNode.Neg = True
                    NextNodeNeg = False
                End If
                RootNode.NextNodes.Add(NewNode)
                If Not IsNothing(NewNode.Var) Then
                    AddToUsedVariablesList(NewNode.Var) 'Aggiunge la variabile alla lista delle variabili utilizzate
                End If
            Else
                'E' un nodo stepmaker
                'Aggiunge un nodo stepmaker                        
                Dim NewNode As StepMakerConditionNode = CreateStepMakerConditionNode(VarName)
                If NextNodeNeg Then 'Nega il nodo se richiesto
                    NewNode.Neg = True
                    NextNodeNeg = False
                End If
                RootNode.NextNodes.Add(NewNode)
                If IsTimerNode(NewNode) Then m_TimerNodes.Add(NewNode)

            End If
        End If
        CreatePlusNode = RootNode
    End Function

    Public Function IsTimerNode(ByRef n As StepMakerConditionNode) As Boolean

        'false (è di tipo S0.X) - true (è di tipo S0.T>2s)
        Return n.Type

    End Function

    Private Function CreateArithmeticNode(ByVal Exp As String) As ArithmeticNode
        Dim varName As String = ""
        Dim compareOp As String = ""
        Dim expr As String = ""
        Dim i As Integer = 1
        While (i < Exp.Length) AndAlso (Not CharTest(">", Exp(i)))
            varName &= Exp(i)
            i += 1
        End While
        While (i < Exp.Length) AndAlso CharTest(">", Exp(i))
            compareOp &= Exp(i)
            i += 1
        End While
        While (i < Exp.Length) AndAlso Exp(i) <> "}"
            expr &= Exp(i)
            i += 1
        End While
        If varName = "" OrElse compareOp = "" OrElse expr = "" Then
            m_Error = "Invalid arithmetic expression"
            Return Nothing
        End If
        Dim ret As New ArithmeticNode
        ret.Var = New VariableNode
        ret.Var.Var = FindVarByName(varName)
        ret.Op = compareOp
        ret.Expression = expr
        Return ret
    End Function

    Private Function CreateMultNode(ByVal Exp As String) As MultNode
        'Crea un nodo *
        Dim i, NumOpenPar, FirstPosPar As Integer
        Dim NextNodeNeg, ParFind As Boolean
        Dim RootNode As New MultNode
        RootNode.NextNodes = New ArrayList
        Dim go As Boolean = True
        RootNode.NextNodes = New ArrayList
        'Exp contiene solo * e ( )
        'Cerca le stringhe tra parentesi
        go = True
        While (go)
            For i = 1 To Exp.Length
                Select Case Mid(Exp, i, 1)
                    Case "("
                        NumOpenPar = NumOpenPar + 1
                        If Not ParFind Then
                            FirstPosPar = i
                            ParFind = True
                            'Controlla se prima c'è una negazione
                            If i > 1 Then
                                If Mid(Exp, i - 1, 1) = "!" Then
                                    NextNodeNeg = True
                                End If
                            End If
                        End If
                    Case ")"
                        NumOpenPar = NumOpenPar - 1
                        If ParFind And NumOpenPar = 0 Then
                            RootNode.NextNodes.Add(CreatePlusNode(Mid(Exp, FirstPosPar + 1, i - FirstPosPar - 1), NextNodeNeg))
                            'Rimuove l'espressione dalla stringa
                            Dim j As Integer
                            'Elimina gli eventuali operatori prima della parentesi aperta
                            If Not FirstPosPar = 1 Then
                                j = 1
                            End If
                            'Elimina la negazione se presente
                            If Not FirstPosPar <= 3 And NextNodeNeg Then
                                j = 2
                            End If
                            Exp = Exp.Remove(FirstPosPar - 1 - j, i - FirstPosPar + 1 + j)
                            i = 0
                            ParFind = False
                            NextNodeNeg = False
                            Exit For
                        End If
                End Select
            Next i
            If i > Exp.Length Then   'Non ha trovato più ( )
                go = False
            End If
        End While
        'Sono terminati le stringhe tra parentesi
        'Exp ora contiene solo * e >
        Dim VarName As String = ""
        Dim NewChar As Char
        'Cerca le singole variabili ,confronti tra interi e stepmaker tra i *
        For i = 1 To Exp.Length
            NewChar = Mid(Exp, i, 1)
            Select Case NewChar
                Case "*"
                    If VarName <> "" Then
                        'Deve aggiungere un nodo o variabile o stepmaker o una comparazione tra interi
                        If (VarName.IndexOf(">") >= 0 Or VarName.IndexOf("<") >= 0 Or VarName.IndexOf("=") >= 0) Then
                            If (Mid(VarName, 1, 1) <> "[" AndAlso Mid(VarName, 1, 1) <> "{") Then
                                RootNode.NextNodes.Add(CreateCompareNode(VarName))
                                VarName = ""
                                Exit Select
                            End If
                        End If
                        If VarName.StartsWith("{") AndAlso Not VarName.EndsWith("}") Then
                            ' se siamo nel mezzo di una espressione
                            VarName = VarName & NewChar
                            Exit Select
                        End If
                        If VarName.EndsWith("}") Then
                            ' é un nodo aritmetico
                            Dim NewNode As ArithmeticNode = CreateArithmeticNode(VarName)
                            If NextNodeNeg Then
                                NewNode.Neg = True
                                NextNodeNeg = False
                            End If
                            RootNode.NextNodes.Add(NewNode)
                            AddToUsedVariablesList(NewNode.Var.Var)
                        ElseIf Mid(VarName, 1, 1) <> "[" Then
                            'E un nodo variabile
                            Dim NewNode As New VariableNode 'Aggiunge un nodo variabile
                            NewNode.Var = FindVarByName(VarName)
                            If NextNodeNeg Then 'Nega la variabile se richiesto
                                NewNode.Neg = True
                                NextNodeNeg = False
                            End If
                            RootNode.NextNodes.Add(NewNode)
                            If Not IsNothing(NewNode.Var) Then
                                AddToUsedVariablesList(NewNode.Var) 'Aggiunge la variabile alla lista delle variabili utilizzate
                            End If
                        Else
                            'E' un nodo stepmaker
                            'Aggiunge un nodo stepmaker                        
                            Dim NewNode As StepMakerConditionNode = CreateStepMakerConditionNode(VarName)
                            If NextNodeNeg Then 'Nega il nodo se richiesto
                                NewNode.Neg = True
                                NextNodeNeg = False
                            End If
                            RootNode.NextNodes.Add(NewNode)
                            If IsTimerNode(NewNode) Then m_TimerNodes.Add(NewNode)
                        End If
                        VarName = ""
                    End If
                Case "!"
                    NextNodeNeg = True
                Case Else
                    VarName = VarName & NewChar
            End Select
        Next i
        'Aggiunge l'ultima variabile rimasto 
        If Exp.Length > 0 Then
            If ((VarName.IndexOf(">") >= 0 Or VarName.IndexOf("<") >= 0 Or VarName.IndexOf("=") >= 0) And _
                    Mid(VarName, 1, 1) <> "[" And Mid(VarName, 1, 1) <> "{") Then
                RootNode.NextNodes.Add(CreateCompareNode(VarName))
                VarName = ""
            ElseIf VarName.EndsWith("}") Then
                ' é un nodo aritmetico
                Dim NewNode As ArithmeticNode = CreateArithmeticNode(VarName)
                If NextNodeNeg Then
                    NewNode.Neg = True
                    NextNodeNeg = False
                End If
                RootNode.NextNodes.Add(NewNode)
                AddToUsedVariablesList(NewNode.Var.Var)
            ElseIf Mid(VarName, 1, 1) <> "[" Then
                'E un nodo variabile
                Dim NewNode As New VariableNode 'Aggiunge un nodo variabile
                NewNode.Var = FindVarByName(VarName)
                If NextNodeNeg Then 'Nega la variabile se richiesto
                    NewNode.Neg = True
                    NextNodeNeg = False
                End If
                RootNode.NextNodes.Add(NewNode)
                If Not IsNothing(NewNode.Var) Then
                    AddToUsedVariablesList(NewNode.Var) 'Aggiunge la variabile alla lista delle variabili utilizzate
                End If
            Else
                'E' un nodo stepmaker
                'Aggiunge un nodo stepmaker                        
                Dim NewNode As StepMakerConditionNode = CreateStepMakerConditionNode(VarName)
                If NextNodeNeg Then 'Nega il nodo se richiesto
                    NewNode.Neg = True
                    NextNodeNeg = False
                End If
                RootNode.NextNodes.Add(NewNode)
                If IsTimerNode(NewNode) Then m_TimerNodes.Add(NewNode)
            End If
        End If
        CreateMultNode = RootNode
    End Function

    Private Function CreateStepMakerConditionNode(ByVal Exp As String) As StepMakerConditionNode
        'Esempio formato Exp: [101.X] or [101.T>10m]
        'Crea un nodo stepmaker secondario
        Dim i As Integer
        Dim NewChar As Char
        Dim NewNode As New StepMakerConditionNode
        Dim StepName As String = ""
        Dim Time As String = ""
        Dim TimeUnitSelected As String = ""
        Dim PointFind As Boolean
        'Aggiunge il collegamento alla fase
        For i = 2 To Exp.Length - 1 'Esclude le parentesi
            NewChar = Mid(Exp, i, 1)
            If Not PointFind Then
                'Non ha trovato l'peratore di confronto, quindi è ancora un il nome di fase o il punto
                Select Case NewChar
                    Case "."
                        PointFind = True
                    Case Else
                        StepName = StepName & NewChar
                End Select
            Else 'Ha trovato il punto
                Select Case NewChar
                    Case "X", "x"
                        NewNode.Type = False
                        Exit For
                    Case "t", "T"
                        NewNode.Type = True
                    Case "<", ">"
                        'memorizza l'operatore di confronto
                        NewNode.Op = NewChar
                    Case "s", "m", "h", "d"  'In questi casi rientrano anche i millisecondi ('m' e 's')
                        'E' l'unità di tempo
                        TimeUnitSelected = TimeUnitSelected & NewChar
                    Case Else
                        'E' il tempo
                        Time = Time & NewChar
                End Select
            End If
        Next i
        'Collega la fase
        NewNode.StepMaker = FindStepByName(StepName)
        'Converte il tempo
        If NewNode.Type Then
            Select Case TimeUnitSelected
                Case "ms"
                    NewNode.Time = TimeSpan.FromMilliseconds(Time)
                    NewNode.TimeUnit = "ms"
                Case "s"
                    NewNode.Time = TimeSpan.FromSeconds(Time)
                    NewNode.TimeUnit = "s"
                Case "m"
                    NewNode.Time = TimeSpan.FromMinutes(Time)
                    NewNode.TimeUnit = "m"
                Case "h"
                    NewNode.Time = TimeSpan.FromHours(Time)
                    NewNode.TimeUnit = "h"
                Case "d"
                    NewNode.Time = TimeSpan.FromDays(Time)
                    NewNode.TimeUnit = "d"
                Case Else
                    NewNode.Time = TimeSpan.FromSeconds(0)
                    NewNode.TimeUnit = "ms"
            End Select
        End If
        CreateStepMakerConditionNode = NewNode
    End Function

    Private Sub AddToUsedVariablesList(ByVal V As Variable)
        'Verifica se non è già presente
        If (m_UsedVariablesList.IndexOf(V)) = -1 Then
            m_UsedVariablesList.Add(V)
        End If
    End Sub

    Private Function FindStepByName(ByVal Name As String) As BaseGraphicalStep
        FindStepByName = Nothing

        FindStepByName = m_Grafcet.FindStepByName(Name)

        If IsNothing(FindStepByName) Then
            'C'è stato un errore nel cercare le variabili
            m_Error = "Invalid name of step: " & Name
        End If
    End Function

    Private Function EvaluateNode(ByRef EvNode As Object) As Boolean
        'Utilizza una formula ricorsiva
        Select Case EvNode.GetType.Name
            Case "ArithmeticNode"
                Dim value As Double = EvNode.Var.Var.ReadActValue
                Dim expr As Double '= New ArithmeticExpression(m_BackEnd.ResGlobalVariables, m_BackEnd.PouInterface).calculateExp(EvNode.Expression)
                Select Case EvNode.Op
                    Case ">"
                        EvaluateNode = value > expr
                    Case "<"
                        EvaluateNode = value < expr
                    Case ">="
                        EvaluateNode = value >= expr
                    Case "<="
                        EvaluateNode = value <= expr
                    Case "="
                        EvaluateNode = value = expr
                    Case "<>"
                        EvaluateNode = value <> expr
                End Select
            Case "CompareNode"

                If (EvNode.NextNodes.item(0).GetType.Name <> "VariableNode") Then
                    For Each N As Object In EvNode.NextNodes
                        EvaluateNode = False
                        EvaluateNode = EvaluateNode Or EvaluateNode(N)
                    Next N
                    Exit Function
                End If

                Dim firstVar As Variable = EvNode.NextNodes.item(0).Var
                Dim secondVar As Variable = EvNode.NextNodes.item(1).Var
                EvaluateNode = False
                Select Case CStr(EvNode.op)
                    Case ">"
                        If (firstVar.ReadActValue > secondVar.ReadActValue) Then
                            EvaluateNode = True
                        End If
                    Case "<"
                        If (firstVar.ReadActValue < secondVar.ReadActValue) Then
                            EvaluateNode = True
                        End If
                    Case "="
                        If (firstVar.ReadActValue = secondVar.ReadActValue) Then
                            EvaluateNode = True
                        End If
                    Case "<="
                        If (firstVar.ReadActValue <= secondVar.ReadActValue) Then
                            EvaluateNode = True
                        End If
                    Case ">="
                        If (firstVar.ReadActValue >= secondVar.ReadActValue) Then
                            EvaluateNode = True
                        End If
                End Select
            Case "PlusNode" 'E' un nodo operatore
                'Fa la Or tra il valore attuale e tutti i figli di EvNode
                For Each N As Object In EvNode.NextNodes
                    EvaluateNode = EvaluateNode Or EvaluateNode(N)
                Next N
                'Nega il risultato se richiesto
                If EvNode.Neg Then
                    EvaluateNode = Not EvaluateNode
                End If
            Case "MultNode"
                'Fa la And tra il valore attuale e tutti i figli di EvNode
                For Each N As Object In EvNode.NextNodes
                    EvaluateNode = True
                    EvaluateNode = EvaluateNode And EvaluateNode(N)
                    If EvaluateNode = False Then
                        'Se c'e un nodo false esce dal ciclo
                        Exit For
                    End If
                Next N
                'Nega il risultato se richiesto
                If EvNode.Neg Then
                    EvaluateNode = Not EvaluateNode
                End If
            Case "VariableNode" 'E' un nodo variabile
                'Legge il valore della variabile
                'Monitor
                EvaluateNode = EvNode.Var.ReadValue
                'La nega se richiesto
                If EvNode.Neg Then
                    EvaluateNode = Not EvaluateNode
                End If
            Case "StepMakerConditionNode"   'E' un nodo stepmaker
                Select Case CBool(EvNode.Type)
                    Case False  'è di tipo S0.X
                        EvaluateNode = EvNode.StepMaker.ReadActive
                    Case True
                        Select Case CStr(EvNode.Op)
                            Case "<"
                                'Confronta l'istante di attivazione della fase più il tempo Time con quello attuale 
                                If DateTime.Compare(EvNode.StepMaker.ReadTimeActivation.Add(EvNode.Time), Now) > 0 And EvNode.StepMaker.ReadActive Then
                                    EvaluateNode = True
                                End If
                            Case ">"
                                'Confronta l'istante di attivazione della fase più il tempo Time con quello attuale 
                                If DateTime.Compare(EvNode.StepMaker.ReadTimeActivation.Add(EvNode.Time), Now) < 0 And EvNode.StepMaker.ReadActive Then
                                    EvaluateNode = True
                                End If
                        End Select
                End Select
                'La nega se richiesto
                If EvNode.Neg Then
                    EvaluateNode = Not EvaluateNode
                End If
        End Select
    End Function

End Class
