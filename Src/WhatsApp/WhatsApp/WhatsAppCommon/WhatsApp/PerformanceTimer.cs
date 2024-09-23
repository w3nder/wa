// Decompiled with JetBrains decompiler
// Type: WhatsApp.PerformanceTimer
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Text;


namespace WhatsApp
{
  public sealed class PerformanceTimer
  {
    public static DateTime? Start(PerformanceTimer.Mode mode = PerformanceTimer.Mode.DebugOnly)
    {
      return mode != PerformanceTimer.Mode.All && mode != PerformanceTimer.Mode.DebugAndBeta ? new DateTime?() : new DateTime?(DateTime.Now);
    }

    public static void End(string header, string body, DateTime? start)
    {
      if (!start.HasValue)
        return;
      Log.l(header, "{0} | {1} ms", (object) body, (object) (PerformanceTimer.Convert(DateTime.Now) - PerformanceTimer.Convert(start.Value)));
    }

    public static void End(string summary, DateTime? start)
    {
      if (!start.HasValue)
        return;
      Log.l(summary, "{0} ms", (object) (PerformanceTimer.Convert(DateTime.Now) - PerformanceTimer.Convert(start.Value)));
    }

    public static int EndMs(DateTime? start)
    {
      return !start.HasValue ? -1 : PerformanceTimer.Convert(DateTime.Now) - PerformanceTimer.Convert(start.Value);
    }

    public static string EndString(string summary, DateTime? start)
    {
      return string.Format("{0} = {1} ms", (object) summary, start.HasValue ? (object) (PerformanceTimer.Convert(DateTime.Now) - PerformanceTimer.Convert(start.Value)).ToString() : (object) "n/a");
    }

    private static int Convert(DateTime dt) => (dt.Minute * 60 + dt.Second) * 1000 + dt.Millisecond;

    public static void LogIfExcessive(DateTime? start, string description)
    {
      if (PerformanceTimer.EndMs(start) <= 1000)
        return;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(description);
      string stackTrace = AppState.GetStackTrace();
      if (stackTrace != null)
      {
        stringBuilder.Append(" Stack:\n");
        stringBuilder.Append(stackTrace);
      }
      Log.WriteLineDebug(stringBuilder.ToString());
    }

    public enum Mode
    {
      DebugOnly,
      DebugAndBeta,
      All,
    }
  }
}
