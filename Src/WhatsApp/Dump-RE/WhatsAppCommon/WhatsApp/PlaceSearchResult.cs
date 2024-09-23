// Decompiled with JetBrains decompiler
// Type: WhatsApp.PlaceSearchResult
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public class PlaceSearchResult
  {
    public double Latitude;
    public double Longitude;
    public WebServices.Attribution Attribution;

    public string Name { get; set; }

    public string ShortText { get; set; }

    public string Icon { get; set; }

    public string LocalIcon { get; set; }

    public double Distance { get; set; }

    public string[] Categories { get; set; }

    public string Locality { get; set; }

    public string AdminDistrict { get; set; }

    public virtual string Address { get; set; }

    public virtual string Url { get; set; }
  }
}
