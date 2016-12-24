Imports Mono.Cecil

Namespace CecilHelper
    Public NotInheritable Class NameChecker

#Region " Methods "
        ''' <summary>
        ''' INFO : Verifying if typeDefinition is renamable
        ''' </summary>
        ''' <param name="type"></param>
        Public Shared Function IsRenamable(type As TypeDefinition) As Boolean
            If Not type.BaseType Is Nothing Then
                If type.BaseType.IsArray Then
                    Return False
                End If
            End If
            Return Not type.FullName = "<Module>" AndAlso Not type.IsImport
        End Function

        ''' <summary>
        ''' INFO : Verifying if methodDefinition is renamable
        ''' </summary>
        ''' <param name="method"></param>
        Public Shared Function IsRenamable(method As MethodDefinition, Optional ByVal Force As Boolean = False) As Boolean
            If Force Then
                If method.HasBody Then
                    If Finder.AccessorMethods(method.DeclaringType).Contains(method) Then
                        Return Not Finder.FindGenericParameter(method) AndAlso Not Finder.FindCustomAttributeByName(method, "DebuggerHiddenAttribute")
                    End If
                End If
            End If
            Return method IsNot Nothing AndAlso Not (method.IsRuntimeSpecialName OrElse method.IsRuntime OrElse method.IsSpecialName OrElse method.IsConstructor OrElse method.HasOverrides OrElse method.IsVirtual OrElse method.IsAbstract OrElse method.Name.EndsWith("GetEnumerator"))
        End Function

        ''' <summary>
        ''' INFO : Verifying if eventDefinition is renamable
        ''' </summary>
        ''' <param name="Events"></param>
        Public Shared Function IsRenamable(ByVal Events As EventDefinition) As Boolean
            Return If(Not Events.IsSpecialName OrElse Not Events.IsRuntimeSpecialName OrElse Not Events.IsDefinition, True, False)
        End Function

        ''' <summary>
        ''' INFO : Verifying if propertyDefinition is renamable
        ''' </summary>
        ''' <param name="prop"></param>
        Public Shared Function IsRenamable(prop As PropertyDefinition) As Boolean
            Return Not prop.IsRuntimeSpecialName OrElse Not prop.IsSpecialName
        End Function

        ''' <summary>
        ''' INFO : Verifying if fieldDefinition is renamable
        ''' </summary>
        ''' <param name="field"></param>
        Public Shared Function IsRenamable(field As FieldDefinition) As Boolean
            If (Not field.IsRuntimeSpecialName AndAlso Not field.DeclaringType.HasGenericParameters) And Not field.IsPInvokeImpl AndAlso Not field.IsSpecialName Then
                Return True
            End If
            Return False
        End Function
#End Region

    End Class
End Namespace

