// Decompiled with JetBrains decompiler
// Type: WhatsApp.ProfilePage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class ProfilePage : PhoneApplicationPage
  {
    private ProfilePageViewModel viewModel_;
    private string newPushName;
    private int lastReportedIndex = -1;
    private static string[] RESTRICTED_EMOJI = new string[3]
    {
      "✔",
      "✅",
      "☑"
    };
    internal Grid LayoutRoot;
    internal WhatsApp.CompatibilityShims.LongListSelector ScrollView;
    internal Image ProfileImage;
    internal EmojiTextBox PushNameBox;
    private bool _contentLoaded;

    public ProfilePage()
    {
      this.InitializeComponent();
      this.viewModel_ = new ProfilePageViewModel(this.Orientation);
      this.DataContext = (object) this.viewModel_;
      this.PushNameBox.TextBox.InputScope = new InputScope()
      {
        Names = {
          (object) new InputScopeName()
          {
            NameValue = InputScopeNameValue.PersonalFullName
          }
        }
      };
      this.PushNameBox.Text = Settings.PushName;
      this.PushNameBox.SIPChanged += (EventHandler) ((sender, e) => this.viewModel_.IsSIPUp = this.PushNameBox.IsSIPUp);
      this.PushNameBox.TextBox.TextChanged += new TextChangedEventHandler(this.PushName_Change);
      this.PushNameBox.MaxLength = 25;
      this.ScrollView.ItemsSource = (IList) new List<object>();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      SysTrayHelper.SetOpacity((DependencyObject) this, 1.0);
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
      if (e.NavigationMode == NavigationMode.Back)
        ProfilePictureChooserPage.ClearPopup();
      this.PushNameBox.CloseEmojiKeyboard();
      base.OnNavigatingFrom(e);
      if (!(Settings.PushName != this.newPushName) || string.IsNullOrEmpty(this.newPushName))
        return;
      Settings.PushName = this.newPushName;
      AppState.GetConnection().SendAvailableForChat(true);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      base.OnRemovedFromJournal(e);
      this.PushNameBox.TextBox.TextChanged -= new TextChangedEventHandler(this.PushName_Change);
      this.viewModel_.SafeDispose();
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      string pushName = this.PushNameBox.Text.Trim();
      if (!string.IsNullOrEmpty(pushName) && Settings.PushName != pushName)
      {
        int restrictedEmoji = ProfilePage.FindRestrictedEmoji(pushName);
        if (restrictedEmoji >= 0)
        {
          ProfilePage.ShowRestrictedEmojiAlert(pushName.Substring(restrictedEmoji, 1));
          this.newPushName = (string) null;
          return;
        }
        this.newPushName = pushName;
      }
      else
        this.newPushName = (string) null;
      base.OnBackKeyPress(e);
    }

    private void ProfilePic_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.ProfileImage.Source != null)
      {
        System.Windows.Point initialPosition = this.ProfileImage.TransformToVisual(Application.Current.RootVisual).Transform(new System.Windows.Point(0.0, 0.0));
        System.Windows.Point returnPosition = initialPosition;
        if (this.Orientation.IsLandscape())
        {
          switch (ResolutionHelper.Plateau)
          {
            case ResolutionHelper.ResolutionPlateau.S10:
              returnPosition.Y = 171.0;
              break;
            case ResolutionHelper.ResolutionPlateau.S20:
              returnPosition.Y = 161.0;
              break;
            case ResolutionHelper.ResolutionPlateau.S30:
              returnPosition.Y = 138.0;
              break;
          }
        }
        ProfilePictureChooserPage.PlayEntranceAnimation(this.ProfileImage, initialPosition, returnPosition, this.Orientation, true);
      }
      SysTrayHelper.SetOpacity((DependencyObject) this, 0.0);
      IDisposable d = (IDisposable) null;
      d = ProfilePictureChooserPage.Start(Settings.MyJid, (string) null, this.viewModel_.ProfilePicSource).Subscribe<ProfilePictureChooserPage.ProfilePictureChooserArgs>((Action<ProfilePictureChooserPage.ProfilePictureChooserArgs>) (args => d.SafeDispose()));
    }

    private void EditStatus_Tap(object sender, EventArgs e)
    {
      NavUtils.NavigateToPage(this.NavigationService, "MyStatusPage");
    }

    private void Background_Tap(object sender, EventArgs e)
    {
      this.PushNameBox.CloseEmojiKeyboard();
    }

    private void PushName_Change(object sender, EventArgs e)
    {
      string pushName = this.PushNameBox.Text.Trim();
      int restrictedEmoji = ProfilePage.FindRestrictedEmoji(pushName);
      if (restrictedEmoji >= 0)
      {
        this.newPushName = (string) null;
        if (this.lastReportedIndex == restrictedEmoji)
          return;
        string str1 = pushName.Substring(restrictedEmoji, 1);
        ProfilePage.ShowRestrictedEmojiAlert(str1);
        int selectionStart = this.PushNameBox.TextBox.SelectionStart;
        string str2 = pushName.Replace(str1, "");
        this.PushNameBox.Text = str2;
        this.PushNameBox.TextBox.SelectionStart = Math.Max(0, Math.Min(str2.Length, Math.Min(selectionStart, restrictedEmoji)));
      }
      else
      {
        this.newPushName = pushName;
        this.lastReportedIndex = -1;
      }
    }

    public static int FindRestrictedEmoji(string pushName)
    {
      if (string.IsNullOrEmpty(pushName))
        return -1;
      foreach (string str in ProfilePage.RESTRICTED_EMOJI)
      {
        int restrictedEmoji = pushName.IndexOf(str);
        if (restrictedEmoji >= 0)
          return restrictedEmoji;
      }
      return -1;
    }

    public static void ShowRestrictedEmojiAlert(string restrictedEmoji)
    {
      UIUtils.ShowMessageBoxWithGeneralLearnMore(string.Format(AppResources.EmojiNotAllowed, (object) restrictedEmoji), "26000056");
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/Settings/ProfilePage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ScrollView = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ScrollView");
      this.ProfileImage = (Image) this.FindName("ProfileImage");
      this.PushNameBox = (EmojiTextBox) this.FindName("PushNameBox");
    }
  }
}
