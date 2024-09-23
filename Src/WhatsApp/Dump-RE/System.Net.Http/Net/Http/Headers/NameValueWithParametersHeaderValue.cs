// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.NameValueWithParametersHeaderValue
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;
using System.Diagnostics.Contracts;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents a name/value pair with parameters.</summary>
  public class NameValueWithParametersHeaderValue : NameValueHeaderValue, ICloneable
  {
    private static readonly Func<NameValueHeaderValue> nameValueCreator = new Func<NameValueHeaderValue>(NameValueWithParametersHeaderValue.CreateNameValue);
    private ICollection<NameValueHeaderValue> parameters;

    /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
    public ICollection<NameValueHeaderValue> Parameters
    {
      get
      {
        if (this.parameters == null)
          this.parameters = (ICollection<NameValueHeaderValue>) new ObjectCollection<NameValueHeaderValue>();
        return this.parameters;
      }
    }

    public NameValueWithParametersHeaderValue(string name)
      : base(name)
    {
    }

    public NameValueWithParametersHeaderValue(string name, string value)
      : base(name, value)
    {
    }

    internal NameValueWithParametersHeaderValue()
    {
    }

    protected NameValueWithParametersHeaderValue(NameValueWithParametersHeaderValue source)
      : base((NameValueHeaderValue) source)
    {
      if (source.parameters == null)
        return;
      foreach (ICloneable parameter in (IEnumerable<NameValueHeaderValue>) source.parameters)
        this.Parameters.Add((NameValueHeaderValue) parameter.Clone());
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public override bool Equals(object obj)
    {
      return base.Equals(obj) && obj is NameValueWithParametersHeaderValue parametersHeaderValue && HeaderUtilities.AreEqualCollections<NameValueHeaderValue>(this.parameters, parametersHeaderValue.parameters);
    }

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public override int GetHashCode()
    {
      return base.GetHashCode() ^ NameValueHeaderValue.GetHashCode(this.parameters);
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString()
    {
      return base.ToString() + NameValueHeaderValue.ToString(this.parameters, ';', true);
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.NameValueWithParametersHeaderValue" />.</returns>
    public static NameValueWithParametersHeaderValue Parse(string input)
    {
      int index = 0;
      return (NameValueWithParametersHeaderValue) GenericHeaderParser.SingleValueNameValueWithParametersParser.ParseValue(input, (object) null, ref index);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool TryParse(string input, out NameValueWithParametersHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (NameValueWithParametersHeaderValue) null;
      object parsedValue1;
      if (!GenericHeaderParser.SingleValueNameValueWithParametersParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return false;
      parsedValue = (NameValueWithParametersHeaderValue) parsedValue1;
      return true;
    }

    internal static int GetNameValueWithParametersLength(
      string input,
      int startIndex,
      out object parsedValue)
    {
      Contract.Requires(input != null);
      Contract.Requires(startIndex >= 0);
      parsedValue = (object) null;
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      NameValueHeaderValue parsedValue1 = (NameValueHeaderValue) null;
      int nameValueLength = NameValueHeaderValue.GetNameValueLength(input, startIndex, NameValueWithParametersHeaderValue.nameValueCreator, out parsedValue1);
      if (nameValueLength == 0)
        return 0;
      int startIndex1 = startIndex + nameValueLength;
      int index = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
      NameValueWithParametersHeaderValue parametersHeaderValue = parsedValue1 as NameValueWithParametersHeaderValue;
      Contract.Assert(parametersHeaderValue != null);
      if (index < input.Length && input[index] == ';')
      {
        int startIndex2 = index + 1;
        int nameValueListLength = NameValueHeaderValue.GetNameValueListLength(input, startIndex2, ';', parametersHeaderValue.Parameters);
        if (nameValueListLength == 0)
          return 0;
        parsedValue = (object) parametersHeaderValue;
        return startIndex2 + nameValueListLength - startIndex;
      }
      parsedValue = (object) parametersHeaderValue;
      return index - startIndex;
    }

    private static NameValueHeaderValue CreateNameValue()
    {
      return (NameValueHeaderValue) new NameValueWithParametersHeaderValue();
    }

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone() => (object) new NameValueWithParametersHeaderValue(this);
  }
}
