
Imports System.Text.RegularExpressions
''' <summary>
''' PLC Database
''' An instance of this class stores all data of a RSS file. One database is binded to one file. To load a new file,
''' a new instance needs to be created.
''' </summary>
Public Class DataEntry
    Private addr As String
    Private name As String
    Private desp As String
    Private des As List(Of String)
    Private src As List(Of String)
    Private ext As String
    Private logic As Node
    Dim addr_format As New Regex("^(?:([A-Z]{1,3})(\d{1,3}):(\d{1,3})|(?:(I|O|S|U):(\d{1,3}(?:\.\d{1,3})*)))(?:\/(\d{1,2}))*(.*)$")
    Public Sub New(address As String, name As String, description As String)
        Dim match As Match = addr_format.Match(address)
        If Not match.Success Then
            Throw New ArgumentException("Invalid address format: " & address)
        End If
        addr = address
        Me.name = name
        desp = description
        des = New List(Of String)
        src = New List(Of String)
        ext = ""
    End Sub

    Public Sub New(address As String)
        Dim match As Match = addr_format.Match(address)
        If Not match.Success Then
            Throw New ArgumentException("Invalid address format: " & address)
        End If
        addr = address
        desp = ""
        name = ""
        des = New List(Of String)
        src = New List(Of String)
        ext = ""
    End Sub

    Public Sub CopyNameAndDesp(other As DataEntry)
        name = other.TagName
        desp = other.Description
    End Sub
    Public Property Address As String
        Get
            Return addr
        End Get
        Set(value As String)
            addr = value
        End Set
    End Property

    Public Property TagName As String
        Get
            Return name
        End Get
        Set(value As String)
            If name <> value Then
            End If
            name = value
        End Set
    End Property

    Public Property Description As String
        Get
            Return desp
        End Get
        Set(value As String)
            If desp <> value Then
            End If
            desp = value
        End Set
    End Property

    Public Property MappingTo As List(Of String)
        Get
            Return des
        End Get
        Set(value As List(Of String))
            des = value
        End Set
    End Property

    Public Property MappingLogic As Node
        Get
            Return logic
        End Get
        Set(value As Node)
            logic = value
        End Set
    End Property
    Public Sub AddMappingTo(addr As String)
        If Not MappingTo.Contains(addr) Then
            des.Add(addr)
        End If
    End Sub
    Public Sub AddMappedTo(addr As String)
        If Not MappedTo.Contains(addr) Then
            src.Add(addr)
        End If
    End Sub
    Public Property MappedTo As List(Of String)
        Get
            Return src
        End Get
        Set(value As List(Of String))
            src = value
        End Set
    End Property
    Public Property Extension As String
        Get
            Return ext
        End Get
        Set(value As String)
            ext = value
        End Set
    End Property
End Class
