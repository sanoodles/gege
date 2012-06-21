Imports System.IO
Imports System.Threading
Imports System.Xml

<Serializable()> _
Public MustInherit Class Body

    'gui
    Protected m_Char As Drawing.Font = New Drawing.Font("Arial", 8)
    Protected m_BackColor As Drawing.Color = Drawing.Color.White
    Protected m_SelectionColor As Drawing.Color = _
            Drawing.Color.FromArgb(242, 72, 69) 'Rojo Valentino.
    'Protected m_SelectionColor As Drawing.Color = Drawing.Color.Red 'Rojo chusma.
    Protected m_NotSelectionColor As Drawing.Color = Drawing.Color.Black
    Protected m_TextColor As Drawing.Color = Drawing.Color.Black
    Protected m_ColorActive As Drawing.Color = Drawing.Color.Red
    Protected m_ColorPreactive As Drawing.Color = Drawing.Color.Pink
    Protected m_ColorDeactive As Drawing.Color = m_BackColor
    Protected m_ColTransitionConditionTrue As Drawing.Color = Drawing.Color.Green
    Protected m_ColTransitionConditionFalse As Drawing.Color = Drawing.Color.Brown
    Protected m_FlagShowDetails As Boolean = True
    Protected m_StateMonitor As Boolean
    <NonSerialized()> Protected m_GraphToDraw As Drawing.Graphics
    Protected m_Dimension As Integer = 44

    'domain
    Protected m_Name As String
    Protected m_SubStepsList As GraphicalStepsList
    Protected m_TransitionsList As GraphicalTransitionsList

    'persistence
    Protected m_XmlSimultaneousConvergences As XmlSimultaneousConvergences
    Protected m_XmlSimultaneousDivergences As XmlSimultaneousDivergences
    Protected m_XmlSelectionConvergences As XmlSelectionConvergences
    Protected m_XmlSelectionDivergences As XmlSelectionDivergences

    Public Event StartScan()
    Public Event EndScan()
    'Public Event Disposing()
    <NonSerialized()> Public m_BodyEventRaiser As BodyEventRaiser



    Public MustOverride ReadOnly Property Project() As Project
    Public MustOverride ReadOnly Property ProjectVariables() As VariablesList
    Public MustOverride ReadOnly Property GrafcetVariables() As VariablesList

    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(ByVal Value As String)
            m_Name = Value
        End Set
    End Property

    Public ReadOnly Property AllVariables() As VariablesList
        Get
            Dim res As New VariablesList
            res.AddRange(ProjectVariables)
            res.AddRange(GrafcetVariables)
            Return res
        End Get
    End Property

    Public Property GraphicalStepsList() As GraphicalStepsList
        Get
            Return m_SubStepsList
        End Get
        Set(ByVal Value As GraphicalStepsList)
            m_SubStepsList = Value
        End Set
    End Property

    Public Property GraphicalTransitionsList() As GraphicalTransitionsList
        Get
            Return m_TransitionsList
        End Get
        Set(ByVal Value As GraphicalTransitionsList)
            m_TransitionsList = Value
        End Set
    End Property

    Public Property XmlSimultaneousConvergences() As XmlSimultaneousConvergences
        Get
            Return m_XmlSimultaneousConvergences
        End Get
        Set(ByVal Value As XmlSimultaneousConvergences)
            m_XmlSimultaneousConvergences = Value
        End Set
    End Property

    Public Property XmlSimultaneousDivergences() As XmlSimultaneousDivergences
        Get
            Return m_XmlSimultaneousDivergences
        End Get
        Set(ByVal Value As XmlSimultaneousDivergences)
            m_XmlSimultaneousDivergences = Value
        End Set
    End Property

    Public Property XmlSelectionConvergences() As XmlSelectionConvergences
        Get
            Return m_XmlSelectionConvergences
        End Get
        Set(ByVal Value As XmlSelectionConvergences)
            m_XmlSelectionConvergences = Value
        End Set
    End Property

    Public Property XmlSelectionDivergences() As XmlSelectionDivergences
        Get
            Return m_XmlSelectionDivergences
        End Get
        Set(ByVal Value As XmlSelectionDivergences)
            m_XmlSelectionDivergences = Value
        End Set
    End Property




    Public MustOverride Function CreateInstance()

    'Grafcets have their own scope only if they are not the expansion of a macro-step
    '(TODO: or the enclosed steps of an enclosing step.)
    '@return Scope of the grafcet.
    Public MustOverride Function GetScope() As String

    Public MustOverride Function GetGrafcet() As Grafcet


    Protected Sub New(ByVal Name As String)

        m_Name = Name

        m_SubStepsList = New GraphicalStepsList(Me, m_BackColor, _
                m_SelectionColor, m_NotSelectionColor, m_TextColor, m_Char, _
                m_ColorActive, m_ColorDeactive, m_ColorPreactive, m_GraphToDraw, _
                m_Dimension)

        m_TransitionsList = New GraphicalTransitionsList(Me, m_BackColor, _
                m_SelectionColor, m_NotSelectionColor, m_TextColor, m_Char, _
                m_ColTransitionConditionTrue, m_ColTransitionConditionFalse, _
                m_ColorDeactive, m_ColorDeactive, m_ColorPreactive, m_GraphToDraw, _
                m_Dimension)

        m_BodyEventRaiser = New BodyEventRaiser

    End Sub

    Public Sub New(ByVal Name As String, _
            ByRef RefResGlobalVariables As VariablesList, _
            ByRef GrapStepsList As GraphicalStepsList, _
            ByRef GrapTransitionsList As GraphicalTransitionsList)

        m_Name = Name
        m_SubStepsList = GrapStepsList
        m_TransitionsList = GrapTransitionsList
    End Sub

    Public Sub New()
        m_SubStepsList = New GraphicalStepsList
        m_TransitionsList = New GraphicalTransitionsList
    End Sub

    Public Overridable Function IsAEnclosure() As Boolean
        Return False
    End Function



    Public Sub SetActiveSteps(ByRef refList As GraphicalStepsList)
        'disactive all
        DisactiveAllSteps()

        'active some
        For Each St As BaseGraphicalStep In refList
            St.Active()
        Next
    End Sub

    Public Sub DisposeMe()     'Distruttore
        'RaiseEvent Disposing()
        m_BodyEventRaiser.RaiseDisposing()
        m_SubStepsList.DisposeMe()
        m_TransitionsList.DisposeMe()
        Me.Finalize()
    End Sub

    Public Sub SetGraphToDraw(ByRef Graph As Drawing.Graphics) 'Implements IIEC61131LanguageImplementation.SetGraphToDraw
        m_GraphToDraw = Graph
        m_SubStepsList.SetGraphToDraw(m_GraphToDraw)
        m_TransitionsList.SetGraphToDraw(m_GraphToDraw)
    End Sub

    Public Function RemoveSelectedElements() As Boolean
        'Tenta di entrare nell'sfc
        Try
            If Monitor.TryEnter(Me, 2000) Then
                m_SubStepsList.RemoveSelectedElements()
                m_TransitionsList.RemoveSelectedElements()
                Monitor.Exit(Me)
                RemoveSelectedElements = True
            End If
        Catch ex As System.Exception
            Monitor.Exit(Me)
            RemoveSelectedElements = False
        End Try
    End Function

    Public Sub DeSelectAll()
        m_SubStepsList.DeSelectAll()
        m_TransitionsList.DeSelectAll()
    End Sub

    Public Function FindElementByNumber(ByVal n As Integer) As Object
        'Cerca tra le fasi
        FindElementByNumber = m_SubStepsList.FindStepByNumber(n)
        If IsNothing(FindElementByNumber) Then
            'Cerca tra le transizioni
            FindElementByNumber = m_TransitionsList.FindTransitionByNumber(n)
        End If
    End Function

    Public Function FindStepByNumber(ByVal n As Integer) As BaseGraphicalStep
        FindStepByNumber = m_SubStepsList.FindStepByNumber(n)
    End Function

    Public Function FindStepByName(ByVal Value As String) As BaseGraphicalStep
        For Each S As BaseGraphicalStep In m_SubStepsList
            If S.Name = Value Then
                Return S
                Exit For
            End If
        Next S
        Return Nothing
    End Function

    Public Function FirstAvaiableStepName() As String
        FirstAvaiableStepName = ""
        Dim i As Integer
        Dim Find As Boolean
        For i = 0 To m_SubStepsList.Count
            Find = False
            For Each S As BaseGraphicalStep In m_SubStepsList
                If S.Name = "S" & i Then
                    Find = True
                    Exit For
                End If
            Next S
            If Not Find Then
                FirstAvaiableStepName = "S" & i
                Exit For
            End If
        Next i
    End Function

    Public Function FindTransitionByName(ByVal Value As String) As GraphicalTransition
        For Each T As GraphicalTransition In m_TransitionsList
            If T.Name = Value Then
                Return T
            End If
        Next T
        Return Nothing
    End Function

    Public Function FirstAvaiableTransitionName() As String
        ' Potrebbe non essere davvero il primo nome disponibile
        ' (ma troverà comunque un nome valido)
        ' L'algoritmo precedente falliva se c'erano più transizioni che fasi e ritornava
        ' nomi inizianti con "S" invece che con "T"
        Dim num As Integer = Me.GraphicalTransitionsList.Count + 1
LoopIn:
        For Each T As GraphicalTransition In Me.GraphicalTransitionsList
            If Me.Name.Equals("T" + num.ToString()) Then
                num += 1
                GoTo LoopIn
            End If
        Next
        Return "T" + num.ToString()
    End Function

    Public Function FirstAvaiableElementNumber() As Integer
        Dim j As Integer = 0
        Dim NumberAvaiable As Boolean
        While j < m_SubStepsList.Count + m_TransitionsList.Count + 1
            j = j + 1
            NumberAvaiable = True
            If IsNothing(m_SubStepsList.FindStepByNumber(j)) And IsNothing(m_TransitionsList.FindTransitionByNumber(j)) Then
                Exit While
            End If
        End While
        FirstAvaiableElementNumber = j
    End Function

    Public Function ControllaPresenzaTransition(ByVal n As Integer) As Boolean
        ControllaPresenzaTransition = m_TransitionsList.ControllaPresenzaTransition(n)
    End Function

    Public Function AddAndDrawStep(ByVal Number As Integer, _
            ByVal Name As String, ByVal Documentation As String, _
            ByVal Position As Drawing.Point, _
            Optional ByVal Initial As Boolean = False) As Boolean

        'Tenta di entrare nell'sfc e nel GraphToDraw
        Try
            If Monitor.TryEnter(Me, 2000) Then
                m_SubStepsList.AddAndDrawStep(Number, Name, Documentation, _
                        Position, m_StateMonitor, Initial)
                AddAndDrawStep = True
                Monitor.Exit(Me)
            End If
        Catch ex As System.Exception
            Monitor.Exit(Me)
            AddAndDrawStep = False
        End Try
    End Function

    Public Function AddAndDrawEnclosedStep(ByVal Number As Integer, _
            ByVal Name As String, ByVal Documentation As String, _
            ByVal Position As Drawing.Point, _
            Optional ByVal Initial As Boolean = False, _
            Optional ByVal ActivationLink As Boolean = False) As Boolean

        'Tenta di entrare nell'sfc e nel GraphToDraw
        Try
            If Monitor.TryEnter(Me, 2000) Then
                m_SubStepsList.AddAndDrawEnclosedStep(Number, Name, Documentation, _
                        Position, m_StateMonitor, Initial, ActivationLink)
                AddAndDrawEnclosedStep = True
                Monitor.Exit(Me)
            End If
        Catch ex As System.Exception
            Monitor.Exit(Me)
            AddAndDrawEnclosedStep = False
        End Try
    End Function

    Public Function AddAndDrawMacroStep(ByVal Number As Integer, ByVal Name As String, ByVal Documentation As String, ByVal Position As Drawing.Point) As Boolean
        'Tenta di entrare nell'sfc e nel GraphToDraw
        Try
            If Monitor.TryEnter(Me, 2000) Then
                m_SubStepsList.AddAndDrawMacroStep(Number, Name, Documentation, ProjectVariables, Position, m_StateMonitor)
                AddAndDrawMacroStep = True
                Monitor.Exit(Me)
            End If
        Catch ex As System.Exception
            Monitor.Exit(Me)
            AddAndDrawMacroStep = False
        End Try
    End Function

    Public Function AddAndDrawEnclosingStep(ByVal Number As Integer, ByVal Name As String, ByVal Documentation As String, ByVal Position As Drawing.Point, Optional ByVal Initial As Boolean = False) As Boolean
        'Tenta di entrare nell'sfc e nel GraphToDraw
        Try
            If Monitor.TryEnter(Me, 2000) Then
                m_SubStepsList.AddAndDrawEnclosingStep(Number, Name, Documentation, Position, m_StateMonitor, Initial)
                AddAndDrawEnclosingStep = True
                Monitor.Exit(Me)
            End If
        Catch ex As System.Exception
            Monitor.Exit(Me)
            AddAndDrawEnclosingStep = False
        End Try
    End Function

    Public Function AddAndDrawTransition(ByVal Number As Integer, _
            ByVal Name As String, ByVal Documentation As String, _
            ByRef RefCondition As Condition) As Boolean
        'Tenta di entrare nell'sfc e nel GraphToDraw
        Try
            If Monitor.TryEnter(Me, 2000) Then
                'Crea la transizione inviando la lista delle fasi selezionate superiormente....
                '....e la lista delle fasi selezionate inferiormente leggendole dalla lista delle fasi
                m_TransitionsList.AddAndDrawTransition(Number, Name, _
                        Documentation, m_SubStepsList.ReadBottomSelectedStepList, _
                        m_SubStepsList.ReadTopSelectedStepList, RefCondition, _
                        m_FlagShowDetails, m_StateMonitor)
                Monitor.Exit(Me)
                AddAndDrawTransition = True
            End If
        Catch ex As System.Exception
            Monitor.Exit(Me)
            AddAndDrawTransition = False
        End Try
    End Function

    Public Function AddActionToSelectedSteps(ByVal actType As enumActionType, _
            ByRef Var As Variable, ByVal vConditionString As String, _
            ByVal Assignation As String, ByVal actEvent As enumActionEvent) As Boolean

        Try
            If Monitor.TryEnter(Me, 2000) Then
                Dim c As Condition = ConditionsManager.CreateCondition(vConditionString, Me)
                m_SubStepsList.AddActionToSelectedSteps(actType, Var, c, Assignation, actEvent)
                Monitor.Exit(Me)
                AddActionToSelectedSteps = True
            End If
        Catch ex As System.Exception
            Monitor.Exit(Me)
            AddActionToSelectedSteps = False
        End Try

    End Function

    Public Function AddFcOdToSelectedSteps(ByRef Gr As Grafcet, _
            ByVal Sit As enumSituationType, _
            ByRef Steps As GraphicalStepsList) As Boolean

        Try
            If Monitor.TryEnter(Me, 2000) Then
                m_SubStepsList.AddFcOdToSelectedSteps(Gr, Sit, Steps)
                Monitor.Exit(Me)
                Return True
            End If
        Catch ex As System.Exception
            Monitor.Exit(Me)
            Return False
        End Try

    End Function

    Public Sub ShowDetails(ByVal value As Boolean)
        m_FlagShowDetails = value
    End Sub

    Protected Function FindAndSelectStep(ByVal x As Integer, ByVal y As Integer) As Boolean
        FindAndSelectStep = m_SubStepsList.FindAndSelectStep(x, y)
    End Function

    Protected Function FindAndSelectTransition(ByVal x As Integer, ByVal y As Integer) As Boolean
        FindAndSelectTransition = m_TransitionsList.FindAndSelectTransition(x, y)
    End Function

    Protected Function FindAndSelectAction(ByVal x As Integer, ByVal y As Integer) As Boolean
        'Cerca tra le Actions
        FindAndSelectAction = m_SubStepsList.FindAndSelectAction(x, y)
    End Function

    Public Sub FindAndSelectElementsArea(ByVal Rect As Drawing.Rectangle)
        m_SubStepsList.FindAndSelectSteps(Rect)
        m_TransitionsList.FindAndSelectTransitions(Rect)
        m_SubStepsList.FindAndSelectActions(Rect)
    End Sub

    Public Function FindAndSelectElement(ByVal x As Integer, ByVal y As Integer) As Boolean
        'Cerca tra le transizioni
        FindAndSelectElement = m_TransitionsList.FindAndSelectTransition(x, y)
        If Not FindAndSelectElement Then
            'Cerca tra le fasi
            FindAndSelectElement = m_SubStepsList.FindAndSelectStep(x, y)
            If Not FindAndSelectElement Then
                'Cerca tra le azioni
                FindAndSelectElement = m_SubStepsList.FindAndSelectAction(x, y)
            End If
        End If
    End Function

    Public Function FindElement(ByVal x As Integer, ByVal y As Integer) As Boolean
        FindElement = m_TransitionsList.FindTransition(x, y)
        If Not FindElement Then
            FindElement = m_SubStepsList.FindStep(x, y)
            If Not FindElement Then
                FindElement = m_SubStepsList.FindAction(x, y)
            End If
        End If
    End Function

    Public Function ReadIfElementIsSelected(ByVal x As Integer, ByVal y As Integer) As Boolean
        ReadIfElementIsSelected = m_SubStepsList.ReadIfStepSelected(x, y)
        If Not ReadIfElementIsSelected Then
            ReadIfElementIsSelected = m_TransitionsList.ReadIfTransitionSelected(x, y)
            If Not ReadIfElementIsSelected Then
                ReadIfElementIsSelected = m_SubStepsList.ReadIfActionSelected(x, y)
            End If
        End If
    End Function

    Public Function FindAndSelectSmallRectangleStep(ByVal x As Integer, ByVal y As Integer) As Boolean
        FindAndSelectSmallRectangleStep = m_SubStepsList.FindAndSelectSmallRectangleStep(x, y)
    End Function

    Public Sub DrawElementsArea(ByVal Rect As Drawing.Rectangle, ByVal DrawSmallRectangels As Boolean)
        Try
            If Monitor.TryEnter(Me, 2000) Then
                m_SubStepsList.DrawArea(Rect, DrawSmallRectangels)
                m_TransitionsList.DrawArea(Rect, m_FlagShowDetails)
                Monitor.Exit(Me)
            End If
        Catch ex As System.Exception
            Monitor.Exit(Me)
        End Try
    End Sub

    Public Function ReadStepList() As GraphicalStepsList
        'Legge solo le fasi
        ReadStepList = New GraphicalStepsList
        For Each S As BaseGraphicalStep In GraphicalStepsList
            If S.IsAStep Then
                ReadStepList.Add(S)
            End If
        Next S
    End Function

    Public Function ReadMacroStepList() As GraphicalStepsList
        'Legge solo le macrofasi
        ReadMacroStepList = New GraphicalStepsList
        For Each S As BaseGraphicalStep In GraphicalStepsList
            If S.GetType.Name = "GraphicalMacroStep" Then
                ReadStepList.Add(S)
            End If
        Next S
    End Function

    Public Function SetInitialSteps() As Boolean
        SetInitialSteps = m_SubStepsList.SetInitialSteps()
    End Function

    Public Function SetFinalSteps() As Boolean
        SetFinalSteps = m_SubStepsList.SetFinalSteps()
    End Function

    Public Function CancelSelection(ByVal CancellSmallRectangels As Boolean) As Boolean
        'Cancella su GraphToDraw gli elementi selezionati
        'Tenta di entrare nell'sfc e nel GraphToDraw
        Try
            If Monitor.TryEnter(Me, 2000) Then
                m_SubStepsList.CancelSelection(CancellSmallRectangels)
                m_TransitionsList.CancellTransitionWithSelectedSteps(m_FlagShowDetails)
                m_TransitionsList.CancellSelection(m_FlagShowDetails)
                CancelSelection = True
                Monitor.Exit(Me)
            End If
        Catch ex As System.Exception
            Monitor.Exit(Me)
            CancelSelection = False
        End Try
    End Function

    Public Sub MoveSelection(ByVal dx As Integer, ByVal dy As Integer)
        m_SubStepsList.MoveSelection(dx, dy)
        m_TransitionsList.MoveSelection(dx)
    End Sub

    Public Function ControllaSelectionFuoriArea(ByVal R As Drawing.Rectangle, ByRef FuoriX As Boolean, ByRef FuoriY As Boolean) As Boolean
        'Verifica se gli Elements Selected sono fuori dal Rectangle R
        ControllaSelectionFuoriArea = m_SubStepsList.ControllaSelectionFuoriArea(R, FuoriX, FuoriY) Or m_TransitionsList.ControllaSelectionFuoriArea(R, FuoriX, FuoriY)
    End Function

    Public Sub DrawSelection(ByVal DrawSmallRectangels As Boolean)
        'Disegna su GraphToDraw gli elementi selezionati
        'Tenta di entrare nell'sfc e nel GraphToDraw
        Try
            If Monitor.TryEnter(Me, 1000) Then
                m_SubStepsList.DrawSelection(DrawSmallRectangels)
                m_TransitionsList.DrawSelection(m_FlagShowDetails)
                Monitor.Exit(Me)
            End If
        Catch ex As System.Exception
            Monitor.Exit(Me)
        End Try
    End Sub

    Public Function CountSelectedElement() As Integer
        CountSelectedElement = m_SubStepsList.CountSelected
        CountSelectedElement = CountSelectedElement + m_TransitionsList.CountSelected
        CountSelectedElement = CountSelectedElement + m_SubStepsList.CountSelectedActions
    End Function

    Public Function ReadObjectSelected() As Object
        ReadObjectSelected = m_SubStepsList.ReadStepSelected
        If IsNothing(ReadObjectSelected) Then
            ReadObjectSelected = m_TransitionsList.ReadTransitionSelected
            If IsNothing(ReadObjectSelected) Then
                ReadObjectSelected = m_SubStepsList.ReadSelectedAction
            End If
        End If
    End Function

    Public Sub StartStateMonitor()
        m_StateMonitor = True
        Try
            If Monitor.TryEnter(Me, 2000) Then
                m_SubStepsList.SetDrawState(True)
                m_TransitionsList.SetDrawState(True)
                'Disegna lo stato
                m_SubStepsList.DrawStepsState()
                m_TransitionsList.DrawTransitionsState()
                Monitor.Exit(Me)
            End If
        Catch ex As System.Exception
            Monitor.Exit(Me)
        End Try
    End Sub

    Public Sub StopStateMonitor()
        m_StateMonitor = False
        m_SubStepsList.SetDrawState(False)
        m_TransitionsList.SetDrawState(False)
    End Sub



    Public Sub PrintMe(ByRef Graph As Drawing.Graphics, ByVal Rect As Drawing.Rectangle) 'Implements IIEC61131LanguageImplementation.PrintMe
        Dim OldGraphToDraw As Drawing.Graphics = Nothing
        If Not IsNothing(m_GraphToDraw) Then
            OldGraphToDraw = m_GraphToDraw
        End If
        Dim AreaToDraw As New Drawing.Rectangle(0, 0, Rect.Width, Rect.Height)
        SetGraphToDraw(Graph)
        DrawElementsArea(AreaToDraw, False)
        ' ??? sembra inutile e ridondante
        'If Not IsNothing(m_GraphToDraw) Then
        'SetGraphToDraw(OldGraphToDraw)
        'End If
        SetGraphToDraw(OldGraphToDraw)
    End Sub

    'Public Function FindElementByLocalId(ByVal id As Integer) As IHasLocalId 'Implements IIEC61131LanguageImplementation.FindElementByLocalId
    ' Dim ret As IHasLocalId = m_GraphicalStepsList.FindStepByNumber(id)
    '     If ret Is Nothing Then ret = m_GraphicalTransitionsList.FindTransitionByNumber(id)
    '    Return ret
    'End Function

    ' Ritorna l'oggetto più a destra del diagramma
    Public Function GetRightmostObject() As IGraphicalObject
        Dim mostX As Integer = -1
        Dim mostXObject As IGraphicalObject = Nothing
        For Each GO As IGraphicalObject In GraphicalStepsList
            If GO.Position.X > mostX Then
                mostX = GO.Position.X
                mostXObject = GO
            End If
        Next
        Return mostXObject
    End Function

    ' Ritorna l'oggetto più in basso del diagramma
    Public Function GetLowermostObject() As IGraphicalObject
        Dim mostY As Integer = -1
        Dim mostYObject As IGraphicalObject = Nothing
        For Each GO As IGraphicalObject In GraphicalStepsList
            If GO.Position.Y > mostY Then
                mostY = GO.Position.Y
                mostYObject = GO
            End If
        Next
        Return mostYObject
    End Function

    Public Function GetTransitionsByPreviousStep(ByRef St As BaseGraphicalStep) As GraphicalTransitionsList
        Return m_TransitionsList.GetTransitionsByPreviousStep(St)
    End Function

    Public Function GetExitStep() As GraphicalStep
        Dim r As GraphicalStep = Nothing

        For Each St As BaseGraphicalStep In GraphicalStepsList
            If GetTransitionsByPreviousStep(St).Count = 0 Then
                r = CType(St, GraphicalStep)
                Exit For
            End If
        Next

        Return r
    End Function

    Public Function GetInitialSteps() As GraphicalStepsList
        Dim r As New GraphicalStepsList

        For Each St As BaseGraphicalStep In GraphicalStepsList
            If St.IsAStep Then
                If CType(St, GraphicalStep).Initial Then
                    r.Add(St)
                End If
            End If
        Next

        Return r
    End Function

    Private Sub DisactiveAllSteps()
        For Each St As BaseGraphicalStep In m_SubStepsList
            If St.IsAStep Then
                St.Disactive()
            ElseIf St.GetType.Name = "GraphicalMacroStep" Then
                CType(St, GraphicalMacroStep).ReadSubBody.DisactiveAllSteps()
            End If
        Next
    End Sub

    Public Sub CompleteDeserialization()
        m_SubStepsList.CompleteDeserialization(m_Char, m_BackColor, _
                m_SelectionColor, m_NotSelectionColor, m_TextColor, _
                m_ColorActive, m_ColorPreactive, m_ColorDeactive, _
                m_ColTransitionConditionTrue, m_ColTransitionConditionFalse)

    End Sub

    Public Function CreateCondition(ByVal vConditionString As String) As Condition
        Return ConditionsManager.CreateCondition(vConditionString, Me)
    End Function

End Class
