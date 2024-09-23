// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.DateConverter
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using Newtonsoft.Json;
using System;

#nullable disable
namespace Microsoft.Graph
{
  public class DateConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType) => objectType == typeof (Date);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      try
      {
        return (object) new Date((DateTime) serializer.Deserialize(reader, typeof (DateTime)));
      }
      catch (JsonSerializationException ex)
      {
        throw new ServiceException(new Error()
        {
          Code = ErrorConstants.Codes.GeneralException,
          Message = ErrorConstants.Messages.UnableToDeserializeDate
        }, (Exception) ex);
      }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      if (value is Date date)
        writer.WriteValue(date.ToString());
      else
        throw new ServiceException(new Error()
        {
          Code = ErrorConstants.Codes.GeneralException,
          Message = ErrorConstants.Messages.InvalidTypeForDateConverter
        });
    }
  }
}
