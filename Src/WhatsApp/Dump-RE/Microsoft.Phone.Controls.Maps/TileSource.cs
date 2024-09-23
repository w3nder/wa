// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.TileSource
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class TileSource : INotifyPropertyChanged
  {
    public const string UriSchemeUriFragment = "{UriScheme}";
    public const string QuadKeyUriFragment = "{quadkey}";
    public const string SubdomainUriFragment = "{subdomain}";
    private const string InternalQuadKeyUriFragment = "{QUADKEY}";
    private const string InternalSubdomainUriFragment = "{SUBDOMAIN}";
    private string convertedUriFormat;
    private int maxX;
    private int maxY;
    private string[][] subdomainsList;
    private string uriFormat;

    public TileSource()
    {
      this.subdomainsList = new string[2][]
      {
        new string[4]{ "0", "2", "4", "6" },
        new string[4]{ "1", "3", "5", "7" }
      };
      this.maxX = 2;
      this.maxY = 4;
    }

    public TileSource(string uriFormat)
      : this()
    {
      this.UriFormat = uriFormat;
    }

    public string UriFormat
    {
      get => this.uriFormat;
      set
      {
        if (!(this.uriFormat != value))
          return;
        this.uriFormat = value;
        this.convertedUriFormat = TileSource.ReplaceString(this.uriFormat, "{UriScheme}", "HTTP");
        this.convertedUriFormat = TileSource.ReplaceString(this.convertedUriFormat, "{quadkey}", "{QUADKEY}");
        this.convertedUriFormat = TileSource.ReplaceString(this.convertedUriFormat, "{subdomain}", "{SUBDOMAIN}");
        this.OnPropertyChanged(nameof (UriFormat));
      }
    }

    public virtual Uri GetUri(int x, int y, int zoomLevel)
    {
      Uri uri = (Uri) null;
      QuadKey quadKey = new QuadKey(x, y, zoomLevel);
      if (!string.IsNullOrEmpty(this.convertedUriFormat) && !string.IsNullOrEmpty(quadKey.Key))
        uri = new Uri(this.convertedUriFormat.Replace("{QUADKEY}", quadKey.Key).Replace("{SUBDOMAIN}", this.GetSubdomain(quadKey)));
      return uri;
    }

    public virtual string GetSubdomain(QuadKey quadKey)
    {
      return this.subdomainsList == null ? string.Empty : this.subdomainsList[quadKey.X % this.maxX][quadKey.Y % this.maxY];
    }

    public void SetSubdomains(string[][] subdomains)
    {
      if (subdomains != null)
      {
        int num = subdomains.Length != 0 && subdomains[0].Length != 0 ? subdomains[0].Length : throw new ArgumentException(ExceptionStrings.TileSource_InvalidSubdomains_LengthMoreThan0);
        foreach (string[] subdomain in subdomains)
        {
          if (subdomain.Length != num)
            throw new ArgumentException(ExceptionStrings.TileSource_InvalidSubdomains_DifferentLength);
          foreach (string str in subdomain)
          {
            if (str == null)
              throw new ArgumentException(ExceptionStrings.TileSource_InvalidSubdomain_stringNull);
          }
        }
        this.subdomainsList = subdomains;
        this.maxX = subdomains.Length;
        this.maxY = num;
      }
      else
        this.subdomainsList = (string[][]) null;
    }

    private static string ReplaceString(string input, string pattern, string replacement)
    {
      return Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
      propertyChanged((object) this, e);
    }
  }
}
