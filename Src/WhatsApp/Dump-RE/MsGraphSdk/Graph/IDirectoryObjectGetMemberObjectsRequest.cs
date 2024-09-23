// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IDirectoryObjectGetMemberObjectsRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IDirectoryObjectGetMemberObjectsRequest : IBaseRequest
  {
    DirectoryObjectGetMemberObjectsRequestBody RequestBody { get; }

    Task<IDirectoryObjectGetMemberObjectsCollectionPage> PostAsync();

    Task<IDirectoryObjectGetMemberObjectsCollectionPage> PostAsync(
      CancellationToken cancellationToken);

    IDirectoryObjectGetMemberObjectsRequest Expand(string value);

    IDirectoryObjectGetMemberObjectsRequest Select(string value);

    IDirectoryObjectGetMemberObjectsRequest Top(int value);

    IDirectoryObjectGetMemberObjectsRequest Filter(string value);

    IDirectoryObjectGetMemberObjectsRequest Skip(int value);

    IDirectoryObjectGetMemberObjectsRequest OrderBy(string value);
  }
}
