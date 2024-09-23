// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatPageHelper
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace WhatsApp
{
  public class ChatPageHelper
  {
    private static string lastJidForMaybeAskForContactDetails;

    public static void SaveDraft(Conversation convo, string draft)
    {
      if (convo == null)
        return;
      if (string.IsNullOrEmpty(draft))
        draft = (string) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        bool flag = false;
        if (db.GetConversation(convo.Jid, CreateOptions.None) == null && (convo.Status.HasValue || convo.LastMessageID.HasValue || draft != null))
        {
          convo.Timestamp = new DateTime?(FunRunner.CurrentServerTimeUtc);
          db.InsertConversationOnSubmit(convo);
          flag = true;
        }
        if (convo.ComposingText != draft)
        {
          convo.ComposingText = draft;
          flag = true;
        }
        if (!flag)
          return;
        db.SubmitChanges();
      }));
    }

    public static WallpaperPanel CreateWallpaperPanel()
    {
      WallpaperPanel wallpaperPanel = new WallpaperPanel(false);
      wallpaperPanel.Margin = new Thickness(-2.0);
      wallpaperPanel.CacheMode = (CacheMode) new BitmapCache();
      wallpaperPanel.IsHitTestVisible = false;
      wallpaperPanel.RenderTransform = (Transform) new TranslateTransform()
      {
        Y = 0.0
      };
      return wallpaperPanel;
    }

    public static bool EnsureCurrentChatPageChild(FrameworkElement child)
    {
      ChatPage current = ChatPage.Current;
      if (current == null)
        return false;
      if (child.Parent != null)
        Log.d("EnsureChild", "Child already parented");
      UIElementCollection children = current.LayoutRoot.Children;
      if (!children.Contains((UIElement) child))
        children.Add((UIElement) child);
      return true;
    }

    public static void MaybeAskForContactDetails(string jid)
    {
      if (ChatPageHelper.lastJidForMaybeAskForContactDetails == jid)
      {
        ChatPageHelper.lastJidForMaybeAskForContactDetails = (string) null;
      }
      else
      {
        ChatPageHelper.lastJidForMaybeAskForContactDetails = jid;
        if (!JidHelper.IsUserJid(jid))
          return;
        UserStatus user = UserCache.Get(jid, false);
        if (user == null || user.IsVerified())
          return;
        bool hasMessages = false;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => hasMessages = db.AnyMessages(jid, true)));
        if (hasMessages)
          return;
        AppState.Worker.Enqueue((Action) (() => UsyncQueryRequest.SendForDetailsForJid(jid)));
      }
    }
  }
}
