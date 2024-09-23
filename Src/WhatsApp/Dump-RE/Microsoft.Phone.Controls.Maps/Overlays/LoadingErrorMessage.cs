// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Overlays.LoadingErrorMessage
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.AutomationPeers;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Overlays
{
  [TemplatePart(Name = "ErrorMessage", Type = typeof (TextBlock))]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class LoadingErrorMessage : Overlay
  {
    internal const string ErrorMessageElementName = "ErrorMessage";
    private TextBlock container;

    public LoadingErrorMessage()
    {
      this.DefaultStyleKey = (object) typeof (LoadingErrorMessage);
      this.container = new TextBlock();
      this.container.TextWrapping = TextWrapping.Wrap;
    }

    private TextBlock Container
    {
      get => this.container;
      set
      {
        if (value == null)
          return;
        this.SetSetContainer(value);
      }
    }

    public void SetConfigurationError(CultureInfo culture)
    {
      OverlayResources.Culture = culture;
      this.container.Text = string.Format((IFormatProvider) culture, OverlayResources.LoadingConfigurationErrorMessage);
    }

    public void SetUriSchemeError(CultureInfo culture)
    {
      OverlayResources.Culture = culture;
      this.container.Text = string.Format((IFormatProvider) culture, OverlayResources.LoadingUriSchemeErrorMessage);
    }

    public void SetCredentialsError(CultureInfo culture)
    {
      OverlayResources.Culture = culture;
      this.container.Text = string.Format((IFormatProvider) culture, OverlayResources.InvalidCredentialsErrorMessage);
    }

    private void SetSetContainer(TextBlock newContainer)
    {
      if (this.container != null)
        newContainer.Text = this.container.Text;
      this.container = newContainer;
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new BaseAutomationPeer((FrameworkElement) this, nameof (LoadingErrorMessage));
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.Container = this.GetTemplateChild("ErrorMessage") as TextBlock;
      this.FireTemplateApplied();
    }
  }
}
