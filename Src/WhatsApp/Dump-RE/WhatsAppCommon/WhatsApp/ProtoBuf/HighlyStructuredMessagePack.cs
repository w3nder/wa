// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.HighlyStructuredMessagePack
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
  public class HighlyStructuredMessagePack
  {
    public string Namespace { get; set; }

    public string Lg { get; set; }

    public string Lc { get; set; }

    public uint? Version { get; set; }

    public List<HighlyStructuredMessagePack.HighlyStructuredMessageTranslation> Translations { get; set; }

    public static HighlyStructuredMessagePack Deserialize(Stream stream)
    {
      HighlyStructuredMessagePack instance = new HighlyStructuredMessagePack();
      HighlyStructuredMessagePack.Deserialize(stream, instance);
      return instance;
    }

    public static HighlyStructuredMessagePack DeserializeLengthDelimited(Stream stream)
    {
      HighlyStructuredMessagePack instance = new HighlyStructuredMessagePack();
      HighlyStructuredMessagePack.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static HighlyStructuredMessagePack DeserializeLength(Stream stream, int length)
    {
      HighlyStructuredMessagePack instance = new HighlyStructuredMessagePack();
      HighlyStructuredMessagePack.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static HighlyStructuredMessagePack Deserialize(byte[] buffer)
    {
      HighlyStructuredMessagePack instance = new HighlyStructuredMessagePack();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        HighlyStructuredMessagePack.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static HighlyStructuredMessagePack Deserialize(
      byte[] buffer,
      HighlyStructuredMessagePack instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        HighlyStructuredMessagePack.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static HighlyStructuredMessagePack Deserialize(
      Stream stream,
      HighlyStructuredMessagePack instance)
    {
      if (instance.Translations == null)
        instance.Translations = new List<HighlyStructuredMessagePack.HighlyStructuredMessageTranslation>();
      while (true)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            goto label_11;
          case 10:
            instance.Namespace = ProtocolParser.ReadString(stream);
            continue;
          case 18:
            instance.Lg = ProtocolParser.ReadString(stream);
            continue;
          case 26:
            instance.Lc = ProtocolParser.ReadString(stream);
            continue;
          case 32:
            instance.Version = new uint?(ProtocolParser.ReadUInt32(stream));
            continue;
          case 42:
            instance.Translations.Add(HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.DeserializeLengthDelimited(stream));
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

    public static HighlyStructuredMessagePack DeserializeLengthDelimited(
      Stream stream,
      HighlyStructuredMessagePack instance)
    {
      if (instance.Translations == null)
        instance.Translations = new List<HighlyStructuredMessagePack.HighlyStructuredMessageTranslation>();
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
            instance.Lg = ProtocolParser.ReadString(stream);
            continue;
          case 26:
            instance.Lc = ProtocolParser.ReadString(stream);
            continue;
          case 32:
            instance.Version = new uint?(ProtocolParser.ReadUInt32(stream));
            continue;
          case 42:
            instance.Translations.Add(HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.DeserializeLengthDelimited(stream));
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

    public static HighlyStructuredMessagePack DeserializeLength(
      Stream stream,
      int length,
      HighlyStructuredMessagePack instance)
    {
      if (instance.Translations == null)
        instance.Translations = new List<HighlyStructuredMessagePack.HighlyStructuredMessageTranslation>();
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
            instance.Lg = ProtocolParser.ReadString(stream);
            continue;
          case 26:
            instance.Lc = ProtocolParser.ReadString(stream);
            continue;
          case 32:
            instance.Version = new uint?(ProtocolParser.ReadUInt32(stream));
            continue;
          case 42:
            instance.Translations.Add(HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.DeserializeLengthDelimited(stream));
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

    public static void Serialize(Stream stream, HighlyStructuredMessagePack instance)
    {
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.Namespace != null)
      {
        stream.WriteByte((byte) 10);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Namespace));
      }
      if (instance.Lg != null)
      {
        stream.WriteByte((byte) 18);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Lg));
      }
      if (instance.Lc != null)
      {
        stream.WriteByte((byte) 26);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Lc));
      }
      if (instance.Version.HasValue)
      {
        stream.WriteByte((byte) 32);
        ProtocolParser.WriteUInt32(stream, instance.Version.Value);
      }
      if (instance.Translations != null)
      {
        foreach (HighlyStructuredMessagePack.HighlyStructuredMessageTranslation translation in instance.Translations)
        {
          stream.WriteByte((byte) 42);
          stream1.SetLength(0L);
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.Serialize((Stream) stream1, translation);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(HighlyStructuredMessagePack instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        HighlyStructuredMessagePack.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, HighlyStructuredMessagePack instance)
    {
      byte[] bytes = HighlyStructuredMessagePack.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }

    public class HighlyStructuredMessageTranslation
    {
      public HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement Element { get; set; }

      public string TranslatedText { get; set; }

      public uint? PluralParamNo { get; set; }

      public List<HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException> PluralExceptions { get; set; }

      public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation Deserialize(
        Stream stream)
      {
        HighlyStructuredMessagePack.HighlyStructuredMessageTranslation instance = new HighlyStructuredMessagePack.HighlyStructuredMessageTranslation();
        HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.Deserialize(stream, instance);
        return instance;
      }

      public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation DeserializeLengthDelimited(
        Stream stream)
      {
        HighlyStructuredMessagePack.HighlyStructuredMessageTranslation instance = new HighlyStructuredMessagePack.HighlyStructuredMessageTranslation();
        HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.DeserializeLengthDelimited(stream, instance);
        return instance;
      }

      public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation DeserializeLength(
        Stream stream,
        int length)
      {
        HighlyStructuredMessagePack.HighlyStructuredMessageTranslation instance = new HighlyStructuredMessagePack.HighlyStructuredMessageTranslation();
        HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.DeserializeLength(stream, length, instance);
        return instance;
      }

      public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation Deserialize(
        byte[] buffer)
      {
        HighlyStructuredMessagePack.HighlyStructuredMessageTranslation instance = new HighlyStructuredMessagePack.HighlyStructuredMessageTranslation();
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation Deserialize(
        byte[] buffer,
        HighlyStructuredMessagePack.HighlyStructuredMessageTranslation instance)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.Deserialize((Stream) memoryStream, instance);
        return instance;
      }

      public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation Deserialize(
        Stream stream,
        HighlyStructuredMessagePack.HighlyStructuredMessageTranslation instance)
      {
        if (instance.PluralExceptions == null)
          instance.PluralExceptions = new List<HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException>();
        while (true)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              goto label_12;
            case 10:
              if (instance.Element == null)
              {
                instance.Element = HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement.DeserializeLengthDelimited(stream);
                continue;
              }
              HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement.DeserializeLengthDelimited(stream, instance.Element);
              continue;
            case 18:
              instance.TranslatedText = ProtocolParser.ReadString(stream);
              continue;
            case 24:
              instance.PluralParamNo = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 34:
              instance.PluralExceptions.Add(HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException.DeserializeLengthDelimited(stream));
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

      public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation DeserializeLengthDelimited(
        Stream stream,
        HighlyStructuredMessagePack.HighlyStructuredMessageTranslation instance)
      {
        if (instance.PluralExceptions == null)
          instance.PluralExceptions = new List<HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException>();
        long num = (long) ProtocolParser.ReadUInt32(stream) + stream.Position;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 10:
              if (instance.Element == null)
              {
                instance.Element = HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement.DeserializeLengthDelimited(stream);
                continue;
              }
              HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement.DeserializeLengthDelimited(stream, instance.Element);
              continue;
            case 18:
              instance.TranslatedText = ProtocolParser.ReadString(stream);
              continue;
            case 24:
              instance.PluralParamNo = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 34:
              instance.PluralExceptions.Add(HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException.DeserializeLengthDelimited(stream));
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

      public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation DeserializeLength(
        Stream stream,
        int length,
        HighlyStructuredMessagePack.HighlyStructuredMessageTranslation instance)
      {
        if (instance.PluralExceptions == null)
          instance.PluralExceptions = new List<HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException>();
        long num = stream.Position + (long) length;
        while (stream.Position < num)
        {
          int firstByte = stream.ReadByte();
          switch (firstByte)
          {
            case -1:
              throw new EndOfStreamException();
            case 10:
              if (instance.Element == null)
              {
                instance.Element = HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement.DeserializeLengthDelimited(stream);
                continue;
              }
              HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement.DeserializeLengthDelimited(stream, instance.Element);
              continue;
            case 18:
              instance.TranslatedText = ProtocolParser.ReadString(stream);
              continue;
            case 24:
              instance.PluralParamNo = new uint?(ProtocolParser.ReadUInt32(stream));
              continue;
            case 34:
              instance.PluralExceptions.Add(HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException.DeserializeLengthDelimited(stream));
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
        HighlyStructuredMessagePack.HighlyStructuredMessageTranslation instance)
      {
        MemoryStream stream1 = ProtocolParser.Stack.Pop();
        if (instance.Element != null)
        {
          stream.WriteByte((byte) 10);
          stream1.SetLength(0L);
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement.Serialize((Stream) stream1, instance.Element);
          uint length = (uint) stream1.Length;
          ProtocolParser.WriteUInt32(stream, length);
          stream1.WriteTo(stream);
        }
        if (instance.TranslatedText != null)
        {
          stream.WriteByte((byte) 18);
          ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.TranslatedText));
        }
        if (instance.PluralParamNo.HasValue)
        {
          stream.WriteByte((byte) 24);
          ProtocolParser.WriteUInt32(stream, instance.PluralParamNo.Value);
        }
        if (instance.PluralExceptions != null)
        {
          foreach (HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException pluralException in instance.PluralExceptions)
          {
            stream.WriteByte((byte) 34);
            stream1.SetLength(0L);
            HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException.Serialize((Stream) stream1, pluralException);
            uint length = (uint) stream1.Length;
            ProtocolParser.WriteUInt32(stream, length);
            stream1.WriteTo(stream);
          }
        }
        ProtocolParser.Stack.Push(stream1);
      }

      public static byte[] SerializeToBytes(
        HighlyStructuredMessagePack.HighlyStructuredMessageTranslation instance)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }

      public static void SerializeLengthDelimited(
        Stream stream,
        HighlyStructuredMessagePack.HighlyStructuredMessageTranslation instance)
      {
        byte[] bytes = HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.SerializeToBytes(instance);
        ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
      }

      public class HighlyStructuredMessageElement
      {
        public string Namespace { get; set; }

        public string ElementName { get; set; }

        public uint? NumParams { get; set; }

        public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement Deserialize(
          Stream stream)
        {
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement instance = new HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement();
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement.Deserialize(stream, instance);
          return instance;
        }

        public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement DeserializeLengthDelimited(
          Stream stream)
        {
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement instance = new HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement();
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement.DeserializeLengthDelimited(stream, instance);
          return instance;
        }

        public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement DeserializeLength(
          Stream stream,
          int length)
        {
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement instance = new HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement();
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement.DeserializeLength(stream, length, instance);
          return instance;
        }

        public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement Deserialize(
          byte[] buffer)
        {
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement instance = new HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement();
          using (MemoryStream memoryStream = new MemoryStream(buffer))
            HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement.Deserialize((Stream) memoryStream, instance);
          return instance;
        }

        public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement Deserialize(
          byte[] buffer,
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement instance)
        {
          using (MemoryStream memoryStream = new MemoryStream(buffer))
            HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement.Deserialize((Stream) memoryStream, instance);
          return instance;
        }

        public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement Deserialize(
          Stream stream,
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement instance)
        {
          while (true)
          {
            int firstByte = stream.ReadByte();
            switch (firstByte)
            {
              case -1:
                goto label_7;
              case 10:
                instance.Namespace = ProtocolParser.ReadString(stream);
                continue;
              case 18:
                instance.ElementName = ProtocolParser.ReadString(stream);
                continue;
              case 24:
                instance.NumParams = new uint?(ProtocolParser.ReadUInt32(stream));
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

        public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement DeserializeLengthDelimited(
          Stream stream,
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement instance)
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
                instance.Namespace = ProtocolParser.ReadString(stream);
                continue;
              case 18:
                instance.ElementName = ProtocolParser.ReadString(stream);
                continue;
              case 24:
                instance.NumParams = new uint?(ProtocolParser.ReadUInt32(stream));
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

        public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement DeserializeLength(
          Stream stream,
          int length,
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement instance)
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
                instance.Namespace = ProtocolParser.ReadString(stream);
                continue;
              case 18:
                instance.ElementName = ProtocolParser.ReadString(stream);
                continue;
              case 24:
                instance.NumParams = new uint?(ProtocolParser.ReadUInt32(stream));
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
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement instance)
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
          if (instance.NumParams.HasValue)
          {
            stream.WriteByte((byte) 24);
            ProtocolParser.WriteUInt32(stream, instance.NumParams.Value);
          }
          ProtocolParser.Stack.Push(stream1);
        }

        public static byte[] SerializeToBytes(
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement instance)
        {
          using (MemoryStream memoryStream = new MemoryStream())
          {
            HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement.Serialize((Stream) memoryStream, instance);
            return memoryStream.ToArray();
          }
        }

        public static void SerializeLengthDelimited(
          Stream stream,
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement instance)
        {
          byte[] bytes = HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.HighlyStructuredMessageElement.SerializeToBytes(instance);
          ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
          stream.Write(bytes, 0, bytes.Length);
        }
      }

      public class TranslationPluralException
      {
        public HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException.PluralQuantityType? Qty { get; set; }

        public string TranslatedText { get; set; }

        public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException Deserialize(
          Stream stream)
        {
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException instance = new HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException();
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException.Deserialize(stream, instance);
          return instance;
        }

        public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException DeserializeLengthDelimited(
          Stream stream)
        {
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException instance = new HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException();
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException.DeserializeLengthDelimited(stream, instance);
          return instance;
        }

        public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException DeserializeLength(
          Stream stream,
          int length)
        {
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException instance = new HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException();
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException.DeserializeLength(stream, length, instance);
          return instance;
        }

        public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException Deserialize(
          byte[] buffer)
        {
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException instance = new HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException();
          using (MemoryStream memoryStream = new MemoryStream(buffer))
            HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException.Deserialize((Stream) memoryStream, instance);
          return instance;
        }

        public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException Deserialize(
          byte[] buffer,
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException instance)
        {
          using (MemoryStream memoryStream = new MemoryStream(buffer))
            HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException.Deserialize((Stream) memoryStream, instance);
          return instance;
        }

        public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException Deserialize(
          Stream stream,
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException instance)
        {
          while (true)
          {
            int firstByte = stream.ReadByte();
            switch (firstByte)
            {
              case -1:
                goto label_6;
              case 8:
                instance.Qty = new HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException.PluralQuantityType?((HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException.PluralQuantityType) ProtocolParser.ReadUInt64(stream));
                continue;
              case 18:
                instance.TranslatedText = ProtocolParser.ReadString(stream);
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

        public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException DeserializeLengthDelimited(
          Stream stream,
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException instance)
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
                instance.Qty = new HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException.PluralQuantityType?((HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException.PluralQuantityType) ProtocolParser.ReadUInt64(stream));
                continue;
              case 18:
                instance.TranslatedText = ProtocolParser.ReadString(stream);
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

        public static HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException DeserializeLength(
          Stream stream,
          int length,
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException instance)
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
                instance.Qty = new HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException.PluralQuantityType?((HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException.PluralQuantityType) ProtocolParser.ReadUInt64(stream));
                continue;
              case 18:
                instance.TranslatedText = ProtocolParser.ReadString(stream);
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
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException instance)
        {
          MemoryStream stream1 = ProtocolParser.Stack.Pop();
          if (instance.Qty.HasValue)
          {
            stream.WriteByte((byte) 8);
            ProtocolParser.WriteUInt64(stream, (ulong) instance.Qty.Value);
          }
          if (instance.TranslatedText != null)
          {
            stream.WriteByte((byte) 18);
            ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.TranslatedText));
          }
          ProtocolParser.Stack.Push(stream1);
        }

        public static byte[] SerializeToBytes(
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException instance)
        {
          using (MemoryStream memoryStream = new MemoryStream())
          {
            HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException.Serialize((Stream) memoryStream, instance);
            return memoryStream.ToArray();
          }
        }

        public static void SerializeLengthDelimited(
          Stream stream,
          HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException instance)
        {
          byte[] bytes = HighlyStructuredMessagePack.HighlyStructuredMessageTranslation.TranslationPluralException.SerializeToBytes(instance);
          ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
          stream.Write(bytes, 0, bytes.Length);
        }

        public enum PluralQuantityType
        {
          ZERO,
          ONE,
          TWO,
          FEW,
          MANY,
          OTHER,
        }
      }
    }
  }
}
