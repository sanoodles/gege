' Gli oggetti di questa interfaccia possono essere selezionati
' In realtà sembra poco plausibile avere oggetti selezionabili che non siano disegnabili, ma questo artificio
' ci serve per poter implementare BasicSelectionManager
Public Interface ISelectable
    Sub SelectObject()
    Sub DeselectObject()
    Property Selected() As Boolean
End Interface

Public Interface IPositionable
    Function ReadPosition() As Drawing.Point
    Sub SetPosition(ByVal value As Drawing.Point)
End Interface
Public Interface IPositionableProperty
End Interface

Public Interface ISizable
    Property Size() As Drawing.Size
End Interface

' Un semplice oggetto per gestire lo stato di selezionato / deselezionato di un
' altro oggetto
Public Class BasicSelectionManager
    Implements ISelectable

    Private m_Selected As Boolean
    Public Event SelectionChanged(ByVal sender As ISelectable, ByVal newValue As Boolean)

    Public Sub New(Optional ByVal initial As Boolean = False)
        m_Selected = initial
    End Sub

    Public Sub DeselectObject() Implements ISelectable.DeselectObject
        Selected = False
    End Sub

    Public Property Selected() As Boolean Implements ISelectable.Selected
        Get
            Return m_Selected
        End Get
        Set(ByVal value As Boolean)
            Dim fire As Boolean = (value <> Selected)
            m_Selected = value
            If fire Then RaiseEvent SelectionChanged(Me, Selected)
        End Set
    End Property

    Public Sub SelectObject() Implements ISelectable.SelectObject
        Selected = True
    End Sub
End Class

' Questa interfaccia rappresenta gli oggetti "grafici", cioè quelli che possono
' essere disegnati su schermo
Public Interface IGraphicalObject
    Inherits ISelectable, ISizable

    Sub SetGraphToDraw(ByRef Graph As Drawing.Graphics)
    Sub Draw(ByVal DrawSmallRectangles As Boolean)
    Sub Cancel(ByVal DrawSmallRectangles As Boolean)
    Sub CalculateArea()
    Property Position() As Drawing.Point
End Interface

' Un oggetto che può essere spostato
Public Interface IMovableObject
    Inherits IGraphicalObject

    Sub Move(ByVal dx As Integer, ByVal dy As Integer)
End Interface