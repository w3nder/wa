// Decompiled with JetBrains decompiler
// Type: WhatsApp.PushSystem
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp
{
  public class PushSystem
  {
    public static MutexWithWatchdog PushLock = new MutexWithWatchdog("WhatsApp.PushLock", false);
    public static Subject<Unit> PushBoundSubject = new Subject<Unit>();
    private static IPushSystemForeground fgInstance;
    private static IPushSystem instance = (IPushSystem) null;

    public static IPushSystemForeground ForegroundInstance
    {
      get
      {
        if (PushSystem.fgInstance != null)
          return PushSystem.fgInstance;
        PushSystem.fgInstance = PushSystem.Instance is IPushSystemForeground instance ? instance : throw new Exception("PushSystem.ForegroundInstance used when uninitialized - too early or possibly not from UI app");
        return instance;
      }
      set => PushSystem.fgInstance = value;
    }

    public static IPushSystem Instance
    {
      get
      {
        return AppState.UseWindowsNotificationService ? Utils.LazyInit<IPushSystem>(ref PushSystem.instance, (Func<IPushSystem>) (() => (IPushSystem) new Wns())) : Utils.LazyInit<IPushSystem>(ref PushSystem.instance, (Func<IPushSystem>) (() => (IPushSystem) new Mpns()));
      }
    }

    public static IEnumerable<string> SanitizeWideContent(IEnumerable<string> strs)
    {
      return ((IEnumerable<string>) ((object) strs ?? (object) new string[0])).Select<string, string>((Func<string, string>) (str => str ?? ""));
    }
  }
}
