Imports System.IO

Public Class Form1

    Private buttons As New List(Of Windows.Forms.Button)
    Private logixApp As Object = CreateObject("RSLogix500.Application")
    Private logixObj As Object
    Private data_collection As Object
    Private db As PLC_DB
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        buttons = New List(Of Windows.Forms.Button) From {Search, display_data_button, perform_mapping, find_invalid_mapping_button}
        For Each btn In buttons
            btn.Enabled = False
        Next

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
                If logixObj IsNot Nothing Then
                    'change the second to true to save
                    logixObj.close(True, False)
                End If
            ElseIf result = DialogResult.No Then
                Return
            End If
        End If

        logixObj = LoadRSSFile(logixApp)
        If logixObj Is Nothing Then
            Return
        End If
        'preparing a new database
        db = New PLC_DB(logixObj)

        'enable all buttons
        For Each btn In buttons
            btn.Enabled = True
        Next
        MessageBox.Show("PLC program successfully loaded.")
    End Sub


    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If logixObj IsNot Nothing Then
            'change the second to true to save
            logixObj.close(True, False)
            logixObj = Nothing
        End If
        If logixApp IsNot Nothing Then
            logixApp.Quit(True, False)
            logixApp = Nothing
        End If
    End Sub


    Private Sub Search_Click(sender As Object, e As EventArgs) Handles Search.Click
        Dim addr As String = TextBox1.Text
        addr = addr.Trim()

        Dim rowIndex = -1
        ' Assuming you want to search in the first column of the DataGridView 
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


    Private invalid_mapping As Dictionary(Of String, String) = New Dictionary(Of String, String)
    Private Sub find_invalid_mapping_button_click(sender As Object, e As EventArgs) Handles find_invalid_mapping_button.Click
        Dim invalid = 0
        Dim content = New List(Of String())
        Dim reverse_mapping As New Dictionary(Of String, List(Of String))
        For Each addr In db.GetModifiedEntries
            For Each des In db.GetMappingSrc(addr)
                If reverse_mapping.ContainsKey(des) Then
                    reverse_mapping(des).Add(addr)
                    If reverse_mapping(des).Count > 1 Then
                        If Not invalid_mapping.ContainsKey(des) Then
                            invalid_mapping.Add(des, "")
                        End If
                    End If
                Else
                    reverse_mapping.Add(des, New List(Of String) From {addr})
                End If
            Next
        Next
        For Each addr In invalid_mapping.keys
            content.Add({addr})
        Next
        content.Sort(New DataEntryComparer)
        WriteToCSV(content, "C:\Users\37239\OneDrive - Entegris\Desktop\export.csv")
        DisplayList(content, {"Address"})
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
        db.LoadMapping(logixObj.ProgramFiles)
        DisplayList(WriteToProject(logixObj, db), {"Address", "Name", "Description"})
        MessageBox.Show("Successfully applied mappings to the project.")
    End Sub


    Private Sub Preview_Change_Button_Click(sender As Object, e As EventArgs)
        Dim programs As Object = logixObj.ProgramFiles
        db.LoadMapping(programs)
        Dim modifiedList = db.GetModifiedEntries()
        Dim content = New List(Of String())
        For Each addr In modifiedList
            content.Add({addr, db.GetTagName(addr), db.GetDescription(addr)})
        Next
        db.ChangeModifiedStatus(modifiedList)
        content.Sort(New DataEntryComparer())
        DisplayList(content, {"Address", "Name", "Description"})
    End Sub


    Private Sub DisplayList(list As List(Of String()), cols As String())
        ' Create a new DataTable.
        Dim table As New DataTable()

        ' Assuming the string arrays all have the same length.
        For Each colName As String In cols
            table.Columns.Add(colName)
        Next

        ' Populate the DataTable from the List
        For Each item() As String In list
            table.Rows.Add(item)
        Next

        ' Bind the DataTable to the DataGridView
        DataGridView1.DataSource = Nothing
        DataGridView1.DataSource = table
        DataGridView1.RowHeadersVisible = False
        DataGridView1.AutoResizeColumns()
    End Sub
End Class
