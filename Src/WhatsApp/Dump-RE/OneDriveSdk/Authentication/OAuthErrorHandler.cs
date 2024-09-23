// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.Authentication.OAuthErrorHandler
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using Microsoft.Graph;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.OneDrive.Sdk.Authentication
{
  public static class OAuthErrorHandler
  {
    public static void ThrowIfError(IDictionary<string, string> responseValues)
    {
      if (responseValues == null)
        return;
      string error = (string) null;
      string errorDescription = (string) null;
      if (!responseValues.TryGetValue("error_description", out errorDescription) && !responseValues.TryGetValue("error", out error))
        return;
      OAuthErrorHandler.ParseAuthenticationError(error, errorDescription);
    }

    private static void ParseAuthenticationError(string error, string errorDescription)
    {
      throw new ServiceException(new Error()
      {
        Code = "authenticationFailure".ToString(),
        Message = errorDescription ?? error
      });
    }
  }
}
