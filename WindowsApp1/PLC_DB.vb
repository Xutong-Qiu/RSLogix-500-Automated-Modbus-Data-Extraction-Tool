Imports System.Text.RegularExpressions
''' <summary>
''' PLC Database
''' An instance of this class stores all data of a RSS file. One database is binded to one file. To load a new file,
''' a new instance needs to be created.
''' </summary>
Public Class PLC_DB
    Private addrDic As Dictionary(Of String, DataEntry)
    Private tag_ref_list As Dictionary(Of String, String)
    Private programs As Object

    ''' <summary>
    ''' The constructor of the PLC database
    ''' </summary>
    ''' <param name="proj">a logixProject object</param>
    Public Sub New(proj As Object)
        If proj Is Nothing Then
            Throw New ArgumentException("The project instance is NULL.")
        End If
        addrDic = New Dictionary(Of String, DataEntry)
        Dim data_collection As Object = proj.AddrSymRecords
        LoadDataEntry(data_collection)
        programs = proj.ProgramFiles
    End Sub
    ''' <summary>
    ''' This function checks whether the current database is empty
    ''' </summary>
    ''' <returns>A boolean that tells whether the current database is empty.</returns>
    Public Function Empty() As Boolean
        Return addrDic.Count = 0
    End Function
    ''' <summary>
    ''' This function converts the current database into a list of string arrays that can be easily
    ''' displayed on a data grid.
    ''' </summary>
    ''' <returns>A list of string array. Each array represents a data entry and has three
    ''' elements: the entry's address, tag name, and description. Index is in order.</returns>
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
            If record.Address IsNot Nothing And record.Address <> "" Then
                Add(record.Address, record.Symbol, desp)
            End If
        Next
    End Sub

    ''' <summary>
    ''' This function loads all the modbus mapping information into the current database.
    ''' </summary>
    Public Sub LoadMapping()
        Dim numOfProg = programs.Count()
        Dim modbus_file As Object
        For i As Integer = 0 To numOfProg - 1
            If programs.Item(i) IsNot Nothing AndAlso programs.Item(i).Name = "MODBUS" Then 'retrieve modbus program file
                modbus_file = programs.Item(i)
                Dim numOfRung = modbus_file.NumberOfRungs
                For j As Integer = 0 To numOfRung - 1 'iterate through rungs in the modbus file
                    Dim mappings = ExtractMapping(modbus_file.GetRungAsAscii(j)) 'for each rung, extract mappings embedded in it
                    For Each pair In mappings 'for each mapping pair
                        If ContainEntry(pair.Item1) Then 'if bd contain src
                            Dim name As String = addrDic(pair.Item1).TagName
                            If name = "ALWAYS_OFF" Then 'skip always_off
                                Continue For
                            End If
                            If Not ContainEntry(pair.Item2) Then 'if no mapping target, add mapping target
                                Add(pair.Item2)
                            End If
                            If name IsNot Nothing AndAlso name.Length + 1 >= 19 Then
                                name = name.Substring(0, name.Length - 1) + "_"
                            Else
                                name += "_"
                            End If
                            UpdateTagName(pair.Item2, name)
                            UpdateDescription(pair.Item2, addrDic(pair.Item1).Description)
                            addrDic(pair.Item1).AddMappingTo(pair.Item2)
                            addrDic(pair.Item2).AddMappedTo(pair.Item1)
                        End If
                    Next
                Next
                Exit For
            End If
        Next
    End Sub

    ''' <summary>
    ''' This function checks whether a entry with the given address is present in the data base.
    ''' This function must be called before any attempt to access an entry in this database to 
    ''' avoid a possible exception.
    ''' </summary>
    ''' <param name="addr">The address of the entry</param>
    ''' <returns>A boolean that indicates whether the entry is present or not.</returns>
    Public Function ContainEntry(addr As String) As Boolean
        Return addrDic.ContainsKey(addr)
    End Function
    ''' <summary>
    ''' This function returns the tag name with the given address.
    ''' </summary>
    ''' <param name="addr">The address of the entry</param>
    ''' <returns>A string that is the tag name with the given address.</returns>
    Public Function GetTagName(addr As String) As String
        If Not addrDic.ContainsKey(addr) Then
            Throw New KeyNotFoundException("The data entry is not presented in the database: " & addr)
        End If
        Return addrDic(addr).TagName
    End Function
    ''' <summary>
    ''' This function returns the description with the given address.
    ''' </summary>
    ''' <param name="addr">The address of the entry</param>
    ''' <returns>A string that is the description with the given address.</returns>
    Public Function GetDescription(addr As String) As String
        If Not addrDic.ContainsKey(addr) Then
            Throw New KeyNotFoundException("The data entry is not presented in the database: " & addr)
        End If
        Return addrDic(addr).Description
    End Function
    ''' <summary>
    ''' This function returns the mapping target of the data entry with the given address. 
    ''' i.e. the addresses that this data entry maps to. 
    ''' </summary>
    ''' <param name="addr">The address of the entry</param>
    ''' <returns>A list of string that is the addresses of the mapping targets.</returns>
    Public Function GetMappingTarget(addr As String) As List(Of String)
        If Not addrDic.ContainsKey(addr) Then
            Throw New KeyNotFoundException("The data entry is not presented in the database: " & addr)
        End If
        Return addrDic(addr).MappingTo
    End Function
    ''' <summary>
    ''' This function returns the mapping source of the data entry with the given address. 
    ''' i.e. the addresses that this data entry is mapped to. 
    ''' </summary>
    ''' <param name="addr">The address of the entry</param>
    ''' <returns>A list of string that is the addresses of the mapping sources.</returns>
    Public Function GetMappingSrc(addr As String) As List(Of String)
        If Not addrDic.ContainsKey(addr) Then
            Throw New KeyNotFoundException("The data entry is not presented in the database: " & addr)
        End If
        Return addrDic(addr).MappedTo
    End Function
    ''' <summary>
    ''' This function adds a new entry to the database. It has a overload version that
    ''' only takes in the address.
    ''' </summary>
    ''' <param name="addr">The address of the entry</param>
    ''' <param name="name">The tag nane of the entry</param>
    ''' <param name="desp">The description of the entry</param>
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
    ''' <summary>
    ''' This function adds a new entry to the database. It has a overload version that
    ''' takes in the address, tag name, and the description.
    ''' </summary>
    ''' <param name="addr">The address of the entry</param>
    Public Sub Add(addr As String)
        If addrDic.ContainsKey(addr) Then
            Throw New ArgumentException("This data entry has presented in the database: " & addr)
        End If
        addrDic.Add(addr, New DataEntry(addr))
    End Sub
    ''' <summary>
    ''' This function sets an entry with new tag name.
    ''' </summary>
    ''' <param name="addr">The address of the entry</param>
    Public Sub UpdateTagName(addr As String, name As String)
        If Not addrDic.ContainsKey(addr) Then
            Throw New KeyNotFoundException("The data entry is not presented in the database: " & addr)
        End If
        addrDic(addr).TagName = name
    End Sub
    ''' <summary>
    ''' This function sets an entry with a new description.
    ''' </summary>
    ''' <param name="addr">The address of the entry</param>
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
    ''' <summary>
    ''' This function returns the mapping logic the involves the data entry with
    ''' the given address. It will be used in dealing with many to one mapping
    ''' </summary>
    ''' <param name="addr">The address of the entry</param>
    ''' <returns>A node that represents the logic.</returns>
    Public Function GetMappingLogic(addr As String) As Node
        If Not addrDic.ContainsKey(addr) Then
            Throw New KeyNotFoundException("The data entry is not presented in the database: " & addr)
        End If
        Return addrDic(addr).MappingLogic
    End Function
    ''' <summary>
    ''' This function finds all modified data entries after perform modbus mapping.
    ''' </summary>
    ''' <returns>A list of addresses that represents the modifed data entries.</returns>
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
    'loadMapping() calls this function. It takes in a rung in text form and returns
    'all the mappings it finds as a list of tuples.
    Private Function ExtractMapping(str As String) As List(Of Tuple(Of String, String))
        Dim words As String() = str.Split(" "c)
        Dim results As New List(Of Tuple(Of String, String))
        For i As Integer = 0 To words.Length - 3
            'Check if the current rung is register mapping
            If coil_start = False AndAlso words(i) = "MOV" Then
                If ContainEntry(words(i + 2)) AndAlso GetTagName(words(i + 2)) = "COIL_START" Then
                    coil_start = True
                Else
                    Dim logic As Node = Parser.Parse(New LinkedList(Of String)(words))
                    RegLogicAnalyzer.FindRegMapping(logic, results)
                    Return results
                End If
            End If
            'Check if the current rung is coil mapping
            If coil_start AndAlso words(i) = "OR" Then
                ' MessageBox.Show(str)
                Dim logic As Node = Parser.Parse(New LinkedList(Of String)(words))
                Dim cur As Node = logic
                CoilLogicAnalyzer.FindCoilMapping(logic, results)
                While cur IsNot Nothing
                    If cur.Ins = "BST" Then
                        FindCoilMapping(cur, results)
                    End If
                    cur = cur.NextIns
                End While
                Return results
                'check if the current rung is SWP coil mapping
            ElseIf words(i) = "SWP" Then
                Dim temp = ExtractAddresses(str)
                Dim src As String = ""
                Dim des As String = ""
                For Each pair As Tuple(Of String, String) In temp
                    If ContainEntry(pair.Item2) AndAlso GetTagName(pair.Item2) = "WORD_TO_REVERSE" Then
                        src = pair.Item1
                    ElseIf ContainEntry(pair.Item1) AndAlso GetTagName(pair.Item1) = "REVERSED_WORD" Then
                        des = pair.Item2
                    End If
                Next
                'if self map to self, not gonna do anything
                If src <> des Then
                    For count As Integer = 0 To 15
                        Dim full_addr1 = src & "/" & count.ToString
                        Dim full_addr2 = des & "/" & count.ToString
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

    'Extractmapping() calls this function to find coil mapping.
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
                    'This regex only matches everything before postfix(i.e. exclude \EN)
                    Dim addr As String = "(?:([A-Z]{1,3})(\d{1,3}):(\d{1,3})|(?:(I|O|S|U):(\d{1,3}(?:\.\d{1,3})*)))(?:\/(\d{1,2}))*"
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
    'ExtractMapping calls this function to extract the arguments of mov instruction
    Private Function ExtractAddresses(rung As String) As List(Of Tuple(Of String, String))
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
