// Decompiled with JetBrains decompiler
// Type: WhatsApp.InternalSettings
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;

#nullable disable
namespace WhatsApp
{
  public class InternalSettings
  {
    private static bool showChagePageDebugInfo = true;
    private static bool showRedrawRegions = false;
    private static bool showFrameRate = false;
    private static bool showCacheVisualization = false;
    private static bool useLLSChatList = false;
    private static bool enableNewMessageAnimation = false;
    private static bool showManualMessageIndex = false;

    public static bool ShowChatPageDebugInfo
    {
      get
      {
        return InternalSettings.GetDebugOnly<bool>((Func<bool>) (() => InternalSettings.showChagePageDebugInfo), false);
      }
      set
      {
        InternalSettings.SetDebugOnly<bool>(ref InternalSettings.showChagePageDebugInfo, value);
      }
    }

    public static bool ShowRedrawRegions
    {
      get
      {
        return InternalSettings.GetDebugOnly<bool>((Func<bool>) (() => InternalSettings.showRedrawRegions), false);
      }
      set
      {
        InternalSettings.SetDebugOnly<bool>(ref InternalSettings.showRedrawRegions, value, (Action) (() => { }));
      }
    }

    public static bool ShowFrameRate
    {
      get
      {
        return InternalSettings.GetDebugOnly<bool>((Func<bool>) (() => InternalSettings.showFrameRate), false);
      }
      set
      {
        InternalSettings.SetDebugOnly<bool>(ref InternalSettings.showFrameRate, value, (Action) (() => { }));
      }
    }

    public static bool ShowCacheVisualization
    {
      get
      {
        return InternalSettings.GetDebugOnly<bool>((Func<bool>) (() => InternalSettings.showCacheVisualization), false);
      }
      set
      {
        InternalSettings.SetDebugOnly<bool>(ref InternalSettings.showCacheVisualization, value, (Action) (() => { }));
      }
    }

    public static bool UseLLSChatList
    {
      get
      {
        return InternalSettings.GetWaAdminOnly<bool>((Func<bool>) (() => InternalSettings.useLLSChatList), false);
      }
      set => InternalSettings.SetWaAdminOnly<bool>(ref InternalSettings.useLLSChatList, value);
    }

    public static bool EnableNewMessageAnimation
    {
      get
      {
        return InternalSettings.GetWaAdminOnly<bool>((Func<bool>) (() => InternalSettings.enableNewMessageAnimation), false);
      }
      set
      {
        InternalSettings.SetWaAdminOnly<bool>(ref InternalSettings.enableNewMessageAnimation, value);
      }
    }

    public static bool ShowManualMessageIndex
    {
      get
      {
        return InternalSettings.GetWaAdminOnly<bool>((Func<bool>) (() => InternalSettings.showManualMessageIndex), false);
      }
      set
      {
        InternalSettings.SetWaAdminOnly<bool>(ref InternalSettings.showManualMessageIndex, value);
      }
    }

    private static T GetDebugOnly<T>(Func<T> getter, T nonDebugVal) => nonDebugVal;

    private static void SetDebugOnly<T>(ref T target, T val, Action onSet = null)
    {
    }

    private static T GetWaAdminOnly<T>(Func<T> getter, T nonWaAdminVal)
    {
      T waAdminOnly = nonWaAdminVal;
      if (Settings.IsWaAdmin)
        waAdminOnly = getter();
      return waAdminOnly;
    }

    private static void SetWaAdminOnly<T>(ref T target, T val, Action onSet = null)
    {
      if (!Settings.IsWaAdmin)
        return;
      target = val;
      if (onSet == null)
        return;
      onSet();
    }
  }
}
