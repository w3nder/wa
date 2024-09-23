// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.ClearAllChatPicker
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;


namespace WhatsApp.CommonOps
{
  public static class ClearAllChatPicker
  {
    public static IObservable<Unit> Launch(bool deleteChats)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        ListPickerPage.Start(new string[2]
        {
          deleteChats ? AppResources.DeleteAllExceptStarred : AppResources.ClearAllExceptStarred,
          deleteChats ? AppResources.DeleteAllMessages : AppResources.ClearAllMessages
        }, title: deleteChats ? AppResources.DeleteAll : AppResources.ClearAll).ObserveOnDispatcher<int>().Subscribe<int>((Action<int>) (i =>
        {
          switch (i)
          {
            case 0:
              Observable.Return<bool>(true).Decision(deleteChats ? AppResources.DeleteAllConfirm : AppResources.ClearAllConfirm).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (accept =>
              {
                if (!accept)
                  return;
                if (deleteChats)
                {
                  ClearChat.ClearAndDeleteAll(true);
                  FieldStats.ReportUiUsage(wam_enum_ui_usage_type.CHATS_ALL_DELETE);
                }
                else
                {
                  ClearChat.ClearAll(true);
                  FieldStats.ReportUiUsage(wam_enum_ui_usage_type.CHATS_ALL_CLEAR);
                }
              }));
              break;
            case 1:
              Observable.Return<bool>(true).Decision(deleteChats ? AppResources.DeleteAllConfirm : AppResources.ClearAllConfirm).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (accept =>
              {
                if (!accept)
                  return;
                if (deleteChats)
                {
                  ClearChat.ClearAndDeleteAll(false);
                  FieldStats.ReportUiUsage(wam_enum_ui_usage_type.CHATS_ALL_DELETE);
                }
                else
                {
                  ClearChat.ClearAll(false);
                  FieldStats.ReportUiUsage(wam_enum_ui_usage_type.CHATS_ALL_CLEAR);
                }
              }));
              break;
          }
          observer.OnCompleted();
        }));
        return (Action) (() => { });
      }));
    }
  }
}
