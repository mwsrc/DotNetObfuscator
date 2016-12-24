Namespace Engine.Context

#Region " Delegates "
    Public Delegate Sub RenamedItemDelegate(sender As Object, e As RenamedItemEventArgs)
#End Region

    Public NotInheritable Class RenamedItemEventArgs
        Inherits EventArgs

#Region " Fields "
        Private m_item As RenamedItem
#End Region

#Region " Constructor "
        Public Sub New(item As RenamedItem)
            m_item = item
        End Sub
#End Region

#Region " Properties "
        Public ReadOnly Property item As RenamedItem
            Get
                Return m_item
            End Get
        End Property
#End Region

    End Class
End Namespace
