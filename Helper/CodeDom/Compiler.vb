Imports Helper.RandomizeHelper
Imports System.CodeDom.Compiler
Imports System.IO
Imports Helper.UtilsHelper

Namespace CodeDomHelper
    Public Class Compiler

#Region " Methods "
        Public Shared Function CreateStubFromString(MainClass$, FrmwkVersion$, str$, Optional ByVal ReferencencedAssemblies As Dictionary(Of String, Byte()) = Nothing) As String
            Try
                Dim nam = Randomizer.GenerateNewAlphabetic()
                Dim Version = New Collections.Generic.Dictionary(Of String, String) : Version.Add("CompilerVersion", FrmwkVersion)
                Dim cProv As New VBCodeProvider(Version)
                Dim cParams As New CompilerParameters()
                With cParams
                    With cParams.ReferencedAssemblies
                        .Add("System.Windows.Forms.dll")
                        .Add("mscorlib.dll")
                        .Add("System.dll")
                        If str.Contains("Imports System.Linq") Then
                            .Add("System.Core.dll")
                        End If
                        If Not ReferencencedAssemblies Is Nothing Then
                            For Each it In ReferencencedAssemblies
                                File.WriteAllBytes(it.Key, it.Value)
                                .Add(it.Key)
                            Next
                        End If
                    End With
                    .CompilerOptions = "/target:library /platform:anycpu /optimize+ /debug-"
                    .GenerateExecutable = False
                    .OutputAssembly = My.Computer.FileSystem.SpecialDirectories.Temp & "\" & nam
                    .GenerateInMemory = True
                    .IncludeDebugInformation = False
                    .MainClass = MainClass
                End With
                Dim cResults As CompilerResults = cProv.CompileAssemblyFromSource(cParams, str)
                If cResults.Errors.Count <> 0 Then
                    For Each er In cResults.Errors
                        MsgBox("Error on line : " & er.Line.ToString & vbNewLine & _
                              "Error description : " & er.ErrorText & vbNewLine & _
                              "File : " & vbNewLine & str)
                    Next
                Else
                    Return My.Computer.FileSystem.SpecialDirectories.Temp & "\" & nam
                End If
            Catch ex As Exception
                MsgBox("Error (CreateStubFromString) : " & ex.ToString)
            End Try

            Return Nothing
        End Function

        Public Shared Function CreateTypeFromString(MainClass$, FrmwkVersion$, str$) As Type
            Try
                Dim Version = New Collections.Generic.Dictionary(Of String, String) : Version.Add("CompilerVersion", FrmwkVersion)
                Dim cProv As New VBCodeProvider(Version)
                Dim cParams As New CompilerParameters()
                With cParams
                    With .ReferencedAssemblies
                        .Add("mscorlib.dll")
                        .Add("System.dll")
                        .Add("System.Windows.Forms.dll")
                    End With
                    .CompilerOptions = "/target:library /platform:anycpu /optimize+ /debug-"
                    .GenerateExecutable = False
                    .GenerateInMemory = True
                    .IncludeDebugInformation = False
                End With

                Dim cResults As CompilerResults = cProv.CompileAssemblyFromSource(cParams, Str)
                If cResults.Errors.Count <> 0 Then
                    For Each er In cResults.Errors
                        MsgBox("Error on line : " & er.Line.ToString & vbNewLine & _
                               "Error description : " & er.ErrorText & vbNewLine & _
                               "File : " & vbNewLine & str)
                    Next
                Else
                    Return cResults.CompiledAssembly.CreateInstance(MainClass).GetType()
                End If
            Catch ex As Exception
                MsgBox("Error (CreateTypeFromString) : " & ex.ToString)
            End Try

            Return Nothing
        End Function
#End Region
      
    End Class
End Namespace