// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.VerifiedNameCertificate
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

#nullable disable
namespace WhatsApp.ProtoBuf
{
  public class VerifiedNameCertificate
  {
    public byte[] DetailsField { get; set; }

    public byte[] Signature { get; set; }

    public static VerifiedNameCertificate Deserialize(Stream stream)
    {
      VerifiedNameCertificate instance = new VerifiedNameCertificate();
      VerifiedNameCertificate.Deserialize(stream, instance);
      return instance;
    }

    public static VerifiedNameCertificate DeserializeLengthDelimited(Stream stream)
    {
      VerifiedNameCertificate instance = new VerifiedNameCertificate();
      VerifiedNameCertificate.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static VerifiedNameCertificate DeserializeLength(Stream stream, int length)
    {
      VerifiedNameCertificate instance = new VerifiedNameCertificate();
      VerifiedNameCertificate.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static VerifiedNameCertificate Deserialize(byte[] buffer)
    {
      VerifiedNameCertificate instance = new VerifiedNameCertificate();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        VerifiedNameCertificate.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static VerifiedNameCertificate Deserialize(
      byte[] buffer,
      VerifiedNameCertificate instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        VerifiedNameCertificate.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static VerifiedNameCertificate Deserialize(
      Stream stream,
      VerifiedNameCertificate instance)
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

    public static VerifiedNameCertificate DeserializeLengthDelimited(
      Stream stream,
      VerifiedNameCertificate instance)
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

    public static VerifiedNameCertificate DeserializeLength(
      Stream stream,
      int length,
      VerifiedNameCertificate instance)
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

    public static void Serialize(Stream stream, VerifiedNameCertificate instance)
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

    public static byte[] SerializeToBytes(VerifiedNameCertificate instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        VerifiedNameCertificate.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, VerifiedNameCertificate instance)
    {
      byte[] bytes = VerifiedNameCertificate.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }

    public class Details
    {
      public ulong? Serial { get; set; }

      public string Issuer { get; set; }

      public ulong? Expires { get; set; }

      public string VerifiedName { get; set; }

      public string Industry { get; set; }

      public string City { get; set; }

      public string Country { get; set; }

      public List<LocalizedName> LocalizedNames { get; set; }

      public static VerifiedNameCertificate.Details Deserialize(Stream stream)
      {
        VerifiedNameCertificate.Details instance = new VerifiedNameCertificate.Details();
        VerifiedNameCertificate.Details.Deserialize(stream, instance);
        return instance;
      }

      public static VerifiedNameCertificate.Details DeserializeLengthDelimited(Stream stream)
      {
        VerifiedNameCertificate.Details instance = new VerifiedNameCertificate.Details();
        VerifiedNameCertificate.Details.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static VerifiedNameCertificate.Details DeserializeLength(Stream stream, int length)
      {
        VerifiedNameCertificate.Details instance = new VerifiedNameCertificate.Details();
        VerifiedNameCertificate.Details.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static VerifiedNameCertificate.Details Deserialize(byte[] buffer)
      {
        VerifiedNameCertificate.Details instance = new VerifiedNameCertificate.Details();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          VerifiedNameCertificate.Details.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static VerifiedNameCertificate.Details Deserialize(
        byte[] buffer,
        VerifiedNameCertificate.Details instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          VerifiedNameCertificate.Details.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static VerifiedNameCertificate.Details Deserialize(
        Stream stream,
        VerifiedNameCertificate.Details instance)
      {
        if (instance.LocalizedNames == null)
          instance.LocalizedNames = new List<LocalizedName>();
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_14;
            case 8:
              instance.Serial = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 18:
              instance.Issuer = ProtocolParser.ReadString(stream);
              continue;
            case 24:
              instance.Expires = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 34:
              instance.VerifiedName = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.Industry = ProtocolParser.ReadString(stream);
              continue;
            case 50:
              instance.City = ProtocolParser.ReadString(stream);
              continue;
            case 58:
              instance.Country = ProtocolParser.ReadString(stream);
              continue;
            case 66:
              instance.LocalizedNames.Add(LocalizedName.DeserializeLengthDelimited(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              if (key.Field != 0U)
              {
                ProtocolParser.SkipKey(stream, key);
                continue;
              }
              goto label_12;
          }
        }
label_12:
        throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_14:
        return instance;
      }

      public static VerifiedNameCertificate.Details DeserializeLengthDelimited(
        Stream stream,
        VerifiedNameCertificate.Details instance)
      {
        if (instance.LocalizedNames == null)
          instance.LocalizedNames = new List<LocalizedName>();
        long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 8:
              instance.Serial = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 18:
              instance.Issuer = ProtocolParser.ReadString(stream);
              continue;
            case 24:
              instance.Expires = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 34:
              instance.VerifiedName = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.Industry = ProtocolParser.ReadString(stream);
              continue;
            case 50:
              instance.City = ProtocolParser.ReadString(stream);
              continue;
            case 58:
              instance.Country = ProtocolParser.ReadString(stream);
              continue;
            case 66:
              instance.LocalizedNames.Add(LocalizedName.DeserializeLengthDelimited(stream));
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

      public static VerifiedNameCertificate.Details DeserializeLength(
        Stream stream,
        int length,
        VerifiedNameCertificate.Details instance)
      {
        if (instance.LocalizedNames == null)
          instance.LocalizedNames = new List<LocalizedName>();
        long num = stream.Position + (long) length;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 8:
              instance.Serial = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 18:
              instance.Issuer = ProtocolParser.ReadString(stream);
              continue;
            case 24:
              instance.Expires = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 34:
              instance.VerifiedName = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.Industry = ProtocolParser.ReadString(stream);
              continue;
            case 50:
              instance.City = ProtocolParser.ReadString(stream);
              continue;
            case 58:
              instance.Country = ProtocolParser.ReadString(stream);
              continue;
            case 66:
              instance.LocalizedNames.Add(LocalizedName.DeserializeLengthDelimited(stream));
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

      public static void Serialize(Stream stream, VerifiedNameCertificate.Details instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Serial.HasValue)
        {
          stream.WriteByte((byte) 8);
          ProtocolParser.WriteUInt64(stream, instance.Serial.Value);
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
        if (instance.VerifiedName != null)
        {
          stream.WriteByte((byte) 34);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.VerifiedName));
        }
        if (instance.Industry != null)
        {
          stream.WriteByte((byte) 42);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Industry));
        }
        if (instance.City != null)
        {
          stream.WriteByte((byte) 50);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.City));
        }
        if (instance.Country != null)
        {
          stream.WriteByte((byte) 58);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Country));
        }
        if (instance.LocalizedNames != null)
        {
          foreach (LocalizedName localizedName in instance.LocalizedNames)
          {
            stream.WriteByte((byte) 66);
            stream1.SetLength(0L);
            LocalizedName.Serialize((Stream) stream1, localizedName);
            uint length = (uint) stream1.Length;
            ProtocolParser.WriteUInt32(stream, length);
            stream1.WriteTo(stream);
          }
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(VerifiedNameCertificate.Details instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          VerifiedNameCertificate.Details.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        VerifiedNameCertificate.Details instance)
      {
        byte[] bytes = VerifiedNameCertificate.Details.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }
    }
  }
}
