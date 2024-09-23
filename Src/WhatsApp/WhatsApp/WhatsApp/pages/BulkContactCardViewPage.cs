// Decompiled with JetBrains decompiler
// Type: WhatsApp.BulkContactCardViewPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;


namespace WhatsApp
{
  public class BulkContactCardViewPage : PhoneApplicationPage
  {
    private static string nextInstanceTitle;
    private static IEnumerable<ContactVCard> nextVCards;
    private IEnumerable<ContactVCard> vCards;
    private bool allLoaded;
    private bool isPageEverLoaded;
    private IEnumerable<BulkContactCardViewPage.VCardListItem> allItems;
    private IList<BulkContactCardViewPage.VCardListItem>[] allItemsSrc;
    private Storyboard slideDownSb;
    private IDisposable delaySub;
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal Pivot Pivot;
    internal WhatsApp.CompatibilityShims.LongListSelector ListControl;
    internal TextBlock FooterTextBlock;
    private bool _contentLoaded;

    public BulkContactCardViewPage()
    {
      this.InitializeComponent();
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      string nextInstanceTitle = BulkContactCardViewPage.nextInstanceTitle;
      this.vCards = BulkContactCardViewPage.nextVCards;
      BulkContactCardViewPage.nextInstanceTitle = (string) null;
      if (!string.IsNullOrEmpty(nextInstanceTitle))
      {
        this.TitlePanel.SmallTitle = nextInstanceTitle;
        this.TitlePanel.Visibility = Visibility.Visible;
      }
      this.Pivot.HeaderTemplate = (DataTemplate) null;
      this.ListControl.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.ListControl_ManipulationStarted);
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      if (this.vCards == null)
        return;
      this.LoadContacts();
    }

    public static void Start(string title, IEnumerable<ContactVCard> vcards)
    {
      BulkContactCardViewPage.nextInstanceTitle = title;
      BulkContactCardViewPage.nextVCards = vcards;
      NavUtils.NavigateToPage(nameof (BulkContactCardViewPage));
    }

    private void LoadContacts()
    {
      if (this.allLoaded)
        return;
      IEnumerable<BulkContactCardViewPage.VCardListItem> source = this.vCards.Select<ContactVCard, BulkContactCardViewPage.VCardListItem>((Func<ContactVCard, BulkContactCardViewPage.VCardListItem>) (v => new BulkContactCardViewPage.VCardListItem(v)));
      IList<BulkContactCardViewPage.VCardListItem>[] array = source.OrderBy<BulkContactCardViewPage.VCardListItem, string>((Func<BulkContactCardViewPage.VCardListItem, string>) (c => c.DisplayName)).GroupBy<BulkContactCardViewPage.VCardListItem, string>((Func<BulkContactCardViewPage.VCardListItem, string>) (c => c.DisplayName.ToGroupChar())).OrderBy<IGrouping<string, BulkContactCardViewPage.VCardListItem>, string>((Func<IGrouping<string, BulkContactCardViewPage.VCardListItem>, string>) (g => g.Key)).Select<IGrouping<string, BulkContactCardViewPage.VCardListItem>, IList<BulkContactCardViewPage.VCardListItem>>((Func<IGrouping<string, BulkContactCardViewPage.VCardListItem>, IList<BulkContactCardViewPage.VCardListItem>>) (g => g.ToGoodGrouping<string, BulkContactCardViewPage.VCardListItem>())).ToArray<IList<BulkContactCardViewPage.VCardListItem>>();
      this.allLoaded = true;
      this.allItemsSrc = array;
      this.allItems = source;
      this.ListControl.IsFlatList = false;
      this.ListControl.GroupHeaderTemplate = this.Resources[(object) "GroupHeaderTemplate"] as DataTemplate;
      this.ListControl.ItemsSource = (IList) this.allItemsSrc;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (this.vCards != null)
        return;
      this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    private void CloseKeyboard() => this.Focus();

    private void SlideDownAndBackOut()
    {
      if (this.slideDownSb == null)
        this.slideDownSb = WaAnimations.PageTransition(PageTransitionAnimation.SlideDownFadeOut);
      Storyboarder.Perform(this.slideDownSb, (DependencyObject) this.LayoutRoot, false, (Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    private void OnLoaded(object sender, EventArgs e)
    {
      if (this.isPageEverLoaded)
        return;
      this.isPageEverLoaded = true;
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      base.OnRemovedFromJournal(e);
      this.delaySub.SafeDispose();
      this.delaySub = (IDisposable) null;
      if (this.allItems == null)
        return;
      foreach (BulkContactCardViewPage.VCardListItem allItem in this.allItems)
        allItem.Dispose();
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      e.Cancel = true;
      base.OnBackKeyPress(e);
      this.SlideDownAndBackOut();
    }

    private void ListControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      BulkContactCardViewPage.VCardListItem selectedItem = this.ListControl.SelectedItem as BulkContactCardViewPage.VCardListItem;
      this.ListControl.SelectedItem = (object) null;
      if (selectedItem == null)
        return;
      ContactVCard card = selectedItem.GetCard();
      if (card == null)
        return;
      card.ToSaveContactTask().GetShowTaskAsync<SaveContactResult>().Subscribe<IEvent<SaveContactResult>>((Action<IEvent<SaveContactResult>>) (result => { }));
    }

    private void ListControl_ManipulationStarted(object sender, EventArgs e)
    {
      this.CloseKeyboard();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/BulkContactCardViewPage.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.Pivot = (Pivot) this.FindName("Pivot");
      this.ListControl = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ListControl");
      this.FooterTextBlock = (TextBlock) this.FindName("FooterTextBlock");
    }

    private class VCardListItem : PropChangedBase
    {
      private ContactVCard vcard;
      private object vCardPhotoLock = new object();
      private WeakReference<byte[]> vCardPhotoBytesRef;
      private IDisposable thumbSub;
      private System.Windows.Media.ImageSource thumbnail;

      public string DisplayName { get; set; }

      public BitmapSource DisplayIcon { get; set; }

      public VCardListItem(ContactVCard card)
      {
        this.vcard = card;
        this.DisplayName = card.GetDisplayName(true);
        this.DisplayIcon = AssetStore.DefaultContactIcon;
      }

      public ContactVCard GetCard() => this.vcard;

      private byte[] CachedVCardPhotoBytes
      {
        get
        {
          lock (this.vCardPhotoLock)
          {
            byte[] target = (byte[]) null;
            if (this.vCardPhotoBytesRef != null && this.vCardPhotoBytesRef.TryGetTarget(out target) && target != null)
              return target;
            string photo;
            if ((photo = this.vcard.Photo) == null)
            {
              Log.l("msg vm", "parse vcard failed");
            }
            else
            {
              try
              {
                target = Convert.FromBase64String(photo);
              }
              catch (Exception ex)
              {
                target = (byte[]) null;
                Log.LogException(ex, "parse vcard photo failed");
              }
              if (target != null && ((IEnumerable<byte>) target).Any<byte>())
              {
                if (this.vCardPhotoBytesRef == null)
                  this.vCardPhotoBytesRef = new WeakReference<byte[]>(target);
                else
                  this.vCardPhotoBytesRef.SetTarget(target);
              }
            }
            return target;
          }
        }
      }

      public System.Windows.Media.ImageSource Thumbnail
      {
        get
        {
          if (this.thumbnail != null)
            return this.thumbnail;
          if (this.thumbSub == null)
            this.thumbSub = this.GetThumbnailObservable().SubscribeOn<System.Windows.Media.ImageSource>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<System.Windows.Media.ImageSource>().Subscribe<System.Windows.Media.ImageSource>((Action<System.Windows.Media.ImageSource>) (img =>
            {
              this.thumbnail = img == null ? (System.Windows.Media.ImageSource) AssetStore.DefaultContactIcon : img;
              this.NotifyPropertyChanged(nameof (Thumbnail));
            }));
          return (System.Windows.Media.ImageSource) AssetStore.DefaultContactIcon;
        }
        set => this.thumbnail = value;
      }

      public IObservable<System.Windows.Media.ImageSource> GetThumbnailObservable()
      {
        return Observable.Create<System.Windows.Media.ImageSource>((Func<IObserver<System.Windows.Media.ImageSource>, Action>) (observer =>
        {
          MemoryStream vCardPhotoStream = (MemoryStream) null;
          byte[] cachedVcardPhotoBytes = this.CachedVCardPhotoBytes;
          if (cachedVcardPhotoBytes != null)
          {
            if (((IEnumerable<byte>) cachedVcardPhotoBytes).Any<byte>())
            {
              try
              {
                vCardPhotoStream = new MemoryStream(cachedVcardPhotoBytes);
              }
              catch (Exception ex)
              {
                vCardPhotoStream = (MemoryStream) null;
              }
            }
          }
          if (vCardPhotoStream != null)
            Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
              System.Windows.Media.ImageSource imageSource = (System.Windows.Media.ImageSource) null;
              using (vCardPhotoStream)
              {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.CreateOptions = BitmapCreateOptions.BackgroundCreation;
                bitmapImage.SetSource((Stream) vCardPhotoStream);
                imageSource = (System.Windows.Media.ImageSource) bitmapImage;
              }
              observer.OnNext(imageSource);
            }));
          string jid = this.vcard == null ? (string) null : ((IEnumerable<ContactVCard.PhoneNumber>) this.vcard.PhoneNumbers).Where<ContactVCard.PhoneNumber>((Func<ContactVCard.PhoneNumber, bool>) (pn => pn.Jid != null && JidHelper.IsUserJid(pn.Jid))).Select<ContactVCard.PhoneNumber, string>((Func<ContactVCard.PhoneNumber, string>) (pn => pn.Jid)).FirstOrDefault<string>();
          if (this.vcard == null || string.IsNullOrEmpty(jid))
          {
            observer.OnNext((System.Windows.Media.ImageSource) null);
            return (Action) (() => { });
          }
          IDisposable chatPicSub = (IDisposable) null;
          chatPicSub = ChatPictureStore.Get(jid, false, false, false, ChatPictureStore.SubMode.GetCurrent).SubscribeOn<ChatPictureStore.PicState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<ChatPictureStore.PicState>().Subscribe<ChatPictureStore.PicState>((Action<ChatPictureStore.PicState>) (picState =>
          {
            observer.OnNext((System.Windows.Media.ImageSource) picState.Image);
            if (picState.Image == null)
              return;
            observer.OnCompleted();
          }));
          return (Action) (() =>
          {
            chatPicSub.SafeDispose();
            chatPicSub = (IDisposable) null;
          });
        }));
      }

      public void Dispose() => this.thumbSub.SafeDispose();
    }
  }
}
