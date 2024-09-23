// Decompiled with JetBrains decompiler
// Type: WhatsApp.LoginBanned
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace WhatsApp
{
  public class LoginBanned : PhoneApplicationPage
  {
    private IDisposable timerSub;
    private IDisposable retrySuccessSub;
    private string learnMoreUrl;
    private bool navAwayAttempted;
    internal Grid LayoutRoot;
    internal StackPanel ContentPanel;
    internal TextBlock ExplanationTextBlock;
    internal Ellipse AccentEllipse;
    internal TextBlock TimerTextBlock;
    internal Button LearnMoreButton;
    private bool _contentLoaded;

    public LoginBanned() => this.InitializeComponent();

    private static string FormatMyNumber()
    {
      return PhoneNumberFormatter.FormatInternationalNumber(Settings.ChatID);
    }

    private static string GetLearnMoreUrl(LoginBanned.BanReason? r)
    {
      int num = (int) r ?? 100;
      try
      {
        string lang;
        string locale;
        CultureInfo.CurrentUICulture.GetLangAndLocale(out lang, out locale);
        return string.Format("https://faq.whatsapp.com/{0}_{1}/general/{2}", (object) lang, (object) locale, (object) num);
      }
      catch (Exception ex)
      {
        return "https://faq.whatsapp.com/general/" + (object) num;
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (this.retrySuccessSub == null)
        this.retrySuccessSub = FunRunner.FailedLoginRetrySucceededSubject.ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ => this.NavAway()));
      int num = Settings.LoginFailed ? 1 : 0;
      string loginFailedReason = Settings.LoginFailedReason;
      string failedReasonCode = Settings.LoginFailedReasonCode;
      if (num == 0)
      {
        this.NavAway();
      }
      else
      {
        string str1 = (string) null;
        int result = 0;
        LoginBanned.BanReason? r = new LoginBanned.BanReason?();
        if (int.TryParse(loginFailedReason, out result))
        {
          r = new LoginBanned.BanReason?((LoginBanned.BanReason) result);
          switch (r.Value)
          {
            case LoginBanned.BanReason.MessagesToUnknowns:
              str1 = string.Format(AppResources.BanReasonMessagesToUnknowns, (object) LoginBanned.FormatMyNumber());
              break;
            case LoginBanned.BanReason.BlockCount:
              str1 = AppResources.BanReasonBlockCount;
              break;
            case LoginBanned.BanReason.GroupCreates:
              str1 = string.Format(AppResources.BanReasonGroupCreates, (object) LoginBanned.FormatMyNumber());
              break;
            case LoginBanned.BanReason.TooManyRecipients:
              str1 = AppResources.BanReasonTooManyRecipients;
              break;
            case LoginBanned.BanReason.TooManyToBList:
              str1 = AppResources.BanReasonTooManyToBList;
              break;
          }
        }
        this.learnMoreUrl = LoginBanned.GetLearnMoreUrl(r);
        string str2 = str1 ?? AppResources.BanReasonDefault;
        this.timerSub.SafeDispose();
        this.timerSub = (IDisposable) null;
        DateTime? banExpUtc = Settings.LoginFailedExpirationUtc;
        if (banExpUtc.HasValue)
        {
          str2 = str2 + "\n\n" + AppResources.BannedTimerExplanation;
          long totalBannedSecs = Settings.LoginFailedExpirationTotalSeconds ?? 0L;
          if (totalBannedSecs == 0L)
            this.UpdateTotalProgress(0.0);
          this.TimerTextBlock.Text = (string) null;
          this.timerSub = Observable.Timer(DateTimeOffset.Now, TimeSpan.FromSeconds(1.0)).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
          {
            DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
            if (banExpUtc.Value < currentServerTimeUtc)
            {
              this.TimerTextBlock.Text = "0:00";
              this.Unblock();
              this.NavAway();
              this.UpdateTotalProgress(1.0);
            }
            else
            {
              TimeSpan delta = banExpUtc.Value - currentServerTimeUtc;
              this.TimerTextBlock.Text = delta.ToFriendlyString();
              if (totalBannedSecs <= 0L)
                return;
              this.UpdateTotalProgress(1.0 - delta.TotalSeconds / (double) totalBannedSecs);
            }
          }));
          this.TimerTextBlock.Visibility = Visibility.Visible;
        }
        else
          this.TimerTextBlock.Visibility = Visibility.Collapsed;
        this.ExplanationTextBlock.Text = str2;
      }
    }

    private static double ThetaForPorgress(double progress)
    {
      return -(progress * 2.0 * Math.PI - 3.0 * Math.PI / 2.0);
    }

    private static IEnumerable<double> Range(double start, double end, double step)
    {
      for (; start < end; start += step)
        yield return start;
      yield return end;
    }

    private void UpdateTotalProgress(double progress)
    {
      if (progress < 0.0)
        progress = 0.0;
      else if (progress > 1.0)
        progress = 1.0;
      progress = 1.0 - progress;
      double radius = this.AccentEllipse.Width / 2.0;
      PathGeometry pathGeometry = new PathGeometry();
      PathFigure figure = new PathFigure()
      {
        StartPoint = new System.Windows.Point(radius, radius)
      };
      int num = progress > 0.005 ? (progress < 0.15 ? 1 : (int) (progress * 100.0 / 4.0)) : 0;
      foreach (System.Windows.Point point in (num != 0 ? LoginBanned.Range(0.0, progress, progress / (double) num) : (IEnumerable<double>) new double[0]).Select<double, double>((Func<double, double>) (p => LoginBanned.ThetaForPorgress(p))).Select<double, System.Windows.Point>((Func<double, System.Windows.Point>) (theta => new System.Windows.Point(Math.Cos(theta) * radius, Math.Sin(theta) * radius))).Select<System.Windows.Point, System.Windows.Point>((Func<System.Windows.Point, System.Windows.Point>) (pt => new System.Windows.Point(pt.X * 2.0, pt.Y * 2.0))).Select<System.Windows.Point, System.Windows.Point>((Func<System.Windows.Point, System.Windows.Point>) (pt => new System.Windows.Point(pt.X + figure.StartPoint.X, pt.Y + figure.StartPoint.Y))))
        figure.Segments.Add((PathSegment) new LineSegment()
        {
          Point = point
        });
      pathGeometry.Figures.Add(figure);
      this.AccentEllipse.Clip = (Geometry) pathGeometry;
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      base.OnRemovedFromJournal(e);
      this.navAwayAttempted = true;
      this.timerSub.SafeDispose();
      this.timerSub = (IDisposable) null;
      this.retrySuccessSub.SafeDispose();
      this.retrySuccessSub = (IDisposable) null;
    }

    private void NavAway()
    {
      if (this.navAwayAttempted)
        return;
      this.NavigationService.Navigate(new Uri("/PageSelect?PageReplace=true", UriKind.Relative));
      this.navAwayAttempted = true;
    }

    private void Unblock()
    {
      Settings.DeleteMany((IEnumerable<Settings.Key>) new Settings.Key[5]
      {
        Settings.Key.LoginFailedReason,
        Settings.Key.LoginFailedRetryUtc,
        Settings.Key.LoginFailedExpirationUtc,
        Settings.Key.LoginFailedExpirationTotalSeconds,
        Settings.Key.LoginFailed
      });
      App.CurrentApp.ConnectionResetSubject.OnNext(new Unit());
    }

    private void LearnMore_Click(object sender, RoutedEventArgs e)
    {
      if (this.learnMoreUrl == null)
        return;
      new WebBrowserTask()
      {
        Uri = new Uri(this.learnMoreUrl)
      }.Show();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/LoginBanned.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ContentPanel = (StackPanel) this.FindName("ContentPanel");
      this.ExplanationTextBlock = (TextBlock) this.FindName("ExplanationTextBlock");
      this.AccentEllipse = (Ellipse) this.FindName("AccentEllipse");
      this.TimerTextBlock = (TextBlock) this.FindName("TimerTextBlock");
      this.LearnMoreButton = (Button) this.FindName("LearnMoreButton");
    }

    public enum BanReason
    {
      Unknown = 100, // 0x00000064
      MessagesToUnknowns = 101, // 0x00000065
      BlockCount = 102, // 0x00000066
      GroupCreates = 103, // 0x00000067
      TooManyRecipients = 104, // 0x00000068
      TooManyToBList = 106, // 0x0000006A
    }
  }
}
