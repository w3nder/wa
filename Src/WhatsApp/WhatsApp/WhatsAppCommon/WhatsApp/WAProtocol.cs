// Decompiled with JetBrains decompiler
// Type: WhatsApp.WAProtocol
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using ICSharpCode.SharpZipLib.Silverlight.Zip.Compression.Streams;
using Microsoft.Phone.Info;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using WhatsApp.ProtoBuf;
using WhatsApp.WaCollections;
using WhatsAppNative;


namespace WhatsApp
{
  public class WAProtocol : FunXMPP.StanzaProvider, FunXMPP.StanzaWriter
  {
    private readonly string CertificateIssuer = "WhatsAppLongTerm1";
    private readonly byte[] CertificatePublicKey = new byte[32]
    {
      (byte) 20,
      (byte) 35,
      (byte) 117,
      (byte) 87,
      (byte) 77,
      (byte) 10,
      (byte) 88,
      (byte) 113,
      (byte) 102,
      (byte) 170,
      (byte) 231,
      (byte) 30,
      (byte) 190,
      (byte) 81,
      (byte) 100,
      (byte) 55,
      (byte) 196,
      (byte) 162,
      (byte) 139,
      (byte) 115,
      (byte) 227,
      (byte) 105,
      (byte) 92,
      (byte) 108,
      (byte) 225,
      (byte) 247,
      (byte) 249,
      (byte) 84,
      (byte) 93,
      (byte) 168,
      (byte) 238,
      (byte) 107
    };
    private readonly byte[] Header = new byte[4]
    {
      (byte) 87,
      (byte) 65,
      (byte) 3,
      (byte) FunXMPP.Dictionary.GetDictionaryVersion()
    };
    private readonly byte[] EdgeHeader = new byte[4]
    {
      (byte) 69,
      (byte) 68,
      (byte) 0,
      (byte) 1
    };
    private byte[] ClientStaticPrivate;
    private byte[] ClientStaticPublic;
    private byte[] ClientEphemeralPrivate;
    private byte[] ClientEphemeralPublic;
    private byte[] ServerStaticPublic;
    private IWASocket Socket;
    private FunXMPP.Connection Connection;
    private MemoryStream WriteStream = new MemoryStream();
    private string Username;
    private WAProtocol.HandshakeState State;
    private WAProtocol.HandshakeCipher Cipher;
    private byte[] WriteKey;
    private byte[] ReadKey;
    private int WriteNonce;
    private int ReadNonce;
    private const int FlagCompressed = 2;
    private const int FlagMoreFragments = 1;
    private const int MaxFragmentSize = 786432;
    private FunXMPP.TreeNodeWriter _treeNodeWriter = (FunXMPP.TreeNodeWriter) new FunXMPP.NullTreeNodeWriter();
    private const int FrameHeaderSize = 3;

    public event FunXMPP.ProcessStanzaHandler StanzaAvailable;

    public event FunXMPP.DecodeStanza DecodeStanza;

    public event EventHandler<FunXMPP.LoginEventArgs> LoggedIn;

    public FunXMPP.TreeNodeWriter TreeNodeWriter
    {
      get => this._treeNodeWriter;
      set => this._treeNodeWriter = value;
    }

    public WAProtocol()
    {
    }

    public WAProtocol(
      byte[] clientStaticPrivate,
      byte[] clientStaticPublic,
      byte[] serverStaticPublic,
      FunXMPP.Connection connection,
      IWASocket socket,
      FunRunner.SocketEventHandler handler,
      string username)
    {
      this.ClientStaticPrivate = clientStaticPrivate;
      this.ClientStaticPublic = clientStaticPublic;
      this.ServerStaticPublic = serverStaticPublic;
      this.Connection = connection;
      this.Socket = socket;
      this.Username = username;
      handler.BytesAvailable += new FunRunner.SocketEventHandler.BytesHandler(this.OnBytesAvailable);
      Curve22519Extensions.GenKeyPair(out this.ClientEphemeralPublic, out this.ClientEphemeralPrivate);
    }

    public void Connect()
    {
      byte[] edgeRoutingInfo = Settings.EdgeRoutingInfo;
      if (edgeRoutingInfo != null && edgeRoutingInfo.Length != 0)
      {
        this.Socket.Send(this.EdgeHeader);
        this.Socket.Send(WAProtocol.MediumToByteArray(edgeRoutingInfo.Length));
        this.Socket.Send(edgeRoutingInfo);
      }
      this.Socket.Send(this.Header);
      if (this.ServerStaticPublic == null)
        this.SendClientHello();
      else
        this.SendClientResume();
    }

    private void SendClientHello()
    {
      this.Cipher = new WAProtocol.HandshakeCipher(WAProtocol.HandshakeCipher.FULL_HANDSHAKE, this.Header);
      this.WriteFrameInternal(HandshakeMessage.SerializeToBytes(new HandshakeMessage()
      {
        ClientHelloField = new HandshakeMessage.ClientHello()
        {
          Ephemeral = this.Cipher.encryptEphemeralKey(this.ClientEphemeralPublic)
        }
      }));
      this.State = WAProtocol.HandshakeState.ClientHello;
    }

    private void SendClientResume()
    {
      this.Cipher = new WAProtocol.HandshakeCipher(WAProtocol.HandshakeCipher.RESUME_HANDSHAKE, this.Header);
      byte[] pubKey = this.Cipher.decryptStaticKey(this.ServerStaticPublic);
      byte[] numArray1 = this.Cipher.encryptEphemeralKey(this.ClientEphemeralPublic);
      this.Cipher.setKey(Curve22519Extensions.Derive(pubKey, this.ClientEphemeralPrivate));
      byte[] numArray2 = this.Cipher.encryptStaticKey(this.ClientStaticPublic);
      this.Cipher.setKey(Curve22519Extensions.Derive(pubKey, this.ClientStaticPrivate));
      byte[] numArray3 = this.Cipher.encryptPayload(this.BuildClientPayload());
      this.WriteFrameInternal(HandshakeMessage.SerializeToBytes(new HandshakeMessage()
      {
        ClientHelloField = new HandshakeMessage.ClientHello()
        {
          Ephemeral = numArray1,
          Static = numArray2,
          Payload = numArray3
        }
      }));
      this.State = WAProtocol.HandshakeState.ClientResume;
    }

    private void ReceiveServerResume(byte[] handshakeMessage)
    {
      HandshakeMessage.ServerHello serverHelloField = HandshakeMessage.Deserialize(handshakeMessage).ServerHelloField;
      if (serverHelloField == null)
        throw new InvalidOperationException();
      if (serverHelloField.Static != null)
      {
        this.ReceiveServerFallback(serverHelloField);
      }
      else
      {
        byte[] pubKey = this.Cipher.decryptEphemeralKey(serverHelloField.Ephemeral);
        this.Cipher.setKey(Curve22519Extensions.Derive(pubKey, this.ClientEphemeralPrivate));
        this.Cipher.setKey(Curve22519Extensions.Derive(pubKey, this.ClientStaticPrivate));
        byte[] buffer = this.Cipher.decryptPayload(serverHelloField.Payload);
        if (buffer != null && buffer.Length != 0)
        {
          NoiseCertificate certificate = NoiseCertificate.Deserialize(buffer);
          NoiseCertificate.Details details = NoiseCertificate.Details.Deserialize(certificate.DetailsField);
          if (!this.ValidateCertificate(certificate, details))
            throw new InvalidOperationException("Untrusted server cert");
        }
        this.GeneratedAuthenticatedCipher();
      }
    }

    private void ReceiveServerFallback(HandshakeMessage.ServerHello serverHello)
    {
      this.Cipher = new WAProtocol.HandshakeCipher(WAProtocol.HandshakeCipher.FALLBACK_HANDSHAKE, this.Header);
      this.Cipher.encryptEphemeralKey(this.ClientEphemeralPublic);
      this.ReceiveServerHello(serverHello);
    }

    private void ReceiveServerHandshake(byte[] handshakeMessage)
    {
      this.ReceiveServerHello(HandshakeMessage.Deserialize(handshakeMessage).ServerHelloField ?? throw new InvalidOperationException());
    }

    private void ReceiveServerHello(HandshakeMessage.ServerHello serverHello)
    {
      byte[] pubKey = this.Cipher.decryptEphemeralKey(serverHello.Ephemeral);
      this.Cipher.setKey(Curve22519Extensions.Derive(pubKey, this.ClientEphemeralPrivate));
      this.Cipher.setKey(Curve22519Extensions.Derive(this.Cipher.decryptStaticKey(serverHello.Static), this.ClientEphemeralPrivate));
      NoiseCertificate certificate = NoiseCertificate.Deserialize(this.Cipher.decryptPayload(serverHello.Payload));
      NoiseCertificate.Details details = NoiseCertificate.Details.Deserialize(certificate.DetailsField);
      Settings.ServerStaticPublicKey = this.ValidateCertificate(certificate, details) ? details.Key : throw new InvalidOperationException("Untrusted server cert");
      byte[] numArray1 = this.Cipher.encryptStaticKey(this.ClientStaticPublic);
      this.Cipher.setKey(Curve22519Extensions.Derive(pubKey, this.ClientStaticPrivate));
      byte[] numArray2 = this.Cipher.encryptPayload(this.BuildClientPayload());
      this.WriteFrameInternal(HandshakeMessage.SerializeToBytes(new HandshakeMessage()
      {
        ClientFinishField = new HandshakeMessage.ClientFinish()
        {
          Static = numArray1,
          Payload = numArray2
        }
      }));
      this.GeneratedAuthenticatedCipher();
    }

    private byte[] BuildClientPayload()
    {
      string[] strArray = AppState.GetAppVersion().Split('.');
      string str1 = "000";
      string str2 = "000";
      try
      {
        CELL_INFO cellInfo = NativeInterfaces.Misc.GetCellInfo(CellInfoFlags.MccMnc);
        str1 = cellInfo.Mcc.ToString().PadLeft(3, '0');
        str2 = cellInfo.Mnc.ToString().PadLeft(3, '0');
      }
      catch (Exception ex)
      {
      }
      Version osVersion = AppState.OSVersion;
      byte[] id = this.Connection.EventHandler.Qr.Session.Id;
      string lang;
      string locale;
      AppState.GetLangAndLocale(out lang, out locale);
      ClientPayload instance = new ClientPayload();
      instance.Username = !string.IsNullOrEmpty(this.Username) ? new ulong?(ulong.Parse(this.Username)) : new ulong?();
      instance.Passive = new bool?(this.Connection.ShouldConnectWithPassive());
      instance.PushName = Settings.PushName;
      instance.connect_type = new ClientPayload.ConnectType?(WAProtocol.CurrentConnectionType());
      instance.UserAgentField = new ClientPayload.UserAgent()
      {
        platform = new ClientPayload.UserAgent.Platform?(ClientPayload.UserAgent.Platform.WINDOWS_PHONE),
        AppVersionField = new ClientPayload.UserAgent.AppVersion()
        {
          Primary = new uint?(strArray.Length != 0 ? uint.Parse(strArray[0]) : 0U),
          Secondary = new uint?(strArray.Length > 1 ? uint.Parse(strArray[1]) : 0U),
          Tertiary = new uint?(strArray.Length > 2 ? uint.Parse(strArray[2]) : 0U),
          Quaternary = new uint?(strArray.Length > 3 ? uint.Parse(strArray[3]) : 0U)
        },
        Mcc = str1,
        Mnc = str2,
        OsVersion = string.Format("{0}.{1}.{2}", (object) osVersion.Major, (object) osVersion.Minor, (object) osVersion.Build),
        Manufacturer = DeviceStatus.DeviceManufacturer,
        Device = string.Format("{0} H{1}", (object) DeviceStatus.DeviceName, (object) DeviceStatus.DeviceHardwareVersion),
        OsBuildNumber = osVersion.Build.ToString(),
        PhoneId = Settings.FacebookPhoneId,
        LocaleLanguageIso6391 = lang,
        LocaleCountryIso31661Alpha2 = locale,
        release_channel = new ClientPayload.UserAgent.ReleaseChannel?(Constants.ReleaseChannel)
      };
      ClientPayload.WebInfo webInfo;
      if (id == null)
      {
        webInfo = (ClientPayload.WebInfo) null;
      }
      else
      {
        webInfo = new ClientPayload.WebInfo();
        webInfo.RefToken = Encoding.UTF8.GetString(id, 0, id.Length);
        webInfo.Version = "0.17.10";
        webInfo.WebdPayloadField = QrSession.WebdPayload;
      }
      instance.WebInfoField = webInfo;
      return ClientPayload.SerializeToBytes(instance);
    }

    private static ClientPayload.ConnectType CurrentConnectionType()
    {
      wam_enum_radio_type? nullable1 = NetworkStateMonitor.IsWifiDataConnected() ? new wam_enum_radio_type?(wam_enum_radio_type.WIFI_UNKNOWN) : NetworkStateMonitor.GetConnectedCellularFSType();
      wam_enum_radio_type? nullable2 = nullable1;
      wam_enum_radio_type wamEnumRadioType = wam_enum_radio_type.WIFI_UNKNOWN;
      if ((nullable2.GetValueOrDefault() == wamEnumRadioType ? (nullable2.HasValue ? 1 : 0) : 0) != 0)
        return ClientPayload.ConnectType.WIFI_UNKNOWN;
      return nullable1.HasValue && nullable1.Value >= wam_enum_radio_type.CELLULAR_EDGE && nullable1.Value <= wam_enum_radio_type.CELLULAR_HSPAP ? (ClientPayload.ConnectType) nullable1.Value : ClientPayload.ConnectType.CELLULAR_UNKNOWN;
    }

    private bool ValidateCertificate(NoiseCertificate certificate, NoiseCertificate.Details details)
    {
      DateTime? dt;
      return !(details.Issuer != this.CertificateIssuer) && (!details.Expires.HasValue || FunXMPP.TryParseTimestamp((long) details.Expires.Value, out dt) && !(dt.Value < DateTime.Now)) && Curve22519Extensions.Verify(certificate.DetailsField, certificate.Signature, this.CertificatePublicKey);
    }

    private void GeneratedAuthenticatedCipher()
    {
      Pair<byte[], byte[]> noiseCipher = this.Cipher.getNoiseCipher();
      this.WriteKey = noiseCipher.First;
      this.ReadKey = noiseCipher.Second;
      this.State = WAProtocol.HandshakeState.Authenticated;
    }

    private void ProcessFrame(byte[] frame, int offset, int length)
    {
      MemoryStream memoryStream = (MemoryStream) null;
      byte[] numArray = (byte[]) null;
      switch (this.State)
      {
        case WAProtocol.HandshakeState.ClientHello:
        case WAProtocol.HandshakeState.ClientResume:
          numArray = new byte[length];
          Array.Copy((Array) frame, offset, (Array) numArray, 0, length);
          break;
        case WAProtocol.HandshakeState.Authenticated:
        case WAProtocol.HandshakeState.LoggedIn:
          byte[] buffer = MbedtlsExtensions.AesGcmDecrypt(this.ReadKey, WAProtocol.LongToByteArray((long) this.ReadNonce++, 12), (byte[]) null, frame, new int?(offset), new int?(length));
          if (((int) buffer[0] & 2) != 0)
          {
            memoryStream = new MemoryStream();
            using (MemoryStream baseInputStream = new MemoryStream(buffer, 1, buffer.Length - 1, false))
            {
              using (InflaterInputStream inflaterInputStream = new InflaterInputStream((Stream) baseInputStream))
                inflaterInputStream.CopyTo((Stream) memoryStream);
            }
            memoryStream.Position = 0L;
            break;
          }
          memoryStream = new MemoryStream(buffer, 1, buffer.Length - 1, false);
          break;
      }
      switch (this.State)
      {
        case WAProtocol.HandshakeState.ClientHello:
          this.ReceiveServerHandshake(numArray);
          break;
        case WAProtocol.HandshakeState.ClientResume:
          this.ReceiveServerResume(numArray);
          break;
        case WAProtocol.HandshakeState.Authenticated:
          FunXMPP.DecodeStanza decodeStanza = this.DecodeStanza;
          if (decodeStanza == null)
            break;
          this.ProcessAuthenticationNode(decodeStanza((Stream) memoryStream));
          break;
        case WAProtocol.HandshakeState.LoggedIn:
          FunXMPP.ProcessStanzaHandler stanzaAvailable = this.StanzaAvailable;
          if (stanzaAvailable == null)
            break;
          stanzaAvailable((Stream) memoryStream);
          break;
      }
    }

    public void WriteStanza(MemoryStream stanzaStream, bool useCompression)
    {
      MemoryStream memoryStream = new MemoryStream();
      long num = 0;
      bool flag = false;
      if (useCompression && stanzaStream.Length < 4294967296L)
      {
        using (DeflaterOutputStream destination = new DeflaterOutputStream((Stream) memoryStream))
        {
          memoryStream.WriteByte((byte) 2);
          stanzaStream.CopyTo((Stream) destination);
          destination.Finish();
          if (memoryStream.Length < stanzaStream.Length)
          {
            num = memoryStream.Length;
            flag = true;
          }
          else
          {
            memoryStream = new MemoryStream();
            stanzaStream.Position = 0L;
          }
        }
      }
      if (!flag)
      {
        memoryStream.WriteByte((byte) 0);
        stanzaStream.CopyTo((Stream) memoryStream);
        num = memoryStream.Length;
      }
      if (num >= 33554432L)
        throw new IOException("Buffer too large: " + (object) memoryStream.Length);
      this.WriteFrameInternal(MbedtlsExtensions.AesGcmEncrypt(this.WriteKey, WAProtocol.LongToByteArray((long) this.WriteNonce++, 12), (byte[]) null, memoryStream.GetBuffer(), new int?(0), new int?((int) num)));
    }

    private void WriteFrameInternal(byte[] frame)
    {
      byte[] byteArray = WAProtocol.MediumToByteArray(frame.Length);
      this.WriteStream.Write(byteArray, 0, byteArray.Length);
      this.WriteStream.Write(frame, 0, frame.Length);
      this.Socket.Send(this.WriteStream.GetBuffer(), 0, (int) this.WriteStream.Length);
      this.WriteStream.Position = 0L;
      this.WriteStream.SetLength(0L);
    }

    public int OnBytesAvailable(byte[] buffer, int offset, int length)
    {
      int index = offset;
      while (length > 0 && length >= 3)
      {
        int length1 = ((int) buffer[index] << 16) + ((int) buffer[index + 1] << 8) + (int) buffer[index + 2];
        length -= 3;
        if (length >= length1)
        {
          this.ProcessFrame(buffer, index + 3, length1);
          length -= length1;
          index += 3 + length1;
        }
        else
          break;
      }
      return index - offset;
    }

    public static void GenerateClientStaticKeyPair(
      out byte[] staticPublic,
      out byte[] staticPrivate)
    {
      Curve22519Extensions.GenKeyPair(out staticPublic, out staticPrivate);
    }

    public static void GenerateClientStaticKeyPair()
    {
      byte[] staticPublic;
      byte[] staticPrivate;
      WAProtocol.GenerateClientStaticKeyPair(out staticPublic, out staticPrivate);
      Settings.ClientStaticPrivateKey = staticPrivate;
      Settings.ClientStaticPublicKey = staticPublic;
    }

    public void ProcessAuthenticationNode(FunXMPP.ProtocolTreeNode node)
    {
      if (FunXMPP.ProtocolTreeNode.TagEquals(node, "web"))
      {
        FunXMPP.ProtocolTreeNode child = node.GetChild("error");
        if (child != null)
        {
          string attributeValue = child.GetAttributeValue("code");
          int result = 500;
          if (attributeValue != null)
            int.TryParse(attributeValue, out result);
          this.Connection.EventHandler.Qr.OnQrError(result);
        }
        else
          this.Connection.EventHandler.Qr.OnQrOnline(new bool?());
      }
      else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "success"))
      {
        DateTime? attributeDateTime = node.GetAttributeDateTime("t");
        string attributeValue = node.GetAttributeValue("props");
        int result = 0;
        if (attributeValue != null && int.TryParse(attributeValue, out result) && result != Settings.ServerPropsVersion)
        {
          Settings.ForceServerPropsReload = true;
          Settings.ServerPropsVersion = result;
        }
        FieldStats.LastDataCenterUsed = node.GetAttributeValue("location");
        this.State = WAProtocol.HandshakeState.LoggedIn;
        EventHandler<FunXMPP.LoginEventArgs> loggedIn = this.LoggedIn;
        if (loggedIn == null)
          return;
        loggedIn((object) this, new FunXMPP.LoginEventArgs()
        {
          ServerTime = attributeDateTime
        });
      }
      else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "failure"))
      {
        FieldStats.LastDataCenterUsed = node.GetAttributeValue("location");
        string attributeValue1 = node.GetAttributeValue("expire");
        string attributeValue2 = node.GetAttributeValue("code");
        string attributeValue3 = node.GetAttributeValue("reason");
        int result1 = 0;
        WAProtocol.LoginFailedReason type = WAProtocol.LoginFailedReason.GenericFailure;
        long result2 = 0;
        if (attributeValue3 != null && int.TryParse(attributeValue3, out result1))
        {
          if (Enum.IsDefined(typeof (WAProtocol.LoginFailedReason), (object) result1))
            type = (WAProtocol.LoginFailedReason) result1;
          else if (result1 > 500)
            type = WAProtocol.LoginFailedReason.ServerBackoffRequest;
        }
        FunXMPP.LoginFailureException failureException1 = new FunXMPP.LoginFailureException(type);
        if (type == WAProtocol.LoginFailedReason.TempBanned)
        {
          if (attributeValue2 != null && long.TryParse(attributeValue1, out result2) && result2 > 0L)
          {
            failureException1.BanReason = attributeValue2;
            failureException1.FailedLoginReason = attributeValue3;
            FunXMPP.LoginFailureException failureException2 = failureException1;
            DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
            DateTime? nullable1 = new DateTime?(currentServerTimeUtc.AddSeconds((double) result2));
            failureException2.BanExpirationUtc = nullable1;
            failureException1.BanTotalSeconds = new long?(result2);
            string attributeValue4 = node.GetAttributeValue("retry");
            long num = 0;
            ref long local = ref num;
            if (long.TryParse(attributeValue4, out local) && num > 0L)
            {
              FunXMPP.LoginFailureException failureException3 = failureException1;
              currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
              DateTime? nullable2 = new DateTime?(currentServerTimeUtc.AddSeconds((double) num));
              failureException3.RetryUtc = nullable2;
            }
          }
          else
            failureException1.Type = WAProtocol.LoginFailedReason.GenericFailure;
        }
        throw failureException1;
      }
    }

    private static byte[] MediumToByteArray(int value)
    {
      byte[] byteArray = new byte[3]
      {
        (byte) 0,
        (byte) 0,
        (byte) value
      };
      byteArray[1] = (byte) (value >> 8);
      byteArray[0] = (byte) (value >> 16);
      return byteArray;
    }

    public static byte[] LongToByteArray(long value, int length)
    {
      byte[] byteArray = new byte[length];
      int index = length - 8;
      byteArray[index + 7] = (byte) value;
      byteArray[index + 6] = (byte) (value >> 8);
      byteArray[index + 5] = (byte) (value >> 16);
      byteArray[index + 4] = (byte) (value >> 24);
      byteArray[index + 3] = (byte) (value >> 32);
      byteArray[index + 2] = (byte) (value >> 40);
      byteArray[index + 1] = (byte) (value >> 48);
      byteArray[index] = (byte) (value >> 56);
      return byteArray;
    }

    private enum HandshakeState
    {
      ClientHello,
      ClientResume,
      Authenticated,
      LoggedIn,
    }

    public enum LoginFailedReason
    {
      GenericFailure = 400, // 0x00000190
      NotAuthorized = 401, // 0x00000191
      TempBanned = 402, // 0x00000192
      Banned = 403, // 0x00000193
      ClientTooOld = 405, // 0x00000195
      BadUserAgent = 409, // 0x00000199
      ServerError = 500, // 0x000001F4
      Experimental = 501, // 0x000001F5
      ServerBackoffRequest = 503, // 0x000001F7
    }

    private class HandshakeCipher
    {
      public static readonly byte[] FULL_HANDSHAKE = new byte[32]
      {
        (byte) 78,
        (byte) 111,
        (byte) 105,
        (byte) 115,
        (byte) 101,
        (byte) 95,
        (byte) 88,
        (byte) 88,
        (byte) 95,
        (byte) 50,
        (byte) 53,
        (byte) 53,
        (byte) 49,
        (byte) 57,
        (byte) 95,
        (byte) 65,
        (byte) 69,
        (byte) 83,
        (byte) 71,
        (byte) 67,
        (byte) 77,
        (byte) 95,
        (byte) 83,
        (byte) 72,
        (byte) 65,
        (byte) 50,
        (byte) 53,
        (byte) 54,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      };
      public static readonly byte[] RESUME_HANDSHAKE = new byte[32]
      {
        (byte) 78,
        (byte) 111,
        (byte) 105,
        (byte) 115,
        (byte) 101,
        (byte) 95,
        (byte) 73,
        (byte) 75,
        (byte) 95,
        (byte) 50,
        (byte) 53,
        (byte) 53,
        (byte) 49,
        (byte) 57,
        (byte) 95,
        (byte) 65,
        (byte) 69,
        (byte) 83,
        (byte) 71,
        (byte) 67,
        (byte) 77,
        (byte) 95,
        (byte) 83,
        (byte) 72,
        (byte) 65,
        (byte) 50,
        (byte) 53,
        (byte) 54,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      };
      public static readonly byte[] FALLBACK_HANDSHAKE = new byte[36]
      {
        (byte) 78,
        (byte) 111,
        (byte) 105,
        (byte) 115,
        (byte) 101,
        (byte) 95,
        (byte) 88,
        (byte) 88,
        (byte) 102,
        (byte) 97,
        (byte) 108,
        (byte) 108,
        (byte) 98,
        (byte) 97,
        (byte) 99,
        (byte) 107,
        (byte) 95,
        (byte) 50,
        (byte) 53,
        (byte) 53,
        (byte) 49,
        (byte) 57,
        (byte) 95,
        (byte) 65,
        (byte) 69,
        (byte) 83,
        (byte) 71,
        (byte) 67,
        (byte) 77,
        (byte) 95,
        (byte) 83,
        (byte) 72,
        (byte) 65,
        (byte) 50,
        (byte) 53,
        (byte) 54
      };
      private WAProtocol.HandshakeHash hash;
      private byte[] chainKey;
      private long nonce;
      private byte[] cipherKey;

      public HandshakeCipher(byte[] handshakeName, byte[] version)
      {
        this.hash = new WAProtocol.HandshakeHash(handshakeName);
        this.chainKey = this.hash.Hash;
        this.hash.Update(version);
      }

      public byte[] encryptStaticKey(byte[] publicKey) => this.encryptPayload(publicKey);

      public byte[] decryptStaticKey(byte[] encrypted) => this.decryptPayload(encrypted);

      public byte[] encryptEphemeralKey(byte[] publicKey)
      {
        this.hash.Update(publicKey);
        return publicKey;
      }

      public byte[] decryptEphemeralKey(byte[] encrypted)
      {
        this.hash.Update(encrypted);
        return encrypted;
      }

      public byte[] encryptPayload(byte[] plaintext)
      {
        byte[] buffer = this.cipherKey == null ? plaintext : MbedtlsExtensions.AesGcmEncrypt(this.cipherKey, WAProtocol.LongToByteArray(this.nonce++, 12), this.hash.Hash, plaintext);
        this.hash.Update(buffer);
        return buffer;
      }

      public byte[] decryptPayload(byte[] ciphertext)
      {
        byte[] numArray = this.cipherKey == null ? ciphertext : MbedtlsExtensions.AesGcmDecrypt(this.cipherKey, WAProtocol.LongToByteArray(this.nonce++, 12), this.hash.Hash, ciphertext);
        this.hash.Update(ciphertext);
        return numArray;
      }

      public void setKey(byte[] agreement)
      {
        byte[] sourceArray = HkdfSha256.Perform(64, agreement, this.chainKey);
        this.nonce = 0L;
        this.chainKey = new byte[32];
        this.cipherKey = new byte[32];
        Array.Copy((Array) sourceArray, 0, (Array) this.chainKey, 0, this.chainKey.Length);
        Array.Copy((Array) sourceArray, this.chainKey.Length, (Array) this.cipherKey, 0, this.cipherKey.Length);
      }

      public Pair<byte[], byte[]> getNoiseCipher()
      {
        byte[] sourceArray = HkdfSha256.Perform(64, new byte[0], this.chainKey);
        byte[] numArray1 = new byte[32];
        byte[] numArray2 = new byte[32];
        Array.Copy((Array) sourceArray, 0, (Array) numArray1, 0, numArray1.Length);
        Array.Copy((Array) sourceArray, numArray1.Length, (Array) numArray2, 0, numArray2.Length);
        return new Pair<byte[], byte[]>(numArray1, numArray2);
      }
    }

    private class HandshakeHash
    {
      public HandshakeHash(byte[] initial)
      {
        if (initial.Length <= 32)
        {
          this.Hash = initial;
        }
        else
        {
          using (SHA256Managed shA256Managed = new SHA256Managed())
            this.Hash = shA256Managed.ComputeHash(initial);
        }
      }

      public void Update(byte[] buffer)
      {
        using (SHA256Managed shA256Managed = new SHA256Managed())
        {
          shA256Managed.TransformBlock(this.Hash, 0, this.Hash.Length, this.Hash, 0);
          shA256Managed.TransformFinalBlock(buffer, 0, buffer.Length);
          this.Hash = shA256Managed.Hash;
        }
      }

      public byte[] Hash { get; private set; }
    }
  }
}
