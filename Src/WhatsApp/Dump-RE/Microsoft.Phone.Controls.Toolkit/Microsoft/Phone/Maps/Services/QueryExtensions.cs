// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Maps.Services.QueryExtensions
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Phone.Maps.Services
{
  public static class QueryExtensions
  {
    public static Task<IList<MapLocation>> GetMapLocationsAsync(this GeocodeQuery geocodeQuery)
    {
      return geocodeQuery.QueryAsync<IList<MapLocation>>();
    }

    public static Task<IList<MapLocation>> GetMapLocationsAsync(
      this ReverseGeocodeQuery reverseGeocodeQuery)
    {
      return reverseGeocodeQuery.QueryAsync<IList<MapLocation>>();
    }

    public static Task<Route> GetRouteAsync(this RouteQuery routeQuery)
    {
      return routeQuery.QueryAsync<Route>();
    }

    private static Task<TResult> QueryAsync<TResult>(this Query<TResult> query)
    {
      EventHandler<QueryCompletedEventArgs<TResult>> queryCompletedHandler = (EventHandler<QueryCompletedEventArgs<TResult>>) null;
      TaskCompletionSource<TResult> taskCompletionSource = QueryExtensions.CreateSource<TResult>((object) query);
      queryCompletedHandler = (EventHandler<QueryCompletedEventArgs<TResult>>) ((sender, e) => QueryExtensions.TransferCompletion<TResult>(taskCompletionSource, (AsyncCompletedEventArgs) e, (Func<TResult>) (() => e.Result), (Action) (() => query.QueryCompleted -= queryCompletedHandler)));
      query.QueryCompleted += queryCompletedHandler;
      try
      {
        query.QueryAsync();
      }
      catch
      {
        query.QueryCompleted -= queryCompletedHandler;
        throw;
      }
      return taskCompletionSource.Task;
    }

    private static TaskCompletionSource<TResult> CreateSource<TResult>(object state)
    {
      return new TaskCompletionSource<TResult>(state, TaskCreationOptions.None);
    }

    private static void TransferCompletion<TResult>(
      TaskCompletionSource<TResult> tcs,
      AsyncCompletedEventArgs e,
      Func<TResult> getResult,
      Action unregisterHandler)
    {
      if (e.UserState != tcs.Task.AsyncState)
        return;
      if (unregisterHandler != null)
        unregisterHandler();
      if (e.Cancelled)
        tcs.TrySetCanceled();
      else if (e.Error != null)
        tcs.TrySetException(e.Error);
      else
        tcs.TrySetResult(getResult());
    }
  }
}
