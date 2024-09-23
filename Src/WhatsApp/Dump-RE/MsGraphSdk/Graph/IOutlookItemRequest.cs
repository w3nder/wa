// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IOutlookItemRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IOutlookItemRequest : IBaseRequest
  {
    Task<OutlookItem> CreateAsync(OutlookItem outlookItemToCreate);

    Task<OutlookItem> CreateAsync(
      OutlookItem outlookItemToCreate,
      CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<OutlookItem> GetAsync();

    Task<OutlookItem> GetAsync(CancellationToken cancellationToken);

    Task<OutlookItem> UpdateAsync(OutlookItem outlookItemToUpdate);

    Task<OutlookItem> UpdateAsync(
      OutlookItem outlookItemToUpdate,
      CancellationToken cancellationToken);

    IOutlookItemRequest Expand(string value);

    IOutlookItemRequest Select(string value);
  }
}
