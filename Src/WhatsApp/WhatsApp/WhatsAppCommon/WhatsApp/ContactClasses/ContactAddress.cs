// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactClasses.ContactAddress
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Collections.Generic;
using System.Device.Location;


namespace WhatsApp.ContactClasses
{
  public class ContactAddress
  {
    public IEnumerable<Account> Accounts { get; internal set; }

    public AddressKind Kind { get; internal set; }

    public CivicAddress PhysicalAddress { get; internal set; }
  }
}
