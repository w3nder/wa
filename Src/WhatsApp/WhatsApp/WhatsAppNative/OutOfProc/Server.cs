// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.OutOfProc.Server
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative.OutOfProc
{
  [MarshalingBehavior]
  [Version(100794368)]
  [Activatable(100794368)]
  public sealed class Server : IOutOfProc
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern Server();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern object GetVoipInstance();
  }
}
