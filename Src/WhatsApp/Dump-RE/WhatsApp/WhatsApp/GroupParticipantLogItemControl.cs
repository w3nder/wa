// Decompiled with JetBrains decompiler
// Type: WhatsApp.GroupParticipantLogItemControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WhatsApp.CommonOps;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class GroupParticipantLogItemControl : CallLogItemControl
  {
    protected override void InitComponents()
    {
      base.InitComponents();
      this.callButton.Tap += new EventHandler<GestureEventArgs>(this.CallButton_Tap);
      this.videoCallButton.Tap += new EventHandler<GestureEventArgs>(this.VideoCallButton_Tap);
    }

    protected override void UpdateComponents(JidItemViewModel vm)
    {
      base.UpdateComponents(vm);
      if (!(vm is GroupParticipantLogItemViewModel logItemViewModel))
        return;
      CallRecord.CallResult res = logItemViewModel.Participant.res;
      if (res == CallRecord.CallResult.Connected)
      {
        this.subtitleBlock.Visibility = Visibility.Collapsed;
        this.callLogItemTitleBlock.VerticalAlignment = VerticalAlignment.Center;
      }
      else
        this.subtitleBlock.Text = new RichTextBlock.TextSet()
        {
          Text = res.ToString()
        };
      if (!this.Children.Contains((UIElement) this.buttonsStackPanel))
        this.Children.Add((UIElement) this.buttonsStackPanel);
      Grid.SetColumn((FrameworkElement) this.buttonsStackPanel, 2);
      this.buttonsStackPanel.Visibility = Visibility.Visible;
      this.callButton.IsEnabled = !GroupParticipantLogItemViewModel.IsInCall;
      if (this.videoCallButton == null)
        return;
      this.videoCallButton.IsEnabled = !GroupParticipantLogItemViewModel.IsInCall;
    }

    protected override void UpdateTitleRow(JidItemViewModel vm, bool useCache)
    {
      if (!(vm is GroupParticipantLogItemViewModel logItemViewModel))
        return;
      this.titleSub.SafeDispose();
      this.titleSub = logItemViewModel.GetRichTitleObservable(!useCache).ObserveOnDispatcherIfNeeded<RichTextBlock.TextSet>().Subscribe<RichTextBlock.TextSet>((Action<RichTextBlock.TextSet>) (t =>
      {
        if (this.ViewModel != vm)
          return;
        this.callLogItemTitleBlock.SetContent(t.Text, "");
      }), (Action) (() => this.titleSub = (IDisposable) null));
    }

    private void CallButton_Tap(object sender, EventArgs e)
    {
      if (!(this.ViewModel is GroupParticipantLogItemViewModel viewModel))
        return;
      CallContact.Call(viewModel.Participant.jid, context: "from group call info page");
    }

    private void VideoCallButton_Tap(object sender, EventArgs e)
    {
      if (!(this.ViewModel is GroupParticipantLogItemViewModel viewModel))
        return;
      CallContact.VideoCall(viewModel.Participant.jid, context: "from group call info page");
    }
  }
}
