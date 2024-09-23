// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageBubble
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

#nullable disable
namespace WhatsApp
{
  public class MessageBubble : Grid
  {
    private static double zoomMultiplier = ResolutionHelper.ZoomMultiplier;
    private static double sideMargin = 12.0 * MessageBubble.zoomMultiplier;
    private Rectangle backgroundRect = new Rectangle();
    private Polygon tailPolygon;
    private CompositeTransform tailTransform = new CompositeTransform();
    private Grid headerPanel;
    private Rectangle headerPanelBackground;
    private Grid senderPanel;
    private RichTextBlock senderBlock;
    private Rectangle gradientBlock;
    private MessageFooterPanel footerPanel;
    private MessageViewModel viewModel;
    private StackPanel topPanel;
    private Grid forwardedRow;
    private Image forwardedIcon;
    private TextBlock forwardedBlock;
    private MessageViewPanel quotedMsgViewPanel;
    private MessageViewPanel paymentViewPanel;
    private MessageViewPanel msgViewPanel;
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.RegisterAttached(nameof (ViewModel), typeof (object), typeof (MessageBubble), new PropertyMetadata((object) null, new PropertyChangedCallback(MessageBubble.OnViewModelChanged)));
    private const int incomingTailRow = 0;
    public const int groupMsgHdrRow = 1;
    private const int forwardedMsgRow = 2;
    private const int msgQuotePaymentRow = 3;
    public const int msgViewFooterRow = 4;
    private const int outgoingTailRow = 5;
    private const int backgroundInsertionPoint = 1;

    private string LogHeader
    {
      get
      {
        return string.Format("msgbubble > curr key:{0}", this.viewModel == null ? (object) "n/a" : (object) this.viewModel.Message.KeyId);
      }
    }

    public object ViewModel
    {
      get => this.GetValue(MessageBubble.ViewModelProperty);
      set => this.SetValue(MessageBubble.ViewModelProperty, value);
    }

    public MessageBubble()
    {
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      Grid.SetRow((FrameworkElement) this.backgroundRect, 2);
      Grid.SetRowSpan((FrameworkElement) this.backgroundRect, 3);
      this.Children.Add((UIElement) this.backgroundRect);
      this.topPanel = new StackPanel();
      Grid.SetRow((FrameworkElement) this.topPanel, 3);
      this.Children.Add((UIElement) this.topPanel);
      MessageFooterPanel messageFooterPanel = new MessageFooterPanel();
      messageFooterPanel.IsHitTestVisible = false;
      this.footerPanel = messageFooterPanel;
      Grid.SetRow((FrameworkElement) this.footerPanel, 4);
      this.Children.Add((UIElement) this.footerPanel);
      Polygon polygon = new Polygon();
      PointCollection pointCollection = new PointCollection();
      pointCollection.Add(new System.Windows.Point(0.0, 0.0));
      pointCollection.Add(new System.Windows.Point(0.0, MessageViewModel.TailSize.Height));
      pointCollection.Add(new System.Windows.Point(MessageViewModel.TailSize.Width, MessageViewModel.TailSize.Height));
      polygon.Points = pointCollection;
      polygon.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
      polygon.RenderTransform = (Transform) this.tailTransform;
      polygon.Margin = new Thickness(0.0);
      polygon.IsHitTestVisible = false;
      this.tailPolygon = polygon;
      this.Children.Add((UIElement) this.tailPolygon);
    }

    public void Render(MessageViewModel vm)
    {
      this.viewModel = vm;
      if (vm == null)
        return;
      vm.InitLazyResources();
      this.MaxWidth = vm.MaxBubbleWidth;
      this.HorizontalAlignment = vm.HorizontalAlignment;
      MessageViewPanel.ViewTypes viewType = MessageViewPanel.GetViewType(vm.Message);
      if (this.msgViewPanel != null && this.msgViewPanel.ViewType == viewType)
      {
        this.msgViewPanel.Cleanup();
        Log.d(this.LogHeader, "view panel reused");
      }
      else
      {
        MessageViewPanel msgViewPanel = this.msgViewPanel;
        MessageViewPanel messageViewPanel = MessageViewPanel.Get(viewType);
        if (msgViewPanel != null)
        {
          this.Children.Remove((UIElement) msgViewPanel);
          msgViewPanel.Cleanup();
          MessageViewPanel.Save(msgViewPanel);
        }
        this.Children.Insert(1, (UIElement) (this.msgViewPanel = messageViewPanel));
        Grid.SetRow((FrameworkElement) this.msgViewPanel, 4);
      }
      this.msgViewPanel.Margin = vm.ViewPanelMargin;
      this.msgViewPanel.Render(vm);
      if (vm.ShouldShowPayment)
      {
        if (this.paymentViewPanel == null)
        {
          this.paymentViewPanel = MessageViewPanel.Get(MessageViewPanel.ViewTypes.Payment);
          this.topPanel.Children.Insert(0, (UIElement) this.paymentViewPanel);
        }
        else
          this.paymentViewPanel.Cleanup();
        this.paymentViewPanel.Margin = vm.QuotedMessageViewPanelMargin;
        this.paymentViewPanel.Render(vm);
      }
      else if (this.paymentViewPanel != null)
      {
        this.topPanel.Children.Remove((UIElement) this.paymentViewPanel);
        this.paymentViewPanel.Cleanup();
        MessageViewPanel.Save(this.paymentViewPanel);
        this.paymentViewPanel = (MessageViewPanel) null;
      }
      if (vm.ShouldShowQuoteViewPanel)
      {
        if (this.quotedMsgViewPanel == null)
        {
          this.quotedMsgViewPanel = MessageViewPanel.Get(MessageViewPanel.ViewTypes.Quote);
          this.topPanel.Children.Insert(0, (UIElement) this.quotedMsgViewPanel);
        }
        else
          this.quotedMsgViewPanel.Cleanup();
        this.quotedMsgViewPanel.Margin = vm.QuotedMessageViewPanelMargin;
        this.quotedMsgViewPanel.Render(vm);
      }
      else if (this.quotedMsgViewPanel != null)
      {
        this.topPanel.Children.Remove((UIElement) this.quotedMsgViewPanel);
        this.quotedMsgViewPanel.Cleanup();
        MessageViewPanel.Save(this.quotedMsgViewPanel);
        this.quotedMsgViewPanel = (MessageViewPanel) null;
      }
      if (vm.ShouldShowForwardedMessageRow)
      {
        if (this.forwardedRow == null)
        {
          Grid grid = new Grid();
          grid.VerticalAlignment = VerticalAlignment.Stretch;
          grid.HorizontalAlignment = HorizontalAlignment.Left;
          this.forwardedRow = grid;
          this.forwardedRow.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = GridLength.Auto
          });
          this.forwardedRow.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = new GridLength(1.0, GridUnitType.Star)
          });
          Image image = new Image();
          image.Margin = new Thickness(6.0, 0.0, 6.0, 0.0);
          image.Height = 12.0;
          image.Opacity = 0.60000002384185791;
          image.Source = (System.Windows.Media.ImageSource) AssetStore.ForwardArrow;
          image.VerticalAlignment = VerticalAlignment.Center;
          this.forwardedIcon = image;
          Grid.SetColumn((FrameworkElement) this.forwardedIcon, 0);
          this.forwardedRow.Children.Add((UIElement) this.forwardedIcon);
          TextBlock textBlock = new TextBlock();
          textBlock.FontSize = MessageViewModel.SmallTextFontSizeStatic;
          textBlock.FontStyle = FontStyles.Italic;
          textBlock.Foreground = UIUtils.SubtleBrushWhite;
          textBlock.Margin = new Thickness(0.0);
          textBlock.Text = AppResources.Forwarded;
          textBlock.VerticalAlignment = VerticalAlignment.Center;
          this.forwardedBlock = textBlock;
          Grid.SetColumn((FrameworkElement) this.forwardedBlock, 1);
          this.forwardedRow.Children.Add((UIElement) this.forwardedBlock);
          Grid.SetRow((FrameworkElement) this.forwardedRow, 2);
          this.Children.Add((UIElement) this.forwardedRow);
        }
        this.forwardedRow.Margin = vm.ForwardedRowMargin;
        this.forwardedRow.Visibility = Visibility.Visible;
      }
      else if (this.forwardedRow != null)
        this.forwardedRow.Visibility = Visibility.Collapsed;
      this.UpdateBackground(vm);
      this.UpdateTail(vm);
      this.UpdateHeader(vm);
      this.UpdateFooter(vm);
    }

    public void Cleanup() => this.viewModel = (MessageViewModel) null;

    private void UpdateFooter(MessageViewModel vm)
    {
      this.footerPanel.Render(vm);
      this.footerPanel.Margin = vm.FooterMargin;
    }

    private void UpdateHeader(MessageViewModel vm)
    {
      if (vm.ShouldShowHeader)
      {
        if (this.headerPanel == null)
        {
          Grid grid = new Grid();
          grid.VerticalAlignment = VerticalAlignment.Top;
          grid.HorizontalAlignment = HorizontalAlignment.Stretch;
          this.headerPanel = grid;
          this.headerPanel.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = GridLength.Auto
          });
          this.headerPanel.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = new GridLength(1.0, GridUnitType.Star)
          });
          this.Children.Insert(1, (UIElement) this.headerPanel);
        }
        Grid.SetRow((FrameworkElement) this.headerPanel, vm.HeaderRow);
        if (this.senderPanel == null)
        {
          Rectangle rectangle1 = new Rectangle();
          rectangle1.Fill = (Brush) vm.BackgroundBrush;
          this.headerPanelBackground = rectangle1;
          this.headerPanel.Children.Add((UIElement) this.headerPanelBackground);
          Grid.SetColumn((FrameworkElement) this.headerPanelBackground, 0);
          Grid grid = new Grid();
          grid.VerticalAlignment = VerticalAlignment.Top;
          this.senderPanel = grid;
          this.headerPanel.Children.Add((UIElement) this.senderPanel);
          Grid.SetColumn((FrameworkElement) this.senderPanel, 0);
          RichTextBlock richTextBlock = new RichTextBlock();
          richTextBlock.FontSize = MessageViewModel.SmallTextFontSizeStatic;
          richTextBlock.Foreground = UIUtils.SubtleBrushWhite;
          richTextBlock.TextWrapping = TextWrapping.NoWrap;
          richTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
          richTextBlock.Margin = new Thickness(12.0 * MessageBubble.zoomMultiplier - 12.0, 4.0 * MessageBubble.zoomMultiplier, 0.0, 4.0 * MessageBubble.zoomMultiplier);
          richTextBlock.AllowTextFormatting = false;
          richTextBlock.EnableMentionLinks = false;
          richTextBlock.AllowLinks = false;
          this.senderBlock = richTextBlock;
          this.senderPanel.Children.Add((UIElement) this.senderBlock);
          Rectangle rectangle2 = new Rectangle();
          rectangle2.Stretch = Stretch.Fill;
          rectangle2.HorizontalAlignment = HorizontalAlignment.Right;
          rectangle2.VerticalAlignment = VerticalAlignment.Stretch;
          rectangle2.Width = 64.0 * MessageBubble.zoomMultiplier;
          this.gradientBlock = rectangle2;
          this.headerPanel.Children.Add((UIElement) this.gradientBlock);
          Grid.SetColumnSpan((FrameworkElement) this.gradientBlock, 2);
        }
        string str = vm.DisplayNameStr;
        if (vm.ShouldShowPushName)
          str = string.Format("{0}{1}{2}", (object) str, (object) new string(' ', 2), (object) vm.PushNameStr);
        this.senderBlock.Text = new RichTextBlock.TextSet()
        {
          Text = str
        };
        this.headerPanel.Visibility = Visibility.Visible;
        this.gradientBlock.Visibility = Visibility.Visible;
        if (vm.ShouldFillFullTitleBackground)
        {
          this.senderPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
          Grid.SetColumnSpan((FrameworkElement) this.headerPanelBackground, 2);
        }
        else
        {
          this.senderPanel.HorizontalAlignment = HorizontalAlignment.Left;
          Grid.SetColumnSpan((FrameworkElement) this.headerPanelBackground, 1);
          if (!vm.ShouldShowPushName)
            this.gradientBlock.Visibility = Visibility.Collapsed;
        }
        this.gradientBlock.Fill = (Brush) UIUtils.CreateFadingGradientBrush(vm.BackgroundBrush.Color, new System.Windows.Point(1.0, 0.0), new System.Windows.Point(0.0, 0.0));
      }
      else
      {
        if (this.headerPanel == null)
          return;
        this.headerPanel.Visibility = Visibility.Collapsed;
      }
    }

    private void UpdateTail(MessageViewModel vm)
    {
      if (vm.ShouldShowTail)
      {
        if (vm.ShouldShowOnOutgoingSide)
        {
          Grid.SetRow((FrameworkElement) this.tailPolygon, 5);
          this.tailTransform.TranslateY = -MessageViewModel.TailShift;
          this.tailTransform.Rotation = 180.0;
        }
        else
        {
          Grid.SetRow((FrameworkElement) this.tailPolygon, 0);
          this.tailTransform.TranslateY = MessageViewModel.TailShift;
          this.tailTransform.Rotation = 0.0;
        }
        this.tailPolygon.Fill = (Brush) vm.BackgroundBrush;
        this.tailPolygon.Visibility = Visibility.Visible;
      }
      else
        this.tailPolygon.Visibility = Visibility.Collapsed;
    }

    private void UpdateBackground(MessageViewModel vm)
    {
      this.backgroundRect.Fill = (Brush) vm.BackgroundBrush;
      if (vm.ShouldFillContentBackground)
        Grid.SetRowSpan((FrameworkElement) this.backgroundRect, 3);
      else
        Grid.SetRowSpan((FrameworkElement) this.backgroundRect, 2);
    }

    public void ProcessViewModelNotification(KeyValuePair<string, object> p)
    {
      this.msgViewPanel.ProcessViewModelNotification(p);
      string key = p.Key;
      // ISSUE: reference to a compiler-generated method
      switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(key))
      {
        case 97405557:
          if (!(key == "ChatBackgroundChanged"))
            break;
          this.UpdateBackground(this.viewModel);
          break;
        case 135637716:
          if (!(key == "Refresh"))
            break;
          this.Render(this.viewModel);
          break;
        case 463420430:
          if (!(key == "FooterInfoChanged"))
            break;
          this.footerPanel.UpdateFooterInfo(this.viewModel);
          break;
        case 1889495869:
          if (!(key == "StatusChanged"))
            break;
          this.OnMessageStatusChanged();
          break;
        case 2357660116:
          if (!(key == "HeaderChanged"))
            break;
          this.UpdateHeader(this.viewModel);
          break;
        case 3273603205:
          if (!(key == "ThumbnailChanged"))
            break;
          this.footerPanel.UpdateForeground(this.viewModel, false);
          break;
        case 3377139206:
          if (!(key == "GroupedPositionChanged"))
            break;
          this.OnGroupingPositionChanged();
          break;
        case 4022894740:
          if (!(key == "IsStarredChanged"))
            break;
          this.footerPanel.UpdateStarred(this.viewModel);
          break;
      }
    }

    public static void OnViewModelChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs args)
    {
      if (!(sender is MessageBubble messageBubble))
        return;
      try
      {
        messageBubble.Render(args.NewValue as MessageViewModel);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "view model changed on message bubble");
      }
    }

    private void OnGroupingPositionChanged()
    {
      this.UpdateHeader(this.viewModel);
      this.UpdateTail(this.viewModel);
      this.Margin = this.viewModel.BubbleMargin;
    }

    private void OnMessageStatusChanged()
    {
      Log.d(this.LogHeader, "update status icon | status:{0}", this.viewModel == null ? (object) "n/a" : (object) this.viewModel.Message.Status.ToString());
      this.footerPanel.UpdateStatusIcon(this.viewModel, true);
      if (this.viewModel.Message.Status != FunXMPP.FMessage.Status.Error)
        return;
      this.OnMessageError(this.viewModel.Message);
    }

    private void OnMessageError(Message m)
    {
      if (m.Status != FunXMPP.FMessage.Status.Error)
        return;
      MessageMiscInfo miscInfo = m.GetMiscInfo();
      if (miscInfo == null)
        return;
      int? errorCode = miscInfo.ErrorCode;
      if (!errorCode.HasValue)
        return;
      errorCode = miscInfo.ErrorCode;
      if (errorCode.Value != 4)
      {
        errorCode = miscInfo.ErrorCode;
        if (errorCode.Value != 9)
          return;
      }
      ReceiptSpec receiptSpec = new ReceiptSpec()
      {
        Jid = m.KeyRemoteJid,
        Id = m.KeyId,
        Participant = m.RemoteResource
      };
      if (string.IsNullOrEmpty(receiptSpec.Participant))
        receiptSpec.Participant = (string) null;
      else if (m.IsBroadcasted())
        Utils.Swap<string>(ref receiptSpec.Jid, ref receiptSpec.Participant);
      AppState.SchedulePersistentAction(PersistentAction.SendReceipts((IEnumerable<ReceiptSpec>) new ReceiptSpec[1]
      {
        receiptSpec
      }, "server-error"));
      Log.l(this.LogHeader, "media not available on server | msg fun time: {0}, current fun time: {1}, type: {2}", (object) m.FunTimestamp, (object) FunRunner.CurrentServerTimeUtc, (object) m.MediaWaType);
      int num = (int) MessageBox.Show(string.Format(AppResources.MediaDownloadFailureResendNeeded, (object) m.GetSenderDisplayName(false)));
    }
  }
}
