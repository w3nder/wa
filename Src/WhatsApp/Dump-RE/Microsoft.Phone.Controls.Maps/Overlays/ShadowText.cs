// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Overlays.ShadowText
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.AutomationPeers;
using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Overlays
{
  [TemplatePart(Name = "Text1", Type = typeof (TextBlock))]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [TemplatePart(Name = "Text2", Type = typeof (TextBlock))]
  public class ShadowText : Overlay
  {
    internal const string Text1ElementName = "Text1";
    internal const string Text2ElementName = "Text2";
    private TextBlock text1;
    private TextBlock text2;
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof (Text), typeof (string), typeof (ShadowText), new PropertyMetadata(new PropertyChangedCallback(ShadowText.OnTextChanged)));
    public static readonly DependencyProperty ForegroundTopProperty = DependencyProperty.Register(nameof (ForegroundTop), typeof (Brush), typeof (ShadowText), new PropertyMetadata(new PropertyChangedCallback(ShadowText.OnTextChanged)));
    public static readonly DependencyProperty ForegroundBottomProperty = DependencyProperty.Register(nameof (ForegroundBottom), typeof (Brush), typeof (ShadowText), new PropertyMetadata(new PropertyChangedCallback(ShadowText.OnTextChanged)));
    private readonly Brush Black = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 0, (byte) 0, (byte) 0));
    private readonly Brush White = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue));

    public ShadowText()
    {
      this.DefaultStyleKey = (object) typeof (ShadowText);
      this.text1 = new TextBlock();
      this.text2 = new TextBlock();
    }

    public string Text
    {
      get => (string) this.GetValue(ShadowText.TextProperty);
      set => this.SetValue(ShadowText.TextProperty, (object) value);
    }

    public Brush ForegroundTop
    {
      get => (Brush) this.GetValue(ShadowText.ForegroundTopProperty);
      set => this.SetValue(ShadowText.ForegroundTopProperty, (object) value);
    }

    public Brush ForegroundBottom
    {
      get => (Brush) this.GetValue(ShadowText.ForegroundBottomProperty);
      set => this.SetValue(ShadowText.ForegroundBottomProperty, (object) value);
    }

    public void SetForegroundColorsForDarkBackground()
    {
      this.SetForegroundColors(this.White, this.Black);
    }

    public void SetForegroundColorsForLightBackground()
    {
      this.SetForegroundColors(this.Black, this.White);
    }

    public void SetForegroundColors(Brush top, Brush bottom)
    {
      this.ForegroundTop = top;
      this.ForegroundBottom = bottom;
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ShadowText) d).OnTextChanged();
    }

    private void OnTextChanged()
    {
      this.text1.Text = this.Text;
      this.text2.Text = this.Text;
      if (this.ForegroundTop == null || this.ForegroundBottom == null)
        return;
      this.text1.Foreground = this.ForegroundBottom;
      this.text2.Foreground = this.ForegroundTop;
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new BaseAutomationPeer((FrameworkElement) this, nameof (ShadowText));
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.text1 = this.GetTemplateChild("Text1") as TextBlock;
      this.text2 = this.GetTemplateChild("Text2") as TextBlock;
      this.OnTextChanged();
      this.FireTemplateApplied();
    }
  }
}
