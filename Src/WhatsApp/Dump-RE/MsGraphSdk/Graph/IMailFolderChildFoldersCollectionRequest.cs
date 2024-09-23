// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IMailFolderChildFoldersCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IMailFolderChildFoldersCollectionRequest : IBaseRequest
  {
    Task<MailFolder> AddAsync(MailFolder mailFolder);

    Task<MailFolder> AddAsync(MailFolder mailFolder, CancellationToken cancellationToken);

    Task<IMailFolderChildFoldersCollectionPage> GetAsync();

    Task<IMailFolderChildFoldersCollectionPage> GetAsync(CancellationToken cancellationToken);

    IMailFolderChildFoldersCollectionRequest Expand(string value);

    IMailFolderChildFoldersCollectionRequest Select(string value);

    IMailFolderChildFoldersCollectionRequest Top(int value);

    IMailFolderChildFoldersCollectionRequest Filter(string value);

    IMailFolderChildFoldersCollectionRequest Skip(int value);

    IMailFolderChildFoldersCollectionRequest OrderBy(string value);
  }
}
