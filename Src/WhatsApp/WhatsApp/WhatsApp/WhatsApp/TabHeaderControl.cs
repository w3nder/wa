// Decompiled with JetBrains decompiler
// Type: WhatsApp.TabHeaderControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace WhatsApp
{
  public class TabHeaderControl : UserControl
  {
    protected Pivot refPivot;
    protected Rectangle selectedIndicator;
    protected List<TabHeaderControl.BadgeIndicator> badges = new List<TabHeaderControl.BadgeIndicator>();
    private Subject<Unit> tappedChatsPivotSubj = new Subject<Unit>();
    internal Grid LayoutRoot;
    private bool _contentLoaded;

    public TabHeaderControl(Pivot p)
    {
      this.InitializeComponent();
      this.refPivot = p;
      this.Refresh();
    }

    public virtual void Refresh()
    {
      if (this.refPivot == null)
        return;
      this.LayoutRoot.ColumnDefinitions.Clear();
      this.LayoutRoot.Children.Clear();
      this.badges.Clear();
      this.refPivot.SelectionChanged -= new SelectionChangedEventHandler(this.Pivot_SelectionChanged);
      this.refPivot.SelectionChanged += new SelectionChangedEventHandler(this.Pivot_SelectionChanged);
      int count = this.refPivot.Items.Count;
      for (int index = 0; index < count; ++index)
      {
        if (index > 0)
          this.LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = new GridLength(8.0)
          });
        this.LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = new GridLength(1.0, GridUnitType.Star)
        });
        StackPanel stackPanel1 = new StackPanel();
        stackPanel1.Orientation = Orientation.Horizontal;
        stackPanel1.VerticalAlignment = VerticalAlignment.Bottom;
        stackPanel1.HorizontalAlignment = HorizontalAlignment.Center;
        StackPanel stackPanel2 = stackPanel1;
        Grid.SetColumn((FrameworkElement) stackPanel2, index * 2);
        this.LayoutRoot.Children.Add((UIElement) stackPanel2);
        this.badges.Add(this.CreateBadgeIndicator(stackPanel2, ((PivotItem) this.refPivot.Items[index]).Header as string));
        Rectangle rectangle = new Rectangle();
        rectangle.Fill = (Brush) new SolidColorBrush(Colors.Transparent);
        rectangle.Tag = (object) index;
        Rectangle element = rectangle;
        element.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.Element_Tapped);
        Grid.SetColumn((FrameworkElement) element, index * 2);
        this.LayoutRoot.Children.Add((UIElement) element);
      }
      Rectangle rectangle1 = new Rectangle();
      rectangle1.Fill = (Brush) UIUtils.ForegroundBrush;
      rectangle1.Height = 3.0;
      rectangle1.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
      this.selectedIndicator = rectangle1;
      Grid.SetRow((FrameworkElement) this.selectedIndicator, 1);
      this.LayoutRoot.Children.Add((UIElement) this.selectedIndicator);
      this.OnPivotItemSelected(this.refPivot.SelectedIndex);
    }

    protected TabHeaderControl.BadgeIndicator CreateBadgeIndicator(
      StackPanel sp,
      string s,
      bool isPlural = false)
    {
      Brush resource1 = this.Resources[(object) "PhoneAccentBrush"] as Brush;
      Brush brush = (Brush) new SolidColorBrush(Colors.White);
      Brush resource2 = this.Resources[(object) "PhoneForegroundBrush"] as Brush;
      TabHeaderControl.BadgeIndicator badgeIndicator1 = new TabHeaderControl.BadgeIndicator();
      badgeIndicator1.MaxLength = TabHeaderControl.BadgeIndicator.MaxSingleSpanTextLength;
      badgeIndicator1.LabelString = s;
      if (isPlural)
        badgeIndicator1.PluralLabelString = s;
      UIElementCollection children1 = sp.Children;
      TabHeaderControl.BadgeIndicator badgeIndicator2 = badgeIndicator1;
      TextBlock textBlock1 = new TextBlock();
      textBlock1.Text = badgeIndicator1.LabelString;
      textBlock1.FontWeight = FontWeights.Light;
      textBlock1.HorizontalAlignment = HorizontalAlignment.Center;
      textBlock1.VerticalAlignment = VerticalAlignment.Bottom;
      textBlock1.Margin = new Thickness(0.0, 8.0, 0.0, 8.0);
      textBlock1.FontSize = 32.0;
      textBlock1.TextWrapping = TextWrapping.NoWrap;
      textBlock1.Foreground = resource2;
      TextBlock textBlock2 = textBlock1;
      badgeIndicator2.Label = textBlock1;
      TextBlock textBlock3 = textBlock2;
      children1.Add((UIElement) textBlock3);
      UIElementCollection children2 = sp.Children;
      TabHeaderControl.BadgeIndicator badgeIndicator3 = badgeIndicator1;
      Grid grid1 = new Grid();
      grid1.Margin = new Thickness(8.0, 3.0, 0.0, 0.0);
      grid1.VerticalAlignment = VerticalAlignment.Center;
      grid1.MinWidth = 24.0;
      grid1.Height = 24.0;
      grid1.Visibility = Visibility.Collapsed;
      Grid grid2 = grid1;
      badgeIndicator3.Grid = grid1;
      Grid grid3 = grid2;
      children2.Add((UIElement) grid3);
      UIElementCollection children3 = badgeIndicator1.Grid.Children;
      TabHeaderControl.BadgeIndicator badgeIndicator4 = badgeIndicator1;
      Ellipse ellipse1 = new Ellipse();
      ellipse1.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
      ellipse1.Fill = resource1;
      ellipse1.Width = 24.0;
      ellipse1.Height = 24.0;
      Ellipse ellipse2 = ellipse1;
      badgeIndicator4.Ellipse = ellipse1;
      Ellipse ellipse3 = ellipse2;
      children3.Add((UIElement) ellipse3);
      UIElementCollection children4 = badgeIndicator1.Grid.Children;
      TabHeaderControl.BadgeIndicator badgeIndicator5 = badgeIndicator1;
      TextBlock textBlock4 = new TextBlock();
      textBlock4.Margin = new Thickness(4.0, 0.0, 4.0, 0.0);
      textBlock4.TextWrapping = TextWrapping.NoWrap;
      textBlock4.FontSize = 16.0;
      textBlock4.Foreground = brush;
      textBlock4.VerticalAlignment = VerticalAlignment.Center;
      textBlock4.HorizontalAlignment = HorizontalAlignment.Center;
      TextBlock textBlock5 = textBlock4;
      badgeIndicator5.TextBlock = textBlock4;
      TextBlock textBlock6 = textBlock5;
      children4.Add((UIElement) textBlock6);
      return badgeIndicator1;
    }

    public IObservable<Unit> TappedChatsPivotObservable()
    {
      return (IObservable<Unit>) this.tappedChatsPivotSubj;
    }

    public void SetIndicator(int index, bool hasNew)
    {
      this.SetIndicatorImpl(index, hasNew ? 1 : 0, true);
    }

    public void SetCount(int index, int count) => this.SetIndicatorImpl(index, count, false);

    private void SetIndicatorImpl(int index, int count, bool omitCount)
    {
      TabHeaderControl.BadgeIndicator badgeIndicator = this.badges.ElementAtOrDefault<TabHeaderControl.BadgeIndicator>(index);
      if (badgeIndicator == null)
        return;
      if (count > 0)
      {
        badgeIndicator.Grid.Visibility = Visibility.Visible;
        if (omitCount)
        {
          badgeIndicator.Ellipse.Width = badgeIndicator.Ellipse.Height = 12.0;
          badgeIndicator.TextBlock.Text = "";
          badgeIndicator.AlwaysHighlightIndicator = true;
        }
        else
        {
          badgeIndicator.Ellipse.Width = badgeIndicator.Ellipse.Height = 24.0;
          badgeIndicator.TextBlock.Text = count.ToString();
          badgeIndicator.AlwaysHighlightIndicator = false;
        }
        if (badgeIndicator.AlwaysHighlightIndicator)
          badgeIndicator.Ellipse.Fill = (Brush) UIUtils.AccentBrush;
        if (badgeIndicator.PluralLabelString != null)
        {
          badgeIndicator.LabelString = Plurals.Instance.GetString(badgeIndicator.PluralLabelString, count);
          badgeIndicator.Label.Text = badgeIndicator.LabelString;
        }
        if (badgeIndicator.LabelString.Length <= badgeIndicator.MaxLength)
          return;
        int num = badgeIndicator.MaxLength - 2;
        string str = badgeIndicator.LabelString.Substring(0, num);
        int funkynessLength = 0;
        if (Utils.IsFunkyUnicode(str, num, out funkynessLength))
        {
          int length = Math.Min(badgeIndicator.LabelString.Length, num + funkynessLength);
          str = badgeIndicator.LabelString.Substring(0, length);
        }
        badgeIndicator.Label.Text = str + "…";
      }
      else
      {
        badgeIndicator.Grid.Visibility = Visibility.Collapsed;
        badgeIndicator.Label.Text = badgeIndicator.LabelString;
      }
    }

    private void Element_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.refPivot.SelectedIndex = (int) ((FrameworkElement) sender).Tag;
      if (this.refPivot.SelectedIndex != 0)
        return;
      this.tappedChatsPivotSubj.OnNext(new Unit());
    }

    private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems == null || e.AddedItems.Count <= 0)
        return;
      this.OnPivotItemSelected(this.refPivot.Items.IndexOf(e.AddedItems[0]));
    }

    private void OnPivotItemSelected(int index)
    {
      Grid.SetColumn((FrameworkElement) this.selectedIndicator, index * 2);
      Brush resource = this.Resources[(object) "PhoneInactiveBrush"] as Brush;
      for (int index1 = 0; index1 < this.badges.Count; ++index1)
      {
        TabHeaderControl.BadgeIndicator badge = this.badges[index1];
        if (index1 == index || badge.AlwaysHighlightIndicator)
          badge.Ellipse.Fill = (Brush) UIUtils.AccentBrush;
        else
          badge.Ellipse.Fill = resource;
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/TabHeaderControl.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
    }

    protected class BadgeIndicator
    {
      public static int MaxSingleSpanTextLength = 7;
      public TextBlock Label;
      public string LabelString;
      public string PluralLabelString;
      public Grid Grid;
      public TextBlock TextBlock;
      public Ellipse Ellipse;
      public bool AlwaysHighlightIndicator;
      public int MaxLength;
    }
  }
}
