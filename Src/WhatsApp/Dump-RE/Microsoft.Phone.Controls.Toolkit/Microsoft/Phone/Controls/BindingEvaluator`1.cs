// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.BindingEvaluator`1
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System.Windows;
using System.Windows.Data;

#nullable disable
namespace Microsoft.Phone.Controls
{
  internal class BindingEvaluator<T> : FrameworkElement
  {
    private Binding _binding;
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof (Value), typeof (T), typeof (BindingEvaluator<T>), new PropertyMetadata((object) default (T)));

    public T Value
    {
      get => (T) this.GetValue(BindingEvaluator<T>.ValueProperty);
      set => this.SetValue(BindingEvaluator<T>.ValueProperty, (object) value);
    }

    public Binding ValueBinding
    {
      get => this._binding;
      set
      {
        this._binding = value;
        this.SetBinding(BindingEvaluator<T>.ValueProperty, this._binding);
      }
    }

    public BindingEvaluator()
    {
    }

    public BindingEvaluator(Binding binding)
    {
      this.SetBinding(BindingEvaluator<T>.ValueProperty, binding);
    }

    public void ClearDataContext() => this.DataContext = (object) null;

    public T GetDynamicValue(object o, bool clearDataContext)
    {
      this.DataContext = o;
      T dynamicValue = this.Value;
      if (clearDataContext)
        this.DataContext = (object) null;
      return dynamicValue;
    }

    public T GetDynamicValue(object o)
    {
      this.DataContext = o;
      return this.Value;
    }
  }
}
