Imports Mono.Cecil

Namespace CecilHelper
    Public NotInheritable Class Finder

#Region " Methods "
        Public Shared Function FindCustomAttributeByName(member As MethodDefinition, CaName$) As Boolean
            Return Enumerable.Any(Of CustomAttribute)(member.CustomAttributes, Function(ca) ca.AttributeType.Name = CaName)
        End Function

        Public Shared Function FindCustomAttributeByName(member As TypeDefinition, CaName$) As Boolean
            Return Enumerable.Any(Of CustomAttribute)(member.CustomAttributes, Function(ca) ca.AttributeType.Name = CaName)
        End Function

        Public Shared Function FindCustomAttributeByName(member As AssemblyDefinition, CaName$) As Boolean
            Return Enumerable.Any(Of CustomAttribute)(member.CustomAttributes, Function(ca) ca.AttributeType.Name = CaName)
        End Function

        Public Shared Function FindGenericParameter(member As MethodDefinition) As Boolean
            If Not member Is Nothing AndAlso Not member.ReturnType Is Nothing AndAlso member.ReturnType.IsGenericParameter Then
                Return True
            End If
            Return False
        End Function

        Public Shared Function FindType(moduleDef As ModuleDefinition, Name As String, Optional ByVal Full As Boolean = False) As TypeDefinition
            For Each typeDef As TypeDefinition In moduleDef.Types
                Dim returnType As TypeDefinition = Nothing

                If Full Then
                    If typeDef.FullName = Name Then
                        Return typeDef
                    End If
                Else
                    If typeDef.Name = Name Then
                        Return typeDef
                    End If
                End If

                returnType = FindNestedType(typeDef, Name)

                If returnType IsNot Nothing Then
                    Return returnType
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' Recursive search for nested types
        ''' </summary>
        ''' <param name="parentType"></param>
        ''' <param name="fullname"></param>
        ''' <returns></returns>
        Private Shared Function FindNestedType(parentType As TypeDefinition, fullname As String) As TypeDefinition
            For Each type In parentType.NestedTypes
                If type.FullName = fullname Then
                    Return type
                End If

                If type.HasNestedTypes Then
                    Return FindNestedType(type, fullname)
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' by The Unknown Programmer
        ''' </summary>
        ''' <param name="parentDef"></param>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Shared Function FindType(parentDef As TypeDefinition, name As String) As TypeDefinition
            Return parentDef.NestedTypes.First(Function(t) t.Name = name)
        End Function

        Public Shared Function FindMethod(assDef As AssemblyDefinition, name As String) As MethodDefinition
            For Each t In assDef.MainModule.GetTypes
                If t.HasMethods Then
                    For Each methodDef As MethodDefinition In t.Methods
                        If methodDef.Name = name Then
                            Return methodDef
                        End If
                    Next
                End If
            Next
            Return Nothing
        End Function

        Public Shared Function FindMethod(typeDef As TypeDefinition, name As String) As MethodDefinition
            For Each methodDef As MethodDefinition In typeDef.Methods
                If methodDef.Name = name Then
                    Return methodDef
                End If
            Next
            Return Nothing
        End Function

        Public Shared Function FindField(parentType As TypeDefinition, name As String) As FieldDefinition
            Return parentType.Fields.First(Function(f) f.Name = name)
        End Function

        Public Shared Function FindDefaultNamespace(assDef As AssemblyDefinition) As String
            Dim NamespaceDefault = assDef.MainModule.EntryPoint.DeclaringType.Namespace
            If NamespaceDefault.EndsWith(".My") Then
                NamespaceDefault = NamespaceDefault.Split(".")(0)
            End If
            Return NamespaceDefault
        End Function

        Public Shared Function FindDefaultNamespace(assDef As AssemblyDefinition, Pack As Boolean) As String
            Return If(Pack = True, String.Empty, assDef.MainModule.EntryPoint.DeclaringType.Namespace)
        End Function

        Public Shared Function frameworkVersion(assDef As AssemblyDefinition) As String
            Return If(assDef.MainModule.Runtime.ToString.StartsWith("Net_4"), "v4.0", "v2.0")
        End Function

        Public Shared Function AccessorMethods(ByVal type As TypeDefinition) As List(Of MethodDefinition)
            Dim list As New List(Of MethodDefinition)
            For Each Pdef In type.Properties
                list.Add(Pdef.GetMethod)
                list.Add(Pdef.SetMethod)
                If Pdef.HasOtherMethods Then
                    For Each oDef In Pdef.OtherMethods
                        list.Add(oDef)
                    Next
                End If
            Next
            For Each eDef In type.Events
                list.Add(eDef.AddMethod)
                list.Add(eDef.RemoveMethod)
                list.Add(eDef.InvokeMethod)
                If eDef.HasOtherMethods Then
                    For Each oDef In eDef.OtherMethods
                        list.Add(oDef)
                    Next
                End If
            Next
            Return list
        End Function
#End Region
     
    End Class
End Namespace
