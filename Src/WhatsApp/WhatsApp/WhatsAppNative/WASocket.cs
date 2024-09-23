// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.WASocket
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  [MarshalingBehavior]
  [Activatable(100794368)]
  public sealed class WASocket : IWASocket
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern WASocket();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetHost([In] IHost Host);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetPort([In] ushort Port);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetHandler([In] IWASocketHandler Handler);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetTimeoutMilliseconds([In] int Milliseconds, [In] bool Cumulative);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Connect([In] IWAScheduler sched);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Send([In] IByteBuffer Buffer);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Dispose();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Close();
  }
}
