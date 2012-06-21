Public Class ConditionsManager

    'Factory method pattern
    Public Shared Function CreateCondition( _
            ByVal vConditionString As String, _
            ByRef refBody As Body)

        If vConditionString Is Nothing Then vConditionString = ""

        Dim r As Condition = Nothing

        If vConditionString.Contains("/") Then
            r = New TimeDependentCondition(vConditionString, refBody)
        Else
            r = New Condition(vConditionString, refBody)
        End If

        Return r
    End Function

End Class
