// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.JidItemViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace WhatsApp.WaViewModels
{
  public class JidItemViewModel : WaViewModelBase
  {
    protected string jid;
    private IDisposable picFetchSub;
    protected bool generateContextMenu = true;
    private bool enableContextMenu = true;
    private bool isSelected;
    protected bool? showVerifiedIcon;

    public virtual object Model => (object) null;

    public virtual string Key => this.Jid;

    public virtual string Jid => this.jid;

    protected RichTextBlock.TextSet richTitleCache { get; set; }

    public RichTextBlock.TextSet RichTitle
    {
      get => this.richTitleCache ?? (this.richTitleCache = this.GetRichTitle());
    }

    public string TitleStr => this.RichTitle.Text;

    public virtual string SubtitleStr => this.GetSubtitle()?.Text ?? "";

    public System.Windows.Media.ImageSource PictureSource
    {
      get
      {
        System.Windows.Media.ImageSource cached = (System.Windows.Media.ImageSource) null;
        if (this.GetCachedPicSource(out cached))
          return cached ?? this.GetDefaultPicture();
        if (this.picFetchSub == null)
          this.picFetchSub = this.GetPictureSourceObservable(true, true).SubscribeOn<System.Windows.Media.ImageSource>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<System.Windows.Media.ImageSource>().Subscribe<System.Windows.Media.ImageSource>((Action<System.Windows.Media.ImageSource>) (imgSrc =>
          {
            this.NotifyPropertyChanged(nameof (PictureSource));
            this.Notify(nameof (PictureSource), (object) imgSrc);
          }));
        return this.GetDefaultPicture();
      }
    }

    public virtual Brush PictureBackgroundBrush => (Brush) null;

    public virtual double PictureSize => 68.0;

    public virtual bool ShowSubtitle => true;

    public virtual Brush SubtitleBrush => UIUtils.SubtleBrush;

    public virtual FontWeight SubtitleWeight => FontWeights.Normal;

    public Func<object, IEnumerable<MenuItem>> Model2MenuItemsFunc { get; set; }

    public bool GenerateContextMenu
    {
      get => this.generateContextMenu;
      protected set => this.generateContextMenu = value;
    }

    public virtual bool EnableContextMenu
    {
      get => this.enableContextMenu;
      set => this.Notify(nameof (EnableContextMenu), (object) (this.enableContextMenu = value));
    }

    public virtual bool IsSelected
    {
      get => this.isSelected;
      set
      {
        if (this.isSelected == value)
          return;
        this.Notify(nameof (IsSelected), (object) (this.isSelected = value));
        this.NotifyPropertyChanged("ItemBackground");
      }
    }

    public Brush ItemBackground
    {
      get
      {
        if (!this.IsSelected)
          return (Brush) null;
        SolidColorBrush itemBackground = new SolidColorBrush(UIUtils.ForegroundBrush.Color);
        itemBackground.Opacity = 0.1;
        return (Brush) itemBackground;
      }
    }

    public virtual bool ShowVerifiedIcon
    {
      get
      {
        return this.showVerifiedIcon ?? (this.showVerifiedIcon = new bool?(JidHelper.IsPsaJid(this.Jid))).Value;
      }
    }

    public virtual bool IsDimmed => false;

    public Brush SelectionBackground { get; set; }

    public System.Windows.Media.ImageSource SelectionIconSource { get; set; }

    public JidItemViewModel()
    {
    }

    public JidItemViewModel(string jid) => this.jid = jid;

    public virtual string GetTitle() => "";

    public virtual RichTextBlock.TextSet GetRichTitle()
    {
      return new RichTextBlock.TextSet()
      {
        Text = this.GetTitle()
      };
    }

    public IObservable<RichTextBlock.TextSet> GetRichTitleObservable(bool discardCache)
    {
      if (discardCache)
        this.richTitleCache = (RichTextBlock.TextSet) null;
      return this.richTitleCache != null ? Observable.Return<RichTextBlock.TextSet>(this.richTitleCache) : this.GetRichTitleObservableImpl().Select<RichTextBlock.TextSet, RichTextBlock.TextSet>((Func<RichTextBlock.TextSet, RichTextBlock.TextSet>) (t => this.richTitleCache = t));
    }

    public virtual IObservable<RichTextBlock.TextSet> GetRichTitleObservableImpl()
    {
      return Observable.Create<RichTextBlock.TextSet>((Func<IObserver<RichTextBlock.TextSet>, Action>) (observer =>
      {
        observer.OnNext(this.GetRichTitle());
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    public virtual RichTextBlock.TextSet GetSubtitle() => (RichTextBlock.TextSet) null;

    public virtual IObservable<RichTextBlock.TextSet> GetSubtitleObservable()
    {
      return Observable.Create<RichTextBlock.TextSet>((Func<IObserver<RichTextBlock.TextSet>, Action>) (observer =>
      {
        observer.OnNext(this.GetSubtitle());
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    public virtual void Refresh()
    {
      this.richTitleCache = (RichTextBlock.TextSet) null;
      this.Notify(nameof (Refresh));
    }

    public IEnumerable<MenuItem> GetMenuItems()
    {
      return this.Model2MenuItemsFunc != null ? this.Model2MenuItemsFunc(this.Model) : this.GetMenuItemsImpl();
    }

    protected virtual IEnumerable<MenuItem> GetMenuItemsImpl() => (IEnumerable<MenuItem>) null;

    public virtual bool GetCachedPicSource(out System.Windows.Media.ImageSource cached)
    {
      cached = (System.Windows.Media.ImageSource) null;
      return false;
    }

    public virtual bool GetCachedPicSource(string jid, out System.Windows.Media.ImageSource cached)
    {
      return this.GetCachedPicSource(out cached);
    }

    public IObservable<System.Windows.Media.ImageSource> GetPictureSourceObservable(
      bool fetchCurrent,
      bool trackChange)
    {
      return this.GetPictureSourceObservable(this.Jid, fetchCurrent, trackChange);
    }

    public IObservable<System.Windows.Media.ImageSource> GetPictureSourceObservable(
      string jid,
      bool fetchCurrent,
      bool trackChange)
    {
      if (fetchCurrent)
      {
        System.Windows.Media.ImageSource cached = (System.Windows.Media.ImageSource) null;
        if (this.GetCachedPicSource(jid, out cached))
          return Observable.Return<System.Windows.Media.ImageSource>(cached).Concat<System.Windows.Media.ImageSource>(this.GetPictureSourceObservableImpl(jid, false, trackChange));
      }
      return this.GetPictureSourceObservableImpl(jid, fetchCurrent, trackChange);
    }

    protected virtual IObservable<System.Windows.Media.ImageSource> GetPictureSourceObservableImpl(
      bool fetchCurrent,
      bool trackChange)
    {
      return this.GetPictureSourceObservableImpl(this.Jid, fetchCurrent, trackChange);
    }

    protected virtual IObservable<System.Windows.Media.ImageSource> GetPictureSourceObservableImpl(
      string jid,
      bool fetchCurrent,
      bool trackChange)
    {
      if (jid == null)
        return Observable.Empty<System.Windows.Media.ImageSource>();
      ChatPictureStore.SubMode subMode = ChatPictureStore.SubMode.None;
      if (fetchCurrent)
        subMode |= ChatPictureStore.SubMode.GetCurrent;
      if (trackChange)
        subMode |= ChatPictureStore.SubMode.TrackChange;
      return ChatPictureStore.Get(jid, false, false, true, subMode).Select<ChatPictureStore.PicState, System.Windows.Media.ImageSource>((Func<ChatPictureStore.PicState, System.Windows.Media.ImageSource>) (picState => (System.Windows.Media.ImageSource) picState.Image));
    }

    public virtual System.Windows.Media.ImageSource GetDefaultPicture()
    {
      return (System.Windows.Media.ImageSource) AssetStore.GetDefaultChatIcon(this.Jid);
    }

    public virtual IDisposable ActivateLazySubscriptions() => (IDisposable) null;

    protected override void DisposeManagedResources()
    {
      this.picFetchSub.SafeDispose();
      this.picFetchSub = (IDisposable) null;
      base.DisposeManagedResources();
    }

    public void ResetVerifiedState() => this.showVerifiedIcon = new bool?();
  }
}
