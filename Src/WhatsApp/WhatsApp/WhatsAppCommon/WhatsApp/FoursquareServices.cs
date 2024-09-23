// Decompiled with JetBrains decompiler
// Type: WhatsApp.FoursquareServices
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


namespace WhatsApp
{
  public class FoursquareServices : IPlaceSearch
  {
    private const string Version = "20140601";
    private List<PlaceSearchResult> additionalPlaces;
    private static WebServices.Attribution attribution = new WebServices.Attribution()
    {
      Text = "Venue data provided by Foursquare",
      Logo = ImageStore.GetStockIcon("/Images/powered-by-foursquare.png")
    };
    public static Dictionary<string, string> TypeIcons = new Dictionary<string, string>()
    {
      {
        "airport",
        "airport.png"
      },
      {
        "theme park",
        "amusement_park.png"
      },
      {
        "aquarium",
        "aquarium.png"
      },
      {
        "art",
        "art_gallery.png"
      },
      {
        "art gallery",
        "art_gallery.png"
      },
      {
        "atm",
        "atm.png"
      },
      {
        "bar",
        "bar.png"
      },
      {
        "beer",
        "bar.png"
      },
      {
        "nightlife",
        "bar.png"
      },
      {
        "pub",
        "bar.png"
      },
      {
        "barbershop",
        "beauty_salon.png"
      },
      {
        "salon",
        "beauty_salon.png"
      },
      {
        "salon / barbershop",
        "beauty_salon.png"
      },
      {
        "bike",
        "bicycle_store.png"
      },
      {
        "bike shop",
        "bicycle_store.png"
      },
      {
        "bowling alley",
        "bowling_alley.png"
      },
      {
        "bus",
        "bus.png"
      },
      {
        "bus station",
        "bus.png"
      },
      {
        "bus line",
        "bus.png"
      },
      {
        "café",
        "cafe.png"
      },
      {
        "cafe",
        "cafe.png"
      },
      {
        "coffee",
        "cafe.png"
      },
      {
        "tea",
        "cafe.png"
      },
      {
        "campground",
        "campground.png"
      },
      {
        "car",
        "car_dealer.png"
      },
      {
        "car dealership",
        "car_dealer.png"
      },
      {
        "car wash",
        "car_dealer.png"
      },
      {
        "rental car location",
        "car_dealer.png"
      },
      {
        "casino",
        "casino.png"
      },
      {
        "community center",
        "civic_building.png"
      },
      {
        "convention center",
        "civic_building.png"
      },
      {
        "courthouse",
        "courthouse.png"
      },
      {
        "dentist",
        "dentist.png"
      },
      {
        "doctor",
        "doctor.png"
      },
      {
        "doctor's office",
        "doctor.png"
      },
      {
        "electronics",
        "electronics.png"
      },
      {
        "electronics store",
        "electronics.png"
      },
      {
        "gym",
        "fitness.png"
      },
      {
        "gym / fitness center",
        "fitness.png"
      },
      {
        "flower",
        "flower.png"
      },
      {
        "flower shop",
        "flower.png"
      },
      {
        "gas",
        "gas_station.png"
      },
      {
        "gas station / garage",
        "gas_station.png"
      },
      {
        "generic_business",
        "generic_business.png"
      },
      {
        "jewelry",
        "jewelry_store.png"
      },
      {
        "jewelry store",
        "jewelry_store.png"
      },
      {
        "library",
        "library.png"
      },
      {
        "hotel",
        "lodging.png"
      },
      {
        "hostel",
        "lodging.png"
      },
      {
        "landmark",
        "monument.png"
      },
      {
        "monument",
        "monument.png"
      },
      {
        "monument / landmark",
        "monument.png"
      },
      {
        "movie",
        "movies.png"
      },
      {
        "movie theater",
        "movies.png"
      },
      {
        "museum",
        "museum.png"
      },
      {
        "night club",
        "bar.png"
      },
      {
        "pet",
        "pet_store.png"
      },
      {
        "pet store",
        "pet_store.png"
      },
      {
        "pet service",
        "pet_store.png"
      },
      {
        "police",
        "police.png"
      },
      {
        "police station",
        "police.png"
      },
      {
        "government building",
        "political.png"
      },
      {
        "government",
        "political.png"
      },
      {
        "post office",
        "post_office.png"
      },
      {
        "Other Repair Shop",
        "repair.png"
      },
      {
        "food",
        "restaurant.png"
      },
      {
        "restaurant",
        "restaurant.png"
      },
      {
        "school",
        "school.png"
      },
      {
        "shop",
        "store.png"
      },
      {
        "shop & service",
        "store.png"
      },
      {
        "shops",
        "store.png"
      },
      {
        "food & drink shop",
        "supermarket.png"
      },
      {
        "college",
        "university.png"
      },
      {
        "college & university",
        "university.png"
      },
      {
        "university",
        "university.png"
      },
      {
        "taxi",
        "taxi.png"
      },
      {
        "train",
        "train.png"
      },
      {
        "train station",
        "train.png"
      },
      {
        "travel agency",
        "travel_agent.png"
      },
      {
        "travel",
        "travel_agent.png"
      },
      {
        "liquor",
        "wine.png"
      },
      {
        "liquor store",
        "wine.png"
      },
      {
        "wine",
        "wine.png"
      },
      {
        "wine shop",
        "wine.png"
      },
      {
        "church",
        "worship_general.png"
      },
      {
        "mosque",
        "worship_general.png"
      },
      {
        "synagogue",
        "worship_general.png"
      },
      {
        "spirtual",
        "worship_general.png"
      },
      {
        "spirtual center",
        "worship_general.png"
      },
      {
        "zoo",
        "zoo.png"
      }
    };

    private static string ClientId => NativeInterfaces.Misc.GetString(1);

    private static string ClientSecret => NativeInterfaces.Misc.GetString(2);

    public IObservable<PlaceSearchResult> PlaceSearch(
      double lat,
      double lon,
      double radius,
      bool autocomplete,
      string name)
    {
      string url = "";
      if (!autocomplete)
      {
        name = !string.IsNullOrEmpty(name) ? "&query=" + name : "";
        url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://api.foursquare.com/v2/venues/search?ll={0},{1}&radius={2}&client_secret={3}&client_id={4}&v={5}{6}", (object) lat, (object) lon, (object) radius, (object) FoursquareServices.ClientSecret, (object) FoursquareServices.ClientId, (object) "20140601", (object) name);
        return Observable.Defer<PlaceSearchResult>((Func<IObservable<PlaceSearchResult>>) (() => Observable.Defer<byte[]>((Func<IObservable<byte[]>>) (() => new Uri(url, UriKind.Absolute).ToGetRequest().GetResponseBytesAync())).Cache(WebServices.GetCachePath("fsq", lat, lon, radius, name)).SelectMany<byte[], PlaceSearchResult, PlaceSearchResult>((Func<byte[], IObservable<PlaceSearchResult>>) (bytes => this.InterpretPlaceResults(bytes)), (Func<byte[], PlaceSearchResult, PlaceSearchResult>) ((bytes, res) => res))));
      }
      url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://api.foursquare.com/v2/venues/suggestcompletion?ll={0},{1}&radius={2}&query={3}&client_secret={4}&client_id={5}&v={6}&limit=50", (object) lat, (object) lon, (object) radius, (object) Uri.EscapeUriString(name), (object) FoursquareServices.ClientSecret, (object) FoursquareServices.ClientId, (object) "20140601");
      return Observable.Defer<PlaceSearchResult>((Func<IObservable<PlaceSearchResult>>) (() => Observable.Defer<byte[]>((Func<IObservable<byte[]>>) (() => new Uri(url, UriKind.Absolute).ToGetRequest().GetResponseBytesAync())).SelectMany<byte[], PlaceSearchResult, PlaceSearchResult>((Func<byte[], IObservable<PlaceSearchResult>>) (bytes => this.InterpretSuggestPlaceResults(bytes)), (Func<byte[], PlaceSearchResult, PlaceSearchResult>) ((bytes, res) => res))));
    }

    private IObservable<PlaceSearchResult> InterpretSuggestPlaceResults(byte[] resultBytes)
    {
      FoursquareServices.SuggestResponseRoot suggestResponseRoot = (FoursquareServices.SuggestResponseRoot) null;
      if (resultBytes != null && resultBytes.Length != 0)
      {
        using (MemoryStream memoryStream = new MemoryStream(resultBytes))
          suggestResponseRoot = new DataContractJsonSerializer(typeof (FoursquareServices.SuggestResponseRoot)).ReadObject((Stream) memoryStream) as FoursquareServices.SuggestResponseRoot;
      }
      if (suggestResponseRoot == null)
        throw new Exception("deserializer came back with null object");
      if (suggestResponseRoot.Metadata.Status < 200 || suggestResponseRoot.Metadata.Status > 299)
        throw new Exception("unexpected status " + (object) suggestResponseRoot.Metadata.Status);
      if (suggestResponseRoot.Response == null)
        throw new Exception("null response");
      return this.InterpretResults(suggestResponseRoot.Response.Venues);
    }

    private IObservable<PlaceSearchResult> InterpretPlaceResults(byte[] resultBytes)
    {
      FoursquareServices.ResponseRoot responseRoot = (FoursquareServices.ResponseRoot) null;
      if (resultBytes != null && resultBytes.Length != 0)
      {
        using (MemoryStream memoryStream = new MemoryStream(resultBytes))
          responseRoot = new DataContractJsonSerializer(typeof (FoursquareServices.ResponseRoot)).ReadObject((Stream) memoryStream) as FoursquareServices.ResponseRoot;
      }
      if (responseRoot == null)
        throw new Exception("deserializer came back with null object");
      if (responseRoot.Metadata.Status < 200 || responseRoot.Metadata.Status > 299)
        throw new Exception("unexpected status " + (object) responseRoot.Metadata.Status);
      if (responseRoot.Response == null)
        throw new Exception("null response");
      return this.InterpretResults(responseRoot.Response.Venues);
    }

    private IObservable<PlaceSearchResult> InterpretResults(FoursquareServices.Venue[] venues)
    {
      IEnumerable<PlaceSearchResult> results = ((IEnumerable<FoursquareServices.Venue>) (venues ?? new FoursquareServices.Venue[0])).Where<FoursquareServices.Venue>((Func<FoursquareServices.Venue, bool>) (venue => venue.ID != null && venue.Name != null && venue.Location != null && venue.Location.Latitude.HasValue && venue.Location.Longitude.HasValue && venue.Location.HasAddress && venue.Categories != null)).Select<FoursquareServices.Venue, PlaceSearchResult>((Func<FoursquareServices.Venue, PlaceSearchResult>) (venue =>
      {
        PlaceSearchResult placeSearchResult1 = new PlaceSearchResult();
        placeSearchResult1.Name = venue.Name;
        placeSearchResult1.ShortText = Utils.CommaSeparate(((IEnumerable<string>) new string[2]
        {
          venue.Location.Address,
          venue.Location.City
        }).Where<string>((Func<string, bool>) (s => !string.IsNullOrWhiteSpace(s))));
        placeSearchResult1.Url = string.Format("https://foursquare.com/v/{0}", (object) venue.ID);
        PlaceSearchResult placeSearchResult2 = placeSearchResult1;
        double? nullable = venue.Location.Latitude;
        double num1 = nullable.Value;
        placeSearchResult2.Latitude = num1;
        PlaceSearchResult placeSearchResult3 = placeSearchResult1;
        nullable = venue.Location.Longitude;
        double num2 = nullable.Value;
        placeSearchResult3.Longitude = num2;
        placeSearchResult1.Address = venue.Location.FormattedAddress;
        placeSearchResult1.Categories = ((IEnumerable<FoursquareServices.Category>) venue.Categories).Select<FoursquareServices.Category, string>((Func<FoursquareServices.Category, string>) (c => c.Name)).Where<string>((Func<string, bool>) (c => !string.IsNullOrEmpty(c))).ToArray<string>();
        placeSearchResult1.Icon = ((IEnumerable<FoursquareServices.Category>) venue.Categories).Count<FoursquareServices.Category>() <= 0 || venue.Categories[0] == null ? "" : venue.Categories[0].Icon.IconUrl();
        placeSearchResult1.LocalIcon = venue.LocalIcon();
        placeSearchResult1.Attribution = FoursquareServices.attribution;
        return placeSearchResult1;
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
    public class ResponseRoot
    {
      [DataMember(Name = "meta")]
      public FoursquareServices.Meta Metadata { get; set; }

      [DataMember(Name = "response")]
      public FoursquareServices.Response Response { get; set; }
    }

    [DataContract]
    public class SuggestResponseRoot
    {
      [DataMember(Name = "meta")]
      public FoursquareServices.Meta Metadata { get; set; }

      [DataMember(Name = "response")]
      public FoursquareServices.SuggestResponse Response { get; set; }
    }

    [DataContract]
    public class Meta
    {
      [DataMember(Name = "code")]
      public int Status { get; set; }
    }

    [DataContract]
    public class Response
    {
      [DataMember(Name = "venues")]
      public FoursquareServices.Venue[] Venues { get; set; }
    }

    [DataContract]
    public class SuggestResponse
    {
      [DataMember(Name = "minivenues")]
      public FoursquareServices.Venue[] Venues { get; set; }
    }

    [DataContract]
    public class Venue
    {
      [DataMember(Name = "id")]
      public string ID { get; set; }

      [DataMember(Name = "name")]
      public string Name { get; set; }

      [DataMember(Name = "location")]
      public FoursquareServices.Location Location { get; set; }

      [DataMember(Name = "categories")]
      public FoursquareServices.Category[] Categories { get; set; }

      public string LocalIcon()
      {
        string str1 = "../Images/Places/" + (ImageStore.IsDarkTheme() ? "Dark/" : "Light/");
        string str2 = "";
        foreach (FoursquareServices.Category category in this.Categories)
        {
          if (category.ShortName != null)
          {
            if (FoursquareServices.TypeIcons.TryGetValue(category.ShortName.ToLower(), out str2) && !string.IsNullOrEmpty(str2))
              return str1 + str2;
            string lower = category.ShortName.ToLower();
            char[] chArray = new char[1]{ ' ' };
            foreach (string key in lower.Split(chArray))
            {
              if (FoursquareServices.TypeIcons.TryGetValue(key, out str2) && !string.IsNullOrEmpty(str2))
                return str1 + str2;
            }
          }
        }
        foreach (FoursquareServices.Category category in this.Categories)
        {
          if (category.Icon != null && category.Icon.Prefix != null)
          {
            string[] strArray = category.Icon.Prefix.Split('/');
            if (strArray.Length > 2 && FoursquareServices.TypeIcons.TryGetValue(strArray[strArray.Length - 2], out str2))
              return str1 + str2;
          }
        }
        return str1 + "generic_business.png";
      }
    }

    [DataContract]
    public class Category
    {
      [DataMember(Name = "icon")]
      public FoursquareServices.Icon Icon { get; set; }

      [DataMember(Name = "name")]
      public string Name { get; set; }

      [DataMember(Name = "shortName")]
      public string ShortName { get; set; }
    }

    [DataContract]
    public class Icon
    {
      [DataMember(Name = "prefix")]
      public string Prefix { get; set; }

      [DataMember(Name = "suffix")]
      public string Suffix { get; set; }

      public string IconUrl(int size = 32, bool white = true)
      {
        return string.Format("{0}{1}{2}{3}", (object) this.Prefix, white ? (object) "" : (object) "bg_", (object) size.ToString(), (object) this.Suffix);
      }
    }

    [DataContract]
    public class Location
    {
      [DataMember(Name = "lat")]
      public double? Latitude { get; set; }

      [DataMember(Name = "lng")]
      public double? Longitude { get; set; }

      [DataMember(Name = "address")]
      public string Address { get; set; }

      [DataMember(Name = "city")]
      public string City { get; set; }

      [DataMember(Name = "state")]
      public string State { get; set; }

      [DataMember(Name = "postalCode")]
      public string PostalCode { get; set; }

      public bool HasAddress
      {
        get
        {
          string[] strArray = new string[2]
          {
            this.Address,
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
            this.Address,
            this.City,
            this.State,
            this.PostalCode
          }).Where<string>((Func<string, bool>) (s => !string.IsNullOrWhiteSpace(s))));
        }
      }
    }
  }
}
