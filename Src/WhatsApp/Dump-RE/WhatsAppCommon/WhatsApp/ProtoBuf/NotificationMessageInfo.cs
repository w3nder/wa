// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.NotificationMessageInfo
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System.IO;
using System.Text;

#nullable disable
namespace WhatsApp.ProtoBuf
{
  public class NotificationMessageInfo
  {
    public MessageKey Key { get; set; }

    public Message Message { get; set; }

    public ulong? MessageTimestamp { get; set; }

    public string Participant { get; set; }

    public static NotificationMessageInfo Deserialize(Stream stream)
    {
      NotificationMessageInfo instance = new NotificationMessageInfo();
      NotificationMessageInfo.Deserialize(stream, instance);
      return instance;
    }

    public static NotificationMessageInfo DeserializeLengthDelimited(Stream stream)
    {
      NotificationMessageInfo instance = new NotificationMessageInfo();
      NotificationMessageInfo.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static NotificationMessageInfo DeserializeLength(Stream stream, int length)
    {
      NotificationMessageInfo instance = new NotificationMessageInfo();
      NotificationMessageInfo.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static NotificationMessageInfo Deserialize(byte[] buffer)
    {
      NotificationMessageInfo instance = new NotificationMessageInfo();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        NotificationMessageInfo.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static NotificationMessageInfo Deserialize(
      byte[] buffer,
      NotificationMessageInfo instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        NotificationMessageInfo.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static NotificationMessageInfo Deserialize(
      Stream stream,
      NotificationMessageInfo instance)
    {
      while (true)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            goto label_12;
          case 10:
            if (instance.Key == null)
            {
              instance.Key = MessageKey.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageKey.DeserializeLengthDelimited(stream, instance.Key);
            continue;
          case 18:
            if (instance.Message == null)
            {
              instance.Message = Message.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.DeserializeLengthDelimited(stream, instance.Message);
            continue;
          case 24:
            instance.MessageTimestamp = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 34:
            instance.Participant = ProtocolParser.ReadString(stream);
            continue;
          default:
            SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
            if (key.Field != 0U)
            {
              ProtocolParser.SkipKey(stream, key);
              continue;
            }
            goto label_10;
        }
      }
label_10:
      throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_12:
      return instance;
    }

    public static NotificationMessageInfo DeserializeLengthDelimited(
      Stream stream,
      NotificationMessageInfo instance)
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
            if (instance.Key == null)
            {
              instance.Key = MessageKey.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageKey.DeserializeLengthDelimited(stream, instance.Key);
            continue;
          case 18:
            if (instance.Message == null)
            {
              instance.Message = Message.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.DeserializeLengthDelimited(stream, instance.Message);
            continue;
          case 24:
            instance.MessageTimestamp = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 34:
            instance.Participant = ProtocolParser.ReadString(stream);
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

    public static NotificationMessageInfo DeserializeLength(
      Stream stream,
      int length,
      NotificationMessageInfo instance)
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
            if (instance.Key == null)
            {
              instance.Key = MessageKey.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageKey.DeserializeLengthDelimited(stream, instance.Key);
            continue;
          case 18:
            if (instance.Message == null)
            {
              instance.Message = Message.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.DeserializeLengthDelimited(stream, instance.Message);
            continue;
          case 24:
            instance.MessageTimestamp = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 34:
            instance.Participant = ProtocolParser.ReadString(stream);
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

    public static void Serialize(Stream stream, NotificationMessageInfo instance)
    {
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.Key != null)
      {
        stream.WriteByte((byte) 10);
        stream1.SetLength(0L);
        MessageKey.Serialize((Stream) stream1, instance.Key);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.Message != null)
      {
        stream.WriteByte((byte) 18);
        stream1.SetLength(0L);
        Message.Serialize((Stream) stream1, instance.Message);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.MessageTimestamp.HasValue)
      {
        stream.WriteByte((byte) 24);
        ProtocolParser.WriteUInt64(stream, instance.MessageTimestamp.Value);
      }
      if (instance.Participant != null)
      {
        stream.WriteByte((byte) 34);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Participant));
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(NotificationMessageInfo instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        NotificationMessageInfo.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, NotificationMessageInfo instance)
    {
      byte[] bytes = NotificationMessageInfo.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }
  }
}
