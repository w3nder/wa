// Decompiled with JetBrains decompiler
// Type: WhatsApp.WallpaperPreviewForeground
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace WhatsApp
{
  public class WallpaperPreviewForeground : Grid
  {
    public PageTitlePanel TitleBlock { get; private set; }

    public StackPanel MessagesPanel { get; private set; }

    private MessageBubbleContainer MessageBubble0 { get; set; }

    private MessageBubbleContainer MessageBubble1 { get; set; }

    public WallpaperPreviewForeground()
    {
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Auto)
      });
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Star)
      });
      PageTitlePanel pageTitlePanel = new PageTitlePanel();
      pageTitlePanel.RenderTransform = (Transform) new CompositeTransform();
      pageTitlePanel.TextBrush = (Brush) UIUtils.WhiteBrush;
      pageTitlePanel.Mode = PageTitlePanel.Modes.Zoomed;
      this.TitleBlock = pageTitlePanel;
      StackPanel stackPanel = new StackPanel();
      stackPanel.RenderTransform = (Transform) new CompositeTransform();
      stackPanel.Margin = new Thickness(0.0, 12.0 * ResolutionHelper.ZoomMultiplier, 0.0, 0.0);
      this.MessagesPanel = stackPanel;
      this.MessageBubble0 = new MessageBubbleContainer()
      {
        IsSelectable = false
      };
      this.MessageBubble1 = new MessageBubbleContainer()
      {
        IsSelectable = false
      };
      this.MessagesPanel.Children.Add((UIElement) this.MessageBubble0);
      this.MessagesPanel.Children.Add((UIElement) this.MessageBubble1);
      Grid.SetRow((FrameworkElement) this.TitleBlock, 0);
      this.Children.Add((UIElement) this.TitleBlock);
      Grid.SetRow((FrameworkElement) this.MessagesPanel, 1);
      this.Children.Add((UIElement) this.MessagesPanel);
    }

    public void Update(WallpaperStore.WallpaperState wallpaper)
    {
      if (wallpaper == null)
        return;
      this.TitleBlock.TextBrush = (Brush) wallpaper.PreferredForegroundBrush;
    }

    public void SetTexts(string title, string leftText, string rightText)
    {
      this.TitleBlock.SmallTitle = title;
      DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
      Message msg1 = new Message()
      {
        MediaWaType = FunXMPP.FMessage.Type.Undefined,
        FunTimestamp = new DateTime?(currentServerTimeUtc),
        KeyFromMe = false,
        KeyRemoteJid = Settings.MyJid,
        Data = leftText
      };
      Message msg2 = new Message();
      msg2.MediaWaType = FunXMPP.FMessage.Type.Undefined;
      msg2.FunTimestamp = new DateTime?(currentServerTimeUtc);
      msg2.KeyFromMe = true;
      msg2.KeyRemoteJid = Settings.MyJid;
      msg2.Status = FunXMPP.FMessage.Status.ReadByTarget;
      msg2.Data = rightText;
      MessageViewModel messageViewModel1 = MessageViewModel.CreateForMessage(msg1, false, false, false).FirstOrDefault<MessageViewModel>();
      MessageViewModel messageViewModel2 = MessageViewModel.CreateForMessage(msg2, false, false, false).FirstOrDefault<MessageViewModel>();
      this.MessageBubble0.ViewModel = (object) messageViewModel1;
      this.MessageBubble1.ViewModel = (object) messageViewModel2;
    }
  }
}
