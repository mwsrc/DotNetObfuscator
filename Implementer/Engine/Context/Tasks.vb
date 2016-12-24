Imports Mono.Cecil
Imports Mono.Cecil.Rocks
Imports System.IO
Imports System.Resources
Imports System.Threading
Imports System.Threading.Tasks
Imports System.ComponentModel
Imports System.Windows.Forms
Imports Implementer.Engine.Context
Imports Implementer.Engine.Processing
Imports Implementer.Engine.Processing.ProcessTask
Imports Implementer.Core.Resources
Imports Implementer.Core.Dependencing
Imports Implementer.Core.Versions
Imports Implementer.Core.ManifestRequest
Imports Implementer.Core.IconChanger
Imports Implementer.Core.Packer
Imports Implementer.Core.Obfuscation
Imports Implementer.Core.Obfuscation.Builder
Imports Implementer.Core.Obfuscation.Protection
Imports Implementer.Core.Obfuscation.Exclusion
Imports Helper.UtilsHelper
Imports Helper.CecilHelper
Imports Helper.RandomizeHelper

Namespace Engine.Context

    ''' <summary>
    ''' INFO : This is the second step of the renamer library. 
    '''        You must pass one argument (parameter) when instantiating this class and calling the RenameAssembly routine.
    ''' </summary>
    Public NotInheritable Class Tasks

#Region " Fields "
        Public AssDef As AssemblyDefinition
        Public ProtectedFilePath$
        Private m_bgw As BackgroundWorker
        Private m_parameters As Parameters
        Private m_processing As ProcessTask
        Private m_resourceCompress As Compression
        Private m_framework$
        Private m_Dependencies As Dependencies

        Private m_fi As FileInfo
        Private m_i% = 0

        Private m_ProtectedPath$
        Private m_TmpProtectedPath$
        Private m_isCountedTask As Boolean
#End Region

#Region " Events "
        Public Shared Event RenamedItem As RenamedItemDelegate
#End Region

#Region " Constructor "
        ''' <summary>
        ''' INFO : Initializes a new instance of the Context.Cls_Context class which allows to add parameters such as members and types state before the task of renaming starts.
        ''' </summary>
        ''' <param name="Parameters"></param>
        Public Sub New(parameters As Parameters, Bgw As BackgroundWorker)
            m_bgw = Bgw
            m_parameters = parameters
        End Sub
#End Region

#Region " Methods "
        Public Sub PreparingTask(inputF$)
            m_bgw.ReportProgress(2, "Preparing Task ...")
            m_parameters.inputFile = inputF
            m_parameters.currentFile = m_parameters.inputFile
            m_ProtectedPath = New FileInfo(m_parameters.currentFile).DirectoryName & "\Protected"
            m_TmpProtectedPath = Functions.GetTempFolder & "\" & Randomizer.GenerateNewAlphabetic

            If Not Directory.Exists(m_ProtectedPath) Then Directory.CreateDirectory(m_ProtectedPath)
            If Not Directory.Exists(m_TmpProtectedPath) Then Directory.CreateDirectory(m_TmpProtectedPath)

            m_fi = New FileInfo(m_parameters.inputFile)
            m_parameters.outputFile = m_TmpProtectedPath & "\" & New FileInfo(m_parameters.currentFile).Name
            File.Copy(m_parameters.inputFile, m_parameters.outputFile, True)

            m_parameters.inputFile = m_parameters.outputFile
            m_processing = New ProcessTask(m_parameters.RenamingAccept)

            m_isCountedTask = False
        End Sub

        ''' <summary>
        ''' INFO : Raise event when a type or a member renamed.
        ''' </summary>
        ''' <param name="it"></param>
        Public Shared Sub RaiseRenamedItemEvent(it As RenamedItem)
            Dim itemEvent As New RenamedItemEventArgs(it)
            RaiseEvent RenamedItem(Nothing, itemEvent)
            itemEvent = Nothing
        End Sub

        ''' <summary>
        ''' INFO : this routine reads the assembly. It uses Mono Cecil library.
        ''' </summary>
        Private Sub ReadAssembly()
            AssDef = AssemblyDefinition.ReadAssembly(m_parameters.inputFile)
            m_framework = Finder.frameworkVersion(AssDef)
            m_i += 1
            m_fi = New FileInfo(m_parameters.inputFile)
            m_parameters.outputFile = m_parameters.inputFile.Replace(m_fi.Name, m_i.ToString & ".exe")
            m_parameters.inputFile = m_parameters.outputFile
            m_isCountedTask = True
        End Sub

        Public Sub EmptyTemp()
            Functions.DeleteFiles(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\Temp")
        End Sub

        Public Function HasObfuscationTask() As Boolean
            With m_parameters.TaskAccept.Obfuscation
                If .Enabled AndAlso _
              (.EncryptNumeric OrElse _
              .EncryptBoolean OrElse _
              .EncryptString OrElse _
              .AntiTamper OrElse _
              .AntiDebug OrElse _
              .AntiDumper OrElse _
              .AntiIlDasm OrElse _
              .HidePublicCalls OrElse _
              .CompressResources OrElse _
              .RenameAssembly OrElse _
              .InvalidOpcodes OrElse _
              .InvalidMetadata) Then
                    Return True
                End If
            End With
            Return False
        End Function

        Private Function HasRenameTask()
            With m_parameters
                If .TaskAccept.Obfuscation.Enabled AndAlso _
                (.RenamingAccept.Namespaces OrElse _
                .RenamingAccept.Types OrElse _
                .RenamingAccept.Methods OrElse _
                .RenamingAccept.Fields OrElse _
                .RenamingAccept.Events OrElse _
                .RenamingAccept.Properties OrElse _
                .TaskAccept.Obfuscation.RenameResourcesContent OrElse _
                .RenamingAccept.CustomAttributes) Then
                    Return True
                End If
            End With
            Return False
        End Function

        Public Function HasPackerTask() As Boolean
            Return m_parameters.TaskAccept.Packer.Enabled
        End Function

        Public Function CheckDependencies() As AnalysisResult
            m_bgw.ReportProgress(8, "Dependencies analysis ...")
            m_Dependencies = New Dependencies(m_parameters.inputFile, m_parameters.TaskAccept.MergeReferences.Dependencies)
            Return m_Dependencies.Analyze()
        End Function

        Public Sub DependenciesTask()
            With m_parameters.TaskAccept
                If .MergeReferences.Enabled Then
                    If .MergeReferences.Dependencies.Count <> 0 Then
                        If .MergeReferences.DependenciesMode = DependenciesInfos.DependenciesAddMode.Merged Then
                            m_bgw.ReportProgress(16, "Dependencies merging ...")
                            m_Dependencies.Merge()
                        ElseIf .MergeReferences.DependenciesMode = DependenciesInfos.DependenciesAddMode.Embedded Then
                            If .Packer.Enabled = False Then DependenciesEmbedded()
                        End If
                    End If
                End If
            End With
        End Sub

        Private Function DependenciesEmbedded(Optional ByVal Pack As Boolean = False) As Embedding
            With m_parameters.TaskAccept.MergeReferences
                If .Enabled AndAlso .Dependencies.Count <> 0 AndAlso .DependenciesMode = DependenciesInfos.DependenciesAddMode.Embedded Then
                    m_bgw.ReportProgress(16, "Dependencies embedding ...")
                    ReadAssembly()
                    Dim em As New Embedding(AssDef, .Dependencies, m_framework, .DependenciesCompressEncryptMode, Pack)
                    With em
                        .CreateResolverClass()
                        .InjectFiles()
                    End With
                    WriteAssembly()
                    Return em
                End If
            End With
            Return Nothing
        End Function

        Public Sub ManifestTask()
            If m_parameters.TaskAccept.Manifest.Modified Then
                m_bgw.ReportProgress(20, "Requested Level patching ...")
                m_processing.ProcessManifest(m_parameters.inputFile, m_parameters.TaskAccept.Manifest.NewRequested)
            End If
        End Sub

        Public Sub VersionInfosTask()
            If m_parameters.TaskAccept.VersionInfos.Enabled Then
                m_bgw.ReportProgress(25, "Version Infos patching ...")
                m_processing.ProcessVersionInfos(m_parameters.inputFile, m_framework, m_parameters.TaskAccept.VersionInfos)
            End If
        End Sub

        Public Sub IconChangerTask()
            If m_parameters.TaskAccept.IconChanger.Enabled Then
                m_bgw.ReportProgress(28, "Icon Changing ...")
                m_processing.ProcessIconChanger(m_parameters.inputFile, m_framework, m_parameters.TaskAccept.IconChanger.NewIcon, m_parameters.TaskAccept.VersionInfos)
            End If
        End Sub

        Public Sub ObfuscationTask()
            With m_parameters.TaskAccept
                If .Obfuscation.Enabled Then
                    If .Obfuscation.RenameResourcesContent AndAlso .Packer.Enabled = False Then RenameResourcesContent()

                    If .Obfuscation.EncryptNumeric Then EncryptNumeric()
                    If .Obfuscation.EncryptBoolean Then EncryptBoolean()

                    If .Obfuscation.AntiTamper AndAlso .Packer.Enabled = False Then PreAntiTamper()
                    If .Obfuscation.AntiDebug AndAlso .Packer.Enabled = False Then AntiDebug()
                    If .Obfuscation.AntiDumper AndAlso .Packer.Enabled = False Then AntiDumper()
                    If .Obfuscation.AntiIlDasm AndAlso .Packer.Enabled = False Then AntiIlDasm()

                    If .Obfuscation.CompressResources OrElse .Obfuscation.EncryptResources Then PreCompressResources()

                    If .Obfuscation.HidePublicCalls AndAlso .Obfuscation.InvalidMetadata = False Then HidePinvokeCalls()
                    If .Obfuscation.EncryptString Then EncryptString()
                    If .Obfuscation.HidePublicCalls Then MildCalls()
                    If .Obfuscation.EncryptNumeric Then HideConstants()

                    If HasRenameTask() Then RenameAssembly()
                    If .Obfuscation.CompressResources OrElse .Obfuscation.EncryptResources Then PostCompressResources()

                    If .Obfuscation.InvalidOpcodes Then InvalidOpcodes()
                    If HasObfuscationTask() Then InjectDnpWatermark()

                    If .Obfuscation.InvalidMetadata AndAlso .Packer.Enabled = False Then InvalidMetadata()
                    If .Obfuscation.AntiTamper AndAlso .Packer.Enabled = False Then PostAntiTamper()
                End If
            End With

        End Sub

        Public Sub PackerTask()
            With m_parameters.TaskAccept
                If .Packer.Enabled Then

                    PreparingPacking()
                    CreateStub()

                    DependenciesEmbedded(True)

                    If .Obfuscation.Enabled AndAlso .Obfuscation.EncryptNumeric Then EncryptNumeric(True)
                    If .Obfuscation.Enabled AndAlso .Obfuscation.EncryptBoolean Then EncryptBoolean(True)

                    If .Obfuscation.Enabled AndAlso .Obfuscation.AntiTamper Then PreAntiTamper(True)
                    If .Obfuscation.Enabled AndAlso .Obfuscation.AntiDebug Then AntiDebug(True)
                    If .Obfuscation.Enabled AndAlso .Obfuscation.AntiDumper Then AntiDumper(True)
                    If .Obfuscation.Enabled AndAlso .Obfuscation.AntiIlDasm Then AntiIlDasm(True)

                    PreCompressResolver()

                    If .Obfuscation.Enabled AndAlso .Obfuscation.HidePublicCalls AndAlso .Obfuscation.InvalidMetadata = False Then HidePinvokeCalls(True)
                    If .Obfuscation.Enabled AndAlso .Obfuscation.EncryptString Then EncryptString(True)
                    If .Obfuscation.Enabled AndAlso .Obfuscation.HidePublicCalls Then MildCalls(True)
                    If .Obfuscation.Enabled AndAlso .Obfuscation.EncryptNumeric Then HideConstants(True)

                    If HasRenameTask() Then RenameAssembly(True)

                    PostCompressResolver()

                    If .Obfuscation.Enabled AndAlso .Obfuscation.InvalidOpcodes Then InvalidOpcodes(True)
                    If HasPackerTask() Then InjectDnpWatermark(True)

                    If .Obfuscation.Enabled AndAlso .Obfuscation.InvalidMetadata Then InvalidMetadata(True)
                    If .Obfuscation.Enabled AndAlso .Obfuscation.AntiTamper Then PostAntiTamper(True)
                End If
            End With

        End Sub

        Private Sub PreparingPacking()
            m_bgw.ReportProgress(10, "Packing (Preparing ...)")
            m_i += 1
            m_fi = New FileInfo(m_parameters.inputFile)
            Dim AssDef = AssemblyDefinition.ReadAssembly(m_parameters.inputFile)
            m_framework = Finder.frameworkVersion(AssDef)
            m_parameters.outputFile = m_parameters.inputFile.Replace(m_fi.Name, m_i.ToString & ".exe")

            File.Copy(m_parameters.inputFile, m_parameters.outputFile, True)
            m_parameters.inputFile = m_parameters.outputFile
            m_isCountedTask = True
        End Sub

        Private Sub CreateStub()
            m_bgw.ReportProgress(13, "Packing (Creating stub ...)")
            m_processing.ProcessPacker(m_parameters.inputFile, Functions.GetTempFolder & "\SevenzipLib.dll", m_framework, m_parameters.TaskAccept.Packer, m_parameters.TaskAccept.VersionInfos)
        End Sub

        Public Sub FinalizeTask()
            m_bgw.ReportProgress(99, "Finalizing Task ...")
            Try
                If m_isCountedTask Then
                    File.Delete(m_TmpProtectedPath & "\" & New FileInfo(m_parameters.currentFile).Name)
                    My.Computer.FileSystem.RenameFile(m_parameters.outputFile, New FileInfo(m_parameters.currentFile).Name)
                    Dim files = Directory.GetFiles(m_TmpProtectedPath)
                    For Each f In files
                        If Not f = m_TmpProtectedPath & "\" & New FileInfo(m_parameters.currentFile).Name Then File.Delete(f)
                    Next
                End If

                File.Copy(m_TmpProtectedPath & "\" & New FileInfo(m_parameters.currentFile).Name, m_ProtectedPath & "\" & New FileInfo(m_parameters.currentFile).Name, True)

                ProtectedFilePath = m_ProtectedPath & "\" & New FileInfo(m_parameters.currentFile).Name

                File.Delete(m_TmpProtectedPath & "\" & New FileInfo(m_parameters.currentFile).Name)
                Directory.Delete(m_TmpProtectedPath)
            Catch ex As Exception
                'MsgBox(ex.ToString)
            End Try
        End Sub

        Private Sub InjectDnpWatermark(Optional pack As Boolean = False)
            ReadAssembly()
            m_processing.ProcessInjectWatermark(AssDef, pack)
            WriteAssembly()
        End Sub

        Private Sub RenameResourcesContent()
            m_bgw.ReportProgress(30, "Obfuscating (Resources content renaming ...)")
            ReadAssembly()
            m_processing.ProcessRenameResourcesContent(AssDef)
            WriteAssembly()
        End Sub

        Private Sub EncryptNumeric(Optional pack As Boolean = False)
            m_bgw.ReportProgress(38, If(pack, "Packing", "Obfuscating") & " (Numeric encryption part 1...)")
            ReadAssembly()
            If m_parameters.TaskAccept.Obfuscation.HidePublicCalls Then
                m_processing.ProcessEncryptNumeric(AssDef, m_framework, Source.EncryptType.ByDefault, pack)
            Else
                m_processing.ProcessEncryptNumeric(AssDef, m_framework, If(m_parameters.TaskAccept.Obfuscation.CompressResources = True OrElse m_parameters.TaskAccept.Obfuscation.EncryptResources = True, Source.EncryptType.ByDefault, Source.EncryptType.ToResources), pack)
            End If
            WriteAssembly()
        End Sub

        Private Sub EncryptBoolean(Optional pack As Boolean = False)
            m_bgw.ReportProgress(40, If(pack, "Packing", "Obfuscating") & " (Boolean encryption ...)")
            ReadAssembly()
            If m_parameters.TaskAccept.Obfuscation.HidePublicCalls Then
                m_processing.ProcessEncryptBoolean(AssDef, m_framework, Source.EncryptType.ByDefault, pack)
            Else
                m_processing.ProcessEncryptBoolean(AssDef, m_framework, If(m_parameters.TaskAccept.Obfuscation.CompressResources = True OrElse m_parameters.TaskAccept.Obfuscation.EncryptResources = True, Source.EncryptType.ByDefault, Source.EncryptType.ToResources), pack)
            End If
            WriteAssembly()
        End Sub

        Private Sub PreAntiTamper(Optional pack As Boolean = False)
            m_bgw.ReportProgress(44, If(pack, "Packing", "Obfuscating") & " (Anti-Tamper preparing ...)")
            ReadAssembly()
            m_processing.ProcessPreAntiTamper(AssDef, m_framework, pack)
            WriteAssembly()
        End Sub

        Private Sub AntiDebug(Optional pack As Boolean = False)
            m_bgw.ReportProgress(48, If(pack, "Packing", "Obfuscating") & " (Anti-Debug injecting ...)")
            ReadAssembly()
            m_processing.ProcessAntiDebug(AssDef, m_framework, pack)
            WriteAssembly()
        End Sub

        Private Sub AntiDumper(Optional pack As Boolean = False)
            m_bgw.ReportProgress(52, If(pack, "Packing", "Obfuscating") & " (Anti-Dumper injecting ...)")
            ReadAssembly()
            m_processing.ProcessAntiDumper(AssDef, pack)
            WriteAssembly()
        End Sub

        Private Sub AntiIlDasm(Optional pack As Boolean = False)
            m_bgw.ReportProgress(56, If(pack, "Packing", "Obfuscating") & " (Anti-ILDasm adding ...)")
            ReadAssembly()
            m_processing.ProcessAntiIlDasm(AssDef)
            WriteAssembly()
        End Sub

        Private Sub PreCompressResources()
            Dim str = "Obfuscating (Resources "

            If m_parameters.TaskAccept.Obfuscation.EncryptResources And m_parameters.TaskAccept.Obfuscation.CompressResources = False Then
                str &= "encrypt preparing ...)"
            ElseIf m_parameters.TaskAccept.Obfuscation.EncryptResources = False And m_parameters.TaskAccept.Obfuscation.CompressResources Then
                str &= "compress preparing ...)"
            ElseIf m_parameters.TaskAccept.Obfuscation.EncryptResources And m_parameters.TaskAccept.Obfuscation.CompressResources Then
                str &= "encrypt & compress preparing ...)"
            End If

            m_bgw.ReportProgress(60, str)
            ReadAssembly()
            m_resourceCompress = New Compression(AssDef, m_framework, m_parameters.TaskAccept.Obfuscation.EncryptResources, m_parameters.TaskAccept.Obfuscation.CompressResources, False)
            m_resourceCompress.CreateResolverClass(False)
            WriteAssembly()
        End Sub

        Private Sub PreCompressResolver()
            ReadAssembly()
            m_resourceCompress = New Compression(AssDef, m_framework, True, True, True)
            m_resourceCompress.CreateResolverClass(True)
            WriteAssembly()
        End Sub

        Private Sub HidePinvokeCalls(Optional pack As Boolean = False)
            m_bgw.ReportProgress(65, If(pack, "Packing", "Obfuscating") & If(m_parameters.TaskAccept.Obfuscation.InvalidMetadata = False, " (Hide calls part 1...)", " (Hide calls ...)"))
            ReadAssembly()
            m_processing.ProcessHidePinvokeCalls(AssDef, m_framework, pack)
            WriteAssembly()
        End Sub

        Private Sub EncryptString(Optional pack As Boolean = False)
            m_bgw.ReportProgress(70, If(pack, "Packing", "Obfuscating") & " (String encryption ...)")
            ReadAssembly()
            If m_parameters.TaskAccept.Obfuscation.HidePublicCalls Then
                m_processing.ProcessEncryptString(AssDef, m_framework, Source.EncryptType.ByDefault, pack)
            Else
                m_processing.ProcessEncryptString(AssDef, m_framework, If(m_parameters.TaskAccept.Obfuscation.CompressResources = True OrElse m_parameters.TaskAccept.Obfuscation.EncryptResources = True, Source.EncryptType.ByDefault, Source.EncryptType.ToResources), pack)
            End If
            WriteAssembly()
        End Sub

        Private Sub MildCalls(Optional pack As Boolean = False)
            m_bgw.ReportProgress(75, If(pack, "Packing", "Obfuscating") & If(m_parameters.TaskAccept.Obfuscation.InvalidMetadata = False, " (Hide calls part 2...)", " (Hide calls ...)"))
            ReadAssembly()
            m_processing.ProcessMildCalls(AssDef, m_framework, pack)
            WriteAssembly()
        End Sub

        Private Sub HideConstants(Optional pack As Boolean = False)
            m_bgw.ReportProgress(78, If(pack, "Packing", "Obfuscating") & " (Numeric encryption part 2...)")
            ReadAssembly()
            m_processing.ProcessConstants(AssDef)
            WriteAssembly()
        End Sub

        ''' <summary>
        ''' INFO : Loop through the modules and types of the loaded assembly and start renaming.
        ''' </summary>
        Private Sub RenameAssembly(Optional pack As Boolean = False)
            If m_parameters.RenamingAccept.ExcludeReflection Then
                m_bgw.ReportProgress(82, If(pack, "Packing", "Obfuscating") & " (Reflection analysis...)")
            End If

            ReadAssembly()
            Dim assemblyMainName$ = AssDef.EntryPoint.DeclaringType.Namespace

            If m_parameters.RenamingAccept.ExcludeReflection Then
                ExcludeReflection.AnalyzeCodes(AssDef, m_parameters.ExcludeList)
            End If

            m_bgw.ReportProgress(85, If(pack, "Packing", "Obfuscating") & " (Renaming assembly...)")
            For Each modul In AssDef.Modules
                If modul.HasTypes Then
                    For Each type In modul.GetAllTypes
                        If m_parameters.ExcludeList.isRenamingExclude(type) = False Then
                            RenameSelectedNamespace(type, assemblyMainName)
                        End If
                    Next
                End If
            Next
            WriteAssembly()
        End Sub

        ''' <summary>
        ''' INFO : Rename the main Namespace or all namespaces according to Cls_Parameters.RenameMainNamespaceSetting setting.
        ''' </summary>
        ''' <param name="type"></param>
        ''' <param name="assemblyMainName"></param>
        ''' <param name="processing"></param>
        Private Sub RenameSelectedNamespace(type As TypeDefinition, assemblyMainName$)
            If m_parameters.RenamingAccept.RenameMainNamespaceSetting = CBool(RenamerState.RenameMainNamespace.Only) Then
                If type.Namespace.StartsWith(assemblyMainName) Then m_processing.ProcessType(type)
            Else
                m_processing.ProcessType(type)
            End If

            If type.HasProperties Then m_processing.ProcessProperties(type)
            If type.HasMethods Then m_processing.ProcessMethods(type)
            If type.HasFields Then m_processing.ProcessFields(type)
            If type.HasEvents Then m_processing.ProcessEvents(type)
        End Sub

        Private Sub PostCompressResources()
            Dim str = "Obfuscating (Resources "

            If m_parameters.TaskAccept.Obfuscation.EncryptResources And m_parameters.TaskAccept.Obfuscation.CompressResources = False Then
                str &= "encrypt finishing ...)"
            ElseIf m_parameters.TaskAccept.Obfuscation.EncryptResources = False And m_parameters.TaskAccept.Obfuscation.CompressResources Then
                str &= "compress finishing ...)"
            ElseIf m_parameters.TaskAccept.Obfuscation.EncryptResources And m_parameters.TaskAccept.Obfuscation.CompressResources Then
                str &= "encrypt & compress finishing ...)"
            End If

            m_bgw.ReportProgress(86, str)
            ReadAssembly()
            m_resourceCompress.CompressInjectResources(AssDef)
            WriteAssembly()
        End Sub

        Private Sub PostCompressResolver()
            ReadAssembly()
            m_resourceCompress.InjectSevenzipLibrary(AssDef)
            WriteAssembly()
        End Sub

        Private Sub InvalidOpcodes(Optional pack As Boolean = False)
            m_bgw.ReportProgress(90, If(pack, "Packing", "Obfuscating") & " (ControlFlow injecting ...)")
            ReadAssembly()
            m_processing.ProcessInvalidOpcodes(AssDef)
            WriteAssembly()
        End Sub

        Private Sub InvalidMetadata(Optional ByVal Pack As Boolean = False)
            m_bgw.ReportProgress(94, If(Pack, "Packing", "Obfuscating") & " (Invalid Metadatas ...)")
            ReadAssembly()
            Dim psr As New MetadataProcessor
            m_processing.ProcessInvalidMetadata(AssDef, psr)
            WriteAssembly(psr)
        End Sub

        Private Sub PostAntiTamper(Optional pack As Boolean = False)
            m_bgw.ReportProgress(98, If(pack, "Packing", "Obfuscating") & " (Anti-Tamper finishing ...)")
            m_i += 1
            m_fi = New FileInfo(m_parameters.inputFile)
            m_parameters.outputFile = m_parameters.inputFile.Replace(m_fi.Name, m_i.ToString & ".exe")
            File.Copy(m_parameters.inputFile, m_parameters.outputFile, True)
            m_parameters.inputFile = m_parameters.outputFile
            m_isCountedTask = True
            m_processing.ProcessPostAntiTamper(m_parameters.inputFile)
        End Sub

        ''' <summary>
        ''' INFO : Records changes to the loaded assembly. It uses Mono Cecil library.
        ''' </summary>
        Private Sub WriteAssembly(Optional ByVal ma As MetadataProcessor = Nothing)
            If Not ma Is Nothing Then
                ma.Process(AssDef.MainModule, m_parameters.outputFile, New WriterParameters())
            Else
                AssDef.Write(m_parameters.outputFile)
            End If
        End Sub

        ''' <summary>
        ''' INFO : Clear the randomize names from the dictionary.
        ''' </summary>
        Public Sub Clean()
            Randomizer.CleanUp()
            Mapping.CleanUp()
            Str.CleanUp()
            Numeric.CleanUp()
            Bool.CleanUp()
            Mild.CleanUp()
            Content.Cleanup()
            Pinvoke.CleanUp()
            EmptyTemp()
            m_parameters.TaskAccept.CleanUp()
            If Not m_parameters.RenamingAccept.ExclusionRule Is Nothing Then m_parameters.RenamingAccept.ExclusionRule.CleanUp()
            If Not m_Dependencies Is Nothing Then m_Dependencies.CleanUp()
        End Sub
#End Region

    End Class

End Namespace
