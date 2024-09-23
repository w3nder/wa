// Decompiled with JetBrains decompiler
// Type: WhatsApp.PhoneAccessibilitySettings
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.ViewManagement;

#nullable disable
namespace WhatsApp
{
  public static class PhoneAccessibilitySettings
  {
    public static double GetTextSize()
    {
      Dictionary<uint, double> textSizes = Constants.TextSizes;
      double num1 = textSizes.Values.Min();
      if (AppState.IsWP10OrLater)
      {
        double textScaleFactor = new UISettings().TextScaleFactor;
        double num2 = textSizes.Values.Min();
        return (textSizes.Values.Max() - num2) * (textScaleFactor - 1.0) + num2;
      }
      uint key = textSizes.Keys.First<uint>();
      try
      {
        key = AppState.RegHelper.ReadDWord(2147483650U, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Control Panel\\Theme", "FontScale");
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "reading fontscale reg key");
      }
      return !textSizes.TryGetValue(key, out num1) ? 22.0 : num1;
    }
  }
}
