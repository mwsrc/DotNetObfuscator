Imports Mono.Cecil
Imports Helper.CecilHelper
Imports Helper.RandomizeHelper
Imports Implementer.Core.Resources
Imports Implementer.Core.Obfuscation
Imports Implementer.Core.Versions
Imports Implementer.Core.ManifestRequest
Imports Implementer.Core.Packer
Imports Implementer.Core.IconChanger
Imports Implementer.Core.Dependencing
Imports Implementer.Core.Obfuscation.Anti
Imports Implementer.Core.Obfuscation.Builder.Source
Imports Implementer.Core.Obfuscation.Protection
Imports Implementer.Engine.Context
Imports System.IO
Imports System.Resources
Imports System.Drawing

Namespace Engine.Processing

    ''' <summary>
    ''' INFO : This is the third step of the renamer library. 
    '''        This will process to rename types and members from the selected assembly with settings of your choice.
    ''' </summary>
    Public NotInheritable Class ProcessTask

#Region " Fields "
        Private m_RenamingAccept As RenamerState
#End Region

#Region " Constructor "
        ''' <summary>
        ''' INFO : Initializes a new instance of the Processing.Cls_Processing class from which is started the task of renaming.
        ''' </summary>
        ''' <param name="RenamingAccept"></param>
        Public Sub New(RenamingAccept As RenamerState)
            m_RenamingAccept = RenamingAccept
            RandomizerType.RenameSetting = m_RenamingAccept.RenamingType
        End Sub
#End Region

#Region " Methods "

        ''' <summary>
        ''' INFO : This is the EntryPoint of the renamer method ! Namespaces, Types and Resources renaming.
        ''' </summary>
        ''' <param name="type"></param>
        Public Sub ProcessType(type As TypeDefinition)

            Dim NamespaceOriginal$ = type.Namespace
            Dim NamespaceObfuscated$ = type.Namespace

            If Not type.Name = "<Module>" Then
                If m_RenamingAccept.Namespaces Then
                    NamespaceObfuscated = If(CBool(m_RenamingAccept.ReplaceNamespacesSetting) = True, String.Empty, Randomizer.GenerateNew())
                    type.Namespace = Mapping.RenameTypeDef(type, NamespaceObfuscated, True)
                End If
            End If

            If NameChecker.IsRenamable(type) Then

                Dim TypeOriginal$ = type.Name
                Dim TypeObfuscated$ = type.Name

                If m_RenamingAccept.CustomAttributes Then Renamer.RenameCustomAttributesValues(type)

                If m_RenamingAccept.Types Then
                    type.Name = Mapping.RenameTypeDef(type, Randomizer.GenerateNew())
                    TypeObfuscated = type.Name
                    Renamer.RenameResources(type, NamespaceOriginal, NamespaceObfuscated, TypeOriginal, TypeObfuscated)
                End If

                If m_RenamingAccept.Namespaces Then
                    type.Namespace = Mapping.RenameTypeDef(type, NamespaceObfuscated, True)
                    Renamer.RenameResources(type, NamespaceOriginal, NamespaceObfuscated, TypeOriginal, TypeObfuscated)
                End If

                If m_RenamingAccept.Properties Then Renamer.RenameResourceManager(type)

                If m_RenamingAccept.Types OrElse m_RenamingAccept.Namespaces Then
                    Renamer.RenameInitializeComponentsValues(type, TypeOriginal, TypeObfuscated, False)
                End If
            End If
        End Sub

        Public Sub ProcessMildCalls(AssDef As AssemblyDefinition, frmwk$, pack As Boolean)
            Mild.DoJob(AssDef, frmwk, m_RenamingAccept.ExclusionRule, pack)
        End Sub

        Public Sub ProcessHidePinvokeCalls(AssDef As AssemblyDefinition, frmwk$, pack As Boolean)
            Pinvoke.DoJob(AssDef, frmwk, m_RenamingAccept.ExclusionRule, pack)
        End Sub

        Public Sub ProcessConstants(AssDef As AssemblyDefinition)
            Constants.DoJob(AssDef, m_RenamingAccept.ExclusionRule)
        End Sub

        Public Sub ProcessEncryptString(AssDef As AssemblyDefinition, frmwk$, EncryptToResources As EncryptType, pack As Boolean)
            Dim emptyNamespaces = If(m_RenamingAccept.ReplaceNamespacesSetting = RenamerState.ReplaceNamespaces.Empty, True, False)
            Str.DoJob(AssDef, frmwk, If(pack = True, EncryptType.ByDefault, EncryptToResources), m_RenamingAccept.ExclusionRule, emptyNamespaces)
        End Sub

        Public Sub ProcessEncryptBoolean(AssDef As AssemblyDefinition, frmwk$, EncryptToResources As EncryptType, pack As Boolean)
            Bool.DoJob(AssDef, frmwk, If(pack = True, EncryptType.ByDefault, EncryptToResources), m_RenamingAccept.ExclusionRule, pack)
        End Sub

        Public Sub ProcessEncryptNumeric(AssDef As AssemblyDefinition, frmwk$, EncryptToResources As EncryptType, pack As Boolean)
            Numeric.DoJob(AssDef, frmwk, If(pack = True, EncryptType.ByDefault, EncryptToResources), m_RenamingAccept.ExclusionRule, pack)
        End Sub

        Public Sub ProcessRenameResourcesContent(AssDef As AssemblyDefinition)
            Content.Rename(AssDef)
        End Sub

        Public Sub ProcessAntiDebug(AssDef As AssemblyDefinition, Frmwk$, pack As Boolean)
            AntiDebug.InjectAntiDebug(AssDef, Frmwk, pack)
        End Sub

        Public Sub ProcessAntiDumper(AssDef As AssemblyDefinition, pack As Boolean)
            AntiDumper.CreateAntiDumperClass(AssDef, pack)
        End Sub

        Public Sub ProcessPreAntiTamper(AssDef As AssemblyDefinition, Frmwk$, Pack As Boolean)
            AntiTamper.CreateAntiTamperClass(AssDef, Frmwk, Pack)
        End Sub

        Public Sub ProcessPostAntiTamper(FilePath$)
            AntiTamper.InjectMD5(FilePath)
        End Sub

        Public Sub ProcessAntiIlDasm(AssDef As AssemblyDefinition)
            AntiIlDasm.Inject(AssDef)
        End Sub

        Public Sub ProcessInvalidOpcodes(AssDef As AssemblyDefinition)
            invalidOpcodes.Inject(AssDef, m_RenamingAccept.ExclusionRule)
        End Sub

        Public Sub ProcessVersionInfos(FilePath$, Frmwk$, vInfos As Infos)
            Injector.InjectAssemblyVersionInfos(FilePath, vInfos)
        End Sub

        Public Sub ProcessManifest(FilePath$, CurrentRequested As String)
            ManifestWriter.ApplyManifest(FilePath, CurrentRequested)
        End Sub

        Public Sub ProcessPacker(FilePath$, SevenZipResName$, Frmwk$, pInfos As PackInfos, vInfos As Infos)
            Dim pack As New Pack(FilePath)
            With pack
                .CreateStub(Frmwk, SevenZipResName)
                .ReplaceIcon(pInfos.NewIcon)
                .InjectAssemblyVersionInfos(vInfos)
                .InjectManifest(pInfos.RequestedLevel)
            End With
        End Sub

        Public Sub ProcessIconChanger(FilePath$, Frmwk$, NewIconPath As Icon, vInfos As Infos)
            Replacer.ReplaceFromIcon(FilePath, NewIconPath)
            Injector.InjectAssemblyVersionInfos(FilePath, vInfos)
        End Sub

        Public Sub ProcessInjectWatermark(AssDef As AssemblyDefinition, ByVal Pack As Boolean)
            Attribut.DoInjection(AssDef, Pack)
        End Sub

        Public Sub ProcessInvalidMetadata(AssDef As AssemblyDefinition, psr As MetadataProcessor)
            InvalidMetadata.DoJob(AssDef, psr)
        End Sub

        ''' <summary>
        ''' INFO : Methods, Parameters and Variables renamer routine.
        ''' </summary>
        ''' <param name="type"></param>
        Public Sub ProcessMethods(type As TypeDefinition)
            If m_RenamingAccept.Methods OrElse m_RenamingAccept.Parameters Then
                For Each method As MethodDefinition In type.Methods
                    If NameChecker.IsRenamable(method) Then
                        If Not Finder.AccessorMethods(type).Contains(method) Then ProcessMethod(method, "Methods")
                    Else
                        ProcessParameters(method)
                    End If
                Next
            End If
        End Sub

        ''' <summary>
        ''' INFO : Properties, CustomAttributes (Only "AccessedThroughPropertyAttribute" attribute) renamer routine. 
        ''' </summary>
        ''' <param name="type"></param>
        Public Sub ProcessProperties(type As TypeDefinition)
            If m_RenamingAccept.Properties OrElse m_RenamingAccept.CustomAttributes Then
                For Each propDef As PropertyDefinition In type.Properties
                    If NameChecker.IsRenamable(propDef) Then

                        Dim originalN = propDef.Name
                        Dim obfuscatedN = propDef.Name

                        If m_RenamingAccept.Properties Then
                            obfuscatedN = Randomizer.GenerateNew()
                        End If

                        Renamer.RenameProperty(propDef, obfuscatedN)
                        Renamer.RenameInitializeComponentsValues(propDef.DeclaringType, originalN, obfuscatedN, True)
                        Renamer.RenameSettings(propDef.GetMethod, originalN, obfuscatedN)
                        Renamer.RenameSettings(propDef.SetMethod, originalN, obfuscatedN)

                        If m_RenamingAccept.CustomAttributes Then
                            Renamer.RenameCustomAttributes(type, propDef, originalN, obfuscatedN)
                        End If

                        If m_RenamingAccept.Methods Then
                            Dim flag = "Property"
                            If Not propDef.GetMethod Is Nothing Then ProcessMethod(propDef.GetMethod, flag)
                            If Not propDef.SetMethod Is Nothing Then ProcessMethod(propDef.SetMethod, flag)
                            For Each def In propDef.OtherMethods
                                ProcessMethod(def, flag)
                            Next
                        End If
                    End If
                Next
            End If
        End Sub

        ''' <summary>
        ''' INFO : Fields renamer routine. 
        ''' </summary>
        ''' <param name="type"></param>
        Public Sub ProcessFields(type As TypeDefinition)
            If m_RenamingAccept.Fields Then
                For Each field As FieldDefinition In type.Fields
                    If NameChecker.IsRenamable(field) Then Renamer.RenameField(field, Randomizer.GenerateNew())
                Next
            End If
        End Sub

        ''' <summary>
        ''' INFO : Events renamer routine. 
        ''' </summary>
        ''' <param name="type"></param>
        Public Sub ProcessEvents(type As TypeDefinition)
            If m_RenamingAccept.Events Then
                For Each events As EventDefinition In type.Events
                    If NameChecker.IsRenamable(events) Then
                        If m_RenamingAccept.CustomAttributes Then Renamer.RenameCustomAttributesValues(events)
                        If m_RenamingAccept.Events Then Renamer.RenameEvent(events, Randomizer.GenerateNew())

                        Dim flag = "Event"
                        If Not events.AddMethod Is Nothing Then ProcessMethod(events.AddMethod, flag)
                        If Not events.RemoveMethod Is Nothing Then ProcessMethod(events.RemoveMethod, flag)
                        For Each def In events.OtherMethods
                            ProcessMethod(def, flag)
                        Next
                    End If
                Next
            End If
        End Sub

        Private Sub ProcessMethod(mDef As MethodDefinition, DestNodeName$)
            Dim meth As MethodDefinition = mDef
            If m_RenamingAccept.Methods Then
                If DestNodeName = "Event" Then
                    meth = Renamer.RenameMethod(meth.DeclaringType, meth)
                Else
                    If NameChecker.IsRenamable(meth) Then
                        meth = Renamer.RenameMethod(meth.DeclaringType, meth)
                    End If
                End If
            End If
            ProcessParameters(meth)
        End Sub

        Private Sub ProcessParameters(Meth As MethodDefinition)
            If m_RenamingAccept.Parameters Then
                Renamer.RenameParameters(Meth)
            End If
            If m_RenamingAccept.Variables Then Renamer.RenameVariables(Meth)
        End Sub
#End Region

    End Class
End Namespace