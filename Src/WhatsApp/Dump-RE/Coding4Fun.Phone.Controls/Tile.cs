// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.Tile
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  [ContentProperty("Content")]
  public class Tile : Button
  {
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof (Title), typeof (string), typeof (Tile), new PropertyMetadata((object) ""));
    public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(nameof (TextWrapping), typeof (TextWrapping), typeof (Tile), new PropertyMetadata((object) (TextWrapping) 1));

    public Tile() => ((Control) this).DefaultStyleKey = (object) typeof (Tile);

    public string Title
    {
      get => (string) ((DependencyObject) this).GetValue(Tile.TitleProperty);
      set => ((DependencyObject) this).SetValue(Tile.TitleProperty, (object) value);
    }

    public TextWrapping TextWrapping
    {
      get => (TextWrapping) ((DependencyObject) this).GetValue(Tile.TextWrappingProperty);
      set => ((DependencyObject) this).SetValue(Tile.TextWrappingProperty, (object) value);
    }
  }
}
