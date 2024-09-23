// Decompiled with JetBrains decompiler
// Type: WhatsApp.LongMessageSplitter
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public static class LongMessageSplitter
  {
    public const byte Version = 2;
    private const double TextureLimit = 2000.0;
    private static Dictionary<int, Size> cachedCharSizeEstimations_ = new Dictionary<int, Size>();
    private static Size? charSizeEstimation_ = new Size?();
    private static object charSizeEstimationLock_ = new object();

    public static Size GetCharSizeEstimation()
    {
      double textFontSize = Settings.SystemFontSize;
      Size charPixelSize;
      lock (LongMessageSplitter.charSizeEstimationLock_)
      {
        if (!LongMessageSplitter.cachedCharSizeEstimations_.TryGetValue((int) textFontSize, out charPixelSize))
        {
          Deployment.Current.Dispatcher.InvokeSynchronous((Action) (() =>
          {
            TextBlock textBlock = new TextBlock()
            {
              FontSize = textFontSize
            };
            textBlock.Text = "XX";
            double val2 = WaRichTextRendering.LargeEmojiSizeFromFontSize(textFontSize);
            charPixelSize = new Size(Math.Min(textBlock.ActualWidth, val2), textBlock.ActualHeight + textBlock.Margin.Bottom + textBlock.Margin.Top);
          }));
          LongMessageSplitter.cachedCharSizeEstimations_[(int) textFontSize] = charPixelSize;
        }
      }
      return charPixelSize;
    }

    public static Size ResetCharSizeEstimation()
    {
      return (LongMessageSplitter.charSizeEstimation_ = new Size?(LongMessageSplitter.GetCharSizeEstimation())).Value;
    }

    public static MessageViewModel[] TrySplitMessage(
      Message m,
      double targetLineWidth,
      bool updateHintIfNeeded)
    {
      return LongMessageSplitter.TrySplitMessage(m, LongMessageSplitter.charSizeEstimation_ ?? LongMessageSplitter.ResetCharSizeEstimation(), targetLineWidth, updateHintIfNeeded);
    }

    private static MessageViewModel[] TrySplitMessage(
      Message m,
      Size charSize,
      double targetLineWidth,
      bool updateHintIfNeeded)
    {
      if (m == null)
        return new MessageViewModel[0];
      bool flag1 = false;
      if (m.TextSplittingHint != null && m.TextSplittingHint.Length == 9 && m.TextSplittingHint[0] == (byte) 2)
      {
        int num1 = BinaryData.ReadInt32(m.TextSplittingHint, 1);
        int num2 = BinaryData.ReadInt32(m.TextSplittingHint, 5);
        if ((int) charSize.Width <= num1 && (int) charSize.Height <= num2)
          flag1 = true;
      }
      if (flag1)
        return new MessageViewModel[1]
        {
          MessageViewModel.Create(m)
        };
      List<MessageViewModel> r = (List<MessageViewModel>) null;
      int chopStart = 0;
      string textForDisplay = m.GetTextForDisplay();
      int num3 = 0;
      int num4 = 0;
      int num5 = -1;
      int num6 = -1;
      bool flag2 = false;
      LinkDetector.Result[] array = (LinkDetector.Result[]) null;
      int num7 = (int) (targetLineWidth / charSize.Width);
      int num8 = (int) (2000.0 / charSize.Height);
      Action<int> action = (Action<int>) (chopEnd =>
      {
        if (chopEnd <= chopStart)
          return;
        if (r == null)
          r = new List<MessageViewModel>();
        Log.d("msg split", "offset:{0},len:{1}", (object) chopStart, (object) (chopEnd - chopStart));
        MessageViewModel messageViewModel = MessageViewModel.Create(m);
        messageViewModel.TextChop = new MessageViewModel.ChopState()
        {
          Offset = chopStart,
          Length = chopEnd - chopStart
        };
        messageViewModel.MergedPosition = MessageViewModel.GroupingPosition.Middle;
        r.Add(messageViewModel);
      });
      int length = textForDisplay.Length;
      for (int index1 = 0; index1 < length; ++index1)
      {
        char c = textForDisplay[index1];
        bool flag3 = char.IsWhiteSpace(c);
        if (flag3)
          num6 = index1;
        if (c == '\n' || c == '\r')
        {
          flag3 = true;
          num5 = num6 = index1;
          ++num3;
          num4 = 0;
        }
        else
        {
          ++num4;
          if (num4 >= num7)
          {
            ++num3;
            num4 = 0;
          }
        }
        if (num3 >= num8)
        {
          int num9 = index1;
          if (!flag3)
          {
            if (num6 > chopStart && num9 - num6 < num7)
            {
              num9 = num6;
              flag3 = true;
            }
            else if (num5 > chopStart)
            {
              num9 = num5;
              flag3 = true;
            }
          }
          if (!flag2)
          {
            array = m.GetRichTextFormattings();
            flag2 = true;
          }
          int index2;
          if (array != null && (index2 = array.BinarySearch<LinkDetector.Result, int>(num9, new Func<int, LinkDetector.Result, int>(LongMessageSplitter.PerfHintFind))) >= 0)
          {
            LinkDetector.Result result = array[index2];
            if (num9 > result.Index && ((result.type & 1) != 0 || (result.type & 2) != 0 || (result.type & 256) != 0))
            {
              num9 = result.Index + result.Length;
              flag3 = num9 < length && char.IsWhiteSpace(textForDisplay[num9]);
            }
          }
          int funkynessLength = 0;
          if (Utils.IsFunkyUnicode(textForDisplay, num9, length, out funkynessLength))
          {
            num9 += funkynessLength;
            flag3 = num9 < length && char.IsWhiteSpace(textForDisplay[num9]);
          }
          action(num9);
          chopStart = flag3 ? num9 + 1 : num9;
          if (index1 < chopStart)
          {
            index1 = chopStart;
            num4 = 0;
          }
          else
            num4 = index1 - chopStart;
          num3 = 0;
        }
      }
      if (r != null && r.Any<MessageViewModel>())
      {
        action(length);
        if (r.Count > 1)
        {
          r.First<MessageViewModel>().MergedPosition = MessageViewModel.GroupingPosition.Top;
          r.Last<MessageViewModel>().MergedPosition = MessageViewModel.GroupingPosition.Bottom;
          Log.d("msg split", "splitted | {0},len:{1},parts:{2},char size:{3}x{4}", (object) m.LogInfo(), (object) length, (object) r.Count, (object) (int) charSize.Width, (object) (int) charSize.Height);
          return r.ToArray();
        }
      }
      if (updateHintIfNeeded)
      {
        BinaryData binaryData = new BinaryData();
        binaryData.AppendByte((byte) 2);
        binaryData.AppendInt32((int) charSize.Width);
        binaryData.AppendInt32((int) charSize.Height);
        m.TextSplittingHint = binaryData.Get();
      }
      return new MessageViewModel[1]
      {
        MessageViewModel.Create(m)
      };
    }

    private static int PerfHintFind(int offset, LinkDetector.Result hint)
    {
      return offset >= hint.Index && offset < hint.Index + hint.Length ? 0 : offset.CompareTo(hint.Index);
    }
  }
}
