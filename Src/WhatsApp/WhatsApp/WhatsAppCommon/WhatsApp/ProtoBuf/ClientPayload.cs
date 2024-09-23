// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.ClientPayload
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace WhatsApp.ProtoBuf
{
  public class ClientPayload
  {
    public ulong? Username { get; set; }

    public bool? Passive { get; set; }

    public List<ClientPayload.ClientFeature> ClientFeatures { get; set; }

    public ClientPayload.UserAgent UserAgentField { get; set; }

    public ClientPayload.WebInfo WebInfoField { get; set; }

    public string PushName { get; set; }

    public int? SessionId { get; set; }

    public bool? ShortConnect { get; set; }

    public ClientPayload.IOSAppExtension? IosAppExtension { get; set; }

    public ClientPayload.ConnectType? connect_type { get; set; }

    public ClientPayload.ConnectReason? connect_reason { get; set; }

    public List<int> Shards { get; set; }

    public ClientPayload.DNSSource DnsSource { get; set; }

    public uint? ConnectAttemptCount { get; set; }

    public static ClientPayload Deserialize(Stream stream)
    {
      ClientPayload instance = new ClientPayload();
      ClientPayload.Deserialize(stream, instance);
      return instance;
    }

    public static ClientPayload DeserializeLengthDelimited(Stream stream)
    {
      ClientPayload instance = new ClientPayload();
      ClientPayload.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static ClientPayload DeserializeLength(Stream stream, int length)
    {
      ClientPayload instance = new ClientPayload();
      ClientPayload.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static ClientPayload Deserialize(byte[] buffer)
    {
      ClientPayload instance = new ClientPayload();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        ClientPayload.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static ClientPayload Deserialize(byte[] buffer, ClientPayload instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        ClientPayload.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static ClientPayload Deserialize(Stream stream, ClientPayload instance)
    {
      BinaryReader binaryReader = new BinaryReader(stream);
      if (instance.ClientFeatures == null)
        instance.ClientFeatures = new List<ClientPayload.ClientFeature>();
      if (instance.Shards == null)
        instance.Shards = new List<int>();
      while (true)
      {
        SilentOrbit.ProtocolBuffers.Key key;
        do
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_30;
            case 8:
              instance.Username = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 24:
              instance.Passive = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 32:
              instance.ClientFeatures.Add((ClientPayload.ClientFeature) ProtocolParser.ReadUInt64(stream));
              continue;
            case 42:
              if (instance.UserAgentField == null)
              {
                instance.UserAgentField = ClientPayload.UserAgent.DeserializeLengthDelimited(stream);
                continue;
              }
              ClientPayload.UserAgent.DeserializeLengthDelimited(stream, instance.UserAgentField);
              continue;
            case 50:
              if (instance.WebInfoField == null)
              {
                instance.WebInfoField = ClientPayload.WebInfo.DeserializeLengthDelimited(stream);
                continue;
              }
              ClientPayload.WebInfo.DeserializeLengthDelimited(stream, instance.WebInfoField);
              continue;
            case 58:
              instance.PushName = ProtocolParser.ReadString(stream);
              continue;
            case 77:
              instance.SessionId = new int?(binaryReader.ReadInt32());
              continue;
            case 80:
              instance.ShortConnect = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 96:
              instance.connect_type = new ClientPayload.ConnectType?((ClientPayload.ConnectType) ProtocolParser.ReadUInt64(stream));
              continue;
            case 104:
              instance.connect_reason = new ClientPayload.ConnectReason?((ClientPayload.ConnectReason) ProtocolParser.ReadUInt64(stream));
              continue;
            case 112:
              instance.Shards.Add((int) ProtocolParser.ReadUInt64(stream));
              continue;
            case 122:
              if (instance.DnsSource == null)
              {
                instance.DnsSource = ClientPayload.DNSSource.DeserializeLengthDelimited(stream);
                continue;
              }
              ClientPayload.DNSSource.DeserializeLengthDelimited(stream, instance.DnsSource);
              continue;
            default:
              key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  continue;
                case 30:
                  if (key.WireType == Wire.Varint)
                  {
                    instance.IosAppExtension = new ClientPayload.IOSAppExtension?((ClientPayload.IOSAppExtension) ProtocolParser.ReadUInt64(stream));
                    continue;
                  }
                  continue;
                default:
                  goto label_29;
              }
          }
        }
        while (key.WireType != Wire.Varint);
        instance.ConnectAttemptCount = new uint?(ProtocolParser.ReadUInt32(stream));
        continue;
label_29:
        ProtocolParser.SkipKey(stream, key);
      }
label_30:
      return instance;
    }

    public static ClientPayload DeserializeLengthDelimited(Stream stream, ClientPayload instance)
    {
      BinaryReader binaryReader = new BinaryReader(stream);
      if (instance.ClientFeatures == null)
        instance.ClientFeatures = new List<ClientPayload.ClientFeature>();
      if (instance.Shards == null)
        instance.Shards = new List<int>();
      long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
      while (stream.Position < num)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            throw new EndOfStreamException();
          case 8:
            instance.Username = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 24:
            instance.Passive = new bool?(ProtocolParser.ReadBool(stream));
            continue;
          case 32:
            instance.ClientFeatures.Add((ClientPayload.ClientFeature) ProtocolParser.ReadUInt64(stream));
            continue;
          case 42:
            if (instance.UserAgentField == null)
            {
              instance.UserAgentField = ClientPayload.UserAgent.DeserializeLengthDelimited(stream);
              continue;
            }
            ClientPayload.UserAgent.DeserializeLengthDelimited(stream, instance.UserAgentField);
            continue;
          case 50:
            if (instance.WebInfoField == null)
            {
              instance.WebInfoField = ClientPayload.WebInfo.DeserializeLengthDelimited(stream);
              continue;
            }
            ClientPayload.WebInfo.DeserializeLengthDelimited(stream, instance.WebInfoField);
            continue;
          case 58:
            instance.PushName = ProtocolParser.ReadString(stream);
            continue;
          case 77:
            instance.SessionId = new int?(binaryReader.ReadInt32());
            continue;
          case 80:
            instance.ShortConnect = new bool?(ProtocolParser.ReadBool(stream));
            continue;
          case 96:
            instance.connect_type = new ClientPayload.ConnectType?((ClientPayload.ConnectType) ProtocolParser.ReadUInt64(stream));
            continue;
          case 104:
            instance.connect_reason = new ClientPayload.ConnectReason?((ClientPayload.ConnectReason) ProtocolParser.ReadUInt64(stream));
            continue;
          case 112:
            instance.Shards.Add((int) ProtocolParser.ReadUInt64(stream));
            continue;
          case 122:
            if (instance.DnsSource == null)
            {
              instance.DnsSource = ClientPayload.DNSSource.DeserializeLengthDelimited(stream);
              continue;
            }
            ClientPayload.DNSSource.DeserializeLengthDelimited(stream, instance.DnsSource);
            continue;
          default:
            SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
            switch (key.Field)
            {
              case 0:
                throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
              case 16:
                if (key.WireType == Wire.Varint)
                {
                  instance.ConnectAttemptCount = new uint?(ProtocolParser.ReadUInt32(stream));
                  continue;
                }
                continue;
              case 30:
                if (key.WireType == Wire.Varint)
                {
                  instance.IosAppExtension = new ClientPayload.IOSAppExtension?((ClientPayload.IOSAppExtension) ProtocolParser.ReadUInt64(stream));
                  continue;
                }
                continue;
              default:
                ProtocolParser.SkipKey(stream, key);
                continue;
            }
        }
      }
      if (stream.Position != num)
        throw new ProtocolBufferException("Read past max limit");
      return instance;
    }

    public static ClientPayload DeserializeLength(
      Stream stream,
      int length,
      ClientPayload instance)
    {
      BinaryReader binaryReader = new BinaryReader(stream);
      if (instance.ClientFeatures == null)
        instance.ClientFeatures = new List<ClientPayload.ClientFeature>();
      if (instance.Shards == null)
        instance.Shards = new List<int>();
      long num = stream.Position + (long) length;
      while (stream.Position < num)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            throw new EndOfStreamException();
          case 8:
            instance.Username = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 24:
            instance.Passive = new bool?(ProtocolParser.ReadBool(stream));
            continue;
          case 32:
            instance.ClientFeatures.Add((ClientPayload.ClientFeature) ProtocolParser.ReadUInt64(stream));
            continue;
          case 42:
            if (instance.UserAgentField == null)
            {
              instance.UserAgentField = ClientPayload.UserAgent.DeserializeLengthDelimited(stream);
              continue;
            }
            ClientPayload.UserAgent.DeserializeLengthDelimited(stream, instance.UserAgentField);
            continue;
          case 50:
            if (instance.WebInfoField == null)
            {
              instance.WebInfoField = ClientPayload.WebInfo.DeserializeLengthDelimited(stream);
              continue;
            }
            ClientPayload.WebInfo.DeserializeLengthDelimited(stream, instance.WebInfoField);
            continue;
          case 58:
            instance.PushName = ProtocolParser.ReadString(stream);
            continue;
          case 77:
            instance.SessionId = new int?(binaryReader.ReadInt32());
            continue;
          case 80:
            instance.ShortConnect = new bool?(ProtocolParser.ReadBool(stream));
            continue;
          case 96:
            instance.connect_type = new ClientPayload.ConnectType?((ClientPayload.ConnectType) ProtocolParser.ReadUInt64(stream));
            continue;
          case 104:
            instance.connect_reason = new ClientPayload.ConnectReason?((ClientPayload.ConnectReason) ProtocolParser.ReadUInt64(stream));
            continue;
          case 112:
            instance.Shards.Add((int) ProtocolParser.ReadUInt64(stream));
            continue;
          case 122:
            if (instance.DnsSource == null)
            {
              instance.DnsSource = ClientPayload.DNSSource.DeserializeLengthDelimited(stream);
              continue;
            }
            ClientPayload.DNSSource.DeserializeLengthDelimited(stream, instance.DnsSource);
            continue;
          default:
            SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
            switch (key.Field)
            {
              case 0:
                throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
              case 16:
                if (key.WireType == Wire.Varint)
                {
                  instance.ConnectAttemptCount = new uint?(ProtocolParser.ReadUInt32(stream));
                  continue;
                }
                continue;
              case 30:
                if (key.WireType == Wire.Varint)
                {
                  instance.IosAppExtension = new ClientPayload.IOSAppExtension?((ClientPayload.IOSAppExtension) ProtocolParser.ReadUInt64(stream));
                  continue;
                }
                continue;
              default:
                ProtocolParser.SkipKey(stream, key);
                continue;
            }
        }
      }
      if (stream.Position != num)
        throw new ProtocolBufferException("Read past max limit");
      return instance;
    }

    public static void Serialize(Stream stream, ClientPayload instance)
    {
      BinaryWriter binaryWriter = new BinaryWriter(stream);
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.Username.HasValue)
      {
        stream.WriteByte((byte) 8);
        ProtocolParser.WriteUInt64(stream, instance.Username.Value);
      }
      if (instance.Passive.HasValue)
      {
        stream.WriteByte((byte) 24);
        ProtocolParser.WriteBool(stream, instance.Passive.Value);
      }
      if (instance.ClientFeatures != null)
      {
        foreach (ClientPayload.ClientFeature clientFeature in instance.ClientFeatures)
        {
          stream.WriteByte((byte) 32);
          ProtocolParser.WriteUInt64(stream, (ulong) clientFeature);
        }
      }
      if (instance.UserAgentField != null)
      {
        stream.WriteByte((byte) 42);
        stream1.SetLength(0L);
        ClientPayload.UserAgent.Serialize((Stream) stream1, instance.UserAgentField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.WebInfoField != null)
      {
        stream.WriteByte((byte) 50);
        stream1.SetLength(0L);
        ClientPayload.WebInfo.Serialize((Stream) stream1, instance.WebInfoField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.PushName != null)
      {
        stream.WriteByte((byte) 58);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.PushName));
      }
      if (instance.SessionId.HasValue)
      {
        stream.WriteByte((byte) 77);
        binaryWriter.Write(instance.SessionId.Value);
      }
      if (instance.ShortConnect.HasValue)
      {
        stream.WriteByte((byte) 80);
        ProtocolParser.WriteBool(stream, instance.ShortConnect.Value);
      }
      if (instance.IosAppExtension.HasValue)
      {
        stream.WriteByte((byte) 240);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.IosAppExtension.Value);
      }
      if (instance.connect_type.HasValue)
      {
        stream.WriteByte((byte) 96);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.connect_type.Value);
      }
      if (instance.connect_reason.HasValue)
      {
        stream.WriteByte((byte) 104);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.connect_reason.Value);
      }
      if (instance.Shards != null)
      {
        foreach (int shard in instance.Shards)
        {
          stream.WriteByte((byte) 112);
          ProtocolParser.WriteUInt64(stream, (ulong) shard);
        }
      }
      if (instance.DnsSource != null)
      {
        stream.WriteByte((byte) 122);
        stream1.SetLength(0L);
        ClientPayload.DNSSource.Serialize((Stream) stream1, instance.DnsSource);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.ConnectAttemptCount.HasValue)
      {
        stream.WriteByte((byte) 128);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteUInt32(stream, instance.ConnectAttemptCount.Value);
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(ClientPayload instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        ClientPayload.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, ClientPayload instance)
    {
      byte[] bytes = ClientPayload.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }

    public enum ClientFeature
    {
      NONE,
    }

    public enum IOSAppExtension
    {
      SHARE_EXTENSION,
      SERVICE_EXTENSION,
      INTENTS_EXTENSION,
    }

    public enum ConnectReason
    {
      PUSH,
      USER_ACTIVATED,
      SCHEDULED,
      ERROR_RECONNECT,
      NETWORK_SWITCH,
      PING_RECONNECT,
    }

    public enum ConnectType
    {
      CELLULAR_UNKNOWN = 0,
      WIFI_UNKNOWN = 1,
      CELLULAR_EDGE = 100, // 0x00000064
      CELLULAR_IDEN = 101, // 0x00000065
      CELLULAR_UMTS = 102, // 0x00000066
      CELLULAR_EVDO = 103, // 0x00000067
      CELLULAR_GPRS = 104, // 0x00000068
      CELLULAR_HSDPA = 105, // 0x00000069
      CELLULAR_HSUPA = 106, // 0x0000006A
      CELLULAR_HSPA = 107, // 0x0000006B
      CELLULAR_CDMA = 108, // 0x0000006C
      CELLULAR_1XRTT = 109, // 0x0000006D
      CELLULAR_EHRPD = 110, // 0x0000006E
      CELLULAR_LTE = 111, // 0x0000006F
      CELLULAR_HSPAP = 112, // 0x00000070
    }

    public class UserAgent
    {
      public ClientPayload.UserAgent.Platform? platform { get; set; }

      public ClientPayload.UserAgent.AppVersion AppVersionField { get; set; }

      public string Mcc { get; set; }

      public string Mnc { get; set; }

      public string OsVersion { get; set; }

      public string Manufacturer { get; set; }

      public string Device { get; set; }

      public string OsBuildNumber { get; set; }

      public string PhoneId { get; set; }

      public ClientPayload.UserAgent.ReleaseChannel? release_channel { get; set; }

      public string LocaleLanguageIso6391 { get; set; }

      public string LocaleCountryIso31661Alpha2 { get; set; }

      public static ClientPayload.UserAgent Deserialize(Stream stream)
      {
        ClientPayload.UserAgent instance = new ClientPayload.UserAgent();
        ClientPayload.UserAgent.Deserialize(stream, instance);
        return instance;
      }

      public static ClientPayload.UserAgent DeserializeLengthDelimited(Stream stream)
      {
        ClientPayload.UserAgent instance = new ClientPayload.UserAgent();
        ClientPayload.UserAgent.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static ClientPayload.UserAgent DeserializeLength(Stream stream, int length)
      {
        ClientPayload.UserAgent instance = new ClientPayload.UserAgent();
        ClientPayload.UserAgent.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static ClientPayload.UserAgent Deserialize(byte[] buffer)
      {
        ClientPayload.UserAgent instance = new ClientPayload.UserAgent();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          ClientPayload.UserAgent.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static ClientPayload.UserAgent Deserialize(
        byte[] buffer,
        ClientPayload.UserAgent instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          ClientPayload.UserAgent.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static ClientPayload.UserAgent Deserialize(
        Stream stream,
        ClientPayload.UserAgent instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_18;
            case 8:
              instance.platform = new ClientPayload.UserAgent.Platform?((ClientPayload.UserAgent.Platform) ProtocolParser.ReadUInt64(stream));
              continue;
            case 18:
              if (instance.AppVersionField == null)
              {
                instance.AppVersionField = ClientPayload.UserAgent.AppVersion.DeserializeLengthDelimited(stream);
                continue;
              }
              ClientPayload.UserAgent.AppVersion.DeserializeLengthDelimited(stream, instance.AppVersionField);
              continue;
            case 26:
              instance.Mcc = ProtocolParser.ReadString(stream);
              continue;
            case 34:
              instance.Mnc = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.OsVersion = ProtocolParser.ReadString(stream);
              continue;
            case 50:
              instance.Manufacturer = ProtocolParser.ReadString(stream);
              continue;
            case 58:
              instance.Device = ProtocolParser.ReadString(stream);
              continue;
            case 66:
              instance.OsBuildNumber = ProtocolParser.ReadString(stream);
              continue;
            case 74:
              instance.PhoneId = ProtocolParser.ReadString(stream);
              continue;
            case 80:
              instance.release_channel = new ClientPayload.UserAgent.ReleaseChannel?((ClientPayload.UserAgent.ReleaseChannel) ProtocolParser.ReadUInt64(stream));
              continue;
            case 90:
              instance.LocaleLanguageIso6391 = ProtocolParser.ReadString(stream);
              continue;
            case 98:
              instance.LocaleCountryIso31661Alpha2 = ProtocolParser.ReadString(stream);
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              if (key.Field != 0U)
              {
                ProtocolParser.SkipKey(stream, key);
                continue;
              }
              goto label_16;
          }
        }
label_16:
        throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_18:
        return instance;
      }

      public static ClientPayload.UserAgent DeserializeLengthDelimited(
        Stream stream,
        ClientPayload.UserAgent instance)
      {
        long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 8:
              instance.platform = new ClientPayload.UserAgent.Platform?((ClientPayload.UserAgent.Platform) ProtocolParser.ReadUInt64(stream));
              continue;
            case 18:
              if (instance.AppVersionField == null)
              {
                instance.AppVersionField = ClientPayload.UserAgent.AppVersion.DeserializeLengthDelimited(stream);
                continue;
              }
              ClientPayload.UserAgent.AppVersion.DeserializeLengthDelimited(stream, instance.AppVersionField);
              continue;
            case 26:
              instance.Mcc = ProtocolParser.ReadString(stream);
              continue;
            case 34:
              instance.Mnc = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.OsVersion = ProtocolParser.ReadString(stream);
              continue;
            case 50:
              instance.Manufacturer = ProtocolParser.ReadString(stream);
              continue;
            case 58:
              instance.Device = ProtocolParser.ReadString(stream);
              continue;
            case 66:
              instance.OsBuildNumber = ProtocolParser.ReadString(stream);
              continue;
            case 74:
              instance.PhoneId = ProtocolParser.ReadString(stream);
              continue;
            case 80:
              instance.release_channel = new ClientPayload.UserAgent.ReleaseChannel?((ClientPayload.UserAgent.ReleaseChannel) ProtocolParser.ReadUInt64(stream));
              continue;
            case 90:
              instance.LocaleLanguageIso6391 = ProtocolParser.ReadString(stream);
              continue;
            case 98:
              instance.LocaleCountryIso31661Alpha2 = ProtocolParser.ReadString(stream);
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              if (key.Field == 0U)
                throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
              ProtocolParser.SkipKey(stream, key);
              continue;
          }
        }
        if (stream.Position != num)
          throw new ProtocolBufferException("Read past max limit");
        return instance;
      }

      public static ClientPayload.UserAgent DeserializeLength(
        Stream stream,
        int length,
        ClientPayload.UserAgent instance)
      {
        long num = stream.Position + (long) length;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 8:
              instance.platform = new ClientPayload.UserAgent.Platform?((ClientPayload.UserAgent.Platform) ProtocolParser.ReadUInt64(stream));
              continue;
            case 18:
              if (instance.AppVersionField == null)
              {
                instance.AppVersionField = ClientPayload.UserAgent.AppVersion.DeserializeLengthDelimited(stream);
                continue;
              }
              ClientPayload.UserAgent.AppVersion.DeserializeLengthDelimited(stream, instance.AppVersionField);
              continue;
            case 26:
              instance.Mcc = ProtocolParser.ReadString(stream);
              continue;
            case 34:
              instance.Mnc = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.OsVersion = ProtocolParser.ReadString(stream);
              continue;
            case 50:
              instance.Manufacturer = ProtocolParser.ReadString(stream);
              continue;
            case 58:
              instance.Device = ProtocolParser.ReadString(stream);
              continue;
            case 66:
              instance.OsBuildNumber = ProtocolParser.ReadString(stream);
              continue;
            case 74:
              instance.PhoneId = ProtocolParser.ReadString(stream);
              continue;
            case 80:
              instance.release_channel = new ClientPayload.UserAgent.ReleaseChannel?((ClientPayload.UserAgent.ReleaseChannel) ProtocolParser.ReadUInt64(stream));
              continue;
            case 90:
              instance.LocaleLanguageIso6391 = ProtocolParser.ReadString(stream);
              continue;
            case 98:
              instance.LocaleCountryIso31661Alpha2 = ProtocolParser.ReadString(stream);
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              if (key.Field == 0U)
                throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
              ProtocolParser.SkipKey(stream, key);
              continue;
          }
        }
        if (stream.Position != num)
          throw new ProtocolBufferException("Read past max limit");
        return instance;
      }

      public static void Serialize(Stream stream, ClientPayload.UserAgent instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.platform.HasValue)
        {
          stream.WriteByte((byte) 8);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.platform.Value);
        }
        if (instance.AppVersionField != null)
        {
          stream.WriteByte((byte) 18);
          stream1.SetLength(0L);
          ClientPayload.UserAgent.AppVersion.Serialize((Stream) stream1, instance.AppVersionField);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        if (instance.Mcc != null)
        {
          stream.WriteByte((byte) 26);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Mcc));
        }
        if (instance.Mnc != null)
        {
          stream.WriteByte((byte) 34);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Mnc));
        }
        if (instance.OsVersion != null)
        {
          stream.WriteByte((byte) 42);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.OsVersion));
        }
        if (instance.Manufacturer != null)
        {
          stream.WriteByte((byte) 50);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Manufacturer));
        }
        if (instance.Device != null)
        {
          stream.WriteByte((byte) 58);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Device));
        }
        if (instance.OsBuildNumber != null)
        {
          stream.WriteByte((byte) 66);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.OsBuildNumber));
        }
        if (instance.PhoneId != null)
        {
          stream.WriteByte((byte) 74);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.PhoneId));
        }
        if (instance.release_channel.HasValue)
        {
          stream.WriteByte((byte) 80);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.release_channel.Value);
        }
        if (instance.LocaleLanguageIso6391 != null)
        {
          stream.WriteByte((byte) 90);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.LocaleLanguageIso6391));
        }
        if (instance.LocaleCountryIso31661Alpha2 != null)
        {
          stream.WriteByte((byte) 98);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.LocaleCountryIso31661Alpha2));
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(ClientPayload.UserAgent instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          ClientPayload.UserAgent.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(Stream stream, ClientPayload.UserAgent instance)
      {
        byte[] bytes = ClientPayload.UserAgent.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public enum Platform
      {
        ANDROID,
        IOS,
        WINDOWS_PHONE,
        BLACKBERRY,
        BLACKBERRYX,
        S40,
        S60,
        PYTHON_CLIENT,
        TIZEN,
        ENTERPRISE,
        SMB_ANDROID,
        KAIOS,
        SMB_IOS,
        WINDOWS,
      }

      public enum ReleaseChannel
      {
        RELEASE,
        BETA,
        ALPHA,
        DEBUG,
      }

      public class AppVersion
      {
        public uint? Primary { get; set; }

        public uint? Secondary { get; set; }

        public uint? Tertiary { get; set; }

        public uint? Quaternary { get; set; }

        public static ClientPayload.UserAgent.AppVersion Deserialize(Stream stream)
        {
          ClientPayload.UserAgent.AppVersion instance = new ClientPayload.UserAgent.AppVersion();
          ClientPayload.UserAgent.AppVersion.Deserialize(stream, instance);
          return instance;
        }

        public static ClientPayload.UserAgent.AppVersion DeserializeLengthDelimited(Stream stream)
        {
          ClientPayload.UserAgent.AppVersion instance = new ClientPayload.UserAgent.AppVersion();
          ClientPayload.UserAgent.AppVersion.DeserializeLengthDelimited(stream, instance);
          return instance;
        }

        public static ClientPayload.UserAgent.AppVersion DeserializeLength(
          Stream stream,
          int length)
        {
          ClientPayload.UserAgent.AppVersion instance = new ClientPayload.UserAgent.AppVersion();
          ClientPayload.UserAgent.AppVersion.DeserializeLength(stream, length, instance);
          return instance;
        }

        public static ClientPayload.UserAgent.AppVersion Deserialize(byte[] buffer)
        {
          ClientPayload.UserAgent.AppVersion instance = new ClientPayload.UserAgent.AppVersion();
          using (MemoryStream memoryStream = new MemoryStream(buffer))
            ClientPayload.UserAgent.AppVersion.Deserialize((Stream) memoryStream, instance);
          return instance;
        }

        public static ClientPayload.UserAgent.AppVersion Deserialize(
          byte[] buffer,
          ClientPayload.UserAgent.AppVersion instance)
        {
          using (MemoryStream memoryStream = new MemoryStream(buffer))
            ClientPayload.UserAgent.AppVersion.Deserialize((Stream) memoryStream, instance);
          return instance;
        }

        public static ClientPayload.UserAgent.AppVersion Deserialize(
          Stream stream,
          ClientPayload.UserAgent.AppVersion instance)
        {
          while (true)
          {
            int firstByte = stream.ReadByte();
            switch (firstByte)
            {
              case -1:
                goto label_8;
              case 8:
                instance.Primary = new uint?(ProtocolParser.ReadUInt32(stream));
                continue;
              case 16:
                instance.Secondary = new uint?(ProtocolParser.ReadUInt32(stream));
                continue;
              case 24:
                instance.Tertiary = new uint?(ProtocolParser.ReadUInt32(stream));
                continue;
              case 32:
                instance.Quaternary = new uint?(ProtocolParser.ReadUInt32(stream));
                continue;
              default:
                SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
                if (key.Field != 0U)
                {
                  ProtocolParser.SkipKey(stream, key);
                  continue;
                }
                goto label_6;
            }
          }
label_6:
          throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_8:
          return instance;
        }

        public static ClientPayload.UserAgent.AppVersion DeserializeLengthDelimited(
          Stream stream,
          ClientPayload.UserAgent.AppVersion instance)
        {
          long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
          while (stream.Position < num)
          {
            int firstByte = stream.ReadByte();
            switch (firstByte)
            {
              case -1:
                throw new EndOfStreamException();
              case 8:
                instance.Primary = new uint?(ProtocolParser.ReadUInt32(stream));
                continue;
              case 16:
                instance.Secondary = new uint?(ProtocolParser.ReadUInt32(stream));
                continue;
              case 24:
                instance.Tertiary = new uint?(ProtocolParser.ReadUInt32(stream));
                continue;
              case 32:
                instance.Quaternary = new uint?(ProtocolParser.ReadUInt32(stream));
                continue;
              default:
                SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
                if (key.Field == 0U)
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                ProtocolParser.SkipKey(stream, key);
                continue;
            }
          }
          if (stream.Position != num)
            throw new ProtocolBufferException("Read past max limit");
          return instance;
        }

        public static ClientPayload.UserAgent.AppVersion DeserializeLength(
          Stream stream,
          int length,
          ClientPayload.UserAgent.AppVersion instance)
        {
          long num = stream.Position + (long) length;
          while (stream.Position < num)
          {
            int firstByte = stream.ReadByte();
            switch (firstByte)
            {
              case -1:
                throw new EndOfStreamException();
              case 8:
                instance.Primary = new uint?(ProtocolParser.ReadUInt32(stream));
                continue;
              case 16:
                instance.Secondary = new uint?(ProtocolParser.ReadUInt32(stream));
                continue;
              case 24:
                instance.Tertiary = new uint?(ProtocolParser.ReadUInt32(stream));
                continue;
              case 32:
                instance.Quaternary = new uint?(ProtocolParser.ReadUInt32(stream));
                continue;
              default:
                SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
                if (key.Field == 0U)
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                ProtocolParser.SkipKey(stream, key);
                continue;
            }
          }
          if (stream.Position != num)
            throw new ProtocolBufferException("Read past max limit");
          return instance;
        }

        public static void Serialize(Stream stream, ClientPayload.UserAgent.AppVersion instance)
        {
          MemoryStream stream1 = ProtocolParser.Stack.Pop();
          if (instance.Primary.HasValue)
          {
            stream.WriteByte((byte) 8);
            ProtocolParser.WriteUInt32(stream, instance.Primary.Value);
          }
          if (instance.Secondary.HasValue)
          {
            stream.WriteByte((byte) 16);
            ProtocolParser.WriteUInt32(stream, instance.Secondary.Value);
          }
          if (instance.Tertiary.HasValue)
          {
            stream.WriteByte((byte) 24);
            ProtocolParser.WriteUInt32(stream, instance.Tertiary.Value);
          }
          if (instance.Quaternary.HasValue)
          {
            stream.WriteByte((byte) 32);
            ProtocolParser.WriteUInt32(stream, instance.Quaternary.Value);
          }
          ProtocolParser.Stack.Push(stream1);
        }

        public static byte[] SerializeToBytes(ClientPayload.UserAgent.AppVersion instance)
        {
          using (MemoryStream memoryStream = new MemoryStream())
          {
            ClientPayload.UserAgent.AppVersion.Serialize((Stream) memoryStream, instance);
            return memoryStream.ToArray();
          }
        }

        public static void SerializeLengthDelimited(
          Stream stream,
          ClientPayload.UserAgent.AppVersion instance)
        {
          byte[] bytes = ClientPayload.UserAgent.AppVersion.SerializeToBytes(instance);
          ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
          stream.Write(bytes, 0, bytes.Length);
        }
      }
    }

    public class WebInfo
    {
      public string RefToken { get; set; }

      public string Version { get; set; }

      public ClientPayload.WebInfo.WebdPayload WebdPayloadField { get; set; }

      public static ClientPayload.WebInfo Deserialize(Stream stream)
      {
        ClientPayload.WebInfo instance = new ClientPayload.WebInfo();
        ClientPayload.WebInfo.Deserialize(stream, instance);
        return instance;
      }

      public static ClientPayload.WebInfo DeserializeLengthDelimited(Stream stream)
      {
        ClientPayload.WebInfo instance = new ClientPayload.WebInfo();
        ClientPayload.WebInfo.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static ClientPayload.WebInfo DeserializeLength(Stream stream, int length)
      {
        ClientPayload.WebInfo instance = new ClientPayload.WebInfo();
        ClientPayload.WebInfo.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static ClientPayload.WebInfo Deserialize(byte[] buffer)
      {
        ClientPayload.WebInfo instance = new ClientPayload.WebInfo();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          ClientPayload.WebInfo.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static ClientPayload.WebInfo Deserialize(byte[] buffer, ClientPayload.WebInfo instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          ClientPayload.WebInfo.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static ClientPayload.WebInfo Deserialize(Stream stream, ClientPayload.WebInfo instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_9;
            case 10:
              instance.RefToken = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Version = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              if (instance.WebdPayloadField == null)
              {
                instance.WebdPayloadField = ClientPayload.WebInfo.WebdPayload.DeserializeLengthDelimited(stream);
                continue;
              }
              ClientPayload.WebInfo.WebdPayload.DeserializeLengthDelimited(stream, instance.WebdPayloadField);
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              if (key.Field != 0U)
              {
                ProtocolParser.SkipKey(stream, key);
                continue;
              }
              goto label_7;
          }
        }
label_7:
        throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_9:
        return instance;
      }

      public static ClientPayload.WebInfo DeserializeLengthDelimited(
        Stream stream,
        ClientPayload.WebInfo instance)
      {
        long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 10:
              instance.RefToken = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Version = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              if (instance.WebdPayloadField == null)
              {
                instance.WebdPayloadField = ClientPayload.WebInfo.WebdPayload.DeserializeLengthDelimited(stream);
                continue;
              }
              ClientPayload.WebInfo.WebdPayload.DeserializeLengthDelimited(stream, instance.WebdPayloadField);
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              if (key.Field == 0U)
                throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
              ProtocolParser.SkipKey(stream, key);
              continue;
          }
        }
        if (stream.Position != num)
          throw new ProtocolBufferException("Read past max limit");
        return instance;
      }

      public static ClientPayload.WebInfo DeserializeLength(
        Stream stream,
        int length,
        ClientPayload.WebInfo instance)
      {
        long num = stream.Position + (long) length;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 10:
              instance.RefToken = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Version = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              if (instance.WebdPayloadField == null)
              {
                instance.WebdPayloadField = ClientPayload.WebInfo.WebdPayload.DeserializeLengthDelimited(stream);
                continue;
              }
              ClientPayload.WebInfo.WebdPayload.DeserializeLengthDelimited(stream, instance.WebdPayloadField);
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              if (key.Field == 0U)
                throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
              ProtocolParser.SkipKey(stream, key);
              continue;
          }
        }
        if (stream.Position != num)
          throw new ProtocolBufferException("Read past max limit");
        return instance;
      }

      public static void Serialize(Stream stream, ClientPayload.WebInfo instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.RefToken != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.RefToken));
        }
        if (instance.Version != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Version));
        }
        if (instance.WebdPayloadField != null)
        {
          stream.WriteByte((byte) 26);
          stream1.SetLength(0L);
          ClientPayload.WebInfo.WebdPayload.Serialize((Stream) stream1, instance.WebdPayloadField);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(ClientPayload.WebInfo instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          ClientPayload.WebInfo.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(Stream stream, ClientPayload.WebInfo instance)
      {
        byte[] bytes = ClientPayload.WebInfo.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public class WebdPayload
      {
        public bool? UsesParticipantInKey { get; set; }

        [Obsolete]
        public bool? SupportsStarredMessages { get; set; }

        [Obsolete]
        public bool? SupportsDocumentMessages { get; set; }

        [Obsolete]
        public bool? SupportsUrlMessages { get; set; }

        [Obsolete]
        public bool? SupportsMediaRetry { get; set; }

        [Obsolete]
        public bool? SupportsE2eImage { get; set; }

        [Obsolete]
        public bool? SupportsE2eVideo { get; set; }

        [Obsolete]
        public bool? SupportsE2eAudio { get; set; }

        [Obsolete]
        public bool? SupportsE2eDocument { get; set; }

        [Obsolete]
        public string DocumentTypes { get; set; }

        public byte[] Features { get; set; }

        public static ClientPayload.WebInfo.WebdPayload Deserialize(Stream stream)
        {
          ClientPayload.WebInfo.WebdPayload instance = new ClientPayload.WebInfo.WebdPayload();
          ClientPayload.WebInfo.WebdPayload.Deserialize(stream, instance);
          return instance;
        }

        public static ClientPayload.WebInfo.WebdPayload DeserializeLengthDelimited(Stream stream)
        {
          ClientPayload.WebInfo.WebdPayload instance = new ClientPayload.WebInfo.WebdPayload();
          ClientPayload.WebInfo.WebdPayload.DeserializeLengthDelimited(stream, instance);
          return instance;
        }

        public static ClientPayload.WebInfo.WebdPayload DeserializeLength(Stream stream, int length)
        {
          ClientPayload.WebInfo.WebdPayload instance = new ClientPayload.WebInfo.WebdPayload();
          ClientPayload.WebInfo.WebdPayload.DeserializeLength(stream, length, instance);
          return instance;
        }

        public static ClientPayload.WebInfo.WebdPayload Deserialize(byte[] buffer)
        {
          ClientPayload.WebInfo.WebdPayload instance = new ClientPayload.WebInfo.WebdPayload();
          using (MemoryStream memoryStream = new MemoryStream(buffer))
            ClientPayload.WebInfo.WebdPayload.Deserialize((Stream) memoryStream, instance);
          return instance;
        }

        public static ClientPayload.WebInfo.WebdPayload Deserialize(
          byte[] buffer,
          ClientPayload.WebInfo.WebdPayload instance)
        {
          using (MemoryStream memoryStream = new MemoryStream(buffer))
            ClientPayload.WebInfo.WebdPayload.Deserialize((Stream) memoryStream, instance);
          return instance;
        }

        public static ClientPayload.WebInfo.WebdPayload Deserialize(
          Stream stream,
          ClientPayload.WebInfo.WebdPayload instance)
        {
          while (true)
          {
            int firstByte = stream.ReadByte();
            switch (firstByte)
            {
              case -1:
                goto label_15;
              case 8:
                instance.UsesParticipantInKey = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 16:
                instance.SupportsStarredMessages = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 24:
                instance.SupportsDocumentMessages = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 32:
                instance.SupportsUrlMessages = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 40:
                instance.SupportsMediaRetry = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 48:
                instance.SupportsE2eImage = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 56:
                instance.SupportsE2eVideo = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 64:
                instance.SupportsE2eAudio = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 72:
                instance.SupportsE2eDocument = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 82:
                instance.DocumentTypes = ProtocolParser.ReadString(stream);
                continue;
              case 90:
                instance.Features = ProtocolParser.ReadBytes(stream);
                continue;
              default:
                SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
                if (key.Field != 0U)
                {
                  ProtocolParser.SkipKey(stream, key);
                  continue;
                }
                goto label_13;
            }
          }
label_13:
          throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_15:
          return instance;
        }

        public static ClientPayload.WebInfo.WebdPayload DeserializeLengthDelimited(
          Stream stream,
          ClientPayload.WebInfo.WebdPayload instance)
        {
          long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
          while (stream.Position < num)
          {
            int firstByte = stream.ReadByte();
            switch (firstByte)
            {
              case -1:
                throw new EndOfStreamException();
              case 8:
                instance.UsesParticipantInKey = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 16:
                instance.SupportsStarredMessages = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 24:
                instance.SupportsDocumentMessages = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 32:
                instance.SupportsUrlMessages = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 40:
                instance.SupportsMediaRetry = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 48:
                instance.SupportsE2eImage = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 56:
                instance.SupportsE2eVideo = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 64:
                instance.SupportsE2eAudio = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 72:
                instance.SupportsE2eDocument = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 82:
                instance.DocumentTypes = ProtocolParser.ReadString(stream);
                continue;
              case 90:
                instance.Features = ProtocolParser.ReadBytes(stream);
                continue;
              default:
                SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
                if (key.Field == 0U)
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                ProtocolParser.SkipKey(stream, key);
                continue;
            }
          }
          if (stream.Position != num)
            throw new ProtocolBufferException("Read past max limit");
          return instance;
        }

        public static ClientPayload.WebInfo.WebdPayload DeserializeLength(
          Stream stream,
          int length,
          ClientPayload.WebInfo.WebdPayload instance)
        {
          long num = stream.Position + (long) length;
          while (stream.Position < num)
          {
            int firstByte = stream.ReadByte();
            switch (firstByte)
            {
              case -1:
                throw new EndOfStreamException();
              case 8:
                instance.UsesParticipantInKey = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 16:
                instance.SupportsStarredMessages = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 24:
                instance.SupportsDocumentMessages = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 32:
                instance.SupportsUrlMessages = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 40:
                instance.SupportsMediaRetry = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 48:
                instance.SupportsE2eImage = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 56:
                instance.SupportsE2eVideo = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 64:
                instance.SupportsE2eAudio = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 72:
                instance.SupportsE2eDocument = new bool?(ProtocolParser.ReadBool(stream));
                continue;
              case 82:
                instance.DocumentTypes = ProtocolParser.ReadString(stream);
                continue;
              case 90:
                instance.Features = ProtocolParser.ReadBytes(stream);
                continue;
              default:
                SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
                if (key.Field == 0U)
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                ProtocolParser.SkipKey(stream, key);
                continue;
            }
          }
          if (stream.Position != num)
            throw new ProtocolBufferException("Read past max limit");
          return instance;
        }

        public static void Serialize(Stream stream, ClientPayload.WebInfo.WebdPayload instance)
        {
          MemoryStream stream1 = ProtocolParser.Stack.Pop();
          if (instance.UsesParticipantInKey.HasValue)
          {
            stream.WriteByte((byte) 8);
            ProtocolParser.WriteBool(stream, instance.UsesParticipantInKey.Value);
          }
          if (instance.SupportsStarredMessages.HasValue)
          {
            stream.WriteByte((byte) 16);
            ProtocolParser.WriteBool(stream, instance.SupportsStarredMessages.Value);
          }
          if (instance.SupportsDocumentMessages.HasValue)
          {
            stream.WriteByte((byte) 24);
            ProtocolParser.WriteBool(stream, instance.SupportsDocumentMessages.Value);
          }
          if (instance.SupportsUrlMessages.HasValue)
          {
            stream.WriteByte((byte) 32);
            ProtocolParser.WriteBool(stream, instance.SupportsUrlMessages.Value);
          }
          if (instance.SupportsMediaRetry.HasValue)
          {
            stream.WriteByte((byte) 40);
            ProtocolParser.WriteBool(stream, instance.SupportsMediaRetry.Value);
          }
          if (instance.SupportsE2eImage.HasValue)
          {
            stream.WriteByte((byte) 48);
            ProtocolParser.WriteBool(stream, instance.SupportsE2eImage.Value);
          }
          if (instance.SupportsE2eVideo.HasValue)
          {
            stream.WriteByte((byte) 56);
            ProtocolParser.WriteBool(stream, instance.SupportsE2eVideo.Value);
          }
          if (instance.SupportsE2eAudio.HasValue)
          {
            stream.WriteByte((byte) 64);
            ProtocolParser.WriteBool(stream, instance.SupportsE2eAudio.Value);
          }
          if (instance.SupportsE2eDocument.HasValue)
          {
            stream.WriteByte((byte) 72);
            ProtocolParser.WriteBool(stream, instance.SupportsE2eDocument.Value);
          }
          if (instance.DocumentTypes != null)
          {
            stream.WriteByte((byte) 82);
            ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.DocumentTypes));
          }
          if (instance.Features != null)
          {
            stream.WriteByte((byte) 90);
            ProtocolParser.WriteBytes(stream, instance.Features);
          }
          ProtocolParser.Stack.Push(stream1);
        }

        public static byte[] SerializeToBytes(ClientPayload.WebInfo.WebdPayload instance)
        {
          using (MemoryStream memoryStream = new MemoryStream())
          {
            ClientPayload.WebInfo.WebdPayload.Serialize((Stream) memoryStream, instance);
            return memoryStream.ToArray();
          }
        }

        public static void SerializeLengthDelimited(
          Stream stream,
          ClientPayload.WebInfo.WebdPayload instance)
        {
          byte[] bytes = ClientPayload.WebInfo.WebdPayload.SerializeToBytes(instance);
          ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
          stream.Write(bytes, 0, bytes.Length);
        }
      }
    }

    public class DNSSource
    {
      public ClientPayload.DNSSource.DNSResolutionMethod? DnsMethod { get; set; }

      public bool? AppCached { get; set; }

      public static ClientPayload.DNSSource Deserialize(Stream stream)
      {
        ClientPayload.DNSSource instance = new ClientPayload.DNSSource();
        ClientPayload.DNSSource.Deserialize(stream, instance);
        return instance;
      }

      public static ClientPayload.DNSSource DeserializeLengthDelimited(Stream stream)
      {
        ClientPayload.DNSSource instance = new ClientPayload.DNSSource();
        ClientPayload.DNSSource.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static ClientPayload.DNSSource DeserializeLength(Stream stream, int length)
      {
        ClientPayload.DNSSource instance = new ClientPayload.DNSSource();
        ClientPayload.DNSSource.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static ClientPayload.DNSSource Deserialize(byte[] buffer)
      {
        ClientPayload.DNSSource instance = new ClientPayload.DNSSource();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          ClientPayload.DNSSource.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static ClientPayload.DNSSource Deserialize(
        byte[] buffer,
        ClientPayload.DNSSource instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          ClientPayload.DNSSource.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static ClientPayload.DNSSource Deserialize(
        Stream stream,
        ClientPayload.DNSSource instance)
      {
        while (true)
        {
          SilentOrbit.ProtocolBuffers.Key key;
          do
          {
            int firstByte = stream.ReadByte();
            switch (firstByte)
            {
              case -1:
                goto label_7;
              case 120:
                instance.DnsMethod = new ClientPayload.DNSSource.DNSResolutionMethod?((ClientPayload.DNSSource.DNSResolutionMethod) ProtocolParser.ReadUInt64(stream));
                continue;
              default:
                key = ProtocolParser.ReadKey((byte) firstByte, stream);
                switch (key.Field)
                {
                  case 0:
                    throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                  case 16:
                    continue;
                  default:
                    goto label_6;
                }
            }
          }
          while (key.WireType != Wire.Varint);
          instance.AppCached = new bool?(ProtocolParser.ReadBool(stream));
          continue;
label_6:
          ProtocolParser.SkipKey(stream, key);
        }
label_7:
        return instance;
      }

      public static ClientPayload.DNSSource DeserializeLengthDelimited(
        Stream stream,
        ClientPayload.DNSSource instance)
      {
        long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 120:
              instance.DnsMethod = new ClientPayload.DNSSource.DNSResolutionMethod?((ClientPayload.DNSSource.DNSResolutionMethod) ProtocolParser.ReadUInt64(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.Varint)
                  {
                    instance.AppCached = new bool?(ProtocolParser.ReadBool(stream));
                    continue;
                  }
                  continue;
                default:
                  ProtocolParser.SkipKey(stream, key);
                  continue;
              }
          }
        }
        if (stream.Position != num)
          throw new ProtocolBufferException("Read past max limit");
        return instance;
      }

      public static ClientPayload.DNSSource DeserializeLength(
        Stream stream,
        int length,
        ClientPayload.DNSSource instance)
      {
        long num = stream.Position + (long) length;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 120:
              instance.DnsMethod = new ClientPayload.DNSSource.DNSResolutionMethod?((ClientPayload.DNSSource.DNSResolutionMethod) ProtocolParser.ReadUInt64(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.Varint)
                  {
                    instance.AppCached = new bool?(ProtocolParser.ReadBool(stream));
                    continue;
                  }
                  continue;
                default:
                  ProtocolParser.SkipKey(stream, key);
                  continue;
              }
          }
        }
        if (stream.Position != num)
          throw new ProtocolBufferException("Read past max limit");
        return instance;
      }

      public static void Serialize(Stream stream, ClientPayload.DNSSource instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.DnsMethod.HasValue)
        {
          stream.WriteByte((byte) 120);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.DnsMethod.Value);
        }
        if (instance.AppCached.HasValue)
        {
          stream.WriteByte((byte) 128);
          stream.WriteByte((byte) 1);
          ProtocolParser.WriteBool(stream, instance.AppCached.Value);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(ClientPayload.DNSSource instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          ClientPayload.DNSSource.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(Stream stream, ClientPayload.DNSSource instance)
      {
        byte[] bytes = ClientPayload.DNSSource.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public enum DNSResolutionMethod
      {
        SYSTEM,
        GOOGLE,
        HARDCODED,
        OVERRIDE,
        FALLBACK,
      }
    }
  }
}
