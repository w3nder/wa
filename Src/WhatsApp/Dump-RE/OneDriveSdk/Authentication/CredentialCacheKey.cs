// Decompiled with JetBrains decompiler
// Type: Microsoft.OneDrive.Sdk.Authentication.CredentialCacheKey
// Assembly: OneDriveSdk, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5E7A8391-E23E-498D-A6DC-9ACB59AE0E08
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\OneDriveSdk.dll

#nullable disable
namespace Microsoft.OneDrive.Sdk.Authentication
{
  public class CredentialCacheKey
  {
    private const string Delimiter = ";";

    public string ClientId { get; set; }

    public string UserId { get; set; }

    public override bool Equals(object obj)
    {
      return obj is CredentialCacheKey credentialCacheKey && credentialCacheKey.GetHashCode() == this.GetHashCode();
    }

    public override int GetHashCode()
    {
      return string.Join(";", new string[2]
      {
        this.ClientId,
        this.UserId
      }).ToLowerInvariant().GetHashCode();
    }
  }
}
