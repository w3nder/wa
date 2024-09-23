// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatMediaPage
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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WhatsApp.CommonOps;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;


namespace WhatsApp
{
  public class ChatMediaPage : PhoneApplicationPage
  {
    private static Message nextInstanceStartingMsg_;
    private static string nextInstanceChatJid_;
    private Message startingMsg_;
    private ChatMediaPage.Item startingItem_;
    private string targetChatJid_;
    private List<ChatMediaPage.Item> items_;
    private List<ChatMediaPage.Item> pendingItems_;
    private bool inited_;
    private bool pageRemoved_;
    private bool isMenuOpened;
    private int currIndex_;
    private PageOrientation? orientationToKeep_;
    private bool showInfoOverlay_ = true;
    private Storyboard infoOverlayFadingSb_;
    private DoubleAnimation infoOverlayFadingAnimation_;
    private IDisposable infoOverlayFadingSbSub_;
    private IDisposable updateMessageWaTypeSub;
    private int currInfoOverlayTarget_ = -1;
    private bool isOverlayPreChanged_;
    private bool shareSet;
    internal Grid LayoutRoot;
    internal ImageSlideViewControl SliderView;
    internal Grid InfoOverlayPanel;
    internal TextBlock IndexBlock;
    internal TextTrimmingControl2 TitleBlock;
    internal RichTextBlock CaptionBlock;
    internal TextBlock TimestampBlock;
    internal RoundButton PlayButton;
    internal Image OverflowDots;
    internal ContextMenu Menu;
    private bool _contentLoaded;

    private ChatMediaPage.Item StartingItem
    {
      get
      {
        return this.startingItem_ ?? (this.startingItem_ = this.startingMsg_ == null ? (ChatMediaPage.Item) null : new ChatMediaPage.Item(this.startingMsg_));
      }
    }

    private ChatMediaPage.Item CurrentItem
    {
      get
      {
        return this.items_ != null ? this.items_.ElementAtOrDefault<ChatMediaPage.Item>(this.currIndex_) : this.StartingItem;
      }
    }

    public ChatMediaPage()
    {
      this.InitializeComponent();
      this.startingMsg_ = ChatMediaPage.nextInstanceStartingMsg_;
      ChatMediaPage.nextInstanceStartingMsg_ = (Message) null;
      this.targetChatJid_ = ChatMediaPage.nextInstanceChatJid_;
      ChatMediaPage.nextInstanceChatJid_ = (string) null;
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.SliderView.ImageSwitched += new ImageSlideViewControl.ImageSwitchedHandler(this.OnItemSwitched);
      this.SliderView.ImageSwitching += new ImageSlideViewControl.ImageSwitchingHandler(this.OnItemSwitching);
      this.SliderView.CenterImage.ImagePinchStarted += new EventHandler(this.OnCurrentImagePinchStarted);
      this.SliderView.CenterImage.ImageManipulationEnded += new EventHandler(this.OnCurrentImageManipulationEnded);
      this.PlayButton.ButtonIcon = (BitmapSource) ImageStore.GetStockIcon("/Images/play.png");
      this.PlayButton.ButtonIconReversed = (BitmapSource) ImageStore.GetStockIcon("/Images/play-active.png");
      if (this.updateMessageWaTypeSub != null)
        return;
      this.updateMessageWaTypeSub = MessagesContext.Events.UpdatedMessagesMediaWaTypeSubject.ObserveOnDispatcher<Message>().Subscribe<Message>(new Action<Message>(this.OnMessageWaMediaTypeUpdate));
    }

    private static Message[] FetchMediaMessages(string jid)
    {
      Message[] msgs = (Message[]) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => msgs = db.GetSavedMediaMessages(new string[1]
      {
        jid
      }, new FunXMPP.FMessage.Type[4]
      {
        FunXMPP.FMessage.Type.Image,
        FunXMPP.FMessage.Type.Video,
        FunXMPP.FMessage.Type.Audio,
        FunXMPP.FMessage.Type.Gif
      }, true, true)));
      return msgs;
    }

    public static void Start(Message startingMsg, string[] jids, string targetChatJid = null)
    {
      ChatMediaPage.nextInstanceStartingMsg_ = startingMsg;
      if (targetChatJid == null && jids != null)
        targetChatJid = ((IEnumerable<string>) jids).FirstOrDefault<string>();
      ChatMediaPage.nextInstanceChatJid_ = targetChatJid;
      WaUriParams uriParams = new WaUriParams();
      uriParams.AddInt("starting_msg", startingMsg.MessageID);
      uriParams.AddString("target_jid", targetChatJid);
      if (jids != null && jids.Length != 0)
        uriParams.AddString(nameof (jids), string.Join(",", jids));
      Deployment.Current.Dispatcher.BeginInvoke((Action) (() => NavUtils.NavigateToPage(nameof (ChatMediaPage), uriParams)));
    }

    public static IObservable<bool> GetDeleteConfirmationObs(Message m)
    {
      string title;
      string message;
      switch (m.MediaWaType)
      {
        case FunXMPP.FMessage.Type.Image:
        case FunXMPP.FMessage.Type.Gif:
          title = AppResources.DeletePhotoConfirmTitle;
          message = AppResources.DeletePhotoConfirmBody;
          break;
        case FunXMPP.FMessage.Type.Audio:
          title = AppResources.DeleteAudioConfirmTitle;
          message = AppResources.DeleteAudioConfirmBody;
          break;
        case FunXMPP.FMessage.Type.Video:
          title = AppResources.DeleteVideoConfirmTitle;
          message = AppResources.DeleteVideoConfirmBody;
          break;
        default:
          title = (string) null;
          message = AppResources.DeleteMediaConfirm;
          break;
      }
      return Observable.Return<bool>(true).Decision(message, AppResources.Delete, AppResources.Cancel, title);
    }

    private void SetItems(List<ChatMediaPage.Item> items)
    {
      this.items_ = items;
      if (this.items_ == null)
        return;
      int num = 0;
      bool flag = false;
      foreach (ChatMediaPage.Item obj in this.items_)
      {
        if (obj.Message.MessageID == this.StartingItem.Message.MessageID)
        {
          flag = true;
          break;
        }
        ++num;
      }
      if (!flag)
        return;
      ChatMediaPage.Item obj1 = this.items_.ElementAtOrDefault<ChatMediaPage.Item>(num);
      if (obj1 == null)
        return;
      this.currIndex_ = num;
      this.UpdateForItem(obj1, num, true);
      this.SetPrevious(this.items_.ElementAtOrDefault<ChatMediaPage.Item>(num - 1), (BitmapSource) null, true);
      this.SetNext(this.items_.ElementAtOrDefault<ChatMediaPage.Item>(num + 1), (BitmapSource) null, true);
    }

    private void SetCurrent(ChatMediaPage.Item item, BitmapSource knownImgSrc)
    {
      if (knownImgSrc == null && item != null)
        this.SliderView.SetCenterImageSource(item.GetBitmapObservable(), true);
      else
        this.SliderView.SetCenterImageSource(knownImgSrc);
    }

    private void SetPrevious(ChatMediaPage.Item item, BitmapSource knownImgSrc, bool async)
    {
      if (knownImgSrc == null && item != null)
        this.SliderView.SetLeftImageSource(item.GetBitmapObservable(), async);
      else
        this.SliderView.SetLeftImageSource(knownImgSrc);
    }

    private void SetNext(ChatMediaPage.Item item, BitmapSource knownImgSrc, bool async)
    {
      if (knownImgSrc == null && item != null)
        this.SliderView.SetRightImageSource(item.GetBitmapObservable(), async);
      else
        this.SliderView.SetRightImageSource(knownImgSrc);
    }

    private void UpdateForItem(ChatMediaPage.Item item, int itemIndex, bool updateInfoOverlay)
    {
      if (item == null)
        return;
      if (updateInfoOverlay)
        this.UpdateInfoOverlay(item, itemIndex, true, true, false, true);
      this.UpdateActionOverlay(item, true);
      this.SliderView.CenterImage.EnableScaling = item.Message != null && item.Message.MediaWaType == FunXMPP.FMessage.Type.Image;
    }

    private void UpdateActionOverlay(ChatMediaPage.Item item, bool show)
    {
      this.PlayButton.Visibility = (show && item != null && item.Message != null && (item.Message.MediaWaType == FunXMPP.FMessage.Type.Video || item.Message.MediaWaType == FunXMPP.FMessage.Type.Audio || item.Message.MediaWaType == FunXMPP.FMessage.Type.Gif)).ToVisibility();
    }

    private void UpdateInfoOverlay(
      ChatMediaPage.Item item,
      int itemIndex,
      bool show,
      bool updateFooter,
      bool animate,
      bool force)
    {
      if (item != null & show && this.showInfoOverlay_ && !this.SliderView.CenterImage.IsImageScaled)
      {
        if (this.currInfoOverlayTarget_ == itemIndex && !force && (this.items_ == null || this.items_.Count != 1))
          return;
        this.currInfoOverlayTarget_ = itemIndex;
        this.IndexBlock.Text = this.items_ == null ? "" : string.Format(AppResources.NthOutOfTotal, (object) (itemIndex + 1), (object) this.items_.Count).ToUpper();
        if (updateFooter && item.Message != null)
        {
          this.TimestampBlock.Text = item.GetFooterStr(this.targetChatJid_);
          List<string> mentionedJids = item.Message.GetMentionedJids();
          string str = !mentionedJids.Any<string>() ? item.Message.MediaCaption : WaRichText.FormatMentions(item.Message.MediaCaption, mentionedJids);
          this.CaptionBlock.Text = new RichTextBlock.TextSet()
          {
            Text = str
          };
        }
        this.ShowInfoOverlay(true, animate);
      }
      else
        this.ShowInfoOverlay(false, animate);
    }

    private void ShowInfoOverlay(bool show, bool animate)
    {
      this.infoOverlayFadingSbSub_.SafeDispose();
      this.infoOverlayFadingSbSub_ = (IDisposable) null;
      if (!animate)
      {
        this.InfoOverlayPanel.Opacity = show ? 1.0 : 0.0;
        this.InfoOverlayPanel.Visibility = show.ToVisibility();
      }
      if (show.ToVisibility() == this.InfoOverlayPanel.Visibility)
        return;
      if (this.infoOverlayFadingSb_ == null || this.infoOverlayFadingAnimation_ == null)
      {
        DoubleAnimation doubleAnimation = new DoubleAnimation();
        doubleAnimation.Duration = (Duration) TimeSpan.FromMilliseconds(250.0);
        DoubleAnimation element = doubleAnimation;
        Storyboard.SetTargetProperty((Timeline) element, new PropertyPath("Opacity", new object[0]));
        Storyboard.SetTarget((Timeline) element, (DependencyObject) this.InfoOverlayPanel);
        Storyboard storyboard = new Storyboard();
        storyboard.Children.Add((Timeline) element);
        this.infoOverlayFadingSb_ = storyboard;
        this.infoOverlayFadingAnimation_ = element;
      }
      this.infoOverlayFadingAnimation_.From = new double?(this.InfoOverlayPanel.Opacity);
      this.infoOverlayFadingAnimation_.To = new double?(show ? 1.0 : 0.0);
      this.InfoOverlayPanel.Visibility = Visibility.Visible;
      this.infoOverlayFadingSbSub_ = Storyboarder.PerformWithDisposable(this.infoOverlayFadingSb_, (DependencyObject) null, true, (Action) (() =>
      {
        this.InfoOverlayPanel.Opacity = show ? 1.0 : 0.0;
        this.InfoOverlayPanel.Visibility = show.ToVisibility();
      }), false, "media page: info overlay fading");
    }

    private void OpenMenu()
    {
      ContextMenu contextMenu = ContextMenuService.GetContextMenu((DependencyObject) this.OverflowDots);
      if (contextMenu.Parent != null || contextMenu.IsOpen)
        return;
      contextMenu.IsOpen = true;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (this.inited_)
        return;
      if (this.startingMsg_ == null)
      {
        Log.WriteLineDebug("media page: no static args | fallback to parse uri");
        IDictionary<string, string> queryString = this.NavigationContext.QueryString;
        string s = (string) null;
        int startingMsgId = -1;
        string str = (string) null;
        if (queryString.TryGetValue("jids", out str) && queryString.TryGetValue("starting_msg", out s) && int.TryParse(s, out startingMsgId))
        {
          string[] source = str.Split(",".ToArray<char>());
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => this.startingMsg_ = db.GetMessageById(startingMsgId)));
          queryString.TryGetValue("target_jid", out this.targetChatJid_);
          if (this.targetChatJid_ == null)
            this.targetChatJid_ = ((IEnumerable<string>) source).FirstOrDefault<string>();
        }
        Log.WriteLineDebug("media page: parse uri successful");
      }
      if (this.startingMsg_ == null)
      {
        Log.WriteLineDebug("media page: abort | argument missing");
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
      }
      else
        WAThreadPool.QueueUserWorkItem((Action) (() => this.OnMessagesUpdated(ChatMediaPage.FetchMediaMessages(this.targetChatJid_))));
    }

    private void OnMessagesUpdated(Message[] msgs)
    {
      Log.WriteLineDebug("media page: fetched {0} media msgs for {1}", (object) msgs.Length, (object) this.targetChatJid_);
      List<ChatMediaPage.Item> items = ((IEnumerable<Message>) msgs).Select<Message, ChatMediaPage.Item>((Func<Message, ChatMediaPage.Item>) (m => new ChatMediaPage.Item(m))).ToList<ChatMediaPage.Item>();
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (this.pageRemoved_)
          return;
        if (this.inited_)
        {
          Log.p("media page", "load fetched items");
          this.SetItems(items);
        }
        else
          this.pendingItems_ = items;
      }));
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.pageRemoved_ = true;
      this.updateMessageWaTypeSub.SafeDispose();
      base.OnRemovedFromJournal(e);
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      if (!this.isMenuOpened)
        Storyboarder.Perform(WaAnimations.PageTransition(PageTransitionAnimation.SlideDownFadeOut), (DependencyObject) this.LayoutRoot);
      base.OnBackKeyPress(e);
    }

    protected override void OnOrientationChanged(OrientationChangedEventArgs e)
    {
      if (this.orientationToKeep_.HasValue)
      {
        PageOrientation pageOrientation = this.orientationToKeep_.Value;
        this.orientationToKeep_ = new PageOrientation?();
        this.Orientation = pageOrientation;
      }
      else
      {
        this.SliderView.Orientation = e.Orientation;
        base.OnOrientationChanged(e);
      }
    }

    private void OnLoaded(object sender, EventArgs e)
    {
      if (this.inited_)
        return;
      this.inited_ = true;
      if (this.pendingItems_ != null)
      {
        Log.p("media page", "load pending items");
        this.SetItems(this.pendingItems_);
        this.pendingItems_ = (List<ChatMediaPage.Item>) null;
      }
      if (this.StartingItem != null)
      {
        this.SliderView.SetCenterImageSource(this.StartingItem.GetBitmapObservable(), true);
        this.UpdateInfoOverlay(this.CurrentItem, this.currIndex_, true, true, false, true);
      }
      if (this.targetChatJid_ == null || JidHelper.IsUserJid(this.targetChatJid_))
        return;
      AppState.Worker.Enqueue((Action) (() =>
      {
        Conversation convo = (Conversation) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => convo = db.GetConversation(this.targetChatJid_, CreateOptions.None)));
        if (convo == null)
          return;
        string title = convo.GetName(true);
        if (string.IsNullOrEmpty(title))
          return;
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.TitleBlock.Text = title.ToUpper();
          this.TitleBlock.Mode = TextTrimmingControl2.TextTrimmingMode.Ellipses;
        }));
      }));
    }

    private void OnCurrentImageManipulationEnded(object sender, EventArgs e)
    {
      if (this.isOverlayPreChanged_)
        return;
      this.UpdateInfoOverlay(this.CurrentItem, this.currIndex_, this.showInfoOverlay_, true, true, true);
    }

    private void OnCurrentImagePinchStarted(object sender, EventArgs e)
    {
      this.UpdateInfoOverlay(this.CurrentItem, this.currIndex_, false, false, false, false);
    }

    private void OnItemSwitching(int direction, double percentage)
    {
      if (direction == 0 || this.items_ == null)
        return;
      this.UpdateActionOverlay(this.CurrentItem, false);
      if (this.InfoOverlayPanel.Visibility != Visibility.Visible)
        return;
      if (this.isOverlayPreChanged_)
      {
        if (percentage >= 0.5)
          return;
        this.isOverlayPreChanged_ = false;
        this.UpdateInfoOverlay(this.CurrentItem, this.currIndex_, true, true, false, false);
      }
      else
      {
        if (percentage <= 0.5)
          return;
        this.isOverlayPreChanged_ = true;
        int num = Math.Max(0, Math.Min(this.items_.Count - 1, this.currIndex_ + (direction < 0 ? -1 : 1)));
        this.UpdateInfoOverlay(this.items_.ElementAtOrDefault<ChatMediaPage.Item>(num), num, true, true, false, false);
      }
    }

    private void OnItemSwitched(int direction)
    {
      if (this.items_ == null)
        return;
      if (direction != 0)
      {
        int num = Math.Max(0, Math.Min(this.items_.Count - 1, this.currIndex_ + (direction < 0 ? -1 : 1)));
        if (direction > 0)
          this.SetNext(this.items_.ElementAtOrDefault<ChatMediaPage.Item>(num + 1), (BitmapSource) null, false);
        else
          this.SetPrevious(this.items_.ElementAtOrDefault<ChatMediaPage.Item>(num - 1), (BitmapSource) null, false);
        this.currIndex_ = num;
      }
      this.UpdateForItem(this.CurrentItem, this.currIndex_, !this.isOverlayPreChanged_);
      this.isOverlayPreChanged_ = false;
    }

    private void Play_Click(object sender, EventArgs e)
    {
      ChatMediaPage.Item currentItem = this.CurrentItem;
      if (currentItem == null || currentItem.Message == null || currentItem.Message.MediaWaType != FunXMPP.FMessage.Type.Video && currentItem.Message.MediaWaType != FunXMPP.FMessage.Type.Audio && currentItem.Message.MediaWaType != FunXMPP.FMessage.Type.Gif)
        return;
      if (!this.Orientation.IsLandscape())
        this.orientationToKeep_ = new PageOrientation?(this.Orientation);
      ViewMessage.View(currentItem.Message);
    }

    private void Save_Click(object sender, EventArgs e)
    {
      ChatMediaPage.Item item = this.CurrentItem;
      if (item == null || item.Message == null || !item.Message.CanSaveMedia())
        return;
      bool saved = false;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => saved = item.Message.CopyMediaToAlbum(db, "Saved Pictures")));
      int num;
      this.Dispatcher.BeginInvoke((Action) (() => num = (int) MessageBox.Show(!saved ? (item.Message.MediaWaType == FunXMPP.FMessage.Type.Video ? AppResources.SaveVideoFailure : AppResources.SavePictureFailure) : string.Format(AppResources.SaveMediaSuccess, (object) AppResources.AlbumTileSavedPictures))));
    }

    private void Star_Click(object sender, EventArgs e)
    {
      ChatMediaPage.Item currentItem = this.CurrentItem;
      if (currentItem == null || currentItem.Message == null)
        return;
      Message msg = currentItem.Message;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        msg.IsStarred = !msg.IsStarred;
        db.SubmitChanges();
      }));
      this.TimestampBlock.Text = currentItem.GetFooterStr(this.targetChatJid_, true);
      StarChange.NotifyQrStarChange(msg);
    }

    private void Forward_Click(object sender, EventArgs e)
    {
      ChatMediaPage.Item currentItem = this.CurrentItem;
      if (currentItem == null || currentItem.Message == null)
        return;
      SendMessage.ChooseRecipientAndForwardExisting(new Message[1]
      {
        currentItem.Message
      });
    }

    private void AssignToGroup_Click(object sender, EventArgs e)
    {
      if (!JidHelper.IsGroupJid(this.targetChatJid_))
        return;
      BitmapSource centerImageSource = this.SliderView.CenterImageSource;
      string targetChatJid = this.targetChatJid_;
      if (!(centerImageSource is WriteableBitmap bitmap))
        bitmap = new WriteableBitmap(centerImageSource);
      SetChatPhoto.CropAndSet(targetChatJid, bitmap);
    }

    private void SetAsProfilePic_Click(object sender, EventArgs e)
    {
      BitmapSource centerImageSource = this.SliderView.CenterImageSource;
      string myJid = Settings.MyJid;
      if (!(centerImageSource is WriteableBitmap bitmap))
        bitmap = new WriteableBitmap(centerImageSource);
      SetChatPhoto.CropAndSet(myJid, bitmap);
    }

    private void Delete_Click(object sender, EventArgs e)
    {
      ChatMediaPage.Item currentItem = this.CurrentItem;
      if (currentItem == null || currentItem.Message == null)
        return;
      Message msg = currentItem.Message;
      ChatMediaPage.GetDeleteConfirmationObs(msg).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (accept =>
      {
        if (!accept)
          return;
        this.RemoveCurrentMedia(msg);
      }));
    }

    private void RemoveCurrentMedia(Message msgToDelete = null)
    {
      if (this.items_ != null)
      {
        this.items_.RemoveAt(this.currIndex_);
        if (this.items_.Count > 0)
          this.SliderView.DeleteCurrent().Subscribe<int>((Action<int>) (d =>
          {
            this.currInfoOverlayTarget_ = -1;
            if (d < 0)
            {
              --this.currIndex_;
              this.SetPrevious(this.items_.ElementAtOrDefault<ChatMediaPage.Item>(this.currIndex_ - 1), (BitmapSource) null, true);
            }
            else if (d > 0)
              this.SetNext(this.items_.ElementAtOrDefault<ChatMediaPage.Item>(this.currIndex_ + 1), (BitmapSource) null, true);
            this.UpdateForItem(this.items_.ElementAtOrDefault<ChatMediaPage.Item>(this.currIndex_), this.currIndex_, true);
          }));
      }
      if (msgToDelete != null)
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.DeleteMessage(msgToDelete)));
      if (this.items_ != null && this.items_.Count != 0)
        return;
      this.NavigationService.JumpBackTo("ChatPage", fallbackToHome: true);
    }

    private void OnMessageWaMediaTypeUpdate(Message m)
    {
      if (!m.IsRevoked() || this.items_ == null)
        return;
      if (this.CurrentItem?.Message == m)
      {
        Log.l("media page", "current media revoked");
        this.RemoveCurrentMedia();
      }
      else if (m.KeyRemoteJid == this.targetChatJid_)
      {
        Log.l("media page", "revoked media is not the current media, but is in the list");
        WAThreadPool.QueueUserWorkItem((Action) (() => this.OnMessagesUpdated(ChatMediaPage.FetchMediaMessages(this.targetChatJid_))));
      }
      else
        Log.l("media page", "revoked media is not in the list");
    }

    private void Share_Click(object sender, EventArgs e)
    {
      if (!this.shareSet)
      {
        DataTransferManager forCurrentView = DataTransferManager.GetForCurrentView();
        // ISSUE: method pointer
        WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<DataTransferManager, DataRequestedEventArgs>>(new Func<TypedEventHandler<DataTransferManager, DataRequestedEventArgs>, EventRegistrationToken>(forCurrentView.add_DataRequested), new Action<EventRegistrationToken>(forCurrentView.remove_DataRequested), new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>((object) this, __methodptr(ShareTextHandler)));
        this.shareSet = true;
      }
      DataTransferManager.ShowShareUI();
    }

    private async void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
    {
      ChatMediaPage.Item currentItem = this.CurrentItem;
      if (currentItem == null || currentItem.Message == null)
        return;
      Message message = currentItem.Message;
      DataRequest request = e.Request;
      DataRequestDeferral deferral = request.GetDeferral();
      try
      {
        request.Data.Properties.put_Title(AppResources.Media);
        StorageFile fileFromPathAsync = await StorageFile.GetFileFromPathAsync(MediaStorage.GetAbsolutePath(message.LocalFileUri));
        if (fileFromPathAsync != null)
          request.Data.SetStorageItems((IEnumerable<IStorageItem>) new StorageFile[1]
          {
            fileFromPathAsync
          });
        else
          request.FailWithDisplayText(AppResources.CouldNotOpenMediaFile);
      }
      catch (Exception ex)
      {
        request.FailWithDisplayText(AppResources.CouldNotOpenMediaFile);
      }
      finally
      {
        deferral.Complete();
      }
      request = (DataRequest) null;
      deferral = (DataRequestDeferral) null;
    }

    private void SliderView_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.SliderView.CenterImage.IsImageScaled)
        return;
      this.Dispatcher.RunAfterDelay(TimeSpan.FromMilliseconds(250.0), (Action) (() =>
      {
        if (this.SliderView.CenterImage.IsImageScaled)
          return;
        this.showInfoOverlay_ = !this.showInfoOverlay_;
        this.UpdateInfoOverlay(this.CurrentItem, this.currIndex_, this.showInfoOverlay_, false, true, true);
      }));
    }

    private IEnumerable<MenuItem> CreateMenuItems(ChatMediaPage.Item item)
    {
      Message message;
      if (item == null || (message = item.Message) == null)
        return (IEnumerable<MenuItem>) null;
      LinkedList<MenuItem> menuItems = new LinkedList<MenuItem>();
      MenuItem menuItem1 = new MenuItem();
      menuItem1.Header = message.IsStarred ? (object) AppResources.UnstarLower : (object) AppResources.StarLower;
      MenuItem menuItem2 = menuItem1;
      menuItem2.Click += new RoutedEventHandler(this.Star_Click);
      menuItems.AddLast(menuItem2);
      bool flag = message.LocalFileExists();
      if (((message.MediaWaType == FunXMPP.FMessage.Type.Image ? 1 : (message.MediaWaType == FunXMPP.FMessage.Type.Video ? 1 : 0)) & (flag ? 1 : 0)) != 0)
      {
        MenuItem menuItem3 = new MenuItem();
        menuItem3.Header = (object) AppResources.Save;
        MenuItem menuItem4 = menuItem3;
        menuItem4.Click += new RoutedEventHandler(this.Save_Click);
        menuItems.AddLast(menuItem4);
      }
      MenuItem menuItem5 = new MenuItem();
      menuItem5.Header = (object) AppResources.Forward;
      menuItem5.IsEnabled = flag;
      MenuItem menuItem6 = menuItem5;
      menuItem6.Click += new RoutedEventHandler(this.Forward_Click);
      menuItems.AddLast(menuItem6);
      MenuItem menuItem7 = new MenuItem();
      menuItem7.Header = (object) AppResources.ShareTitle;
      menuItem7.IsEnabled = flag;
      MenuItem menuItem8 = menuItem7;
      menuItem8.Click += new RoutedEventHandler(this.Share_Click);
      menuItems.AddLast(menuItem8);
      if (message.MediaWaType == FunXMPP.FMessage.Type.Image && this.targetChatJid_ != null && JidHelper.IsGroupJid(this.targetChatJid_))
      {
        MenuItem menuItem9 = new MenuItem();
        menuItem9.Header = (object) AppResources.AssignToGroup;
        MenuItem menuItem10 = menuItem9;
        menuItem10.Click += new RoutedEventHandler(this.AssignToGroup_Click);
        menuItems.AddLast(menuItem10);
      }
      if (message.MediaWaType == FunXMPP.FMessage.Type.Image)
      {
        MenuItem menuItem11 = new MenuItem();
        menuItem11.Header = (object) AppResources.SetAsProfilePhoto;
        MenuItem menuItem12 = menuItem11;
        menuItem12.Click += new RoutedEventHandler(this.SetAsProfilePic_Click);
        menuItems.AddLast(menuItem12);
      }
      MenuItem menuItem13 = new MenuItem();
      menuItem13.Header = (object) AppResources.Delete;
      MenuItem menuItem14 = menuItem13;
      menuItem14.Click += new RoutedEventHandler(this.Delete_Click);
      menuItems.AddLast(menuItem14);
      return (IEnumerable<MenuItem>) menuItems;
    }

    private void OverflowDots_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.OpenMenu();
    }

    private void Menu_Opened(object sender, RoutedEventArgs e)
    {
      this.isMenuOpened = true;
      this.Menu.ItemsSource = (IEnumerable) this.CreateMenuItems(this.CurrentItem);
    }

    private void Menu_Closed(object sender, RoutedEventArgs e)
    {
      this.isMenuOpened = false;
      this.Menu.ItemsSource = (IEnumerable) null;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ChatMediaPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.SliderView = (ImageSlideViewControl) this.FindName("SliderView");
      this.InfoOverlayPanel = (Grid) this.FindName("InfoOverlayPanel");
      this.IndexBlock = (TextBlock) this.FindName("IndexBlock");
      this.TitleBlock = (TextTrimmingControl2) this.FindName("TitleBlock");
      this.CaptionBlock = (RichTextBlock) this.FindName("CaptionBlock");
      this.TimestampBlock = (TextBlock) this.FindName("TimestampBlock");
      this.PlayButton = (RoundButton) this.FindName("PlayButton");
      this.OverflowDots = (Image) this.FindName("OverflowDots");
      this.Menu = (ContextMenu) this.FindName("Menu");
    }

    public class Item : PropChangedBase
    {
      private Message msg_;
      private BitmapSource thumb_;
      private bool isDefaultThumb_;
      private WeakReference<BitmapSource> bitmapRef_;
      private bool shouldAllowZooming_;
      private string footerStr_;

      public Message Message => this.msg_;

      public bool IsDefaultThumb => this.isDefaultThumb_;

      private BitmapSource BitmapCache
      {
        get
        {
          BitmapSource target = (BitmapSource) null;
          return this.bitmapRef_ == null || !this.bitmapRef_.TryGetTarget(out target) || target == null ? (BitmapSource) null : target;
        }
        set
        {
          if (this.bitmapRef_ == null)
          {
            if (value == null)
              return;
            this.bitmapRef_ = new WeakReference<BitmapSource>(value);
          }
          else
            this.bitmapRef_.SetTarget(value);
        }
      }

      public bool IsFullImage { get; private set; }

      public bool ShouldAllowZooming
      {
        get => this.shouldAllowZooming_;
        set
        {
          if (this.shouldAllowZooming_ == value)
            return;
          this.shouldAllowZooming_ = value;
          this.NotifyPropertyChanged(nameof (ShouldAllowZooming));
        }
      }

      public Item(Message msg) => this.msg_ = msg;

      public string GetFooterStr(string targetChatJid, bool forceUpdate = false)
      {
        if (forceUpdate || this.footerStr_ == null && this.msg_ != null)
        {
          DateTime? localTimestamp = this.msg_.LocalTimestamp;
          string str = localTimestamp.HasValue ? DateTimeUtils.FormatLastSeen(localTimestamp.Value) : "";
          this.footerStr_ = !this.msg_.KeyFromMe ? (targetChatJid == null || !JidHelper.IsGroupJid(targetChatJid) ? str : string.Format("{0}, {1}", (object) this.msg_.GetSenderDisplayName(false), (object) str)) : string.Format("{0}, {1}", (object) AppResources.You, (object) str);
          if (this.msg_.IsStarred)
            this.footerStr_ += " ★";
        }
        return this.footerStr_;
      }

      public IObservable<BitmapSource> GetBitmapObservable()
      {
        return Observable.Create<BitmapSource>((Func<IObserver<BitmapSource>, Action>) (observer =>
        {
          BitmapSource bitmapCache = this.BitmapCache;
          if (bitmapCache != null)
          {
            observer.OnNext(bitmapCache);
            observer.OnCompleted();
          }
          else if (this.thumb_ != null)
          {
            observer.OnNext(this.thumb_);
            observer.OnCompleted();
          }
          else if (this.msg_.MediaWaType == FunXMPP.FMessage.Type.Image)
          {
            this.msg_.GetImageStreamObservable().SubscribeOn<Stream>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<Stream>().Subscribe<Stream>((Action<Stream>) (imgStream =>
            {
              BitmapSource bitmapSource = (BitmapSource) null;
              if (imgStream != null)
              {
                try
                {
                  using (imgStream)
                  {
                    ushort? jpegOrientation = JpegUtils.GetJpegOrientation(imgStream);
                    if (jpegOrientation.HasValue)
                    {
                      WriteableBitmap source = JpegUtils.DecodeJpeg(imgStream);
                      ushort? nullable = jpegOrientation;
                      int? orientation = nullable.HasValue ? new int?((int) nullable.GetValueOrDefault()) : new int?();
                      bitmapSource = (BitmapSource) JpegUtils.ApplyJpegOrientation((BitmapSource) source, orientation);
                    }
                    else
                    {
                      bitmapSource = (BitmapSource) new BitmapImage();
                      bitmapSource.SetSource(imgStream);
                    }
                  }
                  this.BitmapCache = bitmapSource;
                  this.ShouldAllowZooming = true;
                  Log.p("media page: loaded {0} | {1}x{2}", this.msg_.LocalFileUri, (object) bitmapSource.PixelWidth, (object) bitmapSource.PixelHeight);
                }
                catch (Exception ex)
                {
                  Log.LogException(ex, "media page exception processing image stream");
                  bitmapSource = (BitmapSource) null;
                }
              }
              else
                Log.l("media page", "image stream null");
              if (bitmapSource == null)
              {
                Log.d("media page", "Supplying default image for {0}", (object) this.msg_.LocalFileUri);
                this.thumb_ = bitmapSource = ChatMediaPage.Item.GetThumbnail(this.msg_, out this.isDefaultThumb_);
              }
              observer.OnNext(bitmapSource);
              observer.OnCompleted();
            }));
          }
          else
          {
            this.thumb_ = ChatMediaPage.Item.GetThumbnail(this.msg_, out this.isDefaultThumb_);
            observer.OnNext(this.thumb_);
            observer.OnCompleted();
          }
          return (Action) (() => { });
        }));
      }

      private static BitmapSource GetThumbnail(Message msg, out bool isDefault)
      {
        isDefault = false;
        BitmapSource source = msg.GetThumbnail(MessageExtensions.ThumbPreference.OnlySmall);
        if (source == null)
        {
          source = AssetStore.GetMessageDefaultIcon(msg.MediaWaType);
          isDefault = true;
        }
        else if (msg.MediaWaType == FunXMPP.FMessage.Type.Video)
        {
          if (!(source is WriteableBitmap bmp))
            bmp = new WriteableBitmap(source);
          source = (BitmapSource) bmp.Convolute(WriteableBitmapExtensions.KernelGaussianBlur3x3).Convolute(WriteableBitmapExtensions.KernelGaussianBlur3x3);
        }
        return source;
      }
    }
  }
}
