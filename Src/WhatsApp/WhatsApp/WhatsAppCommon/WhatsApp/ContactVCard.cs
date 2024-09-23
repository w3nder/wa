// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactVCard
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using WhatsApp.ContactClasses;


namespace WhatsApp
{
  public class ContactVCard
  {
    public string NameTitle;
    public string FirstName;
    public string MiddleName;
    public string LastName;
    public string Suffix;
    public string Nickname;
    public string DisplayName;
    public string Photo;
    public ContactVCard.Address[] Addresses;
    public ContactVCard.EmailAddress[] EmailAddresses;
    public ContactVCard.PhoneNumber[] PhoneNumbers;
    public DateTime Birthday;
    public string[] Websites;
    public string Note;
    public string JobTitle;
    public string CompanyName;

    public bool HasName
    {
      get => !string.IsNullOrEmpty(this.FirstName) || !string.IsNullOrEmpty(this.LastName);
    }

    private string TakeFirstNonEmpty(string one, string two)
    {
      return string.IsNullOrEmpty(one) ? two : one;
    }

    public void Merge(ContactVCard otherCard)
    {
      this.NameTitle = this.TakeFirstNonEmpty(this.NameTitle, otherCard.NameTitle);
      this.FirstName = this.TakeFirstNonEmpty(this.FirstName, otherCard.FirstName);
      this.MiddleName = this.TakeFirstNonEmpty(this.MiddleName, otherCard.MiddleName);
      this.LastName = this.TakeFirstNonEmpty(this.LastName, otherCard.LastName);
      this.Suffix = this.TakeFirstNonEmpty(this.Suffix, otherCard.Suffix);
      this.Nickname = this.TakeFirstNonEmpty(this.Nickname, otherCard.Nickname);
      this.Photo = this.TakeFirstNonEmpty(this.Photo, otherCard.Photo);
      this.Note = this.TakeFirstNonEmpty(this.Note, otherCard.Note);
      this.JobTitle = this.TakeFirstNonEmpty(this.JobTitle, otherCard.JobTitle);
      this.CompanyName = this.TakeFirstNonEmpty(this.CompanyName, otherCard.CompanyName);
      bool flag = otherCard.CountNameFields() > this.CountNameFields();
      if (!string.IsNullOrEmpty(otherCard.FirstName) & flag)
        this.FirstName = otherCard.FirstName;
      if (!string.IsNullOrEmpty(otherCard.LastName) & flag)
        this.LastName = otherCard.LastName;
      if (!string.IsNullOrEmpty(otherCard.MiddleName) & flag)
        this.MiddleName = otherCard.MiddleName;
      if (this.Birthday == new DateTime())
        this.Birthday = otherCard.Birthday;
      this.Addresses = ((IEnumerable<ContactVCard.Address>) this.Addresses).Concat<ContactVCard.Address>((IEnumerable<ContactVCard.Address>) otherCard.Addresses).Distinct<ContactVCard.Address>().ToArray<ContactVCard.Address>();
      this.EmailAddresses = ((IEnumerable<ContactVCard.EmailAddress>) this.EmailAddresses).Concat<ContactVCard.EmailAddress>((IEnumerable<ContactVCard.EmailAddress>) otherCard.EmailAddresses).Distinct<ContactVCard.EmailAddress>().ToArray<ContactVCard.EmailAddress>();
      this.PhoneNumbers = ((IEnumerable<ContactVCard.PhoneNumber>) this.PhoneNumbers).Concat<ContactVCard.PhoneNumber>((IEnumerable<ContactVCard.PhoneNumber>) otherCard.PhoneNumbers).Distinct<ContactVCard.PhoneNumber>().ToArray<ContactVCard.PhoneNumber>();
    }

    private int CountNameFields()
    {
      int num = 0;
      if (!string.IsNullOrEmpty(this.FirstName))
        ++num;
      if (!string.IsNullOrEmpty(this.MiddleName))
        ++num;
      if (!string.IsNullOrEmpty(this.LastName))
        ++num;
      return num;
    }

    public static ContactVCard Create(Contact contact)
    {
      ContactVCard contactVcard = new ContactVCard();
      if (contact.CompleteName != null)
      {
        contactVcard.NameTitle = contact.CompleteName.Title;
        contactVcard.FirstName = contact.CompleteName.FirstName;
        contactVcard.MiddleName = contact.CompleteName.MiddleName;
        contactVcard.LastName = contact.CompleteName.LastName;
        contactVcard.Suffix = contact.CompleteName.Suffix;
        contactVcard.Nickname = contact.CompleteName.Nickname;
      }
      if (contact.PhoneNumbers != null && contact.PhoneNumbers.Count<ContactPhoneNumber>() > 0)
      {
        List<ContactVCard.PhoneNumber> contactNumbers = new List<ContactVCard.PhoneNumber>();
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          foreach (ContactPhoneNumber phoneNumber in contact.PhoneNumbers)
          {
            string jid = db.PhoneNumberForRawNumber(phoneNumber.PhoneNumber)?.Jid;
            string str = jid != null ? JidHelper.GetPhoneNumber(jid, true) : phoneNumber.PhoneNumber;
            contactNumbers.Add(new ContactVCard.PhoneNumber()
            {
              Kind = phoneNumber.Kind,
              Number = str,
              Jid = jid
            });
          }
        }));
        contactVcard.PhoneNumbers = contactNumbers.ToArray();
      }
      else
        contactVcard.PhoneNumbers = new ContactVCard.PhoneNumber[0];
      contactVcard.Addresses = ((IEnumerable<ContactAddress>) ((object) contact.Addresses ?? (object) new ContactAddress[0])).Select<ContactAddress, ContactVCard.Address>((Func<ContactAddress, ContactVCard.Address>) (addr => new ContactVCard.Address()
      {
        Kind = addr.Kind,
        Street = addr.PhysicalAddress != null ? addr.PhysicalAddress.AddressLine1 + (addr.PhysicalAddress.AddressLine2 == null ? "" : "\n" + addr.PhysicalAddress.AddressLine2) : (string) null,
        City = addr.PhysicalAddress != null ? addr.PhysicalAddress.City : (string) null,
        State = addr.PhysicalAddress != null ? addr.PhysicalAddress.StateProvince : (string) null,
        PostalCode = addr.PhysicalAddress != null ? addr.PhysicalAddress.PostalCode : (string) null,
        Country = addr.PhysicalAddress != null ? addr.PhysicalAddress.CountryRegion : (string) null
      })).ToArray<ContactVCard.Address>();
      contactVcard.EmailAddresses = ((IEnumerable<ContactEmailAddress>) ((object) contact.EmailAddresses ?? (object) new ContactEmailAddress[0])).Select<ContactEmailAddress, ContactVCard.EmailAddress>((Func<ContactEmailAddress, ContactVCard.EmailAddress>) (email => new ContactVCard.EmailAddress()
      {
        Address = email.EmailAddress
      })).ToArray<ContactVCard.EmailAddress>();
      using (Stream picture = contact.GetPicture())
        contactVcard.SetPhoto(picture);
      contactVcard.Birthday = ((IEnumerable<DateTime>) ((object) contact.Birthdays ?? (object) new DateTime[0])).FirstOrDefault<DateTime>();
      contactVcard.Websites = ((IEnumerable<string>) ((object) contact.Websites ?? (object) new string[0])).ToArray<string>();
      contactVcard.Note = string.Join("\n\n", (IEnumerable<string>) ((object) contact.Notes ?? (object) new string[0]));
      ContactCompanyInformation companyInformation = ((IEnumerable<ContactCompanyInformation>) ((object) contact.Companies ?? (object) new ContactCompanyInformation[0])).FirstOrDefault<ContactCompanyInformation>();
      if (companyInformation != null)
      {
        contactVcard.CompanyName = companyInformation.CompanyName;
        contactVcard.JobTitle = companyInformation.JobTitle;
      }
      return contactVcard;
    }

    public static ContactVCard Create(string data)
    {
      return ContactVCard.Create(data, ContactVCardParser.ParseCriteria.All);
    }

    public static ContactVCard Create(string data, ContactVCardParser.ParseCriteria criteria)
    {
      return ContactVCardParser.Parse(data, criteria);
    }

    public void SetPhoto(Stream photoStream)
    {
      if (photoStream == null)
        return;
      MemoryStream memStream = new MemoryStream();
      photoStream.CopyTo((Stream) memStream);
      memStream.Position = 0L;
      WriteableBitmap bitmap = (WriteableBitmap) null;
      bool shouldResize = false;
      Deployment.Current.Dispatcher.InvokeSynchronous((Action) (() =>
      {
        bitmap = PictureDecoder.DecodeJpeg((Stream) memStream);
        shouldResize = memStream.Length > 40960L || bitmap.PixelHeight > 64 || bitmap.PixelWidth > 64;
      }));
      if (shouldResize)
      {
        memStream.Dispose();
        memStream = new MemoryStream();
        bitmap.SaveJpegWithMaxSize((Stream) memStream, 64, 64, 0, Settings.JpegQuality, 40960);
      }
      using (memStream)
        this.Photo = Convert.ToBase64String(memStream.ToArray());
    }

    public WriteableBitmap GetPhotoBitmap(int maxPixelWidth = 0, int maxPixelHeight = 0)
    {
      WriteableBitmap photoBitmap = (WriteableBitmap) null;
      if (this.Photo != null)
        photoBitmap = BitmapUtils.CreateBitmap(this.Photo, maxPixelWidth, maxPixelHeight);
      return photoBitmap;
    }

    public string GetDisplayName(bool fallbackToNicknameOrCompanyName)
    {
      if (this.DisplayName != null)
        return this.DisplayName;
      string str = string.Join(" ", ((IEnumerable<string>) new string[5]
      {
        this.NameTitle,
        this.FirstName,
        this.MiddleName,
        this.LastName,
        this.Suffix
      }).Where<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s)))).Trim();
      if (string.IsNullOrEmpty(str) & fallbackToNicknameOrCompanyName)
        str = string.IsNullOrEmpty(this.Nickname) ? this.CompanyName : this.Nickname;
      return str ?? "";
    }

    public Message ToMessage(MessagesContext db, string vcardName = null)
    {
      return new Message(true)
      {
        KeyFromMe = true,
        KeyId = FunXMPP.GenerateMessageId(),
        MediaName = vcardName ?? this.GetDisplayName(true),
        Data = this.ToVCardData(true),
        Status = FunXMPP.FMessage.Status.Unsent,
        MediaWaType = FunXMPP.FMessage.Type.Contact
      };
    }

    public string ToVCardData(bool withJids)
    {
      StringBuilder buf = new StringBuilder();
      int num = 0;
      buf.Append("BEGIN:VCARD\nVERSION:3.0\n");
      buf.Append("N:");
      this.EncodeSemicolons(buf, this.LastName, this.FirstName, this.MiddleName, this.NameTitle, this.Suffix);
      buf.Append("\n");
      buf.Append("FN:");
      this.EncodeSemicolons(buf, this.GetDisplayName(true));
      buf.Append("\n");
      if (!string.IsNullOrEmpty(this.Nickname))
      {
        buf.Append("NICKNAME:");
        buf.Append(this.Nickname);
        buf.Append("\n");
      }
      if (withJids)
      {
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          foreach (ContactVCard.PhoneNumber phoneNum in this.PhoneNumbers ?? new ContactVCard.PhoneNumber[0])
            this.WritePhoneNumberVCardLine(buf, phoneNum, true, db);
        }));
      }
      else
      {
        foreach (ContactVCard.PhoneNumber phoneNum in this.PhoneNumbers ?? new ContactVCard.PhoneNumber[0])
          this.WritePhoneNumberVCardLine(buf, phoneNum);
      }
      foreach (ContactVCard.EmailAddress emailAddress in this.EmailAddresses ?? new ContactVCard.EmailAddress[0])
      {
        buf.Append("EMAIL;type=INTERNET:");
        this.EncodeSemicolons(buf, emailAddress.Address);
        buf.Append("\n");
      }
      foreach (ContactVCard.Address address in this.Addresses ?? new ContactVCard.Address[0])
      {
        string addressType = this.GetAddressType(address.Kind);
        if (addressType != null)
        {
          string str = string.Format("item{0}.", (object) num++);
          buf.Append(str);
          buf.Append("ADR;type=");
          buf.Append(addressType);
          buf.Append(':');
          this.EncodeSemicolons(buf, null, null, address.Street, address.City, address.State, address.PostalCode, address.Country);
          buf.Append("\n");
        }
      }
      if (this.Photo != null)
      {
        buf.Append("PHOTO;BASE64:");
        buf.Append(this.Photo);
        buf.Append("\n");
      }
      if (this.Birthday != new DateTime())
      {
        buf.Append("BDAY:");
        buf.Append(this.Birthday.ToString("yyyy-MM-dd"));
        buf.Append("\n");
      }
      foreach (string str in this.Websites ?? new string[0])
      {
        buf.Append("URL:");
        buf.Append(str);
        buf.Append("\n");
      }
      if (this.Note != null)
      {
        buf.Append("NOTE:");
        buf.Append(this.Note.ConvertLineEndings().Replace("\n", "\\n"));
        buf.Append("\n");
      }
      if (this.JobTitle != null)
      {
        buf.Append("TITLE:");
        buf.Append(this.JobTitle);
        buf.Append("\n");
      }
      if (this.CompanyName != null)
      {
        buf.Append("ORG:");
        buf.Append(this.CompanyName);
        buf.Append("\n");
      }
      buf.Append("END:VCARD");
      return buf.ToString();
    }

    private void WritePhoneNumberVCardLine(
      StringBuilder sb,
      ContactVCard.PhoneNumber phoneNum,
      bool withJid = false,
      ContactsContext db = null)
    {
      string phoneType = this.GetPhoneType(phoneNum.Kind);
      if (phoneType == null)
        return;
      string jid = phoneNum.Jid;
      if (withJid && jid == null && db != null)
      {
        jid = db.JidForRawNumber(phoneNum.Number);
        if (!string.IsNullOrEmpty(jid) && db.GetUserStatus(jid, false) == null)
          jid = (string) null;
      }
      string str = (string) null;
      if (jid != null)
        str = JidHelper.GetPhoneNumber(jid, false);
      if (string.IsNullOrEmpty(str))
        sb.AppendFormat("TEL;type={0}:{1}\n", (object) phoneType, (object) phoneNum.Number);
      else
        sb.AppendFormat("TEL;type={0};waid={1}:{2}\n", (object) phoneType, (object) str, (object) phoneNum.Number);
    }

    private string GetPhoneType(PhoneNumberKind kind)
    {
      switch (kind)
      {
        case PhoneNumberKind.Mobile:
          return "CELL";
        case PhoneNumberKind.Home:
          return "HOME";
        case PhoneNumberKind.Work:
        case PhoneNumberKind.Company:
          return "WORK";
        case PhoneNumberKind.Pager:
          return "PAGER";
        case PhoneNumberKind.HomeFax:
          return "HOME;type=FAX";
        case PhoneNumberKind.WorkFax:
          return "WORK;type=FAX";
        default:
          return (string) null;
      }
    }

    private string GetAddressType(AddressKind kind)
    {
      switch (kind)
      {
        case AddressKind.Home:
        case AddressKind.Other:
          return "HOME";
        case AddressKind.Work:
          return "WORK";
        default:
          return (string) null;
      }
    }

    private void EncodeSemicolons(StringBuilder buf, params string[] args)
    {
      bool flag = false;
      foreach (string str in args)
      {
        if (flag)
          buf.Append(';');
        else
          flag = true;
        if (str == null)
          str = "";
        foreach (char ch in str)
        {
          switch (ch)
          {
            case '\n':
              buf.Append("\\n");
              break;
            case '\r':
              buf.Append("\\r");
              break;
            case ';':
            case '\\':
              buf.Append('\\');
              goto default;
            default:
              buf.Append(ch);
              break;
          }
        }
      }
    }

    public struct Address
    {
      public AddressKind Kind;
      public string Street;
      public string City;
      public string State;
      public string PostalCode;
      public string Country;
    }

    public struct EmailAddress
    {
      public string Address;
    }

    public struct PhoneNumber
    {
      public PhoneNumberKind Kind;
      public string Number;
      public string Jid;

      public override bool Equals(object obj)
      {
        return obj is ContactVCard.PhoneNumber phoneNumber && this == phoneNumber;
      }

      public override int GetHashCode()
      {
        return this.Kind.GetHashCode() ^ this.Number.GetHashCode() ^ this.Jid.GetHashCode();
      }

      public static bool operator ==(ContactVCard.PhoneNumber x, ContactVCard.PhoneNumber y)
      {
        if (x.Jid == null && x.Number == null)
          return x.Number == y.Number;
        return x.Jid.Equals(y.Jid) || x.Number.Equals(y.Number);
      }

      public static bool operator !=(ContactVCard.PhoneNumber x, ContactVCard.PhoneNumber y)
      {
        return !(x == y);
      }
    }
  }
}
