// Decompiled with JetBrains decompiler
// Type: WhatsApp.QrPersistentAction
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace WhatsApp
{
  public class QrPersistentAction
  {
    public static QrPersistentAction Instance = new QrPersistentAction();

    private FunXMPP.Connection Connection => AppState.GetConnection();

    public bool ShouldAttemptQrPersistentAction
    {
      get => AppState.GetConnection() == null || AppState.GetConnection().EventHandler.Qr.Active;
    }

    public void OnQrError(int code)
    {
      Log.WriteLineDebug("   WebClient > OnQrError (Retry) - Code: {0}", (object) code);
    }

    public void NotifyMessage(Message m, QrMessageForwardType type, string modifyTag = null)
    {
      if (!this.ShouldAttemptQrPersistentAction)
        return;
      Log.WriteLineDebug("   WebClient > NotifyMessage - Jid: {0}, KeyId: {1}", (object) m.KeyRemoteJid, (object) m.KeyId);
      QrPersistentAction.PersistentData p = new QrPersistentAction.PersistentData()
      {
        Type = new int?(0),
        KeyRemoteJid = m.KeyRemoteJid,
        KeyFromMe = new bool?(m.KeyFromMe),
        KeyId = m.KeyId,
        IntParam = new int?((int) type),
        StringParam = modifyTag
      };
      if (modifyTag != null)
        m.ModifyTag = modifyTag;
      this.Schedule(p, (Action<Action>) (onComplete => this.Connection.SendQrMessages((IEnumerable<Message>) new Message[1]
      {
        m
      }, type, onComplete)), false);
    }

    private void ProcessMessage(QrPersistentAction.PersistentData d, Action onComplete)
    {
      Message message = d.GetMessage();
      if (message == null)
      {
        onComplete();
      }
      else
      {
        if (d.StringParam != null)
          message.ModifyTag = d.StringParam;
        Log.WriteLineDebug("   WebClient > ProcessMessage (Retry) - Jid: {0}, KeyId: {1}", (object) message.KeyRemoteJid, (object) message.KeyId);
        this.Connection.SendQrMessages((IEnumerable<Message>) new Message[1]
        {
          message
        }, (QrMessageForwardType) d.IntParam.Value, onComplete);
      }
    }

    public void NotifyStatus(FunXMPP.FMessage.Key key, FunXMPP.FMessage.Status status)
    {
      if (!this.ShouldAttemptQrPersistentAction)
        return;
      this.Schedule(new QrPersistentAction.PersistentData()
      {
        Type = new int?(1),
        KeyRemoteJid = key.remote_jid,
        KeyFromMe = new bool?(key.from_me),
        KeyId = key.id,
        IntParam = new int?((int) status)
      }, (Action<Action>) (onComplete => this.Connection.SendQrReceived(key, status, onComplete, new Action<int>(this.OnQrError))));
    }

    private void ProcessStatus(QrPersistentAction.PersistentData d, Action onComplete)
    {
      FunXMPP.FMessage.Key messageKey = d.GetMessageKey();
      FunXMPP.FMessage.Status status = (FunXMPP.FMessage.Status) d.IntParam.Value;
      Log.WriteLineDebug("   WebClient > ProcessStatus (Retry) - Jid: {0}, KeyId: {1}, Status: {2}", (object) messageKey.remote_jid, (object) messageKey.id, (object) status);
      this.Connection.SendQrReceived(messageKey, status, onComplete, new Action<int>(this.OnQrError));
    }

    public void NotifyChatStatus(
      string jid,
      FunXMPP.ChatStatusForwardAction action,
      DateTime? timeParam = null)
    {
      if (!this.ShouldAttemptQrPersistentAction)
        return;
      if (jid == null)
        jid = "s.whatsapp.net";
      this.Schedule(new QrPersistentAction.PersistentData()
      {
        Type = new int?(2),
        KeyRemoteJid = jid,
        IntParam = new int?((int) action),
        LongParam = !timeParam.HasValue ? new long?() : new long?(timeParam.Value.ToUnixTime())
      }, (Action<Action>) (onComplete => this.Connection.SendQrChatStatus(jid, action, timeParam, onComplete, new Action<int>(this.OnQrError))));
    }

    private void ProcessChatStatus(QrPersistentAction.PersistentData d, Action onComplete)
    {
      string keyRemoteJid = d.KeyRemoteJid;
      FunXMPP.ChatStatusForwardAction action = (FunXMPP.ChatStatusForwardAction) d.IntParam.Value;
      DateTime? actionTime = new DateTime?();
      if (d.LongParam.HasValue)
        actionTime = new DateTime?(DateTimeUtils.FromUnixTime(d.LongParam.Value));
      Log.WriteLineDebug("   WebClient > ProcessChatStatus (Retry) - Jid: {0}, Action: {1}", (object) keyRemoteJid, (object) action);
      this.Connection.SendQrChatStatus(keyRemoteJid, action, actionTime, onComplete, new Action<int>(this.OnQrError));
    }

    public void NotifyContactChange(FunXMPP.ContactResponse c)
    {
      if (!this.ShouldAttemptQrPersistentAction)
        return;
      QrPersistentAction.PersistentData p = new QrPersistentAction.PersistentData();
      p.Type = new int?(3);
      p.KeyRemoteJid = c.Jid;
      p.StringParam = c.ShortName;
      p.StringParam1 = c.DisplayName;
      p.StringParam2 = c.Notify;
      p.StringParam3 = c.VName;
      p.IntParam = new int?(c.Checksum);
      p.BoolParam = c.StatusMute.HasValue && c.StatusMute.Value;
      int? verify = c.Verify;
      p.LongParam = verify.HasValue ? new long?((long) verify.GetValueOrDefault()) : new long?();
      p.BoolParam1 = !c.Checkmark.HasValue || c.Checkmark.Value;
      p.BoolParam2 = c.IsEnterprise.HasValue && c.IsEnterprise.Value;
      this.Schedule(p, (Action<Action>) (onComplete => this.Connection.SendQrContact(c, onComplete, new Action<int>(this.OnQrError))));
    }

    private void ProcessContactChange(QrPersistentAction.PersistentData d, Action onComplete)
    {
      FunXMPP.ContactResponse contactResponse = new FunXMPP.ContactResponse();
      contactResponse.Jid = d.KeyRemoteJid;
      contactResponse.ShortName = d.StringParam;
      contactResponse.DisplayName = d.StringParam1;
      contactResponse.Notify = d.StringParam2;
      contactResponse.VName = d.StringParam3;
      contactResponse.Checksum = d.IntParam.GetValueOrDefault();
      contactResponse.StatusMute = new bool?(d.BoolParam);
      long? longParam = d.LongParam;
      contactResponse.Verify = longParam.HasValue ? new int?((int) longParam.GetValueOrDefault()) : new int?();
      contactResponse.Checkmark = new bool?(d.BoolParam1);
      contactResponse.IsEnterprise = new bool?(d.BoolParam2);
      FunXMPP.ContactResponse c = contactResponse;
      Log.WriteLineDebug("   WebClient > ProcessContactChange (Retry) - Jid: {0}", (object) c.Jid);
      this.Connection.SendQrContact(c, onComplete, new Action<int>(this.OnQrError));
    }

    public void NotifyDelete(Message m, int newModTag)
    {
      if (!this.ShouldAttemptQrPersistentAction)
        return;
      QrPersistentAction.PersistentData p = new QrPersistentAction.PersistentData()
      {
        Type = new int?(4),
        KeyRemoteJid = m.KeyRemoteJid,
        KeyFromMe = new bool?(m.KeyFromMe),
        KeyId = m.KeyId,
        IntParam = new int?(newModTag)
      };
      this.Schedule(p, (Action<Action>) (onComplete => this.Connection.SendQrDeleteMessage(p.GetMessageKey(), newModTag, onComplete)));
    }

    private void ProcessDelete(QrPersistentAction.PersistentData d, Action onComplete)
    {
      FunXMPP.FMessage.Key messageKey = d.GetMessageKey();
      int modifyTag = d.IntParam.Value;
      Log.WriteLineDebug("   WebClient > ProcessDelete (Retry) - Jid: {0}, KeyId: {1}, ModifyTag: {2}", (object) messageKey.remote_jid, (object) messageKey.id, (object) modifyTag);
      this.Connection.SendQrDeleteMessage(messageKey, modifyTag, onComplete);
    }

    public void NotifyRevoke(Message m)
    {
      if (!this.ShouldAttemptQrPersistentAction)
        return;
      string revokedMsgId = MessageProperties.GetForMessage(m)?.CommonPropertiesField?.RevokedMsgId;
      Log.WriteLineDebug("   WebClient > NotifyRevoke - Jid: {0}, revokeId: {1}, id: {2}, fromMe: {3}", (object) m.KeyRemoteJid, (object) revokedMsgId, (object) m.KeyId, (object) m.KeyFromMe);
      QrPersistentAction.PersistentData p = new QrPersistentAction.PersistentData()
      {
        Type = new int?(16),
        KeyRemoteJid = m.KeyRemoteJid,
        KeyFromMe = new bool?(m.KeyFromMe),
        KeyId = m.KeyId,
        StringParam = revokedMsgId,
        StringParam1 = m.GetSenderJid(),
        LongParam = new long?((m.FunTimestamp ?? DateTime.UtcNow).ToUnixTime())
      };
      this.Schedule(p, (Action<Action>) (onComplete => this.Connection.SendQrRevokeMessage(p.GetMessageKey(), p.StringParam, p.StringParam1, p.LongParam, onComplete)));
    }

    public void ProcessRevoke(QrPersistentAction.PersistentData d, Action onComplete)
    {
      Log.WriteLineDebug("   WebClient > NotifyRevoke (Retry) - Jid: {0}, revokeId: {1}, id: {2}, fromMe: {3}, senderJid: {4}", (object) d.KeyRemoteJid, (object) d.StringParam, (object) d.KeyId, (object) d.KeyFromMe, (object) d.StringParam1);
      this.Connection.SendQrRevokeMessage(d.GetMessageKey(), d.StringParam, d.StringParam1, d.LongParam, onComplete);
    }

    public void NotifyStarred(Message m, int newModTag)
    {
      if (!this.ShouldAttemptQrPersistentAction)
        return;
      QrPersistentAction.PersistentData p = new QrPersistentAction.PersistentData()
      {
        Type = new int?(11),
        KeyRemoteJid = m.KeyRemoteJid,
        KeyFromMe = new bool?(m.KeyFromMe),
        KeyId = m.KeyId,
        IntParam = new int?(newModTag),
        BoolParam = m.IsStarred
      };
      this.Schedule(p, (Action<Action>) (onComplete => this.Connection.SendQrStarredMessage(p.GetMessageKey(), m.IsStarred, newModTag, onComplete)));
    }

    private void ProcessStarred(QrPersistentAction.PersistentData d, Action onComplete)
    {
      FunXMPP.FMessage.Key messageKey = d.GetMessageKey();
      int modifyTag = d.IntParam.Value;
      Log.WriteLineDebug("   WebClient > ProcessStarred (Retry) - Jid: {0}, KeyId: {1}, ModifyTag: {2}", (object) messageKey.remote_jid, (object) messageKey.id, (object) modifyTag);
      this.Connection.SendQrStarredMessage(messageKey, d.BoolParam, modifyTag, onComplete);
    }

    public void NotifyUnStarAll(string jid, int modTag)
    {
      if (!this.ShouldAttemptQrPersistentAction)
        return;
      this.Schedule(new QrPersistentAction.PersistentData()
      {
        Type = new int?(12),
        KeyRemoteJid = jid,
        IntParam = new int?(modTag)
      }, (Action<Action>) (onComplete => this.Connection.SendQrUnStarAll(jid, modTag, onComplete)));
    }

    private void ProcessUnStarAll(QrPersistentAction.PersistentData d, Action onComplete)
    {
      Log.WriteLineDebug("   WebClient > ProcessUnStarAll (Retry) - Jid: {0}, ModifyTag: {1}", (object) d.KeyRemoteJid, (object) d.IntParam);
      this.Connection.SendQrUnStarAll(d.KeyRemoteJid, d.IntParam.Value, onComplete);
    }

    public void NotifyRead(string jid, bool read)
    {
      if (!this.ShouldAttemptQrPersistentAction)
        return;
      this.Schedule(new QrPersistentAction.PersistentData()
      {
        Type = new int?(5),
        KeyRemoteJid = jid,
        BoolParam = read
      }, (Action<Action>) (onComplete => this.Connection.SendQrRead(jid, read, onComplete, new Action<int>(this.OnQrError))));
    }

    public void ProcessRead(QrPersistentAction.PersistentData d, Action onComplete)
    {
      Log.WriteLineDebug("   WebClient > ProcessRead (Retry) - Jid: {0}", (object) d.KeyRemoteJid);
      this.Connection.SendQrRead(d.KeyRemoteJid, d.BoolParam, onComplete);
    }

    public void NotifySeen(string jid, string msgId, bool owner, string senderJid)
    {
      if (!this.ShouldAttemptQrPersistentAction)
        return;
      this.Schedule(new QrPersistentAction.PersistentData()
      {
        Type = new int?(15),
        KeyRemoteJid = jid,
        KeyId = msgId,
        KeyFromMe = new bool?(owner),
        StringParam = senderJid
      }, (Action<Action>) (onComplete => this.Connection.SendQrSeen(jid, msgId, owner, senderJid, onComplete, new Action<int>(this.OnQrError))));
    }

    public void ProcessSeen(QrPersistentAction.PersistentData d, Action onComplete)
    {
      Log.WriteLineDebug("   WebClient > ProcessSeen (Retry) - Jid: {0}, senderJid: {1}", (object) d.KeyRemoteJid, (object) d.StringParam);
      this.Connection.SendQrSeen(d.KeyRemoteJid, d.KeyId, d.KeyFromMe.Value, d.StringParam, onComplete);
    }

    public void NotifyReupload<T>(Message m, string id, IObservable<T> uploadObs)
    {
      if (!this.ShouldAttemptQrPersistentAction)
        return;
      this.Schedule(new QrPersistentAction.PersistentData()
      {
        Type = new int?(6),
        KeyRemoteJid = m.KeyRemoteJid,
        KeyFromMe = new bool?(m.KeyFromMe),
        KeyId = m.KeyId,
        StringParam = id
      }, (Action<Action>) (onComplete => uploadObs.Subscribe<T>((Action<T>) (_ => onComplete()))));
    }

    public void ProcessReupload(QrPersistentAction.PersistentData d, Action onComplete)
    {
      if (!(this.Connection.EventHandler.Qr is QrListener qr))
      {
        onComplete();
      }
      else
      {
        Message message = d.GetMessage();
        Log.WriteLineDebug("   WebClient > ProcessReupload (Retry) - Jid: {0}, KeyId: {1}", (object) message.KeyRemoteJid, (object) message.KeyId);
        qr.OnReUpload(message, d.StringParam, onComplete);
      }
    }

    public void NotifyMedia(string id, string url, string mediaKey, int? code)
    {
      if (!this.ShouldAttemptQrPersistentAction)
        return;
      this.Schedule(new QrPersistentAction.PersistentData()
      {
        Type = new int?(7),
        KeyId = id,
        StringParam = url,
        IntParam = code,
        KeyRemoteJid = mediaKey
      }, (Action<Action>) (onComplete => this.Connection.SendQrMediaResponse(id, url, mediaKey, code, onComplete)));
    }

    private void ProcessMedia(QrPersistentAction.PersistentData d, Action onComplete)
    {
      string keyId = d.KeyId;
      string stringParam = d.StringParam;
      int? intParam = d.IntParam;
      string keyRemoteJid = d.KeyRemoteJid;
      Log.WriteLineDebug("   WebClient > ProcessMedia (Retry) - KeyId: {0}, Url: {1}, Code: {2}", (object) keyId, (object) stringParam, (object) intParam);
      this.Connection.SendQrMediaResponse(keyId, stringParam, keyRemoteJid, intParam, onComplete);
    }

    public void NotifyPreemptiveChats()
    {
      if (!this.ShouldAttemptQrPersistentAction)
        return;
      this.Schedule(new QrPersistentAction.PersistentData()
      {
        Type = new int?(8)
      }, (Action<Action>) (onComplete => this.Connection.SendQrPreemptiveChats(onComplete)));
    }

    private void ProcessPreemptiveChats(QrPersistentAction.PersistentData d, Action onComplete)
    {
      Log.WriteLineDebug("   WebClient > ProcessPreemptiveChats (Retry)");
      this.Connection.SendQrPreemptiveChats(onComplete);
    }

    public void NotifyPreemptiveContacts()
    {
      if (!this.ShouldAttemptQrPersistentAction)
        return;
      this.Schedule(new QrPersistentAction.PersistentData()
      {
        Type = new int?(9)
      }, (Action<Action>) (onComplete => this.Connection.SendQrPreemptiveContacts(onComplete)));
    }

    private void ProcessPreemptiveContacts(QrPersistentAction.PersistentData d, Action onComplete)
    {
      Log.WriteLineDebug("   WebClient > ProcessPreemptiveContacts (Retry)");
      this.Connection.SendQrPreemptiveContacts(onComplete);
    }

    public void NotifyProfilePhotoRequest(string id, string jid, bool large)
    {
      if (!this.ShouldAttemptQrPersistentAction)
        return;
      this.Schedule(new QrPersistentAction.PersistentData()
      {
        Type = new int?(10),
        KeyId = id,
        KeyRemoteJid = jid,
        BoolParam = large
      }, (Action<Action>) (onComplete => this.Connection.SendQrProfilePictureResponse(id, jid, large, onComplete)));
    }

    private void ProcessProfilePhotoRequest(QrPersistentAction.PersistentData d, Action onComplete)
    {
      string keyId = d.KeyId;
      string keyRemoteJid = d.KeyRemoteJid;
      bool boolParam = d.BoolParam;
      Log.WriteLineDebug("   WebClient > ProcessProfilePhotoRequest (Retry) - Id: {0}, Jid: {1}, Large: {2}", (object) keyId, (object) keyRemoteJid, (object) boolParam);
      this.Connection.SendQrProfilePictureResponse(keyId, keyRemoteJid, boolParam, onComplete);
    }

    public void NotifyIdentityChanged(string jid)
    {
      if (!this.ShouldAttemptQrPersistentAction)
        return;
      this.Schedule(new QrPersistentAction.PersistentData()
      {
        Type = new int?(13),
        KeyRemoteJid = jid
      }, (Action<Action>) (onComplete => this.Connection.SendQrIdentityChange(jid, onComplete, new Action<int>(this.OnQrError))));
    }

    public void ProcessIdentityChanged(QrPersistentAction.PersistentData d, Action onComplete)
    {
      Log.WriteLineDebug("   WebClient > ProcessIdentityChanged (Retry) - Jid: {0}", (object) d.KeyRemoteJid);
      this.Connection.SendQrIdentityChange(d.KeyRemoteJid, onComplete);
    }

    public void NotifyFrequentContacts()
    {
      if (!this.ShouldAttemptQrPersistentAction)
        return;
      this.Schedule(new QrPersistentAction.PersistentData()
      {
        Type = new int?(14)
      }, (Action<Action>) (onComplete => this.Connection.SendQrFrequentContacts(onComplete)));
    }

    private void ProcessFrequentContacts(QrPersistentAction.PersistentData d, Action onComplete)
    {
      Log.WriteLineDebug("   WebClient > ProcessFrequentContacts (Retry)");
      this.Connection.SendQrFrequentContacts(onComplete);
    }

    public void Schedule(
      QrPersistentAction.PersistentData p,
      Action<Action> firstAttempt,
      bool async = true)
    {
      bool shouldAttempt = true;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        if (AppState.GetConnection() != null)
          return;
        p.Ref = (string) null;
        PersistentAction a = PersistentAction.Qr(p);
        db.StorePersistentAction(a);
        db.SubmitChanges();
        shouldAttempt = false;
      }));
      if (!shouldAttempt)
        return;
      QrSession ses = QrListener.Instance.Session;
      if (ses == null || !ses.Active)
        return;
      ses.SynchronizeWithMessageContext((MessagesContext.MessagesCallback) (db =>
      {
        if (!ses.Active)
          return;
        p.Ref = Convert.ToBase64String(ses.Id);
        PersistentAction dbObj = PersistentAction.Qr(p);
        db.StorePersistentAction(dbObj);
        db.SubmitChanges();
        if (async)
          AppState.Worker.Enqueue((Action) (() => this.FirstAttemptAsync(firstAttempt, ses.Id, dbObj)));
        else
          firstAttempt((Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (db2 =>
          {
            db2.DeletePersistentActionOnSubmit(dbObj);
            db2.SubmitChanges();
          }))));
      }));
    }

    private void FirstAttemptAsync(
      Action<Action> innerMethod,
      byte[] sessionId,
      PersistentAction dbObj)
    {
      Action onComplete = (Action) (() => MessagesContext.RunRecursive((MessagesContext.MessagesCallback) (db =>
      {
        db.DeletePersistentActionOnSubmit(dbObj);
        db.SubmitChanges();
      })));
      QrSession session = QrListener.Instance.Session;
      if (session == null || !session.Active)
        onComplete();
      else
        session.SynchronizeWithMessageContext((MessagesContext.MessagesCallback) (db =>
        {
          if (!sessionId.IsEqualBytes(session.Id))
            onComplete();
          else
            innerMethod(onComplete);
        }));
    }

    private void Process(QrPersistentAction.PersistentData d, Action onComplete)
    {
      Action<QrPersistentAction.PersistentData, Action> action = (Action<QrPersistentAction.PersistentData, Action>) null;
      switch (d.Type.Value)
      {
        case 0:
          action = new Action<QrPersistentAction.PersistentData, Action>(this.ProcessMessage);
          break;
        case 1:
          action = new Action<QrPersistentAction.PersistentData, Action>(this.ProcessStatus);
          break;
        case 2:
          action = new Action<QrPersistentAction.PersistentData, Action>(this.ProcessChatStatus);
          break;
        case 3:
          action = new Action<QrPersistentAction.PersistentData, Action>(this.ProcessContactChange);
          break;
        case 4:
          action = new Action<QrPersistentAction.PersistentData, Action>(this.ProcessDelete);
          break;
        case 5:
          action = new Action<QrPersistentAction.PersistentData, Action>(this.ProcessRead);
          break;
        case 6:
          action = new Action<QrPersistentAction.PersistentData, Action>(this.ProcessReupload);
          break;
        case 7:
          action = new Action<QrPersistentAction.PersistentData, Action>(this.ProcessMedia);
          break;
        case 8:
          action = new Action<QrPersistentAction.PersistentData, Action>(this.ProcessPreemptiveChats);
          break;
        case 9:
          action = new Action<QrPersistentAction.PersistentData, Action>(this.ProcessPreemptiveContacts);
          break;
        case 10:
          action = new Action<QrPersistentAction.PersistentData, Action>(this.ProcessProfilePhotoRequest);
          break;
        case 11:
          action = new Action<QrPersistentAction.PersistentData, Action>(this.ProcessStarred);
          break;
        case 12:
          action = new Action<QrPersistentAction.PersistentData, Action>(this.ProcessUnStarAll);
          break;
        case 13:
          action = new Action<QrPersistentAction.PersistentData, Action>(this.ProcessIdentityChanged);
          break;
        case 15:
          action = new Action<QrPersistentAction.PersistentData, Action>(this.ProcessSeen);
          break;
        case 16:
          action = new Action<QrPersistentAction.PersistentData, Action>(this.ProcessRevoke);
          break;
      }
      if (action != null)
        action(d, onComplete);
      else
        onComplete();
    }

    public void Process(byte[] blob, Action onComplete)
    {
      QrSession ses = QrListener.Instance.Session;
      if (ses == null || !ses.Active)
      {
        onComplete();
      }
      else
      {
        QrPersistentAction.PersistentData d = QrPersistentAction.PersistentData.Deserialize(blob);
        ses.SynchronizeWithMessageContext((MessagesContext.MessagesCallback) (db =>
        {
          if (d.Ref != null && Convert.ToBase64String(ses.Id) != d.Ref)
            onComplete();
          else
            this.Process(d, onComplete);
        }));
      }
    }

    [DataContract]
    public class PersistentData
    {
      private static DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof (QrPersistentAction.PersistentData));

      public Message GetMessage()
      {
        return MessagesContext.Select<Message>((Func<MessagesContext, Message>) (db => db.GetMessage(this.KeyRemoteJid, this.KeyId, this.KeyFromMe.Value)));
      }

      public FunXMPP.FMessage.Key GetMessageKey()
      {
        return new FunXMPP.FMessage.Key(this.KeyRemoteJid, this.KeyFromMe.Value, this.KeyId);
      }

      [DataMember]
      public int? Type { get; set; }

      [DataMember]
      public string Ref { get; set; }

      [DataMember]
      public string KeyRemoteJid { get; set; }

      [DataMember]
      public string KeyId { get; set; }

      [DataMember]
      public bool? KeyFromMe { get; set; }

      [DataMember]
      public int? IntParam { get; set; }

      [DataMember]
      public long? LongParam { get; set; }

      [DataMember]
      public string StringParam { get; set; }

      [DataMember]
      public string StringParam1 { get; set; }

      [DataMember]
      public string StringParam2 { get; set; }

      [DataMember]
      public string StringParam3 { get; set; }

      [DataMember]
      public bool BoolParam { get; set; }

      [DataMember]
      public bool BoolParam1 { get; set; }

      [DataMember]
      public bool BoolParam2 { get; set; }

      public byte[] Serialize()
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          QrPersistentAction.PersistentData.serializer.WriteObject((Stream) memoryStream, (object) this);
          return memoryStream.ToArray();
        }
      }

      public static QrPersistentAction.PersistentData Deserialize(byte[] blob)
      {
        using (MemoryStream memoryStream = new MemoryStream(blob, false))
          return (QrPersistentAction.PersistentData) QrPersistentAction.PersistentData.serializer.ReadObject((Stream) memoryStream);
      }
    }

    public enum Types
    {
      Message,
      Status,
      ChatStatus,
      ContactChange,
      Delete,
      Read,
      Reupload,
      MediaResponse,
      PreemptiveChats,
      PreemptiveContacts,
      ProfilePhotoResponse,
      Starred,
      UnStarAll,
      IdentityChanged,
      FrequentContacts,
      Seen,
      Revoke,
    }
  }
}
