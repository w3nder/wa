// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.Internal.BoundingBox
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using ZXing.Common;

#nullable disable
namespace ZXing.PDF417.Internal
{
  /// <summary>A Bounding Box helper class</summary>
  /// <author>Guenther Grau</author>
  public sealed class BoundingBox
  {
    private readonly BitMatrix image;

    public ResultPoint TopLeft { get; private set; }

    public ResultPoint TopRight { get; private set; }

    public ResultPoint BottomLeft { get; private set; }

    public ResultPoint BottomRight { get; private set; }

    public int MinX { get; private set; }

    public int MaxX { get; private set; }

    public int MinY { get; private set; }

    public int MaxY { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.PDF417.Internal.BoundingBox" /> class.
    /// returns null if the corner points don't match up correctly
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="topLeft">The top left.</param>
    /// <param name="bottomLeft">The bottom left.</param>
    /// <param name="topRight">The top right.</param>
    /// <param name="bottomRight">The bottom right.</param>
    /// <returns></returns>
    public static BoundingBox Create(
      BitMatrix image,
      ResultPoint topLeft,
      ResultPoint bottomLeft,
      ResultPoint topRight,
      ResultPoint bottomRight)
    {
      return topLeft == null && topRight == null || bottomLeft == null && bottomRight == null || topLeft != null && bottomLeft == null || topRight != null && bottomRight == null ? (BoundingBox) null : new BoundingBox(image, topLeft, bottomLeft, topRight, bottomRight);
    }

    /// <summary>Creates the specified box.</summary>
    /// <param name="box">The box.</param>
    /// <returns></returns>
    public static BoundingBox Create(BoundingBox box)
    {
      return new BoundingBox(box.image, box.TopLeft, box.BottomLeft, box.TopRight, box.BottomRight);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.PDF417.Internal.BoundingBox" /> class.
    /// Will throw an exception if the corner points don't match up correctly
    /// </summary>
    /// <param name="image">Image.</param>
    /// <param name="topLeft">Top left.</param>
    /// <param name="topRight">Top right.</param>
    /// <param name="bottomLeft">Bottom left.</param>
    /// <param name="bottomRight">Bottom right.</param>
    private BoundingBox(
      BitMatrix image,
      ResultPoint topLeft,
      ResultPoint bottomLeft,
      ResultPoint topRight,
      ResultPoint bottomRight)
    {
      this.image = image;
      this.TopLeft = topLeft;
      this.TopRight = topRight;
      this.BottomLeft = bottomLeft;
      this.BottomRight = bottomRight;
      this.calculateMinMaxValues();
    }

    /// <summary>
    /// Merge two Bounding Boxes, getting the left corners of left, and the right corners of right
    /// (Images should be the same)
    /// </summary>
    /// <param name="leftBox">Left.</param>
    /// <param name="rightBox">Right.</param>
    internal static BoundingBox merge(BoundingBox leftBox, BoundingBox rightBox)
    {
      if (leftBox == null)
        return rightBox;
      return rightBox == null ? leftBox : new BoundingBox(leftBox.image, leftBox.TopLeft, leftBox.BottomLeft, rightBox.TopRight, rightBox.BottomRight);
    }

    /// <summary>Adds the missing rows.</summary>
    /// <returns>The missing rows.</returns>
    /// <param name="missingStartRows">Missing start rows.</param>
    /// <param name="missingEndRows">Missing end rows.</param>
    /// <param name="isLeft">If set to <c>true</c> is left.</param>
    public BoundingBox addMissingRows(int missingStartRows, int missingEndRows, bool isLeft)
    {
      ResultPoint topLeft = this.TopLeft;
      ResultPoint bottomLeft = this.BottomLeft;
      ResultPoint topRight = this.TopRight;
      ResultPoint bottomRight = this.BottomRight;
      if (missingStartRows > 0)
      {
        ResultPoint resultPoint1 = isLeft ? this.TopLeft : this.TopRight;
        int y = (int) resultPoint1.Y - missingStartRows;
        if (y < 0)
          y = 0;
        ResultPoint resultPoint2 = new ResultPoint(resultPoint1.X, (float) y);
        if (isLeft)
          topLeft = resultPoint2;
        else
          topRight = resultPoint2;
      }
      if (missingEndRows > 0)
      {
        ResultPoint resultPoint3 = isLeft ? this.BottomLeft : this.BottomRight;
        int y = (int) resultPoint3.Y + missingEndRows;
        if (y >= this.image.Height)
          y = this.image.Height - 1;
        ResultPoint resultPoint4 = new ResultPoint(resultPoint3.X, (float) y);
        if (isLeft)
          bottomLeft = resultPoint4;
        else
          bottomRight = resultPoint4;
      }
      this.calculateMinMaxValues();
      return new BoundingBox(this.image, topLeft, bottomLeft, topRight, bottomRight);
    }

    private void calculateMinMaxValues()
    {
      if (this.TopLeft == null)
      {
        this.TopLeft = new ResultPoint(0.0f, this.TopRight.Y);
        this.BottomLeft = new ResultPoint(0.0f, this.BottomRight.Y);
      }
      else if (this.TopRight == null)
      {
        this.TopRight = new ResultPoint((float) (this.image.Width - 1), this.TopLeft.Y);
        this.BottomRight = new ResultPoint((float) (this.image.Width - 1), this.TopLeft.Y);
      }
      this.MinX = (int) Math.Min(this.TopLeft.X, this.BottomLeft.X);
      this.MaxX = (int) Math.Max(this.TopRight.X, this.BottomRight.X);
      this.MinY = (int) Math.Min(this.TopLeft.Y, this.TopRight.Y);
      this.MaxY = (int) Math.Max(this.BottomLeft.Y, this.BottomRight.Y);
    }

    /// <summary>
    /// If we adjust the width, set a new right corner coordinate and recalculate
    /// </summary>
    /// <param name="bottomRight">Bottom right.</param>
    internal void SetBottomRight(ResultPoint bottomRight)
    {
      this.BottomRight = bottomRight;
      this.calculateMinMaxValues();
    }
  }
}
