// Decompiled with JetBrains decompiler
// Type: WhatsApp.ContactVCardParser
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;


namespace WhatsApp
{
  public class ContactVCardParser
  {
    private static int maxVCardStringLength = 256;

    public static ContactVCard Parse(string vcardData)
    {
      return ContactVCardParser.Parse(vcardData, ContactVCardParser.ParseCriteria.All);
    }

    public static ContactVCard Parse(string vCardData, ContactVCardParser.ParseCriteria criteria)
    {
      if (vCardData == null)
        return (ContactVCard) null;
      try
      {
        return ContactVCardParser.ParseImpl(vCardData, criteria, (Func<ContactVCard, bool>) null);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "parse vCard");
      }
      return (ContactVCard) null;
    }

    private static ContactVCard ParseImpl(
      string vCardData,
      ContactVCardParser.ParseCriteria criteria,
      Func<ContactVCard, bool> terminationPredicate)
    {
      if (string.IsNullOrEmpty(vCardData))
        return (ContactVCard) null;
      ContactVCard vCard = new ContactVCard();
      bool flag1 = (criteria & ContactVCardParser.ParseCriteria.Name) != 0;
      bool flag2 = (criteria & ContactVCardParser.ParseCriteria.Photo) != 0;
      bool flag3 = (criteria & ContactVCardParser.ParseCriteria.Details) != 0;
      List<ContactVCard.Address> addressList = new List<ContactVCard.Address>();
      List<ContactVCard.EmailAddress> emailAddressList = new List<ContactVCard.EmailAddress>();
      List<ContactVCard.PhoneNumber> phoneNumberList = new List<ContactVCard.PhoneNumber>();
      List<string> stringList = new List<string>();
      string[] strArray = vCardData.ConvertLineEndings().Split('\n');
      bool flag4 = false;
      foreach (string s in strArray)
      {
        ContactVCardParser.VCardProperty vcardProperty = ContactVCardParser.ParseVCardProperty(s);
        if (vcardProperty == null)
        {
          if (flag4)
            vCard.Photo += s.Trim();
          else
            Log.l("vcard", "invalid vcard line:{0}", (object) s);
        }
        else
        {
          flag4 = false;
          if (!(vcardProperty.Name == "END") && (terminationPredicate == null || !terminationPredicate(vCard)))
          {
            if (vcardProperty.Name == "N")
            {
              if (flag1)
                ContactVCardParser.ParseNames(vcardProperty, ref vCard);
            }
            else if (vcardProperty.Name == "TEL")
            {
              if (flag3)
                phoneNumberList.Add(ContactVCardParser.ParsePhone(vcardProperty.Value, vcardProperty.Params));
            }
            else if (vcardProperty.Name == "EMAIL")
            {
              if (flag3)
                emailAddressList.Add(ContactVCardParser.ParseEmail(vcardProperty.Value, vcardProperty.Params));
            }
            else if (vcardProperty.Name == "ADR")
            {
              if (flag3)
                addressList.Add(ContactVCardParser.ParseAddress(vcardProperty.Value, vcardProperty.Params));
            }
            else if (vcardProperty.Name == "NICKNAME")
            {
              if (flag3)
              {
                vcardProperty.Value = ContactVCardParser.TruncateStringIfNeeeded(vcardProperty.Value, "NICKNAME");
                vCard.Nickname = Emoji.ConvertToTextOnly(vcardProperty.Value, (byte[]) null);
              }
            }
            else if (vcardProperty.Name == "PHOTO")
            {
              if (flag2)
                vCard.Photo = vcardProperty.Value.Trim();
              flag4 = true;
            }
            else if (vcardProperty.Name == "URL")
            {
              if (flag3)
                stringList.Add(vcardProperty.Value);
            }
            else if (vcardProperty.Name == "BDAY")
            {
              DateTime result;
              if (flag3 && DateTime.TryParseExact(vcardProperty.Value, "yyyy-MM-dd", (IFormatProvider) null, DateTimeStyles.None, out result))
                vCard.Birthday = result;
            }
            else if (vcardProperty.Name == "NOTE")
            {
              if (flag3)
              {
                vcardProperty.Value = ContactVCardParser.TruncateStringIfNeeeded(vcardProperty.Value, "NOTE");
                vCard.Note = Emoji.ConvertToTextOnly(vcardProperty.Value.Replace("\\n", "\n"), (byte[]) null);
              }
            }
            else if (vcardProperty.Name == "TITLE")
            {
              if (flag3)
              {
                vcardProperty.Value = ContactVCardParser.TruncateStringIfNeeeded(vcardProperty.Value, "TITLE");
                vCard.JobTitle = Emoji.ConvertToTextOnly(vcardProperty.Value, (byte[]) null);
              }
            }
            else if (vcardProperty.Name == "ORG" && flag3)
            {
              vcardProperty.Value = ContactVCardParser.TruncateStringIfNeeeded(vcardProperty.Value, "ORG");
              vCard.CompanyName = Emoji.ConvertToTextOnly(((IEnumerable<string>) vcardProperty.Value.Split(';')).FirstOrDefault<string>(), (byte[]) null);
            }
          }
          else
            break;
        }
      }
      vCard.Addresses = addressList.ToArray();
      vCard.EmailAddresses = emailAddressList.ToArray();
      vCard.PhoneNumbers = phoneNumberList.ToArray();
      vCard.Websites = stringList.ToArray();
      return vCard;
    }

    private static string TruncateStringIfNeeeded(string str, string context)
    {
      if (str == null || str.Length <= ContactVCardParser.maxVCardStringLength)
        return str;
      Log.d("vcard", "Truncating {0}", (object) context);
      return Utils.TruncateAtIndex(str, ContactVCardParser.maxVCardStringLength);
    }

    public static string GetBase64PhotoData(string vcardData)
    {
      return ContactVCardParser.ParseImpl(vcardData, ContactVCardParser.ParseCriteria.Photo, (Func<ContactVCard, bool>) (c => c.Photo != null))?.Photo;
    }

    private static ContactVCardParser.VCardProperty ParseVCardProperty(string s)
    {
      ContactVCardParser.VCardProperty vcardProperty = (ContactVCardParser.VCardProperty) null;
      if (!string.IsNullOrEmpty(s))
      {
        int length = s.IndexOf(':');
        if (length > 0)
        {
          string val = s.Substring(0, length);
          string str1 = s.Substring(length + 1);
          string[] source = ContactVCardParser.PumpSemicolons(val);
          if (((IEnumerable<string>) source).Any<string>())
          {
            string str2 = ((IEnumerable<string>) source).First<string>();
            if (str2.StartsWith("item"))
            {
              int num = str2.IndexOf('.');
              if (num == -1)
                Log.l("vcard", "has item but no dot:{0}", (object) s);
              else
                str2 = str2.Substring(num + 1);
            }
            if (!string.IsNullOrEmpty(str2))
              vcardProperty = new ContactVCardParser.VCardProperty()
              {
                Name = str2,
                Params = string.Join(";", ((IEnumerable<string>) source).Skip<string>(1)),
                Value = str1
              };
          }
        }
      }
      return vcardProperty;
    }

    private static void ParseNames(
      ContactVCardParser.VCardProperty nameProperty,
      ref ContactVCard vCard)
    {
      Func<string, string> func = nameProperty.Params.IndexOf("ENCODING=QUOTED-PRINTABLE") != -1 ? (Func<string, string>) (s => Emoji.ConvertToTextOnly(Utils.DecodeQuotedPrintable(s), (byte[]) null)) : (Func<string, string>) (s => Emoji.ConvertToTextOnly(s, (byte[]) null));
      string[] strArray = ContactVCardParser.PumpSemicolons(nameProperty.Value);
      vCard.LastName = func(strArray.Length != 0 ? ContactVCardParser.TruncateStringIfNeeeded(strArray[0], "LastName") : "");
      vCard.FirstName = func(strArray.Length > 1 ? ContactVCardParser.TruncateStringIfNeeeded(strArray[1], "FirstName") : "");
      vCard.MiddleName = func(strArray.Length > 2 ? ContactVCardParser.TruncateStringIfNeeeded(strArray[2], "MiddleName") : (string) null);
      vCard.NameTitle = func(strArray.Length > 3 ? ContactVCardParser.TruncateStringIfNeeeded(strArray[3], "NameTitle") : (string) null);
      vCard.Suffix = func(strArray.Length > 4 ? ContactVCardParser.TruncateStringIfNeeeded(strArray[4], "Suffix") : (string) null);
    }

    private static ContactVCard.Address ParseAddress(string fullValue, string paramsStr)
    {
      ContactVCard.Address address = new ContactVCard.Address();
      address.Kind = paramsStr.IndexOf("WORK") == -1 ? AddressKind.Home : AddressKind.Work;
      string[] strArray = ContactVCardParser.PumpSemicolons(fullValue);
      if (strArray.Length > 2)
      {
        string str = strArray[2];
        address.Street = ContactVCardParser.TruncateStringIfNeeeded(str, "street");
      }
      if (strArray.Length > 3)
      {
        string str = strArray[3];
        address.City = ContactVCardParser.TruncateStringIfNeeeded(str, "city");
      }
      if (strArray.Length > 4)
      {
        string str = strArray[4];
        address.State = ContactVCardParser.TruncateStringIfNeeeded(str, "state");
      }
      if (strArray.Length > 5)
      {
        string str = strArray[5];
        address.PostalCode = ContactVCardParser.TruncateStringIfNeeeded(str, "zip");
      }
      if (strArray.Length > 6)
      {
        string str = strArray[6];
        address.Country = ContactVCardParser.TruncateStringIfNeeeded(str, "country");
      }
      return address;
    }

    private static ContactVCard.EmailAddress ParseEmail(string fullValue, string paramsStr)
    {
      ContactVCard.EmailAddress email;
      email.Address = ContactVCardParser.TruncateStringIfNeeeded(fullValue, "email");
      return email;
    }

    private static ContactVCard.PhoneNumber ParsePhone(string num, string paramsStr)
    {
      ContactVCard.PhoneNumber phone = new ContactVCard.PhoneNumber();
      int num1 = paramsStr.IndexOf("waid=");
      if (num1 >= 0)
      {
        int startIndex = num1 + "waid=".Length;
        int num2 = paramsStr.IndexOf(';', startIndex + 1);
        string str = num2 > startIndex ? paramsStr.Substring(startIndex, num2 - startIndex) : paramsStr.Substring(startIndex);
        if (str.All<char>((Func<char, bool>) (ch => char.IsDigit(ch))))
        {
          string jid = JidHelper.FromRawNumber(str);
          phone.Jid = jid;
          phone.Number = JidHelper.GetPhoneNumber(jid, true);
        }
      }
      if (phone.Jid == null && phone.Number == null)
        phone.Number = ContactVCardParser.TruncateStringIfNeeeded(num, "phone");
      phone.Kind = paramsStr.IndexOf("CELL") != -1 || paramsStr.IndexOf("IPHONE") != -1 ? PhoneNumberKind.Mobile : (paramsStr.IndexOf("FAX") == -1 ? (paramsStr.IndexOf("HOME") == -1 ? (paramsStr.IndexOf("WORK") == -1 ? (paramsStr.IndexOf("PAGER") == -1 ? PhoneNumberKind.Home : PhoneNumberKind.Pager) : PhoneNumberKind.Work) : PhoneNumberKind.Home) : PhoneNumberKind.WorkFax);
      return phone;
    }

    private static string[] PumpSemicolons(string val)
    {
      List<string> stringList = new List<string>();
      bool flag = false;
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < val.Length; ++index)
      {
        char ch = val[index];
        if (ch == ';' && !flag)
        {
          stringList.Add(stringBuilder.ToString());
          stringBuilder.Clear();
        }
        else if (flag)
        {
          switch (ch)
          {
            case 'n':
              stringBuilder.Append('\n');
              goto case 'r';
            case 'r':
              flag = false;
              continue;
            default:
              stringBuilder.Append(ch);
              goto case 'r';
          }
        }
        else if (ch == '\\')
          flag = true;
        else
          stringBuilder.Append(ch);
      }
      stringList.Add(stringBuilder.ToString());
      return stringList.ToArray();
    }

    private class VCardProperty
    {
      public string Name { get; set; }

      public string Params { get; set; }

      public string Value { get; set; }
    }

    public enum ParseCriteria
    {
      Name = 1,
      Photo = 2,
      Details = 4,
      All = 7,
    }
  }
}
