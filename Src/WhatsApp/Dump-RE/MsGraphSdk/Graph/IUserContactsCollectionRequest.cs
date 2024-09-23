// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IUserContactsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IUserContactsCollectionRequest : IBaseRequest
  {
    Task<Contact> AddAsync(Contact contact);

    Task<Contact> AddAsync(Contact contact, CancellationToken cancellationToken);

    Task<IUserContactsCollectionPage> GetAsync();

    Task<IUserContactsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IUserContactsCollectionRequest Expand(string value);

    IUserContactsCollectionRequest Select(string value);

    IUserContactsCollectionRequest Top(int value);

    IUserContactsCollectionRequest Filter(string value);

    IUserContactsCollectionRequest Skip(int value);

    IUserContactsCollectionRequest OrderBy(string value);
  }
}
