Imports Mono.Cecil
Imports Mono.Cecil.Cil
Imports Mono.Cecil.PE
Imports Mono.Cecil.MetadataProcessor
Imports Mono.Cecil.Metadata
Imports Helper.RandomizeHelper
Imports System.IO
Imports System.Text
Imports Implementer.Core.Obfuscation.Exclusion

Namespace Core.Obfuscation.Protection
    ''' <summary>
    ''' By Yck from Confuser
    ''' </summary>
    Public NotInheritable Class InvalidMetadata

#Region " Fields "
        Private Shared rand As Random
#End Region

#Region " Constructor "
        Shared Sub New()
            rand = New Random
        End Sub
#End Region

#Region " Methods "


        Public Shared Sub DoJob(asm As AssemblyDefinition, psr As MetadataProcessor)
            Try
                AddHandler psr.BeforeWriteTables, Function(accessor)

                                                      accessor.TableHeap.GetTable(Of DeclSecurityTable)(Table.DeclSecurity).AddRow(New Row(Of SecurityAction, UInt32, UInt32)(&HFFFF, UInt32.MaxValue, UInt32.MaxValue))
                                                      accessor.TableHeap.GetTable(Of TypeDefTable)(2).Item(0).Col2 = 65535

                                                      If ExcludeReflection.HasItems = False Then
                                                          If accessor.Module.Runtime <> TargetRuntime.Net_4_0 Then
                                                              Dim mtdLen As UInteger = CUInt(accessor.TableHeap.GetTable(Of MethodTable)(Table.Method).Length) + 1
                                                              Dim fldLen As UInteger = CUInt(accessor.TableHeap.GetTable(Of FieldTable)(Table.Field).Length) + 1

                                                              Dim nss As New List(Of UInteger)()
                                                              For Each k As Row(Of TypeAttributes, UInteger, UInteger, UInteger, UInteger, UInteger) In accessor.TableHeap.GetTable(Of TypeDefTable)(Table.TypeDef)
                                                                  If k Is Nothing Then
                                                                      Exit For
                                                                  ElseIf Not nss.Contains(k.Col3) Then
                                                                      nss.Add(k.Col3)
                                                                  End If
                                                              Next
                                                              Dim nested As UInteger = CUInt(accessor.TableHeap.GetTable(Of TypeDefTable)(Table.TypeDef).AddRow(New Row(Of TypeAttributes, UInteger, UInteger, UInteger, UInteger, UInteger)(0, &H7FFFFFFF, 0, &H3FFFD, fldLen, mtdLen)))
                                                              accessor.TableHeap.GetTable(Of NestedClassTable)(Table.NestedClass).AddRow(New Row(Of UInteger, UInteger)(nested, nested))
                                                              For Each l In nss
                                                                  Dim type As UInteger = CUInt(accessor.TableHeap.GetTable(Of TypeDefTable)(Table.TypeDef).AddRow(New Row(Of TypeAttributes, UInteger, UInteger, UInteger, UInteger, UInteger)(0, &H7FFFFFFF, l, &H3FFFD, fldLen, mtdLen)))
                                                                  accessor.TableHeap.GetTable(Of NestedClassTable)(Table.NestedClass).AddRow(New Row(Of UInteger, UInteger)(nested, type))
                                                              Next
                                                              For Each r As Row(Of ParameterAttributes, UShort, UInteger) In accessor.TableHeap.GetTable(Of ParamTable)(Table.Param)
                                                                  If r IsNot Nothing Then
                                                                      r.Col3 = &H7FFFFFFF
                                                                  End If
                                                              Next
                                                          End If
                                                      End If

                                                      accessor.TableHeap.GetTable(Of ModuleTable)(Table.Module).AddRow(New Row(Of UInt16, UInt32, UInt16, UInt16, UInt16)(0, &H7FFF7FFF, 0, 0, 0).Col2)
                                                      accessor.TableHeap.GetTable(Of AssemblyTable)(Table.Assembly).AddRow(New Row(Of AssemblyHashAlgorithm, UInt16, UInt16, UInt16, UInt16, AssemblyAttributes, UInt32, UInt32, UInt32)(0, 0, 0, 0, 0, 0, 0, &H7FFF7FFF, 0))

                                                      Dim num% = rand.Next(8, &H10)
                                                      Dim i%
                                                      For i = 0 To num - 1
                                                          accessor.TableHeap.GetTable(Of ENCLogTable)(Table.EncLog).AddRow(New Row(Of UInt32, UInt32)(rand.Next, rand.Next))
                                                      Next i
                                                      num = rand.Next(8, &H10)
                                                      Dim j%
                                                      For j = 0 To num - 1
                                                          accessor.TableHeap.GetTable(Of ENCMapTable)(Table.EncMap).AddRow(rand.Next)
                                                      Next j

                                                      accessor.TableHeap.GetTable(Of AssemblyRefTable)(Table.AssemblyRef).AddRow(New Row(Of UInt16, UInt16, UInt16, UInt16, AssemblyAttributes, UInt32, UInt32, UInt32, UInt32)(0, 0, 0, 0, 0, 0, &HFFFF, 0, &HFFFF))
                                                      Randomize(Of Row(Of UInt32, UInt32))(accessor.TableHeap.GetTable(Of NestedClassTable)(Table.NestedClass))

                                                      If ExcludeReflection.HasItems = False Then
                                                          Dim pad As Char() = New Char(65535) {}
                                                          Dim len As Integer = 0
                                                          While accessor.StringHeap.Length + len < &H10000
                                                              For s As Integer = 0 To 4095
                                                                  While (InlineAssignHelper(pad(len + s), CChar(ChrW(rand.Next(0, &H100))))) = ControlChars.NullChar
                                                                  End While
                                                              Next
                                                              len += &H1000
                                                          End While
                                                          Dim idx As UInteger = accessor.StringHeap.GetStringIndex(New String(pad, 0, len))
                                                          accessor.TableHeap.GetTable(Of ManifestResourceTable)(Table.ManifestResource).AddRow(New Row(Of UInteger, ManifestResourceAttributes, UInteger, UInteger)(&HFFFFFFFFUI, ManifestResourceAttributes.[Private], idx, 2))
                                                      Else
                                                          Randomize(Of Row(Of UInt32, ManifestResourceAttributes, UInt32, UInt32))(accessor.TableHeap.GetTable(Of ManifestResourceTable)(Table.ManifestResource))
                                                      End If
                                                      Randomize(Of Row(Of UInt32, UInt32))(accessor.TableHeap.GetTable(Of GenericParamConstraintTable)(Table.GenericParamConstraint))

                                                      Return accessor
                                                  End Function

                AddHandler psr.ProcessPe, Function(stream, accessor)
                                              Dim reader As New BinaryReader(stream)
                                              stream.Seek(60L, SeekOrigin.Begin)
                                              Dim num As UInteger = reader.ReadUInt32()
                                              stream.Seek(CLng(num), SeekOrigin.Begin)
                                              stream.Seek(6L, SeekOrigin.Current)
                                              Dim num2 As UInteger = reader.ReadUInt16()
                                              stream.Seek(CLng(num + &H18), SeekOrigin.Begin)
                                              Dim MagicPos = stream.Position
                                              ' 0x10b = PE32
                                              Dim is32 As Boolean = (reader.ReadUInt16 = &H10B)
                                              stream.Seek(CLng(num + If(is32, &HE0, 240)), SeekOrigin.Begin)
                                              stream.Seek(-12L, SeekOrigin.Current)
                                              For i As Integer = 0 To num2 - 1
                                                  Dim flag2 As Boolean = False
                                                  For k As Integer = 0 To 7
                                                      If (reader.ReadByte() = 0) And Not flag2 Then
                                                          flag2 = True
                                                          stream.Seek(-1L, SeekOrigin.Current)
                                                          stream.WriteByte(&H20)
                                                      End If
                                                  Next
                                                  reader.ReadUInt32()
                                                  reader.ReadUInt32()
                                                  reader.ReadUInt32()
                                                  reader.ReadUInt32()
                                                  stream.Seek(&H10L, SeekOrigin.Current)
                                              Next
                                              Dim num6 As UInteger = accessor.ResolveVirtualAddress(accessor.GetTextSegmentRange(TextSegment.MetadataHeader).Start)
                                              stream.Position = num6 + 12
                                              Dim position As Long = stream.Position
                                              Dim num8 As UInteger = reader.ReadUInt32()
                                              stream.Position += num8
                                              stream.Position += 2L
                                              Dim num9 As UShort = reader.ReadUInt16()
                                              Dim maxValue As UInteger = UInteger.MaxValue
                                              For j As Integer = 0 To num9 - 1
                                                  Dim num14 As Byte
                                                  maxValue = Math.Min(reader.ReadUInt32(), maxValue)
                                                  stream.Position += 4L
                                                  Dim num12 As Long = stream.Position
                                                  Dim num13 As Integer = 0
                                                  Dim str As String = ""
                                                  While (InlineAssignHelper(num14, reader.ReadByte())) <> 0
                                                      str = str & CChar(ChrW(num14))
                                                      num13 += 1
                                                  End While
                                                  If str = "#~" Then
                                                      stream.Position = num12 + 1L
                                                      stream.WriteByte(&H2D)
                                                  End If
                                                  stream.Position = (stream.Position + 3L) And -4L
                                              Next
                                              Dim num15 As UInteger = maxValue - (CUInt(stream.Position) - num6)
                                              Dim num16 As UInteger = CUInt(stream.Position - (position + 4L))
                                              stream.Position = position
                                              stream.Write(BitConverter.GetBytes(CUInt(num8 + num15)), 0, 4)
                                              Dim buffer As Byte() = New Byte(num8 - 1) {}
                                              stream.Read(buffer, 0, CInt(num8))
                                              Dim buffer2 As Byte() = New Byte(num16 - num8 - 1) {}
                                              stream.Read(buffer2, 0, buffer2.Length)
                                              stream.Position = position + 4L
                                              stream.Write(buffer, 0, buffer.Length)
                                              stream.Write(New Byte(num15 - 1) {}, 0, CInt(num15))
                                              stream.Write(buffer2, 0, buffer2.Length)
                                              stream.Seek(236, SeekOrigin.Begin)
                                              stream.Write(BitConverter.GetBytes(rand.Next(1000, 10000)), 0, 4)
                                              reader.ReadUInt32()
                                              stream.Write(BitConverter.GetBytes(rand.Next(11, 14)), 0, 4) 'Anti-Decompilation
                                              stream.Flush()
                                              stream.Close()
                                              reader.Close()

                                              Return accessor
                                          End Function

            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try

        End Sub

        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
            target = value
            Return value
        End Function

        Private Shared Sub Randomize(Of T)(ByVal tbl As MetadataTable(Of T))
            Dim localArray As T() = Enumerable.ToArray(Of T)(Enumerable.OfType(Of T)(tbl))
            Dim i As Integer
            For i = 0 To localArray.Length - 1
                Dim local As T = localArray(i)
                Dim index As Integer = rand.Next(0, localArray.Length)
                localArray(i) = localArray(index)
                localArray(index) = local
            Next i
            tbl.Clear()
            Dim local2 As T
            For Each local2 In localArray
                tbl.AddRow(local2)
            Next
        End Sub

#End Region

    End Class
End Namespace
