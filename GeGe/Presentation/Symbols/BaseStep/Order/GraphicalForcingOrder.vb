Imports System.IO
Imports System.Math
Imports System.Xml
Imports System.Threading

Public Enum enumSituationType
    Explicit
    Current
    Empty
    Initial
End Enum

Public Class GraphicalForcingOrder
    Inherits GraphicalOrder

    'Domain
    Protected WithEvents m_Grafcet As Grafcet
    Protected m_Situation As enumSituationType
    Protected m_SubSteps As GraphicalStepsList

    Public Sub New(ByRef Gr As Grafcet, _
            ByVal valSituation As enumSituationType, _
            ByRef refSubSteps As GraphicalStepsList, _
            ByVal Dimension As Integer, _
            ByVal P As Drawing.Point, ByVal BackCol As Drawing.Color, ByVal SelectionCol As Drawing.Color, _
            ByVal NotSelectionCol As Drawing.Color, ByVal TextCol As Drawing.Color, ByVal Car As Drawing.Font, _
            ByRef Graph As Drawing.Graphics)

        MyBase.New(Dimension, P, BackCol, SelectionCol, NotSelectionCol, TextCol, Car, Graph)

        m_Grafcet = Gr
        m_Situation = valSituation
        m_SubSteps = refSubSteps

    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function CreateInstance() As GraphicalForcingOrder
        CreateInstance = New GraphicalForcingOrder(m_Grafcet, m_Situation, m_SubSteps, _
                CInt(m_Width / WIDTH_FACTOR), m_position, m_BackColor, m_SelectionColor, _
                m_NotSelectionColor, m_TextColor, m_TypeChar, Nothing)
    End Function

    Public Property Grafcet() As Grafcet
        Get
            Return m_Grafcet
        End Get
        Set(ByVal Value As Grafcet)
            m_Grafcet = Value
        End Set
    End Property

    Public Property Situation() As enumSituationType
        Get
            Return m_Situation
        End Get
        Set(ByVal Value As enumSituationType)
            m_Situation = Value
        End Set
    End Property

    Public Property SubSteps() As GraphicalStepsList
        Get
            Return m_SubSteps
        End Get
        Set(ByVal Value As GraphicalStepsList)
            m_SubSteps = Value
        End Set
    End Property



    Protected Overrides Function CalculateHeight() As Integer
        Return CInt(m_Dimension * HEIGHT_FACTOR)
    End Function



    '@see Committee Draft IEC 60848 Ed. 2 - 7.2 Structuring using the forcing of a partial grafcet
    'Table 9 - Forcing of a partial grafcet
    Protected Overrides Sub Draw(ByVal Col1 As Drawing.Color, ByVal Col2 As Drawing.Color, _
            ByVal Col3 As Drawing.Color)

        Dim Penna As New Drawing.Pen(m_NotSelectionColor)
        Dim StartXPoint As Integer = m_position.X
        Dim BoxY1 As Integer = CInt(m_position.Y - CInt(m_Height / 2))
        Dim Br As New Drawing.SolidBrush(Col3)
        Penna.Width = 1

        If m_Selected Then
            Penna.Color = Col1
        Else
            Penna.Color = Col2
        End If

        'outer rectangle
        m_GraphToDraw.DrawRectangle(Penna, StartXPoint, _
                BoxY1, m_Width, m_Height)

        'inner rectangle
        m_GraphToDraw.DrawRectangle(Penna, StartXPoint + 4, _
                BoxY1 + 4, m_Width - 8, m_Height - 8)

        Dim txt As String

        'eg: G12
        txt = m_Grafcet.Name

        Select Case m_Situation

            Case enumSituationType.Explicit
                'eg: G12{8,9,11}
                txt += "{"

                Dim isFirst As Boolean = True
                For Each st As GraphicalStep In m_SubSteps
                    If isFirst Then
                        isFirst = False
                    Else
                        txt += ","
                    End If
                    txt += st.Name
                Next

                txt += "}"

            Case enumSituationType.Current
                'eg: G12{*}
                txt += "{*}"

            Case enumSituationType.Empty
                'eg: G12{}
                txt += "{}"

            Case enumSituationType.Initial
                'eg: G12{INIT}
                txt += "{INIT}"

        End Select

        m_GraphToDraw.DrawString(txt, m_TypeChar, Br, _
                StartXPoint + m_Width / 10, _
                BoxY1 + m_Height / 3)

    End Sub

    'Private Sub HandlesGrafcetNameChanged(ByVal Name As String) Handles m_Grafcet.NameChanged
    '    If Not IsNothing(m_GraphToDraw) Then
    '        If Not Erased Then Draw()
    '    End If
    'End Sub


End Class

