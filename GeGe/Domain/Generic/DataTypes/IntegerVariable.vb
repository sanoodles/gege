Imports System.Threading
Imports System.Xml
Public Class IntegerVariable
    Inherits Variable

    Protected m_intValue As Integer = 0
    Protected m_InitValue As Integer = 0
    Protected m_ViewAsHex As Boolean = False

    Public Sub New(ByVal Scope As String, ByVal Name As String, ByVal Documentation As String, ByVal Address As String, _
        ByVal IniValue As String, ByVal ViewAsHex As Boolean, ByVal Application As enumVariableApplication)

        MyBase.New(Scope, Name, Documentation, Address, IniValue)
        m_InitValue = CInt(IniValue)
        m_InitialValue = m_InitValue.ToString
        m_intValue = m_InitValue
        m_dataType = "INT"
        m_ViewAsHex = ViewAsHex
        m_Application = Application

    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Property ViewAsHex() As Boolean
        Get
            ViewAsHex = m_ViewAsHex
        End Get
        Set(ByVal value As Boolean)
            m_ViewAsHex = value
        End Set
    End Property

    Public Overrides Function CheckValue(ByVal Value As String) As Boolean
        Try
            Dim NewValue As Integer = CInt(Value)
            CheckValue = True
        Catch ex As System.Exception
            CheckValue = False
        End Try
    End Function

    Public Overrides Function ReadInitialValue() As Object
        Return m_InitValue
    End Function
    Public Overrides Sub SetInitialValue(ByVal Value As Object)
        m_InitValue = CInt(Value)
        m_InitialValue = Value.ToString
    End Sub

    Public Overrides Function ReadValue() As Object
        If Monitor.TryEnter(Me, 500) Then
            Try
                Return False
            Finally
                Monitor.Exit(Me)
            End Try
        Else
            Return VariablesManager.DefaultValue("INT")
        End If
    End Function

    Public Overrides Sub SetValue(ByVal Value As Object)
        Nop()
    End Sub
    Public Overrides Function ReadActValue() As Object
        If Monitor.TryEnter(Me, 500) Then
            Try
                Return m_intValue
            Finally
                Monitor.Exit(Me)
            End Try
        Else
            Return Nothing
        End If
    End Function
    Public Overrides Sub SetActValue(ByVal Value0 As Object)
        Dim Value As Integer = CInt(Value0)
        m_intValue = Value
        RaiseActValueChanged()
    End Sub
    Public Overloads Sub IncreaseValue()
        IncreaseValue(ReadActValue())
    End Sub
    Public Overrides Sub IncreaseValue(ByVal Value As Object)
        SetActValue(CInt(Value) + 1)
    End Sub
    Public Overloads Sub DecreaseValue()
        DecreaseValue(ReadActValue())
    End Sub
    Public Overrides Sub DecreaseValue(ByVal Value As Object)
        SetActValue(CInt(Value) - 1)
    End Sub
    Public Overrides Sub ResetValue()
        SetActValue(ReadInitialValue())
    End Sub

    Private Sub IncreaseVariable(ByVal sender As Object, ByVal e As EventArgs)
        Me.IncreaseValue()
    End Sub

    Private Sub DecreaseVariable(ByVal sender As Object, ByVal e As EventArgs)
        Me.DecreaseValue()
    End Sub
    Private Sub ResetVariable(ByVal sender As Object, ByVal e As EventArgs)
        Me.ResetValue()
    End Sub

    Public Overrides Function GetMenu() As System.Windows.Forms.MenuItem
        Dim myMenu As New System.Windows.Forms.MenuItem(Me.Name + " (" + _
            Me.ReadActValue().ToString() + ")")
        ' deve essere fatto prima di inserire i sottomenu per prevenire una
        ' eccezione a runtime
        myMenu.Checked = CBool(Me.ReadValue())
        Dim myIncreaseMenu As New System.Windows.Forms.MenuItem("Increase", _
            AddressOf IncreaseVariable)
        Dim myDecreaseMenu As New System.Windows.Forms.MenuItem("Decrease", _
            AddressOf DecreaseVariable)
        Dim myResetMenu As New System.Windows.Forms.MenuItem("Reset", _
            AddressOf ResetVariable)
        myMenu.MenuItems.Add(myIncreaseMenu)
        myMenu.MenuItems.Add(myDecreaseMenu)
        myMenu.MenuItems.Add(myResetMenu)
        Return myMenu
    End Function

End Class
