Imports System.IO
Imports Mono.Cecil
Imports Mono.Cecil.Rocks
Imports System.Runtime.InteropServices
Imports System.Reflection.Emit
Imports System.Reflection
Imports Helper.UtilsHelper
Imports Helper.AssemblyHelper
Imports Helper.CecilHelper
Imports Mono.Cecil.Cil
Imports Implementer.engine.Analyze

Namespace Engine.Identification

    Public NotInheritable Class Identifier

#Region " Fields "
        Private Shared m_HexAssembly As String = String.Empty
        Private Shared m_assem As Data = Nothing
        Private Shared m_assDef As AssemblyDefinition = Nothing
        Private Shared m_assDefEntryPoint As MethodDefinition = Nothing
        Private Shared m_assDefReferences As Generic.ICollection(Of AssemblyNameReference)
        Private Shared m_assDefResources As Generic.ICollection(Of Resource)
        Private Shared m_assDefTypes As Generic.ICollection(Of TypeDefinition)
        Private Shared m_assDefModuleReferences As Generic.ICollection(Of ModuleReference)

        Private Shared ReadOnly IdentifierSearcher As New List(Of ObfuscatorPackerIdentifierDelegate)() From { _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findConfuserObfuscator), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findIdentifierInitializer), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findRpxMethods), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findILProtectorReferences), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findDotBundle), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findNetz), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findNetPack), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findMpress), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findNetshrink), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findDotNetPatcherAttribute), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findSmartAssemblyAttribute), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findBabelAttribute), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findCliSecureAttribute), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findCodeVeilType), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findCryptoObfuscatorAttribute), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findDotfuscatorAttribute), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findGoliathNETAttribute), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findSpicesAttributes), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findSkaterAttribute), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findManco), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findCodeFort), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findMacrobjectObfuscator), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findPhoenixProtectorObfuscator), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findEazFuscatorObfuscator), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findDotWallObfuscator), _
      New ObfuscatorPackerIdentifierDelegate(AddressOf findOtherAttribute) _
      }
#End Region

#Region " Delegates "
        Private Delegate Function ObfuscatorPackerIdentifierDelegate(ByRef found As Boolean) As IdentifierResult
#End Region

#Region " Methods "
        Public Shared Function search(e As ValidatedFile) As IdentifierResult
            If e.peInfos.HasInvalidSectionHeader Then
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End If

            m_assem = e.assembly
            m_HexAssembly = Functions.AssemblyToHex(m_assem.Location)

            For Each task In IdentifierSearcher
                Dim found As Boolean
                Dim signature = task(found)
                If found Then
                    Return signature
                End If
            Next
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        Private Shared Function findConfuserObfuscator(ByRef found As Boolean) As IdentifierResult
            Try
                If Not m_HexAssembly Is Nothing Then
                    With m_HexAssembly
                        If .Contains(Functions.StrToHex("___.netmodule")) Then
                            found = True
                            Return New IdentifierResult(IdentifierEnum.ResultName.Confuser, IdentifierEnum.ResultType.Packer, My.Resources.Confuser)
                        ElseIf .Contains(Functions.StrToHex("ConfusedByAttribute")) Then
                            found = True
                            Return New IdentifierResult(IdentifierEnum.ResultName.Confuser, IdentifierEnum.ResultType.Obfuscator, My.Resources.Confuser)
                        ElseIf .Contains(Functions.StrToHex("IsDebuggerPresent")) AndAlso _
                            .Contains(Functions.StrToHex("CloseHandle")) AndAlso _
                            .Contains(Functions.StrToHex("NtSetInformationProcess")) AndAlso _
                            .Contains(Functions.StrToHex("NtQueryInformationProcess")) AndAlso _
                            .Contains(Functions.StrToHex("OutputDebugString")) Then
                            found = True
                            Return New IdentifierResult(IdentifierEnum.ResultName.Confuser, IdentifierEnum.ResultType.Obfuscator, My.Resources.Confuser)
                        End If
                    End With
                End If
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        Private Shared Function findIdentifierInitializer(ByRef found As Boolean) As IdentifierResult
            Try
                If Not m_assem.EntryPoint Is Nothing Then
                    m_assDef = AssemblyDefinition.ReadAssembly(m_assem.Location)
                    m_assDefEntryPoint = m_assDef.MainModule.EntryPoint
                    m_assDefReferences = m_assDef.MainModule.AssemblyReferences
                    m_assDefResources = m_assDef.MainModule.Resources
                    m_assDefTypes = m_assDef.MainModule.Types
                    m_assDefModuleReferences = m_assDef.MainModule.ModuleReferences
                Else
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
                End If
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'RPX
        Private Shared Function findRpxMethods(ByRef found As Boolean) As IdentifierResult
            Try
                If ((m_assDefEntryPoint.Body.Instructions.Item(0).OpCode.Code = Code.Ldtoken) AndAlso TryCast(m_assDefEntryPoint.Body.Instructions.Item(0).Operand, TypeReference).FullName = "R.P") Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.Rpx, IdentifierEnum.ResultType.Packer, My.Resources.RPX)
                End If
            Catch ex As FileNotFoundException
                found = False
                Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'ILPROTECTOR
        Private Shared Function findILProtectorReferences(ByRef found As Boolean) As IdentifierResult
            Try
                If m_assDefModuleReferences.Any(Function(ref) ref.Name.EndsWith("Protect32.dll") OrElse ref.Name.EndsWith("Protect64.dll") OrElse ref.Name.EndsWith(m_assDef.MainModule.Name & "32.dll") OrElse ref.Name.EndsWith(m_assDef.MainModule.Name & "64.dll")) Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.ILProtector, IdentifierEnum.ResultType.Packer, My.Resources.ILProtector)
                End If
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'DOTBUNDLE PACKER
        Private Shared Function findDotBundle(ByRef found As Boolean) As IdentifierResult
            Try
                '################################### EAZFUSCATOR DOTNET #####################################
                If m_assDefResources.Any(Function(r) r.Name = "dbrsrc") Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.DotBundle, IdentifierEnum.ResultType.Packer, My.Resources.DotBundle)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'NETZ DOTNET PACKER
        Private Shared Function findNetz(ByRef found As Boolean) As IdentifierResult
            Try
                '###################################### NETZ PACKER #########################################
                If m_assDefTypes.Any(Function(typeDef) typeDef.FullName.Contains("netz") AndAlso typeDef.Name = "NetzStarter") Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.Netz, IdentifierEnum.ResultType.Packer, My.Resources.Netz)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'NETPACK
        Private Shared Function findNetPack(ByRef found As Boolean) As IdentifierResult
            Try
                '######################################## NETPACK ###########################################
                If m_assDefEntryPoint.DeclaringType.Name = "netpack" AndAlso m_assDefEntryPoint.DeclaringType.Namespace = "npack" Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.NetPack, IdentifierEnum.ResultType.Packer, My.Resources.NetPack)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'MPRESS DOTNET PACKER
        Private Shared Function findMpress(ByRef found As Boolean) As IdentifierResult
            Try
                '##################################### MPRESS PACKER ########################################
                If m_assDefEntryPoint.DeclaringType.Name = "_" AndAlso m_assDefEntryPoint.DeclaringType.Namespace = "mpress" Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.MPress, IdentifierEnum.ResultType.Packer, My.Resources.MPress)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        '.NETSHRINK PACKER
        Private Shared Function findNetshrink(ByRef found As Boolean) As IdentifierResult
            Try
                '################################### DOTNETSHRINK PACKER ####################################
                If m_assDefTypes.Any(Function(typeDef) typeDef.FullName.Contains("stub_2.Properties")) Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.NetShrink, IdentifierEnum.ResultType.Packer, My.Resources.NetShrink)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'SMARTASSEMBLY
        Private Shared Function findSmartAssemblyAttribute(ByRef found As Boolean) As IdentifierResult
            Try
                '######################################## SMARTASSEMBLY #####################################
                If m_assDefTypes.Any(Function(typeDef) typeDef.Name = "PoweredByAttribute") Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.SmartAssembly, IdentifierEnum.ResultType.Obfuscator, My.Resources.SmartAssembly)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'BABELNET
        Private Shared Function findBabelAttribute(ByRef found As Boolean) As IdentifierResult
            Try
                '######################################## BABEL OBFUSCATOR ##################################
                If m_assDefTypes.Any(Function(typeDef) typeDef.Name = "Babel" AndAlso typeDef.Name = "Attribute") Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.BabelObfuscator, IdentifierEnum.ResultType.Obfuscator, My.Resources.Babel)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'CLISECURE
        Private Shared Function findCliSecureAttribute(ByRef found As Boolean) As IdentifierResult
            Try
                '######################################## AGILEDOTNET #######################################
                If m_assDefTypes.Any(Function(typeDef) typeDef.FullName.Contains("AgileDotNet")) Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.AgileDotNet, IdentifierEnum.ResultType.Obfuscator, My.Resources.AgileDotNet)
                ElseIf m_assDefModuleReferences.Any(Function(ref) ref.Name = "AgileDotNetRT.dll" OrElse ref.Name = "AgileDotNetRT64.dll") Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.AgileDotNet, IdentifierEnum.ResultType.Obfuscator, My.Resources.AgileDotNet)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'CODEVEIL
        Private Shared Function findCodeVeilType(ByRef found As Boolean) As IdentifierResult
            Try
                '######################################## CODEVEIL ##########################################
                If m_assDefTypes.Any(Function(typeDef) typeDef.FullName = "____KILL") Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.CodeVeil, IdentifierEnum.ResultType.Obfuscator, My.Resources.CodeVeil)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'CRYPTOOBFUSCATOR
        Private Shared Function findCryptoObfuscatorAttribute(ByRef found As Boolean) As IdentifierResult
            Try
                '################################### CRYPTO OBFUSCATOR ######################################
                If m_assDefTypes.Any(Function(typeDef) typeDef.FullName.Contains("CryptoObfuscator")) Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.CryptoObfuscator, IdentifierEnum.ResultType.Obfuscator, My.Resources.CryptoObfuscator)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'DOTFUSCATOR
        Private Shared Function findDotfuscatorAttribute(ByRef found As Boolean) As IdentifierResult
            Try
                '###################################### DOTFUSCATOR #########################################
                If m_assDefTypes.Any(Function(typeDef) typeDef.FullName.Contains("Dotfuscator")) Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.DotFuscator, IdentifierEnum.ResultType.Obfuscator, My.Resources.DotFuscator)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'GOLIATH.NET
        Private Shared Function findGoliathNETAttribute(ByRef found As Boolean) As IdentifierResult
            Try
                '###################################### GOLIATH DOTNET ######################################
                If m_assDefTypes.Any(Function(typeDef) typeDef.FullName = "ObfuscatedByGoliath") Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.GoliathDotNet, IdentifierEnum.ResultType.Obfuscator, My.Resources.Goliath)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'SPICES.NET
        Private Shared Function findSpicesAttributes(ByRef found As Boolean) As IdentifierResult
            Try
                '######################################### NINERAYS #########################################
                If m_assDefTypes.Any(Function(typeDef) typeDef.FullName.Contains("NineRays")) Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.SpicesDotNet, IdentifierEnum.ResultType.Obfuscator, My.Resources.Spices)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'SKATER DOTNET OBFUSCATOR
        Private Shared Function findSkaterAttribute(ByRef found As Boolean) As IdentifierResult
            Try
                '################################ SKATER DOTNET OBFUSCATOR ##################################
                If m_assDefTypes.Any(Function(typeDef) typeDef.Name.Contains("Skater_NET_Obfuscator")) Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.SkaterDotNetObfuscator, IdentifierEnum.ResultType.Obfuscator, My.Resources.Skater)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'MANCO DOTNET OBFUSCATOR
        Private Shared Function findManco(ByRef found As Boolean) As IdentifierResult
            Try
                '#################################### MANCO OBFUSCATOR ######################################
                If m_assDefTypes.Any(Function(typeDef) typeDef.Name.Contains("();" & vbTab)) Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.MancoDotNetObfuscator, IdentifierEnum.ResultType.Obfuscator, My.Resources.Manco)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'CODEFORT OBFUSCTAOR
        Private Shared Function findCodeFort(ByRef found As Boolean) As IdentifierResult
            Try
                '################################## CODEFORT OBFUSCATOR #####################################
                If m_assDefTypes.Any(Function(typeDef) typeDef.FullName.Contains("___codefort")) Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.CodeFortObfuscator, IdentifierEnum.ResultType.Obfuscator, My.Resources.CodeFort)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'DOTNETPATCHER
        Private Shared Function findDotNetPatcherAttribute(ByRef found As Boolean) As IdentifierResult
            Try
                '######################################## DOTNETPATCHER #####################################
                If m_assDefResources.Any(Function(r) r.Name = My.Resources.DnpPattribute) Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.DotNetPatcher, IdentifierEnum.ResultType.Packer, My.Resources.DNP)
                ElseIf m_assDefResources.Any(Function(r) r.Name = My.Resources.DnpOattribute) Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.DotNetPatcher, IdentifierEnum.ResultType.Obfuscator, My.Resources.DNP)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'MACROBJECT OBFUSCATOR
        Private Shared Function findMacrobjectObfuscator(ByRef found As Boolean) As IdentifierResult
            Try
                '################################## MACROBJECT OBFUSCATOR ###################################
                If m_assDefTypes.Any(Function(typeDef) typeDef.FullName.Contains("Macrobject.Obfuscator")) Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.MacrobjectObfuscator, IdentifierEnum.ResultType.Obfuscator, My.Resources.Macrobject)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'PHOENIXPROTECTOR
        Private Shared Function findPhoenixProtectorObfuscator(ByRef found As Boolean) As IdentifierResult
            Try
                '################################### PHOENIX PROTECTOR ######################################
                If m_assDefTypes.Any(Function(typeDef) typeDef.FullName.StartsWith("?") AndAlso typeDef.FullName.EndsWith("?")) Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.PhoenixProtector, IdentifierEnum.ResultType.Obfuscator, My.Resources.NetReactor)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'EAZFUSCATOR OBFUSCATOR
        Private Shared Function findEazFuscatorObfuscator(ByRef found As Boolean) As IdentifierResult
            Try
                '################################### EAZFUSCATOR DOTNET #####################################
                If m_assDefResources.Any(Function(r) r.Name.StartsWith("Eazfuscator.NET")) Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.EazfuscatorDotNet, IdentifierEnum.ResultType.Obfuscator, My.Resources.EazFuscator)
                End If
                '############################################################################################
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'DOTWALL OBFUSCATOR
        Private Shared Function findDotWallObfuscator(ByRef found As Boolean) As IdentifierResult
            Try
                Dim hasFieldRef As Boolean
                Dim RetTypeIsString As Integer

                For Each td In m_assDefTypes
                    For Each meth In td.Methods
                        If meth.HasBody AndAlso meth.IsConstructor AndAlso meth.Name = ".cctor" AndAlso meth.DeclaringType.Methods.Count = 5 AndAlso meth.DeclaringType.Fields.Count = 1 Then
                            For j As Integer = 0 To meth.Body.Instructions.Count - 1
                                Dim ins As Instruction = meth.Body.Instructions(j)
                                If ins.OpCode = Mono.Cecil.Cil.OpCodes.Stsfld Then
                                    Dim fRef As FieldReference = DirectCast(ins.Operand, FieldReference)
                                    If meth.DeclaringType.Fields(0) Is fRef.Resolve Then
                                        hasFieldRef = True
                                    End If
                                ElseIf ins.OpCode.OperandType <> Mono.Cecil.Cil.OperandType.InlineMethod Then
                                    Continue For
                                End If
                            Next
                        ElseIf Not meth.IsConstructor AndAlso meth.DeclaringType.Methods.Count = 5 AndAlso meth.DeclaringType.Fields.Count = 1 Then
                            If meth.ReturnType.ToString = "System.String" Then
                                RetTypeIsString += 1
                            End If
                        End If
                    Next
                Next
                If hasFieldRef AndAlso RetTypeIsString = 3 Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.DotWall, IdentifierEnum.ResultType.Obfuscator, My.Resources.DotWall)
                End If
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function

        'OTHER
        Private Shared Function findOtherAttribute(ByRef found As Boolean) As IdentifierResult
            Try
                If m_assDef.MainModule.HasResources Then
                    If m_assDefTypes.Any(Function(typeDef) typeDef.FullName.Contains("Resources")) = False Then
                        found = True
                        Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
                    End If
                End If

                If m_assDef.MainModule.GetType("<Module>") Is Nothing Then
                    found = True
                    Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
                Else
                    If Not m_assDef.MainModule.GetType("<Module>") Is Nothing AndAlso Not m_assDef.MainModule.GetType("<Module>").GetStaticConstructor Is Nothing Then
                        found = True
                        Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
                    End If
                End If

                'Dim TDef As IEnumerable(Of TypeDefinition) = asmDef.Modules.SelectMany(Function(modDef) modDef.Types).Where(Function(typeDef) typeDef.Namespace = "").Where(Function(typeDef) Not typeDef.Name.ToString.StartsWith("<"))
                'If TDef.Count > 1 Then
                '    found = True
                '    Return New Cls_IdentifierResult(Cls_IdentifierEnum.ResultName.Unknown, Cls_IdentifierEnum.ResultType.Other)
                'End If
            Catch
                found = True
                Return New IdentifierResult(IdentifierEnum.ResultName.Unknown, IdentifierEnum.ResultType.Other, My.Resources.Warning)
            End Try
            found = False
            Return New IdentifierResult(IdentifierEnum.ResultName.Unidentified, IdentifierEnum.ResultType.Empty, My.Resources.Valid)
        End Function
#End Region

    End Class
End Namespace
