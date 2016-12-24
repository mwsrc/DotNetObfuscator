Namespace Core.Dependencing

    Public NotInheritable Class AnalysisResult

#Region " Fields "
        Private m_result As String
#End Region

#Region " Properties "
        Public ReadOnly Property result As String
            Get
                Return m_result
            End Get
        End Property
#End Region

#Region " Constructor "
        Friend Sub New(result As String)
            m_result = result
        End Sub
#End Region

    End Class

End Namespace
