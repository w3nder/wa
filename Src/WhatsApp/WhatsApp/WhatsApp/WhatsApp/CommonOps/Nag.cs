// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.Nag
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;


namespace WhatsApp.CommonOps
{
  public static class Nag
  {
    public static void NagBatterySaver(string turnOffBatterySaverWording = null)
    {
      string str = AppResources.BatterySaverAlert;
      if (AppState.GetConnection() != null && AppState.GetConnection().EventHandler.Qr.Session.HasConnections)
        str = AppResources.BatterySaverAlertWithWeb;
      UIUtils.MessageBox(AppResources.GenericWarning, string.Format("{0}\n\n{1}", (object) str, (object) (turnOffBatterySaverWording ?? AppResources.TurnOffBatterySaverGeneric)), (IEnumerable<string>) new string[2]
      {
        AppResources.DismissButton,
        AppResources.Settings
      }, (Action<int>) (selectedButtonIndex =>
      {
        if (selectedButtonIndex != 1)
          return;
        NavUtils.NavigateExternal("ms-settings-power:");
      }));
    }
  }
}
