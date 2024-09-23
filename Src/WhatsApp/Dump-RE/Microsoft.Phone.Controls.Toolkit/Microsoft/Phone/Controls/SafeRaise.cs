// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.SafeRaise
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;

#nullable disable
namespace Microsoft.Phone.Controls
{
  internal static class SafeRaise
  {
    public static void Raise(EventHandler eventToRaise, object sender)
    {
      if (eventToRaise == null)
        return;
      eventToRaise(sender, EventArgs.Empty);
    }

    public static void Raise(EventHandler<EventArgs> eventToRaise, object sender)
    {
      SafeRaise.Raise<EventArgs>(eventToRaise, sender, EventArgs.Empty);
    }

    public static void Raise<T>(EventHandler<T> eventToRaise, object sender, T args) where T : EventArgs
    {
      if (eventToRaise == null)
        return;
      eventToRaise(sender, args);
    }

    public static void Raise<T>(
      EventHandler<T> eventToRaise,
      object sender,
      SafeRaise.GetEventArgs<T> getEventArgs)
      where T : EventArgs
    {
      if (eventToRaise == null)
        return;
      eventToRaise(sender, getEventArgs());
    }

    public delegate T GetEventArgs<T>() where T : EventArgs;
  }
}
