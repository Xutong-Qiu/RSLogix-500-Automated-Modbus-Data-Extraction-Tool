Imports System.Runtime.ConstrainedExecution
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView
''' <summary>
''' This module carries out operations related to addresses.
''' </summary>
Public Module AddressSolver

    ''' <summary>
    ''' This function gets rid of any prefix and suffix of an address. For example,
    ''' "#N64:0.CVP"->"N64:0"
    ''' </summary>
    ''' <param name="addr">The address of the entry</param>
    ''' <returns>A string after tuning. If it's not a valid address, returns an empty string.</returns>
    Public Function Tune(addr As String) As String
        Dim regex As New Regex("(?:([A-Z]{1,3})(\d{1,3}):(\d{1,3})|(?:(I|O|S|U):(\d{1,3}(?:\.\d{1,3})*)))(?:\/(\d{1,2}))*")
        Dim match As Match = regex.Match(addr)
        If match.Success = True Then
            Return match.Groups(0).ToString
        End If
        Return ""
    End Function
    ''' <summary>
    ''' This function carries out addition on the last group of numbers of an address.
    ''' For example, AddrAdder("N64:34",1) = N64:35
    ''' </summary>
    ''' <param name="addr">The address of the entry</param>
    ''' <returns>A string after adding. If it's not a valid address, returns an empty string.</returns>
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

    Public Function GetExtension(addr As String) As String
        Dim regex As New Regex("(?:([A-Z]{1,3})(\d{1,3}):(\d{1,3})|(?:(I|O|S|U):(\d{1,3}(?:\.\d{1,3})*)))(?:\/(\d{1,2}))*((?:\.|\/)[A-Z]{1,3})*")
        Dim match As Match = regex.Match(addr)
        If match.Success = True Then
            Return match.Groups(7).ToString
        End If
        Return ""
    End Function

End Module


