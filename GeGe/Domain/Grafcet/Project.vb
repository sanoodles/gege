Imports System.Xml.Serialization

'<XmlRootAttribute("Project", IsNullable:=False)> _
<Serializable()> _
Public Class Project

    <NonSerialized()> Private m_Form As ProjectForm
    Private m_Name As String
    Private m_Folder As String
    Private m_numToGrafcet As SerializableDictionary(Of Integer, Grafcet)
    Private m_grafcetToNum As SerializableDictionary(Of Grafcet, Integer)
    Private WithEvents m_Variables As VariablesList

    Public Event ChangeInProjectVariables()
    Public Event ChangeInGrafcetVariables() 'Change in variable of one of project's grafcets

    'name
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(ByVal Value As String)
            m_Name = Value
        End Set
    End Property

    'folder
    Public Property Folder() As String
        Get
            Return m_Folder
        End Get
        Set(ByVal Value As String)
            m_Folder = Value
        End Set
    End Property

    'grafcets 
    Public Property numToGrafcet() As SerializableDictionary(Of Integer, Grafcet)
        Get
            Return m_numToGrafcet
        End Get
        Set(ByVal Value As SerializableDictionary(Of Integer, Grafcet))
            m_numToGrafcet = Value
        End Set
    End Property

    Public Property grafcetToNum() As SerializableDictionary(Of Grafcet, Integer)
        Get
            Return m_grafcetToNum
        End Get
        Set(ByVal Value As SerializableDictionary(Of Grafcet, Integer))
            m_grafcetToNum = Value
        End Set
    End Property

    'variables
    Public Property ProjectVariables() As VariablesList
        Get
            Return m_Variables
        End Get
        Set(ByVal Value As VariablesList)
            m_Variables = Value
        End Set
    End Property

    Public ReadOnly Property AllVariables() As VariablesList
        Get
            Dim r As New VariablesList

            'project variables
            For Each v As Variable In ProjectVariables
                r.Add(v)
            Next

            'grafcet variables
            For Each Gr As Grafcet In m_numToGrafcet.Values
                r.AddRange(Gr.GrafcetVariables)
            Next

            Return r
        End Get
    End Property

    Public Sub New(ByRef frm As ProjectForm, ByVal Name As String, ByVal Folder As String)
        m_Form = frm
        m_Name = Name
        m_Folder = Folder
        m_numToGrafcet = New SerializableDictionary(Of Integer, Grafcet)
        m_grafcetToNum = New SerializableDictionary(Of Grafcet, Integer)
        m_Variables = New VariablesList
    End Sub

    Public Sub New()
        m_numToGrafcet = New SerializableDictionary(Of Integer, Grafcet)
        m_grafcetToNum = New SerializableDictionary(Of Grafcet, Integer)
        m_Variables = New VariablesList
    End Sub

    Public Function NewGrafcet(ByVal Name As String, _
            ByVal Number As Integer) As Grafcet

        Dim g As New Grafcet(Me, Name)
        AddGrafcet(Number, g)

        Return g
    End Function

    Public Function NewEnclosure(ByVal Name As String, _
        ByVal Number As Integer) As Enclosure

        Dim e As New Enclosure(Me, Name)
        AddGrafcet(Number, e)

        Return e
    End Function

    'Public Function NewMacroStepBody(ByRef ParentMacroStep As GraphicalMacroStep, _
    '        ByVal Number As Integer, _
    '        ByRef RefProjectVariables As VariablesList) As Grafcet
    '
    'Dim g As New MacroStepBody(Me, ParentMacroStep, RefProjectVariables)
    '    AddGrafcet(Number, g)
    '
    '        Return g
    '    End Function

    Public Sub AddGrafcet(ByVal Number As Integer, ByRef refGrafcet As Grafcet)
        m_numToGrafcet.Add(Number, refGrafcet)
        m_grafcetToNum.Add(refGrafcet, Number)
        AddHandler refGrafcet.ChangeInGrafcetVariables, AddressOf ChangeInGrafcetVariablesHandler
    End Sub

    Public Function FirstAvailableGrafcetNumber() As Integer
        Dim j As Integer = 0
        Dim NumberAvailable As Boolean

        While j < m_numToGrafcet.Count + 1
            NumberAvailable = True
            For Each n As Integer In m_numToGrafcet.Keys
                If n = j Then
                    NumberAvailable = False
                    Exit For 'Not available
                End If
            Next n

            If NumberAvailable Then
                FirstAvailableGrafcetNumber = j
                Exit While
            End If
            j = j + 1
        End While
    End Function

    Public Function MaxSteps() As Integer
        Dim max As Integer
        max = 0

        For Each g As Grafcet In m_numToGrafcet.Values
            If g.GraphicalStepsList.Count > max Then
                max = g.GraphicalStepsList.Count
            End If
        Next

        Return max
    End Function

    Public Function GetGByName(ByVal Name As String) As Grafcet
        For Each g As Grafcet In m_numToGrafcet.Values
            If g.Name = Name Then Return g
        Next
        Return Nothing
    End Function

    Public Sub SetActiveSteps(ByRef steps As SerializableDictionary(Of Grafcet, GraphicalStepsList))
        For Each s As KeyValuePair(Of Grafcet, GraphicalStepsList) In steps
            s.Key.SetActiveSteps(s.Value)
        Next
    End Sub

    Public Sub SetVarValues(ByRef VarVals As SerializableDictionary(Of Variable, String))
        For Each VarVal As KeyValuePair(Of Variable, String) In VarVals
            VarVal.Key.SetActValue(VarVal.Value)
        Next
    End Sub

    Public Function getVarList(ByVal Scope As String) As VariablesList
        If Scope = "All" Then
            Return AllVariables
        ElseIf Scope = "Project" Then
            Return ProjectVariables
        Else
            Return GetGByName(Scope).GrafcetVariables
        End If
    End Function

    '@return List of scopes of grafcets of the project, without repetition
    Public Function GetScopes() As List(Of String)
        Dim r As New List(Of String)

        For Each Gr As Grafcet In grafcetToNum.Keys
            If Not r.Contains(Gr.GetScope) Then r.Add(Gr.GetScope)
        Next

        Return r
    End Function

    Private Sub ChangeInProjectVariableHandler() Handles _
            m_Variables.NewVariable, _
            m_Variables.VariableModified, _
            m_Variables.VariableDropped, _
            m_Variables.ValueChanged

        RaiseEvent ChangeInProjectVariables()
    End Sub

    Private Sub ChangeInGrafcetVariablesHandler()
        RaiseEvent ChangeInGrafcetVariables()
    End Sub

    Public Sub ClearActiveSteps()
        For Each Gr As Grafcet In m_grafcetToNum.Keys
            Gr.SetActiveSteps(New GraphicalStepsList)
        Next
    End Sub

    Public Function Save(ByVal Folder As String) As Boolean
        Dim pd As New ProjectData
        Return pd.Save(Me, Folder, m_Name)
    End Function

    Public Shared Function Open(ByVal Path As String) As Project
        Dim pd As New ProjectData()
        Return pd.Open(Path)
    End Function

    Public Sub Dispose()
        'Grafcets
        For Each Gr As Grafcet In m_numToGrafcet.Values
            Gr.DisposeMe()
        Next

        'Project variables
        For Each Va As Variable In m_Variables
            Va.DisposeMe()
        Next
    End Sub

End Class
