// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProtoBuf.PaymentInfo
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;
using System.Text;


namespace WhatsApp.ProtoBuf
{
  public class PaymentInfo
  {
    [Obsolete]
    public PaymentInfo.Currency? CurrencyDeprecated { get; set; }

    public ulong? Amount1000 { get; set; }

    public string ReceiverJid { get; set; }

    public PaymentInfo.Status? status { get; set; }

    public ulong? TransactionTimestamp { get; set; }

    public MessageKey RequestMessageKey { get; set; }

    public ulong? ExpiryTimestamp { get; set; }

    public bool? Futureproofed { get; set; }

    public string currency { get; set; }

    public static PaymentInfo Deserialize(Stream stream)
    {
      PaymentInfo instance = new PaymentInfo();
      PaymentInfo.Deserialize(stream, instance);
      return instance;
    }

    public static PaymentInfo DeserializeLengthDelimited(Stream stream)
    {
      PaymentInfo instance = new PaymentInfo();
      PaymentInfo.DeserializeLengthDelimited(stream, instance);
      return instance;
    }

    public static PaymentInfo DeserializeLength(Stream stream, int length)
    {
      PaymentInfo instance = new PaymentInfo();
      PaymentInfo.DeserializeLength(stream, length, instance);
      return instance;
    }

    public static PaymentInfo Deserialize(byte[] buffer)
    {
      PaymentInfo instance = new PaymentInfo();
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        PaymentInfo.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static PaymentInfo Deserialize(byte[] buffer, PaymentInfo instance)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
        PaymentInfo.Deserialize((Stream) memoryStream, instance);
      return instance;
    }

    public static PaymentInfo Deserialize(Stream stream, PaymentInfo instance)
    {
      while (true)
      {
        int firstByte = stream.ReadByte();
        switch (firstByte)
        {
          case -1:
            goto label_15;
          case 8:
            instance.CurrencyDeprecated = new PaymentInfo.Currency?((PaymentInfo.Currency) ProtocolParser.ReadUInt64(stream));
            continue;
          case 16:
            instance.Amount1000 = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 26:
            instance.ReceiverJid = ProtocolParser.ReadString(stream);
            continue;
          case 32:
            instance.status = new PaymentInfo.Status?((PaymentInfo.Status) ProtocolParser.ReadUInt64(stream));
            continue;
          case 40:
            instance.TransactionTimestamp = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 50:
            if (instance.RequestMessageKey == null)
            {
              instance.RequestMessageKey = MessageKey.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageKey.DeserializeLengthDelimited(stream, instance.RequestMessageKey);
            continue;
          case 56:
            instance.ExpiryTimestamp = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 64:
            instance.Futureproofed = new bool?(ProtocolParser.ReadBool(stream));
            continue;
          case 74:
            instance.currency = ProtocolParser.ReadString(stream);
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

    public static PaymentInfo DeserializeLengthDelimited(Stream stream, PaymentInfo instance)
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
            instance.CurrencyDeprecated = new PaymentInfo.Currency?((PaymentInfo.Currency) ProtocolParser.ReadUInt64(stream));
            continue;
          case 16:
            instance.Amount1000 = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 26:
            instance.ReceiverJid = ProtocolParser.ReadString(stream);
            continue;
          case 32:
            instance.status = new PaymentInfo.Status?((PaymentInfo.Status) ProtocolParser.ReadUInt64(stream));
            continue;
          case 40:
            instance.TransactionTimestamp = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 50:
            if (instance.RequestMessageKey == null)
            {
              instance.RequestMessageKey = MessageKey.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageKey.DeserializeLengthDelimited(stream, instance.RequestMessageKey);
            continue;
          case 56:
            instance.ExpiryTimestamp = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 64:
            instance.Futureproofed = new bool?(ProtocolParser.ReadBool(stream));
            continue;
          case 74:
            instance.currency = ProtocolParser.ReadString(stream);
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

    public static PaymentInfo DeserializeLength(Stream stream, int length, PaymentInfo instance)
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
            instance.CurrencyDeprecated = new PaymentInfo.Currency?((PaymentInfo.Currency) ProtocolParser.ReadUInt64(stream));
            continue;
          case 16:
            instance.Amount1000 = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 26:
            instance.ReceiverJid = ProtocolParser.ReadString(stream);
            continue;
          case 32:
            instance.status = new PaymentInfo.Status?((PaymentInfo.Status) ProtocolParser.ReadUInt64(stream));
            continue;
          case 40:
            instance.TransactionTimestamp = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 50:
            if (instance.RequestMessageKey == null)
            {
              instance.RequestMessageKey = MessageKey.DeserializeLengthDelimited(stream);
              continue;
            }
            MessageKey.DeserializeLengthDelimited(stream, instance.RequestMessageKey);
            continue;
          case 56:
            instance.ExpiryTimestamp = new ulong?(ProtocolParser.ReadUInt64(stream));
            continue;
          case 64:
            instance.Futureproofed = new bool?(ProtocolParser.ReadBool(stream));
            continue;
          case 74:
            instance.currency = ProtocolParser.ReadString(stream);
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

    public static void Serialize(Stream stream, PaymentInfo instance)
    {
      MemoryStream stream1 = ProtocolParser.Stack.Pop();
      if (instance.CurrencyDeprecated.HasValue)
      {
        stream.WriteByte((byte) 8);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.CurrencyDeprecated.Value);
      }
      if (instance.Amount1000.HasValue)
      {
        stream.WriteByte((byte) 16);
        ProtocolParser.WriteUInt64(stream, instance.Amount1000.Value);
      }
      if (instance.ReceiverJid != null)
      {
        stream.WriteByte((byte) 26);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.ReceiverJid));
      }
      if (instance.status.HasValue)
      {
        stream.WriteByte((byte) 32);
        ProtocolParser.WriteUInt64(stream, (ulong) instance.status.Value);
      }
      if (instance.TransactionTimestamp.HasValue)
      {
        stream.WriteByte((byte) 40);
        ProtocolParser.WriteUInt64(stream, instance.TransactionTimestamp.Value);
      }
      if (instance.RequestMessageKey != null)
      {
        stream.WriteByte((byte) 50);
        stream1.SetLength(0L);
        MessageKey.Serialize((Stream) stream1, instance.RequestMessageKey);
        uint length = (uint) stream1.Length;
        ProtocolParser.WriteUInt32(stream, length);
        stream1.WriteTo(stream);
      }
      if (instance.ExpiryTimestamp.HasValue)
      {
        stream.WriteByte((byte) 56);
        ProtocolParser.WriteUInt64(stream, instance.ExpiryTimestamp.Value);
      }
      if (instance.Futureproofed.HasValue)
      {
        stream.WriteByte((byte) 64);
        ProtocolParser.WriteBool(stream, instance.Futureproofed.Value);
      }
      if (instance.currency != null)
      {
        stream.WriteByte((byte) 74);
        ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.currency));
      }
      ProtocolParser.Stack.Push(stream1);
    }

    public static byte[] SerializeToBytes(PaymentInfo instance)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        PaymentInfo.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public static void SerializeLengthDelimited(Stream stream, PaymentInfo instance)
    {
      byte[] bytes = PaymentInfo.SerializeToBytes(instance);
      ProtocolParser.WriteUInt32(stream, (uint) bytes.Length);
      stream.Write(bytes, 0, bytes.Length);
    }

    public enum Currency
    {
      UNKNOWN_CURRENCY,
      INR,
    }

    public enum Status
    {
      UNKNOWN_STATUS,
      PROCESSING,
      SENT,
      NEED_TO_ACCEPT,
      COMPLETE,
      COULD_NOT_COMPLETE,
      REFUNDED,
      EXPIRED,
      REJECTED,
      CANCELLED,
      WAITING_FOR_PAYER,
      WAITING,
    }
  }
}
