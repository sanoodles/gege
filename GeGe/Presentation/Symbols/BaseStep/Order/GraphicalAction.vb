Public Class GraphicalAction
    Inherits GraphicalOrder

    'Domain
    Protected WithEvents m_Action As Action

    Public ReadOnly Property Action() As Action
        Get
            Return m_Action
        End Get
    End Property

    '
    'These properites are a gateway to m_Action attributes
    '
    Protected Property m_actType() As enumActionType
        Get
            Return m_Action.actType
        End Get
        Set(ByVal value As enumActionType)
            m_Action.actType = value
        End Set
    End Property

    Protected Property m_Var() As Variable
        Get
            Return m_Action.Var
        End Get
        Set(ByVal value As Variable)
            m_Action.Var = value
        End Set
    End Property

    Protected Property m_Condition() As Condition
        Get
            Return m_Action.Cond
        End Get
        Set(ByVal value As Condition)
            m_Action.Cond = value
        End Set
    End Property

    Protected Property m_Assignation() As String
        Get
            Return m_Action.Assignation
        End Get
        Set(ByVal value As String)
            m_Action.Assignation = value
        End Set
    End Property

    Protected Property m_actEvent() As enumActionEvent
        Get
            Return m_Action.actEvent
        End Get
        Set(ByVal value As enumActionEvent)
            m_Action.actEvent = value
        End Set
    End Property



    Public Sub New(ByVal actType As enumActionType, _
        ByRef Var As Variable, _
        ByVal vCondition As Condition, _
        ByVal Assignation As String, _
        ByVal actEvent As enumActionEvent, _
        ByVal Dimension As Integer, _
        ByVal P As Drawing.Point, ByVal BackCol As Drawing.Color, ByVal SelectionCol As Drawing.Color, _
        ByVal NotSelectionCol As Drawing.Color, ByVal TextCol As Drawing.Color, ByVal Car As Drawing.Font, _
        ByRef Graph As Drawing.Graphics)

        MyBase.New(Dimension, P, BackCol, SelectionCol, NotSelectionCol, TextCol, Car, Graph)

        m_Action = New Action(actType, Var, vCondition, Assignation, actEvent)
    End Sub

    Public Sub New()
        MyBase.new()
    End Sub



    Public Function CreateInstance() As GraphicalOrder
        CreateInstance = New GraphicalAction(m_actType, m_Var, m_Condition, m_Assignation, m_actEvent, _
                CInt(m_Width / WIDTH_FACTOR), m_position, m_BackColor, m_SelectionColor, _
                m_NotSelectionColor, m_TextColor, m_TypeChar, Nothing)
    End Function

    Public Property actType() As enumActionType
        Get
            Return m_actType
        End Get
        Set(ByVal Value As enumActionType)
            m_actType = Value
        End Set
    End Property

    Public Property Variable() As Variable
        Get
            Variable = m_Var
        End Get
        Set(ByVal Value As Variable)
            m_Var = Value
        End Set
    End Property

    Public Property Condition() As Condition
        Get
            Return m_Condition
        End Get
        Set(ByVal Value As Condition)
            m_Condition = Value
        End Set
    End Property

    Public Property Assignation() As String
        Get
            Return m_Assignation
        End Get
        Set(ByVal Value As String)
            m_Assignation = Value
        End Set
    End Property

    Public Property actEvent() As enumActionEvent
        Get
            Return m_actEvent
        End Get
        Set(ByVal Value As enumActionEvent)
            m_actEvent = Value
        End Set
    End Property

    Protected Overrides Function CalculateHeight() As Integer
        Dim r As Integer

        r = CInt(m_Dimension * HEIGHT_FACTOR)
        'If (m_Condition <> "") Then r += 20

        Return r
    End Function



    Protected Overrides Sub Draw(ByVal Col1 As Drawing.Color, ByVal Col2 As Drawing.Color, _
        ByVal Col3 As Drawing.Color)

        Dim Penna As New Drawing.Pen(m_NotSelectionColor)
        Dim StartXPoint As Integer = m_position.X
        Dim BoxY1 As Integer = CInt(m_position.Y - CInt(m_Height / 2))
        Dim Br As New Drawing.SolidBrush(Col3)
        Penna.Width = 1

        'Main rectangle
        If m_Selected Then
            Penna.Color = Col1
        Else
            Penna.Color = Col2
        End If
        m_GraphToDraw.DrawRectangle(Penna, StartXPoint, _
                BoxY1, m_Width, m_Height)

        Dim txt1 As String = ""
        Dim txt2 As String = ""

        'Continuous
        '   txt1: CONDITION
        '   txt2: VARIABLE
        'Stored
        '   Activation
        '       txt1: "Act"
        '       txt2: ASSIGNATION
        '   Deactivation: 
        '       txt1: "Dac"
        '       txt2: ASSIGNATION

        'Continuous action
        If m_actType = enumActionType.Continuous Then

            If m_Condition.GetString <> "true" Then
                txt1 = m_Condition.GetString
            End If
            txt2 = m_Var.Name

            'Stored action
        ElseIf m_actType = enumActionType.Stored Then

            txt2 = m_Assignation

            'Draw event
            If m_actEvent = enumActionEvent.Activation Then

                txt1 = "Act"

                ''Upper line
                'm_GraphToDraw.DrawLine(Penna, _
                '        StartXPoint, m_position.Y - CInt(m_Height / 2), _
                '        StartXPoint, m_position.Y - CInt(m_Height / 2) - 10)

                ''Triangle
                ''   upper point
                'Dim up As Point = New Point(StartXPoint, boxy1 - 10)
                ''   lower left point
                'Dim llp As Point = New Point(StartXPoint - 3, boxy1 - 7)
                ''   lower right point
                'Dim lrp As Point = New Point(StartXPoint + 3, boxy1 - 7)
                ''   array
                'Dim ps(3) As Point
                'ps(0) = up
                'ps(1) = llp
                'ps(2) = lrp
                ''   draw
                ''m_GraphToDraw.DrawPolygon(Penna, ps)'does not work well

            ElseIf m_actEvent = enumActionEvent.Deactivation Then

                txt1 = "Dac"

                ''Lower line
                'm_GraphToDraw.DrawLine(Penna, _
                '        StartXPoint, boxy1 + m_height, _
                '        StartXPoint, boxy1 + m_height + 10)

                ''Triangle
                ''   upper left point

                ''   upper right point

                ''   lower point

            End If

        End If

        'txt1
        m_GraphToDraw.DrawString(txt1, m_TypeChar, Br, _
                StartXPoint + m_Width / 20, _
                BoxY1 + m_Height / 3)

        'separator
        m_GraphToDraw.DrawLine(Penna, _
                StartXPoint + CInt(m_Width / 2), _
                BoxY1, _
                StartXPoint + CInt(m_Width / 2), _
                BoxY1 + m_Height)

        'txt2
        m_GraphToDraw.DrawString(txt2, m_TypeChar, Br, _
                StartXPoint + CInt(m_Width / 2) + 10, _
                BoxY1 + m_Height / 3)

    End Sub

    Private Sub HandlesVarNameChanging(ByVal Name As String) Handles m_Action.NameChangingInVariable
        If Not IsNothing(m_GraphToDraw) Then
            Cancel()
        End If
    End Sub

    Private Sub HandlesVarNameChanged(ByVal Name As String) Handles m_Action.NameChangedInVariable
        If Not IsNothing(m_GraphToDraw) Then
            If Not Erased Then Draw()
        End If
    End Sub


End Class
