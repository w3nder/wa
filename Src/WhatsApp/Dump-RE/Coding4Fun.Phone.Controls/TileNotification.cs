// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.TileNotification
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  [System.Windows.Markup.ContentProperty("Content")]
  public class TileNotification : Control
  {
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof (Content), typeof (object), typeof (TileNotification), new PropertyMetadata((PropertyChangedCallback) null));

    public TileNotification() => this.DefaultStyleKey = (object) typeof (TileNotification);

    public object Content
    {
      get => ((DependencyObject) this).GetValue(TileNotification.ContentProperty);
      set => ((DependencyObject) this).SetValue(TileNotification.ContentProperty, value);
    }
  }
}
