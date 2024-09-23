// Decompiled with JetBrains decompiler
// Type: WhatsApp.DocsTabView
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
using WhatsApp.CommonOps;
using WhatsApp.CompatibilityShims;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class DocsTabView : UserControl
  {
    private bool isLoading;
    private bool isLoaded;
    private bool isShownRequested;
    private bool isDisposed;
    private IDisposable loadingSub;
    private List<KeyedObservableCollection<string, DocumentMessageViewModel>> listSrc;
    internal LongListSelector DocsList;
    internal ProgressBar LoadingProgressBar;
    private bool _contentLoaded;

    public DocsTabView() => this.InitializeComponent();

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
      this.DocsList.ItemsSource = (IList) (this.listSrc = (List<KeyedObservableCollection<string, DocumentMessageViewModel>>) null);
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
        Message[] docMsgs = (Message[]) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          lock (finishLock)
          {
            if (finished)
              return;
          }
          querySub = db.GetDocumentMessages(jids).Subscribe<Message[]>((Action<Message[]>) (msgs =>
          {
            docMsgs = msgs;
            querySub.SafeDispose();
            querySub = (IDisposable) null;
          }));
        }));
        DateTime timeNow = DateTime.Now;
        this.listSrc = ((IEnumerable<Message>) (docMsgs ?? new Message[0])).Select<Message, DocumentMessageViewModel>((Func<Message, DocumentMessageViewModel>) (m => MessageViewModel.Create(m, (Action<MessageViewModel>) (vm => vm.IsForGalleryView = true)) as DocumentMessageViewModel)).Where<DocumentMessageViewModel>((Func<DocumentMessageViewModel, bool>) (vm => vm != null)).GroupBy<DocumentMessageViewModel, int>((Func<DocumentMessageViewModel, int>) (vm => DateTimeUtils.GetDateTimeGroupingKey(vm.Message.LocalTimestamp, timeNow))).OrderBy<IGrouping<int, DocumentMessageViewModel>, long>((Func<IGrouping<int, DocumentMessageViewModel>, long>) (g => g.First<DocumentMessageViewModel>().Message.TimestampLong)).Reverse<IGrouping<int, DocumentMessageViewModel>>().Select<IGrouping<int, DocumentMessageViewModel>, KeyedObservableCollection<string, DocumentMessageViewModel>>((Func<IGrouping<int, DocumentMessageViewModel>, KeyedObservableCollection<string, DocumentMessageViewModel>>) (g => new KeyedObservableCollection<string, DocumentMessageViewModel>(DateTimeUtils.GetDateTimeGroupingTitle(g.Key), (IEnumerable<DocumentMessageViewModel>) g))).ToList<KeyedObservableCollection<string, DocumentMessageViewModel>>();
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
      this.DocsList.ItemsSource = (IList) this.listSrc;
    }

    private void DocsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      DocumentMessageViewModel selectedItem = this.DocsList.SelectedItem as DocumentMessageViewModel;
      this.DocsList.SelectedItem = (object) null;
      if (selectedItem == null)
        return;
      ViewMessage.View(selectedItem.Message);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/DocsTabView.xaml", UriKind.Relative));
      this.DocsList = (LongListSelector) this.FindName("DocsList");
      this.LoadingProgressBar = (ProgressBar) this.FindName("LoadingProgressBar");
    }
  }
}
