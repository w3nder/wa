// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.KbdWatcher
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [MarshalingBehavior]
  [Version(100794368)]
  [Activatable(100794368)]
  public sealed class KbdWatcher : IKbdWatcher
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern KbdWatcher();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern uint GetKeyboardLCID();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool GetSuggestionBoxEnabled();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern uint GetSuggestionLines();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool WaitOne();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Dispose();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool GetIsCancelled();
  }
}
