Imports Mono.Cecil
Imports Mono.Cecil.Rocks
Imports Mono.Cecil.Cil
Imports Helper.RandomizeHelper
Imports System.Runtime.CompilerServices
Imports Helper.CecilHelper
Imports Helper.AssemblyHelper
Imports Helper.CodeDomHelper
Imports System.Text.RegularExpressions
Imports Helper.CryptoHelper
Imports System.Resources
Imports Implementer.Core.Obfuscation.Builder
Imports Implementer.Core.Obfuscation.Exclusion

Namespace Core.Obfuscation.Protection
    Public NotInheritable Class Numeric
        Inherits Source

#Region " Fields "
        Private Shared DecryptReadResources As Stub
        Private Shared DecryptInt As Stub
        Private Shared DecryptRPN As Stub
        Private Shared MethodByInteger As New Dictionary(Of Integer, MethodDefinition)
        Private Shared MethodByIntegerS As New Dictionary(Of Integer, MethodDefinition)
        Private Shared MethodByInteger2 As New Dictionary(Of Integer, MethodDefinition)
        Private Shared Types As New List(Of TypeDefinition)()
#End Region

#Region " Methods "
        Friend Shared Function DoJob(ByVal asm As AssemblyDefinition, Framework$, encryptToRes As EncryptType, Exclude As ExcludeList, Optional ByVal packIt As Boolean = False) As AssemblyDefinition
            AssemblyDef = asm
            Frmwk = Framework
            Pack = packIt
            EncryptToResources = encryptToRes

            If encryptToRes = EncryptType.ToResources Then
                ResName = Randomizer.GenerateNew
                ResWriter = New ResourceWriter(ResName & ".resources")
                DecryptReadResources = New Stub(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic)
                With DecryptReadResources
                    .ResolveTypeFromFile(ReadFromResourcesStub(.className, .funcName1), Finder.FindDefaultNamespace(asm, Pack), Randomizer.GenerateNew, Randomizer.GenerateNew)
                    .InjectType(asm)
                    completedMethods.Add(.GetMethod1)
                    completedMethods.Add(.GetMethod2)
                    completedMethods.Add(.GetMethod3)
                End With
            End If

            DecryptInt = New Stub(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic)
            With DecryptInt
                .ResolveTypeFromFile(DecryptIntStub(.className, .funcName1), Finder.FindDefaultNamespace(asm, Pack), Randomizer.GenerateNew, Randomizer.GenerateNew)
                .InjectType(asm)
                completedMethods.Add(.GetMethod1)
            End With

            DecryptRPN = New Stub(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic)
            With DecryptRPN
                .ResolveTypeFromFile(DecryptRPNStub(.className, .funcName1, .funcName2), Finder.FindDefaultNamespace(asm, Pack), Randomizer.GenerateNew, Randomizer.GenerateNew, Randomizer.GenerateNew)
                .InjectType(asm)
                completedMethods.Add(.GetMethod1)
                completedMethods.Add(.GetMethod2)
            End With

            For Each m As ModuleDefinition In asm.Modules
                Types.AddRange(m.GetAllTypes())
                For Each type As TypeDefinition In Types
                    If NameChecker.IsRenamable(type) Then
                        If Exclude.isIntegerEncodExclude(type) = False Then
                            IterateType(type)
                        End If
                    End If
                Next
                Types.Clear()
            Next

            If encryptToRes = EncryptType.ToResources Then
                If Not ResWriter Is Nothing Then ResWriter.Close()
                InjectResource()
            End If

            MethodByClear()
            DeleteStubs()
            CleanUp()

            Return asm
        End Function

        Private Shared Sub IterateType(ByVal td As TypeDefinition)
            Dim publicMethods As New List(Of MethodDefinition)()
            publicMethods.AddRange(From m In td.Methods Where (m.HasBody AndAlso m.Body.Instructions.Count > 2 AndAlso Not completedMethods.Contains(m) AndAlso Not Finder.FindCustomAttributeByName(m.DeclaringType, "EditorBrowsableAttribute")))
            Try
                For Each md In publicMethods
                    If publicMethods.Contains(md) Then
                        'If Utils.HasUnsafeInstructions(md) = False Then
                        Using optim As New Msil(md.Body)
                            For i = 0 To md.Body.Instructions.Count - 1
                                Dim Instruct = md.Body.Instructions(i)

                                If Not completedInstructions.Contains(Instruct) Then
                                    Dim mdFinal As MethodDefinition = Nothing

                                    Dim index As Integer = md.Body.Instructions.IndexOf(Instruct)
                                    If ((Instruct.OpCode = OpCodes.Ldc_I4) OrElse (Instruct.OpCode = OpCodes.Ldc_I4_S)) Then
                                        If isValidOperand(Instruct) AndAlso CInt(Instruct.Operand) > 1 Then
                                            If Instruct.OpCode = OpCodes.Ldc_I4 Then
                                                If Not MethodByInteger.TryGetValue(CInt(Instruct.Operand), mdFinal) Then
                                                    If Randomizer.GenerateBoolean Then

                                                        mdFinal = New MethodDefinition(Randomizer.GenerateNew, (MethodAttributes.CompilerControlled Or (MethodAttributes.FamANDAssem Or (MethodAttributes.Family Or MethodAttributes.Static))), md.DeclaringType.Module.Import(GetType(Integer)))
                                                        mdFinal.Body = New MethodBody(mdFinal)

                                                        If EncryptToResources = EncryptType.ToResources Then
                                                            Dim integ = Randomizer.GenerateInvisible

                                                            Dim encStr = Generator.IntEncrypt(CInt(Instruct.Operand), integ)
                                                            Dim dataKeyName = Randomizer.GenerateNew
                                                            ResWriter.AddResource(dataKeyName, encStr)

                                                            Dim ilProc = mdFinal.Body.GetILProcessor()
                                                            With ilProc
                                                                .Body.MaxStackSize = 2
                                                                .Body.InitLocals = True
                                                                mdFinal.Body.Variables.Add(New VariableDefinition(AssemblyDef.MainModule.Import(GetType(Integer))))
                                                                .Emit(OpCodes.Ldstr, dataKeyName)
                                                                .Emit(OpCodes.Call, md.Module.Import(DecryptReadResources.GetMethod1))
                                                                .Emit(OpCodes.Ldc_I4, integ)
                                                                .Emit(OpCodes.Call, AssemblyDef.MainModule.Import(DecryptInt.GetMethod1))
                                                                .Emit(OpCodes.Stloc_0)
                                                                .Emit(OpCodes.Ldloc_0)
                                                                .Emit(OpCodes.Ret)
                                                            End With

                                                            md.DeclaringType.Methods.Add(mdFinal)
                                                            MethodByInteger.Add(CInt(Instruct.Operand), mdFinal)
                                                        Else
                                                            Dim integ = Randomizer.GenerateInvisible
                                                            Dim encStr = Generator.IntEncrypt(CInt(Instruct.Operand), integ)

                                                            Dim ilProc = mdFinal.Body.GetILProcessor()
                                                            With ilProc
                                                                .Body.MaxStackSize = 2
                                                                .Body.InitLocals = True
                                                                mdFinal.Body.Variables.Add(New VariableDefinition(AssemblyDef.MainModule.Import(GetType(Integer))))
                                                                .Emit(OpCodes.Ldstr, encStr)
                                                                .Emit(OpCodes.Ldc_I4, integ)
                                                                .Emit(OpCodes.Call, AssemblyDef.MainModule.Import(DecryptInt.GetMethod1))
                                                                .Emit(OpCodes.Stloc_0)
                                                                .Emit(OpCodes.Ldloc_0)
                                                                .Emit(OpCodes.Ret)
                                                            End With

                                                            md.DeclaringType.Methods.Add(mdFinal)
                                                            MethodByInteger.Add(CInt(Instruct.Operand), mdFinal)
                                                        End If
                                                    Else
                                                        Dim resultPrimes = GetPrimes(CInt(Instruct.Operand))
                                                        Dim countPrimes = resultPrimes.Count
                                                        If countPrimes > 2 Then

                                                            Dim num = CInt(Instruct.Operand)
                                                            Dim divider0 = 0
                                                            Dim resultdivider0 = DetermineDiv(num, divider0)
                                                            Dim StrDivider0 = resultdivider0 & " / " & divider0
                                                            Dim divider1 = 0
                                                            Dim resultdivider1 = DetermineDiv(num, divider1)
                                                            Dim StrDivider1 = resultdivider1 & " / " & divider1

                                                            Dim StrDivider = StrDivider0 & " - " & StrDivider1 & " + "

                                                            Dim strPrimes = String.Empty
                                                            strPrimes = String.Join(" ", resultPrimes).TrimEnd(" ")
                                                            For k% = 0 To countPrimes - 2
                                                                strPrimes &= " *"
                                                            Next

                                                            Dim InFix = (StrDivider & strPrimes).TrimEnd(" ")

                                                            Dim postfix = String.Empty
                                                            Dim bResult = InfixToPostfixConvert(InFix, postfix)

                                                            postfix = postfix.TrimEnd(" ").Replace(" ", ",")

                                                            mdFinal = New MethodDefinition(Randomizer.GenerateNew, (MethodAttributes.CompilerControlled Or (MethodAttributes.FamANDAssem Or (MethodAttributes.Family Or MethodAttributes.Static))), AssemblyDef.MainModule.Import(GetType(Integer)))
                                                            mdFinal.Body = New MethodBody(mdFinal)

                                                            Dim ilProc = mdFinal.Body.GetILProcessor()
                                                            With ilProc
                                                                .Body.MaxStackSize = 2
                                                                .Body.InitLocals = True
                                                                mdFinal.Body.Variables.Add(New VariableDefinition(AssemblyDef.MainModule.Import(GetType(Integer))))
                                                                .Emit(OpCodes.Ldstr, postfix)
                                                                .Emit(OpCodes.Call, AssemblyDef.MainModule.Import(DecryptRPN.GetMethod2))
                                                                .Emit(OpCodes.Call, AssemblyDef.MainModule.Import(DecryptRPN.GetMethod1))
                                                                .Emit(OpCodes.Stloc_0)
                                                                .Emit(OpCodes.Ldloc_0)
                                                                .Emit(OpCodes.Ret)
                                                            End With

                                                            md.DeclaringType.Methods.Add(mdFinal)
                                                            MethodByInteger.Add(CInt(Instruct.Operand), mdFinal)
                                                        Else
                                                            Dim divider0 = 0
                                                            Dim resultdivider0 = DetermineDiv(CInt(Instruct.Operand), divider0)
                                                            Dim str = resultdivider0 & "," & divider0 & ",/"

                                                            mdFinal = New MethodDefinition(Randomizer.GenerateNew, (MethodAttributes.CompilerControlled Or (MethodAttributes.FamANDAssem Or (MethodAttributes.Family Or MethodAttributes.Static))), AssemblyDef.MainModule.Import(GetType(Integer)))
                                                            mdFinal.Body = New MethodBody(mdFinal)

                                                            Dim ilProc = mdFinal.Body.GetILProcessor()
                                                            With ilProc
                                                                .Body.MaxStackSize = 2
                                                                .Body.InitLocals = True
                                                                mdFinal.Body.Variables.Add(New VariableDefinition(AssemblyDef.MainModule.Import(GetType(Integer))))
                                                                .Emit(OpCodes.Ldstr, str)
                                                                .Emit(OpCodes.Call, AssemblyDef.MainModule.Import(DecryptRPN.GetMethod2))
                                                                .Emit(OpCodes.Call, AssemblyDef.MainModule.Import(DecryptRPN.GetMethod1))
                                                                .Emit(OpCodes.Stloc_0)
                                                                .Emit(OpCodes.Ldloc_0)
                                                                .Emit(OpCodes.Ret)
                                                            End With

                                                            md.DeclaringType.Methods.Add(mdFinal)
                                                            MethodByInteger.Add(CInt(Instruct.Operand), mdFinal)
                                                        End If
                                                    End If
                                                Else
                                                    mdFinal = MethodByInteger.Item(CInt(Instruct.Operand))
                                                End If
                                            ElseIf Instruct.OpCode = OpCodes.Ldc_I4_S Then
                                                Dim num4 As Double = Math.Log10(Convert.ToDouble(Instruct.Operand))

                                                Dim methodName = Randomizer.GenerateNew
                                                mdFinal = New MethodDefinition(methodName, MethodAttributes.[Static] Or MethodAttributes.[Public] Or MethodAttributes.HideBySig, AssemblyDef.MainModule.Import(GetType(Integer)))
                                                mdFinal.Body = New MethodBody(mdFinal)

                                                Dim ilProc = mdFinal.Body.GetILProcessor()
                                                With ilProc
                                                    .Body.MaxStackSize = 8
                                                    .Body.InitLocals = True
                                                    mdFinal.Body.Variables.Add(New VariableDefinition(AssemblyDef.MainModule.Import(GetType(Integer))))
                                                    .Emit(OpCodes.Ldc_R8, CDbl(10))
                                                    .Emit(OpCodes.Ldc_R8, num4)
                                                    .Emit(OpCodes.Call, AssemblyDef.MainModule.Import(GetType(Math).GetMethod("Pow", New Type() {GetType(Double), GetType(Double)})))
                                                    .Emit(OpCodes.Call, AssemblyDef.MainModule.Import(GetType(Convert).GetMethod("ToInt32", New Type() {GetType(Double)})))
                                                    .Emit(OpCodes.Stloc_0)
                                                    .Emit(OpCodes.Ldloc_0)
                                                    .Emit(OpCodes.Ret)
                                                End With

                                                md.DeclaringType.Methods.Add(mdFinal)
                                            End If
                                            If (Not mdFinal Is Nothing) Then
                                                If mdFinal.DeclaringType.IsNotPublic Then
                                                    mdFinal.DeclaringType.IsPublic = True
                                                End If
                                                md.Body.Instructions.Item(index).OpCode = OpCodes.Call
                                                md.Body.Instructions.Item(index).Operand = AssemblyDef.MainModule.Import(mdFinal)
                                                completedMethods.Add(mdFinal)
                                                completedInstructions.Add(Instruct)
                                            End If
                                        End If
                                    ElseIf (Instruct.OpCode = OpCodes.Ldc_R4) Then
                                        If isValidOperand(Instruct) AndAlso CSng(Instruct.Operand) >= 0 Then
                                            Dim integ As Single
                                            If Single.TryParse(Instruct.Operand, integ) Then
                                                Dim pdefName = Randomizer.GenerateNew

                                                Dim pdef As New PropertyDefinition(pdefName, PropertyAttributes.None, AssemblyDef.MainModule.Import(GetType(Single)))
                                                md.DeclaringType.Properties.Add(pdef)

                                                mdFinal = New MethodDefinition(("get_" & pdef.Name), MethodAttributes.Static Or MethodAttributes.Public, pdef.PropertyType)
                                                mdFinal.Body = New MethodBody(mdFinal)

                                                pdef.GetMethod = mdFinal
                                                pdef.DeclaringType.Methods.Add(mdFinal)

                                                If Not pdef.DeclaringType.IsInterface Then
                                                    Dim iLProcessor = mdFinal.Body.GetILProcessor
                                                    With iLProcessor
                                                        .Body.MaxStackSize = 1
                                                        .Body.InitLocals = True
                                                        mdFinal.Body.Variables.Add(New VariableDefinition(AssemblyDef.MainModule.Import(GetType(Single))))
                                                        .Emit(OpCodes.Ldc_R4, integ)
                                                        .Emit(OpCodes.Ret)
                                                    End With
                                                Else
                                                    mdFinal.IsAbstract = True
                                                    mdFinal.IsVirtual = True
                                                    mdFinal.IsNewSlot = True
                                                End If
                                                mdFinal.IsSpecialName = True
                                                mdFinal.IsGetter = True
                                            End If
                                            If (Not mdFinal Is Nothing) Then
                                                If mdFinal.DeclaringType.IsNotPublic Then
                                                    mdFinal.DeclaringType.IsPublic = True
                                                End If
                                                md.Body.Instructions.Item(index).OpCode = OpCodes.Call
                                                md.Body.Instructions.Item(index).Operand = AssemblyDef.MainModule.Import(mdFinal)
                                                completedMethods.Add(mdFinal)
                                                completedInstructions.Add(Instruct)
                                            End If
                                        End If
                                    ElseIf (Instruct.OpCode = OpCodes.Ldc_R8) Then
                                        If isValidOperand(Instruct) AndAlso CDbl(Instruct.Operand) >= 0 Then

                                            Dim integ As Double
                                            If Double.TryParse(Instruct.Operand, integ) Then

                                                Dim pdefName = Randomizer.GenerateNew

                                                Dim pdef As New PropertyDefinition(pdefName, PropertyAttributes.None, AssemblyDef.MainModule.Import(GetType(Double)))
                                                md.DeclaringType.Properties.Add(pdef)

                                                mdFinal = New MethodDefinition(("get_" & pdef.Name), MethodAttributes.Static Or MethodAttributes.Public, pdef.PropertyType)
                                                mdFinal.Body = New MethodBody(mdFinal)

                                                pdef.GetMethod = mdFinal
                                                pdef.DeclaringType.Methods.Add(mdFinal)

                                                If Not pdef.DeclaringType.IsInterface Then
                                                    Dim iLProcessor = mdFinal.Body.GetILProcessor
                                                    With iLProcessor
                                                        .Body.MaxStackSize = 1
                                                        .Body.InitLocals = True
                                                        mdFinal.Body.Variables.Add(New VariableDefinition(AssemblyDef.MainModule.Import(GetType(Double))))
                                                        .Emit(OpCodes.Ldc_R8, integ)
                                                        .Emit(OpCodes.Ret)
                                                    End With
                                                Else
                                                    mdFinal.IsAbstract = True
                                                    mdFinal.IsVirtual = True
                                                    mdFinal.IsNewSlot = True
                                                End If
                                                mdFinal.IsSpecialName = True
                                                mdFinal.IsGetter = True
                                            End If

                                            If (Not mdFinal Is Nothing) Then
                                                If mdFinal.DeclaringType.IsNotPublic Then
                                                    mdFinal.DeclaringType.IsPublic = True
                                                End If
                                                md.Body.Instructions.Item(index).OpCode = OpCodes.Call
                                                md.Body.Instructions.Item(index).Operand = AssemblyDef.MainModule.Import(mdFinal)
                                                completedMethods.Add(mdFinal)
                                                completedInstructions.Add(Instruct)
                                            End If
                                        End If
                                    End If
                                End If
                            Next
                            optim.FixBranchOffsets()
                            optim.MethodBody.SimplifyMacros()
                        End Using
                    End If
                Next
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            publicMethods.Clear()
        End Sub

        Private Shared Function GetPrimes(n As Decimal) As List(Of Integer)
            Dim storage As New List(Of Integer)()
            While n > 1
                Dim i% = 1
                While True
                    If IsPrime(i) Then
                        If (CDec(n) / i) = Math.Round(CDec(n) / i) Then
                            n /= i
                            storage.Add(i)
                            Exit While
                        End If
                    End If
                    i += 1
                End While
            End While
            Return storage
        End Function

        Private Shared Function IsPrime(n As Integer) As Boolean
            If n <= 1 Then
                Return False
            End If
            For i% = 2 To Math.Sqrt(n)
                If n Mod i = 0 Then
                    Return False
                End If
            Next
            Return True
        End Function

        Private Shared Function DetermineDiv(ByVal real As Integer, ByRef div As Integer) As Integer
            Dim num% = rand.Next(5, 40)
            div = num
            Dim v% = real
            Try
                v = (real * num)
            Catch ex As System.OverflowException
                div = 1
            End Try

            Return v
        End Function

        Private Shared Function InfixToPostfixConvert(ByRef infixBuffer As String, ByRef postfixBuffer As String) As Boolean
            Dim prior% = 0
            postfixBuffer = ""

            Dim s1 As New Stack(Of Char)

            For i% = 0 To infixBuffer.Length - 1
                Dim item As Char = infixBuffer.Chars(i)
                Select Case item
                    Case "+"c, "-"c, "*"c, "/"c
                        If (s1.Count <= 0) Then
                            s1.Push(item)
                        Else
                            If ((s1.Peek = "*"c) OrElse (s1.Peek = "/"c)) Then
                                prior = 1
                            Else
                                prior = 0
                            End If
                            If (prior = 1) Then
                                Select Case item
                                    Case "+"c, "-"c
                                        postfixBuffer = (postfixBuffer & CStr(s1.Pop))
                                        i -= 1
                                        Continue For
                                End Select
                                postfixBuffer = (postfixBuffer & CStr(s1.Pop))
                                i -= 1
                            Else
                                Select Case item
                                    Case "+"c, "-"c
                                        postfixBuffer = (postfixBuffer & CStr(s1.Pop))
                                        s1.Push(item)
                                        Continue For
                                End Select
                                s1.Push(item)
                            End If
                        End If
                        Exit Select
                    Case Else
                        postfixBuffer = (postfixBuffer & CStr(item))
                        Exit Select
                End Select
            Next

            Dim len% = s1.Count
            For j% = 0 To len - 1
                postfixBuffer = (postfixBuffer & CStr(s1.Pop))
            Next

            postfixBuffer = postfixBuffer.Replace("/", " / ").Replace("*", " * ").Replace("+", " + ").Replace("-", " - ")
            postfixBuffer = New Regex("[ ]{2,}", RegexOptions.None).Replace(postfixBuffer, " ")
            Return True
        End Function

        Private Shared Sub DeleteStubs()
            If Not DecryptReadResources Is Nothing Then DecryptReadResources.DeleteDll()
            If Not DecryptInt Is Nothing Then DecryptInt.DeleteDll()
            If Not DecryptRPN Is Nothing Then DecryptRPN.DeleteDll()
        End Sub

        Private Shared Sub MethodByClear()
            MethodByInteger.Clear()
            MethodByIntegerS.Clear()
            MethodByInteger2.Clear()
        End Sub
#End Region

    End Class

End Namespace
