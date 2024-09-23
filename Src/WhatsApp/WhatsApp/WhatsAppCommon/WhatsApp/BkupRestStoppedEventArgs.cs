// Decompiled with JetBrains decompiler
// Type: WhatsApp.BkupRestStoppedEventArgs
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public sealed class BkupRestStoppedEventArgs : EventArgs
  {
    public OneDriveBkupRestStopReason Reason { get; internal set; }

    public OneDriveBkupRestTrigger? StartTrigger { get; internal set; }

    public OneDriveBkupRestTrigger? ChangedTrigger { get; internal set; }
  }
}
