// Decompiled with JetBrains decompiler
// Type: WhatsApp.GdprAgreementPageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class GdprAgreementPageViewModel : PageViewModelBase
  {
    private const string LogHeader = "tos2";
    private bool isAcceptButtonTapped;
    private int screen = 1;
    private System.Windows.Media.ImageSource dismissButtonIcon;
    private bool isAgeConsentChecked;
    private IDisposable settingsSub;

    public bool IsAcceptButtonTapped
    {
      get => this.isAcceptButtonTapped;
      set
      {
        this.isAcceptButtonTapped = value;
        this.NotifyPropertyChanged("AgeConsentTooltipVisibility");
      }
    }

    public int Screen
    {
      get
      {
        int screen = this.screen;
        switch (screen)
        {
          case 1:
          case 2:
            return screen;
          default:
            this.Screen = screen = 1;
            goto case 1;
        }
      }
      set
      {
        int screen = this.screen;
        this.screen = value;
        if (this.screen == 2 && !Settings.GdprTosFirstSeenSecondPageUtc.HasValue)
        {
          Log.l("tos2", "2nd screen first seen");
          Settings.GdprTosFirstSeenSecondPageUtc = new DateTime?(FunRunner.CurrentServerTimeUtc);
          GdprTos.ScheduleSendGdprTosPage(2);
        }
        if (this.screen == screen)
          return;
        this.IsAcceptButtonTapped = false;
        this.Refresh(string.Format("screen changed:{0}->{1}", (object) screen, (object) this.screen));
      }
    }

    public Visibility BackgroundImageVisibility => (this.Screen == 1).ToVisibility();

    public System.Windows.Media.ImageSource BackgroundImageSource
    {
      get => (System.Windows.Media.ImageSource) AssetStore.GdprTosBackground;
    }

    public bool CanBeDismissed => Settings.GdprTosCurrentStage == 1;

    public Visibility DismissButtonVisibility => this.CanBeDismissed.ToVisibility();

    public System.Windows.Media.ImageSource DimissButtonIcon
    {
      get
      {
        if (this.dismissButtonIcon == null)
        {
          BitmapSource dismissIconWhite = AssetStore.DismissIconWhite;
          this.dismissButtonIcon = (System.Windows.Media.ImageSource) IconUtils.CreateColorIcon(dismissIconWhite, this.DimissPanelForeground, new Size?(new Size((double) dismissIconWhite.PixelWidth, (double) dismissIconWhite.PixelHeight)));
          if (this.dismissButtonIcon == null)
            AppState.Worker.RunAfterDelay(TimeSpan.FromMilliseconds(1000.0), (Action) (() => this.NotifyPropertyChanged(nameof (DimissButtonIcon))));
        }
        return this.dismissButtonIcon;
      }
    }

    public string ActionButtonStr
    {
      get => this.Screen != 2 ? AppResources.GdprTosNext : AppResources.GdprTosAgree;
    }

    public Visibility AgeConsentTooltipVisibility
    {
      get
      {
        return (this.Screen == 2 && !this.IsAgeConsentChecked && this.IsAcceptButtonTapped).ToVisibility();
      }
    }

    public string BottomTooltipStr => this.Screen != 2 ? "" : AppResources.GdprTosBottomTooltip;

    public List<RichTextBlock.TextSet> Screen1ContentSource => this.GetScreen1Content();

    public Visibility Screen1ContentVisibility => (this.Screen == 1).ToVisibility();

    public string Screen2ContentTitle => AppResources.GdprTosScreen2Title;

    public List<RichTextBlock.TextSet> Screen2ContentSource => this.GetScreen2Content();

    public Visibility Screen2ContentVisibility => (this.Screen == 2).ToVisibility();

    public Brush Screen2ContentBackground
    {
      get
      {
        return (Brush) new SolidColorBrush(ImageStore.IsDarkTheme() ? Color.FromArgb(byte.MaxValue, (byte) 31, (byte) 31, (byte) 31) : Color.FromArgb(byte.MaxValue, (byte) 245, (byte) 246, (byte) 247));
      }
    }

    public Brush Screen2ContentForeground
    {
      get
      {
        return (Brush) new SolidColorBrush(ImageStore.IsDarkTheme() ? Color.FromArgb(byte.MaxValue, (byte) 214, (byte) 214, (byte) 214) : Color.FromArgb(byte.MaxValue, (byte) 77, (byte) 77, (byte) 78));
      }
    }

    public bool IsAgeConsentChecked
    {
      get => this.isAgeConsentChecked;
      set
      {
        this.isAgeConsentChecked = value;
        this.NotifyPropertyChanged("AgeConsentTooltipVisibility");
        this.NotifyPropertyChanged(nameof (IsAgeConsentChecked));
      }
    }

    public Brush BottomPanelBackground
    {
      get
      {
        return (Brush) new SolidColorBrush(ImageStore.IsDarkTheme() ? Color.FromArgb(byte.MaxValue, (byte) 48, (byte) 48, (byte) 48) : Color.FromArgb(byte.MaxValue, (byte) 240, (byte) 241, (byte) 242));
      }
    }

    public Brush DimissPanelForeground
    {
      get
      {
        return (Brush) new SolidColorBrush(ImageStore.IsDarkTheme() ? Color.FromArgb(byte.MaxValue, (byte) 179, (byte) 179, (byte) 179) : Color.FromArgb(byte.MaxValue, (byte) 76, (byte) 76, (byte) 76));
      }
    }

    public GdprAgreementPageViewModel(PageOrientation initialOrientation)
      : base(initialOrientation)
    {
      GdprTos.CheckStageUpgrade();
      this.settingsSub = Settings.GetSettingsChangedObservable(new Settings.Key[1]
      {
        Settings.Key.GdprTosCurrentStage
      }).ObserveOnDispatcher<Settings.Key>().Subscribe<Settings.Key>((Action<Settings.Key>) (k =>
      {
        if (k != Settings.Key.GdprTosCurrentStage)
          return;
        this.Refresh("stage changed");
      }));
    }

    protected override void DisposeManagedResources()
    {
      base.DisposeManagedResources();
      this.settingsSub.SafeDispose();
      this.settingsSub = (IDisposable) null;
    }

    public void Refresh(string context)
    {
      Log.l("tos2", "vm refresh | {0}", (object) context);
      this.NotifyPropertyChanged("BackgroundImageVisibility");
      this.NotifyPropertyChanged("DismissButtonVisibility");
      this.NotifyPropertyChanged("ActionButtonStr");
      this.NotifyPropertyChanged("AgeConsentTooltipVisibility");
      this.NotifyPropertyChanged("BottomTooltipStr");
      this.NotifyPropertyChanged("Screen1ContentVisibility");
      this.NotifyPropertyChanged("Screen2ContentVisibility");
    }

    private List<RichTextBlock.TextSet> GetScreen1Content()
    {
      List<RichTextBlock.TextSet> screen1Content = new List<RichTextBlock.TextSet>();
      string str = (string) null;
      Exception e = (Exception) null;
      switch (Settings.GdprTosCurrentStage)
      {
        case 1:
          DateTime? tosStage1StartUtc = Settings.GdprTosStage1StartUtc;
          int stage1Hours = 0;
          int stage2Hours = 0;
          if (GdprTos.GetStageDurations(out stage1Hours, out stage2Hours) && stage1Hours >= 0 && tosStage1StartUtc.HasValue)
          {
            str = string.Format(AppResources.GdprTosScreen1Stage1Body, (object) DateTimeUtils.FunTimeToPhoneTime(tosStage1StartUtc.Value + TimeSpan.FromHours((double) stage1Hours)).ToMonthDayString());
            break;
          }
          e = (Exception) new ArgumentException("invalid accept deadline");
          Log.SendCrashLog(e, string.Format("s1 start:{0},s1 hrs:{1}", (object) (tosStage1StartUtc?.ToString() ?? "n/a"), (object) stage1Hours));
          break;
        case 2:
        case 3:
          str = AppResources.GdprTosScreen1Stage2Body;
          break;
        default:
          e = (Exception) new ArgumentException("invalid gdpr tos screen 1 body");
          Log.SendCrashLog(e, str);
          break;
      }
      if (e != null)
        throw e;
      RichTextBlock.TextSet textSet = this.FormatText(str, new KeyValuePair<WaRichText.Formats, string>[3]
      {
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Link, WaWebUrls.GdprTosUrlTerms),
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Link, WaWebUrls.GdprTosUrlPrivacyPolicy),
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Link, WaWebUrls.GdprTosUrlTerms)
      });
      screen1Content.Add(textSet);
      return screen1Content;
    }

    private List<RichTextBlock.TextSet> GetScreen2Content()
    {
      List<RichTextBlock.TextSet> screen2Content = new List<RichTextBlock.TextSet>();
      string text1 = AppResources.GdprTosScreen2BodyPart1 + "\n";
      KeyValuePair<WaRichText.Formats, string>[] formattings1 = new KeyValuePair<WaRichText.Formats, string>[3]
      {
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Link, WaWebUrls.GdprTosUrlTerms),
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Link, WaWebUrls.GdprTosUrlPrivacyPolicy),
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Link, WaWebUrls.GdprTosUrlGroupE2e)
      };
      screen2Content.Add(this.FormatText(text1, formattings1));
      string text2 = AppResources.GdprTosScreen2BodyPart2 + "\n";
      KeyValuePair<WaRichText.Formats, string>[] formattings2 = new KeyValuePair<WaRichText.Formats, string>[2]
      {
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Bold | WaRichText.Formats.Foreground, UIUtils.ForegroundColorCode),
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Link, WaWebUrls.GdprTosUrlAge)
      };
      screen2Content.Add(this.FormatText(text2, formattings2));
      string text3 = AppResources.GdprTosScreen2BodyPart3 + "\n";
      KeyValuePair<WaRichText.Formats, string>[] formattings3 = new KeyValuePair<WaRichText.Formats, string>[2]
      {
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Bold | WaRichText.Formats.Foreground, UIUtils.ForegroundColorCode),
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Link, WaWebUrls.FbUrlFacebookCompanies)
      };
      screen2Content.Add(this.FormatText(text3, formattings3));
      string text4 = AppResources.GdprTosScreen2BodyPart4 + "\n";
      KeyValuePair<WaRichText.Formats, string>[] formattings4 = (KeyValuePair<WaRichText.Formats, string>[]) null;
      screen2Content.Add(this.FormatText(text4, formattings4));
      string text5 = AppResources.GdprTosScreen2BodyPart5 + "\n";
      KeyValuePair<WaRichText.Formats, string>[] formattings5 = new KeyValuePair<WaRichText.Formats, string>[1]
      {
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Link, WaWebUrls.GdprTosUrlWorkingWithFb)
      };
      screen2Content.Add(this.FormatText(text5, formattings5));
      string text6 = AppResources.GdprTosScreen2BodyPart6 + "\n";
      KeyValuePair<WaRichText.Formats, string>[] formattings6 = new KeyValuePair<WaRichText.Formats, string>[3]
      {
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Bold | WaRichText.Formats.Foreground, UIUtils.ForegroundColorCode),
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Link, WaWebUrls.GdprTosUrlTerms),
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Link, WaWebUrls.GdprTosUrlManageAndDeleteInfo)
      };
      screen2Content.Add(this.FormatText(text6, formattings6));
      string text7 = AppResources.GdprTosScreen2BodyPart7 + "\n";
      KeyValuePair<WaRichText.Formats, string>[] formattings7 = new KeyValuePair<WaRichText.Formats, string>[2]
      {
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Bold | WaRichText.Formats.Foreground, UIUtils.ForegroundColorCode),
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Link, WaWebUrls.GdprTosUrlGlobalOp)
      };
      screen2Content.Add(this.FormatText(text7, formattings7));
      string text8 = AppResources.GdprTosScreen2BodyPart8 + "\n";
      KeyValuePair<WaRichText.Formats, string>[] formattings8 = new KeyValuePair<WaRichText.Formats, string>[1]
      {
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Bold | WaRichText.Formats.Foreground, UIUtils.ForegroundColorCode)
      };
      screen2Content.Add(this.FormatText(text8, formattings8));
      string text9 = AppResources.GdprTosScreen2BodyPart9 + "\n";
      KeyValuePair<WaRichText.Formats, string>[] formattings9 = new KeyValuePair<WaRichText.Formats, string>[3]
      {
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Link, WaWebUrls.GdprTosUrlTerms),
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Link, WaWebUrls.GdprTosUrlPrivacyPolicy),
        new KeyValuePair<WaRichText.Formats, string>(WaRichText.Formats.Link, WaWebUrls.GdprTosUrlCookies)
      };
      screen2Content.Add(this.FormatText(text9, formattings9));
      return screen2Content;
    }

    private RichTextBlock.TextSet FormatText(
      string text,
      KeyValuePair<WaRichText.Formats, string>[] formattings)
    {
      return new RichTextBlock.TextSet()
      {
        Text = text,
        PartialFormattings = (IEnumerable<WaRichText.Chunk>) GdprTos.GetRichTextFormattings(text, formattings)
      };
    }
  }
}
