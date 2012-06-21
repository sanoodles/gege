Public Enum enumRunState
    Started
    Stopped
End Enum

'This class coordinates the classes involved in grafcet verification.
'
'UniSim and GeCé domains differ on two aspects
' 1. Whether they store different type (bool, int, real) variables mixed or apart
' 2. Whether they store different scope (project, grafcet 1, .., grafcet N) variables mixed or apart
'
'Different type: UniSim: Mixed, GeCé: Apart 
'       (only EntradaDig and SortidaDig, which are arrays for boolean 
'       input and output variables respectively)
'Different scope: UniSim: Apart, GeCé: Mixed
Public Class RunControl

    Public m_Form As Form 'provisional for RunControlIO delegate invocation

    Private m_Project As Project
    Private m_RunPath As String
    Private m_gcmap As GCMapping
    Private m_rcmap As RCMapping
    Private m_IO As RunControlIO
    Private m_RunProcess As Process
    Private m_RunState As enumRunState

    Private Const t As String = "    " 'tab = 4 spaces
    Private Const e As String = vbCrLf 'enter
    Private Const RUN_DIRECTORY As String = "Run"

    Public ReadOnly Property RunState() As enumRunState
        Get
            Return m_RunState
        End Get
    End Property

    Private Function MakeDir() As Boolean
        My.Computer.FileSystem.CreateDirectory(RUN_DIRECTORY)
        Return True
    End Function

    Public Sub New(ByRef refProject As Project)
        m_Project = refProject

        MakeDir()
        m_RunPath = Application.StartupPath + "\" + RUN_DIRECTORY

        m_gcmap = New GCMapping(refProject)
        m_rcmap = New RCMapping(refProject)
        m_IO = New RunControlIO(Me)
        m_RunState = enumRunState.Stopped
    End Sub

    Public Function GetVarsNames() As List(Of String)
        Dim r As New List(Of String)

        For Each v As Variable In m_Project.AllVariables
            r.Add(v.Name)
        Next

        Return r
    End Function

    'Creates Grafcet's source code in m_RunPath folder
    Public Function Generate() As Boolean
        Dim gc As New GCwithRC(m_Project, m_gcmap, m_rcmap, True, m_RunPath)
        Return gc.Go()
    End Function

    'Compiles source code from m_SimPath folder into an executable file
    Public Function Compile() As Boolean
        Dim Output As String
        Using P As New Process()
            P.StartInfo.FileName = "cmd.exe"
            P.StartInfo.UseShellExecute = False
            P.StartInfo.CreateNoWindow = True
            P.StartInfo.RedirectStandardOutput = True
            P.StartInfo.RedirectStandardInput = True
            P.Start()

            'clean previous compilation
            P.StandardInput.WriteLine("del """ + m_RunPath + "\main.exe""")

            'environment initialization
            P.StandardInput.WriteLine("""C:\Archivos de programa\Microsoft Visual Studio 9.0\VC\vcvarsall.bat""")
            'TODO: make this a user-level parameter

            'compilation
            Dim cmd As String
            cmd = "cl """ + m_RunPath + "\*.c"" /Fe""" + m_RunPath + "\main.exe"" /link User32.lib"
            P.StandardInput.WriteLine(cmd)
            '   /link User32.lib is to have the Windows' SetTimer function available
            '   @see http://www.daniweb.com/software-development/cpp/code/217306

            'clean source and objects
            P.StandardInput.WriteLine("del """ + m_RunPath + "\*.c""")
            P.StandardInput.WriteLine("del """ + m_RunPath + "\*.h""")
            P.StandardInput.WriteLine("del """ + m_RunPath + "\*.obj")

            'exit
            P.StandardInput.WriteLine("exit")

            Output = P.StandardOutput.ReadToEnd()
            P.WaitForExit()
        End Using
        Trace.WriteLine(Output)

        Return True
    End Function

    Public Function Ignition() As Boolean
        m_RunProcess = New Process

        With m_RunProcess
            .StartInfo.FileName = "cmd.exe"
            .StartInfo.UseShellExecute = False
            .StartInfo.CreateNoWindow = True
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.RedirectStandardInput = True
            .Start()
            .StandardInput.WriteLine("""" + m_RunPath + "\main.exe""")
        End With

        m_RunState = enumRunState.Started

        Return True
    End Function

    Public Function Conectar() As Boolean
        Return m_IO.Conectar()
    End Function

    Public Sub RequestActiveSteps()
        m_IO.Send("RequestActiveSteps")
    End Sub

    Public Sub RequestVariables()
        m_IO.Send("RequestVariables")
    End Sub

    Public Sub Steep()
        m_IO.Send("step")
    End Sub

    Public Sub SetVarValue(ByRef refVar As Variable, ByVal newVal As String)
        Select Case refVar.dataType
            Case "BOOL"
                newVal = m_gcmap.BoolStrToBoolInt(newVal).ToString
        End Select
        m_IO.Send("SetVariable " + refVar.Name + " " + newVal)
    End Sub

    Public Sub Eend()
        m_IO.Send("end")
        m_RunState = enumRunState.Stopped
    End Sub

    Public Function Desconectar() As Boolean
        Return m_IO.Desconectar
    End Function

    Public Sub Receive(ByVal msg As String)
        Dim msg_split As String()
        Dim msg_command As String
        Dim msg_param1 As String
        Dim msg_param2 As String

        If msg Is Nothing Then Return

        msg_split = msg.Split(" ")
        msg_command = msg_split(0)
        If msg_split.Length > 1 Then
            msg_param1 = msg_split(1)
        Else
            msg_param1 = ""
        End If

        If msg_split.Length > 2 Then
            msg_param2 = msg_split(2)
        Else
            msg_param2 = ""
        End If

        Select Case msg_command
            Case "ResponseActiveSteps"
                m_Project.SetActiveSteps(GetActiveSteps(msg_param1))
            Case "ResponseVariables"
                m_Project.SetVarValues(GetVariablesValues(msg_param1))
            Case "SetVariable"
                m_Project.SetVarValues(GetVariableValue(msg_param1, msg_param2))
        End Select

    End Sub

    '@pre string msg is a comma-separated list of the union BIT EtapaActiva of each grafcet 
    '   codified in hexadecimal, not including leading zeroes. The list is ordered by number of grafcet.
    '@return Grafcet, GraphicalStepsList pairs with the grafcet and the list of its active steps
    Private Function GetActiveSteps(ByVal msg As String) As SerializableDictionary(Of Grafcet, GraphicalStepsList)

        Dim r As New SerializableDictionary(Of Grafcet, GraphicalStepsList)
        Dim step_list As New GraphicalStepsList
        Dim i As Integer
        Dim i_grafcet As Integer
        Dim msg_bin As String
        Dim bitpos As Integer
        Dim bitval As Integer
        Dim n_leading_zeroes As Integer
        Dim msg_items() As String

        msg_items = msg.Split(",")
        i_grafcet = 0

        For Each msg_item As String In msg_items

            'convert to binary string
            msg_bin = Hexa.HexToBin(msg_item)
            'n_leading_zeroes = m_Project.numToGrafcet(i_grafcet).GraphicalStepsList.Count - msg_bin.Length
            n_leading_zeroes = m_gcmap.G(i_grafcet).stepToBit.Count - msg_bin.Length

            If n_leading_zeroes > 0 Then
                'add leading zeroes
                For i = 1 To n_leading_zeroes
                    msg_bin = "0" + msg_bin
                Next
            ElseIf n_leading_zeroes < 0 Then
                msg_bin = Mid(msg_bin, n_leading_zeroes * -1 + 1)
            End If

            'per a cada caracter binari
            For i = 1 To msg_bin.Length
                bitpos = msg_bin.Length - i
                bitval = Mid(msg_bin, i, 1)

                'si val "1", afegeix el step que representa al resultat
                If bitval = "1" Then
                    step_list.Add(m_gcmap.GetStepFromBit(i_grafcet, bitpos))
                End If
            Next

            r.Add(m_Project.numToGrafcet(i_grafcet), step_list)

            i_grafcet = i_grafcet + 1
        Next

        Return r
    End Function

    '@pre string msg is the value of variables codified in decimal.
    '@return SerializableDictionary Of Variable, String with the value for each variable represented as String
    Private Function GetVariablesValues(ByVal msg As String) As SerializableDictionary(Of Variable, String)
        Dim r As New SerializableDictionary(Of Variable, String)
        Dim pos As Integer
        Dim val As String
        Dim var As Variable
        Dim msg_arr() As String
        Dim varval As SerializableDictionary(Of Variable, String)

        msg_arr = msg.Split(",")

        For i = 1 To msg_arr.Length
            pos = i - 1
            var = m_rcmap.PosToVar(pos)
            val = msg_arr(i - 1)
            varval = GetVariableValue(var.Name, val)

            'there is only one. but how to access the first keyvaluepair if not with a foreach?
            For Each v As KeyValuePair(Of Variable, String) In varval
                r.Add(v.Key, v.Value)
            Next
        Next

        Return r
    End Function

    '@pre the value "value" of the variable named "strvar".
    '@return SerializableDictionary Of Variable, the value of the variable represented as String
    Private Function GetVariableValue(ByVal strvar As String, ByVal value As String) As SerializableDictionary(Of Variable, String)
        Dim r As New SerializableDictionary(Of Variable, String)

        Dim var As Variable = m_gcmap.GCnameToVar(strvar)
        Select Case var.dataType
            Case "BOOL"
                Dim boolval As String = StringToBoolean(value)
                r.Add(var, boolval.ToString)
            Case "INT"
                Dim intval As Integer = CInt(value)
                r.Add(var, intval.ToString)
            Case "REAL"
                Dim realval As Single = StringToSingle(value)
                r.Add(var, realval.ToString)
        End Select

        Return r
    End Function

    'translate en-US decimal separators (used by the executable) to .NET local decimal separator, whichever it is
    Private Function StringToSingle(ByVal str As String) As Single
        Dim nfi As System.Globalization.NumberFormatInfo = New System.Globalization.CultureInfo("en-US", False).NumberFormat
        Return Convert.ToSingle(str, nfi)
    End Function

    '@return "False" if str == 0. "True" if str == 1.
    Private Function StringToBoolean(ByVal str As String) As Boolean
        Return str = "1"
    End Function

End Class
