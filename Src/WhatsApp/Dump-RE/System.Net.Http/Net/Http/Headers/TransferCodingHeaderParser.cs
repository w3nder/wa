// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.TransferCodingHeaderParser
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;

#nullable disable
namespace System.Net.Http.Headers
{
  internal class TransferCodingHeaderParser : BaseHeaderParser
  {
    private Func<TransferCodingHeaderValue> transferCodingCreator;
    internal static readonly TransferCodingHeaderParser SingleValueParser = new TransferCodingHeaderParser(false, new Func<TransferCodingHeaderValue>(TransferCodingHeaderParser.CreateTransferCoding));
    internal static readonly TransferCodingHeaderParser MultipleValueParser = new TransferCodingHeaderParser(true, new Func<TransferCodingHeaderValue>(TransferCodingHeaderParser.CreateTransferCoding));
    internal static readonly TransferCodingHeaderParser SingleValueWithQualityParser = new TransferCodingHeaderParser(false, new Func<TransferCodingHeaderValue>(TransferCodingHeaderParser.CreateTransferCodingWithQuality));
    internal static readonly TransferCodingHeaderParser MultipleValueWithQualityParser = new TransferCodingHeaderParser(true, new Func<TransferCodingHeaderValue>(TransferCodingHeaderParser.CreateTransferCodingWithQuality));

    private TransferCodingHeaderParser(
      bool supportsMultipleValues,
      Func<TransferCodingHeaderValue> transferCodingCreator)
      : base(supportsMultipleValues)
    {
      Contract.Requires(transferCodingCreator != null);
      this.transferCodingCreator = transferCodingCreator;
    }

    protected override int GetParsedValueLength(
      string value,
      int startIndex,
      object storeValue,
      out object parsedValue)
    {
      TransferCodingHeaderValue parsedValue1 = (TransferCodingHeaderValue) null;
      int transferCodingLength = TransferCodingHeaderValue.GetTransferCodingLength(value, startIndex, this.transferCodingCreator, out parsedValue1);
      parsedValue = (object) parsedValue1;
      return transferCodingLength;
    }

    private static TransferCodingHeaderValue CreateTransferCoding()
    {
      return new TransferCodingHeaderValue();
    }

    private static TransferCodingHeaderValue CreateTransferCodingWithQuality()
    {
      return (TransferCodingHeaderValue) new TransferCodingWithQualityHeaderValue();
    }
  }
}
