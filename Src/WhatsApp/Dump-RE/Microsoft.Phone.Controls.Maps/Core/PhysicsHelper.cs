// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.PhysicsHelper
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Windows;
using System.Windows.Media.Animation;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal static class PhysicsHelper
  {
    internal const float FrictionCoefficient = 500f;

    internal static IEasingFunction GetEasingFunction()
    {
      QuadraticEase easingFunction = new QuadraticEase();
      easingFunction.EasingMode = EasingMode.EaseOut;
      return (IEasingFunction) easingFunction;
    }

    internal static Point GetStopPoint(Point velocity, Point initialPosition)
    {
      double stopTime = PhysicsHelper.GetStopTime(velocity);
      double num = Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y);
      Point point = new Point(500.0 * velocity.X / num, 500.0 * velocity.Y / num);
      return new Point(initialPosition.X + stopTime * velocity.X - point.X * stopTime * stopTime / 2.0, initialPosition.Y + stopTime * velocity.Y - point.Y * stopTime * stopTime / 2.0);
    }

    internal static double GetStopTime(Point velocity)
    {
      return Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y) / 500.0;
    }

    internal static int GetAngleFromVelocity(Point velocity)
    {
      double x = velocity.X;
      double y = velocity.Y;
      return x != 0.0 || y != 0.0 ? (x != 0.0 ? (y != 0.0 ? (int) PhysicsHelper.RadianToDegree(Math.Atan2(y, x)) : (x <= 0.0 ? 180 : 0)) : (y <= 0.0 ? 270 : 90)) : 0;
    }

    internal static double DegreeToRadian(double angle) => Math.PI * angle / 180.0;

    internal static double RadianToDegree(double angle) => angle * (180.0 / Math.PI);

    internal static bool ExceedsThreshold(Point delta, long threshold)
    {
      return Math.Abs(delta.X) > (double) threshold || Math.Abs(delta.Y) > (double) threshold;
    }

    internal static bool ExceedsThreshold(TimeSpan delta, long threshold)
    {
      return delta > TimeSpan.FromMilliseconds((double) threshold);
    }

    internal static Point Delta(Point p1, Point p2) => new Point(p1.X - p2.X, p1.Y - p2.Y);

    internal static Point Center(Point p1, Point p2)
    {
      return new Point((p1.X + p2.X) / 2.0, (p1.Y + p2.Y) / 2.0);
    }

    internal static double Distance(Point p1, Point p2)
    {
      Point point = PhysicsHelper.Delta(p1, p2);
      return Math.Sqrt(point.X * point.X + point.Y * point.Y);
    }

    internal static Point NextPoint(Point initial, Point velocity, TimeSpan duration)
    {
      return new Point(initial.X + velocity.X * duration.TotalSeconds, initial.Y + velocity.Y * duration.TotalSeconds);
    }
  }
}
