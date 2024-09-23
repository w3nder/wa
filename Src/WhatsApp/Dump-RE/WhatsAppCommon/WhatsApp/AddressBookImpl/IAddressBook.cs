// Decompiled with JetBrains decompiler
// Type: WhatsApp.AddressBookImpl.IAddressBook
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Threading.Tasks;

#nullable disable
namespace WhatsApp.AddressBookImpl
{
  public interface IAddressBook
  {
    IObservable<AddressBookSearchArgs> GetContactById(string id);

    IObservable<AddressBookSearchArgs> GetContactByPhoneNumber(string number);

    IObservable<AddressBookSearchArgs> GetAllContacts();

    Task<AddressBookSearchArgs> GetAllContactsAsync();
  }
}
