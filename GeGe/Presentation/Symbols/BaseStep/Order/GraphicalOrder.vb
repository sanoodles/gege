Imports System.IO
Imports System.Math
Imports System.Xml
Imports System.Threading

Public Enum enumActionType
    Continuous
    Stored
End Enum

Public Enum enumActionEvent
    Activation
    Deactivation
    Custom
End Enum

<Serializable()> _
Public MustInherit Class GraphicalOrder

    'TODO: Find a way to destroy the action when it is removed from the m_ActionList of ActionBlock
    'meanwhile, this attribute is used to avoid action redraw on name change of its m_Var
    Public Erased As Boolean

    'Presentation
    Protected m_Dimension As Integer
    Protected m_Height As Integer
    Protected m_Width As Integer
    Protected m_position As Drawing.Point
    Protected m_Selected As Boolean
    Protected m_Area As Drawing.Rectangle
    Protected m_BackColor As Drawing.Color
    Protected m_SelectionColor As Drawing.Color
    Protected m_NotSelectionColor As Drawing.Color
    Protected m_TextColor As Drawing.Color
    Protected m_TypeChar As Drawing.Font
    <NonSerialized()> Protected m_GraphToDraw As Drawing.Graphics
    Protected Const WIDTH_FACTOR As Integer = 4
    Protected Const HEIGHT_FACTOR As Single = 3 / 4

    Public Sub New(ByVal Dimension As Integer, ByVal P As Drawing.Point, _
                   ByVal BackCol As Drawing.Color, _
                   ByVal SelectionCol As Drawing.Color, _
                   ByVal NotSelectionCol As Drawing.Color, _
                   ByVal TextCol As Drawing.Color, ByVal Car As Drawing.Font, _
                   ByVal Graph As Drawing.Graphics)

        m_Dimension = Dimension
        m_Width = Dimension * WIDTH_FACTOR
        'm_Height = CInt(Dimension * HEIGHT_FACTOR)
        m_Height = CalculateHeight
        m_position = P
        m_BackColor = BackCol
        m_SelectionColor = SelectionCol
        m_NotSelectionColor = NotSelectionCol
        m_TextColor = TextCol
        m_TypeChar = Car
        m_GraphToDraw = Graph
        m_Area = New Drawing.Rectangle
    End Sub

    Public Sub New()

    End Sub

    Public Property Height() As Integer
        Get
            Height = m_Height
        End Get
        Set(ByVal Value As Integer)
            m_Height = Value
        End Set
    End Property

    Public Property Width() As Integer
        Get
            Width = m_Width
        End Get
        Set(ByVal Value As Integer)
            m_Width = Value
        End Set
    End Property

    Public Property Position() As Drawing.Point
        Get
            Position = m_position
        End Get
        Set(ByVal Value As Drawing.Point)
            m_position = Value
        End Set
    End Property

    Public Property Selected() As Boolean
        Get
            Selected = m_Selected
        End Get
        Set(ByVal Value As Boolean)
            m_Selected = Value
        End Set
    End Property

    Protected MustOverride Function CalculateHeight() As Integer



    Public Function ReadArea() As Drawing.Rectangle
        ReadArea = m_Area
    End Function

    Public Sub DisposeMe()
        Me.Finalize()
    End Sub

    Public Sub Move(ByVal dx As Integer, ByVal dy As Integer)
        m_position.X = m_position.X + dx
        m_position.Y = m_position.Y + dy
    End Sub

    Protected MustOverride Sub Draw(ByVal Col1 As Drawing.Color, ByVal Col2 As Drawing.Color, _
            ByVal Col3 As Drawing.Color)

    Public Sub Draw()
        Draw(m_SelectionColor, m_NotSelectionColor, m_TextColor)
    End Sub

    Public Sub Cancel()
        Draw(m_BackColor, m_BackColor, m_BackColor)
    End Sub



    Public Sub SetGraphToDraw(ByRef Graph As Drawing.Graphics)
        m_GraphToDraw = Graph
    End Sub

    Public Function MyAreaIsHere(ByVal x As Integer, ByVal y As Integer) As Boolean
        If x >= m_position.X And x < m_position.X + m_Width And Abs(y - m_position.Y) < m_Height / 2 Then
            MyAreaIsHere = True
        End If
    End Function

    '@return The area of the action
    Public Function CalculusArea() As Drawing.Rectangle
        CalculusArea.X = m_position.X - 1
        CalculusArea.Y = m_position.Y - m_Height / 2 - 1
        CalculusArea.Width = m_Width + 2
        CalculusArea.Height = m_Height + 2
    End Function

End Class

