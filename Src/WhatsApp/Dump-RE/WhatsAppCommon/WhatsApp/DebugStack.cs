// Decompiled with JetBrains decompiler
// Type: WhatsApp.DebugStack
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace WhatsApp
{
  public static class DebugStack
  {
    private static LinkedList<Func<Action, Action>> filters = new LinkedList<Func<Action, Action>>();
    private static object filtersLock = new object();

    public static IDisposable ApplyFilter(Func<Action, Action> filter)
    {
      LinkedListNode<Func<Action, Action>> node = (LinkedListNode<Func<Action, Action>>) null;
      lock (DebugStack.filtersLock)
        node = DebugStack.filters.AddLast(filter);
      return (IDisposable) new DisposableAction((Action) (() =>
      {
        if (node == null)
          return;
        lock (DebugStack.filtersLock)
        {
          if (node == null)
            return;
          DebugStack.filters.Remove(node);
          node = (LinkedListNode<Func<Action, Action>>) null;
        }
      }));
    }
  }
}
