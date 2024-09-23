// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.ChatBubble
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public class ChatBubble : ContentControl
  {
    public static readonly DependencyProperty ChatBubbleDirectionProperty = DependencyProperty.Register(nameof (ChatBubbleDirection), typeof (ChatBubbleDirection), typeof (ChatBubble), new PropertyMetadata((object) ChatBubbleDirection.UpperRight, new PropertyChangedCallback(ChatBubble.OnChatBubbleDirectionChanged)));

    public ChatBubble()
    {
      ((Control) this).DefaultStyleKey = (object) typeof (ChatBubble);
      ((Control) this).IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.ChatBubbleIsEnabledChanged);
    }

    private void ChatBubbleIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.UpdateIsEnabledVisualState();
    }

    public virtual void OnApplyTemplate()
    {
      ((FrameworkElement) this).OnApplyTemplate();
      this.UpdateChatBubbleDirection();
      this.UpdateIsEnabledVisualState();
    }

    public ChatBubbleDirection ChatBubbleDirection
    {
      get
      {
        return (ChatBubbleDirection) ((DependencyObject) this).GetValue(ChatBubble.ChatBubbleDirectionProperty);
      }
      set
      {
        ((DependencyObject) this).SetValue(ChatBubble.ChatBubbleDirectionProperty, (object) value);
      }
    }

    private static void OnChatBubbleDirectionChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(d is ChatBubble chatBubble))
        return;
      chatBubble.UpdateChatBubbleDirection();
    }

    private void UpdateChatBubbleDirection()
    {
      VisualStateManager.GoToState((Control) this, this.ChatBubbleDirection.ToString(), true);
    }

    private void UpdateIsEnabledVisualState()
    {
      VisualStateManager.GoToState((Control) this, ((Control) this).IsEnabled ? "Normal" : "Disabled", true);
    }
  }
}
