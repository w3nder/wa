// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaMultiSelector
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WhatsApp.CompatibilityShims;
using WhatsApp.WaCollections;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class MediaMultiSelector : UserControl
  {
    private List<MediaMultiSelector.Item> flatListSource_;
    private List<KeyedList<string, MediaMultiSelector.Item>> listSource_;
    private bool isMultiSelectionEnabled_;
    private Dictionary<int, MediaMultiSelector.Item> pendingItems_ = new Dictionary<int, MediaMultiSelector.Item>();
    internal LongListSelector ItemsGrid;
    internal ProgressBar LoadingProgressBar;
    internal TextBlock FooterTextBlock;
    private bool _contentLoaded;

    public event MediaMultiSelector.ItemSelectionChangedHandler SingleItemSelected;

    protected void NotifySingleItemSelected(MediaMultiSelector.Item item)
    {
      if (this.SingleItemSelected == null)
        return;
      this.SingleItemSelected(item);
    }

    public event MediaMultiSelector.ItemSelectionChangedHandler ItemSelectionToggled;

    protected void NotifyItemSelectionToggled(MediaMultiSelector.Item item)
    {
      if (this.ItemSelectionToggled == null)
        return;
      this.ItemSelectionToggled(item);
    }

    public event MediaMultiSelector.ItemSelectionChangedHandler ItemSelectionBlocked;

    protected void NotifyItemSelectionBlocked(MediaMultiSelector.Item item)
    {
      if (this.ItemSelectionBlocked == null)
        return;
      this.ItemSelectionBlocked(item);
    }

    public List<MediaMultiSelector.Item> FlatItemsSource
    {
      set
      {
        this.ItemsGrid.IsFlatList = true;
        this.ItemsGrid.ItemsSource = (IList) (this.flatListSource_ = value ?? new List<MediaMultiSelector.Item>());
        if (value == null)
          return;
        this.LoadingProgressBar.Visibility = Visibility.Collapsed;
      }
    }

    public List<KeyedList<string, MediaMultiSelector.Item>> ItemsSource
    {
      set
      {
        this.ItemsGrid.IsFlatList = false;
        this.ItemsGrid.ItemsSource = (IList) (this.listSource_ = value ?? new List<KeyedList<string, MediaMultiSelector.Item>>());
        if (value == null)
          return;
        this.LoadingProgressBar.Visibility = Visibility.Collapsed;
      }
    }

    public bool IsMultiSelectionEnabled
    {
      get => this.isMultiSelectionEnabled_;
      set
      {
        if (this.isMultiSelectionEnabled_ == value)
          return;
        this.pendingItems_.Clear();
        this.isMultiSelectionEnabled_ = value;
        this.ItemsGrid.ItemTemplate = this.isMultiSelectionEnabled_ ? this.Resources[(object) "SelectionModeItemTemplate"] as DataTemplate : this.Resources[(object) "ItemTemplate"] as DataTemplate;
      }
    }

    public bool IsSelectionAddBlocked { get; set; }

    public MediaMultiSelector.Item[] MultiSelectedItems
    {
      get
      {
        return this.pendingItems_.Values.Where<MediaMultiSelector.Item>((Func<MediaMultiSelector.Item, bool>) (item => item.IsSelected)).ToArray<MediaMultiSelector.Item>();
      }
    }

    public bool IsWhiteScrollBar
    {
      set
      {
        if (ImageStore.IsDarkTheme())
          return;
        this.ItemsGrid.Style = value ? this.Resources[(object) "WhiteScrollBarStyle"] as Style : (Style) null;
      }
    }

    public MediaMultiSelector() => this.InitializeComponent();

    public void ScrollToBottom()
    {
      if (this.ItemsGrid.IsFlatList)
      {
        int count = this.flatListSource_ == null ? 0 : this.flatListSource_.Count;
        if (count <= 0)
          return;
        this.ItemsGrid.ScrollTo((object) this.flatListSource_[count - 1]);
      }
      else
      {
        int count = this.listSource_ == null ? 0 : this.listSource_.Count;
        if (count <= 0)
          return;
        KeyedList<string, MediaMultiSelector.Item> keyedList = this.listSource_[count - 1];
        if (keyedList.Count > 0)
          this.ItemsGrid.ScrollTo((object) keyedList[keyedList.Count - 1]);
        else
          this.ItemsGrid.ScrollTo((object) keyedList);
      }
    }

    private void ProcessItemGridSelectionChanged(MediaMultiSelector.Item item)
    {
      if (item == null)
        return;
      if (this.IsMultiSelectionEnabled)
      {
        bool flag = !item.IsSelected;
        if (flag && this.IsSelectionAddBlocked)
        {
          this.NotifyItemSelectionBlocked(item);
        }
        else
        {
          item.IsSelected = flag;
          this.pendingItems_[item.ItemId] = item;
          this.NotifyItemSelectionToggled(item);
        }
      }
      else
        this.NotifySingleItemSelected(item);
    }

    private void ItemsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      MediaMultiSelector.Item selectedItem = this.ItemsGrid.SelectedItem as MediaMultiSelector.Item;
      this.ItemsGrid.SelectedItem = (object) null;
      this.ProcessItemGridSelectionChanged(selectedItem);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/MediaMultiSelector.xaml", UriKind.Relative));
      this.ItemsGrid = (LongListSelector) this.FindName("ItemsGrid");
      this.LoadingProgressBar = (ProgressBar) this.FindName("LoadingProgressBar");
      this.FooterTextBlock = (TextBlock) this.FindName("FooterTextBlock");
    }

    public class Item : WaViewModelBase
    {
      private WeakReference<BitmapSource> thumbnailRef_;
      private IDisposable thumbSub;
      protected string groupingKey_;
      protected FunXMPP.FMessage.Type mediaType_;
      private bool isSelected_;

      protected BitmapSource ThumbnailCache
      {
        get
        {
          BitmapSource target = (BitmapSource) null;
          return this.thumbnailRef_ == null || !this.thumbnailRef_.TryGetTarget(out target) ? (BitmapSource) null : target;
        }
        set
        {
          if (this.thumbnailRef_ == null)
          {
            if (value == null)
              return;
            this.thumbnailRef_ = new WeakReference<BitmapSource>(value);
          }
          else
            this.thumbnailRef_.SetTarget(value);
        }
      }

      public BitmapSource Thumbnail
      {
        get
        {
          if (this.ThumbnailCache == null && this.thumbSub == null)
            this.thumbSub = this.GetThumbnailObservable().SubscribeOn<BitmapSource>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<BitmapSource>().Subscribe<BitmapSource>((Action<BitmapSource>) (thumb =>
            {
              this.ThumbnailCache = thumb;
              this.NotifyPropertyChanged(nameof (Thumbnail));
            }), (Action) (() =>
            {
              this.thumbSub.SafeDispose();
              this.thumbSub = (IDisposable) null;
            }));
          return this.ThumbnailCache;
        }
      }

      public string GroupingKey => this.groupingKey_ ?? (this.groupingKey_ = this.GetGroupingKey());

      public virtual DateTime? Timestamp => new DateTime?();

      public virtual FunXMPP.FMessage.Type MediaType
      {
        get => this.mediaType_;
        protected set => this.mediaType_ = value;
      }

      public int ItemId { get; protected set; }

      public bool IsSelected
      {
        get => this.isSelected_;
        set
        {
          if (this.isSelected_ == value)
            return;
          this.isSelected_ = value;
          this.OnSelectionToggled();
        }
      }

      public Brush BackgroundBrush
      {
        get
        {
          return this.MediaType != FunXMPP.FMessage.Type.Undefined ? UIUtils.InactiveBrush : (Brush) null;
        }
      }

      public Visibility SelectedVisibility
      {
        get
        {
          return (this.IsSelected && (this.MediaType == FunXMPP.FMessage.Type.Image || this.MediaType == FunXMPP.FMessage.Type.Video || this.MediaType == FunXMPP.FMessage.Type.Gif || this.MediaType == FunXMPP.FMessage.Type.Sticker)).ToVisibility();
        }
      }

      public Visibility SelectionBoxVisibility
      {
        get
        {
          return (this.MediaType == FunXMPP.FMessage.Type.Image || this.MediaType == FunXMPP.FMessage.Type.Video || this.MediaType == FunXMPP.FMessage.Type.Gif || this.MediaType == FunXMPP.FMessage.Type.Sticker).ToVisibility();
        }
      }

      public Visibility PlayOverlayVisibility
      {
        get => (this.MediaType == FunXMPP.FMessage.Type.Video).ToVisibility();
      }

      public Visibility GifOverlayVisibility
      {
        get => (this.MediaType == FunXMPP.FMessage.Type.Gif).ToVisibility();
      }

      public System.Windows.Media.ImageSource SelectedCheckMark
      {
        get => (System.Windows.Media.ImageSource) ImageStore.SelectedCheckMark;
      }

      public System.Windows.Media.ImageSource PlayButtonIcon
      {
        get
        {
          return this.MediaType != FunXMPP.FMessage.Type.Video ? (System.Windows.Media.ImageSource) null : (System.Windows.Media.ImageSource) ImageStore.WhitePlayButton;
        }
      }

      public System.Windows.Media.ImageSource GifIcon
      {
        get
        {
          return this.MediaType != FunXMPP.FMessage.Type.Gif ? (System.Windows.Media.ImageSource) null : (System.Windows.Media.ImageSource) ImageStore.GifIcon;
        }
      }

      public IEnumerable<MediaMultiSelector.Item.Operation> Operations { get; set; }

      public Item(int itemId) => this.ItemId = itemId;

      protected void ResetCachedData()
      {
        this.ThumbnailCache = (BitmapSource) null;
        this.groupingKey_ = (string) null;
        this.mediaType_ = FunXMPP.FMessage.Type.Undefined;
      }

      protected void RefreshView()
      {
        this.NotifyPropertyChanged("Thumbnail");
        this.NotifyPropertyChanged("PlayOverlayVisibility");
        this.NotifyPropertyChanged("GifOverlayVisibility");
        if (this.IsSelected)
        {
          this.NotifyPropertyChanged("SelectionBoxVisibility");
          this.NotifyPropertyChanged("SelectedVisibility");
        }
        else
          this.NotifyPropertyChanged("BackgroundBrush");
      }

      public virtual IObservable<BitmapSource> GetThumbnailObservable()
      {
        return Observable.Create<BitmapSource>((Func<IObserver<BitmapSource>, Action>) (observer =>
        {
          Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
          {
            BitmapSource thumbnail = this.GetThumbnail();
            if (thumbnail != null)
              observer.OnNext(thumbnail);
            observer.OnCompleted();
          }));
          return (Action) (() => { });
        }));
      }

      public virtual BitmapSource GetThumbnail()
      {
        BitmapSource thumbnail = (BitmapSource) null;
        switch (this.MediaType)
        {
          case FunXMPP.FMessage.Type.Image:
          case FunXMPP.FMessage.Type.Gif:
          case FunXMPP.FMessage.Type.Sticker:
            thumbnail = (BitmapSource) WaIcons.AccentBackgroundImageIcon;
            break;
          case FunXMPP.FMessage.Type.Audio:
            thumbnail = (BitmapSource) WaIcons.AccentBackgroundAudioIcon;
            break;
          case FunXMPP.FMessage.Type.Video:
            thumbnail = (BitmapSource) WaIcons.AccentBackgroundVideoIcon;
            break;
        }
        return thumbnail;
      }

      protected virtual string GetGroupingKey() => (string) null;

      protected virtual void OnSelectionToggled()
      {
        this.NotifyPropertyChanged("SelectedVisibility");
      }

      protected override void DisposeManagedResources()
      {
        base.DisposeManagedResources();
        this.ResetCachedData();
        this.thumbSub.SafeDispose();
        this.thumbSub = (IDisposable) null;
      }

      public class Operation
      {
        private string name_;
        private Action<MediaMultiSelector.Item> op_;

        public string Name => this.name_;

        public Action<MediaMultiSelector.Item> Op => this.op_;

        public Operation(string name, Action<MediaMultiSelector.Item> op)
        {
          this.name_ = name;
          this.op_ = op;
        }
      }
    }

    public delegate void ItemSelectionChangedHandler(MediaMultiSelector.Item item);
  }
}
