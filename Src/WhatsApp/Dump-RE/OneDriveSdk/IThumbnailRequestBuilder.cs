// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IThumbnailRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IThumbnailRequestBuilder : IBaseRequestBuilder
  {
    IThumbnailRequest Request();

    IThumbnailRequest Request(IEnumerable<Option> options);

    IThumbnailContentRequestBuilder Content { get; }
  }
}
