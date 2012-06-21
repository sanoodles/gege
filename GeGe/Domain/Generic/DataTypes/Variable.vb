Imports System.Threading
Imports System.Xml
Imports System.Xml.Serialization

Public Enum enumVariableApplication
    Input
    Internal
    Output
End Enum

<Serializable()> _
Public MustInherit Class Variable

    Protected m_Scope As String
    Protected m_Name As String
    Protected m_Documentation As String
    Protected m_Address As String
    Protected m_dataType As String
    Protected m_Application As enumVariableApplication
    Protected m_InitialValue As String
    Protected m_ValueChanged As Boolean

    Public MustOverride Sub SetValue(ByVal Value As Object)
    Public MustOverride Function ReadValue() As Object
    Public MustOverride Function ReadActValue() As Object
    Public MustOverride Sub SetInitialValue(ByVal Value As Object)
    Public MustOverride Function ReadInitialValue() As Object
    Public MustOverride Function CheckValue(ByVal Value As String) As Boolean
    Public MustOverride Sub IncreaseValue(ByVal Value As Object)
    Public MustOverride Sub DecreaseValue(ByVal Value As Object)
    Public MustOverride Sub SetActValue(ByVal Value As Object)
    Public MustOverride Sub ResetValue()
    Public Event NameChanging(ByVal OldName As String) 'Notifica che il nome sta per cambiare
    Public Event NameChanged(ByVal NewName As String)     'Notifica il cambiamento del nome
    Public Event ValueChanging() 'Notifica il cambiamento del valore
    Public Event ValueChanged() 'Notifica il cambiamento del valore
    Public Event ActValueChanged() 'Notifica il cambiamento del valore

    Property Scope() As String
        Get
            Return m_Scope
        End Get
        Set(ByVal Value As String)
            m_Scope = Value
        End Set
    End Property

    Property Name() As String
        Get
            Name = m_Name
        End Get
        Set(ByVal Value As String)
            'Notifica che il nome sta per cambiare
            RaiseEvent NameChanging(m_Name)
            m_Name = Value
            'Notifica il cambiamento del nome
            RaiseEvent NameChanged(m_Name)
        End Set
    End Property

    Property Documentation() As String
        Get
            Documentation = m_Documentation
        End Get
        Set(ByVal Value As String)
            m_Documentation = Value
        End Set
    End Property

    Property Address() As String
        Get
            Address = m_Address
        End Get
        Set(ByVal Value As String)
            m_Address = Value
        End Set
    End Property

    Public Overridable Property ViewAsHex() As Boolean
        Get
            Return False
        End Get
        Set(ByVal value As Boolean)

        End Set
    End Property

    Protected Sub RaiseValueChanging()
        RaiseEvent ValueChanging()
    End Sub

    Sub RaiseValueChanged()
        RaiseEvent ValueChanged()
    End Sub

    Sub RaiseActValueChanged()
        RaiseEvent ActValueChanged()
    End Sub

    Public Property dataType() As String
        Get
            dataType = m_dataType
        End Get
        Set(ByVal value As String)
            m_dataType = value
        End Set
    End Property

    Public Property Application() As enumVariableApplication
        Get
            Return m_Application
        End Get
        Set(ByVal value As enumVariableApplication)
            m_Application = value
        End Set
    End Property

    Public Sub New(ByVal Scope As String, ByVal Name As String, _
            ByVal Documentation As String, ByVal Address As String, ByVal IniValue As String)
        m_Scope = Scope
        m_Name = Name
        m_Documentation = Documentation
        m_Address = Address
        m_InitialValue = IniValue
    End Sub

    Public Sub New()
    End Sub

    Public Sub xmlExport(ByRef RefXMLProjectWriter As XmlTextWriter)

        'Variable
        RefXMLProjectWriter.WriteStartElement("variable")
        'Attributi
        If m_Name <> "" Then
            RefXMLProjectWriter.WriteAttributeString("name", m_Name)
        End If
        If m_Address <> "" Then
            RefXMLProjectWriter.WriteAttributeString("address", m_Address)
        End If
        'type
        RefXMLProjectWriter.WriteStartElement("type")
        RefXMLProjectWriter.WriteStartElement(m_dataType)
        RefXMLProjectWriter.WriteEndElement() 'dataType
        RefXMLProjectWriter.WriteEndElement() 'type
        'initialValue
        RefXMLProjectWriter.WriteStartElement("initialValue")
        'simpleValue
        RefXMLProjectWriter.WriteStartElement("simpleValue")
        'Attributi di simpleValue
        RefXMLProjectWriter.WriteAttributeString("value", InitialValueToUniversalString())
        RefXMLProjectWriter.WriteEndElement() 'simpleValue
        RefXMLProjectWriter.WriteEndElement() 'initialValue

        RefXMLProjectWriter.WriteEndElement() 'variable
    End Sub

    Public Sub DisposeMe()
        Me.Finalize()
    End Sub

    ' Formatta il valore ed il valore iniziale come una stringa indipendente
    ' dalla locale dell'utente
    Public Function ValueToUniversalString() As String
        Return _
            String.Format(System.Globalization.CultureInfo.InvariantCulture.NumberFormat, _
                "{0}", ReadActValue())
    End Function

    Public Function InitialValueToUniversalString() As String
        Return _
            String.Format(System.Globalization.CultureInfo.InvariantCulture.NumberFormat, _
                "{0}", ReadInitialValue())
    End Function

    ' Restituisce il menu usato per l'editor di questa variabile
    Public MustOverride Function GetMenu() As System.Windows.Forms.MenuItem

End Class
