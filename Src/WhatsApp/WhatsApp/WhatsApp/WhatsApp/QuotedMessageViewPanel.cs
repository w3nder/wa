// Decompiled with JetBrains decompiler
// Type: WhatsApp.QuotedMessageViewPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class QuotedMessageViewPanel : MessageViewPanel
  {
    private static Subject<Pair<FunXMPP.FMessage.Key, FunXMPP.FMessage.Key>> quotedMessageJumpSubject;
    private static Subject<string> quotedGroupJumpSubject;
    private Grid topRow;
    private TextBlock quotedSenderBlock;
    private Image quotedMediaIcon;
    private RichTextBlock quotedMediaTypeBlock;
    private StackPanel quotedMediaIconAndTypeLine;
    private Image quotedThumbnailImage;
    private RichTextBlock quotedContentBlock;
    private Image quotedContentImage;
    private Rectangle indicatingBar;
    private Subject<Unit> dismissedSubject;
    private IDisposable thumbSub;
    private bool isComposing;
    private Button dismissButton;

    public override MessageViewPanel.ViewTypes ViewType => MessageViewPanel.ViewTypes.Quote;

    public bool IsComposing
    {
      get => this.isComposing;
      set
      {
        if (this.isComposing == value)
          return;
        this.isComposing = value;
        this.Background = this.isComposing ? (Brush) new SolidColorBrush(Colors.Transparent) : (Brush) new SolidColorBrush(Color.FromArgb((byte) 51, byte.MaxValue, byte.MaxValue, byte.MaxValue));
      }
    }

    public double ThumbnailHeight => (double) (int) (68.0 * this.zoomMultiplier + 0.5);

    public double PreviewExpansionHeight
    {
      get
      {
        MessageViewModel viewModel = this.viewModel;
        int num;
        if (viewModel == null)
        {
          num = 0;
        }
        else
        {
          FunXMPP.FMessage.Type? mediaWaType = viewModel.Message?.MediaWaType;
          FunXMPP.FMessage.Type type = FunXMPP.FMessage.Type.Sticker;
          num = mediaWaType.GetValueOrDefault() == type ? (mediaWaType.HasValue ? 1 : 0) : 0;
        }
        return num == 0 ? this.ThumbnailHeight : 1.5 * this.ThumbnailHeight + 40.0 * this.zoomMultiplier;
      }
    }

    private Button DismissButton
    {
      get
      {
        if (this.dismissButton == null)
        {
          double num = 36.0 * this.zoomMultiplier;
          Image image1 = new Image();
          image1.Stretch = Stretch.UniformToFill;
          image1.VerticalAlignment = VerticalAlignment.Center;
          image1.HorizontalAlignment = HorizontalAlignment.Center;
          image1.Source = (System.Windows.Media.ImageSource) AssetStore.DismissIcon;
          image1.Width = num;
          image1.Height = num;
          Image image2 = image1;
          Button button = new Button();
          button.Margin = new Thickness(12.0 * this.zoomMultiplier, 0.0, 12.0 * this.zoomMultiplier, 0.0);
          button.Padding = new Thickness(0.0);
          button.Style = Application.Current.Resources[(object) "BorderlessButton"] as Style;
          button.VerticalAlignment = VerticalAlignment.Center;
          button.HorizontalAlignment = HorizontalAlignment.Center;
          button.VerticalContentAlignment = VerticalAlignment.Center;
          button.HorizontalContentAlignment = HorizontalAlignment.Center;
          button.Content = (object) image2;
          button.Width = num;
          button.Height = num;
          this.dismissButton = button;
          this.dismissButton.Click += (RoutedEventHandler) ((sender, e) => this.NotifyDismissed());
          Grid.SetRow((FrameworkElement) this.dismissButton, 0);
          Grid.SetColumn((FrameworkElement) this.dismissButton, 2);
          this.Children.Add((UIElement) this.dismissButton);
        }
        return this.dismissButton;
      }
    }

    public QuotedMessageViewPanel()
    {
      this.Background = (Brush) new SolidColorBrush(Color.FromArgb((byte) 51, byte.MaxValue, byte.MaxValue, byte.MaxValue));
      this.Tap += new EventHandler<GestureEventArgs>(this.QuotePanel_Tap);
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Auto)
      });
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Auto)
      });
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(4.0 * this.zoomMultiplier, GridUnitType.Pixel)
      });
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      Rectangle rectangle = new Rectangle();
      rectangle.Fill = (Brush) UIUtils.WhiteBrush;
      rectangle.HorizontalAlignment = HorizontalAlignment.Stretch;
      rectangle.VerticalAlignment = VerticalAlignment.Stretch;
      this.indicatingBar = rectangle;
      Grid.SetColumn((FrameworkElement) this.indicatingBar, 0);
      Grid.SetRow((FrameworkElement) this.indicatingBar, 0);
      Grid.SetRowSpan((FrameworkElement) this.indicatingBar, 2);
      this.Children.Add((UIElement) this.indicatingBar);
      double thumbnailHeight = this.ThumbnailHeight;
      Grid grid = new Grid();
      grid.MaxHeight = thumbnailHeight;
      grid.Margin = new Thickness(6.0 * this.zoomMultiplier, 0.0, 0.0, 0.0);
      this.topRow = grid;
      this.topRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      this.topRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      Grid.SetRow((FrameworkElement) this.topRow, 0);
      Grid.SetColumn((FrameworkElement) this.topRow, 1);
      this.Children.Add((UIElement) this.topRow);
      TextBlock textBlock = new TextBlock();
      textBlock.Foreground = (Brush) UIUtils.WhiteBrush;
      textBlock.TextWrapping = TextWrapping.NoWrap;
      textBlock.FontSize = 22.0 * this.zoomMultiplier;
      textBlock.HorizontalAlignment = HorizontalAlignment.Left;
      textBlock.VerticalAlignment = VerticalAlignment.Top;
      textBlock.FontWeight = FontWeights.SemiBold;
      this.quotedSenderBlock = textBlock;
      Grid.SetColumn((FrameworkElement) this.quotedSenderBlock, 0);
      this.topRow.Children.Add((UIElement) this.quotedSenderBlock);
      StackPanel stackPanel = new StackPanel();
      stackPanel.Orientation = Orientation.Horizontal;
      stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
      stackPanel.VerticalAlignment = VerticalAlignment.Bottom;
      stackPanel.Margin = new Thickness(0.0, 0.0, 12.0 * this.zoomMultiplier, 4.0 * this.zoomMultiplier);
      this.quotedMediaIconAndTypeLine = stackPanel;
      Grid.SetColumn((FrameworkElement) this.quotedMediaIconAndTypeLine, 0);
      this.topRow.Children.Add((UIElement) this.quotedMediaIconAndTypeLine);
      Image image1 = new Image();
      image1.Margin = new Thickness(0.0, 0.0, 6.0 * this.zoomMultiplier, 0.0);
      image1.Height = 24.0 * this.zoomMultiplier;
      image1.Stretch = Stretch.UniformToFill;
      image1.VerticalAlignment = VerticalAlignment.Bottom;
      this.quotedMediaIcon = image1;
      this.quotedMediaIconAndTypeLine.Children.Add((UIElement) this.quotedMediaIcon);
      RichTextBlock richTextBlock1 = new RichTextBlock();
      richTextBlock1.Foreground = (Brush) UIUtils.WhiteBrush;
      richTextBlock1.TextWrapping = TextWrapping.NoWrap;
      richTextBlock1.FontSize = 20.0 * this.zoomMultiplier;
      richTextBlock1.HorizontalAlignment = HorizontalAlignment.Left;
      richTextBlock1.VerticalAlignment = VerticalAlignment.Bottom;
      this.quotedMediaTypeBlock = richTextBlock1;
      this.quotedMediaIconAndTypeLine.Children.Add((UIElement) this.quotedMediaTypeBlock);
      Image image2 = new Image();
      image2.Width = thumbnailHeight;
      image2.Height = thumbnailHeight;
      image2.Stretch = Stretch.UniformToFill;
      image2.VerticalAlignment = VerticalAlignment.Stretch;
      image2.Visibility = Visibility.Collapsed;
      this.quotedThumbnailImage = image2;
      Grid.SetColumn((FrameworkElement) this.quotedThumbnailImage, 1);
      this.topRow.Children.Add((UIElement) this.quotedThumbnailImage);
      RichTextBlock richTextBlock2 = new RichTextBlock();
      richTextBlock2.Foreground = (Brush) UIUtils.WhiteBrush;
      richTextBlock2.TextWrapping = TextWrapping.Wrap;
      richTextBlock2.FontSize = 20.0 * this.zoomMultiplier;
      richTextBlock2.Margin = new Thickness(6.0 * this.zoomMultiplier - 12.0, 0.0, -12.0, 3.0 * this.zoomMultiplier);
      richTextBlock2.VerticalAlignment = VerticalAlignment.Top;
      richTextBlock2.AllowLinks = false;
      richTextBlock2.LargeEmojiSize = true;
      richTextBlock2.MaxHeight = thumbnailHeight * 1.5;
      richTextBlock2.EnableMentionLinks = false;
      this.quotedContentBlock = richTextBlock2;
      Grid.SetRow((FrameworkElement) this.quotedContentBlock, 1);
      Grid.SetColumn((FrameworkElement) this.quotedContentBlock, 1);
      this.Children.Add((UIElement) this.quotedContentBlock);
      Image image3 = new Image();
      image3.VerticalAlignment = VerticalAlignment.Top;
      image3.HorizontalAlignment = HorizontalAlignment.Left;
      image3.Margin = new Thickness(6.0 * this.zoomMultiplier, 0.0, 0.0, 3.0 * this.zoomMultiplier);
      image3.Height = thumbnailHeight * 1.5;
      image3.Width = thumbnailHeight * 1.5;
      this.quotedContentImage = image3;
      Grid.SetRow((FrameworkElement) this.quotedContentImage, 1);
      Grid.SetColumn((FrameworkElement) this.quotedContentImage, 1);
      this.Children.Add((UIElement) this.quotedContentImage);
    }

    public static IObservable<Pair<FunXMPP.FMessage.Key, FunXMPP.FMessage.Key>> JumpToQuotedMessageObservable()
    {
      return (IObservable<Pair<FunXMPP.FMessage.Key, FunXMPP.FMessage.Key>>) QuotedMessageViewPanel.quotedMessageJumpSubject ?? (IObservable<Pair<FunXMPP.FMessage.Key, FunXMPP.FMessage.Key>>) (QuotedMessageViewPanel.quotedMessageJumpSubject = new Subject<Pair<FunXMPP.FMessage.Key, FunXMPP.FMessage.Key>>());
    }

    public static IObservable<string> JumpToQuotedGroupObservable()
    {
      return (IObservable<string>) QuotedMessageViewPanel.quotedGroupJumpSubject ?? (IObservable<string>) (QuotedMessageViewPanel.quotedGroupJumpSubject = new Subject<string>());
    }

    private void NotifyJumpToQuotedMessage()
    {
      if (QuotedMessageViewPanel.quotedMessageJumpSubject == null)
        return;
      Message quotedMessage = this.viewModel.QuotedMessage;
      string quotedGroupJid = this.viewModel.QuotedGroupJid;
      if (quotedMessage != null)
      {
        FunXMPP.FMessage.Key first = new FunXMPP.FMessage.Key(this.viewModel.Message.KeyRemoteJid, this.viewModel.Message.KeyFromMe, this.viewModel.Message.KeyId);
        FunXMPP.FMessage.Key second = new FunXMPP.FMessage.Key(quotedMessage.KeyRemoteJid, quotedMessage.KeyFromMe, quotedMessage.KeyId);
        QuotedMessageViewPanel.quotedMessageJumpSubject.OnNext(new Pair<FunXMPP.FMessage.Key, FunXMPP.FMessage.Key>(first, second));
      }
      else
      {
        if (quotedGroupJid == null)
          return;
        QuotedMessageViewPanel.quotedGroupJumpSubject.OnNext(quotedGroupJid);
      }
    }

    public IObservable<Unit> DimissedObservable()
    {
      return (IObservable<Unit>) this.dismissedSubject ?? (IObservable<Unit>) (this.dismissedSubject = new Subject<Unit>());
    }

    private void NotifyDismissed()
    {
      if (this.dismissedSubject == null || !this.IsComposing)
        return;
      this.dismissedSubject.OnNext(new Unit());
    }

    public override void Render(MessageViewModel vm) => this.Render(vm, false);

    public void Render(string quotedChat)
    {
      this.ClearText();
      this.ClearImage();
      string quotedName = (string) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => quotedName = db.GetConversation(quotedChat, CreateOptions.None)?.GroupSubject));
      if (string.IsNullOrEmpty(quotedName))
      {
        this.NotifyDismissed();
      }
      else
      {
        this.indicatingBar.Fill = this.quotedSenderBlock.Foreground = this.quotedMediaTypeBlock.Foreground = this.quotedContentBlock.Foreground = (Brush) UIUtils.ForegroundBrush;
        this.quotedSenderBlock.Text = string.Format("{0} {1} {2}", (object) AppResources.ReplyFromGroup, (object) '·', (object) quotedName);
        this.ShowElement((FrameworkElement) this.DismissButton, true);
      }
    }

    public void Render(MessageViewModel vm, bool isQuotePreview)
    {
      base.Render(vm);
      this.ClearText();
      this.ClearImage();
      Rectangle indicatingBar = this.indicatingBar;
      TextBlock quotedSenderBlock = this.quotedSenderBlock;
      RichTextBlock quotedMediaTypeBlock = this.quotedMediaTypeBlock;
      Brush brush1;
      this.quotedContentBlock.Foreground = brush1 = this.IsComposing ? (Brush) UIUtils.ForegroundBrush : (Brush) UIUtils.WhiteBrush;
      Brush brush2;
      Brush brush3 = brush2 = brush1;
      quotedMediaTypeBlock.Foreground = brush2;
      Brush brush4;
      Brush brush5 = brush4 = brush3;
      quotedSenderBlock.Foreground = brush4;
      Brush brush6 = brush5;
      indicatingBar.Fill = brush6;
      this.quotedSenderBlock.Text = isQuotePreview ? vm.QuotePreviewSenderNameStr : vm.QuoteSenderNameStr;
      System.Windows.Media.ImageSource imageSource = isQuotePreview ? vm.QuotePreviewMediaIconSource : vm.QuoteMediaIconSource;
      this.quotedMediaIconAndTypeLine.Visibility = (imageSource != null || this.IsComposing).ToVisibility();
      this.quotedMediaIcon.Source = imageSource;
      this.quotedMediaIcon.Visibility = (imageSource != null).ToVisibility();
      LinkDetector.Result[] formats = (LinkDetector.Result[]) null;
      string str1 = this.IsComposing & isQuotePreview ? vm.Message.GetPreviewText(out formats, true, false, isQuotePreview) : vm.QuotedMediaStr;
      this.quotedMediaTypeBlock.Text = new RichTextBlock.TextSet()
      {
        Text = str1,
        SerializedFormatting = (IEnumerable<LinkDetector.Result>) formats
      };
      this.quotedMediaTypeBlock.Visibility = Visibility.Visible;
      this.thumbSub.SafeDispose();
      this.thumbSub = (IDisposable) null;
      MessageViewModel messageViewModel1 = isQuotePreview ? vm : vm.QuotedMessageViewModel;
      StickerMessageViewModel messageViewModel2 = messageViewModel1 as StickerMessageViewModel;
      if (messageViewModel1 == null)
      {
        this.quotedThumbnailImage.Visibility = Visibility.Collapsed;
        this.quotedContentImage.Visibility = Visibility.Collapsed;
      }
      else if (messageViewModel2 != null)
      {
        this.topRow.Height = double.NaN;
        this.quotedMediaTypeBlock.Visibility = Visibility.Collapsed;
        this.quotedMediaIconAndTypeLine.Visibility = Visibility.Collapsed;
        this.quotedThumbnailImage.Visibility = Visibility.Collapsed;
        this.quotedThumbnailImage.Source = (System.Windows.Media.ImageSource) null;
        this.quotedContentImage.Source = AssetStore.StickerThumbnailPlaceholder;
        this.quotedContentImage.Visibility = Visibility.Visible;
        this.quotedContentImage.Opacity = 0.0;
        if (messageViewModel1.IsThumbnailAvailable())
        {
          IObservable<MessageViewModel.ThumbnailState> thumbnailObservable = messageViewModel1.GetThumbnailObservable();
          if (thumbnailObservable != null)
            this.thumbSub = thumbnailObservable.ObserveOnDispatcher<MessageViewModel.ThumbnailState>().Subscribe<MessageViewModel.ThumbnailState>((Action<MessageViewModel.ThumbnailState>) (thumbState =>
            {
              if (thumbState?.Image != null)
                this.quotedContentImage.Source = thumbState?.Image;
              this.quotedContentImage.Opacity = 1.0;
            }));
          else
            this.quotedContentImage.Opacity = 1.0;
        }
        else
        {
          if (vm.Message.QuotedMediaFileUri != null)
            messageViewModel2.Message.LocalFileUri = vm.Message.QuotedMediaFileUri;
          if (messageViewModel2.IsThumbnailAvailable())
          {
            IObservable<MessageViewModel.ThumbnailState> thumbnailObservable = messageViewModel1.GetThumbnailObservable();
            if (thumbnailObservable != null)
              this.thumbSub = thumbnailObservable.ObserveOnDispatcher<MessageViewModel.ThumbnailState>().Subscribe<MessageViewModel.ThumbnailState>((Action<MessageViewModel.ThumbnailState>) (thumbState =>
              {
                if (thumbState?.Image != null)
                  this.quotedContentImage.Source = thumbState?.Image;
                this.quotedContentImage.Opacity = 1.0;
              }));
          }
          else
            this.quotedContentImage.Opacity = 1.0;
        }
      }
      else
      {
        this.quotedContentImage.Visibility = Visibility.Collapsed;
        if (messageViewModel1.IsThumbnailAvailable())
        {
          IObservable<MessageViewModel.ThumbnailState> thumbnailObservable = messageViewModel1.GetThumbnailObservable();
          if (thumbnailObservable != null)
            this.thumbSub = thumbnailObservable.ObserveOnDispatcher<MessageViewModel.ThumbnailState>().Subscribe<MessageViewModel.ThumbnailState>((Action<MessageViewModel.ThumbnailState>) (thumbState =>
            {
              this.quotedThumbnailImage.Visibility = (thumbState?.Image != null).ToVisibility();
              this.quotedThumbnailImage.Source = thumbState?.Image;
            }));
        }
        else
        {
          this.quotedThumbnailImage.Source = (System.Windows.Media.ImageSource) null;
          this.quotedThumbnailImage.Visibility = Visibility.Collapsed;
        }
        this.topRow.Height = this.IsComposing || imageSource != null ? this.quotedThumbnailImage.Height : double.NaN;
      }
      string str2 = this.IsComposing ? (string) null : (isQuotePreview ? vm.TextStr : vm.QuotedMessageViewModel?.TextStr);
      if (string.IsNullOrEmpty(str2))
      {
        this.quotedContentBlock.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.quotedContentBlock.Visibility = Visibility.Visible;
        this.quotedContentBlock.Text = new RichTextBlock.TextSet()
        {
          Text = str2,
          SerializedFormatting = isQuotePreview ? (IEnumerable<LinkDetector.Result>) vm.TextPerformanceHint : (IEnumerable<LinkDetector.Result>) vm.QuotedMessageViewModel?.TextPerformanceHint
        };
      }
      if (this.IsComposing)
        this.ShowElement((FrameworkElement) this.DismissButton, true);
      else
        this.ShowElement((FrameworkElement) this.DismissButton, false);
    }

    private void ClearImage() => this.quotedContentImage.Source = (System.Windows.Media.ImageSource) null;

    public override void Cleanup()
    {
      base.Cleanup();
      this.thumbSub.SafeDispose();
      this.thumbSub = (IDisposable) null;
      this.quotedContentImage.Source = (System.Windows.Media.ImageSource) null;
      if (this.quotedMediaIcon != null)
        this.quotedMediaIcon.Source = (System.Windows.Media.ImageSource) null;
      this.ClearText();
      this.ClearImage();
    }

    private void ClearText() => this.quotedContentBlock.Text = (RichTextBlock.TextSet) null;

    private void QuotePanel_Tap(object sender, GestureEventArgs e)
    {
      if (this.isComposing)
        return;
      this.NotifyJumpToQuotedMessage();
    }
  }
}
