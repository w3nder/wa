// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.TraceJsonReader
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;
using System.Globalization;
using System.IO;

#nullable disable
namespace Newtonsoft.Json.Serialization
{
  internal class TraceJsonReader : JsonReader, IJsonLineInfo
  {
    private readonly JsonReader _innerReader;
    private readonly JsonTextWriter _textWriter;
    private readonly StringWriter _sw;

    public TraceJsonReader(JsonReader innerReader)
    {
      this._innerReader = innerReader;
      this._sw = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      this._sw.Write("Deserialized JSON: " + Environment.NewLine);
      this._textWriter = new JsonTextWriter((TextWriter) this._sw);
      this._textWriter.Formatting = Formatting.Indented;
    }

    public string GetDeserializedJsonMessage() => this._sw.ToString();

    public override bool Read()
    {
      bool flag = this._innerReader.Read();
      this._textWriter.WriteToken(this._innerReader, false, false);
      return flag;
    }

    public override int? ReadAsInt32()
    {
      int? nullable = this._innerReader.ReadAsInt32();
      this._textWriter.WriteToken(this._innerReader, false, false);
      return nullable;
    }

    public override string ReadAsString()
    {
      string str = this._innerReader.ReadAsString();
      this._textWriter.WriteToken(this._innerReader, false, false);
      return str;
    }

    public override byte[] ReadAsBytes()
    {
      byte[] numArray = this._innerReader.ReadAsBytes();
      this._textWriter.WriteToken(this._innerReader, false, false);
      return numArray;
    }

    public override Decimal? ReadAsDecimal()
    {
      Decimal? nullable = this._innerReader.ReadAsDecimal();
      this._textWriter.WriteToken(this._innerReader, false, false);
      return nullable;
    }

    public override DateTime? ReadAsDateTime()
    {
      DateTime? nullable = this._innerReader.ReadAsDateTime();
      this._textWriter.WriteToken(this._innerReader, false, false);
      return nullable;
    }

    public override DateTimeOffset? ReadAsDateTimeOffset()
    {
      DateTimeOffset? nullable = this._innerReader.ReadAsDateTimeOffset();
      this._textWriter.WriteToken(this._innerReader, false, false);
      return nullable;
    }

    public override int Depth => this._innerReader.Depth;

    public override string Path => this._innerReader.Path;

    public override char QuoteChar
    {
      get => this._innerReader.QuoteChar;
      protected internal set => this._innerReader.QuoteChar = value;
    }

    public override JsonToken TokenType => this._innerReader.TokenType;

    public override object Value => this._innerReader.Value;

    public override Type ValueType => this._innerReader.ValueType;

    public override void Close() => this._innerReader.Close();

    bool IJsonLineInfo.HasLineInfo()
    {
      return this._innerReader is IJsonLineInfo innerReader && innerReader.HasLineInfo();
    }

    int IJsonLineInfo.LineNumber
    {
      get => !(this._innerReader is IJsonLineInfo innerReader) ? 0 : innerReader.LineNumber;
    }

    int IJsonLineInfo.LinePosition
    {
      get => !(this._innerReader is IJsonLineInfo innerReader) ? 0 : innerReader.LinePosition;
    }
  }
}
