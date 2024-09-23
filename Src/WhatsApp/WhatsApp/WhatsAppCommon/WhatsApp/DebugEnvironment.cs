// Decompiled with JetBrains decompiler
// Type: WhatsApp.DebugEnvironment
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp
{
  public static class DebugEnvironment
  {
    private static readonly int KeepStackSettingsValue = NonDbSettings.KeepStackTrace;

    private static bool IsKeepStackTraceEnabled => DebugEnvironment.KeepStackSettingsValue == 1;

    public static string TryGetStackTrace()
    {
      return DebugEnvironment.IsKeepStackTraceEnabled ? AppState.GetStackTrace() : "Stack trace suppressed";
    }
  }
}
