// Decompiled with JetBrains decompiler
// Type: WhatsApp.FunXMPP
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using WhatsApp.CommonOps;
using WhatsApp.ProtoBuf;
using WhatsApp.WaCollections;
using WhatsAppCommon;


namespace WhatsApp
{
  public class FunXMPP
  {
    public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    public static NumberStyles WhatsAppNumberStyle = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
    public static Subject<Unit> ConnectionLostSubject = new Subject<Unit>();
    public static Subject<Unit> OfflineMarkerSubject = new Subject<Unit>();
    public static TokenDictionary Dictionary = new TokenDictionary();

    public static bool TryParseTimestamp(string str, out DateTime? dt, bool toLocal = false)
    {
      long result = 0;
      if (!string.IsNullOrEmpty(str) && long.TryParse(str, out result))
        return FunXMPP.TryParseTimestamp(result, out dt, toLocal);
      dt = new DateTime?();
      return false;
    }

    public static bool TryParseTimestamp(long l, out DateTime? dt, bool toLocal = false)
    {
      try
      {
        DateTime dateTime = FunXMPP.UnixEpoch.AddSeconds((double) l);
        if (toLocal)
          dateTime = dateTime.ToLocalTime();
        dt = new DateTime?(dateTime);
        return true;
      }
      catch (ArgumentOutOfRangeException ex)
      {
        string context = string.Format("error parsing timestamp: {0}", (object) l);
        Log.l((Exception) ex, context);
      }
      dt = new DateTime?();
      return false;
    }

    public static string GenerateMessageId()
    {
      byte[] bytes = BitConverter.GetBytes(FunRunner.CurrentServerTimeUtc.Ticks);
      byte[] src = Settings.MyJid != null ? Encoding.UTF8.GetBytes(Settings.MyJid) : new byte[0];
      byte[] randomBytes = Axolotl.GenerateRandomBytes(16);
      byte[] numArray = new byte[bytes.Length + src.Length + randomBytes.Length];
      System.Buffer.BlockCopy((Array) bytes, 0, (Array) numArray, 0, bytes.Length);
      System.Buffer.BlockCopy((Array) src, 0, (Array) numArray, bytes.Length, src.Length);
      System.Buffer.BlockCopy((Array) randomBytes, 0, (Array) numArray, bytes.Length + src.Length, randomBytes.Length);
      byte[] hash = MD5Core.GetHash(numArray);
      StringBuilder stringBuilder = new StringBuilder(18);
      for (int index = 0; index < 9; ++index)
        stringBuilder.Append(hash[index].ToString("X2"));
      return stringBuilder.ToString();
    }

    public delegate void ProcessStanzaHandler(Stream stanzaStream);

    public delegate FunXMPP.ProtocolTreeNode DecodeStanza(Stream stanzaStream);

    public interface StanzaProvider
    {
      event FunXMPP.ProcessStanzaHandler StanzaAvailable;

      event FunXMPP.DecodeStanza DecodeStanza;
    }

    public interface StanzaWriter
    {
      void WriteStanza(MemoryStream stanzaStream, bool useCompression);

      FunXMPP.TreeNodeWriter TreeNodeWriter { set; }
    }

    public interface TreeNodeWriter
    {
      void StreamEnd();

      void Write(FunXMPP.ProtocolTreeNode node, bool compress = false);
    }

    public class Logger
    {
      public static void LogStanza(FunXMPP.ProtocolTreeNode node)
      {
        StringBuilder sb = new StringBuilder("\n");
        FunXMPP.Logger.LogStanza(sb, "", node);
        Log.WriteLineDebug(sb.ToString());
      }

      private static void LogStanza(StringBuilder sb, string space, FunXMPP.ProtocolTreeNode node)
      {
        if (node == null)
        {
          sb.Append("end of stream.\n");
        }
        else
        {
          sb.Append(space);
          sb.Append('<');
          sb.Append(node.tag);
          if (node.attributes != null && node.attributes.Length != 0)
          {
            foreach (FunXMPP.KeyValue attribute in node.attributes)
              sb.AppendFormat(" {0}=\"{1}\"", (object) attribute.key, (object) attribute.value);
          }
          if (node.data != null && node.data.Length != 0)
          {
            sb.Append(">\n");
            sb.AppendFormat("{0}   [data]\n", (object) space);
            sb.AppendFormat("{0}</{1}>\n", (object) space, (object) node.tag);
          }
          else if (node.children == null || node.children.Length == 0)
          {
            sb.Append(" />\n");
          }
          else
          {
            sb.Append(">\n");
            string space1 = space + "   ";
            foreach (FunXMPP.ProtocolTreeNode child in node.children)
              FunXMPP.Logger.LogStanza(sb, space1, child);
            sb.AppendFormat("{0}</{1}>\n", (object) space, (object) node.tag);
          }
        }
      }
    }

    public class NullTreeNodeWriter : FunXMPP.TreeNodeWriter
    {
      public void StreamEnd()
      {
      }

      public void Write(FunXMPP.ProtocolTreeNode node, bool compress)
      {
      }
    }

    public class MemoryStanzaWriter : FunXMPP.StanzaWriter
    {
      public MemoryStream StanzaStream;

      public void WriteStanza(MemoryStream stanzaStream, bool useCompression)
      {
        this.StanzaStream = new MemoryStream();
        stanzaStream.CopyTo((Stream) this.StanzaStream);
      }

      public FunXMPP.TreeNodeWriter TreeNodeWriter { get; set; }
    }

    public class BinTag
    {
      public const byte STREAM_START = 1;
      public const byte STREAM_END = 2;
      public const byte LIST_EMPTY = 0;
      public const byte LIST_8 = 248;
      public const byte LIST_16 = 249;
      public const byte JID_PAIR = 250;
      public const byte HEX_8 = 251;
      public const byte BINARY_8 = 252;
      public const byte BINARY_20 = 253;
      public const byte BINARY_32 = 254;
      public const byte NIBBLE_8 = 255;
    }

    public class BinTreeNodeWriter : FunXMPP.TreeNodeWriter
    {
      protected internal TokenDictionary tokenMap;
      private object writeLock = new object();
      private FunXMPP.StanzaWriter Writer;
      protected internal MemoryStream _out;

      public BinTreeNodeWriter(FunXMPP.StanzaWriter writer, TokenDictionary tokenMap)
      {
        if (writer != null)
        {
          this.Writer = writer;
          writer.TreeNodeWriter = (FunXMPP.TreeNodeWriter) this;
        }
        this.tokenMap = tokenMap;
        this._out = new MemoryStream(2048);
      }

      public void Flush(bool useCompression = false)
      {
        if (this._out.Length == 0L || this.Writer == null)
          return;
        this._out.Position = 0L;
        this.Writer.WriteStanza(this._out, useCompression);
        this._out.Position = 0L;
        this._out.SetLength(0L);
      }

      public void StreamEnd()
      {
        lock (this.writeLock)
        {
          this.WriteListStart(1);
          this._out.WriteByte((byte) 2);
          this.Flush();
        }
      }

      public void Write(FunXMPP.ProtocolTreeNode node, bool useCompression = false)
      {
        lock (this.writeLock)
        {
          try
          {
            if (node == null)
              this._out.WriteByte((byte) 0);
            else
              this.WriteInternal(node);
            this.Flush(useCompression);
          }
          catch (Exception ex)
          {
            this._out.Position = 0L;
            this._out.SetLength(0L);
            throw;
          }
        }
      }

      internal void WriteInternal(FunXMPP.ProtocolTreeNode node)
      {
        this.WriteListStart(1 + (node.attributes == null ? 0 : node.attributes.Length * 2) + (node.children == null ? 0 : 1) + (node.data == null ? 0 : 1));
        this.WriteString(node.tag);
        this.WriteAttributes(node.attributes);
        if (node.data != null)
          this.WriteBytes(node.data);
        if (node.children == null)
          return;
        this.WriteListStart(node.children.Length);
        for (int index = 0; index < node.children.Length; ++index)
          this.WriteInternal(node.children[index]);
      }

      internal void WriteAttributes(FunXMPP.KeyValue[] attributes)
      {
        if (attributes == null)
          return;
        for (int index = 0; index < attributes.Length; ++index)
        {
          this.WriteString(attributes[index].key);
          this.WriteString(attributes[index].value);
        }
      }

      internal void WriteString(string tag)
      {
        int token = -1;
        int subdict = -1;
        if (this.tokenMap.TryGetToken(tag, ref subdict, ref token))
        {
          if (subdict >= 0)
            this.WriteToken(subdict);
          this.WriteToken(token);
        }
        else
        {
          int num = tag.IndexOf('@');
          if (num < 1)
          {
            this.WriteBytes(Encoding.UTF8.GetBytes(tag));
          }
          else
          {
            if (!JidChecker.CheckJidProtocolString(tag))
              JidChecker.MaybeSendJidErrorClb(nameof (WriteString), tag);
            string server = tag.Substring(num + 1);
            this.WriteJid(tag.Substring(0, num - 0), server);
          }
        }
      }

      internal void WriteJid(string user, string server)
      {
        this._out.WriteByte((byte) 250);
        if (user != null)
          this.WriteString(user);
        else
          this.WriteToken(0);
        this.WriteString(server);
      }

      internal void WriteToken(int intValue)
      {
        if (intValue >= 256)
          return;
        this._out.WriteByte((byte) intValue);
      }

      internal void WriteBytes(byte[] bytes)
      {
        int length = bytes.Length;
        byte[] buffer = bytes;
        if (length >= 1048576)
        {
          this._out.WriteByte((byte) 254);
          this.WriteInt31(length);
        }
        else if (length >= 256)
        {
          this._out.WriteByte((byte) 253);
          this.WriteInt20(length);
        }
        else
        {
          byte[] numArray1 = (byte[]) null;
          byte[] numArray2 = (byte[]) null;
          if (length < 128)
          {
            numArray1 = new byte[(length + 1) / 2];
            for (int index = 0; index < length; ++index)
            {
              byte num1 = bytes[index];
              int num2 = 0;
              switch (num1)
              {
                case 45:
                case 46:
                  num2 = (int) num1 - 45 + 10;
                  break;
                case 48:
                case 49:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                  num2 = (int) num1 - 48;
                  break;
                default:
                  numArray1 = (byte[]) null;
                  break;
              }
              if (numArray1 != null)
                numArray1[index / 2] |= (byte) (num2 << 4 * (1 - index % 2));
              else
                break;
            }
            if (numArray1 == null)
            {
              numArray2 = new byte[(length + 1) / 2];
              for (int index = 0; index < length; ++index)
              {
                byte num3 = bytes[index];
                int num4 = 0;
                switch (num3)
                {
                  case 48:
                  case 49:
                  case 50:
                  case 51:
                  case 52:
                  case 53:
                  case 54:
                  case 55:
                  case 56:
                  case 57:
                    num4 = (int) num3 - 48;
                    break;
                  case 65:
                  case 66:
                  case 67:
                  case 68:
                  case 69:
                  case 70:
                    num4 = (int) num3 - 65 + 10;
                    break;
                  default:
                    numArray2 = (byte[]) null;
                    break;
                }
                if (numArray2 != null)
                  numArray2[index / 2] |= (byte) (num4 << 4 * (1 - index % 2));
                else
                  break;
              }
            }
          }
          if (numArray1 != null)
          {
            if (length % 2 == 1)
              numArray1[numArray1.Length - 1] |= (byte) 15;
            buffer = numArray1;
            this._out.WriteByte(byte.MaxValue);
            this.WriteInt8(length % 2 << 7 | numArray1.Length);
          }
          else if (numArray2 != null)
          {
            if (length % 2 == 1)
              numArray2[numArray2.Length - 1] |= (byte) 15;
            buffer = numArray2;
            this._out.WriteByte((byte) 251);
            this.WriteInt8(length % 2 << 7 | numArray2.Length);
          }
          else
          {
            this._out.WriteByte((byte) 252);
            this.WriteInt8(length);
          }
        }
        this._out.Write(buffer, 0, buffer.Length);
      }

      internal void WriteInt8(int v) => this._out.WriteByte((byte) (v & (int) byte.MaxValue));

      internal void WriteInt16(int v)
      {
        FunXMPP.BinTreeNodeWriter.WriteInt16((Stream) this._out, v);
      }

      internal static void WriteInt16(Stream o, int v)
      {
        o.WriteByte((byte) ((v & 65280) >> 8));
        o.WriteByte((byte) (v & (int) byte.MaxValue));
      }

      internal void WriteInt20(int v)
      {
        this._out.WriteByte((byte) ((v & 16711680) >> 16));
        this._out.WriteByte((byte) ((v & 65280) >> 8));
        this._out.WriteByte((byte) (v & (int) byte.MaxValue));
      }

      internal void WriteInt31(int v)
      {
        this._out.WriteByte((byte) ((v & 2130706432) >> 24));
        this._out.WriteByte((byte) ((v & 16711680) >> 16));
        this._out.WriteByte((byte) ((v & 65280) >> 8));
        this._out.WriteByte((byte) (v & (int) byte.MaxValue));
      }

      internal void WriteListStart(int i)
      {
        if (i == 0)
          this._out.WriteByte((byte) 0);
        else if (i < 256)
        {
          this._out.WriteByte((byte) 248);
          this.WriteInt8(i);
        }
        else
        {
          this._out.WriteByte((byte) 249);
          this.WriteInt16(i);
        }
      }
    }

    public class BinTreeNodeReader
    {
      protected internal TokenDictionary tokenMap;
      protected internal FunXMPP.Connection conn;

      public BinTreeNodeReader(
        FunXMPP.StanzaProvider provider,
        FunXMPP.Connection conn,
        TokenDictionary tokenMap)
      {
        this.conn = conn;
        this.tokenMap = tokenMap;
        if (provider == null)
          return;
        provider.StanzaAvailable += new FunXMPP.ProcessStanzaHandler(this.OnStanzaAvailable);
        provider.DecodeStanza += new FunXMPP.DecodeStanza(this.ParseTreeNode);
      }

      public void OnStanzaAvailable(Stream stanzaStream)
      {
        this.conn.ProcessNode(this.ParseTreeNode(stanzaStream));
      }

      public FunXMPP.ProtocolTreeNode ParseTreeNode(Stream stream)
      {
        int num1 = this.ReadListSize(stream.ReadByte(), stream);
        int token = stream.ReadByte();
        if (token == 2)
          return (FunXMPP.ProtocolTreeNode) null;
        string tag = this.ReadStringAsString(token, stream);
        FunXMPP.KeyValue[] attrs = num1 != 0 && tag != null ? this.ReadAttributes((num1 - 2 + num1 % 2) / 2, stream) : throw new FunXMPP.CorruptStreamException("nextTree sees 0 list or null tag");
        if (num1 % 2 == 1)
          return new FunXMPP.ProtocolTreeNode(tag, attrs);
        int num2 = stream.ReadByte();
        if (this.IsListTag(num2))
          return new FunXMPP.ProtocolTreeNode(tag, attrs, this.ReadList(num2, stream));
        object obj = this.ReadString(num2, stream);
        byte[] data = !(obj is string s) ? obj as byte[] : Encoding.UTF8.GetBytes(s);
        return new FunXMPP.ProtocolTreeNode(tag, attrs, data);
      }

      private bool IsListTag(int b) => b == 248 || b == 0 || b == 249;

      private FunXMPP.KeyValue[] ReadAttributes(int attribCount, Stream stanza)
      {
        FunXMPP.KeyValue[] keyValueArray = new FunXMPP.KeyValue[attribCount];
        for (int index = 0; index < attribCount; ++index)
        {
          string k = this.ReadStringAsString(stanza);
          string v = this.ReadStringAsString(stanza);
          keyValueArray[index] = new FunXMPP.KeyValue(k, v);
        }
        return keyValueArray;
      }

      private FunXMPP.ProtocolTreeNode[] ReadList(int token, Stream stream)
      {
        int length = this.ReadListSize(token, stream);
        FunXMPP.ProtocolTreeNode[] protocolTreeNodeArray = new FunXMPP.ProtocolTreeNode[length];
        for (int index = 0; index < length; ++index)
          protocolTreeNodeArray[index] = this.ParseTreeNode(stream);
        return protocolTreeNodeArray;
      }

      private int ReadListSize(int token, Stream s)
      {
        if (token == 0)
          return 0;
        if (token == 248)
          return FunXMPP.BinTreeNodeReader.ReadInt8(s);
        if (token == 249)
          return FunXMPP.BinTreeNodeReader.ReadInt16(s);
        throw new FunXMPP.CorruptStreamException("invalid list size in readListSize: token " + (object) token);
      }

      private FunXMPP.ProtocolTreeNode[] ReadList(Stream stream)
      {
        return this.ReadList(stream.ReadByte(), stream);
      }

      private object ReadString(Stream stanza) => this.ReadString(stanza.ReadByte(), stanza);

      private object ReadString(int token, Stream stanza)
      {
        if (token == -1)
          throw new FunXMPP.CorruptStreamException("-1 token in readString");
        if (token > 2 && token < this.tokenMap.PrimaryTokenMax)
        {
          string str = (string) null;
          int subdict = -1;
          this.tokenMap.GetToken(token, ref subdict, ref str);
          if (str == null)
          {
            token = stanza.ReadByte();
            this.tokenMap.GetToken(token, ref subdict, ref str);
          }
          return (object) str;
        }
        switch (token)
        {
          case 0:
            return (object) null;
          case 250:
            string str1 = this.ReadStringAsString(stanza);
            string str2 = this.ReadStringAsString(stanza);
            if (str1 != null && str2 != null)
              return (object) (str1 + "@" + str2);
            return str2 != null ? (object) str2 : throw new FunXMPP.CorruptStreamException("readString couldn't reconstruct jid");
          case 251:
            return (object) FunXMPP.BinTreeNodeReader.ReadHex8(stanza);
          case 252:
            byte[] buf1 = new byte[FunXMPP.BinTreeNodeReader.ReadInt8(stanza)];
            FunXMPP.BinTreeNodeReader.FillArray(buf1, stanza);
            return (object) buf1;
          case 253:
            byte[] buf2 = new byte[FunXMPP.BinTreeNodeReader.ReadInt20(stanza)];
            FunXMPP.BinTreeNodeReader.FillArray(buf2, stanza);
            return (object) buf2;
          case 254:
            byte[] buf3 = new byte[FunXMPP.BinTreeNodeReader.ReadInt31(stanza)];
            FunXMPP.BinTreeNodeReader.FillArray(buf3, stanza);
            return (object) buf3;
          case (int) byte.MaxValue:
            return (object) FunXMPP.BinTreeNodeReader.ReadNibble8(stanza);
          default:
            throw new FunXMPP.CorruptStreamException("readString couldn't match token " + (object) token);
        }
      }

      private string ObjectAsString(object obj)
      {
        switch (obj)
        {
          case string str:
            return str;
          case byte[] bytes:
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
          default:
            return (string) null;
        }
      }

      private string ReadStringAsString(Stream stanza)
      {
        return this.ObjectAsString(this.ReadString(stanza));
      }

      private string ReadStringAsString(int token, Stream stanza)
      {
        return this.ObjectAsString(this.ReadString(token, stanza));
      }

      private static void FillArray(byte[] buf, Stream i)
      {
        FunXMPP.BinTreeNodeReader.FillArray(buf, buf.Length, i);
      }

      private static void FillArray(byte[] buf, int len, Stream i)
      {
        int offset = 0;
        while (offset < len)
          offset += i.Read(buf, offset, len - offset);
      }

      private static int ReadInt8(Stream i) => i.ReadByte();

      private static int ReadInt16(Stream i) => i.ReadByte() << 8 | i.ReadByte();

      private static int ReadInt20(Stream i)
      {
        return i.ReadByte() << 16 | i.ReadByte() << 8 | i.ReadByte();
      }

      private static int ReadInt31(Stream i)
      {
        return (i.ReadByte() & (int) sbyte.MaxValue) << 24 | i.ReadByte() << 16 | i.ReadByte() << 8 | i.ReadByte();
      }

      private static byte[] ReadNibble8(Stream i)
      {
        int num1 = FunXMPP.BinTreeNodeReader.ReadInt8(i);
        bool flag = (num1 & 128) != 0;
        int length1 = num1 & (int) sbyte.MaxValue;
        byte[] buf = new byte[length1];
        FunXMPP.BinTreeNodeReader.FillArray(buf, i);
        int length2 = length1 * 2 - (flag ? 1 : 0);
        byte[] numArray = new byte[length2];
        for (int index = 0; index < length2; ++index)
        {
          int num2 = 4 * (1 - index % 2);
          int num3 = ((int) buf[index / 2] & 15 << num2) >> num2;
          switch (num3)
          {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
              numArray[index] = (byte) (num3 + 48);
              break;
            case 10:
            case 11:
              numArray[index] = (byte) (num3 - 10 + 45);
              break;
            default:
              throw new FunXMPP.CorruptStreamException("bad nibble " + (object) num3);
          }
        }
        return numArray;
      }

      private static byte[] ReadHex8(Stream i)
      {
        int num1 = FunXMPP.BinTreeNodeReader.ReadInt8(i);
        bool flag = (num1 & 128) != 0;
        int length1 = num1 & (int) sbyte.MaxValue;
        byte[] buf = new byte[length1];
        FunXMPP.BinTreeNodeReader.FillArray(buf, i);
        int length2 = length1 * 2 - (flag ? 1 : 0);
        byte[] numArray = new byte[length2];
        for (int index = 0; index < length2; ++index)
        {
          int num2 = 4 * (1 - index % 2);
          int num3 = ((int) buf[index / 2] & 15 << num2) >> num2;
          switch (num3)
          {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
              numArray[index] = (byte) (num3 + 48);
              break;
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
              numArray[index] = (byte) (num3 - 10 + 65);
              break;
            default:
              throw new FunXMPP.CorruptStreamException("bad nibble " + (object) num3);
          }
        }
        return numArray;
      }
    }

    public interface Listener
    {
      void OnConnected();

      void OnConnectionLost();

      void OnOfflineMessagesCompleted(bool disconnect = false);

      void OnStatusUpdate(string jid, DateTime? timestamp, string status, bool interactive);

      void OnContactChangeNumber(string oldJid, string newJid, DateTime? dtUtc);

      void OnSyncNotification(
        string jid,
        byte[] jidHash,
        FunXMPP.Connection.ContactNotificationType type,
        Action<bool> ack);

      void OnSidelistNotification(
        string jidHashB64,
        UsyncQuery.UsyncProtocol protocol,
        Action<bool> ack);

      void OnMessageForMe(FunXMPP.FMessage message);

      void OnLocationForMe(string sender, int elapsed, byte[] payload);

      void OnLocationNotificationForMe(
        string group,
        string sender,
        int? expiration,
        int elapsed,
        byte[] payload);

      void OnLocationKeyForMe(
        string group,
        string participant,
        int version,
        string type,
        int count,
        byte[] cipherText,
        Action ack);

      void OnLocationKeyDenyForMe(string participant);

      void OnMessageReceipt(
        FunXMPP.FMessage.Key message,
        string participant,
        FunXMPP.FMessage.Status status,
        out bool ignored,
        int? expectedDeliveryCount = null,
        DateTime? dt = null);

      void OnMessageError(FunXMPP.FMessage.Key msg, int code);

      void OnPing(string id);

      void OnPingResponseReceived();

      void OnAvailable(string jid, bool what, DateTime? timestamp = null);

      void OnClientConfigReceived(string push_id);

      void ClearLastSeenCache();

      void OnComposing(
        string jid,
        string participant,
        bool isComposing,
        FunXMPP.FMessage.Type mediaType = FunXMPP.FMessage.Type.Undefined);

      void OnPrivacyBlockList(IEnumerable<string> jids);

      void OnPrivacySettings(System.Collections.Generic.Dictionary<string, PrivacyVisibility> settings);

      void OnSelfSetNewPhoto(
        string jid,
        string photoId,
        bool createSysMessage,
        byte[] smallPicBytes,
        byte[] largePicBytes,
        string context = null);

      void OnNewPhotoIdFetched(
        string jid,
        string authorJid,
        string photoId,
        bool createSystemMessage,
        string context = null);

      void OnPhotoChanged(
        string jid,
        string photoId,
        byte[] smallPicBytes,
        byte[] largePicBytes,
        string context = null);

      void OnPhotoReuploadRequested(string jid);

      void OnDirty(string type, long timestamp);

      void OnSonar(string url);

      void OnEncryptionPreKeyCount(int count, Action ack);

      void OnMessageRetryFromTarget(
        FunXMPP.FMessage.Key key,
        string participant,
        int version,
        int count,
        uint registration);

      FunXMPP.QrListener Qr { get; }

      void OnRemoteClientCaps(IEnumerable<RemoteClientCaps> data, Action onComplete = null);

      void OnChangeChatStaticKey(Action ack);

      void StoreIncomingMessage(FunXMPP.FMessage message);

      bool OnGdprReportReady(
        long? creationTime,
        long? expirationTime,
        byte[] protobuf,
        bool showToast,
        Action ack);
    }

    public interface GroupListener
    {
      void OnGroupAddUser(string gjid, string jid, string author, DateTime? dt, string reason);

      void OnGroupRemoveUser(
        string gjid,
        string jid,
        string author,
        DateTime? dt = null,
        string subject = null,
        bool sysMsg = true);

      void OnGroupParticipantChangeNumber(string gjid, string oldJid, string newJid, DateTime? dt = null);

      void OnGroupNewSubject(
        string gjid,
        string ujid,
        string pushName,
        string subject,
        DateTime? timestamp);

      void OnGroupNewDescription(
        string gjid,
        string ujid,
        string id,
        string description,
        DateTime? timestamp);

      void OnServerProperties(System.Collections.Generic.Dictionary<string, string> nameValueMap);

      void OnGroupWelcome(FunXMPP.Connection.GroupCreationEventArgs args, bool isInvite);

      void OnGroupInfo(FunXMPP.Connection.GroupInfo info, bool checkParticipants = true);

      void OnAddGroupParticipants(
        string gjid,
        List<string> successJids,
        List<Pair<string, int>> failList,
        DateTime? dt,
        string reason);

      void OnRemoveGroupParticipants(
        string gjid,
        List<string> successList,
        List<Pair<string, int>> failList,
        DateTime? dt = null);

      void OnParticipatingGroups(FunXMPP.Connection.GroupInfo[] groups);

      void OnLeaveGroup(string from, DateTime? dt = null, bool delete = false);

      void OnLeaveGroupFail(string groupJid);

      void OnGroupDescriptionMismatch(string groupJid);

      void OnPromoteUsers(string gjid, IEnumerable<string> jids, DateTime? dt);

      void OnDemoteUsers(string gjid, IEnumerable<string> jids, DateTime? dt);

      void OnGroupLocked(string gjid, string ujid, DateTime? dt);

      void OnGroupUnlocked(string gjid, string ujid, DateTime? dt);

      void OnGroupAnnounceOnly(string gjid, string ujid, DateTime? dt);

      void OnGroupNotAnnounceOnly(string gjid, string ujid, DateTime? dt);

      void OnGroupDisbanded(string gjid, string author, DateTime? dt);

      void OnInvitationCode(string gjid, string author, string inviteCode, DateTime? dt);

      void OnBroadcastListInfo(string blJid, string blName, IEnumerable<string> recipientJids);
    }

    public interface VoipListener
    {
      void HandleVoipOfferReceipt(FunXMPP.ProtocolTreeNode node);

      void HandleVoipNode(FunXMPP.ProtocolTreeNode node);

      void HandleVoipAck(FunXMPP.ProtocolTreeNode node);

      void HandleEncRekeyRetry(FunXMPP.ProtocolTreeNode node);
    }

    public sealed class IqResultHandler
    {
      private Action<FunXMPP.ProtocolTreeNode, string> parse;
      private Action<FunXMPP.ProtocolTreeNode> parseErrorNode;

      public Action<FunXMPP.ProtocolTreeNode, string> Parse
      {
        get => this.parse ?? (Action<FunXMPP.ProtocolTreeNode, string>) ((node, tag) => { });
        private set => this.parse = value;
      }

      public Action<FunXMPP.ProtocolTreeNode> ErrorNode
      {
        get => this.parseErrorNode ?? (Action<FunXMPP.ProtocolTreeNode>) (node => { });
        private set => this.parseErrorNode = value;
      }

      public IqResultHandler(Action<FunXMPP.ProtocolTreeNode, string> parse) => this.Parse = parse;

      public IqResultHandler(
        Action<FunXMPP.ProtocolTreeNode, string> parse,
        Action<int> onErrorCode)
        : this(parse)
      {
        if (onErrorCode == null)
          return;
        this.ErrorNode = (Action<FunXMPP.ProtocolTreeNode>) (node =>
        {
          foreach (string s in node.GetAllChildren("error").Where<FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, bool>) (errorNode => errorNode != null)).Select<FunXMPP.ProtocolTreeNode, string>((Func<FunXMPP.ProtocolTreeNode, string>) (errorNode => errorNode.GetAttributeValue("code"))))
            onErrorCode(int.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture));
        });
      }

      public IqResultHandler(
        Action<FunXMPP.ProtocolTreeNode, string> parse,
        Action<FunXMPP.ProtocolTreeNode> parseError)
        : this(parse)
      {
        this.ErrorNode = parseError;
      }
    }

    public sealed class ErrorHandler
    {
      public Action OnCompleted;
      public Action<int> OnError;
    }

    public class LoginEventArgs : EventArgs
    {
      public DateTime? ServerTime;
    }

    public class LoginFailureException : Exception
    {
      public WAProtocol.LoginFailedReason Type;

      public long? BanTotalSeconds { get; set; }

      public DateTime? BanExpirationUtc { get; set; }

      public DateTime? RetryUtc { get; set; }

      public string BanReason { get; set; }

      public string FailedLoginReason { get; set; }

      public LoginFailureException(WAProtocol.LoginFailedReason type)
      {
        Log.l(nameof (LoginFailureException), "Type {0}", (object) type);
        this.Type = type;
      }
    }

    public class EOFException : Exception
    {
    }

    public class CorruptStreamException : Exception
    {
      public CorruptStreamException()
      {
      }

      public CorruptStreamException(string s)
        : base(s)
      {
      }
    }

    public class FunRuntimeException : SystemException
    {
      protected internal string bufString;
      protected internal Exception t;

      public FunRuntimeException(Exception t, string buf)
      {
        this.bufString = buf;
        this.t = t;
      }

      public string GetMessage()
      {
        return "Wrapping: " + this.t.GetType().Name + "\nFunRuntimeException last stanza: " + this.bufString;
      }

      public Exception GetInner() => this.t;
    }

    public class FMessage
    {
      public bool multicast;
      public bool urlText;
      public bool urlPhoneNumber;
      public FunXMPP.FMessage.Status status = FunXMPP.FMessage.Status.Undefined;
      public FunXMPP.FMessage.Key key;
      public string remote_resource;
      public bool wants_receipt;
      public DateTime? timestamp;
      public string media_url;
      public string media_mime_type;
      public string media_ip;
      public FunXMPP.FMessage.Type media_wa_type;
      public long media_size;
      public int media_duration_seconds;
      public string media_name;
      public string media_origin;
      public string media_caption;
      public byte[] media_hash;
      public byte[] media_key;
      public double latitude;
      public double longitude;
      public string details;
      public string location_url;
      public object thumb_image;
      public bool gap_behind = true;
      public int offline;
      public string push_name;
      public byte[] proto_buf;
      public bool mms_retry;
      public int qcount;
      public FunXMPP.FMessage.Encrypted[] encrypted;
      public string participants_hash;
      public FunXMPP.FMessage.Participant[] participants;
      public uint? registrationId;
      public bool web_relay;
      public string notify_mute;
      public ulong? verified_name;
      public string verified_level;
      public byte[] verified_name_certificate;
      public int edit_version;
      public MessageProperties message_properties;
      public bool is_highly_structured_message_rehydrate;

      public string data { get; set; }

      public byte[] binary_data { get; set; }

      internal FMessage(string remote_jid, bool from_me)
      {
        this.key = new FunXMPP.FMessage.Key(remote_jid, from_me, FunXMPP.GenerateMessageId());
      }

      public FMessage(FunXMPP.FMessage.Key key) => this.key = key;

      public FMessage(string remote_jid, string data, object image)
        : this(remote_jid, true)
      {
        this.data = data;
        this.thumb_image = image;
        this.timestamp = new DateTime?(DateTime.Now);
      }

      public static FunXMPP.FMessage.Type GetMediaWaType(string type)
      {
        if (string.IsNullOrEmpty(type))
          return FunXMPP.FMessage.Type.Undefined;
        if (type.ToUpper().Equals("system".ToUpper()))
          return FunXMPP.FMessage.Type.System;
        if (type.ToUpper().Equals("image".ToUpper()))
          return FunXMPP.FMessage.Type.Image;
        if (type.ToUpper().Equals("sticker".ToUpper()))
          return FunXMPP.FMessage.Type.Sticker;
        if (type.ToUpper().Equals("audio".ToUpper()))
          return FunXMPP.FMessage.Type.Audio;
        if (type.ToUpper().Equals("video".ToUpper()))
          return FunXMPP.FMessage.Type.Video;
        if (type.ToUpper().Equals("vcard".ToUpper()))
          return FunXMPP.FMessage.Type.Contact;
        if (type.ToUpper().Equals("location".ToUpper()))
          return FunXMPP.FMessage.Type.Location;
        if (type.ToUpper().Equals("livelocation".ToUpper()))
          return FunXMPP.FMessage.Type.LiveLocation;
        if (type.ToUpper().Equals("document".ToUpper()))
          return FunXMPP.FMessage.Type.Document;
        return type.ToUpper().Equals("url".ToUpper()) ? FunXMPP.FMessage.Type.ExtendedText : FunXMPP.FMessage.Type.Undefined;
      }

      public static string GetMediaWaTypeStr(FunXMPP.FMessage.Type type)
      {
        switch (type)
        {
          case FunXMPP.FMessage.Type.Undefined:
            return (string) null;
          case FunXMPP.FMessage.Type.Image:
            return "image";
          case FunXMPP.FMessage.Type.Audio:
            return "audio";
          case FunXMPP.FMessage.Type.Video:
          case FunXMPP.FMessage.Type.Gif:
            return "video";
          case FunXMPP.FMessage.Type.Contact:
            return "vcard";
          case FunXMPP.FMessage.Type.Location:
            return "location";
          case FunXMPP.FMessage.Type.System:
            return "system";
          case FunXMPP.FMessage.Type.Document:
            return "document";
          case FunXMPP.FMessage.Type.ExtendedText:
            return "url";
          case FunXMPP.FMessage.Type.LiveLocation:
            return "livelocation";
          case FunXMPP.FMessage.Type.Sticker:
            return "sticker";
          default:
            Log.SendCrashLog(new Exception("Unrecognized type " + type.ToString()), "GetMessage_WA_Type_StrValue", logOnlyForRelease: true);
            return (string) null;
        }
      }

      public static string GetFunMediaTypeStr(FunXMPP.FMessage.FunMediaType type)
      {
        switch (type)
        {
          case FunXMPP.FMessage.FunMediaType.Gif:
            return "gif";
          case FunXMPP.FMessage.FunMediaType.Ptt:
            return "ptt";
          case FunXMPP.FMessage.FunMediaType.ContactArray:
            return "contact_array";
          default:
            return FunXMPP.FMessage.GetMediaWaTypeStr((FunXMPP.FMessage.Type) type);
        }
      }

      public static FunXMPP.FMessage.Type TypeFromFunMediaType(FunXMPP.FMessage.FunMediaType type)
      {
        return type == FunXMPP.FMessage.FunMediaType.Ptt ? FunXMPP.FMessage.Type.Audio : (FunXMPP.FMessage.Type) type;
      }

      public string Audience => (string) null;

      public enum Type
      {
        Undefined = 0,
        Image = 1,
        Audio = 2,
        Video = 3,
        Contact = 4,
        Location = 5,
        Divider = 6,
        System = 7,
        Document = 8,
        ExtendedText = 9,
        Gif = 10, // 0x0000000A
        LiveLocation = 11, // 0x0000000B
        Sticker = 19, // 0x00000013
        CipherText = 1000, // 0x000003E8
        ProtocolBuffer = 1001, // 0x000003E9
        Unsupported = 1002, // 0x000003EA
        CallOffer = 1003, // 0x000003EB
        HSM = 1004, // 0x000003EC
        ProtocolMessage = 1005, // 0x000003ED
        Revoked = 1006, // 0x000003EE
        Empty = 1007, // 0x000003EF
      }

      public enum FunMediaType
      {
        Undefined = 0,
        Image = 1,
        Audio = 2,
        Video = 3,
        Contact = 4,
        Location = 5,
        System = 7,
        Document = 8,
        ExtendedText = 9,
        Gif = 10, // 0x0000000A
        LiveLocation = 11, // 0x0000000B
        Payment = 16, // 0x00000010
        Sticker = 19, // 0x00000013
        Ptt = 2000, // 0x000007D0
        ContactArray = 2001, // 0x000007D1
      }

      public enum Status
      {
        UnsentOld = 0,
        Uploading = 1,
        Uploaded = 2,
        SentByClient = 3,
        ReceivedByServer = 4,
        ReceivedByTarget = 5,
        NeverSend = 6,
        ServerBounce = 7,
        Undefined = 8,
        Unsent = 9,
        Error = 10, // 0x0000000A
        PlayedByTarget = 11, // 0x0000000B
        ObsoletePlayedByTargetAcked = 12, // 0x0000000C
        Canceled = 14, // 0x0000000E
        Relay = 15, // 0x0000000F
        UploadingCustomHash = 16, // 0x00000010
        Downloading = 17, // 0x00000011
        ReadByTarget = 18, // 0x00000012
        ObsoleteReadByTargetAcked = 19, // 0x00000013
        Pending = 20, // 0x00000014
      }

      public class Builder
      {
        internal FunXMPP.FMessage message;
        internal string remote_jid;
        internal string remote_resource;
        internal bool? from_me;
        internal string id;
        internal bool? wants_receipt;
        internal string data;
        internal byte[] binary_data;
        internal string thumb_image;
        internal DateTime? timestamp;
        internal int? offline;
        internal FunXMPP.FMessage.Type? media_wa_type;
        internal long? media_size;
        internal int? media_duration_seconds;
        internal string media_origin;
        internal string media_caption;
        internal string media_url;
        internal string media_name;
        internal string media_ip;
        internal byte[] media_hash;
        internal string media_mime_type;
        internal byte[] media_key;
        internal double? latitude;
        internal double? longitude;
        internal string details;
        internal string location_url;
        internal string push_name;
        internal string participants_hash;
        internal List<FunXMPP.FMessage.Encrypted> encrypted;
        internal uint? registrationId;
        internal bool? multicast;
        internal ulong? verified_name;
        internal string verified_level;
        internal byte[] verified_name_certificate;
        internal int? edit_version;
        internal MessageProperties message_props;
        internal bool? is_highly_structured_message_rehydrate;
        internal bool mms_retry;

        public FunXMPP.FMessage.Builder Key(FunXMPP.FMessage.Key key)
        {
          this.remote_jid = key.remote_jid;
          this.from_me = new bool?(key.from_me);
          this.id = key.id;
          return this;
        }

        public FunXMPP.FMessage.Key Key()
        {
          return new FunXMPP.FMessage.Key(this.remote_jid, this.from_me.Value, this.id);
        }

        public FunXMPP.FMessage.Builder Remote_jid(string remote_jid)
        {
          this.remote_jid = remote_jid;
          return this;
        }

        public string Remote_jid() => this.remote_jid;

        public FunXMPP.FMessage.Builder Remote_resource(string remote_resource)
        {
          this.remote_resource = remote_resource;
          return this;
        }

        public string Remote_resource() => this.remote_resource;

        public FunXMPP.FMessage.Builder From_me(bool from_me)
        {
          this.from_me = new bool?(from_me);
          return this;
        }

        public bool? From_me() => this.from_me;

        public FunXMPP.FMessage.Builder Id(string id)
        {
          this.id = id;
          return this;
        }

        public string Id() => this.id;

        public FunXMPP.FMessage.Builder Wants_receipt(bool wants_receipt)
        {
          this.wants_receipt = new bool?(wants_receipt);
          return this;
        }

        public bool? Wants_receipt() => this.wants_receipt;

        public FunXMPP.FMessage.Builder Data(string data)
        {
          this.data = data;
          return this;
        }

        public string Data() => this.data;

        public FunXMPP.FMessage.Builder BinaryData(byte[] data)
        {
          this.binary_data = data;
          return this;
        }

        public byte[] BinaryData() => this.binary_data;

        public FunXMPP.FMessage.Builder Thumb_image(string thumb_image)
        {
          this.thumb_image = thumb_image;
          return this;
        }

        public string Thumb_image() => this.thumb_image;

        public FunXMPP.FMessage.Builder Offline(int offline)
        {
          this.offline = new int?(offline);
          return this;
        }

        public int? Offline() => this.offline;

        public FunXMPP.FMessage.Builder Timestamp(DateTime? timestamp)
        {
          this.timestamp = timestamp;
          return this;
        }

        public DateTime? Timestamp() => this.timestamp;

        public FunXMPP.FMessage.Builder Media_wa_type(FunXMPP.FMessage.Type media_wa_type)
        {
          this.media_wa_type = new FunXMPP.FMessage.Type?(media_wa_type);
          return this;
        }

        public FunXMPP.FMessage.Type? Media_wa_type() => this.media_wa_type;

        public FunXMPP.FMessage.Builder Media_size(long media_size)
        {
          this.media_size = new long?(media_size);
          return this;
        }

        public long? Media_size() => this.media_size;

        public FunXMPP.FMessage.Builder Media_duration_seconds(int media_duration_seconds)
        {
          this.media_duration_seconds = new int?(media_duration_seconds);
          return this;
        }

        public int? Media_duration_seconds() => this.media_duration_seconds;

        public FunXMPP.FMessage.Builder Media_url(string media_url)
        {
          this.media_url = media_url;
          return this;
        }

        public string Media_url() => this.media_url;

        public FunXMPP.FMessage.Builder Media_name(string media_name)
        {
          this.media_name = media_name;
          return this;
        }

        public string Media_name() => this.media_name;

        public FunXMPP.FMessage.Builder Media_ip(string ip)
        {
          this.media_ip = ip;
          return this;
        }

        public string Media_ip() => this.media_ip;

        public FunXMPP.FMessage.Builder Latitude(double latitude)
        {
          this.latitude = new double?(latitude);
          return this;
        }

        public double? Latitude() => this.latitude;

        public FunXMPP.FMessage.Builder Longitude(double longitude)
        {
          this.longitude = new double?(longitude);
          return this;
        }

        public double? Longitude() => this.longitude;

        public FunXMPP.FMessage.Builder Details(string details)
        {
          this.details = details;
          return this;
        }

        public string Details() => this.details;

        public FunXMPP.FMessage.Builder Location_url(string url)
        {
          this.location_url = url;
          return this;
        }

        public string Location_url() => this.location_url;

        public FunXMPP.FMessage.Builder Push_name(string name)
        {
          this.push_name = name;
          return this;
        }

        public string Push_name() => this.push_name;

        public FunXMPP.FMessage.Builder Media_origin(string value)
        {
          this.media_origin = value;
          return this;
        }

        public string Media_origin() => this.media_origin;

        public FunXMPP.FMessage.Builder Media_caption(string value)
        {
          this.media_caption = value;
          return this;
        }

        public string Media_caption() => this.media_caption;

        public FunXMPP.FMessage.Builder Media_hash(byte[] value)
        {
          this.media_hash = value;
          return this;
        }

        public byte[] Media_hash() => this.media_hash;

        public FunXMPP.FMessage.Builder Media_mime_type(string value)
        {
          this.media_mime_type = value;
          return this;
        }

        public string Media_mime_type() => this.media_mime_type;

        public byte[] Media_key() => this.media_key;

        public FunXMPP.FMessage.Builder Media_key(byte[] value)
        {
          this.media_key = value;
          return this;
        }

        public bool Mmms_retry() => this.mms_retry;

        public FunXMPP.FMessage.Builder Mms_retry(bool value)
        {
          this.mms_retry = value;
          return this;
        }

        public FunXMPP.FMessage.Builder Encrypted(FunXMPP.FMessage.Encrypted enc)
        {
          if (this.encrypted == null)
            this.encrypted = new List<FunXMPP.FMessage.Encrypted>();
          this.encrypted.Add(enc);
          return this;
        }

        public FunXMPP.FMessage.Encrypted[] Encrypted()
        {
          if (this.encrypted != null)
            this.encrypted.ToArray();
          return (FunXMPP.FMessage.Encrypted[]) null;
        }

        public string Participants_hash() => this.participants_hash;

        public FunXMPP.FMessage.Builder Participants_hash(string value)
        {
          this.participants_hash = value;
          return this;
        }

        public uint? RegistrationId() => this.registrationId;

        public FunXMPP.FMessage.Builder RegistrationId(uint? value)
        {
          this.registrationId = value;
          return this;
        }

        public ulong? VerifiedName() => this.verified_name;

        public FunXMPP.FMessage.Builder VerifiedName(ulong value)
        {
          this.verified_name = new ulong?(value);
          return this;
        }

        public string VerifiedLevel() => this.verified_level;

        public FunXMPP.FMessage.Builder VerifiedLevel(string value)
        {
          this.verified_level = value;
          return this;
        }

        public FunXMPP.FMessage.Builder Edit_version(int edit)
        {
          this.edit_version = new int?(edit);
          return this;
        }

        public int? Edit_version() => this.edit_version;

        public FunXMPP.FMessage.Builder MessageProperties(MessageProperties msgProps)
        {
          this.message_props = msgProps;
          return this;
        }

        public MessageProperties MessageProps() => this.message_props;

        public FunXMPP.FMessage.Builder IsHighlyStructuredMessageRehydrate(bool val)
        {
          this.is_highly_structured_message_rehydrate = new bool?(val);
          return this;
        }

        public bool IsHighlyStructuredMessageRehydrate()
        {
          return this.is_highly_structured_message_rehydrate.GetValueOrDefault();
        }

        public FunXMPP.FMessage.Builder SetInstance(FunXMPP.FMessage message)
        {
          this.message = message;
          return this;
        }

        public FunXMPP.FMessage.Builder NewIncomingInstance()
        {
          if (this.remote_jid == null || !this.from_me.HasValue || this.id == null)
            throw new NotSupportedException("missing required property before instantiating new incoming message");
          this.message = new FunXMPP.FMessage(new FunXMPP.FMessage.Key(this.remote_jid, this.from_me.Value, this.id));
          return this;
        }

        public FunXMPP.FMessage.Builder NewOutgoingInstance()
        {
          if (this.remote_jid == null || this.data == null || this.thumb_image == null)
            throw new NotSupportedException("missing required property before instantiating new outgoing message");
          if (this.id != null || this.from_me.Value && !this.from_me.Value)
            throw new NotSupportedException("invalid property set before instantiating new outgoing message");
          this.message = new FunXMPP.FMessage(this.remote_jid, this.data, (object) this.thumb_image);
          return this;
        }

        public bool Instantiated() => this.message != null;

        public FunXMPP.FMessage Build()
        {
          if (this.message == null)
            return (FunXMPP.FMessage) null;
          if (this.remote_jid != null && this.from_me.HasValue && this.id != null)
            this.message.key = new FunXMPP.FMessage.Key(this.remote_jid, this.from_me.Value, this.id);
          if (this.remote_resource != null)
            this.message.remote_resource = this.remote_resource;
          if (this.wants_receipt.HasValue)
            this.message.wants_receipt = this.wants_receipt.Value;
          if (this.data != null)
            this.message.data = this.data;
          if (this.thumb_image != null)
            this.message.thumb_image = (object) this.thumb_image;
          if (this.timestamp.HasValue)
            this.message.timestamp = new DateTime?(this.timestamp.Value);
          if (this.offline.HasValue)
            this.message.offline = this.offline.Value;
          if (this.media_wa_type.HasValue)
            this.message.media_wa_type = this.media_wa_type.Value;
          if (this.media_size.HasValue)
            this.message.media_size = this.media_size.Value;
          if (this.media_duration_seconds.HasValue)
            this.message.media_duration_seconds = this.media_duration_seconds.Value;
          if (this.media_url != null)
            this.message.media_url = this.media_url;
          if (this.media_name != null)
            this.message.media_name = this.media_name;
          if (this.latitude.HasValue)
            this.message.latitude = this.latitude.Value;
          if (this.longitude.HasValue)
            this.message.longitude = this.longitude.Value;
          if (this.location_url != null)
            this.message.location_url = this.location_url;
          if (this.details != null)
            this.message.details = this.details;
          if (this.binary_data != null)
            this.message.binary_data = this.binary_data;
          if (this.push_name != null)
            this.message.push_name = this.push_name;
          if (this.media_origin != null)
            this.message.media_origin = this.media_origin;
          if (this.media_caption != null)
            this.message.media_caption = this.media_caption;
          if (this.media_hash != null)
            this.message.media_hash = this.media_hash;
          if (this.media_mime_type != null)
            this.message.media_mime_type = this.media_mime_type;
          if (this.media_ip != null)
            this.message.media_ip = this.media_ip;
          if (this.media_key != null)
            this.message.media_key = this.media_key;
          if (this.mms_retry)
            this.message.mms_retry = this.mms_retry;
          if (this.encrypted != null)
            this.message.encrypted = this.encrypted.ToArray();
          if (this.participants_hash != null)
            this.message.participants_hash = this.participants_hash;
          if (this.registrationId.HasValue)
            this.message.registrationId = this.registrationId;
          if (this.multicast.HasValue)
            this.message.multicast = this.multicast.Value;
          if (this.verified_name.HasValue)
            this.message.verified_name = this.verified_name;
          if (this.verified_level != null)
            this.message.verified_level = this.verified_level;
          if (this.verified_name_certificate != null)
            this.message.verified_name_certificate = this.verified_name_certificate;
          if (this.edit_version.HasValue)
            this.message.edit_version = this.edit_version.Value;
          if (this.message_props != null)
            this.message.message_properties = this.message_props;
          if (this.is_highly_structured_message_rehydrate.HasValue)
            this.message.is_highly_structured_message_rehydrate = this.is_highly_structured_message_rehydrate.Value;
          return this.message;
        }
      }

      public class Key
      {
        public string remote_jid;
        public bool from_me;
        public string id;

        public Key(string remote_jid, bool from_me, string id)
        {
          this.remote_jid = remote_jid;
          this.from_me = from_me;
          this.id = id;
        }

        public override int GetHashCode()
        {
          int num = 31;
          return num * (num * (num * 1 + (this.from_me ? 1231 : 1237)) + (this.id == null ? 0 : this.id.GetHashCode())) + (this.remote_jid == null ? 0 : this.remote_jid.GetHashCode());
        }

        public override bool Equals(object obj)
        {
          if (this == obj)
            return true;
          if (obj == null || this.GetType() != obj.GetType())
            return false;
          FunXMPP.FMessage.Key key = (FunXMPP.FMessage.Key) obj;
          if (this.from_me != key.from_me)
            return false;
          if (this.id == null)
          {
            if (key.id != null)
              return false;
          }
          else if (!this.id.Equals(key.id))
            return false;
          if (this.remote_jid == null)
          {
            if (key.remote_jid != null)
              return false;
          }
          else if (!this.remote_jid.Equals(key.remote_jid))
            return false;
          return true;
        }

        public override string ToString()
        {
          return "Key[id=" + this.id + ", from_me=" + this.from_me.ToString() + ", remote_jid=" + this.remote_jid + "]";
        }
      }

      public class Encrypted
      {
        public string cipher_text_type;
        public byte[] cipher_text_bytes;
        public int cipher_retry_count;
        public int cipher_version;
        public FunXMPP.FMessage.FunMediaType fun_media_type;
      }

      public class Participant
      {
        public string Jid;
        public FunXMPP.FMessage.Encrypted Encrypted;

        public Participant(string jid) => this.Jid = jid;
      }
    }

    public class Connection
    {
      private object iqHandlerLock = new object();
      protected internal System.Collections.Generic.Dictionary<string, FunXMPP.IqResultHandler> pendingServerRequests = new System.Collections.Generic.Dictionary<string, FunXMPP.IqResultHandler>();
      public List<Action> actionsPendingConnection = new List<Action>();
      private object pendingActionLock = new object();
      private bool connectionActive;
      private Axolotl axolotlImpl;
      public List<FunXMPP.Connection.Ack> PendingAcks;
      public bool Passive;
      public FunXMPP.Connection.AccountKind accountType;
      protected internal const int ID_HEADER_SIZE = 4;
      private IDisposable timerSubscription;
      private static ClientCapabilityCategory[] capsValues;
      private object pendingListenerActionsLock = new object();
      private System.Collections.Generic.Dictionary<FunXMPP.FMessage.Key, List<Action>> pendingListenerActions = new System.Collections.Generic.Dictionary<FunXMPP.FMessage.Key, List<Action>>();
      private System.Collections.Generic.Dictionary<string, FunXMPP.Connection.PendingReceiptState> pendingReadReceipts = new System.Collections.Generic.Dictionary<string, FunXMPP.Connection.PendingReceiptState>();
      private object readReceiptLock = new object();
      protected internal TicketCounter iqid = new TicketCounter();
      public static bool ThrottleClbUpload;
      private System.Collections.Generic.Dictionary<string, FunXMPP.ProtocolTreeNode> offlineWebNodes = new System.Collections.Generic.Dictionary<string, FunXMPP.ProtocolTreeNode>();
      private System.Collections.Generic.Dictionary<string, string> offlineWebRefs = new System.Collections.Generic.Dictionary<string, string>();
      private static WorkQueue asyncReceiptThread;
      private object notificationHandlerLock = new object();
      private System.Collections.Generic.Dictionary<string, FunXMPP.Connection.NotificationAckHandler> pendingNotificationServerRequests = new System.Collections.Generic.Dictionary<string, FunXMPP.Connection.NotificationAckHandler>();
      private IDisposable linkMetadataSub;
      private IDisposable thumbSub;

      public FunXMPP.Listener EventHandler { get; set; }

      public FunXMPP.GroupListener GroupEventHandler { get; set; }

      public FunXMPP.VoipListener VoipEventHandler { get; set; }

      protected internal bool IsVerboseId { get; set; }

      private WAProtocol Protocol { get; set; }

      public Subject<WAProtocol> LoginSubject { get; set; }

      public Axolotl Encryption
      {
        get
        {
          return Utils.LazyInit<Axolotl>(ref this.axolotlImpl, (Func<Axolotl>) (() => new Axolotl(this)));
        }
      }

      public Axolotl EncryptionNoInitialize => this.axolotlImpl;

      public bool ShouldConnectWithPassive()
      {
        bool flag = Settings.DanglingAcks.Count > 0 || !Settings.LastFullSyncUtc.HasValue || Settings.PhoneNumberVerificationState != PhoneNumberVerificationState.Verified;
        if (flag)
        {
          Log.l("PassiveMode", "LastFullSync: {0}", (object) Settings.LastFullSyncUtc);
          Log.l("PassiveMode", "DanglingAcks: {0}", (object) Settings.DanglingAcks.Count);
          Log.l("PassiveMode", "Logging in in passive mode.");
        }
        this.Passive = flag;
        return flag;
      }

      public bool IsConnected => this.connectionActive;

      public Connection()
      {
        this.IsVerboseId = false;
        this.Protocol = new WAProtocol();
        this.LoginSubject = new Subject<WAProtocol>();
        this.PendingAcks = new List<FunXMPP.Connection.Ack>();
      }

      public void SetWAProtocol(WAProtocol protocol)
      {
        this.Protocol = protocol;
        if (this.EventHandler != null)
          this.EventHandler.OnConnected();
        this.LoginSubject.OnNext(protocol);
        IEnumerable<Action> pendingConnection;
        lock (this.pendingActionLock)
        {
          this.connectionActive = true;
          pendingConnection = (IEnumerable<Action>) this.actionsPendingConnection;
          this.actionsPendingConnection = new List<Action>();
        }
        foreach (Action action in pendingConnection)
          action();
      }

      public bool InvokeIfConnected(Action a)
      {
        lock (this.pendingActionLock)
        {
          if (!this.connectionActive)
            return false;
          a();
          return true;
        }
      }

      public void InvokeWhenConnected(Action a)
      {
        lock (this.pendingActionLock)
        {
          if (this.connectionActive)
            a();
          else
            this.actionsPendingConnection.Add(a);
        }
      }

      public IObservable<Unit> ConnectedObservable()
      {
        return Observable.Defer<Unit>((Func<IObservable<Unit>>) (() => Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
        {
          bool cancelled = false;
          this.InvokeWhenConnected((Action) (() =>
          {
            if (cancelled)
              return;
            observer.OnNext(new Unit());
            observer.OnCompleted();
          }));
          return (Action) (() =>
          {
            cancelled = true;
            observer.OnCompleted();
          });
        }))));
      }

      public IObservable<bool> ConnectionStateObservable()
      {
        return Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
        {
          bool cancel = false;
          IDisposable cancelSub = (IDisposable) null;
          object @lock = new object();
          Func<Action, Action> func = (Func<Action, Action>) (inner => (Action) (() =>
          {
            lock (@lock)
            {
              if (cancel)
                return;
              inner();
            }
          }));
          Action subscribeConnected = (Action) null;
          Action connected = func((Action) (() => observer.OnNext(true)));
          Action disconnected = func((Action) (() =>
          {
            observer.OnNext(false);
            subscribeConnected();
          }));
          subscribeConnected = func((Action) (() => this.InvokeWhenConnected(connected)));
          func((Action) (() => cancelSub = FunXMPP.ConnectionLostSubject.Subscribe<Unit>((Action<Unit>) (_ => disconnected()))))();
          subscribeConnected();
          return (Action) (() =>
          {
            lock (@lock)
            {
              cancel = true;
              cancelSub.SafeDispose();
              cancelSub = (IDisposable) null;
            }
          });
        })).DistinctUntilChanged<bool>();
      }

      public void OnConnectionLost()
      {
        Log.l("lost connection");
        FunXMPP.IqResultHandler[] array;
        lock (this.pendingActionLock)
        {
          this.connectionActive = false;
          lock (this.iqHandlerLock)
          {
            array = this.pendingServerRequests.Values.ToArray<FunXMPP.IqResultHandler>();
            this.pendingServerRequests.Clear();
          }
          lock (this.readReceiptLock)
            this.pendingReadReceipts.Clear();
        }
        foreach (FunXMPP.IqResultHandler iqResultHandler in array)
          iqResultHandler.ErrorNode(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("type", "error")
          }, new FunXMPP.ProtocolTreeNode[1]
          {
            new FunXMPP.ProtocolTreeNode("error", new FunXMPP.KeyValue[1]
            {
              new FunXMPP.KeyValue("code", "408")
            })
          }));
        if (this.timerSubscription != null)
        {
          this.timerSubscription.Dispose();
          this.timerSubscription = (IDisposable) null;
        }
        if (this.EventHandler != null)
          this.EventHandler.OnConnectionLost();
        FunXMPP.ConnectionLostSubject.OnNext(new Unit());
        this.NoAckAllPendingNotifications();
      }

      public void SetVerboseId(bool value) => this.IsVerboseId = value;

      public void SendNop() => this.Protocol.TreeNodeWriter.Write((FunXMPP.ProtocolTreeNode) null);

      public void SendRawNode(FunXMPP.ProtocolTreeNode node)
      {
        this.Protocol.TreeNodeWriter.Write(node);
      }

      public void SendPong(string id)
      {
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>();
        keyValueList.Add(new FunXMPP.KeyValue("type", "result"));
        keyValueList.Add(new FunXMPP.KeyValue("to", "s.whatsapp.net"));
        if (id != null)
          keyValueList.Add(new FunXMPP.KeyValue(nameof (id), id));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", keyValueList.ToArray()));
      }

      public static FunXMPP.ProtocolTreeNode GetSubjectMessage(
        string to,
        string id,
        FunXMPP.ProtocolTreeNode child)
      {
        return new FunXMPP.ProtocolTreeNode("message", new FunXMPP.KeyValue[3]
        {
          new FunXMPP.KeyValue(nameof (to), to),
          new FunXMPP.KeyValue("type", "subject"),
          new FunXMPP.KeyValue(nameof (id), id)
        }, child);
      }

      public void SendMessage(FunXMPP.FMessage message)
      {
        AppState.ClientInstance?.CheckPushName();
        this.Protocol.TreeNodeWriter.Write(FunXMPP.Connection.GetMessageNode(message));
      }

      public void SendMultiParticipantMessage(
        FunXMPP.FMessage message,
        string participantNodeName,
        FunXMPP.FMessage.Participant[] participants,
        string broadcastListName)
      {
        AppState.ClientInstance?.CheckPushName();
        this.Protocol.TreeNodeWriter.Write(FunXMPP.Connection.GetMessageNode(message, participantNodeName, participants, broadcastListName));
      }

      public void SendChatState(string to, string type, string media = null, string participant = null)
      {
        Log.l("web > (funxmpp) sendChatState type={0}", type);
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>();
        FunXMPP.KeyValue[] attrs = (FunXMPP.KeyValue[]) null;
        keyValueList.Add(new FunXMPP.KeyValue(nameof (to), to));
        if (participant != null)
          keyValueList.Add(new FunXMPP.KeyValue(nameof (participant), participant));
        if (media != null)
          attrs = new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue(nameof (media), media)
          };
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("chatstate", keyValueList.ToArray(), new FunXMPP.ProtocolTreeNode[1]
        {
          new FunXMPP.ProtocolTreeNode(type, attrs)
        }));
      }

      public void SendComposing(string to, string media = null, string participant = null)
      {
        this.SendChatState(to, "composing", media, participant);
      }

      public void SendPaused(string to) => this.SendChatState(to, "paused");

      private static FunXMPP.ProtocolTreeNode CreateEncryptedNode(
        FunXMPP.FMessage.Encrypted enc,
        FunXMPP.FMessage message,
        string id,
        DateTime? timestamp)
      {
        FunXMPP.FMessage.FunMediaType funMediaType = enc.fun_media_type;
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>((IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[2]
        {
          new FunXMPP.KeyValue("v", enc.cipher_version.ToString()),
          new FunXMPP.KeyValue("type", enc.cipher_text_type)
        });
        if (enc.cipher_retry_count > 0)
        {
          keyValueList.Add(new FunXMPP.KeyValue("count", enc.cipher_retry_count.ToString()));
          keyValueList.Add(new FunXMPP.KeyValue("oid", id));
          if (timestamp.HasValue)
            keyValueList.Add(new FunXMPP.KeyValue("ot", timestamp.Value.ToUnixTime().ToString()));
        }
        if (funMediaType != FunXMPP.FMessage.FunMediaType.Undefined && funMediaType != FunXMPP.FMessage.FunMediaType.Payment)
          keyValueList.Add(new FunXMPP.KeyValue("mediatype", FunXMPP.FMessage.GetFunMediaTypeStr(funMediaType)));
        if (funMediaType == FunXMPP.FMessage.FunMediaType.LiveLocation && message != null && message.media_duration_seconds > 0)
          keyValueList.Add(new FunXMPP.KeyValue("duration", message.media_duration_seconds.ToString()));
        return new FunXMPP.ProtocolTreeNode(nameof (enc), keyValueList.ToArray(), enc.cipher_text_bytes);
      }

      private static List<FunXMPP.ProtocolTreeNode> GetInnerNodes(
        FunXMPP.FMessage message,
        bool sendOwnJid)
      {
        List<FunXMPP.ProtocolTreeNode> innerNodes = new List<FunXMPP.ProtocolTreeNode>();
        if (message.multicast)
          innerNodes.Add(new FunXMPP.ProtocolTreeNode("multicast", (FunXMPP.KeyValue[]) null));
        if (message.urlPhoneNumber)
          innerNodes.Add(new FunXMPP.ProtocolTreeNode("url_number", (FunXMPP.KeyValue[]) null));
        if (message.urlText)
          innerNodes.Add(new FunXMPP.ProtocolTreeNode("url_text", (FunXMPP.KeyValue[]) null));
        foreach (FunXMPP.FMessage.Encrypted enc in message.encrypted)
          innerNodes.Add(FunXMPP.Connection.CreateEncryptedNode(enc, message, message.key.id, message.timestamp));
        if (message.HasPaymentInfo() && message.encrypted != null && message.encrypted.Length == 1 && message.encrypted[0].cipher_retry_count < 1)
          innerNodes.Add(FunXMPP.Connection.CreatePaymentNode(message.message_properties.PaymentsPropertiesField));
        return innerNodes;
      }

      internal static FunXMPP.ProtocolTreeNode GetMessageNode(
        FunXMPP.FMessage message,
        string participantNodeName = null,
        FunXMPP.FMessage.Participant[] participants = null,
        string broadcastListName = null,
        bool sendOwnJid = false,
        bool invis = false,
        bool qr = false)
      {
        FunXMPP.ProtocolTreeNode participantNode = participants != null ? FunXMPP.Connection.CreateParticipantNode(participantNodeName, (IEnumerable<FunXMPP.FMessage.Participant>) participants, broadcastListName) : (FunXMPP.ProtocolTreeNode) null;
        List<FunXMPP.ProtocolTreeNode> innerNodes = FunXMPP.Connection.GetInnerNodes(message, sendOwnJid);
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>();
        if (message.key.from_me)
        {
          keyValueList.Add(new FunXMPP.KeyValue("to", message.key.remote_jid));
          if (sendOwnJid)
            keyValueList.Add(new FunXMPP.KeyValue("from", Settings.MyJid));
          else if (message.web_relay)
            keyValueList.Add(new FunXMPP.KeyValue("web", "relay"));
          if (message.remote_resource != null)
            keyValueList.Add(new FunXMPP.KeyValue("participant", message.remote_resource));
          if (message.qcount > 0)
            keyValueList.Add(new FunXMPP.KeyValue("qcount", message.qcount.ToString()));
          if (message.media_wa_type == FunXMPP.FMessage.Type.Revoked)
            keyValueList.Add(new FunXMPP.KeyValue("edit", "7"));
        }
        else
        {
          keyValueList.Add(new FunXMPP.KeyValue("to", Settings.MyJid));
          string remoteJid = message.key.remote_jid;
          string b = message.remote_resource;
          if (b == "")
            b = (string) null;
          if (b != null && b.IsBroadcastJid())
            Utils.Swap<string>(ref remoteJid, ref b);
          keyValueList.Add(new FunXMPP.KeyValue("from", remoteJid));
          if (b != null)
            keyValueList.Add(new FunXMPP.KeyValue("participant", b));
          if (message.push_name != null)
            keyValueList.Add(new FunXMPP.KeyValue("notify", message.push_name));
        }
        string audience = message.Audience;
        if (!string.IsNullOrEmpty(audience))
          keyValueList.Add(new FunXMPP.KeyValue("audience", audience));
        if (qr && message.key.from_me)
        {
          string v;
          switch (message.status)
          {
            case FunXMPP.FMessage.Status.ReceivedByServer:
              v = "1";
              break;
            case FunXMPP.FMessage.Status.ReceivedByTarget:
              v = "2";
              break;
            case FunXMPP.FMessage.Status.NeverSend:
            case FunXMPP.FMessage.Status.Error:
            case FunXMPP.FMessage.Status.Canceled:
              v = "-1";
              break;
            case FunXMPP.FMessage.Status.PlayedByTarget:
              v = "4";
              break;
            case FunXMPP.FMessage.Status.ReadByTarget:
              v = "3";
              break;
            default:
              v = "0";
              break;
          }
          if (v != null)
            keyValueList.Add(new FunXMPP.KeyValue("status", v));
        }
        if (participantNode != null)
          innerNodes.Add(participantNode);
        if ((sendOwnJid || !message.key.from_me) && message.timestamp.HasValue)
        {
          long unixTime = message.timestamp.Value.ToUnixTime();
          keyValueList.Add(new FunXMPP.KeyValue("t", unixTime.ToString()));
        }
        string v1 = "media";
        switch (message.media_wa_type)
        {
          case FunXMPP.FMessage.Type.Undefined:
          case FunXMPP.FMessage.Type.ExtendedText:
            v1 = "text";
            if (message.HasPaymentInfo())
            {
              v1 = "pay";
              break;
            }
            break;
          case FunXMPP.FMessage.Type.CipherText:
            v1 = "ciphertext";
            break;
          case FunXMPP.FMessage.Type.Revoked:
            if (message.encrypted != null)
            {
              switch (((IEnumerable<FunXMPP.FMessage.Encrypted>) message.encrypted).FirstOrDefault<FunXMPP.FMessage.Encrypted>().fun_media_type)
              {
                case FunXMPP.FMessage.FunMediaType.Undefined:
                case FunXMPP.FMessage.FunMediaType.ExtendedText:
                  v1 = "text";
                  break;
              }
            }
            else
              break;
            break;
        }
        keyValueList.Add(new FunXMPP.KeyValue("type", v1));
        keyValueList.Add(new FunXMPP.KeyValue("id", message.key.id));
        if (invis)
          keyValueList.Add(new FunXMPP.KeyValue("web", nameof (invis)));
        if (message.participants_hash != null)
          keyValueList.Add(new FunXMPP.KeyValue("phash", message.participants_hash));
        return new FunXMPP.ProtocolTreeNode(nameof (message), keyValueList.ToArray(), innerNodes.Count > 0 ? innerNodes.ToArray() : (FunXMPP.ProtocolTreeNode[]) null);
      }

      internal static FunXMPP.ProtocolTreeNode CreateParticipantNode(
        string nodeName,
        IEnumerable<FunXMPP.FMessage.Participant> participants,
        string name = null)
      {
        List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList = (List<FunXMPP.ProtocolTreeNode>) null;
        if (participants != null && participants.Any<FunXMPP.FMessage.Participant>())
        {
          protocolTreeNodeList = new List<FunXMPP.ProtocolTreeNode>();
          foreach (FunXMPP.FMessage.Participant participant in participants)
          {
            FunXMPP.ProtocolTreeNode child = (FunXMPP.ProtocolTreeNode) null;
            if (participant.Encrypted != null)
              child = FunXMPP.Connection.CreateEncryptedNode(participant.Encrypted, (FunXMPP.FMessage) null, (string) null, new DateTime?());
            protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("to", new FunXMPP.KeyValue[1]
            {
              new FunXMPP.KeyValue("jid", participant.Jid)
            }, child));
          }
        }
        FunXMPP.KeyValue[] keyValueArray;
        if (!string.IsNullOrEmpty(name))
          keyValueArray = new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue(nameof (name), name)
          };
        else
          keyValueArray = (FunXMPP.KeyValue[]) null;
        FunXMPP.KeyValue[] attrs = keyValueArray;
        return new FunXMPP.ProtocolTreeNode(nodeName, attrs, protocolTreeNodeList?.ToArray());
      }

      public void SendStatusUpdate(
        string status,
        string incomingId,
        Action onComplete,
        Action<int> onError)
      {
        string str = incomingId ?? this.MakeId("sendstatus_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (onComplete == null)
            return;
          onComplete();
        }), (Action<int>) (err =>
        {
          if (onError == null)
            return;
          onError(err);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", FunXMPP.Connection.AppendIf<FunXMPP.KeyValue>(new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("xmlns", nameof (status))
        }, incomingId != null, new Func<FunXMPP.KeyValue>(FunXMPP.Connection.RelayIq)), new FunXMPP.ProtocolTreeNode(nameof (status), (FunXMPP.KeyValue[]) null, status)));
      }

      public bool SendUsyncQuery(
        UsyncQuery query,
        Action onIQReceive = null,
        Action onComplete = null,
        Action<int> onError = null)
      {
        string id = this.MakeId("usync_");
        FunXMPP.ProtocolTreeNode iq = query.ToIq(id);
        if (iq != null)
        {
          Log.l("usync", "send : {0}", (object) query.ToLogString());
          this.AddIqHandler(id, query.GenerateIqHandler(onIQReceive, onComplete, onError));
          this.Protocol.TreeNodeWriter.Write(iq);
          return true;
        }
        Log.l("usync", "bypassed usync send: {0}", (object) query.ToLogString());
        return false;
      }

      public void AddUsyncGetBusinesses(
        UsyncQuery query,
        IEnumerable<FunXMPP.Connection.BusinessRequest> jids,
        IEnumerable<FunXMPP.Connection.BusinessRequest> sidelistJids,
        Action onComplete = null,
        Action<string, int> onError = null)
      {
        query.OnGetBusinesses = (Action<IEnumerable<UsyncQuery.BusinessResult>, IEnumerable<UsyncQuery.BusinessResult>>) ((listResults, sidelistResults) =>
        {
          if (listResults != null)
            FunXMPP.Connection.handleUsyncBusinessResult(listResults);
          if (sidelistResults != null)
            FunXMPP.Connection.handleUsyncBusinessResult(sidelistResults);
          Action action = onComplete;
          if (action == null)
            return;
          action();
        });
        query.OnGetBusinessesError = (Action<int>) (err =>
        {
          Action<string, int> action = onError;
          if (action == null)
            return;
          action((string) null, err);
        });
        query.RequestBusinesses(jids, sidelistJids);
      }

      private static void handleUsyncBusinessResult(IEnumerable<UsyncQuery.BusinessResult> results)
      {
        foreach (UsyncQuery.BusinessResult result in results)
        {
          if (result.IsBusiness)
            VerifiedNamesCertifier.HandleBusiness(result);
          else
            VerifiedNamesCertifier.RemoveUsersBusinessDetails(result.Jid);
        }
      }

      private static ClientCapabilityCategory[] CapsValues
      {
        get
        {
          return Utils.LazyInit<ClientCapabilityCategory[]>(ref FunXMPP.Connection.capsValues, (Func<ClientCapabilityCategory[]>) (() => Enum.GetValues(typeof (ClientCapabilityCategory)).Cast<ClientCapabilityCategory>().Where<ClientCapabilityCategory>((Func<ClientCapabilityCategory, bool>) (cap => cap != 0)).ToArray<ClientCapabilityCategory>()));
        }
      }

      public bool AddUsyncGetRemoteCapabilities(
        UsyncQuery query,
        Action<RemoteClientCaps> onRecord = null,
        Action onComplete = null,
        Action<int> onError = null)
      {
        return this.AddUsyncGetRemoteCapabilities(query, (IEnumerable<string>) new string[0], onRecord, onComplete, onError);
      }

      public bool AddUsyncGetRemoteCapabilities(
        UsyncQuery query,
        IEnumerable<string> jids,
        Action<RemoteClientCaps> onRecord = null,
        Action onComplete = null,
        Action<int> onError = null)
      {
        query.OnGetRemoteCapabilities = (Action<System.Collections.Generic.Dictionary<string, RemoteClientCaps>, System.Collections.Generic.Dictionary<string, RemoteClientCaps>>) ((listFeatures, sidelistFeatures) =>
        {
          DateTime utcNow = DateTime.UtcNow;
          if (listFeatures != null)
          {
            System.Collections.Generic.Dictionary<string, RemoteClientCaps> dictionary = new System.Collections.Generic.Dictionary<string, RemoteClientCaps>((IDictionary<string, RemoteClientCaps>) listFeatures);
            FunXMPP.Connection.PopulateMissingRemoteCaps(jids, utcNow, dictionary);
            this.HandleRemoteCapabilities(dictionary, onComplete, onRecord);
          }
          if (sidelistFeatures != null)
          {
            System.Collections.Generic.Dictionary<string, RemoteClientCaps> dictionary = new System.Collections.Generic.Dictionary<string, RemoteClientCaps>((IDictionary<string, RemoteClientCaps>) sidelistFeatures);
            FunXMPP.Connection.PopulateMissingRemoteCaps(jids, utcNow, dictionary);
            this.HandleRemoteCapabilities(dictionary, onComplete, onRecord);
          }
          Action action = onComplete;
          if (action == null)
            return;
          action();
        });
        query.OnGetRemoteCapabilitiesError = onError;
        return query.RequestRemoteCapabilities(jids, (IEnumerable<string>) new string[0]);
      }

      public static RemoteClientCaps ParseRemoteClientCaps(
        string jid,
        DateTime now,
        FunXMPP.ProtocolTreeNode userFeatureNode)
      {
        RemoteClientCaps remoteClientCaps = new RemoteClientCaps()
        {
          Jid = jid
        };
        Set<ClientCapabilityCategory> set = new Set<ClientCapabilityCategory>();
        foreach (FunXMPP.ProtocolTreeNode node in userFeatureNode.children ?? new FunXMPP.ProtocolTreeNode[0])
        {
          ClientCapabilityCategory type;
          ClientCapabilitySetting setting;
          if (FunXMPP.Connection.TryParseFeatureNode(node, out type, out setting))
          {
            remoteClientCaps.AddValue(type, setting, new DateTime?(now));
            set.Add(type);
          }
        }
        foreach (ClientCapabilityCategory capsValue in FunXMPP.Connection.CapsValues)
        {
          if (capsValue != ClientCapabilityCategory.None && !set.Contains(capsValue))
            remoteClientCaps.AddValue(capsValue, ClientCapabilitySetting.Disabled, new DateTime?(now));
        }
        return remoteClientCaps;
      }

      private static void PopulateMissingRemoteCaps(
        IEnumerable<string> jids,
        DateTime now,
        System.Collections.Generic.Dictionary<string, RemoteClientCaps> results)
      {
        foreach (string jid in jids)
        {
          if (!results.ContainsKey(jid))
          {
            RemoteClientCaps remoteClientCaps = new RemoteClientCaps()
            {
              Jid = jid
            };
            results[jid] = remoteClientCaps;
            foreach (ClientCapabilityCategory capsValue in FunXMPP.Connection.CapsValues)
              remoteClientCaps.AddValue(capsValue, ClientCapabilitySetting.Disabled, new DateTime?(now));
          }
        }
      }

      private void HandleRemoteCapabilities(
        System.Collections.Generic.Dictionary<string, RemoteClientCaps> features,
        Action onComplete = null,
        Action<RemoteClientCaps> onRecord = null)
      {
        if (!features.Any<KeyValuePair<string, RemoteClientCaps>>())
          return;
        Action onComplete1 = (Action) null;
        if (onComplete != null)
          onComplete1 = new Action(RefCountAction.Replace(ref onComplete).Subscribe().Dispose);
        this.EventHandler.OnRemoteClientCaps((IEnumerable<RemoteClientCaps>) features.Values, onComplete1);
        if (onRecord == null)
          return;
        foreach (RemoteClientCaps remoteClientCaps in features.Values)
          onRecord(remoteClientCaps);
      }

      public void AddUsyncGetStatuses(
        UsyncQuery query,
        Action<string, DateTime?, string> onRecord = null,
        Action onComplete = null,
        Action<string, int> onError = null)
      {
        this.AddUsyncGetStatuses(query, (IEnumerable<FunXMPP.Connection.StatusRequest>) new FunXMPP.Connection.StatusRequest[0], (IEnumerable<FunXMPP.Connection.StatusRequest>) new FunXMPP.Connection.StatusRequest[0], onRecord, onComplete, onError);
      }

      public void AddUsyncGetStatuses(
        UsyncQuery query,
        IEnumerable<FunXMPP.Connection.StatusRequest> jids,
        IEnumerable<FunXMPP.Connection.StatusRequest> sidelistJids,
        Action<string, DateTime?, string> onRecord = null,
        Action onComplete = null,
        Action<string, int> onError = null)
      {
        query.OnGetStatuses = (Action<IEnumerable<UsyncQuery.StatusResult>, IEnumerable<UsyncQuery.StatusResult>>) ((listStatusResults, sidelistStatusResults) =>
        {
          if (listStatusResults != null)
            this.HandleStatusResults(listStatusResults, onRecord, onError);
          if (sidelistStatusResults != null)
            this.HandleStatusResults(sidelistStatusResults, onRecord, onError);
          Action action = onComplete;
          if (action == null)
            return;
          action();
        });
        query.OnGetStatusesError = (Action<int>) (err =>
        {
          if (onError == null)
            return;
          onError((string) null, err);
        });
        query.RequestStatuses(jids, sidelistJids);
      }

      private void HandleStatusResults(
        IEnumerable<UsyncQuery.StatusResult> results,
        Action<string, DateTime?, string> onRecord,
        Action<string, int> onError = null)
      {
        DateTime? nullable1;
        foreach (UsyncQuery.StatusResult result in results)
        {
          if (!result.failed)
          {
            if (this.EventHandler != null)
            {
              FunXMPP.Listener eventHandler = this.EventHandler;
              string jid = result.jid;
              nullable1 = result.timestamp;
              DateTime? timestamp = new DateTime?(nullable1 ?? DateTime.UtcNow);
              string status = result.status;
              eventHandler.OnStatusUpdate(jid, timestamp, status, true);
            }
            if (onRecord != null)
            {
              Action<string, DateTime?, string> action = onRecord;
              string jid = result.jid;
              nullable1 = result.timestamp;
              DateTime? nullable2 = new DateTime?(nullable1 ?? DateTime.UtcNow);
              string status = result.status;
              action(jid, nullable2, status);
            }
          }
          else
          {
            switch (result.code)
            {
              case 401:
              case 403:
              case 404:
                FunXMPP.Listener eventHandler = this.EventHandler;
                string jid = result.jid;
                nullable1 = new DateTime?();
                DateTime? timestamp = nullable1;
                eventHandler.OnStatusUpdate(jid, timestamp, (string) null, false);
                break;
            }
            if (onError != null)
              onError(result.jid, result.code);
          }
        }
      }

      public void AddUsyncGetContacts(
        UsyncQuery query,
        string sid,
        int idx,
        bool last,
        IEnumerable<string> numbers,
        IEnumerable<string> deletedJids,
        Action<FunXMPP.Connection.SyncResult> onResult,
        Action<int> onError = null)
      {
        query.OnGetContacts = (Action<FunXMPP.Connection.SyncResult>) (res =>
        {
          if (res != null)
          {
            this.LogContacts(res, "contacts");
            FunXMPP.Connection.MarkJidsAsSidelistSynced(((IEnumerable<FunXMPP.Connection.SyncResult.User>) res.SwellFolks).Select<FunXMPP.Connection.SyncResult.User, string>((Func<FunXMPP.Connection.SyncResult.User, string>) (u => u.Jid)), false);
            FunXMPP.Connection.MarkJidsAsSidelistSynced(((IEnumerable<FunXMPP.Connection.SyncResult.User>) res.Holdouts).Select<FunXMPP.Connection.SyncResult.User, string>((Func<FunXMPP.Connection.SyncResult.User, string>) (u => u.Jid)), false);
          }
          onResult(res);
        });
        query.OnGetContactsError = onError;
        query.RequestContacts(sid, idx, last, numbers, deletedJids);
      }

      public void AddUsyncGetSidelist(
        UsyncQuery query,
        IEnumerable<string> sidelistJids,
        Action<FunXMPP.Connection.SyncResult> onComplete = null,
        Action<int> onError = null)
      {
        query.OnGetSidelist = (Action<FunXMPP.Connection.SyncResult>) (res =>
        {
          if (res != null)
          {
            this.LogContacts(res, "sidelist");
            FunXMPP.Connection.MarkJidsAsSidelistSynced(((IEnumerable<FunXMPP.Connection.SyncResult.User>) res.SwellFolks).Select<FunXMPP.Connection.SyncResult.User, string>((Func<FunXMPP.Connection.SyncResult.User, string>) (u => u.Jid)), true);
            FunXMPP.Connection.MarkJidsAsSidelistSynced(((IEnumerable<FunXMPP.Connection.SyncResult.User>) res.Holdouts).Select<FunXMPP.Connection.SyncResult.User, string>((Func<FunXMPP.Connection.SyncResult.User, string>) (u => u.Jid)), true);
          }
          Action<FunXMPP.Connection.SyncResult> action = onComplete;
          if (action == null)
            return;
          action(res);
        });
        query.OnGetSidelistError = onError;
        query.RequestSidelist(sidelistJids);
      }

      private static void MarkJidsAsSidelistSynced(IEnumerable<string> jids, bool value)
      {
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          List<UserStatus> userStatuses = db.GetUserStatuses(jids, false, false);
          bool flag = false;
          foreach (UserStatus userStatus in userStatuses)
          {
            if (userStatus != null && userStatus.IsSidelistSynced != value)
            {
              userStatus.IsSidelistSynced = value;
              flag = true;
            }
          }
          if (!flag)
            return;
          db.SubmitChanges();
        }));
      }

      private void LogContacts(FunXMPP.Connection.SyncResult contactResults, string prefix)
      {
        Log.l("ContactSync", "{0}: Got {1} members, {2} holdouts, and {3} normalization errors", (object) prefix, (object) contactResults.SwellFolks.Length, (object) contactResults.Holdouts.Length, (object) (contactResults.NormalizationErrors != null ? contactResults.NormalizationErrors.Length : 0));
      }

      public void SendGroupEncryptionMessageWithListener(FunXMPP.FMessage message, Action action)
      {
        this.AddReceiptListener(message.key.id, message.remote_resource, action);
        this.SendMessage(message);
      }

      public void AddReceiptListener(string keyId, string keyRemoteJid, Action action)
      {
        lock (this.pendingListenerActionsLock)
        {
          FunXMPP.FMessage.Key key = new FunXMPP.FMessage.Key(keyRemoteJid, true, keyId);
          if (!this.pendingListenerActions.ContainsKey(key))
            this.pendingListenerActions[key] = new List<Action>();
          this.pendingListenerActions[key].Add(action);
        }
      }

      public List<Action> PopReceiptListener(string keyId, string keyRemoteJid)
      {
        List<Action> actionList = (List<Action>) null;
        FunXMPP.FMessage.Key key = new FunXMPP.FMessage.Key(keyRemoteJid, true, keyId);
        lock (this.pendingListenerActionsLock)
        {
          if (this.pendingListenerActions.ContainsKey(key))
          {
            actionList = this.pendingListenerActions[key];
            this.pendingListenerActions.Remove(key);
          }
        }
        return actionList;
      }

      private FunXMPP.Connection.PendingReceiptState LookupPendingReceipt(
        string jid,
        string participant,
        string id,
        string type,
        bool create,
        bool remove)
      {
        string key = string.Format("{0},{1},{2},{3}", (object) jid, (object) (participant ?? ""), (object) id, (object) (type ?? "delivery"));
        FunXMPP.Connection.PendingReceiptState pendingReceiptState = (FunXMPP.Connection.PendingReceiptState) null;
        lock (this.readReceiptLock)
        {
          if (!this.pendingReadReceipts.TryGetValue(key, out pendingReceiptState))
          {
            if (!create)
              return (FunXMPP.Connection.PendingReceiptState) null;
            this.pendingReadReceipts[key] = pendingReceiptState = new FunXMPP.Connection.PendingReceiptState();
          }
          if (remove)
            this.pendingReadReceipts.Remove(key);
        }
        return pendingReceiptState;
      }

      public void SendReceipt(
        string to,
        string participant,
        string id,
        string type,
        FunXMPP.ProtocolTreeNode child,
        Action onComplete = null,
        string editVersion = null)
      {
        this.SendReceiptImpl(to, participant, id, type, new FunXMPP.ProtocolTreeNode[1]
        {
          child
        }, onComplete, editVersion);
      }

      public void SendReceipt(
        string to,
        string participant,
        string id,
        string type,
        Pair<string, string>[] extraIds,
        Action onComplete = null,
        string editVersion = null)
      {
        FunXMPP.Connection.PendingReceiptState receipt = (FunXMPP.Connection.PendingReceiptState) null;
        Func<FunXMPP.Connection.PendingReceiptState> func = (Func<FunXMPP.Connection.PendingReceiptState>) (() => receipt ?? (receipt = this.LookupPendingReceipt(to, participant, id, type, true, false)));
        Log.l("sending {0} reciept(s)", (object) (extraIds == null ? 1 : extraIds.Length + 1));
        FunXMPP.ProtocolTreeNode[] children = (FunXMPP.ProtocolTreeNode[]) null;
        if (extraIds != null && extraIds.Length != 0)
        {
          children = new FunXMPP.ProtocolTreeNode[1]
          {
            new FunXMPP.ProtocolTreeNode("list", (FunXMPP.KeyValue[]) null, ((IEnumerable<Pair<string, string>>) extraIds).Select<Pair<string, string>, FunXMPP.ProtocolTreeNode>((Func<Pair<string, string>, FunXMPP.ProtocolTreeNode>) (extraId =>
            {
              List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>();
              keyValueList.Add(new FunXMPP.KeyValue(nameof (id), extraId.First));
              if (!string.IsNullOrWhiteSpace(extraId.Second))
                keyValueList.Add(new FunXMPP.KeyValue("edit", extraId.Second));
              return new FunXMPP.ProtocolTreeNode("item", keyValueList.ToArray());
            })).ToArray<FunXMPP.ProtocolTreeNode>())
          };
          func().AdditionalIds = extraIds;
        }
        this.SendReceiptImpl(to, participant, id, type, children, onComplete, editVersion, receipt);
      }

      public void SendReceipt(
        string to,
        string participant,
        string id,
        string type,
        FunXMPP.ProtocolTreeNode[] children = null,
        Action onComplete = null,
        string editVersion = null)
      {
        this.SendReceiptImpl(to, participant, id, type, children, onComplete, editVersion);
      }

      private void SendReceiptImpl(
        string to,
        string participant,
        string id,
        string type,
        FunXMPP.ProtocolTreeNode[] children = null,
        Action onComplete = null,
        string editVersion = null,
        FunXMPP.Connection.PendingReceiptState receipt = null)
      {
        Func<FunXMPP.Connection.PendingReceiptState> func = (Func<FunXMPP.Connection.PendingReceiptState>) (() => receipt ?? (receipt = this.LookupPendingReceipt(to, participant, id, type, true, false)));
        List<FunXMPP.KeyValue> second = new List<FunXMPP.KeyValue>();
        if (type != null)
          second.Add(new FunXMPP.KeyValue(nameof (type), type));
        if (participant != null)
          second.Add(new FunXMPP.KeyValue(nameof (participant), participant));
        if (!string.IsNullOrEmpty(editVersion))
          second.Add(new FunXMPP.KeyValue("edit", editVersion));
        if (onComplete != null)
        {
          FunXMPP.Connection.PendingReceiptState pendingReceiptState = func();
          if (pendingReceiptState.AckCallback != null)
          {
            Action snap = pendingReceiptState.AckCallback;
            pendingReceiptState.AckCallback = (Action) (() =>
            {
              snap();
              onComplete();
            });
          }
          else
            pendingReceiptState.AckCallback = onComplete;
        }
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode(nameof (receipt), ((IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[2]
        {
          new FunXMPP.KeyValue(nameof (to), to),
          new FunXMPP.KeyValue(nameof (id), id)
        }).Concat<FunXMPP.KeyValue>((IEnumerable<FunXMPP.KeyValue>) second).ToArray<FunXMPP.KeyValue>(), children));
      }

      public void SendReceipts(ICollection<FunXMPP.Connection.Ack> acks, string type = null)
      {
        foreach (IGrouping<Pair<string, string>, Pair<string, string>> source in acks.GroupBy<FunXMPP.Connection.Ack, Pair<string, string>, Pair<string, string>>((Func<FunXMPP.Connection.Ack, Pair<string, string>>) (ack => new Pair<string, string>(ack.To, ack.Participant)), (Func<FunXMPP.Connection.Ack, Pair<string, string>>) (ack => new Pair<string, string>(ack.Id, ack.EditVersion))))
        {
          string first = source.Key.First;
          string second = source.Key.Second;
          this.SwapIfNeeded(ref first, ref second);
          this.SendReceipt(first, second, source.First<Pair<string, string>>().First, type, source.Skip<Pair<string, string>>(1).ToArray<Pair<string, string>>());
        }
      }

      private void SwapIfNeeded(ref string to, ref string participant)
      {
        if (participant == "")
          participant = (string) null;
        if (participant == null || !participant.IsBroadcastJid())
          return;
        Utils.Swap<string>(ref to, ref participant);
      }

      public void SendReceipt(FunXMPP.FMessage message, string type = null)
      {
        string remoteJid = message.key.remote_jid;
        string remoteResource = message.remote_resource;
        this.SwapIfNeeded(ref remoteJid, ref remoteResource);
        this.SendReceipt(remoteJid, remoteResource, message.key.id, type, editVersion: message.edit_version > 0 ? message.edit_version.ToString() : (string) null);
      }

      public void SendMessageReceived(FunXMPP.FMessage message)
      {
        if (message.offline >= 0)
        {
          Log.l("Offline message. Retries = {0}", (object) message.offline);
          FunXMPP.Connection.Ack ack = new FunXMPP.Connection.Ack()
          {
            To = message.key.remote_jid,
            Participant = message.remote_resource,
            Id = message.key.id,
            EditVersion = message.edit_version > 0 ? message.edit_version.ToString() : ""
          };
          Settings.ModifyDanglingAcks((Action<List<FunXMPP.Connection.Ack>>) (acks => acks.Add(ack)));
          if (message.offline == 0)
          {
            this.PendingAcks.Add(ack);
          }
          else
          {
            this.SendPendingAcks();
            this.SendReceipt(message);
          }
        }
        else
          this.SendReceipt(message);
      }

      public void SendPendingAcks()
      {
        if (this.PendingAcks.Count <= 0)
          return;
        Log.l("PassiveMode", "Sending pending acks");
        this.SendReceipts((ICollection<FunXMPP.Connection.Ack>) this.PendingAcks);
        this.PendingAcks.Clear();
      }

      public void SendDanglingAcksAndExitPassive()
      {
        if (Settings.DanglingAcks.Count <= 0)
          return;
        Log.l("PassiveMode", "Sending dangling acks");
        this.SendReceipts((ICollection<FunXMPP.Connection.Ack>) Settings.DanglingAcks);
        FunXMPP.Connection.Ack last = Settings.DanglingAcks.Last<FunXMPP.Connection.Ack>();
        this.SendSetPassiveMode(false, (Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) => Settings.ModifyDanglingAcks((Action<List<FunXMPP.Connection.Ack>>) (acks => acks.RemoveRange(0, acks.IndexOf(last) + 1)))));
      }

      public void SendPingAndClearDanglingAcks()
      {
        if (Settings.DanglingAcks.Count <= 0)
          return;
        FunXMPP.Connection.Ack last = Settings.DanglingAcks.Last<FunXMPP.Connection.Ack>();
        Log.l("PassiveMode", "Sending ping");
        this.SendPing((Action) (() =>
        {
          Log.l("PassiveMode", "Got pong");
          Settings.ModifyDanglingAcks((Action<List<FunXMPP.Connection.Ack>>) (acks => acks.RemoveRange(0, acks.IndexOf(last) + 1)));
        }));
      }

      public void SendNotificationReceived(
        string to,
        string from,
        string participant,
        string id,
        string type,
        IEnumerable<FunXMPP.ProtocolTreeNode> extraNodes = null)
      {
        List<FunXMPP.KeyValue> second = new List<FunXMPP.KeyValue>();
        if (!string.IsNullOrEmpty(from))
          second.Add(new FunXMPP.KeyValue(nameof (from), from));
        if (!string.IsNullOrEmpty(participant))
          second.Add(new FunXMPP.KeyValue(nameof (participant), participant));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("ack", ((IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue(nameof (to), to),
          new FunXMPP.KeyValue("class", "notification"),
          new FunXMPP.KeyValue(nameof (id), id),
          new FunXMPP.KeyValue(nameof (type), type)
        }).Concat<FunXMPP.KeyValue>((IEnumerable<FunXMPP.KeyValue>) second).ToArray<FunXMPP.KeyValue>(), extraNodes != null ? extraNodes.ToArray<FunXMPP.ProtocolTreeNode>() : (FunXMPP.ProtocolTreeNode[]) null));
      }

      public void SendPresenceSubscriptionRequest(string to)
      {
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("presence", new FunXMPP.KeyValue[2]
        {
          new FunXMPP.KeyValue("type", "subscribe"),
          new FunXMPP.KeyValue(nameof (to), to)
        }));
      }

      public void SendActive()
      {
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("presence", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("type", "active")
        }));
      }

      public void SendInactive()
      {
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("presence", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("type", "inactive")
        }));
      }

      public void SendUnsubscribeMe(string jid)
      {
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("presence", new FunXMPP.KeyValue[2]
        {
          new FunXMPP.KeyValue("type", "unsubscribe"),
          new FunXMPP.KeyValue("to", jid)
        }));
      }

      public void SendUnsubscribeHim(string jid)
      {
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("presence", new FunXMPP.KeyValue[2]
        {
          new FunXMPP.KeyValue("type", "unsubscribed"),
          new FunXMPP.KeyValue("to", jid)
        }));
      }

      internal string MakeId(string prefix)
      {
        int num = this.iqid.NextTicket();
        return !this.IsVerboseId ? num.ToString("X") : prefix + (object) num;
      }

      public IObservable<Unit> SendClose()
      {
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("presence", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("type", "unavailable")
        }));
        this.Protocol.TreeNodeWriter.StreamEnd();
        return Observable.Return<Unit>(new Unit());
      }

      private void AddIqHandler(string id, FunXMPP.IqResultHandler handler)
      {
        lock (this.iqHandlerLock)
          this.pendingServerRequests.Add(id, handler);
      }

      private FunXMPP.IqResultHandler PopIqHandler(string id)
      {
        FunXMPP.IqResultHandler iqResultHandler = (FunXMPP.IqResultHandler) null;
        lock (this.iqHandlerLock)
        {
          if (this.pendingServerRequests.TryGetValue(id, out iqResultHandler))
            this.pendingServerRequests.Remove(id);
        }
        return iqResultHandler;
      }

      public void SendGetClientConfig()
      {
        string str = this.MakeId("get_config_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          FunXMPP.ProtocolTreeNode child = node.GetChild(0);
          FunXMPP.ProtocolTreeNode.Require(child, "config");
          this.EventHandler.OnClientConfigReceived(child.GetAttributeValue("id"));
        }), (Action<FunXMPP.ProtocolTreeNode>) (node =>
        {
          foreach (string push_id in node.GetAllChildren("config").Where<FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, bool>) (configNode => configNode != null)).Select<FunXMPP.ProtocolTreeNode, string>((Func<FunXMPP.ProtocolTreeNode, string>) (configNode => configNode.GetAttributeValue("id"))))
            this.EventHandler.OnClientConfigReceived(push_id);
        })));
        FunXMPP.ProtocolTreeNode child1 = new FunXMPP.ProtocolTreeNode("config", (FunXMPP.KeyValue[]) null);
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:push")
        }, child1));
      }

      public void SendClientConfig(
        Uri pushUri,
        System.Collections.Generic.Dictionary<string, string> attributes,
        IEnumerable<FunXMPP.Connection.GroupSetting> groups,
        Action onCompleted,
        Action<int> onError)
      {
        string str = this.MakeId("config_");
        if (onCompleted != null || onError != null)
        {
          if (onCompleted == null)
            onCompleted = (Action) (() => { });
          if (onError == null)
            onError = (Action<int>) (ign => { });
          this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) => onCompleted()), onError));
        }
        Log.l("Sending client config; {0}", string.Join(" ", attributes.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (kv => string.Format("{0}={1}", (object) kv.Key, (object) kv.Value)))));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:push")
        }, new FunXMPP.ProtocolTreeNode("config", ((IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("id", pushUri.ToString())
        }).Concat<FunXMPP.KeyValue>(attributes.Select<KeyValuePair<string, string>, FunXMPP.KeyValue>((Func<KeyValuePair<string, string>, FunXMPP.KeyValue>) (kv => new FunXMPP.KeyValue(kv.Key, kv.Value)))).Where<FunXMPP.KeyValue>((Func<FunXMPP.KeyValue, bool>) (kv => kv != null)).ToArray<FunXMPP.KeyValue>(), this.ProcessGroupSettings(groups))));
      }

      private FunXMPP.ProtocolTreeNode[] ProcessGroupSettings(
        IEnumerable<FunXMPP.Connection.GroupSetting> groups)
      {
        if (AppState.IsVoipScheduled())
          return (FunXMPP.ProtocolTreeNode[]) null;
        FunXMPP.ProtocolTreeNode[] protocolTreeNodeArray = (FunXMPP.ProtocolTreeNode[]) null;
        if (groups != null && groups.Any<FunXMPP.Connection.GroupSetting>())
        {
          DateTime now = DateTime.UtcNow;
          protocolTreeNodeArray = groups.Select<FunXMPP.Connection.GroupSetting, FunXMPP.ProtocolTreeNode>((Func<FunXMPP.Connection.GroupSetting, FunXMPP.ProtocolTreeNode>) (group => new FunXMPP.ProtocolTreeNode("item", new FunXMPP.KeyValue[3]
          {
            new FunXMPP.KeyValue("jid", group.Jid),
            new FunXMPP.KeyValue("notify", group.Enabled ? "1" : "0"),
            new FunXMPP.KeyValue("mute", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) (!group.MuteExpiry.HasValue || !(group.MuteExpiry.Value > now) ? 0 : (int) (group.MuteExpiry.Value - now).TotalSeconds)))
          }))).ToArray<FunXMPP.ProtocolTreeNode>();
        }
        return protocolTreeNodeArray;
      }

      public void SendStats(byte[] buffer, Action onComplete = null)
      {
        string str = this.MakeId("stats_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          Log.l("fieldstats sent to server");
          Action action = onComplete;
          if (action == null)
            return;
          action();
        }), (Action<int>) (code => Log.l("error sending fieldstats: {0}", (object) code))));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("xmlns", "w:stats"),
          new FunXMPP.KeyValue("type", "set")
        }, new FunXMPP.ProtocolTreeNode[1]
        {
          new FunXMPP.ProtocolTreeNode("add", new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("t", DateTime.Now.ToUnixTime().ToString())
          }, buffer)
        }), true);
      }

      public void SendStats(
        System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>> stats)
      {
      }

      public void SendReportLocation(byte[] encAttributes, int elapsed = 0)
      {
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("ib", new FunXMPP.KeyValue[0], new FunXMPP.ProtocolTreeNode[1]
        {
          new FunXMPP.ProtocolTreeNode("location", new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue(nameof (elapsed), elapsed.ToString())
          }, new FunXMPP.ProtocolTreeNode("enc", new FunXMPP.KeyValue[2]
          {
            new FunXMPP.KeyValue("v", "2"),
            new FunXMPP.KeyValue("type", "frskmsg")
          }, encAttributes))
        }));
      }

      public void SendToast(string title, string uri)
      {
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("ib", new FunXMPP.KeyValue[0], new FunXMPP.ProtocolTreeNode[1]
        {
          new FunXMPP.ProtocolTreeNode("notify", new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue(nameof (uri), uri)
          }, title)
        }));
      }

      public void SendClearToast(string convoid)
      {
        FunXMPP.KeyValue[] attrs = (FunXMPP.KeyValue[]) null;
        if (!string.IsNullOrEmpty(convoid))
          attrs = new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("group", convoid)
          };
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("ib", new FunXMPP.KeyValue[0], new FunXMPP.ProtocolTreeNode[1]
        {
          new FunXMPP.ProtocolTreeNode("clear", attrs)
        }));
      }

      public void SendPing(Action onPong = null)
      {
        string str = this.MakeId("ping_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) => (onPong ?? new Action(this.EventHandler.OnPingResponseReceived))())));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("xmlns", "w:p"),
          new FunXMPP.KeyValue("type", "get")
        }, new FunXMPP.ProtocolTreeNode("ping", new FunXMPP.KeyValue[0])));
      }

      public void SendSetPassiveMode(
        bool passive = true,
        Action<FunXMPP.ProtocolTreeNode, string> onResponse = null)
      {
        if (this.Passive == passive)
          return;
        Log.l("PassiveMode", "Switching from {0} mode to {1} mode", this.Passive ? (object) nameof (passive) : (object) "active", passive ? (object) nameof (passive) : (object) "active");
        string str = this.MakeId("passive_");
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, tag) =>
        {
          this.Passive = passive;
          if (onResponse != null)
            onResponse(node, tag);
          Log.l("PassiveMode", "Now in {0} mode", passive ? (object) nameof (passive) : (object) "active");
        }));
        this.AddIqHandler(str, handler);
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", nameof (passive))
        }, new FunXMPP.ProtocolTreeNode(passive ? nameof (passive) : "active", new FunXMPP.KeyValue[0])));
      }

      public void SendGetStatusV3PrivacyLists(Action onComplete)
      {
        string str = this.MakeId("getstatusv3privacylists_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          WaStatusHelper.StatusPrivacySettings defaultSetting = WaStatusHelper.StatusPrivacySettings.Undefined;
          System.Collections.Generic.Dictionary<WaStatusHelper.StatusPrivacySettings, string[]> privacyLists = new System.Collections.Generic.Dictionary<WaStatusHelper.StatusPrivacySettings, string[]>();
          FunXMPP.ProtocolTreeNode child = node.GetChild("privacy");
          if (child != null)
          {
            foreach (FunXMPP.ProtocolTreeNode allChild in child.GetAllChildren("list"))
            {
              string attributeValue = allChild.GetAttributeValue("type");
              int num = allChild.GetAttributeValue("default")?.ToLower() == "true" ? 1 : 0;
              WaStatusHelper.StatusPrivacySettings key = WaStatusHelper.StatusPrivacySettings.Undefined;
              switch (attributeValue)
              {
                case "contacts":
                  key = WaStatusHelper.StatusPrivacySettings.Contacts;
                  break;
                case "blacklist":
                  key = WaStatusHelper.StatusPrivacySettings.BlackList;
                  goto default;
                case "whitelist":
                  key = WaStatusHelper.StatusPrivacySettings.WhiteList;
                  goto default;
                default:
                  string[] array = allChild.GetAllChildren("user").Select<FunXMPP.ProtocolTreeNode, string>((Func<FunXMPP.ProtocolTreeNode, string>) (uNode => uNode.GetAttributeValue("jid"))).Where<string>((Func<string, bool>) (jid => JidHelper.IsUserJid(jid))).ToArray<string>();
                  privacyLists[key] = array;
                  break;
              }
              if (num != 0)
                defaultSetting = key;
            }
          }
          WaStatusHelper.InitPrivacySettings(privacyLists, defaultSetting);
          if (onComplete == null)
            return;
          onComplete();
        }), (Action<int>) (errCode =>
        {
          if (errCode == 404)
          {
            Log.l("funxmpp", "status v3 privacy setting not found", (object) errCode);
            WaStatusHelper.InitPrivacySettings((System.Collections.Generic.Dictionary<WaStatusHelper.StatusPrivacySettings, string[]>) null, WaStatusHelper.StatusPrivacySettings.Contacts);
          }
          else
            Log.l("funxmpp", "get status v3 privacy setting error: {0}", (object) errCode);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("xmlns", "status")
        }, new FunXMPP.ProtocolTreeNode("privacy", (FunXMPP.KeyValue[]) null)));
      }

      public void SendSetStatusV3PrivacyList(
        string listType,
        List<string> jids,
        Action onSuccess,
        Action onComplete,
        bool relay = false)
      {
        string str = this.MakeId("setstatusv3privacylists_");
        if (jids == null)
          jids = new List<string>();
        int n = jids.Count;
        Log.l("funxmpp", "set status privacy list | t:{0},n:{1}", (object) listType, (object) n);
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          onSuccess();
          Log.l("funxmpp", "set status privacy list success | t:{0},n:{1}", (object) listType, (object) n);
        }), (Action<int>) (errCode =>
        {
          Log.l("funxmpp", "set status privacy list error | t:{0},n:{1},err:{2}", (object) listType, (object) n, (object) errCode);
          if (errCode != 404)
            return;
          onSuccess();
        })));
        FunXMPP.ProtocolTreeNode[] array = jids.Select<string, FunXMPP.ProtocolTreeNode>((Func<string, FunXMPP.ProtocolTreeNode>) (jid => new FunXMPP.ProtocolTreeNode("user", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue(nameof (jid), jid)
        }))).ToArray<FunXMPP.ProtocolTreeNode>();
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("privacy", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("list", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("type", listType)
        }, ((IEnumerable<FunXMPP.ProtocolTreeNode>) array).Any<FunXMPP.ProtocolTreeNode>() ? array : (FunXMPP.ProtocolTreeNode[]) null));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", FunXMPP.Connection.AppendIf<FunXMPP.KeyValue>(new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", "status")
        }, relay, new Func<FunXMPP.KeyValue>(FunXMPP.Connection.RelayIq)), child));
      }

      private IEnumerable<string> GetJidsFromPrivacyList(FunXMPP.ProtocolTreeNode listNode)
      {
        if (listNode.children != null)
        {
          FunXMPP.ProtocolTreeNode[] protocolTreeNodeArray = listNode.children;
          for (int index = 0; index < protocolTreeNodeArray.Length; ++index)
          {
            FunXMPP.ProtocolTreeNode node = protocolTreeNodeArray[index];
            FunXMPP.ProtocolTreeNode.Require(node, "item");
            if ("jid" == node.GetAttributeValue("type"))
            {
              string attributeValue = node.GetAttributeValue("value");
              if (attributeValue != null)
                yield return attributeValue;
            }
          }
          protocolTreeNodeArray = (FunXMPP.ProtocolTreeNode[]) null;
        }
      }

      public void SendGetPrivacyList()
      {
        string str = this.MakeId("privacylist_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          FunXMPP.ProtocolTreeNode child1 = node.GetChild(0);
          FunXMPP.ProtocolTreeNode.Require(child1, "query");
          FunXMPP.ProtocolTreeNode child2 = child1.GetChild(0);
          FunXMPP.ProtocolTreeNode.Require(child2, "list");
          this.EventHandler.OnPrivacyBlockList(this.GetJidsFromPrivacyList(child2));
        }), (Action<int>) (errorCode =>
        {
          if (errorCode == 404)
            this.EventHandler.OnPrivacyBlockList((IEnumerable<string>) new string[0]);
          else
            Log.l("privacy list - unexpected error: ", (object) errorCode);
        })));
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("query", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("list", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("name", "default")
        }));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[3]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("xmlns", "jabber:iq:privacy")
        }, child));
      }

      public void SendSetBlockList(string[] jidsToBlock, Action onSuccess, Action<int> onError = null)
      {
        System.Collections.Generic.Dictionary<string, string> jidsToBlock1 = new System.Collections.Generic.Dictionary<string, string>();
        foreach (string key in jidsToBlock)
          jidsToBlock1[key] = (string) null;
        this.SendSetBlockList(jidsToBlock1, onSuccess, onError);
      }

      public void SendSetBlockList(
        System.Collections.Generic.Dictionary<string, string> jidsToBlock,
        Action onSuccess,
        Action<int> onError = null,
        string incomingId = null)
      {
        string str = incomingId ?? this.MakeId("privacy_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) => onSuccess()), onError));
        FunXMPP.ProtocolTreeNode[] array = jidsToBlock.Select<KeyValuePair<string, string>, FunXMPP.ProtocolTreeNode>((Func<KeyValuePair<string, string>, FunXMPP.ProtocolTreeNode>) (p =>
        {
          FunXMPP.ProtocolTreeNode protocolTreeNode = new FunXMPP.ProtocolTreeNode("item", new FunXMPP.KeyValue[3]
          {
            new FunXMPP.KeyValue("type", "jid"),
            new FunXMPP.KeyValue("value", p.Key),
            new FunXMPP.KeyValue("action", "deny")
          });
          if (p.Value != null)
            protocolTreeNode.AddAttribute("reason", p.Value);
          return protocolTreeNode;
        })).ToArray<FunXMPP.ProtocolTreeNode>();
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("query", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("list", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("name", "default")
        }, ((IEnumerable<FunXMPP.ProtocolTreeNode>) array).Any<FunXMPP.ProtocolTreeNode>() ? array : (FunXMPP.ProtocolTreeNode[]) null));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[3]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", "jabber:iq:privacy")
        }, child));
      }

      public void SendSpamReport(
        Message[] suspiciousMsgs,
        string flow,
        string convoJid,
        string groupCreatorJid = null,
        string groupSubject = null,
        Action onSuccess = null,
        Action<int> onError = null,
        string incomingId = null)
      {
        string str = incomingId ?? this.MakeId("spam_");
        List<FunXMPP.ProtocolTreeNode> source = new List<FunXMPP.ProtocolTreeNode>();
        bool isGroupJid = JidHelper.IsGroupJid(convoJid);
        if (suspiciousMsgs != null && ((IEnumerable<Message>) suspiciousMsgs).Count<Message>() > 0)
        {
          foreach (Message suspiciousMsg in suspiciousMsgs)
          {
            if (!suspiciousMsg.KeyFromMe && suspiciousMsg.MediaWaType != FunXMPP.FMessage.Type.Divider && suspiciousMsg.MediaWaType != FunXMPP.FMessage.Type.System && suspiciousMsg.MediaWaType != FunXMPP.FMessage.Type.Unsupported)
            {
              if (suspiciousMsg.MediaWaType != FunXMPP.FMessage.Type.CallOffer)
              {
                try
                {
                  FunXMPP.ProtocolTreeNode rawMessageNode = FunXMPP.Connection.getRawMessageNode(suspiciousMsg, convoJid, isGroupJid);
                  if (rawMessageNode != null)
                    source.Add(rawMessageNode);
                }
                catch (Exception ex)
                {
                  Log.l(ex, "Spam reporting message creation exception");
                }
              }
            }
          }
        }
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>();
        if (isGroupJid)
        {
          keyValueList.Add(new FunXMPP.KeyValue("jid", convoJid));
          if (groupCreatorJid != null)
            keyValueList.Add(new FunXMPP.KeyValue("creator", groupCreatorJid));
          if (groupSubject != null)
            keyValueList.Add(new FunXMPP.KeyValue("subject", groupSubject));
        }
        else
          keyValueList.Add(new FunXMPP.KeyValue("jid", convoJid));
        keyValueList.Add(new FunXMPP.KeyValue("spam_flow", flow));
        Log.d("Spam reporting", "Spam reporting for {0} with {1} flow", (object) convoJid, (object) flow);
        FunXMPP.ProtocolTreeNode child = source.Count<FunXMPP.ProtocolTreeNode>() == 0 ? (FunXMPP.ProtocolTreeNode) null : new FunXMPP.ProtocolTreeNode("spam_list", keyValueList.ToArray(), source.ToArray());
        FunXMPP.ProtocolTreeNode node1 = new FunXMPP.ProtocolTreeNode("iq", FunXMPP.Connection.AppendIf<FunXMPP.KeyValue>(new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", "spam"),
          new FunXMPP.KeyValue("to", "s.whatsapp.net")
        }, incomingId != null, new Func<FunXMPP.KeyValue>(FunXMPP.Connection.RelayIq)), child);
        if (onSuccess != null || onError != null)
          this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) => onSuccess()), onError));
        this.Protocol.TreeNodeWriter.Write(node1);
      }

      private static FunXMPP.ProtocolTreeNode getRawMessageNode(
        Message m,
        string convoJid,
        bool isGroupJid)
      {
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>();
        keyValueList.Add(new FunXMPP.KeyValue("from", convoJid));
        keyValueList.Add(new FunXMPP.KeyValue("id", m.KeyId));
        if (isGroupJid)
          keyValueList.Add(new FunXMPP.KeyValue("participant", m.RemoteResource));
        keyValueList.Add(new FunXMPP.KeyValue("type", m.MediaWaType == FunXMPP.FMessage.Type.Undefined ? "text" : "media"));
        keyValueList.Add(new FunXMPP.KeyValue("t", m.TimestampLong.ToString()));
        FunXMPP.ProtocolTreeNode protocolTreeNode = (FunXMPP.ProtocolTreeNode) null;
        if (m.MediaWaType == FunXMPP.FMessage.Type.CipherText)
          protocolTreeNode = new FunXMPP.ProtocolTreeNode("raw", (FunXMPP.KeyValue[]) null, (FunXMPP.ProtocolTreeNode) null);
        else if (m.MediaWaType == FunXMPP.FMessage.Type.ProtocolBuffer)
        {
          protocolTreeNode = new FunXMPP.ProtocolTreeNode("raw", new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("v", "2")
          }, m.BinaryData);
        }
        else
        {
          byte[] plainText = WhatsApp.ProtoBuf.Message.CreateFromFMessage(m.ToFMessage(), new CipherTextIncludes(true)).ToPlainText(false);
          if (m.MediaWaType == FunXMPP.FMessage.Type.Undefined)
          {
            protocolTreeNode = new FunXMPP.ProtocolTreeNode("raw", new FunXMPP.KeyValue[1]
            {
              new FunXMPP.KeyValue("v", "2")
            }, plainText);
          }
          else
          {
            FunXMPP.FMessage.FunMediaType funMediaType = m.GetFunMediaType();
            if (Enum.IsDefined(typeof (FunXMPP.FMessage.FunMediaType), (object) funMediaType) && funMediaType != FunXMPP.FMessage.FunMediaType.System && funMediaType != FunXMPP.FMessage.FunMediaType.Undefined)
              protocolTreeNode = new FunXMPP.ProtocolTreeNode("raw", new FunXMPP.KeyValue[2]
              {
                new FunXMPP.KeyValue("v", "2"),
                new FunXMPP.KeyValue("mediatype", FunXMPP.FMessage.GetFunMediaTypeStr(funMediaType))
              }, plainText);
            else
              Log.l("Spam reporting", "Unexpected message type ignored {0} {1}", (object) m.MediaWaType, (object) funMediaType);
          }
        }
        if (protocolTreeNode == null)
          return (FunXMPP.ProtocolTreeNode) null;
        FunXMPP.ProtocolTreeNode[] children;
        if (m.IsMulticast() && !isGroupJid)
          children = new FunXMPP.ProtocolTreeNode[2]
          {
            new FunXMPP.ProtocolTreeNode("multicast", (FunXMPP.KeyValue[]) null, (FunXMPP.ProtocolTreeNode) null),
            null
          };
        else
          children = new FunXMPP.ProtocolTreeNode[1];
        children[children.Length - 1] = protocolTreeNode;
        return new FunXMPP.ProtocolTreeNode("message", keyValueList.ToArray(), children);
      }

      public void SendSetGdprStage(int stage, Action onSuccess, Action<int> onError)
      {
        string str = this.MakeId("tos2_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          Action action = onSuccess;
          if (action == null)
            return;
          action();
        }), (Action<int>) (errCode =>
        {
          if (errCode != 406)
            ;
          Action<int> action = onError;
          if (action == null)
            return;
          action(errCode);
        })));
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("tos2", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue(nameof (stage), stage.ToString())
        });
        FunXMPP.ProtocolTreeNode node1 = new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:account"),
          new FunXMPP.KeyValue("to", "s.whatsapp.net")
        }, child);
        Log.d("funxmpp", "send gdpr stage: {0}", (object) stage);
        this.Protocol.TreeNodeWriter.Write(node1);
      }

      public void SendSetGdprPage(string page, Action onSuccess, Action<int> onError)
      {
        string str = this.MakeId("tos2_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          Action action = onSuccess;
          if (action == null)
            return;
          action();
        }), (Action<int>) (errCode =>
        {
          if (errCode != 406)
            ;
          Action<int> action = onError;
          if (action == null)
            return;
          action(errCode);
        })));
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("tos2", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue(nameof (page), page)
        });
        FunXMPP.ProtocolTreeNode node1 = new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:account"),
          new FunXMPP.KeyValue("to", "s.whatsapp.net")
        }, child);
        Log.l("funxmpp", "send gdpr page: {0}", (object) page);
        this.Protocol.TreeNodeWriter.Write(node1);
      }

      public void SendAckGdprReset(Action onSuccess, Action<int> onError)
      {
        string str = this.MakeId("tos2_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          Action action = onSuccess;
          if (action == null)
            return;
          action();
        }), (Action<int>) (errCode =>
        {
          if (errCode != 406)
            ;
          Action<int> action = onError;
          if (action == null)
            return;
          action(errCode);
        })));
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("tos2", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("reset", (FunXMPP.KeyValue[]) null));
        FunXMPP.ProtocolTreeNode node1 = new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:account"),
          new FunXMPP.KeyValue("to", "s.whatsapp.net")
        }, child);
        Log.d(nameof (SendAckGdprReset), "send reset");
        this.Protocol.TreeNodeWriter.Write(node1);
      }

      public void SendSetGdprAccept(Action onSuccess, Action<int> onError)
      {
        string str = this.MakeId("tos2_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          Action action = onSuccess;
          if (action == null)
            return;
          action();
        }), (Action<int>) (errCode =>
        {
          if (errCode != 406)
            ;
          Action<int> action = onError;
          if (action == null)
            return;
          action(errCode);
        })));
        FunXMPP.ProtocolTreeNode node1 = new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:account"),
          new FunXMPP.KeyValue("to", "s.whatsapp.net")
        }, new FunXMPP.ProtocolTreeNode("accept2", (FunXMPP.KeyValue[]) null));
        Log.d(nameof (SendSetGdprAccept), "send accept");
        this.Protocol.TreeNodeWriter.Write(node1);
      }

      public void SendSetSecurityCode(
        string securityCode,
        string email = null,
        Action onSuccess = null,
        Action<int> onError = null)
      {
        string str = this.MakeId("2fa_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          Action action = onSuccess;
          if (action == null)
            return;
          action();
        }), (Action<int>) (code =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(code);
        })));
        List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList = new List<FunXMPP.ProtocolTreeNode>();
        if (securityCode != null)
          protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("code", (FunXMPP.KeyValue[]) null, securityCode));
        if (email != null)
          protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode(nameof (email), (FunXMPP.KeyValue[]) null, email));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:account"),
          new FunXMPP.KeyValue("type", "set")
        }, new FunXMPP.ProtocolTreeNode("2fa", (FunXMPP.KeyValue[]) null, protocolTreeNodeList.ToArray())));
      }

      public void SendDeleteSecurityCode(Action onSuccess = null, Action<int> onError = null)
      {
        string str = this.MakeId("2fa_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          Action action = onSuccess;
          if (action == null)
            return;
          action();
        }), (Action<int>) (code =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(code);
        })));
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("2fa", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("code", (FunXMPP.KeyValue[]) null));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:account"),
          new FunXMPP.KeyValue("type", "set")
        }, child));
      }

      public void SendGetHaveSecurityCode(Action<bool, bool> onSuccess, Action<int> onError = null)
      {
        string str = this.MakeId("2fa_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          FunXMPP.ProtocolTreeNode child = node.GetChild("2fa");
          bool flag1 = false;
          bool flag2 = false;
          if (child.GetChild("code") != null)
            flag1 = true;
          if (child.GetChild("email") != null)
            flag2 = true;
          Action<bool, bool> action = onSuccess;
          if (action == null)
            return;
          action(flag1, flag2);
        }), (Action<int>) (code =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(code);
        })));
        FunXMPP.ProtocolTreeNode child1 = new FunXMPP.ProtocolTreeNode("2fa", (FunXMPP.KeyValue[]) null);
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:account"),
          new FunXMPP.KeyValue("type", "get")
        }, child1));
      }

      public void SendGetValidateSecurityCode(
        string securityCode,
        Action<bool, bool> onValidate,
        Action<int> onError = null)
      {
        string str = this.MakeId("2fa_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          FunXMPP.ProtocolTreeNode child = node.GetChild("2fa");
          bool flag1 = false;
          bool flag2 = false;
          if (child.GetChild("code") != null)
            flag1 = true;
          if (child.GetChild("email") != null)
            flag2 = true;
          Action<bool, bool> action = onValidate;
          if (action == null)
            return;
          action(flag1, flag2);
        }), (Action<int>) (code =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(code);
        })));
        FunXMPP.ProtocolTreeNode child1 = new FunXMPP.ProtocolTreeNode("2fa", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("code", (FunXMPP.KeyValue[]) null, securityCode));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:account"),
          new FunXMPP.KeyValue("type", "get")
        }, child1));
      }

      public void SendGetGroupInviteLink(
        Action<string> onSuccess,
        Action<string> onError,
        string jid)
      {
        string str1 = this.MakeId("invite_");
        this.AddIqHandler(str1, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          IEnumerable<FunXMPP.ProtocolTreeNode> allChildren = node.GetAllChildren("invite");
          string str2 = (string) null;
          foreach (FunXMPP.ProtocolTreeNode protocolTreeNode in (IEnumerable<FunXMPP.ProtocolTreeNode>) ((object) allChildren ?? (object) new FunXMPP.ProtocolTreeNode[0]))
            str2 = protocolTreeNode.GetAttributeValue("code");
          if (onSuccess == null)
            return;
          onSuccess(str2);
        }), (Action<int>) (errCode =>
        {
          if (onError == null)
            return;
          switch (errCode)
          {
            case 401:
              onError(AppResources.InviteQueryNotAuthorized);
              break;
            case 403:
              onError(AppResources.InviteQueryForbidden);
              break;
            case 404:
              onError(AppResources.InviteQueryNoGroup);
              break;
            default:
              onError(AppResources.InviteQueryError);
              break;
          }
        })));
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("invite", (FunXMPP.KeyValue[]) null);
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str1),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("xmlns", "w:g2"),
          new FunXMPP.KeyValue("to", jid)
        }, child));
      }

      public void SendCreateGroupInviteLink(
        Action<string> onSuccess,
        Action<string> onError,
        string jid)
      {
        string str1 = this.MakeId("invite_");
        this.AddIqHandler(str1, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          IEnumerable<FunXMPP.ProtocolTreeNode> allChildren = node.GetAllChildren("invite");
          string str2 = (string) null;
          foreach (FunXMPP.ProtocolTreeNode protocolTreeNode in (IEnumerable<FunXMPP.ProtocolTreeNode>) ((object) allChildren ?? (object) new FunXMPP.ProtocolTreeNode[0]))
            str2 = protocolTreeNode.GetAttributeValue("code");
          if (onSuccess == null)
            return;
          onSuccess(str2);
        }), (Action<int>) (errCode =>
        {
          if (onError == null)
            return;
          switch (errCode)
          {
            case 401:
              onError(AppResources.InviteQueryNotAuthorized);
              break;
            case 403:
              onError(AppResources.InviteQueryForbidden);
              break;
            case 404:
              onError(AppResources.InviteQueryNoGroup);
              break;
            default:
              onError(AppResources.InviteQueryError);
              break;
          }
        })));
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("invite", (FunXMPP.KeyValue[]) null);
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str1),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", "w:g2"),
          new FunXMPP.KeyValue("to", jid)
        }, child));
      }

      public void SendGetGroupInfoInviteLink(
        Action<string> onSuccess,
        Action<string> onError,
        string code)
      {
        string str1 = this.MakeId("invite_");
        this.AddIqHandler(str1, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          FunXMPP.ProtocolTreeNode child = node.GetChild("group");
          List<string> participants = new List<string>();
          FunXMPP.Connection.GroupInfo groupInfo = (FunXMPP.Connection.GroupInfo) null;
          if (child != null)
          {
            groupInfo = this.ParseGroupInfo(child);
            participants.AddRange((IEnumerable<string>) groupInfo.AdminJids);
            participants.AddRange((IEnumerable<string>) groupInfo.NonadminJids);
          }
          if (onSuccess == null || groupInfo == null)
            return;
          if (participants.Contains(Settings.MyJid))
          {
            onError(AppResources.AcceptInviteAlreadyMember);
          }
          else
          {
            int count = participants.Count;
            int? attributeInt = child.GetAttributeInt("size");
            if (attributeInt.HasValue)
              count = attributeInt.Value;
            string str2 = JidHelper.GetDisplayNameForContactJid(groupInfo.CreatorJid);
            if (groupInfo.CreatorJid == Settings.MyJid)
              str2 = AppResources.You;
            string stringWithIndex = Plurals.Instance.GetStringWithIndex(AppResources.InviteJoinConfirmation0Plural, 2, (object) groupInfo.Subject, (object) str2, (object) count);
            List<string> contactsInGroup = new List<string>();
            ContactsContext.Instance((Action<ContactsContext>) (cdb =>
            {
              foreach (string jid in participants)
              {
                if (jid != null)
                {
                  UserStatus userStatus = cdb.GetUserStatus(jid, false);
                  if (userStatus != null && userStatus.IsInDeviceContactList)
                    contactsInGroup.Add(userStatus.GetDisplayName());
                }
              }
            }));
            contactsInGroup.Sort(new Comparison<string>(this.ParticipantCompareFunc));
            switch (contactsInGroup.Count)
            {
              case 1:
                stringWithIndex = Plurals.Instance.GetStringWithIndex(AppResources.InviteJoinConfirmation1Plural, 2, (object) groupInfo.Subject, (object) str2, (object) count, (object) contactsInGroup.ElementAt<string>(0));
                break;
              case 2:
                stringWithIndex = Plurals.Instance.GetStringWithIndex(AppResources.InviteJoinConfirmation2Plural, 2, (object) groupInfo.Subject, (object) str2, (object) count, (object) contactsInGroup.ElementAt<string>(0), (object) contactsInGroup.ElementAt<string>(1));
                break;
              default:
                if (contactsInGroup.Count >= 3)
                {
                  stringWithIndex = Plurals.Instance.GetStringWithIndex(AppResources.InviteJoinConfirmation3Plural, 2, (object) groupInfo.Subject, (object) str2, (object) count, (object) contactsInGroup.ElementAt<string>(0), (object) contactsInGroup.ElementAt<string>(1), (object) contactsInGroup.ElementAt<string>(2));
                  break;
                }
                break;
            }
            onSuccess(stringWithIndex);
          }
        }), (Action<int>) (errCode =>
        {
          if (onError == null)
            return;
          switch (errCode)
          {
            case 401:
              onError(AppResources.AcceptInviteNotAuthorized);
              break;
            case 404:
              onError(AppResources.AcceptInviteNoGroup);
              break;
            case 406:
              onError(AppResources.InviteQueryInvalidLink);
              break;
            case 410:
              onError(AppResources.AcceptInviteRevoked);
              break;
            case 419:
              onError(AppResources.AcceptInviteTooManyParticipants);
              break;
            default:
              onError(AppResources.InviteQueryError);
              break;
          }
        })));
        FunXMPP.ProtocolTreeNode child1 = new FunXMPP.ProtocolTreeNode("invite", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue(nameof (code), code)
        });
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str1),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("xmlns", "w:g2"),
          new FunXMPP.KeyValue("to", "g.us")
        }, child1));
      }

      public int ParticipantCompareFunc(string s1, string s2)
      {
        if ((object) s1 == (object) s2)
          return 0;
        Regex regex = new Regex("[a-zA-Z]+");
        if (regex.IsMatch(s1) && !regex.IsMatch(s2))
          return -1;
        return !regex.IsMatch(s1) && regex.IsMatch(s2) ? 1 : string.Compare(s1, s2);
      }

      public void SendAcceptGroupInfoInviteLink(
        Action<string> onSuccess,
        Action<string> onError,
        string code)
      {
        string str1 = this.MakeId("invite_");
        this.AddIqHandler(str1, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          IEnumerable<FunXMPP.ProtocolTreeNode> allChildren = node.GetAllChildren("group");
          string str2 = (string) null;
          foreach (FunXMPP.ProtocolTreeNode protocolTreeNode in (IEnumerable<FunXMPP.ProtocolTreeNode>) ((object) allChildren ?? (object) new FunXMPP.ProtocolTreeNode[0]))
            str2 = protocolTreeNode.GetAttributeValue("jid");
          if (onSuccess == null)
            return;
          onSuccess(str2);
        }), (Action<int>) (errCode =>
        {
          if (onError == null)
            return;
          Log.l(nameof (SendAcceptGroupInfoInviteLink), "Error code {0}", (object) errCode);
          switch (errCode)
          {
            case 401:
              onError(AppResources.AcceptInviteNotAuthorized);
              break;
            case 404:
              onError(AppResources.AcceptInviteNoGroup);
              break;
            case 410:
              onError(AppResources.AcceptInviteRevoked);
              break;
            case 419:
              onError(AppResources.AcceptInviteTooManyParticipants);
              break;
            default:
              onError(AppResources.AcceptInviteError);
              break;
          }
        })));
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("invite", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue(nameof (code), code)
        });
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str1),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", "w:g2"),
          new FunXMPP.KeyValue("to", "g.us")
        }, child));
      }

      public void SendGetHSMLanguagePack(
        string requestedNamespace,
        List<HsmLocalePackInfo> localesToLookFor,
        string requestReason,
        Action<string, string, string, string, string, byte[]> onComplete = null,
        Action<int> onError = null)
      {
        string str = this.MakeId("hsmlp_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          FunXMPP.ProtocolTreeNode child = node.GetChild("languagepack");
          if (child != null)
          {
            string attributeValue1 = child.GetAttributeValue("ns");
            string attributeValue2 = child.GetAttributeValue("lg");
            string attributeValue3 = child.GetAttributeValue("lc");
            string attributeValue4 = child.GetAttributeValue("hash");
            if (string.IsNullOrEmpty(attributeValue4))
              attributeValue4 = child.GetAttributeValue("version");
            byte[] data = child.data;
            Action<string, string, string, string, string, byte[]> action = onComplete;
            if (action == null)
              return;
            action(requestReason, attributeValue1, attributeValue2, attributeValue3, attributeValue4, data);
          }
          else
          {
            if (onError == null)
              return;
            Action<int> action = onError;
            if (action == null)
              return;
            action(400);
          }
        }), (Action<int>) (err =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(err);
        })));
        List<FunXMPP.KeyValue> keyValueList1 = new List<FunXMPP.KeyValue>();
        keyValueList1.Add(new FunXMPP.KeyValue("ns", requestedNamespace));
        if (!string.IsNullOrEmpty(requestReason))
          keyValueList1.Add(new FunXMPP.KeyValue("reason", requestReason));
        List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList = new List<FunXMPP.ProtocolTreeNode>();
        foreach (HsmLocalePackInfo hsmLocalePackInfo in localesToLookFor)
        {
          List<FunXMPP.KeyValue> keyValueList2 = new List<FunXMPP.KeyValue>();
          keyValueList2.Add(new FunXMPP.KeyValue("lg", hsmLocalePackInfo.Lg));
          if (!string.IsNullOrEmpty(hsmLocalePackInfo.Lc))
            keyValueList2.Add(new FunXMPP.KeyValue("lc", hsmLocalePackInfo.Lc));
          if (!string.IsNullOrEmpty(hsmLocalePackInfo.Hash))
            keyValueList2.Add(new FunXMPP.KeyValue("havehash", hsmLocalePackInfo.Hash.ToString()));
          protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("item", keyValueList2.ToArray(), (string) null));
        }
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("xmlns", "w:biz")
        }, new FunXMPP.ProtocolTreeNode("languagepack", keyValueList1.ToArray(), protocolTreeNodeList.ToArray())));
      }

      private static FunXMPP.ProtocolTreeNode CreatePaymentNode(
        MessageProperties.PaymentsProperties properties)
      {
        MessageProperties.PaymentsProperties.PayTypes? nullable = properties != null ? properties.PayType : throw new ArgumentNullException("Couldn't create payment node for message");
        MessageProperties.PaymentsProperties.PayTypes payTypes = MessageProperties.PaymentsProperties.PayTypes.SEND;
        if ((nullable.GetValueOrDefault() == payTypes ? (nullable.HasValue ? 1 : 0) : 0) == 0)
          throw new ArgumentException(string.Format("Unexpected payment property type {0}", (object) properties.PayType));
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>();
        keyValueList.Add(new FunXMPP.KeyValue("type", "send"));
        keyValueList.Add(new FunXMPP.KeyValue("currency", properties.Currency));
        keyValueList.Add(new FunXMPP.KeyValue("amount", properties.Amount));
        if (properties != null && properties.CredentialId != null)
          keyValueList.Add(new FunXMPP.KeyValue("credential-id", properties.CredentialId));
        if (properties.Receiver != null)
          keyValueList.Add(new FunXMPP.KeyValue("receiver", properties.Receiver));
        return new FunXMPP.ProtocolTreeNode("pay", keyValueList.ToArray());
      }

      public void SendPaymentTosQuery(Action<string> onComplete = null, Action<int> onError = null)
      {
        string str1 = this.MakeId("paytosq_");
        this.AddIqHandler(str1, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          string str2 = (string) null;
          FunXMPP.ProtocolTreeNode child = node.GetChild("accept_pay");
          if (child != null)
            str2 = child.GetAttributeValue("accept");
          if (str2 != null)
          {
            onComplete(str2);
          }
          else
          {
            Log.l(nameof (SendPaymentTosQuery), "no accept found");
            Action<int> action = onError;
            if (action == null)
              return;
            action(400);
          }
        }), (Action<int>) (err =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(err);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str1),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:account")
        }, new FunXMPP.ProtocolTreeNode("accept_pay", (FunXMPP.KeyValue[]) null)));
      }

      public void SendPaymentTosSet(Action<string> onComplete = null, Action<int> onError = null)
      {
        string str1 = this.MakeId("paytoss_");
        this.AddIqHandler(str1, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          string str2 = (string) null;
          FunXMPP.ProtocolTreeNode child = node.GetChild("accept_pay");
          if (child != null)
            str2 = child.GetAttributeValue("accept");
          if (str2 != null)
          {
            onComplete(str2);
          }
          else
          {
            Log.l(nameof (SendPaymentTosSet), "no accept found");
            Action<int> action = onError;
            if (action == null)
              return;
            action(400);
          }
        }), (Action<int>) (err =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(err);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str1),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:account")
        }, new FunXMPP.ProtocolTreeNode("accept_pay", (FunXMPP.KeyValue[]) null)));
      }

      public void SendPaymentIdQuery(Action<string> onComplete = null, Action<int> onError = null)
      {
        string str1 = this.MakeId("paypidq_");
        this.AddIqHandler(str1, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          string str2 = (string) null;
          FunXMPP.ProtocolTreeNode child = node.GetChild("account");
          if (child != null)
            str2 = child.GetAttributeValue("pid-setup");
          if (str2 != null)
          {
            onComplete(str2);
          }
          else
          {
            Log.l(nameof (SendPaymentIdQuery), "no pid found");
            Action<int> action = onError;
            if (action == null)
              return;
            action(400);
          }
        }), (Action<int>) (err =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(err);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str1),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("xmlns", "w:pay")
        }, new FunXMPP.ProtocolTreeNode("account", new List<FunXMPP.KeyValue>()
        {
          new FunXMPP.KeyValue("action", "pid-setup")
        }.ToArray())));
      }

      public void SendPaymentCreateWallet(
        string firstName,
        string lastName,
        bool isDefaultPayment,
        bool isDefaultpayout,
        Action<string, string, PaymentsMethod.PaymentTypes, int, bool, bool, string> onComplete = null,
        Action<int> onError = null)
      {
        string str = this.MakeId("paywc_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          FunXMPP.ProtocolTreeNode child = node.GetChild("account")?.GetChild("wallet");
          if (child != null)
          {
            string attributeValue = child.GetAttributeValue("credential-id");
            Action<string, string, PaymentsMethod.PaymentTypes, int, bool, bool, string> action = onComplete;
            if (action == null)
              return;
            action(attributeValue, "", PaymentsMethod.PaymentTypes.Wallet, 0, isDefaultPayment, isDefaultpayout, "INR");
          }
          else
          {
            if (onError == null)
              return;
            Action<int> action = onError;
            if (action == null)
              return;
            action(400);
          }
        }), (Action<int>) (err =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(err);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", "w:pay")
        }, new FunXMPP.ProtocolTreeNode("account", new List<FunXMPP.KeyValue>()
        {
          new FunXMPP.KeyValue("action", "create-wallet"),
          new FunXMPP.KeyValue("first-name", firstName),
          new FunXMPP.KeyValue("last-name", lastName),
          new FunXMPP.KeyValue("def-payments", isDefaultPayment ? "1" : "0"),
          new FunXMPP.KeyValue("def-payout", isDefaultpayout ? "1" : "0")
        }.ToArray())));
      }

      public void SendPaymentValidateBin(
        string binNumber,
        string countryCode,
        Action<string> onComplete = null,
        Action<int> onError = null)
      {
        string str = this.MakeId("paybin_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          FunXMPP.ProtocolTreeNode child = node.GetChild("account");
          if (child != null)
          {
            string attributeValue = child.GetAttributeValue("valid");
            Action<string> action = onComplete;
            if (action == null)
              return;
            action(attributeValue);
          }
          else
          {
            if (onError == null)
              return;
            Action<int> action = onError;
            if (action == null)
              return;
            action(400);
          }
        }), (Action<int>) (err =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(err);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("xmlns", "w:pay")
        }, new FunXMPP.ProtocolTreeNode("account", new List<FunXMPP.KeyValue>()
        {
          new FunXMPP.KeyValue("action", "validate-bin"),
          new FunXMPP.KeyValue("bin", binNumber),
          new FunXMPP.KeyValue("cc", countryCode)
        }.ToArray())));
      }

      public void SendPaymentAddCard(
        string token,
        string expiryMMYYYY,
        string countryCode,
        bool isDebitCard,
        bool isDefaultPayment,
        bool isDefaultpayout,
        Action<string, string, PaymentsMethod.PaymentTypes, int, bool, bool, string, string> onComplete = null,
        Action<int, string> onError = null)
      {
        string str = this.MakeId("payaddc_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          FunXMPP.ProtocolTreeNode child = node.GetChild("account")?.GetChild(isDebitCard ? "debit" : "credit");
          if (child != null)
          {
            string attributeValue = child.GetAttributeValue("credential-id");
            PaymentsMethod.PaymentTypes paymentTypes = isDebitCard ? PaymentsMethod.PaymentTypes.DebitCard : PaymentsMethod.PaymentTypes.CreditCard;
            Action<string, string, PaymentsMethod.PaymentTypes, int, bool, bool, string, string> action = onComplete;
            if (action == null)
              return;
            action(attributeValue, "", paymentTypes, 0, isDefaultPayment, isDefaultpayout, countryCode, "INR");
          }
          else
          {
            Log.l(nameof (SendPaymentAddCard), "Incomplete response");
            if (onError == null)
              return;
            Action<int, string> action = onError;
            if (action == null)
              return;
            action(400, (string) null);
          }
        }), (Action<FunXMPP.ProtocolTreeNode>) (errorNode =>
        {
          string attributeValue1 = errorNode.GetAttributeValue("code");
          string attributeValue2 = errorNode.GetAttributeValue("reason");
          Action<int, string> action = onError;
          if (action == null)
            return;
          action(int.Parse(attributeValue1, (IFormatProvider) CultureInfo.InvariantCulture), attributeValue2);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", "w:pay")
        }, new FunXMPP.ProtocolTreeNode("account", new List<FunXMPP.KeyValue>()
        {
          new FunXMPP.KeyValue("action", isDebitCard ? "add-debit-card" : "add-credit-card"),
          new FunXMPP.KeyValue(nameof (token), token),
          new FunXMPP.KeyValue("cc", countryCode),
          new FunXMPP.KeyValue("expiry-month", expiryMMYYYY.Substring(0, 2)),
          new FunXMPP.KeyValue("expiry-year", expiryMMYYYY.Substring(2)),
          new FunXMPP.KeyValue("def-payments", isDefaultPayment ? "1" : "0"),
          new FunXMPP.KeyValue("def-payout", isDefaultpayout ? "1" : "0")
        }.ToArray())));
      }

      public void SendPaymentAddBankAccount(
        string token,
        string countryCode,
        bool isDefaultPayment,
        bool isDefaultpayout,
        Action<string, string, PaymentsMethod.PaymentTypes, int, bool, bool, string, string> onComplete = null,
        Action<int> onError = null)
      {
        string str = this.MakeId("paybanka_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          FunXMPP.ProtocolTreeNode child = node.GetChild("account")?.GetChild("bank");
          if (child != null)
          {
            string attributeValue = child.GetAttributeValue("credential-id");
            Action<string, string, PaymentsMethod.PaymentTypes, int, bool, bool, string, string> action = onComplete;
            if (action == null)
              return;
            action(attributeValue, "", PaymentsMethod.PaymentTypes.BankAccount, 0, isDefaultPayment, isDefaultpayout, countryCode, "INR");
          }
          else
          {
            if (onError == null)
              return;
            Action<int> action = onError;
            if (action == null)
              return;
            action(400);
          }
        }), (Action<int>) (err =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(err);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", "w:pay")
        }, new FunXMPP.ProtocolTreeNode("account", new List<FunXMPP.KeyValue>()
        {
          new FunXMPP.KeyValue("action", "add-bank"),
          new FunXMPP.KeyValue(nameof (token), token),
          new FunXMPP.KeyValue("cc", countryCode),
          new FunXMPP.KeyValue("def-payments", isDefaultPayment ? "1" : "0"),
          new FunXMPP.KeyValue("def-payout", isDefaultpayout ? "1" : "0")
        }.ToArray())));
      }

      public void SendPaymentGetMethods(
        bool includeWalletBalance,
        Action<List<PaymentsMethod>> onComplete = null,
        Action<int> onError = null)
      {
        string str = this.MakeId("paygetmtd_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          bool flag1 = false;
          try
          {
            FunXMPP.ProtocolTreeNode child1 = node.GetChild("account");
            List<PaymentsMethod> paymentsMethodList = new List<PaymentsMethod>();
            if (child1 != null && child1.children != null)
            {
              bool flag2 = false;
              Log.d(nameof (SendPaymentGetMethods), "Found {0} payment methods", (object) ((IEnumerable<FunXMPP.ProtocolTreeNode>) child1.children).Count<FunXMPP.ProtocolTreeNode>());
              foreach (FunXMPP.ProtocolTreeNode child2 in child1.children)
              {
                string attributeValue1 = child2.GetAttributeValue("credential-id");
                string attributeValue2 = child2.GetAttributeValue("last4");
                string attributeValue3 = child2.GetAttributeValue("cc");
                int num1;
                switch (child2.GetAttributeValue("def-payout"))
                {
                  case "1":
                    num1 = 1;
                    break;
                  default:
                    num1 = 0;
                    break;
                }
                bool isPrimaryPayout = num1 != 0;
                int num2;
                switch (child2.GetAttributeValue("def-payment"))
                {
                  case "1":
                    num2 = 1;
                    break;
                  default:
                    num2 = 0;
                    break;
                }
                bool isPrimaryPayment = num2 != 0;
                string attributeValue4 = child2.GetAttributeValue("currency");
                PaymentsMethod paymentsMethod;
                switch (child2.tag)
                {
                  case "debit":
                    string attributeValue5 = child2.GetAttributeValue("card-type");
                    child2.GetAttributeValue("expiry-month");
                    child2.GetAttributeValue("expiry-year");
                    paymentsMethod = new PaymentsMethod(attributeValue1, attributeValue2, PaymentsMethod.PaymentTypes.DebitCard, PaymentsMethod.GetPaymentCardSubTypeAsInt(attributeValue5), isPrimaryPayment, isPrimaryPayout, attributeValue3);
                    break;
                  case "bank":
                    paymentsMethod = new PaymentsMethod(attributeValue1, attributeValue2, PaymentsMethod.PaymentTypes.BankAccount, 0, isPrimaryPayment, isPrimaryPayout, attributeValue3);
                    break;
                  case "wallet":
                    flag2 = !flag2 ? true : throw new ArgumentOutOfRangeException("Can't have multiple wallets");
                    paymentsMethod = new PaymentsMethod(attributeValue1, attributeValue2, PaymentsMethod.PaymentTypes.Wallet, 0, isPrimaryPayment, isPrimaryPayout, attributeValue3);
                    paymentsMethod.CurrencyIso4217 = attributeValue4;
                    if (includeWalletBalance)
                    {
                      long? nullable = PaymentsHelper.ConvertFBStringToLong(child2.GetAttributeValue("balance"));
                      if (nullable.HasValue)
                        paymentsMethod.UpdateBalance(nullable.Value, DateTime.UtcNow.ToUnixTime());
                    }
                    string attributeValue6 = child2.GetAttributeValue("first-name");
                    string attributeValue7 = child2.GetAttributeValue("last-name");
                    paymentsMethod.setName(attributeValue6, attributeValue7);
                    break;
                  default:
                    throw new ArgumentOutOfRangeException(string.Format("Unexpected payment method found {0}", (object) child2.tag));
                }
                paymentsMethodList.Add(paymentsMethod);
              }
            }
            onComplete(paymentsMethodList);
          }
          catch (Exception ex)
          {
            Log.l(ex, "Exception processing SendPaymentGetMethods");
            flag1 = true;
          }
          if (!flag1 || onError == null)
            return;
          Action<int> action = onError;
          if (action == null)
            return;
          action(400);
        }), (Action<int>) (err =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(err);
        })));
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>();
        keyValueList.Add(new FunXMPP.KeyValue("action", "get-methods"));
        if (!includeWalletBalance)
          keyValueList.Add(new FunXMPP.KeyValue("wallet-balance", "0"));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("xmlns", "w:pay")
        }, new FunXMPP.ProtocolTreeNode("account", keyValueList.ToArray())));
      }

      public void SendPaymentRemoveMethod(
        string credentialId,
        Action<string> onComplete = null,
        Action<int> onError = null)
      {
        string str1 = this.MakeId("paydelmtd_");
        this.AddIqHandler(str1, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          string str2 = (string) null;
          FunXMPP.ProtocolTreeNode child = node.GetChild("account");
          if (child != null)
            str2 = child.GetAttributeValue("credential-id");
          if (str2 != null)
          {
            onComplete(str2);
          }
          else
          {
            Log.l("RemovePayment", "no credential id found");
            Action<int> action = onError;
            if (action == null)
              return;
            action(400);
          }
        }), (Action<int>) (err =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(err);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str1),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", "w:pay")
        }, new FunXMPP.ProtocolTreeNode("account", new List<FunXMPP.KeyValue>()
        {
          new FunXMPP.KeyValue("action", "remove"),
          new FunXMPP.KeyValue("credential-id", credentialId)
        }.ToArray())));
      }

      public void SendPaymentCashInRequest(
        string contextUuid,
        string credentialId,
        string walletCredentialId,
        string amount,
        string currency,
        Action<string, long, string, string> onComplete = null,
        Action<int> onError = null)
      {
        string str1 = this.MakeId("paycashin_");
        this.AddIqHandler(str1, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          string str2 = (string) null;
          long num = 0;
          string str3 = (string) null;
          string str4 = (string) null;
          FunXMPP.ProtocolTreeNode child = node.GetChild("transaction");
          if (child != null)
          {
            str2 = child.GetAttributeValue("id");
            num = Convert.ToInt64(child.GetAttributeValue("ts"));
            str3 = child.GetAttributeValue("status");
            str4 = child.GetAttributeValue("verif-url");
          }
          if (str2 != null && str4 != null)
          {
            onComplete(str2, num, str3, str4);
          }
          else
          {
            Log.l(nameof (SendPaymentCashInRequest), "invalid data returned {0}, {1}, {2}, {3}", (object) str2, (object) num, (object) str3, (object) str4);
            Action<int> action = onError;
            if (action == null)
              return;
            action(400);
          }
        }), (Action<int>) (err =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(err);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str1),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", "w:pay")
        }, new FunXMPP.ProtocolTreeNode("account", new List<FunXMPP.KeyValue>()
        {
          new FunXMPP.KeyValue("action", "cashin"),
          new FunXMPP.KeyValue("context-id", contextUuid),
          new FunXMPP.KeyValue("credential-id", credentialId),
          new FunXMPP.KeyValue("wallet-id", walletCredentialId),
          new FunXMPP.KeyValue(nameof (amount), amount),
          new FunXMPP.KeyValue(nameof (currency), currency)
        }.ToArray())));
      }

      public void SendPaymentCashOutRequest(
        string contextUuid,
        string credentialId,
        string walletCredentialId,
        string amount,
        string currency,
        Action<string, long, string> onComplete = null,
        Action<int> onError = null)
      {
        string str1 = this.MakeId("paycashout_");
        this.AddIqHandler(str1, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          string str2 = (string) null;
          long num = 0;
          string str3 = (string) null;
          FunXMPP.ProtocolTreeNode child = node.GetChild("transaction");
          if (child != null)
          {
            str2 = child.GetAttributeValue("id");
            num = Convert.ToInt64(child.GetAttributeValue("ts") ?? "0");
            str3 = child.GetAttributeValue("status");
          }
          if (str2 != null)
          {
            onComplete(str2, num, str3);
          }
          else
          {
            Log.l(nameof (SendPaymentCashOutRequest), "invalid data returned {0}, {1}, {2}", (object) str2, (object) num, (object) str3);
            Action<int> action = onError;
            if (action == null)
              return;
            action(400);
          }
        }), (Action<int>) (err =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(err);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str1),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", "w:pay")
        }, new FunXMPP.ProtocolTreeNode("account", new List<FunXMPP.KeyValue>()
        {
          new FunXMPP.KeyValue("action", "cashout"),
          new FunXMPP.KeyValue("context-id", contextUuid),
          new FunXMPP.KeyValue("credential-id", credentialId),
          new FunXMPP.KeyValue("wallet-id", walletCredentialId),
          new FunXMPP.KeyValue(nameof (amount), amount),
          new FunXMPP.KeyValue(nameof (currency), currency)
        }.ToArray())));
      }

      public void SendPaymentGetTransactionRequest(
        string transactionId,
        Action<FunXMPP.Connection.PaymentsTransactionResponse> onComplete = null,
        Action<int> onError = null)
      {
        string str = this.MakeId("paytran_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          FunXMPP.Connection.PaymentsTransactionResponse transactionResponse = (FunXMPP.Connection.PaymentsTransactionResponse) null;
          FunXMPP.ProtocolTreeNode child = node.GetChild("transaction");
          if (child != null)
            transactionResponse = new FunXMPP.Connection.PaymentsTransactionResponse(child);
          onComplete(transactionResponse);
        }), (Action<int>) (err =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(err);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("xmlns", "w:pay")
        }, new FunXMPP.ProtocolTreeNode("account", new List<FunXMPP.KeyValue>()
        {
          new FunXMPP.KeyValue("action", "activity-details"),
          new FunXMPP.KeyValue("trans-id", transactionId)
        }.ToArray())));
      }

      public void SendPaymentGetTransactionsRequest(
        DateTime endDateTime,
        Action<List<FunXMPP.Connection.PaymentsTransactionResponse>> onComplete = null,
        Action<int> onError = null)
      {
        string str = this.MakeId("paytrans_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          IEnumerable<FunXMPP.ProtocolTreeNode> allChildren = node.GetAllChildren("transaction");
          List<FunXMPP.Connection.PaymentsTransactionResponse> transactionResponseList = (List<FunXMPP.Connection.PaymentsTransactionResponse>) null;
          if (allChildren != null && allChildren.Count<FunXMPP.ProtocolTreeNode>() > 0)
          {
            transactionResponseList = new List<FunXMPP.Connection.PaymentsTransactionResponse>();
            foreach (FunXMPP.ProtocolTreeNode transactionNode in allChildren)
              transactionResponseList.Add(new FunXMPP.Connection.PaymentsTransactionResponse(transactionNode));
          }
          onComplete(transactionResponseList);
        }), (Action<int>) (err =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(err);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("xmlns", "w:pay")
        }, new FunXMPP.ProtocolTreeNode("account", new List<FunXMPP.KeyValue>()
        {
          new FunXMPP.KeyValue("action", "activities"),
          new FunXMPP.KeyValue("to-time", endDateTime.ToString("ddMMyyyy")),
          new FunXMPP.KeyValue("max", "100")
        }.ToArray())));
      }

      public void SendSetBizVNameCheck(
        string jid,
        string vname,
        string fromTo,
        System.Collections.Generic.Dictionary<string, int> dictionary,
        Action onComplete = null,
        Action<int> onError = null)
      {
        string str = this.MakeId("vnamecheck_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) => onComplete()), (Action<int>) (err =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(err);
        })));
        string v = (string) null;
        if (dictionary == null)
          v = "404";
        else if (dictionary.Count == 0)
          v = "400";
        FunXMPP.ProtocolTreeNode protocolTreeNode1;
        if (v == null)
          protocolTreeNode1 = (FunXMPP.ProtocolTreeNode) null;
        else
          protocolTreeNode1 = new FunXMPP.ProtocolTreeNode("error", new FunXMPP.KeyValue[2]
          {
            new FunXMPP.KeyValue("code", v),
            new FunXMPP.KeyValue("version", "1")
          });
        FunXMPP.ProtocolTreeNode protocolTreeNode2 = protocolTreeNode1;
        FunXMPP.ProtocolTreeNode protocolTreeNode3 = new FunXMPP.ProtocolTreeNode("name", (FunXMPP.KeyValue[]) null, vname);
        FunXMPP.ProtocolTreeNode protocolTreeNode4 = (FunXMPP.ProtocolTreeNode) null;
        if (protocolTreeNode2 == null)
        {
          FunXMPP.ProtocolTreeNode[] children = new FunXMPP.ProtocolTreeNode[dictionary.Count];
          int num = 0;
          foreach (KeyValuePair<string, int> keyValuePair in dictionary)
          {
            string key = keyValuePair.Key;
            children[num++] = new FunXMPP.ProtocolTreeNode("item", new FunXMPP.KeyValue[2]
            {
              new FunXMPP.KeyValue("key", key),
              new FunXMPP.KeyValue("value", keyValuePair.Value.ToString())
            });
          }
          protocolTreeNode4 = new FunXMPP.ProtocolTreeNode("list", new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("version", "1")
          }, children);
        }
        FunXMPP.ProtocolTreeNode[] protocolTreeNodeArray;
        if (protocolTreeNode2 != null)
          protocolTreeNodeArray = new FunXMPP.ProtocolTreeNode[2]
          {
            protocolTreeNode3,
            protocolTreeNode2
          };
        else
          protocolTreeNodeArray = new FunXMPP.ProtocolTreeNode[2]
          {
            protocolTreeNode3,
            protocolTreeNode4
          };
        FunXMPP.ProtocolTreeNode[] children1 = protocolTreeNodeArray;
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("vname_check", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue(nameof (jid), jid)
        }, children1);
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("xmlns", "w:biz:vname_check"),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("to", fromTo)
        }, child));
      }

      public void SendStartLocationReportingResponse(string gjid, string id, bool successful)
      {
        if (successful)
        {
          this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[3]
          {
            new FunXMPP.KeyValue(nameof (id), id),
            new FunXMPP.KeyValue("type", "result"),
            new FunXMPP.KeyValue("to", gjid)
          }));
        }
        else
        {
          FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("error", new FunXMPP.KeyValue[2]
          {
            new FunXMPP.KeyValue("text", "not_authorized"),
            new FunXMPP.KeyValue("code", "401")
          });
          this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[3]
          {
            new FunXMPP.KeyValue(nameof (id), id),
            new FunXMPP.KeyValue("type", "error"),
            new FunXMPP.KeyValue("to", gjid)
          }, child));
        }
      }

      public void SendStopLocationReportingResponse(string id)
      {
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[3]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue(nameof (id), id),
          new FunXMPP.KeyValue("type", "result")
        }));
      }

      public void SendEnabledLocationSharingResponse(string from, string participant, string id)
      {
        FunXMPP.KeyValue[] attrs = (FunXMPP.KeyValue[]) null;
        if (participant != null)
          attrs = new FunXMPP.KeyValue[4]
          {
            null,
            null,
            null,
            new FunXMPP.KeyValue(nameof (participant), participant)
          };
        attrs[0] = new FunXMPP.KeyValue(nameof (id), id);
        attrs[1] = new FunXMPP.KeyValue("type", "result");
        attrs[2] = new FunXMPP.KeyValue("to", from);
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", attrs));
      }

      public void SendEnableLocationSharing(
        Action onAck,
        Action onNoAck,
        IEnumerable<FunXMPP.FMessage.Participant> participants)
      {
        string str = this.MakeId("livelocation_");
        this.AddNotificationHandler(str, onAck, onNoAck);
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("notification", new List<FunXMPP.KeyValue>((IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[3]
        {
          new FunXMPP.KeyValue("to", "location@broadcast"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "location")
        }).ToArray(), FunXMPP.Connection.CreateParticipantNode(nameof (participants), participants)));
      }

      public void SendDisableLocationSharing(
        Action onAck,
        Action onNoAck,
        string gjid,
        string id,
        string sequenceid)
      {
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>()
        {
          new FunXMPP.KeyValue("to", gjid),
          new FunXMPP.KeyValue("type", "location")
        };
        if (id == null)
          id = this.MakeId("livelocation_");
        else
          keyValueList.Add(new FunXMPP.KeyValue("web", "set"));
        keyValueList.Add(new FunXMPP.KeyValue(nameof (id), id));
        this.AddNotificationHandler(id, onAck, onNoAck);
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("disable", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue(nameof (id), sequenceid)
        });
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("notification", keyValueList.ToArray(), child));
      }

      public void SendSubscribeToLocationUpdates(
        Action onSuccess,
        Action<int> onError,
        string gjid,
        bool hasParticipants)
      {
        string str = this.MakeId("location_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          node.GetAttributeValue("type");
          node.GetAttributeValue("t");
          FunXMPP.ProtocolTreeNode child1 = node.GetChild("subscribe");
          int? attributeInt1 = child1.GetAttributeInt("duration");
          int? nullable = attributeInt1;
          int num = 0;
          if ((nullable.GetValueOrDefault() > num ? (nullable.HasValue ? 1 : 0) : 0) != 0)
            Settings.LiveLocationSubscriptionDuration = attributeInt1.Value;
          FunXMPP.ProtocolTreeNode child2 = child1.GetChild("list");
          if (child2 != null)
          {
            System.Collections.Generic.Dictionary<string, Tuple<int?, LocationData>> dictionary = new System.Collections.Generic.Dictionary<string, Tuple<int?, LocationData>>();
            foreach (FunXMPP.ProtocolTreeNode allChild in child2.GetAllChildren("participant"))
            {
              string attributeValue = allChild.GetAttributeValue("jid");
              int? attributeInt2 = allChild.GetAttributeInt("expiration");
              FunXMPP.ProtocolTreeNode child3 = allChild.GetChild("location");
              Tuple<int?, LocationData> tuple = new Tuple<int?, LocationData>(attributeInt2, (LocationData) null);
              if (child3 != null)
              {
                nullable = child3.GetAttributeInt("elapsed");
                int elapsed = nullable ?? 0;
                FunXMPP.ProtocolTreeNode child4 = child3.GetChild("enc");
                if (this.EventHandler != null)
                  this.EventHandler.OnLocationNotificationForMe(from, attributeValue, attributeInt2, elapsed, child4.data);
              }
            }
            this.SendActive();
          }
          onSuccess();
        }), onError));
        FunXMPP.KeyValue[] attrs;
        if (!hasParticipants)
          attrs = (FunXMPP.KeyValue[]) null;
        else
          attrs = new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("participants", "true")
          };
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("subscribe", attrs);
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("xmlns", "location"),
          new FunXMPP.KeyValue("to", gjid)
        }, child));
      }

      public void SendUnsubscribeToLocationUpdates(
        Action onSuccess,
        Action<int> onError,
        string gjid,
        string id)
      {
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>()
        {
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("xmlns", "location"),
          new FunXMPP.KeyValue("to", gjid)
        };
        if (id == null)
          id = this.MakeId("livelocation_");
        else
          keyValueList.Add(new FunXMPP.KeyValue("web", "set"));
        keyValueList.Add(new FunXMPP.KeyValue(nameof (id), id));
        this.AddIqHandler(id, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (onSuccess == null)
            return;
          onSuccess();
        }), onError));
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("unsubscribe", (FunXMPP.KeyValue[]) null);
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", keyValueList.ToArray(), child));
      }

      public void SendMms4EndpointQuery(
        Action<FunXMPP.ProtocolTreeNode> onComplete = null,
        Action<int> onError = null)
      {
        string str = this.MakeId("mm4eq_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          FunXMPP.ProtocolTreeNode child = node.GetChild("media_conn");
          if (child != null)
          {
            onComplete(child);
          }
          else
          {
            Log.l("mms4", "No media connection node");
            onError(400);
          }
        }), (Action<int>) (err =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(err);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", "w:m")
        }, new FunXMPP.ProtocolTreeNode("media_conn", (FunXMPP.KeyValue[]) null)));
      }

      private static string PrivacySettingToString(PrivacyVisibility setting)
      {
        switch (setting)
        {
          case PrivacyVisibility.None:
            return "none";
          case PrivacyVisibility.Contacts:
            return "contacts";
          case PrivacyVisibility.Everyone:
            return "all";
          default:
            throw new InvalidOperationException("not a valid setting");
        }
      }

      private static PrivacyVisibility ParsePrivacySetting(string val)
      {
        switch (val)
        {
          case "none":
            return PrivacyVisibility.None;
          case "contacts":
            return PrivacyVisibility.Contacts;
          case "all":
            return PrivacyVisibility.Everyone;
          default:
            Log.l("funxmpp", "unknown privacy setting: {0}", (object) val);
            return PrivacyVisibility.None;
        }
      }

      public void SendGetPrivacySettings(Action onComplete = null, Action<int> onError = null)
      {
        string str = this.MakeId("privacysettings_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          System.Collections.Generic.Dictionary<string, PrivacyVisibility> settings = new System.Collections.Generic.Dictionary<string, PrivacyVisibility>();
          FunXMPP.ProtocolTreeNode child1 = node.GetChild("privacy");
          if (child1 != null)
          {
            foreach (FunXMPP.ProtocolTreeNode allChild in child1.GetAllChildren("category"))
            {
              string attributeValue1 = allChild.GetAttributeValue("name");
              string attributeValue2 = allChild.GetAttributeValue("value");
              if (attributeValue2 == "error")
              {
                int? nullable = new int?();
                FunXMPP.ProtocolTreeNode child2 = allChild.GetChild("error");
                if (child2 != null)
                {
                  string attributeValue3 = child2.GetAttributeValue("code");
                  int result;
                  if (attributeValue3 != null && int.TryParse(attributeValue3, out result))
                    nullable = new int?(result);
                }
                Log.l("funxmpp", "get privacy settings | category: {0} error: {1}", (object) (attributeValue1 ?? "(unknown)"), nullable.HasValue ? (object) nullable.ToString() : (object) "(unknown)");
              }
              else if (attributeValue1 != null)
                settings[attributeValue1] = FunXMPP.Connection.ParsePrivacySetting(attributeValue2);
            }
          }
          this.EventHandler.OnPrivacySettings(settings);
          if (onComplete == null)
            return;
          onComplete();
        }), (Action<int>) (err =>
        {
          Log.l("funxmpp", "get privacy setting error: {0}", (object) err);
          if (onError == null)
            return;
          onError(err);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("xmlns", "privacy")
        }, new FunXMPP.ProtocolTreeNode("privacy", (FunXMPP.KeyValue[]) null)));
      }

      public void SendSetPrivacySettings(
        System.Collections.Generic.Dictionary<string, PrivacyVisibility> dict,
        Action onComplete = null,
        Action<int> onError = null)
      {
        string str = this.MakeId("setprivacy_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          bool flag = false;
          int num = 500;
          System.Collections.Generic.Dictionary<string, PrivacyVisibility> settings = new System.Collections.Generic.Dictionary<string, PrivacyVisibility>();
          FunXMPP.ProtocolTreeNode child1 = node.GetChild("privacy");
          if (child1 != null)
          {
            foreach (FunXMPP.ProtocolTreeNode protocolTreeNode in (IEnumerable<FunXMPP.ProtocolTreeNode>) ((object) child1.GetAllChildren("category") ?? (object) new FunXMPP.ProtocolTreeNode[0]))
            {
              string attributeValue1 = protocolTreeNode.GetAttributeValue("name");
              string attributeValue2 = protocolTreeNode.GetAttributeValue("value");
              if (attributeValue2 == "error")
              {
                int? nullable = new int?();
                FunXMPP.ProtocolTreeNode child2 = protocolTreeNode.GetChild("error");
                if (child2 != null)
                {
                  string attributeValue3 = child2.GetAttributeValue("code");
                  int result;
                  if (attributeValue3 != null && int.TryParse(attributeValue3, out result))
                  {
                    nullable = new int?(result);
                    num = result;
                  }
                }
                Log.l("privacy settings", "category:{0},error:{1}", (object) (attributeValue1 ?? "(unknown)"), nullable.HasValue ? (object) nullable.ToString() : (object) "(unknown)");
                flag = true;
              }
              else
                settings[attributeValue1] = FunXMPP.Connection.ParsePrivacySetting(attributeValue2);
            }
          }
          if (settings.Count != 0)
            this.EventHandler.OnPrivacySettings(settings);
          if (flag)
          {
            if (onError == null)
              return;
            onError(num);
          }
          else
          {
            if (onComplete == null)
              return;
            onComplete();
          }
        }), (Action<int>) (err =>
        {
          if (onError == null)
            return;
          onError(err);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("xmlns", "privacy")
        }, new FunXMPP.ProtocolTreeNode("privacy", (FunXMPP.KeyValue[]) null, dict.Select<KeyValuePair<string, PrivacyVisibility>, FunXMPP.ProtocolTreeNode>((Func<KeyValuePair<string, PrivacyVisibility>, FunXMPP.ProtocolTreeNode>) (kv => new FunXMPP.ProtocolTreeNode("category", new FunXMPP.KeyValue[2]
        {
          new FunXMPP.KeyValue("name", kv.Key),
          new FunXMPP.KeyValue("value", FunXMPP.Connection.PrivacySettingToString(kv.Value))
        }))).ToArray<FunXMPP.ProtocolTreeNode>())));
      }

      public void SendClearDirty(string category)
      {
        this.SendClearDirty((IEnumerable<string>) new string[1]
        {
          category
        });
      }

      public void SendClearDirty(IEnumerable<string> categoryNames)
      {
        string str = this.MakeId("clean_dirty_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) null));
        IEnumerable<FunXMPP.ProtocolTreeNode> source = categoryNames.Select<string, FunXMPP.ProtocolTreeNode>((Func<string, FunXMPP.ProtocolTreeNode>) (category => new FunXMPP.ProtocolTreeNode("clean", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("type", category)
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:dirty")
        }, source.ToArray<FunXMPP.ProtocolTreeNode>()));
      }

      public void SendGetServerProperties(Action onComplete = null, Action<int> onError = null)
      {
        string str = this.MakeId("get_server_properties_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          KeyValuePair<string, string>[] array = node.GetAllChildren("props").SelectMany<FunXMPP.ProtocolTreeNode, FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, IEnumerable<FunXMPP.ProtocolTreeNode>>) (n => n.GetAllChildren("prop"))).Select<FunXMPP.ProtocolTreeNode, KeyValuePair<string, string>>((Func<FunXMPP.ProtocolTreeNode, KeyValuePair<string, string>>) (n => new KeyValuePair<string, string>(n.GetAttributeValue("name"), n.GetAttributeValue("value")))).Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (p => p.Key != null)).ToArray<KeyValuePair<string, string>>();
          System.Collections.Generic.Dictionary<string, string> nameValueMap = new System.Collections.Generic.Dictionary<string, string>();
          foreach (KeyValuePair<string, string> keyValuePair in array)
            nameValueMap[keyValuePair.Key] = keyValuePair.Value;
          this.GroupEventHandler.OnServerProperties(nameValueMap);
          Action action = onComplete;
          if (action == null)
            return;
          action();
        }), (Action<int>) (err =>
        {
          Action<int> action = onError;
          if (action == null)
            return;
          action(err);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("xmlns", "w")
        }, new FunXMPP.ProtocolTreeNode("props", (FunXMPP.KeyValue[]) null)));
      }

      public IObservable<Backup.KeyState> SendCreateCipherKey(byte[] accountHash)
      {
        return Observable.Create<Backup.KeyState>((Func<IObserver<Backup.KeyState>, Action>) (observer =>
        {
          string str = this.MakeId("create_cipherkey_");
          this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((root, from) =>
          {
            try
            {
              FunXMPP.ProtocolTreeNode child = root.GetChild(0);
              FunXMPP.ProtocolTreeNode.Require(child, "crypto");
              string attributeValue = child.GetAttributeValue("version");
              if (attributeValue == null)
                throw new FunXMPP.CorruptStreamException("Expected version attribute");
              FunXMPP.ProtocolTreeNode protocolTreeNode3 = FunXMPP.ProtocolTreeNode.Require(child.GetChild("code"), "code");
              FunXMPP.ProtocolTreeNode protocolTreeNode4 = FunXMPP.ProtocolTreeNode.Require(child.GetChild("password"), "password");
              observer.OnNext(new Backup.KeyState()
              {
                AccountHash = accountHash,
                Salt = protocolTreeNode3.data,
                Key = protocolTreeNode4.data,
                KeyVersion = attributeValue
              });
              observer.OnCompleted();
            }
            catch (Exception ex)
            {
              observer.OnError(ex);
            }
          }), (Action<int>) (err => observer.OnError((Exception) new IqException("create cipher key", err)))));
          this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
          {
            new FunXMPP.KeyValue("id", str),
            new FunXMPP.KeyValue("type", "get"),
            new FunXMPP.KeyValue("to", "s.whatsapp.net"),
            new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:account")
          }, new FunXMPP.ProtocolTreeNode[1]
          {
            new FunXMPP.ProtocolTreeNode("crypto", new FunXMPP.KeyValue[1]
            {
              new FunXMPP.KeyValue("action", "create")
            }, new FunXMPP.ProtocolTreeNode[1]
            {
              new FunXMPP.ProtocolTreeNode("microsoft", (FunXMPP.KeyValue[]) null, accountHash)
            })
          }));
          return (Action) (() => { });
        }));
      }

      public IObservable<Backup.KeyState> SendGetCipherKey(
        string version,
        byte[] accountHash,
        byte[] saltFromFile)
      {
        return Observable.Create<Backup.KeyState>((Func<IObserver<Backup.KeyState>, Action>) (observer =>
        {
          string str = this.MakeId("get_cipherkey_");
          this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((root, from) =>
          {
            try
            {
              FunXMPP.ProtocolTreeNode child = root.GetChild(0);
              FunXMPP.ProtocolTreeNode.Require(child, "crypto");
              FunXMPP.ProtocolTreeNode protocolTreeNode = FunXMPP.ProtocolTreeNode.Require(child.GetChild("password"), "password");
              observer.OnNext(new Backup.KeyState()
              {
                AccountHash = accountHash,
                KeyVersion = version,
                Salt = saltFromFile,
                Key = protocolTreeNode.data
              });
            }
            catch (Exception ex)
            {
              observer.OnError(ex);
            }
          }), (Action<int>) (err => observer.OnError((Exception) new IqException("get cipher key", err)))));
          this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
          {
            new FunXMPP.KeyValue("id", str),
            new FunXMPP.KeyValue("type", "get"),
            new FunXMPP.KeyValue("to", "s.whatsapp.net"),
            new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:account")
          }, new FunXMPP.ProtocolTreeNode[1]
          {
            new FunXMPP.ProtocolTreeNode("crypto", new FunXMPP.KeyValue[2]
            {
              new FunXMPP.KeyValue("action", "get"),
              new FunXMPP.KeyValue(nameof (version), version)
            }, new FunXMPP.ProtocolTreeNode[2]
            {
              new FunXMPP.ProtocolTreeNode("microsoft", (FunXMPP.KeyValue[]) null, accountHash),
              new FunXMPP.ProtocolTreeNode("code", (FunXMPP.KeyValue[]) null, saltFromFile)
            })
          }));
          return (Action) (() => { });
        }));
      }

      private static T[] AppendIf<T>(T[] arr, bool func, Func<T> @new)
      {
        if (func)
          arr = ((IEnumerable<T>) arr).Concat<T>((IEnumerable<T>) new T[1]
          {
            @new()
          }).ToArray<T>();
        return arr;
      }

      private static FunXMPP.KeyValue RelayIq() => new FunXMPP.KeyValue("web", "set");

      public void SendQueryBroadcastLists(Action onSuccess = null, Action<int> onError = null)
      {
        string str = this.MakeId("query_blist_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          this.ProcessBroadcastListsQueryResults(node);
          if (onSuccess == null)
            return;
          onSuccess();
        }), onError));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[5]
        {
          new FunXMPP.KeyValue("from", Settings.MyJid),
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("xmlns", "w:b")
        }, new FunXMPP.ProtocolTreeNode("lists", (FunXMPP.KeyValue[]) null)));
      }

      public void SendDeleteBroadcastList(
        string broadcastJid,
        Action onSuccess = null,
        Action<int> onError = null)
      {
        string str = this.MakeId("del_blist_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (onSuccess == null)
            return;
          onSuccess();
        }), onError));
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("delete", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("list", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("id", broadcastJid)
        }));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[5]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("from", Settings.MyJid),
          new FunXMPP.KeyValue("xmlns", "w:b")
        }, child));
      }

      public void SendCreateGroupChat(
        string subject,
        IEnumerable<string> initialMembers,
        Action<string, GroupDescription, List<string>, List<Pair<string, int>>> onSuccess = null,
        Action<int> onError = null,
        string incomingId = null,
        GroupDescription description = null,
        GroupProperties properties = null)
      {
        string str = incomingId ?? this.MakeId("create_group_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          FunXMPP.ProtocolTreeNode child = node.GetChild(0);
          FunXMPP.ProtocolTreeNode.Require(child, "group");
          if (description != null)
          {
            int? attributeInt = (int?) child.GetChild(nameof (description))?.GetAttributeInt("error");
            if (attributeInt.HasValue)
              description.Error = new int?(attributeInt.Value);
          }
          List<string> successList = new List<string>();
          List<Pair<string, int>> failList = new List<Pair<string, int>>();
          this.ParseParticipantResults(node, "group", successList, failList);
          string attributeValue = child.GetAttributeValue("id");
          if (onSuccess == null)
            return;
          FieldStats.ReportGroupCreate();
          onSuccess(JidHelper.GroupId2Jid(attributeValue), description, successList, failList);
        }), (Action<int>) (errCode =>
        {
          if (onError == null)
            return;
          onError(errCode);
        })));
        List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList = new List<FunXMPP.ProtocolTreeNode>();
        if (description != null && !string.IsNullOrEmpty(description.Body))
          protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode(nameof (description), new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("id", description.Id)
          }, new FunXMPP.ProtocolTreeNode("body", (FunXMPP.KeyValue[]) null, description.Body)));
        if (initialMembers != null)
        {
          string[] array = initialMembers.ToArray<string>();
          if (array.Length != 0)
            protocolTreeNodeList.AddRange(((IEnumerable<string>) array).Select<string, FunXMPP.ProtocolTreeNode>((Func<string, FunXMPP.ProtocolTreeNode>) (jid => new FunXMPP.ProtocolTreeNode("participant", new FunXMPP.KeyValue[1]
            {
              new FunXMPP.KeyValue(nameof (jid), jid)
            }))));
        }
        if (properties != null && properties.IsAnnounceOnly)
          protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("announcement", (FunXMPP.KeyValue[]) null));
        if (properties != null && properties.IsRestricted)
          protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("locked", (FunXMPP.KeyValue[]) null));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", FunXMPP.Connection.AppendIf<FunXMPP.KeyValue>(new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("to", "g.us"),
          new FunXMPP.KeyValue("xmlns", "w:g2")
        }, incomingId != null, new Func<FunXMPP.KeyValue>(FunXMPP.Connection.RelayIq)), new FunXMPP.ProtocolTreeNode("create", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue(nameof (subject), subject)
        }, protocolTreeNodeList.ToArray())));
      }

      public void SendEndGroupChat(string gjid, Action onSuccess = null, Action<int> onError = null)
      {
        string str = this.MakeId("remove_group_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (onSuccess == null)
            return;
          onSuccess();
        }), onError));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("to", "g.us"),
          new FunXMPP.KeyValue("xmlns", "w:g2")
        }, new FunXMPP.ProtocolTreeNode("delete", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode[1]
        {
          new FunXMPP.ProtocolTreeNode("group", new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("id", gjid)
          })
        })));
      }

      public void SendGetGroupInfo(string gjid, Action onComplete = null)
      {
        string str = this.MakeId("get_g_info_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          FunXMPP.ProtocolTreeNode child = node.GetChild(0);
          FunXMPP.ProtocolTreeNode.Require(child, "group");
          this.GroupEventHandler.OnGroupInfo(this.ParseGroupInfo(child));
          if (onComplete == null)
            return;
          onComplete();
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("to", gjid),
          new FunXMPP.KeyValue("xmlns", "w:g2")
        }, new FunXMPP.ProtocolTreeNode("query", (FunXMPP.KeyValue[]) null)));
      }

      public void SendGetGroupDescription(string gjid, Action onComplete = null)
      {
        string str = this.MakeId("get_g_desc_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          FunXMPP.ProtocolTreeNode child = node.GetChild(0);
          FunXMPP.ProtocolTreeNode.Require(child, "group");
          this.GroupEventHandler.OnGroupInfo(this.ParseGroupInfo(child), false);
          if (onComplete == null)
            return;
          onComplete();
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("to", gjid),
          new FunXMPP.KeyValue("xmlns", "w:g2")
        }, new FunXMPP.ProtocolTreeNode("query", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("request", "description")
        })));
      }

      public void SendGetGroups(Action onSuccess = null, Action<int> onError = null)
      {
        string str = this.MakeId("get_groups_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          FunXMPP.ProtocolTreeNode child = node.GetChild("groups");
          if (child != null)
            this.GroupEventHandler.OnParticipatingGroups(child.GetAllChildren("group").Select<FunXMPP.ProtocolTreeNode, FunXMPP.Connection.GroupInfo>((Func<FunXMPP.ProtocolTreeNode, FunXMPP.Connection.GroupInfo>) (groupNode => this.ParseGroupInfo(groupNode))).ToArray<FunXMPP.Connection.GroupInfo>());
          if (onSuccess == null)
            return;
          onSuccess();
        }), onError));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("to", "g.us"),
          new FunXMPP.KeyValue("xmlns", "w:g2")
        }, new FunXMPP.ProtocolTreeNode[1]
        {
          new FunXMPP.ProtocolTreeNode("participating", (FunXMPP.KeyValue[]) null)
        }));
      }

      public void SendSetGroupSubject(
        string gjid,
        string subject,
        Action onComplete,
        Action<int> onError,
        string incomingId = null)
      {
        string str = incomingId ?? this.MakeId("set_group_subject_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (onComplete == null)
            return;
          onComplete();
        }), (Action<int>) (errCode =>
        {
          string errMsg;
          switch (errCode)
          {
            case 401:
              errMsg = AppResources.GroupInfoNotAnAdmin;
              break;
            case 403:
              errMsg = AppResources.SubjectChangedFailNotAParticipant;
              break;
            case 406:
              errMsg = string.Format(AppResources.SubjectChangedFailTooLong, (object) subject);
              break;
            default:
              errMsg = AppResources.SubjectChangedFail;
              break;
          }
          AppState.ClientInstance.ShowErrorMessage(errMsg, false);
          if (onError == null)
            return;
          onError(errCode);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", FunXMPP.Connection.AppendIf<FunXMPP.KeyValue>(new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("to", gjid),
          new FunXMPP.KeyValue("xmlns", "w:g2")
        }, incomingId != null, new Func<FunXMPP.KeyValue>(FunXMPP.Connection.RelayIq)), new FunXMPP.ProtocolTreeNode(nameof (subject), (FunXMPP.KeyValue[]) null, subject)));
      }

      public void SendSetGroupDescription(
        string gjid,
        GroupDescription description,
        Action onComplete,
        Action<int> onError,
        string incomingId = null)
      {
        string str = incomingId ?? this.MakeId("set_group_description_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          Action action = onComplete;
          if (action == null)
            return;
          action();
        }), (Action<int>) (errCode =>
        {
          string errMsg;
          switch (errCode)
          {
            case 401:
              errMsg = AppResources.GroupInfoNotAnAdmin;
              break;
            case 406:
              errMsg = AppResources.GroupDescriptionChangedFailTooLong;
              break;
            case 409:
              this.GroupEventHandler.OnGroupDescriptionMismatch(gjid);
              errMsg = AppResources.GroupDescriptionFail;
              break;
            default:
              errMsg = AppResources.GroupDescriptionFail;
              break;
          }
          AppState.ClientInstance.ShowErrorMessage(errMsg, false);
          Action<int> action = onError;
          if (action == null)
            return;
          action(errCode);
        })));
        FunXMPP.ProtocolTreeNode child1 = new FunXMPP.ProtocolTreeNode("body", (FunXMPP.KeyValue[]) null, description.Body);
        FunXMPP.ProtocolTreeNode child2 = new FunXMPP.ProtocolTreeNode(nameof (description), FunXMPP.Connection.AppendIf<FunXMPP.KeyValue>(new FunXMPP.KeyValue[2]
        {
          new FunXMPP.KeyValue("id", description.Id),
          new FunXMPP.KeyValue("prev", description.PreviousId)
        }, description.Body == "", (Func<FunXMPP.KeyValue>) (() => new FunXMPP.KeyValue("delete", "true"))), child1);
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", FunXMPP.Connection.AppendIf<FunXMPP.KeyValue>(new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("to", gjid),
          new FunXMPP.KeyValue("xmlns", "w:g2")
        }, incomingId != null, new Func<FunXMPP.KeyValue>(FunXMPP.Connection.RelayIq)), child2));
      }

      public void SendSetGroupRestrict(
        string gjid,
        bool restrict,
        Action onComplete = null,
        Action<int> onError = null,
        string incomingId = null)
      {
        string str = incomingId ?? this.MakeId("set_group_restriction_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          Action action = onComplete;
          if (action == null)
            return;
          action();
        }), (Action<int>) (errCode =>
        {
          string errMsg = errCode != 401 ? AppResources.UpdateGroupInfoFail : AppResources.GroupInfoNotAnAdmin;
          AppState.ClientInstance.ShowErrorMessage(errMsg, false);
          Action<int> action = onError;
          if (action == null)
            return;
          action(errCode);
        })));
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode(restrict ? "locked" : "unlocked", (FunXMPP.KeyValue[]) null);
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", FunXMPP.Connection.AppendIf<FunXMPP.KeyValue>(new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("to", gjid),
          new FunXMPP.KeyValue("xmlns", "w:g2")
        }, incomingId != null, new Func<FunXMPP.KeyValue>(FunXMPP.Connection.RelayIq)), child));
      }

      public void SendSetAnnouncementOnlyGroup(
        string gjid,
        bool announcementOnly,
        Action onComplete = null,
        Action<int> onError = null,
        string incomingId = null)
      {
        string str = incomingId ?? this.MakeId("set_announcement_only_group_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          Action action = onComplete;
          if (action == null)
            return;
          action();
        }), (Action<int>) (errCode =>
        {
          string errMsg = errCode != 401 ? AppResources.UpdateGroupInfoFail : AppResources.GroupInfoNotAnAdmin;
          AppState.ClientInstance.ShowErrorMessage(errMsg, false);
          Action<int> action = onError;
          if (action == null)
            return;
          action(errCode);
        })));
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode(announcementOnly ? "announcement" : "not_announcement", (FunXMPP.KeyValue[]) null);
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", FunXMPP.Connection.AppendIf<FunXMPP.KeyValue>(new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("to", gjid),
          new FunXMPP.KeyValue("xmlns", "w:g2")
        }, incomingId != null, new Func<FunXMPP.KeyValue>(FunXMPP.Connection.RelayIq)), child));
      }

      public void SendAddParticipants(
        string gjid,
        IEnumerable<string> participants,
        Action<List<Pair<string, int>>> onComplete = null,
        Action<int> onError = null,
        string incomingId = null)
      {
        string id = incomingId ?? this.MakeId("add_group_participants_");
        this.AddIqHandler(id, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          List<string> stringList = new List<string>();
          List<Pair<string, int>> failList = new List<Pair<string, int>>();
          this.ParseParticipantResults(node, "add", stringList, failList);
          string attributeValue = node.GetAttributeValue("reason");
          this.GroupEventHandler.OnAddGroupParticipants(from, stringList, failList, new DateTime?(), attributeValue);
          if (onComplete == null)
            return;
          onComplete(failList);
        }), onError));
        this.SendVerbParticipants(gjid, participants, id, "add", incomingId != null);
      }

      public void SendRemoveParticipants(
        string gjid,
        IEnumerable<string> participants,
        Action<List<Pair<string, int>>> onComplete,
        Action<int> onError,
        string incomingId = null)
      {
        string id = incomingId ?? this.MakeId("remove_group_participants_");
        this.AddIqHandler(id, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          List<string> successList = new List<string>();
          List<Pair<string, int>> failList = new List<Pair<string, int>>();
          this.ParseParticipantResults(node, "remove", successList, failList);
          this.GroupEventHandler.OnRemoveGroupParticipants(from, successList, failList);
          if (onComplete == null)
            return;
          onComplete(failList);
        }), onError));
        this.SendVerbParticipants(gjid, participants, id, "remove", incomingId != null);
      }

      public void SendPromoteParticipant(
        string gjid,
        IEnumerable<string> participants,
        Action<List<Pair<string, int>>> onComplete,
        Action<int> onError,
        string incomingId = null)
      {
        string id = incomingId ?? this.MakeId("promote_group_participants_");
        this.AddIqHandler(id, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          List<string> stringList = new List<string>();
          List<Pair<string, int>> failList = new List<Pair<string, int>>();
          this.ParseParticipantResults(node, "promote", stringList, failList);
          this.GroupEventHandler.OnPromoteUsers(from, (IEnumerable<string>) stringList, new DateTime?(DateTime.UtcNow));
          if (onComplete == null)
            return;
          onComplete(failList);
        }), onError));
        this.SendVerbParticipants(gjid, participants, id, "promote", incomingId != null);
      }

      public void SendDemoteParticipant(
        string gjid,
        IEnumerable<string> participants,
        Action<List<Pair<string, int>>> onComplete,
        Action<int> onError,
        string incomingId = null)
      {
        string id = incomingId ?? this.MakeId("demote_group_participants_");
        this.AddIqHandler(id, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          List<string> stringList = new List<string>();
          List<Pair<string, int>> failList = new List<Pair<string, int>>();
          this.ParseParticipantResults(node, "demote", stringList, failList);
          this.GroupEventHandler.OnDemoteUsers(from, (IEnumerable<string>) stringList, new DateTime?(DateTime.UtcNow));
          if (onComplete == null)
            return;
          onComplete(failList);
        }), onError));
        this.SendVerbParticipants(gjid, participants, id, "demote", incomingId != null);
      }

      public IObservable<Unit> SendLeaveGroup(string gjid, string incomingId = null, bool delete = false)
      {
        return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
        {
          string str = incomingId ?? this.MakeId("leave_group_");
          this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
          {
            FunXMPP.ProtocolTreeNode child = node.GetChild("leave");
            if (child != null)
            {
              foreach (string from2 in child.GetAllChildren("group").Select<FunXMPP.ProtocolTreeNode, string>((Func<FunXMPP.ProtocolTreeNode, string>) (group => group.GetAttributeValue("id"))))
                this.GroupEventHandler.OnLeaveGroup(from2, delete: delete);
            }
            observer.OnNext(new Unit());
            observer.OnCompleted();
          }), (Action<int>) (errCode =>
          {
            this.GroupEventHandler.OnLeaveGroupFail(gjid);
            observer.OnError(new Exception(errCode.ToString()));
            observer.OnCompleted();
          })));
          this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", FunXMPP.Connection.AppendIf<FunXMPP.KeyValue>(new FunXMPP.KeyValue[4]
          {
            new FunXMPP.KeyValue("id", str),
            new FunXMPP.KeyValue("type", "set"),
            new FunXMPP.KeyValue("to", "g.us"),
            new FunXMPP.KeyValue("xmlns", "w:g2")
          }, incomingId != null, new Func<FunXMPP.KeyValue>(FunXMPP.Connection.RelayIq)), new FunXMPP.ProtocolTreeNode("leave", (FunXMPP.KeyValue[]) null, ((IEnumerable<string>) new string[1]
          {
            gjid
          }).Select<string, FunXMPP.ProtocolTreeNode>((Func<string, FunXMPP.ProtocolTreeNode>) (jid => new FunXMPP.ProtocolTreeNode("group", new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("id", jid)
          }))).ToArray<FunXMPP.ProtocolTreeNode>())));
          return (Action) (() => { });
        }));
      }

      internal void SendVerbParticipants(
        string gjid,
        IEnumerable<string> participants,
        string id,
        string verb,
        bool relay = false)
      {
        IEnumerable<FunXMPP.ProtocolTreeNode> source = participants.Select<string, FunXMPP.ProtocolTreeNode>((Func<string, FunXMPP.ProtocolTreeNode>) (jid => new FunXMPP.ProtocolTreeNode("participant", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue(nameof (jid), jid)
        })));
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode(verb, (FunXMPP.KeyValue[]) null, source.ToArray<FunXMPP.ProtocolTreeNode>());
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", FunXMPP.Connection.AppendIf<FunXMPP.KeyValue>(new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue(nameof (id), id),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("to", gjid),
          new FunXMPP.KeyValue("xmlns", "w:g2")
        }, relay, new Func<FunXMPP.KeyValue>(FunXMPP.Connection.RelayIq)), child));
      }

      public void SendGetPhoto(
        string jid,
        string expectedPhotoId,
        bool largeFormat,
        Action onComplete = null,
        Action<int> onError = null)
      {
        this.SendGetPhoto(jid, expectedPhotoId, largeFormat, true, onComplete, onError);
      }

      public void SendGetPhoto(
        string jid,
        string expectedPhotoId,
        bool largeFormat,
        bool useEverstore,
        Action onComplete = null,
        Action<int> onError = null)
      {
        if (Voip.IsInCall)
        {
          Log.l("iq", "get_photo_ | skip during voip call | jid:{0}", (object) jid);
        }
        else
        {
          string id = this.MakeId("get_photo_");
          Log.l("Iq", "get_photo_ | id: {0}, jid: {1}, expected pid: {2}, get large: {3}", (object) id, (object) jid, (object) expectedPhotoId, (object) largeFormat);
          this.AddIqHandler(id, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
          {
            Log.l("IqHandler", "get_photo_ | id: {0}, jid: {1}, get large: {2}", (object) id, (object) jid, (object) largeFormat);
            if (StringComparer.Ordinal.Equals(node.GetAttributeValue("type"), "result") && this.EventHandler != null)
            {
              foreach (FunXMPP.ProtocolTreeNode allChild in node.GetAllChildren("picture"))
              {
                string photoId = allChild.GetAttributeValue("id");
                string photoUrl = allChild.GetAttributeValue("url");
                if (photoId != null && photoUrl != null)
                {
                  NativeWeb.SimpleGet(photoUrl).Subscribe<Stream>((Action<Stream>) (stream =>
                  {
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    this.EventHandler.OnPhotoChanged(jid, photoId, largeFormat ? (byte[]) null : buffer, largeFormat ? buffer : (byte[]) null, "get_photo_");
                    this.ResetEverstoreBackoff();
                    Action action = onComplete;
                    if (action == null)
                      return;
                    action();
                  }), (Action<Exception>) (ex =>
                  {
                    Log.l("IqHandler", "get_photo_ | error downloading | exception: {3} | id: {0}, jid: {1}, url: {2}", (object) id, (object) jid, (object) photoUrl, (object) ex.Message);
                    this.IncrementEverstoreBackoff();
                    this.SendGetPhoto(jid, expectedPhotoId, largeFormat, false, onComplete, onError);
                  }));
                  return;
                }
                if (photoId != null && allChild.data != null && allChild.data.Length != 0)
                  this.EventHandler.OnPhotoChanged(jid, photoId, largeFormat ? (byte[]) null : allChild.data, largeFormat ? allChild.data : (byte[]) null, "get_photo_");
                else
                  Log.l("IqHandler", "get_photo_ | bad response | id: {0}, jid: {1}", (object) id, (object) jid);
              }
            }
            if (onComplete == null)
              return;
            onComplete();
          }), onError));
          List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>();
          if (!largeFormat)
            keyValueList.Add(new FunXMPP.KeyValue("type", "preview"));
          else if (useEverstore)
          {
            if (Settings.EverstoreBackoffUtc.HasValue)
            {
              DateTime? everstoreBackoffUtc = Settings.EverstoreBackoffUtc;
              DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
              if ((everstoreBackoffUtc.HasValue ? (everstoreBackoffUtc.GetValueOrDefault() < currentServerTimeUtc ? 1 : 0) : 0) == 0)
                goto label_8;
            }
            keyValueList.Add(new FunXMPP.KeyValue("query", "url"));
          }
label_8:
          if (expectedPhotoId != null)
            keyValueList.Add(new FunXMPP.KeyValue("id", expectedPhotoId));
          FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("picture", keyValueList.ToArray());
          this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
          {
            new FunXMPP.KeyValue("id", id),
            new FunXMPP.KeyValue("type", "get"),
            new FunXMPP.KeyValue("to", jid),
            new FunXMPP.KeyValue("xmlns", "w:profile:picture")
          }, child));
        }
      }

      public void SendGdprRequestReport(Action onSuccess, Action onError)
      {
        string str = this.MakeId("gdpr_req_report_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          Log.l("gdpr", "request report iq success");
          bool flag = false;
          FunXMPP.ProtocolTreeNode child = node.GetChild("gdpr");
          if (child != null)
          {
            long? attributeLong = child.GetAttributeLong("timestamp");
            if (attributeLong.HasValue)
            {
              DateTime readyTime = DateTimeUtils.FromUnixTime(attributeLong.Value);
              Log.l("gdpr", "request report | ready date:{0} utc", (object) readyTime.ToLongDateString());
              GdprReport.SetStateRequestSent(readyTime);
              flag = true;
              Action action = onSuccess;
              if (action != null)
                action();
            }
          }
          if (flag)
            return;
          Log.l("gdpr", "request report iq success | but no valid data");
          Action action1 = onError;
          if (action1 == null)
            return;
          action1();
        }), (Action<int>) (errCode =>
        {
          Log.l("gdpr", "request report iq failed | error:{0}", (object) errCode);
          Action action = onError;
          if (action == null)
            return;
          action();
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:account"),
          new FunXMPP.KeyValue("to", "s.whatsapp.net")
        }, new FunXMPP.ProtocolTreeNode("gdpr", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("action", "request")
        })));
      }

      public void SendGdprCheckReportStatus(Action onComplete, Action onError)
      {
        string str = this.MakeId("gdpr_check_report_status_");
        Action process404 = (Action) (() =>
        {
          GdprReport.States gdprReportState = Settings.GdprReportState;
          Log.l("gdpr", "report not found | curr state:{0}", (object) gdprReportState);
          switch (gdprReportState)
          {
            case GdprReport.States.RequestSent:
            case GdprReport.States.Ready:
            case GdprReport.States.Downloading:
              Log.l("gdpr", "reset to init");
              GdprReport.SetStateInit();
              break;
          }
        });
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          Log.l("gdpr", "check status iq success");
          bool flag = false;
          FunXMPP.ProtocolTreeNode child1 = node.GetChild("gdpr");
          if (child1 == null)
          {
            FunXMPP.ProtocolTreeNode child2 = node.GetChild("error");
            if (child2 != null)
            {
              int? attributeInt = child2.GetAttributeInt("code");
              Log.l("gdpr", "check status error:{0}", (object) (attributeInt ?? -1));
              if (attributeInt.HasValue && attributeInt.Value == 404)
              {
                process404();
                flag = true;
              }
            }
          }
          else
          {
            FunXMPP.ProtocolTreeNode child3 = child1.GetChild("document");
            if (child3 == null)
            {
              long? attributeLong = child1.GetAttributeLong("timestamp");
              if (attributeLong.HasValue)
              {
                DateTime readyTime = DateTimeUtils.FromUnixTime(attributeLong.Value);
                Log.l("gdpr", "check status | found report pending | ready date:{0} utc", (object) readyTime.ToLongDateString());
                GdprReport.SetStateRequestSent(readyTime);
                flag = true;
              }
            }
            else
            {
              long? attributeLong1 = child3.GetAttributeLong("creation");
              long? attributeLong2 = child3.GetAttributeLong("expiration");
              FunXMPP.Listener eventHandler = this.EventHandler;
              flag = eventHandler != null && eventHandler.OnGdprReportReady(attributeLong1, attributeLong2, child3.data, false, (Action) null);
            }
          }
          if (!flag)
            Log.l("gdpr", "check status iq success | but not really handled");
          Action action = onComplete;
          if (action == null)
            return;
          action();
        }), (Action<int>) (errCode =>
        {
          Log.l("gdpr", "check status failed | error:{0}", (object) errCode);
          if (errCode == 404)
            process404();
          Action action1 = onError;
          if (action1 != null)
            action1();
          Action action2 = onComplete;
          if (action2 == null)
            return;
          action2();
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:account"),
          new FunXMPP.KeyValue("to", "s.whatsapp.net")
        }, new FunXMPP.ProtocolTreeNode("gdpr", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("action", "status")
        })));
      }

      public void SendGdprDeleteReport(Action onComplete, Action onError)
      {
        string str = this.MakeId("gdpr_del_report_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          Log.l("gdpr", "delete report success");
          GdprReport.SetStateInit();
          Action action = onComplete;
          if (action == null)
            return;
          action();
        }), (Action<int>) (errCode =>
        {
          Log.l("gdpr", "delete report failed | error:{0}", (object) errCode);
          Action action = onError;
          if (action == null)
            return;
          action();
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:account"),
          new FunXMPP.KeyValue("to", "s.whatsapp.net")
        }, new FunXMPP.ProtocolTreeNode("gdpr", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("action", "delete")
        })));
      }

      public void ResetEverstoreBackoff() => Settings.EverstoreBackoffAttempt = 0;

      public void IncrementEverstoreBackoff()
      {
        uint num = Utils.CalculateFibonacci(Settings.EverstoreBackoffAttempt);
        if (num > 604800U)
          num = 604800U;
        else
          ++Settings.EverstoreBackoffAttempt;
        Settings.EverstoreBackoffUtc = new DateTime?(FunRunner.CurrentServerTimeUtc.AddSeconds((double) num));
      }

      public void SendSetPhoto(
        string jid,
        byte[] bytes,
        byte[] thumbnailBytes,
        Action onSuccess = null,
        Action<int> onError = null,
        string incomingId = null,
        bool showSystemMessage = true)
      {
        string str = incomingId ?? this.MakeId("set_photo_");
        if (onSuccess == null)
          onSuccess = (Action) (() => { });
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (this.EventHandler != null)
          {
            string photoId = (string) null;
            FunXMPP.ProtocolTreeNode child = node.GetChild("picture");
            if (child != null)
              photoId = child.GetAttributeValue("id");
            this.EventHandler.OnSelfSetNewPhoto(jid, photoId, showSystemMessage, thumbnailBytes, bytes, "set_photo_");
          }
          onSuccess();
        }), (Action<int>) (errCode =>
        {
          if (errCode == 406 && !FunXMPP.Connection.ThrottleClbUpload)
          {
            FunXMPP.Connection.ThrottleClbUpload = true;
            Log.d("ProfilePhoto", "406 from profile photo update", bytes, 800);
            Log.SendCrashLog(new Exception("Profile Photo 406"), "Profile Photo 406", logOnlyForRelease: true);
          }
          if (onError == null)
            return;
          onError(errCode);
        })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", FunXMPP.Connection.AppendIf<FunXMPP.KeyValue>(new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("to", jid),
          new FunXMPP.KeyValue("xmlns", "w:profile:picture")
        }, incomingId != null, new Func<FunXMPP.KeyValue>(FunXMPP.Connection.RelayIq)), new List<FunXMPP.ProtocolTreeNode>()
        {
          new FunXMPP.ProtocolTreeNode("picture", (FunXMPP.KeyValue[]) null, bytes)
        }.ToArray()));
      }

      public IObservable<string> SendNormalizePhoneNumber(string cc, string phone)
      {
        return Observable.Create<string>((Func<IObserver<string>, Action>) (observer =>
        {
          string str1 = this.MakeId("normalize_");
          this.AddIqHandler(str1, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
          {
            string str2 = (string) null;
            FunXMPP.ProtocolTreeNode child = node.GetChild("normalize");
            if (child != null)
              str2 = child.GetAttributeValue("result");
            if (str2 != null)
              observer.OnNext(str2);
            else
              observer.OnError(new Exception("normalization - unexpected response"));
          }), (Action<int>) (err => observer.OnError(new Exception("normalization returned " + (object) err)))));
          this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
          {
            new FunXMPP.KeyValue("to", "s.whatsapp.net"),
            new FunXMPP.KeyValue("type", "get"),
            new FunXMPP.KeyValue("id", str1),
            new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:account")
          }, new FunXMPP.ProtocolTreeNode("normalize", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode[2]
          {
            new FunXMPP.ProtocolTreeNode(nameof (cc), (FunXMPP.KeyValue[]) null, cc),
            new FunXMPP.ProtocolTreeNode("in", (FunXMPP.KeyValue[]) null, phone)
          })));
          return (Action) (() => { });
        })).Take<string>(1);
      }

      public IObservable<Unit> SendChangeNumber(string oldChatId, IEnumerable<string> notifyJids)
      {
        return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
        {
          string str = this.MakeId("mod_acct_");
          this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) => observer.OnNext(new Unit())), (Action<int>) (err => observer.OnError(new Exception(err.ToString())))));
          List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList = new List<FunXMPP.ProtocolTreeNode>();
          protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("username", (FunXMPP.KeyValue[]) null, oldChatId));
          bool flag = false;
          if (notifyJids != null && notifyJids.Any<string>())
          {
            flag = true;
            FunXMPP.ProtocolTreeNode[] array = notifyJids.Select<string, FunXMPP.ProtocolTreeNode>((Func<string, FunXMPP.ProtocolTreeNode>) (jid => new FunXMPP.ProtocolTreeNode("user", new FunXMPP.KeyValue[1]
            {
              new FunXMPP.KeyValue(nameof (jid), jid)
            }))).ToArray<FunXMPP.ProtocolTreeNode>();
            protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("notify", (FunXMPP.KeyValue[]) null, array));
          }
          FunXMPP.KeyValue[] attrs;
          if (!flag)
            attrs = (FunXMPP.KeyValue[]) null;
          else
            attrs = new FunXMPP.KeyValue[1]
            {
              new FunXMPP.KeyValue("notify", "true")
            };
          FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("modify", attrs, protocolTreeNodeList.ToArray());
          this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
          {
            new FunXMPP.KeyValue("to", "s.whatsapp.net"),
            new FunXMPP.KeyValue("type", "get"),
            new FunXMPP.KeyValue("id", str),
            new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:account")
          }, child));
          return (Action) (() => { });
        })).Take<Unit>(1);
      }

      public void SendDeleteAccount(
        string feedback,
        string language,
        string locale,
        Action onSuccess,
        Action<int> onError)
      {
        string str = this.MakeId("del_acct_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) => onSuccess()), onError));
        FunXMPP.ProtocolTreeNode[] children = (FunXMPP.ProtocolTreeNode[]) null;
        if (!string.IsNullOrEmpty(feedback))
          children = new FunXMPP.ProtocolTreeNode[1]
          {
            new FunXMPP.ProtocolTreeNode("body", new FunXMPP.KeyValue[2]
            {
              new FunXMPP.KeyValue("lc", locale),
              new FunXMPP.KeyValue("lg", language)
            }, feedback)
          };
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("xmlns", "urn:xmpp:whatsapp:account")
        }, new FunXMPP.ProtocolTreeNode("remove", (FunXMPP.KeyValue[]) null, children)));
      }

      public IObservable<FunXMPP.Connection.UploadResult> SendUploadRequest(
        byte[] hash,
        long? size,
        FunXMPP.FMessage.FunMediaType type,
        byte[] origHash,
        WhatsApp.Events.MediaUpload fsEvent = null,
        bool streaming = false)
      {
        return Observable.Create<FunXMPP.Connection.UploadResult>((Func<IObserver<FunXMPP.Connection.UploadResult>, Action>) (observer =>
        {
          if (streaming)
            size = new long?(4096L);
          if (hash == null && origHash == null)
          {
            observer.OnError((Exception) new FunXMPP.Connection.UnforwardableMessageException());
            return (Action) (() => { });
          }
          string str = this.MakeId("upload_");
          Action<Action> complete = (Action<Action>) (a =>
          {
            try
            {
              a();
            }
            finally
            {
              observer.OnCompleted();
            }
          });
          this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) => complete((Action) (() =>
          {
            FunXMPP.Connection.UploadResult uploadResult = new FunXMPP.Connection.UploadResult();
            FunXMPP.ProtocolTreeNode child3 = node.GetChild("encr_media");
            if (child3 != null)
            {
              long.TryParse(child3.GetAttributeValue("resume") ?? "-1", out uploadResult.ResumeFrom);
              uploadResult.UploadUrl = child3.GetAttributeValue("url");
              if (uploadResult.UploadUrl != null)
              {
                observer.OnNext(uploadResult);
                return;
              }
            }
            FunXMPP.ProtocolTreeNode child4 = node.GetChild("duplicate");
            if (child4 != null)
            {
              FieldStats.SetResultInUploadEvent(fsEvent, wam_enum_media_upload_result_type.DUPLICATE);
              uploadResult.DownloadUrl = child4.GetAttributeValue("url");
              uploadResult.MimeType = child4.GetAttributeValue("mimetype");
              uploadResult.FileSize = long.Parse(child4.GetAttributeValue(nameof (size)));
              uploadResult.Hash = Convert.FromBase64String(child4.GetAttributeValue("filehash"));
              string attributeValue = child4.GetAttributeValue("duration");
              int result;
              if (!string.IsNullOrEmpty(attributeValue) && int.TryParse(attributeValue, out result))
                uploadResult.DurationSeconds = new int?(result);
              if (uploadResult.DownloadUrl != null)
              {
                observer.OnNext(uploadResult);
                return;
              }
            }
            FieldStats.SetResultInUploadEvent(fsEvent, wam_enum_media_upload_result_type.ERROR_REQUEST);
            observer.OnError(new Exception("upload - Unexpected response"));
          }))), (Action<int>) (err => complete((Action) (() =>
          {
            FieldStats.SetResultInUploadEvent(fsEvent, wam_enum_media_upload_result_type.ERROR_REQUEST);
            observer.OnError(new Exception("upload request returned " + (object) err));
          })))));
          string funMediaTypeStr = FunXMPP.FMessage.GetFunMediaTypeStr(type);
          if (funMediaTypeStr == null)
          {
            observer.OnError(new Exception("unsupported media type"));
            return (Action) (() => { });
          }
          List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>()
          {
            new FunXMPP.KeyValue(nameof (type), funMediaTypeStr)
          };
          if (hash != null)
            keyValueList.Add(new FunXMPP.KeyValue(nameof (hash), Convert.ToBase64String(hash, 0, hash.Length)));
          if (size.HasValue)
            keyValueList.Add(new FunXMPP.KeyValue(nameof (size), size.Value.ToString()));
          if (origHash != null)
            keyValueList.Add(new FunXMPP.KeyValue("orighash", Convert.ToBase64String(origHash, 0, origHash.Length)));
          this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
          {
            new FunXMPP.KeyValue("id", str),
            new FunXMPP.KeyValue(nameof (type), "set"),
            new FunXMPP.KeyValue("to", "s.whatsapp.net"),
            new FunXMPP.KeyValue("xmlns", "w:m")
          }, new FunXMPP.ProtocolTreeNode("encr_media", keyValueList.ToArray(), (byte[]) null)));
          return (Action) (() => { });
        }));
      }

      public IObservable<Unit> SendAckMedia(string url, bool webrequest)
      {
        return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
        {
          if (!webrequest)
          {
            string str = this.MakeId("ackmedia_");
            Action<Action> complete = (Action<Action>) (a =>
            {
              try
              {
                a();
              }
              finally
              {
                observer.OnCompleted();
              }
            });
            this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) => complete((Action) (() => observer.OnNext(new Unit())))), (Action<int>) (err => complete((Action) (() => observer.OnError(new Exception("ack media returned " + (object) err)))))));
            this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
            {
              new FunXMPP.KeyValue("id", str),
              new FunXMPP.KeyValue("type", "set"),
              new FunXMPP.KeyValue("to", "s.whatsapp.net"),
              new FunXMPP.KeyValue("xmlns", "w:m")
            }, new FunXMPP.ProtocolTreeNode("ack", new FunXMPP.KeyValue[1]
            {
              new FunXMPP.KeyValue(nameof (url), url)
            }, (byte[]) null)));
          }
          else
            NativeWeb.EmptyPost(url).Subscribe<int>((Action<int>) (code =>
            {
              if (code != 200)
                return;
              observer.OnNext(new Unit());
            }), (Action<Exception>) (ex => Log.l(ex, "Exception attempting ack")));
          return (Action) (() => { });
        }));
      }

      public IObservable<Unit> SendSetRecoveryToken(byte[] bytes)
      {
        return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
        {
          string str = this.MakeId("settoken_");
          Action<Action> complete = (Action<Action>) (a =>
          {
            try
            {
              a();
            }
            finally
            {
              observer.OnCompleted();
            }
          });
          this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) => complete((Action) (() => observer.OnNext(new Unit())))), (Action<int>) (err => complete((Action) (() => observer.OnError(new Exception("set recovery token returned " + (object) err)))))));
          this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
          {
            new FunXMPP.KeyValue("id", str),
            new FunXMPP.KeyValue("type", "set"),
            new FunXMPP.KeyValue("to", "s.whatsapp.net"),
            new FunXMPP.KeyValue("xmlns", "w:ch:p")
          }, new FunXMPP.ProtocolTreeNode("pin", (FunXMPP.KeyValue[]) null, bytes)));
          return (Action) (() => { });
        }));
      }

      public void SendSetChatStaticPublicKey(byte[] staticPublic, Action onComplete)
      {
        string str = this.MakeId("settoken_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) => onComplete()), (Action<int>) (err => { })));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("xmlns", "w:auth:key")
        }, new FunXMPP.ProtocolTreeNode("key", (FunXMPP.KeyValue[]) null, staticPublic)));
      }

      public void SendAvailableForChat(bool force)
      {
        if (AppState.IsBackgroundAgent && !force)
          return;
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("presence", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("name", Settings.PushName ?? "")
        }));
      }

      public static byte[] UIntToBytes(uint from)
      {
        return new byte[4]
        {
          (byte) (from >> 24 & (uint) byte.MaxValue),
          (byte) (from >> 16 & (uint) byte.MaxValue),
          (byte) (from >> 8 & (uint) byte.MaxValue),
          (byte) (from & (uint) byte.MaxValue)
        };
      }

      public static uint UIntFromBytes(byte[] from)
      {
        if (from.Length < 4)
          throw new ArgumentOutOfRangeException();
        return (uint) ((int) from[0] << 24 | (int) from[1] << 16 | (int) from[2] << 8) | (uint) from[3];
      }

      public void SendEncryptionSetPreKey(
        byte[] identityKey,
        uint registrationId,
        byte typeByte,
        AxolotlPreKey[] unsentPreKeys,
        AxolotlPreKey latestSignedPreKey,
        Action onComplete,
        Action<int> onError)
      {
        this.InvokeWhenConnected((Action) (() =>
        {
          string str = this.MakeId("setprekey_");
          this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
          {
            if (onComplete == null)
              return;
            onComplete();
          }), (Action<int>) (err =>
          {
            if (onError == null)
              return;
            onError(err);
          })));
          byte[] data = new byte[1]{ typeByte };
          List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList = new List<FunXMPP.ProtocolTreeNode>();
          foreach (AxolotlPreKey unsentPreKey in unsentPreKeys)
          {
            FunXMPP.ProtocolTreeNode protocolTreeNode = new FunXMPP.ProtocolTreeNode("key", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode[2]
            {
              new FunXMPP.ProtocolTreeNode("id", (FunXMPP.KeyValue[]) null, unsentPreKey.Id),
              new FunXMPP.ProtocolTreeNode("value", (FunXMPP.KeyValue[]) null, unsentPreKey.Data)
            });
            protocolTreeNodeList.Add(protocolTreeNode);
          }
          this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
          {
            new FunXMPP.KeyValue("xmlns", "encrypt"),
            new FunXMPP.KeyValue("type", "set"),
            new FunXMPP.KeyValue("to", "s.whatsapp.net"),
            new FunXMPP.KeyValue("id", str)
          }, new FunXMPP.ProtocolTreeNode[5]
          {
            new FunXMPP.ProtocolTreeNode("registration", (FunXMPP.KeyValue[]) null, FunXMPP.Connection.UIntToBytes(registrationId)),
            new FunXMPP.ProtocolTreeNode("type", (FunXMPP.KeyValue[]) null, data),
            new FunXMPP.ProtocolTreeNode("identity", (FunXMPP.KeyValue[]) null, identityKey),
            new FunXMPP.ProtocolTreeNode("list", (FunXMPP.KeyValue[]) null, protocolTreeNodeList.ToArray()),
            new FunXMPP.ProtocolTreeNode("skey", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode[3]
            {
              new FunXMPP.ProtocolTreeNode("id", (FunXMPP.KeyValue[]) null, latestSignedPreKey.Id),
              new FunXMPP.ProtocolTreeNode("value", (FunXMPP.KeyValue[]) null, latestSignedPreKey.Data),
              new FunXMPP.ProtocolTreeNode("signature", (FunXMPP.KeyValue[]) null, latestSignedPreKey.Signature)
            })
          }));
        }));
      }

      public void SendEncryptionRotateSignedPreKey(
        AxolotlPreKey latestSignedPreKey,
        Action onComplete,
        Action<int?, byte[]> onError)
      {
        this.InvokeWhenConnected((Action) (() =>
        {
          string str = this.MakeId("setsignedprekey_");
          this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
          {
            if (onComplete == null)
              return;
            onComplete();
          }), (Action<FunXMPP.ProtocolTreeNode>) (node =>
          {
            if (onError == null)
              return;
            byte[] numArray = (byte[]) null;
            FunXMPP.ProtocolTreeNode child3 = node.GetChild("error");
            if (child3 == null)
              return;
            int? attributeInt = child3.GetAttributeInt("code");
            FunXMPP.ProtocolTreeNode child4 = node.GetChild("identity");
            if (child4 != null)
              numArray = child4.data;
            onError(attributeInt, numArray);
          })));
          this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
          {
            new FunXMPP.KeyValue("xmlns", "encrypt"),
            new FunXMPP.KeyValue("type", "set"),
            new FunXMPP.KeyValue("to", "s.whatsapp.net"),
            new FunXMPP.KeyValue("id", str)
          }, new FunXMPP.ProtocolTreeNode[1]
          {
            new FunXMPP.ProtocolTreeNode("rotate", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("skey", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode[3]
            {
              new FunXMPP.ProtocolTreeNode("id", (FunXMPP.KeyValue[]) null, latestSignedPreKey.Id),
              new FunXMPP.ProtocolTreeNode("value", (FunXMPP.KeyValue[]) null, latestSignedPreKey.Data),
              new FunXMPP.ProtocolTreeNode("signature", (FunXMPP.KeyValue[]) null, latestSignedPreKey.Signature)
            }))
          }));
        }));
      }

      public void SendEncryptedMessageRetry(
        string keyRemoteJid,
        string keyId,
        string participant,
        int version,
        int retryCount,
        DateTime? timestamp,
        uint registrationId,
        bool mmsretry,
        int edit_version)
      {
        List<FunXMPP.KeyValue> keyValueList1 = new List<FunXMPP.KeyValue>((IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[3]
        {
          new FunXMPP.KeyValue("v", version.ToString()),
          new FunXMPP.KeyValue("count", retryCount.ToString()),
          new FunXMPP.KeyValue("id", keyId)
        });
        if (timestamp.HasValue)
          keyValueList1.Add(new FunXMPP.KeyValue("t", timestamp.Value.ToUnixTime().ToString()));
        if (mmsretry)
          keyValueList1.Add(new FunXMPP.KeyValue("mediareason", "retry"));
        this.SwapIfNeeded(ref keyRemoteJid, ref participant);
        List<FunXMPP.KeyValue> keyValueList2 = new List<FunXMPP.KeyValue>((IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("from", Settings.MyJid),
          new FunXMPP.KeyValue("to", keyRemoteJid),
          new FunXMPP.KeyValue("id", keyId),
          new FunXMPP.KeyValue("type", "retry")
        });
        if (participant != null)
          keyValueList2.Add(new FunXMPP.KeyValue(nameof (participant), participant));
        if (edit_version > 0)
          keyValueList2.Add(new FunXMPP.KeyValue("edit", edit_version.ToString()));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("receipt", keyValueList2.ToArray(), new FunXMPP.ProtocolTreeNode[2]
        {
          new FunXMPP.ProtocolTreeNode("retry", keyValueList1.ToArray(), (byte[]) null),
          new FunXMPP.ProtocolTreeNode("registration", (FunXMPP.KeyValue[]) null, FunXMPP.Connection.UIntToBytes(registrationId))
        }));
      }

      public void SendGetPreKeys(
        IEnumerable<string> jids,
        Action<List<AxolotlUser>, IEnumerable<string>> onComplete,
        Action<IEnumerable<string>> onError)
      {
        this.InvokeWhenConnected((Action) (() =>
        {
          string str = this.MakeId("getprekey_");
          this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
          {
            FunXMPP.ProtocolTreeNode child7 = node.GetChild("list");
            if (child7 == null)
              return;
            List<AxolotlUser> axolotlUserList = new List<AxolotlUser>();
            List<string> source = new List<string>();
            foreach (FunXMPP.ProtocolTreeNode allChild in child7.GetAllChildren("user"))
            {
              AxolotlUser axolotlUser = new AxolotlUser();
              axolotlUser.jid = allChild.GetAttributeValue("jid");
              if (allChild.GetChild("error") == null)
              {
                FunXMPP.ProtocolTreeNode child8 = allChild.GetChild("registration");
                if (child8 != null)
                {
                  byte[] data = child8.data;
                  axolotlUser.registrationId = (int) data[0] << 24 | (int) data[1] << 16 | (int) data[2] << 8 | (int) data[3];
                }
                FunXMPP.ProtocolTreeNode child9 = allChild.GetChild("type");
                if (child9 != null)
                  axolotlUser.type = child9.data[0];
                FunXMPP.ProtocolTreeNode child10 = allChild.GetChild("identity");
                if (child10 != null)
                  axolotlUser.identity = child10.data;
                FunXMPP.ProtocolTreeNode child11 = allChild.GetChild("key");
                if (child11 != null)
                {
                  axolotlUser.preKey = new AxolotlPreKey();
                  axolotlUser.preKey.Id = child11.GetChild("id").data;
                  axolotlUser.preKey.Data = child11.GetChild("value").data;
                }
                FunXMPP.ProtocolTreeNode child12 = allChild.GetChild("skey");
                if (child12 != null)
                {
                  axolotlUser.signedPreKey = new AxolotlPreKey();
                  axolotlUser.signedPreKey.Id = child12.GetChild("id").data;
                  axolotlUser.signedPreKey.Data = child12.GetChild("value").data;
                  axolotlUser.signedPreKey.Signature = child12.GetChild("signature").data;
                }
                axolotlUserList.Add(axolotlUser);
              }
              else
                source.Add(axolotlUser.jid);
            }
            onComplete(axolotlUserList, jids);
            if (!source.Any<string>())
              return;
            onError((IEnumerable<string>) source);
          }), (Action<int>) (err => onError(jids))));
          List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList = new List<FunXMPP.ProtocolTreeNode>();
          foreach (string jid in jids)
          {
            FunXMPP.ProtocolTreeNode protocolTreeNode = new FunXMPP.ProtocolTreeNode("user", new FunXMPP.KeyValue[1]
            {
              new FunXMPP.KeyValue("jid", jid)
            });
            protocolTreeNodeList.Add(protocolTreeNode);
          }
          this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
          {
            new FunXMPP.KeyValue("xmlns", "encrypt"),
            new FunXMPP.KeyValue("type", "get"),
            new FunXMPP.KeyValue("to", "s.whatsapp.net"),
            new FunXMPP.KeyValue("id", str)
          }, new FunXMPP.ProtocolTreeNode("key", (FunXMPP.KeyValue[]) null, protocolTreeNodeList.ToArray())));
        }));
      }

      public void SendGetPreKeyDigest(Action<AxolotlDigest> onComplete, Action<int> onError)
      {
        this.InvokeWhenConnected((Action) (() =>
        {
          string str = this.MakeId("getprekey_");
          this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
          {
            FunXMPP.ProtocolTreeNode child7 = node.GetChild("digest");
            if (child7 == null)
              return;
            AxolotlDigest axolotlDigest = new AxolotlDigest();
            FunXMPP.ProtocolTreeNode child8 = child7.GetChild("registration");
            if (child8 != null)
            {
              byte[] data = child8.data;
              axolotlDigest.registrationId = (int) data[0] << 24 | (int) data[1] << 16 | (int) data[2] << 8 | (int) data[3];
            }
            FunXMPP.ProtocolTreeNode child9 = child7.GetChild("type");
            if (child9 != null)
              axolotlDigest.type = child9.data[0];
            FunXMPP.ProtocolTreeNode child10 = child7.GetChild("skey");
            if (child10 != null)
              axolotlDigest.signedPreKeyId = child10.GetChild("id").data;
            FunXMPP.ProtocolTreeNode child11 = child7.GetChild("hash");
            if (child11 != null)
              axolotlDigest.digestHash = child11.data;
            FunXMPP.ProtocolTreeNode child12 = child7.GetChild("list");
            if (child12 != null)
            {
              List<int> intList = new List<int>();
              foreach (FunXMPP.ProtocolTreeNode allChild in child12.GetAllChildren("id"))
              {
                byte[] data = allChild.data;
                intList.Add((int) data[0] << 16 | (int) data[1] << 8 | (int) data[2]);
              }
              axolotlDigest.preKeyIds = intList;
            }
            onComplete(axolotlDigest);
          }), (Action<int>) (err =>
          {
            if (onError == null)
              return;
            onError(err);
          })));
          this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
          {
            new FunXMPP.KeyValue("xmlns", "encrypt"),
            new FunXMPP.KeyValue("type", "get"),
            new FunXMPP.KeyValue("to", "s.whatsapp.net"),
            new FunXMPP.KeyValue("id", str)
          }, new FunXMPP.ProtocolTreeNode("digest", (FunXMPP.KeyValue[]) null)));
        }));
      }

      public void SendEncryptedMessageUnknownTags(
        string keyRemoteJid,
        string keyId,
        string participant,
        uint[] unknownTags)
      {
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>((IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("from", Settings.MyJid),
          new FunXMPP.KeyValue("to", keyRemoteJid),
          new FunXMPP.KeyValue("id", keyId),
          new FunXMPP.KeyValue("type", "error")
        });
        if (participant != null)
          keyValueList.Add(new FunXMPP.KeyValue(nameof (participant), participant));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("receipt", keyValueList.ToArray(), new FunXMPP.ProtocolTreeNode("error", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("type", "enc-v2-unknown-tags")
        }, ((IEnumerable<uint>) unknownTags).Select<uint, FunXMPP.ProtocolTreeNode>((Func<uint, FunXMPP.ProtocolTreeNode>) (i => new FunXMPP.ProtocolTreeNode("tag", (FunXMPP.KeyValue[]) null, i.ToString()))).ToArray<FunXMPP.ProtocolTreeNode>())));
      }

      public void SendEncryptedMediaNotification(
        Action onAck,
        Action onNoAck,
        string keyRemoteJid,
        string keyId,
        string participant)
      {
        this.AddNotificationHandler(keyId, onAck, onNoAck);
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>((IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("from", Settings.MyJid),
          new FunXMPP.KeyValue("to", keyRemoteJid),
          new FunXMPP.KeyValue("id", keyId),
          new FunXMPP.KeyValue("type", "mediaretry")
        });
        if (participant != null)
          keyValueList.Add(new FunXMPP.KeyValue(nameof (participant), participant));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("notification", keyValueList.ToArray(), (byte[]) null));
      }

      public void SendLiveLocationKeyDeny(string keyRemoteJid)
      {
        string v = this.MakeId("livelocation_");
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>((IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("from", Settings.MyJid),
          new FunXMPP.KeyValue("to", keyRemoteJid),
          new FunXMPP.KeyValue("id", v),
          new FunXMPP.KeyValue("type", "location")
        });
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("encrypt", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode[1]
        {
          new FunXMPP.ProtocolTreeNode("deny", (FunXMPP.KeyValue[]) null, (byte[]) null)
        });
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("notification", keyValueList.ToArray(), child));
      }

      public void SendLiveLocationKeyRetryNotification(
        string keyRemoteJid,
        int retryCount,
        uint registration)
      {
        string v = this.MakeId("livelocation_");
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>((IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("from", Settings.MyJid),
          new FunXMPP.KeyValue("to", keyRemoteJid),
          new FunXMPP.KeyValue("id", v),
          new FunXMPP.KeyValue("type", "location")
        });
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("encrypt", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode[2]
        {
          new FunXMPP.ProtocolTreeNode("request", new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("retry", retryCount.ToString())
          }, (byte[]) null),
          new FunXMPP.ProtocolTreeNode(nameof (registration), (FunXMPP.KeyValue[]) null, FunXMPP.Connection.UIntToBytes(registration))
        });
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("notification", keyValueList.ToArray(), child));
      }

      private void HandleOfflineWebNotifications()
      {
        foreach (FunXMPP.ProtocolTreeNode protocolTreeNode in this.offlineWebNodes.Values)
        {
          string attributeValue1 = protocolTreeNode.GetAttributeValue("id");
          string attributeValue2 = protocolTreeNode.GetAttributeValue("t");
          DateTime? notificationTimestamp = new DateTime?();
          ref DateTime? local = ref notificationTimestamp;
          if (!FunXMPP.TryParseTimestamp(attributeValue2, out local))
            notificationTimestamp = new DateTime?(DateTime.UtcNow);
          Action ackAction = (Action) null;
          try
          {
            Log.d("WebClient", string.Format("Processing offline notification id={0}", (object) attributeValue1));
            this.ProcessQrNode(protocolTreeNode.GetChild(0), attributeValue1, notificationTimestamp, ref ackAction);
          }
          catch (Exception ex)
          {
            Log.l(ex, "WebClient > Exception from offline ProcessQrNode");
          }
        }
        this.offlineWebNodes.Clear();
        this.offlineWebRefs.Clear();
      }

      public void SendGetVerifiedName(
        string keyRemoteJid,
        ulong? currentSerial,
        Action<ulong?, string, byte[]> onCompleted,
        Action<int> onError)
      {
        string str = this.MakeId("getverifiedname_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          FunXMPP.ProtocolTreeNode child = node.GetChild("verified_name");
          string attributeValue = child.GetAttributeValue("serial");
          ulong? nullable = new ulong?();
          if (attributeValue == null)
          {
            if (currentSerial.HasValue)
              nullable = new ulong?(currentSerial.Value);
            else
              Log.l("verified name", "No serial value found in server response");
          }
          else
          {
            ulong result;
            ulong.TryParse(attributeValue, out result);
            nullable = new ulong?(result);
          }
          if (child == null)
            return;
          if (child.GetAttributeValue("jid") != keyRemoteJid || child.GetAttributeValue("v") != "1")
          {
            Log.l("verified name", "Incorrect jid or version in server response {0} {1} {2}", (object) child.GetAttributeValue("jid"), (object) keyRemoteJid, (object) child.GetAttributeValue("v"));
            if (onError == null)
              return;
            onError(600);
          }
          else
            onCompleted(nullable, child.GetAttributeValue("verified_level"), child.data);
        }), (Action<int>) (err =>
        {
          if (onError == null)
            return;
          onError(err);
        })));
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>();
        keyValueList.Add(new FunXMPP.KeyValue("jid", keyRemoteJid));
        if (currentSerial.HasValue)
          keyValueList.Add(new FunXMPP.KeyValue("serial", currentSerial.Value.ToString()));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("xmlns", "w:biz"),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str)
        }, new FunXMPP.ProtocolTreeNode("verified_name", keyValueList.ToArray(), (byte[]) null)));
      }

      public void SendGetBusinessProfile(
        string jid,
        string tag,
        Action<System.Collections.Generic.Dictionary<string, BizProfileDetails>> onCompleted,
        Action<int> onError)
      {
        this.SendGetBusinessProfiles(new List<Pair<string, string>>()
        {
          new Pair<string, string>(jid, tag)
        }, onCompleted, onError);
      }

      private void SendGetBusinessProfiles(
        List<Pair<string, string>> jidsAndTags,
        Action<System.Collections.Generic.Dictionary<string, BizProfileDetails>> onCompleted,
        Action<int> onError)
      {
        string str = this.MakeId("getbizprof_");
        this.AddIqHandler(str, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          System.Collections.Generic.Dictionary<string, BizProfileDetails> dictionary = new System.Collections.Generic.Dictionary<string, BizProfileDetails>();
          FunXMPP.ProtocolTreeNode child = node.GetChild("business_profile");
          if (child != null)
          {
            foreach (FunXMPP.ProtocolTreeNode allChild in child.GetAllChildren("profile"))
            {
              string attributeValue = allChild.GetAttributeValue("jid");
              BizProfileDetails profileDetails = BizProfileDetails.ExtractProfileDetails(allChild);
              dictionary[attributeValue] = profileDetails;
            }
          }
          onCompleted(dictionary);
        }), (Action<int>) (err =>
        {
          if (onError == null)
            return;
          onError(err);
        })));
        List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList = new List<FunXMPP.ProtocolTreeNode>();
        foreach (Pair<string, string> jidsAndTag in jidsAndTags)
        {
          FunXMPP.KeyValue[] attrs;
          if (string.IsNullOrEmpty(jidsAndTag.Second))
            attrs = new FunXMPP.KeyValue[1];
          else
            attrs = new FunXMPP.KeyValue[2]
            {
              null,
              new FunXMPP.KeyValue("tag", jidsAndTag.Second)
            };
          attrs[0] = new FunXMPP.KeyValue("jid", jidsAndTag.First);
          protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("profile", attrs, (FunXMPP.ProtocolTreeNode[]) null));
        }
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("xmlns", "w:biz"),
          new FunXMPP.KeyValue("type", "get"),
          new FunXMPP.KeyValue("to", "s.whatsapp.net"),
          new FunXMPP.KeyValue("id", str)
        }, new FunXMPP.ProtocolTreeNode("business_profile", (FunXMPP.KeyValue[]) null, protocolTreeNodeList.ToArray())));
      }

      public void ProcessNode(FunXMPP.ProtocolTreeNode node)
      {
        if (node == null)
          throw new FunXMPP.StreamEndException("Got stream end");
        if (FunXMPP.ProtocolTreeNode.TagEquals(node, "iq"))
        {
          string attributeValue1 = node.GetAttributeValue("type");
          string attributeValue2 = node.GetAttributeValue("id");
          string attributeValue3 = node.GetAttributeValue("from");
          switch (attributeValue1)
          {
            case null:
              throw new FunXMPP.CorruptStreamException("missing 'type' attribute in iq stanza");
            case "result":
              FunXMPP.IqResultHandler iqResultHandler1;
              if ((iqResultHandler1 = this.PopIqHandler(attributeValue2)) != null)
              {
                iqResultHandler1.Parse(node, attributeValue3);
                break;
              }
              if (!attributeValue2.StartsWith(Settings.ChatID))
                break;
              this.accountType = FunXMPP.Connection.AccountKind.Free;
              break;
            case "error":
              FunXMPP.IqResultHandler iqResultHandler2;
              if ((iqResultHandler2 = this.PopIqHandler(attributeValue2)) == null)
                break;
              iqResultHandler2.ErrorNode(node);
              break;
            case "get":
              if (node.children != null && node.children.Length != 0)
                node.GetChild(0);
              if (!(node.GetAttributeValue("xmlns") == "urn:xmpp:ping"))
                break;
              this.EventHandler.OnPing(attributeValue2);
              break;
            case "set":
              if (!(node.GetAttributeValue("xmlns") == "location"))
                break;
              FunXMPP.ProtocolTreeNode child = node.children == null || node.children.Length == 0 ? (FunXMPP.ProtocolTreeNode) null : node.GetChild(0);
              if (child.tag == "start")
              {
                int? attributeInt = child.GetAttributeInt("duration");
                LiveLocationManager.Instance.StartLocationReporting(attributeValue3, attributeValue2, attributeInt);
              }
              if (child.tag == "stop")
                LiveLocationManager.Instance.StopLocationReporting(attributeValue2);
              if (!(child.tag == "enable"))
                break;
              child.GetAttributeInt("expiration");
              string participant = !attributeValue3.IsGroupJid() ? attributeValue3 : node.GetAttributeValue("participant");
              this.SendEnabledLocationSharingResponse(attributeValue3, participant, attributeValue2);
              break;
            default:
              throw new FunXMPP.CorruptStreamException("unknown iq type attribute: " + attributeValue1);
          }
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "presence"))
        {
          string attributeValue4 = node.GetAttributeValue("xmlns");
          string attributeValue5 = node.GetAttributeValue("from");
          if (attributeValue4 != null && !"urn:xmpp".Equals(attributeValue4) || attributeValue5 == null)
            return;
          switch (node.GetAttributeValue("type"))
          {
            case "unavailable":
              DateTime? dt = new DateTime?();
              string attributeValue6 = node.GetAttributeValue("last");
              switch (attributeValue6)
              {
                case "deny":
                case "none":
                case "error":
                  dt = new DateTime?();
                  break;
                case null:
                  dt = new DateTime?(DateTime.UtcNow);
                  break;
                default:
                  FunXMPP.TryParseTimestamp(attributeValue6, out dt);
                  break;
              }
              this.EventHandler.OnAvailable(attributeValue5, false, dt);
              break;
            case null:
            case "available":
              this.EventHandler.OnAvailable(attributeValue5, true);
              break;
          }
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "message"))
          this.ParseMessageInitialTagAlreadyChecked(node);
        else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "ib"))
        {
          foreach (FunXMPP.ProtocolTreeNode node1 in node.children ?? new FunXMPP.ProtocolTreeNode[0])
          {
            if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "offline"))
            {
              this.EventHandler.OnOfflineMessagesCompleted();
              FunXMPP.OfflineMarkerSubject.OnNext(new Unit());
              this.HandleOfflineWebNotifications();
            }
            else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "dirty"))
            {
              string attributeValue7 = node1.GetAttributeValue("type");
              string attributeValue8 = node1.GetAttributeValue("timestamp");
              long result = 0;
              if (attributeValue7 != null && long.TryParse(attributeValue8, out result))
                this.EventHandler.OnDirty(attributeValue7, result);
            }
            else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "sonar"))
            {
              string attributeValue = node1.GetAttributeValue("url");
              if (!string.IsNullOrEmpty(attributeValue))
                this.EventHandler.OnSonar(attributeValue);
            }
            else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "location"))
            {
              string attributeValue = node.GetAttributeValue("from");
              int elapsed = node1.GetAttributeInt("elapsed") ?? 0;
              foreach (FunXMPP.ProtocolTreeNode node2 in node1.children ?? new FunXMPP.ProtocolTreeNode[0])
              {
                if (FunXMPP.ProtocolTreeNode.TagEquals(node2, "enc"))
                  this.EventHandler.OnLocationForMe(attributeValue, elapsed, node2.data);
              }
            }
            else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "client_expiration"))
            {
              DateTime? dt;
              if (FunXMPP.TryParseTimestamp(node1.GetAttributeValue("t"), out dt))
                AppState.UpdateServerExpirationOverride(dt.GetValueOrDefault());
            }
            else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "edge_routing"))
            {
              foreach (FunXMPP.ProtocolTreeNode node3 in node1.children ?? new FunXMPP.ProtocolTreeNode[0])
              {
                if (FunXMPP.ProtocolTreeNode.TagEquals(node3, "routing_info"))
                  Settings.EdgeRoutingInfo = node3.data;
                else if (FunXMPP.ProtocolTreeNode.TagEquals(node3, "dns_domain"))
                  NonDbSettings.ChatDnsDomain = node3.GetDataString();
              }
            }
            else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "fbip"))
              FunRunner.SaveFallbackIp(node1.GetDataString());
          }
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "receipt"))
        {
          System.Collections.Generic.Dictionary<string, string> attributes = node.GetAttributes(new string[7]
          {
            "from",
            "to",
            "participant",
            "id",
            "type",
            "t",
            "edit"
          });
          string type = attributes["type"];
          string to = attributes["to"];
          string from = attributes["from"];
          string participant = attributes["participant"];
          string keyId = attributes["id"];
          string str = attributes["t"];
          string editVersion = attributes["edit"];
          DateTime? dt = new DateTime?();
          ref DateTime? local = ref dt;
          FunXMPP.TryParseTimestamp(str, out local);
          if (type == "server-error")
          {
            AppState.SchedulePersistentAction(PersistentAction.ReuploadMedia(from, true, keyId, participant));
            this.SendReceiptAck(to, from, participant, type, keyId, (string) null);
          }
          else
          {
            if (type == null)
            {
              if (node.GetChild("offer") != null && this.VoipEventHandler != null)
                this.VoipEventHandler.HandleVoipOfferReceipt(node);
              type = "delivery";
            }
            FunXMPP.Connection.AsyncReceiptThread.Enqueue((Action) (() =>
            {
              bool includeReadReceiptDisabled = false;
              switch (type)
              {
                case "retry":
                  int? nullable1 = new int?();
                  int? nullable2 = new int?();
                  string id = (string) null;
                  uint registration = 0;
                  FunXMPP.ProtocolTreeNode child1 = node.GetChild("retry");
                  if (child1 != null)
                  {
                    nullable1 = child1.GetAttributeInt("v");
                    nullable2 = child1.GetAttributeInt("count");
                    id = child1.GetAttributeValue("id");
                    child1.GetAttributeValue("mediareason");
                  }
                  FunXMPP.ProtocolTreeNode child2 = node.GetChild("registration");
                  if (child2 != null)
                    registration = FunXMPP.Connection.UIntFromBytes(child2.data);
                  this.EventHandler.OnMessageRetryFromTarget(new FunXMPP.FMessage.Key(from, true, id), participant, nullable1.HasValue ? nullable1.Value : 0, nullable2.HasValue ? nullable2.Value : 0, registration);
                  break;
                case "enc_rekey_retry":
                  if (this.VoipEventHandler != null)
                  {
                    this.VoipEventHandler.HandleEncRekeyRetry(node);
                    break;
                  }
                  break;
                default:
                  FunXMPP.FMessage.Status? nullable3 = new FunXMPP.FMessage.Status?();
                  switch (type)
                  {
                    case "delivery":
                      nullable3 = new FunXMPP.FMessage.Status?(FunXMPP.FMessage.Status.ReceivedByTarget);
                      break;
                    case "read":
                      nullable3 = new FunXMPP.FMessage.Status?(FunXMPP.FMessage.Status.ReadByTarget);
                      break;
                    case "played":
                      nullable3 = new FunXMPP.FMessage.Status?(FunXMPP.FMessage.Status.PlayedByTarget);
                      break;
                  }
                  if (nullable3.HasValue)
                  {
                    List<string> stringList = new List<string>()
                    {
                      keyId
                    };
                    FunXMPP.ProtocolTreeNode child3 = node.GetChild("list");
                    if (child3 != null)
                      stringList.AddRange(child3.GetAllChildren("item").Select<FunXMPP.ProtocolTreeNode, string>((Func<FunXMPP.ProtocolTreeNode, string>) (item => item.GetAttributeValue("id"))).Where<string>((Func<string, bool>) (itemId => itemId != null)));
                    using (List<string>.Enumerator enumerator = stringList.GetEnumerator())
                    {
                      while (enumerator.MoveNext())
                      {
                        FunXMPP.FMessage.Key message = new FunXMPP.FMessage.Key(from, true, enumerator.Current);
                        bool ignored = false;
                        this.EventHandler.OnMessageReceipt(message, participant, nullable3.Value, out ignored, dt: dt);
                        if (ignored)
                          includeReadReceiptDisabled = true;
                      }
                      break;
                    }
                  }
                  else
                    break;
              }
              this.SendReceiptAck(to, from, participant, type, keyId, editVersion, includeReadReceiptDisabled);
            }));
          }
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "ack"))
        {
          System.Collections.Generic.Dictionary<string, string> attributes = node.GetAttributes(new string[7]
          {
            "from",
            "participant",
            "id",
            "type",
            "class",
            "count",
            "t"
          });
          string str1 = attributes["class"];
          switch (str1)
          {
            case "message":
              FunXMPP.FMessage.Key key1 = new FunXMPP.FMessage.Key(attributes["from"], true, attributes["id"]);
              string attributeValue = node.GetAttributeValue("error");
              if (!string.IsNullOrEmpty(attributeValue))
              {
                int result = 500;
                int.TryParse(attributeValue, out result);
                this.EventHandler.OnMessageError(key1, result);
                break;
              }
              string s = attributes["count"];
              int result1 = 0;
              int? expectedDeliveryCount = new int?();
              if (s != null && int.TryParse(s, out result1))
                expectedDeliveryCount = new int?(result1);
              string str2 = attributes["t"];
              DateTime? dt = new DateTime?();
              ref DateTime? local = ref dt;
              FunXMPP.TryParseTimestamp(str2, out local);
              string participant1 = attributes["participant"];
              List<Action> actionList = this.PopReceiptListener(key1.id, participant1 ?? key1.remote_jid);
              if (actionList != null)
              {
                foreach (Action action in actionList)
                  action();
              }
              bool ignored = false;
              this.EventHandler.OnMessageReceipt(key1, participant1, FunXMPP.FMessage.Status.ReceivedByServer, out ignored, expectedDeliveryCount, dt);
              break;
            case "receipt":
              FunXMPP.FMessage.Key key2 = new FunXMPP.FMessage.Key(attributes["from"], false, attributes["id"]);
              string type = attributes["type"];
              string participant2 = attributes["participant"];
              FunXMPP.Connection.PendingReceiptState pendingReceiptState = this.LookupPendingReceipt(key2.remote_jid, participant2, key2.id, type, false, true);
              if (pendingReceiptState == null || pendingReceiptState.AckCallback == null)
                break;
              pendingReceiptState.AckCallback();
              break;
            case "call":
              if (this.VoipEventHandler == null)
                break;
              this.VoipEventHandler.HandleVoipAck(node);
              break;
            case "notification":
              string str3 = attributes["type"];
              string id = attributes["id"];
              if (this.TryToAckNotificationReceived(id))
                break;
              Log.l("funxmpp", "ack handler not run for {0}, {1}", (object) (id ?? "<null>"), (object) (str3 ?? "<null>"));
              break;
            default:
              Log.l("funxmpp", "unrecognized ack class [{0}]", (object) (str1 ?? "<null>"));
              break;
          }
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "chatstate"))
        {
          string attributeValue9 = node.GetAttributeValue("from");
          string attributeValue10 = node.GetAttributeValue("participant");
          FunXMPP.ProtocolTreeNode child = node.children == null || node.children.Length == 0 ? (FunXMPP.ProtocolTreeNode) null : node.GetChild(0);
          if (FunXMPP.ProtocolTreeNode.TagEquals(child, "paused"))
          {
            this.EventHandler.OnComposing(attributeValue9, attributeValue10, false);
          }
          else
          {
            if (!FunXMPP.ProtocolTreeNode.TagEquals(child, "composing"))
              return;
            bool flag = child.GetAttributeValue("media") == "audio";
            this.EventHandler.OnComposing(attributeValue9, attributeValue10, true, flag ? FunXMPP.FMessage.Type.Audio : FunXMPP.FMessage.Type.Undefined);
          }
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "notification"))
          this.ProcessNotification(node);
        else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "call"))
        {
          if (this.VoipEventHandler == null)
            return;
          this.VoipEventHandler.HandleVoipNode(node);
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "stream:error"))
        {
          FunXMPP.Logger.LogStanza(node);
          FunXMPP.ProtocolTreeNode child = node.children == null || node.children.Length == 0 ? (FunXMPP.ProtocolTreeNode) null : node.GetChild(0);
          string attributeValue = node.GetAttributeValue("code");
          if (attributeValue != null)
          {
            int result = 0;
            int.TryParse(attributeValue, out result);
            if (result >= 500 && result < 600)
            {
              Settings.ServerRequestedFibBackoffState = Math.Max(Settings.ServerRequestedFibBackoffStateSaved, 1);
              Log.l(nameof (FunXMPP), "Server backoff requested: {0} {1}", (object) Settings.ServerRequestedFibBackoffState, (object) attributeValue);
            }
          }
          if (!FunXMPP.ProtocolTreeNode.TagEquals(child, "ack"))
            return;
          this.Protocol.TreeNodeWriter.StreamEnd();
        }
        else
        {
          Log.l("funxmpp", "unrecognized top-level stanza [{0}]", (object) (node.tag ?? "<null>"));
          FunXMPP.Logger.LogStanza(node);
        }
      }

      public static WorkQueue AsyncReceiptThread
      {
        get
        {
          return Utils.LazyInit<WorkQueue>(ref FunXMPP.Connection.asyncReceiptThread, (Func<WorkQueue>) (() => new WorkQueue(identifierString: nameof (AsyncReceiptThread))));
        }
      }

      private void SendReceiptAck(
        string to,
        string from,
        string participant,
        string type,
        string keyId,
        string editVersion,
        bool includeReadReceiptDisabled = false)
      {
        List<FunXMPP.KeyValue> second = new List<FunXMPP.KeyValue>();
        if (!string.IsNullOrEmpty(to))
          second.Add(new FunXMPP.KeyValue(nameof (from), to));
        if (!string.IsNullOrEmpty(from))
          second.Add(new FunXMPP.KeyValue(nameof (to), from));
        if (!string.IsNullOrEmpty(participant))
          second.Add(new FunXMPP.KeyValue(nameof (participant), participant));
        if (!string.IsNullOrEmpty(editVersion))
          second.Add(new FunXMPP.KeyValue("edit", editVersion));
        FunXMPP.ProtocolTreeNode protocolTreeNode;
        if (!includeReadReceiptDisabled)
          protocolTreeNode = (FunXMPP.ProtocolTreeNode) null;
        else
          protocolTreeNode = new FunXMPP.ProtocolTreeNode("features", new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("readreceipts", "disable")
          });
        FunXMPP.ProtocolTreeNode child = protocolTreeNode;
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("ack", ((IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[3]
        {
          new FunXMPP.KeyValue("class", "receipt"),
          new FunXMPP.KeyValue(nameof (type), type),
          new FunXMPP.KeyValue("id", keyId)
        }).Concat<FunXMPP.KeyValue>((IEnumerable<FunXMPP.KeyValue>) second).ToArray<FunXMPP.KeyValue>(), child));
      }

      private void SendMessageAck(
        string to,
        string from,
        string participant,
        string keyId,
        string typeAttribute,
        string editVersion,
        string error)
      {
        List<FunXMPP.KeyValue> second = new List<FunXMPP.KeyValue>();
        if (!string.IsNullOrEmpty(to))
          second.Add(new FunXMPP.KeyValue(nameof (from), to));
        if (!string.IsNullOrEmpty(from))
          second.Add(new FunXMPP.KeyValue(nameof (to), from));
        if (!string.IsNullOrEmpty(participant))
          second.Add(new FunXMPP.KeyValue(nameof (participant), participant));
        if (!string.IsNullOrEmpty(typeAttribute))
          second.Add(new FunXMPP.KeyValue("type", typeAttribute));
        if (!string.IsNullOrEmpty(editVersion))
          second.Add(new FunXMPP.KeyValue("edit", editVersion));
        if (!string.IsNullOrEmpty(error))
          second.Add(new FunXMPP.KeyValue(nameof (error), error));
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("ack", ((IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[2]
        {
          new FunXMPP.KeyValue("class", "message"),
          new FunXMPP.KeyValue("id", keyId)
        }).Concat<FunXMPP.KeyValue>((IEnumerable<FunXMPP.KeyValue>) second).ToArray<FunXMPP.KeyValue>(), (FunXMPP.ProtocolTreeNode) null));
      }

      internal void ParseMessageInitialTagAlreadyChecked(FunXMPP.ProtocolTreeNode messageNode)
      {
        FunXMPP.FMessage.Builder builder = new FunXMPP.FMessage.Builder();
        string attributeValue1 = messageNode.GetAttributeValue("id");
        messageNode.GetAttributeValue("t");
        string attributeValue2 = messageNode.GetAttributeValue("from");
        string attributeValue3 = messageNode.GetAttributeValue("to");
        string attributeValue4 = messageNode.GetAttributeValue("participant");
        string attributeValue5 = messageNode.GetAttributeValue("edit");
        this.PreparseMessage(builder, messageNode);
        string attributeValue6 = messageNode.GetAttributeValue("type");
        if ("error".Equals(attributeValue6))
        {
          int code = 0;
          foreach (FunXMPP.ProtocolTreeNode allChild in messageNode.GetAllChildren("error"))
          {
            string attributeValue7 = allChild.GetAttributeValue("code");
            try
            {
              code = int.Parse(attributeValue7, (IFormatProvider) CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
            }
          }
          if (attributeValue2 == null || attributeValue1 == null)
            return;
          this.EventHandler.OnMessageError(new FunXMPP.FMessage.Key(attributeValue2, true, attributeValue1), code);
        }
        else
        {
          if (!"text".Equals(attributeValue6) && !"media".Equals(attributeValue6) && !"enc".Equals(attributeValue6) && !"pay".Equals(attributeValue6))
            return;
          builder.Wants_receipt(true);
          bool flag = false;
          if (!string.IsNullOrEmpty(attributeValue2) && !JidChecker.CheckJidProtocolString(attributeValue2))
            flag = JidChecker.MaybeSendJidErrorClb("On Message - from", attributeValue2);
          if (!flag && !string.IsNullOrEmpty(attributeValue4) && !JidChecker.CheckJidProtocolString(attributeValue4))
            flag = JidChecker.MaybeSendJidErrorClb("On Message - participant", attributeValue4);
          if (flag)
          {
            this.SendMessageAck(attributeValue3, attributeValue2, attributeValue4, attributeValue1, attributeValue6, attributeValue5, "406");
          }
          else
          {
            if (attributeValue2.IsBroadcastJid())
              Utils.Swap<string>(ref attributeValue2, ref attributeValue4);
            foreach (FunXMPP.ProtocolTreeNode childNode in messageNode.children ?? new FunXMPP.ProtocolTreeNode[0])
            {
              if (!this.ParseMessageCore(builder, messageNode, childNode, false, attributeValue2, attributeValue4, attributeValue1))
              {
                this.SendMessageAck(attributeValue3, attributeValue2, attributeValue4, attributeValue1, attributeValue6, attributeValue5, "406");
                return;
              }
            }
            if (!string.IsNullOrEmpty(attributeValue5))
            {
              int result = 0;
              if (int.TryParse(attributeValue5, out result))
                builder.Edit_version(result);
            }
            this.PostparseMessage(builder);
            FunXMPP.FMessage message = builder.Build();
            if (message == null || this.EventHandler == null)
              return;
            this.EventHandler.OnMessageForMe(message);
          }
        }
      }

      private void ProcessNotification(FunXMPP.ProtocolTreeNode notifyNode)
      {
        string from = notifyNode.GetAttributeValue("from");
        string to = notifyNode.GetAttributeValue("to");
        string participant = notifyNode.GetAttributeValue("participant");
        string id = notifyNode.GetAttributeValue("id");
        string attributeValue1 = notifyNode.GetAttributeValue("t");
        DateTime? nullable = new DateTime?();
        ref DateTime? local = ref nullable;
        if (!FunXMPP.TryParseTimestamp(attributeValue1, out local))
          nullable = new DateTime?(FunRunner.CurrentServerTimeUtc);
        string type = notifyNode.GetAttributeValue("type");
        Action ackAction = (Action) (() => this.SendNotificationReceived(from, to, participant, id, type));
        if (StringComparer.Ordinal.Equals(type, "picture"))
        {
          foreach (FunXMPP.ProtocolTreeNode node in notifyNode.children ?? new FunXMPP.ProtocolTreeNode[0])
          {
            if (FunXMPP.ProtocolTreeNode.TagEquals(node, "set"))
            {
              string attributeValue2 = node.GetAttributeValue("hash");
              if (attributeValue2 != null)
              {
                if (this.EventHandler != null)
                {
                  Action<bool> sidelistNotificationAck = this.CreateSidelistNotificationAck(from, to, participant, id, type);
                  ackAction = (Action) null;
                  this.EventHandler.OnSidelistNotification(attributeValue2, UsyncQuery.UsyncProtocol.Picture, sidelistNotificationAck);
                }
              }
              else
              {
                string attributeValue3 = node.GetAttributeValue("id");
                if (attributeValue3 != null)
                  this.EventHandler.OnNewPhotoIdFetched(from, node.GetAttributeValue("author"), attributeValue3, true, "notification pic changed");
              }
            }
            else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "update"))
            {
              string attributeValue4 = node.GetAttributeValue("hash");
              if (attributeValue4 != null && this.EventHandler != null)
              {
                Action<bool> sidelistNotificationAck = this.CreateSidelistNotificationAck(from, to, participant, id, type);
                ackAction = (Action) null;
                this.EventHandler.OnSidelistNotification(attributeValue4, UsyncQuery.UsyncProtocol.Picture, sidelistNotificationAck);
              }
            }
            else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "delete"))
              this.EventHandler.OnNewPhotoIdFetched(from, node.GetAttributeValue("author"), (string) null, true, "notification pic deleted");
            else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "request"))
              this.EventHandler.OnPhotoReuploadRequested(node.GetAttributeValue("jid"));
          }
        }
        else if (StringComparer.Ordinal.Equals(type, "status"))
        {
          FunXMPP.ProtocolTreeNode child = notifyNode.GetChild("set");
          if (child != null)
          {
            string attributeValue5 = child.GetAttributeValue("hash");
            if (attributeValue5 != null)
            {
              if (this.EventHandler != null)
              {
                Action<bool> sidelistNotificationAck = this.CreateSidelistNotificationAck(from, to, participant, id, type);
                ackAction = (Action) null;
                this.EventHandler.OnSidelistNotification(attributeValue5, UsyncQuery.UsyncProtocol.Status, sidelistNotificationAck);
              }
            }
            else
            {
              string str = (string) null;
              if (child != null)
                str = child.GetDataString();
              if (this.EventHandler != null)
                this.EventHandler.OnStatusUpdate(from, nullable, str ?? "", false);
            }
          }
          else if (notifyNode.GetChild("update") != null)
          {
            string attributeValue6 = child.GetAttributeValue("hash");
            if (attributeValue6 != null && this.EventHandler != null)
            {
              Action<bool> sidelistNotificationAck = this.CreateSidelistNotificationAck(from, to, participant, id, type);
              ackAction = (Action) null;
              this.EventHandler.OnSidelistNotification(attributeValue6, UsyncQuery.UsyncProtocol.Status, sidelistNotificationAck);
            }
          }
        }
        else if (StringComparer.Ordinal.Equals(type, "contacts"))
        {
          Action<bool> ack = (Action<bool>) (found =>
          {
            string to1 = from;
            string from1 = to;
            string participant1 = participant;
            string id1 = id;
            string type1 = type;
            FunXMPP.ProtocolTreeNode[] extraNodes;
            if (!found)
              extraNodes = new FunXMPP.ProtocolTreeNode[1]
              {
                new FunXMPP.ProtocolTreeNode("sync", new FunXMPP.KeyValue[1]
                {
                  new FunXMPP.KeyValue("contacts", "out")
                })
              };
            else
              extraNodes = (FunXMPP.ProtocolTreeNode[]) null;
            this.SendNotificationReceived(to1, from1, participant1, id1, type1, (IEnumerable<FunXMPP.ProtocolTreeNode>) extraNodes);
          });
          System.Collections.Generic.Dictionary<string, FunXMPP.Connection.ContactNotificationType> mappings = new System.Collections.Generic.Dictionary<string, FunXMPP.Connection.ContactNotificationType>()
          {
            {
              "add",
              FunXMPP.Connection.ContactNotificationType.Add
            },
            {
              "remove",
              FunXMPP.Connection.ContactNotificationType.Remove
            },
            {
              "update",
              FunXMPP.Connection.ContactNotificationType.Update
            },
            {
              "modify",
              FunXMPP.Connection.ContactNotificationType.Modify
            },
            {
              "sync",
              FunXMPP.Connection.ContactNotificationType.Sync
            }
          };
          FunXMPP.ProtocolTreeNode protocolTreeNode = ((IEnumerable<FunXMPP.ProtocolTreeNode>) (notifyNode.children ?? new FunXMPP.ProtocolTreeNode[0])).Where<FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, bool>) (n => n.tag != null && mappings.ContainsKey(n.tag))).FirstOrDefault<FunXMPP.ProtocolTreeNode>();
          if (protocolTreeNode == null)
          {
            ack(false);
            ackAction = (Action) null;
          }
          else if (protocolTreeNode.tag == "sync")
          {
            int num = protocolTreeNode.GetAttributeInt("after") ?? 0;
            int? attributeInt = notifyNode.GetAttributeInt("t");
            if (attributeInt.HasValue)
            {
              Settings.NextFullSyncUtc = new DateTime?(FunRunner.CurrentServerTimeUtc.AddMilliseconds((double) Math.Max(num - attributeInt.Value, 0)));
              Settings.SyncBackoffUtc = new DateTime?();
            }
            ack(true);
            ackAction = (Action) null;
          }
          else if (protocolTreeNode.tag == "modify")
          {
            string attributeValue7 = protocolTreeNode.GetAttributeValue("old");
            string attributeValue8 = protocolTreeNode.GetAttributeValue("new");
            if (this.EventHandler != null)
              this.EventHandler.OnContactChangeNumber(attributeValue7, attributeValue8, nullable);
          }
          else
          {
            string attributeValue9 = protocolTreeNode.tag == "update" ? protocolTreeNode.GetAttributeValue("hash") : (string) null;
            if (attributeValue9 != null)
            {
              if (this.EventHandler != null)
              {
                Action<bool> sidelistNotificationAck = this.CreateSidelistNotificationAck(from, to, participant, id, type);
                this.EventHandler.OnSidelistNotification(attributeValue9, UsyncQuery.UsyncProtocol.Contact, sidelistNotificationAck);
                ackAction = (Action) null;
              }
            }
            else
            {
              bool flag = true;
              byte[] data = protocolTreeNode.data;
              string jid = (string) null;
              if (data == null)
              {
                jid = protocolTreeNode.GetAttributeValue("jid");
                if (string.IsNullOrEmpty(jid))
                {
                  ack(false);
                  ackAction = (Action) null;
                  flag = false;
                }
              }
              if (flag && this.EventHandler != null)
              {
                FunXMPP.Connection.ContactNotificationType type2 = mappings[protocolTreeNode.tag];
                this.EventHandler.OnSyncNotification(jid, data, type2, ack);
                ackAction = (Action) null;
              }
            }
          }
        }
        else if (StringComparer.Ordinal.Equals(type, "web"))
        {
          string[] tags = new string[3]
          {
            "query",
            "action",
            "enc"
          };
          FunXMPP.ProtocolTreeNode node = ((IEnumerable<FunXMPP.ProtocolTreeNode>) (notifyNode.children ?? new FunXMPP.ProtocolTreeNode[0])).Where<FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, bool>) (n => ((IEnumerable<string>) tags).Where<string>((Func<string, bool>) (s => FunXMPP.ProtocolTreeNode.TagEquals(n, s))).Any<string>())).FirstOrDefault<FunXMPP.ProtocolTreeNode>();
          if (node != null)
          {
            bool flag = false;
            if (notifyNode.GetAttributeValue("offline") != null && FunXMPP.ProtocolTreeNode.TagEquals(node, "action"))
            {
              switch (node.GetAttributeValue("type"))
              {
                case "sync":
                  this.offlineWebNodes[id] = notifyNode;
                  FunXMPP.ProtocolTreeNode child = node.GetChild("sync");
                  if (child != null)
                  {
                    string base64String = Convert.ToBase64String(child.data);
                    if (base64String != null)
                      this.offlineWebRefs[base64String] = id;
                  }
                  flag = true;
                  break;
                case "delete":
                  if (node.data != null)
                  {
                    string base64String = Convert.ToBase64String(node.data);
                    string key = (string) null;
                    if (this.offlineWebRefs.TryGetValue(base64String, out key) && key != null)
                    {
                      this.offlineWebNodes.Remove(key);
                      this.offlineWebRefs.Remove(base64String);
                    }
                    flag = true;
                    break;
                  }
                  break;
              }
            }
            if (!flag)
            {
              try
              {
                this.ProcessQrNode(node, id, nullable, ref ackAction);
              }
              catch (Exception ex)
              {
                Log.l(ex, "WebClient > Exception from ProcessQrNode");
              }
            }
          }
        }
        else if (StringComparer.Ordinal.Equals(type, "w:gp2"))
        {
          string attributeValue10 = notifyNode.GetAttributeValue("notify");
          this.ProcessGroupNotification(notifyNode, from, participant, nullable, attributeValue10);
        }
        else if (StringComparer.Ordinal.Equals(type, "server"))
        {
          foreach (FunXMPP.ProtocolTreeNode node in notifyNode.children ?? new FunXMPP.ProtocolTreeNode[0])
          {
            if (FunXMPP.ProtocolTreeNode.TagEquals(node, "log"))
            {
              try
              {
                Log.SendSupportLog((string) null);
              }
              catch (Exception ex)
              {
                Log.SendCrashLog(ex, "exception sending log");
              }
            }
            else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "props"))
            {
              try
              {
                Settings.ForceServerPropsReload = true;
                AppState.GetConnection().SendGetServerProperties();
              }
              catch (Exception ex)
              {
                Log.l(ex, "exception resetting server props");
              }
            }
          }
        }
        else if (StringComparer.Ordinal.Equals(type, "encrypt"))
        {
          foreach (FunXMPP.ProtocolTreeNode node in notifyNode.children ?? new FunXMPP.ProtocolTreeNode[0])
          {
            Action action = ackAction;
            ackAction = (Action) null;
            if (FunXMPP.ProtocolTreeNode.TagEquals(node, "count"))
            {
              int? attributeInt = node.GetAttributeInt("value");
              if (this.EventHandler != null && attributeInt.HasValue)
                this.EventHandler.OnEncryptionPreKeyCount(attributeInt.Value, action);
            }
            else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "identity"))
            {
              if (this.Encryption.ContainsExistingSession(from))
                this.Encryption.SendGetPreKey(from, action);
              else
                action();
            }
            else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "digest"))
            {
              AppState.SchedulePersistentAction(PersistentAction.SendVerifyAxolotlDigest());
              action();
            }
          }
        }
        else if (StringComparer.Ordinal.Equals(type, "features"))
        {
          RemoteClientCaps remoteClientCaps = new RemoteClientCaps()
          {
            Jid = from
          };
          string jidHashB64 = (string) null;
          foreach (FunXMPP.ProtocolTreeNode node in ((IEnumerable<FunXMPP.ProtocolTreeNode>) (notifyNode.children ?? new FunXMPP.ProtocolTreeNode[0])).Where<FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, bool>) (p => FunXMPP.ProtocolTreeNode.TagEquals(p, "feature"))).SelectMany<FunXMPP.ProtocolTreeNode, FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, IEnumerable<FunXMPP.ProtocolTreeNode>>) (p => (IEnumerable<FunXMPP.ProtocolTreeNode>) p.children ?? (IEnumerable<FunXMPP.ProtocolTreeNode>) new FunXMPP.ProtocolTreeNode[0])))
          {
            jidHashB64 = node.GetAttributeValue("hash");
            ClientCapabilityCategory type3;
            ClientCapabilitySetting setting;
            if (jidHashB64 == null && FunXMPP.Connection.TryParseFeatureNode(node, out type3, out setting))
              remoteClientCaps.AddValue(type3, setting, nullable);
          }
          if (remoteClientCaps.Values.Any<Triad<ClientCapabilityCategory, ClientCapabilitySetting, DateTime?>>())
          {
            Action onComplete = ackAction;
            ackAction = (Action) null;
            this.EventHandler.OnRemoteClientCaps((IEnumerable<RemoteClientCaps>) new RemoteClientCaps[1]
            {
              remoteClientCaps
            }, onComplete);
          }
          else if (jidHashB64 != null && this.EventHandler != null)
          {
            Action<bool> sidelistNotificationAck = this.CreateSidelistNotificationAck(from, to, participant, id, type);
            ackAction = (Action) null;
            this.EventHandler.OnSidelistNotification(jidHashB64, UsyncQuery.UsyncProtocol.Feature, sidelistNotificationAck);
          }
        }
        else if (StringComparer.Ordinal.Equals(type, "auth"))
        {
          Action ack = ackAction;
          ackAction = (Action) null;
          this.EventHandler.OnChangeChatStaticKey(ack);
        }
        else if (StringComparer.Ordinal.Equals(type, "psa"))
        {
          Action action = ackAction;
          ackAction = (Action) null;
          string attributeValue11 = notifyNode.GetAttributeValue("mode");
          Log.l("funxmpp", "received psa | from:{0},id:{1},mode:{2}", (object) from, (object) id, (object) attributeValue11);
          foreach (FunXMPP.ProtocolTreeNode node1 in notifyNode.children ?? new FunXMPP.ProtocolTreeNode[0])
          {
            if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "message"))
            {
              FunXMPP.ProtocolTreeNode protocolTreeNode = node1;
              string attributeValue12 = protocolTreeNode.GetAttributeValue("id");
              string str = (string) null;
              byte[] buffer = (byte[]) null;
              string remote_jid;
              if (attributeValue11 == "status")
              {
                remote_jid = "status@broadcast";
                str = "0@s.whatsapp.net";
              }
              else
                remote_jid = "0@s.whatsapp.net";
              bool flag = false;
              int? attributeInt = protocolTreeNode.GetAttributeInt("order");
              protocolTreeNode.GetAttributeValue("id");
              Log.l("funxmpp", "received psa msg | id:{0},oder:{1}", (object) id, (object) (attributeInt ?? -1));
              if (attributeInt.HasValue)
              {
                if (attributeInt.Value > Settings.LastPSAReceived)
                {
                  Settings.LastPSAReceived = attributeInt.Value;
                }
                else
                {
                  flag = true;
                  Log.l("funxmpp", "psa already received | skip", (object) attributeInt.Value);
                }
              }
              if (!flag)
              {
                foreach (FunXMPP.ProtocolTreeNode node2 in protocolTreeNode.children ?? new FunXMPP.ProtocolTreeNode[0])
                {
                  if (FunXMPP.ProtocolTreeNode.TagEquals(node2, "media"))
                    buffer = node2?.data;
                }
                if (buffer == null)
                  buffer = protocolTreeNode?.data;
                if (buffer != null)
                {
                  FunXMPP.FMessage fmessage = new FunXMPP.FMessage(new FunXMPP.FMessage.Key(remote_jid, false, attributeValue12));
                  fmessage.timestamp = nullable;
                  fmessage.wants_receipt = false;
                  fmessage.notify_mute = attributeValue11;
                  fmessage.remote_resource = str;
                  WhatsApp.ProtoBuf.Message.Deserialize(buffer).PopulateFMessage(fmessage);
                  this.EventHandler.StoreIncomingMessage(fmessage);
                }
              }
            }
          }
          if (action != null)
            action();
        }
        else if (StringComparer.Ordinal.Equals(type, "pay") && PaymentsSettings.IsPaymentsEnabled())
        {
          FunXMPP.Connection.PaymentsTransactionUpdate transactionUpdate = FunXMPP.Connection.PaymentsTransactionUpdate.ExtractTransactionUpdate(notifyNode);
          if (transactionUpdate != null)
            PaymentsHelper.OnTransactionUpdate(transactionUpdate);
        }
        else if (StringComparer.Ordinal.Equals(type, "location"))
        {
          foreach (FunXMPP.ProtocolTreeNode node in notifyNode.children ?? new FunXMPP.ProtocolTreeNode[0])
          {
            if (FunXMPP.ProtocolTreeNode.TagEquals(node, "enc"))
            {
              Action ack = ackAction;
              ackAction = (Action) null;
              int? attributeInt = node.GetAttributeInt("v");
              string attributeValue13 = node.GetAttributeValue("type");
              int valueOrDefault = node.GetAttributeInt("count").GetValueOrDefault();
              byte[] data = node.data;
              this.EventHandler.OnLocationKeyForMe(from, participant, attributeInt.GetValueOrDefault(), attributeValue13, valueOrDefault, data, ack);
            }
            else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "encrypt"))
            {
              if (node.GetChild("deny") == null)
              {
                if (LiveLocationManager.Instance.IsSharingLocationWithParticipant(from))
                  LiveLocationManager.Instance.SendEnableLocationSharing(new List<string>()
                  {
                    from
                  });
                else
                  this.SendLiveLocationKeyDeny(from);
              }
              else
                this.EventHandler.OnLocationKeyDenyForMe(from);
            }
            else if (node.tag == "disable")
              LiveLocationManager.Instance.ReceiveLocationDisabled(from, from.IsGroupJid() ? participant : from);
          }
        }
        else if (StringComparer.Ordinal.Equals(type, "business"))
        {
          FunXMPP.ProtocolTreeNode child1 = notifyNode.GetChild("verified_name");
          if (child1 != null)
          {
            string attributeValue14 = child1.GetAttributeValue("hash");
            if (attributeValue14 != null)
            {
              if (this.EventHandler != null)
              {
                Action<bool> sidelistNotificationAck = this.CreateSidelistNotificationAck(from, to, participant, id, type);
                ackAction = (Action) null;
                this.EventHandler.OnSidelistNotification(attributeValue14, UsyncQuery.UsyncProtocol.Business, sidelistNotificationAck);
              }
            }
            else
            {
              string attributeValue15 = child1.GetAttributeValue("jid");
              string attributeValue16 = child1.GetAttributeValue("verified_level");
              string attributeValue17 = child1.GetAttributeValue("serial");
              ulong? serial = new ulong?();
              if (attributeValue17 != null)
              {
                ulong result;
                ulong.TryParse(attributeValue17, out result);
                serial = new ulong?(result);
              }
              byte[] data = child1.data;
              if (child1.GetAttributeValue("v") == "1" && JidHelper.IsUserJid(attributeValue15))
              {
                Action onCompletion = ackAction;
                ackAction = (Action) null;
                try
                {
                  if (data == null || data.Length == 0)
                  {
                    VerifiedNamesCertifier.OnVerifiedLevelNotification(attributeValue15, serial, attributeValue16);
                    ackAction = onCompletion;
                  }
                  else
                    VerifiedNamesCertifier.OnCertificateNotification(attributeValue15, serial, attributeValue16, data, onCompletion);
                }
                catch (Exception ex)
                {
                  string context = "funxmpp exception processing biz notification for " + attributeValue15;
                  Log.l(ex, context);
                  ackAction = onCompletion;
                }
              }
              else
                Log.l("funxmpp", "Unsupported version/jid for biz notification {0} {1}", (object) child1.GetAttributeValue("v"), (object) attributeValue15);
            }
          }
          else
          {
            FunXMPP.ProtocolTreeNode child2 = notifyNode.GetChild("profile");
            if (child2 != null)
            {
              string attributeValue18 = child2.GetAttributeValue("hash");
              if (attributeValue18 != null)
              {
                if (this.EventHandler != null)
                {
                  Action<bool> sidelistNotificationAck = this.CreateSidelistNotificationAck(from, to, participant, id, type);
                  ackAction = (Action) null;
                  this.EventHandler.OnSidelistNotification(attributeValue18, UsyncQuery.UsyncProtocol.Business, sidelistNotificationAck);
                }
              }
              else
              {
                string attributeValue19 = child2.GetAttributeValue("jid");
                BizProfileDetails profileDetails = BizProfileDetails.ExtractProfileDetails(child2);
                Action onCompletion = ackAction;
                ackAction = (Action) null;
                try
                {
                  VerifiedNamesCertifier.OnProfileNotification(attributeValue19, profileDetails, onCompletion);
                }
                catch (Exception ex)
                {
                  string context = "funxmpp exception processing biz profile notification for " + attributeValue19;
                  Log.l(ex, context);
                  ackAction = onCompletion;
                }
              }
            }
            else
            {
              FunXMPP.ProtocolTreeNode child3 = notifyNode.GetChild("remove");
              if (child3 != null)
              {
                string attributeValue20 = child3.GetAttributeValue("hash");
                if (attributeValue20 != null)
                {
                  if (this.EventHandler != null)
                  {
                    Action<bool> sidelistNotificationAck = this.CreateSidelistNotificationAck(from, to, participant, id, type);
                    ackAction = (Action) null;
                    this.EventHandler.OnSidelistNotification(attributeValue20, UsyncQuery.UsyncProtocol.Business, sidelistNotificationAck);
                  }
                }
                else
                  VerifiedNamesCertifier.RemoveUsersBusinessDetails(child3.GetAttributeValue("jid"));
              }
            }
          }
        }
        else if (StringComparer.Ordinal.Equals(type, "vname_check"))
        {
          FunXMPP.ProtocolTreeNode child = notifyNode.GetChild("vname_check");
          if (child != null)
          {
            string attributeValue21 = child.GetAttributeValue("jid");
            string dataString = child.GetChild("name")?.GetDataString();
            if (attributeValue21 == null || dataString == null)
            {
              Log.l("funxmpp", "Unsupported vname/jid for biz vname_check notification {0} {1}", (object) (dataString ?? "null"), (object) (attributeValue21 ?? "null"));
            }
            else
            {
              Action onCompletion = ackAction;
              ackAction = (Action) null;
              try
              {
                VerifiedNamesCertifier.OnVnameCheckNotification(attributeValue21, dataString, from, onCompletion);
              }
              catch (Exception ex)
              {
                string context = "funxmpp exception processing vname_check notification for " + attributeValue21;
                Log.l(ex, context);
                ackAction = onCompletion;
              }
            }
          }
        }
        else if (StringComparer.Ordinal.Equals(type, "gdpr"))
        {
          Log.l("gdpr", "process report notification");
          FunXMPP.ProtocolTreeNode child = notifyNode.GetChild("document");
          if (child == null)
          {
            Log.l("gdpr", "missing doc node from notification");
            Log.SendCrashLog((Exception) new InvalidOperationException("gdpr notification without required data"), "gdpr notification");
          }
          else
          {
            Action ack = ackAction;
            ackAction = (Action) null;
            this.EventHandler?.OnGdprReportReady(child.GetAttributeLong("creation"), child.GetAttributeLong("expiration"), child.data, true, ack);
          }
        }
        else
          Log.l("funxmpp", "unrecognized notification type: {0}", (object) (type ?? "<null>"));
        if (ackAction == null)
          return;
        ackAction();
      }

      private Action<bool> CreateSidelistNotificationAck(
        string from,
        string to,
        string participant,
        string id,
        string type)
      {
        return (Action<bool>) (found =>
        {
          string to1 = from;
          string from1 = to;
          string participant1 = participant;
          string id1 = id;
          string type1 = type;
          FunXMPP.ProtocolTreeNode[] extraNodes;
          if (found)
            extraNodes = (FunXMPP.ProtocolTreeNode[]) null;
          else
            extraNodes = new FunXMPP.ProtocolTreeNode[1]
            {
              new FunXMPP.ProtocolTreeNode("sync", new FunXMPP.KeyValue[1]
              {
                new FunXMPP.KeyValue("side_list", "out")
              })
            };
          this.SendNotificationReceived(to1, from1, participant1, id1, type1, (IEnumerable<FunXMPP.ProtocolTreeNode>) extraNodes);
        });
      }

      public static bool TryParseFeatureNode(
        FunXMPP.ProtocolTreeNode node,
        out ClientCapabilityCategory type,
        out ClientCapabilitySetting setting)
      {
        type = ClientCapabilityCategory.None;
        setting = ClientCapabilitySetting.None;
        switch (node.GetAttributeValue("value"))
        {
          case "allow":
            setting = ClientCapabilitySetting.Allowed;
            break;
          case "disable":
            setting = ClientCapabilitySetting.Disabled;
            break;
          case "upgrade":
            setting = ClientCapabilitySetting.Upgrade;
            break;
          case "forward":
            setting = ClientCapabilitySetting.Forward;
            break;
          case "none":
            setting = ClientCapabilitySetting.None;
            break;
        }
        switch (node.tag)
        {
          case "document":
            type = ClientCapabilityCategory.Document;
            break;
          case "encrypt":
            type = ClientCapabilityCategory.Encrypt;
            break;
          case "encrypt_audio":
            type = ClientCapabilityCategory.EncryptAudio;
            break;
          case "encrypt_blist":
            type = ClientCapabilityCategory.EncryptBroadcast;
            break;
          case "encrypt_contact":
            type = ClientCapabilityCategory.EncryptContact;
            break;
          case "encrypt_group_gen2":
            type = ClientCapabilityCategory.EncryptGroupGen2;
            break;
          case "encrypt_image":
            type = ClientCapabilityCategory.EncryptImage;
            break;
          case "encrypt_location":
            type = ClientCapabilityCategory.EncryptLocation;
            break;
          case "encrypt_v2":
            type = ClientCapabilityCategory.EncryptV2;
            break;
          case "encrypt_video":
            type = ClientCapabilityCategory.EncryptVideo;
            break;
          case "identity_verification":
            type = ClientCapabilityCategory.IdentityVerification;
            break;
        }
        return type != 0;
      }

      private void ParseSubjectAttributes(
        FunXMPP.ProtocolTreeNode node,
        out string subject,
        out string subjectOwner,
        out DateTime? subjectTime)
      {
        subject = node.GetAttributeValue(nameof (subject));
        subjectOwner = node.GetAttributeValue("s_o");
        subjectTime = node.GetAttributeDateTime("s_t");
      }

      private IEnumerable<string> ParseJidsFromGroupNotification(FunXMPP.ProtocolTreeNode node)
      {
        return node.GetAllChildren("participant").Select<FunXMPP.ProtocolTreeNode, string>((Func<FunXMPP.ProtocolTreeNode, string>) (p => p.GetAttributeValue("jid"))).Where<string>((Func<string, bool>) (j => j != null));
      }

      private void ProcessGroupNotification(
        FunXMPP.ProtocolTreeNode node,
        string gjid,
        string participant,
        DateTime? timestamp,
        string pushName)
      {
        string subject1;
        string subjectOwner;
        DateTime? subjectTime;
        DateTime? nullable;
        foreach (FunXMPP.ProtocolTreeNode node1 in node.children ?? new FunXMPP.ProtocolTreeNode[0])
        {
          if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "create"))
          {
            FunXMPP.ProtocolTreeNode child = node1.GetChild("group");
            if (child != null)
              this.GroupEventHandler.OnGroupWelcome(new FunXMPP.Connection.GroupCreationEventArgs()
              {
                Info = this.ParseGroupInfo(child),
                Timestamp = timestamp,
                InviterJid = participant,
                CreationKey = node1.GetAttributeValue("key"),
                IsNew = node1.GetAttributeValue("type") == "new"
              }, node1.GetAttributeValue("reason") == "invite");
          }
          else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "subject"))
          {
            this.ParseSubjectAttributes(node1, out subject1, out subjectOwner, out subjectTime);
            FunXMPP.GroupListener groupEventHandler = this.GroupEventHandler;
            string gjid1 = gjid;
            string ujid = participant;
            string pushName1 = pushName;
            string subject2 = subject1;
            nullable = subjectTime;
            DateTime? timestamp1 = new DateTime?(nullable ?? timestamp ?? FunRunner.CurrentServerTimeUtc);
            groupEventHandler.OnGroupNewSubject(gjid1, ujid, pushName1, subject2, timestamp1);
          }
          else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "description"))
          {
            string attributeValue = node1.GetAttributeValue("id");
            FunXMPP.ProtocolTreeNode child = node1.GetChild("body");
            string dataString = node1.GetChild("delete") != null ? (string) null : child?.GetDataString();
            FunXMPP.GroupListener groupEventHandler = this.GroupEventHandler;
            string gjid2 = gjid;
            string ujid = participant;
            string id = attributeValue;
            string description = dataString;
            nullable = timestamp;
            DateTime? timestamp2 = new DateTime?(nullable ?? FunRunner.CurrentServerTimeUtc);
            groupEventHandler.OnGroupNewDescription(gjid2, ujid, id, description, timestamp2);
          }
          else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "locked"))
            this.GroupEventHandler.OnGroupLocked(gjid, participant, timestamp);
          else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "unlocked"))
            this.GroupEventHandler.OnGroupUnlocked(gjid, participant, timestamp);
          else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "announcement"))
            this.GroupEventHandler.OnGroupAnnounceOnly(gjid, participant, timestamp);
          else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "not_announcement"))
            this.GroupEventHandler.OnGroupNotAnnounceOnly(gjid, participant, timestamp);
          else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "add"))
          {
            string attributeValue = node1.GetAttributeValue("reason");
            foreach (string jid in this.ParseJidsFromGroupNotification(node1))
              this.GroupEventHandler.OnGroupAddUser(gjid, jid, participant, timestamp, attributeValue);
          }
          else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "remove"))
          {
            this.ParseSubjectAttributes(node1, out subject1, out subjectOwner, out subjectTime);
            foreach (string jid in this.ParseJidsFromGroupNotification(node1))
              this.GroupEventHandler.OnGroupRemoveUser(gjid, jid, participant, timestamp, subject1);
          }
          else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "promote"))
            this.GroupEventHandler.OnPromoteUsers(gjid, this.ParseJidsFromGroupNotification(node1), timestamp);
          else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "demote"))
            this.GroupEventHandler.OnDemoteUsers(gjid, this.ParseJidsFromGroupNotification(node1), timestamp);
          else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "modify"))
          {
            string oldJid = participant;
            string newJid = this.ParseJidsFromGroupNotification(node1).FirstOrDefault<string>();
            this.GroupEventHandler.OnGroupParticipantChangeNumber(gjid, oldJid, newJid, timestamp);
          }
          else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "delete"))
            this.GroupEventHandler.OnGroupDisbanded(gjid, participant, timestamp);
          else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "invite"))
          {
            string attributeValue = node1.GetAttributeValue("code");
            this.GroupEventHandler.OnInvitationCode(gjid, participant, attributeValue, timestamp);
          }
        }
      }

      private void PreparseMessage(
        FunXMPP.FMessage.Builder builder,
        FunXMPP.ProtocolTreeNode messageNode)
      {
        string attributeValue1 = messageNode.GetAttributeValue("notify");
        if (!string.IsNullOrEmpty(attributeValue1))
          builder.Push_name(attributeValue1);
        string attributeValue2 = messageNode.GetAttributeValue("t");
        DateTime? nullable = new DateTime?();
        ref DateTime? local = ref nullable;
        if (FunXMPP.TryParseTimestamp(attributeValue2, out local) && nullable.HasValue)
          builder.Timestamp(new DateTime?(nullable.Value));
        string attributeValue3 = messageNode.GetAttributeValue("offline");
        int result1;
        if (attributeValue3 != null && int.TryParse(attributeValue3, out result1))
          builder.Offline(result1);
        else
          builder.Offline(-1);
        string attributeValue4 = messageNode.GetAttributeValue("verified_name");
        if (attributeValue4 == null)
          return;
        ulong result2;
        ulong.TryParse(attributeValue4, out result2);
        builder.VerifiedName(result2);
        string attributeValue5 = messageNode.GetAttributeValue("verified_level");
        if (attributeValue5 == null)
          return;
        builder.VerifiedLevel(attributeValue5);
      }

      private bool ParseMessageCore(
        FunXMPP.FMessage.Builder builder,
        FunXMPP.ProtocolTreeNode messageNode,
        FunXMPP.ProtocolTreeNode childNode,
        bool from_me,
        string jid,
        string author,
        string id)
      {
        if (FunXMPP.ProtocolTreeNode.TagEquals(childNode, "enc") && id != null)
        {
          string attributeValue1 = childNode.GetAttributeValue("oid");
          FunXMPP.FMessage.Key key = new FunXMPP.FMessage.Key(jid, from_me, attributeValue1 ?? id);
          builder.Key(key);
          if (author != null)
            builder.Remote_resource(author);
          builder.NewIncomingInstance();
          int? attributeInt1 = childNode.GetAttributeInt("v");
          string attributeValue2 = childNode.GetAttributeValue("type");
          string attributeValue3 = childNode.GetAttributeValue("mediatype");
          byte[] data = childNode.data;
          string attributeValue4 = childNode.GetAttributeValue("mediareason");
          if (!attributeInt1.HasValue || attributeValue2 == null || data == null)
          {
            if (!attributeInt1.HasValue)
              Log.l("E2EDecrypt", "CipherVersion missing from enc node, jid: " + key.remote_jid + " keyid: " + key.id);
            if (attributeValue2 == null)
              Log.l("E2EDecrypt", "CipherType missing from enc node, jid: " + key.remote_jid + " keyid: " + key.id);
            if (data == null)
              Log.l("E2EDecrypt", "CipherBytes missing from enc node, jid: " + key.remote_jid + " keyid: " + key.id);
            Log.SendCrashLog((Exception) new Axolotl.AxolotlProtocolBufferException(), "Send encoding node with missing parameters", false, false);
          }
          if (attributeInt1.HasValue)
          {
            builder.Encrypted(new FunXMPP.FMessage.Encrypted()
            {
              cipher_version = attributeInt1.Value,
              cipher_text_type = attributeValue2,
              cipher_retry_count = childNode.GetAttributeInt("count").GetValueOrDefault(),
              cipher_text_bytes = data
            });
            switch (attributeValue4)
            {
              case "retry":
                builder.Mms_retry(true);
                break;
            }
            switch (attributeValue3)
            {
              case "livelocation":
                int? attributeInt2 = childNode.GetAttributeInt("duration");
                if (attributeInt2.HasValue)
                {
                  builder.media_duration_seconds = attributeInt2;
                  break;
                }
                break;
            }
            DateTime? attributeDateTime = childNode.GetAttributeDateTime("ot");
            if (attributeDateTime.HasValue)
              builder.Timestamp(attributeDateTime);
          }
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(childNode, "registration") && id != null)
          builder.registrationId = new uint?(FunXMPP.Connection.UIntFromBytes(childNode.data));
        else if (FunXMPP.ProtocolTreeNode.TagEquals(childNode, "multicast"))
          builder.multicast = new bool?(true);
        else if (FunXMPP.ProtocolTreeNode.TagEquals(childNode, "verified_name"))
          builder.verified_name_certificate = childNode.data;
        else if (FunXMPP.ProtocolTreeNode.TagEquals(childNode, "pay"))
        {
          string attributeValue5 = childNode.GetAttributeValue("type");
          string attributeValue6 = childNode.GetAttributeValue("currency");
          string attributeValue7 = childNode.GetAttributeValue("amount");
          string attributeValue8 = childNode.GetAttributeValue("receiver");
          MessageProperties msgProps = builder.MessageProps() ?? new MessageProperties();
          if (msgProps.PaymentsPropertiesField == null)
            msgProps.PaymentsPropertiesField = new MessageProperties.PaymentsProperties();
          msgProps.PaymentsPropertiesField.Amount = attributeValue7;
          msgProps.PaymentsPropertiesField.Currency = attributeValue6;
          msgProps.PaymentsPropertiesField.PayType = new MessageProperties.PaymentsProperties.PayTypes?(MessageProperties.PaymentsProperties.ConvertToPayType(attributeValue5));
          msgProps.PaymentsPropertiesField.Receiver = attributeValue8;
          if (PaymentsSettings.IsPaymentsEnabled())
            PaymentsHelper.ProcessIncomingPayment(msgProps.PaymentsPropertiesField, jid, author, id);
          builder.MessageProperties(msgProps);
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(childNode, "body") || FunXMPP.ProtocolTreeNode.TagEquals(childNode, "media"))
          return false;
        return true;
      }

      private void PostparseMessage(FunXMPP.FMessage.Builder builder)
      {
        if (builder.Timestamp().HasValue)
          return;
        builder.Timestamp(new DateTime?(DateTime.Now));
      }

      internal void ProcessBroadcastListsQueryResults(FunXMPP.ProtocolTreeNode node)
      {
        foreach (FunXMPP.ProtocolTreeNode allChild in node.GetChild("lists").GetAllChildren("list"))
          this.GroupEventHandler.OnBroadcastListInfo(allChild.GetAttributeValue("id"), allChild.GetAttributeValue("name"), allChild.GetAllChildren("recipient").Select<FunXMPP.ProtocolTreeNode, string>((Func<FunXMPP.ProtocolTreeNode, string>) (rNode => rNode.GetAttributeValue("jid"))));
      }

      private FunXMPP.Connection.GroupInfo ParseGroupInfo(FunXMPP.ProtocolTreeNode groupNode)
      {
        FunXMPP.Connection.GroupInfo groupInfo = new FunXMPP.Connection.GroupInfo()
        {
          Jid = JidHelper.GroupId2Jid(groupNode.GetAttributeValue("id")),
          CreatorJid = groupNode.GetAttributeValue("creator"),
          Subject = groupNode.GetAttributeValue("subject"),
          SubjectTime = groupNode.GetAttributeDateTime("s_t"),
          SubjectOwnerJid = groupNode.GetAttributeValue("s_o"),
          CreationTime = groupNode.GetAttributeDateTime("creation"),
          AnnouncementOnly = groupNode.GetChild("announcement") != null,
          Locked = groupNode.GetChild("locked") != null
        };
        List<string> stringList1 = new List<string>();
        List<string> stringList2 = new List<string>();
        FunXMPP.ProtocolTreeNode child = groupNode.GetChild("description");
        if (child != null)
        {
          if (child.GetChild("body") != null)
            groupInfo.Description = new GroupDescription(child.GetChild("body").GetDataString(), child.GetAttributeValue("id"))
            {
              CreateTime = child.GetAttributeDateTime("t"),
              Owner = child.GetAttributeValue("participant")
            };
          else
            groupInfo.Description = (GroupDescription) null;
        }
        foreach (FunXMPP.ProtocolTreeNode allChild in groupNode.GetAllChildren("participant"))
        {
          string attributeValue1 = allChild.GetAttributeValue("jid");
          if (attributeValue1 != null)
          {
            string attributeValue2 = allChild.GetAttributeValue("type");
            ((attributeValue2 == "admin" ? 1 : (attributeValue2 == "superadmin" ? 1 : 0)) != 0 ? stringList1 : stringList2).Add(attributeValue1);
            if (attributeValue2 == "superadmin")
              groupInfo.SuperAdmin = attributeValue1;
          }
        }
        groupInfo.AdminJids = stringList1;
        groupInfo.NonadminJids = stringList2;
        return groupInfo;
      }

      internal void ReadAttributeList(
        FunXMPP.ProtocolTreeNode node,
        List<string> list,
        string tag,
        string attribute)
      {
        list.AddRange(node.GetAllChildren(tag).Select<FunXMPP.ProtocolTreeNode, string>((Func<FunXMPP.ProtocolTreeNode, string>) (tagNode => tagNode.GetAttributeValue(attribute))));
      }

      internal void ParseParticipantResults(
        FunXMPP.ProtocolTreeNode iqNode,
        string childTag,
        List<string> successList,
        List<Pair<string, int>> failList)
      {
        FunXMPP.ProtocolTreeNode child = iqNode.GetChild(childTag);
        if (child == null)
          return;
        child.GetAllChildren("participant").ToList<FunXMPP.ProtocolTreeNode>().ForEach((Action<FunXMPP.ProtocolTreeNode>) (pNode =>
        {
          string attributeValue1 = pNode.GetAttributeValue("jid");
          if (string.IsNullOrEmpty(attributeValue1))
            return;
          string attributeValue2 = pNode.GetAttributeValue("error");
          if (attributeValue2 == null)
          {
            successList.Add(attributeValue1);
          }
          else
          {
            int result = 499;
            if (!int.TryParse(attributeValue2, FunXMPP.WhatsAppNumberStyle, (IFormatProvider) CultureInfo.InvariantCulture, out result))
              result = 499;
            failList.Add(new Pair<string, int>(attributeValue1, result));
          }
        }));
      }

      private void AddNotificationHandler(string id, Action onAck, Action onNoAck)
      {
        FunXMPP.Connection.NotificationAckHandler notificationAckHandler = new FunXMPP.Connection.NotificationAckHandler(onAck, onNoAck, id);
        lock (this.notificationHandlerLock)
          this.pendingNotificationServerRequests.Add(id, notificationAckHandler);
      }

      private bool TryToAckNotificationReceived(string id)
      {
        FunXMPP.Connection.NotificationAckHandler notificationAckHandler = (FunXMPP.Connection.NotificationAckHandler) null;
        lock (this.notificationHandlerLock)
        {
          if (this.pendingNotificationServerRequests.TryGetValue(id, out notificationAckHandler))
            this.pendingNotificationServerRequests.Remove(id);
        }
        return notificationAckHandler != null && notificationAckHandler.OnAck();
      }

      private void NoAckAllPendingNotifications()
      {
        FunXMPP.Connection.NotificationAckHandler[] notificationAckHandlerArray = (FunXMPP.Connection.NotificationAckHandler[]) null;
        lock (this.notificationHandlerLock)
        {
          notificationAckHandlerArray = this.pendingNotificationServerRequests.Values.ToArray<FunXMPP.Connection.NotificationAckHandler>();
          this.pendingNotificationServerRequests.Clear();
        }
        foreach (FunXMPP.Connection.NotificationAckHandler notificationAckHandler in notificationAckHandlerArray ?? new FunXMPP.Connection.NotificationAckHandler[0])
          notificationAckHandler.OnNoAck();
      }

      private void SendQrIq(
        string prefix,
        FunXMPP.ProtocolTreeNode[] innerNodes,
        FunXMPP.IqResultHandler handler)
      {
        string str = this.MakeId(prefix);
        this.AddIqHandler(str, handler);
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[3]
        {
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue("id", str),
          new FunXMPP.KeyValue("xmlns", "w:web")
        }, innerNodes));
      }

      private FunXMPP.ProtocolTreeNode EncryptNode(
        FunXMPP.ProtocolTreeNode innerNode,
        QrMetricsMapping? metrics)
      {
        FunXMPP.KeyValue[] attrs = (FunXMPP.KeyValue[]) null;
        if (metrics.HasValue)
          attrs = new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("type", ((char) metrics.Value).ToString())
          };
        return new FunXMPP.ProtocolTreeNode("enc", attrs, this.GetQrCrypto().CreateBlob(innerNode).ToArray());
      }

      private void SendQrIqEncrypted(
        string prefix,
        FunXMPP.ProtocolTreeNode innerNode,
        QrMetricsMapping? metrics,
        FunXMPP.IqResultHandler handler)
      {
        this.EventHandler.Qr.Session.Synchronize((Action) (() => this.SendQrIqEncryptedImpl(prefix, innerNode, metrics, handler)));
      }

      private void SendQrIqEncryptedImpl(
        string prefix,
        FunXMPP.ProtocolTreeNode innerNode,
        QrMetricsMapping? metrics,
        FunXMPP.IqResultHandler handler)
      {
        FunXMPP.ProtocolTreeNode protocolTreeNode = this.EncryptNode(innerNode, metrics);
        if (protocolTreeNode == null)
          return;
        this.SendQrIq(prefix, new FunXMPP.ProtocolTreeNode[1]
        {
          protocolTreeNode
        }, handler);
      }

      private void SendQrAction(
        string prefix,
        FunXMPP.KeyValue[] attributes,
        FunXMPP.ProtocolTreeNode[] actions,
        QrMetricsMapping? metrics,
        FunXMPP.IqResultHandler handler)
      {
        FunXMPP.ProtocolTreeNode innerNode = new FunXMPP.ProtocolTreeNode("action", attributes, actions);
        this.SendQrIqEncrypted(prefix, innerNode, metrics, handler);
      }

      private void SendQrAction(
        string prefix,
        FunXMPP.KeyValue[] attributes,
        FunXMPP.ProtocolTreeNode action,
        QrMetricsMapping? metrics,
        FunXMPP.IqResultHandler handler)
      {
        this.SendQrAction(prefix, attributes, new FunXMPP.ProtocolTreeNode[1]
        {
          action
        }, metrics, handler);
      }

      public void SendQrSync(
        byte[] sessionData,
        byte[] token = null,
        bool response = false,
        byte[] password = null,
        Action<string, string, bool> onComplete = null,
        Action<int> onError = null)
      {
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          FunXMPP.ProtocolTreeNode child1 = node.GetChild(0);
          FunXMPP.ProtocolTreeNode child2 = child1.GetChild(0);
          FunXMPP.ProtocolTreeNode child3 = child1.GetChild("timeout");
          string attributeValue1 = child2.GetAttributeValue("os");
          string attributeValue2 = child2.GetAttributeValue("browser");
          bool flag = child3 > null;
          if (onComplete == null)
            return;
          onComplete(attributeValue1, attributeValue2, flag);
        }), onError ?? (Action<int>) (err => { }));
        List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList = new List<FunXMPP.ProtocolTreeNode>();
        List<FunXMPP.KeyValue> keyValueList1 = new List<FunXMPP.KeyValue>();
        List<FunXMPP.KeyValue> keyValueList2 = new List<FunXMPP.KeyValue>();
        if (response)
          keyValueList1.Add(new FunXMPP.KeyValue("web", nameof (response)));
        keyValueList1.Add(new FunXMPP.KeyValue("version", "0.17.10"));
        ClientPayload.WebInfo.WebdPayload webdPayload = QrSession.WebdPayload;
        string[] strArray = new string[8]
        {
          "participant",
          webdPayload.UsesParticipantInKey.ToWebClientBool(),
          "favorites",
          webdPayload.SupportsStarredMessages.ToWebClientBool(),
          "url",
          webdPayload.SupportsUrlMessages.ToWebClientBool(),
          "retry",
          webdPayload.SupportsMediaRetry.ToWebClientBool()
        };
        for (int index = 0; index < strArray.Length; index += 2)
          keyValueList1.Add(new FunXMPP.KeyValue(strArray[index], strArray[index + 1]));
        protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("sync", keyValueList1.ToArray(), sessionData));
        if (password != null)
          protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode(nameof (password), (FunXMPP.KeyValue[]) null, Convert.ToBase64String(password)));
        if (token != null)
          protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("code", (FunXMPP.KeyValue[]) null, Encoding.UTF8.GetBytes(Convert.ToBase64String(token))));
        byte[] serializedWebFeatures = QrSession.getSerializedWebFeatures();
        if (strArray != null)
          protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("features", (FunXMPP.KeyValue[]) null, serializedWebFeatures));
        foreach (FunXMPP.ProtocolTreeNode protocolTreeNode in this.NodesFromSyncResponse(this.EventHandler.Qr.GetSync()))
          protocolTreeNodeList.Add(protocolTreeNode);
        this.SendQrIq("qrsync_", protocolTreeNodeList.ToArray(), handler);
      }

      public void SendQrUnsync(bool destroy, Action onComplete = null, Action<int> onError = null)
      {
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (onComplete == null)
            return;
          onComplete();
        }), onError ?? (Action<int>) (err => { }));
        FunXMPP.KeyValue[] attrs = (FunXMPP.KeyValue[]) null;
        if (!destroy)
          attrs = new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("type", "Replaced by new connection")
          };
        this.SendQrIq("qrunsync_", new FunXMPP.ProtocolTreeNode[1]
        {
          new FunXMPP.ProtocolTreeNode("delete", attrs)
        }, handler);
      }

      public void SendQrMessages(
        IEnumerable<Message> msgs,
        QrMessageForwardType msgType,
        Action onComplete = null,
        Action<int> onError = null,
        bool invis = false,
        bool resume = false,
        FunXMPP.FMessage.Key unreadKey = null)
      {
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (onComplete == null)
            return;
          onComplete();
        }), onError ?? (Action<int>) (err => { }));
        string v = "relay";
        QrMetricsMapping metrics = QrMetricsMapping.FORWARD_MESSAGE;
        switch (msgType)
        {
          case QrMessageForwardType.MessageHistory:
            v = "before";
            metrics = QrMetricsMapping.FORWARD_MESSAGE_HISTORY;
            break;
          case QrMessageForwardType.LastMessages:
            v = "last";
            metrics = QrMetricsMapping.FORWARD_MESSAGE_HISTORY;
            break;
          case QrMessageForwardType.MessagesSinceReconnect:
            v = "after";
            metrics = QrMetricsMapping.FORWARD_MESSAGE_HISTORY;
            break;
          case QrMessageForwardType.Update:
            v = "update";
            metrics = QrMetricsMapping.FORWARD_MESSAGE;
            break;
          case QrMessageForwardType.Unread:
            v = "unread";
            metrics = QrMetricsMapping.FORWARD_MESSAGE_HISTORY;
            break;
        }
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>()
        {
          new FunXMPP.KeyValue("add", v)
        };
        if (msgType == QrMessageForwardType.Unread && unreadKey != null)
        {
          keyValueList.Add(new FunXMPP.KeyValue("index", unreadKey.id));
          keyValueList.Add(new FunXMPP.KeyValue("owner", unreadKey.from_me ? "true" : "false"));
        }
        if (resume)
          keyValueList.Add(new FunXMPP.KeyValue(nameof (resume), "true"));
        this.SendQrAction("qrmsg_", keyValueList.ToArray(), msgs != null ? FunXMPP.Connection.QrSerializeMessages(msgs, ref metrics, invis).ToArray<FunXMPP.ProtocolTreeNode>() : (FunXMPP.ProtocolTreeNode[]) null, new QrMetricsMapping?(metrics), handler);
      }

      public void SendQrReceived(
        FunXMPP.FMessage.Key key,
        FunXMPP.FMessage.Status status,
        Action onComplete = null,
        Action<int> onError = null)
      {
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (onComplete == null)
            return;
          onComplete();
        }), onError ?? (Action<int>) (err => { }));
        string v = "message";
        switch (status)
        {
          case FunXMPP.FMessage.Status.ReceivedByTarget:
            v = "message";
            break;
          case FunXMPP.FMessage.Status.Error:
          case FunXMPP.FMessage.Status.Canceled:
            v = "error";
            break;
          case FunXMPP.FMessage.Status.PlayedByTarget:
            v = "played";
            break;
          case FunXMPP.FMessage.Status.ReadByTarget:
            v = "read";
            break;
        }
        this.SendQrAction("qrreceived_", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("received", new FunXMPP.KeyValue[4]
        {
          new FunXMPP.KeyValue("index", key.id),
          new FunXMPP.KeyValue("jid", key.remote_jid),
          new FunXMPP.KeyValue("type", v),
          new FunXMPP.KeyValue("owner", key.from_me ? "true" : "false")
        }), new QrMetricsMapping?(QrMetricsMapping.FORWARD_ACK), handler);
      }

      public void SendQrChatStatus(
        string jid,
        FunXMPP.ChatStatusForwardAction action,
        DateTime? actionTime,
        Action onComplete = null,
        Action<int> onError = null)
      {
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (onComplete == null)
            return;
          onComplete();
        }), onError ?? (Action<int>) (err => { }));
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>()
        {
          new FunXMPP.KeyValue(nameof (jid), jid)
        };
        switch (action)
        {
          case FunXMPP.ChatStatusForwardAction.Mute:
            keyValueList.Add(new FunXMPP.KeyValue("type", "mute"));
            if (actionTime.HasValue)
            {
              keyValueList.Add(new FunXMPP.KeyValue("mute", actionTime.Value.ToUnixTime().ToString()));
              break;
            }
            break;
          case FunXMPP.ChatStatusForwardAction.Archive:
            keyValueList.Add(new FunXMPP.KeyValue("type", "archive"));
            break;
          case FunXMPP.ChatStatusForwardAction.Unarchive:
            keyValueList.Add(new FunXMPP.KeyValue("type", "unarchive"));
            break;
          case FunXMPP.ChatStatusForwardAction.Delete:
            keyValueList.Add(new FunXMPP.KeyValue("type", "delete"));
            break;
          case FunXMPP.ChatStatusForwardAction.Clear:
            keyValueList.Add(new FunXMPP.KeyValue("type", "clear"));
            keyValueList.Add(new FunXMPP.KeyValue("star", "true"));
            if (actionTime.HasValue)
            {
              keyValueList.Add(new FunXMPP.KeyValue("before", actionTime.Value.ToUnixTime().ToString()));
              break;
            }
            break;
          case FunXMPP.ChatStatusForwardAction.ModifyTag:
            keyValueList.Add(new FunXMPP.KeyValue("type", "modify_tag"));
            keyValueList.Add(new FunXMPP.KeyValue("modify_tag", MessagesContext.Select<string>((Func<MessagesContext, string>) (db => db.GetConversation(jid, CreateOptions.None)?.ModifyTag.ToString()))));
            break;
          case FunXMPP.ChatStatusForwardAction.ClearNotStarred:
            keyValueList.Add(new FunXMPP.KeyValue("type", "clear"));
            keyValueList.Add(new FunXMPP.KeyValue("star", "false"));
            break;
          case FunXMPP.ChatStatusForwardAction.NotSpam:
            keyValueList.Add(new FunXMPP.KeyValue("type", "spam"));
            keyValueList.Add(new FunXMPP.KeyValue("spam", "false"));
            break;
          case FunXMPP.ChatStatusForwardAction.Pin:
            keyValueList.Add(new FunXMPP.KeyValue("type", "pin"));
            if (actionTime.HasValue)
            {
              keyValueList.Add(new FunXMPP.KeyValue("pin", actionTime.Value.ToUnixTime().ToString()));
              break;
            }
            break;
        }
        this.SendQrAction("qrchatstatus_", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("chat", keyValueList.ToArray()), new QrMetricsMapping?(QrMetricsMapping.FORWARD_CHATS), handler);
      }

      public void SendQrContact(FunXMPP.ContactResponse c, Action onComplete = null, Action<int> onError = null)
      {
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (onComplete == null)
            return;
          onComplete();
        }), onError ?? (Action<int>) (err => { }));
        this.SendQrAction("qrcontact_", (FunXMPP.KeyValue[]) null, this.CreateContactNode(c, true), new QrMetricsMapping?(QrMetricsMapping.FORWARD_CONTACTS), handler);
      }

      public void SendQrRead(string jid, bool read, Action onComplete = null, Action<int> onError = null)
      {
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (onComplete == null)
            return;
          onComplete();
        }), onError ?? (Action<int>) (err => { }));
        this.SendQrAction("qrread_", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode(nameof (read), new FunXMPP.KeyValue[2]
        {
          new FunXMPP.KeyValue(nameof (jid), jid),
          new FunXMPP.KeyValue("type", read ? "true" : "false")
        }), new QrMetricsMapping?(QrMetricsMapping.FORWARD_CHAT_SEEN), handler);
      }

      public void SendQrSeen(
        string jid,
        string msgId,
        bool owner,
        string senderJid,
        Action onComplete = null,
        Action<int> onError = null)
      {
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (onComplete == null)
            return;
          onComplete();
        }), onError ?? (Action<int>) (err => { }));
        this.SendQrAction("qrseen_", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("read", new FunXMPP.KeyValue[6]
        {
          new FunXMPP.KeyValue("kind", "status"),
          new FunXMPP.KeyValue(nameof (jid), jid),
          new FunXMPP.KeyValue("index", msgId),
          new FunXMPP.KeyValue(nameof (owner), owner ? "true" : "false"),
          new FunXMPP.KeyValue("chat", senderJid),
          new FunXMPP.KeyValue("checksum", Settings.ContactsChecksum.ToString())
        }), new QrMetricsMapping?(QrMetricsMapping.FORWARD_CHAT_SEEN), handler);
      }

      public void SendQrRevokeMessage(
        FunXMPP.FMessage.Key key,
        string revokedId,
        string senderJid,
        long? messageTimestamp,
        Action onComplete = null,
        Action<int> onError = null)
      {
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          Action action = onComplete;
          if (action == null)
            return;
          action();
        }), onError ?? (Action<int>) (err => { }));
        WebMessageInfo instance = new WebMessageInfo()
        {
          Key = new MessageKey()
        };
        instance.Key.FromMe = new bool?(key.from_me);
        instance.Key.Id = key.id;
        instance.Key.RemoteJid = key.remote_jid;
        instance.MessageTimestamp = new ulong?((ulong) (messageTimestamp ?? DateTime.UtcNow.ToUnixTime()));
        instance.Participant = senderJid;
        instance.Message = new WhatsApp.ProtoBuf.Message();
        instance.Message.ProtocolMessageField = WhatsApp.ProtoBuf.Message.CreateRevokeProtocolMessage(key, revokedId, senderJid);
        Log.l("web client", "send revoke for message | jid={0}, id={1}, fromMe={2}", (object) key.remote_jid, (object) revokedId, (object) key.from_me);
        this.SendQrAction("qrrevokemsg_", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("add", "relay")
        }, new FunXMPP.ProtocolTreeNode("message", (FunXMPP.KeyValue[]) null, WebMessageInfo.SerializeToBytes(instance)), new QrMetricsMapping?(QrMetricsMapping.FORWARD_MESSAGE), handler);
      }

      public void SendQrDeleteMessage(
        FunXMPP.FMessage.Key key,
        int modifyTag,
        Action onComplete = null,
        Action<int> onError = null)
      {
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (onComplete == null)
            return;
          onComplete();
        }), onError ?? (Action<int>) (err => { }));
        this.SendQrAction("qrdeletemsg_", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("chat", new FunXMPP.KeyValue[3]
        {
          new FunXMPP.KeyValue("jid", key.remote_jid),
          new FunXMPP.KeyValue("type", "clear"),
          new FunXMPP.KeyValue("modify_tag", modifyTag.ToString())
        }, new FunXMPP.ProtocolTreeNode("item", new FunXMPP.KeyValue[2]
        {
          new FunXMPP.KeyValue("index", key.id),
          new FunXMPP.KeyValue("owner", key.from_me ? "true" : "false")
        })), new QrMetricsMapping?(QrMetricsMapping.FORWARD_MESSAGE_DELETE), handler);
      }

      public void SendQrStarredMessage(
        FunXMPP.FMessage.Key key,
        bool star,
        int modifyTag,
        Action onComplete = null,
        Action<int> onError = null)
      {
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (onComplete == null)
            return;
          onComplete();
        }), onError ?? (Action<int>) (err => { }));
        FunXMPP.ProtocolTreeNode child = (FunXMPP.ProtocolTreeNode) null;
        if (star)
        {
          Message m = (Message) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => m = db.GetMessage(key.remote_jid, key.id, key.from_me)));
          if (m != null)
            child = FunXMPP.Connection.QrSerializeMessage(m);
        }
        else
          child = new FunXMPP.ProtocolTreeNode("item", new FunXMPP.KeyValue[2]
          {
            new FunXMPP.KeyValue("index", key.id),
            new FunXMPP.KeyValue("owner", key.from_me ? "true" : "false")
          });
        this.SendQrAction("qrstarredmsg_", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("chat", new FunXMPP.KeyValue[3]
        {
          new FunXMPP.KeyValue("jid", key.remote_jid),
          new FunXMPP.KeyValue("type", star ? nameof (star) : "unstar"),
          new FunXMPP.KeyValue("modify_tag", modifyTag.ToString())
        }, child), new QrMetricsMapping?(QrMetricsMapping.FORWARD_MESSAGE_STAR), handler);
      }

      public void SendQrUnStarAll(
        string jid,
        int modifyTag,
        Action onComplete = null,
        Action<int> onError = null)
      {
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (onComplete == null)
            return;
          onComplete();
        }), onError ?? (Action<int>) (err => { }));
        this.SendQrAction("qrunstarall_", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("chat", new FunXMPP.KeyValue[3]
        {
          new FunXMPP.KeyValue(nameof (jid), jid),
          new FunXMPP.KeyValue("type", "unstar"),
          new FunXMPP.KeyValue("modify_tag", modifyTag.ToString())
        }), new QrMetricsMapping?(QrMetricsMapping.FORWARD_CHATS), handler);
      }

      public void SendQrBattery(
        int battery,
        bool powerSourceConnected,
        bool batterySaverEnabled,
        Action onComplete = null,
        Action<int> onError = null)
      {
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (onComplete == null)
            return;
          onComplete();
        }), onError ?? (Action<int>) (err => { }));
        this.SendQrAction("qrbattery_", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode(nameof (battery), new FunXMPP.KeyValue[3]
        {
          new FunXMPP.KeyValue("value", battery.ToString()),
          new FunXMPP.KeyValue("live", powerSourceConnected ? "true" : "false"),
          new FunXMPP.KeyValue("powersave", batterySaverEnabled ? "true" : "false")
        }), new QrMetricsMapping?(QrMetricsMapping.FORWARD_BATTERY), handler);
      }

      public void SendQrChangeNumberNotificationDismiss(
        string chatJid,
        Action onComplete = null,
        Action<int> onError = null)
      {
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          Action action = onComplete;
          if (action == null)
            return;
          action();
        }), onError ?? (Action<int>) (err => { }));
        this.SendQrAction("qrchangenumber_", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("chat", new FunXMPP.KeyValue[2]
        {
          new FunXMPP.KeyValue("jid", chatJid),
          new FunXMPP.KeyValue("type", "modify")
        }), new QrMetricsMapping?(), handler);
      }

      public void SendQrChangeNumberNotificationForward(
        string chatJid,
        string oldJid,
        string newJid,
        Action onComplete = null,
        Action<int> onError = null)
      {
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          Action action = onComplete;
          if (action == null)
            return;
          action();
        }), onError ?? (Action<int>) (err => { }));
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>()
        {
          new FunXMPP.KeyValue("jid", chatJid),
          new FunXMPP.KeyValue("type", "modify")
        };
        if (oldJid != null)
          keyValueList.Add(new FunXMPP.KeyValue("old_jid", oldJid));
        if (oldJid != null)
          keyValueList.Add(new FunXMPP.KeyValue("new_jid", newJid));
        this.SendQrAction("qrchangenumber_", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("chat", keyValueList.ToArray()), new QrMetricsMapping?(), handler);
      }

      public void SendQrIdentityChange(string jid, Action onComplete = null, Action<int> onError = null)
      {
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (onComplete == null)
            return;
          onComplete();
        }), onError ?? (Action<int>) (err => { }));
        string Displayable = (string) null;
        byte[] ScannableBytes = (byte[]) null;
        AppState.GetConnection().Encryption.IdentityGetFingerprintBytes(jid, out Displayable, out ScannableBytes);
        this.SendQrAction("qridentity_", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("identity", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue(nameof (jid), jid)
        }, new List<FunXMPP.ProtocolTreeNode>()
        {
          new FunXMPP.ProtocolTreeNode("raw", (FunXMPP.KeyValue[]) null, ScannableBytes),
          new FunXMPP.ProtocolTreeNode("text", (FunXMPP.KeyValue[]) null, Displayable)
        }.ToArray()), new QrMetricsMapping?(QrMetricsMapping.FORWARD_IDENTITY), handler);
      }

      public void SendQrFrequentContacts(Action onComplete = null, Action<int> onError = null)
      {
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          if (onComplete == null)
            return;
          onComplete();
        }), onError ?? (Action<int>) (err => { }));
        List<FunXMPP.ProtocolTreeNode> innerNodes = new List<FunXMPP.ProtocolTreeNode>();
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          foreach (Pair<string, FunXMPP.FMessage.Type> pair in new List<Pair<string, FunXMPP.FMessage.Type>>()
          {
            new Pair<string, FunXMPP.FMessage.Type>("image", FunXMPP.FMessage.Type.Image),
            new Pair<string, FunXMPP.FMessage.Type>("video", FunXMPP.FMessage.Type.Video),
            new Pair<string, FunXMPP.FMessage.Type>("message", FunXMPP.FMessage.Type.Undefined)
          })
          {
            MessagesContext messagesContext = db;
            int second = (int) pair.Second;
            int? limit = new int?(3);
            string[] excludedJids = new string[1]
            {
              "0@s.whatsapp.net"
            };
            foreach (string frequentChat in messagesContext.GetFrequentChats((FunXMPP.FMessage.Type) second, limit, excludedJids))
              innerNodes.Add(new FunXMPP.ProtocolTreeNode(pair.First, new FunXMPP.KeyValue[1]
              {
                new FunXMPP.KeyValue("jid", frequentChat)
              }));
          }
        }));
        this.SendQrAction("qrfreqcontacts_", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("contacts", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("type", "frequent")
        }, innerNodes.ToArray()), new QrMetricsMapping?(QrMetricsMapping.FORWARD_FREQ_CONTACTS), handler);
      }

      public void SendQrLocationUpdate(
        string jid,
        int elapsed,
        WhatsApp.ProtoBuf.Message msg,
        Action onComplete = null,
        Action<int> onError = null)
      {
        FunXMPP.IqResultHandler handler = new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
        {
          Action action = onComplete;
          if (action == null)
            return;
          action();
        }), onError ?? (Action<int>) (err => { }));
        this.SendQrAction("qrlocation_", (FunXMPP.KeyValue[]) null, new FunXMPP.ProtocolTreeNode("location", new FunXMPP.KeyValue[3]
        {
          new FunXMPP.KeyValue("type", "update"),
          new FunXMPP.KeyValue(nameof (jid), jid),
          new FunXMPP.KeyValue(nameof (elapsed), elapsed.ToString())
        }, msg.ToPlainText(false)), new QrMetricsMapping?(QrMetricsMapping.FORWARD_LIVE_LOCATION), handler);
      }

      private FunXMPP.Connection.QrChatActions GetQrChatsResponse(string jid)
      {
        FunXMPP.Connection.QrChatActions qrChatsResponse = new FunXMPP.Connection.QrChatActions();
        FunXMPP.ChatResponse[] chats = this.EventHandler.Qr.GetChats(true, jid);
        List<FunXMPP.ChatResponse> chatsWithUnread = new List<FunXMPP.ChatResponse>();
        List<FunXMPP.ChatResponse> chatsWithBefore = new List<FunXMPP.ChatResponse>();
        List<Message> lastMessages = new List<Message>();
        int num = 0;
        foreach (FunXMPP.ChatResponse chatResponse in chats)
        {
          if (chatResponse.Messages != null && chatResponse.Messages.Count != 0)
          {
            List<Message> messages = chatResponse.Messages;
            if (jid != null || lastMessages.Count < 1000)
            {
              int index = messages.Count - 1;
              lastMessages.Add(messages[index]);
              messages.RemoveAt(index);
            }
            ++num;
            if (messages.Count > 0)
            {
              if (chatResponse.UnreadMessageKey != null)
                chatsWithUnread.Add(chatResponse);
              else
                chatsWithBefore.Add(chatResponse);
            }
          }
        }
        Log.d("WebClient", "Sending {0} / {1} last messages", (object) lastMessages.Count, (object) num);
        qrChatsResponse.ResponseNodes = ((IEnumerable<FunXMPP.ChatResponse>) chats).Select<FunXMPP.ChatResponse, FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ChatResponse, FunXMPP.ProtocolTreeNode>) (c => this.NodeFromChatResponse(c)));
        qrChatsResponse.AcksFromMessages = (lastMessages.Count > 0 ? 1 : 0) + (chatsWithUnread.Count + chatsWithBefore.Count);
        qrChatsResponse.PostResponse = (Action<Action>) (onComplete =>
        {
          if (lastMessages.Count > 0)
            this.SendQrMessages((IEnumerable<Message>) lastMessages, QrMessageForwardType.LastMessages, onComplete, invis: true);
          else
            this.SendQrMessages((IEnumerable<Message>) null, QrMessageForwardType.LastMessages, onComplete, invis: true);
          if (chatsWithBefore.Count > 0)
          {
            foreach (FunXMPP.ChatResponse chatResponse in chatsWithBefore)
              this.SendQrMessages((IEnumerable<Message>) chatResponse.Messages, QrMessageForwardType.MessageHistory, onComplete, invis: true);
          }
          if (chatsWithUnread.Count <= 0)
            return;
          foreach (FunXMPP.ChatResponse chatResponse in chatsWithUnread)
            this.SendQrMessages((IEnumerable<Message>) chatResponse.Messages, QrMessageForwardType.Unread, onComplete, invis: true, unreadKey: chatResponse.UnreadMessageKey);
        });
        return qrChatsResponse;
      }

      public void SendQrPreemptiveChats(Action onComplete)
      {
        Log.WriteLineDebug("   WebClient > Sending Preemptive Chats");
        FunXMPP.Connection.QrChatActions qrChatsResponse = this.GetQrChatsResponse((string) null);
        int totalAcksExpected = qrChatsResponse.AcksFromMessages + 1;
        Action onComplete1 = (Action) (() =>
        {
          --totalAcksExpected;
          if (totalAcksExpected > 0)
            return;
          onComplete();
        });
        string id = "preempt-" + this.MakeId("qrmsg_");
        FunXMPP.ProtocolTreeNode innerNode = this.BuildQrResponse(id, "chat", qrChatsResponse.ResponseNodes, (IEnumerable<FunXMPP.KeyValue>) null);
        this.SendQrResponse(id, innerNode, true, new QrMetricsMapping?(QrMetricsMapping.RESPONSE_CHATS), onComplete1);
        qrChatsResponse.PostResponse(onComplete1);
      }

      private IEnumerable<FunXMPP.ProtocolTreeNode> GetQrContactsResponse(
        bool sendResponse,
        string id,
        Action onComplete)
      {
        IEnumerable<FunXMPP.ProtocolTreeNode> nodes = ((IEnumerable<FunXMPP.ContactResponse>) this.EventHandler.Qr.GetContacts()).Select<FunXMPP.ContactResponse, FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ContactResponse, FunXMPP.ProtocolTreeNode>) (c => this.CreateContactNode(c, false)));
        FunXMPP.KeyValue[] attrs = new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("checksum", Settings.ContactsChecksum.ToString())
        };
        if (sendResponse)
        {
          FunXMPP.ProtocolTreeNode innerNode = this.BuildQrResponse(id, "contacts", nodes, (IEnumerable<FunXMPP.KeyValue>) attrs);
          this.SendQrResponse(id, innerNode, true, new QrMetricsMapping?(QrMetricsMapping.RESPONSE_CONTACTS), onComplete);
        }
        return nodes;
      }

      public void SendQrPreemptiveContacts(Action onComplete)
      {
        Log.WriteLineDebug("   WebClient > Sending Preemptive Contacts");
        this.GetQrContactsResponse(true, "preempt-" + this.MakeId("qrmsg_"), onComplete);
      }

      public FunXMPP.ProtocolTreeNode[] NodesFromSyncResponse(FunXMPP.SyncResponse s)
      {
        return new FunXMPP.ProtocolTreeNode[2]
        {
          new FunXMPP.ProtocolTreeNode("battery", new FunXMPP.KeyValue[3]
          {
            new FunXMPP.KeyValue("value", s.BatteryPercentage.ToString()),
            new FunXMPP.KeyValue("live", s.PowerSourceConnected ? "true" : "false"),
            new FunXMPP.KeyValue("powersave", s.BatterySaverEnabled ? "true" : "false")
          }),
          new FunXMPP.ProtocolTreeNode("config", new FunXMPP.KeyValue[3]
          {
            new FunXMPP.KeyValue("lg", s.Language),
            new FunXMPP.KeyValue("lc", s.Locale),
            new FunXMPP.KeyValue("t", s.IsMilitaryTime ? "24" : "12")
          })
        };
      }

      public FunXMPP.ProtocolTreeNode NodeFromChatResponse(FunXMPP.ChatResponse c)
      {
        List<FunXMPP.KeyValue> keyValueList1 = new List<FunXMPP.KeyValue>();
        keyValueList1.Add(new FunXMPP.KeyValue("jid", c.Jid));
        if (!string.IsNullOrEmpty(c.DisplayName))
          keyValueList1.Add(new FunXMPP.KeyValue("name", c.DisplayName));
        if (c.Archived)
          keyValueList1.Add(new FunXMPP.KeyValue("archive", "true"));
        if (c.ReadOnly)
          keyValueList1.Add(new FunXMPP.KeyValue("read_only", "true"));
        if (!c.Spam)
          keyValueList1.Add(new FunXMPP.KeyValue("spam", "false"));
        long unixTime;
        if (c.Timestamp.HasValue)
        {
          List<FunXMPP.KeyValue> keyValueList2 = keyValueList1;
          unixTime = c.Timestamp.Value.ToUnixTime();
          FunXMPP.KeyValue keyValue = new FunXMPP.KeyValue("t", unixTime.ToString());
          keyValueList2.Add(keyValue);
        }
        if (c.MuteExpiration.HasValue)
        {
          List<FunXMPP.KeyValue> keyValueList3 = keyValueList1;
          unixTime = c.MuteExpiration.Value.ToUnixTime();
          FunXMPP.KeyValue keyValue = new FunXMPP.KeyValue("mute", unixTime.ToString());
          keyValueList3.Add(keyValue);
        }
        if (c.PinTimestamp.HasValue)
        {
          List<FunXMPP.KeyValue> keyValueList4 = keyValueList1;
          unixTime = c.PinTimestamp.Value.ToUnixTime();
          FunXMPP.KeyValue keyValue = new FunXMPP.KeyValue("pin", unixTime.ToString());
          keyValueList4.Add(keyValue);
        }
        if (c.ModifyTag != 0)
          keyValueList1.Add(new FunXMPP.KeyValue("modify_tag", c.ModifyTag.ToString()));
        if (c.OldJid != null)
          keyValueList1.Add(new FunXMPP.KeyValue("old_jid", c.OldJid));
        if (c.NewJid != null)
          keyValueList1.Add(new FunXMPP.KeyValue("new_jid", c.NewJid));
        keyValueList1.Add(new FunXMPP.KeyValue("count", c.Count.ToString()));
        if (c.Type.HasValue)
        {
          string v = (string) null;
          FunXMPP.ChatResponseAction? type = c.Type;
          if (type.HasValue)
          {
            switch (type.GetValueOrDefault())
            {
              case FunXMPP.ChatResponseAction.Clear:
                v = "clear";
                break;
              case FunXMPP.ChatResponseAction.Delete:
                v = "delete";
                break;
              case FunXMPP.ChatResponseAction.Resend:
                v = "ahead";
                break;
            }
          }
          if (v != null)
            keyValueList1.Add(new FunXMPP.KeyValue("type", v));
        }
        if (c.Messages != null && c.Messages.Count > 0)
          keyValueList1.Add(new FunXMPP.KeyValue("message", "true"));
        if (c.UnreadMessageKey == null)
          return new FunXMPP.ProtocolTreeNode("chat", keyValueList1.ToArray());
        FunXMPP.ProtocolTreeNode child = new FunXMPP.ProtocolTreeNode("item", new List<FunXMPP.KeyValue>()
        {
          new FunXMPP.KeyValue("index", c.UnreadMessageKey.id),
          new FunXMPP.KeyValue("owner", c.UnreadMessageKey.from_me.ToString()),
          new FunXMPP.KeyValue("participant", c.UnreadMessageKey.remote_jid)
        }.ToArray());
        return new FunXMPP.ProtocolTreeNode("chat", keyValueList1.ToArray(), child);
      }

      public FunXMPP.ChatResponse ChatResponseFromNode(FunXMPP.ProtocolTreeNode n)
      {
        string attributeValue = n.GetAttributeValue("jid");
        return new FunXMPP.ChatResponse()
        {
          Jid = attributeValue,
          DisplayName = n.GetAttributeValue("name"),
          Archived = n.GetAttributeValue("archived") == "true",
          ReadOnly = n.GetAttributeValue("read_only") == "true",
          Spam = !(n.GetAttributeValue("spam") == "false"),
          Timestamp = n.GetAttributeDateTime("t"),
          MuteExpiration = n.GetAttributeDateTime("mute"),
          PinTimestamp = n.GetAttributeDateTime("pin"),
          ModifyTag = n.GetAttributeValue("modify_tag") != null ? int.Parse(n.GetAttributeValue("modify_tag")) : 0,
          Active = n.GetAttributeValue("active") == "true",
          Count = n.GetAttributeValue("count") == null ? 0 : int.Parse(n.GetAttributeValue("count")),
          LastMessageKey = new FunXMPP.FMessage.Key(attributeValue, n.GetAttributeValue("owner") == "true", n.GetAttributeValue("index")),
          OldJid = n.GetAttributeValue("old_jid"),
          NewJid = n.GetAttributeValue("new_jid")
        };
      }

      private void SendQrResponse(
        string id,
        FunXMPP.ProtocolTreeNode innerNode,
        bool encrypted,
        QrMetricsMapping? metrics,
        Action onComplete = null,
        Action<int> onError = null)
      {
        if (encrypted)
          this.EventHandler.Qr.Session.Synchronize((Action) (() => this.SendQrResponseImpl(id, innerNode, encrypted, metrics, onComplete, onError)));
        else
          this.SendQrResponseImpl(id, innerNode, encrypted, metrics, onComplete, onError);
      }

      private void SendQrResponseImpl(
        string id,
        FunXMPP.ProtocolTreeNode innerNode,
        bool encrypted,
        QrMetricsMapping? metrics,
        Action onComplete = null,
        Action<int> onError = null)
      {
        if (encrypted)
          innerNode = this.EncryptNode(innerNode, metrics);
        if (onComplete != null || onError != null)
        {
          try
          {
            this.AddIqHandler(id, new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
            {
              if (onComplete == null)
                return;
              onComplete();
            }), onError ?? (Action<int>) (err => { })));
          }
          catch (ArgumentException ex)
          {
            Log.l("WebClient", "Exception adding iq for {0}", (object) id);
            Log.SendCrashLog((Exception) ex, nameof (SendQrResponseImpl), logOnlyForRelease: true);
            return;
          }
        }
        this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[3]
        {
          new FunXMPP.KeyValue("type", "set"),
          new FunXMPP.KeyValue(nameof (id), id),
          new FunXMPP.KeyValue("xmlns", "w:web")
        }, innerNode));
      }

      private FunXMPP.ProtocolTreeNode BuildQrActionItem(string id, int status)
      {
        return new FunXMPP.ProtocolTreeNode("item", new FunXMPP.KeyValue[2]
        {
          new FunXMPP.KeyValue(nameof (id), id),
          new FunXMPP.KeyValue("code", status.ToString())
        });
      }

      private FunXMPP.ProtocolTreeNode BuildQrResponse(
        string id,
        string type,
        IEnumerable<FunXMPP.ProtocolTreeNode> nodes,
        IEnumerable<FunXMPP.KeyValue> attrs)
      {
        return new FunXMPP.ProtocolTreeNode("response", ((IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue(nameof (type), type)
        }).Concat<FunXMPP.KeyValue>((IEnumerable<FunXMPP.KeyValue>) ((object) attrs ?? (object) new FunXMPP.KeyValue[0])).ToArray<FunXMPP.KeyValue>(), nodes == null ? (FunXMPP.ProtocolTreeNode[]) null : nodes.ToArray<FunXMPP.ProtocolTreeNode>());
      }

      public void SendQrMediaResponse(
        string id,
        string url,
        string mediaKey,
        int? error,
        Action onComplete = null)
      {
        List<FunXMPP.KeyValue> attrs = new List<FunXMPP.KeyValue>();
        if (url != null)
        {
          attrs.Add(new FunXMPP.KeyValue("code", "200"));
          attrs.Add(new FunXMPP.KeyValue(nameof (url), url));
        }
        else
          attrs.Add(new FunXMPP.KeyValue("code", (error ?? 500).ToString()));
        if (mediaKey != null)
          attrs.Add(new FunXMPP.KeyValue("media_key", mediaKey));
        FunXMPP.ProtocolTreeNode innerNode = this.BuildQrResponse(id, "media", (IEnumerable<FunXMPP.ProtocolTreeNode>) null, (IEnumerable<FunXMPP.KeyValue>) attrs);
        this.SendQrResponse(id, innerNode, true, new QrMetricsMapping?(QrMetricsMapping.RESPONSE_MEDIA), onComplete);
      }

      public void SendQrProfilePictureResponse(
        string id,
        string jid,
        bool large,
        Action onComplete)
      {
        string v = (string) null;
        byte[] data = (byte[]) null;
        using (FunXMPP.PhotoResponse profilePhoto = this.EventHandler.Qr.GetProfilePhoto(jid, large))
        {
          if (profilePhoto.Stream != null)
          {
            try
            {
              MemoryStream destination = new MemoryStream();
              profilePhoto.Stream.CopyTo((Stream) destination);
              data = destination.ToArray();
              v = profilePhoto.Id;
            }
            catch (Exception ex)
            {
            }
          }
        }
        FunXMPP.ProtocolTreeNode protocolTreeNode = new FunXMPP.ProtocolTreeNode("preview", new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue(nameof (id), v)
        }, data);
        FunXMPP.ProtocolTreeNode innerNode = this.BuildQrResponse(id, "preview", (IEnumerable<FunXMPP.ProtocolTreeNode>) new FunXMPP.ProtocolTreeNode[1]
        {
          protocolTreeNode
        }, (IEnumerable<FunXMPP.KeyValue>) null);
        this.SendQrResponse(id, innerNode, true, new QrMetricsMapping?(QrMetricsMapping.RESPONSE_PREVIEW), onComplete);
      }

      private QrCrypto GetQrCrypto()
      {
        return this.EventHandler.Qr.Session.Crypto ?? throw new InvalidOperationException("Asked to get crypto keys when no session active");
      }

      private FunXMPP.ProtocolTreeNode BuildErrorResponse(int error, ref bool encryptedResponse)
      {
        encryptedResponse = false;
        return new FunXMPP.ProtocolTreeNode(nameof (error), new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue("code", error.ToString())
        });
      }

      private void ProcessQrNode(
        FunXMPP.ProtocolTreeNode node,
        string id,
        DateTime? notificationTimestamp,
        ref Action ackAction)
      {
        string type = node.GetAttributeValue("type");
        FunXMPP.ProtocolTreeNode response = (FunXMPP.ProtocolTreeNode) null;
        IEnumerable<FunXMPP.ProtocolTreeNode> responseNodes = (IEnumerable<FunXMPP.ProtocolTreeNode>) null;
        QrMetricsMapping metricsType = QrMetricsMapping.UNKNOWN;
        bool encryptedResponse = true;
        List<Action> postResponse = new List<Action>();
        IEnumerable<FunXMPP.KeyValue> responseAttributes = (IEnumerable<FunXMPP.KeyValue>) null;
        if (FunXMPP.ProtocolTreeNode.TagEquals(node, "enc"))
        {
          byte[] payload = node.data;
          try
          {
            this.EventHandler.Qr.Session.Synchronize((Action) (() => node = this.GetQrCrypto().ParseBlob(payload, 0, payload.Length)));
          }
          catch
          {
            response = this.BuildErrorResponse(401, ref encryptedResponse);
          }
          type = node.GetAttributeValue("type");
          if (FunXMPP.ProtocolTreeNode.TagEquals(node, "query"))
          {
            bool flag1 = false;
            int? error = new int?();
            byte[] dataBytes = (byte[]) null;
            Log.d("WebClient", "query | type: {0}", (object) type);
            if (type == "message" || type == "media_message")
            {
              bool mediaOnly = type == "media_message";
              string attributeValue1 = node.GetAttributeValue("count");
              int count = 0;
              Message[] msgs = (Message[]) null;
              ref int local = ref count;
              if (int.TryParse(attributeValue1, out local) && count > 0)
              {
                string jid = node.GetAttributeValue("jid");
                if (!string.IsNullOrEmpty(jid))
                {
                  string attributeValue2 = node.GetAttributeValue("index");
                  string attributeValue3 = node.GetAttributeValue("owner");
                  string attributeValue4 = node.GetAttributeValue("kind");
                  string attributeValue5 = node.GetAttributeValue("media");
                  FunXMPP.FMessage.Type? fmsgType = new FunXMPP.FMessage.Type?();
                  switch (attributeValue5)
                  {
                    case "document":
                      fmsgType = new FunXMPP.FMessage.Type?(FunXMPP.FMessage.Type.Document);
                      break;
                    case "url":
                      fmsgType = new FunXMPP.FMessage.Type?(FunXMPP.FMessage.Type.ExtendedText);
                      break;
                  }
                  if (!string.IsNullOrEmpty(attributeValue2) && !string.IsNullOrEmpty(attributeValue3) && !string.IsNullOrEmpty(attributeValue4))
                  {
                    bool flag2 = attributeValue3 == "true";
                    msgs = !(attributeValue4 == "before") ? this.EventHandler.Qr.GetMessagesAfter(new FunXMPP.FMessage.Key(jid, flag2, attributeValue2), true, new int?(count), false, mediaOnly, fmsgType) : this.EventHandler.Qr.GetMessagesBefore(jid, flag2, attributeValue2, new int?(count), true, mediaOnly, fmsgType);
                  }
                  else
                    MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
                    {
                      Conversation conversation = db.GetConversation(jid, CreateOptions.None);
                      if (conversation == null)
                        return;
                      msgs = ((IEnumerable<Message>) conversation.GetLatestMessages(db, new int?(count), new int?(0), mediaType: fmsgType)).Reverse<Message>().ToArray<Message>();
                    }));
                }
              }
              QrMetricsMapping metrics = QrMetricsMapping.UNKNOWN;
              responseNodes = FunXMPP.Connection.QrSerializeMessages((IEnumerable<Message>) (msgs ?? new Message[0]), ref metrics, false);
              metricsType = QrMetricsMapping.RESPONSE_MESSAGES;
            }
            else if (type == "chat")
            {
              if (node.GetAttributeValue("kind") == "retry")
              {
                FunXMPP.Connection.QrChatActions result = this.GetQrChatsResponse(node.GetAttributeValue("jid"));
                responseNodes = result.ResponseNodes;
                postResponse.Add((Action) (() => result.PostResponse((Action) null)));
              }
              else
                responseAttributes = (IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[1]
                {
                  new FunXMPP.KeyValue("duplicate", "true")
                };
              metricsType = QrMetricsMapping.RESPONSE_CHATS;
            }
            else if (type == "contacts")
            {
              List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>();
              keyValueList.Add(new FunXMPP.KeyValue("checksum", Settings.ContactsChecksum.ToString()));
              if (node.GetAttributeValue("kind") == "retry")
                responseNodes = this.GetQrContactsResponse(false, (string) null, (Action) null);
              else
                keyValueList.Add(new FunXMPP.KeyValue("duplicate", "true"));
              responseAttributes = (IEnumerable<FunXMPP.KeyValue>) keyValueList;
              metricsType = QrMetricsMapping.RESPONSE_CONTACTS;
            }
            else if (type == "media")
            {
              string attributeValue6 = node.GetAttributeValue("index");
              string attributeValue7 = node.GetAttributeValue("jid");
              string attributeValue8 = node.GetAttributeValue("owner");
              flag1 = true;
              int num = attributeValue8 == "true" ? 1 : 0;
              string id1 = attributeValue6;
              this.EventHandler.Qr.OnReUpload(new FunXMPP.FMessage.Key(attributeValue7, num != 0, id1), id);
            }
            else if (type == "preview")
            {
              string attributeValue9 = node.GetAttributeValue("kind");
              string jid = node.GetAttributeValue("jid");
              if (attributeValue9 == "status")
              {
                if (jid != null)
                  MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
                  {
                    WaStatus[] statuses = db.GetStatuses(jid, false, true, new TimeSpan?());
                    List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList = new List<FunXMPP.ProtocolTreeNode>();
                    if (((IEnumerable<WaStatus>) statuses).Any<WaStatus>())
                    {
                      WaStatus waStatus = statuses[0];
                      Message messageById = db.GetMessageById(waStatus.MessageId);
                      if (messageById != null && messageById.BinaryData != null && messageById.BinaryData.Length != 0)
                      {
                        protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("preview", (FunXMPP.KeyValue[]) null, messageById.BinaryData));
                        responseNodes = (IEnumerable<FunXMPP.ProtocolTreeNode>) protocolTreeNodeList;
                      }
                    }
                    if (responseNodes == null)
                      protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("preview", new FunXMPP.KeyValue[1]
                      {
                        new FunXMPP.KeyValue("type", "missing")
                      }));
                    responseNodes = (IEnumerable<FunXMPP.ProtocolTreeNode>) protocolTreeNodeList;
                  }));
              }
              else
              {
                string attributeValue10 = node.GetAttributeValue(nameof (id));
                bool flag3 = false;
                FunXMPP.KeyValue[] attrs = (FunXMPP.KeyValue[]) null;
                if (jid != null)
                {
                  string profilePhotoId = this.EventHandler.Qr.GetProfilePhotoId(jid);
                  if (attributeValue10 != null && profilePhotoId == attributeValue10)
                    attrs = new FunXMPP.KeyValue[1]
                    {
                      new FunXMPP.KeyValue(nameof (id), profilePhotoId)
                    };
                  else if (profilePhotoId == null)
                    attrs = new FunXMPP.KeyValue[1]
                    {
                      new FunXMPP.KeyValue("type", "missing")
                    };
                  else
                    flag3 = true;
                }
                if (!flag3)
                {
                  responseNodes = (IEnumerable<FunXMPP.ProtocolTreeNode>) new FunXMPP.ProtocolTreeNode[1]
                  {
                    new FunXMPP.ProtocolTreeNode("preview", attrs, (byte[]) null)
                  };
                  metricsType = QrMetricsMapping.RESPONSE_PREVIEW;
                }
                else if (jid != null)
                {
                  this.EventHandler.Qr.OnProfilePhotoRequest(id, jid, false);
                  flag1 = true;
                }
              }
            }
            else if (type == "resume")
            {
              FunXMPP.ResumeResponse resumeState = this.EventHandler.Qr.GetResumeState(node.GetAllChildren("last").Select<FunXMPP.ProtocolTreeNode, FunXMPP.ChatResponse>((Func<FunXMPP.ProtocolTreeNode, FunXMPP.ChatResponse>) (n => this.ChatResponseFromNode(n))).ToDictionary<FunXMPP.ChatResponse, string, FunXMPP.ChatResponse>((Func<FunXMPP.ChatResponse, string>) (n => n.Jid), (Func<FunXMPP.ChatResponse, FunXMPP.ChatResponse>) (n => n)));
              responseAttributes = (IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[1]
              {
                new FunXMPP.KeyValue("checksum", resumeState.Checksum.ToString())
              };
              responseNodes = resumeState.ExistingChats.Select<FunXMPP.ChatResponse, FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ChatResponse, FunXMPP.ProtocolTreeNode>) (chat => this.NodeFromChatResponse(chat))).Concat<FunXMPP.ProtocolTreeNode>(resumeState.NewChats.Select<FunXMPP.ChatResponse, FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ChatResponse, FunXMPP.ProtocolTreeNode>) (chat => this.NodeFromChatResponse(chat))));
              List<Message> lastMessages = new List<Message>();
              postResponse.Add((Action) (() =>
              {
                if (lastMessages.Count <= 0)
                  return;
                this.SendQrMessages((IEnumerable<Message>) lastMessages, QrMessageForwardType.LastMessages, invis: true, resume: true);
              }));
              foreach (FunXMPP.ChatResponse chatResponse in resumeState.ExistingChats.Concat<FunXMPP.ChatResponse>((IEnumerable<FunXMPP.ChatResponse>) resumeState.NewChats))
              {
                FunXMPP.ChatResponse chat = chatResponse;
                List<Message> messages = chat.Messages;
                if (messages != null)
                {
                  FunXMPP.ChatResponseAction? type1 = chat.Type;
                  FunXMPP.ChatResponseAction chatResponseAction = FunXMPP.ChatResponseAction.Ahead;
                  if ((type1.GetValueOrDefault() == chatResponseAction ? (type1.HasValue ? 1 : 0) : 0) != 0)
                  {
                    postResponse.Add((Action) (() => this.SendQrMessages((IEnumerable<Message>) chat.Messages, QrMessageForwardType.MessagesSinceReconnect, invis: true, resume: true)));
                  }
                  else
                  {
                    lastMessages.Add(messages.Last<Message>());
                    messages.RemoveAt(messages.Count - 1);
                    if (messages.Count > 0)
                    {
                      if (chat.UnreadMessageKey != null)
                        postResponse.Add((Action) (() => this.SendQrMessages((IEnumerable<Message>) chat.Messages, QrMessageForwardType.Unread, invis: true, resume: true, unreadKey: chat.UnreadMessageKey)));
                      else
                        postResponse.Add((Action) (() => this.SendQrMessages((IEnumerable<Message>) chat.Messages, QrMessageForwardType.LastMessages, invis: true, resume: true)));
                    }
                  }
                }
              }
              metricsType = QrMetricsMapping.RESPONSE_RESUME;
            }
            else if (type == "receipt")
            {
              List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList1 = new List<FunXMPP.ProtocolTreeNode>();
              List<Message[]> messageArrayList = new List<Message[]>();
              foreach (FunXMPP.ProtocolTreeNode allChild in node.GetAllChildren("last"))
              {
                List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList2 = new List<FunXMPP.ProtocolTreeNode>();
                string attributeValue11 = allChild.GetAttributeValue("index");
                string attributeValue12 = allChild.GetAttributeValue("jid");
                DateTime? attributeDateTime = allChild.GetAttributeDateTime("t");
                FunXMPP.ReceiptResponse receiptStates = this.EventHandler.Qr.GetReceiptStates(new FunXMPP.FMessage.Key(attributeValue12, true, attributeValue11), attributeDateTime.Value);
                List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>();
                keyValueList.Add(new FunXMPP.KeyValue("jid", attributeValue12));
                if (receiptStates != null)
                {
                  if (receiptStates.Timestamp.HasValue)
                    keyValueList.Add(new FunXMPP.KeyValue("t", receiptStates.Timestamp.Value.ToUnixTime().ToString()));
                  foreach (FunXMPP.ReceiptStateResponse receipt in receiptStates.Receipts)
                  {
                    string v;
                    switch (receipt.Status)
                    {
                      case FunXMPP.FMessage.Status.ReceivedByServer:
                        v = "1";
                        break;
                      case FunXMPP.FMessage.Status.ReceivedByTarget:
                        v = "2";
                        break;
                      case FunXMPP.FMessage.Status.PlayedByTarget:
                        v = "4";
                        break;
                      case FunXMPP.FMessage.Status.ReadByTarget:
                        v = "3";
                        break;
                      default:
                        v = "0";
                        break;
                    }
                    protocolTreeNodeList2.Add(new FunXMPP.ProtocolTreeNode("item", new FunXMPP.KeyValue[3]
                    {
                      new FunXMPP.KeyValue("index", receipt.KeyId),
                      new FunXMPP.KeyValue("owner", receipt.KeyFromMe ? "true" : "false"),
                      new FunXMPP.KeyValue("status", v)
                    }));
                  }
                }
                protocolTreeNodeList1.Add(new FunXMPP.ProtocolTreeNode("receipt", keyValueList.ToArray(), protocolTreeNodeList2.Count > 0 ? protocolTreeNodeList2.ToArray() : (FunXMPP.ProtocolTreeNode[]) null));
              }
              responseNodes = (IEnumerable<FunXMPP.ProtocolTreeNode>) protocolTreeNodeList1;
              metricsType = QrMetricsMapping.RESPONSE_RECEIPT;
            }
            else if (type == "group")
            {
              string attributeValue = node.GetAttributeValue("jid");
              FunXMPP.Connection.GroupInfo groupMetadata = this.EventHandler.Qr.GetGroupMetadata(attributeValue);
              List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>();
              List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList = new List<FunXMPP.ProtocolTreeNode>();
              keyValueList.Add(new FunXMPP.KeyValue("jid", attributeValue));
              if (groupMetadata != null)
              {
                if (groupMetadata.CreationTime.HasValue)
                  keyValueList.Add(new FunXMPP.KeyValue("create", groupMetadata.CreationTime.Value.ToUnixTime().ToString()));
                if (!string.IsNullOrEmpty(groupMetadata.CreatorJid))
                  keyValueList.Add(new FunXMPP.KeyValue("creator", groupMetadata.CreatorJid));
                if (!string.IsNullOrEmpty(groupMetadata.SuperAdmin))
                {
                  protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("participant", new FunXMPP.KeyValue[2]
                  {
                    new FunXMPP.KeyValue("jid", groupMetadata.SuperAdmin),
                    new FunXMPP.KeyValue("type", "superadmin")
                  }));
                  groupMetadata.AdminJids.Remove(groupMetadata.SuperAdmin);
                }
                if (groupMetadata.AdminJids != null)
                {
                  foreach (string adminJid in groupMetadata.AdminJids)
                    protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("participant", new FunXMPP.KeyValue[2]
                    {
                      new FunXMPP.KeyValue("jid", adminJid),
                      new FunXMPP.KeyValue("type", "admin")
                    }));
                }
                if (groupMetadata.NonadminJids != null)
                {
                  foreach (string nonadminJid in groupMetadata.NonadminJids)
                    protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("participant", new FunXMPP.KeyValue[1]
                    {
                      new FunXMPP.KeyValue("jid", nonadminJid)
                    }));
                }
                if (groupMetadata.Description != null)
                  protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("description", new FunXMPP.KeyValue[0], groupMetadata.Description.Body));
              }
              else
                keyValueList.Add(new FunXMPP.KeyValue("type", "missing"));
              responseNodes = (IEnumerable<FunXMPP.ProtocolTreeNode>) new FunXMPP.ProtocolTreeNode[1]
              {
                new FunXMPP.ProtocolTreeNode("group", keyValueList.ToArray(), protocolTreeNodeList.ToArray())
              };
              metricsType = QrMetricsMapping.RESPONSE_GROUP;
            }
            else if (type == "action")
            {
              int? attributeInt = node.GetAttributeInt("epoch");
              if (attributeInt.HasValue)
                this.EventHandler.Qr.Session.ProcessEpoch(attributeInt.GetValueOrDefault());
              FunXMPP.ActionResponse actions = this.EventHandler.Qr.GetActions(((IEnumerable<FunXMPP.ProtocolTreeNode>) (node.children ?? new FunXMPP.ProtocolTreeNode[0])).Where<FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, bool>) (n => FunXMPP.ProtocolTreeNode.TagEquals(n, "item"))).Select<FunXMPP.ProtocolTreeNode, string>((Func<FunXMPP.ProtocolTreeNode, string>) (n => n.GetAttributeValue(nameof (id)))).ToArray<string>());
              if (actions.Replaced)
              {
                responseAttributes = (IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[1]
                {
                  new FunXMPP.KeyValue("replaced", "true")
                };
              }
              else
              {
                List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList = new List<FunXMPP.ProtocolTreeNode>();
                foreach (string key in actions.Actions.Keys)
                  protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("item", new FunXMPP.KeyValue[2]
                  {
                    new FunXMPP.KeyValue(nameof (id), key),
                    new FunXMPP.KeyValue("code", actions.Actions[key].ToString())
                  }));
                responseNodes = (IEnumerable<FunXMPP.ProtocolTreeNode>) protocolTreeNodeList;
              }
              metricsType = QrMetricsMapping.RESPONSE_ACTION;
            }
            else if (type == "message_info")
            {
              string attributeValue = node.GetAttributeValue("index");
              FunXMPP.MessageInfoStateResponse messageInfoState = this.EventHandler.Qr.GetMessageInfoState(new FunXMPP.FMessage.Key(node.GetAttributeValue("jid"), true, attributeValue));
              if (messageInfoState != null)
              {
                responseAttributes = (IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[1]
                {
                  new FunXMPP.KeyValue("count", messageInfoState.Count.ToString())
                };
                List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList = new List<FunXMPP.ProtocolTreeNode>();
                if (messageInfoState.Played.Any<FunXMPP.MessageInfoResponse>())
                  protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("played", (FunXMPP.KeyValue[]) null, messageInfoState.Played.Select<FunXMPP.MessageInfoResponse, FunXMPP.ProtocolTreeNode>((Func<FunXMPP.MessageInfoResponse, FunXMPP.ProtocolTreeNode>) (r => new FunXMPP.ProtocolTreeNode("item", new FunXMPP.KeyValue[2]
                  {
                    new FunXMPP.KeyValue("jid", r.Jid),
                    new FunXMPP.KeyValue("t", r.Timestamp.ToUnixTime().ToString())
                  }, (byte[]) null))).ToArray<FunXMPP.ProtocolTreeNode>()));
                if (messageInfoState.Read.Any<FunXMPP.MessageInfoResponse>())
                  protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("read", (FunXMPP.KeyValue[]) null, messageInfoState.Read.Select<FunXMPP.MessageInfoResponse, FunXMPP.ProtocolTreeNode>((Func<FunXMPP.MessageInfoResponse, FunXMPP.ProtocolTreeNode>) (r => new FunXMPP.ProtocolTreeNode("item", new FunXMPP.KeyValue[2]
                  {
                    new FunXMPP.KeyValue("jid", r.Jid),
                    new FunXMPP.KeyValue("t", r.Timestamp.ToUnixTime().ToString())
                  }, (byte[]) null))).ToArray<FunXMPP.ProtocolTreeNode>()));
                if (messageInfoState.Delivered.Any<FunXMPP.MessageInfoResponse>())
                  protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("delivery", (FunXMPP.KeyValue[]) null, messageInfoState.Delivered.Select<FunXMPP.MessageInfoResponse, FunXMPP.ProtocolTreeNode>((Func<FunXMPP.MessageInfoResponse, FunXMPP.ProtocolTreeNode>) (r => new FunXMPP.ProtocolTreeNode("item", new FunXMPP.KeyValue[2]
                  {
                    new FunXMPP.KeyValue("jid", r.Jid),
                    new FunXMPP.KeyValue("t", r.Timestamp.ToUnixTime().ToString())
                  }, (byte[]) null))).ToArray<FunXMPP.ProtocolTreeNode>()));
                responseNodes = (IEnumerable<FunXMPP.ProtocolTreeNode>) protocolTreeNodeList;
                metricsType = QrMetricsMapping.RESPONSE_MESSAGE_INFO;
              }
            }
            else if (type == "emoji")
            {
              IEnumerable<Pair<string, double>> emojis = this.EventHandler.Qr.GetEmojis(node.GetAllChildren("item").Select<FunXMPP.ProtocolTreeNode, Pair<string, double>>((Func<FunXMPP.ProtocolTreeNode, Pair<string, double>>) (n => new Pair<string, double>(n.GetAttributeValue("code"), double.Parse(n.GetAttributeValue("value"))))));
              IEnumerable<FunXMPP.ProtocolTreeNode> protocolTreeNodes = (IEnumerable<FunXMPP.ProtocolTreeNode>) null;
              if (emojis != null && emojis.Any<Pair<string, double>>())
                protocolTreeNodes = emojis.Select<Pair<string, double>, FunXMPP.ProtocolTreeNode>((Func<Pair<string, double>, FunXMPP.ProtocolTreeNode>) (p => new FunXMPP.ProtocolTreeNode("item", new FunXMPP.KeyValue[2]
                {
                  new FunXMPP.KeyValue("code", p.First),
                  new FunXMPP.KeyValue("value", p.Second.ToString())
                }, (byte[]) null)));
              responseNodes = protocolTreeNodes;
              metricsType = QrMetricsMapping.RESPONSE_EMOJI;
            }
            else if (type == "star")
            {
              string attributeValue13 = node.GetAttributeValue("count");
              string attributeValue14 = node.GetAttributeValue("chat");
              string attributeValue15 = node.GetAttributeValue("index");
              string attributeValue16 = node.GetAttributeValue("jid");
              bool fromMe = node.GetAttributeValue("owner") == "true";
              string attributeValue17 = node.GetAttributeValue("media");
              int num = 0;
              ref int local = ref num;
              Message[] msgs;
              if (int.TryParse(attributeValue13, out local) && num > 0)
              {
                FunXMPP.FMessage.Type[] types = (FunXMPP.FMessage.Type[]) null;
                if (attributeValue17 == "gif")
                  types = new FunXMPP.FMessage.Type[1]
                  {
                    FunXMPP.FMessage.Type.Gif
                  };
                msgs = this.EventHandler.Qr.GetStarredMessagesBefore(attributeValue14, attributeValue16, fromMe, attributeValue15, new int?(num), true, types);
              }
              else
                msgs = new Message[0];
              QrMetricsMapping metrics = QrMetricsMapping.UNKNOWN;
              responseNodes = FunXMPP.Connection.QrSerializeMessages((IEnumerable<Message>) msgs, ref metrics, false);
              metricsType = QrMetricsMapping.RESPONSE_STAR;
            }
            else if (type == "search")
            {
              string search = node.GetAttributeValue("search");
              string jid = node.GetAttributeValue("jid");
              string s1 = node.GetAttributeValue("page");
              if (string.IsNullOrEmpty(s1))
                s1 = "1";
              string s2 = node.GetAttributeValue("count");
              if (string.IsNullOrEmpty(s2))
                s2 = "50";
              int count = 0;
              int result = 0;
              Message[] msgs = (Message[]) null;
              if (int.TryParse(s2, out count) && count > 0 && int.TryParse(s1, out result) && result > 0 && !string.IsNullOrEmpty(search))
              {
                search += "*";
                int offset = (result - 1) * count;
                MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
                {
                  MessagesContext messagesContext = db;
                  string query = search;
                  int? offset1 = new int?(offset);
                  int? limit = new int?(count);
                  string[] jids;
                  if (jid == null)
                    jids = (string[]) null;
                  else
                    jids = new string[1]{ jid };
                  MessageSearchResult[] messageSearchResultArray = messagesContext.QueryFtsTableWithOffsets(query, offset1, limit, jids);
                  List<Message> messageList = new List<Message>();
                  foreach (MessageSearchResult messageSearchResult in messageSearchResultArray)
                    messageList.Add(messageSearchResult.Message);
                  msgs = messageList.ToArray();
                  if (msgs != null && msgs.Length >= count)
                    return;
                  responseAttributes = (IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[1]
                  {
                    new FunXMPP.KeyValue("last", "true")
                  };
                }));
              }
              else
                msgs = new Message[0];
              QrMetricsMapping metrics = QrMetricsMapping.UNKNOWN;
              responseNodes = FunXMPP.Connection.QrSerializeMessages((IEnumerable<Message>) msgs, ref metrics, false);
              metricsType = QrMetricsMapping.RESPONSE_SEARCH;
            }
            else if (type == "identity")
            {
              string attributeValue = node.GetAttributeValue("jid");
              string Displayable = (string) null;
              byte[] ScannableBytes = (byte[]) null;
              AppState.GetConnection().Encryption.IdentityGetFingerprintBytes(attributeValue, out Displayable, out ScannableBytes);
              responseNodes = (IEnumerable<FunXMPP.ProtocolTreeNode>) new List<FunXMPP.ProtocolTreeNode>()
              {
                new FunXMPP.ProtocolTreeNode("raw", (FunXMPP.KeyValue[]) null, ScannableBytes),
                new FunXMPP.ProtocolTreeNode("text", (FunXMPP.KeyValue[]) null, Displayable)
              };
              metricsType = QrMetricsMapping.RESPONSE_IDENTITY;
            }
            else if (type == "url")
            {
              string attributeValue = node.GetAttributeValue("url");
              if (!string.IsNullOrEmpty(attributeValue))
              {
                Log.d("WebClient", "url query string: {0}", (object) attributeValue);
                string matchedText = LinkPreviewUtils.GetLink(attributeValue);
                if (matchedText != null)
                {
                  string uri;
                  try
                  {
                    uri = new Uri(matchedText).ToString();
                  }
                  catch (Exception ex)
                  {
                    string context = "Exception creating Uri for " + matchedText;
                    Log.LogException(ex, context);
                    uri = (string) null;
                  }
                  Log.d("WebClient", "text:{0}, uri:{1}", (object) matchedText, (object) (uri ?? ""));
                  if (!string.IsNullOrEmpty(uri))
                  {
                    flag1 = true;
                    this.linkMetadataSub.SafeDispose();
                    this.linkMetadataSub = LinkPreviewUtils.FetchWebPageMetadata(uri).SubscribeOn<WebPageMetadata>((IScheduler) AppState.Worker).ObserveOnDispatcher<WebPageMetadata>().Subscribe<WebPageMetadata>((Action<WebPageMetadata>) (data =>
                    {
                      List<FunXMPP.KeyValue> urlAttributes = new List<FunXMPP.KeyValue>();
                      if (!string.IsNullOrEmpty(data.Title))
                        urlAttributes.Add(new FunXMPP.KeyValue("title", data.Title));
                      if (!string.IsNullOrEmpty(data.Description))
                        urlAttributes.Add(new FunXMPP.KeyValue("description", data.Description));
                      if (!string.IsNullOrEmpty(data.CanonicalUrl))
                        urlAttributes.Add(new FunXMPP.KeyValue("canonical-url", data.CanonicalUrl));
                      if (!string.IsNullOrEmpty(matchedText))
                        urlAttributes.Add(new FunXMPP.KeyValue("matched-text", matchedText));
                      this.thumbSub.SafeDispose();
                      this.thumbSub = (IDisposable) null;
                      if (data.ThumbnailUrl == null)
                        return;
                      this.thumbSub = data.LoadThumbnail().ObserveOnDispatcher<BitmapSource>().Subscribe<BitmapSource>((Action<BitmapSource>) (x =>
                      {
                        dataBytes = data.ThumbnailBytes;
                        this.thumbSub.SafeDispose();
                        this.thumbSub = (IDisposable) null;
                        if (!error.HasValue)
                        {
                          bool flag5 = false;
                          if (urlAttributes != null)
                          {
                            foreach (FunXMPP.KeyValue keyValue in urlAttributes)
                            {
                              if ((keyValue.key.Equals("title") || keyValue.key.Equals("description")) && !string.IsNullOrEmpty(keyValue.value))
                              {
                                flag5 = true;
                                break;
                              }
                            }
                          }
                          if (!flag5)
                            error = new int?(404);
                        }
                        responseAttributes = (IEnumerable<FunXMPP.KeyValue>) urlAttributes;
                        metricsType = QrMetricsMapping.RESPONSE_URL;
                        if (error.HasValue)
                        {
                          response = this.BuildErrorResponse(error.Value, ref encryptedResponse);
                        }
                        else
                        {
                          response = this.BuildQrResponse(id, type, responseNodes, responseAttributes);
                          response.data = dataBytes;
                        }
                        this.SendQrResponse(id, response, encryptedResponse, new QrMetricsMapping?(metricsType));
                        postResponse.ForEach((Action<Action>) (a => a()));
                      }));
                    }), (Action<Exception>) (ex =>
                    {
                      error = new int?(501);
                      this.linkMetadataSub.SafeDispose();
                      this.linkMetadataSub = (IDisposable) null;
                    }), (Action) (() => this.linkMetadataSub = (IDisposable) null));
                  }
                  else
                    error = new int?(400);
                }
                else
                  error = new int?(400);
              }
              if (flag1)
                return;
            }
            else if (type == "vcard")
            {
              string jid = node.GetAttributeValue("jid");
              List<Message> msgs = new List<Message>();
              MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
              {
                foreach (Message msg in db.GetMessagesByType(FunXMPP.FMessage.Type.Contact))
                {
                  foreach (ContactVCard contactCard in msg.GetContactCards())
                  {
                    foreach (ContactVCard.PhoneNumber phoneNumber in (IEnumerable<ContactVCard.PhoneNumber>) contactCard.PhoneNumbers)
                    {
                      if (jid == phoneNumber.Jid)
                        msgs.Add(msg);
                    }
                  }
                }
              }));
              QrMetricsMapping metrics = QrMetricsMapping.UNKNOWN;
              responseNodes = FunXMPP.Connection.QrSerializeMessages((IEnumerable<Message>) msgs, ref metrics, false);
              metricsType = QrMetricsMapping.RESPONSE_VCARD;
            }
            else if (type == "status")
            {
              if (string.IsNullOrEmpty(node.GetAttributeValue("chat")))
              {
                List<WaStatusThread> statusThreads = (List<WaStatusThread>) null;
                MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
                {
                  List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList = new List<FunXMPP.ProtocolTreeNode>();
                  statusThreads = db.GetStatusThreads(true);
                  Log.d("WebClient", "Status thread count {0}", statusThreads == null ? (object) "-1" : (object) statusThreads.Count.ToString());
                  int num = 0;
                  foreach (WaStatusThread waStatusThread in statusThreads)
                  {
                    WaStatus[] statuses = db.GetStatuses(new string[1]
                    {
                      waStatusThread.Jid
                    }, (string[]) null, false, true, new TimeSpan?(WaStatus.Expiration));
                    List<Message> msgs = new List<Message>();
                    foreach (WaStatus waStatus in statuses)
                    {
                      msgs.Add(db.GetMessageById(waStatus.MessageId));
                      ++num;
                    }
                    Message messageById = db.GetMessageById(waStatusThread.LatestStatus.MessageId);
                    QrMetricsMapping metrics = QrMetricsMapping.UNKNOWN;
                    protocolTreeNodeList.Add(new FunXMPP.ProtocolTreeNode("status", new FunXMPP.KeyValue[4]
                    {
                      new FunXMPP.KeyValue("jid", waStatusThread.Jid),
                      new FunXMPP.KeyValue("unread", (waStatusThread.Count - waStatusThread.ViewedCount).ToString()),
                      new FunXMPP.KeyValue("count", waStatusThread.Count.ToString()),
                      new FunXMPP.KeyValue("t", messageById.TimestampLong.ToString())
                    }, FunXMPP.Connection.QrSerializeMessages((IEnumerable<Message>) msgs, ref metrics, false).ToArray<FunXMPP.ProtocolTreeNode>()));
                  }
                  Log.d("WebClient", "Status message count {0}", (object) num);
                  responseAttributes = (IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[1]
                  {
                    new FunXMPP.KeyValue("checksum", Settings.ContactsChecksum.ToString())
                  };
                  responseNodes = (IEnumerable<FunXMPP.ProtocolTreeNode>) protocolTreeNodeList;
                }));
              }
              else
              {
                string attributeValue = node.GetAttributeValue("count");
                int count = string.IsNullOrEmpty(attributeValue) ? 20 : int.Parse(attributeValue);
                string kind = string.IsNullOrEmpty(node.GetAttributeValue("kind")) ? "after" : node.GetAttributeValue("kind");
                string jid = node.GetAttributeValue("jid");
                string index = node.GetAttributeValue("index");
                string owner = node.GetAttributeValue("owner");
                MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
                {
                  IEnumerable<Message> msgs = (IEnumerable<Message>) null;
                  if (!string.IsNullOrEmpty(jid) && !string.IsNullOrEmpty(index) && !string.IsNullOrEmpty(owner))
                  {
                    WaStatus waStatus1 = db.GetWaStatus(jid, index);
                    if (waStatus1 != null)
                    {
                      WaStatus[] statuses = db.GetStatuses(new string[1]
                      {
                        jid
                      }, (string[]) null, false, true, new TimeSpan?(WaStatus.Expiration), count, new int?(waStatus1.StatusId), (kind == "after" ? 1 : 0) != 0);
                      List<Message> messageList = new List<Message>();
                      foreach (WaStatus waStatus2 in statuses)
                        messageList.Add(db.GetMessageById(waStatus2.MessageId));
                      msgs = (IEnumerable<Message>) messageList.ToArray();
                    }
                  }
                  else
                  {
                    WaStatus oldestStatus = db.GetOldestStatus(jid);
                    if (oldestStatus != null)
                    {
                      WaStatus[] statuses = db.GetStatuses(new string[1]
                      {
                        jid
                      }, (string[]) null, false, true, new TimeSpan?(WaStatus.Expiration), count, new int?(oldestStatus.StatusId));
                      List<Message> messageList = new List<Message>();
                      foreach (WaStatus waStatus in statuses)
                        messageList.Add(db.GetMessageById(waStatus.MessageId));
                      msgs = (IEnumerable<Message>) messageList.ToArray();
                    }
                  }
                  if (msgs != null)
                  {
                    QrMetricsMapping metrics = QrMetricsMapping.UNKNOWN;
                    responseNodes = (IEnumerable<FunXMPP.ProtocolTreeNode>) FunXMPP.Connection.QrSerializeMessages(msgs, ref metrics, false).ToArray<FunXMPP.ProtocolTreeNode>();
                  }
                  else
                    error = new int?(404);
                }));
              }
            }
            else if (type == "location")
            {
              string attributeValue18 = node.GetAttributeValue("kind");
              string attributeValue19 = node.GetAttributeValue("jid");
              string attributeValue20 = node.GetAttributeValue("participant");
              switch (attributeValue18)
              {
                case "subscribe":
                  bool hasParticipants = attributeValue20 != null && attributeValue20 == "true";
                  LiveLocationManager.Instance.SubscribeToLocationUpdates(attributeValue19, hasParticipants);
                  responseAttributes = (IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[1]
                  {
                    new FunXMPP.KeyValue("duration", Settings.LiveLocationSubscriptionDuration.ToString())
                  };
                  responseNodes = hasParticipants ? (IEnumerable<FunXMPP.ProtocolTreeNode>) this.CreateParticipantNodesWithLocation(attributeValue19) : (IEnumerable<FunXMPP.ProtocolTreeNode>) new List<FunXMPP.ProtocolTreeNode>();
                  break;
                case "participant":
                  responseAttributes = (IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[1]
                  {
                    new FunXMPP.KeyValue("duration", Settings.LiveLocationSubscriptionDuration.ToString())
                  };
                  responseNodes = (IEnumerable<FunXMPP.ProtocolTreeNode>) this.CreateParticipantNodesWithoutLocation(attributeValue19);
                  break;
              }
            }
            if (responseNodes != null || responseAttributes != null)
            {
              response = this.BuildQrResponse(id, type, responseNodes, responseAttributes);
              if (dataBytes != null)
                response.data = dataBytes;
            }
            else if (!flag1)
              response = this.BuildErrorResponse(error ?? 501, ref encryptedResponse);
          }
          else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "action"))
          {
            if (type == "relay")
            {
              foreach (FunXMPP.ProtocolTreeNode protocolTreeNode in ((IEnumerable<FunXMPP.ProtocolTreeNode>) (node.children ?? new FunXMPP.ProtocolTreeNode[0])).Where<FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, bool>) (n => FunXMPP.ProtocolTreeNode.TagEquals(n, "message"))).Take<FunXMPP.ProtocolTreeNode>(1))
              {
                FunXMPP.FMessage fmessage = (FunXMPP.FMessage) null;
                if (protocolTreeNode.data != null)
                {
                  WebMessageInfo webMessageInfo = WebMessageInfo.Deserialize(protocolTreeNode.data);
                  fmessage = new FunXMPP.FMessage(new FunXMPP.FMessage.Key(webMessageInfo.Key.RemoteJid, webMessageInfo.Key.FromMe.GetValueOrDefault(), webMessageInfo.Key.Id));
                  DateTime? dt = new DateTime?();
                  ulong? messageTimestamp = webMessageInfo.MessageTimestamp;
                  fmessage.timestamp = !FunXMPP.TryParseTimestamp((long) messageTimestamp.Value, out dt) ? new DateTime?(DateTime.Now) : dt;
                  webMessageInfo.Message.PopulateFMessage(fmessage);
                  fmessage.urlPhoneNumber = ((int) webMessageInfo.UrlNumber ?? 0) != 0;
                  fmessage.urlText = ((int) webMessageInfo.UrlText ?? 0) != 0;
                }
                else if (protocolTreeNode.GetAttributeValue("type") == "ciphertext")
                {
                  fmessage = new FunXMPP.FMessage(new FunXMPP.FMessage.Key(protocolTreeNode.GetAttributeValue("from"), false, protocolTreeNode.GetAttributeValue(nameof (id))));
                  fmessage.media_wa_type = FunXMPP.FMessage.Type.CipherText;
                  DateTime? dt = new DateTime?();
                  fmessage.timestamp = string.IsNullOrEmpty(protocolTreeNode.GetAttributeValue("t")) || !FunXMPP.TryParseTimestamp(protocolTreeNode.GetAttributeValue("t"), out dt) ? new DateTime?(DateTime.Now) : dt;
                  if (!string.IsNullOrEmpty(protocolTreeNode.GetAttributeValue("participant")))
                    fmessage.remote_resource = protocolTreeNode.GetAttributeValue("participant");
                }
                if (fmessage != null && !this.EventHandler.Qr.OnRelay(fmessage))
                  response = this.BuildErrorResponse(200, ref encryptedResponse);
              }
            }
            else if (type == "set")
            {
              metricsType = QrMetricsMapping.RESPONSE_ACTION;
              bool flag6 = false;
              int? attributeInt = node.GetAttributeInt("epoch");
              int? nullable1 = new int?();
              if (attributeInt.HasValue)
                this.EventHandler.Qr.Session.ProcessEpoch(attributeInt.GetValueOrDefault());
              if (this.EventHandler.Qr.Session.PendingActions.Actions.ContainsKey(id))
              {
                response = this.BuildErrorResponse(this.EventHandler.Qr.Session.PendingActions.Actions[id], ref encryptedResponse);
              }
              else
              {
                Action ackActionCopy = ackAction;
                Action<string, int> sendResponse = (Action<string, int>) ((sender, code) =>
                {
                  FunXMPP.ProtocolTreeNode[] nodes = new FunXMPP.ProtocolTreeNode[1]
                  {
                    this.BuildQrActionItem(sender, code)
                  };
                  this.SendQrResponse(id, this.BuildQrResponse((string) null, "action", (IEnumerable<FunXMPP.ProtocolTreeNode>) nodes, (IEnumerable<FunXMPP.KeyValue>) null), true, new QrMetricsMapping?(metricsType));
                });
                Action onComplete = (Action) (() =>
                {
                  this.EventHandler.Qr.Session.SetActionStatus(id, 200);
                  ackActionCopy();
                  sendResponse(id, 200);
                });
                Action<int> onError = (Action<int>) (error =>
                {
                  this.EventHandler.Qr.Session.SetActionStatus(id, error);
                  ackActionCopy();
                  sendResponse(id, error);
                });
                foreach (FunXMPP.ProtocolTreeNode node1 in ((IEnumerable<FunXMPP.ProtocolTreeNode>) (node.children ?? new FunXMPP.ProtocolTreeNode[0])).Take<FunXMPP.ProtocolTreeNode>(1))
                {
                  if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "group"))
                  {
                    string jid = node1.GetAttributeValue("jid");
                    string attributeValue21 = node1.GetAttributeValue(nameof (id));
                    string attributeValue22 = node1.GetAttributeValue("type");
                    string attributeValue23 = node1.GetAttributeValue("author");
                    string attributeValue24 = node1.GetAttributeValue("subject");
                    string[] array = ((IEnumerable<FunXMPP.ProtocolTreeNode>) (node1.children ?? new FunXMPP.ProtocolTreeNode[0])).Where<FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, bool>) (n => FunXMPP.ProtocolTreeNode.TagEquals(n, "participant"))).Select<FunXMPP.ProtocolTreeNode, string>((Func<FunXMPP.ProtocolTreeNode, string>) (n => n.GetAttributeValue("jid"))).Where<string>((Func<string, bool>) (j => !string.IsNullOrEmpty(j))).ToArray<string>();
                    FunXMPP.ProtocolTreeNode child = node1.GetChild("description");
                    string myJid = Settings.MyJid;
                    if (attributeValue23 != myJid)
                      return;
                    switch (attributeValue22)
                    {
                      case "remove":
                        if (array.Length != 0)
                        {
                          string str = string.Join(", ", array);
                          Log.l("WebClient", "Action: RemoveParticipant - " + jid + ": " + str);
                          Action<List<Pair<string, int>>> onComplete1 = (Action<List<Pair<string, int>>>) (pair => onComplete());
                          this.SendRemoveParticipants(jid, (IEnumerable<string>) array, onComplete1, onError, attributeValue21);
                          break;
                        }
                        break;
                      case "add":
                        if (array.Length != 0)
                        {
                          string str = string.Join(", ", array);
                          Log.l("WebClient", "Action: AddParticipant - " + jid + ": " + str);
                          Action<List<Pair<string, int>>> onComplete2 = (Action<List<Pair<string, int>>>) (pair => onComplete());
                          this.SendAddParticipants(jid, (IEnumerable<string>) array, onComplete2, onError, attributeValue21);
                          break;
                        }
                        break;
                      case "promote":
                        if (array.Length != 0)
                        {
                          string str = string.Join(", ", array);
                          Log.l("WebClient", "Action: PromoteParticipant - " + jid + ": " + str);
                          Action<List<Pair<string, int>>> onComplete3 = (Action<List<Pair<string, int>>>) (pair => onComplete());
                          this.SendPromoteParticipant(jid, (IEnumerable<string>) array, onComplete3, onError, attributeValue21);
                          break;
                        }
                        break;
                      case "leave":
                        Log.l("WebClient", "Action: LeaveGroup - " + jid);
                        this.SendLeaveGroup(jid, attributeValue21).Subscribe<Unit>((Action<Unit>) (onNext => onComplete()), (Action<Exception>) (exception => onError(int.Parse(exception.Message))), (Action) (() => { }));
                        break;
                      case "subject":
                        Log.l("WebClient", "Action: SetSubject - " + jid);
                        this.SendSetGroupSubject(jid, attributeValue24, onComplete, onError, attributeValue21);
                        break;
                      case "description":
                        GroupDescription description1 = (GroupDescription) null;
                        if (child != null)
                        {
                          if (child.GetAttributeValue("delete") == "true")
                          {
                            description1 = new GroupDescription("", child.GetAttributeValue(nameof (id)));
                            description1.PreviousId = child.GetAttributeValue("prev");
                          }
                          else
                          {
                            description1 = new GroupDescription(child.GetDataString(), child.GetAttributeValue(nameof (id)));
                            description1.PreviousId = child.GetAttributeValue("prev");
                          }
                        }
                        this.SendSetGroupDescription(jid, description1, onComplete, onError);
                        break;
                      case "create":
                        Log.l("WebClient", "Action: CreateGroup - " + jid);
                        GroupDescription description2 = (GroupDescription) null;
                        if (child != null)
                          description2 = new GroupDescription(child.GetDataString(), child.GetAttributeValue(nameof (id)));
                        bool flag7 = node1.GetChild("announcement") != null;
                        bool flag8 = node1.GetChild("locked") != null;
                        Action<string, GroupDescription, List<string>, List<Pair<string, int>>> onSuccess = (Action<string, GroupDescription, List<string>, List<Pair<string, int>>>) ((a, b, c, d) => onComplete());
                        GroupProperties properties = new GroupProperties()
                        {
                          IsAnnounceOnly = flag7,
                          IsRestricted = flag8
                        };
                        this.SendCreateGroupChat(attributeValue24, (IEnumerable<string>) array, onSuccess, onError, attributeValue21, description2, properties);
                        break;
                      case "prop":
                        FunXMPP.ProtocolTreeNode announcement = node1.GetChild("announcement");
                        FunXMPP.ProtocolTreeNode locked = node1.GetChild("locked");
                        string toggleState = (string) null;
                        Conversation convo = (Conversation) null;
                        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
                        {
                          convo = db.GetConversation(jid, CreateOptions.None);
                          if (convo == null)
                            return;
                          if (announcement != null)
                          {
                            toggleState = announcement.GetAttributeValue("value");
                            switch (toggleState)
                            {
                              case "true":
                                this.SendSetAnnouncementOnlyGroup(jid, true);
                                break;
                              case "false":
                                this.SendSetAnnouncementOnlyGroup(jid, false);
                                break;
                            }
                          }
                          else
                          {
                            if (locked == null)
                              return;
                            toggleState = locked.GetAttributeValue("value");
                            switch (toggleState)
                            {
                              case "true":
                                this.SendSetGroupRestrict(jid, true);
                                break;
                              case "false":
                                this.SendSetGroupRestrict(jid, false);
                                break;
                            }
                          }
                        }));
                        break;
                    }
                    ackAction = (Action) null;
                  }
                  else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "read"))
                  {
                    string attributeValue25 = node1.GetAttributeValue("jid");
                    string attributeValue26 = node1.GetAttributeValue("index");
                    bool fromMe = node1.GetAttributeValue("owner") == "true";
                    int? count = new int?();
                    string attributeValue27 = node1.GetAttributeValue("count");
                    if (attributeValue27 != null)
                    {
                      int result = 0;
                      count = int.TryParse(attributeValue27, out result) ? new int?(result) : new int?();
                    }
                    string attributeValue28 = node1.GetAttributeValue("participant");
                    if (!string.IsNullOrEmpty(attributeValue25))
                    {
                      string attributeValue29 = node1.GetAttributeValue("kind");
                      if (!string.IsNullOrEmpty(attributeValue29) && attributeValue29 == "status")
                      {
                        string attributeValue30 = node1.GetAttributeValue("chat");
                        nullable1 = new int?(this.EventHandler.Qr.OnMarkAsSeen(attributeValue25, attributeValue26, fromMe, attributeValue30));
                      }
                      else
                        nullable1 = new int?(this.EventHandler.Qr.OnMarkAsRead(attributeValue25, attributeValue26, fromMe, attributeValue28, count));
                    }
                    flag6 = true;
                  }
                  else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "picture"))
                  {
                    string attributeValue31 = node1.GetAttributeValue(nameof (id));
                    node1.GetAttributeValue("type");
                    string attributeValue32 = node1.GetAttributeValue("jid");
                    Func<FunXMPP.ProtocolTreeNode, byte[]> func = (Func<FunXMPP.ProtocolTreeNode, byte[]>) (n => n?.data);
                    byte[] bytes = func(node1.GetChild("image"));
                    byte[] thumbnailBytes = func(node1.GetChild("preview"));
                    this.SendSetPhoto(attributeValue32, bytes, thumbnailBytes, onComplete, onError, attributeValue31);
                    ackAction = (Action) null;
                  }
                  else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "presence"))
                  {
                    string attributeValue33 = node1.GetAttributeValue("type");
                    string attributeValue34 = node1.GetAttributeValue("to");
                    string attributeValue35 = node1.GetAttributeValue("jid");
                    switch (attributeValue33)
                    {
                      case "subscribe":
                        PresenceState.Instance.GetPresence(attributeValue34).Take<PresenceEventArgs>(1).Subscribe<PresenceEventArgs>();
                        nullable1 = new int?(200);
                        flag6 = true;
                        continue;
                      case "available":
                        this.EventHandler.Qr.OnAvailable(true, notificationTimestamp);
                        nullable1 = new int?(200);
                        flag6 = true;
                        continue;
                      case "unavailable":
                        this.EventHandler.Qr.OnAvailable(false, notificationTimestamp);
                        continue;
                      case "composing":
                        this.EventHandler.Qr.OnComposing(attributeValue34, attributeValue35, Presence.OnlineAndTyping);
                        continue;
                      case "recording":
                        this.EventHandler.Qr.OnComposing(attributeValue34, attributeValue35, Presence.OnlineAndRecording);
                        continue;
                      case "paused":
                        this.EventHandler.Qr.OnComposing(attributeValue34, attributeValue35, Presence.Online);
                        continue;
                      default:
                        continue;
                    }
                  }
                  else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "received"))
                  {
                    string attributeValue36 = node1.GetAttributeValue("type");
                    string attributeValue37 = node1.GetAttributeValue("index");
                    string attributeValue38 = node1.GetAttributeValue("from");
                    string attributeValue39 = node1.GetAttributeValue("participant");
                    nullable1 = new int?(this.EventHandler.Qr.OnReceipt(attributeValue36, attributeValue37, attributeValue38, attributeValue39));
                    if (attributeValue36 == "played")
                      flag6 = true;
                  }
                  else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "status"))
                  {
                    Log.l("WebClient", "Action: SetStatus");
                    string jid = node1.GetAttributeValue("jid");
                    if (!string.IsNullOrEmpty(jid))
                    {
                      string mute = node1.GetAttributeValue("mute");
                      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
                      {
                        db.GetJidInfo(jid, CreateOptions.CreateToDbIfNotFound).IsStatusMuted = mute == "true";
                        db.SubmitChanges();
                      }));
                    }
                    else
                      this.EventHandler.Qr.OnSetStatus(node1.GetDataString(), id);
                    nullable1 = new int?(200);
                    flag6 = true;
                  }
                  else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "chat"))
                  {
                    FunXMPP.ChatResponse chat = new FunXMPP.ChatResponse();
                    chat.Jid = node1.GetAttributeValue("jid");
                    string attributeValue40 = node1.GetAttributeValue("index");
                    bool from_me = node1.GetAttributeValue("owner") == "true";
                    node1.GetAttributeValue("participant");
                    if (attributeValue40 != null)
                      chat.LastMessageKey = new FunXMPP.FMessage.Key(chat.Jid, from_me, attributeValue40);
                    string attributeValue41 = node1.GetAttributeValue("type");
                    switch (attributeValue41)
                    {
                      case "delete":
                        chat.SetType = new FunXMPP.ChatSetAction?(FunXMPP.ChatSetAction.Delete);
                        break;
                      case "clear":
                        chat.SetType = new FunXMPP.ChatSetAction?(FunXMPP.ChatSetAction.Clear);
                        break;
                      case "archive":
                        chat.SetType = new FunXMPP.ChatSetAction?(FunXMPP.ChatSetAction.Archive);
                        chat.Archived = true;
                        break;
                      case "unarchive":
                        chat.SetType = new FunXMPP.ChatSetAction?(FunXMPP.ChatSetAction.Archive);
                        chat.Archived = false;
                        break;
                      case "mute":
                        if (!string.IsNullOrEmpty(node1.GetAttributeValue("mute")))
                        {
                          chat.SetType = new FunXMPP.ChatSetAction?(FunXMPP.ChatSetAction.Mute);
                          chat.MuteExpiration = node1.GetAttributeDateTime("mute");
                          break;
                        }
                        chat.SetType = new FunXMPP.ChatSetAction?(FunXMPP.ChatSetAction.Unmute);
                        chat.MuteExpiration = node1.GetAttributeDateTime("previous");
                        break;
                      case "pin":
                        if (!string.IsNullOrEmpty(node1.GetAttributeValue("pin")))
                        {
                          chat.SetType = new FunXMPP.ChatSetAction?(FunXMPP.ChatSetAction.Pin);
                          chat.PinTimestamp = node1.GetAttributeDateTime("pin");
                          break;
                        }
                        chat.SetType = new FunXMPP.ChatSetAction?(FunXMPP.ChatSetAction.Unpin);
                        chat.PinTimestamp = node1.GetAttributeDateTime("previous");
                        break;
                      case "unstar":
                        chat.SetType = new FunXMPP.ChatSetAction?(FunXMPP.ChatSetAction.Unstar);
                        break;
                      case "spam":
                        string attributeValue42 = node1.GetAttributeValue("spam");
                        if (string.IsNullOrEmpty(attributeValue42) || attributeValue42 == "true")
                        {
                          chat.Spam = true;
                          break;
                        }
                        chat.Spam = false;
                        chat.SetType = new FunXMPP.ChatSetAction?(FunXMPP.ChatSetAction.NotSpam);
                        break;
                    }
                    if (attributeValue41 != "spam")
                      flag6 = true;
                    bool flag9 = node1.GetAttributeValue("star") == "true";
                    FunXMPP.FMessage.Key[] array = ((IEnumerable<FunXMPP.ProtocolTreeNode>) (node1.children ?? new FunXMPP.ProtocolTreeNode[0])).Where<FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, bool>) (n => FunXMPP.ProtocolTreeNode.TagEquals(n, "item"))).Select<FunXMPP.ProtocolTreeNode, FunXMPP.FMessage.Key>((Func<FunXMPP.ProtocolTreeNode, FunXMPP.FMessage.Key>) (n => new FunXMPP.FMessage.Key(chat.Jid, n.GetAttributeValue("owner") == "true", n.GetAttributeValue("index")))).ToArray<FunXMPP.FMessage.Key>();
                    if (attributeValue41 == "clear" && array != null && array.Length != 0)
                    {
                      this.EventHandler.Qr.OnDeleteMessages(array);
                      nullable1 = new int?(200);
                    }
                    else if ((attributeValue41 == "star" || attributeValue41 == "unstar") && array != null && array.Length != 0)
                    {
                      this.EventHandler.Qr.OnStarMessages(attributeValue41 == "star", array);
                      nullable1 = new int?(200);
                    }
                    else if (attributeValue41 == "clear" && !flag9)
                    {
                      ClearChat.Clear(chat.Jid, true);
                      nullable1 = new int?(200);
                    }
                    else if (attributeValue41 == "modify")
                    {
                      this.EventHandler.Qr.OnChangeNumberNotificationDismiss(chat.Jid);
                      nullable1 = new int?(200);
                    }
                    else
                      nullable1 = new int?(this.EventHandler.Qr.OnSetChat(chat));
                  }
                  else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "block"))
                  {
                    string subType = node1.GetAttributeValue("type");
                    System.Collections.Generic.Dictionary<string, string> participants = ((IEnumerable<FunXMPP.ProtocolTreeNode>) (node1.children ?? new FunXMPP.ProtocolTreeNode[0])).Where<FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, bool>) (n => FunXMPP.ProtocolTreeNode.TagEquals(n, "user"))).ToDictionary<FunXMPP.ProtocolTreeNode, string, string>((Func<FunXMPP.ProtocolTreeNode, string>) (n => n.GetAttributeValue("jid")), (Func<FunXMPP.ProtocolTreeNode, string>) (n => n.GetAttributeValue("reason")));
                    System.Collections.Generic.Dictionary<string, string> jidsToBlock = (System.Collections.Generic.Dictionary<string, string>) null;
                    ContactsContext.Instance((Action<ContactsContext>) (cdb =>
                    {
                      jidsToBlock = cdb.BlockListSet.ToDictionary<KeyValuePair<string, bool>, string, string>((Func<KeyValuePair<string, bool>, string>) (p => p.Key), (Func<KeyValuePair<string, bool>, string>) (p => (string) null));
                      if (subType == "add")
                      {
                        foreach (string key in participants.Keys)
                          jidsToBlock[key] = participants[key];
                      }
                      else
                      {
                        foreach (string key in participants.Keys)
                        {
                          if (jidsToBlock.ContainsKey(key))
                            jidsToBlock.Remove(key);
                        }
                      }
                    }));
                    this.SendSetBlockList(jidsToBlock, (Action) (() =>
                    {
                      string[] jidsToBlockArray = jidsToBlock.Keys.ToArray<string>();
                      ContactsContext.Instance((Action<ContactsContext>) (cdb =>
                      {
                        cdb.BlockListSet.Clear();
                        foreach (string key in jidsToBlockArray)
                          cdb.BlockListSet.Add(key, true);
                        cdb.FlushBlockList();
                        cdb.SubmitChanges();
                      }));
                      MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.ClearWaStatuses(jidsToBlockArray)));
                      onComplete();
                    }), onError, id);
                    ackAction = (Action) null;
                  }
                  else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "spam"))
                  {
                    bool flag10 = false;
                    string jid = node1.GetAttributeValue("jid");
                    if (!string.IsNullOrEmpty(jid))
                    {
                      bool flag11 = JidHelper.IsGroupJid(jid);
                      if (JidHelper.IsUserJid(jid) | flag11)
                      {
                        Conversation convo = (Conversation) null;
                        Message[] msgsToReport = (Message[]) null;
                        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
                        {
                          convo = db.GetConversation(jid, CreateOptions.None);
                          if (convo == null)
                            return;
                          msgsToReport = db.GetLatestMessages(convo.Jid, convo.MessageLoadingStart(), new int?(5), new int?(0));
                        }));
                        if (convo != null)
                        {
                          try
                          {
                            this.SendSpamReport(msgsToReport, "chat", convo.Jid, flag11 ? convo.GroupOwner : (string) null, flag11 ? convo.GroupSubject : (string) null, onComplete, onError, id);
                            ackAction = (Action) null;
                            flag10 = true;
                          }
                          catch (Exception ex)
                          {
                            string context = string.Format("Relaying spam report for {0}", (object) convo.Jid);
                            Log.LogException(ex, context);
                          }
                        }
                      }
                    }
                    if (!flag10)
                      response = this.BuildErrorResponse(501, ref encryptedResponse);
                  }
                  else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "profile"))
                  {
                    string attributeValue = node1.GetAttributeValue("name");
                    if (!string.IsNullOrEmpty(attributeValue) && Settings.PushName != attributeValue)
                    {
                      Settings.PushName = attributeValue;
                      this.Protocol.TreeNodeWriter.Write(new FunXMPP.ProtocolTreeNode("presence", new FunXMPP.KeyValue[3]
                      {
                        new FunXMPP.KeyValue("name", attributeValue),
                        new FunXMPP.KeyValue(nameof (id), id),
                        new FunXMPP.KeyValue("web", "set")
                      }));
                    }
                    ackAction = (Action) null;
                  }
                  else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "contact"))
                    response = this.BuildErrorResponse(501, ref encryptedResponse);
                  else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "privacy"))
                  {
                    string attributeValue = node1.GetAttributeValue("kind");
                    if (!string.IsNullOrEmpty(attributeValue) && attributeValue == "status")
                    {
                      string privType = node1.GetAttributeValue("type");
                      if (!string.IsNullOrEmpty(privType))
                      {
                        if (privType == "blacklist" || privType == "whitelist")
                        {
                          string[] jids = ((IEnumerable<FunXMPP.ProtocolTreeNode>) (node.children ?? new FunXMPP.ProtocolTreeNode[0])).Where<FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, bool>) (n => FunXMPP.ProtocolTreeNode.TagEquals(n, "user"))).Select<FunXMPP.ProtocolTreeNode, string>((Func<FunXMPP.ProtocolTreeNode, string>) (n => n.GetAttributeValue("jid"))).ToArray<string>();
                          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
                          {
                            if (privType == "whitelist")
                            {
                              Settings.StatusV3PrivacySetting = WaStatusHelper.StatusPrivacySettings.WhiteList;
                              WaStatusHelper.SetWhiteList(db, jids, true, true);
                            }
                            else
                            {
                              Settings.StatusV3PrivacySetting = WaStatusHelper.StatusPrivacySettings.BlackList;
                              WaStatusHelper.SetBlackList(db, jids, true, true);
                            }
                          }));
                        }
                        else if (privType == "contacts")
                          Settings.StatusV3PrivacySetting = WaStatusHelper.StatusPrivacySettings.Contacts;
                      }
                    }
                  }
                  else if (FunXMPP.ProtocolTreeNode.TagEquals(node1, "location"))
                  {
                    switch (node1.GetAttributeValue("type"))
                    {
                      case "unsubscribe":
                        this.EventHandler.Qr.OnUnsubscribeLocation(node1.GetAttributeValue("jid"), id);
                        continue;
                      case "disable":
                        nullable1 = new int?(this.EventHandler.Qr.OnDisableLocation(node1.GetAttributeValue("jid"), id));
                        flag6 = true;
                        continue;
                      default:
                        continue;
                    }
                  }
                  else
                    response = this.BuildErrorResponse(501, ref encryptedResponse);
                }
              }
              if (response == null)
              {
                int? nullable2;
                if (nullable1.HasValue)
                {
                  nullable2 = nullable1;
                  int num = 400;
                  if ((nullable2.GetValueOrDefault() >= num ? (nullable2.HasValue ? 1 : 0) : 0) != 0)
                    goto label_307;
                }
                int num1;
                if (nullable1.HasValue)
                {
                  nullable2 = nullable1;
                  int num2 = 200;
                  num1 = nullable2.GetValueOrDefault() == num2 ? (nullable2.HasValue ? 1 : 0) : 0;
                }
                else
                  num1 = 1;
                int num3 = flag6 ? 1 : 0;
                if ((num1 & num3) == 0)
                  goto label_365;
label_307:
                response = this.BuildErrorResponse(nullable1.Value, ref encryptedResponse);
              }
            }
            else if (type == "debug")
            {
              foreach (FunXMPP.ProtocolTreeNode node2 in ((IEnumerable<FunXMPP.ProtocolTreeNode>) (node.children ?? new FunXMPP.ProtocolTreeNode[0])).Take<FunXMPP.ProtocolTreeNode>(1))
              {
                if (FunXMPP.ProtocolTreeNode.TagEquals(node2, "log"))
                  Log.SendSupportLog((string) null, false);
                else
                  response = this.BuildErrorResponse(501, ref encryptedResponse);
              }
            }
            else
              response = this.BuildErrorResponse(501, ref encryptedResponse);
          }
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "action"))
        {
          Func<FunXMPP.ProtocolTreeNode, byte[]> func = (Func<FunXMPP.ProtocolTreeNode, byte[]>) (n =>
          {
            try
            {
              return Convert.FromBase64String(n.GetDataString());
            }
            catch (Exception ex)
            {
              return (byte[]) null;
            }
          });
          if (type == "delete")
          {
            string attributeValue43 = node.GetAttributeValue("name");
            byte[] numArray = (byte[]) null;
            if (attributeValue43 != null)
            {
              try
              {
                numArray = Convert.FromBase64String(attributeValue43);
              }
              catch
              {
                numArray = (byte[]) null;
              }
            }
            if (node.GetAttributeValue("modify") == "clear")
            {
              string attributeValue44 = node.GetAttributeValue("code");
              byte[] code = (byte[]) null;
              if (attributeValue44 != null)
              {
                try
                {
                  code = Convert.FromBase64String(attributeValue44);
                }
                catch
                {
                  code = (byte[]) null;
                }
              }
              this.EventHandler.Qr.OnLogout(numArray, code);
            }
            else
            {
              DateTime? attributeDateTime = node.GetAttributeDateTime("t");
              this.EventHandler.Qr.OnDisconnect(node.data, numArray, attributeDateTime);
            }
          }
          else if (type == "sync")
          {
            FunXMPP.ProtocolTreeNode child1 = node.GetChild("sync");
            FunXMPP.ProtocolTreeNode child2 = node.GetChild("name");
            FunXMPP.ProtocolTreeNode child3 = node.GetChild("code");
            FunXMPP.ProtocolTreeNode child4 = node.GetChild("platform");
            FunXMPP.VerifySessionType type2 = FunXMPP.VerifySessionType.Normal;
            byte[] numArray = (byte[]) null;
            byte[] browserId = (byte[]) null;
            byte[] clientToken = (byte[]) null;
            string os = (string) null;
            string browser = (string) null;
            encryptedResponse = false;
            if (child1 != null)
            {
              numArray = child1.data;
              switch (child1.GetAttributeValue("type"))
              {
                case "resume":
                  type2 = !(child1.GetAttributeValue("kind") == "required") ? FunXMPP.VerifySessionType.Resume : FunXMPP.VerifySessionType.ForcedResume;
                  break;
                case "challenge":
                  type2 = FunXMPP.VerifySessionType.Challenge;
                  break;
                case "required":
                  type2 = FunXMPP.VerifySessionType.Takeover;
                  break;
              }
            }
            if (child2 != null)
              browserId = func(child2);
            if (child3 != null)
              clientToken = func(child3);
            if (child4 != null)
            {
              os = child4.GetAttributeValue("os");
              browser = child4.GetAttributeValue("browser");
            }
            FunXMPP.VerifySessionResponse verifySessionResponse = this.EventHandler.Qr.VerifyCurrent(type2, numArray, browserId, clientToken, postResponse, os, browser);
            string v = (string) null;
            switch (verifySessionResponse.Response)
            {
              case FunXMPP.VerifySessionResponse.ResponseType.Conflict:
                v = "conflict";
                break;
              case FunXMPP.VerifySessionResponse.ResponseType.Challenge:
                v = "challenge";
                break;
              case FunXMPP.VerifySessionResponse.ResponseType.Failure:
                v = "fail";
                break;
            }
            if (v != null)
            {
              List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>();
              keyValueList.Add(new FunXMPP.KeyValue("reason", v));
              if (verifySessionResponse.ChallengeData != null)
                keyValueList.Add(new FunXMPP.KeyValue("challenge", Convert.ToBase64String(verifySessionResponse.ChallengeData)));
              response = new FunXMPP.ProtocolTreeNode("deny", keyValueList.ToArray(), numArray);
            }
          }
          else
            response = this.BuildErrorResponse(501, ref encryptedResponse);
        }
        else if (FunXMPP.ProtocolTreeNode.TagEquals(node, "query"))
        {
          if (type == "sync")
          {
            FunXMPP.ProtocolTreeNode child = node.GetChild("sync");
            FunXMPP.VerifySessionType type3 = FunXMPP.VerifySessionType.Normal;
            byte[] numArray = (byte[]) null;
            encryptedResponse = false;
            if (child != null)
            {
              numArray = child.data;
              if (child.GetAttributeValue("type") == "query")
                type3 = FunXMPP.VerifySessionType.Query;
            }
            FunXMPP.VerifySessionResponse verifySessionResponse = this.EventHandler.Qr.VerifyQuerySync(type3, numArray);
            string str = (string) null;
            switch (verifySessionResponse.Response)
            {
              case FunXMPP.VerifySessionResponse.ResponseType.Failure:
                str = "fail";
                break;
            }
            if (str == null)
              response = new FunXMPP.ProtocolTreeNode("sync", new FunXMPP.KeyValue[1]
              {
                new FunXMPP.KeyValue("web", "query")
              }, numArray);
            else
              response = new FunXMPP.ProtocolTreeNode("deny", new FunXMPP.KeyValue[1]
              {
                new FunXMPP.KeyValue("web", "query")
              }, numArray);
          }
          else
            response = this.BuildErrorResponse(501, ref encryptedResponse);
        }
        else
          response = this.BuildErrorResponse(501, ref encryptedResponse);
label_365:
        if (response != null)
          this.SendQrResponse(id, response, encryptedResponse, new QrMetricsMapping?(metricsType));
        postResponse.ForEach((Action<Action>) (a => a()));
      }

      private static IEnumerable<FunXMPP.ProtocolTreeNode> QrSerializeMessages(
        IEnumerable<Message> msgs,
        ref QrMetricsMapping metrics,
        bool invis)
      {
        List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList = new List<FunXMPP.ProtocolTreeNode>();
        foreach (Message msg in msgs)
        {
          FunXMPP.ProtocolTreeNode protocolTreeNode = (FunXMPP.ProtocolTreeNode) null;
          switch (msg.MediaWaType)
          {
            case FunXMPP.FMessage.Type.Divider:
              if (protocolTreeNode != null)
              {
                protocolTreeNodeList.Add(protocolTreeNode);
                continue;
              }
              continue;
            case FunXMPP.FMessage.Type.System:
              protocolTreeNode = FunXMPP.Connection.ConvertSystemMessage(msg, ref metrics, invis);
              goto case FunXMPP.FMessage.Type.Divider;
            default:
              protocolTreeNode = FunXMPP.Connection.QrSerializeMessage(msg);
              goto case FunXMPP.FMessage.Type.Divider;
          }
        }
        return (IEnumerable<FunXMPP.ProtocolTreeNode>) protocolTreeNodeList;
      }

      private static WebMessageInfo BuildWebMessageInfoFromMessage(Message m)
      {
        FunXMPP.FMessage fmessage = m.ToFMessage();
        if (!fmessage.timestamp.HasValue)
        {
          Log.l("web client", "missing fun timestamp | {0}", (object) m.LogInfo());
          fmessage.timestamp = new DateTime?(DateTime.UtcNow);
        }
        WebMessageInfo webMessageInfo1 = new WebMessageInfo()
        {
          Key = new MessageKey()
        };
        webMessageInfo1.Key.FromMe = new bool?(m.KeyFromMe);
        webMessageInfo1.Key.Id = m.KeyId;
        webMessageInfo1.Key.RemoteJid = m.KeyRemoteJid;
        if (m.KeyFromMe)
        {
          switch (m.Status)
          {
            case FunXMPP.FMessage.Status.ReceivedByServer:
              webMessageInfo1.status = new WebMessageInfo.Status?(WebMessageInfo.Status.SERVER_ACK);
              break;
            case FunXMPP.FMessage.Status.ReceivedByTarget:
              webMessageInfo1.status = new WebMessageInfo.Status?(WebMessageInfo.Status.DELIVERY_ACK);
              break;
            case FunXMPP.FMessage.Status.Unsent:
              webMessageInfo1.status = new WebMessageInfo.Status?(WebMessageInfo.Status.PENDING);
              break;
            case FunXMPP.FMessage.Status.PlayedByTarget:
              webMessageInfo1.status = new WebMessageInfo.Status?(WebMessageInfo.Status.PLAYED);
              break;
            case FunXMPP.FMessage.Status.ReadByTarget:
              webMessageInfo1.status = new WebMessageInfo.Status?(WebMessageInfo.Status.READ);
              break;
            default:
              webMessageInfo1.status = new WebMessageInfo.Status?(WebMessageInfo.Status.ERROR);
              break;
          }
        }
        else if (JidHelper.IsPsaJid(m.KeyRemoteJid) && m.ShouldAlert() && !m.IsAutomuted)
          webMessageInfo1.status = new WebMessageInfo.Status?(WebMessageInfo.Status.DELIVERY_ACK);
        if (m.MediaWaType == FunXMPP.FMessage.Type.LiveLocation)
          webMessageInfo1.Duration = new uint?(Convert.ToUInt32(m.MediaDurationSeconds));
        webMessageInfo1.MessageTimestamp = new ulong?((ulong) fmessage.timestamp.Value.ToUnixTime());
        webMessageInfo1.Participant = JidHelper.IsGroupJid(m.KeyRemoteJid) || JidHelper.IsStatusJid(m.KeyRemoteJid) ? m.RemoteResource : (string) null;
        webMessageInfo1.Ignore = new bool?(!m.KeyFromMe && AppState.IsConversationOpen(m.KeyRemoteJid));
        webMessageInfo1.Starred = new bool?(m.IsStarred);
        webMessageInfo1.Broadcast = new bool?(JidHelper.IsBroadcastJid(m.KeyRemoteJid));
        webMessageInfo1.MediaCiphertextSha256 = (byte[]) null;
        MessageProperties forMessage = MessageProperties.GetForMessage(m);
        WebMessageInfo webMessageInfo2 = webMessageInfo1;
        bool? nullable1;
        int num;
        if (!forMessage.EnsureCommonProperties.Multicast.HasValue)
        {
          num = 0;
        }
        else
        {
          nullable1 = forMessage.EnsureCommonProperties.Multicast;
          num = nullable1.Value ? 1 : 0;
        }
        bool? nullable2 = new bool?(num != 0);
        webMessageInfo2.Multicast = nullable2;
        WebMessageInfo webMessageInfo3 = webMessageInfo1;
        nullable1 = forMessage.EnsureCommonProperties.UrlNumber;
        bool? nullable3 = new bool?(((int) nullable1 ?? 0) != 0);
        webMessageInfo3.UrlNumber = nullable3;
        WebMessageInfo webMessageInfo4 = webMessageInfo1;
        nullable1 = forMessage.EnsureCommonProperties.UrlText;
        bool? nullable4 = new bool?(((int) nullable1 ?? 0) != 0);
        webMessageInfo4.UrlText = nullable4;
        CipherTextIncludes includes = new CipherTextIncludes(true);
        webMessageInfo1.Message = WhatsApp.ProtoBuf.Message.CreateFromFMessage(fmessage, includes);
        if (webMessageInfo1.Message != null)
          return webMessageInfo1;
        Log.SendCrashLog(new Exception("Web message info should not be null"), "QrSerializeMessage");
        return (WebMessageInfo) null;
      }

      private static FunXMPP.ProtocolTreeNode QrSerializeMessage(Message m)
      {
        WebMessageInfo instance = FunXMPP.Connection.BuildWebMessageInfoFromMessage(m);
        switch (m.MediaWaType)
        {
          case FunXMPP.FMessage.Type.System:
            Log.l("WebClient", "QrSerializeMessage being called for system message!!");
            break;
          case FunXMPP.FMessage.Type.CipherText:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.CIPHERTEXT);
            break;
          case FunXMPP.FMessage.Type.ProtocolMessage:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.FUTUREPROOF);
            break;
          case FunXMPP.FMessage.Type.Revoked:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.REVOKE);
            break;
        }
        if (instance.MessageStubType.HasValue)
          instance.Message = (WhatsApp.ProtoBuf.Message) null;
        return new FunXMPP.ProtocolTreeNode("message", (FunXMPP.KeyValue[]) null, WebMessageInfo.SerializeToBytes(instance));
      }

      private static FunXMPP.ProtocolTreeNode ConvertSystemMessage(
        Message m,
        ref QrMetricsMapping metrics,
        bool invis = false)
      {
        return FunXMPP.Connection.ConvertSystemMessageNew(m, ref metrics) ?? FunXMPP.Connection.ConvertSystemMessageLegacy(m, ref metrics, invis);
      }

      private static FunXMPP.ProtocolTreeNode ConvertSystemMessageNew(
        Message m,
        ref QrMetricsMapping metrics)
      {
        bool flag = !m.KeyRemoteJid.IsGroupJid();
        WebMessageInfo instance = FunXMPP.Connection.BuildWebMessageInfoFromMessage(m);
        switch (m.GetSystemMessageType())
        {
          case SystemMessageWrapper.MessageTypes.ParticipantChange:
            string senderJid = m.GetSenderJid();
            string messageChangeAuthor = m.GetSystemMessageChangeAuthor();
            switch (m.GetSystemMessageChangeType())
            {
              case SystemMessageUtils.ParticipantChange.Leave:
                if (flag)
                {
                  instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.BROADCAST_REMOVE);
                  metrics = QrMetricsMapping.FORWARD_BC_UPDATE;
                  break;
                }
                instance.MessageStubType = !(messageChangeAuthor == senderJid) ? new WebMessageInfo.StubType?(WebMessageInfo.StubType.GROUP_PARTICIPANT_REMOVE) : new WebMessageInfo.StubType?(WebMessageInfo.StubType.GROUP_PARTICIPANT_LEAVE);
                break;
              case SystemMessageUtils.ParticipantChange.Join:
              case SystemMessageUtils.ParticipantChange.Added:
                if (flag)
                {
                  instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.BROADCAST_ADD);
                  metrics = QrMetricsMapping.FORWARD_BC_UPDATE;
                  break;
                }
                instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.GROUP_PARTICIPANT_ADD);
                break;
              case SystemMessageUtils.ParticipantChange.Removed:
                instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.GROUP_PARTICIPANT_REMOVE);
                break;
              case SystemMessageUtils.ParticipantChange.Invite:
                instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.GROUP_PARTICIPANT_INVITE);
                break;
            }
            instance.MessageStubParameters = new List<string>()
            {
              senderJid
            };
            instance.Participant = messageChangeAuthor;
            break;
          case SystemMessageWrapper.MessageTypes.SubjectChange:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.GROUP_CHANGE_SUBJECT);
            instance.MessageStubParameters = new List<string>()
            {
              m.GetSystemMessageNewSubject()
            };
            break;
          case SystemMessageWrapper.MessageTypes.GroupPhotoChange:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.GROUP_CHANGE_ICON);
            instance.MessageStubParameters = new List<string>()
            {
              m.GetSystemMessageIsGroupPhotoDeleted() ? "remove" : "changed"
            };
            break;
          case SystemMessageWrapper.MessageTypes.Rename:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.INDIVIDUAL_CHANGE_NUMBER);
            string messagePreviousJid = m.GetSystemMessagePreviousJid();
            instance.MessageStubParameters = new List<string>()
            {
              messagePreviousJid,
              m.RemoteResource
            };
            break;
          case SystemMessageWrapper.MessageTypes.BroadcastListCreated:
          case SystemMessageWrapper.MessageTypes.GroupCreated:
            if (flag)
            {
              instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.BROADCAST_CREATE);
              metrics = QrMetricsMapping.FORWARD_BC_UPDATE;
              instance.MessageStubParameters = new List<string>()
              {
                m.GetSystemMessageBroadcastListCount().ToString()
              };
              break;
            }
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.GROUP_CREATE);
            instance.MessageStubParameters = new List<string>()
            {
              m.GetSystemMessageNewSubject()
            };
            break;
          case SystemMessageWrapper.MessageTypes.GainedAdmin:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.GROUP_PARTICIPANT_PROMOTE);
            instance.MessageStubParameters = new List<string>()
            {
              m.GetSenderJid()
            };
            instance.Participant = m.RemoteResource;
            break;
          case SystemMessageWrapper.MessageTypes.IdentityChanged:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.E2E_IDENTITY_CHANGED);
            instance.MessageStubParameters = new List<string>()
            {
              m.GetSenderJid()
            };
            break;
          case SystemMessageWrapper.MessageTypes.ConversationEncrypted:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.E2E_ENCRYPTED);
            instance.MessageStubParameters = new List<string>();
            break;
          case SystemMessageWrapper.MessageTypes.MissedCall:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.CALL_MISSED_VOICE);
            instance.MessageTimestamp = new ulong?((ulong) SystemMessageUtils.ExtractMissedCallUnixTime(m.BinaryData));
            break;
          case SystemMessageWrapper.MessageTypes.MissedVideoCall:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.CALL_MISSED_VIDEO);
            instance.MessageTimestamp = new ulong?((ulong) SystemMessageUtils.ExtractMissedCallUnixTime(m.BinaryData));
            break;
          case SystemMessageWrapper.MessageTypes.GroupInviteChanged:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.GROUP_CHANGE_INVITE_LINK);
            instance.MessageStubParameters = new List<string>();
            break;
          case SystemMessageWrapper.MessageTypes.GroupDescriptionChanged:
          case SystemMessageWrapper.MessageTypes.GroupDescriptionDeleted:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.GROUP_CHANGE_DESCRIPTION);
            break;
          case SystemMessageWrapper.MessageTypes.VerifiedBizInitial:
            Pair<VerifiedLevel, string> verifiedState1 = (Pair<VerifiedLevel, string>) null;
            SystemMessageUtils.GetDataFromVerifiedBizInitialSysMsg(m, out verifiedState1);
            VerifiedLevel verifiedLevel = verifiedState1 != null ? verifiedState1.First : VerifiedLevel.unknown;
            string second1 = verifiedState1.Second;
            switch (verifiedLevel)
            {
              case VerifiedLevel.unknown:
                instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.VERIFIED_INITIAL_UNKNOWN);
                break;
              case VerifiedLevel.low:
                instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.VERIFIED_INITIAL_LOW);
                break;
              case VerifiedLevel.high:
                instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.VERIFIED_INITIAL_HIGH);
                break;
            }
            instance.MessageStubParameters = new List<string>()
            {
              SystemMessageUtils.ExtractVnameFromInitialBizSystemMessageData(m.BinaryData, m.KeyRemoteJid) ?? "?"
            };
            break;
          case SystemMessageWrapper.MessageTypes.VerifiedBizTransit:
            Pair<VerifiedLevel, string> verifiedState2 = (Pair<VerifiedLevel, string>) null;
            Pair<VerifiedLevel, string> prevVerifiedState = (Pair<VerifiedLevel, string>) null;
            SystemMessageUtils.GetDataFromVerifiedBizTransitSysMsg(m, out verifiedState2, out prevVerifiedState);
            VerifiedLevel first1 = verifiedState2.First;
            string second2 = verifiedState2.Second;
            VerifiedLevel first2 = prevVerifiedState.First;
            string second3 = prevVerifiedState.Second;
            switch (first1)
            {
              case VerifiedLevel.NotApplicable:
                instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.VERIFIED_TRANSITION_ANY_TO_NONE);
                break;
              case VerifiedLevel.unknown:
                switch (first2)
                {
                  case VerifiedLevel.NotApplicable:
                    instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.VERIFIED_TRANSITION_NONE_TO_UNKNOWN);
                    break;
                  case VerifiedLevel.low:
                    instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.VERIFIED_TRANSITION_LOW_TO_UNKNOWN);
                    break;
                  case VerifiedLevel.high:
                    instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.VERIFIED_TRANSITION_HIGH_TO_UNKNOWN);
                    break;
                }
                break;
              case VerifiedLevel.low:
                switch (first2)
                {
                  case VerifiedLevel.NotApplicable:
                    instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.VERIFIED_TRANSITION_NONE_TO_LOW);
                    break;
                  case VerifiedLevel.unknown:
                    instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.VERIFIED_TRANSITION_UNKNOWN_TO_LOW);
                    break;
                  case VerifiedLevel.high:
                    instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.VERIFIED_TRANSITION_HIGH_TO_LOW);
                    break;
                }
                break;
              case VerifiedLevel.high:
                instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.VERIFIED_TRANSITION_ANY_TO_HIGH);
                break;
            }
            instance.MessageStubParameters = new List<string>()
            {
              second2 ?? "?"
            };
            break;
          case SystemMessageWrapper.MessageTypes.GroupRestrictionLocked:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.GROUP_CHANGE_RESTRICT);
            instance.MessageStubParameters = new List<string>()
            {
              "on"
            };
            break;
          case SystemMessageWrapper.MessageTypes.GroupRestrictionUnlocked:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.GROUP_CHANGE_RESTRICT);
            instance.MessageStubParameters = new List<string>()
            {
              "off"
            };
            break;
          case SystemMessageWrapper.MessageTypes.GroupMadeAnnouncementOnly:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.GROUP_CHANGE_ANNOUNCE);
            instance.MessageStubParameters = new List<string>()
            {
              "on"
            };
            break;
          case SystemMessageWrapper.MessageTypes.GroupMadeNotAnnouncementOnly:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.GROUP_CHANGE_ANNOUNCE);
            instance.MessageStubParameters = new List<string>()
            {
              "off"
            };
            break;
          case SystemMessageWrapper.MessageTypes.LostAdmin:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.GROUP_PARTICIPANT_DEMOTE);
            instance.MessageStubParameters = new List<string>()
            {
              m.GetSenderJid()
            };
            instance.Participant = m.RemoteResource;
            break;
          case SystemMessageWrapper.MessageTypes.GroupParticipantNumberChanged:
            instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.GROUP_PARTICIPANT_CHANGE_NUMBER);
            instance.MessageStubParameters = new List<string>()
            {
              m.GetSenderJid()
            };
            break;
          case SystemMessageWrapper.MessageTypes.VerifiedBizInitial2Tier:
            bool inPhoneBoook = false;
            bool nameMatchesPhonebook1 = false;
            Triad<VerifiedLevel, string, VerifiedTier> initialSysMsg2Tier = SystemMessageUtils.GetDataFromVerifiedBizInitialSysMsg2Tier(m, out inPhoneBoook, out nameMatchesPhonebook1);
            switch (initialSysMsg2Tier.Third)
            {
              case VerifiedTier.Bottom:
                instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.BIZ_INTRO_BOTTOM);
                break;
              case VerifiedTier.Top:
                instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.BIZ_INTRO_TOP);
                break;
            }
            instance.MessageStubParameters = new List<string>()
            {
              initialSysMsg2Tier.Second ?? "?"
            };
            break;
          case SystemMessageWrapper.MessageTypes.VerifiedBizTransit2Tier:
            bool inPhonebook = false;
            bool nameMatchesPhonebook2 = false;
            Triad<SystemMessageUtils.TransistionToDisplay2Tier, string, string> triad = SystemMessageUtils.DetectChange(m, out inPhonebook, out nameMatchesPhonebook2);
            SystemMessageUtils.TransistionToDisplay2Tier first3 = triad.First;
            string second4 = triad.Second;
            switch (first3)
            {
              case SystemMessageUtils.TransistionToDisplay2Tier.TopToBottom:
                instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.BIZ_VERIFIED_TRANSITION_TOP_TO_BOTTOM);
                break;
              case SystemMessageUtils.TransistionToDisplay2Tier.TopToConsumer:
              case SystemMessageUtils.TransistionToDisplay2Tier.BottomToConsumer:
                instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.BIZ_MOVE_TO_CONSUMER_APP);
                break;
              case SystemMessageUtils.TransistionToDisplay2Tier.BottomToTop:
                instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.BIZ_VERIFIED_TRANSITION_BOTTOM_TO_TOP);
                break;
              case SystemMessageUtils.TransistionToDisplay2Tier.ConsumerToTop:
                instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.BIZ_TWO_TIER_MIGRATION_TOP);
                break;
              case SystemMessageUtils.TransistionToDisplay2Tier.ConsumerToBottom:
                instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.BIZ_TWO_TIER_MIGRATION_BOTTOM);
                break;
              case SystemMessageUtils.TransistionToDisplay2Tier.NameChange:
                instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.BIZ_NAME_CHANGE);
                break;
            }
            instance.MessageStubParameters = new List<string>()
            {
              second4 ?? "?"
            };
            break;
          case SystemMessageWrapper.MessageTypes.VerifiedBizOneTime2Tier:
            Triad<VerifiedLevel, string, VerifiedTier> oneTimeSysMsgTier2 = SystemMessageUtils.GetDataFromVerifiedBizOneTimeSysMsgTier2(m);
            if (oneTimeSysMsgTier2 != null)
            {
              VerifiedTier third = oneTimeSysMsgTier2.Third;
              string second5 = oneTimeSysMsgTier2.Second;
              switch (third)
              {
                case VerifiedTier.Bottom:
                  instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.BIZ_INTRO_BOTTOM);
                  break;
                case VerifiedTier.Top:
                  instance.MessageStubType = new WebMessageInfo.StubType?(WebMessageInfo.StubType.BIZ_INTRO_TOP);
                  break;
              }
              instance.MessageStubParameters = new List<string>()
              {
                oneTimeSysMsgTier2.Second ?? "?"
              };
              break;
            }
            break;
          default:
            return (FunXMPP.ProtocolTreeNode) null;
        }
        if (instance.MessageStubType.HasValue)
          instance.Message = (WhatsApp.ProtoBuf.Message) null;
        return new FunXMPP.ProtocolTreeNode("message", (FunXMPP.KeyValue[]) null, WebMessageInfo.SerializeToBytes(instance));
      }

      private static FunXMPP.ProtocolTreeNode ConvertSystemMessageLegacy(
        Message m,
        ref QrMetricsMapping metrics,
        bool invis = false)
      {
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>();
        keyValueList.Add(new FunXMPP.KeyValue("id", m.KeyId));
        keyValueList.Add(new FunXMPP.KeyValue("jid", m.KeyRemoteJid));
        byte[] bytes = Encoding.UTF8.GetBytes(m.GetSystemMessage());
        string senderJid = m.GetSenderJid();
        if (senderJid != null)
          keyValueList.Add(new FunXMPP.KeyValue("author", senderJid));
        DateTime? localTimestamp = m.LocalTimestamp;
        if (localTimestamp.HasValue)
        {
          long unixTime = localTimestamp.Value.ToUnixTime();
          keyValueList.Add(new FunXMPP.KeyValue("t", unixTime.ToString()));
        }
        return new FunXMPP.ProtocolTreeNode("notification", keyValueList.ToArray(), bytes);
      }

      private FunXMPP.ProtocolTreeNode CreateContactNode(
        FunXMPP.ContactResponse c,
        bool includeChecksum)
      {
        List<FunXMPP.KeyValue> keyValueList = new List<FunXMPP.KeyValue>()
        {
          new FunXMPP.KeyValue("jid", c.Jid)
        };
        if (!string.IsNullOrEmpty(c.DisplayName))
          keyValueList.Add(new FunXMPP.KeyValue("name", c.DisplayName));
        if (!string.IsNullOrEmpty(c.ShortName))
          keyValueList.Add(new FunXMPP.KeyValue("short", c.ShortName));
        if (includeChecksum)
          keyValueList.Add(new FunXMPP.KeyValue("checksum", c.Checksum.ToString()));
        if (!string.IsNullOrEmpty(c.VName))
          keyValueList.Add(new FunXMPP.KeyValue("vname", c.VName));
        if (c.Notify != null)
          keyValueList.Add(new FunXMPP.KeyValue("notify", c.Notify));
        if (c.Verify.HasValue)
        {
          keyValueList.Add(new FunXMPP.KeyValue("verify", c.Verify.Value.ToString()));
          if (c.Checkmark.HasValue && !c.Checkmark.Value)
            keyValueList.Add(new FunXMPP.KeyValue("checkmark", "false"));
        }
        if (c.IsEnterprise.HasValue && c.IsEnterprise.Value)
          keyValueList.Add(new FunXMPP.KeyValue("enterprise", c.IsEnterprise.Value.ToString()));
        if (c.StatusMute.HasValue)
          keyValueList.Add(new FunXMPP.KeyValue("status_mute", c.StatusMute.Value ? "true" : "false"));
        return new FunXMPP.ProtocolTreeNode("user", keyValueList.ToArray());
      }

      private List<FunXMPP.ProtocolTreeNode> CreateParticipantNodesWithLocation(string jid)
      {
        LiveLocationManager instance = LiveLocationManager.Instance;
        List<FunXMPP.ProtocolTreeNode> nodesWithLocation = new List<FunXMPP.ProtocolTreeNode>();
        System.Collections.Generic.Dictionary<string, int?> jidsSharingForGroup = instance.GetJidsSharingForGroup(jid);
        foreach (string key in jidsSharingForGroup.Keys)
        {
          LocationData locationDataForJid = instance.GetLocationDataForJid(key);
          WhatsApp.ProtoBuf.Message liveLocationMessage = instance.CreateLiveLocationMessage(locationDataForJid);
          int num = (int) FunRunner.CurrentServerTimeUtc.Subtract(locationDataForJid.Timestamp).TotalSeconds;
          string v1 = num.ToString();
          int? nullable = jidsSharingForGroup[key];
          FunXMPP.KeyValue[] attrs = new FunXMPP.KeyValue[3]
          {
            new FunXMPP.KeyValue(nameof (jid), key),
            null,
            null
          };
          string v2;
          if (!nullable.HasValue)
          {
            v2 = (string) null;
          }
          else
          {
            num = nullable.GetValueOrDefault();
            v2 = num.ToString();
          }
          attrs[1] = new FunXMPP.KeyValue("expiration", v2);
          attrs[2] = new FunXMPP.KeyValue("elapsed", v1);
          FunXMPP.ProtocolTreeNode protocolTreeNode = new FunXMPP.ProtocolTreeNode("participant", attrs, liveLocationMessage.ToPlainText(false));
          nodesWithLocation.Add(protocolTreeNode);
        }
        return nodesWithLocation;
      }

      private List<FunXMPP.ProtocolTreeNode> CreateParticipantNodesWithoutLocation(string jid)
      {
        List<FunXMPP.ProtocolTreeNode> nodesWithoutLocation = new List<FunXMPP.ProtocolTreeNode>();
        System.Collections.Generic.Dictionary<string, int?> jidsSharingForGroup = LiveLocationManager.Instance.GetJidsSharingForGroup(jid);
        foreach (string key in jidsSharingForGroup.Keys)
        {
          int? nullable = jidsSharingForGroup[key];
          FunXMPP.ProtocolTreeNode protocolTreeNode = new FunXMPP.ProtocolTreeNode("participant", new FunXMPP.KeyValue[2]
          {
            new FunXMPP.KeyValue(nameof (jid), key),
            new FunXMPP.KeyValue("expiration", nullable?.ToString())
          });
          nodesWithoutLocation.Add(protocolTreeNode);
        }
        return nodesWithoutLocation;
      }

      public enum AccountKind
      {
        Unknown = -1, // 0xFFFFFFFF
        Free = 0,
      }

      public class Ack
      {
        public string To;
        public string Participant;
        public string Id;
        public string EditVersion;

        public override string ToString()
        {
          return this.To + "\n" + this.Participant + "\n" + this.Id + "\n" + this.EditVersion;
        }

        public static FunXMPP.Connection.Ack FromString(string data)
        {
          string[] strArray1 = data.Split('\n');
          if (strArray1.Length == 3)
          {
            string[] strArray2 = new string[4];
            strArray1.CopyTo((Array) strArray2, 0);
            strArray1 = strArray2;
            strArray1[3] = "";
          }
          return strArray1.Length == 4 ? new FunXMPP.Connection.Ack()
          {
            To = strArray1[0],
            Participant = strArray1[1],
            Id = strArray1[2],
            EditVersion = strArray1[3]
          } : throw new ArgumentException("Does not split into four fields", nameof (data));
        }

        public override bool Equals(object obj)
        {
          return obj is FunXMPP.Connection.Ack ack && this.To.Equals(ack.To) && this.Participant.Equals(ack.Participant) && this.Id.Equals(ack.Id) && this.EditVersion.Equals(ack.EditVersion);
        }
      }

      public class BusinessRequest
      {
        public string Jid;
        public UserStatus UserStatus;
      }

      public class StatusRequest
      {
        public string Jid;
        public DateTime? LastUpdated;
      }

      public enum SyncMode
      {
        Full,
        Delta,
        Query,
        Chunked,
      }

      public enum SyncContext
      {
        Interactive,
        Background,
        Registration,
        Notification,
      }

      public class SyncResult
      {
        public FunXMPP.Connection.SyncResult.User[] SwellFolks;
        public FunXMPP.Connection.SyncResult.User[] Holdouts;
        public string[] NormalizationErrors;
        public DateTime? NextFullSyncUtc;

        public class User
        {
          public string OriginalNumber;
          public string Jid;
        }
      }

      private class PendingReceiptState
      {
        public Action AckCallback;
        public Pair<string, string>[] AdditionalIds;
      }

      public struct GroupSetting
      {
        public string Jid;
        public bool Enabled;
        public DateTime? MuteExpiry;
      }

      public class PaymentsTransactionUpdate
      {
        public string TranId;
        public long TranTs;
        public string TranStatus;
        public string TranType;
        public long TranAmountx1000;
        public string TranCurrency;
        public string GroupJid;
        public string SenderJid;
        public string ReceiverJid;
        public string MessageId;
        public string CredentialId;
        public long? Balancex1000;
        public bool DefPayment;
        public bool DefPayout;

        private PaymentsTransactionUpdate(FunXMPP.ProtocolTreeNode notifyNode)
        {
          try
          {
            FunXMPP.ProtocolTreeNode child1 = notifyNode.GetChild("transaction");
            if (child1 != null)
            {
              long? nullable = PaymentsHelper.ConvertFBStringToLong(child1.GetAttributeValue("amount"));
              if (nullable.HasValue)
              {
                this.TranAmountx1000 = nullable.Value;
              }
              else
              {
                Log.l(nameof (PaymentsTransactionUpdate), "transaction amount not present defaulted to 0");
                this.TranAmountx1000 = 0L;
              }
              this.TranCurrency = child1.GetAttributeValue("currency");
              this.GroupJid = child1.GetAttributeValue("group");
              this.TranId = child1.GetAttributeValue("id");
              this.MessageId = child1.GetAttributeValue("message-id");
              this.ReceiverJid = child1.GetAttributeValue("receiver");
              this.SenderJid = child1.GetAttributeValue("sender");
              this.TranStatus = child1.GetAttributeValue("status");
              this.TranTs = Convert.ToInt64(child1.GetAttributeValue("ts"));
              this.TranType = child1.GetAttributeValue("type");
              if (this.TranType == "cashin" || this.TranType == "cashout")
              {
                FunXMPP.ProtocolTreeNode child2 = notifyNode.GetChild("wallet");
                if (child2 == null)
                  return;
                this.CredentialId = child2.GetAttributeValue("credential-id");
                this.Balancex1000 = PaymentsHelper.ConvertFBStringToLong(child2.GetAttributeValue("balance"));
                int num1;
                switch (child2.GetAttributeValue("def-payment"))
                {
                  case "1":
                    num1 = 1;
                    break;
                  default:
                    num1 = 0;
                    break;
                }
                this.DefPayment = num1 != 0;
                int num2;
                switch (child2.GetAttributeValue("def-payout"))
                {
                  case "1":
                    num2 = 1;
                    break;
                  default:
                    num2 = 0;
                    break;
                }
                this.DefPayout = num2 != 0;
              }
              else
              {
                if (string.IsNullOrEmpty(this.TranType))
                  return;
                Log.l("Payments", "unexpected transaction type found {0}", (object) this.TranType);
                this.TranTs = -1L;
              }
            }
            else
            {
              Log.l("Payments", "No transaction node found");
              this.TranTs = -1L;
            }
          }
          catch (Exception ex)
          {
            Log.l(ex, "Payments exception procesing transaction notification");
            this.TranTs = -1L;
          }
        }

        public static FunXMPP.Connection.PaymentsTransactionUpdate ExtractTransactionUpdate(
          FunXMPP.ProtocolTreeNode notifyNode)
        {
          FunXMPP.Connection.PaymentsTransactionUpdate transactionUpdate = new FunXMPP.Connection.PaymentsTransactionUpdate(notifyNode);
          return transactionUpdate.TranTs != -1L && transactionUpdate.TranStatus != null ? transactionUpdate : (FunXMPP.Connection.PaymentsTransactionUpdate) null;
        }
      }

      public class PaymentsTransactionResponse
      {
        public string TranId;
        public long Ts;
        public string Status;
        public string TranType;
        public long TranAmountx1000;
        public string TranCurrency;
        public FunXMPP.Connection.PaymentsTransactionResponseSource[] Sources;
        public FunXMPP.Connection.PaymentsTransactionResponseDest[] Dests;
        public string SenderJid;
        public string ReceiverJid;
        public string MessageId;

        public PaymentsTransactionResponse(FunXMPP.ProtocolTreeNode transactionNode)
        {
          this.TranId = transactionNode.GetAttributeValue("id");
          this.TranType = transactionNode.GetAttributeValue("type");
          this.Ts = Convert.ToInt64(transactionNode.GetAttributeValue("ts"));
          this.Status = transactionNode.GetAttributeValue("status");
          long? nullable = PaymentsHelper.ConvertFBStringToLong(transactionNode.GetAttributeValue("amount"));
          if (nullable.HasValue)
          {
            this.TranAmountx1000 = nullable.Value;
          }
          else
          {
            Log.l(nameof (PaymentsTransactionResponse), "No amount found - defaulting to 0");
            this.TranAmountx1000 = 0L;
          }
          this.TranCurrency = transactionNode.GetAttributeValue("currency");
          IEnumerable<FunXMPP.ProtocolTreeNode> allChildren = transactionNode.GetAllChildren("source");
          if (allChildren != null && allChildren.Count<FunXMPP.ProtocolTreeNode>() > 0)
          {
            this.Sources = new FunXMPP.Connection.PaymentsTransactionResponseSource[allChildren.Count<FunXMPP.ProtocolTreeNode>()];
            int index = 0;
            foreach (FunXMPP.ProtocolTreeNode sourceNode in allChildren)
            {
              this.Sources[index] = new FunXMPP.Connection.PaymentsTransactionResponseSource(sourceNode);
              ++index;
            }
          }
          this.SenderJid = transactionNode.GetAttributeValue("sender");
          this.ReceiverJid = transactionNode.GetAttributeValue("receiver");
          this.MessageId = transactionNode.GetAttributeValue("message-id");
          FunXMPP.ProtocolTreeNode child = transactionNode.GetChild("dest");
          if (child == null)
            return;
          this.Dests = new FunXMPP.Connection.PaymentsTransactionResponseDest[1]
          {
            new FunXMPP.Connection.PaymentsTransactionResponseDest(child)
          };
        }
      }

      public class PaymentsTransactionResponseSource
      {
        public string CredentialId;
        public string Amount;

        public PaymentsTransactionResponseSource(FunXMPP.ProtocolTreeNode sourceNode)
        {
          this.CredentialId = sourceNode.GetAttributeValue("credential-id");
          this.Amount = sourceNode.GetAttributeValue("amount");
        }
      }

      public class PaymentsTransactionResponseDest
      {
        public string CredentialId;
        public string Amount;

        public PaymentsTransactionResponseDest(FunXMPP.ProtocolTreeNode destNode)
        {
          this.CredentialId = destNode.GetAttributeValue("credential-id");
          this.Amount = destNode.GetAttributeValue("amount");
        }
      }

      public class UploadResult : MediaUploadMms4.Mms4UploadResult
      {
        public string MimeType;
        public long FileSize;
        public byte[] Hash;
        public int? DurationSeconds;
      }

      public class UnforwardableMessageException : Exception
      {
      }

      public enum ContactNotificationType
      {
        Add,
        Remove,
        Update,
        Modify,
        Sync,
      }

      public class GroupInfo
      {
        public string Jid;
        public string CreatorJid;
        public DateTime? CreationTime;
        public string Subject;
        public string SubjectOwnerJid;
        public DateTime? SubjectTime;
        public List<string> AdminJids;
        public List<string> NonadminJids;
        public GroupDescription Description;
        public bool Locked;
        public bool AnnouncementOnly;
        public string SuperAdmin;
      }

      public class GroupCreationEventArgs
      {
        public FunXMPP.Connection.GroupInfo Info;
        public DateTime? Timestamp;
        public string InviterJid;
        public string CreationKey;
        public bool IsNew;
      }

      private sealed class NotificationAckHandler
      {
        private Action onAck;
        private Action onNoAck;
        public string logContextData;

        public NotificationAckHandler(Action onAck, Action onNoAck, string contextData)
        {
          this.onAck = onAck;
          this.onNoAck = onNoAck;
          this.logContextData = contextData;
        }

        public bool OnAck()
        {
          try
          {
            Action onAck = this.onAck;
            if (onAck != null)
              onAck();
            return true;
          }
          catch (Exception ex)
          {
            string context = "AckNotificationHandler OnAck Exception processing " + this.logContextData;
            Log.SendCrashLog(ex, context, logOnlyForRelease: true);
            return false;
          }
        }

        public bool OnNoAck()
        {
          try
          {
            Action onNoAck = this.onNoAck;
            if (onNoAck != null)
              onNoAck();
            return true;
          }
          catch (Exception ex)
          {
            string context = "AckNotificationHandler OnNoAck Exception processing " + this.logContextData;
            Log.SendCrashLog(ex, context, logOnlyForRelease: true);
            return false;
          }
        }
      }

      public class QrChatActions
      {
        public IEnumerable<FunXMPP.ProtocolTreeNode> ResponseNodes;
        public int AcksFromMessages;
        public Action<Action> PostResponse;
      }
    }

    public sealed class ProtocolTreeNode
    {
      public string tag;
      public FunXMPP.KeyValue[] attributes;
      public FunXMPP.ProtocolTreeNode[] children;
      public byte[] data;

      public ProtocolTreeNode(
        string tag,
        FunXMPP.KeyValue[] attrs,
        FunXMPP.ProtocolTreeNode[] children)
      {
        this.Init(tag, attrs, children);
      }

      public ProtocolTreeNode(string tag, FunXMPP.KeyValue[] attrs, FunXMPP.ProtocolTreeNode child)
      {
        string tagName = tag;
        FunXMPP.KeyValue[] attrs1 = attrs;
        FunXMPP.ProtocolTreeNode[] childNodes;
        if (child != null)
          childNodes = new FunXMPP.ProtocolTreeNode[1]
          {
            child
          };
        else
          childNodes = (FunXMPP.ProtocolTreeNode[]) null;
        this.Init(tagName, attrs1, childNodes);
      }

      public ProtocolTreeNode(string tag, FunXMPP.KeyValue[] attrs, byte[] data)
      {
        this.Init(tag, attrs, dataBytes: data);
      }

      public ProtocolTreeNode(string tag, FunXMPP.KeyValue[] attrs, string data)
      {
        this.Init(tag, attrs, dataBytes: data == null ? (byte[]) null : Encoding.UTF8.GetBytes(data.ConvertLineEndings()));
      }

      public ProtocolTreeNode(string tag, FunXMPP.KeyValue[] attrs) => this.Init(tag, attrs);

      private void Init(
        string tagName,
        FunXMPP.KeyValue[] attrs = null,
        FunXMPP.ProtocolTreeNode[] childNodes = null,
        byte[] dataBytes = null)
      {
        this.tag = tagName;
        this.attributes = attrs ?? new FunXMPP.KeyValue[0];
        this.children = childNodes;
        this.data = dataBytes;
      }

      public string GetDataString()
      {
        return this.data != null ? Encoding.UTF8.GetString(this.data, 0, this.data.Length) : (string) null;
      }

      public void AddAttribute(string attrKey, string attrVal)
      {
        if (attrKey == null || attrVal == null)
          return;
        FunXMPP.KeyValue[] keyValueArray = this.attributes ?? new FunXMPP.KeyValue[0];
        if (((IEnumerable<FunXMPP.KeyValue>) keyValueArray).Any<FunXMPP.KeyValue>((Func<FunXMPP.KeyValue, bool>) (p => p.key == attrKey)))
          return;
        this.attributes = ((IEnumerable<FunXMPP.KeyValue>) keyValueArray).Concat<FunXMPP.KeyValue>((IEnumerable<FunXMPP.KeyValue>) new FunXMPP.KeyValue[1]
        {
          new FunXMPP.KeyValue(attrKey, attrVal)
        }).ToArray<FunXMPP.KeyValue>();
      }

      public string GetAttributeValue(string attrKey)
      {
        if (this.attributes == null)
          return (string) null;
        for (int index = 0; index < this.attributes.Length; ++index)
        {
          FunXMPP.KeyValue attribute = this.attributes[index];
          if (attrKey.Equals(attribute.key))
            return attribute.value;
        }
        return (string) null;
      }

      public DateTime? GetAttributeDateTime(string str)
      {
        DateTime? dt = new DateTime?();
        FunXMPP.TryParseTimestamp(this.GetAttributeValue(str), out dt);
        return dt;
      }

      public int? GetAttributeInt(string str)
      {
        int? attributeInt = new int?();
        string attributeValue = this.GetAttributeValue(str);
        if (attributeValue != null)
        {
          int result = 0;
          if (int.TryParse(attributeValue, out result))
            attributeInt = new int?(result);
        }
        return attributeInt;
      }

      public long? GetAttributeLong(string str)
      {
        long? attributeLong = new long?();
        string attributeValue = this.GetAttributeValue(str);
        if (attributeValue != null)
        {
          long result = 0;
          if (long.TryParse(attributeValue, out result))
            attributeLong = new long?(result);
        }
        return attributeLong;
      }

      public System.Collections.Generic.Dictionary<string, string> GetAttributes(
        string[] expectedKeys = null)
      {
        System.Collections.Generic.Dictionary<string, string> attributes = new System.Collections.Generic.Dictionary<string, string>();
        if (this.attributes != null)
        {
          if (expectedKeys != null)
          {
            foreach (string expectedKey in expectedKeys)
              attributes[expectedKey] = (string) null;
          }
          for (int index = 0; index < this.attributes.Length; ++index)
          {
            FunXMPP.KeyValue attribute = this.attributes[index];
            attributes[attribute.key] = attribute.value;
          }
        }
        return attributes;
      }

      public FunXMPP.ProtocolTreeNode GetChild(string tag)
      {
        FunXMPP.ProtocolTreeNode child1 = (FunXMPP.ProtocolTreeNode) null;
        if (this.children != null)
        {
          foreach (FunXMPP.ProtocolTreeNode child2 in this.children)
          {
            if (tag.Equals(child2.tag))
              child1 = child2;
          }
        }
        return child1;
      }

      public static FunXMPP.ProtocolTreeNode SafeGetChild(FunXMPP.ProtocolTreeNode node, int i)
      {
        if (node == null || node.children == null || node.children.Length <= i)
          throw new FunXMPP.CorruptStreamException("safeGetChild sees null node/child");
        return node.children[i];
      }

      public IEnumerable<FunXMPP.ProtocolTreeNode> GetAllChildren(string tagName)
      {
        return ((IEnumerable<FunXMPP.ProtocolTreeNode>) (this.children ?? new FunXMPP.ProtocolTreeNode[0])).Where<FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, bool>) (child => child != null && FunXMPP.ProtocolTreeNode.TagEquals(child, tagName)));
      }

      public FunXMPP.ProtocolTreeNode GetChild(int i)
      {
        return this.children == null || this.children.Length <= i ? (FunXMPP.ProtocolTreeNode) null : this.children[i];
      }

      public static FunXMPP.ProtocolTreeNode Require(FunXMPP.ProtocolTreeNode node, string _string)
      {
        return FunXMPP.ProtocolTreeNode.TagEquals(node, _string) ? node : throw new FunXMPP.CorruptStreamException("failed require. node: " + (object) node + " string: " + _string);
      }

      public static bool TagEquals(FunXMPP.ProtocolTreeNode node, string _string)
      {
        return node != null && node.tag != null && node.tag.Equals(_string);
      }
    }

    public sealed class KeyValue
    {
      public string key;
      public string value;

      public KeyValue(string k, string v)
      {
        if (v == null || k == null)
        {
          string message = "KeyValue argument was null.";
          if (k != null)
            message += string.Format(" key = [{0}]", (object) k);
          if (v != null)
            message += string.Format(" value = [{0}]", (object) v);
          throw new NullReferenceException(message);
        }
        this.key = k;
        this.value = v;
      }
    }

    public class StreamEndException : Exception
    {
      public StreamEndException(string msg)
        : base(msg)
      {
      }
    }

    public interface QrListener
    {
      QrSession Session { get; }

      bool Active { get; }

      FunXMPP.SyncResponse GetSync();

      Message[] GetMessagesAfter(
        FunXMPP.FMessage.Key webMsgKey,
        bool asc,
        int? limit,
        bool includeBound,
        bool mediaOnly = false,
        FunXMPP.FMessage.Type? mediaType = null);

      Message[] GetMessagesBefore(
        string jid,
        bool fromMe,
        string keyId,
        int? count,
        bool asc,
        bool mediaOnly = false,
        FunXMPP.FMessage.Type? mediaType = null);

      Message[] GetStarredMessagesBefore(
        string chat,
        string jid,
        bool fromMe,
        string keyId,
        int? count,
        bool asc,
        FunXMPP.FMessage.Type[] types);

      FunXMPP.ChatResponse[] GetChats(bool getUnreadMessages, string jid);

      FunXMPP.ContactResponse[] GetContacts();

      string GetProfilePhotoId(string jid);

      void OnProfilePhotoRequest(string id, string jid, bool large);

      FunXMPP.PhotoResponse GetProfilePhoto(string jid, bool large);

      FunXMPP.ResumeResponse GetResumeState(System.Collections.Generic.Dictionary<string, FunXMPP.ChatResponse> webChats);

      FunXMPP.ReceiptResponse GetReceiptStates(FunXMPP.FMessage.Key key, DateTime tsUtc);

      FunXMPP.MessageInfoStateResponse GetMessageInfoState(FunXMPP.FMessage.Key key);

      FunXMPP.Connection.GroupInfo GetGroupMetadata(string jid);

      FunXMPP.ActionResponse GetActions(string[] ids);

      IEnumerable<Pair<string, double>> GetEmojis(IEnumerable<Pair<string, double>> webEmojis);

      void OnQrOnline(bool? timeout);

      void OnQrError(int code);

      bool OnRelay(FunXMPP.FMessage msg);

      void OnReUpload(FunXMPP.FMessage.Key key, string id);

      int OnMarkAsRead(string jid, string keyId, bool fromMe, string participant, int? count);

      int OnMarkAsSeen(string jid, string keyId, bool fromMe, string senderJid);

      void OnLogout(byte[] browserId, byte[] code);

      void OnDisconnect(byte[] sessionData, byte[] browserid, DateTime? timestamp);

      FunXMPP.VerifySessionResponse VerifyCurrent(
        FunXMPP.VerifySessionType type,
        byte[] sessionData,
        byte[] browserId,
        byte[] clientToken,
        List<Action> postResponse,
        string os = null,
        string browser = null);

      FunXMPP.VerifySessionResponse VerifyQuerySync(
        FunXMPP.VerifySessionType type,
        byte[] sessionData);

      void OnAvailable(bool on, DateTime? presenceTimestamp);

      void OnComposing(string jid, string gjid, Presence type);

      int OnReceipt(string type, string messageId, string remoteJid, string participant);

      void OnSetStatus(string status, string incomingId);

      int OnSetChat(FunXMPP.ChatResponse chat);

      void OnChangeNumberNotificationDismiss(string jid);

      void OnDeleteMessages(FunXMPP.FMessage.Key[] keys);

      void OnStarMessages(bool star, FunXMPP.FMessage.Key[] keys);

      bool ShouldAddWebRelay(string keyId);

      void OnUnsubscribeLocation(string jid, string id);

      int OnDisableLocation(string jid, string id);
    }

    public enum VerifySessionType
    {
      Normal,
      Resume,
      Challenge,
      Query,
      Takeover,
      ForcedResume,
    }

    public class VerifySessionResponse
    {
      public FunXMPP.VerifySessionResponse.ResponseType Response;
      public byte[] ClientToken;
      public byte[] ChallengeData;

      public enum ResponseType
      {
        Accept,
        Conflict,
        Challenge,
        Failure,
      }
    }

    public class SyncResponse
    {
      public int BatteryPercentage;
      public bool PowerSourceConnected;
      public bool BatterySaverEnabled;
      public string Language;
      public string Locale;
      public bool IsMilitaryTime;
    }

    public class ChatResponse
    {
      public string Jid;
      public string DisplayName;
      public bool Archived;
      public bool ReadOnly;
      public bool Spam;
      public DateTime? Timestamp;
      public DateTime? MuteExpiration;
      public DateTime? PinTimestamp;
      public int ModifyTag;
      public int Count;
      public List<Message> Messages;
      public bool Active;
      public FunXMPP.FMessage.Key UnreadMessageKey;
      public FunXMPP.FMessage.Key LastMessageKey;
      public FunXMPP.ChatResponseAction? Type;
      public FunXMPP.ChatSetAction? SetType;
      public string OldJid;
      public string NewJid;
    }

    public enum ChatResponseAction
    {
      Clear,
      Delete,
      Ahead,
      Resend,
    }

    public enum ChatSetAction
    {
      Clear,
      Delete,
      Archive,
      Mute,
      Unmute,
      Unstar,
      NotSpam,
      Pin,
      Unpin,
    }

    public class ContactResponse
    {
      public string Jid;
      public string DisplayName;
      public string ShortName;
      public string VName;
      public int Checksum;
      public string Notify;
      public bool? IsEnterprise;
      public bool? StatusMute;
      public int? Verify;
      public bool? Checkmark;
    }

    public class ResumeResponse
    {
      public List<FunXMPP.ChatResponse> ExistingChats;
      public List<FunXMPP.ChatResponse> NewChats;
      public int Checksum;
    }

    public class ActionResponse
    {
      public bool Replaced;
      public System.Collections.Generic.Dictionary<string, int> Actions;
    }

    public class ReceiptStateResponse
    {
      public string KeyId;
      public bool KeyFromMe;
      public FunXMPP.FMessage.Status Status;

      public ReceiptStateResponse(string id, bool keyFromMe, FunXMPP.FMessage.Status status)
      {
        this.KeyId = id;
        this.KeyFromMe = keyFromMe;
        this.Status = status;
      }
    }

    public class ReceiptResponse
    {
      public DateTime? Timestamp;
      public FunXMPP.ReceiptStateResponse[] Receipts;
    }

    public class MessageInfoResponse
    {
      public string Jid;
      public DateTime Timestamp;

      public MessageInfoResponse(string jid, DateTime timestamp)
      {
        this.Jid = jid;
        this.Timestamp = timestamp;
      }
    }

    public class MessageInfoStateResponse
    {
      public int Count;
      public IEnumerable<FunXMPP.MessageInfoResponse> Played;
      public IEnumerable<FunXMPP.MessageInfoResponse> Read;
      public IEnumerable<FunXMPP.MessageInfoResponse> Delivered;
    }

    public class PhotoResponse : IDisposable
    {
      public string Id;
      public Stream Stream;

      public void Dispose()
      {
        Stream stream = this.Stream;
        this.Stream = (Stream) null;
        stream.SafeDispose();
      }
    }

    public class MessagesResponse
    {
      public FunXMPP.FMessage.Key UnreadMessageKey;
      public List<Message> RecentMessages;
    }

    public enum ChatStatusForwardAction
    {
      Mute,
      Archive,
      Unarchive,
      Delete,
      Clear,
      DeleteAll,
      ClearAll,
      ModifyTag,
      ClearNotStarred,
      NotSpam,
      Pin,
    }
  }
}
