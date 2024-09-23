// Decompiled with JetBrains decompiler
// Type: WhatsApp.SupportSearchResult
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using Microsoft.Phone.Tasks;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

#nullable disable
namespace WhatsApp
{
  [DataContract]
  public class SupportSearchResult
  {
    public Action CustomAction;
    public DateTime startReadTime;

    [DataMember(Name = "url")]
    public string Url { get; set; }

    [DataMember(Name = "description")]
    public string DescriptionHtml { get; set; }

    [DataMember(Name = "lang")]
    public string Language { get; set; }

    [DataMember(Name = "platform")]
    public string Platform { get; set; }

    [DataMember(Name = "title")]
    public string Title { get; set; }

    public void Select()
    {
      if (this.CustomAction != null)
      {
        this.CustomAction();
      }
      else
      {
        if (this.Url == null)
          return;
        new WebBrowserTask() { Uri = new Uri(this.Url) }.Show();
        this.startReadTime = DateTime.Now;
      }
    }

    public int URLtoID()
    {
      int result;
      if (int.TryParse(this.Url.Substring(this.Url.LastIndexOf('/') + 1), out result))
        return result;
      Log.WriteLineDebug("Support article ID could not be parsed");
      return -1;
    }

    public static IObservable<SupportSearchResult[]> Fetch(
      string searchTerm,
      bool includeDescription)
    {
      return NativeWeb.SimpleGet(WaWebUrls.GetFaqSearchUrl(searchTerm, includeDescription)).Select<Stream, SupportSearchResult[]>((Func<Stream, SupportSearchResult[]>) (stream => new DataContractJsonSerializer(typeof (SupportSearchResult[])).ReadObject(stream) as SupportSearchResult[]));
    }
  }
}
