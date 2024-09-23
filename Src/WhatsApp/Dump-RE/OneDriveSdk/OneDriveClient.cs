// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.OneDriveClient
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;

#nullable disable
namespace Microsoft.OneDrive.Sdk
{
  public class OneDriveClient : BaseClient, IOneDriveClient, IBaseClient
  {
    public IDriveRequestBuilder Drive
    {
      get
      {
        return (IDriveRequestBuilder) new DriveRequestBuilder(string.Format("{0}/{1}", (object) this.BaseUrl, (object) "drive"), (IBaseClient) this);
      }
    }

    public IItemRequestBuilder ItemWithPath(string path)
    {
      return (IItemRequestBuilder) new ItemRequestBuilder(string.Format("{0}{1}:", (object) this.BaseUrl, (object) path), (IBaseClient) this);
    }

    public OneDriveClient(
      IAuthenticationProvider authenticationProvider,
      IHttpProvider httpProvider = null)
      : this("https://graph.microsoft.com/v1.0", authenticationProvider, httpProvider)
    {
    }

    public OneDriveClient(
      string baseUrl,
      IAuthenticationProvider authenticationProvider,
      IHttpProvider httpProvider = null)
      : base(baseUrl, authenticationProvider, httpProvider)
    {
    }

    public IOneDriveDrivesCollectionRequestBuilder Drives
    {
      get
      {
        return (IOneDriveDrivesCollectionRequestBuilder) new OneDriveDrivesCollectionRequestBuilder(this.BaseUrl + "/drives", (IBaseClient) this);
      }
    }

    public IOneDriveSharesCollectionRequestBuilder Shares
    {
      get
      {
        return (IOneDriveSharesCollectionRequestBuilder) new OneDriveSharesCollectionRequestBuilder(this.BaseUrl + "/shares", (IBaseClient) this);
      }
    }
  }
}
