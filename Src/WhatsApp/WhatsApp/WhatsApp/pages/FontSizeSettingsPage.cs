// Decompiled with JetBrains decompiler
// Type: WhatsApp.FontSizeSettingsPage
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
  public class FontSizeSettingsPage : PhoneApplicationPage
  {
    internal StackPanel LayoutRoot;
    internal SteppedSliderControl Slider;
    internal TextBlock ExplanationText;
    internal TextBlock SystemExplanationText;
    private bool _contentLoaded;

    public FontSizeSettingsPage() => this.InitializeComponent();

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/Settings/FontSizeSettingsPage.xaml", UriKind.Relative));
      this.LayoutRoot = (StackPanel) this.FindName("LayoutRoot");
      this.Slider = (SteppedSliderControl) this.FindName("Slider");
      this.ExplanationText = (TextBlock) this.FindName("ExplanationText");
      this.SystemExplanationText = (TextBlock) this.FindName("SystemExplanationText");
    }
  }
}
