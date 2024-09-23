// Decompiled with JetBrains decompiler
// Type: WhatsApp.ConversationProperties
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;


namespace WhatsApp
{
  public class ConversationProperties
  {
    private Conversation Conversation;

    public static bool HasConversationProperties(Conversation conversation)
    {
      return conversation.InternalProperties != null;
    }

    public static ConversationProperties GetForConversation(Conversation conversation)
    {
      ConversationProperties forConversation = conversation.InternalProperties ?? new ConversationProperties();
      forConversation.Conversation = conversation;
      return forConversation;
    }

    public void Save()
    {
      if (this.Conversation == null)
        throw new ArgumentNullException("No Conversation associated with these properties");
      this.Conversation.InternalProperties = this;
    }

    public void SetForConversation(Conversation conversation)
    {
      this.Conversation = conversation.InternalProperties == null ? conversation : throw new ArgumentNullException("Overriding ConversationProperties in Converastion, Jid: " + conversation.Jid);
      conversation.InternalProperties = this;
    }

    public ConversationProperties.VerifiedNameProperties VerifiedNamePropertiesField { get; set; }

    public static ConversationProperties Deserialize(Stream stream)
    {
      ConversationProperties instance = new ConversationProperties();
      ConversationProperties.Deserialize(stream, instance);
      return instance;
    }

    public static ConversationProperties DeserializeLengthDelimited(Stream stream)
    {
      ConversationProperties instance = new ConversationProperties();
      ConversationProperties.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static ConversationProperties DeserializeLength(Stream stream, int length)
    {
      ConversationProperties instance = new ConversationProperties();
      ConversationProperties.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static ConversationProperties Deserialize(byte[] buffer)
    {
      ConversationProperties instance = new ConversationProperties();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        ConversationProperties.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static ConversationProperties Deserialize(byte[] buffer, ConversationProperties instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        ConversationProperties.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static ConversationProperties Deserialize(Stream stream, ConversationProperties instance)
    {
      while (true)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            goto label_7;
          case 10:
            if (instance.VerifiedNamePropertiesField == null)
            {
              instance.VerifiedNamePropertiesField = ConversationProperties.VerifiedNameProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            ConversationProperties.VerifiedNameProperties.DeserializeLengthDelimited(stream, instance.VerifiedNamePropertiesField);
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

    public static ConversationProperties DeserializeLengthDelimited(
      Stream stream,
      ConversationProperties instance)
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
            if (instance.VerifiedNamePropertiesField == null)
            {
              instance.VerifiedNamePropertiesField = ConversationProperties.VerifiedNameProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            ConversationProperties.VerifiedNameProperties.DeserializeLengthDelimited(stream, instance.VerifiedNamePropertiesField);
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

    public static ConversationProperties DeserializeLength(
      Stream stream,
      int length,
      ConversationProperties instance)
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
            if (instance.VerifiedNamePropertiesField == null)
            {
              instance.VerifiedNamePropertiesField = ConversationProperties.VerifiedNameProperties.DeserializeLengthDelimited(stream);
              continue;
            }
            ConversationProperties.VerifiedNameProperties.DeserializeLengthDelimited(stream, instance.VerifiedNamePropertiesField);
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

    public static void Serialize(Stream stream, ConversationProperties instance)
    {
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.VerifiedNamePropertiesField != null)
      {
        stream.WriteByte((byte) 10);
        stream1.SetLength(0L);
        ConversationProperties.VerifiedNameProperties.Serialize((Stream) stream1, instance.VerifiedNamePropertiesField);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(ConversationProperties instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        ConversationProperties.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, ConversationProperties instance)
    {
      byte[] bytes = ConversationProperties.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }

    public class VerifiedNameProperties
    {
      public bool? Enterprise { get; set; }

      public static ConversationProperties.VerifiedNameProperties Deserialize(Stream stream)
      {
        ConversationProperties.VerifiedNameProperties instance = new ConversationProperties.VerifiedNameProperties();
        ConversationProperties.VerifiedNameProperties.Deserialize(stream, instance);
        return instance;
      }

      public static ConversationProperties.VerifiedNameProperties DeserializeLengthDelimited(
        Stream stream)
      {
        ConversationProperties.VerifiedNameProperties instance = new ConversationProperties.VerifiedNameProperties();
        ConversationProperties.VerifiedNameProperties.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static ConversationProperties.VerifiedNameProperties DeserializeLength(
        Stream stream,
        int length)
      {
        ConversationProperties.VerifiedNameProperties instance = new ConversationProperties.VerifiedNameProperties();
        ConversationProperties.VerifiedNameProperties.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static ConversationProperties.VerifiedNameProperties Deserialize(byte[] buffer)
      {
        ConversationProperties.VerifiedNameProperties instance = new ConversationProperties.VerifiedNameProperties();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          ConversationProperties.VerifiedNameProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static ConversationProperties.VerifiedNameProperties Deserialize(
        byte[] buffer,
        ConversationProperties.VerifiedNameProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          ConversationProperties.VerifiedNameProperties.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static ConversationProperties.VerifiedNameProperties Deserialize(
        Stream stream,
        ConversationProperties.VerifiedNameProperties instance)
      {
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_5;
            case 8:
              instance.Enterprise = new bool?(ProtocolParser.ReadBool(stream));
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

      public static ConversationProperties.VerifiedNameProperties DeserializeLengthDelimited(
        Stream stream,
        ConversationProperties.VerifiedNameProperties instance)
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
              instance.Enterprise = new bool?(ProtocolParser.ReadBool(stream));
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

      public static ConversationProperties.VerifiedNameProperties DeserializeLength(
        Stream stream,
        int length,
        ConversationProperties.VerifiedNameProperties instance)
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
              instance.Enterprise = new bool?(ProtocolParser.ReadBool(stream));
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
        ConversationProperties.VerifiedNameProperties instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Enterprise.HasValue)
        {
          stream.WriteByte((byte) 8);
          ProtocolParser.WriteBool(stream, instance.Enterprise.Value);
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(
        ConversationProperties.VerifiedNameProperties instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          ConversationProperties.VerifiedNameProperties.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        ConversationProperties.VerifiedNameProperties instance)
      {
        byte[] bytes = ConversationProperties.VerifiedNameProperties.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }
    }
  }
}
