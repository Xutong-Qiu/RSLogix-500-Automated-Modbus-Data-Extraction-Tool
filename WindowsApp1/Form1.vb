Imports System.IO

Public Class Form1

    Private dataEntries As New Dictionary(Of String, Tuple(Of String, String))
    Private modbusDic As New Dictionary(Of String, List(Of String))
    Private buttons As New List(Of Windows.Forms.Button)
    Private logixApp As Object = CreateObject("RSLogix500.Application")
    Private logixObj As Object
    Private data_collection As Object

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
        If dataEntries.Count <> 0 Then
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
        'loading new file
        Dim openFileDialog As New OpenFileDialog
        Dim path As String = ""
        If openFileDialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            path = openFileDialog.FileName
            Dim extension = IO.Path.GetExtension(path)
            If extension <> ".RSS" Then
                MessageBox.Show("The file must be an RSS file.")
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
        logixObj = logixApp.FileOpen(path, False, False, True)
        If logixObj Is Nothing Then
            MessageBox.Show("ERROR: Failed to open the file.")
            Return
        End If

        'preparing database
        dataEntries.Clear()
        modbusDic.Clear()
        Dim programs = logixObj.ProgramFiles
        data_collection = logixObj.AddrSymRecords
        LoadData()
        LoadMapping(programs)

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
        If logixApp IsNot Nothing AndAlso dataEntries.Count <> 0 Then
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

        If dataEntries.ContainsKey(addr) Then
            'MessageBox.Show("Name: " + dataEntries(addr).Item1 + vbNewLine + "Description: " + dataEntries(addr).Item2)
            Label2.Text = dataEntries(addr).Item1
        Else
            MessageBox.Show("Address not found.")
        End If
        If modbusDic.ContainsKey(addr) Then
            Dim s As String = ""
            Dim l As List(Of String) = modbusDic(addr)
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
        For Each addr In modbusDic.Keys
            For Each des In modbusDic(addr)
                If reverse_mapping.ContainsKey(des) Then
                    reverse_mapping(des).Add(addr)
                    If reverse_mapping(des).Count > 1 Then
                        If Not invalid_mapping.ContainsKey(des) Then
                            invalid_mapping.add(des, "")
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
        'Update database if any change is made
        LoadData()
        Dim content = New List(Of String())
        For Each addr In dataEntries.Keys
            content.Add({addr, dataEntries(addr).Item1, dataEntries(addr).Item2})
        Next
        content.Sort(New DataEntryComparer())
        DisplayList(content, {"Address", "Name", "Description"})
    End Sub

    Private Sub perform_mapping_Click(sender As Object, e As EventArgs) Handles perform_mapping.Click
        Dim content = New List(Of String())
        For Each addr In modbusDic.Keys
            If dataEntries.ContainsKey(addr) Then 'if database has source addr
                Dim str As String = ""
                For Each s In modbusDic(addr) 'for each des addr
                    str = str + " " + s
                    Dim record = logixObj.AddrSymRecords.GetRecordViaAddrOrSym(s, 0)
                    ' If record IsNot Nothing Then
                    'MessageBox.Show(s & " " & record.Scope)
                    'End If
                    Dim symbol = dataEntries(addr).Item1 'got the des addr name by looking up src addr name
                    If symbol = "" Then
                        Exit For
                    End If
                    If record Is Nothing Then
                        record = logixObj.AddrSymRecords.add()
                        'MessageBox.Show("creating new addr: " + s)
                        record.SetAddress(s)
                        record.SetScope(0)
                    End If 'got the des addr instance
                    If symbol.Length + 1 >= logixApp.MaxSymbolLength - 1 Then
                        symbol = symbol.Substring(0, symbol.Length - 2) + "_"
                    Else
                        symbol += "_"
                    End If
                    'If addr = "I:4.4" Then
                    'MessageBox.Show(record.SetSymbol(symbol + "_"))
                    'End If
                    If symbol <> "_" And record.SetSymbol(symbol) = True Then
                        'MessageBox.Show("Unable to set name. Addr: " + s + " Source: " + addr + " " + symbol)
                    End If
                    If dataEntries(addr).Item2 <> "" Then
                        If record.SetDescription(dataEntries(addr).Item2) = True Then
                            'MessageBox.Show("Unable to set description. Addr: " + s + " Source: " + addr)
                        End If
                    End If
                Next
                content.Add({addr, str, dataEntries(addr).Item1, dataEntries(addr).Item2})
            End If
        Next
        content.Sort(New DataEntryComparer())
        DisplayList(content, {"Source", "Destination", "Name", "Description"})
    End Sub
End Class
