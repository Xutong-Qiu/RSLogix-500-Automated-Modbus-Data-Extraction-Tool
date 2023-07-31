Imports Microsoft.VisualBasic.Logging
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView

''' <summary>
''' RegLogicAnalyzer
''' This module analyzes the logic of the given parse tree and finds register mapping
''' within it. It is based on logic patterns that have been seen in the PLC programs.
''' </summary>
Public Module RegLogicAnalyzer
    Public Sub FindRegMapping(root As Node, results As List(Of Tuple(Of String, String)))
        Dim cur As Node = root
        If cur.Ins = "XIO" Then
            cur = cur.NextIns
            If cur.Ins = "BST" Then
                For Each branch In cur.Children
                    If RegPattern1(branch, results) Then
                        Continue For
                    End If
                    If RegPattern2(branch, results) Then
                        Continue For
                    End If
                    If RegPattern3(branch, results) Then
                        Continue For
                    End If
                    If RegPattern4(branch, results) Then
                        Continue For
                    End If
                    If RegPattern5(branch, results) Then
                        Continue For
                    End If
                    MessageBox.Show("Register logic not found: " & branch.ToString)
                Next
            End If
        End If
    End Sub

    'MOV case
    Private Function RegPattern1(root As Node, results As List(Of Tuple(Of String, String))) As Boolean
        Dim cur As Node = root
        If cur IsNot Nothing AndAlso cur.Ins = "MOV" Then
            results.Add(New Tuple(Of String, String)(Tune(cur.Args(0)), Tune(cur.Args(1))))
            Return True
        End If
        Return False
    End Function

    'EQU MOV case
    Private Function RegPattern2(root As Node, results As List(Of Tuple(Of String, String))) As Boolean
        Dim cur As Node = root
        If cur IsNot Nothing AndAlso cur.Ins = "EQU" Then
            cur = cur.NextIns
            If cur IsNot Nothing AndAlso cur.Ins = "MOV" Then
                results.Add(New Tuple(Of String, String)(Tune(cur.Args(0)), Tune(cur.Args(1))))
                Return True
            End If
        End If
        Return False
    End Function

    'EQU BST MOV MOV case
    Private Function RegPattern3(root As Node, results As List(Of Tuple(Of String, String))) As Boolean
        Dim cur As Node = root
        If cur IsNot Nothing AndAlso cur.Ins = "EQU" Then
            cur = cur.NextIns
            If cur IsNot Nothing AndAlso cur.Ins = "BST" AndAlso cur.Children.Count = 2 Then
                Dim branch1 As Node = cur.Children(0)
                Dim branch2 As Node = cur.Children(1)
                If RegPattern1(branch1, results) Then
                    If branch2 IsNot Nothing And branch1.Ins = "MOV" Then
                        Return RegPattern1(branch2, results)
                    End If
                End If
            End If
        End If
        Return False
    End Function

    'BST MOV EQU MOV case
    Private Function RegPattern4(root As Node, results As List(Of Tuple(Of String, String))) As Boolean
        Dim cur As Node = root
        If cur IsNot Nothing AndAlso cur.Ins = "BST" AndAlso cur.Children.Count = 2 Then
            Dim branch1 As Node = cur.Children(0)
            Dim branch2 As Node = cur.Children(1)
            If branch1 IsNot Nothing AndAlso branch1.Ins = "MOV" Then
                If RegPattern1(branch1, results) Then
                    If branch2 IsNot Nothing AndAlso branch2.Ins = "EQU" Then
                        Return RegPattern2(branch2, results)
                    End If
                End If
            End If
        End If
        Return False
    End Function

    'CPW case
    Private Function RegPattern5(root As Node, results As List(Of Tuple(Of String, String))) As Boolean
        Dim cur As Node = root
        If cur IsNot Nothing AndAlso cur.Ins = "CPW" Then
            Dim src As String = Tune(cur.Args(0))
            Dim des As String = Tune(cur.Args(1))
            Dim offset As Integer = CSng(cur.Args(2)) - 1
            results.Add(New Tuple(Of String, String)(src, des))
            results.Add(New Tuple(Of String, String)(src, AddrAdder(des, offset)))
            Return True
        End If
        Return False
    End Function
End Module