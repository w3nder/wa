// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.SelfStatusThreadViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using WhatsApp.WaCollections;


namespace WhatsApp.WaViewModels
{
  public class SelfStatusThreadViewModel : StatusThreadItemViewModel
  {
    private int? pendingCount;

    public bool IsRecipientMode { get; private set; }

    public override string Jid => Settings.MyJid;

    public override bool ShowViewProgress => !this.IsRecipientMode;

    public override bool IsDimmed => false;

    public SelfStatusThreadViewModel(WaStatusThread statusThread, bool recipientMode)
      : base(statusThread, (UserStatus) null)
    {
      this.IsRecipientMode = recipientMode;
      this.EnableContextMenu = false;
    }

    public override string GetTitle() => AppResources.MyStatusV3Title;

    public override RichTextBlock.TextSet GetSubtitle()
    {
      string str = (string) null;
      if (this.IsRecipientMode)
      {
        int n = 0;
        switch (Settings.StatusV3PrivacySetting)
        {
          case WaStatusHelper.StatusPrivacySettings.WhiteList:
            MessagesContext.Run((MessagesContext.MessagesCallback) (db => n = WaStatusHelper.GetWhiteListCount(db)));
            str = Plurals.Instance.GetString(AppResources.StatusShareToNPlural, n);
            break;
          case WaStatusHelper.StatusPrivacySettings.BlackList:
            MessagesContext.Run((MessagesContext.MessagesCallback) (db => n = WaStatusHelper.GetBlackListCount(db)));
            str = Plurals.Instance.GetString(AppResources.StatusShareToContactsExceptNPlural, n);
            break;
          default:
            str = AppResources.StatusShareToContacts;
            break;
        }
      }
      else
      {
        WaStatusThread statusThread = this.StatusThread;
        int count = statusThread != null ? statusThread.Count : 0;
        if (count > 0)
        {
          if (this.pendingCount.HasValue)
            str = this.pendingCount.Value <= 0 ? Plurals.Instance.GetString(AppResources.StatusNRecentUpdatesPlural, count) : Plurals.Instance.GetString(AppResources.StatusNPendingUpdatesPlural, this.pendingCount.Value);
          else
            this.GetSelfStatusThreadCounts().SubscribeOn<Pair<int, int>>(WAThreadPool.Scheduler).ObserveOnDispatcher<Pair<int, int>>().Subscribe<Pair<int, int>>((Action<Pair<int, int>>) (p =>
            {
              this.pendingCount = new int?(p.Second);
              if (!this.pendingCount.HasValue)
                return;
              this.Notify("Subtitle");
            }));
        }
        else
          str = AppResources.StatusSubtitleTooltip;
      }
      return new RichTextBlock.TextSet()
      {
        Text = str ?? ""
      };
    }

    protected override IObservable<System.Windows.Media.ImageSource> GetPictureSourceObservableImpl(
      bool getCurrent,
      bool trackChange)
    {
      if (this.IsRecipientMode)
        return Observable.Return<System.Windows.Media.ImageSource>(this.IsSelected ? (System.Windows.Media.ImageSource) AssetStore.StatusIconWhite : (System.Windows.Media.ImageSource) AssetStore.StatusIcon);
      return this.StatusThread?.LatestStatus == null ? Observable.Return<System.Windows.Media.ImageSource>((System.Windows.Media.ImageSource) AssetStore.StatusIcon) : base.GetPictureSourceObservableImpl(getCurrent, trackChange);
    }

    public override void Refresh()
    {
      if (!this.IsRecipientMode)
        this.pendingCount = new int?();
      base.Refresh();
    }

    private IObservable<Pair<int, int>> GetSelfStatusThreadCounts()
    {
      return Observable.Create<Pair<int, int>>((Func<IObserver<Pair<int, int>>, Action>) (observer =>
      {
        Pair<int, int> p = new Pair<int, int>(0, 0);
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          WaStatus[] statuses = db.GetStatuses(this.Jid, false, false, new TimeSpan?(WaStatus.Expiration));
          int num = 0;
          foreach (WaStatus waStatus in statuses)
          {
            Message messageById = db.GetMessageById(waStatus.MessageId);
            if (messageById != null && !messageById.IsDeliveredToServer())
              ++num;
          }
          p.First = statuses.Length;
          p.Second = num;
        }));
        observer.OnNext(p);
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }
  }
}
