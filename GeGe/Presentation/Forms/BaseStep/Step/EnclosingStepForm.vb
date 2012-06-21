Public Class EnclosingStepForm
    Inherits System.Windows.Forms.Form

    'view
    Private m_ProjectForm As ProjectForm

    'use case controller
    Public StepDocumentation As String
    Public EnclosureList As List(Of Enclosure)
    Private m_EnclosingStep As GraphicalEnclosingStep
    Private itemToEn As SerializableDictionary(Of Integer, Grafcet)
    Private m_RC As RunControl

    Public Sub New(ByRef St As GraphicalEnclosingStep, ByRef RC As RunControl, _
            ByRef mdiContainer As System.Windows.Forms.Form)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        m_ProjectForm = mdiContainer
        EnclosureList = St.EnclosureList
        m_EnclosingStep = St
        itemToEn = New SerializableDictionary(Of Integer, Grafcet)
        m_RC = RC
    End Sub

    Private Sub EnclosingStepForm_Load(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles MyBase.Load

        'Modifying existent enclosing step
        If Not m_EnclosingStep Is Nothing Then
            Me.Text = m_EnclosingStep.SuperBody.Name + ":" + m_EnclosingStep.Name
            FillEnclosureList()

            'Adding new step
        Else
            DisableEnclosureControls()
        End If

    End Sub

    Private Sub DisableEnclosureControls()
        ListBox1.Enabled = False
        Button4.Enabled = False
        Button5.Enabled = False
    End Sub

    Private Sub FillEnclosureList()
        ListBox1.Items.Clear()
        itemToEn.Clear()

        For Each Gr As Grafcet In EnclosureList
            Dim index As Integer = ListBox1.Items.Add(Gr.Name)
            itemToEn.Add(index, Gr)
        Next
    End Sub

    Private Sub AddEnclosure()
        Dim dialog As New GrafcetDialogForm
        dialog.GNumber = m_EnclosingStep.SuperBody.Project.FirstAvailableGrafcetNumber
        dialog.GName = "G" + dialog.GNumber

        Dim result As System.Windows.Forms.DialogResult
        result = dialog.ShowDialog

        If result = Windows.Forms.DialogResult.OK Then

            If m_EnclosingStep.SuperBody.Project.GetGByName(dialog.GName) Is Nothing Then

                'New Enclosre
                Dim e As Enclosure
                e = m_EnclosingStep.SuperBody.Project.NewEnclosure(dialog.GName, dialog.GNumber)

                'Update domain
                m_EnclosingStep.AddEnclosure(e)

                'Update ListBox1
                Dim index As Integer = ListBox1.Items.Add(dialog.GName)
                itemToEn.Add(index, e)

                'Update Project Form
                m_ProjectForm.FillGrafcetListView()

                'New BodyForm
                Dim g_form As New BodyForm(e)
                g_form.MdiParent = m_ProjectForm

                'Show BodyForm
                g_form.Show()

            Else
                MsgBox("Grafcet already exists!", MsgBoxStyle.Critical)
            End If

        End If
    End Sub

    Private Sub ShowEnclosure()
        m_ProjectForm.ShowBodyFormByGrafcetName(ListBox1.SelectedItem.ToString)
    End Sub


    Private Sub RemoveEnclosure()

    End Sub



    '
    'As in ProjectForm.vb
    '
    Private Sub ShowBodyFormByGrafcetName(ByVal Name As String)
        Dim g_form As BodyForm

        If Not IsGFormOpened(Name) Then
            Dim g As Grafcet
            g = m_EnclosingStep.SuperBody.Project.GetGByName(Name)
            g_form = New BodyForm(g, m_RC)
            g_form.MdiParent = m_ProjectForm
        Else
            g_form = GetBFormByGName(Name)
        End If

        g_form.Show()
        g_form.Focus()
    End Sub

    Private Function IsGFormOpened(ByVal Name As String) As Boolean
        For Each g_form As BodyForm In m_ProjectForm.MdiChildren
            If g_form.GetBodyName = Name Then Return True
        Next
        Return False
    End Function

    Private Function GetBFormByGName(ByVal Name) As BodyForm
        For Each g_form As BodyForm In m_ProjectForm.MdiChildren
            If g_form.GetBodyName = Name Then Return g_form
        Next
        Return Null
    End Function

    Private Function GetOpenedBodyForms() As List(Of BodyForm)
        Dim r As New List(Of BodyForm)

        For Each b_form As BodyForm In m_ProjectForm.MdiChildren
            r.Add(b_form)
        Next

        Return r
    End Function



    '
    'Controls events
    '

    'OK
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.Dispose()
    End Sub

    'Show
    Private Sub ListBox1_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListBox1.MouseDoubleClick
        ShowEnclosure()
    End Sub

    'Add
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        AddEnclosure()
    End Sub

    'Remove
    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        RemoveEnclosure()
    End Sub

End Class
