Imports System.Drawing

Public Interface IReader

#Region " Methods "
    Sub ReadFile(filePath$)
    Function isExecutable() As Boolean
#End Region

#Region " Properties "
    ReadOnly Property isManagedFile() As Boolean
    ReadOnly Property GetSystemType As String
    ReadOnly Property GetTargetRuntime As String
    ReadOnly Property GetTargetPlatform As String
    ReadOnly Property GetMainIcon As Icon
    ReadOnly Property GetVersionInfos As FileVersionInfo
    ReadOnly Property HasInvalidSectionHeader As Boolean
#End Region

End Interface
