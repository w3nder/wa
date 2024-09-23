// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IDirectoryObjectGetMemberGroupsRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IDirectoryObjectGetMemberGroupsRequest : IBaseRequest
  {
    DirectoryObjectGetMemberGroupsRequestBody RequestBody { get; }

    Task<IDirectoryObjectGetMemberGroupsCollectionPage> PostAsync();

    Task<IDirectoryObjectGetMemberGroupsCollectionPage> PostAsync(
      CancellationToken cancellationToken);

    IDirectoryObjectGetMemberGroupsRequest Expand(string value);

    IDirectoryObjectGetMemberGroupsRequest Select(string value);

    IDirectoryObjectGetMemberGroupsRequest Top(int value);

    IDirectoryObjectGetMemberGroupsRequest Filter(string value);

    IDirectoryObjectGetMemberGroupsRequest Skip(int value);

    IDirectoryObjectGetMemberGroupsRequest OrderBy(string value);
  }
}
