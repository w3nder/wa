// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactClasses.ContactEmailAddress
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Collections.Generic;

#nullable disable
namespace WhatsApp.ContactClasses
{
  public class ContactEmailAddress
  {
    public IEnumerable<Account> Accounts { get; internal set; }

    public string EmailAddress { get; internal set; }

    public EmailAddressKind Kind { get; internal set; }
  }
}
