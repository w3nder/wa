// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.MatrixHelper
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public static class MatrixHelper
  {
    public static void Rotate(ref Matrix matrix, double angle)
    {
      Matrix rotationMatrix = MatrixHelper.CreateRotationMatrix(angle, 0.0, 0.0);
      matrix = MatrixHelper.Multiply(ref matrix, ref rotationMatrix);
    }

    public static void RotateAt(ref Matrix matrix, double angle, Point center)
    {
      Matrix rotationMatrix = MatrixHelper.CreateRotationMatrix(angle, center.X, center.Y);
      matrix = MatrixHelper.Multiply(ref matrix, ref rotationMatrix);
    }

    public static void Translate(ref Matrix matrix, double translateX, double translateY)
    {
      matrix.OffsetX += translateX;
      matrix.OffsetY += translateY;
    }

    private static Matrix Multiply(ref Matrix matrix1, ref Matrix matrix2)
    {
      return new Matrix(matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21, matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22, matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21, matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22, matrix1.OffsetX * matrix2.M11 + matrix1.OffsetY * matrix2.M21 + matrix2.OffsetX, matrix1.OffsetX * matrix2.M12 + matrix1.OffsetY * matrix2.M22 + matrix2.OffsetY);
    }

    private static Matrix CreateRotationMatrix(double degrees, double centerX, double centerY)
    {
      double num1 = degrees * Math.PI / 180.0;
      double m12 = Math.Sin(num1);
      double num2 = Math.Cos(num1);
      double offsetX = centerX * (1.0 - num2) + centerY * m12;
      double offsetY = centerY * (1.0 - num2) - centerX * m12;
      return new Matrix(num2, m12, -m12, num2, offsetX, offsetY);
    }
  }
}
