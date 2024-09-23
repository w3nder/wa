// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.MessageCreateForwardRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public class MessageCreateForwardRequest : BaseRequest, IMessageCreateForwardRequest, IBaseRequest
  {
    public MessageCreateForwardRequest(
      string requestUrl,
      IBaseClient client,
      IEnumerable<Option> options)
      : base(requestUrl, client, options)
    {
      this.Method = "POST";
    }

    public Task<Message> PostAsync() => this.PostAsync(CancellationToken.None);

    public Task<Message> PostAsync(CancellationToken cancellationToken)
    {
      return this.SendAsync<Message>((object) null, cancellationToken);
    }

    public IMessageCreateForwardRequest Expand(string value)
    {
      this.QueryOptions.Add(new QueryOption("$expand", value));
      return (IMessageCreateForwardRequest) this;
    }

    public IMessageCreateForwardRequest Select(string value)
    {
      this.QueryOptions.Add(new QueryOption("$select", value));
      return (IMessageCreateForwardRequest) this;
    }
  }
}
