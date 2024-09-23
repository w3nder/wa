// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.Binding.TextBinding
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Coding4Fun.Phone.Controls.Binding
{
  public class TextBinding
  {
    public static readonly DependencyProperty UpdateSourceOnChangeProperty = DependencyProperty.RegisterAttached("UpdateSourceOnChange", typeof (bool), typeof (TextBinding), new PropertyMetadata((object) false, new PropertyChangedCallback(TextBinding.OnUpdateSourceOnChangePropertyChanged)));

    public static bool GetUpdateSourceOnChange(DependencyObject obj)
    {
      return (bool) obj.GetValue(TextBinding.UpdateSourceOnChangeProperty);
    }

    public static void SetUpdateSourceOnChange(DependencyObject obj, bool value)
    {
      obj.SetValue(TextBinding.UpdateSourceOnChangeProperty, (object) value);
    }

    private static void OnUpdateSourceOnChangePropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      if (e.NewValue == e.OldValue)
        return;
      TextBinding.HandleUpdateSourceOnChangeEventAppend((object) obj, (bool) e.NewValue);
    }

    private static void HandleUpdateSourceOnChangeEventAppend(object sender, bool value)
    {
      switch (sender)
      {
        case TextBox _:
          TextBinding.HandleUpdateSourceOnChangeEventAppendTextBox(sender, value);
          break;
        case PasswordBox _:
          TextBinding.HandleUpdateSourceOnChangeEventAppendPassword(sender, value);
          break;
      }
    }

    private static void HandleUpdateSourceOnChangeEventAppendTextBox(object sender, bool value)
    {
      if (!(sender is TextBox textBox))
        return;
      if (value)
        textBox.TextChanged += new TextChangedEventHandler(TextBinding.UpdateSourceOnChangePropertyChanged);
      else
        textBox.TextChanged -= new TextChangedEventHandler(TextBinding.UpdateSourceOnChangePropertyChanged);
    }

    private static void HandleUpdateSourceOnChangeEventAppendPassword(object sender, bool value)
    {
      if (!(sender is PasswordBox passwordBox))
        return;
      if (value)
        passwordBox.PasswordChanged += new RoutedEventHandler(TextBinding.UpdateSourceOnChangePropertyChanged);
      else
        passwordBox.PasswordChanged -= new RoutedEventHandler(TextBinding.UpdateSourceOnChangePropertyChanged);
    }

    private static void UpdateSourceOnChangePropertyChanged(object sender, RoutedEventArgs e)
    {
      DependencyProperty dependancyPropertyForText = TextBinding.GetDependancyPropertyForText(sender);
      if (dependancyPropertyForText == null)
        return;
      ((FrameworkElement) sender).GetBindingExpression(dependancyPropertyForText)?.UpdateSource();
    }

    private static DependencyProperty GetDependancyPropertyForText(object sender)
    {
      DependencyProperty dependancyPropertyForText = (DependencyProperty) null;
      switch (sender)
      {
        case TextBox _:
          dependancyPropertyForText = TextBox.TextProperty;
          break;
        case PasswordBox _:
          dependancyPropertyForText = PasswordBox.PasswordProperty;
          break;
      }
      return dependancyPropertyForText;
    }
  }
}
