// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.

using System;
using System.ComponentModel;
using System.Xml.Serialization;

/// <remarks />
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
[XmlRoot(Namespace = "", IsNullable = false)]
public class XmlPatch
{
    /// <remarks />
    public byte Version { get; set; }

    /// <remarks />
    [XmlElement("Patch")]
    public Patch[] Patches { get; set; }
}

/// <remarks />
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
public class Patch
{
    /// <remarks />
    public string FileName { get; set; }

    /// <remarks />
    public string Description { get; set; }

    /// <remarks />
    [XmlElement("FindAndReplace")]
    public FindAndReplace[] FindAndReplace { get; set; }
}

/// <remarks />
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
public class FindAndReplace
{
    /// <remarks />
    public Find Find { get; set; }

    /// <remarks />
    public Replace Replace { get; set; }
}

/// <remarks />
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
public class Find
{
    /// <remarks />
    [XmlElement("option")]
    public Option Option { get; set; }
}

/// <remarks />
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
public class Option
{
    /// <remarks />
    [XmlAttribute("name")]
    public string Name { get; set; }

    /// <remarks />
    [XmlAttribute("value")]
    public string Value { get; set; }
}

/// <remarks />
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
public class Replace
{
    /// <remarks />
    [XmlElement("option")]
    public Option Option { get; set; }
}