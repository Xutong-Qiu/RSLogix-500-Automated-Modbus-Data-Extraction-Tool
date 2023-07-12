Public Class Form1
    Private Function ExtractMapping(str As String) As List(Of Tuple(Of String, String))
        Dim words As String() = str.Split(" "c)
        Dim results As New List(Of Tuple(Of String, String))()

        For i As Integer = 0 To words.Length - 3
            If words(i) = "MOV" Then
                Dim source As String = words(i + 1).Replace(".ACC", "")
                results.Add(New Tuple(Of String, String)(source, words(i + 2)))
            End If
        Next

        Return results
    End Function

    Private Sub LoadData(records As Object)
        Dim numOfRec = records.Count
        Dim Data As Object
        For i As Integer = 0 To numOfRec - 1
            Data = records.GetRecordViaIndex(i)
            dataEntries.Add(Data.Address, New Tuple(Of String, String)(Data.Symbol, Data.Description))
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


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If dataEntries.Count <> 0 Then
            Dim result As DialogResult = MessageBox.Show("Are you sure to load a new file?", "A File Has Been Loaded", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                dataEntries.Clear()
                modbusDic.Clear()
            ElseIf result = DialogResult.No Then
                Return
            End If
        End If
        Dim logixApp As Object = CreateObject("RSLogix500.Application")
        If logixApp Is Nothing Then 'Error checking, if gApplication is not set then display a message
            MessageBox.Show("ERROR: Failed to open RSLogix500 software.",
                            "ERROR: 001",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation)
            Return
        End If
        Dim logixObj = logixApp.FileOpen(“C:\Users\37239\OneDrive - Entegris\Documents\RSLogix500\Project\SW2031_W.RSS”, True, False, True)
        If logixObj Is Nothing Then
            MessageBox.Show("ERROR: Failed to open the file.")
            Return
        End If

        'MessageBox.Show(logix_obj.ImportDataBase("C:\Users\37239\OneDrive - Entegris\Desktop\my_data.csv", True))
        Dim data_collection = logixObj.DataFiles
        Dim programs = logixObj.ProgramFiles
        data_collection = logixObj.AddrSymRecords
        LoadData(data_collection)
        LoadMapping(programs)
        'logixObj.DisplayReportOptions
        logixObj.save(True, True)
        logixObj.close(True, True)
        logixObj = Nothing
        logixObj = Nothing
        logixApp.Quit(True, False)
        logixApp = Nothing
        'For Each p As Process In Process.GetProcessesByName("Rs500")
        '    'MessageBox.Show(p.ProcessName)
        '    If p.ProcessName = "Rs500" Then
        '        p.Kill()
        '        p.WaitForExit()
        '    End If
        'Next
        MessageBox.Show("PLC program successfully loaded.")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim addr As String = TextBox1.Text
        addr = addr.Trim()
        If dataEntries.ContainsKey(addr) Then
            'MessageBox.Show("Name: " + dataEntries(addr).Item1 + vbNewLine + "Description: " + dataEntries(addr).Item2)
            Label2.Text = dataEntries(addr).Item1
            Label4.Text = dataEntries(addr).Item2
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

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

    End Sub
    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click

    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

End Class
