// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.MapPolyline
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.Core;
using System;
using System.Windows.Media;
using System.Windows.Shapes;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class MapPolyline : MapShapeBase
  {
    public MapPolyline()
    {
      this.DefaultStyleKey = (object) typeof (MapPolyline);
      this.EmbeddedShape = (Shape) new Polyline();
    }

    protected override PointCollection ProjectedPoints
    {
      get => ((Polyline) this.EmbeddedShape).Points;
      set
      {
        if (this.EmbeddedShape == null)
          return;
        ((Polyline) this.EmbeddedShape).Points = value;
      }
    }

    public FillRule FillRule
    {
      get => ((Polyline) this.EmbeddedShape).FillRule;
      set
      {
        if (this.EmbeddedShape == null)
          return;
        ((Polyline) this.EmbeddedShape).FillRule = value;
      }
    }

    protected override void SetEmbeddedShape(Shape newShape)
    {
      if (this.EmbeddedShape != null)
        ((Polyline) newShape).FillRule = ((Polyline) this.EmbeddedShape).FillRule;
      base.SetEmbeddedShape(newShape);
    }
  }
}
