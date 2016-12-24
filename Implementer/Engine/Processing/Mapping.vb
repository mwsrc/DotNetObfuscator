Imports Mono.Cecil
Imports Mono.Cecil.Cil
Imports Implementer.Engine.Context

Namespace Engine.Processing
    ''' <summary>
    ''' INFO : This is the fith step of the renamer library. 
    '''        This will check the existence of the name and if true will return the associated renamed value.
    '''        This class is able to clean dictionaries when the renaming process is complete.
    ''' </summary>
    Public NotInheritable Class Mapping

#Region " Fields "
        Private Shared m_ObfNamespaces As New Dictionary(Of String, String)
        Private Shared m_ObfTypes As New Dictionary(Of TypeDefinition, String)
        Private Shared m_ObfMethods As New Dictionary(Of MethodDefinition, String)
        Private Shared m_ObfParameters As New Dictionary(Of ParameterDefinition, String)
        Private Shared m_ObfGenericParameters As New Dictionary(Of GenericParameter, String)
        Private Shared m_ObfVariables As New Dictionary(Of VariableDefinition, String)
        Private Shared m_ObfProperties As New Dictionary(Of PropertyDefinition, String)
        Private Shared m_ObfEvents As New Dictionary(Of EventDefinition, String)
        Private Shared m_ObfFields As New Dictionary(Of FieldDefinition, String)
#End Region

#Region " Methods "

        ''' <summary>
        ''' INFO : Store Key/Value pair (TypeDefinition/ObfuscatedName and third arg set to True if this is a namespace) to dictionary only if key not exists. Return NamespaceObfuscated value.
        ''' </summary>
        ''' <param name="Type"></param>
        ''' <param name="NamespaceObfuscated"></param>
        Friend Shared Function RenameTypeDef(Type As TypeDefinition, ByRef NamespaceObfuscated$, Optional ByVal isNamespace As Boolean = False) As String
            If isNamespace Then
                If Not m_ObfNamespaces.ContainsKey(Type.Namespace) Then
                    m_ObfNamespaces.Add(Type.Namespace, NamespaceObfuscated)
                    Tasks.RaiseRenamedItemEvent(New RenamedItem(RenamedItemType.ItemType.Namespaces, Type.Namespace, NamespaceObfuscated))
                Else
                    NamespaceObfuscated = m_ObfNamespaces.Item(Type.Namespace)
                End If
            Else
                If Not m_ObfTypes.ContainsKey(Type) Then
                    m_ObfTypes.Add(Type, NamespaceObfuscated)
                    Tasks.RaiseRenamedItemEvent(New RenamedItem(RenamedItemType.ItemType.Types, Type.Name, NamespaceObfuscated))
                Else
                    NamespaceObfuscated = m_ObfTypes.Item(Type)
                End If
            End If
            Return NamespaceObfuscated
        End Function

        ''' <summary>
        ''' INFO : Store Key/Value pair (MethodDefinition/ObfuscatedName) to dictionary only if key not exists. Return MemberObfuscated value.
        ''' </summary>
        ''' <param name="Member"></param>
        ''' <param name="MemberObfuscated"></param>
        Friend Shared Function RenameMethodMember(Member As MethodDefinition, ByRef MemberObfuscated$) As String
            If Not m_ObfMethods.ContainsKey(Member) Then
                m_ObfMethods.Add(Member, MemberObfuscated)
                Tasks.RaiseRenamedItemEvent(New RenamedItem(RenamedItemType.ItemType.Methods, Member.Name, MemberObfuscated))
            Else
                MemberObfuscated = m_ObfMethods.Item(Member)
            End If
            Return MemberObfuscated
        End Function

        ''' <summary>
        ''' INFO : Store Key/Value pair (Parameter/ObfuscatedName) to dictionary only if key not exists. Return MemberObfuscated value.
        ''' </summary>
        ''' <param name="Member"></param>
        ''' <param name="MemberObfuscated"></param>
        Friend Shared Function RenameParamMember(Member As ParameterDefinition, ByRef MemberObfuscated$) As String
            If Not m_ObfParameters.ContainsKey(Member) Then
                m_ObfParameters.Add(Member, MemberObfuscated)
                Tasks.RaiseRenamedItemEvent(New RenamedItem(RenamedItemType.ItemType.Parameters, Member.Name, MemberObfuscated))
            Else
                MemberObfuscated = m_ObfParameters.Item(Member)
            End If
            Return MemberObfuscated
        End Function

        ''' <summary>
        ''' INFO : Store Key/Value pair (GenericParameter/ObfuscatedName) to dictionary only if key not exists. Return MemberObfuscated value.
        ''' </summary>
        ''' <param name="Member"></param>
        ''' <param name="MemberObfuscated"></param>
        Friend Shared Function RenameGenericParamMember(Member As GenericParameter, ByRef MemberObfuscated$) As String
            If Not m_ObfGenericParameters.ContainsKey(Member) Then
                m_ObfGenericParameters.Add(Member, MemberObfuscated)
                Tasks.RaiseRenamedItemEvent(New RenamedItem(RenamedItemType.ItemType.GenericParameters, Member.Name, MemberObfuscated))
            Else
                MemberObfuscated = m_ObfGenericParameters.Item(Member)
            End If
            Return MemberObfuscated
        End Function

        ''' <summary>
        ''' INFO : Store Key/Value pair (VariableDefinition/ObfuscatedName) to dictionary only if key not exists. Return MemberObfuscated value.
        ''' </summary>
        ''' <param name="Member"></param>
        ''' <param name="MemberObfuscated"></param>
        Friend Shared Function RenameVariableMember(Member As VariableDefinition, ByRef MemberObfuscated$) As String
            If Not m_ObfVariables.ContainsKey(Member) Then
                m_ObfVariables.Add(Member, MemberObfuscated)
                Tasks.RaiseRenamedItemEvent(New RenamedItem(RenamedItemType.ItemType.Variables, Member.ToString, MemberObfuscated))
            Else
                MemberObfuscated = m_ObfVariables.Item(Member)
            End If
            Return MemberObfuscated
        End Function

        ''' <summary>
        ''' INFO : Store Key/Value pair (PropertydDefinition/ObfuscatedName) to dictionary only if key not exists. Return MemberObfuscated value.
        ''' </summary>
        ''' <param name="Member"></param>
        ''' <param name="MemberObfuscated"></param>
        Friend Shared Function RenamePropertyMember(Member As PropertyDefinition, ByRef MemberObfuscated$) As String
            If Not m_ObfProperties.ContainsKey(Member) Then
                m_ObfProperties.Add(Member, MemberObfuscated)
                Tasks.RaiseRenamedItemEvent(New RenamedItem(RenamedItemType.ItemType.Properties, Member.Name, MemberObfuscated))
            Else
                MemberObfuscated = m_ObfProperties.Item(Member)
            End If
            Return MemberObfuscated
        End Function

        ''' <summary>
        ''' INFO : Store Key/Value pair (EventDefinition/ObfuscatedName) to dictionary only if key not exists. Return MemberObfuscated value.
        ''' </summary>
        ''' <param name="Member"></param>
        ''' <param name="MemberObfuscated"></param>
        Friend Shared Function RenameEventMember(Member As EventDefinition, ByRef MemberObfuscated$) As String
            If Not m_ObfEvents.ContainsKey(Member) Then
                m_ObfEvents.Add(Member, MemberObfuscated)
                Tasks.RaiseRenamedItemEvent(New RenamedItem(RenamedItemType.ItemType.Events, Member.Name, MemberObfuscated))
            Else
                MemberObfuscated = m_ObfEvents.Item(Member)
            End If
            Return MemberObfuscated
        End Function

        ''' <summary>
        ''' INFO : Store Key/Value pair (FieldDefinition/ObfuscatedName) to dictionary only if key not exists. Return MemberObfuscated value.
        ''' </summary>
        ''' <param name="Member"></param>
        ''' <param name="MemberObfuscated"></param>
        Friend Shared Function RenameFieldMember(Member As FieldDefinition, ByRef MemberObfuscated$) As String
            If Not m_ObfFields.ContainsKey(Member) Then
                m_ObfFields.Add(Member, MemberObfuscated)
                Tasks.RaiseRenamedItemEvent(New RenamedItem(RenamedItemType.ItemType.Fields, Member.Name, MemberObfuscated))
            Else
                MemberObfuscated = m_ObfFields.Item(Member)
            End If
            Return MemberObfuscated
        End Function

        ''' <summary>
        ''' INFO : CleanUp Namespaces dictionary and MethodReferences List.
        ''' </summary>
        Friend Shared Sub CleanUp()
            If Not m_ObfNamespaces.Count <> 0 Then m_ObfNamespaces.Clear()
            If Not m_ObfTypes.Count <> 0 Then m_ObfTypes.Clear()
            If Not m_ObfMethods.Count <> 0 Then m_ObfMethods.Clear()
            If Not m_ObfGenericParameters.Count <> 0 Then m_ObfGenericParameters.Clear()
            If Not m_ObfParameters.Count <> 0 Then m_ObfParameters.Clear()
            If Not m_ObfVariables.Count <> 0 Then m_ObfVariables.Clear()
            If Not m_ObfProperties.Count <> 0 Then m_ObfProperties.Clear()
            If Not m_ObfEvents.Count <> 0 Then m_ObfEvents.Clear()
            If Not m_ObfFields.Count <> 0 Then m_ObfFields.Clear()
        End Sub
#End Region

    End Class
End Namespace


