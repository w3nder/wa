// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.Authentication.AccountSession
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

using System;
using System.Collections.Generic;
using System.Net;

#nullable disable
namespace Microsoft.OneDrive.Sdk.Authentication
{
  public class AccountSession
  {
    public AccountSession()
    {
    }

    public AccountSession(
      IDictionary<string, string> authenticationResponseValues,
      string clientId = null)
    {
      this.ClientId = clientId;
      this.ParseAuthenticationResponseValues(authenticationResponseValues);
    }

    public string AccessToken { get; set; }

    public string AccessTokenType { get; set; }

    public string ClientId { get; set; }

    public DateTimeOffset ExpiresOnUtc { get; set; }

    public string RefreshToken { get; set; }

    public string[] Scopes { get; set; }

    public string UserId { get; set; }

    public bool CanRefresh => !string.IsNullOrEmpty(this.RefreshToken);

    public bool IsExpiring
    {
      get => this.ExpiresOnUtc <= (DateTimeOffset) DateTimeOffset.Now.UtcDateTime.AddMinutes(5.0);
    }

    public bool ShouldRefresh => string.IsNullOrEmpty(this.AccessToken) || this.IsExpiring;

    private void ParseAuthenticationResponseValues(
      IDictionary<string, string> authenticationResponseValues)
    {
      if (authenticationResponseValues == null)
        return;
      foreach (KeyValuePair<string, string> authenticationResponseValue in (IEnumerable<KeyValuePair<string, string>>) authenticationResponseValues)
      {
        switch (authenticationResponseValue.Key)
        {
          case "access_token":
            this.AccessToken = authenticationResponseValue.Value;
            continue;
          case "expires_in":
            this.ExpiresOnUtc = DateTimeOffset.UtcNow.Add(new TimeSpan(0, 0, int.Parse(authenticationResponseValue.Value)));
            continue;
          case "scope":
            string str = WebUtility.UrlDecode(authenticationResponseValue.Value);
            string[] strArray;
            if (!string.IsNullOrEmpty(str))
              strArray = str.Split(' ');
            else
              strArray = (string[]) null;
            this.Scopes = strArray;
            continue;
          case "user_id":
            this.UserId = authenticationResponseValue.Value;
            continue;
          case "refresh_token":
            this.RefreshToken = authenticationResponseValue.Value;
            continue;
          default:
            continue;
        }
      }
    }
  }
}
