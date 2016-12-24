Imports Helper.RandomizeHelper
Imports Helper.UtilsHelper
Imports System.IO
Imports Core20Reader
Imports Helper.CecilHelper
Imports System.Drawing

Namespace Core.IconChanger
    Public Class IconInfos
        Implements IDisposable

#Region " Fields "
        Private m_tmpExePath As String
#End Region

#Region " Properties "
        Private m_Enabled As Boolean
        Public ReadOnly Property Enabled As Boolean
            Get
                Return m_Enabled
            End Get
        End Property

        Private m_NewIcon As Icon
        Public ReadOnly Property NewIcon As Icon
            Get
                Return m_NewIcon
            End Get
        End Property
#End Region

#Region " Constructor "
        Public Sub New(Enable As Boolean, NewIconPath$)
            m_Enabled = Enable
            m_NewIcon = If(File.Exists(NewIconPath), New Icon(NewIconPath), Nothing)
            m_Enabled = checkNewIconExists()
        End Sub

        Public Sub New(FilePath$)
            m_NewIcon = FromExeFile(FilePath)
        End Sub
#End Region

#Region " Methods "
        Private Function FromExeFile(FilePath$) As Icon
            If File.Exists(FilePath) Then
                m_tmpExePath = Functions.GetTempFolder & "\" & Randomizer.GenerateNewAlphabetic & ".exe"
                File.Copy(FilePath, m_tmpExePath, True)
                Dim ic As New Reader()
                With ic
                    .ReadFile(m_tmpExePath)
                    m_Enabled = True
                    Return .GetMainIcon
                End With
            End If
            m_Enabled = False
            Return Nothing
        End Function

        Private Function checkNewIconExists() As Boolean
            If m_Enabled Then
                If m_NewIcon IsNot Nothing Then
                    Return True
                End If
            End If
            Return False
        End Function
#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                End If
                m_Enabled = False
                m_NewIcon = Nothing
                Try
                    File.Delete(m_tmpExePath)
                Catch ex As Exception
                End Try
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
