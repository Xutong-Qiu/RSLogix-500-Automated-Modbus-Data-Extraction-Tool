Imports System.Drawing.Drawing2D
Imports System.Text.RegularExpressions
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
            If coil_start AndAlso words(i) = "OR" Then
                ' MessageBox.Show(str)
                Dim ans As Node = Parser.Parse(New LinkedList(Of String)(words))
                Dim out As String = ""
                Dim cur As Node = ans
                While cur IsNot Nothing
                    out &= cur.ToString()
                    If cur.Ins = "BST" Then
                        findcoilmapping(cur, results)
                    End If
                    cur = cur.NextIns
                End While
                'For j = 0 To ans.Count - 1
                '    out &= ans(j).ToString()
                '    If ans(j).Ins = "BST" Then
                '        FindCoilMapping(ans(j), results)
                '        Exit For
                '        'MessageBox.Show(str & Environment.NewLine & results.Count)
                '    End If
                'Next
                'MessageBox.Show(out)
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

    Private Sub findcoilmapping(bst As Node, results As List(Of Tuple(Of String, String)))
        Dim count = 0
        For Each branch In bst.Children
            Dim cur = branch
            Dim rungSize = 1
            While cur.NextIns IsNot Nothing
                rungSize += 1
                cur = cur.NextIns
            End While
            If cur.Ins = "OR" Then
                'MessageBox.Show(branch.Ins)
                If rungSize = 2 AndAlso branch.Ins <> "BST" Then
                    Dim addr As String = "([A-Z]{1,3}\d{1,3}:\d{1,3}(?:\/\d{1,2})*|(?:I|O):\d{1,3}\.\d{1,3})"
                    Dim regex As New Regex(addr)
                    Dim match As Match = regex.Match(branch.Args(0))
                    If match.Success = True Then
                        addr = match.Groups(0).ToString
                    End If
                    'MessageBox.Show(addr)
                    If dataEntries.ContainsKey(addr) AndAlso dataEntries(addr).Item1 <> "ALWAYS_OFF" Then
                        results.Add(New Tuple(Of String, String)(addr, cur.Args(0) & "/" & count))
                    End If
                ElseIf rungSize >= 2 Then
                    Dim temp = branch
                    Dim related = New List(Of Node)
                    While temp IsNot cur
                        related.Add(temp)
                        temp = temp.NextIns
                    End While
                    Dim des = cur.Args(0) & "/" & count
                    results.Add(New Tuple(Of String, String)(makeupmapping(related, des), des))
                    'MessageBox.Show("This coil depends on multiple addresses: " & branch.ToString)
                End If
                count += 1
            End If
        Next
    End Sub

    Private Function makeupmapping(related As List(Of Node), des As String) As String
        Dim addrs As List(Of String) = New List(Of String)
        Dim s = ""
        Dim stored = des
        If related.First.Ins = "BST" Then
            dataEntries(des) = dataEntries(related.First.Children.First.Args(0))
        Else
            dataEntries(des) = dataEntries(related.First.Args(0))
        End If
        Return stored
    End Function
End Class
