Imports Helper.RandomizeHelper
Imports System.IO
Imports Helper.CecilHelper
Imports Mono.Cecil.Cil
Imports Mono.Cecil.Rocks
Imports System.Threading
Imports Mono.Cecil
Imports System.Security.Cryptography
Imports Helper.CodeDomHelper
Imports Implementer.Core.Obfuscation.Builder

Namespace Core.Obfuscation.Anti

    Public NotInheritable Class AntiTamper
        Inherits Source

#Region " Methods "
        Friend Shared Sub CreateAntiTamperClass(AssDef As AssemblyDefinition, ByVal framwk$, EnabledPack As Boolean)
            Try
                AssemblyDef = AssDef
                Frmwk = framwk
                Pack = EnabledPack

                Dim reposit = New Stub(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic)
                With reposit
                    .ResolveTypeFromFile(AntiTamperStub(.className, .funcName1), Finder.FindDefaultNamespace(AssDef, Pack), Randomizer.GenerateNew, Randomizer.GenerateNew)
                    .InjectToCctor(AssDef)
                    .InjectToMyProject()
                    .DeleteDll()
                End With

                CleanUp()
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End Sub

        Public Shared Sub InjectMD5(ByVal SelectedFile As String)
            Dim md5bytes As Byte() = Nothing
            md5bytes = CType(CryptoConfig.CreateFromName("MD5"), HashAlgorithm).ComputeHash(File.ReadAllBytes(SelectedFile))
            Using stream = New FileStream(SelectedFile, FileMode.Append)
                stream.Write(md5bytes, 0, md5bytes.Length)
            End Using
        End Sub

#End Region

    End Class

End Namespace