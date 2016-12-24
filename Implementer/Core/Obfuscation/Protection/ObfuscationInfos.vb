Namespace Core.Obfuscation.Protection
    Public Class ObfuscationInfos
        Implements IDisposable

#Region " Properties "
        Private m_Enabled As Boolean
        Public ReadOnly Property Enabled As Boolean
            Get
                Return m_Enabled
            End Get
        End Property

        Private m_RenameResourcesContent As Boolean
        Public ReadOnly Property RenameResourcesContent As Boolean
            Get
                Return m_RenameResourcesContent
            End Get
        End Property

        Private m_EncryptResources As Boolean
        Public ReadOnly Property EncryptResources As Boolean
            Get
                Return m_EncryptResources
            End Get
        End Property

        Private m_CompressResources As Boolean
        Public ReadOnly Property CompressResources As Boolean
            Get
                Return m_CompressResources
            End Get
        End Property

        Private m_AntiIlDasm As Boolean
        Public ReadOnly Property AntiIlDasm As Boolean
            Get
                Return m_AntiIlDasm
            End Get
        End Property

        Private m_AntiTamper As Boolean
        Public ReadOnly Property AntiTamper As Boolean
            Get
                Return m_AntiTamper
            End Get
        End Property

        Private m_AntiDebug As Boolean
        Public ReadOnly Property AntiDebug As Boolean
            Get
                Return m_AntiDebug
            End Get
        End Property

        Private m_AntiDumper As Boolean
        Public ReadOnly Property AntiDumper As Boolean
            Get
                Return m_AntiDumper
            End Get
        End Property

        'Private m_AntiReflector As Boolean
        'Public ReadOnly Property AntiReflector As Boolean
        '    Get
        '        Return m_AntiReflector
        '    End Get
        'End Property

        Private m_EncryptBoolean As Boolean
        Public ReadOnly Property EncryptBoolean As Boolean
            Get
                Return m_EncryptBoolean
            End Get
        End Property

        Private m_EncryptNumeric As Boolean
        Public ReadOnly Property EncryptNumeric As Boolean
            Get
                Return m_EncryptNumeric
            End Get
        End Property

        Private m_EncryptString As Boolean
        Public ReadOnly Property EncryptString As Boolean
            Get
                Return m_EncryptString
            End Get
        End Property

        Private m_HidePublicCalls As Boolean
        Public ReadOnly Property HidePublicCalls As Boolean
            Get
                Return m_HidePublicCalls
            End Get
        End Property

        Private m_InvalidOpcodes As Boolean
        Public ReadOnly Property InvalidOpcodes As Boolean
            Get
                Return m_InvalidOpcodes
            End Get
        End Property

        Private m_InvalidMetadata As Boolean
        Public ReadOnly Property InvalidMetadata As Boolean
            Get
                Return m_InvalidMetadata
            End Get
        End Property

        Private m_RenameAssembly As Boolean
        Public ReadOnly Property RenameAssembly As Boolean
            Get
                Return m_RenameAssembly
            End Get
        End Property
#End Region

#Region " Constructor "
        Public Sub New(Enable As Boolean, RenameResourcesCont As Boolean, EncryptRes As Boolean, CompressRes As Boolean, EncryptNum As Boolean, EncryptBool As Boolean, EncryptStr As Boolean, _
                       AntiIlD As Boolean, AntiT As Boolean, AntiD As Boolean, AntiDump As Boolean, HideCalls As Boolean, InvalidOp As Boolean, InvalidM As Boolean, RenameAss As Boolean)
            m_Enabled = Enable
            m_RenameResourcesContent = RenameResourcesCont
            m_EncryptResources = EncryptRes
            m_CompressResources = CompressRes
            m_EncryptNumeric = EncryptNum
            m_EncryptBoolean = EncryptBool
            m_EncryptString = EncryptStr
            m_AntiIlDasm = AntiIlD
            m_AntiTamper = AntiT
            m_AntiDebug = AntiD
            m_AntiDumper = AntiDump
            m_HidePublicCalls = HideCalls
            m_InvalidOpcodes = InvalidOp
            m_InvalidMetadata = InvalidM
            m_RenameAssembly = RenameAss
        End Sub
#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                End If
                m_Enabled = False
                m_RenameResourcesContent = False
                m_EncryptResources = False
                m_CompressResources = False
                m_AntiIlDasm = False
                m_AntiTamper = False
                m_AntiDebug = False
                m_AntiDumper = False
                'm_AntiReflector = False
                m_EncryptBoolean = False
                m_EncryptNumeric = False
                m_EncryptString = False
                m_HidePublicCalls = False
                m_InvalidOpcodes = False
                m_InvalidMetadata = False
                m_RenameAssembly = False
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
