// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.MessageKey
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System.IO;
using System.Text;

#nullable disable
namespace WhatsApp.ProtoBuf
{
  public class MessageKey
  {
    public string RemoteJid { get; set; }

    public bool? FromMe { get; set; }

    public string Id { get; set; }

    public string Participant { get; set; }

    public static MessageKey Deserialize(Stream stream)
    {
      MessageKey instance = new MessageKey();
      MessageKey.Deserialize(stream, instance);
      return instance;
    }

    public static MessageKey DeserializeLengthDelimited(Stream stream)
    {
      MessageKey instance = new MessageKey();
      MessageKey.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static MessageKey DeserializeLength(Stream stream, int length)
    {
      MessageKey instance = new MessageKey();
      MessageKey.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static MessageKey Deserialize(byte[] buffer)
    {
      MessageKey instance = new MessageKey();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        MessageKey.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static MessageKey Deserialize(byte[] buffer, MessageKey instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        MessageKey.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static MessageKey Deserialize(Stream stream, MessageKey instance)
    {
      while (true)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            goto label_8;
          case 10:
            instance.RemoteJid = ProtocolParser.ReadString(stream);
            continue;
          case 16:
            instance.FromMe = new bool?(ProtocolParser.ReadBool(stream));
            continue;
          case 26:
            instance.Id = ProtocolParser.ReadString(stream);
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
            goto label_6;
        }
      }
label_6:
      throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_8:
      return instance;
    }

    public static MessageKey DeserializeLengthDelimited(Stream stream, MessageKey instance)
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
            instance.RemoteJid = ProtocolParser.ReadString(stream);
            continue;
          case 16:
            instance.FromMe = new bool?(ProtocolParser.ReadBool(stream));
            continue;
          case 26:
            instance.Id = ProtocolParser.ReadString(stream);
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

    public static MessageKey DeserializeLength(Stream stream, int length, MessageKey instance)
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
            instance.RemoteJid = ProtocolParser.ReadString(stream);
            continue;
          case 16:
            instance.FromMe = new bool?(ProtocolParser.ReadBool(stream));
            continue;
          case 26:
            instance.Id = ProtocolParser.ReadString(stream);
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

    public static void Serialize(Stream stream, MessageKey instance)
    {
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.RemoteJid != null)
      {
        stream.WriteByte((byte) 10);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.RemoteJid));
      }
      if (instance.FromMe.HasValue)
      {
        stream.WriteByte((byte) 16);
        ProtocolParser.WriteBool(stream, instance.FromMe.Value);
      }
      if (instance.Id != null)
      {
        stream.WriteByte((byte) 26);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Id));
      }
      if (instance.Participant != null)
      {
        stream.WriteByte((byte) 34);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Participant));
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(MessageKey instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        MessageKey.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, MessageKey instance)
    {
      byte[] bytes = MessageKey.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }
  }
}
