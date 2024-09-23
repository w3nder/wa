// Decompiled with JetBrains decompiler
// Type: WhatsApp.Storyboarder
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

#nullable disable
namespace WhatsApp
{
  public class Storyboarder
  {
    public static IDisposable PerformWithDisposable(
      Storyboard sb,
      DependencyObject target = null,
      bool shouldStop = true,
      Action onComplete = null,
      Action onDisposing = null,
      string context = null)
    {
      if (onComplete != null)
        onComplete = Storyboarder.WrapOnComplete(onComplete);
      if (onDisposing != null)
        onDisposing = Storyboarder.WrapOnComplete(onDisposing);
      Action raiseOnComplete = (Action) (() =>
      {
        if (onComplete == null)
          return;
        Action action = onComplete;
        onComplete = onDisposing = (Action) null;
        action();
      });
      Action raiseOnDisposing = (Action) (() =>
      {
        if (onDisposing == null)
          return;
        Action action = onDisposing;
        onComplete = onDisposing = (Action) null;
        action();
      });
      if (sb == null)
      {
        raiseOnComplete();
        return (IDisposable) null;
      }
      IDisposable completedSub = (IDisposable) null;
      Action<bool, Action> cleanup = (Action<bool, Action>) ((shouldStopSb, cleanUpAct) =>
      {
        completedSub.SafeDispose();
        completedSub = (IDisposable) null;
        if (shouldStopSb)
        {
          try
          {
            sb.Stop();
          }
          catch (Exception ex)
          {
          }
        }
        if (cleanUpAct == null)
          return;
        cleanUpAct();
      });
      try
      {
        sb.Stop();
        if (target != null)
          Storyboard.SetTarget((Timeline) sb, target);
        completedSub = sb.GetCompletedAsync().Take<Unit>(1).Subscribe<Unit>((Action<Unit>) (_ => cleanup(shouldStop, raiseOnComplete)));
        sb.Begin();
      }
      catch (Exception ex)
      {
        cleanup(true, raiseOnComplete);
        string context1 = "ignored storyboard exception: " + context;
        Log.SendCrashLog(ex, context1);
      }
      return (IDisposable) new DisposableAction((Action) (() => cleanup(true, raiseOnDisposing)));
    }

    public static IDisposable PerformWithDisposable(
      Storyboard sb,
      DependencyObject target = null,
      bool shouldStop = true,
      Action onComplete = null,
      bool callOnCompleteOnDisposing = false,
      string context = null)
    {
      onComplete = Storyboarder.WrapOnComplete(onComplete);
      return Storyboarder.PerformWithDisposable(sb, target, shouldStop, onComplete, callOnCompleteOnDisposing ? onComplete : (Action) null, context);
    }

    public static Storyboard Perform(
      string name,
      DependencyObject target,
      bool shouldStop = true,
      Action onComplete = null)
    {
      return Storyboarder.Perform(Application.Current.Resources, name, target, shouldStop, onComplete);
    }

    public static Storyboard Perform(
      ResourceDictionary dict,
      string name,
      DependencyObject target,
      bool shouldStop = true,
      Action onComplete = null)
    {
      return Storyboarder.Perform(dict[(object) name] as Storyboard, target, shouldStop, onComplete, name);
    }

    public static Storyboard Perform(string name, bool shouldStop = true, Action onComplete = null)
    {
      return Storyboarder.Perform(name, (DependencyObject) null, shouldStop, onComplete);
    }

    public static Storyboard Perform(
      ResourceDictionary dict,
      string name,
      bool shouldStop = true,
      Action onComplete = null)
    {
      return Storyboarder.Perform(dict, name, (DependencyObject) null, shouldStop, onComplete);
    }

    public static Storyboard Perform(
      Storyboard sb,
      DependencyObject target,
      bool shouldStop = true,
      Action onComplete = null,
      string name = null)
    {
      onComplete = Storyboarder.WrapOnComplete(onComplete);
      if (sb == null)
      {
        onComplete();
        return (Storyboard) null;
      }
      try
      {
        sb.Stop();
        if (target != null)
          Storyboard.SetTarget((Timeline) sb, target);
        sb.GetCompletedAsync().Take<Unit>(1).Subscribe<Unit>((Action<Unit>) (_ =>
        {
          if (shouldStop)
            sb.Stop();
          onComplete();
        }));
        sb.Begin();
      }
      catch (Exception ex1)
      {
        if (name == null)
          name = Application.Current.Resources.Where<KeyValuePair<object, object>>((Func<KeyValuePair<object, object>, bool>) (kv => kv.Value == sb && kv.Key is string)).Select<KeyValuePair<object, object>, object>((Func<KeyValuePair<object, object>, object>) (kv => kv.Key)).Cast<string>().FirstOrDefault<string>();
        try
        {
          sb.Stop();
        }
        catch (Exception ex2)
        {
        }
        onComplete();
        Log.SendCrashLog(ex1, "ignored storyboard exception: " + (name ?? "unnamed"));
      }
      return sb;
    }

    public static Storyboard Perform(
      Storyboard sb,
      bool shouldStop = true,
      Action onComplete = null,
      string name = null)
    {
      return Storyboarder.Perform(sb, (DependencyObject) null, shouldStop, onComplete, name);
    }

    private static Action WrapOnComplete(Action onComplete)
    {
      if (onComplete == null)
        return (Action) (() => { });
      Action snap = onComplete;
      onComplete = (Action) (() => Deployment.Current.Dispatcher.BeginInvokeIfNeeded(snap));
      return Utils.IgnoreMultipleInvokes(onComplete);
    }
  }
}
