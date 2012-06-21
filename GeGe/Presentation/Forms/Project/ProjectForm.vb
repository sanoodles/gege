Imports System.Net.Sockets
Imports System.Net

Public Class ProjectForm

    Public Enum ProjectMode
        Edit = 1
        Run = 2
    End Enum

    Private m_Project As Project
    Private m_DefaultPFolder As String

    Private m_Mode As ProjectMode
    Private m_RC As RunControl
    Private m_ProjectVariablesForm As VariablesForm

    Public ReadOnly Property Mode() As ProjectForm.ProjectMode
        Get
            Return m_Mode
        End Get
    End Property

    Public Sub New()
        m_DefaultPFolder = Application.StartupPath
        m_Mode = ProjectMode.Edit
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Private Sub ProjectForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LoadMenuImages()
        MenuItemsSetEnable()
    End Sub

    Private Sub MenuItemsSetEnable()
        Dim OP As Boolean = Not m_Project Is Nothing 'Opened Project

        'A project is opened <-> Enabled
        SaveToolStripMenuItem.Enabled = OP
        NewGrafcetToolStripMenuItem.Enabled = OP
        VariablesToolStripMenuItem.Enabled = OP
        GeCeToolStripMenuItem.Enabled = OP
        SimulationToolStripMenuItem.Enabled = OP
        CloseToolStripMenuItem.Enabled = OP

        'A project is opened <-> Disabled
        NewToolStripMenuItem.Enabled = Not OP
        OpenToolStripMenuItem.Enabled = Not OP
    End Sub

    Public Sub ClearActiveSteps()
        m_Project.ClearActiveSteps()
    End Sub

    Public Sub Deselect_All()
        For Each Gr As Grafcet In m_Project.grafcetToNum.Keys
            Gr.DeSelectAll()
        Next
    End Sub

    Private Sub LoadMenuImages()
        GeCeToolStripMenuItem.Image = ImageList1.Images(0) 'GeCe
    End Sub

    'Private Sub ProjectForm_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles Me.KeyPress
    '    If e.KeyChar = Microsoft.VisualBasic.ChrW(Keys.Escape) Then
    'Dim a As Form = Me.ActiveMdiChild
    '        If TypeOf (a) Is BodyForm Then
    '            CType(a, BodyForm).ResetCurrentOperation()
    '        End If
    '    End If
    'End Sub

    Private Sub NewProject()
        If m_Project Is Nothing Then
            Dim dialog As New ProjectDialogForm
            dialog.PName = "New Project"
            dialog.PFolder = m_DefaultPFolder

            Dim result As System.Windows.Forms.DialogResult
            result = dialog.ShowDialog

            If result = Windows.Forms.DialogResult.OK Then
                m_Project = New Project(Me, dialog.PName, dialog.PFolder)
                Me.Text = dialog.PName
            End If

            dialog.Dispose()

            MenuItemsSetEnable()
        Else
            MsgBox("A project is already opened")
        End If
    End Sub

    Private Sub Open()
        If m_Project Is Nothing Then
            Me.Cursor = Cursors.WaitCursor

            Dim d As New OpenFileDialog
            d.Filter = "GeGé file (*.gege)|*.gege|All files (*.*)|*.*"
            d.FileName = ""
            d.DefaultExt = ".gege"
            d.CheckFileExists = True
            d.CheckPathExists = True

            Dim rd As DialogResult = d.ShowDialog()

            If rd = Windows.Forms.DialogResult.OK Then
                m_Project = Project.Open(d.FileName)
                Me.Text = m_Project.Name
                FillGrafcetListView()
            End If

            MenuItemsSetEnable()

            Me.Cursor = Cursors.Default
        Else
            MsgBox("A project is already opened")
        End If
    End Sub

    Private Sub Save()
        Me.Cursor = Cursors.WaitCursor
        m_Project.Save(m_DefaultPFolder)
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub AddGrafcet()
        Dim dialog As New GrafcetDialogForm
        dialog.GNumber = m_Project.FirstAvailableGrafcetNumber
        dialog.GName = "G" + dialog.GNumber

        Dim result As System.Windows.Forms.DialogResult
        result = dialog.ShowDialog

        If result = Windows.Forms.DialogResult.OK Then

            'New Grafcet
            Dim g As Grafcet
            g = m_Project.NewGrafcet(dialog.GName, dialog.GNumber)

            'Update TreeView
            FillGrafcetListView()

            'New BodyForm
            Dim g_form As New BodyForm(g)
            g_form.MdiParent = Me

            'Show BodyForm
            g_form.Show()
        End If
    End Sub

    Public Sub FillGrafcetListView()
        TreeView1.Nodes.Clear()

        For Each Gr As KeyValuePair(Of Integer, Grafcet) In m_Project.numToGrafcet
            TreeView1.Nodes.Add(Gr.Key, Gr.Value.Name)
        Next

    End Sub

    Private Sub EditProjectVariables()
        If Not IsPVarsFormOpened() Then
            m_ProjectVariablesForm = New VariablesForm(m_Project, "Project", m_RC)
            m_ProjectVariablesForm.Show(Me)
        End If
        m_ProjectVariablesForm.Focus()
    End Sub

    Private Sub GeCe()
        My.Computer.FileSystem.CreateDirectory("GeCe")
        Dim path As String = m_Project.Folder + "\GeCe"
        Dim gece As New GeCeControl(m_Project, path)
        gece.Go()
    End Sub

    Private Sub RunProject()
        'ProjectForm opens RunControlForm
        '   Then, ProjectForm sets its own mode to Run
        'RunControlForm will alert ProjectForm of its closing
        '   Then, ProjectForm will set its own mode to Edit
        m_RC = New RunControl(m_Project)
        SetModeRun(m_RC)
        Dim f As New RunControlForm(Me, m_RC)
        f.Show(Me)
    End Sub

    Private Sub CloseProject()
        If Not m_Project Is Nothing Then
            For Each Fr As Form In GetOpenedBodyForms()
                Fr.Dispose()
            Next
            m_Project.Dispose()
            m_Project = Nothing
            TreeView1.Nodes.Clear()
            Me.Text = "ProjectForm"
            MenuItemsSetEnable()
        Else
            MsgBox("No project is opened")
        End If
    End Sub




    Public Sub AlertStartRun()
        Me.Show()
        Deselect_All()
    End Sub

    Public Sub AlertEndRun()
        ClearActiveSteps()
    End Sub

    Public Sub AlertRCFormClosing()
        SetModeEdit()
    End Sub

    'sets all forms to edit mode
    Private Sub SetModeEdit()
        m_Mode = Mode

        '1. project variables form
        '2. grafcet1..grafcetN form
        If IsPVarsFormOpened() Then m_ProjectVariablesForm.SetModeEdit()

        For Each GF As BodyForm In GetOpenedBodyForms()
            GF.SetModeEdit()
        Next

    End Sub

    'sets all forms to run mode
    Private Sub SetModeRun(ByRef RC As RunControl)
        m_Mode = ProjectMode.Run

        '1. project variables form
        '2. grafcet1..grafcetN form
        If IsPVarsFormOpened() Then m_ProjectVariablesForm.SetModeRun(RC)

        For Each GF As BodyForm In GetOpenedBodyForms()
            GF.SetModeRun(RC)
        Next

    End Sub

    Private Function IsPVarsFormOpened() As Boolean
        If m_ProjectVariablesForm Is Nothing Then Return False
        If m_ProjectVariablesForm.IsDisposed Then Return False
        Return True
    End Function

    Private Sub TreeView1_NodeMouseClick(ByVal sender As Object, _
            ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles TreeView1.NodeMouseClick
        ShowBodyFormByGrafcetName(e.Node.Text)
    End Sub

    Public Sub ShowBodyFormByGrafcetName(ByVal Name As String)
        Dim g_form As BodyForm

        If Not IsGFormOpened(Name) Then
            Dim g As Grafcet
            g = m_Project.GetGByName(Name)
            g_form = New BodyForm(g, m_RC)
            g_form.MdiParent = Me
        Else
            g_form = GetBFormByGName(Name)
        End If

        g_form.Show()
        g_form.Focus()
    End Sub

    Private Function IsGFormOpened(ByVal Name As String) As Boolean
        For Each g_form As BodyForm In GetOpenedBodyForms()
            If g_form.GetBodyName = Name Then Return True
        Next
        Return False
    End Function

    Private Function GetBFormByGName(ByVal Name) As BodyForm
        For Each g_form As BodyForm In GetOpenedBodyForms()
            If g_form.GetBodyName = Name Then Return g_form
        Next
        Return Null
    End Function

    Private Function GetOpenedBodyForms() As List(Of BodyForm)
        Dim r As New List(Of BodyForm)

        For Each Fo As Form In MdiChildren
            If Fo.GetType.Name = "BodyForm" Then
                r.Add(Fo)
            End If
        Next

        Return r
    End Function

    Private Sub NewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewToolStripMenuItem.Click
        NewProject()
    End Sub

    Private Sub AddGrafcetToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewGrafcetToolStripMenuItem.Click
        AddGrafcet()
    End Sub

    Private Sub ProjectVariablesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VariablesToolStripMenuItem.Click
        EditProjectVariables()
    End Sub

    Private Sub GeCeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GeCeToolStripMenuItem.Click
        GeCe()
    End Sub

    Private Sub CascadeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CascadeToolStripMenuItem.Click
        Me.LayoutMdi(System.Windows.Forms.MdiLayout.Cascade)
    End Sub

    Private Sub SimulationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SimulationToolStripMenuItem.Click
        RunProject()
    End Sub

    Private Sub SaveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveToolStripMenuItem.Click
        Save()
    End Sub

    Private Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        Open()
    End Sub

    Private Sub CloseToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseToolStripMenuItem.Click
        CloseProject()
    End Sub
End Class
