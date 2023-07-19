Imports System.Text.RegularExpressionsVisualBasic

Partial Class Form1

    Function ExtractAddresses(rung As String) As List(Of Tuple(Of String, String))
        ' Define the regular expression pattern for addr format
        Dim addr As String = "([A-Z]{1,3}\d{1,3}:\d{1,3}(?:\/\d{1,2})*|(?:I|O):\d{1,3}\.\d{1,3})"
        Dim regex As New Regex("MOV " & addr & " " & addr)
        Dim matches As MatchCollection = regex.Matches(rung)
        Dim results As New List(Of Tuple(Of String, String))
        For Each match In matches
            'MessageBox.Show(match.Groups(0).Value)
            results.Add(New Tuple(Of String, String)(match.Groups(1).Value, match.Groups(2).Value))
        Next

        Return results
    End Function

    Private coil_start = False
    'Returns a list of tuple

    Private Function ExtractMapping(str As String) As List(Of Tuple(Of String, String))
        Dim words As String() = str.Split(" "c)
        Dim results As New List(Of Tuple(Of String, String))
        For i As Integer = 0 To words.Length - 3
            If coil_start = False AndAlso words(i) = "MOV" Then
                Dim source As String = words(i + 1).Replace(".ACC", "")
                If dataEntries.ContainsKey(source) Then
                    results.Add(New Tuple(Of String, String)(source, words(i + 2)))
                End If
                If dataEntries.ContainsKey(words(i + 2)) AndAlso dataEntries(words(i + 2)).Item1 = "COIL_START" Then
                    coil_start = True
                End If
            End If
            'If words(i) = "T23:2/EN" Then
            '    MessageBox.Show(str)
            'End If
            If coil_start AndAlso words(i) = "OR" Then
                ' MessageBox.Show(str)
                Dim ans As List(Of Node) = Parser.Parse(New LinkedList(Of String)(words))
                Dim out As String = ""
                For j = 0 To ans.Count - 1
                    out &= ans(j).ToString()
                    If ans(j).Ins = "BST" Then
                        FindCoilMapping(ans(j), results)
                        Exit For
                        'MessageBox.Show(str & Environment.NewLine & results.Count)
                    End If
                Next
                Return results
                'MessageBox.Show("Parser result:" + Environment.NewLine + out)
                'MessageBox.Show(str)
            ElseIf words(i) = "SWP" Then
                Dim out As String = ""
                Dim temp = ExtractAddresses(str)
                Dim src As String = ""
                Dim des As String = ""
                For Each pair As Tuple(Of String, String) In temp
                    If dataEntries.ContainsKey(pair.Item2) AndAlso dataEntries(pair.Item2).Item1 = "WORD_TO_REVERSE" Then
                        src = pair.Item1
                    ElseIf dataEntries.ContainsKey(pair.Item1) AndAlso dataEntries(pair.Item1).Item1 = "REVERSED_WORD" Then
                        des = pair.Item2
                    End If
                Next
                If src <> des Then
                    For count As Integer = 0 To 15
                        Dim full_addr1 = src & "/" & count.ToString
                        Dim full_addr2 = des & "/" & count.ToString
                        'MessageBox.Show(full_addr1 & " " & full_addr2)
                        If dataEntries.ContainsKey(full_addr1) = False Then
                            Continue For
                        End If
                        results.Add(New Tuple(Of String, String)(full_addr1, full_addr2))
                    Next
                End If
                Return results
            End If
        Next
        Return results
    End Function

    Public Sub FindCoilMapping(bst As Node, results As List(Of Tuple(Of String, String)))
        Dim count = 0
        For Each branch In bst.Children
            If branch.Last.Ins = "OR" Then
                If branch.First.Children.Count = 0 Then
                    'MessageBox.Show("Source: " & child.First.Args(0) & " DES: " & child.Last.Args(0) & "/" & count)
                    If dataEntries(branch.First.Args(0)).Item1 <> "ALWAYS_OFF" Then
                        results.Add(New Tuple(Of String, String)(branch.First.Args(0), branch.Last.Args(0) & "/" & count))
                    End If
                Else
                    'MessageBox.Show("Source: " & child.First.Children(0)(0).Args(0) & " DES: " & child.Last.Args(0) & "/" & count)
                    If dataEntries(branch.First.Children(0)(0).Args(0)).Item1 <> "ALWAYS_OFF" Then
                        results.Add(New Tuple(Of String, String)(branch.First.Children(0)(0).Args(0), branch.Last.Args(0) & "/" & count))
                        'MessageBox.Show("This coil address is mapped to multiple blocks: " & branch.Last.Args(0) & "/" & count & Environment.NewLine & branch.First.ToString())
                    End If
                End If
                count += 1
            End If

        Next
    End Sub
End Class
