﻿// Decompiled with JetBrains decompiler
// Type: WhatsApp.WAGroupCallChangedArgs
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Collections.Generic;

#nullable disable
namespace WhatsApp
{
  public class WAGroupCallChangedArgs
  {
    public List<CallParticipantDetail> Peers { get; private set; }

    public WAGroupCallChangedArgs(List<CallParticipantDetail> peers) => this.Peers = peers;
  }
}