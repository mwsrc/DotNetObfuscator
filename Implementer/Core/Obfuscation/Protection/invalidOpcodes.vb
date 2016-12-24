Imports System.Text
Imports System.IO
Imports System.CodeDom.Compiler
Imports Mono.Cecil
Imports Mono.Cecil.Cil
Imports System.ComponentModel
Imports System.Resources
Imports Helper.RandomizeHelper
Imports Helper.CecilHelper
Imports Mono.Cecil.Rocks
Imports Implementer.Core.Obfuscation.Exclusion

Namespace Core.Obfuscation.Protection
    Public NotInheritable Class invalidOpcodes

#Region " Fields "
        Private Shared r As Random
#End Region

#Region " Constructor "
        Shared Sub New()
            r = New Random
        End Sub
#End Region

#Region " Methods "
        Friend Shared Sub Inject(library As AssemblyDefinition, Exclude As ExcludeList)
            For Each mode In (From m In library.Modules
                 Where m.HasTypes
                 Select m)

                For Each type In (From t In mode.Types
                    Where t.HasMethods
                    Select t)

                    For Each method In (From mtd In type.Methods
                        Where mtd.HasBody AndAlso Not Finder.FindCustomAttributeByName(mtd.DeclaringType, "EditorBrowsableAttribute")
                        Select mtd)
                        Using optim As New Msil(method.Body)
                            For i% = 0 To method.Body.Instructions.Count - 1
                                Dim Instruction As Instruction = method.Body.Instructions(i)
                                If Randomizer.GenerateBoolean() Then
                                    If Exclude.isInvalidOpcodesExclude(type) = False Then
                                        optim.StackUnflow(r)
                                    End If
                                Else
                                    If Exclude.isInvalidOpcodesExclude(type) = False Then
                                        optim.StackOverFlow(r)
                                    End If
                                End If
                            Next
                            optim.FixBranchOffsets()
                            optim.MethodBody.SimplifyMacros()
                        End Using
                    Next
                Next
            Next
        End Sub
#End Region

    End Class
End Namespace

