'Additional mapping needed by Run Control, which is not already provided by GCMapping
Public Class RCMapping

    Public nGrafcet As Integer

    Public VarToPos As SerializableDictionary(Of Variable, Integer) 'not used so far. created for symmetry with posToVar
    Public PosToVar As SerializableDictionary(Of Integer, Variable)

    Public Sub New(ByRef p As Project)
        nGrafcet = p.numToGrafcet.Count
        MapVars(p)
    End Sub

    Public Sub MapVars(ByRef p As Project)

        Dim i As Integer

        VarToPos = New SerializableDictionary(Of Variable, Integer)
        PosToVar = New SerializableDictionary(Of Integer, Variable)

        i = 0

        'project variables
        For Each Va As Variable In p.ProjectVariables
            VarToPos.Add(Va, i)
            PosToVar.Add(i, Va)
            i = i + 1
        Next

        'grafcets variables
        For Each Gr As Grafcet In p.numToGrafcet.Values
            For Each Va As Variable In Gr.GrafcetVariables
                VarToPos.Add(Va, i)
                PosToVar.Add(i, Va)
                i = i + 1
            Next
        Next

    End Sub

End Class
