Imports System.Net.Sockets
Imports System.IO
Imports System.Threading

Public Class RunControlIO
    'this class could be generically named "NetworkIO" if 
    'm_Subscriber were an Interface instead of RunControl

    Private Const DEBUG As Boolean = True

    Private stream As NetworkStream
    Private streamw As StreamWriter
    Private streamr As StreamReader
    Private client As TcpClient
    Private listener As Threading.Thread

    Private m_Subscriber As RunControl
    'actually, m_Subscriber can be whatever implementing Public Sub Receive(ByVal msg As String)

    Private m_IP As String = "127.0.0.1"
    Private m_Port As Integer = 8000

    Private Delegate Sub ReceiveDelegate(ByVal msg As String)

    Public Sub New(ByRef refSubscriber As RunControl)
        m_Subscriber = refSubscriber
    End Sub

    Public Function Conectar() As Boolean
        Try
            client = New TcpClient 'APT (c) 2011
            client.Connect(m_IP, m_Port)
            If client.Connected Then
                stream = client.GetStream
                streamw = New StreamWriter(stream)
                streamr = New StreamReader(stream)
                streamw.Flush()
                listener = New Threading.Thread(AddressOf Listen)
                listener.Name = "RunControlIO listener"
                listener.Start()
            Else
                MessageBox.Show("Client not connected")
                Return False
            End If
        Catch ex As Exception
            Warn("Client connect exception: " + vbCrLf + _
                 ex.StackTrace + vbCrLf + _
                 vbCrLf + _
                 "" + ex.Message)
            Return False
        End Try

        Return True
    End Function

    Public Function Desconectar() As Boolean
        If listener Is Nothing Then Return False

        If listener.IsAlive Then listener.Abort()
        client.Close()
        Return True
    End Function

    Public Function Send(ByVal msg As String)
        'This function can be called several times during the same interaction turn.
        '
        'Unfortunately, the server (the C executable) seems not to receive successfully 
        'those messages that are sent with a very small delay in between.
        '
        'Several solutions can be used to address this problem:
        '1. To impose a higher delay between messages.
        '2. To send just one message per turn, encoding several commands per message in 
        'the client (this VB program), and decoding them in the server
        '3. If the problem is in the server, to allow the server receive more 
        'close messages. Eg: using several reception buffers.
        '
        'Currently, the first solution has been adopted.
        'An arbitrarily high delay is provoked after each sending.
        'Other, better informed ways to choose this delay could be explored.
        Dim delay As Integer = 0
        Try
            streamw.WriteLine(msg)
            streamw.Flush()
            Thread.Sleep(50)
            Return True
        Catch
            MsgBox("Failed to send this message: " + msg)
            Return False
        End Try
    End Function

    Private Sub ReceiveDelegateMethod(ByVal msg As String)
        m_Subscriber.Receive(msg)
    End Sub

    Private Sub Listen()
        While client.Connected
            Try
                Dim params() As Object = {streamr.ReadLine()}
                m_Subscriber.m_Form.Invoke(New ReceiveDelegate(AddressOf Me.ReceiveDelegateMethod), params)
                'provisional: an inter-thread communication technique decoupled from forms should be used instead
            Catch ex As KeyNotFoundException
                MsgBox(ex.Message + vbCrLf + vbCrLf + ex.StackTrace)
                If Not ex.InnerException Is Nothing Then MsgBox(ex.InnerException.ToString)
            Catch ex As IOException
                MessageBox.Show("Verbindung zum Server nicht möglich!" + vbCrLf + vbCrLf + ex.Message)
            End Try
        End While
    End Sub

    Private Sub Warn(ByVal msg As String)
        If DEBUG Then MsgBox(msg)
    End Sub

End Class
