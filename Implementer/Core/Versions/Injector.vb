Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.CodeDom.Compiler
Imports System.IO
Imports Helper.RandomizeHelper
Imports Helper.UtilsHelper
Imports Mono.Cecil
Imports System.Reflection
Imports Helper.CecilHelper
Imports Vestris.ResourceLib

Namespace Core.Versions
    Friend NotInheritable Class Injector

#Region " Fields "
        Private Shared langId As Integer()
#End Region

#Region " Constructor "
        Shared Sub New()
            langId = New Integer(1) {0, 1033}
        End Sub
#End Region

#Region " Methods "
        Private Shared Sub InjectAssemblyInfos(SelectedFilePath$, vInfos As Infos)
            Dim AssDefTarget = AssemblyDefinition.ReadAssembly(SelectedFilePath)
            Dim AttribInfos As New Dictionary(Of Type, String) From { _
                                                                {GetType(AssemblyTitleAttribute), vInfos.FileDescription}, _
                                                                {GetType(AssemblyDescriptionAttribute), vInfos.Comments}, _
                                                                {GetType(AssemblyCompanyAttribute), vInfos.CompanyName}, _
                                                                {GetType(AssemblyProductAttribute), vInfos.ProductName}, _
                                                                {GetType(AssemblyCopyrightAttribute), vInfos.LegalCopyright}, _
                                                                {GetType(AssemblyTrademarkAttribute), vInfos.LegalTrademarks}, _
                                                                {GetType(AssemblyFileVersionAttribute), vInfos.FileVersion}, _
                                                                {GetType(AssemblyVersionAttribute), vInfos.ProductVersion}}

            For Each info In AttribInfos
                Utils.RemoveCustomAttributeByName(AssDefTarget, info.Key.Name)
                Injecter.InjectAssemblyInfoCustomAttribute(AssDefTarget, info.Key, info.Value)
            Next

            AssDefTarget.Write(SelectedFilePath)
        End Sub

        Private Shared Sub InjectVersionInfos(ByVal SelectedFilePath$, ByVal vInfos As Infos)

            DeleteVersionFromLangId(SelectedFilePath)

            Dim versionResource As New VersionResource()
            With versionResource
                .FileVersion = vInfos.FileVersion
                .ProductVersion = vInfos.ProductVersion

                Dim stringFileInfo As New StringFileInfo()
                versionResource(stringFileInfo.Key) = stringFileInfo

                Dim stringFileInfoStrings As New StringTable()
                stringFileInfoStrings.LanguageID = 1033
                stringFileInfoStrings.CodePage = 1252
                stringFileInfo.Strings.Add(stringFileInfoStrings.Key, stringFileInfoStrings)
                stringFileInfoStrings("ProductName") = vInfos.ProductName
                stringFileInfoStrings("FileDescription") = vInfos.FileDescription
                stringFileInfoStrings("CompanyName") = vInfos.CompanyName
                stringFileInfoStrings("LegalCopyright") = vInfos.LegalCopyright
                stringFileInfoStrings("LegalTrademarks") = vInfos.LegalTrademarks
                stringFileInfoStrings("Comments") = vInfos.Comments
                stringFileInfoStrings("FileVersion") = versionResource.FileVersion
                stringFileInfoStrings("ProductVersion") = versionResource.ProductVersion

                Dim varFileInfo As New VarFileInfo()
                versionResource(varFileInfo.Key) = varFileInfo
                Dim varFileInfoTranslation As New VarTable("Translation")
                varFileInfo.Vars.Add(varFileInfoTranslation.Key, varFileInfoTranslation)
                varFileInfoTranslation(ResourceUtil.NEUTRALLANGID) = 1252

                .SaveTo(SelectedFilePath)
            End With

        End Sub

        Friend Shared Sub InjectAssemblyVersionInfos(SelectedFilePath$, vInfos As Infos)
            InjectVersionInfos(SelectedFilePath, vInfos)
            InjectAssemblyInfos(SelectedFilePath, vInfos)
        End Sub

        Private Shared Sub DeleteVersionFromLangId(SelectedFilePath$)
            Dim OldversionResource As New VersionResource()
            For Each id In langId
                Try
                    With OldversionResource
                        .Language = id
                        .LoadFrom(SelectedFilePath)
                        .DeleteFrom(SelectedFilePath)
                    End With
                Catch ex As Exception
                End Try
            Next
        End Sub
#End Region

    End Class
End Namespace