// Decompiled with JetBrains decompiler
// Type: BNSDat.BXML
// Assembly: BnS Buddy, Version=5.9.0.3, Culture=neutral, PublicKeyToken=null
// MVID: 9E2C87FE-8AF0-4CC8-AEAD-1B0C55517684
// Assembly location: E:\Games\GarenaBnSVN\BNS Buddy\BnS Buddy.exe

using System;
using System.IO;

namespace BNSDat
{
  internal class BXML
  {
    private BXML_CONTENT _content = new BXML_CONTENT();

    private byte[] XOR_KEY
    {
      get
      {
        return this._content.XOR_KEY;
      }
      set
      {
        this._content.XOR_KEY = value;
      }
    }

    public BXML(byte[] xor)
    {
      this.XOR_KEY = xor;
    }

    public void Load(Stream iStream, BXML_TYPE iType)
    {
      this._content.Read(iStream, iType);
    }

    public void Save(Stream oStream, BXML_TYPE oType)
    {
      this._content.Write(oStream, oType);
    }

    public BXML_TYPE DetectType(Stream iStream)
    {
      int position = (int) iStream.Position;
      iStream.Position = 0L;
      byte[] buffer = new byte[13];
      iStream.Read(buffer, 0, 13);
      iStream.Position = (long) position;
      BXML_TYPE bxmlType = BXML_TYPE.BXML_UNKNOWN;
      if (BitConverter.ToString(buffer).Replace("-", "").Replace("00", "").Contains(BitConverter.ToString(new byte[5]
      {
        (byte) 60,
        (byte) 63,
        (byte) 120,
        (byte) 109,
        (byte) 108
      }).Replace("-", "")))
        bxmlType = BXML_TYPE.BXML_PLAIN;
      if (buffer[7] == (byte) 66 && buffer[6] == (byte) 76 && (buffer[5] == (byte) 83 && buffer[4] == (byte) 79) && (buffer[3] == (byte) 66 && buffer[2] == (byte) 88 && (buffer[1] == (byte) 77 && buffer[0] == (byte) 76)))
        bxmlType = BXML_TYPE.BXML_BINARY;
      return bxmlType;
    }
  }
}
