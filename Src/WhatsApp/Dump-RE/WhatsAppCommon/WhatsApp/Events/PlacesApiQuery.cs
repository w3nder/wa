// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.PlacesApiQuery
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class PlacesApiQuery : WamEvent
  {
    public wam_enum_places_api? placesApiSource;
    public wam_enum_places_api? placesApiSourceDefault;
    public wam_enum_places_api_response? placesApiResponse;
    public string placesApiFailureDescription;
    public long? placesApiRequestIndex;
    public bool? placesApiCached;
    public string placesApiQueryString;
    public double? placesApiPlacesCount;
    public long? placesApiResponseT;

    public void Reset()
    {
      this.placesApiSource = new wam_enum_places_api?();
      this.placesApiSourceDefault = new wam_enum_places_api?();
      this.placesApiResponse = new wam_enum_places_api_response?();
      this.placesApiFailureDescription = (string) null;
      this.placesApiRequestIndex = new long?();
      this.placesApiCached = new bool?();
      this.placesApiQueryString = (string) null;
      this.placesApiPlacesCount = new double?();
      this.placesApiResponseT = new long?();
    }

    public override uint GetCode() => 834;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_places_api>(this.placesApiSource));
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_places_api>(this.placesApiSourceDefault));
      Wam.MaybeSerializeField(3, Wam.EnumToLong<wam_enum_places_api_response>(this.placesApiResponse));
      Wam.MaybeSerializeField(4, this.placesApiFailureDescription);
      Wam.MaybeSerializeField(5, this.placesApiRequestIndex);
      Wam.MaybeSerializeField(6, this.placesApiCached);
      Wam.MaybeSerializeField(7, this.placesApiQueryString);
      Wam.MaybeSerializeField(8, this.placesApiPlacesCount);
      Wam.MaybeSerializeField(9, this.placesApiResponseT);
    }
  }
}
