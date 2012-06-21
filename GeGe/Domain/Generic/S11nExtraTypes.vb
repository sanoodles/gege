'Extra Types for serialization
Public Class S11nExtraTypes

    Public Shared Function GetTypes() As Type()
        Dim r(11) As Type

        'Variable
        r(0) = GetType(BooleanVariable)
        r(1) = GetType(IntegerVariable)
        r(2) = GetType(RealVariable)

        'BaseStep
        r(3) = GetType(GraphicalMacroStep)
        r(4) = GetType(GraphicalStep)
        r(5) = GetType(GraphicalEnclosingStep)
        r(6) = GetType(GraphicalEnclosedStep)
        ''Order
        r(7) = GetType(GraphicalAction)
        r(8) = GetType(GraphicalForcingOrder)

        'Body
        r(9) = GetType(MacroStepBody)
        r(10) = GetType(Grafcet)
        ''Grafcet
        r(11) = GetType(Enclosure)

        Return r
    End Function

End Class
