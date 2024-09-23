// Decompiled with JetBrains decompiler
// Type: WhatsApp.LargeEmojiMessageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

#nullable disable
namespace WhatsApp
{
  public class LargeEmojiMessageViewModel : MessageViewModel
  {
    private Emoji.EmojiChar emoji;
    private int numEmojis;

    public override double MaxBubbleWidth
    {
      get
      {
        double maxBubbleWidth = 192.0 * this.zoomMultiplier;
        if (this.Message.HasPaymentInfo())
          maxBubbleWidth = MessageViewModel.DefaultBubbleWidth;
        else if (this.QuotedMessage != null || this.ShouldShowHeader)
          maxBubbleWidth = double.PositiveInfinity;
        return maxBubbleWidth;
      }
    }

    public override HorizontalAlignment HorizontalAlignment
    {
      get
      {
        HorizontalAlignment horizontalAlignment = base.HorizontalAlignment;
        if (!this.Message.HasPaymentInfo() && (this.QuotedMessage != null || this.ShouldShowHeader))
          horizontalAlignment = HorizontalAlignment.Stretch;
        return horizontalAlignment;
      }
    }

    public LargeEmojiMessageViewModel(Message m)
      : base(m)
    {
      List<WaRichText.Chunk> chunkList = ((int) WaRichText.BufferContainsValidChunks(m.TextPerformanceHint) ?? 0) != 0 ? WaRichText.Deserialize(m.TextPerformanceHint, (string) null) : (List<WaRichText.Chunk>) null;
      if (chunkList != null)
        this.numEmojis = chunkList.Count;
      if (m.MediaWaType != FunXMPP.FMessage.Type.Undefined)
        return;
      this.emoji = LargeEmojiMessageViewModel.GetAsSingleEmoji(m);
    }

    public int NumEmojiChars => this.numEmojis;

    public static bool ShouldShowAsLargeEmoji(Message m)
    {
      bool flag1 = false;
      if (((int) WaRichText.BufferContainsValidChunks(m.TextPerformanceHint) ?? 0) != 0)
      {
        int? nullable1 = WaRichText.NumChunks(m.TextPerformanceHint);
        if (nullable1.HasValue)
        {
          int? nullable2 = nullable1;
          int num1 = 1;
          if ((nullable2.GetValueOrDefault() >= num1 ? (nullable2.HasValue ? 1 : 0) : 0) != 0)
          {
            int? nullable3 = nullable1;
            int num2 = 3;
            if ((nullable3.GetValueOrDefault() <= num2 ? (nullable3.HasValue ? 1 : 0) : 0) != 0)
            {
              List<WaRichText.Chunk> source = WaRichText.Deserialize(m.TextPerformanceHint, (string) null);
              if (source != null && source.Any<WaRichText.Chunk>())
              {
                bool flag2 = false;
                foreach (WaRichText.Chunk chunk in source)
                {
                  if (chunk == null || chunk.Format != WaRichText.Formats.Emoji)
                    flag2 = true;
                }
                flag1 = !flag2;
              }
            }
          }
        }
      }
      return flag1;
    }

    public static Emoji.EmojiChar GetAsSingleEmoji(Message m)
    {
      Emoji.EmojiChar asSingleEmoji = (Emoji.EmojiChar) null;
      if (m.MediaWaType != FunXMPP.FMessage.Type.Undefined || m.TextPerformanceHint == null)
        return asSingleEmoji;
      WaRichText.Chunk chunk = WaRichText.SingleChunkOrDefault(m.TextPerformanceHint);
      if (chunk != null && chunk.Format == WaRichText.Formats.Emoji)
        asSingleEmoji = Emoji.GetEmojiChar(m.GetTextForDisplay());
      return asSingleEmoji;
    }

    public static bool IsHeartEmoji(Emoji.EmojiChar e)
    {
      return e != null && (e.codepoints.Length == 1 && (e.codepoints[0] == '\uE022' || e.codepoints[0] == '❤') || e.codepoints.Length == 2 && e.codepoints[0] == '❤' && e.codepoints[1] == '️');
    }

    public System.Windows.Media.ImageSource LargeEmojiImageSource
    {
      get
      {
        return LargeEmojiMessageViewModel.IsHeartEmoji(this.emoji) ? (System.Windows.Media.ImageSource) AssetStore.StickerE022 : (System.Windows.Media.ImageSource) null;
      }
    }

    public Storyboard Animation
    {
      get
      {
        return LargeEmojiMessageViewModel.IsHeartEmoji(this.emoji) ? this.GetBeatingHeartAnimation() : (Storyboard) null;
      }
    }

    private Storyboard GetBeatingHeartAnimation()
    {
      Storyboard beatingHeartAnimation = new Storyboard();
      DoubleAnimation doubleAnimation1 = new DoubleAnimation();
      QuadraticEase quadraticEase1 = new QuadraticEase();
      quadraticEase1.EasingMode = EasingMode.EaseInOut;
      doubleAnimation1.EasingFunction = (IEasingFunction) quadraticEase1;
      doubleAnimation1.Duration = (Duration) TimeSpan.FromMilliseconds(500.0);
      doubleAnimation1.AutoReverse = true;
      doubleAnimation1.From = new double?(0.9);
      doubleAnimation1.To = new double?(1.1);
      doubleAnimation1.RepeatBehavior = RepeatBehavior.Forever;
      DoubleAnimation element1 = doubleAnimation1;
      Storyboard.SetTargetProperty((Timeline) element1, new PropertyPath("ScaleX", new object[0]));
      beatingHeartAnimation.Children.Add((Timeline) element1);
      DoubleAnimation doubleAnimation2 = new DoubleAnimation();
      QuadraticEase quadraticEase2 = new QuadraticEase();
      quadraticEase2.EasingMode = EasingMode.EaseInOut;
      doubleAnimation2.EasingFunction = (IEasingFunction) quadraticEase2;
      doubleAnimation2.Duration = (Duration) TimeSpan.FromMilliseconds(500.0);
      doubleAnimation2.AutoReverse = true;
      doubleAnimation2.From = new double?(0.9);
      doubleAnimation2.To = new double?(1.1);
      doubleAnimation2.RepeatBehavior = RepeatBehavior.Forever;
      DoubleAnimation element2 = doubleAnimation2;
      Storyboard.SetTargetProperty((Timeline) element2, new PropertyPath("ScaleY", new object[0]));
      beatingHeartAnimation.Children.Add((Timeline) element2);
      return beatingHeartAnimation;
    }
  }
}
