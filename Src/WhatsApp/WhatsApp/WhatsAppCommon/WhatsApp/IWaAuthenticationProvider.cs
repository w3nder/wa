// Decompiled with JetBrains decompiler
// Type: WhatsApp.IWaAuthenticationProvider
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Graph;
using Microsoft.OneDrive.Sdk.Authentication;
using System.Threading.Tasks;


namespace WhatsApp
{
  public interface IWaAuthenticationProvider : IAuthenticationProvider
  {
    AccountSession CurrentAccountSession { get; }

    bool IsAuthenticated { get; }

    Task SignOutAsync();
  }
}
