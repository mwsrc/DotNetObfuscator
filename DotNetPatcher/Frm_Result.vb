Imports System.IO
Imports Implementer.Core.Obfuscation.Exclusion

Public Class Frm_Result

#Region " Fields "
    Private m_FilePath$
    Private m_hideAssemblyViewer As Boolean
#End Region

#Region " Constructor "
    Sub New(Title$, Message$, FilePath$, Optional ByVal hideAssemblyViewer As Boolean = False)
        InitializeComponent()
        Frm_ResultThemeContainer.Text = Title
        LblResultMessage.Text = Message
        m_FilePath = FilePath
        m_hideAssemblyViewer = hideAssemblyViewer
    End Sub
#End Region

#Region " Methods "

    Private Sub Frm_Result_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        If Frm_ResultThemeContainer.Text.Contains("Error") Then
            PcbResultIcon.Image = My.Resources._error
            BtnResultOpenFileDir.Visible = False
            BtnResultOpenAssemblyViewer.Visible = False
        ElseIf Frm_ResultThemeContainer.Text.Contains("Success") Then
            PcbResultIcon.Image = My.Resources.Valid
            BtnResultOpenFileDir.Visible = True
            BtnResultOpenAssemblyViewer.Visible = True
            BtnResultOpenAssemblyViewer.Enabled = If(m_hideAssemblyViewer, False, True)
        ElseIf Frm_ResultThemeContainer.Text.Contains("Warning") Then
            PcbResultIcon.Image = My.Resources.Warning
            BtnResultOpenFileDir.Visible = False
            BtnResultOpenAssemblyViewer.Visible = False
        ElseIf Frm_ResultThemeContainer.Text.Contains("Loading") Then
            PcbResultIcon.Image = My.Resources.Loading
            BtnResultOpenFileDir.Visible = False
            BtnResultOpenAssemblyViewer.Visible = False      
        End If
        PcbResultIcon.Visible = True
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As EventArgs) Handles BtnResultClose.Click
        Me.Close()
    End Sub

    Private Sub BtnResultOpenFileDir_Click(sender As Object, e As EventArgs) Handles BtnResultOpenFileDir.Click
        Dim fi As New FileInfo(m_FilePath)
        Process.Start(fi.DirectoryName)
        Me.Close()
    End Sub

    Private Sub BtnResultOpenAssemblyViewer_Click(sender As Object, e As EventArgs) Handles BtnResultOpenAssemblyViewer.Click
        Dim m_exclude = New Frm_Exclusion
        m_exclude.InitializeExcludeList()
        m_exclude.SettingsState = New ExclusionState(False, False, False, False, False, False)
        With m_exclude
            .Dependencies = New List(Of String)
            .Title = "Protected file"
            .ViewAssemblyOnly = True
            .FilePath = m_FilePath
            .ShowDialog()
        End With

        'Dim frm As New Frm_Exclusion(New ExclusionTreeview(_FilePath), "Protected file", True)
        'frm.ShowDialog()
    End Sub
#End Region

End Class