Imports System.Text
Imports System.IO
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Resources
Imports Mono.Cecil
Imports Mono.Cecil.Cil
Imports Mono.Cecil.Rocks
Imports Helper.CecilHelper
Imports Helper.CodeDomHelper
Imports Helper.AssemblyHelper
Imports Helper.RandomizeHelper
Imports Implementer.Engine.Processing
Imports Implementer.Core.Obfuscation
Imports Implementer.Core.Obfuscation.Builder
Imports Implementer.Core.Obfuscation.Exclusion

Namespace Core.Obfuscation.Protection
    Public NotInheritable Class Pinvoke
        Inherits Source

#Region " Fields "
        Private Shared LoaderInvoke As Stub
        Private Shared PinvokeCreate As PinvokeModifier
#End Region

#Region " Constructor "
        Shared Sub New()
            PinvokeCreate = New PinvokeModifier
        End Sub
#End Region

#Region " Methods "

        Friend Shared Sub DoJob(ByVal asm As AssemblyDefinition, Framework$, Exclude As ExcludeList, Optional ByVal packIt As Boolean = False)
            AssemblyDef = asm
            Frmwk = Framework
            Pack = packIt

            Dim Types As New List(Of TypeDefinition)()
            Dim HasPinvokeCalls As Boolean
            For Each mo As ModuleDefinition In asm.Modules
                For Each t In mo.GetAllTypes()
                    Types.Add(t)
                    For Each m In t.Methods
                        If m.IsPInvokeImpl Then
                            HasPinvokeCalls = True
                            Exit For
                        End If
                    Next
                Next
            Next

            If HasPinvokeCalls Then
                LoaderInvoke = New Stub(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, _
                              Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic)
                With LoaderInvoke
                    .ResolveTypeFromFile(DynamicInvokeStub(.className, .funcName1, .funcName2, .funcName3), Finder.FindDefaultNamespace(asm, Pack), Randomizer.GenerateNew, Randomizer.GenerateNew, Randomizer.GenerateNew, Randomizer.GenerateNew)
                    .InjectType(asm)
                    completedMethods.Add(.GetMethod1)
                    completedMethods.Add(.GetMethod2)
                    completedMethods.Add(.GetMethod3)
                End With

                PinvokeCreate.AddModuleRef(asm.MainModule)

                For Each type As TypeDefinition In Types
                    If NameChecker.IsRenamable(type) Then
                        If Exclude.isHideCallsExclude(type) = False Then
                            IterateType(type)
                        End If
                    End If
                Next

                LoaderInvoke.DeleteDll()
                PinvokeCreate.Dispose()
            End If

            Types.Clear()
        End Sub

        Private Shared Sub IterateType(ByVal td As TypeDefinition)
            Dim publicMethods As New List(Of MethodDefinition)()
            publicMethods.AddRange(From m In td.Methods Where (m.HasBody AndAlso m.Body.Instructions.Count > 1 AndAlso Not completedMethods.Contains(m)))

            Try
                For Each md In publicMethods
                    If publicMethods.Contains(md) Then
                        If Utils.HasUnsafeInstructions(md) = False Then
                            Using optim As New Msil(md.Body)
                                For i = 0 To md.Body.Instructions.Count - 1
                                    Dim item As Instruction = md.Body.Instructions.Item(i)
                                    If (item.OpCode = OpCodes.Call) Then
                                        Try
                                            Dim originalReference As MethodReference = DirectCast(item.Operand, MethodReference)
                                            Dim originalMethod As MethodDefinition = originalReference.Resolve

                                            If Not originalMethod Is Nothing AndAlso Not originalMethod.DeclaringType Is Nothing AndAlso Not completedMethods.Contains(originalMethod) Then
                                                If originalMethod.IsPInvokeImpl Then
                                                    If originalMethod.Name = "SendMessage" OrElse originalMethod.Name = "PostMessage" Then
                                                        Continue For
                                                    End If

                                                    PinvokeCreate.InitPinvokeInfos(originalMethod, td)
                                                    PinvokeCreate.CreatePinvokeBody(LoaderInvoke)

                                                    completedMethods.Add(originalMethod)
                                                End If
                                            End If
                                        Catch ex As Mono.Cecil.AssemblyResolutionException
                                            Continue For
                                        End Try
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

#End Region

    End Class

End Namespace

