Imports System.IO
''' <summary>
''' PLC Database
''' An instance of this class stores all data of a RSS file. One database is binded to one file. To load a new file,
''' a new instance needs to be created.
''' </summary>
Public Class PLC_DB
    Private addrDic As Dictionary(Of String, DataEntry)
    Private tag_ref_list As Dictionary(Of String, String)
    Private programs As Object
    Private coil_start_addr As String = ""
    Private modbusList As List(Of String)
    ''' <summary>
    ''' The constructor of the PLC database
    ''' </summary>
    ''' <param name="proj">a logixProject object</param>
    Public Sub New(proj As Object)
        If proj Is Nothing Then
            Throw New ArgumentException("The project instance is NULL.")
        End If
        addrDic = New Dictionary(Of String, DataEntry)
        tag_ref_list = New Dictionary(Of String, String)
        Dim data_collection As Object = proj.AddrSymRecords
        LoadDataEntry(data_collection)
        programs = proj.ProgramFiles
        modbusList = New List(Of String)
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

    Private Function ChangeName(Name As String, extension As String) As String
        Dim words As String() = Name.Split("_")
        Dim changedName As String = ""
        For Each word In words
            If tag_ref_list.ContainsKey(word) Then
                changedName &= tag_ref_list(word) + "_"
            Else
                changedName &= word + "_"
            End If
        Next
        If extension <> "" Then
            changedName &= extension
        End If
        Return changedName
    End Function
    ''' <summary>
    ''' This function loads all the modbus mapping information into the current database.
    ''' </summary>
    Public Sub LoadMapping()
        coil_start = False
        modbusList = New List(Of String)
        Try
            LoadDefaultTagNameRef()
        Catch ex As FileNotFoundException
            MessageBox.Show(ex.Message)
            Return
        End Try
        Dim numOfProg = programs.Count()
        Dim modbus_file As Object
        For i As Integer = 0 To numOfProg - 1
            If programs.Item(i) IsNot Nothing AndAlso programs.Item(i).Name = "MODBUS" Then 'retrieve modbus program file
                modbus_file = programs.Item(i)
                Dim numOfRung = modbus_file.NumberOfRungs
                For j As Integer = 0 To numOfRung - 1 'iterate through rungs in the modbus file
                    Dim mappings = ExtractMapping(modbus_file.GetRungAsAscii(j)) 'for each rung, extract mappings embedded in it
                    For Each pair In mappings 'for each mapping pair
                        Dim extension As String = GetExtension(pair.Item1)
                        Dim srcInfo As String = pair.Item1
                        Dim src_addr As String = Tune(pair.Item1)
                        Dim des_addr As String = pair.Item2
                        If ContainEntry(src_addr) Then 'if db contain src
                            Dim src_name As String = GetTagName(src_addr)
                            If src_name Is Nothing OrElse src_name = "ALWAYS_OFF" Then 'skip always_off
                                Continue For
                            End If
                            modbusList.Add(des_addr)
                            If Not ContainEntry(des_addr) Then 'if no mapping target, add mapping target
                                Add(des_addr)
                            End If
                            addrDic(src_addr).AddMappingTo(des_addr)
                            addrDic(des_addr).AddMappedTo(pair.Item1)
                            If GetMappingSrc(des_addr).Count = 1 Then
                                Dim target_name As String = ChangeName(src_name, extension)
                                If target_name.Length > 20 Then
                                    MessageBox.Show("Tag Name exceeds 20 characters: " & target_name)
                                Else
                                    UpdateTagName(des_addr, target_name)
                                End If
                                UpdateDescription(des_addr, addrDic(src_addr).Description)
                            Else
                                UpdateTagName(des_addr, "")
                                UpdateDescription(des_addr, "")
                            End If
                        Else 'handles exception
                            If Not ContainEntry(des_addr) Then 'if no mapping target, add mapping target
                                Add(des_addr)
                            End If
                            modbusList.Add(des_addr)
                            addrDic(des_addr).AddMappedTo(srcInfo)
                        End If
                    Next
                Next
                Exit For
            End If
        Next
        modbusList = modbusList.Distinct().ToList()
    End Sub

    Public Function GetModbusData() As List(Of String())
        If modbusList.Count = 0 Then
            FindModbusData()
        End If
        Dim content = New List(Of String())
        Dim comp As New AddrComparer
        modbusList.Sort(comp)
        Dim coil_start As Integer = ConvertCoilAddr(coil_start_addr)
        For Each addr In modbusList
            If comp.Compare(addr, coil_start_addr.ToString) > 0 Then
                content.Add({ConvertCoilAddr(addr).ToString, GetTagName(addr), GetDescription(addr)})
            Else
                content.Add({ConvertRegAddr(addr).ToString, GetTagName(addr), GetDescription(addr)})
            End If
        Next
        Return content
    End Function


    Public Sub FindModbusData()
        coil_start = False
        Dim numOfProg = programs.Count()
        Dim modbus_file As Object
        For i As Integer = 0 To numOfProg - 1
            If programs.Item(i) IsNot Nothing AndAlso programs.Item(i).Name = "MODBUS" Then 'retrieve modbus program file
                modbus_file = programs.Item(i)
                Dim numOfRung = modbus_file.NumberOfRungs
                For j As Integer = 0 To numOfRung - 1 'iterate through rungs in the modbus file
                    Dim mappings = ExtractMapping(modbus_file.GetRungAsAscii(j)) 'for each rung, extract mappings embedded in it
                    For Each pair In mappings 'for each mapping pair
                        Dim extension As String = GetExtension(pair.Item1)
                        Dim src_addr As String = Tune(pair.Item1)
                        Dim des_addr As String = pair.Item2
                        If ContainEntry(src_addr) AndAlso ContainEntry(des_addr) Then 'if db contain src
                            Dim src_name As String = GetTagName(src_addr)
                            If src_name Is Nothing OrElse src_name = "ALWAYS_OFF" Then 'skip always_off
                                Continue For
                            End If
                            modbusList.Add(des_addr)
                        End If
                    Next
                Next
                Exit For
            End If
        Next
        modbusList = modbusList.Distinct().ToList()
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
    Public Function GetModbusList() As List(Of String)
        Return modbusList
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
                    coil_start_addr = words(i + 2)
                Else
                    Dim logic As Node = Parser.Parse(New LinkedList(Of String)(words))
                    RegLogicAnalyzer.FindRegMapping(logic, results)
                    Return results
                End If
            End If
            'Check if the current rung is coil mapping
            If coil_start AndAlso (words(i) = "OR" OrElse words(i) = "SWP") Then
                ' MessageBox.Show(str)
                Dim logic As Node = Parser.Parse(New LinkedList(Of String)(words))
                Dim cur As Node = logic
                CoilLogicAnalyzer.FindCoilMapping(Me, logic, results)
                Return results
            End If
        Next
        Return results
    End Function

    Public Function GetInvalidMapping() As List(Of String())
        Dim content = New List(Of String())
        For Each addr In addrDic.Keys
            If addrDic(addr).MappingTo.Count <= 1 AndAlso addrDic(addr).MappedTo.Count <= 1 Then
                Continue For
            End If
            Dim des As String = ""
            If addrDic(addr).MappingTo.Count > 1 Then
                Dim target1 As String = GetMappingTarget(addr)(0)
                Dim target2 As String = GetMappingTarget(addr)(1)
                If GetTagName(target1) = GetTagName(target2) Then
                    For Each d In addrDic(addr).MappingTo
                        des &= d + " "
                    Next
                Else
                    Continue For
                End If
            End If
            Dim src As String = ""
            If addrDic(addr).MappedTo.Count > 1 Then
                For Each target In addrDic(addr).MappedTo
                    src &= target + " "
                Next
            End If
            content.Add({addr, des, src})
        Next
        content.Sort(New DataEntryComparer)
        Return content
    End Function

    Public Function GetExceptionMapping() As List(Of String())
        Dim content = New List(Of String())
        For Each addr In modbusList
            Dim src As String = ""
            For Each target In GetMappingSrc(addr)
                src &= target + " "
            Next
            If GetTagName(addr) = "" OrElse GetDescription(addr) = "" Then
                content.Add({addr, GetTagName(addr), src, GetDescription(addr)})
            End If
        Next
        content.Sort(New DataEntryComparer)
        Return content
    End Function

    Public Sub LoadDefaultTagNameRef()
        Dim exePath As String = System.Windows.Forms.Application.StartupPath
        Dim filePath As String = Path.Combine(exePath, "ref.xlsx")
        ' Check if the file exists
        If File.Exists(filePath) Then
            tag_ref_list = LoadExcel(filePath)
        Else
            Throw New FileNotFoundException("The default translation table " & filePath & " Not Found!")
        End If
    End Sub
    Public Property TagAbbrDictionary As Dictionary(Of String, String)
        Get
            Return tag_ref_list
        End Get
        Set(value As Dictionary(Of String, String))
            tag_ref_list = value
        End Set
    End Property

    Public Function IsX() As Boolean
        FindModbusData()
        For Each addr In modbusList
            If GetTagName(addr) Is Nothing Or GetTagName(addr) = "" Then
                Return False
            End If
        Next
        Return True
    End Function
End Class
