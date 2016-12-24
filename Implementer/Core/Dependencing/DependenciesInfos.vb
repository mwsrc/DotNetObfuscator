Namespace Core.Dependencing
    Public Class DependenciesInfos
        Implements IDisposable

#Region " Fields "
        Private m_Enabled As Boolean
        Private m_Dependencies As IEnumerable(Of String)
        Private m_DependenciesMode As DependenciesAddMode
        Private m_DependenciesCompressEncryptMode As CompressEncryptMode
#End Region

#Region " Enumerations "
        Enum DependenciesAddMode
            Merged = 0
            Embedded = 1
        End Enum

        Enum CompressEncryptMode
            None = 0
            Encrypt = 1
            Compress = 2
            Both = 3
        End Enum
#End Region

#Region " Properties "
        Public ReadOnly Property Enabled As Boolean
            Get
                Return m_Enabled
            End Get
        End Property

        Public ReadOnly Property Dependencies As IEnumerable(Of String)
            Get
                Return m_Dependencies
            End Get
        End Property

        Public ReadOnly Property DependenciesMode As DependenciesAddMode
            Get
                Return m_DependenciesMode
            End Get
        End Property

        Public ReadOnly Property DependenciesCompressEncryptMode As CompressEncryptMode
            Get
                Return m_DependenciesCompressEncryptMode
            End Get
        End Property
#End Region

#Region " Constructor "
        Public Sub New(Enable As Boolean, Dependenc As IEnumerable(Of String), DependenciesMod As Boolean, DependenciesCompressEncryptMod%)
            m_Enabled = Enable
            m_Dependencies = Dependenc
            m_DependenciesMode = DependenciesModeValue(DependenciesMod)
            m_DependenciesCompressEncryptMode = DependenciesCompressEncryptModeValue(DependenciesCompressEncryptMod)
        End Sub
#End Region

#Region " Methods "
        Private Function DependenciesModeValue(boolValue As Boolean) As DependenciesAddMode
            Return If(boolValue, DependenciesInfos.DependenciesAddMode.Embedded, DependenciesInfos.DependenciesAddMode.Merged)
        End Function

        Private Function DependenciesCompressEncryptModeValue(intValue%) As CompressEncryptMode
            Select Case intValue
                Case 0
                    Return CompressEncryptMode.None
                Case 1
                    Return CompressEncryptMode.Encrypt
                Case 2
                    Return CompressEncryptMode.Compress
                Case 3
                    Return CompressEncryptMode.Both
            End Select
            Return CompressEncryptMode.None
        End Function
#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                End If
                m_Enabled = False
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
