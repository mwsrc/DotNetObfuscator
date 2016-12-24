
Namespace Core.Versions
    Public Class Infos
        Implements IDisposable

#Region " Fields "
        Private m_Enabled As Boolean
        Private m_FileDescription As String
        Private m_Comments As String
        Private m_CompanyName As String
        Private m_ProductName As String
        Private m_LegalCopyright As String
        Private m_LegalTrademarks As String
        Private m_FileVersion As String
        Private m_ProductVersion As String
#End Region

#Region " Properties "

        Public ReadOnly Property Enabled As Boolean
            Get
                Return m_Enabled
            End Get
        End Property

        Public ReadOnly Property FileDescription As String
            Get
                Return m_FileDescription
            End Get
        End Property

        Public ReadOnly Property Comments As String
            Get
                Return m_Comments
            End Get
        End Property

        Public ReadOnly Property CompanyName As String
            Get
                Return m_CompanyName
            End Get
        End Property

        Public ReadOnly Property ProductName As String
            Get
                Return m_ProductName
            End Get
        End Property

        Public ReadOnly Property LegalCopyright As String
            Get
                Return m_LegalCopyright
            End Get
        End Property

        Public ReadOnly Property LegalTrademarks As String
            Get
                Return m_LegalTrademarks
            End Get
        End Property

        Public ReadOnly Property FileVersion As String
            Get
                Return checkVersion(m_FileVersion)
            End Get
        End Property

        Public ReadOnly Property ProductVersion As String
            Get
                Return checkVersion(m_ProductVersion)
            End Get
        End Property
#End Region

#Region " Constructor "
        Public Sub New(Enabl As Boolean, FileDescript$, Comment$, CompanyN$, ProductN$, LegalCopy$, LegalTrade$, FileV$, ProductV$)
            m_Enabled = Enabl
            m_FileDescription = FileDescript
            m_Comments = Comment
            m_CompanyName = CompanyN
            m_ProductName = ProductN
            m_LegalCopyright = LegalCopy
            m_LegalTrademarks = LegalTrade
            m_FileVersion = FileV
            m_ProductVersion = ProductV
        End Sub

        Public Sub New(Enabl As Boolean, FilePath$)
            Dim fvi As FileVersionInfo = FileVersionInfo.GetVersionInfo(FilePath)
            m_Enabled = Enabl
            m_FileDescription = fvi.FileDescription
            m_Comments = fvi.Comments
            m_CompanyName = fvi.CompanyName
            m_ProductName = fvi.ProductName
            m_LegalCopyright = fvi.LegalCopyright
            m_LegalTrademarks = fvi.LegalTrademarks
            m_FileVersion = fvi.FileVersion
            m_ProductVersion = fvi.ProductVersion
        End Sub
#End Region

#Region " Methods "

        Private Sub CleanUp()
            m_Enabled = False
            m_FileDescription = String.Empty
            m_Comments = String.Empty
            m_CompanyName = String.Empty
            m_ProductName = String.Empty
            m_LegalCopyright = String.Empty
            m_LegalTrademarks = String.Empty
            m_FileVersion = String.Empty
            m_ProductVersion = String.Empty
        End Sub

        Private Function checkVersion(VersionValue$) As String
            If (VersionValue <> String.Empty) Then
                If Not VersionValue.Contains(".") Then Return "0.0.0.0"
                If (VersionValue.Split(New Char() {"."c}).Length = 4) Then Return VersionValue
            End If
            Return "0.0.0.0"
        End Function

#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                End If
                CleanUp()
            End If
            Me.disposedValue = True
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
