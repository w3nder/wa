// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.DatePickerPage
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class DatePickerPage : DateTimePickerPageBase
  {
    internal VisualStateGroup VisibilityStates;
    internal VisualState Open;
    internal VisualState Closed;
    internal PlaneProjection PlaneProjection;
    internal Rectangle SystemTrayPlaceholder;
    internal TextBlock HeaderTitle;
    internal LoopingSelector SecondarySelector;
    internal LoopingSelector TertiarySelector;
    internal LoopingSelector PrimarySelector;
    private bool _contentLoaded;

    public DatePickerPage()
    {
      this.InitializeComponent();
      this.PrimarySelector.DataSource = (ILoopingSelectorDataSource) new YearDataSource();
      this.SecondarySelector.DataSource = (ILoopingSelectorDataSource) new MonthDataSource();
      this.TertiarySelector.DataSource = (ILoopingSelectorDataSource) new DayDataSource();
      this.InitializeDateTimePickerPage(this.PrimarySelector, this.SecondarySelector, this.TertiarySelector);
    }

    protected override IEnumerable<LoopingSelector> GetSelectorsOrderedByCulturePattern()
    {
      string pattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.ToUpperInvariant();
      if (DateTimePickerBase.DateShouldFlowRTL())
      {
        char[] charArray = pattern.ToCharArray();
        Array.Reverse((Array) charArray);
        pattern = new string(charArray);
      }
      return DateTimePickerPageBase.GetSelectorsOrderedByCulturePattern(pattern, new char[3]
      {
        'Y',
        'M',
        'D'
      }, new LoopingSelector[3]
      {
        this.PrimarySelector,
        this.SecondarySelector,
        this.TertiarySelector
      });
    }

    protected override void OnOrientationChanged(OrientationChangedEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException(nameof (e));
      base.OnOrientationChanged(e);
      this.SystemTrayPlaceholder.Visibility = (PageOrientation.Portrait & e.Orientation) != PageOrientation.None ? Visibility.Visible : Visibility.Collapsed;
    }

    public override void SetFlowDirection(FlowDirection flowDirection)
    {
      this.HeaderTitle.FlowDirection = flowDirection;
      this.PrimarySelector.FlowDirection = flowDirection;
      this.SecondarySelector.FlowDirection = flowDirection;
      this.TertiarySelector.FlowDirection = flowDirection;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Phone.Controls.Toolkit;component/DateTimePickers/DatePickerPage.xaml", UriKind.Relative));
      this.VisibilityStates = (VisualStateGroup) this.FindName("VisibilityStates");
      this.Open = (VisualState) this.FindName("Open");
      this.Closed = (VisualState) this.FindName("Closed");
      this.PlaneProjection = (PlaneProjection) this.FindName("PlaneProjection");
      this.SystemTrayPlaceholder = (Rectangle) this.FindName("SystemTrayPlaceholder");
      this.HeaderTitle = (TextBlock) this.FindName("HeaderTitle");
      this.SecondarySelector = (LoopingSelector) this.FindName("SecondarySelector");
      this.TertiarySelector = (LoopingSelector) this.FindName("TertiarySelector");
      this.PrimarySelector = (LoopingSelector) this.FindName("PrimarySelector");
    }
  }
}
