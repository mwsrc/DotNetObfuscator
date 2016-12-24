Imports System.IO
Imports System.ComponentModel
Imports LoginTheme.XertzLoginTheme
Imports Implementer.Core.Obfuscation.Exclusion
Imports Implementer.Core.Dependencing
Imports System.Security.Cryptography
Imports System.Text
Imports Helper.UtilsHelper
Imports Helper.RandomizeHelper

Public Class Frm_Exclusion

#Region " Fields "
    Private m_FilePath$ = String.Empty
    Private m_excludeList As ExcludeList
    Private m_TreeviewHandler As ExclusionTreeview
    Private SettingsButton As Dictionary(Of LogInCheckBox, Boolean)
    Private m_treeNode As TreeNode
    Private md5hash As String
#End Region

#Region " Properties "
    Public Property SettingsState As ExclusionState
    Public Property Dependencies As List(Of String)
    Public Property Title As String
    Public Property ViewAssemblyOnly As Boolean
    Public Property FilePath As String
#End Region

#Region " Events "
    Public Event OnShowingExclusionInfos As ShowingExclusionInfosDelegate
#End Region

#Region " Delegates "
    Public Delegate Sub ShowingExclusionInfosDelegate(e As ExcludeList)
#End Region

#Region " Constructor "
    Sub New()
        InitializeComponent()
    End Sub
#End Region

#Region " Methods "

    Private Sub Frm_Exclusion_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        If ViewAssemblyOnly Then
            FillViewer()
        End If
        If Not TvExclusion.Nodes.Count = 0 Then
            m_treeNode = TvExclusion.Nodes(0)
        End If
        If BgwExclusion.IsBusy = False Then
            BgwExclusion.RunWorkerAsync()
        Else
            BgwExclusion.CancelAsync()
            BgwExclusion.RunWorkerAsync()
        End If
    End Sub

    Public Sub InitializeExcludeList()
        SettingsState = New ExclusionState(True, True, True, True, True, True, True)
        m_excludeList = New ExcludeList
        RaiseEvent OnShowingExclusionInfos(m_excludeList)
    End Sub

    Public Sub FinalizeExcludeList()
        m_excludeList.CleanUp()
        RaiseEvent OnShowingExclusionInfos(m_excludeList)
    End Sub

    Private Sub Frm_Exclusion_FormClosing(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        BgwExclusion.CancelAsync()
        RaiseEvent OnShowingExclusionInfos(m_excludeList)
    End Sub

    Private Sub BgwExclusion_DoWork(sender As Object, e As DoWorkEventArgs) Handles BgwExclusion.DoWork
        BgwExclusion.ReportProgress(101, Nothing)
        Dim r As New Frm_Result("Loading", "Please wait while loading ...", "")
        BgwExclusion.ReportProgress(102, r)
        If Dependencies.Count <> 0 Then
            Dim fPath = Directory.CreateDirectory(String.Format("{0}{1}\", System.IO.Path.GetTempPath, Randomizer.GenerateNewAlphabetic)).FullName & New FileInfo(_FilePath).Name
            File.Copy(_FilePath, fPath, True)
            _FilePath = fPath

            Dim m_Dependencies = New Dependencies(_FilePath, Dependencies)
            Dim dependenciesResult = m_Dependencies.Analyze()

            If dependenciesResult.result = String.Empty Then
                m_Dependencies.Merge(BgwExclusion)
                If Functions.GetMD5HashFromFile(_FilePath) <> md5hash Then
                    m_TreeviewHandler = New ExclusionTreeview(_FilePath)
                    BgwExclusion.ReportProgress(103, r)
                    e.Result = New Object() {"Success", m_TreeviewHandler.LoadTreeNode, Nothing, True}
                    md5hash = Functions.GetMD5HashFromFile(_FilePath)
                Else
                    BgwExclusion.ReportProgress(103, r)
                    e.Result = New Object() {"Already", m_treeNode, Nothing, True}
                End If
            ElseIf dependenciesResult.result.StartsWith("Error") Then
                BgwExclusion.ReportProgress(103, r)
                e.Result = New String() {"Error", dependenciesResult.result, Nothing, True}
                m_Dependencies.CleanUp()
            Else
                m_TreeviewHandler = New ExclusionTreeview(_FilePath)
                BgwExclusion.ReportProgress(103, r)
                e.Result = New Object() {"Warning", dependenciesResult.result, m_TreeviewHandler.LoadTreeNode, True}
            End If
        Else
            If Functions.GetMD5HashFromFile(_FilePath) <> md5hash Then
                m_TreeviewHandler = New ExclusionTreeview(_FilePath)
                BgwExclusion.ReportProgress(103, r)
                e.Result = New Object() {"Success", m_TreeviewHandler.LoadTreeNode, Nothing, False}
                md5hash = Functions.GetMD5HashFromFile(_FilePath)
            Else
                BgwExclusion.ReportProgress(103, r)
                e.Result = New Object() {"Already", m_treeNode, Nothing, False}
            End If
        End If

    End Sub

    Private Sub BgwExclusion_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles BgwExclusion.ProgressChanged
        If e.ProgressPercentage = 101 Then
            TvExclusion.Nodes.Clear()
        ElseIf e.ProgressPercentage = 102 Then
            TryCast(e.UserState, Frm_Result).ShowDialog()
        ElseIf e.ProgressPercentage = 103 Then
            TryCast(e.UserState, Frm_Result).Close()
        End If
    End Sub

    Private Sub BgwExclusion_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BgwExclusion.RunWorkerCompleted
        If Not e.Result Is Nothing Then
            Select Case e.Result(0)
                Case "Error"
                    Dim r As New Frm_Result(e.Result(0), e.Result(1).ToString, "")
                    r.ShowDialog()
                Case "Warning"
                    TvExclusion.Nodes.Add(e.Result(2))
                    FillForm()
                    Dim r As New Frm_Result(e.Result(0), "Following dependencies are missing : " & vbNewLine & e.Result(1).ToString, "")
                    r.ShowDialog()
                Case "Success", "Already"
                    TvExclusion.Nodes.Add(e.Result(1))
                    FillForm()
            End Select
            If CBool(e.Result(3)) Then
                Try
                    Dim fi As New FileInfo(_FilePath)
                    fi.Directory.Delete(True)
                Catch ex As Exception
                End Try
            End If
        End If

    End Sub

    Private Sub FillForm()
        If ViewAssemblyOnly = False Then
            SettingsButton = New Dictionary(Of LogInCheckBox, Boolean)
            With SettingsButton
                .Add(ChbExclusionRenaming, SettingsState.Renaming)
                .Add(ChbExclusionStringsEncrypt, SettingsState.stringEncrypt)
                .Add(ChbExclusionIntegersEncode, SettingsState.integerEncoding)
                .Add(ChbExclusionBooleanEncrypt, SettingsState.booleanEncrypt)
                .Add(ChbExclusionHideCalls, SettingsState.hideCalls)
                .Add(ChbExclusionInvalidOpCodes, SettingsState.InvalidOpcodes)
            End With

            SettingsButton.Keys.ToList.ForEach(Sub(x) x.Visible = False)

            Dim y% = 94
            For Each c In SettingsButton
                If c.Value Then
                    c.Key.Location = New Size(4, y)
                    y += 28
                    c.Key.Visible = True
                End If
            Next
        Else
            FillViewer()
            RemoveHandler TvExclusion.AfterSelect, AddressOf TvExclusion_AfterSelect
        End If

        TvExclusion.Nodes(0).Expand()
        TvExclusion.Nodes(0).FirstNode.Expand()
    End Sub

    Private Sub FillViewer()
        Frm_ExclusionThemeContainer.Text = Title
        GbxExclusionRule.Visible = False
        GbxExclusionDetails.Visible = False
        GbxExclusionViewer.Text = "                                                                             Assembly viewer"
        GbxExclusionViewer.Size = New Size(680, 640)
        TvExclusion.Size = New Size(680, 605)
    End Sub

    Private Sub TvExclusion_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TvExclusion.AfterSelect
        If Not m_TreeviewHandler.isRenamable(e.Node.Tag) Then
            ChbExclusion.Checked = False
            ChbAllEntities.Checked = False
            GbxExclusionRule.Enabled = False

            SettingsButton.Keys.ToList.ForEach(Sub(x) x.Checked = False)
            SettingsButton.Keys.ToList.ForEach(Sub(x) x.Enabled = False)

            ChbExclusionCheckAll.Enabled = False
        ElseIf (Not e.Node.Tag Is Nothing) Then
            ChbExclusion.Checked = m_TreeviewHandler.isExclude(e.Node.Tag)
            ChbAllEntities.Checked = m_TreeviewHandler.getEntitiesVal(e.Node.Tag)
            GbxExclusionRule.Enabled = m_TreeviewHandler.isTypedef(e.Node.Tag)
            ChbAllEntities.Enabled = ChbExclusion.Checked

            ChbExclusionStringsEncrypt.Checked = m_TreeviewHandler.isStringsEncryptExclude(e.Node.Tag)
            ChbExclusionIntegersEncode.Checked = m_TreeviewHandler.isIntegersEncodingExclude(e.Node.Tag)
            ChbExclusionBooleanEncrypt.Checked = m_TreeviewHandler.isBooleansEncryptExclude(e.Node.Tag)
            ChbExclusionRenaming.Checked = m_TreeviewHandler.isRenamingExclude(e.Node.Tag)
            ChbExclusionInvalidOpCodes.Checked = m_TreeviewHandler.isInvalidOpcodesExclude(e.Node.Tag)
            ChbExclusionHideCalls.Checked = m_TreeviewHandler.isHideCallsExclude(e.Node.Tag)

            SettingsButton.Keys.ToList.ForEach(Sub(x) x.Enabled = ChbExclusion.Checked)

            ChbExclusionCheckAll.Enabled = ChbExclusion.Checked
        Else
            GbxExclusionRule.Enabled = False
            ChbExclusion.Checked = False

            SettingsButton.Keys.ToList.ForEach(Sub(x) x.Checked = False)
            SettingsButton.Keys.ToList.ForEach(Sub(x) x.Enabled = False)

            ChbExclusionCheckAll.Enabled = False
        End If

        ChbExclusionCheckAll.Text = "Check all"
        ChbExclusionCheckAll.Checked = False
    End Sub

    Private Sub IncludeAllChildNodes(treeNode As TreeNode, Optional ByVal nodeChecked As Boolean = False, Optional ByVal stringEncr As Boolean = False, _
                                                           Optional ByVal integerEncod As Boolean = False, Optional ByVal booleanEncr As Boolean = False, _
                                                           Optional ByVal Renamin As Boolean = False, Optional ByVal invalidOp As Boolean = False, _
                                                           Optional ByVal hideCall As Boolean = False)
        For Each node As TreeNode In treeNode.Nodes
            With node
                If m_TreeviewHandler.isRenamable(.Tag) Then
                    .Tag.Exclude = nodeChecked
                    .Tag.stringEncrypt = stringEncr
                    .Tag.integerEncoding = integerEncod
                    .Tag.booleanEncrypt = booleanEncr
                    .Tag.Renaming = Renamin
                    .Tag.InvalidOpcodes = invalidOp
                    .Tag.HideCalls = hideCall
                    ColorNode(node, nodeChecked)
                    If .Nodes.Count > 0 Then
                        IncludeAllChildNodes(node, nodeChecked, stringEncr, integerEncod, booleanEncr, Renamin, invalidOp, hideCall)
                    End If
                End If
            End With
        Next
    End Sub

    Private Sub IncludeEntitiesChildNodes(treeNode As TreeNode, Optional ByVal nodeChecked As Boolean = False, Optional ByVal stringEncr As Boolean = False, _
                                                                Optional ByVal integerEncod As Boolean = False, Optional ByVal booleanEncr As Boolean = False, _
                                                                Optional ByVal Renamin As Boolean = False, Optional ByVal invalidOp As Boolean = False, _
                                                                Optional ByVal hideCall As Boolean = False)
        For Each node As TreeNode In treeNode.Nodes
            With node
                If m_TreeviewHandler.isRenamable(.Tag) Then
                    .Tag.Exclude = nodeChecked
                    .Tag.AllEntities = nodeChecked
                    .Tag.stringEncrypt = stringEncr
                    .Tag.integerEncoding = integerEncod
                    .Tag.booleanEncrypt = booleanEncr
                    .Tag.Renaming = Renamin
                    .Tag.InvalidOpcodes = invalidOp
                    .Tag.HideCalls = hideCall
                    ColorNode(node, nodeChecked)
                    If .Nodes.Count > 0 Then
                        IncludeEntitiesChildNodes(node, nodeChecked, stringEncr, integerEncod, booleanEncr, Renamin, invalidOp, hideCall)
                    End If
                End If
            End With
        Next
    End Sub

    Private Sub ChbAllEntities_Click(sender As Object, e As EventArgs) Handles ChbAllEntities.Click
        If (Not Me.TvExclusion.SelectedNode.Tag Is Nothing) Then
            With TvExclusion.SelectedNode
                .Tag.AllEntities = ChbAllEntities.Checked
                .Tag.stringEncrypt = ChbExclusionStringsEncrypt.Checked
                .Tag.integerEncoding = ChbExclusionIntegersEncode.Checked
                .Tag.booleanEncrypt = ChbExclusionBooleanEncrypt.Checked
                .Tag.Renaming = ChbExclusionRenaming.Checked
                .Tag.InvalidOpcodes = ChbExclusionInvalidOpCodes.Checked
                .Tag.HideCalls = ChbExclusionInvalidOpCodes.Checked
            End With
        End If
        If ChbAllEntities.Checked = True Then
            IncludeEntitiesChildNodes(TvExclusion.SelectedNode, ChbAllEntities.Checked, ChbExclusionStringsEncrypt.Checked, ChbExclusionIntegersEncode.Checked, _
                                      ChbExclusionBooleanEncrypt.Checked, ChbExclusionRenaming.Checked, ChbExclusionInvalidOpCodes.Checked, ChbExclusionHideCalls.Checked)
        Else
            ColorNode(TvExclusion.SelectedNode, False)
            IncludeAllChildNodes(TvExclusion.SelectedNode)
            IncludeEntitiesChildNodes(TvExclusion.SelectedNode)
        End If
    End Sub

    Private Sub ChbExclusion_Click(sender As Object, e As EventArgs) Handles ChbExclusion.Click

        If ChbExclusion.Checked = True Then
            ChbAllEntities.Enabled = True

            SettingsButton.Keys.ToList.ForEach(Sub(x) x.Enabled = True)

            ChbExclusionCheckAll.Enabled = True

            IncludeAllChildNodes(TvExclusion.SelectedNode, ChbAllEntities.Checked, ChbExclusionStringsEncrypt.Checked, ChbExclusionIntegersEncode.Checked, _
                                 ChbExclusionBooleanEncrypt.Checked, ChbExclusionRenaming.Checked, ChbExclusionInvalidOpCodes.Checked, ChbExclusionHideCalls.Checked)
        Else
            ChbAllEntities.Enabled = False

            SettingsButton.Keys.ToList.ForEach(Sub(x) x.Enabled = False)

            ChbExclusionCheckAll.Enabled = False

            ColorNode(TvExclusion.SelectedNode, False)
            IncludeAllChildNodes(TvExclusion.SelectedNode)
            IncludeEntitiesChildNodes(TvExclusion.SelectedNode)
            ChbAllEntities.Checked = False

            SettingsButton.Keys.ToList.ForEach(Sub(x) x.Checked = False)

            ChbExclusionCheckAll.Checked = False
            ChbExclusionCheckAll.Text = "Check all"
        End If

        If (Not Me.TvExclusion.SelectedNode.Tag Is Nothing) Then
            With TvExclusion.SelectedNode
                .Tag.exclude = ChbExclusion.Checked
                .Tag.AllEntities = ChbAllEntities.Checked
                .Tag.stringEncrypt = ChbExclusionStringsEncrypt.Checked
                .Tag.integerEncoding = ChbExclusionIntegersEncode.Checked
                .Tag.booleanEncrypt = ChbExclusionBooleanEncrypt.Checked
                .Tag.Renaming = ChbExclusionRenaming.Checked
                .Tag.InvalidOpcodes = ChbExclusionInvalidOpCodes.Checked
                .Tag.HideCalls = ChbExclusionHideCalls.Checked
            End With
            ColorNode(TvExclusion.SelectedNode, ChbExclusion.Checked)
        End If
    End Sub

    Private Sub CheckSettings(sender As Object, e As EventArgs) Handles ChbExclusionStringsEncrypt.Click, ChbExclusionBooleanEncrypt.Click, ChbExclusionIntegersEncode.Click, _
                                                            ChbExclusion.Click, ChbExclusionInvalidOpCodes.Click, ChbExclusionRenaming.Click, ChbExclusionHideCalls.Click

        If (Not Me.TvExclusion.SelectedNode.Tag Is Nothing) Then
            With TvExclusion.SelectedNode
                .Tag.AllEntities = ChbAllEntities.Checked
                .Tag.stringEncrypt = ChbExclusionStringsEncrypt.Checked
                .Tag.integerEncoding = ChbExclusionIntegersEncode.Checked
                .Tag.booleanEncrypt = ChbExclusionBooleanEncrypt.Checked
                .Tag.Renaming = ChbExclusionRenaming.Checked
                .Tag.InvalidOpcodes = ChbExclusionInvalidOpCodes.Checked
                .Tag.HideCalls = ChbExclusionHideCalls.Checked
            End With

        End If
    End Sub

    Private Sub ChbExclusionCheckAll_Click(sender As Object, e As EventArgs) Handles ChbExclusionCheckAll.Click
        If ChbExclusionCheckAll.Text = "Check all" Then
            If (Not Me.TvExclusion.SelectedNode.Tag Is Nothing) Then
                With TvExclusion.SelectedNode
                    .Tag.stringEncrypt = True
                    .Tag.integerEncoding = True
                    .Tag.booleanEncrypt = True
                    .Tag.Renaming = True
                    .Tag.InvalidOpcodes = True
                    .Tag.HideCalls = True

                    SettingsButton.Keys.ToList.ForEach(Sub(x) x.Checked = True)

                    .Tag.AllEntities = ChbAllEntities.Checked
                End With
                ChbExclusionCheckAll.Text = "Uncheck all"
            End If
        Else
            If (Not Me.TvExclusion.SelectedNode.Tag Is Nothing) Then
                With TvExclusion.SelectedNode
                    .Tag.stringEncrypt = False
                    .Tag.integerEncoding = False
                    .Tag.booleanEncrypt = False
                    .Tag.Renaming = False
                    .Tag.InvalidOpcodes = False
                    .Tag.HideCalls = False

                    SettingsButton.Keys.ToList.ForEach(Sub(x) x.Checked = False)

                    .Tag.AllEntities = ChbAllEntities.Checked
                End With
                ChbExclusionCheckAll.Text = "Check all"
            End If
        End If

    End Sub

    Private Sub ColorNode(Node As TreeNode, Checked As Boolean)
        If Checked = True Then
            Node.ForeColor = Color.Red
            m_excludeList.AddTo(Node.Tag)
        Else
            Node.ForeColor = Color.Black
            m_excludeList.RemoveFrom(Node.Tag)
        End If
        LblExclusionTotal.Text = "Total : " & m_excludeList.itemsCount.ToString
        LblExclusionDetailsTypes.Text = "Types : " & m_excludeList.TypesCount
        LblExclusionDetailsMethods.Text = "Methods : " & m_excludeList.MethodsCount
        LblExclusionDetailsProperties.Text = "Properties : " & m_excludeList.PropertiesCount
        LblExclusionDetailsFields.Text = "Fields : " & m_excludeList.FieldsCount
        LblExclusionDetailsEvents.Text = "Events : " & m_excludeList.EventsCount
    End Sub
#End Region

End Class