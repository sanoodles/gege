Public Class Action

    Protected m_actType As enumActionType
    Protected WithEvents m_Var As Variable
    Protected m_Condition As Condition
    Protected m_Assignation As String
    Protected m_actEvent As enumActionEvent

    Public Event NameChangingInVariable(ByVal Name As String)
    Public Event NameChangedInVariable(ByVal Name As String)



    Public Property actType() As enumActionType
        Get
            Return m_actType
        End Get
        Set(ByVal value As enumActionType)
            m_actType = value
        End Set
    End Property

    Public Property Var() As Variable
        Get
            Return m_Var
        End Get
        Set(ByVal value As Variable)
            m_Var = value
        End Set
    End Property

    Public Property Cond() As Condition
        Get
            Return m_Condition
        End Get
        Set(ByVal value As Condition)
            m_Condition = value
        End Set
    End Property

    Public Property Assignation() As String
        Get
            Return m_Assignation
        End Get
        Set(ByVal value As String)
            m_Assignation = value
        End Set
    End Property

    Public Property actEvent() As enumActionEvent
        Get
            Return m_actEvent
        End Get
        Set(ByVal value As enumActionEvent)
            m_actEvent = value
        End Set
    End Property



    Public Sub New(ByVal actType As enumActionType, _
            ByRef Var As Variable, ByVal refCondition As Condition, _
            ByVal Assignation As String, ByVal actEvent As enumActionEvent)

        m_actType = actType
        m_Var = Var
        m_Condition = refCondition
        m_Assignation = Assignation
        m_actEvent = actEvent
    End Sub

    Public Function IsTimed() As Boolean
        Return m_Condition.IsTimeDependent
    End Function

    Public Function BoolExpr() As BooleanExpression
        Return m_Condition.GetBoolExpr
    End Function



    Private Sub NameChangingInVariableHandler(ByVal Name As String) Handles m_Var.NameChanging
        RaiseEvent NameChangingInVariable(Name)
    End Sub

    Private Sub NameChangedInVariableHandler(ByVal Name As String) Handles m_Var.NameChanged
        RaiseEvent NameChangedInVariable(Name)
    End Sub

End Class
