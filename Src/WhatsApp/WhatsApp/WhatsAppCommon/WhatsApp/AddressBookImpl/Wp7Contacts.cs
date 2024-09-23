// Decompiled with JetBrains decompiler
// Type: WhatsApp.AddressBookImpl.Wp7Contacts
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using Microsoft.Phone.UserData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;


namespace WhatsApp.AddressBookImpl
{
  public class Wp7Contacts : IAddressBook
  {
    public IObservable<AddressBookSearchArgs> GetContactById(string id)
    {
      return Wp7Contacts.GetSearchAsync(id, FilterKind.Identifier).Select<ContactsSearchEventArgs, AddressBookSearchArgs>(new Func<ContactsSearchEventArgs, AddressBookSearchArgs>(Wp7Contacts.ConvertSearchArgs));
    }

    public IObservable<AddressBookSearchArgs> GetContactByPhoneNumber(string number)
    {
      return Wp7Contacts.GetSearchAsync(number, FilterKind.PhoneNumber).Select<ContactsSearchEventArgs, AddressBookSearchArgs>(new Func<ContactsSearchEventArgs, AddressBookSearchArgs>(Wp7Contacts.ConvertSearchArgs));
    }

    public IObservable<AddressBookSearchArgs> GetAllContacts()
    {
      return Wp7Contacts.GetSearchAsync().Select<ContactsSearchEventArgs, AddressBookSearchArgs>(new Func<ContactsSearchEventArgs, AddressBookSearchArgs>(Wp7Contacts.ConvertSearchArgs));
    }

    public async Task<AddressBookSearchArgs> GetAllContactsAsync()
    {
      IObservable<AddressBookSearchArgs> allContacts = this.GetAllContacts();
      TaskCompletionSource<AddressBookSearchArgs> tcs = new TaskCompletionSource<AddressBookSearchArgs>();
      Action<AddressBookSearchArgs> onNext = (Action<AddressBookSearchArgs>) (result => tcs.SetResult(result));
      Action<Exception> onError = (Action<Exception>) (ex => tcs.SetException(ex));
      Action onCompleted = (Action) (() =>
      {
        if (!tcs.TrySetCanceled())
          return;
        Log.l("test", "Needed to set cancelled for GetSelectedRouteAsync");
      });
      allContacts.Subscribe<AddressBookSearchArgs>(onNext, onError, onCompleted);
      try
      {
        AddressBookSearchArgs task = await tcs.Task;
      }
      catch (Exception ex)
      {
        throw new InvalidDataException("GetAllContactsAsync faulted", ex);
      }
      return tcs.Task.Result;
    }

    private static AddressBookSearchArgs ConvertSearchArgs(ContactsSearchEventArgs args)
    {
      return new AddressBookSearchArgs()
      {
        Results = args.Results.SafeSelect<Microsoft.Phone.UserData.Contact, WhatsApp.Contact>(new Func<Microsoft.Phone.UserData.Contact, WhatsApp.Contact>(Wp7Contacts.ConvertContact))
      };
    }

    private static WhatsApp.Contact ConvertContact(Microsoft.Phone.UserData.Contact contact)
    {
      return (WhatsApp.Contact) new Wp7Contacts.ContactImplWp7(contact);
    }

    private static IObservable<ContactsSearchEventArgs> GetSearchAsync(
      string filter = null,
      FilterKind filterKind = FilterKind.None)
    {
      return Observable.Create<ContactsSearchEventArgs>((Func<IObserver<ContactsSearchEventArgs>, Action>) (observer =>
      {
        Contacts contacts = new Contacts();
        EventHandler<ContactsSearchEventArgs> eh = (EventHandler<ContactsSearchEventArgs>) ((sender, args) => observer.OnNext(args));
        contacts.SearchCompleted += eh;
        IDisposable exnSubscription = AppState.ClientInstance.GetUnhandledExceptionSubject().Where<ApplicationUnhandledExceptionEventArgs>((Func<ApplicationUnhandledExceptionEventArgs, bool>) (exnArgs =>
        {
          string stackTrace = exnArgs.ExceptionObject.StackTrace;
          return stackTrace != null && stackTrace.Contains("Microsoft.Phone.UserData.Contacts.GetContacts(Object state)");
        })).Subscribe<ApplicationUnhandledExceptionEventArgs>((Action<ApplicationUnhandledExceptionEventArgs>) (exnArgs =>
        {
          exnArgs.Handled = true;
          observer.OnError(exnArgs.ExceptionObject);
        }));
        contacts.SearchAsync(filter, filterKind, (object) null);
        return (Action) (() =>
        {
          contacts.SearchCompleted -= eh;
          exnSubscription.Dispose();
        });
      })).Do<ContactsSearchEventArgs>((Action<ContactsSearchEventArgs>) (_ => { }), (Action<Exception>) (ex =>
      {
        string summary = string.Format("{0},{1}", (object) ex.GetHResult().ToString("x").PadLeft(8, '0'), (object) DateTime.Now.ToUnixTime().ToString());
        WAThreadPool.QueueUserWorkItem((Action) (() => Settings.LastContactException = summary));
      }));
    }

    private class ContactImplWp7 : WhatsApp.Contact
    {
      private Microsoft.Phone.UserData.Contact contact;

      public ContactImplWp7(Microsoft.Phone.UserData.Contact contact) => this.contact = contact;

      public string GetIdentifier() => this.contact.GetHashCode().ToString();

      public IEnumerable<WhatsApp.ContactClasses.Account> Accounts
      {
        get
        {
          return this.contact.Accounts.SafeSelect<Microsoft.Phone.UserData.Account, WhatsApp.ContactClasses.Account>(new Func<Microsoft.Phone.UserData.Account, WhatsApp.ContactClasses.Account>(Wp7Contacts.ContactImplWp7.AccountImpl.Create));
        }
      }

      public string DisplayName => this.contact.DisplayName;

      public WhatsApp.ContactClasses.CompleteName CompleteName
      {
        get
        {
          return this.contact.CompleteName.WrapIfNonNull<Microsoft.Phone.UserData.CompleteName, WhatsApp.ContactClasses.CompleteName>(new Func<Microsoft.Phone.UserData.CompleteName, WhatsApp.ContactClasses.CompleteName>(Wp7Contacts.ContactImplWp7.CompleteNameImpl.Create));
        }
      }

      public IEnumerable<DateTime> Birthdays => this.contact.Birthdays;

      public IEnumerable<WhatsApp.ContactClasses.ContactPhoneNumber> PhoneNumbers
      {
        get
        {
          return this.contact.PhoneNumbers.SafeSelect<Microsoft.Phone.UserData.ContactPhoneNumber, WhatsApp.ContactClasses.ContactPhoneNumber>(new Func<Microsoft.Phone.UserData.ContactPhoneNumber, WhatsApp.ContactClasses.ContactPhoneNumber>(Wp7Contacts.ContactImplWp7.ContactPhoneNumberImpl.Create));
        }
      }

      public IEnumerable<WhatsApp.ContactClasses.ContactEmailAddress> EmailAddresses
      {
        get
        {
          return this.contact.EmailAddresses.SafeSelect<Microsoft.Phone.UserData.ContactEmailAddress, WhatsApp.ContactClasses.ContactEmailAddress>(new Func<Microsoft.Phone.UserData.ContactEmailAddress, WhatsApp.ContactClasses.ContactEmailAddress>(Wp7Contacts.ContactImplWp7.ContactEmailAddressImpl.Create));
        }
      }

      public IEnumerable<WhatsApp.ContactClasses.ContactAddress> Addresses
      {
        get
        {
          return this.contact.Addresses.SafeSelect<Microsoft.Phone.UserData.ContactAddress, WhatsApp.ContactClasses.ContactAddress>(new Func<Microsoft.Phone.UserData.ContactAddress, WhatsApp.ContactClasses.ContactAddress>(Wp7Contacts.ContactImplWp7.ContactAddressImpl.Create));
        }
      }

      public IEnumerable<WhatsApp.ContactClasses.ContactCompanyInformation> Companies
      {
        get
        {
          return this.contact.Companies.SafeSelect<Microsoft.Phone.UserData.ContactCompanyInformation, WhatsApp.ContactClasses.ContactCompanyInformation>(new Func<Microsoft.Phone.UserData.ContactCompanyInformation, WhatsApp.ContactClasses.ContactCompanyInformation>(Wp7Contacts.ContactImplWp7.ContactCompanyInformationImpl.Create));
        }
      }

      public IEnumerable<string> Websites => this.contact.Websites;

      public IEnumerable<string> Notes => this.contact.Notes;

      public Stream GetPicture() => this.contact.GetPicture();

      private class AccountImpl : WhatsApp.ContactClasses.Account
      {
        public static WhatsApp.ContactClasses.Account Create(Microsoft.Phone.UserData.Account a)
        {
          return new WhatsApp.ContactClasses.Account()
          {
            Kind = (WhatsApp.StorageKind) a.Kind,
            Name = a.Name
          };
        }
      }

      private class ContactAddressImpl : WhatsApp.ContactClasses.ContactAddress
      {
        public static WhatsApp.ContactClasses.ContactAddress Create(Microsoft.Phone.UserData.ContactAddress a)
        {
          return new WhatsApp.ContactClasses.ContactAddress()
          {
            Accounts = a.Accounts.SafeSelect<Microsoft.Phone.UserData.Account, WhatsApp.ContactClasses.Account>(new Func<Microsoft.Phone.UserData.Account, WhatsApp.ContactClasses.Account>(Wp7Contacts.ContactImplWp7.AccountImpl.Create)),
            Kind = (WhatsApp.AddressKind) a.Kind,
            PhysicalAddress = a.PhysicalAddress
          };
        }
      }

      private class ContactEmailAddressImpl : WhatsApp.ContactClasses.ContactEmailAddress
      {
        public static WhatsApp.ContactClasses.ContactEmailAddress Create(Microsoft.Phone.UserData.ContactEmailAddress a)
        {
          return new WhatsApp.ContactClasses.ContactEmailAddress()
          {
            Accounts = a.Accounts.SafeSelect<Microsoft.Phone.UserData.Account, WhatsApp.ContactClasses.Account>(new Func<Microsoft.Phone.UserData.Account, WhatsApp.ContactClasses.Account>(Wp7Contacts.ContactImplWp7.AccountImpl.Create)),
            EmailAddress = a.EmailAddress,
            Kind = (WhatsApp.EmailAddressKind) a.Kind
          };
        }
      }

      private class ContactPhoneNumberImpl : WhatsApp.ContactClasses.ContactPhoneNumber
      {
        public static WhatsApp.ContactClasses.ContactPhoneNumber Create(Microsoft.Phone.UserData.ContactPhoneNumber a)
        {
          return new WhatsApp.ContactClasses.ContactPhoneNumber()
          {
            Accounts = a.Accounts.SafeSelect<Microsoft.Phone.UserData.Account, WhatsApp.ContactClasses.Account>(new Func<Microsoft.Phone.UserData.Account, WhatsApp.ContactClasses.Account>(Wp7Contacts.ContactImplWp7.AccountImpl.Create)),
            Kind = (WhatsApp.PhoneNumberKind) a.Kind,
            PhoneNumber = a.PhoneNumber
          };
        }
      }

      private class ContactCompanyInformationImpl : WhatsApp.ContactClasses.ContactCompanyInformation
      {
        public static WhatsApp.ContactClasses.ContactCompanyInformation Create(
          Microsoft.Phone.UserData.ContactCompanyInformation a)
        {
          return new WhatsApp.ContactClasses.ContactCompanyInformation()
          {
            Accounts = a.Accounts.SafeSelect<Microsoft.Phone.UserData.Account, WhatsApp.ContactClasses.Account>(new Func<Microsoft.Phone.UserData.Account, WhatsApp.ContactClasses.Account>(Wp7Contacts.ContactImplWp7.AccountImpl.Create)),
            CompanyName = a.CompanyName,
            JobTitle = a.JobTitle,
            OfficeLocation = a.OfficeLocation,
            YomiCompanyName = a.YomiCompanyName
          };
        }
      }

      private class CompleteNameImpl : WhatsApp.ContactClasses.CompleteName
      {
        public static WhatsApp.ContactClasses.CompleteName Create(Microsoft.Phone.UserData.CompleteName a)
        {
          return new WhatsApp.ContactClasses.CompleteName()
          {
            FirstName = a.FirstName,
            LastName = a.LastName,
            MiddleName = a.MiddleName,
            Nickname = a.Nickname,
            Suffix = a.Suffix,
            Title = a.Title,
            YomiFirstName = a.YomiFirstName,
            YomiLastName = a.YomiLastName
          };
        }
      }
    }
  }
}
