Imports System.Xml
Imports System.Collections.Generic

<Serializable()> _
Public Class OrderBlock
    'Implements IXMLSerializable

    'presentation
    Protected m_Dimension As Integer
    Protected m_Position As Drawing.Point
    Protected m_Area As Drawing.Rectangle
    Protected m_BackColor As Drawing.Color
    Protected m_SelectionColor As Drawing.Color
    Protected m_NotSelectionColor As Drawing.Color
    Protected m_TextColor As Drawing.Color
    Protected m_Car As Drawing.Font
    Protected ColorActive As Drawing.Color
    Protected ColorPreActive As Drawing.Color
    Protected ColorDeactive As Drawing.Color
    <NonSerialized()> Protected m_GraphToDraw As Drawing.Graphics

    'domain
    Protected m_Body As Body
    Protected m_documentation As String
    Protected m_OrderList As ArrayList

    'Protected m_pouslist As Pous
    Public XmlPreviousConnectionsList As ArrayList

    Public Property OrderList() As ArrayList
        Get
            OrderList = m_OrderList
        End Get
        Set(ByVal Value As ArrayList)
            m_OrderList = Value
        End Set
    End Property

    Public Property Position() As Drawing.Point
        Get
            Position = m_Position

        End Get
        Set(ByVal Value As Drawing.Point)
            m_Position = Value
            'Ricalcola la posizione delle azioni
            Dim OrderPos As Drawing.Point
            Dim i As Integer = 0
            For Each A As GraphicalOrder In m_OrderList
                OrderPos.X = m_Position.X
                OrderPos.Y = m_Position.Y + i * CInt(m_Dimension * (3 / 4))
                A.Position = OrderPos
                i = i + 1
            Next A
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

    Public Sub New(ByVal Dimension As Integer, ByRef RefBody As Body, ByVal Position As Drawing.Point, ByVal BackCol As Drawing.Color, ByVal SelectionCol As Drawing.Color, ByVal NotSelectionCol As Drawing.Color, ByVal TextCol As Drawing.Color, ByVal Car As Drawing.Font, ByRef Graph As Drawing.Graphics)
        m_Dimension = Dimension
        m_Position = Position
        m_Body = RefBody
        m_BackColor = BackCol
        m_SelectionColor = SelectionCol
        m_NotSelectionColor = NotSelectionCol
        m_TextColor = TextCol
        m_Car = Car
        m_GraphToDraw = Graph
        m_OrderList = New ArrayList
        XmlPreviousConnectionsList = New ArrayList
    End Sub

    Public Sub New()
        m_OrderList = New ArrayList
        XmlPreviousConnectionsList = New ArrayList
    End Sub

    Public Sub AddAndDrawAction( _
            ByVal actType As enumActionType, _
            ByRef Var As Variable, _
            ByVal vCondition As Condition, _
            ByVal Assignation As String, _
            ByVal actEvent As enumActionEvent, _
            ByVal Dimension As Integer, _
            ByVal m_BackColor As Drawing.Color, _
            ByVal m_SelectionColor As Drawing.Color, ByVal m_NotSelectionColor As Drawing.Color, _
            ByVal m_TextColor As Drawing.Color, ByVal Car As Drawing.Font, _
            ByVal m_GraphToDraw As Drawing.Graphics)

        'Calcola la posizione dell'azione
        Dim ActionPos As Drawing.Point
        ActionPos.X = m_Position.X
        ActionPos.Y = m_Position.Y + m_OrderList.Count * CInt(m_Dimension * (3 / 4))
        Dim A As New GraphicalAction(actType, Var, vCondition, Assignation, actEvent, Dimension, _
                ActionPos, m_BackColor, _
                m_SelectionColor, m_NotSelectionColor, m_TextColor, Car, m_GraphToDraw)
        m_OrderList.Add(A)
        A.Draw()
    End Sub

    Public Sub AddAndDrawFcOd( _
            ByRef Gr As Grafcet, _
            ByVal Sit As enumSituationType, _
            ByRef Steps As GraphicalStepsList, _
            ByVal Dimension As Integer, _
            ByVal m_BackColor As Drawing.Color, _
            ByVal m_SelectionColor As Drawing.Color, ByVal m_NotSelectionColor As Drawing.Color, _
            ByVal m_TextColor As Drawing.Color, ByVal Car As Drawing.Font, _
            ByVal m_GraphToDraw As Drawing.Graphics)

        'Calcola la posizione dell'azione
        Dim ActionPos As Drawing.Point
        ActionPos.X = m_Position.X
        ActionPos.Y = m_Position.Y + m_OrderList.Count * CInt(m_Dimension * (3 / 4))
        Dim FO As New GraphicalForcingOrder(Gr, Sit, Steps, Dimension, ActionPos, m_BackColor, _
                m_SelectionColor, m_NotSelectionColor, m_TextColor, Car, m_GraphToDraw)
        m_OrderList.Add(FO)
        FO.Draw()
    End Sub

    Public Sub SetGraphToDraw(ByRef Graph As Drawing.Graphics)
        For Each A As GraphicalOrder In m_OrderList
            A.SetGraphToDraw(Graph)
        Next A
    End Sub

    '@return The area of the action block
    Public Function CalculusArea() As Drawing.Rectangle
        CalculusArea.X = m_Position.X
        CalculusArea.Y = m_Position.Y
        For Each A As GraphicalOrder In m_OrderList
            CalculusArea = Drawing.Rectangle.Union(CalculusArea, A.CalculusArea)
        Next A
    End Function

    Public Sub DrawOrders()
        For Each A As GraphicalOrder In m_OrderList
            A.Draw()
        Next A
    End Sub

    Public Sub CancelOrders()
        For Each A As GraphicalOrder In m_OrderList
            A.Cancel()
        Next A
    End Sub

    Public Sub Move(ByVal dx As Integer, ByVal dy As Integer)
        m_Position.X = m_Position.X + dx
        m_Position.Y = m_Position.Y + dy
        'Muove le azioni
        For Each A As GraphicalOrder In m_OrderList
            A.Move(dx, dy)
        Next A
    End Sub

    Public Sub DisposeMe()
        'Distrugge tutte le azioni
        For Each A As GraphicalOrder In m_OrderList
            A.DisposeMe()
        Next A
        Me.Finalize()
    End Sub

    Public Function FindOrder(ByVal x As Integer, ByVal y As Integer) As Boolean
        'Trova solo un'azione del blocco
        For Each A As GraphicalOrder In m_OrderList
            If A.MyAreaIsHere(x, y) Then
                FindOrder = True
                Exit Function
            End If
        Next A
    End Function

    Public Function FindAndSelectOrder(ByVal x As Integer, ByVal y As Integer) As Boolean
        'Seleziona solo la prima azione della fase che trova
        For Each A As GraphicalOrder In m_OrderList
            If A.MyAreaIsHere(x, y) = True Then
                FindAndSelectOrder = True
                A.Selected = True
                Exit Function
            End If
        Next A
    End Function

    Public Function ReadIfOrderSelected(ByVal x As Integer, ByVal y As Integer) As Boolean
        'Trova la prima azione in x,y
        For Each A As GraphicalOrder In m_OrderList
            If A.MyAreaIsHere(x, y) And A.Selected Then
                ReadIfOrderSelected = True
                Exit Function
            End If
        Next A
    End Function

    Public Function FindAndSelectOrders(ByVal Rect As Drawing.Rectangle) As Boolean
        'Seleziona tutte le azioni della fase che trova
        Dim x, y, Passo As Integer
        For Each A As GraphicalOrder In m_OrderList
            Passo = A.Height()
            For x = Rect.X To Rect.X + Rect.Width Step Passo
                For y = Rect.Y To Rect.Y + Rect.Height Step Passo
                    If A.MyAreaIsHere(x, y) = True Then
                        A.Selected = True
                    End If
                Next y
            Next x
            'Ricerca sul bordo verticale destro
            x = Rect.X + Rect.Width
            For y = Rect.Y To Rect.Y + Rect.Height Step Passo
                If A.MyAreaIsHere(x, y) = True Then
                    A.Selected = True
                End If
            Next y
            'Ricerca sul bordo orizzontale sinistro
            y = Rect.Y + Rect.Height
            For x = Rect.X To Rect.X + Rect.Width Step Passo
                If A.MyAreaIsHere(x, y) = True Then
                    A.Selected = True
                End If
            Next x
            'Ricerca sull'angolo inferiore destro 
            x = Rect.X + Rect.Width
            y = Rect.Y + Rect.Height
            If A.MyAreaIsHere(x, y) = True Then
                A.Selected = True
            End If
        Next A
    End Function

    Public Function ReadSelectedOrder() As GraphicalOrder
        'Legge solo la prima azione selezionata
        For Each A As GraphicalOrder In m_OrderList
            If A.Selected Then
                Return A
                Exit Function
            End If
        Next A
        Return Nothing
    End Function

    Public Sub RemoveSelectedOrders()
        Dim i, j As Integer
        j = 0
        For i = 0 To m_OrderList.Count - 1
            If m_OrderList(i - j).Selected = True Then
                m_OrderList(i - j).DisposeMe()
                CType(m_OrderList(i - j), GraphicalOrder).Erased = True
                m_OrderList.RemoveAt(i - j)
                j = j + 1
            End If
        Next i
        'Ricalcola la posizione delle azioni
        Dim OrderPos As Drawing.Point
        i = 0
        For Each A As GraphicalOrder In m_OrderList
            OrderPos.X = m_Position.X
            OrderPos.Y = m_Position.Y + i * CInt(m_Dimension * (3 / 4))
            A.Position = OrderPos
            i = i + 1
        Next A
    End Sub

    Public Function ReadSelectedOrdersList() As List(Of GraphicalOrder)
        ReadSelectedOrdersList = New List(Of GraphicalOrder)()
        For Each A As GraphicalOrder In m_OrderList
            If A.Selected = True Then
                ReadSelectedOrdersList.Add(A)
            End If
        Next A
    End Function

    Public Function CountSelectedOrders() As Integer
        CountSelectedOrders = 0
        For Each A As GraphicalOrder In m_OrderList
            If A.Selected Then
                CountSelectedOrders = CountSelectedOrders + 1
            End If
        Next
    End Function

    Public Sub DeSelectOrders()
        For Each A As GraphicalOrder In m_OrderList
            If A.Selected Then
                A.Selected = False
            End If
        Next
    End Sub

    Public Function getActionsByEvent(ByVal actEvent As enumActionEvent) As List(Of Action)
        Dim r As New List(Of Action)

        For Each Ac As GraphicalOrder In OrderList
            If Ac.GetType.Name = "GraphicalAction" Then
                If CType(Ac, GraphicalAction).actEvent = actEvent Then
                    r.Add(CType(Ac, GraphicalAction).Action)
                End If
            End If
        Next

        Return r
    End Function

    Public Function getActionsByType(ByVal actType As enumActionType) As List(Of Action)
        Dim r As New List(Of Action)

        For Each Ac As GraphicalOrder In OrderList
            If Ac.GetType.Name = "GraphicalAction" Then
                If CType(Ac, GraphicalAction).actType = actType Then
                    r.Add(CType(Ac, GraphicalAction).Action)
                End If
            End If
        Next

        Return r
    End Function

    Public Function getForcingOrders() As List(Of GraphicalForcingOrder)
        Dim r As New List(Of GraphicalForcingOrder)

        For Each FO As GraphicalOrder In OrderList
            If FO.GetType.Name = "GraphicalForcingOrder" Then
                r.Add(FO)
            End If
        Next

        Return r
    End Function

End Class
