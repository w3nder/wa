// Decompiled with JetBrains decompiler
// Type: WhatsApp.StarredMessageBubbleContainer
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace WhatsApp
{
  public class StarredMessageBubbleContainer : MessageBubbleContainer
  {
    private Grid headerPanel;
    private StackPanel verifiedWrapper;
    private Image verifiedBadge;
    private RichTextBlock senderBlock;
    private TextBlock dateBlock;
    private Rectangle divider;
    private Rectangle tapRegion;

    public StarredMessageBubbleContainer()
    {
      double zoomMultiplier = ResolutionHelper.ZoomMultiplier;
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Auto)
      });
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Auto)
      });
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Auto)
      });
      Grid grid = new Grid();
      grid.Margin = new Thickness(0.0);
      this.headerPanel = grid;
      Grid.SetRow((FrameworkElement) this.headerPanel, 0);
      Grid.SetColumn((FrameworkElement) this.headerPanel, 1);
      Grid.SetColumnSpan((FrameworkElement) this.headerPanel, 3);
      this.Children.Add((UIElement) this.headerPanel);
      this.headerPanel.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      this.headerPanel.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      this.verifiedWrapper = new StackPanel()
      {
        Orientation = Orientation.Horizontal
      };
      RichTextBlock richTextBlock = new RichTextBlock();
      richTextBlock.Margin = new Thickness(-12.0, 0.0, 0.0, 0.0);
      richTextBlock.TextWrapping = TextWrapping.NoWrap;
      richTextBlock.FontSize = 22.0 * zoomMultiplier;
      this.senderBlock = richTextBlock;
      this.verifiedWrapper.Children.Add((UIElement) this.senderBlock);
      Grid.SetColumn((FrameworkElement) this.verifiedWrapper, 0);
      this.headerPanel.Children.Add((UIElement) this.verifiedWrapper);
      TextBlock textBlock = new TextBlock();
      textBlock.Margin = new Thickness(24.0 * zoomMultiplier, 0.0, 0.0, 0.0);
      textBlock.TextWrapping = TextWrapping.NoWrap;
      textBlock.FontSize = 22.0 * zoomMultiplier;
      this.dateBlock = textBlock;
      Grid.SetColumn((FrameworkElement) this.dateBlock, 1);
      this.headerPanel.Children.Add((UIElement) this.dateBlock);
      Grid.SetRow((FrameworkElement) this.msgBubble, 1);
      Rectangle rectangle1 = new Rectangle();
      rectangle1.Fill = (Brush) UIUtils.TransparentBrush;
      this.tapRegion = rectangle1;
      this.tapRegion.Tap += new EventHandler<GestureEventArgs>(this.Background_Tap);
      Grid.SetRow((FrameworkElement) this.tapRegion, 1);
      Grid.SetColumn((FrameworkElement) this.tapRegion, 3);
      Grid.SetColumnSpan((FrameworkElement) this.tapRegion, 2);
      this.Children.Add((UIElement) this.tapRegion);
      Rectangle rectangle2 = new Rectangle();
      rectangle2.Height = 1.0;
      rectangle2.HorizontalAlignment = HorizontalAlignment.Stretch;
      rectangle2.Opacity = 0.5;
      rectangle2.Margin = new Thickness(0.0, 6.0 * zoomMultiplier, 0.0, 12.0 * zoomMultiplier);
      rectangle2.Fill = (Brush) UIUtils.ForegroundBrush;
      this.divider = rectangle2;
      Grid.SetRow((FrameworkElement) this.divider, 2);
      Grid.SetColumn((FrameworkElement) this.divider, 1);
      Grid.SetColumnSpan((FrameworkElement) this.divider, 4);
      this.Children.Add((UIElement) this.divider);
    }

    protected override void Render(MessageViewModel vm, bool forceUpdate = false)
    {
      if (vm == null || !forceUpdate && vm == this.viewModel)
        return;
      base.Render(vm);
      if (vm.ShouldShowStarredViewHeader)
      {
        if (JidHelper.IsPsaJid(vm.Message.KeyRemoteJid))
        {
          if (this.verifiedBadge == null)
          {
            double num = 22.0 * ResolutionHelper.ZoomMultiplier;
            Image image = new Image();
            image.Source = (System.Windows.Media.ImageSource) AssetStore.InlineVerified;
            image.Margin = new Thickness(-12.0, 0.0, 0.0, 0.0);
            image.Height = num;
            image.Width = num;
            this.verifiedBadge = image;
            this.verifiedWrapper.Children.Add((UIElement) this.verifiedBadge);
          }
          this.verifiedBadge.Visibility = Visibility.Visible;
          this.senderBlock.Text = new RichTextBlock.TextSet()
          {
            Text = Constants.OffcialName
          };
        }
        else
        {
          if (this.verifiedBadge != null)
            this.verifiedBadge.Visibility = Visibility.Collapsed;
          this.senderBlock.Text = new RichTextBlock.TextSet()
          {
            Text = vm.StarredMessageSenderStr
          };
        }
        this.dateBlock.Text = vm.StarredMessageDateStr;
        this.headerPanel.Visibility = Visibility.Visible;
      }
      else
        this.headerPanel.Visibility = Visibility.Collapsed;
      if (vm.IsPsaChat)
        Grid.SetColumnSpan((FrameworkElement) this.headerPanel, 4);
      this.divider.Visibility = vm.ShouldShowFooter.ToVisibility();
    }

    protected override void OnViewModelNotified(KeyValuePair<string, object> p)
    {
      base.OnViewModelNotified(p);
      if (this.viewModel == null || !(p.Key == "StarredMessageSenderStrChanged"))
        return;
      this.senderBlock.Text = new RichTextBlock.TextSet()
      {
        Text = this.viewModel.StarredMessageSenderStr
      };
    }

    private void Background_Tap(object sender, EventArgs e)
    {
      if (!((sender as FrameworkElement).DataContext is MessageViewModel dataContext) || dataContext.Message == null)
        return;
      Message message = dataContext.Message;
      Log.l("starred msg", "open chat to msg | jid:{0},msgid:{1}", (object) message.KeyRemoteJid, (object) message.MessageID);
      ChatPage.NextInstanceInitState = new ChatPage.InitState()
      {
        MessageLoader = MessageLoader.Get(message.KeyRemoteJid, new int?(), 0, targetInitialLandingMsgId: new int?(message.MessageID)),
        SearchResult = new MessageSearchResult(message)
      };
      NavUtils.NavigateToChat(message.KeyRemoteJid, false, "StarredMessagesPage");
    }
  }
}
