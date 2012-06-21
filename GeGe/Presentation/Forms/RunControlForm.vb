Public Class RunControlForm

    Private Const DEBUG As Boolean = True

    Private m_ProjectForm As ProjectForm
    Private m_RunControl As RunControl

    Public Sub New(ByRef refProjectForm As ProjectForm, ByRef RC As RunControl)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        m_ProjectForm = refProjectForm
        m_RunControl = RC
        m_RunControl.m_Form = Me 'for RunControlIO delegate invocation
        'TODO: find a method for RunControlIO to call delegates without needing a form
    End Sub

    Private Sub RunControlForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Me.Width = 142
        'Me.Height = 128
    End Sub

    Private Sub Warn(ByVal msg As String)
        If DEBUG Then MsgBox(msg)
    End Sub

    Private Sub Form_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.FormClosing
        'See ProjectForm.RunProject for explanations
        If m_RunControl.RunState = enumRunState.Started Then StopRun()
        m_ProjectForm.AlertRCFormClosing()
    End Sub



    '
    '@section UseCaseController
    '



    'macros

    Private Sub StartRun()
        If m_RunControl.RunState = enumRunState.Stopped Then
            Me.Cursor = Cursors.WaitCursor
            m_ProjectForm.AlertStartRun()
            Generate()
            Compile()
            Ignition()
            Conectar()
            RequestState()
            Me.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub Steep()
        If m_RunControl.RunState = enumRunState.Started Then
            m_RunControl.Steep()
            RequestState()
        End If
    End Sub

    Private Sub StopRun()
        If m_RunControl.RunState = enumRunState.Started Then
            Eend()
            Desconectar()
            m_ProjectForm.AlertEndRun()
        End If
    End Sub



    'primitives

    Private Sub Generate()
        If Not m_RunControl.Generate() Then
            MsgBox("Code generation failed")
        End If
    End Sub

    Private Sub Compile()
        If Not m_RunControl.Compile() Then
            MsgBox("Compilation failed")
        End If
    End Sub

    Private Sub Ignition()
        If Not m_RunControl.Ignition() Then
            MsgBox("Ignition failed")
        End If
    End Sub

    Private Sub Conectar()
        If Not m_RunControl.Conectar() Then
            Warn("Connection failed")
        End If
    End Sub

    Private Sub RequestActiveSteps()
        m_RunControl.RequestActiveSteps()
    End Sub

    Private Sub RequestVariables()
        m_RunControl.RequestVariables()
    End Sub

    Private Sub RequestState()
        RequestActiveSteps()
        RequestVariables()
    End Sub

    Private Sub Eend()
        m_RunControl.Eend()
    End Sub

    Private Sub Desconectar()
        m_RunControl.Desconectar()
    End Sub



    '
    '@section: View
    '



    'normal controls

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        StartRun()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Steep()
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        StopRun()
    End Sub



    'debug controls

    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click
        Generate()
    End Sub

    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        Compile()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Ignition()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Conectar()
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Desconectar()
    End Sub

    Private Sub Button5_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        RequestActiveSteps()
    End Sub

    Private Sub Button14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button14.Click
        RequestVariables()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Eend()
    End Sub

    Private Sub Button12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button12.Click
        Desconectar()
    End Sub

End Class