Imports Mono.Cecil.Cil

Namespace CecilHelper
    Public NotInheritable Class ProcessedIL

#Region " Fields "
        Public Instruction As Instruction
        Public OriginalOffset As Integer
#End Region

#Region " Constructors "
        Public Sub New(ByVal instruct As Instruction)
            Me.New(instruct, instruct.Offset)
        End Sub

        Public Sub New(instruct As Instruction, isNewInstruction As Boolean)
            Me.New(instruct, If(isNewInstruction, -1, instruct.Offset))
        End Sub

        Public Sub New(instruct As Instruction, originalOffset%)
            Instruction = instruct
            originalOffset = originalOffset
        End Sub
#End Region

    End Class

End Namespace
