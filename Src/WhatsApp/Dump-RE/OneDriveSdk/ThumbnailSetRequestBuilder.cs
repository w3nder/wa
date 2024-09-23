// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.ThumbnailSetRequestBuilder
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class ThumbnailSetRequestBuilder : 
    BaseRequestBuilder,
    IThumbnailSetRequestBuilder,
    IBaseRequestBuilder
  {
    public IThumbnailRequestBuilder this[string size]
    {
      get
      {
        return (IThumbnailRequestBuilder) new ThumbnailRequestBuilder(this.AppendSegmentToRequestUrl(size), this.Client);
      }
    }

    public ThumbnailSetRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IThumbnailSetRequest Request() => this.Request((IEnumerable<Option>) null);

    public IThumbnailSetRequest Request(IEnumerable<Option> options)
    {
      return (IThumbnailSetRequest) new ThumbnailSetRequest(this.RequestUrl, this.Client, options);
    }
  }
}
