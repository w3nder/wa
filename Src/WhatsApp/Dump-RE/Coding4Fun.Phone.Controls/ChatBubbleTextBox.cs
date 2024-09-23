// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.ChatBubbleTextBox
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public class ChatBubbleTextBox : TextBox
  {
    private const string HintContentElementName = "HintContentElement";
    protected ContentControl HintContentElement;
    private bool _hasFocus;
    public static readonly DependencyProperty ChatBubbleDirectionProperty = DependencyProperty.Register(nameof (ChatBubbleDirection), typeof (ChatBubbleDirection), typeof (ChatBubbleTextBox), new PropertyMetadata((object) ChatBubbleDirection.UpperRight, new PropertyChangedCallback(ChatBubbleTextBox.OnChatBubbleDirectionChanged)));
    public static readonly DependencyProperty HintProperty = DependencyProperty.Register(nameof (Hint), typeof (string), typeof (ChatBubbleTextBox), new PropertyMetadata((object) ""));
    public static readonly DependencyProperty HintStyleProperty = DependencyProperty.Register(nameof (HintStyle), typeof (Style), typeof (ChatBubbleTextBox), new PropertyMetadata((PropertyChangedCallback) null));

    public ChatBubbleTextBox()
    {
      ((Control) this).DefaultStyleKey = (object) typeof (ChatBubbleTextBox);
      this.TextChanged += new TextChangedEventHandler(this.ChatBubbleTextBoxTextChanged);
    }

    public ChatBubbleDirection ChatBubbleDirection
    {
      get
      {
        return (ChatBubbleDirection) ((DependencyObject) this).GetValue(ChatBubbleTextBox.ChatBubbleDirectionProperty);
      }
      set
      {
        ((DependencyObject) this).SetValue(ChatBubbleTextBox.ChatBubbleDirectionProperty, (object) value);
      }
    }

    public string Hint
    {
      get => (string) ((DependencyObject) this).GetValue(ChatBubbleTextBox.HintProperty);
      set => ((DependencyObject) this).SetValue(ChatBubbleTextBox.HintProperty, (object) value);
    }

    public Style HintStyle
    {
      get => (Style) ((DependencyObject) this).GetValue(ChatBubbleTextBox.HintStyleProperty);
      set
      {
        ((DependencyObject) this).SetValue(ChatBubbleTextBox.HintStyleProperty, (object) value);
      }
    }

    public virtual void OnApplyTemplate()
    {
      ((FrameworkElement) this).OnApplyTemplate();
      this.HintContentElement = ((Control) this).GetTemplateChild("HintContentElement") as ContentControl;
      this.UpdateHintVisibility();
      this.UpdateChatBubbleDirection();
    }

    protected virtual void OnGotFocus(RoutedEventArgs e)
    {
      this._hasFocus = true;
      this.SetHintVisibility((Visibility) 1);
      base.OnGotFocus(e);
    }

    protected virtual void OnLostFocus(RoutedEventArgs e)
    {
      this._hasFocus = false;
      this.UpdateHintVisibility();
      base.OnLostFocus(e);
    }

    private void UpdateHintVisibility()
    {
      if (this._hasFocus)
        return;
      this.SetHintVisibility(string.IsNullOrEmpty(this.Text) ? (Visibility) 0 : (Visibility) 1);
    }

    private void SetHintVisibility(Visibility value)
    {
      if (this.HintContentElement == null)
        return;
      ((UIElement) this.HintContentElement).Visibility = value;
    }

    private static void OnChatBubbleDirectionChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(d is ChatBubbleTextBox chatBubbleTextBox))
        return;
      chatBubbleTextBox.UpdateChatBubbleDirection();
    }

    private void UpdateChatBubbleDirection()
    {
      VisualStateManager.GoToState((Control) this, this.ChatBubbleDirection.ToString(), true);
    }

    private void ChatBubbleTextBoxTextChanged(object sender, TextChangedEventArgs e)
    {
      this.UpdateHintVisibility();
    }
  }
}
