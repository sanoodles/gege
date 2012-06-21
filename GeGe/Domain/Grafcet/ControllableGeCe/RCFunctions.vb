'remote control additions for gece
Public Class RCFunctions

    Private Const t As String = "    " 'tab = 4 spaces
    Private Const e As String = vbCrLf 'enter

    Private m_rcmap As RCMapping

    Public Sub New(ByRef m As RCMapping)
        m_rcmap = m
    End Sub

    'sections
    ' includes
    ' constants
    ' variables
    ' functions

    '
    '@section includes
    '
    Public Function GetIncludes() As String
        Return "#include <windows.h>" + e + _
                "#include <winsock2.h>" + e + _
                "#include <ws2tcpip.h>" + e + _
                "#include <stdlib.h>" + e + _
                "#include <stdio.h>" + e + _
                e + _
                "// Need to link with Ws2_32.lib" + e + _
                "#pragma comment (lib, ""Ws2_32.lib"")" + e + _
                "// #pragma comment (lib, ""Mswsock.lib"")" + e
    End Function

    '
    '@section constants
    '
    Public Function GetConstants() As String
        Dim r As String

        r = "#define DEFAULT_BUFLEN 512" + e + _
               "#define DEFAULT_PORT ""8000""" + e

        r += "#define N_GRAFCET " + m_rcmap.nGrafcet.ToString + e

        Return r
    End Function

    '
    '@section variables
    '
    Public Function GetVariables() As String
        Dim r As String

        r = "WSADATA wsaData;" + e + _
                "SOCKET ListenSocket = INVALID_SOCKET, " + e + _
                "	   ClientSocket = INVALID_SOCKET;" + e + _
                "struct addrinfo *result = NULL, " + e + _
                "				hints;" + e + _
                "char recvbuf[DEFAULT_BUFLEN];" + e + _
                "int iResult, iSendResult;" + e + _
                "int recvbuflen = DEFAULT_BUFLEN;" + e + _
                "int end = 0;" + e + _
                "int step = 0;" + e + _
                "int csteps = 0;" + e + _
                "int cvars = 0;" + e

        r += "#define TYPE_INT 3" + e 'as in IEC 61131-3 Table 10 - Elementary data types Column "No."
        r += "#define TYPE_REAL 10" + e

        Return r
    End Function

    '
    '@section functions
    '
    Public Function GetFunctions() As String
        Return GetWaitForconnection() + e + _
                GetResponse() + e + _
                GetAttend() + e + _
                GetDisconnect() + e + _
                GetIniciRC() + e + _
                GetTimerCallBack()
    End Function

    Private Function GetWaitForconnection() As String
        Return "int wait_for_connection(void)" + e + _
                "{" + e + _
                t + "iResult = WSAStartup(MAKEWORD(2,2), &wsaData);" + e + _
                e + _
                t + "ZeroMemory(&hints, sizeof(hints));" + e + _
                t + "hints.ai_family = AF_INET;" + e + _
                t + "hints.ai_socktype = SOCK_STREAM;" + e + _
                t + "hints.ai_protocol = IPPROTO_TCP;" + e + _
                t + "hints.ai_flags = AI_PASSIVE;" + e + _
                e + _
                t + "iResult = getaddrinfo(NULL, DEFAULT_PORT, &hints, &result);" + e + _
                t + "ListenSocket = socket(result->ai_family, result->ai_socktype, result->ai_protocol);" + e + _
                t + "iResult = bind( ListenSocket, result->ai_addr, (int)result->ai_addrlen);" + e + _
                t + "freeaddrinfo(result);" + e + _
                t + "iResult = listen(ListenSocket, SOMAXCONN);" + e + _
                t + "ClientSocket = accept(ListenSocket, NULL, NULL);" + e + _
                t + "closesocket(ListenSocket);" + e + _
                e + _
                t + "return 0;" + e + _
                "}" + e
    End Function

    Private Function GetResponse() As String
        Dim r As String
        Dim isFirst As Boolean

        r = "int Response()" + e + _
            "{" + e + _
            t + "if(strcmp(recvbuf, ""end\r\n"") == 0) {" + e + _
            "	    end = 1;" + e

        'RequestActiveSteps
        r += t + "} else if (strcmp(recvbuf, ""RequestActiveSteps\r\n"") == 0) {" + e + _
            t + t + "char buffer[2];" + e + _
            t + t + "int i;" + e + _
            t + t + "int isFirst = 1;" + e + _
            t + t + "printf(""RequestActiveSteps %d\n"", csteps);" + e + _
            t + t + "csteps = csteps + 1;" + e + _
            t + t + "iSendResult = send(ClientSocket, ""ResponseActiveSteps "", strlen(""ResponseActiveSteps ""), 0);" + e + _
            t + t + "for (i = 0; i < N_GRAFCET; i++) {" + e + _
            t + t + t + "if (!isFirst) {" + e + _
            t + t + t + t + "iSendResult = send(ClientSocket, "","", 1, 0);" + e + _
            t + t + t + "} else {" + e + _
            t + t + t + t + "isFirst = 0;" + e + _
            t + t + t + "}" + e + _
            t + t + t + "itoa(G[i].EtapaActiva.all, buffer, 16);" + e + _
            t + t + t + "iSendResult = send(ClientSocket, buffer, strlen(buffer), 0);" + e + _
            t + t + "}" + e + _
            t + t + "iSendResult = send(ClientSocket, ""\r\n"", 2, 0);" + e

        'RequestVariables
        r += t + "} else if (strcmp(recvbuf, ""RequestVariables\r\n"") == 0) {" + e + _
            t + t + "char buffer[20];" + e + _
            t + t + "printf(""RequestVariables %d\n"", cvars);" + e + _
            t + t + "cvars = cvars + 1;" + e + _
            t + t + "iSendResult = send(ClientSocket, ""ResponseVariables "", strlen(""ResponseVariables ""), 0);" + e

        isFirst = True
        For Each Va As Variable In m_rcmap.PosToVar.Values

            'send comma or not
            If isFirst Then
                isFirst = False
            Else
                r += t + t + "send(ClientSocket, "","", 1, 0);" + e
            End If

            'send variable value
            Select Case Va.dataType
                Case "BOOL"
                    r += t + t + "itoa(" + Va.Name + ", buffer, 10);" + e
                Case "INT"
                    r += t + t + "itoa(" + Va.Name + ", buffer, 10);" + e
                Case "REAL"
                    r += t + t + "sprintf(buffer, ""%f"", " + Va.Name + ");" + e
            End Select
            r += t + t + "iSendResult = send(ClientSocket, buffer, strlen(buffer), 0);" + e

        Next
        r += t + t + "iSendResult = send(ClientSocket, ""\r\n"", 2, 0);" + e

        'SetVariable
        r += t + "} else if (strstr(recvbuf, ""SetVariable "") != 0) {" + e + _
            t + t + "char buffer[20];" + e + _
            t + t + "char * pch;" + e + _
            t + t + "char * var; // variable name" + e + _
            t + t + "char * val; // variable value as string" + e
        r += t + t + "pch = strtok(recvbuf, "" ""); // skip command" + e + _
            t + t + "var = strtok(NULL, "" ""); // variable name is first parameter" + e + _
            t + t + "val = strtok(NULL, "" ""); // variable value is second parameter" + e

        'r += "printf(""%s\n"", var);" + e
        'r += "printf(""%s\n"", val);" + e

        'confirmation
        r += t + t + "iSendResult = send(ClientSocket, ""SetVariable "", strlen(""SetVariable ""), 0);" + e + _
            t + t + "iSendResult = send(ClientSocket, var, strlen(var), 0);" + e + _
            t + t + "iSendResult = send(ClientSocket, "" "", 1, 0);" + e

        isFirst = True

        'tasks of 
        '- conversion of the value to proper type
        '- write variable
        '- de-conversion of the value to char for confirmation
        'depend on the type of the variable the value of which is being set
        For Each Va As Variable In m_rcmap.PosToVar.Values

            r += t + t

            'print else or not
            If isFirst Then
                isFirst = False
            Else
                r += "else "
            End If

            'if
            r += "if (strcmp(var, """ + Va.Name + """) == 0) {" + e

            Select Case Va.dataType
                Case "BOOL"
                    'convert + write variable
                    r += t + t + t + Va.Name + " = atoi(val);" + e
                    'de-convert for confirmation
                    r += t + t + t + "itoa(" + Va.Name + ", buffer, 10);" + e
                Case "INT"
                    'convert + write variable
                    r += t + t + t + Va.Name + " = atoi(val);" + e
                    'de-convert for confirmation
                    r += t + t + t + "itoa(" + Va.Name + ", buffer, 10);" + e
                Case "REAL"
                    'convert + write variable
                    r += t + t + t + Va.Name + " = atof(val);" + e
                    'de-convert for confirmation
                    r += t + t + t + "sprintf(buffer, ""%f"", " + Va.Name + ");" + e
            End Select

            r += t + t + "}" + e
        Next

        'confirmation
        r += t + t + "iSendResult = send(ClientSocket, buffer, strlen(buffer), 0);" + e
        r += t + t + "iSendResult = send(ClientSocket, ""\r\n"", 2, 0);" + e

        'step
        r += t + "} else if (strcmp(recvbuf, ""step\r\n"") == 0) {" + e + _
            t + t + "printf(""step\n"");" + e + _
            t + t + "step = 1;" + e + _
            t + "}" + e + _
            e + _
            t + "return 0;" + e + _
            "}" + e

            Return r
    End Function

    Private Function GetAttend() As String
        Return "int Attend()" + e + _
                "{" + e + _
                t + "iResult = recv(ClientSocket, recvbuf, recvbuflen, 0);" + e + _
                e + _
                t + "if (iResult > 0) {" + e + _
                t + "    recvbuf[iResult] = 0; // mark end of string" + e + _
                t + "    Response();" + e + _
                t + "}" + e + _
                e + _
                t + "return 0;" + e + _
                "}" + e
    End Function

    Private Function GetDisconnect() As String
        Return "int disconnect()" + e + _
                "{" + e + _
                t + "shutdown(ClientSocket, SD_SEND);" + e + _
                t + "closesocket(ClientSocket);" + e + _
                t + "WSACleanup();" + e + _
                t + "return 0;" + e + _
                "}" + e
    End Function

    Private Function GetIniciRC() As String
        Dim r As String 'result

        r = "int IniciRC()" + e + _
                "{" + e
        r += t + "return 0;" + e
        r += "}" + e

        Return r
    End Function

    '@see http://msdn.microsoft.com/en-us/library/ms644901%28v=vs.85%29.aspx
    Private Function GetTimerCallBack() As String
        Dim r As String

        r = "LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)" + e
        r += "{" + e
        r += t + "switch(message)" + e
        r += t + "{" + e
        r += t + t + "case WM_TIMER:" + e
        r += t + t + t + "printf(""t"");" + e
        r += t + t + t + "temps_adeq();" + e
        r += t + t + t + "break;" + e
        r += t + "}" + e
        r += t + e + e
        r += t + "return 0;" + e
        r += "}" + e

        Return r
    End Function

End Class
