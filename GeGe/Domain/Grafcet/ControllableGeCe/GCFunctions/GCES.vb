Public Class GCES
    Inherits GCFunctions

    Public Sub New(ByRef c As GCFunctionsController)
        MyBase.New(c)
    End Sub

    Public Function GetES() As String
        Dim r As String 'result

        r = GetIncludes() + e

        r += GetLecturaEntrades() + e + _
             GetLecturaEntradesFisiques() + e + _
             GetSortidesFisiques()

        Return r
    End Function

    Private Function GetLecturaEntrades() As String
        Dim r As String 'result

        r = "void LecturaEntrades(void)" + e + _
             "{" + e + _
             t + "LecturaEntradesFisiques();" + e + _
             e

        '' mòdul "Busca Flancs"
        'per detectar els flancs de pujada
        r += t + "FlancP.all = EntradaDig.all & ~EntradaDigAux.all;" + e
        'pels de baixada
        r += t + "FlancB.all = ~EntradaDig.all & EntradaDigAux.all;" + e

        'flancs d'altres variables, com ara una comparació respecte d'una entrada analògica
        'TODO

        'mòdul "Recorda les entrades"
        r += t + "EntradaDigAux.all = EntradaDig.all;" + e

        r += "}" + e

        Return r
    End Function

    Private Function GetLecturaEntradesFisiques() As String
        Dim r As String

        r = "void LecturaEntradesFisiques(void)" + e + _
            "{" + e

        'primer, es llegeixen les entrades digitals, però abans s'han de
        'posar totes a zero
        r += t + "EntradaDig.all = 0x0000;" + e
        For Each Var As KeyValuePair(Of BooleanVariable, Integer) In m_gcmap.digitalInputToBit
            r += t + "if (" + Var.Key.Name.ToString + ") {" + e
            r += t + t + "EntradaDig.bits.bit" + Var.Value.ToString + " = 1;" + e
            r += t + "}" + e
        Next

        'per a cada entrada analògica
        'TODO

        r += "}" + e

        Return r
    End Function

    Private Function GetSortidesFisiques() As String
        Dim r As String

        r = "void SortidesFisiques(void)" + e + _
            "{" + e

        For Each Va As KeyValuePair(Of Integer, BooleanVariable) In m_gcmap.bitToDigitalOutput
            r += t + "if (SortidaDig.bits.bit" + Va.Key.ToString + ") {" + e
            r += t + t + Va.Value.Name + " = 1;" + e
            r += t + "} else {" + e
            r += t + t + Va.Value.Name + " = 0;" + e
            r += t + "}" + e
        Next

        r += "}" + e

        Return r
    End Function
End Class
