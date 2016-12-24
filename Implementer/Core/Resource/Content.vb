Imports System.Resources
Imports Mono.Cecil
Imports Helper.RandomizeHelper
Imports System.IO
Imports Helper.CecilHelper
Imports System.Text.RegularExpressions
Imports Helper.UtilsHelper
Imports Helper.ResourcesHelper
Imports Implementer.Core.Dependencing
Imports Implementer.engine.Processing

Namespace Core.Resources

    Public NotInheritable Class Content

#Region " Fields "
        Private Shared m_Resources As New List(Of EmbeddedResource)
        Private Shared m_embeddedResource As New Dictionary(Of EmbeddedResource, EmbeddedResource)
#End Region

#Region " Methods "

        Friend Shared Sub Rename(assdef As AssemblyDefinition)
            If assdef.MainModule.HasResources Then
                For Each EmbRes As Resource In assdef.MainModule.Resources
                    m_Resources.Add(EmbRes)
                Next
                For Each modul In (From m In assdef.Modules
                    Where m.HasTypes
                    Select m)
                    For Each type As TypeDefinition In modul.GetTypes
                        RenameContent(type)
                    Next
                Next
            End If
        End Sub

        Private Shared Sub RenameContent(typeDef As TypeDefinition)
            For Each prop In (From p In typeDef.Properties
                Where Not p.GetMethod Is Nothing AndAlso p.GetMethod.Name = "get_ResourceManager" AndAlso p.GetMethod.HasBody
                Select p)
                If prop.GetMethod.Body.Instructions.Count <> 0 Then
                    For Each instruction In prop.GetMethod.Body.Instructions
                        If TypeOf instruction.Operand Is String Then
                            Dim NewResManagerName$ = instruction.Operand
                            For Each EmbRes As Resource In m_Resources
                                UpdateResources(typeDef, EmbRes, NewResManagerName)
                            Next
                        End If
                    Next
                End If
            Next
        End Sub

        Private Shared Sub UpdateResources(TypeDef As TypeDefinition, OriginalEmbeddedRes As EmbeddedResource, KeyNameOriginal$)
            Try
                Dim ToHex = Functions.StreamToHex(OriginalEmbeddedRes.GetResourceStream)
                If Not ToHex.StartsWith(Functions.StrToHex("MZ")) Then
                    If ToHex.Contains(Functions.StrToHex("System.Resources.ResourceReader, mscorlib, ")) Then

                        If Not OriginalEmbeddedRes.GetResourceStream Is Nothing Then
                            Dim NewEmbeddedRes As New ResourceWriter(KeyNameOriginal)

                            Using read As New ResourceReader(OriginalEmbeddedRes.GetResourceStream)
                                For Each Dat As System.Collections.DictionaryEntry In read
                                    Dim data() As Byte = Nothing
                                    Dim dataType = String.Empty
                                    Dim originalDataKey$ = Dat.Key
                                    read.GetResourceData(Dat.Key, dataType, data)
                                    Dim obfuscatedDataKey$ = UpdateKey(NewEmbeddedRes, dataType, data)
                                    UpdateResourcesKeys(TypeDef, obfuscatedDataKey, originalDataKey, OriginalEmbeddedRes.Name)
                                Next
                            End Using

                            NewEmbeddedRes.Generate()
                            NewEmbeddedRes.Close()
                            NewEmbeddedRes.Dispose()

                            UpdateAssembly(TypeDef, KeyNameOriginal, OriginalEmbeddedRes)
                        End If
                    End If
                End If
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End Sub

        Private Shared Sub UpdateResourcesKeys(TypeDef As TypeDefinition, NewKeyName$, OriginalKeyName$, resName$, Optional ByVal Ren As Boolean = False)
            If resName.EndsWith("Resources.resources") = False Then
                If resName.EndsWith(".resources") Then
                    Dim typeFullName$ = resName.Substring(0, resName.LastIndexOf("."))
                    Dim typeName$ = typeFullName.Replace(".resources", String.Empty).Substring(typeFullName.LastIndexOf(".") + 1)
                    Dim typeSearch As TypeDefinition = Finder.FindType(TypeDef.Module.Assembly.MainModule, typeName)
                    If Not typeSearch Is Nothing Then
                        Dim methodSearch As MethodDefinition = Finder.FindMethod(typeSearch, "InitializeComponent")
                        If Not methodSearch Is Nothing Then
                            UpdateMethodBody(methodSearch, NewKeyName, OriginalKeyName)
                        End If
                    End If
                End If
            ElseIf resName.EndsWith("Resources.resources") Then
                For Each pr In TypeDef.Properties
                    If Not pr.GetMethod Is Nothing Then
                        If pr.GetMethod.Name = "get_" & Regex.Replace(OriginalKeyName, "[^\w]+", "_") Then
                            pr.GetMethod.Name = Mapping.RenameMethodMember(pr.GetMethod, Randomizer.GenerateNew())
                            UpdateMethodBody(pr.GetMethod, NewKeyName, OriginalKeyName)
                        End If
                    End If
                Next
            End If
        End Sub

        Private Shared Sub UpdateMethodBody(Meth As MethodDefinition, NewKeyName$, OriginalKeyName$)
            If Meth.HasBody AndAlso Meth.Body.Instructions.Count <> 0 Then
                For Each instruction As Cil.Instruction In Meth.Body.Instructions
                    If TypeOf instruction.Operand Is String Then
                        If CStr(instruction.Operand) = OriginalKeyName Then
                            instruction.Operand = NewKeyName
                        End If
                    End If
                Next
            End If
        End Sub

        Private Shared Function UpdateKey(NewEmbeddedRes As ResourceWriter, datatype As Object, data As Byte(), Optional ByVal Ren As Boolean = False) As String
            Dim newdataKey = Randomizer.GenerateNew()
            NewEmbeddedRes.AddResourceData(newdataKey, datatype, data)
            Return newdataKey
        End Function

        Private Shared Sub UpdateAssembly(TypeDef As TypeDefinition, resWriterPath$, OriginalEmbeddedResource As Resource)
            Try
                Dim CompressRes = New EmbeddedResource(OriginalEmbeddedResource.Name, ManifestResourceAttributes.Private, File.ReadAllBytes(My.Application.Info.DirectoryPath & "\" & resWriterPath))
                If Not m_embeddedResource.ContainsKey(OriginalEmbeddedResource) Then
                    m_embeddedResource.Add(OriginalEmbeddedResource, CompressRes)
                    TypeDef.Module.Assembly.MainModule.Resources.Remove(OriginalEmbeddedResource)
                    TypeDef.Module.Assembly.MainModule.Resources.Add(CompressRes)
                End If
                File.Delete(My.Application.Info.DirectoryPath & "\" & resWriterPath)
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End Sub

        Private Shared Sub CleanUpTmpFiles()
            Try
                For Each f In System.IO.Directory.GetFiles(My.Application.Info.DirectoryPath, "*.resources", IO.SearchOption.TopDirectoryOnly)
                    System.IO.File.Delete(f)
                Next
            Catch ex As Exception
            End Try
        End Sub

        Friend Shared Sub Cleanup()
            If m_Resources.Count <> 0 Then m_Resources.Clear()
            If m_embeddedResource.Count <> 0 Then m_embeddedResource.Clear()
            CleanUpTmpFiles()
        End Sub
#End Region

    End Class
End Namespace