// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.SqliteShell
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Activatable(100794368)]
  [MarshalingBehavior]
  [Version(100794368)]
  public sealed class SqliteShell : ISqliteShell
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern SqliteShell();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetOutputCallback([In] ISqliteShellOutputCallback cb);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Start();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void EnterCommand([In] string String);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool ExecuteMetaCommand([In] string Database, [In] string Command);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool ExecuteMetaCommandWithOutput([In] string Database, [In] string Command, [In] string Path);
  }
}
