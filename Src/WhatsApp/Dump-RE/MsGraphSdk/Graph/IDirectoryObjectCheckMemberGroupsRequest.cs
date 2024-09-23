// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IDirectoryObjectCheckMemberGroupsRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IDirectoryObjectCheckMemberGroupsRequest : IBaseRequest
  {
    DirectoryObjectCheckMemberGroupsRequestBody RequestBody { get; }

    Task<IDirectoryObjectCheckMemberGroupsCollectionPage> PostAsync();

    Task<IDirectoryObjectCheckMemberGroupsCollectionPage> PostAsync(
      CancellationToken cancellationToken);

    IDirectoryObjectCheckMemberGroupsRequest Expand(string value);

    IDirectoryObjectCheckMemberGroupsRequest Select(string value);

    IDirectoryObjectCheckMemberGroupsRequest Top(int value);

    IDirectoryObjectCheckMemberGroupsRequest Filter(string value);

    IDirectoryObjectCheckMemberGroupsRequest Skip(int value);

    IDirectoryObjectCheckMemberGroupsRequest OrderBy(string value);
  }
}
