'This class is coupled with this knowledge:
' How project's data has to be prepared to be saved on disk
' How project's data has to be derived after being loaded from disk
Public Class ProjectData

    'Saves a project to a file on disk
    Public Function Save(ByRef Pr As Project, _
            ByVal Folder As String, ByVal Name As String) As Boolean
        Return DataIO.SaveToDisk(Pr, Folder + "\" + Name + ".gege")
    End Function

    'Loads a project from a file on disk
    Public Function Open(ByVal Path As String) As Project
        Dim r As New Project
        DataIO.LoadFromDisk(r, Path)

        'In .NET 2.0, circular references are excluded during serialization
        'Here, circular references are manually derived
        For Each Gr As Grafcet In r.numToGrafcet.Values
            Gr.CompleteDeserialization(r)
        Next

        Return r
    End Function

End Class
