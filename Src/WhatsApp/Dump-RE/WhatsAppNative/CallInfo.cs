// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.CallInfo
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
  public sealed class CallInfo : ICallInfo
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GetCallInfo(
      out string CallId,
      out string PeerJid,
      out CallInfoStruct InfoStruct);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool GetParticipantInfo(
      [In] int ParticipantIdx,
      out string ParticipantJid,
      out CallParticipantDetailInfo Participant);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool GetPendingCallInfo([In] int Idx, out string PeerJid, out string CallId);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool GetHeldCallInfo([In] int Idx, out string PeerJid, out string CallId);
  }
}
