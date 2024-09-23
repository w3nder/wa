// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactsPagePivotHeader
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace WhatsApp
{
  public class ContactsPagePivotHeader : TabHeaderControl
  {
    private IDisposable chatsChangedSub;
    private IDisposable missedCallsSub;
    private IDisposable newStatusSub;
    private object enableUpdateLock = new object();
    private bool isUpdateEnabled = true;
    private StackPanel selectedStackPanel;

    public ContactsPagePivotHeader(Pivot p)
      : base(p)
    {
    }

    public override void Refresh()
    {
      base.Refresh();
      this.InitializeSelectedPivot();
    }

    private void InitializeSelectedPivot()
    {
      Log.d("pagetitle", "creating sub class tab headers");
      StackPanel stackPanel = new StackPanel();
      stackPanel.Orientation = Orientation.Horizontal;
      stackPanel.VerticalAlignment = VerticalAlignment.Bottom;
      stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
      this.selectedStackPanel = stackPanel;
      int num = this.refPivot.Items.Count - 1;
      Grid.SetColumn((FrameworkElement) this.selectedStackPanel, 2);
      Grid.SetColumnSpan((FrameworkElement) this.selectedStackPanel, num * 2);
      this.LayoutRoot.Children.Add((UIElement) this.selectedStackPanel);
      this.selectedStackPanel.Background = this.Resources[(object) "PhoneBackgroundBrush"] as Brush;
      this.selectedStackPanel.Visibility = Visibility.Collapsed;
      TabHeaderControl.BadgeIndicator badgeIndicator = this.CreateBadgeIndicator(this.selectedStackPanel, AppResources.SelectedPlural, true);
      badgeIndicator.MaxLength = num * TabHeaderControl.BadgeIndicator.MaxSingleSpanTextLength;
      badgeIndicator.Ellipse.Fill = (Brush) UIUtils.AccentBrush;
      this.badges.Add(badgeIndicator);
    }

    public void ShowSelected(int count)
    {
      if (count > 0)
      {
        this.SetCount(this.refPivot.Items.Count, count);
        for (int index = 1; index < this.refPivot.Items.Count; ++index)
        {
          this.LayoutRoot.Children[2 * index].Visibility = Visibility.Collapsed;
          this.LayoutRoot.Children[2 * index + 1].Visibility = Visibility.Collapsed;
        }
        this.badges[0].Ellipse.Fill = this.Resources[(object) "PhoneInactiveBrush"] as Brush;
        this.badges[this.refPivot.Items.Count].Ellipse.Fill = (Brush) UIUtils.AccentBrush;
        this.selectedStackPanel.Visibility = Visibility.Visible;
      }
      else
      {
        this.selectedStackPanel.Visibility = Visibility.Collapsed;
        this.badges[0].Ellipse.Fill = (Brush) UIUtils.AccentBrush;
        for (int index = 1; index < this.refPivot.Items.Count; ++index)
        {
          this.LayoutRoot.Children[2 * index].Visibility = Visibility.Visible;
          this.LayoutRoot.Children[2 * index + 1].Visibility = Visibility.Visible;
        }
        this.SetCount(this.refPivot.Items.Count, 0);
      }
    }

    public void UpdateAsync(
      bool updateChats,
      bool updateCalls,
      bool updateStatus,
      TimeSpan? delay = null)
    {
      Log.d("pagetitle", "update async | update chats:{0},update calls:{1},update status:{2}", (object) updateChats, (object) updateCalls, (object) updateStatus);
      Action a = (Action) (() =>
      {
        int? unreadChats = new int?();
        int? missedCalls = new int?();
        bool? hasNewStatus = new bool?();
        lock (this.enableUpdateLock)
        {
          if (!this.isUpdateEnabled)
          {
            Log.l("pagetitle", "update async | skipped");
            return;
          }
          if (updateChats | updateStatus)
          {
            DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              if (updateChats)
              {
                unreadChats = new int?(db.GetUnreadConversationsCount(false));
                if (this.chatsChangedSub == null)
                  this.chatsChangedSub = db.UpdatedConversationSubject.ObserveOn<ConvoAndMessage>((IScheduler) AppState.Worker).Subscribe<ConvoAndMessage>((Action<ConvoAndMessage>) (_ => this.UpdateAsync(true, false, false)));
              }
              if (!updateStatus)
                return;
              TimeSpan withinTimespan = Settings.LastSeenStatusListTimeUtc.HasValue ? FunRunner.CurrentServerTimeUtc - Settings.LastSeenStatusListTimeUtc.Value : WaStatus.Expiration;
              WaStatus waStatus = ((IEnumerable<WaStatus>) db.GetStatuses((string[]) null, new string[1]
              {
                Settings.MyJid
              }, true, true, new TimeSpan?(withinTimespan), 1)).FirstOrDefault<WaStatus>();
              if (waStatus == null)
              {
                hasNewStatus = new bool?(false);
              }
              else
              {
                JidInfo jidInfo = db.GetJidInfo(waStatus.Jid, CreateOptions.None);
                hasNewStatus = (jidInfo != null ? (jidInfo.IsStatusMuted ? 1 : 0) : 0) == 0 ? new bool?(true) : new bool?(db.GetStatusThreads(false, withinTimespan, false, new int?(1)).Any<WaStatusThread>());
              }
              if (this.newStatusSub != null)
                return;
              this.newStatusSub = db.NewMessagesSubject.Where<Message>((Func<Message, bool>) (m => !m.KeyFromMe && m.IsStatus())).ObserveOn<Message>((IScheduler) AppState.Worker).Subscribe<Message>((Action<Message>) (m =>
              {
                bool isMuted = false;
                MessagesContext.Run((MessagesContext.MessagesCallback) (db2 =>
                {
                  JidInfo jidInfo = db2.GetJidInfo(m.GetSenderJid(), CreateOptions.None);
                  isMuted = jidInfo != null && jidInfo.IsStatusMuted;
                }));
                if (isMuted)
                  return;
                this.UpdateAsync(false, false, true);
              }));
            }));
            PerformanceTimer.End("pagetitle", "unread chats count and new status query", start);
          }
          if (updateCalls)
          {
            DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
            missedCalls = new int?(CallLog.GetMissedCallCount(Settings.LastSeenMissedCallTimeUtc));
            PerformanceTimer.End("pagetitle", "missed call count query", start);
            if (this.missedCallsSub == null)
              this.missedCallsSub = CallLog.GetUpdateObservable().ObserveOn<CallRecordUpdate[]>((IScheduler) AppState.Worker).Subscribe<CallRecordUpdate[]>((Action<CallRecordUpdate[]>) (_ => this.UpdateAsync(false, true, false)));
          }
        }
        if (!unreadChats.HasValue && !missedCalls.HasValue && !hasNewStatus.HasValue)
          return;
        this.Dispatcher.BeginInvoke((Action) (() => this.UpdateUI(unreadChats, missedCalls, hasNewStatus)));
      });
      if (delay.HasValue)
        AppState.Worker.RunAfterDelay(delay.Value, a);
      else
        AppState.Worker.Enqueue(a);
    }

    public void EnableUpdate(bool enable)
    {
      AppState.Worker.Enqueue((Action) (() =>
      {
        lock (this.enableUpdateLock)
        {
          this.isUpdateEnabled = enable;
          if (this.isUpdateEnabled)
            return;
          this.DisposeSubscriptions();
        }
      }));
    }

    public void DisposeSubscriptions()
    {
      this.chatsChangedSub.SafeDispose();
      this.chatsChangedSub = (IDisposable) null;
      this.missedCallsSub.SafeDispose();
      this.missedCallsSub = (IDisposable) null;
    }

    public void ClearMissedCalls()
    {
      this.Dispatcher.RunAfterDelay(TimeSpan.FromMilliseconds(350.0), (Action) (() => this.SetCount(1, 0)));
    }

    public void ClearNewStatusIndicator()
    {
      this.Dispatcher.RunAfterDelay(TimeSpan.FromMilliseconds(350.0), (Action) (() => this.SetIndicator(2, false)));
    }

    private void UpdateUI(int? unreadChats, int? missedCalls, bool? newStatus)
    {
      if (unreadChats.HasValue)
      {
        if (unreadChats.Value > 0)
          this.SetCount(0, unreadChats.Value);
        else
          this.SetCount(0, 0);
      }
      if (missedCalls.HasValue)
      {
        if (missedCalls.Value > 0)
          this.SetCount(1, missedCalls.Value);
        else
          this.ClearMissedCalls();
      }
      if (!newStatus.HasValue)
        return;
      this.SetIndicator(2, newStatus.Value);
    }
  }
}
