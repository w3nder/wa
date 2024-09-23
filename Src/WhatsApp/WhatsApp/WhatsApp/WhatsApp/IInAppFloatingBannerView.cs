// Decompiled with JetBrains decompiler
// Type: WhatsApp.IInAppFloatingBannerView
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;


namespace WhatsApp
{
  public interface IInAppFloatingBannerView
  {
    event EventHandler DragStarted;

    event EventHandler DragEnded;

    event EventHandler Dismissed;

    event EventHandler Tapped;

    double GetTargetHeight();

    double GetMaxHeight();

    double GetActualHeight();

    IObservable<Unit> HandleTimeout();
  }
}
