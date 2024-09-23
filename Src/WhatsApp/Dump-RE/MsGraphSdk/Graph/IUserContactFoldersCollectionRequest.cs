// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IUserContactFoldersCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IUserContactFoldersCollectionRequest : IBaseRequest
  {
    Task<ContactFolder> AddAsync(ContactFolder contactFolder);

    Task<ContactFolder> AddAsync(ContactFolder contactFolder, CancellationToken cancellationToken);

    Task<IUserContactFoldersCollectionPage> GetAsync();

    Task<IUserContactFoldersCollectionPage> GetAsync(CancellationToken cancellationToken);

    IUserContactFoldersCollectionRequest Expand(string value);

    IUserContactFoldersCollectionRequest Select(string value);

    IUserContactFoldersCollectionRequest Top(int value);

    IUserContactFoldersCollectionRequest Filter(string value);

    IUserContactFoldersCollectionRequest Skip(int value);

    IUserContactFoldersCollectionRequest OrderBy(string value);
  }
}
