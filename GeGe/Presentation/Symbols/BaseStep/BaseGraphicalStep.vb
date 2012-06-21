Imports System.Math
Imports System.IO
Imports System.Xml
Imports System.Threading

<Serializable()> _
Public MustInherit Class BaseGraphicalStep
    Implements IMovableObject

    'presentation
    Protected m_Position As Drawing.Point
    Protected m_Area As Drawing.Rectangle
    Protected m_Selected As Boolean
    Protected TopRectSelected As Boolean
    Protected BottomRectSelected As Boolean
    Protected m_Dimension As Integer
    Protected Carattere As Drawing.Font
    Protected BackColor As Drawing.Color
    Protected SelectionColor As Drawing.Color
    Protected NotSelectionColor As Drawing.Color
    Protected TextColor As Drawing.Color
    Protected ColorActive As Drawing.Color
    Protected ColorPreActive As Drawing.Color
    Protected ColorDeactive As Drawing.Color
    <NonSerialized()> Protected GraphToDraw As Drawing.Graphics
    Protected DrawState As Boolean
    Protected Const SmallRectWidthFactor As Single = 1 / 3
    Protected Const SmallRectHeightFactor As Single = 1 / 5

    'domain
    Protected m_SuperBody As Body
    Protected m_Name As String
    Protected m_documentation As String
    Protected m_Number As Integer
    Protected m_Negated As Boolean
    Protected m_Active As Boolean
    Protected m_PreActive As Boolean
    Protected m_TimeActivation As DateTime

    Public Property Name() As String
        Get
            Name = m_Name
        End Get
        Set(ByVal Value As String)
            m_Name = Value
        End Set
    End Property

    Public Property Documentation() As String
        Get
            Return m_documentation
        End Get
        Set(ByVal Value As String)
            m_documentation = Value
        End Set
    End Property

    Public Property Number() As Integer
        Get
            Number = m_Number
        End Get
        Set(ByVal Value As Integer)
            m_Number = Value
        End Set
    End Property

    Public Property Negated() As Boolean
        Get
            Negated = m_Negated
        End Get
        Set(ByVal Value As Boolean)
            m_Negated = Value
        End Set
    End Property

    Public Property Area() As Drawing.Rectangle
        Get
            Return m_Area
        End Get
        Set(ByVal value As Drawing.Rectangle)
            m_Area = value
        End Set
    End Property

    Public ReadOnly Property SuperBody() As Body
        Get
            Return m_SuperBody
        End Get
    End Property



    Public MustOverride Sub DisposeMe()
    Public MustOverride Sub Active()
    Public MustOverride Sub Disactive()
    Public MustOverride Sub Cancel(ByVal CancellSmallRectangels As Boolean) Implements IMovableObject.Cancel
    Public MustOverride Sub Draw(ByVal DrawSmallRectangels As Boolean) Implements IMovableObject.Draw
    Public MustOverride Sub CalculateArea() Implements IMovableObject.CalculateArea
    Public MustOverride Sub Move(ByVal dx As Integer, ByVal dy As Integer) Implements IMovableObject.Move
    Public MustOverride Sub SetGraphToDraw(ByRef Graph As Drawing.Graphics) Implements IMovableObject.SetGraphToDraw
    Public MustOverride Function CreateInstance(ByRef refSuperBody As Body) As Object

    Public Sub New(ByVal StepNumber As Integer, ByRef StepName As String, _
                   ByVal StepDocumentation As String, ByRef refSuperBody As Body, _
                   ByVal P As Drawing.Point, _
                   ByVal BackCol As Drawing.Color, ByVal SelectionCol As Drawing.Color, _
                   ByVal NotSelectionCol As Drawing.Color, ByVal TextCol As Drawing.Color, _
                   ByVal Car As Drawing.Font, ByVal ColActive As Drawing.Color, _
                   ByVal ColDeactive As Drawing.Color, ByVal ColPreActive As Drawing.Color, _
                   ByRef Graph As Drawing.Graphics, ByVal DrState As Boolean, ByVal Dimen As Integer)

        m_Number = StepNumber
        m_Name = StepName
        m_documentation = StepDocumentation
        m_SuperBody = refSuperBody
        m_Position = P
        BackColor = BackCol
        SelectionColor = SelectionCol
        NotSelectionColor = NotSelectionCol
        TextColor = TextCol
        Carattere = Car
        ColorActive = ColActive
        ColorDeactive = ColDeactive
        ColorPreActive = ColPreActive
        GraphToDraw = Graph
        DrawState = DrState
        m_Dimension = Dimen
        m_Area = New Drawing.Rectangle

    End Sub

    Public Sub New()
        m_Area = New Drawing.Rectangle
    End Sub

    Public Overridable Function IsAStep() As Boolean
        Return False
    End Function

    Public Overridable Function getActionsByEvent(ByVal actEvent As enumActionEvent) As List(Of Action)
        Return New List(Of Action)
    End Function

    Public Overridable Function getActionsByType(ByVal actType As enumActionType) As List(Of Action)
        Return New List(Of Action)
    End Function


    Public Sub SetDrawState(ByRef Value As Boolean)
        DrawState = Value
    End Sub

    Public Function ReadTimeActivation() As DateTime
        ReadTimeActivation = m_TimeActivation
    End Function

    Function ReadActive() As Boolean
        ReadActive = m_Active
    End Function

    Function ReadPreActive() As Boolean
        ReadPreActive = m_PreActive
    End Function

    Public Sub SetPosition(ByVal p As Drawing.Point)
        m_Position = p
        CalculateArea()
    End Sub
    Public Function ReadPosition() As Drawing.Point
        ReadPosition = m_Position
    End Function
    Public Sub SetSelected(ByVal v As Boolean)
        m_Selected = v
    End Sub
    Public Sub SelectObject() Implements IMovableObject.SelectObject
        SetSelected(True)
    End Sub
    Public Sub DeselectObject() Implements IMovableObject.DeselectObject
        SetSelected(False)
    End Sub
    Public Function ReadSelected() As Boolean
        ReadSelected = m_Selected
    End Function
    Public Sub SetDimension(ByVal Dimen As Integer)
        m_Dimension = Dimen
    End Sub
    Public Function ReadDimension() As Integer
        ReadDimension = m_Dimension
    End Function
    Public Property Size() As Drawing.Size Implements IMovableObject.Size
        Get
            Return New Drawing.Size(ReadDimension(), ReadDimension())
        End Get
        Set(ByVal value As Drawing.Size)
            m_Dimension = Math.Max(value.Width, value.Height)
        End Set
    End Property
    Public Sub SetTopRectSelected(ByVal v As Boolean)
        TopRectSelected = v
    End Sub
    Public Function ReadTopRectSelected() As Boolean
        ReadTopRectSelected = TopRectSelected
    End Function

    Public Sub SetBottomRectSelected(ByVal v As Boolean)
        BottomRectSelected = v
    End Sub
    Public Function ReadBottomRectSelected() As Boolean
        ReadBottomRectSelected = BottomRectSelected
    End Function
    Public Function ReadArea() As Drawing.Rectangle
        ReadArea = m_Area
    End Function

    Public Function SmallRetX(ByVal RectangleX As Integer, ByVal Dimension As Integer)
        Return RectangleX - CInt(Dimension * SmallRectWidthFactor / 2)
    End Function
    Public Function TopSmallRetY(ByVal RectangleY As Integer, ByVal Dimension As Integer)
        Return RectangleY - CInt(Dimension / 2) - CInt(Dimension * SmallRectHeightFactor)
    End Function
    Public Function BottomSmallRetY(ByVal RectangleY As Integer, ByVal Dimension As Integer)
        Return RectangleY + CInt(Dimension / 2) + 1
    End Function
    Public Function SmallRetLarg(ByVal Dimension As Integer) As Integer
        Return CInt(Dimension * SmallRectWidthFactor)
    End Function
    Public Function SmallRetAlt(ByVal Dimension As Integer) As Integer
        Return CInt(Dimension * SmallRectHeightFactor)
    End Function

    Public Function IsMyTopRect(ByVal x As Integer, ByVal y As Integer) As Boolean

        If Abs(x - m_Position.X) <= CInt(m_Dimension * SmallRectWidthFactor / 2) And _
                Abs(y - (m_Position.Y - CInt(m_Dimension / 2) - _
                        CInt(m_Dimension * SmallRectHeightFactor / 2))) <= _
                                (m_Dimension * SmallRectHeightFactor / 2) Then

            IsMyTopRect = True
        End If
    End Function

    Public Function IsMyBotRect(ByVal x As Integer, ByVal y As Integer) As Boolean
        If Abs(x - m_Position.X) <= CInt(m_Dimension * SmallRectWidthFactor / 2) And _
                Abs(y - (m_Position.Y + CInt(m_Dimension / 2) + _
                        CInt(m_Dimension * SmallRectHeightFactor / 2))) <= _
                                (m_Dimension * SmallRectHeightFactor / 2) Then

            IsMyBotRect = True
        End If
    End Function

    Public Function MyAreaIsHere(ByVal x As Integer, ByVal y As Integer) As Boolean
        If Abs(x - m_Position.X) <= (m_Dimension / 2) And Abs(y - m_Position.Y) <= (m_Dimension / 2) Then
            MyAreaIsHere = True
        End If
    End Function

    Public Property Selected() As Boolean Implements ISelectable.Selected
        Get
            Return ReadSelected()
        End Get
        Set(ByVal value As Boolean)
            SetSelected(value)
        End Set
    End Property

    Public Property Position() As System.Drawing.Point Implements IGraphicalObject.Position
        Get
            Return m_Position
        End Get
        Set(ByVal value As System.Drawing.Point)
            m_Position = value
        End Set
    End Property



    '
    'Common code for different step subclasses
    '

    Protected Sub DrawSmallRectangles(ByRef Penna As Drawing.Pen, _
            ByVal Col1 As Drawing.Color, ByVal Col2 As Drawing.Color, _
            ByVal Col3 As Drawing.Color, ByVal Col4 As Drawing.Color)

        Dim larg As Integer
        Dim alt As Integer

        If TopRectSelected Then
            Penna.Color = Col1
        Else
            Penna.Color = Col2
        End If
        larg = SmallRetLarg(m_Dimension)
        alt = SmallRetAlt(m_Dimension)
        Penna.Width = 2
        GraphToDraw.DrawRectangle(Penna, _
                SmallRetX(m_Position.X, m_Dimension), _
                TopSmallRetY(m_Position.Y, m_Dimension), _
                larg, alt)

        If BottomRectSelected Then
            Penna.Color = Col1
        Else
            Penna.Color = Col2
        End If
        larg = SmallRetLarg(m_Dimension)
        alt = SmallRetAlt(m_Dimension)
        Penna.Width = 2
        GraphToDraw.DrawRectangle(Penna, _
                SmallRetX(m_Position.X, m_Dimension), _
                BottomSmallRetY(m_Position.Y, m_Dimension), _
                larg, alt)

    End Sub

    Protected Sub DrawStepText(ByVal Col2 As Drawing.Color)

        Dim Br As Drawing.Brush
        Dim Rect As Drawing.SizeF
        Br = New Drawing.SolidBrush(Col2)
        If Not IsNothing(m_Name) Then
            Rect = GraphToDraw.MeasureString(m_Name, Carattere)
            'Controlla se il nome è più largo
            If Rect.Width < m_Dimension Then
                GraphToDraw.DrawString(m_Name, Carattere, Br, _
                        m_Position.X - (Rect.Width / 2) + 1, m_Position.Y - (Rect.Height / 2))
            Else
                Dim NewRect As New Drawing.RectangleF(m_Position.X - m_Dimension / 2 + 4, _
                        m_Position.Y - (Rect.Height / 2), m_Dimension - 4, Rect.Height)
                GraphToDraw.DrawString(m_Name, Carattere, Br, NewRect)
            End If
        End If

    End Sub

    Public Sub DrawStepState(ByVal Cancel As Boolean)
        Try
            If Monitor.TryEnter(GraphToDraw, 2000) Then
                Dim Lato As Integer = CInt(m_Dimension / 4)
                Dim Br As Drawing.Brush
                If Cancel Then
                    Br = New Drawing.SolidBrush(BackColor)
                Else
                    If m_PreActive And Not m_Active Then
                        Br = New Drawing.SolidBrush(ColorPreActive)
                    Else
                        If m_Active Then
                            Br = New Drawing.SolidBrush(ColorActive)
                        Else
                            Br = New Drawing.SolidBrush(ColorDeactive)
                        End If
                    End If
                End If
                GraphToDraw.FillEllipse(Br, m_Position.X - CInt(m_Dimension / 8), m_Position.Y + CInt(m_Dimension / 7), Lato, Lato)
            End If
        Catch ex As System.Exception
        Finally
            Monitor.Exit(GraphToDraw)
        End Try
    End Sub

    Public Sub CompleteDeserialization(ByRef refChar As Drawing.Font, _
            ByRef refBackColor As Drawing.Color, _
            ByRef refSelectionColor As Drawing.Color, _
            ByRef refNotSelectionColor As Drawing.Color, _
            ByRef refTextColor As Drawing.Color, _
            ByRef refColorActive As Drawing.Color, _
            ByRef refColorPreactive As Drawing.Color, _
            ByRef refColorDeactive As Drawing.Color, _
            ByRef refColTransitionConditionTrue As Drawing.Color, _
            ByRef refColTransitionConditionFalse As Drawing.Color)

        Carattere = refChar

        BackColor = refBackColor
        SelectionColor = refSelectionColor
        NotSelectionColor = refNotSelectionColor
        TextColor = refTextColor
        ColorActive = refColorActive
        ColorPreActive = refColorPreactive
        ColorDeactive = refColorDeactive

    End Sub

End Class
