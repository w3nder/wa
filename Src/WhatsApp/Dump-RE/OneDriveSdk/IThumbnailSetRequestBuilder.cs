// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IThumbnailSetRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IThumbnailSetRequestBuilder : IBaseRequestBuilder
  {
    IThumbnailRequestBuilder this[string size] { get; }

    IThumbnailSetRequest Request();

    IThumbnailSetRequest Request(IEnumerable<Option> options);
  }
}
