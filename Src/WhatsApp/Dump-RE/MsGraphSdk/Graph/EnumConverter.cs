// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.EnumConverter
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

#nullable disable
namespace Microsoft.Graph
{
  public class EnumConverter : StringEnumConverter
  {
    public EnumConverter() => this.CamelCaseText = true;

    public override bool CanConvert(Type objectType) => true;

    public override bool CanWrite => true;

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      try
      {
        return base.ReadJson(reader, objectType, existingValue, serializer);
      }
      catch (JsonSerializationException ex)
      {
      }
      return (object) null;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
    }
  }
}
