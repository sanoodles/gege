Imports System.Collections.Generic
Imports System
Imports System.Xml
Imports System.Xml.Serialization

<Serializable()> _
Public Class VariablesList
    Inherits List(Of Variable)

    Protected m_Name As String
    Protected m_Constant As Boolean
    Protected m_Retain As Boolean
    Protected m_NonRetain As Boolean
    Protected m_Persistent As Boolean
    Protected m_NonPersistent As Boolean

    Public Event NameChanged(ByVal NewName As String)     'Notifica il cambiamento del nome
    Public Event Disposing()    'Notifica la distruzione della lista
    Public Event NewVariable(ByRef Var As Variable)
    Public Event VariableModified(ByRef Var As Variable)
    Public Event VariableDropped(ByRef Var As Variable)
    Public Event ValueChanged()

    Property Name() As String
        Get
            Name = m_Name
        End Get
        Set(ByVal Value As String)
            m_Name = Value

            RaiseEvent NameChanged(Value)
        End Set
    End Property

    Sub New(ByVal Name As String)
        MyBase.New()
        m_Name = Name
    End Sub

    Sub New()
        MyBase.New()
    End Sub



    Public Sub AddVariable(ByRef Var As Variable)
        MyBase.Add(Var)

        AddHandler Var.ValueChanged, AddressOf ValueChangedHandler

        RaiseEvent NewVariable(Var)
    End Sub

    Public Function CreateAndAddVariable(ByVal Scope As String, ByVal Name As String, _
        ByVal Documentation As String, ByVal Address As String, _
        ByVal InitialValue As String, ByVal Type As String, _
        ByVal Application As enumVariableApplication, _
        ByVal ViewAsHex As Boolean) As Variable
        'Crea e aggiunge la variabile
        'Restituisce un riferimento nullo se c'è un errore
        CreateAndAddVariable = Nothing
        Dim NewVariable As Variable = VariablesManager.CreateVariable(Scope, Name, _
                Documentation, Address, InitialValue, Type, Application, ViewAsHex)
        If Not IsNothing(NewVariable) Then
            AddVariable(NewVariable)
            Return NewVariable
        End If
    End Function

    Public Sub ModifyVariable(ByRef Var As Variable, _
            ByVal Scope As String, ByVal Name As String, _
            ByVal Documentation As String, _
            ByVal varType As String, ByVal initialValue As String, _
            ByVal Application As String, ByVal ViewAsHex As Boolean)

        Var.Scope = Scope
        Var.Name = Name
        Var.Documentation = Documentation
        Var.SetInitialValue(initialValue)
        Var.Application = Application
        Var.ViewAsHex = ViewAsHex

        RaiseEvent VariableModified(Var)
    End Sub

    Public Sub RemoveVariable(ByRef Var As Variable)
        MyBase.Remove(Var)

        RaiseEvent VariableDropped(Var)
    End Sub

    Public Function FindVariableByName(ByVal VarName As String) As Variable
        For Each V As Variable In Me
            If V.Name = VarName Then
                Return V
            End If
        Next V
        Return Nothing
    End Function

    Public Sub Reset()
        'Resetta al valore iniziale tutte le variabili della lista
        For Each V As Variable In Me
            ' In questo modo le variabili intere si resettano correttamente
            ' (in realtà il problema è a monte in come è stata pensata l'implementazione
            ' di IntegerVariable, ma è tardi per cambiarla)
            V.ResetValue()
        Next V
    End Sub

    Public Overloads Sub RemoveAll()
        For Each V As Variable In Me
            V.DisposeMe()
            Remove(V)
        Next V
    End Sub

    Public Sub DisposeMe()
        RaiseEvent Disposing()
        Dim i As Integer
        'Cancella tutte le variabili
        For i = 0 To Count - 1
            Me(0).DisposeMe()
            RemoveAt(0)
        Next i
        Me.Finalize()
    End Sub

    ' Crea un nome del tipo nameRoot<nn> dove nn è un numero e il nome è univoco
    ' all'interno di questa lista di variabili
    Public Function MakeUniqueName(ByVal nameRoot As String) As String
        Dim fmtString As String = nameRoot & "{0}"
        Dim index As Integer = 0
        Dim outString As String = ""
        While True
            outString = String.Format(fmtString, index)
            If Me.FindVariableByName(outString) Is Nothing Then Return outString
            index += 1
        End While
        ' Dovremmo trovare un nome prima o poi, ma per far contento VB fingiamo che questo
        ' percorso vada coperto
        Return Nothing
    End Function

    ' Crea un duplicato di questo elenco variabili. Se si mette useActualValues a True vengono
    ' usati i valori correnti di ogni variabile nel duplicato, se si mette a False vengono usati
    ' i valori iniziali
    Public Function CreateInstance(Optional ByVal useActualValues As Boolean = False) As VariablesList
        Dim vList As New VariablesList()

        For Each V As Variable In Me
            Dim vDup As Variable = vList.CreateAndAddVariable(V.Scope.Clone, V.Name.Clone().ToString, _
                V.Documentation.Clone().ToString, V.Address.Clone().ToString, _
                    V.InitialValueToUniversalString().Clone.ToString(), _
                    V.dataType.Clone().ToString, V.Application.ToString, _
                    V.ViewAsHex)
            If useActualValues Then vDup.SetActValue(V.ValueToUniversalString().Clone())
        Next

        Return vList
    End Function

    Private Sub ValueChangedHandler()
        RaiseEvent ValueChanged()
    End Sub

End Class
