Public Class GCAccionsContinues
    Inherits GCFunctions

    Public Sub New(ByRef c As GCFunctionsController)
        MyBase.New(c)
    End Sub

    Public Function GetAccionsContinues() As String
        Dim r As String 'result

        r = GetIncludes() + e

        r += "void AccionsContinues(void)" + e + _
            "{" + e + _
            t + "unsigned char nG;" + e + _
            t + e

        r += t + "/*" + e
        r += t + " * All grafcets are unforced, but the enclosed ones" + e
        r += t + " */" + e + e

        r += t + "for (nG = 0; nG < nGRAFCET; nG++)" + e + _
            t + t + "G[nG].Forcat = 0;" + e + _
            e

        For Each Gr As KeyValuePair(Of Integer, Grafcet) In m_gcmap.numToGrafcet
            If Gr.Value.IsAEnclosure Then
                r += t + "G[" + Gr.Value.Name + "].Forcat = 1;" + e
            End If
        Next
        r += e

        r += t + "/*" + e
        r += t + " * For all steps, execute associated forcing orders" + e
        r += t + " *  and grafcet forcing" + e
        r += t + " */" + e + e
        For Each Gr As KeyValuePair(Of Integer, Grafcet) In m_gcmap.numToGrafcet

            r += t + "// Grafcet " + Gr.Value.Name + e + e

            For Each St As BaseGraphicalStep In Gr.Value.GraphicalStepsList

                If St.IsAStep Then

                    If CType(St, GraphicalStep).getForcingOrders().Count > 0 Then

                        r += t + "if (G[" + Gr.Value.Name + "].EtapaActiva.bits.bit" + _
                                m_gcmap.G(Gr.Key).stepToBit(St).ToString + ") {" + e

                        For Each FO As GraphicalForcingOrder In CType(St, GraphicalStep).getForcingOrders()
                            r += t + t + "G[" + FO.Grafcet.Name + "].Forcat = 1;" + e
                            r += GetForcingOrderEtapaActiva(t + t, FO)
                        Next

                        r += t + "}" + e + e

                    End If

                End If

                If St.GetType.Name = "GraphicalEnclosingStep" Then
                    r += t + "if (G[" + Gr.Value.Name + "].EtapaActiva.bits.bit" + _
                            m_gcmap.G(Gr.Key).stepToBit(St).ToString + ") {" + e

                    For Each En As Enclosure In CType(St, GraphicalEnclosingStep).EnclosureList
                        r += t + t + "G[" + En.Name + "].Forcat = 0;" + e
                    Next

                    r += t + "}" + e + _
                    e
                End If

            Next
        Next
        r += e

        r += t + "/*" + e
        r += t + " * For all steps, execute associated continous actions" + e
        r += t + " */" + e

        r += t + "SortidaDig.all = 0x0000;" + e

        For Each Gr As KeyValuePair(Of Integer, Grafcet) In m_gcmap.numToGrafcet
            For Each St As BaseGraphicalStep In Gr.Value.GraphicalStepsList

                If St.IsAStep Then
                    If CType(St, GraphicalStep).getActionsByType(enumActionType.Continuous).Count > 0 Then

                        r += t + "if (G[" + Gr.Value.Name + "].EtapaActiva.bits.bit" + _
                                m_gcmap.G(Gr.Key).stepToBit(St).ToString + ") {" + e

                        For Each Ac As Action In CType(St, GraphicalStep).getActionsByType(enumActionType.Continuous)
                            r += GetAccioContinua(Gr.Key, Ac)
                        Next

                        r += t + "}" + e + e
                    End If
                End If
            Next
        Next Gr
        r += e

        r += "}" + e

        Return r
    End Function

    Private Function GetAccioContinua(ByVal numG As Integer, ByRef Ac As Action) As String
        Dim r As String = ""
        Dim et As String = "" 'extra tab
        Dim HayCond As Boolean

        HayCond = Ac.Cond.GetString <> "true"

        If HayCond Then
            r += GetCondition(t + t, numG, Ac.Cond)
            et = t
        End If

        r += _
            et + t + t + "SortidaDig.bits.bit" + _
            m_gcmap.digitalOutputToBit(Ac.Var).ToString + _
            " = 1; // " + Ac.Var.Name + e

        If HayCond Then
            r += t + t + "}" + e
        End If

        Return r
    End Function

    Private Function GetForcingOrderEtapaActiva(ByVal Indentation As String, _
            ByRef FO As GraphicalForcingOrder) As String

        Dim r As String = "" 'result

        Select Case FO.Situation

            'Explicit
            Case enumSituationType.Explicit
                r += "G[" + FO.Grafcet.Name + "].EtapaActiva.all = ("

                Dim isFirst As Boolean = True
                For Each St As GraphicalStep In FO.SubSteps
                    If isFirst Then
                        isFirst = False
                    Else
                        r += " | "
                    End If
                    r += "BIT" + m_gcmap.G(m_gcmap.grafcetToNum(FO.Grafcet)).stepToBit(St).ToString
                Next

                r += ");" + e

                'Current
            Case enumSituationType.Current
                'Don't change EtapaActiva

                'Empty
            Case enumSituationType.Empty
                r += "G[" + FO.Grafcet.Name + "].EtapaActiva.all = 0;" + e

                'Initial
            Case enumSituationType.Initial
                r += "G[" + FO.Grafcet.Name + "].EtapaActiva.all = ("

                Dim isFirst As Boolean = True
                For Each St As GraphicalStep In FO.Grafcet.GetInitialSteps
                    If isFirst Then
                        isFirst = False
                    Else
                        r += " | "
                    End If
                    r += "BIT" + m_gcmap.G(m_gcmap.grafcetToNum(FO.Grafcet)).stepToBit(St).ToString
                Next
                r += ");" + e

        End Select

        Return r
    End Function

End Class
