// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageViewPanel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace WhatsApp
{
  public abstract class MessageViewPanel : Grid
  {
    protected const string LogHeader = "msgbubble";
    protected static double cachedZoomMultiplier = ResolutionHelper.ZoomMultiplier;
    protected double zoomMultiplier = MessageViewPanel.cachedZoomMultiplier;
    private static Dictionary<MessageViewPanel.ViewTypes, Stack<MessageViewPanel>> cachedInstances = new Dictionary<MessageViewPanel.ViewTypes, Stack<MessageViewPanel>>();
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.RegisterAttached(nameof (ViewModel), typeof (object), typeof (MessageViewPanel), new PropertyMetadata((object) null, new PropertyChangedCallback(MessageViewPanel.OnViewModelChanged)));
    protected MessageViewModel viewModel;

    public virtual MessageViewPanel.ViewTypes ViewType => MessageViewPanel.ViewTypes.Undefined;

    public object ViewModel
    {
      get => this.GetValue(MessageViewPanel.ViewModelProperty);
      set => this.SetValue(MessageViewPanel.ViewModelProperty, value);
    }

    ~MessageViewPanel() => this.DisposeSubscriptions();

    public static MessageViewPanel.ViewTypes GetViewType(Message m)
    {
      if (!m.IsViewingSupported())
        return MessageViewPanel.ViewTypes.Unsupported;
      MessageViewPanel.ViewTypes viewType = MessageViewPanel.ViewTypes.Undefined;
      switch (m.MediaWaType)
      {
        case FunXMPP.FMessage.Type.Undefined:
          viewType = !LargeEmojiMessageViewModel.ShouldShowAsLargeEmoji(m) ? MessageViewPanel.ViewTypes.Text : MessageViewPanel.ViewTypes.LargeEmoji;
          break;
        case FunXMPP.FMessage.Type.Image:
        case FunXMPP.FMessage.Type.Video:
          viewType = MessageViewPanel.ViewTypes.ImageAndVideo;
          break;
        case FunXMPP.FMessage.Type.Audio:
          viewType = MessageViewPanel.ViewTypes.Audio;
          break;
        case FunXMPP.FMessage.Type.Contact:
          viewType = MessageViewPanel.ViewTypes.Contact;
          break;
        case FunXMPP.FMessage.Type.Location:
          viewType = MessageViewPanel.ViewTypes.Location;
          break;
        case FunXMPP.FMessage.Type.Divider:
          viewType = MessageViewPanel.ViewTypes.UnreadDivider;
          break;
        case FunXMPP.FMessage.Type.System:
          viewType = MessageViewPanel.ViewTypes.System;
          break;
        case FunXMPP.FMessage.Type.Document:
          viewType = MessageViewPanel.ViewTypes.Document;
          break;
        case FunXMPP.FMessage.Type.ExtendedText:
          if (UrlMessageViewModel.ShouldRenderAsUrlMessage(m))
          {
            viewType = MessageViewPanel.ViewTypes.Url;
            break;
          }
          break;
        case FunXMPP.FMessage.Type.Gif:
          viewType = MessageViewPanel.ViewTypes.InlineVideo;
          break;
        case FunXMPP.FMessage.Type.LiveLocation:
          viewType = MessageViewPanel.ViewTypes.LiveLocation;
          break;
        case FunXMPP.FMessage.Type.Sticker:
          viewType = MessageViewPanel.ViewTypes.Sticker;
          break;
        case FunXMPP.FMessage.Type.Revoked:
          viewType = MessageViewPanel.ViewTypes.Revoked;
          break;
      }
      return viewType;
    }

    public static MessageViewPanel Get(MessageViewPanel.ViewTypes viewType)
    {
      MessageViewPanel messageViewPanel = (MessageViewPanel) null;
      Stack<MessageViewPanel> source = (Stack<MessageViewPanel>) null;
      if (MessageViewPanel.cachedInstances.TryGetValue(viewType, out source) && source != null && source.Any<MessageViewPanel>())
        messageViewPanel = source.Pop();
      if (messageViewPanel == null)
      {
        switch (viewType)
        {
          case MessageViewPanel.ViewTypes.Text:
            messageViewPanel = (MessageViewPanel) new TextMessageViewPanel();
            break;
          case MessageViewPanel.ViewTypes.ImageAndVideo:
            messageViewPanel = (MessageViewPanel) new ImageMessageViewPanel();
            break;
          case MessageViewPanel.ViewTypes.Audio:
            messageViewPanel = (MessageViewPanel) new AudioMessageViewPanel();
            break;
          case MessageViewPanel.ViewTypes.Location:
            messageViewPanel = (MessageViewPanel) new LocationMessageViewPanel();
            break;
          case MessageViewPanel.ViewTypes.LiveLocation:
            messageViewPanel = (MessageViewPanel) new LiveLocationMessageViewPanel();
            break;
          case MessageViewPanel.ViewTypes.Contact:
            messageViewPanel = (MessageViewPanel) new ContactMessageViewPanel();
            break;
          case MessageViewPanel.ViewTypes.Document:
            messageViewPanel = (MessageViewPanel) new DocumentMessageViewPanel();
            break;
          case MessageViewPanel.ViewTypes.Url:
            messageViewPanel = (MessageViewPanel) new UrlMessageViewPanel();
            break;
          case MessageViewPanel.ViewTypes.Quote:
            messageViewPanel = (MessageViewPanel) new QuotedMessageViewPanel();
            break;
          case MessageViewPanel.ViewTypes.LargeEmoji:
            messageViewPanel = (MessageViewPanel) new LargeEmojiMessageViewPanel();
            break;
          case MessageViewPanel.ViewTypes.InlineVideo:
            messageViewPanel = (MessageViewPanel) new InlineVideoMessageViewPanel();
            break;
          case MessageViewPanel.ViewTypes.System:
            messageViewPanel = (MessageViewPanel) new SystemMessageViewPanel();
            break;
          case MessageViewPanel.ViewTypes.UnreadDivider:
            messageViewPanel = (MessageViewPanel) new UnreadDividerViewPanel();
            break;
          case MessageViewPanel.ViewTypes.Unsupported:
            messageViewPanel = (MessageViewPanel) new UnsupportedMessageViewPanel();
            break;
          case MessageViewPanel.ViewTypes.Revoked:
            messageViewPanel = (MessageViewPanel) new RevokedMessageViewPanel();
            break;
          case MessageViewPanel.ViewTypes.Payment:
            messageViewPanel = (MessageViewPanel) new PaymentMessageViewPanel();
            break;
          case MessageViewPanel.ViewTypes.Sticker:
            messageViewPanel = (MessageViewPanel) new StickerMessageViewPanel();
            break;
        }
      }
      return messageViewPanel ?? (MessageViewPanel) new TextMessageViewPanel();
    }

    public static void Save(MessageViewPanel instance)
    {
      if (instance == null)
        return;
      Stack<MessageViewPanel> messageViewPanelStack = (Stack<MessageViewPanel>) null;
      if (!MessageViewPanel.cachedInstances.TryGetValue(instance.ViewType, out messageViewPanelStack) || messageViewPanelStack == null)
        MessageViewPanel.cachedInstances[instance.ViewType] = messageViewPanelStack = new Stack<MessageViewPanel>();
      if (messageViewPanelStack.Count >= 10)
        return;
      messageViewPanelStack.Push(instance);
    }

    public static void ClearCache() => MessageViewPanel.cachedInstances.Clear();

    public static void LogCache()
    {
      foreach (KeyValuePair<MessageViewPanel.ViewTypes, Stack<MessageViewPanel>> cachedInstance in MessageViewPanel.cachedInstances)
        Log.p("msgbubble", "cached instances | type:{0} count:{1}", (object) cachedInstance.Key, (object) cachedInstance.Value);
    }

    public virtual void Render(MessageViewModel vm) => this.viewModel = vm;

    public virtual void Cleanup()
    {
      this.DisposeSubscriptions();
      this.viewModel = (MessageViewModel) null;
    }

    protected virtual void DisposeSubscriptions()
    {
    }

    protected void ShowElement(FrameworkElement elem, bool show)
    {
      if (elem == null)
        return;
      elem.Visibility = show.ToVisibility();
    }

    protected void ShowElements(IEnumerable<FrameworkElement> elems, bool show)
    {
      foreach (FrameworkElement elem in elems)
        this.ShowElement(elem, show);
    }

    public virtual void ProcessViewModelNotification(KeyValuePair<string, object> args)
    {
    }

    public static void OnViewModelChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs args)
    {
      if (!(sender is MessageViewPanel messageViewPanel))
        return;
      try
      {
        messageViewPanel.Render(args.NewValue as MessageViewModel);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "render new view model on view panel");
      }
    }

    public enum ViewTypes
    {
      Undefined,
      Text,
      ImageAndVideo,
      Audio,
      Location,
      LiveLocation,
      Contact,
      Document,
      Url,
      Quote,
      LargeEmoji,
      InlineVideo,
      System,
      UnreadDivider,
      Unsupported,
      Revoked,
      Payment,
      Sticker,
    }
  }
}
