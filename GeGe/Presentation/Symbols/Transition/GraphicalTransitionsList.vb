Imports System.IO
Imports System.Collections.Generic
Imports System.Xml

<Serializable()> _
Public Class GraphicalTransitionsList
    Inherits List(Of GraphicalTransition)

    Private m_Body As Body

    Dim NotSelectionColor As Drawing.Color
    Dim SelectionColor As Drawing.Color
    Dim Carattere As Drawing.Font
    Dim BackColor As Drawing.Color
    Dim TextColor As Drawing.Color
    Dim ColorConditionTrue As Drawing.Color
    Dim ColorConditionFalse As Drawing.Color
    Dim ColorActive As Drawing.Color
    Dim ColorPreActive As Drawing.Color
    Dim ColorDeactive As Drawing.Color

    <NonSerialized()> Private GraphToDraw As Drawing.Graphics
    Private DrawState As Boolean
    Private Dimension As Integer

    Sub New(ByRef refBody As Body, ByVal ColSfondo As Drawing.Color, ByVal SelectionCol As Drawing.Color, ByVal NotSelectionCol As Drawing.Color, ByVal ColTesto As Drawing.Color, ByVal Car As Drawing.Font, ByVal ColConditionTrue As Drawing.Color, ByVal ColConditionFalse As Drawing.Color, ByVal ColActive As Drawing.Color, ByVal ColDeactive As Drawing.Color, ByVal ColPreActive As Drawing.Color, ByRef Graph As Drawing.Graphics, ByVal Dimen As Integer)
        MyBase.New()
        m_Body = refBody
        NotSelectionColor = NotSelectionCol
        SelectionColor = SelectionCol
        Carattere = Car
        BackColor = ColSfondo
        TextColor = ColTesto
        ColorActive = ColActive
        ColorPreActive = ColPreActive
        ColorDeactive = Drawing.Color.Brown
        ColorConditionTrue = ColConditionTrue
        ColorConditionFalse = ColConditionFalse
        GraphToDraw = Graph
        Dimension = Dimen
    End Sub

    Public Sub New(ByRef refBody As Body, ByRef R As BinaryReader, ByRef m_GraphicalStepsList As GraphicalStepsList, ByVal ColSfondo As Drawing.Color, ByVal SelectionCol As Drawing.Color, ByVal NotSelectionCol As Drawing.Color, ByVal ColTesto As Drawing.Color, ByVal Car As Drawing.Font, ByVal ColConditionTrue As Drawing.Color, ByVal ColConditionFalse As Drawing.Color, ByVal ColActive As Drawing.Color, ByVal ColDeactive As Drawing.Color, ByVal ColPreActive As Drawing.Color, ByRef Graph As Drawing.Graphics)
        m_Body = refBody
        NotSelectionColor = NotSelectionCol
        SelectionColor = SelectionCol
        Carattere = Car
        BackColor = ColSfondo
        TextColor = ColTesto
        ColorActive = ColActive
        ColorPreActive = ColPreActive
        ColorDeactive = ColDeactive
        ColorConditionTrue = ColConditionTrue
        ColorConditionFalse = ColConditionFalse
        GraphToDraw = Graph
        Dim i As Integer
        Dim NumTransizions As Integer = R.ReadInt32
        For i = 1 To NumTransizions
            'Dim T As New GraphicalTransition
            'Add(T)
        Next i
    End Sub
    Public Sub New()
        MyBase.new()
    End Sub
    Public Function CreateInstance(ByRef refBody As Body, ByRef StepsList As GraphicalStepsList) As GraphicalTransitionsList
        CreateInstance = New GraphicalTransitionsList(refBody, BackColor, SelectionColor, NotSelectionColor, TextColor, Carattere, ColorConditionTrue, ColorConditionFalse, ColorActive, ColorDeactive, ColorPreActive, Nothing, Dimension)
        For Each T As GraphicalTransition In Me
            CreateInstance.Add(T.CreateInstance(refBody, StepsList))
        Next T
    End Function
    Public Sub DisposeMe()
        'Distrugge tutte le transizioni
        For Each T As GraphicalTransition In Me
            T.DisposeMe()
        Next T
        Me.Finalize()
    End Sub


    Public Sub ClearXMLConnectionsLists()
        'Svuota le liste delle connessioni delle tansizioni per l'esportazione xml
        For Each T As GraphicalTransition In Me
            T.ReadXmlPreviousConnectionsList.Clear()
        Next T
    End Sub

    Public Sub SetGraphToDraw(ByRef Graph As Drawing.Graphics)
        GraphToDraw = Graph
        For Each T As GraphicalTransition In Me
            T.SetGraphToDraw(GraphToDraw)
        Next T
    End Sub

    Public Sub SetDrawState(ByVal DrState As Boolean)
        DrawState = DrState
        For Each T As GraphicalTransition In Me
            T.SetDrawState(DrawState)
        Next T
    End Sub

    Public Sub DrawTransitionsState()
        For Each T As GraphicalTransition In Me
            T.DrawTransitionState()
        Next T
    End Sub

    Public Sub AddAndDrawTransition(ByVal Number As Integer, _
            ByVal Name As String, ByVal Documentation As String, _
            ByRef LPreviousSteps As GraphicalStepsList, _
            ByRef LNextSteps As GraphicalStepsList, _
            ByRef RefCondition As Condition, _
            ByVal FlagShowDetails As Boolean, _
            ByVal DrawState As Boolean)
        Dim T As New GraphicalTransition(m_Body, Number, Name, Documentation, _
                LPreviousSteps, LNextSteps, RefCondition, -1, BackColor, SelectionColor, _
                NotSelectionColor, TextColor, Carattere, ColorConditionTrue, ColorConditionFalse, _
                ColorActive, ColorPreActive, ColorDeactive, GraphToDraw, DrawState, Dimension)
        Add(T)
        T.Draw(FlagShowDetails)
    End Sub

    Public Function ReadTransition(ByVal i As Integer) As GraphicalTransition
        ReadTransition = Me(i)
    End Function

    Public Sub DrawArea(ByVal Rect As Drawing.Rectangle, ByVal ShowDetails As Boolean)
        For Each T As GraphicalTransition In Me
            'La Draw se si interseca con il Rectangle da Drawre
            If T.ReadArea.IntersectsWith(Rect) Then
                T.Draw(ShowDetails)
            End If
        Next T
    End Sub

    Function IndexOfTransition(ByVal n As Integer) As Integer
        Dim i As Integer
        IndexOfTransition = -1
        For i = 0 To Count - 1
            If Me(i).Number = n Then
                IndexOfTransition = i
                Exit For
            End If
        Next
    End Function

    Public Function FindTransitionByNumber(ByVal n As Integer) As GraphicalTransition
        For Each T As GraphicalTransition In Me
            If T.Number = n Then
                Return T
            End If
        Next T
        Return Nothing
    End Function
    Public Function IndexOfTransition(ByRef T As GraphicalTransition) As Integer
        IndexOfTransition = IndexOf(T)
        If IsNothing(IndexOfTransition) Then
            IndexOfTransition = -1
        End If
    End Function
    Function FirstAvaiableTransitionNumber() As Integer
        Dim j As Integer
        For j = 1 To Count + 1
            If IndexOfTransition(j) = -1 Then
                FirstAvaiableTransitionNumber = j
                Exit For
            End If
        Next
    End Function
    Public Function CountTransitionsList() As Integer
        CountTransitionsList = Count
    End Function
    Public Function FindTransition(ByVal x As Integer, ByVal y As Integer) As Boolean
        'Find la prima Transition in x,y
        For Each T As GraphicalTransition In Me
            If T.MyAreaIsHere(x, y) Then
                FindTransition = True
                Exit Function
            End If
        Next T
    End Function
    Public Function FindAndSelectTransition(ByVal x As Integer, ByVal y As Integer) As Boolean
        'Seleziona solo la prima transizione che trova
        For Each T As GraphicalTransition In Me
            If T.MyAreaIsHere(x, y) = True Then
                FindAndSelectTransition = True
                T.SetSelected(True)
                Exit Function
            End If
        Next T
    End Function
    Public Sub FindAndSelectTransitions(ByVal Rect As Drawing.Rectangle)
        'Seleziona tutte le transizioni che trova
        Dim x, y, Passo As Integer
        For Each T As GraphicalTransition In Me
            Passo = CInt(T.ReadDimension() / 5)
            For x = Rect.X To Rect.X + Rect.Width Step Passo
                For y = Rect.Y To Rect.Y + Rect.Height Step Passo
                    If T.MyAreaIsHere(x, y) = True Then
                        T.SetSelected(True)
                    End If
                Next y
            Next x
            'Ricerca sul bordo verticale destro
            x = Rect.X + Rect.Width
            For y = Rect.Y To Rect.Y + Rect.Height Step Passo
                If T.MyAreaIsHere(x, y) = True Then
                    T.SetSelected(True)
                End If
            Next y
            'Ricerca sul bordo orizzontale sinistro
            y = Rect.Y + Rect.Height
            For x = Rect.X To Rect.X + Rect.Width Step Passo
                If T.MyAreaIsHere(x, y) = True Then
                    T.SetSelected(True)
                End If
            Next x
            'Ricerca sull'angolo inferiore destro 
            x = Rect.X + Rect.Width
            y = Rect.Y + Rect.Height
            If T.MyAreaIsHere(x, y) = True Then
                T.SetSelected(True)
            End If
        Next T
    End Sub
    Public Function ReadIfTransitionSelected(ByVal x As Integer, ByVal y As Integer) As Boolean
        'Find la prima Step in x,y
        For Each T As GraphicalTransition In Me
            If T.MyAreaIsHere(x, y) And T.ReadSelected Then
                ReadIfTransitionSelected = True
                Exit Function
            End If
        Next T
    End Function
    Public Sub DeSelectAll()
        For Each T As GraphicalTransition In Me
            T.SetSelected(False)
        Next T
    End Sub
    Public Sub SelectAll()
        For Each T As GraphicalTransition In Me
            T.SetSelected(True)
        Next T
    End Sub
    Public Sub RemoveSelectedElements()
        Dim i, j As Integer
        j = 0
        For i = 0 To Count - 1
            If Me(i - j).ReadSelected = True Then
                Me(i - j).DisposeMe()
                RemoveAt(i - j)
                j = j + 1
            End If
        Next i
    End Sub
    Public Sub MoveSelectedTransitions(ByVal OffsetX As Integer)
        For Each T As GraphicalTransition In Me
            If T.ReadSelected Then
                Dim x As Integer
                x = T.ReadPosition
                x = x + OffsetX
                T.SetPosition(x)
            End If
        Next T
    End Sub
    Public Function ControllaPresenzaStepsInTransizioni(ByVal LSteps As GraphicalStepsList) As Boolean
        For Each T As GraphicalTransition In Me
            For Each F As BaseGraphicalStep In LSteps
                If T.ReadPreviousGraphicalStepsList.IndexOfStep(F) <> -1 Then
                    ControllaPresenzaStepsInTransizioni = True
                    Exit Function
                End If
                If T.ReadNextGraphicalStepsList.IndexOfStep(F) <> -1 Then
                    ControllaPresenzaStepsInTransizioni = True
                    Exit Function
                End If
            Next F
        Next T
    End Function
    Public Function ReadSelectedTransitionList() As GraphicalTransitionsList
        ReadSelectedTransitionList = New GraphicalTransitionsList
        For Each T As GraphicalTransition In Me
            If T.ReadSelected = True Then
                ReadSelectedTransitionList.Add(T)
            End If
        Next T
    End Function
    Public Function ReadArea() As Drawing.Rectangle
        Dim R As Drawing.Rectangle
        For Each T As GraphicalTransition In Me
            If IndexOfTransition(T) Then
                R = T.ReadArea
            Else
                R = Drawing.Rectangle.Union(R, T.ReadArea)
            End If
        Next T
        ReadArea = R

    End Function
    Public Function FindPreviousTransitionOfStep(ByVal F As GraphicalStep) As GraphicalTransitionsList
        FindPreviousTransitionOfStep = New GraphicalTransitionsList
        For Each T As GraphicalTransition In Me
            If T.ReadNextGraphicalStepsList.IndexOfStep(F) <> -1 Then
                FindPreviousTransitionOfStep.Add(T)
            End If
        Next T
    End Function
    Public Function FindNextTransitionOfStep(ByVal F As GraphicalStep) As GraphicalTransitionsList
        FindNextTransitionOfStep = New GraphicalTransitionsList
        For Each T As GraphicalTransition In Me
            If T.ReadPreviousGraphicalStepsList.IndexOfStep(F) <> -1 Then
                FindNextTransitionOfStep.Add(T)
            End If
        Next T
    End Function
    Public Sub CancellSelection(ByVal FlagShowDetails As Boolean)
        'Cancell le transizioni selezionate
        For Each T As GraphicalTransition In Me
            If T.ReadSelected Then
                T.Cancel(FlagShowDetails)
            End If
        Next T
    End Sub
    Public Sub CancellTransitionWithSelectedSteps(ByVal FlagShowDetails As Boolean)
        'Cancell le transizioni che hanno Steps selezionate
        For Each T As GraphicalTransition In Me
            If T.ReadPreviousGraphicalStepsList.ReadSelected.CountStepsList > 0 Or T.ReadNextGraphicalStepsList.ReadSelected.CountStepsList > 0 Then
                T.Cancel(FlagShowDetails)
            End If
        Next T
    End Sub
    Public Sub DrawSelection(ByVal ShowDetails As Boolean)
        'Draw le transizioni selezionate
        For Each T As GraphicalTransition In Me
            If T.ReadSelected Then
                T.Draw(ShowDetails)
            End If
        Next T
    End Sub
    Public Sub MoveSelection(ByVal dx As Integer)
        'Move le transizioni selezionate
        For Each T As GraphicalTransition In Me
            If T.ReadSelected Then
                T.Move(dx)
            End If
        Next T
    End Sub
    Public Function ControllaSelectionFuoriArea(ByVal R As Drawing.Rectangle, ByRef FuoriX As Boolean, ByRef FuoriY As Boolean) As Boolean
        'Controlla che l'area delle transizioni selezionate....
        '....non sia fuori da R (se è fuori restituisce True)
        Dim Area As Drawing.Rectangle
        For Each T As GraphicalTransition In Me
            If T.ReadSelected Then
                Area = T.ReadArea
                If Area.X <= R.X Or Area.X + Area.Width >= R.X + R.Width Then
                    ControllaSelectionFuoriArea = True
                    FuoriX = True
                End If
                If Area.Y <= R.Y Or Area.Y + Area.Height >= R.Y + R.Height Then
                    ControllaSelectionFuoriArea = True
                    FuoriY = True
                End If
                If ControllaSelectionFuoriArea Then
                    Exit For
                End If
            End If
        Next T
    End Function
    Public Function ControllaPresenzaTransition(ByVal n As Integer) As Boolean
        For Each T As GraphicalTransition In Me
            If T.Number = n Then
                ControllaPresenzaTransition = True
                Exit Function
            End If
        Next T
        ControllaPresenzaTransition = False
    End Function
    Public Function CountSelected() As Integer
        CountSelected = 0
        For Each T As GraphicalTransition In Me
            If T.ReadSelected Then
                CountSelected = CountSelected + 1
            End If
        Next
    End Function
    Public Function ReadTransitionSelected() As GraphicalTransition
        For Each T As GraphicalTransition In Me
            If T.ReadSelected Then
                Return T
            End If
        Next T
        Return Nothing
    End Function


    Public Sub ResetState()
        For Each T As GraphicalTransition In Me
            T.SetActualCondition(False)
        Next T
    End Sub

    Public Function GetTransitionsByPreviousStep(ByRef St As BaseGraphicalStep) As GraphicalTransitionsList
        Dim res As New GraphicalTransitionsList()

        For Each Tr As GraphicalTransition In Me
            If Tr.HasPreviousStep(St) Then res.Add(Tr)
        Next

        Return res
    End Function

End Class
