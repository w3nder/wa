// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.Point
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

#nullable disable
namespace WhatsApp.ProtoBuf
{
  public class Point
  {
    [Obsolete]
    public int? XDeprecated { get; set; }

    [Obsolete]
    public int? YDeprecated { get; set; }

    public double? X { get; set; }

    public double? Y { get; set; }

    public static Point Deserialize(Stream stream)
    {
      Point instance = new Point();
      Point.Deserialize(stream, instance);
      return instance;
    }

    public static Point DeserializeLengthDelimited(Stream stream)
    {
      Point instance = new Point();
      Point.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static Point DeserializeLength(Stream stream, int length)
    {
      Point instance = new Point();
      Point.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static Point Deserialize(byte[] buffer)
    {
      Point instance = new Point();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        Point.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static Point Deserialize(byte[] buffer, Point instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        Point.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static Point Deserialize(Stream stream, Point instance)
    {
      BinaryReader binaryReader = new BinaryReader(stream);
      while (true)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            goto label_9;
          case 8:
            instance.XDeprecated = new int?((int) ProtocolParser.ReadUInt64(stream));
            continue;
          case 16:
            instance.YDeprecated = new int?((int) ProtocolParser.ReadUInt64(stream));
            continue;
          case 25:
            instance.X = new double?(binaryReader.ReadDouble());
            continue;
          case 33:
            instance.Y = new double?(binaryReader.ReadDouble());
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

    public static Point DeserializeLengthDelimited(Stream stream, Point instance)
    {
      BinaryReader binaryReader = new BinaryReader(stream);
      long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
      while (stream.Position < num)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            throw new EndOfStreamException();
          case 8:
            instance.XDeprecated = new int?((int) ProtocolParser.ReadUInt64(stream));
            continue;
          case 16:
            instance.YDeprecated = new int?((int) ProtocolParser.ReadUInt64(stream));
            continue;
          case 25:
            instance.X = new double?(binaryReader.ReadDouble());
            continue;
          case 33:
            instance.Y = new double?(binaryReader.ReadDouble());
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

    public static Point DeserializeLength(Stream stream, int length, Point instance)
    {
      BinaryReader binaryReader = new BinaryReader(stream);
      long num = stream.Position + (long) length;
      while (stream.Position < num)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            throw new EndOfStreamException();
          case 8:
            instance.XDeprecated = new int?((int) ProtocolParser.ReadUInt64(stream));
            continue;
          case 16:
            instance.YDeprecated = new int?((int) ProtocolParser.ReadUInt64(stream));
            continue;
          case 25:
            instance.X = new double?(binaryReader.ReadDouble());
            continue;
          case 33:
            instance.Y = new double?(binaryReader.ReadDouble());
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

    public static void Serialize(Stream stream, Point instance)
    {
      BinaryWriter binaryWriter = new BinaryWriter(stream);
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.XDeprecated.HasValue)
      {
        stream.WriteByte((byte) 8);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.XDeprecated.Value);
      }
      if (instance.YDeprecated.HasValue)
      {
        stream.WriteByte((byte) 16);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.YDeprecated.Value);
      }
      if (instance.X.HasValue)
      {
        stream.WriteByte((byte) 25);
        binaryWriter.Write(instance.X.Value);
      }
      if (instance.Y.HasValue)
      {
        stream.WriteByte((byte) 33);
        binaryWriter.Write(instance.Y.Value);
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(Point instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        Point.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, Point instance)
    {
      byte[] bytes = Point.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }
  }
}
