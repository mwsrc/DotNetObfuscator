Imports Mono.Cecil
Imports Mono.Cecil.Cil
Imports Implementer.Engine.Context
Imports Helper.RandomizeHelper
Imports Helper.CecilHelper
Imports Implementer.core.Obfuscation.Exclusion
Imports Mono.Collections

Namespace Engine.Processing
    ''' <summary>
    ''' INFO : This is the forth step of the renamer library. 
    '''        This is the core of the rename library !
    ''' </summary>
    Friend NotInheritable Class Renamer

#Region " Methods "
        ''' <summary>
        ''' INFO : Rename the method. Return methodDefinition member.
        ''' </summary>
        ''' <param name="method"></param>
        Friend Shared Function RenameMethod(type As TypeDefinition, method As MethodDefinition) As MethodDefinition
            Dim MethodOriginal$ = method.Name
            Dim MethodPublicObf$ = Randomizer.GenerateNew()

            If method.IsPInvokeImpl Then
                If method.PInvokeInfo.EntryPoint = String.Empty Then method.PInvokeInfo.EntryPoint = MethodOriginal
            End If

            If Not Finder.FindGenericParameter(method) AndAlso Not Finder.FindCustomAttributeByName(method, "DebuggerHiddenAttribute") Then
                method.Name = Mapping.RenameMethodMember(method, MethodPublicObf)
            End If

            Return method
        End Function

        ''' <summary>
        ''' INFO : Rename Parameters from method.
        ''' </summary>
        ''' <param name="method"></param>
        Friend Shared Sub RenameParameters(method As MethodDefinition)
            If method.HasParameters Then
                For Each ParDef As ParameterDefinition In method.Parameters
                    If ParDef.CustomAttributes.Count = 0 Then
                        ParDef.Name = Mapping.RenameParamMember(ParDef, Randomizer.GenerateNew())
                    End If
                Next
            End If
            If method.HasGenericParameters Then
                For Each GenPar As GenericParameter In method.GenericParameters
                    If GenPar.CustomAttributes.Count = 0 Then
                        GenPar.Name = Mapping.RenameGenericParamMember(GenPar, Randomizer.GenerateNew())
                    End If
                Next
            End If
        End Sub

        ''' <summary>
        ''' INFO : Rename Variables from method.
        ''' </summary>
        ''' <param name="method"></param>
        Friend Shared Sub RenameVariables(Method As MethodDefinition)
            If Method.HasBody Then
                For Each vari In Method.Body.Variables
                    vari.Name = Mapping.RenameVariableMember(vari, Randomizer.GenerateNew())
                Next
            End If
        End Sub

        ''' <summary>
        ''' INFO : Rename embedded Resources from Resources dir and updates method bodies.
        ''' </summary>
        ''' <param name="TypeDef"></param>
        ''' <param name="NamespaceOriginal"></param>
        ''' <param name="NamespaceObfuscated"></param>
        ''' <param name="TypeOriginal"></param>
        ''' <param name="TypeObfuscated"></param>
        Friend Shared Sub RenameResources(TypeDef As TypeDefinition, ByRef NamespaceOriginal$, ByRef NamespaceObfuscated$, TypeOriginal$, TypeObfuscated$)
            Dim ModuleDef As ModuleDefinition = TypeDef.Module

            For Each EmbRes As Resource In ModuleDef.Resources
                If Utils.isStronglyTypedResourceBuilder(TypeDef) Then
                    If NamespaceOriginal.EndsWith(".My.Resources") Then
                        If EmbRes.Name = NamespaceOriginal.Replace(".My.Resources", "") & "." & TypeOriginal & ".resources" Then
                            EmbRes.Name = If(NamespaceObfuscated = String.Empty, TypeObfuscated & ".resources", NamespaceObfuscated & "." & TypeObfuscated & ".resources")
                        End If
                    Else
                        If EmbRes.Name = NamespaceOriginal & "." & TypeOriginal & ".resources" Then
                            EmbRes.Name = If(NamespaceObfuscated = String.Empty, TypeObfuscated & ".resources", NamespaceObfuscated & "." & TypeObfuscated & ".resources")
                        End If
                    End If
                Else
                    If EmbRes.Name = NamespaceOriginal & "." & TypeOriginal & ".resources" Then
                        EmbRes.Name = If(NamespaceObfuscated = String.Empty, TypeObfuscated & ".resources", NamespaceObfuscated & "." & TypeObfuscated & ".resources")
                    End If
                End If
            Next

            If TypeDef.HasMethods Then
                For Each method In TypeDef.Methods
                    If method.HasBody Then
                        For Each inst In method.Body.Instructions
                            If inst.OpCode = OpCodes.Ldstr Then
                                If NamespaceOriginal.EndsWith(".My.Resources") Then
                                    If inst.Operand.ToString() = (NamespaceOriginal.Replace(".My.Resources", "") & ".Resources") Then
                                        inst.Operand = If(NamespaceObfuscated = String.Empty, TypeObfuscated, NamespaceObfuscated & "." & TypeObfuscated)
                                    End If
                                Else
                                    If inst.Operand.ToString() = (NamespaceOriginal & "." & TypeOriginal) Then
                                        inst.Operand = If(NamespaceObfuscated = String.Empty, TypeObfuscated, NamespaceObfuscated & "." & TypeObfuscated)
                                    End If
                                End If
                            End If
                        Next
                    End If
                Next
            End If
        End Sub

        ''' <summary>
        ''' INFO : Rename embedded Resources from Resources dir and from ResourcesManager method.
        ''' </summary>
        ''' <param name="typeDef"></param>
        Friend Shared Sub RenameResourceManager(typeDef As TypeDefinition)
            For Each pr In (From p In typeDef.Properties
                            Where Not p.GetMethod Is Nothing AndAlso p.GetMethod.Name = "get_ResourceManager" AndAlso p.GetMethod.HasBody AndAlso p.GetMethod.Body.Instructions.Count <> 0
                            Select p)
                For Each instruction In pr.GetMethod.Body.Instructions
                    If TypeOf instruction.Operand Is String Then
                        Dim NewResManagerName$ = instruction.Operand
                        For Each EmbRes As EmbeddedResource In typeDef.Module.Resources
                            If EmbRes.Name = instruction.Operand & ".resources" Then
                                NewResManagerName = Randomizer.GenerateNew()
                                EmbRes.Name = NewResManagerName & ".resources"
                            End If
                        Next
                        instruction.Operand = NewResManagerName
                    End If
                Next
            Next
        End Sub

        Friend Shared Sub RenameSettings(mDef As MethodDefinition, originalN$, obfuscatedN$)
            If Not mDef Is Nothing Then
                If Not mDef.DeclaringType.BaseType Is Nothing AndAlso mDef.DeclaringType.BaseType.Name = "ApplicationSettingsBase" Then
                    If mDef.HasBody AndAlso mDef.Body.Instructions.Count <> 0 Then
                        For Each instruction In mDef.Body.Instructions
                            If TypeOf instruction.Operand Is String Then
                                Dim Name$ = instruction.Operand
                                If originalN = Name Then
                                    If mDef.Name.StartsWith("set_") Then
                                        mDef.Name = "set_" & obfuscatedN
                                    ElseIf mDef.Name.StartsWith("get_") Then
                                        mDef.Name = "get_" & obfuscatedN
                                    End If
                                    instruction.Operand = obfuscatedN
                                End If
                            End If
                        Next
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' INFO : Rename Property.
        ''' </summary>
        ''' <param name="prop"></param>
        ''' <param name="obfuscatedN"></param>
        Friend Shared Sub RenameProperty(ByRef prop As PropertyDefinition, obfuscatedN$)
            prop.Name = Mapping.RenamePropertyMember(prop, obfuscatedN)

            If Not prop.GetMethod Is Nothing Then
                Dim meth = Renamer.RenameMethod(prop.DeclaringType, prop.GetMethod)
                Renamer.RenameParameters(meth)
                Renamer.RenameVariables(meth)
            End If
            If Not prop.SetMethod Is Nothing Then
                Dim meth = Renamer.RenameMethod(prop.DeclaringType, prop.SetMethod)
                Renamer.RenameParameters(meth)
                Renamer.RenameVariables(meth)
            End If

            For Each m In (From p In prop.OtherMethods
                           Where Not p Is Nothing AndAlso NameChecker.IsRenamable(p)
                           Select p)
                Dim meth = Renamer.RenameMethod(prop.DeclaringType, m)
                Renamer.RenameParameters(meth)
                Renamer.RenameVariables(meth)
            Next
        End Sub

        ''' <summary>
        ''' INFO : Rename Field.
        ''' </summary>
        ''' <param name="field"></param>
        ''' <param name="obfuscatedN"></param>
        Friend Shared Sub RenameField(field As FieldDefinition, obfuscatedN$)
            field.Name = Mapping.RenameFieldMember(field, obfuscatedN)
        End Sub

        ''' <summary>
        ''' INFO : Rename Event.
        ''' </summary>
        ''' <param name="events"></param>
        ''' <param name="obfuscatedN"></param>
        Friend Shared Sub RenameEvent(ByRef events As EventDefinition, obfuscatedN$)
            events.Name = Mapping.RenameEventMember(events, obfuscatedN)
        End Sub

        ''' <summary>
        ''' INFO : Rename CustomAttributes.
        ''' </summary>
        ''' <remarks>
        ''' REMARKS : Only AccessedThroughPropertyAttribute attribute is renamed to prevent de4Dot to retrieve original names.
        ''' </remarks>
        ''' <param name="type"></param>
        ''' <param name="prop"></param>
        ''' <param name="originalN"></param>
        ''' <param name="obfuscatedN"></param> 
        Friend Shared Sub RenameCustomAttributes(type As TypeDefinition, prop As PropertyDefinition, originalN$, obfuscatedN$)
            If type.HasFields Then
                For Each field As FieldDefinition In (From f In type.Fields
                                                      Where f.IsPrivate AndAlso f.HasCustomAttributes
                                                      Select f)
                    For Each ca In (From c In field.CustomAttributes
                                    Where c.AttributeType.Name = "AccessedThroughPropertyAttribute" AndAlso c.HasConstructorArguments AndAlso c.ConstructorArguments(0).Value = originalN
                                    Select c)
                        ca.ConstructorArguments(0) = New CustomAttributeArgument(ca.AttributeType, obfuscatedN)
                        RenameProperty(prop, obfuscatedN)
                        Exit For
                    Next
                Next
            End If
            RenameCustomAttributesValues(prop)
        End Sub

        Friend Shared Sub RenameCustomAttributesValues(member As Object)
            If member.HasCustomAttributes Then
                For Each ca As CustomAttribute In member.CustomAttributes
                    If Not ca Is Nothing Then
                        If ca.AttributeType.Name = "CategoryAttribute" OrElse ca.AttributeType.Name = "DescriptionAttribute" Then
                            If ca.HasConstructorArguments Then
                                ca.ConstructorArguments(0) = New CustomAttributeArgument(ca.AttributeType, Randomizer.GenerateNew())
                            End If
                        End If
                    End If
                Next
            End If
        End Sub

        Friend Shared Sub RenameInitializeComponentsValues(TypeDef As TypeDefinition, OriginalKeyName$, NewKeyName$, ByVal Properties As Boolean)
            Dim methodSearch As MethodDefinition = Finder.FindMethod(TypeDef, "InitializeComponent")
            If Not methodSearch Is Nothing Then
                If methodSearch.HasBody Then
                    If methodSearch.Body.Instructions.Count <> 0 Then
                        For Each instruction As Cil.Instruction In methodSearch.Body.Instructions
                            If TypeOf instruction.Operand Is String Then
                                If Properties Then
                                    If Not instruction.Previous Is Nothing Then
                                        If instruction.Previous.OpCode = Mono.Cecil.Cil.OpCodes.Callvirt AndAlso instruction.Previous.Operand.ToString.EndsWith("get_" & OriginalKeyName & "()") Then
                                            If CStr(instruction.Operand) = OriginalKeyName Then
                                                instruction.Operand = NewKeyName
                                            End If
                                        End If
                                    End If
                                Else
                                    If Not instruction.Next Is Nothing Then
                                        If instruction.Next.OpCode = Mono.Cecil.Cil.OpCodes.Callvirt AndAlso instruction.Next.ToString.EndsWith("set_Name(System.String)") Then
                                            If CStr(instruction.Operand) = OriginalKeyName Then
                                                instruction.Operand = NewKeyName
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        Next
                    End If
                End If
            End If
        End Sub
#End Region

    End Class
End Namespace
