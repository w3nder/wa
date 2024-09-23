// Decompiled with JetBrains decompiler
// Type: WhatsApp.Axolotl
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using WhatsApp.Events;
using WhatsApp.RegularExpressions;
using WhatsApp.WaCollections;
using WhatsAppNative;


namespace WhatsApp
{
  public class Axolotl
  {
    private const string LogHeader = "E2EEncryption";
    public const int MaxDecryptionRetries = 4;
    public const int RetriesBeforeFetchNewKey = 2;
    public const byte TypeByte = 5;
    public const int CurrentVersion = 2;
    public const int HashVersion = 1;
    private IAxolotlNative native;
    private SqliteAxolotlStore store;
    private object EncryptionLock = new object();
    private object GetPreKeyLock = new object();
    private Dictionary<string, List<Action>> messagesAwaitingPrekey = new Dictionary<string, List<Action>>();
    private List<Action> SendPreKeyOnComplete;
    private object SendPreKeyLock = new object();
    private System.Threading.Timer KeydBackoffTimer;

    public event Axolotl.MessageDecryptedHandler MessageDecrypted;

    public Axolotl(FunXMPP.Connection connection)
    {
      this.native = (IAxolotlNative) NativeInterfaces.CreateInstance<AxolotlNative>();
      this.store = new SqliteAxolotlStore(this);
      this.native.SetManagedCallbacks((IAxolotlStore) this.store);
      this.store.Initialize();
      AppState.Worker.Enqueue((Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.PurgeParticipantsHashJournal(DateTime.Now - TimeSpan.FromDays(30.0))))));
      this.MessageDecrypted += new Axolotl.MessageDecryptedHandler(connection.EventHandler.StoreIncomingMessage);
    }

    public IAxolotlNative NativeInterface => this.native;

    public SqliteAxolotlStore Store => this.store;

    public void OnLoggedIn()
    {
      if (!this.Store.HasEverSentKeys)
        this.SendPreKeysToServer();
      if (!this.Store.CheckRotateSignedPreKey())
        return;
      this.SendSignedPreKeyToServer();
    }

    private void SendSignedPreKeyToServer()
    {
      lock (this.SendPreKeyLock)
      {
        if (this.SendPreKeyOnComplete != null)
          return;
        Action onComplete = (Action) (() => Settings.LastSignedPreKeySent = new DateTime?(DateTime.Now));
        Action<int?, byte[]> onError = (Action<int?, byte[]>) ((err, identity) =>
        {
          if (!err.HasValue || err.Value != 409)
            return;
          Axolotl encryption = AppState.GetConnection().Encryption;
          if (identity == null || identity.IsEqualBytes(encryption.GetIdentityKeyForSending()))
            return;
          encryption.SendPreKeysToServer();
        });
        AppState.GetConnection().SendEncryptionRotateSignedPreKey(this.Store.LatestSignedPreKey, onComplete, onError);
      }
    }

    public void PreKeyServerCount(int count, Action ack)
    {
      Log.l("E2E", "Server notified of pre key running low");
      this.SendPreKeysToServer(ack);
    }

    private void SendPreKeysToServerImpl()
    {
      Log.l("E2E", "Sending local pre keys to server for registration ID: " + (object) this.store.LocalRegistrationId);
      bool signedKeyChanged = this.Store.CheckRotateSignedPreKey();
      Action onComplete1 = (Action) (() =>
      {
        lock (this.SendPreKeyLock)
        {
          if (signedKeyChanged)
            Settings.LastSignedPreKeySent = new DateTime?(DateTime.Now);
          this.Store.MarkAllPreKeysSentToServer();
          if (this.SendPreKeyOnComplete != null)
          {
            foreach (Action action in this.SendPreKeyOnComplete)
              action();
          }
          this.SendPreKeyOnComplete = (List<Action>) null;
        }
      });
      Action<int> onError = (Action<int>) (error =>
      {
        lock (this.SendPreKeyLock)
        {
          if (error != 406)
            return;
          Log.l("E2E", "Got a 406 error from server.  RESETTING REGISTRATION DATA!");
          List<Action> previousSendPreKeyOnComplete = this.SendPreKeyOnComplete;
          Action onComplete2 = (Action) (() =>
          {
            foreach (Action action in previousSendPreKeyOnComplete)
              action();
          });
          this.SendPreKeyOnComplete = (List<Action>) null;
          this.Store.Reset(onComplete2);
        }
      });
      AppState.GetConnection().SendEncryptionSetPreKey(this.Store.IdentityKeyForSending, this.Store.LocalRegistrationId, (byte) 5, this.Store.UnsentPreKeys, this.Store.LatestSignedPreKey, onComplete1, onError);
    }

    public void PreGeneratePreKeys()
    {
      bool flag = false;
      lock (this.SendPreKeyLock)
      {
        if (this.SendPreKeyOnComplete == null)
        {
          this.SendPreKeyOnComplete = new List<Action>();
          flag = true;
        }
      }
      if (!flag)
        return;
      this.Store.EnsureUnsentPreKeys();
      lock (this.SendPreKeyLock)
      {
        if (this.SendPreKeyOnComplete != null && this.SendPreKeyOnComplete.Any<Action>())
          this.SendPreKeysToServerImpl();
        else
          this.SendPreKeyOnComplete = (List<Action>) null;
      }
    }

    public void SendPreKeysToServer(Action onComplete = null)
    {
      bool flag = false;
      lock (this.SendPreKeyLock)
      {
        if (this.SendPreKeyOnComplete == null)
        {
          this.SendPreKeyOnComplete = new List<Action>();
          if (onComplete != null)
            this.SendPreKeyOnComplete.Add(onComplete);
          flag = true;
        }
        else if (onComplete != null)
          this.SendPreKeyOnComplete.Add(onComplete);
      }
      if (!flag)
        return;
      if (!this.Store.HasEnoughUnsentPreKeys)
        this.Store.EnsureUnsentPreKeys();
      this.SendPreKeysToServerImpl();
    }

    public void EnqueueToSendPreKeyComplete(Action onComplete)
    {
      bool flag = false;
      lock (this.SendPreKeyLock)
      {
        if (this.SendPreKeyOnComplete == null)
          flag = true;
        else
          this.SendPreKeyOnComplete.Add(onComplete);
      }
      if (!flag)
        return;
      onComplete();
    }

    private bool AddMessageAwaitingPreKey(string jid, Action onComplete)
    {
      bool flag = false;
      lock (this.GetPreKeyLock)
      {
        if (!this.messagesAwaitingPrekey.ContainsKey(jid))
        {
          this.messagesAwaitingPrekey[jid] = new List<Action>();
          flag = true;
        }
        this.messagesAwaitingPrekey[jid].Add(onComplete);
      }
      return flag;
    }

    private List<Action> RemoveMessageAwaitingPreKey(string jid)
    {
      List<Action> actionList = (List<Action>) null;
      lock (this.GetPreKeyLock)
      {
        if (this.messagesAwaitingPrekey.ContainsKey(jid))
        {
          actionList = this.messagesAwaitingPrekey[jid];
          this.messagesAwaitingPrekey.Remove(jid);
        }
      }
      return actionList;
    }

    private void SendGetMultiplePreKeys(
      Message message,
      IEnumerable<string> participants,
      Action retryAction)
    {
      string keyRemoteJid = message.KeyRemoteJid;
      Log.l("E2EEncryption", "Fetching PreKeys for GroupMessage Jid: " + message.KeyRemoteJid + " KeyId: " + message.KeyId + "Members: " + (object) participants.Count<string>());
      this.SendGetMultiplePreKeys(keyRemoteJid, participants, retryAction);
    }

    private void SendGetMultiplePreKeys(
      string keyRemoteJid,
      IEnumerable<string> participants,
      Action retryAction)
    {
      if (!this.ShouldSendGetPreKey)
        Log.l("E2EEncryption", "keyd backoff until: " + (object) Settings.KeydBackoffUtc);
      Action<List<AxolotlUser>, IEnumerable<string>> onComplete = (Action<List<AxolotlUser>, IEnumerable<string>>) ((retrieved, requested) =>
      {
        List<Action> actions = this.RemoveMessageAwaitingPreKey(keyRemoteJid);
        WAThreadPool.QueueUserWorkItem((Action) (() => this.ProcessPrekeySuccess(retrieved, requested, actions)));
      });
      if (!this.AddMessageAwaitingPreKey(keyRemoteJid, retryAction))
        return;
      AppState.GetConnection().SendGetPreKeys(participants, onComplete, new Action<IEnumerable<string>>(this.OnGetPreKeyFailure));
    }

    public void SendGetPreKey(string jid, Action completeAction)
    {
      if (!this.ShouldSendGetPreKey)
        Log.l("E2EEncryption", "keyd backoff until: " + (object) Settings.KeydBackoffUtc);
      Log.l("E2EEncryption", "Fetching PreKey for Jid: " + jid);
      Action<List<AxolotlUser>, IEnumerable<string>> onComplete = (Action<List<AxolotlUser>, IEnumerable<string>>) ((retrieved, requested) =>
      {
        List<Action> actions = this.RemoveMessageAwaitingPreKey(jid);
        WAThreadPool.QueueUserWorkItem((Action) (() => this.ProcessPrekeySuccess(retrieved, requested, actions)));
      });
      if (!this.AddMessageAwaitingPreKey(jid, completeAction))
        return;
      AppState.GetConnection().SendGetPreKeys((IEnumerable<string>) new string[1]
      {
        jid
      }, onComplete, new Action<IEnumerable<string>>(this.OnGetPreKeyFailure));
    }

    private void ProcessPrekeySuccess(
      List<AxolotlUser> usersRetrieved,
      IEnumerable<string> jidsRequested,
      List<Action> onComplete)
    {
      Set<string> set = new Set<string>();
      foreach (AxolotlUser axolotlUser in usersRetrieved)
      {
        if (axolotlUser.signedPreKey != null)
        {
          set.Add(axolotlUser.jid);
          Log.l("E2EEncryption", "Creating PreKeyBundle for Jid: " + axolotlUser.jid);
          int preKeyId = axolotlUser.preKey != null ? (int) axolotlUser.preKey.Id[0] << 16 | (int) axolotlUser.preKey.Id[1] << 8 | (int) axolotlUser.preKey.Id[2] : -1;
          int signedPreKeyId = (int) axolotlUser.signedPreKey.Id[0] << 16 | (int) axolotlUser.signedPreKey.Id[1] << 8 | (int) axolotlUser.signedPreKey.Id[2];
          try
          {
            this.SessionProcessPreKeyBundle(axolotlUser.jid, axolotlUser.registrationId, axolotlUser.type, axolotlUser.identity, axolotlUser.preKey != null ? axolotlUser.preKey.Data : (byte[]) null, preKeyId, axolotlUser.signedPreKey.Data, signedPreKeyId, axolotlUser.signedPreKey.Signature);
          }
          catch (Exception ex)
          {
            if (ex.GetHResult() == 2684748781U)
              Log.l("E2EDecrypt", "Session Process PreKeyBundle - Invalid Message");
            else
              Log.l("E2EDecrypt", "Session Process PreKeyBundle - Unknown Error");
          }
        }
      }
      foreach (string str in jidsRequested)
      {
        if (!set.Contains(str))
          this.Store.IdentityClearIdentity(str);
      }
      if (onComplete != null)
      {
        foreach (Action action in onComplete)
          action();
      }
      this.ResetKeydBackoff();
    }

    private void OnGetPreKeyFailure(IEnumerable<string> jidsFailed)
    {
      foreach (string jid in jidsFailed)
        this.RemoveMessageAwaitingPreKey(jid);
      this.IncrementKeydBackoff();
      this.EnsureKeydBackoffTimer();
    }

    public void VerifyDigest(Action onComplete)
    {
      Log.l("E2EEncrypt", "Sending Verify Digest");
      Action<AxolotlDigest> onComplete1 = (Action<AxolotlDigest>) (digest =>
      {
        if (!this.Store.VerifyDigest(digest))
        {
          Log.l("E2EEncrypt", "Digest check failed, resending our PreKeys");
          this.SendPreKeysToServer();
        }
        else
          Log.l("E2EEncrypt", "Digest verified successfully");
        if (onComplete == null)
          return;
        onComplete();
      });
      AppState.GetConnection().SendGetPreKeyDigest(onComplete1, (Action<int>) null);
    }

    public bool ContainsExistingSession(string jid) => this.Store.SessionContainsSession(jid);

    private wam_enum_media_type GetMediaType(FunXMPP.FMessage message)
    {
      switch (message.media_wa_type)
      {
        case FunXMPP.FMessage.Type.Image:
          return wam_enum_media_type.PHOTO;
        case FunXMPP.FMessage.Type.Audio:
          return wam_enum_media_type.AUDIO;
        case FunXMPP.FMessage.Type.Video:
          return wam_enum_media_type.VIDEO;
        case FunXMPP.FMessage.Type.Contact:
          return message.message_properties.HasMultipleContacts() ? wam_enum_media_type.CONTACT_ARRAY : wam_enum_media_type.CONTACT;
        case FunXMPP.FMessage.Type.Location:
          return wam_enum_media_type.LOCATION;
        case FunXMPP.FMessage.Type.Document:
          return wam_enum_media_type.DOCUMENT;
        case FunXMPP.FMessage.Type.ExtendedText:
          return wam_enum_media_type.URL;
        case FunXMPP.FMessage.Type.Gif:
          return wam_enum_media_type.GIF;
        case FunXMPP.FMessage.Type.LiveLocation:
          return wam_enum_media_type.LIVE_LOCATION;
        case FunXMPP.FMessage.Type.Sticker:
          return wam_enum_media_type.STICKER;
        case FunXMPP.FMessage.Type.Unsupported:
          return wam_enum_media_type.FUTURE;
        case FunXMPP.FMessage.Type.CallOffer:
          return wam_enum_media_type.CALL;
        default:
          return wam_enum_media_type.NONE;
      }
    }

    public void ProcessPlainTextToProtocolBufferMessage(
      string recipientId,
      byte[] plainText,
      FunXMPP.FMessage message)
    {
      WhatsApp.ProtoBuf.Message message1;
      if (((IEnumerable<FunXMPP.FMessage.Encrypted>) message.encrypted).FirstOrDefault<FunXMPP.FMessage.Encrypted>().cipher_version == 2)
        message1 = WhatsApp.ProtoBuf.Message.CreateFromPlainText(plainText);
      else if (((IEnumerable<FunXMPP.FMessage.Encrypted>) message.encrypted).FirstOrDefault<FunXMPP.FMessage.Encrypted>().cipher_version == 3)
      {
        message1 = new WhatsApp.ProtoBuf.Message();
        message1.UnknownSerialized = plainText;
      }
      else
      {
        message1 = new WhatsApp.ProtoBuf.Message();
        message1.Conversation = Encoding.UTF8.GetString(plainText, 0, plainText.Length);
      }
      if (message1.SenderKeyDistributionMessageField != null)
      {
        WhatsApp.ProtoBuf.Message.SenderKeyDistributionMessage distributionMessageField = message1.SenderKeyDistributionMessageField;
        try
        {
          this.GroupProcessSenderKeyBundle(distributionMessageField.GroupId, recipientId, false, distributionMessageField.AxolotlSenderKeyDistributionMessage);
        }
        catch (Exception ex)
        {
          if (ex.GetHResult() == 2684748781U)
            Log.l("E2EDecrypt", "Group Process PreKeyBundle - Invalid Message");
          else
            Log.l("E2EDecrypt", "Group Process PreKeyBundle - Unknown Error");
          Log.SendCrashLog(ex, "Group Process PreKeyBundle", false);
        }
      }
      if (message1.FastRatchetKeySenderKeyDistributionMessage != null)
      {
        WhatsApp.ProtoBuf.Message.SenderKeyDistributionMessage distributionMessage = message1.FastRatchetKeySenderKeyDistributionMessage;
        try
        {
          this.GroupProcessSenderKeyBundle(LiveLocationManager.LocationGroupJid, recipientId, true, distributionMessage.AxolotlSenderKeyDistributionMessage);
        }
        catch (Exception ex)
        {
          if (ex.GetHResult() == 2684748781U)
            Log.l("E2EDecrypt", "Group Process PreKeyBundle - Invalid Message");
          else
            Log.l("E2EDecrypt", "Group Process PreKeyBundle - Unknown Error");
          Log.SendCrashLog(ex, "Group Process PreKeyBundle", false);
        }
      }
      if (message1.UnknownTagsToReply != null)
        throw new Axolotl.DecryptUnknownTagsException()
        {
          UnknownTags = message1.UnknownTagsToReply.Select<WhatsApp.ProtoBuf.MessageTypes, uint>((Func<WhatsApp.ProtoBuf.MessageTypes, uint>) (t => (uint) t)).ToArray<uint>()
        };
      if (message1.UnknownSerialized != null)
      {
        message.binary_data = message1.UnknownSerialized;
        message.media_wa_type = FunXMPP.FMessage.Type.ProtocolBuffer;
        Axolotl.MessageDecryptedHandler messageDecrypted = this.MessageDecrypted;
        if (messageDecrypted == null)
          return;
        messageDecrypted(message);
      }
      else
      {
        if (!message1.IsValid)
          throw new Axolotl.DecryptInvalidProtocolBufferException();
        if (message1.ProtocolMessageField != null)
        {
          WhatsApp.ProtoBuf.Message.ProtocolMessage.Type? type1 = message1.ProtocolMessageField.type;
          WhatsApp.ProtoBuf.Message.ProtocolMessage.Type type2 = WhatsApp.ProtoBuf.Message.ProtocolMessage.Type.REVOKE;
          if ((type1.GetValueOrDefault() == type2 ? (type1.HasValue ? 1 : 0) : 0) == 0 || message.edit_version != 7)
            return;
          message.media_wa_type = FunXMPP.FMessage.Type.ProtocolMessage;
          message.proto_buf = message1.ToPlainText();
          Axolotl.MessageDecryptedHandler messageDecrypted = this.MessageDecrypted;
          if (messageDecrypted == null)
            return;
          messageDecrypted(message);
        }
        else if (message1.IsEmpty)
        {
          message.media_wa_type = FunXMPP.FMessage.Type.Empty;
          Axolotl.MessageDecryptedHandler messageDecrypted = this.MessageDecrypted;
          if (messageDecrypted == null)
            return;
          messageDecrypted(message);
        }
        else
        {
          if (message.edit_version > 0)
            return;
          message1.PopulateFMessage(message);
          Log.l("E2EDecrypt", "Decrypted message of type: " + message.media_wa_type.ToString() + " from: " + message.key.remote_jid + " id: " + message.key.id + " - " + message.remote_resource);
          Axolotl.MessageDecryptedHandler messageDecrypted = this.MessageDecrypted;
          if (messageDecrypted == null)
            return;
          messageDecrypted(message);
        }
      }
    }

    public void DecryptMessage(
      FunXMPP.FMessage message,
      AxolotlSessionCipher.PayloadDecryptedHandler handler = null)
    {
      lock (this.EncryptionLock)
      {
        bool flag = true;
        E2eMessageRecv e2eMessageRecv = new E2eMessageRecv();
        MessageDecryptionState messageDecryptionState = MessageDecryptionState.Success;
        string groupId = message.key.remote_jid;
        string str = message.key.remote_jid;
        int num = 0;
        uint[] numArray = (uint[]) null;
        if (handler == null)
          handler = new AxolotlSessionCipher.PayloadDecryptedHandler(this.ProcessPlainTextToProtocolBufferMessage);
        switch (JidHelper.GetJidType(message.key.remote_jid))
        {
          case JidHelper.JidTypes.Group:
            e2eMessageRecv.e2eDestination = new wam_enum_e2e_destination?(wam_enum_e2e_destination.GROUP);
            str = message.remote_resource;
            break;
          case JidHelper.JidTypes.Status:
            e2eMessageRecv.e2eDestination = new wam_enum_e2e_destination?(wam_enum_e2e_destination.STATUS);
            str = message.remote_resource;
            break;
          default:
            if (JidHelper.IsBroadcastJid(message.remote_resource))
            {
              e2eMessageRecv.e2eDestination = new wam_enum_e2e_destination?(wam_enum_e2e_destination.LIST);
              groupId = message.remote_resource;
              break;
            }
            e2eMessageRecv.e2eDestination = new wam_enum_e2e_destination?(wam_enum_e2e_destination.INDIVIDUAL);
            break;
        }
        if (message.encrypted.Length == 1)
          num = message.encrypted[0].cipher_retry_count;
        e2eMessageRecv.retryCount = new long?((long) num);
        foreach (FunXMPP.FMessage.Encrypted encrypted in message.encrypted)
        {
          e2eMessageRecv.e2eCiphertextVersion = new long?((long) encrypted.cipher_version);
          try
          {
            if (encrypted.cipher_text_type == "msg")
            {
              e2eMessageRecv.e2eCiphertextType = new wam_enum_e2e_ciphertext_type?(wam_enum_e2e_ciphertext_type.MESSAGE);
              using (AxolotlSessionCipher axolotlSessionCipher = new AxolotlSessionCipher(this, str))
              {
                try
                {
                  axolotlSessionCipher.DecryptMessage(message, encrypted.cipher_text_bytes, false, handler);
                  e2eMessageRecv.e2eSuccessful = new bool?(true);
                }
                catch (Exception ex)
                {
                  if (2684748784U == ex.GetHResult())
                  {
                    messageDecryptionState = MessageDecryptionState.Failed;
                    Log.l("E2EDecrypt", "No Session");
                    e2eMessageRecv.e2eSuccessful = new bool?(false);
                    e2eMessageRecv.e2eFailureReason = new wam_enum_e2e_failure_reason?(wam_enum_e2e_failure_reason.NO_SESSION_AVAILABLE);
                  }
                  else
                    throw;
                }
              }
            }
            else if (encrypted.cipher_text_type == "pkmsg")
            {
              e2eMessageRecv.e2eCiphertextType = new wam_enum_e2e_ciphertext_type?(wam_enum_e2e_ciphertext_type.PREKEY_MESSAGE);
              using (AxolotlSessionCipher axolotlSessionCipher = new AxolotlSessionCipher(this, str))
              {
                try
                {
                  axolotlSessionCipher.DecryptMessage(message, encrypted.cipher_text_bytes, true, handler);
                  e2eMessageRecv.e2eSuccessful = new bool?(true);
                }
                catch (Exception ex)
                {
                  switch ((Axolotl_Error) ex.GetHResult())
                  {
                    case Axolotl_Error.AX_ERR_INVALID_KEY:
                      messageDecryptionState = MessageDecryptionState.Failed;
                      Log.l("E2EDecrypt", "Invalid Key");
                      e2eMessageRecv.e2eSuccessful = new bool?(false);
                      e2eMessageRecv.e2eFailureReason = new wam_enum_e2e_failure_reason?(wam_enum_e2e_failure_reason.PRE_KEY_MESSAGE_INVALID_KEY);
                      continue;
                    case Axolotl_Error.AX_ERR_INVALID_KEY_ID:
                      messageDecryptionState = MessageDecryptionState.Failed;
                      Log.l("E2EDecrypt", "Invalid Key ID, PreKeyMessage missing PreKey");
                      e2eMessageRecv.e2eSuccessful = new bool?(false);
                      e2eMessageRecv.e2eFailureReason = new wam_enum_e2e_failure_reason?(wam_enum_e2e_failure_reason.PRE_KEY_MESSAGE_MISSING_PRE_KEY);
                      continue;
                    case Axolotl_Error.AX_ERR_UNTRUSTED_IDENTITY:
                      messageDecryptionState = MessageDecryptionState.Failed;
                      Log.l("E2EDecrypt", "Untrusted identity");
                      e2eMessageRecv.e2eSuccessful = new bool?(false);
                      e2eMessageRecv.e2eFailureReason = new wam_enum_e2e_failure_reason?(wam_enum_e2e_failure_reason.UNTRUSTED_IDENTITY);
                      continue;
                    default:
                      throw;
                  }
                }
              }
            }
            else if (encrypted.cipher_text_type == "skmsg")
            {
              e2eMessageRecv.e2eCiphertextType = new wam_enum_e2e_ciphertext_type?(wam_enum_e2e_ciphertext_type.SENDER_KEY_MESSAGE);
              using (AxolotlGroupCipher axolotlGroupCipher = new AxolotlGroupCipher(this, groupId, str))
              {
                try
                {
                  axolotlGroupCipher.DecryptMessage(message, encrypted.cipher_text_bytes);
                  e2eMessageRecv.e2eSuccessful = new bool?(true);
                }
                catch (Exception ex)
                {
                  if (2684748784U == ex.GetHResult())
                  {
                    messageDecryptionState = MessageDecryptionState.Failed;
                    Log.l("E2EGroupDecrypt", "No Session");
                    e2eMessageRecv.e2eSuccessful = new bool?(false);
                    e2eMessageRecv.e2eFailureReason = new wam_enum_e2e_failure_reason?(wam_enum_e2e_failure_reason.NO_SESSION_AVAILABLE);
                  }
                  else
                    throw;
                }
              }
            }
            else
            {
              messageDecryptionState = MessageDecryptionState.Failed;
              Log.l("E2EDecrypt", "Invalid Cipher Text Type");
              e2eMessageRecv.e2eSuccessful = new bool?(false);
              e2eMessageRecv.e2eFailureReason = new wam_enum_e2e_failure_reason?(wam_enum_e2e_failure_reason.INVALID_MESSAGE);
            }
          }
          catch (Axolotl.AxolotlPaddingException ex)
          {
            messageDecryptionState = MessageDecryptionState.Failed;
            e2eMessageRecv.e2eSuccessful = new bool?(false);
            e2eMessageRecv.e2eFailureReason = new wam_enum_e2e_failure_reason?(wam_enum_e2e_failure_reason.INVALID_PADDING);
            Log.LogException((Exception) ex, "E2EDecrypt: Axolotl Padding Exception");
          }
          catch (Axolotl.DecryptUnknownTagsException ex)
          {
            messageDecryptionState = MessageDecryptionState.MultipleUnknownTags;
            numArray = ex.UnknownTags;
            e2eMessageRecv.e2eSuccessful = new bool?(true);
          }
          catch (Axolotl.DecryptInvalidProtocolBufferException ex)
          {
            messageDecryptionState = MessageDecryptionState.InvalidProtocolBuffer;
            e2eMessageRecv.e2eSuccessful = new bool?(true);
          }
          catch (Exception ex)
          {
            switch (ex.GetHResult())
            {
              case 2684748777:
                Log.l("E2EDecrypt", "Duplicate Message");
                flag = false;
                continue;
              case 2684748781:
                messageDecryptionState = MessageDecryptionState.Failed;
                Log.l("E2EDecrypt", "Invalid Message");
                e2eMessageRecv.e2eSuccessful = new bool?(false);
                e2eMessageRecv.e2eFailureReason = new wam_enum_e2e_failure_reason?(wam_enum_e2e_failure_reason.INVALID_MESSAGE);
                continue;
              case 2684748782:
                messageDecryptionState = MessageDecryptionState.Failed;
                Log.l("E2EDecrypt", "Invalid Version");
                e2eMessageRecv.e2eSuccessful = new bool?(false);
                e2eMessageRecv.e2eFailureReason = new wam_enum_e2e_failure_reason?(wam_enum_e2e_failure_reason.INVALID_VERSION);
                continue;
              case 2684748783:
                messageDecryptionState = MessageDecryptionState.Failed;
                Log.l("E2EDecrypt", "Legacy Message");
                e2eMessageRecv.e2eSuccessful = new bool?(false);
                e2eMessageRecv.e2eFailureReason = new wam_enum_e2e_failure_reason?(wam_enum_e2e_failure_reason.LEGACY_MESSAGE);
                continue;
              default:
                messageDecryptionState = MessageDecryptionState.Failed;
                Log.SendCrashLog(ex, "E2EDecrypt: Unexpected error - HRESULT: " + ex.GetHResult().ToString("X8"));
                e2eMessageRecv.e2eSuccessful = new bool?(false);
                continue;
            }
          }
        }
        if (flag)
        {
          e2eMessageRecv.messageMediaType = new wam_enum_media_type?(this.GetMediaType(message));
          if (e2eMessageRecv.e2eSuccessful.HasValue && e2eMessageRecv.e2eSuccessful.Value)
            e2eMessageRecv.SaveEventSampled(20U);
          else
            e2eMessageRecv.SaveEvent();
          if (e2eMessageRecv.e2eSuccessful.HasValue && !e2eMessageRecv.e2eSuccessful.Value && num == 4)
            Log.SendCrashLog((Exception) new Axolotl.AxolotlDecryptException(), "E2E - Max Decryption Failed - Reason: " + e2eMessageRecv.e2eFailureReason.Value.ToString());
        }
        switch (messageDecryptionState)
        {
          case MessageDecryptionState.MultipleUnknownTags:
            throw new Axolotl.DecryptUnknownTagsException()
            {
              UnknownTags = numArray
            };
          case MessageDecryptionState.InvalidProtocolBuffer:
            Log.SendCrashLog((Exception) new Axolotl.AxolotlProtocolBufferException(), "Invalid Protocol Buffer Message", filter: false);
            throw new Axolotl.AxolotlProtocolBufferException();
          case MessageDecryptionState.Failed:
            throw new Axolotl.DecryptRetryException()
            {
              RetryCount = num + 1
            };
        }
      }
    }

    public FunXMPP.FMessage EncryptMessage(Message message, Action retryAction)
    {
      lock (this.EncryptionLock)
      {
        FunXMPP.FMessage fmessage = (FunXMPP.FMessage) null;
        if (JidHelper.IsSelfJid(message.KeyRemoteJid))
          return message.ToFMessage();
        Axolotl.MessageParticipants messageParticipants = Axolotl.GetMessageParticipants(message);
        ShouldEncryptResult shouldEncryptResult = ShouldEncryptResult.Encrypted;
        if (!message.ContainsMediaContent())
          shouldEncryptResult = this.ShouldEncryptMessage(message);
        if (shouldEncryptResult == ShouldEncryptResult.Fail)
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            message.Status = FunXMPP.FMessage.Status.Error;
            db.SubmitChanges();
          }));
        else
          fmessage = !JidHelper.IsMultiParticipantsChatJid(message.KeyRemoteJid) ? this.EncryptIndividualMessage(message, retryAction) : this.EncryptGroupMessage(message, retryAction, messageParticipants);
        return fmessage;
      }
    }

    private FunXMPP.FMessage EncryptIndividualMessage(
      Message message,
      Action retryAction,
      Axolotl.GroupEncryptionInfo groupInfo = null)
    {
      string recipient = groupInfo != null ? groupInfo.Participant : message.KeyRemoteJid;
      E2eMessageSend e2eMessageSend = new E2eMessageSend();
      switch (JidHelper.GetJidType(message.KeyRemoteJid))
      {
        case JidHelper.JidTypes.Group:
          e2eMessageSend.e2eDestination = new wam_enum_e2e_destination?(wam_enum_e2e_destination.GROUP);
          break;
        case JidHelper.JidTypes.Broadcast:
          e2eMessageSend.e2eDestination = new wam_enum_e2e_destination?(wam_enum_e2e_destination.LIST);
          break;
        case JidHelper.JidTypes.Status:
          e2eMessageSend.e2eDestination = new wam_enum_e2e_destination?(wam_enum_e2e_destination.STATUS);
          break;
        default:
          e2eMessageSend.e2eDestination = new wam_enum_e2e_destination?(wam_enum_e2e_destination.INDIVIDUAL);
          break;
      }
      int retryCount = 0;
      if (groupInfo != null)
        retryCount = groupInfo.RetryCount;
      else
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => retryCount = message.GetCipherRetryCount(db)));
      e2eMessageSend.retryCount = new long?((long) retryCount);
      FunXMPP.FMessage fmessage;
      try
      {
        int version = 2;
        fmessage = message.ToFMessage();
        if (version != 2)
          throw new ArgumentOutOfRangeException();
        CipherTextIncludes includes = new CipherTextIncludes(true);
        if (groupInfo != null)
          includes.SenderKeyDistributionMessage = this.GroupCreateSenderKeyBundle(fmessage.key.remote_jid, false);
        AxolotlCiphertextType type;
        byte[] numArray = this.EncryptIndividualPayloadImpl(WhatsApp.ProtoBuf.Message.CreateFromFMessage(fmessage, includes).ToPlainText(), retryAction, recipient, message.KeyId, retryCount, version, out type);
        if (numArray != null)
        {
          fmessage.encrypted = new FunXMPP.FMessage.Encrypted[1]
          {
            new FunXMPP.FMessage.Encrypted()
            {
              cipher_text_type = Axolotl.CiphertextTypeString(type),
              cipher_text_bytes = numArray,
              cipher_version = version,
              cipher_retry_count = retryCount,
              fun_media_type = message.GetFunMediaType()
            }
          };
          e2eMessageSend.e2eCiphertextType = new wam_enum_e2e_ciphertext_type?(type == AxolotlCiphertextType.CiphertextMessage ? wam_enum_e2e_ciphertext_type.MESSAGE : wam_enum_e2e_ciphertext_type.PREKEY_MESSAGE);
          e2eMessageSend.e2eCiphertextVersion = new long?((long) version);
          e2eMessageSend.e2eSuccessful = new bool?(true);
          Log.l("E2EEncryption", "Sending a message using " + Axolotl.CiphertextTypeString(type) + ": " + recipient + " - " + fmessage.key.id + ". Size: " + (object) numArray.Length);
        }
        else
          fmessage = (FunXMPP.FMessage) null;
      }
      catch (Axolotl.AxolotlEncryptMaxRetriesException ex)
      {
        if (groupInfo != null)
          groupInfo.MaxRetriesReached = true;
        else
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            message.Status = FunXMPP.FMessage.Status.Error;
            db.SubmitChanges();
          }));
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => this.MessageRetryCleanup(message, db)));
        Log.l("E2EEncryption", "Giving up sending message (" + message.KeyId + ") after max retries");
        e2eMessageSend.e2eSuccessful = new bool?(false);
        e2eMessageSend.SaveEvent();
        fmessage = (FunXMPP.FMessage) null;
        Log.SendCrashLog((Exception) new Axolotl.AxolotlEncryptException(), "E2EEncryption reach max retries", false);
      }
      catch (Exception ex)
      {
        e2eMessageSend.e2eSuccessful = new bool?(false);
        e2eMessageSend.SaveEvent();
        fmessage = (FunXMPP.FMessage) null;
      }
      if (fmessage != null)
      {
        e2eMessageSend.messageMediaType = new wam_enum_media_type?(this.GetMediaType(fmessage));
        if (e2eMessageSend.e2eSuccessful.HasValue && e2eMessageSend.e2eSuccessful.Value)
          e2eMessageSend.SaveEventSampled(10U);
        else
          e2eMessageSend.SaveEvent();
      }
      return fmessage;
    }

    public byte[] EncryptIndividualPayload(
      byte[] plainText,
      Action retryAction,
      string recipient,
      string keyId,
      int retryCount,
      int version,
      out AxolotlCiphertextType type)
    {
      lock (this.EncryptionLock)
        return this.EncryptIndividualPayloadImpl(plainText, retryAction, recipient, keyId, retryCount, version, out type);
    }

    private byte[] EncryptIndividualPayloadImpl(
      byte[] plainText,
      Action retryAction,
      string recipient,
      string keyId,
      int retryCount,
      int version,
      out AxolotlCiphertextType type)
    {
      byte[] CipherText = (byte[]) null;
      type = AxolotlCiphertextType.CiphertextPreKeyMessage;
      if (retryCount > 4)
        throw new Axolotl.AxolotlEncryptMaxRetriesException();
      bool flag = this.Store.SessionContainsSession(recipient);
      if (flag && retryCount > 0 && retryCount >= 2)
      {
        byte[] aliceBaseKey = (byte[]) null;
        this.GetSessionInfo(recipient, out int _, out aliceBaseKey);
        if (retryCount == 2)
        {
          Log.l("E2EEncryption", "Saving MessageBaseKey for Jid: " + recipient + " KeyId: " + keyId);
          this.Store.SaveMessageBaseKey(recipient, true, keyId, aliceBaseKey);
        }
        else if (retryCount > 2)
        {
          if (this.Store.IsSameBaseKey(recipient, true, keyId, aliceBaseKey))
          {
            Log.l("E2EEncryption", "Fetching new MessageBaseKey for Jid: " + recipient + " KeyId: " + keyId);
            flag = false;
          }
          else
          {
            Log.l("E2EEncryption", "Saving MessageBaseKey for Jid: " + recipient + " KeyId: " + keyId);
            this.Store.SaveMessageBaseKey(recipient, true, keyId, aliceBaseKey);
          }
        }
      }
      if (!flag)
      {
        this.SendGetPreKey(recipient, retryAction);
      }
      else
      {
        using (AxolotlSessionCipher axolotlSessionCipher = new AxolotlSessionCipher(this, recipient))
          axolotlSessionCipher.Encrypt(plainText, out type, out CipherText);
      }
      return CipherText;
    }

    public FunXMPP.FMessage EncryptGroupMessage(
      Message message,
      Action retryAction,
      Axolotl.MessageParticipants messageParticipants)
    {
      E2eMessageSend e2eMessageSend = new E2eMessageSend();
      JidHelper.JidTypes jidType = JidHelper.GetJidType(message.KeyRemoteJid);
      switch (jidType)
      {
        case JidHelper.JidTypes.Group:
          e2eMessageSend.e2eDestination = new wam_enum_e2e_destination?(wam_enum_e2e_destination.GROUP);
          break;
        case JidHelper.JidTypes.Broadcast:
          e2eMessageSend.e2eDestination = new wam_enum_e2e_destination?(wam_enum_e2e_destination.LIST);
          break;
        case JidHelper.JidTypes.Status:
          e2eMessageSend.e2eDestination = new wam_enum_e2e_destination?(wam_enum_e2e_destination.STATUS);
          break;
      }
      e2eMessageSend.retryCount = new long?(0L);
      string[] strArray = (string[]) null;
      GroupParticipantState[] participantsNeedingWelcome = (GroupParticipantState[]) null;
      if (!messageParticipants.IsHashCurrent)
      {
        strArray = messageParticipants.Participants;
      }
      else
      {
        participantsNeedingWelcome = messageParticipants.ParticipantsNeedingWelcome;
        if (((IEnumerable<GroupParticipantState>) participantsNeedingWelcome).Count<GroupParticipantState>() > 0)
        {
          strArray = ((IEnumerable<GroupParticipantState>) participantsNeedingWelcome).Select<GroupParticipantState, string>((Func<GroupParticipantState, string>) (s => s.MemberJid)).ToArray<string>();
          Log.l("E2EEncryption", "Adding Welcome Message to " + (object) ((IEnumerable<GroupParticipantState>) participantsNeedingWelcome).Count<GroupParticipantState>() + ": " + message.KeyRemoteJid + " - " + message.KeyId);
        }
      }
      byte[] numArray = (byte[]) null;
      if (!this.Store.SenderKeyContainsSenderKey(message.KeyRemoteJid, Settings.MyJid))
      {
        if (participantsNeedingWelcome != null && !((IEnumerable<GroupParticipantState>) participantsNeedingWelcome).Any<GroupParticipantState>())
          strArray = messageParticipants.Participants;
        numArray = this.GroupCreateSenderKeyBundle(message.KeyRemoteJid, false);
      }
      FunXMPP.FMessage fmessage = message.ToFMessage();
      List<FunXMPP.FMessage.Participant> source = new List<FunXMPP.FMessage.Participant>();
      if (strArray != null && ((IEnumerable<string>) strArray).Any<string>())
      {
        IEnumerable<string> strings = ((IEnumerable<string>) strArray).Where<string>((Func<string, bool>) (p => !this.Store.SessionContainsSession(p)));
        if (strings.Count<string>() > 0)
        {
          this.SendGetMultiplePreKeys(message, strings, retryAction);
          return (FunXMPP.FMessage) null;
        }
        foreach (string str in strArray)
        {
          FunXMPP.FMessage.Participant participant = new FunXMPP.FMessage.Participant(str);
          using (AxolotlSessionCipher axolotlSessionCipher = new AxolotlSessionCipher(this, str))
          {
            byte[] CipherText = (byte[]) null;
            FunXMPP.FMessage.Encrypted encrypted = new FunXMPP.FMessage.Encrypted();
            try
            {
              CipherTextIncludes includes = new CipherTextIncludes(false);
              if (messageParticipants.IsHashCurrent)
              {
                if (numArray == null)
                  numArray = this.GroupCreateSenderKeyBundle(fmessage.key.remote_jid, false);
                includes.SenderKeyDistributionMessage = numArray;
              }
              else
                includes.Message = true;
              AxolotlCiphertextType CipherTextType;
              axolotlSessionCipher.Encrypt(WhatsApp.ProtoBuf.Message.CreateFromFMessage(fmessage, includes).ToPlainText(), out CipherTextType, out CipherText);
              encrypted.cipher_text_type = Axolotl.CiphertextTypeString(CipherTextType);
              encrypted.cipher_text_bytes = CipherText;
              encrypted.cipher_version = 2;
              if (includes.Message)
                encrypted.fun_media_type = message.GetFunMediaType();
              participant.Encrypted = encrypted;
              Log.l("E2EEncryption", "Successfully encrypted individual message type: " + encrypted.cipher_text_type + " : " + fmessage.key.remote_jid + " - " + str);
            }
            catch (Exception ex)
            {
              Log.l("E2EEncryption", "Failed encrypted individual message: " + fmessage.key.remote_jid + " - " + str);
              participant = (FunXMPP.FMessage.Participant) null;
            }
          }
          if (participant != null)
            source.Add(participant);
        }
        e2eMessageSend.e2eSuccessful = new bool?(true);
        if (messageParticipants.IsHashCurrent)
        {
          Set<string> participantsWithNodes = new Set<string>();
          foreach (FunXMPP.FMessage.Participant participant in source)
            participantsWithNodes.Add(participant.Jid);
          Action action = (Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (mdb =>
          {
            Conversation conversation = mdb.GetConversation(message.KeyRemoteJid, CreateOptions.None);
            if (conversation == null || !(conversation.ParticipantsHash == message.ParticipantsHash))
              return;
            foreach (GroupParticipantState participantState in participantsNeedingWelcome)
            {
              if (participantsWithNodes.Contains(participantState.MemberJid))
                participantState.Flags |= 2L;
            }
            mdb.SubmitChanges();
          })));
          AppState.GetConnection().AddReceiptListener(message.KeyId, message.KeyRemoteJid, action);
        }
      }
      if (jidType == JidHelper.JidTypes.Broadcast || jidType == JidHelper.JidTypes.Status)
      {
        Set<string> set = (Set<string>) null;
        if (strArray != null)
          set = new Set<string>((IEnumerable<string>) strArray);
        foreach (string participant in messageParticipants.Participants)
        {
          if (set == null || !set.Contains(participant))
            source.Add(new FunXMPP.FMessage.Participant(participant));
        }
      }
      fmessage.participants = source.Any<FunXMPP.FMessage.Participant>() ? source.ToArray() : (FunXMPP.FMessage.Participant[]) null;
      if (messageParticipants.IsHashCurrent)
      {
        if (!this.Store.SenderKeyContainsSenderKey(message.KeyRemoteJid, Settings.MyJid))
        {
          Log.SendCrashLog((Exception) new Axolotl.AxolotlGroupException(), "SenderKey Missing, but we think everything is present");
          this.GroupCreateSenderKeyBundle(message.KeyRemoteJid, false);
        }
        using (AxolotlGroupCipher axolotlGroupCipher = new AxolotlGroupCipher(this, message.KeyRemoteJid, Settings.MyJid))
        {
          byte[] CipherText = (byte[]) null;
          FunXMPP.FMessage.Encrypted encrypted = new FunXMPP.FMessage.Encrypted();
          try
          {
            axolotlGroupCipher.EncryptMessage(fmessage, out CipherText);
            encrypted.cipher_text_type = Axolotl.CiphertextTypeString(AxolotlCiphertextType.CiphertextSenderKeyMessage);
            encrypted.cipher_text_bytes = CipherText;
            encrypted.cipher_version = 2;
            encrypted.fun_media_type = message.GetFunMediaType();
            fmessage.encrypted = new FunXMPP.FMessage.Encrypted[1]
            {
              encrypted
            };
            e2eMessageSend.e2eCiphertextType = new wam_enum_e2e_ciphertext_type?(wam_enum_e2e_ciphertext_type.SENDER_KEY_MESSAGE);
            e2eMessageSend.e2eSuccessful = new bool?(true);
            Log.l("E2EEncryption", "Sending a group message: " + fmessage.key.remote_jid + " - " + fmessage.key.id);
          }
          catch (Exception ex)
          {
            this.OnParticipantRemovedFromConversation(message.KeyRemoteJid);
            Log.LogException(ex, "GroupCipher Encryption failed - HRESULT: " + (object) ex.GetHResult());
            Log.SendCrashLog((Exception) new Axolotl.AxolotlGroupException(), "GroupCipher Encryption failed");
            e2eMessageSend.e2eSuccessful = new bool?(false);
            fmessage = (FunXMPP.FMessage) null;
          }
        }
      }
      else
      {
        fmessage.encrypted = new FunXMPP.FMessage.Encrypted[1]
        {
          new FunXMPP.FMessage.Encrypted()
          {
            cipher_text_type = Axolotl.CiphertextTypeString(AxolotlCiphertextType.CiphertextSenderKeyMessage),
            cipher_text_bytes = (byte[]) null,
            cipher_version = 2,
            fun_media_type = message.GetFunMediaType()
          }
        };
        e2eMessageSend.e2eCiphertextType = new wam_enum_e2e_ciphertext_type?(wam_enum_e2e_ciphertext_type.MESSAGE);
        e2eMessageSend.e2eSuccessful = new bool?(true);
        if (fmessage.participants == null)
          fmessage.participants = new FunXMPP.FMessage.Participant[0];
      }
      if (fmessage != null)
        e2eMessageSend.messageMediaType = new wam_enum_media_type?(this.GetMediaType(fmessage));
      e2eMessageSend.e2eCiphertextVersion = new long?(2L);
      if (e2eMessageSend.e2eSuccessful.HasValue && e2eMessageSend.e2eSuccessful.Value)
        e2eMessageSend.SaveEventSampled(10U);
      else
        e2eMessageSend.SaveEvent();
      return fmessage;
    }

    public byte[] EncryptFastRatchetPayload(byte[] plainText, string group)
    {
      lock (this.EncryptionLock)
      {
        if (!this.Store.FastRatchetSenderKeyContainsFastRatchetSenderKey(group, Settings.MyJid))
        {
          Log.SendCrashLog((Exception) new Axolotl.AxolotlGroupException(), "SenderKey Missing, but we think everything is present");
          this.GroupCreateSenderKeyBundle(group, true);
        }
        byte[] CipherText = (byte[]) null;
        try
        {
          using (AxolotlFastRatchetGroupCipher ratchetGroupCipher = new AxolotlFastRatchetGroupCipher(this, group, Settings.MyJid))
          {
            ratchetGroupCipher.EncryptPayload(plainText, out CipherText);
            Log.l("E2EEncryption", "Encrypted fast ratchet message: " + group);
          }
        }
        catch (Exception ex)
        {
          this.OnParticipantRemovedFromFastRatchetConversation(group);
          Log.LogException(ex, "FastRatchetGroupCipher Encryption failed - HRESULT: " + (object) ex.GetHResult());
          Log.SendCrashLog((Exception) new Axolotl.AxolotlGroupException(), "FastRatchetGroupCipher Encryption failed");
          CipherText = (byte[]) null;
        }
        return CipherText;
      }
    }

    public byte[] DecryptFastRatchetPayload(byte[] cipherText, string group, string sender)
    {
      lock (this.EncryptionLock)
      {
        byte[] plainText = (byte[]) null;
        using (AxolotlFastRatchetGroupCipher ratchetGroupCipher = new AxolotlFastRatchetGroupCipher(this, group, sender))
        {
          AxolotlFastRatchetGroupCipher.PayloadDecryptedHandler handler = (AxolotlFastRatchetGroupCipher.PayloadDecryptedHandler) (bytes => plainText = bytes);
          ratchetGroupCipher.DecryptPayload(cipherText, handler);
        }
        return plainText;
      }
    }

    public void OnParticipantRemovedFromConversation(string gJid)
    {
      Log.l("E2EEncryption", "participant left group, clearing SenderKey | {0}", (object) gJid);
      this.store.SenderKeyStoreRemoveKey(gJid, Settings.MyJid);
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        foreach (KeyValuePair<string, GroupParticipantState> participant in db.GetParticipants(gJid, false))
          participant.Value.Flags &= -3L;
        db.SubmitChanges();
      }));
    }

    public void OnParticipantRemovedFromFastRatchetConversation(string gJid)
    {
      Log.l("E2EEncryption", "participant left group, clearing SenderKey | {0}", (object) gJid);
      this.store.FastRatchetSenderKeyStoreRemoveKey(gJid, Settings.MyJid);
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        foreach (KeyValuePair<string, GroupParticipantState> participant in db.GetParticipants(gJid, false))
          participant.Value.Flags &= -3L;
        db.SubmitChanges();
      }));
    }

    public static Axolotl.MessageParticipants GetMessageParticipants(Message msg)
    {
      Axolotl.MessageParticipants messageParticipants = new Axolotl.MessageParticipants();
      string[] msgParticipants = (string[]) null;
      GroupParticipantState[] participantsNeedingWelcome = (GroupParticipantState[]) null;
      bool isHashCurrent = true;
      if (JidHelper.IsMultiParticipantsChatJid(msg.KeyRemoteJid))
      {
        JidHelper.IsStatusJid(msg.KeyRemoteJid);
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          Conversation conversation = db.GetConversation(msg.KeyRemoteJid, CreateOptions.None);
          if (conversation == null)
            return;
          if (msg.ParticipantsHash == null)
          {
            msg.ParticipantsHash = conversation.ParticipantsHash;
            Log.l("E2EEncrypt", "set participant hash | jid:{0},keyid:{1},hash:{2}", (object) msg.KeyRemoteJid, (object) msg.KeyId, (object) msg.ParticipantsHash);
            db.SubmitChanges();
          }
          Dictionary<string, GroupParticipantState> participants = db.GetParticipants(conversation.Jid, true);
          if (msg.ParticipantsHash != conversation.ParticipantsHash || msg.HasHashMismatchError(db))
          {
            Set<string> participantsFromHash = conversation.GetParticipantsFromHash(db, msg.ParticipantsHash);
            if (participantsFromHash != null && participantsFromHash.Any<string>() && participants != null)
            {
              if (!msg.IsRevoked())
              {
                msgParticipants = participants.Keys.Where<string>((Func<string, bool>) (pJid => participantsFromHash.Contains(pJid))).ToArray<string>();
              }
              else
              {
                msgParticipants = participantsFromHash.Where<string>((Func<string, bool>) (pJid => pJid != Settings.MyJid)).ToArray<string>();
                List<GroupParticipantState> participantStateList = new List<GroupParticipantState>();
                foreach (string key in msgParticipants)
                {
                  GroupParticipantState participantState = (GroupParticipantState) null;
                  if (participants.TryGetValue(key, out participantState))
                  {
                    if ((participantState.Flags & 2L) == 0L)
                      participantStateList.Add(participantState);
                  }
                  else
                    participantStateList.Add(new GroupParticipantState()
                    {
                      GroupJid = conversation.Jid,
                      MemberJid = key,
                      Flags = 0L
                    });
                }
                participantsNeedingWelcome = participantStateList.ToArray();
                if (participantsNeedingWelcome.Length == 0)
                  participantsNeedingWelcome = (GroupParticipantState[]) null;
                object[] objArray = new object[2]
                {
                  (object) msgParticipants,
                  null
                };
                GroupParticipantState[] participantStateArray = participantsNeedingWelcome;
                objArray[1] = (object) (participantStateArray != null ? participantStateArray.Length : -1);
                Log.l("E2EEncryption", "Revoking message for {0} participants - {1}", objArray);
              }
            }
            isHashCurrent = false;
          }
          else
          {
            msgParticipants = participants.Keys.ToArray<string>();
            participantsNeedingWelcome = participants.Where<KeyValuePair<string, GroupParticipantState>>((Func<KeyValuePair<string, GroupParticipantState>, bool>) (p => (p.Value.Flags & 2L) == 0L)).Select<KeyValuePair<string, GroupParticipantState>, GroupParticipantState>((Func<KeyValuePair<string, GroupParticipantState>, GroupParticipantState>) (p => p.Value)).ToArray<GroupParticipantState>();
          }
          if (!msg.IsStatus())
            return;
          MessageProperties forMessage = MessageProperties.GetForMessage(msg);
          forMessage.EnsureCommonProperties.SentRecipientsCount = new int?(!msg.IsRevoked() || msgParticipants == null ? participants.Count : msgParticipants.Length);
          forMessage.Save();
        }));
      }
      else
        msgParticipants = new string[1]{ msg.KeyRemoteJid };
      messageParticipants.IsHashCurrent = isHashCurrent;
      if (msgParticipants != null)
        messageParticipants.Participants = msgParticipants;
      if (participantsNeedingWelcome != null)
        messageParticipants.ParticipantsNeedingWelcome = participantsNeedingWelcome;
      return messageParticipants;
    }

    public void OnLocationSharingDisabled(string gJid)
    {
      Log.l("E2EEncryption", "live location disabled, clearing LocationSenderKey | {0}", (object) gJid);
      this.store.FastRatchetSenderKeyStoreRemoveKey(gJid, Settings.MyJid);
    }

    private bool CodecIsE2eElligble(Message message)
    {
      switch (message.MediaWaType)
      {
        case FunXMPP.FMessage.Type.Audio:
          string mimeType = message.MediaMimeType;
          if (!message.UploadContext.isType(UploadContext.UploadContextType.Streaming))
          {
            try
            {
              string mimeType1 = CodecDetector.DetectAudioCodec(message.LocalFileUri).MimeType;
              if (mimeType1 != null)
                mimeType = mimeType1;
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "codec detector");
            }
          }
          if (mimeType != message.MediaMimeType)
          {
            Log.l("codec detector", "Rewriting mime type: [{0}] => [{1}]", (object) message.MediaMimeType, (object) mimeType);
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              message.MediaMimeType = mimeType;
              db.SubmitChanges();
            }));
          }
          return ((IEnumerable<string>) WhatsApp.ProtoBuf.Message.SupportedAudioTypes).Contains<string>(mimeType);
        case FunXMPP.FMessage.Type.Video:
        case FunXMPP.FMessage.Type.Gif:
          message.RecomputeVideoDimensions();
          return CodecDetector.GetCodecInfo(message.LocalFileUri, false) == CodecInfo.SupportedCodec;
        default:
          return true;
      }
    }

    public ShouldEncryptResult ShouldEncryptMessage(Message message)
    {
      try
      {
        if (!this.CodecIsE2eElligble(message))
          return ShouldEncryptResult.Fail;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "codec detection");
        return ShouldEncryptResult.Fail;
      }
      return ShouldEncryptResult.Encrypted;
    }

    public bool ShouldSendMessageRetry(MessagesContext db, Message msg, string participantJid)
    {
      if (msg.MediaWaType == FunXMPP.FMessage.Type.Revoked)
        return true;
      if (db.GetReceiptsForMessage(msg.MessageID, recipientJid: participantJid).Where<ReceiptState>((Func<ReceiptState, bool>) (r => r.Status == FunXMPP.FMessage.Status.ReceivedByTarget)).Any<ReceiptState>())
      {
        Log.l("E2EEncryption", "RetryFromTarget Rejected - Reason: Already Received");
        new E2eRetryAfterDelivery().SaveEvent();
        return false;
      }
      bool flag = false;
      if (!JidHelper.IsMultiParticipantsChatJid(msg.KeyRemoteJid))
      {
        flag = true;
      }
      else
      {
        Conversation conversation = db.GetConversation(msg.KeyRemoteJid, CreateOptions.None);
        if (conversation != null)
        {
          string[] participantJids = conversation.GetParticipantJids();
          Set<string> participantsFromHash = conversation.GetParticipantsFromHash(db, msg.ParticipantsHash);
          if (participantsFromHash != null && participantJids != null)
          {
            flag = participantsFromHash.Contains(participantJid) && ((IEnumerable<string>) participantJids).Contains<string>(participantJid);
            if (!flag)
              Log.l("E2EEncryption", "RetryFromTarget Rejected - Reason: Not in ParticipantHash");
          }
        }
      }
      return flag;
    }

    public void SendIndividualRetryForGroup(
      string keyRemoteJid,
      string keyId,
      string participantJid,
      int count,
      Action onCompleteSend)
    {
      bool retryMessage = false;
      Message message = (Message) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        GroupParticipantState participant = db.GetParticipant(keyRemoteJid, participantJid);
        message = db.GetMessage(keyRemoteJid, keyId, true);
        if (message == null || participant == null || !this.ShouldSendMessageRetry(db, message, participantJid))
          return;
        retryMessage = true;
      }));
      if (retryMessage)
      {
        Log.l("E2EEncryption", "Attempting to send an individual retry message: " + message.KeyRemoteJid + " - " + message.KeyId + " - Member: " + participantJid);
        Axolotl.GroupEncryptionInfo groupInfo = new Axolotl.GroupEncryptionInfo(participantJid, count);
        Action retryAction = (Action) (() => this.SendIndividualRetryForGroup(keyRemoteJid, keyId, participantJid, count, onCompleteSend));
        FunXMPP.FMessage message1 = (FunXMPP.FMessage) null;
        lock (this.EncryptionLock)
          message1 = this.EncryptIndividualMessage(message, retryAction, groupInfo);
        if (message1 != null)
        {
          Log.l("E2EEncryption", "Sending an individual retry message: " + message.KeyRemoteJid + " - " + message.KeyId + " - Member: " + participantJid);
          message1.remote_resource = participantJid;
          if (message1.encrypted != null)
            message1.encrypted[0].cipher_retry_count = count;
          AppState.GetConnection().SendGroupEncryptionMessageWithListener(message1, onCompleteSend);
        }
        else
        {
          if (!groupInfo.MaxRetriesReached)
            return;
          Log.LogException((Exception) new Axolotl.AxolotlGroupException(), "Individual Retry For Group Failed, Max Retries Reached");
          onCompleteSend();
        }
      }
      else
      {
        Log.l("E2EEncrypt", "Message not found or eligible for Individual Retry");
        onCompleteSend();
      }
    }

    public void MessageRetryFromTarget(Message message, int version, uint registrationId)
    {
      this.ConfirmRecipientRegistration(message.KeyRemoteJid, new uint?(registrationId));
    }

    public bool ConfirmRecipientRegistration(string recipientId, uint? registrationId)
    {
      bool flag = false;
      if (this.store.SessionContainsSession(recipientId))
      {
        byte[] aliceBaseKey = (byte[]) null;
        int remoteRegistrationId;
        this.GetSessionInfo(recipientId, out remoteRegistrationId, out aliceBaseKey);
        if (!registrationId.HasValue || (long) remoteRegistrationId != (long) registrationId.Value)
        {
          Log.l("E2EEncryption", "Target has different RegistrationId than local for Jid: " + recipientId);
          this.Store.SessionDeleteAllSessions(recipientId);
          flag = true;
        }
      }
      return flag;
    }

    public void MessageRetryCleanup(Message message, MessagesContext db)
    {
      if (message.GetCipherRetryCount(db) <= 0)
        return;
      Log.l("E2EEncryption", "Deleting MessageBaseKey for Jid: " + message.KeyRemoteJid + " KeyId: " + message.KeyId);
      this.Store.DeleteMessageBaseKey(message.KeyRemoteJid, message.KeyFromMe, message.KeyId);
    }

    public static void IdentityChangedForUser(string keyRemoteJid)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        GroupParticipantState[] participantsInCommonGroups = db.GetGroupParticipantsInCommonGroups(keyRemoteJid);
        if (participantsInCommonGroups != null && participantsInCommonGroups.Length != 0)
        {
          foreach (GroupParticipantState participantState in participantsInCommonGroups)
            participantState.Flags &= -3L;
          db.SubmitChanges();
        }
        if (!Settings.E2EVerificationEnabled)
          return;
        Message identityChanged1 = SystemMessageUtils.CreateIdentityChanged(db, keyRemoteJid, (string) null);
        db.InsertMessageOnSubmit(identityChanged1);
        Conversation[] groupsInCommon = db.GetGroupsInCommon(new string[1]
        {
          keyRemoteJid
        });
        if (groupsInCommon != null && ((IEnumerable<Conversation>) groupsInCommon).Any<Conversation>())
        {
          foreach (Conversation conversation in groupsInCommon)
          {
            Message identityChanged2 = SystemMessageUtils.CreateIdentityChanged(db, conversation.Jid, keyRemoteJid);
            db.InsertMessageOnSubmit(identityChanged2);
          }
        }
        db.SubmitChanges();
      }));
    }

    public IObservable<bool> RecipientHasKeys(string recipient)
    {
      return Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
      {
        if (this.Store.SessionContainsSession(recipient))
          observer.OnNext(true);
        else
          this.SendGetPreKey(recipient, (Action) (() => observer.OnNext(this.Store.SessionContainsSession(recipient))));
        return (Action) (() => { });
      }));
    }

    public static void CallKeysFromCipherKeyV1(
      byte[] cipherKey,
      out byte[] callerSRTPBytes,
      out byte[] calleeSRTPBytes,
      out byte[] callerP2PBytes,
      out byte[] calleeP2PBytes)
    {
      byte[] sourceArray = HkdfSha256.Perform(92, cipherKey, info: AxolotlMediaCipher.CallHKDFInfo);
      callerSRTPBytes = new byte[30];
      calleeSRTPBytes = new byte[30];
      callerP2PBytes = new byte[16];
      calleeP2PBytes = new byte[16];
      Array.Copy((Array) sourceArray, 0, (Array) callerSRTPBytes, 0, callerSRTPBytes.Length);
      Array.Copy((Array) sourceArray, callerSRTPBytes.Length, (Array) calleeSRTPBytes, 0, calleeSRTPBytes.Length);
      Array.Copy((Array) sourceArray, callerSRTPBytes.Length + calleeSRTPBytes.Length, (Array) callerP2PBytes, 0, callerP2PBytes.Length);
      Array.Copy((Array) sourceArray, callerSRTPBytes.Length + calleeSRTPBytes.Length + callerP2PBytes.Length, (Array) calleeP2PBytes, 0, calleeP2PBytes.Length);
    }

    public static void CallKeysFromCipherKeyV2(
      string jid,
      byte[] cipherKey,
      out byte[] srtpBytes,
      out byte[] p2pBytes)
    {
      byte[] sourceArray = HkdfSha256.Perform(46, cipherKey, info: Encoding.UTF8.GetBytes(jid));
      srtpBytes = new byte[30];
      p2pBytes = new byte[16];
      Array.Copy((Array) sourceArray, 0, (Array) srtpBytes, 0, srtpBytes.Length);
      Array.Copy((Array) sourceArray, srtpBytes.Length, (Array) p2pBytes, 0, p2pBytes.Length);
    }

    public byte[] DecryptCallKey(FunXMPP.FMessage offerMessage)
    {
      byte[] callKey = (byte[]) null;
      AxolotlSessionCipher.PayloadDecryptedHandler handler = (AxolotlSessionCipher.PayloadDecryptedHandler) ((recipientId, plainText, message) =>
      {
        WhatsApp.ProtoBuf.Message fromPlainText = WhatsApp.ProtoBuf.Message.CreateFromPlainText(plainText);
        if (fromPlainText == null || fromPlainText.CallField == null)
          return;
        callKey = fromPlainText.CallField.CallKey;
      });
      try
      {
        this.DecryptMessage(offerMessage, handler);
      }
      catch (Exception ex)
      {
        Log.l("E2EDecrypt", "Couldn't decrypt Callkeys payload {0}", (object) ex.ToString());
        throw;
      }
      return callKey;
    }

    public IEnumerable<FunXMPP.FMessage.Participant> EncryptLiveLocationKeys(
      IEnumerable<string> jidsRequiringKey,
      Action retryAction)
    {
      IEnumerable<string> strings = jidsRequiringKey.Where<string>((Func<string, bool>) (p => !this.Store.SessionContainsSession(p)));
      if (strings.Count<string>() > 0)
      {
        this.SendGetMultiplePreKeys(LiveLocationManager.LocationGroupJid, strings, retryAction);
        return (IEnumerable<FunXMPP.FMessage.Participant>) null;
      }
      List<FunXMPP.FMessage.Participant> participantList = new List<FunXMPP.FMessage.Participant>();
      WhatsApp.ProtoBuf.Message message = new WhatsApp.ProtoBuf.Message()
      {
        FastRatchetKeySenderKeyDistributionMessage = new WhatsApp.ProtoBuf.Message.SenderKeyDistributionMessage()
        {
          GroupId = LiveLocationManager.LocationGroupJid,
          AxolotlSenderKeyDistributionMessage = this.GroupCreateSenderKeyBundle(LiveLocationManager.LocationGroupJid, true)
        }
      };
      foreach (string str in jidsRequiringKey)
      {
        FunXMPP.FMessage.Participant participant = new FunXMPP.FMessage.Participant(str);
        using (AxolotlSessionCipher axolotlSessionCipher = new AxolotlSessionCipher(this, str))
        {
          byte[] CipherText = (byte[]) null;
          FunXMPP.FMessage.Encrypted encrypted = new FunXMPP.FMessage.Encrypted();
          try
          {
            AxolotlCiphertextType CipherTextType;
            axolotlSessionCipher.Encrypt(message.ToPlainText(), out CipherTextType, out CipherText);
            encrypted.cipher_text_type = Axolotl.CiphertextTypeString(CipherTextType);
            encrypted.cipher_text_bytes = CipherText;
            encrypted.cipher_version = 2;
            participant.Encrypted = encrypted;
            Log.l("E2EEncryption", "Successfully encrypted location key message type: " + encrypted.cipher_text_type + " - " + str);
          }
          catch (Exception ex)
          {
            Log.l("E2EEncryption", "Failed encrypted location key message: " + str);
            participant = (FunXMPP.FMessage.Participant) null;
          }
        }
        if (participant != null)
          participantList.Add(participant);
      }
      return (IEnumerable<FunXMPP.FMessage.Participant>) participantList;
    }

    public void Reset()
    {
      Log.l("E2EEncryption", "Database Reset");
      this.Store.Reset();
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Axolotl.ClearGroupParticipantStateSenderKey(db);
        db.SubmitChanges();
      }));
    }

    public static void ClearGroupParticipantStateSenderKey(MessagesContext db)
    {
      foreach (GroupParticipantState groupParticipant in db.GetAllGroupParticipants())
        groupParticipant.Flags &= -3L;
    }

    public static void Close()
    {
      Axolotl axolotl = (Axolotl) null;
      if (AppState.GetConnection() != null)
        axolotl = AppState.GetConnection().EncryptionNoInitialize;
      if (axolotl == null)
        return;
      lock (axolotl.EncryptionLock)
        axolotl.Store.DisposeDatabase();
    }

    private void SessionProcessPreKeyBundle(
      string recipientId,
      int registrationId,
      byte type,
      byte[] identity,
      byte[] preKeyData,
      int preKeyId,
      byte[] signedPreKeyData,
      int signedPreKeyId,
      byte[] signedPreKeySignature)
    {
      IByteBuffer instance1 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      IByteBuffer instance2 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      IByteBuffer instance3 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      IByteBuffer byteBuffer = (IByteBuffer) null;
      instance1.Put(identity);
      instance2.Put(signedPreKeyData);
      instance3.Put(signedPreKeySignature);
      if (preKeyData != null)
      {
        byteBuffer = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
        byteBuffer.Put(preKeyData);
      }
      this.native.SessionProcessPreKeyBundle(recipientId, registrationId, type, instance1, byteBuffer, preKeyId, instance2, signedPreKeyId, instance3);
    }

    public void GetSessionInfo(
      string recipientId,
      out int remoteRegistrationId,
      out byte[] aliceBaseKey)
    {
      IByteBuffer AliceBaseKey = (IByteBuffer) null;
      this.native.GetSessionInfo(recipientId, out remoteRegistrationId, out AliceBaseKey);
      aliceBaseKey = AliceBaseKey.Get();
    }

    public byte[] GetIdentityKeyForSending()
    {
      IByteBuffer IdentityKey = (IByteBuffer) null;
      this.native.GetIdentityKeyForSending(out IdentityKey);
      return IdentityKey.Get();
    }

    public byte[] GetUnsentPreKeys(int limit)
    {
      IByteBuffer UnsentPreKeysBuffer = (IByteBuffer) null;
      this.native.GetUnsentPreKeys(limit, out UnsentPreKeysBuffer);
      return UnsentPreKeysBuffer.Get();
    }

    public byte[] GetLatestSignedPreKey()
    {
      IByteBuffer SignedPreKeyBuffer = (IByteBuffer) null;
      this.native.GetLatestSignedPreKey(out SignedPreKeyBuffer);
      return SignedPreKeyBuffer.Get();
    }

    private void GroupProcessSenderKeyBundle(
      string groupid,
      string recipientId,
      bool fastRatchet,
      byte[] senderKeyDistributionMessage)
    {
      IByteBuffer instance = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      instance.Put(senderKeyDistributionMessage);
      this.native.GroupProcessSenderKeyBundle(groupid, recipientId, fastRatchet, instance);
    }

    public byte[] GroupCreateSenderKeyBundle(string groupid, bool fastRatchet)
    {
      Log.l("E2EEncryption", "Creating SenderKeyBundle for: " + groupid);
      return this.native.GroupCreateSenderKeyBundle(groupid, Settings.MyJid, fastRatchet).Get();
    }

    public void IdentityGetFingerprint(
      string RecipientId,
      out string Displayable,
      out string Scannable)
    {
      Displayable = (string) null;
      Scannable = (string) null;
      try
      {
        byte[] ScannableBytes = (byte[]) null;
        this.IdentityGetFingerprintBytes(RecipientId, out Displayable, out ScannableBytes);
        Scannable = Encoding.GetEncoding("ISO-8859-1").GetString(ScannableBytes, 0, ScannableBytes.Length);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Trying to get fingerprint");
      }
    }

    public void IdentityGetFingerprintBytes(
      string RecipientId,
      out string Displayable,
      out byte[] ScannableBytes)
    {
      Displayable = (string) null;
      ScannableBytes = (byte[]) null;
      try
      {
        Regex regex = new Regex("^([17]|2[07]|3[0123469]|4[013456789]|5[12345678]|6[0123456]|8[1246]|9[0123458]|\\d{3})\\d*?(\\d{4,6})$");
        Match match1 = regex.Match(Settings.ChatID);
        Match match2 = regex.Match(JidHelper.GetPhoneNumber(RecipientId, false));
        string MyStableId = match1.Groups[1].Value + match1.Groups[2].Value;
        string RecipientStableId = match2.Groups[1].Value + match2.Groups[2].Value;
        IByteBuffer Scannable = (IByteBuffer) null;
        this.native.IdentityGetFingerprint(MyStableId, RecipientId, RecipientStableId, out Displayable, out Scannable);
        ScannableBytes = Scannable.Get();
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Trying to get fingerprint bytes");
      }
    }

    public Axolotl.IdentityVerificationResult IdentityVerifyFingerprint(
      string expected,
      byte[] scanned)
    {
      Axolotl.IdentityVerificationResult verificationResult = Axolotl.IdentityVerificationResult.NoMatch;
      try
      {
        byte[] bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(expected);
        IByteBuffer instance1 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
        instance1.Put(bytes);
        IByteBuffer instance2 = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
        instance2.Put(scanned);
        verificationResult = (Axolotl.IdentityVerificationResult) this.native.IdentityVerifyFingerprint(instance1, instance2);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Trying to get fingerprint");
      }
      return verificationResult;
    }

    public static byte[] GenerateRandomBytes(int length)
    {
      byte[] data = new byte[length];
      new RNGCryptoServiceProvider().GetBytes(data);
      return data;
    }

    public static string CiphertextTypeString(AxolotlCiphertextType type)
    {
      switch (type)
      {
        case AxolotlCiphertextType.CiphertextMessage:
          return "msg";
        case AxolotlCiphertextType.CiphertextPreKeyMessage:
          return "pkmsg";
        case AxolotlCiphertextType.CiphertextSenderKeyMessage:
          return "skmsg";
        default:
          return (string) null;
      }
    }

    public byte[] GetPublicKey(string recipientId, bool quiet = false)
    {
      byte[] publicKey = this.Store.IdentityGetPublicKey(recipientId, quiet);
      return publicKey == null ? (byte[]) null : ((IEnumerable<byte>) publicKey).Skip<byte>(1).ToArray<byte>();
    }

    private bool ShouldSendGetPreKey
    {
      get
      {
        if (Settings.KeydBackoffUtc.HasValue)
        {
          DateTime? keydBackoffUtc = Settings.KeydBackoffUtc;
          DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
          if ((keydBackoffUtc.HasValue ? (keydBackoffUtc.GetValueOrDefault() < currentServerTimeUtc ? 1 : 0) : 0) == 0)
          {
            this.EnsureKeydBackoffTimer();
            return false;
          }
        }
        return true;
      }
    }

    public void IncrementKeydBackoff()
    {
      uint num = Utils.CalculateFibonacci(Settings.KeydBackoffAttempt);
      if (num > 610U)
        num = 610U;
      else
        ++Settings.KeydBackoffAttempt;
      Settings.KeydBackoffUtc = new DateTime?(FunRunner.CurrentServerTimeUtc.AddSeconds((double) num));
    }

    public void ResetKeydBackoff()
    {
      this.KeydBackoffTimerCallback((object) null);
      Settings.KeydBackoffAttempt = 0;
      Settings.KeydBackoffUtc = new DateTime?();
    }

    private void EnsureKeydBackoffTimer()
    {
      lock (this.GetPreKeyLock)
      {
        if (this.KeydBackoffTimer != null)
          return;
        long totalMilliseconds = (long) (Settings.KeydBackoffUtc.Value - FunRunner.CurrentServerTimeUtc).TotalMilliseconds;
        if (totalMilliseconds < 0L)
        {
          this.KeydBackoffTimerCallback((object) null);
        }
        else
        {
          if (totalMilliseconds > 610000L)
          {
            Log.l("E2EEncryption", "Duetime {0}, KeyBackOff {1}, Server {2}", (object) totalMilliseconds, (object) Settings.KeydBackoffUtc, (object) FunRunner.CurrentServerTimeUtc);
            Log.SendCrashLog((Exception) new InvalidDataException("Unexpected keyd backoff"), "Unexpected keyd backoff", logOnlyForRelease: true);
          }
          long dueTime = Math.Min(totalMilliseconds, 610000L);
          Log.d("E2EEncryption", "backoff for {0} msecs", (object) dueTime);
          this.KeydBackoffTimer = new System.Threading.Timer(new TimerCallback(this.KeydBackoffTimerCallback), (object) this, dueTime, -1L);
        }
      }
    }

    private void KeydBackoffTimerCallback(object state)
    {
      bool flag = false;
      lock (this.GetPreKeyLock)
      {
        if (this.KeydBackoffTimer != null || Settings.KeydBackoffUtc.HasValue)
          flag = true;
        this.KeydBackoffTimer.SafeDispose();
        this.KeydBackoffTimer = (System.Threading.Timer) null;
      }
      if (!flag)
        return;
      AppState.Worker.Enqueue((Action) (() => AppState.ProcessPendingNetworkTasks()));
    }

    public class AxolotlDecryptException : Exception
    {
    }

    public class AxolotlMediaDecryptException : Exception
    {
    }

    public class AxolotlEncryptException : Exception
    {
    }

    public class AxolotlEncryptMaxRetriesException : Exception
    {
    }

    public class AxolotlMediaEncryptException : Exception
    {
    }

    public class AxolotlPaddingException : Exception
    {
    }

    public class AxolotlProtocolBufferException : Exception
    {
    }

    public class AxolotlGroupException : Exception
    {
    }

    public class AxolotlRegistrationException : Exception
    {
    }

    public class DecryptRetryException : Exception
    {
      public int RetryCount;
    }

    public class DecryptUnknownTagsException : Exception
    {
      public uint[] UnknownTags;
    }

    public class DecryptInvalidProtocolBufferException : Exception
    {
    }

    public class MessageParticipants
    {
      public string[] Participants;
      public GroupParticipantState[] ParticipantsNeedingWelcome;
      public bool IsHashCurrent;

      public MessageParticipants()
      {
        this.Participants = new string[0];
        this.ParticipantsNeedingWelcome = new GroupParticipantState[0];
        this.IsHashCurrent = true;
      }
    }

    public delegate void MessageDecryptedHandler(FunXMPP.FMessage message);

    public class GroupEncryptionInfo
    {
      public string Participant;
      public int RetryCount;
      public bool MaxRetriesReached;

      public GroupEncryptionInfo(string participant, int retryCount)
      {
        this.Participant = participant;
        this.RetryCount = retryCount;
      }
    }

    public enum IdentityVerificationResult
    {
      IdentityMismatchContact = -12011, // 0xFFFFD115
      IdentityMismatchYou = -12010, // 0xFFFFD116
      IdentityMismatch = -1201, // 0xFFFFFB4F
      VersionMismatch = -1200, // 0xFFFFFB50
      NoMatch = 0,
      Match = 1,
    }
  }
}
