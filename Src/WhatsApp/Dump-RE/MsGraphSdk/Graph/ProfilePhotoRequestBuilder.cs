// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ProfilePhotoRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class ProfilePhotoRequestBuilder : 
    EntityRequestBuilder,
    IProfilePhotoRequestBuilder,
    IEntityRequestBuilder,
    IBaseRequestBuilder
  {
    public ProfilePhotoRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IProfilePhotoRequest Request() => this.Request((IEnumerable<Option>) null);

    public IProfilePhotoRequest Request(IEnumerable<Option> options)
    {
      return (IProfilePhotoRequest) new ProfilePhotoRequest(this.RequestUrl, this.Client, options);
    }

    public IProfilePhotoContentRequestBuilder Content
    {
      get
      {
        return (IProfilePhotoContentRequestBuilder) new ProfilePhotoContentRequestBuilder(this.AppendSegmentToRequestUrl("$value"), this.Client);
      }
    }
  }
}
