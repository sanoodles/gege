'Mapping between domains
'Named "Variables and constants definition" in GeCé methodology
'@see: http://upcommons.upc.edu/pfc/bitstream/2099.1/2731/1/36670-1.pdf
Public Class GCMapping

    Public numToGrafcet As SerializableDictionary(Of Integer, Grafcet)
    Public grafcetToNum As SerializableDictionary(Of Grafcet, Integer)

    Public digitalInputToBit As SerializableDictionary(Of BooleanVariable, Integer)
    Public bitToDigitalInput As SerializableDictionary(Of Integer, BooleanVariable)

    Public digitalOutputToBit As SerializableDictionary(Of BooleanVariable, Integer)
    Public bitToDigitalOutput As SerializableDictionary(Of Integer, BooleanVariable)

    Public VarToGCname As SerializableDictionary(Of Variable, String) 'not needed, but for symmetry with nameToInput
    Public GCnameToVar As SerializableDictionary(Of String, Variable)

    Public timedConditionToFlancBit As SerializableDictionary(Of TimeDependentCondition, Integer)
    Public flancBitToTimedCondition As SerializableDictionary(Of Integer, TimeDependentCondition)

    Public timedConditionToPBit As SerializableDictionary(Of TimeDependentCondition, Integer)
    Public PBitToTimedCondition As SerializableDictionary(Of Integer, TimeDependentCondition)

    Public timedConditionToPBBit As SerializableDictionary(Of TimeDependentCondition, Integer)
    Public PBBitToTimedCondition As SerializableDictionary(Of Integer, TimeDependentCondition)

    Public nETAPES As Integer
    Public nTempP As Integer
    Public nTempPB As Integer

    Public Structure grafcetGENERAL

        'which is the bit for a step in EtapaActiva, EtapaActivaAux, ...
        Public stepToBit As SerializableDictionary(Of BaseGraphicalStep, Integer)
        Public bitToStep As SerializableDictionary(Of Integer, BaseGraphicalStep)

        'which is the bit for a transition in Receptivitat
        Public transitionToBit As SerializableDictionary(Of GraphicalTransition, Integer)
        Public bitToTransition As SerializableDictionary(Of Integer, GraphicalTransition)

    End Structure

    'which are the bits for steps, transitions, and timers for each grafcet
    Public G As SerializableDictionary(Of Integer, grafcetGENERAL)

    'which are the steps that have to be activated when the enclosure G# is enabled
    Public encToEncIni As SerializableDictionary(Of Enclosure, Integer)

    Public Sub New(ByRef p As Project)
        numToGrafcet = p.numToGrafcet
        grafcetToNum = p.grafcetToNum

        nETAPES = p.MaxSteps

        MapG(p)
        MapVariables(p)
        MapEncIniG(p)
        MapTimers(p)
    End Sub

    Public Function GetStepFromBit(ByVal grafcet As Integer, ByVal bit As Integer) As BaseGraphicalStep
        Return G(grafcet).bitToStep(bit)
    End Function

    Public Function BoolStrToBoolInt(ByVal BoolStr As String) As Integer
        Return Integer.Parse(CInt(Boolean.Parse(BoolStr)) * -1)
    End Function



    '
    'Map expanded GeGé steps and transitions to GeCé steps and transitions
    '
    Private Sub MapG(ByRef p As Project)
        '
        'G[]
        '
        Dim i As Integer 'multipurpose iterator
        Dim g_aux As grafcetGENERAL = Nothing

        G = New SerializableDictionary(Of Integer, grafcetGENERAL)

        'for each grafcet
        For Each Gr As KeyValuePair(Of Integer, Grafcet) In p.numToGrafcet

            'Map steps
            'Map timers
            'Map transitions


            'Map steps
            g_aux.stepToBit = New SerializableDictionary(Of BaseGraphicalStep, Integer)
            g_aux.bitToStep = New SerializableDictionary(Of Integer, BaseGraphicalStep)
            i = 0
            MapBodySteps(Gr.Value, g_aux, i)

            'Map transitions
            g_aux.transitionToBit = New SerializableDictionary(Of GraphicalTransition, Integer)
            g_aux.bitToTransition = New SerializableDictionary(Of Integer, GraphicalTransition)
            i = 0
            MapBodyTransitions(Gr.Value, g_aux, i)

            G.Add(Gr.Key, g_aux)
        Next

    End Sub

    '@param Bo What to map. Body the steps of which will be mapped.
    '@param g_aux Where to map. generalGRAFCET which contains the field that will store the mapping.
    '@param i Bit number for the first step of the body.
    Private Sub MapBodySteps(ByRef Bo As Body, ByRef g_aux As grafcetGENERAL, _
            ByRef i As Integer)

        For Each St As BaseGraphicalStep In Bo.GraphicalStepsList
            If St.IsAStep Then
                g_aux.stepToBit.Add(St, i)
                g_aux.bitToStep.Add(i, St)
                i = i + 1
            Else
                MapBodySteps(CType(St, GraphicalMacroStep).ReadSubBody, g_aux, i)
            End If
        Next
    End Sub

    '@param Bo What to map. Body the transitions of which will be mapped.
    '@param g_aux Where to map. generalGRAFCET which contains the field that will store the mapping.
    '@param i Bit number for the first transition of the body.
    Private Sub MapBodyTransitions(ByRef Bo As Body, ByRef g_aux As grafcetGENERAL, _
            ByRef i As Integer)

        For Each Tr As GraphicalTransition In Bo.GraphicalTransitionsList
            g_aux.transitionToBit.Add(Tr, i)
            g_aux.bitToTransition.Add(i, Tr)
            i = i + 1
        Next

        For Each St As BaseGraphicalStep In Bo.GraphicalStepsList
            If St.GetType.Name = "GraphicalMacroStep" Then
                MapBodyTransitions(CType(St, GraphicalMacroStep).ReadSubBody, g_aux, i)
            End If
        Next
    End Sub

    Private Sub MapVariables(ByRef p As Project)
        VarToGCname = New SerializableDictionary(Of Variable, String)
        GCnameToVar = New SerializableDictionary(Of String, Variable)

        'project variables
        For Each Va As Variable In p.ProjectVariables
            VarToGCname.Add(Va, Va.Name)
            GCnameToVar.Add(Va.Name, Va)
        Next

        'grafcets variables
        For Each Gr As Grafcet In p.numToGrafcet.Values
            For Each Va As Variable In Gr.GrafcetVariables
                VarToGCname.Add(Va, Va.Name)
                GCnameToVar.Add(Va.Name, Va)
            Next
        Next

        MapEntrades(p)
        MapSortides(p)
    End Sub

    '
    'Map GeGé project's and GeGé grafcets' variables to GeCé's Entrada
    '
    Private Sub MapEntrades(ByRef p As Project)
        Dim i_input As Integer

        digitalInputToBit = New SerializableDictionary(Of BooleanVariable, Integer)
        bitToDigitalInput = New SerializableDictionary(Of Integer, BooleanVariable)

        i_input = 0

        'project variables
        For Each Va As Variable In p.ProjectVariables
            If Va.Application = enumVariableApplication.Input Then
                If Va.dataType = "BOOL" Then
                    digitalInputToBit.Add(CType(Va, BooleanVariable), i_input)
                    bitToDigitalInput.Add(i_input, CType(Va, BooleanVariable))
                    i_input = i_input + 1
                End If
                'TODO: analog input project variables mapping
            End If
        Next

        'grafcets variables
        For Each Gr As Grafcet In p.numToGrafcet.Values
            For Each Va As Variable In Gr.GrafcetVariables
                If Va.Application = enumVariableApplication.Input Then
                    If Va.dataType = "BOOL" Then
                        digitalInputToBit.Add(CType(Va, BooleanVariable), i_input)
                        bitToDigitalInput.Add(i_input, CType(Va, BooleanVariable))
                        i_input = i_input + 1
                    End If
                    'TODO: analog input grafcet variables mapping
                End If
            Next
        Next

    End Sub

    Private Sub MapSortides(ByRef p As Project)
        Dim i_output As Integer

        digitalOutputToBit = New SerializableDictionary(Of BooleanVariable, Integer)
        bitToDigitalOutput = New SerializableDictionary(Of Integer, BooleanVariable)

        i_output = 0

        'project variables
        For Each Va As Variable In p.ProjectVariables
            If Va.Application = enumVariableApplication.Output Then
                If Va.dataType = "BOOL" Then
                    digitalOutputToBit.Add(CType(Va, BooleanVariable), i_output)
                    bitToDigitalOutput.Add(i_output, CType(Va, BooleanVariable))
                    i_output = i_output + 1
                End If
                'TODO: analog output project variables mapping
            End If
        Next

        'grafcets variables
        For Each Gr As Grafcet In p.numToGrafcet.Values
            For Each Va As Variable In Gr.GrafcetVariables
                If Va.Application = enumVariableApplication.Output Then
                    If Va.dataType = "BOOL" Then
                        digitalOutputToBit.Add(CType(Va, BooleanVariable), i_output)
                        bitToDigitalOutput.Add(i_output, CType(Va, BooleanVariable))
                        i_output = i_output + 1
                    End If
                    'TODO: analog output grafcet variables mapping
                End If
            Next
        Next

    End Sub

    'Define values for ENC_INI_G#
    Private Sub MapEncIniG(ByRef p As Project)
        Dim ENC_INI_ACTUAL As Integer

        encToEncIni = New SerializableDictionary(Of Enclosure, Integer)

        'For each enclosure
        For Each Gr As Grafcet In p.numToGrafcet.Values
            If Gr.IsAEnclosure Then
                ENC_INI_ACTUAL = 0

                'For each step with activation link
                For Each St As BaseGraphicalStep In Gr.GraphicalStepsList
                    If St.GetType.Name = "GraphicalEnclosedStep" Then
                        If CType(St, GraphicalEnclosedStep).ActivationLink Then 'Or _
                            'CType(St, GraphicalStep).Initial Then
                            ENC_INI_ACTUAL += 2 ^ G(grafcetToNum(Gr)).stepToBit(St)
                        End If
                    End If
                Next

                encToEncIni.Add(Gr, ENC_INI_ACTUAL)
            End If
        Next

    End Sub

    'Supports
    '* Continuous actions
    '** Time dependent assignation conditions
    '*** Input variable: DONE
    '*** Step variable: TODO
    '** Delayed action: TODO
    '** Time limited action: TODO
    '
    'Maps between
    '* Timed actions and edge bits
    '* Timed actions and timerP bits
    '* Timed actions and timerPB bits
    '
    Private Sub MapTimers(ByRef p As Project)
        Dim pb As Integer = 0 'number of bit for TimerP, TlimitP, BitTimerP
        Dim pbb As Integer = 0 'number of bit for TimerPB, TlimitPB, BitTimerPB
        
        timedConditionToFlancBit = New SerializableDictionary(Of TimeDependentCondition, Integer)
        flancBitToTimedCondition = New SerializableDictionary(Of Integer, TimeDependentCondition)

        timedConditionToPBit = New SerializableDictionary(Of TimeDependentCondition, Integer)
        PBitToTimedCondition = New SerializableDictionary(Of Integer, TimeDependentCondition)

        timedConditionToPBBit = New SerializableDictionary(Of TimeDependentCondition, Integer)
        PBBitToTimedCondition = New SerializableDictionary(Of Integer, TimeDependentCondition)

        For Each Gr As Grafcet In p.numToGrafcet.Values
            For Each St As BaseGraphicalStep In Gr.GraphicalStepsList
                For Each Ac As Action In St.getActionsByType(enumActionType.Continuous)
                    If Ac.IsTimed Then
                        MapTimer(Ac.Cond, pb, pbb)
                    End If
                Next
            Next

            For Each Tr As GraphicalTransition In Gr.GraphicalTransitionsList
                If Tr.IsTimed Then
                    MapTimer(Tr.Condition, pb, pbb)
                End If
            Next

        Next

    End Sub

    Private Sub MapTimer(ByRef Co As Condition, ByRef pb As Integer, _
            ByRef pbb As Integer)

        Dim tv As Variable 'timed variable
        Dim fb As Integer 'number of bit for FlancP (or FlancB) of the timed variable

        tv = GetVariableFromCondition(Co)
        fb = digitalInputToBit(tv)
        timedConditionToFlancBit.Add(Co, fb)
        flancBitToTimedCondition.Add(fb, Co)
        nTempPB += 1

        timedConditionToPBBit.Add(Co, pbb)
        PBBitToTimedCondition.Add(pbb, Co)
        pbb = pbb + 2 'one bit for rising edge, one bit for falling edge
    End Sub

    'pre: the condition's boolean expression contains just one variable
    Private Function GetVariableFromCondition(ByRef Cond As Condition) As Variable
        Return Cond.GetBoolExpr.GetUsedVariablesList.Item(0)
    End Function
    
End Class
