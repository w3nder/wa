// Decompiled with JetBrains decompiler
// Type: WhatsApp.RemoteClientCaps
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class RemoteClientCaps
  {
    public string Jid;
    public List<Triad<ClientCapabilityCategory, ClientCapabilitySetting, DateTime?>> Values = new List<Triad<ClientCapabilityCategory, ClientCapabilitySetting, DateTime?>>();

    public void AddValue(
      ClientCapabilityCategory category,
      ClientCapabilitySetting setting,
      DateTime? dt)
    {
      this.Values.Add(new Triad<ClientCapabilityCategory, ClientCapabilitySetting, DateTime?>(category, setting, dt));
    }
  }
}
