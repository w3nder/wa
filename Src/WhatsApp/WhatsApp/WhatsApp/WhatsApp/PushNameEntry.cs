// Decompiled with JetBrains decompiler
// Type: WhatsApp.PushNameEntry
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using WhatsApp.CommonOps;


namespace WhatsApp
{
  public class PushNameEntry : PhoneApplicationPage
  {
    private IDisposable selfImageSub;
    private IDisposable selfImageFetchSub;
    private IDisposable selfImageChangedSub;
    private IDisposable silenceUiSub;
    internal ZoomBox RootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal Image Image;
    internal ProgressBar ImagePendingProgressBar;
    internal EmojiTextBox PushName;
    private bool _contentLoaded;

    public PushNameEntry()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      ContactStore.EnsureSyncRegCompleteWithRetry();
      this.silenceUiSub = AppState.MuteUIUpdates.Subscribe();
      this.RootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      InputScope inputScope = new InputScope();
      inputScope.Names.Add((object) new InputScopeName()
      {
        NameValue = InputScopeNameValue.PersonalFullName
      });
      this.PushName.Text = Settings.PushName;
      this.PushName.TextBox.InputScope = inputScope;
      this.PushName.TextBox.TextWrapping = TextWrapping.NoWrap;
      this.TitlePanel.SmallTitle = AppResources.ProfileInfoTitle;
      this.selfImageSub = ChatPictureStore.GetState(Settings.MyJid, true, true, false).SubscribeOn<ChatPictureStore.PicState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<ChatPictureStore.PicState>().Subscribe<ChatPictureStore.PicState>((Action<ChatPictureStore.PicState>) (state =>
      {
        Log.WriteLineDebug("push name entry: profile pic result is pending? {0}", (object) state.IsPending);
        this.ImagePendingProgressBar.Visibility = (state.IsPending || state.Image == null).ToVisibility();
        this.Image.Source = (System.Windows.Media.ImageSource) state.Image;
      }));
      ChatPictureStore.RequestPicture(Settings.MyJid, true, onError: (Action) (() =>
      {
        Log.WriteLineDebug("push name entry: profile pic error");
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.ImagePendingProgressBar.Visibility = Visibility.Collapsed;
          this.Image.Source = (System.Windows.Media.ImageSource) null;
        }));
      }));
      UsyncQueryRequest.SendGetStatuses((IEnumerable<FunXMPP.Connection.StatusRequest>) new FunXMPP.Connection.StatusRequest[1]
      {
        new FunXMPP.Connection.StatusRequest()
        {
          Jid = Settings.MyJid
        }
      }, FunXMPP.Connection.SyncMode.Query, FunXMPP.Connection.SyncContext.Interactive, onComplete: (Action) (() =>
      {
        string selfStatus = (string) null;
        ContactsContext.Instance((Action<ContactsContext>) (db => selfStatus = db.GetUserStatus(Settings.MyJid).Status));
        if (!string.IsNullOrEmpty(selfStatus))
          return;
        SetStatus.Set(AppResources.DefaultStatus);
      }), onError: (Action<string, int>) ((jid, err) => SetStatus.Set(AppResources.DefaultStatus)));
      this.PushName.MaxLength = 25;
      Settings.ShowPushNameScreen = false;
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
      if (e.NavigationMode == NavigationMode.Back)
        ProfilePictureChooserPage.ClearPopup();
      this.PushName.CloseEmojiKeyboard();
      base.OnNavigatingFrom(e);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      foreach (IDisposable disposable in ((IEnumerable<IDisposable>) new IDisposable[4]
      {
        this.selfImageSub,
        this.selfImageFetchSub,
        this.selfImageChangedSub,
        this.silenceUiSub
      }).Where<IDisposable>((Func<IDisposable, bool>) (d => d != null)))
        disposable.Dispose();
      base.OnRemovedFromJournal(e);
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      if (Settings.PushName.Length == 0)
      {
        string pushName = this.PushName.Text.Trim();
        if (!string.IsNullOrEmpty(pushName) && ProfilePage.FindRestrictedEmoji(pushName) > 0)
          Settings.PushName = pushName;
      }
      base.OnBackKeyPress(e);
    }

    private void OnContinueClick(object sender, EventArgs e)
    {
      string pushName = this.PushName.Text.Trim();
      if (string.IsNullOrEmpty(pushName))
        return;
      int restrictedEmoji = ProfilePage.FindRestrictedEmoji(pushName);
      if (restrictedEmoji < 0)
      {
        Settings.PushName = pushName;
        if (GdprTos.ShouldShowOnAppEntry(true))
        {
          WaUriParams uriParams = new WaUriParams();
          uriParams.AddBool("ClearStack", true);
          uriParams.AddString("Timestamp", DateTimeUtils.GetShortTimestampId(FunRunner.CurrentServerTimeUtc));
          App.CurrentApp.RootFrame.Navigate(UriUtils.CreatePageUri("GdprAgreementPage", uriParams));
        }
        else
          NavUtils.NavigateHome();
      }
      else
        ProfilePage.ShowRestrictedEmojiAlert(pushName.Substring(restrictedEmoji, 1));
    }

    private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.Image.Source != null)
      {
        System.Windows.Point point = this.Image.TransformToVisual(Application.Current.RootVisual).Transform(new System.Windows.Point(0.0, 0.0));
        ProfilePictureChooserPage.PlayEntranceAnimation(this.Image, point, point, this.Orientation, false);
      }
      IDisposable d = (IDisposable) null;
      d = ProfilePictureChooserPage.Start(Settings.MyJid, (string) null, this.Image.Source).Subscribe<ProfilePictureChooserPage.ProfilePictureChooserArgs>((Action<ProfilePictureChooserPage.ProfilePictureChooserArgs>) (args => d.SafeDispose()));
    }

    private void Background_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.PushName.CloseEmojiKeyboard();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/PushNameEntry.xaml", UriKind.Relative));
      this.RootZoomBox = (ZoomBox) this.FindName("RootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.Image = (Image) this.FindName("Image");
      this.ImagePendingProgressBar = (ProgressBar) this.FindName("ImagePendingProgressBar");
      this.PushName = (EmojiTextBox) this.FindName("PushName");
    }
  }
}
