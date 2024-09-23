// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactInfoPageData
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Windows;
using WhatsApp.ContactClasses;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class ContactInfoPageData : WaDisposable
  {
    private const string LogHeader = "contact info";
    private UserStatus[] waAccountsFromContact;
    private ContactInfoPageData.ContactInfoItem[] nonWaNumbersFromContact;
    private ReplaySubject<Unit> chatDataLoadedSubject = new ReplaySubject<Unit>();
    private Subject<Unit> bizDataUpdatedSubject = new Subject<Unit>();
    private Subject<Unit> jidInfosUpdatedSubj = new Subject<Unit>();
    private IDisposable jidInfoTableUpdateSub;

    public Contact Contact { get; private set; }

    public Dictionary<string, JidInfo> JidInfos { get; private set; }

    public UserStatus TargetWaAccount { get; private set; }

    public bool IsChatDataLoaded { get; private set; }

    public System.Windows.Media.ImageSource LargeFormatPicSource { get; set; }

    public string PicJid { get; set; }

    public bool HasWaAccount
    {
      get
      {
        if (this.TargetWaAccount != null)
          return true;
        return this.waAccountsFromContact != null && ((IEnumerable<UserStatus>) this.waAccountsFromContact).Any<UserStatus>();
      }
    }

    public bool IsAllDataFromFacebook
    {
      get
      {
        return this.TargetWaAccount == null && this.Contact != null && this.Contact.Accounts != null && this.Contact.Accounts.Any<Account>() && this.Contact.Accounts.All<Account>((Func<Account, bool>) (acct => acct.Kind == StorageKind.Facebook));
      }
    }

    protected override void DisposeManagedResources()
    {
      this.jidInfoTableUpdateSub.SafeDispose();
      this.jidInfoTableUpdateSub = (IDisposable) null;
      base.DisposeManagedResources();
    }

    public IObservable<Unit> GetBizDataUpdatedObservable()
    {
      return (IObservable<Unit>) this.bizDataUpdatedSubject;
    }

    public IObservable<Unit> GetChatDataLoadedObservable()
    {
      return !this.IsChatDataLoaded ? (IObservable<Unit>) this.chatDataLoadedSubject : Observable.Return<Unit>(new Unit());
    }

    public IObservable<Unit> GetJidInfosUpdatedObservable()
    {
      return (IObservable<Unit>) this.jidInfosUpdatedSubj;
    }

    public UserStatus[] GetKnownWaAccounts()
    {
      Dictionary<string, UserStatus> dictionary = new Dictionary<string, UserStatus>();
      if (this.TargetWaAccount != null)
        dictionary[this.TargetWaAccount.Jid] = this.TargetWaAccount;
      if (this.waAccountsFromContact != null)
      {
        foreach (UserStatus userStatus in this.waAccountsFromContact)
          dictionary[userStatus.Jid] = userStatus;
      }
      return dictionary.Values.ToArray<UserStatus>();
    }

    public UserStatus GetPrimaryWaAccount()
    {
      return this.TargetWaAccount ?? ((IEnumerable<UserStatus>) this.GetKnownWaAccounts()).FirstOrDefault<UserStatus>();
    }

    private void Reset()
    {
      this.Contact = (Contact) null;
      this.waAccountsFromContact = (UserStatus[]) null;
      this.nonWaNumbersFromContact = (ContactInfoPageData.ContactInfoItem[]) null;
      this.TargetWaAccount = (UserStatus) null;
    }

    public void Load(Contact c)
    {
      this.Reset();
      if ((this.Contact = c) == null)
        return;
      WAThreadPool.QueueUserWorkItem((Action) (() =>
      {
        UserStatus[] waAccounts = (UserStatus[]) null;
        ContactInfoPageData.ContactInfoItem[] nonWaNums = (ContactInfoPageData.ContactInfoItem[]) null;
        this.ProcessContactPhoneNumbers(this.Contact, out waAccounts, out nonWaNums);
        Dictionary<string, JidInfo> jiDict = this.LoadJidInfos((string) null, ((IEnumerable<UserStatus>) waAccounts).Select<UserStatus, string>((Func<UserStatus, string>) (u => u.Jid)).ToArray<string>());
        Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.waAccountsFromContact = waAccounts;
          this.nonWaNumbersFromContact = nonWaNums;
          this.JidInfos = jiDict;
          this.IsChatDataLoaded = true;
          this.chatDataLoadedSubject.OnNext(new Unit());
          this.chatDataLoadedSubject.OnCompleted();
        }));
      }));
    }

    public void Load(UserStatus user)
    {
      this.Reset();
      if (user == null)
        return;
      this.TargetWaAccount = user;
      if (user.IsVerified())
      {
        Log.l("contact info", "biz verification details {0} {1} {2} {3}", (object) user.VerifiedName, (object) user.VerifiedLevel, (object) (user.VerifiedNameCertificateDetails != null), (object) user.VerifiedNameMatchesContactName());
        FieldStats.ReportBizProfileAction(user.Jid, wam_enum_view_business_profile_action.ACTION_IMPRESSION);
        bool forceFetch = false;
        string tag = forceFetch ? (string) null : user.InternalProperties?.BusinessUserPropertiesField?.Tag;
        Action<Dictionary<string, BizProfileDetails>> onCompleted = (Action<Dictionary<string, BizProfileDetails>>) (res =>
        {
          BizProfileDetails bizProfile = (BizProfileDetails) null;
          if (res == null || !res.TryGetValue(user.Jid, out bizProfile) || bizProfile == null)
            Log.l("contact info", "no profile for {0}", (object) user.Jid);
          if (bizProfile?.Tag == tag)
          {
            Log.d("contact info", "biz user already up to date {0} {1}", (object) user.Jid, (object) (bizProfile?.Tag ?? "null"));
            if (!forceFetch)
              return;
          }
          Log.l("contact info", "biz user profile being updated {0} {1}", (object) user.Jid, (object) (bizProfile?.Tag ?? "null"));
          ContactsContext.Instance((Action<ContactsContext>) (cdb =>
          {
            UserStatusProperties forUserStatus = UserStatusProperties.GetForUserStatus(user);
            if (!forUserStatus.UpdateBusinessUserProperties(bizProfile))
              return;
            forUserStatus.Save();
            cdb.SubmitChanges();
          }));
          this.bizDataUpdatedSubject.OnNext(new Unit());
        });
        Action<int> onError = (Action<int>) (err => Log.l("contact info", "couldn't refresh profile for {0} | {1}", (object) user.Jid, (object) err));
        FunXMPP.Connection connection = AppState.GetConnection();
        if (connection != null && connection.IsConnected)
          connection.SendGetBusinessProfile(user.Jid, tag, onCompleted, onError);
      }
      if (user.IsInDeviceContactList)
      {
        AddressBook.Instance.GetContactByPhoneNumber(JidHelper.GetPhoneNumber(user.Jid, false)).SubscribeOn<AddressBookSearchArgs>(WAThreadPool.Scheduler).Select<AddressBookSearchArgs, object[]>((Func<AddressBookSearchArgs, object[]>) (args =>
        {
          Contact contact = args == null || args.Results == null ? (Contact) null : args.Results.FirstOrDefault<Contact>();
          UserStatus[] waAccounts = (UserStatus[]) null;
          ContactInfoPageData.ContactInfoItem[] nonWaNumItems = (ContactInfoPageData.ContactInfoItem[]) null;
          if (contact != null)
            this.ProcessContactPhoneNumbers(contact, out waAccounts, out nonWaNumItems);
          Dictionary<string, JidInfo> dictionary = this.LoadJidInfos(user.Jid, waAccounts == null ? (string[]) null : ((IEnumerable<UserStatus>) waAccounts).Select<UserStatus, string>((Func<UserStatus, string>) (u => u.Jid)).ToArray<string>());
          return new object[4]
          {
            (object) contact,
            (object) waAccounts,
            (object) nonWaNumItems,
            (object) dictionary
          };
        })).ObserveOnDispatcher<object[]>().Subscribe<object[]>((Action<object[]>) (arr =>
        {
          this.Contact = arr[0] as Contact;
          this.waAccountsFromContact = arr[1] as UserStatus[];
          this.nonWaNumbersFromContact = arr[2] as ContactInfoPageData.ContactInfoItem[];
          this.JidInfos = arr[3] as Dictionary<string, JidInfo>;
          this.IsChatDataLoaded = true;
          this.chatDataLoadedSubject.OnNext(new Unit());
          this.chatDataLoadedSubject.OnCompleted();
        }));
      }
      else
      {
        this.JidInfos = this.LoadJidInfos(user.Jid, (string[]) null);
        this.IsChatDataLoaded = true;
        this.chatDataLoadedSubject.OnNext(new Unit());
        this.chatDataLoadedSubject.OnCompleted();
      }
    }

    private void ProcessContactPhoneNumbers(
      Contact contact,
      out UserStatus[] waAccounts,
      out ContactInfoPageData.ContactInfoItem[] nonWaNumItems)
    {
      if (contact == null)
      {
        waAccounts = new UserStatus[0];
        nonWaNumItems = new ContactInfoPageData.ContactInfoItem[0];
      }
      else
      {
        List<UserStatus> waList = new List<UserStatus>();
        List<ContactInfoPageData.ContactInfoItem> nonWaList = new List<ContactInfoPageData.ContactInfoItem>();
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          foreach (ContactPhoneNumber phoneNumber1 in contact.PhoneNumbers)
          {
            PhoneNumber phoneNumber2 = db.PhoneNumberForRawNumber(phoneNumber1.PhoneNumber);
            UserStatus userStatus;
            if (phoneNumber2 != null && phoneNumber2.Jid != null && (userStatus = db.GetUserStatus(phoneNumber2.Jid, false)) != null)
            {
              waList.Add(userStatus);
            }
            else
            {
              string str1;
              switch (phoneNumber1.Kind)
              {
                case PhoneNumberKind.Mobile:
                  str1 = AppResources.NumberKindMobile;
                  break;
                case PhoneNumberKind.Home:
                  str1 = AppResources.NumberKindHome;
                  break;
                case PhoneNumberKind.Work:
                  str1 = AppResources.NumberKindWork;
                  break;
                case PhoneNumberKind.Company:
                  str1 = AppResources.NumberKindCompany;
                  break;
                case PhoneNumberKind.Pager:
                  str1 = AppResources.NumberKindPager;
                  break;
                case PhoneNumberKind.HomeFax:
                  str1 = AppResources.NumberKindHomeFax;
                  break;
                case PhoneNumberKind.WorkFax:
                  str1 = AppResources.NumberKindWorkFax;
                  break;
                default:
                  str1 = AppResources.NumberKindOther;
                  break;
              }
              string str2 = phoneNumber2 == null || phoneNumber2.Jid == null ? phoneNumber1.PhoneNumber : JidHelper.GetPhoneNumber(phoneNumber2.Jid, true);
              nonWaList.Add(new ContactInfoPageData.ContactInfoItem()
              {
                Title = str1,
                Data = str2
              });
            }
          }
        }));
        waAccounts = waList.ToArray();
        nonWaNumItems = nonWaList.ToArray();
      }
    }

    private Dictionary<string, JidInfo> LoadJidInfos(string targetJid, string[] otherJids)
    {
      Set<string> jids = new Set<string>((IEnumerable<string>) (otherJids ?? new string[0]));
      if (!string.IsNullOrEmpty(targetJid))
        jids.Add(targetJid);
      Dictionary<string, JidInfo> d = new Dictionary<string, JidInfo>();
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        foreach (string str in jids)
          d[str] = db.GetJidInfo(str, CreateOptions.None);
      }));
      if (this.jidInfoTableUpdateSub == null)
        this.jidInfoTableUpdateSub = MessagesContext.Events.JidInfoUpdateSubject.SubscribeOn<DbDataUpdate>((IScheduler) AppState.Worker).Where<DbDataUpdate>((Func<DbDataUpdate, bool>) (u => u.UpdatedObj is JidInfo updatedObj && jids.Contains(updatedObj.Jid))).ObserveOnDispatcher<DbDataUpdate>().Subscribe<DbDataUpdate>((Action<DbDataUpdate>) (_ => this.OnJidInfoTableUpdated()));
      return d;
    }

    private void OnJidInfoTableUpdated()
    {
      string targetJid = this.TargetWaAccount == null ? (string) null : this.TargetWaAccount.Jid;
      string[] otherJids = this.waAccountsFromContact == null ? (string[]) null : ((IEnumerable<UserStatus>) this.waAccountsFromContact).Select<UserStatus, string>((Func<UserStatus, string>) (u => u.Jid)).ToArray<string>();
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => this.JidInfos = this.LoadJidInfos(targetJid, otherJids)));
      this.jidInfosUpdatedSubj.OnNext(new Unit());
    }

    public string GetDisplayName()
    {
      if (this.TargetWaAccount != null)
        return this.TargetWaAccount.GetDisplayName();
      return this.Contact != null ? this.Contact.DisplayName : (string) null;
    }

    public IEnumerable<ContactInfoPageData.ContactInfoItem> GetActionableItems()
    {
      return ((IEnumerable<ContactInfoPageData.ContactInfoItem>) (this.nonWaNumbersFromContact ?? new ContactInfoPageData.ContactInfoItem[0])).Concat<ContactInfoPageData.ContactInfoItem>(this.GetNonNumberActionableItems());
    }

    public IEnumerable<ContactInfoPageData.ContactInfoItem> GetInfoItems()
    {
      if (this.Contact == null)
        return (IEnumerable<ContactInfoPageData.ContactInfoItem>) new ContactInfoPageData.ContactInfoItem[0];
      List<ContactInfoPageData.ContactInfoItem> list = this.Contact.Birthdays.Select<DateTime, ContactInfoPageData.ContactInfoItem>((Func<DateTime, ContactInfoPageData.ContactInfoItem>) (birthdayItem => new ContactInfoPageData.ContactInfoItem()
      {
        Title = AppResources.Birthday,
        Data = birthdayItem.ToShortDateString()
      })).ToList<ContactInfoPageData.ContactInfoItem>();
      string str = string.Join("\n", this.Contact.Notes);
      if (!string.IsNullOrEmpty(str))
        list.Add(new ContactInfoPageData.ContactInfoItem()
        {
          Title = AppResources.Note,
          Data = str
        });
      return (IEnumerable<ContactInfoPageData.ContactInfoItem>) list;
    }

    private IEnumerable<ContactInfoPageData.ContactInfoItem> GetNonNumberActionableItems()
    {
      if (this.Contact == null)
        return (IEnumerable<ContactInfoPageData.ContactInfoItem>) new ContactInfoPageData.ContactInfoItem[0];
      Contact contact = this.Contact;
      return contact.EmailAddresses.Select<ContactEmailAddress, ContactInfoPageData.ContactInfoItem>((Func<ContactEmailAddress, ContactInfoPageData.ContactInfoItem>) (email => new ContactInfoPageData.ContactInfoItem()
      {
        Title = AppResources.SendEmail,
        Data = email.EmailAddress,
        ActionOnData = (Action<string>) (emailAddr => new EmailComposeTask()
        {
          To = emailAddr
        }.Show())
      })).Concat<ContactInfoPageData.ContactInfoItem>(contact.Addresses.Select<ContactAddress, ContactInfoPageData.ContactInfoItem>((Func<ContactAddress, ContactInfoPageData.ContactInfoItem>) (address =>
      {
        string str1 = (string) null;
        switch (address.Kind)
        {
          case AddressKind.Home:
            str1 = AppResources.MapHomeAddress;
            break;
          case AddressKind.Work:
            str1 = AppResources.MapWorkAddress;
            break;
          case AddressKind.Other:
            str1 = AppResources.MapOtherAddress;
            break;
        }
        CivicAddress physicalAddress = address.PhysicalAddress;
        string str2 = string.Format("{0} {1}\n{2} {3} {4}\n{5}", (object) physicalAddress.AddressLine1, (object) physicalAddress.AddressLine2, (object) physicalAddress.City, (object) physicalAddress.StateProvince, (object) physicalAddress.PostalCode, (object) physicalAddress.CountryRegion);
        return new ContactInfoPageData.ContactInfoItem()
        {
          Title = str1,
          Data = str2,
          ActionOnData = (Action<string>) (addrStr => new MapsTask()
          {
            SearchTerm = addrStr
          }.Show())
        };
      }))).Concat<ContactInfoPageData.ContactInfoItem>(contact.Websites.Select<string, ContactInfoPageData.ContactInfoItem>((Func<string, ContactInfoPageData.ContactInfoItem>) (uri => new ContactInfoPageData.ContactInfoItem()
      {
        Title = AppResources.ViewWebsite,
        Data = uri,
        ActionOnData = (Action<string>) (uriStr => new WebBrowserTask()
        {
          Uri = new UriBuilder(uriStr).Uri
        }.Show())
      })));
    }

    public ContactInfoPageData.ContactInfoItem[] GetInvitationItems()
    {
      return this.HasWaAccount || this.Contact == null ? new ContactInfoPageData.ContactInfoItem[0] : this.Contact.PhoneNumbers.Select<ContactPhoneNumber, ContactInfoPageData.ContactInfoItem>((Func<ContactPhoneNumber, ContactInfoPageData.ContactInfoItem>) (num => new ContactInfoPageData.ContactInfoItem()
      {
        Data = num.PhoneNumber,
        ActionOnData = (Action<string>) (phoneNum => new SmsComposeTask()
        {
          To = phoneNum,
          Body = string.Format(AppResources.TellFriendBodyShort, (object) "https://whatsapp.com/dl/")
        }.Show())
      })).Concat<ContactInfoPageData.ContactInfoItem>(this.Contact.EmailAddresses.Select<ContactEmailAddress, ContactInfoPageData.ContactInfoItem>((Func<ContactEmailAddress, ContactInfoPageData.ContactInfoItem>) (email => new ContactInfoPageData.ContactInfoItem()
      {
        Data = email.EmailAddress,
        ActionOnData = (Action<string>) (emailAddr => new EmailComposeTask()
        {
          To = emailAddr,
          Subject = AppResources.TellFriendSubject,
          Body = string.Format(AppResources.TellFriendBodyLong, (object) "https://www.whatsapp.com/download/")
        }.Show())
      }))).ToArray<ContactInfoPageData.ContactInfoItem>();
    }

    public ContactVCard GetBizContactVCard()
    {
      ContactVCard bizContactVcard = (ContactVCard) null;
      UserStatus user = this.TargetWaAccount;
      if (user != null && user.IsVerified() && user.VerifiedLevel == VerifiedLevel.high)
      {
        ContactVCard contactVcard = new ContactVCard();
        contactVcard.CompanyName = user.GetVerifiedNameForDisplay();
        contactVcard.PhoneNumbers = new ContactVCard.PhoneNumber[1]
        {
          new ContactVCard.PhoneNumber()
          {
            Kind = PhoneNumberKind.Mobile,
            Jid = user.Jid,
            Number = JidHelper.GetPhoneNumber(user.Jid, true)
          }
        };
        bizContactVcard = contactVcard;
        string photoId = (string) null;
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          ChatPicture chatPictureState = db.GetChatPictureState(user.Jid, CreateOptions.None);
          if (chatPictureState == null)
            return;
          photoId = chatPictureState.WaPhotoId;
        }));
        if (photoId != null)
        {
          Stream storedPictureStream = ChatPictureStore.GetStoredPictureStream(user.Jid, photoId, false);
          bizContactVcard.SetPhoto(storedPictureStream);
        }
        UserStatusProperties.BusinessUserProperties userPropertiesField = user.InternalProperties?.BusinessUserPropertiesField;
        if (userPropertiesField != null && !string.IsNullOrEmpty(userPropertiesField.Email))
          bizContactVcard.EmailAddresses = new ContactVCard.EmailAddress[1]
          {
            new ContactVCard.EmailAddress()
            {
              Address = userPropertiesField.Email
            }
          };
      }
      return bizContactVcard;
    }

    public class ContactInfoItem
    {
      public string Title { get; set; }

      public string Data { get; set; }

      public Action<string> ActionOnData { private get; set; }

      public void Act()
      {
        if (this.ActionOnData == null || this.Data == null)
          return;
        this.ActionOnData(this.Data);
      }
    }
  }
}
