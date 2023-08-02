Imports System.IO
Imports System.Windows.Forms.AxHost
Imports System.Runtime.InteropServices
Imports Microsoft.Office.Interop.Excel
Imports Microsoft.Office.Interop

Public Module IOHandler
    ''' <summary>
    ''' This function writes data to a CSV file whose path is spcified by the caller function.
    ''' </summary>
    ''' <param name="data">Data to be written to the file</param>
    ''' <param name="path">The path of the target file</param>
    ''' <returns>A boolean that indicates whether the operation is successful or not.</returns>
    Public Function WriteToCSV(data As List(Of String()), path As String) As Boolean
        If File.Exists(path) Then
            Try
                Using fs As FileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None)
                    ' If the file can be opened, it is not in use
                End Using
            Catch ex As IOException
                ' If an IOException occurred, the file is in use
                MessageBox.Show("File is being used by other applications.")
                Return False
            End Try
        Else
            Dim fs = File.Create(path)
            fs.Close()
        End If
        Using sw As New StreamWriter(path)
            For Each row As String() In data
                sw.WriteLine(String.Join(",", row))
            Next
        End Using
        MessageBox.Show("Success!")
        Return True
    End Function

    ''' <summary>
    ''' This function applies all changes in the database to the RSS project file.
    ''' </summary>
    ''' <param name="proj">The RSS project</param>
    ''' <param name="db">The database</param>
    ''' <returns>A list of string array including all changed data entry which can be
    ''' then displayed on the datagrid using DisplayList().</returns>
    Public Function WriteToProject(proj As Object, db As PLC_DB) As List(Of String())
        Dim modifiedList = db.GetModifiedEntries()
        Dim content = New List(Of String())
        For Each addr In modifiedList
            Dim record = proj.AddrSymRecords.GetRecordViaAddrOrSym(addr, 0)
            If record Is Nothing Then
                record = proj.AddrSymRecords.add()
                'MessageBox.Show("creating new addr: " + s)
                record.SetAddress(addr)
                record.SetScope(0)
            End If 'got the des addr instance
            record.SetSymbol(db.GetTagName(addr))
            record.SetDescription(db.GetDescription(addr))
            Dim str As String = ""
            For Each src In db.GetMappingSrc(addr)
                str &= src + " "
            Next
            content.Add({addr, db.GetTagName(addr), str, db.GetDescription(addr)})
        Next
        'db.ChangeModifiedStatus(modifiedList)
        content.Sort(New DataEntryComparer())
        Return content
    End Function

    ''' <summary>
    ''' This function loads a RSS file
    ''' </summary>
    ''' <param name="rslogixApp">The RsLogix 500 Pro Application object that is used to open an RSS file</param>
    ''' <param name="path">Thepath to the RSS file to be loaded</param>
    ''' <returns>A RSS project object obtained from the RSS file</returns>
    Public Function LoadRSSFile(path As String, rslogixApp As Object) As Object
        'Obtain path chose by the user
        Dim openFileDialog As New OpenFileDialog
        Dim extension = IO.Path.GetExtension(path)
        If extension <> ".RSS" Then
            MessageBox.Show("The file must be an RSS file.")
            Return Nothing
        End If
        'check if file is in use
        If File.Exists(path) Then
            Try
                Using fs As FileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None)
                End Using
            Catch ex As IOException
                MessageBox.Show("File is being used by other applications.")
                Return Nothing
            End Try
        Else
            Dim fs = File.Create(path)
            fs.Close()
        End If
        'open the file
        Dim rslogixProj = rslogixApp.FileOpen(path, False, False, True)
        If rslogixProj Is Nothing Then
            MessageBox.Show("ERROR: Failed to open the file.")
            Return Nothing
        End If
        Return rslogixProj
    End Function

    Public Function LoadExcel(path As String) As Dictionary(Of String, String)

        Dim excelApp As New Application()

        ' Open the workbook (file) in read-only mode
        Dim workbook As Workbook = excelApp.Workbooks.Open(path, [ReadOnly]:=True)

        ' Assuming the data is in the first worksheet (you can adjust the sheet index as needed)
        Dim worksheet As Worksheet = DirectCast(workbook.Worksheets(1), Worksheet)
        Dim range As Excel.Range = worksheet.UsedRange

        Dim numRows As Integer = range.Rows.Count
        Dim dataDictionary As New Dictionary(Of String, String)

        ' Loop through the data and add to the dictionary
        For row As Integer = 1 To numRows
            Dim key As String = CType(range.Cells(row, 1).Value, String) ' Assuming the key is in the first column
            Dim value As String = CType(range.Cells(row, 2).Value, String) ' Assuming the value is in the second column

            If Not String.IsNullOrEmpty(key) Then ' Ignore rows with empty keys
                dataDictionary.Add(key, value)
            End If
        Next

        ' Close the workbook and release resources
        workbook.Close()
        excelApp.Quit()
        ReleaseObject(worksheet)
        ReleaseObject(workbook)
        ReleaseObject(excelApp)

        Return dataDictionary
    End Function

    Private Sub ReleaseObject(ByVal obj As Object)
        Try
            System.Runtime.InteropServices.Marshal.ReleaseComObject(obj)
            obj = Nothing
        Catch ex As Exception
            obj = Nothing
        Finally
            GC.Collect()
        End Try
    End Sub

End Module
