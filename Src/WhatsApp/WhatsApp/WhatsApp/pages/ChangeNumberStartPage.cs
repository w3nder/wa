// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChangeNumberStartPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;


namespace WhatsApp
{
  public class ChangeNumberStartPage : PhoneApplicationPage
  {
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal ColorIcon PhonePic1;
    internal ColorIcon PhonePic2;
    internal TextBlock ExplanationTextBlock1;
    internal TextBlock ExplanationTextBlock2;
    private bool _contentLoaded;

    public ChangeNumberStartPage()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.PhonePic1.Source = this.PhonePic2.Source = (System.Windows.Media.ImageSource) AssetStore.LoadAsset("change-num-phone.png", AssetStore.ThemeSetting.NoTheme);
    }

    private void Continue_Click(object sender, EventArgs e)
    {
      this.NavigationService.Navigate(UriUtils.CreatePageUri("ChangeNumberEntryPage"));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ChangeNumberStartPage.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.PhonePic1 = (ColorIcon) this.FindName("PhonePic1");
      this.PhonePic2 = (ColorIcon) this.FindName("PhonePic2");
      this.ExplanationTextBlock1 = (TextBlock) this.FindName("ExplanationTextBlock1");
      this.ExplanationTextBlock2 = (TextBlock) this.FindName("ExplanationTextBlock2");
    }
  }
}
