// Decompiled with JetBrains decompiler
// Type: SilentOrbit.ProtocolBuffers.ProtocolParser
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;
using System.Text;


namespace SilentOrbit.ProtocolBuffers
{
  public static class ProtocolParser
  {
    public static MemoryStreamStack Stack = (MemoryStreamStack) new AllocationStack();

    public static string ReadString(Stream stream)
    {
      byte[] bytes = ProtocolParser.ReadBytes(stream);
      return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
    }

    public static byte[] ReadBytes(Stream stream)
    {
      int length = (int) ProtocolParser.ReadUInt32(stream);
      byte[] buffer = new byte[length];
      int num;
      for (int offset = 0; offset < length; offset += num)
      {
        num = stream.Read(buffer, offset, length - offset);
        if (num == 0)
          throw new ProtocolBufferException("Expected " + (object) (length - offset) + " got " + (object) offset);
      }
      return buffer;
    }

    public static void SkipBytes(Stream stream)
    {
      int offset = (int) ProtocolParser.ReadUInt32(stream);
      if (stream.CanSeek)
        stream.Seek((long) offset, SeekOrigin.Current);
      else
        ProtocolParser.ReadBytes(stream);
    }

    public static void WriteString(Stream stream, string val)
    {
      ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(val));
    }

    public static void WriteBytes(Stream stream, byte[] val)
    {
      ProtocolParser.WriteUInt32(stream, (uint) val.Length);
      stream.Write(val, 0, val.Length);
    }

    [Obsolete("Only for reference")]
    public static ulong ReadFixed64(BinaryReader reader) => reader.ReadUInt64();

    [Obsolete("Only for reference")]
    public static long ReadSFixed64(BinaryReader reader) => reader.ReadInt64();

    [Obsolete("Only for reference")]
    public static uint ReadFixed32(BinaryReader reader) => reader.ReadUInt32();

    [Obsolete("Only for reference")]
    public static int ReadSFixed32(BinaryReader reader) => reader.ReadInt32();

    [Obsolete("Only for reference")]
    public static void WriteFixed64(BinaryWriter writer, ulong val) => writer.Write(val);

    [Obsolete("Only for reference")]
    public static void WriteSFixed64(BinaryWriter writer, long val) => writer.Write(val);

    [Obsolete("Only for reference")]
    public static void WriteFixed32(BinaryWriter writer, uint val) => writer.Write(val);

    [Obsolete("Only for reference")]
    public static void WriteSFixed32(BinaryWriter writer, int val) => writer.Write(val);

    [Obsolete("Only for reference")]
    public static float ReadFloat(BinaryReader reader) => reader.ReadSingle();

    [Obsolete("Only for reference")]
    public static double ReadDouble(BinaryReader reader) => reader.ReadDouble();

    [Obsolete("Only for reference")]
    public static void WriteFloat(BinaryWriter writer, float val) => writer.Write(val);

    [Obsolete("Only for reference")]
    public static void WriteDouble(BinaryWriter writer, double val) => writer.Write(val);

    public static Key ReadKey(Stream stream)
    {
      uint num = ProtocolParser.ReadUInt32(stream);
      return new Key(num >> 3, (Wire) ((int) num & 7));
    }

    public static Key ReadKey(byte firstByte, Stream stream)
    {
      return firstByte < (byte) 128 ? new Key((uint) firstByte >> 3, (Wire) ((int) firstByte & 7)) : new Key((uint) ((int) ProtocolParser.ReadUInt32(stream) << 4 | (int) firstByte >> 3 & 15), (Wire) ((int) firstByte & 7));
    }

    public static void WriteKey(Stream stream, Key key)
    {
      uint val = (uint) ((Wire) ((int) key.Field << 3) | key.WireType);
      ProtocolParser.WriteUInt32(stream, val);
    }

    public static void SkipKey(Stream stream, Key key)
    {
      switch (key.WireType)
      {
        case Wire.Varint:
          ProtocolParser.ReadSkipVarInt(stream);
          break;
        case Wire.Fixed64:
          stream.Seek(8L, SeekOrigin.Current);
          break;
        case Wire.LengthDelimited:
          stream.Seek((long) ProtocolParser.ReadUInt32(stream), SeekOrigin.Current);
          break;
        case Wire.Fixed32:
          stream.Seek(4L, SeekOrigin.Current);
          break;
        default:
          throw new NotImplementedException("Unknown wire type: " + (object) key.WireType);
      }
    }

    public static byte[] ReadValueBytes(Stream stream, Key key)
    {
      int offset = 0;
      switch (key.WireType)
      {
        case Wire.Varint:
          return ProtocolParser.ReadVarIntBytes(stream);
        case Wire.Fixed64:
          byte[] buffer1 = new byte[8];
          while (offset < 8)
            offset += stream.Read(buffer1, offset, 8 - offset);
          return buffer1;
        case Wire.LengthDelimited:
          uint val = ProtocolParser.ReadUInt32(stream);
          byte[] buffer2;
          using (MemoryStream memoryStream = new MemoryStream())
          {
            ProtocolParser.WriteUInt32((Stream) memoryStream, val);
            buffer2 = new byte[(long) val + memoryStream.Length];
            memoryStream.ToArray().CopyTo((Array) buffer2, 0);
            offset = (int) memoryStream.Length;
          }
          while (offset < buffer2.Length)
            offset += stream.Read(buffer2, offset, buffer2.Length - offset);
          return buffer2;
        case Wire.Fixed32:
          byte[] buffer3 = new byte[4];
          while (offset < 4)
            offset += stream.Read(buffer3, offset, 4 - offset);
          return buffer3;
        default:
          throw new NotImplementedException("Unknown wire type: " + (object) key.WireType);
      }
    }

    public static void ReadSkipVarInt(Stream stream)
    {
      int num;
      do
      {
        num = stream.ReadByte();
        if (num < 0)
          throw new IOException("Stream ended too early");
      }
      while ((num & 128) != 0);
    }

    public static byte[] ReadVarIntBytes(Stream stream)
    {
      byte[] sourceArray = new byte[10];
      int length = 0;
      do
      {
        int num = stream.ReadByte();
        sourceArray[length] = num >= 0 ? (byte) num : throw new IOException("Stream ended too early");
        ++length;
        if ((num & 128) == 0)
          goto label_6;
      }
      while (length < sourceArray.Length);
      throw new ProtocolBufferException("VarInt too long, more than 10 bytes");
label_6:
      byte[] destinationArray = new byte[length];
      Array.Copy((Array) sourceArray, (Array) destinationArray, destinationArray.Length);
      return destinationArray;
    }

    [Obsolete("Use (int)ReadUInt64(stream); //yes 64")]
    public static int ReadInt32(Stream stream) => (int) ProtocolParser.ReadUInt64(stream);

    [Obsolete("Use WriteUInt64(stream, (ulong)val); //yes 64, negative numbers are encoded that way")]
    public static void WriteInt32(Stream stream, int val)
    {
      ProtocolParser.WriteUInt64(stream, (ulong) val);
    }

    public static int ReadZInt32(Stream stream)
    {
      uint num = ProtocolParser.ReadUInt32(stream);
      return (int) (num >> 1) ^ (int) num << 31 >> 31;
    }

    public static void WriteZInt32(Stream stream, int val)
    {
      ProtocolParser.WriteUInt32(stream, (uint) (val << 1 ^ val >> 31));
    }

    public static uint ReadUInt32(Stream stream)
    {
      uint num1 = 0;
      for (int index = 0; index < 5; ++index)
      {
        int num2 = stream.ReadByte();
        if (num2 < 0)
          throw new IOException("Stream ended too early");
        if (index == 4 && (num2 & 240) != 0)
          throw new ProtocolBufferException("Got larger VarInt than 32bit unsigned");
        if ((num2 & 128) == 0)
          return num1 | (uint) (num2 << 7 * index);
        num1 |= (uint) ((num2 & (int) sbyte.MaxValue) << 7 * index);
      }
      throw new ProtocolBufferException("Got larger VarInt than 32bit unsigned");
    }

    public static void WriteUInt32(Stream stream, uint val)
    {
      byte num1;
      while (true)
      {
        num1 = (byte) (val & (uint) sbyte.MaxValue);
        val >>= 7;
        if (val != 0U)
        {
          byte num2 = (byte) ((uint) num1 | 128U);
          stream.WriteByte(num2);
        }
        else
          break;
      }
      stream.WriteByte(num1);
    }

    [Obsolete("Use (long)ReadUInt64(stream); instead")]
    public static int ReadInt64(Stream stream) => (int) ProtocolParser.ReadUInt64(stream);

    [Obsolete("Use WriteUInt64 (stream, (ulong)val); instead")]
    public static void WriteInt64(Stream stream, int val)
    {
      ProtocolParser.WriteUInt64(stream, (ulong) val);
    }

    public static long ReadZInt64(Stream stream)
    {
      ulong num = ProtocolParser.ReadUInt64(stream);
      return (long) (num >> 1) ^ (long) num << 63 >> 63;
    }

    public static void WriteZInt64(Stream stream, long val)
    {
      ProtocolParser.WriteUInt64(stream, (ulong) (val << 1 ^ val >> 63));
    }

    public static ulong ReadUInt64(Stream stream)
    {
      ulong num1 = 0;
      for (int index = 0; index < 10; ++index)
      {
        int num2 = stream.ReadByte();
        if (num2 < 0)
          throw new IOException("Stream ended too early");
        if (index == 9 && (num2 & 254) != 0)
          throw new ProtocolBufferException("Got larger VarInt than 64 bit unsigned");
        if ((num2 & 128) == 0)
          return num1 | (ulong) num2 << 7 * index;
        num1 |= (ulong) (num2 & (int) sbyte.MaxValue) << 7 * index;
      }
      throw new ProtocolBufferException("Got larger VarInt than 64 bit unsigned");
    }

    public static void WriteUInt64(Stream stream, ulong val)
    {
      byte num1;
      while (true)
      {
        num1 = (byte) (val & (ulong) sbyte.MaxValue);
        val >>= 7;
        if (val != 0UL)
        {
          byte num2 = (byte) ((uint) num1 | 128U);
          stream.WriteByte(num2);
        }
        else
          break;
      }
      stream.WriteByte(num1);
    }

    public static bool ReadBool(Stream stream)
    {
      int num = stream.ReadByte();
      if (num < 0)
        throw new IOException("Stream ended too early");
      if (num == 1)
        return true;
      if (num == 0)
        return false;
      throw new ProtocolBufferException("Invalid boolean value");
    }

    public static void WriteBool(Stream stream, bool val)
    {
      stream.WriteByte(val ? (byte) 1 : (byte) 0);
    }
  }
}
