// Decompiled with JetBrains decompiler
// Type: ICSharpCode.SharpZipLib.Silverlight.Serialization.XmlFormatter
// Assembly: ICSharpCode.SharpZipLib.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C68203F-9543-4D84-A3B9-6AE68DADF1C2
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\ICSharpCode.SharpZipLib.WindowsPhone.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

#nullable disable
namespace ICSharpCode.SharpZipLib.Silverlight.Serialization
{
  public sealed class XmlFormatter
  {
    private readonly Dictionary<ISerializable, SerializationInfo> _serializationReferences = new Dictionary<ISerializable, SerializationInfo>();
    private readonly Dictionary<int, ISerializable> _deserializationReferences = new Dictionary<int, ISerializable>();

    public void Serialize(Stream serializationStream, object graph)
    {
      XmlWriter writer = XmlWriter.Create(serializationStream);
      this.Serialize(writer, graph);
      writer?.Flush();
    }

    public void Serialize(TextWriter textWriter, object graph)
    {
      XmlWriter writer = XmlWriter.Create(textWriter);
      this.Serialize(writer, graph);
      writer?.Flush();
    }

    public void Serialize(XmlWriter writer, object graph)
    {
      this._serializationReferences.Clear();
      XDocument xdocument = new XDocument();
      this.SerializeObject(graph);
      XElement content = new XElement((XName) "g");
      foreach (KeyValuePair<ISerializable, SerializationInfo> serializationReference in this._serializationReferences)
        content.Add((object) serializationReference.Value.ToXElement());
      xdocument.Add((object) content);
      xdocument.Save(writer);
    }

    internal SerializationInfo SerializeObject(object obj)
    {
      Type type = obj.GetType();
      if (!XmlFormatter.IsSerializable((ICustomAttributeProvider) type))
        throw new InvalidOperationException("Object not serializable");
      if (!(obj is ISerializable key))
        throw new InvalidOperationException(string.Format("Type {0} must implement ISerializable", (object) type.Name));
      SerializationInfo info;
      if (!this._serializationReferences.TryGetValue(key, out info))
      {
        info = new SerializationInfo(this._serializationReferences.Count + 1);
        this._serializationReferences.Add(key, info);
        key.Serialize(info, this);
      }
      return info;
    }

    private static bool IsSerializable(ICustomAttributeProvider objectType)
    {
      return objectType.GetCustomAttributes(typeof (SerializableAttribute), false).Length > 0;
    }

    public object Deserialize(Stream serializationStream)
    {
      return this.Deserialize(XmlReader.Create(serializationStream));
    }

    public object Deserialize(TextReader textReader)
    {
      return this.Deserialize(XmlReader.Create(textReader));
    }

    public object Deserialize(XmlReader reader)
    {
      XElement firstNode = (XElement) XDocument.Load(reader).FirstNode;
      this._deserializationReferences.Clear();
      IEnumerable<XElement> xelements = firstNode.Elements().Where<XElement>((Func<XElement, bool>) (e => e.Name == (XName) "o"));
      Dictionary<int, SerializationInfo> dictionary = new Dictionary<int, SerializationInfo>();
      foreach (XElement data in xelements)
      {
        SerializationInfo serializationInfo = new SerializationInfo(data);
        dictionary.Add(serializationInfo.ReferenceId, serializationInfo);
        ISerializable instance = Activator.CreateInstance(Type.GetType(serializationInfo.TypeName)) as ISerializable;
        this._deserializationReferences.Add(serializationInfo.ReferenceId, instance);
      }
      foreach (XElement data in xelements)
      {
        if (data != null)
        {
          int int32 = Convert.ToInt32(data.Attribute((XName) "i").Value);
          dictionary[int32].Deserialize(data, this);
        }
      }
      foreach (KeyValuePair<int, SerializationInfo> keyValuePair in dictionary)
        this.GetObject(keyValuePair.Value.ReferenceId).Deserialize(keyValuePair.Value, this);
      return (object) this._deserializationReferences[1];
    }

    internal ISerializable GetObject(int referenceId)
    {
      return this._deserializationReferences[referenceId];
    }
  }
}
