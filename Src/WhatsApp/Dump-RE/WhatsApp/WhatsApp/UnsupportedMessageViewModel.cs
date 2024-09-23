// Decompiled with JetBrains decompiler
// Type: WhatsApp.UnsupportedMessageViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class UnsupportedMessageViewModel : MessageViewModel
  {
    public override Thickness ViewPanelMargin
    {
      get
      {
        double num = 12.0 * this.zoomMultiplier;
        return new Thickness(num, num, num, num * 3.0);
      }
    }

    public override HorizontalAlignment HorizontalAlignment
    {
      get
      {
        return !this.Message.HasPaymentInfo() ? HorizontalAlignment.Stretch : base.HorizontalAlignment;
      }
    }

    public override double MaxBubbleWidth
    {
      get
      {
        return !this.Message.HasPaymentInfo() ? double.PositiveInfinity : MessageViewModel.DefaultBubbleWidth;
      }
    }

    public UnsupportedMessageViewModel(Message m)
      : base(m)
    {
      this.ExcludedMenuItems = new Set<MessageMenu.MessageMenuItem>((IEnumerable<MessageMenu.MessageMenuItem>) new MessageMenu.MessageMenuItem[4]
      {
        MessageMenu.MessageMenuItem.ShowDetails,
        MessageMenu.MessageMenuItem.Copy,
        MessageMenu.MessageMenuItem.Forward,
        MessageMenu.MessageMenuItem.Reply
      });
      if (m.MediaWaType != FunXMPP.FMessage.Type.CipherText)
        return;
      ++Settings.CipherTextPlaceholderShown;
    }

    public override RichTextBlock.TextSet GetRichText()
    {
      string str1 = (string) null;
      IEnumerable<WaRichText.Chunk> chunks = (IEnumerable<WaRichText.Chunk>) null;
      switch (this.Message.MediaWaType)
      {
        case FunXMPP.FMessage.Type.CipherText:
          string str2 = AppResources.MessagePendingDecrypt + " ";
          string learnMoreText = AppResources.LearnMoreText;
          WaRichText.Chunk chunk = new WaRichText.Chunk(str2.Length, learnMoreText.Length, WaRichText.Formats.Link, WaWebUrls.FaqUrlE2ePlaceholder);
          str1 = str2 + learnMoreText;
          chunks = (IEnumerable<WaRichText.Chunk>) new WaRichText.Chunk[1]
          {
            chunk
          };
          break;
        case FunXMPP.FMessage.Type.ProtocolBuffer:
          return UnsupportedMessageViewModel.GetUnsupportedMessageTextSet();
      }
      return new RichTextBlock.TextSet()
      {
        Text = str1 ?? "",
        PartialFormattings = chunks
      };
    }

    public static RichTextBlock.TextSet GetUnsupportedMessageTextSet()
    {
      IEnumerable<WaRichText.Chunk> chunks = (IEnumerable<WaRichText.Chunk>) null;
      string messageNotSupported = AppResources.MessageNotSupported;
      try
      {
        WaRichText.Chunk chunk = ((IEnumerable<WaRichText.Chunk>) WaRichText.GetHtmlLinkChunks(messageNotSupported)).SingleOrDefault<WaRichText.Chunk>();
        if (chunk != null)
        {
          chunk.Format = WaRichText.Formats.Link;
          chunk.AuxiliaryInfo = Constants.SwitchPhonesUrl;
          chunks = (IEnumerable<WaRichText.Chunk>) new WaRichText.Chunk[1]
          {
            chunk
          };
        }
      }
      catch (Exception ex)
      {
      }
      return new RichTextBlock.TextSet()
      {
        Text = messageNotSupported ?? "",
        PartialFormattings = chunks
      };
    }

    protected override string GetTextStr()
    {
      string textStr = (string) null;
      switch (this.Message.MediaWaType)
      {
        case FunXMPP.FMessage.Type.CipherText:
          textStr = AppResources.MessagePendingDecrypt + " ";
          break;
        case FunXMPP.FMessage.Type.ProtocolBuffer:
          textStr = AppResources.MessageNotSupported;
          break;
      }
      return textStr;
    }

    public BitmapSource Icon
    {
      get
      {
        BitmapSource icon = (BitmapSource) null;
        switch (this.Message.MediaWaType)
        {
          case FunXMPP.FMessage.Type.CipherText:
            icon = AssetStore.EncryptedMessage;
            break;
          case FunXMPP.FMessage.Type.ProtocolBuffer:
            icon = AssetStore.UnsupportedMessage;
            break;
        }
        return icon;
      }
    }

    public override bool ShouldReplace => true;
  }
}
