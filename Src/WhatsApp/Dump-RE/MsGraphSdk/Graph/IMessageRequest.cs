// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IMessageRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IMessageRequest : IBaseRequest
  {
    Task<Message> CreateAsync(Message messageToCreate);

    Task<Message> CreateAsync(Message messageToCreate, CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<Message> GetAsync();

    Task<Message> GetAsync(CancellationToken cancellationToken);

    Task<Message> UpdateAsync(Message messageToUpdate);

    Task<Message> UpdateAsync(Message messageToUpdate, CancellationToken cancellationToken);

    IMessageRequest Expand(string value);

    IMessageRequest Select(string value);
  }
}
