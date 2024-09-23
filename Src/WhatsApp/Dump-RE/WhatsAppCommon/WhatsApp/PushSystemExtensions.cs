// Decompiled with JetBrains decompiler
// Type: WhatsApp.PushSystemExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public static class PushSystemExtensions
  {
    public static void ShellToast(
      this IPushSystem push,
      string msg,
      string uri,
      string tone = null,
      string tag = null)
    {
      push.ShellToastEx(new string[1]{ msg }, (string) null, uri, false, tone, tag);
    }
  }
}
