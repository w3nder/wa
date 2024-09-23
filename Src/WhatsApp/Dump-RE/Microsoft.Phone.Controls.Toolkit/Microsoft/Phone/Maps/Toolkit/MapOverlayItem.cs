// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Maps.Toolkit.MapOverlayItem
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Maps.Controls;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Phone.Maps.Toolkit
{
  internal class MapOverlayItem : ContentPresenter
  {
    public MapOverlayItem(object content, DataTemplate contentTemplate, MapOverlay mapOverlay)
    {
      this.ContentTemplate = contentTemplate;
      this.Content = content;
      this.MapOverlay = mapOverlay;
    }

    private MapOverlay MapOverlay { get; set; }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      MapChild.BindMapOverlayProperties(this.MapOverlay);
    }
  }
}
