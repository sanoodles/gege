'XML Serialization in VB.NET
'http://gauravkhanna.blog.co.in/2008/07/20/xml-serialization-in-vbnet/

'This class is coupled with this knowledge:
' How to save an object to a file on disk
' How to load an object from a file on disk
Public Class DataIO

    Public Shared Function SaveToDisk(Of T)(ByVal instance As T, ByVal filePath As String) As Boolean
        'Dim objSerialize As System.Xml.Serialization.XmlSerializer
        Dim objSerialize As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
        Dim fs As System.IO.FileStream
        Try
            If instance Is Nothing Then
                Throw New ArgumentNullException("instance")
            End If

            'objSerialize = New System.Xml.Serialization.XmlSerializer(instance.GetType(), S11nExtraTypes.GetTypes)
            objSerialize = New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            fs = New System.IO.FileStream(filePath, IO.FileMode.Create)
            Try
                objSerialize.Serialize(fs, instance)
            Catch ex As Exception
                ExcBox.Show(ex)
            End Try
            fs.Close()
            Return True
        Catch ex As Exception

            ExcBox.Show(ex)

        End Try
    End Function

    Public Shared Function LoadFromDisk(Of T)(ByRef instance As T, ByVal filePath As String) As Boolean
        'Dim objSerialize As System.Xml.Serialization.XmlSerializer
        Dim objSerialize As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
        Dim fs As System.IO.FileStream = Nothing

        Try
            If instance Is Nothing Then
                Throw New ArgumentNullException("instance")
            End If

            'Throw File not found Exception
            If System.IO.File.Exists(filePath) = False Then
                Throw New IO.FileNotFoundException("File not found", filePath)
            End If

            'objSerialize = New System.Xml.Serialization.XmlSerializer(instance.GetType, S11nExtraTypes.GetTypes)
            objSerialize = New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            fs = New System.IO.FileStream(filePath, IO.FileMode.OpenOrCreate)
            instance = CType(objSerialize.Deserialize(fs), T)
            fs.Close()
        Catch ex As Exception

            ExcBox.Show(ex)

            fs.Close()
        End Try

    End Function

End Class
