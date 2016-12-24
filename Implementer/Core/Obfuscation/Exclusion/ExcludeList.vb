Imports Mono.Cecil
Imports Mono.Cecil.Rocks

Namespace Core.Obfuscation.Exclusion
    Public NotInheritable Class ExcludeList

#Region " Fields "
        Private m_ObfTypes As New Dictionary(Of TypeDefinition, ExclusionState)
        Private m_ObfMethods As New Dictionary(Of MethodDefinition, ExclusionState)
        Private m_ObfProperties As New Dictionary(Of PropertyDefinition, ExclusionState)
        Private m_ObfEvents As New Dictionary(Of EventDefinition, ExclusionState)
        Private m_ObfFields As New Dictionary(Of FieldDefinition, ExclusionState)
#End Region

#Region " Methods "
        Public Sub AddTo(m As Object)
            If Not m Is Nothing AndAlso Not m.member Is Nothing Then
                If m.memberType = ExclusionState.mType.Types Then
                    If Not m_ObfTypes.ContainsKey(m.member) Then m_ObfTypes.Add(m.member, m)
                ElseIf m.memberType = ExclusionState.mType.Methods Then
                    If Not m_ObfMethods.ContainsKey(m.member) Then m_ObfMethods.Add(m.member, m)
                ElseIf m.memberType = ExclusionState.mType.Properties Then
                    If Not m_ObfProperties.ContainsKey(m.member) Then m_ObfProperties.Add(m.member, m)
                ElseIf m.memberType = ExclusionState.mType.Events Then
                    If Not m_ObfEvents.ContainsKey(m.member) Then m_ObfEvents.Add(m.member, m)
                ElseIf m.memberType = ExclusionState.mType.Fields Then
                    If Not m_ObfFields.ContainsKey(m.member) Then m_ObfFields.Add(m.member, m)
                End If
            End If
        End Sub

        Public Sub RemoveFrom(m As Object)
            If Not m Is Nothing AndAlso Not m.member Is Nothing Then
                If m.memberType = ExclusionState.mType.Types Then
                    If m_ObfTypes.ContainsKey(m.member) Then m_ObfTypes.Remove(m.member)
                ElseIf m.memberType = ExclusionState.mType.Methods Then
                    If m_ObfMethods.ContainsKey(m.member) Then m_ObfMethods.Remove(m.member)
                ElseIf m.memberType = ExclusionState.mType.Properties Then
                    If m_ObfProperties.ContainsKey(m.member) Then m_ObfProperties.Remove(m.member)
                ElseIf m.memberType = ExclusionState.mType.Events Then
                    If m_ObfEvents.ContainsKey(m.member) Then m_ObfEvents.Remove(m.member)
                ElseIf m.memberType = ExclusionState.mType.Fields Then
                    If m_ObfFields.ContainsKey(m.member) Then m_ObfFields.Remove(m.member)
                End If
            End If
        End Sub

        Public Sub CleanUp()
            m_ObfTypes.Clear()
            m_ObfMethods.Clear()
            m_ObfProperties.Clear()
            m_ObfEvents.Clear()
            m_ObfFields.Clear()
        End Sub

        Friend Function isRenamingExclude(m As TypeDefinition) As Boolean
            Return m_ObfTypes.Any(Function(x) x.Key.FullName = m.FullName AndAlso x.Value.Renaming = True)
        End Function

        Friend Function isStringEncryptExclude(m As TypeDefinition) As Boolean
            Return m_ObfTypes.Any(Function(x) x.Key.FullName = m.FullName AndAlso x.Value.stringEncrypt = True)
        End Function

        Friend Function isIntegerEncodExclude(m As TypeDefinition) As Boolean
            Return m_ObfTypes.Any(Function(x) x.Key.FullName = m.FullName AndAlso x.Value.integerEncoding = True)
        End Function

        Friend Function isBooleanEncryptExclude(m As TypeDefinition) As Boolean
            Return m_ObfTypes.Any(Function(x) x.Key.FullName = m.FullName AndAlso x.Value.booleanEncrypt = True)
        End Function

        Friend Function isInvalidOpcodesExclude(m As TypeDefinition) As Boolean
            Return m_ObfTypes.Any(Function(x) x.Key.FullName = m.FullName AndAlso x.Value.InvalidOpcodes = True)
        End Function

        Friend Function isHideCallsExclude(m As TypeDefinition) As Boolean
            Return m_ObfTypes.Any(Function(x) x.Key.FullName = m.FullName AndAlso x.Value.hideCalls = True)
        End Function

        Public Function itemsCount() As Integer
            Return m_ObfTypes.Count + m_ObfMethods.Count + m_ObfProperties.Count + m_ObfEvents.Count + m_ObfFields.Count
        End Function

        Public Function TypesCount() As String
            Return m_ObfTypes.Count.ToString
        End Function

        Public Function MethodsCount() As String
            Return m_ObfMethods.Count.ToString
        End Function

        Public Function PropertiesCount() As String
            Return m_ObfProperties.Count.ToString
        End Function

        Public Function FieldsCount() As String
            Return m_ObfFields.Count.ToString
        End Function

        Public Function EventsCount() As String
            Return m_ObfEvents.Count.ToString
        End Function

#End Region

    End Class
End Namespace
