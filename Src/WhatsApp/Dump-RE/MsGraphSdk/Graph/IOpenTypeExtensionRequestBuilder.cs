﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IOpenTypeExtensionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public interface IOpenTypeExtensionRequestBuilder : 
    IExtensionRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    IOpenTypeExtensionRequest Request();

    IOpenTypeExtensionRequest Request(IEnumerable<Option> options);
  }
}
