Imports Helper.RandomizeHelper
Imports Helper.AssemblyHelper

Namespace CryptoHelper
    Public Class Generator

        Public Shared numberPrime As Integer() = New Integer() {547, 557, 563, 569, 571, 577, 587, 593, 599, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659, _
                                             739, 743, 751, 757, 761, 769, 773, 787, 797, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997, _
                                             1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583, 2063, 2069, 2081, 2083, 2087, 2089, 2099, 2111, 2113, 2129, _
                                             3659, 3671, 3673, 3677, 3691, 3697, 3701, 3709, 3719, 3727, 4153, 4157, 4159, 4177, 4201, 4211, 4217, 4219, 4229, 4231, _
                                             5281, 5297, 5303, 5309, 5323, 5333, 5347, 5351, 5381, 5387, 6311, 6317, 6323, 6329, 6337, 6343, 6353, 6359, 6361, 6367, _
                                             9901, 9907, 9923, 9929, 9931, 9941, 9949, 9967, 9973}

        Public Shared numberUnPrime As Integer() = New Integer() {548, 549, 550, 570, 572, 578, 588, 594, 600, 608, 614, 618, 620, 632, 642, 644, 649, 655, 660, _
                                                          740, 744, 752, 758, 763, 770, 774, 789, 799, 812, 822, 826, 889, 830, 840, 855, 858, 861, 869, 939, 949, 948, 955, 970, 972, 979, 984, 992, 999, _
                                                          1525, 1535, 1547, 1550, 1554, 1561, 1569, 1573, 1585, 1584, 2065, 2070, 2082, 2085, 2090, 2091, 2092, 2112, 2116, 2130, _
                                                          3660, 3675, 3674, 3679, 3696, 3699, 3703, 3711, 3720, 3728, 4156, 4158, 4160, 4178, 4209, 4215, 4218, 4220, 4230, 4236, _
                                                          5282, 5298, 5306, 5326, 5335, 5348, 5354, 5386, 5388, 6313, 6319, 6325, 6330, 6339, 6345, 6351, 6360, 6365, 6363, _
                                                          9905, 9909, 9928, 9930, 9946, 9950, 9970, 9974}

        Public Shared Function IntEncrypt(num%, integ%) As String
            Dim ch As Char
            Dim str = String.Empty
            Dim num2 = New Random().Next(0, &H13)
            Dim random As New Random
            Dim i%
            For i = 0 To &H13 - 1
                If (i = num2) Then
                    ch = ChrW(integ)
                    str = (str & num.ToString & ch.ToString)
                Else
                    str = (str & random.Next(100).ToString & ChrW(integ).ToString)
                End If
            Next i
            ch = ChrW(integ)
            Return (str & num2.ToString & ch.ToString)
        End Function

        Public Shared Function GenerateDecryptIntFunc(_ClassIntName$, _DecryptIntFuncName$) As String
            Dim str = "Public Class " & _ClassIntName & vbNewLine _
                              & "    Public Shared Function " & _DecryptIntFuncName & " (ByVal d$, Byval integ%) As Integer" & vbNewLine _
                              & "        Dim s As String() = d.Split(New Char() {Strings.ChrW(integ)})" & vbNewLine _
                              & "        Dim n As Integer() = New Integer((s.Length - 1) - 1) {}" & vbNewLine _
                              & "        Dim i%" & vbNewLine _
                              & "        For i = 0 To n.Length - 1" & vbNewLine _
                              & "            n(i) = Integer.Parse(s(i))" & vbNewLine _
                              & "        Next i" & vbNewLine _
                              & "        Return n(n(n.Length - 1))" & vbNewLine _
                              & "    End Function" & vbNewLine _
                              & "End Class"
            Return str
        End Function

        Public Shared Function GenerateDecryptOddFunc(_ClassOddName$, _DecryptOddFuncName$) As String
            Dim str = "Public Class " & _ClassOddName & vbNewLine _
                              & "    Public Shared Function " & _DecryptOddFuncName & " (ByVal n%) As Boolean" & vbNewLine _
                              & "        Return n Mod 2 <> 0" & vbNewLine _
                              & "    End Function" & vbNewLine _
                              & "End Class"
            Return str
        End Function

        Public Shared Function GenerateDecryptXorFunc(_ClassXorName$, _DecryptXorFuncName$) As String
            Dim str = "Public Class " & _ClassXorName & vbNewLine _
                              & "    Public Shared Function " & _DecryptXorFuncName & " (ByVal t$, ByVal n%) As String" & vbNewLine _
                              & "        Dim s$ = String.Empty" & vbNewLine _
                              & "        Dim o% = (t.Length - 1)" & vbNewLine _
                              & "        Dim j% = 0" & vbNewLine _
                              & "        Do While (j <= o)" & vbNewLine _
                              & "            Dim p% = (Convert.ToInt32(t.Chars(j)) Xor n)" & vbNewLine _
                              & "            s = (s & Char.ConvertFromUtf32(p))" & vbNewLine _
                              & "            j += 1" & vbNewLine _
                              & "        Loop" & vbNewLine _
                              & "        Return s" & vbNewLine _
                              & "    End Function" & vbNewLine _
                              & "End Class"
            Return str
        End Function

        Public Shared Function GenerateCompressWithGzipByteFunc(_Decompress0$, Decompress1$) As String
            Return "    Public Shared Function " & _Decompress0 & "(ByVal d As Byte()) As Byte()" & vbNewLine _
                     & "        Try : Return " & Decompress1 & "(New GZipStream(New MemoryStream(d), CompressionMode.Decompress, False), d.Length)" & vbNewLine _
                     & "        Catch : Return Nothing : End Try" & vbNewLine _
                     & "    End Function" & vbNewLine _
                     & GenerateDeCompressWithGzipFunc(Decompress1)
        End Function

        Public Shared Function GenerateDeCompressWithGzipStreamFunc(_Decompress0$, Decompress1$) As String
            Return "    Private Shared Function " & _Decompress0 & "(ByVal d As Stream) As Byte()" & vbNewLine _
                     & "        Try : Return " & Decompress1 & "(New GZipStream(d, CompressionMode.Decompress, False), d.Length)" & vbNewLine _
                     & "        Catch : Return Nothing : End Try" & vbNewLine _
                     & "    End Function" & vbNewLine _
                     & GenerateDeCompressWithGzipFunc(Decompress1)
        End Function

        Private Shared Function GenerateDeCompressWithGzipFunc(Decompress1$) As String
            Return "    Public Shared Function " & Decompress1 & "(ByVal ds As Stream, ByVal c As Integer) As Byte()" & vbNewLine _
                     & "        Dim d() As Byte : Dim tb As Int32 = 0" & vbNewLine _
                     & "        Try : While True" & vbNewLine _
                     & "            ReDim Preserve d(tb + c)" & vbNewLine _
                     & "            Dim br As Int32 = ds.Read(d, tb, c)" & vbNewLine _
                     & "            If br = 0 Then Exit While" & vbNewLine _
                     & "                tb += br" & vbNewLine _
                     & "              End While" & vbNewLine _
                     & "            ReDim Preserve d(tb - 1)" & vbNewLine _
                     & "            Return d" & vbNewLine _
                     & "        Catch : Return Nothing : End Try" & vbNewLine _
                     & "    End Function" & vbNewLine
        End Function

        Public Shared Function GenereateDecryptPrimeFunc(_FunctionName$) As String
            Return "Public Shared Function " & _FunctionName & " (n as Integer) As Boolean" & vbNewLine _
                 & "        Dim b As Boolean = True" & vbNewLine _
                 & "        Dim f as integer = n / 2" & vbNewLine _
                 & "        Dim i as integer = 0" & vbNewLine _
                 & "        For i = 2 To f" & vbNewLine _
                 & "            If (n Mod i) = 0 Then" & vbNewLine _
                 & "                b = False" & vbNewLine _
                 & "            End If" & vbNewLine _
                 & "        Next" & vbNewLine _
                 & "        Return b" & vbNewLine _
                 & "     End Function"
        End Function

        Public Shared Function GenerateDecryptRPNFunc(_FunctionName0$, _FunctionName1$) As String
            Return "    Public Shared Function " & _FunctionName0 & " (ByVal operands As String()) As Integer" & vbNewLine _
                 & "        Dim sta As New Stack(Of Integer)" & vbNewLine _
                 & "        'Dim operands() As String = expression.ToLower().Split(New Char() {"" ""c}, StringSplitOptions.RemoveEmptyEntries)" & vbNewLine _
                 & "        For Each op As String In operands" & vbNewLine _
                 & "            Select Case op" & vbNewLine _
                 & "                Case ""+""" & vbNewLine _
                 & "                    sta.Push(sta.Pop() + sta.Pop())" & vbNewLine _
                 & "                Case ""-""" & vbNewLine _
                 & "                    sta.Push(-sta.Pop() + sta.Pop())" & vbNewLine _
                 & "                Case ""*""" & vbNewLine _
                 & "                    sta.Push(sta.Pop() * sta.Pop())" & vbNewLine _
                 & "                Case ""/""" & vbNewLine _
                 & "                    Dim tmp As Integer = sta.Pop()" & vbNewLine _
                 & "                    sta.Push(sta.Pop() / tmp)" & vbNewLine _
                 & "                Case ""sqrt""" & vbNewLine _
                 & "                    sta.Push(Math.Sqrt(sta.Pop()))" & vbNewLine _
                 & "                Case Else" & vbNewLine _
                 & "                    sta.Push(Integer.Parse(op))" & vbNewLine _
                 & "            End Select" & vbNewLine _
                 & "        Next" & vbNewLine _
                 & "        If sta.Count <> 1 Then Throw New ArgumentException(""Unbalanced expression"")" & vbNewLine _
                 & "        Return sta.Pop()" & vbNewLine _
                 & "    End Function" & vbNewLine _
                 & "    Public Shared Function " & _FunctionName1 & " (ByVal expression As String) As String()" & vbNewLine _
                 & "        Return expression.ToLower().Split(New Char() {"",""c}, StringSplitOptions.RemoveEmptyEntries)" & vbNewLine _
                 & "    End Function" & vbNewLine
        End Function

        Public Shared Function GenerateReadFromResourcesFunc(ClassName$, ReadFromResourcesFuncName$, ResName$) As String
            Return "Public Class " & ClassName & vbNewLine _
                          & "    Public Shared Function " & ReadFromResourcesFuncName & " (ByVal N As String) As String" & vbNewLine _
                          & "        Dim m As New ResourceManager(""" & ResName & """, GetType(System.Reflection.Assembly).GetMethod(""GetExecutingAssembly"").Invoke(Nothing, Nothing))" & vbNewLine _
                          & "        Dim s As String = DirectCast(m.GetObject(N), String)" & vbNewLine _
                          & "        m.ReleaseAllResources()" & vbNewLine _
                          & "        Return s" & vbNewLine _
                          & "    End Function" & vbNewLine _
                          & "End Class" & vbNewLine

        End Function

        Public Shared Function GenerateFromBase64Func(ClassName$, FromBase64FuncName$, GetStringFuncName$) As String
            Return "Public Class " & ClassName & vbNewLine _
                          & "    Public Shared Function " & FromBase64FuncName & " (ByVal bbbb As String) As Byte()" & vbNewLine _
                          & "        Return Convert.FromBase64String(bbbb)" & vbNewLine _
                          & "    End Function" & vbNewLine _
                          & "    Public Shared Function " & GetStringFuncName & " (ByVal aaaa As Byte()) As String" & vbNewLine _
                          & "        Return Encoding.Unicode.GetString(aaaa)" & vbNewLine _
                          & "    End Function" & vbNewLine _
                          & "End Class" & vbNewLine
        End Function

    End Class


End Namespace
