// Decompiled with JetBrains decompiler
// Type: WhatsApp.NotificationString
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace WhatsApp
{
  public static class NotificationString
  {
    public const string PinChar = "\uD83D\uDCCD";
    public const string CameraChar = "\uD83D\uDCF7";
    public const string CamCorderChar = "\uD83C\uDFA5";
    public const string DocumentChar = "\uD83D\uDCC4";
    public const string GifChar = "\uD83D\uDC7E";
    public const string StickerChar = "\uD83D\uDC9F";
    public const string RevokedChar = "\uD83D\uDEAB";

    public static string RtlSafeFormat(string fmt, params string[] args) => Bidi.Format(fmt, args);

    private static string FormatWithAt(string name, string subj)
    {
      return NotificationString.RtlSafeFormat("{0} @ {1}", name, subj);
    }

    private static string FormatWithColon(string name, string msg)
    {
      return NotificationString.RtlSafeFormat("{0}: {1}", name, msg);
    }

    public static NotificationString.MessageNotificationContent GetForMessage(
      Message msg,
      bool showPreview)
    {
      NotificationString.MessageNotificationContent forMessage = new NotificationString.MessageNotificationContent();
      string senderDisplayName = msg.GetSenderDisplayName(true);
      if (msg.MediaWaType == FunXMPP.FMessage.Type.System)
        forMessage.Header = NotificationString.ProcessSystemMessage(msg);
      else if (JidHelper.IsGroupJid(msg.KeyRemoteJid))
      {
        Conversation convo = (Conversation) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => convo = db.GetConversation(msg.KeyRemoteJid, CreateOptions.None)));
        if (showPreview)
        {
          string str = (string) null;
          LinkDetector.Result[] resultArray = (LinkDetector.Result[]) null;
          string fmt;
          switch (msg.MediaWaType)
          {
            case FunXMPP.FMessage.Type.Image:
              if (msg.MediaCaption != null)
              {
                fmt = "{0} @ {1}: \uD83D\uDCF7 ";
                str = msg.GetTextForDisplay();
                resultArray = msg.GetRichTextFormattings();
                break;
              }
              fmt = AppResources.PushImageToGroup;
              break;
            case FunXMPP.FMessage.Type.Audio:
              fmt = msg.IsPtt() ? AppResources.PushPttToGroup : AppResources.PushAudioToGroup;
              break;
            case FunXMPP.FMessage.Type.Video:
              if (msg.MediaCaption != null)
              {
                fmt = "{0} @ {1}: \uD83C\uDFA5 ";
                str = msg.GetTextForDisplay();
                resultArray = msg.GetRichTextFormattings();
                break;
              }
              fmt = AppResources.PushVideoToGroup;
              break;
            case FunXMPP.FMessage.Type.Contact:
              fmt = AppResources.PushContactToGroup;
              break;
            case FunXMPP.FMessage.Type.Location:
              if (msg.IsCoordinateLocation())
              {
                fmt = AppResources.PushLocationToGroup;
                break;
              }
              fmt = "{0} @ {1}: \uD83D\uDCCD ";
              str = msg.LocationDetails;
              break;
            case FunXMPP.FMessage.Type.Document:
              fmt = "{0} @ {1}: \uD83D\uDCC4 ";
              str = msg.MediaName;
              break;
            case FunXMPP.FMessage.Type.Gif:
              if (msg.MediaCaption != null)
              {
                fmt = "{0} @ {1}: \uD83D\uDC7E ";
                str = msg.GetTextForDisplay();
                resultArray = msg.GetRichTextFormattings();
                break;
              }
              fmt = AppResources.PushGifToGroup;
              break;
            case FunXMPP.FMessage.Type.LiveLocation:
              if (msg.IsCoordinateLocation())
              {
                fmt = AppResources.PushLocationToGroup;
                break;
              }
              fmt = "{0} @ {1}: \uD83D\uDCCD ";
              str = msg.LocationDetails;
              break;
            case FunXMPP.FMessage.Type.Sticker:
              fmt = "{0} @ {1}: \uD83D\uDC9F ";
              break;
            case FunXMPP.FMessage.Type.Revoked:
              fmt = "{0} @ {1}: \uD83D\uDEAB ";
              break;
            default:
              fmt = "{0} @ {1}: ";
              str = msg.GetTextForDisplay();
              resultArray = msg.GetRichTextFormattings();
              break;
          }
          if (fmt != null)
            forMessage.Header = NotificationString.RtlSafeFormat(fmt, senderDisplayName, convo?.GroupSubject ?? "");
          forMessage.MessagePreview = str;
          forMessage.PreviewFormattings = resultArray;
        }
        else
          forMessage.Header = NotificationString.RtlSafeFormat(AppResources.PushMessageNoPreview, NotificationString.FormatWithAt(senderDisplayName, convo?.GroupSubject ?? ""));
      }
      else
      {
        string str = (string) null;
        LinkDetector.Result[] resultArray = (LinkDetector.Result[]) null;
        if (showPreview)
        {
          string fmt;
          switch (msg.MediaWaType)
          {
            case FunXMPP.FMessage.Type.Image:
              if (msg.MediaCaption != null)
              {
                fmt = "{0}: \uD83D\uDCF7 ";
                str = msg.GetTextForDisplay();
                resultArray = msg.GetRichTextFormattings();
                break;
              }
              fmt = AppResources.PushImage;
              break;
            case FunXMPP.FMessage.Type.Audio:
              fmt = msg.IsPtt() ? AppResources.PushPtt : AppResources.PushAudio;
              break;
            case FunXMPP.FMessage.Type.Video:
              if (msg.MediaCaption != null)
              {
                fmt = "{0}: \uD83C\uDFA5 ";
                str = msg.GetTextForDisplay();
                resultArray = msg.GetRichTextFormattings();
                break;
              }
              fmt = AppResources.PushVideo;
              break;
            case FunXMPP.FMessage.Type.Contact:
              fmt = AppResources.PushContact;
              break;
            case FunXMPP.FMessage.Type.Location:
              if (msg.IsCoordinateLocation())
              {
                fmt = AppResources.PushLocation;
                break;
              }
              fmt = "{0}: \uD83D\uDCCD ";
              str = msg.LocationDetails;
              break;
            case FunXMPP.FMessage.Type.Document:
              fmt = "{0}: \uD83D\uDCC4 ";
              str = msg.MediaName;
              break;
            case FunXMPP.FMessage.Type.Gif:
              if (msg.MediaCaption != null)
              {
                fmt = "{0}: \uD83D\uDC7E ";
                str = msg.GetTextForDisplay();
                resultArray = msg.GetRichTextFormattings();
                break;
              }
              fmt = AppResources.PushGif;
              break;
            case FunXMPP.FMessage.Type.LiveLocation:
              if (msg.IsCoordinateLocation())
              {
                fmt = AppResources.PushLocation;
                break;
              }
              fmt = "{0}: \uD83D\uDCCD ";
              str = msg.LocationDetails;
              break;
            case FunXMPP.FMessage.Type.Sticker:
              fmt = AppResources.PushSticker;
              break;
            default:
              fmt = "{0}: ";
              str = msg.GetTextForDisplay();
              resultArray = msg.GetRichTextFormattings();
              break;
          }
          if (fmt != null)
            forMessage.Header = NotificationString.RtlSafeFormat(fmt, senderDisplayName);
          forMessage.MessagePreview = str;
          forMessage.PreviewFormattings = resultArray;
        }
        else
          forMessage.Header = NotificationString.RtlSafeFormat(AppResources.PushMessageNoPreview, senderDisplayName);
      }
      if (forMessage.Header != null)
        forMessage.Header = Emoji.ConvertToUnicode(forMessage.Header);
      if (forMessage.MessagePreview != null)
      {
        int num = 63;
        List<LinkDetector.Result> source = new List<LinkDetector.Result>();
        if (forMessage.PreviewFormattings != null && ((IEnumerable<LinkDetector.Result>) forMessage.PreviewFormattings).Any<LinkDetector.Result>())
        {
          foreach (LinkDetector.Result previewFormatting in forMessage.PreviewFormattings)
          {
            LinkDetector.Result result = new LinkDetector.Result(previewFormatting);
            source.Add(result);
            if (result.Index < 63 && result.Index + result.Length >= 63)
            {
              if (result.type == 2 || result.type == 256)
              {
                num = result.Index;
                break;
              }
              result.Length = 63 - result.Index;
              break;
            }
          }
        }
        string str;
        if (num < forMessage.MessagePreview.Length)
        {
          str = NotificationString.ReplaceNewlines(Utils.TruncateAtIndex(forMessage.MessagePreview, num)) + "…";
          foreach (LinkDetector.Result result in source)
            result.OriginalStr = str;
          source.Add(new LinkDetector.Result(num, 1, 0, new string(new char[1]
          {
            '…'
          })));
        }
        else
        {
          str = NotificationString.ReplaceNewlines(forMessage.MessagePreview);
          foreach (LinkDetector.Result result in source)
            result.OriginalStr = str;
        }
        forMessage.MessagePreview = str;
        forMessage.PreviewFormattings = source.Any<LinkDetector.Result>() ? source.ToArray() : (LinkDetector.Result[]) null;
      }
      return forMessage;
    }

    private static string ProcessSystemMessage(Message msg)
    {
      return msg.GetSystemMessage(usePushName: true);
    }

    public static string ReplaceNewlines(string str) => str?.Replace('\n', ' ').Replace('\r', ' ');

    public static string[] GetTwoLineForMessage(Message msg, bool showPreview)
    {
      List<string> source = new List<string>();
      string senderDisplayName = msg.GetSenderDisplayName(true);
      if (JidHelper.IsGroupJid(msg.KeyRemoteJid))
      {
        Conversation convo = (Conversation) null;
        MessagesContext.RunRecursive((MessagesContext.MessagesCallback) (db => convo = db.GetConversation(msg.KeyRemoteJid, CreateOptions.None)));
        string str = convo == null || string.IsNullOrEmpty(convo.GroupSubject) ? "group" : convo.GroupSubject;
        if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(senderDisplayName) && msg.MediaWaType != FunXMPP.FMessage.Type.System)
          source.Add(NotificationString.RtlSafeFormat("{0} @ \"{1}\"", senderDisplayName, str));
        else
          source.Add(str);
      }
      else
        source.Add(senderDisplayName);
      string str1;
      if (showPreview || msg.MediaWaType == FunXMPP.FMessage.Type.System)
      {
        str1 = TileHelper.FormatSecondaryTileContent(msg);
        if (string.IsNullOrEmpty(str1))
          return (string[]) null;
      }
      else
        str1 = NotificationString.RtlSafeFormat(AppResources.PushMessageNoPreview, senderDisplayName);
      source.Add(str1);
      return source.Select<string, string>((Func<string, string>) (str => NotificationString.ReplaceNewlines(Emoji.ConvertToTextOnly(str, (byte[]) null)))).ToArray<string>();
    }

    public class MessageNotificationContent
    {
      private string headerStr;
      private string previewStr;

      public string Header
      {
        get => this.headerStr ?? "";
        set => this.headerStr = value;
      }

      public string MessagePreview
      {
        get => this.previewStr ?? "";
        set => this.previewStr = value;
      }

      public LinkDetector.Result[] PreviewFormattings { get; set; }
    }
  }
}
