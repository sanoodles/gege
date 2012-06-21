Public Class ActionDialogForm
    Inherits System.Windows.Forms.Form

    Public m_Type As enumActionType
    Public m_Variable As Variable
    Public m_ConditionString As String
    Public m_Assignation As String
    Public m_Event As enumActionEvent

    Private m_VarList As VariablesList

    'Association between listbox's items and variables
    Private itemToVar As SerializableDictionary(Of Integer, Variable)
    'the integer is the zero-based index of the item in the listbox collection

    Public Sub New(ByRef refVarList As VariablesList)
        MyBase.New()
        InitializeComponent()

        m_VarList = refVarList
    End Sub

    Public Sub New(ByRef refVarList As VariablesList, _
            ByRef Ac As GraphicalAction)
        MyBase.New()
        InitializeComponent()

        m_Type = Ac.actType
        m_Variable = Ac.Variable
        m_ConditionString = Ac.Condition.GetString
        m_Assignation = Ac.Assignation
        m_Event = Ac.actEvent

        m_VarList = refVarList
    End Sub

    Private Sub ActionDialogForm_Load(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles MyBase.Load

        'Type in ComboBox1
        Dim curr_index As Integer
        For Each curr_name As String In [Enum].GetNames(GetType(enumActionType))
            curr_index = ComboBox1.Items.Add(curr_name)
            If m_Type = System.Enum.Parse(GetType(enumActionType), curr_name) Then
                ComboBox1.SelectedIndex = curr_index
            End If
        Next



        '
        ' Continuous action
        '

        'Variable in ComboBox3
        itemToVar = New SerializableDictionary(Of Integer, Variable)
        Dim index As Integer
        If m_VarList.Count > 0 Then
            For Each Va As Variable In m_VarList
                If Va.Application = enumVariableApplication.Output Then
                    If Va.dataType = "BOOL" Then
                        index = ComboBox3.Items.Add(Va.Name)
                        itemToVar.Add(index, Va)
                        If m_Variable Is Va Then
                            ComboBox3.SelectedIndex = index
                        End If
                    End If
                End If
            Next
            If m_Variable Is Nothing Then
                If itemToVar.Count > 0 Then m_Variable = itemToVar(0)
            End If
        End If

        'Condition in TextBox2
        TextBox2.Text = m_ConditionString



        '
        ' Stored action
        '

        'Assignation in TextBox1
        TextBox1.Text = m_Assignation

        'Event in ComboBox2
        For Each curr_name As String In [Enum].GetNames(GetType(enumActionEvent))
            curr_index = ComboBox2.Items.Add(curr_name)
            If m_Event = System.Enum.Parse(GetType(enumActionEvent), curr_name) Then
                ComboBox2.SelectedIndex = curr_index
            End If
        Next



        GroupBox2.Left = GroupBox1.Left
        Me.Width = Button2.Left + Button2.Width + 20

    End Sub

    Private Sub HideContinuousControls()
        GroupBox1.Visible = False
    End Sub

    Private Sub HideStoredControls()
        GroupBox2.Visible = False
    End Sub

    Private Sub ListBox1_MouseDoubleClick(ByVal sender As Object, _
            ByVal e As System.Windows.Forms.MouseEventArgs)
        If ComboBox3.SelectedIndex > -1 Then
            DialogResult = Windows.Forms.DialogResult.OK
            Me.Hide()
        End If
    End Sub



    'Type
    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged

        m_Type = getActionType()

        HideContinuousControls()
        HideStoredControls()

        Select Case m_Type

            'Continuous
            Case enumActionType.Continuous
                ShowContinuousControls()

                'Stored
            Case enumActionType.Stored
                ShowStoredControls()

        End Select
    End Sub

    'Variable
    Private Sub ComboBox3_SelectedIndexChanged(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles ComboBox3.SelectedIndexChanged

        If ComboBox3.SelectedIndex > -1 Then
            m_Variable = itemToVar(ComboBox3.SelectedIndex)
        End If
    End Sub

    'Event
    Private Sub ComboBox2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox2.SelectedIndexChanged
        m_Event = getActionEvent()
    End Sub



    Private Sub ShowContinuousControls()
        GroupBox1.Visible = True
    End Sub

    Private Sub ShowStoredControls()
        GroupBox2.Visible = True
    End Sub

    Private Function getActionType() As enumActionType
        Return System.Enum.Parse(GetType(enumActionType), _
                ComboBox1.Items(ComboBox1.SelectedIndex).ToString)
    End Function

    Private Function getActionEvent() As enumActionEvent
        Return System.Enum.Parse(GetType(enumActionEvent), _
                ComboBox2.Items(ComboBox2.SelectedIndex).ToString)
    End Function

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        m_Assignation = TextBox1.Text
    End Sub

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox2.TextChanged
        m_ConditionString = TextBox2.Text
    End Sub
End Class
