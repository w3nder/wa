// Decompiled with JetBrains decompiler
// Type: WhatsApp.AsyncAction
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;

#nullable disable
namespace WhatsApp
{
  public class AsyncAction
  {
    private object @lock = new object();
    private bool shouldRun;
    private Action realAction;

    public void Perform()
    {
      lock (this.@lock)
      {
        if (this.realAction != null)
          this.realAction();
        else
          this.shouldRun = true;
      }
    }

    public void SetAction(Action a)
    {
      lock (this.@lock)
      {
        this.realAction = a;
        if (!this.shouldRun)
          return;
        a();
      }
    }
  }
}
