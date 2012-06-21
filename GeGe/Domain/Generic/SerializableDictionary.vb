Option Compare Text
Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml.Serialization
Imports System.Runtime.Serialization

'<XmlRoot("dictionary")> _
<Serializable()> _
Public Class SerializableDictionary(Of TKey, TValue)
    Inherits Dictionary(Of TKey, TValue)
    Implements IXmlSerializable
    Implements ISerializable

    Public Sub New()

    End Sub

    Protected Sub New(ByVal info As SerializationInfo, _
            ByVal context As StreamingContext)
        MyBase.New(info, context)
    End Sub

#Region "IXmlSerializable Members"

    Public Function GetSchema() As System.Xml.Schema.XmlSchema Implements IXmlSerializable.GetSchema
        Return Nothing
    End Function

    Public Sub ReadXml(ByVal reader As System.Xml.XmlReader) Implements IXmlSerializable.ReadXml
        Dim keySerializer As New XmlSerializer(GetType(TKey), S11nExtraTypes.GetTypes)
        Dim valueSerializer As New XmlSerializer(GetType(TValue), S11nExtraTypes.GetTypes)
        Dim wasEmpty As Boolean = reader.IsEmptyElement

        reader.Read()

        If wasEmpty Then
            Return
        End If

        While reader.NodeType <> System.Xml.XmlNodeType.EndElement

            reader.ReadStartElement("item")
            reader.ReadStartElement("key")

            Dim key As TKey = CType(keySerializer.Deserialize(reader), TKey)

            reader.ReadEndElement()
            reader.ReadStartElement("value")

            Dim value As TValue = CType(valueSerializer.Deserialize(reader), TValue)

            reader.ReadEndElement()
            Me.Add(key, value)

            reader.ReadEndElement()
            reader.MoveToContent()
        End While

        reader.ReadEndElement()
    End Sub

    Public Sub WriteXml(ByVal writer As System.Xml.XmlWriter) Implements IXmlSerializable.WriteXml
        Dim keySerializer As New XmlSerializer(GetType(TKey), S11nExtraTypes.GetTypes)
        Dim valueSerializer As New XmlSerializer(GetType(TValue), S11nExtraTypes.GetTypes)

        For Each key As TKey In Me.Keys
            writer.WriteStartElement("item")
            writer.WriteStartElement("key")
            keySerializer.Serialize(writer, key)

            writer.WriteEndElement()
            writer.WriteStartElement("value")

            Dim value As Object = Me(key)

            Try
                valueSerializer.Serialize(writer, value)
            Catch ex As Exception
                ExcBox.Show(ex)
            End Try

            writer.WriteEndElement()
            writer.WriteEndElement()
        Next
    End Sub
#End Region
End Class
