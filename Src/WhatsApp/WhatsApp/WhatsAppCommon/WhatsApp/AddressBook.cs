// Decompiled with JetBrains decompiler
// Type: WhatsApp.AddressBook
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using WhatsApp.AddressBookImpl;


namespace WhatsApp
{
  public static class AddressBook
  {
    private static IAddressBook instance;

    public static IAddressBook Instance
    {
      get
      {
        return Utils.LazyInit<IAddressBook>(ref AddressBook.instance, (Func<IAddressBook>) (() =>
        {
          bool flag = false;
          if (AppState.IsWP10OrLater)
            flag = true;
          return flag ? (IAddressBook) new WinRtContacts() : (IAddressBook) new Wp7Contacts();
        }));
      }
    }
  }
}
