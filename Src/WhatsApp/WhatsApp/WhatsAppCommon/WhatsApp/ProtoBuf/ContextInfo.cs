// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.ContextInfo
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace WhatsApp.ProtoBuf
{
  public class ContextInfo
  {
    public string StanzaId { get; set; }

    public string Participant { get; set; }

    public Message QuotedMessage { get; set; }

    public string RemoteJid { get; set; }

    public List<string> MentionedJid { get; set; }

    public string ConversionSource { get; set; }

    public byte[] ConversionData { get; set; }

    public uint? ConversionDelaySeconds { get; set; }

    public bool? IsForwarded { get; set; }

    public static ContextInfo Deserialize(Stream stream)
    {
      ContextInfo instance = new ContextInfo();
      ContextInfo.Deserialize(stream, instance);
      return instance;
    }

    public static ContextInfo DeserializeLengthDelimited(Stream stream)
    {
      ContextInfo instance = new ContextInfo();
      ContextInfo.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static ContextInfo DeserializeLength(Stream stream, int length)
    {
      ContextInfo instance = new ContextInfo();
      ContextInfo.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static ContextInfo Deserialize(byte[] buffer)
    {
      ContextInfo instance = new ContextInfo();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        ContextInfo.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static ContextInfo Deserialize(byte[] buffer, ContextInfo instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        ContextInfo.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static ContextInfo Deserialize(Stream stream, ContextInfo instance)
    {
      if (instance.MentionedJid == null)
        instance.MentionedJid = new List<string>();
      while (true)
      {
        SilentOrbit.ProtocolBuffers.Key key;
        do
        {
          do
          {
            do
            {
              do
              {
                int firstByte = stream.ReadByte();
                switch (firstByte)
                {
                  case -1:
                    goto label_21;
                  case 10:
                    instance.StanzaId = ProtocolParser.ReadString(stream);
                    continue;
                  case 18:
                    instance.Participant = ProtocolParser.ReadString(stream);
                    continue;
                  case 26:
                    if (instance.QuotedMessage == null)
                    {
                      instance.QuotedMessage = Message.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    Message.DeserializeLengthDelimited(stream, instance.QuotedMessage);
                    continue;
                  case 34:
                    instance.RemoteJid = ProtocolParser.ReadString(stream);
                    continue;
                  case 122:
                    instance.MentionedJid.Add(ProtocolParser.ReadString(stream));
                    continue;
                  default:
                    key = ProtocolParser.ReadKey((byte) firstByte, stream);
                    switch (key.Field)
                    {
                      case 0:
                        throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                      case 18:
                        continue;
                      case 19:
                        goto label_14;
                      case 20:
                        goto label_16;
                      case 22:
                        goto label_18;
                      default:
                        goto label_20;
                    }
                }
              }
              while (key.WireType != Wire.LengthDelimited);
              instance.ConversionSource = ProtocolParser.ReadString(stream);
              continue;
label_14:;
            }
            while (key.WireType != Wire.LengthDelimited);
            instance.ConversionData = ProtocolParser.ReadBytes(stream);
            continue;
label_16:;
          }
          while (key.WireType != Wire.Varint);
          instance.ConversionDelaySeconds = new uint?(ProtocolParser.ReadUInt32(stream));
          continue;
label_18:;
        }
        while (key.WireType != Wire.Varint);
        instance.IsForwarded = new bool?(ProtocolParser.ReadBool(stream));
        continue;
label_20:
        ProtocolParser.SkipKey(stream, key);
      }
label_21:
      return instance;
    }

    public static ContextInfo DeserializeLengthDelimited(Stream stream, ContextInfo instance)
    {
      if (instance.MentionedJid == null)
        instance.MentionedJid = new List<string>();
      long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
      while (stream.Position < num)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            throw new EndOfStreamException();
          case 10:
            instance.StanzaId = ProtocolParser.ReadString(stream);
            continue;
          case 18:
            instance.Participant = ProtocolParser.ReadString(stream);
            continue;
          case 26:
            if (instance.QuotedMessage == null)
            {
              instance.QuotedMessage = Message.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.DeserializeLengthDelimited(stream, instance.QuotedMessage);
            continue;
          case 34:
            instance.RemoteJid = ProtocolParser.ReadString(stream);
            continue;
          case 122:
            instance.MentionedJid.Add(ProtocolParser.ReadString(stream));
            continue;
          default:
            SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
            switch (key.Field)
            {
              case 0:
                throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
              case 18:
                if (key.WireType == Wire.LengthDelimited)
                {
                  instance.ConversionSource = ProtocolParser.ReadString(stream);
                  continue;
                }
                continue;
              case 19:
                if (key.WireType == Wire.LengthDelimited)
                {
                  instance.ConversionData = ProtocolParser.ReadBytes(stream);
                  continue;
                }
                continue;
              case 20:
                if (key.WireType == Wire.Varint)
                {
                  instance.ConversionDelaySeconds = new uint?(ProtocolParser.ReadUInt32(stream));
                  continue;
                }
                continue;
              case 22:
                if (key.WireType == Wire.Varint)
                {
                  instance.IsForwarded = new bool?(ProtocolParser.ReadBool(stream));
                  continue;
                }
                continue;
              default:
                ProtocolParser.SkipKey(stream, key);
                continue;
            }
        }
      }
      if (stream.Position != num)
        throw new ProtocolBufferException("Read past max limit");
      return instance;
    }

    public static ContextInfo DeserializeLength(Stream stream, int length, ContextInfo instance)
    {
      if (instance.MentionedJid == null)
        instance.MentionedJid = new List<string>();
      long num = stream.Position + (long) length;
      while (stream.Position < num)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            throw new EndOfStreamException();
          case 10:
            instance.StanzaId = ProtocolParser.ReadString(stream);
            continue;
          case 18:
            instance.Participant = ProtocolParser.ReadString(stream);
            continue;
          case 26:
            if (instance.QuotedMessage == null)
            {
              instance.QuotedMessage = Message.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.DeserializeLengthDelimited(stream, instance.QuotedMessage);
            continue;
          case 34:
            instance.RemoteJid = ProtocolParser.ReadString(stream);
            continue;
          case 122:
            instance.MentionedJid.Add(ProtocolParser.ReadString(stream));
            continue;
          default:
            SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
            switch (key.Field)
            {
              case 0:
                throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
              case 18:
                if (key.WireType == Wire.LengthDelimited)
                {
                  instance.ConversionSource = ProtocolParser.ReadString(stream);
                  continue;
                }
                continue;
              case 19:
                if (key.WireType == Wire.LengthDelimited)
                {
                  instance.ConversionData = ProtocolParser.ReadBytes(stream);
                  continue;
                }
                continue;
              case 20:
                if (key.WireType == Wire.Varint)
                {
                  instance.ConversionDelaySeconds = new uint?(ProtocolParser.ReadUInt32(stream));
                  continue;
                }
                continue;
              case 22:
                if (key.WireType == Wire.Varint)
                {
                  instance.IsForwarded = new bool?(ProtocolParser.ReadBool(stream));
                  continue;
                }
                continue;
              default:
                ProtocolParser.SkipKey(stream, key);
                continue;
            }
        }
      }
      if (stream.Position != num)
        throw new ProtocolBufferException("Read past max limit");
      return instance;
    }

    public static void Serialize(Stream stream, ContextInfo instance)
    {
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.StanzaId != null)
      {
        stream.WriteByte((byte) 10);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.StanzaId));
      }
      if (instance.Participant != null)
      {
        stream.WriteByte((byte) 18);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Participant));
      }
      if (instance.QuotedMessage != null)
      {
        stream.WriteByte((byte) 26);
        stream1.SetLength(0L);
        Message.Serialize((Stream) stream1, instance.QuotedMessage);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.RemoteJid != null)
      {
        stream.WriteByte((byte) 34);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.RemoteJid));
      }
      if (instance.MentionedJid != null)
      {
        foreach (string s in instance.MentionedJid)
        {
          stream.WriteByte((byte) 122);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(s));
        }
      }
      if (instance.ConversionSource != null)
      {
        stream.WriteByte((byte) 146);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.ConversionSource));
      }
      if (instance.ConversionData != null)
      {
        stream.WriteByte((byte) 154);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteBytes(stream, instance.ConversionData);
      }
      if (instance.ConversionDelaySeconds.HasValue)
      {
        stream.WriteByte((byte) 160);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteUInt32(stream, instance.ConversionDelaySeconds.Value);
      }
      if (instance.IsForwarded.HasValue)
      {
        stream.WriteByte((byte) 176);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteBool(stream, instance.IsForwarded.Value);
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(ContextInfo instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        ContextInfo.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, ContextInfo instance)
    {
      byte[] bytes = ContextInfo.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }
  }
}
