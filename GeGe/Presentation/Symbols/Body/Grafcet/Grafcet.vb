<Serializable()> _
Public Class Grafcet
    Inherits Body

    'domain
    Protected m_Project As Project
    Protected WithEvents m_GrafcetVariables As VariablesList

    Public Event ChangeInGrafcetVariables()

    Public Overrides ReadOnly Property Project() As Project
        Get
            Return m_Project
        End Get
    End Property

    Public Overrides ReadOnly Property ProjectVariables() As VariablesList
        Get
            Return m_Project.ProjectVariables
        End Get
    End Property

    Public Overrides ReadOnly Property GrafcetVariables() As VariablesList
        Get
            Return m_GrafcetVariables
        End Get
    End Property

    'project, name
    Public Sub New(ByRef refProject As Project, ByVal Name As String)
        MyBase.New(Name)
        m_Project = refProject
        m_GrafcetVariables = New VariablesList
    End Sub

    Public Sub New()
        MyBase.New()
        m_Project = New Project
        m_GrafcetVariables = New VariablesList
    End Sub

    Public Overrides Function CreateInstance()
        Dim NewGr As New Grafcet(m_Project, m_Name)
        Dim NewGraphicalStepsList As GraphicalStepsList = _
                m_SubStepsList.CreateInstance(NewGr)
        Dim NewGraphicalTransitionsList As GraphicalTransitionsList = _
                m_TransitionsList.CreateInstance(NewGr, NewGraphicalStepsList)
        NewGr.GraphicalStepsList = NewGraphicalStepsList
        NewGr.GraphicalTransitionsList = NewGraphicalTransitionsList
        Return NewGr
    End Function


    Public Overrides Function GetScope() As String
        Return m_Name
    End Function

    Public Overrides Function GetGrafcet() As Grafcet
        Return Me
    End Function

    Protected Sub ChangeInProjectVariableHandler() Handles _
            m_GrafcetVariables.NewVariable, _
            m_GrafcetVariables.VariableModified, _
            m_GrafcetVariables.VariableDropped, _
            m_GrafcetVariables.ValueChanged

        RaiseEvent ChangeInGrafcetVariables()
    End Sub

    Public Overloads Sub CompleteDeserialization(ByRef Pr As Project)
        MyBase.CompleteDeserialization()
        m_Project = Pr
    End Sub


End Class
