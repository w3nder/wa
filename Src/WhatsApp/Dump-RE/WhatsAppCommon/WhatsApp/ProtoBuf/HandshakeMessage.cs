// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.HandshakeMessage
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System.IO;

#nullable disable
namespace WhatsApp.ProtoBuf
{
  public class HandshakeMessage
  {
    public HandshakeMessage.ClientHello ClientHelloField { get; set; }

    public HandshakeMessage.ServerHello ServerHelloField { get; set; }

    public HandshakeMessage.ClientFinish ClientFinishField { get; set; }

    public static HandshakeMessage Deserialize(Stream stream)
    {
      HandshakeMessage instance = new HandshakeMessage();
      HandshakeMessage.Deserialize(stream, instance);
      return instance;
    }

    public static HandshakeMessage DeserializeLengthDelimited(Stream stream)
    {
      HandshakeMessage instance = new HandshakeMessage();
      HandshakeMessage.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static HandshakeMessage DeserializeLength(Stream stream, int length)
    {
      HandshakeMessage instance = new HandshakeMessage();
      HandshakeMessage.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static HandshakeMessage Deserialize(byte[] buffer)
    {
      HandshakeMessage instance = new HandshakeMessage();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        HandshakeMessage.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static HandshakeMessage Deserialize(byte[] buffer, HandshakeMessage instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        HandshakeMessage.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static HandshakeMessage Deserialize(Stream stream, HandshakeMessage instance)
    {
      while (true)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            goto label_13;
          case 18:
            if (instance.ClientHelloField == null)
            {
              instance.ClientHelloField = HandshakeMessage.ClientHello.DeserializeLengthDelimited(stream);
              continue;
            }
            HandshakeMessage.ClientHello.DeserializeLengthDelimited(stream, instance.ClientHelloField);
            continue;
          case 26:
            if (instance.ServerHelloField == null)
            {
              instance.ServerHelloField = HandshakeMessage.ServerHello.DeserializeLengthDelimited(stream);
              continue;
            }
            HandshakeMessage.ServerHello.DeserializeLengthDelimited(stream, instance.ServerHelloField);
            continue;
          case 34:
            if (instance.ClientFinishField == null)
            {
              instance.ClientFinishField = HandshakeMessage.ClientFinish.DeserializeLengthDelimited(stream);
              continue;
            }
            HandshakeMessage.ClientFinish.DeserializeLengthDelimited(stream, instance.ClientFinishField);
            continue;
          default:
            SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
            if (key.Field != 0U)
            {
              ProtocolParser.SkipKey(stream, key);
              continue;
            }
            goto label_11;
        }
      }
label_11:
      throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_13:
      return instance;
    }

    public static HandshakeMessage DeserializeLengthDelimited(
      Stream stream,
      HandshakeMessage instance)
    {
      long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
      while (stream.Position < num)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            throw new EndOfStreamException();
          case 18:
            if (instance.ClientHelloField == null)
            {
              instance.ClientHelloField = HandshakeMessage.ClientHello.DeserializeLengthDelimited(stream);
              continue;
            }
            HandshakeMessage.ClientHello.DeserializeLengthDelimited(stream, instance.ClientHelloField);
            continue;
          case 26:
            if (instance.ServerHelloField == null)
            {
              instance.ServerHelloField = HandshakeMessage.ServerHello.DeserializeLengthDelimited(stream);
              continue;
            }
            HandshakeMessage.ServerHello.DeserializeLengthDelimited(stream, instance.ServerHelloField);
            continue;
          case 34:
            if (instance.ClientFinishField == null)
            {
              instance.ClientFinishField = HandshakeMessage.ClientFinish.DeserializeLengthDelimited(stream);
              continue;
            }
            HandshakeMessage.ClientFinish.DeserializeLengthDelimited(stream, instance.ClientFinishField);
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

    public static HandshakeMessage DeserializeLength(
      Stream stream,
      int length,
      HandshakeMessage instance)
    {
      long num = stream.Position + (long) length;
      while (stream.Position < num)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            throw new EndOfStreamException();
          case 18:
            if (instance.ClientHelloField == null)
            {
              instance.ClientHelloField = HandshakeMessage.ClientHello.DeserializeLengthDelimited(stream);
              continue;
            }
            HandshakeMessage.ClientHello.DeserializeLengthDelimited(stream, instance.ClientHelloField);
            continue;
          case 26:
            if (instance.ServerHelloField == null)
            {
              instance.ServerHelloField = HandshakeMessage.ServerHello.DeserializeLengthDelimited(stream);
              continue;
            }
            HandshakeMessage.ServerHello.DeserializeLengthDelimited(stream, instance.ServerHelloField);
            continue;
          case 34:
            if (instance.ClientFinishField == null)
            {
              instance.ClientFinishField = HandshakeMessage.ClientFinish.DeserializeLengthDelimited(stream);
              continue;
            }
            HandshakeMessage.ClientFinish.DeserializeLengthDelimited(stream, instance.ClientFinishField);
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

    public static void Serialize(Stream stream, HandshakeMessage instance)
    {
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.ClientHelloField != null)
      {
        stream.WriteByte((byte) 18);
        stream1.SetLength(0L);
        HandshakeMessage.ClientHello.Serialize((Stream) stream1, instance.ClientHelloField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.ServerHelloField != null)
      {
        stream.WriteByte((byte) 26);
        stream1.SetLength(0L);
        HandshakeMessage.ServerHello.Serialize((Stream) stream1, instance.ServerHelloField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.ClientFinishField != null)
      {
        stream.WriteByte((byte) 34);
        stream1.SetLength(0L);
        HandshakeMessage.ClientFinish.Serialize((Stream) stream1, instance.ClientFinishField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(HandshakeMessage instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        HandshakeMessage.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, HandshakeMessage instance)
    {
      byte[] bytes = HandshakeMessage.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }

    public class ClientHello
    {
      public byte[] Ephemeral { get; set; }

      public byte[] Static { get; set; }

      public byte[] Payload { get; set; }

      public static HandshakeMessage.ClientHello Deserialize(Stream stream)
      {
        HandshakeMessage.ClientHello instance = new HandshakeMessage.ClientHello();
        HandshakeMessage.ClientHello.Deserialize(stream, instance);
        return instance;
      }

      public static HandshakeMessage.ClientHello DeserializeLengthDelimited(Stream stream)
      {
        HandshakeMessage.ClientHello instance = new HandshakeMessage.ClientHello();
        HandshakeMessage.ClientHello.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static HandshakeMessage.ClientHello DeserializeLength(Stream stream, int length)
      {
        HandshakeMessage.ClientHello instance = new HandshakeMessage.ClientHello();
        HandshakeMessage.ClientHello.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static HandshakeMessage.ClientHello Deserialize(byte[] buffer)
      {
        HandshakeMessage.ClientHello instance = new HandshakeMessage.ClientHello();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          HandshakeMessage.ClientHello.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static HandshakeMessage.ClientHello Deserialize(
        byte[] buffer,
        HandshakeMessage.ClientHello instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          HandshakeMessage.ClientHello.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static HandshakeMessage.ClientHello Deserialize(
        Stream stream,
        HandshakeMessage.ClientHello instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_7;
            case 10:
              instance.Ephemeral = ProtocolParser.ReadBytes(stream);
              continue;
            case 18:
              instance.Static = ProtocolParser.ReadBytes(stream);
              continue;
            case 26:
              instance.Payload = ProtocolParser.ReadBytes(stream);
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

      public static HandshakeMessage.ClientHello DeserializeLengthDelimited(
        Stream stream,
        HandshakeMessage.ClientHello instance)
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
              instance.Ephemeral = ProtocolParser.ReadBytes(stream);
              continue;
            case 18:
              instance.Static = ProtocolParser.ReadBytes(stream);
              continue;
            case 26:
              instance.Payload = ProtocolParser.ReadBytes(stream);
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

      public static HandshakeMessage.ClientHello DeserializeLength(
        Stream stream,
        int length,
        HandshakeMessage.ClientHello instance)
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
              instance.Ephemeral = ProtocolParser.ReadBytes(stream);
              continue;
            case 18:
              instance.Static = ProtocolParser.ReadBytes(stream);
              continue;
            case 26:
              instance.Payload = ProtocolParser.ReadBytes(stream);
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

      public static void Serialize(Stream stream, HandshakeMessage.ClientHello instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Ephemeral != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, instance.Ephemeral);
        }
        if (instance.Static != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, instance.Static);
        }
        if (instance.Payload != null)
        {
          stream.WriteByte((byte) 26);
          ProtocolParser.WriteBytes(stream, instance.Payload);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(HandshakeMessage.ClientHello instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          HandshakeMessage.ClientHello.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        HandshakeMessage.ClientHello instance)
      {
        byte[] bytes = HandshakeMessage.ClientHello.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }
    }

    public class ServerHello
    {
      public byte[] Ephemeral { get; set; }

      public byte[] Static { get; set; }

      public byte[] Payload { get; set; }

      public static HandshakeMessage.ServerHello Deserialize(Stream stream)
      {
        HandshakeMessage.ServerHello instance = new HandshakeMessage.ServerHello();
        HandshakeMessage.ServerHello.Deserialize(stream, instance);
        return instance;
      }

      public static HandshakeMessage.ServerHello DeserializeLengthDelimited(Stream stream)
      {
        HandshakeMessage.ServerHello instance = new HandshakeMessage.ServerHello();
        HandshakeMessage.ServerHello.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static HandshakeMessage.ServerHello DeserializeLength(Stream stream, int length)
      {
        HandshakeMessage.ServerHello instance = new HandshakeMessage.ServerHello();
        HandshakeMessage.ServerHello.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static HandshakeMessage.ServerHello Deserialize(byte[] buffer)
      {
        HandshakeMessage.ServerHello instance = new HandshakeMessage.ServerHello();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          HandshakeMessage.ServerHello.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static HandshakeMessage.ServerHello Deserialize(
        byte[] buffer,
        HandshakeMessage.ServerHello instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          HandshakeMessage.ServerHello.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static HandshakeMessage.ServerHello Deserialize(
        Stream stream,
        HandshakeMessage.ServerHello instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_7;
            case 10:
              instance.Ephemeral = ProtocolParser.ReadBytes(stream);
              continue;
            case 18:
              instance.Static = ProtocolParser.ReadBytes(stream);
              continue;
            case 26:
              instance.Payload = ProtocolParser.ReadBytes(stream);
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

      public static HandshakeMessage.ServerHello DeserializeLengthDelimited(
        Stream stream,
        HandshakeMessage.ServerHello instance)
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
              instance.Ephemeral = ProtocolParser.ReadBytes(stream);
              continue;
            case 18:
              instance.Static = ProtocolParser.ReadBytes(stream);
              continue;
            case 26:
              instance.Payload = ProtocolParser.ReadBytes(stream);
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

      public static HandshakeMessage.ServerHello DeserializeLength(
        Stream stream,
        int length,
        HandshakeMessage.ServerHello instance)
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
              instance.Ephemeral = ProtocolParser.ReadBytes(stream);
              continue;
            case 18:
              instance.Static = ProtocolParser.ReadBytes(stream);
              continue;
            case 26:
              instance.Payload = ProtocolParser.ReadBytes(stream);
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

      public static void Serialize(Stream stream, HandshakeMessage.ServerHello instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Ephemeral != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, instance.Ephemeral);
        }
        if (instance.Static != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, instance.Static);
        }
        if (instance.Payload != null)
        {
          stream.WriteByte((byte) 26);
          ProtocolParser.WriteBytes(stream, instance.Payload);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(HandshakeMessage.ServerHello instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          HandshakeMessage.ServerHello.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        HandshakeMessage.ServerHello instance)
      {
        byte[] bytes = HandshakeMessage.ServerHello.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }
    }

    public class ClientFinish
    {
      public byte[] Static { get; set; }

      public byte[] Payload { get; set; }

      public static HandshakeMessage.ClientFinish Deserialize(Stream stream)
      {
        HandshakeMessage.ClientFinish instance = new HandshakeMessage.ClientFinish();
        HandshakeMessage.ClientFinish.Deserialize(stream, instance);
        return instance;
      }

      public static HandshakeMessage.ClientFinish DeserializeLengthDelimited(Stream stream)
      {
        HandshakeMessage.ClientFinish instance = new HandshakeMessage.ClientFinish();
        HandshakeMessage.ClientFinish.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static HandshakeMessage.ClientFinish DeserializeLength(Stream stream, int length)
      {
        HandshakeMessage.ClientFinish instance = new HandshakeMessage.ClientFinish();
        HandshakeMessage.ClientFinish.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static HandshakeMessage.ClientFinish Deserialize(byte[] buffer)
      {
        HandshakeMessage.ClientFinish instance = new HandshakeMessage.ClientFinish();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          HandshakeMessage.ClientFinish.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static HandshakeMessage.ClientFinish Deserialize(
        byte[] buffer,
        HandshakeMessage.ClientFinish instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          HandshakeMessage.ClientFinish.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static HandshakeMessage.ClientFinish Deserialize(
        Stream stream,
        HandshakeMessage.ClientFinish instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_6;
            case 10:
              instance.Static = ProtocolParser.ReadBytes(stream);
              continue;
            case 18:
              instance.Payload = ProtocolParser.ReadBytes(stream);
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              if (key.Field != 0U)
              {
                ProtocolParser.SkipKey(stream, key);
                continue;
              }
              goto label_4;
          }
        }
label_4:
        throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_6:
        return instance;
      }

      public static HandshakeMessage.ClientFinish DeserializeLengthDelimited(
        Stream stream,
        HandshakeMessage.ClientFinish instance)
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
              instance.Static = ProtocolParser.ReadBytes(stream);
              continue;
            case 18:
              instance.Payload = ProtocolParser.ReadBytes(stream);
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

      public static HandshakeMessage.ClientFinish DeserializeLength(
        Stream stream,
        int length,
        HandshakeMessage.ClientFinish instance)
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
              instance.Static = ProtocolParser.ReadBytes(stream);
              continue;
            case 18:
              instance.Payload = ProtocolParser.ReadBytes(stream);
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

      public static void Serialize(Stream stream, HandshakeMessage.ClientFinish instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Static != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, instance.Static);
        }
        if (instance.Payload != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, instance.Payload);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(HandshakeMessage.ClientFinish instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          HandshakeMessage.ClientFinish.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        HandshakeMessage.ClientFinish instance)
      {
        byte[] bytes = HandshakeMessage.ClientFinish.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }
    }
  }
}
