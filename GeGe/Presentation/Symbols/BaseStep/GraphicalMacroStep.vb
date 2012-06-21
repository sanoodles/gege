Imports System.IO
Imports System.Math
Imports System.Xml
Imports System.Threading

Public Class GraphicalMacroStep
    Inherits BaseGraphicalStep

    'La macrofase non deve essere attivata, 
    'se preattivata si attiva da sola quando il proprio body ha raggiunto una fase finale...
    '....quindi si Depreattiva e si attiva. Cosi nelle scansioni successive la macrofase
    'La macrofase, quando viene chiamato il proprio ciclo di scansione per il body lo 
    'effetua solo se è preattiva

    Protected m_SubBody As MacroStepBody

    Public Sub New( _
            ByVal StepNumber As Integer, ByVal StepName As String, _
            ByVal StepDocumentation As String, ByRef RefBody As Body, _
            ByVal P As Drawing.Point, ByVal ColSfondo As Drawing.Color, _
            ByVal SelectionCol As Drawing.Color, ByVal NotSelectionCol As Drawing.Color, _
            ByVal TextColor As Drawing.Color, ByVal Car As Drawing.Font, _
            ByVal ColActive As Drawing.Color, ByVal ColDeactive As Drawing.Color, _
            ByVal ColPreActive As Drawing.Color, ByVal Graph As Drawing.Graphics, _
            ByVal DrState As Boolean, ByVal Dimen As Integer)

        MyBase.New(StepNumber, StepName, StepDocumentation, RefBody, P, ColSfondo, _
                SelectionCol, NotSelectionCol, TextColor, Car, ColActive, ColDeactive, _
                ColPreActive, Graph, DrState, Dimen)

        m_SubBody = New MacroStepBody(Me)

        CalculateArea()
    End Sub

    Public Sub New()
        MyBase.new()
    End Sub

    Public Overrides Function CreateInstance(ByRef NewBody As Body) As Object
        CreateInstance = New GraphicalMacroStep(m_Number, m_Name, m_documentation, NewBody, m_Position, BackColor, SelectionColor, NotSelectionColor, TextColor, Carattere, ColorActive, ColorDeactive, ColorPreActive, Nothing, False, m_Dimension)
        CreateInstance.SetBody(m_SubBody.CreateInstance)
    End Function

    Public Overrides Sub DisposeMe()
        m_SubBody.DisposeMe()
        Me.Finalize()
    End Sub

    Public Overrides Sub Cancel(ByVal CancellSmallRectangels As Boolean)
        Draw(CancellSmallRectangels, BackColor, BackColor, BackColor, BackColor)
        'Cancella lo stato se richiesto
        If DrawState Then
            DrawStepState(True)
        End If
    End Sub

    Public Overrides Sub Move(ByVal dx As Integer, ByVal dy As Integer)
        m_Position.X = m_Position.X + dx
        m_Position.Y = m_Position.Y + dy
        CalculateArea()
    End Sub

    Public Overrides Sub SetGraphToDraw(ByRef Graph As Drawing.Graphics)
        GraphToDraw = Graph
    End Sub

    Public Overrides Sub Active()
        'La macrofase non deve essere attivata, se preattivata si attiva da sola quando il proprio body ha raggiunto una fase finale...
        '....quindi si Depreattiva e si attiva. Cosi nelle scansioni successive la macrofase
        'La macrofase, quando viene chiamato il proprio ciclo di scansione per il body lo effetua solo se è preattiva
    End Sub

    Public Overrides Sub Disactive()
        'Poichè non può essere attivata (si attiva da sola), se viene disattivata....
        '....forzatamente resetta l'evoluzione del proprio body se era attiva o preattiva
        If m_Active Or m_PreActive Then
            'm_Body.Reset()
            m_PreActive = False
            m_Active = False
            'Resetta lo stato del body
            ResetBodyState()
            'Disegna lo stato se richiesto
            If DrawState Then
                DrawStepState(False)
            End If
        End If
    End Sub

    Public Overloads Overrides Sub Draw(ByVal DrawSmallRectangles As Boolean)
        Draw(DrawSmallRectangles, SelectionColor, NotSelectionColor, TextColor, BackColor)
        'Disegna lo stato se richiesto
        If DrawState Then
            DrawStepState(False)
        End If
    End Sub

    Public Overloads Sub Draw(ByVal DoDrawSmallRectangles As Boolean, _
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
                Penna.Width = 3
                Dim R As New Drawing.Rectangle( _
                        m_Position.X - CInt(m_Dimension / 2), _
                        m_Position.Y - CInt(m_Dimension / 2), _
                        m_Dimension, m_Dimension)
                GraphToDraw.DrawRectangle(Penna, R)
                'Rettangolo vuoto al centro
                'GraphToDraw.FillRectangle(Br, R.X + 1, R.Y + 1, R.Width - 1, R.Height - 1)

                'Small rectangles
                If DoDrawSmallRectangles Then
                    DrawSmallRectangles(Penna, Col1, Col2, Col3, Col4)
                End If

                'Testo nella fase
                DrawStepText(Col2)

            End If
        Catch ex As System.Exception
        Finally
            Monitor.Exit(GraphToDraw)
        End Try

    End Sub

    Public Overrides Sub CalculateArea()
        'Mamorizza l'area
        m_Area.X = m_Position.X - CInt(m_Dimension / 2) - 2
        m_Area.Y = m_Position.Y - CInt(m_Dimension / 2) - CInt(m_Dimension / 5)
        m_Area.Width = m_Dimension + 4 '+ m_Dimension * Sign(m_ListActions.Count) + m_ListActions.Count * m_Dimension * 3
        m_Area.Height = m_Dimension + 2 * CInt(m_Dimension / 5)
    End Sub

    Public Function ReadSubBody() As MacroStepBody
        Return m_SubBody
    End Function

    Public Sub SetSubBody(ByRef Value As MacroStepBody)
        m_SubBody = Value
    End Sub

    Public Sub ResetBodyState()
        'Resetta lo stato del suo body
        'm_Body.Reset()
    End Sub



End Class
