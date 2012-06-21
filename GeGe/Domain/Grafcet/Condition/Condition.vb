Public Class Condition

    Protected m_BoolExpr As BooleanExpression

    'Design decision: The Condition class is the expert in parsing.
    'A reference to a body is needed to perform lexical analysis.
    Protected m_Body As Body

    Public Overridable ReadOnly Property GetString() As String
        Get
            Return m_BoolExpr.GetExpressionString
        End Get
    End Property

    Public ReadOnly Property GetBoolExpr() As BooleanExpression
        Get
            Return m_BoolExpr
        End Get
    End Property

    'If the kind of condition is unknown, ConditionsManager.CreateCondition 
    'must be called instead
    Public Sub New(ByVal vBoolExp As String, _
            ByRef refBody As Body)

        Try
            m_BoolExpr = New BooleanExpression(refBody.GetGrafcet, vBoolExp)
        Catch e As Exception
            Throw New ArgumentException("Invalid condition")
        End Try
        m_Body = refBody
    End Sub

    Protected Sub New(ByRef refBody As Body)
        m_Body = refBody
    End Sub

    Public Overridable Function IsTimeDependent() As Boolean
        Return False
    End Function

End Class
