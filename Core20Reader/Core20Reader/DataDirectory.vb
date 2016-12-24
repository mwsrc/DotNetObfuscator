Imports Core20Reader.Reader

Friend Class DataDirectory

#Region " PRIVATE MEMBERS "
    Private m_Section As IMAGE_SECTION_HEADER
    Private m_SectionStartOffset As ULong
    Private m_Address As ULong
    Private m_Size As ULong
#End Region

#Region " PROPERTIES "
    Friend Property Address() As ULong
        Get
            Return m_Address
        End Get
        Set(value As ULong)
            m_Address = value
        End Set
    End Property

    Friend Property Size() As ULong
        Get
            Return m_Size
        End Get
        Set(value As ULong)
            m_Size = value
        End Set
    End Property

    Friend Property Section As IMAGE_SECTION_HEADER
        Get
            Return Me.m_Section
        End Get
        Set(value As IMAGE_SECTION_HEADER)
            m_Section = value
        End Set
    End Property

    Friend Property SectionStartOffset As ULong
        Get
            Return Me.m_SectionStartOffset
        End Get
        Set(value As ULong)
            m_SectionStartOffset = value
        End Set
    End Property
#End Region

End Class
