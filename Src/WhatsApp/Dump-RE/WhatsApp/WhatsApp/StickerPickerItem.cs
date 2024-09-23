// Decompiled with JetBrains decompiler
// Type: WhatsApp.StickerPickerItem
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class StickerPickerItem : UserControl
  {
    internal Grid LayoutRoot;
    private bool _contentLoaded;

    public StickerPickerItem() => this.InitializeComponent();

    private void StickerItem_Tap(object sender, GestureEventArgs gestureEventArgs)
    {
      if (!((sender is Grid grid ? grid.DataContext : (object) null) is StickerPickerItemViewModel dataContext))
        return;
      EmojiKeyboard.ActionSubject.OnNext(new Pair<EmojiKeyboard.Actions, object>(EmojiKeyboard.Actions.ActionedSticker, (object) dataContext.Sticker));
      if (dataContext == null)
        return;
      dataContext.TappedAction();
    }

    private void StickerItem_ManipulationStarted(
      object sender,
      ManipulationStartedEventArgs manipulationStartedEventArgs)
    {
      if (!((sender is Grid grid ? grid.DataContext : (object) null) is StickerPickerItemViewModel dataContext))
        return;
      dataContext.IsSelected = true;
    }

    private void StickerItem_ManipulationCompleted(
      object sender,
      ManipulationCompletedEventArgs manipulationCompletedEventArgs)
    {
      if (!((sender is Grid grid ? grid.DataContext : (object) null) is StickerPickerItemViewModel dataContext))
        return;
      dataContext.IsSelected = false;
    }

    private void StickerItem_Hold(object sender, GestureEventArgs e)
    {
      if (!((sender is Grid grid ? grid.DataContext : (object) null) is StickerPickerItemViewModel dataContext))
        return;
      Action holdAction = dataContext.HoldAction;
      if (holdAction == null)
        return;
      holdAction();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/StickerPickerItem.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
    }
  }
}
