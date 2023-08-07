Imports System.IO
Imports System.Threading

Public Class Form1

    Private buttons As New List(Of Windows.Forms.Button)
    Private logixApp As Object = CreateObject("RSLogix500.Application")
    Private logixObj As Object
    Private data_collection As Object
    Private db As PLC_DB
    Private BGTask As Thread
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        buttons = New List(Of Windows.Forms.Button) From {Search, display_data_button, perform_mapping, find_invalid_mapping_button, load_ref_table_button}

        For Each btn In buttons 'When the software is open. Disable all buttons except load file button
            btn.Enabled = False
        Next
        'PictureBox1.Image = My.Resources.blank_image
    End Sub

    Private Sub load_file_Click(sender As Object, e As EventArgs) Handles load_file_button.Click
        'checking if rslogix500 is opened successfully
        If logixApp Is Nothing Then
            MessageBox.Show("ERROR: Failed to open RSLogix500 software.",
                            "ERROR: 001",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation)
            Return
        End If
        'Checking if a file is already loaded
        If db IsNot Nothing Then
            Dim result As DialogResult = MessageBox.Show("Are you sure to load a new file?", "A File Has Been Loaded", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                For Each btn In buttons
                    btn.Enabled = False
                Next
                DataGridView1.Rows.Clear()
                DataGridView1.Columns.Clear()
                rss_path.Text = "Not Loaded"
                If logixObj IsNot Nothing Then
                    'change the second to true to save
                    logixObj.Close(False, False)
                End If
            ElseIf result = DialogResult.No Then
                Return
            End If
        End If
        'open file load dialog
        Dim openFileDialog As New OpenFileDialog
        Dim path As String
        If openFileDialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            path = openFileDialog.FileName
        Else
            Return
        End If
        logixObj = LoadRSSFile(path, logixApp)
        If logixObj Is Nothing Then
            Return
        End If
        'preparing a new database
        db = New PLC_DB(logixObj)
        StartLoadDBDefaultFile()
        'display rss file
        rss_path.Text = path
        'enable all buttons
        For Each btn In buttons
            btn.Enabled = True
        Next
        find_invalid_mapping_button.Enabled = False
        MessageBox.Show("PLC program successfully loaded.")
        Focus() 'Keep the window in the view after closing the message box
    End Sub


    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If logixObj IsNot Nothing Then
            'change the second to true to save
            logixObj.close(False, True)
            logixObj = Nothing
        End If
        If logixApp IsNot Nothing Then
            logixApp.Quit(True, False)
            logixApp = Nothing
        End If
    End Sub


    Private Sub Search_Click(sender As Object, e As EventArgs) Handles Search.Click
        Dim addr As String = TextBox1.Text
        addr = addr.Trim() 'get rid of all leading and tailing spaces
        Dim rowIndex = -1
        For Each row As DataGridViewRow In DataGridView1.Rows
            If row.Cells(0).Value IsNot Nothing AndAlso row.Cells(0).Value.ToString().Equals(addr) Then
                rowIndex = row.Index
                Exit For
            End If
        Next
        If rowIndex <> -1 Then
            ' Clear the selection first
            DataGridView1.ClearSelection()
            ' Select the found row
            DataGridView1.Rows(rowIndex).Selected = True
            ' Scroll to the found row if it is not currently visible
            If Not DataGridView1.Rows(rowIndex).Displayed Then
                DataGridView1.FirstDisplayedScrollingRowIndex = rowIndex
            End If
        End If

        If db.ContainEntry(addr) Then
            'MessageBox.Show("Name: " + dataEntries(addr).Item1 + vbNewLine + "Description: " + dataEntries(addr).Item2)
            Label2.Text = db.GetTagName(addr)
        Else
            MessageBox.Show("Address not found.")
        End If
        If db.ContainEntry(addr) Then
            Dim s As String = ""
            Dim l As List(Of String) = db.GetMappingTarget(addr)
            For Each i In l
                s = s + i + " "
            Next
            Label6.Text = s
        Else
            Label6.Text = "Empty"
        End If
    End Sub

    Private Sub find_invalid_mapping_button_click(sender As Object, e As EventArgs) Handles find_invalid_mapping_button.Click
        Dim content As List(Of String()) = db.GetInvalidMapping()
        Dim exePath As String = System.Windows.Forms.Application.StartupPath
        Dim filePath As String = Path.Combine(exePath, "invalid.csv")
        If File.Exists(filePath) Then
            Try
                Using fs As FileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None)
                    ' If the file can be opened, it is not in use
                End Using
            Catch ex As IOException
                ' If an IOException occurred, the file is in use
                MessageBox.Show("File is being used by other applications.")
            End Try
        Else
            Dim fs = File.Create(filePath)
            fs.Close()
        End If
        WriteToCSV(content, {"Address", "Mapping To", "Mapped To"}, filePath)
        DisplayList(content, {"Address", "Mapping To", "Mapped To"})
    End Sub

    ' Handle the KeyDown event for your textbox
    Private Sub TextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown
        ' Check if the pressed key is the Enter key
        If e.KeyCode = Keys.Enter Then
            ' Handle your search logic same as clicking on the button
            Search_Click(sender, e)
        End If
    End Sub

    Private Sub display_data_button_click(sender As Object, e As EventArgs) Handles display_data_button.Click
        DisplayList(db.DBtoList(), {"Address", "Name", "Description"})
        Return
    End Sub

    Private Sub perform_mapping_Click(sender As Object, e As EventArgs) Handles perform_mapping.Click
        WaitForBGTask()
        db.LoadMapping()
        find_invalid_mapping_button.Enabled = True
        DisplayListWithEditing(WriteToProject(logixObj, db), {"Address", "Name", "Source", "Description"})
    End Sub

    Private Sub DisplayList(list As List(Of String()), cols As String())
        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        ' Assuming the string arrays all have the same length.
        For Each colName As String In cols
            Dim col As New DataGridViewTextBoxColumn
            col.Name = colName
            DataGridView1.Columns.Add(col)
        Next

        For Each item() As String In list
            DataGridView1.Rows.Add(item)
        Next

        DataGridView1.RowHeadersVisible = False
        DataGridView1.AutoResizeColumns()
        DataGridView1.AllowUserToAddRows = False
    End Sub

    Private Sub DisplayListWithEditing(list As List(Of String()), cols As String())
        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        ' Assuming the string arrays all have the same length.
        For Each colName As String In cols
            Dim col As New DataGridViewTextBoxColumn
            col.Name = colName
            DataGridView1.Columns.Add(col)
        Next

        For Each item() As String In list
            DataGridView1.Rows.Add(item)
        Next

        DataGridView1.RowHeadersVisible = False
        DataGridView1.ReadOnly = False
        DataGridView1.AutoResizeColumns()
        DataGridView1.AllowUserToAddRows = False
    End Sub

    Private Sub DataGridView1_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles DataGridView1.CellBeginEdit
        ' Determine which column and row to allow editing
        Dim columnIndexToAllowEdit As Integer = 3 ' Adjust this to the column index you want to allow editing (0-based index)
        Dim cellContent As String = DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
        If e.ColumnIndex = columnIndexToAllowEdit AndAlso DataGridView1.Columns(columnIndexToAllowEdit).HeaderText = "Description" Then
            ' Allow editing for the specified cell
            If cellContent Is Nothing OrElse cellContent = "" Then
                e.Cancel = False
                Dim desp As Object = DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
                Dim addr As Object = DataGridView1.Rows(e.RowIndex).Cells(0).Value
                db.UpdateDescription(addr, desp)
            Else
                e.Cancel = True
            End If
        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub DataGridView1_CanEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick, DataGridView1.CellEnter
        Dim editableCols As New HashSet(Of Integer)({1, 3})
        If editableCols.Contains(e.ColumnIndex) AndAlso DataGridView1.Columns(3).HeaderText = "Description" Then
            If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 AndAlso Not DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).ReadOnly Then
                ' Begin editing the clicked cell
                DataGridView1.BeginEdit(False)
            End If
        End If

    End Sub
    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridView1.CellFormatting
        ' Check if the cell's value is empty or Nothing
        Dim columnHeaderText As String = DataGridView1.Columns(e.ColumnIndex).HeaderText
        If (columnHeaderText = "Name" OrElse columnHeaderText = "Description") AndAlso (e.ColumnIndex = 3 OrElse e.ColumnIndex = 1) Then
            If e.Value Is Nothing OrElse String.IsNullOrEmpty(e.Value.ToString()) Then
                ' Change the background color of the empty cell to red
                e.CellStyle.BackColor = Color.Red
                e.CellStyle.ForeColor = Color.White ' Optionally, you can also set the text color for better visibility
            End If
        End If
    End Sub


    Private Sub load_ref_table_Click(sender As Object, e As EventArgs) Handles load_ref_table_button.Click
        Dim openFileDialog As New OpenFileDialog
        Dim path As String
        If openFileDialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            path = openFileDialog.FileName
            Dim extension = IO.Path.GetExtension(path)
            If extension <> ".xlsx" Then
                MessageBox.Show("The file must be an Excel file.")
                Return
            End If
        Else
            Return
        End If
        'check if file is in use
        If File.Exists(path) Then
            Try
                Using fs As FileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None)
                End Using
            Catch ex As IOException
                MessageBox.Show("File is being used by other applications.")
                Return
            End Try
        Else
            Dim fs = File.Create(path)
            fs.Close()
        End If
        db.TagAbbrDictionary = IOHandler.LoadExcel(path)
        ref_file_status.Text = path
        MessageBox.Show("Reference File Loaded.")
    End Sub

    Dim toggle As Boolean = False
    Private Sub DataGridView1_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridView1.ColumnHeaderMouseClick
        ' Check if the clicked column is the one you want to perform custom sorting on.
        If e.ColumnIndex = 0 Then
            DataGridView1.Sort(New DataGridEntryComparer(toggle))
            toggle = Not toggle
        End If
    End Sub


    Private Sub StartLoadDBDefaultFile()
        BGTask = New Thread(AddressOf LoadDefaultFile)
        BGTask.Start()
    End Sub

    Private Sub LoadDefaultFile()
        Try
            db.LoadDefaultTagNameRef()
        Catch ex As FileNotFoundException
            Return
        End Try
    End Sub


    Private Sub WaitForBGTask()
        ' Use Join to wait for the background thread to complete
        If BGTask IsNot Nothing AndAlso BGTask.IsAlive Then
            BGTask.Join()
        End If
    End Sub


End Class
