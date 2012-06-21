Public Class VariableSelectorDialogForm
    Inherits System.Windows.Forms.Form

    Public m_SelectedVar As Object
    Public m_Type As String
    Public m_Value As String

    Private Const PROJECT As Integer = 0
    Private Const GRAFCET As Integer = 1

    Private m_Grafcet As Grafcet

    Private VLists(2) As VariablesList
    Private LViews(2) As ListView

    Public Sub New(ByRef refGrafcet As Grafcet)
        MyBase.New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        m_Grafcet = refGrafcet
        VLists(PROJECT) = refGrafcet.ProjectVariables
        VLists(GRAFCET) = refGrafcet.GrafcetVariables
        LViews(PROJECT) = ListView1
        LViews(GRAFCET) = ListView2
    End Sub

    Private Sub VariablesSelectorDialogForm_Load(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles MyBase.Load

        FillVariablesList(VLists(PROJECT), LViews(PROJECT))

        If Not VLists(GRAFCET) Is Nothing Then
            FillVariablesList(VLists(GRAFCET), LViews(GRAFCET))
        Else
            TabControl1.TabPages.RemoveAt(GRAFCET)
        End If

    End Sub

    Private Sub FillVariableDetails(ByRef lstView As Windows.Forms.ListView, _
            ByVal V As Variable)

        Dim i As Integer = lstView.Items.Add(V.Name).Index
        lstView.Items(i).SubItems.Add(V.dataType)

        If V.ViewAsHex Then
            lstView.Items(i).SubItems.Add(Hexa.IntToHex(V.InitialValueToUniversalString()))
        Else
            lstView.Items(i).SubItems.Add(V.InitialValueToUniversalString())
        End If
    End Sub

    Private Sub FillVariablesList(ByRef Vars As VariablesList, _
           ByRef lstView As ListView)

        lstView.Items.Clear()
        For Each V As Variable In Vars
            FillVariableDetails(lstView, V)
        Next V
    End Sub

    Private Sub SelectVariable(ByVal VarSet As Integer)
        Dim dismiss As Boolean = False

        Dim VList As VariablesList
        Dim LView As ListView

        VList = VLists(VarSet)
        LView = LViews(VarSet)

        If LView.SelectedItems.Count > 0 Then
            Dim TempVar As Variable = VList.FindVariableByName(LView.SelectedItems(0).Text)
            If Not IsNothing(TempVar) Then
                m_SelectedVar = TempVar
                m_Type = TempVar.dataType
                m_Value = TempVar.ReadValue
                dismiss = True
            End If
        End If

        If dismiss Then
            DialogResult = Windows.Forms.DialogResult.OK
            Me.Hide()
        End If

    End Sub

    Private Sub EditVariables(ByVal VarSet As Integer)
        Dim f As New VariablesForm(m_Grafcet.Project, getScope(VarSet))
        f.ShowDialog(Me)
        FillVariablesList(VLists(VarSet), LViews(VarSet))
    End Sub

    Private Function getScope(ByVal VarSet As Integer) As String
        If VarSet = PROJECT Then
            Return "Project"
        Else
            Return m_Grafcet.Name
        End If
    End Function



    '
    'listview double click
    '
    Private Sub ListView1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.DoubleClick
        SelectVariable(PROJECT)
    End Sub
    Private Sub ListView2_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView2.DoubleClick
        SelectVariable(GRAFCET)
    End Sub

    '
    'select click
    '
    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        SelectVariable(PROJECT)
    End Sub
    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        SelectVariable(GRAFCET)
    End Sub

    '
    'edit click
    '
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        EditVariables(PROJECT)
    End Sub
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        EditVariables(GRAFCET)
    End Sub

End Class
