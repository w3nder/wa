// Decompiled with JetBrains decompiler
// Type: WhatsApp.ShellButtonExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using WhatsAppNative;


namespace WhatsApp
{
  public static class ShellButtonExtensions
  {
    public static IDisposable RegisterShellButtonCallback(
      this IMisc misc,
      ShellButton button,
      Action<ShellButton, ShellButtonPressEvent> callback)
    {
      return misc.RegisterShellButtonCallback(button, (IShellButtonCallback) new ShellButtonExtensions.Callback()
      {
        Action = callback
      });
    }

    private class Callback : IShellButtonCallback
    {
      public Action<ShellButton, ShellButtonPressEvent> Action;

      public void OnShellButtonEvent(ShellButton button, ShellButtonPressEvent ev)
      {
        this.Action(button, ev);
      }
    }
  }
}
