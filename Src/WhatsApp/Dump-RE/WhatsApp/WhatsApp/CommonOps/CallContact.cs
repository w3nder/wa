// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.CallContact
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Reactive;
using System;
using System.Windows;
using WhatsAppNative;

#nullable disable
namespace WhatsApp.CommonOps
{
  public static class CallContact
  {
    public static void Call(string jid, bool replacePage = false, bool forceNavigation = false, string context = null)
    {
      Log.l("common ops", "voip call | jid:{0},context:{1}", (object) jid, (object) context);
      UserStatus user = UserCache.Get(jid, true);
      if (user == null)
        return;
      CallContact.VoipCall(user, false, replacePage, forceNavigation);
    }

    public static void VideoCall(
      string jid,
      bool replacePage = false,
      bool forceNavigation = false,
      string context = null)
    {
      Log.l("common ops", "video call | jid:{0},context:{1}", (object) jid, (object) context);
      UserStatus user = UserCache.Get(jid, true);
      if (user == null)
        return;
      CallContact.VoipCall(user, true, replacePage, forceNavigation);
    }

    private static void VoipCall(
      UserStatus user,
      bool useVideo,
      bool replacePage,
      bool forceNavigation)
    {
      if (user == null)
        Log.l("common ops", "voip call | null user");
      else if (!DeviceNetworkInformation.IsNetworkAvailable)
      {
        Log.l("common ops", "voip call | trying to call without network");
        int num = (int) MessageBox.Show(AppResources.CallNoNetworkConnection, AppResources.PlaceCallErrorTitle, MessageBoxButton.OK);
      }
      else
        BlockContact.PromptUnblockIfBlocked(user.Jid).Subscribe<bool>((Action<bool>) (notBlocked =>
        {
          if (notBlocked)
          {
            string messageId = FunXMPP.GenerateMessageId();
            Action startCallScreen = (Action) (() => Deployment.Current.Dispatcher.BeginInvoke((Action) (() => CallScreenPage.Launch(user.Jid, new UiCallState?(UiCallState.Calling), replacePage, useVideo, forceNavigation))));
            WhatsApp.Voip.StartCall(Settings.MyJid, user.Jid, messageId, user.GetDisplayName(), useVideo, startCallScreen);
          }
          else
            Log.l("common ops", "voip call | skipped");
        }));
    }
  }
}
