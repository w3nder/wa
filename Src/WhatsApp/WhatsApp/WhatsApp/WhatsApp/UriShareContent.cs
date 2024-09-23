// Decompiled with JetBrains decompiler
// Type: WhatsApp.UriShareContent
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;


namespace WhatsApp
{
  public class UriShareContent : IExternalShare
  {
    public static readonly string URI_SCHEME = "UriScheme";
    public static readonly string SHARE_TYPE_ID = "type";
    public static readonly string SHARE_TYPE_DATA = nameof (data);
    public static readonly string SHARE_DATA_TEXT = "dataText";
    public static readonly string SHARE_DATA_PHONE = "dataPhone";
    public static readonly string SHARE_DATA_SOURCE = "dataSource";
    public static readonly string SHARE_DATA_DATA = "dataData";
    private bool isValid;
    private string contentType;
    private string text;
    private string phone;
    private string source;
    private string data;

    public string TextFromLink => this.text;

    public string PhoneNumberFromLink => this.phone;

    public string SourceFromLink => this.source;

    public string DataFromLink => this.data;

    public UriShareContent(IDictionary<string, string> queryStrings)
    {
      queryStrings.TryGetValue(UriShareContent.SHARE_DATA_SOURCE, out this.source);
      queryStrings.TryGetValue(UriShareContent.SHARE_DATA_DATA, out this.data);
      if (!queryStrings.TryGetValue(UriShareContent.SHARE_TYPE_ID, out this.contentType) || !(queryStrings.TryGetValue(UriShareContent.SHARE_DATA_TEXT, out this.text) | queryStrings.TryGetValue(UriShareContent.SHARE_DATA_PHONE, out this.phone)) || !(this.contentType == UriShareContent.SHARE_TYPE_DATA))
        return;
      this.isValid = true;
    }

    public bool IsValid => this.isValid;

    public bool ShouldConfirmSending() => false;

    public IObservable<ExternalShare.ExternalShareResult> ShareContent(List<string> jids)
    {
      ExternalShare.ExternalShareResult externalShareResult = ExternalShare.ExternalShareResult.Unknown;
      if (this.contentType == UriShareContent.SHARE_TYPE_DATA)
        externalShareResult = ExternalShare.ShareTextContent(jids, this.text, jids.Count == 1);
      return Observable.Return<ExternalShare.ExternalShareResult>(externalShareResult);
    }

    public IObservable<bool> ShouldEnableSharingToStatus() => Observable.Return<bool>(false);

    public string DescribeError(ExternalShare.ExternalShareResult result)
    {
      Log.l(nameof (UriShareContent), "DescribeError {0}", (object) result);
      return (string) null;
    }

    public IObservable<bool> GetTruncationCheck()
    {
      return Observable.Return<bool>(this.text != null && this.text.Length > 65536);
    }
  }
}
