Public Class GCAltresRutines
    Inherits GCFunctions

    Public Sub New(ByRef c As GCFunctionsController)
        MyBase.New(c)
    End Sub

    Public Function GetAltresRutines() As String
        Dim r As String

        r = GetIncludes() + e

        r += GetIniciMicro() + e
        r += GetIniciGrafcet() + e
        r += GetGestioTempo() + e
        r += GetTempsAdeq() + e
        r += GetAssignaAux() + e
        r += GetGrafcetEstable() + e

        Return r
    End Function

    Private Function GetIniciMicro() As String
        Dim r As String 'result

        r = "void IniciMicro(void)" + e + _
             "{" + e
        r += "}" + e

        Return r
    End Function

    'pre: les condicions temporals estan introduides en segons.
    'pre: la unitat dels comptadors és el milisegon.
    Private Function GetIniciGrafcet() As String
        Dim r As String 'result
        Dim ea_str As String 'initial steps string

        r = "void IniciGrafcet(void)" + e + _
            "{" + e

        'per a cada grafcet
        For Each Gr As KeyValuePair(Of Integer, Grafcet) In m_gcmap.numToGrafcet

            'Forçat
            If Gr.Value.IsAEnclosure Then
                r += t + "G[" + Gr.Value.Name + "].Forcat = 1;" + e
            Else
                r += t + "G[" + Gr.Value.Name + "].Forcat = 0;" + e
            End If

            'EtapaActiva
            ea_str = "0"
            For Each St As BaseGraphicalStep In Gr.Value.GraphicalStepsList
                If St.IsAStep Then
                    If CType(St, GraphicalStep).Initial Then
                        ea_str += " | BIT" + m_gcmap.G(Gr.Key).stepToBit(St).ToString
                    End If
                End If
            Next St
            r += t + "G[" + Gr.Value.Name + "].EtapaActiva.all = " + ea_str + ";" + e

            'FlagIniciEtapa
            r += t + "G[" + Gr.Value.Name + "].FlagIniciEtapa.all = G[" + Gr.Value.Name + "].EtapaActiva.all;" + e + e
        Next Gr



        '
        'inicialitza comptadors
        '

        'pujada
        For Each Ac As KeyValuePair(Of Integer, TimeDependentCondition) _
                In m_gcmap.PBitToTimedCondition

        Next

        'pujada i baixada
        For Each Co As KeyValuePair(Of Integer, TimeDependentCondition) _
                In m_gcmap.PBBitToTimedCondition
            r += t + "TLimitPB[" + Co.Key.ToString + "].all = " + _
                    (Co.Value.TimeAfterRise * 1000).ToString + ";" + e
            r += t + "TLimitPB[" + (Co.Key + 1).ToString + "].all = " + _
                    (Co.Value.TimeAfterFall * 1000).ToString + ";" + e
        Next

        r += e
        r += t + "BitTimerP.all = 0x0000;" + e + _
             t + "BitTimerPB.all = 0x0000;" + e + _
             t + "FlancP.all = 0x0000;" + e + _
             t + "FlancB.all = 0x0000;" + e + e

        'inicialitza les variables internes
        Dim val As String
        For Each var As KeyValuePair(Of String, Variable) In m_gcmap.GCnameToVar
            If var.Value.dataType = "BOOL" Then
                val = m_gcmap.BoolStrToBoolInt(var.Value.InitialValueToUniversalString).ToString
            Else
                val = var.Value.InitialValueToUniversalString
            End If
            r += t + var.Key + " = " + val + ";" + e
        Next
        r += e

        'inicialitza el mòdul de lectura d'entrades del sistema
        r += t + "LecturaEntradesFisiques();" + e

        r += "}" + e

        Return r
    End Function

    Private Function GetGestioTempo() As String
        Dim r As String
        'Dim gnum As String
        'Dim sbit As String
        'Dim tbit As String

        r = "void GestioTempo()" + e + _
            "{" + e

        'comptadors de pujada
        'For Each Gr As KeyValuePair(Of Integer, Grafcet) In m_gcmap.numToGrafcet
        ' gnum = Gr.Key
        '
        '        For Each St As BaseGraphicalStep In Gr.Value.GraphicalStepsList
        ' If St.IsAStep Then
        ' sbit = m_gcmap.G(Gr.Key).stepToBit(St).ToString
        '
        '        If (m_gcmap.stepsTimerBit.ContainsKey(St)) Then
        ' tbit = m_gcmap.stepsTimerBit(St).ToString
        '
        '        r += t + "if (BitTimerP.bits.bit" + tbit + ") {" + e + _
        ' t(+t + "if (TimerP[" + tbit + "].all >= TLimitP[" + tbit + "].all) {" + e + _
        ' t + t + t + "BitTimerP.bits.bit" + tbit + " = 0;" + e + _
        ' t + t + "}" + e + _
        ' t + "} else if (G[" + gnum + "].FlagIniciEtapa.bits.bit" + sbit + ") {" + e + _
        ' t + t + "BitTimerP.bits.bit" + tbit + " = 1;" + e + _
        ' t + t + "TimerP[" + tbit + "].all = 0x0000;" + e + _
        ' t + "}" + e + _
        ' e)
        ' End If
        ' End If
        ' Next
        ' Next

        'comptadors de pujada i baixada
        Dim bp As String 'bit de pujada
        Dim bb As String 'bit de baixada
        Dim bf As String 'bit de flanc
        For Each Co As KeyValuePair(Of Integer, TimeDependentCondition) In m_gcmap.flancBitToTimedCondition
            r += t + "/*" + e
            r += t + " * Condition: " + Co.Value.GetString + "" + e
            r += t + " */" + e

            bp = m_gcmap.timedConditionToPBBit(Co.Value).ToString
            bb = (m_gcmap.timedConditionToPBBit(Co.Value) + 1).ToString
            bf = Co.Key.ToString
            r += t + "if (BitTimerPB.bits.bit" + bp + ") {" + e + _
                t + t + "if (TimerPB[" + bp + "].all >= TLimitPB[" + bp + "].all) {" + e + _
                t + t + t + "BitTimerPB.bits.bit" + bp + " = 0;" + e + _
                t + t + "}" + e + _
                t + "} else if (FlancP.bits.bit" + bf + ") {" + e + _
                t + t + "BitTimerPB.bits.bit" + bp + " = 1;" + e + _
                t + t + "TimerPB[" + bp + "].all = 0x0000;" + e + _
                t + "} else if (BitTimerPB.bits.bit" + bb + ") {" + e + _
                t + t + "if (TimerPB[" + bb + "].all >= TLimitPB[" + bb + "].all) {" + e + _
                t + t + t + "BitTimerPB.bits.bit" + bb + " = 0;" + e + _
                t + t + "}" + e + _
                t + "} else if (FlancB.bits.bit" + bf + ") {" + e + _
                t + t + "BitTimerPB.bits.bit" + bb + " = 1;" + e + _
                t + t + "TimerPB[" + bb + "].all = 0x0000;" + e + _
                t + "}" + e + e
        Next

        r += "}" + e

        Return r
    End Function

    'pre: Timer-related variables are represented in milliseconds
    Private Function GetTempsAdeq() As String
        Dim r As String

        r = "void temps_adeq(void)" + e
        r += "{" + e
        r += t + "unsigned char nT;" + e + e

        'Incrementa TimerP
        If m_gcmap.nTempP > 0 Then
            r += t + "for (nT = 0; nT < nTempP; nT++)" + e
            r += t + t + "TimerP[nT].all = TimerP[nT].all + " + _
                    m_TimerPeriod.ToString + ";" + e + e
        End If

        'Incrementa TimerPB
        If m_gcmap.nTempPB > 0 Then
            r += t + "for (nT = 0; nT < nTempPB; nT++)" + e
            r += t + t + "TimerPB[nT].all = TimerPB[nT].all + " + _
                    m_TimerPeriod.ToString + ";" + e + e
        End If

        r += "}" + e

        Return r
    End Function

    Private Function GetAssignaAux() As String
        Dim r As String 'result

        r = "void AssignaAux(void)" + e + _
             "{" + e
        r += "    unsigned char nG;" + e
        r += e
        r += "    for (nG = 0; nG < nGRAFCET; nG++)" + e
        r += "        G[nG].EtapaActivaAux.all = G[nG].EtapaActiva.all;" + e
        r += "}" + e

        Return r
    End Function

    Public Function GetGrafcetEstable() As String
        Dim r As String 'result

        r = "unsigned char GrafcetEstable(void)" + e + _
             "{" + e
        r += t + "unsigned char nG;" + e
        r += t + "unsigned char estable;" + e
        r += t + "" + e
        r += t + "nG = 0;" + e
        r += t + "estable = 1;" + e
        r += t + "" + e
        r += t + "while ((nG < nGRAFCET) && estable)" + e
        r += t + "{" + e
        r += t + t + "estable = (G[nG].EtapaActiva.all == G[nG].EtapaActivaAux.all);" + e
        r += t + t + "nG++;" + e
        r += t + "}" + e
        r += t + "return(estable);" + e
        r += "}" + e

        Return r
    End Function

End Class
