// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.NoiseCertificate
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System.IO;
using System.Text;


namespace WhatsApp.ProtoBuf
{
  public class NoiseCertificate
  {
    public byte[] DetailsField { get; set; }

    public byte[] Signature { get; set; }

    public static NoiseCertificate Deserialize(Stream stream)
    {
      NoiseCertificate instance = new NoiseCertificate();
      NoiseCertificate.Deserialize(stream, instance);
      return instance;
    }

    public static NoiseCertificate DeserializeLengthDelimited(Stream stream)
    {
      NoiseCertificate instance = new NoiseCertificate();
      NoiseCertificate.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static NoiseCertificate DeserializeLength(Stream stream, int length)
    {
      NoiseCertificate instance = new NoiseCertificate();
      NoiseCertificate.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static NoiseCertificate Deserialize(byte[] buffer)
    {
      NoiseCertificate instance = new NoiseCertificate();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        NoiseCertificate.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static NoiseCertificate Deserialize(byte[] buffer, NoiseCertificate instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        NoiseCertificate.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static NoiseCertificate Deserialize(Stream stream, NoiseCertificate instance)
    {
      while (true)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            goto label_6;
          case 10:
            instance.DetailsField = ProtocolParser.ReadBytes(stream);
            continue;
          case 18:
            instance.Signature = ProtocolParser.ReadBytes(stream);
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

    public static NoiseCertificate DeserializeLengthDelimited(
      Stream stream,
      NoiseCertificate instance)
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
            instance.DetailsField = ProtocolParser.ReadBytes(stream);
            continue;
          case 18:
            instance.Signature = ProtocolParser.ReadBytes(stream);
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

    public static NoiseCertificate DeserializeLength(
      Stream stream,
      int length,
      NoiseCertificate instance)
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
            instance.DetailsField = ProtocolParser.ReadBytes(stream);
            continue;
          case 18:
            instance.Signature = ProtocolParser.ReadBytes(stream);
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

    public static void Serialize(Stream stream, NoiseCertificate instance)
    {
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.DetailsField != null)
      {
        stream.WriteByte((byte) 10);
        ProtocolParser.WriteBytes(stream, instance.DetailsField);
      }
      if (instance.Signature != null)
      {
        stream.WriteByte((byte) 18);
        ProtocolParser.WriteBytes(stream, instance.Signature);
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(NoiseCertificate instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        NoiseCertificate.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, NoiseCertificate instance)
    {
      byte[] bytes = NoiseCertificate.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }

    public class Details
    {
      public uint? Serial { get; set; }

      public string Issuer { get; set; }

      public ulong? Expires { get; set; }

      public string Subject { get; set; }

      public byte[] Key { get; set; }

      public static NoiseCertificate.Details Deserialize(Stream stream)
      {
        NoiseCertificate.Details instance = new NoiseCertificate.Details();
        NoiseCertificate.Details.Deserialize(stream, instance);
        return instance;
      }

      public static NoiseCertificate.Details DeserializeLengthDelimited(Stream stream)
      {
        NoiseCertificate.Details instance = new NoiseCertificate.Details();
        NoiseCertificate.Details.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static NoiseCertificate.Details DeserializeLength(Stream stream, int length)
      {
        NoiseCertificate.Details instance = new NoiseCertificate.Details();
        NoiseCertificate.Details.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static NoiseCertificate.Details Deserialize(byte[] buffer)
      {
        NoiseCertificate.Details instance = new NoiseCertificate.Details();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          NoiseCertificate.Details.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static NoiseCertificate.Details Deserialize(
        byte[] buffer,
        NoiseCertificate.Details instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          NoiseCertificate.Details.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static NoiseCertificate.Details Deserialize(
        Stream stream,
        NoiseCertificate.Details instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_9;
            case 8:
              instance.Serial = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 18:
              instance.Issuer = ProtocolParser.ReadString(stream);
              continue;
            case 24:
              instance.Expires = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 34:
              instance.Subject = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.Key = ProtocolParser.ReadBytes(stream);
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

      public static NoiseCertificate.Details DeserializeLengthDelimited(
        Stream stream,
        NoiseCertificate.Details instance)
      {
        long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 8:
              instance.Serial = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 18:
              instance.Issuer = ProtocolParser.ReadString(stream);
              continue;
            case 24:
              instance.Expires = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 34:
              instance.Subject = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.Key = ProtocolParser.ReadBytes(stream);
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

      public static NoiseCertificate.Details DeserializeLength(
        Stream stream,
        int length,
        NoiseCertificate.Details instance)
      {
        long num = stream.Position + (long) length;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 8:
              instance.Serial = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 18:
              instance.Issuer = ProtocolParser.ReadString(stream);
              continue;
            case 24:
              instance.Expires = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 34:
              instance.Subject = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.Key = ProtocolParser.ReadBytes(stream);
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

      public static void Serialize(Stream stream, NoiseCertificate.Details instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Serial.HasValue)
        {
          stream.WriteByte((byte) 8);
          ProtocolParser.WriteUInt32(stream, instance.Serial.Value);
        }
        if (instance.Issuer != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Issuer));
        }
        if (instance.Expires.HasValue)
        {
          stream.WriteByte((byte) 24);
          ProtocolParser.WriteUInt64(stream, instance.Expires.Value);
        }
        if (instance.Subject != null)
        {
          stream.WriteByte((byte) 34);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Subject));
        }
        if (instance.Key != null)
        {
          stream.WriteByte((byte) 42);
          ProtocolParser.WriteBytes(stream, instance.Key);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(NoiseCertificate.Details instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          NoiseCertificate.Details.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(Stream stream, NoiseCertificate.Details instance)
      {
        byte[] bytes = NoiseCertificate.Details.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }
    }
  }
}
