Imports System.Reflection
Imports Core20Reader
Imports System.Runtime.InteropServices
Imports Helper.RandomizeHelper
Imports Mono.Cecil
Imports System.Runtime.CompilerServices
Imports System.Xml
Imports System.IO
Imports System.Drawing
Imports Helper.AssemblyHelper
Imports Implementer.Core.ManifestRequest
Imports Implementer.Core.Obfuscation.Exclusion

Namespace Engine.Analyze

    ''' <summary>
    ''' INFO : This is the first step of the renamer library. 
    '''        You must pass two arguments (inputFile and outputFile properties) when instantiating this class.
    '''        You can either check if the selected file if executable and DotNet by calling the isValidFile routine.
    ''' </summary>
    Public Class Analyzer

#Region " Fields "
        Private m_pe As IReader
        Private m_assemblyName As String = String.Empty
        Private m_assemblyVersion As String = String.Empty
        Private m_isWpfProgram As Boolean
#End Region

#Region " Events "
        Public Event FileValidated(sender As Object, e As ValidatedFile)
#End Region

#Region " Properties "
        Public Property inputFile As String
        Public Property outputFile As String
        Public Property currentFile As String
#End Region

#Region " Constructor "
        ''' <summary>
        ''' INFO : Initilize a new instance of the class Analyzer.Cls_Analyzer which used to check if the selected inputfile is a valid PE and executable file. 
        ''' </summary>
        ''' <param name="inputFilePath"></param>
        ''' <param name="outputFilePath"></param>
        Public Sub New(inputFilePath$, outPutFilePath$)
            _inputFile = inputFilePath
            _outputFile = outPutFilePath
            m_pe = New Reader()
            m_pe.ReadFile(_inputFile)
        End Sub
#End Region

#Region " Methods "
        ''' <summary>
        ''' INFO : Check if inputfile is valid DotNet and executable and not Wpf !
        ''' </summary>
        Public Function isValidFile() As Boolean
            If m_pe.isExecutable Then
                If m_pe.isManagedFile Then
                    Dim infos = Loader.Minimal(_inputFile)
                    If infos.Result = Data.Message.Success Then
                        If infos.EntryPoint IsNot Nothing Then
                            m_assemblyName = infos.AssName
                            m_assemblyVersion = infos.AssVersion
                            m_isWpfProgram = infos.IsWpf
                            If m_isWpfProgram = False Then
                                RaiseEvent FileValidated(Me, New ValidatedFile(True, m_pe, infos))
                                Return True
                            End If
                        End If
                    Else
                        RaiseEvent FileValidated(Me, New ValidatedFile(False, m_pe, Nothing))
                        Return False
                    End If
                End If
            End If
            RaiseEvent FileValidated(Me, New ValidatedFile(False, m_pe, Nothing))
            Return False
        End Function

        Public Function getAssemblyName() As String
            Return m_assemblyName
        End Function

        Public Function getAssemblyVersion() As String
            Return m_assemblyVersion
        End Function

        Public Function getModuleKind() As String
            Return m_pe.GetSystemType
        End Function

        Public Function getRuntime() As String
            Return m_pe.GetTargetRuntime
        End Function

        Public Function getProcessArchitecture() As String
            Return m_pe.GetTargetPlatform
        End Function

        Public Function getExecutionLevel() As String
            Try
                Return ManifestReader.ExtractManifest(_inputFile)
            Catch ex As Exception
                Return "asInvoker"
            End Try
            Return "asInvoker"
        End Function

        Public Function getMainIcon() As Bitmap
            Return If(m_pe.GetMainIcon Is Nothing, System.Drawing.Icon.ExtractAssociatedIcon(_inputFile).ToBitmap, m_pe.GetMainIcon.ToBitmap)
        End Function

        ''' <summary>
        ''' INFO : Check if inputfile is WPF application. 
        ''' </summary>
        ''' <remarks>
        '''        DotNet Renamer didn't take charge WPF program !
        ''' </remarks>
        Private Function isWpfProgram() As Boolean
            Return m_isWpfProgram
            Return False
        End Function

        Public Function getTreeViewHandler() As ExclusionTreeview
            Return New ExclusionTreeview(_inputFile)
        End Function

#End Region
    End Class
End Namespace
