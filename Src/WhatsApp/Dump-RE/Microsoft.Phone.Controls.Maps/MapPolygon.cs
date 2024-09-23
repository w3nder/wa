// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.MapPolygon
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
  public class MapPolygon : MapShapeBase
  {
    public MapPolygon()
    {
      this.DefaultStyleKey = (object) typeof (MapPolygon);
      this.EmbeddedShape = (Shape) new Polygon();
    }

    protected override PointCollection ProjectedPoints
    {
      get => ((Polygon) this.EmbeddedShape).Points;
      set
      {
        if (this.EmbeddedShape == null)
          return;
        ((Polygon) this.EmbeddedShape).Points = value;
      }
    }

    public FillRule FillRule
    {
      get => ((Polygon) this.EmbeddedShape).FillRule;
      set
      {
        if (this.EmbeddedShape == null)
          return;
        ((Polygon) this.EmbeddedShape).FillRule = value;
      }
    }

    protected override void SetEmbeddedShape(Shape newShape)
    {
      if (this.EmbeddedShape != null)
        ((Polygon) newShape).FillRule = ((Polygon) this.EmbeddedShape).FillRule;
      base.SetEmbeddedShape(newShape);
    }
  }
}
