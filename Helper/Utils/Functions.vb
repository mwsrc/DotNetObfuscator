Imports System.IO
Imports System.Drawing.Imaging
Imports System.Drawing
Imports System.Security.AccessControl
Imports System.Security.Principal
Imports System.IO.Compression
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography

Namespace UtilsHelper
    Public NotInheritable Class Functions

#Region " Fields "
        Private Shared tempFolder$ = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\Temp"
#End Region

#Region " Methods "
        Public Shared Sub DeleteFiles(Path$)
            Directory.GetFiles(Path, "*.*", SearchOption.AllDirectories).ToList.ForEach(AddressOf DeleteFile)
        End Sub

        Private Shared Sub DeleteFile(f$)
            Try
                File.Delete(f)
            Catch ex As Exception
            End Try
        End Sub

        Public Shared Function GetTempFolder() As String
            If Not Directory.Exists(tempFolder) Then
                Directory.CreateDirectory(tempFolder)
            End If
            Return tempFolder
        End Function

        Public Shared Function StrToHex(ByRef Data As String) As String
            Dim sVal As String
            Dim sHex As String = ""
            While Data.Length > 0
                sVal = Conversion.Hex(Strings.Asc(Data.Substring(0, 1).ToString()))
                Data = Data.Substring(1, Data.Length - 1)
                sHex = sHex & sVal
            End While
            Return sHex
        End Function

        Public Shared Function AssemblyToHex(FilePath$) As String
            Dim val As String = Nothing
            Using fr As New FileStream(FilePath, FileMode.Open, FileAccess.Read)
                Using br As New BinaryReader(fr)
                    Return BitConverter.ToString(br.ReadBytes(CInt(fr.Length))).Replace("-", "")
                End Using
            End Using
            Return val
        End Function

        Public Shared Function ByteArrayToHex(ba As Byte()) As String
            Dim hex As String = BitConverter.ToString(ba)
            Return hex.Replace("-", "")
        End Function

        Public Shared Function StreamToHex(ba As Stream) As String
            Dim hex As String = BitConverter.ToString(StreamToBytArray(ba))
            Return hex.Replace("-", "")
        End Function

        Public Shared Function StreamToBytArray(input As Stream) As Byte()
            Using ms As New MemoryStream()
                input.CopyTo(ms)
                Return ms.ToArray()
            End Using
        End Function

        Public Shared Function GZipedByte(raw As Byte()) As Byte()
            Using memory As New MemoryStream()
                Using gzip As New GZipStream(memory, CompressionMode.Compress, True)
                    gzip.Write(raw, 0, raw.Length)
                End Using
                Return memory.ToArray()
            End Using
        End Function

        Public Shared Function isBase64StringEncoded(str$) As Boolean
            Dim r As New RegularExpressions.Regex("^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$")
            Return r.IsMatch(str)
        End Function

        Public Shared Function isValid(file As Byte()) As Boolean
            If file.Take(2).SequenceEqual(New Byte() {77, 90}) Then
                Return True
            End If
            Return False
        End Function

        Public Shared Function isValid(ByVal filepath As String, ByVal Mime As ImageFormat) As Boolean
            Try
                Dim id As Guid = GetGuidID(filepath)
                If id = Mime.Guid Then
                    Return True
                End If
            Catch
                Return False
            End Try
            Return False
        End Function

        Private Shared Function GetGuidID(ByVal filepath As String) As Guid
            Dim imagedata As Byte() = File.ReadAllBytes(filepath)
            Dim id As Guid
            Using ms As New MemoryStream(imagedata)
                Using img As Image = Image.FromStream(ms)
                    id = img.RawFormat.Guid
                End Using
            End Using
            Return id
        End Function

        Public Shared Function GetMD5HashFromFile(filename As String) As String
            Using md5 = New MD5CryptoServiceProvider()
                Dim buffer = md5.ComputeHash(File.ReadAllBytes(filename))
                Dim sb = New StringBuilder()
                For i As Integer = 0 To buffer.Length - 1
                    sb.Append(buffer(i).ToString("x2"))
                Next
                Return sb.ToString()
            End Using
        End Function
#End Region

    End Class
End Namespace
