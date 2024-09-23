// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.SaveIncomingMediaPicker
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace WhatsApp.CommonOps
{
  public static class SaveIncomingMediaPicker
  {
    public static IObservable<Unit> Launch(string[] jids)
    {
      bool isSettingDefault = jids == null || !((IEnumerable<string>) jids).Any<string>();
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        List<string> stringList = new List<string>();
        int selectedIndex = -1;
        if (isSettingDefault)
        {
          selectedIndex = Settings.SaveIncomingMedia ? 0 : 1;
        }
        else
        {
          stringList.Add(Settings.SaveIncomingMedia ? AppResources.SaveIncomingMediaDefaultOn : AppResources.SaveIncomingMediaDefaultOff);
          JidInfo[] jidInfos = (JidInfo[]) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => jidInfos = ((IEnumerable<string>) jids).Select<string, JidInfo>((Func<string, JidInfo>) (jid => db.GetJidInfo(jid, CreateOptions.None))).Where<JidInfo>((Func<JidInfo, bool>) (ji => ji != null)).ToArray<JidInfo>()));
          bool flag = false;
          bool? firstState = new bool?();
          if (((IEnumerable<JidInfo>) jidInfos).Any<JidInfo>())
          {
            firstState = ((IEnumerable<JidInfo>) jidInfos).First<JidInfo>().SaveMediaToPhone;
            flag = jids.Length != jidInfos.Length || ((IEnumerable<JidInfo>) jidInfos).Skip<JidInfo>(1).Any<JidInfo>((Func<JidInfo, bool>) (ji =>
            {
              bool? saveMediaToPhone = ji.SaveMediaToPhone;
              bool? nullable = firstState;
              return saveMediaToPhone.GetValueOrDefault() != nullable.GetValueOrDefault() || saveMediaToPhone.HasValue != nullable.HasValue;
            }));
          }
          if (!flag)
            selectedIndex = !firstState.HasValue ? 0 : (firstState.Value ? 1 : 2);
        }
        stringList.Add(AppResources.SaveIncomingMediaYes);
        stringList.Add(AppResources.SaveIncomingMediaNo);
        ListPickerPage.Start(stringList.ToArray(), selectedIndex, AppResources.SaveIncomingMedia).ObserveOnDispatcher<int>().Subscribe<int>((Action<int>) (selected =>
        {
          bool? selectedState = new bool?();
          switch (selected)
          {
            case 0:
              selectedState = isSettingDefault ? new bool?(true) : new bool?();
              break;
            case 1:
              selectedState = new bool?(!isSettingDefault);
              break;
            case 2:
              selectedState = new bool?(false);
              break;
          }
          if (isSettingDefault)
          {
            if (!selectedState.HasValue)
              return;
            Settings.SaveIncomingMedia = selectedState.Value;
          }
          else
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              ((IEnumerable<string>) jids).Select<string, JidInfo>((Func<string, JidInfo>) (jid => db.GetJidInfo(jid, selectedState.HasValue ? CreateOptions.CreateToDbIfNotFound : CreateOptions.None))).Where<JidInfo>((Func<JidInfo, bool>) (ji => ji != null)).ToList<JidInfo>().ForEach((Action<JidInfo>) (ji => ji.SaveMediaToPhone = selectedState));
              db.SubmitChanges();
            }));
        }));
        observer.OnNext(new Unit());
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }
  }
}
