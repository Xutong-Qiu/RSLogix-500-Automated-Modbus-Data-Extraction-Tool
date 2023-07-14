Imports System.Text.RegularExpressions

Public Class CustomComparer
    Implements IComparer(Of String)

    Public Function Compare(x As String, y As String) As Integer Implements IComparer(Of String).Compare
        Dim str = x + y
        Dim addr As String = "^([A-Z]\d{1,3}:\d{1,3})([A-Z]\d{1,3}:\d{1,3})$"

        Dim regex As New Regex(addr)
        Dim match As Match = regex.Match(str)
        Dim x1 As String = (x.Substring((1)))
        Dim y1 As String = (y.Substring((1)))
        ' Split the remaining string by ':'
        Dim xnumbers As String() = x1.Split(":")
        Dim ynumbers As String() = y1.Split(":")
        If match.Success AndAlso xnumbers.Length = 2 AndAlso ynumbers.Length = 2 AndAlso xnumbers(0) <> "" AndAlso xnumbers(1) <> "" AndAlso ynumbers(0) <> "" AndAlso ynumbers(1) <> "" Then
            ' Parse the split strings into integers
            ' MessageBox.Show(ynumbers(1))
            Dim xfirstNumber As Integer = Integer.Parse(xnumbers(0))
            Dim xsecondNumber As Integer = Integer.Parse(xnumbers(1))
            Dim yfirstNumber As Integer = Integer.Parse(ynumbers(0))
            Dim ysecondNumber As Integer = Integer.Parse(ynumbers(1))
            If xfirstNumber <> yfirstNumber Then
                Return xfirstNumber.CompareTo(yfirstNumber)
            Else
                Return xsecondNumber.CompareTo(ysecondNumber)
            End If
        End If
        Return x.CompareTo(y)
    End Function

End Class

