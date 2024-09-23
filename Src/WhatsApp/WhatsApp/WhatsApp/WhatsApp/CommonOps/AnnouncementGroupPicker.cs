// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.AnnouncementGroupPicker
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Windows;


namespace WhatsApp.CommonOps
{
  public static class AnnouncementGroupPicker
  {
    public static void Launch(string gjid, GlobalProgressIndicator progressIndicator)
    {
      string[] options = new string[2]
      {
        AppResources.AllParticipants,
        AppResources.AdminOnly
      };
      int selected = -1;
      Conversation convo = (Conversation) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        convo = db.GetConversation(gjid, CreateOptions.None);
        if (convo == null)
          return;
        selected = convo.IsAnnounceOnly() ? 1 : 0;
      }));
      if (convo == null)
      {
        Log.l("Announcement Only Picker", "OnNavigatedTo | no conversation found for gjid={0}", (object) gjid);
      }
      else
      {
        Action release = (Action) (() =>
        {
          release = (Action) (() => { });
          if (progressIndicator == null)
            return;
          Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => progressIndicator?.Release()));
        });
        Action<int> onError = (Action<int>) (i => release());
        string subtitle = Plurals.Instance.GetString(AppResources.SendMessagesDescriptionPlural, Settings.AnnouncementGroupToggleTimeHours);
        ListPickerPage.Start(options, selected, AppResources.SendMessagesTitle, subtitle: subtitle).ObserveOnDispatcher<int>().Subscribe<int>((Action<int>) (i =>
        {
          FunXMPP.Connection connection = App.CurrentApp.Connection;
          if (connection == null || selected == i || i == -1)
            return;
          progressIndicator?.Acquire();
          if (i == 0)
          {
            connection.SendSetAnnouncementOnlyGroup(gjid, false, release, onError);
          }
          else
          {
            if (i != 1)
              return;
            connection.SendSetAnnouncementOnlyGroup(gjid, true, release, onError);
          }
        }));
      }
    }
  }
}
