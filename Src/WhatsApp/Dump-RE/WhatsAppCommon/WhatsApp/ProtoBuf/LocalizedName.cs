// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.LocalizedName
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System.IO;
using System.Text;

#nullable disable
namespace WhatsApp.ProtoBuf
{
  public class LocalizedName
  {
    public string Lg { get; set; }

    public string Lc { get; set; }

    public string VerifiedName { get; set; }

    public static LocalizedName Deserialize(Stream stream)
    {
      LocalizedName instance = new LocalizedName();
      LocalizedName.Deserialize(stream, instance);
      return instance;
    }

    public static LocalizedName DeserializeLengthDelimited(Stream stream)
    {
      LocalizedName instance = new LocalizedName();
      LocalizedName.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static LocalizedName DeserializeLength(Stream stream, int length)
    {
      LocalizedName instance = new LocalizedName();
      LocalizedName.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static LocalizedName Deserialize(byte[] buffer)
    {
      LocalizedName instance = new LocalizedName();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        LocalizedName.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static LocalizedName Deserialize(byte[] buffer, LocalizedName instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        LocalizedName.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static LocalizedName Deserialize(Stream stream, LocalizedName instance)
    {
      while (true)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            goto label_7;
          case 10:
            instance.Lg = ProtocolParser.ReadString(stream);
            continue;
          case 18:
            instance.Lc = ProtocolParser.ReadString(stream);
            continue;
          case 26:
            instance.VerifiedName = ProtocolParser.ReadString(stream);
            continue;
          default:
            SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
            if (key.Field != 0U)
            {
              ProtocolParser.SkipKey(stream, key);
              continue;
            }
            goto label_5;
        }
      }
label_5:
      throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_7:
      return instance;
    }

    public static LocalizedName DeserializeLengthDelimited(Stream stream, LocalizedName instance)
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
            instance.Lg = ProtocolParser.ReadString(stream);
            continue;
          case 18:
            instance.Lc = ProtocolParser.ReadString(stream);
            continue;
          case 26:
            instance.VerifiedName = ProtocolParser.ReadString(stream);
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

    public static LocalizedName DeserializeLength(
      Stream stream,
      int length,
      LocalizedName instance)
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
            instance.Lg = ProtocolParser.ReadString(stream);
            continue;
          case 18:
            instance.Lc = ProtocolParser.ReadString(stream);
            continue;
          case 26:
            instance.VerifiedName = ProtocolParser.ReadString(stream);
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

    public static void Serialize(Stream stream, LocalizedName instance)
    {
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.Lg != null)
      {
        stream.WriteByte((byte) 10);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Lg));
      }
      if (instance.Lc != null)
      {
        stream.WriteByte((byte) 18);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Lc));
      }
      if (instance.VerifiedName != null)
      {
        stream.WriteByte((byte) 26);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.VerifiedName));
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(LocalizedName instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        LocalizedName.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, LocalizedName instance)
    {
      byte[] bytes = LocalizedName.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }
  }
}
