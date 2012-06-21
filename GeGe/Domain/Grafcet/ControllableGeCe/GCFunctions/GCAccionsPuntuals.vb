Public Class GCAccionsPuntuals
    Inherits GCFunctions

    Public Sub New(ByRef c As GCFunctionsController)
        MyBase.New(c)
    End Sub

    '@see GeCé - 6. IMPLEMENTACIÓ DE GRAFCETS ENCAPSATS
    'Per a la correcta evolució dels Grafcets amb grafcet encapsat
    'cal transformar les etapes capses en etapes amb les següents
    'accions puntuals a l'activació i desactivació
    'A l'activació: carregar l'estat inicial, així com els flags
    'd'inici d'etapa. Alliberar el grafcet encapsat posant la variable
    'Forcat a zero.
    'A la desactivació: carregar l'estat auxiliar del grafcet encapsat
    'als flags de fi d'etapa, anul·lar-ne totes les etapes i forçar-lo.
    Public Function GetAccionsPuntuals() As String
        Dim r As String 'result

        r = GetIncludes() + e

        r += "void AccionsPuntuals(void)" + e + _
            "{" + e

        'Per ordre de jerarquia 
        For Each Gr As KeyValuePair(Of Integer, Grafcet) In m_gcmap.numToGrafcet

            r += t + "/*" + e
            r += t + " * Grafcet " + Gr.Value.Name + e
            r += t + " */" + e + e

            r += t + "// Accions associades a la desactivació de l'etapa" + e
            r += t + "if (G[" + Gr.Key.ToString + "].FlagFiEtapa.all != 0) {" + e
            For Each St As BaseGraphicalStep In _
                    getStepsWithActByEvent(Gr.Value, enumActionEvent.Deactivation)
                r += t + t + "if (G[" + Gr.Key.ToString + "].FlagFiEtapa.bits.bit" + _
                        m_gcmap.G(Gr.Key).stepToBit(St).ToString + ") {" + e
                For Each Ac As Action In getActionsByEvent(St, enumActionEvent.Deactivation)
                    r += t + t + t + Ac.Assignation + ";" + e
                Next
                r += t + t + "}" + e
            Next
            r += t + "}" + e + e

            'Accions associades al franqueig de la transició

            r += t + "// Accions associades a l'activació de l'etapa" + e
            r += t + "if (G[" + Gr.Key.ToString + "].FlagIniciEtapa.all != 0) {" + e
            For Each St As BaseGraphicalStep In _
                    getStepsWithActByEvent(Gr.Value, enumActionEvent.Activation)
                r += t + t + "if (G[" + Gr.Key.ToString + "].FlagIniciEtapa.bits.bit" + _
                        m_gcmap.G(Gr.Key).stepToBit(St).ToString + ") {" + e
                For Each Ac As Action In getActionsByEvent(St, enumActionEvent.Activation)
                    r += t + t + t + Ac.Assignation + ";" + e
                Next
                r += t + t + "}" + e
            Next
            r += t + "}" + e + e

        Next

        r += "}" + e

        Return r
    End Function


    Private Function getStepsWithActByEvent(ByRef Gr As Grafcet, _
            ByVal actEvent As enumActionEvent) As GraphicalStepsList

        Dim r As New GraphicalStepsList

        For Each St As BaseGraphicalStep In Gr.GraphicalStepsList
            If getActionsByEvent(St, actEvent).Count > 0 Then
                r.Add(St)
            End If
        Next

        Return r
    End Function

    Private Function getActionsByEvent(ByRef St As BaseGraphicalStep, _
            ByVal actEvent As enumActionEvent) As List(Of Action)

        Dim r As New List(Of Action)

        r.AddRange(St.getActionsByEvent(actEvent))

        'SI GraphicalEnclosingStep add actions resulting from 
        'transformation specified by GeCé methodology
        If St.GetType.Name = "GraphicalEnclosingStep" Then
            Dim Assig As String 'aux
            Dim Act As Action 'aux

            Select Case actEvent

                Case enumActionEvent.Activation

                    For Each En As Enclosure In CType(St, GraphicalEnclosingStep).EnclosureList

                        'Carregar l'estat inicial
                        Assig = "G[" + En.Name + "].EtapaActiva.all = ENC_INI_" + En.Name
                        Act = New Action(enumActionType.Stored, Nothing, Nothing, Assig, _
                                enumActionEvent.Activation)
                        r.Add(Act)

                        'Escriure flags d'inici d'etapa
                        Assig = "G[" + En.Name + "].FlagIniciEtapa.all = G[" + En.Name + "].EtapaActiva.all"
                        Act = New Action(enumActionType.Stored, Nothing, Nothing, Assig, _
                                enumActionEvent.Activation)
                        r.Add(Act)

                        'Alliberar el grafcet
                        Assig = "G[" + En.Name + "].Forcat = 0"
                        Act = New Action(enumActionType.Stored, Nothing, Nothing, Assig, _
                                enumActionEvent.Activation)
                        r.Add(Act)

                    Next


                Case enumActionEvent.Deactivation

                    For Each En As Enclosure In CType(St, GraphicalEnclosingStep).EnclosureList

                        'Escriure els flags de fi d'etapa
                        Assig = "G[" + En.Name + "].FlagFiEtapa.all = G[" + En.Name + "].EtapaActivaAux.all"
                        Act = New Action(enumActionType.Stored, Nothing, Nothing, Assig, _
                                enumActionEvent.Activation)
                        r.Add(Act)

                        'Anular totes les etapes
                        Assig = "G[" + En.Name + "].EtapaActiva.all = 0x0000"
                        Act = New Action(enumActionType.Stored, Nothing, Nothing, Assig, _
                                enumActionEvent.Activation)
                        r.Add(Act)

                        'Forçar el grafcet
                        Assig = "G[" + En.Name + "].Forcat = 1"
                        Act = New Action(enumActionType.Stored, Nothing, Nothing, Assig, _
                                enumActionEvent.Activation)
                        r.Add(Act)

                    Next

            End Select
        End If


        Return r
    End Function

End Class
