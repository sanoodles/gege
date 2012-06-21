Imports System.IO
Imports System.Xml
Imports System.Collections.Generic

<Serializable()> _
Public Class GraphicalStepsList
    Inherits List(Of BaseGraphicalStep)
    'Implements IXMLSerializable

    Private MyBody As Body
    Private Dimension As Integer
    Private SelectionColor As Drawing.Color
    Private NotSelectionColor As Drawing.Color
    Private TextColor As Drawing.Color
    Private Carattere As Drawing.Font
    Private BackColor As Drawing.Color
    Private ActiveColor As Drawing.Color
    Private PreActiveColor As Drawing.Color
    Private DeactiveColor As Drawing.Color
    <NonSerialized()> Private GraphToDraw As Drawing.Graphics
    Private DrawState As Boolean

    'Private m_pouslist As Pous
    WriteOnly Property ResGlobalVariables() As VariablesList
        Set(ByVal Value As VariablesList)
            'Comunica alle Macrofasi le lista di variabili globali della risorsa
            For Each S As BaseGraphicalStep In Me
                If S.GetType.Name = "GraphicalMacroStep" Then
                    Dim MS As GraphicalMacroStep = S
                    'MS.ResGlobalVariables = Value
                End If
            Next
        End Set
    End Property

    Sub New(ByRef RefBody As Body, ByVal BackCol As Drawing.Color, _
            ByVal SelectionCol As Drawing.Color, ByVal NotSelectionCol As Drawing.Color, _
            ByVal ColTesto As Drawing.Color, ByVal Car As Drawing.Font, _
            ByVal ColActive As Drawing.Color, ByVal ColDeactive As Drawing.Color, _
            ByVal ColPreActive As Drawing.Color, ByRef Graph As Drawing.Graphics, _
            ByVal Dimen As Integer)

        MyBase.new()
        MyBody = RefBody
        SelectionColor = SelectionCol
        NotSelectionColor = NotSelectionCol
        BackColor = BackCol
        TextColor = ColTesto
        Carattere = Car
        ActiveColor = ColActive
        DeactiveColor = ColDeactive
        PreActiveColor = ColPreActive
        GraphToDraw = Graph
        Dimension = Dimen
        'm_pouslist = pouslist
    End Sub

    Public Sub New(ByRef R As BinaryReader, ByVal BackCol As Drawing.Color, _
                   ByVal SelectionCol As Drawing.Color, ByVal NotSelectionCol As Drawing.Color, _
                   ByVal ColTesto As Drawing.Color, ByVal Car As Drawing.Font, _
                   ByVal ColActive As Drawing.Color, ByVal ColDeactive As Drawing.Color, _
                   ByVal ColPreActive As Drawing.Color, ByRef Graph As Drawing.Graphics)

        MyBase.new()
        SelectionColor = SelectionCol
        NotSelectionColor = NotSelectionCol
        BackColor = BackCol
        TextColor = ColTesto
        Carattere = Car
        ActiveColor = ColActive
        DeactiveColor = ColDeactive
        PreActiveColor = ColPreActive
        GraphToDraw = Graph
        Dim NumSteps As Integer = R.ReadInt32
        ' For i = 1 To NumSteps
        'Dim S As New GraphicalStep(R, BackCol, SelectionCol, NotSelectionCol, ColTesto, Car, ColActive, ColDeactive, ColPreActive)
        'Add(S)
        ' Next i
        'm_pouslist = pouslist
    End Sub

    Public Sub New()
        MyBase.new()
    End Sub

    Public Function CreateInstance(ByRef NewSfc As Body) As GraphicalStepsList
        CreateInstance = New GraphicalStepsList(NewSfc, BackColor, SelectionColor, NotSelectionColor, TextColor, Carattere, ActiveColor, DeactiveColor, PreActiveColor, Nothing, Dimension)
        For Each S As BaseGraphicalStep In Me
            CreateInstance.Add(S.CreateInstance(NewSfc))
        Next S
    End Function

    Public Sub DisposeMe()
        'Distrugge tutte le fasi
        For Each S As BaseGraphicalStep In Me
            S.DisposeMe()
        Next S
        Me.Finalize()
    End Sub
    Public Sub SetGraphToDraw(ByRef Graph As Drawing.Graphics)
        GraphToDraw = Graph
        For Each S As BaseGraphicalStep In Me
            S.SetGraphToDraw(GraphToDraw)
        Next
    End Sub
    Public Sub SetDrawState(ByVal DrState As Boolean)
        DrawState = DrState
        For Each F As BaseGraphicalStep In Me
            F.SetDrawState(DrawState)
        Next F
    End Sub
    Public Sub DrawStepsState()
        For Each F As BaseGraphicalStep In Me
            F.DrawStepState(False)
        Next F
    End Sub
    Public Sub AddExistingStep(ByVal S As BaseGraphicalStep)
        Add(S)
    End Sub

    Public Sub AddAndDrawStep(ByVal StepNumber As Integer, ByVal StepName As String, _
            ByVal StepDocumentation As String, ByVal Position As Drawing.Point, _
            ByVal DrawState As Boolean, Optional ByVal Initial As Boolean = False)

        Dim S As New GraphicalStep(StepNumber, StepName, StepDocumentation, MyBody, _
                Initial, False, Position, BackColor, SelectionColor, NotSelectionColor, _
                TextColor, Carattere, ActiveColor, DeactiveColor, PreActiveColor, _
                GraphToDraw, DrawState, Dimension)
        Add(S)
        S.Draw(False)
    End Sub

    Public Sub AddAndDrawEnclosedStep(ByVal StepNumber As Integer, ByVal StepName As String, _
        ByVal StepDocumentation As String, ByVal Position As Drawing.Point, _
        ByVal DrawState As Boolean, Optional ByVal Initial As Boolean = False, _
        Optional ByVal ActivationLink As Boolean = False)

        Dim S As New GraphicalEnclosedStep(StepNumber, StepName, StepDocumentation, MyBody, _
                Initial, False, ActivationLink, Position, BackColor, SelectionColor, NotSelectionColor, _
                TextColor, Carattere, ActiveColor, DeactiveColor, PreActiveColor, _
                GraphToDraw, DrawState, Dimension)
        Add(S)
        S.Draw(False)
    End Sub

    Public Sub AddAndDrawMacroStep(ByVal StepNumber As Integer, ByVal StepName As String, ByVal StepDocumentation As String, ByRef ResGlobalVariables As VariablesList, ByVal Position As Drawing.Point, ByVal DrState As Boolean)
        Dim S As New GraphicalMacroStep(StepNumber, StepName, StepDocumentation, _
            MyBody, Position, BackColor, SelectionColor, _
            NotSelectionColor, TextColor, Carattere, ActiveColor, DeactiveColor, _
            PreActiveColor, GraphToDraw, DrState, Dimension)
        Me.Add(S)
        S.Draw(False)
    End Sub

    Public Sub AddAndDrawEnclosingStep(ByVal StepNumber As Integer, ByVal StepName As String, _
            ByVal StepDocumentation As String, ByVal Position As Drawing.Point, _
            ByVal DrawState As Boolean, Optional ByVal Initial As Boolean = False)

        Dim S As New GraphicalEnclosingStep(StepNumber, StepName, StepDocumentation, MyBody, _
                Initial, False, Position, BackColor, SelectionColor, NotSelectionColor, _
                TextColor, Carattere, ActiveColor, DeactiveColor, PreActiveColor, _
                GraphToDraw, DrawState, Dimension)
        Add(S)
        S.Draw(False)
    End Sub

    Public Function FindAndSelectStep(ByVal x As Integer, ByVal y As Integer) As Boolean
        'Seleziona solo la prima fase che trova
        For Each S As BaseGraphicalStep In Me
            If S.MyAreaIsHere(x, y) = True Then
                FindAndSelectStep = True
                S.SetSelected(True)
                Exit Function
            End If
        Next S
    End Function

    Public Function FindStep(ByVal x As Integer, ByVal y As Integer) As Boolean
        'Trova la prima fase in x,y
        For Each S As BaseGraphicalStep In Me
            If S.MyAreaIsHere(x, y) Then
                FindStep = True
                Exit Function
            End If
        Next S
    End Function
    Public Function ReadIfStepSelected(ByVal x As Integer, ByVal y As Integer) As Boolean
        'Trova la prima fase in x,y
        For Each S As BaseGraphicalStep In Me
            If S.MyAreaIsHere(x, y) And S.ReadSelected Then
                ReadIfStepSelected = True
                Exit Function
            End If
        Next S
    End Function
    Public Sub FindAndSelectSteps(ByVal Rect As Drawing.Rectangle)
        'Seleziona tutte le fasi che trova
        Dim x, y, Passo As Integer
        For Each S As BaseGraphicalStep In Me
            Passo = S.ReadDimension()
            For x = Rect.X To Rect.X + Rect.Width Step Passo
                For y = Rect.Y To Rect.Y + Rect.Height Step Passo
                    If S.MyAreaIsHere(x, y) = True Then
                        S.SetSelected(True)
                    End If
                Next y
            Next x
            'Ricerca sul bordo verticale destro
            x = Rect.X + Rect.Width
            For y = Rect.Y To Rect.Y + Rect.Height Step Passo
                If S.MyAreaIsHere(x, y) = True Then
                    S.SetSelected(True)
                End If
            Next y
            'Ricerca sul bordo orizzontale sinistro
            y = Rect.Y + Rect.Height
            For x = Rect.X To Rect.X + Rect.Width Step Passo
                If S.MyAreaIsHere(x, y) = True Then
                    S.SetSelected(True)
                End If
            Next x
            'Ricerca sull'angolo inferiore destro 
            x = Rect.X + Rect.Width
            y = Rect.Y + Rect.Height
            If S.MyAreaIsHere(x, y) = True Then
                S.SetSelected(True)
            End If
        Next S
    End Sub
    Public Function FindStepByNumber(ByVal n As Integer) As BaseGraphicalStep
        For Each S As BaseGraphicalStep In Me
            If S.Number = n Then
                Return S
            End If
        Next S
        Return Nothing
    End Function
    Function FirstAvaiableStepNumber() As Integer
        Dim j As Integer = 0
        Dim NumberAvaiable As Boolean
        While j < Count + 1
            j = j + 1
            NumberAvaiable = True
            For Each S As BaseGraphicalStep In Me
                If TypeOf (S) Is GraphicalStep Then
                    If S.Number = j Then
                        NumberAvaiable = False
                        Exit For 'Ha trovato una fase co il numero j
                    End If
                End If
            Next S
            If NumberAvaiable Then
                FirstAvaiableStepNumber = j
                Exit While
            End If
        End While
    End Function
    Function FirstAvaiableMacroStepNumber() As Integer
        Dim j As Integer = 0
        Dim NumberAvaiable As Boolean
        While j < Count + 1
            j = j + 1
            NumberAvaiable = True
            For Each S As BaseGraphicalStep In Me
                If S.GetType.Name = "GraphicalMacroStep" Then
                    If S.Number = j Then
                        NumberAvaiable = False
                        Exit For 'Ha trovato una fase co il numero j
                    End If
                End If
            Next S
            If NumberAvaiable Then
                FirstAvaiableMacroStepNumber = j
                Exit While
            End If

        End While
    End Function
    Public Function ReadStep(ByVal n As Integer) As BaseGraphicalStep
        ReadStep = Me(IndexOfStep(n))
    End Function
    Public Function IndexOfStep(ByVal n As Integer) As Integer
        Dim i As Integer
        IndexOfStep = -1
        For i = 0 To Count - 1
            If Me(i).Number = n Then
                IndexOfStep = i
                Exit For
            End If
        Next
    End Function
    Public Function IndexOfStep(ByRef S As BaseGraphicalStep) As Integer
        IndexOfStep = IndexOf(S)
        If IsNothing(IndexOfStep) Then
            IndexOfStep = -1
        End If
    End Function
    Public Function CountStepsList() As Integer
        CountStepsList = Count
    End Function
    Public Sub RemoveSelectedElements()
        Dim i, j As Integer
        j = 0
        For i = 0 To Count - 1
            'Cancella le azioni della fase
            If TypeOf (Me(i - j)) Is GraphicalStep Then
                CType(Me(i - j), GraphicalStep).RemoveSelectedActions()
            End If
            'Cancella la fase se selezionata
            If Me(i - j).ReadSelected = True Then
                Me(i - j).DisposeMe()
                RemoveAt(i - j)
                j = j + 1
            End If
        Next i
    End Sub
    Public Function ReadSelected() As GraphicalStepsList
        ReadSelected = New GraphicalStepsList
        For Each S As BaseGraphicalStep In Me
            If S.ReadSelected = True Then
                ReadSelected.AddExistingStep(S)
            End If
        Next S
    End Function
    Public Function ReadTopSelectedStepList() As GraphicalStepsList
        ReadTopSelectedStepList = New GraphicalStepsList
        For Each S As BaseGraphicalStep In Me
            If S.ReadTopRectSelected = True Then
                ReadTopSelectedStepList.AddExistingStep(S)
            End If
        Next S
    End Function
    Public Function ReadBottomSelectedStepList() As GraphicalStepsList
        ReadBottomSelectedStepList = New GraphicalStepsList
        For Each S As BaseGraphicalStep In Me
            If S.ReadBottomRectSelected = True Then
                ReadBottomSelectedStepList.AddExistingStep(S)
            End If
        Next S
    End Function
    Public Function ReadSelectedStepsActionsList() As ArrayList
        ReadSelectedStepsActionsList = New ArrayList
        For Each S As Object In Me
            If S.IsAStep Then
                If S.ReadSelectedActionsList.Count > 0 Then
                    ReadSelectedStepsActionsList.AddRange(S.ReadSelectedActionsList)
                End If
            End If
        Next S
    End Function
    Public Function FindAndSelectAction(ByVal x As Integer, ByVal y As Integer) As Boolean
        'Seleziona solo la prima azione che trova
        For Each S As Object In Me
            If S.IsAStep Then
                FindAndSelectAction = S.FindAndSelectAction(x, y)
                If FindAndSelectAction Then
                    Exit Function
                End If
            End If
        Next S
    End Function
    Public Function FindAction(ByVal x As Integer, ByVal y As Integer) As Boolean
        For Each S As Object In Me
            If S.IsAStep Then
                FindAction = S.FindAction(x, y)
                If FindAction Then
                    Exit Function
                End If
            End If
        Next S
    End Function
    Public Function ReadIfActionSelected(ByVal x As Integer, ByVal y As Integer) As Boolean
        For Each S As Object In Me
            If S.IsAStep Then
                ReadIfActionSelected = S.ReadIfActionSelected(x, y)
                If ReadIfActionSelected Then
                    Exit Function
                End If
            End If
        Next S
    End Function
    Public Function ReadIfTransitionSelected(ByVal x As Integer, ByVal y As Integer) As Boolean
        'Trova la prima fase in x,y
        For Each T As BaseGraphicalStep In Me
            If T.MyAreaIsHere(x, y) And T.ReadSelected Then
                ReadIfTransitionSelected = True
                Exit Function
            End If
        Next T
    End Function
    Public Sub FindAndSelectActions(ByVal Rect As Drawing.Rectangle)
        'Seleziona tutte le azioni che trova
        For Each S As Object In Me
            If S.IsAStep Then
                S.FindAndSelectActions(Rect)
            End If
        Next S
    End Sub
    Public Function FindAndSelectSmallRectangleStep(ByVal x As Integer, ByVal y As Integer) As Boolean
        For Each S As BaseGraphicalStep In Me
            'Controlla il ret sup
            If S.IsMyTopRect(x, y) Then
                'Inverte il valore selezionato del ret sup
                S.SetTopRectSelected(Not S.ReadTopRectSelected)
                FindAndSelectSmallRectangleStep = True
                Exit Function
            End If
            'Controlla il valore selezionato
            If S.IsMyBotRect(x, y) = True Then
                'Inverte il Value Selected del ret sup
                S.SetBottomRectSelected(Not S.ReadBottomRectSelected)
                FindAndSelectSmallRectangleStep = True
                Exit Function
            End If
        Next S
    End Function
    Public Sub DeSelectAll()
        For Each S As Object In Me
            S.SetSelected(False)
            S.SetBottomRectSelected(False)
            S.SetTopRectSelected(False)
            If S.IsAStep Then
                S.DeSelectActions()
            End If
        Next S
    End Sub
    Public Sub SelezionaAll()
        For Each S As Object In Me
            S.SetSelected(True)
        Next
    End Sub
    Public Sub DrawArea(ByVal Rect As Drawing.Rectangle, ByVal DrawSmallRectangels As Boolean)
        For Each S As BaseGraphicalStep In Me
            'La disegna se si interseca con il rectanglolo da disegnare
            If S.ReadArea.IntersectsWith(Rect) Then
                S.Draw(DrawSmallRectangels)
            End If
        Next S
    End Sub
    Public Sub DrawSelection(ByVal DrawSmallRectangles As Boolean)
        For Each S As BaseGraphicalStep In Me
            'Disegna le fasi selezionate
            If S.ReadSelected Then
                S.Draw(DrawSmallRectangles)
            End If
        Next S
    End Sub
    Public Sub MoveSelection(ByVal dx As Integer, ByVal dy As Integer)
        'Muove le fasi selezionate
        For Each S As BaseGraphicalStep In Me
            If S.ReadSelected Then
                S.Move(dx, dy)
            End If
        Next S
    End Sub

    Public Sub AddActionToSelectedSteps(ByVal actType As enumActionType, _
            ByRef Var As Variable, ByVal vCondition As Condition, _
            ByVal Assignation As String, ByVal actEvent As enumActionEvent)

        For Each S As Object In Me
            If S.ReadSelected And S.IsAStep Then
                CType(S, GraphicalStep).AddAndDrawAction(actType, Var, vCondition, Assignation, actEvent)
            End If
        Next S

    End Sub

    Public Sub AddFcOdToSelectedSteps(ByRef Gr As Grafcet, _
            ByVal Sit As enumSituationType, _
            ByRef Steps As GraphicalStepsList)

        For Each S As Object In Me
            If S.ReadSelected And S.IsAStep Then
                CType(S, GraphicalStep).AddAndDrawFcOd(Gr, Sit, Steps)
            End If
        Next S

    End Sub

    Public Function SetInitialSteps() As Boolean
        'Fa lo switch della variabile Initial per ogni fase selezionata In Mea
        'Restituisce True se ha trovato almeno una fase selezionata
        For Each S As BaseGraphicalStep In Me
            If S.IsAStep And S.ReadSelected Then
                SetInitialSteps = True
                CType(S, GraphicalStep).SetInitial(Not CType(S, GraphicalStep).ReadInitial)
            End If
        Next S
    End Function

    Public Function SetFinalSteps() As Boolean
        'Fa lo switch della variabile Final per ogni fase selezionata In Mea
        'Restituisce True se ha trovato almeno una fase selezionata
        For Each S As BaseGraphicalStep In Me
            If S.IsAStep And S.ReadSelected Then
                SetFinalSteps = True
                CType(S, GraphicalStep).SetFinal(Not CType(S, GraphicalStep).ReadFinal)
            End If
        Next S
    End Function

    Public Sub CancelSelection(ByVal CancelSmallRectangles As Boolean)
        For Each S As BaseGraphicalStep In Me
            If S.ReadSelected Then
                S.Cancel(CancelSmallRectangles)
            End If
        Next S
    End Sub

    Public Function ControllaSelectionFuoriArea(ByVal R As Drawing.Rectangle, ByRef FuoriX As Boolean, ByRef FuoriY As Boolean) As Boolean
        'Controlla che l'area delle Steps selezionate....
        '....non sia fuori da R (se è fuori restituisce True)
        For Each S As BaseGraphicalStep In Me
            If S.ReadSelected Then
                ControllaSelectionFuoriArea = Outside(R, S.ReadArea, FuoriX, FuoriY)
                If ControllaSelectionFuoriArea Then
                    Exit For
                End If
            End If
        Next S
    End Function

    Public Function CountSelected() As Integer
        CountSelected = 0
        For Each S As BaseGraphicalStep In Me
            If S.ReadSelected Then
                CountSelected = CountSelected + 1
            End If
        Next S
    End Function

    Public Function CountSelectedActions() As Integer
        CountSelectedActions = 0
        For Each S As Object In Me
            If S.IsAStep Then
                CountSelectedActions = CountSelectedActions + S.CountSelectedActions()
            End If
        Next S
    End Function

    Public Function ReadStepSelected() As BaseGraphicalStep
        For Each S As BaseGraphicalStep In Me
            If S.ReadSelected Then
                Return S
            End If
        Next S
        Return Nothing
    End Function

    Public Function ReadSelectedAction() As GraphicalOrder
        ReadSelectedAction = Nothing
        For Each S As Object In Me
            If S.IsAStep Then
                ReadSelectedAction = S.ReadSelectedAction
                If Not IsNothing(ReadSelectedAction) Then
                    Exit Function
                End If
            End If
        Next S
    End Function


    Public Sub DeactiveAllSteps()
        'Disattiva tutte le fasi
        For Each S As BaseGraphicalStep In Me
            S.Disactive()
        Next S
    End Sub

    Public Sub ActiveSteps(ByRef StepsList As GraphicalStepsList)
        Dim i As Integer
        For Each S As BaseGraphicalStep In StepsList
            i = IndexOf(S)
            Me(i).Active()
        Next S
    End Sub



    Public Function ReadIfAllStepsActive() As Boolean
        'Le macrofasi devono essere attive (non basta preattive come le fasi)
        ReadIfAllStepsActive = True
        For Each S As BaseGraphicalStep In Me
            If S.GetType.Name = "GraphicalMacroStep" Then
                'E' una macrofase
                If Not S.ReadActive Then
                    ReadIfAllStepsActive = False
                    Exit For
                End If
            Else
                'E' una fase
                If S.IsAStep Then
                    If Not S.ReadActive And Not S.ReadPreActive Then
                        ReadIfAllStepsActive = False
                        Exit For
                    End If
                End If
            End If
        Next S
    End Function

    ' Ritorna una lista delle fasi inserite in questa lista in forma stringa
    Public Function ListSteps() As String
        Dim ret As String = ""
        For Each S As BaseGraphicalStep In Me
            ret += S.Name + ", "
        Next
        ret = ret.TrimEnd(" ").TrimEnd(",")
        Return ret
    End Function

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

        For Each St As BaseGraphicalStep In Me
            St.CompleteDeserialization(refChar, refBackColor, _
                    refSelectionColor, refNotSelectionColor, refTextColor, _
                    refColorActive, refColorPreactive, refColorDeactive, _
                    refColTransitionConditionTrue, refColTransitionConditionFalse)
        Next
    End Sub
End Class

