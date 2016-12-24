Imports System.Windows.Forms
Imports System.Drawing
Imports Mono.Cecil
Imports Helper.CecilHelper
Imports Helper.UtilsHelper
Imports System.Security.Cryptography
Imports System.IO
Imports System.Text

Namespace Core.Obfuscation.Exclusion
    Public NotInheritable Class ExclusionTreeview

#Region " Fields "
        Private m_AssDef As AssemblyDefinition = Nothing
#End Region

#Region " Properties "
        Public Property filepath() As String
        Public Property fileMD5 As String
#End Region

#Region " Constructor "
        Sub New(FilePath$)
            _filepath = FilePath$
            fileMD5 = Functions.GetMD5HashFromFile(FilePath)
        End Sub
#End Region

#Region " Methods "

        Public Function LoadTreeNode() As TreeNode
            m_AssDef = AssemblyDefinition.ReadAssembly(_filepath)

            Dim assNode As New TreeNode(m_AssDef.FullName)
            assNode.ExpandAll()
            SetImageKey(assNode, "assembly.png")

            Dim namespaces As New Dictionary(Of String, TreeNode)

            For Each Mdef In m_AssDef.Modules
                Dim libNode As New TreeNode(Mdef.Name)
                libNode.ExpandAll()
                SetImageKey(libNode, "library.png")
                For Each tDef In Mdef.Types
                    Dim tNode As TreeNode = Nothing
                    If Not namespaces.ContainsKey(tDef.Namespace) Then
                        tNode = New TreeNode(tDef.Namespace) With { _
                            .Tag = New ExclusionState(False, tDef, ExclusionState.mType.Namespaces)}
                        SetImageKey(tNode, "namespace.png")
                        namespaces.Add(tDef.Namespace, tNode)
                        libNode.Nodes.Add(tNode)
                    Else
                        tNode = namespaces.Item(tDef.Namespace)
                    End If
                    If (tNode.Text = tDef.Namespace) Then
                        Dim destNode As New TreeNode(tDef.Name)
                        For Each ntDef In tDef.NestedTypes
                            Dim ntNode As New TreeNode(ntDef.Name)
                            CreateMembers(ntDef, ntNode)
                            destNode.Nodes.Add(ntNode)
                        Next
                        CreateMembers(tDef, destNode)
                        tNode.Nodes.Add(destNode)
                    Else
                        Continue For
                    End If
                Next
                assNode.Nodes.Add(libNode)
            Next
            namespaces.Clear()
            Return assNode
        End Function

        Private Sub CreateMembers(ByRef OriginalType As TypeDefinition, ByRef DestNode As TreeNode)

            DestNode.Tag = New ExclusionState(False, OriginalType, ExclusionState.mType.Types)
            SetImageKey(DestNode, GetTypeImage(OriginalType))

            For Each mDef As MethodDefinition In OriginalType.Methods
                If Not Finder.AccessorMethods(OriginalType).Contains(mDef) Then
                    CreateMethodNode(mDef, DestNode)
                End If
            Next

            For Each fieldDef In OriginalType.Fields
                Dim fieldNode = New TreeNode(fieldDef.Name.ToString & " : " & fieldDef.FieldType.Name) With { _
                    .Tag = New ExclusionState(False, fieldDef, ExclusionState.mType.Fields)}
                SetImageKey(fieldNode, "field.png")

                DestNode.Nodes.Add(fieldNode)
            Next

            For Each propDef In OriginalType.Properties
                Dim propNode = New TreeNode(propDef.Name.ToString & " : " & propDef.PropertyType.Name) With { _
                    .Tag = New ExclusionState(False, propDef, ExclusionState.mType.Properties)}
                SetImageKey(propNode, "property.png")

                If Not propDef.GetMethod Is Nothing Then CreateMethodNode(propDef.GetMethod, propNode)
                If Not propDef.SetMethod Is Nothing Then CreateMethodNode(propDef.SetMethod, propNode)

                For Each def In propDef.OtherMethods
                    CreateMethodNode(def, propNode)
                Next

                DestNode.Nodes.Add(propNode)
            Next

            For Each EventDef In OriginalType.Events
                Dim eventNode = New TreeNode(EventDef.Name) With { _
                    .Tag = New ExclusionState(False, EventDef, ExclusionState.mType.Events)}
                SetImageKey(eventNode, "event.png")

                If Not EventDef.AddMethod Is Nothing Then CreateMethodNode(EventDef.AddMethod, eventNode)
                If Not EventDef.RemoveMethod Is Nothing Then CreateMethodNode(EventDef.RemoveMethod, eventNode)

                For Each def In EventDef.OtherMethods
                    CreateMethodNode(def, eventNode)
                Next

                DestNode.Nodes.Add(eventNode)
            Next
        End Sub

        Private Sub CreateMethodNode(mDef As MethodDefinition, DestNode As TreeNode)
            Dim methodNode As New TreeNode(mDef.Name) With { _
                .Tag = New ExclusionState(False, mDef, ExclusionState.mType.Methods)}
            SetImageKey(methodNode, GetMethodImage(mDef))

            Dim tmpStr As String = Nothing

            For Each paramDef In mDef.Parameters
                If Not paramDef.ParameterType.Name = mDef.DeclaringType.Name Then
                    tmpStr &= String.Concat(paramDef.ParameterType.Name, ",")
                End If
            Next

            methodNode.Text &= String.Concat("(", (If(tmpStr IsNot Nothing, tmpStr.TrimEnd(New Char() {","c, " "c}), Nothing)), ")")
            DestNode.Nodes.Add(methodNode)
        End Sub

        Private Function GetMethodImage(mdef As MethodDefinition) As String
            Dim str = "Method.png"
            If mdef.IsConstructor Then
                str = "Constructor.png"
            ElseIf mdef.IsPInvokeImpl Then
                str = "PInvokeMethod.png"
            End If
            Return str
        End Function

        Private Function GetTypeImage(mdef As TypeDefinition) As String
            Dim str = "class.png"
            If mdef.IsInterface Then
                str = "interface.png"
            ElseIf mdef.IsEnum Then
                str = "enum.png"
            ElseIf mdef.IsValueType Then
                str = "enumvalue.png"
            ElseIf (mdef.BaseType IsNot Nothing) AndAlso (mdef.BaseType.Name.ToLower.Contains("delegate")) Then
                str = "delegate.png"
            ElseIf mdef.IsSealed Then
                str = "staticclass.png"
            End If
            Return str
        End Function

        Private Sub SetImageKey(node As TreeNode, imageKey$)
            node.ImageKey = imageKey
            node.SelectedImageKey = imageKey
        End Sub

        Public Function isRenamable(Obj As Object) As Boolean
            If Obj Is Nothing Then Return False
            Dim b As Boolean = False
            If TypeOf TryCast(Obj.member, MethodDefinition) Is MethodDefinition Then
                b = NameChecker.IsRenamable(TryCast(Obj.member, MethodDefinition), True)
            ElseIf TypeOf TryCast(Obj.member, TypeDefinition) Is TypeDefinition Then
                b = NameChecker.IsRenamable(TryCast(Obj.member, TypeDefinition))
            ElseIf TypeOf TryCast(Obj.member, EventDefinition) Is EventDefinition Then
                b = NameChecker.IsRenamable(TryCast(Obj.member, EventDefinition))
            ElseIf TypeOf TryCast(Obj.member, PropertyDefinition) Is PropertyDefinition Then
                b = NameChecker.IsRenamable(TryCast(Obj.member, PropertyDefinition))
            ElseIf TypeOf TryCast(Obj.member, FieldDefinition) Is FieldDefinition Then
                b = NameChecker.IsRenamable(TryCast(Obj.member, FieldDefinition))
            End If
            Return b
        End Function

        Public Function isTypedef(n As Object) As Boolean
            If n Is Nothing Then Return False
            Return TypeOf TryCast(n.member, TypeDefinition) Is TypeDefinition
        End Function

        Public Function getEntitiesVal(n As Object) As Boolean
            If n Is Nothing Then Return False
            Return CBool(n.AllEntities)
        End Function

        Public Function isExclude(n As Object) As Boolean
            If n Is Nothing Then Return False
            Return n.exclude
        End Function

        Public Function isStringsEncryptExclude(n As Object) As Boolean
            If n Is Nothing Then Return False
            Return n.stringEncrypt
        End Function

        Public Function isIntegersEncodingExclude(n As Object) As Boolean
            If n Is Nothing Then Return False
            Return n.integerEncoding
        End Function

        Public Function isBooleansEncryptExclude(n As Object) As Boolean
            If n Is Nothing Then Return False
            Return n.booleanEncrypt
        End Function

        Public Function isRenamingExclude(n As Object) As Boolean
            If n Is Nothing Then Return False
            Return n.Renaming
        End Function

        Public Function isInvalidOpcodesExclude(n As Object) As Boolean
            If n Is Nothing Then Return False
            Return n.InvalidOpcodes
        End Function

        Public Function isHideCallsExclude(n As Object) As Boolean
            If n Is Nothing Then Return False
            Return n.HideCalls
        End Function
#End Region

    End Class
End Namespace
