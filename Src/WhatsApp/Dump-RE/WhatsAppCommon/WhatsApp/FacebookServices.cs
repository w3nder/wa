// Decompiled with JetBrains decompiler
// Type: WhatsApp.FacebookServices
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

#nullable disable
namespace WhatsApp
{
  public class FacebookServices : IPlaceSearch
  {
    private List<PlaceSearchResult> additionalPlaces;
    private static WebServices.Attribution attribution = new WebServices.Attribution()
    {
      Text = "Powered by Facebook",
      Logo = ImageStore.GetStockIcon("/Images/powered-by-facebook-on-white.png", "/Images/powered-by-facebook-on-non-white.png")
    };
    public static Dictionary<string, string> TypeIcons = new Dictionary<string, string>()
    {
      {
        "activity-recreation",
        "school.png"
      },
      {
        "airport-terminal",
        "airport.png"
      },
      {
        "airport",
        "airport.png"
      },
      {
        "arts",
        "art_gallery.png"
      },
      {
        "bank",
        "civic_building.png"
      },
      {
        "bar-beergarden",
        "bar.png"
      },
      {
        "breakfast-brunch",
        "cafe.png"
      },
      {
        "burgers",
        "restaurant.png"
      },
      {
        "chinese",
        "restaurant.png"
      },
      {
        "city",
        "office-71.png"
      },
      {
        "cocktail-nightlife",
        "bar.png"
      },
      {
        "coffee",
        "cafe.png"
      },
      {
        "deli-sandwich",
        "restaurant.png"
      },
      {
        "delivery-takeaway",
        "restaurant.png"
      },
      {
        "dessert",
        "restaurant.png"
      },
      {
        "entertainment",
        "movies.png"
      },
      {
        "event",
        "political.png"
      },
      {
        "fastfood",
        "restaurant.png"
      },
      {
        "home",
        "generic_business.png"
      },
      {
        "hotel",
        "lodging.png"
      },
      {
        "italian",
        "restaurant.png"
      },
      {
        "lunch",
        "restaurant.png"
      },
      {
        "mexican",
        "restaurant.png"
      },
      {
        "more",
        "generic_business.png"
      },
      {
        "outdoor",
        "flower.png"
      },
      {
        "pin",
        "geocode.png"
      },
      {
        "pizza",
        "restaurant.png"
      },
      {
        "professional-services",
        "doctor.png"
      },
      {
        "restaurant",
        "restaurant.png"
      },
      {
        "region",
        "political.png"
      },
      {
        "steak",
        "restaurant.png"
      },
      {
        "thai",
        "restaurant.png"
      },
      {
        "winebar",
        "wine.png"
      },
      {
        "shopping",
        "store.png"
      }
    };

    private static string Key => "1609427805955024%7Cf1de6fcdcb11b215ea7a2d3cd062ecff";

    public IObservable<PlaceSearchResult> PlaceSearch(
      double lat,
      double lon,
      double radius,
      bool autocomplete,
      string name)
    {
      string letterIsoLanguageName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
      name = !string.IsNullOrEmpty(name) ? "&q=" + name : "";
      string uri = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://graph.facebook.com/v2.3/search?access_token={0}&center={1},{2}&distance={3}&type=place&limit=25&fields=name,location,link,place_topics.limit(1){{icon_url}}&locale={4}{5}", (object) FacebookServices.Key, (object) lat, (object) lon, (object) radius, (object) letterIsoLanguageName, (object) name);
      return Observable.Defer<PlaceSearchResult>((Func<IObservable<PlaceSearchResult>>) (() => Observable.Defer<byte[]>((Func<IObservable<byte[]>>) (() => new Uri(uri, UriKind.Absolute).ToGetRequest().GetResponseBytesAync())).Cache(WebServices.GetCachePath("face", lat, lon, radius, name)).SelectMany<byte[], PlaceSearchResult, PlaceSearchResult>((Func<byte[], IObservable<PlaceSearchResult>>) (bytes => this.InterpretPlaceResults(bytes)), (Func<byte[], PlaceSearchResult, PlaceSearchResult>) ((bytes, res) => res))));
    }

    private IObservable<PlaceSearchResult> InterpretPlaceResults(byte[] resultBytes)
    {
      FacebookServices.ResultRoot resultRoot = (FacebookServices.ResultRoot) null;
      if (resultBytes != null && resultBytes.Length != 0)
      {
        using (MemoryStream memoryStream = new MemoryStream(resultBytes))
          resultRoot = new DataContractJsonSerializer(typeof (FacebookServices.ResultRoot)).ReadObject((Stream) memoryStream) as FacebookServices.ResultRoot;
      }
      if (resultRoot == null)
        throw new Exception("json results came back as null");
      IEnumerable<PlaceSearchResult> results = ((IEnumerable<FacebookServices.Data>) (resultRoot.Data ?? new FacebookServices.Data[0])).Where<FacebookServices.Data>((Func<FacebookServices.Data, bool>) (venue => venue.ID != null && venue.Name != null && venue.Location != null && venue.Link != null && venue.Location.Latitude.HasValue && venue.Location.Longitude.HasValue && venue.Location.HasAddress && venue.PlaceTopics != null)).Select<FacebookServices.Data, PlaceSearchResult>((Func<FacebookServices.Data, PlaceSearchResult>) (venue => new PlaceSearchResult()
      {
        Name = venue.Name,
        ShortText = Utils.CommaSeparate(((IEnumerable<string>) new string[2]
        {
          venue.Location.Street,
          venue.Location.City
        }).Where<string>((Func<string, bool>) (s => !string.IsNullOrWhiteSpace(s)))),
        Url = venue.Link,
        Latitude = venue.Location.Latitude.Value,
        Longitude = venue.Location.Longitude.Value,
        Address = venue.Location.FormattedAddress,
        Categories = ((IEnumerable<FacebookServices.Data2>) venue.PlaceTopics.Data).Select<FacebookServices.Data2, string>((Func<FacebookServices.Data2, string>) (c => c.ID)).Where<string>((Func<string, bool>) (c => !string.IsNullOrEmpty(c))).ToArray<string>(),
        Icon = ((IEnumerable<FacebookServices.Data2>) venue.PlaceTopics.Data).Count<FacebookServices.Data2>() <= 0 || venue.PlaceTopics.Data[0] == null ? "" : venue.PlaceTopics.Data[0].getIconUrl(),
        LocalIcon = ((IEnumerable<FacebookServices.Data2>) venue.PlaceTopics.Data).Count<FacebookServices.Data2>() <= 0 || venue.PlaceTopics.Data[0] == null ? "" : venue.PlaceTopics.LocalIcon(),
        Attribution = FacebookServices.attribution
      }));
      return Observable.Create<PlaceSearchResult>((Func<IObserver<PlaceSearchResult>, Action>) (observer =>
      {
        int num = 0;
        this.additionalPlaces = new List<PlaceSearchResult>();
        foreach (PlaceSearchResult placeSearchResult in results)
        {
          if (num < 20)
          {
            observer.OnNext(placeSearchResult);
            ++num;
          }
          else
            this.additionalPlaces.Add(placeSearchResult);
        }
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    public IObservable<PlaceSearchResult> LoadAdditionalPlacesFromSearch()
    {
      return Observable.Create<PlaceSearchResult>((Func<IObserver<PlaceSearchResult>, Action>) (observer =>
      {
        int num = 0;
        List<PlaceSearchResult> placeSearchResultList = new List<PlaceSearchResult>();
        if (this.additionalPlaces != null)
        {
          foreach (PlaceSearchResult additionalPlace in this.additionalPlaces)
          {
            if (num < 20)
            {
              observer.OnNext(additionalPlace);
              ++num;
            }
            else
              placeSearchResultList.Add(additionalPlace);
          }
          this.additionalPlaces = placeSearchResultList;
        }
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    [DataContract]
    public class ResultRoot
    {
      [DataMember(Name = "data")]
      public FacebookServices.Data[] Data { get; set; }

      [DataMember(Name = "paging")]
      public FacebookServices.Paging Paging { get; set; }
    }

    [DataContract]
    public class Paging
    {
      [DataMember(Name = "next")]
      public string Next { get; set; }
    }

    [DataContract]
    public class Data
    {
      [DataMember(Name = "name")]
      public string Name { get; set; }

      [DataMember(Name = "location")]
      public FacebookServices.Location Location { get; set; }

      [DataMember(Name = "link")]
      public string Link { get; set; }

      [DataMember(Name = "id")]
      public string ID { get; set; }

      [DataMember(Name = "place_topics")]
      public FacebookServices.Place_Topics PlaceTopics { get; set; }
    }

    [DataContract]
    public class Location
    {
      [DataMember(Name = "street")]
      public string Street { get; set; }

      [DataMember(Name = "city")]
      public string City { get; set; }

      [DataMember(Name = "state")]
      public string State { get; set; }

      [DataMember(Name = "country")]
      public string Country { get; set; }

      [DataMember(Name = "zip")]
      public string Zip { get; set; }

      [DataMember(Name = "latitude")]
      public double? Latitude { get; set; }

      [DataMember(Name = "longitude")]
      public double? Longitude { get; set; }

      public bool HasAddress
      {
        get
        {
          string[] strArray = new string[2]
          {
            this.Street,
            this.City
          };
          foreach (string str in strArray)
          {
            if (str == null)
              return false;
          }
          return true;
        }
      }

      public string FormattedAddress
      {
        get
        {
          return Utils.CommaSeparate(((IEnumerable<string>) new string[4]
          {
            this.Street,
            this.City,
            this.State,
            this.Zip
          }).Where<string>((Func<string, bool>) (s => !string.IsNullOrWhiteSpace(s))));
        }
      }
    }

    [DataContract]
    public class Place_Topics
    {
      [DataMember(Name = "data")]
      public FacebookServices.Data2[] Data { get; set; }

      public string LocalIcon()
      {
        string str = "../Images/Places/" + (ImageStore.IsDarkTheme() ? "Dark/" : "Light/");
        foreach (FacebookServices.Data2 data2 in this.Data)
        {
          string key = "";
          try
          {
            key = data2.IconUrl.Substring(data2.IconUrl.LastIndexOf("/") + 1);
          }
          catch (IndexOutOfRangeException ex)
          {
            Log.LogException((Exception) ex, "Could not find catagory from icon_url");
          }
          if (FacebookServices.TypeIcons.ContainsKey(key))
          {
            string typeIcon = FacebookServices.TypeIcons[key];
            if (typeIcon != null)
              return str + typeIcon;
          }
        }
        return str + "generic_business.png";
      }
    }

    [DataContract]
    public class Data2
    {
      [DataMember(Name = "icon_url")]
      public string IconUrl { get; set; }

      [DataMember(Name = "id")]
      public string ID { get; set; }

      public string getIconUrl()
      {
        return this.IconUrl == null ? "https://www.facebook.com/images/places/topics/pin_72.png" : this.IconUrl + "_72.png";
      }
    }
  }
}
