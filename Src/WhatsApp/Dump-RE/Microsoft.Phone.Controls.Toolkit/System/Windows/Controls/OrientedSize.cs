// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.OrientedSize
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

#nullable disable
namespace System.Windows.Controls
{
  internal struct OrientedSize
  {
    private Orientation _orientation;
    private double _direct;
    private double _indirect;

    public Orientation Orientation => this._orientation;

    public double Direct
    {
      get => this._direct;
      set => this._direct = value;
    }

    public double Indirect
    {
      get => this._indirect;
      set => this._indirect = value;
    }

    public double Width
    {
      get => this.Orientation != Orientation.Horizontal ? this.Indirect : this.Direct;
      set
      {
        if (this.Orientation == Orientation.Horizontal)
          this.Direct = value;
        else
          this.Indirect = value;
      }
    }

    public double Height
    {
      get => this.Orientation == Orientation.Horizontal ? this.Indirect : this.Direct;
      set
      {
        if (this.Orientation != Orientation.Horizontal)
          this.Direct = value;
        else
          this.Indirect = value;
      }
    }

    public OrientedSize(Orientation orientation)
      : this(orientation, 0.0, 0.0)
    {
    }

    public OrientedSize(Orientation orientation, double width, double height)
    {
      this._orientation = orientation;
      this._direct = 0.0;
      this._indirect = 0.0;
      this.Width = width;
      this.Height = height;
    }
  }
}
