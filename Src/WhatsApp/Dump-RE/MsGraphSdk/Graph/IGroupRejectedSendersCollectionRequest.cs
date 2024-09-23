// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IGroupRejectedSendersCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IGroupRejectedSendersCollectionRequest : IBaseRequest
  {
    Task<DirectoryObject> AddAsync(DirectoryObject directoryObject);

    Task<DirectoryObject> AddAsync(
      DirectoryObject directoryObject,
      CancellationToken cancellationToken);

    Task<IGroupRejectedSendersCollectionPage> GetAsync();

    Task<IGroupRejectedSendersCollectionPage> GetAsync(CancellationToken cancellationToken);

    IGroupRejectedSendersCollectionRequest Top(int value);

    IGroupRejectedSendersCollectionRequest OrderBy(string value);
  }
}
