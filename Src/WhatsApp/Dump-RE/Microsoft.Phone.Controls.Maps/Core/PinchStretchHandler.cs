// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.PinchStretchHandler
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Linq;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class PinchStretchHandler
  {
    private bool inProgress;
    private Point[] lastKnownPoints = new Point[2];
    private ushort minimumUpdateThreshold = 3;
    private Point[] trackingPoints = new Point[2];
    private UIElement root;
    private bool isEnabled;

    public PinchStretchHandler(UIElement element)
    {
      this.root = element != null ? element : throw new ArgumentNullException(nameof (element));
    }

    public void Enable(bool enable)
    {
      if (enable && !this.isEnabled)
      {
        TouchHelper.AddHandlers(this.root, new TouchHandlers()
        {
          TouchDown = (EventHandler<TouchEventArgs>) ((o, e) => e.TouchPoint.TouchDevice.Capture(this.root)),
          CapturedTouchUp = (EventHandler<TouchEventArgs>) ((o, e) => this.Reset()),
          CapturedTouchReported = new EventHandler<TouchReportedEventArgs>(this.OnCapturedTouchReported)
        });
        this.isEnabled = true;
      }
      else
      {
        if (enable || !this.isEnabled)
          return;
        TouchHelper.RemoveHandlers(this.root);
        this.isEnabled = false;
      }
    }

    public event EventHandler<PinchStretchEventArgs> PinchStretchReported;

    private void Reset()
    {
      this.inProgress = false;
      this.trackingPoints = new Point[2];
      this.lastKnownPoints = new Point[2];
    }

    private void OnCapturedTouchReported(object sender, TouchReportedEventArgs e)
    {
      if (e.TouchPoints.Count<Point>() != 2)
        return;
      this.RecordPoints(e.TouchPoints.ElementAt<Point>(0), e.TouchPoints.ElementAt<Point>(1));
      this.inProgress = true;
    }

    private void RecordPoints(Point p1, Point p2)
    {
      this.trackingPoints[0] = p1;
      this.trackingPoints[1] = p2;
      if (!this.inProgress)
      {
        this.lastKnownPoints[0] = p1;
        this.lastKnownPoints[1] = p2;
      }
      Point delta1 = PhysicsHelper.Delta(this.trackingPoints[0], this.lastKnownPoints[0]);
      Point delta2 = PhysicsHelper.Delta(this.trackingPoints[1], this.lastKnownPoints[1]);
      if (this.inProgress && !PhysicsHelper.ExceedsThreshold(delta1, (long) this.minimumUpdateThreshold) && !PhysicsHelper.ExceedsThreshold(delta2, (long) this.minimumUpdateThreshold))
        return;
      PinchStretchData data = new PinchStretchData()
      {
        ContactPoint1 = this.trackingPoints[0],
        ContactPoint2 = this.trackingPoints[1],
        ContactPoint1Delta = delta1,
        ContactPoint2Delta = delta2
      };
      Point p2_1 = PhysicsHelper.Center(this.lastKnownPoints[0], this.lastKnownPoints[1]);
      Point p1_1 = PhysicsHelper.Center(this.trackingPoints[0], this.trackingPoints[1]);
      data.CenterPoint = p1_1;
      data.CenterPointDelta = PhysicsHelper.Delta(p1_1, p2_1);
      double num1 = PhysicsHelper.Distance(this.lastKnownPoints[0], this.lastKnownPoints[1]);
      double num2 = PhysicsHelper.Distance(this.trackingPoints[0], this.trackingPoints[1]);
      data.Scale = num2 / num1;
      this.lastKnownPoints[0] = this.trackingPoints[0];
      this.lastKnownPoints[1] = this.trackingPoints[1];
      this.FirePinchStretchReported(data);
    }

    private void FirePinchStretchReported(PinchStretchData data)
    {
      EventHandler<PinchStretchEventArgs> pinchStretchReported = this.PinchStretchReported;
      if (pinchStretchReported == null)
        return;
      pinchStretchReported((object) this, new PinchStretchEventArgs(data));
    }
  }
}
