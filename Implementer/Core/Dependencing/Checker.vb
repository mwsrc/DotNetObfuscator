Imports System.IO
Imports System.Windows.Forms
Imports Helper.UtilsHelper
Imports Implementer.engine.Checking
Imports Core20Reader

Namespace Core.Dependencing
    Public NotInheritable Class Checker

#Region " Events "
        Public Event CheckerResult As Check
#End Region

#Region " Fields "
        Private m_Lbx As ListBox
#End Region

#Region " Constructor "
        Public Sub New(ByVal lbx As ListBox)
            m_Lbx = lbx
        End Sub
#End Region

#Region " Methods "
        Private Sub RaiseCheckerResultEvent(message As String, title As String, addedFiles As String)
            Dim itemEvent As New CheckEventArgs(message, title, addedFiles)
            RaiseEvent CheckerResult(Nothing, itemEvent)
            itemEvent = Nothing
        End Sub

        Public Sub AddReferences(ByVal filesToAdd As String())
            Try
                For Each f In filesToAdd
                    If New FileInfo(f).Length = 0 Then
                        RaiseCheckerResultEvent("The file : " & New FileInfo(f).Name & " is empty !", "Bad size", "")
                    Else
                        If ReferencesExists(New FileInfo(f).Name) Then
                            Exit Sub
                        End If
                        If m_Lbx.Items.Contains(f) Then
                            Exit Sub
                        End If

                        If Not Functions.isValid(File.ReadAllBytes(f)) Then
                            RaiseCheckerResultEvent("The file : " & New FileInfo(f).Name & " isn't a Dynamic-Link Library !", "Bad file", "")
                        Else
                            Dim pe As New Reader
                            pe.ReadFile(f)

                            If pe.isExecutable Then
                                RaiseCheckerResultEvent("The file : " & New FileInfo(f).Name & " is a DotNet executable file !" & vbNewLine & "You had to choose a real Dynamic-Link Library DotNet file !", "Bad DotNet file", "")
                            Else
                                If pe.isManagedFile Then
                                    Try
                                        Dim AssemblyName As Reflection.AssemblyName = Reflection.AssemblyName.GetAssemblyName(f)
                                        RaiseCheckerResultEvent("File Added", "Operation Completed", f)
                                    Catch ex As System.IO.FileNotFoundException
                                        RaiseCheckerResultEvent("The file : " & New FileInfo(f).Name & " doesn't exist !", "Inexistant file", "")
                                    Catch ex As System.BadImageFormatException
                                        RaiseCheckerResultEvent("The file : " & New FileInfo(f).Name & " isn't a DotNet assembly or was probably modified by an obfuscator !", "Bad file", "")
                                    Catch ex As System.IO.FileLoadException
                                        RaiseCheckerResultEvent("The file : " & New FileInfo(f).Name & " seems to be loaded somewhere else !", "Open file", "")
                                    End Try
                                End If

                            End If
                        End If
                    End If
                Next
            Catch ex As Exception
                RaiseCheckerResultEvent(ex.Message, "Error", "")
            End Try
        End Sub

        Private Function ReferencesExists(ByVal fileName As String) As Boolean
            Return m_Lbx.Items.Cast(Of String).ToList().Any(Function(x) New FileInfo(x.ToString).Name.ToLower = fileName.ToLower)
        End Function
#End Region

    End Class
End Namespace

