'Creates source code files specified by GeCé
'@param Control To make the code remotely controllable
'@see: http://upcommons.upc.edu/pfc/bitstream/2099.1/2731/1/36670-1.pdf
Public Class GCwithRC

    Private Const t As String = "    " 'tab = 4 spaces
    Private Const e As String = vbCrLf 'enter

    Private m_Control As Boolean
    Private m_TimerPeriod As Integer 'in milliseconds
    Private m_Path As String
    Private m_rc As RCFunctions
    Private m_gc As GCFunctionsController

    Public Sub New(ByRef p As Project, ByRef gcmap As GCMapping, _
            ByRef rcmap As RCMapping, ByVal Control As Boolean, _
            ByVal Path As String)

        m_Control = Control
        m_TimerPeriod = 200
        m_Path = Path
        If Control Then m_rc = New RCFunctions(rcmap)
        m_gc = New GCFunctionsController(gcmap, m_TimerPeriod)
    End Sub

    Public Function Go() As Boolean

        With My.Computer.FileSystem
            .WriteAllText(m_Path + "\bit.h", _
                          m_gc.GetBitH(), False)

            .WriteAllText(m_Path + "\parametres.h", _
                          m_gc.GetParametresH(), False)

            .WriteAllText(m_Path + "\main.h", _
                          m_gc.GetMainH(), False)

            .WriteAllText(m_Path + "\main.c", _
                          GetMainC(), False)

            .WriteAllText(m_Path + "\EvolucioGrafcet.c", _
                          m_gc.GetEvolucioGrafcet(), False)

            .WriteAllText(m_Path + "\AvaluaReceptivitats.c", _
                          m_gc.GetAvaluaReceptivitats(), False)

            .WriteAllText(m_Path + "\AccionsContinues.c", _
                          m_gc.GetAccionsContinues(), False)

            .WriteAllText(m_Path + "\AccionsPuntuals.c", _
                          m_gc.GetAccionsPuntuals(), False)

            .WriteAllText(m_Path + "\ES.c", _
                          m_gc.GetES(), False)

            .WriteAllText(m_Path + "\AltresRutines.c", _
                          m_gc.GetAltresRutines(), False)
        End With

        Return True
    End Function

    '@see http://msdn.microsoft.com/en-us/library/ms644901%28v=vs.85%29.aspx
    'TODO: for running on windows, a technique for timers have to be studied
    'the technique used so far does not achieve the timer callback function to be
    'called
    Private Function GetMainC() As String
        Dim r As String = "" 'result

        '
        'sections:
        '* includes
        '* constants
        '* variables
        '* functions (main is last function)
        '

        'includes
        If m_Control Then
            r = _
            "#undef UNICODE" + e + _
            e + _
            "#define WIN32_LEAN_AND_MEAN" + e + _
            e + _
            m_rc.GetIncludes() + e
        End If
        r += m_gc.GetIncludes() + e

        'constants
        If m_Control Then
            r += m_rc.GetConstants() + e
        End If

        'variables
        If m_Control Then
            r += m_rc.GetVariables() + e
        End If

        'functions
        If m_Control Then
            r += m_rc.GetFunctions() + e
        End If

        'main
        r += "void main(void)" + e + _
            "{" + e + _
            t + "unsigned char i;" + e + e
        If m_Control Then
            r += t + "wait_for_connection();" + e + e
            r += t + "IniciRC();" + e + e
            r += t + "SetTimer(NULL, 1234, " + m_TimerPeriod.ToString + ", (TIMERPROC) NULL);" + e + e
        End If
        r += _
            t + "IniciMicro();" + e + _
            e + _
            t + "IniciGrafcet();" + e + _
            e
        r += t + "while (" + GetMainWhileCondition() + ")" + e + _
        t + "{" + e
        If m_Control Then
            r += t + t + "while (!end && !step) Attend();" + e
        End If
        r += _
            t + t + "LecturaEntrades();" + e + _
            t + t + "GestioTempo();" + e + _
            t + t + "for (i = 0; i < N; i++)" + e + _
            t + t + "{" + e + _
            t + t + t + "AssignaAux();" + e + _
            t + t + t + "EvolucioGrafcet();" + e + _
            t + t + t + "AccionsPuntuals();" + e + _
            t + t + t + "if (GrafcetEstable())" + e + _
            t + t + t + t + "i = N + 1;" + e + _
            t + t + "}" + e + _
            t + t + "AccionsContinues();" + e + _
            t + t + "SortidesFisiques();" + e
        If m_Control Then
            r += t + t + "step = 0;" + e
        End If
        r += _
            t + "} // fwhile" + e
        If m_Control Then
            r += t + "disconnect();" + e
            r += t + "KillTimer(NULL, 1234);" + e
        End If
        r += _
            "} // fmain" + e

        Return r
    End Function

    Private Function GetMainWhileCondition() As String
        If m_Control Then
            Return "!end"
        Else
            Return "1"
        End If
    End Function

End Class
