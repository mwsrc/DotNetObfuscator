Imports System.Windows.Forms

Namespace Engine.Checking

#Region " Delegates "
    Public Delegate Sub Check(sender As Object, e As CheckEventArgs)
#End Region

    Public NotInheritable Class CheckEventArgs
        Inherits EventArgs

#Region " Fields "
        Private m_message As String
        Private m_title As String
        Private m_checkedFile As String
#End Region

#Region " Properties "
        Public ReadOnly Property message As String
            Get
                Return m_message
            End Get
        End Property

        Public ReadOnly Property title As String
            Get
                Return m_title
            End Get
        End Property

        Public ReadOnly Property checkedFile As String
            Get
                Return m_checkedFile
            End Get
        End Property
#End Region

#Region " Constructor "
        Public Sub New(message As String, title As String, checkedFile As String)
            m_message = message
            m_title = title
            m_checkedFile = checkedFile
        End Sub
#End Region

    End Class
End Namespace
