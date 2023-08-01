Imports System.Text.RegularExpressions


''' <summary>
''' This class defines data entry comparers that are used to compare and sort data entries displayed
''' on the datagrid. It does so by calling the address comparer method and pass x(0), y(0) as its
''' parameter. This is because in a data entry, the first column is address. So a data entry comparer 
''' is just using address comparer to compare the addresses.
''' </summary>
Public Class DataEntryComparer
    Implements IComparer(Of String())

    Public Function Compare(x() As String, y() As String) As Integer Implements IComparer(Of String()).Compare
        Dim com = New AddrComparer
        Return com.Compare(x(0), y(0))
    End Function
End Class


Public Class DataGridEntryComparer
    Implements IComparer

    Private descending As Boolean

    Public Sub New(descending As Boolean)
        Me.descending = descending
    End Sub

    Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
        Dim dataGridViewRowX As DataGridViewRow = TryCast(x, DataGridViewRow)
        Dim dataGridViewRowY As DataGridViewRow = TryCast(y, DataGridViewRow)
        If dataGridViewRowX IsNot Nothing AndAlso dataGridViewRowY IsNot Nothing Then
            Dim cellValueX As Object = dataGridViewRowX.Cells(0).Value
            Dim cellValueY As Object = dataGridViewRowY.Cells(0).Value

            If cellValueX Is Nothing AndAlso cellValueY Is Nothing Then
                Return 0
            ElseIf cellValueX Is Nothing Then
                Return -1
            ElseIf cellValueY Is Nothing Then
                Return 1
            End If
            ' In this example, we assume the cell values are integers.
            Dim addrx As String = cellValueX.ToString
            Dim addry As String = cellValueY.ToString
            Dim com = New AddrComparer
            If descending Then
                Return com.Compare(addrx, addry)
            Else
                Return com.Compare(addry, addrx)
            End If

        End If
        Return 0
    End Function
End Class

''' <summary>
''' This class defines address comparers that are used to compare and sort the addresses. It 
''' uses regex to parse the letter and integers in the given address string and compare them.
''' </summary>
Public Class AddrComparer
    Implements IComparer(Of String)

    Public Function Compare(x As String, y As String) As Integer Implements IComparer(Of String).Compare
        Dim addr As String = "^(?:([A-Z]{1,3})(\d{1,3}):(\d{1,3})|(?:(I|O|S|U):(\d{1,3}(?:\.\d{1,3})*)))(?:\/(\d{1,2}))*(.*)$"
        Dim regex As New Regex(addr)
        Dim matchx As Match = regex.Match(x)
        Dim matchy As Match = regex.Match(y)
        If matchx.Success = False Or matchy.Success = False Then
            Return x.CompareTo(y)
        End If
        Dim xletter = If(matchx.Groups(1).ToString <> "", matchx.Groups(1).ToString, matchx.Groups(4).ToString)
        Dim yletter = If(matchy.Groups(1).ToString <> "", matchy.Groups(1).ToString, matchy.Groups(4).ToString)
        If xletter <> yletter Then
            Return xletter.CompareTo(yletter)
        End If
        Dim xaddr1 = If(matchx.Groups(2).ToString <> "", matchx.Groups(2).ToString, matchx.Groups(5).ToString)
        Dim yaddr1 = If(matchy.Groups(2).ToString <> "", matchy.Groups(2).ToString, matchy.Groups(5).ToString)
        If xaddr1 <> yaddr1 Then
            Return CSng(xaddr1).CompareTo(CSng(yaddr1))
        End If
        Dim xaddr2 = If(matchx.Groups(3).ToString <> "", matchx.Groups(3).ToString, matchx.Groups(5).ToString)
        Dim yaddr2 = If(matchy.Groups(3).ToString <> "", matchy.Groups(3).ToString, matchy.Groups(5).ToString)

        ' MessageBox.Show(xaddr2 + " " + yaddr2)
        If xaddr2 <> yaddr2 Then
            Return CSng(xaddr2).CompareTo(CSng(yaddr2))
        End If
        Dim xaddr3 = matchx.Groups(6).ToString
        Dim yaddr3 = matchy.Groups(6).ToString
        If xaddr3 <> yaddr3 Then
            If xaddr3 = "" Then
                Return -1
            End If
            If yaddr3 = "" Then
                Return 1
            End If
            Return CSng(xaddr3).CompareTo(CSng(yaddr3))
        End If
        Return matchx.Groups(7).ToString.CompareTo(matchy.Groups(7).ToString)
    End Function
End Class

