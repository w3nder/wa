// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.NativeRegex
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Activatable(100794368)]
  [MarshalingBehavior]
  public sealed class NativeRegex : IRegex
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern NativeRegex();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Initialize([In] string Pattern, [In] RegexOptions Options);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IMatch GetMatch([In] string Input, [In] int Offset, [In] int Length);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Dispose();
  }
}
