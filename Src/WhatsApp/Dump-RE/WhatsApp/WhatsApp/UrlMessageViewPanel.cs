// Decompiled with JetBrains decompiler
// Type: WhatsApp.UrlMessageViewPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

#nullable disable
namespace WhatsApp
{
  public class UrlMessageViewPanel : MessageViewPanel
  {
    protected RichTextBlock textBlock;
    private Grid topPanel;
    private Image thumbnailImage;
    private Grid detailsPanel;
    private TextBlock titleBlock;
    private TextBlock descriptionBlock;
    private TextBlock hostnameBlock;
    private Rectangle bottomPanelCover;

    public override MessageViewPanel.ViewTypes ViewType => MessageViewPanel.ViewTypes.Url;

    public UrlMessageViewPanel()
    {
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Star)
      });
      int pixels = (int) (100.0 * this.zoomMultiplier + 0.5);
      Grid grid1 = new Grid();
      grid1.Height = (double) pixels;
      grid1.Background = (Brush) new SolidColorBrush(Color.FromArgb((byte) 51, byte.MaxValue, byte.MaxValue, byte.MaxValue));
      this.topPanel = grid1;
      this.topPanel.Tap += new EventHandler<GestureEventArgs>(this.TopPanel_Tap);
      Grid.SetRow((FrameworkElement) this.topPanel, 0);
      this.Children.Add((UIElement) this.topPanel);
      this.topPanel.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength((double) pixels)
      });
      this.topPanel.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      Image image = new Image();
      image.HorizontalAlignment = HorizontalAlignment.Center;
      image.VerticalAlignment = VerticalAlignment.Center;
      image.Width = (double) pixels;
      image.Height = (double) pixels;
      image.Stretch = Stretch.UniformToFill;
      this.thumbnailImage = image;
      Grid.SetColumn((FrameworkElement) this.thumbnailImage, 0);
      this.topPanel.Children.Add((UIElement) this.thumbnailImage);
      Grid grid2 = new Grid();
      grid2.Margin = new Thickness(6.0, 6.0, 6.0, 0.0);
      grid2.HorizontalAlignment = HorizontalAlignment.Stretch;
      LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
      GradientStopCollection gradientStopCollection = new GradientStopCollection();
      gradientStopCollection.Add(new GradientStop()
      {
        Color = Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
        Offset = 0.0
      });
      gradientStopCollection.Add(new GradientStop()
      {
        Color = Color.FromArgb((byte) 0, byte.MaxValue, byte.MaxValue, byte.MaxValue),
        Offset = 1.0
      });
      linearGradientBrush.GradientStops = gradientStopCollection;
      linearGradientBrush.StartPoint = new System.Windows.Point(0.85, 0.0);
      linearGradientBrush.EndPoint = new System.Windows.Point(1.0, 0.0);
      grid2.OpacityMask = (Brush) linearGradientBrush;
      this.detailsPanel = grid2;
      Grid.SetColumn((FrameworkElement) this.detailsPanel, 1);
      this.topPanel.Children.Add((UIElement) this.detailsPanel);
      StackPanel stackPanel1 = new StackPanel();
      stackPanel1.VerticalAlignment = VerticalAlignment.Top;
      StackPanel stackPanel2 = stackPanel1;
      this.detailsPanel.Children.Add((UIElement) stackPanel2);
      TextBlock textBlock1 = new TextBlock();
      textBlock1.Foreground = (Brush) UIUtils.WhiteBrush;
      textBlock1.TextWrapping = TextWrapping.NoWrap;
      textBlock1.FontSize = 22.0 * this.zoomMultiplier;
      textBlock1.VerticalAlignment = VerticalAlignment.Top;
      this.titleBlock = textBlock1;
      stackPanel2.Children.Add((UIElement) this.titleBlock);
      TextBlock textBlock2 = new TextBlock();
      textBlock2.Foreground = (Brush) UIUtils.WhiteBrush;
      textBlock2.TextWrapping = TextWrapping.NoWrap;
      textBlock2.FontSize = 22.0 * this.zoomMultiplier;
      textBlock2.VerticalAlignment = VerticalAlignment.Top;
      textBlock2.Opacity = 0.65;
      this.descriptionBlock = textBlock2;
      stackPanel2.Children.Add((UIElement) this.descriptionBlock);
      TextBlock textBlock3 = new TextBlock();
      textBlock3.Foreground = (Brush) UIUtils.WhiteBrush;
      textBlock3.TextWrapping = TextWrapping.NoWrap;
      textBlock3.FontSize = 16.0 * this.zoomMultiplier;
      textBlock3.VerticalAlignment = VerticalAlignment.Bottom;
      textBlock3.Margin = new Thickness(0.0, 0.0, 0.0, 2.0);
      this.hostnameBlock = textBlock3;
      this.detailsPanel.Children.Add((UIElement) this.hostnameBlock);
      RichTextBlock richTextBlock = new RichTextBlock();
      richTextBlock.Foreground = (Brush) UIUtils.WhiteBrush;
      richTextBlock.TextWrapping = TextWrapping.Wrap;
      richTextBlock.LargeEmojiSize = true;
      this.textBlock = richTextBlock;
      Grid.SetRow((FrameworkElement) this.textBlock, 1);
      this.Children.Add((UIElement) this.textBlock);
    }

    public override void Render(MessageViewModel vm)
    {
      base.Render(vm);
      this.ClearText();
      if (!(vm is UrlMessageViewModel messageViewModel))
        return;
      if (messageViewModel.IsForGalleryView)
      {
        this.textBlock.TextWrapping = TextWrapping.NoWrap;
        if (this.bottomPanelCover == null)
        {
          Rectangle rectangle = new Rectangle();
          rectangle.Fill = (Brush) UIUtils.TransparentBrush;
          rectangle.HorizontalAlignment = HorizontalAlignment.Stretch;
          rectangle.VerticalAlignment = VerticalAlignment.Stretch;
          this.bottomPanelCover = rectangle;
          this.bottomPanelCover.Tap += new EventHandler<GestureEventArgs>(this.BottomPanelCover_Tap);
          Grid.SetRow((FrameworkElement) this.bottomPanelCover, 1);
          this.Children.Add((UIElement) this.bottomPanelCover);
        }
        this.ShowElement((FrameworkElement) this.bottomPanelCover, true);
      }
      else
      {
        this.textBlock.TextWrapping = TextWrapping.Wrap;
        this.ShowElement((FrameworkElement) this.bottomPanelCover, false);
      }
      WaRichText.Chunk[] inlineFormattings = vm.InlineFormattings;
      RichTextBlock.TextSet textSet = new RichTextBlock.TextSet();
      if (vm.TextHasRichContent || inlineFormattings != null)
      {
        this.textBlock.AllowLinks = vm.AllowLinks;
        textSet.SerializedFormatting = (IEnumerable<LinkDetector.Result>) vm.TextPerformanceHint;
        textSet.PartialFormattings = (IEnumerable<WaRichText.Chunk>) inlineFormattings;
      }
      textSet.Text = vm.TextStr;
      this.textBlock.Text = textSet;
      this.textBlock.FontSize = vm.TextFontSize;
      this.textBlock.Margin = new Thickness(-12.0, 4.0 * this.zoomMultiplier, -12.0, 0.0);
      if (vm.MergedPosition == MessageViewModel.GroupingPosition.Top || vm.MergedPosition == MessageViewModel.GroupingPosition.None || vm.IsForGalleryView)
      {
        if (vm.Message.BinaryData == null)
        {
          Grid.SetColumn((FrameworkElement) this.detailsPanel, 0);
          Grid.SetColumnSpan((FrameworkElement) this.detailsPanel, 2);
          this.thumbnailImage.Visibility = Visibility.Collapsed;
        }
        else
        {
          Grid.SetColumn((FrameworkElement) this.detailsPanel, 1);
          Grid.SetColumnSpan((FrameworkElement) this.detailsPanel, 1);
          this.thumbnailImage.Source = (System.Windows.Media.ImageSource) null;
          this.UpdateThumbnail(vm);
        }
        this.titleBlock.Text = messageViewModel.TitleStr;
        this.descriptionBlock.Text = messageViewModel.DescriptionStr;
        this.hostnameBlock.Text = messageViewModel.HostNameStr;
        this.ShowElement((FrameworkElement) this.topPanel, true);
      }
      else
        this.ShowElement((FrameworkElement) this.topPanel, false);
    }

    public override void ProcessViewModelNotification(KeyValuePair<string, object> args)
    {
      base.ProcessViewModelNotification(args);
      switch (args.Key)
      {
        case "TextFontSizeChanged":
          this.OnTextFontSizeChanged();
          break;
        case "ThumbnailChanged":
          this.UpdateThumbnail(this.viewModel);
          break;
        case "AllowLinksChanged":
          if (this.viewModel == null)
            break;
          this.textBlock.AllowLinks = this.viewModel.AllowLinks;
          this.textBlock.Refresh();
          break;
      }
    }

    public override void Cleanup()
    {
      base.Cleanup();
      this.thumbnailImage.Source = (System.Windows.Media.ImageSource) null;
      this.ClearText();
    }

    private void ClearText() => this.textBlock.Text = (RichTextBlock.TextSet) null;

    private void UpdateThumbnail(MessageViewModel vm)
    {
      if (vm == null)
        return;
      MessageViewModel.ThumbnailState thumbnail = vm.GetThumbnail();
      if (thumbnail == null || thumbnail.Image == null)
      {
        this.thumbnailImage.Source = (System.Windows.Media.ImageSource) null;
        this.thumbnailImage.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.thumbnailImage.Source = thumbnail.Image;
        this.thumbnailImage.Visibility = Visibility.Visible;
      }
    }

    private void OnTextFontSizeChanged() => this.textBlock.FontSize = this.viewModel.TextFontSize;

    private void BottomPanelCover_Tap(object sender, GestureEventArgs e)
    {
      if (this.viewModel == null || this.viewModel.Message == null)
        return;
      Message message = this.viewModel.Message;
      Log.l("msgbubble", "open chat to msg | uri cover tap | jid:{0},msgid:{1}", (object) message.KeyRemoteJid, (object) message.MessageID);
      ChatPage.NextInstanceInitState = new ChatPage.InitState()
      {
        MessageLoader = MessageLoader.Get(message.KeyRemoteJid, new int?(), 0, targetInitialLandingMsgId: new int?(message.MessageID)),
        SearchResult = new MessageSearchResult(message)
      };
      NavUtils.NavigateToChat(message.KeyRemoteJid, false, "MediaGalleryPage");
    }

    private void TopPanel_Tap(object sender, EventArgs e)
    {
      if (this.viewModel == null || this.viewModel.Message == null)
        return;
      UriMessageWrapper uriMessageWrapper = new UriMessageWrapper(this.viewModel.Message);
      string uriString = uriMessageWrapper.CanonicalUrl ?? uriMessageWrapper.MatchedText;
      if (uriString == null)
        return;
      if (uriMessageWrapper.MatchedText.StartsWith(GroupInviteLinkPage.GroupInviteLinkStart, StringComparison.OrdinalIgnoreCase))
      {
        GroupInviteLinkPage.JoinGroupWithInviteLink(uriMessageWrapper.MatchedText);
      }
      else
      {
        Uri result = (Uri) null;
        if (!Uri.TryCreate(uriString, UriKind.Absolute, out result))
          return;
        if (!(result != (Uri) null))
          return;
        try
        {
          new WebBrowserTask() { Uri = result }.Show();
        }
        catch (Exception ex)
        {
          Log.l("msgbubble", "Failing uri {0}, {1}", (object) uriString, (object) this.viewModel.Message.KeyId);
          Log.SendCrashLog(ex, "msgbubble Exception displaying url message", logOnlyForRelease: true);
        }
      }
    }
  }
}
