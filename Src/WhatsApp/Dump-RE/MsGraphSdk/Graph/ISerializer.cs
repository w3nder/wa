// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ISerializer
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.IO;

#nullable disable
namespace Microsoft.Graph
{
  public interface ISerializer
  {
    T DeserializeObject<T>(Stream stream);

    T DeserializeObject<T>(string inputString);

    string SerializeObject(object serializeableObject);
  }
}
