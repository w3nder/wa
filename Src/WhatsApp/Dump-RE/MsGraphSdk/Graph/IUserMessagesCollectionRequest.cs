// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IUserMessagesCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IUserMessagesCollectionRequest : IBaseRequest
  {
    Task<Message> AddAsync(Message message);

    Task<Message> AddAsync(Message message, CancellationToken cancellationToken);

    Task<IUserMessagesCollectionPage> GetAsync();

    Task<IUserMessagesCollectionPage> GetAsync(CancellationToken cancellationToken);

    IUserMessagesCollectionRequest Expand(string value);

    IUserMessagesCollectionRequest Select(string value);

    IUserMessagesCollectionRequest Top(int value);

    IUserMessagesCollectionRequest Filter(string value);

    IUserMessagesCollectionRequest Skip(int value);

    IUserMessagesCollectionRequest OrderBy(string value);
  }
}
