// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IContactRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IContactRequest : IBaseRequest
  {
    Task<Contact> CreateAsync(Contact contactToCreate);

    Task<Contact> CreateAsync(Contact contactToCreate, CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<Contact> GetAsync();

    Task<Contact> GetAsync(CancellationToken cancellationToken);

    Task<Contact> UpdateAsync(Contact contactToUpdate);

    Task<Contact> UpdateAsync(Contact contactToUpdate, CancellationToken cancellationToken);

    IContactRequest Expand(string value);

    IContactRequest Select(string value);
  }
}
