Option Compare Text
Option Explicit On
Option Strict On

Imports System.Xml.Serialization

<XmlRoot("color")> _
Public Class SerializableColor
    Implements IXmlSerializable
    Private m_Color As Drawing.Color

    <XmlIgnore()> Public Property Value() As Drawing.Color
        Get
            Return m_Color
        End Get
        Set(ByVal v As Drawing.Color)
            m_Color = v
        End Set
    End Property

#Region "IXmlSerializable Members"
    Public Function GetSchema() As System.Xml.Schema.XmlSchema Implements IXmlSerializable.GetSchema
        Return Nothing
    End Function

    Public Sub ReadXml(ByVal reader As System.Xml.XmlReader) Implements IXmlSerializable.ReadXml
        Dim wasEmpty As Boolean = reader.IsEmptyElement
        reader.Read()
        If wasEmpty Then Return
        If reader.MoveToAttribute("value") Then
            m_Color = ColorTranslator.FromHtml(reader.Value)
        End If
        reader.ReadEndElement()
    End Sub

    Public Sub WriteXml(ByVal writer As System.Xml.XmlWriter) Implements IXmlSerializable.WriteXml
        writer.WriteStartElement("color")
        writer.WriteAttributeString("value", ColorTranslator.ToHtml(m_Color))
        writer.WriteEndElement()
    End Sub
#End Region
End Class
