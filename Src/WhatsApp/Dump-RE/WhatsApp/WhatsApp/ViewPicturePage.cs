// Decompiled with JetBrains decompiler
// Type: WhatsApp.ViewPicturePage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

#nullable disable
namespace WhatsApp
{
  public class ViewPicturePage : PhoneApplicationPage
  {
    private ApplicationBarIconButton AppBarSaveButton;
    private string Jid;
    public static System.Windows.Media.ImageSource _GroupThumbSource;
    private IDisposable imageSubscription;
    internal Grid LayoutRoot;
    internal Image PreviewImage;
    internal Rectangle PreviewImageShadow;
    internal TextBlock SubjectTitle;
    private bool _contentLoaded;

    public ViewPicturePage()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      foreach (ApplicationBarIconButton button in (IEnumerable) this.ApplicationBar.Buttons)
      {
        if (button != null && button.Text == AppResources.Save)
          this.AppBarSaveButton = button;
      }
      this.OrientationChanged += new EventHandler<OrientationChangedEventArgs>(this.ViewPicturePage_OrientationChanged);
      this.ViewPicturePage_OrientationChanged((object) null, new OrientationChangedEventArgs(this.Orientation));
    }

    private void ViewPicturePage_OrientationChanged(object sender, OrientationChangedEventArgs e)
    {
      if (e.Orientation.IsLandscape())
      {
        this.SubjectTitle.Margin = new Thickness(96.0, 10.0, 24.0, 24.0);
        this.SubjectTitle.Foreground = (Brush) new SolidColorBrush(Colors.White);
        this.PreviewImageShadow.Opacity = 1.0;
      }
      else
      {
        this.SubjectTitle.Margin = new Thickness(24.0, 44.0, 24.0, 44.0);
        this.SubjectTitle.Foreground = (Brush) UIUtils.ForegroundBrush;
        this.PreviewImageShadow.Opacity = 0.0;
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      bool flag = false;
      this.NavigationContext.QueryString.TryGetValue("jid", out this.Jid);
      if (this.Jid == null)
      {
        if (ViewPicturePage._GroupThumbSource != null)
        {
          this.PreviewImage.Source = ViewPicturePage._GroupThumbSource;
          ViewPicturePage._GroupThumbSource = (System.Windows.Media.ImageSource) null;
        }
      }
      else if (this.Jid.IsGroupJid())
      {
        Conversation convo = (Conversation) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => convo = db.GetConversation(this.Jid, CreateOptions.None)));
        if (convo == null)
        {
          flag = true;
        }
        else
        {
          this.SubjectTitle.Text = Emoji.ConvertToTextOnly(convo.GroupSubject ?? "", (byte[]) null).ToUpper();
          this.SetImageSource(convo.Jid);
        }
      }
      else if (this.Jid.IsUserJid())
      {
        UserStatus userStatus = ContactsContext.Instance<UserStatus>((Func<ContactsContext, UserStatus>) (db => db.GetUserStatus(this.Jid)));
        if (userStatus == null)
        {
          flag = true;
        }
        else
        {
          if (this.Jid != Settings.MyJid)
            this.SubjectTitle.Text = Emoji.ConvertToTextOnly(userStatus.GetDisplayName() ?? "", (byte[]) null).ToUpper();
          this.SetImageSource(this.Jid);
        }
      }
      base.OnNavigatedTo(e);
      if (!flag)
        return;
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack()));
    }

    private void SetImageSource(string jid)
    {
      this.imageSubscription = ChatPictureStore.Get(jid, true, false, jid != Settings.MyJid).SubscribeOn<ChatPictureStore.PicState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<ChatPictureStore.PicState>().Subscribe<ChatPictureStore.PicState>((Action<ChatPictureStore.PicState>) (picState =>
      {
        this.PreviewImage.Source = (System.Windows.Media.ImageSource) (picState.Image ?? AssetStore.GetDefaultChatIcon(jid));
        if (this.AppBarSaveButton == null)
          return;
        this.AppBarSaveButton.IsEnabled = true;
      }));
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.imageSubscription.SafeDispose();
      this.imageSubscription = (IDisposable) null;
      base.OnRemovedFromJournal(e);
    }

    private void Save_Click(object sender, EventArgs e)
    {
      this.AppBarSaveButton.IsEnabled = false;
      if (ChatPictureStore.SaveToPhone(this.Jid))
        return;
      int num = (int) MessageBox.Show(AppResources.SavePictureFailure);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ViewPicturePage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PreviewImage = (Image) this.FindName("PreviewImage");
      this.PreviewImageShadow = (Rectangle) this.FindName("PreviewImageShadow");
      this.SubjectTitle = (TextBlock) this.FindName("SubjectTitle");
    }
  }
}
