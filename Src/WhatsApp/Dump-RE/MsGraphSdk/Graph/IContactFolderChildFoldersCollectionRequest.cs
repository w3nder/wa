// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IContactFolderChildFoldersCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IContactFolderChildFoldersCollectionRequest : IBaseRequest
  {
    Task<ContactFolder> AddAsync(ContactFolder contactFolder);

    Task<ContactFolder> AddAsync(ContactFolder contactFolder, CancellationToken cancellationToken);

    Task<IContactFolderChildFoldersCollectionPage> GetAsync();

    Task<IContactFolderChildFoldersCollectionPage> GetAsync(CancellationToken cancellationToken);

    IContactFolderChildFoldersCollectionRequest Expand(string value);

    IContactFolderChildFoldersCollectionRequest Select(string value);

    IContactFolderChildFoldersCollectionRequest Top(int value);

    IContactFolderChildFoldersCollectionRequest Filter(string value);

    IContactFolderChildFoldersCollectionRequest Skip(int value);

    IContactFolderChildFoldersCollectionRequest OrderBy(string value);
  }
}
