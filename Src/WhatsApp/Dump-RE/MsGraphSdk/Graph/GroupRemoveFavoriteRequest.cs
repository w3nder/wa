// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.GroupRemoveFavoriteRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class GroupRemoveFavoriteRequest : BaseRequest, IGroupRemoveFavoriteRequest, IBaseRequest
  {
    public GroupRemoveFavoriteRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.Method = "POST";
    }

    public Task PostAsync() => this.PostAsync(CancellationToken.None);

    public Task PostAsync(CancellationToken cancellationToken)
    {
      return this.SendAsync((object) null, cancellationToken);
    }

    public IGroupRemoveFavoriteRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IGroupRemoveFavoriteRequest) this;
    }

    public IGroupRemoveFavoriteRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IGroupRemoveFavoriteRequest) this;
    }
  }
}
