Public Class MacroStepBody
    Inherits Body

    'domain
    Private m_SuperStep As GraphicalMacroStep

    Public ReadOnly Property SuperStep() As GraphicalMacroStep
        Get
            Return m_SuperStep
        End Get
    End Property

    Public Overrides ReadOnly Property Project() As Project
        Get
            Return m_SuperStep.SuperBody.Project
        End Get
    End Property

    Public Overrides ReadOnly Property ProjectVariables() As VariablesList
        Get
            Return m_SuperStep.SuperBody.ProjectVariables
        End Get
    End Property

    Public Overrides ReadOnly Property GrafcetVariables() As VariablesList
        Get
            Return m_SuperStep.SuperBody.GrafcetVariables
        End Get
    End Property

    'superstep
    Public Sub New(ByRef SuperStep As GraphicalMacroStep)
        MyBase.New(SuperStep.SuperBody.Name + ":" + SuperStep.Name)
        m_SuperStep = SuperStep
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Function CreateInstance()
        Dim NewGr As New MacroStepBody(m_SuperStep)
        Dim NewGraphicalStepsList As GraphicalStepsList = _
                m_SubStepsList.CreateInstance(NewGr)
        Dim NewGraphicalTransitionsList As GraphicalTransitionsList = _
                m_TransitionsList.CreateInstance(NewGr, NewGraphicalStepsList)
        NewGr.GraphicalStepsList = NewGraphicalStepsList
        NewGr.GraphicalTransitionsList = NewGraphicalTransitionsList
        Return NewGr
    End Function

    Public Overrides Function GetScope() As String
        Return m_SuperStep.SuperBody.GetScope
    End Function

    Public Overrides Function GetGrafcet() As Grafcet
        Return m_SuperStep.SuperBody.GetGrafcet
    End Function

End Class
