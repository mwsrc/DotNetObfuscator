Imports System.Reflection
Imports System.IO

Namespace AssemblyHelper

    <Serializable> _
    Public Class Infos
        Implements IAssemblyInfos

#Region " Methods "

        Private Sub AssemblyInfo(assemblyBuffer As Byte(), ByRef AssName$, ByRef AssVersion$, ByRef IsWpfApp As Boolean, ByRef EntryPoint As MethodInfo, ByRef AssemblyReferences As AssemblyName(), ByRef ManifestResourceNames As IEnumerable(Of String), ByRef ManifestResourceStreams As List(Of Stream), ByRef TypesClass As IEnumerable(Of Type), ByRef Modules As IEnumerable(Of [Module]), ByRef Result As Data.Message, Optional ByVal LoadMaxInfos As Boolean = False)
            Try
                Dim assembly = AppDomain.CurrentDomain.Load(assemblyBuffer)

                Dim manifest = assembly.ManifestModule
                AssName = manifest.ScopeName

                Dim fileVersionAttributes = assembly.GetCustomAttributes(GetType(AssemblyFileVersionAttribute), True)
                If fileVersionAttributes.Length = 1 Then
                    Dim fileVersion = TryCast(fileVersionAttributes(0), AssemblyFileVersionAttribute)
                    AssVersion = fileVersion.Version
                End If

                Dim isWpfProg = assembly.GetReferencedAssemblies().Any(Function(x) x.Name.ToLower = "system.xaml") AndAlso _
        assembly.GetManifestResourceNames().Any(Function(x) x.ToLower.EndsWith(".g.resources"))

                IsWpfApp = isWpfProg
                EntryPoint = assembly.EntryPoint
                AssemblyReferences = assembly.GetReferencedAssemblies

                If LoadMaxInfos = True Then
                    ManifestResourceNames = assembly.GetManifestResourceNames

                    For Each r In ManifestResourceNames
                        Dim resourceStream As Stream = assembly.GetManifestResourceStream(r)
                        If Not resourceStream Is Nothing Then
                            ManifestResourceStreams.Add(resourceStream)
                        End If
                    Next

                    TypesClass = assembly.GetTypes.Where(Function(t) t.IsClass)
                    Modules = assembly.GetModules
                End If

                Result = Data.Message.Success

            Catch ex As ReflectionTypeLoadException
                Result = Data.Message.Failed
            Catch ex As FileNotFoundException
                Result = Data.Message.Failed
            Catch ex As FileLoadException
                Result = Data.Message.Failed
            Catch ex As NotSupportedException
                Result = Data.Message.Failed
            Catch ex As BadImageFormatException
                Result = Data.Message.Failed
            Finally
                'LogLoadedAssemblies()
            End Try
        End Sub

        Public Sub GetAssemblyInfo(assembly() As Byte, ByRef AssName$, ByRef AssVersion$, ByRef IsWpfApp As Boolean, ByRef EntryPoint As MethodInfo, ByRef AssemblyReferences As AssemblyName(), ByRef ManifestResourceNames As IEnumerable(Of String), ByRef ManifestResourceStreams As List(Of Stream), ByRef TypesClass As IEnumerable(Of Type), ByRef Modules As IEnumerable(Of [Module]), ByRef Result As Data.Message, Optional ByVal LoadMaxInfos As Boolean = False) Implements IAssemblyInfos.GetAssemblyInfo
            AssemblyInfo(assembly, AssName, AssVersion, IsWpfApp, EntryPoint, AssemblyReferences, ManifestResourceNames, ManifestResourceStreams, TypesClass, Modules, Result, LoadMaxInfos)
        End Sub

#End Region

    End Class

End Namespace
