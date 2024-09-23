// Decompiled with JetBrains decompiler
// Type: WhatsApp.LinksTabView
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
using WhatsApp.CompatibilityShims;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class LinksTabView : UserControl, IDisposable
  {
    private bool isLoading;
    private bool isLoaded;
    private bool isShownRequested;
    private bool isDisposed;
    private IDisposable loadingSub;
    private List<KeyedObservableCollection<string, MessageViewModel>> listSrc;
    internal LongListSelector LinksList;
    internal ProgressBar LoadingProgressBar;
    private bool _contentLoaded;

    public LinksTabView() => this.InitializeComponent();

    public void Dispose()
    {
      this.isDisposed = true;
      this.loadingSub.SafeDispose();
      this.loadingSub = (IDisposable) null;
    }

    public void Clear()
    {
      this.loadingSub.SafeDispose();
      this.loadingSub = (IDisposable) null;
      this.LinksList.ItemsSource = (IList) (this.listSrc = (List<KeyedObservableCollection<string, MessageViewModel>>) null);
    }

    public void Load(string[] jids)
    {
      if (this.isLoaded || this.isLoading)
        return;
      this.isLoading = true;
      this.LoadingProgressBar.Visibility = Visibility.Visible;
      this.loadingSub = this.LoadFromDb(jids).SubscribeOn<Unit>(WAThreadPool.Scheduler).ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ =>
      {
        if (this.isDisposed)
          return;
        this.isLoading = false;
        this.isLoaded = true;
        this.LoadingProgressBar.Visibility = Visibility.Collapsed;
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
      if (!this.isLoaded || this.isDisposed)
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
        Message[] urlMsgs = (Message[]) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          lock (finishLock)
          {
            if (finished)
              return;
          }
          querySub = db.GetUrlMessages(jids).Subscribe<Message[]>((Action<Message[]>) (msgs =>
          {
            urlMsgs = msgs;
            querySub.SafeDispose();
            querySub = (IDisposable) null;
          }));
        }));
        DateTime timeNow = DateTime.Now;
        this.listSrc = ((IEnumerable<Message>) (urlMsgs ?? new Message[0])).Select<Message, MessageViewModel>((Func<Message, MessageViewModel>) (m => MessageViewModel.Create(m, (Action<MessageViewModel>) (vm => vm.IsForGalleryView = true)))).GroupBy<MessageViewModel, int>((Func<MessageViewModel, int>) (vm => DateTimeUtils.GetDateTimeGroupingKey(vm.Message.LocalTimestamp, timeNow))).OrderBy<IGrouping<int, MessageViewModel>, long>((Func<IGrouping<int, MessageViewModel>, long>) (g => g.First<MessageViewModel>().Message.TimestampLong)).Reverse<IGrouping<int, MessageViewModel>>().Select<IGrouping<int, MessageViewModel>, KeyedObservableCollection<string, MessageViewModel>>((Func<IGrouping<int, MessageViewModel>, KeyedObservableCollection<string, MessageViewModel>>) (g => new KeyedObservableCollection<string, MessageViewModel>(DateTimeUtils.GetDateTimeGroupingTitle(g.Key), (IEnumerable<MessageViewModel>) g))).ToList<KeyedObservableCollection<string, MessageViewModel>>();
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
      this.LinksList.ItemsSource = (IList) this.listSrc;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/LinksTabView.xaml", UriKind.Relative));
      this.LinksList = (LongListSelector) this.FindName("LinksList");
      this.LoadingProgressBar = (ProgressBar) this.FindName("LoadingProgressBar");
    }
  }
}
