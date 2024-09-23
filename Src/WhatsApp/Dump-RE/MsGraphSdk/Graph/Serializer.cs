// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.Serializer
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using Newtonsoft.Json;
using System.IO;
using System.Text;

#nullable disable
namespace Microsoft.Graph
{
  public class Serializer : ISerializer
  {
    private JsonSerializerSettings jsonSerializerSettings;

    public Serializer()
      : this(new JsonSerializerSettings()
      {
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        TypeNameHandling = TypeNameHandling.None
      })
    {
    }

    public Serializer(JsonSerializerSettings jsonSerializerSettings)
    {
      this.jsonSerializerSettings = jsonSerializerSettings;
    }

    public T DeserializeObject<T>(Stream stream)
    {
      if (stream == null)
        return default (T);
      using (StreamReader reader1 = new StreamReader(stream, Encoding.UTF8, true, 4096, true))
      {
        using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
          return new JsonSerializer().Deserialize<T>((JsonReader) reader2);
      }
    }

    public T DeserializeObject<T>(string inputString)
    {
      return string.IsNullOrEmpty(inputString) ? default (T) : JsonConvert.DeserializeObject<T>(inputString, this.jsonSerializerSettings);
    }

    public string SerializeObject(object serializeableObject)
    {
      switch (serializeableObject)
      {
        case null:
          return (string) null;
        case Stream stream:
          using (StreamReader streamReader = new StreamReader(stream))
            return streamReader.ReadToEnd();
        case string str:
          return str;
        default:
          return JsonConvert.SerializeObject(serializeableObject, this.jsonSerializerSettings);
      }
    }
  }
}
