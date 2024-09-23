// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.InteractiveAnnotation
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System.Collections.Generic;
using System.IO;


namespace WhatsApp.ProtoBuf
{
  public class InteractiveAnnotation
  {
    public List<Point> PolygonVertices { get; set; }

    public Location Location { get; set; }

    public static InteractiveAnnotation Deserialize(Stream stream)
    {
      InteractiveAnnotation instance = new InteractiveAnnotation();
      InteractiveAnnotation.Deserialize(stream, instance);
      return instance;
    }

    public static InteractiveAnnotation DeserializeLengthDelimited(Stream stream)
    {
      InteractiveAnnotation instance = new InteractiveAnnotation();
      InteractiveAnnotation.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static InteractiveAnnotation DeserializeLength(Stream stream, int length)
    {
      InteractiveAnnotation instance = new InteractiveAnnotation();
      InteractiveAnnotation.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static InteractiveAnnotation Deserialize(byte[] buffer)
    {
      InteractiveAnnotation instance = new InteractiveAnnotation();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        InteractiveAnnotation.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static InteractiveAnnotation Deserialize(byte[] buffer, InteractiveAnnotation instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        InteractiveAnnotation.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static InteractiveAnnotation Deserialize(Stream stream, InteractiveAnnotation instance)
    {
      if (instance.PolygonVertices == null)
        instance.PolygonVertices = new List<Point>();
      while (true)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            goto label_10;
          case 10:
            instance.PolygonVertices.Add(Point.DeserializeLengthDelimited(stream));
            continue;
          case 18:
            if (instance.Location == null)
            {
              instance.Location = Location.DeserializeLengthDelimited(stream);
              continue;
            }
            Location.DeserializeLengthDelimited(stream, instance.Location);
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

    public static InteractiveAnnotation DeserializeLengthDelimited(
      Stream stream,
      InteractiveAnnotation instance)
    {
      if (instance.PolygonVertices == null)
        instance.PolygonVertices = new List<Point>();
      long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
      while (stream.Position < num)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            throw new EndOfStreamException();
          case 10:
            instance.PolygonVertices.Add(Point.DeserializeLengthDelimited(stream));
            continue;
          case 18:
            if (instance.Location == null)
            {
              instance.Location = Location.DeserializeLengthDelimited(stream);
              continue;
            }
            Location.DeserializeLengthDelimited(stream, instance.Location);
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

    public static InteractiveAnnotation DeserializeLength(
      Stream stream,
      int length,
      InteractiveAnnotation instance)
    {
      if (instance.PolygonVertices == null)
        instance.PolygonVertices = new List<Point>();
      long num = stream.Position + (long) length;
      while (stream.Position < num)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            throw new EndOfStreamException();
          case 10:
            instance.PolygonVertices.Add(Point.DeserializeLengthDelimited(stream));
            continue;
          case 18:
            if (instance.Location == null)
            {
              instance.Location = Location.DeserializeLengthDelimited(stream);
              continue;
            }
            Location.DeserializeLengthDelimited(stream, instance.Location);
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

    public static void Serialize(Stream stream, InteractiveAnnotation instance)
    {
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.PolygonVertices != null)
      {
        foreach (Point polygonVertex in instance.PolygonVertices)
        {
          stream.WriteByte((byte) 10);
          stream1.SetLength(0L);
          Point.Serialize((Stream) stream1, polygonVertex);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
      }
      if (instance.Location != null)
      {
        stream.WriteByte((byte) 18);
        stream1.SetLength(0L);
        Location.Serialize((Stream) stream1, instance.Location);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(InteractiveAnnotation instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        InteractiveAnnotation.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, InteractiveAnnotation instance)
    {
      byte[] bytes = InteractiveAnnotation.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }
  }
}
