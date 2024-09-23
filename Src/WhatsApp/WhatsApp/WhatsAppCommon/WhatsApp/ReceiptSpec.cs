// Decompiled with JetBrains decompiler
// Type: WhatsApp.ReceiptSpec
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public struct ReceiptSpec
  {
    public string Jid;
    public string Id;
    public string Participant;
    public bool IsCipherText;
    public DateTime? MessageTimestamp;
  }
}
