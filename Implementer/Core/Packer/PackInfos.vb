Imports Implementer.Core.IconChanger
Imports System.IO
Imports System.Drawing

Namespace Core.Packer
    Public Class PackInfos
        Implements IDisposable

#Region " Fields "
        Private m_Enabled As Boolean
        Private m_NewIcon As Icon
        Private m_RequestedLevel As String
#End Region

#Region " Properties "
        Public ReadOnly Property Enabled As Boolean
            Get
                Return m_Enabled
            End Get
        End Property

        Public ReadOnly Property NewIcon As Icon
            Get
                Return m_NewIcon
            End Get
        End Property

        Public ReadOnly Property RequestedLevel As String
            Get
                Return m_RequestedLevel
            End Get
        End Property
#End Region

#Region " Constructor "
        Public Sub New(Enable As Boolean, NewIconP$, RequestedL$)
            m_Enabled = Enable
            m_NewIcon = NewIconValue(NewIconP)
            m_RequestedLevel = RequestedL
        End Sub
#End Region

#Region " Methods "
        Private Function NewIconValue(fPath$) As Icon
            If fPath.ToLower.EndsWith(".ico") Then Return New Icon(fPath)
            Return New IconInfos(fPath).NewIcon
        End Function
#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean ' Pour détecter les appels redondants

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: supprimez l'état managé (objets managés).
                End If

                ' TODO: libérez les ressources non managées (objets non managés) et substituez la méthode Finalize() ci-dessous.
                ' TODO: définissez les champs volumineux à null.
            End If
            Me.disposedValue = True
        End Sub

        ' Ce code a été ajouté par Visual Basic pour permettre l'implémentation correcte du modèle pouvant être supprimé.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ne modifiez pas ce code. Ajoutez du code de nettoyage dans Dispose(disposing As Boolean) ci-dessus.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
