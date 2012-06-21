Imports System.IO
Imports System.Math
Imports System.Xml
Imports System.Threading
Imports System.Collections.Generic
Imports System.Drawing

<Serializable()> _
Public Class GraphicalStep
    Inherits BaseGraphicalStep

    Protected m_OrderBlock As OrderBlock
    Protected m_Initial As Boolean
    Protected m_Final As Boolean

    Public ReadOnly Property Orders() As ArrayList
        Get
            Return m_OrderBlock.OrderList
        End Get
    End Property

    Property Initial() As Boolean
        Get
            Initial = m_Initial
        End Get
        Set(ByVal Value As Boolean)
            m_Initial = Value
        End Set
    End Property


    Public Sub SetFinal(ByVal Value As Boolean)
        m_Final = Value
    End Sub
    Public Function ReadFinal() As Boolean
        ReadFinal = m_Final
    End Function
    Public Sub SetInitial(ByVal Value As Boolean)
        m_Initial = Value
    End Sub
    Public Function ReadInitial() As Boolean
        ReadInitial = m_Initial
    End Function

    Public Overrides Function getActionsByEvent(ByVal actEvent As enumActionEvent) As List(Of Action)
        Return m_OrderBlock.getActionsByEvent(actEvent)
    End Function

    Public Overrides Function getActionsByType(ByVal actType As enumActionType) As List(Of Action)
        Return m_OrderBlock.getActionsByType(actType)
    End Function

    Public Function getForcingOrders() As List(Of GraphicalForcingOrder)
        Return m_OrderBlock.getForcingOrders
    End Function

    'Protected m_pouslist As Pous
    'Questo tipo di fase può contenere azioni
    'Funzioni per la fase
    Public Sub New(ByVal StepNumber As Integer, ByVal StepName As String, _
            ByVal StepDocumentation As String, ByRef RefBody As Body, ByVal Ini As Boolean, _
            ByVal Fin As Boolean, ByVal P As Drawing.Point, ByVal BackCol As Drawing.Color, _
            ByVal SelectionCol As Drawing.Color, ByVal NotSelectionCol As Drawing.Color, _
            ByVal TextCol As Drawing.Color, ByVal Car As Drawing.Font, ByVal ColActive As Drawing.Color, _
            ByVal ColDeactive As Drawing.Color, ByVal ColPreActive As Drawing.Color, _
            ByRef Graph As Drawing.Graphics, ByVal DrState As Boolean, ByVal Dimension As Integer)

        MyBase.New(StepNumber, StepName, StepDocumentation, RefBody, P, _
                BackCol, SelectionCol, NotSelectionCol, TextCol, Car, ColActive, _
                ColDeactive, ColPreActive, Graph, DrState, Dimension)

        m_Initial = Ini
        m_Final = Fin

        'Calcola la posizione dell'actionBlock
        Dim AcBolockPosition As New Drawing.Point
        AcBolockPosition.X = m_Position.X + m_Dimension
        AcBolockPosition.Y = m_Position.Y
        m_OrderBlock = New OrderBlock(m_Dimension, RefBody, AcBolockPosition, BackColor, SelectionColor, NotSelectionColor, TextColor, Carattere, GraphToDraw)
        'Aggiunge la fase ai collegamenti precedenti
        m_OrderBlock.XmlPreviousConnectionsList.Add(Me)
        CalculateArea()

    End Sub

    Public Sub New()
        MyBase.new()
        m_OrderBlock = New OrderBlock
    End Sub



    Public Overrides Function IsAStep() As Boolean
        Return True
    End Function

    Public Overrides Function CreateInstance(ByRef NewBody As Body) As Object
        CreateInstance = New GraphicalStep(m_Number, m_Name, m_documentation, NewBody, m_Initial, m_Final, m_Position, BackColor, SelectionColor, NotSelectionColor, TextColor, Carattere, ColorActive, ColorDeactive, ColorPreActive, Nothing, False, m_Dimension)
    End Function

    Public Overrides Sub DisposeMe()
        m_OrderBlock.DisposeMe()
        Me.Finalize()
    End Sub

    Public Overrides Sub Active()
        Dim StateChanged As Boolean = Not m_Active
        m_Active = True
        m_TimeActivation = Now  'Imposta l'istate di attivazione
        m_PreActive = False
        'Disegna lo stato se richiesto
        If StateChanged And DrawState Then
            DrawStepState(False)
        End If
    End Sub

    Public Overrides Sub Disactive()
        'Termina le azioni non impulsive se era ancora attiva
        m_Active = False
        m_PreActive = False
        'Disegna lo stato se è cambiato e se richiesto
        If DrawState Then
            DrawStepState(False)
        End If
    End Sub

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
            GraphToDraw.DrawLine(penna, m_Position.X + CInt((m_Dimension / 2)), _
                    m_Position.Y, m_Position.X + m_Dimension, m_Position.Y)
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

                'Testo nella fase
                DrawStepText(Col2)

                DrawFinalArrow(Penna)

            End If
        Catch ex As System.Exception
        Finally
            Monitor.Exit(GraphToDraw)
        End Try
    End Sub

    Protected Sub DrawInitialRectangle(ByRef Penna As Drawing.Pen)
        Penna.Width = 1
        GraphToDraw.DrawRectangle(Penna, _
                m_Position.X - CInt((m_Dimension - 8) / 2), _
                m_Position.Y - CInt((m_Dimension - 8) / 2), _
                m_Dimension - 8, _
                m_Dimension - 8)
    End Sub

    Protected Sub DrawOuterRectangle(ByRef Penna As Drawing.Pen)
        Dim R As New Drawing.Rectangle( _
                m_Position.X - CInt(m_Dimension / 2), _
                m_Position.Y - CInt(m_Dimension / 2), _
                m_Dimension, m_Dimension)
        GraphToDraw.DrawRectangle(Penna, R)
    End Sub

    Protected Sub DrawFinalArrow(ByRef Penna As Drawing.Pen)
        'Freccia se finale
        If m_Final Then
            Penna.Width = 1

            GraphToDraw.DrawLine(Penna, _
                    m_Position.X - CInt(m_Dimension / 4), _
                    m_Position.Y - CInt(m_Dimension / 4), _
                    m_Position.X + CInt(m_Dimension / 4), _
                    m_Position.Y - CInt(m_Dimension / 4))

            GraphToDraw.DrawLine(Penna, _
                    m_Position.X + CInt(m_Dimension / 6), _
                    m_Position.Y - CInt(m_Dimension / 4) - CInt(m_Dimension / 8), _
                    m_Position.X + CInt(m_Dimension / 4), _
                    m_Position.Y - CInt(m_Dimension / 4))

            GraphToDraw.DrawLine(Penna, _
                    m_Position.X + CInt(m_Dimension / 6), _
                    m_Position.Y - CInt(m_Dimension / 4) + CInt(m_Dimension / 8), _
                    m_Position.X + CInt(m_Dimension / 4), _
                    m_Position.Y - CInt(m_Dimension / 4))
        End If
    End Sub

    Public Overrides Sub Cancel(ByVal CancellSmallRectangels As Boolean)
        Draw(CancellSmallRectangels, BackColor, BackColor, BackColor, BackColor)
        'Cancella lo stato se richiesto
        If DrawState Then
            DrawStepState(True)
        End If
        'Cancella le azioni
        'Disegna le azioni
        If m_OrderBlock.OrderList.Count > 0 Then
            Dim penna As New Drawing.Pen(BackColor)
            'Line tra fase e prima azione
            GraphToDraw.DrawLine(penna, m_Position.X + CInt((m_Dimension / 2)), m_Position.Y, m_Position.X + m_Dimension, m_Position.Y)
            m_OrderBlock.CancelOrders()
        End If
    End Sub

    Public Overrides Sub SetGraphToDraw(ByRef Graph As Drawing.Graphics)
        GraphToDraw = Graph
        m_OrderBlock.SetGraphToDraw(Graph)
    End Sub

    Public Overrides Sub CalculateArea()
        'Mamorizza l'area
        m_Area.X = m_Position.X - CInt(m_Dimension / 2) - 2
        m_Area.Y = m_Position.Y - CInt(m_Dimension / 2) - CInt(m_Dimension / 5)
        m_Area.Width = m_Dimension + 4
        m_Area.Height = m_Dimension + 4
        If m_OrderBlock.OrderList.Count > 0 Then
            m_Area = Drawing.Rectangle.Union(m_Area, m_OrderBlock.CalculusArea)
        End If
    End Sub

    Public Overrides Sub Move(ByVal dx As Integer, ByVal dy As Integer)
        m_Position.X = m_Position.X + dx
        m_Position.Y = m_Position.Y + dy
        'Muove le azioni
        m_OrderBlock.Move(dx, dy)
        CalculateArea()
    End Sub

    'Funzioni per le azioni
    Public Sub SetActionBlock(ByRef AcBlock As OrderBlock)
        m_OrderBlock = AcBlock
        'Calcola la posizione dell'actionBlock
        Dim AcBolockPosition As New Drawing.Point
        AcBolockPosition.X = m_Position.X + m_Dimension
        AcBolockPosition.Y = m_Position.Y
        m_OrderBlock.Position = AcBolockPosition
        'Svuota le connesisoni dell'ActionBlock
        m_OrderBlock.XmlPreviousConnectionsList.Clear()
        'Aggiunge all'actionBlock il collegamento precedente
        m_OrderBlock.XmlPreviousConnectionsList.Add(Me)
        'Aggiorna l'area della fase
        CalculateArea()
    End Sub

    Public Sub AddAndDrawAction(ByVal actType As enumActionType, _
            ByRef Var As Variable, ByVal vCondition As Condition, _
            ByVal Assignation As String, ByVal actEvent As enumActionEvent)

        m_OrderBlock.AddAndDrawAction(actType, Var, vCondition, Assignation, actEvent, _
                m_Dimension, BackColor, SelectionColor, _
                NotSelectionColor, TextColor, Carattere, GraphToDraw)

        CalculateArea()
    End Sub

    Public Sub AddAndDrawFcOd(ByRef Gr As Grafcet, _
            ByVal Sit As enumSituationType, _
            ByRef Steps As GraphicalStepsList)

        m_OrderBlock.AddAndDrawFcOd(Gr, Sit, Steps, _
                m_Dimension, BackColor, SelectionColor, _
                NotSelectionColor, TextColor, Carattere, GraphToDraw)

        CalculateArea()
    End Sub

    Private Sub CancelActionBlock()
        If m_OrderBlock.OrderList.Count > 0 Then
            Dim penna As New Drawing.Pen(BackColor)
            'Line tra fase e prima azione
            GraphToDraw.DrawLine(penna, m_Position.X + CInt((m_Dimension / 2)), m_Position.Y, m_Position.X + m_Dimension, m_Position.Y)
            m_OrderBlock.CancelOrders()
        End If
    End Sub

    Public Function FindAction(ByVal x As Integer, ByVal y As Integer) As Boolean
        'Trova solo un'azione della fase
        If m_OrderBlock.OrderList.Count > 0 Then
            FindAction = m_OrderBlock.FindOrder(x, y)
        End If
    End Function

    Public Function FindAndSelectAction(ByVal x As Integer, ByVal y As Integer) As Boolean
        'Trova solo un'azione della fase
        If m_OrderBlock.OrderList.Count > 0 Then
            FindAndSelectAction = m_OrderBlock.FindAndSelectOrder(x, y)
        End If
    End Function

    Public Function ReadIfActionSelected(ByVal x As Integer, ByVal y As Integer) As Boolean
        'Trova la prima azione in x,y
        If m_OrderBlock.OrderList.Count > 0 Then
            ReadIfActionSelected = m_OrderBlock.ReadIfOrderSelected(x, y)
        End If
    End Function

    Public Function FindAndSelectActions(ByVal Rect As Drawing.Rectangle) As Boolean
        'Seleziona tutte le azioni della fase che trova
        If m_OrderBlock.OrderList.Count > 0 Then
            FindAndSelectActions = m_OrderBlock.FindAndSelectOrders(Rect)
        End If
    End Function

    Public Function ReadSelectedAction() As GraphicalOrder
        'Legge solo la prima azione selezionata
        If m_OrderBlock.OrderList.Count > 0 Then
            Return m_OrderBlock.ReadSelectedOrder()
        Else
            Return Nothing
        End If
    End Function

    Public Function CountActionsInList() As Integer
        Return m_OrderBlock.OrderList.Count
    End Function

    Public Function ReadListActions() As ArrayList
        Return m_OrderBlock.OrderList
    End Function

    Public Sub RemoveSelectedActions()
        m_OrderBlock.RemoveSelectedOrders()
        CalculateArea()
    End Sub

    Public Function ReadSelectedActionsList() As List(Of GraphicalOrder)
        Return m_OrderBlock.ReadSelectedOrdersList()
    End Function

    Public Function CountSelectedActions() As Integer
        Return m_OrderBlock.CountSelectedOrders()
    End Function

    Public Sub DeSelectActions()
        m_OrderBlock.DeSelectOrders()
    End Sub



End Class
