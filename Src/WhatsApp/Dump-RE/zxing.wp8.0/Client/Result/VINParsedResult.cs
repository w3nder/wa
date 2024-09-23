// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.VINParsedResult
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Text;

#nullable disable
namespace ZXing.Client.Result
{
  public class VINParsedResult : ParsedResult
  {
    public string VIN { get; private set; }

    public string WorldManufacturerID { get; private set; }

    public string VehicleDescriptorSection { get; private set; }

    public string VehicleIdentifierSection { get; private set; }

    public string CountryCode { get; private set; }

    public string VehicleAttributes { get; private set; }

    public int ModelYear { get; private set; }

    public char PlantCode { get; private set; }

    public string SequentialNumber { get; private set; }

    public VINParsedResult(
      string vin,
      string worldManufacturerID,
      string vehicleDescriptorSection,
      string vehicleIdentifierSection,
      string countryCode,
      string vehicleAttributes,
      int modelYear,
      char plantCode,
      string sequentialNumber)
      : base(ParsedResultType.VIN)
    {
      this.VIN = vin;
      this.WorldManufacturerID = worldManufacturerID;
      this.VehicleDescriptorSection = vehicleDescriptorSection;
      this.VehicleIdentifierSection = vehicleIdentifierSection;
      this.CountryCode = countryCode;
      this.VehicleAttributes = vehicleAttributes;
      this.ModelYear = modelYear;
      this.PlantCode = plantCode;
      this.SequentialNumber = sequentialNumber;
    }

    public override string DisplayResult
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder(50);
        stringBuilder.Append(this.WorldManufacturerID).Append(' ');
        stringBuilder.Append(this.VehicleDescriptorSection).Append(' ');
        stringBuilder.Append(this.VehicleIdentifierSection).Append('\n');
        if (this.CountryCode != null)
          stringBuilder.Append(this.CountryCode).Append(' ');
        stringBuilder.Append(this.ModelYear).Append(' ');
        stringBuilder.Append(this.PlantCode).Append(' ');
        stringBuilder.Append(this.SequentialNumber).Append('\n');
        return stringBuilder.ToString();
      }
    }
  }
}
