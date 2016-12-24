Imports Implementer.Core.Obfuscation
Imports Implementer.Core.Versions
Imports Implementer.Core.ManifestRequest
Imports Implementer.Core.IconChanger
Imports Implementer.Core.Packer
Imports Implementer.Core.Dependencing
Imports Implementer.Core.Obfuscation.Protection

Namespace Engine.Context
    Public NotInheritable Class TaskState

#Region " Fields "
        Public MergeReferences As DependenciesInfos
        Public Obfuscation As ObfuscationInfos
        Public Packer As PackInfos
        Public VersionInfos As Infos
        Public Manifest As ManifestInfos
        Public IconChanger As IconInfos
#End Region

#Region " Methods "
        Public Sub CleanUp()
            If Not MergeReferences Is Nothing Then MergeReferences.Dispose()
            If Not Obfuscation Is Nothing Then Obfuscation.Dispose()
            If Not Packer Is Nothing Then Packer.Dispose()
            If Not VersionInfos Is Nothing Then VersionInfos.Dispose()
            If Not Manifest Is Nothing Then Manifest.Dispose()
            If Not IconChanger Is Nothing Then IconChanger.Dispose()
        End Sub
#End Region

    End Class

End Namespace

