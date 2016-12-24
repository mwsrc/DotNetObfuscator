Imports System.IO
Imports Implementer.Core.Obfuscation.Exclusion.ReflectionAnalyzer

Namespace Core.Obfuscation.Exclusion
    Public NotInheritable Class ExclusionReflection

#Region " Constants "
        Private Const database = "Microsoft.VisualBasic.CompilerServices.LateBinding" & ChrW(13) & ChrW(10) & _
                                    "LateCall[0:This,1:Type,2:Target]" & ChrW(13) & ChrW(10) & _
                                    "LateGet[0:This,1:Type,2:Target]" & ChrW(13) & ChrW(10) & _
                                    "LateSet[0:This,1:Type,2:Target]" & ChrW(13) & ChrW(10) & _
                                    "LateSetComplex[0:This,1:Type,2:Target]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                                    "Microsoft.VisualBasic.CompilerServices.NewLateBinding" & ChrW(13) & ChrW(10) & _
                                    "LateCall[0:This,1:Type,2:Target]" & ChrW(13) & ChrW(10) & _
                                    "LateCanEvaluate[0:This,1:Type,2:Target]" & ChrW(13) & ChrW(10) & _
                                    "LateGet[0:This,1:Type,2:Target]" & ChrW(13) & ChrW(10) & _
                                    "LateSet[0:This,1:Type,2:Target]" & ChrW(13) & ChrW(10) & _
                                    "LateSetComplex[0:This,1:Type,2:Target]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                                    "System.Type" & ChrW(13) & ChrW(10) & _
                                    "GetEvent[0:Type,1:Target]" & ChrW(13) & ChrW(10) & _
                                    "GetField[0:Type,1:Target]" & ChrW(13) & ChrW(10) & _
                                    "GetMember[0:Type,1:Target]" & ChrW(13) & ChrW(10) & _
                                    "GetMethod[0:Type,1:Target]" & ChrW(13) & ChrW(10) & _
                                    "GetNestedType[0:Type,1:Target]" & ChrW(13) & ChrW(10) & _
                                    "GetProperty[0:Type,1:Target]" & ChrW(13) & ChrW(10) & _
                                    "GetType[0:TargetType]" & ChrW(13) & ChrW(10) & _
                                    "InvokeMember[0:Type,1:Target]" & ChrW(13) & ChrW(10) & _
                                    "ReflectionOnlyGetType[0:TargetType]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                                    "System.Delegate" & ChrW(13) & ChrW(10) & _
                                    "CreateDelegate[1:Type,2:Target]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                                    "System.Reflection.Assembly" & ChrW(13) & ChrW(10) & _
                                    "GetType[1:TargetType]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                                    "System.Reflection.Module" & ChrW(13) & ChrW(10) & _
                                    "GetType[1:TargetType]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                                    "System.Activator" & ChrW(13) & ChrW(10) & _
                                    "CreateInstance[1:TargetType]" & ChrW(13) & ChrW(10) & _
                                    "CreateInstanceFrom[1:TargetType]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                                    "System.AppDomain" & ChrW(13) & ChrW(10) & _
                                    "CreateInstance[2:TargetType]" & ChrW(13) & ChrW(10) & _
                                    "CreateInstanceFrom[2:TargetType]" & ChrW(13) & ChrW(10) & _
                                    "CreateInstanceAndUnwrap[2:TargetType]" & ChrW(13) & ChrW(10) & _
                                    "CreateInstanceFromAndUnwrap[2:TargetType]" & ChrW(13) & ChrW(10) & _
                                    "CreateComInstanceFrom[2:TargetType]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                                    "System.Windows.DependencyProperty" & ChrW(13) & ChrW(10) & _
                                    "Register[0:Target,2:Type]" & ChrW(13) & ChrW(10) & _
                                    "RegisterAttached[0:Target,2:Type]" & ChrW(13) & ChrW(10) & _
                                    "RegisterAttachedReadOnly[0:Target,2:Type]" & ChrW(13) & ChrW(10) & _
                                    "RegisterReadOnly[0:Target,2:Type]" & ChrW(13) & ChrW(10) & "="

        Private Const exclude = "System.ServiceModel.ServiceContractAttribute" & ChrW(13) & ChrW(10) & _
                                          "System.ServiceModel.OperationContractAttribute" & ChrW(13) & ChrW(10) & _
                                          "System.Data.Services.Common.DataServiceKeyAttribute" & ChrW(13) & ChrW(10) & _
                                          "System.Data.Services.Common.EntitySetAttribute" & ChrW(13) & ChrW(10) & _
                                          "Microsoft.SqlServer.Server.SqlFacetAttribute" & ChrW(13) & ChrW(10) & _
                                          "Microsoft.SqlServer.Server.SqlFunctionAttribute" & ChrW(13) & ChrW(10) & _
                                          "Microsoft.SqlServer.Server.SqlMethodAttribute" & ChrW(13) & ChrW(10) & _
                                          "Microsoft.SqlServer.Server.SqlProcedureAttribute" & ChrW(13) & ChrW(10) & _
                                          "Microsoft.SqlServer.Server.SqlTriggerAttribute" & ChrW(13) & ChrW(10) & _
                                          "Microsoft.SqlServer.Server.SqlUserDefinedAggregateAttribute" & ChrW(13) & ChrW(10) & _
                                          "Microsoft.SqlServer.Server.SqlUserDefinedTypeAttribute" & ChrW(13) & ChrW(10) & "="
#End Region
       
#Region " Fields "
        Public Shared ReadOnly ExcludeAttributes As List(Of String)
        Public Shared ReadOnly Reflections As Dictionary(Of String, ReflectionMethod) = New Dictionary(Of String, ReflectionMethod)
#End Region
      
#Region " Constructor "
        Shared Sub New()
            Dim str As String = Nothing
            Using reader = New StringReader("Microsoft.VisualBasic.CompilerServices.LateBinding" & ChrW(13) & ChrW(10) & _
                                                            "LateCall[0:This,1:Type,2:Target]" & ChrW(13) & ChrW(10) & _
                                                            "LateGet[0:This,1:Type,2:Target]" & ChrW(13) & ChrW(10) & _
                                                            "LateSet[0:This,1:Type,2:Target]" & ChrW(13) & ChrW(10) & _
                                                            "LateSetComplex[0:This,1:Type,2:Target]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                                                            "Microsoft.VisualBasic.CompilerServices.NewLateBinding" & ChrW(13) & ChrW(10) & _
                                                            "LateCall[0:This,1:Type,2:Target]" & ChrW(13) & ChrW(10) & _
                                                            "LateCanEvaluate[0:This,1:Type,2:Target]" & ChrW(13) & ChrW(10) & _
                                                            "LateGet[0:This,1:Type,2:Target]" & ChrW(13) & ChrW(10) & _
                                                            "LateSet[0:This,1:Type,2:Target]" & ChrW(13) & ChrW(10) & _
                                                            "LateSetComplex[0:This,1:Type,2:Target]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                                                            "System.Type" & ChrW(13) & ChrW(10) & _
                                                            "GetEvent[0:Type,1:Target]" & ChrW(13) & ChrW(10) & _
                                                            "GetField[0:Type,1:Target]" & ChrW(13) & ChrW(10) & _
                                                            "GetMember[0:Type,1:Target]" & ChrW(13) & ChrW(10) & _
                                                            "GetMethod[0:Type,1:Target]" & ChrW(13) & ChrW(10) & _
                                                            "GetNestedType[0:Type,1:Target]" & ChrW(13) & ChrW(10) & _
                                                            "GetProperty[0:Type,1:Target]" & ChrW(13) & ChrW(10) & _
                                                            "GetType[0:TargetType]" & ChrW(13) & ChrW(10) & _
                                                            "InvokeMember[0:Type,1:Target]" & ChrW(13) & ChrW(10) & _
                                                            "ReflectionOnlyGetType[0:TargetType]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                                                            "System.Delegate" & ChrW(13) & ChrW(10) & _
                                                            "CreateDelegate[1:Type,2:Target]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                                                            "System.Reflection.Assembly" & ChrW(13) & ChrW(10) & _
                                                            "GetType[1:TargetType]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                                                            "System.Reflection.Module" & ChrW(13) & ChrW(10) & _
                                                            "GetType[1:TargetType]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                                                            "System.Activator" & ChrW(13) & ChrW(10) & _
                                                            "CreateInstance[1:TargetType]" & ChrW(13) & ChrW(10) & _
                                                            "CreateInstanceFrom[1:TargetType]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                                                            "System.AppDomain" & ChrW(13) & ChrW(10) & _
                                                            "CreateInstance[2:TargetType]" & ChrW(13) & ChrW(10) & _
                                                            "CreateInstanceFrom[2:TargetType]" & ChrW(13) & ChrW(10) & _
                                                            "CreateInstanceAndUnwrap[2:TargetType]" & ChrW(13) & ChrW(10) & _
                                                            "CreateInstanceFromAndUnwrap[2:TargetType]" & ChrW(13) & ChrW(10) & _
                                                            "CreateComInstanceFrom[2:TargetType]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                                                            "System.Windows.DependencyProperty" & ChrW(13) & ChrW(10) & _
                                                            "Register[0:Target,2:Type]" & ChrW(13) & ChrW(10) & _
                                                            "RegisterAttached[0:Target,2:Type]" & ChrW(13) & ChrW(10) & _
                                                            "RegisterAttachedReadOnly[0:Target,2:Type]" & ChrW(13) & ChrW(10) & _
                                                            "RegisterReadOnly[0:Target,2:Type]" & ChrW(13) & ChrW(10) & "=")
                Dim str2 As String
Label_0017:
                str2 = reader.ReadLine
                If (str2 <> "=") Then
                    If (str Is Nothing) Then
                        str = str2
                    ElseIf (str2 = "") Then
                        str = Nothing
                    Else
                        Dim method As New ReflectionMethod With { _
                            .typeName = str, _
                            .mtdName = str2.Substring(0, str2.IndexOf("["c)) _
                        }
                        Dim strArray As String() = str2.Substring((str2.IndexOf("["c) + 1), ((str2.IndexOf("]"c) - str2.IndexOf("["c)) - 1)).Split(New Char() {","c})
                        method.paramLoc = New Integer(strArray.Length - 1) {}
                        method.paramType = New String(strArray.Length - 1) {}
                        Dim i As Integer
                        For i = 0 To strArray.Length - 1
                            method.paramLoc(i) = Integer.Parse(strArray(i).Split(New Char() {":"c})(0))
                            method.paramType(i) = strArray(i).Split(New Char() {":"c})(1)
                        Next i
                        ExclusionReflection.Reflections.Add((method.typeName & "::" & method.mtdName), method)
                    End If
                    GoTo Label_0017
                End If
            End Using
            ExclusionReflection.ExcludeAttributes = New List(Of String)
            Using reader2 = New StringReader("System.ServiceModel.ServiceContractAttribute" & ChrW(13) & ChrW(10) & _
                                                             "System.ServiceModel.OperationContractAttribute" & ChrW(13) & ChrW(10) & _
                                                             "System.Data.Services.Common.DataServiceKeyAttribute" & ChrW(13) & ChrW(10) & _
                                                             "System.Data.Services.Common.EntitySetAttribute" & ChrW(13) & ChrW(10) & _
                                                             "Microsoft.SqlServer.Server.SqlFacetAttribute" & ChrW(13) & ChrW(10) & _
                                                             "Microsoft.SqlServer.Server.SqlFunctionAttribute" & ChrW(13) & ChrW(10) & _
                                                             "Microsoft.SqlServer.Server.SqlMethodAttribute" & ChrW(13) & ChrW(10) & _
                                                             "Microsoft.SqlServer.Server.SqlProcedureAttribute" & ChrW(13) & ChrW(10) & _
                                                             "Microsoft.SqlServer.Server.SqlTriggerAttribute" & ChrW(13) & ChrW(10) & _
                                                             "Microsoft.SqlServer.Server.SqlUserDefinedAggregateAttribute" & ChrW(13) & ChrW(10) & _
                                                             "Microsoft.SqlServer.Server.SqlUserDefinedTypeAttribute" & ChrW(13) & ChrW(10) & "=")
                Dim str4 As String
Label_017E:
                str4 = reader2.ReadLine
                If Not (str4 = "=") Then
                    ExclusionReflection.ExcludeAttributes.Add(str4)
                    GoTo Label_017E
                End If
            End Using
        End Sub
#End Region
      
    End Class

End Namespace

