// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.ICalendarRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface ICalendarRequest : IBaseRequest
  {
    Task<Calendar> CreateAsync(Calendar calendarToCreate);

    Task<Calendar> CreateAsync(Calendar calendarToCreate, CancellationToken cancellationToken);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task<Calendar> GetAsync();

    Task<Calendar> GetAsync(CancellationToken cancellationToken);

    Task<Calendar> UpdateAsync(Calendar calendarToUpdate);

    Task<Calendar> UpdateAsync(Calendar calendarToUpdate, CancellationToken cancellationToken);

    ICalendarRequest Expand(string value);

    ICalendarRequest Select(string value);
  }
}
