// Decompiled with JetBrains decompiler
// Type: WhatsApp.PausedThread
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp
{
  public class PausedThread
  {
    private Action wait;
    private Action cancel;

    public PausedThread(Action wait, Action cancel)
    {
      this.wait = wait;
      this.cancel = cancel;
    }

    public PausedThread(params PausedThread[] sources)
    {
      List<PausedThread> sourcesList = ((IEnumerable<PausedThread>) sources).ToList<PausedThread>();
      this.wait = (Action) (() => sourcesList.ForEach((Action<PausedThread>) (src => src.WaitForPauseCompleted())));
      this.cancel = (Action) (() => sourcesList.ForEach((Action<PausedThread>) (src => src.Unpause())));
    }

    public void Unpause()
    {
      this.cancel();
      this.cancel = (Action) (() => { });
    }

    public void WaitForPauseCompleted() => this.wait();
  }
}
