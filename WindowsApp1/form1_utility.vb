Imports System.IO
Imports System.Text.RegularExpressions

Partial Class Form1
    Private Function Compare(x As String, y As String) As Integer Implements IComparer(Of String).Compare
        Dim xSplit = x.Skip(1).ToString().Split(":"c)
        Dim ySplit = y.Skip(1).ToString().Split(":"c)

        ' Extract the numbers from the inputs.
        Dim xFirst = Integer.Parse(xSplit(0))
        Dim xSecond = Integer.Parse(xSplit(1))
        Dim yFirst = Integer.Parse(ySplit(0))
        Dim ySecond = Integer.Parse(ySplit(1))

        ' If the first parts aren't equal, compare based on them.
        If xFirst <> yFirst Then
            Return xFirst.CompareTo(yFirst)
        End If

        ' If the first parts are equal, compare based on the second parts.
        Return xSecond.CompareTo(ySecond)
    End Function

    Function ExtractAddresses(instruction As String) As Dictionary(Of String, String)
        ' Define the regular expression pattern for addr format
        Dim addr As String = "([A-Z]\d{0,2}:\d{1,2})"

        Dim regex As New Regex("MOV " & addr & " " & addr)
        Dim match As Match = regex.Match(instruction)

        Dim result As New Dictionary(Of String, String)
        If match.Success Then
            result("addr1") = match.Groups(1).Value
            result("addr2") = match.Groups(2).Value
        End If

        Return result
    End Function


    Private Function ExtractMapping(str As String) As List(Of Tuple(Of String, String))
        Dim words As String() = str.Split(" "c)
        Dim results As New List(Of Tuple(Of String, String))()

        For i As Integer = 0 To words.Length - 3
            If words(i) = "MOV" Then
                Dim source As String = words(i + 1).Replace(".ACC", "")
                results.Add(New Tuple(Of String, String)(source, words(i + 2)))
            End If
            If words(i) = "OR" Then
                'MessageBox.Show(str)
            End If
        Next
        Return results
    End Function

    Private Sub LoadData(records As Object)
        Dim numOfRec = records.Count
        Dim Data As Object
        For i As Integer = 0 To numOfRec - 1
            Data = records.GetRecordViaIndex(i)
            Dim str = Data.Description
            str = str.Replace(Environment.NewLine, " ")
            If Data.Address IsNot Nothing Then
                If dataEntries.ContainsKey(Data.Address) Then
                    dataEntries(Data.Address) = New Tuple(Of String, String)(Data.Symbol, str)
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
