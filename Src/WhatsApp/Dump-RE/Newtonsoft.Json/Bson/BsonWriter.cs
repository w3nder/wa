// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Bson.BsonWriter
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;
using System.IO;

#nullable disable
namespace Newtonsoft.Json.Bson
{
  /// <summary>
  /// Represents a writer that provides a fast, non-cached, forward-only way of generating JSON data.
  /// </summary>
  public class BsonWriter : JsonWriter
  {
    private readonly BsonBinaryWriter _writer;
    private BsonToken _root;
    private BsonToken _parent;
    private string _propertyName;

    /// <summary>
    /// Gets or sets the <see cref="T:System.DateTimeKind" /> used when writing <see cref="T:System.DateTime" /> values to BSON.
    /// When set to <see cref="F:System.DateTimeKind.Unspecified" /> no conversion will occur.
    /// </summary>
    /// <value>The <see cref="T:System.DateTimeKind" /> used when writing <see cref="T:System.DateTime" /> values to BSON.</value>
    public DateTimeKind DateTimeKindHandling
    {
      get => this._writer.DateTimeKindHandling;
      set => this._writer.DateTimeKindHandling = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Bson.BsonWriter" /> class.
    /// </summary>
    /// <param name="stream">The stream.</param>
    public BsonWriter(Stream stream)
    {
      ValidationUtils.ArgumentNotNull((object) stream, nameof (stream));
      this._writer = new BsonBinaryWriter(new BinaryWriter(stream));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Bson.BsonWriter" /> class.
    /// </summary>
    /// <param name="writer">The writer.</param>
    public BsonWriter(BinaryWriter writer)
    {
      ValidationUtils.ArgumentNotNull((object) writer, nameof (writer));
      this._writer = new BsonBinaryWriter(writer);
    }

    /// <summary>
    /// Flushes whatever is in the buffer to the underlying streams and also flushes the underlying stream.
    /// </summary>
    public override void Flush() => this._writer.Flush();

    /// <summary>Writes the end.</summary>
    /// <param name="token">The token.</param>
    protected override void WriteEnd(JsonToken token)
    {
      base.WriteEnd(token);
      this.RemoveParent();
      if (this.Top != 0)
        return;
      this._writer.WriteToken(this._root);
    }

    /// <summary>
    /// Writes out a comment <code>/*...*/</code> containing the specified text.
    /// </summary>
    /// <param name="text">Text to place inside the comment.</param>
    public override void WriteComment(string text)
    {
      throw JsonWriterException.Create((JsonWriter) this, "Cannot write JSON comment as BSON.", (Exception) null);
    }

    /// <summary>
    /// Writes the start of a constructor with the given name.
    /// </summary>
    /// <param name="name">The name of the constructor.</param>
    public override void WriteStartConstructor(string name)
    {
      throw JsonWriterException.Create((JsonWriter) this, "Cannot write JSON constructor as BSON.", (Exception) null);
    }

    /// <summary>Writes raw JSON.</summary>
    /// <param name="json">The raw JSON to write.</param>
    public override void WriteRaw(string json)
    {
      throw JsonWriterException.Create((JsonWriter) this, "Cannot write raw JSON as BSON.", (Exception) null);
    }

    /// <summary>
    /// Writes raw JSON where a value is expected and updates the writer's state.
    /// </summary>
    /// <param name="json">The raw JSON to write.</param>
    public override void WriteRawValue(string json)
    {
      throw JsonWriterException.Create((JsonWriter) this, "Cannot write raw JSON as BSON.", (Exception) null);
    }

    /// <summary>Writes the beginning of a JSON array.</summary>
    public override void WriteStartArray()
    {
      base.WriteStartArray();
      this.AddParent((BsonToken) new BsonArray());
    }

    /// <summary>Writes the beginning of a JSON object.</summary>
    public override void WriteStartObject()
    {
      base.WriteStartObject();
      this.AddParent((BsonToken) new BsonObject());
    }

    /// <summary>
    /// Writes the property name of a name/value pair on a JSON object.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    public override void WritePropertyName(string name)
    {
      base.WritePropertyName(name);
      this._propertyName = name;
    }

    /// <summary>Closes this stream and the underlying stream.</summary>
    public override void Close()
    {
      base.Close();
      if (!this.CloseOutput || this._writer == null)
        return;
      this._writer.Close();
    }

    private void AddParent(BsonToken container)
    {
      this.AddToken(container);
      this._parent = container;
    }

    private void RemoveParent() => this._parent = this._parent.Parent;

    private void AddValue(object value, BsonType type)
    {
      this.AddToken((BsonToken) new BsonValue(value, type));
    }

    internal void AddToken(BsonToken token)
    {
      if (this._parent != null)
      {
        if (this._parent is BsonObject)
        {
          ((BsonObject) this._parent).Add(this._propertyName, token);
          this._propertyName = (string) null;
        }
        else
          ((BsonArray) this._parent).Add(token);
      }
      else
      {
        this._parent = token.Type == BsonType.Object || token.Type == BsonType.Array ? token : throw JsonWriterException.Create((JsonWriter) this, "Error writing {0} value. BSON must start with an Object or Array.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) token.Type), (Exception) null);
        this._root = token;
      }
    }

    /// <summary>
    /// Writes a <see cref="T:System.Object" /> value.
    /// An error will raised if the value cannot be written as a single JSON token.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Object" /> value to write.</param>
    public override void WriteValue(object value) => base.WriteValue(value);

    /// <summary>Writes a null value.</summary>
    public override void WriteNull()
    {
      base.WriteNull();
      this.AddValue((object) null, BsonType.Null);
    }

    /// <summary>Writes an undefined value.</summary>
    public override void WriteUndefined()
    {
      base.WriteUndefined();
      this.AddValue((object) null, BsonType.Undefined);
    }

    /// <summary>
    /// Writes a <see cref="T:System.String" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.String" /> value to write.</param>
    public override void WriteValue(string value)
    {
      base.WriteValue(value);
      if (value == null)
        this.AddValue((object) null, BsonType.Null);
      else
        this.AddToken((BsonToken) new BsonString((object) value, true));
    }

    /// <summary>
    /// Writes a <see cref="T:System.Int32" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Int32" /> value to write.</param>
    public override void WriteValue(int value)
    {
      base.WriteValue(value);
      this.AddValue((object) value, BsonType.Integer);
    }

    /// <summary>
    /// Writes a <see cref="T:System.UInt32" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.UInt32" /> value to write.</param>
    [CLSCompliant(false)]
    public override void WriteValue(uint value)
    {
      if (value > (uint) int.MaxValue)
        throw JsonWriterException.Create((JsonWriter) this, "Value is too large to fit in a signed 32 bit integer. BSON does not support unsigned values.", (Exception) null);
      base.WriteValue(value);
      this.AddValue((object) value, BsonType.Integer);
    }

    /// <summary>
    /// Writes a <see cref="T:System.Int64" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Int64" /> value to write.</param>
    public override void WriteValue(long value)
    {
      base.WriteValue(value);
      this.AddValue((object) value, BsonType.Long);
    }

    /// <summary>
    /// Writes a <see cref="T:System.UInt64" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.UInt64" /> value to write.</param>
    [CLSCompliant(false)]
    public override void WriteValue(ulong value)
    {
      if (value > (ulong) long.MaxValue)
        throw JsonWriterException.Create((JsonWriter) this, "Value is too large to fit in a signed 64 bit integer. BSON does not support unsigned values.", (Exception) null);
      base.WriteValue(value);
      this.AddValue((object) value, BsonType.Long);
    }

    /// <summary>
    /// Writes a <see cref="T:System.Single" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Single" /> value to write.</param>
    public override void WriteValue(float value)
    {
      base.WriteValue(value);
      this.AddValue((object) value, BsonType.Number);
    }

    /// <summary>
    /// Writes a <see cref="T:System.Double" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Double" /> value to write.</param>
    public override void WriteValue(double value)
    {
      base.WriteValue(value);
      this.AddValue((object) value, BsonType.Number);
    }

    /// <summary>
    /// Writes a <see cref="T:System.Boolean" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Boolean" /> value to write.</param>
    public override void WriteValue(bool value)
    {
      base.WriteValue(value);
      this.AddValue((object) value, BsonType.Boolean);
    }

    /// <summary>
    /// Writes a <see cref="T:System.Int16" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Int16" /> value to write.</param>
    public override void WriteValue(short value)
    {
      base.WriteValue(value);
      this.AddValue((object) value, BsonType.Integer);
    }

    /// <summary>
    /// Writes a <see cref="T:System.UInt16" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.UInt16" /> value to write.</param>
    [CLSCompliant(false)]
    public override void WriteValue(ushort value)
    {
      base.WriteValue(value);
      this.AddValue((object) value, BsonType.Integer);
    }

    /// <summary>
    /// Writes a <see cref="T:System.Char" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Char" /> value to write.</param>
    public override void WriteValue(char value)
    {
      base.WriteValue(value);
      this.AddToken((BsonToken) new BsonString((object) value.ToString(), true));
    }

    /// <summary>
    /// Writes a <see cref="T:System.Byte" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Byte" /> value to write.</param>
    public override void WriteValue(byte value)
    {
      base.WriteValue(value);
      this.AddValue((object) value, BsonType.Integer);
    }

    /// <summary>
    /// Writes a <see cref="T:System.SByte" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.SByte" /> value to write.</param>
    [CLSCompliant(false)]
    public override void WriteValue(sbyte value)
    {
      base.WriteValue(value);
      this.AddValue((object) value, BsonType.Integer);
    }

    /// <summary>
    /// Writes a <see cref="T:System.Decimal" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Decimal" /> value to write.</param>
    public override void WriteValue(Decimal value)
    {
      base.WriteValue(value);
      this.AddValue((object) value, BsonType.Number);
    }

    /// <summary>
    /// Writes a <see cref="T:System.DateTime" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.DateTime" /> value to write.</param>
    public override void WriteValue(DateTime value)
    {
      base.WriteValue(value);
      value = DateTimeUtils.EnsureDateTime(value, this.DateTimeZoneHandling);
      this.AddValue((object) value, BsonType.Date);
    }

    /// <summary>
    /// Writes a <see cref="T:System.DateTimeOffset" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.DateTimeOffset" /> value to write.</param>
    public override void WriteValue(DateTimeOffset value)
    {
      base.WriteValue(value);
      this.AddValue((object) value, BsonType.Date);
    }

    /// <summary>
    /// Writes a <see cref="T:System.Byte" />[] value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Byte" />[] value to write.</param>
    public override void WriteValue(byte[] value)
    {
      base.WriteValue(value);
      this.AddToken((BsonToken) new BsonBinary(value, BsonBinaryType.Binary));
    }

    /// <summary>
    /// Writes a <see cref="T:System.Guid" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Guid" /> value to write.</param>
    public override void WriteValue(Guid value)
    {
      base.WriteValue(value);
      this.AddToken((BsonToken) new BsonBinary(value.ToByteArray(), BsonBinaryType.Uuid));
    }

    /// <summary>
    /// Writes a <see cref="T:System.TimeSpan" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.TimeSpan" /> value to write.</param>
    public override void WriteValue(TimeSpan value)
    {
      base.WriteValue(value);
      this.AddToken((BsonToken) new BsonString((object) value.ToString(), true));
    }

    /// <summary>
    /// Writes a <see cref="T:System.Uri" /> value.
    /// </summary>
    /// <param name="value">The <see cref="T:System.Uri" /> value to write.</param>
    public override void WriteValue(Uri value)
    {
      base.WriteValue(value);
      this.AddToken((BsonToken) new BsonString((object) value.ToString(), true));
    }

    /// <summary>
    /// Writes a <see cref="T:System.Byte" />[] value that represents a BSON object id.
    /// </summary>
    /// <param name="value">The Object ID value to write.</param>
    public void WriteObjectId(byte[] value)
    {
      ValidationUtils.ArgumentNotNull((object) value, nameof (value));
      if (value.Length != 12)
        throw JsonWriterException.Create((JsonWriter) this, "An object id must be 12 bytes", (Exception) null);
      this.UpdateScopeWithFinishedValue();
      this.AutoComplete(JsonToken.Undefined);
      this.AddValue((object) value, BsonType.Oid);
    }

    /// <summary>Writes a BSON regex.</summary>
    /// <param name="pattern">The regex pattern.</param>
    /// <param name="options">The regex options.</param>
    public void WriteRegex(string pattern, string options)
    {
      ValidationUtils.ArgumentNotNull((object) pattern, nameof (pattern));
      this.UpdateScopeWithFinishedValue();
      this.AutoComplete(JsonToken.Undefined);
      this.AddToken((BsonToken) new BsonRegex(pattern, options));
    }
  }
}
