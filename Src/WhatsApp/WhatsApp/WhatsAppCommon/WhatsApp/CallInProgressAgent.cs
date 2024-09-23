// Decompiled with JetBrains decompiler
// Type: WhatsApp.CallInProgressAgent
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Networking.Voip;
using Microsoft.Phone.Reactive;
using System;
using System.Threading;


namespace WhatsApp
{
  public class CallInProgressAgent : VoipCallInProgressAgent, IWaBackgroundAgent
  {
    private static TreeInForestSubject<Unit> socketRequestSubject = new TreeInForestSubject<Unit>();
    private static object bgSockSub = (object) null;
    private IDisposable outOfProc;
    private BackgroundAgent agent = new BackgroundAgent("InCall Agent", false);
    private IDisposable sockReqSub;
    private IDisposable killSub;

    public string LogHeader => this.agent.LogHeader;

    private static void WaitForBgSocketRequest()
    {
      while (true)
      {
        WhatsApp.Voip.BgSockRequestEvent.WaitOne();
        CallInProgressAgent.socketRequestSubject.OnNext(new Unit());
      }
    }

    public CallInProgressAgent() => this.OnNewAgent(this.agent);

    private void OnNewAgent(BackgroundAgent agent)
    {
      agent.AddDtor((Action) (() =>
      {
        Log.l(this.LogHeader, "reset agent state for the next run");
        BackgroundAgent agent1 = new BackgroundAgent("InCall Agent", false);
        this.OnNewAgent(agent1);
        this.agent = agent1;
      }));
    }

    protected override void OnFirstCallStarting()
    {
      Log.l(this.LogHeader, nameof (OnFirstCallStarting));
      Utils.LazyInit<object>(ref CallInProgressAgent.bgSockSub, (Func<object>) (() =>
      {
        new Thread(new ThreadStart(CallInProgressAgent.WaitForBgSocketRequest)).Start();
        return new object();
      }));
      this.outOfProc = BackgroundAgent.RegisterOutOfProc();
      WhatsApp.Voip.InCallEvent.Set();
      this.sockReqSub = CallInProgressAgent.socketRequestSubject.Repeat<Unit>().Subscribe<Unit>((Action<Unit>) (_ =>
      {
        bool dupe = false;
        Log.l(this.LogHeader, "Background socket was requested");
        BackgroundAgent.SyncWorker.InvokeSynchronous((Action) (() =>
        {
          if (this.killSub != null)
          {
            Log.l(this.LogHeader, "socket is already active, bailing early");
            dupe = true;
          }
          else
          {
            this.agent.AddDtor((Action) (() =>
            {
              Log.l(this.LogHeader, "background socket tearing down");
              BackgroundAgent.SyncWorker.InvokeSynchronous((Action) (() =>
              {
                this.killSub.SafeDispose();
                this.killSub = (IDisposable) null;
              }));
            }));
            this.killSub = this.agent.KillEventListen();
          }
        }));
        if (dupe)
          return;
        this.agent.Invoke(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan, false, BackgroundAgent.BackgroundAgentType.AudioAgent);
      }));
    }

    protected override void OnCancel()
    {
      Log.l(this.LogHeader, nameof (OnCancel));
      this.outOfProc.SafeDispose();
      WhatsApp.Voip.InCallEvent.Reset();
      Action onComplete = (Action) (() => this.agent.PerformAndLog("NotifyComplete", new Action(((Microsoft.Phone.BackgroundAgent) this).NotifyComplete)));
      BackgroundAgent.SyncWorker.Enqueue((Action) (() =>
      {
        this.sockReqSub.SafeDispose();
        this.sockReqSub = (IDisposable) null;
        if (this.killSub != null)
        {
          this.agent.AddDtor(onComplete);
          BackgroundAgent snap = this.agent;
          ThreadPool.QueueUserWorkItem((WaitCallback) (_ => snap.Die()));
        }
        else
          onComplete();
      }));
    }
  }
}
