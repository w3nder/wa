// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.Authentication.WebAuthenticationBrokerWebAuthenticationUi
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

#nullable disable
namespace Microsoft.OneDrive.Sdk.Authentication
{
  public class WebAuthenticationBrokerWebAuthenticationUi : IWebAuthenticationUi
  {
    public async Task<IDictionary<string, string>> AuthenticateAsync(
      Uri requestUri,
      Uri callbackUri = null)
    {
      WebAuthenticationResult result = (WebAuthenticationResult) null;
      try
      {
        result = await this.AuthenticateAsync(requestUri, callbackUri, (WebAuthenticationOptions) 1);
      }
      catch (Exception ex)
      {
      }
      if (result == null || result.ResponseStatus == 1)
      {
        try
        {
          result = await this.AuthenticateAsync(requestUri, callbackUri, (WebAuthenticationOptions) 0);
        }
        catch (Exception ex)
        {
          throw new ServiceException(new Error()
          {
            Code = "authenticationFailure"
          }, ex);
        }
      }
      if (result != null && !string.IsNullOrEmpty(result.ResponseData))
        return UrlHelper.GetQueryOptions(new Uri(result.ResponseData));
      if (result != null && result.ResponseStatus == 1)
        throw new ServiceException(new Error()
        {
          Code = "authenticationCancelled",
          Message = "Authentication cancelled by user."
        });
      throw new ServiceException(new Error()
      {
        Code = "authenticationNeverOccured"
      });
    }

    private Task<WebAuthenticationResult> AuthenticateAsync(
      Uri requestUri,
      Uri callbackUri,
      WebAuthenticationOptions authenticationOptions)
    {
      return !(callbackUri == (Uri) null) ? WebAuthenticationBroker.AuthenticateAsync(authenticationOptions, requestUri, callbackUri).AsTask<WebAuthenticationResult>() : WebAuthenticationBroker.AuthenticateAsync(authenticationOptions, requestUri).AsTask<WebAuthenticationResult>();
    }
  }
}
