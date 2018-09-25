// Decompiled with JetBrains decompiler
// Type: BNSDat.BXML_CONTENT
// Assembly: BnS Buddy, Version=5.9.0.3, Culture=neutral, PublicKeyToken=null
// MVID: 9E2C87FE-8AF0-4CC8-AEAD-1B0C55517684
// Assembly location: E:\Games\GarenaBnSVN\BNS Buddy\BnS Buddy.exe

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace BNSDat
{
  internal class BXML_CONTENT
  {
    private bool Keep_XML_WhiteSpace = true;
    private XmlDocument Nodes = new XmlDocument();
    public byte[] XOR_KEY;
    private byte[] Signature;
    private int Version;
    private int FileSize;
    private byte[] Padding;
    private bool Unknown;
    private int OriginalPathLength;
    private byte[] OriginalPath;
    private int AutoID;

    private void Xor(byte[] buffer, int size)
    {
      for (int index = 0; index < size; ++index)
        buffer[index] = (byte) ((uint) buffer[index] ^ (uint) this.XOR_KEY[index % this.XOR_KEY.Length]);
    }

    public void Read(Stream iStream, BXML_TYPE iType)
    {
      if (iType == BXML_TYPE.BXML_PLAIN)
      {
        this.Signature = new byte[8]
        {
          (byte) 76,
          (byte) 77,
          (byte) 88,
          (byte) 66,
          (byte) 79,
          (byte) 83,
          (byte) 76,
          (byte) 66
        };
        this.Version = 3;
        this.FileSize = 85;
        this.Padding = new byte[64];
        this.Unknown = true;
        this.OriginalPathLength = 0;
        this.Nodes.PreserveWhitespace = this.Keep_XML_WhiteSpace;
        this.Nodes.Load(iStream);
        XmlNode xmlNode = (XmlNode) this.Nodes.DocumentElement.ChildNodes.OfType<XmlComment>().First<XmlComment>();
        if (xmlNode != null && xmlNode.NodeType == XmlNodeType.Comment)
        {
          string innerText = xmlNode.InnerText;
          this.OriginalPathLength = innerText.Length;
          this.OriginalPath = Encoding.Unicode.GetBytes(innerText);
          this.Xor(this.OriginalPath, 2 * this.OriginalPathLength);
          if (this.Nodes.PreserveWhitespace && xmlNode.NextSibling.NodeType == XmlNodeType.Whitespace)
            this.Nodes.DocumentElement.RemoveChild(xmlNode.NextSibling);
        }
        else
          this.OriginalPath = new byte[2 * this.OriginalPathLength];
      }
      if (iType != BXML_TYPE.BXML_BINARY)
        return;
      this.Signature = new byte[8];
      BinaryReader binaryReader = new BinaryReader(iStream);
      binaryReader.BaseStream.Position = 0L;
      this.Signature = binaryReader.ReadBytes(8);
      this.Version = binaryReader.ReadInt32();
      this.FileSize = binaryReader.ReadInt32();
      this.Padding = binaryReader.ReadBytes(64);
      this.Unknown = binaryReader.ReadByte() == (byte) 1;
      this.OriginalPathLength = binaryReader.ReadInt32();
      this.OriginalPath = binaryReader.ReadBytes(2 * this.OriginalPathLength);
      this.AutoID = 1;
      this.ReadNode(iStream, (XmlNode) null);
      byte[] originalPath = this.OriginalPath;
      this.Xor(originalPath, 2 * this.OriginalPathLength);
      this.Nodes.DocumentElement.PrependChild((XmlNode) this.Nodes.CreateComment(Encoding.Unicode.GetString(originalPath)));
      this.Nodes.PrependChild((XmlNode) this.Nodes.CreateXmlDeclaration("1.0", "utf-8", (string) null));
      if ((long) this.FileSize != iStream.Position)
        throw new Exception(string.Format("Filesize Mismatch, expected size was {0} while actual size was {1}.", (object) this.FileSize, (object) iStream.Position));
    }

    public void Write(Stream oStream, BXML_TYPE oType)
    {
      if (oType == BXML_TYPE.BXML_PLAIN)
        this.Nodes.Save(oStream);
      if (oType != BXML_TYPE.BXML_BINARY)
        return;
      BinaryWriter binaryWriter = new BinaryWriter(oStream);
      binaryWriter.Write(this.Signature);
      binaryWriter.Write(this.Version);
      binaryWriter.Write(this.FileSize);
      binaryWriter.Write(this.Padding);
      binaryWriter.Write(this.Unknown);
      binaryWriter.Write(this.OriginalPathLength);
      binaryWriter.Write(this.OriginalPath);
      this.AutoID = 1;
      this.WriteNode(oStream, (XmlNode) null);
      this.FileSize = (int) oStream.Position;
      oStream.Position = 12L;
      binaryWriter.Write(this.FileSize);
    }

    private void ReadNode(Stream iStream, XmlNode parent = null)
    {
      XmlNode xmlNode = (XmlNode) null;
      BinaryReader binaryReader = new BinaryReader(iStream);
      int num1 = 1;
      if (parent != null)
        num1 = binaryReader.ReadInt32();
      if (num1 == 1)
      {
        xmlNode = (XmlNode) this.Nodes.CreateElement("Text");
        int num2 = binaryReader.ReadInt32();
        for (int index = 0; index < num2; ++index)
        {
          int num3 = binaryReader.ReadInt32();
          byte[] numArray1 = binaryReader.ReadBytes(2 * num3);
          this.Xor(numArray1, 2 * num3);
          int num4 = binaryReader.ReadInt32();
          byte[] numArray2 = binaryReader.ReadBytes(2 * num4);
          this.Xor(numArray2, 2 * num4);
          ((XmlElement) xmlNode).SetAttribute(Encoding.Unicode.GetString(numArray1), Encoding.Unicode.GetString(numArray2));
        }
      }
      if (num1 == 2)
      {
        xmlNode = (XmlNode) this.Nodes.CreateTextNode("");
        int num2 = binaryReader.ReadInt32();
        byte[] numArray = binaryReader.ReadBytes(num2 * 2);
        this.Xor(numArray, 2 * num2);
        xmlNode.Value = Encoding.Unicode.GetString(numArray);
      }
      if (num1 > 2)
        throw new Exception("Unknown XML Node Type");
      int num5 = (int) binaryReader.ReadByte();
      int num6 = binaryReader.ReadInt32();
      byte[] numArray3 = binaryReader.ReadBytes(2 * num6);
      this.Xor(numArray3, 2 * num6);
      if (num1 == 1)
        xmlNode = BXML_CONTENT.RenameNode(xmlNode, Encoding.Unicode.GetString(numArray3));
      int num7 = binaryReader.ReadInt32();
      this.AutoID = binaryReader.ReadInt32();
      ++this.AutoID;
      for (int index = 0; index < num7; ++index)
        this.ReadNode(iStream, xmlNode);
      if (parent != null)
      {
        if (!this.Keep_XML_WhiteSpace && num1 == 2 && string.IsNullOrWhiteSpace(xmlNode.Value))
          return;
        parent.AppendChild(xmlNode);
      }
      else
        this.Nodes.AppendChild(xmlNode);
    }

    public static XmlNode RenameNode(XmlNode node, string Name)
    {
      if (node.NodeType != XmlNodeType.Element)
        return node;
      XmlElement xmlElement = (XmlElement) node;
      XmlElement element = node.OwnerDocument.CreateElement(Name);
      while (xmlElement.HasAttributes)
        element.SetAttributeNode(xmlElement.RemoveAttributeNode(xmlElement.Attributes[0]));
      while (xmlElement.HasChildNodes)
        element.AppendChild(xmlElement.FirstChild);
      if (xmlElement.ParentNode != null)
        xmlElement.ParentNode.ReplaceChild((XmlNode) element, (XmlNode) xmlElement);
      return (XmlNode) element;
    }

    private bool WriteNode(Stream oStream, XmlNode parent = null)
    {
      BinaryWriter binaryWriter = new BinaryWriter(oStream);
      int num1 = 1;
      XmlNode xmlNode;
      if (parent != null)
      {
        xmlNode = parent;
        if (xmlNode.NodeType == XmlNodeType.Element)
          num1 = 1;
        if (xmlNode.NodeType == XmlNodeType.Text || xmlNode.NodeType == XmlNodeType.Whitespace)
          num1 = 2;
        if (xmlNode.NodeType == XmlNodeType.Comment)
          return false;
        binaryWriter.Write(num1);
      }
      else
        xmlNode = (XmlNode) this.Nodes.DocumentElement;
      if (num1 == 1)
      {
        int position1 = (int) oStream.Position;
        int num2 = 0;
        binaryWriter.Write(num2);
        foreach (XmlAttribute attribute in (XmlNamedNodeMap) xmlNode.Attributes)
        {
          string name = attribute.Name;
          int length1 = name.Length;
          binaryWriter.Write(length1);
          byte[] bytes1 = Encoding.Unicode.GetBytes(name);
          this.Xor(bytes1, 2 * length1);
          binaryWriter.Write(bytes1);
          string s = attribute.Value;
          int length2 = s.Length;
          binaryWriter.Write(length2);
          byte[] bytes2 = Encoding.Unicode.GetBytes(s);
          this.Xor(bytes2, 2 * length2);
          binaryWriter.Write(bytes2);
          ++num2;
        }
        int position2 = (int) oStream.Position;
        oStream.Position = (long) position1;
        binaryWriter.Write(num2);
        oStream.Position = (long) position2;
      }
      if (num1 == 2)
      {
        string s = xmlNode.Value;
        int length = s.Length;
        binaryWriter.Write(length);
        byte[] bytes = Encoding.Unicode.GetBytes(s);
        this.Xor(bytes, 2 * length);
        binaryWriter.Write(bytes);
      }
      if (num1 > 2)
        throw new Exception(string.Format("ERROR: XML NODE TYPE [{0}] UNKNOWN", (object) xmlNode.NodeType.ToString()));
      bool flag = true;
      binaryWriter.Write(flag);
      string name1 = xmlNode.Name;
      int length3 = name1.Length;
      binaryWriter.Write(length3);
      byte[] bytes3 = Encoding.Unicode.GetBytes(name1);
      this.Xor(bytes3, 2 * length3);
      binaryWriter.Write(bytes3);
      int position3 = (int) oStream.Position;
      int num3 = 0;
      binaryWriter.Write(num3);
      binaryWriter.Write(this.AutoID);
      ++this.AutoID;
      foreach (XmlNode childNode in xmlNode.ChildNodes)
      {
        if (this.WriteNode(oStream, childNode))
          ++num3;
      }
      int position4 = (int) oStream.Position;
      oStream.Position = (long) position3;
      binaryWriter.Write(num3);
      oStream.Position = (long) position4;
      return true;
    }
  }
}
