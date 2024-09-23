// Decompiled with JetBrains decompiler
// Type: WhatsApp.FlowPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Windows;
using System.Windows.Controls;


namespace WhatsApp
{
  public class FlowPanel : Panel
  {
    public static DependencyProperty ColumnCountProperty = DependencyProperty.Register(nameof (ColumnCount), typeof (int), typeof (FlowPanel), new PropertyMetadata((object) 1, (PropertyChangedCallback) ((d, e) => ((FlowPanel) d).OnColumnCountChanged((int) e.OldValue, (int) e.NewValue))));
    public static DependencyProperty RowCountProperty = DependencyProperty.Register(nameof (RowCount), typeof (int), typeof (FlowPanel), new PropertyMetadata((object) 1, (PropertyChangedCallback) ((d, e) => ((FlowPanel) d).OnRowCountChanged((int) e.OldValue, (int) e.NewValue))));

    private void OnColumnCountChanged(int oldValue, int newValue)
    {
      if (oldValue == newValue)
        return;
      this.InvalidateArrange();
    }

    public int ColumnCount
    {
      get => (int) this.GetValue(FlowPanel.ColumnCountProperty);
      set => this.SetValue(FlowPanel.ColumnCountProperty, (object) value);
    }

    private void OnRowCountChanged(int oldValue, int newValue)
    {
      if (oldValue == newValue)
        return;
      this.InvalidateArrange();
    }

    public int RowCount
    {
      get => (int) this.GetValue(FlowPanel.RowCountProperty);
      set => this.SetValue(FlowPanel.RowCountProperty, (object) value);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      double width = finalSize.Width / (double) this.ColumnCount;
      double height = finalSize.Height / (double) this.RowCount;
      int num1 = this.RowCount - 1;
      int num2 = this.ColumnCount - 1;
      bool flag = false;
      for (int index = 0; index < this.Children.Count; ++index)
      {
        this.Children[index].Arrange(new Rect((flag ? (double) (this.ColumnCount - 1 - num2) : (double) num2) * width, (double) num1 * height, width, height));
        if (this.Children[index].Visibility == Visibility.Visible)
        {
          --num2;
          if (num2 < 0)
          {
            num2 = this.ColumnCount - 1;
            --num1;
            flag = !flag;
          }
        }
      }
      return base.ArrangeOverride(finalSize);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      Size availableSize1 = new Size(availableSize.Width / (double) this.ColumnCount, availableSize.Height / (double) this.RowCount);
      foreach (UIElement child in (PresentationFrameworkCollection<UIElement>) this.Children)
        child.Measure(availableSize1);
      return base.MeasureOverride(availableSize);
    }
  }
}
