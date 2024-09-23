// Decompiled with JetBrains decompiler
// Type: WhatsApp.JsonBase
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace WhatsApp
{
  [DataContract]
  public class JsonBase
  {
    public static T Deserialize<T>(Stream stream)
    {
      return (T) new DataContractJsonSerializer(typeof (T)).ReadObject(stream);
    }

    public static T Deserialize<T>(byte[] bytes, int offset, int len)
    {
      using (MemoryStream memoryStream = new MemoryStream(bytes, offset, len, false))
        return JsonBase.Deserialize<T>((Stream) memoryStream);
    }

    public void Serialize(Stream stream)
    {
      new DataContractJsonSerializer(this.GetType()).WriteObject(stream, (object) this);
    }

    public byte[] Serialize()
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        this.Serialize((Stream) memoryStream);
        return memoryStream.ToArray();
      }
    }
  }
}
