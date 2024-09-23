// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Overlays.Copyright
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.AutomationPeers;
using Microsoft.Phone.Controls.Maps.Core;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Overlays
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [TemplatePart(Name = "CopyrightContainer", Type = typeof (ShadowText))]
  public class Copyright : Overlay
  {
    internal const string CopyrightContainerElementName = "CopyrightContainer";
    private const int maxLine = 60;
    private const string spacesBetween = "   ";
    private const char nonBreakingSpace = ' ';
    private ObservableCollection<AttributionInfo> attributions;
    private ShadowText container;
    private ModeBackground setForBackground;

    public Copyright()
    {
      this.DefaultStyleKey = (object) typeof (Copyright);
      this.container = new ShadowText();
      this.Attributions = new ObservableCollection<AttributionInfo>();
    }

    public ObservableCollection<AttributionInfo> Attributions
    {
      get => this.attributions;
      internal set
      {
        if (this.attributions != null)
          this.attributions.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.Attributions_CollectionChanged);
        if (value == null)
          return;
        this.attributions = value;
        this.attributions.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Attributions_CollectionChanged);
      }
    }

    internal ModeBackground SetForBackground
    {
      get => this.setForBackground;
      set
      {
        this.setForBackground = value;
        if (this.setForBackground == ModeBackground.Light)
          this.container.SetForegroundColorsForLightBackground();
        else
          this.container.SetForegroundColorsForDarkBackground();
      }
    }

    private ShadowText Container
    {
      get => this.container;
      set
      {
        if (value == null)
          return;
        this.SetContainer(value);
      }
    }

    private void SetContainer(ShadowText newContainer)
    {
      if (this.container != null)
        newContainer.Text = this.container.Text;
      this.container = newContainer;
    }

    private void Attributions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.OnAttributionsChanged(e.OldItems, e.NewItems);
    }

    private void OnAttributionsChanged(IList oldItems, IList newItems)
    {
      int num = 0;
      StringBuilder sb = new StringBuilder();
      foreach (AttributionInfo attribution in (Collection<AttributionInfo>) this.Attributions)
      {
        if (num > 0)
        {
          if (num + attribution.Text.Length > 60)
          {
            sb.AppendLine();
            num = 0;
          }
          else
          {
            sb.Append("   ");
            num += "   ".Length;
          }
        }
        Copyright.AddNonBreakingString(sb, attribution.Text);
        num += attribution.Text.Length;
      }
      this.container.Text = sb.ToString();
    }

    private static void AddNonBreakingString(StringBuilder sb, string s)
    {
      for (int index = 0; index < s.Length; ++index)
      {
        char ch = s[index];
        if (' ' == ch)
          ch = ' ';
        sb.Append(ch);
      }
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new BaseAutomationPeer((FrameworkElement) this, nameof (Copyright));
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.Container = this.GetTemplateChild("CopyrightContainer") as ShadowText;
      this.FireTemplateApplied();
    }
  }
}
