// Decompiled with JetBrains decompiler
// Type: WhatsApp.PendingMsgProperties
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

#nullable disable
namespace WhatsApp
{
  public class PendingMsgProperties
  {
    private PendingMessage pendingMessage;

    public PendingMsgProperties.LiveLocationProperties LiveLocationPropertiesField { get; set; }

    public static PendingMsgProperties Deserialize(Stream stream)
    {
      PendingMsgProperties instance = new PendingMsgProperties();
      PendingMsgProperties.Deserialize(stream, instance);
      return instance;
    }

    public static PendingMsgProperties DeserializeLengthDelimited(Stream stream)
    {
      PendingMsgProperties instance = new PendingMsgProperties();
      PendingMsgProperties.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static PendingMsgProperties DeserializeLength(Stream stream, int length)
    {
      PendingMsgProperties instance = new PendingMsgProperties();
      PendingMsgProperties.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static PendingMsgProperties Deserialize(byte[] buffer)
    {
      PendingMsgProperties instance = new PendingMsgProperties();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        PendingMsgProperties.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static PendingMsgProperties Deserialize(byte[] buffer, PendingMsgProperties instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        PendingMsgProperties.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static PendingMsgProperties Deserialize(Stream stream, PendingMsgProperties instance)
    {
      while (true)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            goto label_7;
          case 10:
            if (instance.LiveLocationPropertiesField == null)
            {
              instance.LiveLocationPropertiesField = PendingMsgProperties.LiveLocationProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            PendingMsgProperties.LiveLocationProperties.DeserializeLengthDelimited(stream, instance.LiveLocationPropertiesField);
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

    public static PendingMsgProperties DeserializeLengthDelimited(
      Stream stream,
      PendingMsgProperties instance)
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
            if (instance.LiveLocationPropertiesField == null)
            {
              instance.LiveLocationPropertiesField = PendingMsgProperties.LiveLocationProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            PendingMsgProperties.LiveLocationProperties.DeserializeLengthDelimited(stream, instance.LiveLocationPropertiesField);
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

    public static PendingMsgProperties DeserializeLength(
      Stream stream,
      int length,
      PendingMsgProperties instance)
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
            if (instance.LiveLocationPropertiesField == null)
            {
              instance.LiveLocationPropertiesField = PendingMsgProperties.LiveLocationProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            PendingMsgProperties.LiveLocationProperties.DeserializeLengthDelimited(stream, instance.LiveLocationPropertiesField);
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

    public static void Serialize(Stream stream, PendingMsgProperties instance)
    {
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.LiveLocationPropertiesField != null)
      {
        stream.WriteByte((byte) 10);
        stream1.SetLength(0L);
        PendingMsgProperties.LiveLocationProperties.Serialize((Stream) stream1, instance.LiveLocationPropertiesField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(PendingMsgProperties instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        PendingMsgProperties.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, PendingMsgProperties instance)
    {
      byte[] bytes = PendingMsgProperties.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }

    public static bool HasPendingMessageProperties(PendingMessage pendingMsg)
    {
      return pendingMsg.InternalProperties != null;
    }

    public static PendingMsgProperties GetForPendingMsg(PendingMessage pendingMsg)
    {
      PendingMsgProperties forPendingMsg = pendingMsg.InternalProperties ?? new PendingMsgProperties();
      forPendingMsg.pendingMessage = pendingMsg;
      return forPendingMsg;
    }

    public void Save()
    {
      if (this.pendingMessage == null)
        throw new ArgumentNullException("No Pending Message associated with these properties");
      this.pendingMessage.InternalProperties = this;
    }

    public void SetForPendingMsg(PendingMessage pendingMsg)
    {
      this.pendingMessage = pendingMsg.InternalProperties == null ? pendingMsg : throw new ArgumentNullException("Overriding PendingMessageProperties in Pending Message, id: " + (object) pendingMsg.PendingMessagesId);
      pendingMsg.InternalProperties = this;
    }

    public PendingMsgProperties.LiveLocationProperties EnsureLiveLocationProperties
    {
      get
      {
        if (this.LiveLocationPropertiesField == null)
          this.LiveLocationPropertiesField = new PendingMsgProperties.LiveLocationProperties();
        return this.LiveLocationPropertiesField;
      }
    }

    public class LiveLocationProperties
    {
      public int? DurationSeconds { get; set; }

      public static PendingMsgProperties.LiveLocationProperties Deserialize(Stream stream)
      {
        PendingMsgProperties.LiveLocationProperties instance = new PendingMsgProperties.LiveLocationProperties();
        PendingMsgProperties.LiveLocationProperties.Deserialize(stream, instance);
        return instance;
      }

      public static PendingMsgProperties.LiveLocationProperties DeserializeLengthDelimited(
        Stream stream)
      {
        PendingMsgProperties.LiveLocationProperties instance = new PendingMsgProperties.LiveLocationProperties();
        PendingMsgProperties.LiveLocationProperties.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static PendingMsgProperties.LiveLocationProperties DeserializeLength(
        Stream stream,
        int length)
      {
        PendingMsgProperties.LiveLocationProperties instance = new PendingMsgProperties.LiveLocationProperties();
        PendingMsgProperties.LiveLocationProperties.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static PendingMsgProperties.LiveLocationProperties Deserialize(byte[] buffer)
      {
        PendingMsgProperties.LiveLocationProperties instance = new PendingMsgProperties.LiveLocationProperties();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          PendingMsgProperties.LiveLocationProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static PendingMsgProperties.LiveLocationProperties Deserialize(
        byte[] buffer,
        PendingMsgProperties.LiveLocationProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          PendingMsgProperties.LiveLocationProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static PendingMsgProperties.LiveLocationProperties Deserialize(
        Stream stream,
        PendingMsgProperties.LiveLocationProperties instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_5;
            case 8:
              instance.DurationSeconds = new int?((int) ProtocolParser.ReadUInt64(stream));
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

      public static PendingMsgProperties.LiveLocationProperties DeserializeLengthDelimited(
        Stream stream,
        PendingMsgProperties.LiveLocationProperties instance)
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
              instance.DurationSeconds = new int?((int) ProtocolParser.ReadUInt64(stream));
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

      public static PendingMsgProperties.LiveLocationProperties DeserializeLength(
        Stream stream,
        int length,
        PendingMsgProperties.LiveLocationProperties instance)
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
              instance.DurationSeconds = new int?((int) ProtocolParser.ReadUInt64(stream));
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
        PendingMsgProperties.LiveLocationProperties instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.DurationSeconds.HasValue)
        {
          stream.WriteByte((byte) 8);
          ProtocolParser.WriteUInt64(stream, (ulong) instance.DurationSeconds.Value);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(
        PendingMsgProperties.LiveLocationProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          PendingMsgProperties.LiveLocationProperties.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        PendingMsgProperties.LiveLocationProperties instance)
      {
        byte[] bytes = PendingMsgProperties.LiveLocationProperties.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }
    }
  }
}
