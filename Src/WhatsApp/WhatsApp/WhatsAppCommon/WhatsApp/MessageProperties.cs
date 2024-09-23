// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageProperties
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace WhatsApp
{
  public class MessageProperties
  {
    private Message Message;

    public MessageProperties.WebClientProperties WebClientPropertiesField { get; set; }

    public MessageProperties.MediaProperties MediaPropertiesField { get; set; }

    public MessageProperties.CommonProperties CommonPropertiesField { get; set; }

    public MessageProperties.ContactProperties ContactPropertiesField { get; set; }

    public MessageProperties.PaymentsProperties PaymentsPropertiesField { get; set; }

    public MessageProperties.LiveLocationProperties LiveLocationPropertiesField { get; set; }

    public MessageProperties.ConversionRecordProperties ConversionRecordPropertiesField { get; set; }

    public MessageProperties.ExtendedTextProperties ExtendedTextPropertiesField { get; set; }

    public MessageProperties.BizProperties BizPropertiesField { get; set; }

    public static MessageProperties Deserialize(Stream stream)
    {
      MessageProperties instance = new MessageProperties();
      MessageProperties.Deserialize(stream, instance);
      return instance;
    }

    public static MessageProperties DeserializeLengthDelimited(Stream stream)
    {
      MessageProperties instance = new MessageProperties();
      MessageProperties.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static MessageProperties DeserializeLength(Stream stream, int length)
    {
      MessageProperties instance = new MessageProperties();
      MessageProperties.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static MessageProperties Deserialize(byte[] buffer)
    {
      MessageProperties instance = new MessageProperties();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        MessageProperties.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static MessageProperties Deserialize(byte[] buffer, MessageProperties instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        MessageProperties.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static MessageProperties Deserialize(Stream stream, MessageProperties instance)
    {
      while (true)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            goto label_31;
          case 10:
            if (instance.WebClientPropertiesField == null)
            {
              instance.WebClientPropertiesField = MessageProperties.WebClientProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.WebClientProperties.DeserializeLengthDelimited(stream, instance.WebClientPropertiesField);
            continue;
          case 18:
            if (instance.MediaPropertiesField == null)
            {
              instance.MediaPropertiesField = MessageProperties.MediaProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.MediaProperties.DeserializeLengthDelimited(stream, instance.MediaPropertiesField);
            continue;
          case 26:
            if (instance.CommonPropertiesField == null)
            {
              instance.CommonPropertiesField = MessageProperties.CommonProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.CommonProperties.DeserializeLengthDelimited(stream, instance.CommonPropertiesField);
            continue;
          case 34:
            if (instance.ContactPropertiesField == null)
            {
              instance.ContactPropertiesField = MessageProperties.ContactProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.ContactProperties.DeserializeLengthDelimited(stream, instance.ContactPropertiesField);
            continue;
          case 42:
            if (instance.PaymentsPropertiesField == null)
            {
              instance.PaymentsPropertiesField = MessageProperties.PaymentsProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.PaymentsProperties.DeserializeLengthDelimited(stream, instance.PaymentsPropertiesField);
            continue;
          case 50:
            if (instance.LiveLocationPropertiesField == null)
            {
              instance.LiveLocationPropertiesField = MessageProperties.LiveLocationProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.LiveLocationProperties.DeserializeLengthDelimited(stream, instance.LiveLocationPropertiesField);
            continue;
          case 58:
            if (instance.ConversionRecordPropertiesField == null)
            {
              instance.ConversionRecordPropertiesField = MessageProperties.ConversionRecordProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.ConversionRecordProperties.DeserializeLengthDelimited(stream, instance.ConversionRecordPropertiesField);
            continue;
          case 66:
            if (instance.ExtendedTextPropertiesField == null)
            {
              instance.ExtendedTextPropertiesField = MessageProperties.ExtendedTextProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.ExtendedTextProperties.DeserializeLengthDelimited(stream, instance.ExtendedTextPropertiesField);
            continue;
          case 74:
            if (instance.BizPropertiesField == null)
            {
              instance.BizPropertiesField = MessageProperties.BizProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.BizProperties.DeserializeLengthDelimited(stream, instance.BizPropertiesField);
            continue;
          default:
            SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
            if (key.Field != 0U)
            {
              ProtocolParser.SkipKey(stream, key);
              continue;
            }
            goto label_29;
        }
      }
label_29:
      throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_31:
      return instance;
    }

    public static MessageProperties DeserializeLengthDelimited(
      Stream stream,
      MessageProperties instance)
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
            if (instance.WebClientPropertiesField == null)
            {
              instance.WebClientPropertiesField = MessageProperties.WebClientProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.WebClientProperties.DeserializeLengthDelimited(stream, instance.WebClientPropertiesField);
            continue;
          case 18:
            if (instance.MediaPropertiesField == null)
            {
              instance.MediaPropertiesField = MessageProperties.MediaProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.MediaProperties.DeserializeLengthDelimited(stream, instance.MediaPropertiesField);
            continue;
          case 26:
            if (instance.CommonPropertiesField == null)
            {
              instance.CommonPropertiesField = MessageProperties.CommonProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.CommonProperties.DeserializeLengthDelimited(stream, instance.CommonPropertiesField);
            continue;
          case 34:
            if (instance.ContactPropertiesField == null)
            {
              instance.ContactPropertiesField = MessageProperties.ContactProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.ContactProperties.DeserializeLengthDelimited(stream, instance.ContactPropertiesField);
            continue;
          case 42:
            if (instance.PaymentsPropertiesField == null)
            {
              instance.PaymentsPropertiesField = MessageProperties.PaymentsProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.PaymentsProperties.DeserializeLengthDelimited(stream, instance.PaymentsPropertiesField);
            continue;
          case 50:
            if (instance.LiveLocationPropertiesField == null)
            {
              instance.LiveLocationPropertiesField = MessageProperties.LiveLocationProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.LiveLocationProperties.DeserializeLengthDelimited(stream, instance.LiveLocationPropertiesField);
            continue;
          case 58:
            if (instance.ConversionRecordPropertiesField == null)
            {
              instance.ConversionRecordPropertiesField = MessageProperties.ConversionRecordProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.ConversionRecordProperties.DeserializeLengthDelimited(stream, instance.ConversionRecordPropertiesField);
            continue;
          case 66:
            if (instance.ExtendedTextPropertiesField == null)
            {
              instance.ExtendedTextPropertiesField = MessageProperties.ExtendedTextProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.ExtendedTextProperties.DeserializeLengthDelimited(stream, instance.ExtendedTextPropertiesField);
            continue;
          case 74:
            if (instance.BizPropertiesField == null)
            {
              instance.BizPropertiesField = MessageProperties.BizProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.BizProperties.DeserializeLengthDelimited(stream, instance.BizPropertiesField);
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

    public static MessageProperties DeserializeLength(
      Stream stream,
      int length,
      MessageProperties instance)
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
            if (instance.WebClientPropertiesField == null)
            {
              instance.WebClientPropertiesField = MessageProperties.WebClientProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.WebClientProperties.DeserializeLengthDelimited(stream, instance.WebClientPropertiesField);
            continue;
          case 18:
            if (instance.MediaPropertiesField == null)
            {
              instance.MediaPropertiesField = MessageProperties.MediaProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.MediaProperties.DeserializeLengthDelimited(stream, instance.MediaPropertiesField);
            continue;
          case 26:
            if (instance.CommonPropertiesField == null)
            {
              instance.CommonPropertiesField = MessageProperties.CommonProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.CommonProperties.DeserializeLengthDelimited(stream, instance.CommonPropertiesField);
            continue;
          case 34:
            if (instance.ContactPropertiesField == null)
            {
              instance.ContactPropertiesField = MessageProperties.ContactProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.ContactProperties.DeserializeLengthDelimited(stream, instance.ContactPropertiesField);
            continue;
          case 42:
            if (instance.PaymentsPropertiesField == null)
            {
              instance.PaymentsPropertiesField = MessageProperties.PaymentsProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.PaymentsProperties.DeserializeLengthDelimited(stream, instance.PaymentsPropertiesField);
            continue;
          case 50:
            if (instance.LiveLocationPropertiesField == null)
            {
              instance.LiveLocationPropertiesField = MessageProperties.LiveLocationProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.LiveLocationProperties.DeserializeLengthDelimited(stream, instance.LiveLocationPropertiesField);
            continue;
          case 58:
            if (instance.ConversionRecordPropertiesField == null)
            {
              instance.ConversionRecordPropertiesField = MessageProperties.ConversionRecordProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.ConversionRecordProperties.DeserializeLengthDelimited(stream, instance.ConversionRecordPropertiesField);
            continue;
          case 66:
            if (instance.ExtendedTextPropertiesField == null)
            {
              instance.ExtendedTextPropertiesField = MessageProperties.ExtendedTextProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.ExtendedTextProperties.DeserializeLengthDelimited(stream, instance.ExtendedTextPropertiesField);
            continue;
          case 74:
            if (instance.BizPropertiesField == null)
            {
              instance.BizPropertiesField = MessageProperties.BizProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageProperties.BizProperties.DeserializeLengthDelimited(stream, instance.BizPropertiesField);
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

    public static void Serialize(Stream stream, MessageProperties instance)
    {
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.WebClientPropertiesField != null)
      {
        stream.WriteByte((byte) 10);
        stream1.SetLength(0L);
        MessageProperties.WebClientProperties.Serialize((Stream) stream1, instance.WebClientPropertiesField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.MediaPropertiesField != null)
      {
        stream.WriteByte((byte) 18);
        stream1.SetLength(0L);
        MessageProperties.MediaProperties.Serialize((Stream) stream1, instance.MediaPropertiesField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.CommonPropertiesField != null)
      {
        stream.WriteByte((byte) 26);
        stream1.SetLength(0L);
        MessageProperties.CommonProperties.Serialize((Stream) stream1, instance.CommonPropertiesField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.ContactPropertiesField != null)
      {
        stream.WriteByte((byte) 34);
        stream1.SetLength(0L);
        MessageProperties.ContactProperties.Serialize((Stream) stream1, instance.ContactPropertiesField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.PaymentsPropertiesField != null)
      {
        stream.WriteByte((byte) 42);
        stream1.SetLength(0L);
        MessageProperties.PaymentsProperties.Serialize((Stream) stream1, instance.PaymentsPropertiesField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.LiveLocationPropertiesField != null)
      {
        stream.WriteByte((byte) 50);
        stream1.SetLength(0L);
        MessageProperties.LiveLocationProperties.Serialize((Stream) stream1, instance.LiveLocationPropertiesField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.ConversionRecordPropertiesField != null)
      {
        stream.WriteByte((byte) 58);
        stream1.SetLength(0L);
        MessageProperties.ConversionRecordProperties.Serialize((Stream) stream1, instance.ConversionRecordPropertiesField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.ExtendedTextPropertiesField != null)
      {
        stream.WriteByte((byte) 66);
        stream1.SetLength(0L);
        MessageProperties.ExtendedTextProperties.Serialize((Stream) stream1, instance.ExtendedTextPropertiesField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.BizPropertiesField != null)
      {
        stream.WriteByte((byte) 74);
        stream1.SetLength(0L);
        MessageProperties.BizProperties.Serialize((Stream) stream1, instance.BizPropertiesField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(MessageProperties instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        MessageProperties.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, MessageProperties instance)
    {
      byte[] bytes = MessageProperties.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }

    public static bool HasMessageProperties(Message msg) => msg.InternalProperties != null;

    public static MessageProperties GetForMessage(Message msg)
    {
      MessageProperties forMessage = msg.InternalProperties ?? new MessageProperties();
      forMessage.Message = msg;
      return forMessage;
    }

    public void Save()
    {
      if (this.Message == null)
        throw new ArgumentNullException("No Message associated with these properties");
      this.Message.InternalProperties = this;
    }

    public void SetForMessage(Message msg)
    {
      this.Message = msg.InternalProperties == null ? msg : throw new ArgumentNullException("Overriding MessageProperties in Message, Id: " + msg.KeyId);
      msg.InternalProperties = this;
    }

    public MessageProperties.WebClientProperties EnsureWebClientProperties
    {
      get
      {
        if (this.WebClientPropertiesField == null)
          this.WebClientPropertiesField = new MessageProperties.WebClientProperties();
        return this.WebClientPropertiesField;
      }
    }

    public MessageProperties.MediaProperties EnsureMediaProperties
    {
      get
      {
        if (this.MediaPropertiesField == null)
          this.MediaPropertiesField = new MessageProperties.MediaProperties();
        return this.MediaPropertiesField;
      }
    }

    public MessageProperties.CommonProperties EnsureCommonProperties
    {
      get
      {
        if (this.CommonPropertiesField == null)
          this.CommonPropertiesField = new MessageProperties.CommonProperties();
        return this.CommonPropertiesField;
      }
    }

    public MessageProperties.ExtendedTextProperties EnsureExtendedTextProperties
    {
      get
      {
        return this.ExtendedTextPropertiesField ?? (this.ExtendedTextPropertiesField = new MessageProperties.ExtendedTextProperties());
      }
    }

    public MessageProperties.ContactProperties EnsureContactProperties
    {
      get
      {
        if (this.ContactPropertiesField == null)
          this.ContactPropertiesField = new MessageProperties.ContactProperties();
        return this.ContactPropertiesField;
      }
    }

    public MessageProperties.LiveLocationProperties EnsureLiveLocationProperties
    {
      get
      {
        if (this.LiveLocationPropertiesField == null)
          this.LiveLocationPropertiesField = new MessageProperties.LiveLocationProperties();
        return this.LiveLocationPropertiesField;
      }
    }

    public IEnumerable<string> Contacts
    {
      get
      {
        return this.ContactPropertiesField != null ? (IEnumerable<string>) this.ContactPropertiesField.Vcards : (IEnumerable<string>) new string[0];
      }
      set => this.EnsureContactProperties.Vcards = value.ToList<string>();
    }

    public MessageProperties.ConversionRecordProperties EnsureConversionRecordProperties
    {
      get
      {
        if (this.ConversionRecordPropertiesField == null)
          this.ConversionRecordPropertiesField = new MessageProperties.ConversionRecordProperties();
        return this.ConversionRecordPropertiesField;
      }
    }

    public MessageProperties.BizProperties EnsureBizProperties
    {
      get
      {
        if (this.BizPropertiesField == null)
          this.BizPropertiesField = new MessageProperties.BizProperties();
        return this.BizPropertiesField;
      }
    }

    public class WebClientProperties
    {
      public bool? WebRelay { get; set; }

      public static MessageProperties.WebClientProperties Deserialize(Stream stream)
      {
        MessageProperties.WebClientProperties instance = new MessageProperties.WebClientProperties();
        MessageProperties.WebClientProperties.Deserialize(stream, instance);
        return instance;
      }

      public static MessageProperties.WebClientProperties DeserializeLengthDelimited(Stream stream)
      {
        MessageProperties.WebClientProperties instance = new MessageProperties.WebClientProperties();
        MessageProperties.WebClientProperties.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static MessageProperties.WebClientProperties DeserializeLength(
        Stream stream,
        int length)
      {
        MessageProperties.WebClientProperties instance = new MessageProperties.WebClientProperties();
        MessageProperties.WebClientProperties.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static MessageProperties.WebClientProperties Deserialize(byte[] buffer)
      {
        MessageProperties.WebClientProperties instance = new MessageProperties.WebClientProperties();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          MessageProperties.WebClientProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static MessageProperties.WebClientProperties Deserialize(
        byte[] buffer,
        MessageProperties.WebClientProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          MessageProperties.WebClientProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static MessageProperties.WebClientProperties Deserialize(
        Stream stream,
        MessageProperties.WebClientProperties instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_5;
            case 8:
              instance.WebRelay = new bool?(ProtocolParser.ReadBool(stream));
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

      public static MessageProperties.WebClientProperties DeserializeLengthDelimited(
        Stream stream,
        MessageProperties.WebClientProperties instance)
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
              instance.WebRelay = new bool?(ProtocolParser.ReadBool(stream));
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

      public static MessageProperties.WebClientProperties DeserializeLength(
        Stream stream,
        int length,
        MessageProperties.WebClientProperties instance)
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
              instance.WebRelay = new bool?(ProtocolParser.ReadBool(stream));
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

      public static void Serialize(Stream stream, MessageProperties.WebClientProperties instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.WebRelay.HasValue)
        {
          stream.WriteByte((byte) 8);
          ProtocolParser.WriteBool(stream, instance.WebRelay.Value);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(MessageProperties.WebClientProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          MessageProperties.WebClientProperties.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        MessageProperties.WebClientProperties instance)
      {
        byte[] bytes = MessageProperties.WebClientProperties.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }
    }

    public class MediaProperties
    {
      public bool? CheckAndRepairRun { get; set; }

      public MessageProperties.MediaProperties.Attribution? GifAttribution { get; set; }

      public uint? Height { get; set; }

      public uint? Width { get; set; }

      public byte[] Sidecar { get; set; }

      public bool? AutoDownloadEligible { get; set; }

      public uint? DecryptRetryCount { get; set; }

      public string MediaDirectPath { get; set; }

      public static MessageProperties.MediaProperties Deserialize(Stream stream)
      {
        MessageProperties.MediaProperties instance = new MessageProperties.MediaProperties();
        MessageProperties.MediaProperties.Deserialize(stream, instance);
        return instance;
      }

      public static MessageProperties.MediaProperties DeserializeLengthDelimited(Stream stream)
      {
        MessageProperties.MediaProperties instance = new MessageProperties.MediaProperties();
        MessageProperties.MediaProperties.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static MessageProperties.MediaProperties DeserializeLength(Stream stream, int length)
      {
        MessageProperties.MediaProperties instance = new MessageProperties.MediaProperties();
        MessageProperties.MediaProperties.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static MessageProperties.MediaProperties Deserialize(byte[] buffer)
      {
        MessageProperties.MediaProperties instance = new MessageProperties.MediaProperties();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          MessageProperties.MediaProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static MessageProperties.MediaProperties Deserialize(
        byte[] buffer,
        MessageProperties.MediaProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          MessageProperties.MediaProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static MessageProperties.MediaProperties Deserialize(
        Stream stream,
        MessageProperties.MediaProperties instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_12;
            case 8:
              instance.CheckAndRepairRun = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 16:
              instance.GifAttribution = new MessageProperties.MediaProperties.Attribution?((MessageProperties.MediaProperties.Attribution) ProtocolParser.ReadUInt64(stream));
              continue;
            case 24:
              instance.Height = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 32:
              instance.Width = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 42:
              instance.Sidecar = ProtocolParser.ReadBytes(stream);
              continue;
            case 48:
              instance.AutoDownloadEligible = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 56:
              instance.DecryptRetryCount = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 66:
              instance.MediaDirectPath = ProtocolParser.ReadString(stream);
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              if (key.Field != 0U)
              {
                ProtocolParser.SkipKey(stream, key);
                continue;
              }
              goto label_10;
          }
        }
label_10:
        throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_12:
        return instance;
      }

      public static MessageProperties.MediaProperties DeserializeLengthDelimited(
        Stream stream,
        MessageProperties.MediaProperties instance)
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
              instance.CheckAndRepairRun = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 16:
              instance.GifAttribution = new MessageProperties.MediaProperties.Attribution?((MessageProperties.MediaProperties.Attribution) ProtocolParser.ReadUInt64(stream));
              continue;
            case 24:
              instance.Height = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 32:
              instance.Width = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 42:
              instance.Sidecar = ProtocolParser.ReadBytes(stream);
              continue;
            case 48:
              instance.AutoDownloadEligible = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 56:
              instance.DecryptRetryCount = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 66:
              instance.MediaDirectPath = ProtocolParser.ReadString(stream);
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

      public static MessageProperties.MediaProperties DeserializeLength(
        Stream stream,
        int length,
        MessageProperties.MediaProperties instance)
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
              instance.CheckAndRepairRun = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 16:
              instance.GifAttribution = new MessageProperties.MediaProperties.Attribution?((MessageProperties.MediaProperties.Attribution) ProtocolParser.ReadUInt64(stream));
              continue;
            case 24:
              instance.Height = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 32:
              instance.Width = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 42:
              instance.Sidecar = ProtocolParser.ReadBytes(stream);
              continue;
            case 48:
              instance.AutoDownloadEligible = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 56:
              instance.DecryptRetryCount = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 66:
              instance.MediaDirectPath = ProtocolParser.ReadString(stream);
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

      public static void Serialize(Stream stream, MessageProperties.MediaProperties instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.CheckAndRepairRun.HasValue)
        {
          stream.WriteByte((byte) 8);
          ProtocolParser.WriteBool(stream, instance.CheckAndRepairRun.Value);
        }
        if (instance.GifAttribution.HasValue)
        {
          stream.WriteByte((byte) 16);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.GifAttribution.Value);
        }
        if (instance.Height.HasValue)
        {
          stream.WriteByte((byte) 24);
          ProtocolParser.WriteUInt32(stream, instance.Height.Value);
        }
        if (instance.Width.HasValue)
        {
          stream.WriteByte((byte) 32);
          ProtocolParser.WriteUInt32(stream, instance.Width.Value);
        }
        if (instance.Sidecar != null)
        {
          stream.WriteByte((byte) 42);
          ProtocolParser.WriteBytes(stream, instance.Sidecar);
        }
        if (instance.AutoDownloadEligible.HasValue)
        {
          stream.WriteByte((byte) 48);
          ProtocolParser.WriteBool(stream, instance.AutoDownloadEligible.Value);
        }
        if (instance.DecryptRetryCount.HasValue)
        {
          stream.WriteByte((byte) 56);
          ProtocolParser.WriteUInt32(stream, instance.DecryptRetryCount.Value);
        }
        if (instance.MediaDirectPath != null)
        {
          stream.WriteByte((byte) 66);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.MediaDirectPath));
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(MessageProperties.MediaProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          MessageProperties.MediaProperties.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        MessageProperties.MediaProperties instance)
      {
        byte[] bytes = MessageProperties.MediaProperties.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public enum Attribution
      {
        NONE,
        GIPHY,
        TENOR,
      }
    }

    public class CommonProperties
    {
      public bool? Multicast { get; set; }

      public MessageProperties.CommonProperties.NotifyLevels? NotifyLevel { get; set; }

      public bool? UrlNumber { get; set; }

      public bool? UrlText { get; set; }

      public int? CipherRetryCount { get; set; }

      public byte[] CipherMediaHash { get; set; }

      public int? AckedRecipientsCount { get; set; }

      public string RevokedMsgId { get; set; }

      public int? RevokedMediaType { get; set; }

      public int? SentRecipientsCount { get; set; }

      public bool? ForwardedFlag { get; set; }

      public static MessageProperties.CommonProperties Deserialize(Stream stream)
      {
        MessageProperties.CommonProperties instance = new MessageProperties.CommonProperties();
        MessageProperties.CommonProperties.Deserialize(stream, instance);
        return instance;
      }

      public static MessageProperties.CommonProperties DeserializeLengthDelimited(Stream stream)
      {
        MessageProperties.CommonProperties instance = new MessageProperties.CommonProperties();
        MessageProperties.CommonProperties.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static MessageProperties.CommonProperties DeserializeLength(Stream stream, int length)
      {
        MessageProperties.CommonProperties instance = new MessageProperties.CommonProperties();
        MessageProperties.CommonProperties.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static MessageProperties.CommonProperties Deserialize(byte[] buffer)
      {
        MessageProperties.CommonProperties instance = new MessageProperties.CommonProperties();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          MessageProperties.CommonProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static MessageProperties.CommonProperties Deserialize(
        byte[] buffer,
        MessageProperties.CommonProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          MessageProperties.CommonProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static MessageProperties.CommonProperties Deserialize(
        Stream stream,
        MessageProperties.CommonProperties instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_15;
            case 8:
              instance.Multicast = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 16:
              instance.NotifyLevel = new MessageProperties.CommonProperties.NotifyLevels?((MessageProperties.CommonProperties.NotifyLevels) ProtocolParser.ReadUInt64(stream));
              continue;
            case 24:
              instance.UrlNumber = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 32:
              instance.UrlText = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 40:
              instance.CipherRetryCount = new int?((int) ProtocolParser.ReadUInt64(stream));
              continue;
            case 50:
              instance.CipherMediaHash = ProtocolParser.ReadBytes(stream);
              continue;
            case 56:
              instance.AckedRecipientsCount = new int?((int) ProtocolParser.ReadUInt64(stream));
              continue;
            case 66:
              instance.RevokedMsgId = ProtocolParser.ReadString(stream);
              continue;
            case 72:
              instance.RevokedMediaType = new int?((int) ProtocolParser.ReadUInt64(stream));
              continue;
            case 80:
              instance.SentRecipientsCount = new int?((int) ProtocolParser.ReadUInt64(stream));
              continue;
            case 88:
              instance.ForwardedFlag = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            default:
              SilentOrbit.ProtocolBuffers.Key key = ProtocolParser.ReadKey((byte) firstByte, stream);
              if (key.Field != 0U)
              {
                ProtocolParser.SkipKey(stream, key);
                continue;
              }
              goto label_13;
          }
        }
label_13:
        throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
label_15:
        return instance;
      }

      public static MessageProperties.CommonProperties DeserializeLengthDelimited(
        Stream stream,
        MessageProperties.CommonProperties instance)
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
              instance.Multicast = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 16:
              instance.NotifyLevel = new MessageProperties.CommonProperties.NotifyLevels?((MessageProperties.CommonProperties.NotifyLevels) ProtocolParser.ReadUInt64(stream));
              continue;
            case 24:
              instance.UrlNumber = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 32:
              instance.UrlText = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 40:
              instance.CipherRetryCount = new int?((int) ProtocolParser.ReadUInt64(stream));
              continue;
            case 50:
              instance.CipherMediaHash = ProtocolParser.ReadBytes(stream);
              continue;
            case 56:
              instance.AckedRecipientsCount = new int?((int) ProtocolParser.ReadUInt64(stream));
              continue;
            case 66:
              instance.RevokedMsgId = ProtocolParser.ReadString(stream);
              continue;
            case 72:
              instance.RevokedMediaType = new int?((int) ProtocolParser.ReadUInt64(stream));
              continue;
            case 80:
              instance.SentRecipientsCount = new int?((int) ProtocolParser.ReadUInt64(stream));
              continue;
            case 88:
              instance.ForwardedFlag = new bool?(ProtocolParser.ReadBool(stream));
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

      public static MessageProperties.CommonProperties DeserializeLength(
        Stream stream,
        int length,
        MessageProperties.CommonProperties instance)
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
              instance.Multicast = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 16:
              instance.NotifyLevel = new MessageProperties.CommonProperties.NotifyLevels?((MessageProperties.CommonProperties.NotifyLevels) ProtocolParser.ReadUInt64(stream));
              continue;
            case 24:
              instance.UrlNumber = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 32:
              instance.UrlText = new bool?(ProtocolParser.ReadBool(stream));
              continue;
            case 40:
              instance.CipherRetryCount = new int?((int) ProtocolParser.ReadUInt64(stream));
              continue;
            case 50:
              instance.CipherMediaHash = ProtocolParser.ReadBytes(stream);
              continue;
            case 56:
              instance.AckedRecipientsCount = new int?((int) ProtocolParser.ReadUInt64(stream));
              continue;
            case 66:
              instance.RevokedMsgId = ProtocolParser.ReadString(stream);
              continue;
            case 72:
              instance.RevokedMediaType = new int?((int) ProtocolParser.ReadUInt64(stream));
              continue;
            case 80:
              instance.SentRecipientsCount = new int?((int) ProtocolParser.ReadUInt64(stream));
              continue;
            case 88:
              instance.ForwardedFlag = new bool?(ProtocolParser.ReadBool(stream));
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

      public static void Serialize(Stream stream, MessageProperties.CommonProperties instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Multicast.HasValue)
        {
          stream.WriteByte((byte) 8);
          ProtocolParser.WriteBool(stream, instance.Multicast.Value);
        }
        if (instance.NotifyLevel.HasValue)
        {
          stream.WriteByte((byte) 16);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.NotifyLevel.Value);
        }
        if (instance.UrlNumber.HasValue)
        {
          stream.WriteByte((byte) 24);
          ProtocolParser.WriteBool(stream, instance.UrlNumber.Value);
        }
        if (instance.UrlText.HasValue)
        {
          stream.WriteByte((byte) 32);
          ProtocolParser.WriteBool(stream, instance.UrlText.Value);
        }
        if (instance.CipherRetryCount.HasValue)
        {
          stream.WriteByte((byte) 40);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.CipherRetryCount.Value);
        }
        if (instance.CipherMediaHash != null)
        {
          stream.WriteByte((byte) 50);
          ProtocolParser.WriteBytes(stream, instance.CipherMediaHash);
        }
        if (instance.AckedRecipientsCount.HasValue)
        {
          stream.WriteByte((byte) 56);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.AckedRecipientsCount.Value);
        }
        if (instance.RevokedMsgId != null)
        {
          stream.WriteByte((byte) 66);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.RevokedMsgId));
        }
        if (instance.RevokedMediaType.HasValue)
        {
          stream.WriteByte((byte) 72);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.RevokedMediaType.Value);
        }
        if (instance.SentRecipientsCount.HasValue)
        {
          stream.WriteByte((byte) 80);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.SentRecipientsCount.Value);
        }
        if (instance.ForwardedFlag.HasValue)
        {
          stream.WriteByte((byte) 88);
          ProtocolParser.WriteBool(stream, instance.ForwardedFlag.Value);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(MessageProperties.CommonProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          MessageProperties.CommonProperties.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        MessageProperties.CommonProperties instance)
      {
        byte[] bytes = MessageProperties.CommonProperties.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public enum NotifyLevels
      {
        NotifyAll,
        ExcludeAlertAndBadge,
        ExcludeAlert,
      }
    }

    public class ContactProperties
    {
      public List<string> Vcards { get; set; }

      public static MessageProperties.ContactProperties Deserialize(Stream stream)
      {
        MessageProperties.ContactProperties instance = new MessageProperties.ContactProperties();
        MessageProperties.ContactProperties.Deserialize(stream, instance);
        return instance;
      }

      public static MessageProperties.ContactProperties DeserializeLengthDelimited(Stream stream)
      {
        MessageProperties.ContactProperties instance = new MessageProperties.ContactProperties();
        MessageProperties.ContactProperties.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static MessageProperties.ContactProperties DeserializeLength(Stream stream, int length)
      {
        MessageProperties.ContactProperties instance = new MessageProperties.ContactProperties();
        MessageProperties.ContactProperties.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static MessageProperties.ContactProperties Deserialize(byte[] buffer)
      {
        MessageProperties.ContactProperties instance = new MessageProperties.ContactProperties();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          MessageProperties.ContactProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static MessageProperties.ContactProperties Deserialize(
        byte[] buffer,
        MessageProperties.ContactProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          MessageProperties.ContactProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static MessageProperties.ContactProperties Deserialize(
        Stream stream,
        MessageProperties.ContactProperties instance)
      {
        if (instance.Vcards == null)
          instance.Vcards = new List<string>();
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_7;
            case 10:
              instance.Vcards.Add(ProtocolParser.ReadString(stream));
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

      public static MessageProperties.ContactProperties DeserializeLengthDelimited(
        Stream stream,
        MessageProperties.ContactProperties instance)
      {
        if (instance.Vcards == null)
          instance.Vcards = new List<string>();
        long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 10:
              instance.Vcards.Add(ProtocolParser.ReadString(stream));
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

      public static MessageProperties.ContactProperties DeserializeLength(
        Stream stream,
        int length,
        MessageProperties.ContactProperties instance)
      {
        if (instance.Vcards == null)
          instance.Vcards = new List<string>();
        long num = stream.Position + (long) length;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 10:
              instance.Vcards.Add(ProtocolParser.ReadString(stream));
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

      public static void Serialize(Stream stream, MessageProperties.ContactProperties instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Vcards != null)
        {
          foreach (string vcard in instance.Vcards)
          {
            stream.WriteByte((byte) 10);
            ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(vcard));
          }
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(MessageProperties.ContactProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          MessageProperties.ContactProperties.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        MessageProperties.ContactProperties instance)
      {
        byte[] bytes = MessageProperties.ContactProperties.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }
    }

    public class ExtendedTextProperties
    {
      public uint? BackgroundArgb { get; set; }

      public int? Font { get; set; }

      public static MessageProperties.ExtendedTextProperties Deserialize(Stream stream)
      {
        MessageProperties.ExtendedTextProperties instance = new MessageProperties.ExtendedTextProperties();
        MessageProperties.ExtendedTextProperties.Deserialize(stream, instance);
        return instance;
      }

      public static MessageProperties.ExtendedTextProperties DeserializeLengthDelimited(
        Stream stream)
      {
        MessageProperties.ExtendedTextProperties instance = new MessageProperties.ExtendedTextProperties();
        MessageProperties.ExtendedTextProperties.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static MessageProperties.ExtendedTextProperties DeserializeLength(
        Stream stream,
        int length)
      {
        MessageProperties.ExtendedTextProperties instance = new MessageProperties.ExtendedTextProperties();
        MessageProperties.ExtendedTextProperties.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static MessageProperties.ExtendedTextProperties Deserialize(byte[] buffer)
      {
        MessageProperties.ExtendedTextProperties instance = new MessageProperties.ExtendedTextProperties();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          MessageProperties.ExtendedTextProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static MessageProperties.ExtendedTextProperties Deserialize(
        byte[] buffer,
        MessageProperties.ExtendedTextProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          MessageProperties.ExtendedTextProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static MessageProperties.ExtendedTextProperties Deserialize(
        Stream stream,
        MessageProperties.ExtendedTextProperties instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_6;
            case 8:
              instance.BackgroundArgb = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 16:
              instance.Font = new int?((int) ProtocolParser.ReadUInt64(stream));
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

      public static MessageProperties.ExtendedTextProperties DeserializeLengthDelimited(
        Stream stream,
        MessageProperties.ExtendedTextProperties instance)
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
              instance.BackgroundArgb = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 16:
              instance.Font = new int?((int) ProtocolParser.ReadUInt64(stream));
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

      public static MessageProperties.ExtendedTextProperties DeserializeLength(
        Stream stream,
        int length,
        MessageProperties.ExtendedTextProperties instance)
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
              instance.BackgroundArgb = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 16:
              instance.Font = new int?((int) ProtocolParser.ReadUInt64(stream));
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

      public static void Serialize(Stream stream, MessageProperties.ExtendedTextProperties instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.BackgroundArgb.HasValue)
        {
          stream.WriteByte((byte) 8);
          ProtocolParser.WriteUInt32(stream, instance.BackgroundArgb.Value);
        }
        if (instance.Font.HasValue)
        {
          stream.WriteByte((byte) 16);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.Font.Value);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(MessageProperties.ExtendedTextProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          MessageProperties.ExtendedTextProperties.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        MessageProperties.ExtendedTextProperties instance)
      {
        byte[] bytes = MessageProperties.ExtendedTextProperties.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }
    }

    public class PaymentsProperties
    {
      public MessageProperties.PaymentsProperties.PayTypes? PayType { get; set; }

      public string Currency { get; set; }

      public string Amount { get; set; }

      public string CredentialId { get; set; }

      public string Receiver { get; set; }

      public static MessageProperties.PaymentsProperties Deserialize(Stream stream)
      {
        MessageProperties.PaymentsProperties instance = new MessageProperties.PaymentsProperties();
        MessageProperties.PaymentsProperties.Deserialize(stream, instance);
        return instance;
      }

      public static MessageProperties.PaymentsProperties DeserializeLengthDelimited(Stream stream)
      {
        MessageProperties.PaymentsProperties instance = new MessageProperties.PaymentsProperties();
        MessageProperties.PaymentsProperties.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static MessageProperties.PaymentsProperties DeserializeLength(
        Stream stream,
        int length)
      {
        MessageProperties.PaymentsProperties instance = new MessageProperties.PaymentsProperties();
        MessageProperties.PaymentsProperties.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static MessageProperties.PaymentsProperties Deserialize(byte[] buffer)
      {
        MessageProperties.PaymentsProperties instance = new MessageProperties.PaymentsProperties();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          MessageProperties.PaymentsProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static MessageProperties.PaymentsProperties Deserialize(
        byte[] buffer,
        MessageProperties.PaymentsProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          MessageProperties.PaymentsProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static MessageProperties.PaymentsProperties Deserialize(
        Stream stream,
        MessageProperties.PaymentsProperties instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_9;
            case 8:
              instance.PayType = new MessageProperties.PaymentsProperties.PayTypes?((MessageProperties.PaymentsProperties.PayTypes) ProtocolParser.ReadUInt64(stream));
              continue;
            case 18:
              instance.Currency = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              instance.Amount = ProtocolParser.ReadString(stream);
              continue;
            case 34:
              instance.CredentialId = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.Receiver = ProtocolParser.ReadString(stream);
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

      public static MessageProperties.PaymentsProperties DeserializeLengthDelimited(
        Stream stream,
        MessageProperties.PaymentsProperties instance)
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
              instance.PayType = new MessageProperties.PaymentsProperties.PayTypes?((MessageProperties.PaymentsProperties.PayTypes) ProtocolParser.ReadUInt64(stream));
              continue;
            case 18:
              instance.Currency = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              instance.Amount = ProtocolParser.ReadString(stream);
              continue;
            case 34:
              instance.CredentialId = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.Receiver = ProtocolParser.ReadString(stream);
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

      public static MessageProperties.PaymentsProperties DeserializeLength(
        Stream stream,
        int length,
        MessageProperties.PaymentsProperties instance)
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
              instance.PayType = new MessageProperties.PaymentsProperties.PayTypes?((MessageProperties.PaymentsProperties.PayTypes) ProtocolParser.ReadUInt64(stream));
              continue;
            case 18:
              instance.Currency = ProtocolParser.ReadString(stream);
              continue;
            case 26:
              instance.Amount = ProtocolParser.ReadString(stream);
              continue;
            case 34:
              instance.CredentialId = ProtocolParser.ReadString(stream);
              continue;
            case 42:
              instance.Receiver = ProtocolParser.ReadString(stream);
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

      public static void Serialize(Stream stream, MessageProperties.PaymentsProperties instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.PayType.HasValue)
        {
          stream.WriteByte((byte) 8);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.PayType.Value);
        }
        if (instance.Currency != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Currency));
        }
        if (instance.Amount != null)
        {
          stream.WriteByte((byte) 26);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Amount));
        }
        if (instance.CredentialId != null)
        {
          stream.WriteByte((byte) 34);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.CredentialId));
        }
        if (instance.Receiver != null)
        {
          stream.WriteByte((byte) 42);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Receiver));
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(MessageProperties.PaymentsProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          MessageProperties.PaymentsProperties.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        MessageProperties.PaymentsProperties instance)
      {
        byte[] bytes = MessageProperties.PaymentsProperties.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public static MessageProperties.PaymentsProperties.PayTypes ConvertToPayType(string wireForm)
      {
        if (!string.IsNullOrEmpty(wireForm) && wireForm.ToUpper() == MessageProperties.PaymentsProperties.PayTypes.SEND.ToString())
          return MessageProperties.PaymentsProperties.PayTypes.SEND;
        throw new ArgumentOutOfRangeException("Unsupported pay type received " + wireForm);
      }

      public enum PayTypes
      {
        NONE,
        SEND,
      }
    }

    public class LiveLocationProperties
    {
      public string Caption { get; set; }

      public uint? AccuracyInMeters { get; set; }

      public float? SpeedInMps { get; set; }

      public uint? DegreesClockwiseFromMagneticNorth { get; set; }

      public static MessageProperties.LiveLocationProperties Deserialize(Stream stream)
      {
        MessageProperties.LiveLocationProperties instance = new MessageProperties.LiveLocationProperties();
        MessageProperties.LiveLocationProperties.Deserialize(stream, instance);
        return instance;
      }

      public static MessageProperties.LiveLocationProperties DeserializeLengthDelimited(
        Stream stream)
      {
        MessageProperties.LiveLocationProperties instance = new MessageProperties.LiveLocationProperties();
        MessageProperties.LiveLocationProperties.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static MessageProperties.LiveLocationProperties DeserializeLength(
        Stream stream,
        int length)
      {
        MessageProperties.LiveLocationProperties instance = new MessageProperties.LiveLocationProperties();
        MessageProperties.LiveLocationProperties.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static MessageProperties.LiveLocationProperties Deserialize(byte[] buffer)
      {
        MessageProperties.LiveLocationProperties instance = new MessageProperties.LiveLocationProperties();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          MessageProperties.LiveLocationProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static MessageProperties.LiveLocationProperties Deserialize(
        byte[] buffer,
        MessageProperties.LiveLocationProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          MessageProperties.LiveLocationProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static MessageProperties.LiveLocationProperties Deserialize(
        Stream stream,
        MessageProperties.LiveLocationProperties instance)
      {
        BinaryReader binaryReader = new BinaryReader(stream);
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_9;
            case 10:
              instance.Caption = ProtocolParser.ReadString(stream);
              continue;
            case 16:
              instance.AccuracyInMeters = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 29:
              instance.SpeedInMps = new float?(binaryReader.ReadSingle());
              continue;
            case 32:
              instance.DegreesClockwiseFromMagneticNorth = new uint?(ProtocolParser.ReadUInt32(stream));
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

      public static MessageProperties.LiveLocationProperties DeserializeLengthDelimited(
        Stream stream,
        MessageProperties.LiveLocationProperties instance)
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
              instance.Caption = ProtocolParser.ReadString(stream);
              continue;
            case 16:
              instance.AccuracyInMeters = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 29:
              instance.SpeedInMps = new float?(binaryReader.ReadSingle());
              continue;
            case 32:
              instance.DegreesClockwiseFromMagneticNorth = new uint?(ProtocolParser.ReadUInt32(stream));
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

      public static MessageProperties.LiveLocationProperties DeserializeLength(
        Stream stream,
        int length,
        MessageProperties.LiveLocationProperties instance)
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
              instance.Caption = ProtocolParser.ReadString(stream);
              continue;
            case 16:
              instance.AccuracyInMeters = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 29:
              instance.SpeedInMps = new float?(binaryReader.ReadSingle());
              continue;
            case 32:
              instance.DegreesClockwiseFromMagneticNorth = new uint?(ProtocolParser.ReadUInt32(stream));
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

      public static void Serialize(Stream stream, MessageProperties.LiveLocationProperties instance)
      {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Caption != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Caption));
        }
        if (instance.AccuracyInMeters.HasValue)
        {
          stream.WriteByte((byte) 16);
          ProtocolParser.WriteUInt32(stream, instance.AccuracyInMeters.Value);
        }
        if (instance.SpeedInMps.HasValue)
        {
          stream.WriteByte((byte) 29);
          binaryWriter.Write(instance.SpeedInMps.Value);
        }
        if (instance.DegreesClockwiseFromMagneticNorth.HasValue)
        {
          stream.WriteByte((byte) 32);
          ProtocolParser.WriteUInt32(stream, instance.DegreesClockwiseFromMagneticNorth.Value);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(MessageProperties.LiveLocationProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          MessageProperties.LiveLocationProperties.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        MessageProperties.LiveLocationProperties instance)
      {
        byte[] bytes = MessageProperties.LiveLocationProperties.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }
    }

    public class ConversionRecordProperties
    {
      public string Source { get; set; }

      public byte[] Data { get; set; }

      public uint? DelaySeconds { get; set; }

      public static MessageProperties.ConversionRecordProperties Deserialize(Stream stream)
      {
        MessageProperties.ConversionRecordProperties instance = new MessageProperties.ConversionRecordProperties();
        MessageProperties.ConversionRecordProperties.Deserialize(stream, instance);
        return instance;
      }

      public static MessageProperties.ConversionRecordProperties DeserializeLengthDelimited(
        Stream stream)
      {
        MessageProperties.ConversionRecordProperties instance = new MessageProperties.ConversionRecordProperties();
        MessageProperties.ConversionRecordProperties.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static MessageProperties.ConversionRecordProperties DeserializeLength(
        Stream stream,
        int length)
      {
        MessageProperties.ConversionRecordProperties instance = new MessageProperties.ConversionRecordProperties();
        MessageProperties.ConversionRecordProperties.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static MessageProperties.ConversionRecordProperties Deserialize(byte[] buffer)
      {
        MessageProperties.ConversionRecordProperties instance = new MessageProperties.ConversionRecordProperties();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          MessageProperties.ConversionRecordProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static MessageProperties.ConversionRecordProperties Deserialize(
        byte[] buffer,
        MessageProperties.ConversionRecordProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          MessageProperties.ConversionRecordProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static MessageProperties.ConversionRecordProperties Deserialize(
        Stream stream,
        MessageProperties.ConversionRecordProperties instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_7;
            case 10:
              instance.Source = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Data = ProtocolParser.ReadBytes(stream);
              continue;
            case 24:
              instance.DelaySeconds = new uint?(ProtocolParser.ReadUInt32(stream));
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

      public static MessageProperties.ConversionRecordProperties DeserializeLengthDelimited(
        Stream stream,
        MessageProperties.ConversionRecordProperties instance)
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
              instance.Source = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Data = ProtocolParser.ReadBytes(stream);
              continue;
            case 24:
              instance.DelaySeconds = new uint?(ProtocolParser.ReadUInt32(stream));
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

      public static MessageProperties.ConversionRecordProperties DeserializeLength(
        Stream stream,
        int length,
        MessageProperties.ConversionRecordProperties instance)
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
              instance.Source = ProtocolParser.ReadString(stream);
              continue;
            case 18:
              instance.Data = ProtocolParser.ReadBytes(stream);
              continue;
            case 24:
              instance.DelaySeconds = new uint?(ProtocolParser.ReadUInt32(stream));
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
        MessageProperties.ConversionRecordProperties instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Source != null)
        {
          stream.WriteByte((byte) 10);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Source));
        }
        if (instance.Data != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, instance.Data);
        }
        if (instance.DelaySeconds.HasValue)
        {
          stream.WriteByte((byte) 24);
          ProtocolParser.WriteUInt32(stream, instance.DelaySeconds.Value);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(
        MessageProperties.ConversionRecordProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          MessageProperties.ConversionRecordProperties.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        MessageProperties.ConversionRecordProperties instance)
      {
        byte[] bytes = MessageProperties.ConversionRecordProperties.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }
    }

    public class BizProperties
    {
      public ulong? Serial { get; set; }

      public byte[] Cert { get; set; }

      public string Level { get; set; }

      public static MessageProperties.BizProperties Deserialize(Stream stream)
      {
        MessageProperties.BizProperties instance = new MessageProperties.BizProperties();
        MessageProperties.BizProperties.Deserialize(stream, instance);
        return instance;
      }

      public static MessageProperties.BizProperties DeserializeLengthDelimited(Stream stream)
      {
        MessageProperties.BizProperties instance = new MessageProperties.BizProperties();
        MessageProperties.BizProperties.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static MessageProperties.BizProperties DeserializeLength(Stream stream, int length)
      {
        MessageProperties.BizProperties instance = new MessageProperties.BizProperties();
        MessageProperties.BizProperties.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static MessageProperties.BizProperties Deserialize(byte[] buffer)
      {
        MessageProperties.BizProperties instance = new MessageProperties.BizProperties();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          MessageProperties.BizProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static MessageProperties.BizProperties Deserialize(
        byte[] buffer,
        MessageProperties.BizProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          MessageProperties.BizProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static MessageProperties.BizProperties Deserialize(
        Stream stream,
        MessageProperties.BizProperties instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_7;
            case 8:
              instance.Serial = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 18:
              instance.Cert = ProtocolParser.ReadBytes(stream);
              continue;
            case 26:
              instance.Level = ProtocolParser.ReadString(stream);
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

      public static MessageProperties.BizProperties DeserializeLengthDelimited(
        Stream stream,
        MessageProperties.BizProperties instance)
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
              instance.Serial = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 18:
              instance.Cert = ProtocolParser.ReadBytes(stream);
              continue;
            case 26:
              instance.Level = ProtocolParser.ReadString(stream);
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

      public static MessageProperties.BizProperties DeserializeLength(
        Stream stream,
        int length,
        MessageProperties.BizProperties instance)
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
              instance.Serial = new ulong?(ProtocolParser.ReadUInt64(stream));
              continue;
            case 18:
              instance.Cert = ProtocolParser.ReadBytes(stream);
              continue;
            case 26:
              instance.Level = ProtocolParser.ReadString(stream);
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

      public static void Serialize(Stream stream, MessageProperties.BizProperties instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Serial.HasValue)
        {
          stream.WriteByte((byte) 8);
          ProtocolParser.WriteUInt64(stream, instance.Serial.Value);
        }
        if (instance.Cert != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, instance.Cert);
        }
        if (instance.Level != null)
        {
          stream.WriteByte((byte) 26);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Level));
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(MessageProperties.BizProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          MessageProperties.BizProperties.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        MessageProperties.BizProperties instance)
      {
        byte[] bytes = MessageProperties.BizProperties.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }
    }
  }
}
