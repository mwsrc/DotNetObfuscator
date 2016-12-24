Imports Mono.Cecil
Imports Helper.RandomizeHelper
Imports System.IO
Imports Helper.CecilHelper
Imports Mono.Cecil.Cil
Imports Mono.Cecil.Rocks
Imports System.Threading
Imports Helper.AssemblyHelper
Imports Helper.CodeDomHelper
Imports Implementer.Core.Obfuscation.Builder

Namespace Core.Obfuscation.Anti

    Public NotInheritable Class AntiDebug
        Inherits Source

#Region " Methods "
        Friend Shared Sub InjectAntiDebug(assDef As AssemblyDefinition, Framwk$, EnabledPack As Boolean)
            Try
                AssemblyDef = assDef
                Frmwk = Framwk
                Pack = EnabledPack

                Dim reposit = New Stub(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic)
                With reposit
                    .ResolveTypeFromFile(AntiDebugStub(.className, .funcName1), Finder.FindDefaultNamespace(assDef, Pack), Randomizer.GenerateNew, Randomizer.GenerateNew)
                    .InjectToCctor(assDef)
                    .DeleteDll()
                End With

                CleanUp()

            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End Sub

#End Region

    End Class
End Namespace
