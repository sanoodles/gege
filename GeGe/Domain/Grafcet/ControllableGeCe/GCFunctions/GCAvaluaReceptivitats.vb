Public Class GCAvaluaReceptivitats
    Inherits GCFunctions

    Public Sub New(ByRef c As GCFunctionsController)
        MyBase.New(c)
    End Sub

    Public Function GetAvaluaReceptivitats() As String
        Dim r As String 'result

        r = GetIncludes() + e

        r += "void AvaluaReceptivitats(unsigned char nG)" + e + _
            "{" + e + _
            t + "switch (nG)" + e + _
            t + "{" + e

        For Each Gr As KeyValuePair(Of Integer, Grafcet) In m_gcmap.numToGrafcet
            r += t + t + "case " + Gr.Value.Name + ":" + e

            Dim TrList As New GraphicalTransitionsList
            GetTransitionsByBody(Gr.Value, TrList)
            For Each Tr As GraphicalTransition In TrList
                r += GetAvaluaReceptivat(Gr, Tr)
            Next Tr

            r += t + t + t + "break;" + e + _
                 e
        Next Gr

        r += t + "}" + e + _
             "}" + e

        GetAvaluaReceptivitats = r
    End Function

    Private Function GetAvaluaReceptivat(ByRef Gr As KeyValuePair(Of Integer, Grafcet), _
            ByRef Tr As GraphicalTransition) As String
        Dim r As String = ""
        Dim et As String = "" 'extra tab
        Dim HayCond As Boolean

        HayCond = Tr.Condition.GetString <> "true"

        If HayCond Then
            r += GetCondition(t + t + t, Gr.Key, Tr.ReadCondition)
            et = t
        End If

        r += _
            et + t + t + t + "G[" + Gr.Value.Name + "].Receptivitat.bits.bit" + _
            m_gcmap.G(Gr.Key).transitionToBit(Tr).ToString + " = 1;" + e

        If HayCond Then
            r += t + t + t + "}" + e
        End If

        Return r
    End Function

    


End Class
