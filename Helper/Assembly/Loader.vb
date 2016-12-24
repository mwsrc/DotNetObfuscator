Imports System.Reflection
Imports System.IO
Imports Helper.RandomizeHelper
Imports Helper.UtilsHelper

Namespace AssemblyHelper
    Public Class Loader

#Region " Methods "
        Public Shared Function Minimal(AssPath$) As Data

            'Dim inf As New Infos()
            'inf.

            Dim tempAppDomain As AppDomain = Nothing
            Dim fName = Randomizer.GenerateNewAlphabetic
            Dim path As String = String.Format("{0}{1}\", System.IO.Path.GetTempPath, fName)
            Directory.CreateDirectory(path)
            Dim tempAssemblyFilePath As String = (path & fName)
            File.Copy(AssPath, tempAssemblyFilePath, True)

            Dim AssData As New Data
            Try
                tempAppDomain = AppDomain.CreateDomain(Randomizer.GenerateNewAlphabetic)

                Dim assemblyBuffer As Byte() = File.ReadAllBytes(tempAssemblyFilePath)

                Dim anObject As Object = Nothing

                If Assembly.GetExecutingAssembly.GetName.Name.ToLower = "helper" Then
                    anObject = tempAppDomain.CreateInstanceAndUnwrap("Helper", "Helper.AssemblyHelper.Infos")
                Else
                    anObject = tempAppDomain.CreateInstanceAndUnwrap("DotNetPatcher", "Helper.AssemblyHelper.Infos")
                End If

                Dim assemblyInspector As IAssemblyInfos = TryCast(anObject, IAssemblyInfos)

                Dim AssName$ = String.Empty
                Dim AssVersion$ = String.Empty
                Dim IsWpf As Boolean
                Dim Location$ = String.Empty
                Dim EntryPoint As MethodInfo = Nothing
                Dim AssemblyReferences As AssemblyName() = Nothing
                Dim ManifestResourceNames As IEnumerable(Of String) = Nothing
                Dim ManifestResourceStreams As New List(Of Stream)
                Dim TypesClass As IEnumerable(Of Type) = Nothing
                Dim Modules As IEnumerable(Of [Module]) = Nothing
                Dim Result As Data.Message

                assemblyInspector.GetAssemblyInfo(assemblyBuffer, AssName, AssVersion, IsWpf, EntryPoint, AssemblyReferences, ManifestResourceNames, ManifestResourceStreams, TypesClass, Modules, Result)

                With AssData
                    .AssName = AssName
                    .AssVersion = AssVersion
                    .IsWpf = IsWpf
                    .Location = AssPath
                    .EntryPoint = EntryPoint
                    .AssemblyReferences = AssemblyReferences
                    .Result = Result
                End With

            Catch exception As Exception
                'MsgBox(exception.ToString)
            Finally
                CleanDomain(tempAppDomain, tempAssemblyFilePath, path)
            End Try
            Return AssData
        End Function

        Public Shared Function Full(AssPath$) As DataFull

            Dim tempAppDomain As AppDomain = Nothing
            Dim fName = Randomizer.GenerateNewAlphabetic
            Dim path As String = String.Format("{0}{1}\", System.IO.Path.GetTempPath, fName)
            Directory.CreateDirectory(path)
            Dim tempAssemblyFilePath As String = (path & fName)
            File.Copy(AssPath, tempAssemblyFilePath, True)

            Dim AssData As New DataFull
            Try
                tempAppDomain = AppDomain.CreateDomain(Randomizer.GenerateNewAlphabetic)

                Dim assemblyBuffer As Byte() = File.ReadAllBytes(tempAssemblyFilePath)

                Dim anObject As Object = Nothing

                If Assembly.GetExecutingAssembly.GetName.Name.ToLower = "helper" Then
                    anObject = tempAppDomain.CreateInstanceAndUnwrap("Helper", "Helper.AssemblyHelper.Infos")
                Else
                    anObject = tempAppDomain.CreateInstanceAndUnwrap("DotNetPatcher", "Helper.AssemblyHelper.Infos")
                End If

                Dim assemblyInspector As IAssemblyInfos = TryCast(anObject, IAssemblyInfos)

                Dim AssName$ = String.Empty
                Dim AssVersion$ = String.Empty
                Dim IsWpf As Boolean
                Dim Location$ = String.Empty
                Dim EntryPoint As MethodInfo = Nothing
                Dim AssemblyReferences As AssemblyName() = Nothing
                Dim ManifestResourceNames As IEnumerable(Of String) = Nothing
                Dim ManifestResourceStreams As New List(Of Stream)
                Dim TypesClass As IEnumerable(Of Type) = Nothing
                Dim Modules As IEnumerable(Of [Module]) = Nothing
                Dim Result As DataFull.Message

                assemblyInspector.GetAssemblyInfo(assemblyBuffer, AssName, AssVersion, IsWpf, EntryPoint, AssemblyReferences, ManifestResourceNames, ManifestResourceStreams, TypesClass, Modules, Result, True)

                With AssData
                    .AssName = AssName
                    .AssVersion = AssVersion
                    .IsWpf = IsWpf
                    .Location = New FileInfo(AssPath).DirectoryName
                    .EntryPoint = EntryPoint
                    .AssemblyReferences = AssemblyReferences
                    .ManifestResourceNames = ManifestResourceNames
                    .ManifestResourceStreams = ManifestResourceStreams
                    .TypesClass = TypesClass
                    .Modules = Modules
                    .Result = Result
                End With

            Catch exception As Exception
                'MsgBox(exception.ToString)
            Finally
                CleanDomain(tempAppDomain, tempAssemblyFilePath, path)
            End Try
            Return AssData
        End Function

        Private Shared Sub CleanDomain(tempAppDomain As AppDomain, tempAssemblyFilePath$, path$)
            If Not tempAppDomain Is Nothing Then
                AppDomain.Unload(tempAppDomain)
                If File.Exists(tempAssemblyFilePath) Then
                    File.Delete(tempAssemblyFilePath)
                End If
                If Directory.Exists(path) Then
                    Directory.Delete(path)
                End If
            End If
        End Sub

        Public Shared Function GenerateInfos(ByVal Title As String, ByVal Description As String, ByVal Company As String, ByVal Product As String, ByVal Copyright As String, ByVal Trademark As String, ByVal Version As String) As String
            Return "Imports System.Reflection" & vbNewLine & vbNewLine _
            & "<" & "Assembly: AssemblyTitle(""" & Title & """)>" & vbNewLine _
            & "<Assembly: AssemblyDescription(""" & Description & """)>" & vbNewLine _
            & "<" & "Assembly: AssemblyCompany(""" & Company & """)>" & vbNewLine _
            & "<Assembly: AssemblyProduct(""" & Product & """)>" & vbNewLine _
            & "<Assembly: AssemblyCopyright(""" & Copyright & """)>" & vbNewLine _
            & "<" & "Assembly: AssemblyTrademark(""" & Trademark & """)>" & vbNewLine _
            & "<Assembly: AssemblyVersion(""" & Version & """)>" & vbNewLine _
            & "<Assembly: AssemblyFileVersion(""" & Version & """)>"
        End Function
#End Region
      
    End Class
End Namespace