Imports System.Drawing
Imports System.Math
Imports System.Xml
Imports System.Threading
Imports System.Collections
Imports System.Collections.Generic
Imports System.Windows.Forms

Public Class BodyForm
    Inherits System.Windows.Forms.Form

    'view
    Private m_GraphToDraw As Drawing.Graphics
    Private m_monitoring As Boolean
    Private Dimension As Integer
    Private Foglio As Drawing.Rectangle
    Private ColGriglia As Drawing.Color
    Private Br, BrSfondo As Drawing.Brush
    Private OffsetXMove As Integer
    Private OffsetYMove As Integer
    Private NextCondition As Condition
    Private NextElementName As String
    Private NextElementDocumentation As String
    Private NextEnclosedStepActivationLink As Boolean
    Private CurrentOperation As Operation
    Private DistanzaDaSelectionX As Integer
    Private DistanzaDaSelectiony As Integer
    Private MultipleSelection As Boolean
    Private InitialPoint As Drawing.Point
    Private PreviousFinelPoint As Drawing.Point
    Private ColSfondo As Drawing.Color = Drawing.Color.White
    Private TreeOpen As Boolean = False
    Private ShowDetails As Boolean = True

    Private CursorStep As New Cursor(IO.Path.Combine(CursorsPath, "CursorStep.Cur"))
    Private CursorMacroStep As New Cursor(IO.Path.Combine(CursorsPath, "CursorMacroStep.Cur"))
    Private CursorEnclosingStep As New Cursor(IO.Path.Combine(CursorsPath, "CursorEnclosingStep.Cur"))
    Private CursorTransition As New System.Windows.Forms.Cursor(IO.Path.Combine(CursorsPath, "CursorTransition.Cur"))
    Private CursorMove As New Cursor(IO.Path.Combine(CursorsPath, "CursorMove.cur"))

    Private PanelScrollH As Integer
    Private PanelScrollV As Integer

    'use case controller
    Private m_Body As Body
    Private WithEvents m_BodyEventRaiser As BodyEventRaiser
    Private m_Mode As ProjectForm.ProjectMode
    Private m_RC As RunControl
    Private VarCtrl As VariablesGridControl

    Dim WithEvents SollevaEvento As New BooleanVariable

    Private Enum Operation
        DefiningInitialStep
        DefiningStep
        DefiningMacroStep
        DefiningInitialEnclosedStep
        DefiningEnclosedStep
        DefiningEnclosingStep
        DefiningTransition
        Selection
        MultipleSelection
        Selected
    End Enum

    Public ReadOnly Property Body() As Body
        Get
            Return m_Body
        End Get
    End Property

    Public Sub New(ByRef RefB As Body, Optional ByRef RC As RunControl = Nothing)
        MyBase.New()

        m_Body = RefB
        m_BodyEventRaiser = RefB.m_BodyEventRaiser
        CurrentOperation = Operation.Selection
        Foglio.Width = 800
        Foglio.Height = 1200
        BrSfondo = New Drawing.SolidBrush(ColSfondo)
        InitialPoint = New Drawing.Point(0, 0)

        'Chiamata richiesta da ProgettAction Windows Form.
        InitializeComponent()
        m_RC = RC
        VarCtrl = New VariablesGridControl(Grid1, _
                 m_Body.Project, m_Body.GetScope, RC)
        If RC Is Nothing Then
            SetModeEdit()
        Else
            SetModeRun(RC)
        End If

        'canvas
        m_GraphToDraw = Panel1.CreateGraphics
        ShowDetails = True
        m_Body.SetGraphToDraw(m_GraphToDraw)

        'AddHandler Me.KeyPress, AddressOf Panel1_KeyPress
    End Sub

    Public Sub SetModeEdit()
        m_Mode = ProjectForm.ProjectMode.Edit
        VarCtrl.SetModeEdit()
    End Sub

    Public Sub SetModeRun(ByRef RC As RunControl)
        m_Mode = ProjectForm.ProjectMode.Run
        VarCtrl.SetModeRun(RC)
    End Sub



    Private Sub AddStep(ByVal Initial As Boolean)
        ResetCurrentOperation()
        'Cerca il numero della prima fase libera
        'e passa il valore alla finestra di dialogo
        Dim StepDialog As New StepDialogForm
        StepDialog.StepName = m_Body.FirstAvaiableStepName
        Dim ResultDialog As System.Windows.Forms.DialogResult = StepDialog.ShowDialog()
        If ResultDialog = Windows.Forms.DialogResult.OK Then
            'Controlla se il nome della fase è già presente
            If Not IsNothing(m_Body.FindStepByName(StepDialog.StepName)) Then
                MsgBox("Step " & StepDialog.StepName & " already exists", MsgBoxStyle.Critical)
            Else
                NextElementName = StepDialog.StepName
                If Not Initial Then
                    CurrentOperation = Operation.DefiningStep
                Else
                    CurrentOperation = Operation.DefiningInitialStep
                End If
            End If
        End If
        StepDialog.Dispose()
    End Sub

    Private Sub AddEnclosedStep(ByVal Initial As Boolean)
        ResetCurrentOperation()
        'Cerca il numero della prima fase libera
        'e passa il valore alla finestra di dialogo
        Dim StepDialog As New EnclosedStepDialogForm
        StepDialog.StepName = m_Body.FirstAvaiableStepName
        Dim ResultDialog As System.Windows.Forms.DialogResult = StepDialog.ShowDialog()
        If ResultDialog = Windows.Forms.DialogResult.OK Then
            'Controlla se il nome della fase è già presente
            If Not IsNothing(m_Body.FindStepByName(StepDialog.StepName)) Then
                MsgBox("Step " & StepDialog.StepName & " already exists", MsgBoxStyle.Critical)
            Else
                NextElementName = StepDialog.StepName
                NextEnclosedStepActivationLink = StepDialog.ActivationLink
                If Not Initial Then
                    CurrentOperation = Operation.DefiningEnclosedStep
                Else
                    CurrentOperation = Operation.DefiningInitialEnclosedStep
                End If
            End If
        End If
        StepDialog.Dispose()
    End Sub

    Private Sub AddMacroStep()
        ResetCurrentOperation()

        Dim MacroStepDialog As New MacroStepDialogForm
        MacroStepDialog.StepName = m_Body.FirstAvaiableStepName
        MacroStepDialog.Button3.Enabled = False

        Dim ResultDialog As System.Windows.Forms.DialogResult = MacroStepDialog.ShowDialog()

        If ResultDialog = Windows.Forms.DialogResult.OK Then
            If Not IsNothing(m_Body.FindStepByName(MacroStepDialog.StepName)) = True Then
                MsgBox("Step " & MacroStepDialog.StepName & " already exists", MsgBoxStyle.Critical)
            Else
                NextElementName = MacroStepDialog.StepName
                CurrentOperation = Operation.DefiningMacroStep
            End If
        End If

        MacroStepDialog.Dispose()
    End Sub

    Private Sub AddEnclosingStep()
        ResetCurrentOperation()

        Dim ESDialog As New StepDialogForm
        ESDialog.StepName = m_Body.FirstAvaiableStepName

        Dim ResultDialog As System.Windows.Forms.DialogResult = ESDialog.ShowDialog()

        If ResultDialog = Windows.Forms.DialogResult.OK Then
            If Not IsNothing(m_Body.FindStepByName(ESDialog.StepName)) = True Then
                MsgBox("Step " & ESDialog.StepName & " already exists", MsgBoxStyle.Critical)
            Else
                NextElementName = ESDialog.StepName
                CurrentOperation = Operation.DefiningEnclosingStep
            End If
        End If

        ESDialog.Dispose()
    End Sub

    Private Sub AddAction()
        ResetCurrentOperation()
        If m_Body.ReadStepList.ReadSelected.CountStepsList > 0 Then
            Dim ActionDialog As New ActionDialogForm(m_Body.AllVariables)
            Dim ResultDialog As System.Windows.Forms.DialogResult = ActionDialog.ShowDialog()
            If ResultDialog = Windows.Forms.DialogResult.OK Then
                StoreAction(ActionDialog.m_Type, _
                    ActionDialog.m_Variable, ActionDialog.m_ConditionString, _
                    ActionDialog.m_Assignation, ActionDialog.m_Event)
                DrawVisibleArea()
            End If
        Else
            MsgBox("No step selected", MsgBoxStyle.Critical)
        End If
    End Sub

    Private Sub AddForcingOrder()
        ResetCurrentOperation()
        If m_Body.ReadStepList.ReadSelected.CountStepsList > 0 Then
            Dim FODialog As New ForcingOrderDialogForm(m_Body.Project)
            Dim ResultDialog As System.Windows.Forms.DialogResult = FODialog.ShowDialog()
            If ResultDialog = Windows.Forms.DialogResult.OK Then
                StoreForcingOrder(FODialog.m_Grafcet, FODialog.m_Situation, _
                        FODialog.m_SubSteps)
                DrawVisibleArea()
            End If
        Else
            MsgBox("No step selected", MsgBoxStyle.Critical)
        End If
    End Sub

    Private Sub AddTransition()
        ResetCurrentOperation()
        'Cerca il numero della prima transizione libera
        'e passa il valore alla finestra di dialogo
        Dim TransitionDialog As New TransitionDialogForm(m_Body.FirstAvaiableTransitionName, _
                                                         m_Body.GetGrafcet)
        Dim ResultDialog As System.Windows.Forms.DialogResult = TransitionDialog.ShowDialog()
        If ResultDialog = Windows.Forms.DialogResult.OK Then
            NextCondition = TransitionDialog.m_Condition
            CurrentOperation = Operation.DefiningTransition
            DeSelectAll()
            DrawVisibleArea()
        End If
        UpdateTable()
        TransitionDialog.Dispose()
    End Sub

    Private Sub UpdateTable()
        'This function is called when variables list may have been updated (variable created, modified, or deleted)
        'Calls to this function should be removed and the table updating mechanism should be events
        'from the variables list to the listview. Using VariablesGridControl.
        VarCtrl.FillTable()
    End Sub



    Public Function GetBodyName() As String
        Return m_Body.Name
    End Function

    Public Sub setActiveSteps(ByRef stepsList As GraphicalStepsList)
        m_Body.SetActiveSteps(stepsList)
    End Sub

    Private Sub SetInitial()
        If CurrentOperation = Operation.DefiningTransition Then
            m_Body.CancelSelection(True)
        Else
            m_Body.CancelSelection(False)
        End If
        Dim Find As Boolean = m_Body.SetInitialSteps()
        DrawVisibleArea()
        If Not Find Then
            MsgBox("No steps selected", MsgBoxStyle.Information)
        End If
    End Sub

    Private Sub SetFinal()
        If CurrentOperation = Operation.DefiningTransition Then
            m_Body.CancelSelection(True)
        Else
            m_Body.CancelSelection(False)
        End If
        Dim Find As Boolean = m_Body.SetFinalSteps
        DrawVisibleArea()
        If Not Find Then
            MsgBox("No steps selected", MsgBoxStyle.Information)
        End If
    End Sub

    Private Sub RemoveSelectedElements()
        If m_Body.GraphicalStepsList.ReadSelected.CountSelected > 0 Or _
                m_Body.GraphicalStepsList.ReadSelectedStepsActionsList.Count > 0 Or m_Body.GraphicalTransitionsList.ReadSelectedTransitionList.CountSelected > 0 Then

            If m_Body.GraphicalTransitionsList.ControllaPresenzaStepsInTransizioni(m_Body.GraphicalStepsList.ReadSelected) Then
                MsgBox("Steps that are source or destination of a transition can't be deleted", MsgBoxStyle.Critical)
            Else
                If My.Computer.Keyboard.ShiftKeyDown OrElse (MsgBox("Do you really want to delete the selected elements?", MsgBoxStyle.OkCancel Or MsgBoxStyle.Question) = MsgBoxResult.Ok) Then
                    m_Body.RemoveSelectedElements()
                    CancelVisibleArea()
                    DrawVisibleArea()
                End If
                CurrentOperation = Operation.Selection
            End If
        Else
            MsgBox("Nothing selected to delete", MsgBoxStyle.Information)
        End If
    End Sub

    Private Sub WriteTitlePanel()
        Me.Text = m_Body.Name
    End Sub

    Private Sub StartMonitor()
        m_Body.StartStateMonitor()
    End Sub

    Private Sub StopMonitor()
        m_Body.StopStateMonitor()
        CancelVisibleArea()
        DrawVisibleArea()
    End Sub

    Public Function ReadBody() As Body
        Return m_Body
    End Function

    Public Sub ResetCurrentOperation()
        CurrentOperation = Operation.Selection
        CancelVisibleArea()
        DrawVisibleArea()
    End Sub

    Private Function StoreStep(ByVal P As Drawing.Point, _
             ByVal Initial As Boolean) As Boolean
        StoreStep = m_Body.AddAndDrawStep(m_Body.FirstAvaiableElementNumber, _
                NextElementName, NextElementDocumentation, P, Initial)
        DrawVisibleArea()
    End Function

    Private Function StoreEnclosedStep(ByVal P As Drawing.Point, _
             ByVal Initial As Boolean, _
             ByVal ActivationLink As Boolean) As Boolean
        StoreEnclosedStep = m_Body.AddAndDrawEnclosedStep(m_Body.FirstAvaiableElementNumber, _
                NextElementName, NextElementDocumentation, P, Initial, ActivationLink)
        DrawVisibleArea()
    End Function

    Private Function StoreMacroStep(ByVal P As Drawing.Point) As Boolean
        Return m_Body.AddAndDrawMacroStep(m_Body.FirstAvaiableElementNumber, NextElementName, NextElementDocumentation, P)
    End Function

    Private Function StoreEnclosingStep(ByVal P As Drawing.Point) As Boolean
        Return m_Body.AddAndDrawEnclosingStep(m_Body.FirstAvaiableElementNumber, NextElementName, NextElementDocumentation, P)
    End Function

    Private Sub ConfirmAddTransition()
        If StepsSelectedForTransitionOk() Then
            StoreTransition()
            CurrentOperation = Operation.Selection
            CancelVisibleArea()
            DeSelectAll()
            DrawVisibleArea()
        Else
            MsgBox("You must select source and destination steps before adding a transition" + vbCrLf + "If you choose more than one source and one destination, divergences and convergences shall be created as appropriate", MsgBoxStyle.Information)
        End If
    End Sub

    Private Function StepsSelectedForTransitionOk() As Boolean
        If m_Body.GraphicalStepsList.ReadBottomSelectedStepList.CountStepsList > 0 _
                And m_Body.GraphicalStepsList.ReadTopSelectedStepList.CountStepsList > 0 Then
            Return True
        End If
        Return False
    End Function

    Private Function StoreTransition() As Boolean
        Dim tCount As Integer = m_Body.GraphicalTransitionsList.Count + 1
        m_Body.AddAndDrawTransition(m_Body.FirstAvaiableElementNumber, _
                m_Body.FirstAvaiableTransitionName(), _
                NextElementDocumentation, _
                NextCondition)
    End Function

    Private Sub StoreAction(ByVal actType As enumActionType, _
            ByRef Var As Variable, ByVal vConditionString As String, _
            ByVal Assignation As String, ByVal actEvent As enumActionEvent)
        m_Body.AddActionToSelectedSteps(actType, Var, vConditionString, _
                Assignation, actEvent)
    End Sub

    Private Sub StoreForcingOrder(ByRef Gr As Grafcet, ByVal Sit As enumSituationType, _
            ByRef Steps As GraphicalStepsList)
        m_Body.AddFcOdToSelectedSteps(Gr, Sit, Steps)
    End Sub

    Private Sub DrawMultipleSelectionRectangle(ByVal R As Drawing.Rectangle)
        Dim Penna As New Drawing.Pen(Color.Black)
        Penna.DashStyle = Drawing2D.DashStyle.Dot
        Try
            If Monitor.TryEnter(m_GraphToDraw, 2000) Then
                m_GraphToDraw.DrawRectangle(Penna, R)
            End If
        Catch ex As System.Exception
        Finally
            Monitor.Exit(m_GraphToDraw)
        End Try
    End Sub

    Private Sub CancelMultipleSelectionRectangle(ByVal R As Drawing.Rectangle)
        Dim Penna As New Drawing.Pen(ColSfondo)
        Try
            If Monitor.TryEnter(m_GraphToDraw, 2000) Then
                m_GraphToDraw.DrawRectangle(Penna, R)
            End If
        Catch ex As System.Exception
        Finally
            Monitor.Exit(m_GraphToDraw)
        End Try
    End Sub

    Private Sub FindAndSelectElementsArea(ByVal Rect As Drawing.Rectangle)
        m_Body.FindAndSelectElementsArea(Rect)
    End Sub

    Private Sub DrawArea(ByVal Rect As Drawing.Rectangle)
        Rect.X = Rect.X - 1
        Rect.Y = Rect.Y - 1
        Rect.Width = Rect.Width + 2
        Rect.Height = Rect.Height + 2
        If CurrentOperation = Operation.DefiningTransition Then
            m_Body.DrawElementsArea(Rect, True)
        Else
            m_Body.DrawElementsArea(Rect, False)
        End If
    End Sub

    Private Sub DrawVisibleArea()
        Dim R As New Drawing.Rectangle(0, 0, Panel1.Width, Panel1.Height)
        DrawArea(R)
    End Sub

    Private Sub CancelVisibleArea()
        Try
            If Monitor.TryEnter(m_GraphToDraw, 2000) Then
                m_GraphToDraw.Clear(ColSfondo)
            End If
        Catch ex As System.Exception
        Finally
            Monitor.Exit(m_GraphToDraw)
        End Try

    End Sub

    Private Sub CancelSelection()
        m_Body.CancelSelection(True)

    End Sub

    Public Sub DeSelectAll()
        m_Body.DeSelectAll()
    End Sub

    Private Sub MoveSelection(ByVal dx As Integer, ByVal dy As Integer)
        CancelSelection()
        m_Body.MoveSelection(dx, dy)
        DrawVisibleArea()
    End Sub

    ' Impostiamo la dimensione del pannello ad almeno 1200*1200 pixels, ma
    ' se ci sono oggetti più in basso o più a destra di questo limite, aumentiamo
    ' la dimensione del pannello in modo da consentire di mostrare tutto il diagramma
    Private Sub InitialPanelSizing()
        Dim rightMost As IGraphicalObject = m_Body.GetRightmostObject
        Dim lowerMost As IGraphicalObject = m_Body.GetLowermostObject
        Dim myWantedSize As New Drawing.Size(1200, 1200)
        Dim rmX As Integer = -1
        If rightMost IsNot Nothing Then rmX = rightMost.Position.X + rightMost.Size.Width
        Dim lmY As Integer = -1
        If lowerMost IsNot Nothing Then lmY = lowerMost.Position.Y + lowerMost.Size.Height
        rmX = (110 * rmX) / 100 ' 110%
        lmY = (110 * lmY) / 100 ' 110%
        myWantedSize.Width = Math.Max(myWantedSize.Width, rmX)
        myWantedSize.Height = Math.Max(myWantedSize.Height, lmY)
        Me.ResizePanel(myWantedSize)
    End Sub

    Private Sub LoadButtonImages()
        ToolStrip1.ImageList = ImageList1
        With ToolStrip1
            .Items(0).Image = .ImageList.Images(10) 'new ini step
            .Items(1).Image = .ImageList.Images(6) 'new step
            .Items(2).Image = .ImageList.Images(9) 'new macrostep
            .Items(3).Image = .ImageList.Images(21) 'new enclosing step
            .Items(4).Image = .ImageList.Images(20) 'new forcing order
            .Items(5).Image = .ImageList.Images(12) 'new action
            .Items(6).Image = .ImageList.Images(5) 'new transition
            '.Items(7) separator
            .Items(8).Image = .ImageList.Images(15) 'set fin
            .Items(9).Image = .ImageList.Images(11) 'cancel
            '.Items(10) separator
            .Items(11).Image = .ImageList.Images(16) 'cursor
        End With
    End Sub

    'Private Sub BodyForm_KeyPress(ByVal sender As Object, _
    '       ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles Me.KeyPress
    '    If e.KeyChar = Microsoft.VisualBasic.ChrW(Keys.Escape) Then
    '        ResetCurrentOperation()
    '    End If
    'End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles MyBase.Load
        WriteTitlePanel()

        LoadButtonImages()

        ' Attiva la modalità "Monitoraggio" all'apertura della finestra
        Button1_Click(sender, e)

        ' Mostra i commenti sin dall'inizio
        Call Me.InitialPanelSizing()

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Select Case m_monitoring
            Case False
                StartMonitor()
                m_monitoring = True
            Case True
                StopMonitor()
                m_monitoring = False
        End Select
        'Button1.BackColor = CType(m_MonitorStatus(m_monitoring)("color"), Color)
    End Sub

    Private Sub Panel1_Paint(ByVal sender As System.Object, _
            ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel1.Paint
        DrawArea(e.ClipRectangle)
    End Sub

    Private Sub Panel1_MouseDown(ByVal sender As Object, _
            ByVal e As System.Windows.Forms.MouseEventArgs) Handles Panel1.MouseDown
        Me.Activate()

        If m_Mode = ProjectForm.ProjectMode.Edit Then

            Select Case e.Button

                Case Windows.Forms.MouseButtons.Left



                    Select Case CurrentOperation

                        'Initial step
                        Case Is = Operation.DefiningInitialStep
                            Dim P As New Drawing.Point(Drawer.Snap(e.X), Drawer.Snap(e.Y))
                            'Chiama AddAndDrawStep
                            If StoreStep(P, True) Then  'quando ha fatto setta a true
                                CurrentOperation = Operation.Selection
                            End If

                            'Step
                        Case Is = Operation.DefiningStep
                            Dim P As New Drawing.Point(Drawer.Snap(e.X), Drawer.Snap(e.Y))
                            'Chiama AddAndDrawStep
                            If StoreStep(P, False) Then  'quando ha fatto setta a true
                                CurrentOperation = Operation.Selection
                            End If

                            'Initial enclosed-step
                        Case Is = Operation.DefiningInitialEnclosedStep
                            Dim P As New Drawing.Point(Drawer.Snap(e.X), Drawer.Snap(e.Y))
                            'Chiama AddAndDrawStep
                            If StoreEnclosedStep(P, True, NextEnclosedStepActivationLink) Then  'quando ha fatto setta a true
                                CurrentOperation = Operation.Selection
                            End If

                            'Enclosed-step
                        Case Is = Operation.DefiningEnclosedStep
                            Dim P As New Drawing.Point(Drawer.Snap(e.X), Drawer.Snap(e.Y))
                            'Chiama AddAndDrawStep
                            If StoreEnclosedStep(P, False, NextEnclosedStepActivationLink) Then  'quando ha fatto setta a true
                                CurrentOperation = Operation.Selection
                            End If

                            'Macro-step
                        Case Is = Operation.DefiningMacroStep
                            Dim P As New Drawing.Point(Drawer.Snap(e.X), Drawer.Snap(e.Y))
                            'Chiama AddAndDrawMacroStep
                            If StoreMacroStep(P) Then
                                CurrentOperation = Operation.Selection
                            End If

                            'Enclosing step
                        Case Is = Operation.DefiningEnclosingStep
                            Dim P As New Drawing.Point(Drawer.Snap(e.X), Drawer.Snap(e.Y))
                            'Chiama AddAndDrawEnclosingStep
                            If StoreEnclosingStep(P) Then
                                CurrentOperation = Operation.Selection
                            End If

                            'Transition
                        Case Is = Operation.DefiningTransition
                            m_Body.FindAndSelectSmallRectangleStep(e.X, e.Y)
                            DrawVisibleArea()

                            'Selection
                        Case Is = Operation.Selection
                            Dim Found As Boolean
                            If MultipleSelection Then
                                Found = m_Body.FindAndSelectElement(e.X, e.Y)
                            Else
                                Found = m_Body.FindElement(e.X, e.Y)
                                If Found Then
                                    'Ha trovato una Step o una Transition o un Action
                                    Dim GiaSelected As Boolean = m_Body.ReadIfElementIsSelected(e.X, e.Y)
                                    If Not GiaSelected Then
                                        DeSelectAll()
                                        m_Body.FindAndSelectElement(e.X, e.Y)
                                    End If
                                End If
                            End If

                            If Found Then
                                'Ha trovato l'elemento e prepara per la selezione
                                CurrentOperation = Operation.Selected
                                DrawVisibleArea()

                            Else
                                'Non ha trovato elementi e prepara per la selezione multipla
                                CurrentOperation = Operation.MultipleSelection
                                DeSelectAll()
                            End If

                            InitialPoint.X = e.X
                            InitialPoint.Y = e.Y
                            DrawVisibleArea()

                    End Select



                Case Windows.Forms.MouseButtons.Middle, _
                        Windows.Forms.MouseButtons.Right

                    Select Case CurrentOperation
                        Case Operation.DefiningTransition
                            ConfirmAddTransition()
                    End Select

            End Select
        Else
            'Non è in modo di editing quindi seleziona solo la macrofasi per aprirle
            m_Body.FindAndSelectElement(e.X, e.Y)
        End If
    End Sub

    Private Sub Panel1_MouseUp(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.MouseEventArgs) Handles Panel1.MouseUp

        If m_Mode = ProjectForm.ProjectMode.Edit Then
            Select Case CurrentOperation
                Case Operation.MultipleSelection
                    CurrentOperation = Operation.Selection
                    DeSelectAll()
                    FindAndSelectElementsArea(CreaRectangle(InitialPoint.X, InitialPoint.Y, e.X, e.Y))
                    Dim R As Drawing.Rectangle = CreaRectangle(InitialPoint.X, InitialPoint.Y, PreviousFinelPoint.X, PreviousFinelPoint.Y)
                    CancelMultipleSelectionRectangle(R)
                    DrawArea(R)
                Case Operation.Selected
                    CurrentOperation = Operation.Selection
                    'DrawVisibleArea()
            End Select
        End If

        ' elimina le sbavature presenti su alcuni sistemi
        Panel1.Refresh()
    End Sub

    Private Sub Panel1_MouseMove(ByVal sender As Object, _
            ByVal e As System.Windows.Forms.MouseEventArgs) Handles Panel1.MouseMove

        If m_Mode = ProjectForm.ProjectMode.Edit Then

            Select Case CurrentOperation

                Case Operation.Selected
                    Panel1.Cursor = CursorMove
                    'Effettua lo Movemento solo se dopo lo snap....
                    '....è maggiore di 0
                    Dim dx As Integer = Drawer.Snap(e.X - InitialPoint.X)
                    Dim dy As Integer = Drawer.Snap(e.Y - InitialPoint.Y)
                    If dx <> 0 Or dy <> 0 Then
                        Dim R As New Drawing.Rectangle(-dx, -dy, Foglio.Width - dx, Foglio.Height - dy)
                        Dim FuoriX, FuoriY As Boolean
                        'Simula l'area del foglio Moveta nei versi opposti
                        If Not m_Body.ControllaSelectionFuoriArea(R, FuoriX, FuoriY) Then
                            'Move la Selection se ci riesce
                            MoveSelection(dx, dy)
                            InitialPoint.X = Drawer.Snap(e.X)
                            InitialPoint.Y = Drawer.Snap(e.Y)
                        Else
                            If Not FuoriX And dx <> 0 Then
                                'Move la Selection solo di dx se ci riesce
                                MoveSelection(dx, 0)
                                InitialPoint.X = Drawer.Snap(e.X)
                            End If
                            If Not FuoriY And dy <> 0 Then
                                'Move la Selection solo di dy se ci riesce
                                MoveSelection(0, dy)
                                InitialPoint.Y = Drawer.Snap(e.Y)
                            End If
                        End If
                    End If

                Case Operation.DefiningInitialStep
                    Panel1.Cursor = CursorStep

                Case Operation.DefiningStep
                    Panel1.Cursor = CursorStep

                Case Operation.DefiningInitialEnclosedStep
                    Panel1.Cursor = CursorStep

                Case Operation.DefiningEnclosedStep
                    Panel1.Cursor = CursorStep

                Case Operation.DefiningMacroStep
                    Panel1.Cursor = CursorMacroStep

                Case Operation.DefiningEnclosingStep
                    Panel1.Cursor = CursorEnclosingStep

                Case Operation.DefiningTransition
                    Panel1.Cursor = CursorTransition

                Case Operation.MultipleSelection
                    CancelMultipleSelectionRectangle(CreaRectangle(InitialPoint.X, InitialPoint.Y, PreviousFinelPoint.X, PreviousFinelPoint.Y))
                    DrawArea(CreaRectangle(InitialPoint.X, InitialPoint.Y, PreviousFinelPoint.X, PreviousFinelPoint.Y))
                    DrawMultipleSelectionRectangle(CreaRectangle(InitialPoint.X, InitialPoint.Y, e.X, e.Y))
                    PreviousFinelPoint.X = e.X
                    PreviousFinelPoint.Y = e.Y

                Case Else
                    Panel1.Cursor = System.Windows.Forms.Cursors.Default

            End Select
        End If
    End Sub

    Private Sub Panel1_DoubleClick(ByVal sender As Object, _
            ByVal e As System.EventArgs) Handles Panel1.DoubleClick

        If m_Mode = ProjectForm.ProjectMode.Edit Then
            If CurrentOperation = Operation.Selected And _
                    m_Body.CountSelectedElement = 1 And _
                    Not MultipleSelection Then

                Dim Obj As Object = m_Body.ReadObjectSelected

                Select Case Obj.GetType.Name

                    Case "GraphicalStep"
                        Dim StepDialog As New StepDialogForm
                        StepDialog.StepName = Obj.Name

                        Dim ResultDialog As System.Windows.Forms.DialogResult = StepDialog.ShowDialog()
                        If ResultDialog = Windows.Forms.DialogResult.OK Then
                            'Controlla se il mome della fase è già presente e se il numero è stato cambiato
                            If StepDialog.StepName <> Obj.Name Then
                                If Not IsNothing(m_Body.FindStepByName(StepDialog.StepName)) Then
                                    MsgBox("Step " & StepDialog.StepName & " already exists", MsgBoxStyle.Critical)
                                Else
                                    'Update name
                                    Obj.Name = StepDialog.StepName
                                End If
                            End If
                        End If
                        StepDialog.Dispose()

                    Case "GraphicalEnclosedStep"
                        Dim StepDialog As New EnclosedStepDialogForm
                        StepDialog.StepName = Obj.Name
                        StepDialog.ActivationLink = Obj.ActivationLink

                        Dim ResultDialog As System.Windows.Forms.DialogResult = StepDialog.ShowDialog()
                        If ResultDialog = Windows.Forms.DialogResult.OK Then
                            'Controlla se il mome della fase è già presente e se il numero è stato cambiato
                            If StepDialog.StepName <> Obj.Name Then
                                If Not IsNothing(m_Body.FindStepByName(StepDialog.StepName)) Then
                                    MsgBox("Step " & StepDialog.StepName & " already exists", MsgBoxStyle.Critical)
                                Else
                                    'Update name
                                    Obj.Name = StepDialog.StepName
                                End If
                            End If
                            'Update other fields
                            Obj.ActivationLink = StepDialog.ActivationLink
                        End If
                        StepDialog.Dispose()

                    Case "GraphicalMacroStep"
                        Dim MacroStepDialog As New MacroStepDialogForm
                        MacroStepDialog.StepName = Obj.Name

                        Dim ResultDialog As System.Windows.Forms.DialogResult = MacroStepDialog.ShowDialog()
                        If ResultDialog = Windows.Forms.DialogResult.OK Or ResultDialog = Windows.Forms.DialogResult.Yes Then
                            'Controlla se il nome della fase è già presente e se il numero è stato cambiato
                            If MacroStepDialog.StepName <> Obj.Name Then
                                If Not IsNothing(m_Body.FindStepByName(MacroStepDialog.StepName)) Then
                                    MsgBox("Step " & MacroStepDialog.StepName & " already exists", MsgBoxStyle.Critical)
                                Else
                                    'Update name
                                    Obj.Name = MacroStepDialog.StepName
                                End If
                            End If
                            If ResultDialog = Windows.Forms.DialogResult.Yes Then
                                'Mostra il body se è stato premuto Show body (Resituisce Yes)
                                ShowBody(Obj.ReadSubBody)
                            End If
                        End If
                        MacroStepDialog.Dispose()

                    Case "GraphicalEnclosingStep"
                        Dim ESForm As New EnclosingStepForm(Obj, m_RC, Me.MdiParent)
                        ESForm.MdiParent = Me.MdiParent
                        ESForm.Show()

                    Case "GraphicalTransition"
                        Dim TransitionDlgForm As New TransitionDialogForm(Obj.name, m_Body.GetGrafcet)
                        TransitionDlgForm.m_Condition = Obj.ReadCondition

                        Dim ResultDialog As System.Windows.Forms.DialogResult = TransitionDlgForm.ShowDialog()
                        If ResultDialog = Windows.Forms.DialogResult.OK Then
                            'Controlla se il nome della transizione è già presente se il numero è stato cambiato
                            If TransitionDlgForm.m_Name <> Obj.Name Then
                                'Controlla se il nome non è gia presente
                                If Not IsNothing(m_Body.FindTransitionByName(TransitionDlgForm.m_Name)) Then
                                    MsgBox("Transition " & TransitionDlgForm.m_Name & " already exists", MsgBoxStyle.Critical)
                                Else
                                    'Update name
                                    Obj.Name = TransitionDlgForm.m_Name
                                End If
                            End If
                            'Update other fields
                            Obj.SetCondition(TransitionDlgForm.m_Condition)
                        End If
                        UpdateTable()
                        TransitionDlgForm.Dispose()

                    Case "GraphicalAction"
                        Dim ActionDlgForm As New ActionDialogForm(m_Body.AllVariables, Obj)
                        Dim ResultDialog As System.Windows.Forms.DialogResult = ActionDlgForm.ShowDialog()
                        If ResultDialog = Windows.Forms.DialogResult.OK Then
                            CType(Obj, GraphicalAction).actType = ActionDlgForm.m_Type
                            CType(Obj, GraphicalAction).Variable = ActionDlgForm.m_Variable
                            CType(Obj, GraphicalAction).Condition = _
                                    m_Body.CreateCondition(ActionDlgForm.m_ConditionString)
                            CType(Obj, GraphicalAction).Assignation = ActionDlgForm.m_Assignation
                            CType(Obj, GraphicalAction).actEvent = ActionDlgForm.m_Event
                        End If

                    Case "GraphicalForcingOrder"
                        Dim ForcingOrderDlgForm As New ForcingOrderDialogForm(m_Body.Project, Obj)
                        Dim ResultDialog As System.Windows.Forms.DialogResult = ForcingOrderDlgForm.ShowDialog()
                        If ResultDialog = Windows.Forms.DialogResult.OK Then
                            CType(Obj, GraphicalForcingOrder).Grafcet = ForcingOrderDlgForm.m_Grafcet
                            CType(Obj, GraphicalForcingOrder).Situation = ForcingOrderDlgForm.m_Situation
                            CType(Obj, GraphicalForcingOrder).SubSteps = ForcingOrderDlgForm.m_SubSteps
                        End If

                End Select

                CurrentOperation = Operation.Selection
                CancelVisibleArea()
                DrawVisibleArea()
            End If

        Else
            'Non è in modo di editing quindi apre solo le macrofasi
            'Controlla se è selezionata
            If m_Body.CountSelectedElement = 1 Then
                Dim Obj As Object = m_Body.ReadObjectSelected
                Select Case Obj.GetType.Name
                    Case "GraphicalMacroStep"
                        ShowBody(Obj.ReadSubBody)
                        m_Body.DeSelectAll()
                End Select

            End If
        End If
    End Sub

    Private Sub ShowBody(ByRef RefBody As Body)
        Dim Found As Boolean
        For Each Fr As System.Windows.Forms.Form In Me.MdiParent.MdiChildren
            If Fr.GetType.Name = "BodyForm" Then
                Dim GrFrm As BodyForm = Fr
                If GrFrm.ReadBody.Name = RefBody.Name Then
                    Fr.Activate()
                    Found = True
                    Exit For
                End If
            End If
        Next Fr

        'Se non lo ha trovato lo crea e lo visualizza
        If Not Found Then
            Dim GrFrmTemp As New BodyForm(RefBody, m_RC)
            GrFrmTemp.MdiParent = Me.MdiParent
            GrFrmTemp.Show()
        End If

    End Sub

    Private Sub Panel1_KeyPress(ByVal sender As Object, _
            ByVal e As KeyPressEventArgs) Handles Me.KeyPress
        'MsgBox("hola")
        If e.KeyChar = Microsoft.VisualBasic.ChrW(Keys.Escape) Then
            ResetCurrentOperation()
        End If
    End Sub

    Private Sub BodyForm_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        If Not IsNothing(m_GraphToDraw) Then
            Try
                If Monitor.TryEnter(m_GraphToDraw, 2000) Then
                    Try
                        m_GraphToDraw = Panel1.CreateGraphics
                        If Monitor.TryEnter(m_GraphToDraw, 4000) Then
                            m_GraphToDraw.Clear(Color.White)
                            m_Body.SetGraphToDraw(m_GraphToDraw)
                            Monitor.Exit(m_GraphToDraw)
                            Me.DrawVisibleArea()
                        End If
                    Catch ex As System.Exception
                        Monitor.Exit(m_GraphToDraw)
                    End Try
                End If
            Catch ex As System.Exception
                If Not IsNothing(m_GraphToDraw) Then
                    Monitor.Exit(m_GraphToDraw)
                End If
            End Try
        End If
    End Sub

    Private Sub BodyForm_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        'there is no Panel.KeyDown event. how to achieve same effect? this neither works.
        'update: now it works. keydown was being captured by button1. now button1 has been deleted.
        Select Case CurrentOperation
            Case Operation.DefiningTransition
                If e.KeyCode = 13 Then
                    ConfirmAddTransition()
                End If
            Case Operation.Selection
                If e.KeyCode = 46 Then
                    CancelSelection()
                End If
            Case Operation.Selection
                If e.KeyCode = Keys.ControlKey Or e.KeyCode = Keys.ShiftKey Then
                    MultipleSelection = True
                End If
        End Select
    End Sub

    Private Sub BodyForm_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyUp
        Select Case CurrentOperation
            Case Operation.Selection
                If e.KeyCode = Keys.ControlKey Or e.KeyCode = Keys.ShiftKey Then
                    MultipleSelection = False
                End If
        End Select
    End Sub

    Private Function CreaRectangle(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer) As Drawing.Rectangle
        Dim a, b, c, d As Integer
        If x1 < x2 Then
            a = x1
            c = x2
        Else
            a = x2
            c = x1
        End If
        If y1 < y2 Then
            b = y1
            d = y2
        Else
            b = y2
            d = y1
        End If
        CreaRectangle = New Drawing.Rectangle(a, b, c - a, d - b)
    End Function

    Private Sub BodyForm_Closing(ByVal sender As Object, _
            ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing

        'Entra cou un monitor in m_GraphToDraw per aspettare che l'sfc finisca il disegno dello stato
        Monitor.Enter(m_GraphToDraw)
        m_Body.StopStateMonitor()
        Monitor.Exit(m_GraphToDraw)
        Me.Dispose()
    End Sub

    Public Sub RefreshDraw()
        Me.DrawVisibleArea()
    End Sub

    Private Sub ResizePanel(ByVal newSize As Size)
        Panel1.Size = newSize
        Foglio.Width = newSize.Width
        Foglio.Height = newSize.Height
        Try
            If Monitor.TryEnter(m_GraphToDraw, 2000) Then
                Try
                    m_GraphToDraw = Panel1.CreateGraphics
                    If Monitor.TryEnter(m_GraphToDraw, 4000) Then
                        m_GraphToDraw.Clear(Color.White)
                        m_Body.SetGraphToDraw(m_GraphToDraw)
                        Monitor.Exit(m_GraphToDraw)
                        Me.DrawVisibleArea()
                    End If
                Catch ex As System.Exception
                    Monitor.Exit(m_GraphToDraw)
                End Try
            End If
        Catch ex As System.Exception
            If Not IsNothing(m_GraphToDraw) Then
                Monitor.Exit(m_GraphToDraw)
            End If
        End Try
    End Sub

    Private Sub btnEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'Dim iComm As IDocumentable = GetCurrentIDocumentable()
        'If iComm Is Nothing Then Return
        'Dim commEditorWnd As New CommentsDialogForm(iComm)
        'If commEditorWnd.ShowDialog() = Windows.Forms.DialogResult.OK Then
        'iComm.Documentation = commEditorWnd.CommentTypedIn
        'End If
    End Sub

    ' Crea il menu per l'editor variabili
    'Private Sub toolBar1_ButtonDropDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolBarButtonClickEventArgs)
    '    If e.Button Is Me.VariablesMenu Then
    'Dim myMenu As ContextMenu = New ContextMenu()
    ''myMenu.MenuItems.Add(m_Grafcet.PouInterface.localVars.GetMenu())
    '        If Not m_Grafcet.ResGlobalVariables Is Nothing Then
    '            For Each VL As VariablesList In m_Grafcet.ResGlobalVariables
    '                If VL Is Nothing Then Continue For
    '                myMenu.MenuItems.Add(VL.GetMenu())
    '            Next
    '        End If
    '        e.Button.DropDownMenu = myMenu
    '    End If
    'End Sub

    ' Esporta il diagramma in formato JPEG (utile per le tesine di TSA)
    Private Sub ExportAsJPEG(ByVal destFile As String)
        Dim imgSize As New Rectangle(Foglio.X, Foglio.Y, _
            Foglio.Width, Foglio.Height)
        Dim img As New Bitmap(imgSize.Width, imgSize.Height)
        Dim imgGraphics As Graphics = _
            Graphics.FromImage(img)
        imgGraphics.FillRectangle(Brushes.White, _
            imgSize)
        m_Body.PrintMe(imgGraphics, _
            imgSize)
        img.Save(destFile, Imaging.ImageFormat.Jpeg)
    End Sub

    Private Sub MenuItem6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        SetInitial()
    End Sub

    Private Sub MenuItem7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        SetFinal()
    End Sub

    Private Sub MenuItem10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ResetCurrentOperation()
    End Sub

    Private Sub AddstepToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddstepToolStripMenuItem.Click

        Select Case m_Body.GetType.Name
            Case "Enclosure"
                AddEnclosedStep(False)
            Case Else
                AddStep(False)
        End Select

    End Sub

    Private Sub AddactionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddactionToolStripMenuItem.Click
        AddAction()
    End Sub

    Private Sub AdddToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AdddToolStripMenuItem.Click
        AddTransition()
    End Sub

    Private Sub DeleteSelectedElementsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteSelectedElementsToolStripMenuItem.Click
        RemoveSelectedElements()
    End Sub

    Private Sub ResizeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResizeToolStripMenuItem.Click
        Dim resizeDialog As New ResizePanelDialogForm(Panel1.Size)
        If resizeDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then _
            ResizePanel(resizeDialog.NewSize)
        resizeDialog.Dispose()
    End Sub

    Private Sub SaveAsJPEGToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveAsJPEGToolStripMenuItem.Click
        If sdgJPEGFile.ShowDialog() = Windows.Forms.DialogResult.OK Then _
            ExportAsJPEG(sdgJPEGFile.FileName)
    End Sub



    'Private Sub BodyForm_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseWheel
    '    PanelScrollV += e.Delta
    '    Me.AutoScrollPosition = New Point(0, PanelScrollV)
    'End Sub

    Private Sub NewIniStepButton_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles NewIniStepButton.Click

        Select Case m_Body.GetType.Name
            Case "Enclosure"
                AddEnclosedStep(True)
            Case Else
                AddStep(True)
        End Select

    End Sub

    Private Sub NewStepButton_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles NewStepButton.Click

        Select Case m_Body.GetType.Name
            Case "Enclosure"
                AddEnclosedStep(False)
            Case Else
                AddStep(False)
        End Select

    End Sub

    Private Sub NewMacroStepButton_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles NewMacroStepButton.Click
        AddMacroStep()
    End Sub

    Private Sub EnclosingStepButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EnclosingStepButton.Click
        AddEnclosingStep()
    End Sub

    Private Sub NewActionButton_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles NewActionButton.Click
        AddAction()
    End Sub

    Private Sub NewForcingOrderButton_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles NewForcingOrderButton.Click
        AddForcingOrder()
    End Sub

    Private Sub NewTransButton_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles NewTransButton.Click
        AddTransition()
    End Sub

    Private Sub SetFinButton_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles SetFinButton.Click
        SetFinal()
    End Sub

    Private Sub DelButton_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles DelButton.Click
        RemoveSelectedElements()
    End Sub

    Private Sub ShowCursor_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles ShowCursor.Click
        ResetCurrentOperation()
    End Sub

    Private Sub SetValueToolStripMenuItem_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles SetValueToolStripMenuItem.Click
        VarCtrl.SetVarValue()
    End Sub

    Private Sub NewVariableToolStripMenuItem_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles NewVariableToolStripMenuItem.Click
        VarCtrl.CreateNewVariable(Me)
    End Sub

    Private Sub DataGridView1_CellDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles Grid1.CellDoubleClick
        'MsgBox(Grid1.CurrentCell.RowIndex.ToString + " " + Grid1.CurrentCell.ColumnIndex.ToString)

        Dim CurCol As Integer
        CurCol = Grid1.CurrentCell.ColumnIndex

        If CurCol = VariablesGridControl.COL_CURRENT_VALUE Then
            VarCtrl.SetVarValue()
        Else
            If VarCtrl.GetMode = ProjectForm.ProjectMode.Edit Then VarCtrl.ModifyVariable()
        End If
    End Sub


    Private Sub BodyDisposingHandler() Handles m_BodyEventRaiser.Disposing
        Me.Dispose()
    End Sub



End Class

