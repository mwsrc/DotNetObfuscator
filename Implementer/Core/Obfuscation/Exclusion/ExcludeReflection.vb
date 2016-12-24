Imports System.IO
Imports Mono.Cecil.Cil
Imports Mono.Cecil.Rocks
Imports Mono.Cecil

Namespace Core.Obfuscation.Exclusion

    Public NotInheritable Class ExcludeReflection

#Region " Properties "
        Public Shared HasItems As Boolean
#End Region

#Region " Methods "
        Public Shared Sub AnalyzeCodes(assDef As AssemblyDefinition, m_exclude As ExcludeList)
            For Each modul In assDef.Modules
                If modul.HasTypes Then
                    For Each type In modul.GetAllTypes
                        If type.HasMethods Then
                            For Each method In type.Methods
                                If method.HasBody AndAlso method.Body.Instructions.Count <> 0 Then
                                    For i = 0 To method.Body.Instructions.Count - 1
                                        Dim Inst = method.Body.Instructions(i)
                                        If TypeOf Inst.Operand Is MethodReference Then
                                            Dim refer As MethodReference = TryCast(Inst.Operand, MethodReference)
                                            Dim id = refer.DeclaringType.FullName & "::" & refer.Name

                                            If ExclusionReflection.Reflections.ContainsKey(id) Then
                                                Dim Rmtd = ExclusionReflection.Reflections(id)
                                                Dim memInst As Instruction = Nothing
                                                Dim mem = ReflectionAnalyzer.StackTrace(i, method.Body, Rmtd, method.Module, memInst)
                                                If Not mem Is Nothing Then
                                                    m_exclude.AddTo(New ExclusionState(True, TryCast(mem, TypeDefinition), ExclusionState.mType.Types, False, False, False, False, True, False, False))
                                                    m_exclude.AddTo(New ExclusionState(True, TryCast(refer, MethodDefinition), ExclusionState.mType.Methods, False, False, False, False, False, True, False))
                                                    HasItems = True
                                                End If
                                            End If
                                        End If
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If
            Next
            'm_methodReferences = m_assdef.MainModule.Types.Cast(Of TypeDefinition).SelectMany(Function(type) Type.Methods.Cast(Of MethodDefinition)()).Where(Function(method) method.Body IsNot Nothing).SelectMany(Function(method) method.Body.Instructions.Cast(Of Instruction)()).Select(Function(instr) InStr.Operand).OfType(Of MethodReference)()
        End Sub

        Public Shared Sub CleanUp()
            HasItems = False
        End Sub

#End Region

    End Class

End Namespace
