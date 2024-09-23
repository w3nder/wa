// Decompiled with JetBrains decompiler
// Type: WhatsApp.TileHelper
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace WhatsApp
{
  public static class TileHelper
  {
    private const string LogHeader = "tilehelper";
    public const int TargetTilePicturePixelSize = 256;

    public static void IncrementTiles(
      MessagesContext db,
      TileDataSource msg,
      int inc = 1,
      bool isChatMuted = false)
    {
      if (msg == null)
        return;
      TileHelper.SetMainTile(db, new int?(inc), msg, isChatMuted);
      Conversation conversation = (Conversation) null;
      if (msg.GetContents != null && msg.Jid != null && (conversation = db.GetConversation(msg.Jid, CreateOptions.None)) != null)
      {
        conversation.UnreadTileCount += (long) inc;
        db.SubmitChanges();
        Log.d("tilehelper", "unread tile count | -> {0} | jid:{1}", (object) conversation.UnreadTileCount, (object) conversation.Jid);
      }
      ITile chatTile;
      if (msg.GetSecondaryContents == null || msg.Jid == null || (chatTile = TileHelper.GetChatTile(msg.Jid)) == null)
        return;
      int unreadMessagesCount = conversation == null ? 0 : conversation.GetUnreadMessagesCount();
      chatTile.SetChatCountAndContent(unreadMessagesCount, msg);
    }

    public static string FormatSecondaryTileContent(Message msg)
    {
      string str1 = msg.GetPreviewText(false, true);
      string str2 = (string) null;
      switch (msg.MediaWaType)
      {
        case FunXMPP.FMessage.Type.Image:
          str2 = "\uD83D\uDCF7";
          break;
        case FunXMPP.FMessage.Type.Video:
          str2 = "\uD83C\uDFA5";
          break;
        case FunXMPP.FMessage.Type.Location:
        case FunXMPP.FMessage.Type.LiveLocation:
          str2 = "\uD83D\uDCCD";
          break;
        case FunXMPP.FMessage.Type.Gif:
          str2 = "\uD83D\uDC7E";
          break;
        case FunXMPP.FMessage.Type.Sticker:
          str2 = "\uD83D\uDC9F";
          break;
        case FunXMPP.FMessage.Type.Revoked:
          str2 = "\uD83D\uDEAB";
          break;
      }
      if (str2 != null)
        str1 = NotificationString.RtlSafeFormat("{0} {1}", str2, str1);
      return str1;
    }

    public static void ClearMainTile()
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        db.ClearUnreadTileCounts();
        TileHelper.SetMainTile(db, new int?(), (TileDataSource) null);
        db.SubmitChanges();
      }));
      PushSystem.Instance.ClearToastHistoryGroup((string) null);
    }

    public static void DecrementMainTileCount(MessagesContext db, int decrement)
    {
      Log.d("tilehelper", "decrement main tile | decrement:{0}", (object) decrement);
      if (PushSystem.Instance.PrimaryTile == null)
        return;
      if (decrement < 0)
        decrement = 0;
      TileHelper.SetMainTile(db, new int?(-decrement), (TileDataSource) null);
    }

    private static TileDataSource GetSourceForPrimaryTile(MessagesContext db)
    {
      Dictionary<DateTime, Func<TileDataSource>> seq = new Dictionary<DateTime, Func<TileDataSource>>();
      Message msg = db.GetMessageForPrimaryTile();
      if (msg != null)
        seq.Add((msg.FunTimestamp ?? msg.LocalTimestamp) ?? DateTime.UtcNow, (Func<TileDataSource>) (() => TileDataSource.CreateForMessage(msg)));
      int missedCallCount = CallLog.GetMissedCallCount(Settings.LastSeenMissedCallTimeUtc);
      if (missedCallCount > 0)
      {
        CallRecord[] callRecordArray = CallLog.Load(count: new int?(missedCallCount), result: new CallRecord.CallResult?(CallRecord.CallResult.Missed));
        if (callRecordArray != null)
        {
          foreach (CallRecord callRecord in callRecordArray)
          {
            string jid = callRecord.PeerJid;
            seq[callRecord.StartTime] = (Func<TileDataSource>) (() =>
            {
              string displayName = (string) null;
              ContactsContext.Instance((Action<ContactsContext>) (cdb => displayName = cdb.GetUserStatus(jid).GetDisplayName()));
              return TileDataSource.CreateForMissedCall(jid, displayName);
            });
          }
        }
      }
      return seq.Count > 0 ? seq.MaxOfFunc<KeyValuePair<DateTime, Func<TileDataSource>>, DateTime>((Func<KeyValuePair<DateTime, Func<TileDataSource>>, DateTime>) (kv => kv.Key)).Value() : (TileDataSource) null;
    }

    private static void SetMainTile(
      MessagesContext db,
      int? tileCountDelta,
      TileDataSource tileMsg,
      bool isChatMuted = false)
    {
      ITile primaryTile = PushSystem.Instance.PrimaryTile;
      if (primaryTile == null)
        return;
      int num1 = NonDbSettings.LocalTileCount;
      if (num1 == -1)
      {
        int localTileCount = Settings.LocalTileCount;
        num1 = Math.Max(localTileCount, 0);
        if (localTileCount != 0)
        {
          Log.l("tilehelper", "Migrating tile count value {0}", (object) localTileCount);
          Settings.LocalTileCount = 0;
        }
      }
      int num2 = tileCountDelta.HasValue ? num1 + tileCountDelta.Value : 0;
      if (num2 < 0)
      {
        Log.SendCrashLog((Exception) new InvalidOperationException("negative main tile"), string.Format("negative main tile: {0} -> {1}", (object) num1, (object) num2), logOnlyForRelease: true);
        int num3 = num2;
        num2 = (int) db.GetUnreadTileCountsSum();
        Log.l("tilehelper", "re-calc main tile count | curr:{0},target:{1},new target:{2}", (object) num1, (object) num3, (object) num2);
        if (num2 > 0)
          tileMsg = TileHelper.GetSourceForPrimaryTile(db);
      }
      if (num2 > 0)
      {
        primaryTile.SetCount(new int?(num2));
        if (tileMsg != null)
        {
          int? nullable = tileCountDelta;
          int num4 = 0;
          if ((nullable.GetValueOrDefault() == num4 ? (nullable.HasValue ? 1 : 0) : 0) == 0 || isChatMuted)
            goto label_11;
        }
        tileMsg = TileHelper.GetSourceForPrimaryTile(db);
label_11:
        if (tileMsg != null)
          primaryTile.SetWideContent((IEnumerable<string>) tileMsg.GetContents());
      }
      else
        primaryTile.Clear();
      primaryTile.Update();
      Log.d("tilehelper", "set main tile | {0} -> {1} | msg:{2}", (object) num1, (object) num2, tileMsg == null ? (object) "n/a" : (object) tileMsg.LogIdentifier);
      NonDbSettings.LocalTileCount = num2;
    }

    public static void DecrementChatTile(MessagesContext db, string jid, int decrement)
    {
      Log.d("tilehelper", "decrement chat tile | jid:{0},decrement:{1}", (object) jid, (object) decrement);
      int num = 0;
      Conversation conversation = db.GetConversation(jid, CreateOptions.None);
      if (conversation != null)
      {
        int unreadTileCount = (int) conversation.UnreadTileCount;
        if (unreadTileCount > 0)
        {
          if (decrement < 0)
            decrement = 0;
          else if (decrement > unreadTileCount)
            decrement = unreadTileCount;
          num = unreadTileCount - decrement;
        }
      }
      if (num > 0)
      {
        ITile chatTile = TileHelper.GetChatTile(jid);
        if (chatTile == null || conversation == null)
          return;
        conversation.UnreadTileCount = (long) num;
        db.SubmitChanges();
        TileHelper.DecrementMainTileCount(db, decrement);
        chatTile.SetCount(new int?(num));
        chatTile.Update();
      }
      else
        TileHelper.ClearChatTile(db, jid);
    }

    public static void ClearChatTile(MessagesContext db, string jid)
    {
      Log.d("tilehelper", "clear chat tile | jid:{0}", (object) jid);
      string str = (string) null;
      Conversation conversation = db.GetConversation(jid, CreateOptions.None);
      if (conversation != null)
      {
        str = conversation.ConversationID.ToString();
        if (conversation.UnreadTileCount > 0L)
        {
          int unreadTileCount = (int) conversation.UnreadTileCount;
          conversation.UnreadTileCount = 0L;
          db.SubmitChanges();
          TileHelper.DecrementMainTileCount(db, unreadTileCount);
        }
      }
      ITile chatTile = TileHelper.GetChatTile(jid);
      if (chatTile != null)
      {
        chatTile.Clear();
        string name = conversation.GetName(true);
        if (AppState.UseWindowsNotificationService)
          chatTile.SetTitle("", true);
        else
          chatTile.SetTitle(name ?? "", true);
        chatTile.SetTitle(name ?? "");
        chatTile.SetWideContent("WhatsApp");
        chatTile.Update();
      }
      PushSystem.Instance.ClearToastHistoryGroup(str ?? "NULL");
    }

    public static void SetChatCountAndContent(this ITile tile, int count, TileDataSource m)
    {
      string title = m.GetSenderDisplayName();
      if (title != null)
        tile.SetTitle(title, true);
      tile.SetCount(new int?(count));
      tile.SetWideContent(m.GetSecondaryContents());
      tile.Update();
    }

    public static void CreateChatTile(
      string jid,
      string name,
      string chatPicPath,
      int initialCount,
      Message initialMessage)
    {
      if (TileHelper.ChatTileExists(jid))
        return;
      Uri backgroundImage = (Uri) null;
      Uri smallBackgroundImage = (Uri) null;
      string uriString = WaUris.ChatPageStr(WaUriParams.ForChatPage(jid, "SecondaryTile", false));
      bool flag = true;
      if (chatPicPath != null)
      {
        string chatTilePictureFile1 = TileHelper.CreateChatTilePictureFile(jid, chatPicPath, false);
        string chatTilePictureFile2 = TileHelper.CreateChatTilePictureFile(jid, chatPicPath, true);
        if (chatTilePictureFile1 != null)
        {
          flag = false;
          backgroundImage = new Uri(string.Format("isostore:{0}", (object) chatTilePictureFile1), UriKind.Absolute);
          if (chatTilePictureFile2 != null)
            smallBackgroundImage = new Uri(string.Format("isostore:{0}", (object) chatTilePictureFile2), UriKind.Absolute);
        }
      }
      if (flag)
        backgroundImage = new Uri(JidHelper.IsMultiParticipantsChatJid(jid) ? "Images/group_w_173.png" : "Images/contact_w_173.png", UriKind.Relative);
      try
      {
        PushSystem.ForegroundInstance.CreateTile(jid, Emoji.ConvertToTextOnly(name, (byte[]) null), initialCount, initialMessage != null ? TileHelper.FormatSecondaryTileContent(initialMessage) : "WhatsApp", new Uri(uriString, UriKind.Relative), backgroundImage, smallBackgroundImage);
      }
      catch (Exception ex)
      {
        string context = string.Format("create conversation tile for {0}", (object) jid);
        Log.SendCrashLog(ex, context);
      }
    }

    public static bool ChatTileExists(string jid)
    {
      return !string.IsNullOrEmpty(jid) && PushSystem.Instance.SecondaryTileExists((Func<Uri, bool>) (uri => TileHelper.UriToJid(uri) == jid), jid);
    }

    public static bool ChatTileExists() => PushSystem.Instance.SecondaryTileExists();

    public static ITile GetChatTile(string jid)
    {
      return string.IsNullOrEmpty(jid) ? (ITile) null : PushSystem.Instance.GetSecondaryTile((Func<Uri, bool>) (uri => TileHelper.UriToJid(uri) == jid), jid);
    }

    public static string UriToJid(Uri uri)
    {
      string jid = (string) null;
      UriUtils.ParsePageParams(uri).TryGetValue("jid", out jid);
      return jid;
    }

    public static void UpdateChatTilePicture(ITile tile, string jid, ImageSource chatPic)
    {
      if (tile == null)
        return;
      if (jid == null)
        return;
      try
      {
        string chatTilePictureFile1 = chatPic == null ? (string) null : TileHelper.CreateChatTilePictureFile(jid, chatPic, false);
        Uri uri1 = chatTilePictureFile1 != null ? new Uri(string.Format("isostore:{0}", (object) chatTilePictureFile1, (object) UriKind.Absolute)) : new Uri(JidHelper.IsMultiParticipantsChatJid(jid) ? "Images/group_w_173.png" : "Images/contact_w_173.png", UriKind.Relative);
        string chatTilePictureFile2 = chatPic == null ? (string) null : TileHelper.CreateChatTilePictureFile(jid, chatPic, true);
        Uri uri2 = chatTilePictureFile2 != null ? new Uri(string.Format("isostore:{0}", (object) chatTilePictureFile2, (object) UriKind.Absolute)) : new Uri(JidHelper.IsMultiParticipantsChatJid(jid) ? "Images/group_w_173.png" : "Images/contact_w_173.png", UriKind.Relative);
        tile.SetBackgroundImage(uri1);
        tile.SetSmallBackgroundImage(uri2);
        tile.Update();
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "tile pic update");
      }
    }

    public static void UpdateChatTilePicture(ITile tile, string jid, string chatPicPath)
    {
      if (tile == null)
        return;
      if (jid == null)
        return;
      try
      {
        string chatTilePictureFile1 = TileHelper.CreateChatTilePictureFile(jid, chatPicPath, false);
        string chatTilePictureFile2 = TileHelper.CreateChatTilePictureFile(jid, chatPicPath, true);
        if (chatTilePictureFile1 == null)
        {
          TileHelper.UpdateChatTilePicture(tile, jid, (ImageSource) null);
        }
        else
        {
          tile.SetSmallBackgroundImage(new Uri(string.Format("isostore:{0}", (object) chatTilePictureFile2, (object) UriKind.Absolute)));
          tile.SetBackgroundImage(new Uri(string.Format("isostore:{0}", (object) chatTilePictureFile1, (object) UriKind.Absolute)));
          tile.Update();
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "tile pic update");
      }
    }

    private static string GetChatTilePictureFilepath(string jid)
    {
      return string.Format("/Shared/ShellContent/tile_{0}", (object) jid);
    }

    private static string GetSmallChatTilePictureFilepath(string jid)
    {
      return string.Format("/Shared/ShellContent/tilesmall_{0}", (object) jid);
    }

    public static string CreateChatTilePictureFile(
      string jid,
      ImageSource chatPicSrc,
      bool isSmall)
    {
      if (chatPicSrc == null || string.IsNullOrEmpty(jid))
        return (string) null;
      int num = 256;
      Image image = new Image();
      image.Source = chatPicSrc;
      image.Width = (double) num;
      image.Height = (double) num;
      image.Stretch = Stretch.UniformToFill;
      Image element1 = image;
      GradientStopCollection gradientStopCollection = new GradientStopCollection();
      if (!isSmall)
      {
        gradientStopCollection.Add(new GradientStop()
        {
          Color = Color.FromArgb((byte) 0, (byte) 0, (byte) 0, (byte) 0),
          Offset = 0.5
        });
        gradientStopCollection.Add(new GradientStop()
        {
          Color = Color.FromArgb((byte) 102, (byte) 0, (byte) 0, (byte) 0),
          Offset = 1.0
        });
      }
      Rectangle rectangle = new Rectangle();
      rectangle.Width = (double) num;
      rectangle.Height = (double) num;
      LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
      linearGradientBrush.StartPoint = new System.Windows.Point(0.5, 0.0);
      linearGradientBrush.EndPoint = new System.Windows.Point(0.5, 1.0);
      linearGradientBrush.GradientStops = gradientStopCollection;
      rectangle.Fill = (Brush) linearGradientBrush;
      Rectangle element2 = rectangle;
      WriteableBitmap bitmap = new WriteableBitmap((UIElement) element1, (Transform) null);
      bitmap.Render((UIElement) element2, (Transform) null);
      bitmap.Invalidate();
      element1.Source = (ImageSource) null;
      MemoryStream jpegStream = bitmap.ToJpegStream(-1, new int?(80));
      if (jpegStream == null)
        return (string) null;
      using (jpegStream)
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          string tilePictureFilepath = TileHelper.GetChatTilePictureFilepath(jid);
          if (isSmall)
            tilePictureFilepath = TileHelper.GetSmallChatTilePictureFilepath(jid);
          if (storeForApplication.FileExists(tilePictureFilepath))
            storeForApplication.DeleteFile(tilePictureFilepath);
          using (IsolatedStorageFileStream destination = storeForApplication.OpenFile(tilePictureFilepath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
            jpegStream.CopyTo((Stream) destination);
          if (storeForApplication.FileExists(tilePictureFilepath))
            return tilePictureFilepath;
        }
      }
      return (string) null;
    }

    public static string CreateChatTilePictureFile(string jid, string chatPicPath, bool isSmall)
    {
      if (string.IsNullOrEmpty(jid) || string.IsNullOrEmpty(chatPicPath))
        return (string) null;
      WriteableBitmap chatPicSrc = BitmapUtils.LoadFromFile(chatPicPath, 256, 256);
      return TileHelper.CreateChatTilePictureFile(jid, (ImageSource) chatPicSrc, isSmall);
    }
  }
}
