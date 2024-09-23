// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.WebMessageInfo
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace WhatsApp.ProtoBuf
{
  public class WebMessageInfo
  {
    public MessageKey Key { get; set; }

    public Message Message { get; set; }

    public ulong? MessageTimestamp { get; set; }

    public WebMessageInfo.Status? status { get; set; }

    public string Participant { get; set; }

    public bool? Ignore { get; set; }

    public bool? Starred { get; set; }

    public bool? Broadcast { get; set; }

    public string PushName { get; set; }

    public byte[] MediaCiphertextSha256 { get; set; }

    public bool? Multicast { get; set; }

    public bool? UrlText { get; set; }

    public bool? UrlNumber { get; set; }

    public WebMessageInfo.StubType? MessageStubType { get; set; }

    public bool? ClearMedia { get; set; }

    public List<string> MessageStubParameters { get; set; }

    public uint? Duration { get; set; }

    public List<string> Labels { get; set; }

    public PaymentInfo PaymentInfo { get; set; }

    public Message.LiveLocationMessage FinalLiveLocation { get; set; }

    public PaymentInfo QuotedPaymentInfo { get; set; }

    public static WebMessageInfo Deserialize(Stream stream)
    {
      WebMessageInfo instance = new WebMessageInfo();
      WebMessageInfo.Deserialize(stream, instance);
      return instance;
    }

    public static WebMessageInfo DeserializeLengthDelimited(Stream stream)
    {
      WebMessageInfo instance = new WebMessageInfo();
      WebMessageInfo.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static WebMessageInfo DeserializeLength(Stream stream, int length)
    {
      WebMessageInfo instance = new WebMessageInfo();
      WebMessageInfo.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static WebMessageInfo Deserialize(byte[] buffer)
    {
      WebMessageInfo instance = new WebMessageInfo();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        WebMessageInfo.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static WebMessageInfo Deserialize(byte[] buffer, WebMessageInfo instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        WebMessageInfo.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static WebMessageInfo Deserialize(Stream stream, WebMessageInfo instance)
    {
      instance.status = new WebMessageInfo.Status?(WebMessageInfo.Status.PENDING);
      if (instance.MessageStubParameters == null)
        instance.MessageStubParameters = new List<string>();
      if (instance.Labels == null)
        instance.Labels = new List<string>();
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
                do
                {
                  do
                  {
                    do
                    {
                      do
                      {
                        do
                        {
                          do
                          {
                            do
                            {
                              do
                              {
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
                                            goto label_55;
                                          case 10:
                                            if (instance.Key == null)
                                            {
                                              instance.Key = MessageKey.DeserializeLengthDelimited(stream);
                                              continue;
                                            }
                                            MessageKey.DeserializeLengthDelimited(stream, instance.Key);
                                            continue;
                                          case 18:
                                            if (instance.Message == null)
                                            {
                                              instance.Message = Message.DeserializeLengthDelimited(stream);
                                              continue;
                                            }
                                            Message.DeserializeLengthDelimited(stream, instance.Message);
                                            continue;
                                          case 24:
                                            instance.MessageTimestamp = new ulong?(ProtocolParser.ReadUInt64(stream));
                                            continue;
                                          case 32:
                                            instance.status = new WebMessageInfo.Status?((WebMessageInfo.Status) ProtocolParser.ReadUInt64(stream));
                                            continue;
                                          case 42:
                                            instance.Participant = ProtocolParser.ReadString(stream);
                                            continue;
                                          default:
                                            key = ProtocolParser.ReadKey((byte) firstByte, stream);
                                            switch (key.Field)
                                            {
                                              case 0:
                                                throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                                              case 16:
                                                continue;
                                              case 17:
                                                goto label_18;
                                              case 18:
                                                goto label_20;
                                              case 19:
                                                goto label_22;
                                              case 20:
                                                goto label_24;
                                              case 21:
                                                goto label_26;
                                              case 22:
                                                goto label_28;
                                              case 23:
                                                goto label_30;
                                              case 24:
                                                goto label_32;
                                              case 25:
                                                goto label_34;
                                              case 26:
                                                goto label_36;
                                              case 27:
                                                goto label_38;
                                              case 28:
                                                goto label_40;
                                              case 29:
                                                goto label_42;
                                              case 30:
                                                goto label_46;
                                              case 31:
                                                goto label_50;
                                              default:
                                                goto label_54;
                                            }
                                        }
                                      }
                                      while (key.WireType != Wire.Varint);
                                      instance.Ignore = new bool?(ProtocolParser.ReadBool(stream));
                                      continue;
label_18:;
                                    }
                                    while (key.WireType != Wire.Varint);
                                    instance.Starred = new bool?(ProtocolParser.ReadBool(stream));
                                    continue;
label_20:;
                                  }
                                  while (key.WireType != Wire.Varint);
                                  instance.Broadcast = new bool?(ProtocolParser.ReadBool(stream));
                                  continue;
label_22:;
                                }
                                while (key.WireType != Wire.LengthDelimited);
                                instance.PushName = ProtocolParser.ReadString(stream);
                                continue;
label_24:;
                              }
                              while (key.WireType != Wire.LengthDelimited);
                              instance.MediaCiphertextSha256 = ProtocolParser.ReadBytes(stream);
                              continue;
label_26:;
                            }
                            while (key.WireType != Wire.Varint);
                            instance.Multicast = new bool?(ProtocolParser.ReadBool(stream));
                            continue;
label_28:;
                          }
                          while (key.WireType != Wire.Varint);
                          instance.UrlText = new bool?(ProtocolParser.ReadBool(stream));
                          continue;
label_30:;
                        }
                        while (key.WireType != Wire.Varint);
                        instance.UrlNumber = new bool?(ProtocolParser.ReadBool(stream));
                        continue;
label_32:;
                      }
                      while (key.WireType != Wire.Varint);
                      instance.MessageStubType = new WebMessageInfo.StubType?((WebMessageInfo.StubType) ProtocolParser.ReadUInt64(stream));
                      continue;
label_34:;
                    }
                    while (key.WireType != Wire.Varint);
                    instance.ClearMedia = new bool?(ProtocolParser.ReadBool(stream));
                    continue;
label_36:;
                  }
                  while (key.WireType != Wire.LengthDelimited);
                  instance.MessageStubParameters.Add(ProtocolParser.ReadString(stream));
                  continue;
label_38:;
                }
                while (key.WireType != Wire.Varint);
                instance.Duration = new uint?(ProtocolParser.ReadUInt32(stream));
                continue;
label_40:;
              }
              while (key.WireType != Wire.LengthDelimited);
              instance.Labels.Add(ProtocolParser.ReadString(stream));
              continue;
label_42:;
            }
            while (key.WireType != Wire.LengthDelimited);
            if (instance.PaymentInfo == null)
            {
              instance.PaymentInfo = PaymentInfo.DeserializeLengthDelimited(stream);
              continue;
            }
            PaymentInfo.DeserializeLengthDelimited(stream, instance.PaymentInfo);
            continue;
label_46:;
          }
          while (key.WireType != Wire.LengthDelimited);
          if (instance.FinalLiveLocation == null)
          {
            instance.FinalLiveLocation = Message.LiveLocationMessage.DeserializeLengthDelimited(stream);
            continue;
          }
          Message.LiveLocationMessage.DeserializeLengthDelimited(stream, instance.FinalLiveLocation);
          continue;
label_50:;
        }
        while (key.WireType != Wire.LengthDelimited);
        if (instance.QuotedPaymentInfo == null)
        {
          instance.QuotedPaymentInfo = PaymentInfo.DeserializeLengthDelimited(stream);
          continue;
        }
        PaymentInfo.DeserializeLengthDelimited(stream, instance.QuotedPaymentInfo);
        continue;
label_54:
        ProtocolParser.SkipKey(stream, key);
      }
label_55:
      return instance;
    }

    public static WebMessageInfo DeserializeLengthDelimited(Stream stream, WebMessageInfo instance)
    {
      instance.status = new WebMessageInfo.Status?(WebMessageInfo.Status.PENDING);
      if (instance.MessageStubParameters == null)
        instance.MessageStubParameters = new List<string>();
      if (instance.Labels == null)
        instance.Labels = new List<string>();
      long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
      while (stream.Position < num)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            throw new EndOfStreamException();
          case 10:
            if (instance.Key == null)
            {
              instance.Key = MessageKey.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageKey.DeserializeLengthDelimited(stream, instance.Key);
            continue;
          case 18:
            if (instance.Message == null)
            {
              instance.Message = Message.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.DeserializeLengthDelimited(stream, instance.Message);
            continue;
          case 24:
            instance.MessageTimestamp = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 32:
            instance.status = new WebMessageInfo.Status?((WebMessageInfo.Status) ProtocolParser.ReadUInt64(stream));
            continue;
          case 42:
            instance.Participant = ProtocolParser.ReadString(stream);
            continue;
          default:
            SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
            switch (key.Field)
            {
              case 0:
                throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
              case 16:
                if (key.WireType == Wire.Varint)
                {
                  instance.Ignore = new bool?(ProtocolParser.ReadBool(stream));
                  continue;
                }
                continue;
              case 17:
                if (key.WireType == Wire.Varint)
                {
                  instance.Starred = new bool?(ProtocolParser.ReadBool(stream));
                  continue;
                }
                continue;
              case 18:
                if (key.WireType == Wire.Varint)
                {
                  instance.Broadcast = new bool?(ProtocolParser.ReadBool(stream));
                  continue;
                }
                continue;
              case 19:
                if (key.WireType == Wire.LengthDelimited)
                {
                  instance.PushName = ProtocolParser.ReadString(stream);
                  continue;
                }
                continue;
              case 20:
                if (key.WireType == Wire.LengthDelimited)
                {
                  instance.MediaCiphertextSha256 = ProtocolParser.ReadBytes(stream);
                  continue;
                }
                continue;
              case 21:
                if (key.WireType == Wire.Varint)
                {
                  instance.Multicast = new bool?(ProtocolParser.ReadBool(stream));
                  continue;
                }
                continue;
              case 22:
                if (key.WireType == Wire.Varint)
                {
                  instance.UrlText = new bool?(ProtocolParser.ReadBool(stream));
                  continue;
                }
                continue;
              case 23:
                if (key.WireType == Wire.Varint)
                {
                  instance.UrlNumber = new bool?(ProtocolParser.ReadBool(stream));
                  continue;
                }
                continue;
              case 24:
                if (key.WireType == Wire.Varint)
                {
                  instance.MessageStubType = new WebMessageInfo.StubType?((WebMessageInfo.StubType) ProtocolParser.ReadUInt64(stream));
                  continue;
                }
                continue;
              case 25:
                if (key.WireType == Wire.Varint)
                {
                  instance.ClearMedia = new bool?(ProtocolParser.ReadBool(stream));
                  continue;
                }
                continue;
              case 26:
                if (key.WireType == Wire.LengthDelimited)
                {
                  instance.MessageStubParameters.Add(ProtocolParser.ReadString(stream));
                  continue;
                }
                continue;
              case 27:
                if (key.WireType == Wire.Varint)
                {
                  instance.Duration = new uint?(ProtocolParser.ReadUInt32(stream));
                  continue;
                }
                continue;
              case 28:
                if (key.WireType == Wire.LengthDelimited)
                {
                  instance.Labels.Add(ProtocolParser.ReadString(stream));
                  continue;
                }
                continue;
              case 29:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.PaymentInfo == null)
                  {
                    instance.PaymentInfo = PaymentInfo.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  PaymentInfo.DeserializeLengthDelimited(stream, instance.PaymentInfo);
                  continue;
                }
                continue;
              case 30:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.FinalLiveLocation == null)
                  {
                    instance.FinalLiveLocation = Message.LiveLocationMessage.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.LiveLocationMessage.DeserializeLengthDelimited(stream, instance.FinalLiveLocation);
                  continue;
                }
                continue;
              case 31:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.QuotedPaymentInfo == null)
                  {
                    instance.QuotedPaymentInfo = PaymentInfo.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  PaymentInfo.DeserializeLengthDelimited(stream, instance.QuotedPaymentInfo);
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

    public static WebMessageInfo DeserializeLength(
      Stream stream,
      int length,
      WebMessageInfo instance)
    {
      instance.status = new WebMessageInfo.Status?(WebMessageInfo.Status.PENDING);
      if (instance.MessageStubParameters == null)
        instance.MessageStubParameters = new List<string>();
      if (instance.Labels == null)
        instance.Labels = new List<string>();
      long num = stream.Position + (long) length;
      while (stream.Position < num)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            throw new EndOfStreamException();
          case 10:
            if (instance.Key == null)
            {
              instance.Key = MessageKey.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageKey.DeserializeLengthDelimited(stream, instance.Key);
            continue;
          case 18:
            if (instance.Message == null)
            {
              instance.Message = Message.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.DeserializeLengthDelimited(stream, instance.Message);
            continue;
          case 24:
            instance.MessageTimestamp = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 32:
            instance.status = new WebMessageInfo.Status?((WebMessageInfo.Status) ProtocolParser.ReadUInt64(stream));
            continue;
          case 42:
            instance.Participant = ProtocolParser.ReadString(stream);
            continue;
          default:
            SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
            switch (key.Field)
            {
              case 0:
                throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
              case 16:
                if (key.WireType == Wire.Varint)
                {
                  instance.Ignore = new bool?(ProtocolParser.ReadBool(stream));
                  continue;
                }
                continue;
              case 17:
                if (key.WireType == Wire.Varint)
                {
                  instance.Starred = new bool?(ProtocolParser.ReadBool(stream));
                  continue;
                }
                continue;
              case 18:
                if (key.WireType == Wire.Varint)
                {
                  instance.Broadcast = new bool?(ProtocolParser.ReadBool(stream));
                  continue;
                }
                continue;
              case 19:
                if (key.WireType == Wire.LengthDelimited)
                {
                  instance.PushName = ProtocolParser.ReadString(stream);
                  continue;
                }
                continue;
              case 20:
                if (key.WireType == Wire.LengthDelimited)
                {
                  instance.MediaCiphertextSha256 = ProtocolParser.ReadBytes(stream);
                  continue;
                }
                continue;
              case 21:
                if (key.WireType == Wire.Varint)
                {
                  instance.Multicast = new bool?(ProtocolParser.ReadBool(stream));
                  continue;
                }
                continue;
              case 22:
                if (key.WireType == Wire.Varint)
                {
                  instance.UrlText = new bool?(ProtocolParser.ReadBool(stream));
                  continue;
                }
                continue;
              case 23:
                if (key.WireType == Wire.Varint)
                {
                  instance.UrlNumber = new bool?(ProtocolParser.ReadBool(stream));
                  continue;
                }
                continue;
              case 24:
                if (key.WireType == Wire.Varint)
                {
                  instance.MessageStubType = new WebMessageInfo.StubType?((WebMessageInfo.StubType) ProtocolParser.ReadUInt64(stream));
                  continue;
                }
                continue;
              case 25:
                if (key.WireType == Wire.Varint)
                {
                  instance.ClearMedia = new bool?(ProtocolParser.ReadBool(stream));
                  continue;
                }
                continue;
              case 26:
                if (key.WireType == Wire.LengthDelimited)
                {
                  instance.MessageStubParameters.Add(ProtocolParser.ReadString(stream));
                  continue;
                }
                continue;
              case 27:
                if (key.WireType == Wire.Varint)
                {
                  instance.Duration = new uint?(ProtocolParser.ReadUInt32(stream));
                  continue;
                }
                continue;
              case 28:
                if (key.WireType == Wire.LengthDelimited)
                {
                  instance.Labels.Add(ProtocolParser.ReadString(stream));
                  continue;
                }
                continue;
              case 29:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.PaymentInfo == null)
                  {
                    instance.PaymentInfo = PaymentInfo.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  PaymentInfo.DeserializeLengthDelimited(stream, instance.PaymentInfo);
                  continue;
                }
                continue;
              case 30:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.FinalLiveLocation == null)
                  {
                    instance.FinalLiveLocation = Message.LiveLocationMessage.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.LiveLocationMessage.DeserializeLengthDelimited(stream, instance.FinalLiveLocation);
                  continue;
                }
                continue;
              case 31:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.QuotedPaymentInfo == null)
                  {
                    instance.QuotedPaymentInfo = PaymentInfo.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  PaymentInfo.DeserializeLengthDelimited(stream, instance.QuotedPaymentInfo);
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

    public static void Serialize(
      MemoryStream stream,
      WebMessageInfo instance,
      bool oversizedMessage)
    {
      long position = stream.Position;
      long length1 = stream.Length;
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.Key == null)
        throw new ProtocolBufferException("Key is required by the proto specification.");
      stream.WriteByte((byte) 10);
      stream1.SetLength(0L);
      MessageKey.Serialize((Stream) stream1, instance.Key);
      uint length2 = (uint) stream1.Length;
      ProtocolParser.WriteUInt32((Stream) stream, length2);
      stream1.WriteTo((Stream) stream);
      if (instance.Message != null && !oversizedMessage)
      {
        stream.WriteByte((byte) 18);
        stream1.SetLength(0L);
        Message.Serialize((Stream) stream1, instance.Message);
        uint length3 = (uint) stream1.Length;
        ProtocolParser.WriteUInt32((Stream) stream, length3);
        stream1.WriteTo((Stream) stream);
      }
      if (instance.MessageTimestamp.HasValue)
      {
        stream.WriteByte((byte) 24);
        ProtocolParser.WriteUInt64((Stream) stream, instance.MessageTimestamp.Value);
      }
      if (instance.status.HasValue)
      {
        stream.WriteByte((byte) 32);
        ProtocolParser.WriteUInt64((Stream) stream, (ulong) instance.status.Value);
      }
      if (instance.Participant != null)
      {
        stream.WriteByte((byte) 42);
        ProtocolParser.WriteBytes((Stream) stream, Encoding.UTF8.GetBytes(instance.Participant));
      }
      if (instance.Ignore.HasValue)
      {
        stream.WriteByte((byte) 128);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteBool((Stream) stream, instance.Ignore.Value);
      }
      if (instance.Starred.HasValue)
      {
        stream.WriteByte((byte) 136);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteBool((Stream) stream, instance.Starred.Value);
      }
      if (instance.Broadcast.HasValue)
      {
        stream.WriteByte((byte) 144);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteBool((Stream) stream, instance.Broadcast.Value);
      }
      if (instance.PushName != null)
      {
        stream.WriteByte((byte) 154);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteBytes((Stream) stream, Encoding.UTF8.GetBytes(instance.PushName));
      }
      if (instance.MediaCiphertextSha256 != null)
      {
        stream.WriteByte((byte) 162);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteBytes((Stream) stream, instance.MediaCiphertextSha256);
      }
      if (instance.Multicast.HasValue)
      {
        stream.WriteByte((byte) 168);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteBool((Stream) stream, instance.Multicast.Value);
      }
      if (instance.UrlText.HasValue)
      {
        stream.WriteByte((byte) 176);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteBool((Stream) stream, instance.UrlText.Value);
      }
      if (instance.UrlNumber.HasValue)
      {
        stream.WriteByte((byte) 184);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteBool((Stream) stream, instance.UrlNumber.Value);
      }
      if (instance.MessageStubType.HasValue | oversizedMessage)
      {
        ulong val = oversizedMessage ? 68UL : (ulong) instance.MessageStubType.Value;
        stream.WriteByte((byte) 192);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteUInt64((Stream) stream, val);
      }
      if (instance.ClearMedia.HasValue)
      {
        stream.WriteByte((byte) 200);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteBool((Stream) stream, instance.ClearMedia.Value);
      }
      if (instance.MessageStubParameters != null)
      {
        foreach (string messageStubParameter in instance.MessageStubParameters)
        {
          stream.WriteByte((byte) 210);
          stream.WriteByte((byte) 1);
          ProtocolParser.WriteBytes((Stream) stream, Encoding.UTF8.GetBytes(messageStubParameter));
        }
      }
      if (instance.Duration.HasValue)
      {
        stream.WriteByte((byte) 216);
        stream.WriteByte((byte) 1);
        ProtocolParser.WriteUInt32((Stream) stream, instance.Duration.Value);
      }
      if (instance.Labels != null)
      {
        foreach (string label in instance.Labels)
        {
          stream.WriteByte((byte) 226);
          stream.WriteByte((byte) 1);
          ProtocolParser.WriteBytes((Stream) stream, Encoding.UTF8.GetBytes(label));
        }
      }
      if (instance.PaymentInfo != null)
      {
        stream.WriteByte((byte) 234);
        stream.WriteByte((byte) 1);
        stream1.SetLength(0L);
        PaymentInfo.Serialize((Stream) stream1, instance.PaymentInfo);
        uint length4 = (uint) stream1.Length;
        ProtocolParser.WriteUInt32((Stream) stream, length4);
        stream1.WriteTo((Stream) stream);
      }
      if (instance.FinalLiveLocation != null)
      {
        stream.WriteByte((byte) 242);
        stream.WriteByte((byte) 1);
        stream1.SetLength(0L);
        Message.LiveLocationMessage.Serialize((Stream) stream1, instance.FinalLiveLocation);
        uint length5 = (uint) stream1.Length;
        ProtocolParser.WriteUInt32((Stream) stream, length5);
        stream1.WriteTo((Stream) stream);
      }
      if (instance.QuotedPaymentInfo != null)
      {
        stream.WriteByte((byte) 250);
        stream.WriteByte((byte) 1);
        stream1.SetLength(0L);
        PaymentInfo.Serialize((Stream) stream1, instance.QuotedPaymentInfo);
        uint length6 = (uint) stream1.Length;
        ProtocolParser.WriteUInt32((Stream) stream, length6);
        stream1.WriteTo((Stream) stream);
      }
      ProtocolParser.Stack.Push(stream1);
      long num = stream.Length - length1;
      if (oversizedMessage)
      {
        Log.d("e2e", "Suppressed over length message, now {0} bytes", (object) num);
      }
      else
      {
        if (num <= (long) (Settings.WebMsgMaxSizeKb * 1024) || Settings.WebMsgMaxSizeKb <= 0)
          return;
        Log.d("e2e", "Suppressing over length message, was {0} bytes", (object) num);
        stream.Position = position;
        stream.SetLength(length1);
        WebMessageInfo.Serialize(stream, instance, true);
      }
    }

    public static byte[] SerializeToBytes(WebMessageInfo instance)
    {
      using (MemoryStream stream = new MemoryStream())
      {
        WebMessageInfo.Serialize(stream, instance, false);
        return stream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, WebMessageInfo instance)
    {
      byte[] bytes = WebMessageInfo.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }

    public enum Status
    {
      ERROR,
      PENDING,
      SERVER_ACK,
      DELIVERY_ACK,
      READ,
      PLAYED,
    }

    public enum StubType
    {
      UNKNOWN,
      REVOKE,
      CIPHERTEXT,
      FUTUREPROOF,
      NON_VERIFIED_TRANSITION,
      UNVERIFIED_TRANSITION,
      VERIFIED_TRANSITION,
      VERIFIED_LOW_UNKNOWN,
      VERIFIED_HIGH,
      VERIFIED_INITIAL_UNKNOWN,
      VERIFIED_INITIAL_LOW,
      VERIFIED_INITIAL_HIGH,
      VERIFIED_TRANSITION_ANY_TO_NONE,
      VERIFIED_TRANSITION_ANY_TO_HIGH,
      VERIFIED_TRANSITION_HIGH_TO_LOW,
      VERIFIED_TRANSITION_HIGH_TO_UNKNOWN,
      VERIFIED_TRANSITION_UNKNOWN_TO_LOW,
      VERIFIED_TRANSITION_LOW_TO_UNKNOWN,
      VERIFIED_TRANSITION_NONE_TO_LOW,
      VERIFIED_TRANSITION_NONE_TO_UNKNOWN,
      GROUP_CREATE,
      GROUP_CHANGE_SUBJECT,
      GROUP_CHANGE_ICON,
      GROUP_CHANGE_INVITE_LINK,
      GROUP_CHANGE_DESCRIPTION,
      GROUP_CHANGE_RESTRICT,
      GROUP_CHANGE_ANNOUNCE,
      GROUP_PARTICIPANT_ADD,
      GROUP_PARTICIPANT_REMOVE,
      GROUP_PARTICIPANT_PROMOTE,
      GROUP_PARTICIPANT_DEMOTE,
      GROUP_PARTICIPANT_INVITE,
      GROUP_PARTICIPANT_LEAVE,
      GROUP_PARTICIPANT_CHANGE_NUMBER,
      BROADCAST_CREATE,
      BROADCAST_ADD,
      BROADCAST_REMOVE,
      GENERIC_NOTIFICATION,
      E2E_IDENTITY_CHANGED,
      E2E_ENCRYPTED,
      CALL_MISSED_VOICE,
      CALL_MISSED_VIDEO,
      INDIVIDUAL_CHANGE_NUMBER,
      GROUP_DELETE,
      GROUP_ANNOUNCE_MODE_MESSAGE_BOUNCE,
      CALL_MISSED_GROUP_VOICE,
      CALL_MISSED_GROUP_VIDEO,
      PAYMENT_CIPHERTEXT,
      PAYMENT_FUTUREPROOF,
      PAYMENT_TRANSACTION_STATUS_UPDATE_FAILED,
      PAYMENT_TRANSACTION_STATUS_UPDATE_REFUNDED,
      PAYMENT_TRANSACTION_STATUS_UPDATE_REFUND_FAILED,
      PAYMENT_TRANSACTION_STATUS_RECEIVER_PENDING_SETUP,
      PAYMENT_TRANSACTION_STATUS_RECEIVER_SUCCESS_AFTER_HICCUP,
      PAYMENT_ACTION_ACCOUNT_SETUP_REMINDER,
      PAYMENT_ACTION_SEND_PAYMENT_REMINDER,
      PAYMENT_ACTION_SEND_PAYMENT_INVITATION,
      PAYMENT_ACTION_REQUEST_DECLINED,
      PAYMENT_ACTION_REQUEST_EXPIRED,
      PAYMENT_ACTION_REQUEST_CANCELLED,
      BIZ_VERIFIED_TRANSITION_TOP_TO_BOTTOM,
      BIZ_VERIFIED_TRANSITION_BOTTOM_TO_TOP,
      BIZ_INTRO_TOP,
      BIZ_INTRO_BOTTOM,
      BIZ_NAME_CHANGE,
      BIZ_MOVE_TO_CONSUMER_APP,
      BIZ_TWO_TIER_MIGRATION_TOP,
      BIZ_TWO_TIER_MIGRATION_BOTTOM,
      OVERSIZED,
    }
  }
}
