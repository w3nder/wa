// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.InterfaceConverter`1
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using Newtonsoft.Json.Converters;
using System;

#nullable disable
namespace Microsoft.Graph
{
  public class InterfaceConverter<T> : CustomCreationConverter<T> where T : new()
  {
    public override T Create(Type objectType) => new T();
  }
}
