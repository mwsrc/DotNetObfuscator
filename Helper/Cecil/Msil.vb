Imports Mono.Cecil.Cil

Namespace CecilHelper
    Public Class Msil
        Implements IDisposable

#Region " Properties "
        Property MethodBody As MethodBody
        Property ProcessedInstructions As List(Of ProcessedIL)
#End Region
       
#Region " Constructor "
        Public Sub New(ByVal methodBody As MethodBody)
            _MethodBody = methodBody
            CollectInstructions()
        End Sub
#End Region
      
#Region " Methods "
        Public Sub Append(ByVal newInstruction As Instruction)
            _MethodBody.Instructions.Add(newInstruction)
            ProcessedInstructions.Add(New ProcessedIL(newInstruction))
        End Sub

        Public Sub StackOverFlow(r As Random)
            If (_MethodBody.Instructions.Item(0).OpCode.Code <> Code.Br_S OrElse _MethodBody.Instructions.Item(0).OpCode.Code <> Code.Ldarg) Then
                Dim body = _MethodBody
                Dim iLProcessor = body.GetILProcessor
                Dim target = body.Instructions.Item(0)
                Dim instruct = iLProcessor.Create(OpCodes.Br_S, target)
                Dim instruction2 = iLProcessor.Create(OpCodes.Pop)
                Dim instruction3 = iLProcessor.Create(OpCodes.Ldc_I8, CLng(r.Next))
                InsertBefore(target, instruction3)
                InsertBefore(instruction3, instruction2)
                InsertBefore(instruction2, instruct)
            End If
        End Sub

        Public Sub StackUnflow(r As Random)
            Dim iLProcessor0 = _MethodBody.GetILProcessor
            _MethodBody.Instructions.Add(iLProcessor0.Create(GetRndCrap(r)))
            _MethodBody.Instructions.Add(iLProcessor0.Create(OpCodes.Ret))
        End Sub

        Private Function GetRndCrap(r As Random) As OpCode
            Dim code As OpCode
            Select Case r.Next(1, 5)
                Case 1
                    Return OpCodes.Add
                Case 2
                    Return OpCodes.Div
                Case 3
                    Return OpCodes.Xor
                Case 4
                    Return OpCodes.Mul
            End Select
            Return code
        End Function

        Public Sub CalculateOffsets()
            Dim num As Integer = 0
            Dim i As Integer
            For i = 0 To Me.ProcessedInstructions.Count - 1
                Me.ProcessedInstructions.Item(i).Instruction.Offset = num
                num = (num + Me.ProcessedInstructions.Item(i).Instruction.GetSize)
            Next i
        End Sub

        Private Sub CollectInstructions()
            ProcessedInstructions = New List(Of ProcessedIL)
            Dim i%
            For i = 0 To Me._MethodBody.Instructions.Count - 1
                _MethodBody.Instructions.Item(i).OpCode = Me.SimplifyOpCode(Me._MethodBody.Instructions.Item(i).OpCode)
                ProcessedInstructions.Add(New ProcessedIL(Me.MethodBody.Instructions.Item(i)))
            Next i
        End Sub

        Public Function containsTryCatch() As Boolean
            Return _MethodBody.ExceptionHandlers.Count <> 0
        End Function

        Public Function IsSettingStr(str$) As Boolean
            If _MethodBody.Method.IsGetter Then
                Return _MethodBody.Method.Name.ToLower = "get_" & str.ToLower
            ElseIf _MethodBody.Method.IsSetter Then
                Return _MethodBody.Method.Name.ToLower = "set_" & str.ToLower
            End If
            Return False
        End Function

        Public Sub FixBranchOffsets()
            For Each instruct In ProcessedInstructions
                If (instruct.OriginalOffset <> -1) Then
                    If ((Not instruct.Instruction.Operand Is Nothing) AndAlso TypeOf instruct.Instruction.Operand Is Instruction) Then
                        Dim operand = TryCast(instruct.Instruction.Operand, Instruction)
                        For Each instruction3 In ProcessedInstructions
                            If (operand.Offset = instruction3.OriginalOffset) Then
                                instruct.Instruction.OpCode = SimplifyOpCode(instruct.Instruction.OpCode)
                                instruct.Instruction.Operand = instruction3.Instruction
                            End If
                        Next
                    End If
                    If (instruct.Instruction.OpCode.OperandType = OperandType.InlineSwitch) Then
                        Dim instructionArray As Instruction() = TryCast(instruct.Instruction.Operand, Instruction())
                        Dim i%
                        For i = 0 To instructionArray.Length - 1
                            For Each instruction4 In ProcessedInstructions
                                If (instructionArray(i).Offset = instruction4.OriginalOffset) Then
                                    instructionArray(i) = instruction4.Instruction
                                End If
                            Next
                        Next i
                        instruct.Instruction.Operand = instructionArray
                    End If
                End If
            Next
        End Sub

        Private Function GetIndex(ByVal instruction As Instruction) As Integer
            Dim i%
            For i = 0 To ProcessedInstructions.Count - 1
                If (ProcessedInstructions.Item(i).Instruction Is instruction) Then
                    Return i
                End If
            Next i
            Return -1
        End Function

        Private Function GetOpCodeByName(ByVal name As String) As OpCode?
            Dim info As Reflection.FieldInfo
            For Each info In GetType(OpCodes).GetFields
                If (info.Name.ToLower = name) Then
                    Return New OpCode?(DirectCast(info.GetValue(Nothing), OpCode))
                End If
            Next
            Return Nothing
        End Function

        Public Sub InsertAfter(ByVal targetInstruction As Instruction, ByVal newInstruction As Instruction)
            Dim index% = (GetIndex(targetInstruction) + 1)
            _MethodBody.Instructions.Insert(index, newInstruction)
            UpdateExceptionHandlers(newInstruction, (targetInstruction.Offset + targetInstruction.GetSize))
            ProcessedInstructions.Insert(index, New ProcessedIL(newInstruction, True))
        End Sub

        Public Sub InsertBefore(ByVal targetInstruction As Instruction, ByVal newInstruction As Instruction)
            Dim index% = GetIndex(targetInstruction)
            Me._MethodBody.Instructions.Insert(index, newInstruction)
            Me.UpdateExceptionHandlers(newInstruction, targetInstruction.Offset)
            Me.ProcessedInstructions.Insert(index, New ProcessedIL(newInstruction, True))
        End Sub

        Private Function OptimizeOpCode(ByVal opcode As OpCode) As OpCode
            If (opcode.OperandType = OperandType.InlineBrTarget) Then
                Dim name As String = (opcode.Name.ToLower & "_s")
                Dim opCodeByName As OpCode? = Me.GetOpCodeByName(name)
                If opCodeByName.HasValue Then
                    Return opCodeByName.Value
                End If
            End If
            Return opcode
        End Function

        Public Sub Replace(ByVal targetInstruction As Instruction, ByVal newInstruction As Instruction)
            Dim index% = GetIndex(targetInstruction)
            UpdateExceptionHandlers(newInstruction, targetInstruction.Offset)
            ProcessedInstructions.RemoveAt(index)
            ProcessedInstructions.Insert(index, New ProcessedIL(newInstruction, targetInstruction.Offset))
            _MethodBody.Instructions.RemoveAt(index)
            _MethodBody.Instructions.Insert(index, newInstruction)
        End Sub

        Private Function SimplifyOpCode(ByVal opcode As OpCode) As OpCode
            If (opcode.OperandType = OperandType.ShortInlineBrTarget) Then
                Dim name = opcode.Name.Remove((opcode.Name.Length - 2)).ToLower
                Dim opCodeByName As OpCode? = Me.GetOpCodeByName(name)
                If opCodeByName.HasValue Then
                    Return opCodeByName.Value
                End If
            End If
            Return opcode
        End Function

        Private Sub UpdateExceptionHandlers(ByVal instruction As Instruction, ByVal offset As Integer)
            Dim handler As ExceptionHandler
            For Each handler In _MethodBody.ExceptionHandlers
                If (handler.TryStart.Offset = offset) Then
                    handler.TryStart = instruction
                End If
                If (handler.TryEnd.Offset = offset) Then
                    handler.TryEnd = instruction
                End If
                If (handler.HandlerStart.Offset = offset) Then
                    handler.HandlerStart = instruction
                End If
                If (handler.HandlerEnd.Offset = offset) Then
                    handler.HandlerEnd = instruction
                End If
                If ((Not handler.FilterStart Is Nothing) AndAlso (handler.FilterStart.Offset = offset)) Then
                    handler.FilterStart = instruction
                End If
            Next
        End Sub
#End Region
       
#Region "IDisposable Support"
        Private disposedValue As Boolean ' Pour détecter les appels redondants

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: supprimez l'état managé (objets managés).
                End If
                If ProcessedInstructions.Count <> 0 Then ProcessedInstructions.Clear()
                _MethodBody = Nothing
                ' TODO: libérez les ressources non managées (objets non managés) et substituez la méthode Finalize() ci-dessous.
                ' TODO: définissez les champs volumineux à null.
            End If
            Me.disposedValue = True
        End Sub

        ' Ce code a été ajouté par Visual Basic pour permettre l'implémentation correcte du modèle pouvant être supprimé.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ne modifiez pas ce code. Ajoutez du code de nettoyage dans Dispose(disposing As Boolean) ci-dessus.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

End Namespace


