// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.MessagePrompt
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public class MessagePrompt : UserPrompt
  {
    public static readonly DependencyProperty BodyProperty = DependencyProperty.Register(nameof (Body), typeof (object), typeof (MessagePrompt), new PropertyMetadata((PropertyChangedCallback) null));

    public MessagePrompt()
    {
      this.DefaultStyleKey = (object) typeof (MessagePrompt);
      this.MessageChanged = new Action(this.SetBodyMessage);
    }

    public object Body
    {
      get => ((DependencyObject) this).GetValue(MessagePrompt.BodyProperty);
      set => ((DependencyObject) this).SetValue(MessagePrompt.BodyProperty, value);
    }

    protected internal void SetBodyMessage()
    {
      this.Body = (object) new TextBlock()
      {
        Text = this.Message,
        TextWrapping = (TextWrapping) 2
      };
    }
  }
}
