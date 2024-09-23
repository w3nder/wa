// Decompiled with JetBrains decompiler
// Type: WhatsApp.StickerRow
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#nullable disable
namespace WhatsApp
{
  public class StickerRow : UserControl
  {
    public static readonly DependencyProperty ItemsContextProperty = DependencyProperty.Register(nameof (ItemsContext), typeof (object), typeof (StickerRow), new PropertyMetadata((PropertyChangedCallback) ((dep, e) =>
    {
      if (!(dep is StickerRow stickerRow2))
        return;
      stickerRow2.ItemsContextChanged(e.NewValue);
    })));
    internal Grid LayoutRoot;
    private bool _contentLoaded;

    public StickerRow() => this.InitializeComponent();

    public object ItemsContext
    {
      get => this.GetValue(StickerRow.ItemsContextProperty);
      set => this.SetValue(StickerRow.ItemsContextProperty, value);
    }

    public void ItemsContextChanged(object newContext)
    {
      this.InitializeStickers((StickerRowContext) newContext);
    }

    public void InitializeStickers(StickerRowContext context)
    {
    }

    private void Sticker_Tap(object sender, GestureEventArgs e)
    {
      throw new NotImplementedException();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/StickerRow.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
    }
  }
}
