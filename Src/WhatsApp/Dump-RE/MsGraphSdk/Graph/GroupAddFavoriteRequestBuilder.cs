// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GroupAddFavoriteRequestBuilder
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Graph
{
  public class GroupAddFavoriteRequestBuilder : 
    BaseGetMethodRequestBuilder<IGroupAddFavoriteRequest>,
    IGroupAddFavoriteRequestBuilder
  {
    public GroupAddFavoriteRequestBuilder(string requestUrl, IBaseClient client)
      : base(requestUrl, client)
    {
    }

    protected override IGroupAddFavoriteRequest CreateRequest(
      string functionUrl,
      IEnumerable<Option> options)
    {
      return (IGroupAddFavoriteRequest) new GroupAddFavoriteRequest(functionUrl, this.Client, options);
    }
  }
}
