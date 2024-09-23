// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.Location
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System.IO;
using System.Text;


namespace WhatsApp.ProtoBuf
{
  public class Location
  {
    public double? DegreesLatitude { get; set; }

    public double? DegreesLongitude { get; set; }

    public string Name { get; set; }

    public static Location Deserialize(Stream stream)
    {
      Location instance = new Location();
      Location.Deserialize(stream, instance);
      return instance;
    }

    public static Location DeserializeLengthDelimited(Stream stream)
    {
      Location instance = new Location();
      Location.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static Location DeserializeLength(Stream stream, int length)
    {
      Location instance = new Location();
      Location.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static Location Deserialize(byte[] buffer)
    {
      Location instance = new Location();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        Location.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static Location Deserialize(byte[] buffer, Location instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        Location.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static Location Deserialize(Stream stream, Location instance)
    {
      BinaryReader binaryReader = new BinaryReader(stream);
      while (true)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            goto label_8;
          case 9:
            instance.DegreesLatitude = new double?(binaryReader.ReadDouble());
            continue;
          case 17:
            instance.DegreesLongitude = new double?(binaryReader.ReadDouble());
            continue;
          case 26:
            instance.Name = ProtocolParser.ReadString(stream);
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

    public static Location DeserializeLengthDelimited(Stream stream, Location instance)
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
          case 9:
            instance.DegreesLatitude = new double?(binaryReader.ReadDouble());
            continue;
          case 17:
            instance.DegreesLongitude = new double?(binaryReader.ReadDouble());
            continue;
          case 26:
            instance.Name = ProtocolParser.ReadString(stream);
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

    public static Location DeserializeLength(Stream stream, int length, Location instance)
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
          case 9:
            instance.DegreesLatitude = new double?(binaryReader.ReadDouble());
            continue;
          case 17:
            instance.DegreesLongitude = new double?(binaryReader.ReadDouble());
            continue;
          case 26:
            instance.Name = ProtocolParser.ReadString(stream);
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

    public static void Serialize(Stream stream, Location instance)
    {
      BinaryWriter binaryWriter = new BinaryWriter(stream);
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.DegreesLatitude.HasValue)
      {
        stream.WriteByte((byte) 9);
        binaryWriter.Write(instance.DegreesLatitude.Value);
      }
      if (instance.DegreesLongitude.HasValue)
      {
        stream.WriteByte((byte) 17);
        binaryWriter.Write(instance.DegreesLongitude.Value);
      }
      if (instance.Name != null)
      {
        stream.WriteByte((byte) 26);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Name));
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(Location instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        Location.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, Location instance)
    {
      byte[] bytes = Location.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }
  }
}
