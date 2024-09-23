// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.IItemCreateSessionRequest
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public interface IItemCreateSessionRequest : IBaseRequest
  {
    ItemCreateSessionRequestBody RequestBody { get; }

    Task<UploadSession> PostAsync();

    Task<UploadSession> PostAsync(CancellationToken cancellationToken);

    IItemCreateSessionRequest Expand(string value);

    IItemCreateSessionRequest Select(string value);
  }
}
