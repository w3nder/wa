// Decompiled with JetBrains decompiler
// Type: WhatsApp.VoipSignaling
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class VoipSignaling : IVoipSignalingCallbacks, FunXMPP.VoipListener
  {
    public const string LogManaged = "voip signaling";
    private const string CallIdPrefix = "call:";
    private static ISignalingStruct signalInstance = (ISignalingStruct) null;
    private static CallCoalesceTable coalesceTable = new CallCoalesceTable();
    private static bool offlineBurstInProgress = false;
    private static Dictionary<string, VoipSignaling.PendingOfferState> offerTable = new Dictionary<string, VoipSignaling.PendingOfferState>();
    private static VoipSignaling instance;

    public static VoipSignaling Instance
    {
      get
      {
        return Utils.LazyInit<VoipSignaling>(ref VoipSignaling.instance, (Func<VoipSignaling>) (() => new VoipSignaling()));
      }
    }

    public static VoipSignaling.PendingOfferState GetPendingOffer(
      string jid,
      string callId,
      bool pop = false)
    {
      string key = CallCoalesceTable.KeyForCall(jid, callId);
      VoipSignaling.PendingOfferState pendingOffer = (VoipSignaling.PendingOfferState) null;
      bool flag;
      if (!(flag = VoipSignaling.offerTable.TryGetValue(key, out pendingOffer)) && !pop)
      {
        pendingOffer = new VoipSignaling.PendingOfferState();
        VoipSignaling.offerTable[key] = pendingOffer;
      }
      else if (pop & flag)
        VoipSignaling.offerTable.Remove(key);
      return pendingOffer;
    }

    public static void OnConnected()
    {
      Voip.Worker.Enqueue((Action) (() =>
      {
        VoipSignaling.coalesceTable.Clear();
        VoipSignaling.offlineBurstInProgress = true;
      }));
    }

    public static void OnDisconnected()
    {
      Voip.Worker.Enqueue((Action) (() => VoipSignaling.coalesceTable.Clear()));
    }

    public static void OnOfflineCompleted()
    {
      Voip.Worker.Enqueue((Action) (() =>
      {
        VoipSignaling.coalesceTable.ProcessAll();
        VoipSignaling.offlineBurstInProgress = false;
        if (!AppState.IsDecentMemoryDevice)
          return;
        if (AppState.IsBackgroundAgent)
          return;
        try
        {
          Voip.Instance.GetCallbacks();
        }
        catch (Exception ex)
        {
          Log.l("voip signaling", "Voip.Instance failed during deferred voip stack initialization");
        }
      }));
    }

    public void HandleVoipOfferReceipt(FunXMPP.ProtocolTreeNode node)
    {
      Voip.Worker.Enqueue((Action) (() => this.HandleOfferReceiptImpl(node)));
    }

    private void HandleOfferReceiptImpl(FunXMPP.ProtocolTreeNode node)
    {
      string attributeValue1 = node.GetAttributeValue("from");
      FunXMPP.ProtocolTreeNode protocolTreeNode = ((IEnumerable<FunXMPP.ProtocolTreeNode>) (node.children ?? new FunXMPP.ProtocolTreeNode[0])).FirstOrDefault<FunXMPP.ProtocolTreeNode>();
      if (protocolTreeNode == null)
        return;
      string attributeValue2 = protocolTreeNode.GetAttributeValue("call-id");
      if (attributeValue2 == null)
        return;
      FunXMPP.ProtocolTreeNode child = protocolTreeNode.GetChild("client");
      if (child == null)
        return;
      ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
      signalStruct.SetMessageType(SignalingMessageType.OfferReceipt);
      signalStruct.SetCallId(attributeValue2);
      signalStruct.SetPeerJid(attributeValue1);
      VoipSignaling.OnIncomingSignalData(signalStruct);
      int? attributeInt = child.GetAttributeInt("callee_bad_asn");
      int num1 = 1;
      if (attributeInt.GetValueOrDefault() != num1)
        return;
      int num2 = attributeInt.HasValue ? 1 : 0;
    }

    public void HandleVoipNode(FunXMPP.ProtocolTreeNode node)
    {
      Voip.Worker.Enqueue((Action) (() => this.HandleVoipNodeImpl(node)));
    }

    private FunXMPP.FMessage FMessageFromCallOfferKeys(
      string senderJid,
      string callId,
      string cipherType,
      int cipherVersion,
      int retryCount,
      byte[] encryptedBytes)
    {
      FunXMPP.FMessage fmessage = new FunXMPP.FMessage(new FunXMPP.FMessage.Key(senderJid, false, "call:" + callId));
      fmessage.status = FunXMPP.FMessage.Status.NeverSend;
      fmessage.media_wa_type = FunXMPP.FMessage.Type.CallOffer;
      fmessage.encrypted = new FunXMPP.FMessage.Encrypted[1]
      {
        new FunXMPP.FMessage.Encrypted()
        {
          cipher_text_type = cipherType,
          cipher_text_bytes = encryptedBytes,
          cipher_version = cipherVersion,
          cipher_retry_count = retryCount
        }
      };
      return fmessage;
    }

    private AudioDriver PreferredAudioDriver
    {
      get
      {
        AudioDriver preferredAudioDriver = AppState.IsWP10OrLater || !VoipCallParams.MsftSoftwareECNeeded ? AudioDriver.Wasapi : AudioDriver.WinMM;
        Log.l("voip signaling", "PreferredAudioDriver = {0}, IsWP10OrLater {1}, MsftSoftwareECNeeded {2}", (object) preferredAudioDriver, (object) AppState.IsWP10OrLater, (object) VoipCallParams.MsftSoftwareECNeeded);
        return preferredAudioDriver;
      }
    }

    private void OnE2EDecryptFailed(int retryCount, byte[] registration)
    {
      if (retryCount < 2)
        return;
      if (registration != null)
      {
        if ((int) FunXMPP.Connection.UIntFromBytes(registration) == (int) AppState.GetConnection().Encryption.Store.LocalRegistrationId)
          return;
        AppState.GetConnection().Encryption.SendPreKeysToServer();
      }
      else
        Log.l("voip signaling", "Missing registration element with retry count >= 2");
    }

    private void SendReceipt(string to, string id, string tag, string callId)
    {
      AppState.GetConnection().SendReceipt(to, (string) null, id, (string) null, new FunXMPP.ProtocolTreeNode(tag, new FunXMPP.KeyValue[1]
      {
        new FunXMPP.KeyValue("call-id", callId)
      }));
    }

    private void SendAck(string to, string id, string tag, FunXMPP.KeyValue[] additionalAttrs = null)
    {
      FunXMPP.Connection connection = AppState.GetConnection();
      List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>()
      {
        new FunXMPP.KeyValue(nameof (to), to),
        new FunXMPP.KeyValue("class", "call"),
        new FunXMPP.KeyValue("type", tag),
        new FunXMPP.KeyValue(nameof (id), id)
      };
      FunXMPP.ProtocolTreeNode child = (FunXMPP.ProtocolTreeNode) null;
      if (additionalAttrs != null && ((IEnumerable<FunXMPP.KeyValue>) additionalAttrs).Count<FunXMPP.KeyValue>() > 0)
        child = new FunXMPP.ProtocolTreeNode(tag, ((IEnumerable<FunXMPP.KeyValue>) additionalAttrs).ToArray<FunXMPP.KeyValue>());
      if (child != null)
        connection.SendRawNode(new FunXMPP.ProtocolTreeNode("ack", keyValueList.ToArray(), child));
      else
        connection.SendRawNode(new FunXMPP.ProtocolTreeNode("ack", keyValueList.ToArray()));
    }

    private void ParseOffer(
      string callId,
      string id,
      string from,
      DateTime? t,
      FunXMPP.ProtocolTreeNode callNode,
      FunXMPP.ProtocolTreeNode offerNode)
    {
      ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
      ManagedCallProperties? nullable = new ManagedCallProperties?();
      int? attributeInt1 = callNode.GetAttributeInt("e");
      int num1 = attributeInt1 ?? 0;
      string peerPlatform = callNode.GetAttributeValue("platform");
      string peerVersion = callNode.GetAttributeValue("version");
      bool flag1 = true;
      bool flag2 = false;
      bool flag3 = false;
      int num2 = 0;
      bool flag4 = VoipSignaling.CheckOfferDupe(callId, from);
      ManagedCallProperties managedCallProperties = new ManagedCallProperties();
      VoipSignaling.SetLastCallProperties(from, ref managedCallProperties);
      PlatformCallParams callParams = new PlatformCallParams()
      {
        EnableLowDataUsage = Settings.LowBandwidthVoip,
        UseSoftwareEC = VoipCallParams.MsftSoftwareECNeeded,
        AudioDriver = this.PreferredAudioDriver
      };
      Capabilities caps = (Capabilities) 0;
      int num3 = 0;
      signalStruct.SetMessageType(SignalingMessageType.Offer);
      signalStruct.SetCallId(callId);
      this.ParseCallParams(signalStruct, offerNode, callParams, true);
      VoipSignaling.FillVoipParams(signalStruct, ref managedCallProperties);
      signalStruct.SetPeerPlatform(this.ParsePeerPlatformName(peerPlatform));
      string attributeValue1 = offerNode.GetAttributeValue("call-creator");
      if (attributeValue1 != null)
        signalStruct.SetCallCreatorJid(attributeValue1);
      string attributeValue2 = offerNode.GetAttributeValue("resume");
      if (attributeValue2 != null)
        signalStruct.SetResume(attributeValue2 == "true");
      signalStruct.SetDeviceName(DeviceNameSide.Caller, offerNode.GetAttributeValue("device") ?? "");
      foreach (FunXMPP.ProtocolTreeNode protocolTreeNode in offerNode.children ?? new FunXMPP.ProtocolTreeNode[0])
      {
        if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "audio"))
          caps |= VoipSignaling.ParseCapabilities(protocolTreeNode);
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "video"))
        {
          int videoParamsCount = signalStruct.GetVideoParamsCount();
          signalStruct.SetVideoParamsCount(videoParamsCount + 1);
          signalStruct.SetVideoParamsIdx(videoParamsCount, VoipSignaling.ParseVideoParams(protocolTreeNode));
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "te"))
        {
          try
          {
            signalStruct.SetTransportCount(num3 + 1);
          }
          catch (ArgumentException ex)
          {
            Log.l("voip signaling", "Transport candidates above the limit of " + (object) num3);
            continue;
          }
          VoipSignaling.SetTransportCandidate(num3++, signalStruct, protocolTreeNode);
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "relay"))
          VoipSignaling.ParseRelay(protocolTreeNode, signalStruct);
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "rte"))
          signalStruct.SetReflexiveAddress(protocolTreeNode.data);
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "net"))
        {
          int? attributeInt2 = protocolTreeNode.GetAttributeInt("medium");
          if (attributeInt2.HasValue)
            signalStruct.SetNetworkMedium((VoipNetworkType) attributeInt2.Value);
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "capability"))
        {
          int? attributeInt3 = protocolTreeNode.GetAttributeInt("ver");
          if (attributeInt3.HasValue && attributeInt3.Value > 0)
            signalStruct.SetCapability(attributeInt3.Value, protocolTreeNode.data);
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "enc"))
        {
          Axolotl encryption = AppState.GetConnection().Encryption;
          string attributeValue3 = protocolTreeNode.GetAttributeValue("type");
          attributeInt1 = protocolTreeNode.GetAttributeInt("count");
          int num4 = attributeInt1 ?? 0;
          attributeInt1 = protocolTreeNode.GetAttributeInt("v");
          int cipherVersion = attributeInt1 ?? 2;
          signalStruct.SetRetryCount((uint) num4);
          FunXMPP.ProtocolTreeNode child = offerNode.GetChild("registration");
          try
          {
            signalStruct.SetCallKey(encryption.DecryptCallKey(this.FMessageFromCallOfferKeys(from, callId, attributeValue3, cipherVersion, num4, protocolTreeNode.data)));
          }
          catch (Axolotl.DecryptRetryException ex)
          {
            Log.l(nameof (VoipSignaling), "Decrypt failed with retry {0}", (object) ex.RetryCount);
            num2 = ex.RetryCount;
            flag2 = true;
            flag3 = true;
            this.OnE2EDecryptFailed(num2, child?.data);
          }
          catch (Exception ex)
          {
            Log.l(nameof (VoipSignaling), "Decrypt failed exception: {0}", (object) ex.ToString());
            flag2 = true;
          }
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "encopt"))
        {
          int? attributeInt4 = protocolTreeNode.GetAttributeInt("keygen");
          if (attributeInt4.HasValue)
            signalStruct.SetKeyGenVersion((uint) attributeInt4.Value);
        }
        else if (VoipSignaling.HandleVoipParamsFromProtoTree(protocolTreeNode, false, ref managedCallProperties))
        {
          managedCallProperties.ElapsedServerTime = num1 * 1000;
          nullable = new ManagedCallProperties?(managedCallProperties);
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "group_info"))
          VoipSignaling.ParseGroupInfo(protocolTreeNode, signalStruct);
        else
          Log.l("voip signaling", "Unrecognized offer element: {0}", protocolTreeNode != null ? (object) protocolTreeNode.tag : (object) "<null>");
      }
      if (managedCallProperties.ForbidP2PForNonContact)
      {
        string phoneNumber = JidHelper.GetPhoneNumber(from, false);
        Contact contact = (Contact) null;
        bool contactChecked = false;
        IObservable<AddressBookSearchArgs> contactByPhoneNumber = AddressBook.Instance.GetContactByPhoneNumber(phoneNumber);
        if (contactByPhoneNumber != null)
        {
          ManualResetEvent mrs = new ManualResetEvent(false);
          contactByPhoneNumber.Subscribe<AddressBookSearchArgs>((Action<AddressBookSearchArgs>) (args =>
          {
            contact = args.Results.FirstOrDefault<Contact>();
            contactChecked = true;
            mrs.Set();
          }), (Action<Exception>) (ex => mrs.Set()), (Action) (() => mrs.Set()));
          mrs.WaitOne(500);
        }
        signalStruct.SetIsNotContact(contact == null);
        if (!contactChecked)
          Log.SendCrashLog((Exception) new ArgumentException("Checking if caller is in callee's address book timeout."), "Address book search for number " + (phoneNumber ?? "<null>") + " wasn't completed on time.");
      }
      signalStruct.SetCapabilities(caps);
      bool hasVideo = signalStruct.GetVideoParamsCount() > 0;
      if (GdprTos.ShouldRejectCalls())
      {
        signalStruct.ClearCallParams();
        signalStruct.SetMessageType(SignalingMessageType.Reject);
        signalStruct.SetPeerJid(from);
        signalStruct.SetCallId(callId);
        signalStruct.SetReason("tos");
        VoipSignaling.WriteVoipNode(signalStruct.GetBuffer());
        flag1 = false;
        VoipHandler.Instance.OnMissedCall(from ?? "", callId ?? "", t.HasValue ? t.Value.ToFileTimeUtc() : 0L, num1 * 1000, true, hasVideo);
      }
      else if (num1 > 45)
      {
        if (VoipSignaling.offlineBurstInProgress)
          VoipSignaling.coalesceTable.OnMissedCall(from, callId, t, num1 * 1000, hasVideo);
        else
          VoipHandler.Instance.OnMissedCall(from, callId, t.HasValue ? t.Value.ToFileTimeUtc() : 0L, num1 * 1000, true, hasVideo);
        flag1 = false;
      }
      else if (flag2 && !flag4)
      {
        signalStruct.ClearCallParams();
        signalStruct.SetMessageType(SignalingMessageType.Reject);
        signalStruct.SetPeerJid(from);
        signalStruct.SetCallId(callId);
        signalStruct.SetReason("enc");
        if (flag3)
        {
          Axolotl encryption = AppState.GetConnection().Encryption;
          if (num2 > 2)
            AppState.SchedulePersistentAction(PersistentAction.SendVerifyAxolotlDigest());
          signalStruct.SetRetryCount((uint) num2);
          byte[] buffer = signalStruct.GetBuffer();
          Action onComplete = (Action) (() => VoipSignaling.WriteVoipNode(buffer));
          encryption.EnqueueToSendPreKeyComplete(onComplete);
        }
        else
          VoipSignaling.WriteVoipNode(signalStruct.GetBuffer());
        if (!flag3 || num2 >= 5)
          VoipHandler.Instance.OnMissedCall(from ?? "", callId ?? "", t.HasValue ? t.Value.ToFileTimeUtc() : 0L, num1 * 1000, true, hasVideo);
        flag1 = false;
      }
      else if (!flag4)
      {
        bool flag5 = false;
        try
        {
          IVoip instance = Voip.Instance;
          CallInfoStruct? callInfo = instance.GetCallInfo();
          if (callInfo.HasValue)
          {
            if (callInfo.Value.CallState != CallState.None)
              goto label_65;
          }
          flag5 = instance.GetCallbacks().IsMsftCallbackRegistered();
        }
        catch (Exception ex)
        {
        }
label_65:
        if (flag5)
        {
          signalStruct.ClearCallParams();
          signalStruct.SetMessageType(SignalingMessageType.Reject);
          signalStruct.SetPeerJid(from);
          signalStruct.SetCallId(callId);
          signalStruct.SetReason("busy");
          VoipSignaling.WriteVoipNode(signalStruct.GetBuffer());
          flag1 = false;
        }
      }
      if (flag1)
      {
        VoipSignaling.PendingOfferState pendingOffer = VoipSignaling.GetPendingOffer(from, callId);
        if (nullable.HasValue)
          pendingOffer.CallProperties = nullable.Value;
        if (peerPlatform != null || peerVersion != null)
        {
          pendingOffer.RemotePlatform = peerPlatform;
          pendingOffer.RemoteAppVersion = peerVersion;
        }
      }
      signalStruct.SetPeerJid(from);
      signalStruct.SetCallId(callId);
      this.SendStanzaToVoip(signalStruct, t, 0, (Action) (() => this.SendReceipt(from, id, offerNode.tag, callId)), (Action) (() => Voip.Instance.SetRemotePlatform(peerPlatform, peerVersion)));
    }

    private void ParseAccept(
      string callId,
      string id,
      string from,
      DateTime? t,
      FunXMPP.ProtocolTreeNode callNode,
      FunXMPP.ProtocolTreeNode acceptNode)
    {
      ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
      string peerPlatform = callNode.GetAttributeValue("platform");
      string peerVersion = callNode.GetAttributeValue("version");
      bool flag1 = false;
      bool flag2 = false;
      int num = 0;
      ManagedCallProperties? callProperties = new ManagedCallProperties?();
      PlatformCallParams callParams1 = new PlatformCallParams()
      {
        EnableLowDataUsage = Settings.LowBandwidthVoip,
        UseSoftwareEC = VoipCallParams.MsftSoftwareECNeeded,
        AudioDriver = this.PreferredAudioDriver
      };
      ManagedCallProperties managedCallProperties = new ManagedCallProperties();
      VoipSignaling.SetLastCallProperties(from, ref managedCallProperties);
      signalStruct.SetMessageType(SignalingMessageType.Accept);
      signalStruct.SetCallId(callId);
      signalStruct.SetPeerPlatform(this.ParsePeerPlatformName(peerPlatform));
      bool callParams2 = this.ParseCallParams(signalStruct, acceptNode, callParams1, false);
      if (callParams2)
      {
        VoipSignaling.FillVoipParams(signalStruct, ref managedCallProperties);
        signalStruct.SetSettingsType(VoipSettingsType.Audio);
      }
      signalStruct.SetDeviceName(DeviceNameSide.Callee, acceptNode.GetAttributeValue("device") ?? "");
      foreach (FunXMPP.ProtocolTreeNode protocolTreeNode in acceptNode.children ?? new FunXMPP.ProtocolTreeNode[0])
      {
        if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "audio") && VoipSignaling.EnforceOnce(ref flag1))
          signalStruct.SetCapabilities(VoipSignaling.ParseCapabilities(protocolTreeNode));
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "video") && VoipSignaling.EnforceOnce(ref flag2))
        {
          signalStruct.SetVideoParams(VoipSignaling.ParseVideoParams(protocolTreeNode));
          if (callParams2)
            signalStruct.SetSettingsType(VoipSettingsType.Video);
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "te"))
        {
          try
          {
            signalStruct.SetTransportCount(num + 1);
          }
          catch (ArgumentException ex)
          {
            Log.l("voip signaling", "Transport candidates above the limit of " + (object) num);
            continue;
          }
          VoipSignaling.SetTransportCandidate(num++, signalStruct, protocolTreeNode);
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "net"))
        {
          int? attributeInt = protocolTreeNode.GetAttributeInt("medium");
          if (attributeInt.HasValue)
            signalStruct.SetNetworkMedium((VoipNetworkType) attributeInt.Value);
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "encopt"))
        {
          int? attributeInt = protocolTreeNode.GetAttributeInt("keygen");
          if (attributeInt.HasValue)
            signalStruct.SetKeyGenVersion((uint) attributeInt.Value);
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "capability"))
        {
          int? attributeInt = protocolTreeNode.GetAttributeInt("ver");
          if (attributeInt.HasValue && attributeInt.Value > 0)
            signalStruct.SetCapability(attributeInt.Value, protocolTreeNode.data);
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "relay"))
        {
          int? attributeInt = protocolTreeNode.GetAttributeInt("transaction-id");
          VoipSignaling.ParseRelay(protocolTreeNode, signalStruct);
          if (attributeInt.HasValue)
            signalStruct.SetTransactionId(attributeInt.Value);
        }
        else if (VoipSignaling.HandleVoipParamsFromProtoTree(protocolTreeNode, true, ref managedCallProperties))
          callProperties = new ManagedCallProperties?(managedCallProperties);
      }
      signalStruct.SetPeerJid(from);
      signalStruct.SetCallId(callId);
      this.SendStanzaToVoip(signalStruct, t, 0, (Action) (() => this.SendReceipt(from, id, acceptNode.tag, callId)), (Action) (() =>
      {
        Voip.Instance.SetRemotePlatform(peerPlatform, peerVersion);
        if (!callProperties.HasValue)
          return;
        Voip.Instance.GetCallbacks().SetManagedCallProperties(Settings.MyJid, callProperties.Value);
      }));
    }

    private void ParsePreaccept(
      string callId,
      string id,
      string from,
      DateTime? t,
      FunXMPP.ProtocolTreeNode callNode,
      FunXMPP.ProtocolTreeNode preacceptNode)
    {
      ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
      bool flag1 = false;
      bool flag2 = false;
      signalStruct.SetMessageType(SignalingMessageType.PreAccept);
      foreach (FunXMPP.ProtocolTreeNode node in preacceptNode.children ?? new FunXMPP.ProtocolTreeNode[0])
      {
        if (FunXMPP.ProtocolTreeNode.TagEquals(node, "audio") && VoipSignaling.EnforceOnce(ref flag1))
          signalStruct.SetCapabilities(VoipSignaling.ParseCapabilities(node));
        else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "video") && VoipSignaling.EnforceOnce(ref flag2))
          signalStruct.SetVideoParams(VoipSignaling.ParseVideoParams(node));
        else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "encopt"))
        {
          int? attributeInt = node.GetAttributeInt("keygen");
          if (attributeInt.HasValue)
            signalStruct.SetKeyGenVersion((uint) attributeInt.Value);
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "capability"))
        {
          int? attributeInt = node.GetAttributeInt("ver");
          if (attributeInt.HasValue && attributeInt.Value > 0)
            signalStruct.SetCapability(attributeInt.Value, node.data);
        }
      }
      signalStruct.SetPeerJid(from);
      signalStruct.SetCallId(callId);
      this.SendStanzaToVoip(signalStruct, t, 0, (Action) (() => this.SendAck(from, id, preacceptNode.tag)));
    }

    private void ParseReject(
      string callId,
      string id,
      string from,
      DateTime? t,
      FunXMPP.ProtocolTreeNode callNode,
      FunXMPP.ProtocolTreeNode rejectNode)
    {
      ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
      string attributeValue = rejectNode.GetAttributeValue("reason");
      signalStruct.SetMessageType(SignalingMessageType.Reject);
      if (!string.IsNullOrEmpty(attributeValue))
      {
        signalStruct.SetReason(attributeValue);
        if (attributeValue == "enc")
        {
          uint? registrationId = new uint?();
          FunXMPP.ProtocolTreeNode child = rejectNode.GetChild("registration");
          uint Count = (uint) (rejectNode.GetAttributeInt("count") ?? 0);
          signalStruct.SetRetryCount(Count);
          if (child != null && child.data != null)
            registrationId = new uint?(FunXMPP.Connection.UIntFromBytes(child.data));
          if (AppState.GetConnection().Encryption.ConfirmRecipientRegistration(from, registrationId))
            AppState.GetConnection().Encryption.SendGetPreKey(from, (Action) (() => { }));
        }
      }
      signalStruct.SetPeerJid(from);
      signalStruct.SetCallId(callId);
      this.SendStanzaToVoip(signalStruct, t, 0, (Action) (() => this.SendReceipt(from, id, rejectNode.tag, callId)));
    }

    private void ParseTerminate(
      string callId,
      string id,
      string from,
      DateTime? t,
      FunXMPP.ProtocolTreeNode callNode,
      FunXMPP.ProtocolTreeNode terminateNode)
    {
      ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
      List<FunXMPP.KeyValue> additionalAckAttrs = new List<FunXMPP.KeyValue>();
      string attributeValue = terminateNode.GetAttributeValue("reason");
      int? attributeInt = terminateNode.GetAttributeInt("videostate");
      signalStruct.SetMessageType(SignalingMessageType.Terminate);
      if (!string.IsNullOrEmpty(attributeValue))
      {
        signalStruct.SetReason(attributeValue);
        if (attributeValue == "relay_bind_failed")
          Voip.Instance.GetCallbacks().PlumbThroughError(from ?? "", callId ?? "", "\0PeerFailed");
      }
      if (attributeInt.HasValue)
        signalStruct.SetVideoState((VideoState) attributeInt.Value);
      try
      {
        IVoip instance = Voip.Instance;
        CallInfoStruct? callInfo = Voip.Instance.GetCallInfo();
        if (callInfo.HasValue)
        {
          if (callInfo.Value.AudioDuration > 0)
            additionalAckAttrs.Add(new FunXMPP.KeyValue("audio_duration", callInfo.Value.AudioDuration.ToString()));
          if (callInfo.Value.VideoDuration > 0)
            additionalAckAttrs.Add(new FunXMPP.KeyValue("video_duration", callInfo.Value.VideoDuration.ToString()));
          if (!string.IsNullOrEmpty(callId))
            additionalAckAttrs.Add(new FunXMPP.KeyValue("call-id", callId));
        }
      }
      catch (Exception ex)
      {
      }
      signalStruct.SetPeerJid(from);
      signalStruct.SetCallId(callId);
      this.SendStanzaToVoip(signalStruct, t, 0, (Action) (() => this.SendAck(from, id, terminateNode.tag, additionalAckAttrs.ToArray())));
    }

    private void ParseRelayLatency(
      string callId,
      string id,
      string from,
      DateTime? t,
      FunXMPP.ProtocolTreeNode callNode,
      FunXMPP.ProtocolTreeNode relayLatencyNode)
    {
      ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
      int num = 0;
      signalStruct.SetMessageType(SignalingMessageType.RelayLatency);
      signalStruct.SetTransactionId(relayLatencyNode.GetAttributeInt("transaction-id") ?? -1);
      foreach (FunXMPP.ProtocolTreeNode node in relayLatencyNode.children ?? new FunXMPP.ProtocolTreeNode[0])
      {
        if (FunXMPP.ProtocolTreeNode.TagEquals(node, "te"))
        {
          try
          {
            signalStruct.SetRelayLatencyCount(num + 1);
          }
          catch (ArgumentException ex)
          {
            Log.l("voip signaling", "Relay count above the limit of " + (object) num);
            continue;
          }
          VoipSignaling.SetRelayLatency(num++, signalStruct, node);
        }
      }
      signalStruct.SetPeerJid(from);
      signalStruct.SetCallId(callId);
      this.SendStanzaToVoip(signalStruct, t, 0, (Action) (() => this.SendAck(from, id, relayLatencyNode.tag)));
    }

    private void ParseRelayElection(
      string callId,
      string id,
      string from,
      DateTime? t,
      FunXMPP.ProtocolTreeNode callNode,
      FunXMPP.ProtocolTreeNode relayElectionNode)
    {
      ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
      bool flag = false;
      signalStruct.SetMessageType(SignalingMessageType.RelayElection);
      foreach (FunXMPP.ProtocolTreeNode node in relayElectionNode.children ?? new FunXMPP.ProtocolTreeNode[0])
      {
        if (FunXMPP.ProtocolTreeNode.TagEquals(node, "te") && VoipSignaling.EnforceOnce(ref flag))
        {
          string attributeValue = node.GetAttributeValue("latency");
          int result = 0;
          if (attributeValue != null)
            int.TryParse(attributeValue, out result);
          signalStruct.SetRelayElection(result, node.data);
        }
      }
      signalStruct.SetPeerJid(from);
      signalStruct.SetCallId(callId);
      this.SendStanzaToVoip(signalStruct, t, 0, (Action) (() => this.SendAck(from, id, relayElectionNode.tag)));
    }

    private void ParseTransport(
      string callId,
      string id,
      string from,
      DateTime? t,
      FunXMPP.ProtocolTreeNode callNode,
      FunXMPP.ProtocolTreeNode transportNode)
    {
      ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
      int num = 0;
      signalStruct.SetMessageType(SignalingMessageType.Transport);
      foreach (FunXMPP.ProtocolTreeNode node in transportNode.children ?? new FunXMPP.ProtocolTreeNode[0])
      {
        if (FunXMPP.ProtocolTreeNode.TagEquals(node, "te"))
        {
          try
          {
            signalStruct.SetTransportCount(num + 1);
          }
          catch (ArgumentException ex)
          {
            Log.l("voip signaling", "Transport candidates above the limit of " + (object) num);
            continue;
          }
          VoipSignaling.SetTransportCandidate(num++, signalStruct, node);
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "net"))
        {
          int? attributeInt = node.GetAttributeInt("medium");
          if (attributeInt.HasValue)
            signalStruct.SetNetworkMedium((VoipNetworkType) attributeInt.Value);
        }
      }
      signalStruct.SetPeerJid(from);
      signalStruct.SetCallId(callId);
      this.SendStanzaToVoip(signalStruct, t, 0, (Action) (() => this.SendAck(from, id, transportNode.tag)));
    }

    private static string VideoCodecToString(VideoCodec codec)
    {
      switch (codec)
      {
        case VideoCodec.H26X:
          return "h.264";
        case VideoCodec.VPX:
          return "vp8";
        case VideoCodec.Hybrid:
          return "vp8/h.264";
        default:
          return "";
      }
    }

    private static VideoCodec StringToVideoCodec(string codec)
    {
      switch (codec)
      {
        case "h.264":
          return VideoCodec.H26X;
        case "vp8":
          return VideoCodec.VPX;
        case "vp8/h.264":
          return VideoCodec.Hybrid;
        default:
          return VideoCodec.None;
      }
    }

    private void ParseVideo(
      string callId,
      string id,
      string from,
      DateTime? t,
      FunXMPP.ProtocolTreeNode callNode,
      FunXMPP.ProtocolTreeNode videoNode)
    {
      ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
      ManagedCallProperties callProperties = new ManagedCallProperties();
      VideoParams Params = new VideoParams();
      Params.VideoState = (VideoState) (videoNode.GetAttributeInt("state") ?? 0);
      int? attributeInt1 = videoNode.GetAttributeInt("orientation");
      if (attributeInt1.HasValue && attributeInt1.Value >= 0 && attributeInt1.Value < 4)
        Params.Orientation = (VideoOrientation) attributeInt1.Value;
      if (Params.VideoState == VideoState.UpgradeRequest)
      {
        int? attributeInt2 = videoNode.GetAttributeInt("enc_supported");
        if (attributeInt2.HasValue)
          Params.EncSupported = (uint) attributeInt2.Value;
      }
      else
        Params.Codec = VoipSignaling.StringToVideoCodec(videoNode.GetAttributeValue("enc") ?? "");
      signalStruct.SetMessageType(SignalingMessageType.VideoState);
      string attributeValue = videoNode.GetAttributeValue("voip_settings");
      switch (attributeValue)
      {
        case "audio":
          signalStruct.SetSettingsType(VoipSettingsType.Audio);
          break;
        case "video":
          signalStruct.SetSettingsType(VoipSettingsType.Video);
          break;
      }
      signalStruct.SetVideoParams(Params);
      signalStruct.SetPeerJid(from);
      signalStruct.SetCallId(callId);
      if (attributeValue != null)
        this.ProcessVoipParams(videoNode, signalStruct, ref callProperties, false);
      this.SendStanzaToVoip(signalStruct, t, 0, (Action) (() => this.SendAck(from, id, videoNode.tag)));
    }

    private void ParseNotify(
      string callId,
      string id,
      string from,
      DateTime? t,
      FunXMPP.ProtocolTreeNode callNode,
      FunXMPP.ProtocolTreeNode notifyNode)
    {
      ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
      signalStruct.SetMessageType(SignalingMessageType.Notify);
      int? attributeInt = notifyNode.GetAttributeInt("batterystate");
      if (attributeInt.HasValue)
      {
        NotifyParams Params;
        Params.BatteryState = (BatteryLevel) attributeInt.Value;
        signalStruct.SetNotifyParams(Params);
      }
      signalStruct.SetPeerJid(from);
      signalStruct.SetCallId(callId);
      this.SendStanzaToVoip(signalStruct, t, 0, (Action) (() => this.SendAck(from, id, notifyNode.tag)));
    }

    private void ParseInterruption(
      string callId,
      string id,
      string from,
      DateTime? t,
      FunXMPP.ProtocolTreeNode callNode,
      FunXMPP.ProtocolTreeNode interruptionNode)
    {
      ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
      string attributeValue = interruptionNode.GetAttributeValue("state");
      bool? nullable = new bool?();
      switch (attributeValue)
      {
        case "begin":
          nullable = new bool?(true);
          break;
        case "end":
          nullable = new bool?(false);
          break;
      }
      if (nullable.HasValue)
      {
        signalStruct.SetMessageType(SignalingMessageType.Interruption);
        signalStruct.SetInterrupted(nullable.Value);
      }
      signalStruct.SetPeerJid(from);
      signalStruct.SetCallId(callId);
      this.SendStanzaToVoip(signalStruct, t, 0, (Action) (() => this.SendAck(from, id, interruptionNode.tag)));
    }

    private void ParseMute(
      string callId,
      string id,
      string from,
      DateTime? t,
      FunXMPP.ProtocolTreeNode callNode,
      FunXMPP.ProtocolTreeNode muteNode)
    {
      ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
      string attributeValue = muteNode.GetAttributeValue("state");
      bool? nullable = new bool?();
      switch (attributeValue)
      {
        case "begin":
          nullable = new bool?(true);
          break;
        case "end":
          nullable = new bool?(false);
          break;
      }
      if (nullable.HasValue)
      {
        signalStruct.SetMessageType(SignalingMessageType.Mute);
        signalStruct.SetMuted(nullable.Value);
        VoipHandler.PeerMutedSubject.OnNext(nullable.Value);
      }
      signalStruct.SetPeerJid(from);
      signalStruct.SetCallId(callId);
      this.SendStanzaToVoip(signalStruct, t, 0, (Action) (() => this.SendAck(from, id, muteNode.tag)));
    }

    private void ParseGroupUpdate(
      string callId,
      string id,
      string from,
      DateTime? t,
      FunXMPP.ProtocolTreeNode callNode,
      FunXMPP.ProtocolTreeNode groupUpdateNode)
    {
      ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
      PlatformCallParams callParams = new PlatformCallParams()
      {
        EnableLowDataUsage = Settings.LowBandwidthVoip,
        UseSoftwareEC = VoipCallParams.MsftSoftwareECNeeded,
        AudioDriver = this.PreferredAudioDriver
      };
      ManagedCallProperties managedCallProperties = new ManagedCallProperties();
      VoipSignaling.SetLastCallProperties(from, ref managedCallProperties);
      signalStruct.SetMessageType(SignalingMessageType.GroupInfo);
      signalStruct.SetCallId(callId);
      this.ParseCallParams(signalStruct, groupUpdateNode, callParams, false);
      VoipSignaling.FillVoipParams(signalStruct, ref managedCallProperties);
      foreach (FunXMPP.ProtocolTreeNode protocolTreeNode in groupUpdateNode.children ?? new FunXMPP.ProtocolTreeNode[0])
      {
        if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "group_info"))
          VoipSignaling.ParseGroupInfo(protocolTreeNode, signalStruct);
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "relay"))
        {
          int? attributeInt = protocolTreeNode.GetAttributeInt("transaction-id");
          VoipSignaling.ParseRelay(protocolTreeNode, signalStruct);
          if (attributeInt.HasValue)
            signalStruct.SetTransactionId(attributeInt.Value);
        }
        else if (!VoipSignaling.HandleVoipParamsFromProtoTree(protocolTreeNode, false, ref managedCallProperties))
          Log.l("voip signaling", "Unknown element on voip group_update: {0}", protocolTreeNode != null ? (object) protocolTreeNode.tag : (object) "<null>");
      }
      signalStruct.SetPeerJid(from);
      signalStruct.SetCallId(callId);
      this.SendStanzaToVoip(signalStruct, t, 0, (Action) (() => this.SendAck(from, id, groupUpdateNode.tag)));
    }

    private void ParseRekey(
      string callId,
      string id,
      string from,
      DateTime? t,
      FunXMPP.ProtocolTreeNode callNode,
      FunXMPP.ProtocolTreeNode rekeyNode)
    {
      ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
      int? attributeInt1 = rekeyNode.GetAttributeInt("transaction-id");
      bool flag = false;
      signalStruct.SetMessageType(SignalingMessageType.ReKey);
      if (attributeInt1.HasValue)
        signalStruct.SetTransactionId(attributeInt1.Value);
      foreach (FunXMPP.ProtocolTreeNode node in rekeyNode.children ?? new FunXMPP.ProtocolTreeNode[0])
      {
        if (FunXMPP.ProtocolTreeNode.TagEquals(node, "enc"))
        {
          Axolotl encryption = AppState.GetConnection().Encryption;
          string attributeValue = node.GetAttributeValue("type");
          int num = node.GetAttributeInt("count") ?? 0;
          int cipherVersion = node.GetAttributeInt("v") ?? 2;
          signalStruct.SetRetryCount((uint) num);
          FunXMPP.ProtocolTreeNode child = rekeyNode.GetChild("registration");
          try
          {
            signalStruct.SetCallKey(encryption.DecryptCallKey(this.FMessageFromCallOfferKeys(from, callId, attributeValue, cipherVersion, num, node.data)));
          }
          catch (Axolotl.DecryptRetryException ex)
          {
            Log.l(nameof (VoipSignaling), "Decrypt failed with retry {0}", (object) ex.RetryCount);
            int retryCount = ex.RetryCount;
            flag = true;
            this.OnE2EDecryptFailed(retryCount, child?.data);
          }
          catch (Exception ex)
          {
            Log.l(nameof (VoipSignaling), "Decrypt failed exception: {0}", (object) ex.ToString());
            flag = true;
          }
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "encopt"))
        {
          int? attributeInt2 = node.GetAttributeInt("keygen");
          if (attributeInt2.HasValue)
            signalStruct.SetKeyGenVersion((uint) attributeInt2.Value);
        }
        else
          Log.d(nameof (VoipSignaling), "Unknown node on rekey {0}", (object) node.tag);
      }
      if (flag)
      {
        AppState.GetConnection().SendReceipt(from, (string) null, id, "enc_rekey_retry", new FunXMPP.ProtocolTreeNode[2]
        {
          new FunXMPP.ProtocolTreeNode(rekeyNode.tag, new FunXMPP.KeyValue[3]
          {
            new FunXMPP.KeyValue("call-id", callId),
            new FunXMPP.KeyValue("call-creator", rekeyNode.GetAttributeValue("call-creator") ?? ""),
            new FunXMPP.KeyValue("count", (signalStruct.GetRetryCount() + 1U).ToString())
          }),
          new FunXMPP.ProtocolTreeNode("registration", (FunXMPP.KeyValue[]) null, FunXMPP.Connection.UIntToBytes(AppState.GetConnection().Encryption.Store.LocalRegistrationId))
        });
      }
      else
      {
        signalStruct.SetPeerJid(from);
        signalStruct.SetCallId(callId);
        this.SendStanzaToVoip(signalStruct, t, 0, (Action) (() => this.SendReceipt(from, id, rekeyNode.tag, callId)));
      }
    }

    private void ParseFlowControl(
      string callId,
      string id,
      string from,
      DateTime? t,
      FunXMPP.ProtocolTreeNode callNode,
      FunXMPP.ProtocolTreeNode flowControlNode)
    {
      ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
      int? attributeInt1 = flowControlNode.GetAttributeInt("transaction-id");
      int? attributeInt2 = flowControlNode.GetAttributeInt("bitrate");
      int? attributeInt3 = flowControlNode.GetAttributeInt("width");
      int? attributeInt4 = flowControlNode.GetAttributeInt("fps");
      signalStruct.SetMessageType(SignalingMessageType.FlowControl);
      signalStruct.SetTransactionId(attributeInt1 ?? -1);
      ISignalingStruct signalingStruct = signalStruct;
      int? nullable = attributeInt2;
      int Bitrate = nullable ?? -1;
      nullable = attributeInt3;
      int Width = nullable ?? -1;
      nullable = attributeInt4;
      int Fps = nullable ?? -1;
      signalingStruct.SetFlowControlParams(Bitrate, Width, Fps);
      signalStruct.SetPeerJid(from);
      signalStruct.SetCallId(callId);
      this.SendStanzaToVoip(signalStruct, t, 0, (Action) (() => this.SendAck(from, id, flowControlNode.tag)));
    }

    private void SendStanzaToVoip(
      ISignalingStruct signalStruct,
      DateTime? t,
      int elapsed,
      Action ack = null,
      Action accept = null)
    {
      if (VoipSignaling.offlineBurstInProgress)
      {
        SignalingStruct instance = NativeInterfaces.CreateInstance<SignalingStruct>();
        instance.PutBuffer(signalStruct.GetBuffer());
        VoipSignaling.coalesceTable.Offer((ISignalingStruct) instance, t, elapsed * 1000, ack, accept);
      }
      else
      {
        VoipSignaling.OnIncomingSignalData(signalStruct, accept);
        if (ack == null)
          return;
        ack();
      }
    }

    private void HandleVoipNodeImpl(FunXMPP.ProtocolTreeNode node)
    {
      string attributeValue1 = node.GetAttributeValue("from");
      string attributeValue2 = node.GetAttributeValue("id");
      DateTime? attributeDateTime = node.GetAttributeDateTime("t");
      FunXMPP.ProtocolTreeNode protocolTreeNode = ((IEnumerable<FunXMPP.ProtocolTreeNode>) (node.children ?? new FunXMPP.ProtocolTreeNode[0])).FirstOrDefault<FunXMPP.ProtocolTreeNode>();
      string attributeValue3 = protocolTreeNode.GetAttributeValue("call-id");
      if (attributeValue3 == null)
        return;
      Log.d(nameof (VoipSignaling), "Received voip stanza {0} from {1} for call-id {2}", (object) protocolTreeNode.tag, (object) attributeValue1, (object) attributeValue3);
      if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "offer"))
        this.ParseOffer(attributeValue3, attributeValue2, attributeValue1, attributeDateTime, node, protocolTreeNode);
      else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "accept"))
        this.ParseAccept(attributeValue3, attributeValue2, attributeValue1, attributeDateTime, node, protocolTreeNode);
      else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "preaccept"))
        this.ParsePreaccept(attributeValue3, attributeValue2, attributeValue1, attributeDateTime, node, protocolTreeNode);
      else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "reject"))
        this.ParseReject(attributeValue3, attributeValue2, attributeValue1, attributeDateTime, node, protocolTreeNode);
      else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "terminate"))
        this.ParseTerminate(attributeValue3, attributeValue2, attributeValue1, attributeDateTime, node, protocolTreeNode);
      else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "relaylatency"))
        this.ParseRelayLatency(attributeValue3, attributeValue2, attributeValue1, attributeDateTime, node, protocolTreeNode);
      else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "relayelection"))
        this.ParseRelayElection(attributeValue3, attributeValue2, attributeValue1, attributeDateTime, node, protocolTreeNode);
      else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "transport"))
        this.ParseTransport(attributeValue3, attributeValue2, attributeValue1, attributeDateTime, node, protocolTreeNode);
      else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "video"))
        this.ParseVideo(attributeValue3, attributeValue2, attributeValue1, attributeDateTime, node, protocolTreeNode);
      else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "notify"))
        this.ParseNotify(attributeValue3, attributeValue2, attributeValue1, attributeDateTime, node, protocolTreeNode);
      else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "interruption"))
        this.ParseInterruption(attributeValue3, attributeValue2, attributeValue1, attributeDateTime, node, protocolTreeNode);
      else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "mute"))
        this.ParseMute(attributeValue3, attributeValue2, attributeValue1, attributeDateTime, node, protocolTreeNode);
      else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "group_update"))
        this.ParseGroupUpdate(attributeValue3, attributeValue2, attributeValue1, attributeDateTime, node, protocolTreeNode);
      else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "enc_rekey"))
        this.ParseRekey(attributeValue3, attributeValue2, attributeValue1, attributeDateTime, node, protocolTreeNode);
      else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "flowcontrol"))
        this.ParseFlowControl(attributeValue3, attributeValue2, attributeValue1, attributeDateTime, node, protocolTreeNode);
      else
        Log.d(nameof (VoipSignaling), "Unknown voip message type {0}", (object) protocolTreeNode.tag);
    }

    public static void OnIncomingSignalData(ISignalingStruct signalStruct, Action onSuccess = null)
    {
      try
      {
        Voip.Instance.OnIncomingSignalData(signalStruct.GetBuffer());
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "voip signalling");
        return;
      }
      if (onSuccess == null)
        return;
      onSuccess();
    }

    public void HandleVoipAck(FunXMPP.ProtocolTreeNode node)
    {
      Voip.Worker.Enqueue((Action) (() => this.HandleVoipAckImpl(node)));
    }

    public static void SetLastCallProperties(string from, ref ManagedCallProperties props)
    {
      CallRecord lastCall = CallLog.GetLastCall();
      if (lastCall == null)
        return;
      props.LastCallSamePeer = from == lastCall.PeerJid;
      props.LastCallVideo = ((int) lastCall.VideoCall ?? 0) != 0;
      props.LastCallInterval = (DateTime.UtcNow - lastCall.EndTime).TotalMilliseconds;
    }

    private void HandleOfferAck(FunXMPP.ProtocolTreeNode node)
    {
      string peerJid = node.GetAttributeValue("from");
      ManagedCallProperties callProperties = new ManagedCallProperties();
      PlatformCallParams platformCallParams = new PlatformCallParams()
      {
        EnableLowDataUsage = Settings.LowBandwidthVoip,
        UseSoftwareEC = VoipCallParams.MsftSoftwareECNeeded,
        AudioDriver = this.PreferredAudioDriver
      };
      string id = ((IEnumerable<FunXMPP.ProtocolTreeNode>) (node.children ?? new FunXMPP.ProtocolTreeNode[0])).Where<FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, bool>) (n => FunXMPP.ProtocolTreeNode.TagEquals(n, "relay"))).Select<FunXMPP.ProtocolTreeNode, string>((Func<FunXMPP.ProtocolTreeNode, string>) (n => n.GetAttributeValue("call-id"))).FirstOrDefault<string>();
      int? attributeInt = node.GetAttributeInt("error");
      if (attributeInt.HasValue)
      {
        string str = ((IEnumerable<FunXMPP.ProtocolTreeNode>) (node.children ?? new FunXMPP.ProtocolTreeNode[0])).Where<FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, bool>) (n => FunXMPP.ProtocolTreeNode.TagEquals(n, "relay"))).Select<FunXMPP.ProtocolTreeNode, string>((Func<FunXMPP.ProtocolTreeNode, string>) (n => n.GetAttributeValue("call-id"))).FirstOrDefault<string>();
        Voip.Instance.GetCallbacks().PlumbThroughError(peerJid ?? "", str ?? "", "\0" + attributeInt.Value.ToString());
      }
      else
      {
        ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
        signalStruct.SetMessageType(SignalingMessageType.OfferAck);
        CallInfoStruct? callInfo = Voip.Instance.GetCallInfo();
        if ((callInfo.HasValue ? (callInfo.Value.IsGroupCall ? 1 : 0) : 0) == 0)
        {
          if (id != null)
            signalStruct.SetCallId(id);
          this.ProcessVoipParams(node, signalStruct, ref callProperties, true);
        }
        if (peerJid == null || id == null)
          return;
        signalStruct.SetPeerJid(peerJid);
        signalStruct.SetCallId(id);
        VoipSignaling.OnIncomingSignalData(signalStruct, (Action) (() =>
        {
          VoipSignaling.SetLastCallProperties(peerJid, ref callProperties);
          Voip.Instance.GetCallbacks().SetManagedCallProperties(Settings.MyJid, callProperties);
        }));
      }
    }

    private void HandleVideoStateAck(FunXMPP.ProtocolTreeNode node)
    {
      string attributeValue1 = node.GetAttributeValue("from");
      FunXMPP.ProtocolTreeNode protocolTreeNode = node.GetChild("video") ?? new FunXMPP.ProtocolTreeNode("video", (FunXMPP.KeyValue[]) null);
      ManagedCallProperties callProperties = new ManagedCallProperties();
      ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
      signalStruct.SetMessageType(SignalingMessageType.VideoStateAck);
      string attributeValue2 = protocolTreeNode.GetAttributeValue("call-id");
      string attributeValue3 = protocolTreeNode.GetAttributeValue("voip_settings");
      if (attributeValue3 == "audio")
        signalStruct.SetSettingsType(VoipSettingsType.Audio);
      else if (attributeValue3 == "video")
        signalStruct.SetSettingsType(VoipSettingsType.Video);
      if (attributeValue3 != null)
      {
        if (attributeValue2 != null)
          signalStruct.SetCallId(attributeValue2);
        this.ProcessVoipParams(node, signalStruct, ref callProperties, false);
      }
      if (attributeValue1 == null || attributeValue2 == null)
        return;
      signalStruct.SetPeerJid(attributeValue1);
      signalStruct.SetCallId(attributeValue2);
      VoipSignaling.OnIncomingSignalData(signalStruct);
    }

    private void HandleVoipAckImpl(FunXMPP.ProtocolTreeNode node)
    {
      string attributeValue = node.GetAttributeValue("type");
      switch (attributeValue)
      {
        case "offer":
          this.HandleOfferAck(node);
          break;
        case "video":
          this.HandleVideoStateAck(node);
          break;
        default:
          Log.l("voip signaling", "Received ack for {0}, ignoring", (object) attributeValue);
          break;
      }
    }

    public void HandleEncRekeyRetry(FunXMPP.ProtocolTreeNode node)
    {
      Voip.Worker.Enqueue((Action) (() => this.HandleEncRekeyRetryImpl(node)));
    }

    private void HandleEncRekeyRetryImpl(FunXMPP.ProtocolTreeNode node)
    {
      int count = 0;
      uint? registrationId = new uint?();
      string peerJid = node.GetAttributeValue("from");
      FunXMPP.ProtocolTreeNode child1 = node.GetChild("enc_rekey");
      if (child1 != null)
      {
        count = child1.GetAttributeInt("count") ?? 0;
        child1.GetAttributeValue("call-id");
        child1.GetAttributeValue("call-creator");
      }
      FunXMPP.ProtocolTreeNode child2 = node.GetChild("registration");
      if (child2 != null)
        registrationId = new uint?(FunXMPP.Connection.UIntFromBytes(child2.data));
      if (!AppState.GetConnection().Encryption.ConfirmRecipientRegistration(peerJid, registrationId))
        return;
      AppState.GetConnection().Encryption.SendGetPreKey(peerJid, (Action) (() => Voip.Instance.SendRekeyRequest(peerJid, (uint) count)));
    }

    private void ProcessVoipParams(
      FunXMPP.ProtocolTreeNode node,
      ISignalingStruct signalStruct,
      ref ManagedCallProperties callProperties,
      bool isVoipSettingsMandatory)
    {
      bool flag1 = false;
      bool flag2 = false;
      PlatformCallParams callParams = new PlatformCallParams()
      {
        EnableLowDataUsage = Settings.LowBandwidthVoip,
        UseSoftwareEC = VoipCallParams.MsftSoftwareECNeeded,
        AudioDriver = this.PreferredAudioDriver
      };
      this.ParseCallParams(signalStruct, node, callParams, isVoipSettingsMandatory);
      VoipSignaling.FillVoipParams(signalStruct, ref callProperties);
      foreach (FunXMPP.ProtocolTreeNode protocolTreeNode in node.children ?? new FunXMPP.ProtocolTreeNode[0])
      {
        if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "audio") && VoipSignaling.EnforceOnce(ref flag1))
          signalStruct.SetCapabilities(VoipSignaling.ParseCapabilities(protocolTreeNode));
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "relay") && VoipSignaling.EnforceOnce(ref flag2))
          VoipSignaling.ParseRelay(protocolTreeNode, signalStruct);
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "rte"))
          signalStruct.SetReflexiveAddress(protocolTreeNode.data);
        else if (FunXMPP.ProtocolTreeNode.TagEquals(protocolTreeNode, "group_info"))
          VoipSignaling.ParseGroupInfo(protocolTreeNode, signalStruct);
        else if (!VoipSignaling.HandleVoipParamsFromProtoTree(protocolTreeNode, true, ref callProperties))
          Log.l("voip signaling", "Unknown element on voip ack: {0}", protocolTreeNode != null ? (object) protocolTreeNode.tag : (object) "<null>");
      }
    }

    private static void ParseRelay(FunXMPP.ProtocolTreeNode child, ISignalingStruct signalStruct)
    {
      int Idx = 0;
      int num = 0;
      foreach (FunXMPP.ProtocolTreeNode node in child.children ?? new FunXMPP.ProtocolTreeNode[0])
      {
        if (FunXMPP.ProtocolTreeNode.TagEquals(node, "te"))
        {
          try
          {
            signalStruct.SetRelayCount(Idx + 1);
          }
          catch (ArgumentException ex)
          {
            Log.l("voip signaling", "Relay count above the limit of " + (object) Idx);
            continue;
          }
          signalStruct.SetRelayInformation(Idx, node.data);
          ++Idx;
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "token"))
        {
          try
          {
            signalStruct.SetRelayTokenCount(num + 1);
          }
          catch (ArgumentException ex)
          {
            Log.l("voip signaling", "Relay token count above the limit of " + (object) num);
            continue;
          }
          signalStruct.SetRelayToken(num++, node.data);
        }
      }
    }

    private static CallParticipantState StringToParticipantState(string state)
    {
      switch (state)
      {
        case "connected":
          return CallParticipantState.Connected;
        case "missed":
          return CallParticipantState.CancelOffer;
        case "outgoing":
          return CallParticipantState.Incoming;
        case "receipt":
          return CallParticipantState.Receipt;
        case "rejected":
          return CallParticipantState.Rejected;
        case "terminated":
          return CallParticipantState.Terminated;
        case "timedout":
          return CallParticipantState.Timedout;
        default:
          return CallParticipantState.Invalid;
      }
    }

    private static void ParseGroupInfo(
      FunXMPP.ProtocolTreeNode child,
      ISignalingStruct signalStruct)
    {
      int? attributeInt1 = child.GetAttributeInt("transaction-id");
      int? attributeInt2 = child.GetAttributeInt("rekey");
      int num1 = 1;
      bool Rekey = attributeInt2.GetValueOrDefault() == num1 && attributeInt2.HasValue;
      string attributeValue = child.GetAttributeValue("media");
      List<CallParticipantInfo> callParticipantInfoList = new List<CallParticipantInfo>();
      List<int> intList = new List<int>();
      List<byte[]> numArrayList = new List<byte[]>();
      foreach (FunXMPP.ProtocolTreeNode protocolTreeNode in child.children ?? new FunXMPP.ProtocolTreeNode[0])
      {
        FunXMPP.ProtocolTreeNode child1 = protocolTreeNode.GetChild("capability");
        int num2 = -1;
        byte[] numArray = (byte[]) null;
        if (child1 != null)
        {
          int? attributeInt3 = child1.GetAttributeInt("ver");
          if (attributeInt3.HasValue && attributeInt3.Value > 0)
          {
            num2 = attributeInt3.Value;
            numArray = child1.data;
          }
        }
        intList.Add(num2);
        numArrayList.Add(numArray);
        callParticipantInfoList.Add(new CallParticipantInfo()
        {
          Jid = protocolTreeNode.GetAttributeValue("jid"),
          State = VoipSignaling.StringToParticipantState(protocolTreeNode.GetAttributeValue("state")),
          Reason = protocolTreeNode.GetAttributeValue("reason") ?? "",
          Device = protocolTreeNode.GetAttributeValue("device") ?? ""
        });
      }
      signalStruct.SetGroupInfo(attributeInt1.GetValueOrDefault(), Rekey, attributeValue, callParticipantInfoList.ToArray());
      for (int index = 0; index < intList.Count; ++index)
        signalStruct.SetGroupCapability((uint) index, intList[index], numArrayList[index]);
    }

    private bool ParseCallParams(
      ISignalingStruct signalStruct,
      FunXMPP.ProtocolTreeNode node,
      PlatformCallParams callParams,
      bool isVoipSettingsMandatory)
    {
      FunXMPP.ProtocolTreeNode child = node.GetChild("voip_settings");
      if (child != null)
      {
        signalStruct.ParseCompressedCallParams(child.data, callParams);
        return true;
      }
      if (isVoipSettingsMandatory)
        throw new ArgumentException("No `voip_settings` child found in stanza", "voip_settings");
      return false;
    }

    private ClientPlatformName ParsePeerPlatformName(string platformStr)
    {
      switch (platformStr)
      {
        case null:
          return ClientPlatformName.Unknown;
        case "android":
          return ClientPlatformName.Android;
        case "iphone":
          return ClientPlatformName.iPhone;
        case "wp":
          return ClientPlatformName.WP;
        case "ios_tablet":
          return ClientPlatformName.iOSTablet;
        default:
          return ClientPlatformName.Unknown;
      }
    }

    private static int? GetCallParamAsInt(ISignalingStruct signalStruct, string paramPath)
    {
      int? callParamAsInt = new int?();
      string callParam = signalStruct.GetCallParam(paramPath);
      if (callParam.Length > 0)
      {
        int result = 0;
        if (int.TryParse(callParam, out result))
          callParamAsInt = new int?(result);
      }
      return callParamAsInt;
    }

    private static void FillVoipParams(
      ISignalingStruct signalStruct,
      ref ManagedCallProperties managedProperties)
    {
      int? callParamAsInt1 = VoipSignaling.GetCallParamAsInt(signalStruct, "options.caller_timeout");
      int? callParamAsInt2 = VoipSignaling.GetCallParamAsInt(signalStruct, "options.audio_sampling_rate");
      int? callParamAsInt3 = VoipSignaling.GetCallParamAsInt(signalStruct, "options.call_start_delay");
      int? callParamAsInt4 = VoipSignaling.GetCallParamAsInt(signalStruct, "options.enable_audio_video_switch");
      int? callParamAsInt5 = VoipSignaling.GetCallParamAsInt(signalStruct, "options.upload_logs");
      int? callParamAsInt6 = VoipSignaling.GetCallParamAsInt(signalStruct, "options.forbid_p2p_for_non_contact");
      int? callParamAsInt7 = VoipSignaling.GetCallParamAsInt(signalStruct, "options.send_call_state_for_video_enabled");
      ref ManagedCallProperties local1 = ref managedProperties;
      int? nullable = callParamAsInt5;
      int num1 = (nullable ?? 0) != 0 ? 1 : 0;
      local1.ShouldUploadLogs = num1 != 0;
      ref ManagedCallProperties local2 = ref managedProperties;
      nullable = callParamAsInt6;
      int num2 = (nullable ?? 0) != 0 ? 1 : 0;
      local2.ForbidP2PForNonContact = num2 != 0;
      if (callParamAsInt1.HasValue)
        managedProperties.CallerTimeout = callParamAsInt1.Value;
      if (callParamAsInt2.HasValue)
        Voip.Instance.SetSampleRates(new int[1]
        {
          callParamAsInt2.Value
        });
      nullable = callParamAsInt3;
      Settings.CallStartDelay = nullable ?? 0;
      if (callParamAsInt4.HasValue)
        Settings.AudioVideoSwitchEnabled = callParamAsInt4.Value == 1;
      if (!callParamAsInt7.HasValue)
        return;
      NonDbSettings.SendCallStateForVideoEnabled = callParamAsInt7.Value == 1;
    }

    private static bool HandleVoipParamsFromProtoTree(
      FunXMPP.ProtocolTreeNode child,
      bool isCaller,
      ref ManagedCallProperties managedProperties)
    {
      managedProperties.ShouldRateCall = true;
      if (FunXMPP.ProtocolTreeNode.TagEquals(child, "userrate"))
      {
        managedProperties.ShouldRateCall = true;
        int? attributeInt = child.GetAttributeInt("interval");
        ref ManagedCallProperties local = ref managedProperties;
        int? nullable = attributeInt;
        int num = nullable ?? -1;
        local.CallRatingIntervalInSeconds = num;
        object[] objArray = new object[1];
        nullable = attributeInt;
        objArray[0] = (object) (nullable ?? 86400);
        Log.l("voip signaling", "User rating enabled with interval: {0}", objArray);
      }
      else if (FunXMPP.ProtocolTreeNode.TagEquals(child, "uploadfieldstat"))
      {
        managedProperties.SendImmediately = true;
      }
      else
      {
        if (!FunXMPP.ProtocolTreeNode.TagEquals(child, "client"))
          return false;
        int? attributeInt = child.GetAttributeInt("callee_bad_asn");
        int num1 = 1;
        bool flag1 = attributeInt.GetValueOrDefault() == num1 && attributeInt.HasValue;
        attributeInt = child.GetAttributeInt("caller_bad_asn");
        int num2 = 1;
        bool flag2 = attributeInt.GetValueOrDefault() == num2 && attributeInt.HasValue;
        if (isCaller & flag2 || !isCaller & flag1)
          managedProperties.BadASN = true;
      }
      return true;
    }

    private static bool EnforceOnce(ref bool flag)
    {
      if (flag)
        return false;
      flag = true;
      return true;
    }

    private static void SetTransportCandidate(
      int idx,
      ISignalingStruct signalStruct,
      FunXMPP.ProtocolTreeNode node)
    {
      string attributeValue = node.GetAttributeValue("priority");
      byte result = 0;
      if (attributeValue != null)
        byte.TryParse(attributeValue, out result);
      int? attributeInt = node.GetAttributeInt("portpredicting");
      signalStruct.SetTransportCandidate(idx, result, (attributeInt ?? 0) != 0, node.data);
    }

    private static void SetRelayLatency(
      int idx,
      ISignalingStruct signalStruct,
      FunXMPP.ProtocolTreeNode node)
    {
      string attributeValue = node.GetAttributeValue("latency");
      int result = 0;
      if (attributeValue != null)
        int.TryParse(attributeValue, out result);
      signalStruct.SetRelayLatency(idx, result, node.data);
    }

    private static Capabilities ParseCapabilities(FunXMPP.ProtocolTreeNode node)
    {
      string attributeValue1 = node.GetAttributeValue("enc");
      string attributeValue2 = node.GetAttributeValue("rate");
      if (attributeValue1 == "opus")
      {
        switch (attributeValue2)
        {
          case "48000":
            return Capabilities.Opus48KHz;
          case "24000":
            return Capabilities.Opus24KHz;
          case "16000":
            return Capabilities.Opus16KHz;
          case "8000":
            return Capabilities.Opus8KHz;
        }
      }
      Log.l("voip signaling", "Unknown capabilities: codec={0}, rate={1}", (object) attributeValue1, (object) attributeValue2);
      return (Capabilities) 0;
    }

    private static VideoParams ParseVideoParams(FunXMPP.ProtocolTreeNode node)
    {
      string codec = node.GetAttributeValue("enc") ?? "h.264";
      VideoParams videoParams;
      ref VideoParams local1 = ref videoParams;
      int? attributeInt = node.GetAttributeInt("state");
      int num1 = attributeInt ?? 1;
      local1.VideoState = (VideoState) num1;
      ref VideoParams local2 = ref videoParams;
      attributeInt = node.GetAttributeInt("enc_supported");
      int num2 = attributeInt ?? 0;
      local2.EncSupported = (uint) num2;
      ref VideoParams local3 = ref videoParams;
      attributeInt = node.GetAttributeInt("orientation");
      int num3 = attributeInt ?? 0;
      local3.Orientation = (VideoOrientation) num3;
      videoParams.Codec = VoipSignaling.StringToVideoCodec(codec);
      return videoParams;
    }

    private static Size GetScreenResolution() => new Size(480.0, 640.0);

    private static void AddVideoParams(
      VideoParams vidParams,
      List<FunXMPP.ProtocolTreeNode> children)
    {
      if (vidParams.VideoState == VideoState.Disabled || vidParams.Codec != VideoCodec.H26X && vidParams.Codec != VideoCodec.VPX)
        return;
      Log.l("voip video", "Sending signaling message with video enabled");
      Size screenResolution = VoipSignaling.GetScreenResolution();
      children.Add(new FunXMPP.ProtocolTreeNode("video", new FunXMPP.KeyValue[4]
      {
        new FunXMPP.KeyValue("enc", VoipSignaling.VideoCodecToString(vidParams.Codec)),
        new FunXMPP.KeyValue("screen_width", screenResolution.Width.ToString()),
        new FunXMPP.KeyValue("screen_height", screenResolution.Height.ToString()),
        new FunXMPP.KeyValue("orientation", ((int) vidParams.Orientation).ToString())
      }));
    }

    private static void AddNetworkMedium(
      ISignalingStruct signalStruct,
      List<FunXMPP.ProtocolTreeNode> children)
    {
      VoipNetworkType networkMedium = signalStruct.GetNetworkMedium();
      if (networkMedium == VoipNetworkType.Unknown)
        return;
      children.Add(new FunXMPP.ProtocolTreeNode("net", new FunXMPP.KeyValue[1]
      {
        new FunXMPP.KeyValue("medium", ((int) networkMedium).ToString())
      }));
    }

    private static void AddEncOpt(
      ISignalingStruct signalStruct,
      List<FunXMPP.ProtocolTreeNode> children)
    {
      uint keyGenVersion = signalStruct.GetKeyGenVersion();
      if (keyGenVersion <= 0U)
        return;
      children.Add(new FunXMPP.ProtocolTreeNode("encopt", new FunXMPP.KeyValue[1]
      {
        new FunXMPP.KeyValue("keygen", ((int) keyGenVersion).ToString())
      }));
    }

    private static void AddCapability(
      ISignalingStruct signalStruct,
      List<FunXMPP.ProtocolTreeNode> children)
    {
      int Version;
      byte[] Capability;
      signalStruct.GetCapability(out Version, out Capability);
      if (Version <= 0 || Capability == null || Capability.Length == 0)
        return;
      children.Add(new FunXMPP.ProtocolTreeNode("capability", new FunXMPP.KeyValue[1]
      {
        new FunXMPP.KeyValue("ver", Version.ToString())
      }, Capability));
    }

    private static void AddGroupInfo(
      ISignalingStruct signalStruct,
      List<FunXMPP.ProtocolTreeNode> children)
    {
      CallParticipantInfo[] ParticipantInfo;
      signalStruct.GetGroupInfo(out int _, out bool _, out string _, out ParticipantInfo);
      if (ParticipantInfo.Length == 0)
        return;
      List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList1 = new List<FunXMPP.ProtocolTreeNode>(ParticipantInfo.Length);
      for (int Index = 0; Index < ParticipantInfo.Length; ++Index)
      {
        int Version;
        byte[] Capability;
        signalStruct.GetGroupCapability((uint) Index, out Version, out Capability);
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>();
        keyValueList.Add(new FunXMPP.KeyValue("jid", ParticipantInfo[Index].Jid));
        string device = ParticipantInfo[Index].Device;
        if ((device != null ? (device.Length > 0 ? 1 : 0) : 0) != 0)
          keyValueList.Add(new FunXMPP.KeyValue("jid", ParticipantInfo[Index].Device));
        List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList2 = protocolTreeNodeList1;
        FunXMPP.KeyValue[] array = keyValueList.ToArray();
        FunXMPP.ProtocolTreeNode child;
        if (Version <= 0)
          child = (FunXMPP.ProtocolTreeNode) null;
        else
          child = new FunXMPP.ProtocolTreeNode("capability", new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("ver", Version.ToString())
          }, Capability);
        FunXMPP.ProtocolTreeNode protocolTreeNode = new FunXMPP.ProtocolTreeNode("participant", array, child);
        protocolTreeNodeList2.Add(protocolTreeNode);
      }
      children.Add(new FunXMPP.ProtocolTreeNode("group_info", new FunXMPP.KeyValue[0], protocolTreeNodeList1.ToArray()));
    }

    private static bool AddEncryptionNode(
      ISignalingStruct signalStruct,
      List<FunXMPP.ProtocolTreeNode> children,
      Action retryAction)
    {
      byte[] KeyBytes;
      signalStruct.GetCallKey(out KeyBytes);
      string peerJid = signalStruct.GetPeerJid();
      if (KeyBytes != null)
      {
        string callId = signalStruct.GetCallId();
        uint retryCount = signalStruct.GetRetryCount();
        AxolotlCiphertextType type;
        byte[] data = AppState.GetConnection().Encryption.EncryptIndividualPayload(WhatsApp.ProtoBuf.Message.CreateForCall(KeyBytes), retryAction, peerJid, callId, (int) retryCount, 2, out type);
        if (data == null)
        {
          Log.l("Null ciphertext when encrypting call key");
          return false;
        }
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>()
        {
          new FunXMPP.KeyValue("type", Axolotl.CiphertextTypeString(type)),
          new FunXMPP.KeyValue("v", 2.ToString())
        };
        if (retryCount > 0U)
          keyValueList.Add(new FunXMPP.KeyValue("count", retryCount.ToString()));
        children.Add(new FunXMPP.ProtocolTreeNode("enc", keyValueList.ToArray(), data));
      }
      return true;
    }

    public static void WriteVoipNode(
      byte[] signalStructPayload,
      SignalingDataArgs? args = null,
      bool onThread = false)
    {
      if (!onThread)
      {
        Voip.Worker.Enqueue((Action) (() => VoipSignaling.WriteVoipNode(signalStructPayload, args, true)));
      }
      else
      {
        ISignalingStruct signalStruct = Utils.LazyInit<ISignalingStruct>(ref VoipSignaling.signalInstance, (Func<ISignalingStruct>) (() => (ISignalingStruct) NativeInterfaces.CreateInstance<SignalingStruct>()));
        signalStruct.PutBuffer(signalStructPayload);
        List<FunXMPP.KeyValue> keyValueList1 = new List<FunXMPP.KeyValue>();
        List<FunXMPP.ProtocolTreeNode> children = new List<FunXMPP.ProtocolTreeNode>();
        List<FunXMPP.KeyValue> keyValueList2 = new List<FunXMPP.KeyValue>();
        keyValueList2.Add(new FunXMPP.KeyValue("call-id", signalStruct.GetCallId()));
        string callCreatorJid = signalStruct.GetCallCreatorJid();
        if (callCreatorJid != null)
          keyValueList2.Add(new FunXMPP.KeyValue("call-creator", callCreatorJid));
        SignalingMessageType messageType = signalStruct.GetMessageType();
        Log.l("voip signaling", "Call stack sent message: {0}", (object) messageType.ToString());
        switch (messageType)
        {
          case SignalingMessageType.Offer:
            children.AddRange(VoipSignaling.GetAudioNodes(signalStruct.GetCapabilities()));
            for (int Idx = 0; Idx < signalStruct.GetVideoParamsCount(); ++Idx)
              VoipSignaling.AddVideoParams(signalStruct.GetVideoParamsIdx(Idx), children);
            if (Voip.Instance.GetCallPeers().Count < 2)
            {
              if (!VoipSignaling.AddEncryptionNode(signalStruct, children, (Action) (() => VoipSignaling.WriteVoipNode(signalStructPayload, args))))
                return;
              VoipSignaling.AddEncOpt(signalStruct, children);
            }
            else
              VoipSignaling.AddGroupInfo(signalStruct, children);
            VoipSignaling.AddNetworkMedium(signalStruct, children);
            VoipSignaling.AddCapability(signalStruct, children);
            if (signalStruct.GetResume())
              keyValueList2.Add(new FunXMPP.KeyValue("resume", "true"));
            string deviceName1 = signalStruct.GetDeviceName(DeviceNameSide.Caller);
            if (deviceName1 != null && deviceName1.Length > 0)
              keyValueList2.Add(new FunXMPP.KeyValue("device", deviceName1));
            children.AddRange(VoipSignaling.GetTransportCandidateNodes(signalStruct));
            children = ((IEnumerable<FunXMPP.ProtocolTreeNode>) new FunXMPP.ProtocolTreeNode[1]
            {
              new FunXMPP.ProtocolTreeNode("offer", keyValueList2.ToArray(), children.ToArray())
            }).ToList<FunXMPP.ProtocolTreeNode>();
            break;
          case SignalingMessageType.Accept:
            CallCoalesceTable.KeyForCall(signalStruct.GetPeerJid(), signalStruct.GetCallId());
            VoipSignaling.GetPendingOffer(signalStruct.GetPeerJid(), signalStruct.GetCallId(), true)?.Process();
            children.AddRange(VoipSignaling.GetAudioNodes(signalStruct.GetCapabilities()).Take<FunXMPP.ProtocolTreeNode>(1));
            VoipSignaling.AddVideoParams(signalStruct.GetVideoParams(), children);
            children.AddRange(VoipSignaling.GetTransportCandidateNodes(signalStruct));
            VoipSignaling.AddNetworkMedium(signalStruct, children);
            VoipSignaling.AddEncOpt(signalStruct, children);
            string deviceName2 = signalStruct.GetDeviceName(DeviceNameSide.Callee);
            if (deviceName2 != null && deviceName2.Length > 0)
              keyValueList2.Add(new FunXMPP.KeyValue("device", deviceName2));
            string deviceName3 = signalStruct.GetDeviceName(DeviceNameSide.Caller);
            if (deviceName3 != null && deviceName3.Length > 0)
              keyValueList2.Add(new FunXMPP.KeyValue("peer-device", deviceName3));
            children = ((IEnumerable<FunXMPP.ProtocolTreeNode>) new FunXMPP.ProtocolTreeNode[1]
            {
              new FunXMPP.ProtocolTreeNode("accept", keyValueList2.ToArray(), children.ToArray())
            }).ToList<FunXMPP.ProtocolTreeNode>();
            break;
          case SignalingMessageType.Reject:
            string reason = signalStruct.GetReason();
            keyValueList2.Add(new FunXMPP.KeyValue("reason", reason));
            if (reason == "enc")
            {
              children.Add(new FunXMPP.ProtocolTreeNode("registration", (FunXMPP.KeyValue[]) null, FunXMPP.Connection.UIntToBytes(AppState.GetConnection().Encryption.Store.LocalRegistrationId)));
              uint retryCount = signalStruct.GetRetryCount();
              if (retryCount > 0U)
                keyValueList2.Add(new FunXMPP.KeyValue("count", retryCount.ToString()));
            }
            children = ((IEnumerable<FunXMPP.ProtocolTreeNode>) new FunXMPP.ProtocolTreeNode[1]
            {
              new FunXMPP.ProtocolTreeNode("reject", keyValueList2.ToArray(), children.ToArray())
            }).ToList<FunXMPP.ProtocolTreeNode>();
            break;
          case SignalingMessageType.Terminate:
            keyValueList2.Add(new FunXMPP.KeyValue("reason", signalStruct.GetReason()));
            if (args.HasValue)
            {
              if (args.Value.CallDuration > 0)
                keyValueList2.Add(new FunXMPP.KeyValue("duration", args.Value.CallDuration.ToString()));
              if (args.Value.AudioDuration > 0)
                keyValueList2.Add(new FunXMPP.KeyValue("audio_duration", args.Value.AudioDuration.ToString()));
              if (args.Value.VideoDuration > 0)
                keyValueList2.Add(new FunXMPP.KeyValue("video_duration", args.Value.VideoDuration.ToString()));
            }
            VideoState videoState = signalStruct.GetVideoState();
            if (videoState != VideoState.Disabled)
              keyValueList2.Add(new FunXMPP.KeyValue("videostate", ((int) videoState).ToString()));
            children.Add(new FunXMPP.ProtocolTreeNode("terminate", keyValueList2.ToArray()));
            break;
          case SignalingMessageType.Transport:
            children.AddRange(VoipSignaling.GetTransportCandidateNodes(signalStruct));
            VoipSignaling.AddNetworkMedium(signalStruct, children);
            children = ((IEnumerable<FunXMPP.ProtocolTreeNode>) new FunXMPP.ProtocolTreeNode[1]
            {
              new FunXMPP.ProtocolTreeNode("transport", keyValueList2.ToArray(), children.ToArray())
            }).ToList<FunXMPP.ProtocolTreeNode>();
            break;
          case SignalingMessageType.RelayLatency:
            keyValueList2.Add(new FunXMPP.KeyValue("transaction-id", signalStruct.GetTransactionId().ToString()));
            children.Add(new FunXMPP.ProtocolTreeNode("relaylatency", keyValueList2.ToArray(), VoipSignaling.GetRelayLatencyNodes(signalStruct).ToArray<FunXMPP.ProtocolTreeNode>()));
            break;
          case SignalingMessageType.RelayElection:
            int Latency;
            byte[] Address;
            signalStruct.GetRelayElection(out Latency, out Address);
            children.Add(new FunXMPP.ProtocolTreeNode("relayelection", keyValueList2.ToArray(), new FunXMPP.ProtocolTreeNode[1]
            {
              new FunXMPP.ProtocolTreeNode("te", new FunXMPP.KeyValue[1]
              {
                new FunXMPP.KeyValue("latency", Latency.ToString())
              }, Address)
            }));
            break;
          case SignalingMessageType.Interruption:
            keyValueList2.Add(new FunXMPP.KeyValue("state", signalStruct.GetInterrupted() ? "begin" : "end"));
            children.Add(new FunXMPP.ProtocolTreeNode("interruption", keyValueList2.ToArray()));
            break;
          case SignalingMessageType.Mute:
            keyValueList2.Add(new FunXMPP.KeyValue("state", signalStruct.GetMuted() ? "begin" : "end"));
            children.Add(new FunXMPP.ProtocolTreeNode("mute", keyValueList2.ToArray()));
            break;
          case SignalingMessageType.PreAccept:
            keyValueList1.Add(new FunXMPP.KeyValue("to", signalStruct.GetPeerJid()));
            children.AddRange(VoipSignaling.GetAudioNodes(signalStruct.GetCapabilities()));
            VoipSignaling.AddVideoParams(signalStruct.GetVideoParams(), children);
            VoipSignaling.AddEncOpt(signalStruct, children);
            VoipSignaling.AddCapability(signalStruct, children);
            children = ((IEnumerable<FunXMPP.ProtocolTreeNode>) new FunXMPP.ProtocolTreeNode[1]
            {
              new FunXMPP.ProtocolTreeNode("preaccept", keyValueList2.ToArray(), children.ToArray())
            }).ToList<FunXMPP.ProtocolTreeNode>();
            break;
          case SignalingMessageType.VideoState:
            VideoParams videoParams = signalStruct.GetVideoParams();
            Size screenResolution = VoipSignaling.GetScreenResolution();
            keyValueList2.Add(new FunXMPP.KeyValue("state", ((int) videoParams.VideoState).ToString()));
            keyValueList2.Add(new FunXMPP.KeyValue("orientation", ((int) videoParams.Orientation).ToString()));
            string deviceName4 = signalStruct.GetDeviceName(DeviceNameSide.Caller);
            if (deviceName4 != null && deviceName4.Length > 0)
              keyValueList2.Add(new FunXMPP.KeyValue("device", deviceName4));
            string deviceName5 = signalStruct.GetDeviceName(DeviceNameSide.Callee);
            if (deviceName5 != null && deviceName5.Length > 0)
              keyValueList2.Add(new FunXMPP.KeyValue("peer-device", deviceName5));
            if (videoParams.VideoState == VideoState.UpgradeRequest)
              keyValueList2.Add(new FunXMPP.KeyValue("enc_supported", videoParams.EncSupported.ToString()));
            if (videoParams.Codec != VideoCodec.None)
              keyValueList2.Add(new FunXMPP.KeyValue("enc", VoipSignaling.VideoCodecToString(videoParams.Codec)));
            switch (signalStruct.GetSettingsType())
            {
              case VoipSettingsType.Audio:
                keyValueList2.Add(new FunXMPP.KeyValue("voip_settings", "audio"));
                break;
              case VoipSettingsType.Video:
                keyValueList2.Add(new FunXMPP.KeyValue("voip_settings", "video"));
                break;
            }
            keyValueList2.Add(new FunXMPP.KeyValue("screen_width", screenResolution.Width.ToString()));
            keyValueList2.Add(new FunXMPP.KeyValue("screen_height", screenResolution.Height.ToString()));
            children.Add(new FunXMPP.ProtocolTreeNode("video", keyValueList2.ToArray()));
            break;
          case SignalingMessageType.Notify:
            NotifyParams notifyParams = signalStruct.GetNotifyParams();
            keyValueList2.Add(new FunXMPP.KeyValue("batterystate", ((int) notifyParams.BatteryState).ToString()));
            children.Add(new FunXMPP.ProtocolTreeNode("notify", keyValueList2.ToArray()));
            break;
          case SignalingMessageType.ReKey:
            keyValueList2.Add(new FunXMPP.KeyValue("transaction-id", signalStruct.GetTransactionId().ToString()));
            if (!VoipSignaling.AddEncryptionNode(signalStruct, children, (Action) (() => VoipSignaling.WriteVoipNode(signalStructPayload, args))))
              return;
            VoipSignaling.AddEncOpt(signalStruct, children);
            children = ((IEnumerable<FunXMPP.ProtocolTreeNode>) new FunXMPP.ProtocolTreeNode[1]
            {
              new FunXMPP.ProtocolTreeNode("enc_rekey", keyValueList2.ToArray(), children.ToArray())
            }).ToList<FunXMPP.ProtocolTreeNode>();
            break;
          case SignalingMessageType.PeerState:
            string Jid;
            CallParticipantState State;
            signalStruct.GetPeerState(out Jid, out State);
            keyValueList2.Add(new FunXMPP.KeyValue("t", ((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString()));
            FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("participant", new FunXMPP.KeyValue[2]
            {
              new FunXMPP.KeyValue("state", VoipSignaling.GetPeerStateString(State)),
              new FunXMPP.KeyValue("jid", Jid)
            });
            children.Add(new FunXMPP.ProtocolTreeNode("peer_state", keyValueList2.ToArray(), child));
            break;
          case SignalingMessageType.FlowControl:
            keyValueList2.Add(new FunXMPP.KeyValue("transaction-id", signalStruct.GetTransactionId().ToString()));
            int Bitrate;
            int Width;
            int Fps;
            signalStruct.GetFlowControlParams(out Bitrate, out Width, out Fps);
            if (Bitrate != 0)
              keyValueList2.Add(new FunXMPP.KeyValue("bitrate", Bitrate.ToString()));
            if (Width != 0)
              keyValueList2.Add(new FunXMPP.KeyValue("width", Width.ToString()));
            if (Fps != 0)
              keyValueList2.Add(new FunXMPP.KeyValue("fps", Fps.ToString()));
            children.Add(new FunXMPP.ProtocolTreeNode("flowcontrol", keyValueList2.ToArray()));
            break;
          default:
            Log.l("voip signaling", "Message type {0} not handled", (object) messageType.ToString());
            return;
        }
        keyValueList1.Add(new FunXMPP.KeyValue("to", signalStruct.GetPeerJid()));
        string messageId = FunXMPP.GenerateMessageId();
        keyValueList1.Add(new FunXMPP.KeyValue("id", messageId));
        FunXMPP.ProtocolTreeNode node = new FunXMPP.ProtocolTreeNode("call", keyValueList1.ToArray(), children.ToArray());
        AppState.GetConnection().SendRawNode(node);
      }
    }

    private static string GetPeerStateString(CallParticipantState state)
    {
      switch (state)
      {
        case CallParticipantState.Invisible:
          return "invisible";
        case CallParticipantState.Visible:
          return "visible";
        case CallParticipantState.CancelOffer:
          return "cancel_offer";
        default:
          return "";
      }
    }

    private static IEnumerable<FunXMPP.ProtocolTreeNode> GetAudioNodes(Capabilities caps)
    {
      List<string> source = new List<string>();
      if ((caps & Capabilities.Opus48KHz) != (Capabilities) 0)
        source.Add("48000");
      if ((caps & Capabilities.Opus24KHz) != (Capabilities) 0)
        source.Add("24000");
      if ((caps & Capabilities.Opus16KHz) != (Capabilities) 0)
        source.Add("16000");
      if ((caps & Capabilities.Opus8KHz) != (Capabilities) 0)
        source.Add("8000");
      return source.Select<string, FunXMPP.ProtocolTreeNode>((Func<string, FunXMPP.ProtocolTreeNode>) (rate => new FunXMPP.ProtocolTreeNode("audio", new FunXMPP.KeyValue[2]
      {
        new FunXMPP.KeyValue("enc", "opus"),
        new FunXMPP.KeyValue(nameof (rate), rate)
      })));
    }

    private static IEnumerable<FunXMPP.ProtocolTreeNode> GetTransportCandidateNodes(
      ISignalingStruct signalStruct)
    {
      return Enumerable.Range(0, signalStruct.GetTransportCount()).Select<int, FunXMPP.ProtocolTreeNode>((Func<int, FunXMPP.ProtocolTreeNode>) (index =>
      {
        byte Priority;
        bool PortPredicting;
        byte[] Address;
        signalStruct.GetTransportCandidate(index, out Priority, out PortPredicting, out Address);
        return new FunXMPP.ProtocolTreeNode("te", new FunXMPP.KeyValue[2]
        {
          new FunXMPP.KeyValue("priority", Priority.ToString()),
          new FunXMPP.KeyValue("portpredicting", PortPredicting ? "1" : "0")
        }, Address);
      }));
    }

    private static IEnumerable<FunXMPP.ProtocolTreeNode> GetRelayLatencyNodes(
      ISignalingStruct signalStruct)
    {
      return Enumerable.Range(0, signalStruct.GetRelayLatencyCount()).Select<int, FunXMPP.ProtocolTreeNode>((Func<int, FunXMPP.ProtocolTreeNode>) (index =>
      {
        int Latency;
        byte[] Address;
        signalStruct.GetRelayLatency(index, out Latency, out Address);
        return new FunXMPP.ProtocolTreeNode("te", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("latency", Latency.ToString())
        }, Address);
      }));
    }

    private static bool CheckOfferDupe(string callId, string peerJid)
    {
      string callId1;
      string peerJid1;
      if (!Voip.Instance.GetCallMetadata(out callId1, out peerJid1) || callId1 == null || peerJid1 == null || !(callId == callId1) || !(peerJid == peerJid1))
        return false;
      Log.l("voip signaling", "Ignoring duplicate call offer {0} for peer {1}", (object) callId, (object) peerJid);
      return true;
    }

    public void OnSignalingData(byte[] payload, SignalingDataArgs args)
    {
      VoipSignaling.WriteVoipNode(payload, args.CallDuration >= 0 ? new SignalingDataArgs?(args) : new SignalingDataArgs?());
    }

    public class PendingOfferState
    {
      public ManagedCallProperties CallProperties;
      public string RemotePlatform;
      public string RemoteAppVersion;

      public void Process()
      {
        IVoip instance = Voip.Instance;
        instance.GetCallbacks().SetManagedCallProperties(Settings.MyJid, this.CallProperties);
        if (this.RemotePlatform == null || this.RemoteAppVersion == null)
          return;
        instance.SetRemotePlatform(this.RemotePlatform, this.RemoteAppVersion);
      }
    }
  }
}
