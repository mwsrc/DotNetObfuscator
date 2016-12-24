Imports Mono.Cecil.Cil
Imports Mono.Cecil

Namespace Core.Obfuscation.Exclusion
    ''' <summary>
    ''' By Yck from Confuser
    ''' </summary>
    Public NotInheritable Class ReflectionAnalyzer

#Region " Structures "
        Public Structure ReflectionMethod
            Public typeName As String
            Public mtdName As String
            Public paramLoc As Integer()
            Public paramType As String()
        End Structure
#End Region

#Region " Methods "
        Private Shared Sub FollowStack(ByVal op As OpCode, ByVal stack As Stack(Of Object))
            Select Case op.StackBehaviourPop
                Case StackBehaviour.Pop1, StackBehaviour.Pop1_pop1, StackBehaviour.Popi, StackBehaviour.Popref
                    stack.Pop()
                    Exit Select
                Case StackBehaviour.Popi_pop1, StackBehaviour.Popi_popi, StackBehaviour.Popi_popi8, StackBehaviour.Popi_popr4, StackBehaviour.Popi_popr8, StackBehaviour.Popref_pop1, StackBehaviour.Popref_popi
                    stack.Pop()
                    stack.Pop()
                    Exit Select
                Case StackBehaviour.Popi_popi_popi, StackBehaviour.Popref_popi_popi, StackBehaviour.Popref_popi_popi8, StackBehaviour.Popref_popi_popr4, StackBehaviour.Popref_popi_popr8, StackBehaviour.Popref_popi_popref
                    stack.Pop()
                    stack.Pop()
                    stack.Pop()
                    Exit Select
                Case StackBehaviour.PopAll
                    stack.Clear()
                    Exit Select
                Case StackBehaviour.Varpop
                    Throw New InvalidOperationException
            End Select
            Select Case op.StackBehaviourPush
                Case StackBehaviour.Push1, StackBehaviour.Pushi, StackBehaviour.Pushi8, StackBehaviour.Pushr4, StackBehaviour.Pushr8, StackBehaviour.Pushref
                    stack.Push(Nothing)
                    Return
                Case StackBehaviour.Push1_push1
                    stack.Push(Nothing)
                    stack.Push(Nothing)
                    Return
                Case StackBehaviour.Varpop
                    Return
                Case StackBehaviour.Varpush
                    Throw New InvalidOperationException
            End Select
        End Sub

        Private Shared Sub FollowStack(ByVal op As OpCode, ByRef stack As Integer)
            Select Case op.StackBehaviourPop
                Case StackBehaviour.Pop1, StackBehaviour.Pop1_pop1, StackBehaviour.Popi, StackBehaviour.Popref
                    stack -= 1
                    Exit Select
                Case StackBehaviour.Popi_pop1, StackBehaviour.Popi_popi, StackBehaviour.Popi_popi8, StackBehaviour.Popi_popr4, StackBehaviour.Popi_popr8, StackBehaviour.Popref_pop1, StackBehaviour.Popref_popi
                    stack = (stack - 2)
                    Exit Select
                Case StackBehaviour.Popi_popi_popi, StackBehaviour.Popref_popi_popi, StackBehaviour.Popref_popi_popi8, StackBehaviour.Popref_popi_popr4, StackBehaviour.Popref_popi_popr8, StackBehaviour.Popref_popi_popref
                    stack = (stack - 3)
                    Exit Select
                Case StackBehaviour.PopAll
                    stack = 0
                    Exit Select
                Case StackBehaviour.Varpop
                    Throw New InvalidOperationException
            End Select
            Select Case op.StackBehaviourPush
                Case StackBehaviour.Push1, StackBehaviour.Pushi, StackBehaviour.Pushi8, StackBehaviour.Pushr4, StackBehaviour.Pushr8, StackBehaviour.Pushref
                    stack += 1
                    Return
                Case StackBehaviour.Push1_push1
                    stack = (stack + 2)
                    Return
                Case StackBehaviour.Varpop
                    Return
                Case StackBehaviour.Varpush
                    Throw New InvalidOperationException
            End Select
        End Sub

        Public Shared Function StackTrace(ByVal idx As Integer, ByVal body As MethodBody, ByVal mtd As ReflectionMethod, ByVal scope As ModuleDefinition, ByRef memInst As Instruction) As MemberReference
            memInst = Nothing
            Dim instructions As Mono.Collections.Generic.Collection(Of Instruction) = body.Instructions
            Dim c As Integer = (If(TryCast(instructions.Item(idx).Operand, MethodReference).HasThis, 1, 0) + TryCast(instructions.Item(idx).Operand, MethodReference).Parameters.Count)
            If (instructions.Item(idx).OpCode.Code = Code.Newobj) Then
                c -= 1
            End If
            Dim stack As Integer = 0
            idx -= 1
            Do While (idx >= 0)
                If (c = stack) Then
                    Exit Do
                End If
                Dim instruction As Instruction = instructions.Item(idx)
                Select Case instruction.OpCode.Code
                    Case Code.Ldc_I4, Code.Ldc_I8, Code.Ldc_R4, Code.Ldc_R8, Code.Ldstr
                        stack += 1
                        Exit Select
                    Case Code.Pop
                        stack -= 1
                        Exit Select
                    Case Code.Call, Code.Callvirt
                        Dim operand As MethodReference = TryCast(instruction.Operand, MethodReference)
                        stack = (stack - (If(operand.HasThis, 1, 0) + operand.Parameters.Count))
                        If (operand.ReturnType.FullName <> "System.Void") Then
                            stack += 1
                        End If
                        Exit Select
                    Case Code.Ldnull
                        stack += 1
                        Exit Select
                    Case Code.Newobj
                        Dim reference2 As MethodReference = TryCast(instruction.Operand, MethodReference)
                        stack = (stack - (reference2.Parameters.Count - 1))
                        Exit Select
                    Case Code.Ldfld
                        stack += 1
                        Exit Select
                    Case Code.Stfld, Code.Starg, Code.Stloc
                        stack -= 1
                        Exit Select
                    Case Code.Ldtoken
                        stack += 1
                        Exit Select
                    Case Code.Ldarg
                        stack += 1
                        Exit Select
                    Case Code.Ldloc
                        stack += 1
                        Exit Select
                    Case Else
                        ReflectionAnalyzer.FollowStack(instruction.OpCode, stack)
                        Exit Select
                End Select
                idx -= 1
            Loop
            Return ReflectionAnalyzer.StackTrace2((idx + 1), c, body, mtd, scope, memInst)
        End Function

        Private Shared Function StackTrace2(ByVal idx As Integer, ByVal c As Integer, ByVal body As MethodBody, ByVal mtd As ReflectionMethod, ByVal scope As ModuleDefinition, ByRef memInst As Instruction) As MemberReference
            memInst = Nothing
            Dim num As Integer = c
            Dim stack As New Stack(Of Object)
            Dim num2 As Integer = idx
            Do While True
                If (stack.Count = num) Then
                    Dim arr As Object() = stack.ToArray
                    Array.Reverse(arr)
                    Dim str As String = Nothing
                    Dim type As TypeDefinition = Nothing
                    Dim resource As Resource = Nothing
                    Dim i As Integer
                    For i = 0 To mtd.paramLoc.Length - 1
                        Dim predicate As Func(Of Resource, Boolean) = Nothing
                        If (mtd.paramLoc(i) >= arr.Length) Then
                            Return Nothing
                        End If
                        Dim param As Object = arr(mtd.paramLoc(i))
                        Dim str2 As String = mtd.paramType(i)
                        If (Not str2 Is Nothing) Then
                            If Not (str2 = "Target") Then
                                If ((str2 = "Type") OrElse (str2 = "This")) Then
                                    GoTo Label_0333
                                End If
                                If (str2 = "TargetType") Then
                                    GoTo Label_0363
                                End If
                                If (str2 = "TargetResource") Then
                                    GoTo Label_038A
                                End If
                            Else
                                str = TryCast(param, String)
                                If (str Is Nothing) Then
                                    Return Nothing
                                End If
                                memInst = ReflectionAnalyzer.StackTrace3(idx, c, body.Instructions, mtd.paramLoc(i))
                            End If
                        End If
                        Continue For
Label_0333:
                        If TypeOf param Is TypeDefinition Then
                            type = TryCast(param, TypeDefinition)
                        Else
                            type = body.Method.DeclaringType
                        End If
                        Continue For
Label_0363:
                        If Not TypeOf param Is String Then
                            Return Nothing
                        End If
                        type = scope.GetType(TryCast(param, String))
                        Continue For
Label_038A:
                        If Not TypeOf param Is String Then
                            Return Nothing
                        End If
                        If (predicate Is Nothing) Then
                            predicate = Function(r) (r.Name = (TryCast(param, String) & ".resources"))
                        End If
                        resource = Enumerable.FirstOrDefault(Of Resource)(scope.Resources, predicate)
                        memInst = ReflectionAnalyzer.StackTrace3(idx, c, body.Instructions, mtd.paramLoc(i))
                    Next i
                    If (((Not str Is Nothing) OrElse (Not type Is Nothing)) OrElse (Not resource Is Nothing)) Then
                        If (Not resource Is Nothing) Then
                            Return Nothing
                        End If
                        If ((Not str Is Nothing) AndAlso (Not type Is Nothing)) Then
                            Dim definition2 As FieldDefinition
                            For Each definition2 In type.Fields
                                If (definition2.Name = str) Then
                                    Return definition2
                                End If
                            Next
                            Dim definition3 As MethodDefinition
                            For Each definition3 In type.Methods
                                If (definition3.Name = str) Then
                                    Return definition3
                                End If
                            Next
                            Dim definition4 As PropertyDefinition
                            For Each definition4 In type.Properties
                                If (definition4.Name = str) Then
                                    Return definition4
                                End If
                            Next
                            Dim definition5 As EventDefinition
                            For Each definition5 In type.Events
                                If (definition5.Name = str) Then
                                    Return definition5
                                End If
                            Next
                        ElseIf (Not type Is Nothing) Then
                            memInst = ReflectionAnalyzer.StackTrace3(idx, c, body.Instructions, mtd.paramLoc(Array.IndexOf(Of String)(mtd.paramType, "TargetType")))
                            Return type
                        End If
                    End If
                    Return Nothing
                End If
                Dim instruction As Instruction = body.Instructions.Item(num2)
                Select Case instruction.OpCode.Code
                    Case Code.Ldc_I4, Code.Ldc_I8, Code.Ldc_R4, Code.Ldc_R8, Code.Ldstr
                        stack.Push(instruction.Operand)
                        Exit Select
                    Case Code.Pop
                        stack.Pop()
                        Exit Select
                    Case Code.Call, Code.Callvirt
                        Dim operand As MethodReference = TryCast(instruction.Operand, MethodReference)
                        If ((operand.Name <> "GetTypeFromHandle") OrElse (operand.DeclaringType.FullName <> "System.Type")) Then
                            Dim num3 As Integer = (-If(operand.HasThis, 1, 0) - operand.Parameters.Count)
                            Dim j As Integer = num3
                            Do While (j <> 0)
                                stack.Pop()
                                j += 1
                            Loop
                            If (operand.ReturnType.FullName <> "System.Void") Then
                                stack.Push(operand.ReturnType)
                            End If
                        End If
                        Exit Select
                    Case Code.Ldnull
                        stack.Push(Nothing)
                        Exit Select
                    Case Code.Newobj
                        Dim reference2 As MethodReference = TryCast(instruction.Operand, MethodReference)
                        Dim k As Integer = -reference2.Parameters.Count
                        Do While (k <> 0)
                            stack.Pop()
                            k += 1
                        Loop
                        stack.Push(reference2.DeclaringType)
                        Exit Select
                    Case Code.Ldfld
                        stack.Push(TryCast(instruction.Operand, FieldReference).FieldType)
                        Exit Select
                    Case Code.Stfld, Code.Starg, Code.Stloc
                        stack.Pop()
                        Exit Select
                    Case Code.Ldtoken
                        stack.Push(instruction.Operand)
                        Exit Select
                    Case Code.Ldarg
                        stack.Push(TryCast(instruction.Operand, ParameterReference).ParameterType)
                        Exit Select
                    Case Code.Ldloc
                        stack.Push(TryCast(instruction.Operand, VariableReference).VariableType)
                        Exit Select
                    Case Else
                        ReflectionAnalyzer.FollowStack(instruction.OpCode, stack)
                        Exit Select
                End Select
                num2 += 1
            Loop

            Return Nothing
        End Function

        Private Shared Function StackTrace3(ByVal idx As Integer, ByVal count As Integer, ByVal insts As Mono.Collections.Generic.Collection(Of Instruction), ByVal c As Integer) As Instruction
            c = (count - c)
            Do While True
                If (count < c) Then
                    Return insts.Item((idx - 1))
                End If
                Dim instruction As Instruction = insts.Item(idx)
                Select Case instruction.OpCode.Code
                    Case Code.Ldc_I4, Code.Ldc_I8, Code.Ldc_R4, Code.Ldc_R8, Code.Ldstr
                        count -= 1
                        Exit Select
                    Case Code.Pop
                        count += 1
                        Exit Select
                    Case Code.Call, Code.Callvirt
                        Dim operand As MethodReference = TryCast(instruction.Operand, MethodReference)
                        count = (count + (If(operand.HasThis, 1, 0) + operand.Parameters.Count))
                        If (operand.ReturnType.FullName <> "System.Void") Then
                            count -= 1
                        End If
                        Exit Select
                    Case Code.Ldnull
                        count -= 1
                        Exit Select
                    Case Code.Newobj
                        Dim reference2 As MethodReference = TryCast(instruction.Operand, MethodReference)
                        c = (c + (reference2.Parameters.Count - 1))
                        Exit Select
                    Case Code.Ldfld
                        count -= 1
                        Exit Select
                    Case Code.Stfld, Code.Starg, Code.Stloc
                        count += 1
                        Exit Select
                    Case Code.Ldtoken
                        count -= 1
                        Exit Select
                    Case Code.Ldarg
                        count -= 1
                        Exit Select
                    Case Code.Ldloc
                        count -= 1
                        Exit Select
                    Case Else
                        Dim num As Integer = count
                        ReflectionAnalyzer.FollowStack(instruction.OpCode, count)
                        count = (count - (count - num))
                        Exit Select
                End Select
                idx += 1
            Loop
            Return Nothing
        End Function
#End Region

    End Class

End Namespace

