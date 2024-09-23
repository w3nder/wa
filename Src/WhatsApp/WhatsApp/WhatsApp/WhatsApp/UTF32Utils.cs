// Decompiled with JetBrains decompiler
// Type: WhatsApp.UTF32Utils
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
  public static class UTF32Utils
  {
    private static bool recursive = false;
    public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.RegisterAttached("MaxLength", typeof (int?), typeof (UTF32), new PropertyMetadata((object) null, (PropertyChangedCallback) ((sender, args) => UTF32Utils.Truncate(!(sender is EmojiTextBox) ? (TextBox) sender : ((EmojiTextBox) sender).TextBox, (int?) args.NewValue))));

    private static void Truncate(TextBox text, int? maxLength)
    {
      if (UTF32Utils.recursive)
        return;
      if (!maxLength.HasValue)
        return;
      try
      {
        List<uint> list = (text.Text ?? "").ToUtf32().ToList<uint>();
        if (list.Count <= maxLength.Value)
          return;
        list.RemoveRange(maxLength.Value, list.Count - maxLength.Value);
        try
        {
          UTF32Utils.recursive = true;
          int selectionStart = text.SelectionStart;
          int selectionLength = text.SelectionLength;
          text.Text = new string(UTF32.ToUtf16((IEnumerable<uint>) list).ToArray<char>());
          text.SelectionStart = selectionStart;
          text.SelectionLength = selectionLength;
        }
        finally
        {
          UTF32Utils.recursive = false;
        }
      }
      catch (Exception ex)
      {
      }
    }

    public static void SetMaxLength(DependencyObject element, int? value)
    {
      element.SetValue(UTF32Utils.MaxLengthProperty, (object) value);
    }

    public static int? GetMaxLength(DependencyObject element)
    {
      return (int?) element.GetValue(UTF32Utils.MaxLengthProperty);
    }
  }
}
