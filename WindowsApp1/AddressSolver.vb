Imports System.Runtime.ConstrainedExecution
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView

Public Module AddressSolver
    Public Function Tune(addr As String) As String
        Dim regex As New Regex("(?:([A-Z]{1,3})(\d{1,3}):(\d{1,3})|(?:(I|O|S|U):(\d{1,3}(?:\.\d{1,3})*)))(?:\/(\d{1,2}))*")
        Dim match As Match = regex.Match(addr)
        If match.Success = True Then
            Return match.Groups(0).ToString
        End If
        Return ""
    End Function

    Public Function AddrAdder(addr As String, offset As Integer) As String
        Dim regex As New Regex("(?:([A-Z]{1,3})(\d{1,3}):(\d{1,3})|(?:(I|O|S|U):(\d{1,3}(?:\.\d{1,3})*)))(?:\/(\d{1,2}))*")
        Dim match As Match = regex.Match(addr)
        Dim value As Integer

        If match.Success = True Then
            value = CSng(match.Groups(3).Value)
            value += offset
            Dim index As Integer = match.Groups(3).Index
            Return addr.Substring(0, index) & value.ToString
        End If
        Return ""
    End Function

End Module


