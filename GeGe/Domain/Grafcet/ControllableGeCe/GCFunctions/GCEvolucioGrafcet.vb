Public Class GCEvolucioGrafcet
    Inherits GCFunctions

    Public Sub New(ByRef c As GCFunctionsController)
        MyBase.New(c)
    End Sub

    Public Function GetEvolucioGrafcet() As String
        Dim r As String 'result

        r = GetIncludes() + e

        r += "void EvolucioGrafcet(void)" + e + _
            "{" + e + _
            t + "unsigned char nG;" + e + _
            e + _
            t + "for (nG = 0; nG < nGRAFCET; nG++)" + e + _
            t + t + "G[nG].Receptivitat.all = 0x0000;" + e + _
            e

        'For each grafcet
        For Each Gr As KeyValuePair(Of Integer, Grafcet) In m_gcmap.numToGrafcet
            r += t + "if (!G[" + Gr.Value.Name + "].Forcat)" + e + _
                 t + "{" + e + _
                 t + t + "AvaluaReceptivitats(" + Gr.Value.Name + ");" + e + _
                 e

            'optimització: als sincronismes, per a no generar codi igual (equivalent) 
            'per a cada etapa prèvia a la transició, guardem si ja ha estat generat
            'el codi de la transició
            Dim TrList As New GraphicalTransitionsList
            GetTransitionsByBody(Gr.Value, TrList)
            Dim transitionToCodeDone As New SerializableDictionary(Of GraphicalTransition, Boolean)
            'Initialization
            For Each Tr As GraphicalTransition In TrList
                transitionToCodeDone.Add(Tr, False)
            Next

            'Deactivation of steps
            r += t + t + "/*" + e
            r += t + t + " * Desactivació d'etapes" + e
            r += t + t + " */" + e + e
            r += GetEvolucioEtapes(Gr.Key, Gr.Value, transitionToCodeDone, True)

            'Reset
            transitionToCodeDone.Clear()
            For Each Tr As GraphicalTransition In TrList
                transitionToCodeDone.Add(Tr, False)
            Next

            'Activation of steps
            r += t + t + "/*" + e
            r += t + t + " * Activació d'etapes" + e
            r += t + t + " */" + e + e
            r += GetEvolucioEtapes(Gr.Key, Gr.Value, transitionToCodeDone, False)

            r += t + "}" + e + e
        Next Gr

        'Rising and falling edge flags
        r += t + "for (nG = 0; nG < nGRAFCET; nG++) {" + e
        r += t + t + "G[nG].FlagIniciEtapa.all =    G[nG].EtapaActiva.all & ~G[nG].EtapaActivaAux.all;" + e
        r += t + t + "G[nG].FlagFiEtapa.all =       ~G[nG].EtapaActiva.all & G[nG].EtapaActivaAux.all;" + e
        r += t + "}" + e + e

        r += "}" + e

        Return r
    End Function



    Private Function GetEvolucioEtapes(ByVal GrafcetNumber As Integer, _
        ByRef Bo As Body, _
        ByRef transitionToCodeDone As SerializableDictionary(Of GraphicalTransition, Boolean), _
        ByVal SonEtapesPrevies As Boolean) As String

        Dim r As String

        Dim IsSynchronism As Boolean
        Dim vt As String 'variable identation

        r = ""

        'For each step
        Dim StList As New GraphicalStepsList
        GetStepsByBody(Bo, StList)
        For Each St As BaseGraphicalStep In StList

            r += t + t + "// Etapa " + St.SuperBody.Name + ":" + St.Name + e

            '
            'IF active step
            '
            r += t + t + "if (G[" + Bo.GetGrafcet.Name + "].EtapaActivaAux.bits.bit" + _
                        m_gcmap.G(GrafcetNumber).stepToBit(St).ToString() + ") {" + e

            'For each transtion after the step
            For Each Tr As GraphicalTransition In GetTransitionsByPreviousStep(St)

                'IsSynchronism
                If GetStepsByNextTransition(Tr).Count > 1 Then
                    IsSynchronism = True
                    r += t + t + t + "// Sincronisme: transició " + Tr.Name + e
                Else
                    IsSynchronism = False
                End If

                'YES actions of the step have been already generated
                If Not transitionToCodeDone(Tr) Then

                    '
                    'IF receptive transition
                    '
                    r += t + t + t + "if (G[" + Bo.GetGrafcet.Name + "].Receptivitat.bits.bit" + _
                            m_gcmap.G(GrafcetNumber).transitionToBit(Tr).ToString + ") {" + e

                    If IsSynchronism Then
                        r += t + t + t + t + "if (1" + e

                        'print: IF all previous steps are true
                        '(There is no need to check the current one again)
                        For Each ExtraSt As BaseGraphicalStep In GetStepsByPreviousTransition(Tr)
                            If Not ExtraSt Is St Then
                                r += t + t + t + t + t + t + "&& " + "G[" + Bo.GetGrafcet.Name + "].EtapaActivaAux.bits.bit" + _
                                m_gcmap.G(GrafcetNumber).stepToBit(ExtraSt).ToString() + e
                            End If
                        Next
                        r += t + t + t + t + ") {" + e
                        vt = t
                    Else
                        vt = ""
                    End If

                    '
                    'Disable/Enable Previous/Next steps
                    '
                    If SonEtapesPrevies Then
                        r += GetDesactivaEtapesPrevies(GrafcetNumber, Bo, Tr, t + t + t + t + vt)
                    Else
                        r += GetActivaEtapesSeguents(GrafcetNumber, Bo, Tr, t + t + t + t + vt)
                    End If

                    If IsSynchronism Then r += t + t + t + t + "}" + e

                    'FI receptive transition
                    r += t + t + t + "}" + e

                    transitionToCodeDone(Tr) = True

                    'NO actions of the step have been already generated
                Else
                    r += t + t + t + "// Codi ja generat abans." + e

                    'FI actions of the step have been already generated
                End If

            Next Tr

            'print: FI active step
            r += t + t + "}" + e + e



        Next St

        Return r
    End Function

    'For code generation, macro-steps have to be expanded.
    'If a step is the end step of a macro-step's expansion, transitions of the macro-step become
    'transitions of the step.
    '
    '    [ ] <-------- Input
    '     |
    '----------
    '+  +  +  + <----- Output
    Private Function GetTransitionsByPreviousStep(ByRef St As BaseGraphicalStep) As GraphicalTransitionsList
        Dim r As GraphicalTransitionsList

        Dim SBo As Body = St.SuperBody 'SuperBody

        r = SBo.GetTransitionsByPreviousStep(St)

        'IF final step
        If r.Count = 0 Then

            'IF belongs to the expansion of a macro-step
            If SBo.GetType.Name = "MacroStepBody" Then

                'Transitions of the macro-step become the transitions of the step
                Dim SSt As GraphicalMacroStep = CType(SBo, MacroStepBody).SuperStep 'SuperStep
                r = GetTransitionsByPreviousStep(SSt)
            End If
        End If

        Return r
    End Function

    'For code generation, macro-steps have to be expanded.
    'Steps of a body are not only its own steps, but also the steps of
    'its macro-step's expansions.
    '@param[in] Bo Body which contains the steps
    '@param[out] StList Already initialized list of steps
    Private Sub GetStepsByBody(ByRef Bo As Body, _
                ByRef StList As GraphicalStepsList)

        For Each St As BaseGraphicalStep In Bo.GraphicalStepsList
            If St.IsAStep Then
                StList.Add(St)
            ElseIf St.GetType.Name = "GraphicalMacroStep" Then
                GetStepsByBody(CType(St, GraphicalMacroStep).ReadSubBody, StList)
            End If
        Next

    End Sub

    'For code generation, macro-steps have to be expanded.
    'If a step that precedes a transition is a macro-step, the exit step of the macro-step's 
    'expansion become previous step of the transition.
    '@see EN60848-2000 - 7.4 Structuring using the macro-steps
    'There is one and only one exit step per expansion of macro-step, and it can not
    'be a macro-step
    '
    '[ ] [ ] [ ] [ ] <----- Output
    ' |   |   |   |
    '==============
    '       + <------------ Input
    Private Function GetStepsByNextTransition(ByRef Tr As GraphicalTransition) As GraphicalStepsList
        Dim r As New GraphicalStepsList

        'For each previous step
        For Each St As BaseGraphicalStep In Tr.ReadPreviousGraphicalStepsList

            'IF regular step
            If St.IsAStep Then

                r.Add(St)

                'IF macro-step
            ElseIf St.GetType.Name = "GraphicalMacroStep" Then

                'Get the exit step of the macro-step's expansion
                r.Add(CType(St, GraphicalMacroStep).ReadSubBody.GetExitStep)
                'Macro-step's expansions have only one exit step

            End If
        Next St

        Return r
    End Function

    'For code generation, macro-steps have to be expanded.
    'If a step that follows a transition is a macro-step, initial steps of the macro-step's 
    'expansion become next steps of the transition.
    '@see EN60848-2000 - 7.4 Structuring using the macro-steps - NOTE 1: 
    'The expansion of a macro-step can have one or several initial steps*, and they can not
    'be macro-steps.
    'As opposed to what it is stated in 
    'GeCé's report "Metodologia per a implementar automatismes GRAFCET en microprocessadors 
    'programats en C", page 39: "[...] A macro-step is a step which contains a sequence with
    'a single entry step [...]"
    '
    '       + <------------ Input
    '==============
    ' |   |   |   |
    '[ ] [ ] [ ] [ ] <----- Output
    Private Function GetStepsByPreviousTransition(ByRef Tr As GraphicalTransition) As GraphicalStepsList
        Dim r As New GraphicalStepsList

        'For each next step
        For Each St As BaseGraphicalStep In Tr.ReadNextGraphicalStepsList

            'IF regular step
            If St.IsAStep Then

                r.Add(St)

                'IF macro-step
            ElseIf St.GetType.Name = "GraphicalMacroStep" Then

                'For each initial step of the macro-step's expansion
                For Each SubSt As BaseGraphicalStep In CType(St, GraphicalMacroStep).ReadSubBody.GraphicalStepsList
                    If SubSt.IsAStep() Then
                        If CType(SubSt, GraphicalStep).Initial Then

                            r.Add(SubSt)

                        End If
                    End If
                Next

            End If
        Next St

        Return r
    End Function

    Private Function GetDesactivaEtapesPrevies(ByVal GrafcetNumber As Integer, _
        ByRef Bo As Body, _
        ByRef Tr As GraphicalTransition, ByVal Indentation As String) As String
        Dim r As String
        r = ""

        'For each previous step
        For Each St As BaseGraphicalStep In GetStepsByNextTransition(Tr)

            'Disable it
            r += Indentation + "G[" + Bo.GetGrafcet.Name + "].EtapaActiva.bits.bit" + _
                                    m_gcmap.G(GrafcetNumber).stepToBit(St).ToString + " = 0;" + e
        Next St

        Return r
    End Function

    Private Function GetActivaEtapesSeguents(ByVal GrafcetNumber As Integer, _
            ByRef Bo As Body, _
            ByRef Tr As GraphicalTransition, ByVal Indentation As String) As String
        Dim r As String
        r = ""

        'For each next step
        For Each St As BaseGraphicalStep In GetStepsByPreviousTransition(Tr)

            'Enable it
            r += Indentation + "G[" + Bo.GetGrafcet.Name + "].EtapaActiva.bits.bit" + _
                                    m_gcmap.G(GrafcetNumber).stepToBit(St).ToString + " = 1;" + e
        Next St

        Return r
    End Function

End Class
