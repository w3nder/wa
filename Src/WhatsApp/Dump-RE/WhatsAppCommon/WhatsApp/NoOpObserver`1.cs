// Decompiled with JetBrains decompiler
// Type: WhatsApp.NoOpObserver`1
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;

#nullable disable
namespace WhatsApp
{
  public class NoOpObserver<T> : IObserver<T>
  {
    public void OnNext(T obj)
    {
    }

    public void OnError(Exception e)
    {
    }

    public void OnCompleted()
    {
    }
  }
}
