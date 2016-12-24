Imports System.Reflection
Imports System.IO

Namespace AssemblyHelper
    Public Interface IAssemblyInfos
        Sub GetAssemblyInfo(assembly() As Byte, ByRef AssName$, ByRef AssVersion$, ByRef IsWpfApp As Boolean, ByRef EntryPoint As MethodInfo, ByRef AssemblyReferences As AssemblyName(), ByRef ManifestResourceNames As IEnumerable(Of String), ByRef ManifestResourceStreams As List(Of Stream), ByRef TypesClass As IEnumerable(Of Type), ByRef Modules As IEnumerable(Of [Module]), ByRef Result As Data.Message, Optional ByVal LoadMaxInfos As Boolean = False)
    End Interface

End Namespace

