Imports Mono.Cecil
Imports Mono.Cecil.Rocks
Imports Mono.Cecil.Cil
Imports Helper.RandomizeHelper
Imports System.Runtime.CompilerServices
Imports Helper.CecilHelper
Imports Helper.AssemblyHelper
Imports Helper.CodeDomHelper
Imports System.Text.RegularExpressions
Imports Helper.CryptoHelper
Imports Helper.UtilsHelper
Imports System.Resources
Imports System.IO
Imports System.CodeDom.Compiler
Imports System.Text
Imports System.IO.Compression
Imports Implementer.Core.Obfuscation.Builder
Imports Implementer.Core.Obfuscation.Exclusion

Namespace Core.Obfuscation.Protection
    Public NotInheritable Class Str
        Inherits Source

#Region " Fields "
        Private Shared DecryptReadStringResources As Stub
        Private Shared DecryptXor As Stub
        Private Shared DecryptBase64 As Stub
        Private Shared m_s As MemoryStream = Nothing
        Private Shared m_bw As BinaryWriter = Nothing
        Private Shared randSalt As Random
        Private Shared Types As New List(Of TypeDefinition)
#End Region

#Region " Properties "
        Shared Property objTarget() As Object
        Shared Property XorEncryptType() As Type
#End Region

#Region " Constructor "
        Shared Sub New()
            randSalt = New Random
        End Sub
#End Region

#Region " Methods "
        Friend Shared Function DoJob(ByVal asm As AssemblyDefinition, Framework$, encryptToRes As EncryptType, Exclude As ExcludeList, Optional ByVal packIt As Boolean = False) As AssemblyDefinition
            AssemblyDef = asm
            Frmwk = Framework
            Pack = packIt

            EncryptToResources = encryptToRes

            If encryptToRes = EncryptType.ToResources Then
                ResName = Randomizer.GenerateNew
                m_s = New MemoryStream
                m_bw = New BinaryWriter(m_s)

                DecryptReadStringResources = New Stub(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic)
                With DecryptReadStringResources
                    .ResolveTypeFromFile(ReadStringFromResourcesStub(.className, .funcName1, .funcName2, .funcName3), Finder.FindDefaultNamespace(asm, Pack), Randomizer.GenerateNew, Randomizer.GenerateNew, Randomizer.GenerateNew, Randomizer.GenerateNew)
                    .InjectType(asm)
                    completedMethods.Add(.GetMethod1)
                End With
            End If

            DecryptXor = New Stub(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic)
            With DecryptXor
                .ResolveTypeFromFile(DecryptXorStub(.className, .funcName1), Finder.FindDefaultNamespace(asm, Pack), Randomizer.GenerateNew)
                .InjectType(asm)
                completedMethods.Add(.GetMethod1)
                _XorEncryptType = GenerateEncryptXor()
            End With

            DecryptBase64 = New Stub(Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic, Randomizer.GenerateNewAlphabetic)
            With DecryptBase64
                .ResolveTypeFromFile(FromBase64Stub(.className, .funcName1, .funcName2), Finder.FindDefaultNamespace(asm, Pack), Randomizer.GenerateNew, Randomizer.GenerateNew, Randomizer.GenerateNew)
                .InjectType(asm)
                completedMethods.Add(.GetMethod1)
                completedMethods.Add(.GetMethod2)
            End With

            For Each m As ModuleDefinition In asm.Modules
                Types.AddRange(m.GetAllTypes())
                For Each type As TypeDefinition In Types
                    If NameChecker.IsRenamable(type) Then
                        If Exclude.isStringEncryptExclude(type) = False Then
                            IterateType(type)
                        End If
                    End If
                Next
                Types.Clear()
            Next

            If encryptToRes = EncryptType.ToResources Then
                Injecter.InjectResource(asm.MainModule, ResName, ResourceType.Embedded, CompressWithGStream(m_s.ToArray))
                m_bw.Close()
            End If

            DeleteStubs()
            CleanUp()

            Return asm
        End Function

        Private Shared Function writeData(str$) As Integer
            Dim IntegPosit% = m_bw.BaseStream.Position
            m_bw.Write(str)
            m_bw.Flush()
            Return IntegPosit
        End Function

        Private Shared Sub DeleteStubs()
            If Not DecryptReadStringResources Is Nothing Then DecryptReadStringResources.DeleteDll()
            If Not DecryptXor Is Nothing Then DecryptXor.DeleteDll()
            If Not DecryptBase64 Is Nothing Then DecryptBase64.DeleteDll()
        End Sub

        Private Shared Sub IterateType(ByVal td As TypeDefinition)
            Dim publicMethods As New List(Of MethodDefinition)()
            publicMethods.AddRange(From m In td.Methods Where (m.HasBody AndAlso m.Body.Instructions.Count >= 1 AndAlso Not completedMethods.Contains(m) AndAlso Not m.Name = "get_ResourceManager" AndAlso Not Utils.isStronglyTypedResourceBuilder(m.DeclaringType) AndAlso Not Finder.FindCustomAttributeByName(m.DeclaringType, "EditorBrowsableAttribute")))

            Try
                For Each md In publicMethods
                    If publicMethods.Contains(md) Then

                        Using optim As New Msil(md.Body)
                            For i = 0 To md.Body.Instructions.Count - 1
                                Dim Instruction As Instruction = md.Body.Instructions(i)
                                If Not completedInstructions.Contains(Instruction) Then
                                    Dim mdFinal As MethodDefinition = Nothing
                                    Dim index As Integer = md.Body.Instructions.IndexOf(Instruction)
                                    If (Instruction.OpCode = OpCodes.Ldstr) Then
                                        Dim str = TryCast(Instruction.Operand, String)
                                        Dim salt = randSalt.Next(1, 255)
                                        Dim addProperty As Boolean = Randomizer.GenerateBoolean
                                        If Not String.IsNullOrWhiteSpace(str) And str.Length > 0 Then
                                            If optim.IsSettingStr(str) = False Then
                                                If Not str = ResName Then
                                                    'If Functions.isBase64StringEncoded(str) = False Then

                                                    mdFinal = New MethodDefinition(Randomizer.GenerateNew, MethodAttributes.[Static] Or MethodAttributes.[Public] Or MethodAttributes.HideBySig, AssemblyDef.MainModule.Import(GetType(String)))
                                                    mdFinal.Body = New MethodBody(mdFinal)

                                                    Dim encXor = EncryptXor(EncodeTo_64(str), salt)

                                                    Dim ilProc As ILProcessor = mdFinal.Body.GetILProcessor()
                                                    ilProc.Body.MaxStackSize = 8
                                                    ilProc.Body.InitLocals = True
                                                    mdFinal.Body.Variables.Add(New VariableDefinition(AssemblyDef.MainModule.Import(GetType(String))))

                                                    If EncryptToResources = EncryptType.ToResources Then
                                                        Dim inte = writeData(encXor)
                                                        ilProc.Emit(OpCodes.Ldc_I4, inte)
                                                        ilProc.Emit(OpCodes.Call, DecryptReadStringResources.GetMethod1)
                                                    Else
                                                        ilProc.Emit(Mono.Cecil.Cil.OpCodes.Ldstr, encXor)
                                                    End If

                                                    ilProc.Emit(OpCodes.Ldc_I4, salt)
                                                    ilProc.Emit(OpCodes.Call, DecryptXor.GetMethod1)
                                                    ilProc.Emit(OpCodes.Call, DecryptBase64.GetMethod1)
                                                    ilProc.Emit(OpCodes.Call, DecryptBase64.GetMethod2)
                                                    ilProc.Emit(OpCodes.Stloc_0)
                                                    ilProc.Emit(OpCodes.Ldloc_0)
                                                    ilProc.Emit(OpCodes.Ret)

                                                    md.DeclaringType.Methods.Add(mdFinal)

                                                    If addProperty Then
                                                        Dim pDefinit As New PropertyDefinition(Randomizer.GenerateNew, PropertyAttributes.None, AssemblyDef.MainModule.Import(GetType(String)))
                                                        md.DeclaringType.Properties.Add(pDefinit)

                                                        Dim mDefinit = New MethodDefinition(("get_" & pDefinit.Name), MethodAttributes.Static Or MethodAttributes.Public, pDefinit.PropertyType)
                                                        mDefinit.Body = New MethodBody(mDefinit)
                                                        pDefinit.GetMethod = mDefinit
                                                        pDefinit.DeclaringType.Methods.Add(mDefinit)

                                                        If Not pDefinit.DeclaringType.IsInterface Then
                                                            Dim iLProcessor As ILProcessor = mDefinit.Body.GetILProcessor
                                                            With iLProcessor
                                                                .Body.MaxStackSize = 1
                                                                .Body.InitLocals = True
                                                                mDefinit.Body.Variables.Add(New VariableDefinition(AssemblyDef.MainModule.Import(GetType(String))))
                                                                .Emit(OpCodes.Call, mdFinal)
                                                                .Emit(OpCodes.Stloc_0)
                                                                .Emit(OpCodes.Ldloc_0)
                                                                .Emit(OpCodes.Ret)
                                                            End With
                                                        Else
                                                            mDefinit.IsAbstract = True
                                                            mDefinit.IsVirtual = True
                                                            mDefinit.IsNewSlot = True
                                                        End If
                                                        mDefinit.IsSpecialName = True
                                                        mDefinit.IsGetter = True
                                                    End If
                                                    'End If
                                                End If
                                            End If
                                        End If
                                        If (Not mdFinal Is Nothing) Then
                                            If mdFinal.DeclaringType.IsNotPublic Then
                                                mdFinal.DeclaringType.IsPublic = True
                                            End If
                                            md.Body.Instructions.Item(index).OpCode = OpCodes.Call
                                            md.Body.Instructions.Item(index).Operand = mdFinal

                                            completedMethods.Add(mdFinal)
                                            completedInstructions.Add(Instruction)
                                        End If
                                    End If
                                End If
                            Next
                            optim.FixBranchOffsets()
                            optim.MethodBody.SimplifyMacros()
                        End Using
                    End If
                Next
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            publicMethods.Clear()
        End Sub

        Private Shared Function CompressWithGStream(raw As Byte()) As Byte()
            Using memory As New MemoryStream()
                Using gzip As New GZipStream(memory, CompressionMode.Compress, True)
                    gzip.Write(raw, 0, raw.Length)
                End Using
                Return memory.ToArray()
            End Using
        End Function

        Private Shared Function EncodeTo_64(toEncode$) As String
            Return Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(toEncode))
        End Function

        Private Shared Function GenerateEncryptXor() As Type
            _objTarget = DecryptXorType(DecryptXor.className, DecryptXor.funcName1)
            Return _objTarget
        End Function

        Private Shared Function EncryptXor(text$, key%) As String
            Return XorEncryptType.InvokeMember(DecryptXor.funcName1, Reflection.BindingFlags.InvokeMethod Or Reflection.BindingFlags.Default, Nothing, _objTarget, New Object() {text, key})
        End Function

        Overloads Shared Sub CleanUp()
            objTarget() = Nothing
            XorEncryptType() = Nothing
            Frmwk = String.Empty
        End Sub
#End Region

    End Class
End Namespace
