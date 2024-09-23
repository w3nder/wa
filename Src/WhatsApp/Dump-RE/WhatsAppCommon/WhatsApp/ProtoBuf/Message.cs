// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.Message
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#nullable disable
namespace WhatsApp.ProtoBuf
{
  public class Message
  {
    public List<SilentOrbit.ProtocolBuffers.KeyValue> PreservedFields;
    public static readonly string[] SupportedAudioTypes = new string[5]
    {
      "audio/aac",
      "audio/mp4",
      "audio/amr",
      "audio/mpeg",
      "audio/ogg; codecs=opus"
    };
    public static readonly string[] SupportedVideoTypes = new string[2]
    {
      "video/mp4",
      "video/3gpp"
    };
    public static readonly string[] SupportedStickerTypes = new string[2]
    {
      "image/png",
      "image/webp"
    };
    public byte[] Serialized;
    private byte[] unknownSerialized;

    public string Conversation { get; set; }

    public Message.SenderKeyDistributionMessage SenderKeyDistributionMessageField { get; set; }

    public Message.ImageMessage ImageMessageField { get; set; }

    public Message.ContactMessage ContactMessageField { get; set; }

    public Message.LocationMessage LocationMessageField { get; set; }

    public Message.ExtendedTextMessage ExtendedTextMessageField { get; set; }

    public Message.DocumentMessage DocumentMessageField { get; set; }

    public Message.AudioMessage AudioMessageField { get; set; }

    public Message.VideoMessage VideoMessageField { get; set; }

    public Message.Call CallField { get; set; }

    public Message.Chat ChatField { get; set; }

    public Message.ProtocolMessage ProtocolMessageField { get; set; }

    public Message.ContactsArrayMessage ContactsArrayMessageField { get; set; }

    public Message.HighlyStructuredMessage HighlyStructuredMessageField { get; set; }

    public Message.SenderKeyDistributionMessage FastRatchetKeySenderKeyDistributionMessage { get; set; }

    public Message.SendPaymentMessage SendPaymentMessageField { get; set; }

    public Message.LiveLocationMessage LiveLocationMessageField { get; set; }

    public Message.RequestPaymentMessage RequestPaymentMessageField { get; set; }

    public Message.DeclinePaymentRequestMessage DeclinePaymentRequestMessageField { get; set; }

    public Message.CancelPaymentRequestMessage CancelPaymentRequestMessageField { get; set; }

    public Message.TemplateMessage TemplateMessageField { get; set; }

    public Message.StickerMessage StickerMessageField { get; set; }

    public static Message Deserialize(Stream stream)
    {
      Message instance = new Message();
      Message.Deserialize(stream, instance);
      return instance;
    }

    public static Message DeserializeLengthDelimited(Stream stream)
    {
      Message instance = new Message();
      Message.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static Message DeserializeLength(Stream stream, int length)
    {
      Message instance = new Message();
      Message.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static Message Deserialize(byte[] buffer)
    {
      Message instance = new Message();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        Message.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static Message Deserialize(byte[] buffer, Message instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        Message.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static Message Deserialize(Stream stream, Message instance)
    {
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
                      int firstByte = stream.ReadByte();
                      switch (firstByte)
                      {
                        case -1:
                          goto label_77;
                        case 10:
                          instance.Conversation = ProtocolParser.ReadString(stream);
                          continue;
                        case 18:
                          if (instance.SenderKeyDistributionMessageField == null)
                          {
                            instance.SenderKeyDistributionMessageField = Message.SenderKeyDistributionMessage.DeserializeLengthDelimited(stream);
                            continue;
                          }
                          Message.SenderKeyDistributionMessage.DeserializeLengthDelimited(stream, instance.SenderKeyDistributionMessageField);
                          continue;
                        case 26:
                          if (instance.ImageMessageField == null)
                          {
                            instance.ImageMessageField = Message.ImageMessage.DeserializeLengthDelimited(stream);
                            continue;
                          }
                          Message.ImageMessage.DeserializeLengthDelimited(stream, instance.ImageMessageField);
                          continue;
                        case 34:
                          if (instance.ContactMessageField == null)
                          {
                            instance.ContactMessageField = Message.ContactMessage.DeserializeLengthDelimited(stream);
                            continue;
                          }
                          Message.ContactMessage.DeserializeLengthDelimited(stream, instance.ContactMessageField);
                          continue;
                        case 42:
                          if (instance.LocationMessageField == null)
                          {
                            instance.LocationMessageField = Message.LocationMessage.DeserializeLengthDelimited(stream);
                            continue;
                          }
                          Message.LocationMessage.DeserializeLengthDelimited(stream, instance.LocationMessageField);
                          continue;
                        case 50:
                          if (instance.ExtendedTextMessageField == null)
                          {
                            instance.ExtendedTextMessageField = Message.ExtendedTextMessage.DeserializeLengthDelimited(stream);
                            continue;
                          }
                          Message.ExtendedTextMessage.DeserializeLengthDelimited(stream, instance.ExtendedTextMessageField);
                          continue;
                        case 58:
                          if (instance.DocumentMessageField == null)
                          {
                            instance.DocumentMessageField = Message.DocumentMessage.DeserializeLengthDelimited(stream);
                            continue;
                          }
                          Message.DocumentMessage.DeserializeLengthDelimited(stream, instance.DocumentMessageField);
                          continue;
                        case 66:
                          if (instance.AudioMessageField == null)
                          {
                            instance.AudioMessageField = Message.AudioMessage.DeserializeLengthDelimited(stream);
                            continue;
                          }
                          Message.AudioMessage.DeserializeLengthDelimited(stream, instance.AudioMessageField);
                          continue;
                        case 74:
                          if (instance.VideoMessageField == null)
                          {
                            instance.VideoMessageField = Message.VideoMessage.DeserializeLengthDelimited(stream);
                            continue;
                          }
                          Message.VideoMessage.DeserializeLengthDelimited(stream, instance.VideoMessageField);
                          continue;
                        case 82:
                          if (instance.CallField == null)
                          {
                            instance.CallField = Message.Call.DeserializeLengthDelimited(stream);
                            continue;
                          }
                          Message.Call.DeserializeLengthDelimited(stream, instance.CallField);
                          continue;
                        case 90:
                          if (instance.ChatField == null)
                          {
                            instance.ChatField = Message.Chat.DeserializeLengthDelimited(stream);
                            continue;
                          }
                          Message.Chat.DeserializeLengthDelimited(stream, instance.ChatField);
                          continue;
                        case 98:
                          if (instance.ProtocolMessageField == null)
                          {
                            instance.ProtocolMessageField = Message.ProtocolMessage.DeserializeLengthDelimited(stream);
                            continue;
                          }
                          Message.ProtocolMessage.DeserializeLengthDelimited(stream, instance.ProtocolMessageField);
                          continue;
                        case 106:
                          if (instance.ContactsArrayMessageField == null)
                          {
                            instance.ContactsArrayMessageField = Message.ContactsArrayMessage.DeserializeLengthDelimited(stream);
                            continue;
                          }
                          Message.ContactsArrayMessage.DeserializeLengthDelimited(stream, instance.ContactsArrayMessageField);
                          continue;
                        case 114:
                          if (instance.HighlyStructuredMessageField == null)
                          {
                            instance.HighlyStructuredMessageField = Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream);
                            continue;
                          }
                          Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream, instance.HighlyStructuredMessageField);
                          continue;
                        case 122:
                          if (instance.FastRatchetKeySenderKeyDistributionMessage == null)
                          {
                            instance.FastRatchetKeySenderKeyDistributionMessage = Message.SenderKeyDistributionMessage.DeserializeLengthDelimited(stream);
                            continue;
                          }
                          Message.SenderKeyDistributionMessage.DeserializeLengthDelimited(stream, instance.FastRatchetKeySenderKeyDistributionMessage);
                          continue;
                        default:
                          key = ProtocolParser.ReadKey((byte) firstByte, stream);
                          switch (key.Field)
                          {
                            case 0:
                              throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                            case 16:
                              continue;
                            case 18:
                              goto label_50;
                            case 22:
                              goto label_54;
                            case 23:
                              goto label_58;
                            case 24:
                              goto label_62;
                            case 25:
                              goto label_66;
                            case 26:
                              goto label_70;
                            default:
                              goto label_74;
                          }
                      }
                    }
                    while (key.WireType != Wire.LengthDelimited);
                    if (instance.SendPaymentMessageField == null)
                    {
                      instance.SendPaymentMessageField = Message.SendPaymentMessage.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    Message.SendPaymentMessage.DeserializeLengthDelimited(stream, instance.SendPaymentMessageField);
                    continue;
label_50:;
                  }
                  while (key.WireType != Wire.LengthDelimited);
                  if (instance.LiveLocationMessageField == null)
                  {
                    instance.LiveLocationMessageField = Message.LiveLocationMessage.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.LiveLocationMessage.DeserializeLengthDelimited(stream, instance.LiveLocationMessageField);
                  continue;
label_54:;
                }
                while (key.WireType != Wire.LengthDelimited);
                if (instance.RequestPaymentMessageField == null)
                {
                  instance.RequestPaymentMessageField = Message.RequestPaymentMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.RequestPaymentMessage.DeserializeLengthDelimited(stream, instance.RequestPaymentMessageField);
                continue;
label_58:;
              }
              while (key.WireType != Wire.LengthDelimited);
              if (instance.DeclinePaymentRequestMessageField == null)
              {
                instance.DeclinePaymentRequestMessageField = Message.DeclinePaymentRequestMessage.DeserializeLengthDelimited(stream);
                continue;
              }
              Message.DeclinePaymentRequestMessage.DeserializeLengthDelimited(stream, instance.DeclinePaymentRequestMessageField);
              continue;
label_62:;
            }
            while (key.WireType != Wire.LengthDelimited);
            if (instance.CancelPaymentRequestMessageField == null)
            {
              instance.CancelPaymentRequestMessageField = Message.CancelPaymentRequestMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.CancelPaymentRequestMessage.DeserializeLengthDelimited(stream, instance.CancelPaymentRequestMessageField);
            continue;
label_66:;
          }
          while (key.WireType != Wire.LengthDelimited);
          if (instance.TemplateMessageField == null)
          {
            instance.TemplateMessageField = Message.TemplateMessage.DeserializeLengthDelimited(stream);
            continue;
          }
          Message.TemplateMessage.DeserializeLengthDelimited(stream, instance.TemplateMessageField);
          continue;
label_70:;
        }
        while (key.WireType != Wire.LengthDelimited);
        if (instance.StickerMessageField == null)
        {
          instance.StickerMessageField = Message.StickerMessage.DeserializeLengthDelimited(stream);
          continue;
        }
        Message.StickerMessage.DeserializeLengthDelimited(stream, instance.StickerMessageField);
        continue;
label_74:
        if (instance.PreservedFields == null)
          instance.PreservedFields = new List<SilentOrbit.ProtocolBuffers.KeyValue>();
        instance.PreservedFields.Add(new SilentOrbit.ProtocolBuffers.KeyValue(key, ProtocolParser.ReadValueBytes(stream, key)));
      }
label_77:
      return instance;
    }

    public static Message DeserializeLengthDelimited(Stream stream, Message instance)
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
            instance.Conversation = ProtocolParser.ReadString(stream);
            continue;
          case 18:
            if (instance.SenderKeyDistributionMessageField == null)
            {
              instance.SenderKeyDistributionMessageField = Message.SenderKeyDistributionMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.SenderKeyDistributionMessage.DeserializeLengthDelimited(stream, instance.SenderKeyDistributionMessageField);
            continue;
          case 26:
            if (instance.ImageMessageField == null)
            {
              instance.ImageMessageField = Message.ImageMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.ImageMessage.DeserializeLengthDelimited(stream, instance.ImageMessageField);
            continue;
          case 34:
            if (instance.ContactMessageField == null)
            {
              instance.ContactMessageField = Message.ContactMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.ContactMessage.DeserializeLengthDelimited(stream, instance.ContactMessageField);
            continue;
          case 42:
            if (instance.LocationMessageField == null)
            {
              instance.LocationMessageField = Message.LocationMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.LocationMessage.DeserializeLengthDelimited(stream, instance.LocationMessageField);
            continue;
          case 50:
            if (instance.ExtendedTextMessageField == null)
            {
              instance.ExtendedTextMessageField = Message.ExtendedTextMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.ExtendedTextMessage.DeserializeLengthDelimited(stream, instance.ExtendedTextMessageField);
            continue;
          case 58:
            if (instance.DocumentMessageField == null)
            {
              instance.DocumentMessageField = Message.DocumentMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.DocumentMessage.DeserializeLengthDelimited(stream, instance.DocumentMessageField);
            continue;
          case 66:
            if (instance.AudioMessageField == null)
            {
              instance.AudioMessageField = Message.AudioMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.AudioMessage.DeserializeLengthDelimited(stream, instance.AudioMessageField);
            continue;
          case 74:
            if (instance.VideoMessageField == null)
            {
              instance.VideoMessageField = Message.VideoMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.VideoMessage.DeserializeLengthDelimited(stream, instance.VideoMessageField);
            continue;
          case 82:
            if (instance.CallField == null)
            {
              instance.CallField = Message.Call.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.Call.DeserializeLengthDelimited(stream, instance.CallField);
            continue;
          case 90:
            if (instance.ChatField == null)
            {
              instance.ChatField = Message.Chat.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.Chat.DeserializeLengthDelimited(stream, instance.ChatField);
            continue;
          case 98:
            if (instance.ProtocolMessageField == null)
            {
              instance.ProtocolMessageField = Message.ProtocolMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.ProtocolMessage.DeserializeLengthDelimited(stream, instance.ProtocolMessageField);
            continue;
          case 106:
            if (instance.ContactsArrayMessageField == null)
            {
              instance.ContactsArrayMessageField = Message.ContactsArrayMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.ContactsArrayMessage.DeserializeLengthDelimited(stream, instance.ContactsArrayMessageField);
            continue;
          case 114:
            if (instance.HighlyStructuredMessageField == null)
            {
              instance.HighlyStructuredMessageField = Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream, instance.HighlyStructuredMessageField);
            continue;
          case 122:
            if (instance.FastRatchetKeySenderKeyDistributionMessage == null)
            {
              instance.FastRatchetKeySenderKeyDistributionMessage = Message.SenderKeyDistributionMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.SenderKeyDistributionMessage.DeserializeLengthDelimited(stream, instance.FastRatchetKeySenderKeyDistributionMessage);
            continue;
          default:
            SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
            switch (key.Field)
            {
              case 0:
                throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
              case 16:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.SendPaymentMessageField == null)
                  {
                    instance.SendPaymentMessageField = Message.SendPaymentMessage.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.SendPaymentMessage.DeserializeLengthDelimited(stream, instance.SendPaymentMessageField);
                  continue;
                }
                continue;
              case 18:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.LiveLocationMessageField == null)
                  {
                    instance.LiveLocationMessageField = Message.LiveLocationMessage.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.LiveLocationMessage.DeserializeLengthDelimited(stream, instance.LiveLocationMessageField);
                  continue;
                }
                continue;
              case 22:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.RequestPaymentMessageField == null)
                  {
                    instance.RequestPaymentMessageField = Message.RequestPaymentMessage.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.RequestPaymentMessage.DeserializeLengthDelimited(stream, instance.RequestPaymentMessageField);
                  continue;
                }
                continue;
              case 23:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.DeclinePaymentRequestMessageField == null)
                  {
                    instance.DeclinePaymentRequestMessageField = Message.DeclinePaymentRequestMessage.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.DeclinePaymentRequestMessage.DeserializeLengthDelimited(stream, instance.DeclinePaymentRequestMessageField);
                  continue;
                }
                continue;
              case 24:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.CancelPaymentRequestMessageField == null)
                  {
                    instance.CancelPaymentRequestMessageField = Message.CancelPaymentRequestMessage.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.CancelPaymentRequestMessage.DeserializeLengthDelimited(stream, instance.CancelPaymentRequestMessageField);
                  continue;
                }
                continue;
              case 25:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.TemplateMessageField == null)
                  {
                    instance.TemplateMessageField = Message.TemplateMessage.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.TemplateMessage.DeserializeLengthDelimited(stream, instance.TemplateMessageField);
                  continue;
                }
                continue;
              case 26:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.StickerMessageField == null)
                  {
                    instance.StickerMessageField = Message.StickerMessage.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.StickerMessage.DeserializeLengthDelimited(stream, instance.StickerMessageField);
                  continue;
                }
                continue;
              default:
                if (instance.PreservedFields == null)
                  instance.PreservedFields = new List<SilentOrbit.ProtocolBuffers.KeyValue>();
                instance.PreservedFields.Add(new SilentOrbit.ProtocolBuffers.KeyValue(key, ProtocolParser.ReadValueBytes(stream, key)));
                continue;
            }
        }
      }
      if (stream.Position != num)
        throw new ProtocolBufferException("Read past max limit");
      return instance;
    }

    public static Message DeserializeLength(Stream stream, int length, Message instance)
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
            instance.Conversation = ProtocolParser.ReadString(stream);
            continue;
          case 18:
            if (instance.SenderKeyDistributionMessageField == null)
            {
              instance.SenderKeyDistributionMessageField = Message.SenderKeyDistributionMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.SenderKeyDistributionMessage.DeserializeLengthDelimited(stream, instance.SenderKeyDistributionMessageField);
            continue;
          case 26:
            if (instance.ImageMessageField == null)
            {
              instance.ImageMessageField = Message.ImageMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.ImageMessage.DeserializeLengthDelimited(stream, instance.ImageMessageField);
            continue;
          case 34:
            if (instance.ContactMessageField == null)
            {
              instance.ContactMessageField = Message.ContactMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.ContactMessage.DeserializeLengthDelimited(stream, instance.ContactMessageField);
            continue;
          case 42:
            if (instance.LocationMessageField == null)
            {
              instance.LocationMessageField = Message.LocationMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.LocationMessage.DeserializeLengthDelimited(stream, instance.LocationMessageField);
            continue;
          case 50:
            if (instance.ExtendedTextMessageField == null)
            {
              instance.ExtendedTextMessageField = Message.ExtendedTextMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.ExtendedTextMessage.DeserializeLengthDelimited(stream, instance.ExtendedTextMessageField);
            continue;
          case 58:
            if (instance.DocumentMessageField == null)
            {
              instance.DocumentMessageField = Message.DocumentMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.DocumentMessage.DeserializeLengthDelimited(stream, instance.DocumentMessageField);
            continue;
          case 66:
            if (instance.AudioMessageField == null)
            {
              instance.AudioMessageField = Message.AudioMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.AudioMessage.DeserializeLengthDelimited(stream, instance.AudioMessageField);
            continue;
          case 74:
            if (instance.VideoMessageField == null)
            {
              instance.VideoMessageField = Message.VideoMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.VideoMessage.DeserializeLengthDelimited(stream, instance.VideoMessageField);
            continue;
          case 82:
            if (instance.CallField == null)
            {
              instance.CallField = Message.Call.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.Call.DeserializeLengthDelimited(stream, instance.CallField);
            continue;
          case 90:
            if (instance.ChatField == null)
            {
              instance.ChatField = Message.Chat.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.Chat.DeserializeLengthDelimited(stream, instance.ChatField);
            continue;
          case 98:
            if (instance.ProtocolMessageField == null)
            {
              instance.ProtocolMessageField = Message.ProtocolMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.ProtocolMessage.DeserializeLengthDelimited(stream, instance.ProtocolMessageField);
            continue;
          case 106:
            if (instance.ContactsArrayMessageField == null)
            {
              instance.ContactsArrayMessageField = Message.ContactsArrayMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.ContactsArrayMessage.DeserializeLengthDelimited(stream, instance.ContactsArrayMessageField);
            continue;
          case 114:
            if (instance.HighlyStructuredMessageField == null)
            {
              instance.HighlyStructuredMessageField = Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream, instance.HighlyStructuredMessageField);
            continue;
          case 122:
            if (instance.FastRatchetKeySenderKeyDistributionMessage == null)
            {
              instance.FastRatchetKeySenderKeyDistributionMessage = Message.SenderKeyDistributionMessage.DeserializeLengthDelimited(stream);
              continue;
            }
            Message.SenderKeyDistributionMessage.DeserializeLengthDelimited(stream, instance.FastRatchetKeySenderKeyDistributionMessage);
            continue;
          default:
            SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
            switch (key.Field)
            {
              case 0:
                throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
              case 16:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.SendPaymentMessageField == null)
                  {
                    instance.SendPaymentMessageField = Message.SendPaymentMessage.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.SendPaymentMessage.DeserializeLengthDelimited(stream, instance.SendPaymentMessageField);
                  continue;
                }
                continue;
              case 18:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.LiveLocationMessageField == null)
                  {
                    instance.LiveLocationMessageField = Message.LiveLocationMessage.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.LiveLocationMessage.DeserializeLengthDelimited(stream, instance.LiveLocationMessageField);
                  continue;
                }
                continue;
              case 22:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.RequestPaymentMessageField == null)
                  {
                    instance.RequestPaymentMessageField = Message.RequestPaymentMessage.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.RequestPaymentMessage.DeserializeLengthDelimited(stream, instance.RequestPaymentMessageField);
                  continue;
                }
                continue;
              case 23:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.DeclinePaymentRequestMessageField == null)
                  {
                    instance.DeclinePaymentRequestMessageField = Message.DeclinePaymentRequestMessage.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.DeclinePaymentRequestMessage.DeserializeLengthDelimited(stream, instance.DeclinePaymentRequestMessageField);
                  continue;
                }
                continue;
              case 24:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.CancelPaymentRequestMessageField == null)
                  {
                    instance.CancelPaymentRequestMessageField = Message.CancelPaymentRequestMessage.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.CancelPaymentRequestMessage.DeserializeLengthDelimited(stream, instance.CancelPaymentRequestMessageField);
                  continue;
                }
                continue;
              case 25:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.TemplateMessageField == null)
                  {
                    instance.TemplateMessageField = Message.TemplateMessage.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.TemplateMessage.DeserializeLengthDelimited(stream, instance.TemplateMessageField);
                  continue;
                }
                continue;
              case 26:
                if (key.WireType == Wire.LengthDelimited)
                {
                  if (instance.StickerMessageField == null)
                  {
                    instance.StickerMessageField = Message.StickerMessage.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.StickerMessage.DeserializeLengthDelimited(stream, instance.StickerMessageField);
                  continue;
                }
                continue;
              default:
                if (instance.PreservedFields == null)
                  instance.PreservedFields = new List<SilentOrbit.ProtocolBuffers.KeyValue>();
                instance.PreservedFields.Add(new SilentOrbit.ProtocolBuffers.KeyValue(key, ProtocolParser.ReadValueBytes(stream, key)));
                continue;
            }
        }
      }
      if (stream.Position != num)
        throw new ProtocolBufferException("Read past max limit");
      return instance;
    }

    public static void Serialize(Stream stream, Message instance)
    {
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.Conversation != null)
      {
        stream.WriteByte((byte) 10);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Conversation));
      }
      if (instance.SenderKeyDistributionMessageField != null)
      {
        stream.WriteByte((byte) 18);
        stream1.SetLength(0L);
        Message.SenderKeyDistributionMessage.Serialize((Stream) stream1, instance.SenderKeyDistributionMessageField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.ImageMessageField != null)
      {
        stream.WriteByte((byte) 26);
        stream1.SetLength(0L);
        Message.ImageMessage.Serialize((Stream) stream1, instance.ImageMessageField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.ContactMessageField != null)
      {
        stream.WriteByte((byte) 34);
        stream1.SetLength(0L);
        Message.ContactMessage.Serialize((Stream) stream1, instance.ContactMessageField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.LocationMessageField != null)
      {
        stream.WriteByte((byte) 42);
        stream1.SetLength(0L);
        Message.LocationMessage.Serialize((Stream) stream1, instance.LocationMessageField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.ExtendedTextMessageField != null)
      {
        stream.WriteByte((byte) 50);
        stream1.SetLength(0L);
        Message.ExtendedTextMessage.Serialize((Stream) stream1, instance.ExtendedTextMessageField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.DocumentMessageField != null)
      {
        stream.WriteByte((byte) 58);
        stream1.SetLength(0L);
        Message.DocumentMessage.Serialize((Stream) stream1, instance.DocumentMessageField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.AudioMessageField != null)
      {
        stream.WriteByte((byte) 66);
        stream1.SetLength(0L);
        Message.AudioMessage.Serialize((Stream) stream1, instance.AudioMessageField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.VideoMessageField != null)
      {
        stream.WriteByte((byte) 74);
        stream1.SetLength(0L);
        Message.VideoMessage.Serialize((Stream) stream1, instance.VideoMessageField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.CallField != null)
      {
        stream.WriteByte((byte) 82);
        stream1.SetLength(0L);
        Message.Call.Serialize((Stream) stream1, instance.CallField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.ChatField != null)
      {
        stream.WriteByte((byte) 90);
        stream1.SetLength(0L);
        Message.Chat.Serialize((Stream) stream1, instance.ChatField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.ProtocolMessageField != null)
      {
        stream.WriteByte((byte) 98);
        stream1.SetLength(0L);
        Message.ProtocolMessage.Serialize((Stream) stream1, instance.ProtocolMessageField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.ContactsArrayMessageField != null)
      {
        stream.WriteByte((byte) 106);
        stream1.SetLength(0L);
        Message.ContactsArrayMessage.Serialize((Stream) stream1, instance.ContactsArrayMessageField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.HighlyStructuredMessageField != null)
      {
        stream.WriteByte((byte) 114);
        stream1.SetLength(0L);
        Message.HighlyStructuredMessage.Serialize((Stream) stream1, instance.HighlyStructuredMessageField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.FastRatchetKeySenderKeyDistributionMessage != null)
      {
        stream.WriteByte((byte) 122);
        stream1.SetLength(0L);
        Message.SenderKeyDistributionMessage.Serialize((Stream) stream1, instance.FastRatchetKeySenderKeyDistributionMessage);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.SendPaymentMessageField != null)
      {
        stream.WriteByte((byte) 130);
        stream.WriteByte((byte) 1);
        stream1.SetLength(0L);
        Message.SendPaymentMessage.Serialize((Stream) stream1, instance.SendPaymentMessageField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.LiveLocationMessageField != null)
      {
        stream.WriteByte((byte) 146);
        stream.WriteByte((byte) 1);
        stream1.SetLength(0L);
        Message.LiveLocationMessage.Serialize((Stream) stream1, instance.LiveLocationMessageField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.RequestPaymentMessageField != null)
      {
        stream.WriteByte((byte) 178);
        stream.WriteByte((byte) 1);
        stream1.SetLength(0L);
        Message.RequestPaymentMessage.Serialize((Stream) stream1, instance.RequestPaymentMessageField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.DeclinePaymentRequestMessageField != null)
      {
        stream.WriteByte((byte) 186);
        stream.WriteByte((byte) 1);
        stream1.SetLength(0L);
        Message.DeclinePaymentRequestMessage.Serialize((Stream) stream1, instance.DeclinePaymentRequestMessageField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.CancelPaymentRequestMessageField != null)
      {
        stream.WriteByte((byte) 194);
        stream.WriteByte((byte) 1);
        stream1.SetLength(0L);
        Message.CancelPaymentRequestMessage.Serialize((Stream) stream1, instance.CancelPaymentRequestMessageField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.TemplateMessageField != null)
      {
        stream.WriteByte((byte) 202);
        stream.WriteByte((byte) 1);
        stream1.SetLength(0L);
        Message.TemplateMessage.Serialize((Stream) stream1, instance.TemplateMessageField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.StickerMessageField != null)
      {
        stream.WriteByte((byte) 210);
        stream.WriteByte((byte) 1);
        stream1.SetLength(0L);
        Message.StickerMessage.Serialize((Stream) stream1, instance.StickerMessageField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      ProtocolParser.Stack.Push(stream1);
      if (instance.PreservedFields == null)
        return;
      foreach (SilentOrbit.ProtocolBuffers.KeyValue preservedField in instance.PreservedFields)
      {
        ProtocolParser.WriteKey(stream, preservedField.Key);
        stream.Write(preservedField.Value, 0, preservedField.Value.Length);
      }
    }

    public static byte[] SerializeToBytes(Message instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        Message.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, Message instance)
    {
      byte[] bytes = Message.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }

    public IEnumerable<IProtoBufMessage> AvailableFields
    {
      get
      {
        return ((IEnumerable<IProtoBufMessage>) new IProtoBufMessage[15]
        {
          this.Conversation != null ? (IProtoBufMessage) new Message.ConversationMessage(this.Conversation) : (IProtoBufMessage) null,
          (IProtoBufMessage) this.ImageMessageField,
          (IProtoBufMessage) this.ContactMessageField,
          (IProtoBufMessage) this.LocationMessageField,
          (IProtoBufMessage) this.ExtendedTextMessageField,
          (IProtoBufMessage) this.DocumentMessageField,
          (IProtoBufMessage) this.AudioMessageField,
          (IProtoBufMessage) this.VideoMessageField,
          (IProtoBufMessage) this.CallField,
          (IProtoBufMessage) this.ProtocolMessageField,
          (IProtoBufMessage) this.ContactsArrayMessageField,
          (IProtoBufMessage) this.HighlyStructuredMessageField,
          (IProtoBufMessage) this.SendPaymentMessageField,
          (IProtoBufMessage) this.LiveLocationMessageField,
          (IProtoBufMessage) this.StickerMessageField
        }).Where<IProtoBufMessage>((Func<IProtoBufMessage, bool>) (o => o != null));
      }
    }

    public IEnumerable<IProtoBufMessage> UnsupportedFields
    {
      get
      {
        List<IProtoBufMessage> list = ((IEnumerable<IProtoBufMessage>) new IProtoBufMessage[1]
        {
          (IProtoBufMessage) this.RequestPaymentMessageField
        }).Where<IProtoBufMessage>((Func<IProtoBufMessage, bool>) (o => o != null)).ToList<IProtoBufMessage>();
        if (this.SendPaymentMessageField != null)
        {
          if (!Settings.IsWaAdmin)
          {
            list.Add((IProtoBufMessage) this.SendPaymentMessageField);
          }
          else
          {
            Message noteMessage = this.SendPaymentMessageField.NoteMessage;
            if (noteMessage == null || noteMessage.ExtendedTextMessageField == null)
              list.Add((IProtoBufMessage) this.SendPaymentMessageField);
          }
        }
        return (IEnumerable<IProtoBufMessage>) list;
      }
    }

    public int ContentCount => this.AvailableFields.Count<IProtoBufMessage>();

    public IEnumerable<MessageTypes> TagsOfMessage
    {
      get
      {
        return this.AvailableFields.Select<IProtoBufMessage, MessageTypes>((Func<IProtoBufMessage, MessageTypes>) (m => m.MessageType));
      }
    }

    public bool IsEmpty => this.ContentCount == 0;

    public bool IsHighlyStructuredMessage => this.HighlyStructuredMessageField != null;

    public IEnumerable<MessageTypes> UnknownTagsToReply
    {
      get
      {
        List<SilentOrbit.ProtocolBuffers.KeyValue> source = this.PreservedFields ?? new List<SilentOrbit.ProtocolBuffers.KeyValue>();
        return source.Count + this.TagsOfMessage.Count<MessageTypes>() > 1 ? source.Select<SilentOrbit.ProtocolBuffers.KeyValue, MessageTypes>((Func<SilentOrbit.ProtocolBuffers.KeyValue, MessageTypes>) (kv => (MessageTypes) kv.Key.Field)).Concat<MessageTypes>(this.TagsOfMessage) : (IEnumerable<MessageTypes>) null;
      }
    }

    public byte[] UnknownSerialized
    {
      get
      {
        if (this.unknownSerialized != null)
          return this.unknownSerialized;
        List<SilentOrbit.ProtocolBuffers.KeyValue> preservedFields = this.PreservedFields;
        return (preservedFields != null ? (__nonvirtual (preservedFields.Count) == 1 ? 1 : 0) : 0) != 0 && !this.TagsOfMessage.Any<MessageTypes>() || this.UnsupportedFields.Any<IProtoBufMessage>() ? this.Serialized : (byte[]) null;
      }
      set => this.unknownSerialized = value;
    }

    private bool ValidateMedia(
      FunXMPP.FMessage.Type type,
      byte[] hash,
      byte[] key,
      string url,
      string mimetype)
    {
      return hash != null && hash.Length == 32 && key != null && key.Length == 32 && this.ValidateMediaUrl(url) && this.ValidateMediaMimetype(type, mimetype);
    }

    private bool ValidateMediaUrl(string url)
    {
      if (string.IsNullOrEmpty(url))
        return false;
      Uri uri;
      try
      {
        uri = new Uri(url);
      }
      catch (Exception ex)
      {
        return false;
      }
      return !(uri.Scheme != "https") && uri.Host.EndsWith("whatsapp.net");
    }

    private bool ValidateMediaMimetype(FunXMPP.FMessage.Type type, string mimetype)
    {
      switch (type)
      {
        case FunXMPP.FMessage.Type.Image:
          return true;
        case FunXMPP.FMessage.Type.Audio:
          return ((IEnumerable<string>) Message.SupportedAudioTypes).Contains<string>(mimetype);
        case FunXMPP.FMessage.Type.Video:
          return ((IEnumerable<string>) Message.SupportedVideoTypes).Contains<string>(mimetype);
        case FunXMPP.FMessage.Type.Sticker:
          return ((IEnumerable<string>) Message.SupportedStickerTypes).Contains<string>(mimetype);
        default:
          return true;
      }
    }

    public bool IsValid
    {
      get
      {
        return this.ContentCount <= 1 && (this.ImageMessageField == null || this.ValidateMedia(FunXMPP.FMessage.Type.Image, this.ImageMessageField.FileSha256, this.ImageMessageField.MediaKey, this.ImageMessageField.Url, this.ImageMessageField.Mimetype)) && (this.AudioMessageField == null || this.ValidateMedia(FunXMPP.FMessage.Type.Audio, this.AudioMessageField.FileSha256, this.AudioMessageField.MediaKey, this.AudioMessageField.Url, this.AudioMessageField.Mimetype)) && (this.VideoMessageField == null || this.ValidateMedia(FunXMPP.FMessage.Type.Video, this.VideoMessageField.FileSha256, this.VideoMessageField.MediaKey, this.VideoMessageField.Url, this.VideoMessageField.Mimetype)) && (this.StickerMessageField == null || this.ValidateMedia(FunXMPP.FMessage.Type.Sticker, this.StickerMessageField.FileSha256, this.StickerMessageField.MediaKey, this.StickerMessageField.Url, this.StickerMessageField.Mimetype));
      }
    }

    public void Trim()
    {
      if (this.ImageMessageField != null)
      {
        this.ImageMessageField.FileSha256 = (byte[]) null;
        this.ImageMessageField.Caption = (string) null;
        this.ImageMessageField.MediaKey = (byte[]) null;
        this.ImageMessageField.JpegThumbnail = (byte[]) null;
      }
      else if (this.AudioMessageField != null)
      {
        this.AudioMessageField.FileSha256 = (byte[]) null;
        this.AudioMessageField.MediaKey = (byte[]) null;
        this.AudioMessageField.Url = (string) null;
        this.AudioMessageField.StreamingSidecar = (byte[]) null;
      }
      else if (this.ContactMessageField != null)
        this.ContactMessageField.Vcard = (string) null;
      else if (this.LocationMessageField != null)
        this.LocationMessageField.JpegThumbnail = (byte[]) null;
      else if (this.ExtendedTextMessageField != null)
        this.ExtendedTextMessageField.JpegThumbnail = (byte[]) null;
      else if (this.DocumentMessageField != null)
      {
        this.DocumentMessageField.FileSha256 = (byte[]) null;
        this.DocumentMessageField.MediaKey = (byte[]) null;
        this.DocumentMessageField.Url = (string) null;
        this.DocumentMessageField.JpegThumbnail = (byte[]) null;
      }
      else
      {
        if (this.VideoMessageField == null)
          return;
        this.VideoMessageField.FileSha256 = (byte[]) null;
        this.VideoMessageField.MediaKey = (byte[]) null;
        this.VideoMessageField.Url = (string) null;
        this.VideoMessageField.Caption = (string) null;
        this.VideoMessageField.JpegThumbnail = (byte[]) null;
        this.VideoMessageField.StreamingSidecar = (byte[]) null;
      }
    }

    public void RestoreTrimmed(FunXMPP.FMessage fmsg)
    {
      if (this.ImageMessageField != null)
      {
        this.ImageMessageField.FileSha256 = fmsg.media_hash;
        this.ImageMessageField.Caption = fmsg.media_caption;
        this.ImageMessageField.MediaKey = fmsg.media_key;
        this.ImageMessageField.JpegThumbnail = fmsg.binary_data;
      }
      else if (this.AudioMessageField != null)
      {
        this.AudioMessageField.FileSha256 = fmsg.media_hash;
        this.AudioMessageField.MediaKey = fmsg.media_key;
        this.AudioMessageField.Url = fmsg.media_url;
        this.AudioMessageField.StreamingSidecar = fmsg.message_properties?.MediaPropertiesField?.Sidecar;
      }
      else if (this.ContactMessageField != null)
        this.ContactMessageField.Vcard = fmsg.data;
      else if (this.LocationMessageField != null)
        this.LocationMessageField.JpegThumbnail = fmsg.binary_data;
      else if (this.ExtendedTextMessageField != null)
        this.ExtendedTextMessageField.JpegThumbnail = fmsg.binary_data;
      else if (this.DocumentMessageField != null)
      {
        this.DocumentMessageField.FileSha256 = fmsg.media_hash;
        this.DocumentMessageField.MediaKey = fmsg.media_key;
        this.DocumentMessageField.Url = fmsg.media_url;
        this.DocumentMessageField.JpegThumbnail = fmsg.binary_data;
      }
      else
      {
        if (this.VideoMessageField == null)
          return;
        this.VideoMessageField.FileSha256 = fmsg.media_hash;
        this.VideoMessageField.MediaKey = fmsg.media_key;
        this.VideoMessageField.Url = fmsg.media_url;
        this.VideoMessageField.Caption = fmsg.media_caption;
        this.VideoMessageField.JpegThumbnail = fmsg.binary_data;
        this.VideoMessageField.StreamingSidecar = fmsg.message_properties?.MediaPropertiesField?.Sidecar;
      }
    }

    public void PopulateFMessage(FunXMPP.FMessage fMessage)
    {
      ContextInfo contextInfo = (ContextInfo) null;
      if (this.Conversation != null)
      {
        fMessage.media_wa_type = FunXMPP.FMessage.Type.Undefined;
        fMessage.data = this.Conversation.MaybeTruncateToMaxRealCharLength(65536);
      }
      else if (this.ProtocolMessageField != null)
      {
        Message.ProtocolMessage.Type? type1 = this.ProtocolMessageField.type;
        Message.ProtocolMessage.Type type2 = Message.ProtocolMessage.Type.REVOKE;
        if ((type1.GetValueOrDefault() == type2 ? (type1.HasValue ? 1 : 0) : 0) != 0)
        {
          fMessage.media_wa_type = FunXMPP.FMessage.Type.Revoked;
          fMessage.key.id = this.ProtocolMessageField.Key.Id;
          fMessage.key.remote_jid = this.ProtocolMessageField.Key.RemoteJid;
          fMessage.key.from_me = ((int) this.ProtocolMessageField.Key.FromMe ?? 1) != 0;
          contextInfo = this.ProtocolMessageField.ContextInfo;
        }
      }
      else if (this.ImageMessageField != null)
      {
        contextInfo = this.ImageMessageField.ContextInfo;
        fMessage.media_wa_type = FunXMPP.FMessage.Type.Image;
        fMessage.media_caption = string.IsNullOrEmpty(this.ImageMessageField.Caption) ? (string) null : this.ImageMessageField.Caption.MaybeTruncateToMaxRealCharLength(1024);
        fMessage.media_size = (long) this.ImageMessageField.FileLength.GetValueOrDefault();
        fMessage.media_hash = this.ImageMessageField.FileSha256;
        fMessage.binary_data = this.ImageMessageField.JpegThumbnail;
        fMessage.media_mime_type = this.ImageMessageField.Mimetype;
        fMessage.media_url = this.ImageMessageField.Url;
        fMessage.media_key = this.ImageMessageField.MediaKey;
        if (this.ImageMessageField.FileEncSha256 != null)
        {
          if (fMessage.message_properties == null)
            fMessage.message_properties = new MessageProperties();
          fMessage.message_properties.EnsureCommonProperties.CipherMediaHash = this.ImageMessageField.FileEncSha256;
        }
        uint? height = this.ImageMessageField.Height;
        uint num1 = 0;
        if (((int) height.GetValueOrDefault() == (int) num1 ? (!height.HasValue ? 1 : 0) : 1) != 0)
        {
          uint? width = this.ImageMessageField.Width;
          uint num2 = 0;
          if (((int) width.GetValueOrDefault() == (int) num2 ? (!width.HasValue ? 1 : 0) : 1) != 0)
          {
            if (fMessage.message_properties == null)
              fMessage.message_properties = new MessageProperties();
            fMessage.message_properties.EnsureMediaProperties.Height = this.ImageMessageField.Height;
            fMessage.message_properties.EnsureMediaProperties.Width = this.ImageMessageField.Width;
          }
        }
        if (!string.IsNullOrEmpty(this.ImageMessageField.DirectPath))
        {
          if (fMessage.message_properties == null)
            fMessage.message_properties = new MessageProperties();
          fMessage.message_properties.EnsureMediaProperties.MediaDirectPath = this.ImageMessageField.DirectPath;
        }
      }
      else if (this.AudioMessageField != null)
      {
        contextInfo = this.AudioMessageField.ContextInfo;
        fMessage.media_wa_type = FunXMPP.FMessage.Type.Audio;
        fMessage.media_size = (long) this.AudioMessageField.FileLength.GetValueOrDefault();
        fMessage.media_hash = this.AudioMessageField.FileSha256;
        fMessage.media_mime_type = this.AudioMessageField.Mimetype;
        fMessage.media_url = this.AudioMessageField.Url;
        fMessage.media_key = this.AudioMessageField.MediaKey;
        if (this.AudioMessageField.FileEncSha256 != null)
        {
          if (fMessage.message_properties == null)
            fMessage.message_properties = new MessageProperties();
          fMessage.message_properties.EnsureCommonProperties.CipherMediaHash = this.AudioMessageField.FileEncSha256;
        }
        fMessage.media_origin = !this.AudioMessageField.Ptt.HasValue || !this.AudioMessageField.Ptt.Value ? (string) null : "live";
        fMessage.media_duration_seconds = (int) this.AudioMessageField.Seconds.GetValueOrDefault();
        byte[] streamingSidecar = this.AudioMessageField.StreamingSidecar;
        if ((streamingSidecar != null ? streamingSidecar.Length : 0) != 0)
        {
          if (fMessage.message_properties == null)
            fMessage.message_properties = new MessageProperties();
          fMessage.message_properties.EnsureMediaProperties.Sidecar = this.AudioMessageField.StreamingSidecar;
        }
        if (!string.IsNullOrEmpty(this.AudioMessageField.DirectPath))
        {
          if (fMessage.message_properties == null)
            fMessage.message_properties = new MessageProperties();
          fMessage.message_properties.EnsureMediaProperties.MediaDirectPath = this.AudioMessageField.DirectPath;
        }
      }
      else if (this.ContactMessageField != null)
      {
        contextInfo = this.ContactMessageField.ContextInfo;
        fMessage.media_wa_type = FunXMPP.FMessage.Type.Contact;
        fMessage.media_name = this.ContactMessageField.DisplayName;
        fMessage.data = this.ContactMessageField.Vcard;
      }
      else if (this.LocationMessageField != null)
      {
        contextInfo = this.LocationMessageField.ContextInfo;
        fMessage.media_wa_type = FunXMPP.FMessage.Type.Location;
        fMessage.details = string.Format("{0}\n{1}", (object) this.LocationMessageField.Name, (object) this.LocationMessageField.Address);
        fMessage.longitude = this.LocationMessageField.DegreesLongitude.GetValueOrDefault();
        fMessage.latitude = this.LocationMessageField.DegreesLatitude.GetValueOrDefault();
        fMessage.location_url = this.LocationMessageField.Url;
        fMessage.binary_data = this.LocationMessageField.JpegThumbnail;
      }
      else if (this.LiveLocationMessageField != null)
      {
        contextInfo = this.LiveLocationMessageField.ContextInfo;
        fMessage.media_wa_type = FunXMPP.FMessage.Type.LiveLocation;
        fMessage.longitude = this.LiveLocationMessageField.DegreesLongitude.GetValueOrDefault();
        fMessage.latitude = this.LiveLocationMessageField.DegreesLatitude.GetValueOrDefault();
        fMessage.binary_data = this.LiveLocationMessageField.JpegThumbnail;
        fMessage.message_properties = new MessageProperties();
        fMessage.message_properties.EnsureLiveLocationProperties.AccuracyInMeters = this.LiveLocationMessageField.AccuracyInMeters;
        fMessage.message_properties.EnsureLiveLocationProperties.SpeedInMps = this.LiveLocationMessageField.SpeedInMps;
        fMessage.message_properties.EnsureLiveLocationProperties.DegreesClockwiseFromMagneticNorth = this.LiveLocationMessageField.DegreesClockwiseFromMagneticNorth;
        fMessage.message_properties.EnsureLiveLocationProperties.Caption = this.LiveLocationMessageField.Caption;
      }
      else if (this.ExtendedTextMessageField != null)
        Message.PopulateFMessageFromExtendedTextMessage(this.ExtendedTextMessageField, ref fMessage, ref contextInfo);
      else if (this.DocumentMessageField != null)
      {
        contextInfo = this.DocumentMessageField.ContextInfo;
        fMessage.media_wa_type = FunXMPP.FMessage.Type.Document;
        fMessage.media_size = (long) this.DocumentMessageField.FileLength.GetValueOrDefault();
        fMessage.media_hash = this.DocumentMessageField.FileSha256;
        fMessage.media_mime_type = this.DocumentMessageField.Mimetype;
        fMessage.media_url = this.DocumentMessageField.Url;
        fMessage.media_key = this.DocumentMessageField.MediaKey;
        if (this.DocumentMessageField.FileEncSha256 != null)
        {
          if (fMessage.message_properties == null)
            fMessage.message_properties = new MessageProperties();
          fMessage.message_properties.EnsureCommonProperties.CipherMediaHash = this.DocumentMessageField.FileEncSha256;
        }
        fMessage.binary_data = this.DocumentMessageField.JpegThumbnail;
        DocumentMessageWrapper documentMessageWrapper = new DocumentMessageWrapper(fMessage)
        {
          Title = this.DocumentMessageField.Title.MaybeTruncateToMaxRealCharLength(65536),
          PageCount = (int) this.DocumentMessageField.PageCount.GetValueOrDefault(),
          Filename = this.DocumentMessageField.FileName.MaybeTruncateToMaxRealCharLength(65536)
        };
        if (!string.IsNullOrEmpty(this.DocumentMessageField.DirectPath))
        {
          if (fMessage.message_properties == null)
            fMessage.message_properties = new MessageProperties();
          fMessage.message_properties.EnsureMediaProperties.MediaDirectPath = this.DocumentMessageField.DirectPath;
        }
      }
      else if (this.VideoMessageField != null)
      {
        contextInfo = this.VideoMessageField.ContextInfo;
        FunXMPP.FMessage fmessage1 = fMessage;
        bool? gifPlayback = this.VideoMessageField.GifPlayback;
        int num3;
        if (gifPlayback.HasValue)
        {
          gifPlayback = this.VideoMessageField.GifPlayback;
          if (gifPlayback.Value)
          {
            num3 = 10;
            goto label_54;
          }
        }
        num3 = 3;
label_54:
        fmessage1.media_wa_type = (FunXMPP.FMessage.Type) num3;
        fMessage.media_url = this.VideoMessageField.Url;
        fMessage.media_mime_type = this.VideoMessageField.Mimetype;
        fMessage.media_hash = this.VideoMessageField.FileSha256;
        fMessage.media_size = (long) this.VideoMessageField.FileLength.GetValueOrDefault();
        FunXMPP.FMessage fmessage2 = fMessage;
        uint? nullable1 = this.VideoMessageField.Seconds;
        int valueOrDefault = (int) nullable1.GetValueOrDefault();
        fmessage2.media_duration_seconds = valueOrDefault;
        fMessage.media_key = this.VideoMessageField.MediaKey;
        if (this.VideoMessageField.FileEncSha256 != null)
        {
          if (fMessage.message_properties == null)
            fMessage.message_properties = new MessageProperties();
          fMessage.message_properties.EnsureCommonProperties.CipherMediaHash = this.VideoMessageField.FileEncSha256;
        }
        fMessage.binary_data = this.VideoMessageField.JpegThumbnail;
        fMessage.media_caption = string.IsNullOrEmpty(this.VideoMessageField.Caption) ? (string) null : this.VideoMessageField.Caption.MaybeTruncateToMaxRealCharLength(1024);
        if (fMessage.message_properties == null)
          fMessage.message_properties = new MessageProperties();
        MessageProperties.MediaProperties ensureMediaProperties = fMessage.message_properties.EnsureMediaProperties;
        Message.VideoMessage.Attribution? gifAttribution = this.VideoMessageField.GifAttribution;
        int num4;
        if (!gifAttribution.HasValue)
        {
          num4 = 0;
        }
        else
        {
          gifAttribution = this.VideoMessageField.GifAttribution;
          num4 = (int) gifAttribution.Value;
        }
        MessageProperties.MediaProperties.Attribution? nullable2 = new MessageProperties.MediaProperties.Attribution?((MessageProperties.MediaProperties.Attribution) num4);
        ensureMediaProperties.GifAttribution = nullable2;
        nullable1 = this.VideoMessageField.Height;
        uint num5 = 0;
        if (((int) nullable1.GetValueOrDefault() == (int) num5 ? (!nullable1.HasValue ? 1 : 0) : 1) != 0)
        {
          nullable1 = this.VideoMessageField.Width;
          uint num6 = 0;
          if (((int) nullable1.GetValueOrDefault() == (int) num6 ? (!nullable1.HasValue ? 1 : 0) : 1) != 0)
          {
            fMessage.message_properties.EnsureMediaProperties.Height = this.VideoMessageField.Height;
            fMessage.message_properties.EnsureMediaProperties.Width = this.VideoMessageField.Width;
          }
        }
        byte[] streamingSidecar = this.VideoMessageField.StreamingSidecar;
        if ((streamingSidecar != null ? streamingSidecar.Length : 0) != 0)
          fMessage.message_properties.EnsureMediaProperties.Sidecar = this.VideoMessageField.StreamingSidecar;
        if (!string.IsNullOrEmpty(this.VideoMessageField.DirectPath))
        {
          if (fMessage.message_properties == null)
            fMessage.message_properties = new MessageProperties();
          fMessage.message_properties.EnsureMediaProperties.MediaDirectPath = this.VideoMessageField.DirectPath;
        }
      }
      else if (this.ContactsArrayMessageField != null)
      {
        contextInfo = this.ContactsArrayMessageField.ContextInfo;
        fMessage.media_wa_type = FunXMPP.FMessage.Type.Contact;
        fMessage.media_name = this.ContactsArrayMessageField.DisplayName;
        List<string> stringList = new List<string>();
        foreach (Message.ContactMessage contact in this.ContactsArrayMessageField.Contacts)
          stringList.Add(contact.Vcard);
        fMessage.message_properties = new MessageProperties();
        fMessage.message_properties.EnsureContactProperties.Vcards = stringList;
      }
      else if (this.HighlyStructuredMessageField != null)
      {
        fMessage.media_wa_type = FunXMPP.FMessage.Type.HSM;
        fMessage.binary_data = Message.HighlyStructuredMessage.SerializeToBytes(this.HighlyStructuredMessageField);
      }
      else if (this.SendPaymentMessageField != null)
      {
        Message noteMessage = this.SendPaymentMessageField.NoteMessage;
        if (noteMessage != null && noteMessage.ExtendedTextMessageField != null)
        {
          Message.PopulateFMessageFromExtendedTextMessage(noteMessage.ExtendedTextMessageField, ref fMessage, ref contextInfo);
        }
        else
        {
          Log.SendCrashLog((Exception) new InvalidDataException("Unsupported payment message received"), "Unsupported payment message", logOnlyForRelease: true);
          throw new InvalidDataException("Unsupported payment message");
        }
      }
      else if (this.StickerMessageField != null)
      {
        fMessage.media_wa_type = FunXMPP.FMessage.Type.Sticker;
        fMessage.media_url = this.StickerMessageField.Url;
        fMessage.media_mime_type = this.StickerMessageField.Mimetype;
        fMessage.media_hash = this.StickerMessageField.FileSha256;
        fMessage.media_key = this.StickerMessageField.MediaKey;
        ulong? fileLength = this.StickerMessageField.FileLength;
        if (fileLength.HasValue)
          fMessage.media_size = (long) fileLength.Value;
        if (this.StickerMessageField.FileEncSha256 != null)
        {
          if (fMessage.message_properties == null)
            fMessage.message_properties = new MessageProperties();
          fMessage.message_properties.EnsureCommonProperties.CipherMediaHash = this.StickerMessageField.FileEncSha256;
        }
        if (fMessage.message_properties == null)
          fMessage.message_properties = new MessageProperties();
        uint? height = this.StickerMessageField.Height;
        uint num7 = 0;
        if (((int) height.GetValueOrDefault() == (int) num7 ? (!height.HasValue ? 1 : 0) : 1) != 0)
        {
          uint? width = this.StickerMessageField.Width;
          uint num8 = 0;
          if (((int) width.GetValueOrDefault() == (int) num8 ? (!width.HasValue ? 1 : 0) : 1) != 0)
          {
            fMessage.message_properties.EnsureMediaProperties.Height = this.StickerMessageField.Height;
            fMessage.message_properties.EnsureMediaProperties.Width = this.StickerMessageField.Width;
          }
        }
        contextInfo = this.StickerMessageField.ContextInfo;
      }
      if (contextInfo == null)
        return;
      this.Trim();
      fMessage.proto_buf = this.ToPlainText();
    }

    public static void PopulateFMessageFromExtendedTextMessage(
      Message.ExtendedTextMessage exTextField,
      ref FunXMPP.FMessage fMessage,
      ref ContextInfo contextInfo)
    {
      contextInfo = exTextField.ContextInfo;
      fMessage.media_wa_type = FunXMPP.FMessage.Type.ExtendedText;
      fMessage.data = exTextField.Text.MaybeTruncateToMaxRealCharLength(65536);
      Message.ExtendedTextMessage.FontType? font;
      int num;
      if (JidHelper.IsStatusJid(fMessage.key.remote_jid))
      {
        if (!exTextField.BackgroundArgb.HasValue)
        {
          font = exTextField.Font;
          num = font.HasValue ? 1 : 0;
        }
        else
          num = 1;
      }
      else
        num = 0;
      bool flag1 = false;
      if (string.IsNullOrEmpty(exTextField.MatchedText))
        flag1 = true;
      else if (string.IsNullOrEmpty(exTextField.Title) && string.IsNullOrEmpty(exTextField.Description))
        flag1 = true;
      else if (string.IsNullOrEmpty(exTextField.Text) || string.IsNullOrEmpty(exTextField.MatchedText))
        flag1 = true;
      else if (!exTextField.Text.Contains(exTextField.MatchedText))
        flag1 = true;
      bool flag2 = false;
      if (num != 0)
      {
        if (fMessage.message_properties == null)
          fMessage.message_properties = new MessageProperties();
        fMessage.message_properties.EnsureExtendedTextProperties.BackgroundArgb = exTextField.BackgroundArgb;
        MessageProperties.ExtendedTextProperties extendedTextProperties = fMessage.message_properties.EnsureExtendedTextProperties;
        font = exTextField.Font;
        int? nullable;
        if (!font.HasValue)
        {
          nullable = new int?();
        }
        else
        {
          font = exTextField.Font;
          nullable = new int?((int) font.Value);
        }
        extendedTextProperties.Font = nullable;
        flag2 = true;
      }
      if (!flag1)
      {
        fMessage.binary_data = exTextField.JpegThumbnail;
        UriMessageWrapper uriMessageWrapper = new UriMessageWrapper(fMessage)
        {
          Text = exTextField.Text.MaybeTruncateToMaxRealCharLength(65536),
          Title = exTextField.Title.MaybeTruncateToMaxRealCharLength(65536),
          Description = exTextField.Description.MaybeTruncateToMaxRealCharLength(65536),
          MatchedText = exTextField.MatchedText,
          CanonicalUrl = exTextField.CanonicalUrl.IdnToUnicodeAbsoluteUriString()
        };
        flag2 = true;
      }
      if (flag2)
        return;
      fMessage.media_wa_type = FunXMPP.FMessage.Type.Undefined;
    }

    public void SetMentionedJids(string[] jids)
    {
      List<string> list = jids == null || !((IEnumerable<string>) jids).Any<string>() ? (List<string>) null : ((IEnumerable<string>) jids).ToList<string>();
      ContextInfo ci = this.GetContextInfo();
      if (ci == null)
      {
        if (list == null)
          return;
        ci = new ContextInfo();
        this.AddContextInfo(ci);
      }
      ci.MentionedJid = list;
    }

    public void SetForwardedFlag(bool flag)
    {
      ContextInfo ci = this.GetContextInfo();
      if (ci == null && !flag)
        return;
      if (ci == null)
      {
        ci = new ContextInfo();
        this.AddContextInfo(ci);
      }
      if (flag)
        ci.IsForwarded = new bool?(true);
      else
        ci.IsForwarded = new bool?();
    }

    public void SetQuote(
      FunXMPP.FMessage quotedFmsg,
      string quotedAuthor,
      string quoteFrom,
      string quoteTo)
    {
      Message fromFmessage = Message.CreateFromFMessage(quotedFmsg, new CipherTextIncludes(true));
      this.RemoveQuote();
      if (fromFmessage == null && string.IsNullOrEmpty(quoteFrom))
        return;
      ContextInfo ci = this.GetContextInfo();
      if (ci == null)
      {
        ci = new ContextInfo();
        this.AddContextInfo(ci);
      }
      if (fromFmessage != null)
      {
        fromFmessage.RemoveQuote();
        ci.QuotedMessage = fromFmessage;
        ci.StanzaId = quotedFmsg.key.id;
      }
      ci.Participant = quotedAuthor;
      ci.RemoteJid = quoteFrom == quoteTo ? (string) null : quoteFrom;
    }

    private void RemoveQuote()
    {
      ContextInfo contextInfo = this.GetContextInfo();
      if (contextInfo == null)
        return;
      contextInfo.QuotedMessage = (Message) null;
      contextInfo.Participant = (string) null;
      contextInfo.StanzaId = (string) null;
      contextInfo.RemoteJid = (string) null;
      if (contextInfo.MentionedJid != null && contextInfo.MentionedJid.Any<string>())
        return;
      this.RemoveContextInfo();
    }

    private void AddContextInfo(ContextInfo ci)
    {
      if (this.Conversation != null)
      {
        string conversation = this.Conversation;
        this.Conversation = (string) null;
        this.ExtendedTextMessageField = new Message.ExtendedTextMessage()
        {
          Text = conversation
        };
      }
      if (this.ImageMessageField != null)
        this.ImageMessageField.ContextInfo = ci;
      else if (this.AudioMessageField != null)
        this.AudioMessageField.ContextInfo = ci;
      else if (this.ContactMessageField != null)
        this.ContactMessageField.ContextInfo = ci;
      else if (this.LocationMessageField != null)
        this.LocationMessageField.ContextInfo = ci;
      else if (this.ExtendedTextMessageField != null)
        this.ExtendedTextMessageField.ContextInfo = ci;
      else if (this.DocumentMessageField != null)
        this.DocumentMessageField.ContextInfo = ci;
      else if (this.VideoMessageField != null)
      {
        this.VideoMessageField.ContextInfo = ci;
      }
      else
      {
        if (this.StickerMessageField == null)
          return;
        this.StickerMessageField.ContextInfo = ci;
      }
    }

    public void RemoveContextInfo()
    {
      if (this.ImageMessageField != null)
        this.ImageMessageField.ContextInfo = (ContextInfo) null;
      else if (this.AudioMessageField != null)
        this.AudioMessageField.ContextInfo = (ContextInfo) null;
      else if (this.ContactMessageField != null)
        this.ContactMessageField.ContextInfo = (ContextInfo) null;
      else if (this.LocationMessageField != null)
        this.LocationMessageField.ContextInfo = (ContextInfo) null;
      else if (this.LiveLocationMessageField != null)
        this.LiveLocationMessageField.ContextInfo = (ContextInfo) null;
      else if (this.ExtendedTextMessageField != null)
      {
        this.ExtendedTextMessageField.ContextInfo = (ContextInfo) null;
        if (!string.IsNullOrEmpty(this.ExtendedTextMessageField.MatchedText))
          return;
        this.Conversation = this.ExtendedTextMessageField.Text;
        this.ExtendedTextMessageField = (Message.ExtendedTextMessage) null;
      }
      else if (this.DocumentMessageField != null)
        this.DocumentMessageField.ContextInfo = (ContextInfo) null;
      else if (this.VideoMessageField != null)
      {
        this.VideoMessageField.ContextInfo = (ContextInfo) null;
      }
      else
      {
        if (this.StickerMessageField == null)
          return;
        this.StickerMessageField.ContextInfo = (ContextInfo) null;
      }
    }

    public ContextInfo GetContextInfo()
    {
      return this.AvailableFields.Where<IProtoBufMessage>((Func<IProtoBufMessage, bool>) (m => m.ContextInfo != null)).Select<IProtoBufMessage, ContextInfo>((Func<IProtoBufMessage, ContextInfo>) (m => m.ContextInfo)).FirstOrDefault<ContextInfo>();
    }

    public static Message CreateFromFMessage(FunXMPP.FMessage fMessage, CipherTextIncludes includes)
    {
      if (fMessage == null)
        return (Message) null;
      Message fromFmessage = new Message();
      if (includes.SenderKeyDistributionMessage != null && JidHelper.IsMultiParticipantsChatJid(fMessage.key.remote_jid))
        fromFmessage.SenderKeyDistributionMessageField = new Message.SenderKeyDistributionMessage()
        {
          GroupId = fMessage.key.remote_jid,
          AxolotlSenderKeyDistributionMessage = includes.SenderKeyDistributionMessage
        };
      if (includes.Message)
      {
        FunXMPP.FMessage.Type type = fMessage.media_wa_type;
        ContextInfo contextInfo = new MessageContextInfoWrapper(fMessage).ContextInfo;
        if (fMessage.message_properties?.ConversionRecordPropertiesField != null)
        {
          if (contextInfo == null)
            contextInfo = new ContextInfo();
          contextInfo.ConversionSource = fMessage.message_properties?.ConversionRecordPropertiesField.Source;
          contextInfo.ConversionData = fMessage.message_properties?.ConversionRecordPropertiesField.Data;
          contextInfo.ConversionDelaySeconds = (uint?) fMessage.message_properties?.ConversionRecordPropertiesField.DelaySeconds;
        }
        else if (contextInfo != null)
        {
          contextInfo.ConversionSource = (string) null;
          contextInfo.ConversionData = (byte[]) null;
          contextInfo.ConversionDelaySeconds = new uint?();
        }
        bool flag = JidHelper.IsStatusJid(fMessage.key.remote_jid);
        if (fMessage.media_wa_type == FunXMPP.FMessage.Type.Undefined && contextInfo != null | flag)
          type = FunXMPP.FMessage.Type.ExtendedText;
        switch (type)
        {
          case FunXMPP.FMessage.Type.Undefined:
            fromFmessage.Conversation = fMessage.data;
            break;
          case FunXMPP.FMessage.Type.Image:
            fromFmessage.ImageMessageField = new Message.ImageMessage()
            {
              Caption = fMessage.media_caption,
              FileLength = new ulong?((ulong) fMessage.media_size),
              FileSha256 = fMessage.media_hash,
              JpegThumbnail = fMessage.binary_data,
              Mimetype = fMessage.media_mime_type,
              Url = fMessage.media_url,
              MediaKey = fMessage.media_key,
              ContextInfo = contextInfo,
              FileEncSha256 = fMessage.message_properties.GetCipherMediaHash(),
              Width = (uint?) fMessage.message_properties?.MediaPropertiesField?.Width,
              Height = (uint?) fMessage.message_properties?.MediaPropertiesField?.Height,
              DirectPath = fMessage.message_properties?.MediaPropertiesField?.MediaDirectPath
            };
            break;
          case FunXMPP.FMessage.Type.Audio:
            fromFmessage.AudioMessageField = new Message.AudioMessage()
            {
              FileLength = new ulong?((ulong) fMessage.media_size),
              FileSha256 = fMessage.media_hash,
              Mimetype = fMessage.media_mime_type,
              Url = fMessage.media_url,
              MediaKey = fMessage.media_key,
              Ptt = new bool?(fMessage.media_origin == "live"),
              Seconds = new uint?((uint) fMessage.media_duration_seconds),
              ContextInfo = contextInfo,
              StreamingSidecar = fMessage.message_properties?.MediaPropertiesField?.Sidecar,
              FileEncSha256 = fMessage.message_properties.GetCipherMediaHash(),
              DirectPath = fMessage.message_properties?.MediaPropertiesField?.MediaDirectPath
            };
            break;
          case FunXMPP.FMessage.Type.Video:
          case FunXMPP.FMessage.Type.Gif:
            Message.VideoMessage videoMessage = new Message.VideoMessage();
            videoMessage.FileLength = new ulong?((ulong) fMessage.media_size);
            videoMessage.FileSha256 = fMessage.media_hash;
            videoMessage.JpegThumbnail = fMessage.binary_data;
            videoMessage.MediaKey = fMessage.media_key;
            videoMessage.Mimetype = fMessage.media_mime_type;
            videoMessage.Seconds = new uint?((uint) fMessage.media_duration_seconds);
            videoMessage.Url = fMessage.media_url;
            videoMessage.Caption = fMessage.media_caption;
            videoMessage.ContextInfo = contextInfo;
            videoMessage.GifAttribution = new Message.VideoMessage.Attribution?((Message.VideoMessage.Attribution) fMessage.message_properties.GetGifAttribution());
            videoMessage.StreamingSidecar = fMessage.message_properties?.MediaPropertiesField?.Sidecar;
            videoMessage.FileEncSha256 = fMessage.message_properties.GetCipherMediaHash();
            videoMessage.Width = (uint?) fMessage.message_properties?.MediaPropertiesField?.Width;
            videoMessage.Height = (uint?) fMessage.message_properties?.MediaPropertiesField?.Height;
            videoMessage.DirectPath = fMessage.message_properties?.MediaPropertiesField?.MediaDirectPath;
            if (type == FunXMPP.FMessage.Type.Gif)
              videoMessage.GifPlayback = new bool?(true);
            fromFmessage.VideoMessageField = videoMessage;
            break;
          case FunXMPP.FMessage.Type.Contact:
            if (!fMessage.message_properties.HasMultipleContacts())
            {
              fromFmessage.ContactMessageField = new Message.ContactMessage()
              {
                DisplayName = fMessage.media_name,
                Vcard = fMessage.data,
                ContextInfo = contextInfo
              };
              break;
            }
            Message.ContactsArrayMessage contactsArrayMessage = new Message.ContactsArrayMessage();
            contactsArrayMessage.DisplayName = fMessage.media_name;
            int capacity = Math.Min(Settings.MaxGroupParticipants, fMessage.message_properties.ContactPropertiesField.Vcards.Count);
            if (capacity < fMessage.message_properties.ContactPropertiesField.Vcards.Count)
              Log.l("Webclient", "restricting number of contacts shown {0}, {1}", (object) capacity, (object) fMessage.message_properties.ContactPropertiesField.Vcards.Count);
            contactsArrayMessage.Contacts = new List<Message.ContactMessage>(capacity);
            for (int index = 0; index < capacity; ++index)
              contactsArrayMessage.Contacts.Add(new Message.ContactMessage()
              {
                Vcard = fMessage.message_properties.ContactPropertiesField.Vcards[index]
              });
            contactsArrayMessage.ContextInfo = contextInfo;
            fromFmessage.ContactsArrayMessageField = contactsArrayMessage;
            break;
          case FunXMPP.FMessage.Type.Location:
            Message.LocationMessage locationMessage = new Message.LocationMessage();
            WhatsApp.Message.PlaceDetails placeDetails = WhatsApp.Message.SplitPlaceDetails(fMessage.details);
            if (placeDetails != null)
            {
              locationMessage.Name = placeDetails.Name;
              locationMessage.Address = placeDetails.Address;
            }
            locationMessage.DegreesLongitude = new double?(fMessage.longitude);
            locationMessage.DegreesLatitude = new double?(fMessage.latitude);
            locationMessage.JpegThumbnail = fMessage.binary_data;
            locationMessage.Url = fMessage.location_url;
            locationMessage.ContextInfo = contextInfo;
            fromFmessage.LocationMessageField = locationMessage;
            break;
          case FunXMPP.FMessage.Type.Document:
            Message.DocumentMessage documentMessage = new Message.DocumentMessage();
            DocumentMessageWrapper documentMessageWrapper = new DocumentMessageWrapper(fMessage);
            documentMessage.FileLength = new ulong?((ulong) fMessage.media_size);
            documentMessage.FileSha256 = fMessage.media_hash;
            documentMessage.Mimetype = fMessage.media_mime_type;
            documentMessage.Url = fMessage.media_url;
            documentMessage.MediaKey = fMessage.media_key;
            documentMessage.Title = documentMessageWrapper.Title;
            documentMessage.JpegThumbnail = fMessage.binary_data;
            documentMessage.PageCount = new uint?((uint) documentMessageWrapper.PageCount);
            documentMessage.FileName = documentMessageWrapper.Filename;
            documentMessage.ContextInfo = contextInfo;
            documentMessage.FileEncSha256 = fMessage.message_properties.GetCipherMediaHash();
            documentMessage.DirectPath = fMessage.message_properties?.MediaPropertiesField?.MediaDirectPath;
            fromFmessage.DocumentMessageField = documentMessage;
            break;
          case FunXMPP.FMessage.Type.ExtendedText:
            fromFmessage.ExtendedTextMessageField = Message.CreateExtendedTextMessageFromFMessage(fMessage, contextInfo);
            if (fMessage.HasPaymentInfo())
            {
              fromFmessage.SendPaymentMessageField = new Message.SendPaymentMessage()
              {
                NoteMessage = new Message()
                {
                  ExtendedTextMessageField = fromFmessage.ExtendedTextMessageField
                }
              };
              fromFmessage.ExtendedTextMessageField = (Message.ExtendedTextMessage) null;
              break;
            }
            break;
          case FunXMPP.FMessage.Type.LiveLocation:
            fromFmessage.LiveLocationMessageField = new Message.LiveLocationMessage()
            {
              AccuracyInMeters = (uint?) fMessage.message_properties?.LiveLocationPropertiesField?.AccuracyInMeters,
              Caption = fMessage.message_properties?.LiveLocationPropertiesField?.Caption,
              ContextInfo = contextInfo,
              DegreesClockwiseFromMagneticNorth = (uint?) fMessage.message_properties?.LiveLocationPropertiesField?.DegreesClockwiseFromMagneticNorth,
              DegreesLatitude = new double?(fMessage.latitude),
              DegreesLongitude = new double?(fMessage.longitude),
              JpegThumbnail = fMessage.binary_data,
              SequenceNumber = new long?(fMessage.media_size),
              SpeedInMps = (float?) fMessage.message_properties?.LiveLocationPropertiesField?.SpeedInMps
            };
            break;
          case FunXMPP.FMessage.Type.Sticker:
            fromFmessage.StickerMessageField = new Message.StickerMessage()
            {
              FileSha256 = fMessage.media_hash,
              MediaKey = fMessage.media_key,
              Mimetype = fMessage.media_mime_type,
              Url = fMessage.media_url,
              ContextInfo = contextInfo,
              FileLength = new ulong?((ulong) fMessage.media_size),
              FileEncSha256 = fMessage.message_properties.GetCipherMediaHash(),
              Width = (uint?) fMessage.message_properties?.MediaPropertiesField?.Width,
              Height = (uint?) fMessage.message_properties?.MediaPropertiesField?.Height,
              DirectPath = fMessage.message_properties?.MediaPropertiesField?.MediaDirectPath
            };
            break;
          case FunXMPP.FMessage.Type.Revoked:
            fromFmessage.ProtocolMessageField = Message.CreateRevokeProtocolMessage(fMessage.key, fMessage.message_properties?.CommonPropertiesField?.RevokedMsgId, fMessage.remote_resource);
            break;
        }
      }
      return fromFmessage;
    }

    public static Message.ProtocolMessage CreateRevokeProtocolMessage(
      FunXMPP.FMessage.Key key,
      string revokedId,
      string remoteResource)
    {
      Message.ProtocolMessage revokeProtocolMessage = new Message.ProtocolMessage();
      revokeProtocolMessage.type = new Message.ProtocolMessage.Type?(Message.ProtocolMessage.Type.REVOKE);
      MessageKey messageKey = new MessageKey()
      {
        RemoteJid = key.remote_jid,
        FromMe = new bool?(key.from_me),
        Id = revokedId
      };
      messageKey.Participant = JidHelper.IsGroupJid(key.remote_jid) || JidHelper.IsStatusJid(key.remote_jid) ? remoteResource : (string) null;
      revokeProtocolMessage.Key = messageKey;
      return revokeProtocolMessage;
    }

    private static Message.ExtendedTextMessage CreateExtendedTextMessageFromFMessage(
      FunXMPP.FMessage fMessage,
      ContextInfo contextInfo)
    {
      UriMessageWrapper uriMessageWrapper = new UriMessageWrapper(fMessage);
      Message.ExtendedTextMessage extendedTextMessage = new Message.ExtendedTextMessage();
      extendedTextMessage.Title = uriMessageWrapper.Title;
      extendedTextMessage.Description = uriMessageWrapper.Description;
      extendedTextMessage.MatchedText = uriMessageWrapper.MatchedText;
      extendedTextMessage.Text = uriMessageWrapper.Text;
      extendedTextMessage.CanonicalUrl = uriMessageWrapper.CanonicalUrl.IdnToAsciiAbsoluteUriString();
      extendedTextMessage.JpegThumbnail = fMessage.binary_data;
      extendedTextMessage.ContextInfo = contextInfo;
      int? font = (int?) fMessage.message_properties?.ExtendedTextPropertiesField?.Font;
      extendedTextMessage.Font = font.HasValue ? new Message.ExtendedTextMessage.FontType?((Message.ExtendedTextMessage.FontType) font.GetValueOrDefault()) : new Message.ExtendedTextMessage.FontType?();
      extendedTextMessage.BackgroundArgb = (uint?) fMessage.message_properties?.ExtendedTextPropertiesField?.BackgroundArgb;
      Message.ExtendedTextMessage messageFromFmessage = extendedTextMessage;
      if (JidHelper.IsStatusJid(fMessage.key.remote_jid))
      {
        if (!messageFromFmessage.Font.HasValue)
          messageFromFmessage.Font = new Message.ExtendedTextMessage.FontType?(Message.ExtendedTextMessage.FontType.SANS_SERIF);
        if (!messageFromFmessage.BackgroundArgb.HasValue)
          messageFromFmessage.BackgroundArgb = new uint?(WaStatusHelper.ColorToUint(WaStatusHelper.GetRandomTextStatusBackgroundColor()));
      }
      return messageFromFmessage;
    }

    public static byte[] CreateForCall(byte[] callKey)
    {
      return new Message()
      {
        CallField = new Message.Call() { CallKey = callKey }
      }.ToPlainText();
    }

    public byte[] ToPlainText(bool withPadding = true)
    {
      MemoryStream memoryStream = new MemoryStream();
      Message.Serialize((Stream) memoryStream, this);
      if (withPadding)
      {
        int num = new Random().Next(1, 16);
        for (int index = 0; index < num; ++index)
          memoryStream.WriteByte((byte) num);
      }
      return memoryStream.ToArray();
    }

    public static Message CreateFromPlainText(byte[] plaintext)
    {
      if (plaintext == null)
        return (Message) null;
      byte num = ((IEnumerable<byte>) plaintext).Last<byte>();
      if (num == (byte) 0 || (int) num >= plaintext.Length)
        throw new Axolotl.AxolotlPaddingException();
      int length = plaintext.Length - (int) num;
      byte[] numArray = new byte[length];
      Array.Copy((Array) plaintext, (Array) numArray, length);
      return Message.CreateFromUnpaddedPlainText(numArray);
    }

    public static Message CreateFromUnpaddedPlainText(byte[] plaintext)
    {
      Message unpaddedPlainText;
      try
      {
        unpaddedPlainText = Message.Deserialize(plaintext);
        unpaddedPlainText.Serialized = plaintext;
      }
      catch (Exception ex)
      {
        Log.l(ex, "create protobuf msg from bytes");
        throw new Axolotl.AxolotlProtocolBufferException();
      }
      return unpaddedPlainText;
    }

    public class SenderKeyDistributionMessage
    {
      public string GroupId { get; set; }

      public byte[] AxolotlSenderKeyDistributionMessage { get; set; }

      public static Message.SenderKeyDistributionMessage Deserialize(Stream stream)
      {
        Message.SenderKeyDistributionMessage instance = new Message.SenderKeyDistributionMessage();
        Message.SenderKeyDistributionMessage.Deserialize(stream, instance);
        return instance;
      }

      public static Message.SenderKeyDistributionMessage DeserializeLengthDelimited(Stream stream)
      {
        Message.SenderKeyDistributionMessage instance = new Message.SenderKeyDistributionMessage();
        Message.SenderKeyDistributionMessage.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.SenderKeyDistributionMessage DeserializeLength(
        Stream stream,
        int length)
      {
        Message.SenderKeyDistributionMessage instance = new Message.SenderKeyDistributionMessage();
        Message.SenderKeyDistributionMessage.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.SenderKeyDistributionMessage Deserialize(byte[] buffer)
      {
        Message.SenderKeyDistributionMessage instance = new Message.SenderKeyDistributionMessage();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.SenderKeyDistributionMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.SenderKeyDistributionMessage Deserialize(
        byte[] buffer,
        Message.SenderKeyDistributionMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.SenderKeyDistributionMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.SenderKeyDistributionMessage Deserialize(
        Stream stream,
        Message.SenderKeyDistributionMessage instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_6;
            case 10:
              instance.GroupId = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.AxolotlSenderKeyDistributionMessage = ProtocolParser.ReadBytes(stream);
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

      public static Message.SenderKeyDistributionMessage DeserializeLengthDelimited(
        Stream stream,
        Message.SenderKeyDistributionMessage instance)
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
              instance.GroupId = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.AxolotlSenderKeyDistributionMessage = ProtocolParser.ReadBytes(stream);
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

      public static Message.SenderKeyDistributionMessage DeserializeLength(
        Stream stream,
        int length,
        Message.SenderKeyDistributionMessage instance)
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
              instance.GroupId = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.AxolotlSenderKeyDistributionMessage = ProtocolParser.ReadBytes(stream);
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

      public static void Serialize(Stream stream, Message.SenderKeyDistributionMessage instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.GroupId != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.GroupId));
        }
        if (instance.AxolotlSenderKeyDistributionMessage != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, instance.AxolotlSenderKeyDistributionMessage);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.SenderKeyDistributionMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.SenderKeyDistributionMessage.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        Message.SenderKeyDistributionMessage instance)
      {
        byte[] bytes = Message.SenderKeyDistributionMessage.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }
    }

    public class ImageMessage : IProtoBufMessage
    {
      public string Url { get; set; }

      public string Mimetype { get; set; }

      public string Caption { get; set; }

      public byte[] FileSha256 { get; set; }

      public ulong? FileLength { get; set; }

      public uint? Height { get; set; }

      public uint? Width { get; set; }

      public byte[] MediaKey { get; set; }

      public byte[] FileEncSha256 { get; set; }

      public List<InteractiveAnnotation> InteractiveAnnotations { get; set; }

      public string DirectPath { get; set; }

      public long? MediaKeyTimestamp { get; set; }

      public byte[] JpegThumbnail { get; set; }

      public ContextInfo ContextInfo { get; set; }

      public byte[] FirstScanSidecar { get; set; }

      public uint? FirstScanLength { get; set; }

      public static Message.ImageMessage Deserialize(Stream stream)
      {
        Message.ImageMessage instance = new Message.ImageMessage();
        Message.ImageMessage.Deserialize(stream, instance);
        return instance;
      }

      public static Message.ImageMessage DeserializeLengthDelimited(Stream stream)
      {
        Message.ImageMessage instance = new Message.ImageMessage();
        Message.ImageMessage.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.ImageMessage DeserializeLength(Stream stream, int length)
      {
        Message.ImageMessage instance = new Message.ImageMessage();
        Message.ImageMessage.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.ImageMessage Deserialize(byte[] buffer)
      {
        Message.ImageMessage instance = new Message.ImageMessage();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.ImageMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.ImageMessage Deserialize(byte[] buffer, Message.ImageMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.ImageMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.ImageMessage Deserialize(Stream stream, Message.ImageMessage instance)
      {
        if (instance.InteractiveAnnotations == null)
          instance.InteractiveAnnotations = new List<InteractiveAnnotation>();
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
                      goto label_28;
                    case 10:
                      instance.Url = ProtocolParser.ReadString(stream);
                      continue;
                    case 18:
                      instance.Mimetype = ProtocolParser.ReadString(stream);
                      continue;
                    case 26:
                      instance.Caption = ProtocolParser.ReadString(stream);
                      continue;
                    case 34:
                      instance.FileSha256 = ProtocolParser.ReadBytes(stream);
                      continue;
                    case 40:
                      instance.FileLength = new ulong?(ProtocolParser.ReadUInt64(stream));
                      continue;
                    case 48:
                      instance.Height = new uint?(ProtocolParser.ReadUInt32(stream));
                      continue;
                    case 56:
                      instance.Width = new uint?(ProtocolParser.ReadUInt32(stream));
                      continue;
                    case 66:
                      instance.MediaKey = ProtocolParser.ReadBytes(stream);
                      continue;
                    case 74:
                      instance.FileEncSha256 = ProtocolParser.ReadBytes(stream);
                      continue;
                    case 82:
                      instance.InteractiveAnnotations.Add(InteractiveAnnotation.DeserializeLengthDelimited(stream));
                      continue;
                    case 90:
                      instance.DirectPath = ProtocolParser.ReadString(stream);
                      continue;
                    case 96:
                      instance.MediaKeyTimestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
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
                          goto label_19;
                        case 18:
                          goto label_23;
                        case 19:
                          goto label_25;
                        default:
                          goto label_27;
                      }
                  }
                }
                while (key.WireType != Wire.LengthDelimited);
                instance.JpegThumbnail = ProtocolParser.ReadBytes(stream);
                continue;
label_19:;
              }
              while (key.WireType != Wire.LengthDelimited);
              if (instance.ContextInfo == null)
              {
                instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                continue;
              }
              ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
              continue;
label_23:;
            }
            while (key.WireType != Wire.LengthDelimited);
            instance.FirstScanSidecar = ProtocolParser.ReadBytes(stream);
            continue;
label_25:;
          }
          while (key.WireType != Wire.Varint);
          instance.FirstScanLength = new uint?(ProtocolParser.ReadUInt32(stream));
          continue;
label_27:
          ProtocolParser.SkipKey(stream, key);
        }
label_28:
        return instance;
      }

      public static Message.ImageMessage DeserializeLengthDelimited(
        Stream stream,
        Message.ImageMessage instance)
      {
        if (instance.InteractiveAnnotations == null)
          instance.InteractiveAnnotations = new List<InteractiveAnnotation>();
        long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 10:
              instance.Url = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Mimetype = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              instance.Caption = ProtocolParser.ReadString(stream);
              continue;
            case 34:
              instance.FileSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 40:
              instance.FileLength = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 48:
              instance.Height = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 56:
              instance.Width = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 66:
              instance.MediaKey = ProtocolParser.ReadBytes(stream);
              continue;
            case 74:
              instance.FileEncSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 82:
              instance.InteractiveAnnotations.Add(InteractiveAnnotation.DeserializeLengthDelimited(stream));
              continue;
            case 90:
              instance.DirectPath = ProtocolParser.ReadString(stream);
              continue;
            case 96:
              instance.MediaKeyTimestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.JpegThumbnail = ProtocolParser.ReadBytes(stream);
                    continue;
                  }
                  continue;
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
                    continue;
                  }
                  continue;
                case 18:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.FirstScanSidecar = ProtocolParser.ReadBytes(stream);
                    continue;
                  }
                  continue;
                case 19:
                  if (key.WireType == Wire.Varint)
                  {
                    instance.FirstScanLength = new uint?(ProtocolParser.ReadUInt32(stream));
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

      public static Message.ImageMessage DeserializeLength(
        Stream stream,
        int length,
        Message.ImageMessage instance)
      {
        if (instance.InteractiveAnnotations == null)
          instance.InteractiveAnnotations = new List<InteractiveAnnotation>();
        long num = stream.Position + (long) length;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 10:
              instance.Url = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Mimetype = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              instance.Caption = ProtocolParser.ReadString(stream);
              continue;
            case 34:
              instance.FileSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 40:
              instance.FileLength = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 48:
              instance.Height = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 56:
              instance.Width = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 66:
              instance.MediaKey = ProtocolParser.ReadBytes(stream);
              continue;
            case 74:
              instance.FileEncSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 82:
              instance.InteractiveAnnotations.Add(InteractiveAnnotation.DeserializeLengthDelimited(stream));
              continue;
            case 90:
              instance.DirectPath = ProtocolParser.ReadString(stream);
              continue;
            case 96:
              instance.MediaKeyTimestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.JpegThumbnail = ProtocolParser.ReadBytes(stream);
                    continue;
                  }
                  continue;
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
                    continue;
                  }
                  continue;
                case 18:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.FirstScanSidecar = ProtocolParser.ReadBytes(stream);
                    continue;
                  }
                  continue;
                case 19:
                  if (key.WireType == Wire.Varint)
                  {
                    instance.FirstScanLength = new uint?(ProtocolParser.ReadUInt32(stream));
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

      public static void Serialize(Stream stream, Message.ImageMessage instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Url != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Url));
        }
        if (instance.Mimetype != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Mimetype));
        }
        if (instance.Caption != null)
        {
          stream.WriteByte((byte) 26);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Caption));
        }
        if (instance.FileSha256 != null)
        {
          stream.WriteByte((byte) 34);
          ProtocolParser.WriteBytes(stream, instance.FileSha256);
        }
        if (instance.FileLength.HasValue)
        {
          stream.WriteByte((byte) 40);
          ProtocolParser.WriteUInt64(stream, instance.FileLength.Value);
        }
        if (instance.Height.HasValue)
        {
          stream.WriteByte((byte) 48);
          ProtocolParser.WriteUInt32(stream, instance.Height.Value);
        }
        if (instance.Width.HasValue)
        {
          stream.WriteByte((byte) 56);
          ProtocolParser.WriteUInt32(stream, instance.Width.Value);
        }
        if (instance.MediaKey != null)
        {
          stream.WriteByte((byte) 66);
          ProtocolParser.WriteBytes(stream, instance.MediaKey);
        }
        if (instance.FileEncSha256 != null)
        {
          stream.WriteByte((byte) 74);
          ProtocolParser.WriteBytes(stream, instance.FileEncSha256);
        }
        if (instance.InteractiveAnnotations != null)
        {
          foreach (InteractiveAnnotation interactiveAnnotation in instance.InteractiveAnnotations)
          {
            stream.WriteByte((byte) 82);
            stream1.SetLength(0L);
            InteractiveAnnotation.Serialize((Stream) stream1, interactiveAnnotation);
            uint length = (uint) stream1.Length;
            ProtocolParser.WriteUInt32(stream, length);
            stream1.WriteTo(stream);
          }
        }
        if (instance.DirectPath != null)
        {
          stream.WriteByte((byte) 90);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.DirectPath));
        }
        if (instance.MediaKeyTimestamp.HasValue)
        {
          stream.WriteByte((byte) 96);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.MediaKeyTimestamp.Value);
        }
        if (instance.JpegThumbnail != null)
        {
          stream.WriteByte((byte) 130);
          stream.WriteByte((byte) 1);
          ProtocolParser.WriteBytes(stream, instance.JpegThumbnail);
        }
        if (instance.ContextInfo != null)
        {
          stream.WriteByte((byte) 138);
          stream.WriteByte((byte) 1);
          stream1.SetLength(0L);
          ContextInfo.Serialize((Stream) stream1, instance.ContextInfo);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        if (instance.FirstScanSidecar != null)
        {
          stream.WriteByte((byte) 146);
          stream.WriteByte((byte) 1);
          ProtocolParser.WriteBytes(stream, instance.FirstScanSidecar);
        }
        if (instance.FirstScanLength.HasValue)
        {
          stream.WriteByte((byte) 152);
          stream.WriteByte((byte) 1);
          ProtocolParser.WriteUInt32(stream, instance.FirstScanLength.Value);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.ImageMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.ImageMessage.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(Stream stream, Message.ImageMessage instance)
      {
        byte[] bytes = Message.ImageMessage.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public MessageTypes MessageType => MessageTypes.image_message;
    }

    public class ContactMessage : IProtoBufMessage
    {
      public string DisplayName { get; set; }

      public string Vcard { get; set; }

      public ContextInfo ContextInfo { get; set; }

      public static Message.ContactMessage Deserialize(Stream stream)
      {
        Message.ContactMessage instance = new Message.ContactMessage();
        Message.ContactMessage.Deserialize(stream, instance);
        return instance;
      }

      public static Message.ContactMessage DeserializeLengthDelimited(Stream stream)
      {
        Message.ContactMessage instance = new Message.ContactMessage();
        Message.ContactMessage.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.ContactMessage DeserializeLength(Stream stream, int length)
      {
        Message.ContactMessage instance = new Message.ContactMessage();
        Message.ContactMessage.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.ContactMessage Deserialize(byte[] buffer)
      {
        Message.ContactMessage instance = new Message.ContactMessage();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.ContactMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.ContactMessage Deserialize(
        byte[] buffer,
        Message.ContactMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.ContactMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.ContactMessage Deserialize(
        Stream stream,
        Message.ContactMessage instance)
      {
        while (true)
        {
          SilentOrbit.ProtocolBuffers.Key key;
          do
          {
            do
            {
              int firstByte = stream.ReadByte();
              switch (firstByte)
              {
                case -1:
                  goto label_11;
                case 10:
                  instance.DisplayName = ProtocolParser.ReadString(stream);
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
                      goto label_6;
                    default:
                      goto label_10;
                  }
              }
            }
            while (key.WireType != Wire.LengthDelimited);
            instance.Vcard = ProtocolParser.ReadString(stream);
            continue;
label_6:;
          }
          while (key.WireType != Wire.LengthDelimited);
          if (instance.ContextInfo == null)
          {
            instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
            continue;
          }
          ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
          continue;
label_10:
          ProtocolParser.SkipKey(stream, key);
        }
label_11:
        return instance;
      }

      public static Message.ContactMessage DeserializeLengthDelimited(
        Stream stream,
        Message.ContactMessage instance)
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
              instance.DisplayName = ProtocolParser.ReadString(stream);
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.Vcard = ProtocolParser.ReadString(stream);
                    continue;
                  }
                  continue;
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
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

      public static Message.ContactMessage DeserializeLength(
        Stream stream,
        int length,
        Message.ContactMessage instance)
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
              instance.DisplayName = ProtocolParser.ReadString(stream);
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.Vcard = ProtocolParser.ReadString(stream);
                    continue;
                  }
                  continue;
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
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

      public static void Serialize(Stream stream, Message.ContactMessage instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.DisplayName != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.DisplayName));
        }
        if (instance.Vcard != null)
        {
          stream.WriteByte((byte) 130);
          stream.WriteByte((byte) 1);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Vcard));
        }
        if (instance.ContextInfo != null)
        {
          stream.WriteByte((byte) 138);
          stream.WriteByte((byte) 1);
          stream1.SetLength(0L);
          ContextInfo.Serialize((Stream) stream1, instance.ContextInfo);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.ContactMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.ContactMessage.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(Stream stream, Message.ContactMessage instance)
      {
        byte[] bytes = Message.ContactMessage.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public MessageTypes MessageType => MessageTypes.contact_message;
    }

    public class LocationMessage : IProtoBufMessage
    {
      public double? DegreesLatitude { get; set; }

      public double? DegreesLongitude { get; set; }

      public string Name { get; set; }

      public string Address { get; set; }

      public string Url { get; set; }

      [Obsolete]
      public bool? IsLive { get; set; }

      [Obsolete]
      public uint? AccuracyInMeters { get; set; }

      [Obsolete]
      public float? SpeedInMps { get; set; }

      [Obsolete]
      public uint? DegreesClockwiseFromMagneticNorth { get; set; }

      [Obsolete]
      public string Comment { get; set; }

      public byte[] JpegThumbnail { get; set; }

      public ContextInfo ContextInfo { get; set; }

      public static Message.LocationMessage Deserialize(Stream stream)
      {
        Message.LocationMessage instance = new Message.LocationMessage();
        Message.LocationMessage.Deserialize(stream, instance);
        return instance;
      }

      public static Message.LocationMessage DeserializeLengthDelimited(Stream stream)
      {
        Message.LocationMessage instance = new Message.LocationMessage();
        Message.LocationMessage.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.LocationMessage DeserializeLength(Stream stream, int length)
      {
        Message.LocationMessage instance = new Message.LocationMessage();
        Message.LocationMessage.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.LocationMessage Deserialize(byte[] buffer)
      {
        Message.LocationMessage instance = new Message.LocationMessage();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.LocationMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.LocationMessage Deserialize(
        byte[] buffer,
        Message.LocationMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.LocationMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.LocationMessage Deserialize(
        Stream stream,
        Message.LocationMessage instance)
      {
        BinaryReader binaryReader = new BinaryReader(stream);
        while (true)
        {
          SilentOrbit.ProtocolBuffers.Key key;
          do
          {
            do
            {
              int firstByte = stream.ReadByte();
              switch (firstByte)
              {
                case -1:
                  goto label_21;
                case 9:
                  instance.DegreesLatitude = new double?(binaryReader.ReadDouble());
                  continue;
                case 17:
                  instance.DegreesLongitude = new double?(binaryReader.ReadDouble());
                  continue;
                case 26:
                  instance.Name = ProtocolParser.ReadString(stream);
                  continue;
                case 34:
                  instance.Address = ProtocolParser.ReadString(stream);
                  continue;
                case 42:
                  instance.Url = ProtocolParser.ReadString(stream);
                  continue;
                case 48:
                  instance.IsLive = new bool?(ProtocolParser.ReadBool(stream));
                  continue;
                case 56:
                  instance.AccuracyInMeters = new uint?(ProtocolParser.ReadUInt32(stream));
                  continue;
                case 69:
                  instance.SpeedInMps = new float?(binaryReader.ReadSingle());
                  continue;
                case 72:
                  instance.DegreesClockwiseFromMagneticNorth = new uint?(ProtocolParser.ReadUInt32(stream));
                  continue;
                case 90:
                  instance.Comment = ProtocolParser.ReadString(stream);
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
                      goto label_16;
                    default:
                      goto label_20;
                  }
              }
            }
            while (key.WireType != Wire.LengthDelimited);
            instance.JpegThumbnail = ProtocolParser.ReadBytes(stream);
            continue;
label_16:;
          }
          while (key.WireType != Wire.LengthDelimited);
          if (instance.ContextInfo == null)
          {
            instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
            continue;
          }
          ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
          continue;
label_20:
          ProtocolParser.SkipKey(stream, key);
        }
label_21:
        return instance;
      }

      public static Message.LocationMessage DeserializeLengthDelimited(
        Stream stream,
        Message.LocationMessage instance)
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
            case 34:
              instance.Address = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.Url = ProtocolParser.ReadString(stream);
              continue;
            case 48:
              instance.IsLive = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 56:
              instance.AccuracyInMeters = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 69:
              instance.SpeedInMps = new float?(binaryReader.ReadSingle());
              continue;
            case 72:
              instance.DegreesClockwiseFromMagneticNorth = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 90:
              instance.Comment = ProtocolParser.ReadString(stream);
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.JpegThumbnail = ProtocolParser.ReadBytes(stream);
                    continue;
                  }
                  continue;
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
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

      public static Message.LocationMessage DeserializeLength(
        Stream stream,
        int length,
        Message.LocationMessage instance)
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
            case 34:
              instance.Address = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.Url = ProtocolParser.ReadString(stream);
              continue;
            case 48:
              instance.IsLive = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 56:
              instance.AccuracyInMeters = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 69:
              instance.SpeedInMps = new float?(binaryReader.ReadSingle());
              continue;
            case 72:
              instance.DegreesClockwiseFromMagneticNorth = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 90:
              instance.Comment = ProtocolParser.ReadString(stream);
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.JpegThumbnail = ProtocolParser.ReadBytes(stream);
                    continue;
                  }
                  continue;
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
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

      public static void Serialize(Stream stream, Message.LocationMessage instance)
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
        if (instance.Address != null)
        {
          stream.WriteByte((byte) 34);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Address));
        }
        if (instance.Url != null)
        {
          stream.WriteByte((byte) 42);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Url));
        }
        if (instance.IsLive.HasValue)
        {
          stream.WriteByte((byte) 48);
          ProtocolParser.WriteBool(stream, instance.IsLive.Value);
        }
        if (instance.AccuracyInMeters.HasValue)
        {
          stream.WriteByte((byte) 56);
          ProtocolParser.WriteUInt32(stream, instance.AccuracyInMeters.Value);
        }
        if (instance.SpeedInMps.HasValue)
        {
          stream.WriteByte((byte) 69);
          binaryWriter.Write(instance.SpeedInMps.Value);
        }
        if (instance.DegreesClockwiseFromMagneticNorth.HasValue)
        {
          stream.WriteByte((byte) 72);
          ProtocolParser.WriteUInt32(stream, instance.DegreesClockwiseFromMagneticNorth.Value);
        }
        if (instance.Comment != null)
        {
          stream.WriteByte((byte) 90);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Comment));
        }
        if (instance.JpegThumbnail != null)
        {
          stream.WriteByte((byte) 130);
          stream.WriteByte((byte) 1);
          ProtocolParser.WriteBytes(stream, instance.JpegThumbnail);
        }
        if (instance.ContextInfo != null)
        {
          stream.WriteByte((byte) 138);
          stream.WriteByte((byte) 1);
          stream1.SetLength(0L);
          ContextInfo.Serialize((Stream) stream1, instance.ContextInfo);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.LocationMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.LocationMessage.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(Stream stream, Message.LocationMessage instance)
      {
        byte[] bytes = Message.LocationMessage.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public MessageTypes MessageType => MessageTypes.location_message;
    }

    public class ExtendedTextMessage : IProtoBufMessage
    {
      public string Text { get; set; }

      public string MatchedText { get; set; }

      public string CanonicalUrl { get; set; }

      public string Description { get; set; }

      public string Title { get; set; }

      public uint? TextArgb { get; set; }

      public uint? BackgroundArgb { get; set; }

      public Message.ExtendedTextMessage.FontType? Font { get; set; }

      public Message.ExtendedTextMessage.PreviewType? preview_type { get; set; }

      public byte[] JpegThumbnail { get; set; }

      public ContextInfo ContextInfo { get; set; }

      public static Message.ExtendedTextMessage Deserialize(Stream stream)
      {
        Message.ExtendedTextMessage instance = new Message.ExtendedTextMessage();
        Message.ExtendedTextMessage.Deserialize(stream, instance);
        return instance;
      }

      public static Message.ExtendedTextMessage DeserializeLengthDelimited(Stream stream)
      {
        Message.ExtendedTextMessage instance = new Message.ExtendedTextMessage();
        Message.ExtendedTextMessage.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.ExtendedTextMessage DeserializeLength(Stream stream, int length)
      {
        Message.ExtendedTextMessage instance = new Message.ExtendedTextMessage();
        Message.ExtendedTextMessage.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.ExtendedTextMessage Deserialize(byte[] buffer)
      {
        Message.ExtendedTextMessage instance = new Message.ExtendedTextMessage();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.ExtendedTextMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.ExtendedTextMessage Deserialize(
        byte[] buffer,
        Message.ExtendedTextMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.ExtendedTextMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.ExtendedTextMessage Deserialize(
        Stream stream,
        Message.ExtendedTextMessage instance)
      {
        BinaryReader binaryReader = new BinaryReader(stream);
        while (true)
        {
          SilentOrbit.ProtocolBuffers.Key key;
          do
          {
            do
            {
              int firstByte = stream.ReadByte();
              switch (firstByte)
              {
                case -1:
                  goto label_20;
                case 10:
                  instance.Text = ProtocolParser.ReadString(stream);
                  continue;
                case 18:
                  instance.MatchedText = ProtocolParser.ReadString(stream);
                  continue;
                case 34:
                  instance.CanonicalUrl = ProtocolParser.ReadString(stream);
                  continue;
                case 42:
                  instance.Description = ProtocolParser.ReadString(stream);
                  continue;
                case 50:
                  instance.Title = ProtocolParser.ReadString(stream);
                  continue;
                case 61:
                  instance.TextArgb = new uint?(binaryReader.ReadUInt32());
                  continue;
                case 69:
                  instance.BackgroundArgb = new uint?(binaryReader.ReadUInt32());
                  continue;
                case 72:
                  instance.Font = new Message.ExtendedTextMessage.FontType?((Message.ExtendedTextMessage.FontType) ProtocolParser.ReadUInt64(stream));
                  continue;
                case 80:
                  instance.preview_type = new Message.ExtendedTextMessage.PreviewType?((Message.ExtendedTextMessage.PreviewType) ProtocolParser.ReadUInt64(stream));
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
                      goto label_15;
                    default:
                      goto label_19;
                  }
              }
            }
            while (key.WireType != Wire.LengthDelimited);
            instance.JpegThumbnail = ProtocolParser.ReadBytes(stream);
            continue;
label_15:;
          }
          while (key.WireType != Wire.LengthDelimited);
          if (instance.ContextInfo == null)
          {
            instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
            continue;
          }
          ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
          continue;
label_19:
          ProtocolParser.SkipKey(stream, key);
        }
label_20:
        return instance;
      }

      public static Message.ExtendedTextMessage DeserializeLengthDelimited(
        Stream stream,
        Message.ExtendedTextMessage instance)
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
            case 10:
              instance.Text = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.MatchedText = ProtocolParser.ReadString(stream);
              continue;
            case 34:
              instance.CanonicalUrl = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.Description = ProtocolParser.ReadString(stream);
              continue;
            case 50:
              instance.Title = ProtocolParser.ReadString(stream);
              continue;
            case 61:
              instance.TextArgb = new uint?(binaryReader.ReadUInt32());
              continue;
            case 69:
              instance.BackgroundArgb = new uint?(binaryReader.ReadUInt32());
              continue;
            case 72:
              instance.Font = new Message.ExtendedTextMessage.FontType?((Message.ExtendedTextMessage.FontType) ProtocolParser.ReadUInt64(stream));
              continue;
            case 80:
              instance.preview_type = new Message.ExtendedTextMessage.PreviewType?((Message.ExtendedTextMessage.PreviewType) ProtocolParser.ReadUInt64(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.JpegThumbnail = ProtocolParser.ReadBytes(stream);
                    continue;
                  }
                  continue;
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
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

      public static Message.ExtendedTextMessage DeserializeLength(
        Stream stream,
        int length,
        Message.ExtendedTextMessage instance)
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
            case 10:
              instance.Text = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.MatchedText = ProtocolParser.ReadString(stream);
              continue;
            case 34:
              instance.CanonicalUrl = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.Description = ProtocolParser.ReadString(stream);
              continue;
            case 50:
              instance.Title = ProtocolParser.ReadString(stream);
              continue;
            case 61:
              instance.TextArgb = new uint?(binaryReader.ReadUInt32());
              continue;
            case 69:
              instance.BackgroundArgb = new uint?(binaryReader.ReadUInt32());
              continue;
            case 72:
              instance.Font = new Message.ExtendedTextMessage.FontType?((Message.ExtendedTextMessage.FontType) ProtocolParser.ReadUInt64(stream));
              continue;
            case 80:
              instance.preview_type = new Message.ExtendedTextMessage.PreviewType?((Message.ExtendedTextMessage.PreviewType) ProtocolParser.ReadUInt64(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.JpegThumbnail = ProtocolParser.ReadBytes(stream);
                    continue;
                  }
                  continue;
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
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

      public static void Serialize(Stream stream, Message.ExtendedTextMessage instance)
      {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Text != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Text));
        }
        if (instance.MatchedText != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.MatchedText));
        }
        if (instance.CanonicalUrl != null)
        {
          stream.WriteByte((byte) 34);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.CanonicalUrl));
        }
        if (instance.Description != null)
        {
          stream.WriteByte((byte) 42);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Description));
        }
        if (instance.Title != null)
        {
          stream.WriteByte((byte) 50);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Title));
        }
        if (instance.TextArgb.HasValue)
        {
          stream.WriteByte((byte) 61);
          binaryWriter.Write(instance.TextArgb.Value);
        }
        if (instance.BackgroundArgb.HasValue)
        {
          stream.WriteByte((byte) 69);
          binaryWriter.Write(instance.BackgroundArgb.Value);
        }
        if (instance.Font.HasValue)
        {
          stream.WriteByte((byte) 72);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.Font.Value);
        }
        if (instance.preview_type.HasValue)
        {
          stream.WriteByte((byte) 80);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.preview_type.Value);
        }
        if (instance.JpegThumbnail != null)
        {
          stream.WriteByte((byte) 130);
          stream.WriteByte((byte) 1);
          ProtocolParser.WriteBytes(stream, instance.JpegThumbnail);
        }
        if (instance.ContextInfo != null)
        {
          stream.WriteByte((byte) 138);
          stream.WriteByte((byte) 1);
          stream1.SetLength(0L);
          ContextInfo.Serialize((Stream) stream1, instance.ContextInfo);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.ExtendedTextMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.ExtendedTextMessage.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        Message.ExtendedTextMessage instance)
      {
        byte[] bytes = Message.ExtendedTextMessage.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public MessageTypes MessageType => MessageTypes.extended_text_message;

      public enum FontType
      {
        SANS_SERIF,
        SERIF,
        NORICAN_REGULAR,
        BRYNDAN_WRITE,
        BEBASNEUE_REGULAR,
        OSWALD_HEAVY,
      }

      public enum PreviewType
      {
        NONE,
        VIDEO,
      }
    }

    public class DocumentMessage : IProtoBufMessage
    {
      public string Url { get; set; }

      public string Mimetype { get; set; }

      public string Title { get; set; }

      public byte[] FileSha256 { get; set; }

      public ulong? FileLength { get; set; }

      public uint? PageCount { get; set; }

      public byte[] MediaKey { get; set; }

      public string FileName { get; set; }

      public byte[] FileEncSha256 { get; set; }

      public string DirectPath { get; set; }

      public long? MediaKeyTimestamp { get; set; }

      public byte[] JpegThumbnail { get; set; }

      public ContextInfo ContextInfo { get; set; }

      public static Message.DocumentMessage Deserialize(Stream stream)
      {
        Message.DocumentMessage instance = new Message.DocumentMessage();
        Message.DocumentMessage.Deserialize(stream, instance);
        return instance;
      }

      public static Message.DocumentMessage DeserializeLengthDelimited(Stream stream)
      {
        Message.DocumentMessage instance = new Message.DocumentMessage();
        Message.DocumentMessage.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.DocumentMessage DeserializeLength(Stream stream, int length)
      {
        Message.DocumentMessage instance = new Message.DocumentMessage();
        Message.DocumentMessage.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.DocumentMessage Deserialize(byte[] buffer)
      {
        Message.DocumentMessage instance = new Message.DocumentMessage();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.DocumentMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.DocumentMessage Deserialize(
        byte[] buffer,
        Message.DocumentMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.DocumentMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.DocumentMessage Deserialize(
        Stream stream,
        Message.DocumentMessage instance)
      {
        while (true)
        {
          SilentOrbit.ProtocolBuffers.Key key;
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
                  instance.Url = ProtocolParser.ReadString(stream);
                  continue;
                case 18:
                  instance.Mimetype = ProtocolParser.ReadString(stream);
                  continue;
                case 26:
                  instance.Title = ProtocolParser.ReadString(stream);
                  continue;
                case 34:
                  instance.FileSha256 = ProtocolParser.ReadBytes(stream);
                  continue;
                case 40:
                  instance.FileLength = new ulong?(ProtocolParser.ReadUInt64(stream));
                  continue;
                case 48:
                  instance.PageCount = new uint?(ProtocolParser.ReadUInt32(stream));
                  continue;
                case 58:
                  instance.MediaKey = ProtocolParser.ReadBytes(stream);
                  continue;
                case 66:
                  instance.FileName = ProtocolParser.ReadString(stream);
                  continue;
                case 74:
                  instance.FileEncSha256 = ProtocolParser.ReadBytes(stream);
                  continue;
                case 82:
                  instance.DirectPath = ProtocolParser.ReadString(stream);
                  continue;
                case 88:
                  instance.MediaKeyTimestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
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
                      goto label_16;
                    default:
                      goto label_20;
                  }
              }
            }
            while (key.WireType != Wire.LengthDelimited);
            instance.JpegThumbnail = ProtocolParser.ReadBytes(stream);
            continue;
label_16:;
          }
          while (key.WireType != Wire.LengthDelimited);
          if (instance.ContextInfo == null)
          {
            instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
            continue;
          }
          ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
          continue;
label_20:
          ProtocolParser.SkipKey(stream, key);
        }
label_21:
        return instance;
      }

      public static Message.DocumentMessage DeserializeLengthDelimited(
        Stream stream,
        Message.DocumentMessage instance)
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
              instance.Url = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Mimetype = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              instance.Title = ProtocolParser.ReadString(stream);
              continue;
            case 34:
              instance.FileSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 40:
              instance.FileLength = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 48:
              instance.PageCount = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 58:
              instance.MediaKey = ProtocolParser.ReadBytes(stream);
              continue;
            case 66:
              instance.FileName = ProtocolParser.ReadString(stream);
              continue;
            case 74:
              instance.FileEncSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 82:
              instance.DirectPath = ProtocolParser.ReadString(stream);
              continue;
            case 88:
              instance.MediaKeyTimestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.JpegThumbnail = ProtocolParser.ReadBytes(stream);
                    continue;
                  }
                  continue;
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
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

      public static Message.DocumentMessage DeserializeLength(
        Stream stream,
        int length,
        Message.DocumentMessage instance)
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
              instance.Url = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Mimetype = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              instance.Title = ProtocolParser.ReadString(stream);
              continue;
            case 34:
              instance.FileSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 40:
              instance.FileLength = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 48:
              instance.PageCount = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 58:
              instance.MediaKey = ProtocolParser.ReadBytes(stream);
              continue;
            case 66:
              instance.FileName = ProtocolParser.ReadString(stream);
              continue;
            case 74:
              instance.FileEncSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 82:
              instance.DirectPath = ProtocolParser.ReadString(stream);
              continue;
            case 88:
              instance.MediaKeyTimestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.JpegThumbnail = ProtocolParser.ReadBytes(stream);
                    continue;
                  }
                  continue;
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
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

      public static void Serialize(Stream stream, Message.DocumentMessage instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Url != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Url));
        }
        if (instance.Mimetype != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Mimetype));
        }
        if (instance.Title != null)
        {
          stream.WriteByte((byte) 26);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Title));
        }
        if (instance.FileSha256 != null)
        {
          stream.WriteByte((byte) 34);
          ProtocolParser.WriteBytes(stream, instance.FileSha256);
        }
        if (instance.FileLength.HasValue)
        {
          stream.WriteByte((byte) 40);
          ProtocolParser.WriteUInt64(stream, instance.FileLength.Value);
        }
        if (instance.PageCount.HasValue)
        {
          stream.WriteByte((byte) 48);
          ProtocolParser.WriteUInt32(stream, instance.PageCount.Value);
        }
        if (instance.MediaKey != null)
        {
          stream.WriteByte((byte) 58);
          ProtocolParser.WriteBytes(stream, instance.MediaKey);
        }
        if (instance.FileName != null)
        {
          stream.WriteByte((byte) 66);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.FileName));
        }
        if (instance.FileEncSha256 != null)
        {
          stream.WriteByte((byte) 74);
          ProtocolParser.WriteBytes(stream, instance.FileEncSha256);
        }
        if (instance.DirectPath != null)
        {
          stream.WriteByte((byte) 82);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.DirectPath));
        }
        if (instance.MediaKeyTimestamp.HasValue)
        {
          stream.WriteByte((byte) 88);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.MediaKeyTimestamp.Value);
        }
        if (instance.JpegThumbnail != null)
        {
          stream.WriteByte((byte) 130);
          stream.WriteByte((byte) 1);
          ProtocolParser.WriteBytes(stream, instance.JpegThumbnail);
        }
        if (instance.ContextInfo != null)
        {
          stream.WriteByte((byte) 138);
          stream.WriteByte((byte) 1);
          stream1.SetLength(0L);
          ContextInfo.Serialize((Stream) stream1, instance.ContextInfo);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.DocumentMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.DocumentMessage.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(Stream stream, Message.DocumentMessage instance)
      {
        byte[] bytes = Message.DocumentMessage.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public MessageTypes MessageType => MessageTypes.document_message;
    }

    public class AudioMessage : IProtoBufMessage
    {
      public string Url { get; set; }

      public string Mimetype { get; set; }

      public byte[] FileSha256 { get; set; }

      public ulong? FileLength { get; set; }

      public uint? Seconds { get; set; }

      public bool? Ptt { get; set; }

      public byte[] MediaKey { get; set; }

      public byte[] FileEncSha256 { get; set; }

      public string DirectPath { get; set; }

      public long? MediaKeyTimestamp { get; set; }

      public ContextInfo ContextInfo { get; set; }

      public byte[] StreamingSidecar { get; set; }

      public static Message.AudioMessage Deserialize(Stream stream)
      {
        Message.AudioMessage instance = new Message.AudioMessage();
        Message.AudioMessage.Deserialize(stream, instance);
        return instance;
      }

      public static Message.AudioMessage DeserializeLengthDelimited(Stream stream)
      {
        Message.AudioMessage instance = new Message.AudioMessage();
        Message.AudioMessage.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.AudioMessage DeserializeLength(Stream stream, int length)
      {
        Message.AudioMessage instance = new Message.AudioMessage();
        Message.AudioMessage.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.AudioMessage Deserialize(byte[] buffer)
      {
        Message.AudioMessage instance = new Message.AudioMessage();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.AudioMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.AudioMessage Deserialize(byte[] buffer, Message.AudioMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.AudioMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.AudioMessage Deserialize(Stream stream, Message.AudioMessage instance)
      {
        while (true)
        {
          SilentOrbit.ProtocolBuffers.Key key;
          do
          {
            do
            {
              int firstByte = stream.ReadByte();
              switch (firstByte)
              {
                case -1:
                  goto label_20;
                case 10:
                  instance.Url = ProtocolParser.ReadString(stream);
                  continue;
                case 18:
                  instance.Mimetype = ProtocolParser.ReadString(stream);
                  continue;
                case 26:
                  instance.FileSha256 = ProtocolParser.ReadBytes(stream);
                  continue;
                case 32:
                  instance.FileLength = new ulong?(ProtocolParser.ReadUInt64(stream));
                  continue;
                case 40:
                  instance.Seconds = new uint?(ProtocolParser.ReadUInt32(stream));
                  continue;
                case 48:
                  instance.Ptt = new bool?(ProtocolParser.ReadBool(stream));
                  continue;
                case 58:
                  instance.MediaKey = ProtocolParser.ReadBytes(stream);
                  continue;
                case 66:
                  instance.FileEncSha256 = ProtocolParser.ReadBytes(stream);
                  continue;
                case 74:
                  instance.DirectPath = ProtocolParser.ReadString(stream);
                  continue;
                case 80:
                  instance.MediaKeyTimestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
                  continue;
                default:
                  key = ProtocolParser.ReadKey((byte) firstByte, stream);
                  switch (key.Field)
                  {
                    case 0:
                      throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                    case 17:
                      continue;
                    case 18:
                      goto label_17;
                    default:
                      goto label_19;
                  }
              }
            }
            while (key.WireType != Wire.LengthDelimited);
            if (instance.ContextInfo == null)
            {
              instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
              continue;
            }
            ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
            continue;
label_17:;
          }
          while (key.WireType != Wire.LengthDelimited);
          instance.StreamingSidecar = ProtocolParser.ReadBytes(stream);
          continue;
label_19:
          ProtocolParser.SkipKey(stream, key);
        }
label_20:
        return instance;
      }

      public static Message.AudioMessage DeserializeLengthDelimited(
        Stream stream,
        Message.AudioMessage instance)
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
              instance.Url = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Mimetype = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              instance.FileSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 32:
              instance.FileLength = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 40:
              instance.Seconds = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 48:
              instance.Ptt = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 58:
              instance.MediaKey = ProtocolParser.ReadBytes(stream);
              continue;
            case 66:
              instance.FileEncSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 74:
              instance.DirectPath = ProtocolParser.ReadString(stream);
              continue;
            case 80:
              instance.MediaKeyTimestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
                    continue;
                  }
                  continue;
                case 18:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.StreamingSidecar = ProtocolParser.ReadBytes(stream);
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

      public static Message.AudioMessage DeserializeLength(
        Stream stream,
        int length,
        Message.AudioMessage instance)
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
              instance.Url = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Mimetype = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              instance.FileSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 32:
              instance.FileLength = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 40:
              instance.Seconds = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 48:
              instance.Ptt = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 58:
              instance.MediaKey = ProtocolParser.ReadBytes(stream);
              continue;
            case 66:
              instance.FileEncSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 74:
              instance.DirectPath = ProtocolParser.ReadString(stream);
              continue;
            case 80:
              instance.MediaKeyTimestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
                    continue;
                  }
                  continue;
                case 18:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.StreamingSidecar = ProtocolParser.ReadBytes(stream);
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

      public static void Serialize(Stream stream, Message.AudioMessage instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Url != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Url));
        }
        if (instance.Mimetype != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Mimetype));
        }
        if (instance.FileSha256 != null)
        {
          stream.WriteByte((byte) 26);
          ProtocolParser.WriteBytes(stream, instance.FileSha256);
        }
        if (instance.FileLength.HasValue)
        {
          stream.WriteByte((byte) 32);
          ProtocolParser.WriteUInt64(stream, instance.FileLength.Value);
        }
        if (instance.Seconds.HasValue)
        {
          stream.WriteByte((byte) 40);
          ProtocolParser.WriteUInt32(stream, instance.Seconds.Value);
        }
        if (instance.Ptt.HasValue)
        {
          stream.WriteByte((byte) 48);
          ProtocolParser.WriteBool(stream, instance.Ptt.Value);
        }
        if (instance.MediaKey != null)
        {
          stream.WriteByte((byte) 58);
          ProtocolParser.WriteBytes(stream, instance.MediaKey);
        }
        if (instance.FileEncSha256 != null)
        {
          stream.WriteByte((byte) 66);
          ProtocolParser.WriteBytes(stream, instance.FileEncSha256);
        }
        if (instance.DirectPath != null)
        {
          stream.WriteByte((byte) 74);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.DirectPath));
        }
        if (instance.MediaKeyTimestamp.HasValue)
        {
          stream.WriteByte((byte) 80);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.MediaKeyTimestamp.Value);
        }
        if (instance.ContextInfo != null)
        {
          stream.WriteByte((byte) 138);
          stream.WriteByte((byte) 1);
          stream1.SetLength(0L);
          ContextInfo.Serialize((Stream) stream1, instance.ContextInfo);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        if (instance.StreamingSidecar != null)
        {
          stream.WriteByte((byte) 146);
          stream.WriteByte((byte) 1);
          ProtocolParser.WriteBytes(stream, instance.StreamingSidecar);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.AudioMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.AudioMessage.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(Stream stream, Message.AudioMessage instance)
      {
        byte[] bytes = Message.AudioMessage.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public MessageTypes MessageType => MessageTypes.audio_message;
    }

    public class VideoMessage : IProtoBufMessage
    {
      public string Url { get; set; }

      public string Mimetype { get; set; }

      public byte[] FileSha256 { get; set; }

      public ulong? FileLength { get; set; }

      public uint? Seconds { get; set; }

      public byte[] MediaKey { get; set; }

      public string Caption { get; set; }

      public bool? GifPlayback { get; set; }

      public uint? Height { get; set; }

      public uint? Width { get; set; }

      public byte[] FileEncSha256 { get; set; }

      public List<InteractiveAnnotation> InteractiveAnnotations { get; set; }

      public string DirectPath { get; set; }

      public long? MediaKeyTimestamp { get; set; }

      public byte[] JpegThumbnail { get; set; }

      public ContextInfo ContextInfo { get; set; }

      public byte[] StreamingSidecar { get; set; }

      public Message.VideoMessage.Attribution? GifAttribution { get; set; }

      public static Message.VideoMessage Deserialize(Stream stream)
      {
        Message.VideoMessage instance = new Message.VideoMessage();
        Message.VideoMessage.Deserialize(stream, instance);
        return instance;
      }

      public static Message.VideoMessage DeserializeLengthDelimited(Stream stream)
      {
        Message.VideoMessage instance = new Message.VideoMessage();
        Message.VideoMessage.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.VideoMessage DeserializeLength(Stream stream, int length)
      {
        Message.VideoMessage instance = new Message.VideoMessage();
        Message.VideoMessage.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.VideoMessage Deserialize(byte[] buffer)
      {
        Message.VideoMessage instance = new Message.VideoMessage();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.VideoMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.VideoMessage Deserialize(byte[] buffer, Message.VideoMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.VideoMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.VideoMessage Deserialize(Stream stream, Message.VideoMessage instance)
      {
        if (instance.InteractiveAnnotations == null)
          instance.InteractiveAnnotations = new List<InteractiveAnnotation>();
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
                      goto label_30;
                    case 10:
                      instance.Url = ProtocolParser.ReadString(stream);
                      continue;
                    case 18:
                      instance.Mimetype = ProtocolParser.ReadString(stream);
                      continue;
                    case 26:
                      instance.FileSha256 = ProtocolParser.ReadBytes(stream);
                      continue;
                    case 32:
                      instance.FileLength = new ulong?(ProtocolParser.ReadUInt64(stream));
                      continue;
                    case 40:
                      instance.Seconds = new uint?(ProtocolParser.ReadUInt32(stream));
                      continue;
                    case 50:
                      instance.MediaKey = ProtocolParser.ReadBytes(stream);
                      continue;
                    case 58:
                      instance.Caption = ProtocolParser.ReadString(stream);
                      continue;
                    case 64:
                      instance.GifPlayback = new bool?(ProtocolParser.ReadBool(stream));
                      continue;
                    case 72:
                      instance.Height = new uint?(ProtocolParser.ReadUInt32(stream));
                      continue;
                    case 80:
                      instance.Width = new uint?(ProtocolParser.ReadUInt32(stream));
                      continue;
                    case 90:
                      instance.FileEncSha256 = ProtocolParser.ReadBytes(stream);
                      continue;
                    case 98:
                      instance.InteractiveAnnotations.Add(InteractiveAnnotation.DeserializeLengthDelimited(stream));
                      continue;
                    case 106:
                      instance.DirectPath = ProtocolParser.ReadString(stream);
                      continue;
                    case 112:
                      instance.MediaKeyTimestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
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
                          goto label_21;
                        case 18:
                          goto label_25;
                        case 19:
                          goto label_27;
                        default:
                          goto label_29;
                      }
                  }
                }
                while (key.WireType != Wire.LengthDelimited);
                instance.JpegThumbnail = ProtocolParser.ReadBytes(stream);
                continue;
label_21:;
              }
              while (key.WireType != Wire.LengthDelimited);
              if (instance.ContextInfo == null)
              {
                instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                continue;
              }
              ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
              continue;
label_25:;
            }
            while (key.WireType != Wire.LengthDelimited);
            instance.StreamingSidecar = ProtocolParser.ReadBytes(stream);
            continue;
label_27:;
          }
          while (key.WireType != Wire.Varint);
          instance.GifAttribution = new Message.VideoMessage.Attribution?((Message.VideoMessage.Attribution) ProtocolParser.ReadUInt64(stream));
          continue;
label_29:
          ProtocolParser.SkipKey(stream, key);
        }
label_30:
        return instance;
      }

      public static Message.VideoMessage DeserializeLengthDelimited(
        Stream stream,
        Message.VideoMessage instance)
      {
        if (instance.InteractiveAnnotations == null)
          instance.InteractiveAnnotations = new List<InteractiveAnnotation>();
        long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 10:
              instance.Url = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Mimetype = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              instance.FileSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 32:
              instance.FileLength = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 40:
              instance.Seconds = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 50:
              instance.MediaKey = ProtocolParser.ReadBytes(stream);
              continue;
            case 58:
              instance.Caption = ProtocolParser.ReadString(stream);
              continue;
            case 64:
              instance.GifPlayback = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 72:
              instance.Height = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 80:
              instance.Width = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 90:
              instance.FileEncSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 98:
              instance.InteractiveAnnotations.Add(InteractiveAnnotation.DeserializeLengthDelimited(stream));
              continue;
            case 106:
              instance.DirectPath = ProtocolParser.ReadString(stream);
              continue;
            case 112:
              instance.MediaKeyTimestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.JpegThumbnail = ProtocolParser.ReadBytes(stream);
                    continue;
                  }
                  continue;
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
                    continue;
                  }
                  continue;
                case 18:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.StreamingSidecar = ProtocolParser.ReadBytes(stream);
                    continue;
                  }
                  continue;
                case 19:
                  if (key.WireType == Wire.Varint)
                  {
                    instance.GifAttribution = new Message.VideoMessage.Attribution?((Message.VideoMessage.Attribution) ProtocolParser.ReadUInt64(stream));
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

      public static Message.VideoMessage DeserializeLength(
        Stream stream,
        int length,
        Message.VideoMessage instance)
      {
        if (instance.InteractiveAnnotations == null)
          instance.InteractiveAnnotations = new List<InteractiveAnnotation>();
        long num = stream.Position + (long) length;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 10:
              instance.Url = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Mimetype = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              instance.FileSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 32:
              instance.FileLength = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 40:
              instance.Seconds = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 50:
              instance.MediaKey = ProtocolParser.ReadBytes(stream);
              continue;
            case 58:
              instance.Caption = ProtocolParser.ReadString(stream);
              continue;
            case 64:
              instance.GifPlayback = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 72:
              instance.Height = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 80:
              instance.Width = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 90:
              instance.FileEncSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 98:
              instance.InteractiveAnnotations.Add(InteractiveAnnotation.DeserializeLengthDelimited(stream));
              continue;
            case 106:
              instance.DirectPath = ProtocolParser.ReadString(stream);
              continue;
            case 112:
              instance.MediaKeyTimestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.JpegThumbnail = ProtocolParser.ReadBytes(stream);
                    continue;
                  }
                  continue;
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
                    continue;
                  }
                  continue;
                case 18:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.StreamingSidecar = ProtocolParser.ReadBytes(stream);
                    continue;
                  }
                  continue;
                case 19:
                  if (key.WireType == Wire.Varint)
                  {
                    instance.GifAttribution = new Message.VideoMessage.Attribution?((Message.VideoMessage.Attribution) ProtocolParser.ReadUInt64(stream));
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

      public static void Serialize(Stream stream, Message.VideoMessage instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Url != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Url));
        }
        if (instance.Mimetype != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Mimetype));
        }
        if (instance.FileSha256 != null)
        {
          stream.WriteByte((byte) 26);
          ProtocolParser.WriteBytes(stream, instance.FileSha256);
        }
        if (instance.FileLength.HasValue)
        {
          stream.WriteByte((byte) 32);
          ProtocolParser.WriteUInt64(stream, instance.FileLength.Value);
        }
        if (instance.Seconds.HasValue)
        {
          stream.WriteByte((byte) 40);
          ProtocolParser.WriteUInt32(stream, instance.Seconds.Value);
        }
        if (instance.MediaKey != null)
        {
          stream.WriteByte((byte) 50);
          ProtocolParser.WriteBytes(stream, instance.MediaKey);
        }
        if (instance.Caption != null)
        {
          stream.WriteByte((byte) 58);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Caption));
        }
        if (instance.GifPlayback.HasValue)
        {
          stream.WriteByte((byte) 64);
          ProtocolParser.WriteBool(stream, instance.GifPlayback.Value);
        }
        if (instance.Height.HasValue)
        {
          stream.WriteByte((byte) 72);
          ProtocolParser.WriteUInt32(stream, instance.Height.Value);
        }
        if (instance.Width.HasValue)
        {
          stream.WriteByte((byte) 80);
          ProtocolParser.WriteUInt32(stream, instance.Width.Value);
        }
        if (instance.FileEncSha256 != null)
        {
          stream.WriteByte((byte) 90);
          ProtocolParser.WriteBytes(stream, instance.FileEncSha256);
        }
        if (instance.InteractiveAnnotations != null)
        {
          foreach (InteractiveAnnotation interactiveAnnotation in instance.InteractiveAnnotations)
          {
            stream.WriteByte((byte) 98);
            stream1.SetLength(0L);
            InteractiveAnnotation.Serialize((Stream) stream1, interactiveAnnotation);
            uint length = (uint) stream1.Length;
            ProtocolParser.WriteUInt32(stream, length);
            stream1.WriteTo(stream);
          }
        }
        if (instance.DirectPath != null)
        {
          stream.WriteByte((byte) 106);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.DirectPath));
        }
        if (instance.MediaKeyTimestamp.HasValue)
        {
          stream.WriteByte((byte) 112);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.MediaKeyTimestamp.Value);
        }
        if (instance.JpegThumbnail != null)
        {
          stream.WriteByte((byte) 130);
          stream.WriteByte((byte) 1);
          ProtocolParser.WriteBytes(stream, instance.JpegThumbnail);
        }
        if (instance.ContextInfo != null)
        {
          stream.WriteByte((byte) 138);
          stream.WriteByte((byte) 1);
          stream1.SetLength(0L);
          ContextInfo.Serialize((Stream) stream1, instance.ContextInfo);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        if (instance.StreamingSidecar != null)
        {
          stream.WriteByte((byte) 146);
          stream.WriteByte((byte) 1);
          ProtocolParser.WriteBytes(stream, instance.StreamingSidecar);
        }
        if (instance.GifAttribution.HasValue)
        {
          stream.WriteByte((byte) 152);
          stream.WriteByte((byte) 1);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.GifAttribution.Value);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.VideoMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.VideoMessage.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(Stream stream, Message.VideoMessage instance)
      {
        byte[] bytes = Message.VideoMessage.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public MessageTypes MessageType => MessageTypes.video_message;

      public enum Attribution
      {
        NONE,
        GIPHY,
        TENOR,
      }
    }

    public class Call : IProtoBufMessage
    {
      public byte[] CallKey { get; set; }

      public static Message.Call Deserialize(Stream stream)
      {
        Message.Call instance = new Message.Call();
        Message.Call.Deserialize(stream, instance);
        return instance;
      }

      public static Message.Call DeserializeLengthDelimited(Stream stream)
      {
        Message.Call instance = new Message.Call();
        Message.Call.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.Call DeserializeLength(Stream stream, int length)
      {
        Message.Call instance = new Message.Call();
        Message.Call.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.Call Deserialize(byte[] buffer)
      {
        Message.Call instance = new Message.Call();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.Call.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.Call Deserialize(byte[] buffer, Message.Call instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.Call.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.Call Deserialize(Stream stream, Message.Call instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_5;
            case 10:
              instance.CallKey = ProtocolParser.ReadBytes(stream);
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              if (key.Field != 0U)
              {
                ProtocolParser.SkipKey(stream, key);
                continue;
              }
              goto label_3;
          }
        }
label_3:
        throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_5:
        return instance;
      }

      public static Message.Call DeserializeLengthDelimited(Stream stream, Message.Call instance)
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
              instance.CallKey = ProtocolParser.ReadBytes(stream);
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

      public static Message.Call DeserializeLength(
        Stream stream,
        int length,
        Message.Call instance)
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
              instance.CallKey = ProtocolParser.ReadBytes(stream);
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

      public static void Serialize(Stream stream, Message.Call instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.CallKey != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, instance.CallKey);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.Call instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.Call.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(Stream stream, Message.Call instance)
      {
        byte[] bytes = Message.Call.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public MessageTypes MessageType => MessageTypes.call;

      public ContextInfo ContextInfo => (ContextInfo) null;
    }

    public class Chat
    {
      public string DisplayName { get; set; }

      public string Id { get; set; }

      public static Message.Chat Deserialize(Stream stream)
      {
        Message.Chat instance = new Message.Chat();
        Message.Chat.Deserialize(stream, instance);
        return instance;
      }

      public static Message.Chat DeserializeLengthDelimited(Stream stream)
      {
        Message.Chat instance = new Message.Chat();
        Message.Chat.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.Chat DeserializeLength(Stream stream, int length)
      {
        Message.Chat instance = new Message.Chat();
        Message.Chat.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.Chat Deserialize(byte[] buffer)
      {
        Message.Chat instance = new Message.Chat();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.Chat.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.Chat Deserialize(byte[] buffer, Message.Chat instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.Chat.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.Chat Deserialize(Stream stream, Message.Chat instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_6;
            case 10:
              instance.DisplayName = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Id = ProtocolParser.ReadString(stream);
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

      public static Message.Chat DeserializeLengthDelimited(Stream stream, Message.Chat instance)
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
              instance.DisplayName = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Id = ProtocolParser.ReadString(stream);
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

      public static Message.Chat DeserializeLength(
        Stream stream,
        int length,
        Message.Chat instance)
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
              instance.DisplayName = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Id = ProtocolParser.ReadString(stream);
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

      public static void Serialize(Stream stream, Message.Chat instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.DisplayName != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.DisplayName));
        }
        if (instance.Id != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Id));
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.Chat instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.Chat.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(Stream stream, Message.Chat instance)
      {
        byte[] bytes = Message.Chat.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }
    }

    public class ProtocolMessage : IProtoBufMessage
    {
      public MessageKey Key { get; set; }

      public Message.ProtocolMessage.Type? type { get; set; }

      public static Message.ProtocolMessage Deserialize(Stream stream)
      {
        Message.ProtocolMessage instance = new Message.ProtocolMessage();
        Message.ProtocolMessage.Deserialize(stream, instance);
        return instance;
      }

      public static Message.ProtocolMessage DeserializeLengthDelimited(Stream stream)
      {
        Message.ProtocolMessage instance = new Message.ProtocolMessage();
        Message.ProtocolMessage.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.ProtocolMessage DeserializeLength(Stream stream, int length)
      {
        Message.ProtocolMessage instance = new Message.ProtocolMessage();
        Message.ProtocolMessage.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.ProtocolMessage Deserialize(byte[] buffer)
      {
        Message.ProtocolMessage instance = new Message.ProtocolMessage();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.ProtocolMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.ProtocolMessage Deserialize(
        byte[] buffer,
        Message.ProtocolMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.ProtocolMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.ProtocolMessage Deserialize(
        Stream stream,
        Message.ProtocolMessage instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_8;
            case 10:
              if (instance.Key == null)
              {
                instance.Key = MessageKey.DeserializeLengthDelimited(stream);
                continue;
              }
              MessageKey.DeserializeLengthDelimited(stream, instance.Key);
              continue;
            case 16:
              instance.type = new Message.ProtocolMessage.Type?((Message.ProtocolMessage.Type) ProtocolParser.ReadUInt64(stream));
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

      public static Message.ProtocolMessage DeserializeLengthDelimited(
        Stream stream,
        Message.ProtocolMessage instance)
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
              if (instance.Key == null)
              {
                instance.Key = MessageKey.DeserializeLengthDelimited(stream);
                continue;
              }
              MessageKey.DeserializeLengthDelimited(stream, instance.Key);
              continue;
            case 16:
              instance.type = new Message.ProtocolMessage.Type?((Message.ProtocolMessage.Type) ProtocolParser.ReadUInt64(stream));
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

      public static Message.ProtocolMessage DeserializeLength(
        Stream stream,
        int length,
        Message.ProtocolMessage instance)
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
              if (instance.Key == null)
              {
                instance.Key = MessageKey.DeserializeLengthDelimited(stream);
                continue;
              }
              MessageKey.DeserializeLengthDelimited(stream, instance.Key);
              continue;
            case 16:
              instance.type = new Message.ProtocolMessage.Type?((Message.ProtocolMessage.Type) ProtocolParser.ReadUInt64(stream));
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

      public static void Serialize(Stream stream, Message.ProtocolMessage instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Key != null)
        {
          stream.WriteByte((byte) 10);
          stream1.SetLength(0L);
          MessageKey.Serialize((Stream) stream1, instance.Key);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        if (instance.type.HasValue)
        {
          stream.WriteByte((byte) 16);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.type.Value);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.ProtocolMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.ProtocolMessage.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(Stream stream, Message.ProtocolMessage instance)
      {
        byte[] bytes = Message.ProtocolMessage.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public MessageTypes MessageType => MessageTypes.protocol_message;

      public ContextInfo ContextInfo => (ContextInfo) null;

      public enum Type
      {
        REVOKE,
      }
    }

    public class ContactsArrayMessage : IProtoBufMessage
    {
      public string DisplayName { get; set; }

      public List<Message.ContactMessage> Contacts { get; set; }

      public ContextInfo ContextInfo { get; set; }

      public static Message.ContactsArrayMessage Deserialize(Stream stream)
      {
        Message.ContactsArrayMessage instance = new Message.ContactsArrayMessage();
        Message.ContactsArrayMessage.Deserialize(stream, instance);
        return instance;
      }

      public static Message.ContactsArrayMessage DeserializeLengthDelimited(Stream stream)
      {
        Message.ContactsArrayMessage instance = new Message.ContactsArrayMessage();
        Message.ContactsArrayMessage.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.ContactsArrayMessage DeserializeLength(Stream stream, int length)
      {
        Message.ContactsArrayMessage instance = new Message.ContactsArrayMessage();
        Message.ContactsArrayMessage.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.ContactsArrayMessage Deserialize(byte[] buffer)
      {
        Message.ContactsArrayMessage instance = new Message.ContactsArrayMessage();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.ContactsArrayMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.ContactsArrayMessage Deserialize(
        byte[] buffer,
        Message.ContactsArrayMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.ContactsArrayMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.ContactsArrayMessage Deserialize(
        Stream stream,
        Message.ContactsArrayMessage instance)
      {
        if (instance.Contacts == null)
          instance.Contacts = new List<Message.ContactMessage>();
        while (true)
        {
          SilentOrbit.ProtocolBuffers.Key key;
          do
          {
            int firstByte = stream.ReadByte();
            switch (firstByte)
            {
              case -1:
                goto label_12;
              case 10:
                instance.DisplayName = ProtocolParser.ReadString(stream);
                continue;
              case 18:
                instance.Contacts.Add(Message.ContactMessage.DeserializeLengthDelimited(stream));
                continue;
              default:
                key = ProtocolParser.ReadKey((byte) firstByte, stream);
                switch (key.Field)
                {
                  case 0:
                    throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                  case 17:
                    continue;
                  default:
                    goto label_11;
                }
            }
          }
          while (key.WireType != Wire.LengthDelimited);
          if (instance.ContextInfo == null)
          {
            instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
            continue;
          }
          ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
          continue;
label_11:
          ProtocolParser.SkipKey(stream, key);
        }
label_12:
        return instance;
      }

      public static Message.ContactsArrayMessage DeserializeLengthDelimited(
        Stream stream,
        Message.ContactsArrayMessage instance)
      {
        if (instance.Contacts == null)
          instance.Contacts = new List<Message.ContactMessage>();
        long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 10:
              instance.DisplayName = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Contacts.Add(Message.ContactMessage.DeserializeLengthDelimited(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
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

      public static Message.ContactsArrayMessage DeserializeLength(
        Stream stream,
        int length,
        Message.ContactsArrayMessage instance)
      {
        if (instance.Contacts == null)
          instance.Contacts = new List<Message.ContactMessage>();
        long num = stream.Position + (long) length;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 10:
              instance.DisplayName = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Contacts.Add(Message.ContactMessage.DeserializeLengthDelimited(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
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

      public static void Serialize(Stream stream, Message.ContactsArrayMessage instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.DisplayName != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.DisplayName));
        }
        if (instance.Contacts != null)
        {
          foreach (Message.ContactMessage contact in instance.Contacts)
          {
            stream.WriteByte((byte) 18);
            stream1.SetLength(0L);
            Message.ContactMessage.Serialize((Stream) stream1, contact);
            uint length = (uint) stream1.Length;
            ProtocolParser.WriteUInt32(stream, length);
            stream1.WriteTo(stream);
          }
        }
        if (instance.ContextInfo != null)
        {
          stream.WriteByte((byte) 138);
          stream.WriteByte((byte) 1);
          stream1.SetLength(0L);
          ContextInfo.Serialize((Stream) stream1, instance.ContextInfo);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.ContactsArrayMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.ContactsArrayMessage.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        Message.ContactsArrayMessage instance)
      {
        byte[] bytes = Message.ContactsArrayMessage.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public MessageTypes MessageType => MessageTypes.contacts_array_message;
    }

    public class HighlyStructuredMessage : IProtoBufMessage
    {
      public string Namespace { get; set; }

      public string ElementName { get; set; }

      public List<string> Params { get; set; }

      public string FallbackLg { get; set; }

      public string FallbackLc { get; set; }

      public List<Message.HighlyStructuredMessage.HSMLocalizableParameter> LocalizableParams { get; set; }

      public string DeterministicLg { get; set; }

      public string DeterministicLc { get; set; }

      public static Message.HighlyStructuredMessage Deserialize(Stream stream)
      {
        Message.HighlyStructuredMessage instance = new Message.HighlyStructuredMessage();
        Message.HighlyStructuredMessage.Deserialize(stream, instance);
        return instance;
      }

      public static Message.HighlyStructuredMessage DeserializeLengthDelimited(Stream stream)
      {
        Message.HighlyStructuredMessage instance = new Message.HighlyStructuredMessage();
        Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.HighlyStructuredMessage DeserializeLength(Stream stream, int length)
      {
        Message.HighlyStructuredMessage instance = new Message.HighlyStructuredMessage();
        Message.HighlyStructuredMessage.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.HighlyStructuredMessage Deserialize(byte[] buffer)
      {
        Message.HighlyStructuredMessage instance = new Message.HighlyStructuredMessage();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.HighlyStructuredMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.HighlyStructuredMessage Deserialize(
        byte[] buffer,
        Message.HighlyStructuredMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.HighlyStructuredMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.HighlyStructuredMessage Deserialize(
        Stream stream,
        Message.HighlyStructuredMessage instance)
      {
        if (instance.Params == null)
          instance.Params = new List<string>();
        if (instance.LocalizableParams == null)
          instance.LocalizableParams = new List<Message.HighlyStructuredMessage.HSMLocalizableParameter>();
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_16;
            case 10:
              instance.Namespace = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.ElementName = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              instance.Params.Add(ProtocolParser.ReadString(stream));
              continue;
            case 34:
              instance.FallbackLg = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.FallbackLc = ProtocolParser.ReadString(stream);
              continue;
            case 50:
              instance.LocalizableParams.Add(Message.HighlyStructuredMessage.HSMLocalizableParameter.DeserializeLengthDelimited(stream));
              continue;
            case 58:
              instance.DeterministicLg = ProtocolParser.ReadString(stream);
              continue;
            case 66:
              instance.DeterministicLc = ProtocolParser.ReadString(stream);
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              if (key.Field != 0U)
              {
                ProtocolParser.SkipKey(stream, key);
                continue;
              }
              goto label_14;
          }
        }
label_14:
        throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_16:
        return instance;
      }

      public static Message.HighlyStructuredMessage DeserializeLengthDelimited(
        Stream stream,
        Message.HighlyStructuredMessage instance)
      {
        if (instance.Params == null)
          instance.Params = new List<string>();
        if (instance.LocalizableParams == null)
          instance.LocalizableParams = new List<Message.HighlyStructuredMessage.HSMLocalizableParameter>();
        long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 10:
              instance.Namespace = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.ElementName = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              instance.Params.Add(ProtocolParser.ReadString(stream));
              continue;
            case 34:
              instance.FallbackLg = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.FallbackLc = ProtocolParser.ReadString(stream);
              continue;
            case 50:
              instance.LocalizableParams.Add(Message.HighlyStructuredMessage.HSMLocalizableParameter.DeserializeLengthDelimited(stream));
              continue;
            case 58:
              instance.DeterministicLg = ProtocolParser.ReadString(stream);
              continue;
            case 66:
              instance.DeterministicLc = ProtocolParser.ReadString(stream);
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

      public static Message.HighlyStructuredMessage DeserializeLength(
        Stream stream,
        int length,
        Message.HighlyStructuredMessage instance)
      {
        if (instance.Params == null)
          instance.Params = new List<string>();
        if (instance.LocalizableParams == null)
          instance.LocalizableParams = new List<Message.HighlyStructuredMessage.HSMLocalizableParameter>();
        long num = stream.Position + (long) length;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 10:
              instance.Namespace = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.ElementName = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              instance.Params.Add(ProtocolParser.ReadString(stream));
              continue;
            case 34:
              instance.FallbackLg = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.FallbackLc = ProtocolParser.ReadString(stream);
              continue;
            case 50:
              instance.LocalizableParams.Add(Message.HighlyStructuredMessage.HSMLocalizableParameter.DeserializeLengthDelimited(stream));
              continue;
            case 58:
              instance.DeterministicLg = ProtocolParser.ReadString(stream);
              continue;
            case 66:
              instance.DeterministicLc = ProtocolParser.ReadString(stream);
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

      public static void Serialize(Stream stream, Message.HighlyStructuredMessage instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Namespace != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Namespace));
        }
        if (instance.ElementName != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.ElementName));
        }
        if (instance.Params != null)
        {
          foreach (string s in instance.Params)
          {
            stream.WriteByte((byte) 26);
            ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(s));
          }
        }
        if (instance.FallbackLg != null)
        {
          stream.WriteByte((byte) 34);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.FallbackLg));
        }
        if (instance.FallbackLc != null)
        {
          stream.WriteByte((byte) 42);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.FallbackLc));
        }
        if (instance.LocalizableParams != null)
        {
          foreach (Message.HighlyStructuredMessage.HSMLocalizableParameter localizableParam in instance.LocalizableParams)
          {
            stream.WriteByte((byte) 50);
            stream1.SetLength(0L);
            Message.HighlyStructuredMessage.HSMLocalizableParameter.Serialize((Stream) stream1, localizableParam);
            uint length = (uint) stream1.Length;
            ProtocolParser.WriteUInt32(stream, length);
            stream1.WriteTo(stream);
          }
        }
        if (instance.DeterministicLg != null)
        {
          stream.WriteByte((byte) 58);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.DeterministicLg));
        }
        if (instance.DeterministicLc != null)
        {
          stream.WriteByte((byte) 66);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.DeterministicLc));
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.HighlyStructuredMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.HighlyStructuredMessage.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        Message.HighlyStructuredMessage instance)
      {
        byte[] bytes = Message.HighlyStructuredMessage.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public MessageTypes MessageType => MessageTypes.highly_structured_message;

      public ContextInfo ContextInfo => (ContextInfo) null;

      public class HSMLocalizableParameter
      {
        public string Default { get; set; }

        public Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency Currency { get; set; }

        public Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime DateTime { get; set; }

        public static Message.HighlyStructuredMessage.HSMLocalizableParameter Deserialize(
          Stream stream)
        {
          Message.HighlyStructuredMessage.HSMLocalizableParameter instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter();
          Message.HighlyStructuredMessage.HSMLocalizableParameter.Deserialize(stream, instance);
          return instance;
        }

        public static Message.HighlyStructuredMessage.HSMLocalizableParameter DeserializeLengthDelimited(
          Stream stream)
        {
          Message.HighlyStructuredMessage.HSMLocalizableParameter instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter();
          Message.HighlyStructuredMessage.HSMLocalizableParameter.DeserializeLengthDelimited(stream, instance);
          return instance;
        }

        public static Message.HighlyStructuredMessage.HSMLocalizableParameter DeserializeLength(
          Stream stream,
          int length)
        {
          Message.HighlyStructuredMessage.HSMLocalizableParameter instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter();
          Message.HighlyStructuredMessage.HSMLocalizableParameter.DeserializeLength(stream, length, instance);
          return instance;
        }

        public static Message.HighlyStructuredMessage.HSMLocalizableParameter Deserialize(
          byte[] buffer)
        {
          Message.HighlyStructuredMessage.HSMLocalizableParameter instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter();
          using (MemoryStream memoryStream = new MemoryStream(buffer))
            Message.HighlyStructuredMessage.HSMLocalizableParameter.Deserialize((Stream) memoryStream, instance);
          return instance;
        }

        public static Message.HighlyStructuredMessage.HSMLocalizableParameter Deserialize(
          byte[] buffer,
          Message.HighlyStructuredMessage.HSMLocalizableParameter instance)
        {
          using (MemoryStream memoryStream = new MemoryStream(buffer))
            Message.HighlyStructuredMessage.HSMLocalizableParameter.Deserialize((Stream) memoryStream, instance);
          return instance;
        }

        public static Message.HighlyStructuredMessage.HSMLocalizableParameter Deserialize(
          Stream stream,
          Message.HighlyStructuredMessage.HSMLocalizableParameter instance)
        {
          while (true)
          {
            int firstByte = stream.ReadByte();
            switch (firstByte)
            {
              case -1:
                goto label_11;
              case 10:
                instance.Default = ProtocolParser.ReadString(stream);
                continue;
              case 18:
                if (instance.Currency == null)
                {
                  instance.Currency = Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency.DeserializeLengthDelimited(stream, instance.Currency);
                continue;
              case 26:
                if (instance.DateTime == null)
                {
                  instance.DateTime = Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.DeserializeLengthDelimited(stream, instance.DateTime);
                continue;
              default:
                SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
                if (key.Field != 0U)
                {
                  ProtocolParser.SkipKey(stream, key);
                  continue;
                }
                goto label_9;
            }
          }
label_9:
          throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_11:
          return instance;
        }

        public static Message.HighlyStructuredMessage.HSMLocalizableParameter DeserializeLengthDelimited(
          Stream stream,
          Message.HighlyStructuredMessage.HSMLocalizableParameter instance)
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
                instance.Default = ProtocolParser.ReadString(stream);
                continue;
              case 18:
                if (instance.Currency == null)
                {
                  instance.Currency = Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency.DeserializeLengthDelimited(stream, instance.Currency);
                continue;
              case 26:
                if (instance.DateTime == null)
                {
                  instance.DateTime = Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.DeserializeLengthDelimited(stream, instance.DateTime);
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

        public static Message.HighlyStructuredMessage.HSMLocalizableParameter DeserializeLength(
          Stream stream,
          int length,
          Message.HighlyStructuredMessage.HSMLocalizableParameter instance)
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
                instance.Default = ProtocolParser.ReadString(stream);
                continue;
              case 18:
                if (instance.Currency == null)
                {
                  instance.Currency = Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency.DeserializeLengthDelimited(stream, instance.Currency);
                continue;
              case 26:
                if (instance.DateTime == null)
                {
                  instance.DateTime = Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.DeserializeLengthDelimited(stream, instance.DateTime);
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

        public static void Serialize(
          Stream stream,
          Message.HighlyStructuredMessage.HSMLocalizableParameter instance)
        {
          MemoryStream stream1 = ProtocolParser.Stack.Pop();
          if (instance.Default != null)
          {
            stream.WriteByte((byte) 10);
            ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Default));
          }
          if (instance.Currency != null)
          {
            stream.WriteByte((byte) 18);
            stream1.SetLength(0L);
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency.Serialize((Stream) stream1, instance.Currency);
            uint length = (uint) stream1.Length;
            ProtocolParser.WriteUInt32(stream, length);
            stream1.WriteTo(stream);
          }
          if (instance.DateTime != null)
          {
            stream.WriteByte((byte) 26);
            stream1.SetLength(0L);
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.Serialize((Stream) stream1, instance.DateTime);
            uint length = (uint) stream1.Length;
            ProtocolParser.WriteUInt32(stream, length);
            stream1.WriteTo(stream);
          }
          ProtocolParser.Stack.Push(stream1);
        }

        public static byte[] SerializeToBytes(
          Message.HighlyStructuredMessage.HSMLocalizableParameter instance)
        {
          using (MemoryStream memoryStream = new MemoryStream())
          {
            Message.HighlyStructuredMessage.HSMLocalizableParameter.Serialize((Stream) memoryStream, instance);
            return memoryStream.ToArray();
          }
        }

        public static void SerializeLengthDelimited(
          Stream stream,
          Message.HighlyStructuredMessage.HSMLocalizableParameter instance)
        {
          byte[] bytes = Message.HighlyStructuredMessage.HSMLocalizableParameter.SerializeToBytes(instance);
          ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
          stream.Write(bytes, 0, bytes.Length);
        }

        public class HSMCurrency
        {
          public string CurrencyCode { get; set; }

          public long? Amount1000 { get; set; }

          public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency Deserialize(
            Stream stream)
          {
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency();
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency.Deserialize(stream, instance);
            return instance;
          }

          public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency DeserializeLengthDelimited(
            Stream stream)
          {
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency();
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency.DeserializeLengthDelimited(stream, instance);
            return instance;
          }

          public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency DeserializeLength(
            Stream stream,
            int length)
          {
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency();
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency.DeserializeLength(stream, length, instance);
            return instance;
          }

          public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency Deserialize(
            byte[] buffer)
          {
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency();
            using (MemoryStream memoryStream = new MemoryStream(buffer))
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency.Deserialize((Stream) memoryStream, instance);
            return instance;
          }

          public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency Deserialize(
            byte[] buffer,
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency instance)
          {
            using (MemoryStream memoryStream = new MemoryStream(buffer))
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency.Deserialize((Stream) memoryStream, instance);
            return instance;
          }

          public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency Deserialize(
            Stream stream,
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency instance)
          {
            while (true)
            {
              int firstByte = stream.ReadByte();
              switch (firstByte)
              {
                case -1:
                  goto label_6;
                case 10:
                  instance.CurrencyCode = ProtocolParser.ReadString(stream);
                  continue;
                case 16:
                  instance.Amount1000 = new long?((long) ProtocolParser.ReadUInt64(stream));
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

          public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency DeserializeLengthDelimited(
            Stream stream,
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency instance)
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
                  instance.CurrencyCode = ProtocolParser.ReadString(stream);
                  continue;
                case 16:
                  instance.Amount1000 = new long?((long) ProtocolParser.ReadUInt64(stream));
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

          public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency DeserializeLength(
            Stream stream,
            int length,
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency instance)
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
                  instance.CurrencyCode = ProtocolParser.ReadString(stream);
                  continue;
                case 16:
                  instance.Amount1000 = new long?((long) ProtocolParser.ReadUInt64(stream));
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

          public static void Serialize(
            Stream stream,
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency instance)
          {
            MemoryStream stream1 = ProtocolParser.Stack.Pop();
            if (instance.CurrencyCode != null)
            {
              stream.WriteByte((byte) 10);
              ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.CurrencyCode));
            }
            if (instance.Amount1000.HasValue)
            {
              stream.WriteByte((byte) 16);
              ProtocolParser.WriteUInt64(stream, (ulong) instance.Amount1000.Value);
            }
            ProtocolParser.Stack.Push(stream1);
          }

          public static byte[] SerializeToBytes(
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency instance)
          {
            using (MemoryStream memoryStream = new MemoryStream())
            {
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency.Serialize((Stream) memoryStream, instance);
              return memoryStream.ToArray();
            }
          }

          public static void SerializeLengthDelimited(
            Stream stream,
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency instance)
          {
            byte[] bytes = Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMCurrency.SerializeToBytes(instance);
            ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
            stream.Write(bytes, 0, bytes.Length);
          }
        }

        public class HSMDateTime
        {
          public Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent Component { get; set; }

          public Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch UnixEpoch { get; set; }

          public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime Deserialize(
            Stream stream)
          {
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime();
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.Deserialize(stream, instance);
            return instance;
          }

          public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime DeserializeLengthDelimited(
            Stream stream)
          {
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime();
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.DeserializeLengthDelimited(stream, instance);
            return instance;
          }

          public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime DeserializeLength(
            Stream stream,
            int length)
          {
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime();
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.DeserializeLength(stream, length, instance);
            return instance;
          }

          public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime Deserialize(
            byte[] buffer)
          {
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime();
            using (MemoryStream memoryStream = new MemoryStream(buffer))
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.Deserialize((Stream) memoryStream, instance);
            return instance;
          }

          public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime Deserialize(
            byte[] buffer,
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime instance)
          {
            using (MemoryStream memoryStream = new MemoryStream(buffer))
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.Deserialize((Stream) memoryStream, instance);
            return instance;
          }

          public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime Deserialize(
            Stream stream,
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime instance)
          {
            while (true)
            {
              int firstByte = stream.ReadByte();
              switch (firstByte)
              {
                case -1:
                  goto label_10;
                case 10:
                  if (instance.Component == null)
                  {
                    instance.Component = Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.DeserializeLengthDelimited(stream, instance.Component);
                  continue;
                case 18:
                  if (instance.UnixEpoch == null)
                  {
                    instance.UnixEpoch = Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch.DeserializeLengthDelimited(stream, instance.UnixEpoch);
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

          public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime DeserializeLengthDelimited(
            Stream stream,
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime instance)
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
                  if (instance.Component == null)
                  {
                    instance.Component = Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.DeserializeLengthDelimited(stream, instance.Component);
                  continue;
                case 18:
                  if (instance.UnixEpoch == null)
                  {
                    instance.UnixEpoch = Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch.DeserializeLengthDelimited(stream, instance.UnixEpoch);
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

          public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime DeserializeLength(
            Stream stream,
            int length,
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime instance)
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
                  if (instance.Component == null)
                  {
                    instance.Component = Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.DeserializeLengthDelimited(stream, instance.Component);
                  continue;
                case 18:
                  if (instance.UnixEpoch == null)
                  {
                    instance.UnixEpoch = Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch.DeserializeLengthDelimited(stream);
                    continue;
                  }
                  Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch.DeserializeLengthDelimited(stream, instance.UnixEpoch);
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

          public static void Serialize(
            Stream stream,
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime instance)
          {
            MemoryStream stream1 = ProtocolParser.Stack.Pop();
            if (instance.Component != null)
            {
              stream.WriteByte((byte) 10);
              stream1.SetLength(0L);
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.Serialize((Stream) stream1, instance.Component);
              uint length = (uint) stream1.Length;
              ProtocolParser.WriteUInt32(stream, length);
              stream1.WriteTo(stream);
            }
            if (instance.UnixEpoch != null)
            {
              stream.WriteByte((byte) 18);
              stream1.SetLength(0L);
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch.Serialize((Stream) stream1, instance.UnixEpoch);
              uint length = (uint) stream1.Length;
              ProtocolParser.WriteUInt32(stream, length);
              stream1.WriteTo(stream);
            }
            ProtocolParser.Stack.Push(stream1);
          }

          public static byte[] SerializeToBytes(
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime instance)
          {
            using (MemoryStream memoryStream = new MemoryStream())
            {
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.Serialize((Stream) memoryStream, instance);
              return memoryStream.ToArray();
            }
          }

          public static void SerializeLengthDelimited(
            Stream stream,
            Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime instance)
          {
            byte[] bytes = Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.SerializeToBytes(instance);
            ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
            stream.Write(bytes, 0, bytes.Length);
          }

          public class HSMDateTimeComponent
          {
            public Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.DayOfWeekType? DayOfWeek { get; set; }

            public uint? Year { get; set; }

            public uint? Month { get; set; }

            public uint? DayOfMonth { get; set; }

            public uint? Hour { get; set; }

            public uint? Minute { get; set; }

            public Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.CalendarType? Calendar { get; set; }

            public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent Deserialize(
              Stream stream)
            {
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent();
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.Deserialize(stream, instance);
              return instance;
            }

            public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent DeserializeLengthDelimited(
              Stream stream)
            {
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent();
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.DeserializeLengthDelimited(stream, instance);
              return instance;
            }

            public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent DeserializeLength(
              Stream stream,
              int length)
            {
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent();
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.DeserializeLength(stream, length, instance);
              return instance;
            }

            public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent Deserialize(
              byte[] buffer)
            {
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent();
              using (MemoryStream memoryStream = new MemoryStream(buffer))
                Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.Deserialize((Stream) memoryStream, instance);
              return instance;
            }

            public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent Deserialize(
              byte[] buffer,
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent instance)
            {
              using (MemoryStream memoryStream = new MemoryStream(buffer))
                Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.Deserialize((Stream) memoryStream, instance);
              return instance;
            }

            public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent Deserialize(
              Stream stream,
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent instance)
            {
              while (true)
              {
                int firstByte = stream.ReadByte();
                switch (firstByte)
                {
                  case -1:
                    goto label_11;
                  case 8:
                    instance.DayOfWeek = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.DayOfWeekType?((Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.DayOfWeekType) ProtocolParser.ReadUInt64(stream));
                    continue;
                  case 16:
                    instance.Year = new uint?(ProtocolParser.ReadUInt32(stream));
                    continue;
                  case 24:
                    instance.Month = new uint?(ProtocolParser.ReadUInt32(stream));
                    continue;
                  case 32:
                    instance.DayOfMonth = new uint?(ProtocolParser.ReadUInt32(stream));
                    continue;
                  case 40:
                    instance.Hour = new uint?(ProtocolParser.ReadUInt32(stream));
                    continue;
                  case 48:
                    instance.Minute = new uint?(ProtocolParser.ReadUInt32(stream));
                    continue;
                  case 56:
                    instance.Calendar = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.CalendarType?((Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.CalendarType) ProtocolParser.ReadUInt64(stream));
                    continue;
                  default:
                    SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
                    if (key.Field != 0U)
                    {
                      ProtocolParser.SkipKey(stream, key);
                      continue;
                    }
                    goto label_9;
                }
              }
label_9:
              throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_11:
              return instance;
            }

            public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent DeserializeLengthDelimited(
              Stream stream,
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent instance)
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
                    instance.DayOfWeek = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.DayOfWeekType?((Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.DayOfWeekType) ProtocolParser.ReadUInt64(stream));
                    continue;
                  case 16:
                    instance.Year = new uint?(ProtocolParser.ReadUInt32(stream));
                    continue;
                  case 24:
                    instance.Month = new uint?(ProtocolParser.ReadUInt32(stream));
                    continue;
                  case 32:
                    instance.DayOfMonth = new uint?(ProtocolParser.ReadUInt32(stream));
                    continue;
                  case 40:
                    instance.Hour = new uint?(ProtocolParser.ReadUInt32(stream));
                    continue;
                  case 48:
                    instance.Minute = new uint?(ProtocolParser.ReadUInt32(stream));
                    continue;
                  case 56:
                    instance.Calendar = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.CalendarType?((Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.CalendarType) ProtocolParser.ReadUInt64(stream));
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

            public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent DeserializeLength(
              Stream stream,
              int length,
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent instance)
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
                    instance.DayOfWeek = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.DayOfWeekType?((Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.DayOfWeekType) ProtocolParser.ReadUInt64(stream));
                    continue;
                  case 16:
                    instance.Year = new uint?(ProtocolParser.ReadUInt32(stream));
                    continue;
                  case 24:
                    instance.Month = new uint?(ProtocolParser.ReadUInt32(stream));
                    continue;
                  case 32:
                    instance.DayOfMonth = new uint?(ProtocolParser.ReadUInt32(stream));
                    continue;
                  case 40:
                    instance.Hour = new uint?(ProtocolParser.ReadUInt32(stream));
                    continue;
                  case 48:
                    instance.Minute = new uint?(ProtocolParser.ReadUInt32(stream));
                    continue;
                  case 56:
                    instance.Calendar = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.CalendarType?((Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.CalendarType) ProtocolParser.ReadUInt64(stream));
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

            public static void Serialize(
              Stream stream,
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent instance)
            {
              MemoryStream stream1 = ProtocolParser.Stack.Pop();
              if (instance.DayOfWeek.HasValue)
              {
                stream.WriteByte((byte) 8);
                ProtocolParser.WriteUInt64(stream, (ulong) instance.DayOfWeek.Value);
              }
              if (instance.Year.HasValue)
              {
                stream.WriteByte((byte) 16);
                ProtocolParser.WriteUInt32(stream, instance.Year.Value);
              }
              if (instance.Month.HasValue)
              {
                stream.WriteByte((byte) 24);
                ProtocolParser.WriteUInt32(stream, instance.Month.Value);
              }
              if (instance.DayOfMonth.HasValue)
              {
                stream.WriteByte((byte) 32);
                ProtocolParser.WriteUInt32(stream, instance.DayOfMonth.Value);
              }
              if (instance.Hour.HasValue)
              {
                stream.WriteByte((byte) 40);
                ProtocolParser.WriteUInt32(stream, instance.Hour.Value);
              }
              if (instance.Minute.HasValue)
              {
                stream.WriteByte((byte) 48);
                ProtocolParser.WriteUInt32(stream, instance.Minute.Value);
              }
              if (instance.Calendar.HasValue)
              {
                stream.WriteByte((byte) 56);
                ProtocolParser.WriteUInt64(stream, (ulong) instance.Calendar.Value);
              }
              ProtocolParser.Stack.Push(stream1);
            }

            public static byte[] SerializeToBytes(
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent instance)
            {
              using (MemoryStream memoryStream = new MemoryStream())
              {
                Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.Serialize((Stream) memoryStream, instance);
                return memoryStream.ToArray();
              }
            }

            public static void SerializeLengthDelimited(
              Stream stream,
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent instance)
            {
              byte[] bytes = Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeComponent.SerializeToBytes(instance);
              ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
              stream.Write(bytes, 0, bytes.Length);
            }

            public enum DayOfWeekType
            {
              MONDAY = 1,
              TUESDAY = 2,
              WEDNESDAY = 3,
              THURSDAY = 4,
              FRIDAY = 5,
              SATURDAY = 6,
              SUNDAY = 7,
            }

            public enum CalendarType
            {
              GREGORIAN = 1,
              SOLAR_HIJRI = 2,
            }
          }

          public class HSMDateTimeUnixEpoch
          {
            public long? Timestamp { get; set; }

            public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch Deserialize(
              Stream stream)
            {
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch();
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch.Deserialize(stream, instance);
              return instance;
            }

            public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch DeserializeLengthDelimited(
              Stream stream)
            {
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch();
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch.DeserializeLengthDelimited(stream, instance);
              return instance;
            }

            public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch DeserializeLength(
              Stream stream,
              int length)
            {
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch();
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch.DeserializeLength(stream, length, instance);
              return instance;
            }

            public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch Deserialize(
              byte[] buffer)
            {
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch instance = new Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch();
              using (MemoryStream memoryStream = new MemoryStream(buffer))
                Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch.Deserialize((Stream) memoryStream, instance);
              return instance;
            }

            public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch Deserialize(
              byte[] buffer,
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch instance)
            {
              using (MemoryStream memoryStream = new MemoryStream(buffer))
                Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch.Deserialize((Stream) memoryStream, instance);
              return instance;
            }

            public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch Deserialize(
              Stream stream,
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch instance)
            {
              while (true)
              {
                int firstByte = stream.ReadByte();
                switch (firstByte)
                {
                  case -1:
                    goto label_5;
                  case 8:
                    instance.Timestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
                    continue;
                  default:
                    SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
                    if (key.Field != 0U)
                    {
                      ProtocolParser.SkipKey(stream, key);
                      continue;
                    }
                    goto label_3;
                }
              }
label_3:
              throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_5:
              return instance;
            }

            public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch DeserializeLengthDelimited(
              Stream stream,
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch instance)
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
                    instance.Timestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
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

            public static Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch DeserializeLength(
              Stream stream,
              int length,
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch instance)
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
                    instance.Timestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
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

            public static void Serialize(
              Stream stream,
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch instance)
            {
              MemoryStream stream1 = ProtocolParser.Stack.Pop();
              if (instance.Timestamp.HasValue)
              {
                stream.WriteByte((byte) 8);
                ProtocolParser.WriteUInt64(stream, (ulong) instance.Timestamp.Value);
              }
              ProtocolParser.Stack.Push(stream1);
            }

            public static byte[] SerializeToBytes(
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch instance)
            {
              using (MemoryStream memoryStream = new MemoryStream())
              {
                Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch.Serialize((Stream) memoryStream, instance);
                return memoryStream.ToArray();
              }
            }

            public static void SerializeLengthDelimited(
              Stream stream,
              Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch instance)
            {
              byte[] bytes = Message.HighlyStructuredMessage.HSMLocalizableParameter.HSMDateTime.HSMDateTimeUnixEpoch.SerializeToBytes(instance);
              ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
              stream.Write(bytes, 0, bytes.Length);
            }
          }
        }
      }
    }

    public class SendPaymentMessage : IProtoBufMessage
    {
      public Message NoteMessage { get; set; }

      public MessageKey RequestMessageKey { get; set; }

      public static Message.SendPaymentMessage Deserialize(Stream stream)
      {
        Message.SendPaymentMessage instance = new Message.SendPaymentMessage();
        Message.SendPaymentMessage.Deserialize(stream, instance);
        return instance;
      }

      public static Message.SendPaymentMessage DeserializeLengthDelimited(Stream stream)
      {
        Message.SendPaymentMessage instance = new Message.SendPaymentMessage();
        Message.SendPaymentMessage.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.SendPaymentMessage DeserializeLength(Stream stream, int length)
      {
        Message.SendPaymentMessage instance = new Message.SendPaymentMessage();
        Message.SendPaymentMessage.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.SendPaymentMessage Deserialize(byte[] buffer)
      {
        Message.SendPaymentMessage instance = new Message.SendPaymentMessage();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.SendPaymentMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.SendPaymentMessage Deserialize(
        byte[] buffer,
        Message.SendPaymentMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.SendPaymentMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.SendPaymentMessage Deserialize(
        Stream stream,
        Message.SendPaymentMessage instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_10;
            case 18:
              if (instance.NoteMessage == null)
              {
                instance.NoteMessage = Message.DeserializeLengthDelimited(stream);
                continue;
              }
              Message.DeserializeLengthDelimited(stream, instance.NoteMessage);
              continue;
            case 26:
              if (instance.RequestMessageKey == null)
              {
                instance.RequestMessageKey = MessageKey.DeserializeLengthDelimited(stream);
                continue;
              }
              MessageKey.DeserializeLengthDelimited(stream, instance.RequestMessageKey);
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

      public static Message.SendPaymentMessage DeserializeLengthDelimited(
        Stream stream,
        Message.SendPaymentMessage instance)
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
              if (instance.NoteMessage == null)
              {
                instance.NoteMessage = Message.DeserializeLengthDelimited(stream);
                continue;
              }
              Message.DeserializeLengthDelimited(stream, instance.NoteMessage);
              continue;
            case 26:
              if (instance.RequestMessageKey == null)
              {
                instance.RequestMessageKey = MessageKey.DeserializeLengthDelimited(stream);
                continue;
              }
              MessageKey.DeserializeLengthDelimited(stream, instance.RequestMessageKey);
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

      public static Message.SendPaymentMessage DeserializeLength(
        Stream stream,
        int length,
        Message.SendPaymentMessage instance)
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
              if (instance.NoteMessage == null)
              {
                instance.NoteMessage = Message.DeserializeLengthDelimited(stream);
                continue;
              }
              Message.DeserializeLengthDelimited(stream, instance.NoteMessage);
              continue;
            case 26:
              if (instance.RequestMessageKey == null)
              {
                instance.RequestMessageKey = MessageKey.DeserializeLengthDelimited(stream);
                continue;
              }
              MessageKey.DeserializeLengthDelimited(stream, instance.RequestMessageKey);
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

      public static void Serialize(Stream stream, Message.SendPaymentMessage instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.NoteMessage != null)
        {
          stream.WriteByte((byte) 18);
          stream1.SetLength(0L);
          Message.Serialize((Stream) stream1, instance.NoteMessage);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        if (instance.RequestMessageKey != null)
        {
          stream.WriteByte((byte) 26);
          stream1.SetLength(0L);
          MessageKey.Serialize((Stream) stream1, instance.RequestMessageKey);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.SendPaymentMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.SendPaymentMessage.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        Message.SendPaymentMessage instance)
      {
        byte[] bytes = Message.SendPaymentMessage.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public MessageTypes MessageType => MessageTypes.send_payments_message;

      public ContextInfo ContextInfo => (ContextInfo) null;
    }

    public class RequestPaymentMessage : IProtoBufMessage
    {
      public Message NoteMessage { get; set; }

      public string CurrencyCodeIso4217 { get; set; }

      public ulong? Amount1000 { get; set; }

      public string RequestFrom { get; set; }

      public long? ExpiryTimestamp { get; set; }

      public static Message.RequestPaymentMessage Deserialize(Stream stream)
      {
        Message.RequestPaymentMessage instance = new Message.RequestPaymentMessage();
        Message.RequestPaymentMessage.Deserialize(stream, instance);
        return instance;
      }

      public static Message.RequestPaymentMessage DeserializeLengthDelimited(Stream stream)
      {
        Message.RequestPaymentMessage instance = new Message.RequestPaymentMessage();
        Message.RequestPaymentMessage.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.RequestPaymentMessage DeserializeLength(Stream stream, int length)
      {
        Message.RequestPaymentMessage instance = new Message.RequestPaymentMessage();
        Message.RequestPaymentMessage.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.RequestPaymentMessage Deserialize(byte[] buffer)
      {
        Message.RequestPaymentMessage instance = new Message.RequestPaymentMessage();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.RequestPaymentMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.RequestPaymentMessage Deserialize(
        byte[] buffer,
        Message.RequestPaymentMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.RequestPaymentMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.RequestPaymentMessage Deserialize(
        Stream stream,
        Message.RequestPaymentMessage instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_11;
            case 10:
              instance.CurrencyCodeIso4217 = ProtocolParser.ReadString(stream);
              continue;
            case 16:
              instance.Amount1000 = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 26:
              instance.RequestFrom = ProtocolParser.ReadString(stream);
              continue;
            case 34:
              if (instance.NoteMessage == null)
              {
                instance.NoteMessage = Message.DeserializeLengthDelimited(stream);
                continue;
              }
              Message.DeserializeLengthDelimited(stream, instance.NoteMessage);
              continue;
            case 40:
              instance.ExpiryTimestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              if (key.Field != 0U)
              {
                ProtocolParser.SkipKey(stream, key);
                continue;
              }
              goto label_9;
          }
        }
label_9:
        throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_11:
        return instance;
      }

      public static Message.RequestPaymentMessage DeserializeLengthDelimited(
        Stream stream,
        Message.RequestPaymentMessage instance)
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
              instance.CurrencyCodeIso4217 = ProtocolParser.ReadString(stream);
              continue;
            case 16:
              instance.Amount1000 = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 26:
              instance.RequestFrom = ProtocolParser.ReadString(stream);
              continue;
            case 34:
              if (instance.NoteMessage == null)
              {
                instance.NoteMessage = Message.DeserializeLengthDelimited(stream);
                continue;
              }
              Message.DeserializeLengthDelimited(stream, instance.NoteMessage);
              continue;
            case 40:
              instance.ExpiryTimestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
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

      public static Message.RequestPaymentMessage DeserializeLength(
        Stream stream,
        int length,
        Message.RequestPaymentMessage instance)
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
              instance.CurrencyCodeIso4217 = ProtocolParser.ReadString(stream);
              continue;
            case 16:
              instance.Amount1000 = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 26:
              instance.RequestFrom = ProtocolParser.ReadString(stream);
              continue;
            case 34:
              if (instance.NoteMessage == null)
              {
                instance.NoteMessage = Message.DeserializeLengthDelimited(stream);
                continue;
              }
              Message.DeserializeLengthDelimited(stream, instance.NoteMessage);
              continue;
            case 40:
              instance.ExpiryTimestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
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

      public static void Serialize(Stream stream, Message.RequestPaymentMessage instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.NoteMessage != null)
        {
          stream.WriteByte((byte) 34);
          stream1.SetLength(0L);
          Message.Serialize((Stream) stream1, instance.NoteMessage);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        if (instance.CurrencyCodeIso4217 != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.CurrencyCodeIso4217));
        }
        if (instance.Amount1000.HasValue)
        {
          stream.WriteByte((byte) 16);
          ProtocolParser.WriteUInt64(stream, instance.Amount1000.Value);
        }
        if (instance.RequestFrom != null)
        {
          stream.WriteByte((byte) 26);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.RequestFrom));
        }
        if (instance.ExpiryTimestamp.HasValue)
        {
          stream.WriteByte((byte) 40);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.ExpiryTimestamp.Value);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.RequestPaymentMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.RequestPaymentMessage.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        Message.RequestPaymentMessage instance)
      {
        byte[] bytes = Message.RequestPaymentMessage.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public MessageTypes MessageType => MessageTypes.request_payment_message;

      public ContextInfo ContextInfo => (ContextInfo) null;
    }

    public class DeclinePaymentRequestMessage
    {
      public MessageKey Key { get; set; }

      public static Message.DeclinePaymentRequestMessage Deserialize(Stream stream)
      {
        Message.DeclinePaymentRequestMessage instance = new Message.DeclinePaymentRequestMessage();
        Message.DeclinePaymentRequestMessage.Deserialize(stream, instance);
        return instance;
      }

      public static Message.DeclinePaymentRequestMessage DeserializeLengthDelimited(Stream stream)
      {
        Message.DeclinePaymentRequestMessage instance = new Message.DeclinePaymentRequestMessage();
        Message.DeclinePaymentRequestMessage.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.DeclinePaymentRequestMessage DeserializeLength(
        Stream stream,
        int length)
      {
        Message.DeclinePaymentRequestMessage instance = new Message.DeclinePaymentRequestMessage();
        Message.DeclinePaymentRequestMessage.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.DeclinePaymentRequestMessage Deserialize(byte[] buffer)
      {
        Message.DeclinePaymentRequestMessage instance = new Message.DeclinePaymentRequestMessage();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.DeclinePaymentRequestMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.DeclinePaymentRequestMessage Deserialize(
        byte[] buffer,
        Message.DeclinePaymentRequestMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.DeclinePaymentRequestMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.DeclinePaymentRequestMessage Deserialize(
        Stream stream,
        Message.DeclinePaymentRequestMessage instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_7;
            case 10:
              if (instance.Key == null)
              {
                instance.Key = MessageKey.DeserializeLengthDelimited(stream);
                continue;
              }
              MessageKey.DeserializeLengthDelimited(stream, instance.Key);
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

      public static Message.DeclinePaymentRequestMessage DeserializeLengthDelimited(
        Stream stream,
        Message.DeclinePaymentRequestMessage instance)
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
              if (instance.Key == null)
              {
                instance.Key = MessageKey.DeserializeLengthDelimited(stream);
                continue;
              }
              MessageKey.DeserializeLengthDelimited(stream, instance.Key);
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

      public static Message.DeclinePaymentRequestMessage DeserializeLength(
        Stream stream,
        int length,
        Message.DeclinePaymentRequestMessage instance)
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
              if (instance.Key == null)
              {
                instance.Key = MessageKey.DeserializeLengthDelimited(stream);
                continue;
              }
              MessageKey.DeserializeLengthDelimited(stream, instance.Key);
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

      public static void Serialize(Stream stream, Message.DeclinePaymentRequestMessage instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Key != null)
        {
          stream.WriteByte((byte) 10);
          stream1.SetLength(0L);
          MessageKey.Serialize((Stream) stream1, instance.Key);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.DeclinePaymentRequestMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.DeclinePaymentRequestMessage.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        Message.DeclinePaymentRequestMessage instance)
      {
        byte[] bytes = Message.DeclinePaymentRequestMessage.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }
    }

    public class CancelPaymentRequestMessage
    {
      public MessageKey Key { get; set; }

      public static Message.CancelPaymentRequestMessage Deserialize(Stream stream)
      {
        Message.CancelPaymentRequestMessage instance = new Message.CancelPaymentRequestMessage();
        Message.CancelPaymentRequestMessage.Deserialize(stream, instance);
        return instance;
      }

      public static Message.CancelPaymentRequestMessage DeserializeLengthDelimited(Stream stream)
      {
        Message.CancelPaymentRequestMessage instance = new Message.CancelPaymentRequestMessage();
        Message.CancelPaymentRequestMessage.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.CancelPaymentRequestMessage DeserializeLength(Stream stream, int length)
      {
        Message.CancelPaymentRequestMessage instance = new Message.CancelPaymentRequestMessage();
        Message.CancelPaymentRequestMessage.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.CancelPaymentRequestMessage Deserialize(byte[] buffer)
      {
        Message.CancelPaymentRequestMessage instance = new Message.CancelPaymentRequestMessage();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.CancelPaymentRequestMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.CancelPaymentRequestMessage Deserialize(
        byte[] buffer,
        Message.CancelPaymentRequestMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.CancelPaymentRequestMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.CancelPaymentRequestMessage Deserialize(
        Stream stream,
        Message.CancelPaymentRequestMessage instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_7;
            case 10:
              if (instance.Key == null)
              {
                instance.Key = MessageKey.DeserializeLengthDelimited(stream);
                continue;
              }
              MessageKey.DeserializeLengthDelimited(stream, instance.Key);
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

      public static Message.CancelPaymentRequestMessage DeserializeLengthDelimited(
        Stream stream,
        Message.CancelPaymentRequestMessage instance)
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
              if (instance.Key == null)
              {
                instance.Key = MessageKey.DeserializeLengthDelimited(stream);
                continue;
              }
              MessageKey.DeserializeLengthDelimited(stream, instance.Key);
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

      public static Message.CancelPaymentRequestMessage DeserializeLength(
        Stream stream,
        int length,
        Message.CancelPaymentRequestMessage instance)
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
              if (instance.Key == null)
              {
                instance.Key = MessageKey.DeserializeLengthDelimited(stream);
                continue;
              }
              MessageKey.DeserializeLengthDelimited(stream, instance.Key);
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

      public static void Serialize(Stream stream, Message.CancelPaymentRequestMessage instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Key != null)
        {
          stream.WriteByte((byte) 10);
          stream1.SetLength(0L);
          MessageKey.Serialize((Stream) stream1, instance.Key);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.CancelPaymentRequestMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.CancelPaymentRequestMessage.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        Message.CancelPaymentRequestMessage instance)
      {
        byte[] bytes = Message.CancelPaymentRequestMessage.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }
    }

    public class LiveLocationMessage : IProtoBufMessage
    {
      public double? DegreesLatitude { get; set; }

      public double? DegreesLongitude { get; set; }

      public uint? AccuracyInMeters { get; set; }

      public float? SpeedInMps { get; set; }

      public uint? DegreesClockwiseFromMagneticNorth { get; set; }

      public string Caption { get; set; }

      public long? SequenceNumber { get; set; }

      public uint? TimeOffset { get; set; }

      public byte[] JpegThumbnail { get; set; }

      public ContextInfo ContextInfo { get; set; }

      public static Message.LiveLocationMessage Deserialize(Stream stream)
      {
        Message.LiveLocationMessage instance = new Message.LiveLocationMessage();
        Message.LiveLocationMessage.Deserialize(stream, instance);
        return instance;
      }

      public static Message.LiveLocationMessage DeserializeLengthDelimited(Stream stream)
      {
        Message.LiveLocationMessage instance = new Message.LiveLocationMessage();
        Message.LiveLocationMessage.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.LiveLocationMessage DeserializeLength(Stream stream, int length)
      {
        Message.LiveLocationMessage instance = new Message.LiveLocationMessage();
        Message.LiveLocationMessage.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.LiveLocationMessage Deserialize(byte[] buffer)
      {
        Message.LiveLocationMessage instance = new Message.LiveLocationMessage();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.LiveLocationMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.LiveLocationMessage Deserialize(
        byte[] buffer,
        Message.LiveLocationMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.LiveLocationMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.LiveLocationMessage Deserialize(
        Stream stream,
        Message.LiveLocationMessage instance)
      {
        BinaryReader binaryReader = new BinaryReader(stream);
        while (true)
        {
          SilentOrbit.ProtocolBuffers.Key key;
          do
          {
            do
            {
              int firstByte = stream.ReadByte();
              switch (firstByte)
              {
                case -1:
                  goto label_19;
                case 9:
                  instance.DegreesLatitude = new double?(binaryReader.ReadDouble());
                  continue;
                case 17:
                  instance.DegreesLongitude = new double?(binaryReader.ReadDouble());
                  continue;
                case 24:
                  instance.AccuracyInMeters = new uint?(ProtocolParser.ReadUInt32(stream));
                  continue;
                case 37:
                  instance.SpeedInMps = new float?(binaryReader.ReadSingle());
                  continue;
                case 40:
                  instance.DegreesClockwiseFromMagneticNorth = new uint?(ProtocolParser.ReadUInt32(stream));
                  continue;
                case 50:
                  instance.Caption = ProtocolParser.ReadString(stream);
                  continue;
                case 56:
                  instance.SequenceNumber = new long?((long) ProtocolParser.ReadUInt64(stream));
                  continue;
                case 64:
                  instance.TimeOffset = new uint?(ProtocolParser.ReadUInt32(stream));
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
                      goto label_14;
                    default:
                      goto label_18;
                  }
              }
            }
            while (key.WireType != Wire.LengthDelimited);
            instance.JpegThumbnail = ProtocolParser.ReadBytes(stream);
            continue;
label_14:;
          }
          while (key.WireType != Wire.LengthDelimited);
          if (instance.ContextInfo == null)
          {
            instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
            continue;
          }
          ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
          continue;
label_18:
          ProtocolParser.SkipKey(stream, key);
        }
label_19:
        return instance;
      }

      public static Message.LiveLocationMessage DeserializeLengthDelimited(
        Stream stream,
        Message.LiveLocationMessage instance)
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
            case 24:
              instance.AccuracyInMeters = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 37:
              instance.SpeedInMps = new float?(binaryReader.ReadSingle());
              continue;
            case 40:
              instance.DegreesClockwiseFromMagneticNorth = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 50:
              instance.Caption = ProtocolParser.ReadString(stream);
              continue;
            case 56:
              instance.SequenceNumber = new long?((long) ProtocolParser.ReadUInt64(stream));
              continue;
            case 64:
              instance.TimeOffset = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.JpegThumbnail = ProtocolParser.ReadBytes(stream);
                    continue;
                  }
                  continue;
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
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

      public static Message.LiveLocationMessage DeserializeLength(
        Stream stream,
        int length,
        Message.LiveLocationMessage instance)
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
            case 24:
              instance.AccuracyInMeters = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 37:
              instance.SpeedInMps = new float?(binaryReader.ReadSingle());
              continue;
            case 40:
              instance.DegreesClockwiseFromMagneticNorth = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 50:
              instance.Caption = ProtocolParser.ReadString(stream);
              continue;
            case 56:
              instance.SequenceNumber = new long?((long) ProtocolParser.ReadUInt64(stream));
              continue;
            case 64:
              instance.TimeOffset = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.JpegThumbnail = ProtocolParser.ReadBytes(stream);
                    continue;
                  }
                  continue;
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
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

      public static void Serialize(Stream stream, Message.LiveLocationMessage instance)
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
        if (instance.AccuracyInMeters.HasValue)
        {
          stream.WriteByte((byte) 24);
          ProtocolParser.WriteUInt32(stream, instance.AccuracyInMeters.Value);
        }
        if (instance.SpeedInMps.HasValue)
        {
          stream.WriteByte((byte) 37);
          binaryWriter.Write(instance.SpeedInMps.Value);
        }
        if (instance.DegreesClockwiseFromMagneticNorth.HasValue)
        {
          stream.WriteByte((byte) 40);
          ProtocolParser.WriteUInt32(stream, instance.DegreesClockwiseFromMagneticNorth.Value);
        }
        if (instance.Caption != null)
        {
          stream.WriteByte((byte) 50);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Caption));
        }
        if (instance.SequenceNumber.HasValue)
        {
          stream.WriteByte((byte) 56);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.SequenceNumber.Value);
        }
        if (instance.TimeOffset.HasValue)
        {
          stream.WriteByte((byte) 64);
          ProtocolParser.WriteUInt32(stream, instance.TimeOffset.Value);
        }
        if (instance.JpegThumbnail != null)
        {
          stream.WriteByte((byte) 130);
          stream.WriteByte((byte) 1);
          ProtocolParser.WriteBytes(stream, instance.JpegThumbnail);
        }
        if (instance.ContextInfo != null)
        {
          stream.WriteByte((byte) 138);
          stream.WriteByte((byte) 1);
          stream1.SetLength(0L);
          ContextInfo.Serialize((Stream) stream1, instance.ContextInfo);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.LiveLocationMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.LiveLocationMessage.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        Message.LiveLocationMessage instance)
      {
        byte[] bytes = Message.LiveLocationMessage.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public MessageTypes MessageType => MessageTypes.live_location_message;
    }

    public class StickerMessage : IProtoBufMessage
    {
      public string Url { get; set; }

      public byte[] FileSha256 { get; set; }

      public byte[] FileEncSha256 { get; set; }

      public byte[] MediaKey { get; set; }

      public string Mimetype { get; set; }

      public uint? Height { get; set; }

      public uint? Width { get; set; }

      public string DirectPath { get; set; }

      public ulong? FileLength { get; set; }

      public long? MediaKeyTimestamp { get; set; }

      public byte[] PngThumbnail { get; set; }

      public ContextInfo ContextInfo { get; set; }

      public static Message.StickerMessage Deserialize(Stream stream)
      {
        Message.StickerMessage instance = new Message.StickerMessage();
        Message.StickerMessage.Deserialize(stream, instance);
        return instance;
      }

      public static Message.StickerMessage DeserializeLengthDelimited(Stream stream)
      {
        Message.StickerMessage instance = new Message.StickerMessage();
        Message.StickerMessage.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.StickerMessage DeserializeLength(Stream stream, int length)
      {
        Message.StickerMessage instance = new Message.StickerMessage();
        Message.StickerMessage.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.StickerMessage Deserialize(byte[] buffer)
      {
        Message.StickerMessage instance = new Message.StickerMessage();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.StickerMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.StickerMessage Deserialize(
        byte[] buffer,
        Message.StickerMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.StickerMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.StickerMessage Deserialize(
        Stream stream,
        Message.StickerMessage instance)
      {
        while (true)
        {
          SilentOrbit.ProtocolBuffers.Key key;
          do
          {
            do
            {
              int firstByte = stream.ReadByte();
              switch (firstByte)
              {
                case -1:
                  goto label_20;
                case 10:
                  instance.Url = ProtocolParser.ReadString(stream);
                  continue;
                case 18:
                  instance.FileSha256 = ProtocolParser.ReadBytes(stream);
                  continue;
                case 26:
                  instance.FileEncSha256 = ProtocolParser.ReadBytes(stream);
                  continue;
                case 34:
                  instance.MediaKey = ProtocolParser.ReadBytes(stream);
                  continue;
                case 42:
                  instance.Mimetype = ProtocolParser.ReadString(stream);
                  continue;
                case 48:
                  instance.Height = new uint?(ProtocolParser.ReadUInt32(stream));
                  continue;
                case 56:
                  instance.Width = new uint?(ProtocolParser.ReadUInt32(stream));
                  continue;
                case 66:
                  instance.DirectPath = ProtocolParser.ReadString(stream);
                  continue;
                case 72:
                  instance.FileLength = new ulong?(ProtocolParser.ReadUInt64(stream));
                  continue;
                case 80:
                  instance.MediaKeyTimestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
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
                      goto label_15;
                    default:
                      goto label_19;
                  }
              }
            }
            while (key.WireType != Wire.LengthDelimited);
            instance.PngThumbnail = ProtocolParser.ReadBytes(stream);
            continue;
label_15:;
          }
          while (key.WireType != Wire.LengthDelimited);
          if (instance.ContextInfo == null)
          {
            instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
            continue;
          }
          ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
          continue;
label_19:
          ProtocolParser.SkipKey(stream, key);
        }
label_20:
        return instance;
      }

      public static Message.StickerMessage DeserializeLengthDelimited(
        Stream stream,
        Message.StickerMessage instance)
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
              instance.Url = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.FileSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 26:
              instance.FileEncSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 34:
              instance.MediaKey = ProtocolParser.ReadBytes(stream);
              continue;
            case 42:
              instance.Mimetype = ProtocolParser.ReadString(stream);
              continue;
            case 48:
              instance.Height = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 56:
              instance.Width = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 66:
              instance.DirectPath = ProtocolParser.ReadString(stream);
              continue;
            case 72:
              instance.FileLength = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 80:
              instance.MediaKeyTimestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.PngThumbnail = ProtocolParser.ReadBytes(stream);
                    continue;
                  }
                  continue;
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
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

      public static Message.StickerMessage DeserializeLength(
        Stream stream,
        int length,
        Message.StickerMessage instance)
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
              instance.Url = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.FileSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 26:
              instance.FileEncSha256 = ProtocolParser.ReadBytes(stream);
              continue;
            case 34:
              instance.MediaKey = ProtocolParser.ReadBytes(stream);
              continue;
            case 42:
              instance.Mimetype = ProtocolParser.ReadString(stream);
              continue;
            case 48:
              instance.Height = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 56:
              instance.Width = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 66:
              instance.DirectPath = ProtocolParser.ReadString(stream);
              continue;
            case 72:
              instance.FileLength = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 80:
              instance.MediaKeyTimestamp = new long?((long) ProtocolParser.ReadUInt64(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              switch (key.Field)
              {
                case 0:
                  throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
                case 16:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    instance.PngThumbnail = ProtocolParser.ReadBytes(stream);
                    continue;
                  }
                  continue;
                case 17:
                  if (key.WireType == Wire.LengthDelimited)
                  {
                    if (instance.ContextInfo == null)
                    {
                      instance.ContextInfo = ContextInfo.DeserializeLengthDelimited(stream);
                      continue;
                    }
                    ContextInfo.DeserializeLengthDelimited(stream, instance.ContextInfo);
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

      public static void Serialize(Stream stream, Message.StickerMessage instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Url != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Url));
        }
        if (instance.FileSha256 != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, instance.FileSha256);
        }
        if (instance.FileEncSha256 != null)
        {
          stream.WriteByte((byte) 26);
          ProtocolParser.WriteBytes(stream, instance.FileEncSha256);
        }
        if (instance.MediaKey != null)
        {
          stream.WriteByte((byte) 34);
          ProtocolParser.WriteBytes(stream, instance.MediaKey);
        }
        if (instance.Mimetype != null)
        {
          stream.WriteByte((byte) 42);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Mimetype));
        }
        if (instance.Height.HasValue)
        {
          stream.WriteByte((byte) 48);
          ProtocolParser.WriteUInt32(stream, instance.Height.Value);
        }
        if (instance.Width.HasValue)
        {
          stream.WriteByte((byte) 56);
          ProtocolParser.WriteUInt32(stream, instance.Width.Value);
        }
        if (instance.DirectPath != null)
        {
          stream.WriteByte((byte) 66);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.DirectPath));
        }
        if (instance.FileLength.HasValue)
        {
          stream.WriteByte((byte) 72);
          ProtocolParser.WriteUInt64(stream, instance.FileLength.Value);
        }
        if (instance.MediaKeyTimestamp.HasValue)
        {
          stream.WriteByte((byte) 80);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.MediaKeyTimestamp.Value);
        }
        if (instance.PngThumbnail != null)
        {
          stream.WriteByte((byte) 130);
          stream.WriteByte((byte) 1);
          ProtocolParser.WriteBytes(stream, instance.PngThumbnail);
        }
        if (instance.ContextInfo != null)
        {
          stream.WriteByte((byte) 138);
          stream.WriteByte((byte) 1);
          stream1.SetLength(0L);
          ContextInfo.Serialize((Stream) stream1, instance.ContextInfo);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.StickerMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.StickerMessage.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(Stream stream, Message.StickerMessage instance)
      {
        byte[] bytes = Message.StickerMessage.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public MessageTypes MessageType => MessageTypes.sticker_message;
    }

    public class TemplateMessage
    {
      public Message.TemplateMessage.FourRowTemplate FourRowTemplateField { get; set; }

      public static Message.TemplateMessage Deserialize(Stream stream)
      {
        Message.TemplateMessage instance = new Message.TemplateMessage();
        Message.TemplateMessage.Deserialize(stream, instance);
        return instance;
      }

      public static Message.TemplateMessage DeserializeLengthDelimited(Stream stream)
      {
        Message.TemplateMessage instance = new Message.TemplateMessage();
        Message.TemplateMessage.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static Message.TemplateMessage DeserializeLength(Stream stream, int length)
      {
        Message.TemplateMessage instance = new Message.TemplateMessage();
        Message.TemplateMessage.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static Message.TemplateMessage Deserialize(byte[] buffer)
      {
        Message.TemplateMessage instance = new Message.TemplateMessage();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.TemplateMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.TemplateMessage Deserialize(
        byte[] buffer,
        Message.TemplateMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          Message.TemplateMessage.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static Message.TemplateMessage Deserialize(
        Stream stream,
        Message.TemplateMessage instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_7;
            case 10:
              if (instance.FourRowTemplateField == null)
              {
                instance.FourRowTemplateField = Message.TemplateMessage.FourRowTemplate.DeserializeLengthDelimited(stream);
                continue;
              }
              Message.TemplateMessage.FourRowTemplate.DeserializeLengthDelimited(stream, instance.FourRowTemplateField);
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

      public static Message.TemplateMessage DeserializeLengthDelimited(
        Stream stream,
        Message.TemplateMessage instance)
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
              if (instance.FourRowTemplateField == null)
              {
                instance.FourRowTemplateField = Message.TemplateMessage.FourRowTemplate.DeserializeLengthDelimited(stream);
                continue;
              }
              Message.TemplateMessage.FourRowTemplate.DeserializeLengthDelimited(stream, instance.FourRowTemplateField);
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

      public static Message.TemplateMessage DeserializeLength(
        Stream stream,
        int length,
        Message.TemplateMessage instance)
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
              if (instance.FourRowTemplateField == null)
              {
                instance.FourRowTemplateField = Message.TemplateMessage.FourRowTemplate.DeserializeLengthDelimited(stream);
                continue;
              }
              Message.TemplateMessage.FourRowTemplate.DeserializeLengthDelimited(stream, instance.FourRowTemplateField);
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

      public static void Serialize(Stream stream, Message.TemplateMessage instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.FourRowTemplateField != null)
        {
          stream.WriteByte((byte) 10);
          stream1.SetLength(0L);
          Message.TemplateMessage.FourRowTemplate.Serialize((Stream) stream1, instance.FourRowTemplateField);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(Message.TemplateMessage instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          Message.TemplateMessage.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(Stream stream, Message.TemplateMessage instance)
      {
        byte[] bytes = Message.TemplateMessage.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public class FourRowTemplate
      {
        public Message.DocumentMessage DocumentMessage { get; set; }

        public Message.HighlyStructuredMessage HighlyStructuredMessage { get; set; }

        public Message.ImageMessage ImageMessage { get; set; }

        public Message.VideoMessage VideoMessage { get; set; }

        public Message.LocationMessage LocationMessage { get; set; }

        public Message.HighlyStructuredMessage Content { get; set; }

        public Message.HighlyStructuredMessage Footer { get; set; }

        public static Message.TemplateMessage.FourRowTemplate Deserialize(Stream stream)
        {
          Message.TemplateMessage.FourRowTemplate instance = new Message.TemplateMessage.FourRowTemplate();
          Message.TemplateMessage.FourRowTemplate.Deserialize(stream, instance);
          return instance;
        }

        public static Message.TemplateMessage.FourRowTemplate DeserializeLengthDelimited(
          Stream stream)
        {
          Message.TemplateMessage.FourRowTemplate instance = new Message.TemplateMessage.FourRowTemplate();
          Message.TemplateMessage.FourRowTemplate.DeserializeLengthDelimited(stream, instance);
          return instance;
        }

        public static Message.TemplateMessage.FourRowTemplate DeserializeLength(
          Stream stream,
          int length)
        {
          Message.TemplateMessage.FourRowTemplate instance = new Message.TemplateMessage.FourRowTemplate();
          Message.TemplateMessage.FourRowTemplate.DeserializeLength(stream, length, instance);
          return instance;
        }

        public static Message.TemplateMessage.FourRowTemplate Deserialize(byte[] buffer)
        {
          Message.TemplateMessage.FourRowTemplate instance = new Message.TemplateMessage.FourRowTemplate();
          using (MemoryStream memoryStream = new MemoryStream(buffer))
            Message.TemplateMessage.FourRowTemplate.Deserialize((Stream) memoryStream, instance);
          return instance;
        }

        public static Message.TemplateMessage.FourRowTemplate Deserialize(
          byte[] buffer,
          Message.TemplateMessage.FourRowTemplate instance)
        {
          using (MemoryStream memoryStream = new MemoryStream(buffer))
            Message.TemplateMessage.FourRowTemplate.Deserialize((Stream) memoryStream, instance);
          return instance;
        }

        public static Message.TemplateMessage.FourRowTemplate Deserialize(
          Stream stream,
          Message.TemplateMessage.FourRowTemplate instance)
        {
          while (true)
          {
            int firstByte = stream.ReadByte();
            switch (firstByte)
            {
              case -1:
                goto label_25;
              case 10:
                if (instance.DocumentMessage == null)
                {
                  instance.DocumentMessage = Message.DocumentMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.DocumentMessage.DeserializeLengthDelimited(stream, instance.DocumentMessage);
                continue;
              case 18:
                if (instance.HighlyStructuredMessage == null)
                {
                  instance.HighlyStructuredMessage = Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream, instance.HighlyStructuredMessage);
                continue;
              case 26:
                if (instance.ImageMessage == null)
                {
                  instance.ImageMessage = Message.ImageMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.ImageMessage.DeserializeLengthDelimited(stream, instance.ImageMessage);
                continue;
              case 34:
                if (instance.VideoMessage == null)
                {
                  instance.VideoMessage = Message.VideoMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.VideoMessage.DeserializeLengthDelimited(stream, instance.VideoMessage);
                continue;
              case 42:
                if (instance.LocationMessage == null)
                {
                  instance.LocationMessage = Message.LocationMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.LocationMessage.DeserializeLengthDelimited(stream, instance.LocationMessage);
                continue;
              case 50:
                if (instance.Content == null)
                {
                  instance.Content = Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream, instance.Content);
                continue;
              case 58:
                if (instance.Footer == null)
                {
                  instance.Footer = Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream, instance.Footer);
                continue;
              default:
                SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
                if (key.Field != 0U)
                {
                  ProtocolParser.SkipKey(stream, key);
                  continue;
                }
                goto label_23;
            }
          }
label_23:
          throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_25:
          return instance;
        }

        public static Message.TemplateMessage.FourRowTemplate DeserializeLengthDelimited(
          Stream stream,
          Message.TemplateMessage.FourRowTemplate instance)
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
                if (instance.DocumentMessage == null)
                {
                  instance.DocumentMessage = Message.DocumentMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.DocumentMessage.DeserializeLengthDelimited(stream, instance.DocumentMessage);
                continue;
              case 18:
                if (instance.HighlyStructuredMessage == null)
                {
                  instance.HighlyStructuredMessage = Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream, instance.HighlyStructuredMessage);
                continue;
              case 26:
                if (instance.ImageMessage == null)
                {
                  instance.ImageMessage = Message.ImageMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.ImageMessage.DeserializeLengthDelimited(stream, instance.ImageMessage);
                continue;
              case 34:
                if (instance.VideoMessage == null)
                {
                  instance.VideoMessage = Message.VideoMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.VideoMessage.DeserializeLengthDelimited(stream, instance.VideoMessage);
                continue;
              case 42:
                if (instance.LocationMessage == null)
                {
                  instance.LocationMessage = Message.LocationMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.LocationMessage.DeserializeLengthDelimited(stream, instance.LocationMessage);
                continue;
              case 50:
                if (instance.Content == null)
                {
                  instance.Content = Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream, instance.Content);
                continue;
              case 58:
                if (instance.Footer == null)
                {
                  instance.Footer = Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream, instance.Footer);
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

        public static Message.TemplateMessage.FourRowTemplate DeserializeLength(
          Stream stream,
          int length,
          Message.TemplateMessage.FourRowTemplate instance)
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
                if (instance.DocumentMessage == null)
                {
                  instance.DocumentMessage = Message.DocumentMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.DocumentMessage.DeserializeLengthDelimited(stream, instance.DocumentMessage);
                continue;
              case 18:
                if (instance.HighlyStructuredMessage == null)
                {
                  instance.HighlyStructuredMessage = Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream, instance.HighlyStructuredMessage);
                continue;
              case 26:
                if (instance.ImageMessage == null)
                {
                  instance.ImageMessage = Message.ImageMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.ImageMessage.DeserializeLengthDelimited(stream, instance.ImageMessage);
                continue;
              case 34:
                if (instance.VideoMessage == null)
                {
                  instance.VideoMessage = Message.VideoMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.VideoMessage.DeserializeLengthDelimited(stream, instance.VideoMessage);
                continue;
              case 42:
                if (instance.LocationMessage == null)
                {
                  instance.LocationMessage = Message.LocationMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.LocationMessage.DeserializeLengthDelimited(stream, instance.LocationMessage);
                continue;
              case 50:
                if (instance.Content == null)
                {
                  instance.Content = Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream, instance.Content);
                continue;
              case 58:
                if (instance.Footer == null)
                {
                  instance.Footer = Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream);
                  continue;
                }
                Message.HighlyStructuredMessage.DeserializeLengthDelimited(stream, instance.Footer);
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

        public static void Serialize(
          Stream stream,
          Message.TemplateMessage.FourRowTemplate instance)
        {
          MemoryStream stream1 = ProtocolParser.Stack.Pop();
          if (instance.DocumentMessage != null)
          {
            stream.WriteByte((byte) 10);
            stream1.SetLength(0L);
            Message.DocumentMessage.Serialize((Stream) stream1, instance.DocumentMessage);
            uint length = (uint) stream1.Length;
            ProtocolParser.WriteUInt32(stream, length);
            stream1.WriteTo(stream);
          }
          if (instance.HighlyStructuredMessage != null)
          {
            stream.WriteByte((byte) 18);
            stream1.SetLength(0L);
            Message.HighlyStructuredMessage.Serialize((Stream) stream1, instance.HighlyStructuredMessage);
            uint length = (uint) stream1.Length;
            ProtocolParser.WriteUInt32(stream, length);
            stream1.WriteTo(stream);
          }
          if (instance.ImageMessage != null)
          {
            stream.WriteByte((byte) 26);
            stream1.SetLength(0L);
            Message.ImageMessage.Serialize((Stream) stream1, instance.ImageMessage);
            uint length = (uint) stream1.Length;
            ProtocolParser.WriteUInt32(stream, length);
            stream1.WriteTo(stream);
          }
          if (instance.VideoMessage != null)
          {
            stream.WriteByte((byte) 34);
            stream1.SetLength(0L);
            Message.VideoMessage.Serialize((Stream) stream1, instance.VideoMessage);
            uint length = (uint) stream1.Length;
            ProtocolParser.WriteUInt32(stream, length);
            stream1.WriteTo(stream);
          }
          if (instance.LocationMessage != null)
          {
            stream.WriteByte((byte) 42);
            stream1.SetLength(0L);
            Message.LocationMessage.Serialize((Stream) stream1, instance.LocationMessage);
            uint length = (uint) stream1.Length;
            ProtocolParser.WriteUInt32(stream, length);
            stream1.WriteTo(stream);
          }
          if (instance.Content != null)
          {
            stream.WriteByte((byte) 50);
            stream1.SetLength(0L);
            Message.HighlyStructuredMessage.Serialize((Stream) stream1, instance.Content);
            uint length = (uint) stream1.Length;
            ProtocolParser.WriteUInt32(stream, length);
            stream1.WriteTo(stream);
          }
          if (instance.Footer != null)
          {
            stream.WriteByte((byte) 58);
            stream1.SetLength(0L);
            Message.HighlyStructuredMessage.Serialize((Stream) stream1, instance.Footer);
            uint length = (uint) stream1.Length;
            ProtocolParser.WriteUInt32(stream, length);
            stream1.WriteTo(stream);
          }
          ProtocolParser.Stack.Push(stream1);
        }

        public static byte[] SerializeToBytes(Message.TemplateMessage.FourRowTemplate instance)
        {
          using (MemoryStream memoryStream = new MemoryStream())
          {
            Message.TemplateMessage.FourRowTemplate.Serialize((Stream) memoryStream, instance);
            return memoryStream.ToArray();
          }
        }

        public static void SerializeLengthDelimited(
          Stream stream,
          Message.TemplateMessage.FourRowTemplate instance)
        {
          byte[] bytes = Message.TemplateMessage.FourRowTemplate.SerializeToBytes(instance);
          ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
          stream.Write(bytes, 0, bytes.Length);
        }
      }
    }

    public class ConversationMessage : IProtoBufMessage
    {
      private string conversation;

      public ConversationMessage(string conversation) => this.conversation = conversation;

      public MessageTypes MessageType => MessageTypes.conversation;

      public string Conversation => this.conversation;

      public ContextInfo ContextInfo => (ContextInfo) null;
    }
  }
}
