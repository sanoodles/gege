Public Class ForcingOrderDialogForm
    Inherits System.Windows.Forms.Form

    Public m_Grafcet As Grafcet
    Public m_Situation As enumSituationType
    Public m_SubSteps As GraphicalStepsList

    Private m_Project As Project

    Private Initializing As Boolean

    'Association between listbox's items and grafcets
    Private itemToGrafcet As SerializableDictionary(Of Integer, Grafcet)
    'the integer is the zero-based index of the item in the listbox collection

    'Association between listbox's and combobox's items and steps
    Private itemToExplStep As SerializableDictionary(Of Integer, GraphicalStep)
    Private itemToAvailStep As SerializableDictionary(Of Integer, GraphicalStep)
    'the integer is the zero-based index of the item in the listbox/combobox collection

    Public Sub New(ByRef refProject As Project)
        MyBase.New()
        InitializeComponent()

        m_Grafcet = Nothing
        m_Situation = enumSituationType.Explicit
        m_SubSteps = New GraphicalStepsList

        m_Project = refProject
    End Sub

    Public Sub New(ByRef refProject As Project, ByRef FO As GraphicalForcingOrder)
        MyBase.New()
        InitializeComponent()

        m_Grafcet = FO.Grafcet
        m_Situation = FO.Situation
        m_SubSteps = FO.SubSteps

        m_Project = refProject
    End Sub

    Private Sub ForcingOrderDialogForm_Load(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles MyBase.Load

        Dim index As Integer 'auxiliar

        Initializing = True

        'Grafcet in ComboBox1
        itemToGrafcet = New SerializableDictionary(Of Integer, Grafcet)
        If m_Project.numToGrafcet.Values.Count > 0 Then
            For Each Gr As Grafcet In m_Project.numToGrafcet.Values
                index = ComboBox1.Items.Add(Gr.Name)
                itemToGrafcet.Add(index, Gr)
                If m_Grafcet Is Gr Then
                    ComboBox1.SelectedIndex = index
                End If
            Next
            If m_Grafcet Is Nothing Then
                If itemToGrafcet.Count > 0 Then m_Grafcet = itemToGrafcet(0)
            End If

        End If

        'Situation in ComboBox2
        For Each curr_name As String In [Enum].GetNames(GetType(enumSituationType))
            index = ComboBox2.Items.Add(curr_name)
            If m_Situation = System.Enum.Parse(GetType(enumSituationType), curr_name) Then
                ComboBox2.SelectedIndex = index
            End If
        Next

        'Explicited steps in ListBox1
        FillExplicited()

        'Available steps in ComboBox3
        FillAvailable()


        Initializing = False
    End Sub

    Private Sub HideExplicitControls()
        GroupBox1.Visible = False

        Button1.Top = GroupBox1.Top + 5
        Button2.Top = Button1.Top

        Me.Height = Button1.Top + Button1.Height + 35
    End Sub

    Private Sub ShowExplicitControls()
        GroupBox1.Visible = True

        Button1.Top = GroupBox1.Top + GroupBox1.Height + 5
        Button2.Top = Button1.Top

        Me.Height = Button1.Top + Button1.Height + 35
    End Sub

    Private Function getSituation() As enumSituationType
        Return System.Enum.Parse(GetType(enumSituationType), _
                ComboBox2.Items(ComboBox2.SelectedIndex).ToString)
    End Function


    Private Sub FillExplicited()
        Dim index As Integer

        ListBox1.Items.Clear()
        itemToExplStep = New SerializableDictionary(Of Integer, GraphicalStep)
        For Each St As GraphicalStep In m_SubSteps
            index = ListBox1.Items.Add(St.Name)
            itemToExplStep.Add(index, St)
        Next
    End Sub

    Private Sub FillAvailable()
        Dim index As Integer

        ComboBox3.Items.Clear()
        itemToAvailStep = New SerializableDictionary(Of Integer, GraphicalStep)
        If Not m_Grafcet Is Nothing Then
            For Each St As BaseGraphicalStep In m_Grafcet.GraphicalStepsList
                If St.IsAStep Then
                    If Not m_SubSteps.Contains(St) Then 'exclude already explicited
                        index = ComboBox3.Items.Add(St.Name)
                        itemToAvailStep.Add(index, St)
                    End If
                End If
            Next
        End If
    End Sub

    Private Sub RemoveExplicitedStep()
        Dim St As GraphicalStep

        If Not ListBox1.SelectedItem Is Nothing Then
            'Get step
            St = itemToExplStep(ListBox1.SelectedIndex)

            'Update data
            m_SubSteps.Remove(St)

            'Update GUI
            FillExplicited()
            FillAvailable()
        End If

    End Sub

    Private Sub AddAvailableStep()
        Dim St As GraphicalStep

        If Not ComboBox3.SelectedItem Is Nothing Then
            'Get step
            St = itemToAvailStep(ComboBox3.SelectedIndex)

            'Update data
            m_SubSteps.Add(St)

            'Update GUI
            FillExplicited()
            FillAvailable()
        End If

    End Sub



    '
    'Controls events
    '

    'Grafcet
    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged

        If Initializing Then Return

        'Update m_Grafcet
        m_Grafcet = itemToGrafcet(ComboBox1.SelectedIndex)

        'Update m_SubSteps
        m_SubSteps = New GraphicalStepsList

        'Update GUI
        FillExplicited()
        FillAvailable()

    End Sub

    'Situation
    Private Sub ComboBox2_SelectedIndexChanged(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles ComboBox2.SelectedIndexChanged

        m_Situation = getSituation()

        If m_Situation = enumSituationType.Explicit Then
            ShowExplicitControls()
        Else
            HideExplicitControls()
        End If

    End Sub

    'Explicited steps
    Private Sub Button3_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles Button3.Click
        RemoveExplicitedStep()
    End Sub

    'Available steps
    Private Sub Button4_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles Button4.Click
        AddAvailableStep()
    End Sub

End Class
