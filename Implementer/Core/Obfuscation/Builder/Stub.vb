Imports Mono.Cecil
Imports Mono.Cecil.Rocks
Imports Helper.CecilHelper
Imports System.IO
Imports Mono.Cecil.Cil
Imports Helper.RandomizeHelper
Imports Implementer.Engine.Processing
Imports Implementer.Core.Packer

Namespace Core.Obfuscation.Builder

    Public Class Stub
        Implements IDisposable

#Region " Fields "
        Private m_assDef As AssemblyDefinition = Nothing
        Private m_assDefTarget As AssemblyDefinition = Nothing
        Private m_typeDefResolver As TypeDefinition = Nothing
        Private m_resolverDll As String = String.Empty
#End Region

#Region " Properties "
        Private m_className = String.Empty
        Friend ReadOnly Property className As String
            Get
                Return m_className
            End Get
        End Property

        Private m_funcName1 = String.Empty
        Friend ReadOnly Property funcName1 As String
            Get
                Return m_funcName1
            End Get
        End Property

        Private m_funcName2 = String.Empty
        Friend ReadOnly Property funcName2 As String
            Get
                Return m_funcName2
            End Get
        End Property

        Private m_funcName3 = String.Empty
        Friend ReadOnly Property funcName3 As String
            Get
                Return m_funcName3
            End Get
        End Property

        Private m_funcName4 = String.Empty
        Friend ReadOnly Property funcName4 As String
            Get
                Return m_funcName4
            End Get
        End Property

        Private m_resolvedTypeDef As TypeDefinition = Nothing
        Friend ReadOnly Property resolvedTypeDef As TypeDefinition
            Get
                Return m_resolvedTypeDef
            End Get
        End Property

        Public Property ReferencedZipperAssembly As ZipInfos

#End Region

#Region " Constructor "
        Friend Sub New(className$, FuncName1$, Optional ByVal FuncName2 As String = Nothing, Optional ByVal FuncName3 As String = Nothing, Optional ByVal FuncName4 As String = Nothing)
            m_className = className
            m_funcName1 = FuncName1
            m_funcName2 = FuncName2
            m_funcName3 = FuncName3
            m_funcName4 = FuncName4
        End Sub
#End Region

#Region " Methods "
        Friend Function ResolveTypeFromFile(ResolverDll$, Optional ByVal replaceNamespace As String = "", _
                                                          Optional ByVal replaceClassName As String = "", _
                                                          Optional ByVal FuncNewName1 As String = "", _
                                                          Optional ByVal FuncNewName2 As String = "", _
                                                          Optional ByVal FuncNewName3 As String = "", _
                                                          Optional ByVal FuncNewName4 As String = "") As TypeDefinition

            If Not ResolverDll.ToLower.EndsWith(".exe") Then
                If Not ResolverDll.ToLower.EndsWith(".dll") Then
                    ResolverDll &= ".dll"
                End If
            End If
            m_resolverDll = ResolverDll
            m_assDef = AssemblyDefinition.ReadAssembly(ResolverDll)
            m_typeDefResolver = Finder.FindType(m_assDef.MainModule, m_className)

            If Not replaceNamespace = "" Then m_typeDefResolver.Namespace = replaceNamespace
            If Not replaceClassName = "" Then m_typeDefResolver.Name = replaceClassName

            If Not FuncNewName1 = "" Then
                Finder.FindMethod(m_typeDefResolver, m_funcName1).Name = FuncNewName1
                m_funcName1 = FuncNewName1
            End If
            If Not FuncNewName2 = "" Then
                Finder.FindMethod(m_typeDefResolver, m_funcName2).Name = FuncNewName2
                m_funcName2 = FuncNewName2
            End If
            If Not FuncNewName3 = "" Then
                Finder.FindMethod(m_typeDefResolver, m_funcName3).Name = FuncNewName3
                m_funcName3 = FuncNewName3
            End If
            If Not FuncNewName4 = "" Then
                Finder.FindMethod(m_typeDefResolver, m_funcName4).Name = FuncNewName4
                m_funcName4 = FuncNewName4
            End If

            Return m_typeDefResolver
        End Function

        Friend Function InjectType(assDefTarget As AssemblyDefinition) As TypeDefinition
            m_assDefTarget = assDefTarget
            m_resolvedTypeDef = Injecter.Inject(assDefTarget.MainModule, m_typeDefResolver)
            assDefTarget.MainModule.Types.Add(m_resolvedTypeDef)
            Return m_resolvedTypeDef
        End Function

        Friend Sub InjectToCctor(assDefTarget As AssemblyDefinition)
            m_assDefTarget = assDefTarget
            m_resolvedTypeDef = Injecter.Inject(assDefTarget.MainModule, m_typeDefResolver)

            Dim globalType = m_assDefTarget.MainModule.GetType("<Module>")

            Dim cctorMethod = globalType.GetStaticConstructor
            If cctorMethod Is Nothing Then
                globalType.Methods.Add(Injecter.CreateGenericCctor(m_assDefTarget))
                cctorMethod = globalType.GetStaticConstructor
            End If

            If cctorMethod.Body.Instructions.Count > 0 AndAlso cctorMethod.Body.Instructions.Last.OpCode = OpCodes.Ret Then
                cctorMethod.Body.Instructions.Remove(cctorMethod.Body.Instructions.Last)
            End If

            m_assDefTarget.MainModule.Types.Add(m_resolvedTypeDef)
            Dim initializeMethod = Finder.FindMethod(m_assDefTarget, m_funcName1)

            If Not initializeMethod Is Nothing Then
                If Not cctorMethod Is Nothing Then
                    Dim ilproc = cctorMethod.Body.GetILProcessor()
                    Dim last = ilproc.Body.Instructions.Count
                    Dim init = ilproc.Create(OpCodes.Call, initializeMethod)
                    cctorMethod.Body.Instructions.Insert(last, init)
                    ilproc.InsertAfter(init, ilproc.Create(Mono.Cecil.Cil.OpCodes.Ret))
                End If
            End If
        End Sub

        Friend Sub InjectToMyProject()
            If Utils.IsDebuggerNonUserCode(m_assDefTarget) Then
                Dim myProjectType = Finder.FindType(m_assDefTarget.MainModule, Finder.FindDefaultNamespace(m_assDefTarget) & ".My.MyProject", True)

                If Not myProjectType Is Nothing Then
                    Dim cctr = myProjectType.GetStaticConstructor
                    If Not cctr Is Nothing Then

                        If cctr.HasBody Then
                            If cctr.Body.Instructions.Count <> 0 Then
                                Dim originalMeth = Finder.FindMethod(m_typeDefResolver, m_funcName1)
                                If Not originalMeth Is Nothing Then

                                    If cctr.Body.Instructions.Last.OpCode = OpCodes.Ret Then cctr.Body.Instructions.Remove(cctr.Body.Instructions.Last)
                                    Dim ImportedMeth = Injecter.Inject(m_assDefTarget.MainModule, originalMeth)
                                    myProjectType.Methods.Add(ImportedMeth)

                                    Dim ilproc = cctr.Body.GetILProcessor()
                                    Dim last = ilproc.Body.Instructions.Count
                                    Dim init = ilproc.Create(OpCodes.Call, ImportedMeth)
                                    cctr.Body.Instructions.Insert(last, init)
                                    ilproc.InsertAfter(init, ilproc.Create(Mono.Cecil.Cil.OpCodes.Ret))
                                End If

                            End If
                        End If
                    End If
                End If
            End If
        End Sub

        Friend Function GetMethod1() As MethodDefinition
            Return Finder.FindMethod(m_assDefTarget, m_funcName1)
        End Function

        Friend Function GetMethod2() As MethodDefinition
            Return Finder.FindMethod(m_assDefTarget, m_funcName2)
        End Function

        Friend Function GetMethod3() As MethodDefinition
            Return Finder.FindMethod(m_assDefTarget, m_funcName3)
        End Function

        Friend Function GetMethod4() As MethodDefinition
            Return Finder.FindMethod(m_assDefTarget, m_funcName4)
        End Function

        Friend Sub DeleteDll()
            If File.Exists(m_resolverDll) Then
                Try
                    System.IO.File.Delete(m_resolverDll)
                Catch ex As Exception
                End Try
            End If
        End Sub
#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                End If
                Try
                    m_assDef = Nothing
                    File.Delete(m_resolverDll)
                Catch ex As Exception
                End Try
            End If
            Me.disposedValue = True
        End Sub

        Friend Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace