// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactVCardPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;


namespace WhatsApp
{
  public class ContactVCardPage : PhoneApplicationPage
  {
    private static string nextInstanceJid;
    private string jid;
    private IMessageListControl msgList;
    private FrameworkElement msgListElement;
    private IDisposable msgDeletedSub;
    private IDisposable msgUpdatedSub;
    private bool isPageRemoved;
    internal Grid LayoutRoot;
    internal PageTitlePanel PageTitle;
    internal ZoomBox SearchFieldZoomBox;
    internal EmojiTextBox SearchField;
    internal ZoomBox TooltipZoomBox;
    internal TextBlock TooltipBlock;
    private bool _contentLoaded;

    public ContactVCardPage()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.jid = ContactVCardPage.nextInstanceJid;
      ContactVCardPage.nextInstanceJid = (string) null;
      this.PageTitle.SmallTitle = string.Format(AppResources.ContactVCardPageTitle, (object) JidHelper.GetPhoneNumber(this.jid, true));
      this.TooltipZoomBox.ZoomFactor = this.SearchFieldZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      if (this.jid == null)
        return;
      this.InitContactVCardList();
    }

    public static void Start(string jid)
    {
      ContactVCardPage.nextInstanceJid = jid;
      NavUtils.NavigateToPage(nameof (ContactVCardPage));
    }

    private void InitContactVCardList()
    {
      MessageListControl messageListControl1 = new MessageListControl(false, false, false);
      messageListControl1.CacheMode = (CacheMode) new BitmapCache();
      messageListControl1.Margin = new Thickness(0.0, 12.0, 0.0, 0.0);
      messageListControl1.ItemTemplate = this.Resources[(object) "ContactVCardTemplate"] as DataTemplate;
      messageListControl1.EnableScrollButton = false;
      MessageListControl messageListControl2 = messageListControl1;
      this.msgList = (IMessageListControl) messageListControl2;
      this.msgListElement = (FrameworkElement) messageListControl2;
      Grid.SetRow(this.msgListElement, 1);
      this.LayoutRoot.Children.Insert(1, (UIElement) this.msgListElement);
      AppState.Worker.Enqueue((Action) (() =>
      {
        Message[] msgs = (Message[]) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          msgs = ((IEnumerable<Message>) db.GetTrustedContactVCardsWithJid(this.jid)).OrderByDescending<Message, int>((Func<Message, int>) (x => x.MessageID)).ToArray<Message>();
          this.msgDeletedSub = db.DeletedMessagesSubject.Where<Message>((Func<Message, bool>) (msg => this.jid.Equals(msg.KeyRemoteJid) && ((IEnumerable<Message>) msgs).Contains<Message>(msg))).ObserveOnDispatcher<Message>().Subscribe<Message>((Action<Message>) (deletedMsg =>
          {
            if (this.isPageRemoved)
              return;
            this.msgList.Remove(deletedMsg);
          }));
        }));
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          if (this.isPageRemoved)
          {
            this.msgDeletedSub.SafeDispose();
            this.msgUpdatedSub.SafeDispose();
            this.msgUpdatedSub = this.msgDeletedSub = (IDisposable) null;
          }
          else
          {
            if (msgs == null)
              msgs = new Message[0];
            if (!((IEnumerable<Message>) msgs).Any<Message>())
            {
              this.TooltipBlock.Text = AppResources.NoStarredMessages;
              this.TooltipZoomBox.Visibility = Visibility.Visible;
              new AppBarWrapper(this.ApplicationBar).EnableMenuItems(false);
            }
            this.msgList.SetMessages(msgs, false, new int?(), forStarredMessagesView: true, jidForContactCardView: this.jid);
          }
        }));
      }));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e) => base.OnNavigatedTo(e);

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
      base.OnNavigatingFrom(e);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.isPageRemoved = true;
      this.msgDeletedSub.SafeDispose();
      this.msgUpdatedSub.SafeDispose();
      this.msgUpdatedSub = this.msgDeletedSub = (IDisposable) null;
      base.OnRemovedFromJournal(e);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ContactVCardPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.SearchFieldZoomBox = (ZoomBox) this.FindName("SearchFieldZoomBox");
      this.SearchField = (EmojiTextBox) this.FindName("SearchField");
      this.TooltipZoomBox = (ZoomBox) this.FindName("TooltipZoomBox");
      this.TooltipBlock = (TextBlock) this.FindName("TooltipBlock");
    }
  }
}
