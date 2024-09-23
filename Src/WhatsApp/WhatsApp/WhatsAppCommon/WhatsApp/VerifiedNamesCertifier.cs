// Decompiled with JetBrains decompiler
// Type: WhatsApp.VerifiedNamesCertifier
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using WhatsApp.ProtoBuf;
using WhatsApp.WaCollections;
using WhatsAppCommon;


namespace WhatsApp
{
  public static class VerifiedNamesCertifier
  {
    private static readonly string[] IssuersAllowList = new string[2]
    {
      "ent:wa",
      "smb:wa"
    };
    private static readonly string LogHdr = "VnCert";
    private static bool VerifiedLevelClbThrottled = false;

    public static void OnProfileNotification(
      string jid,
      BizProfileDetails bizProfile,
      Action onCompletion)
    {
      if (string.IsNullOrEmpty(bizProfile.Tag))
      {
        Log.l(VerifiedNamesCertifier.LogHdr, "ignoring notification of profile as tag is null {0} {1}", (object) jid, (object) bizProfile.Description);
      }
      else
      {
        Log.l(VerifiedNamesCertifier.LogHdr, "processing notification of profile {0} {1} {2}", (object) jid, (object) bizProfile.Tag, (object) bizProfile.Description);
        ContactsContext.Instance((Action<ContactsContext>) (cdb =>
        {
          UserStatusProperties forUserStatus = UserStatusProperties.GetForUserStatus(cdb.GetUserStatus(jid));
          if (!forUserStatus.UpdateBusinessUserProperties(bizProfile))
            return;
          forUserStatus.Save();
          cdb.SubmitChanges();
        }));
        onCompletion();
      }
    }

    public static void OnVerifiedLevelNotification(
      string senderJid,
      ulong? serial,
      string verified_level)
    {
      Log.l(VerifiedNamesCertifier.LogHdr, "processing notification of level change {0} {1} {2}", (object) senderJid, (object) serial, (object) verified_level);
      VerifiedLevel newLevel = VerifiedNamesCertifier.GetVerfiedLevelAsEnum(verified_level);
      bool needsCertificate = false;
      bool verifiedUserFlag = false;
      ContactsContext.Instance((Action<ContactsContext>) (cdb =>
      {
        UserStatus userStatus = cdb.GetUserStatus(senderJid, false);
        if (userStatus == null)
          return;
        VerifiedNameState verifiedNameState = userStatus.VerifiedName;
        switch (newLevel)
        {
          case VerifiedLevel.NotApplicable:
            verifiedNameState = VerifiedNameState.NotApplicable;
            newLevel = VerifiedLevel.NotApplicable;
            break;
          case VerifiedLevel.unknown:
          case VerifiedLevel.low:
          case VerifiedLevel.high:
            needsCertificate = !userStatus.IsVerified() || userStatus.VerifiedNameCertificateDetails == null;
            break;
        }
        userStatus.VerifiedName = verifiedNameState;
        userStatus.VerifiedLevel = newLevel;
        cdb.SubmitChanges();
        verifiedUserFlag = userStatus.IsVerified();
      }));
      if (!needsCertificate)
        return;
      Log.l(VerifiedNamesCertifier.LogHdr, "Profile notification cert req, verified user? {0}", (object) verifiedUserFlag);
      VerifiedNamesCertifier.ScheduleCertifyVerifiedUserAction(senderJid, new DateTime?());
    }

    public static void OnCertificateNotification(
      string senderJid,
      ulong? serial,
      string verified_level,
      byte[] serializedCertificate,
      Action onCompletion)
    {
      Log.l(VerifiedNamesCertifier.LogHdr, "processing notification of cert {0} {1} {2}", (object) senderJid, (object) serial, (object) verified_level);
      Action completeAction = (Action) (() =>
      {
        onCompletion();
        VerifiedNamesCertifier.OnCertificate(senderJid, serial, verified_level, serializedCertificate);
      });
      if (AppState.GetConnection().Encryption.GetPublicKey(senderJid, true) == null)
        AppState.GetConnection().Encryption.SendGetPreKey(senderJid, completeAction);
      else
        completeAction();
    }

    public static void RemoveUsersBusinessDetails(string jid)
    {
      Log.d(VerifiedNamesCertifier.LogHdr, "processing removal of business details for {0}", (object) jid);
      if (string.IsNullOrEmpty(jid))
        return;
      try
      {
        VerifiedNameState oldState = VerifiedNameState.NotApplicable;
        VerifiedLevel oldLevel = VerifiedLevel.NotApplicable;
        bool updateRequired = false;
        ContactsContext.Instance((Action<ContactsContext>) (cdb =>
        {
          UserStatus userStatus = cdb.GetUserStatus(jid, false);
          if (userStatus == null)
            return;
          oldState = userStatus.VerifiedName;
          oldLevel = userStatus.VerifiedLevel;
          updateRequired = userStatus.VerifiedName != VerifiedNameState.NotApplicable || userStatus.VerifiedLevel != VerifiedLevel.NotApplicable || userStatus.VerifiedNameCertificateDetails != null || UserStatusProperties.HasUserStatusProperties(userStatus) && UserStatusProperties.GetForUserStatus(userStatus)?.BusinessUserPropertiesField != null;
          if (!updateRequired)
            return;
          userStatus.VerifiedName = VerifiedNameState.NotApplicable;
          userStatus.VerifiedLevel = VerifiedLevel.NotApplicable;
          userStatus.VerifiedNameCertificateDetails = (VerifiedNameCertificate.Details) null;
          if (UserStatusProperties.HasUserStatusProperties(userStatus))
            UserStatusProperties.GetForUserStatus(userStatus).BusinessUserPropertiesField = (UserStatusProperties.BusinessUserProperties) null;
          cdb.SubmitChanges();
        }));
        if (!updateRequired)
          return;
        Log.l(VerifiedNamesCertifier.LogHdr, "Business details removed for user {0} were {1} {2} {3}", (object) jid, (object) oldState, (object) oldLevel, (object) updateRequired);
      }
      catch (Exception ex)
      {
        string context = "RemoveUsersBusinessDetails exception processing biz remove notification for " + jid;
        Log.LogException(ex, context);
      }
    }

    public static void OnVnameCheckNotification(
      string jid,
      string vNameToCheck,
      string from,
      Action onCompletion)
    {
      Log.d(VerifiedNamesCertifier.LogHdr, "received notification of vname check for {0}", (object) jid);
      WAThreadPool.QueueUserWorkItem((Action) (() =>
      {
        Log.l(VerifiedNamesCertifier.LogHdr, "processing notification of vname check for {0}", (object) jid);
        VnameCheckManager.OnVNameCheckRequest(jid, vNameToCheck, from);
      }));
      onCompletion();
    }

    public static void OnCertificate(
      string senderJid,
      ulong? serial,
      string verified_level,
      byte[] serializedCertificate)
    {
      Log.l(VerifiedNamesCertifier.LogHdr, "processing cert {0} {1} {2}", (object) senderJid, (object) verified_level, (object) (serializedCertificate != null));
      VerifiedNameState verifiedState = VerifiedNameState.NotApplicable;
      string verifiedName = (string) null;
      VerifiedNameCertificate.Details details = (VerifiedNameCertificate.Details) null;
      bool flag = serializedCertificate != null && serializedCertificate.Length != 0;
      byte[] signKey = (byte[]) null;
      if (flag)
      {
        signKey = AppState.GetConnection().Encryption.GetPublicKey(senderJid);
        if (signKey == null)
        {
          Log.l(VerifiedNamesCertifier.LogHdr, "Can't find identity details for {0}", (object) senderJid);
          flag = false;
        }
      }
      if (flag)
      {
        VerifiedNameCertificate verifiedNameCertificate = VerifiedNameCertificate.Deserialize(serializedCertificate);
        if (verifiedNameCertificate.Signature == null || !Curve22519Extensions.Verify(verifiedNameCertificate.DetailsField, verifiedNameCertificate.Signature, signKey))
        {
          Log.l(VerifiedNamesCertifier.LogHdr, "Invalid cert: sig null? {0}", (object) (verifiedNameCertificate.Signature == null));
          flag = false;
        }
        details = VerifiedNameCertificate.Details.Deserialize(verifiedNameCertificate.DetailsField);
        if (!((IEnumerable<string>) VerifiedNamesCertifier.IssuersAllowList).Select<string, string>((Func<string, string>) (w => w.ToLowerInvariant())).Contains<string>(details.Issuer.ToLowerInvariant()))
        {
          flag = false;
          if (Settings.IsWaAdmin && details.Issuer.ToLowerInvariant() == "ent-internal-python")
            flag = true;
        }
      }
      ulong? nullable;
      if (flag)
      {
        nullable = details.Expires;
        if (nullable.HasValue)
        {
          nullable = details.Expires;
          DateTime? dt;
          if (!FunXMPP.TryParseTimestamp((long) nullable.Value, out dt))
            flag = false;
          if (dt.HasValue && dt.Value < DateTime.Now)
            flag = false;
        }
      }
      if (flag)
      {
        if (serial.HasValue)
        {
          nullable = details.Serial;
          if (nullable.HasValue)
          {
            nullable = details.Serial;
            if ((long) nullable.Value == (long) serial.Value)
              goto label_22;
          }
          flag = false;
        }
        else
        {
          nullable = details.Serial;
          if (!nullable.HasValue)
            Log.l(VerifiedNamesCertifier.LogHdr, "Certificate supplied with no serial");
        }
      }
label_22:
      if (flag)
      {
        string lang;
        string locale;
        AppState.GetLangAndLocale(out lang, out locale);
        if (details.LocalizedNames != null)
        {
          foreach (LocalizedName localizedName in details.LocalizedNames)
          {
            if (localizedName.Lg == lang)
            {
              verifiedName = localizedName.VerifiedName;
              if (localizedName.Lc == locale)
                break;
            }
          }
        }
        if (verifiedName == null)
          verifiedName = details.VerifiedName;
        verifiedState = VerifiedNameState.Verified;
      }
      Log.l(VerifiedNamesCertifier.LogHdr, "cert processing {0}, {1}, {2}, {3}, {4}.", (object) senderJid, (object) flag, (object) verifiedName, (object) verifiedState, (object) verified_level);
      MessagesContext.Run((MessagesContext.MessagesCallback) (mdb =>
      {
        ContactsContext.Instance((Action<ContactsContext>) (cdb =>
        {
          UserStatus userStatus = cdb.GetUserStatus(senderJid);
          userStatus.VerifiedName = verifiedState;
          if (verifiedState == VerifiedNameState.Verified)
          {
            if (string.IsNullOrEmpty(userStatus.ContactName))
              userStatus.ContactName = verifiedName;
            userStatus.PushName = verifiedName;
            userStatus.VerifiedNameCertificateDetails = details;
            userStatus.VerifiedLevel = VerifiedNamesCertifier.GetVerfiedLevelAsEnum(verified_level);
          }
          else
          {
            userStatus.PushName = (string) null;
            userStatus.VerifiedNameCertificateDetails = (VerifiedNameCertificate.Details) null;
            userStatus.VerifiedLevel = VerifiedLevel.NotApplicable;
          }
          cdb.SubmitChanges();
        }));
        mdb.ReleasePendingMessages(senderJid);
      }));
    }

    public static void IdentityChangedForUser(string senderJid, bool immediate)
    {
      if (!JidHelper.IsUserJid(senderJid))
        return;
      MessagesContext.Run((MessagesContext.MessagesCallback) (mdb =>
      {
        bool pendingCertification = false;
        ContactsContext.Instance((Action<ContactsContext>) (cdb =>
        {
          UserStatus userStatus = cdb.GetUserStatus(senderJid);
          if (!userStatus.IsVerified())
            return;
          userStatus.VerifiedName = VerifiedNameState.PendingCertification;
          userStatus.VerifiedNameCertificateDetails = (VerifiedNameCertificate.Details) null;
          if (userStatus.VerifiedLevel == VerifiedLevel.high)
            userStatus.VerifiedLevel = VerifiedLevel.low;
          cdb.SubmitChanges();
          pendingCertification = true;
        }));
        if (!pendingCertification)
          return;
        Log.l(VerifiedNamesCertifier.LogHdr, "Ident Change for {0}", (object) senderJid);
        DateTime utcNow = DateTime.UtcNow;
        if (!immediate)
        {
          int num = new Random().Next(60, 240);
          utcNow += TimeSpan.FromMinutes((double) num);
        }
        VerifiedNamesCertifier.ScheduleCertifyVerifiedUserAction(senderJid, new DateTime?(utcNow));
      }));
    }

    public static void OnIncomingMessage(
      string senderJid,
      ulong? serial,
      string level,
      byte[] certificate)
    {
      if (!JidHelper.IsUserJid(senderJid))
        return;
      UserStatus userStatus = UserCache.Get(senderJid, false);
      if (userStatus == null && !serial.HasValue)
        return;
      UserStatus user = userStatus == null ? UserCache.Get(senderJid, true) : userStatus;
      VerifiedNameState verifiedName = user.VerifiedName;
      switch (verifiedName)
      {
        case VerifiedNameState.NotApplicable:
          if (!serial.HasValue)
            break;
          if (certificate != null)
          {
            VerifiedNamesCertifier.OnCertificate(senderJid, serial, level, certificate);
            break;
          }
          Log.l(VerifiedNamesCertifier.LogHdr, "Requesting cert for previously non biz user: {0}, existing cert {1}", (object) senderJid, (object) serial);
          VerifiedNamesCertifier.ScheduleCertifyVerifiedUserAction(senderJid, new DateTime?());
          ContactsContext.Instance((Action<ContactsContext>) (db =>
          {
            user.VerifiedName = VerifiedNameState.PendingCertification;
            user.VerifiedLevel = string.IsNullOrEmpty(level) ? VerifiedLevel.unknown : VerifiedNamesCertifier.GetVerfiedLevelAsEnum(level);
            db.SubmitChanges();
          }));
          break;
        case VerifiedNameState.PendingCertification:
        case VerifiedNameState.Verified:
          if (!serial.HasValue)
          {
            Log.l(VerifiedNamesCertifier.LogHdr, "Downgrading user, was {0}", (object) verifiedName);
            ContactsContext.Instance((Action<ContactsContext>) (db =>
            {
              user.VerifiedName = VerifiedNameState.NotApplicable;
              user.PushName = (string) null;
              user.VerifiedNameCertificateDetails = (VerifiedNameCertificate.Details) null;
              user.VerifiedLevel = VerifiedLevel.NotApplicable;
              db.SubmitChanges();
            }));
            if (verifiedName != VerifiedNameState.PendingCertification)
              break;
            MessagesContext.Run((MessagesContext.MessagesCallback) (mdb => mdb.ReleasePendingMessages(senderJid)));
            break;
          }
          ulong? nullable;
          if (certificate != null)
          {
            nullable = serial;
            ulong? serial1 = (ulong?) user.VerifiedNameCertificateDetails?.Serial;
            if (((long) nullable.GetValueOrDefault() == (long) serial1.GetValueOrDefault() ? (nullable.HasValue == serial1.HasValue ? 1 : 0) : 0) != 0)
            {
              VerifiedNamesCertifier.OnCertificate(senderJid, serial, level, certificate);
              break;
            }
          }
          VerifiedNameCertificate.Details certificateDetails1 = user.VerifiedNameCertificateDetails;
          int num1;
          if (certificateDetails1 == null)
          {
            num1 = serial.HasValue ? 1 : 0;
          }
          else
          {
            ulong? serial2 = certificateDetails1.Serial;
            nullable = serial;
            num1 = (long) serial2.GetValueOrDefault() == (long) nullable.GetValueOrDefault() ? (serial2.HasValue != nullable.HasValue ? 1 : 0) : 1;
          }
          if (num1 == 0)
          {
            VerifiedNameCertificate.Details certificateDetails2 = user.VerifiedNameCertificateDetails;
            int num2;
            if (certificateDetails2 == null)
            {
              num2 = 1;
            }
            else
            {
              nullable = certificateDetails2.Serial;
              num2 = !nullable.HasValue ? 1 : 0;
            }
            if (num2 == 0)
            {
              if (level == null || VerifiedNamesCertifier.GetVerfiedLevelAsEnum(level) == user.VerifiedLevel)
                break;
              ContactsContext.Instance((Action<ContactsContext>) (db =>
              {
                VerifiedLevel verifiedLevel = user.VerifiedLevel;
                user.VerifiedLevel = VerifiedNamesCertifier.GetVerfiedLevelAsEnum(level);
                Log.l(VerifiedNamesCertifier.LogHdr, "Changing verified level {0} to {1}", (object) verifiedLevel, (object) user.VerifiedLevel);
                db.SubmitChanges();
              }));
              break;
            }
          }
          VerifiedNamesCertifier.IdentityChangedForUser(senderJid, true);
          break;
      }
    }

    public static void MaybeAddBizSystemMessageToChat(
      string chatJid,
      SystemMessageWrapper.MessageTypes bizMessageType)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        if (db.GetConversation(chatJid, CreateOptions.None) != null)
        {
          Message bizSystemMessage = SystemMessageUtils.CreateBizSystemMessage(db, chatJid, bizMessageType);
          db.InsertMessageOnSubmit(bizSystemMessage);
          db.SubmitChanges();
        }
        else
          Log.d(VerifiedNamesCertifier.LogHdr, "No conversation for {0}, can't show {1}", (object) chatJid, (object) bizMessageType);
      }));
    }

    public static void HandleBusiness(UsyncQuery.BusinessResult serverBizData)
    {
      try
      {
        if (serverBizData.BizProfile != null)
        {
          Action onCompletion = (Action) (() => Log.d(VerifiedNamesCertifier.LogHdr, "Usync biz profile processed for {0}", (object) serverBizData.Jid));
          VerifiedNamesCertifier.OnProfileNotification(serverBizData.Jid, serverBizData.BizProfile, onCompletion);
        }
        if (serverBizData.VerifiedLevel == null)
          return;
        UserStatus userStatus = UserCache.Get(serverBizData.Jid, false);
        if (userStatus == null)
        {
          Log.l(VerifiedNamesCertifier.LogHdr, "Usync supplied user not found locally {0}", (object) serverBizData.Jid);
          VerifiedNamesCertifier.IdentityChangedForUser(serverBizData.Jid, true);
        }
        if (serverBizData.Certificate != null)
        {
          Action onCompletion = (Action) (() => Log.d("biz", "Usync biz certificate updated for {0}", (object) serverBizData.Jid));
          VerifiedNamesCertifier.OnCertificateNotification(serverBizData.Jid, (ulong?) userStatus?.VerifiedNameCertificateDetails?.Serial, serverBizData.VerifiedLevel, serverBizData.Certificate, onCompletion);
        }
        else
          VerifiedNamesCertifier.OnVerifiedLevelNotification(serverBizData.Jid, (ulong?) userStatus?.VerifiedNameCertificateDetails?.Serial, serverBizData.VerifiedLevel);
      }
      catch (Exception ex)
      {
        string context = VerifiedNamesCertifier.LogHdr + " exception hadling usync data";
        Log.LogException(ex, context);
      }
    }

    public static void ScheduleCertifyVerifiedUserAction(
      string senderJid,
      DateTime? attemptAtTimeUtc,
      ulong? serial = null)
    {
      Log.d(VerifiedNamesCertifier.LogHdr, "schedule cert for {0}, utc {1}, {2}", (object) senderJid, attemptAtTimeUtc.HasValue ? (object) attemptAtTimeUtc.Value.ToString() : (object) "none", serial.HasValue ? (object) serial.Value.ToString() : (object) "null");
      DateTime utcNow = DateTime.UtcNow;
      PersistentAction pa = VerifiedNamesCertifier.AddPaAndRemoveDuplicates(senderJid, attemptAtTimeUtc, serial, utcNow);
      bool flag = false;
      if (pa != null && VerifiedNamesCertifier.ExtractTimeUtcFromBinaryData(pa.ActionData) <= utcNow)
      {
        AppState.Worker.Enqueue((Action) (() => AppState.AttemptPersistentAction(pa)));
        flag = true;
      }
      Log.l(VerifiedNamesCertifier.LogHdr, "Scheduling complete {0}", (object) flag);
    }

    private static PersistentAction AddPaAndRemoveDuplicates(
      string senderJid,
      DateTime? attemptAtTimeUtc,
      ulong? serial,
      DateTime utcTimeNow)
    {
      PersistentAction pa = (PersistentAction) null;
      int deleteCount = 0;
      int foundCount = 0;
      int b4NowCount = 0;
      int ignoringAsOneAlreadyThere = 0;
      int dontScheduleCount = 0;
      DateTime runat = attemptAtTimeUtc.HasValue ? attemptAtTimeUtc.Value : utcTimeNow;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        List<PersistentAction> persistentActionList = new List<PersistentAction>();
        PersistentAction[] persistentActions = db.GetPersistentActions(PersistentAction.Types.CertifyVerifiedUser);
        foundCount = ((IEnumerable<PersistentAction>) persistentActions).Count<PersistentAction>();
        Log.d(VerifiedNamesCertifier.LogHdr, "Found {0}", (object) foundCount);
        foreach (PersistentAction persistentAction in persistentActions)
        {
          if (persistentAction.Jid == senderJid)
          {
            DateTime utcFromBinaryData = VerifiedNamesCertifier.ExtractTimeUtcFromBinaryData(persistentAction.ActionData);
            if (utcTimeNow >= utcFromBinaryData)
            {
              ++b4NowCount;
              if (!attemptAtTimeUtc.HasValue)
              {
                ++dontScheduleCount;
                ++ignoringAsOneAlreadyThere;
              }
            }
            else if (runat < utcFromBinaryData)
              persistentActionList.Add(persistentAction);
            else
              ++dontScheduleCount;
          }
        }
        deleteCount = persistentActionList.Count;
        foreach (PersistentAction a in persistentActionList)
          db.DeletePersistentActionOnSubmit(a);
        if (dontScheduleCount == 0)
        {
          pa = VerifiedNamesCertifier.CertifyVerifiedUser(senderJid, runat, serial);
          db.StorePersistentAction(pa);
        }
        db.SubmitChanges();
      }));
      Log.l(VerifiedNamesCertifier.LogHdr, "Deduplication complete {0}, {1}, {2}, {3}, {4}", (object) dontScheduleCount, (object) deleteCount, (object) foundCount, (object) b4NowCount, (object) ignoringAsOneAlreadyThere);
      return pa;
    }

    private static DateTime ExtractTimeUtcFromBinaryData(byte[] actionData)
    {
      return DateTime.FromBinary(new BinaryData(actionData).ReadLong64(0));
    }

    private static ulong? ExtractSerialFromBinaryData(byte[] actionData)
    {
      ulong? serialFromBinaryData = new ulong?();
      BinaryData binaryData = new BinaryData(actionData);
      if (binaryData.Length() > 8 && binaryData.ReadByte(8) == (byte) 1)
        serialFromBinaryData = new ulong?(binaryData.ReadULong64(9));
      return serialFromBinaryData;
    }

    private static PersistentAction CertifyVerifiedUser(
      string senderJid,
      DateTime attemptTimeUtc,
      ulong? serial)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendLong64(attemptTimeUtc.ToBinary());
      if (serial.HasValue)
      {
        binaryData.AppendByte((byte) 1);
        binaryData.AppendULong64(serial.Value);
      }
      else
      {
        binaryData.AppendByte((byte) 0);
        binaryData.AppendULong64(0UL);
      }
      return new PersistentAction()
      {
        ActionType = 30,
        Jid = senderJid,
        ActionData = binaryData.Get(),
        AttemptsLimit = new int?(100)
      };
    }

    public static IObservable<Unit> PerformCertifyVerifiedUser(
      FunXMPP.Connection conn,
      string senderJid,
      byte[] actionData)
    {
      DateTime attemptTime = VerifiedNamesCertifier.ExtractTimeUtcFromBinaryData(actionData);
      ulong? requestedSerial = VerifiedNamesCertifier.ExtractSerialFromBinaryData(actionData);
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        if (DateTime.UtcNow < attemptTime)
        {
          Log.l(VerifiedNamesCertifier.LogHdr, "Bypass cert request - not time yet");
          observer.OnCompleted();
        }
        else
        {
          bool stillWaiting = false;
          ContactsContext.Instance((Action<ContactsContext>) (cdb =>
          {
            UserStatus userStatus = cdb.GetUserStatus(senderJid);
            stillWaiting = userStatus != null && userStatus.VerifiedName == VerifiedNameState.PendingCertification;
          }));
          if (!stillWaiting)
          {
            VerifiedNamesCertifier.ReleasePendingMessages(senderJid);
            observer.OnNext(new Unit());
          }
          else
          {
            Action<ulong?, string, byte[]> onCompleted = (Action<ulong?, string, byte[]>) ((serial, verified_level, serializedCertificate) =>
            {
              Log.l(VerifiedNamesCertifier.LogHdr, "received cert {0} {1}", (object) verified_level, serializedCertificate == null ? (object) "null" : (object) serializedCertificate.Length.ToString());
              VerifiedNamesCertifier.OnCertificate(senderJid, serial, verified_level, serializedCertificate);
              VerifiedNamesCertifier.ReleasePendingMessages(senderJid);
              observer.OnNext(new Unit());
            });
            Action<int> onError = (Action<int>) (error =>
            {
              Log.l(VerifiedNamesCertifier.LogHdr, "error {0} getting cert for {1}", (object) error, (object) senderJid);
              DateTime dateTime = DateTime.UtcNow;
              ulong? serial = requestedSerial;
              if (error >= 500)
              {
                dateTime = dateTime.AddMinutes(10.0);
              }
              else
              {
                dateTime = dateTime.AddHours(24.0);
                serial = new ulong?();
              }
              VerifiedNamesCertifier.ScheduleCertifyVerifiedUserAction(senderJid, new DateTime?(dateTime), serial);
              VerifiedNamesCertifier.ReleasePendingMessages(senderJid);
              observer.OnNext(new Unit());
            });
            try
            {
              conn.SendGetVerifiedName(senderJid, requestedSerial, onCompleted, onError);
            }
            catch (Exception ex)
            {
              string context = VerifiedNamesCertifier.LogHdr + " exception requesting certificate";
              Log.LogException(ex, context);
              VerifiedNamesCertifier.ScheduleCertifyVerifiedUserAction(senderJid, new DateTime?(DateTime.UtcNow.AddMinutes(10.0)), requestedSerial);
              VerifiedNamesCertifier.ReleasePendingMessages(senderJid);
              observer.OnNext(new Unit());
            }
          }
        }
        return (Action) (() => { });
      }));
    }

    private static void ReleasePendingMessages(string senderJid)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (mdb => mdb.ReleasePendingMessages(senderJid)));
    }

    private static VerifiedLevel GetVerfiedLevelAsEnum(string verified_level)
    {
      verified_level = !string.IsNullOrEmpty(verified_level) ? verified_level.ToLowerInvariant() : throw new ArgumentNullException("Null verified_level supplied to GetVerfiedLevelAsEnum");
      foreach (VerifiedLevel verfiedLevelAsEnum in Enum.GetValues(typeof (VerifiedLevel)))
      {
        if (verfiedLevelAsEnum.ToString() == verified_level)
          return verfiedLevelAsEnum;
      }
      Log.l(VerifiedNamesCertifier.LogHdr, "Supplied verified_level {0} is not valid", (object) verified_level);
      if (!VerifiedNamesCertifier.VerifiedLevelClbThrottled)
      {
        Log.SendCrashLog((Exception) new ArgumentOutOfRangeException("Invalid Verified Level"), "Invalid Verified Level");
        VerifiedNamesCertifier.VerifiedLevelClbThrottled = true;
      }
      return VerifiedLevel.unknown;
    }

    public static VerifiedTier ConvertVerifiedLevel(VerifiedLevel level)
    {
      switch (level)
      {
        case VerifiedLevel.unknown:
        case VerifiedLevel.low:
          return VerifiedTier.Bottom;
        case VerifiedLevel.high:
          return VerifiedTier.Top;
        default:
          return VerifiedTier.NotApplicable;
      }
    }
  }
}
