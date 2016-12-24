Imports System.IO
Imports Vestris.ResourceLib
Imports System.Xml

Namespace Core.ManifestRequest
    Public Class ManifestInfos
        Implements IDisposable

#Region " Properties "
        Private m_LastRequested As String
        Public ReadOnly Property LastRequested As String
            Get
                Return m_LastRequested
            End Get
        End Property

        Private m_NewRequested As String
        Public ReadOnly Property NewRequested As String
            Get
                Return m_NewRequested
            End Get
        End Property

        Public ReadOnly Property Modified() As Boolean
            Get
                Return If(m_LastRequested <> "" AndAlso m_NewRequested <> "", m_LastRequested <> m_NewRequested, False)
            End Get
        End Property
#End Region

#Region " Constructor "
        Public Sub New(LastRequest$, NewRequest$)
            m_LastRequested = LastRequest
            m_NewRequested = NewRequest
        End Sub
#End Region

#Region " Methods "
        Private Sub CleanUp()
            m_LastRequested = String.Empty
            m_NewRequested = String.Empty
        End Sub
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

