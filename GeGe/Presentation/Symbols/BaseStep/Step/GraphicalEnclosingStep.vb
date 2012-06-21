Imports System.IO
Imports System.Math
Imports System.Xml
Imports System.Threading

Public Class GraphicalEnclosingStep
    Inherits GraphicalStep

    Private m_EnclosureList As List(Of Enclosure)

    Public ReadOnly Property EnclosureList() As List(Of Enclosure)
        Get
            Return m_EnclosureList
        End Get
    End Property

    Public Sub New(ByVal StepNumber As Integer, ByVal StepName As String, _
            ByVal StepDocumentation As String, ByRef RefBody As Body, ByVal Ini As Boolean, _
            ByVal Fin As Boolean, ByVal P As Drawing.Point, ByVal BackCol As Drawing.Color, _
            ByVal SelectionCol As Drawing.Color, ByVal NotSelectionCol As Drawing.Color, _
            ByVal TextCol As Drawing.Color, ByVal Car As Drawing.Font, ByVal ColActive As Drawing.Color, _
            ByVal ColDeactive As Drawing.Color, ByVal ColPreActive As Drawing.Color, _
            ByRef Graph As Drawing.Graphics, ByVal DrState As Boolean, ByVal Dimension As Integer)

        MyBase.New(StepNumber, StepName, StepDocumentation, RefBody, Ini, Fin, P, BackCol, _
                SelectionCol, NotSelectionCol, TextCol, Car, ColActive, ColDeactive, _
                ColPreActive, Graph, DrState, Dimension)

        m_EnclosureList = New List(Of Enclosure)

    End Sub

    Public Sub New()
        MyBase.new()
    End Sub

    Public Overrides Function CreateInstance(ByRef NewBody As Body) As Object
        CreateInstance = New GraphicalStep(m_Number, m_Name, m_documentation, NewBody, m_Initial, m_Final, m_Position, BackColor, SelectionColor, NotSelectionColor, TextColor, Carattere, ColorActive, ColorDeactive, ColorPreActive, Nothing, False, m_Dimension)
    End Function

    Public Sub AddEnclosure(ByRef Gr As Grafcet)
        m_EnclosureList.Add(Gr)
    End Sub

    Public Overloads Overrides Sub Draw(ByVal DoDrawSmallRectangles As Boolean, _
            ByVal Col1 As Drawing.Color, ByVal Col2 As Drawing.Color, _
            ByVal Col3 As Drawing.Color, ByVal Col4 As Drawing.Color)

        MyBase.Draw(DoDrawSmallRectangles, Col1, Col2, Col3, Col4)

        Dim Penna As New Drawing.Pen(Col1)

        If Not m_Selected Then
            Penna.Color = Col2
        End If

        Penna.Width = 1
        DrawChamfer(Penna, m_Initial)

    End Sub

    Protected Sub DrawChamfer(ByRef Penna As Drawing.Pen, _
            ByVal Initial As Boolean)

        Penna.Width = 1

        Dim offset As Integer
        If Initial Then
            offset = CInt(m_Dimension / 2) - 4
        Else
            offset = CInt(m_Dimension / 2)
        End If

        '/
        '
        '
        GraphToDraw.DrawLine(Penna, _
            m_Position.X - offset, _
            m_Position.Y - CInt(m_Dimension / 5), _
            m_Position.X - CInt(m_Dimension / 5), _
            m_Position.Y - offset)

        '  \
        '
        '
        GraphToDraw.DrawLine(Penna, _
            m_Position.X + CInt(m_Dimension / 5), _
            m_Position.Y - offset, _
            m_Position.X + offset, _
            m_Position.Y - CInt(m_Dimension / 5))

        '
        '
        '  /
        GraphToDraw.DrawLine(Penna, _
            m_Position.X + offset, _
            m_Position.Y + CInt(m_Dimension / 5), _
            m_Position.X + CInt(m_Dimension / 5), _
            m_Position.Y + offset)

        '
        '
        '\
        GraphToDraw.DrawLine(Penna, _
            m_Position.X - CInt(m_Dimension / 5), _
            m_Position.Y + offset, _
            m_Position.X - offset, _
            m_Position.Y + CInt(m_Dimension / 5))

    End Sub

End Class
