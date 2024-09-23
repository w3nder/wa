// Decompiled with JetBrains decompiler
// Type: WhatsApp.VoipExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;
using WhatsAppNative;


namespace WhatsApp
{
  public static class VoipExtensions
  {
    public static bool GetCallInfo(
      this IVoip voip,
      out string callId,
      out string peerJid,
      out CallInfoStruct info)
    {
      callId = peerJid = (string) null;
      if (voip is ICallInfo callInfo)
      {
        try
        {
          callInfo.GetCallInfo(out callId, out peerJid, out info);
          return true;
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "get call info");
        }
      }
      info = new CallInfoStruct();
      return false;
    }

    public static bool GetCallMetadata(this IVoip voip, out string callId, out string peerJid)
    {
      return voip.GetCallInfo(out callId, out peerJid, out CallInfoStruct _);
    }

    public static CallInfoStruct? GetCallInfo(this IVoip voip)
    {
      CallInfoStruct info;
      return !voip.GetCallInfo(out string _, out string _, out info) ? new CallInfoStruct?() : new CallInfoStruct?(info);
    }

    public static CallInfoStruct? GetCallInfo(this ICallInfo info)
    {
      CallInfoStruct InfoStruct;
      try
      {
        info.GetCallInfo(out string _, out string _, out InfoStruct);
      }
      catch
      {
        return new CallInfoStruct?();
      }
      return new CallInfoStruct?(InfoStruct);
    }

    public static CallParticipantDetail? GetFirstPeer(this IVoip voip)
    {
      return voip is ICallInfo info ? info.GetFirstPeer() : new CallParticipantDetail?();
    }

    public static CallParticipantDetail? GetFirstPeer(this ICallInfo info)
    {
      return info.GetSelfOrFirstPeer(false);
    }

    public static CallParticipantDetail? GetSelf(this IVoip voip)
    {
      return voip is ICallInfo info ? info.GetSelf() : new CallParticipantDetail?();
    }

    public static CallParticipantDetail? GetSelf(this ICallInfo info)
    {
      return info.GetSelfOrFirstPeer(true);
    }

    public static CallParticipantDetail? GetSelfOrFirstPeer(this ICallInfo info, bool self)
    {
      try
      {
        CallParticipantDetailInfo Participant;
        string ParticipantJid;
        for (int ParticipantIdx = 0; info.GetParticipantInfo(ParticipantIdx, out ParticipantJid, out Participant); ++ParticipantIdx)
        {
          if (Participant.IsSelf == self)
            return new CallParticipantDetail?(VoipExtensions.ConvertParticipantDetailInfo(ParticipantJid, Participant));
        }
      }
      catch
      {
      }
      return new CallParticipantDetail?();
    }

    private static CallParticipantDetail ConvertParticipantDetailInfo(
      string jid,
      CallParticipantDetailInfo info)
    {
      CallParticipantDetail participantDetail;
      participantDetail.Jid = jid;
      participantDetail.State = info.State;
      participantDetail.IsSelf = info.IsSelf;
      participantDetail.IsMuted = info.IsMuted;
      participantDetail.IsInterrupted = info.IsInterrupted;
      participantDetail.VideoRenderStarted = info.VideoRenderStarted;
      participantDetail.VideoDecodeStarted = info.VideoDecodeStarted;
      participantDetail.VideoDecodePaused = info.VideoDecodePaused;
      participantDetail.VideoStreamState = info.VideoStreamState;
      participantDetail.VideoWidth = info.VideoWidth;
      participantDetail.VideoHeight = info.VideoHeight;
      participantDetail.VideoOrientation = info.VideoOrientation;
      participantDetail.IsAudioVideoSwitchEnabled = info.IsAudioVideoSwitchEnabled;
      participantDetail.IsAudioVideoSwitchSupported = info.IsAudioVideoSwitchSupported;
      participantDetail.IsInvitedBySelf = info.IsInvitedBySelf;
      participantDetail.RxConnecting = info.RxConnecting;
      participantDetail.RxTimedOut = info.RxTimedOut;
      participantDetail.RxAudioPacketCount = info.RxAudioPacketCount;
      return participantDetail;
    }

    public static List<CallParticipantDetail> GetCallPeers(this IVoip voip)
    {
      ICallInfo info = voip as ICallInfo;
      List<CallParticipantDetail> callPeers = new List<CallParticipantDetail>();
      if (info != null)
        callPeers = info.GetCallPeers();
      return callPeers;
    }

    public static List<CallParticipantDetail> GetCallPeers(this ICallInfo info)
    {
      List<CallParticipantDetail> callPeers = new List<CallParticipantDetail>();
      try
      {
        CallParticipantDetailInfo Participant;
        string ParticipantJid;
        for (int ParticipantIdx = 0; info.GetParticipantInfo(ParticipantIdx, out ParticipantJid, out Participant); ++ParticipantIdx)
        {
          if (!Participant.IsSelf)
            callPeers.Add(VoipExtensions.ConvertParticipantDetailInfo(ParticipantJid, Participant));
        }
      }
      catch
      {
      }
      return callPeers;
    }

    public static string GetCallParticipantsDisplayName(this IVoip voip, bool preferFirstNames = false)
    {
      try
      {
        return Utils.CommaSeparate(voip.GetCallPeers().Select<CallParticipantDetail, UserStatus>((Func<CallParticipantDetail, UserStatus>) (p => UserCache.Get(p.Jid, true))).Select<UserStatus, string>((Func<UserStatus, string>) (u => u.GetDisplayName(preferFirstNames))));
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "get call participants display name");
      }
      return string.Empty;
    }

    public static VoipExtensions.CallLogEntry? GetCallLogEntry(this IVoip voip)
    {
      try
      {
        List<VoipExtensions.CallLogEntryParticipant> entryParticipantList = new List<VoipExtensions.CallLogEntryParticipant>();
        VoipExtensions.CallLogEntry callLogEntry;
        callLogEntry.res = CallLogResult.Invalid;
        int Idx;
        string Jid;
        CallLogResult Result;
        for (Idx = 0; voip.GetNextCallLogEntry(Idx, out Jid, out Result); ++Idx)
        {
          if (Jid == null || Jid.Length == 0)
            callLogEntry.res = Result;
          else
            entryParticipantList.Add(new VoipExtensions.CallLogEntryParticipant()
            {
              jid = Jid,
              res = Result
            });
        }
        callLogEntry.participants = entryParticipantList.ToArray();
        if (Idx > 0)
          return new VoipExtensions.CallLogEntry?(callLogEntry);
      }
      catch
      {
      }
      return new VoipExtensions.CallLogEntry?();
    }

    public struct CallLogEntryParticipant
    {
      public string jid;
      public CallLogResult res;
    }

    public struct CallLogEntry
    {
      public CallLogResult res;
      public VoipExtensions.CallLogEntryParticipant[] participants;
    }
  }
}
