// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.BaseClient
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

#nullable disable
namespace Microsoft.Graph
{
  public class BaseClient : IBaseClient
  {
    private string baseUrl;

    public BaseClient(
      string baseUrl,
      IAuthenticationProvider authenticationProvider,
      IHttpProvider httpProvider = null)
    {
      this.BaseUrl = baseUrl;
      this.AuthenticationProvider = authenticationProvider;
      this.HttpProvider = httpProvider ?? (IHttpProvider) new Microsoft.Graph.HttpProvider((ISerializer) new Serializer());
    }

    public IAuthenticationProvider AuthenticationProvider { get; set; }

    public string BaseUrl
    {
      get => this.baseUrl;
      set
      {
        this.baseUrl = !string.IsNullOrEmpty(value) ? value.TrimEnd('/') : throw new ServiceException(new Error()
        {
          Code = ErrorConstants.Codes.InvalidRequest,
          Message = ErrorConstants.Messages.BaseUrlMissing
        });
      }
    }

    public IHttpProvider HttpProvider { get; private set; }
  }
}
