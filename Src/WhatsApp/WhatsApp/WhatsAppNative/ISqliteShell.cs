// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.ISqliteShell
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(3924995244, 53668, 16621, 175, 158, 44, 167, 40, 215, 197, 188)]
  public interface ISqliteShell
  {
    void SetOutputCallback([In] ISqliteShellOutputCallback cb);

    void Start();

    void EnterCommand([In] string String);

    bool ExecuteMetaCommand([In] string Database, [In] string Command);

    bool ExecuteMetaCommandWithOutput([In] string Database, [In] string Command, [In] string Path);
  }
}
