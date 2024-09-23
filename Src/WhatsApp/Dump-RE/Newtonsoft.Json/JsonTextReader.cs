// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonTextReader
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Utilities;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Represents a reader that provides fast, non-cached, forward-only access to JSON text data.
  /// </summary>
  public class JsonTextReader : JsonReader, IJsonLineInfo
  {
    private const char UnicodeReplacementChar = '�';
    private const int MaximumJavascriptIntegerCharacterLength = 380;
    private readonly TextReader _reader;
    private char[] _chars;
    private int _charsUsed;
    private int _charPos;
    private int _lineStartPos;
    private int _lineNumber;
    private bool _isEndOfFile;
    private StringBuffer _buffer;
    private StringReference _stringReference;
    internal PropertyNameTable NameTable;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonReader" /> class with the specified <see cref="T:System.IO.TextReader" />.
    /// </summary>
    /// <param name="reader">The <c>TextReader</c> containing the XML data to read.</param>
    public JsonTextReader(TextReader reader)
    {
      this._reader = reader != null ? reader : throw new ArgumentNullException(nameof (reader));
      this._lineNumber = 1;
      this._chars = new char[1025];
    }

    private StringBuffer GetBuffer()
    {
      if (this._buffer == null)
        this._buffer = new StringBuffer(1025);
      else
        this._buffer.Position = 0;
      return this._buffer;
    }

    private void OnNewLine(int pos)
    {
      ++this._lineNumber;
      this._lineStartPos = pos - 1;
    }

    private void ParseString(char quote)
    {
      ++this._charPos;
      this.ShiftBufferIfNeeded();
      this.ReadStringIntoBuffer(quote);
      this.SetPostValueState(true);
      if (this._readType == ReadType.ReadAsBytes)
      {
        Guid g;
        this.SetToken(JsonToken.Bytes, this._stringReference.Length != 0 ? (this._stringReference.Length != 36 || !ConvertUtils.TryConvertGuid(this._stringReference.ToString(), out g) ? (object) Convert.FromBase64CharArray(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length) : (object) g.ToByteArray()) : (object) new byte[0], false);
      }
      else if (this._readType == ReadType.ReadAsString)
      {
        this.SetToken(JsonToken.String, (object) this._stringReference.ToString(), false);
        this._quoteChar = quote;
      }
      else
      {
        string s = this._stringReference.ToString();
        if (this._dateParseHandling != DateParseHandling.None)
        {
          DateParseHandling dateParseHandling = this._readType != ReadType.ReadAsDateTime ? (this._readType != ReadType.ReadAsDateTimeOffset ? this._dateParseHandling : DateParseHandling.DateTimeOffset) : DateParseHandling.DateTime;
          object dt;
          if (DateTimeUtils.TryParseDateTime(s, dateParseHandling, this.DateTimeZoneHandling, this.DateFormatString, this.Culture, out dt))
          {
            this.SetToken(JsonToken.Date, dt, false);
            return;
          }
        }
        this.SetToken(JsonToken.String, (object) s, false);
        this._quoteChar = quote;
      }
    }

    private static void BlockCopyChars(
      char[] src,
      int srcOffset,
      char[] dst,
      int dstOffset,
      int count)
    {
      Buffer.BlockCopy((Array) src, srcOffset * 2, (Array) dst, dstOffset * 2, count * 2);
    }

    private void ShiftBufferIfNeeded()
    {
      int length = this._chars.Length;
      if ((double) (length - this._charPos) > (double) length * 0.1)
        return;
      int count = this._charsUsed - this._charPos;
      if (count > 0)
        JsonTextReader.BlockCopyChars(this._chars, this._charPos, this._chars, 0, count);
      this._lineStartPos -= this._charPos;
      this._charPos = 0;
      this._charsUsed = count;
      this._chars[this._charsUsed] = char.MinValue;
    }

    private int ReadData(bool append) => this.ReadData(append, 0);

    private int ReadData(bool append, int charsRequired)
    {
      if (this._isEndOfFile)
        return 0;
      if (this._charsUsed + charsRequired >= this._chars.Length - 1)
      {
        if (append)
        {
          char[] dst = new char[Math.Max(this._chars.Length * 2, this._charsUsed + charsRequired + 1)];
          JsonTextReader.BlockCopyChars(this._chars, 0, dst, 0, this._chars.Length);
          this._chars = dst;
        }
        else
        {
          int count = this._charsUsed - this._charPos;
          if (count + charsRequired + 1 >= this._chars.Length)
          {
            char[] dst = new char[count + charsRequired + 1];
            if (count > 0)
              JsonTextReader.BlockCopyChars(this._chars, this._charPos, dst, 0, count);
            this._chars = dst;
          }
          else if (count > 0)
            JsonTextReader.BlockCopyChars(this._chars, this._charPos, this._chars, 0, count);
          this._lineStartPos -= this._charPos;
          this._charPos = 0;
          this._charsUsed = count;
        }
      }
      int num = this._reader.Read(this._chars, this._charsUsed, this._chars.Length - this._charsUsed - 1);
      this._charsUsed += num;
      if (num == 0)
        this._isEndOfFile = true;
      this._chars[this._charsUsed] = char.MinValue;
      return num;
    }

    private bool EnsureChars(int relativePosition, bool append)
    {
      return this._charPos + relativePosition < this._charsUsed || this.ReadChars(relativePosition, append);
    }

    private bool ReadChars(int relativePosition, bool append)
    {
      if (this._isEndOfFile)
        return false;
      int num1 = this._charPos + relativePosition - this._charsUsed + 1;
      int num2 = 0;
      do
      {
        int num3 = this.ReadData(append, num1 - num2);
        if (num3 != 0)
          num2 += num3;
        else
          break;
      }
      while (num2 < num1);
      return num2 >= num1;
    }

    /// <summary>Reads the next JSON token from the stream.</summary>
    /// <returns>
    /// true if the next token was read successfully; false if there are no more tokens to read.
    /// </returns>
    [DebuggerStepThrough]
    public override bool Read()
    {
      this._readType = ReadType.Read;
      if (this.ReadInternal())
        return true;
      this.SetToken(JsonToken.None);
      return false;
    }

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:System.Byte" />[].
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Byte" />[] or a null reference if the next JSON token is null. This method will return <c>null</c> at the end of an array.
    /// </returns>
    public override byte[] ReadAsBytes() => this.ReadAsBytesInternal();

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <returns>A <see cref="T:System.Nullable`1" />. This method will return <c>null</c> at the end of an array.</returns>
    public override Decimal? ReadAsDecimal() => this.ReadAsDecimalInternal();

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <returns>A <see cref="T:System.Nullable`1" />. This method will return <c>null</c> at the end of an array.</returns>
    public override int? ReadAsInt32() => this.ReadAsInt32Internal();

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:System.String" />.
    /// </summary>
    /// <returns>A <see cref="T:System.String" />. This method will return <c>null</c> at the end of an array.</returns>
    public override string ReadAsString() => this.ReadAsStringInternal();

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <returns>A <see cref="T:System.String" />. This method will return <c>null</c> at the end of an array.</returns>
    public override DateTime? ReadAsDateTime() => this.ReadAsDateTimeInternal();

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <returns>A <see cref="T:System.DateTimeOffset" />. This method will return <c>null</c> at the end of an array.</returns>
    public override DateTimeOffset? ReadAsDateTimeOffset() => this.ReadAsDateTimeOffsetInternal();

    internal override bool ReadInternal()
    {
      do
      {
        switch (this._currentState)
        {
          case JsonReader.State.Start:
          case JsonReader.State.Property:
          case JsonReader.State.ArrayStart:
          case JsonReader.State.Array:
          case JsonReader.State.ConstructorStart:
          case JsonReader.State.Constructor:
            return this.ParseValue();
          case JsonReader.State.ObjectStart:
          case JsonReader.State.Object:
            return this.ParseObject();
          case JsonReader.State.PostValue:
            continue;
          case JsonReader.State.Finished:
            goto label_5;
          default:
            goto label_12;
        }
      }
      while (!this.ParsePostValue());
      return true;
label_5:
      if (!this.EnsureChars(0, false))
        return false;
      this.EatWhitespace(false);
      if (this._isEndOfFile)
        return false;
      if (this._chars[this._charPos] != '/')
        throw JsonReaderException.Create((JsonReader) this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this._chars[this._charPos]));
      this.ParseComment();
      return true;
label_12:
      throw JsonReaderException.Create((JsonReader) this, "Unexpected state: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this.CurrentState));
    }

    private void ReadStringIntoBuffer(char quote)
    {
      int charPos1 = this._charPos;
      int charPos2 = this._charPos;
      int num1 = this._charPos;
      StringBuffer buffer = (StringBuffer) null;
      do
      {
        char ch1 = this._chars[charPos1++];
        if (ch1 <= '\r')
        {
          if (ch1 != char.MinValue)
          {
            if (ch1 != '\n')
            {
              if (ch1 == '\r')
              {
                this._charPos = charPos1 - 1;
                this.ProcessCarriageReturn(true);
                charPos1 = this._charPos;
              }
            }
            else
            {
              this._charPos = charPos1 - 1;
              this.ProcessLineFeed();
              charPos1 = this._charPos;
            }
          }
          else if (this._charsUsed == charPos1 - 1)
          {
            --charPos1;
            if (this.ReadData(true) == 0)
            {
              this._charPos = charPos1;
              throw JsonReaderException.Create((JsonReader) this, "Unterminated string. Expected delimiter: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) quote));
            }
          }
        }
        else if (ch1 != '"' && ch1 != '\'')
        {
          if (ch1 == '\\')
          {
            this._charPos = charPos1;
            if (!this.EnsureChars(0, true))
            {
              this._charPos = charPos1;
              throw JsonReaderException.Create((JsonReader) this, "Unterminated string. Expected delimiter: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) quote));
            }
            int writeToPosition = charPos1 - 1;
            char ch2 = this._chars[charPos1];
            char ch3;
            switch (ch2)
            {
              case '"':
              case '\'':
              case '/':
                ch3 = ch2;
                ++charPos1;
                break;
              case '\\':
                ++charPos1;
                ch3 = '\\';
                break;
              case 'b':
                ++charPos1;
                ch3 = '\b';
                break;
              case 'f':
                ++charPos1;
                ch3 = '\f';
                break;
              case 'n':
                ++charPos1;
                ch3 = '\n';
                break;
              case 'r':
                ++charPos1;
                ch3 = '\r';
                break;
              case 't':
                ++charPos1;
                ch3 = '\t';
                break;
              case 'u':
                this._charPos = charPos1 + 1;
                ch3 = this.ParseUnicode();
                if (StringUtils.IsLowSurrogate(ch3))
                  ch3 = '�';
                else if (StringUtils.IsHighSurrogate(ch3))
                {
                  bool flag;
                  do
                  {
                    flag = false;
                    if (this.EnsureChars(2, true) && this._chars[this._charPos] == '\\' && this._chars[this._charPos + 1] == 'u')
                    {
                      char writeChar = ch3;
                      this._charPos += 2;
                      ch3 = this.ParseUnicode();
                      if (!StringUtils.IsLowSurrogate(ch3))
                      {
                        if (StringUtils.IsHighSurrogate(ch3))
                        {
                          writeChar = '�';
                          flag = true;
                        }
                        else
                          writeChar = '�';
                      }
                      if (buffer == null)
                        buffer = this.GetBuffer();
                      this.WriteCharToBuffer(buffer, writeChar, num1, writeToPosition);
                      num1 = this._charPos;
                    }
                    else
                      ch3 = '�';
                  }
                  while (flag);
                }
                charPos1 = this._charPos;
                break;
              default:
                this._charPos = charPos1 + 1;
                throw JsonReaderException.Create((JsonReader) this, "Bad JSON escape sequence: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) ("\\" + (object) ch2)));
            }
            if (buffer == null)
              buffer = this.GetBuffer();
            this.WriteCharToBuffer(buffer, ch3, num1, writeToPosition);
            num1 = charPos1;
          }
        }
      }
      while ((int) this._chars[charPos1 - 1] != (int) quote);
      int num2 = charPos1 - 1;
      if (charPos2 == num1)
      {
        this._stringReference = new StringReference(this._chars, charPos2, num2 - charPos2);
      }
      else
      {
        if (buffer == null)
          buffer = this.GetBuffer();
        if (num2 > num1)
          buffer.Append(this._chars, num1, num2 - num1);
        this._stringReference = new StringReference(buffer.GetInternalBuffer(), 0, buffer.Position);
      }
      this._charPos = num2 + 1;
    }

    private void WriteCharToBuffer(
      StringBuffer buffer,
      char writeChar,
      int lastWritePosition,
      int writeToPosition)
    {
      if (writeToPosition > lastWritePosition)
        buffer.Append(this._chars, lastWritePosition, writeToPosition - lastWritePosition);
      buffer.Append(writeChar);
    }

    private char ParseUnicode()
    {
      if (!this.EnsureChars(4, true))
        throw JsonReaderException.Create((JsonReader) this, "Unexpected end while parsing unicode character.");
      char unicode = Convert.ToChar(int.Parse(new string(this._chars, this._charPos, 4), NumberStyles.HexNumber, (IFormatProvider) NumberFormatInfo.InvariantInfo));
      this._charPos += 4;
      return unicode;
    }

    private void ReadNumberIntoBuffer()
    {
      int charPos = this._charPos;
      while (true)
      {
        do
        {
          switch (this._chars[charPos])
          {
            case char.MinValue:
              this._charPos = charPos;
              continue;
            case '+':
            case '-':
            case '.':
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
            case 'A':
            case 'B':
            case 'C':
            case 'D':
            case 'E':
            case 'F':
            case 'X':
            case 'a':
            case 'b':
            case 'c':
            case 'd':
            case 'e':
            case 'f':
            case 'x':
              goto label_5;
            default:
              goto label_6;
          }
        }
        while (this._charsUsed == charPos && this.ReadData(true) != 0);
        break;
label_5:
        ++charPos;
      }
      return;
label_6:
      this._charPos = charPos;
      char c = this._chars[this._charPos];
      if (!char.IsWhiteSpace(c) && c != ',' && c != '}' && c != ']' && c != ')' && c != '/')
        throw JsonReaderException.Create((JsonReader) this, "Unexpected character encountered while parsing number: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) c));
    }

    private void ClearRecentString()
    {
      if (this._buffer != null)
        this._buffer.Position = 0;
      this._stringReference = new StringReference();
    }

    private bool ParsePostValue()
    {
      char c;
      while (true)
      {
        do
        {
          c = this._chars[this._charPos];
          switch (c)
          {
            case char.MinValue:
              if (this._charsUsed == this._charPos)
                continue;
              goto label_4;
            case '\t':
            case ' ':
              goto label_10;
            case '\n':
              goto label_12;
            case '\r':
              goto label_11;
            case ')':
              goto label_7;
            case ',':
              goto label_9;
            case '/':
              goto label_8;
            case ']':
              goto label_6;
            case '}':
              goto label_5;
            default:
              goto label_13;
          }
        }
        while (this.ReadData(false) != 0);
        break;
label_4:
        ++this._charPos;
        continue;
label_10:
        ++this._charPos;
        continue;
label_11:
        this.ProcessCarriageReturn(false);
        continue;
label_12:
        this.ProcessLineFeed();
        continue;
label_13:
        if (char.IsWhiteSpace(c))
          ++this._charPos;
        else
          goto label_15;
      }
      this._currentState = JsonReader.State.Finished;
      return false;
label_5:
      ++this._charPos;
      this.SetToken(JsonToken.EndObject);
      return true;
label_6:
      ++this._charPos;
      this.SetToken(JsonToken.EndArray);
      return true;
label_7:
      ++this._charPos;
      this.SetToken(JsonToken.EndConstructor);
      return true;
label_8:
      this.ParseComment();
      return true;
label_9:
      ++this._charPos;
      this.SetStateBasedOnCurrent();
      return false;
label_15:
      throw JsonReaderException.Create((JsonReader) this, "After parsing a value an unexpected character was encountered: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) c));
    }

    private bool ParseObject()
    {
      while (true)
      {
        char c;
        do
        {
          c = this._chars[this._charPos];
          switch (c)
          {
            case char.MinValue:
              if (this._charsUsed == this._charPos)
                continue;
              goto label_4;
            case '\t':
            case ' ':
              goto label_9;
            case '\n':
              goto label_8;
            case '\r':
              goto label_7;
            case '/':
              goto label_6;
            case '}':
              goto label_5;
            default:
              goto label_10;
          }
        }
        while (this.ReadData(false) != 0);
        break;
label_4:
        ++this._charPos;
        continue;
label_7:
        this.ProcessCarriageReturn(false);
        continue;
label_8:
        this.ProcessLineFeed();
        continue;
label_9:
        ++this._charPos;
        continue;
label_10:
        if (char.IsWhiteSpace(c))
          ++this._charPos;
        else
          goto label_12;
      }
      return false;
label_5:
      this.SetToken(JsonToken.EndObject);
      ++this._charPos;
      return true;
label_6:
      this.ParseComment();
      return true;
label_12:
      return this.ParseProperty();
    }

    private bool ParseProperty()
    {
      char ch = this._chars[this._charPos];
      char quote;
      switch (ch)
      {
        case '"':
        case '\'':
          ++this._charPos;
          quote = ch;
          this.ShiftBufferIfNeeded();
          this.ReadStringIntoBuffer(quote);
          break;
        default:
          if (!this.ValidIdentifierChar(ch))
            throw JsonReaderException.Create((JsonReader) this, "Invalid property identifier character: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this._chars[this._charPos]));
          quote = char.MinValue;
          this.ShiftBufferIfNeeded();
          this.ParseUnquotedProperty();
          break;
      }
      string str = this.NameTable == null ? this._stringReference.ToString() : this.NameTable.Get(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length) ?? this._stringReference.ToString();
      this.EatWhitespace(false);
      if (this._chars[this._charPos] != ':')
        throw JsonReaderException.Create((JsonReader) this, "Invalid character after parsing property name. Expected ':' but got: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this._chars[this._charPos]));
      ++this._charPos;
      this.SetToken(JsonToken.PropertyName, (object) str);
      this._quoteChar = quote;
      this.ClearRecentString();
      return true;
    }

    private bool ValidIdentifierChar(char value)
    {
      return char.IsLetterOrDigit(value) || value == '_' || value == '$';
    }

    private void ParseUnquotedProperty()
    {
      int charPos = this._charPos;
      do
      {
        for (; this._chars[this._charPos] != char.MinValue; ++this._charPos)
        {
          char c = this._chars[this._charPos];
          if (!this.ValidIdentifierChar(c))
          {
            if (!char.IsWhiteSpace(c) && c != ':')
              throw JsonReaderException.Create((JsonReader) this, "Invalid JavaScript property identifier character: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) c));
            this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
            return;
          }
        }
        if (this._charsUsed != this._charPos)
          goto label_5;
      }
      while (this.ReadData(true) != 0);
      throw JsonReaderException.Create((JsonReader) this, "Unexpected end while parsing unquoted property name.");
label_5:
      this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
    }

    private bool ParseValue()
    {
      char ch;
      while (true)
      {
        do
        {
          ch = this._chars[this._charPos];
          switch (ch)
          {
            case char.MinValue:
              if (this._charsUsed == this._charPos)
                continue;
              goto label_4;
            case '\t':
            case ' ':
              goto label_30;
            case '\n':
              goto label_29;
            case '\r':
              goto label_28;
            case '"':
            case '\'':
              goto label_5;
            case ')':
              goto label_27;
            case ',':
              goto label_26;
            case '-':
              goto label_17;
            case '/':
              goto label_21;
            case 'I':
              goto label_16;
            case 'N':
              goto label_15;
            case '[':
              goto label_24;
            case ']':
              goto label_25;
            case 'f':
              goto label_7;
            case 'n':
              goto label_8;
            case 't':
              goto label_6;
            case 'u':
              goto label_22;
            case '{':
              goto label_23;
            default:
              goto label_31;
          }
        }
        while (this.ReadData(false) != 0);
        break;
label_4:
        ++this._charPos;
        continue;
label_28:
        this.ProcessCarriageReturn(false);
        continue;
label_29:
        this.ProcessLineFeed();
        continue;
label_30:
        ++this._charPos;
        continue;
label_31:
        if (char.IsWhiteSpace(ch))
          ++this._charPos;
        else
          goto label_33;
      }
      return false;
label_5:
      this.ParseString(ch);
      return true;
label_6:
      this.ParseTrue();
      return true;
label_7:
      this.ParseFalse();
      return true;
label_8:
      if (!this.EnsureChars(1, true))
        throw JsonReaderException.Create((JsonReader) this, "Unexpected end.");
      switch (this._chars[this._charPos + 1])
      {
        case 'e':
          this.ParseConstructor();
          break;
        case 'u':
          this.ParseNull();
          break;
        default:
          throw JsonReaderException.Create((JsonReader) this, "Unexpected character encountered while parsing value: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this._chars[this._charPos]));
      }
      return true;
label_15:
      this.ParseNumberNaN();
      return true;
label_16:
      this.ParseNumberPositiveInfinity();
      return true;
label_17:
      if (this.EnsureChars(1, true) && this._chars[this._charPos + 1] == 'I')
        this.ParseNumberNegativeInfinity();
      else
        this.ParseNumber();
      return true;
label_21:
      this.ParseComment();
      return true;
label_22:
      this.ParseUndefined();
      return true;
label_23:
      ++this._charPos;
      this.SetToken(JsonToken.StartObject);
      return true;
label_24:
      ++this._charPos;
      this.SetToken(JsonToken.StartArray);
      return true;
label_25:
      ++this._charPos;
      this.SetToken(JsonToken.EndArray);
      return true;
label_26:
      this.SetToken(JsonToken.Undefined);
      return true;
label_27:
      ++this._charPos;
      this.SetToken(JsonToken.EndConstructor);
      return true;
label_33:
      if (!char.IsNumber(ch) && ch != '-' && ch != '.')
        throw JsonReaderException.Create((JsonReader) this, "Unexpected character encountered while parsing value: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) ch));
      this.ParseNumber();
      return true;
    }

    private void ProcessLineFeed()
    {
      ++this._charPos;
      this.OnNewLine(this._charPos);
    }

    private void ProcessCarriageReturn(bool append)
    {
      ++this._charPos;
      if (this.EnsureChars(1, append) && this._chars[this._charPos] == '\n')
        ++this._charPos;
      this.OnNewLine(this._charPos);
    }

    private bool EatWhitespace(bool oneOrMore)
    {
      bool flag1 = false;
      bool flag2 = false;
      while (!flag1)
      {
        char c = this._chars[this._charPos];
        switch (c)
        {
          case char.MinValue:
            if (this._charsUsed == this._charPos)
            {
              if (this.ReadData(false) == 0)
              {
                flag1 = true;
                continue;
              }
              continue;
            }
            ++this._charPos;
            continue;
          case '\n':
            this.ProcessLineFeed();
            continue;
          case '\r':
            this.ProcessCarriageReturn(false);
            continue;
          default:
            if (c == ' ' || char.IsWhiteSpace(c))
            {
              flag2 = true;
              ++this._charPos;
              continue;
            }
            flag1 = true;
            continue;
        }
      }
      return !oneOrMore || flag2;
    }

    private void ParseConstructor()
    {
      if (!this.MatchValueWithTrailingSeparator("new"))
        throw JsonReaderException.Create((JsonReader) this, "Unexpected content while parsing JSON.");
      this.EatWhitespace(false);
      int charPos1 = this._charPos;
      char c;
      while (true)
      {
        do
        {
          c = this._chars[this._charPos];
          if (c == char.MinValue)
          {
            if (this._charsUsed != this._charPos)
              goto label_6;
          }
          else
            goto label_7;
        }
        while (this.ReadData(true) != 0);
        break;
label_7:
        if (char.IsLetterOrDigit(c))
          ++this._charPos;
        else
          goto label_9;
      }
      throw JsonReaderException.Create((JsonReader) this, "Unexpected end while parsing constructor.");
label_6:
      int charPos2 = this._charPos;
      ++this._charPos;
      goto label_17;
label_9:
      switch (c)
      {
        case '\n':
          charPos2 = this._charPos;
          this.ProcessLineFeed();
          break;
        case '\r':
          charPos2 = this._charPos;
          this.ProcessCarriageReturn(true);
          break;
        default:
          if (char.IsWhiteSpace(c))
          {
            charPos2 = this._charPos;
            ++this._charPos;
            break;
          }
          if (c != '(')
            throw JsonReaderException.Create((JsonReader) this, "Unexpected character while parsing constructor: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) c));
          charPos2 = this._charPos;
          break;
      }
label_17:
      this._stringReference = new StringReference(this._chars, charPos1, charPos2 - charPos1);
      string str = this._stringReference.ToString();
      this.EatWhitespace(false);
      if (this._chars[this._charPos] != '(')
        throw JsonReaderException.Create((JsonReader) this, "Unexpected character while parsing constructor: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this._chars[this._charPos]));
      ++this._charPos;
      this.ClearRecentString();
      this.SetToken(JsonToken.StartConstructor, (object) str);
    }

    private void ParseNumber()
    {
      this.ShiftBufferIfNeeded();
      char c = this._chars[this._charPos];
      int charPos = this._charPos;
      this.ReadNumberIntoBuffer();
      this.SetPostValueState(true);
      this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
      bool flag1 = char.IsDigit(c) && this._stringReference.Length == 1;
      bool flag2 = c == '0' && this._stringReference.Length > 1 && this._stringReference.Chars[this._stringReference.StartIndex + 1] != '.' && this._stringReference.Chars[this._stringReference.StartIndex + 1] != 'e' && this._stringReference.Chars[this._stringReference.StartIndex + 1] != 'E';
      object obj;
      JsonToken newToken;
      if (this._readType == ReadType.ReadAsInt32)
      {
        if (flag1)
          obj = (object) ((int) c - 48);
        else if (flag2)
        {
          string str = this._stringReference.ToString();
          try
          {
            obj = (object) (str.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt32(str, 16) : Convert.ToInt32(str, 8));
          }
          catch (Exception ex)
          {
            throw JsonReaderException.Create((JsonReader) this, "Input string '{0}' is not a valid integer.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) str), ex);
          }
        }
        else
        {
          int num;
          switch (ConvertUtils.Int32TryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out num))
          {
            case ParseResult.Success:
              obj = (object) num;
              break;
            case ParseResult.Overflow:
              throw JsonReaderException.Create((JsonReader) this, "JSON integer {0} is too large or small for an Int32.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this._stringReference.ToString()));
            default:
              throw JsonReaderException.Create((JsonReader) this, "Input string '{0}' is not a valid integer.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this._stringReference.ToString()));
          }
        }
        newToken = JsonToken.Integer;
      }
      else if (this._readType == ReadType.ReadAsDecimal)
      {
        if (flag1)
          obj = (object) ((Decimal) c - 48M);
        else if (flag2)
        {
          string str = this._stringReference.ToString();
          try
          {
            obj = (object) Convert.ToDecimal(str.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(str, 16) : Convert.ToInt64(str, 8));
          }
          catch (Exception ex)
          {
            throw JsonReaderException.Create((JsonReader) this, "Input string '{0}' is not a valid decimal.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) str), ex);
          }
        }
        else
        {
          Decimal result;
          if (!Decimal.TryParse(this._stringReference.ToString(), NumberStyles.Number | NumberStyles.AllowExponent, (IFormatProvider) CultureInfo.InvariantCulture, out result))
            throw JsonReaderException.Create((JsonReader) this, "Input string '{0}' is not a valid decimal.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this._stringReference.ToString()));
          obj = (object) result;
        }
        newToken = JsonToken.Float;
      }
      else if (flag1)
      {
        obj = (object) ((long) c - 48L);
        newToken = JsonToken.Integer;
      }
      else if (flag2)
      {
        string str = this._stringReference.ToString();
        try
        {
          obj = (object) (str.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(str, 16) : Convert.ToInt64(str, 8));
        }
        catch (Exception ex)
        {
          throw JsonReaderException.Create((JsonReader) this, "Input string '{0}' is not a valid number.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) str), ex);
        }
        newToken = JsonToken.Integer;
      }
      else
      {
        long num;
        switch (ConvertUtils.Int64TryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out num))
        {
          case ParseResult.Success:
            obj = (object) num;
            newToken = JsonToken.Integer;
            break;
          case ParseResult.Overflow:
            throw JsonReaderException.Create((JsonReader) this, "JSON integer {0} is too large or small for an Int64.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this._stringReference.ToString()));
          default:
            string s = this._stringReference.ToString();
            if (this._floatParseHandling == FloatParseHandling.Decimal)
            {
              Decimal result;
              if (!Decimal.TryParse(s, NumberStyles.Number | NumberStyles.AllowExponent, (IFormatProvider) CultureInfo.InvariantCulture, out result))
                throw JsonReaderException.Create((JsonReader) this, "Input string '{0}' is not a valid decimal.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) s));
              obj = (object) result;
            }
            else
            {
              double result;
              if (!double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider) CultureInfo.InvariantCulture, out result))
                throw JsonReaderException.Create((JsonReader) this, "Input string '{0}' is not a valid number.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) s));
              obj = (object) result;
            }
            newToken = JsonToken.Float;
            break;
        }
      }
      this.ClearRecentString();
      this.SetToken(newToken, obj, false);
    }

    private void ParseComment()
    {
      ++this._charPos;
      if (!this.EnsureChars(1, false))
        throw JsonReaderException.Create((JsonReader) this, "Unexpected end while parsing comment.");
      bool flag1;
      if (this._chars[this._charPos] == '*')
      {
        flag1 = false;
      }
      else
      {
        if (this._chars[this._charPos] != '/')
          throw JsonReaderException.Create((JsonReader) this, "Error parsing comment. Expected: *, got {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this._chars[this._charPos]));
        flag1 = true;
      }
      ++this._charPos;
      int charPos = this._charPos;
      bool flag2 = false;
      while (!flag2)
      {
        switch (this._chars[this._charPos])
        {
          case char.MinValue:
            if (this._charsUsed == this._charPos)
            {
              if (this.ReadData(true) == 0)
              {
                if (!flag1)
                  throw JsonReaderException.Create((JsonReader) this, "Unexpected end while parsing comment.");
                this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
                flag2 = true;
                continue;
              }
              continue;
            }
            ++this._charPos;
            continue;
          case '\n':
            if (flag1)
            {
              this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
              flag2 = true;
            }
            this.ProcessLineFeed();
            continue;
          case '\r':
            if (flag1)
            {
              this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
              flag2 = true;
            }
            this.ProcessCarriageReturn(true);
            continue;
          case '*':
            ++this._charPos;
            if (!flag1 && this.EnsureChars(0, true) && this._chars[this._charPos] == '/')
            {
              this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos - 1);
              ++this._charPos;
              flag2 = true;
              continue;
            }
            continue;
          default:
            ++this._charPos;
            continue;
        }
      }
      this.SetToken(JsonToken.Comment, (object) this._stringReference.ToString());
      this.ClearRecentString();
    }

    private bool MatchValue(string value)
    {
      if (!this.EnsureChars(value.Length - 1, true))
        return false;
      for (int index = 0; index < value.Length; ++index)
      {
        if ((int) this._chars[this._charPos + index] != (int) value[index])
          return false;
      }
      this._charPos += value.Length;
      return true;
    }

    private bool MatchValueWithTrailingSeparator(string value)
    {
      if (!this.MatchValue(value))
        return false;
      return !this.EnsureChars(0, false) || this.IsSeparator(this._chars[this._charPos]) || this._chars[this._charPos] == char.MinValue;
    }

    private bool IsSeparator(char c)
    {
      switch (c)
      {
        case '\t':
        case '\n':
        case '\r':
        case ' ':
          return true;
        case ')':
          if (this.CurrentState == JsonReader.State.Constructor || this.CurrentState == JsonReader.State.ConstructorStart)
            return true;
          break;
        case ',':
        case ']':
        case '}':
          return true;
        case '/':
          if (!this.EnsureChars(1, false))
            return false;
          char ch = this._chars[this._charPos + 1];
          return ch == '*' || ch == '/';
        default:
          if (char.IsWhiteSpace(c))
            return true;
          break;
      }
      return false;
    }

    private void ParseTrue()
    {
      if (!this.MatchValueWithTrailingSeparator(JsonConvert.True))
        throw JsonReaderException.Create((JsonReader) this, "Error parsing boolean value.");
      this.SetToken(JsonToken.Boolean, (object) true);
    }

    private void ParseNull()
    {
      if (!this.MatchValueWithTrailingSeparator(JsonConvert.Null))
        throw JsonReaderException.Create((JsonReader) this, "Error parsing null value.");
      this.SetToken(JsonToken.Null);
    }

    private void ParseUndefined()
    {
      if (!this.MatchValueWithTrailingSeparator(JsonConvert.Undefined))
        throw JsonReaderException.Create((JsonReader) this, "Error parsing undefined value.");
      this.SetToken(JsonToken.Undefined);
    }

    private void ParseFalse()
    {
      if (!this.MatchValueWithTrailingSeparator(JsonConvert.False))
        throw JsonReaderException.Create((JsonReader) this, "Error parsing boolean value.");
      this.SetToken(JsonToken.Boolean, (object) false);
    }

    private void ParseNumberNegativeInfinity()
    {
      if (!this.MatchValueWithTrailingSeparator(JsonConvert.NegativeInfinity))
        throw JsonReaderException.Create((JsonReader) this, "Error parsing negative infinity value.");
      if (this._floatParseHandling == FloatParseHandling.Decimal)
        throw new JsonReaderException("Cannot read -Infinity as a decimal.");
      this.SetToken(JsonToken.Float, (object) double.NegativeInfinity);
    }

    private void ParseNumberPositiveInfinity()
    {
      if (!this.MatchValueWithTrailingSeparator(JsonConvert.PositiveInfinity))
        throw JsonReaderException.Create((JsonReader) this, "Error parsing positive infinity value.");
      if (this._floatParseHandling == FloatParseHandling.Decimal)
        throw new JsonReaderException("Cannot read Infinity as a decimal.");
      this.SetToken(JsonToken.Float, (object) double.PositiveInfinity);
    }

    private void ParseNumberNaN()
    {
      if (!this.MatchValueWithTrailingSeparator(JsonConvert.NaN))
        throw JsonReaderException.Create((JsonReader) this, "Error parsing NaN value.");
      if (this._floatParseHandling == FloatParseHandling.Decimal)
        throw new JsonReaderException("Cannot read NaN as a decimal.");
      this.SetToken(JsonToken.Float, (object) double.NaN);
    }

    /// <summary>Changes the state to closed.</summary>
    public override void Close()
    {
      base.Close();
      if (this.CloseInput && this._reader != null)
        this._reader.Dispose();
      if (this._buffer == null)
        return;
      this._buffer.Clear();
    }

    /// <summary>
    /// Gets a value indicating whether the class can return line information.
    /// </summary>
    /// <returns>
    /// 	<c>true</c> if LineNumber and LinePosition can be provided; otherwise, <c>false</c>.
    /// </returns>
    public bool HasLineInfo() => true;

    /// <summary>Gets the current line number.</summary>
    /// <value>
    /// The current line number or 0 if no line information is available (for example, HasLineInfo returns false).
    /// </value>
    public int LineNumber
    {
      get
      {
        return this.CurrentState == JsonReader.State.Start && this.LinePosition == 0 ? 0 : this._lineNumber;
      }
    }

    /// <summary>Gets the current line position.</summary>
    /// <value>
    /// The current line position or 0 if no line information is available (for example, HasLineInfo returns false).
    /// </value>
    public int LinePosition => this._charPos - this._lineStartPos;
  }
}
