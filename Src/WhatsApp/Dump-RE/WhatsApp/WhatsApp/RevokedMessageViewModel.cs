// Decompiled with JetBrains decompiler
// Type: WhatsApp.RevokedMessageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class RevokedMessageViewModel : MessageViewModel
  {
    public override Thickness ViewPanelMargin
    {
      get
      {
        double num = 12.0 * this.zoomMultiplier;
        return new Thickness(num, num, num, num * 3.0);
      }
    }

    public override HorizontalAlignment HorizontalAlignment => HorizontalAlignment.Stretch;

    public override double MaxBubbleWidth => double.PositiveInfinity;

    public RevokedMessageViewModel(Message m)
      : base(m)
    {
      this.ExcludedMenuItems = new Set<MessageMenu.MessageMenuItem>((IEnumerable<MessageMenu.MessageMenuItem>) new MessageMenu.MessageMenuItem[5]
      {
        MessageMenu.MessageMenuItem.Star,
        MessageMenu.MessageMenuItem.Copy,
        MessageMenu.MessageMenuItem.Forward,
        MessageMenu.MessageMenuItem.Reply,
        MessageMenu.MessageMenuItem.ReplyInPrivate
      });
    }

    protected override string GetTextStr()
    {
      return !this.Message.KeyFromMe ? AppResources.ReceivedDeleted : AppResources.SentDeleted;
    }

    public BitmapSource Icon => AssetStore.RevokedMessage;
  }
}
