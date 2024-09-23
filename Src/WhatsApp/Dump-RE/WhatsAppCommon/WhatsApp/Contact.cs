// Decompiled with JetBrains decompiler
// Type: WhatsApp.Contact
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using WhatsApp.ContactClasses;

#nullable disable
namespace WhatsApp
{
  public interface Contact
  {
    IEnumerable<Account> Accounts { get; }

    string DisplayName { get; }

    CompleteName CompleteName { get; }

    IEnumerable<DateTime> Birthdays { get; }

    IEnumerable<ContactPhoneNumber> PhoneNumbers { get; }

    IEnumerable<ContactEmailAddress> EmailAddresses { get; }

    IEnumerable<ContactAddress> Addresses { get; }

    IEnumerable<ContactCompanyInformation> Companies { get; }

    IEnumerable<string> Websites { get; }

    IEnumerable<string> Notes { get; }

    Stream GetPicture();

    string GetIdentifier();
  }
}
