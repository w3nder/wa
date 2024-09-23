// Decompiled with JetBrains decompiler
// Type: WhatsApp.AddressBookImpl.WinRtContacts
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WhatsApp.ContactClasses;
using Windows.ApplicationModel.Contacts;
using Windows.Storage.Streams;

#nullable disable
namespace WhatsApp.AddressBookImpl
{
  public class WinRtContacts : IAddressBook
  {
    private async Task<AddressBookSearchArgs> GetContactByIdAsync(string id)
    {
      return this.WrapContacts(await (await ContactManager.RequestStoreAsync()).GetContactAsync(id));
    }

    private async Task<AddressBookSearchArgs> GetContactByPhoneNumberAsync(string number)
    {
      ContactStore contactStore = await ContactManager.RequestStoreAsync();
      number = new string(number.Where<char>((Func<char, bool>) (c => char.IsDigit(c))).ToArray<char>());
      string str = number;
      return WinRtContacts.WrapContacts((IEnumerable<Contact>) await contactStore.FindContactsAsync(str));
    }

    public async Task<AddressBookSearchArgs> GetAllContactsAsync()
    {
      ContactStore contactStore = await ContactManager.RequestStoreAsync();
      if (contactStore == null)
      {
        Log.l("contacts", "ContactsManager returned null!");
        return WinRtContacts.ErrorResult();
      }
      IReadOnlyList<Contact> contactsAsync = await contactStore.FindContactsAsync();
      if (contactsAsync != null)
        return WinRtContacts.WrapContacts((IEnumerable<Contact>) contactsAsync);
      Log.l("contacts", "FindContactsAsync returned null!");
      return WinRtContacts.ErrorResult();
    }

    private static AddressBookSearchArgs ErrorResult()
    {
      return new AddressBookSearchArgs()
      {
        Results = (IEnumerable<Contact>) new Contact[0]
      };
    }

    private static AddressBookSearchArgs WrapContacts(IEnumerable<Contact> args)
    {
      return new AddressBookSearchArgs()
      {
        Results = ((IEnumerable<Contact>) ((object) args ?? (object) new Contact[0])).Where<Contact>((Func<Contact, bool>) (a => a != null)).Select<Contact, Contact>(new Func<Contact, Contact>(WinRtContacts.WrapContact))
      };
    }

    private AddressBookSearchArgs WrapContacts(params Contact[] args)
    {
      return WinRtContacts.WrapContacts(((IEnumerable<Contact>) args).AsEnumerable<Contact>());
    }

    private static Contact WrapContact(Contact contact)
    {
      return contact == null ? (Contact) null : (Contact) new WinRtContacts.ContactImplWinRt(contact);
    }

    public IObservable<AddressBookSearchArgs> GetContactById(string id)
    {
      return this.GetContactByIdAsync(id).ToObservable<AddressBookSearchArgs>();
    }

    public IObservable<AddressBookSearchArgs> GetContactByPhoneNumber(string number)
    {
      return this.GetContactByPhoneNumberAsync(number).ToObservable<AddressBookSearchArgs>();
    }

    public IObservable<AddressBookSearchArgs> GetAllContacts()
    {
      return this.GetAllContactsAsync().ToObservable<AddressBookSearchArgs>();
    }

    private class ContactImplWinRt : Contact
    {
      private Contact contact;

      public ContactImplWinRt(Contact contact) => this.contact = contact;

      public string GetIdentifier() => this.contact.Id;

      public IEnumerable<Account> Accounts
      {
        get
        {
          return this.contact.ConnectedServiceAccounts.SafeSelect<ContactConnectedServiceAccount, Account>(new Func<ContactConnectedServiceAccount, Account>(WinRtContacts.ContactImplWinRt.AccountImpl.Create)).SafeWhere<Account>((Func<Account, bool>) (a => a != null));
        }
      }

      public string DisplayName => this.contact.DisplayName;

      public CompleteName CompleteName
      {
        get => WinRtContacts.ContactImplWinRt.CompleteNameImpl.Create(this.contact);
      }

      public IEnumerable<DateTime> Birthdays
      {
        get
        {
          return this.contact.ImportantDates.SafeSelect<ContactDate, DateTime?>((Func<ContactDate, DateTime?>) (w => w == null || w.Kind != null ? new DateTime?() : this.ContactDateToDateTime(w))).SafeWhere<DateTime?>((Func<DateTime?, bool>) (w => w.HasValue)).SafeSelect<DateTime?, DateTime>((Func<DateTime?, DateTime>) (a => a.Value));
        }
      }

      public IEnumerable<ContactPhoneNumber> PhoneNumbers
      {
        get
        {
          return this.contact.Phones.SafeSelect<ContactPhone, ContactPhoneNumber>(new Func<ContactPhone, ContactPhoneNumber>(WinRtContacts.ContactImplWinRt.ContactPhoneNumberImpl.Create)).SafeWhere<ContactPhoneNumber>((Func<ContactPhoneNumber, bool>) (a => a != null));
        }
      }

      public IEnumerable<ContactEmailAddress> EmailAddresses
      {
        get
        {
          return this.contact.Emails.SafeSelect<ContactEmail, ContactEmailAddress>(new Func<ContactEmail, ContactEmailAddress>(WinRtContacts.ContactImplWinRt.ContactEmailAddressImpl.Create)).SafeWhere<ContactEmailAddress>((Func<ContactEmailAddress, bool>) (a => a != null));
        }
      }

      public IEnumerable<ContactAddress> Addresses
      {
        get
        {
          return this.contact.Addresses.SafeSelect<ContactAddress, ContactAddress>(new Func<ContactAddress, ContactAddress>(WinRtContacts.ContactImplWinRt.ContactAddressImpl.Create)).SafeWhere<ContactAddress>((Func<ContactAddress, bool>) (a => a != null));
        }
      }

      public IEnumerable<ContactCompanyInformation> Companies
      {
        get
        {
          return this.contact.JobInfo.SafeSelect<ContactJobInfo, ContactCompanyInformation>(new Func<ContactJobInfo, ContactCompanyInformation>(WinRtContacts.ContactImplWinRt.ContactCompanyImpl.Create)).SafeWhere<ContactCompanyInformation>((Func<ContactCompanyInformation, bool>) (a => a != null));
        }
      }

      public IEnumerable<string> Websites
      {
        get
        {
          return this.contact.Websites.SafeSelect<ContactWebsite, string>((Func<ContactWebsite, string>) (w => w?.Uri?.OriginalString)).SafeWhere<string>((Func<string, bool>) (w => w != null));
        }
      }

      public IEnumerable<string> Notes
      {
        get
        {
          if (this.contact.Notes == null)
            return (IEnumerable<string>) null;
          return (IEnumerable<string>) new string[1]
          {
            this.contact.Notes
          };
        }
      }

      public Stream GetPicture()
      {
        IRandomAccessStreamReference thumbnail = this.contact.Thumbnail;
        if (thumbnail == null)
          return (Stream) null;
        Stream r = (Stream) null;
        IObservable<IRandomAccessStreamWithContentType> source = thumbnail.OpenReadAsync().AsTask<IRandomAccessStreamWithContentType>().ToObservable<IRandomAccessStreamWithContentType>().SubscribeOn<IRandomAccessStreamWithContentType>((IScheduler) Scheduler.ThreadPool);
        Action @throw = (Action) null;
        Action<IRandomAccessStreamWithContentType> onNext = (Action<IRandomAccessStreamWithContentType>) (s => r = ((IRandomAccessStream) s).AsStream());
        Action<Exception> onError = (Action<Exception>) (ex => @throw = ex.GetRethrowAction());
        source.Run<IRandomAccessStreamWithContentType>(onNext, onError);
        if (@throw != null)
        {
          r.SafeDispose();
          r = (Stream) null;
          @throw();
        }
        return r;
      }

      private DateTime? ContactDateToDateTime(ContactDate c)
      {
        try
        {
          DateTime dateTime = new DateTime();
          if (c.Year.HasValue)
            dateTime = dateTime.AddYears(c.Year.Value - 1);
          uint? nullable;
          if (c.Day.HasValue)
          {
            ref DateTime local = ref dateTime;
            nullable = c.Day;
            double num = (double) ((int) nullable.Value - 1);
            dateTime = local.AddDays(num);
          }
          nullable = c.Month;
          if (nullable.HasValue)
          {
            ref DateTime local = ref dateTime;
            nullable = c.Month;
            int months = (int) nullable.Value - 1;
            dateTime = local.AddMonths(months);
          }
          return new DateTime?(dateTime);
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "Failed parsing contact date");
        }
        return new DateTime?();
      }

      private class ContactCompanyImpl : ContactCompanyInformation
      {
        public static ContactCompanyInformation Create(ContactJobInfo a)
        {
          if (a == null)
            return (ContactCompanyInformation) null;
          return new ContactCompanyInformation()
          {
            CompanyName = a.CompanyName,
            JobTitle = a.Title,
            OfficeLocation = a.Office,
            YomiCompanyName = a.CompanyYomiName,
            Accounts = (IEnumerable<Account>) new Account[0]
          };
        }
      }

      private class AccountImpl : Account
      {
        public static Account Create(ContactConnectedServiceAccount a)
        {
          if (a == null)
            return (Account) null;
          return new Account() { Name = a.ServiceName };
        }
      }

      private class ContactAddressImpl : ContactEmailAddress
      {
        public static ContactAddress Create(ContactAddress a)
        {
          if (a == null)
            return (ContactAddress) null;
          return new ContactAddress()
          {
            PhysicalAddress = new CivicAddress()
            {
              AddressLine1 = a.StreetAddress,
              City = a.Locality,
              CountryRegion = a.Country,
              PostalCode = a.PostalCode,
              StateProvince = a.Region
            },
            Kind = WinRtContacts.ContactImplWinRt.ContactAddressImpl.ConvertAddressKind(a.Kind),
            Accounts = (IEnumerable<Account>) new Account[0]
          };
        }

        private static AddressKind ConvertAddressKind(ContactAddressKind a)
        {
          switch ((int) a)
          {
            case 0:
              return AddressKind.Home;
            case 1:
              return AddressKind.Work;
            default:
              return AddressKind.Other;
          }
        }
      }

      private class ContactEmailAddressImpl : ContactEmailAddress
      {
        public static ContactEmailAddress Create(ContactEmail a)
        {
          if (a == null)
            return (ContactEmailAddress) null;
          return new ContactEmailAddress()
          {
            EmailAddress = a.Address,
            Kind = WinRtContacts.ContactImplWinRt.ContactEmailAddressImpl.ConvertEmailKind(a.Kind),
            Accounts = (IEnumerable<Account>) new Account[0]
          };
        }

        private static EmailAddressKind ConvertEmailKind(ContactEmailKind a)
        {
          switch ((int) a)
          {
            case 0:
              return EmailAddressKind.Personal;
            case 1:
              return EmailAddressKind.Work;
            default:
              return EmailAddressKind.Other;
          }
        }
      }

      private class CompleteNameImpl : CompleteName
      {
        public static CompleteName Create(Contact a)
        {
          return new CompleteName()
          {
            FirstName = a.FirstName,
            LastName = a.LastName,
            MiddleName = a.MiddleName,
            YomiFirstName = a.YomiGivenName,
            YomiLastName = a.YomiFamilyName
          };
        }
      }

      private class ContactPhoneNumberImpl : ContactPhoneNumber
      {
        public static ContactPhoneNumber Create(ContactPhone a)
        {
          if (a == null)
            return (ContactPhoneNumber) null;
          return new ContactPhoneNumber()
          {
            Accounts = (IEnumerable<Account>) new Account[0],
            Kind = WinRtContacts.ContactImplWinRt.ContactPhoneNumberImpl.ConvertPhoneNumberKind(a.Kind),
            PhoneNumber = a.Number
          };
        }

        private static PhoneNumberKind ConvertPhoneNumberKind(ContactPhoneKind a)
        {
          switch ((int) a)
          {
            case 0:
              return PhoneNumberKind.Home;
            case 1:
              return PhoneNumberKind.Mobile;
            case 2:
              return PhoneNumberKind.Work;
            default:
              return PhoneNumberKind.Other;
          }
        }
      }
    }
  }
}
