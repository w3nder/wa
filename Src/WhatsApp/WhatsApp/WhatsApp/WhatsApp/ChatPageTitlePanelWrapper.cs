// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatPageTitlePanelWrapper
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class ChatPageTitlePanelWrapper : Grid
  {
    private ChatPageTitlePanel titlePanel;
    private Grid titleInfoPanel;
    private Border titleInfoPanelBorder;
    private StackPanel contentPanel;
    private TextBlock subtitleBlock;
    private RichTextBlock contentBlock;
    private Button dismissTitleInfoPanelButton;
    private Image dismissButtonImage;
    private readonly ChatPageViewModel viewModel;

    public ChatPageTitlePanelWrapper(ChatPageViewModel viewModel) => this.viewModel = viewModel;

    public void Dispose() => this.titlePanel.Dispose();

    public void InitTitleInfoPanel()
    {
      if (this.titleInfoPanel != null)
        return;
      Border border = new Border();
      border.BorderBrush = UIUtils.PhoneInactiveBrush;
      border.BorderThickness = new Thickness(0.0, 0.0, 0.0, 1.0);
      border.Visibility = Visibility.Collapsed;
      this.titleInfoPanelBorder = border;
      Grid grid = new Grid();
      grid.CacheMode = (CacheMode) new BitmapCache();
      grid.HorizontalAlignment = HorizontalAlignment.Stretch;
      grid.VerticalAlignment = VerticalAlignment.Top;
      grid.Background = UIUtils.PhoneChromeBrush;
      grid.Visibility = Visibility.Collapsed;
      grid.Height = 72.0 * ResolutionHelper.ZoomMultiplier;
      grid.Margin = new Thickness(0.0);
      this.titleInfoPanel = grid;
      this.titleInfoPanelBorder.Child = (UIElement) this.titleInfoPanel;
      Grid.SetRow((FrameworkElement) this.titleInfoPanelBorder, 1);
      this.Children.Insert(1, (UIElement) this.titleInfoPanelBorder);
      this.titleInfoPanel.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      this.titleInfoPanel.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      double num1 = 8.0 * ResolutionHelper.ZoomMultiplier;
      StackPanel stackPanel = new StackPanel();
      stackPanel.Orientation = Orientation.Vertical;
      stackPanel.Margin = new Thickness(num1, num1, 0.0, num1);
      stackPanel.Width = 440.0 * ResolutionHelper.ZoomMultiplier;
      stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
      this.contentPanel = stackPanel;
      this.contentPanel.Tap += new EventHandler<GestureEventArgs>(this.ContentPanel_Tap);
      TextBlock textBlock = new TextBlock();
      textBlock.Margin = new Thickness(0.0);
      textBlock.TextWrapping = TextWrapping.NoWrap;
      textBlock.FontSize = UIUtils.FontSizeSmall;
      textBlock.FontFamily = UIUtils.FontFamilyNormal;
      textBlock.FontWeight = FontWeights.SemiBold;
      textBlock.Foreground = (Brush) UIUtils.AccentBrush;
      this.subtitleBlock = textBlock;
      RichTextBlock richTextBlock = new RichTextBlock();
      richTextBlock.Margin = new Thickness(-12.0 * ResolutionHelper.ZoomMultiplier, 0.0, 0.0, 0.0);
      richTextBlock.TextWrapping = TextWrapping.NoWrap;
      richTextBlock.AllowLinks = true;
      richTextBlock.FontSize = UIUtils.FontSizeSmall;
      richTextBlock.FontFamily = UIUtils.FontFamilySemiLight;
      richTextBlock.Foreground = (Brush) UIUtils.ForegroundBrush;
      this.contentBlock = richTextBlock;
      this.contentPanel.Children.Add((UIElement) this.subtitleBlock);
      this.contentPanel.Children.Add((UIElement) this.contentBlock);
      Grid.SetColumn((FrameworkElement) this.contentPanel, 0);
      this.titleInfoPanel.Children.Insert(0, (UIElement) this.contentPanel);
      Image image = new Image();
      image.Width = 36.0 * ResolutionHelper.ZoomMultiplier;
      image.Height = 36.0 * ResolutionHelper.ZoomMultiplier;
      image.VerticalAlignment = VerticalAlignment.Top;
      image.Margin = new Thickness(0.0);
      this.dismissButtonImage = image;
      double num2 = 18.0 * ResolutionHelper.ZoomMultiplier;
      Button button = new Button();
      button.Margin = new Thickness(num1, num2, num1, num2);
      button.Padding = new Thickness(0.0);
      button.HorizontalAlignment = HorizontalAlignment.Right;
      button.VerticalAlignment = VerticalAlignment.Center;
      button.FontSize = UIUtils.FontSizeSmall;
      button.FontFamily = UIUtils.FontFamilySemiLight;
      button.Foreground = (Brush) UIUtils.ForegroundBrush;
      button.Style = Application.Current.Resources[(object) "BorderlessButton"] as Style;
      button.Content = (object) this.dismissButtonImage;
      this.dismissTitleInfoPanelButton = button;
      Grid.SetColumn((FrameworkElement) this.dismissTitleInfoPanelButton, 1);
      this.titleInfoPanel.Children.Insert(1, (UIElement) this.dismissTitleInfoPanelButton);
      this.dismissTitleInfoPanelButton.Click += new RoutedEventHandler(this.Dismiss_Click);
    }

    private void ContentPanel_Tap(object sender, GestureEventArgs e)
    {
      Log.d("chat page info title", "tapped see more");
      GroupInfoPage.Start((NavigationService) null, this.viewModel.Conversation, true);
    }

    public void Init(ChatPage.InitState initState)
    {
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Auto)
      });
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Auto)
      });
      string title = (string) null;
      System.Windows.Media.ImageSource chatPic = (System.Windows.Media.ImageSource) null;
      if (initState != null)
      {
        title = initState.Title;
        chatPic = initState.Picture;
      }
      ChatPageTitlePanel chatPageTitlePanel = new ChatPageTitlePanel(title, chatPic);
      chatPageTitlePanel.CacheMode = (CacheMode) new BitmapCache();
      chatPageTitlePanel.HorizontalAlignment = HorizontalAlignment.Stretch;
      chatPageTitlePanel.VerticalAlignment = VerticalAlignment.Top;
      chatPageTitlePanel.Margin = new Thickness(UIUtils.PageSideMargin, 0.0, UIUtils.PageSideMargin / 2.0, 0.0);
      this.titlePanel = chatPageTitlePanel;
      Grid.SetRow((FrameworkElement) this.titlePanel, 0);
      this.Children.Insert(0, (UIElement) this.titlePanel);
    }

    public void UpdateTitleInfoPanel()
    {
      if (this.titleInfoPanel == null)
        return;
      if (this.viewModel != null)
      {
        this.titleInfoPanelBorder.Visibility = this.titleInfoPanel.Visibility = this.viewModel.TitleInfoPanelVisibility;
        this.subtitleBlock.Text = AppResources.GroupDescriptionTitle;
        this.contentBlock.AllowLinks = this.viewModel.AllowLinks;
        this.contentBlock.Text = new RichTextBlock.TextSet()
        {
          Text = this.viewModel.Conversation.GroupDescription
        };
        this.dismissButtonImage.Source = this.viewModel.DimissButtonIcon;
      }
      else
        this.titleInfoPanelBorder.Visibility = this.titleInfoPanel.Visibility = Visibility.Collapsed;
    }

    public void Render(Conversation convo) => this.titlePanel.Render(convo);

    public ProcessedPresence LastPresence
    {
      set => this.titlePanel.LastPresence = value;
    }

    public void UpdateTitle() => this.titlePanel.UpdateTitle();

    public void UpdateTitleForeground()
    {
      this.titlePanel.SetForeground(this.viewModel.TitleForeground);
    }

    public void SetMoreActions(IEnumerable<FlyoutCommand> actions)
    {
      this.titlePanel.SetMoreActions(actions);
    }

    public void EnableVoipButtons(bool enable) => this.titlePanel.EnableVoipButtons(enable);

    private void Dismiss_Click(object sender, RoutedEventArgs e)
    {
      this.viewModel.ShouldShowGroupDescription = false;
      this.UpdateTitleInfoPanel();
    }

    public void SetTitlePanelMargin(Thickness margin) => this.titlePanel.Margin = margin;
  }
}
