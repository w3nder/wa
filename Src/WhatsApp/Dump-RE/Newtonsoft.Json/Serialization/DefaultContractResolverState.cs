// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.DefaultContractResolverState
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Utilities;
using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  internal class DefaultContractResolverState
  {
    public Dictionary<ResolverContractKey, JsonContract> ContractCache;
    public PropertyNameTable NameTable = new PropertyNameTable();
  }
}
