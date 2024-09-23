// Decompiled with JetBrains decompiler
// Type: WhatsApp.CallRatingPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class CallRatingPage : PhoneApplicationPage
  {
    private const string LogHeader = "callrating";
    private static byte[] NextInstanceRatingCookie;
    private static string NextInstancePeerJid;
    private static bool NextInstanceShowName;
    private CallRatingPageViewModel viewModel;
    private ApplicationBarIconButton submitButton;
    private byte[] ratingCookie;
    private bool ratingSubmitted;
    private IDisposable propChangeSub;
    internal Grid LayoutRoot;
    internal PhoneTextBox FeedbackBox;
    private bool _contentLoaded;

    public CallRatingPage()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.ratingCookie = CallRatingPage.NextInstanceRatingCookie;
      CallRatingPage.NextInstanceRatingCookie = (byte[]) null;
      this.submitButton = this.ApplicationBar.Buttons[0] as ApplicationBarIconButton;
      string nextInstancePeerJid = CallRatingPage.NextInstancePeerJid;
      bool instanceShowName = CallRatingPage.NextInstanceShowName;
      CallRatingPage.NextInstancePeerJid = (string) null;
      CallRatingPage.NextInstanceShowName = false;
      this.DataContext = (object) (this.viewModel = new CallRatingPageViewModel(nextInstancePeerJid, instanceShowName, this.Orientation));
      this.propChangeSub = this.viewModel.GetPropertyChangedAsync().Where<PropertyChangedEventArgs>((Func<PropertyChangedEventArgs, bool>) (args => args.PropertyName == "RatingValue")).ObserveOnDispatcher<PropertyChangedEventArgs>().Subscribe<PropertyChangedEventArgs>((Action<PropertyChangedEventArgs>) (_ =>
      {
        int ratingValue = this.viewModel.RatingValue;
        this.submitButton.IsEnabled = ratingValue > 0 && ratingValue < 6;
      }));
    }

    public static void Start(string peerJid, byte[] fsCookie, bool replacePage, bool showName)
    {
      CallRatingPage.NextInstanceRatingCookie = fsCookie;
      CallRatingPage.NextInstancePeerJid = peerJid;
      CallRatingPage.NextInstanceShowName = showName;
      WaUriParams uriParams = new WaUriParams();
      if (replacePage)
        uriParams.AddBool("PageReplace", true);
      Deployment.Current.Dispatcher.BeginInvoke((Action) (() => NavUtils.NavigateToPage(nameof (CallRatingPage), uriParams)));
    }

    private void TrySubmitRating(int rating, string feedback = null)
    {
      byte[] cookie = this.ratingCookie;
      if (this.ratingSubmitted || cookie == null)
        return;
      this.ratingSubmitted = true;
      string peerJid = this.viewModel.PeerJid;
      Log.l("callrating", "submit fs | peer:{0},rate:{1},feedback:{2}", (object) (peerJid ?? "n/a"), (object) rating, (object) feedback);
      FieldStatsRunner.FieldStatsAction((Action<IFieldStats>) (fs =>
      {
        if (rating >= 0 && rating < 6)
        {
          fs.SubmitVoipRating(cookie, rating, feedback ?? "");
          Settings.LastShowCallRatingTimeUtc = new DateTime?(DateTime.UtcNow);
        }
        else
          fs.SubmitVoipNullRating(cookie);
        Log.l("callrating", "submit fs | complete | peer:{0}", (object) peerJid);
      }));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (this.NavigationService.SearchBackStack("CallScreenPage") >= 0)
      {
        Log.l("callrating", "found call screen in back stack | clear everything..");
        NavUtils.ClearBackStack();
      }
      if (!this.ratingSubmitted && this.ratingCookie != null)
        return;
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      this.TrySubmitRating(-1);
      base.OnNavigatedFrom(e);
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      this.TrySubmitRating(0);
      base.OnBackKeyPress(e);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      base.OnRemovedFromJournal(e);
      this.viewModel.SafeDispose();
      this.propChangeSub.SafeDispose();
      this.propChangeSub = (IDisposable) null;
    }

    private void DismissButton_Click(object sender, EventArgs e)
    {
      this.TrySubmitRating(0);
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    private void SubmitButton_Click(object sender, EventArgs e)
    {
      this.TrySubmitRating(this.viewModel.RatingValue, (this.FeedbackBox.Text ?? "").Trim());
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    private void ViewCallSettings_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      NavUtils.NavigateToPage(this.NavigationService, "ChatSettingsPage", folderName: "Pages/Settings");
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/CallRatingPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.FeedbackBox = (PhoneTextBox) this.FindName("FeedbackBox");
    }
  }
}
