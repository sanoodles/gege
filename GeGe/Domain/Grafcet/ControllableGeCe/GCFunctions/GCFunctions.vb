'This is an abstract class inherited by the 
'concrete classes that generate the code.
'It contains auxiliary methods used by two or more concrete classes.
'
'Regarding Object Orientation:
'Since this class contains some methods
'never called by some inheriting classes, a more clean/ellaborated/cohesioned
'hierarchy of abstract classes could be used instead of this "all-in-one"
'abstract class.
'Nevertheless, this class is still small, so it is preferred to create
'even smaller, although more cohesioned, classes.
Public MustInherit Class GCFunctions

    Protected c As GCFunctionsController

    Protected e As String
    Protected t As String

    Protected m_gcmap As GCMapping
    Protected m_TimerPeriod As Integer

    Public Sub New(ByRef Controller As GCFunctionsController)
        c = Controller

        e = GCFunctionsController.e
        t = GCFunctionsController.t

        m_gcmap = c.m_gcmap
        m_TimerPeriod = c.m_TimerPeriod
    End Sub

    Protected Function GetIncludes() As String
        Return c.GetIncludes
    End Function

    'For code generation, macro-steps have to be expanded.
    'Transitions of a body are not only its own transitions, but also the transitions of
    'its macro-step's expansions.
    '@param[in] Bo Body which contains the transitions
    '@param[out] TrList Already initialized list of transitions
    Protected Sub GetTransitionsByBody(ByRef Bo As Body, _
                ByRef TrList As GraphicalTransitionsList)

        For Each Tr As GraphicalTransition In Bo.GraphicalTransitionsList
            TrList.Add(Tr)
        Next

        For Each St As BaseGraphicalStep In Bo.GraphicalStepsList
            If St.GetType.Name = "GraphicalMacroStep" Then
                GetTransitionsByBody(CType(St, GraphicalMacroStep).ReadSubBody, TrList)
            End If
        Next

    End Sub


    '
    'Boolean expressions are used in conditions
    'Conditions are used in transitions and actions
    '

    Protected Function GetCondition(ByVal indent As String, _
            ByVal numG As Integer, _
            ByRef Co As Condition) As String
        Dim r As String 'result

        r = ""

        r += e
        r += indent + "/*" + e
        r += indent + " * Condition: " + Co.GetString + "" + e
        r += indent + " */" + e

        'if time dependent assignation condition
        If Co.IsTimeDependent Then
            r += indent + "if ((" + GetBooleanExpression(numG, Co.GetBoolExpr) + _
                    " && !BitTimerPB.bits.bit" + m_gcmap.timedConditionToPBBit(Co).ToString + ") ||" + _
                    " BitTimerPB.bits.bit" + (m_gcmap.timedConditionToPBBit(Co) + 1).ToString + ") {" + e

            'if boolean expression assignation condition
        Else
            r += indent + "if (" + GetBooleanExpression(numG, Co.GetBoolExpr) + ") {" + e

        End If

        Return r
    End Function

    Protected Function GetBooleanExpression(ByVal numG As Integer, ByRef e As BooleanExpression) As String
        Dim res As String

        res = Nothing

        If IsNothing(e.RootNode) Then
            If e.IsFalse Then
                res = "0"
            Else
                res = "1"
            End If
        Else
            If IsNothing(e.RootNode.NextNodes) Then   'Se il nodo no ha figli restituisce una stringa vuota
                res = ""
            Else
                'Ricostruisce la stringa leggendo gli indirizzi in memoria delle....
                '....variabili dall'interno dell'albero mediante la funzione chiamata 
                Dim Result As String = MakeString(numG, e.RootNode)
                res = Mid(Result, 2, Result.Length - 2) 'Esclude le parentesi esterne
            End If
        End If

        Return res
    End Function

    Private Function MakeString(ByVal numG As Integer, ByRef EvNode As Object) As String
        Dim r As String = "" 'result

        'Ricostruisce ricorsivamente la strnga dai nodi
        Select Case EvNode.GetType.Name

            Case "PlusNode" 'E' un nodo operatore
                Dim i As Integer
                For i = 0 To EvNode.NextNodes.Count - 1
                    r = r & MakeString(numG, EvNode.NextNodes(i))
                    If Not i = EvNode.NextNodes.Count - 1 Then
                        'Se non è l'ultimo nodo aggiunge un +
                        r = r & " | "
                    End If
                Next i
                'Inserisce le parentesi
                r = "(" & r & ")"
                'Nega il risultato se richiesto
                If EvNode.Neg Then
                    r = " !" & r
                End If

            Case "MultNode"
                Dim i As Integer
                For i = 0 To EvNode.NextNodes.Count - 1
                    r = r & MakeString(numG, EvNode.NextNodes(i))
                    If Not i = EvNode.NextNodes.Count - 1 Then
                        'Se non è l'ultimo nodo aggiunge un +
                        r = r & " & "
                    End If
                    'Nega il risultato se richiesto
                    If EvNode.Neg Then
                        r = " !" & r
                    End If
                Next i

            Case "CompareNode"
                Dim i As Integer
                For i = 0 To EvNode.NextNodes.Count - 1
                    r = r & MakeString(numG, EvNode.NextNodes(i))
                    If Not i = EvNode.NextNodes.Count - 1 Then
                        'Se non è l'ultimo nodo aggiunge l'operatore di confronto
                        r = r & " " & GetOperator(EvNode.op) & " "
                    End If
                Next i

            Case "ArithmeticNode"
                r = EvNode.Var.Var.Name
                r &= " " & GetOperator(EvNode.Op) & " "
                r &= EvNode.Expression
                If EvNode.Neg Then r = " !" & r

            Case "VariableNode" 'E' un nodo variabile
                r = GetVariableNode(EvNode.Var)
                'Nega il risultato se richiesto
                If EvNode.Neg Then
                    r = " !" & r
                End If

            Case "StepMakerConditionNode"   'E' un nodo stepmaker
                'Controlla Type
                Select Case CBool(EvNode.Type)
                    Case False 'step variable
                        r = GetStepVariableNode(numG, EvNode.StepMaker)
                    Case True 'step timer
                        'r = GetStepTimerNode(numG, EvNode.StepMaker)
                End Select

                If EvNode.Neg Then
                    r = " !" & r
                End If
        End Select

        Return r
    End Function

    Private Function GetOperator(ByVal op As String) As String
        Select Case op
            Case "="
                Return "=="
            Case Else
                Return op
        End Select
    End Function

    Private Function GetVariableNode(ByRef v As Variable) As String
        Return m_gcmap.VarToGCname(v)
        'Select Case v.dataType
        '
        '            Case "BOOL"
        '        Return "EntradaDig.bits.bit" + m_gcmap.digitalInputToBit( _
        '                CType(v, BooleanVariable)).ToString
        '
        '            Case "INT", "REAL"
        '        Return v.Name
        '
        '            Case Else'
        'Return v.Name
        'End Select
    End Function

    Private Function GetStepVariableNode(ByVal ng As Integer, ByRef s As BaseGraphicalStep) As String
        Return "G[" + m_gcmap.numToGrafcet(ng).Name + "].EtapaActiva.bits.bit" + m_gcmap.G(ng).stepToBit(s).ToString
    End Function

End Class
