// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaTabView
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WhatsApp.CommonOps;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class MediaTabView : UserControl
  {
    private bool isLoading;
    private bool isLoaded;
    private bool isShownRequested;
    private IDisposable loadingSub;
    private List<KeyedList<string, MediaMultiSelector.Item>> listSrc;
    private MediaMultiSelector.Item.Operation[] mediaItemOperations_;
    internal StackPanel NoMediaPanel;
    internal TextBlock TooltipNoMedia;
    internal TextBlock TooltipSendMedia;
    internal MediaMultiSelector MediaBrowser;
    private bool _contentLoaded;

    public bool IsMultiSelectionEnabled
    {
      get => this.MediaBrowser.IsMultiSelectionEnabled;
      set => this.MediaBrowser.IsMultiSelectionEnabled = value;
    }

    private MediaMultiSelector.Item.Operation[] GetMediaItemOperations()
    {
      MediaMultiSelector.Item.Operation[] mediaItemOperations = this.mediaItemOperations_;
      if (mediaItemOperations != null)
        return mediaItemOperations;
      return this.mediaItemOperations_ = new MediaMultiSelector.Item.Operation[2]
      {
        new MediaMultiSelector.Item.Operation(AppResources.Forward, (Action<MediaMultiSelector.Item>) (gmsItem =>
        {
          if (!(gmsItem is MediaTabView.Item obj2) || obj2.Msg == null)
            return;
          SendMessage.ChooseRecipientAndForwardExisting(new Message[1]
          {
            obj2.Msg
          });
        })),
        new MediaMultiSelector.Item.Operation(AppResources.Delete, (Action<MediaMultiSelector.Item>) (gmsItem =>
        {
          MediaTabView.Item item = gmsItem as MediaTabView.Item;
          if (item == null || item.Msg == null)
            return;
          Message msg = item.Msg;
          ChatMediaPage.GetDeleteConfirmationObs(msg).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (accept =>
          {
            if (!accept)
              return;
            item.Msg = (Message) null;
            item.Operations = (IEnumerable<MediaMultiSelector.Item.Operation>) null;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.DeleteMessage(msg)));
          }));
        }))
      };
    }

    public MediaTabView()
    {
      this.InitializeComponent();
      this.NoMediaPanel.Margin = new Thickness(24.0 * ResolutionHelper.ZoomMultiplier);
      this.TooltipNoMedia.Text = AppResources.NoMediaForThread;
      this.TooltipSendMedia.Text = AppResources.SendMediaTip;
      this.MediaBrowser.SingleItemSelected += new MediaMultiSelector.ItemSelectionChangedHandler(this.MediaBrowser_SingleItemSelected);
    }

    public void ClearMediaItems()
    {
      this.MediaBrowser.ItemsSource = (List<KeyedList<string, MediaMultiSelector.Item>>) null;
    }

    public void Dispose()
    {
      this.loadingSub.SafeDispose();
      this.loadingSub = (IDisposable) null;
    }

    public void Load(string[] jids)
    {
      if (this.isLoaded || this.isLoading)
        return;
      this.isLoading = true;
      this.NoMediaPanel.Visibility = Visibility.Collapsed;
      this.loadingSub = this.LoadFromDb(jids).SubscribeOn<Unit>(WAThreadPool.Scheduler).ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ =>
      {
        this.isLoading = false;
        this.isLoaded = true;
        if (!this.isShownRequested)
          return;
        this.ShowImpl();
      }));
    }

    public void Show()
    {
      if (this.isShownRequested)
        return;
      this.isShownRequested = true;
      if (!this.isLoaded)
        return;
      this.ShowImpl();
    }

    private IObservable<Unit> LoadFromDb(string[] jids)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        object finishLock = new object();
        bool finished = false;
        IDisposable querySub = (IDisposable) null;
        Message[] mediaMsgs = (Message[]) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          lock (finishLock)
          {
            if (finished)
              return;
          }
          querySub = db.GetMediaMessages(jids, true, true).Subscribe<Message[]>((Action<Message[]>) (msgs =>
          {
            mediaMsgs = msgs;
            Log.d("mediatab", "Found {0}", mediaMsgs == null ? (object) "-1" : (object) mediaMsgs.Length.ToString());
          }));
        }));
        Log.d("mediatab", "Given {0}", mediaMsgs == null ? (object) "-1" : (object) mediaMsgs.Length.ToString());
        DateTime timeNow = DateTime.Now;
        this.listSrc = ((IEnumerable<Message>) (mediaMsgs ?? new Message[0])).Select<Message, MediaMultiSelector.Item>((Func<Message, MediaMultiSelector.Item>) (m => (MediaMultiSelector.Item) new MediaTabView.Item(m, (IEnumerable<MediaMultiSelector.Item.Operation>) this.GetMediaItemOperations()))).GroupBy<MediaMultiSelector.Item, int>((Func<MediaMultiSelector.Item, int>) (item => DateTimeUtils.GetDateTimeGroupingKey(item.Timestamp, timeNow))).OrderBy<IGrouping<int, MediaMultiSelector.Item>, DateTime?>((Func<IGrouping<int, MediaMultiSelector.Item>, DateTime?>) (g => g.First<MediaMultiSelector.Item>().Timestamp)).Select<IGrouping<int, MediaMultiSelector.Item>, KeyedList<string, MediaMultiSelector.Item>>((Func<IGrouping<int, MediaMultiSelector.Item>, KeyedList<string, MediaMultiSelector.Item>>) (g => new KeyedList<string, MediaMultiSelector.Item>(DateTimeUtils.GetDateTimeGroupingTitle(g.Key), (IEnumerable<MediaMultiSelector.Item>) g))).ToList<KeyedList<string, MediaMultiSelector.Item>>();
        observer.OnNext(new Unit());
        observer.OnCompleted();
        return (Action) (() =>
        {
          lock (finishLock)
            finished = true;
          querySub.SafeDispose();
          querySub = (IDisposable) null;
        });
      }));
    }

    private void ShowImpl()
    {
      if (this.listSrc == null)
        return;
      this.MediaBrowser.ItemsSource = this.listSrc;
      this.MediaBrowser.ScrollToBottom();
    }

    private void MediaBrowser_SingleItemSelected(MediaMultiSelector.Item gmsItem)
    {
      if (!(gmsItem is MediaTabView.Item obj))
        return;
      ViewMessage.View(obj.Msg);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/MediaTabView.xaml", UriKind.Relative));
      this.NoMediaPanel = (StackPanel) this.FindName("NoMediaPanel");
      this.TooltipNoMedia = (TextBlock) this.FindName("TooltipNoMedia");
      this.TooltipSendMedia = (TextBlock) this.FindName("TooltipSendMedia");
      this.MediaBrowser = (MediaMultiSelector) this.FindName("MediaBrowser");
    }

    public class Item : MediaMultiSelector.Item
    {
      private static DateTime? cachedDateTimeNow_;
      private Message msg;

      public static DateTime? CachedDateTimeNow
      {
        get => new DateTime?(MediaTabView.Item.cachedDateTimeNow_ ?? DateTime.Now);
        set => MediaTabView.Item.cachedDateTimeNow_ = value;
      }

      public Message Msg
      {
        get => this.msg;
        set
        {
          if (this.msg == value)
            return;
          this.msg = value;
          this.ResetCachedData();
          this.RefreshView();
        }
      }

      public override DateTime? Timestamp
      {
        get => this.Msg != null ? this.Msg.LocalTimestamp : new DateTime?();
      }

      public override FunXMPP.FMessage.Type MediaType
      {
        get
        {
          Message msg = this.Msg;
          return msg != null ? msg.MediaWaType : FunXMPP.FMessage.Type.Undefined;
        }
        protected set
        {
        }
      }

      public Item(Message msg, IEnumerable<MediaMultiSelector.Item.Operation> ops)
        : base(msg == null ? 0 : msg.MessageID)
      {
        this.Msg = msg;
        this.Operations = ops;
      }

      public override IObservable<BitmapSource> GetThumbnailObservable()
      {
        if (this.Msg == null)
          return Observable.Empty<BitmapSource>();
        Message msg = this.Msg;
        return Observable.Create<BitmapSource>((Func<IObserver<BitmapSource>, Action>) (observer =>
        {
          bool isLargeSize = false;
          MemoryStream thumbStream = msg.GetThumbnailStream(false, out isLargeSize);
          if (thumbStream == null)
            observer.OnCompleted();
          else
            Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
              BitmapSource bitmapSource = (BitmapSource) null;
              try
              {
                using (thumbStream)
                {
                  BitmapImage bitmapImage = new BitmapImage();
                  bitmapImage.CreateOptions = BitmapCreateOptions.BackgroundCreation;
                  bitmapImage.SetSource((Stream) thumbStream);
                  bitmapSource = (BitmapSource) bitmapImage;
                }
              }
              catch (Exception ex)
              {
                Log.LogException(ex, "decode thumb stream");
                bitmapSource = (BitmapSource) null;
              }
              observer.OnNext(bitmapSource);
              observer.OnCompleted();
            }));
          return (Action) (() => { });
        }));
      }

      public override BitmapSource GetThumbnail()
      {
        return this.Msg == null ? (BitmapSource) null : this.Msg.GetThumbnail(MessageExtensions.ThumbPreference.OnlySmall) ?? base.GetThumbnail();
      }

      protected override string GetGroupingKey()
      {
        if (this.Msg == null)
          return "";
        DateTime? cachedDateTimeNow = MediaTabView.Item.CachedDateTimeNow;
        DateTime? localTimestamp = this.Msg.LocalTimestamp;
        if (!localTimestamp.HasValue || !cachedDateTimeNow.HasValue)
          return "";
        TimeSpan timeSpan = cachedDateTimeNow.Value - localTimestamp.Value;
        if (timeSpan < Constants.TwoDays)
          return AppResources.GroupingRecent;
        if (timeSpan < Constants.OneWeek)
          return Plurals.Instance.GetString(AppResources.GroupingLastNDaysPlural, 7);
        bool flag = localTimestamp.Value.Month == cachedDateTimeNow.Value.Month;
        if (flag && timeSpan < Constants.ThirtyDays)
          return Plurals.Instance.GetString(AppResources.GroupingLastNDaysPlural, 30);
        return !flag && timeSpan < Constants.OneYear ? localTimestamp.Value.ToString("MMMM") : localTimestamp.Value.Year.ToString();
      }
    }
  }
}
