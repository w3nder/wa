// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IContactFolderContactsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IContactFolderContactsCollectionRequest : IBaseRequest
  {
    Task<Contact> AddAsync(Contact contact);

    Task<Contact> AddAsync(Contact contact, CancellationToken cancellationToken);

    Task<IContactFolderContactsCollectionPage> GetAsync();

    Task<IContactFolderContactsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IContactFolderContactsCollectionRequest Expand(string value);

    IContactFolderContactsCollectionRequest Select(string value);

    IContactFolderContactsCollectionRequest Top(int value);

    IContactFolderContactsCollectionRequest Filter(string value);

    IContactFolderContactsCollectionRequest Skip(int value);

    IContactFolderContactsCollectionRequest OrderBy(string value);
  }
}
