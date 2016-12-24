Imports System.IO
Imports Mono.Cecil

Namespace Core.Packer
    Public NotInheritable Class ZipInfos

#Region " Fields "
        Private m_fPath As String
        Private m_refByte As Byte()
        Private m_refNewNamespaceName As String
        Private m_refNewTypeName As String
        Private m_refNewMethodName As String
#End Region

#Region " Properties "
        Friend ReadOnly Property fPath As String
            Get
                Return m_fPath
            End Get
        End Property

        Friend ReadOnly Property refByte As Byte()
            Get
                Return m_refByte
            End Get
        End Property

        Friend ReadOnly Property refNewNamespaceName As String
            Get
                Return m_refNewNamespaceName
            End Get
        End Property

        Friend ReadOnly Property refNewTypeName As String
            Get
                Return m_refNewTypeName
            End Get
        End Property

        Friend ReadOnly Property refNewMethodName As String
            Get
                Return m_refNewMethodName
            End Get
        End Property
#End Region

#Region " Constructor "

        Friend Sub New(filePath$, rByte As Byte(), refNewNamespaceName$, refNewTypeName$, refNewMethodName$)
            m_fPath = filePath
            m_refByte = rByte
            m_refNewNamespaceName = refNewNamespaceName
            m_refNewTypeName = refNewTypeName
            m_refNewMethodName = refNewMethodName
        End Sub

#End Region

    End Class
End Namespace
