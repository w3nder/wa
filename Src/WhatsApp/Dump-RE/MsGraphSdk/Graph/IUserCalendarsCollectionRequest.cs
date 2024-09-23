// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IUserCalendarsCollectionRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IUserCalendarsCollectionRequest : IBaseRequest
  {
    Task<Calendar> AddAsync(Calendar calendar);

    Task<Calendar> AddAsync(Calendar calendar, CancellationToken cancellationToken);

    Task<IUserCalendarsCollectionPage> GetAsync();

    Task<IUserCalendarsCollectionPage> GetAsync(CancellationToken cancellationToken);

    IUserCalendarsCollectionRequest Expand(string value);

    IUserCalendarsCollectionRequest Select(string value);

    IUserCalendarsCollectionRequest Top(int value);

    IUserCalendarsCollectionRequest Filter(string value);

    IUserCalendarsCollectionRequest Skip(int value);

    IUserCalendarsCollectionRequest OrderBy(string value);
  }
}
