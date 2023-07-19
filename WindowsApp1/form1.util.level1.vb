Imports System.IO

Partial Class Form1

    Private Sub LoadData(records As Object)
        Dim numOfRec = records.Count
        Dim Data As Object
        For i As Integer = 0 To numOfRec - 1
            Data = records.GetRecordViaIndex(i)
            Dim str = Data.Description
            'If Data.Address = "N17:21" Then
            'MessageBox.Show(Data.Symbol)
            'End If
            str = str.Replace(Environment.NewLine, " ")
            If Data.Address IsNot Nothing Then
                If dataEntries.ContainsKey(Data.Address) Then
                    If dataEntries(Data.Address).Item1 = "" Then
                        'MessageBox.Show("repeat data: " + Data.Address + "New:" + Data.Symbol + "Old:" + dataEntries(Data.Address).Item1)
                        dataEntries(Data.Address) = New Tuple(Of String, String)(Data.Symbol, str)
                    End If
                Else
                    dataEntries.Add(Data.Address, New Tuple(Of String, String)(Data.Symbol, str))
                End If
            End If
        Next
    End Sub

    Private Sub LoadMapping(programs As Object)
        Dim numOfProg = programs.Count()
        Dim ladder As Object
        For i As Integer = 0 To 0
            ladder = programs.Item(49)
            'If ladder IsNot Nothing Then
            '    MessageBox.Show(i.ToString() + ladder.Name)
            'End If
            Dim numOfRung = ladder.NumberOfRungs
            For j As Integer = 0 To numOfRung - 1
                Dim r = ladder.GetRung(j)
                'modbusDic
                Dim mapping = ExtractMapping(ladder.GetRungAsAscii(j))
                For Each pair In mapping
                    'MessageBox.Show(pair.Item1 + pair.Item2)
                    'If modbusDic.ContainsKey(pair.Item1) Then
                    ' MessageBox.Show(pair.Item1 + modbusDic(pair.Item1)(0) + " " + pair.Item2)
                    'End If
                    If Not modbusDic.ContainsKey(pair.Item1) Then
                        modbusDic.Add(pair.Item1, New List(Of String))
                    End If

                    modbusDic(pair.Item1).Add(pair.Item2)
                Next

                'MessageBox.Show(ladder.GetRungAsAscii(j))
            Next
        Next

    End Sub

    Private Function WriteToCSV(data As List(Of String()), path As String) As Boolean
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
