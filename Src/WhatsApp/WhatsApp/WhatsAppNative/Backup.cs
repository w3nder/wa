// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.Backup
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  [Activatable(100794368)]
  [MarshalingBehavior]
  public sealed class Backup : IBackup
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern Backup();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string GetBackupDirs();
  }
}
