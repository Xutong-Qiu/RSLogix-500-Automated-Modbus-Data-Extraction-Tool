Imports System.Security.Cryptography
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView
''' <summary>
''' CoilLogicAnalyzer
''' This module analyzes the logic of the given parse tree and finds coil mapping
''' within it. It is based on logic patterns that have been seen in the PLC programs.
''' </summary>
Public Module CoilLogicAnalyzer
    Public Sub FindCoilMapping(root As Node, results As List(Of Tuple(Of String, String)))
        Dim cur As Node = root
        Dim bit As Integer = 0
        If cur IsNot Nothing AndAlso cur.Ins = "EQU" Then
            cur = cur.NextIns
            If cur IsNot Nothing AndAlso cur.Ins = "XIO" Then
                cur = cur.NextIns
                If cur.Ins = "BST" Then
                    If CoilPatternSWP(cur, results) Then
                        Return
                    End If
                    For Each branch In cur.Children
                        If CoilPatternIgnored(branch, results, bit) Then
                            Continue For
                        End If
                        If CoilPattern1(branch, results, bit) Then
                            bit += 1
                            Continue For
                        End If
                        If CoilPattern2(branch, results, bit) Then
                            bit += 1
                            Continue For
                        End If
                        If CoilPattern3(branch, results, bit) Then
                            bit += 1
                            Continue For
                        End If
                        MessageBox.Show("Coil logic not found: " & branch.ToString)
                    Next
                End If
            End If
        End If
    End Sub

    'MOV 0 to coil
    Private Function CoilPatternIgnored(root As Node, results As List(Of Tuple(Of String, String)), bit As Integer) As Boolean
        Dim cur As Node = root
        If cur IsNot Nothing AndAlso cur.Ins = "MOV" Then
            Return True
        End If
        Return False
    End Function

    'direct mapping case: XIC->OR
    Private Function CoilPattern1(root As Node, results As List(Of Tuple(Of String, String)), bit As Integer) As Boolean
        Dim cur As Node = root
        If cur IsNot Nothing AndAlso cur.Ins = "XIC" Then
            Dim src As String = Tune(cur.Args(0))
            cur = cur.NextIns
            If cur IsNot Nothing AndAlso cur.Ins = "OR" Then
                results.Add(New Tuple(Of String, String)(src, Tune(cur.Args(0) & "/" & bit)))
                Return True
            End If
        End If
        Return False
    End Function

    'EQU->OR
    Private Function CoilPattern2(root As Node, results As List(Of Tuple(Of String, String)), bit As Integer) As Boolean
        Dim cur As Node = root
        If cur IsNot Nothing AndAlso cur.Ins = "EQU" Then
            Dim src As String = Tune(cur.Args(1))
            cur = cur.NextIns
            If cur IsNot Nothing AndAlso cur.Ins = "OR" Then
                results.Add(New Tuple(Of String, String)(src, Tune(cur.Args(0) & "/" & bit)))
                Return True
            End If
        End If
        Return False
    End Function

    'NEQ->XIC->OR
    Private Function CoilPattern3(root As Node, results As List(Of Tuple(Of String, String)), bit As Integer) As Boolean
        Dim cur As Node = root
        If cur IsNot Nothing AndAlso cur.Ins = "NEQ" Then
            cur = cur.NextIns
            If cur IsNot Nothing Then
                Return CoilPattern1(cur, results, bit)
            End If
        End If
        Return False
    End Function

    'MOV/JSR/MOV/SWP
    Private Function CoilPatternSWP(root As Node, results As List(Of Tuple(Of String, String))) As Boolean
        Dim cur As Node = root
        If cur IsNot Nothing AndAlso cur.Ins = "BST" Then
            If cur.Children.Count = 4 Then
                If cur.Children(0).Ins = "MOV" AndAlso cur.Children(1).Ins = "JSR" AndAlso cur.Children(2).Ins = "MOV" AndAlso cur.Children(3).Ins = "SWP" Then
                    Dim src As String = cur.Children(0).Args(0)
                    Dim des As String = cur.Children(2).Args(1)
                    If src <> des Then
                        For count As Integer = 0 To 15
                            Dim full_addr1 = src & "/" & count.ToString
                            Dim full_addr2 = des & "/" & count.ToString
                            results.Add(New Tuple(Of String, String)(full_addr1, full_addr2))
                        Next
                    End If
                    Return True
                End If
            End If
        End If
        Return False
    End Function
End Module
