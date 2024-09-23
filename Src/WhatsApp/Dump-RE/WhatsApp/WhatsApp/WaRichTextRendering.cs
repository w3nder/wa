// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaRichTextRendering
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

#nullable disable
namespace WhatsApp
{
  public static class WaRichTextRendering
  {
    public static double LargeEmojiSizeFromFontSize(double fontSize) => fontSize * 1.175;

    public static double SmallEmojiSizeFromFontSize(double fontSize) => fontSize * 0.95;

    public static void RenderRichText(
      ref InlineCollection inlines,
      IEnumerable<LinkDetector.Result> chunks,
      WaRichTextRendering.RenderArgs args)
    {
      foreach (LinkDetector.Result chunk in chunks)
      {
        string s = chunk.Value;
        if (chunk.type == 0)
          WaRichTextRendering.RenderPlainText(ref inlines, s, args);
        else if ((chunk.type & 16) != 0)
          WaRichTextRendering.RenderCode(ref inlines, s);
        else if ((chunk.type & 2) != 0)
          WaRichTextRendering.RenderEmoji(ref inlines, s, (WaRichText.Formats) chunk.type, args);
        else if ((chunk.type & 1) != 0)
          WaRichTextRendering.RenderLink(ref inlines, s, chunk, args);
        else if ((chunk.type & 256) != 0)
          WaRichTextRendering.RenderMention(ref inlines, chunk.AuxiliaryInfo, (WaRichText.Formats) chunk.type, args);
        else if (args.AllowTextFormatting)
          WaRichTextRendering.RenderText(ref inlines, s, (WaRichText.Formats) chunk.type, chunk.AuxiliaryInfo, args);
        else
          WaRichTextRendering.RenderPlainText(ref inlines, s, args);
      }
    }

    public static void RenderPlainText(
      ref InlineCollection inlines,
      string s,
      WaRichTextRendering.RenderArgs args)
    {
      InlineCollection inlineCollection = inlines;
      Run run = new Run();
      run.Text = s ?? "";
      run.FontFamily = args.FontFamily;
      inlineCollection.Add((Inline) run);
    }

    public static void RenderEmoji(
      ref InlineCollection inlines,
      string s,
      WaRichText.Formats format,
      WaRichTextRendering.RenderArgs args)
    {
      Emoji.EmojiChar emojiChar = Emoji.GetEmojiChar(s);
      if (emojiChar == null)
        WaRichTextRendering.RenderPlainText(ref inlines, s, args);
      else
        inlines.Add((Inline) new InlineUIContainer()
        {
          Child = (UIElement) WaRichTextRendering.CreateEmojiBlock(emojiChar, format, args)
        });
    }

    public static void RenderLink(
      ref InlineCollection inlines,
      string s,
      LinkDetector.Result chunk,
      WaRichTextRendering.RenderArgs args)
    {
      if (string.IsNullOrEmpty(s))
        return;
      WaRichText.Formats type = (WaRichText.Formats) chunk.type;
      if (!args.AllowLinks)
      {
        WaRichTextRendering.RenderText(ref inlines, s, type, (string) null, args);
      }
      else
      {
        string uriStr = chunk.AuxiliaryInfo ?? s;
        Action clickAction = chunk.ClickAction;
        if (clickAction == null)
          clickAction = (Action) (() => WaRichTextRendering.ProcessLinkClick(uriStr));
        if (args.LinkBackground == null)
        {
          Hyperlink link = WaRichTextRendering.CreateLink(s, uriStr, clickAction, type, chunk.ForegroundColor, chunk.LinkUnderscore, args);
          inlines.Add((Inline) link);
        }
        else
        {
          RichTextBox richTextBox1 = (RichTextBox) null;
          foreach (char c in s)
          {
            Run run1 = new Run();
            run1.Text = new string(c, 1);
            run1.FontFamily = args.FontFamily;
            Run run2 = run1;
            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add((Inline) run2);
            RichTextBox richTextBox2 = new RichTextBox();
            richTextBox2.FontSize = args.FontSize;
            richTextBox2.Margin = new Thickness(richTextBox1 == null ? -5.0 : -11.0, 0.0, -11.0, -2.0);
            richTextBox2.VerticalAlignment = VerticalAlignment.Center;
            richTextBox2.Foreground = args.Foreground;
            richTextBox1 = richTextBox2;
            if (WaRichText.ContainsFormat(type, WaRichText.Formats.Bold))
              richTextBox1.FontWeight = FontWeights.SemiBold;
            if (WaRichText.ContainsFormat(type, WaRichText.Formats.Italic))
              richTextBox1.FontStyle = FontStyles.Italic;
            richTextBox1.Blocks.Add((Block) paragraph);
            Grid grid1 = new Grid();
            grid1.Margin = new Thickness(-1.0, 0.0, -1.0, -4.0);
            grid1.Background = args.LinkBackground;
            Grid grid2 = grid1;
            grid2.Tap += (EventHandler<GestureEventArgs>) ((sender, e) => clickAction());
            grid2.Children.Add((UIElement) richTextBox1);
            inlines.Add((Inline) new InlineUIContainer()
            {
              Child = (UIElement) grid2
            });
          }
          richTextBox1.Margin = new Thickness(-11.0, 0.0, -5.0, -2.0);
        }
      }
    }

    public static void RenderCode(ref InlineCollection inlines, string s)
    {
      InlineCollection inlineCollection = inlines;
      Run run = new Run();
      run.Text = s ?? "";
      run.FontFamily = new FontFamily("Courier New");
      inlineCollection.Add((Inline) run);
    }

    public static void RenderText(
      ref InlineCollection inlines,
      string s,
      WaRichText.Formats format,
      string hex,
      WaRichTextRendering.RenderArgs args)
    {
      if (WaRichText.ContainsFormat(format, WaRichText.Formats.Strikethrough))
      {
        CultureInfo cult = new CultureInfo(AppResources.CultureString);
        if ((!cult.IsRightToLeft() || BidiLanguageExtensions.HasRtlChars(s)) && (cult.IsRightToLeft() || !BidiLanguageExtensions.HasRtlChars(s)))
        {
          WaRichTextRendering.RenderStrikethrough(ref inlines, s, format, hex, args);
          return;
        }
      }
      Run run1 = new Run();
      run1.Text = s ?? "";
      run1.FontFamily = args.FontFamily;
      Run run2 = run1;
      if (WaRichText.ContainsFormat(format, WaRichText.Formats.Bold))
        run2.FontWeight = FontWeights.SemiBold;
      if (WaRichText.ContainsFormat(format, WaRichText.Formats.Italic))
        run2.FontStyle = FontStyles.Italic;
      if (WaRichText.ContainsFormat(format, WaRichText.Formats.Foreground) && !string.IsNullOrEmpty(hex))
        run2.Foreground = (Brush) new SolidColorBrush(UIUtils.HexToColor(hex));
      inlines.Add((Inline) run2);
    }

    private static void RenderStrikethrough(
      ref InlineCollection inlines,
      string s,
      WaRichText.Formats format,
      string hex,
      WaRichTextRendering.RenderArgs args)
    {
      if (string.IsNullOrEmpty(s))
        return;
      if (BidiLanguageExtensions.HasRtlChars(s))
      {
        string[] strArray = s.Split(' ');
        int length = strArray.Length;
        for (int index = 0; index < length; ++index)
        {
          string s1 = strArray[index];
          if (s1.Length > 15)
          {
            foreach (char ch in s1)
              WaRichTextRendering.RenderStrikethroughImpl(ref inlines, ch.ToString(), format, hex, args);
          }
          else if (s1.Length > 0)
            WaRichTextRendering.RenderStrikethroughImpl(ref inlines, s1, format, hex, args);
          if (index != strArray.Length - 1)
            WaRichTextRendering.RenderStrikethroughImpl(ref inlines, " ", format, hex, args);
        }
      }
      else
      {
        foreach (char ch in s)
          WaRichTextRendering.RenderStrikethroughImpl(ref inlines, ch.ToString(), format, hex, args);
      }
    }

    private static void RenderStrikethroughImpl(
      ref InlineCollection inlines,
      string s,
      WaRichText.Formats format,
      string hex,
      WaRichTextRendering.RenderArgs args)
    {
      Grid grid1 = new Grid();
      grid1.Margin = new Thickness(0.0, 0.0, 0.0, -6.0 * ResolutionHelper.ZoomMultiplier);
      Grid grid2 = grid1;
      Rectangle rectangle1 = new Rectangle();
      rectangle1.Stretch = Stretch.Fill;
      rectangle1.Height = 1.0;
      rectangle1.Margin = new Thickness(0.0, 3.0 * ResolutionHelper.ZoomMultiplier, 0.0, 0.0);
      rectangle1.Fill = args.Foreground ?? (Brush) UIUtils.ForegroundBrush;
      rectangle1.FlowDirection = FlowDirection.LeftToRight;
      rectangle1.VerticalAlignment = VerticalAlignment.Center;
      Rectangle rectangle2 = rectangle1;
      RichTextBox richTextBox1 = new RichTextBox();
      richTextBox1.FontSize = args.FontSize;
      richTextBox1.Margin = new Thickness(-13.0, 0.0, -13.0, 0.0);
      richTextBox1.VerticalAlignment = VerticalAlignment.Center;
      RichTextBox richTextBox2 = richTextBox1;
      if (args.FontFamily != null)
        richTextBox2.FontFamily = args.FontFamily;
      if (args.Foreground != null)
        richTextBox2.Foreground = args.Foreground;
      if (args.ForegroundBindingSource != null)
      {
        Binding binding = new Binding("Foreground");
        binding.Source = args.ForegroundBindingSource;
        rectangle2.SetBinding(Shape.FillProperty, binding);
        richTextBox2.SetBinding(Control.ForegroundProperty, binding);
      }
      Paragraph paragraph = new Paragraph();
      Run run1 = new Run();
      run1.Text = s;
      run1.FontFamily = args.FontFamily;
      Run run2 = run1;
      if (WaRichText.ContainsFormat(format, WaRichText.Formats.Bold))
        run2.FontWeight = FontWeights.SemiBold;
      if (WaRichText.ContainsFormat(format, WaRichText.Formats.Italic))
        run2.FontStyle = FontStyles.Italic;
      if (WaRichText.ContainsFormat(format, WaRichText.Formats.Foreground) && !string.IsNullOrEmpty(hex))
        run2.Foreground = (Brush) new SolidColorBrush(UIUtils.HexToColor(hex));
      grid2.Children.Add((UIElement) richTextBox2);
      richTextBox2.Blocks.Add((Block) paragraph);
      paragraph.Inlines.Add((Inline) run2);
      grid2.Children.Add((UIElement) rectangle2);
      inlines.Add((Inline) new InlineUIContainer()
      {
        Child = (UIElement) grid2
      });
    }

    private static void RenderMention(
      ref InlineCollection inlines,
      string jid,
      WaRichText.Formats format,
      WaRichTextRendering.RenderArgs args)
    {
      if (!JidHelper.IsUserJid(jid))
        return;
      if (!args.AllowMentions)
      {
        WaRichTextRendering.RenderPlainText(ref inlines, JidHelper.GetPhoneNumber(jid, false), args);
      }
      else
      {
        format |= WaRichText.Formats.Bold;
        WaRichTextRendering.RenderText(ref inlines, "@", format, (string) null, args);
        UserStatus userStatus = UserCache.Get(jid, false);
        string str = userStatus?.GetDisplayName(getNumberIfNoName: false, getFormattedNumber: false);
        bool flag1 = false;
        if (string.IsNullOrEmpty(str))
        {
          str = userStatus?.PushName ?? (jid == Settings.MyJid ? Settings.PushName : (string) null);
          if (string.IsNullOrEmpty(str))
          {
            str = JidHelper.GetPhoneNumber(jid, true);
            flag1 = true;
          }
        }
        IEnumerable<LinkDetector.Result> chunks = (IEnumerable<LinkDetector.Result>) null;
        bool flag2 = false;
        if (!flag1)
        {
          chunks = LinkDetector.GetMatches(str, new WaRichText.DetectionArgs(WaRichText.Formats.Emoji));
          flag2 = WaRichText.ContainsFormat(chunks, WaRichText.Formats.Emoji);
        }
        if (!flag2)
          chunks = (IEnumerable<LinkDetector.Result>) new LinkDetector.Result[1]
          {
            new LinkDetector.Result(0, str.Length, 0, str)
          };
        foreach (LinkDetector.Result result in chunks)
        {
          if ((result.type & 2) != 0)
          {
            Emoji.EmojiChar emojiChar = Emoji.GetEmojiChar(result.Value);
            if (emojiChar == null)
            {
              WaRichTextRendering.RenderText(ref inlines, result.Value, format, (string) null, args);
            }
            else
            {
              FrameworkElement emojiBlock = WaRichTextRendering.CreateEmojiBlock(emojiChar, format, args);
              if (args.EnableMentionLinks)
                emojiBlock.Tap += (EventHandler<GestureEventArgs>) ((sender, e) => WaRichTextRendering.ProcessMentionClick(jid));
              inlines.Add((Inline) new InlineUIContainer()
              {
                Child = (UIElement) emojiBlock
              });
            }
          }
          else if (args.EnableMentionLinks)
          {
            Hyperlink link = WaRichTextRendering.CreateLink(result.Value, (string) null, (Action) (() => WaRichTextRendering.ProcessMentionClick(jid)), format, new Color?(), false, args);
            inlines.Add((Inline) link);
          }
          else
            WaRichTextRendering.RenderText(ref inlines, result.Value, format, (string) null, args);
        }
      }
    }

    private static void ProcessMentionClick(string jid)
    {
      if (!JidHelper.IsUserJid(jid))
        return;
      ContactInfoPage.Start(UserCache.Get(jid, true));
    }

    private static Hyperlink CreateLink(
      string s,
      string uriStr,
      Action clickAction,
      WaRichText.Formats format,
      Color? textColor,
      bool underscore,
      WaRichTextRendering.RenderArgs args)
    {
      Brush brush = !textColor.HasValue ? args.Foreground ?? (Brush) UIUtils.ForegroundBrush : (Brush) new SolidColorBrush(textColor.Value);
      Hyperlink hyperlink = new Hyperlink();
      hyperlink.Foreground = brush;
      hyperlink.TextDecorations = underscore ? TextDecorations.Underline : (TextDecorationCollection) null;
      Hyperlink link = hyperlink;
      link.Inlines.Add(s ?? uriStr);
      Action a = clickAction;
      link.Command = (ICommand) new ActionCommand((Action) (() =>
      {
        if (a == null)
          WaRichTextRendering.ProcessLinkClick(uriStr);
        else
          a();
      }));
      if (WaRichText.ContainsFormat(format, WaRichText.Formats.Bold))
        link.FontWeight = FontWeights.SemiBold;
      if (WaRichText.ContainsFormat(format, WaRichText.Formats.Italic))
        link.FontStyle = FontStyles.Italic;
      if (WaRichText.ContainsFormat(format, WaRichText.Formats.Foreground))
        link.Foreground = (Brush) UIUtils.AccentBrush;
      return link;
    }

    private static FrameworkElement CreateEmojiBlock(
      Emoji.EmojiChar emojiChar,
      WaRichText.Formats format,
      WaRichTextRendering.RenderArgs args)
    {
      double fontSize = args.FontSize;
      double emojiSize = args.UseLargeEmojiSize ? WaRichTextRendering.LargeEmojiSizeFromFontSize(fontSize) : WaRichTextRendering.SmallEmojiSizeFromFontSize(fontSize);
      Rectangle rectangle1 = new Rectangle();
      rectangle1.Margin = new Thickness(emojiSize * 0.08, 0.0, emojiSize * 0.08, -emojiSize * 0.16);
      rectangle1.Width = emojiSize;
      rectangle1.Height = fontSize;
      rectangle1.RenderTransform = (Transform) new CompositeTransform()
      {
        ScaleY = (emojiSize / fontSize)
      };
      rectangle1.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
      rectangle1.FlowDirection = FlowDirection.LeftToRight;
      Rectangle emojiRectangle = rectangle1;
      IDisposable sub = (IDisposable) null;
      sub = emojiChar.Image.Subscribe<Emoji.EmojiChar.Args>((Action<Emoji.EmojiChar.Args>) (res =>
      {
        CompositeTransform compositeTransform = new CompositeTransform()
        {
          ScaleX = emojiSize / res.Width,
          ScaleY = fontSize / res.Height,
          TranslateX = -res.X * emojiSize / res.Width,
          TranslateY = -res.Y * fontSize / res.Height
        };
        Rectangle rectangle2 = emojiRectangle;
        rectangle2.Fill = (Brush) new ImageBrush()
        {
          Transform = (Transform) compositeTransform,
          ImageSource = (System.Windows.Media.ImageSource) res.BaseImage,
          AlignmentX = AlignmentX.Left,
          AlignmentY = AlignmentY.Top,
          Stretch = Stretch.None
        };
        sub.SafeDispose();
        sub = (IDisposable) null;
      }), (Action<Exception>) (ex =>
      {
        sub.SafeDispose();
        sub = (IDisposable) null;
      }));
      FrameworkElement emojiBlock;
      if (WaRichText.ContainsFormat(format, WaRichText.Formats.Strikethrough))
      {
        Grid grid = new Grid();
        grid.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
        Rectangle rectangle3 = new Rectangle();
        rectangle3.Stretch = Stretch.Fill;
        rectangle3.Height = 1.0;
        rectangle3.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
        rectangle3.Fill = args.Foreground ?? (Brush) UIUtils.ForegroundBrush;
        rectangle3.FlowDirection = FlowDirection.LeftToRight;
        rectangle3.VerticalAlignment = VerticalAlignment.Center;
        Rectangle rectangle4 = rectangle3;
        emojiRectangle.VerticalAlignment = VerticalAlignment.Center;
        grid.Children.Add((UIElement) emojiRectangle);
        grid.Children.Add((UIElement) rectangle4);
        emojiBlock = (FrameworkElement) grid;
      }
      else
        emojiBlock = (FrameworkElement) emojiRectangle;
      return emojiBlock;
    }

    private static void ProcessLinkClick(string uriStr)
    {
      if (uriStr == null)
        return;
      uriStr = LinkDetector.InferUriScheme(uriStr);
      if (uriStr.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
        WaRichTextRendering.CreateEmailTask(uriStr).Show();
      else if (uriStr.StartsWith(GroupInviteLinkPage.GroupInviteLinkStart, StringComparison.OrdinalIgnoreCase))
      {
        GroupInviteLinkPage.JoinGroupWithInviteLink(uriStr);
      }
      else
      {
        Uri result = (Uri) null;
        if (!Uri.TryCreate(uriStr, UriKind.Absolute, out result) || !(result != (Uri) null))
          return;
        new WebBrowserTask() { Uri = result }.Show();
      }
    }

    public static EmailComposeTask CreateEmailTask(string mailToUri)
    {
      EmailComposeTask task = new EmailComposeTask();
      int num = mailToUri.IndexOf('?');
      string str1 = num < 0 ? "" : mailToUri.Substring(num + 1);
      string str2 = mailToUri.Substring("mailto:".Length, mailToUri.Length - "mailto:".Length - (num < 0 ? 0 : str1.Length + 1));
      task.To = str2;
      StringComparison comparisonType = StringComparison.OrdinalIgnoreCase;
      \u003C\u003Ef__AnonymousType2<string, Action<string>>[] dataArray = new \u003C\u003Ef__AnonymousType2<string, Action<string>>[4]
      {
        new
        {
          Key = "subject",
          OnParse = (Action<string>) (value => task.Subject = value)
        },
        new
        {
          Key = "cc",
          OnParse = (Action<string>) (value => task.Cc = value)
        },
        new
        {
          Key = "bcc",
          OnParse = (Action<string>) (value => task.Bcc = value)
        },
        new
        {
          Key = "body",
          OnParse = (Action<string>) (value => task.Body = value)
        }
      };
      string str3 = str1;
      char[] chArray = new char[1]{ '&' };
      foreach (string str4 in str3.Split(chArray))
      {
        int length = str4.IndexOf('=');
        if (length >= 0)
        {
          string strA = str4.Substring(0, length);
          string str5 = HttpUtility.UrlDecode(str4.Substring(length + 1));
          foreach (var data in dataArray)
          {
            if (string.Compare(strA, data.Key, comparisonType) == 0)
            {
              data.OnParse(str5);
              break;
            }
          }
        }
      }
      return task;
    }

    public class RenderArgs
    {
      private bool allowLinks = true;
      private bool allowMentions = true;
      private bool enableMentionLinks = true;
      private bool allowTextFormatting = true;

      public Brush Foreground { get; set; }

      public double FontSize { get; set; }

      public FontFamily FontFamily { get; set; }

      public FontStyle? FontStyle { get; set; }

      public FontWeight? FontWeight { get; set; }

      public bool UseLargeEmojiSize { get; set; }

      public object ForegroundBindingSource { get; set; }

      public bool AllowLinks
      {
        get => this.allowLinks;
        set => this.allowLinks = value;
      }

      public Brush LinkBackground { get; set; }

      public bool AllowMentions
      {
        get => this.allowMentions;
        set => this.allowMentions = value;
      }

      public bool EnableMentionLinks
      {
        get => this.enableMentionLinks;
        set => this.enableMentionLinks = value;
      }

      public bool AllowTextFormatting
      {
        get => this.allowTextFormatting;
        set => this.allowTextFormatting = value;
      }

      public RenderArgs(double fontSize) => this.FontSize = fontSize;
    }
  }
}
