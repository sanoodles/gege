Public Class TransitionDialogForm
    Inherits System.Windows.Forms.Form

    Public m_Name As String
    Public m_Documentation As String
    Public m_Grafcet As Grafcet
    Public m_Condition As Condition

    Private m_ExpressionValidated As Boolean

    Public Sub New(ByVal Name As String, ByRef refGrafcet As Grafcet)
        m_Name = Name
        m_Grafcet = refGrafcet

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Private Sub TransitionDialogForm_Load(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles MyBase.Load
        Me.AcceptButton = Button1
        If Not IsNothing(m_Condition) Then
            TextBox1.Text = m_Condition.GetString
        End If
    End Sub

    Private Sub TryEnableOkButton()
        ErrorProvider1.Clear()
        Dim CondTemp As New BooleanExpression(m_Grafcet)
        Dim validExpr As Boolean = CondTemp.SetExpression(GetBoolExpString())
        If Not validExpr Then
            ErrorProvider1.SetError(TextBox1, CondTemp.ReadError)
        End If
        CondTemp = Nothing
        Button1.Enabled = validExpr
    End Sub

    Private Sub SelectVariable()

        Dim VariableSelectorDialog As New VariableSelectorDialogForm(m_Grafcet)

        Dim ResultDialog As System.Windows.Forms.DialogResult = _
                VariableSelectorDialog.ShowDialog(Me)

        If ResultDialog = Windows.Forms.DialogResult.OK Then
            Dim Init As Integer
            Dim NameLength As Integer
            If Not IsNothing(VariableSelectorDialog.m_SelectedVar) Then
                Init = TextBox1.SelectionStart
                NameLength = VariableSelectorDialog.m_SelectedVar.name.length
                TextBox1.Text = TextBox1.Text.Insert(TextBox1.SelectionStart, VariableSelectorDialog.m_SelectedVar.name)
            End If
            TextBox1.Focus()
            TextBox1.SelectionStart = Init + NameLength
            TextBox1.SelectionLength = 0
        End If

        TryEnableOkButton()
    End Sub

    Private Sub OK()
        m_Condition = ConditionsManager.CreateCondition(TextBox1.Text, m_Grafcet)
    End Sub

    Private Function GetBoolExpString()
        Dim spl() As String = TextBox1.Text.Split("/")
        If spl.Length < 2 Then Return ""

        Return spl(1)
    End Function

    Private Sub SetBoolExpString(ByVal s As String)
        Dim spl() As String = TextBox1.Text.Split("/")

        If spl.Length = 1 Then TextBox1.Text = s
        If spl.Length = 2 Then TextBox1.Text = spl(0) + s
        If spl.Length = 3 Then TextBox1.Text = spl(0) + s + spl(2)
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        TryEnableOkButton()
    End Sub

    Private Sub TextBox2_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        'Limita l'inserimento di caratteri per i nomi di variabili
        Dim NewChar As Char = e.KeyChar
        If Not InvalidVariableChars.IndexOf(e.KeyChar) = -1 Then
            e.Handled = True
        End If
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        SelectVariable()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        OK()
    End Sub

End Class
