// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IGroupAddFavoriteRequest
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Graph
{
  public interface IGroupAddFavoriteRequest : IBaseRequest
  {
    Task PostAsync();

    Task PostAsync(CancellationToken cancellationToken);

    IGroupAddFavoriteRequest Expand(string value);

    IGroupAddFavoriteRequest Select(string value);
  }
}
