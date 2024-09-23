// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.RegHelper
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
  public sealed class RegHelper : IRegHelper
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern RegHelper();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string ReadString([In] uint Key, [In] string Subkey, [In] string Value);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern uint ReadDWord([In] uint Key, [In] string Subkey, [In] string Value);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IAction NotifyChangedImpl([In] uint Key, [In] string Subkey, [In] IAction Callback);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void WriteDWord([In] uint Key, [In] string Subkey, [In] string Value, [In] uint Word);
  }
}
