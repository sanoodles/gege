Imports System.Threading
Imports System.Xml
Public Class RealVariable
    Inherits Variable

    Protected m_dblValue As Double = 0
    Protected m_InitValue As Double = 0

    Public Sub New(ByVal Scope As String, ByVal Name As String, ByVal Documentation As String, ByVal Address As String, _
        ByVal IniValue As String, ByVal Application As enumVariableApplication)

        MyBase.New(Scope, Name, Documentation, Address, IniValue)
        m_InitValue = CDbl(IniValue)
        m_InitialValue = m_InitValue.ToString
        m_dblValue = m_InitValue
        m_dataType = "REAL"
        m_Application = Application

    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Function CheckValue(ByVal Value As String) As Boolean
        Try
            Dim NewValue As Integer = CInt(Double.Parse(Value, _
                                        System.Globalization.CultureInfo.InvariantCulture.NumberFormat))
            CheckValue = True
        Catch ex As System.Exception
            CheckValue = False
        End Try
    End Function

    Public Overrides Function ReadInitialValue() As Object
        Return m_InitValue
    End Function
    Public Overrides Sub SetInitialValue(ByVal Value As Object)
        If TypeOf (Value) Is String Then Value = Double.Parse(CStr(Value), _
                                        System.Globalization.CultureInfo.InvariantCulture.NumberFormat)
        m_InitValue = CDbl(Value)
        m_InitialValue = InitialValueToUniversalString()
    End Sub

    Public Overrides Function ReadValue() As Object
        If Monitor.TryEnter(Me, 500) Then
            Try
                Return False
            Finally
                Monitor.Exit(Me)
            End Try
        Else
            Return VariablesManager.DefaultValue("REAL")
        End If
    End Function

    Public Overrides Sub SetValue(ByVal Value As Object)
        Nop()
    End Sub
    Public Overrides Function ReadActValue() As Object
        If Monitor.TryEnter(Me, 500) Then
            Try
                Return m_dblValue
            Finally
                Monitor.Exit(Me)
            End Try
        Else
            Return Nothing
        End If
    End Function
    Public Overrides Sub SetActValue(ByVal Value0 As Object)
        Dim Value As Double = CDbl(Value0)
        m_dblValue = Value
        RaiseActValueChanged()
    End Sub
    Public Overloads Sub IncreaseValue()
        IncreaseValue(ReadActValue())
    End Sub
    Public Overrides Sub IncreaseValue(ByVal Value As Object)
        SetActValue(CDbl(Value) + 0.5)
    End Sub
    Public Overloads Sub DecreaseValue()
        DecreaseValue(ReadActValue())
    End Sub
    Public Overrides Sub DecreaseValue(ByVal Value As Object)
        SetActValue(CDbl(Value) - 0.5)
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
