'Abstract variables form.
'VariablesForm and BodyForm conceptually inherit from it, 
'although inheritance is not implemented yet. An association is used instead.
<Serializable()> _
Public Class VariablesGridControl
    'TODO: should not be serialized. find who has it associated, 
    'and mark it as NonSerialized there

    Public Const COL_CURRENT_VALUE As Integer = 5 'Protected

    Public m_SelectedVar As Object
    Public m_Type As String
    Public m_Value As String

    <NonSerialized()> Private Grid1 As DataGridView
    Private WithEvents m_Project As Project
    Private m_Scope As String '"Project", or the name of the grafcet
    Private rowToVar As SerializableDictionary(Of Integer, Variable) 'mapping row index -> variable
    <NonSerialized()> Private VarControlList As List(Of VariableCellControl)

    Private m_Mode As ProjectForm.ProjectMode
    <NonSerialized()> Private m_RC As RunControl

    Public Sub New(ByRef refDataGrid As DataGridView, _
            ByRef refProject As Project, ByVal Scope As String, _
            Optional ByRef RC As RunControl = Nothing)

        MyBase.New()

        Grid1 = refDataGrid
        m_Project = refProject
        m_Scope = Scope
        VarControlList = New List(Of VariableCellControl)

        If RC Is Nothing Then
            SetModeEdit()
        Else
            SetModeRun(RC)
        End If

    End Sub



    Public Sub SetModeEdit()
        m_Mode = ProjectForm.ProjectMode.Edit
        m_RC = Nothing
        SetTableColumns()
        FillTable()
    End Sub

    Public Sub SetModeRun(ByRef RC As RunControl)
        m_Mode = ProjectForm.ProjectMode.Run
        m_RC = RC
        SetTableColumns()
        FillTable()
    End Sub

    Public Function GetMode() As ProjectForm.ProjectMode
        Return m_Mode
    End Function

    Private Sub SetTableColumns()
        Select Case m_Mode

            Case ProjectForm.ProjectMode.Run
                If Not IsVarValuesVisible() Then
                    Grid1.Columns.Add("CurrentValue", "Current Value")
                    Grid1.Columns(COL_CURRENT_VALUE).ReadOnly = True
                End If

            Case ProjectForm.ProjectMode.Edit
                If IsVarValuesVisible() Then
                    Grid1.Columns.RemoveAt(COL_CURRENT_VALUE)
                End If

        End Select
    End Sub

    Public Sub FillTable()
        ClearTable()

        'Always show project variables
        For Each V As Variable In getVarList("Project")
            FillRow(V, "Project")
        Next V

        'Show variables of the corresponding grafcet, if any
        If m_Scope <> "Project" Then
            For Each V As Variable In getVarList(m_Scope)
                FillRow(V, m_Scope)
            Next V
        End If

    End Sub



    Public Sub CreateNewVariable(ByRef refForm As Form)
        Dim vd As New VariableDialogForm(m_Project, m_Scope)

        Do
            Dim ResultDialog As System.Windows.Forms.DialogResult = _
                    vd.ShowDialog(refForm)

            If ResultDialog = Windows.Forms.DialogResult.OK Then
                If Not IsNothing(getVarList("All"). _
                        FindVariableByName(vd.m_Name)) Then
                    MsgBox("Variable " & vd.m_Name & " already exists!", _
                           MsgBoxStyle.Critical)
                Else
                    Dim NewVariable As Variable = _
                            getVarList(vd.m_Scope).CreateAndAddVariable( _
                            vd.m_Scope, _
                            vd.m_Name, _
                            vd.m_Documentation, _
                            "", _
                            vd.m_InitialValue, _
                            vd.m_Type, _
                            vd.m_Application, _
                            vd.m_ViewAsHex)

                    'FillTable() 'Event listeners will invoke FillTable

                End If
            End If
        Loop While vd.AgainOn

    End Sub

    Public Sub ModifyVariable()

        'check: some cell selected
        If Grid1.SelectedCells.Count = 0 Then Return

        Dim Var As Variable = rowToVar(Grid1.SelectedCells(0).RowIndex)

        'check: item is a variable
        If IsNothing(Var) Then Return

        Dim vd As New VariableDialogForm(m_Project, Var)
        Dim ResultDialog As DialogResult = vd.ShowDialog

        'check: dialog was accepted
        If ResultDialog <> Windows.Forms.DialogResult.OK Then Return

        'proceed
        If Var.Name <> vd.m_Name And _
                Not IsNothing(getVarList(vd.m_Scope).FindVariableByName(vd.m_Name)) Then
            MsgBox("Variable " & vd.m_Name & " already exists!", MsgBoxStyle.Critical)
        Else
            Dim prev_scope As String
            prev_scope = Var.Scope

            getVarList(vd.m_Scope).ModifyVariable(Var, _
                    vd.m_Scope, vd.m_Name, vd.m_Documentation, _
                    vd.m_Type, vd.m_InitialValue, _
                    vd.m_Application, vd.m_ViewAsHex)

            If prev_scope <> Var.Scope Then
                getVarList(Var.Scope).AddVariable(Var)
                getVarList(prev_scope).RemoveVariable(Var)
            End If

        End If

        'FillTable() 'Event listeners will invoke FillTable

    End Sub

    Public Sub DeleteVariable()

        If Grid1.SelectedRows.Count = 0 Then Return

        If MsgBox("Please confirm variable deletion", MsgBoxStyle.OkCancel) = _
                MsgBoxResult.Cancel Then Exit Sub

        Dim TempVar As Variable = getVarList(m_Scope).FindVariableByName(Grid1.SelectedRows(0).Cells(0).Value)
        If IsNothing(TempVar) Then Return

        getVarList(m_Scope).RemoveVariable(TempVar)
        'FillTable() 'Event listeners will invoke FillTable

    End Sub

    Public Sub SetVarValue()
        Dim answer As String

        'check: some cell selected
        If Grid1.SelectedCells.Count = 0 Then Return

        Dim Var As Variable = rowToVar(Grid1.SelectedCells(0).RowIndex)

        'check: item is a variable
        If IsNothing(Var) Then Return

        'for boolean variables, asking for setting the value justs toggles between true and false
        If Var.dataType = "BOOL" Then
            Var.SetValue(Not Var.ReadValue)
            answer = Var.ValueToUniversalString

            'for other data types, an inputbox is shown
        Else
            answer = InputBox("Enter new value", "Set variable value", _
                                            Var.ValueToUniversalString)
            'check: dialog was accepted
            If answer = "" Then Return
        End If

        m_RC.SetVarValue(Var, answer)
    End Sub



    Private Sub ClearTable()
        Grid1.Rows.Clear()
        rowToVar = New SerializableDictionary(Of Integer, Variable)
        VarControlList.Clear()
    End Sub

    'associates gridview row index to vars
    Private Sub FillRow(ByRef V As Variable, ByVal Scope As String)
        Dim col As Integer

        Dim i As Integer = Grid1.Rows.Add()

        'Scope
        col = 0
        Grid1.Rows(i).Cells(col).Value = V.Scope

        'Name
        col = col + 1
        Grid1.Rows(i).Cells(col).Value = V.Name
        SetRowToVar(i, V)

        'Type
        col = col + 1
        Grid1.Rows(i).Cells(col).Value = V.dataType

        'Application
        col = col + 1
        Grid1.Rows(i).Cells(col).Value = V.Application.ToString

        'Initial Value
        col = col + 1
        If V.ViewAsHex Then
            Grid1.Rows(i).Cells(col).Value = Hexa.IntToHex(V.InitialValueToUniversalString())
        Else
            Grid1.Rows(i).Cells(col).Value = V.InitialValueToUniversalString()
        End If

        'Current Value
        col = col + 1
        If m_Mode = ProjectForm.ProjectMode.Run Then
            Grid1.Rows(i).Cells(col).Value = V.ValueToUniversalString
            SetVariableControl(i, V)
        End If

    End Sub

    Private Sub SetRowToVar(ByRef row As Integer, ByRef v As Variable)
        rowToVar.Add(row, v)
    End Sub

    Private Sub SetVariableControl(ByVal row As Integer, ByRef v As Variable)
        Dim VCtrl As New VariableCellControl(Grid1, row, COL_CURRENT_VALUE, v)
        VarControlList.Add(VCtrl)
    End Sub

    Private Function IsVarValuesVisible() As Boolean
        Return Grid1.ColumnCount > COL_CURRENT_VALUE
    End Function

    Private Function getVarList(ByVal Scope As String) As VariablesList
        Return m_Project.getVarList(Scope)
    End Function

    Private Sub ChangeInProjectVariablesHandler() Handles _
            m_Project.ChangeInProjectVariables
        'Project variables are always shown
        'TODO: this object should be destroyed together with closing of the VariablesForm
        'in order to avoid handling of m_Project.NewProjectVariable when
        'the grid of the form has been destroyed
        If Grid1.ColumnCount > 0 Then FillTable()
    End Sub

    'TODO: better selection of the event handling, by distinguishing which grafcet the event comes from
    Private Sub ChangeInGrafcetVariablesHandler() Handles _
            m_Project.ChangeInGrafcetVariables
        If m_Scope <> "Project" Then FillTable()
    End Sub

End Class
