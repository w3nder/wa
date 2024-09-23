// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatPageTitlePanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WhatsApp.CommonOps;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class ChatPageTitlePanel : Grid
  {
    private double PanelSize = 60.0 * ResolutionHelper.ZoomMultiplier;
    private StackPanel textPanelContent;
    private JidNameControl titleBlock;
    private TextTrimmingControl2 subtitleBlock;
    private Button textPanel;
    private Grid chatImagePanel;
    private Image chatImage;
    private System.Windows.Media.ImageSource chatImageSource;
    private SegmentedCircle statusIndicatorCircle;
    private ButtonFlyout menuButton;
    private Button callButton;
    private ButtonFlyout combinedCallButton;
    private ColorIcon videoCallButtonIcon;
    private List<IDisposable> subscriptions = new List<IDisposable>();
    private IDisposable tooltipTimerSub;
    private IDisposable subtitleFadeOutSub;
    private IDisposable subtitleFadeInSub;
    private bool disposed;
    private bool initedOnLoaded;
    private Conversation convo;
    private string title;
    private ChatPageTitlePanel.SubtitleStates subtitleState;
    private string participantsStr;
    private bool tooltipExpired;
    private bool delayChatPicFetch;
    private SolidColorBrush foreground = UIUtils.ForegroundBrush;
    private WaStatus recentStatus;
    private List<ButtonFlyout> buttonFlyouts = new List<ButtonFlyout>();
    private ProcessedPresence lastPresence;
    private StackPanel statusPanel;
    private JidNameControl statusLineName;

    private double IconSize => this.PanelSize * (this.recentStatus == null ? 0.9 : 0.8);

    private double ButtonSize => this.PanelSize * 0.9;

    private double ButtonIconSize => this.PanelSize * 0.5;

    public string Title => this.title;

    public ProcessedPresence LastPresence
    {
      set
      {
        this.lastPresence = value;
        this.UpdateSubtitle(true, "new presence");
      }
    }

    public ChatPageTitlePanel()
    {
      double zoomMultiplier = ResolutionHelper.ZoomMultiplier;
      this.Height = this.PanelSize;
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      double num1 = this.PanelSize * 0.5;
      Grid grid = new Grid();
      grid.Margin = new Thickness(0.0, 0.0, 12.0, 0.0);
      grid.Width = this.PanelSize;
      grid.Height = this.PanelSize;
      grid.VerticalAlignment = VerticalAlignment.Center;
      grid.HorizontalAlignment = HorizontalAlignment.Left;
      grid.Background = (Brush) UIUtils.TransparentBrush;
      this.chatImagePanel = grid;
      this.chatImagePanel.Tap += new EventHandler<GestureEventArgs>(this.ChatImagePanel_Click);
      Grid.SetColumn((FrameworkElement) this.chatImagePanel, 0);
      this.Children.Add((UIElement) this.chatImagePanel);
      double iconSize = this.IconSize;
      double num2 = iconSize * 0.5;
      Image image = new Image();
      image.Stretch = Stretch.UniformToFill;
      image.Clip = (Geometry) new EllipseGeometry()
      {
        Center = new System.Windows.Point(num2, num2),
        RadiusX = num2,
        RadiusY = num2
      };
      image.Width = iconSize;
      image.Height = iconSize;
      image.VerticalAlignment = VerticalAlignment.Center;
      image.HorizontalAlignment = HorizontalAlignment.Center;
      this.chatImage = image;
      this.chatImagePanel.Children.Add((UIElement) this.chatImage);
      StackPanel stackPanel1 = new StackPanel();
      stackPanel1.Margin = new Thickness(0.0);
      stackPanel1.HorizontalAlignment = HorizontalAlignment.Stretch;
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
      linearGradientBrush.StartPoint = new System.Windows.Point(0.9, 0.0);
      linearGradientBrush.EndPoint = new System.Windows.Point(1.0, 0.0);
      stackPanel1.OpacityMask = (Brush) linearGradientBrush;
      this.textPanelContent = stackPanel1;
      JidNameControl jidNameControl1 = new JidNameControl();
      jidNameControl1.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
      jidNameControl1.RenderTransform = (Transform) new CompositeTransform();
      jidNameControl1.FontSize = (double) (int) (23.0 * zoomMultiplier + 0.5);
      jidNameControl1.FontWeight = FontWeights.Medium;
      jidNameControl1.CacheMode = (CacheMode) new BitmapCache();
      jidNameControl1.HorizontalAlignment = HorizontalAlignment.Left;
      this.titleBlock = jidNameControl1;
      this.textPanelContent.Children.Add((UIElement) this.titleBlock);
      StackPanel stackPanel2 = new StackPanel();
      stackPanel2.Orientation = Orientation.Horizontal;
      stackPanel2.HorizontalAlignment = HorizontalAlignment.Left;
      stackPanel2.Margin = new Thickness(0.0, -4.0 * zoomMultiplier, 0.0, 0.0);
      stackPanel2.RenderTransform = (Transform) new CompositeTransform();
      this.statusPanel = stackPanel2;
      TranslateTransform translateTransform = new TranslateTransform();
      TextTrimmingControl2 trimmingControl2 = new TextTrimmingControl2();
      trimmingControl2.HorizontalAlignment = HorizontalAlignment.Left;
      trimmingControl2.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
      trimmingControl2.FontSize = (double) (int) (20.0 * zoomMultiplier + 0.5);
      trimmingControl2.RenderTransform = (Transform) new CompositeTransform();
      trimmingControl2.CacheMode = (CacheMode) new BitmapCache();
      this.subtitleBlock = trimmingControl2;
      JidNameControl jidNameControl2 = new JidNameControl();
      jidNameControl2.Margin = new Thickness(0.0, 0.0, 4.0 * zoomMultiplier, 0.0);
      jidNameControl2.FontSize = this.subtitleBlock.FontSize;
      this.statusLineName = jidNameControl2;
      this.statusPanel.Children.Add((UIElement) this.statusLineName);
      this.statusPanel.Children.Add((UIElement) this.subtitleBlock);
      Grid wrapper = new Grid();
      wrapper.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = GridLength.Auto
      });
      wrapper.Children.Add((UIElement) this.statusPanel);
      wrapper.SizeChanged += (SizeChangedEventHandler) ((s, e) => wrapper.Clip = (Geometry) new RectangleGeometry()
      {
        Rect = new Rect(new System.Windows.Point(0.0, 0.0), e.NewSize)
      });
      this.textPanelContent.Children.Add((UIElement) wrapper);
      this.textPanelContent.SizeChanged += (SizeChangedEventHandler) ((s, e) => this.subtitleBlock.MaxWidth = e.NewSize.Width);
      Button button = new Button();
      button.Height = this.PanelSize;
      button.Margin = new Thickness(0.0);
      button.Padding = new Thickness(0.0);
      button.Style = Application.Current.Resources[(object) "BorderlessButton"] as Style;
      button.HorizontalAlignment = HorizontalAlignment.Stretch;
      button.HorizontalContentAlignment = HorizontalAlignment.Stretch;
      button.VerticalAlignment = VerticalAlignment.Top;
      button.VerticalContentAlignment = VerticalAlignment.Top;
      button.Content = (object) this.textPanelContent;
      button.Name = "AIdChatInfoButton";
      this.textPanel = button;
      this.textPanel.Click += new RoutedEventHandler(this.TextPanel_Click);
      Grid.SetColumn((FrameworkElement) this.textPanel, 1);
      this.Children.Add((UIElement) this.textPanel);
      ButtonFlyout buttonFlyout1 = new ButtonFlyout();
      buttonFlyout1.Name = "AidMoreMenu";
      buttonFlyout1.Height = iconSize;
      buttonFlyout1.Width = iconSize;
      buttonFlyout1.VerticalAlignment = VerticalAlignment.Center;
      buttonFlyout1.Visibility = Visibility.Collapsed;
      ButtonFlyout buttonFlyout2 = buttonFlyout1;
      FrameworkElement[] frameworkElementArray = new FrameworkElement[2];
      Ellipse ellipse = new Ellipse();
      ellipse.Fill = this.Resources[(object) "PhoneChromeBrush"] as Brush;
      frameworkElementArray[0] = (FrameworkElement) ellipse;
      TextBlock textBlock = new TextBlock();
      textBlock.Text = "⋯";
      textBlock.Style = this.Resources[(object) "PhoneTextLargeStyle"] as Style;
      textBlock.HorizontalAlignment = HorizontalAlignment.Center;
      textBlock.VerticalAlignment = VerticalAlignment.Center;
      textBlock.Margin = new Thickness(0.0, -6.0, 0.0, 0.0);
      frameworkElementArray[1] = (FrameworkElement) textBlock;
      buttonFlyout2.Content = (IEnumerable<FrameworkElement>) frameworkElementArray;
      this.menuButton = buttonFlyout1;
      this.menuButton.ContentPanel.Tap += new EventHandler<GestureEventArgs>(this.menuButton.OpenFlyout);
      this.menuButton.ContentPanel.DoubleTap += new EventHandler<GestureEventArgs>(this.menuButton.OpenFlyout);
      Grid.SetColumn((FrameworkElement) this.menuButton, 4);
      this.Children.Add((UIElement) this.menuButton);
      this.menuButton.FlyoutOpening += new EventHandler(this.ButtonFlyout_FlyoutOpening);
      this.buttonFlyouts.Add(this.menuButton);
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
    }

    public ChatPageTitlePanel(string title, System.Windows.Media.ImageSource chatPic)
      : this()
    {
      if (!string.IsNullOrEmpty(title))
        this.titleBlock.Set(new RichTextBlock.TextSet()
        {
          Text = title.ToUpper()
        }, false);
      if (chatPic == null)
        return;
      this.chatImage.Source = this.chatImageSource = chatPic;
      this.chatImagePanel.Visibility = Visibility.Visible;
      this.delayChatPicFetch = true;
    }

    public void Render(Conversation c)
    {
      this.ClearSubscriptions();
      this.convo = c;
      if (this.convo == null)
        return;
      this.UpdateVoipButtons();
      Action a = (Action) (() =>
      {
        if (this.disposed)
          return;
        this.subscriptions.Add(ChatPictureStore.Get(this.convo.Jid, false, false, true, this.chatImageSource == null ? ChatPictureStore.SubMode.Default : ChatPictureStore.SubMode.TrackChange).SubscribeOn<ChatPictureStore.PicState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<ChatPictureStore.PicState>().Subscribe<ChatPictureStore.PicState>((Action<ChatPictureStore.PicState>) (picState =>
        {
          if (this.disposed)
            return;
          if (picState == null || picState.Image == null)
          {
            if (this.recentStatus == null)
            {
              this.chatImageSource = (System.Windows.Media.ImageSource) null;
              this.chatImage.Source = (System.Windows.Media.ImageSource) AssetStore.GetDefaultChatIcon(this.convo.Jid);
              this.chatImagePanel.Visibility = Visibility.Visible;
            }
            else
              this.UpdateStatusIndication();
          }
          else
          {
            this.chatImage.Source = this.chatImageSource = (System.Windows.Media.ImageSource) picState.Image;
            this.chatImagePanel.Visibility = Visibility.Visible;
          }
        })));
      });
      JidHelper.JidTypes jidType = JidHelper.GetJidType(this.convo.Jid);
      if (jidType == JidHelper.JidTypes.Psa)
      {
        this.chatImage.Source = this.chatImageSource = (System.Windows.Media.ImageSource) AssetStore.WhatsAppAvatar;
        this.chatImagePanel.Visibility = Visibility.Visible;
      }
      else if (this.delayChatPicFetch)
        this.Dispatcher.RunAfterDelay(TimeSpan.FromSeconds(2.0), a);
      else
        a();
      Set<string> trackedProperties = new Set<string>();
      switch (jidType)
      {
        case JidHelper.JidTypes.User:
          this.subscriptions.Add(ContactStore.ContactsUpdatedSubject.ObserveOnDispatcher<string[]>().Where<string[]>((Func<string[], bool>) (updatedJids => ((IEnumerable<string>) updatedJids).Contains<string>(this.convo.Jid))).Subscribe<string[]>((Action<string[]>) (_ => this.UpdateTitle())));
          this.subscriptions.Add(this.GetRecentStatusObservable(this.convo.Jid).SubscribeOn<WaStatus>(WAThreadPool.Scheduler).ObserveOnDispatcher<WaStatus>().Subscribe<WaStatus>((Action<WaStatus>) (s =>
          {
            this.recentStatus = s;
            this.UpdateStatusIndication();
          })));
          break;
        case JidHelper.JidTypes.Group:
          trackedProperties.Add("GroupSubject");
          this.subscriptions.Add(FunEventHandler.Events.GroupMembershipUpdatedSubject.Where<FunEventHandler.Events.ConversationWithFlags>((Func<FunEventHandler.Events.ConversationWithFlags, bool>) (args => args.Conversation != null && this.convo != null && args.Conversation.Jid == this.convo.Jid && args.MembersChanged)).ObserveOnDispatcher<FunEventHandler.Events.ConversationWithFlags>().Subscribe<FunEventHandler.Events.ConversationWithFlags>((Action<FunEventHandler.Events.ConversationWithFlags>) (_ =>
          {
            this.participantsStr = (string) null;
            this.UpdateSubtitle(false, "participants changed");
          })));
          break;
        case JidHelper.JidTypes.Broadcast:
          trackedProperties.Add("GroupSubject");
          break;
      }
      if (trackedProperties.Any<string>())
        this.subscriptions.Add(this.convo.GetPropertyChangedAsync().Where<PropertyChangedEventArgs>((Func<PropertyChangedEventArgs, bool>) (arg => trackedProperties.Contains(arg.PropertyName))).ObserveOnDispatcher<PropertyChangedEventArgs>().Subscribe<PropertyChangedEventArgs>((Action<PropertyChangedEventArgs>) (arg =>
        {
          if (!(arg.PropertyName == "GroupSubject"))
            return;
          this.UpdateTitle();
        })));
      this.UpdateTitle();
      this.UpdateSubtitle(false, "init");
    }

    public void EnableVoipButtons(bool enable)
    {
      if (this.callButton != null)
      {
        this.callButton.IsEnabled = enable;
        this.callButton.Opacity = enable ? 1.0 : 0.35;
      }
      if (this.combinedCallButton == null)
        return;
      this.combinedCallButton.IsEnabled = enable;
      this.combinedCallButton.Opacity = enable ? 1.0 : 0.35;
    }

    public void SetForeground(SolidColorBrush brush)
    {
      this.titleBlock.Foreground = (Brush) (this.foreground = brush);
      if (this.lastPresence != null && (this.lastPresence.Presence == Presence.OnlineAndTyping || this.lastPresence.Presence == Presence.OnlineAndRecording))
      {
        this.subtitleBlock.Foreground = (Brush) UIUtils.AccentBrush;
        this.subtitleBlock.Opacity = 1.0;
      }
      else
      {
        this.subtitleBlock.Foreground = (Brush) brush;
        this.subtitleBlock.Opacity = 0.65;
      }
    }

    public void UpdateVoipButtons()
    {
      if (!this.initedOnLoaded)
        return;
      if (!this.convo.IsUserChat())
      {
        if (this.callButton == null)
          return;
        this.callButton.Visibility = Visibility.Collapsed;
      }
      else
      {
        if (this.combinedCallButton != null)
          return;
        double num = this.PanelSize * 0.6;
        ColorIcon colorIcon = new ColorIcon();
        colorIcon.Source = (System.Windows.Media.ImageSource) AssetStore.PhoneIconWhite;
        colorIcon.Width = this.IconSize;
        colorIcon.Height = this.IconSize;
        colorIcon.IconBackground = this.Resources[(object) "PhoneChromeBrush"] as Brush;
        colorIcon.IconWidth = num;
        colorIcon.IconHeight = num;
        this.videoCallButtonIcon = colorIcon;
        ButtonFlyout buttonFlyout = new ButtonFlyout();
        buttonFlyout.Name = "AidCallMenu";
        buttonFlyout.Margin = new Thickness(0.0, 0.0, 8.0, 0.0);
        buttonFlyout.Height = this.IconSize;
        buttonFlyout.Width = this.IconSize;
        buttonFlyout.VerticalAlignment = VerticalAlignment.Center;
        buttonFlyout.Content = (IEnumerable<FrameworkElement>) new FrameworkElement[1]
        {
          (FrameworkElement) this.videoCallButtonIcon
        };
        this.combinedCallButton = buttonFlyout;
        this.combinedCallButton.Actions = (IEnumerable<FlyoutCommand>) new FlyoutCommand[2]
        {
          new FlyoutCommand(AppResources.InitiateVoiceCall, true, (Action) (() => this.CallButton_Click((object) null, (EventArgs) null)), "AidVoiceCallButton"),
          new FlyoutCommand(AppResources.InitiateVideoCall, true, (Action) (() => this.VideoCallButton_Click((object) null, (EventArgs) null)), "AidVideoCallButton")
        };
        this.combinedCallButton.ContentPanel.Tap += new EventHandler<GestureEventArgs>(this.combinedCallButton.OpenFlyout);
        this.combinedCallButton.ContentPanel.DoubleTap += new EventHandler<GestureEventArgs>(this.combinedCallButton.OpenFlyout);
        Grid.SetColumn((FrameworkElement) this.combinedCallButton, 2);
        this.Children.Add((UIElement) this.combinedCallButton);
        this.combinedCallButton.FlyoutOpening += new EventHandler(this.ButtonFlyout_FlyoutOpening);
        this.buttonFlyouts.Add(this.combinedCallButton);
      }
    }

    private void ButtonFlyout_FlyoutOpening(object sender, EventArgs e)
    {
      if (!(sender is ButtonFlyout))
        return;
      foreach (ButtonFlyout buttonFlyout in this.buttonFlyouts)
      {
        if (buttonFlyout != sender && buttonFlyout.FlyoutIsOpen)
          buttonFlyout.FlyoutIsOpen = false;
      }
    }

    private IObservable<WaStatus> GetRecentStatusObservable(string jid)
    {
      return !JidHelper.IsUserJid(jid) ? Observable.Empty<WaStatus>() : Observable.Create<WaStatus>((Func<IObserver<WaStatus>, Action>) (observer =>
      {
        IDisposable newStatusSub = (IDisposable) null;
        IDisposable statusSeenSub = (IDisposable) null;
        WaStatus lastStatus = (WaStatus) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          lastStatus = db.GetLastestStatus(jid);
          if (lastStatus != null)
            statusSeenSub = lastStatus.GetPropertyChangedAsync().Where<PropertyChangedEventArgs>((Func<PropertyChangedEventArgs, bool>) (prop => prop.PropertyName == "IsViewed")).Subscribe<PropertyChangedEventArgs>((Action<PropertyChangedEventArgs>) (_ =>
            {
              observer.OnNext(lastStatus);
              statusSeenSub.SafeDispose();
              statusSeenSub = (IDisposable) null;
            }));
          newStatusSub = db.NewMessagesSubject.Where<Message>((Func<Message, bool>) (m => m.IsStatus() && m.GetSenderJid() == jid)).ObserveOn<Message>((IScheduler) AppState.Worker).Subscribe<Message>((Action<Message>) (m =>
          {
            WaStatus lastStatus2 = (WaStatus) null;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db2 => lastStatus2 = db2.GetLastestStatus(jid)));
            statusSeenSub.SafeDispose();
            statusSeenSub = (IDisposable) null;
            if (lastStatus2 != null)
              statusSeenSub = lastStatus2.GetPropertyChangedAsync().Where<PropertyChangedEventArgs>((Func<PropertyChangedEventArgs, bool>) (prop => prop.PropertyName == "IsViewed")).Subscribe<PropertyChangedEventArgs>((Action<PropertyChangedEventArgs>) (_ =>
              {
                observer.OnNext(lastStatus2);
                statusSeenSub.SafeDispose();
                statusSeenSub = (IDisposable) null;
              }));
            observer.OnNext(lastStatus2);
          }));
        }));
        observer.OnNext(lastStatus);
        return (Action) (() =>
        {
          newStatusSub.SafeDispose();
          newStatusSub = (IDisposable) null;
          statusSeenSub.SafeDispose();
          statusSeenSub = (IDisposable) null;
        });
      }));
    }

    private void UpdateStatusIndication()
    {
      double iconSize = this.IconSize;
      if (this.recentStatus == null)
      {
        if (this.statusIndicatorCircle != null)
          this.statusIndicatorCircle.Visibility = Visibility.Collapsed;
      }
      else
      {
        Brush brush = this.recentStatus.IsViewed ? UIUtils.PhoneChromeBrush : (Brush) UIUtils.AccentBrush;
        if (this.chatImageSource == null)
        {
          this.chatImage.Source = (System.Windows.Media.ImageSource) AssetStore.GetDefaultChatIcon(this.convo.Jid);
          this.chatImagePanel.Visibility = Visibility.Visible;
        }
        if (this.statusIndicatorCircle == null)
        {
          double num1 = 3.0;
          double num2 = Math.Max(0.0, (this.PanelSize - iconSize) / 2.0 - num1);
          SegmentedCircle segmentedCircle = new SegmentedCircle();
          segmentedCircle.StrokeThickness = 2.0;
          segmentedCircle.Radius = iconSize / 2.0 + num1;
          segmentedCircle.Margin = new Thickness(num2, num2, 0.0, 0.0);
          segmentedCircle.Fill = (Brush) UIUtils.TransparentBrush;
          segmentedCircle.SegmentCount = 1;
          segmentedCircle.FillCount = 0;
          segmentedCircle.VerticalAlignment = VerticalAlignment.Top;
          segmentedCircle.HorizontalAlignment = HorizontalAlignment.Left;
          this.statusIndicatorCircle = segmentedCircle;
          this.chatImagePanel.Children.Add((UIElement) this.statusIndicatorCircle);
        }
        this.statusIndicatorCircle.Background = brush;
        this.statusIndicatorCircle.InvalidateMeasure();
        this.statusIndicatorCircle.Visibility = Visibility.Visible;
      }
      this.chatImage.Width = this.chatImage.Height = iconSize;
      double num = iconSize * 0.5;
      this.chatImage.Clip = (Geometry) new EllipseGeometry()
      {
        Center = new System.Windows.Point(num, num),
        RadiusX = num,
        RadiusY = num
      };
    }

    public void UpdateTitle()
    {
      this.statusLineName.Visibility = Visibility.Collapsed;
      JidHelper.JidTypes? jidType = this.convo?.JidType;
      if (!jidType.HasValue)
        return;
      switch (jidType.GetValueOrDefault())
      {
        case JidHelper.JidTypes.User:
          this.UpdateTitle_UserChat();
          break;
        case JidHelper.JidTypes.Group:
          this.UpdateGroup_ChatTitle();
          break;
        case JidHelper.JidTypes.Psa:
          this.UpdateTitle_PsaChat();
          break;
        default:
          this.UpdateTitle_DefaultChat();
          break;
      }
    }

    private void UpdateTitle_UserChat()
    {
      UserStatus user = UserCache.Get(this.convo.Jid, true);
      if (VerifiedNameRules.IsApplicable(user))
      {
        bool checkMark = false;
        this.title = VerifiedNameRules.GetFirstChatName(user, out checkMark);
        this.titleBlock.Set(this.ToTextSet(this.title), checkMark);
        string secondChatName = VerifiedNameRules.GetSecondChatName(user, out checkMark);
        if (secondChatName != null)
        {
          this.statusLineName.Set(this.ToTextSet(secondChatName), checkMark);
          this.statusLineName.Visibility = Visibility.Visible;
        }
        else
          this.statusLineName.Visibility = Visibility.Collapsed;
        this.textPanelContent.OpacityMask = (Brush) null;
      }
      else
      {
        this.title = this.convo.GetName(true);
        this.titleBlock.Set(this.ToTextSet(this.title), false);
      }
    }

    private void UpdateTitle_PsaChat()
    {
      this.title = this.convo.GetName(true);
      this.titleBlock.Set(this.ToTextSet(this.title), true);
      this.textPanelContent.OpacityMask = (Brush) null;
    }

    private void UpdateGroup_ChatTitle()
    {
      if (this.convo.GroupSubject == null || this.convo.GroupSubject == AppResources.GroupChatSubject)
        App.CurrentApp.Connection.SendGetGroupInfo(this.convo.Jid);
      this.title = this.convo.GetName(true);
      this.titleBlock.Set(this.ToTextSet(this.title), false);
    }

    private void UpdateTitle_DefaultChat()
    {
      this.title = this.convo.GetName(true);
      this.titleBlock.Set(this.ToTextSet(this.title), false);
    }

    private RichTextBlock.TextSet ToTextSet(string value)
    {
      return new RichTextBlock.TextSet()
      {
        Text = (value ?? "").ToUpper(),
        SerializedFormatting = this.convo.GetGroupSubjectPerformanceHint()
      };
    }

    private void UpdateSubtitle(bool animate, string context = null)
    {
      if (this.convo == null)
        return;
      ChatPageTitlePanel.SubtitleStates newState = ChatPageTitlePanel.SubtitleStates.Undefined;
      string subtitle = (string) null;
      FontFamily fontFamily = (FontFamily) null;
      SolidColorBrush brush = this.foreground;
      double opacity = 0.65;
      TextTrimmingControl2.TextTrimmingMode trimMode = TextTrimmingControl2.TextTrimmingMode.None;
      string prefix = (string) null;
      if (this.lastPresence != null && (this.lastPresence.Presence == Presence.OnlineAndTyping || this.lastPresence.Presence == Presence.OnlineAndRecording))
      {
        newState = ChatPageTitlePanel.SubtitleStates.Activity;
        subtitle = this.lastPresence.PresenceString;
        trimMode = TextTrimmingControl2.TextTrimmingMode.TrimPrefix;
        brush = UIUtils.AccentBrush;
        opacity = 1.0;
        fontFamily = Application.Current.Resources[(object) "PhoneFontFamilySemiBold"] as FontFamily;
      }
      else if (this.ShouldShowTooltip())
      {
        newState = ChatPageTitlePanel.SubtitleStates.Tooltip;
        trimMode = TextTrimmingControl2.TextTrimmingMode.None;
        switch (JidHelper.GetJidType(this.convo.Jid))
        {
          case JidHelper.JidTypes.User:
            subtitle = AppResources.TapForContactInfo;
            break;
          case JidHelper.JidTypes.Group:
            subtitle = this.convo.IsAnnounceOnly() ? AppResources.AnnouncementOnlyGroupChatSubtitleHint : AppResources.TapForGroupInfo;
            break;
          case JidHelper.JidTypes.Broadcast:
            subtitle = AppResources.TapForBroadcastListInfo;
            break;
          case JidHelper.JidTypes.Psa:
            subtitle = AppResources.TapForAnnouncementInfo;
            break;
        }
        if (!this.tooltipExpired && this.tooltipTimerSub == null)
        {
          TimeSpan dueTime = TimeSpan.FromMilliseconds(2500.0);
          if (this.statusLineName.Visibility == Visibility.Visible)
            dueTime += TimeSpan.FromSeconds(2.0);
          this.tooltipTimerSub = Observable.Timer(dueTime, WAThreadPool.Scheduler).Take<long>(1).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
          {
            this.tooltipExpired = true;
            this.tooltipTimerSub.SafeDispose();
            this.tooltipTimerSub = (IDisposable) null;
            this.UpdateSubtitle(true, "tooltip expired");
          }));
          if (this.convo.IsMultiParticipantsChat())
            this.FetchParticipants();
        }
      }
      else
      {
        switch (JidHelper.GetJidType(this.convo.Jid))
        {
          case JidHelper.JidTypes.User:
            if (this.lastPresence != null)
            {
              subtitle = this.lastPresence.PresenceString;
              prefix = this.lastPresence.PresencePrefix;
              trimMode = TextTrimmingControl2.TextTrimmingMode.TrimPrefix;
            }
            newState = string.IsNullOrEmpty(subtitle) ? ChatPageTitlePanel.SubtitleStates.None : ChatPageTitlePanel.SubtitleStates.Static;
            break;
          case JidHelper.JidTypes.Group:
          case JidHelper.JidTypes.Broadcast:
            if (this.participantsStr == null)
            {
              this.FetchParticipants();
              break;
            }
            subtitle = this.participantsStr;
            newState = ChatPageTitlePanel.SubtitleStates.Static;
            trimMode = TextTrimmingControl2.TextTrimmingMode.Ellipses;
            break;
          case JidHelper.JidTypes.Psa:
            subtitle = AppResources.OfficialAnnouncementsSubtitle;
            trimMode = TextTrimmingControl2.TextTrimmingMode.None;
            newState = ChatPageTitlePanel.SubtitleStates.Static;
            break;
        }
      }
      if (newState == ChatPageTitlePanel.SubtitleStates.Undefined)
        return;
      if (fontFamily == null)
        fontFamily = Application.Current.Resources[(object) "PhoneFontFamilyNormal"] as FontFamily;
      if (subtitle == null)
        subtitle = "";
      ChatPageTitlePanel.SubtitleStates prevState = this.subtitleState;
      if (newState == prevState && newState != ChatPageTitlePanel.SubtitleStates.Static)
        animate = false;
      this.subtitleState = newState;
      Action update = (Action) (() =>
      {
        if (newState == ChatPageTitlePanel.SubtitleStates.None)
        {
          this.subtitleBlock.Text = (string) null;
          this.subtitleBlock.Mode = TextTrimmingControl2.TextTrimmingMode.None;
          this.subtitleBlock.Visibility = Visibility.Collapsed;
          this.textPanel.VerticalContentAlignment = VerticalAlignment.Center;
        }
        else
        {
          if (prevState == ChatPageTitlePanel.SubtitleStates.None)
          {
            this.subtitleBlock.Visibility = Visibility.Visible;
            this.textPanel.VerticalContentAlignment = VerticalAlignment.Top;
          }
          this.subtitleBlock.Foreground = (Brush) brush;
          this.subtitleBlock.Opacity = opacity;
          this.subtitleBlock.FontFamily = fontFamily;
          this.subtitleBlock.Text = subtitle;
          this.subtitleBlock.Mode = trimMode;
          this.subtitleBlock.Prefix = prefix;
        }
      });
      this.subtitleFadeOutSub.SafeDispose();
      this.subtitleFadeInSub.SafeDispose();
      this.subtitleFadeInSub = this.subtitleFadeOutSub = (IDisposable) null;
      if (animate)
      {
        Storyboard storyboard = WaAnimations.CreateStoryboard(WaAnimations.Fade(this.subtitleBlock.Opacity, 0.0, TimeSpan.FromMilliseconds(200.0), (DependencyObject) this.subtitleBlock));
        Storyboard fadeInSb = WaAnimations.CreateStoryboard(WaAnimations.Fade(0.0, opacity, TimeSpan.FromMilliseconds(200.0), (DependencyObject) this.subtitleBlock));
        this.subtitleFadeOutSub = Storyboarder.PerformWithDisposable(storyboard, (DependencyObject) null, true, (Action) (() =>
        {
          this.subtitleFadeOutSub = (IDisposable) null;
          update();
          if (newState == ChatPageTitlePanel.SubtitleStates.None)
            return;
          this.subtitleBlock.Opacity = 0.0;
          this.subtitleFadeInSub = Storyboarder.PerformWithDisposable(fadeInSb, onComplete: (Action) (() =>
          {
            this.subtitleBlock.Opacity = opacity;
            this.subtitleFadeOutSub = (IDisposable) null;
          }), callOnCompleteOnDisposing: true);
        }), false, (string) null);
      }
      else
        update();
    }

    private bool ShouldShowTooltip()
    {
      bool flag = true;
      if (this.tooltipExpired && (this.convo == null || !this.convo.IsMultiParticipantsChat() || !string.IsNullOrEmpty(this.participantsStr)))
        flag = false;
      return flag;
    }

    private void FetchParticipants()
    {
      if (this.convo == null || this.participantsStr != null)
        return;
      this.participantsStr = "";
      WAThreadPool.Scheduler.Schedule((Action) (() =>
      {
        string participants = Emoji.ConvertToTextOnly(this.convo.GetParticipantNames(8), (byte[]) null);
        if (participants == null)
        {
          if (!this.convo.IsGroup())
            return;
          App.CurrentApp.Connection.SendGetGroupInfo(this.convo.Jid);
        }
        else
          Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
          {
            this.participantsStr = participants;
            this.UpdateSubtitle(false, "participants fetched");
          }));
      }));
    }

    private void ClearSubscriptions()
    {
      IDisposable[] array = this.subscriptions.ToArray();
      this.subscriptions.Clear();
      foreach (IDisposable d in array)
        d.SafeDispose();
      this.subtitleFadeInSub.SafeDispose();
      this.subtitleFadeInSub = (IDisposable) null;
      this.subtitleFadeOutSub.SafeDispose();
      this.subtitleFadeOutSub = (IDisposable) null;
      this.tooltipTimerSub.SafeDispose();
      this.tooltipTimerSub = (IDisposable) null;
    }

    public void Dispose()
    {
      this.disposed = true;
      this.ClearSubscriptions();
      this.chatImage.Source = (System.Windows.Media.ImageSource) null;
      this.chatImageSource = (System.Windows.Media.ImageSource) null;
    }

    private void OnLoaded(object sender, EventArgs e)
    {
      if (this.initedOnLoaded)
        return;
      this.initedOnLoaded = true;
      this.AnimateVerifiedStatus();
      this.UpdateVoipButtons();
    }

    private async void AnimateVerifiedStatus()
    {
      if (this.statusLineName.Visibility != Visibility.Visible)
        return;
      await Task.Delay(1000);
      if (this.statusLineName.Visibility != Visibility.Visible)
        return;
      double actualWidth = this.statusLineName.ActualWidth;
      Thickness margin = this.statusLineName.Margin;
      double left = margin.Left;
      double num = actualWidth + left;
      margin = this.statusLineName.Margin;
      double right = margin.Right;
      WaAnimations.CreateStoryboard(WaAnimations.HorizontalSlide(0.0, -(num + right), TimeSpan.FromMilliseconds(1000.0), (DependencyObject) this.statusPanel)).Begin();
    }

    private void ChatImagePanel_Click(object sender, EventArgs e)
    {
      if (this.recentStatus == null)
      {
        this.TextPanel_Click((object) null, (EventArgs) null);
      }
      else
      {
        UserCache.Get(this.convo.Jid, true);
        WaStatusViewPage.Start(new WaStatusThread(this.convo.Jid, this.recentStatus), (WaStatusThread[]) null, !this.recentStatus.IsViewed);
      }
    }

    private void TextPanel_Click(object sender, EventArgs e)
    {
      if (InAppFloatingBanner.IsShowing)
        return;
      switch (JidHelper.GetJidType(this.convo?.Jid))
      {
        case JidHelper.JidTypes.User:
          ContactInfoPage.Start(UserCache.Get(this.convo?.Jid, true), this.chatImageSource);
          break;
        case JidHelper.JidTypes.Group:
          GroupInfoPage.Start((NavigationService) null, this.convo);
          break;
        case JidHelper.JidTypes.Broadcast:
          BroadcastListInfoPage.Start((NavigationService) null, this.convo?.Jid);
          break;
        case JidHelper.JidTypes.Psa:
          PsaInfoPage.Start((NavigationService) null, this.convo);
          break;
      }
    }

    private void VideoCallButton_Click(object sender, EventArgs e)
    {
      if (this.convo == null)
        return;
      CallContact.VideoCall(this.convo.Jid);
    }

    private void CallButton_Click(object sender, EventArgs e)
    {
      if (this.convo == null)
        return;
      CallContact.Call(this.convo.Jid, context: "from chat title");
    }

    public void SetMoreActions(IEnumerable<FlyoutCommand> actions)
    {
      this.menuButton.Actions = actions;
      this.menuButton.Visibility = actions.Any<FlyoutCommand>().ToVisibility();
    }

    public void UpdateMoreAction(FlyoutCommand action) => this.menuButton.UpdateAction(action);

    private enum SubtitleStates
    {
      Undefined,
      None,
      Activity,
      Tooltip,
      Static,
    }
  }
}
