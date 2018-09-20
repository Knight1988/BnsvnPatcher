// Decompiled with JetBrains decompiler
// Type: BNSDat.BNSDat
// Assembly: BnS Buddy, Version=5.9.0.3, Culture=neutral, PublicKeyToken=null
// MVID: 9E2C87FE-8AF0-4CC8-AEAD-1B0C55517684
// Assembly location: E:\Games\GarenaBnSVN\BNS Buddy\BnS Buddy.exe

using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BNSDat
{
  public class BNSDat
  {
    public string AES_KEY = "bns_obt_kr_2014#";
    public byte[] XOR_KEY = new byte[16]
    {
      (byte) 164,
      (byte) 159,
      (byte) 216,
      (byte) 179,
      (byte) 246,
      (byte) 142,
      (byte) 57,
      (byte) 194,
      (byte) 45,
      (byte) 224,
      (byte) 97,
      (byte) 117,
      (byte) 92,
      (byte) 75,
      (byte) 26,
      (byte) 7
    };

    private byte[] Decrypt(byte[] buffer, int size)
    {
      int length = this.AES_KEY.Length;
      int inputCount = size + length;
      byte[] outputBuffer = new byte[inputCount];
      byte[] inputBuffer = new byte[inputCount];
      buffer.CopyTo((Array) inputBuffer, 0);
      buffer = (byte[]) null;
      Rijndael rijndael = Rijndael.Create();
      rijndael.Mode = CipherMode.ECB;
      rijndael.CreateDecryptor(Encoding.ASCII.GetBytes(this.AES_KEY), new byte[16]).TransformBlock(inputBuffer, 0, inputCount, outputBuffer, 0);
      byte[] numArray1 = outputBuffer;
      byte[] numArray2 = new byte[size];
      Array.Copy((Array) numArray1, 0, (Array) numArray2, 0, size);
      return numArray2;
    }

    private byte[] Deflate(byte[] buffer, int sizeCompressed, int sizeDecompressed)
    {
      byte[] numArray1 = ZlibStream.UncompressBuffer(buffer);
      if (numArray1.Length != sizeDecompressed)
      {
        byte[] numArray2 = new byte[sizeDecompressed];
        if (numArray1.Length > sizeDecompressed)
          Array.Copy((Array) numArray1, 0, (Array) numArray2, 0, sizeDecompressed);
        else
          Array.Copy((Array) numArray1, 0, (Array) numArray2, 0, numArray1.Length);
        numArray1 = numArray2;
      }
      return numArray1;
    }

    private byte[] Unpack(byte[] buffer, int sizeStored, int sizeSheared, int sizeUnpacked, bool isEncrypted, bool isCompressed)
    {
      byte[] buffer1 = buffer;
      if (isEncrypted)
        buffer1 = this.Decrypt(buffer1, sizeStored);
      if (isCompressed)
        buffer1 = this.Deflate(buffer1, sizeSheared, sizeUnpacked);
      if (buffer1 == buffer)
      {
        buffer1 = new byte[sizeUnpacked];
        if (sizeSheared < sizeUnpacked)
          Array.Copy((Array) buffer, 0, (Array) buffer1, 0, sizeSheared);
        else
          Array.Copy((Array) buffer, 0, (Array) buffer1, 0, sizeUnpacked);
      }
      return buffer1;
    }

    private byte[] Inflate(byte[] buffer, int sizeDecompressed, out int sizeCompressed, int compressionLevel)
    {
      MemoryStream memoryStream = new MemoryStream();
      ZlibStream zlibStream = new ZlibStream((Stream) memoryStream, (CompressionMode) 0, (CompressionLevel) compressionLevel, true);
      ((Stream) zlibStream).Write(buffer, 0, sizeDecompressed);
      ((Stream) zlibStream).Flush();
      ((Stream) zlibStream).Close();
      sizeCompressed = (int) memoryStream.Length;
      return memoryStream.ToArray();
    }

    private byte[] Encrypt(byte[] buffer, int size, out int sizePadded)
    {
      int length = this.AES_KEY.Length;
      sizePadded = size + (length - size % length);
      byte[] outputBuffer = new byte[sizePadded];
      byte[] inputBuffer = new byte[sizePadded];
      Array.Copy((Array) buffer, 0, (Array) inputBuffer, 0, buffer.Length);
      buffer = (byte[]) null;
      Rijndael rijndael = Rijndael.Create();
      rijndael.Mode = CipherMode.ECB;
      rijndael.CreateEncryptor(Encoding.ASCII.GetBytes(this.AES_KEY), new byte[16]).TransformBlock(inputBuffer, 0, sizePadded, outputBuffer, 0);
      return outputBuffer;
    }

    private byte[] Pack(byte[] buffer, int sizeUnpacked, out int sizeSheared, out int sizeStored, bool encrypt, bool compress, int compressionLevel)
    {
      byte[] buffer1 = buffer;
      buffer = (byte[]) null;
      sizeSheared = sizeUnpacked;
      sizeStored = sizeSheared;
      if (compress)
      {
        byte[] numArray = this.Inflate(buffer1, sizeUnpacked, out sizeSheared, compressionLevel);
        sizeStored = sizeSheared;
        buffer1 = numArray;
      }
      if (encrypt)
        buffer1 = this.Encrypt(buffer1, sizeSheared, out sizeStored);
      return buffer1;
    }

    public void Extract(string FileName, bool is64 = false)
    {
      FileStream fileStream = new FileStream(FileName, FileMode.Open);
      BinaryReader binaryReader1 = new BinaryReader((Stream) fileStream);
      binaryReader1.ReadBytes(8);
      int num1 = (int) binaryReader1.ReadUInt32();
      binaryReader1.ReadBytes(5);
      if (!is64)
        binaryReader1.ReadInt32();
      else
        binaryReader1.ReadInt64();
      int num2 = is64 ? (int) binaryReader1.ReadInt64() : binaryReader1.ReadInt32();
      bool isCompressed = binaryReader1.ReadByte() == (byte) 1;
      bool isEncrypted = binaryReader1.ReadByte() == (byte) 1;
      binaryReader1.ReadBytes(62);
      int num3 = is64 ? (int) binaryReader1.ReadInt64() : binaryReader1.ReadInt32();
      int sizeUnpacked = is64 ? (int) binaryReader1.ReadInt64() : binaryReader1.ReadInt32();
      byte[] buffer1 = binaryReader1.ReadBytes(num3);
      int num4 = is64 ? (int) binaryReader1.ReadInt64() : binaryReader1.ReadInt32();
      int position = (int) binaryReader1.BaseStream.Position;
      byte[] buffer2 = this.Unpack(buffer1, num3, num3, sizeUnpacked, isEncrypted, isCompressed);
      byte[] numArray1 = (byte[]) null;
      MemoryStream memoryStream1 = new MemoryStream(buffer2);
      BinaryReader binaryReader2 = new BinaryReader((Stream) memoryStream1);
      for (int index = 0; index < num2; ++index)
      {
        BPKG_FTE bpkgFte = new BPKG_FTE()
        {
          FilePathLength = is64 ? (int) binaryReader2.ReadInt64() : binaryReader2.ReadInt32()
        };
        bpkgFte.FilePath = Encoding.Unicode.GetString(binaryReader2.ReadBytes(bpkgFte.FilePathLength * 2));
        bpkgFte.Unknown_001 = binaryReader2.ReadByte();
        bpkgFte.IsCompressed = binaryReader2.ReadByte() == (byte) 1;
        bpkgFte.IsEncrypted = binaryReader2.ReadByte() == (byte) 1;
        bpkgFte.Unknown_002 = binaryReader2.ReadByte();
        bpkgFte.FileDataSizeUnpacked = is64 ? (int) binaryReader2.ReadInt64() : binaryReader2.ReadInt32();
        bpkgFte.FileDataSizeSheared = is64 ? (int) binaryReader2.ReadInt64() : binaryReader2.ReadInt32();
        bpkgFte.FileDataSizeStored = is64 ? (int) binaryReader2.ReadInt64() : binaryReader2.ReadInt32();
        bpkgFte.FileDataOffset = (is64 ? (int) binaryReader2.ReadInt64() : binaryReader2.ReadInt32()) + position;
        bpkgFte.Padding = binaryReader2.ReadBytes(60);
        string str1 = string.Format("{0}.files\\{1}", (object) FileName, (object) bpkgFte.FilePath);
        if (!Directory.Exists(new FileInfo(str1).DirectoryName))
          Directory.CreateDirectory(new FileInfo(str1).DirectoryName);
        binaryReader1.BaseStream.Position = (long) bpkgFte.FileDataOffset;
        byte[] numArray2 = this.Unpack(binaryReader1.ReadBytes(bpkgFte.FileDataSizeStored), bpkgFte.FileDataSizeStored, bpkgFte.FileDataSizeSheared, bpkgFte.FileDataSizeUnpacked, bpkgFte.IsEncrypted, bpkgFte.IsCompressed);
        numArray1 = (byte[]) null;
        byte[] numArray3;
        if (str1.EndsWith("xml") || str1.EndsWith("x16"))
        {
          MemoryStream memoryStream2 = new MemoryStream();
          MemoryStream memoryStream3 = new MemoryStream(numArray2);
          BXML bxml = new BXML(this.XOR_KEY);
          this.Convert((Stream) memoryStream3, bxml.DetectType((Stream) memoryStream3), (Stream) memoryStream2, BXML_TYPE.BXML_PLAIN);
          memoryStream3.Close();
          File.WriteAllBytes(str1, memoryStream2.ToArray());
          memoryStream2.Close();
          numArray3 = (byte[]) null;
        }
        else
        {
          File.WriteAllBytes(str1, numArray2);
          numArray3 = (byte[]) null;
        }
        string str2 = "Extracting: " + index.ToString() + "/" + num2.ToString();
      }
      binaryReader2.Close();
      memoryStream1.Close();
      binaryReader1.Close();
      fileStream.Close();
    }

    public void Compress(string Folder, bool is64 = false, int compression = 9)
    {
      string[] array1 = Directory.EnumerateFiles(Folder, "*", SearchOption.AllDirectories).ToArray<string>();
      int num1 = ((IEnumerable<string>) array1).Count<string>();
      BPKG_FTE bpkgFte = new BPKG_FTE();
      MemoryStream memoryStream1 = new MemoryStream();
      BinaryWriter binaryWriter1 = new BinaryWriter((Stream) memoryStream1);
      MemoryStream memoryStream2 = new MemoryStream();
      BinaryWriter binaryWriter2 = new BinaryWriter((Stream) memoryStream2);
      byte[] numArray1;
      byte[] numArray2;
      for (int index = 0; index < num1; ++index)
      {
        string str1 = array1[index].Replace(Folder, "").TrimStart('\\');
        bpkgFte.FilePathLength = str1.Length;
        if (is64)
          binaryWriter1.Write((long) bpkgFte.FilePathLength);
        else
          binaryWriter1.Write(bpkgFte.FilePathLength);
        bpkgFte.FilePath = str1;
        binaryWriter1.Write(Encoding.Unicode.GetBytes(bpkgFte.FilePath));
        bpkgFte.Unknown_001 = (byte) 2;
        binaryWriter1.Write(bpkgFte.Unknown_001);
        bpkgFte.IsCompressed = true;
        binaryWriter1.Write(bpkgFte.IsCompressed);
        bpkgFte.IsEncrypted = true;
        binaryWriter1.Write(bpkgFte.IsEncrypted);
        bpkgFte.Unknown_002 = (byte) 0;
        binaryWriter1.Write(bpkgFte.Unknown_002);
        FileStream fileStream = new FileStream(array1[index], FileMode.Open);
        MemoryStream memoryStream3 = new MemoryStream();
        if (str1.EndsWith(".xml") || str1.EndsWith(".x16"))
        {
          BXML bxml = new BXML(this.XOR_KEY);
          this.Convert((Stream) fileStream, bxml.DetectType((Stream) fileStream), (Stream) memoryStream3, BXML_TYPE.BXML_BINARY);
        }
        else
          fileStream.CopyTo((Stream) memoryStream3);
        fileStream.Close();
        bpkgFte.FileDataOffset = (int) binaryWriter2.BaseStream.Position;
        bpkgFte.FileDataSizeUnpacked = (int) memoryStream3.Length;
        if (is64)
          binaryWriter1.Write((long) bpkgFte.FileDataSizeUnpacked);
        else
          binaryWriter1.Write(bpkgFte.FileDataSizeUnpacked);
        byte[] array2 = memoryStream3.ToArray();
        memoryStream3.Close();
        byte[] buffer = this.Pack(array2, bpkgFte.FileDataSizeUnpacked, out bpkgFte.FileDataSizeSheared, out bpkgFte.FileDataSizeStored, bpkgFte.IsEncrypted, bpkgFte.IsCompressed, compression);
        numArray1 = (byte[]) null;
        binaryWriter2.Write(buffer);
        numArray2 = (byte[]) null;
        if (is64)
          binaryWriter1.Write((long) bpkgFte.FileDataSizeSheared);
        else
          binaryWriter1.Write(bpkgFte.FileDataSizeSheared);
        if (is64)
          binaryWriter1.Write((long) bpkgFte.FileDataSizeStored);
        else
          binaryWriter1.Write(bpkgFte.FileDataSizeStored);
        if (is64)
          binaryWriter1.Write((long) bpkgFte.FileDataOffset);
        else
          binaryWriter1.Write(bpkgFte.FileDataOffset);
        bpkgFte.Padding = new byte[60];
        binaryWriter1.Write(bpkgFte.Padding);
        string str2 = "Compiling: " + index.ToString() + "/" + num1.ToString();
      }
      MemoryStream memoryStream4 = new MemoryStream();
      BinaryWriter binaryWriter3 = new BinaryWriter((Stream) memoryStream4);
      byte[] buffer1 = new byte[8]
      {
        (byte) 85,
        (byte) 79,
        (byte) 83,
        (byte) 69,
        (byte) 68,
        (byte) 65,
        (byte) 76,
        (byte) 66
      };
      binaryWriter3.Write(buffer1);
      int num2 = 2;
      binaryWriter3.Write(num2);
      byte[] buffer2 = new byte[5];
      binaryWriter3.Write(buffer2);
      int length1 = (int) binaryWriter2.BaseStream.Length;
      if (is64)
      {
        binaryWriter3.Write((long) length1);
        binaryWriter3.Write((long) num1);
      }
      else
      {
        binaryWriter3.Write(length1);
        binaryWriter3.Write(num1);
      }
      bool compress = true;
      binaryWriter3.Write(compress);
      bool encrypt = true;
      binaryWriter3.Write(encrypt);
      byte[] buffer3 = new byte[62];
      binaryWriter3.Write(buffer3);
      int length2 = (int) binaryWriter1.BaseStream.Length;
      int sizeSheared = length2;
      int sizeStored = length2;
      byte[] array3 = memoryStream1.ToArray();
      binaryWriter1.Close();
      memoryStream1.Close();
      byte[] buffer4 = this.Pack(array3, length2, out sizeSheared, out sizeStored, encrypt, compress, compression);
      numArray1 = (byte[]) null;
      if (is64)
        binaryWriter3.Write((long) sizeStored);
      else
        binaryWriter3.Write(sizeStored);
      if (is64)
        binaryWriter3.Write((long) length2);
      else
        binaryWriter3.Write(length2);
      binaryWriter3.Write(buffer4);
      numArray2 = (byte[]) null;
      int num3 = (int) memoryStream4.Position + (is64 ? 8 : 4);
      if (is64)
        binaryWriter3.Write((long) num3);
      else
        binaryWriter3.Write(num3);
      byte[] array4 = memoryStream2.ToArray();
      binaryWriter2.Close();
      memoryStream2.Close();
      binaryWriter3.Write(array4);
      numArray2 = (byte[]) null;
      File.WriteAllBytes(Folder.Replace(".files", ""), memoryStream4.ToArray());
      binaryWriter3.Close();
      memoryStream4.Close();
    }

    private void Convert(Stream iStream, BXML_TYPE iType, Stream oStream, BXML_TYPE oType)
    {
      if (iType == BXML_TYPE.BXML_PLAIN && oType == BXML_TYPE.BXML_BINARY || iType == BXML_TYPE.BXML_BINARY && oType == BXML_TYPE.BXML_PLAIN)
      {
        BXML bxml = new BXML(this.XOR_KEY);
        bxml.Load(iStream, iType);
        bxml.Save(oStream, oType);
      }
      else
        iStream.CopyTo(oStream);
    }
  }
}
