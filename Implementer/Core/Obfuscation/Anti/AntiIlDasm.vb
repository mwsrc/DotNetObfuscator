Imports Mono.Cecil
Imports System.IO
Imports Mono.Cecil.Cil
Imports System.Threading
Imports System.Runtime.CompilerServices
Imports Helper.CecilHelper

Namespace Core.Obfuscation.Anti

    Public NotInheritable Class AntiIlDasm

#Region " Methods "

        Friend Shared Sub Inject(Ass As AssemblyDefinition)
            Dim si As Type = GetType(SuppressIldasmAttribute)
            If Finder.FindCustomAttributeByName(Ass, si.Name) = False Then
                Ass.CustomAttributes.Add(New CustomAttribute(Ass.MainModule.Import(si.GetConstructor(Type.EmptyTypes))))
            End If
        End Sub

#End Region

    End Class

End Namespace

