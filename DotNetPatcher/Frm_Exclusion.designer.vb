
Imports LoginTheme
Imports LoginTheme.XertzLoginTheme

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Frm_Exclusion
    Inherits System.Windows.Forms.Form

    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requise par le Concepteur Windows Form
    Private components As System.ComponentModel.IContainer

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Frm_Exclusion))
        Me.BgwRenameTask = New System.ComponentModel.BackgroundWorker()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.BgwExclusion = New System.ComponentModel.BackgroundWorker()
        Me.Frm_ExclusionThemeContainer = New LoginTheme.XertzLoginTheme.LogInThemeContainer()
        Me.GbxExclusionDetails = New LoginTheme.XertzLoginTheme.LogInGroupBox()
        Me.LblExclusionDetailsEvents = New LoginTheme.XertzLoginTheme.LogInLabel()
        Me.LblExclusionTotal = New LoginTheme.XertzLoginTheme.LogInLabel()
        Me.LblExclusionDetailsProperties = New LoginTheme.XertzLoginTheme.LogInLabel()
        Me.LblExclusionDetailsFields = New LoginTheme.XertzLoginTheme.LogInLabel()
        Me.LblExclusionDetailsMethods = New LoginTheme.XertzLoginTheme.LogInLabel()
        Me.LblExclusionDetailsTypes = New LoginTheme.XertzLoginTheme.LogInLabel()
        Me.GbxExclusionViewer = New LoginTheme.XertzLoginTheme.LogInGroupBox()
        Me.TvExclusion = New LoginTheme.XertzLoginTheme.TreeViewEx()
        Me.GbxExclusionRule = New LoginTheme.XertzLoginTheme.LogInGroupBox()
        Me.ChbExclusionCheckAll = New LoginTheme.XertzLoginTheme.LogInCheckBox()
        Me.ChbExclusionHideCalls = New LoginTheme.XertzLoginTheme.LogInCheckBox()
        Me.ChbAllEntities = New LoginTheme.XertzLoginTheme.LogInCheckBox()
        Me.ChbExclusionInvalidOpCodes = New LoginTheme.XertzLoginTheme.LogInCheckBox()
        Me.ChbExclusionBooleanEncrypt = New LoginTheme.XertzLoginTheme.LogInCheckBox()
        Me.ChbExclusionRenaming = New LoginTheme.XertzLoginTheme.LogInCheckBox()
        Me.ChbExclusionIntegersEncode = New LoginTheme.XertzLoginTheme.LogInCheckBox()
        Me.ChbExclusionStringsEncrypt = New LoginTheme.XertzLoginTheme.LogInCheckBox()
        Me.ChbExclusion = New LoginTheme.XertzLoginTheme.LogInCheckBox()
        Me.Frm_ExclusionThemeContainer.SuspendLayout
        Me.GbxExclusionDetails.SuspendLayout
        Me.GbxExclusionViewer.SuspendLayout
        Me.GbxExclusionRule.SuspendLayout
        Me.SuspendLayout
        '
        'BgwRenameTask
        '
        Me.BgwRenameTask.WorkerReportsProgress = true
        Me.BgwRenameTask.WorkerSupportsCancellation = true
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"),System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "Assembly.png")
        Me.ImageList1.Images.SetKeyName(1, "Library.png")
        Me.ImageList1.Images.SetKeyName(2, "NameSpace.png")
        Me.ImageList1.Images.SetKeyName(3, "Class.png")
        Me.ImageList1.Images.SetKeyName(4, "Constructor.png")
        Me.ImageList1.Images.SetKeyName(5, "Delegate.png")
        Me.ImageList1.Images.SetKeyName(6, "Enum.png")
        Me.ImageList1.Images.SetKeyName(7, "EnumValue.png")
        Me.ImageList1.Images.SetKeyName(8, "Event.png")
        Me.ImageList1.Images.SetKeyName(9, "Field.png")
        Me.ImageList1.Images.SetKeyName(10, "Interface.png")
        Me.ImageList1.Images.SetKeyName(11, "Method.png")
        Me.ImageList1.Images.SetKeyName(12, "PInvokeMethod.png")
        Me.ImageList1.Images.SetKeyName(13, "Property.png")
        Me.ImageList1.Images.SetKeyName(14, "StaticClass.png")
        '
        'BgwExclusion
        '
        Me.BgwExclusion.WorkerReportsProgress = true
        Me.BgwExclusion.WorkerSupportsCancellation = True
        '
        'Frm_ExclusionThemeContainer
        '
        Me.Frm_ExclusionThemeContainer.AllowClose = True
        Me.Frm_ExclusionThemeContainer.AllowMaximize = False
        Me.Frm_ExclusionThemeContainer.AllowMinimize = False
        Me.Frm_ExclusionThemeContainer.BackColor = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer))
        Me.Frm_ExclusionThemeContainer.BaseColour = System.Drawing.Color.FromArgb(CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(35, Byte), Integer))
        Me.Frm_ExclusionThemeContainer.BorderColour = System.Drawing.Color.DimGray
        Me.Frm_ExclusionThemeContainer.ContainerColour = System.Drawing.Color.FromArgb(CType(CType(54, Byte), Integer), CType(CType(54, Byte), Integer), CType(CType(54, Byte), Integer))
        Me.Frm_ExclusionThemeContainer.Controls.Add(Me.GbxExclusionDetails)
        Me.Frm_ExclusionThemeContainer.Controls.Add(Me.GbxExclusionViewer)
        Me.Frm_ExclusionThemeContainer.Controls.Add(Me.GbxExclusionRule)
        Me.Frm_ExclusionThemeContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Frm_ExclusionThemeContainer.FontColour = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.Frm_ExclusionThemeContainer.FontSize = 12
        Me.Frm_ExclusionThemeContainer.HoverColour = System.Drawing.Color.FromArgb(CType(CType(42, Byte), Integer), CType(CType(42, Byte), Integer), CType(CType(42, Byte), Integer))
        Me.Frm_ExclusionThemeContainer.Location = New System.Drawing.Point(0, 0)
        Me.Frm_ExclusionThemeContainer.MouseOverColour = System.Drawing.Color.BlueViolet
        Me.Frm_ExclusionThemeContainer.Name = "Frm_ExclusionThemeContainer"
        Me.Frm_ExclusionThemeContainer.ShowControlBox = True
        Me.Frm_ExclusionThemeContainer.ShowIcon = True
        Me.Frm_ExclusionThemeContainer.ShowMaximizeButton = False
        Me.Frm_ExclusionThemeContainer.ShowMinimizeButton = False
        Me.Frm_ExclusionThemeContainer.Size = New System.Drawing.Size(704, 697)
        Me.Frm_ExclusionThemeContainer.TabIndex = 0
        Me.Frm_ExclusionThemeContainer.Text = "Exclusion rules"
        '
        'GbxExclusionDetails
        '
        Me.GbxExclusionDetails.BorderColour = System.Drawing.SystemColors.ButtonShadow
        Me.GbxExclusionDetails.Controls.Add(Me.LblExclusionDetailsEvents)
        Me.GbxExclusionDetails.Controls.Add(Me.LblExclusionTotal)
        Me.GbxExclusionDetails.Controls.Add(Me.LblExclusionDetailsProperties)
        Me.GbxExclusionDetails.Controls.Add(Me.LblExclusionDetailsFields)
        Me.GbxExclusionDetails.Controls.Add(Me.LblExclusionDetailsMethods)
        Me.GbxExclusionDetails.Controls.Add(Me.LblExclusionDetailsTypes)
        Me.GbxExclusionDetails.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.GbxExclusionDetails.HeaderColour = System.Drawing.Color.FromArgb(CType(CType(42, Byte), Integer), CType(CType(42, Byte), Integer), CType(CType(42, Byte), Integer))
        Me.GbxExclusionDetails.Location = New System.Drawing.Point(549, 408)
        Me.GbxExclusionDetails.MainColour = System.Drawing.Color.FromArgb(CType(CType(54, Byte), Integer), CType(CType(54, Byte), Integer), CType(CType(54, Byte), Integer))
        Me.GbxExclusionDetails.Name = "GbxExclusionDetails"
        Me.GbxExclusionDetails.Size = New System.Drawing.Size(143, 277)
        Me.GbxExclusionDetails.TabIndex = 65
        Me.GbxExclusionDetails.Text = "      Excluded items"
        Me.GbxExclusionDetails.TextColour = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'LblExclusionDetailsEvents
        '
        Me.LblExclusionDetailsEvents.AutoSize = True
        Me.LblExclusionDetailsEvents.BackColor = System.Drawing.Color.Transparent
        Me.LblExclusionDetailsEvents.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.LblExclusionDetailsEvents.FontColour = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.LblExclusionDetailsEvents.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.LblExclusionDetailsEvents.Location = New System.Drawing.Point(25, 159)
        Me.LblExclusionDetailsEvents.Name = "LblExclusionDetailsEvents"
        Me.LblExclusionDetailsEvents.Size = New System.Drawing.Size(47, 15)
        Me.LblExclusionDetailsEvents.TabIndex = 4
        Me.LblExclusionDetailsEvents.Text = "Events :"
        '
        'LblExclusionTotal
        '
        Me.LblExclusionTotal.AutoSize = True
        Me.LblExclusionTotal.BackColor = System.Drawing.Color.Transparent
        Me.LblExclusionTotal.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.LblExclusionTotal.FontColour = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.LblExclusionTotal.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.LblExclusionTotal.Location = New System.Drawing.Point(25, 214)
        Me.LblExclusionTotal.Name = "LblExclusionTotal"
        Me.LblExclusionTotal.Size = New System.Drawing.Size(43, 15)
        Me.LblExclusionTotal.TabIndex = 21
        Me.LblExclusionTotal.Text = "Total : "
        '
        'LblExclusionDetailsProperties
        '
        Me.LblExclusionDetailsProperties.AutoSize = True
        Me.LblExclusionDetailsProperties.BackColor = System.Drawing.Color.Transparent
        Me.LblExclusionDetailsProperties.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.LblExclusionDetailsProperties.FontColour = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.LblExclusionDetailsProperties.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.LblExclusionDetailsProperties.Location = New System.Drawing.Point(25, 110)
        Me.LblExclusionDetailsProperties.Name = "LblExclusionDetailsProperties"
        Me.LblExclusionDetailsProperties.Size = New System.Drawing.Size(69, 15)
        Me.LblExclusionDetailsProperties.TabIndex = 3
        Me.LblExclusionDetailsProperties.Text = "Properties : "
        '
        'LblExclusionDetailsFields
        '
        Me.LblExclusionDetailsFields.AutoSize = True
        Me.LblExclusionDetailsFields.BackColor = System.Drawing.Color.Transparent
        Me.LblExclusionDetailsFields.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.LblExclusionDetailsFields.FontColour = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.LblExclusionDetailsFields.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.LblExclusionDetailsFields.Location = New System.Drawing.Point(25, 135)
        Me.LblExclusionDetailsFields.Name = "LblExclusionDetailsFields"
        Me.LblExclusionDetailsFields.Size = New System.Drawing.Size(46, 15)
        Me.LblExclusionDetailsFields.TabIndex = 2
        Me.LblExclusionDetailsFields.Text = "Fields : "
        '
        'LblExclusionDetailsMethods
        '
        Me.LblExclusionDetailsMethods.AutoSize = True
        Me.LblExclusionDetailsMethods.BackColor = System.Drawing.Color.Transparent
        Me.LblExclusionDetailsMethods.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.LblExclusionDetailsMethods.FontColour = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.LblExclusionDetailsMethods.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.LblExclusionDetailsMethods.Location = New System.Drawing.Point(25, 86)
        Me.LblExclusionDetailsMethods.Name = "LblExclusionDetailsMethods"
        Me.LblExclusionDetailsMethods.Size = New System.Drawing.Size(63, 15)
        Me.LblExclusionDetailsMethods.TabIndex = 1
        Me.LblExclusionDetailsMethods.Text = "Methods : "
        '
        'LblExclusionDetailsTypes
        '
        Me.LblExclusionDetailsTypes.AutoSize = True
        Me.LblExclusionDetailsTypes.BackColor = System.Drawing.Color.Transparent
        Me.LblExclusionDetailsTypes.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.LblExclusionDetailsTypes.FontColour = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.LblExclusionDetailsTypes.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.LblExclusionDetailsTypes.Location = New System.Drawing.Point(25, 61)
        Me.LblExclusionDetailsTypes.Name = "LblExclusionDetailsTypes"
        Me.LblExclusionDetailsTypes.Size = New System.Drawing.Size(47, 15)
        Me.LblExclusionDetailsTypes.TabIndex = 0
        Me.LblExclusionDetailsTypes.Text = "Types : "
        '
        'GbxExclusionViewer
        '
        Me.GbxExclusionViewer.BorderColour = System.Drawing.SystemColors.ButtonShadow
        Me.GbxExclusionViewer.Controls.Add(Me.TvExclusion)
        Me.GbxExclusionViewer.Font = New System.Drawing.Font("Segoe UI", 10!)
        Me.GbxExclusionViewer.HeaderColour = System.Drawing.Color.FromArgb(CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer))
        Me.GbxExclusionViewer.Location = New System.Drawing.Point(12, 45)
        Me.GbxExclusionViewer.MainColour = System.Drawing.Color.FromArgb(CType(CType(54,Byte),Integer), CType(CType(54,Byte),Integer), CType(CType(54,Byte),Integer))
        Me.GbxExclusionViewer.Name = "GbxExclusionViewer"
        Me.GbxExclusionViewer.Size = New System.Drawing.Size(531, 640)
        Me.GbxExclusionViewer.TabIndex = 64
        Me.GbxExclusionViewer.Text = "                                                       Assembly viewer"
        Me.GbxExclusionViewer.TextColour = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer))
        '
        'TvExclusion
        '
        Me.TvExclusion.Font = New System.Drawing.Font("Segoe UI", 9!)
        Me.TvExclusion.ImageIndex = 0
        Me.TvExclusion.ImageList = Me.ImageList1
        Me.TvExclusion.Location = New System.Drawing.Point(3, 33)
        Me.TvExclusion.Name = "TvExclusion"
        Me.TvExclusion.SelectedImageIndex = 0
        Me.TvExclusion.Size = New System.Drawing.Size(525, 605)
        Me.TvExclusion.TabIndex = 3
        '
        'GbxExclusionRule
        '
        Me.GbxExclusionRule.BorderColour = System.Drawing.SystemColors.ButtonShadow
        Me.GbxExclusionRule.Controls.Add(Me.ChbExclusionCheckAll)
        Me.GbxExclusionRule.Controls.Add(Me.ChbExclusionHideCalls)
        Me.GbxExclusionRule.Controls.Add(Me.ChbAllEntities)
        Me.GbxExclusionRule.Controls.Add(Me.ChbExclusionInvalidOpCodes)
        Me.GbxExclusionRule.Controls.Add(Me.ChbExclusionBooleanEncrypt)
        Me.GbxExclusionRule.Controls.Add(Me.ChbExclusionRenaming)
        Me.GbxExclusionRule.Controls.Add(Me.ChbExclusionIntegersEncode)
        Me.GbxExclusionRule.Controls.Add(Me.ChbExclusionStringsEncrypt)
        Me.GbxExclusionRule.Controls.Add(Me.ChbExclusion)
        Me.GbxExclusionRule.Enabled = false
        Me.GbxExclusionRule.Font = New System.Drawing.Font("Segoe UI", 10!)
        Me.GbxExclusionRule.HeaderColour = System.Drawing.Color.FromArgb(CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer))
        Me.GbxExclusionRule.Location = New System.Drawing.Point(549, 45)
        Me.GbxExclusionRule.MainColour = System.Drawing.Color.FromArgb(CType(CType(54,Byte),Integer), CType(CType(54,Byte),Integer), CType(CType(54,Byte),Integer))
        Me.GbxExclusionRule.Name = "GbxExclusionRule"
        Me.GbxExclusionRule.Size = New System.Drawing.Size(143, 349)
        Me.GbxExclusionRule.TabIndex = 2
        Me.GbxExclusionRule.Text = "              Rule"
        Me.GbxExclusionRule.TextColour = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer))
        '
        'ChbExclusionCheckAll
        '
        Me.ChbExclusionCheckAll.BaseColour = System.Drawing.Color.FromArgb(CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer))
        Me.ChbExclusionCheckAll.BorderColour = System.Drawing.Color.DimGray
        Me.ChbExclusionCheckAll.Checked = false
        Me.ChbExclusionCheckAll.CheckedColour = System.Drawing.Color.BlueViolet
        Me.ChbExclusionCheckAll.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ChbExclusionCheckAll.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.ChbExclusionCheckAll.FontColour = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.ChbExclusionCheckAll.Location = New System.Drawing.Point(4, 50)
        Me.ChbExclusionCheckAll.Name = "ChbExclusionCheckAll"
        Me.ChbExclusionCheckAll.Size = New System.Drawing.Size(136, 22)
        Me.ChbExclusionCheckAll.TabIndex = 65
        Me.ChbExclusionCheckAll.Tag = ""
        Me.ChbExclusionCheckAll.Text = "Check all"
        '
        'ChbExclusionHideCalls
        '
        Me.ChbExclusionHideCalls.BaseColour = System.Drawing.Color.FromArgb(CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer))
        Me.ChbExclusionHideCalls.BorderColour = System.Drawing.Color.DimGray
        Me.ChbExclusionHideCalls.Checked = false
        Me.ChbExclusionHideCalls.CheckedColour = System.Drawing.Color.BlueViolet
        Me.ChbExclusionHideCalls.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ChbExclusionHideCalls.Font = New System.Drawing.Font("Segoe UI", 9!)
        Me.ChbExclusionHideCalls.FontColour = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.ChbExclusionHideCalls.Location = New System.Drawing.Point(4, 206)
        Me.ChbExclusionHideCalls.Name = "ChbExclusionHideCalls"
        Me.ChbExclusionHideCalls.Size = New System.Drawing.Size(136, 22)
        Me.ChbExclusionHideCalls.TabIndex = 64
        Me.ChbExclusionHideCalls.Tag = ""
        Me.ChbExclusionHideCalls.Text = "Hide calls"
        Me.ChbExclusionHideCalls.Visible = false
        '
        'ChbAllEntities
        '
        Me.ChbAllEntities.BaseColour = System.Drawing.Color.FromArgb(CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer))
        Me.ChbAllEntities.BorderColour = System.Drawing.Color.DimGray
        Me.ChbAllEntities.Checked = false
        Me.ChbAllEntities.CheckedColour = System.Drawing.Color.BlueViolet
        Me.ChbAllEntities.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ChbAllEntities.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.ChbAllEntities.FontColour = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.ChbAllEntities.Location = New System.Drawing.Point(4, 323)
        Me.ChbAllEntities.Name = "ChbAllEntities"
        Me.ChbAllEntities.Size = New System.Drawing.Size(136, 22)
        Me.ChbAllEntities.TabIndex = 18
        Me.ChbAllEntities.Tag = ""
        Me.ChbAllEntities.Text = "All entities"
        '
        'ChbExclusionInvalidOpCodes
        '
        Me.ChbExclusionInvalidOpCodes.BaseColour = System.Drawing.Color.FromArgb(CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer))
        Me.ChbExclusionInvalidOpCodes.BorderColour = System.Drawing.Color.DimGray
        Me.ChbExclusionInvalidOpCodes.Checked = false
        Me.ChbExclusionInvalidOpCodes.CheckedColour = System.Drawing.Color.BlueViolet
        Me.ChbExclusionInvalidOpCodes.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ChbExclusionInvalidOpCodes.Font = New System.Drawing.Font("Segoe UI", 9!)
        Me.ChbExclusionInvalidOpCodes.FontColour = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.ChbExclusionInvalidOpCodes.Location = New System.Drawing.Point(4, 234)
        Me.ChbExclusionInvalidOpCodes.Name = "ChbExclusionInvalidOpCodes"
        Me.ChbExclusionInvalidOpCodes.Size = New System.Drawing.Size(136, 22)
        Me.ChbExclusionInvalidOpCodes.TabIndex = 63
        Me.ChbExclusionInvalidOpCodes.Tag = ""
        Me.ChbExclusionInvalidOpCodes.Text = "Invalid OpCodes"
        Me.ChbExclusionInvalidOpCodes.Visible = false
        '
        'ChbExclusionBooleanEncrypt
        '
        Me.ChbExclusionBooleanEncrypt.BaseColour = System.Drawing.Color.FromArgb(CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer))
        Me.ChbExclusionBooleanEncrypt.BorderColour = System.Drawing.Color.DimGray
        Me.ChbExclusionBooleanEncrypt.Checked = false
        Me.ChbExclusionBooleanEncrypt.CheckedColour = System.Drawing.Color.BlueViolet
        Me.ChbExclusionBooleanEncrypt.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ChbExclusionBooleanEncrypt.Font = New System.Drawing.Font("Segoe UI", 9!)
        Me.ChbExclusionBooleanEncrypt.FontColour = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.ChbExclusionBooleanEncrypt.Location = New System.Drawing.Point(4, 178)
        Me.ChbExclusionBooleanEncrypt.Name = "ChbExclusionBooleanEncrypt"
        Me.ChbExclusionBooleanEncrypt.Size = New System.Drawing.Size(136, 22)
        Me.ChbExclusionBooleanEncrypt.TabIndex = 60
        Me.ChbExclusionBooleanEncrypt.Tag = ""
        Me.ChbExclusionBooleanEncrypt.Text = "Booleans encryption"
        Me.ChbExclusionBooleanEncrypt.Visible = false
        '
        'ChbExclusionRenaming
        '
        Me.ChbExclusionRenaming.BaseColour = System.Drawing.Color.FromArgb(CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer))
        Me.ChbExclusionRenaming.BorderColour = System.Drawing.Color.DimGray
        Me.ChbExclusionRenaming.Checked = false
        Me.ChbExclusionRenaming.CheckedColour = System.Drawing.Color.BlueViolet
        Me.ChbExclusionRenaming.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ChbExclusionRenaming.Font = New System.Drawing.Font("Segoe UI", 9!)
        Me.ChbExclusionRenaming.FontColour = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.ChbExclusionRenaming.Location = New System.Drawing.Point(4, 94)
        Me.ChbExclusionRenaming.Name = "ChbExclusionRenaming"
        Me.ChbExclusionRenaming.Size = New System.Drawing.Size(136, 22)
        Me.ChbExclusionRenaming.TabIndex = 59
        Me.ChbExclusionRenaming.Tag = ""
        Me.ChbExclusionRenaming.Text = "Renaming"
        Me.ChbExclusionRenaming.Visible = false
        '
        'ChbExclusionIntegersEncode
        '
        Me.ChbExclusionIntegersEncode.BaseColour = System.Drawing.Color.FromArgb(CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer))
        Me.ChbExclusionIntegersEncode.BorderColour = System.Drawing.Color.DimGray
        Me.ChbExclusionIntegersEncode.Checked = false
        Me.ChbExclusionIntegersEncode.CheckedColour = System.Drawing.Color.BlueViolet
        Me.ChbExclusionIntegersEncode.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ChbExclusionIntegersEncode.Font = New System.Drawing.Font("Segoe UI", 9!)
        Me.ChbExclusionIntegersEncode.FontColour = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.ChbExclusionIntegersEncode.Location = New System.Drawing.Point(4, 150)
        Me.ChbExclusionIntegersEncode.Name = "ChbExclusionIntegersEncode"
        Me.ChbExclusionIntegersEncode.Size = New System.Drawing.Size(136, 22)
        Me.ChbExclusionIntegersEncode.TabIndex = 58
        Me.ChbExclusionIntegersEncode.Tag = ""
        Me.ChbExclusionIntegersEncode.Text = "Integers encoding"
        Me.ChbExclusionIntegersEncode.Visible = false
        '
        'ChbExclusionStringsEncrypt
        '
        Me.ChbExclusionStringsEncrypt.BaseColour = System.Drawing.Color.FromArgb(CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer))
        Me.ChbExclusionStringsEncrypt.BorderColour = System.Drawing.Color.DimGray
        Me.ChbExclusionStringsEncrypt.Checked = false
        Me.ChbExclusionStringsEncrypt.CheckedColour = System.Drawing.Color.BlueViolet
        Me.ChbExclusionStringsEncrypt.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ChbExclusionStringsEncrypt.Font = New System.Drawing.Font("Segoe UI", 9!)
        Me.ChbExclusionStringsEncrypt.FontColour = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.ChbExclusionStringsEncrypt.Location = New System.Drawing.Point(4, 122)
        Me.ChbExclusionStringsEncrypt.Name = "ChbExclusionStringsEncrypt"
        Me.ChbExclusionStringsEncrypt.Size = New System.Drawing.Size(136, 22)
        Me.ChbExclusionStringsEncrypt.TabIndex = 57
        Me.ChbExclusionStringsEncrypt.Tag = ""
        Me.ChbExclusionStringsEncrypt.Text = "Strings encryption"
        Me.ChbExclusionStringsEncrypt.Visible = false
        '
        'ChbExclusion
        '
        Me.ChbExclusion.BaseColour = System.Drawing.Color.FromArgb(CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer), CType(CType(42,Byte),Integer))
        Me.ChbExclusion.BorderColour = System.Drawing.Color.DimGray
        Me.ChbExclusion.Checked = false
        Me.ChbExclusion.CheckedColour = System.Drawing.Color.BlueViolet
        Me.ChbExclusion.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ChbExclusion.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.ChbExclusion.FontColour = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.ChbExclusion.Location = New System.Drawing.Point(4, 4)
        Me.ChbExclusion.Name = "ChbExclusion"
        Me.ChbExclusion.Size = New System.Drawing.Size(22, 22)
        Me.ChbExclusion.TabIndex = 16
        Me.ChbExclusion.Tag = ""
        '
        'Frm_Exclusion
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(704, 697)
        Me.ControlBox = false
        Me.Controls.Add(Me.Frm_ExclusionThemeContainer)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.MaximizeBox = false
        Me.Name = "Frm_Exclusion"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.TransparencyKey = System.Drawing.Color.Fuchsia
        Me.Frm_ExclusionThemeContainer.ResumeLayout(false)
        Me.GbxExclusionDetails.ResumeLayout(false)
        Me.GbxExclusionDetails.PerformLayout
        Me.GbxExclusionViewer.ResumeLayout(false)
        Me.GbxExclusionRule.ResumeLayout(false)
        Me.ResumeLayout(false)

End Sub
    Friend WithEvents Frm_ExclusionThemeContainer As LogInThemeContainer
    Friend WithEvents BgwRenameTask As System.ComponentModel.BackgroundWorker
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents GbxExclusionRule As XertzLoginTheme.LogInGroupBox
    Friend WithEvents ChbExclusion As XertzLoginTheme.LogInCheckBox
    Friend WithEvents ChbAllEntities As XertzLoginTheme.LogInCheckBox
    Friend WithEvents BgwExclusion As System.ComponentModel.BackgroundWorker
    Friend WithEvents TvExclusion As XertzLoginTheme.TreeViewEx
    Friend WithEvents ChbExclusionBooleanEncrypt As XertzLoginTheme.LogInCheckBox
    Friend WithEvents ChbExclusionIntegersEncode As XertzLoginTheme.LogInCheckBox
    Friend WithEvents ChbExclusionStringsEncrypt As XertzLoginTheme.LogInCheckBox
    Friend WithEvents ChbExclusionInvalidOpCodes As XertzLoginTheme.LogInCheckBox
    Friend WithEvents ChbExclusionRenaming As XertzLoginTheme.LogInCheckBox
    Friend WithEvents GbxExclusionViewer As XertzLoginTheme.LogInGroupBox
    Friend WithEvents LblExclusionTotal As XertzLoginTheme.LogInLabel
    Friend WithEvents ChbExclusionHideCalls As XertzLoginTheme.LogInCheckBox
    Friend WithEvents ChbExclusionCheckAll As XertzLoginTheme.LogInCheckBox
    Friend WithEvents GbxExclusionDetails As LoginTheme.XertzLoginTheme.LogInGroupBox
    Friend WithEvents LblExclusionDetailsEvents As LoginTheme.XertzLoginTheme.LogInLabel
    Friend WithEvents LblExclusionDetailsProperties As LoginTheme.XertzLoginTheme.LogInLabel
    Friend WithEvents LblExclusionDetailsFields As LoginTheme.XertzLoginTheme.LogInLabel
    Friend WithEvents LblExclusionDetailsMethods As LoginTheme.XertzLoginTheme.LogInLabel
    Friend WithEvents LblExclusionDetailsTypes As LoginTheme.XertzLoginTheme.LogInLabel
End Class
