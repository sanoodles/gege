Imports System.Threading

Public Class GraphicalEnclosedStep
    Inherits GraphicalStep

    Protected m_ActivationLink As Boolean

    Private ActivationLinkFont As Drawing.Font

    Public Sub New(ByVal StepNumber As Integer, ByVal StepName As String, _
        ByVal StepDocumentation As String, ByRef RefBody As Body, ByVal Ini As Boolean, _
        ByVal Fin As Boolean, ByRef ActivationLink As Boolean, ByVal P As Drawing.Point, ByVal BackCol As Drawing.Color, _
        ByVal SelectionCol As Drawing.Color, ByVal NotSelectionCol As Drawing.Color, _
        ByVal TextCol As Drawing.Color, ByVal Car As Drawing.Font, ByVal ColActive As Drawing.Color, _
        ByVal ColDeactive As Drawing.Color, ByVal ColPreActive As Drawing.Color, _
        ByRef Graph As Drawing.Graphics, ByVal DrState As Boolean, ByVal Dimension As Integer)

        MyBase.New(StepNumber, StepName, StepDocumentation, RefBody, Ini, Fin, P, _
                BackCol, SelectionCol, NotSelectionCol, TextCol, Car, ColActive, _
                ColDeactive, ColPreActive, Graph, DrState, Dimension)

        m_ActivationLink = ActivationLink

        ActivationLinkFont = New Drawing.Font("Arial", 16)

    End Sub

    Public Sub New()
        MyBase.new()
    End Sub

    Public Property ActivationLink() As Boolean
        Get
            Return m_ActivationLink
        End Get
        Set(ByVal Value As Boolean)
            m_ActivationLink = Value
        End Set
    End Property

    Public Overloads Overrides Sub Draw(ByVal DrawSmallRectangels As Boolean)
        Draw(DrawSmallRectangels, SelectionColor, NotSelectionColor, TextColor, BackColor)
        'Disegna lo stato se richiesto
        If DrawState Then
            DrawStepState(False)
        End If
        'Disegna le azioni
        If m_OrderBlock.OrderList.Count > 0 Then
            Dim penna As New Drawing.Pen(NotSelectionColor)
            'Line tra fase e prima azione
            GraphToDraw.DrawLine(penna, m_Position.X + CInt((m_Dimension / 2)), m_Position.Y, m_Position.X + m_Dimension, m_Position.Y)
            m_OrderBlock.DrawOrders()
        End If
    End Sub

    Public Overridable Overloads Sub Draw(ByVal DoDrawSmallRectangles As Boolean, _
        ByVal Col1 As Drawing.Color, ByVal Col2 As Drawing.Color, _
        ByVal Col3 As Drawing.Color, ByVal Col4 As Drawing.Color)

        Try
            If Monitor.TryEnter(GraphToDraw, 2000) Then
                Dim Penna As New Drawing.Pen(Col1)

                If m_Selected Then
                    'Penna.Color = SelectionColor
                Else
                    Penna.Color = Col2
                End If
                Penna.Width = 1

                DrawOuterRectangle(Penna)

                'Rectangle interno se iniziale
                If m_Initial Then
                    DrawInitialRectangle(Penna)
                End If

                'Small rectangles
                If DoDrawSmallRectangles Then
                    DrawSmallRectangles(Penna, Col1, Col2, Col3, Col4)
                End If

                'Activation link
                DrawActivationLink(Col2)

                'Testo nella fase
                DrawStepText(Col2)

                DrawFinalArrow(Penna)

            End If
        Catch ex As System.Exception
        Finally
            Monitor.Exit(GraphToDraw)
        End Try
    End Sub

    Private Sub DrawActivationLink(ByRef Col2 As Drawing.Color)

        Dim Br As Drawing.Brush
        Br = New Drawing.SolidBrush(Col2)

        If ActivationLink Then
            GraphToDraw.DrawString("*", _
                    ActivationLinkFont, Br, _
                    m_Position.X - m_Dimension, _
                    m_Position.Y - 0.2 * m_Dimension)
        End If

    End Sub

End Class
