// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.GroupParticipantLogItemViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Windows.Media;

#nullable disable
namespace WhatsApp.WaViewModels
{
  public class GroupParticipantLogItemViewModel : UserViewModel
  {
    private static bool? isInCall;
    private static IDisposable voipCallGlobalSub = (IDisposable) null;
    private static Subject<bool> voipCallGlobalSubject = new Subject<bool>();

    public CallRecord.CallLogEntryParticipant Participant { get; private set; }

    public GroupParticipantLogItemViewModel(
      UserStatus user,
      CallRecord.CallLogEntryParticipant participant)
      : base(user, false)
    {
      this.Participant = participant;
    }

    public override Brush SubtitleBrush
    {
      get
      {
        return this.Participant.res == CallRecord.CallResult.Connected ? UIUtils.SubtleBrush : (Brush) UIUtils.AccentBrush;
      }
    }

    public override IDisposable ActivateLazySubscriptions()
    {
      return (IDisposable) new DisposableChain(new IDisposable[2]
      {
        GroupParticipantLogItemViewModel.voipCallGlobalSubject.Subscribe<bool>(new Action<bool>(this.Voip_CallStateChanged)),
        base.ActivateLazySubscriptions()
      });
    }

    public static bool IsInCall
    {
      get
      {
        if (!GroupParticipantLogItemViewModel.isInCall.HasValue)
        {
          GroupParticipantLogItemViewModel.voipCallGlobalSub = VoipHandler.CallStateChangedSubject.ObserveOnDispatcher<WaCallStateChangedArgs>().Subscribe<WaCallStateChangedArgs>(new Action<WaCallStateChangedArgs>(GroupParticipantLogItemViewModel.Voip_GlobalCallStateChanged));
          GroupParticipantLogItemViewModel.isInCall = new bool?(Voip.IsInCall);
        }
        return GroupParticipantLogItemViewModel.isInCall.Value;
      }
      private set
      {
        bool? isInCall = GroupParticipantLogItemViewModel.isInCall;
        bool flag = value;
        if ((isInCall.GetValueOrDefault() == flag ? (!isInCall.HasValue ? 1 : 0) : 1) == 0)
          return;
        GroupParticipantLogItemViewModel.isInCall = new bool?(value);
        GroupParticipantLogItemViewModel.voipCallGlobalSubject.OnNext(value);
      }
    }

    private static void Voip_GlobalCallStateChanged(WaCallStateChangedArgs state)
    {
      GroupParticipantLogItemViewModel.IsInCall = state.CurrState != 0;
    }

    private void Voip_CallStateChanged(bool isInCall) => this.Refresh();
  }
}
