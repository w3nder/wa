// Decompiled with JetBrains decompiler
// Type: WhatsApp.CallLogItemControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WhatsApp.CommonOps;
using WhatsApp.WaCollections;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class CallLogItemControl : ItemControlBase
  {
    private const int MaxNumberOfPeers = 3;
    private List<IDisposable> iconSubs = new List<IDisposable>();
    private List<Image> icons = new List<Image>();
    private Dictionary<int, Pair<string, System.Windows.Media.ImageSource>> assignedIcons = new Dictionary<int, Pair<string, System.Windows.Media.ImageSource>>();
    private ColumnDefinition secondColumn;
    private RowDefinition secondRow;
    protected CallLogItemTitleBlock callLogItemTitleBlock;
    protected StackPanel buttonsStackPanel;
    protected RoundButton callButton;
    protected RoundButton videoCallButton;

    protected override void InitComponents()
    {
      base.InitComponents();
      this.iconPanel.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      this.secondColumn = new ColumnDefinition()
      {
        Width = new GridLength(0.0)
      };
      this.iconPanel.ColumnDefinitions.Add(this.secondColumn);
      this.iconPanel.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Star)
      });
      this.secondRow = new RowDefinition()
      {
        Height = new GridLength(0.0)
      };
      this.iconPanel.RowDefinitions.Add(this.secondRow);
      Grid.SetColumn((FrameworkElement) this.icon, 0);
      Grid.SetRow((FrameworkElement) this.icon, 0);
      Grid.SetRowSpan((FrameworkElement) this.icon, 2);
      this.icons.Add(this.icon);
      for (int index = 1; index < 3; ++index)
      {
        Image image = new Image();
        image.Stretch = Stretch.UniformToFill;
        image.HorizontalAlignment = HorizontalAlignment.Center;
        image.VerticalAlignment = VerticalAlignment.Center;
        Image element = image;
        this.icons.Add(element);
        this.iconPanel.Children.Add((UIElement) element);
        Grid.SetColumn((FrameworkElement) element, 1);
        Grid.SetRow((FrameworkElement) element, index - 1);
      }
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Auto)
      });
      StackPanel stackPanel = new StackPanel();
      stackPanel.Orientation = Orientation.Horizontal;
      stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
      this.buttonsStackPanel = stackPanel;
      RoundButton roundButton1 = new RoundButton();
      roundButton1.ButtonSize = 42.0;
      roundButton1.VerticalAlignment = VerticalAlignment.Center;
      roundButton1.HorizontalAlignment = HorizontalAlignment.Right;
      roundButton1.IsActivatable = true;
      roundButton1.ButtonIcon = AssetStore.VideoCallIconWhiteSolid;
      this.videoCallButton = roundButton1;
      this.videoCallButton.Tap += new EventHandler<GestureEventArgs>(this.VideoCallButton_Tap);
      this.buttonsStackPanel.Children.Add((UIElement) this.videoCallButton);
      RoundButton roundButton2 = new RoundButton();
      roundButton2.ButtonSize = 42.0;
      roundButton2.Margin = new Thickness(30.0, 0.0, 0.0, 0.0);
      roundButton2.VerticalAlignment = VerticalAlignment.Center;
      roundButton2.HorizontalAlignment = HorizontalAlignment.Right;
      roundButton2.IsActivatable = true;
      roundButton2.ButtonIcon = AssetStore.PhoneIconWhiteSolid;
      this.callButton = roundButton2;
      this.callButton.Tap += new EventHandler<GestureEventArgs>(this.CallButton_Tap);
      this.buttonsStackPanel.Children.Add((UIElement) this.callButton);
      this.titleRow.Children.Clear();
      CallLogItemTitleBlock logItemTitleBlock = new CallLogItemTitleBlock();
      logItemTitleBlock.TextFontSize = UIUtils.FontSizeLarge;
      logItemTitleBlock.Margin = new Thickness(0.0);
      logItemTitleBlock.FontFamily = UIUtils.FontFamilySemiLight;
      logItemTitleBlock.VerticalAlignment = VerticalAlignment.Top;
      logItemTitleBlock.VerticalContentAlignment = VerticalAlignment.Top;
      this.callLogItemTitleBlock = logItemTitleBlock;
      this.titleRow.Children.Add((UIElement) this.callLogItemTitleBlock);
      Grid.SetColumn((FrameworkElement) this.callLogItemTitleBlock, 0);
      this.titleRow.HorizontalAlignment = HorizontalAlignment.Left;
      this.detailsPanel.Tap += new EventHandler<GestureEventArgs>(this.ContentRegion_Tap);
      this.iconPanel.Tap += new EventHandler<GestureEventArgs>(this.ContentRegion_Tap);
    }

    protected override void UpdateComponents(JidItemViewModel vm)
    {
      base.UpdateComponents(vm);
      this.secondRow.Height = new GridLength(0.0);
      this.secondColumn.Width = new GridLength(0.0);
      foreach (FrameworkElement icon in this.icons)
        icon.Margin = new Thickness(0.0);
      if (!(vm is CallLogItemViewModel logItemViewModel))
        return;
      this.Children.Remove((UIElement) this.buttonsStackPanel);
      if (!logItemViewModel.IsGroupCall)
      {
        this.Children.Add((UIElement) this.buttonsStackPanel);
        Grid.SetColumn((FrameworkElement) this.buttonsStackPanel, 2);
      }
      List<CallRecord.CallLogEntryParticipant> participantEntriesSorted = logItemViewModel.Calls.FirstOrDefault<CallRecord>()?.ParticipantEntriesSorted;
      if (participantEntriesSorted != null)
      {
        for (int index = 0; index < 3; ++index)
        {
          Image image = this.icons.ElementAt<Image>(index);
          participantEntriesSorted.ElementAtOrDefault<CallRecord.CallLogEntryParticipant>(index);
          if (this.assignedIcons.ContainsKey(index))
          {
            if (participantEntriesSorted.Count <= index || participantEntriesSorted.ElementAt<CallRecord.CallLogEntryParticipant>(index).jid != this.assignedIcons[index].First || image.Source != this.assignedIcons[index].Second)
            {
              image.Source = (System.Windows.Media.ImageSource) null;
              this.assignedIcons.Remove(index);
            }
          }
          else
            image.Source = (System.Windows.Media.ImageSource) null;
        }
        if (participantEntriesSorted.Count > 1)
        {
          this.secondColumn.Width = new GridLength(1.0, GridUnitType.Star);
          this.icons.ElementAt<Image>(0).Margin = new Thickness(0.0, 0.0, 1.0, 0.0);
          this.icons.ElementAt<Image>(1).Margin = new Thickness(1.0, 0.0, 0.0, 0.0);
          this.iconPanel.Background = (Brush) UIUtils.TransparentBrush;
        }
        if (participantEntriesSorted.Count > 2)
        {
          this.secondRow.Height = new GridLength(1.0, GridUnitType.Star);
          this.icons.ElementAt<Image>(1).Margin = new Thickness(1.0, 0.0, 0.0, 1.0);
          this.icons.ElementAt<Image>(2).Margin = new Thickness(1.0, 1.0, 0.0, 0.0);
        }
      }
      logItemViewModel.GetSubtitleObservable().Take<RichTextBlock.TextSet>(1).ObserveOnDispatcher<RichTextBlock.TextSet>().Subscribe<RichTextBlock.TextSet>((Action<RichTextBlock.TextSet>) (s => this.subtitleBlock.Text = s));
      this.subtitleBlock.Foreground = logItemViewModel.SubtitleBrush;
      this.subtitleBlock.FontWeight = logItemViewModel.SubtitleWeight;
      this.callButton.IsEnabled = !CallLogItemViewModel.IsInCall;
      if (this.videoCallButton == null)
        return;
      this.videoCallButton.IsEnabled = !CallLogItemViewModel.IsInCall;
    }

    protected override void UpdateTitleRow(JidItemViewModel vm, bool useCache)
    {
      CallLogItemViewModel cvm = vm as CallLogItemViewModel;
      if (cvm == null)
        return;
      this.titleSub.SafeDispose();
      this.titleSub = cvm.GetRichTitleObservable(!useCache).ObserveOnDispatcherIfNeeded<RichTextBlock.TextSet>().Subscribe<RichTextBlock.TextSet>((Action<RichTextBlock.TextSet>) (t =>
      {
        if (this.ViewModel != vm)
          return;
        if (cvm.IsGroupCall)
          this.callLogItemTitleBlock.SetContent(t.Text, cvm.GroupCountStr, cvm.ParticipantFirstNames);
        else
          this.callLogItemTitleBlock.SetContent(t.Text, cvm.CountStr);
      }), (Action) (() => this.titleSub = (IDisposable) null));
    }

    private void ContentRegion_Tap(object sender, EventArgs e)
    {
      if (!(this.ViewModel is CallLogItemViewModel viewModel))
        return;
      CallRecord mostRecentCall = viewModel.MostRecentCall;
      if (mostRecentCall == null)
        return;
      if (mostRecentCall.IsGroupCall)
        GroupCallInfoPage.Start(mostRecentCall);
      else
        ContactInfoPage.Start(UserCache.Get(mostRecentCall.PeerJid, true), startingTab: ContactInfoPage.Tab.Calls);
    }

    private void CallButton_Tap(object sender, EventArgs e)
    {
      if (!(this.ViewModel is CallLogItemViewModel viewModel))
        return;
      CallRecord mostRecentCall = viewModel.MostRecentCall;
      if (mostRecentCall == null)
        return;
      CallContact.Call(mostRecentCall.PeerJid, context: "from call log");
    }

    private void VideoCallButton_Tap(object sender, EventArgs e)
    {
      if (!(this.ViewModel is CallLogItemViewModel viewModel))
        return;
      CallRecord mostRecentCall = viewModel.MostRecentCall;
      if (mostRecentCall == null)
        return;
      CallContact.VideoCall(mostRecentCall.PeerJid);
    }

    protected override void DisposeShortSubscriptions()
    {
      this.iconSubs.ForEach((Action<IDisposable>) (s => s.SafeDispose()));
      this.iconSubs.Clear();
      base.DisposeShortSubscriptions();
    }

    protected override void ResetIconSubscription(JidItemViewModel vm)
    {
      this.iconSubs.ForEach((Action<IDisposable>) (s => s.SafeDispose()));
      this.iconSubs.Clear();
      if (!(vm is CallLogItemViewModel logItemViewModel) || !logItemViewModel.IsGroupCall)
      {
        base.ResetIconSubscription(vm);
      }
      else
      {
        CallRecord callRecord = logItemViewModel.Calls.FirstOrDefault<CallRecord>();
        if (callRecord == null)
          return;
        List<CallRecord.CallLogEntryParticipant> participantEntriesSorted = callRecord.ParticipantEntriesSorted;
        for (int index = 0; index < Math.Min(participantEntriesSorted.Count, 3); ++index)
        {
          int order = index;
          CallRecord.CallLogEntryParticipant participant = participantEntriesSorted.ElementAt<CallRecord.CallLogEntryParticipant>(order);
          this.iconSubs.Add(vm.GetPictureSourceObservable(participant.jid, true, true).SubscribeOn<System.Windows.Media.ImageSource>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<System.Windows.Media.ImageSource>().Subscribe<System.Windows.Media.ImageSource>((Action<System.Windows.Media.ImageSource>) (iconSrc => this.OnIconFetched(vm, order, participant.jid, iconSrc)), (Action<Exception>) (ex => this.OnIconFetchFailed(vm, order, participant.jid))));
        }
      }
    }

    private void OnIconFetched(
      JidItemViewModel vm,
      int participantOrder,
      string jid,
      System.Windows.Media.ImageSource iconSrc)
    {
      if (iconSrc == null)
        iconSrc = vm.GetDefaultPicture();
      Pair<string, System.Windows.Media.ImageSource> valueOrDefault = this.assignedIcons.GetValueOrDefault<int, Pair<string, System.Windows.Media.ImageSource>>(participantOrder);
      if (valueOrDefault != null && !(valueOrDefault.First != jid) && valueOrDefault.Second == iconSrc)
        return;
      Image image = this.icons.ElementAtOrDefault<Image>(participantOrder);
      if (image == null)
        return;
      image.Source = iconSrc;
      this.assignedIcons[participantOrder] = new Pair<string, System.Windows.Media.ImageSource>(jid, iconSrc);
    }

    private void OnIconFetchFailed(JidItemViewModel vm, int participantOrder, string jid)
    {
      Image image = this.icons.ElementAtOrDefault<Image>(participantOrder);
      if (image == null)
        return;
      System.Windows.Media.ImageSource defaultPicture = vm.GetDefaultPicture();
      image.Source = defaultPicture;
      this.assignedIcons[participantOrder] = new Pair<string, System.Windows.Media.ImageSource>(jid, defaultPicture);
    }
  }
}
