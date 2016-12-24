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
Imports System.IO

Namespace Core.Obfuscation.Protection
    Public NotInheritable Class Mild
        Inherits Source

#Region " Fields "
        Private Shared MdByString As New Dictionary(Of String, MethodDefinition)
        Private Shared MdByInteger As New Dictionary(Of Integer, MethodDefinition)
        Private Shared MdByDouble As New Dictionary(Of Double, MethodDefinition)
        Private Shared MdBySingle As New Dictionary(Of Single, MethodDefinition)
        Private Shared MdByByte As New Dictionary(Of Byte, MethodDefinition)
        Private Shared MdByRef As New Dictionary(Of MethodReference, MethodDefinition)
        Private Shared Types As New List(Of TypeDefinition)
#End Region

#Region " Methods "
        Friend Shared Function DoJob(asm As AssemblyDefinition, Framework$, Exclude As ExcludeList, Optional ByVal packIt As Boolean = False) As AssemblyDefinition
            AssemblyDef = asm
            Frmwk = Framework
            Pack = packIt

            For Each m As ModuleDefinition In asm.Modules
                Types.AddRange(m.GetAllTypes())
                For Each type As TypeDefinition In Types
                    If NameChecker.IsRenamable(type) Then
                        If Exclude.isHideCallsExclude(type) = False Then
                            IterateType(type)
                        End If
                    End If
                Next
                Types.Clear()
            Next

            MethodByClear()
            Return asm
        End Function

        Private Shared Sub MethodByClear()
            MdByString.Clear()
            MdByInteger.Clear()
            MdByDouble.Clear()
            MdBySingle.Clear()
            MdByByte.Clear()
            MdByRef.Clear()
        End Sub

        Private Shared Sub IterateType(ByVal td As TypeDefinition)
            Dim publicMethods As New List(Of MethodDefinition)()
            publicMethods.AddRange(From m In td.Methods Where (m.HasBody AndAlso m.Body.Instructions.Count > 2 AndAlso Not completedMethods.Contains(m) AndAlso Not m.DeclaringType.BaseType Is Nothing AndAlso Not m.DeclaringType.BaseType.Name = "ApplicationSettingsBase" AndAlso Not Finder.FindCustomAttributeByName(m.DeclaringType, "EditorBrowsableAttribute")))
            Try
                For Each md In publicMethods
                    If publicMethods.Contains(md) Then
                        If Utils.HasUnsafeInstructions(md) = False Then
                            Using optim As New Msil(md.Body)
                                For i = 0 To md.Body.Instructions.Count - 1
                                    Dim Instruction = md.Body.Instructions(i)

                                    If Not completedInstructions.Contains(Instruction) Then
                                        Dim mdFinal As MethodDefinition = Nothing
                                        Dim index% = md.Body.Instructions.IndexOf(Instruction)

                                        If (Instruction.OpCode = OpCodes.Ldc_I4) Then
                                            If isValidIntegerOperand(Instruction) AndAlso Not Randomizer.invisibleChars.Contains(CInt(Instruction.Operand)) Then
                                                If Not Instruction.Next Is Nothing AndAlso Not Instruction.Next.Operand Is Nothing Then
                                                    If Instruction.Next.Operand.ToString.EndsWith("System.Int32)") Then
                                                        If MdByInteger.ContainsKey(CInt(Instruction.Operand)) Then
                                                            mdFinal = MdByInteger.Item(CInt(Instruction.Operand))
                                                        Else
                                                            mdFinal = CreateMethod(Integer.Parse(Instruction.Operand.ToString), md)
                                                            MdByInteger.Add(CInt(Instruction.Operand), mdFinal)
                                                        End If
                                                        If (Not mdFinal Is Nothing) Then
                                                            md.Body.Instructions.Item(index).OpCode = OpCodes.Call
                                                            md.Body.Instructions.Item(index).Operand = AssemblyDef.MainModule.Import(mdFinal)

                                                            completedMethods.Add(mdFinal)
                                                            completedInstructions.Add(Instruction)
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        ElseIf Instruction.OpCode = OpCodes.Ldc_I4_0 OrElse Instruction.OpCode = OpCodes.Ldc_I4_1 OrElse Instruction.OpCode = OpCodes.Ldc_I4_2 _
                                             OrElse Instruction.OpCode = OpCodes.Ldc_I4_3 OrElse Instruction.OpCode = OpCodes.Ldc_I4_4 OrElse Instruction.OpCode = OpCodes.Ldc_I4_5 _
                                              OrElse Instruction.OpCode = OpCodes.Ldc_I4_6 OrElse Instruction.OpCode = OpCodes.Ldc_I4_7 OrElse Instruction.OpCode = OpCodes.Ldc_I4_8 Then
                                            Dim num = Integer.Parse(Instruction.OpCode.ToString().Split(".")(2))

                                            If MdByInteger.ContainsKey(num) Then
                                                mdFinal = MdByInteger.Item(num)
                                            Else
                                                mdFinal = CreateMethod(num, md)
                                                MdByInteger.Add(num, mdFinal)
                                            End If
                                            If (Not mdFinal Is Nothing) Then
                                                md.Body.Instructions.Item(index).OpCode = OpCodes.Call
                                                md.Body.Instructions.Item(index).Operand = AssemblyDef.MainModule.Import(mdFinal)

                                                completedMethods.Add(mdFinal)
                                                completedInstructions.Add(Instruction)
                                            End If
                                        ElseIf (Instruction.OpCode = OpCodes.Ldstr) Then
                                            If Not CStr(Instruction.Operand) = String.Empty Then
                                                If MdByString.ContainsKey(CStr(Instruction.Operand)) Then
                                                    mdFinal = MdByString.Item(CStr(Instruction.Operand))
                                                Else
                                                    mdFinal = CreateMethod(CStr(Instruction.Operand), md)
                                                    MdByString.Add(CStr(Instruction.Operand), mdFinal)
                                                End If
                                                If (Not mdFinal Is Nothing) Then
                                                    md.Body.Instructions.Item(index).OpCode = OpCodes.Call
                                                    md.Body.Instructions.Item(index).Operand = AssemblyDef.MainModule.Import(mdFinal)

                                                    completedMethods.Add(mdFinal)
                                                    completedInstructions.Add(Instruction)
                                                End If
                                            End If
                                        ElseIf Instruction.OpCode = OpCodes.Ldc_I4_S Then
                                            If MdByByte.ContainsKey(CByte(Instruction.Operand)) Then
                                                mdFinal = MdByByte.Item(CByte(Instruction.Operand))
                                            Else
                                                mdFinal = CreateMethod(Byte.Parse(Instruction.Operand.ToString), md)
                                                MdByByte.Add(CByte(Instruction.Operand), mdFinal)
                                            End If
                                            If (Not mdFinal Is Nothing) Then
                                                md.Body.Instructions.Item(index).OpCode = OpCodes.Call
                                                md.Body.Instructions.Item(index).Operand = AssemblyDef.MainModule.Import(mdFinal)

                                                completedMethods.Add(mdFinal)
                                                completedInstructions.Add(Instruction)
                                            End If
                                        ElseIf (Instruction.OpCode = OpCodes.Newobj) Then
                                            Dim mRef = DirectCast(Instruction.Operand, MethodReference)
                                            If Not mRef Is Nothing Then
                                                If MdByRef.ContainsKey(mRef) Then
                                                    mdFinal = MdByRef.Item(mRef)
                                                Else
                                                    mdFinal = CreateReferenceMethod(mRef, md)
                                                    MdByRef.Add(mRef, mdFinal)
                                                End If
                                                If (Not mdFinal Is Nothing) Then
                                                    md.Body.Instructions.Item(index).OpCode = OpCodes.Call
                                                    md.Body.Instructions.Item(index).Operand = AssemblyDef.MainModule.Import(mdFinal)

                                                    completedMethods.Add(mdFinal)
                                                    completedInstructions.Add(Instruction)
                                                End If
                                            End If
                                        ElseIf (Instruction.OpCode = OpCodes.Ldc_R4) Then
                                            If MdBySingle.ContainsKey(CSng(Instruction.Operand)) Then
                                                mdFinal = MdBySingle.Item(CSng(Instruction.Operand))
                                            Else
                                                mdFinal = CreateMethod(CSng(Instruction.Operand), md)
                                                MdBySingle.Add(CSng(Instruction.Operand), mdFinal)
                                            End If
                                            If (Not mdFinal Is Nothing) Then
                                                md.Body.Instructions.Item(index).OpCode = OpCodes.Call
                                                md.Body.Instructions.Item(index).Operand = AssemblyDef.MainModule.Import(mdFinal)

                                                completedMethods.Add(mdFinal)
                                                completedInstructions.Add(Instruction)
                                            End If
                                        ElseIf (Instruction.OpCode = OpCodes.Ldc_R8) Then
                                            If MdByDouble.ContainsKey(CDbl(Instruction.Operand)) Then
                                                mdFinal = MdByDouble.Item(CDbl(Instruction.Operand))
                                            Else
                                                mdFinal = CreateMethod(CDbl(Instruction.Operand), md)
                                                MdByDouble.Add(CDbl(Instruction.Operand), mdFinal)
                                            End If
                                            If (Not mdFinal Is Nothing) Then
                                                md.Body.Instructions.Item(index).OpCode = OpCodes.Call
                                                md.Body.Instructions.Item(index).Operand = AssemblyDef.MainModule.Import(mdFinal)

                                                completedMethods.Add(mdFinal)
                                                completedInstructions.Add(Instruction)
                                            End If
                                        End If
                                    End If
                                Next
                                optim.FixBranchOffsets()
                                optim.MethodBody.SimplifyMacros()
                            End Using
                        End If
                    End If
                Next
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            publicMethods.Clear()
        End Sub

        Private Shared Function CreateReferenceMethod(targetConstructor As MethodReference, md As MethodDefinition) As MethodDefinition
            If (targetConstructor.Parameters.Count <> 0) Then
                Return Nothing
            End If
            Dim item As New MethodDefinition(Randomizer.GenerateNew, (MethodAttributes.CompilerControlled Or (MethodAttributes.FamANDAssem Or (MethodAttributes.Family Or MethodAttributes.Static))), AssemblyDef.MainModule.Import(targetConstructor.DeclaringType))
            item.Body = New MethodBody(item)
            Dim ilProc As ILProcessor = item.Body.GetILProcessor()
            With ilProc
                .Body.MaxStackSize = 1
                .Body.InitLocals = True
                .Emit(OpCodes.Newobj, targetConstructor)
                .Emit(OpCodes.Ret)
            End With

            md.DeclaringType.Methods.Add(item)

            Return item
        End Function

        Private Shared Function CreateMethod(value As Object, md As MethodDefinition) As MethodDefinition
            Dim opc As OpCode = Nothing
            Select Case value.GetType
                Case GetType(String)
                    opc = OpCodes.Ldstr
                Case GetType(Integer)
                    opc = OpCodes.Ldc_I4
                Case GetType(Byte)
                    opc = OpCodes.Ldc_I4_S
                Case GetType(Single)
                    opc = OpCodes.Ldc_R4
                Case GetType(Double)
                    opc = OpCodes.Ldc_R8
            End Select
            Dim item As New MethodDefinition(Randomizer.GenerateNew, (MethodAttributes.CompilerControlled Or (MethodAttributes.FamANDAssem Or (MethodAttributes.Family Or MethodAttributes.Static))), AssemblyDef.MainModule.Import(value.GetType))
            item.Body = New MethodBody(item)
            Dim ilProc As ILProcessor = item.Body.GetILProcessor()
            With ilProc
                .Body.MaxStackSize = 1
                .Body.InitLocals = True
                .Emit(opc, value)
                .Emit(OpCodes.Ret)
            End With

            md.DeclaringType.Methods.Add(item)

            Return item
        End Function

#End Region

    End Class

End Namespace
