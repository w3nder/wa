// Decompiled with JetBrains decompiler
// Type: WhatsApp.ForegroundLifetimeAgent
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Networking.Voip;
using System;
using System.Threading;

#nullable disable
namespace WhatsApp
{
  public class ForegroundLifetimeAgent : VoipForegroundLifetimeAgent, IWaBackgroundAgent
  {
    private IDisposable outOfProc;
    private BackgroundAgent agent = new BackgroundAgent("FG Lifetime", false);

    public string LogHeader => this.agent.LogHeader;

    protected override void OnLaunched()
    {
      Log.l(this.LogHeader, nameof (OnLaunched));
      this.outOfProc = BackgroundAgent.RegisterOutOfProc();
    }

    protected override void OnCancel()
    {
      Log.l(this.LogHeader, nameof (OnCancel));
      using (EventWaitHandle eventWaitHandle = WhatsApp.Voip.EventForPid(NativeInterfaces.Misc.GetProcessId()))
        eventWaitHandle.WaitOne();
      this.outOfProc.SafeDispose();
      this.agent.PerformAndLog("NotifyComplete", new Action(((Microsoft.Phone.BackgroundAgent) this).NotifyComplete));
    }
  }
}
