// Decompiled with JetBrains decompiler
// Type: WhatsApp.IExternalShare
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace WhatsApp
{
  public interface IExternalShare
  {
    IObservable<ExternalShare.ExternalShareResult> ShareContent(List<string> jids);

    string DescribeError(ExternalShare.ExternalShareResult result);

    bool ShouldConfirmSending();

    IObservable<bool> GetTruncationCheck();

    IObservable<bool> ShouldEnableSharingToStatus();
  }
}
