Imports System.IO

Public Module IOHandler
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
            content.Add({addr, db.GetTagName(addr), db.GetDescription(addr)})
        Next
        content.Sort(New DataEntryComparer())
        Return content
    End Function

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
End Module
