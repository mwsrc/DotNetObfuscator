Imports System.Reflection
Imports Helper.AssemblyHelper
Imports Core20Reader

Namespace Engine.Analyze
    Public NotInheritable Class ValidatedFile
        Inherits EventArgs

#Region " Fields "
        Private m_peInfos As IReader
        Private m_isValid As Boolean
        Private m_assembly As Data
#End Region

#Region " Properties "
        Public ReadOnly Property peInfos As IReader
            Get
                Return m_peInfos
            End Get
        End Property

        Public ReadOnly Property isValid As Boolean
            Get
                Return m_isValid
            End Get
        End Property

        Public ReadOnly Property assembly As Data
            Get
                Return m_assembly
            End Get
        End Property
#End Region

#Region " Constructor "
        Public Sub New(isvalid As Boolean, peInfos As Reader, ass As Data)
            m_isValid = isvalid
            m_assembly = ass
            m_peInfos = peInfos
        End Sub
#End Region

    End Class
End Namespace