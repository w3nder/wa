// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.ExpandedProductParsedResult
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary>
  /// 
  /// </summary>
  /// <author> Antonio Manuel Benjumea Conde, Servinform, S.A.</author>
  /// <author> Agustín Delgado, Servinform, S.A.</author>
  public class ExpandedProductParsedResult : ParsedResult
  {
    public static string KILOGRAM = "KG";
    public static string POUND = "LB";
    private readonly string rawText;
    private readonly string productID;
    private readonly string sscc;
    private readonly string lotNumber;
    private readonly string productionDate;
    private readonly string packagingDate;
    private readonly string bestBeforeDate;
    private readonly string expirationDate;
    private readonly string weight;
    private readonly string weightType;
    private readonly string weightIncrement;
    private readonly string price;
    private readonly string priceIncrement;
    private readonly string priceCurrency;
    private readonly IDictionary<string, string> uncommonAIs;

    public ExpandedProductParsedResult(
      string rawText,
      string productID,
      string sscc,
      string lotNumber,
      string productionDate,
      string packagingDate,
      string bestBeforeDate,
      string expirationDate,
      string weight,
      string weightType,
      string weightIncrement,
      string price,
      string priceIncrement,
      string priceCurrency,
      IDictionary<string, string> uncommonAIs)
      : base(ParsedResultType.PRODUCT)
    {
      this.rawText = rawText;
      this.productID = productID;
      this.sscc = sscc;
      this.lotNumber = lotNumber;
      this.productionDate = productionDate;
      this.packagingDate = packagingDate;
      this.bestBeforeDate = bestBeforeDate;
      this.expirationDate = expirationDate;
      this.weight = weight;
      this.weightType = weightType;
      this.weightIncrement = weightIncrement;
      this.price = price;
      this.priceIncrement = priceIncrement;
      this.priceCurrency = priceCurrency;
      this.uncommonAIs = uncommonAIs;
      this.displayResultValue = productID;
    }

    public override bool Equals(object o)
    {
      if (!(o is ExpandedProductParsedResult))
        return false;
      ExpandedProductParsedResult productParsedResult = (ExpandedProductParsedResult) o;
      return ExpandedProductParsedResult.equalsOrNull((object) this.productID, (object) productParsedResult.productID) && ExpandedProductParsedResult.equalsOrNull((object) this.sscc, (object) productParsedResult.sscc) && ExpandedProductParsedResult.equalsOrNull((object) this.lotNumber, (object) productParsedResult.lotNumber) && ExpandedProductParsedResult.equalsOrNull((object) this.productionDate, (object) productParsedResult.productionDate) && ExpandedProductParsedResult.equalsOrNull((object) this.bestBeforeDate, (object) productParsedResult.bestBeforeDate) && ExpandedProductParsedResult.equalsOrNull((object) this.expirationDate, (object) productParsedResult.expirationDate) && ExpandedProductParsedResult.equalsOrNull((object) this.weight, (object) productParsedResult.weight) && ExpandedProductParsedResult.equalsOrNull((object) this.weightType, (object) productParsedResult.weightType) && ExpandedProductParsedResult.equalsOrNull((object) this.weightIncrement, (object) productParsedResult.weightIncrement) && ExpandedProductParsedResult.equalsOrNull((object) this.price, (object) productParsedResult.price) && ExpandedProductParsedResult.equalsOrNull((object) this.priceIncrement, (object) productParsedResult.priceIncrement) && ExpandedProductParsedResult.equalsOrNull((object) this.priceCurrency, (object) productParsedResult.priceCurrency) && ExpandedProductParsedResult.equalsOrNull(this.uncommonAIs, productParsedResult.uncommonAIs);
    }

    private static bool equalsOrNull(object o1, object o2)
    {
      return o1 != null ? o1.Equals(o2) : o2 == null;
    }

    private static bool equalsOrNull(IDictionary<string, string> o1, IDictionary<string, string> o2)
    {
      if (o1 == null)
        return o2 == null;
      if (o1.Count != o2.Count)
        return false;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) o1)
      {
        if (!o2.ContainsKey(keyValuePair.Key) || !keyValuePair.Value.Equals(o2[keyValuePair.Key]))
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      return 0 ^ ExpandedProductParsedResult.hashNotNull((object) this.productID) ^ ExpandedProductParsedResult.hashNotNull((object) this.sscc) ^ ExpandedProductParsedResult.hashNotNull((object) this.lotNumber) ^ ExpandedProductParsedResult.hashNotNull((object) this.productionDate) ^ ExpandedProductParsedResult.hashNotNull((object) this.bestBeforeDate) ^ ExpandedProductParsedResult.hashNotNull((object) this.expirationDate) ^ ExpandedProductParsedResult.hashNotNull((object) this.weight) ^ ExpandedProductParsedResult.hashNotNull((object) this.weightType) ^ ExpandedProductParsedResult.hashNotNull((object) this.weightIncrement) ^ ExpandedProductParsedResult.hashNotNull((object) this.price) ^ ExpandedProductParsedResult.hashNotNull((object) this.priceIncrement) ^ ExpandedProductParsedResult.hashNotNull((object) this.priceCurrency) ^ ExpandedProductParsedResult.hashNotNull((object) this.uncommonAIs);
    }

    private static int hashNotNull(object o) => o != null ? o.GetHashCode() : 0;

    public string RawText => this.rawText;

    public string ProductID => this.productID;

    public string Sscc => this.sscc;

    public string LotNumber => this.lotNumber;

    public string ProductionDate => this.productionDate;

    public string PackagingDate => this.packagingDate;

    public string BestBeforeDate => this.bestBeforeDate;

    public string ExpirationDate => this.expirationDate;

    public string Weight => this.weight;

    public string WeightType => this.weightType;

    public string WeightIncrement => this.weightIncrement;

    public string Price => this.price;

    public string PriceIncrement => this.priceIncrement;

    public string PriceCurrency => this.priceCurrency;

    public IDictionary<string, string> UncommonAIs => this.uncommonAIs;

    public override string DisplayResult => this.rawText;
  }
}
