// Decompiled with JetBrains decompiler
// Type: WhatsApp.InAppFloatingBannerViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class InAppFloatingBannerViewModel : WaViewModelBase
  {
    private bool forError;
    private string headerStr;
    private string contentStr;
    private LinkDetector.Result[] formattings;

    public double ZoomFactor => ResolutionHelper.ZoomFactor;

    public System.Windows.Media.ImageSource IconSource
    {
      get => (System.Windows.Media.ImageSource) AssetStore.LoadAsset("whatsapp-icon.png", AssetStore.ThemeSetting.Dark);
    }

    public VerticalAlignment ContentVerticalAlignment
    {
      get => !this.forError ? VerticalAlignment.Bottom : VerticalAlignment.Center;
    }

    public Visibility IconVisibility => (!this.forError).ToVisibility();

    public Orientation ContentPanelOrientation
    {
      get => !this.forError ? Orientation.Horizontal : Orientation.Vertical;
    }

    public Thickness ContentPanelMargin
    {
      get
      {
        return !this.forError ? new Thickness(24.0, 0.0, 0.0, 6.0) : new Thickness(24.0, 4.0, 0.0, 6.0);
      }
    }

    public double ContentFontSize
    {
      get => !this.forError ? 22.0 : Math.Min(Settings.SystemFontSize, 28.0);
    }

    public double MinHeight
    {
      get => UIUtils.SystemTraySizePortrait * ResolutionHelper.ZoomMultiplier + 32.0;
    }

    public double MaxHeight => !this.forError ? this.MinHeight : 144.0;

    public Brush BackgroundBrush
    {
      get => !this.forError ? (Brush) UIUtils.AccentBrush : (Brush) new SolidColorBrush(Colors.Red);
    }

    public RichTextBlock.TextSet HeaderText
    {
      get
      {
        return new RichTextBlock.TextSet()
        {
          Text = this.headerStr ?? ""
        };
      }
    }

    public RichTextBlock.TextSet ContentText
    {
      get
      {
        return new RichTextBlock.TextSet()
        {
          Text = this.contentStr ?? "",
          SerializedFormatting = (IEnumerable<LinkDetector.Result>) this.formattings
        };
      }
    }

    public TextWrapping ContentWrapping => !this.forError ? TextWrapping.NoWrap : TextWrapping.Wrap;

    public InAppFloatingBannerViewModel(string content, bool isError)
    {
      this.contentStr = content;
      this.forError = isError;
      this.formattings = (LinkDetector.Result[]) null;
    }

    public InAppFloatingBannerViewModel(Message m)
    {
      NotificationString.MessageNotificationContent forMessage = NotificationString.GetForMessage(m, Settings.PreviewEnabled);
      this.headerStr = forMessage.Header;
      this.contentStr = forMessage.MessagePreview;
      this.formattings = forMessage.PreviewFormattings;
      this.forError = false;
    }
  }
}
