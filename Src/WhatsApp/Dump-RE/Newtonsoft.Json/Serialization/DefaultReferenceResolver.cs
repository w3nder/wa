// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.DefaultReferenceResolver
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  internal class DefaultReferenceResolver : IReferenceResolver
  {
    private int _referenceCount;

    private BidirectionalDictionary<string, object> GetMappings(object context)
    {
      JsonSerializerInternalBase serializerInternalBase;
      switch (context)
      {
        case JsonSerializerInternalBase _:
          serializerInternalBase = (JsonSerializerInternalBase) context;
          break;
        case JsonSerializerProxy _:
          serializerInternalBase = ((JsonSerializerProxy) context).GetInternalSerializer();
          break;
        default:
          throw new JsonException("The DefaultReferenceResolver can only be used internally.");
      }
      return serializerInternalBase.DefaultReferenceMappings;
    }

    public object ResolveReference(object context, string reference)
    {
      object second;
      this.GetMappings(context).TryGetByFirst(reference, out second);
      return second;
    }

    public string GetReference(object context, object value)
    {
      BidirectionalDictionary<string, object> mappings = this.GetMappings(context);
      string first;
      if (!mappings.TryGetBySecond(value, out first))
      {
        ++this._referenceCount;
        first = this._referenceCount.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        mappings.Set(first, value);
      }
      return first;
    }

    public void AddReference(object context, string reference, object value)
    {
      this.GetMappings(context).Set(reference, value);
    }

    public bool IsReferenced(object context, object value)
    {
      return this.GetMappings(context).TryGetBySecond(value, out string _);
    }
  }
}
