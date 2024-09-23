// Decompiled with JetBrains decompiler
// Type: WhatsApp.EmailChatHistory
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;

#nullable disable
namespace WhatsApp
{
  public abstract class EmailChatHistory
  {
    protected const string LogHeader = "emailchat";
    protected string jid;

    public static IObservable<Unit> Create(string jid)
    {
      return !JidHelper.IsUserJid(jid) && !JidHelper.IsGroupJid(jid) ? Observable.Empty<Unit>() : new EmailChatHistoryUWP(jid).Send().Select<Unit, Unit>((Func<Unit, Unit>) (a => new Unit()));
    }

    protected EmailChatHistory(string jid) => this.jid = jid;

    protected abstract IObservable<Unit> Send();

    protected static string GetNameByJid(
      string jid,
      ref Dictionary<string, string> chatParticipants)
    {
      string name = (string) null;
      if (jid != null && !chatParticipants.TryGetValue(jid, out name))
      {
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          UserStatus userStatus = db.GetUserStatus(jid);
          if (userStatus != null)
            name = userStatus.GetDisplayName();
          else
            name = AppState.FormatPhoneNumber(jid.Substring(0, jid.IndexOf('@')));
        }));
        chatParticipants[jid] = name;
      }
      return name;
    }

    protected static string GenerateEmailSubject(
      Conversation convo,
      ref Dictionary<string, string> chatParticipants)
    {
      return !convo.Jid.IsGroupJid() ? string.Format(AppResources.EmailSubjectDual, (object) EmailChatHistory.GetNameByJid(convo.Jid, ref chatParticipants)) : string.Format(AppResources.EmailSubjectGroup, (object) convo.GroupSubject);
    }

    protected static string GetEmailLine(Message m, string senderName, bool includeDate)
    {
      string str = (string) null;
      switch (m.MediaWaType)
      {
        case FunXMPP.FMessage.Type.Undefined:
        case FunXMPP.FMessage.Type.ExtendedText:
          str = m.Data;
          break;
        case FunXMPP.FMessage.Type.Image:
        case FunXMPP.FMessage.Type.Gif:
        case FunXMPP.FMessage.Type.Sticker:
          str = string.Format("<{0}>{1}", (object) AppResources.OmittedImage, m.MediaCaption == null ? (object) "" : (object) (" " + m.MediaCaption));
          break;
        case FunXMPP.FMessage.Type.Audio:
          str = string.Format("<{0}>", m.IsPtt() ? (object) AppResources.OmittedVoiceMessage : (object) AppResources.OmittedAudio);
          break;
        case FunXMPP.FMessage.Type.Video:
          str = string.Format("<{0}>{1}", (object) AppResources.OmittedVideo, m.MediaCaption == null ? (object) "" : (object) (" " + m.MediaCaption));
          break;
        case FunXMPP.FMessage.Type.Contact:
          str = string.Format("<{0}>", (object) AppResources.OmittedContact);
          break;
        case FunXMPP.FMessage.Type.Location:
          str = !string.IsNullOrEmpty(m.LocationDetails) ? string.Format("{0}: {1}", (object) m.LocationDetails, (object) m.LocalFileUri) : string.Format("location: https://maps.google.com/?q={0},{1}", (object) m.Latitude, (object) m.Longitude);
          break;
        case FunXMPP.FMessage.Type.System:
          return m.GetSystemMessage() + "\n";
        case FunXMPP.FMessage.Type.Document:
          str = string.Format("<{0}>", (object) AppResources.OmittedDocument);
          break;
        case FunXMPP.FMessage.Type.LiveLocation:
          if (m.InternalProperties.LiveLocationPropertiesField != null)
          {
            str = string.Format("live location: {0}", (object) m.InternalProperties.LiveLocationPropertiesField.Caption);
            break;
          }
          break;
        default:
          str = "";
          break;
      }
      string emailLine = (string) null;
      DateTime? localTimestamp = m.LocalTimestamp;
      if (localTimestamp.HasValue)
      {
        if (includeDate)
          emailLine = string.Format("{0}: {1}: {2}\n", (object) localTimestamp.Value, (object) senderName, (object) str);
      }
      else
        emailLine = string.Format("{0}: {1}\n", (object) senderName, (object) str);
      return emailLine;
    }
  }
}
