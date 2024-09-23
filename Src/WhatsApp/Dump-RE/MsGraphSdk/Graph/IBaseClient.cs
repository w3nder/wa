// Decompiled with JetBrains decompiler
// Type: Microsoft.Graph.IBaseClient
// Assembly: MsGraphSdk, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B6767127-13D0-4992-B741-2642C0E7F410
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\MsGraphSdk.dll

#nullable disable
namespace Microsoft.Graph
{
  public interface IBaseClient
  {
    IAuthenticationProvider AuthenticationProvider { get; }

    string BaseUrl { get; }

    IHttpProvider HttpProvider { get; }
  }
}
