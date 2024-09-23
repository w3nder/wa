// Decompiled with JetBrains decompiler
// Type: WhatsApp.ImageSearchResult
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Windows.Media.Imaging;

#nullable disable
namespace WhatsApp
{
  public class ImageSearchResult
  {
    private IObservable<WriteableBitmap> thumbnail;
    private IObservable<WriteableBitmap> contents;
    private string sourcePageUrl;

    public ImageSearchResult(
      IObservable<WriteableBitmap> thumbnail,
      IObservable<WriteableBitmap> contents,
      string sourceUrl)
    {
      this.thumbnail = thumbnail;
      this.contents = contents;
      this.sourcePageUrl = sourceUrl;
    }

    public IObservable<WriteableBitmap> Thumbnail => this.thumbnail;

    public IObservable<WriteableBitmap> Contents => this.contents;

    public string SourcePageUrl => this.sourcePageUrl;
  }
}
