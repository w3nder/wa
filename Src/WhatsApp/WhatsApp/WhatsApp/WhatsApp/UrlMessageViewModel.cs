// Decompiled with JetBrains decompiler
// Type: WhatsApp.UrlMessageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;


namespace WhatsApp
{
  public class UrlMessageViewModel : TextMessageViewModel
  {
    private UriMessageWrapper uriMsg;

    public static bool ShouldRenderAsUrlMessage(Message m)
    {
      return new UriMessageWrapper(m).MatchedText != null;
    }

    public override Thickness ViewPanelMargin
    {
      get
      {
        double num = 12.0 * this.zoomMultiplier;
        return new Thickness(num, this.ViewPanelTopMargin, num, num);
      }
    }

    public string TitleStr => this.uriMsg != null ? this.uriMsg.Title : (string) null;

    public string DescriptionStr
    {
      get
      {
        if (this.uriMsg == null)
          return (string) null;
        string description = this.uriMsg.Description;
        return description == null ? (string) null : description.ReplaceLineBreaksWithSpaces();
      }
    }

    public string HostNameStr
    {
      get
      {
        string uriString = this.uriMsg == null ? (string) null : this.uriMsg.CanonicalUrl;
        if (string.IsNullOrEmpty(uriString))
          uriString = this.uriMsg.MatchedText;
        if (uriString != null)
        {
          try
          {
            return new Uri(uriString).Host;
          }
          catch (Exception ex)
          {
          }
        }
        return (string) null;
      }
    }

    public override Thickness ForwardedRowMargin
    {
      get
      {
        return base.ForwardedRowMargin with
        {
          Bottom = this.Message.KeyFromMe ? -4.0 : 6.0
        };
      }
    }

    public override bool ShouldShowFooter => !this.IsForGalleryView && base.ShouldShowFooter;

    public UrlMessageViewModel(Message m)
      : base(m)
    {
      if (m.MediaWaType != FunXMPP.FMessage.Type.ExtendedText)
        return;
      this.uriMsg = new UriMessageWrapper(m);
    }

    public override MessageViewModel.ThumbnailState GetThumbnail(
      MessageViewModel.ThumbnailOptions thumbOptions = MessageViewModel.ThumbnailOptions.Standard)
    {
      MessageViewModel.ThumbnailState thumbnail = this.GetCachedThumbnail(thumbOptions);
      if (thumbnail == null && this.Message != null && this.Message.BinaryData != null)
        thumbnail = new MessageViewModel.ThumbnailState((System.Windows.Media.ImageSource) BitmapUtils.CreateBitmap(this.Message.BinaryData), this.Message.KeyId, false);
      return thumbnail;
    }
  }
}
