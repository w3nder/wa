// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.ICallInfo
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(3780818792, 59918, 20440, 131, 234, 35, 74, 129, 31, 14, 204)]
  public interface ICallInfo
  {
    void GetCallInfo(out string CallId, out string PeerJid, out CallInfoStruct InfoStruct);

    bool GetParticipantInfo(
      [In] int ParticipantIdx,
      out string ParticipantJid,
      out CallParticipantDetailInfo Participant);

    bool GetPendingCallInfo([In] int Idx, out string PeerJid, out string CallId);

    bool GetHeldCallInfo([In] int Idx, out string PeerJid, out string CallId);
  }
}
