// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IConversationThreadReplyRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IConversationThreadReplyRequest : IBaseRequest
  {
    ConversationThreadReplyRequestBody RequestBody { get; }

    Task PostAsync();

    Task PostAsync(CancellationToken cancellationToken);

    IConversationThreadReplyRequest Expand(string value);

    IConversationThreadReplyRequest Select(string value);
  }
}
