// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.LocationPicker
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class LocationPicker : WamEvent
  {
    public wam_enum_places_api? locationPickerPlacesSource;
    public wam_enum_places_api? locationPickerPlacesSourceDefault;
    public wam_enum_location_picker_result_type? locationPickerResultType;
    public wam_enum_location_picker_places_response? locationPickerPlacesResponse;
    public string locationPickerFailureDescription;
    public bool? locationPickerFullScreen;
    public string locationPickerQueryString;
    public double? locationPickerRequestsCount;
    public double? locationPickerPlacesCount;
    public double? locationPickerSelectedPlaceIndex;
    public long? locationPickerSpendT;
    public wam_enum_maps_api? locationPickerMapsApi;

    public void Reset()
    {
      this.locationPickerPlacesSource = new wam_enum_places_api?();
      this.locationPickerPlacesSourceDefault = new wam_enum_places_api?();
      this.locationPickerResultType = new wam_enum_location_picker_result_type?();
      this.locationPickerPlacesResponse = new wam_enum_location_picker_places_response?();
      this.locationPickerFailureDescription = (string) null;
      this.locationPickerFullScreen = new bool?();
      this.locationPickerQueryString = (string) null;
      this.locationPickerRequestsCount = new double?();
      this.locationPickerPlacesCount = new double?();
      this.locationPickerSelectedPlaceIndex = new double?();
      this.locationPickerSpendT = new long?();
      this.locationPickerMapsApi = new wam_enum_maps_api?();
    }

    public override uint GetCode() => 482;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_places_api>(this.locationPickerPlacesSource));
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_places_api>(this.locationPickerPlacesSourceDefault));
      Wam.MaybeSerializeField(3, Wam.EnumToLong<wam_enum_location_picker_result_type>(this.locationPickerResultType));
      Wam.MaybeSerializeField(4, Wam.EnumToLong<wam_enum_location_picker_places_response>(this.locationPickerPlacesResponse));
      Wam.MaybeSerializeField(5, this.locationPickerFailureDescription);
      Wam.MaybeSerializeField(7, this.locationPickerFullScreen);
      Wam.MaybeSerializeField(8, this.locationPickerQueryString);
      Wam.MaybeSerializeField(9, this.locationPickerRequestsCount);
      Wam.MaybeSerializeField(10, this.locationPickerPlacesCount);
      Wam.MaybeSerializeField(11, this.locationPickerSelectedPlaceIndex);
      Wam.MaybeSerializeField(12, this.locationPickerSpendT);
      Wam.MaybeSerializeField(13, Wam.EnumToLong<wam_enum_maps_api>(this.locationPickerMapsApi));
    }
  }
}
