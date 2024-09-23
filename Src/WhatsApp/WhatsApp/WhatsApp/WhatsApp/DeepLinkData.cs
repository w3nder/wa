// Decompiled with JetBrains decompiler
// Type: WhatsApp.DeepLinkData
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Net;
using System.Text;


namespace WhatsApp
{
  public class DeepLinkData
  {
    public static string DeeplinkParamPhone = "phone";
    public static string DeeplinkParamSource = "source";
    public static string DeeplinkParamData = "data";
    private const int DeeplinkParamSourceMaxLength = 32;
    private const int DeeplinkParamDataMaxLength = 512;

    public string SharedPhoneNumber { get; private set; }

    public string SharedText { get; private set; }

    public string SharedSource { get; private set; }

    public byte[] SharedData { get; private set; }

    public DeepLinkData.DeepLinkTypes DeepLinkType
    {
      get
      {
        return this.SharedPhoneNumber != null ? DeepLinkData.DeepLinkTypes.Conversion : DeepLinkData.DeepLinkTypes.Text;
      }
    }

    private DeepLinkData(string phoneNumber, string text, string source, byte[] data)
    {
      this.SharedPhoneNumber = phoneNumber;
      this.SharedText = text;
      this.SharedSource = source;
      this.SharedData = data;
    }

    public static DeepLinkData CreateFrom(UriShareContent uriShared)
    {
      string phoneNumberFromLink = uriShared.PhoneNumberFromLink;
      string textFromLink = uriShared.TextFromLink;
      string str = WebUtility.UrlDecode(uriShared.SourceFromLink);
      if (str != null && str.Length > 32)
        ;
      byte[] data = (byte[]) null;
      if (!string.IsNullOrEmpty(uriShared.DataFromLink))
      {
        byte[] bytes = Encoding.UTF8.GetBytes(uriShared.DataFromLink);
        data = WebUtility.UrlDecodeToBytes(bytes, 0, bytes.Length);
        if (data != null && data.Length != 0 && data.Length > 512)
          data = (byte[]) null;
      }
      return new DeepLinkData(uriShared.PhoneNumberFromLink, uriShared.TextFromLink, uriShared.SourceFromLink, data);
    }

    public enum DeepLinkTypes
    {
      Text = 1,
      Conversion = 2,
    }
  }
}
