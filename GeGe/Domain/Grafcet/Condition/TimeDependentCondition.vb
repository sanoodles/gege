Public Class TimeDependentCondition
    Inherits Condition

    Protected m_TimeAfterRise As Integer
    Protected m_TimeAfterFall As Integer

    Public ReadOnly Property TimeAfterRise() As Integer
        Get
            Return m_TimeAfterRise
        End Get
    End Property

    Public Overrides ReadOnly Property GetString() As String
        Get
            Dim r As String
            r = m_TimeAfterRise.ToString + "s/" + m_BoolExpr.GetExpressionString
            If m_TimeAfterFall > 0 Then r += "/" + m_TimeAfterFall.ToString + "s"
            Return r
        End Get
    End Property

    Public ReadOnly Property TimeAfterFall() As Integer
        Get
            Return m_TimeAfterFall
        End Get
    End Property

    'If the kind of condition is unknown, ConditionsManager.CreateCondition 
    'must be called instead
    'pre: if a time after rise is specified, ConditionString is in format t1/*
    'pre: if a time after fall is specified, ConditionString is in format t1/*/t2
    '@see IEC60848:2002 Table 5 - Continuous actions Nº 23
    Public Sub New(ByVal ConditionString As String, _
            ByRef refBody As Body)

        MyBase.New(refBody)

        Dim csplit() As String

        'time after rise
        csplit = ConditionString.Split("/")
        If csplit.Length < 2 Then
            m_TimeAfterRise = 0
        Else
            Dim Tstring As String = csplit(0)
            m_TimeAfterRise = CInt(Mid(Tstring, 1, Tstring.Length - 1))
        End If

        'timed expression
        If csplit.Length < 2 Then
            m_BoolExpr = Nothing
        Else
            Dim BEstring As String = csplit(1)
            Try
                m_BoolExpr = New BooleanExpression(refBody.GetGrafcet, BEstring)
            Catch e As Exception
                Throw New ArgumentException("Invalid time dependent condition")
            End Try

        End If

        'time after fall
        If csplit.Length < 3 Then
            m_TimeAfterFall = 0
        Else
            Dim Tstring As String = csplit(2)
            m_TimeAfterFall = CInt(Mid(Tstring, 1, Tstring.Length - 1))
        End If

    End Sub

    Public Overrides Function IsTimeDependent() As Boolean
        Return True
    End Function

End Class
