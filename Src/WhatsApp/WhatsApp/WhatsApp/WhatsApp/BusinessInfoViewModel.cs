// Decompiled with JetBrains decompiler
// Type: WhatsApp.BusinessInfoViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class BusinessInfoViewModel : WaViewModelBase
  {
    private static string LogHdr = "BizInfoVM";
    private List<string> regularLinks;
    private string instagramLink;
    private string instagramAccount;
    private const string instagramHostName = "instagram.com";
    private Regex instagramRegex = new Regex("instagram.com/([a-zA-Z\\d\\._]+)", RegexOptions.IgnoreCase);
    private BusinessHoursDetails bizHours;

    public UserStatus BizUser { get; private set; }

    private UserStatusProperties.BusinessUserProperties BizInfo { get; set; }

    public BusinessInfoViewModel(UserStatus bizUser)
    {
      this.BizUser = bizUser;
      this.BizInfo = bizUser?.InternalProperties?.BusinessUserPropertiesField;
      this.ProcessLinks();
      if (this.BizInfo?.BizHoursBlob == null)
        return;
      try
      {
        this.bizHours = BusinessHoursDetails.Deserialize(this.BizInfo.BizHoursBlob);
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "deserialize biz hours");
      }
    }

    public List<string> Links
    {
      get
      {
        if (this.regularLinks == null)
          this.ProcessLinks();
        return this.regularLinks;
      }
    }

    public string InstagramAccount
    {
      get
      {
        if (this.instagramAccount == null && this.regularLinks == null)
          this.ProcessLinks();
        return this.instagramAccount;
      }
    }

    public string InstagramLink => this.instagramLink;

    public Brush VerifiedStateForeground => (Brush) UIUtils.ForegroundBrush;

    public FontWeight VerifiedStateFontWeight => FontWeights.Normal;

    public string VerifiedStateText
    {
      get
      {
        switch (this.BizUser.VerifiedLevel)
        {
          case VerifiedLevel.unknown:
          case VerifiedLevel.low:
            return AppResources.VerifiedBusinessAccount;
          case VerifiedLevel.high:
            return AppResources.VerifiedOfficialBusinessAccount;
          default:
            return string.Empty;
        }
      }
    }

    public Visibility WebsitesVisibility => this.Links.Any<string>().ToVisibility();

    public Visibility DescriptionVisibility
    {
      get => (!string.IsNullOrEmpty(this.BizInfo?.Description?.Trim())).ToVisibility();
    }

    public RichTextBlock.TextSet DescriptionText
    {
      get
      {
        return new RichTextBlock.TextSet()
        {
          Text = this.BizInfo?.Description
        };
      }
    }

    public Visibility AddressVisibility
    {
      get => (!string.IsNullOrEmpty(this.BizInfo?.Address?.Trim())).ToVisibility();
    }

    public string AddressText => this.BizInfo?.Address;

    public GeoCoordinate LocationCoordinate
    {
      get
      {
        GeoCoordinate locationCoordinate = (GeoCoordinate) null;
        if (this.BizInfo != null)
        {
          double? nullable1 = this.BizInfo.Longitude;
          if (nullable1.HasValue)
          {
            nullable1 = this.BizInfo.Latitude;
            if (nullable1.HasValue)
            {
              try
              {
                nullable1 = this.BizInfo.Latitude;
                double latitude = nullable1.Value;
                nullable1 = this.BizInfo.Longitude;
                double longitude = nullable1.Value;
                locationCoordinate = new GeoCoordinate(latitude, longitude);
              }
              catch (Exception ex)
              {
                string logHdr = BusinessInfoViewModel.LogHdr;
                object[] objArray = new object[2];
                double? nullable2 = this.BizInfo.Latitude;
                objArray[0] = (object) nullable2.Value;
                nullable2 = this.BizInfo.Longitude;
                objArray[1] = (object) nullable2.Value;
                Log.l(logHdr, "Exception processing coordinates: {0}, {1}", objArray);
                Log.SendCrashLog(ex, "Exception processing biz coordinates", logOnlyForRelease: true);
              }
            }
          }
        }
        return locationCoordinate;
      }
    }

    public Visibility CategoryVisibility
    {
      get => (!string.IsNullOrEmpty(this.BizInfo?.VerticalCanonical?.Trim())).ToVisibility();
    }

    public string CategoryText => this.BizInfo?.VerticalDescription;

    public Visibility EmailVisibility
    {
      get => (!string.IsNullOrEmpty(this.BizInfo?.Email?.Trim())).ToVisibility();
    }

    public string EmailText => this.BizInfo?.Email;

    public Visibility InstagramVisibility
    {
      get => (!string.IsNullOrEmpty(this.InstagramAccount)).ToVisibility();
    }

    public Visibility HoursVisibility
    {
      get
      {
        BusinessHoursDetails bizHours = this.bizHours;
        bool? nullable;
        if (bizHours == null)
        {
          nullable = new bool?();
        }
        else
        {
          List<BusinessHoursDetails.BusinessHoursPeriodDetails> openPeriods = bizHours.OpenPeriods;
          nullable = openPeriods != null ? new bool?(openPeriods.Any<BusinessHoursDetails.BusinessHoursPeriodDetails>()) : new bool?();
        }
        return (((int) nullable ?? 0) != 0).ToVisibility();
      }
    }

    public List<KeyValuePair<string, string>> GetSchedule(bool startWithToday)
    {
      List<KeyValuePair<string, string>> schedule1 = new List<KeyValuePair<string, string>>();
      List<DayOfWeek> source1 = new List<DayOfWeek>()
      {
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday,
        DayOfWeek.Saturday,
        DayOfWeek.Sunday
      };
      if (startWithToday)
      {
        DayOfWeek dayOfWeek = DateTime.Now.DayOfWeek;
        int count = source1.IndexOf(dayOfWeek);
        if (count > 0)
          source1 = source1.Skip<DayOfWeek>(count).Concat<DayOfWeek>(source1.Take<DayOfWeek>(count)).ToList<DayOfWeek>();
      }
      List<BusinessHoursDetails.BusinessHoursPeriodDetails> openPeriods = this.bizHours?.OpenPeriods;
      if (openPeriods != null && openPeriods.Any<BusinessHoursDetails.BusinessHoursPeriodDetails>())
      {
        Dictionary<DayOfWeek, List<BusinessHoursDetails.BusinessHoursPeriodDetails>> dictionary = new Dictionary<DayOfWeek, List<BusinessHoursDetails.BusinessHoursPeriodDetails>>();
        foreach (BusinessHoursDetails.BusinessHoursPeriodDetails hoursPeriodDetails in openPeriods)
        {
          if (dictionary.ContainsKey(hoursPeriodDetails.Day))
            dictionary[hoursPeriodDetails.Day].Add(hoursPeriodDetails);
          else
            dictionary[hoursPeriodDetails.Day] = new List<BusinessHoursDetails.BusinessHoursPeriodDetails>()
            {
              hoursPeriodDetails
            };
        }
        foreach (DayOfWeek dayOfWeek in source1)
        {
          string dayName = DateTimeFormatInfo.CurrentInfo.GetDayName(dayOfWeek);
          string str = (string) null;
          List<BusinessHoursDetails.BusinessHoursPeriodDetails> source2 = (List<BusinessHoursDetails.BusinessHoursPeriodDetails>) null;
          if (dictionary.TryGetValue(dayOfWeek, out source2) && source2 != null && source2.Any<BusinessHoursDetails.BusinessHoursPeriodDetails>())
          {
            BusinessHoursDetails.BusinessHoursPeriodDetails hoursPeriodDetails = source2.FirstOrDefault<BusinessHoursDetails.BusinessHoursPeriodDetails>((Func<BusinessHoursDetails.BusinessHoursPeriodDetails, bool>) (period => period.Mode != BusinessHoursDetails.BusinessHoursPeriodDetails.OpenMode.SpecificHours));
            if (hoursPeriodDetails == null)
            {
              if (source2.Count > 1)
                source2.Sort((Comparison<BusinessHoursDetails.BusinessHoursPeriodDetails>) ((p1, p2) => Nullable.Compare<TimeSpan>(p1.OpenTime, p2.OpenTime)));
              Func<BusinessHoursDetails.BusinessHoursPeriodDetails, string> formatHours = (Func<BusinessHoursDetails.BusinessHoursPeriodDetails, string>) (period =>
              {
                string schedule2 = (string) null;
                if (period.OpenTime.HasValue && period.CloseTime.HasValue)
                {
                  DateTime dateTime1 = DateTime.Today.Add(period.OpenTime.Value);
                  DateTime dateTime2 = DateTime.Today.Add(period.CloseTime.Value);
                  schedule2 = string.Format("{0} - {1}", (object) dateTime1.ToShortTimeString(), (object) dateTime2.ToShortTimeString());
                }
                return schedule2;
              });
              str = string.Join(";\n", source2.Select<BusinessHoursDetails.BusinessHoursPeriodDetails, string>((Func<BusinessHoursDetails.BusinessHoursPeriodDetails, string>) (period => formatHours(period))).Where<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s))));
            }
            else
            {
              switch (hoursPeriodDetails.Mode)
              {
                case BusinessHoursDetails.BusinessHoursPeriodDetails.OpenMode.Open24:
                  str = AppResources.BizHoursOpen24;
                  break;
                case BusinessHoursDetails.BusinessHoursPeriodDetails.OpenMode.AppointmentOnly:
                  str = AppResources.BizHoursAppointmentsOnly;
                  break;
              }
            }
          }
          KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(dayName, str ?? AppResources.BizHoursClosed);
          schedule1.Add(keyValuePair);
        }
      }
      return schedule1;
    }

    private void ProcessLinks()
    {
      try
      {
        this.regularLinks = new List<string>();
        foreach (string input in this.BizInfo?.Websites ?? new List<string>())
        {
          if (this.instagramAccount == null)
          {
            System.Text.RegularExpressions.Match match = this.instagramRegex.Match(input);
            if (match.Success)
            {
              this.instagramLink = input;
              this.instagramAccount = string.Format("@{0}", (object) match.Groups[1].Value);
              continue;
            }
          }
          this.regularLinks.Add(input);
        }
      }
      catch (Exception ex)
      {
      }
    }
  }
}
