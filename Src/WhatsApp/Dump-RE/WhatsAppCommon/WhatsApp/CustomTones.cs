// Decompiled with JetBrains decompiler
// Type: WhatsApp.CustomTones
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

#nullable disable
namespace WhatsApp
{
  public class CustomTones
  {
    private const int DefaultAlertIndex = 5;

    public static string DefaultAlertPath
    {
      get => string.Format("Sounds\\Alert-{0}.wma", (object) 5.ToString().PadLeft(2, '0'));
    }

    public static string DefaultAlertName => string.Format(AppResources.AlertAtIdx, (object) 5);

    private static CustomTones.Tone FilterNumberedAlert(CustomTones.Tone t, int i)
    {
      switch (i)
      {
        case 1:
          t.Title = AppResources.AlertWindowsVoicemail;
          t.SortIdx = 2;
          break;
        case 7:
          i = 1;
          break;
        case 8:
          t.Title = AppResources.AlertWindowsEmail;
          t.SortIdx = 3;
          break;
        case 9:
          i = 7;
          break;
        case 10:
          t.Title = AppResources.AlertWindowsMessage;
          t.SortIdx = 0;
          break;
      }
      if (t.Title == null)
      {
        t.Title = string.Format(AppResources.AlertAtIdx, (object) i);
        t.SortIdx = 4 + i;
      }
      return t;
    }

    public static CustomTones.Tone[] ListAlerts()
    {
      IEnumerable<CustomTones.Tone> tones = Enumerable.Range(1, 10).Select<int, CustomTones.Tone>((Func<int, CustomTones.Tone>) (i => CustomTones.FilterNumberedAlert(new CustomTones.Tone()
      {
        Path = string.Format("Sounds\\Alert-{0}.wma", (object) i.ToString().PadLeft(2, '0'))
      }, i))).Concat<CustomTones.Tone>((IEnumerable<CustomTones.Tone>) new CustomTones.Tone[1]
      {
        new CustomTones.Tone()
        {
          Title = AppResources.NoTone,
          Path = "Sounds\\Silent",
          SortIdx = -1
        }
      }).Concat<CustomTones.Tone>((IEnumerable<CustomTones.Tone>) new CustomTones.Tone[1]
      {
        new CustomTones.Tone()
        {
          Title = AppResources.AlertWindowsReminder,
          Path = "Sounds\\Alert_calendar.wma",
          SortIdx = 1
        }
      }).OrderBy<CustomTones.Tone, int>((Func<CustomTones.Tone, int>) (t => t.SortIdx)).Select<CustomTones.Tone, CustomTones.Tone>((Func<CustomTones.Tone, CustomTones.Tone>) (tone =>
      {
        tone.Label = "Windows Phone";
        return tone;
      }));
      if (DeviceStatus.DeviceManufacturer.ToLowerInvariant().Contains("nokia"))
        tones = tones.Concat<CustomTones.Tone>(((IEnumerable<CustomTones.Tone>) new CustomTones.Tone[3]
        {
          new CustomTones.Tone()
          {
            Title = "Nokia calendar",
            Path = "Sounds\\NokiaCalendar.mp3"
          },
          new CustomTones.Tone()
          {
            Title = "Nokia email",
            Path = "Sounds\\NokiaEmail.mp3"
          },
          new CustomTones.Tone()
          {
            Title = "Nokia message",
            Path = "Sounds\\NokiaMessage.mp3"
          }
        }).Select<CustomTones.Tone, CustomTones.Tone>((Func<CustomTones.Tone, CustomTones.Tone>) (tone =>
        {
          tone.Label = "NOKIA";
          return tone;
        })));
      return tones.ToArray<CustomTones.Tone>();
    }

    public static string GetNotificationSoundPath(string jid)
    {
      string soundPath = (string) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        JidInfo jidInfo = db.GetJidInfo(jid, CreateOptions.None);
        if (jidInfo == null)
          return;
        soundPath = jidInfo.NotificationSound;
      }));
      if (soundPath == null)
        soundPath = JidHelper.IsGroupJid(jid) ? Settings.GroupTone : Settings.IndividualTone;
      return soundPath;
    }

    public static string GetNotificationSoundName(string jid)
    {
      return CustomTones.GetNotificationSoundNameForPath(CustomTones.GetNotificationSoundPath(jid));
    }

    public static string GetNotificationSoundNameForPath(string filepath)
    {
      string str = (string) null;
      if (filepath != null)
      {
        CustomTones.Tone notificationSoundForPath = CustomTones.GetNotificationSoundForPath(filepath);
        if (notificationSoundForPath != null)
          str = notificationSoundForPath.Title;
      }
      return str ?? CustomTones.DefaultAlertName;
    }

    public static CustomTones.Tone GetNotificationSoundForPath(string filepath)
    {
      if (filepath == null)
        filepath = CustomTones.DefaultAlertPath;
      return ((IEnumerable<CustomTones.Tone>) CustomTones.ListAlerts()).FirstOrDefault<CustomTones.Tone>((Func<CustomTones.Tone, bool>) (t => t.Path == filepath));
    }

    public class Tone : PropChangedBase
    {
      public string Path;
      public int SortIdx;
      public string Label;

      public string Title { get; set; }

      public CustomTones.Tone Self => this;

      public void UpdateBinding() => this.NotifyPropertyChanged("Self");

      public DataTemplate Template { get; set; }
    }
  }
}
