Imports System.Net
Imports System.Runtime.Remoting.Messaging
Imports System.Text.RegularExpressions

Public Class PLC_DB
    Private addrDic As Dictionary(Of String, DataEntry)
    Private tag_ref_list As Dictionary(Of String, String)
    Public Sub New(proj As Object)
        If proj Is Nothing Then
            Throw New ArgumentException("The project instance is NULL.")
        End If
        addrDic = New Dictionary(Of String, DataEntry)
        Dim data_collection As Object = proj.AddrSymRecords
        LoadDataEntry(data_collection)
    End Sub

    Public Function Empty() As Boolean
        Return addrDic.Count = 0
    End Function
    Public Function DBtoList() As List(Of String())
        Dim content As New List(Of String())
        For Each addr In addrDic.Keys
            content.Add({addr, addrDic(addr).TagName, addrDic(addr).Description})
        Next
        content.Sort(New DataEntryComparer())
        Return content
    End Function
    Private Sub LoadDataEntry(data_collection As Object)
        Dim numOfRec = data_collection.Count
        Dim record As Object
        For i As Integer = 0 To numOfRec - 1
            record = data_collection.GetRecordViaIndex(i)
            Dim desp = record.Description
            desp = desp.Replace(Environment.NewLine, " ")
            If record.Address IsNot Nothing Then
                Add(record.Address, record.Symbol, desp)
            End If
        Next
    End Sub

    Public Sub LoadMapping(programs As Object)
        Dim numOfProg = programs.Count()
        Dim modbus_file As Object
        For i As Integer = 0 To numOfProg - 1
            If programs.Item(i) IsNot Nothing AndAlso programs.Item(i).Name = "MODBUS" Then
                modbus_file = programs.Item(i)
                Dim numOfRung = modbus_file.NumberOfRungs
                For j As Integer = 0 To numOfRung - 1 'iterate through rungs
                    Dim mapping = ExtractMapping(modbus_file.GetRungAsAscii(j))
                    For Each pair In mapping
                        If ContainEntry(pair.Item1) Then
                            addrDic(pair.Item1).AddMappingTo(pair.Item2)
                            If Not ContainEntry(pair.Item2) Then
                                Add(pair.Item2)
                            End If
                            UpdateDescription(pair.Item2, addrDic(pair.Item1).Description)
                            UpdateTagName(pair.Item2, addrDic(pair.Item1).TagName)
                            addrDic(pair.Item2).AddMappedTo(pair.Item1)
                        End If
                    Next
                Next
                Exit For
            End If
        Next
    End Sub

    Public Function ContainEntry(addr As String) As Boolean
        Return addrDic.ContainsKey(addr)
    End Function

    Public Function GetTagName(addr As String) As String
        If Not addrDic.ContainsKey(addr) Then
            Throw New KeyNotFoundException("The data entry is not presented in the database: " & addr)
        End If
        Return addrDic(addr).TagName
    End Function
    Public Function GetDescription(addr As String) As String
        If Not addrDic.ContainsKey(addr) Then
            Throw New KeyNotFoundException("The data entry is not presented in the database: " & addr)
        End If
        Return addrDic(addr).Description
    End Function
    Public Function GetMappingTarget(addr As String) As List(Of String)
        If Not addrDic.ContainsKey(addr) Then
            Throw New KeyNotFoundException("The data entry is not presented in the database: " & addr)
        End If
        Return addrDic(addr).MappingTo
    End Function

    Public Function GetMappingSrc(addr As String) As List(Of String)
        If Not addrDic.ContainsKey(addr) Then
            Throw New KeyNotFoundException("The data entry is not presented in the database: " & addr)
        End If
        Return addrDic(addr).MappedTo
    End Function

    Public Sub Add(addr As String, name As String, desp As String)
        If addrDic.ContainsKey(addr) Then
            Throw New ArgumentException("This data entry has presented in the database: " & addr)
        End If
        addrDic.Add(addr, New DataEntry(addr, name, desp))
    End Sub

    Public Sub ChangeModifiedStatus(l As List(Of String))
        For Each addr In l
            addrDic(addr).isModified = False
        Next
    End Sub
    Public Sub Add(addr As String)
        If addrDic.ContainsKey(addr) Then
            Throw New ArgumentException("This data entry has presented in the database: " & addr)
        End If
        addrDic.Add(addr, New DataEntry(addr))
    End Sub

    Public Sub UpdateTagName(addr As String, name As String)
        If Not addrDic.ContainsKey(addr) Then
            Throw New KeyNotFoundException("The data entry is not presented in the database: " & addr)
        End If
        addrDic(addr).TagName = name
    End Sub

    Public Sub UpdateDescription(addr As String, desp As String)
        If Not addrDic.ContainsKey(addr) Then
            Throw New KeyNotFoundException("The data entry is not presented in the database: " & addr)
        End If
        addrDic(addr).Description = desp
    End Sub

    Public Sub SetMappingLogic(addr As String, logic As Node)
        If Not addrDic.ContainsKey(addr) Then
            Throw New KeyNotFoundException("The data entry is not presented in the database: " & addr)
        End If
        addrDic(addr).MappingLogic = logic
    End Sub

    Public Function GetMappingLogic(addr As String) As Node
        If Not addrDic.ContainsKey(addr) Then
            Throw New KeyNotFoundException("The data entry is not presented in the database: " & addr)
        End If
        Return addrDic(addr).MappingLogic
    End Function

    Public Function GetModifiedEntries() As List(Of String)
        Dim results As New List(Of String)
        For Each entry In addrDic
            If entry.Value.isModified Then
                results.Add(entry.Value.Address)
            End If
        Next
        Return results
    End Function

    Private coil_start = False
    Public Function ExtractMapping(str As String) As List(Of Tuple(Of String, String))
        Dim words As String() = str.Split(" "c)
        Dim results As New List(Of Tuple(Of String, String))
        For i As Integer = 0 To words.Length - 3
            If coil_start = False AndAlso words(i) = "MOV" Then
                If ContainEntry(words(i + 2)) AndAlso GetTagName(words(i + 2)) = "COIL_START" Then
                    coil_start = True
                    Return results
                End If
                Dim logic As Node = Parser.Parse(New LinkedList(Of String)(words))
                FindRegMapping(logic, results)
                Return results
            End If
            If coil_start AndAlso words(i) = "OR" Then
                ' MessageBox.Show(str)
                Dim ans As Node = Parser.Parse(New LinkedList(Of String)(words))
                'Dim out As String = ""
                Dim cur As Node = ans
                While cur IsNot Nothing
                    'out &= cur.ToString()
                    If cur.Ins = "BST" Then
                        FindCoilMapping(cur, results)
                    End If
                    cur = cur.NextIns
                End While
                Return results
                'MessageBox.Show("Parser result:" + Environment.NewLine + out)
                'MessageBox.Show(str)
            ElseIf words(i) = "SWP" Then
                Dim temp = ExtractAddresses(str)
                Dim src As String = ""
                Dim des As String = ""
                For Each pair As Tuple(Of String, String) In temp
                    'MessageBox.Show(pair.Item1 & pair.Item2)
                    If ContainEntry(pair.Item2) AndAlso GetTagName(pair.Item2) = "WORD_TO_REVERSE" Then
                        src = pair.Item1
                    ElseIf ContainEntry(pair.Item1) AndAlso GetTagName(pair.Item1) = "REVERSED_WORD" Then
                        des = pair.Item2
                    End If
                Next
                If src <> des Then
                    For count As Integer = 0 To 15
                        Dim full_addr1 = src & "/" & count.ToString
                        Dim full_addr2 = des & "/" & count.ToString
                        'MessageBox.Show(full_addr1 & " " & full_addr2)
                        If Not ContainEntry(full_addr1) Then
                            Continue For
                        End If
                        results.Add(New Tuple(Of String, String)(full_addr1, full_addr2))
                    Next
                End If
                Return results
            End If
        Next
        Return results
    End Function

    Private Sub FindRegMapping(logic As Node, results As List(Of Tuple(Of String, String)))
        Dim cur As Node = logic
        While cur IsNot Nothing AndAlso cur.Ins <> "BST"
            cur = cur.NextIns
        End While
        If cur Is Nothing Then
            Return
        End If
        For Each child In cur.Children
            ReadRegLogic(child, results)
        Next
    End Sub

    Private Sub ReadRegLogic(logic As Node, results As List(Of Tuple(Of String, String)))
        If logic.Ins = "MOV" Then
            results.Add(New Tuple(Of String, String)(logic.Args(0).Replace(".ACC", ""), logic.Args(1).Replace(".ACC", "")))
        ElseIf logic.Ins = "EQU" Then
            Dim cur As Node = logic
            cur = cur.NextIns
            If cur.Ins <> "BST" Then
                Return
            End If
            Dim child1 As Node = cur.Children(0)
            Dim child2 As Node = cur.Children(1)
            If child1.Ins = "MOV" AndAlso child2.Ins = "MOV" Then
                    results.Add(New Tuple(Of String, String)(child1.Args(0).Replace(".ACC", ""), child1.Args(1).Replace(".ACC", "")))
                    results.Add(New Tuple(Of String, String)(child2.Args(0).Replace(".ACC", ""), child2.Args(1).Replace(".ACC", "")))
                    If Not ContainEntry(child1.Args(1)) Then
                        Add(child1.Args(1))
                    End If
                    If Not ContainEntry(child2.Args(1)) Then
                        Add(child2.Args(1))
                    End If
                    SetMappingLogic(child1.Args(1), logic)
                    SetMappingLogic(child2.Args(1), logic)
                End If
            ElseIf logic.Ins = "CPW" Then
            Else
                MessageBox.Show("not good: " & logic.ToString)
        End If
    End Sub
    Private Sub FindCoilMapping(bst As Node, results As List(Of Tuple(Of String, String)))
        Dim count = 0
        For Each branch In bst.Children
            Dim cur = branch
            Dim rungSize = 1
            While cur.NextIns IsNot Nothing
                rungSize += 1
                cur = cur.NextIns
            End While
            If cur.Ins = "OR" Then
                'MessageBox.Show(branch.Ins)
                If rungSize = 2 AndAlso branch.Ins <> "BST" Then
                    Dim addr As String = "^(?:([A-Z]{1,3})(\d{1,3}):(\d{1,3})|(?:(I|O|S|U):(\d{1,3}(?:\.\d{1,3})*)))(?:\/(\d{1,2}))*(.*)$"
                    Dim regex As New Regex(addr)
                    Dim match As Match = regex.Match(branch.Args(0))
                    If match.Success = True Then
                        addr = match.Groups(0).ToString
                    End If
                    'MessageBox.Show(addr)
                    If ContainEntry(addr) AndAlso GetTagName(addr) <> "ALWAYS_OFF" Then
                        results.Add(New Tuple(Of String, String)(addr, cur.Args(0) & "/" & count))
                    End If
                ElseIf rungSize >= 2 Then
                    Dim temp = branch
                    Dim related = New List(Of Node)
                    While temp IsNot cur
                        related.Add(temp)
                        temp = temp.NextIns
                    End While
                    Dim des = cur.Args(0) & "/" & count
                    results.Add(New Tuple(Of String, String)(makeupmapping(related, des), des))
                    'MessageBox.Show("This coil depends on multiple addresses: " & branch.ToString)
                End If
                count += 1
            End If
        Next
    End Sub
    'This function is reserved for future use
    Private Function makeupmapping(related As List(Of Node), des As String) As String
        'If Not invalid_mapping.ContainsKey(des) Then
        '    invalid_mapping.add(des, "")
        'End If
        Dim addrs As List(Of String) = New List(Of String)
        Dim stored = des
        If Not ContainEntry(des) Then
            Add(des)
        End If
        SetMappingLogic(des, related.First)
        If related.First.Ins = "BST" Then
            addrDic(des).CopyNameAndDesp(addrDic(related.First.Children.First.Args(0)))
        Else
            addrDic(des).CopyNameAndDesp(addrDic(related.First.Args(0)))
        End If
        Return stored
    End Function

    Function ExtractAddresses(rung As String) As List(Of Tuple(Of String, String))
        ' Define the regular expression pattern for addr format
        Dim addr As String = "((?:[A-Z]{1,3}\d{1,3}:\d{1,3}|(?:(?:I|O|S|U):\d{1,3}(?:\.\d{1,3})*))(?:\/\d{1,2})*)" 'This regex requires to map the addr as a whole
        Dim regex As New Regex("MOV " & addr & " " & addr)
        Dim matches As MatchCollection = regex.Matches(rung)
        Dim results As New List(Of Tuple(Of String, String))
        For Each match In matches
            'MessageBox.Show(match.Groups(1).ToString & match.Groups(2).ToString)
            results.Add(New Tuple(Of String, String)(match.Groups(1).Value, match.Groups(2).Value))
        Next

        Return results
    End Function

End Class

Public Class DataEntry
    Private addr As String
    Private name As String
    Private desp As String
    Private des As List(Of String)
    Private src As List(Of String)
    Private modified As Boolean
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
        modified = False
        des = New List(Of String)
        src = New List(Of String)
    End Sub

    Public Sub New(address As String)
        Dim match As Match = addr_format.Match(address)
        If Not match.Success Then
            Throw New ArgumentException("Invalid address format: " & address)
        End If
        addr = address
        modified = False
        des = New List(Of String)
        src = New List(Of String)
    End Sub

    Public Sub CopyNameAndDesp(other As DataEntry)
        name = other.TagName
        desp = other.Description
        modified = True
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
            modified = True
            name = value
        End Set
    End Property

    Public Property Description As String
        Get
            Return desp
        End Get
        Set(value As String)
            modified = True
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

    Public Property isModified As Boolean
        Get
            Return modified
        End Get
        Set(value As Boolean)
            modified = value
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
        des.Add(addr)
    End Sub
    Public Sub AddMappedTo(addr As String)
        src.Add(addr)
    End Sub
    Public Property MappedTo As List(Of String)
        Get
            Return src
        End Get
        Set(value As List(Of String))
            src = value
        End Set
    End Property

End Class
