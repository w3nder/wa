// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.WebNotificationsInfo
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System.Collections.Generic;
using System.IO;


namespace WhatsApp.ProtoBuf
{
  public class WebNotificationsInfo
  {
    public ulong? Timestamp { get; set; }

    public uint? UnreadChats { get; set; }

    public uint? NotifyMessageCount { get; set; }

    public List<WebMessageInfo> NotifyMessages { get; set; }

    public static WebNotificationsInfo Deserialize(Stream stream)
    {
      WebNotificationsInfo instance = new WebNotificationsInfo();
      WebNotificationsInfo.Deserialize(stream, instance);
      return instance;
    }

    public static WebNotificationsInfo DeserializeLengthDelimited(Stream stream)
    {
      WebNotificationsInfo instance = new WebNotificationsInfo();
      WebNotificationsInfo.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static WebNotificationsInfo DeserializeLength(Stream stream, int length)
    {
      WebNotificationsInfo instance = new WebNotificationsInfo();
      WebNotificationsInfo.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static WebNotificationsInfo Deserialize(byte[] buffer)
    {
      WebNotificationsInfo instance = new WebNotificationsInfo();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        WebNotificationsInfo.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static WebNotificationsInfo Deserialize(byte[] buffer, WebNotificationsInfo instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        WebNotificationsInfo.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static WebNotificationsInfo Deserialize(Stream stream, WebNotificationsInfo instance)
    {
      if (instance.NotifyMessages == null)
        instance.NotifyMessages = new List<WebMessageInfo>();
      while (true)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            goto label_10;
          case 16:
            instance.Timestamp = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 24:
            instance.UnreadChats = new uint?(ProtocolParser.ReadUInt32(stream));
            continue;
          case 32:
            instance.NotifyMessageCount = new uint?(ProtocolParser.ReadUInt32(stream));
            continue;
          case 42:
            instance.NotifyMessages.Add(WebMessageInfo.DeserializeLengthDelimited(stream));
            continue;
          default:
            SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
            if (key.Field != 0U)
            {
              ProtocolParser.SkipKey(stream, key);
              continue;
            }
            goto label_8;
        }
      }
label_8:
      throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_10:
      return instance;
    }

    public static WebNotificationsInfo DeserializeLengthDelimited(
      Stream stream,
      WebNotificationsInfo instance)
    {
      if (instance.NotifyMessages == null)
        instance.NotifyMessages = new List<WebMessageInfo>();
      long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
      while (stream.Position < num)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            throw new EndOfStreamException();
          case 16:
            instance.Timestamp = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 24:
            instance.UnreadChats = new uint?(ProtocolParser.ReadUInt32(stream));
            continue;
          case 32:
            instance.NotifyMessageCount = new uint?(ProtocolParser.ReadUInt32(stream));
            continue;
          case 42:
            instance.NotifyMessages.Add(WebMessageInfo.DeserializeLengthDelimited(stream));
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

    public static WebNotificationsInfo DeserializeLength(
      Stream stream,
      int length,
      WebNotificationsInfo instance)
    {
      if (instance.NotifyMessages == null)
        instance.NotifyMessages = new List<WebMessageInfo>();
      long num = stream.Position + (long) length;
      while (stream.Position < num)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            throw new EndOfStreamException();
          case 16:
            instance.Timestamp = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 24:
            instance.UnreadChats = new uint?(ProtocolParser.ReadUInt32(stream));
            continue;
          case 32:
            instance.NotifyMessageCount = new uint?(ProtocolParser.ReadUInt32(stream));
            continue;
          case 42:
            instance.NotifyMessages.Add(WebMessageInfo.DeserializeLengthDelimited(stream));
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

    public static void Serialize(Stream stream, WebNotificationsInfo instance)
    {
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.Timestamp.HasValue)
      {
        stream.WriteByte((byte) 16);
        ProtocolParser.WriteUInt64(stream, instance.Timestamp.Value);
      }
      if (instance.UnreadChats.HasValue)
      {
        stream.WriteByte((byte) 24);
        ProtocolParser.WriteUInt32(stream, instance.UnreadChats.Value);
      }
      if (instance.NotifyMessageCount.HasValue)
      {
        stream.WriteByte((byte) 32);
        ProtocolParser.WriteUInt32(stream, instance.NotifyMessageCount.Value);
      }
      if (instance.NotifyMessages != null)
      {
        foreach (WebMessageInfo notifyMessage in instance.NotifyMessages)
        {
          stream.WriteByte((byte) 42);
          stream1.SetLength(0L);
          WebMessageInfo.Serialize(stream1, notifyMessage, false);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(WebNotificationsInfo instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        WebNotificationsInfo.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, WebNotificationsInfo instance)
    {
      byte[] bytes = WebNotificationsInfo.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }
  }
}
