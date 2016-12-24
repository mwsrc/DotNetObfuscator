Imports System.Text
Imports System.CodeDom.Compiler
Imports Mono.Cecil
Imports Mono.Cecil.Cil
Imports System.ComponentModel
Imports Helper.RandomizeHelper
Imports Helper.CryptoHelper
Imports System.Resources
Imports System.IO
Imports Helper.AssemblyHelper
Imports Helper.CecilHelper
Imports Helper.CodeDomHelper
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports Implementer.Core.Packer

Namespace Core.Obfuscation.Builder
    Public Class Source

#Region " Enumerations "
        Public Enum EncryptType
            ByDefault = 0
            ToResources = 1
        End Enum
#End Region

#Region " Fields "
        Protected Shared AssemblyDef As AssemblyDefinition = Nothing
        Protected Shared Pack As Boolean
        Protected Shared Frmwk As String = String.Empty
        Protected Shared rand As Random
        Protected Shared EncryptToResources As EncryptType

        Protected Shared completedInstructions As Mono.Collections.Generic.Collection(Of Instruction)
        Protected Shared completedMethods As Mono.Collections.Generic.Collection(Of MethodDefinition)

        Protected Shared ResName As String = String.Empty
        Protected Shared ResWriter As ResourceWriter = Nothing

        Private Shared m_AddedNamespaceStart = String.Empty
        Private Shared m_AddedNamespaceEnd = String.Empty
#End Region

#Region " Constructor "

        Shared Sub New()
            completedMethods = New Mono.Collections.Generic.Collection(Of MethodDefinition)
            completedInstructions = New Mono.Collections.Generic.Collection(Of Instruction)
            rand = New Random
        End Sub

#End Region

#Region " Methods "

        Private Shared Sub LoadNamespacesHeaders()
            Dim NamespaceDefault = Finder.FindDefaultNamespace(AssemblyDef)

            m_AddedNamespaceStart = "Namespace " & NamespaceDefault
            m_AddedNamespaceEnd = "End Namespace"

            If NamespaceDefault = String.Empty Then
                m_AddedNamespaceStart = String.Empty
                m_AddedNamespaceEnd = String.Empty
            End If

            If Pack Then
                m_AddedNamespaceStart = String.Empty
                m_AddedNamespaceEnd = String.Empty
            End If
        End Sub

        Protected Shared Function isValidOperand(instruct As Instruction) As Boolean
            If Not instruct.Operand Is Nothing Then
                Return True
            End If
            Return False
        End Function

        Protected Shared Function isValidIntegerOperand(instruct As Instruction) As Boolean
            If Not instruct.Operand Is Nothing AndAlso Not Integer.Parse(instruct.Operand) = Nothing Then
                Return True
            End If
            Return False
        End Function

        Protected Shared Function DecryptIntStub(ClassName$, DecryptIntFuncName$) As String
            LoadNamespacesHeaders()
            Dim str = _
                "Imports System" & vbNewLine & _
                "Imports Microsoft.VisualBasic" & vbNewLine _
                        & Loader.GenerateInfos(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, "1.0.0.0") & vbNewLine _
                        & m_AddedNamespaceStart & vbNewLine _
                        & Generator.GenerateDecryptIntFunc(ClassName, DecryptIntFuncName) & vbNewLine _
                        & m_AddedNamespaceEnd
            Return Compiler.CreateStubFromString(ClassName, Frmwk, str)
        End Function

        Protected Shared Function DecryptOddStub(ClassName$, DecryptOddFuncName$) As String
            LoadNamespacesHeaders()
            Dim str = _
                "Imports System" & vbNewLine & _
                "Imports Microsoft.VisualBasic" & vbNewLine _
                      & Loader.GenerateInfos(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, "1.0.0.0") & vbNewLine _
                      & m_AddedNamespaceStart & vbNewLine _
                      & Generator.GenerateDecryptOddFunc(ClassName, DecryptOddFuncName) & vbNewLine _
                      & m_AddedNamespaceEnd
            Return Compiler.CreateStubFromString(ClassName, Frmwk, str)
        End Function

        Protected Shared Function DecryptXorStub(ClassName$, DecryptXorFuncName$) As String
            LoadNamespacesHeaders()
            Dim str = "Imports System" & vbNewLine _
                      & Loader.GenerateInfos(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, "1.0.0.0") & vbNewLine _
                      & m_AddedNamespaceStart & vbNewLine _
                      & Generator.GenerateDecryptXorFunc(ClassName, DecryptXorFuncName) & vbNewLine _
                      & m_AddedNamespaceEnd
            Return Compiler.CreateStubFromString(ClassName, Frmwk, str)
        End Function

        Protected Shared Function DecryptXorType(ClassName$, DecryptXorFuncName$) As Type
            LoadNamespacesHeaders()
            Dim str = "Imports System" & vbNewLine _
                      & Loader.GenerateInfos(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, "1.0.0.0") & vbNewLine _
                      & Generator.GenerateDecryptXorFunc(ClassName, DecryptXorFuncName)
            Return Compiler.CreateTypeFromString(ClassName, Frmwk, str)
        End Function

        Protected Shared Function ReadFromResourcesStub(ClassName$, ReadFromResourcesFuncName$) As String
            LoadNamespacesHeaders()
            Dim str = _
                "Imports System" & vbNewLine & _
                "Imports Microsoft.VisualBasic" & vbNewLine & _
                "Imports System.Resources" & vbNewLine _
                          & Loader.GenerateInfos(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, "1.0.0.0") & vbNewLine _
                          & m_AddedNamespaceStart & vbNewLine _
                          & Generator.GenerateReadFromResourcesFunc(ClassName, ReadFromResourcesFuncName, ResName) & vbNewLine _
                          & m_AddedNamespaceEnd
            Return Compiler.CreateStubFromString(ClassName, Frmwk, str)
        End Function

        Protected Shared Function ReadStringFromResourcesStub(ClassName$, ResourceDecryptFunc$, Decompress0$, Decompress1$) As String
            LoadNamespacesHeaders()
            Dim ms$ = Randomizer.GenerateNewAlphabetic

            Dim str = _
                "Imports System.Windows.Forms" & vbNewLine & _
                "Imports System.Collections.Generic" & vbNewLine & _
                "Imports System" & vbNewLine & _
                "Imports System.IO" & vbNewLine & _
                "Imports System.IO.Compression" & vbNewLine & vbNewLine _
                       & Loader.GenerateInfos(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, "1.0.0.0") & vbNewLine _
                       & "Public Class " & ClassName & vbNewLine _
                       & "    Private Shared " & ms & " As Stream " & vbNewLine _
                       & "    Public Shared Function " & ResourceDecryptFunc & " (ByVal p As Integer) As String" & vbNewLine _
                       & "        Dim br As New BinaryReader(" & ms & ")" & vbNewLine _
                       & "        br.BaseStream.Position = p" & vbNewLine _
                       & "        Return br.ReadString" & vbNewLine _
                       & "    End Function" & vbNewLine _
                       & "    Shared Sub New" & vbNewLine _
                       & "        If " & ms & " Is Nothing Then" & vbNewLine _
                       & "           Dim b as Byte()" & vbNewLine _
                       & "           b = " & Decompress0 & "(Assembly.GetExecutingAssembly.GetManifestResourceStream(""" & ResName & """))" & vbNewLine _
                       & "           " & ms & " = New MemoryStream(b)" & vbNewLine _
                       & "        End If" & vbNewLine _
                       & "    End Sub" & vbNewLine _
                       & Generator.GenerateDeCompressWithGzipStreamFunc(Decompress0, Decompress1) & vbNewLine _
                       & "End Class"
            Return Compiler.CreateStubFromString(ClassName, Frmwk, str)
        End Function

        Protected Shared Sub InjectResource()
            Dim CompressRes As EmbeddedResource = New EmbeddedResource(ResName & ".resources", ManifestResourceAttributes.Private, File.ReadAllBytes(My.Application.Info.DirectoryPath & "\" & ResName & ".resources"))
            AssemblyDef.MainModule.Resources.Add(CompressRes)
        End Sub

        Protected Shared Function DecryptPrimeStub(className$, DecryptPrimeFuncName$) As String
            LoadNamespacesHeaders()
            Dim str = _
                "Imports System.Collections.Generic" & vbNewLine & _
                "Imports System" & vbNewLine & _
                      Loader.GenerateInfos(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, "1.0.0.0") & vbNewLine _
                    & m_AddedNamespaceStart & vbNewLine _
                    & "Public Class " & className & vbNewLine _
                    & Generator.GenereateDecryptPrimeFunc(DecryptPrimeFuncName) & vbNewLine _
                    & "End Class" & vbNewLine & vbNewLine _
                    & m_AddedNamespaceEnd

            Return Compiler.CreateStubFromString(className, Frmwk, str)
        End Function

        Protected Shared Function DecryptRPNStub(ClassName$, DecryptRPNFuncName1$, DecryptRPNFuncName2$) As String
            LoadNamespacesHeaders()
            Dim str = _
                "Imports System.Collections.Generic" & vbNewLine & _
                "Imports System" & vbNewLine & _
                      Loader.GenerateInfos(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, "1.0.0.0") & vbNewLine _
                    & m_AddedNamespaceStart & vbNewLine _
                    & "Public Class " & ClassName & vbNewLine _
                    & Generator.GenerateDecryptRPNFunc(DecryptRPNFuncName1, DecryptRPNFuncName2) & vbNewLine _
                    & "End Class" & vbNewLine & vbNewLine _
                    & m_AddedNamespaceEnd

            Return Compiler.CreateStubFromString(ClassName, Frmwk, str)
        End Function

        Protected Shared Function FromBase64Stub(ClassName$, Base64FuncName$, GetStringFuncName$) As String
            LoadNamespacesHeaders()
            Dim str = _
                "Imports System" & vbNewLine & _
                "Imports System.Text" & vbNewLine _
                      & Loader.GenerateInfos(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, "1.0.0.0") & vbNewLine _
                      & m_AddedNamespaceStart & vbNewLine _
                      & Generator.GenerateFromBase64Func(ClassName, Base64FuncName, GetStringFuncName) & vbNewLine _
                      & m_AddedNamespaceEnd
            Return Compiler.CreateStubFromString(ClassName, Frmwk, str)
        End Function

        Protected Shared Function SevenZipStub(ClassName$, initializeFuncName$, resolverName$, Decompress0$, Decompress1$, encrypt As Boolean, compress As Boolean) As String
            Dim reverseStr = If(encrypt = True, "                    Array.Reverse(d)", String.Empty)
            Dim DecompressStr0 = "                d = " & Decompress0 & "(cm.ToArray)"

            Dim str = _
                "Imports System.Windows.Forms" & vbNewLine & _
                "Imports System.Collections.Generic" & vbNewLine & _
                "Imports System" & vbNewLine & _
                "Imports System.IO" & vbNewLine & _
                "Imports System.IO.Compression" & vbNewLine & vbNewLine _
                       & Loader.GenerateInfos(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, "1.0.0.0") & vbNewLine _
                       & "Public Class " & ClassName & vbNewLine _
                       & "    Private Shared Function " & resolverName & " (ByVal sender As Object, ByVal args As ResolveEventArgs) As Assembly" & vbNewLine _
                       & "        Dim names As String() = Nothing" & vbNewLine _
                       & "        Dim ass As Assembly = Nothing" & vbNewLine _
                       & "        Dim d as Byte()" & vbNewLine _
                       & "        Using cs As Stream = (GetType(System.Reflection.Assembly).GetMethod(""GetExecutingAssembly"").Invoke(Nothing, Nothing)).GetManifestResourceStream(""" & ResName & """)" & vbNewLine _
                       & "            If cs Is Nothing Then" & vbNewLine _
                       & "                Return Nothing" & vbNewLine _
                       & "            End If" & vbNewLine _
                       & "            Using cm As MemoryStream = New MemoryStream" & vbNewLine _
                       & "                Const bValue = 4096" & vbNewLine _
                       & "                Dim buffer As Byte() = New Byte(bValue - 1) {}" & vbNewLine _
                       & "                Dim count As Integer = cs.Read(buffer, 0, bValue)" & vbNewLine _
                       & "                Do" & vbNewLine _
                       & "                    cm.Write(buffer, 0, count)" & vbNewLine _
                       & "                    count = cs.Read(buffer, 0, bValue)" & vbNewLine _
                       & "                Loop While (count <> 0)" & vbNewLine _
                       & If(compress = True, DecompressStr0, "d = cm.ToArray()") & vbNewLine _
                       & "                " & reverseStr & vbNewLine _
                       & "                ass = Assembly.Load(d)" & vbNewLine _
                       & "            End Using" & vbNewLine _
                       & "        End Using" & vbNewLine _
                       & "        Return ass" & vbNewLine _
                       & "    End Function" & vbNewLine _
                       & If(compress = True, Generator.GenerateCompressWithGzipByteFunc(Decompress0, Decompress1), "") & vbNewLine _
                       & "    Public Shared Sub " & initializeFuncName & vbNewLine _
                       & "        AddHandler AppDomain.CurrentDomain.AssemblyResolve, New ResolveEventHandler(AddressOf " & resolverName & ")" & vbNewLine _
                       & "    End Sub" & vbNewLine _
                       & "End Class"

            Return Compiler.CreateStubFromString(ClassName, Frmwk, str)
        End Function

        Protected Shared Function PackerStub(Resolver As Stub, EncodedResName$, m_polyXor As Crypt, encrypt As Boolean) As String
            Dim ResourceAssembly = Randomizer.GenerateNewAlphabetic
            Dim decodeString = Randomizer.GenerateNewAlphabetic
            Dim Decrypt = Randomizer.GenerateNewAlphabetic
            Dim fromBase64 = Randomizer.GenerateNewAlphabetic

            Dim reverseStr As String = String.Empty
            If encrypt Then
                reverseStr = "                    Array.Reverse(b)"
            End If

            Dim aesStr As String = "    Private Shared Function " & Decrypt & "(ByVal i As Byte()) As Byte()" & vbNewLine _
                        & "        Dim k as Byte() = " & Resolver.ReferencedZipperAssembly.refNewTypeName & ".pKey(""" & Convert.ToBase64String(SevenZipLib.SevenZipHelper.Compress(m_polyXor.key)) & """)" & vbNewLine _
                        & "        Dim O As Byte() = New Byte(i.Length - " & m_polyXor.SaltSize.ToString & " - 1) {}" & vbNewLine _
                        & "        Dim S As Byte() = New Byte(" & m_polyXor.SaltSize.ToString & " - 1) {}" & vbNewLine _
                        & "        Buffer.BlockCopy(i, i.Length - " & m_polyXor.SaltSize.ToString & ", S, 0, " & m_polyXor.SaltSize.ToString & ")" & vbNewLine _
                        & "        Array.Resize(Of Byte)(i, i.Length - " & m_polyXor.SaltSize.ToString & ")" & vbNewLine _
                        & "        For j As Integer = 0 To i.Length - 1" & vbNewLine _
                        & "            O(j) = CByte(i(j) Xor k(j Mod k.Length) Xor S(j Mod S.Length))" & vbNewLine _
                        & "        Next" & vbNewLine _
                        & "        Return O" & vbNewLine _
                        & "    End Function"


            Dim str = "Imports System.Windows.Forms" & vbNewLine & _
                    "Imports System.Security.Cryptography" & vbNewLine & _
                    "Imports System" & vbNewLine & _
                    "Imports System.Threading" & vbNewLine & _
                    "Imports System.Text" & vbNewLine & _
                    "Imports System.IO" & vbNewLine & _
                    "Imports System.Resources" & vbNewLine & _
                    "Imports System.IO.Compression" & vbNewLine & _
                    "Imports " & Resolver.ReferencedZipperAssembly.refNewNamespaceName & vbNewLine & vbNewLine _
          & Loader.GenerateInfos(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, "1.0.0.0") & vbNewLine _
          & "Friend Class " & Resolver.className & vbNewLine & vbNewLine _
          & "    Private Delegate Function z() As Assembly" & vbNewLine _
          & "    <STAThread()> _" & vbNewLine _
          & "    Public Shared Sub Main(ByVal a As String())" & vbNewLine _
          & "        Dim y As Assembly = " & ResourceAssembly & "((""app, version=0.0.0.0, culture=neutral, publickeytoken=null"").Replace("".resources"",""""))" & vbNewLine _
          & "        If y.EntryPoint.GetParameters().Length = 0 Then" & vbNewLine _
          & "            y.EntryPoint.Invoke(Nothing, New Object(-1) {})" & vbNewLine _
          & "        Else" & vbNewLine _
          & "            y.EntryPoint.Invoke(Nothing, New Object() {a})" & vbNewLine _
          & "        End If" & vbNewLine _
          & "    End Sub" & vbNewLine _
          & "    Private Shared Function " & decodeString & "(Byval S as String) As String" & vbNewLine _
          & "        Return Encoding.Default.GetString(" & fromBase64 & "(S))" & vbNewLine _
          & "    End Function" & vbNewLine _
          & "    Private Shared Function " & ResourceAssembly & "(n As String) As Assembly" & vbNewLine _
          & "        Dim a As Assembly = Nothing" & vbNewLine _
          & "        Using st As Stream = DirectCast([Delegate].CreateDelegate(GetType(z), GetType(Assembly).GetMethod(""GetExecutingAssembly"", New Type() {})), z).Invoke.GetManifestResourceStream(n & "".resources"")" & vbNewLine _
          & "            If st Is Nothing Then" & vbNewLine _
          & "                Exit Function" & vbNewLine _
          & "            End If" & vbNewLine _
          & "            Dim b As Byte() = " & Resolver.ReferencedZipperAssembly.refNewTypeName & "." & Resolver.ReferencedZipperAssembly.refNewMethodName & "(New BinaryReader(st).ReadBytes(CInt(st.Length)))" & vbNewLine _
          & "            " & reverseStr & vbNewLine _
          & "            a = Assembly.Load(" & Decrypt & "(b))" & vbNewLine _
          & "        End Using" & vbNewLine _
          & "        Return a" & vbNewLine _
          & "    End Function" & vbNewLine _
          & Generator.GenerateCompressWithGzipByteFunc(Resolver.funcName1, Resolver.funcName2) & vbNewLine _
          & "    Private Shared Function " & fromBase64 & "(ByVal i As String) As Byte()" & vbNewLine _
          & "        Return Convert.FromBase64String(i)" & vbNewLine _
          & "    End Function" & vbNewLine _
          & "    " & aesStr & vbNewLine _
          & "End Class"

            Dim dic As New Dictionary(Of String, Byte())
            dic.Add(Resolver.ReferencedZipperAssembly.fPath, Resolver.ReferencedZipperAssembly.refByte)

            Return Compiler.CreateStubFromString(Resolver.className, Frmwk, str.Replace("app, version=0.0.0.0, culture=neutral, publickeytoken=null", EncodedResName), dic)
        End Function

        Protected Shared Function ResourcesStub(ClassName$, initializeFuncName$, resolverName$, Decompress0$, Decompress1$, encrypt As Boolean, compress As Boolean) As String
            LoadNamespacesHeaders()

            Dim reverseStr = If(encrypt = True, "                    Array.Reverse(d)", String.Empty)
            Dim DecompressStr0 = "                d = " & Decompress0 & "(cm.ToArray)"

            Dim str = _
                "Imports System.Windows.Forms" & vbNewLine & _
                "Imports System.Collections.Generic" & vbNewLine & _
                "Imports System" & vbNewLine & _
                "Imports System.IO" & vbNewLine & _
                "Imports System.IO.Compression" & vbNewLine & vbNewLine _
                       & Loader.GenerateInfos(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, "1.0.0.0") & vbNewLine _
                       & m_AddedNamespaceStart & vbNewLine _
                       & "Public Class " & ClassName & vbNewLine _
                       & "    Private Shared Function " & resolverName & " (ByVal sender As Object, ByVal args As ResolveEventArgs) As Assembly" & vbNewLine _
                       & "        Dim names As String() = Nothing" & vbNewLine _
                       & "        Dim ass As Assembly = Nothing" & vbNewLine _
                       & "        Dim d as Byte()" & vbNewLine _
                       & "        If (ass Is Nothing) Then" & vbNewLine _
                       & "        Using cs As Stream = (GetType(System.Reflection.Assembly).GetMethod(""GetExecutingAssembly"").Invoke(Nothing, Nothing)).GetManifestResourceStream(""" & ResName & """)" & vbNewLine _
                       & "            If cs Is Nothing Then" & vbNewLine _
                       & "                Return Nothing" & vbNewLine _
                       & "            End If" & vbNewLine _
                       & "            Using cm As MemoryStream = New MemoryStream" & vbNewLine _
                       & "                Const bValue = 4096" & vbNewLine _
                       & "                Dim buffer As Byte() = New Byte(bValue - 1) {}" & vbNewLine _
                       & "                Dim count As Integer = cs.Read(buffer, 0, bValue)" & vbNewLine _
                       & "                Do" & vbNewLine _
                       & "                    cm.Write(buffer, 0, count)" & vbNewLine _
                       & "                    count = cs.Read(buffer, 0, bValue)" & vbNewLine _
                       & "                Loop While (count <> 0)" & vbNewLine _
                       & If(compress = True, DecompressStr0, "d = cm.ToArray()") & vbNewLine _
                       & "                " & reverseStr & vbNewLine _
                       & "                ass = Assembly.Load(d)" & vbNewLine _
                       & "                names = ass.GetManifestResourceNames" & vbNewLine _
                       & "            End Using" & vbNewLine _
                       & "        End Using" & vbNewLine _
                       & "        End If" & vbNewLine _
                       & "        If New List(Of String)(names).Contains(args.Name) Then" & vbNewLine _
                       & "            Return ass" & vbNewLine _
                       & "        End If" & vbNewLine _
                       & "        Return Nothing" & vbNewLine _
                       & "    End Function" & vbNewLine _
                       & If(compress = True, Generator.GenerateCompressWithGzipByteFunc(Decompress0, Decompress1), "") & vbNewLine _
                       & "    Public Shared Sub " & initializeFuncName & vbNewLine _
                       & "        AddHandler AppDomain.CurrentDomain.ResourceResolve, New ResolveEventHandler(AddressOf " & resolverName & ")" & vbNewLine _
                       & "    End Sub" & vbNewLine _
                       & "End Class" & vbNewLine _
                       & m_AddedNamespaceEnd

            Return Compiler.CreateStubFromString(ClassName, Frmwk, str)
        End Function

        Protected Shared Function AntiDebugStub(classname$, funcName$) As String
            LoadNamespacesHeaders()
            Dim FuncName2 = Randomizer.GenerateNewAlphabetic

            Dim str As String = _
    "Imports System" & vbNewLine & _
    "Imports System.Diagnostics" & vbNewLine & _
    "Imports System.Threading" & vbNewLine & _
     Loader.GenerateInfos(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, "1.0.0.0") & vbNewLine _
    & m_AddedNamespaceStart & vbNewLine & _
        "Friend Class " & classname & vbNewLine & _
            "Public Shared Sub " & funcName & "()" & vbNewLine & _
            "   If ((Not Environment.GetEnvironmentVariable(""COR_ENABLE_PROFILING"") Is Nothing) OrElse (Not Environment.GetEnvironmentVariable(""COR_PROFILER"") Is Nothing)) Then" & vbNewLine & _
            "       Environment.FailFast(""Profiler detected"")" & vbNewLine & _
            "   End If" & vbNewLine & _
            "   Dim parameter As New Thread(New ParameterizedThreadStart(AddressOf " & FuncName2 & "))" & vbNewLine & _
            "   Dim t As New Thread(New ParameterizedThreadStart(AddressOf " & FuncName2 & "))" & vbNewLine & _
            "   parameter.IsBackground = True" & vbNewLine & _
            "   t.IsBackground = True" & vbNewLine & _
            "   parameter.Start(t)" & vbNewLine & _
            "   Thread.Sleep(500)" & vbNewLine & _
            "   t.Start(parameter)" & vbNewLine & _
            "End Sub" & vbNewLine & vbNewLine & _
            "Private Shared Sub " & FuncName2 & "(ByVal th As Object)" & vbNewLine & _
            "   Thread.Sleep(&H3E8)" & vbNewLine & _
            "   Dim t As Thread = DirectCast(th, Thread)" & vbNewLine & _
            "   Do While True" & vbNewLine & _
            "       If (Debugger.IsAttached OrElse Debugger.IsLogging) Then" & vbNewLine & _
            "           Environment.FailFast(""Debugger detected (Managed)"")" & vbNewLine & _
            "       End If" & vbNewLine & _
            "       If Not t.IsAlive Then" & vbNewLine & _
            "           Environment.FailFast(""Loop broken"")" & vbNewLine & _
            "       End If" & vbNewLine & _
            "       Thread.Sleep(&H3E8)" & vbNewLine & _
            "   Loop" & vbNewLine & _
            "End Sub" & vbNewLine & vbNewLine & _
        "End Class" & vbNewLine & vbNewLine & _
    m_AddedNamespaceEnd
            Return Compiler.CreateStubFromString(classname, Frmwk, str)
        End Function

        Protected Shared Function AntiTamperStub(className$, FuncName$) As String
            LoadNamespacesHeaders()
            Dim str As String = _
                "Imports System.Security.Cryptography" & vbNewLine & _
                "Imports System.Windows.Forms" & vbNewLine & _
                "Imports System.Collections.Generic" & vbNewLine & _
                "Imports System" & vbNewLine & _
                "Imports System.IO" & vbNewLine & _
                "Imports System.IO.Compression" & vbNewLine & vbNewLine _
                 & Loader.GenerateInfos(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, "1.0.0.0") & vbNewLine _
                 & m_AddedNamespaceStart & vbNewLine _
                 & "Public Class " & className & vbNewLine _
                 & "    Public Shared Sub " & FuncName & " ()" & vbNewLine _
                 & "        Dim l As String = (GetType(System.Reflection.Assembly).GetMethod(""GetExecutingAssembly"").Invoke(Nothing, Nothing)).Location" & vbNewLine _
                 & "        Dim b As Stream = New StreamReader(l).BaseStream" & vbNewLine _
                 & "        Dim r As New BinaryReader(b)" & vbNewLine _
                 & "        Dim b0 As String = Nothing" & vbNewLine _
                 & "        Dim b1 As String = Nothing" & vbNewLine _
                 & "        b0 = BitConverter.ToString(Ctype(CryptoConfig.CreateFromName(" & Chr(34) & "MD5" & Chr(34) & "), HashAlgorithm).ComputeHash(r.ReadBytes((File.ReadAllBytes(l).Length - 16))))" & vbNewLine _
                 & "        b.Seek(-16, SeekOrigin.End)" & vbNewLine _
                 & "        b1 = BitConverter.ToString(r.ReadBytes(16))" & vbNewLine _
                 & "        If (b0 <> b1) Then" & vbNewLine _
                 & "            Throw New BadImageFormatException" & vbNewLine _
                 & "        End If" & vbNewLine _
                 & "    End Sub" & vbNewLine _
                 & "End Class" & vbNewLine _
                 & m_AddedNamespaceEnd
            Return Compiler.CreateStubFromString(className, Frmwk, str)
        End Function

        Protected Shared Function DynamicInvokeStub(className$, m_loadLibraryFuncName$, m_getMethProcFuncName$, m_invokeMethFuncName$)
            LoadNamespacesHeaders()
            Dim str = "Imports System.Threading" & vbNewLine & _
                        "Imports System" & vbNewLine & _
                        "Imports System.Reflection.Emit" & vbNewLine & _
                        "Imports System.Runtime.InteropServices" & vbNewLine & vbNewLine _
                       & Loader.GenerateInfos(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, "1.0.0.0") & vbNewLine _
                       & m_AddedNamespaceStart & vbNewLine _
                       & "Public Class " & className & vbNewLine _
                       & "    <DllImport(""kernel32.dll"", EntryPoint :=""LoadLibrary"")> _" & vbNewLine _
                       & "    Private Shared Function " & m_loadLibraryFuncName & "(hLib As String) As IntPtr" & vbNewLine _
                       & "    End Function" & vbNewLine _
                       & "    <DllImport(""kernel32.dll"", EntryPoint :=""GetProcAddress"")> _" & vbNewLine _
                       & "    Private Shared Function " & m_getMethProcFuncName & "(hMod As IntPtr, pName As String) As IntPtr" & vbNewLine _
                       & "    End Function" & vbNewLine _
                       & "    Public Shared Function " & m_invokeMethFuncName & " (Of T As Class)(libF$, funcN$) As T" & vbNewLine _
                       & "        Dim ll As IntPtr = " & m_loadLibraryFuncName & "(libF)" & vbNewLine _
                       & "        Dim delegT As System.Delegate = Marshal.GetDelegateForFunctionPointer(" & m_getMethProcFuncName & "(ll, funcN), GetType(T))" & vbNewLine _
                       & "        Return TryCast(delegT, T)" & vbNewLine _
                       & "    End Function" & vbNewLine _
                       & "End Class" & vbNewLine _
                       & m_AddedNamespaceEnd
            Return Compiler.CreateStubFromString(className, Frmwk, str)
        End Function

        Protected Shared Function ResourcesEmbeddingStub(ClassName$, initializeFuncName$, encrypt As Boolean, compress As Boolean) As String
            LoadNamespacesHeaders()

            Dim resolverName = Randomizer.GenerateNewAlphabetic
            Dim Decompress0 = Randomizer.GenerateNewAlphabetic
            Dim Decompress1 = Randomizer.GenerateNewAlphabetic

            Dim reverseStr = If(encrypt = True, "                    Array.Reverse(b)", String.Empty)
            Dim DecompressStr0 = "                b = " & Decompress0 & "(b)"

            Dim str = "Imports Microsoft.VisualBasic" & vbNewLine & _
                        "Imports System.Windows.Forms" & vbNewLine & _
                        "Imports System.Runtime.InteropServices" & vbNewLine & _
                        "Imports System.Collections.Generic" & vbNewLine & _
                        "Imports System" & vbNewLine & _
                        "Imports System.IO" & vbNewLine & _
                        "Imports System.IO.Compression" & vbNewLine & vbNewLine _
                       & Loader.GenerateInfos(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, "1.0.0.0") & vbNewLine _
                       & m_AddedNamespaceStart & vbNewLine _
                       & "Public Class " & ClassName & vbNewLine _
                       & "    <DllImport(""kernel32"")> _" & vbNewLine _
                       & "    Private Shared Function MoveFileEx(ByVal existingFileName As String, ByVal newFileName As String, ByVal flags As Integer) As Boolean" & vbNewLine _
                       & "    End Function" & vbNewLine _
                       & "    Private Delegate Function z() As Assembly" & vbNewLine _
                       & "    Private Shared Function " & resolverName & " (ByVal sender As Object, ByVal args As ResolveEventArgs) As Assembly" & vbNewLine _
                       & If(Pack, "        Dim k As String = getEnc(Cstr(New AssemblyName(args.Name).FullName.GetHashCode))", "        Dim k As String = Cstr(New AssemblyName(args.Name).FullName.GetHashCode)") & vbNewLine _
                       & "        Dim ass As Assembly = Nothing" & vbNewLine _
                       & "        If Not k.Length = 0 Then" & vbNewLine _
                       & "        Dim baseResourceName As String =  k & "".resources""" & vbNewLine _
                       & "        Dim bn as boolean" & vbNewLine _
                       & "        SyncLock hashtable" & vbNewLine _
                       & "            If hashtable.ContainsKey(baseResourceName) Then" & vbNewLine _
                       & "                Return hashtable.Item(baseResourceName)" & vbNewLine _
                       & "            End If" & vbNewLine _
                       & "            Using st As Stream = DirectCast([Delegate].CreateDelegate(GetType(z), GetType(Assembly).GetMethod(""GetExecutingAssembly"", New Type() {})), z).Invoke.GetManifestResourceStream(baseResourceName)" & vbNewLine _
                       & "                If st Is Nothing Then" & vbNewLine _
                       & "                    Return ass" & vbNewLine _
                       & "                End If" & vbNewLine _
                       & "                Dim b As Byte() = New BinaryReader(st).ReadBytes(CInt(st.Length))" & vbNewLine _
                       & If(compress = True, DecompressStr0, "") & vbNewLine _
                       & "                " & reverseStr & vbNewLine _
                       & "                Try" & vbNewLine _
                       & "                    ass = Assembly.Load(b)" & vbNewLine _
                       & "                Catch ex1 As FileLoadException" & vbNewLine _
                       & "                    bn = True" & vbNewLine _
                       & "                Catch ex2 As BadImageFormatException" & vbNewLine _
                       & "                    bn = True" & vbNewLine _
                       & "                End Try" & vbNewLine _
                       & "                If bn Then" & vbNewLine _
                       & "                    Try" & vbNewLine _
                       & "                        Dim npath As String = String.Format(""{0}{1}\"", System.IO.Path.GetTempPath, k)" & vbNewLine _
                       & "                        Directory.CreateDirectory(npath)" & vbNewLine _
                       & "                        Dim nfileP As String = (npath & baseResourceName)" & vbNewLine _
                       & "                        If Not File.Exists(nfileP) Then" & vbNewLine _
                       & "                            Dim fStream As FileStream = File.OpenWrite(nfileP)" & vbNewLine _
                       & "                            fStream.Write(b, 0, b.Length)" & vbNewLine _
                       & "                            fStream.Close" & vbNewLine _
                       & "                            MoveFileEx(nfileP, Nothing, 4)" & vbNewLine _
                       & "                            MoveFileEx(npath, Nothing, 4)" & vbNewLine _
                       & "                        End If" & vbNewLine _
                       & "                        ass = Assembly.LoadFile(nfileP)" & vbNewLine _
                       & "                    Catch Ex As Exception" & vbNewLine _
                       & "                    End Try" & vbNewLine _
                       & "                End If" & vbNewLine _
                       & "                hashtable.Item(baseResourceName) = ass" & vbNewLine _
                       & "                Return ass" & vbNewLine _
                       & "            End Using" & vbNewLine _
                       & "        End SyncLock" & vbNewLine _
                       & "        End If" & vbNewLine _
                       & "        Return ass" & vbNewLine _
                       & "    End Function" & vbNewLine _
                       & "    Shared Sub new" & vbNewLine _
                       & "        hashtable = New Dictionary(Of String, Assembly)" & vbNewLine _
                       & "    End Sub" & vbNewLine _
                       & If(compress = True, Generator.GenerateCompressWithGzipByteFunc(Decompress0, Decompress1), "") & vbNewLine _
                       & "    Private Shared hashtable As Dictionary(Of String, Assembly)" & vbNewLine _
                       & "    Public Shared Sub " & initializeFuncName & vbNewLine _
                       & "        AddHandler AppDomain.CurrentDomain.AssemblyResolve, New ResolveEventHandler(AddressOf " & resolverName & ")" & vbNewLine _
                       & "    End Sub" & vbNewLine _
                       & "    Private Shared Function getEnc(Str$) As String" & vbNewLine _
                       & "        Return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Str))" & vbNewLine _
                       & "    End Function" & vbNewLine _
                       & "End Class" & vbNewLine & vbNewLine & _
                       m_AddedNamespaceEnd

            Return Compiler.CreateStubFromString(ClassName, Frmwk, str)
        End Function

        Overloads Shared Sub CleanUp()
            Frmwk = String.Empty
            completedInstructions.Clear()
            completedMethods.Clear()
            If Not ResWriter Is Nothing Then ResWriter.Dispose()
            If File.Exists(My.Application.Info.DirectoryPath & "\" & ResName & ".resources") Then
                File.Delete(My.Application.Info.DirectoryPath & "\" & ResName & ".resources")
            End If
            ResName = String.Empty
            Pack = False
        End Sub

#End Region

    End Class
End Namespace




