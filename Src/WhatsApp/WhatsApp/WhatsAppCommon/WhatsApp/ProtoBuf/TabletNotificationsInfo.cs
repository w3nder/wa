// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.TabletNotificationsInfo
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System.Collections.Generic;
using System.IO;


namespace WhatsApp.ProtoBuf
{
  public class TabletNotificationsInfo
  {
    public ulong? Timestamp { get; set; }

    public uint? UnreadChats { get; set; }

    public uint? NotifyMessageCount { get; set; }

    public List<NotificationMessageInfo> NotifyMessage { get; set; }

    public static TabletNotificationsInfo Deserialize(Stream stream)
    {
      TabletNotificationsInfo instance = new TabletNotificationsInfo();
      TabletNotificationsInfo.Deserialize(stream, instance);
      return instance;
    }

    public static TabletNotificationsInfo DeserializeLengthDelimited(Stream stream)
    {
      TabletNotificationsInfo instance = new TabletNotificationsInfo();
      TabletNotificationsInfo.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static TabletNotificationsInfo DeserializeLength(Stream stream, int length)
    {
      TabletNotificationsInfo instance = new TabletNotificationsInfo();
      TabletNotificationsInfo.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static TabletNotificationsInfo Deserialize(byte[] buffer)
    {
      TabletNotificationsInfo instance = new TabletNotificationsInfo();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        TabletNotificationsInfo.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static TabletNotificationsInfo Deserialize(
      byte[] buffer,
      TabletNotificationsInfo instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        TabletNotificationsInfo.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static TabletNotificationsInfo Deserialize(
      Stream stream,
      TabletNotificationsInfo instance)
    {
      if (instance.NotifyMessage == null)
        instance.NotifyMessage = new List<NotificationMessageInfo>();
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
            instance.NotifyMessage.Add(NotificationMessageInfo.DeserializeLengthDelimited(stream));
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

    public static TabletNotificationsInfo DeserializeLengthDelimited(
      Stream stream,
      TabletNotificationsInfo instance)
    {
      if (instance.NotifyMessage == null)
        instance.NotifyMessage = new List<NotificationMessageInfo>();
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
            instance.NotifyMessage.Add(NotificationMessageInfo.DeserializeLengthDelimited(stream));
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

    public static TabletNotificationsInfo DeserializeLength(
      Stream stream,
      int length,
      TabletNotificationsInfo instance)
    {
      if (instance.NotifyMessage == null)
        instance.NotifyMessage = new List<NotificationMessageInfo>();
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
            instance.NotifyMessage.Add(NotificationMessageInfo.DeserializeLengthDelimited(stream));
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

    public static void Serialize(Stream stream, TabletNotificationsInfo instance)
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
      if (instance.NotifyMessage != null)
      {
        foreach (NotificationMessageInfo instance1 in instance.NotifyMessage)
        {
          stream.WriteByte((byte) 42);
          stream1.SetLength(0L);
          NotificationMessageInfo.Serialize((Stream) stream1, instance1);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(TabletNotificationsInfo instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        TabletNotificationsInfo.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, TabletNotificationsInfo instance)
    {
      byte[] bytes = TabletNotificationsInfo.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }
  }
}
