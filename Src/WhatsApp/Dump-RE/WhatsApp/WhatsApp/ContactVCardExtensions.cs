// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactVCardExtensions
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Tasks;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace WhatsApp
{
  public static class ContactVCardExtensions
  {
    public static SaveContactTask ToSaveContactTask(this ContactVCard vcard)
    {
      SaveContactTask task = new SaveContactTask();
      bool flag1 = false;
      if (vcard.NameTitle != null)
        task.Title = vcard.NameTitle;
      if (vcard.FirstName != null)
        task.FirstName = vcard.FirstName;
      if (vcard.MiddleName != null)
        task.MiddleName = vcard.MiddleName;
      if (vcard.LastName != null)
        task.LastName = vcard.LastName;
      if (vcard.Suffix != null)
        task.Suffix = vcard.Suffix;
      if (vcard.Nickname != null)
        task.Nickname = vcard.Nickname;
      List<ContactVCard.PhoneNumber> phoneNumberList = new List<ContactVCard.PhoneNumber>();
      foreach (ContactVCard.PhoneNumber phoneNumber in vcard.PhoneNumbers)
      {
        if (phoneNumber.Kind == PhoneNumberKind.Mobile && task.MobilePhone == null)
          task.MobilePhone = phoneNumber.Number;
        else if (phoneNumber.Kind == PhoneNumberKind.Home && task.HomePhone == null)
          task.HomePhone = phoneNumber.Number;
        else if (phoneNumber.Kind == PhoneNumberKind.Work && task.WorkPhone == null)
          task.WorkPhone = phoneNumber.Number;
        else
          phoneNumberList.Add(phoneNumber);
      }
      foreach (ContactVCard.PhoneNumber phoneNumber in phoneNumberList)
      {
        if (task.MobilePhone == null)
          task.MobilePhone = phoneNumber.Number;
        else if (task.HomePhone == null)
          task.HomePhone = phoneNumber.Number;
        else if (task.WorkPhone == null)
          task.WorkPhone = phoneNumber.Number;
      }
      int num1;
      bool flag2 = (num1 = 0) != 0;
      bool flag3 = num1 != 0;
      bool flag4 = num1 != 0;
      foreach (ContactVCard.EmailAddress emailAddress in vcard.EmailAddresses)
      {
        if (!flag4)
        {
          task.WorkEmail = emailAddress.Address;
          flag4 = true;
        }
        else if (!flag3)
        {
          task.PersonalEmail = emailAddress.Address;
          flag3 = true;
        }
        else if (!flag2)
        {
          task.OtherEmail = emailAddress.Address;
          flag2 = true;
        }
      }
      List<ContactVCard.Address> addressList = new List<ContactVCard.Address>();
      int num2;
      flag1 = (num2 = 0) != 0;
      bool flag5 = num2 != 0;
      bool flag6 = num2 != 0;
      foreach (ContactVCard.Address address in vcard.Addresses)
      {
        if (address.Kind == AddressKind.Work && !flag6)
        {
          ContactVCardExtensions.AddAsWork(task, address);
          flag6 = true;
        }
        else if (address.Kind == AddressKind.Home && !flag5)
        {
          ContactVCardExtensions.AddAsHome(task, address);
          flag5 = true;
        }
        else
          addressList.Add(address);
      }
      foreach (ContactVCard.Address addr in addressList)
      {
        if (!flag6)
        {
          ContactVCardExtensions.AddAsWork(task, addr);
          flag6 = true;
        }
        else if (!flag5)
        {
          ContactVCardExtensions.AddAsHome(task, addr);
          flag5 = true;
        }
      }
      task.Website = vcard.Websites != null ? ((IEnumerable<string>) vcard.Websites).FirstOrDefault<string>() : (string) null;
      task.Notes = vcard.Note;
      task.JobTitle = vcard.JobTitle;
      task.Company = vcard.CompanyName;
      return task;
    }

    private static void AddAsWork(SaveContactTask task, ContactVCard.Address addr)
    {
      task.WorkAddressStreet = addr.Street;
      task.WorkAddressCity = addr.City;
      task.WorkAddressState = addr.State;
      task.WorkAddressZipCode = addr.PostalCode;
      task.WorkAddressCountry = addr.Country;
    }

    private static void AddAsHome(SaveContactTask task, ContactVCard.Address addr)
    {
      task.HomeAddressStreet = addr.Street;
      task.HomeAddressCity = addr.City;
      task.HomeAddressState = addr.State;
      task.HomeAddressZipCode = addr.PostalCode;
      task.HomeAddressCountry = addr.Country;
    }
  }
}
