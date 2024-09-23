// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Serialization.SerializationInfo
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Serialization
{
  public class SerializationInfo
  {
    private readonly Dictionary<string, SerializationInfo.ValueEntry> _values = new Dictionary<string, SerializationInfo.ValueEntry>();

    internal SerializationInfo(int referenceId) => this.ReferenceId = referenceId;

    internal int ReferenceId { get; private set; }

    public string TypeName { get; set; }

    public void AddValue(string name, object value)
    {
      this._values.Add(name, new SerializationInfo.ValueEntry(name, value));
    }

    public object GetValue(string name)
    {
      SerializationInfo.ValueEntry valueEntry;
      return !this._values.TryGetValue(name, out valueEntry) ? (object) null : valueEntry.Value;
    }

    internal XElement ToXElement()
    {
      XElement xelement = new XElement((XName) "o");
      xelement.Add((object) new XAttribute((XName) "i", (object) this.ReferenceId));
      xelement.Add((object) new XAttribute((XName) "t", (object) this.TypeName));
      foreach (KeyValuePair<string, SerializationInfo.ValueEntry> keyValuePair in this._values)
      {
        if (!(keyValuePair.Value.Value is SerializationInfo serializationInfo1))
        {
          if (!(keyValuePair.Value.Value is List<SerializationInfo> serializationInfoList))
          {
            if (keyValuePair.Value.Value != null)
              xelement.Add((object) new XElement((XName) "f", new object[2]
              {
                (object) new XAttribute((XName) "n", (object) keyValuePair.Value.Name),
                (object) new XAttribute((XName) "v", keyValuePair.Value.Value)
              }));
          }
          else
          {
            XElement content = new XElement((XName) "l", (object) new XAttribute((XName) "n", (object) keyValuePair.Value.Name));
            foreach (SerializationInfo serializationInfo in serializationInfoList)
              content.Add((object) new XElement((XName) "r", (object) new XAttribute((XName) "i", (object) serializationInfo.ReferenceId)));
            xelement.Add((object) content);
          }
        }
        else
          xelement.Add((object) new XElement((XName) "r", new object[2]
          {
            (object) new XAttribute((XName) "n", (object) keyValuePair.Value.Name),
            (object) new XAttribute((XName) "i", (object) serializationInfo1.ReferenceId)
          }));
      }
      return xelement;
    }

    internal SerializationInfo(XElement data)
    {
      this.ReferenceId = Convert.ToInt32(data.Attribute((XName) "i").Value);
      if (!(data.Name == (XName) "o"))
        return;
      this.TypeName = data.Attribute((XName) "t").Value;
    }

    internal void Deserialize(XElement data, XmlFormatter formatter)
    {
      foreach (XElement element in data.Elements())
      {
        if (element.Name == (XName) "f")
        {
          SerializationInfo.ValueEntry valueEntry = new SerializationInfo.ValueEntry(element.Attribute((XName) "n").Value, (object) element.Attribute((XName) "v").Value);
          this._values.Add(valueEntry.Name, valueEntry);
        }
        else if (element.Name == (XName) "l")
        {
          List<SerializationInfo> list = element.Elements().Select<XElement, SerializationInfo>((Func<XElement, SerializationInfo>) (content => new SerializationInfo(content))).ToList<SerializationInfo>();
          SerializationInfo.ValueEntry valueEntry = new SerializationInfo.ValueEntry(element.Attribute((XName) "n").Value, (object) list);
          this._values.Add(valueEntry.Name, valueEntry);
        }
        else
        {
          int int32 = Convert.ToInt32(element.Attribute((XName) "i").Value);
          SerializationInfo.ValueEntry valueEntry = new SerializationInfo.ValueEntry(element.Attribute((XName) "n").Value, (object) new SerializationInfo(int32));
          this._values.Add(valueEntry.Name, valueEntry);
        }
      }
    }

    private class ValueEntry
    {
      public string Name { get; private set; }

      public object Value { get; private set; }

      public ValueEntry(string name, object value)
      {
        this.Name = name;
        this.Value = value;
      }
    }
  }
}
