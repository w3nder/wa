// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.AddressBookParsedResult
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Text;

#nullable disable
namespace ZXing.Client.Result
{
  /// <author>Sean Owen</author>
  public sealed class AddressBookParsedResult : ParsedResult
  {
    private readonly string[] names;
    private readonly string[] nicknames;
    private readonly string pronunciation;
    private readonly string[] phoneNumbers;
    private readonly string[] phoneTypes;
    private readonly string[] emails;
    private readonly string[] emailTypes;
    private readonly string instantMessenger;
    private readonly string note;
    private readonly string[] addresses;
    private readonly string[] addressTypes;
    private readonly string org;
    private readonly string birthday;
    private readonly string title;
    private readonly string[] urls;
    private readonly string[] geo;

    public AddressBookParsedResult(
      string[] names,
      string[] phoneNumbers,
      string[] phoneTypes,
      string[] emails,
      string[] emailTypes,
      string[] addresses,
      string[] addressTypes)
      : this(names, (string[]) null, (string) null, phoneNumbers, phoneTypes, emails, emailTypes, (string) null, (string) null, addresses, addressTypes, (string) null, (string) null, (string) null, (string[]) null, (string[]) null)
    {
    }

    public AddressBookParsedResult(
      string[] names,
      string[] nicknames,
      string pronunciation,
      string[] phoneNumbers,
      string[] phoneTypes,
      string[] emails,
      string[] emailTypes,
      string instantMessenger,
      string note,
      string[] addresses,
      string[] addressTypes,
      string org,
      string birthday,
      string title,
      string[] urls,
      string[] geo)
      : base(ParsedResultType.ADDRESSBOOK)
    {
      this.names = names;
      this.nicknames = nicknames;
      this.pronunciation = pronunciation;
      this.phoneNumbers = phoneNumbers;
      this.phoneTypes = phoneTypes;
      this.emails = emails;
      this.emailTypes = emailTypes;
      this.instantMessenger = instantMessenger;
      this.note = note;
      this.addresses = addresses;
      this.addressTypes = addressTypes;
      this.org = org;
      this.birthday = birthday;
      this.title = title;
      this.urls = urls;
      this.geo = geo;
      this.displayResultValue = this.getDisplayResult();
    }

    public string[] Names => this.names;

    public string[] Nicknames => this.nicknames;

    /// <summary>
    /// In Japanese, the name is written in kanji, which can have multiple readings. Therefore a hint
    /// is often provided, called furigana, which spells the name phonetically.
    /// </summary>
    /// <return>The pronunciation of the getNames() field, often in hiragana or katakana.</return>
    public string Pronunciation => this.pronunciation;

    public string[] PhoneNumbers => this.phoneNumbers;

    /// <return>optional descriptions of the type of each phone number. It could be like "HOME", but,
    /// there is no guaranteed or standard format.</return>
    public string[] PhoneTypes => this.phoneTypes;

    public string[] Emails => this.emails;

    /// <return>optional descriptions of the type of each e-mail. It could be like "WORK", but,
    /// there is no guaranteed or standard format.</return>
    public string[] EmailTypes => this.emailTypes;

    public string InstantMessenger => this.instantMessenger;

    public string Note => this.note;

    public string[] Addresses => this.addresses;

    /// <return>optional descriptions of the type of each e-mail. It could be like "WORK", but,
    /// there is no guaranteed or standard format.</return>
    public string[] AddressTypes => this.addressTypes;

    public string Title => this.title;

    public string Org => this.org;

    public string[] URLs => this.urls;

    /// <return>birthday formatted as yyyyMMdd (e.g. 19780917)</return>
    public string Birthday => this.birthday;

    /// <return>a location as a latitude/longitude pair</return>
    public string[] Geo => this.geo;

    private string getDisplayResult()
    {
      StringBuilder result = new StringBuilder(100);
      ParsedResult.maybeAppend(this.names, result);
      ParsedResult.maybeAppend(this.nicknames, result);
      ParsedResult.maybeAppend(this.pronunciation, result);
      ParsedResult.maybeAppend(this.title, result);
      ParsedResult.maybeAppend(this.org, result);
      ParsedResult.maybeAppend(this.addresses, result);
      ParsedResult.maybeAppend(this.phoneNumbers, result);
      ParsedResult.maybeAppend(this.emails, result);
      ParsedResult.maybeAppend(this.instantMessenger, result);
      ParsedResult.maybeAppend(this.urls, result);
      ParsedResult.maybeAppend(this.birthday, result);
      ParsedResult.maybeAppend(this.geo, result);
      ParsedResult.maybeAppend(this.note, result);
      return result.ToString();
    }
  }
}
