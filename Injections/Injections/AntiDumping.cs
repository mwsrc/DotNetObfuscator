using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using System.Security.Cryptography;

namespace Injections
{
    public class AntiDumping
    {
        [DllImportAttribute("kernel32.dll")]
        static unsafe extern bool VirtualProtect(byte* lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);

        public static unsafe void Initialize()
        {
            uint old;
            byte* bas = (byte*)Marshal.GetHINSTANCE(typeof(AntiDumping).Module);
            byte* ptr = bas + 0x3c;
            byte* ptr2;
            ptr = ptr2 = bas + *(uint*)ptr;
            ptr += 0x6;
            ushort sectNum = *(ushort*)ptr;
            ptr += 14;
            ushort optSize = *(ushort*)ptr;
            ptr = ptr2 = ptr + 0x4 + optSize;

            byte* @new = stackalloc byte[11];// (byte*)Marshal.AllocHGlobal(11);
            if (typeof(AntiDumping).Module.FullyQualifiedName[0] != '<')   //Mapped
            {
                //VirtualProtect(ptr - 16, 8, 0x40, out old);
                //*(uint*)(ptr - 12) = 0;
                byte* mdDir = bas + *(uint*)(ptr - 16);
                //*(uint*)(ptr - 16) = 0;

                if (*(uint*)(ptr - 0x78) != 0)
                {
                    byte* importDir = bas + *(uint*)(ptr - 0x78);
                    byte* oftMod = bas + *(uint*)importDir;
                    byte* modName = bas + *(uint*)(importDir + 12);
                    byte* funcName = bas + *(uint*)oftMod + 2;
                    VirtualProtect(modName, 11, 0x40, out old);

                    *(uint*)@new = 0x6c64746e;
                    *((uint*)@new + 1) = 0x6c642e6c;
                    *((ushort*)@new + 4) = 0x006c;
                    *(@new + 10) = 0;

                    for (int i = 0; i < 11; i++)
                        *(modName + i) = *(@new + i);

                    VirtualProtect(funcName, 11, 0x40, out old);

                    *(uint*)@new = 0x6f43744e;
                    *((uint*)@new + 1) = 0x6e69746e;
                    *((ushort*)@new + 4) = 0x6575;
                    *(@new + 10) = 0;

                    for (int i = 0; i < 11; i++)
                        *(funcName + i) = *(@new + i);
                }

                for (int i = 0; i < sectNum; i++)
                {
                    VirtualProtect(ptr, 8, 0x40, out old);
                    Marshal.Copy(new byte[8], 0, (IntPtr)ptr, 8);
                    ptr += 0x28;
                }
                VirtualProtect(mdDir, 0x48, 0x40, out old);
                byte* mdHdr = bas + *(uint*)(mdDir + 8);
                *(uint*)mdDir = 0;
                *((uint*)mdDir + 1) = 0;
                *((uint*)mdDir + 2) = 0;
                *((uint*)mdDir + 3) = 0;

                VirtualProtect(mdHdr, 4, 0x40, out old);
                *(uint*)mdHdr = 0;
                mdHdr += 12;
                mdHdr += *(uint*)mdHdr;
                mdHdr = (byte*)(((uint)mdHdr + 7) & ~3);
                mdHdr += 2;
                ushort numOfStream = *mdHdr;
                mdHdr += 2;
                for (int i = 0; i < numOfStream; i++)
                {
                    VirtualProtect(mdHdr, 8, 0x40, out old);
                    //*(uint*)mdHdr = 0;
                    mdHdr += 4;
                    //*(uint*)mdHdr = 0;
                    mdHdr += 4;
                    for (int ii = 0; ii < 8; ii++)
                    {
                        VirtualProtect(mdHdr, 4, 0x40, out old);
                        *mdHdr = 0; mdHdr++;
                        if (*mdHdr == 0)
                        {
                            mdHdr += 3;
                            break;
                        }
                        *mdHdr = 0; mdHdr++;
                        if (*mdHdr == 0)
                        {
                            mdHdr += 2;
                            break;
                        }
                        *mdHdr = 0; mdHdr++;
                        if (*mdHdr == 0)
                        {
                            mdHdr += 1;
                            break;
                        }
                        *mdHdr = 0; mdHdr++;
                    }
                }
            }
            else   //Flat
            {
                //VirtualProtect(ptr - 16, 8, 0x40, out old);
                //*(uint*)(ptr - 12) = 0;
                uint mdDir = *(uint*)(ptr - 16);
                //*(uint*)(ptr - 16) = 0;
                uint importDir = *(uint*)(ptr - 0x78);

                uint[] vAdrs = new uint[sectNum];
                uint[] vSizes = new uint[sectNum];
                uint[] rAdrs = new uint[sectNum];
                for (int i = 0; i < sectNum; i++)
                {
                    VirtualProtect(ptr, 8, 0x40, out old);
                    Marshal.Copy(new byte[8], 0, (IntPtr)ptr, 8);
                    vAdrs[i] = *(uint*)(ptr + 12);
                    vSizes[i] = *(uint*)(ptr + 8);
                    rAdrs[i] = *(uint*)(ptr + 20);
                    ptr += 0x28;
                }


                if (importDir != 0)
                {
                    for (int i = 0; i < sectNum; i++)
                        if (vAdrs[i] < importDir && importDir < vAdrs[i] + vSizes[i])
                        {
                            importDir = importDir - vAdrs[i] + rAdrs[i];
                            break;
                        }
                    byte* importDirPtr = bas + importDir;
                    uint oftMod = *(uint*)importDirPtr;
                    for (int i = 0; i < sectNum; i++)
                        if (vAdrs[i] < oftMod && oftMod < vAdrs[i] + vSizes[i])
                        {
                            oftMod = oftMod - vAdrs[i] + rAdrs[i];
                            break;
                        }
                    byte* oftModPtr = bas + oftMod;
                    uint modName = *(uint*)(importDirPtr + 12);
                    for (int i = 0; i < sectNum; i++)
                        if (vAdrs[i] < modName && modName < vAdrs[i] + vSizes[i])
                        {
                            modName = modName - vAdrs[i] + rAdrs[i];
                            break;
                        }
                    uint funcName = *(uint*)oftModPtr + 2;
                    for (int i = 0; i < sectNum; i++)
                        if (vAdrs[i] < funcName && funcName < vAdrs[i] + vSizes[i])
                        {
                            funcName = funcName - vAdrs[i] + rAdrs[i];
                            break;
                        }
                    VirtualProtect(bas + modName, 11, 0x40, out old);

                    *(uint*)@new = 0x6c64746e;
                    *((uint*)@new + 1) = 0x6c642e6c;
                    *((ushort*)@new + 4) = 0x006c;
                    *(@new + 10) = 0;

                    for (int i = 0; i < 11; i++)
                        *(bas + modName + i) = *(@new + i);

                    VirtualProtect(bas + funcName, 11, 0x40, out old);

                    *(uint*)@new = 0x6f43744e;
                    *((uint*)@new + 1) = 0x6e69746e;
                    *((ushort*)@new + 4) = 0x6575;
                    *(@new + 10) = 0;

                    for (int i = 0; i < 11; i++)
                        *(bas + funcName + i) = *(@new + i);
                }


                for (int i = 0; i < sectNum; i++)
                    if (vAdrs[i] < mdDir && mdDir < vAdrs[i] + vSizes[i])
                    {
                        mdDir = mdDir - vAdrs[i] + rAdrs[i];
                        break;
                    }
                byte* mdDirPtr = bas + mdDir;
                VirtualProtect(mdDirPtr, 0x48, 0x40, out old);
                uint mdHdr = *(uint*)(mdDirPtr + 8);
                for (int i = 0; i < sectNum; i++)
                    if (vAdrs[i] < mdHdr && mdHdr < vAdrs[i] + vSizes[i])
                    {
                        mdHdr = mdHdr - vAdrs[i] + rAdrs[i];
                        break;
                    }
                *(uint*)mdDirPtr = 0;
                *((uint*)mdDirPtr + 1) = 0;
                *((uint*)mdDirPtr + 2) = 0;
                *((uint*)mdDirPtr + 3) = 0;


                byte* mdHdrPtr = bas + mdHdr;
                VirtualProtect(mdHdrPtr, 4, 0x40, out old);
                *(uint*)mdHdrPtr = 0;
                mdHdrPtr += 12;
                mdHdrPtr += *(uint*)mdHdrPtr;
                mdHdrPtr = (byte*)(((uint)mdHdrPtr + 7) & ~3);
                mdHdrPtr += 2;
                ushort numOfStream = *mdHdrPtr;
                mdHdrPtr += 2;
                for (int i = 0; i < numOfStream; i++)
                {
                    VirtualProtect(mdHdrPtr, 8, 0x40, out old);
                    //*(uint*)mdHdrPtr = 0;
                    mdHdrPtr += 4;
                    //*(uint*)mdHdrPtr = 0;
                    mdHdrPtr += 4;
                    for (int ii = 0; ii < 8; ii++)
                    {
                        VirtualProtect(mdHdrPtr, 4, 0x40, out old);
                        *mdHdrPtr = 0; mdHdrPtr++;
                        if (*mdHdrPtr == 0)
                        {
                            mdHdrPtr += 3;
                            break;
                        }
                        *mdHdrPtr = 0; mdHdrPtr++;
                        if (*mdHdrPtr == 0)
                        {
                            mdHdrPtr += 2;
                            break;
                        }
                        *mdHdrPtr = 0; mdHdrPtr++;
                        if (*mdHdrPtr == 0)
                        {
                            mdHdrPtr += 1;
                            break;
                        }
                        *mdHdrPtr = 0; mdHdrPtr++;
                    }
                }
            }
            //Marshal.FreeHGlobal((IntPtr)@new);
        }
    }



    //static class Proxies
    //{
    //    private static void CtorProxy(RuntimeFieldHandle f)
    //    {
    //        FieldInfo fld = FieldInfo.GetFieldFromHandle(f);
    //        var m = fld.Module;
    //        byte[] dat = m.ResolveSignature(fld.MetadataToken);

    //        uint x =
    //            ((uint)dat[dat.Length - 6] << 0) |
    //            ((uint)dat[dat.Length - 5] << 8) |
    //            ((uint)dat[dat.Length - 3] << 16) |
    //            ((uint)dat[dat.Length - 2] << 24);

    //        ConstructorInfo mtd = m.ResolveMethod(Mutation.Placeholder((int)x) | ((int)dat[dat.Length - 7] << 24)) as ConstructorInfo;

    //        var args = mtd.GetParameters();
    //        Type[] arg = new Type[args.Length];
    //        for (int i = 0; i < args.Length; i++)
    //            arg[i] = args[i].ParameterType;

    //        DynamicMethod dm;
    //        if (mtd.DeclaringType.IsInterface || mtd.DeclaringType.IsArray)
    //            dm = new DynamicMethod("", mtd.DeclaringType, arg, fld.DeclaringType, true);
    //        else
    //            dm = new DynamicMethod("", mtd.DeclaringType, arg, mtd.DeclaringType, true);
    //        Console.WriteLine(mtd.DeclaringType);
    //        Console.WriteLine(mtd.Name);
    //        var info = dm.GetDynamicILInfo();
    //        info.SetLocalSignature(new byte[] { 0x7, 0x0 });
    //        byte[] y = new byte[2 * arg.Length + 6 + 5];
    //        for (int i = 0; i < arg.Length; i++)
    //        {
    //            y[i * 2] = 0x0e;
    //            y[i * 2 + 1] = (byte)i;
    //        }
    //        y[arg.Length * 2] = 0x73;
    //        Buffer.BlockCopy(BitConverter.GetBytes(info.GetTokenFor(mtd.MethodHandle)), 0, y, arg.Length * 2 + 1, 4);
    //        y[arg.Length * 2 + 5] = 0x74;
    //        Buffer.BlockCopy(BitConverter.GetBytes(info.GetTokenFor(mtd.DeclaringType.TypeHandle)), 0, y, arg.Length * 2 + 6, 4);
    //        y[y.Length - 1] = 0x2a;
    //        info.SetCode(y, arg.Length + 1);
    //        //Mutation.Break();
    //        fld.SetValue(null, dm.CreateDelegate(fld.FieldType));
    //    }
    //    private static void MtdProxy(RuntimeFieldHandle f)
    //    {
    //        var fld = FieldInfo.GetFieldFromHandle(f);
    //        var m = fld.Module;
    //        byte[] dat = m.ResolveSignature(fld.MetadataToken);

    //        uint x =
    //            ((uint)dat[dat.Length - 6] << 0) |
    //            ((uint)dat[dat.Length - 5] << 8) |
    //            ((uint)dat[dat.Length - 3] << 16) |
    //            ((uint)dat[dat.Length - 2] << 24);

    //        var mtd = m.ResolveMethod(Mutation.Placeholder((int)x) | ((int)dat[dat.Length - 7] << 24)) as MethodInfo;

    //        if (mtd.IsStatic)
    //            fld.SetValue(null, Delegate.CreateDelegate(fld.FieldType, mtd));
    //        else
    //        {
    //            string n = fld.Name;

    //            var tmp = mtd.GetParameters();
    //            Type[] arg = new Type[tmp.Length + 1];
    //            arg[0] = typeof(object);
    //            for (int i = 0; i < tmp.Length; i++)
    //                arg[i + 1] = tmp[i].ParameterType;

    //            DynamicMethod dm;
    //            var decl = mtd.DeclaringType;
    //            var decl2 = fld.DeclaringType;
    //            if (decl.IsInterface || decl.IsArray)
    //                dm = new DynamicMethod("", mtd.ReturnType, arg, decl2, true);
    //            else
    //                dm = new DynamicMethod("", mtd.ReturnType, arg, decl, true);

    //            var info = dm.GetDynamicILInfo();
    //            info.SetLocalSignature(new byte[] { 0x7, 0x0 });
    //            byte[] y = new byte[2 * arg.Length + 11];
    //            int idx = 0;
    //            for (int i = 0; i < arg.Length; i++)
    //            {
    //                y[idx++] = 0x0e;
    //                y[idx++] = (byte)i;
    //                if (i == 0)
    //                {
    //                    y[idx++] = 0x74;
    //                    Buffer.BlockCopy(BitConverter.GetBytes(info.GetTokenFor(decl.TypeHandle)), 0, y, idx, 4);
    //                    idx += 4;
    //                }
    //            }
    //            y[idx++] = (byte)((n[0] == Mutation.Key0I) ? 0x6f : 0x28);
    //            Buffer.BlockCopy(BitConverter.GetBytes(info.GetTokenFor(mtd.MethodHandle)), 0, y, idx, 4);
    //            idx += 4;
    //            y[idx] = 0x2a;
    //            info.SetCode(y, arg.Length + 1);

    //            fld.SetValue(null, dm.CreateDelegate(fld.FieldType));
    //        }
    //    }
    //}





    //static class AntiTamperMem
    //{
    //    [DllImportAttribute("kernel32.dll")]
    //    static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

    //    public static unsafe void Initalize()
    //    {
    //        Module mod = typeof(AntiTamperMem).Module;
    //        IntPtr modPtr = Marshal.GetHINSTANCE(mod);
    //        if (modPtr == (IntPtr)(-1)) Environment.FailFast("Module error");
    //        bool mapped = mod.FullyQualifiedName[0] != '<'; //<Unknown>
    //        Stream stream;
    //        stream = new UnmanagedMemoryStream((byte*)modPtr.ToPointer(), 0xfffffff, 0xfffffff, FileAccess.ReadWrite);

    //        byte[] buff;
    //        int checkSumOffset;
    //        ulong checkSum;
    //        byte[] iv;
    //        byte[] dats;
    //        int sn;
    //        int snLen;
    //        using (BinaryReader rdr = new BinaryReader(stream))
    //        {
    //            stream.Seek(0x3c, SeekOrigin.Begin);
    //            uint offset = rdr.ReadUInt32();
    //            stream.Seek(offset, SeekOrigin.Begin);
    //            stream.Seek(0x6, SeekOrigin.Current);
    //            uint sections = rdr.ReadUInt16();
    //            stream.Seek(0xC, SeekOrigin.Current);
    //            uint optSize = rdr.ReadUInt16();
    //            stream.Seek(offset = offset + 0x18, SeekOrigin.Begin);  //Optional hdr
    //            bool pe32 = (rdr.ReadUInt16() == 0x010b);
    //            stream.Seek(0x3e, SeekOrigin.Current);
    //            checkSumOffset = (int)stream.Position;
    //            uint md = rdr.ReadUInt32() ^ (uint)Mutation.Key0I;
    //            if (md == (uint)Mutation.Key0I)
    //                Environment.FailFast("Broken file");

    //            stream.Seek(offset = offset + optSize, SeekOrigin.Begin);  //sect hdr
    //            uint datLoc = 0;
    //            for (int i = 0; i < sections; i++)
    //            {
    //                int h = 0;
    //                for (int j = 0; j < 8; j++)
    //                {
    //                    byte chr = rdr.ReadByte();
    //                    if (chr != 0) h += chr;
    //                }
    //                uint vSize = rdr.ReadUInt32();
    //                uint vLoc = rdr.ReadUInt32();
    //                uint rSize = rdr.ReadUInt32();
    //                uint rLoc = rdr.ReadUInt32();
    //                if (h == Mutation.Key1I)
    //                    datLoc = mapped ? vLoc : rLoc;
    //                if (!mapped && md > vLoc && md < vLoc + vSize)
    //                    md = md - vLoc + rLoc;
    //                stream.Seek(0x10, SeekOrigin.Current);
    //            }

    //            stream.Seek(md, SeekOrigin.Begin);
    //            using (MemoryStream str = new MemoryStream())
    //            {
    //                stream.Position += 12;
    //                stream.Position += rdr.ReadUInt32() + 4;
    //                stream.Position += 2;

    //                ushort streams = rdr.ReadUInt16();

    //                for (int i = 0; i < streams; i++)
    //                {
    //                    uint pos = rdr.ReadUInt32() + md;
    //                    uint size = rdr.ReadUInt32();

    //                    int c = 0;
    //                    while (rdr.ReadByte() != 0) c++;
    //                    long ori = stream.Position += (((c + 1) + 3) & ~3) - (c + 1);

    //                    stream.Position = pos;
    //                    str.Write(rdr.ReadBytes((int)size), 0, (int)size);
    //                    stream.Position = ori;
    //                }

    //                buff = str.ToArray();
    //            }

    //            stream.Seek(datLoc, SeekOrigin.Begin);
    //            checkSum = rdr.ReadUInt64() ^ (ulong)Mutation.Key0L;
    //            sn = rdr.ReadInt32();
    //            snLen = rdr.ReadInt32();
    //            iv = rdr.ReadBytes(rdr.ReadInt32() ^ Mutation.Key2I);
    //            dats = rdr.ReadBytes(rdr.ReadInt32() ^ Mutation.Key3I);
    //        }

    //        byte[] md5 = MD5.Create().ComputeHash(buff);
    //        ulong tCs = BitConverter.ToUInt64(md5, 0) ^ BitConverter.ToUInt64(md5, 8);
    //        if (tCs != checkSum)
    //            Environment.FailFast("Broken file");

    //        byte[] b = Decrypt(buff, iv, dats);
    //        Buffer.BlockCopy(new byte[buff.Length], 0, buff, 0, buff.Length);
    //        if (b[0] != 0xd6 || b[1] != 0x6f)
    //            Environment.FailFast("Broken file");
    //        byte[] tB = new byte[b.Length - 2];
    //        Buffer.BlockCopy(b, 2, tB, 0, tB.Length);
    //        using (BinaryReader rdr = new BinaryReader(new MemoryStream(tB)))
    //        {
    //            uint len = rdr.ReadUInt32();
    //            int[] codeLens = new int[len];
    //            IntPtr[] ptrs = new IntPtr[len];
    //            for (int i = 0; i < len; i++)
    //            {
    //                uint pos = rdr.ReadUInt32() ^ (uint)Mutation.Key4I;
    //                if (pos == 0) continue;
    //                uint rva = rdr.ReadUInt32() ^ (uint)Mutation.Key5I;
    //                byte[] cDat = rdr.ReadBytes(rdr.ReadInt32());
    //                uint old;
    //                IntPtr ptr = (IntPtr)((uint)modPtr + (mapped ? rva : pos));
    //                VirtualProtect(ptr, (uint)cDat.Length, 0x04, out old);
    //                Marshal.Copy(cDat, 0, ptr, cDat.Length);
    //                VirtualProtect(ptr, (uint)cDat.Length, old, out old);
    //                codeLens[i] = cDat.Length;
    //                ptrs[i] = ptr;
    //            }
    //            //for (int i = 0; i < len; i++)
    //            //{
    //            //    if (codeLens[i] == 0) continue;
    //            //    RuntimeHelpers.PrepareMethod(mod.ModuleHandle.GetRuntimeMethodHandleFromMetadataToken(0x06000000 + i + 1));
    //            //}
    //            //for (int i = 0; i < len; i++)
    //            //{
    //            //    if (codeLens[i] == 0) continue;
    //            //    uint old;
    //            //    VirtualProtect(ptrs[i], (uint)codeLens[i], 0x04, out old);
    //            //    Marshal.Copy(new byte[codeLens[i]], 0, ptrs[i], codeLens[i]);
    //            //    VirtualProtect(ptrs[i], (uint)codeLens[i], old, out old);
    //            //}
    //        }
    //    }

    //    static byte[] Decrypt(byte[] buff, byte[] iv, byte[] dat)
    //    {
    //        RijndaelManaged ri = new RijndaelManaged();
    //        byte[] ret = new byte[dat.Length];
    //        MemoryStream ms = new MemoryStream(dat);
    //        using (CryptoStream cStr = new CryptoStream(ms, ri.CreateDecryptor(SHA256.Create().ComputeHash(buff), iv), CryptoStreamMode.Read))
    //        { cStr.Read(ret, 0, dat.Length); }

    //        SHA512 sha = SHA512.Create();
    //        byte[] c = sha.ComputeHash(buff);
    //        for (int i = 0; i < ret.Length; i += 64)
    //        {
    //            int len = ret.Length <= i + 64 ? ret.Length : i + 64;
    //            for (int j = i; j < len; j++)
    //                ret[j] ^= (byte)(c[j - i] ^ Mutation.Key6I);
    //            c = sha.ComputeHash(ret, i, len - i);
    //        }
    //        return ret;
    //    }
    //}





}
