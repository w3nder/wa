// Decompiled with JetBrains decompiler
// Type: WhatsApp.IPlaceSearch
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;

#nullable disable
namespace WhatsApp
{
  public interface IPlaceSearch
  {
    IObservable<PlaceSearchResult> PlaceSearch(
      double lat,
      double lon,
      double radius,
      bool autocomplete,
      string name);

    IObservable<PlaceSearchResult> LoadAdditionalPlacesFromSearch();
  }
}
