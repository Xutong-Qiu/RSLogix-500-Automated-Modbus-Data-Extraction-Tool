Module Parser

    ''' <summary>
    ''' This function parses the logic of the given token list
    ''' </summary>
    ''' <param name="tokens">The token list as a linked list of string</param>
    ''' <returns>A node that represents the logic.</returns>
    Public Function Parse(tokens As LinkedList(Of String)) As Node
        If tokens.First.Value = "SOR" Then
            tokens.RemoveFirst()
        End If
        Return ParseIns(tokens)
    End Function

    Private Function ParseIns(Tokens As LinkedList(Of String)) As Node
        Dim ans As Node = Nothing
        If Tokens.Count = 0 Then
            Return Nothing
        End If
        Dim token = Tokens.First.Value
        Select Case token
            Case "EQU"
                ans = ParseEQU(Tokens)
            Case "GEQ"
                ans = ParseGEQ(Tokens)
            Case "EOU"
                ans = ParseEOU(Tokens)
            Case "NEQ"
                ans = ParseNEQ(Tokens)
            Case "MOV"
                ans = ParseMOV(Tokens)
            Case "BST"
                ans = ParseBST(Tokens)
            Case "XIC"
                ans = ParseXIC(Tokens)
            Case "XIO"
                ans = ParseXIO(Tokens)
            Case "OR"
                ans = ParseOR(Tokens)
            Case "JSR"
                ans = ParseJSR(Tokens)
            Case "SWP"
                ans = ParseSWP(Tokens)
            Case "CPW"
                ans = ParseCPW(Tokens)
            Case Else
                MessageBox.Show("invalid instruction: " + token)
        End Select
        If Tokens.First.Value <> "EOR" AndAlso Tokens.First.Value <> "NXB" AndAlso Tokens.First.Value <> "BND" Then
            ans.NextIns = ParseIns(Tokens)
        End If
        Return ans
    End Function
    Private Function ParseCPW(Tokens As LinkedList(Of String)) As Node
        Dim ans As New Node("CPW")
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        Return ans
    End Function
    Private Function ParseSWP(Tokens As LinkedList(Of String)) As Node
        Dim ans As New Node("SWP")
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        Return ans
    End Function

    Private Function ParseOR(Tokens As LinkedList(Of String)) As Node
        Dim ans As New Node("OR")
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        Return ans
    End Function
    Private Function ParseEQU(Tokens As LinkedList(Of String)) As Node
        Dim ans As New Node("EQU")
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        Return ans
    End Function
    Private Function ParseGEQ(Tokens As LinkedList(Of String)) As Node
        Dim ans As New Node("GEQ")
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        Return ans
    End Function
    Private Function ParseEOU(Tokens As LinkedList(Of String)) As Node
        Dim ans As New Node("EOU")
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        Return ans
    End Function
    Private Function ParseNEQ(Tokens As LinkedList(Of String)) As Node
        Dim ans As New Node("NEQ")
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        Return ans
    End Function
    Private Function ParseMOV(Tokens As LinkedList(Of String)) As Node
        Dim ans As New Node("MOV")
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        Return ans
    End Function
    Private Function ParseXIC(Tokens As LinkedList(Of String)) As Node
        Dim ans As New Node("XIC")
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        Return ans
    End Function
    Private Function ParseJSR(Tokens As LinkedList(Of String)) As Node
        Dim ans As New Node("JSR")
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        Return ans
    End Function
    Private Function ParseXIO(Tokens As LinkedList(Of String)) As Node
        Dim ans As New Node("XIO")
        Tokens.RemoveFirst()
        ans.Args.Add(Tokens.First.Value)
        Tokens.RemoveFirst()
        Return ans
    End Function
    Private Function ParseBST(Tokens As LinkedList(Of String)) As Node
        Tokens.RemoveFirst()
        Dim ans As New Node("BST")
        While Tokens.First.Value <> "BND"
            ans.Children.Add(ParseIns(Tokens))
            If Tokens.Count = 0 Then
                MessageBox.Show("empty token!" + ans.ToString)
            End If
            If Tokens.First.Value = "NXB" Then
                Tokens.RemoveFirst()
            End If
        End While
        Tokens.RemoveFirst()
        Return ans
    End Function

End Module

Public Class Node
    Public Property Ins As String
    Public Property Children As List(Of Node)
    Public Property NextIns As Node
    Public Property Args As List(Of String)
    Public Sub New(Str As String)
        Ins = Str
        Children = New List(Of Node)
        Args = New List(Of String)
        NextIns = Nothing
    End Sub
    Public Overrides Function ToString() As String
        Dim s As String = ""
        If Ins = "BST" Then
            s = Environment.NewLine
            s &= "BST{" + Environment.NewLine
            For Each child In Children
                Dim temp As String
                temp = child.ToString()
                'temp = temp.Replace(Environment.NewLine, Environment.NewLine & "    ")  'add indent
                s &= "  " + temp
                s &= Environment.NewLine
            Next
            s &= "}"
        Else
            s &= Ins + "("
            For Each arg In Args
                s &= arg & ", "
            Next
            s = s.Substring(0, s.Length - 2) & ")" & " "
        End If

        If NextIns IsNot Nothing Then
            s &= NextIns.ToString
        End If
        Return s
    End Function
End Class


