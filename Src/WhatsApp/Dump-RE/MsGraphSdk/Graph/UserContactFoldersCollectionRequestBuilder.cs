// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.UserContactFoldersCollectionRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class UserContactFoldersCollectionRequestBuilder : 
    BaseRequestBuilder,
    IUserContactFoldersCollectionRequestBuilder
  {
    public UserContactFoldersCollectionRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    public IUserContactFoldersCollectionRequest Request()
    {
      return this.Request((IEnumerable<Option>) null);
    }

    public IUserContactFoldersCollectionRequest Request(IEnumerable<Option> options)
    {
      return (IUserContactFoldersCollectionRequest) new UserContactFoldersCollectionRequest(this.RequestUrl, this.Client, options);
    }

    public IContactFolderRequestBuilder this[string id]
    {
      get
      {
        return (IContactFolderRequestBuilder) new ContactFolderRequestBuilder(this.AppendSegmentToRequestUrl(id), this.Client);
      }
    }
  }
}
