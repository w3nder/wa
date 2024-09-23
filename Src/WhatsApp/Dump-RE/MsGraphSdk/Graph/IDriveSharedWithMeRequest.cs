// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IDriveSharedWithMeRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IDriveSharedWithMeRequest : IBaseRequest
  {
    Task<IDriveSharedWithMeCollectionPage> GetAsync();

    Task<IDriveSharedWithMeCollectionPage> GetAsync(CancellationToken cancellationToken);

    IDriveSharedWithMeRequest Expand(string value);

    IDriveSharedWithMeRequest Select(string value);

    IDriveSharedWithMeRequest Top(int value);

    IDriveSharedWithMeRequest Filter(string value);

    IDriveSharedWithMeRequest Skip(int value);

    IDriveSharedWithMeRequest OrderBy(string value);
  }
}
