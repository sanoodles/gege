Public Class VariableDialogForm
    Inherits System.Windows.Forms.Form

    Public m_IsNew As Boolean
    Public m_Project As Project
    Public m_Scope As String
    Public m_Name As String
    Public m_Documentation As String
    Public m_Type As String
    Public m_Application As enumVariableApplication
    Public m_InitialValue As String = VariablesManager.DefaultValue("BOOL")
    Public m_ViewAsHex As Boolean = False

    Public ReadOnly Property AgainOn() As Boolean
        Get
            Return (CheckBox1.Checked AndAlso _
                    Me.DialogResult = Windows.Forms.DialogResult.OK)
        End Get
    End Property

    'For new variables. Scope is passed as predefined value.
    Public Sub New(ByRef refProject As Project, ByVal Scope As String)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        m_Project = refProject
        m_Scope = Scope
        m_IsNew = True
    End Sub

    'For modifying existing variables
    Public Sub New(ByRef refProject As Project, ByRef rv As Variable)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        m_Project = refProject
        m_Scope = rv.Scope
        m_Name = rv.Name
        m_Documentation = rv.Documentation
        m_Type = rv.dataType
        m_Application = rv.Application
        m_InitialValue = rv.InitialValueToUniversalString
        m_ViewAsHex = rv.ViewAsHex
        m_IsNew = False
    End Sub

    Private Sub ResetFocus()
        TextBox1.Focus()
    End Sub

    Public Sub BlockNameTo(ByVal name As String, _
                           Optional ByVal forceOK As Boolean = True)
        m_Name = name
        TextBox1.Text = m_Name
        TextBox1.Enabled = False
        Button2.Visible = Not (forceOK)
    End Sub

    Private Sub VariableDialogForm_Load(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles MyBase.Load

        'Scope
        Dim i As Integer
        ComboBox3.Items.Add("Project")
        If m_Scope = "Project" Then ComboBox3.SelectedIndex = 0
        For Each Sc As String In m_Project.getScopes
            i = ComboBox3.Items.Add(Sc)
            If m_Scope = Sc Then ComboBox3.SelectedIndex = i
        Next

        'Name
        TextBox1.Text = m_Name

        'Type
        If m_Type <> "" Then
            ComboBox1.Text = m_Type
        Else
            ComboBox1.SelectedIndex = 0
        End If
        'the dataType of a variable can not change
        ComboBox1.Enabled = m_IsNew
        'this constraint could be removed if a way to change the class of an object was found
        'eg: Changing from BooleanVariable to IntegerVariable

        'Application
        'http://msdn.microsoft.com/en-us/library/b1kz44y8%28v=VS.90%29.aspx
        Dim curr_index As Integer
        For Each curr_name As String In [Enum].GetNames(GetType(enumVariableApplication))
            curr_index = ComboBox2.Items.Add(curr_name)
            If m_Application = System.Enum.Parse(GetType(enumVariableApplication), curr_name) Then
                ComboBox2.SelectedIndex = curr_index
            End If
        Next


        If m_ViewAsHex Then
            TextBox4.Text = Hexa.IntToHex(m_InitialValue)
        Else
            TextBox4.Text = m_InitialValue
        End If

        TextBox1.Focus()
        TryEnableOkButton()
        Me.AcceptButton = Button1
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        TryEnableOkButton()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles Button1.Click

        TextBox1.Focus()

        If (IsNumeric(Mid(TextBox1.Text, 1, 1))) Then
            MsgBox("Variable name can't start with a number", _
                   MsgBoxStyle.OkOnly Or MsgBoxStyle.Critical)
        Else
            'Scope
            m_Scope = ComboBox3.Text

            'Name
            m_Name = TextBox1.Text

            'Type
            m_Type = ComboBox1.Text

            'Application
            m_Application = System.Enum.Parse(GetType(enumVariableApplication), ComboBox2.Text)

            'Initial Value
            If (Hexa.EsHex(TextBox4.Text)) Then
                m_InitialValue = Hexa.HexToInt(TextBox4.Text)
                m_ViewAsHex = True
            Else
                m_InitialValue = TextBox4.Text
                m_ViewAsHex = False
            End If

        End If

    End Sub

    Private Sub TextBox1_KeyPress(ByVal sender As Object, _
            ByVal e As System.Windows.Forms.KeyPressEventArgs) _
            Handles TextBox1.KeyPress

        'Limita l'inserimento di caratteri per i nomi di variabili
        Dim NewChar As Char = e.KeyChar
        If Not InvalidVariableChars.IndexOf(e.KeyChar) = -1 Then
            e.Handled = True
        End If
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        TextBox4.Text = VariablesManager.DefaultValue(ComboBox1.SelectedItem)
        TryEnableOkButton()
    End Sub

    Private Sub TryEnableOkButton()

        'Controlla initial value e in nome
        Dim str As String

        If Hexa.EsHex(TextBox4.Text) Then
            str = Hexa.HexToInt(TextBox4.Text)
        Else
            str = TextBox4.Text
        End If

        If VariablesManager.CheckValue(str, ComboBox1.Text) _
                And TextBox1.Text <> "" Then
            Button1.Enabled = True
        Else
            Button1.Enabled = False
        End If

    End Sub

    Private Sub TextBox4_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox4.TextChanged
        TryEnableOkButton()
    End Sub


End Class
