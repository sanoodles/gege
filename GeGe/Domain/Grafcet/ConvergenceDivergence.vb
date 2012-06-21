Imports System.Xml

Public Class ConvDivStruct
    Public m_number As Integer
    Public m_documentation As String
    Public m_position As Drawing.point
    Public m_height As Integer
    Public m_width As Integer
    Public XmlPreviousConnectionsList As ArrayList 'Array di elementi padre connessi
    Property Number() As String
        Get
            Number = m_number.ToString
        End Get
        Set(ByVal Value As String)
            m_number = CInt(Value)
        End Set
    End Property
    Property Documentation() As String
        Get
            Documentation = m_documentation
        End Get
        Set(ByVal Value As String)
            m_documentation = Value
        End Set
    End Property
    Public Function ReadXmlPreviousConnectionsList() As ArrayList
        ReadXmlPreviousConnectionsList = XmlPreviousConnectionsList
    End Function
    Public Sub New()
        XmlPreviousConnectionsList = New ArrayList
    End Sub
    Public Sub xmlExport(ByRef RefXMLProjectWriter As XmlTextWriter, ByVal ElementName As String)
        'Esporta l'elemento
        RefXMLProjectWriter.WriteStartElement(ElementName)
        'Attibuti
        RefXMLProjectWriter.WriteAttributeString("localId", m_number.ToString)

        'Position
        RefXMLProjectWriter.WriteStartElement("position")
        'Attributi di Position
        RefXMLProjectWriter.WriteAttributeString("x", (m_position.X.ToString))
        RefXMLProjectWriter.WriteAttributeString("y", (m_position.Y.ToString))

        RefXMLProjectWriter.WriteEndElement() 'Position

        'ConnectionPointIn
        'Dim ConnectionsList As New ConnectionPointIn(XmlPreviousConnectionsList)
        'ConnectionsList.xmlExport(RefXMLProjectWriter)

        'Esporta documentation
        If m_documentation <> "" Then
            RefXMLProjectWriter.WriteElementString("documentation", m_documentation)  'documentation
        End If

        RefXMLProjectWriter.WriteEndElement() 'elemento
    End Sub
    Public Sub xmlImport(ByRef RefXmlProjectReader As XmlTextReader)
        'Memorizza la profondità del nodo
        Dim NodeDepth As Integer = RefXmlProjectReader.Depth
        'Legge gli attributi
        If RefXmlProjectReader.MoveToAttribute("localId") Then
            m_number = CInt(RefXmlProjectReader.Value)
        End If
        'Si sposta sul nodo successivo
        RefXmlProjectReader.Read()
        'Scorre fino alla fine della struttura
        While RefXmlProjectReader.Depth > NodeDepth
            Select Case RefXmlProjectReader.Name
                Case "position"
                    'La posizione non è utile
                Case "connectionPointIn"
                    'Dim ConnectionsList As New ConnectionPointIn(XmlPreviousConnectionsList)
                    'ConnectionsList.xmlImport(RefXmlProjectReader)
                Case "documentation"
                    'Controlla se è l'inizio dell'elemento
                    If RefXmlProjectReader.NodeType = XmlNodeType.Element Then
                        m_documentation = RefXmlProjectReader.Value
                    End If

            End Select
            'Si sposta sul nodo successivo
            RefXmlProjectReader.Read()

        End While
    End Sub
End Class

<Serializable()> _
Public Class ConvDivStructList
    Inherits ArrayList
    Public Sub New()
        MyBase.New()
    End Sub
    Public Overloads Function Add(ByVal Number As Integer, ByVal Documentation As String, ByVal Position As Drawing.Point, ByVal Connections As ArrayList, ByVal height As Integer, ByVal width As Integer) As ConvDivStruct
        Dim NewConvDivStruct As New ConvDivStruct
        NewConvDivStruct.m_number = Number
        NewConvDivStruct.m_documentation = Documentation
        NewConvDivStruct.m_position = Position
        NewConvDivStruct.m_height = height
        NewConvDivStruct.m_width = width
        NewConvDivStruct.XmlPreviousConnectionsList = Connections
        Me.Add(NewConvDivStruct)
        Add = NewConvDivStruct
    End Function
    Public Sub xmlExportBase(ByRef RefXMLProjectWriter As XmlTextWriter, ByVal ElementName As String)
        For Each C As ConvDivStruct In Me
            C.xmlExport(RefXMLProjectWriter, ElementName)
        Next C
    End Sub
    Public Sub xmlImportBase(ByRef RefXmlProjectReader As XmlTextReader)
        'Crea la struttura
        Dim NewStruct As New ConvDivStruct
        'legge i dati
        NewStruct.xmlImport(RefXmlProjectReader)
        'La aggiunge alla lista
        Me.Add(NewStruct)
    End Sub
    Public Function FindStructByNumber(ByVal n As Integer) As ConvDivStruct
        For Each S As ConvDivStruct In Me
            If S.m_number = n Then
                Return S
            End If
        Next S
        Return Nothing
    End Function
End Class

<Serializable()> _
Public Class XmlSimultaneousConvergences
    Inherits ConvDivStructList
    Public Sub xmlExport(ByRef RefXMLProjectWriter As XmlTextWriter)
        xmlExportBase(RefXMLProjectWriter, "simultaneousConvergence")
    End Sub
    Public Sub xmlImport(ByRef RefXmlProjectReader As XmlTextReader)
        xmlImportBase(RefXmlProjectReader)
    End Sub
End Class

<Serializable()> _
Public Class XmlSimultaneousDivergences
    Inherits ConvDivStructList
    Public Sub xmlExport(ByRef RefXMLProjectWriter As XmlTextWriter)
        xmlExportBase(RefXMLProjectWriter, "simultaneousDivergence")
    End Sub
    Public Sub xmlImport(ByRef RefXmlProjectReader As XmlTextReader)
        xmlImportBase(RefXmlProjectReader)
    End Sub
End Class

<Serializable()> _
Public Class XmlSelectionConvergences
    Inherits ConvDivStructList
    Public Sub xmlExport(ByRef RefXMLProjectWriter As XmlTextWriter)
        xmlExportBase(RefXMLProjectWriter, "selectionConvergence")
    End Sub
    Public Sub xmlImport(ByRef RefXmlProjectReader As XmlTextReader)
        xmlImportBase(RefXmlProjectReader)
    End Sub
End Class

<Serializable()> _
Public Class XmlSelectionDivergences
    Inherits ConvDivStructList
    Public Sub xmlExport(ByRef RefXMLProjectWriter As XmlTextWriter)
        xmlExportBase(RefXMLProjectWriter, "selectionDivergence")
    End Sub
    Public Sub xmlImport(ByRef RefXmlProjectReader As XmlTextReader)
        xmlImportBase(RefXmlProjectReader)
    End Sub
End Class