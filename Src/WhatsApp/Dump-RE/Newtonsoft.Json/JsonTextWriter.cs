// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonTextWriter
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;
using System.IO;

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Represents a writer that provides a fast, non-cached, forward-only way of generating JSON data.
  /// </summary>
  public class JsonTextWriter : JsonWriter
  {
    private readonly TextWriter _writer;
    private Base64Encoder _base64Encoder;
    private char _indentChar;
    private int _indentation;
    private char _quoteChar;
    private bool _quoteName;
    private bool[] _charEscapeFlags;
    private char[] _writeBuffer;
    private char[] _indentChars;

    private Base64Encoder Base64Encoder
    {
      get
      {
        if (this._base64Encoder == null)
          this._base64Encoder = new Base64Encoder(this._writer);
        return this._base64Encoder;
      }
    }

    /// <summary>
    /// Gets or sets how many IndentChars to write for each level in the hierarchy when <see cref="T:Newtonsoft.Json.Formatting" /> is set to <c>Formatting.Indented</c>.
    /// </summary>
    public int Indentation
    {
      get => this._indentation;
      set
      {
        this._indentation = value >= 0 ? value : throw new ArgumentException("Indentation value must be greater than 0.");
      }
    }

    /// <summary>
    /// Gets or sets which character to use to quote attribute values.
    /// </summary>
    public char QuoteChar
    {
      get => this._quoteChar;
      set
      {
        this._quoteChar = value == '"' || value == '\'' ? value : throw new ArgumentException("Invalid JavaScript string quote character. Valid quote characters are ' and \".");
        this.UpdateCharEscapeFlags();
      }
    }

    /// <summary>
    /// Gets or sets which character to use for indenting when <see cref="T:Newtonsoft.Json.Formatting" /> is set to <c>Formatting.Indented</c>.
    /// </summary>
    public char IndentChar
    {
      get => this._indentChar;
      set
      {
        if ((int) value == (int) this._indentChar)
          return;
        this._indentChar = value;
        this._indentChars = (char[]) null;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether object names will be surrounded with quotes.
    /// </summary>
    public bool QuoteName
    {
      get => this._quoteName;
      set => this._quoteName = value;
    }

    /// <summary>
    /// Creates an instance of the <c>JsonWriter</c> class using the specified <see cref="T:System.IO.TextWriter" />.
    /// </summary>
    /// <param name="textWriter">The <c>TextWriter</c> to write to.</param>
    public JsonTextWriter(TextWriter textWriter)
    {
      this._writer = textWriter != null ? textWriter : throw new ArgumentNullException(nameof (textWriter));
      this._quoteChar = '"';
      this._quoteName = true;
      this._indentChar = ' ';
      this._indentation = 2;
      this.UpdateCharEscapeFlags();
    }

    /// <summary>
    /// Flushes whatever is in the buffer to the underlying streams and also flushes the underlying stream.
    /// </summary>
    public override void Flush() => this._writer.Flush();

    /// <summary>Closes this stream and the underlying stream.</summary>
    public override void Close()
    {
      base.Close();
      if (!this.CloseOutput || this._writer == null)
        return;
      this._writer.Dispose();
    }

    /// <summary>Writes the beginning of a JSON object.</summary>
    public override void WriteStartObject()
    {
      this.InternalWriteStart(JsonToken.StartObject, JsonContainerType.Object);
      this._writer.Write('{');
    }

    /// <summary>Writes the beginning of a JSON array.</summary>
    public override void WriteStartArray()
    {
      this.InternalWriteStart(JsonToken.StartArray, JsonContainerType.Array);
      this._writer.Write('[');
    }

    /// <summary>
    /// Writes the start of a constructor with the given name.
    /// </summary>
    /// <param name="name">The name of the constructor.</param>
    public override void WriteStartConstructor(string name)
    {
      this.InternalWriteStart(JsonToken.StartConstructor, JsonContainerType.Constructor);
      this._writer.Write("new ");
      this._writer.Write(name);
      this._writer.Write('(');
    }

    /// <summary>Writes the specified end token.</summary>
    /// <param name="token">The end token to write.</param>
    protected override void WriteEnd(JsonToken token)
    {
      switch (token)
      {
        case JsonToken.EndObject:
          this._writer.Write('}');
          break;
        case JsonToken.EndArray:
          this._writer.Write(']');
          break;
        case JsonToken.EndConstructor:
          this._writer.Write(')');
          break;
        default:
          throw JsonWriterException.Create((JsonWriter) this, "Invalid JsonToken: " + (object) token, (Exception) null);
      }
    }

    /// <summary>
    /// Writes the property name of a name/value pair on a JSON object.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    public override void WritePropertyName(string name)
    {
      this.InternalWritePropertyName(name);
      this.WriteEscapedString(name, this._quoteName);
      this._writer.Write(':');
    }

    /// <summary>
    /// Writes the property name of a name/value pair on a JSON object.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="escape">A flag to indicate whether the text should be escaped when it is written as a JSON property name.</param>
    public override void WritePropertyName(string name, bool escape)
    {
      this.InternalWritePropertyName(name);
      if (escape)
      {
        this.WriteEscapedString(name, this._quoteName);
      }
      else
      {
        if (this._quoteName)
          this._writer.Write(this._quoteChar);
        this._writer.Write(name);
        if (this._quoteName)
          this._writer.Write(this._quoteChar);
      }
      this._writer.Write(':');
    }

    internal override void OnStringEscapeHandlingChanged() => this.UpdateCharEscapeFlags();

    private void UpdateCharEscapeFlags()
    {
      this._charEscapeFlags = JavaScriptUtils.GetCharEscapeFlags(this.StringEscapeHandling, this._quoteChar);
    }

    /// <summary>Writes indent characters.</summary>
    protected override void WriteIndent()
    {
      this._writer.WriteLine();
      int val1 = this.Top * this._indentation;
      if (val1 <= 0)
        return;
      if (this._indentChars == null)
        this._indentChars = new string(this._indentChar, 10).ToCharArray();
      int count;
      for (; val1 > 0; val1 -= count)
      {
        count = Math.Min(val1, 10);
        this._writer.Write(this._indentChars, 0, count);
      }
    }

    /// <summary>Writes the JSON value delimiter.</summary>
    protected override void WriteValueDelimiter() => this._writer.Write(',');

    /// <summary>Writes an indent space.</summary>
    protected override void WriteIndentSpace() => this._writer.Write(' ');

    private void WriteValueInternal(string value, JsonToken token) => this._writer.Write(value);

    /// <summary>
    /// Writes a <see cref="T:System.Object" /> value.
    /// An error will raised if the value cannot be written as a single JSON token.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Object" /> value to write.</param>
    public override void WriteValue(object value) => base.WriteValue(value);

    /// <summary>Writes a null value.</summary>
    public override void WriteNull()
    {
      this.InternalWriteValue(JsonToken.Null);
      this.WriteValueInternal(JsonConvert.Null, JsonToken.Null);
    }

    /// <summary>Writes an undefined value.</summary>
    public override void WriteUndefined()
    {
      this.InternalWriteValue(JsonToken.Undefined);
      this.WriteValueInternal(JsonConvert.Undefined, JsonToken.Undefined);
    }

    /// <summary>Writes raw JSON.</summary>
    /// <param name="json">The raw JSON to write.</param>
    public override void WriteRaw(string json)
    {
      this.InternalWriteRaw();
      this._writer.Write(json);
    }

    /// <summary>
    /// Writes a <see cref="T:System.String" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.String" /> value to write.</param>
    public override void WriteValue(string value)
    {
      this.InternalWriteValue(JsonToken.String);
      if (value == null)
        this.WriteValueInternal(JsonConvert.Null, JsonToken.Null);
      else
        this.WriteEscapedString(value, true);
    }

    private void WriteEscapedString(string value, bool quote)
    {
      this.EnsureWriteBuffer();
      JavaScriptUtils.WriteEscapedJavaScriptString(this._writer, value, this._quoteChar, quote, this._charEscapeFlags, this.StringEscapeHandling, ref this._writeBuffer);
    }

    /// <summary>
    /// Writes a <see cref="T:System.Int32" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Int32" /> value to write.</param>
    public override void WriteValue(int value)
    {
      this.InternalWriteValue(JsonToken.Integer);
      this.WriteIntegerValue((long) value);
    }

    /// <summary>
    /// Writes a <see cref="T:System.UInt32" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.UInt32" /> value to write.</param>
    [CLSCompliant(false)]
    public override void WriteValue(uint value)
    {
      this.InternalWriteValue(JsonToken.Integer);
      this.WriteIntegerValue((long) value);
    }

    /// <summary>
    /// Writes a <see cref="T:System.Int64" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Int64" /> value to write.</param>
    public override void WriteValue(long value)
    {
      this.InternalWriteValue(JsonToken.Integer);
      this.WriteIntegerValue(value);
    }

    /// <summary>
    /// Writes a <see cref="T:System.UInt64" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.UInt64" /> value to write.</param>
    [CLSCompliant(false)]
    public override void WriteValue(ulong value)
    {
      this.InternalWriteValue(JsonToken.Integer);
      this.WriteIntegerValue(value);
    }

    /// <summary>
    /// Writes a <see cref="T:System.Single" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Single" /> value to write.</param>
    public override void WriteValue(float value)
    {
      this.InternalWriteValue(JsonToken.Float);
      this.WriteValueInternal(JsonConvert.ToString(value, this.FloatFormatHandling, this.QuoteChar, false), JsonToken.Float);
    }

    /// <summary>
    /// Writes a <see cref="T:System.Nullable`1" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Nullable`1" /> value to write.</param>
    public override void WriteValue(float? value)
    {
      if (!value.HasValue)
      {
        this.WriteNull();
      }
      else
      {
        this.InternalWriteValue(JsonToken.Float);
        this.WriteValueInternal(JsonConvert.ToString(value.Value, this.FloatFormatHandling, this.QuoteChar, true), JsonToken.Float);
      }
    }

    /// <summary>
    /// Writes a <see cref="T:System.Double" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Double" /> value to write.</param>
    public override void WriteValue(double value)
    {
      this.InternalWriteValue(JsonToken.Float);
      this.WriteValueInternal(JsonConvert.ToString(value, this.FloatFormatHandling, this.QuoteChar, false), JsonToken.Float);
    }

    /// <summary>
    /// Writes a <see cref="T:System.Nullable`1" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Nullable`1" /> value to write.</param>
    public override void WriteValue(double? value)
    {
      if (!value.HasValue)
      {
        this.WriteNull();
      }
      else
      {
        this.InternalWriteValue(JsonToken.Float);
        this.WriteValueInternal(JsonConvert.ToString(value.Value, this.FloatFormatHandling, this.QuoteChar, true), JsonToken.Float);
      }
    }

    /// <summary>
    /// Writes a <see cref="T:System.Boolean" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Boolean" /> value to write.</param>
    public override void WriteValue(bool value)
    {
      this.InternalWriteValue(JsonToken.Boolean);
      this.WriteValueInternal(JsonConvert.ToString(value), JsonToken.Boolean);
    }

    /// <summary>
    /// Writes a <see cref="T:System.Int16" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Int16" /> value to write.</param>
    public override void WriteValue(short value)
    {
      this.InternalWriteValue(JsonToken.Integer);
      this.WriteIntegerValue((long) value);
    }

    /// <summary>
    /// Writes a <see cref="T:System.UInt16" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.UInt16" /> value to write.</param>
    [CLSCompliant(false)]
    public override void WriteValue(ushort value)
    {
      this.InternalWriteValue(JsonToken.Integer);
      this.WriteIntegerValue((long) value);
    }

    /// <summary>
    /// Writes a <see cref="T:System.Char" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Char" /> value to write.</param>
    public override void WriteValue(char value)
    {
      this.InternalWriteValue(JsonToken.String);
      this.WriteValueInternal(JsonConvert.ToString(value), JsonToken.String);
    }

    /// <summary>
    /// Writes a <see cref="T:System.Byte" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Byte" /> value to write.</param>
    public override void WriteValue(byte value)
    {
      this.InternalWriteValue(JsonToken.Integer);
      this.WriteIntegerValue((long) value);
    }

    /// <summary>
    /// Writes a <see cref="T:System.SByte" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.SByte" /> value to write.</param>
    [CLSCompliant(false)]
    public override void WriteValue(sbyte value)
    {
      this.InternalWriteValue(JsonToken.Integer);
      this.WriteIntegerValue((long) value);
    }

    /// <summary>
    /// Writes a <see cref="T:System.Decimal" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Decimal" /> value to write.</param>
    public override void WriteValue(Decimal value)
    {
      this.InternalWriteValue(JsonToken.Float);
      this.WriteValueInternal(JsonConvert.ToString(value), JsonToken.Float);
    }

    /// <summary>
    /// Writes a <see cref="T:System.DateTime" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.DateTime" /> value to write.</param>
    public override void WriteValue(DateTime value)
    {
      this.InternalWriteValue(JsonToken.Date);
      value = DateTimeUtils.EnsureDateTime(value, this.DateTimeZoneHandling);
      if (string.IsNullOrEmpty(this.DateFormatString))
      {
        this.EnsureWriteBuffer();
        int num1 = 0;
        char[] writeBuffer1 = this._writeBuffer;
        int index1 = num1;
        int start = index1 + 1;
        int quoteChar1 = (int) this._quoteChar;
        writeBuffer1[index1] = (char) quoteChar1;
        int num2 = DateTimeUtils.WriteDateTimeString(this._writeBuffer, start, value, new TimeSpan?(), value.Kind, this.DateFormatHandling);
        char[] writeBuffer2 = this._writeBuffer;
        int index2 = num2;
        int count = index2 + 1;
        int quoteChar2 = (int) this._quoteChar;
        writeBuffer2[index2] = (char) quoteChar2;
        this._writer.Write(this._writeBuffer, 0, count);
      }
      else
      {
        this._writer.Write(this._quoteChar);
        this._writer.Write(value.ToString(this.DateFormatString, (IFormatProvider) this.Culture));
        this._writer.Write(this._quoteChar);
      }
    }

    /// <summary>
    /// Writes a <see cref="T:System.Byte" />[] value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Byte" />[] value to write.</param>
    public override void WriteValue(byte[] value)
    {
      if (value == null)
      {
        this.WriteNull();
      }
      else
      {
        this.InternalWriteValue(JsonToken.Bytes);
        this._writer.Write(this._quoteChar);
        this.Base64Encoder.Encode(value, 0, value.Length);
        this.Base64Encoder.Flush();
        this._writer.Write(this._quoteChar);
      }
    }

    /// <summary>
    /// Writes a <see cref="T:System.DateTimeOffset" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.DateTimeOffset" /> value to write.</param>
    public override void WriteValue(DateTimeOffset value)
    {
      this.InternalWriteValue(JsonToken.Date);
      if (string.IsNullOrEmpty(this.DateFormatString))
      {
        this.EnsureWriteBuffer();
        int num1 = 0;
        char[] writeBuffer1 = this._writeBuffer;
        int index1 = num1;
        int start = index1 + 1;
        int quoteChar1 = (int) this._quoteChar;
        writeBuffer1[index1] = (char) quoteChar1;
        int num2 = DateTimeUtils.WriteDateTimeString(this._writeBuffer, start, this.DateFormatHandling == DateFormatHandling.IsoDateFormat ? value.DateTime : value.UtcDateTime, new TimeSpan?(value.Offset), DateTimeKind.Local, this.DateFormatHandling);
        char[] writeBuffer2 = this._writeBuffer;
        int index2 = num2;
        int count = index2 + 1;
        int quoteChar2 = (int) this._quoteChar;
        writeBuffer2[index2] = (char) quoteChar2;
        this._writer.Write(this._writeBuffer, 0, count);
      }
      else
      {
        this._writer.Write(this._quoteChar);
        this._writer.Write(value.ToString(this.DateFormatString, (IFormatProvider) this.Culture));
        this._writer.Write(this._quoteChar);
      }
    }

    /// <summary>
    /// Writes a <see cref="T:System.Guid" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Guid" /> value to write.</param>
    public override void WriteValue(Guid value)
    {
      this.InternalWriteValue(JsonToken.String);
      string str = value.ToString("D");
      this._writer.Write(this._quoteChar);
      this._writer.Write(str);
      this._writer.Write(this._quoteChar);
    }

    /// <summary>
    /// Writes a <see cref="T:System.TimeSpan" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.TimeSpan" /> value to write.</param>
    public override void WriteValue(TimeSpan value)
    {
      this.InternalWriteValue(JsonToken.String);
      string str = value.ToString((string) null, (IFormatProvider) CultureInfo.InvariantCulture);
      this._writer.Write(this._quoteChar);
      this._writer.Write(str);
      this._writer.Write(this._quoteChar);
    }

    /// <summary>
    /// Writes a <see cref="T:System.Uri" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Uri" /> value to write.</param>
    public override void WriteValue(Uri value)
    {
      if (value == (Uri) null)
      {
        this.WriteNull();
      }
      else
      {
        this.InternalWriteValue(JsonToken.String);
        this.WriteEscapedString(value.OriginalString, true);
      }
    }

    /// <summary>
    /// Writes out a comment <code>/*...*/</code> containing the specified text.
    /// </summary>
    /// <param name="text">Text to place inside the comment.</param>
    public override void WriteComment(string text)
    {
      this.InternalWriteComment();
      this._writer.Write("/*");
      this._writer.Write(text);
      this._writer.Write("*/");
    }

    /// <summary>Writes out the given white space.</summary>
    /// <param name="ws">The string of white space characters.</param>
    public override void WriteWhitespace(string ws)
    {
      this.InternalWriteWhitespace(ws);
      this._writer.Write(ws);
    }

    private void EnsureWriteBuffer()
    {
      if (this._writeBuffer != null)
        return;
      this._writeBuffer = new char[35];
    }

    private void WriteIntegerValue(long value)
    {
      if (value >= 0L && value <= 9L)
      {
        this._writer.Write((char) (48UL + (ulong) value));
      }
      else
      {
        ulong uvalue = value < 0L ? (ulong) -value : (ulong) value;
        if (value < 0L)
          this._writer.Write('-');
        this.WriteIntegerValue(uvalue);
      }
    }

    private void WriteIntegerValue(ulong uvalue)
    {
      if (uvalue <= 9UL)
      {
        this._writer.Write((char) (48UL + uvalue));
      }
      else
      {
        this.EnsureWriteBuffer();
        int num = MathUtils.IntLength(uvalue);
        int count = 0;
        do
        {
          this._writeBuffer[num - ++count] = (char) (48UL + uvalue % 10UL);
          uvalue /= 10UL;
        }
        while (uvalue != 0UL);
        this._writer.Write(this._writeBuffer, 0, count);
      }
    }
  }
}
