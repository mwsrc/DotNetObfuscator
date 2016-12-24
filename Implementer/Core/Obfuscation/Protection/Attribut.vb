Imports Mono.Cecil
Imports System.IO
Imports Mono.Cecil.Cil
Imports System.Reflection

Namespace Core.Obfuscation.Protection
    Public NotInheritable Class Attribut

#Region " Fields "
        Private Shared attribs As String()
#End Region

#Region " Constructor "
        Shared Sub New()
            attribs = New String() {"DotNetPatcherObfuscatorAttribute", "DotNetPatcherPackerAttribute", "DotfuscatorAttribute", "ConfusedByAttribute", _
                                            "ObfuscatedByGoliath", "dotNetProtector", "PoweredByAttribute", "AssemblyInfoAttribute"}
        End Sub
#End Region

#Region " Methods "
        Public Shared Sub DoInjection(assdef As AssemblyDefinition, pack As Boolean)
            assdef.MainModule.Resources.Add(New EmbeddedResource(If(pack, My.Resources.DnpPattribute, My.Resources.DnpOattribute), ManifestResourceAttributes.Private, File.ReadAllBytes(IO.Path.GetTempFileName)))

            For Each it In attribs
                Dim item As New TypeDefinition("", it, Mono.Cecil.TypeAttributes.AnsiClass, assdef.MainModule.Import(GetType(Attribute)))
                If it = "DotNetPatcherObfuscatorAttribute" AndAlso pack = False Then
                    creatAttribut(assdef, item, it)
                ElseIf it = "DotNetPatcherPackerAttribute" AndAlso pack = True Then
                    creatAttribut(assdef, item, it)
                ElseIf it = "AssemblyInfoAttribute" Then
                    creatAttribut(assdef, item, it)
                Else
                    If it = "DotNetPatcherObfuscatorAttribute" Then
                    ElseIf it = "DotNetPatcherPackerAttribute" Then
                    ElseIf it = "AssemblyInfoAttribute" Then
                    Else
                        assdef.MainModule.Types.Add(item)
                    End If
                End If
            Next
        End Sub

        Private Shared Sub creatAttribut(assdef As AssemblyDefinition, item As TypeDefinition, it$)
            Dim method As New MethodDefinition(".ctor", (Mono.Cecil.MethodAttributes.CompilerControlled Or (Mono.Cecil.MethodAttributes.FamANDAssem Or (Mono.Cecil.MethodAttributes.Family Or (Mono.Cecil.MethodAttributes.RTSpecialName Or Mono.Cecil.MethodAttributes.SpecialName)))), assdef.MainModule.TypeSystem.Void)
            method.Parameters.Add(New ParameterDefinition(assdef.MainModule.TypeSystem.String))

            Dim iLProc As ILProcessor = method.Body.GetILProcessor
            With iLProc
                .Emit(OpCodes.Ldarg_0)
                .Emit(OpCodes.Call, assdef.MainModule.Import(GetType(Attribute).GetConstructor((BindingFlags.NonPublic Or BindingFlags.Instance), Nothing, Type.EmptyTypes, Nothing)))
                .Emit(OpCodes.Ret)
            End With

            item.Methods.Add(method)
            assdef.MainModule.Types.Add(item)
            Dim att As New CustomAttribute(method)
            att.ConstructorArguments.Add(New CustomAttributeArgument(assdef.MainModule.TypeSystem.String, If(it = "AssemblyInfoAttribute", "", String.Format(("DotNetPatcher v" & GetType(Attribut).Assembly.GetName.Version.ToString), New Object(0 - 1) {}))))
            assdef.MainModule.CustomAttributes.Add(att)
            assdef.CustomAttributes.Add(att)
        End Sub
#End Region

    End Class
End Namespace

