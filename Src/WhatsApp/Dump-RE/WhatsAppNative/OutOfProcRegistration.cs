// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.OutOfProcRegistration
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Activatable(100794368)]
  [MarshalingBehavior]
  [Version(100794368)]
  public sealed class OutOfProcRegistration : IOutOfProcRegistration
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern OutOfProcRegistration();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Register([In] string ClassName);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Dispose();
  }
}
