// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.JsonReader
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#nullable disable
namespace Newtonsoft.Json
{
  /// <summary>
  /// Represents a reader that provides fast, non-cached, forward-only access to serialized JSON data.
  /// </summary>
  public abstract class JsonReader : IDisposable
  {
    private JsonToken _tokenType;
    private object _value;
    internal char _quoteChar;
    internal JsonReader.State _currentState;
    internal ReadType _readType;
    private JsonPosition _currentPosition;
    private CultureInfo _culture;
    private DateTimeZoneHandling _dateTimeZoneHandling;
    private int? _maxDepth;
    private bool _hasExceededMaxDepth;
    internal DateParseHandling _dateParseHandling;
    internal FloatParseHandling _floatParseHandling;
    private string _dateFormatString;
    private readonly List<JsonPosition> _stack;

    /// <summary>Gets the current reader state.</summary>
    /// <value>The current reader state.</value>
    protected JsonReader.State CurrentState => this._currentState;

    /// <summary>
    /// Gets or sets a value indicating whether the underlying stream or
    /// <see cref="T:System.IO.TextReader" /> should be closed when the reader is closed.
    /// </summary>
    /// <value>
    /// true to close the underlying stream or <see cref="T:System.IO.TextReader" /> when
    /// the reader is closed; otherwise false. The default is true.
    /// </value>
    public bool CloseInput { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether multiple pieces of JSON content can
    /// be read from a continuous stream without erroring.
    /// </summary>
    /// <value>
    /// true to support reading multiple pieces of JSON content; otherwise false. The default is false.
    /// </value>
    public bool SupportMultipleContent { get; set; }

    /// <summary>
    /// Gets the quotation mark character used to enclose the value of a string.
    /// </summary>
    public virtual char QuoteChar
    {
      get => this._quoteChar;
      protected internal set => this._quoteChar = value;
    }

    /// <summary>
    /// Get or set how <see cref="T:System.DateTime" /> time zones are handling when reading JSON.
    /// </summary>
    public DateTimeZoneHandling DateTimeZoneHandling
    {
      get => this._dateTimeZoneHandling;
      set => this._dateTimeZoneHandling = value;
    }

    /// <summary>
    /// Get or set how date formatted strings, e.g. "\/Date(1198908717056)\/" and "2012-03-21T05:40Z", are parsed when reading JSON.
    /// </summary>
    public DateParseHandling DateParseHandling
    {
      get => this._dateParseHandling;
      set => this._dateParseHandling = value;
    }

    /// <summary>
    /// Get or set how floating point numbers, e.g. 1.0 and 9.9, are parsed when reading JSON text.
    /// </summary>
    public FloatParseHandling FloatParseHandling
    {
      get => this._floatParseHandling;
      set => this._floatParseHandling = value;
    }

    /// <summary>
    /// Get or set how custom date formatted strings are parsed when reading JSON.
    /// </summary>
    public string DateFormatString
    {
      get => this._dateFormatString;
      set => this._dateFormatString = value;
    }

    /// <summary>
    /// Gets or sets the maximum depth allowed when reading JSON. Reading past this depth will throw a <see cref="T:Newtonsoft.Json.JsonReaderException" />.
    /// </summary>
    public int? MaxDepth
    {
      get => this._maxDepth;
      set
      {
        int? nullable = value;
        if ((nullable.GetValueOrDefault() > 0 ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
          throw new ArgumentException("Value must be positive.", nameof (value));
        this._maxDepth = value;
      }
    }

    /// <summary>Gets the type of the current JSON token.</summary>
    public virtual JsonToken TokenType => this._tokenType;

    /// <summary>Gets the text value of the current JSON token.</summary>
    public virtual object Value => this._value;

    /// <summary>
    /// Gets The Common Language Runtime (CLR) type for the current JSON token.
    /// </summary>
    public virtual Type ValueType => this._value == null ? (Type) null : this._value.GetType();

    /// <summary>
    /// Gets the depth of the current token in the JSON document.
    /// </summary>
    /// <value>The depth of the current token in the JSON document.</value>
    public virtual int Depth
    {
      get
      {
        int count = this._stack.Count;
        return JsonTokenUtils.IsStartToken(this.TokenType) || this._currentPosition.Type == JsonContainerType.None ? count : count + 1;
      }
    }

    /// <summary>Gets the path of the current JSON token.</summary>
    public virtual string Path
    {
      get
      {
        if (this._currentPosition.Type == JsonContainerType.None)
          return string.Empty;
        IEnumerable<JsonPosition> positions;
        if (this._currentState != JsonReader.State.ArrayStart && this._currentState != JsonReader.State.ConstructorStart && this._currentState != JsonReader.State.ObjectStart)
          positions = this._stack.Concat<JsonPosition>((IEnumerable<JsonPosition>) new JsonPosition[1]
          {
            this._currentPosition
          });
        else
          positions = (IEnumerable<JsonPosition>) this._stack;
        return JsonPosition.BuildPath(positions);
      }
    }

    /// <summary>
    /// Gets or sets the culture used when reading JSON. Defaults to <see cref="P:System.Globalization.CultureInfo.InvariantCulture" />.
    /// </summary>
    public CultureInfo Culture
    {
      get => this._culture ?? CultureInfo.InvariantCulture;
      set => this._culture = value;
    }

    internal JsonPosition GetPosition(int depth)
    {
      return depth < this._stack.Count ? this._stack[depth] : this._currentPosition;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.JsonReader" /> class with the specified <see cref="T:System.IO.TextReader" />.
    /// </summary>
    protected JsonReader()
    {
      this._currentState = JsonReader.State.Start;
      this._stack = new List<JsonPosition>(4);
      this._dateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
      this._dateParseHandling = DateParseHandling.DateTime;
      this._floatParseHandling = FloatParseHandling.Double;
      this.CloseInput = true;
    }

    private void Push(JsonContainerType value)
    {
      this.UpdateScopeWithFinishedValue();
      if (this._currentPosition.Type == JsonContainerType.None)
      {
        this._currentPosition = new JsonPosition(value);
      }
      else
      {
        this._stack.Add(this._currentPosition);
        this._currentPosition = new JsonPosition(value);
        if (!this._maxDepth.HasValue)
          return;
        int num = this.Depth + 1;
        int? maxDepth = this._maxDepth;
        if ((num <= maxDepth.GetValueOrDefault() ? 0 : (maxDepth.HasValue ? 1 : 0)) != 0 && !this._hasExceededMaxDepth)
        {
          this._hasExceededMaxDepth = true;
          throw JsonReaderException.Create(this, "The reader's MaxDepth of {0} has been exceeded.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this._maxDepth));
        }
      }
    }

    private JsonContainerType Pop()
    {
      JsonPosition currentPosition;
      if (this._stack.Count > 0)
      {
        currentPosition = this._currentPosition;
        this._currentPosition = this._stack[this._stack.Count - 1];
        this._stack.RemoveAt(this._stack.Count - 1);
      }
      else
      {
        currentPosition = this._currentPosition;
        this._currentPosition = new JsonPosition();
      }
      if (this._maxDepth.HasValue)
      {
        int depth = this.Depth;
        int? maxDepth = this._maxDepth;
        if ((depth > maxDepth.GetValueOrDefault() ? 0 : (maxDepth.HasValue ? 1 : 0)) != 0)
          this._hasExceededMaxDepth = false;
      }
      return currentPosition.Type;
    }

    private JsonContainerType Peek() => this._currentPosition.Type;

    /// <summary>Reads the next JSON token from the stream.</summary>
    /// <returns>true if the next token was read successfully; false if there are no more tokens to read.</returns>
    public abstract bool Read();

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <returns>A <see cref="T:System.Nullable`1" />. This method will return <c>null</c> at the end of an array.</returns>
    public abstract int? ReadAsInt32();

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:System.String" />.
    /// </summary>
    /// <returns>A <see cref="T:System.String" />. This method will return <c>null</c> at the end of an array.</returns>
    public abstract string ReadAsString();

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:System.Byte" />[].
    /// </summary>
    /// <returns>A <see cref="T:System.Byte" />[] or a null reference if the next JSON token is null. This method will return <c>null</c> at the end of an array.</returns>
    public abstract byte[] ReadAsBytes();

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <returns>A <see cref="T:System.Nullable`1" />. This method will return <c>null</c> at the end of an array.</returns>
    public abstract Decimal? ReadAsDecimal();

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <returns>A <see cref="T:System.String" />. This method will return <c>null</c> at the end of an array.</returns>
    public abstract DateTime? ReadAsDateTime();

    /// <summary>
    /// Reads the next JSON token from the stream as a <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <returns>A <see cref="T:System.Nullable`1" />. This method will return <c>null</c> at the end of an array.</returns>
    public abstract DateTimeOffset? ReadAsDateTimeOffset();

    internal virtual bool ReadInternal() => throw new NotImplementedException();

    internal DateTimeOffset? ReadAsDateTimeOffsetInternal()
    {
      this._readType = ReadType.ReadAsDateTimeOffset;
      while (this.ReadInternal())
      {
        JsonToken tokenType = this.TokenType;
        switch (tokenType)
        {
          case JsonToken.Comment:
            continue;
          case JsonToken.String:
            string str = (string) this.Value;
            if (string.IsNullOrEmpty(str))
            {
              this.SetToken(JsonToken.Null);
              return new DateTimeOffset?();
            }
            object dt;
            if (DateTimeUtils.TryParseDateTime(str, DateParseHandling.DateTimeOffset, this.DateTimeZoneHandling, this._dateFormatString, this.Culture, out dt))
            {
              DateTimeOffset dateTimeOffset = (DateTimeOffset) dt;
              this.SetToken(JsonToken.Date, (object) dateTimeOffset, false);
              return new DateTimeOffset?(dateTimeOffset);
            }
            DateTimeOffset result;
            if (!DateTimeOffset.TryParse(str, (IFormatProvider) this.Culture, DateTimeStyles.RoundtripKind, out result))
              throw JsonReaderException.Create(this, "Could not convert string to DateTimeOffset: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, this.Value));
            this.SetToken(JsonToken.Date, (object) result, false);
            return new DateTimeOffset?(result);
          case JsonToken.Null:
            return new DateTimeOffset?();
          case JsonToken.EndArray:
            return new DateTimeOffset?();
          case JsonToken.Date:
            if (this.Value is DateTime)
              this.SetToken(JsonToken.Date, (object) new DateTimeOffset((DateTime) this.Value), false);
            return new DateTimeOffset?((DateTimeOffset) this.Value);
          default:
            throw JsonReaderException.Create(this, "Error reading date. Unexpected token: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) tokenType));
        }
      }
      this.SetToken(JsonToken.None);
      return new DateTimeOffset?();
    }

    internal byte[] ReadAsBytesInternal()
    {
      this._readType = ReadType.ReadAsBytes;
      while (this.ReadInternal())
      {
        JsonToken tokenType1 = this.TokenType;
        if (tokenType1 != JsonToken.Comment)
        {
          if (this.IsWrappedInTypeObject())
          {
            byte[] numArray = this.ReadAsBytes();
            this.ReadInternal();
            this.SetToken(JsonToken.Bytes, (object) numArray, false);
            return numArray;
          }
          switch (tokenType1)
          {
            case JsonToken.StartArray:
              List<byte> byteList = new List<byte>();
              while (this.ReadInternal())
              {
                JsonToken tokenType2 = this.TokenType;
                switch (tokenType2)
                {
                  case JsonToken.Comment:
                    continue;
                  case JsonToken.Integer:
                    byteList.Add(Convert.ToByte(this.Value, (IFormatProvider) CultureInfo.InvariantCulture));
                    continue;
                  case JsonToken.EndArray:
                    byte[] array = byteList.ToArray();
                    this.SetToken(JsonToken.Bytes, (object) array, false);
                    return array;
                  default:
                    throw JsonReaderException.Create(this, "Unexpected token when reading bytes: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) tokenType2));
                }
              }
              throw JsonReaderException.Create(this, "Unexpected end when reading bytes.");
            case JsonToken.String:
              string s = (string) this.Value;
              Guid g;
              byte[] numArray1 = s.Length != 0 ? (!ConvertUtils.TryConvertGuid(s, out g) ? Convert.FromBase64String(s) : g.ToByteArray()) : new byte[0];
              this.SetToken(JsonToken.Bytes, (object) numArray1, false);
              return numArray1;
            case JsonToken.Null:
              return (byte[]) null;
            case JsonToken.EndArray:
              return (byte[]) null;
            case JsonToken.Bytes:
              if (this.ValueType != typeof (Guid))
                return (byte[]) this.Value;
              byte[] byteArray = ((Guid) this.Value).ToByteArray();
              this.SetToken(JsonToken.Bytes, (object) byteArray, false);
              return byteArray;
            default:
              throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) tokenType1));
          }
        }
      }
      this.SetToken(JsonToken.None);
      return (byte[]) null;
    }

    internal Decimal? ReadAsDecimalInternal()
    {
      this._readType = ReadType.ReadAsDecimal;
      while (this.ReadInternal())
      {
        JsonToken tokenType = this.TokenType;
        switch (tokenType)
        {
          case JsonToken.Comment:
            continue;
          case JsonToken.Integer:
          case JsonToken.Float:
            if (!(this.Value is Decimal))
              this.SetToken(JsonToken.Float, (object) Convert.ToDecimal(this.Value, (IFormatProvider) CultureInfo.InvariantCulture), false);
            return new Decimal?((Decimal) this.Value);
          case JsonToken.String:
            string s = (string) this.Value;
            if (string.IsNullOrEmpty(s))
            {
              this.SetToken(JsonToken.Null);
              return new Decimal?();
            }
            Decimal result;
            if (!Decimal.TryParse(s, NumberStyles.Number, (IFormatProvider) this.Culture, out result))
              throw JsonReaderException.Create(this, "Could not convert string to decimal: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, this.Value));
            this.SetToken(JsonToken.Float, (object) result, false);
            return new Decimal?(result);
          case JsonToken.Null:
            return new Decimal?();
          case JsonToken.EndArray:
            return new Decimal?();
          default:
            throw JsonReaderException.Create(this, "Error reading decimal. Unexpected token: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) tokenType));
        }
      }
      this.SetToken(JsonToken.None);
      return new Decimal?();
    }

    internal int? ReadAsInt32Internal()
    {
      this._readType = ReadType.ReadAsInt32;
      while (this.ReadInternal())
      {
        switch (this.TokenType)
        {
          case JsonToken.Comment:
            continue;
          case JsonToken.Integer:
          case JsonToken.Float:
            if (!(this.Value is int))
              this.SetToken(JsonToken.Integer, (object) Convert.ToInt32(this.Value, (IFormatProvider) CultureInfo.InvariantCulture), false);
            return new int?((int) this.Value);
          case JsonToken.String:
            string s = (string) this.Value;
            if (string.IsNullOrEmpty(s))
            {
              this.SetToken(JsonToken.Null);
              return new int?();
            }
            int result;
            if (!int.TryParse(s, NumberStyles.Integer, (IFormatProvider) this.Culture, out result))
              throw JsonReaderException.Create(this, "Could not convert string to integer: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, this.Value));
            this.SetToken(JsonToken.Integer, (object) result, false);
            return new int?(result);
          case JsonToken.Null:
            return new int?();
          case JsonToken.EndArray:
            return new int?();
          default:
            throw JsonReaderException.Create(this, "Error reading integer. Unexpected token: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this.TokenType));
        }
      }
      this.SetToken(JsonToken.None);
      return new int?();
    }

    internal string ReadAsStringInternal()
    {
      this._readType = ReadType.ReadAsString;
      while (this.ReadInternal())
      {
        JsonToken tokenType = this.TokenType;
        switch (tokenType)
        {
          case JsonToken.Comment:
            continue;
          case JsonToken.String:
            return (string) this.Value;
          case JsonToken.Null:
            return (string) null;
          default:
            if (JsonTokenUtils.IsPrimitiveToken(tokenType) && this.Value != null)
            {
              string str = !(this.Value is IFormattable) ? this.Value.ToString() : ((IFormattable) this.Value).ToString((string) null, (IFormatProvider) this.Culture);
              this.SetToken(JsonToken.String, (object) str, false);
              return str;
            }
            if (tokenType == JsonToken.EndArray)
              return (string) null;
            throw JsonReaderException.Create(this, "Error reading string. Unexpected token: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) tokenType));
        }
      }
      this.SetToken(JsonToken.None);
      return (string) null;
    }

    internal DateTime? ReadAsDateTimeInternal()
    {
      this._readType = ReadType.ReadAsDateTime;
      while (this.ReadInternal())
      {
        if (this.TokenType != JsonToken.Comment)
        {
          if (this.TokenType == JsonToken.Date)
            return new DateTime?((DateTime) this.Value);
          if (this.TokenType == JsonToken.Null)
            return new DateTime?();
          if (this.TokenType == JsonToken.String)
          {
            string s = (string) this.Value;
            if (string.IsNullOrEmpty(s))
            {
              this.SetToken(JsonToken.Null);
              return new DateTime?();
            }
            object dt;
            if (DateTimeUtils.TryParseDateTime(s, DateParseHandling.DateTime, this.DateTimeZoneHandling, this._dateFormatString, this.Culture, out dt))
            {
              DateTime dateTime = DateTimeUtils.EnsureDateTime((DateTime) dt, this.DateTimeZoneHandling);
              this.SetToken(JsonToken.Date, (object) dateTime, false);
              return new DateTime?(dateTime);
            }
            DateTime result;
            if (!DateTime.TryParse(s, (IFormatProvider) this.Culture, DateTimeStyles.RoundtripKind, out result))
              throw JsonReaderException.Create(this, "Could not convert string to DateTime: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, this.Value));
            DateTime dateTime1 = DateTimeUtils.EnsureDateTime(result, this.DateTimeZoneHandling);
            this.SetToken(JsonToken.Date, (object) dateTime1, false);
            return new DateTime?(dateTime1);
          }
          if (this.TokenType == JsonToken.EndArray)
            return new DateTime?();
          throw JsonReaderException.Create(this, "Error reading date. Unexpected token: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this.TokenType));
        }
      }
      this.SetToken(JsonToken.None);
      return new DateTime?();
    }

    private bool IsWrappedInTypeObject()
    {
      this._readType = ReadType.Read;
      if (this.TokenType != JsonToken.StartObject)
        return false;
      if (!this.ReadInternal())
        throw JsonReaderException.Create(this, "Unexpected end when reading bytes.");
      if (this.Value.ToString() == "$type")
      {
        this.ReadInternal();
        if (this.Value != null && this.Value.ToString().StartsWith("System.Byte[]", StringComparison.Ordinal))
        {
          this.ReadInternal();
          if (this.Value.ToString() == "$value")
            return true;
        }
      }
      throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) JsonToken.StartObject));
    }

    /// <summary>Skips the children of the current token.</summary>
    public void Skip()
    {
      if (this.TokenType == JsonToken.PropertyName)
        this.Read();
      if (!JsonTokenUtils.IsStartToken(this.TokenType))
        return;
      int depth = this.Depth;
      do
        ;
      while (this.Read() && depth < this.Depth);
    }

    /// <summary>Sets the current token.</summary>
    /// <param name="newToken">The new token.</param>
    protected void SetToken(JsonToken newToken) => this.SetToken(newToken, (object) null, true);

    /// <summary>Sets the current token and value.</summary>
    /// <param name="newToken">The new token.</param>
    /// <param name="value">The value.</param>
    protected void SetToken(JsonToken newToken, object value)
    {
      this.SetToken(newToken, value, true);
    }

    internal void SetToken(JsonToken newToken, object value, bool updateIndex)
    {
      this._tokenType = newToken;
      this._value = value;
      switch (newToken)
      {
        case JsonToken.StartObject:
          this._currentState = JsonReader.State.ObjectStart;
          this.Push(JsonContainerType.Object);
          break;
        case JsonToken.StartArray:
          this._currentState = JsonReader.State.ArrayStart;
          this.Push(JsonContainerType.Array);
          break;
        case JsonToken.StartConstructor:
          this._currentState = JsonReader.State.ConstructorStart;
          this.Push(JsonContainerType.Constructor);
          break;
        case JsonToken.PropertyName:
          this._currentState = JsonReader.State.Property;
          this._currentPosition.PropertyName = (string) value;
          break;
        case JsonToken.Raw:
        case JsonToken.Integer:
        case JsonToken.Float:
        case JsonToken.String:
        case JsonToken.Boolean:
        case JsonToken.Null:
        case JsonToken.Undefined:
        case JsonToken.Date:
        case JsonToken.Bytes:
          this.SetPostValueState(updateIndex);
          break;
        case JsonToken.EndObject:
          this.ValidateEnd(JsonToken.EndObject);
          break;
        case JsonToken.EndArray:
          this.ValidateEnd(JsonToken.EndArray);
          break;
        case JsonToken.EndConstructor:
          this.ValidateEnd(JsonToken.EndConstructor);
          break;
      }
    }

    internal void SetPostValueState(bool updateIndex)
    {
      if (this.Peek() != JsonContainerType.None)
        this._currentState = JsonReader.State.PostValue;
      else
        this.SetFinished();
      if (!updateIndex)
        return;
      this.UpdateScopeWithFinishedValue();
    }

    private void UpdateScopeWithFinishedValue()
    {
      if (!this._currentPosition.HasIndex)
        return;
      ++this._currentPosition.Position;
    }

    private void ValidateEnd(JsonToken endToken)
    {
      JsonContainerType jsonContainerType = this.Pop();
      if (this.GetTypeForCloseToken(endToken) != jsonContainerType)
        throw JsonReaderException.Create(this, "JsonToken {0} is not valid for closing JsonType {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) endToken, (object) jsonContainerType));
      if (this.Peek() != JsonContainerType.None)
        this._currentState = JsonReader.State.PostValue;
      else
        this.SetFinished();
    }

    /// <summary>Sets the state based on current token type.</summary>
    protected void SetStateBasedOnCurrent()
    {
      JsonContainerType jsonContainerType = this.Peek();
      switch (jsonContainerType)
      {
        case JsonContainerType.None:
          this.SetFinished();
          break;
        case JsonContainerType.Object:
          this._currentState = JsonReader.State.Object;
          break;
        case JsonContainerType.Array:
          this._currentState = JsonReader.State.Array;
          break;
        case JsonContainerType.Constructor:
          this._currentState = JsonReader.State.Constructor;
          break;
        default:
          throw JsonReaderException.Create(this, "While setting the reader state back to current object an unexpected JsonType was encountered: {0}".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) jsonContainerType));
      }
    }

    private void SetFinished()
    {
      if (this.SupportMultipleContent)
        this._currentState = JsonReader.State.Start;
      else
        this._currentState = JsonReader.State.Finished;
    }

    private JsonContainerType GetTypeForCloseToken(JsonToken token)
    {
      switch (token)
      {
        case JsonToken.EndObject:
          return JsonContainerType.Object;
        case JsonToken.EndArray:
          return JsonContainerType.Array;
        case JsonToken.EndConstructor:
          return JsonContainerType.Constructor;
        default:
          throw JsonReaderException.Create(this, "Not a valid close JsonToken: {0}".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) token));
      }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    void IDisposable.Dispose() => this.Dispose(true);

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (this._currentState == JsonReader.State.Closed || !disposing)
        return;
      this.Close();
    }

    /// <summary>
    /// Changes the <see cref="T:Newtonsoft.Json.JsonReader.State" /> to Closed.
    /// </summary>
    public virtual void Close()
    {
      this._currentState = JsonReader.State.Closed;
      this._tokenType = JsonToken.None;
      this._value = (object) null;
    }

    /// <summary>Specifies the state of the reader.</summary>
    protected internal enum State
    {
      /// <summary>The Read method has not been called.</summary>
      Start,
      /// <summary>The end of the file has been reached successfully.</summary>
      Complete,
      /// <summary>Reader is at a property.</summary>
      Property,
      /// <summary>Reader is at the start of an object.</summary>
      ObjectStart,
      /// <summary>Reader is in an object.</summary>
      Object,
      /// <summary>Reader is at the start of an array.</summary>
      ArrayStart,
      /// <summary>Reader is in an array.</summary>
      Array,
      /// <summary>The Close method has been called.</summary>
      Closed,
      /// <summary>Reader has just read a value.</summary>
      PostValue,
      /// <summary>Reader is at the start of a constructor.</summary>
      ConstructorStart,
      /// <summary>Reader in a constructor.</summary>
      Constructor,
      /// <summary>
      /// An error occurred that prevents the read operation from continuing.
      /// </summary>
      Error,
      /// <summary>The end of the file has been reached successfully.</summary>
      Finished,
    }
  }
}
