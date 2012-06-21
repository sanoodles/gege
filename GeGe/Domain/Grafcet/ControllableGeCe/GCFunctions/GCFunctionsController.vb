'Since a class with all methods needed to generate the subroutines 
'specified by GeCé would be too big, it has been splitted into
'several, smaller classes, which are called by this class.
'
'This class abstracts the user classes (only GCwithRC in fact)
'from the knowledge of which is the particular (smaller) class
'that provides each service.
'
'@see: http://upcommons.upc.edu/pfc/bitstream/2099.1/2731/1/36670-1.pdf
Public Class GCFunctionsController

    Public Const t As String = "    " 'tab = 4 spaces
    Public Const e As String = vbCrLf 'enter

    Public m_gcmap As GCMapping
    Public m_TimerPeriod As Integer

    Public Sub New(ByRef m As GCMapping, ByVal TimerPeriod As Integer)
        m_gcmap = m
        m_TimerPeriod = TimerPeriod
    End Sub

    'sections:
    ' includes
    ' constants
    ' variables
    ' functions

    '
    '@section includes
    '
    Public Function GetIncludes() As String
        Dim r As String 'result

        r = "#include ""bit.h""" + e + _
            "#include ""parametres.h""" + e + _
            "#include ""main.h""" + e

        Return r
    End Function

    '
    '@section constants
    '
    Public Function GetBitH() As String
        Dim r As String 'result

        'constants per accedir als bits de les variables
        r = "#define BIT0  0x0001" + e + _
            "#define BIT1  0x0002" + e + _
            "#define BIT2  0x0004" + e + _
            "#define BIT3  0x0008" + e + _
            "#define BIT4  0x0010" + e + _
            "#define BIT5  0x0020" + e + _
            "#define BIT6  0x0040" + e + _
            "#define BIT7  0x0080" + e + _
            "#define BIT8  0x0100" + e + _
            "#define BIT9  0x0200" + e + _
            "#define BIT10 0x0400" + e + _
            "#define BIT11 0x0800" + e + _
            "#define BIT12 0x1000" + e + _
            "#define BIT13 0x2000" + e + _
            "#define BIT14 0x4000" + e + _
            "#define BIT15 0x8000" + e + _
            e

        'i un bit per fer-lo córrer.
        'r += "#define BIT 0x0001" + e + e
        'chillón 2010: this constant should have another name
        'moreover, it is not needed if you don't automate timer management

        'declaració de les unions i estructures de bits
        r += "struct BITS" + e + _
             "{" + e + _
             t + "unsigned bit0 : 1;" + e + _
             t + "unsigned bit1 : 1;" + e + _
             t + "unsigned bit2 : 1;" + e + _
             t + "unsigned bit3 : 1;" + e + _
             t + "unsigned bit4 : 1;" + e + _
             t + "unsigned bit5 : 1;" + e + _
             t + "unsigned bit6 : 1;" + e + _
             t + "unsigned bit7 : 1;" + e + _
             t + "unsigned bit8 : 1;" + e + _
             t + "unsigned bit9 : 1;" + e + _
             t + "unsigned bit10 : 1;" + e + _
             t + "unsigned bit11 : 1;" + e + _
             t + "unsigned bit12 : 1;" + e + _
             t + "unsigned bit13 : 1;" + e + _
             t + "unsigned bit14 : 1;" + e + _
             t + "unsigned bit15 : 1;" + e + _
             "};" + e + e

        r += "union BIT" + e + _
             "{" + e + _
             t + "unsigned int all; // variable de 16 bits" + e + _
             t + "struct BITS bits; // estructura de 16 variables binàries" + e + _
             "};" + e

        Return r
    End Function

    Public Function GetParametresH() As String
        Dim r As String 'result

        'constants dimensionals 
        r = "#define nGRAFCET " + m_gcmap.numToGrafcet.Count.ToString + e
        r += "#define nETAPES " + m_gcmap.nETAPES.ToString + e
        r += "#define nTempP " + m_gcmap.nTempP.ToString + e
        r += "#define nTempPB " + m_gcmap.nTempPB.ToString + e

        'constants de jerarquia etapes i valors
        For Each Gr As KeyValuePair(Of Integer, Grafcet) In m_gcmap.numToGrafcet
            r += "#define " + Gr.Value.Name + " " + Gr.Key.ToString + e
        Next
        For Each En As KeyValuePair(Of Enclosure, Integer) In m_gcmap.encToEncIni
            r += "#define ENC_INI_" + En.Key.Name + " " + Hexa.IntToHex(En.Value) + e
        Next
        r += "#define N 100" + e

        Return r
    End Function

    '
    '@section variables
    '
    Public Function GetMainH() As String
        Dim r As String 'result

        r = ""

        'declaració de l'estructura dels grafcets
        r += "struct grafcetGENERAL" + e + _
             "{" + e + _
             t + "unsigned char Forcat;     // per forçar el grafcet. " + e + _
             t + "union BIT EtapaActiva;    // estat futur de les entrades" + e + _
             t + "union BIT EtapaActivaAux; // estat anterior de les etapes" + e + _
             t + "union BIT FlagIniciEtapa; // si l'etapa s'acaba d'activar" + e + _
             t + "union BIT FlagFiEtapa;    // si l'etapa s'acaba de desactivar" + e + _
             t + "union BIT Receptivitat;   // estat de les receptivitats" + e + _
             "} G[nGRAFCET];" + e + e

        r += "struct grafcetGENERAL G[nGRAFCET];" + e + e

        'declaració de variables globals
        r += "union BIT EntradaDig;" + e
        r += "union BIT EntradaDigAux;" + e
        r += "union BIT SortidaDig;" + e
        For Each aI As KeyValuePair(Of String, Variable) In m_gcmap.GCnameToVar
            Select Case aI.Value.dataType
                'as in IEC 61131-3 Table 10 - Elementary data types, column "No"
                Case "BOOL"
                    r += "int " + aI.Key + ";" + e
                Case "INT"
                    r += "int " + aI.Key + ";" + e
                Case "REAL"
                    r += "float " + aI.Key + ";" + e
            End Select
        Next
        r += e

        r += "union BIT FlancP;" + e
        r += "union BIT FlancB;" + e
        If m_gcmap.nTempP > 0 Then
            r += "union BIT TimerP [nTempP];" + e
            r += "union BIT TLimitP [nTempP];" + e
        End If
        If m_gcmap.nTempPB > 0 Then
            r += "union BIT TimerPB [nTempPB];" + e
            r += "union BIT TLimitPB [nTempPB];" + e
        End If
        r += "union BIT BitTimerP;" + e
        r += "union BIT BitTimerPB;" + e + e

        'declaració de funcions del programa principal
        r += "void IniciMicro();        // inicialitzacions pròpies del microcontrolador" + e + _
             "void IniciGrafcet();      // Inicialització del Grafcet" + e + _
             "void LecturaEntrades();   // Lectura d'entrades" + e + _
             "void GestioTempo();       // Gestió de temporitzadors" + e + _
             "void AssignaAux();        // Assignació Auxiliar del Grafcet" + e + _
             "void EvolucioGrafcet();   // Evolució del Grafcet" + e + _
             "void AccionsPuntuals();   // Execució d'Accions Puntuals" + e + _
             "unsigned char GrafcetEstable(); // Grafcet Estable? Recerca d'estabilitat" + e + _
             "void AccionsContinues();  // Forçament de grafcets i Execució d'Accions Contínues" + e + _
             "void SortidesFisiques();  // Activació de les sortides Físiques" + e + e

        'i la resta de subrutines
        r += "void LecturaEntradesFisiques();   // registra el valor de les entrades" + e + _
             "// del sistema en les variables corresponents." + e + _
             "void AvaluaReceptivitats(unsigned char nG); // per al grafcet nG" + e + _
             "// determina l'estat de les seves receptivitats." + e

        Return r
    End Function

    '
    '@section functions
    '
    Public Function GetEvolucioGrafcet()
        Dim eg As New GCEvolucioGrafcet(Me)
        Return eg.GetEvolucioGrafcet
    End Function

    Public Function GetAvaluaReceptivitats() As String
        Dim ar As New GCAvaluaReceptivitats(Me)
        Return ar.GetAvaluaReceptivitats
    End Function

    Public Function GetAccionsContinues() As String
        Dim ac As New GCAccionsContinues(Me)
        Return ac.GetAccionsContinues
    End Function

    Public Function GetAccionsPuntuals() As String
        Dim ap As New GCAccionsPuntuals(Me)
        Return ap.GetAccionsPuntuals
    End Function

    Public Function GetES() As String
        Dim es As New GCES(Me)
        Return es.GetES
    End Function

    Public Function GetAltresRutines()
        Dim ar As New GCAltresRutines(Me)
        Return ar.GetAltresRutines
    End Function

    'Private Function GetStepTimerNode(ByVal nG As Integer, ByRef s As BaseGraphicalStep) As String
    '    Return "(G[" + m_gcmap.numToGrafcet(nG).Name + "].EtapaActivaAux.bits.bit" + m_gcmap.G(nG).stepToBit(s).ToString + ") && " + _
    '            "!(BitTimerP.bits.bit" + m_gcmap.stepsTimerBit(s).ToString + ")"
    'End Function

End Class
