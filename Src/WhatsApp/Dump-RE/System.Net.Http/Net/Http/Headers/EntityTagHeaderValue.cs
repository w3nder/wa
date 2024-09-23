// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.EntityTagHeaderValue
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents an entity-tag header value.</summary>
  public class EntityTagHeaderValue : ICloneable
  {
    private static EntityTagHeaderValue any;
    private string tag;
    private bool isWeak;

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string Tag => this.tag;

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool IsWeak => this.isWeak;

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" />.</returns>
    public static EntityTagHeaderValue Any
    {
      get
      {
        if (EntityTagHeaderValue.any == null)
        {
          EntityTagHeaderValue.any = new EntityTagHeaderValue();
          EntityTagHeaderValue.any.tag = "*";
          EntityTagHeaderValue.any.isWeak = false;
        }
        return EntityTagHeaderValue.any;
      }
    }

    public EntityTagHeaderValue(string tag)
      : this(tag, false)
    {
    }

    public EntityTagHeaderValue(string tag, bool isWeak)
    {
      if (string.IsNullOrEmpty(tag))
        throw new ArgumentException(SR.net_http_argument_empty_string, nameof (tag));
      int length = 0;
      if (HttpRuleParser.GetQuotedStringLength(tag, 0, out length) != HttpParseResult.Parsed || length != tag.Length)
        throw new FormatException(SR.net_http_headers_invalid_etag_name);
      this.tag = tag;
      this.isWeak = isWeak;
    }

    private EntityTagHeaderValue(EntityTagHeaderValue source)
    {
      Contract.Requires(source != null);
      this.tag = source.tag;
      this.isWeak = source.isWeak;
    }

    private EntityTagHeaderValue()
    {
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString() => this.isWeak ? "W/" + this.tag : this.tag;

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public override bool Equals(object obj)
    {
      return obj is EntityTagHeaderValue entityTagHeaderValue && this.isWeak == entityTagHeaderValue.isWeak && string.CompareOrdinal(this.tag, entityTagHeaderValue.tag) == 0;
    }

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public override int GetHashCode() => this.tag.GetHashCode() ^ this.isWeak.GetHashCode();

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" />.</returns>
    public static EntityTagHeaderValue Parse(string input)
    {
      int index = 0;
      return (EntityTagHeaderValue) GenericHeaderParser.SingleValueEntityTagParser.ParseValue(input, (object) null, ref index);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool TryParse(string input, out EntityTagHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (EntityTagHeaderValue) null;
      object parsedValue1;
      if (!GenericHeaderParser.SingleValueEntityTagParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return false;
      parsedValue = (EntityTagHeaderValue) parsedValue1;
      return true;
    }

    internal static int GetEntityTagLength(
      string input,
      int startIndex,
      out EntityTagHeaderValue parsedValue)
    {
      Contract.Requires(startIndex >= 0);
      parsedValue = (EntityTagHeaderValue) null;
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      bool flag = false;
      int startIndex1 = startIndex;
      int startIndex2;
      switch (input[startIndex])
      {
        case '*':
          parsedValue = EntityTagHeaderValue.Any;
          startIndex2 = startIndex1 + 1;
          break;
        case 'W':
        case 'w':
          int index = startIndex1 + 1;
          if (index + 2 >= input.Length || input[index] != '/')
            return 0;
          flag = true;
          int startIndex3 = index + 1;
          startIndex1 = startIndex3 + HttpRuleParser.GetWhitespaceLength(input, startIndex3);
          goto default;
        default:
          int startIndex4 = startIndex1;
          int length = 0;
          if (HttpRuleParser.GetQuotedStringLength(input, startIndex1, out length) != HttpParseResult.Parsed)
            return 0;
          parsedValue = new EntityTagHeaderValue();
          if (length == input.Length)
          {
            Contract.Assert(startIndex == 0);
            Contract.Assert(!flag);
            parsedValue.tag = input;
            parsedValue.isWeak = false;
          }
          else
          {
            parsedValue.tag = input.Substring(startIndex4, length);
            parsedValue.isWeak = flag;
          }
          startIndex2 = startIndex1 + length;
          break;
      }
      return startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2) - startIndex;
    }

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone()
    {
      return this == EntityTagHeaderValue.any ? (object) EntityTagHeaderValue.any : (object) new EntityTagHeaderValue(this);
    }
  }
}
