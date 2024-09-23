// Decompiled with JetBrains decompiler
// Type: WhatsApp.GifSearchResult
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public class GifSearchResult
  {
    public string GifPath { get; set; }

    public string GifPreviewPath { get; set; }

    public string Mp4Path { get; set; }

    public string Mp4PreviewPath { get; set; }

    public BitmapImage bitmap { get; set; }

    public MessageProperties.MediaProperties.Attribution Attribution { get; set; }
  }
}
