Namespace Engine.Context

    Public NotInheritable Class RenamedItem

#Region " Fields "
        Private ReadOnly m_itemType As RenamedItemType.ItemType
        Private ReadOnly m_itemName$
        Private ReadOnly m_obfuscatedItemName$
#End Region

#Region " Constructor "
        Friend Sub New(ItemType As RenamedItemType.ItemType, ItemName$, obfuscatedItemName$)
            m_itemType = ItemType
            m_itemName = ItemName
            m_obfuscatedItemName = obfuscatedItemName
        End Sub
#End Region

#Region " Properties "
        Public ReadOnly Property ItemType As String
            Get
                Return TypeToString(Me.m_itemType)
            End Get
        End Property

        Public ReadOnly Property ItemName As String
            Get
                Return m_itemName
            End Get
        End Property

        Public ReadOnly Property obfuscatedItemName As String
            Get
                Return m_obfuscatedItemName
            End Get
        End Property
#End Region

#Region " Methods "
        Private Function TypeToString(ItemType%) As String
            Select Case ItemType
                Case 0
                    Return "Namespace"
                    Exit Select
                Case 1
                    Return "Type"
                    Exit Select
                Case 2
                    Return "Method"
                    Exit Select
                Case 3
                    Return "Parameter"
                    Exit Select
                Case 4
                    Return "Generic Parameter"
                    Exit Select
                Case 5
                    Return "Variable"
                    Exit Select
                Case 6
                    Return "Property"
                    Exit Select
                Case 7
                    Return "Event"
                    Exit Select
                Case 8
                    Return "Field"
                    Exit Select
            End Select
            Return Nothing
        End Function
#End Region

    End Class
End Namespace
