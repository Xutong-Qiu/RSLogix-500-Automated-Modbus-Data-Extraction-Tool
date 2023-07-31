Imports System.IO
Imports System.Windows.Forms.AxHost
Imports System.Runtime.InteropServices
Imports Microsoft.Office.Interop.Excel

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
    ''' <returns>A RSS project object obtained from the RSS file</returns>
    Public Function LoadRSSFile(rslogixApp As Object) As Object
        'Obtain path chose by the user
        Dim openFileDialog As New OpenFileDialog
        Dim path As String
        If openFileDialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            path = openFileDialog.FileName
            Dim extension = IO.Path.GetExtension(path)
            If extension <> ".RSS" Then
                MessageBox.Show("The file must be an RSS file.")
                Return Nothing
            End If
        Else
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

    Public Function LoadCSV() As List(Of String())
        Dim openFileDialog As New OpenFileDialog
        Dim path As String
        If openFileDialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            path = openFileDialog.FileName
            Dim extension = IO.Path.GetExtension(path)
            If extension <> ".CSV" Then
                MessageBox.Show("The file must be an CSV file.")
                Return Nothing
            End If
        Else
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

        Dim excelApp As New Application()

        ' Open the workbook (file) in read-only mode
        Dim workbook As Workbook = excelApp.Workbooks.Open(path, [ReadOnly]:=True)

        ' Assuming the data is in the first worksheet (you can adjust the sheet index as needed)
        Dim worksheet As Worksheet = DirectCast(workbook.Worksheets(1), Worksheet)

        ' Get the range of cells containing data
        Dim range As Range = worksheet.UsedRange

        ' Get the number of rows and columns with data
        Dim rowCount As Integer = range.Rows.Count
        Dim columnCount As Integer = range.Columns.Count

        ' Read the data from the Excel file
        Dim data As New List(Of String())

        ' Read the data from the Excel file and add it to the list
        For i As Integer = 1 To rowCount
            Dim rowValues(columnCount - 1) As String
            For j As Integer = 1 To columnCount
                rowValues(j - 1) = range.Cells(i, j).Value.ToString()
            Next
            data.Add(rowValues)
        Next

        ' Close the workbook and release resources
        workbook.Close()
        Marshal.ReleaseComObject(range)
        Marshal.ReleaseComObject(worksheet)
        Marshal.ReleaseComObject(workbook)

        ' Quit Excel application and release resources
        excelApp.Quit()
        Marshal.ReleaseComObject(excelApp)

        ' Return the list of string arrays containing the Excel file data
        Return data

    End Function
End Module
