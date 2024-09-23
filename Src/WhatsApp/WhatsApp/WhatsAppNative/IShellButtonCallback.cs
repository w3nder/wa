// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IShellButtonCallback
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(1535095350, 9969, 17172, 170, 84, 28, 71, 66, 134, 206, 146)]
  public interface IShellButtonCallback
  {
    void OnShellButtonEvent([In] ShellButton button, [In] ShellButtonPressEvent @event);
  }
}
