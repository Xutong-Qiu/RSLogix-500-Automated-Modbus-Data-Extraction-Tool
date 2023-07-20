Imports System.Text.RegularExpressions

Public Class DataEntryComparer
    Implements IComparer(Of String())

    Public Function Compare(x() As String, y() As String) As Integer Implements IComparer(Of String()).Compare
        Dim com = New AddrComparer
        Return com.Compare(x(0), y(0))
    End Function
End Class
Public Class AddrComparer
    Implements IComparer(Of String)

    Public Function Compare(x As String, y As String) As Integer Implements IComparer(Of String).Compare
        Dim addr As String = "^([A-Z]){1,3}(\d{1,3}):(\d{1,3})(?:\/(\d{1,2}))*|(I|O):(\d{1,3}\.\d{1,3})(.)$"
        Dim regex As New Regex(addr)
        Dim matchx As Match = regex.Match(x)
        Dim matchy As Match = regex.Match(y)
        If matchx.Success = False Or matchy.Success = False Then
            Return x.CompareTo(y)
        End If
        Dim xletter = If(matchx.Groups(1).ToString <> "", matchx.Groups(1).ToString, matchx.Groups(5).ToString)
        Dim yletter = If(matchy.Groups(1).ToString <> "", matchy.Groups(1).ToString, matchy.Groups(5).ToString)
        If xletter <> yletter Then
            Return xletter.CompareTo(yletter)
        End If
        Dim xaddr1 = If(matchx.Groups(2).ToString <> "", matchx.Groups(2).ToString, matchx.Groups(6).ToString)
        Dim yaddr1 = If(matchy.Groups(2).ToString <> "", matchy.Groups(2).ToString, matchy.Groups(6).ToString)
        If xaddr1 <> yaddr1 Then
            Return CSng(xaddr1).CompareTo(CSng(yaddr1))
        End If
        Dim xaddr2 = If(matchx.Groups(3).ToString <> "", matchx.Groups(3).ToString, matchx.Groups(6).ToString)
        Dim yaddr2 = If(matchy.Groups(3).ToString <> "", matchy.Groups(3).ToString, matchy.Groups(6).ToString)

        ' MessageBox.Show(xaddr2 + " " + yaddr2)
        If xaddr2 <> yaddr2 Then
            Return CSng(xaddr2).CompareTo(CSng(yaddr2))
        End If
        Dim xaddr3 = If(matchx.Groups(4).ToString <> "", matchx.Groups(4).ToString, matchx.Groups(7).ToString)
        Dim yaddr3 = If(matchy.Groups(4).ToString <> "", matchy.Groups(4).ToString, matchy.Groups(7).ToString)
        If xaddr3 <> yaddr3 Then
            If xaddr3 = "" Then
                Return -1
            End If
            If yaddr3 = "" Then
                Return 1
            End If
            Return CSng(xaddr3).CompareTo(CSng(yaddr3))
        End If
        If matchx.Groups(8).Value.CompareTo(matchy.Groups(8).Value) = 0 Then
            'MessageBox.Show(x & " " & y & " " & matchx.Groups(8).Value.CompareTo(matchy.Groups(8).Value))
        End If
        Return matchx.Groups(8).Value.CompareTo(matchy.Groups(8).Value)
    End Function

End Class

