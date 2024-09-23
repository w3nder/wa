// Decompiled with JetBrains decompiler
// Type: WhatsApp.IResolverExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using WhatsApp.Resolvers;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public static class IResolverExtensions
  {
    public static volatile int NumAttempts;
    public static volatile int NumSuccesses;
    public static volatile int NumFailures;

    public static void Resolve(
      this IResolver resolver,
      string host,
      Action<IEnumerable<ResolveResult>> onResults,
      Action onError)
    {
      string displayName = resolver.DisplayName;
      if (displayName != null)
        IResolverExtensions.InjectLogger(displayName, host, ref onResults, ref onError);
      IResolverExtensions.InjectRunOnce(ref onResults, ref onError);
      if (displayName != null && !AppState.IsBackgroundAgent)
        IResolverExtensions.InjectStats(ref onResults, ref onError);
      try
      {
        resolver.ResolveImpl(host, onResults, onError);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "resolver exception");
        onError();
      }
    }

    private static void InjectLogger(
      string dpyName,
      string host,
      ref Action<IEnumerable<ResolveResult>> onResults,
      ref Action onError)
    {
      Log.WriteLineDebug("Looking up {0} using resolver {1}", (object) host, (object) dpyName);
      Action innerError = onError;
      onError = (Action) (() =>
      {
        Log.WriteLineDebug("{0}: Host lookup failed", (object) dpyName);
        innerError();
      });
    }

    private static void InjectRunOnce(
      ref Action<IEnumerable<ResolveResult>> onResults,
      ref Action onError)
    {
      Action<Action> route = Utils.IgnoreMultipleInvokes<Action>((Action<Action>) (a => a()));
      Action<IEnumerable<ResolveResult>> innerRes = onResults;
      Action innerError = onError;
      onResults = (Action<IEnumerable<ResolveResult>>) (res => route((Action) (() => innerRes(res))));
      onError = (Action) (() => route(innerError));
    }

    private static void InjectStats(
      ref Action<IEnumerable<ResolveResult>> onResults,
      ref Action onError)
    {
      Action<IEnumerable<ResolveResult>> innerRes = onResults;
      Action innerError = onError;
      Interlocked.Increment(ref IResolverExtensions.NumAttempts);
      onResults = (Action<IEnumerable<ResolveResult>>) (r =>
      {
        Interlocked.Increment(ref IResolverExtensions.NumSuccesses);
        innerRes(r);
      });
      onError = (Action) (() =>
      {
        Interlocked.Increment(ref IResolverExtensions.NumFailures);
        innerError();
      });
    }

    public static IHostResolver ToNativeResolver(this IResolver resolver)
    {
      return (IHostResolver) new IResolverExtensions.NativeResolverThunk()
      {
        Resolver = resolver
      };
    }

    private class NativeResolverThunk : IHostResolver
    {
      public IResolver Resolver;

      public string Resolve(string host, bool shuffle)
      {
        string r = (string) null;
        using (ManualResetEvent ev = new ManualResetEvent(false))
        {
          this.Resolver.Resolve(host, (Action<IEnumerable<ResolveResult>>) (results =>
          {
            if (shuffle)
              results = results.Shuffle<ResolveResult>();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string str in results.Select<ResolveResult, string>((Func<ResolveResult, string>) (rs => rs.Address)))
            {
              stringBuilder.Append(str);
              stringBuilder.Append(char.MinValue);
            }
            stringBuilder.Append(char.MinValue);
            r = stringBuilder.ToString();
            ev.Set();
          }), (Action) (() => ev.Set()));
          ev.WaitOne();
        }
        return r != null ? r : throw new Exception("Could not find host " + host);
      }
    }
  }
}
